using System;
namespace ElswordPixel {
    public sealed partial class GameModel {
        const string IceSoloV35="NUI_NASOD_ELEMENT_ICE_SOLO",IceStateV35="NASOD_ELEMENT_ICE_SOLO_";
        Enemy iceSourceHitActorV35;float iceSourceHitTimeV35;bool iceSourceHitBehindV35;
        internal int iceDownChoicesV35,iceDownCounterFrontV35,iceDownCounterBackV35;
        bool PlainIceSourceBirthV35(Enemy actor) {
            return actor!=null && actor.sourceNPC==IceSoloV35 && actor.kind=="nasod-element-ice-solo" &&
                room==1 && CurrentRoom.id=="Dungeon_Altera_Plain_Normal-1" && EncounterIndex==1 &&
                actor.encounterIndex==1 && actor.sourceKeyCode==0 && CurrentRoom.enemies.Count>26 &&
                CurrentRoom.enemies[26].sourceNPC==IceSoloV35 && enemies.Contains(actor) &&
                actor.active && !actor.dormant && actor.hp>0 && plainElementRouteR1315==PlainNormalR1258 &&
                PlainElementCallbackScopeR1315();
        }
        void RememberPlainIceRecoveryHitV35(Enemy actor,float attackerX) {
            if(!PlainIceSourceBirthV35(actor))return;
            iceSourceHitActorV35=actor;iceSourceHitTimeV35=elapsed;
            iceSourceHitBehindV35=(attackerX-actor.x)*actor.facing<0;
        }
        string PlainIceDownEntryV35(Fighter f,bool playerActor) {
            if(playerActor)return null;
            var actor=f as Enemy;if(!PlainIceSourceBirthV35(actor))return null;
            if(actor.state==CombatState.Airborne&&actor.knockdownOnLanding)return IceStateV35+"DAMAGE_AIR_DOWN_LANDING";
            bool rear=ReferenceEquals(iceSourceHitActorV35,actor)&&iceSourceHitTimeV35==elapsed&&iceSourceHitBehindV35;
            return IceStateV35+(rear?"DAMAGE_DOWN_BACK":"DAMAGE_DOWN_FRONT");
        }
        void BeginPlainIceDownV35(Enemy actor,string stateId) {
            var st=npcStates[IceSoloV35+"/"+stateId];
            if(st.loop||st.sourceAnimSpeed!=1||st.duration<=0||!st.superArmor)
                throw new InvalidOperationException("Original ice-solo guarded one-play down state "+stateId);
            EnterNpcState(actor,st);
            actor.downTime=st.duration;actor.recoveryTime=0;
        }
        bool PlainIceRecoveryDownActiveV35(Enemy actor) {
            if(!PlainIceSourceBirthV35(actor)||actor.state!=CombatState.Knockdown)return false;
            return actor.npcState==IceStateV35+"DAMAGE_DOWN_FRONT" || actor.npcState==IceStateV35+"DAMAGE_DOWN_BACK" ||
                actor.npcState==IceStateV35+"DAMAGE_AIR_DOWN_LANDING";
        }
        bool PlainIceBackGetUpV35(Enemy actor) {
            return PlainIceSourceBirthV35(actor) && actor.npcState==IceStateV35+"STAND_UP_BACK";
        }
        bool TickPlainIceRecoveryDownV35(Enemy actor,float packet) {
            if(!PlainIceRecoveryDownActiveV35(actor))return false;
            if(!Grounded(actor)){
                actor.state=CombatState.Airborne;actor.knockdownOnLanding=true;actor.downTime=actor.recoveryTime=0;
                EnterNpcState(actor,npcStates[IceSoloV35+"/"+IceStateV35+"DAMAGE_AIR_FALL"]);return true;
            }
            if(actor.hitPause>0){
                float frozen=Math.Min(packet,actor.hitPause);actor.hitPause-=frozen;packet-=frozen;
            }
            if(packet<=.000001f)return true;
            var st=NpcState(actor);float from=actor.npcTime,to=Math.Min(st.duration,from+packet);
            AdvanceNpcStateSounds(actor,st,from,to,elapsed-packet);
            actor.clock=actor.npcTime=to;actor.downTime=Math.Max(0,st.duration-to);
            if(to<st.duration)return true;
            // SCT_CONDITION_TABLE occurs before the original SCT_MOTION_END. Each
            // of these three down states owns one ANIM_PLAY_COUNT=1, RATE=10 roll.
            bool rear=st.id==IceStateV35+"DAMAGE_DOWN_BACK";bool counter=NpcRoll()<10;
            iceDownChoicesV35++;
            if(counter){if(rear)iceDownCounterBackV35++;else iceDownCounterFrontV35++;}
            string id=IceStateV35+(counter?"STAND_UP_ATTACK_":"STAND_UP_")+(rear?"BACK":"FRONT");
            var next=npcStates[IceSoloV35+"/"+id];
            if(counter && (next.hits.Length!=1 || Math.Abs(next.hits[0].start-(rear?.40f:.20f))>.00001f ||
                Math.Abs(next.hits[0].end-(rear?.47f:.27f))>.00001f))
                throw new InvalidOperationException("Original ice-solo counter damage window "+id);
            actor.state=counter?CombatState.Normal:CombatState.GettingUp;actor.stun=actor.downTime=0;
            actor.recoveryTime=counter?0:next.duration;EnterNpcState(actor,next);
            return true;
        }
        void ClearPlainIceRecoveryModelV35() {
            iceSourceHitActorV35=null;iceSourceHitTimeV35=0;iceSourceHitBehindV35=false;
            iceDownChoicesV35=iceDownCounterFrontV35=iceDownCounterBackV35=0;
        }
    }
}
