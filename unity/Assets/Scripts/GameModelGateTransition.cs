using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        // Presentation pacing adapter: source clear/exit/weighted destinations
        // stay intact. The opening interval runs before changing rooms/zones.
        public const float StageClearDuration = .95f;
        public const float CurtainBreakDuration = .55f;
        public const float GateTransitionDelay = StageClearDuration + CurtainBreakDuration + .15f;
        public const float ManualGatePause = 1.55f;
        public bool RoomCombatClear {
            get {
                if(HasEncounters)return EncounterCombatClear;
                foreach(var e in enemies)if(e.countsForClear&&!e.passive&&!e.isGate&&!IsRecoveryContainer(e)&&e.hp>0)return false;
                return true;
            }
        }
        public bool ExitGateProtected(Enemy gate) {
            return gate!=null&&gate.isGate&&gate.hp>0&&
                (HasEncounters&&gate.encounterIndex!=EncounterIndex||!RoomCombatClear);
        }
        public Enemy UnbrokenExitGate {
            get {foreach(var e in enemies)if(e.isGate&&e.active&&!e.dormant&&e.hp>0&&(!HasEncounters||e.encounterIndex==EncounterIndex))return e;return null;}
        }
        public Enemy RequiredExitGate() {
            if (gateOpen || !RoomCombatClear) return null;
            if (!HasEncounters) return UnbrokenExitGate;
            Enemy nearest=null;
            foreach (var e in enemies) {
                if (!e.active || e.dormant || e.passive || !e.isGate || e.hp<=0 || e.encounterIndex!=currentEncounter) continue;
                foreach (var clause in CurrentEncounter.clauses) {
                    if (!ClauseRequiresGate(clause,e)) continue;
                    if (nearest==null || Math.Abs(e.x-player.x)<Math.Abs(nearest.x-player.x)) nearest=e;
                    break;
                }
            }
            return nearest;
        }
        static bool ClauseRequiresGate(EncounterClearClause clause,Enemy gate) {
            return (clause.mode==EncounterClearMode.KillNpcTypes && Array.IndexOf(clause.npcTypes,gate.sourceNPC)>=0) ||
                   (clause.mode==EncounterClearMode.KillNpcKeyCodes && gate.sourceKeyCode>0 && Array.IndexOf(clause.npcKeyCodes,gate.sourceKeyCode)>=0);
        }
        bool BrokeExitGate(EncounterClearClause clause) {
            foreach (var e in enemies)
                if (e.active && e.isGate && e.hp<=0 && (!HasEncounters || e.encounterIndex==currentEncounter && ClauseRequiresGate(clause,e))) return true;
            return false;
        }
        float stageClearAge=-1, openingAge=-1, shatterAt, passageAt;
        EncounterClearClause gateTransitionClause;
        bool autoEnterAfterGateBreak;
        public float StageClearAge {get{return stageClearAge;}}
        public float CurtainBreakAge {get{return openingAge<0 ? -1 : openingAge-shatterAt;}}
        public float GateTransitionDelayRemaining {get{return gateOpen && !victory && player.hp>0 && openingAge>=0 ? Math.Max(0,passageAt-openingAge) : 0;}}
        void ResetGateTransitionDelay(){stageClearAge=openingAge=-1;gateTransitionClause=null;autoEnterAfterGateBreak=false;}
        void AdvanceGateTransitionDelay(bool cleared,bool showClear,EncounterClearClause clause,float dt,bool changingRoom=false){
            if(player.hp<=0 || (!showClear && !cleared)){ResetGateTransitionDelay();return;}
            if(openingAge>=0 && gateTransitionClause!=clause) ResetGateTransitionDelay();
            stageClearAge=showClear ? (stageClearAge<0 ? 0 : Math.Min(StageClearDuration,stageClearAge+dt)) : -1;
            if(!cleared){openingAge=-1;gateTransitionClause=null;autoEnterAfterGateBreak=false;return;}
            if(openingAge<0 || gateTransitionClause!=clause){
                // A destructible gate can be struck long after the CLEAR lettering faded.
                // Time the break and the camera hold from the actual finishing blow.
                openingAge=0; gateTransitionClause=clause;
                shatterAt=showClear ? Math.Max(0,StageClearDuration-stageClearAge) : 0;
                float duration=showClear ? shatterAt+CurtainBreakDuration+.15f : 0;
                bool manualBreak=BrokeExitGate(clause);
                autoEnterAfterGateBreak=changingRoom && manualBreak;
                if(manualBreak) duration=Math.Max(duration,ManualGatePause);
                passageAt=Math.Max(duration,clause==null ? 0 : clause.delaySeconds);
            }else openingAge=Math.Min(openingAge+dt,passageAt);
        }
        void TickLegacyGateTransition(float dt){
            bool clear=RoomCombatClear;
            foreach(var e in enemies)if(e.isGate&&e.hp>0)e.invincible=clear?0:3600;
            gateOpen=clear&&UnbrokenExitGate==null;
            bool final=room==rooms.Count-1;AdvanceGateTransitionDelay(gateOpen && !final,!final && clear,null,dt,!final);
            if(!gateOpen||player.hp<=0)return;
            if(!autoEnterAfterGateBreak&&(!Grounded(player)||player.x<ExitX))return;
            if(final){ResetGateTransitionDelay();FinishDungeonScore();CancelAction(true);player.clip="idle";player.clock=0;return;}
            if(GateTransitionDelayRemaining>.00001f)return;
            RecoverBetweenRooms(room+1);
        }
    }
}
