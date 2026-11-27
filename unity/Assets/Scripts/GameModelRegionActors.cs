using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    [Serializable] public sealed class RegionMechanicsDocument { public RegionActorMechanics[] rows; }
    [Serializable] public sealed class RegionActorMechanics {
        public string id, start, wait, common, death;
        public float walk, jump, near, targetRange, mpRate, maxMp,subNear,subTargetRange;
        public float chaseSplit,nearWalkRate,farWalkRate,walkInterval,flyArea,flyAltitude;
        public bool boss, immuneStop;
        public RegionMove[] states;
        public RegionFunction[] functions;
        public RegionSpellDefinition[] spells;
        public RegionSpellSet[] spellSets;
        public RegionLandmarks[] landmarks;
        public float[] body;
        [NonSerialized] internal Dictionary<string,RegionMove> nodes;
        [NonSerialized] internal Dictionary<string,RegionFunction> code;
        [NonSerialized] internal Dictionary<string,RegionSpellDefinition> projectiles;
        [NonSerialized] internal Dictionary<string,RegionSpellSet> emitters;
    }
    [Serializable] public sealed class RegionMove {
        public string id, clip, onStart, onFrame, onEnd;
        public float duration, speed, vx, vy, passiveX, passiveY, cooldown, initialCooldown, invFrom, invTo;
        public float guard,reflectFrom,reflectTo;
        public int followHits;
        public bool guardFront,guardBack;
        public bool loop, armor, turn, fixedPos, hasVx, hasVy, dyingEnd, hidden;
        public float[] intervals;
        public RegionEdge[] edges;
        public RegionHit[] hits;
        public RegionCue[] cues;
        public RegionBone[] bones;
        public RegionSpellDefinition[] damageTables;
        [NonSerialized] internal PeitaAttack art;
        [NonSerialized] internal bool allowsDash;
    }
    [Serializable] public sealed class RegionEdge { public string type, target, fn; public RegionPredicate[] tests; }
    [Serializable] public sealed class RegionPredicate { public string key, symbol; public float value; }
    [Serializable] public sealed class RegionHit {
        public float from, to, strength, launch;
        public string reaction,table;
        public RegionHitSample[] samples;
    }
    [Serializable] public sealed class RegionHitSample { public float time; public float[] volumes; }
    [Serializable] public sealed class RegionCue { public float time; public string id; }
    [Serializable] public sealed class RegionFunction {
        public string name;
        public int frame;
        public int[] op;
        public string[] constants;
        public float[] numbers;
        public RegionTableTemplate[] tables;
    }
    [Serializable] public sealed class RegionLandmarks {public string room; public RegionLandmark[] points;}
    [Serializable] public sealed class RegionLandmark {public int index;public float x,y;}
    [Serializable] public sealed class RegionTableTemplate {public int index;public RegionTableEntry[] items;}
    [Serializable] public sealed class RegionTableEntry {public int key;public float value;public string text;}
    internal sealed partial class RegionActorState {
        internal RegionActorMechanics spec;
        internal RegionMove node;
        internal string request;
        internal float time, prev, age, mp, animSpeed=1;
        internal int nextSound, nextCue;
        internal long soundEntry;
        internal bool hit, suspended, hidden, hiddenGauge, deathEntered, selfCommand;
        internal string command;
        internal RegionSpellDefinition damageTable;
        internal Enemy commander;
        internal Fighter linked;
        internal Enemy mount;
        internal bool alternateAi, scriptedArmor, holdingPlayer, capturesInput;
        internal float defaultSpeed=1;
        internal float nextWalkDecision, flightFloor, flightTargetY, flightAimX, flightAimY, flightSupportY;
        internal bool flightAimValid;
        internal bool chooseDash;
        internal string holdBone;
        internal readonly List<int> lines=new List<int>();
        internal readonly Dictionary<int,object> vectors=new Dictionary<int,object>();
        internal int lastPlayerHit;
        internal int stick,stickDirection;
        internal float gravityOverride=Single.NaN,reloadOverride=Single.NaN;
        internal float extraDamageUntil,extraDamageContact;
        internal float physicalMultiplier=1,passiveX;
        internal readonly Dictionary<string,object> visuals=new Dictionary<string,object>();
        internal readonly Dictionary<string,float> available=new Dictionary<string,float>();
        internal readonly Dictionary<string,object> globals=new Dictionary<string,object>();
        internal readonly Dictionary<int,float> ints=new Dictionary<int,float>(), timers=new Dictionary<int,float>();
        internal readonly HashSet<int> flags=new HashSet<int>(), hitWindows=new HashSet<int>();
        internal readonly Dictionary<int,float> nextContact=new Dictionary<int,float>();

        internal bool ActionArmored {
            get {
                if(suspended||node==null)return false;
                if(node.armor||scriptedArmor)return true;
                if(spec.id!="ran")return false;
                // RAN.LUA SUPER_ARMOR_TIME0 uses source animation seconds.
                // Keep the same clock as his attack cues, including speed changes and hitstop.
                switch(node.id){
                    case "RAN_ATTACK_A": case "RAN_MAGIC_ATTACK_A":
                    case "RAN_MAGIC_ATTACK_B": case "RAN_MAGIC_ATTACK_D":
                        return MotionTime>=0&&MotionTime<=1.7f;
                    case "RAN_SPECIAL_ATTACK_A": case "RAN_SPECIAL_ATTACK_A_PHASE2START":
                    case "RAN_SPECIAL_ATTACK_A_PHASE3START": case "RAN_SPECIAL_ATTACK_B":
                    case "RAN_SPECIAL_ATTACK_C": case "RAN_SPECIAL_ATTACK_D":
                    case "RAN_SPECIAL_ATTACK_D_COMBO": case "RAN_JUMP_ATTACK_GRAVITY":
                        return MotionTime>=0&&MotionTime<=100;
                    default: return false;
                }
            }
        }
    }

    public sealed partial class GameModel
    {
        readonly Dictionary<string,RegionActorMechanics> regionActors=new Dictionary<string,RegionActorMechanics>();
        Fighter RegionTarget(Enemy e)
        {
            if(!e.regionalAlly)return player;
            Enemy nearest=null;float best=Single.PositiveInfinity;
            foreach(var other in enemies)if(other!=e&&!other.regionalAlly&&other.hp>0&&other.active&&!other.dormant&&!other.passive&&!other.isGate){
                float distance=Math.Abs(other.x-e.x);if(distance<best){best=distance;nearest=other;}
            }
            return nearest;
        }
        float regionInputUntil;
        public void RegionSourceVisibility(Enemy e,ref bool body,ref bool gauge) {
            if(e.regionActor==null||e.regionActor.node==null)return;
            body&=!e.regionActor.hidden&&!e.regionActor.node.hidden;
            gauge&=body&&!e.regionActor.hiddenGauge;
        }
        public void SetRegionNpcMechanics(RegionActorMechanics[] rows)
        {
            regionActors.Clear();
            regionSummons.Clear();
            ClearRegionSpells();
            regionInputUntil=0;
            if(rows==null)return;
            foreach(var spec in rows) {
                if(spec==null||spec.states==null||String.IsNullOrEmpty(spec.id))continue;
                spec.nodes=new Dictionary<string,RegionMove>();
                spec.code=new Dictionary<string,RegionFunction>();
                spec.projectiles=new Dictionary<string,RegionSpellDefinition>();
                spec.emitters=new Dictionary<string,RegionSpellSet>(StringComparer.OrdinalIgnoreCase);
                if(spec.spells!=null)foreach(var spell in spec.spells)spec.projectiles[spell.id]=spell;
                if(spec.spellSets!=null)foreach(var set in spec.spellSets)spec.emitters[set.id]=set;
                foreach(var node in spec.states){spec.nodes[node.id]=node;node.allowsDash=node.edges!=null&&Array.Exists(node.edges,x=>x.type=="SCT_AI_DASH");}
                if(spec.functions!=null)foreach(var fn in spec.functions)spec.code[fn.name]=fn;
                PeitaBehaviorRow art;
                if(peitaBehaviors.TryGetValue(spec.id,out art)&&art.actions!=null)
                    foreach(var move in art.actions) {
                        RegionMove node;
                        if(!String.IsNullOrEmpty(move.sourceState)&&spec.nodes.TryGetValue(move.sourceState,out node))node.art=move;
                    }
                regionActors[spec.id]=spec;
            }
        }
        void EnterRegionNode(Enemy e,RegionActorState actor,RegionMove node)
        {
            if(actor.node!=null)RunRegionFunction(e,actor,actor.node.onEnd);
            // Ran can land or be interrupted during the gravity attack before its
            // descent callback restores acceleration. Ordinary jumps need gravity again.
            if(e.kind=="ran" && actor.node!=null && actor.node.id.EndsWith("_GRAVITY",StringComparison.Ordinal) &&
               !node.id.EndsWith("_GRAVITY",StringComparison.Ordinal))actor.gravityOverride=Single.NaN;
            actor.node=node;
            actor.time=actor.prev=0;
            actor.extraMotion=actor.previousExtraMotion=0;
            actor.animSpeed=node.speed*actor.defaultSpeed;
            actor.passiveX=node.passiveX;
            actor.request=null;
            actor.hit=false;
            actor.stick=node.followHits;actor.stickDirection=0;
            actor.hitWindows.Clear();
            actor.nextContact.Clear();actor.damageTable=null;
            actor.nextSound=actor.nextCue=0;
            actor.soundEntry=++npcSoundEntrySerial;
            if(node.cooldown>0)actor.available[node.id]=actor.age+node.cooldown;
            e.clip=node.art==null?"idle":node.art.clip;
            e.clock=0;
            e.npcState=node.id;
            e.npcTime=0;
            var aim=RegionTarget(e);
            bool flightAim=e.kind=="wyvern" && actor.flightAimValid;
            float aimX=flightAim?actor.flightAimX:aim==null?e.x:aim.x;
            if(node.turn&&aim!=null)e.facing=(actor.formationActive?actor.formationX:aimX)>=e.x?1:-1;
            if(node.hasVx)e.vx=node.vx*e.facing;
            if(node.hasVy){e.vy=node.vy;if(e.vy>0)e.groundLine=-1;}
            if(node.id=="WYVERN_FLY_ATTACK_A"&&aim!=null){
                // The source impulse covers a full approach. When the player
                // corners the wyvern, aim the shorter dive before releasing it.
                float drop=Math.Max(0,e.y-(flightAim?actor.flightAimY:aim.y)-23),gravity=RegionalAcceleration(e,1120,true);
                float time=((float)Math.Sqrt(e.vy*e.vy+2*gravity*drop)+e.vy)/gravity;
                if(time>0){
                    float dx=Math.Abs(aimX-e.x);
                    float speed=dx/time+RegionalAcceleration(e,560,false)*time*.5f;
                    e.vx=e.facing*Math.Min(Math.Abs(e.vx),speed);
                }
            }
            RunRegionFunction(e,actor,node.onStart);
        }
        bool WyvernOwnsFlight(Enemy e) {
            if(e.kind!="wyvern" || !e.aiEnabled || e.passive || !regionActors.ContainsKey(e.kind) ||
               e.stun>0 || e.knockdownOnLanding || e.state==CombatState.Knockdown ||
               e.state==CombatState.GettingUp || RegionEnemyStunned(e))return false;
            string node=e.regionActor?.node?.id;
            // The source dive carries a downward impulse and ends on actual foot contact.
            return node!="WYVERN_FLY_ATTACK_A" && node!="WYVERN_DAMAGE_AIR_FALL" && node!="WYVERN_DAMAGE_AIR_FALL_LANDING";
        }

        float FlightFloor(float x,float previousFloor,float step) {
            if(!NativeTerrain)return GroundAt(CurrentRoom,x);
            // Track the launch surface over ramps, without mistaking an overhead platform
            // crossed during ascent for a second launch surface.
            var floor=SurfaceBelow(x,previousFloor+Math.Abs(step)*2+2);
            return floor==null || IsFallSurface(floor) ? previousFloor : floor.Y(x);
        }

        void MoveWyvernInFlight(Enemy e,RegionActorState actor,float dt) {
            var node=actor.node;
            float step=node.fixedPos?0:actor.passiveX*e.facing*dt;
            e.x=ConstrainEncounterX(e,Clamp(e.x+step,12,RoomWidth-12));
            actor.flightFloor=FlightFloor(e.x,actor.flightFloor,step);
            var target=RegionTarget(e);
            actor.flightTargetY=actor.flightFloor+actor.spec.flyAltitude;
            if(target!=null && target.hp>0 && Math.Abs(target.x-e.x)<=actor.spec.targetRange)
            {
                // Keep hunting the last viable approach when the player is
                // falling into a recovery pit. Chasing the fall, backing away,
                // then following the respawn cancels the dive on every cycle.
                var support=NativeTerrain?SurfaceBelow(target.x,target.y+.05f):null;
                if(!actor.flightAimValid || !NativeTerrain || support!=null&&!IsFallSurface(support)){
                    actor.flightAimX=target.x;
                    actor.flightAimY=target.y;
                    actor.flightSupportY=support==null?target.y:support.Y(target.x);
                    actor.flightAimValid=true;
                }
                else actor.flightAimY=actor.flightSupportY;
                float aimX=actor.flightAimX,aimY=actor.flightAimY;
                actor.flightTargetY=aimY+actor.spec.flyAltitude;
                // Set up the dive on the target's side of an overhead platform.
                // Otherwise the dive lands on that upper floor before it ever
                // reaches the player underneath it.
                if(NativeTerrain)foreach(var s in CurrentRoom.surfaces) {
                    if(s.hamelDisabled || s.npcOnly || IsFallSurface(s))continue;
                    float ceiling;
                    if(s.platform){
                        // A balcony can cover the start of the dive but end
                        // before the player's feet, as in the First Market.
                        float left=Math.Max(s.x1,Math.Min(e.x,aimX)),right=Math.Min(s.x2,Math.Max(e.x,aimX));
                        if(left>right)continue;
                        ceiling=Math.Min(s.Y(left),s.Y(right));
                    }else{
                        if(aimX<s.x1 || aimX>s.x2)continue;
                        ceiling=s.Y(aimX);
                    }
                    if(ceiling>aimY+1 && ceiling<=actor.flightTargetY)
                        actor.flightTargetY=ceiling-2;
                }
            }
            else actor.flightAimValid=false;
            float dy=actor.flightTargetY-e.y;
            e.y+=Clamp(dy,-actor.spec.walk*dt,actor.spec.walk*dt);
            e.vx=e.vy=0;
            e.groundLine=-1;
        }

        bool TickRegionNpc(Enemy e,float dt)
        {
            RegionActorMechanics spec;
            if(!regionActors.TryGetValue(e.kind,out spec))return false;
            if(e.state==CombatState.Knockdown||e.state==CombatState.GettingUp||e.stun>0||RegionEnemyStunned(e)||e.kind=="wyvern"&&e.knockdownOnLanding){
                if(e.regionActor!=null)e.regionActor.suspended=true;
                return true;
            }
            var actor=e.regionActor;
            RegionMove node;
            if(actor==null||actor.spec!=spec) {
                e.regionActor=actor=new RegionActorState{spec=spec,flightFloor=e.y};
                if(IsHamelSurvivalSpawner(e))PrepareHamelSurvivalSpawner(e);
                foreach(var move in spec.states)if(move.initialCooldown>0)actor.available[move.id]=move.initialCooldown;
                if(!spec.nodes.TryGetValue(spec.start??"",out node)&&!spec.nodes.TryGetValue(spec.wait??"",out node))return true;
                EnterRegionNode(e,actor,node);
            }else if(actor.suspended) {
                actor.suspended=false;
                if(spec.nodes.TryGetValue(spec.wait??"",out node))EnterRegionNode(e,actor,node);
            }
            node=actor.node;
            if(node==null)return true;
            if(actor.formationActive&&node.id==spec.wait&&Math.Abs(e.x-actor.formationX)<.05f&&Math.Abs(e.y-actor.formationY)<.1f){
                actor.formationActive=false;actor.selfCommand=true;
            }
            actor.age+=dt;
            if(actor.extraDamageUntil>0&&actor.age>=actor.extraDamageUntil){actor.extraDamageUntil=0;actor.extraDamageContact=0;}
            actor.mp=Math.Min(spec.maxMp,actor.mp+spec.mpRate*dt);
            actor.prev=actor.time;
            actor.time+=dt;
            actor.previousExtraMotion=actor.extraMotion;
            if(actor.supportSpeedUntil>elapsed)actor.extraMotion+=dt*actor.animSpeed*(actor.supportSpeed-1);
            float now=actor.MotionTime;
            e.clock=node.loop&&node.duration>0?now%node.duration:now;
            e.clip=node.art==null?(actor.deathEntered?"death":"idle"):node.art.clip;
            e.npcTime=actor.time;
            if(IsResourceEater(e)&&e.peitaAttack!=null&&e.peitaAttack.fleeUntil>0)
                e.facing=e.x>=player.x?1:-1;
            if(node.invTo>0&&now>=node.invFrom&&now<node.invTo)e.invincible=Math.Max(e.invincible,dt*2);
            if(WyvernOwnsFlight(e))MoveWyvernInFlight(e,actor,dt);
            else if(!node.fixedPos) {
                if(actor.formationActive&&(node.id.EndsWith("_WALK",StringComparison.Ordinal)||node.id.EndsWith("_DASH",StringComparison.Ordinal))){
                    float delta=actor.formationX-e.x;
                    if(Math.Abs(delta)>.025f)e.facing=delta>0?1:-1;
                    float speed=actor.passiveX!=0?Math.Abs(actor.passiveX):actor.spec.walk;
                    MoveAlongGround(e,e.x+Math.Sign(delta)*Math.Min(Math.Abs(delta),speed*dt));
                }else if(actor.passiveX!=0)MoveAlongGround(e,Clamp(e.x+e.facing*actor.passiveX*dt,12,RoomWidth-12));
                if(node.passiveY!=0)e.y+=node.passiveY*dt;
            }
            if(node.art!=null&&node.art.sounds!=null)while(actor.nextSound<node.art.sounds.Length&&node.art.sounds[actor.nextSound].candidateTime<=actor.time){
                var cue=node.art.sounds[actor.nextSound++];
                QueueNpcSound(cue,e,actor.soundEntry,0,elapsed-actor.time+cue.candidateTime,node.clip);
            }
            if(node.cues!=null)while(actor.nextCue<node.cues.Length&&node.cues[actor.nextCue].time<=now){
                var cue=node.cues[actor.nextCue++];
                ScheduleRegionSet(e,cue.id);
                RegionEffectCue(e,cue.id);
            }
            if(node.dyingEnd&&now>=node.duration){e.hp=0;return true;}
            RunRegionFunction(e,actor,spec.common);
            RunRegionFunction(e,actor,node.onFrame);
            RegionContact(e,actor);
            TrackRegionalBindings(e,actor,dt);
            TickRegionChargerGrip(e,actor,dt);
            TickVelderRider(e,actor,dt);
            if(TickRegionEater(e,actor))return true;
            RegionMove next;
            if(actor.request!=null&&spec.nodes.TryGetValue(actor.request,out next)){EnterRegionNode(e,actor,next);return true;}
            if(node.edges==null)return true;
            foreach(var edge in node.edges) {
                float available;
                if(!spec.nodes.TryGetValue(edge.target,out next)||actor.available.TryGetValue(edge.target,out available)&&available>actor.age)continue;
                if(!RegionEdgeMatches(e,actor,edge))continue;
                EnterRegionNode(e,actor,next);
                break;
            }
            return true;
        }
        int GuardRegionActor(Enemy e,int damage,float fromX,ref HitReaction reaction, ref bool shieldContact)
        {
            var a=e.regionActor;
            if(a!=null&&a.defenceUntil>elapsed&&damage>0)damage=Math.Max(1,(int)Math.Ceiling(damage*a.defenceFactor));
            if(a==null||a.node==null||a.node.guard<=0||e.state!=CombatState.Normal)return damage;
            bool front=(fromX-e.x)*e.facing>=0;
            if(front?!a.node.guardFront:!a.node.guardBack)return damage;
            reaction=HitReaction.None;
            shieldContact=true;
            SpawnEffect("guard",e.x,e.y+24,e.facing);
            return Math.Max(1,(int)Math.Ceiling(damage*(1-a.node.guard/100.0)));
        }
        bool RegionEdgeMatches(Enemy e,RegionActorState a,RegionEdge edge)
        {
            var node=a.node;
            var target=RegionTarget(e);
            float targetX=e.kind=="wyvern"&&a.flightAimValid?a.flightAimX:target==null?e.x:target.x;
            float dx=target==null?Single.PositiveInfinity:Math.Abs(targetX-e.x),dy=target==null?0:target.y-e.y;
            float near=a.alternateAi?a.spec.subNear:a.spec.near,range=a.alternateAi?a.spec.subTargetRange:a.spec.targetRange;
            bool focus=(!e.passive||e.regionalAlly)&&e.aiEnabled&&target!=null&&target.hp>0&&dx<=range;
            bool flyBack=focus&&a.spec.flyArea>0&&dx<a.spec.flyArea;
            if(e.kind=="wyvern"&&flyBack){
                // Only back away if that side has enough room to reach the dive's
                // approach distance. At a closed gate the original attack timer
                // must keep running while the player presses in underneath.
                int away=e.x>targetX?1:e.x<targetX?-1:-e.facing;
                float retreat=ConstrainEncounterX(e,Clamp(targetX+away*a.spec.flyArea,12,RoomWidth-12));
                flyBack=(retreat-targetX)*away>=a.spec.flyArea-.05f;
            }
            float flyDelta=e.kind=="wyvern" ? a.flightTargetY-e.y : dy+a.spec.flyAltitude;
            if(e.kind=="wyvern")dy=flyDelta;
            if(a.formationActive&&(edge.type=="SCT_AI_WAIT"||edge.type=="SCT_AI_WALK"||edge.type=="SCT_AI_DASH"||edge.type=="SCT_AI_JUMP"||edge.type=="SCT_AI_JUMP_DIR"||edge.type=="SCT_AI_DOWN"||edge.type=="SCT_AI_DOWN_DIR")){
                dx=Math.Abs(a.formationX-e.x);dy=a.formationY-e.y;near=.025f;focus=e.aiEnabled&&!e.passive;
            }
            switch(edge.type) {
                case "SCT_MOTION_END":return a.MotionTime>=node.duration&&(!node.loop||
                    (int)(a.PreviousMotionTime/node.duration)!=(int)(a.MotionTime/node.duration));
                case "SCT_FOOT_ON_LINE_TRUE":return Grounded(e);
                case "SCT_FOOT_ON_LINE_FALSE_DOWN":return !Grounded(e)&&e.vy<=0;
                case "SCT_NEGATIVE_Y_SPEED":return e.vy<0;
                case "SCT_POSITIVE_Y_SPEED":return e.vy>0;
                case "SCT_AI_WAIT":return !focus||dx<=near&&(e.kind!="wyvern"||!flyBack);
                case "SCT_AI_SELF_COMMAND_TRANSIT":if(!a.selfCommand)return false;a.selfCommand=false;return true;
                case "SCT_AI_WALK":case "SCT_AI_DASH":
                    if(!focus||dx<=near)return false;
                    if(!node.allowsDash)return edge.type=="SCT_AI_WALK";
                    if(a.age>=a.nextWalkDecision){
                        float walkRate=dx>a.spec.chaseSplit?a.spec.farWalkRate:a.spec.nearWalkRate;
                        a.chooseDash=a.command=="GAI_CMD_FORCED_MOVE"||walkRate<100&&SkillRandom()*100>=walkRate;
                        a.nextWalkDecision=a.age+Math.Max(.1f,a.spec.walkInterval);
                    }
                    return (edge.type=="SCT_AI_DASH")==a.chooseDash;
                case "SCT_AI_JUMP":case "SCT_AI_JUMP_DIR":return focus&&Grounded(e)&&dy>44&&dx<120;
                case "SCT_AI_DOWN":case "SCT_AI_DOWN_DIR":return focus&&Grounded(e)&&dy< -44&&dx<120;
                case "SCT_AI_FLY_FRONT":return focus&&dx>a.spec.near&&(e.kind!="wyvern"||Math.Abs(dy)<=32);
                case "SCT_AI_FLY_UP":return focus&&dy>32&&(e.kind!="wyvern"||!flyBack&&dx<=near);
                case "SCT_AI_FLY_DOWN":return focus&&dy< -32&&(e.kind!="wyvern"||!flyBack&&dx<=near);
                case "SCT_AI_FLY_FRONT_UP":return focus&&dx>a.spec.near&&dy>32;
                case "SCT_AI_FLY_FRONT_DOWN":return focus&&dx>a.spec.near&&dy< -32;
                case "SCT_AI_FLY_BACK":return flyBack&&Math.Abs(flyDelta)<=32;
                case "SCT_AI_FLY_BACK_UP":return flyBack&&flyDelta>32;
                case "SCT_AI_FLY_BACK_DOWN":return flyBack&&flyDelta< -32;
                case "SCT_CONDITION_FUNCTION":return Equals(RunRegionFunction(e,a,edge.fn),true);
                case "SCT_CONDITION_TABLE":break;
                default:return false;
            }
            // Fixed room hazards retain their authored proximity sensor while combat AI is disabled.
            bool sensesTarget=focus||e.passive&&target!=null&&target.hp>0&&dx<=range&&
                (e.kind=="black_flower_trap"||e.kind=="black_flower_front"||e.kind=="chilling_spine_trap"||e.kind=="ice_rock");
            float chance=100;
            foreach(var test in edge.tests) {
                float v=test.value;
                switch(test.key) {
                    case "RATE":chance=v;break;
                    case "DISTANCE_TO_TARGET_NEAR":if(!sensesTarget||dx>v*.28f)return false;break;
                    case "DISTANCE_OVER_TARGET_NEAR":if(!focus||dx<=v*.28f)return false;break;
                    case "MY_HP_LESS_THAN_PERCENT":if(e.hp*100>=e.maxHp*v)return false;break;
                    case "MY_MP_MORE_THAN":if(a.mp<=v)return false;break;
                    case "MY_MP_MORE_THAN_PERCENT":if(a.mp*100<=a.spec.maxMp*v)return false;break;
                    case "STATE_TIME_OVER":if(a.time<=v)return false;break;
                    case "TIMER_ELAPSED0":if(a.age-RegionValue(a.timers,0)<v)return false;break;
                    case "ANIM_EVENT_TIMER":if(!(a.PreviousMotionTime<v&&a.MotionTime>=v))return false;break;
                    case "ANIM_PLAY_COUNT":if(a.MotionTime<node.duration*v)return false;break;
                    case "EVENT_INTERVAL_ID":case "EVENT_INTERBAL_ID":
                        int ix=(int)v;
                        if(ix<0||node.intervals==null||ix>=node.intervals.Length)return false;
                        float window=node.intervals[ix];
                        // The black flower replays a 1.33 s wait pose while retaining its 4 s sensor timer.
                        float time=(e.kind=="black_flower_trap"||e.kind=="black_flower_front")&&node.id=="BLACK_FLOWER_WAIT"?a.age:a.time;
                        if(window>0&&(int)((time-(a.time-a.prev))/window)==(int)(time/window))return false;
                        break;
                    case "FLAG_FALSE":if(a.flags.Contains((int)v))return false;break;
                    case "FLAG_TRUE":if(!a.flags.Contains((int)v))return false;break;
                    case "ATTACK_SUCCESS":if(a.hit!=(v!=0))return false;break;
                    case "GE_KEYCODE":if(e.sourceKeyCode<(int)v)return false;break;
                    case "SAME_LINE_WITH_TARGET":if(!sensesTarget||(e.groundLine==target.groundLine)!=(v!=0))return false;break;
                    case "HAVE_TARGET":if(focus!=(v!=0))return false;break;
                    case "TARGET_ABOVE_ME":if(!focus||(dy>0)!=(v!=0))return false;break;
                    case "TARGET_BELOW_ME":if(!focus||(dy<0)!=(v!=0))return false;break;
                    case "SMALL_THEN_Y_SPEED":if(e.vy>=v*.28f)return false;break;
                    default:
                        if(test.key.StartsWith("COMMAND_FALSE",StringComparison.Ordinal)){if(a.command==test.symbol)return false;}
                        else if(test.key.StartsWith("COMMAND_TRUE",StringComparison.Ordinal)){if(a.command!=test.symbol)return false;}
                        else return false;
                        break;
                }
            }
            return chance>=100||SkillRandom()*100<chance;
        }
        void RegionContact(Enemy e,RegionActorState a)
        {
            var hits=a.node.hits;
            if(hits==null)return;
            var target=RegionTarget(e);
            if(target==null)return;
            float now=a.MotionTime;
            for(int i=0;i<hits.Length;i++) {
                var h=hits[i];
                float due;
                if(a.hitWindows.Contains(i)||a.nextContact.TryGetValue(i,out due)&&due>now||a.PreviousMotionTime>h.to||now<h.from||h.samples==null||h.samples.Length==0)continue;
                var table=a.damageTable;
                if(table==null&&a.node.damageTables!=null)foreach(var candidate in a.node.damageTables)if(candidate.id==(h.table??"DAMAGE_DATA")){table=candidate;break;}
                int ix=0;
                while(ix+1<h.samples.Length&&Math.Abs(h.samples[ix+1].time-now)<Math.Abs(h.samples[ix].time-now))ix++;
                var v=h.samples[ix].volumes;
                for(int j=0;j+2<v.Length;j+=3) {
                    float dx=Math.Max(0,Math.Abs(e.x+e.facing*v[j]-target.x)-10);
                    float dy=Math.Max(0,Math.Abs(e.y+v[j+1]-target.y-23)-23);
                    if(dx*dx+dy*dy>v[j+2]*v[j+2])continue;
                    string type=table!=null?table.reaction:h.reaction;
                    float launch=table!=null?table.launch:h.launch;
                    HitReaction reaction=type=="RT_DOWN"||type=="RT_FLY"?HitReaction.Knockdown:
                        launch>0?HitReaction.Launch:HitReaction.Hurt;
                    int power=(int)Math.Round((a.spec.boss?48:21)*a.physicalMultiplier);
                    int damage=(int)Math.Round((table!=null?table.power:h.strength)*power);
                    int dealt=table!=null&&table.noDamage?(target.invincible<=0?1:0):damage<=0?0:target==player?ApplyEnemyAttackToPlayer(damage,e.x,reaction,launch):ApplyHit(target,damage,reaction,e.x,launch,false);
                    if(dealt>0){
                        a.hit=true;
                        if(a.extraDamageUntil>a.age&&(table==null||!table.noDamage))a.extraDamageContact=dealt;
                        if(target==player&&a.holdBone!=null)a.holdingPlayer=true;
                        if(table!=null){ApplyRegionStatus(table,power,target);if(table.repeat)a.nextContact[i]=now+table.gap;}
                        if(table==null||!table.repeat)a.hitWindows.Add(i);
                    }
                    break;
                }
            }
        }
        void TickRegionDeath(Enemy e,float dt)
        {
            var a=e.regionActor;
            if(a==null||String.IsNullOrEmpty(a.spec.death))return;
            RegionMove node;
            string death=e.kind=="cannon_demon"&&a.cannonBack?"CANNON_DEMON_DYING_BACK_SIDE":a.spec.death;
            if(!a.spec.nodes.TryGetValue(death,out node))return;
            if(!a.deathEntered){
                a.deathEntered=true;
                if(a.node!=node)EnterRegionNode(e,a,node);
            }
            a.prev=a.time;a.time+=dt;
            e.clip=node.art==null?"death":node.art.clip;e.clock=a.MotionTime;
            if(node.cues!=null)while(a.nextCue<node.cues.Length&&node.cues[a.nextCue].time<=e.clock) {
                var cue=node.cues[a.nextCue++];
                ScheduleRegionSet(e,cue.id);RegionEffectCue(e,cue.id);
            }
            RunRegionFunction(e,a,node.onFrame);
        }
        public Func<Enemy,string,bool> RegionalVisualCue;
        void RegionEffectCue(Enemy e,string id)
        {
            if(String.IsNullOrEmpty(id)||RegionalVisualCue!=null&&RegionalVisualCue(e,id))return;
            string name=id.ToLowerInvariant();
            string style=name.Contains("shock")||name.Contains("explosion")||name.Contains("crash")?"slam":
                name.Contains("shield")||name.Contains("guard")||name.Contains("barrier")?"guard":
                name.Contains("slash")||name.Contains("hit")?"enemy-hit":null;
            if(style!=null)SpawnEffect(style,e.x,e.y+20,e.facing);
        }
    }
}
