using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class FairyViewV33 {
            public SpriteRenderer source;
            public FairyAuthoredPartsV33 parts;
        }
        readonly Dictionary<Enemy,FairyViewV33> fairyViewsV33 = new Dictionary<Enemy,FairyViewV33>();
        readonly HashSet<Enemy> fairySeenV33 = new HashSet<Enemy>();
        readonly HashSet<Enemy> fairyBlockedV33 = new HashSet<Enemy>();
        readonly List<Enemy> fairyStaleV33 = new List<Enemy>();
        FairyPoseLibraryV33 fairyLibraryV33;
        bool fairyLoadAttemptedV33;

        bool FairyAuthoredOwnsRendererV33(Renderer candidate) {
            foreach (var view in fairyViewsV33.Values)
                if (view.parts.OwnsRenderer(candidate)) return true;
            return false;
        }

        void ReleaseFairyViewV33(Enemy e) {
            FairyViewV33 entry;
            if (!fairyViewsV33.TryGetValue(e, out entry)) return;
            fairyViewsV33.Remove(e);
            entry.parts.Dispose();
        }

        void DisposeFairyAuthoredViewsV33() {
            foreach (var entry in fairyViewsV33.Values) entry.parts.Dispose();
            fairyViewsV33.Clear();
            fairySeenV33.Clear();
            fairyBlockedV33.Clear();
            fairyStaleV33.Clear();
            if (fairyLibraryV33 != null) fairyLibraryV33.ReleaseOwned(DestroyNativeWorldObject);
            fairyLibraryV33 = null;
            fairyLoadAttemptedV33 = false;
        }

        bool IsFairySourceBirthV33(Enemy e, int index) {
            return e != null && GuardianCombatNativeRulesV28.Identity(e.kind, e.sourceNPC) &&
                Generic7OriginalBirthV28(e, index) && game.GuardianSourceProfileV28(e);
        }

        bool TryFairySourceFrameV33(Enemy e, SpriteRenderer source, out FairyAuthoredPartsV33.Frame frame) {
            frame = default;
            if (!source || !source.sprite || !libraries.TryGetValue(e.kind, out var library)) return false;
            GuardianCombatNativeRulesV28.Spec spec;
            float clock;
            if (e.state == CombatState.Airborne && string.IsNullOrEmpty(e.npcState) &&
                (e.clip == "launched" || e.clip == "fall")) {
                spec = GuardianCombatNativeRulesV28.Find((int)CombatState.Knockdown, "", "knockdown");
                clock = e.clock;
            } else {
                spec = GuardianCombatNativeRulesV28.Find((int)e.state, e.npcState, e.clip);
                if (spec == null) return false;
                clock = GuardianCombatNativeRulesV28.Clock(spec, e.npcTime, e.clock, e.deathTime, game.FairyGetUpSecondsV34(e));
            }
            if (spec == null || !GuardianCombatNativeRulesV28.Finite(clock) || clock < 0 ||
                !library.frames.TryGetValue(spec.alias, out var original) || original.Length != spec.samples)
                return false;
            float sampled = spec.loop ? Mathf.Repeat(clock, spec.span) : clock;
            int index = Mathf.Clamp(Mathf.FloorToInt(sampled * 24), 0, original.Length - 1);
            if (!ReferenceEquals(original[index], source.sprite)) return false;
            frame.alias = spec.alias;
            frame.sourceClock = clock;
            if (game.FairyRearReactionV34(e)) {
                if (spec.alias == "source-down") frame.alias = "source-down-back-v34";
                if (spec.alias == "source-get-up") {
                    frame.alias = "source-get-up-back-v34";
                    frame.sourceClock = Mathf.Clamp(e.clock, 0, GameModel.FairyBackStandSecondsV34);
                }
            }
            frame.facing = e.facing;
            frame.visible = e.active && !e.dormant && !e.passive && e.aiEnabled &&
                (e.state == CombatState.Dead ? e.hp <= 0 : e.hp > 0) &&
                source.enabled && source.gameObject.activeInHierarchy;
            return true;
        }

        void UpdateFairyLabelV33(Enemy e, int i, FairyAuthoredPartsV33 parts) {
            if (e.hp <= 0 || i >= enemyBars.Count || !enemyBars[i] || !enemyBars[i].enabled ||
                !parts.TryVisibleTop(out var top)) return;
            var bar = enemyBars[i].transform.position;
            bar.y = top + 8;
            enemyBars[i].transform.position = bar;
            if (i < enemyDebuffViews.Count && enemyDebuffViews[i]) {
                var status = enemyDebuffViews[i].transform.position;
                status.y = top + 20;
                enemyDebuffViews[i].transform.position = status;
            }
        }

        // Called after FinishEnemyInitialProjectionV25 in both native projection branches.
        void UpdateFairyAuthoredViewsV33() {
            if (game == null || !worldRoot || !actorsRoot) {
                DisposeFairyAuthoredViewsV33();
                return;
            }
            fairySeenV33.Clear();
            for (int i = 0; i < game.enemies.Count && i < enemyViews.Count; ++i) {
                var e = game.enemies[i];
                if(MonsterPixelOwnsActorV53(e)){ReleaseFairyViewV33(e);continue;}
                if (!IsFairySourceBirthV33(e, i)) continue;
                fairySeenV33.Add(e);
                if (!fairyLoadAttemptedV33) {
                    fairyLoadAttemptedV33 = true;
                    try { fairyLibraryV33 = FairyPoseLibraryV33.Load(); }
                    catch (Exception error) { Debug.LogWarning("FAIRY_PARTS_RESOURCE_V33 " + error.Message); }
                }
                if (fairyLibraryV33 == null || fairyBlockedV33.Contains(e)) continue;
                var source = enemyViews[i];
                try {
                    if (!TryFairySourceFrameV33(e, source, out var frame)) {
                        ReleaseFairyViewV33(e);
                        continue;
                    }
                    if (fairyViewsV33.TryGetValue(e, out var entry) && entry.source != source) {
                        ReleaseFairyViewV33(e);
                        entry = null;
                    }
                    if (entry == null) {
                        entry = new FairyViewV33 { source = source,
                            parts = new FairyAuthoredPartsV33(source, fairyLibraryV33, DestroyNativeWorldObject) };
                        fairyViewsV33.Add(e, entry);
                    }
                    var point = RenderEnemyPosition(e);
                    var worldFoot = actorsRoot.TransformPoint(new Vector3(point.x, point.y, 0));
                    var projectedFoot = NativeWorldActive ? ProjectNativePoint(worldFoot, e.nativeDepth) : worldFoot;
                    entry.parts.Apply(frame, projectedFoot);
                    entry.parts.VisitRenderers(r => BindNativeSceneOcclusionR1461(r, worldFoot, e.nativeDepth));
                    UpdateFairyLabelV33(e, i, entry.parts);
                } catch (Exception error) {
                    ReleaseFairyViewV33(e);
                    fairyBlockedV33.Add(e);
                    Debug.LogWarning("FAIRY_PARTS_FALLBACK_V33 " + error.Message);
                }
            }
            fairyStaleV33.Clear();
            foreach (var e in fairyViewsV33.Keys) if (!fairySeenV33.Contains(e)) fairyStaleV33.Add(e);
            foreach (var e in fairyStaleV33) ReleaseFairyViewV33(e);
            fairyBlockedV33.RemoveWhere(e => !fairySeenV33.Contains(e));
            if (fairySeenV33.Count == 0 && fairyLibraryV33 != null) DisposeFairyAuthoredViewsV33();
        }
    }
}

