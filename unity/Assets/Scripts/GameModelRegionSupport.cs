using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public sealed partial class RegionSpellInstance {
        internal Dictionary<Enemy,float> supportContacts;
    }

    internal sealed partial class RegionActorState {
        internal float defenceUntil, defenceFactor, supportSpeedUntil, supportSpeed;
        internal float extraMotion, previousExtraMotion;
        internal float MotionTime {get{return time*animSpeed+extraMotion;}}
        internal float PreviousMotionTime {get{return prev*animSpeed+previousExtraMotion;}}
    }

    public sealed partial class GameModel {
        bool HandleRegionSupport(RegionSpellInstance field) {
            var source=field.spell;
            if(!source.ownTeam)return false;
            // The barrier and totem are attached sustained fields. The thrown
            // alchemist potion remains on the ground for its own authored life.
            if((source.defenceFactor>0||source.speedUp)&&(!field.owner.active||field.owner.hp<=0)){
                field.stopped=true;
                return true;
            }
            if(source.frames==null||source.frames.Length==0||field.hits>=source.count)return true;
            float phase=source.loop&&source.span>0?field.age%source.span:field.age;
            var frame=source.frames[0];
            foreach(var candidate in source.frames){
                if(Math.Abs(candidate.time-phase)>Math.Abs(frame.time-phase))break;
                frame=candidate;
            }
            if(frame.spheres==null)return true;
            if(field.supportContacts==null)field.supportContacts=new Dictionary<Enemy,float>();
            foreach(var ally in enemies){
                if(!ally.active||ally.dormant||ally.hp<=0||ally.isGate||ally.encounterIndex!=field.owner.encounterIndex||
                    ally.regionalAlly!=field.owner.regionalAlly)continue;
                float next;
                if(field.supportContacts.TryGetValue(ally,out next)&&(!source.repeat||field.age<next))continue;
                if(!PeitaSphereHit(frame.spheres,field.x,field.y,field.facing,0,EnemyBounds(ally)))continue;
                if(source.healFraction>0)ally.hp=Math.Min(ally.maxHp,ally.hp+ally.maxHp*source.healFraction);
                var a=ally.regionActor;
                if(a!=null&&source.defenceFactor>0){
                    a.defenceFactor=source.defenceFactor;
                    a.defenceUntil=Math.Max(a.defenceUntil,elapsed+source.defenceTime);
                }
                if(a!=null&&source.speedUp&&source.speedUpRate>0){
                    a.supportSpeed=source.speedUpRate;
                    a.supportSpeedUntil=Math.Max(a.supportSpeedUntil,elapsed+source.gap);
                }
                field.supportContacts[ally]=field.age+source.gap;
                if(++field.hits>=source.count)break;
            }
            return true;
        }
    }
}
