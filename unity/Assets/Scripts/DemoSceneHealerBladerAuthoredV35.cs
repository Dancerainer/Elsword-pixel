using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class HealerDrawV35 {
            internal int frame;
            internal SpriteRenderer carrier;
            internal Sprite original;
            internal SourceWholeSelectionV21 selected;
        }
        sealed class HealerViewV35 {
            internal SpriteRenderer carrier;
            internal FairyAuthoredPartsV33 parts;
            internal string state, motion, key;
            internal float motionClock;
        }

        readonly Dictionary<Enemy,HealerDrawV35> healerDrawsV35 = new Dictionary<Enemy,HealerDrawV35>();
        readonly Dictionary<Enemy,HealerViewV35> healerViewsV35 = new Dictionary<Enemy,HealerViewV35>();
        readonly HashSet<Enemy> healerSeenV35 = new HashSet<Enemy>();
        readonly HashSet<Enemy> healerFailedV35 = new HashSet<Enemy>();
        readonly List<Enemy> healerStaleV35 = new List<Enemy>();
        FairyPoseLibraryV33 healerBankV35;
        bool healerLoadAttemptedV35;

        static bool HealerIdentityV35(Enemy actor) {
            return actor != null && actor.sourceNPC == HealerBladerRigV35.Id && actor.kind == HealerBladerRigV35.Kind;
        }
        void RecordHealerBladerSelectionV35(Enemy actor, SpriteRenderer carrier, SourceWholeSelectionV21 selected) {
            if (!HealerIdentityV35(actor) || !carrier || !carrier.sprite || selected == null) return;
            if (!healerDrawsV35.TryGetValue(actor, out var seen)) {
                seen = new HealerDrawV35();
                healerDrawsV35.Add(actor, seen);
            }
            seen.frame = Time.frameCount;
            seen.carrier = carrier;
            seen.original = carrier.sprite;
            seen.selected = selected;
        }

        bool HealerBladerOwnsRendererV35(Renderer renderer) {
            foreach (var entry in healerViewsV35.Values)
                if (entry.parts.OwnsRenderer(renderer)) return true;
            return false;
        }
        void ReleaseHealerViewV35(Enemy actor) {
            if (!healerViewsV35.TryGetValue(actor, out var entry)) return;
            healerViewsV35.Remove(actor);
            entry.parts.Dispose();
        }
        void DisposeHealerBladerViewsV35() {
            foreach (var entry in healerViewsV35.Values) entry.parts.Dispose();
            healerViewsV35.Clear();
            healerDrawsV35.Clear();
            healerSeenV35.Clear();
            healerFailedV35.Clear();
            healerStaleV35.Clear();
            if (healerBankV35 != null) healerBankV35.ReleaseOwned(DestroyNativeWorldObject);
            healerBankV35 = null;
            healerLoadAttemptedV35 = false;
        }
        void SetHealerLabelsV35(Enemy actor, int index, FairyAuthoredPartsV33 parts) {
            if (actor.hp <= 0 || index >= enemyBars.Count || !enemyBars[index] ||
                !enemyBars[index].enabled || !parts.TryVisibleTop(out var top)) return;
            var bar = enemyBars[index].transform.position;
            bar.y = top + 8;
            enemyBars[index].transform.position = bar;
            if (index < enemyDebuffViews.Count && enemyDebuffViews[index]) {
                var status = enemyDebuffViews[index].transform.position;
                status.y = top + 20;
                enemyDebuffViews[index].transform.position = status;
            }
        }
        // The existing Core/Factory renderer has already chosen the exact state, clock,
        // whole key and signed original anchor. This pass consumes that selection.
        void UpdateHealerBladerViewsV35() {
            if (game == null || !actorsRoot || !worldRoot ||
                (!FactoryProductSessionActiveR1179 && !CoreHardProductSessionActiveR1147)) {
                DisposeHealerBladerViewsV35();
                return;
            }
            healerSeenV35.Clear();
            for (int i = 0; i < game.enemies.Count && i < enemyViews.Count; ++i) {
                var actor = game.enemies[i];
                if(MonsterPixelOwnsActorV53(actor)){ReleaseHealerViewV35(actor);continue;}
                if (!HealerIdentityV35(actor)) continue;
                healerSeenV35.Add(actor);
                if (healerFailedV35.Contains(actor)) continue;
                try {
                    var carrier = enemyViews[i];
                    if(StreamMobileArt&&carrier&&(!actor.active||actor.dormant)&&!carrier.gameObject.activeSelf)continue;
                    if (!healerDrawsV35.TryGetValue(actor, out var draw) || draw.frame != Time.frameCount ||
                        draw.carrier != carrier || !carrier || carrier.sprite != draw.original) {
                        ReleaseHealerViewV35(actor);
                        continue;
                    }
                    if (!healerLoadAttemptedV35) {
                        healerLoadAttemptedV35 = true;
                        healerBankV35 = HealerBladerRigV35.Load();
                    }
                    if (healerBankV35 == null) continue;
                    var selected = draw.selected;
                    var track = healerBankV35.Find(selected.State.Motion);
                    if (track == null || Math.Abs(track.duration - selected.State.MotionDuration) > .00001f)
                        throw new InvalidOperationException("Original Healer motion duration " + selected.State.Motion);
                    if (healerViewsV35.TryGetValue(actor, out var entry) && entry.carrier != carrier) {
                        ReleaseHealerViewV35(actor);
                        entry = null;
                    }
                    if (entry == null) {
                        entry = new HealerViewV35 { carrier = carrier,
                            parts = new FairyAuthoredPartsV33(carrier, healerBankV35, DestroyNativeWorldObject) };
                        healerViewsV35.Add(actor, entry);
                    }
                    var point = RenderEnemyPosition(actor);
                    var worldRootPoint = actorsRoot.TransformPoint(new Vector3(point.x, point.y, 0));
                    var projected = ProjectNativePointWithEye(worldRootPoint,actor.nativeDepth,out float eye);
                    var frame = new FairyAuthoredPartsV33.Frame { alias = selected.State.Motion,
                        sourceClock = selected.SourceMotionClock, facing = actor.facing,
                        visible = selected.ApplySourceShow(actor.active && !actor.dormant && carrier.enabled && carrier.gameObject.activeInHierarchy) };
                    entry.parts.Apply(frame, projected);
                    foreach(var member in entry.parts.Members)BindProjectedSceneOcclusion(member,worldRootPoint,actor.nativeDepth,eye);
                    SetHealerLabelsV35(actor, i, entry.parts);
                    entry.state = selected.State.State;
                    entry.motion = selected.State.Motion;
                    entry.motionClock = selected.SourceMotionClock;
                    entry.key = selected.Whole.Key;
                } catch (Exception error) {
                    ReleaseHealerViewV35(actor);
                    healerFailedV35.Add(actor);
                    Debug.LogWarning("HEALER_BLADER_PARTS_FALLBACK_V35 " + error.Message);
                }
            }
            healerStaleV35.Clear();
            foreach (var actor in healerViewsV35.Keys) if (!healerSeenV35.Contains(actor)) healerStaleV35.Add(actor);
            foreach (var actor in healerStaleV35) ReleaseHealerViewV35(actor);
            healerFailedV35.RemoveWhere(actor => !healerSeenV35.Contains(actor));
            healerDrawsV35.Clear();
            if (healerSeenV35.Count == 0 && healerBankV35 != null) DisposeHealerBladerViewsV35();
        }
    }
}

