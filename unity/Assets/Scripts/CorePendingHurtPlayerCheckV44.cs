#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class CoreReactionPlayerCheckV32 {
        [Serializable] sealed class PendingHurtV44 {
            public string sourceNPC, beforeState, nextState, bodySprite;
            public int room, damage, cameraResetsBefore, cameraResetsAfter;
            public float hpBefore, hpAfter;
            public bool sameSimulationTime, emptyGameplayState, committed, returnedToMenu;
            public RegisteredRigPlayerGpuV35.PixelProof before, impact, after;
        }

        static bool ManagedHurtBodyV44(Enemy actor) {
            return actor.sourceNPC == "NUI_NASOD_KING" || actor.sourceNPC == "NUI_EL_POWER_PLANT" ||
                actor.sourceNPC == "NUI_EL_EXTRACTOR_EARTH" || actor.sourceNPC == "NUI_EL_EXTRACTOR_FIRE" ||
                actor.sourceNPC == "NUI_EL_EXTRACTOR_ICE" || actor.sourceNPC == "NUI_EL_EXTRACTOR_NOVA";
        }

        void RedrawCurrentFrameV44() {
            Call(scene, "RestoreNativeProjection");
            Call(scene, "RenderActors");
            Call(scene, "RenderCombatPresentation");
            Call(scene, "FinishNativeProjection");
        }

        IEnumerator CheckPendingHurtV44() {
            var record = new PendingHurtV44 { room = 4 };
            var receipt = Path.Combine(output, "pending-hurt-v44.json");
            var camera = (Camera)Field(scene, "worldCamera");
            Call(game, "EnterRoom", record.room);
            Call(scene, "RebuildRoom");
            Need(game.room == 4 && game.EncounterIndex == 0, "Original Core arena and encounter");
            var members = new List<SpriteRenderer>(1);
            SpriteRenderer body = null;
            double end = Time.realtimeSinceStartupAsDouble + 110;
            bool moved = false;
            while (string.IsNullOrEmpty(record.nextState)) {
                Check();
                Need(Time.realtimeSinceStartupAsDouble < end, "Could not hit a visible original Core managed actor");
                // Let ordinary Update initialize all source actors and the arena camera.
                yield return null;
                var carriers = (IList)Field(scene, "enemyViews");
                for (int i = 0; i < game.enemies.Count; ++i) {
                    var candidate = game.enemies[i];
                    if (!ManagedHurtBodyV44(candidate) || !candidate.active || candidate.dormant ||
                        candidate.hp < 3 || string.IsNullOrEmpty(candidate.npcState) ||
                        candidate.invincible > 0 || game.EnemyHasNpcInvincibility(candidate) ||
                        game.EnemyHasSuperArmor(candidate)) continue;
                    var view = carriers[i] as SpriteRenderer;
                    if (!view || !view.enabled || !view.sprite || !view.gameObject.activeInHierarchy) continue;
                    if (!moved) {
                        game.player.x = candidate.x - 35;
                        game.player.y = candidate.y;
                        game.player.groundLine = candidate.groundLine;
                        game.player.vx = game.player.vy = 0;
                        game.player.invincible = 90;
                        moved = true;
                        break;
                    }
                    if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), view.bounds)) continue;
                    members.Clear();members.Add(view);
                    var before = RegisteredRigPlayerGpuV35.CaptureAtResolution(camera, game, candidate, members, output, "pending-before", 1280, 720, false);
                    if (before.count == 0) continue;
                    float hp = candidate.hp;
                    string prior = candidate.npcState;
                    int damage = game.ApplyHitToEnemy(candidate, 2, HitReaction.Hurt);
                    if (damage == 0 || !game.TryCoreHardHitDisplayPendingV39(candidate, out var next)) continue;
                    target = candidate;body = view;
                    record.before = before;record.beforeState = prior;record.nextState = next;record.sourceNPC = candidate.sourceNPC;
                    record.hpBefore = hp;record.hpAfter = candidate.hp;record.damage = damage;
                    break;
                }
            }
            float elapsed = game.elapsed;
            record.cameraResetsBefore = (int)Field(scene, "coreArenaRestoresV21");
            RedrawCurrentFrameV44();
            record.sameSimulationTime = game.elapsed == elapsed;
            record.emptyGameplayState = string.IsNullOrEmpty(target.npcState);
            record.cameraResetsAfter = (int)Field(scene, "coreArenaRestoresV21");
            Need(record.sameSimulationTime && record.emptyGameplayState && target.clock == 0 &&
                body.enabled && body.sprite && body.gameObject.activeInHierarchy &&
                record.cameraResetsBefore == record.cameraResetsAfter, "Impact frame drew before the source owner tick");
            record.bodySprite = body.sprite.name;
            record.impact = RegisteredRigPlayerGpuV35.CaptureAtResolution(camera, game, target, members, output, "pending-impact", 1280, 720);
            File.WriteAllText(receipt, JsonUtility.ToJson(record, true));
            while (string.IsNullOrEmpty(target.npcState)) { Check(); yield return null; }
            record.committed = target.npcState == record.nextState;
            Need(record.committed, "Source owner committed the same reaction");
            record.after = RegisteredRigPlayerGpuV35.CaptureAtResolution(camera, game, target, members, output, "pending-committed", 1280, 720);
            target = null;
            Press("dungeons");
            for (int i = 0; i < 4; ++i) yield return null;
            record.returnedToMenu = !scene.CoreHardProductSessionActiveR1147 && (string)Field(scene,"openMenu") == "dungeons";
            Need(record.returnedToMenu, "Returned from Core through the ordinary menu");
            report.returnedMain = true;
            File.WriteAllText(receipt, JsonUtility.ToJson(record, true));
        }
    }
}
#endif
