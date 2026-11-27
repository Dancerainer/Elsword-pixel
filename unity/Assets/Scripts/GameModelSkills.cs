using System;
using System.Collections.Generic;
namespace ElswordPixel
{
    [Serializable] public sealed class SkillDefinition
    {
        public string id,name,description,clip,category,job,itemName;
        public int sourceSkillId,mpCost,damage,costItem;
        public float cooldown,range,launchVelocity=290;
        public HitReaction reaction;
        public bool allAround,airborne,appliesBuff,awakeningOnly;
    }
    public sealed class SkillDebuff
    {
        public float armorBreak,burn,stun,burnTick,knockdownGauge,accuracy,burnPower=.3f;
        public int stunHits;
        public string Label { get {return armorBreak>0?"ARMOR -40%":stun>0?"STUN":accuracy>0?"ACCURACY -30%":burn>0?"BURN":"";} }
    }
    public sealed partial class GameModel
    {
        public const float ArmorDuration=5,ArmorDamageMultiplier=.614f,SkillAttackPower=20;
        private const float SkillUnit=.28f;
        public const float ArmageddonAttackStart=20.6701254f,ArmageddonAttackEnd=148.073191f;
        public float ArmageddonRemaining {get;private set;}
        public float BaseCriticalChance=DefaultCriticalChance;
        public float CriticalChance {get{return Clamp(BaseCriticalChance*(ArmageddonRemaining>0?1.2f:1),0,1);}}
        public bool ArmageddonActive {get{return ArmageddonRemaining>0||(actionSkill!=null&&actionSkill.id=="armageddon-blade"&&skillPhaseTime>=1.1f/1.3f);}}
        public float SkillWorldStopRemaining {get;private set;}
        public int SkillCastSerial {get;private set;}
        public string LastCastSkillId {get;private set;}
        public int HyperEssence=3;
        public float CastScreenDim {get{return actionSkill!=null&&SkillGeometry2016.State(player.clip).dim?.65f:0;}}
        public bool SkillCastInvulnerable {get{return actionSkill!=null&&SkillGeometry2016.State(player.clip).Invulnerable(skillPhaseTime);}}
        private bool skillAwakened, skillForwardBlocked;
        private uint skillRandom=20160420;
        private float SkillRandom(){skillRandom=1664525*skillRandom+1013904223;return(skillRandom>>8)/16777216f;}
        private bool EnemyAccuracyAllowsHit(Enemy e){var d=GetEnemyDebuff(e);return d==null||d.accuracy<=0||SkillRandom()<.7f;}
        private void RequestWorldStop(float duration){SkillWorldStopRemaining=Math.Max(SkillWorldStopRemaining,duration);foreach(var e in enemies)if(e.active&&e.hp>0&&!PeitaIgnoresSkillStop(e))e.hitPause=Math.Max(e.hitPause,duration);}
        private string skillPhase="";
        private float skillPhaseTime,skillPreviousTime,skillPhaseDuration,skillHoldTime,skillPower=1,skillMotionRoot,skillStop,skillPoseRootCarry;
        private int skillSlot=-1;
        private bool skillMagic;
        private SkillGroundPoint skillCastGround;
        private float lastGeyserX;
        private readonly HashSet<string> skillEvents=new HashSet<string>();
        private readonly HashSet<Enemy> skillMeleeHits=new HashSet<Enemy>();
        private readonly Dictionary<Enemy,float> skillMeleeNext=new Dictionary<Enemy,float>();
        private readonly Dictionary<Enemy,SkillDebuff> skillDebuffs=new Dictionary<Enemy,SkillDebuff>();
        private readonly List<SkillArea> skillAreas=new List<SkillArea>();
        private readonly Dictionary<Enemy,float> mortalGroupNext=new Dictionary<Enemy,float>();
        private readonly struct SkillGroundPoint
        {
            public readonly float X,Y;
            public readonly GroundSurface Support;
            public SkillGroundPoint(float x,float y,GroundSurface support){X=x;Y=y;Support=support;}
        }
        private sealed class SkillArea
        {
            public string mesh,skill;
            public float x,y,birthX,birthY,age,life,start,end,repeat,damage,backX,backY,stun,burn,defenderStop,mpGain;
            public float vx,vy,moveUntil,offsetX,offsetY,pullSpeed,pullRange,burnPower=.3f,accuracy,stunChance=1,childAt=.03f;
            public Effect visual;
            public SkillGroundPoint birthGround,ground;
            public float lastTrailX=float.NaN;
            public int facing,stunHits;
            public bool follow,magic,wind,mortalTrail,sand,grouped,groundRelative;
            public HitReaction reaction;
            public readonly Dictionary<Enemy,float> nextHit=new Dictionary<Enemy,float>();
        }
        public bool CounterWaiting {get{return actionSkill!=null&&actionSkill.id=="counter-attack"&&skillPhase=="stance";}}
        public bool IsSkillCharging {get{return actionSkill!=null&&actionSkill.id=="mega-slash"&&skillPhase=="charge";}}
        public float SkillChargeAmount {get{return IsSkillCharging?Clamp(skillPhaseTime/1.7f,0,1):0;}}
        public string SkillPhaseLabel {get{return actionSkill==null?"":actionSkill.name+" / "+skillPhase.ToUpperInvariant();}}
        public float SkillPhaseRemaining {get{return Math.Max(0,skillPhaseDuration-skillPhaseTime);}}
        public float SkillMoveMultiplier {get{return armor>0?1.625f:1;}}
        public string ActiveSkillId {get{return actionSkill==null?"":actionSkill.id;}}
        public int ActiveSkillAreaCount {get{return skillAreas.Count;}}
        // Side-scroller comfort adaptation: retain Windmill's original bounded
        // Bip01 sway in presentation, so it cannot move the controller/camera
        // off a platform or accumulate displacement when the player turns.
        public float SkillPresentationRootX(float clock)
        {
            if(actionSkill==null||actionSkill.id!="windmill")return 0;
            if(skillPhase=="loop")return player.facing*SkillGeometry2016.Root("windmill-loop",clock,true);
            return skillPhase=="end"?skillPoseRootCarry*(1-Clamp(clock/.12f,0,1)):0;
        }
        private bool SkillAllowsPass {get{return actionSkill!=null&&(actionSkill.id=="windmill"&&skillPhase=="loop"||actionSkill.id=="roll"||actionSkill.id=="gigantic-slash"&&skillPhaseTime>=1/1.15f&&skillPhaseTime<3/1.15f);}}
        private bool SkillSuppressGravity {get{return actionSkill!=null&&actionSkill.id=="windmill"&&skillPhase=="start";}}
        public SkillDebuff GetEnemyDebuff(Enemy enemy){SkillDebuff d;return enemy!=null&&skillDebuffs.TryGetValue(enemy,out d)?d:null;}
        private SkillDebuff Debuff(Enemy enemy){SkillDebuff d;if(!skillDebuffs.TryGetValue(enemy,out d)){d=new SkillDebuff();skillDebuffs.Add(enemy,d);}return d;}
        public float GetEnemyKnockdownGauge(Enemy enemy){var d=GetEnemyDebuff(enemy);return d==null?0:d.knockdownGauge;}
        public bool RegisterKnockdownGauge(Enemy enemy,float amount)
        {
            if(enemy==null||enemy.passive||enemy.hp<=0||Mine2RailRejectDownV21(enemy)||CoreHardShieldGuardingR1417(enemy,player.x))return false;
            var d=Debuff(enemy);d.knockdownGauge=Clamp(d.knockdownGauge+amount,0,100);
            if(d.knockdownGauge<100)return false;d.knockdownGauge=0;return true;
        }
        private void Skill(string id,int sourceId,string name,string description,int mp,float cooldown,float coefficient,HitReaction reaction,bool air=false)
        {
            var meta=SkillGeometry2016.Metadata[sourceId];
            skills.Add(new SkillDefinition{id=id,sourceSkillId=sourceId,name=name,description=description,clip=id,mpCost=mp,cooldown=cooldown,damage=(int)Math.Round(coefficient*SkillAttackPower),reaction=reaction,airborne=air,category=meta[0],job=meta[1],appliesBuff=id=="iron-body"||id=="armageddon-blade",costItem=id=="gigantic-slash"?1:0,itemName=id=="gigantic-slash"?"El Essence":""});
        }
        private void CreateSkills()
        {
            // Raw costs, cooldowns and coefficients are target2016 declarations.
            // Multiplying by SkillAttackPower is this demo's explicit stat scale.
            Skill("mega-slash",21026,"MEGA SLASH","Ready, hold >1s for 1.5x, then a forward cut.",100,6,16.02f*1.2f,HitReaction.Knockdown);
            Skill("flame-geyser",21027,"FLAME GEYSER","Ground fire column; repeated magic hits and burn.",150,10,4.88f*1.2f,HitReaction.Launch);
            Skill("iron-body",21218,"IRON BODY","Instant 5s armor; 38.6% reduction, movement boost.",50,15,0,HitReaction.None);
            Skill("armor-break",21204,"ARMOR BREAK","Foot strike; physical defense -40% for 7s.",40,7,9.7f,HitReaction.Hurt);
            Skill("sonic-blade",21011,"SONIC BLADE","Fixed expanding shockwave; one hit and stun.",100,8,15.86f,HitReaction.Hurt);
            Skill("rolling-smash",21205,"ROLLING SMASH","Armored vault; spin contacts then landing shock.",40,7,.76f,HitReaction.Hurt);
            Skill("windmill",21010,"WINDMILL","Move/jump while spinning; hold for extra MP time.",150,15,1.35f,HitReaction.Hurt,true);
            Skill("kick",21219,"KICK","Foot strike; reduce knockdown gauge by 50.",30,6,7.08f*1.2f,HitReaction.Hurt);
            Skill("counter-attack",21206,"COUNTER ATTACK","Front guard during .06-.5s; hit triggers counter.",10,0,5.25f,HitReaction.Launch);
            Skill("double-slash",21007,"DOUBLE SLASH","First stun impact; delayed multi-hit finishing arc.",300,21,6.21f,HitReaction.Launch);
            Skill("spiral-blast",21031,"SPIRAL BLAST","Stunning first slash, then a short moving blast.",180,14,6.24f,HitReaction.Launch);
            Skill("roll",21217,"ROLL","Armored forward roll; foot contact, 1% HP above 40% HP.",15,3,2.19f*2,HitReaction.Hurt);
            Skill("unlimited-blade",21028,"UNLIMITED BLADE","Six separate sword strikes and a magic flame finisher.",250,16,3.49f*1.5f,HitReaction.Hurt);
            Skill("assault-slash",21029,"ASSAULT SLASH","Root-motion thrust with repeated sword contacts.",100,6,2.92f*1.2f,HitReaction.Hurt);
            Skill("triple-geyser",21030,"TRIPLE GEYSER","Three successive fire columns at separate world positions.",300,20,3.17f*1.2f,HitReaction.Launch);
            Skill("mortal-blow",21034,"MORTAL BLOW","Moving ground blast leaves burning patches behind.",100,7,11.7f,HitReaction.Hurt);
            Skill("impact-smash",21203,"IMPACT SMASH","Two contacts then a shockwave; 50% chance to stun.",50,7,7.55f,HitReaction.Hurt);
            Skill("armageddon-blade",21008,"ARMAGEDDON BLADE","Summon an extended sword for 25s; initial heavy slash.",250,25,27.09f,HitReaction.Hurt);
            Skill("sandstorm",21009,"SANDSTORM","Moving storm pulls enemies; accuracy -30% for 5s.",200,16,1.27f,HitReaction.Hurt);
            Skill("gigantic-slash",21603,"GIGANTIC SLASH","Capture, repeated slashes and final launch; costs 1 El Essence.",300,180,44.78f*1.2f,HitReaction.Launch);
        }
        private bool CanPaySkill(int slot,SkillDefinition s)
        {return s!=null&&slot>=0&&slot<SkillSlotCount&&cooldowns[slot]<=0&&mana>=s.mpCost&&HyperEssence>=s.costItem&&!paused&&!victory&&player.hp>0;}
        private void PaySkill(int slot,SkillDefinition s)
        {mana-=s.mpCost;HyperEssence-=s.costItem;PeitaImmuneSkill(s);skillCooldowns[s.id]=s.cooldown;SyncCooldowns();lastCommand=s.name;commandDisplay=1.5f;if(s.category!="Active"){SkillCastSerial++;LastCastSkillId=s.id;}}
        private bool TryInstantSkill(int slot)
        {
            var s=GetEquippedSkill(slot);
            if(s==null||s.id!="iron-body"||!CanPaySkill(slot,s)||!Grounded(player)||player.state==CombatState.Knockdown||player.state==CombatState.GettingUp)return false;
            PaySkill(slot,s);armor=ArmorDuration;
            // The current demo has no negative player buff producer. Do not
            // incorrectly treat ordinary hit-stun/knockdown as a dispellable buff.
            SkillVisual("iron-body",0,ArmorDuration,player.x,player.y+34,.7f,.7f,true,0,34);
            return true;
        }
        private bool StartSkill(int slot)
        {
            var s=GetEquippedSkill(slot);
            if(s!=null&&s.id=="iron-body")return TryInstantSkill(slot);
            if(!CanPaySkill(slot,s)||(!s.airborne&&!Grounded(player)))return false;
            PaySkill(slot,s);CancelAction(true);actionSkill=s;skillSlot=slot;
            BeginSkillAction(s);return true;
        }
        private void BeginSkillAction(SkillDefinition s)
        {
            ClearCounterFractionalBonusR901();
            var support=NativeTerrain?SurfaceBelow(player.x,player.y+.05f):null;
            skillCastGround=new SkillGroundPoint(player.x,support==null?player.y:support.Y(player.x),support);
            lastGeyserX=float.NaN;
            skillPower=1;skillHoldTime=skillStop=0;player.vx=0;skillAwakened=IsAwakened;
            switch(s.id)
            {
                case "mega-slash": BeginSkillPhase("ready","mega-ready",.333333333f/.7f);break;
                case "rolling-smash": BeginSkillPhase("ready","rolling-ready",1.133333333f/5);break;
                case "windmill": player.vx=player.vy=0;BeginSkillPhase("start","windmill-start",.366666667f);break;
                case "counter-attack": BeginSkillPhase("stance","counter-stance",1.2f);SkillVisual(s.id,0,1.2f,player.x+player.facing*28,player.y+20,.45f,.45f,true,28,20);break;
                case "flame-geyser":BeginSkillPhase("cast",s.id,1.9f);break;
                case "sonic-blade":BeginSkillPhase("cast",s.id,1.533333333f/.85f);break;
                case "double-slash":BeginSkillPhase("cast",s.id,5);break;
                case "spiral-blast":BeginSkillPhase("cast",s.id,2.366666667f);break;
                case "armor-break":BeginSkillPhase("strike",s.id,.666666667f);break;
                case "roll":if(player.hp>=player.maxHp*.4f)player.hp=Math.Max(1,player.hp-player.maxHp*.01f);BeginSkillPhase("roll",s.id,.6f/1.3f);player.vx=player.facing*1200*SkillUnit;break;
                case "unlimited-blade":BeginSkillPhase("slashes",skillAwakened?"unlimited-blade-awakened":s.id,4.833333f);break;
                default:BeginSkillPhase("cast",s.id,SkillGeometry2016.State(s.id).duration);break;
            }
        }
        private void BeginSkillPhase(string phase,string clip,float duration)
        {
            skillPoseRootCarry=actionSkill!=null&&actionSkill.id=="windmill"&&skillPhase=="loop"&&phase=="end"?SkillPresentationRootX(skillPhaseTime):0;
            if(clip=="mega-fire")for(int i=effects.Count-1;i>=0;i--)if(effects[i].type=="mega-slash"&&effects[i].phase==1)effects.RemoveAt(i);
            skillForwardBlocked=false;skillPhase=phase;skillPhaseTime=skillPreviousTime=0;skillPhaseDuration=duration;skillMotionRoot=0;skillEvents.Clear();skillMeleeHits.Clear();skillMeleeNext.Clear();
            player.clip=clip;player.clock=0;actionTiming=Timing(clip);
            ApplySkillSourceState(.001f);
            SkillTrace("skill-phase",null,0);
        }
        private void SkillTrace(string type,Enemy enemy,int damage)
        {combatEvents.Add(new CombatEvent{type=type,clip=actionSkill==null?player.clip:actionSkill.id,time=elapsed,clock=skillPhaseTime,playerX=player.x,playerY=player.y,target=enemy==null?"":enemy.kind,targetX=enemy==null?0:enemy.x,targetY=enemy==null?0:enemy.y,targetHp=enemy==null?0:enemy.hp,state=enemy==null?"":enemy.state.ToString(),phase=skillPhase,windowIndex=-1,damage=damage});if(combatEvents.Count>5000)combatEvents.RemoveRange(0,1000);}
        private bool SkillEvent(string name,float time)
        {if(skillPhaseTime+0.00001f<time||skillEvents.Contains(name))return false;skillEvents.Add(name);return true;}
        private void SkillWorldStop(float at,float duration)
        {if(SkillEvent("world-stop-"+at,at)){RequestWorldStop(duration);SkillTrace("skill-world-stop",null,0);}}
        private void ApplySkillSourceState(float dt)
        {
            var s=SkillGeometry2016.State(player.clip);
            if(s.Invulnerable(skillPhaseTime))player.invincible=Math.Max(player.invincible,dt+.001f);
            foreach(var w in s.armor)if(skillPhaseTime>=w[0]&&skillPhaseTime<w[1])player.superArmor=Math.Max(player.superArmor,dt+.001f);
            foreach(var w in s.stops)SkillWorldStop(w[0],w[1]);
            for(int i=0;i<s.velocity.Length;i++){var v=s.velocity[i];if(skillPhaseTime>=v[0]&&skillEvents.Add("velocity-"+i)){if(v.Length>3&&v[3]==1){player.vx+=player.facing*v[1]*SkillUnit;player.vy+=v[2]*SkillUnit;}else{player.vx=player.facing*v[1]*SkillUnit;player.vy=v[2]*SkillUnit;}}}
        }
        private bool SkillHeld(Input input){return skillSlot>=0&&(input.skillHeldMask&(1<<skillSlot))!=0;}

        [Flags]
        private enum SkillExit
        {
            Walk=1, Dash=2, Jump=4, Attack=8, Skill=16, Awaken=32, Held=64,
            Recovery=Walk|Dash|Jump|Attack|Skill|Awaken
        }

        // Recovery is evaluated in this move's own animation clock. Multi-phase
        // actions become cancelable only after their final strike has committed.
        private SkillExit SkillRecoveryWindows()
        {
            float time=skillPhaseTime;
            switch(player.clip){
                case "counter-stance": return time>.60f ? SkillExit.Recovery : 0;
                case "mega-fire": return time>.64f ? SkillExit.Recovery : 0;
                case "rolling-land": return time>.30f ? SkillExit.Recovery : 0;
                case "windmill-end": return time>.8f/1.15f ? SkillExit.Recovery : 0;
                case "armor-break": return time>.43f ? SkillExit.Recovery : 0;
                case "kick": return time>.36f ? SkillExit.Recovery : 0;
                case "flame-geyser": return time>1.26f ? SkillExit.Recovery : 0;
                case "sonic-blade": return time>1f/.85f ? SkillExit.Recovery : 0;
                case "double-slash": return time>=3.667000055f ? SkillExit.Recovery|SkillExit.Held : 0;
                case "spiral-blast": return time>1.52f ? SkillExit.Recovery : 0;
                case "roll-end": return time>.266f ? SkillExit.Recovery : 0;
                case "unlimited-blade":
                case "unlimited-blade-awakened": return time>3.05f ? SkillExit.Recovery : 0;
                case "assault-slash": return time>1.07f ? SkillExit.Recovery : 0;
                case "triple-geyser": return time>1.63f ? SkillExit.Recovery : 0;
                case "mortal-blow":
                    if(time>.633f)return SkillExit.Walk|SkillExit.Dash|SkillExit.Skill|SkillExit.Awaken;
                    return time>.533f ? SkillExit.Dash : 0;
                case "impact-smash": return time>1.22f/1.1f ? SkillExit.Recovery : 0;
                case "armageddon-blade": return time>1.68f ? SkillExit.Recovery : 0;
                case "sandstorm": return time>1.45f ? SkillExit.Recovery : 0;
                default: return 0;
            }
        }

        private bool WantsSkillRecovery(Input input)
        {
            var exits=SkillRecoveryWindows();
            if(exits==0)return false;
            if((exits&SkillExit.Walk)!=0&&Finite(input.move)&&input.move!=0)return true;
            if((exits&SkillExit.Dash)!=0&&DashCancelRequested(input))return true;
            if((exits&SkillExit.Jump)!=0&&(jumpBuffered>0||input.drop&&(exits&SkillExit.Held)!=0))return true;
            if((exits&SkillExit.Attack)!=0&&presses.Count>0&&presses.Peek().expires-elapsed>=InputBufferDuration-RecoveryInputLead)
                return true;
            if((exits&SkillExit.Held)!=0&&(input.attackHeld||input.attackXHeld))return true;
            if((exits&SkillExit.Skill)==0)return false;
            if(skillBufferTime>=InputBufferDuration-RecoveryInputLead&&CanStartBufferedSkill(skillBuffered))return true;
            if((exits&SkillExit.Held)==0)return false;
            for(int slot=0;slot<SkillSlotCount;slot++)
                if((input.skillHeldMask&(1<<slot))!=0&&CanStartBufferedSkill(slot))return true;
            return false;
        }

        private bool CanStartBufferedSkill(int slot)
        {
            var skill=GetEquippedSkill(slot);
            return CanPaySkill(slot,skill)&&(skill.airborne||Grounded(player));
        }
        private void FinishSkill()
        {CancelAction(false);player.clip=Grounded(player)?"idle":"jump";player.clock=0;player.vx=0;}
        private void CancelSkillAction()
        {
            ClearCounterFractionalBonusR901();
            for(int i=skillAreas.Count-1;i>=0;i--)if(skillAreas[i].follow)skillAreas.RemoveAt(i);
            for(int i=effects.Count-1;i>=0;i--)if(effects[i].followPlayer&&effects[i].type!="iron-body"&&effects[i].type!="armageddon-blade")effects.RemoveAt(i);
            if(actionSkill!=null&&(actionSkill.id=="rolling-smash"||actionSkill.id=="windmill"))player.vx=0;
            skillForwardBlocked=false;skillPhase="";skillPhaseTime=skillPhaseDuration=skillStop=skillPoseRootCarry=0;skillSlot=-1;skillEvents.Clear();skillMeleeHits.Clear();
        }
        private void ResetSkillState(){ClearCounterFractionalBonusR901();skillAreas.Clear();skillDebuffs.Clear();mortalGroupNext.Clear();skillMagic=false;ArmageddonRemaining=SkillWorldStopRemaining=0;HyperEssence=3;SkillCastSerial=0;LastCastSkillId="";skillRandom=20160420;}
        private void ClearRoomSkillEffects()
        {ClearCounterFractionalBonusR901();skillAreas.Clear();skillDebuffs.Clear();mortalGroupNext.Clear();SkillWorldStopRemaining=0;for(int i=effects.Count-1;i>=0;i--)if(effects[i].type!="iron-body"&&effects[i].type!="armageddon-blade")effects.RemoveAt(i);}
        private int ApplyPlayerDefense(int damage)
        {
            float multiplier=armor>0?ArmorDamageMultiplier:1;
            if(actionSkill!=null&&actionSkill.id=="windmill"&&skillPhase=="loop")multiplier*=.5f;
            return Math.Max(1,(int)Math.Round(damage*multiplier));
        }
        private int ModifyEnemyDamage(Enemy enemy,int damage,out bool critical)
        {
            damage=CriticalDamage(damage,CriticalChance,out critical);
            var d=GetEnemyDebuff(enemy);
            if(d!=null&&enemy.invincible<=0&&d.stun>0&&d.stunHits>0&&--d.stunHits==0){d.stun=0;enemy.stun=Math.Min(enemy.stun,.1f);}
            // Existing damage values are after a normalized demo defense of100.
            // Source Armor Break multiplies that physical defense by .6:
            // before/after mitigation ratio = (100+100)/(100+60) =1.25.
            int plainPairDamageR1258;if(TryPlainPairDefenceAuthor2DR1258(enemy,damage,skillMagic,d!=null&&d.armorBreak>0,out plainPairDamageR1258))return plainPairDamageR1258;
            return d!=null&&d.armorBreak>0&&!skillMagic?Math.Max(1,(int)Math.Round(damage*1.25f)):damage;
        }
        private bool TryCounterPlayerHit(int damage,float fromX,HitReaction reaction)
        {
            if(!CounterWaiting||damage<=0||player.hp<=0||player.invincible>0||skillPhaseTime<=.06f||skillPhaseTime>=.5f||(fromX-player.x)*player.facing<0)return false;
            ClearCounterFractionalBonusR901();
            skillPower=1;counterExtraDamage=damage;
            for(int i=effects.Count-1;i>=0;i--)if(effects[i].type=="counter-attack"&&effects[i].phase==0)effects.RemoveAt(i);
            BeginSkillPhase("counter","counter-attack",.8f);
            SkillVisual("counter-attack",1,.55f,player.x+player.facing*28,player.y+30,.75f,.75f,true,28,30);
            player.invincible=Math.Max(player.invincible,.03f);
            StaggerCounterAttackers(fromX);
            return true;
        }
        private void StaggerCounterAttackers(float fromX)
        {
            foreach(var e in enemies)
            {
                if(!e.active||e.dormant||e.hp<=0||e.passive||Math.Abs(e.x-fromX)>=24||
                   e.invincible>0||EnemyHasSuperArmor(e)||EnemyInitialHurtboxHiddenV25(e)||EnemyHasNpcInvincibility(e))continue;
                e.telegraph=0;e.stun=Math.Max(e.stun,.3f);e.state=CombatState.Hurt;e.clip="hurt";e.clock=0;e.vx=e.vy=0;
            }
        }
        private float counterExtraDamageFractionalR901;
        private bool counterExtraDamageIsFractionalR901;
        void ClearCounterFractionalBonusR901(){counterExtraDamageFractionalR901=0;counterExtraDamageIsFractionalR901=false;}
        float ApplyBelowCannonFractionalDefenseR901(float damage){
            float multiplier=armor>0?ArmorDamageMultiplier:1;
            if(actionSkill!=null&&actionSkill.id=="windmill"&&skillPhase=="loop")multiplier*=.5f;
            return damage*multiplier;
        }
        bool TryCounterPlayerFractionalHitR901(float damage,float fromX,HitReaction reaction){
            if(!CounterWaiting||damage<=0||player.hp<=0||player.invincible>0||skillPhaseTime<=.06f||skillPhaseTime>=.5f||(fromX-player.x)*player.facing<0)return false;
            skillPower=1;counterExtraDamage=0;counterExtraDamageFractionalR901=damage;counterExtraDamageIsFractionalR901=true;
            for(int i=effects.Count-1;i>=0;i--)if(effects[i].type=="counter-attack"&&effects[i].phase==0)effects.RemoveAt(i);
            BeginSkillPhase("counter","counter-attack",.8f);
            SkillVisual("counter-attack",1,.55f,player.x+player.facing*28,player.y+30,.75f,.75f,true,28,30);
            player.invincible=Math.Max(player.invincible,.03f);
            StaggerCounterAttackers(fromX);
            return true;
        }
        private int counterExtraDamage;
        private Effect SkillVisual(string id,int phase,float life,float x,float y,float sx,float sy,bool follow=false,float ox=0,float oy=0,float? groundY=null)
        {
            var e=new Effect{type=id,phase=phase,x=x,y=y,facing=player.facing,age=0,lifetime=life,scaleX=sx,scaleY=sy,followPlayer=follow,offsetX=ox,offsetY=oy};
            // A world-space burst keeps its original floor when the caster changes platforms.
            e.visualGroundY=groundY ?? GroundAt(x);
            effects.Add(e);return e;
        }
        private SkillArea Area(string skill,string mesh,float x,float y,float life,float start,float end,float repeat,float coefficient,HitReaction reaction,float backX,float backY)
        {
            var a=new SkillArea{skill=skill,mesh=mesh,x=x,y=y,birthX=x,birthY=y,life=life,start=start,end=end,repeat=repeat,damage=coefficient*SkillAttackPower*skillPower,reaction=reaction,backX=backX*SkillUnit,backY=backY*SkillUnit,facing=player.facing};
            a.mpGain=mesh=="mega"||mesh=="sonic"?10:mesh=="flame"?3:mesh=="windmill"?.94f:mesh=="rolling-land"?1:mesh=="double-first"||mesh=="spiral-first"?2.5f:mesh=="double-second"?2.75f:mesh=="spiral-main"?3.5f:0;
            skillAreas.Add(a);return a;
        }
        private SkillGroundPoint TraceSkillGround(SkillGroundPoint from,float x)
        {
            if(!NativeTerrain)return new SkillGroundPoint(x,GroundAt(CurrentRoom,x),null);
            var floor=from.Support;
            if(floor==null||floor.hamelDisabled)return from;
            bool right=x>=from.X;
            const float tolerance=.05f;
            while(x<floor.x1||x>floor.x2){
                float edge=right?floor.x2:floor.x1;
                float edgeHeight=floor.Y(edge);
                GroundSurface continuation=null;
                float nearestHeight=NativeWalkStepHeight;
                foreach(var candidate in CurrentRoom.surfaces){
                    if(candidate.hamelDisabled||candidate.npcOnly)continue;
                    if(right ? candidate.x1>edge+tolerance||candidate.x2<=edge+tolerance
                             : candidate.x2<edge-tolerance||candidate.x1>=edge-tolerance)continue;
                    float heightGap=Math.Abs(candidate.Y(Clamp(edge,candidate.x1,candidate.x2))-edgeHeight);
                    if(heightGap>nearestHeight)continue;
                    continuation=candidate;
                    nearestHeight=heightGap;
                }
                // A distant lower deck is a different fighting plane. Keep
                // floor-bound magic at the edge when its support ends.
                if(continuation==null)return new SkillGroundPoint(edge,edgeHeight,floor);
                floor=continuation;
                if(right?x<floor.x1:x>floor.x2)x=right?floor.x1:floor.x2;
            }
            return new SkillGroundPoint(x,floor.Y(x),floor);
        }
        private SkillArea GroundArea(string skill,string mesh,SkillGroundPoint floor,float offsetY,float life,float start,float end,float repeat,float coefficient,HitReaction reaction,float backX,float backY)
        {
            var area=Area(skill,mesh,floor.X,floor.Y+offsetY,life,start,end,repeat,coefficient,reaction,backX,backY);
            area.groundRelative=true;
            area.birthGround=area.ground=floor;
            return area;
        }
        private bool MeshIntersects(SkillArea a,Enemy e)
        {
            var shapes=SkillGeometry2016.Sample(a.mesh,a.age);if(shapes==null)return false;
            if(NpcHurtSpheres(e)!=null){for(int i=0;i+2<shapes.Length;i+=3)if(EnemyCircleIntersects(e,a.x+shapes[i]*a.facing,a.y+shapes[i+1],shapes[i+2]))return true;return false;}
            var bounds=EnemyBounds(e);
            for(int i=0;i+2<shapes.Length;i+=3)
            {
                float cx=a.x+shapes[i]*a.facing,cy=a.y+shapes[i+1],r=shapes[i+2];
                float closestX=Clamp(cx,bounds.left,bounds.right),closestY=Clamp(cy,bounds.bottom,bounds.top);
                float dx=cx-closestX,dy=cy-closestY;if(dx*dx+dy*dy<=r*r)return true;
            }
            return false;
        }
        private int DealSkillHit(Enemy e,float damage,HitReaction reaction,float backX,float backY,float stun,bool magic=false,float attackerStop=0,float defenderStop=0,float mpGain=0)
        {
            float beforeMana=mana;skillMagic=magic;
            int rounded=Math.Max(1,(int)Math.Round(damage));
            try {
                int result=reaction==HitReaction.None?ApplyHitToEnemy(e,rounded,HitReaction.None):ApplySourceHit(e,rounded,reaction,backX,backY,stun,attackerStop,defenderStop);
                if(result>0)mana=Math.Min(MaxMana,beforeMana+mpGain);
                return result;
            } finally {skillMagic=false;}
        }
        private void EvaluateSkillArea(SkillArea a)
        {
            if(a.age<a.start||a.age>a.end)return;
            foreach(var e in enemies)
            {
                if(!e.active||e.dormant||(e.passive&&!CoreHardBuffDeviceDamageableR1417(e)&&!IsPeitaCrossbow(e)&&!IsRegionHitFixture(e))||e.hp<=0)continue;
                float next;if(a.nextHit.TryGetValue(e,out next)&&(a.repeat<=0||a.age+0.00001f<next))continue;
                if(a.grouped&&mortalGroupNext.TryGetValue(e,out next)&&elapsed<next)continue;
                if(!MeshIntersects(a,e))continue;
                bool statusOnly=a.damage==0;if(statusOnly&&(e.invincible>0||EnemyInitialHurtboxHiddenV25(e)||EnemyHasNpcInvincibility(e)))continue;
                float statusStun=a.stun>0&&!EnemyHasSuperArmor(e)&&!CoreHardShieldGuardingR1417(e,player.x)&&SkillRandom()<=a.stunChance?a.stun:0;
                int dealt=statusOnly?0:DealSkillHit(e,a.damage,a.reaction,a.backX,a.backY,statusStun,a.magic,0,a.defenderStop,a.mpGain);if(!statusOnly&&dealt<=0)continue;
                if(statusOnly&&statusStun>0&&e.state!=CombatState.Knockdown&&e.state!=CombatState.GettingUp){e.stun=Math.Max(e.stun,statusStun);e.state=CombatState.Hurt;e.clip="hurt";e.vx=0;e.hitPause=Math.Max(e.hitPause,a.defenderStop);}
                SkillTrace((statusOnly?"skill-control:":"skill-hit:")+a.mesh,e,dealt);
                if(a.skill=="double-slash" && dealt>0)DoubleSlashContact(a,e,next);
                a.nextHit[e]=a.age+Math.Max(.001f,a.repeat);
                if(a.grouped)mortalGroupNext[e]=elapsed+a.repeat;
                var d=Debuff(e);
                if(a.burn>0){d.burn=a.burn;d.burnPower=a.burnPower;if(d.burnTick<=0)d.burnTick=1;}
                if(statusStun>0){d.stun=statusStun;d.stunHits=a.stunHits;}
                if(a.accuracy>0)d.accuracy=a.accuracy;
                if(a.mortalTrail)d.knockdownGauge=Math.Max(0,d.knockdownGauge-10);
            }
        }

        void DoubleSlashContact(SkillArea area, Enemy target, float nextHitWasDue)
        {
            bool finisher = area.mesh == "double-second";
            // One accent for the first contact, then roughly every third pulse
            // of the finishing arc. Each target keeps its own hit history.
            if (finisher && nextHitWasDue > 0) {
                float interval = area.repeat * 3;
                float lastContact = nextHitWasDue - area.repeat;
                if ((int)(area.age / interval) == (int)(lastContact / interval)) return;
            }
            var bounds = EnemyBlockingBounds(target);
            var effect = SkillVisual("double-contact",finisher ? 1 : 0,finisher ? .125f : .16f,
                (bounds.left+bounds.right)*.5f,(bounds.bottom+bounds.top)*.5f,1,1);
            effect.facing=area.facing;
        }
        private void ApplyBurnTick(Enemy e)
        {
            // BBT_CHANGE_NOW_HP_PER_SECOND uses a resource delta, not the
            // attack pipeline: no combo, knockdown reduction, armor, stun-hit
            // finalizer or invincibility test is involved in this tick.
            int delta=Math.Min((int)Math.Ceiling(e.hp),(int)Math.Round(Debuff(e).burnPower*SkillAttackPower));
            if(delta<=0)return;e.hp=Math.Max(0,e.hp-delta);SpawnDamageNumber(e,delta,false);SkillTrace("skill-burn",e,delta);PlainPairAfterBurnResourceDeltaR1258(e,delta);
            if(e.hp<=0){e.state=CombatState.Dead;e.clip="death";e.clock=0;e.stun=0;kills++;stageKills++;}
        }
        private void TickSkillEffects(float dt)
        {
            ArmageddonRemaining=Math.Max(0,ArmageddonRemaining-dt);SkillWorldStopRemaining=Math.Max(0,SkillWorldStopRemaining-dt);
            // Hitstop freezes the source action clock. Its invincibility and
            // armor windows must therefore remain in force during that pause.
            if(actionSkill!=null)ApplySkillSourceState(dt);
            if(actionSkill!=null||skillAreas.Count>0)activeHitBoxes.Clear();
            foreach(var effect in effects)if(effect.followPlayer){effect.x=player.x+player.facing*effect.offsetX;effect.y=player.y+effect.offsetY;effect.facing=player.facing;if(effect.type=="iron-body"&&effect.phase==0&&effect.age>=.4f){effect.phase=1;effect.age=0;effect.lifetime=armor;}}
            foreach(var pair in skillDebuffs)
            {
                var e=pair.Key;var d=pair.Value;if(e.hitPause>0)continue;d.armorBreak=Math.Max(0,d.armorBreak-dt);d.stun=Math.Max(0,d.stun-dt);d.accuracy=Math.Max(0,d.accuracy-dt);
                if(d.stun>0&&e.hp>0&&!EnemyHasSuperArmor(e)&&!EnemyInitialHurtboxHiddenV25(e)&&!EnemyHasNpcInvincibility(e)&&e.state!=CombatState.Knockdown&&e.state!=CombatState.GettingUp){e.stun=Math.Max(e.stun,d.stun);e.state=CombatState.Hurt;e.clip="hurt";}
                if(d.burn>0&&e.hp>0)
                {
                    d.burn=Math.Max(0,d.burn-dt);d.burnTick-=dt;
                    if(d.burnTick<=0){d.burnTick+=1;ApplyBurnTick(e);}
                }
            }
            for(int i=skillAreas.Count-1;i>=0;i--)
            {
                var a=skillAreas[i];a.age+=dt;
                if(a.age>a.life){skillAreas.RemoveAt(i);continue;}
                if(a.follow){a.x=player.x+a.offsetX*player.facing;a.y=player.y+a.offsetY;a.facing=player.facing;}
                else{a.x=a.birthX+a.vx*Math.Min(a.age,a.moveUntil);a.y=a.birthY+a.vy*Math.Min(a.age,a.moveUntil);}
                if(a.mesh=="triple-primary")a.y=a.birthY+(-1200*Math.Min(a.age,.1f)+1200*Clamp(a.age-.455f,0,.1f))*SkillUnit;
                if(a.sand){a.x=a.birthX+a.facing*(1000*a.age+200*Math.Max(0,a.age-.8f))*SkillUnit;if(a.age>=1.3f){a.reaction=HitReaction.Launch;a.backX=0;}}
                if(a.groundRelative){
                    if(a.x!=a.ground.X)a.ground=TraceSkillGround(a.ground,a.x);
                    a.x=a.ground.X;
                    a.y+=a.ground.Y-a.birthGround.Y;
                }
                if(a.mortalTrail)while(a.childAt<=Math.Min(a.age,.61f))
                {
                    float x=a.birthX+a.vx*Math.Min(a.childAt,a.moveUntil);
                    a.childAt+=.05f;
                    var floor=TraceSkillGround(a.birthGround,x);
                    if(floor.X==a.lastTrailX)continue;
                    a.lastTrailX=floor.X;
                    var ground=GroundArea(a.skill,"mortal-ground",floor,0,1,.25f,1,.5f,2.925f,HitReaction.None,0,0);
                    ground.damage=2.925f*SkillAttackPower;ground.grouped=true;ground.burn=3;ground.burnPower=.1f;ground.facing=a.facing;
                    SkillVisual(a.skill,1,1,floor.X,floor.Y+12,.45f,.35f,groundY:floor.Y);
                }
                if(a.visual!=null){
                    a.visual.x=a.x;
                    a.visual.y=a.y+38;
                    if(a.groundRelative)a.visual.visualGroundY=a.ground.Y;
                }
                if(a.age>=a.start&&a.age<=a.end)
                {var shapes=SkillGeometry2016.Sample(a.mesh,a.age);if(shapes!=null)for(int h=0;h+2<shapes.Length;h+=3){float x=a.x+shapes[h]*a.facing,y=a.y+shapes[h+1],r=shapes[h+2];activeHitBoxes.Add(new AttackBox{left=x-r,right=x+r,bottom=y-r,top=y+r,shape="skill:"+a.mesh,windowIndex=-1});}}
                EvaluateSkillArea(a);
                if(a.wind)foreach(var e in a.nextHit.Keys)if(e.hp>0&&!e.passive&&Math.Abs(e.x-a.x)<750*SkillUnit&&!EnemyHasSuperArmor(e))
                {float pull=Clamp(a.x-e.x,-1500*SkillUnit*dt,1500*SkillUnit*dt);MoveAlongGround(e,Clamp(e.x+pull,22,RoomWidth-26));}
                if(a.pullSpeed>0)foreach(var e in enemies)if(e.active&&e.hp>0&&!e.passive&&Math.Abs(e.x-a.x)<a.pullRange&&!EnemyHasSuperArmor(e))
                {float pull=Clamp(a.x-e.x,-a.pullSpeed*dt,a.pullSpeed*dt);MoveAlongGround(e,Clamp(e.x+pull,22,RoomWidth-26));}
            }
        }
        private void FollowArea(SkillArea a,float x,float y){a.follow=true;a.offsetX=x;a.offsetY=y;}
        private void MoveSkillRoot(float dt)
        {
            float root=SkillGeometry2016.Root(player.clip,skillPhaseTime,player.clip=="rolling-loop"||player.clip=="windmill-loop");
            float step=root-skillMotionRoot;
            if((actionSkill.id!="windmill"||skillPhase!="loop")&&!skillForwardBlocked)
                MovePlayerAlongGround(player.x+player.facing*step);
            skillMotionRoot=root;
        }
        private void NativeSpeed(float sourceTime,float dt,float[] times,float[] speeds)
        {for(int i=0;i<times.Length;i++)if(sourceTime+0.00001f>=times[i]&&skillEvents.Add("velocity-"+i)){player.vx=player.facing*speeds[i]*SkillUnit;player.vy=0;}}
        private bool SkillPartIntersects(Enemy e,float clock,string part)
        {
            if(part=="Sword")return WeaponIntersects(e,clock,actionTiming);
            var g=GeometryAt(clock,actionTiming);if(g==null)return false;float ax,ay,bx,by;Segment(g,part,player.x,player.y,out ax,out ay,out bx,out by);
            bool armorBreak=actionSkill.id=="armor-break";
            float radius=armorBreak ? ArmorBreakBootRadius : 5;
            if(EnemySegmentIntersects(e,ax,ay,bx,by,radius))return true;
            if(!armorBreak)return false;
            var impact=ArmorBreakImpactBounds(ax,ay,bx,by,part);
            if(AreaIntersects(e,impact.left,impact.bottom,impact.right,impact.top))return true;
            if(!Grounded(player) || !Grounded(e))return false;
            // A grounded lunge stops at the front of the enemy's solid body.
            // Individual damage spheres can leave a gap there at ankle height.
            // Accept shoe contact with that same front face, including on slopes.
            var body=EnemyBlockingBounds(e);
            float front=player.facing>0 ? body.left : body.right;
            float far=player.facing>0 ? body.right : body.left;
            float gap=(front-player.x)*player.facing;
            if(gap>9.8f+.05f || (far-player.x)*player.facing<0 || CoreArenaBodyPassableV59(e))return false;
            return LineHitsBox(ax,ay,bx,by,body,radius);
        }
        // The active ankle-to-toe poses need up to ~6 pixels to cover both boot silhouettes.
        const float ArmorBreakBootRadius=6.25f;
        private AttackBox ArmorBreakImpactBounds(float ax,float ay,float bx,float by,string foot)
        {
            const float forwardReach=12, shinHeight=14;
            return new AttackBox {
                left=Math.Min(ax,bx)-ArmorBreakBootRadius-(player.facing<0?forwardReach:0),
                right=Math.Max(ax,bx)+ArmorBreakBootRadius+(player.facing>0?forwardReach:0),
                bottom=Math.Min(ay,by)-ArmorBreakBootRadius,
                top=Math.Max(ay,by)+ArmorBreakBootRadius+shinHeight,
                shape=foot,windowIndex=-1
            };
        }
        private static readonly string[] armorBreakFeet={"Rfoot","Lfoot"}, kickFoot={"Lfoot"};
        private void MeleeSkill(float start,float end,float damage,HitReaction reaction,float bx,float by,float stun,string debuff=null,float repeat=0,float mpGain=-1,float attStop=0,float defStop=0,string shape=null)
        {
            if(skillPhaseTime<start||skillPreviousTime>end)return;
            float clock=Math.Min(end,skillPhaseTime);if(player.clip=="rolling-loop")clock%=.266666667f/1.5f;
            string[] parts=shape!=null?new[]{shape}:actionSkill.id=="armor-break"?armorBreakFeet:actionSkill.id=="kick"?kickFoot:defaultSwordBoxes;
            foreach(var part in parts){
                if(actionSkill.id=="armor-break"){
                    var geometry=GeometryAt(clock,actionTiming);
                    if(geometry==null)continue;
                    float ax,ay,bxFoot,byFoot;Segment(geometry,part,player.x,player.y,out ax,out ay,out bxFoot,out byFoot);
                    activeHitBoxes.Add(ArmorBreakImpactBounds(ax,ay,bxFoot,byFoot,part));
                }else{
                    var b=GeometryBounds(clock,actionTiming,part,-1);
                    if(part!="Sword"){b.left-=2;b.right+=2;b.bottom-=2;b.top+=2;}
                    activeHitBoxes.Add(b);
                }
            }
            foreach(var e in enemies)
            {
                bool contact=false;float from=Math.Max(start,skillPreviousTime),to=Math.Min(end,skillPhaseTime);int samples=Math.Max(1,(int)Math.Ceiling((to-from)*actionTiming.fps*3));
                for(int step=0;step<=samples&&!contact;step++){float sample=from+(to-from)*step/samples;if(player.clip=="rolling-loop")sample%=.266666667f/1.5f;foreach(var part in parts)if(SkillPartIntersects(e,sample,part)){contact=true;break;}}
                float next;if(!e.active||e.dormant||e.hp<=0||(e.passive&&!IsRegionHitFixture(e))||(! (repeat>0)&&skillMeleeHits.Contains(e))||repeat>0&&skillMeleeNext.TryGetValue(e,out next)&&skillPhaseTime<next||!contact)continue;
                int dealt=DealSkillHit(e,damage,reaction,bx*SkillUnit,by*SkillUnit,stun,false,attStop>0?attStop:actionSkill.id=="kick"?.1f:0,defStop>0?defStop:actionSkill.id=="kick"?.2f:actionSkill.id=="armor-break"?.1f:0,mpGain>=0?mpGain:actionSkill.id=="kick"?1.5f:actionSkill.id=="armor-break"?3:.25f);if(dealt<=0)continue;
                SkillTrace("skill-hit:foot-or-sword",e,dealt);
                skillMeleeHits.Add(e);
                skillMeleeNext[e]=skillPhaseTime+repeat;
                if(debuff=="armor"){
                    Debuff(e).armorBreak=7;
                    var body=EnemyBlockingBounds(e);
                    SkillVisual(actionSkill.id,0,.3f,Clamp(player.x,body.left,body.right),Clamp(player.y+8,body.bottom,body.top),.55f,.55f);
                }
                if(debuff=="kick")Debuff(e).knockdownGauge=Math.Max(0,Debuff(e).knockdownGauge-50);
            }
        }
        private void TickSkillAction(float dt,Input input)
        {
            if(actionSkill==null)return;
            if(skillStop>0){ApplySkillSourceState(dt);float frozen=Math.Min(dt,skillStop);skillStop=Math.Max(0,skillStop-frozen);dt-=frozen;player.clock=skillPhaseTime;if(dt<=.000001f)return;}
            skillPreviousTime=skillPhaseTime;skillPhaseTime+=dt;player.clock=skillPhaseTime;
            ApplySkillSourceState(dt);
            MoveSkillRoot(dt);
            string id=actionSkill.id;
            if(id=="mega-slash")
            {
                if(skillPhase=="ready")
                {
                    if(skillPhaseTime>=skillPhaseDuration){if(SkillHeld(input)){BeginSkillPhase("charge","mega-charge",1.7f);SkillVisual(id,1,1.7f,player.x,player.y+35,.5f,.5f,true,0,35);}else BeginSkillPhase("fire","mega-fire",1.066666667f/.9f);}
                    return;
                }
                if(skillPhase=="charge")
                {
                    if(!SkillHeld(input)||skillPhaseTime>=1.7f){skillHoldTime=skillPhaseTime;skillPower=skillHoldTime>=1?1.5f:1;BeginSkillPhase("fire","mega-fire",1.066666667f/.9f);}return;
                }
                NativeSpeed(skillPhaseTime*.9f,dt,new[]{0f,.014f,.264f},new[]{740f,1600f,700f});
                if(SkillEvent("slash",.094f/.9f))
                {var a=Area(id,"mega",player.x,player.y+19,.17f/.9f,0,.17f/.9f,0,16.02f*1.2f,HitReaction.Knockdown,1050,0);FollowArea(a,0,19);SkillVisual(id,2,.4f,player.x+player.facing*28,player.y+36,.9f,.9f,true,28,36);}
            }
            else if(id=="rolling-smash")
            {
                if(skillPhase=="ready")
                {
                    if(skillPhaseTime>=skillPhaseDuration){BeginSkillPhase("spin","rolling-loop",100);player.vy=1000*SkillUnit;player.state=CombatState.Airborne;player.vx=0;SkillVisual(id,1,.55f,player.x,player.y+32,.72f,.72f,true,0,32);}return;
                }
                if(skillPhase=="spin")
                {
                    if(Grounded(player)&&skillPhaseTime>.04f){BeginSkillPhase("land","rolling-land",.6f);return;}
                    MovePlayerAlongGround(player.x+player.facing*(9.3f*100*SkillUnit/(2*1000f/4000f))*dt);player.vx=0;
                    if(SkillEvent("spin-hit-"+(int)(skillPhaseTime/.14f),(int)(skillPhaseTime/.14f)*.14f))
                    {skillMeleeHits.Clear();MeleeSkill(0,100,.76f*SkillAttackPower,HitReaction.Hurt,1000,0,.08f);}
                    return;
                }
                if(SkillEvent("land-hit",.03f))
                {var a=Area(id,"rolling-land",player.x+player.facing*160*SkillUnit,player.y+50*SkillUnit,.1f,0,.1f,0,5.12f,HitReaction.Hurt,600,1300);a.defenderStop=.166f;SkillVisual(id,2,.4f,player.x+player.facing*40,player.y+24,.9f,.6f);}
            }
            else if(id=="windmill")
            {
                if(skillPhase=="start")
                {
                    if(SkillEvent("wind",.35f)){var a=Area(id,"windmill",player.x,player.y+30*SkillUnit,6,0,6,.14f,1.35f,HitReaction.Hurt,-100,0);FollowArea(a,0,30*SkillUnit);a.wind=true;SkillVisual(id,1,6,player.x,player.y+45,1.05f,1.05f,true,0,45);}
                    if(skillPhaseTime>=skillPhaseDuration)BeginSkillPhase("loop","windmill-loop",4.9f);return;
                }
                if(skillPhase=="loop")
                {
                    float move=Finite(input.move)?Clamp(input.move,-1,1):0;if(move!=0)player.facing=move>0?1:-1;
                    MovePlayerAlongGround(player.x+move*740*SkillUnit*1.75f*SkillMoveMultiplier*dt);player.vx=0;
                    if(input.jump&&Grounded(player)){player.vy=1500*SkillUnit;player.state=CombatState.Airborne;}
                    if(skillPhaseTime>2.1f)
                    {
                        float cost=100f/2.8f*dt;
                        if(!SkillHeld(input)||skillPhaseTime>=4.9f||mana<cost)
                        {
                            for(int i=skillAreas.Count-1;i>=0;i--)if(skillAreas[i].wind)skillAreas.RemoveAt(i);
                            for(int i=effects.Count-1;i>=0;i--)if(effects[i].type==id&&effects[i].phase==1)effects.RemoveAt(i);
                            BeginSkillPhase("end","windmill-end",1.1f/1.15f);SkillVisual(id,2,.4f,player.x,player.y+28,.85f,.65f);return;
                        }
                        mana-=cost;
                    }
                    return;
                }
            }
            else if(id=="counter-attack")
            {
                if(skillPhase=="stance"){player.vx=0;if(skillPhaseTime>=1.2f||WantsSkillRecovery(input))FinishSkill();return;}
                if(SkillEvent("self-stop",.15f)){skillStop=.3f;return;}
                if(counterExtraDamageIsFractionalR901)
                    MeleeSkill(.23f,.33f,5.25f*SkillAttackPower+counterExtraDamageFractionalR901,HitReaction.Launch,1100,1900,.12f);
                else
                MeleeSkill(.23f,.33f,5.25f*SkillAttackPower+counterExtraDamage,HitReaction.Launch,1100,1900,.12f);
            }
            else if(id=="armor-break"||id=="kick")
            {
                bool armorBreak=id=="armor-break";float at=armorBreak?.22f:.1f;
                if(!armorBreak&&SkillEvent("foot-fx",at))SkillVisual(id,0,.3f,player.x+player.facing*33,player.y+22,.55f,.55f,true,33,22);
                MeleeSkill(at,armorBreak?.27f:.22f,actionSkill.damage,HitReaction.Hurt,armorBreak?700:800,armorBreak?900:-600,armorBreak?.1f:.2f,armorBreak?"armor":"kick");
            }
            else if(id=="flame-geyser")
            {
                NativeSpeed(skillPhaseTime,dt,new[]{.01f,.15f,.4f,.543f,.7f},new[]{500f,700f,900f,500f,0f});
                if(SkillEvent("column",.751f)){
                    var floor=TraceSkillGround(skillCastGround,player.x+player.facing*150*SkillUnit);
                    var a=GroundArea(id,"flame",floor,0,.57f,0,.57f,.08f,4.88f*1.2f,HitReaction.Launch,0,1500);
                    a.magic=true;a.burn=10;
                    SkillVisual(id,0,.6333333f,a.x,floor.Y+62,.95f,1.1f,groundY:floor.Y);
                }
            }
            else if(id=="sonic-blade")
            {
                if(SkillEvent("shockwave",.501f/.85f)){var a=Area(id,"sonic",player.x,player.y,.48f,0,.48f,0,15.86f,HitReaction.Hurt,0,0);a.stun=2;a.stunHits=2;a.defenderStop=.12f;SkillVisual(id,0,.48f,a.x,player.y+30,1,1);}
            }
            else if(id=="double-slash")
            {
                if(SkillEvent("first-impact",.229f)){var a=Area(id,"double-first",player.x+player.facing*50*SkillUnit,player.y+19,.15f,.1f,.15f,0,5.37f,HitReaction.Hurt,0,0);a.vx=player.facing*750*SkillUnit;a.moveUntil=.15f;a.stun=4;a.stunHits=2;a.defenderStop=.2f;}
                if(SkillEvent("first-visual",.404f))SkillVisual(id,0,.55f,player.x+player.facing*70,player.y+42,.95f,.95f);
                if(SkillEvent("second-visual",1.854f))SkillVisual(id,1,1.0f,player.x+player.facing*70,player.y+58,1.35f,1.35f);
                if(SkillEvent("second-impact",2.1206f))Area(id,"double-second",player.x-player.facing*350*SkillUnit,player.y+250*SkillUnit,.7f,0,.7f,.06f,6.21f,HitReaction.Launch,500,750);
            }
            else if(id=="spiral-blast")
            {
                NativeSpeed(skillPhaseTime,dt,new[]{.1f,.275f},new[]{1750f,0f});
                if(SkillEvent("first",.231f)){var a=Area(id,"spiral-first",player.x-player.facing*70*SkillUnit,player.y-35*SkillUnit,.12f,0,.12f,0,4.69f,HitReaction.Hurt,750,0);a.stun=2;a.stunHits=2;SkillVisual(id,0,.45f,player.x+player.facing*40,player.y+30,.8f,.8f);}
                if(SkillEvent("blast",.951f)){var a=Area(id,"spiral-main",player.x+player.facing*300*SkillUnit,player.y,1,.01f,1,.13f,6.24f,HitReaction.Launch,950,700);a.vx=player.facing*850*SkillUnit;a.vy=-500*SkillUnit;a.moveUntil=.1f;a.visual=SkillVisual(id,1,1,a.x,player.y+38,1.1f,1.1f);}
            }
            else if(id=="roll")
            {
                if(skillPhase=="roll")
                {
                    MeleeSkill(.1f/1.3f,100,actionSkill.damage,HitReaction.Hurt,-300,0,.15f,null,0,.75f,.1f,.233f,"Lfoot");
                    if(skillPhaseTime>=skillPhaseDuration&&Grounded(player)){BeginSkillPhase("land","roll-end",.666666667f);player.vx=0;}return;
                }
                if(SkillEvent("dust",.03333f))SkillVisual(id,1,.3f,player.x,player.y+12,.6f,.3f);
            }
            else if(id=="unlimited-blade")
            {
                float[] starts={.166f,.433f,.666f,.966f,1.3f,1.5f},ends={.233f,.5f,.733f,1.033f,1.366f,1.566f};
                for(int i=0;i<starts.Length;i++)
                {
                    if(SkillEvent("swing-"+i,starts[i])){skillMeleeHits.Clear();SkillVisual(id,0,.22f,player.x+player.facing*30,player.y+30,.7f,.7f,true,30,30);}
                    MeleeSkill(starts[i],ends[i],3.49f*1.5f*SkillAttackPower,HitReaction.Hurt,700,0,.12f,null,0,2.08f);
                }
                if(SkillEvent("magnum",1.730f)){var a=Area(id,"unlimited-final",player.x,player.y,.966666639f,0,.966666639f,.15f,3.23f*1.5f,HitReaction.Launch,300,800);a.magic=true;a.mpGain=1.79f;SkillVisual(id,1,.966666639f,a.x+player.facing*32,a.y+65,1.2f,1.35f);}
            }
            else if(id=="assault-slash")
            {
                if(SkillEvent("thrust",.37f))SkillVisual(id,0,.4f,player.x+player.facing*30,player.y+27,1,.5f,true,30,27);
                MeleeSkill(.37f,.7f,2.92f*1.2f*SkillAttackPower,HitReaction.Hurt,500,0,.1f,null,.06f,1.67f);
            }
            else if(id=="triple-geyser")
            {
                float[] offsets={150,425,700};for(int i=0;i<3;i++)if(SkillEvent("geyser-"+i,.751f+i*.2f))
                {
                    float x=player.x+player.facing*offsets[i]*SkillUnit;
                    var floor=TraceSkillGround(skillCastGround,x);
                    if(floor.X==lastGeyserX)continue;
                    lastGeyserX=floor.X;
                    var a=GroundArea(id,"triple-primary",floor,0,.57f,0,.57f,.1f,3.17f*1.2f,HitReaction.Launch,-1500,1000);a.magic=true;a.mpGain=1.25f;
                    a=GroundArea(id,"triple-secondary",floor,100*SkillUnit,.57f,0,.57f,.1f,3.17f*1.2f,HitReaction.Launch,750,1000);a.magic=true;a.mpGain=1.25f;
                    SkillVisual(id,0,.6333333f,floor.X,floor.Y+65,1.1f,1.4f,groundY:floor.Y);
                }
            }
            else if(id=="mortal-blow")
            {
                if(SkillEvent("ground-front",.25f))
                {
                    var floor=TraceSkillGround(skillCastGround,player.x);
                    var a=GroundArea(id,"mortal-front",floor,0,.65f,0,.55f,0,11.7f,HitReaction.Hurt,-200,0);a.vx=player.facing*1750*SkillUnit;a.moveUntil=.55f;a.mortalTrail=true;a.burn=10;a.defenderStop=.5f;a.mpGain=3.33f;
                    a.visual=SkillVisual(id,0,.65f,a.x,floor.Y+38,1.15f,1.0f,groundY:floor.Y);
                }
            }
            else if(id=="impact-smash")
            {
                if(SkillEvent("second-contact",.713f/1.1f))skillMeleeHits.Clear();
                MeleeSkill(.266f/1.1f,.345f/1.1f,.95f*SkillAttackPower,HitReaction.Hurt,400,0,.1f,null,0,.25f);
                MeleeSkill(.713f/1.1f,.8f/1.1f,.95f*SkillAttackPower,HitReaction.Hurt,400,0,.1f,null,0,.25f);
                if(SkillEvent("impact",.8f/1.1f+.01f)){var a=Area(id,"impact",player.x+player.facing*200*SkillUnit,player.y+25,.1f,0,.1f,0,7.55f,HitReaction.Hurt,900,0);a.stun=2;a.stunHits=2;a.stunChance=.5f;a.mpGain=2;SkillVisual(id,0,.45f,a.x,a.y,.9f,.8f);}
            }
            else if(id=="armageddon-blade")
            {
                if(SkillEvent("sword-appears",1.1f/1.3f))SkillVisual(id,0,.45f,player.x,player.y+45,1.4f,1.5f,true,0,45);
                if(SkillEvent("blade-buff",1.68f/1.3f)){ArmageddonRemaining=25;SkillVisual(id,1,25,player.x,player.y+35,1,1,true,0,35);}
                MeleeSkill(1.68f/1.3f,1.85f/1.3f,27.09f*SkillAttackPower,HitReaction.Hurt,740,0,.15f,null,0,30,.5f,.7f);
            }
            else if(id=="sandstorm")
            {
                if(SkillEvent("sandstorm",1.0333f))for(int i=0;i<2;i++)
                {
                    var floor=TraceSkillGround(skillCastGround,player.x);
                    var a=GroundArea(id,"sandstorm",floor,0,1.7f,.1f,1.4f,i==0?0:.08f,1.27f,HitReaction.Hurt,-1000,0);a.sand=true;a.accuracy=5;a.mpGain=.77f;
                    if(i==0){a.pullSpeed=2000*SkillUnit;a.pullRange=250*SkillUnit;a.visual=SkillVisual(id,0,1.7f,a.x,a.y+38,1.45f,1.55f,groundY:floor.Y);}
                }
            }
            else if(id=="gigantic-slash")
            {
                float[] times={.474f,3.374f,5.533f,7.333f},life={2.5f,2.55f,1.2f,.2f},rate={0,7.47f,4.98f,44.78f},gap={0,.33f,.15f,0},stop={.35f,.1f,.033f,.15f},mp={0,4.17f,2.78f,25};
                for(int i=0;i<4;i++)if(SkillEvent("giant-"+i,times[i]/1.15f))
                {
                    float x=player.x,y=player.y+32;if(i==0){var g=GeometryAt(skillPhaseTime,actionTiming);if(g!=null){x+=player.facing*g.weaponBaseX;y=player.y+g.weaponBaseY;}}
                    var a=Area(id,"gigantic-"+(i+1),x,y,life[i],0,i==0?.1f:life[i],gap[i],rate[i]*1.2f,i==3?HitReaction.Launch:HitReaction.Hurt,i==3?2000:i==0?-1000:-500,i==3?1200:i==0?0:-650);a.mpGain=mp[i];a.defenderStop=stop[i];
                    if(i==0){a.stun=3;a.stunHits=2;a.pullSpeed=250*SkillUnit;a.pullRange=2000*SkillUnit;}
                    SkillVisual(id,i==0?0:i==3?2:1,life[i],x,y,2.1f,2.1f);
                }
            }
            if(skillPhaseTime>=skillPhaseDuration||WantsSkillRecovery(input))FinishSkill();
        }
    }
}
