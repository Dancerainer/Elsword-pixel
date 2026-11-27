using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public sealed partial class GameModel {
        sealed class BossHealthRule {
            public readonly float multiplier;
            public readonly string[] actors;
            public bool preserveFinalHealth;
            public BossHealthRule(float multiplier, params string[] actors) {
                this.multiplier = multiplier;
                this.actors = actors;
            }
        }

        // Final-room identity matters: some of the same NPCs appear as lesser
        // enemies elsewhere. Only actual boss bodies own a health gauge.
        static readonly Dictionary<string, BossHealthRule> bossHealthRules = new Dictionary<string, BossHealthRule> {
            {"tree-3", new BossHealthRule(5, "NUI_BENDERS_NORMAL")},
            {"ruins-3", new BossHealthRule(5, "NUI_PPORU_WILLIAM_NORMAL")},
            {"swamp-3", new BossHealthRule(5, "NUI_ANCIENT_PPORU")},
            {"forest-3", new BossHealthRule(6, "NUI_ANGER_FOREST_ENT")},
            {"cave-3", new BossHealthRule(2.5f, "NUI_BENDERS_UPGRADE_NORMAL")},
            {"waterway-3", new BossHealthRule(4, "NUI_BENDERS_NASOD")},
            {"outskirts-3", new BossHealthRule(6, "NUI_PPORU_WILLIAM_C")},
            {"castle-center-2", new BossHealthRule(7, "NUI_WALLY_8TH")},
            {"dragon-road-3", new BossHealthRule(4, "NUI_LIZARDMAN_HIGH")},
            {"besma-lake-3", new BossHealthRule(6, "NUI_LIZARDMAN_OLDER")},
            {"Dungeon_Besma_Lake_Night_Normal-2", new BossHealthRule(6, "NUI_LIZARDMAN_HIGH_DARK_NEW")},
            {"Dungeon_Besma_Dragon_Nest_Normal-2", new BossHealthRule(7, "NUI_LIZARDMAN_SHAMAN_KING")},
            // The mine supplies 60,000 base HP, versus 6,000–7,000 for its regional peers.
            {"Dungeon_Besma_MINE2_Normal-2", new BossHealthRule(.7f, "NUI_WALLY_8TH_MK2")},
            {"Dungeon_Besma_Air_Ship_Normal-2", new BossHealthRule(5, CargoWallyArmorV58, CargoWallyInnerV58)},
            {"Dungeon_Altera_Battle_Air_Ship_Normal-3", new BossHealthRule(6, "NUI_RAVEN")},
            {"Dungeon_Altera_Below_Tunnel_Normal-2", new BossHealthRule(7, "NUI_CANNON_FORT_PARASITE_BODY")},
            {"Dungeon_Altera_Plain_Normal-2", new BossHealthRule(5, "NUI_NASOD_ELEMENT_FIRE", "NUI_NASOD_ELEMENT_ICE")},
            {"Dungeon_Altera_Plain_Recycle_Normal-2", new BossHealthRule(7, "NUI_NASOD_DRILLER_CANNON")},
            {"Dungeon_Altera_Nasod_Factory_Normal-2", new BossHealthRule(7, "NUI_RAVEN_CLONE")},
            {"Dungeon_Altera_Core_Hard-4", new BossHealthRule(10, "NUI_EL_POWER_PLANT") { preserveFinalHealth = true }},
            {"peita-offerings-3", new BossHealthRule(5, "NUI_DULLAHAN")},
            {"peita-spiral-corridor-4", new BossHealthRule(6, "NUI_GOLEM_BOSS")},
            {"peita-chapel-4", new BossHealthRule(7, "NUI_AMETHYST")},
            {"peita-under-garden-3", new BossHealthRule(5, "NUI_PLANT_OVERLOAD", "NUI_PLANT_OVERLOAD_QUEST")},
            {"peita-tower-heart-4", new BossHealthRule(7, "NUI_CUTTYSARK")},
            {"peita-offerings-alter-5", new BossHealthRule(10, "NUI_SUMMON_BOSS_A") { preserveFinalHealth = true }},
            {"velder-third-dwelling-4", new BossHealthRule(6, "NUI_UNOHOUND")},
            {"velder-bridge-hope-5", new BossHealthRule(7, "NUI_DARKELF_BOSS")},
            {"velder-palace-entrance-4", new BossHealthRule(6, "NUI_GLITER_VANGUARD")},
            {"velder-bridge-burning-5", new BossHealthRule(7, "NUI_CRAZY_PUPPET")},
            {"velder-market-4", new BossHealthRule(8, "NUI_SPRIGGAN_SMALL")},
            {"velder-gate-3", new BossHealthRule(10, "NUI_DARK_NEPHILIM") { preserveFinalHealth = true }},
            {"hamel-outskirts-resiam-4", new BossHealthRule(7, "NUI_SHADOW_MASTER")},
            {"hamel-break-resiam-4", new BossHealthRule(5, "NUI_CORAL_SERPENT")},
            {"hamel-waterway-4", new BossHealthRule(7, "NUI_TARANVASH_THE_SHINE")},
            {"hamel-waterway-core-5", new BossHealthRule(8, "NUI_VICTOR")},
            {"hamel-cave-of-magmanta-4", new BossHealthRule(8, "NUI_MAGMANTA")},
            {"hamel-frozen-water-temple-4", new BossHealthRule(8, "NUI_AVALANCHE")},
            {"hamel-frozen-water-hall-5", new BossHealthRule(10, "NUI_RAN") { preserveFinalHealth = true }}
        };

        public bool IsDungeonBoss(Enemy enemy) { return enemy != null && enemy.dungeonBoss; }

        void ApplyDungeonBossHealth(Enemy enemy) {
            if (enemy.dungeonBoss || enemy.isGate || enemy.passive || enemy.regionalAlly ||
                !bossHealthRules.TryGetValue(CurrentRoom.id ?? "", out var rule) ||
                Array.IndexOf(rule.actors, enemy.sourceNPC) < 0) return;
            enemy.dungeonBoss = true;
            float oldMaximum = enemy.maxHp;
            enemy.maxHp = oldMaximum * rule.multiplier;
            if (!rule.preserveFinalHealth && oldMaximum > 0)
                enemy.maxHp = Math.Max(1, (float)Math.Round(enemy.maxHp * (2.0 / 3.0), MidpointRounding.AwayFromZero));
            enemy.hp = oldMaximum > 0 ? (enemy.hp / oldMaximum) * enemy.maxHp : 0;
        }

        // Product pacing, enabled with the shared DANGER presentation. Native
        // source-reference harnesses can still exercise their original exits.
        public const float BossCleanupDelayV57 = 2f;
        public const float BossResultDelayV57 = 1.5f;
        float bossCompletionClockV57 = -1;
        public bool BossCompletionPendingV57 { get { return bossCompletionClockV57 >= 0 && !victory; } }

        void ResetBossCompletionV57() { bossCompletionClockV57 = -1; }

        internal static bool IsCombatMonster(Enemy e) {
            return e != null && !e.passive && !e.isGate && !IsRecoveryContainer(e) && !IsHamelSurvivalSpawner(e) &&
                !(e.kind ?? "").StartsWith("prop-", StringComparison.Ordinal) &&
                !(e.sourceNPC ?? "").StartsWith("NUI_EVENT_", StringComparison.Ordinal);
        }

        bool FinalBossesDefeatedV57() {
            if (HasEncounters) {
                foreach (var clause in CurrentEncounter.clauses) {
                    if (clause.transitions.Length == 0) continue;
                    bool final = true;
                    foreach (var edge in clause.transitions) if (!edge.completeDungeon) final = false;
                    if (!final) continue;
                    // A scripted final target (including a multi-part controller)
                    // is authoritative; a miniboss elsewhere is never sufficient.
                    if (clause.mode == EncounterClearMode.KillNpcKeyCodes) {
                        if (SourceNpcKeyCodesSatisfied(clause.npcKeyCodes)) return true;
                        continue;
                    }
                    if (clause.mode == EncounterClearMode.KillNpcTypes) {
                        bool all = clause.npcTypes.Length > 0;
                        foreach (var npc in clause.npcTypes) {
                            bool found = false;
                            foreach (var e in enemies) if (e.encounterIndex == currentEncounter && e.sourceNPC == npc) {
                                found = true; if (e.hp > 0) all = false;
                            }
                            if (!found) all = false;
                        }
                        if (all) return true;
                        continue;
                    }
                    if (clause.mode == EncounterClearMode.KillAllActive &&
                        (ActiveFinalBossesDefeatedV57() || EncounterCombatClear)) return true;
                }
                return false;
            }
            return room == rooms.Count - 1 && ActiveFinalBossesDefeatedV57();
        }

        bool ActiveFinalBossesDefeatedV57() {
            bool authored = bossHealthRules.ContainsKey(CurrentRoom.id ?? "");
            bool found = false;
            foreach (var e in enemies) {
                if (!e.active || e.encounterIndex != currentEncounter || e.regionalAlly ||
                    e.dormant && string.IsNullOrEmpty(e.spawnAfterDeath)) continue;
                if (authored) {
                    if (!IsDungeonBoss(e)) continue;
                } else {
                    // Legacy rooms without a published boss role use their original model definitions.
                    bool regionalBoss = e.countsForClear && !e.passive && !e.isGate && !string.IsNullOrEmpty(e.kind) &&
                        peitaBehaviors.TryGetValue(e.kind, out var behavior) && behavior.boss;
                    if (!regionalBoss && !IsDangerCombatActor(e)) continue;
                }
                found = true; if (e.hp > 0) return false;
            }
            return found;
        }

        bool IsRoomMonsterForCleanup(Enemy e) {
            if (e.regionalAlly || e.isGate || IsRecoveryContainer(e)) return false;
            if (IsCombatMonster(e)) return true;
            // Some original boss entrances stage regular enemies with their
            // combat AI disabled. Match an actual hostile spawn in this route,
            // keeping event doubles, siege props and spectators intact.
            if (!e.passive || string.IsNullOrEmpty(e.sourceNPC)) return false;
            foreach (var stage in rooms)
                foreach (var spawn in stage.enemies)
                    if (spawn.sourceNPC == e.sourceNPC && spawn.countsForClear && !spawn.passive && !spawn.isGate) return true;
            return false;
        }

        void ClearBossAddsV57() {
            // Cleanup is a death transition, not an attack: buried, armored and
            // invincible adds must also die, without synthetic hits/MP/combos.
            foreach (var e in enemies) {
                if (e.hp <= 0 || !e.active || !IsRoomMonsterForCleanup(e)) continue;
                e.hp = 0; e.state = CombatState.Dead; e.clip = "death"; e.clock = e.deathTime = 0;
                e.stun = e.hitPause = e.telegraph = e.invincible = e.superArmor = 0;
                e.vx = e.vy = 0; e.knockdownOnLanding = false;
                fairyRearReactionsV34.Remove(e); InterruptNpcAction(e);
                if (!e.passive) { kills++; stageKills++; TrySpawnRecoveryDrop(e); }
            }
            npcSummonRequests.Clear();
            regionSummons.Clear();
            npcAttachedSets.Clear();
            foreach (var fx in npcEffects) if (!fx.ended) {
                fx.ended = true; EndNpcEffectSounds(fx);
            }
            npcEffectLiveCount = 0;
            effects.RemoveAll(fx => fx.hostileDamage > 0);
        }

        bool TickBossCompletionV57(float dt) {
            if (!SimpleDangerIntroductions) return false;
            if (player.hp <= 0 || !FinalBossesDefeatedV57()) { ResetBossCompletionV57(); return false; }
            // Consume the complete interval even when the hero is at an exit.
            gateOpen = false; ResetGateTransitionDelay();
            if (AwaitCoreKingDestructionV59()) return true;
            float previous = bossCompletionClockV57;
            if (bossCompletionClockV57 < 0) { bossCompletionClockV57 = 0; npcSummonRequests.Clear(); }
            else bossCompletionClockV57 += dt;
            if (bossCompletionClockV57 + .00001f >= BossCleanupDelayV57) {
                if (previous + .00001f < BossCleanupDelayV57) { ClearRegionSpells(); ClearPeitaSpells(); }
                // A late death callback can queue another minion. Living actors
                // alone enter death, so subsequent ticks never reset a death pose.
                ClearBossAddsV57();
            }
            if (bossCompletionClockV57 + .00001f >= BossCleanupDelayV57 + BossResultDelayV57) {
                FinishDungeonScore(); CancelAction(true); player.vx = player.vy = 0;
                player.clip = "idle"; player.clock = 0;
            }
            return true;
        }
    }
}
