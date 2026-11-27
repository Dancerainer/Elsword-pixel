using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] class FeedbackSceneRequestV25 {
            public string id, stage;
            public int dungeon, room, surface = -1;
            public float x;
            public bool useEntry;
        }
        [Serializable] class FeedbackScenePlanV25 { public FeedbackSceneRequestV25[] requests; }
        [Serializable] class FeedbackSceneSampleV25 {
            public string id, stage, mode, image, state, nativeImage;
            public int dungeon, room, surface;
            public float playerX, playerY;
            public bool originalFraming, originalMaterialTreatment;
        }
        [Serializable] class FeedbackSceneReportV25 {
            public string schema = "feedback-v25-actual-scene-comparison-v1";
            public string checkedUtc, executable, catalogSha256, planSha256, error;
            public bool completed, actualWindowsPlayer;
            public string scope = "Paused production scenery/HUD renderer at recorded support points. Legacy is a presentation comparison, not an old mechanics build or a natural playthrough.";
            public FeedbackSceneSampleV25[] samples;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void StartFeedbackSceneReviewV25() {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--feedback-v25-review") < 0) return;
            var scene = FindObjectOfType<DemoScene>();
            if (!scene) throw new InvalidOperationException("Feedback review requires DemoScene.");
            scene.StartCoroutine(scene.RunFeedbackSceneReviewV25());
        }

        static string FeedbackDigestV25(byte[] bytes) {
            using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        void SaveFeedbackNativeBufferV25(string name) {
            if (!nativeTarget || !nativeTarget.IsCreated()) throw new InvalidOperationException("Missing native scenery buffer.");
            string path = Path.Combine(captureDir, name);
            if (File.Exists(path)) throw new InvalidOperationException("Preserve the existing native capture.");
            var previous = RenderTexture.active;
            var image = new Texture2D(nativeTarget.width, nativeTarget.height, TextureFormat.RGBA32, false);
            try {
                RenderTexture.active = nativeTarget;
                image.ReadPixels(new Rect(0, 0, nativeTarget.width, nativeTarget.height), 0, 0, false);
                image.Apply(false, false);
                File.WriteAllBytes(path, image.EncodeToPNG());
            } finally {
                RenderTexture.active = previous;
                Destroy(image);
            }
        }

        IEnumerator RunFeedbackSceneReviewV25() {
            Application.runInBackground = true;
            string planPath = Environment.GetEnvironmentVariable("EL_FEEDBACK_V25_PLAN");
            if (string.IsNullOrEmpty(planPath) || !File.Exists(planPath))
                throw new InvalidOperationException("An explicit feedback review plan is required.");
            byte[] planBytes = File.ReadAllBytes(planPath);
            var plan = JsonUtility.FromJson<FeedbackScenePlanV25>(System.Text.Encoding.UTF8.GetString(planBytes));
            if (plan == null || plan.requests == null || plan.requests.Length == 0)
                throw new InvalidOperationException("Feedback review plan has no requests.");
            Directory.CreateDirectory(captureDir);
            if (File.Exists(Path.Combine(captureDir, "feedback-v25-scenes.json")))
                throw new InvalidOperationException("Preserve the existing feedback capture.");
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var request in plan.requests) {
                if (request == null || string.IsNullOrEmpty(request.id) || !seen.Add(request.id)
                    || request.id.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                    || request.dungeon < 0 || request.dungeon >= catalog.dungeons.Length
                    || request.room < 0 || request.room >= catalog.dungeons[request.dungeon].count
                    || catalog.stages[catalog.dungeons[request.dungeon].start + request.room].id != request.stage)
                    throw new InvalidOperationException("Invalid feedback scene request.");
                foreach (string mode in new[] { "legacy", "wide", "quiet" })
                    if (File.Exists(Path.Combine(captureDir, request.id + "-" + mode + ".png")))
                        throw new InvalidOperationException("Preserve existing planned screenshots.");
            }
            var report = new FeedbackSceneReportV25 {
                checkedUtc = DateTime.UtcNow.ToString("o"),
                executable = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName,
                catalogSha256 = FeedbackDigestV25(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes),
                planSha256 = FeedbackDigestV25(planBytes), actualWindowsPlayer = !Application.isEditor
            };
            var samples = new List<FeedbackSceneSampleV25>();
            monsterArtReview = true;
            foreach (var request in plan.requests) {
                selectedDungeon = previewDungeon = request.dungeon; ConfigureRooms();
                typeof(GameModel).GetMethod("EnterRoom", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(game, new object[] { request.room });
                CancelBossIntroduction();
                if (!request.useEntry) {
                    var support = game.Surface(request.surface);
                    if (support == null || request.x < support.x1 || request.x > support.x2)
                        throw new InvalidOperationException("Missing planned player support.");
                    game.player.x = request.x; game.player.y = support.Y(request.x);
                    game.player.groundLine = support.id;
                }
                game.player.vx = game.player.vy = 0; game.player.state = CombatState.Normal;
                game.player.clip = "idle"; game.player.clock = 0; game.player.facing = 1;
                adventureStarted = true; openMenu = ""; pausedBeforeMenu = false;
                game.paused = true; game.roomBanner = 0;
                shownRoom = previousRoom = cameraFollowRoom = -1; accumulator = 0;
                ClearInput(); RebuildRoom(); RenderMenus(); hudCanvas.enabled = true;
                foreach (string mode in new[] { "legacy", "wide", "quiet" }) {
                    bool legacy = mode == "legacy", originalMaterial = mode != "quiet";
                    SetCameraFramingReviewOriginalV25(legacy);
                    SceneBackgroundStyleV25.CompareOriginalMaterials(originalMaterial);
                    // Let the ordinary render/projection/HUD code settle after
                    // a deliberate review switch; simulation remains paused.
                    for (int i = 0; i < 4; i++) yield return null;
                    yield return new WaitForEndOfFrame();
                    string name = request.id + "-" + mode;
                    Capture(name);
                    yield return new WaitForEndOfFrame();
                    SaveFeedbackNativeBufferV25(name + ".native.png");
                    float deadline = Time.realtimeSinceStartup + 8;
                    while (!File.Exists(Path.Combine(captureDir, name + ".png"))) {
                        if (Time.realtimeSinceStartup > deadline)
                            throw new InvalidOperationException("Screenshot did not finish: " + name);
                        yield return null;
                    }
                    samples.Add(new FeedbackSceneSampleV25 {
                        id = request.id, stage = CurrentStageArt.id, mode = mode,
                        image = name + ".png", state = name + ".json", nativeImage = name + ".native.png",
                        dungeon = request.dungeon, room = game.room, surface = game.player.groundLine,
                        playerX = game.player.x, playerY = game.player.y,
                        originalFraming = legacy, originalMaterialTreatment = originalMaterial
                    });
                }
            }
            SetCameraFramingReviewOriginalV25(false);
            SceneBackgroundStyleV25.CompareOriginalMaterials(false);
            monsterArtReview = false;
            report.samples = samples.ToArray(); report.completed = samples.Count == plan.requests.Length * 3;
            File.WriteAllText(Path.Combine(captureDir, "feedback-v25-scenes.json"), JsonUtility.ToJson(report, true));
            Debug.Log("FEEDBACK_V25_SCENE_REVIEW_COMPLETE: " + samples.Count + " actual captures.");
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--review-quit") >= 0) {
                yield return new WaitForSecondsRealtime(.5f); Application.Quit();
            }
        }
    }
}
