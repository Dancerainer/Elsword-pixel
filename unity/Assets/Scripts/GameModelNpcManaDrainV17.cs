using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        double npcManaDrainEndV17,npcManaDrainNextV17;
        float npcManaDrainPerSecondV17;
        public float NpcPlayerManaDrainRemaining {get{return (float)Math.Max(0,npcManaDrainEndV17-elapsed);}}

        static void ValidateNpcManaDrainV17(float duration,float perSecond){
            if(!Finite(duration)||duration<0||!Finite(perSecond)||perSecond<0||(duration>0)!=(perSecond>0))
                throw new ArgumentException("Partial or invalid original NPC mana-drain status.");
        }
        void ClearNpcManaDrainV17(){npcManaDrainEndV17=npcManaDrainNextV17=0;npcManaDrainPerSecondV17=0;}
        void SourceManaDrainEventV17(string type,float at,float amount){
            if(!traceNpcEffects)return;if(npcPlayerStatusEvents.Count>=512)npcPlayerStatusEvents.RemoveAt(0);
            npcPlayerStatusEvents.Add(new NpcPlayerStatusEvent{type=type,time=at,amount=amount,remaining=(float)Math.Max(0,npcManaDrainEndV17-at)});
        }
        void ApplyNpcManaDrainV17(float duration,float perSecond,float at){
            if(duration<=0||player.hp<=0)return;
            ValidateNpcManaDrainV17(duration,perSecond);
            bool active=npcManaDrainEndV17>=at&&npcManaDrainPerSecondV17>0;
            npcManaDrainEndV17=at+duration;npcManaDrainPerSecondV17=perSecond;
            if(!active)npcManaDrainNextV17=at+1;
            SourceManaDrainEventV17(active?"curse-refresh":"curse-start",at,0);
        }
        void TickNpcManaDrainV17(){
            if(npcManaDrainPerSecondV17<=0)return;
            if(player.hp<=0){ClearNpcManaDrainV17();return;}
            // Source BCT_FIX_VALUE / -10MP per second for five normal seconds.
            // This uses the existing status resource-delta clock convention;
            // it is independent of weapon hits and cannot make MP negative.
            while(npcManaDrainNextV17<=elapsed+.000001&&npcManaDrainNextV17<=npcManaDrainEndV17+.000001){
                float amount=Math.Min(mana,npcManaDrainPerSecondV17);mana=Math.Max(0,mana-amount);
                SourceManaDrainEventV17("curse-tick",(float)npcManaDrainNextV17,amount);npcManaDrainNextV17+=1;
            }
            if(elapsed>=npcManaDrainEndV17)ClearNpcManaDrainV17();
        }
    }
}
