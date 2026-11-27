using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public sealed partial class GameModel {
        // Gameplay uses the short shared DANGER banner. Source introduction
        // contracts remain available to the offline source-reference harnesses.
        public bool SimpleDangerIntroductions { get; private set; }

        static readonly HashSet<string> dangerNpcIds = new HashSet<string>(StringComparer.Ordinal) {
            "NUI_BENDERS", "NUI_BENDERS_UPGRADE_NORMAL", "NUI_BENDERS_NASOD",
            "NUI_PPORU_GIANT_WHITE", "NUI_PPORU_GIANT_RED", "NUI_ANCIENT_PPORU",
            "NUI_PPORU_WILLIAM", "NUI_PPORU_WILLIAM_NORMAL", "NUI_PPORU_WILLIAM_B", "NUI_PPORU_WILLIAM_C",
            "NUI_ANGER_FOREST_ENT", "NUI_THIEF_FAT_BOSS", "NUI_WALLY_8TH", "NUI_WALLY_9TH_ARMOR", "NUI_WALLY_9TH",
            "NUI_LIZARDMAN_HIGH", "NUI_LIZARDMAN_OLDER", "NUI_LIZARDMAN_SHAMAN_KING", "NUI_LIZARDMAN_HIGH_DARK", "NUI_LIZARDMAN_HIGH_DARK_NEW",
            "NUI_RAVEN", "NUI_RAVEN_CLONE", "NUI_RAVEN_CLONE_B", "NUI_CANNON_MINI_UP",
            "NUI_NASOD_MINER_UP", "NUI_NASOD_GUARD_UP", "NUI_NASOD_KING", "NUI_EL_POWER_PLANT",
            "NUI_WALLY_8TH_MK2", "NUI_NASOD_DRILLER_CANNON", "NUI_CANNON_FORT_PARASITE_BODY",
            "NUI_NASOD_ELEMENT_FIRE", "NUI_NASOD_ELEMENT_ICE", "NUI_NASOD_ELEMENT_FIRE_SOLO", "NUI_NASOD_ELEMENT_ICE_SOLO",
            "NUI_NASOD_ELEMENT_FIRE_B", "NUI_NASOD_ELEMENT_ICE_B",
            "NUI_DULLAHAN", "NUI_GOLEM_BOSS", "NUI_AMETHYST", "NUI_PLANT_OVERLOAD",
            "NUI_PLANT_OVERLOAD_QUEST", "NUI_CUTTYSARK", "NUI_SUMMON_BOSS_A"
        };

        // Promoted only at their authored encounters. NPC names and body/AI
        // variants are reused by ordinary groups, random rolls and summons.
        public bool IsBossIntroductionActor(Enemy actor) {
            if (!IsCombatMonster(actor) || actor.regionalAlly) return false;
            if (IsDungeonBoss(actor)) return true;
            if (!actor.countsForClear || actor.recoverySpawn == null) return false;
            switch (CurrentRoom.id) {
                case "cave-2": return actor.encounterIndex == 0 && actor.sourceNPC == "NUI_PPORU_GIANT_RED";
                case "outskirts-2": return actor.encounterIndex == 1 && actor.sourceNPC == "NUI_PPORU_WILLIAM_B";
                case "Dungeon_Altera_Plain_Normal-0": return actor.encounterIndex == 0 && actor.sourceNPC == "NUI_NASOD_ELEMENT_FIRE_SOLO";
                case "Dungeon_Altera_Plain_Normal-1": return actor.encounterIndex == 1 && actor.sourceNPC == "NUI_NASOD_ELEMENT_ICE_SOLO";
                case "Dungeon_Altera_Core_Hard-0": return actor.encounterIndex == 1 && actor.sourceNPC == "NUI_RAVEN_CLONE_B";
                case "Dungeon_Altera_Core_Hard-2":
                    return actor.encounterIndex == 1 && actor.sourceNPC == "NUI_NASOD_ELEMENT_FIRE_B" ||
                           actor.encounterIndex == 2 && actor.sourceNPC == "NUI_NASOD_ELEMENT_ICE_B";
                default: return false;
            }
        }

        // Legacy source-opening policy. These actors can still need their AI
        // released from an opening state even when they fight as regular mobs.
        public static bool IsDangerCombatActor(Enemy actor) {
            if (actor == null || actor.passive || actor.isGate || !actor.countsForClear && actor.sourceNPC != "NUI_CANNON_FORT_PARASITE_BODY" ||
                (actor.sourceNPC ?? "").StartsWith("NUI_EVENT_", StringComparison.Ordinal) ||
                (actor.kind ?? "").StartsWith("prop-", StringComparison.Ordinal)) return false;
            return dangerNpcIds.Contains(actor.sourceNPC ?? "") || IsBoss(actor.kind) ||
                (actor.kind ?? "").EndsWith("-boss", StringComparison.Ordinal) || actor.kind == "shadow-warrior";
        }

        public void EnableSimpleDangerIntroductions() {
            SimpleDangerIntroductions = true;
            // Room construction may have run before a product host hands its
            // new model to DemoScene. Release those already constructed actors.
            foreach (var actor in enemies) ReleaseSimpleDangerOpening(actor);
        }

        static bool IsDangerOpeningState(Enemy actor, string state) {
            if (actor == null || actor.sourceNPC == CargoWallyInnerV58 || string.IsNullOrEmpty(state)) return false;
            string npc = actor.sourceNPC ?? "";
            string prefix = npc.StartsWith("NUI_", StringComparison.Ordinal) ? npc.Substring(4) : npc;
            // Exact birth-state names only. ATTACK_START, TRAP_MODE_START and
            // other combat phases must retain their normal clocks and edges.
            return state == prefix + "_START" || state == prefix + "_START_READY" || state == prefix + "_START_HENIR" ||
                npc == "NUI_NASOD_ELEMENT_FIRE_B" && state == "NASOD_ELEMENT_FIRE_SOLO_B_START" ||
                npc == "NUI_NASOD_ELEMENT_ICE_B" && state == "NASOD_ELEMENT_ICE_SOLO_B_START" ||
                npc == "NUI_CANNON_FORT_PARASITE_BODY" && state == "CANNONFORT_PARASITE_BODY_START";
        }

        static bool IsDangerFortOpening(Enemy actor, string state) {
            if (actor == null) return false;
            switch (actor.sourceNPC) {
                case "NUI_CANNON_FORT_PARASITE_BODY":
                case "NUI_CANNON_FORT_PARASITE_BODY_ARMOR":
                case "NUI_CANNON_FORT_PARASITE_TOP_EYE":
                case "NUI_CANNON_FORT_PARASITE_BOTTOM_EYE":
                case "NUI_CANNON_FORT_PARASITE_TOP_CANNON":
                case "NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON":
                    return state == actor.sourceNPC.Substring(4).Replace("CANNON_FORT_", "CANNONFORT_") + "_START";
                default: return false;
            }
        }

        NpcStateDefinition SimpleDangerEntryState(Enemy actor, NpcStateDefinition requested) {
            // The Fort is one six-part entrance. Follow each part's exact
            // source opening edge; the body's guarded phase owns its armor.
            NpcStateDefinition fortReady;
            if (SimpleDangerIntroductions && requested != null && IsDangerFortOpening(actor, requested.id) &&
                !string.IsNullOrEmpty(requested.nextState) && npcStates.TryGetValue(actor.sourceNPC + "/" + requested.nextState, out fortReady)) return fortReady;
            if (!SimpleDangerIntroductions || requested == null || !IsDangerCombatActor(actor) || !IsDangerOpeningState(actor, requested.id)) return requested;
            var profile = NpcProfile(actor); NpcStateDefinition ready;
            if (profile != null && !string.IsNullOrEmpty(profile.waitState) && !IsDangerOpeningState(actor, profile.waitState) && npcStates.TryGetValue(actor.sourceNPC + "/" + profile.waitState, out ready)) return ready;
            if (!string.IsNullOrEmpty(requested.nextState) && !IsDangerOpeningState(actor, requested.nextState) && npcStates.TryGetValue(actor.sourceNPC + "/" + requested.nextState, out ready)) return ready;
            return requested;
        }

        void ReleaseSimpleDangerOpening(Enemy actor) {
            if (!SimpleDangerIntroductions || actor == null || !actor.active || actor.dormant || actor.hp <= 0) return;
            if (actor == swampIntroActorV39) swampActorReleasedV39 = true;
            if (actor == alteraAirshipIntroRavenV18) alteraAirshipIntroReleasedV18 = true;
            if (actor == dragonIntroKingV20) dragonIntroReleasedV20 = true;
            var current = NpcState(actor);
            bool fortOpening=current != null && IsDangerFortOpening(actor, current.id);
            if (!IsDangerCombatActor(actor) && !fortOpening) return;
            if (current == null || !IsDangerOpeningState(actor, current.id) && !fortOpening) return;
            var ready = SimpleDangerEntryState(actor, current);
            if (ready == current) return;
            bool aiBefore=actor.aiEnabled;
            EnterNpcState(actor, ready);
            actor.aiEnabled = fortOpening ? aiBefore : true;
            actor.clock = actor.npcTime = 0;
            actor.vx = actor.vy = 0;
            if (actor == factoryRavenPreparedActorV21) factoryRavenReleasedActorV21 = actor;
            if (CoreHardStopIntroBirthV21(actor) != null) coreHardStopIntroReleasedV21.Add(actor);
        }
    }
}
