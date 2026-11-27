using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        MaterialPropertyBlock initialNpcMaterialV25;
        readonly HashSet<SpriteRenderer> initialPoseModifiedViewsV25 = new HashSet<SpriteRenderer>();
        readonly HashSet<SpriteRenderer> burialClipViewsV25 = new HashSet<SpriteRenderer>();

        static bool InitialBatV25(Enemy enemy) {
            return enemy.sourceNPC == "NUI_BAT_A" || enemy.sourceNPC == "NUI_BAT_B";
        }

        string InitialNpcRenderClipV25(Enemy enemy, string selected) {
            if (game.EnemyInitialPosePhaseV25(enemy) == "complete"
                && (selected == "idle" || selected == "walk")
                && libraries.TryGetValue(enemy.kind, out var library)) {
                string continuous = InitialBatV25(enemy) ? "source-flight"
                    : enemy.sourceNPC == "NUI_TREE_KNIGHT" ? "source-idle" : null;
                if (continuous != null && library.clips.ContainsKey(continuous)) return continuous;
            }
            return selected;
        }

        static float InitialBatFlightFloorV25(Enemy enemy) {
            // Original Wait reference floor, in logical units. This converts
            // the source SceneRoot artwork to the existing flight root once.
            return (enemy.sourceNPC == "NUI_BAT_B" ? 18.6438825313f : 18.4808775980f) * enemy.visualScale;
        }

        // Presentation of the source root is separate from foot-ground support.
        // These fallback placements are not counted as newly authored poses.
        void ApplyEnemyInitialPresentationV25() {
            for (int i = 0; i < game.enemies.Count && i < enemyViews.Count; i++) {
                var enemy = game.enemies[i]; var view = enemyViews[i];
                string phase = enemy.hp > 0 ? game.EnemyInitialPosePhaseV25(enemy) : "complete";
                bool opening = phase == "buried-wait" || phase == "emerging" || phase == "hanging" || phase == "releasing";
                bool flight = First3NativeNormalSpriteV25(enemy,view);
                if (!opening && !flight && !initialPoseModifiedViewsV25.Contains(view)) continue;
                if (opening) initialPoseModifiedViewsV25.Add(view); else initialPoseModifiedViewsV25.Remove(view);
                view.flipY = false; view.enabled = true;
                bool authored = libraries.TryGetValue(enemy.kind, out var library)
                    && library.clips.ContainsKey(enemy.clip);
                float height = Mathf.Max(1, view.bounds.size.y);
                if (phase == "buried-wait" || (phase == "emerging" && !game.EnemyInitialPresentationVisibleV25(enemy))) {
                    view.enabled = false;
                    enemyBars[i].enabled = warnings[i].enabled = false;
                    enemyDebuffViews[i].text = "";
                } else if (phase == "emerging") {
                    // User-requested underground opening: the source WaitHabit
                    // starts with a jump and burrow. Keep that portion buried,
                    // then expose its final unfold. This world offset applies
                    // to native art too; it is not a newly painted pose or a
                    // per-frame silhouette/bounds registration.
                    float progress = Mathf.SmoothStep(0, 1, game.EnemyInitialRevealProgressV25(enemy));
                    var position = view.transform.localPosition;
                    position.y -= 24 * enemy.visualScale * (1 - progress);
                    view.transform.localPosition = position;
                    var bar = enemyBars[i].transform.localPosition;
                    bar.y -= 24 * enemy.visualScale * (1 - progress);
                    enemyBars[i].transform.localPosition = bar;
                } else if (phase == "hanging") {
                    // Existing flight art is a temporary placement fallback;
                    // native WaitZero art has its own authored ceiling anchor.
                    if (!authored) {
                        view.flipY = true;
                        var position = view.transform.localPosition;
                        position.y -= 2 * SpriteOffset(view).y * view.transform.localScale.y;
                        view.transform.localPosition = position;
                    }
                    var bar = enemyBars[i].transform.localPosition;
                    bar.y = game.EnemyInitialSupportYV25(enemy) - height - 8;
                    enemyBars[i].transform.localPosition = bar;
                }
                if (InitialBatV25(enemy) && ((phase == "releasing" && authored) || flight)) {
                    float conversion = flight ? 1 : Mathf.SmoothStep(0, 1,
                        game.EnemyInitialPoseElapsedV25(enemy) / GameModel.BatReleaseDurationV25);
                    var position = view.transform.localPosition;
                    position.y -= InitialBatFlightFloorV25(enemy) * conversion;
                    view.transform.localPosition = position;
                    var bar = enemyBars[i].transform.localPosition;
                    bar.y = position.y + height * .5f + 8;
                    enemyBars[i].transform.localPosition = bar;
                }
                if (phase == "emerging") {
                    var bounds=view.bounds;
                    if(bounds.max.y<=game.EnemyInitialSupportYV25(enemy)+2) {
                        enemyBars[i].enabled=false;enemyDebuffViews[i].text="";
                    } else {
                        var bar=enemyBars[i].transform.position;bar.x=bounds.center.x;bar.y=bounds.max.y+8;enemyBars[i].transform.position=bar;
                        var debuff=enemyDebuffViews[i].transform.position;debuff.x=bounds.center.x;debuff.y=bounds.max.y+20;enemyDebuffViews[i].transform.position=debuff;
                    }
                }
                if (flight) {
                    ApplyFirst3NativeBoneRegistrationV25(enemy,view);
                    var bounds=view.bounds;
                    var bar=enemyBars[i].transform.position;bar.x=bounds.center.x;bar.y=bounds.max.y+8;enemyBars[i].transform.position=bar;
                    var debuff=enemyDebuffViews[i].transform.position;debuff.x=bounds.center.x;debuff.y=bounds.max.y+20;enemyDebuffViews[i].transform.position=debuff;
                }
            }
        }

        void FinishEnemyInitialProjectionV25() {
            for (int i = 0; i < game.enemies.Count && i < enemyViews.Count; i++) {
                var enemy = game.enemies[i]; var view = enemyViews[i];
                bool emerging = enemy.hp > 0 && game.EnemyInitialPosePhaseV25(enemy) == "emerging";
                if (!emerging && !burialClipViewsV25.Contains(view)) continue;
                if (initialNpcMaterialV25 == null) initialNpcMaterialV25 = new MaterialPropertyBlock();
                view.GetPropertyBlock(initialNpcMaterialV25);
                initialNpcMaterialV25.SetFloat("_BurialClip", emerging ? 1 : 0);
                if (emerging) {
                    burialClipViewsV25.Add(view);
                    var ground = new Vector3(enemy.x, game.EnemyInitialSupportYV25(enemy), 0);
                    float groundY = NativeWorldActive ? ProjectNativePoint(ground, enemy.nativeDepth).y : ground.y;
                    initialNpcMaterialV25.SetFloat("_BurialGroundY", groundY);
                } else burialClipViewsV25.Remove(view);
                view.SetPropertyBlock(initialNpcMaterialV25);
                initialNpcMaterialV25.Clear();
            }
            ShowNearbyForestExitGate();
        }
    }
}
