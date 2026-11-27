using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcSourceEventTransition {
        public string condition,nextState;
    }
    [Serializable] public sealed class NpcSourceArmorWindow {
        public float start,end;
    }

    public sealed partial class GameModel {
        // These are source event predicates, not animation-duration aliases.
        // The source NPC runner calls them against the actual physics body.
        public static void ValidateNpcSourceEventsV17(string stateId,float duration,
            NpcSourceEventTransition[] transitions,NpcSourceArmorWindow[] armorWindows,
            float entryOffsetY,ISet<string> localStates=null) {
            if(string.IsNullOrEmpty(stateId)||!Finite(duration)||duration<=0||!Finite(entryOffsetY))
                throw new ArgumentException("Invalid source event state or entry offset.");
            var unique=new HashSet<string>();
            if(transitions!=null)foreach(var transition in transitions){
                if(transition==null||string.IsNullOrEmpty(transition.nextState)||
                    !unique.Add(transition.condition)||
                    transition.condition!="SCT_POSITIVE_Y_SPEED"&&
                    transition.condition!="SCT_NEGATIVE_Y_SPEED"&&
                    transition.condition!="SCT_FOOT_ON_LINE_TRUE"&&
                    transition.condition!="SCT_FOOT_ON_LINE_FALSE_DOWN"||
                    localStates!=null&&!localStates.Contains(transition.nextState))
                    throw new ArgumentException("Invalid source physics event transition.");
            }
            if(armorWindows!=null)foreach(var window in armorWindows)
                if(window==null||!Finite(window.start)||!Finite(window.end)||window.start<0||window.end<window.start)
                    throw new ArgumentException("Invalid source finite super-armor window.");
        }

        public static string SourceNpcEventTargetV17(NpcSourceEventTransition[] transitions,float velocityY,bool grounded) {
            if(transitions==null)return null;
            if(!Finite(velocityY))throw new ArgumentException("Invalid source event velocity.");
            foreach(var transition in transitions){
                bool accepted=transition.condition=="SCT_POSITIVE_Y_SPEED"?velocityY>0:
                    transition.condition=="SCT_NEGATIVE_Y_SPEED"?velocityY<0:
                    transition.condition=="SCT_FOOT_ON_LINE_TRUE"?grounded:
                    transition.condition=="SCT_FOOT_ON_LINE_FALSE_DOWN"&&!grounded&&velocityY<0;
                if(accepted)return transition.nextState;
            }
            return null;
        }

        // Native SUPER_ARMOR_NOT_RED affects the outline only. Retain ordinary
        // armor damage and control behavior for these exact source states.
        static readonly HashSet<string> sourceNonRedArmorStates=new HashSet<string> {
            "NUI_BARRIERMACHINE_FIRE/BARRIER_CREATEMACHINE_FIRE_DEFENCE_UP",
            "NUI_BARRIERMACHINE_FIRE/BARRIER_CREATEMACHINE_FIRE_RESPAWN",
            "NUI_BARRIERMACHINE_FIRE/BARRIER_CREATEMACHINE_FIRE_START",
            "NUI_BARRIERMACHINE_ICE/BARRIER_CREATEMACHINE_ICE_DEFENCE_UP",
            "NUI_BARRIERMACHINE_ICE/BARRIER_CREATEMACHINE_ICE_RESPAWN",
            "NUI_BARRIERMACHINE_ICE/BARRIER_CREATEMACHINE_ICE_START",
            "NUI_CANNON_BIG/CANNON_BIG_ATTACK_1",
            "NUI_CANNON_BIG/CANNON_BIG_ATTACK_2",
            "NUI_CANNON_BIG/CANNON_BIG_WAIT",
            "NUI_CANNON_HEAVY_MACHINE_GUN/CANNON_HEAVY_MACHINE_GUN_ATTACK_1",
            "NUI_CANNON_HEAVY_MACHINE_GUN/CANNON_HEAVY_MACHINE_GUN_ATTACK_2",
            "NUI_CANNON_HEAVY_MACHINE_GUN/CANNON_HEAVY_MACHINE_GUN_ATTACK_3",
            "NUI_CANNON_HEAVY_MACHINE_GUN/CANNON_HEAVY_MACHINE_GUN_WAIT",
            "NUI_CANNON_MINI/CANNON_MINI_ATTACK_1",
            "NUI_CANNON_MINI/CANNON_MINI_ATTACK_2",
            "NUI_CANNON_MINI/CANNON_MINI_ATTACK_3",
            "NUI_CANNON_MINI_UP/CANNON_MINI_UP_ATTACK_1",
            "NUI_CANNON_MINI_UP/CANNON_MINI_UP_ATTACK_2",
            "NUI_CANNON_MINI_UP/CANNON_MINI_UP_ATTACK_3",
            "NUI_EL_EXTRACTOR_EARTH/EL_EXTRACTOR_EARTH_START",
            "NUI_EL_EXTRACTOR_FIRE/EL_EXTRACTOR_FIRE_START",
            "NUI_EL_EXTRACTOR_ICE/EL_EXTRACTOR_ICE_START",
            "NUI_EL_EXTRACTOR_NORMAL_A/EL_EXTRACTOR_NORMAL_A_START",
            "NUI_EL_EXTRACTOR_NORMAL_A/EL_EXTRACTOR_NORMAL_A_WAIT",
            "NUI_EL_EXTRACTOR_NORMAL_B/EL_EXTRACTOR_NORMAL_B_START",
            "NUI_EL_EXTRACTOR_NOVA/EL_EXTRACTOR_NOVA_START",
            "NUI_EL_POWER_PLANT/EL_POWER_PLANT_DOWN",
            "NUI_EL_POWER_PLANT/EL_POWER_PLANT_START",
            "NUI_EL_POWER_PLANT/EL_POWER_PLANT_UP",
            "NUI_LIZARDMAN_SHAMAN_KING/LIZARDMAN_SHAMAN_KING_START_READY",
            "NUI_NASOD_KING/NASOD_KING_ATTACK1_LEFT",
            "NUI_NASOD_KING/NASOD_KING_ATTACK1_RIGHT",
            "NUI_NASOD_KING/NASOD_KING_ATTACK2",
            "NUI_NASOD_KING/NASOD_KING_ATTACK3_LEFT",
            "NUI_NASOD_KING/NASOD_KING_ATTACK3_RIGHT",
            "NUI_NASOD_KING/NASOD_KING_ATTACK_PATTERN1",
            "NUI_NASOD_KING/NASOD_KING_ATTACK_PATTERN2",
            "NUI_NASOD_KING/NASOD_KING_COMBO1",
            "NUI_NASOD_KING/NASOD_KING_COMBO2",
            "NUI_NASOD_KING/NASOD_KING_COMBO3",
            "NUI_NASOD_KING/NASOD_KING_COMBO4",
            "NUI_NASOD_KING/NASOD_KING_COMBOEND",
            "NUI_NASOD_KING/NASOD_KING_COMBOREADY",
            "NUI_NASOD_KING/NASOD_KING_DIE",
            "NUI_NASOD_KING/NASOD_KING_KNOCKDOWN",
            "NUI_NASOD_KING/NASOD_KING_KNOCKDOWN2",
            "NUI_NASOD_KING/NASOD_KING_KNOCKDOWN_WAIT",
            "NUI_NASOD_KING/NASOD_KING_LASER",
            "NUI_NASOD_KING/NASOD_KING_LOSE",
            "NUI_NASOD_KING/NASOD_KING_MISSILE",
            "NUI_NASOD_KING/NASOD_KING_SPELLATTACK",
            "NUI_NASOD_KING/NASOD_KING_START",
            "NUI_NASOD_KING/NASOD_KING_START_HENIR",
            "NUI_NASOD_KING/NASOD_KING_START_READY",
            "NUI_NASOD_KING/NASOD_KING_WAIT",
            "NUI_NASOD_MAKER/NASOD_MAKER_RESPAWN",
            "NUI_NASOD_MAKER/NASOD_MAKER_WAIT",
            "NUI_NASOD_MAKER/NASOD_MAKER_WAIT_RESPAWN_READY",
            "NUI_RAVEN/RAVEN_START_READY",
            "NUI_SHOCK_STICK/SHOCK_STICK_ATTACK",
            "NUI_STEAM_CONTROLER/STEAM_TRAP_DAMAGE",
            "NUI_STEAM_CONTROLER/STEAM_TRAP_DYING",
            "NUI_STEAM_CONTROLER/STEAM_TRAP_WAIT",
            "NUI_WALLY_9TH_ARMOR/WALLY_9TH_ARMOR_START_READY",
        };

        public bool EnemyShowsArmorOutline(Enemy enemy) {
            if(enemy==null||enemy.hp<=0||!EnemyHasSuperArmor(enemy))return false;
            var state=NpcState(enemy);
            return state==null||!state.suppressArmorOutline;
        }

        public static bool SourceNpcArmorAtV17(bool baseArmor,NpcSourceArmorWindow[] windows,float stateTime) {
            if(!Finite(stateTime))throw new ArgumentException("Invalid source armor clock.");
            if(baseArmor)return true;
            if(windows!=null)foreach(var window in windows)
                if(stateTime>=window.start&&stateTime<=window.end)return true;
            return false;
        }

        // Old declarations use -1 for an unchanged component. Explicit source
        // SPEED_Y=-4000 is a downward impulse and must survive projection.
        public static bool SourceNpcVelocityHasComponentV17(float value,bool explicitSourceComponent) {
            if(!Finite(value))throw new ArgumentException("Invalid source velocity component.");
            return explicitSourceComponent||value>=0;
        }
    }
}
