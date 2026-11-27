using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        MaterialPropertyBlock buriedMonsterPropertiesV57;

        bool SaplingOpeningV57(Enemy actor) {
            if (actor.hp <= 0 || actor.kind != "tree-knight" || actor.sourceNPC != "NUI_TREE_KNIGHT") return false;
            string phase = game.EnemyInitialPosePhaseV25(actor);
            return phase == "buried-wait" || phase == "emerging";
        }

        float MonsterBurialOffsetV57(MonsterSelectionV53 selected) {
            var actor = selected.actor;
            if (SaplingOpeningV57(actor))
                return BuriedMonsterPresentationV57.SaplingOffset(game.EnemyInitialRevealProgressV25(actor)) * actor.visualScale;
            if (actor.hp > 0 && actor.kind == "fairy-guardian" && actor.sourceNPC == "NUI_FAIRY_GUARDIAN")
                return BuriedMonsterPresentationV57.GuardianOffset(selected.motion, selected.clock) * actor.visualScale;
            return 0;
        }

        void ApplyMonsterBurialV57(MonsterSelectionV53 selected, SpriteRenderer renderer) {
            var actor = selected.actor;
            bool sapling = SaplingOpeningV57(actor);
            bool buried = sapling || MonsterBurialOffsetV57(selected) < 0;
            if (!buried) return; // Apply already copied the ordinary carrier block.
            if (sapling) {
                // A small visible shoot marks the pre-existing birth. Its
                // contact/armor window still follows the original model timer.
                renderer.enabled = actor.active && !actor.dormant && selected.carrier.gameObject.activeInHierarchy;
            }
            float support = sapling ? game.EnemyInitialSupportYV25(actor) : actor.y;
            var ground = actorsRoot.TransformPoint(new Vector3(actor.x, support, 0));
            if (buriedMonsterPropertiesV57 == null) buriedMonsterPropertiesV57 = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(buriedMonsterPropertiesV57);
            buriedMonsterPropertiesV57.SetFloat("_BurialClip", 1);
            if (sapling && game.EnemyInitialRevealProgressV25(actor) <= 0)
                buriedMonsterPropertiesV57.SetFloat("_Outline", 0);
            buriedMonsterPropertiesV57.SetFloat("_BurialGroundY", NativeWorldActive ? ProjectNativePoint(ground, actor.nativeDepth).y : ground.y);
            renderer.SetPropertyBlock(buriedMonsterPropertiesV57);
            buriedMonsterPropertiesV57.Clear();
            if (sapling && game.EnemyInitialRevealProgressV25(actor) <= 0) {
                int i = game.enemies.IndexOf(actor);
                if (i >= 0 && i < enemyBars.Count) {
                    enemyBars[i].enabled = warnings[i].enabled = false;
                    enemyDebuffViews[i].text = "";
                }
            }
        }
    }
}
