using System;
using System.Runtime.CompilerServices;

namespace ElswordPixel {
    public sealed partial class GameModel {
        readonly ConditionalWeakTable<Enemy,object> departedSoloElementsV45=new ConditionalWeakTable<Enemy,object>();
        readonly ConditionalWeakTable<Enemy,object> cloneComboContactsV45=new ConditionalWeakTable<Enemy,object>();
        static readonly string[] giantCounterStatesV45={"_ATTACK_A_READY","_ATTACK_B_READY"};
        static readonly string[] williamCounterStatesV45={"_ATTACK_A_READY","_ATTACK_B_READY","_SPECIAL_ATTACK_READY"};
        static bool CloneComboV45(string state){return state=="RAVEN_CLONE_COMBO_Z2"||state=="RAVEN_CLONE_B_COMBO_Z2";}
        bool RememberCloneComboHitV45(Enemy actor,NpcStateDefinition state){
            if(!CloneComboV45(state.id))return false;
            cloneComboContactsV45.GetValue(actor,_=>new object());
            return true;
        }
        bool TryCloneSpecialFollowUpV45(Enemy actor,NpcStateDefinition state,float from,float to){
            if(!CloneComboV45(state.id)||!cloneComboContactsV45.TryGetValue(actor,out _))return false;
            float threshold=.4f/state.sourceAnimSpeed;
            if(from>=threshold||to<threshold||NpcRoll()>=20)return false;
            return TryEnterNpcState(actor,npcStates[actor.sourceNPC+"/"+state.onHitState]);
        }

        void CheckOriginalGroundCounterV45(Enemy actor,NpcMechanismActor mechanism,float dt) {
            if(actor.state!=CombatState.Hurt || actor.stun<=0 || !Grounded(actor) ||
               !string.IsNullOrEmpty(actor.npcState) || mechanism.pending!=null)return;
            string prefix;float interval;string[] choices;
            switch(actor.sourceNPC){
                case "NUI_PPORU_GIANT_RED": prefix="PPORU_GIANT_RED"; interval=.5f; choices=giantCounterStatesV45; break;
                case "NUI_PPORU_WILLIAM_B": prefix="PPORU_WILLIAM_B"; interval=.03f; choices=williamCounterStatesV45; break;
                case "NUI_PPORU_WILLIAM_C": prefix="PPORU_WILLIAM_C"; interval=.03f; choices=williamCounterStatesV45; break;
                default:return;
            }
            if(Math.Floor((actor.clock+.000001f)/interval)>=Math.Floor((actor.clock+dt+.000001f)/interval))return;
            float dx=player.x-actor.x,dy=player.y-actor.y;
            if(dx*dx+dy*dy>140f*140f)return;
            foreach(string suffix in choices){
                if(NpcRoll()>=5||!TryRequestNpcState(actor,prefix+suffix))continue;
                mechanism.pendingDamage=true;
                return;
            }
        }

        bool TryFarCannonV45(Enemy actor,NpcStateDefinition state,float distance) {
            if(actor.sourceNPC!="NUI_CANNON_BIG" || state.id!="CANNON_BIG_WAIT" ||
               distance<=1000f*.28f || NpcRoll()>=50)return false;
            NpcStateDefinition attack;
            return npcStates.TryGetValue("NUI_CANNON_BIG/CANNON_BIG_ATTACK_1",out attack) &&
                   TryEnterNpcState(actor,attack);
        }

        static string SoloElementV45(Enemy actor) {
            if(actor.sourceNPC=="NUI_NASOD_ELEMENT_FIRE_SOLO")return "NASOD_ELEMENT_FIRE_SOLO";
            if(actor.sourceNPC=="NUI_NASOD_ELEMENT_ICE_SOLO")return "NASOD_ELEMENT_ICE_SOLO";
            return null;
        }
        bool TrySoloEscapeV45(Enemy actor) {
            if(actor.hp>=actor.maxHp*.10f || actor.hp<=0)return false;
            string prefix=SoloElementV45(actor);NpcStateDefinition teleport;
            return prefix!=null && npcStates.TryGetValue(actor.sourceNPC+"/"+prefix+"_TELEPORT",out teleport) &&
                   TryEnterNpcState(actor,teleport);
        }
        bool CompleteSoloEscapeV45(Enemy actor,NpcStateDefinition state,float elapsedState) {
            if(!state.id.EndsWith("_TELEPORT",StringComparison.Ordinal)||elapsedState+1e-6f<state.duration||
               state.id!=SoloElementV45(actor)+"_TELEPORT")return false;
            // Original TELEPORT_STATE_END: SetShow(false), SetNowHP_LUA(0).
            departedSoloElementsV45.GetValue(actor,_=>new object());
            TraceNpc(actor,"original-teleport-end");
            actor.hp=0;actor.state=CombatState.Dead;actor.stun=0;
            InterruptNpcAction(actor);actor.clip="death";actor.clock=0;
            return true;
        }
    }
}
