using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    [Serializable] public sealed partial class RegionSpellDefinition {
        public string id,reaction,statusTemplate;
        public int statusHitLimit;
        public float life,span,vx,vy,gx,gy,power,gap,launch,statusTime;
        public float vxSpread,vySpread;
        public float dirAt,dirSpeed,curveSpeed;
        public bool ownTeam,speedUp;
        public float healFraction,defenceFactor,defenceTime,speedUpRate;
        public int count;
        public bool loop,repeat,noDamage;
        public RegionSpellFrame[] frames;
        public RegionSpellStatus[] status;
        public RegionSpellChild[] children;
    }
    [Serializable] public sealed class RegionSpellChild {public string id,trigger;public float at,x,y,angle;public bool inherit;}
    [Serializable] public sealed class RegionSpellStatus {public string id;public float value;}
    [Serializable] public sealed class RegionSpellFrame {public float time;public float[] spheres;}
    [Serializable] public sealed class RegionBone {public string id;public float[] points;}
    [Serializable] public sealed class RegionSpellSet {public string id;public RegionEmission[] shots;}
    [Serializable] public sealed class RegionEmission {public string id,bone;public float x,y,at;public bool trace;}
    public sealed partial class RegionSpellInstance {
        public string id;
        public float x,y,age;
        public int facing;
        internal float baseX,baseY,wait,nextHit;
        internal float angle;
        internal float heading,travelX,travelY;
        internal Fighter tracking;
        internal float targetX,targetY;
        internal bool trackingPoint;
        internal float vx,vy;
        internal int hits;
        internal bool launched,stopped;
        internal Enemy owner;
        internal RegionEmission attach;
        internal RegionSpellDefinition spell;
    }
    sealed class RegionAppliedStatus {
        internal float until,nextTick,hp,mp,animation=1;
        internal bool frozen,immobile,armor,reverse,noDash,noAttack,noJump,pressed,stunned;
        internal string effect;
        internal int hitsLeft;
    }
    public sealed partial class GameModel
    {
        public Action<string,float,float,int,float> RegionSpellContactArt;
        readonly List<RegionSpellInstance> regionSpells=new List<RegionSpellInstance>();
        readonly Dictionary<string,RegionAppliedStatus> regionStatuses=new Dictionary<string,RegionAppliedStatus>();
        readonly Dictionary<Fighter,RegionAppliedStatus> regionEnemyStuns=new Dictionary<Fighter,RegionAppliedStatus>();
        readonly List<string> expiredStatuses=new List<string>();
        float regionLinkerAnimation=1, regionLastPlayerDamage;
        float regionGravityUntil,regionGravityRadius,regionGravityTop,regionGravityX;
        public IReadOnlyList<RegionSpellInstance> RegionSpellInstances {get {return regionSpells;}}
        public bool RegionPlayerStatusActive(string damageEffect) {
            RegionAppliedStatus status;
            if(regionStatuses.TryGetValue(damageEffect,out status)&&status.until>elapsed)return true;
            foreach(var active in regionStatuses.Values)if(active.effect==damageEffect&&active.until>elapsed)return true;
            return false;
        }
        float RegionStatusAnimation {
            get {float value=regionLinkerAnimation;foreach(var s in regionStatuses.Values)if(s.until>elapsed)value=Math.Min(value,s.frozen?0:s.animation);return value;}
        }
        void ClearRegionSpells() { ResetRegionalBindings();regionSpells.Clear();regionEnemyStuns.Clear();regionStatuses.Clear();regionLinkerAnimation=1;regionLastPlayerDamage=0;regionGravityUntil=0; }
        Input RegionFilteredInput(Input input)
        {
            if(regionStatuses.Count==0)return input;
            bool reverse=false,noDash=false,noAttack=false,noMove=false,noJump=false;
            foreach(var s in regionStatuses.Values){
                if(s.until<=elapsed)continue;
                if(s.frozen||s.stunned)return EmptyInput;
                reverse|=s.reverse;noDash|=s.noDash;noAttack|=s.noAttack;noMove|=s.immobile;noJump|=s.noJump;
            }
            if(!reverse&&!noDash&&!noAttack&&!noMove&&!noJump)return input;
            if(noDash)airborneDash=false;
            return new Input{move=noMove?0:reverse?-input.move:input.move,drop=!noMove&&input.drop,
                dash=!noDash&&!noMove&&input.dash,dashPressed=!noDash&&!noMove&&input.dashPressed,jump=!noJump&&!noMove&&input.jump,
                potion=input.potion,potionPressedMask=input.potionPressedMask,awaken=input.awaken,
                attack=!noAttack&&input.attack,attackHeld=!noAttack&&input.attackHeld,attackX=!noAttack&&input.attackX,
                attackXHeld=!noAttack&&input.attackXHeld,attackXReleased=!noAttack&&input.attackXReleased,
                skill=noAttack?-1:input.skill,skillHeldMask=noAttack?0:input.skillHeldMask,skillReleasedMask=noAttack?0:input.skillReleasedMask};
        }
        void RegionStatusHit(Fighter victim)
        {
            RegionAppliedStatus status;
            if(victim==player){
                foreach(var s in regionStatuses.Values)if(s.hitsLeft>0&&--s.hitsLeft==0)s.until=elapsed;
            }else if(regionEnemyStuns.TryGetValue(victim,out status)&&status.hitsLeft>0&&--status.hitsLeft==0){
                regionEnemyStuns.Remove(victim);
            }
        }
        bool RegionEnemyStunned(Enemy e)
        {
            RegionAppliedStatus status;
            if(!regionEnemyStuns.TryGetValue(e,out status))return false;
            if(status.until>elapsed)return true;
            regionEnemyStuns.Remove(e);return false;
        }
        RegionPoint RegionBonePoint(Enemy e,string name)
        {
            var node=e.regionActor!=null?e.regionActor.node:null;
            if(node!=null&&node.bones!=null)foreach(var b in node.bones) {
                if(b.id!=name||b.points==null||b.points.Length<2)continue;
                int last=b.points.Length/2-1;
                float progress=Clamp(e.clock/Math.Max(.001f,node.duration),0,1)*last;
                int lo=Math.Min((int)progress,last),hi=Math.Min(last,lo+1);
                float mix=progress-lo;
                return new RegionPoint(e.x+e.facing*(b.points[2*lo]+(b.points[2*hi]-b.points[2*lo])*mix),
                    e.y+b.points[2*lo+1]+(b.points[2*hi+1]-b.points[2*lo+1])*mix);
            }
            return new RegionPoint(e.x,e.y);
        }
        void ScheduleRegionSet(Enemy e,string key,RegionPoint? origin=null)
        {
            RegionSpellSet set;
            if(e.regionActor==null||String.IsNullOrEmpty(key)||!e.regionActor.spec.emitters.TryGetValue(key,out set))return;
            foreach(var shot in set.shots) {
                var spell=AddRegionSpell(e,shot.id,e.x,e.y);
                if(spell==null)continue;
                if(origin.HasValue){
                    spell.baseX=origin.Value.x*.28f+e.facing*shot.x;spell.baseY=origin.Value.y*.28f+shot.y;
                }else spell.attach=shot;
                spell.wait=shot.at;
            }
        }
        RegionSpellInstance AddRegionSpell(Enemy e,string id,float x,float y)
        {
            RegionSpellDefinition def;
            if(e.regionActor==null||!e.regionActor.spec.projectiles.TryGetValue(id,out def)||def.life<=0)return null;
            var instance=new RegionSpellInstance{id=id,spell=def,owner=e,facing=e.facing,baseX=x,baseY=y,x=x,y=y};
            instance.vx=def.vx+(def.vxSpread>0?SkillRandom()*def.vxSpread:0);
            instance.vy=def.vy+(def.vySpread>0?SkillRandom()*def.vySpread:0);
            regionSpells.Add(instance);
            return instance;
        }
        void EmitRegionChildren(RegionSpellInstance parent,string trigger,float from,float to)
        {
            if(parent.spell.children==null)return;
            foreach(var rule in parent.spell.children) {
                if(rule.trigger!=trigger||trigger=="CREATE"&&(rule.at<=from||rule.at>to))continue;
                var next=AddRegionSpell(parent.owner,rule.id,parent.x+parent.facing*rule.x,parent.y+rule.y);
                if(next==null)continue;
                next.facing=parent.facing;
                next.angle=rule.angle+(rule.inherit?parent.angle:0);
            }
        }
        void TickRegionSpells(float dt)
        {
            // The 2D fighter motor supplies acceleration. Ran's callback supplies
            // the area's origin, range, duration and vertical extent for each phase.
            if(regionGravityUntil>elapsed&&Math.Abs(player.x-regionGravityX)<=regionGravityRadius&&player.y<regionGravityTop){
                player.groundLine=-1;player.state=CombatState.Airborne;
                player.vy=Math.Min((float)Math.Sqrt(2*1120*(regionGravityTop-player.y)),Math.Max(0,player.vy)+2240*dt);
            }
            for(int i=regionSpells.Count-1;i>=0;i--) {
                var s=regionSpells[i];var d=s.spell;
                if(s.stopped||s.owner.encounterIndex!=EncounterIndex){regionSpells.RemoveAt(i);continue;}
                float step=dt;
                if(s.wait>0){s.wait-=dt;if(s.wait>0)continue;step=-s.wait;s.wait=0;}
                if(s.attach!=null&&(!s.launched||s.attach.trace)){
                    var point=RegionBonePoint(s.owner,s.attach.bone);
                    s.baseX=point.x+s.facing*s.attach.x;s.baseY=point.y+s.attach.y;
                }
                float prior=s.launched?s.age:-.0001f;s.launched=true;s.age+=step;
                float dx=s.vx*s.age+.5f*d.gx*s.age*s.age,dy=s.vy*s.age+.5f*d.gy*s.age*s.age;
                float rot=s.angle*(float)Math.PI/180,co=(float)Math.Cos(rot),si=(float)Math.Sin(rot);
                s.x=s.baseX+s.facing*(dx*co-dy*si);
                s.y=s.baseY+dx*si+dy*co;
                if(d.dirSpeed!=0&&s.age>d.dirAt){
                    float moving=s.age-Math.Max(prior,d.dirAt);
                    if(s.tracking!=null||s.trackingPoint){
                        float tx=s.tracking==null?s.targetX:s.tracking.x,ty=s.tracking==null?s.targetY:s.tracking.y+23;
                        float target=(float)Math.Atan2(ty-s.y-s.travelY,(tx-s.x-s.travelX)*s.facing);
                        float delta=(float)Math.Atan2(Math.Sin(target-s.heading),Math.Cos(target-s.heading));
                        s.heading+=Clamp(delta,-d.curveSpeed*moving,d.curveSpeed*moving);
                    }
                    s.travelX+=s.facing*(float)Math.Cos(s.heading)*d.dirSpeed*moving;
                    s.travelY+=(float)Math.Sin(s.heading)*d.dirSpeed*moving;
                    s.x+=s.travelX;s.y+=s.travelY;
                }
                EmitRegionChildren(s,"CREATE",prior,Math.Min(s.age,d.life));
                if(s.age>d.life){EmitRegionChildren(s,"DIE",0,0);regionSpells.RemoveAt(i);continue;}
                if(d.ownTeam&&HandleRegionSupport(s))continue;
                if(d.count<=0||d.power<=0||s.age<s.nextHit||s.hits>0&&!d.repeat||s.hits>=d.count||
                        d.frames==null||d.frames.Length==0)continue;
                var victim=RegionTarget(s.owner);
                if(victim==null)continue;
                var box=victim==player?PeitaPlayerBox():EnemyBounds((Enemy)victim);
                float phase=d.loop&&d.span>0?s.age%d.span:s.age;
                var frame=d.frames[0];
                foreach(var f in d.frames) {if(Math.Abs(f.time-phase)>Math.Abs(frame.time-phase))break;frame=f;}
                if(frame.spheres==null||!PeitaSphereHit(frame.spheres,s.x,s.y,s.facing,0,box))continue;
                int baseAttack=s.owner.regionActor.spec.boss?48:21;
                HitReaction react=d.reaction=="RT_DOWN"||d.reaction=="RT_FLY"?HitReaction.Knockdown:
                    d.launch>0?HitReaction.Launch:HitReaction.Hurt;
                int dmg=Math.Max(1,(int)Math.Round(d.power*baseAttack));
                int dealt=d.noDamage?(victim.invincible<=0?1:0):victim==player?ApplyEnemyAttackToPlayer(dmg,s.x,react,d.launch):
                    ApplyHit(victim,dmg,react,s.x,d.launch,false);
                if(dealt<=0)continue;
                RegionSpellContactArt?.Invoke(s.id,victim.x,victim.y+22,s.facing,(victim as Enemy)?.nativeDepth ?? 0);
                s.hits++;s.nextHit=s.age+d.gap;
                EmitRegionChildren(s,"HIT",0,0);
                if(s.owner.regionActor!=null)s.owner.regionActor.hit=true;
                ApplyRegionStatus(d,baseAttack,victim);
                if(s.hits>=d.count){EmitRegionChildren(s,"DIE",0,0);regionSpells.RemoveAt(i);}
            }
            expiredStatuses.Clear();
            foreach(var pair in regionStatuses) {
                var s=pair.Value;
                if(elapsed>=s.nextTick&&s.nextTick<=s.until){
                    if(s.hp!=0)ApplyCombatHealth(player,Clamp(player.hp+s.hp,0,player.maxHp));
                    if(s.mp!=0)mana=Clamp(mana+s.mp,0,MaxMana);
                    s.nextTick+=1;
                }
                if(elapsed>=s.until){expiredStatuses.Add(pair.Key);continue;}
                if(s.armor)player.superArmor=Math.Max(player.superArmor,dt*2);
            }
            foreach(var key in expiredStatuses)regionStatuses.Remove(key);
        }
        void ApplyRegionStatus(RegionSpellDefinition d,int baseAttack,Fighter victim)
        {
            if(d.status==null||d.statusTime<=0)return;
            var state=new RegionAppliedStatus{until=elapsed+d.statusTime,nextTick=elapsed+1,effect=d.id,hitsLeft=d.statusHitLimit};
            foreach(var s in d.status) {
                switch(s.id){
                    case "BBT_CHANGE_NOW_HP_PER_SECOND":state.hp=s.value*baseAttack;break;
                    case "BBT_CHANGE_NOW_MP_PER_SECOND":state.mp=s.value;break;
                    case "BBT_CHANGE_ANIM_SPEED":state.animation=s.value;break;
                    case "BBT_FROZEN":state.frozen=true;break;
                    case "BBT_SUPER_ARMOR":state.armor=true;break;
                    case "BBT_NEVER_MOVE":state.immobile=true;break;
                    case "BBT_STUN":state.stunned=true;break;
                    case "BBT_CHANGE_UNIT_SCALE":state.pressed=true;break;
                    case "BBT_ATTACK_IMPOSSIBLE":state.noAttack=true;break;
                    case "BBT_REVERSE_LEFT_RIGHT":state.reverse=true;break;
                    case "BBT_DASH_IMPOSSIBLE":state.noDash=true;break;
                    case "BBT_CHANGE_WALK_RUN_SPEED":if(s.value==0)state.immobile=true;break;
                    case "BBT_CHANGE_JUMP_POWER":if(s.value==0)state.noJump=true;break;
                }
            }
            if(victim!=player){if(state.stunned&&!EnemyHasSuperArmor(victim as Enemy))regionEnemyStuns[victim]=state;return;}
            string key=String.IsNullOrEmpty(d.statusTemplate)?d.id:d.statusTemplate;
            RegionAppliedStatus previous;
            if(regionStatuses.TryGetValue(key,out previous)&&previous.until>elapsed)state.nextTick=previous.nextTick;
            regionStatuses[key]=state;
        }
    }
}
