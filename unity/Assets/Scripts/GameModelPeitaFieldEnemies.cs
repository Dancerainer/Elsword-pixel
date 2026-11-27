using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        float peitaStunUntil;
        int peitaStunHitSerial;
        double peitaPoisonUntil, peitaPoisonNext;
        const float PeitaPoisonTick = 28 * .3f;

        static bool PeitaPoisonMove(Enemy enemy, PeitaAttack move) {
            return enemy.kind == "jubigee" || (enemy.kind == "plant_drosera" && move.clip == "peita-2");
        }

        void StrikePeitaProjectile(Effect fx) {
            int dealt = ApplyEnemyAttackToPlayer(fx.hostileDamage, fx.x - fx.facing * 20);
            if (dealt > 0 && fx.type == "peita-poison") {
                // JUBIGEE_ATTACK and DROSERA_ATTACKB both apply a five-second MIX_ATTACK poison.
                if (peitaPoisonUntil <= elapsed) peitaPoisonNext = elapsed + 1;
                peitaPoisonUntil = elapsed + 5;
            }
        }

        void TickPeitaPoison() {
            if (peitaPoisonUntil <= 0) return;
            while (player.hp > 0 && peitaPoisonNext <= elapsed && peitaPoisonNext <= peitaPoisonUntil) {
                float amount = Math.Min(player.hp, PeitaPoisonTick);
                ApplyCombatHealth(player, player.hp - amount);
                SpawnResourceDamage(player, amount);
                peitaPoisonNext += 1;
                if (player.hp <= 0) {
                    player.state = CombatState.Dead;
                    player.clip = "death"; player.clock = player.stun = 0;
                    CancelAction(true);
                }
            }
            if (elapsed >= peitaPoisonUntil || player.hp <= 0) peitaPoisonUntil = 0;
        }
        bool PeitaPlayerStunned {
            get { return elapsed < peitaStunUntil && PlayerImpactSerial - peitaStunHitSerial < 2; }
        }

        static bool PeitaMoveArmored(Enemy enemy) {
            var state = enemy.peitaAttack;
            if (state == null || state.attack == null || enemy.telegraph <= 0 || enemy.hp <= 0) return false;
            return state.elapsed < state.attack.windup ? state.attack.readyArmor : state.attack.attackArmor;
        }

        static HitReaction PeitaContactReaction(PeitaAttack move) {
            switch (move.hitReaction) {
                case "RT_FLY": case "RT_UP": return HitReaction.Launch;
                case "RT_DOWN": return HitReaction.Knockdown;
                case "RT_BIG_DAMAGE": case "RT_SMALL_DAMAGE": return HitReaction.Hurt;
                default: return move.heavy ? HitReaction.Knockdown : HitReaction.Hurt;
            }
        }

        int GuardPeitaShield(Enemy enemy, int damage, float attackerX, ref HitReaction reaction, ref bool shieldContact) {
            if ((enemy.kind != "gliter_shield" && enemy.kind != "gliter_shield_elite") ||
                !peitaBehaviors.ContainsKey(enemy.kind) || enemy.state != CombatState.Normal || !Grounded(enemy) ||
                (attackerX - enemy.x) * enemy.facing < 0 || enemy.clip == "peita-1-ready") return damage;
            // Both shield scripts give 70% front defence to Wait, Walk, Guard and AttackA_Fire.
            reaction = HitReaction.None;
            shieldContact=true;
            SpawnEffect("guard", enemy.x, enemy.y + 25, enemy.facing);
            return Math.Max(1, (int)Math.Ceiling(damage * .3));
        }

        static bool PeitaManaEaterProtected(Enemy enemy) {
            if (enemy == null || enemy.kind != "mana_eater" || enemy.peitaAttack == null) return false;
            var state = enemy.peitaAttack;
            return (state.attack != null && enemy.telegraph > 0 && state.elapsed >= .29f) ||
                   (state.escapeClock >= .67f);
        }

        static bool IsResourceEater(Enemy enemy) {
            return enemy.kind == "mana_eater" || enemy.kind == "blood_eater";
        }

        // Both 2016 MagicAttack states pulse from 0.30 to 0.50 s. Their
        // resource loss is in EXTRA_DAMAGE, outside the regular weapon samples.
        bool TryDrainEaterResource(Enemy enemy, float reach, float remainingMotion) {
            if (Math.Abs(player.x - enemy.x) > reach || Math.Abs(player.y - enemy.y) >= 48 ||
                player.hp <= 0 || player.invincible > 0 || AwakeningCasting || SkillCastInvulnerable ||
                !EnemyAccuracyAllowsHit(enemy)) return false;
            bool blood = enemy.kind == "blood_eater";
            if (blood) {
                if (HitPlayer((int)Math.Ceiling(player.maxHp * 40.0 / 100), enemy.x, HitReaction.Hurt, 0, false) <= 0)
                    return false;
            } else {
                if (mana <= 0) return false;
                mana = Math.Max(0, mana - 400);
            }
            SpawnEffect("enemy-hit", player.x, player.y + 24);
            if (enemy.peitaAttack == null) enemy.peitaAttack = new PeitaAttackState();
            enemy.peitaAttack.fleeUntil = elapsed + Math.Max(0, remainingMotion) + (blood ? 8 : 6);
            return true;
        }

        void EscapeEater(Enemy enemy) {
            enemy.peitaAttack.fleeUntil = 0;
            enemy.active = false;
            enemy.hp = 0;
        }

        bool TickRegionEater(Enemy enemy, RegionActorState actor) {
            if (!IsResourceEater(enemy)) return false;
            var node = actor.node;
            bool draining = node.id.EndsWith("_MAGIC_ATTACK", StringComparison.Ordinal);
            if (draining && !actor.hit && actor.PreviousMotionTime <= .5f && actor.MotionTime >= .3f)
                actor.hit = TryDrainEaterResource(enemy, 154, node.duration - actor.MotionTime);
            var state = enemy.peitaAttack;
            if (state == null || state.fleeUntil <= 0) return false;
            if (draining && actor.MotionTime < node.duration) return true;
            if (node.id.EndsWith("_RUNAWAY_READY", StringComparison.Ordinal)) {
                if (actor.MotionTime >= node.duration) EscapeEater(enemy);
                return true;
            }
            string prefix = enemy.kind == "blood_eater" ? "BLOOD_EATER" : "MANA_EATER";
            string next = elapsed >= state.fleeUntil ? prefix + "_RUNAWAY_READY" : prefix + "_RUN";
            if (node.id != next) EnterRegionNode(enemy, actor, actor.spec.nodes[next]);
            enemy.facing = enemy.x >= player.x ? 1 : -1;
            return true;
        }

        bool TickPeitaManaEater(Enemy enemy, PeitaBehaviorRow row, float dt) {
            var state = enemy.peitaAttack;
            if (state.attack != null && enemy.telegraph <= 0) state.attack = null;
            if (state.attack != null) {
                float before = state.elapsed;
                state.elapsed += dt;
                EmitPeitaAttackSounds(enemy, state);
                if (!state.struck && before <= .5f && state.elapsed >= state.attack.hitAt)
                    state.struck = TryDrainEaterResource(enemy, row.reach, state.attack.duration - state.elapsed);
                if (state.elapsed >= state.attack.duration) {
                    state.attack = null;
                    enemy.telegraph = 0;
                    enemy.cooldown = .65f;
                    PeitaPose(enemy, "idle");
                }
                return true;
            }
            if (state.fleeUntil <= 0) return false;
            if (elapsed < state.fleeUntil) {
                enemy.facing = enemy.x >= player.x ? 1 : -1;
                MoveAlongGround(enemy, Clamp(enemy.x + enemy.facing * 820 * .28f * dt, 22, RoomWidth - 26));
                PeitaPose(enemy, "peita-run");
            } else {
                PeitaPose(enemy, "peita-escape");
                state.escapeClock += dt;
                if (state.escapeClock >= 2.6667f) EscapeEater(enemy);
            }
            return true;
        }

        void PeitaEaterBonusDrop(Enemy enemy) {
            if (!IsResourceEater(enemy) || enemy.peitaAttack == null || enemy.peitaAttack.fleeUntil <= 0 ||
                enemy.hp > 0 || !enemy.active) return;
            enemy.peitaAttack.fleeUntil = 0;
            bool hp = enemy.kind == "blood_eater";
            AddRecoveryDrop(enemy, hp, hp ? 40 : 100);
        }

        bool TickPeitaLanternStrike(Enemy e, PeitaAttackState state, float strike, float dt, int damage) {
            if (e.kind != "fly_lantern" && e.kind != "fly_lantern_thunder") return false;
            // PASSIVE_SPEED_X 1500, active contact window 0.01–0.21 seconds.
            float advance = Math.Min(dt, strike);
            MoveAlongGround(e, Clamp(e.x + e.facing * 1500 * .28f * advance, 22, RoomWidth - 26));
            if (!state.struck && strike >= .01f && strike - dt <= .21f &&
                Math.Abs(player.x - e.x) <= 32 && Math.Abs(player.y - e.y) < 44 && EnemyAccuracyAllowsHit(e)) {
                if (ApplyEnemyAttackToPlayer(damage, e.x, HitReaction.Hurt) > 0) {
                    state.struck = true;
                    if (e.kind == "fly_lantern_thunder") {
                        peitaStunUntil = elapsed + 2;
                        peitaStunHitSerial = PlayerImpactSerial;
                        CancelAction(true);
                    }
                }
            }
            return true;
        }
    }
}
