#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class GuardianWakeHitV58 {
            public string reaction, stateAfter;
            public float clockBefore, clockAfter, hpBefore, hpAfter;
        }

        [Serializable] sealed class GuardianWakeResultV58 {
            public string room, npc;
            public int spawnIndex, redOutlinePixels;
            public bool buriedUnarmored, hurtAfterWake, passed;
            public List<GuardianWakeHitV58> hits = new List<GuardianWakeHitV58>();
        }

        IEnumerable<object> ReviewGuardianWakeV58(RuinsVisualCheckV56 check, MethodInfo enterRoom) {
            enterRoom.Invoke(game, new object[] { 1 });
            RebuildRoom();
            yield return null;
            yield return null;

            var guardian = game.enemies.Find(e => GuardianCombatNativeRulesV28.Identity(e.kind, e.sourceNPC));
            if (guardian == null || !game.GuardianSourceProfileV28(guardian) || guardian.npcState != GuardianCombatNativeRulesV28.TrapWait)
                throw new InvalidOperationException("Missing original buried Guardian in Ruins room 2");
            var result = new GuardianWakeResultV58 { room = game.CurrentRoom.id, npc = guardian.sourceNPC,
                spawnIndex = game.enemies.IndexOf(guardian), buriedUnarmored = !game.EnemyHasSuperArmor(guardian) };
            if (!result.buriedUnarmored) throw new InvalidOperationException("Wake armor enabled before detection");
            PlaceMonsterPixelReviewPlayerV53(guardian, -80);
            double deadline = Time.realtimeSinceStartupAsDouble + 45;
            var marks = new[] { .2f, 2.4f, 4.45f };
            var reactions = new[] { HitReaction.Hurt, HitReaction.Launch, HitReaction.Knockdown };
            for (int i = 0; i < marks.Length; i++) {
                while (guardian.npcState != GuardianCombatNativeRulesV28.TrapStart || guardian.npcTime < marks[i]) {
                    if (Time.realtimeSinceStartupAsDouble > deadline) throw new TimeoutException("Guardian did not reach wake sample " + marks[i]);
                    game.player.invincible = 120;
                    yield return null;
                }
                if (!game.EnemyHasSuperArmor(guardian)) throw new InvalidOperationException("Missing Guardian wake armor");
                if (i == 1) result.redOutlinePixels = GuardianWakeOutlinePixelsV58(check, guardian);
                var hit = new GuardianWakeHitV58 { reaction = reactions[i].ToString(), hpBefore = guardian.hp, clockBefore = guardian.npcTime };
                int dealt = game.ApplyHitToEnemy(guardian, 1, reactions[i]);
                hit.hpAfter = guardian.hp;
                hit.clockAfter = guardian.npcTime;
                hit.stateAfter = guardian.npcState;
                result.hits.Add(hit);
                if (dealt <= 0 || hit.hpBefore - hit.hpAfter != dealt || hit.clockBefore != hit.clockAfter ||
                    guardian.npcState != GuardianCombatNativeRulesV28.TrapStart || guardian.state != CombatState.Normal ||
                    guardian.stun > 0 || guardian.knockdownOnLanding)
                    throw new InvalidOperationException("Interrupted/immune Guardian wake: " + JsonUtility.ToJson(hit));
            }
            while (guardian.npcState == GuardianCombatNativeRulesV28.TrapStart) {
                if (Time.realtimeSinceStartupAsDouble > deadline) throw new TimeoutException("Guardian did not finish waking");
                yield return null;
            }
            if (guardian.npcState != GuardianCombatNativeRulesV28.Wait || game.EnemyHasSuperArmor(guardian))
                throw new InvalidOperationException("Guardian did not return to ordinary Wait after waking");
            float hpBeforeHit = guardian.hp;
            int dealtAfterWake = game.ApplyHitToEnemy(guardian, 1, HitReaction.Hurt);
            result.hurtAfterWake = dealtAfterWake > 0 && hpBeforeHit - guardian.hp == dealtAfterWake && guardian.state == CombatState.Hurt;
            if (!result.hurtAfterWake) throw new InvalidOperationException("Guardian remained protected after wake");
            result.passed = true;
            File.WriteAllText(Path.Combine(check.output, "guardian-wake.json"), JsonUtility.ToJson(result, true));
        }

        int GuardianWakeOutlinePixelsV58(RuinsVisualCheckV56 check, Enemy guardian) {
            if (!monsterPixelViewsV53.TryGetValue(guardian, out var view) || !view.renderer.enabled)
                throw new InvalidOperationException("Missing live Guardian whole-frame renderer");
            var original = new MaterialPropertyBlock();
            view.renderer.GetPropertyBlock(original);
            if (original.GetFloat("_Outline") != 1) throw new InvalidOperationException("Guardian outline property is off during wake");
            var normal = check.Render(worldCamera, "guardian-armored");
            var off = new MaterialPropertyBlock();
            view.renderer.GetPropertyBlock(off);
            off.SetFloat("_Outline", 0);
            Color32[] withoutOutline;
            try {
                view.renderer.SetPropertyBlock(off);
                withoutOutline = check.Render(worldCamera, null);
            } finally { view.renderer.SetPropertyBlock(original); }
            int red = 0;
            for (int i = 0; i < normal.Length; i++) {
                var pixel = normal[i];
                if (!pixel.Equals(withoutOutline[i]) && pixel.r > pixel.g && pixel.r > pixel.b) red++;
            }
            if (red == 0) throw new InvalidOperationException("No visible red outline pixels on waking Guardian");
            return red;
        }
    }
}
#endif
