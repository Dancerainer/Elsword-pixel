using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public enum ImpactMaterial { Flesh, Wood, Shell, Metal, Spirit }
    internal enum ImpactTone { Flesh, Wood, Shell, Metal, Spirit, IronArmor, DenseStone, DenseMetal, Hive, KnightShield, GlitterShield, CocoonHurt, SoldierPlate, CastleMachine }

    public sealed class EnemyImpactSound {
        public int serial, room;
        public ImpactMaterial material;
        public bool heavy;
        public float x, y, time;
        internal ImpactTone tone;
        internal int soundActor;
    }

    public sealed partial class GameModel {
        public readonly List<EnemyImpactSound> enemyImpactSounds = new List<EnemyImpactSound>();

        public static ImpactMaterial MaterialForEnemy(Enemy enemy) {
            return ImpactFamily(EnemyTone(enemy));
        }

        static ImpactMaterial ImpactFamily(ImpactTone tone) {
            switch(tone) {
                case ImpactTone.Hive: return ImpactMaterial.Wood;
                case ImpactTone.DenseStone: case ImpactTone.CocoonHurt: return ImpactMaterial.Shell;
                case ImpactTone.DenseMetal:
                case ImpactTone.IronArmor:
                case ImpactTone.SoldierPlate:
                case ImpactTone.CastleMachine:
                case ImpactTone.KnightShield:
                case ImpactTone.GlitterShield: return ImpactMaterial.Metal;
                default: return (ImpactMaterial)tone;
            }
        }

        static ImpactTone EnemyTone(Enemy enemy) {
            if(enemy == null) return ImpactTone.Flesh;
            // 2016 INIT_COMPONENT.HITTED_TYPE, with exact presentation overrides for visible armor,
            // shells, spirits, and the HP-bearing phases of undeclared mechanical props.
            switch(enemy.sourceNPC) {
                case "NUI_BENDERS_NASOD":
                case "NUI_SOLDIER_BENDERS":
                case "NUI_SOLDIER_CROSSBOW":
                case "NUI_SOLDIER_FAT":
                case "NUI_SOLDIER_THIN":
                    return ImpactTone.IronArmor;
                case "NUI_CAVE_OF_MAGMANTA_GATE":
                case "NUI_CRYSTAL_TRAP":
                case "NUI_CRYSTAL_TRAP_LEFT":
                case "NUI_CRYSTAL_TRAP_RIGHT":
                case "NUI_FROZEN_WATER_HALL_GATE":
                case "NUI_FROZEN_WATER_TEMPLE_GATE":
                case "NUI_LATTICE_GATE_BELOW_PASSAGE":
                case "NUI_NASOD_KING":
                case "NUI_RESIAM_BOSS_GATE":
                case "NUI_RESIAM_GATE":
                case "NUI_UNDERWATER_BOSS_GATE":
                case "NUI_UNDERWATER_GATE":
                case "NUI_VELDER_BRIDGE_GATE":
                case "NUI_VELDER_DWELLING_GATE":
                case "NUI_VELDER_GATE_SMALL_GATE1":
                case "NUI_VELDER_GATE_SMALL_GATE2":
                case "NUI_VELDER_PALACE_GATE":
                case "NUI_WATERWAY_BOSS_GATE":
                case "NUI_WATERWAY_GATE":
                    return ImpactTone.DenseMetal;
                case "NUI_ALTERA_MINE":
                case "NUI_BARRIERMACHINE_FIRE":
                case "NUI_BARRIERMACHINE_ICE":
                case "NUI_BEE_NASOD":
                case "NUI_CANNON_BIG":
                case "NUI_CANNON_BIG_DOWN":
                case "NUI_CANNON_DEMON":
                case "NUI_CANNON_FORT_PARASITE_BODY_ARMOR":
                case "NUI_CANNON_FORT_PARASITE_BODY_SHELL":
                case "NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON":
                case "NUI_CANNON_FORT_PARASITE_TOP_CANNON":
                case "NUI_CONRAD_THE_ODD":
                case "NUI_DULLAHAN":
                case "NUI_EL_EXTRACTOR_EARTH":
                case "NUI_EL_EXTRACTOR_FIRE":
                case "NUI_EL_EXTRACTOR_ICE":
                case "NUI_EL_EXTRACTOR_NORMAL_A":
                case "NUI_EL_EXTRACTOR_NORMAL_B":
                case "NUI_EL_EXTRACTOR_NOVA":
                case "NUI_EL_POWER_PLANT":
                case "NUI_FACTORY_PRESS":
                case "NUI_FACTORY_PRESS3":
                case "NUI_GEAR_TRAP":
                case "NUI_GLITER_FRAIL_GREAT":
                case "NUI_GLITER_FRAIL_GREAT_BOSS":
                case "NUI_GLITER_HAMMER_BOSS":
                case "NUI_GLITER_VANGUARD":
                case "NUI_NASOD_DRILLER_CANNON":
                case "NUI_NASOD_ELEMENT_FIRE":
                case "NUI_NASOD_ELEMENT_FIRE_B":
                case "NUI_NASOD_ELEMENT_FIRE_SOLO":
                case "NUI_NASOD_ELEMENT_ICE":
                case "NUI_NASOD_ELEMENT_ICE_B":
                case "NUI_NASOD_ELEMENT_ICE_SOLO":
                case "NUI_NASOD_ELITE_GUARD":
                case "NUI_NASOD_ELITE_SHERIFF":
                case "NUI_NASOD_GUARD":
                case "NUI_NASOD_GUARDIAN_SMALL":
                case "NUI_NASOD_HEALER":
                case "NUI_NASOD_HEALER_BLADER":
                case "NUI_NASOD_MAKER":
                case "NUI_NASOD_MINER_FAT_B":
                case "NUI_NASOD_MINER_FAT_B_AIR":
                case "NUI_NASOD_MINER_FAT_C":
                case "NUI_NASOD_MINER_FAT_C_AIR":
                case "NUI_NASOD_MINER_THIN":
                case "NUI_NASOD_MINER_THIN_AIR":
                case "NUI_NASOD_MOUSE":
                case "NUI_NASOD_MOUSE_HOUSE":
                case "NUI_NASOD_RAIL":
                case "NUI_NASOD_RAIL_CART":
                case "NUI_NASOD_SHERIFF":
                case "NUI_NASOD_SPRAY":
                case "NUI_NASOD_SPRAY_ICE":
                case "NUI_NASOD_WATCH":
                case "NUI_PEITA_CHAPEL_GATE":
                case "NUI_SHOCK_STICK":
                case "NUI_STEAM_CONTROLER":
                case "NUI_STEAM_TRAP":
                case "NUI_STEAM_TRAP_DOWN":
                case "NUI_STEAM_TRAP_SIDE":
                case "NUI_STEAM_TRAP_UP":
                case "NUI_TRAP_CHAIN":
                case "NUI_TRAP_CHAIN_RED":
                case "NUI_WALLY_8TH":
                case "NUI_WALLY_8TH_EVENT":
                case "NUI_WALLY_8TH_MK2":
                case "NUI_WALLY_9TH":
                case "NUI_WALLY_9TH_ARMOR":
                case "NUI_WALLY_9TH_TYPE2":
                    return ImpactTone.Metal;
                case "NUI_ANCIENTNASOD_GUARD":
                case "NUI_GOLEM_BOSS":
                case "NUI_GOLEM_STONE":
                case "NUI_GOLEM_STONE_RAGE":
                    return ImpactTone.DenseStone;
                case "NUI_ANCIENTNASOD_SENTRY":
                case "NUI_ANCIENTNASOD_SENTRY_MINI":
                case "NUI_AVALANCHE_ICE_PILLAR_BIG":
                case "NUI_AVALANCHE_ICE_PILLAR_SMALL":
                case "NUI_AVALANCHE_STATUE":
                case "NUI_BUNKER":
                case "NUI_CHILLING_SPINE_TRAP":
                case "NUI_DOWN_STONE_TRAP":
                case "NUI_DOWN_STONE_TRAP_BESMA":
                case "NUI_EVENT_NPC_ALTERA_CORE_BOSS":
                case "NUI_EVENT_NPC_ANCIENT_PPORU":
                case "NUI_EVENT_NPC_BATTLE_AIR_SHIP":
                case "NUI_EVENT_NPC_BENDERS_ELDER":
                case "NUI_EVENT_NPC_BERTHE":
                case "NUI_EVENT_NPC_DARK_NEPHILIM":
                case "NUI_EVENT_NPC_DULLAHAN":
                case "NUI_EVENT_NPC_HAMEL_FROZEN_WATER_HALL":
                case "NUI_EVENT_NPC_HAMEL_RESIAM":
                case "NUI_EVENT_NPC_LIZARDMAN_SHAMAN_KING":
                case "NUI_EVENT_NPC_NASOD_FACTORY":
                case "NUI_EVENT_NPC_VELDER_BRIDGE_HOPE":
                case "NUI_EVENT_NPC_WALLY_9TH_ARMOR":
                case "NUI_EVENT_NPC_WILLIAM_PPORU":
                case "NUI_GATE_FLYING_STONE":
                case "NUI_GOCHI_A":
                case "NUI_GOCHI_B":
                case "NUI_ICE_PILLAR_TRAP":
                case "NUI_ICE_ROCK":
                case "NUI_NASOD_RAIL_HOUSE":
                case "NUI_PEITA_OFFERINGS_GATE":
                case "NUI_PEITA_OFFERINGS_GATE_POST":
                case "NUI_PEITA_SPIRAL_CORRIDOR_GATE":
                case "NUI_PEITA_SPIRAL_CORRIDOR_GATE_POST":
                case "NUI_PEITA_UNDER_GARDEN_GATE_BOSS_POST":
                case "NUI_PEITA_UNDER_GARDEN_GATE_POST":
                case "NUI_RUBEN_RUIN_GATE":
                case "NUI_SCORPION":
                case "NUI_SCORPION_FIRE":
                case "NUI_TRAP_COLUMN":
                case "NUI_VELDER_DWELLING_GATE_POST":
                case "NUI_VELDER_MARKET_WALL_BREAK1":
                case "NUI_VELDER_MARKET_WALL_BREAK2":
                case "NUI_VELDER_MARKET_WALL_BREAK3":
                case "NUI_VELDER_STAGE6_BOSS_TRAP":
                    return ImpactTone.Shell;
                case "NUI_MOUSEHOUSE":
                case "NUI_PIRANHA_HOUSE":
                case "NUI_WATERWAY_CORE_RESPAWNER":
                    return ImpactTone.Hive;
                case "NUI_ANGER_FOREST_ENT":
                case "NUI_ANGER_FOREST_KNIGHT":
                case "NUI_BARRICADE_DEMON":
                case "NUI_BESMA_TOTEM_BASE":
                case "NUI_BLACK_FLOWER_FRONT":
                case "NUI_BLACK_FLOWER_TRAP":
                case "NUI_BOX":
                case "NUI_BOX_ALTERA":
                case "NUI_BOX_BESMA1":
                case "NUI_BOX_BESMA2":
                case "NUI_BOX_HAMEL":
                case "NUI_BOX_PEITA":
                case "NUI_BOX_VELDER":
                case "NUI_CACTUS":
                case "NUI_CHEST":
                case "NUI_ELDER_DRUID_FOREST_GATE":
                case "NUI_PD_MONSTER_B":
                case "NUI_PEITA_UNDER_GARDEN_GATE":
                case "NUI_PEITA_UNDER_GARDEN_GATE_BOSS_STEM":
                case "NUI_PEITA_UNDER_GARDEN_GATE_STEM":
                case "NUI_RUBEN_EL_TREE_GATE":
                case "NUI_RUBEN_EL_TREE_GATE_POST":
                case "NUI_RUBEN_SWAMP_GATE":
                case "NUI_SHADOW_TRICKSTER_TOTEM_B":
                case "NUI_SHADOW_TRICKSTER_TOTEM_C":
                case "NUI_SIEGE_TOWER":
                case "NUI_SMALLGATE_BENDERS_CAVE_A":
                case "NUI_SMALLGATE_BESMA_AIR_SHIP":
                case "NUI_SMALLGATE_BESMA_DRAGON_NEST":
                case "NUI_SMALLGATE_BESMA_DRAGON_ROAD":
                case "NUI_SMALLGATE_BESMA_LAKE":
                case "NUI_SMALLGATE_BESMA_LAKE_NIGHT":
                case "NUI_SMALLGATE_BESMA_MINE":
                case "NUI_SMALLGATE_BESMA_MINE_A":
                case "NUI_SMALLGATE_BOSS_BENDERS_CAVE":
                case "NUI_SMALLGATE_BOSS_BESMA_AIR_SHIP":
                case "NUI_SMALLGATE_BOSS_BESMA_MINE":
                case "NUI_SMALLGATE_POST_BENDERS_CAVE_A":
                case "NUI_SMALLGATE_POST_BESMA_AIR_SHIP":
                case "NUI_SMALLGATE_POST_BESMA_DRAGON_NEST":
                case "NUI_SMALLGATE_POST_BESMA_DRAGON_ROAD":
                case "NUI_SMALLGATE_POST_BESMA_LAKE":
                case "NUI_SMALLGATE_POST_BESMA_LAKE_NIGHT":
                case "NUI_SMALLGATE_POST_BESMA_MINE":
                case "NUI_SMALLGATE_POST_BESMA_MINE_A":
                case "NUI_SMALLGATE_POST_WALLY_CASTLE_A":
                case "NUI_SMALLGATE_WALLY_CASTLE_A":
                case "NUI_STEAM_GEYSER":
                case "NUI_TRAP_CROSSBOW":
                case "NUI_TREE_KNIGHT":
                case "NUI_VELDER_MARKET_BOSS_GATE":
                case "NUI_VELDER_MARKET_GATE":
                case "NUI_VELDER_STAGE6_BOSS_TRAP_SWITCH":
                case "NUI_WOOD_POST_TRAP":
                    return ImpactTone.Wood;
                case "NUI_ANGER_FOREST_FAIRY":
                case "NUI_CURSE_WISP":
                case "NUI_FAIRY_GUARDIAN":
                case "NUI_FOREST_DARK_MAGICIAN":
                case "NUI_GHOST_MAGICIAN_RAGGED":
                case "NUI_SHADOW_MASTER_DARK_BALL":
                case "NUI_SPRIGGAN_LIGHT_BALL":
                    return ImpactTone.Spirit;
            }
            if(!string.IsNullOrEmpty(enemy.sourceNPC)) return ImpactTone.Flesh;
            // Older, source-ID-free practice rosters still identify their original visual family by kind.
            if(IsRecoveryContainer(enemy)) return ImpactTone.Wood;
            switch(enemy.kind) {
                case "benders-nasod": case "soldier-thin": case "soldier-fat":
                case "soldier-crossbow": case "soldier-benders": return ImpactTone.IronArmor;
                case "wally-8th": case "lattice-gate-below-passage": return ImpactTone.Metal;
                case "curse-wisp": case "fairy-guardian": case "anger-forest-fairy":
                case "forest-dark-magician": case "ghost-magician-ragged": return ImpactTone.Spirit;
                case "tree-knight": case "anger-forest-knight": case "anger-forest-ent":
                case "cactus": case "prop-box": case "prop-chest": case "elder-druid-forest-gate":
                case "prop-tree-gate": case "prop-tree-gate-post": return ImpactTone.Wood;
                case "mousehouse": return ImpactTone.Hive;
                case "down-stone-trap": case "scorpion": case "prop-ruins-gate": return ImpactTone.Shell;
                default: return ImpactTone.Flesh;
            }
        }

        void RecordEnemyImpact(Enemy enemy, HitReaction reaction, bool guard, int rawReaction) {
            var tone = EnemyTone(enemy);
            // The Castle infantry's plate and the Boss's chassis use separate
            // contact sounds. They still share the metal voice budget.
            if(CurrentRoom.id!=null && CurrentRoom.id.StartsWith("castle-center-",StringComparison.Ordinal)) {
                if(tone==ImpactTone.IronArmor)tone=ImpactTone.SoldierPlate;
                else if(enemy.sourceNPC=="NUI_WALLY_8TH")tone=ImpactTone.CastleMachine;
            }
            // Gochi maps every living hurt reaction to GOCHI_A/B_DAMAGE. The region AI yields
            // to its matching generic hurt pose, so its source SOUND_PLAY0 is carried by this hit.
            if(enemy.hp>0 && reaction!=HitReaction.None &&
                (enemy.state==CombatState.Hurt || enemy.state==CombatState.Knockdown || enemy.state==CombatState.Airborne) &&
                (enemy.sourceNPC=="NUI_GOCHI_A" || enemy.sourceNPC=="NUI_GOCHI_B")) tone=ImpactTone.CocoonHurt;
            if(guard) {
                switch(enemy.sourceNPC) {
                    case "NUI_DULLAHAN": tone=ImpactTone.KnightShield; break;
                    case "NUI_GLITER_SHIELD": case "NUI_GLITER_SHIELD_ELITE": case "NUI_GLITER_SHIELD_GREAT":
                        tone=ImpactTone.GlitterShield; break;
                }
            }
            if(enemyImpactSounds.Count > 0 && enemyImpactSounds[enemyImpactSounds.Count-1].serial >= EnemyImpactSerial)
                enemyImpactSounds.Clear();
            bool heavy=reaction==HitReaction.Launch || reaction==HitReaction.Knockdown ||
                (rawReaction==2 && (tone==ImpactTone.SoldierPlate || tone==ImpactTone.CastleMachine));
            enemyImpactSounds.Add(new EnemyImpactSound {
                serial=EnemyImpactSerial, room=room, material=ImpactFamily(tone), tone=tone,
                soundActor=NpcSoundActorId(enemy), heavy=heavy,
                x=enemy.x, y=enemy.y, time=elapsed
            });
            if(enemyImpactSounds.Count > 128) enemyImpactSounds.RemoveRange(0, enemyImpactSounds.Count-128);
        }
    }
}
