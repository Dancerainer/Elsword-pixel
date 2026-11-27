using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcPlayerStatusEvent {public string type;public float time,amount,remaining;}
    public sealed partial class GameModel {
        double npcBurnEnd,npcBurnNext;float npcBurnPerSecond;
        public readonly List<NpcPlayerStatusEvent> npcPlayerStatusEvents=new List<NpcPlayerStatusEvent>();
        public float NpcPlayerBurnRemaining {get{return (float)Math.Max(0,npcBurnEnd-elapsed);}}
        void ClearNpcPlayerStatuses(){ClearNpcManaDrainV17();npcBurnEnd=npcBurnNext=0;npcBurnPerSecond=0;npcPlayerStatusEvents.Clear();}
        static void ValidateNpcPlayerStatus(NpcEffectDefinition d){
            ValidateNpcManaDrainV17(d.onHitManaDrainDuration,d.onHitManaDrainPerSecond);
            if(!Finite(d.onHitBurnDuration)||d.onHitBurnDuration<0||!Finite(d.onHitBurnPerSecond)||d.onHitBurnPerSecond<0||
               (d.onHitBurnDuration>0)!=(d.onHitBurnPerSecond>0))throw new ArgumentException("Partial or invalid NPC fire status payload");
        }
        void NpcStatusEvent(string type,float at,float amount){
            if(!traceNpcEffects)return;if(npcPlayerStatusEvents.Count>=512)npcPlayerStatusEvents.RemoveAt(0);
            npcPlayerStatusEvents.Add(new NpcPlayerStatusEvent{type=type,time=at,amount=amount,remaining=(float)Math.Max(0,npcBurnEnd-at)});
        }
        void ApplyNpcPlayerStatus(NpcEffectDefinition d,float at){
            ApplyNpcManaDrainV17(d.onHitManaDrainDuration,d.onHitManaDrainPerSecond,at);
            if(d.onHitBurnDuration<=0||player.hp<=0)return;
            bool active=npcBurnEnd>=at&&npcBurnPerSecond>0;
            npcBurnEnd=at+d.onHitBurnDuration;npcBurnPerSecond=d.onHitBurnPerSecond;
            // Repeated successful hits refresh duration, without stacking or
            // postponing every pending tick indefinitely during a multihit.
            if(!active)npcBurnNext=at+1;
            NpcStatusEvent(active?"fire-refresh":"fire-start",at,0);
        }
        void TickNpcPlayerStatuses(){
            TickNpcManaDrainV17();
            if(npcBurnPerSecond<=0)return;
            if(player.hp<=0){npcBurnEnd=npcBurnNext=0;npcBurnPerSecond=0;return;}
            // This is the existing demo burn convention: one-second resource
            // deltas, not attacks. The original MIX_ATTACK formula/scheduler
            // is not proved. The scoped source table explicitly authors
            // .003 * equal P/M28 = .084HP/s, retaining fractional HP.
            while(npcBurnNext<=elapsed+.000001&&npcBurnNext<=npcBurnEnd+.000001){
                float amount=Math.Min(player.hp,npcBurnPerSecond);ApplyCombatHealth(player,Math.Max(0,player.hp-amount));SpawnResourceDamage(player,amount);
                NpcStatusEvent("fire-tick",(float)npcBurnNext,amount);npcBurnNext+=1;
                if(player.hp<=0){player.state=CombatState.Dead;player.clip="death";player.clock=0;player.stun=0;CancelAction(true);npcBurnEnd=npcBurnNext=0;npcBurnPerSecond=0;return;}
            }
            if(elapsed>=npcBurnEnd){npcBurnEnd=npcBurnNext=0;npcBurnPerSecond=0;}
        }
    }
}
