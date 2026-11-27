using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    public enum CombatState { Normal, Airborne, Hurt, Knockdown, GettingUp, Dead }
    public enum HitReaction { None, Hurt, Launch, Knockdown }
    public enum CommandContext { Ground, Dash, Air, DashAir }
    [Serializable] public sealed class Input
    {
        public float move;
        public bool drop;
        public bool dash, dashPressed, jump, attack, attackHeld, attackX, attackXHeld, attackXReleased, potion,awaken;
        public int skill = -1;
        public int skillHeldMask,skillReleasedMask,potionPressedMask;
    }
    [Serializable] public class Fighter
    {
        public float x=60, y=54, vx, vy, hp=500, maxHp=500;
        public int groundLine=-1;public float dropThrough;
        public int facing=1;
        public string clip="idle";
        public float clock, stun, invincible, downTime, recoveryTime, superArmor,hitPause;
        // Presentation pulse is independent of armor, hurt-state duration and
        // world position. A damaging armored/downed hit still needs feedback.
        public float impactFlash,impactFlashDuration;
        public CombatState state=CombatState.Normal;
        public bool knockdownOnLanding;
    }
    [Serializable] public sealed class Enemy : Fighter
    {
        [NonSerialized] internal PeitaAttackState peitaAttack;
        [NonSerialized] internal RegionActorState regionActor;
        [NonSerialized] internal EnemySpawn recoverySpawn;
        [NonSerialized] internal string stateLookupNpc, stateLookupState, stateLookupKey;
        [NonSerialized] internal float[] hurtSphereBuffer;
        [NonSerialized] internal bool dungeonBoss;
        [NonSerialized] internal bool usesNpcRoad;        public bool regionalAlly;        public string npcState="";public float npcTime,npcDecisionElapsed,npcVelocityX;public int npcVelocityIndex,npcVelocityMode;public float[] npcNextHits=new float[0];
        public bool[] npcAttachedEffectSets=new bool[0];
        internal string npcEffectPreviousState;internal float npcEffectPreviousTime,npcEffectPreviousX,npcEffectPreviousY,npcEffectSnapshotAt;
        public string sourceNPC="";public int encounterIndex,sourceKeyCode;public bool countsForClear=true,isGate,siegeMode;
        [NonSerialized] internal bool hasOriginalHitV45, originalHitBackV45, originalHitAirV45, originalHitWasDownV45;
        [NonSerialized] internal int originalHitCodeV45;
        public float nativeDepth,visualScale=1,spawnDelay;public bool aiEnabled=true;public string spawnAfterDeath="";
        public string kind="phoru";
        public float cooldown=1, telegraph, deathTime, spawnX, aggroRange=220;
        public bool struck, active=true, alerted, passive,dormant;
    }
    [Serializable] public sealed class EnemySpawn
    {
        public string sourceNPC="";public int encounterIndex,sourceKeyCode;public bool countsForClear=true,isGate,siegeMode;
        public string kind="phoru";
        // Legacy floor-height spawns attach to the local terrain. Higher Y
        // values remain explicit airborne spawn positions in world pixels.
        public float x=300, y=GameModel.GroundY, aggroRange=220, hp;
        public int groundLine=-1,facing=-1;public float nativeDepth,visualScale=1;public bool aiEnabled=true;public string spawnAfterDeath="";public float spawnDelay;
        public bool passive,dormant;
    }
    [Serializable] public sealed class RoomDefinition
    {
        public string id="room", title="Tree of El";
        public float width=1440, spawnX=60, exitX;
        public bool hasSpawnY;public float spawnY=54;public int entryEncounter;
        public EncounterDefinition[] encounters=new EncounterDefinition[0];
        public GroundNode[] groundNodes=new GroundNode[0];public GroundSurface[] surfaces=new GroundSurface[0];
        public GroundTeleport[] teleports;
        public int[] bungeeLineIds;
        public List<EnemySpawn> enemies=new List<EnemySpawn>();
    }
    [Serializable] public sealed class Effect
    {
        public string type;
        public float x,y;
        public int facing=1;
        public float age,lifetime=.4f;
        // Real fractional player damage is separate from the legacy integer amount.
        public bool hasFractionalAmountR901;
        public float fractionalAmountR901;
        public CombatNumberKind numberKind;public float recoveryAmount,numberDepth;public bool numberPercent;public int numberLane;
        public int amount;
        public int phase;
        public float scaleX=1,scaleY=1,offsetX,offsetY;        public float visualGroundY;
        public float peitaWidth,peitaHeight;public bool followPlayer;public float velocityX;public int hostileDamage;
    }
    [Serializable] public sealed class DamageWindow
    {
        public float start,end,damageScale=1,repeatInterval;
        public int rawReaction;
        public bool damageDefined=true;
        public string[] enabledAttackBoxes;
        public DamageWindow() { }
        public DamageWindow(float start,float end,float damageScale=1){this.start=start;this.end=end;this.damageScale=damageScale;}
    }
    [Serializable] public sealed class ClipTiming
    {
        public float duration=.46f, hitStart=.15f, hitEnd=.24f, travel=18, fps=30, hitStop=.045f;
        public List<float> rootCurve=new List<float>();
        public List<DamageWindow> hitWindows=new List<DamageWindow>();
        public CommandCalibration command;
        public bool loop;
        public List<AttackGeometry> geometry=new List<AttackGeometry>();
        public ClipTiming() { }
        public ClipTiming(float duration,float hitStart,float hitEnd,float travel,float hitStop=.045f)
        { this.duration=duration;this.hitStart=hitStart;this.hitEnd=hitEnd;this.travel=travel;this.hitStop=hitStop; }
    }
    [Serializable] public sealed class CommandDefinition
    {
        public string id,sequence,clip,chargeClip,description;
        public CommandContext context;
        public int damage;
        public float range,launchVelocity=280,chargeHoldDuration;
        public HitReaction reaction;
        public bool landBeforeStart;
        public bool chargeable { get { return !string.IsNullOrEmpty(chargeClip); } }
    }

    // Pure fixed-step simulation. +Y is up; button edges and X hold are distinct.
    public sealed partial class GameModel
    {
        public const int SkillSlotCount=8;
        public const float GroundY=54,PlayerDownDuration=.75f,EnemyDownDuration=.8f;
        public const float PlayerGetUpDuration=.45f,EnemyGetUpDuration=.45f;
        public const float GroundWalkSpeed=72f;
        public const float InputBufferDuration=.65f,JumpBufferDuration=.2f;
        public Fighter player=new Fighter();
        public readonly List<Enemy> enemies=new List<Enemy>();
        public readonly List<Effect> effects=new List<Effect>();
        public readonly float[] cooldowns=new float[SkillSlotCount];
        // Compatibility slots: the three legacy Z clips and original three skills.
        public readonly ClipTiming[] timings={new ClipTiming(.6f,5f/30,7f/30,25),new ClipTiming(.6f,6f/30,8f/30,34,.055f),new ClipTiming(22f/30,7f/30,10f/30,48,.075f),new ClipTiming(1.2f,10f/30,14f/30,42,.09f),new ClipTiming(40f/30,16f/30,22f/30,0,.065f),new ClipTiming(.6f,10,10,0)};
        public const float MaxMana=400f;
        public float mana=MaxMana,armor,elapsed,hitstop,roomBanner,shake,comboTimeout,landing;
        public const int PotionSlotCount=3;
        public readonly float[] potionCooldowns=new float[PotionSlotCount];
        // Legacy model drivers use the HP/MP recovery item.
        public float potionCooldown {get{return potionCooldowns[2];}}
        public int room,kills,hits,combo,stageKills;
        public bool gateOpen,victory,paused;
        public readonly List<string> roomKinds=new List<string>();
        private readonly List<RoomDefinition> rooms=new List<RoomDefinition>();
        private readonly List<SkillDefinition> skills=new List<SkillDefinition>();
        private readonly List<CommandDefinition> commands=new List<CommandDefinition>();
        private readonly Dictionary<string,ClipTiming> clipTimings=new Dictionary<string,ClipTiming>();
        private readonly Dictionary<string,float> skillCooldowns=new Dictionary<string,float>();
        private readonly List<string> tickingCooldowns=new List<string>(8);
        private readonly List<Effect> hostileEffectSnapshot=new List<Effect>(8);
        private readonly string[] equipped=new string[SkillSlotCount];
        private readonly Queue<Press> presses=new Queue<Press>();
        private static readonly Input EmptyInput=new Input();
        private CommandDefinition actionCommand;
        private SkillDefinition actionSkill;
        private ClipTiming actionTiming;
        private ClipTiming landingTiming;
        private string chain="",lastCommand="",airLandingClip="land";
        private CommandContext chainContext;
        private bool actionHit,charging,airborneDash;
        private enum FastFallPhase { None, Requested, Descending, Landing }
        private FastFallPhase fastFall;
        private const float DashRunSpeed=207.2f, SpringStepWindow=.15f;
        private int takeoffDirection, springDirection;
        private float dashJumpStarted, springContactTime;
        private float LocomotionSpeed(float move, bool dash, float dt)
        {
            if(springDirection!=0)
            {
                // Body integration subtracts ground friction once on every tick.
                // Scale the distance remaining after that loss, just as for a ground dash.
                float run=move*DashRunSpeed*SkillMoveMultiplier;
                float drag=RegionalAcceleration(player,560,false)*dt;
                return run+Math.Sign(run)*.2f*Math.Max(0,Math.Abs(run)-drag);
            }
            float speed=Grounded(player)?(dash?DashRunSpeed:GroundWalkSpeed):(airborneDash?DashAirSpeed:140);
            return move*speed*SkillMoveMultiplier;
        }
        private const float NormalFallSpeedLimit=560f, FastFallSpeedLimit=840f;
        private float FallSpeedLimit(bool isPlayer)
        {
            return isPlayer && fastFall==FastFallPhase.Descending ? FastFallSpeedLimit : NormalFallSpeedLimit;
        }
        private float previousRoot,jumpBuffered,chainGrace,commandDisplay,chargeTime,jumpTakeoff;
        private int nextHitWindow;
        private float nextRepeatTime=-1;
        private int skillBuffered=-1;
        private float skillBufferTime;
        private sealed class Press { public char button;public bool forward,held;public float expires;public CommandTransition transition;public string fromClip; }

        public IReadOnlyList<SkillDefinition> Skills { get { return skills.AsReadOnly(); } }
        public IReadOnlyList<CommandDefinition> CommandDefinitions { get { return commands.AsReadOnly(); } }
        public RoomDefinition CurrentRoom { get { return rooms[room]; } }
        public float RoomWidth { get { return CurrentRoom.width; } }
        public float ExitX { get { return CurrentRoom.exitX; } }
        public int RoomCount { get { return rooms.Count; } }
        public string CurrentCommand { get { return actionTiming!=null||commandDisplay>0?lastCommand:""; } }
        public bool IsCharging { get { return charging; } }
        public float ChargeAmount { get { return Clamp(chargeTime/(actionCommand!=null&&actionCommand.chargeHoldDuration>0?actionCommand.chargeHoldDuration:1),0,1); } }
        public string[] CommandGuide { get { return new[] {
            "ZZZZ  /  knockdown", "ZZZX  /  launcher", "ZZZ > Z  /  forward strike",
            "XXZ  /  launcher", "XXXXX  /  heavy finisher",
            ">> ZZZ  /  dash combo", ">> ZZX  /  dash launcher", ">> ZXX  /  heavy follow-up",
            ">> XXX  /  dash finisher", "^ Z  /  aerial slash", "^ X  /  aerial strike",
            ">>^ ZZZ  /  aerial combo", ">>^ XXX  /  aerial finisher", ">>^ XXZ  /  land, then Z",
            ">> Z hold X / release X", ">> X hold X / release X" }; } }

        public GameModel()
        {
            CreateSkills();CreateCommands();
            var defaults=new[]{"mega-slash","flame-geyser","iron-body","armor-break","sonic-blade","rolling-smash","windmill","kick"};Array.Copy(defaults,equipped,SkillSlotCount);
            SetRoomRoster(new[]{new[]{"phoru","phoru","phoru"},new[]{"phoru","phoru","phoru"},new[]{"bender"}});
        }
        private static float Clamp(float n,float min,float max){return Math.Max(min,Math.Min(max,n));}
        private static float Smooth(float n){n=Clamp(n,0,1);return n*n*(3-2*n);}
        private static bool Finite(float n){return !float.IsNaN(n)&&!float.IsInfinity(n);}
        private static bool IsBoss(string k){return IsPeitaBoss(k)||k=="bender"||k=="benders"||k=="boss"||k=="white-giant"||k=="william"||k=="ancient-phoru";}
        public bool Busy(){return AwakeningCasting||actionTiming!=null||player.stun>0||PlainOriginalFxStunRemainingR1304>0||PeitaPlayerStunned||player.knockdownOnLanding||player.state==CombatState.Knockdown||player.state==CombatState.GettingUp||player.hp<=0||landing>0;}

        public void SetRooms(IEnumerable<RoomDefinition> definitions)
        {
            if(definitions==null)throw new ArgumentNullException("definitions");
            var validated=new List<RoomDefinition>();
            foreach(var s in definitions)
            {
                if(s==null||!Finite(s.width)||s.width<480||!Finite(s.spawnX)||!Finite(s.exitX))throw new ArgumentException("Rooms need finite coordinates and width >= 480.");
                bool sectioned=s.encounters!=null&&s.encounters.Length>0;
                if(s.hasSpawnY&&!Finite(s.spawnY))throw new ArgumentException("Invalid authored player spawn height.");
                var r=new RoomDefinition{id=s.id,title=s.title,width=s.width,spawnX=sectioned?s.spawnX:Clamp(s.spawnX,22,s.width-80),exitX=sectioned?s.exitX:s.exitX>0?Clamp(s.exitX,80,s.width-26):s.width-56,groundNodes=CopyGroundNodes(s),surfaces=s.surfaces==null?new GroundSurface[0]:Array.ConvertAll(s.surfaces,q=>new GroundSurface{id=q.id,x1=q.x1,y1=q.y1,x2=q.x2,y2=q.y2,platform=q.platform,water=q.water,hamelTrigger=q.hamelTrigger,initiallyDisabled=q.initiallyDisabled,bungeeTargetPlusOne=q.bungeeTargetPlusOne,npcOnly=q.npcOnly}),hasSpawnY=s.hasSpawnY,spawnY=s.spawnY,entryEncounter=s.entryEncounter,encounters=CopyEncounters(s)};
                r.teleports=CopyGroundTeleports(s.teleports,r.surfaces);BindHamelSourceGroundSpeeds(r);
                r.bungeeLineIds=s.bungeeLineIds==null?null:(int[])s.bungeeLineIds.Clone();
                if(!sectioned&&r.exitX<=r.spawnX)throw new ArgumentException("Room exit must be after spawn.");
                if(s.enemies!=null)foreach(var e in s.enemies)
                {
                    if(e==null||string.IsNullOrEmpty(e.kind)||!Finite(e.x)||!Finite(e.y)||!Finite(e.aggroRange)||!Finite(e.hp))throw new ArgumentException("Invalid enemy spawn.");
                    float x=Clamp(e.x,22,r.width-26),ground=GroundAt(r,x);
                    r.enemies.Add(new EnemySpawn{kind=e.kind,x=x,y=r.surfaces.Length>0?e.y:e.y<=GroundY+.01f?ground:Math.Max(ground,e.y),aggroRange=Math.Max(0,e.aggroRange),hp=e.hp,passive=e.passive,dormant=e.dormant,groundLine=e.groundLine,facing=e.facing,nativeDepth=e.nativeDepth,visualScale=e.visualScale,aiEnabled=e.aiEnabled,spawnAfterDeath=e.spawnAfterDeath,spawnDelay=e.spawnDelay,sourceNPC=e.sourceNPC,sourceKeyCode=e.sourceKeyCode,encounterIndex=e.encounterIndex,countsForClear=e.countsForClear,isGate=e.isGate,siegeMode=e.siegeMode});
                }
                validated.Add(r);
            }
            if(validated.Count==0)throw new ArgumentException("A dungeon needs at least one room.");
            ValidateEncounterGraph(validated);
            alteraDeferredCannonDefinitions.Clear();
            rooms.Clear();rooms.AddRange(validated);roomKinds.Clear();foreach(var r in rooms)roomKinds.Add(r.enemies.Count>0?r.enemies[0].kind:"");Reset();
        }
        public void SetRoomRoster(IEnumerable<IEnumerable<string>> roster)
        {
            var result=new List<RoomDefinition>();
            if(roster!=null)foreach(var source in roster)
            {
                var kinds=source==null?new List<string>():new List<string>(source);
                var r=new RoomDefinition{id="room-"+(result.Count+1),width=Math.Max(960,350+kinds.Count*150)};
                for(int i=0;i<kinds.Count;i++)r.enemies.Add(new EnemySpawn{kind=kinds[i],x=300+i*150,aggroRange=IsBoss(kinds[i])?280:kinds[i]=="chicken"?160:220});
                result.Add(r);
            }
            if(result.Count==0)result.Add(new RoomDefinition());SetRooms(result);
        }
        public SkillDefinition GetEquippedSkill(int slot){return slot<0||slot>=SkillSlotCount?null:FindSkill(equipped[slot]);}
        private SkillDefinition FindSkill(string id){foreach(var s in skills)if(s.id==id)return s;return null;}
        public bool EquipSkill(int slot,string id)
        {
            if(slot<0||slot>=SkillSlotCount||FindSkill(id)==null)return false;
            int other=Array.IndexOf(equipped,id);if(other>=0&&other!=slot)equipped[other]=equipped[slot];equipped[slot]=id;SyncCooldowns();return true;
        }
        public void SetClipTiming(string clip,ClipTiming timing)
        {
            if(string.IsNullOrEmpty(clip)||timing==null||!Finite(timing.duration)||timing.duration<=0||!Finite(timing.fps)||timing.fps<=0)throw new ArgumentException("Invalid clip timing.");
            clipTimings[clip]=timing;
        }
        public bool UsePotion(){return UsePotion(2);}
        public bool UsePotion(int slot)
        {
            if(slot<0||slot>=PotionSlotCount||paused||victory||player.hp<=0||potionCooldowns[slot]>0)return false;
            float beforeHp=player.hp,beforeMana=mana;
            if(slot==2){player.hp=player.maxHp;mana=MaxMana;potionCooldowns[slot]=60;}
            else{mana=Math.Min(MaxMana,mana+(slot==0?50:100));potionCooldowns[slot]=30;}
            if(player.hp>beforeHp)SpawnRecoveryNumber(true,slot==2?100:player.hp-beforeHp,slot==2);
            if(mana>beforeMana)SpawnRecoveryNumber(false,slot==2?100:mana-beforeMana,slot==2);
            SpawnEffect("heal",player.x,player.y+25);return true;
        }
        public void ClearBufferedInput()
        {
            if(launchFollowup==LaunchFollowup.Jump||launchFollowup==LaunchFollowup.Dive)ClearLaunchFollowup();
            launchGesture=LaunchGesture.None;standBuffered=false;presses.Clear();jumpBuffered=skillBufferTime=0;skillBuffered=-1;            bufferedDashDirection=0;bufferedDashTime=0;
            // Releasing a held charge preserves the current attack. It does not
            // allow an old key edge to create another action after menu/focus loss.
            if(charging&&actionCommand!=null){charging=false;player.clip=actionCommand.clip;player.clock=0;previousRoot=0;commandPreviousClock=0;if(actionTiming.command!=null)ApplyCommandStart(actionTiming.command,false);}
        }
        public void Reset()
        {
            ResetDungeonScore(); ResetRecoveryDropRun();
            ResetNpcEffects();
            npcActionEvents.Clear();npcRandom=0x7a2189u;
            ResetSkillState();ResetAwakening();combatEvents.Clear();
            player=new Fighter();mana=MaxMana;armor=elapsed=hitstop=shake=comboTimeout=landing=0;Array.Clear(potionCooldowns,0,potionCooldowns.Length);kills=hits=combo=stageKills=0;victory=paused=false;
            skillCooldowns.Clear();Array.Clear(cooldowns,0,cooldowns.Length);effects.Clear();pendingEncounterEntry=-1;encounterRandom=0x9173a5u;EnterRoom(0);
        }
        private void CancelAction(bool clearInput)
        {
            ClearLaunchFollowup();
            CancelSkillAction();
            activeHitBoxes.Clear();
            actionTiming=null;actionCommand=null;actionSkill=null;actionHit=charging=false;previousRoot=chargeTime=0;nextHitWindow=0;nextRepeatTime=-1;
            if(clearInput){fastFall=FastFallPhase.None;springDirection=takeoffDirection=0;presses.Clear();jumpBuffered=skillBufferTime=0;skillBuffered=-1;bufferedDashDirection=0;bufferedDashTime=0;chain="";chainGrace=0;}
        }
        private void EnterRoom(int next)
        {
            arrivalTeleportLine=-1;
            ResetAlteraDeferredCannonState();
            ClearPeitaSpells();ClearRegionSpells();regionInputUntil=0;ClearNpcEffectWorld();if(cargoSourceV21!=null)ClearCargoSourceRoomV21();BeginFactoryRavenIntroRoomV21();
            nativeNavigationRoom=nativeNavigationDestination=-1;
            ClearRoomSkillEffects();
            room=next;gateOpen=false;stageKills=0;roomBanner=2.6f;player.x=CurrentRoom.spawnX;player.y=CurrentRoom.hasSpawnY?CurrentRoom.spawnY:GroundAt(CurrentRoom,player.x);player.vx=player.vy=0;
            landingTiming=null;
            player.groundLine=-1;player.dropThrough=0;player.state=CombatState.Normal;player.knockdownOnLanding=false;player.clip="idle";player.clock=0;player.invincible=1;
            player.stun=player.downTime=player.recoveryTime=player.superArmor=0;CancelAction(true);hitstop=landing=commandDisplay=jumpTakeoff=0;airborneDash=false;airLandingClip="land";enemies.Clear();
            foreach(var s in CurrentRoom.enemies)
            {
                float hp=s.hp>0?s.hp:IsBoss(s.kind)?(s.kind=="white-giant"?780:480):s.kind=="chicken"?75:110;
                hp*=MouseHouseHealthScale(s.sourceNPC);
                enemies.Add(new Enemy{recoverySpawn=s,usesNpcRoad=Surface(s.groundLine)?.npcOnly==true,kind=s.kind,x=s.x,spawnX=s.x,y=s.y,facing=s.facing,groundLine=s.groundLine,nativeDepth=s.nativeDepth,visualScale=s.visualScale,aiEnabled=s.aiEnabled,spawnAfterDeath=s.spawnAfterDeath,spawnDelay=s.spawnDelay,hp=hp,maxHp=hp,active=true,aggroRange=s.aggroRange,passive=s.passive,dormant=s.dormant,cooldown=1,state=s.y>BelowNpcOwnFloorV21(s.sourceNPC,s.x,s.y,false)+.01f?CombatState.Airborne:CombatState.Normal,sourceNPC=s.sourceNPC,sourceKeyCode=s.sourceKeyCode,encounterIndex=s.encounterIndex,countsForClear=s.countsForClear,isGate=s.isGate,siegeMode=s.siegeMode});
            }
            BeginEncounter(pendingEncounterEntry>=0?pendingEncounterEntry:CurrentRoom.entryEncounter);pendingEncounterEntry=-1;
            BeginAlteraAirshipIntroRoomV18();
            BeginDragonNestRoomV20();BeginMine2MechanismsRoomV21();BeginCargoSourceRoomV21();BeginCoreHardRoomV21();PrepareBelowSteamController();
            velderAlarmRoom=null;ResetHamelWater(); ResetHamelTriggers(); peitaStunUntil=0;peitaPoisonUntil=0;ResetRecoveryDrops();ResetNpcFeedbackMechanicsV25();BeginPlainPairRoomR1258();
            // The configured source owner initializes its original births on their first ordinary Step.
            bool sourceOwnedBirthsR1111=coreHardV21!=null&&coreHardEntryActivationV21!=null&&CurrentRoom.id=="Dungeon_Altera_Core_Hard-"+room||factoryPopulationPolicyV21!=null&&factoryEventBindingV21!=null&&CurrentRoom.id==FactoryRouteV542+"-"+room;
            if(!sourceOwnedBirthsR1111)foreach(var enemy in enemies)InitializeNpcBirthStateV26(enemy);
            BindSwampIntroV39();
            foreach(var enemy in enemies)ApplyDungeonBossHealth(enemy);
        }
        private void SpawnEffect(string type,float x,float y,int facing=1,int amount=0)
        {effects.Add(new Effect{type=type,x=x,y=y,facing=facing,amount=amount,lifetime=type=="number"?.7f:.48f});}
        private void QueuePress(char button,Input input)
        {
            if(landing>0&&landingTiming!=null&&landingTiming.command!=null)
            {                float window=landingTiming.command.attackCancel;                if(window<0||player.clock+RecoveryInputLead<window)return;                presses.Clear();                presses.Enqueue(NewAttackPress(button,input,RecoveryInputLead));                return;            }
            if(actionTiming!=null&&actionTiming.command!=null){QueueCalibratedPress(button,input);return;}            if(actionSkill!=null){                presses.Clear();                presses.Enqueue(NewAttackPress(button,input,InputBufferDuration));                return;            }
            float life=actionTiming==null?InputBufferDuration:Math.Max(InputBufferDuration,actionTiming.duration-player.clock+.25f);
            if(charging)life=Math.Max(life,actionCommand.chargeHoldDuration-chargeTime+actionTiming.duration+.25f);
            if(presses.Count<4)presses.Enqueue(NewAttackPress(button,input,life));
        }
        private CommandContext Context(Input input)
        {bool air=!Grounded(player),dash=air?airborneDash:input.dash&&Math.Abs(input.move)>.01f;return air?(dash?CommandContext.DashAir:CommandContext.Air):(dash?CommandContext.Dash:CommandContext.Ground);}        private Press NewAttackPress(char button,Input input,float life)        {            return new Press{button=button,forward=input.move*player.facing>.1f,                held=button=='X'&&input.attackXHeld,expires=elapsed+life};        }
        private CommandDefinition FindCommand(CommandContext context,string sequence)
        {foreach(var c in commands)if(c.context==context&&c.sequence==sequence)return c;return null;}
        private string DisplayCommand(CommandContext context,string sequence)
        {return(context==CommandContext.Ground?"":context==CommandContext.Dash?">> ":context==CommandContext.Air?"^ ":">>^ ")+sequence.Replace(">","→");}
        private ClipTiming Timing(string clip)
        {
            ClipTiming t;if(clipTimings.TryGetValue(clip,out t))return t;
            string[] ids={"combo-z1","combo-z2","combo-z3","mega-slash","flame-geyser","iron-body"};
            for(int i=0;i<ids.Length;i++)if(ids[i]==clip)return timings[i];return new ClipTiming();
        }
        private void BeginCommand(CommandDefinition command,Press press)
        {
            ClearLaunchFollowup();
            actionCommand=command;actionSkill=null;actionTiming=Timing(command.clip);actionHit=false;previousRoot=chargeTime=0;nextHitWindow=0;nextRepeatTime=-1;charging=command.chargeable&&press.held;
            chain=command.sequence;chainContext=command.context;chainGrace=0;player.clip=charging?command.chargeClip:command.clip;player.clock=0;
            if(command.context==CommandContext.Air||command.context==CommandContext.DashAir)
                airLandingClip=command.clip=="dash-air-z3"?"dash-air-z-land":command.clip=="dash-air-x2"?"dash-air-x-land":"land";
            lastCommand=DisplayCommand(command.context,command.sequence);commandDisplay=1.3f;
            BeginCalibratedCommand(press);
        }
        private bool StartBufferedCommand(Input input,bool chaining)
        {
            if(chaining&&actionTiming!=null&&actionTiming.command!=null)return StartCalibratedContinuation();
            if(presses.Count==0)return false;var p=presses.Peek();CommandDefinition next=null;
            if(chain.Length>0&&(chaining||chainGrace>0))
            {
                if(p.button=='Z'&&p.forward)next=FindCommand(chainContext,chain+">Z");if(next==null)next=FindCommand(chainContext,chain+p.button);
                if(next!=null&&next.landBeforeStart&&!Grounded(player))return false;
                if(next!=null&&(chainContext==CommandContext.Air||chainContext==CommandContext.DashAir)&&Grounded(player)&&!next.landBeforeStart)next=null;
            }
            if(next==null){if(chaining)return false;next=FindCommand(Context(input),p.button.ToString());}
            presses.Dequeue();if(next==null)return false;BeginCommand(next,p);return true;
        }
        private void SyncCooldowns(){for(int i=0;i<SkillSlotCount;i++){float t;cooldowns[i]=skillCooldowns.TryGetValue(equipped[i],out t)?t:0;}}

        // Scripted hazards and tests use the same damage path as normal attacks.
        public int ApplyHitToEnemy(Enemy enemy,int damage,HitReaction reaction=HitReaction.Hurt,float launchVelocity=280){return ApplyHitToEnemyFromXR1417(enemy,damage,reaction,launchVelocity,player.x);}
        private const float DefaultHitMpGain=7f;        private int ApplyHitToEnemyFromXR1417(Enemy enemy,int damage,HitReaction reaction,float launchVelocity,float attackerX,int rawReaction=0)
        {return ApplyHitToEnemyWithMp(enemy,damage,reaction,launchVelocity,attackerX,rawReaction,DefaultHitMpGain);}        private int ApplyHitToEnemyWithMp(Enemy enemy,int damage,HitReaction reaction,float launchVelocity,float attackerX,int rawReaction,float mpGain)        {
            if(IsPeitaCrossbow(enemy))return TriggerPeitaCrossbow(enemy,damage);
            if(IsRegionMirror(enemy))return StrikeRegionMirror(enemy,damage,reaction,rawReaction);
            if(enemy!=null&&enemy.kind=="ice_pillar_trap")return StrikeHamelIcePillar(enemy,damage);
            if(PeitaTowerDragon(enemy))reaction=HitReaction.None;            if(enemy!=null&&enemy.hp>0&&IsRecoveryContainer(enemy))PrepareRecoveryContainer(enemy);
            if(enemy==null||enemy.hp<=0||!enemy.active||enemy.dormant||(enemy.passive&&!CoreHardBuffDeviceDamageableR1417(enemy)&&!IsRegionCannon(enemy))||damage<=0||EnemyInitialHurtboxHiddenV25(enemy)||EnemyHasNpcInvincibility(enemy))return 0;enemy.alerted=true;if(!enemy.aiEnabled&&!enemy.isGate&&!IsRegionCannon(enemy)&&!FactoryOwnsBirthAiV58(enemy)&&!BelowVisibilityPreservePartAiV21(enemy)&&!CoreHardPreserveSourceAiV21(enemy)&&!CoreHardStopIntroPreserveAiV21(enemy)&&!IsPlainPairManagedMachineR1258(enemy)&&!PreserveOriginalDisabledTreeCombatAiV25(enemy)&&!PreservePlainShockStickAiR1477(enemy))enemy.aiEnabled=true;
            if(enemy.isGate)reaction=HitReaction.None;reaction=Mine2RailReactionV21(enemy,reaction);
            damage=ModifyEnemyDamage(enemy,ApplyAwakeningDamage(damage),out bool critical);damage=CoreHardIncomingEnemyDamageR1417(enemy,damage,ref reaction,attackerX);
            bool shieldContact=false;
            damage=GuardPeitaShield(enemy,damage,attackerX,ref reaction,ref shieldContact);
            damage=GuardDurahan(enemy,damage,attackerX,ref reaction,ref shieldContact);
            damage=GuardRegionActor(enemy,damage,attackerX,ref reaction,ref shieldContact);            damage=GuardVelderSwitch(enemy,damage,ref reaction);            string coreHardHitPriorStateV39=enemy.npcState;CoreHardHitWitnessR642 coreHardHitWitnessR642=CaptureCoreHardHitDisplayWitnessR642(enemy,coreHardHitPriorStateV39);int dealt=ApplyHit(enemy,damage,reaction,attackerX,launchVelocity,false,rawReaction,critical);
            if(dealt>0){RecordCannonWreckImpactV39(enemy);RecordCoreHardHitDisplayPendingV39(enemy,dealt,coreHardHitPriorStateV39);ContinueCoreHardHitDisplayPendingR642(enemy,dealt,coreHardHitWitnessR642);NoteCoreHardHitPacketR642(enemy);EnemyImpactSerial++;LastImpactHeavy=reaction==HitReaction.Launch||reaction==HitReaction.Knockdown;RecordEnemyImpact(enemy,reaction,shieldContact,rawReaction);hits++;combo++;runMaxCombo=Math.Max(runMaxCombo,combo);comboTimeout=2.2f;mana=Math.Min(MaxMana,mana+mpGain);AddAwakeningCharge(1);if(enemy.hp<=0&&!enemy.isGate&&!IsRecoveryContainer(enemy)){kills++;stageKills++;}}            if(dealt>0)ApplyVelderSquadImpact(enemy,reaction,rawReaction);
            if(dealt>0&&enemy.hp<=0)TrySpawnRecoveryDrop(enemy);            if(dealt>0&&enemy.hp>0)ReactDurahanToDamage(enemy,reaction);
            return dealt;
        }
        public int ApplyHitToPlayer(int damage,float fromX,HitReaction reaction=HitReaction.Hurt,float launchVelocity=270)
        {return HitPlayer(damage,fromX,reaction,launchVelocity,false);}
        private int HitPlayer(int damage,float fromX,HitReaction reaction,float launchVelocity,bool critical)
        {if(TryCounterPlayerHit(damage,fromX,reaction))return 0;int dealt=ApplyHit(player,damage,reaction,fromX,launchVelocity,true,0,critical);if(dealt>0){regionLastPlayerDamage=dealt;PlayerImpactSerial++;OnCoreHardPlayerDamageV21();OnPlainOriginalFxPlayerDamageR1304();LastPlayerGuard=PlayerHasSuperArmor;AddAwakeningCharge(2);}return dealt;}
        private int ApplyHit(Fighter f,int damage,HitReaction reaction,float fromX,float launchVelocity,bool isPlayer,int rawReaction=0,bool critical=false)
        {
            if(damage<=0||f.hp<=0||f.invincible>0||isPlayer&&AwakeningCasting)return 0;if(!isPlayer)reaction=Mine2RailReactionV21(f as Enemy,reaction);if(!isPlayer)ReleaseInitialNpcPoseForHitV25(f as Enemy);bool fixedPosition=!isPlayer&&EnemyPositionFixedV25(f as Enemy);if(fixedPosition&&reaction!=HitReaction.None)reaction=HitReaction.Hurt;bool down=f.state==CombatState.Knockdown;
            int actual=down?Math.Max(1,(int)Math.Ceiling(damage*.5f)):damage;bool armored=f.superArmor>0||(!isPlayer&&EnemyHasSuperArmor(f as Enemy))||(!isPlayer&&CoreHardShieldGuardingR1417(f as Enemy,fromX))||(isPlayer&&(armor>0||CommandSuperArmorActive));
            float beforeHp=f.hp;if(isPlayer)actual=ApplyPlayerDefense(actual);actual=Math.Min(actual,(int)Math.Ceiling(f.hp));ApplyCombatHealth(f,Math.Max(0,f.hp-actual));if(actual>0)RegionStatusHit(f);bool plainPairManagedPacketR1258=!isPlayer&&PlainPairAfterAttackDamageR1258(f as Enemy,actual,!down&&!armored&&reaction!=HitReaction.None);
            if(!isPlayer)OnDragonDamageFrameV20(f as Enemy);
            int direction=f.x>=fromX?1:-1;
            f.impactFlashDuration=reaction==HitReaction.Launch||reaction==HitReaction.Knockdown?.10f:.075f;f.impactFlash=f.impactFlashDuration;
            SpawnResourceDamage(f,beforeHp-f.hp,critical);SpawnEffect(armored?"guard":"hit",f.x,f.y+25,direction);if(plainPairManagedPacketR1258)return actual;
            if(f is Enemy originalEnemy && (f.hp<=0 || (!down&&!armored&&reaction!=HitReaction.None))){
                originalEnemy.hasOriginalHitV45=true;originalEnemy.originalHitBackV45=(fromX-f.x)*f.facing<0;
                originalEnemy.originalHitAirV45=!Grounded(f);originalEnemy.originalHitCodeV45=rawReaction;
                originalEnemy.originalHitWasDownV45=reaction==HitReaction.Knockdown;
            }
            if(f.hp<=0){f.state=CombatState.Dead;f.clip="death";f.clock=0;f.stun=0;if(isPlayer)CancelAction(true);else{RememberPlainLethalPoseV35(f as Enemy,fromX);fairyRearReactionsV34.Remove(f as Enemy);InterruptNpcAction(f as Enemy);}return actual;}
            // Ground hits change HP only: no resetting downTime or pose clock.
            if(down||armored||reaction==HitReaction.None)return actual;
            if(isPlayer){CancelAction(true);landing=0;f.invincible=.22f;}
            var e=f as Enemy;if(e!=null){RememberFairyIncomingV34(e,fromX,reaction);RememberPlainIceRecoveryHitV35(e,fromX);e.telegraph=0;e.struck=false;InterruptNpcAction(e);if(fixedPosition)RememberFixedNpcReactionV25(e);}
            f.clock=0;
            if(reaction==HitReaction.Knockdown&&Grounded(f))BeginKnockdown(f,isPlayer,fromX);
            else if(!fixedPosition&&(reaction==HitReaction.Launch||!Grounded(f)))
            {f.state=CombatState.Airborne;f.knockdownOnLanding=isPlayer||Remaining3AllowsKnockdownV28(f as Enemy);f.vy=Math.Max(f.vy,reaction==HitReaction.Launch?launchVelocity:125);f.vx=direction*34;f.stun=.18f;f.clip="launched";}
            else{f.state=CombatState.Hurt;f.stun=FairyReactionScopeV34(f)?FairySmallDamageSecondsV34:.28f;f.clip="hurt";if(fixedPosition)f.vx=f.vy=0;if(e!=null)NotifyNpcDamageEntry(e,fromX);}return actual;
        }
        private void BeginKnockdown(Fighter f,bool isPlayer,float? attackerX=null)
        {
            // Gauge-triggered down can arrive while a native NPC has no real floor.
            // Keep falling with the existing motion; physical landing owns down entry.
            if(!isPlayer&&NativeTerrain&&!NativeGrounded(f)){f.state=CombatState.Airborne;f.knockdownOnLanding=true;f.clip=f.vy>0?"launched":"fall";return;}
            string iceDown=PlainIceDownEntryV35(f,isPlayer);
            string sheriffDown=CoreSheriffDownEntryV35(f,isPlayer,attackerX);
            f.state=CombatState.Knockdown;f.downTime=isPlayer?PlayerDownDuration:FairyReactionScopeV34(f)?FairyDownSecondsV34:CargoBeeDownDurationR1484(f,EnemyDownDuration);f.recoveryTime=f.stun=f.vx=f.vy=0;f.y=NativeTerrain?NativeFloor(f):GroundAt(f.x);
            f.knockdownOnLanding=false;f.clip="knockdown";f.clock=0;
            if(isPlayer){f.invincible=Math.Max(f.invincible,f.downTime+GetUpDuration()+.7f);CancelAction(true);}
            else if(!slammedVictims.Contains(f as Enemy)&&!FairyReactionScopeV34(f))f.superArmor=Math.Max(f.superArmor,f.downTime+EnemyGetUpDuration+.55f);
            var e=f as Enemy;if(e!=null){e.telegraph=0;e.struck=false;if(iceDown!=null)BeginPlainIceDownV35(e,iceDown);else if(sheriffDown!=null)BeginCoreSheriffDownV35(e,sheriffDown);}
        }
        private void TickState(Fighter f,float dt,bool isPlayer)
        {
            f.impactFlash=Math.Max(0,f.impactFlash-dt);
            f.invincible=Math.Max(0,f.invincible-dt);f.superArmor=Math.Max(0,f.superArmor-dt);if(f.hp<=0){f.state=CombatState.Dead;return;}
            if(f.state==CombatState.Knockdown)
            {
                if(!isPlayer&&(PlainIceRecoveryDownActiveV35(f as Enemy)||CoreSheriffPhysicalDownV35(f as Enemy)))return;
                f.downTime=Math.Max(0,f.downTime-CargoBeeReactionStepR1484(f,dt));if(f.downTime<=0){f.state=CombatState.GettingUp;f.recoveryTime=isPlayer?GetUpDuration():FairyGetUpSecondsV34(f);f.clip="getup";f.clock=0;if(isPlayer)f.invincible=f.recoveryTime+.7f;else if(!FairyReactionScopeV34(f))f.superArmor=f.recoveryTime+.55f;}return;
            }
            if(f.state==CombatState.GettingUp)
            {f.recoveryTime=Math.Max(0,f.recoveryTime-CargoBeeReactionStepR1484(f,dt));if(f.recoveryTime<=0){fairyRearReactionsV34.Remove(f as Enemy);f.state=CombatState.Normal;f.clip="idle";f.clock=0;}return;}
            if(f.hitPause<=0)f.stun=Math.Max(0,f.stun-dt);if(f.state==CombatState.Hurt&&f.stun<=0){f.state=Grounded(f)?CombatState.Normal:CombatState.Airborne;f.clip=Grounded(f)?"idle":"jump";f.clock=0;}
        }
        private void IntegrateBody(Fighter f,float dt,bool isPlayer)
        {
            if(!isPlayer&&HoldNpcFixedPhysicsV25(f as Enemy))return;if(NativeTerrain){IntegrateNativeBody(f,dt,isPlayer);return;}
            if(f.state==CombatState.Knockdown||f.state==CombatState.GettingUp)return;
            if(Grounded(f))
            {
                f.y=GroundAt(f.x);
                // A descending integration may land inside the .01 grounding
                // epsilon without crossing the exact floor. Resolve the same
                // contact transition instead of stranding an air command there.
                if(!f.knockdownOnLanding)
                {f.vy=0;if(f.state==CombatState.Airborne&&f.hp>0){f.state=CombatState.Normal;if(!isPlayer||springDirection==0)SpawnEffect("dust",f.x,f.y);if(isPlayer)LandPlayer();}}
            }
            float oldX=f.x,oldY=f.y;bool wasGrounded=Grounded(f);f.vx=Math.Sign(f.vx)*Math.Max(0,Math.Abs(f.vx)-RegionalAcceleration(f,560,false)*dt);if(isPlayer)MovePlayerAlongGround(f.x+f.vx*dt);else MoveAlongGround(f,f.x+f.vx*dt);
            if(Grounded(f)&&f.knockdownOnLanding&&f.state==CombatState.Airborne){BeginKnockdown(f,isPlayer);return;}
            if(!Grounded(f)&&!(isPlayer&&SkillSuppressGravity))
            {
                float gravity=isPlayer&&airborneDash&&actionTiming==null?1120*2.2f:RegionalAcceleration(f,1120,true);
                f.vy=Math.Max(-FallSpeedLimit(isPlayer),f.vy-gravity*dt);f.y+=f.vy*dt;
                if(f.knockdownOnLanding&&f.hp>0)f.clip=f.vy>0?"launched":"fall";
                if(f.y<=GroundAt(f.x))
                {
                    f.y=GroundAt(f.x);f.vy=0;if(!isPlayer||springDirection==0)SpawnEffect("dust",f.x,f.y);
                    if(f.hp>0&&f.knockdownOnLanding)BeginKnockdown(f,isPlayer);
                    else if(f.hp>0){f.state=CombatState.Normal;if(isPlayer)LandPlayer();}
                }
            }
            if(isPlayer)BlockPlayerBodies(oldX,oldY);
            f.x=ConstrainEncounterX(f,Clamp(f.x,22,RoomWidth-26));
            if(wasGrounded)f.y=GroundAt(f.x);
            if(isPlayer&&actionTiming!=null&&actionTiming.command!=null){if(commandPassiveX>=0)f.vx=commandPassiveX*f.facing;if(commandPassiveY>=0)f.vy=commandPassiveY;}
        }
        private float GetUpDuration(){ClipTiming t;return clipTimings.TryGetValue("getup",out t)?t.duration:PlayerGetUpDuration;}
        private void LandPlayer()
        {
            airborneDash=false;
            fastFall=fastFall==FastFallPhase.Descending && actionTiming==null && !AwakeningCasting
                ? FastFallPhase.Landing : FastFallPhase.None;
            if(CatchSwingNeedsHitWindow()&&slammedVictims.Count==0)return;
            bool airCommand=actionCommand!=null&&(actionCommand.context==CommandContext.Air||actionCommand.context==CommandContext.DashAir);
            bool airChain=chain.Length>0&&(chainContext==CommandContext.Air||chainContext==CommandContext.DashAir);
            // The second dash-air X resolves its buffered finisher on contact:
            // X starts the grounded launcher, Z starts the regular grounded Z1.
            if(airCommand&&actionCommand.clip=="dash-air-x2"&&StartBufferedCommand(EmptyInput,true))return;
            if(airCommand||airChain){
                keepSlamWindow=launchFollowup==LaunchFollowup.Strike&&slammedVictims.Count>0;
                CancelAction(false);presses.Clear();chain="";chainGrace=0;
                if(keepSlamWindow){keepSlamWindow=false;launchFollowup=LaunchFollowup.Landing;launchInputRemaining=.30f;}
            }
            if(actionTiming==null)
            {
                ClipTiming authored;landingTiming=clipTimings.TryGetValue(airLandingClip,out authored)?authored:null;
                landing=landingTiming!=null?(landingTiming.command!=null?landingTiming.command.endAt:landingTiming.duration):.08f;
                if(landingTiming!=null&&landingTiming.command!=null)
                {var motion=landingTiming.command;if(motion.hasStartX)player.vx=motion.startX*player.facing;if(motion.passiveX>=0)player.vx=motion.passiveX*player.facing;if(motion.hasStartY)player.vy=motion.startY;if(motion.passiveY>=0)player.vy=motion.passiveY;}
                if(springDirection!=0 && fastFall==FastFallPhase.Landing)
                    springContactTime=elapsed;
                else {player.clip=airLandingClip;player.clock=0;}
                airLandingClip="land";
                RecordCombat("land",-1,null);
                // Ending a jump also ends its input context. A later Z/X is a
                // ground opener, never an air continuation played on the floor.
                chain="";chainGrace=0;
            }
        }
        private void HitNearby(int damage,float range,HitReaction reaction,bool around,float launch,float stop,int rawReaction=0)
        {
            bool connected=false;foreach(var e in enemies){float dx=e.x-player.x;if(Math.Abs(dx)>range||(!around&&dx*player.facing< -16)||Math.Abs(player.y-e.y)>85)continue;bool catching=CanCatchLaunchedEnemy(e),standing=CanStandFromSlam(e);
                if(ApplyHitToEnemyFromXR1417(e,damage,catching||standing?HitReaction.None:reaction,launch,player.x,rawReaction)>0){
                    connected=true;RememberCommandLaunch(e,reaction);if(catching)CatchLaunchedEnemy(e);else if(standing)StandFromSlam(e);
                }}
            if(connected){hitstop=Math.Max(hitstop,stop);shake=4;}
        }
        private void AdvanceAction(float dt,Input input)
        {
            if(actionTiming==null)return;
            if(launchFollowup==LaunchFollowup.Strike&&Grounded(player)&&!CatchSwingNeedsHitWindow()){LandPlayer();return;}
            if(actionSkill!=null){TickSkillAction(dt,input);return;}
            if(actionTiming.command!=null){AdvanceCalibratedCommand(dt,input);return;}
            if(charging)
            {
                chargeTime+=dt;
                if(input.attackXReleased||!input.attackXHeld||chargeTime>=actionCommand.chargeHoldDuration){charging=false;player.clip=actionCommand.clip;player.clock=0;previousRoot=0;}
                else return;
            }
            var t=actionTiming;float root=t.travel*Smooth((player.clock-t.hitStart+.09f)/.22f);
            // The reference callback adapter starts dash-air X2's falling
            // phase at .23s. Native speed units are adapted to logical pixels;
            // this is not an original-client runtime measurement.
            if(actionCommand!=null&&actionCommand.clip=="dash-air-x2"&&!Grounded(player)&&player.clock>=.23f)player.vy=Math.Min(player.vy,-360);
            if(t.rootCurve!=null&&t.rootCurve.Count>0){float frame=Math.Min(player.clock*t.fps,t.rootCurve.Count-1);int a=(int)frame,b=Math.Min(a+1,t.rootCurve.Count-1);root=t.rootCurve[a]+(t.rootCurve[b]-t.rootCurve[a])*(frame-a);}
            if(actionSkill!=null&&actionSkill.id=="flame-geyser")root=0;MovePlayerAlongGround(player.x+player.facing*(root-previousRoot));previousRoot=root;
            int windowCount=t.hitWindows!=null&&t.hitWindows.Count>0?t.hitWindows.Count:1,eventCount=0;
            while(nextHitWindow<windowCount&&eventCount++<32)
            {
                DamageWindow w=t.hitWindows!=null&&t.hitWindows.Count>0?t.hitWindows[nextHitWindow]:null;
                if(nextRepeatTime<0)nextRepeatTime=w!=null?w.start:t.hitStart;
                if(player.clock+.00001f<nextRepeatTime)break;
                float scale=w!=null?w.damageScale:1;int raw=w!=null?w.rawReaction:0;actionHit=true;
                if(actionSkill!=null){var s=actionSkill;SpawnEffect(s.id,player.x+player.facing*35,player.y+32,player.facing);if(s.damage>0&&scale>0)HitNearby(Math.Max(1,(int)Math.Round(s.damage*scale)),s.range,Reaction(raw,s.reaction),s.allAround,s.launchVelocity,t.hitStop,raw);}
                else if(actionCommand!=null){var c=actionCommand;SpawnEffect("slash",player.x+player.facing*25,player.y+30,player.facing);if(scale>0)HitNearby(Math.Max(1,(int)Math.Round(c.damage*scale)),c.range,Reaction(raw,c.reaction),false,c.launchVelocity,t.hitStop,raw);}
                if(w!=null&&w.repeatInterval>0)nextRepeatTime+=Math.Max(.01f,w.repeatInterval);
                if(w==null||w.repeatInterval<=0||nextRepeatTime>w.end+.00001f){nextHitWindow++;nextRepeatTime=-1;}
            }
            float chainStart=t.hitEnd;if(t.hitWindows!=null)foreach(var w in t.hitWindows)chainStart=Math.Max(chainStart,w.end);
            if(actionCommand!=null&&actionHit&&player.clock>=Math.Max(chainStart,t.duration*.65f)&&StartBufferedCommand(input,true))return;
            if(player.clock>=t.duration)
            {
                // These attack states finish on ground contact; the finite
                // atlas holds its final pose while physics completes the fall.
                if(actionCommand!=null&&!Grounded(player)&&(actionCommand.clip=="dash-air-z3"||actionCommand.clip=="dash-air-x2"))return;
                bool command=actionCommand!=null;CancelAction(false);chainGrace=command?.25f:0;player.clip=Grounded(player)?"idle":"jump";player.clock=0;
            }
        }
        private void TickEnemyCore(Enemy e,float dt)
        {
            if(MirrorPeitaHands(e))return;
            if(TickCargoDirectorOwnedActorV21(e))return;EnsureCargoSourceInitialStateV21(e);if(e.aiEnabled&&!e.passive&&!e.isGate&&TickCargoSourceFlightV21(e,dt))return;if(TickFactoryRavenIntroOwnerV21(e,dt))return;
            if(e.hitPause>0){float frozen=Math.Min(e.hitPause,dt);e.hitPause=Math.Max(0,e.hitPause-frozen);dt-=frozen;if(dt<=.000001f){SampleCoreHardHitDisplayV39(e);NoteCoreHardHitPacketR642(e);return;}}
            if(e.dormant){foreach(var bossEnemy in enemies)if(string.IsNullOrEmpty(e.spawnAfterDeath)?IsBoss(bossEnemy.kind)&&bossEnemy.alerted:bossEnemy.kind==e.spawnAfterDeath&&bossEnemy.hp<=0&&bossEnemy.deathTime>=e.spawnDelay){e.dormant=false;ReleaseCargoInnerRobotV58(e,bossEnemy);break;}if(e.dormant)return;}
            if(!e.active)return;TickCoreHardEntryActivationV21(e);if(TickCoreStopIntroOwnerV21(e,dt))return;if(TickBelowFortManagedV20(e,dt))return;if(TickBelowTunnelManagedV20(e,dt))return;if(TickCoreHardManagedV21(e,dt))return;e.clock+=dt;if(e.isGate){TickCannonWreckStateV39(e);if(e.hp<=0)e.deathTime+=dt;return;}if(e.hp<=0){TickRegionDeath(e,dt);TickCargoDeathSoundsV58(e,dt);e.deathTime+=dt;IntegrateBody(e,dt,false);return;}if(PeitaSpecialProtected(e)){TickPeitaSpecial(e,dt);return;}if(TickPeitaEnvironment(e,dt)||TickPeitaDragonFlight(e,dt))return;if(!WyvernOwnsFlight(e))IntegrateBody(e,dt,false);
            if(TickPlainPairManagedR1258(e,dt))return;
            if(IsPeitaCrossbow(e)){TickPeitaCrossbow(e,dt);return;}
            if(IsRecoveryContainer(e)){e.aiEnabled=false;e.telegraph=0;return;}            if(TickRegionNpc(e,dt))return;
            if(e.isGate||e.passive||!e.aiEnabled||e.state==CombatState.Knockdown||e.state==CombatState.GettingUp||e.stun>0||!Grounded(e)&&!EnemyPositionFixedV25(e)&&!FactoryRavenCloneAirborneStateV21(e))return;
            if(TickPeitaEnemy(e,dt)||TickNpcRule(e,dt))return;
            float dx=player.x-e.x;bool boss=IsBoss(e.kind),ranged=e.kind=="toto"||e.kind=="fairy-guardian";if(!e.alerted){if(Math.Abs(dx)>e.aggroRange)return;e.alerted=true;}
            if(e.telegraph>0)
            {
                e.telegraph-=dt;if(e.telegraph<=.12f&&!e.struck){e.struck=true;if(ranged)effects.Add(new Effect{type="enemy-bolt",x=e.x+e.facing*20,y=e.y+24,facing=e.facing,velocityX=e.facing*150,hostileDamage=e.kind=="toto"?22:18,lifetime=2});SpawnEffect(boss?"slam":"enemy-hit",e.x+e.facing*30,e.y+16,e.facing);if(!ranged&&Math.Abs(dx)<(boss?92:52)&&Math.Abs(player.y-e.y)<38&&dx*e.facing> -12&&EnemyAccuracyAllowsHit(e))ApplyEnemyAttackToPlayer(boss?70:26,e.x,boss?HitReaction.Knockdown:HitReaction.Hurt);if(boss)shake=5;}
                if(e.telegraph<=0){e.cooldown=boss?1.5f:1.15f;e.clip="idle";e.clock=0;}return;
            }
            e.cooldown-=dt;e.facing=dx>=0?1:-1;
            if(Math.Abs(player.y-e.y)<38&&Math.Abs(dx)<(ranged?175:boss?85:43)&&e.cooldown<=0){e.telegraph=boss?.85f:.52f;e.struck=false;e.clip="attack";e.clock=0;}
            else if(Math.Abs(dx)>(ranged?130:boss?66:32)){MoveAlongGround(e,Clamp(e.x+e.facing*(e.kind=="tree-knight"?26:boss?46:36)*dt,22,RoomWidth-26));if(e.clip!="walk"){e.clip="walk";e.clock=0;}}
            else if(e.clip!="idle"){e.clip="idle";e.clock=0;}
        }

        public void Step(float dt,Input input)
        {
            if(paused||!Finite(dt)||dt<=0)return;
            dt=Math.Min(dt,.05f);if(input==null)input=EmptyInput;
            if(victory){TickCoreHardAfterVictoryV21(dt);StepClearMovement(dt,input);return;}
            if(player.hp<=0)ResetGateTransitionDelay();
            BeginCoreStopIntroOrdinaryStepV21();
            SnapshotNpcEffectOwners();
            if(CoreHardClosureScopeV21())coreHardHitPacketSerialR642++;elapsed+=dt;regionRawDirection=Finite(input.move)?Math.Sign(input.move):0;input=FilterFactoryPressInputV569(input,dt);input=CoreHardFilterPressInputV21(input);input=CoreHardClosureFilterInputV21(input,dt);input=PeitaPlayerStunned||elapsed<regionInputUntil?EmptyInput:RegionFilteredInput(PlainOriginalFxFilterInputR1304(input));roomBanner=Math.Max(0,roomBanner-dt);commandDisplay=Math.Max(0,commandDisplay-dt);
            UpdatePeitaSummons();FlushRegionSummons();TickRegionSpells(dt);TickPeitaPoison();TickRecoveryDrops(dt);
            for(int i=effects.Count-1;i>=0;i--){effects[i].age+=effects[i].followPlayer&&effects[i].type!="iron-body"&&effects[i].type!="armageddon-blade"?Math.Max(0,dt-hitstop):dt;if(effects[i].age>=effects[i].lifetime)effects.RemoveAt(i);}
            TickPeitaSpells(dt);hostileEffectSnapshot.Clear();foreach(var fx in effects)if(fx.hostileDamage>0)hostileEffectSnapshot.Add(fx);
            foreach(var fx in hostileEffectSnapshot)if(fx.hostileDamage>0){fx.x+=fx.velocityX*dt;if(Math.Abs(fx.x-player.x)<16&&Math.Abs(fx.y-player.y-24)<29){StrikePeitaProjectile(fx);fx.hostileDamage=0;fx.age=fx.lifetime;} }
            shake=Math.Max(0,shake-dt*18);armor=Math.Max(0,armor-dt);for(int slot=0;slot<PotionSlotCount;slot++)potionCooldowns[slot]=Math.Max(0,potionCooldowns[slot]-dt);
            tickingCooldowns.Clear();foreach(var entry in skillCooldowns)tickingCooldowns.Add(entry.Key);foreach(var key in tickingCooldowns){float left=skillCooldowns[key]-dt;if(left>0)skillCooldowns[key]=left;else skillCooldowns.Remove(key);}SyncCooldowns();comboTimeout-=dt;if(comboTimeout<=0)combo=0;
            TickState(player,dt,true);foreach(var e in enemies)TickState(e,dt,false);
            TickSkillEffects(dt);
            TickCommandHitMemory(dt);
            TickAwakening(dt);
            if(input.awaken)TryAwaken();
            int dashDirection=input.dash&&Finite(input.move)&&Math.Abs(input.move)>.01f?Math.Sign(input.move):0;
            bool lateRise=fastFall==FastFallPhase.None && elapsed-dashJumpStarted>SpringStepWindow;
            bool missedRebound=fastFall==FastFallPhase.Landing && elapsed-springContactTime>.075f;
            if(springDirection!=0 && (springDirection!=dashDirection || lateRise || missedRebound))
            {
                springDirection=0;
                if(fastFall==FastFallPhase.Landing){player.clip="land";player.clock=elapsed-springContactTime;}
            }
            bool accept=!AwakeningCasting&&player.hp>0&&!player.knockdownOnLanding&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp;            if(dashDirection!=bufferedDashDirection){bufferedDashTime=0;bufferedDashDirection=0;}            if(accept&&input.dashPressed&&dashDirection!=0){                bufferedDashDirection=dashDirection;bufferedDashTime=RecoveryInputLead;            }
            bool usedUp=false,usedDown=false,usedX=false,usedZ=false;
            if(accept)ReadLaunchFollowup(input,out usedUp,out usedDown,out usedX,out usedZ);
            if(accept && input.drop && !usedDown)
            {
                if(Grounded(player))
                {
                    var platform=NativeTerrain ? SurfaceBelow(player.x,player.y+.1f) : null;
                    if(platform!=null && platform.platform)
                    {
                        player.dropThrough=.25f;player.y-=.1f;player.groundLine=-1;
                        player.vy=-20;player.state=CombatState.Airborne;
                    }
                }
                else if(!Busy() && fastFall==FastFallPhase.None)
                {
                    fastFall=FastFallPhase.Requested;
                    jumpBuffered=0;
                }
            }
            if(accept){if(input.jump&&!usedUp)jumpBuffered=JumpBufferDuration;if(input.attack&&!usedZ)QueuePress('Z',input);if(input.attackX&&!usedX)QueuePress('X',input);if(input.skill>=0&&input.skill<SkillSlotCount&&!TryInstantSkill(input.skill)){skillBuffered=input.skill;skillBufferTime=InputBufferDuration;}for(int slot=0;slot<PotionSlotCount;slot++)if((input.potionPressedMask&(1<<slot))!=0)UsePotion(slot);if(input.potion)UsePotion();}
            // Input recovery follows action time. Repeated impact freezes must not
            // age out an early follow-up while the current animation is stopped.
            float simulationDt=dt;
            if(hitstop>0)
            {
                float frozen=Math.Min(dt,hitstop);foreach(var press in presses)press.expires+=frozen;hitstop=Math.Max(0,hitstop-frozen);dt-=frozen;
                // A .01s source stop consumes .01s, not an entire 60 Hz packet.
                // Independent enemy/effect clocks still receive simulationDt.
                if(dt<=.000001f){foreach(var enemy in enemies)TickEnemy(enemy,simulationDt);TickNpcEffects();return;}
            }
            if(launchFollowup==LaunchFollowup.Jump||launchFollowup==LaunchFollowup.Dive||launchFollowup==LaunchFollowup.Landing){
                launchInputRemaining-=dt;
                if(launchFollowup==LaunchFollowup.Dive&&launchInputRemaining<JumpBufferDuration-.05f&&!Grounded(player)&&fastFall==FastFallPhase.None)
                    fastFall=FastFallPhase.Requested;
                if(launchInputRemaining<=0)ClearLaunchFollowup();
            }
            while(presses.Count>0&&presses.Peek().expires<elapsed)presses.Dequeue();
            float walkClock=player.clip=="walk"?player.clock:0;
            TickHamelWater(simulationDt); player.clock+=PlainOriginalFxAnimationDeltaR1304(dt)*HamelWaterAnimation*RegionStatusAnimation;landing=Math.Max(0,landing-dt);jumpTakeoff=Math.Max(0,jumpTakeoff-dt);chainGrace=Math.Max(0,chainGrace-dt);if(chainGrace<=0&&actionTiming==null)chain="";TickManaRecovery(dt);
            AdvanceAwakeningCast();
            if(landing>0&&landingTiming!=null&&landingTiming.command!=null){                var recovery=landingTiming.command;                if(CanCancelCommandMove(recovery,input)||                    (presses.Count>0&&recovery.attackCancel>=0&&player.clock>recovery.attackCancel))landing=0;            }
            if(landing>0&&landingTiming!=null&&landingTiming.command!=null&&skillBufferTime>0&&landingTiming.command.skillCancel>=0&&player.clock>landingTiming.command.skillCancel&&StartSkill(skillBuffered))
            {landing=0;skillBufferTime=0;skillBuffered=-1;}
            // A fresh up press can be buffered above the floor. It cancels only a
            // voluntary fast landing, after collision has found actual support.
            if(fastFall==FastFallPhase.Landing && jumpBuffered>0 && Grounded(player))landing=0;
            if(landing<=0)
            {
                landingTiming=null;
                if(fastFall==FastFallPhase.Landing){if(jumpBuffered<=0)springDirection=0;fastFall=FastFallPhase.None;}
            }
            if(player.hp>0&&!Busy())
            {
                if(Finite(input.move)&&input.move!=0)player.facing=input.move>0?1:-1;
                if(Finite(input.move)&&input.move!=0)player.vx=LocomotionSpeed(Clamp(input.move,-1,1), input.dash, dt);
                if(jumpBuffered>0 && Grounded(player))
                {
                    takeoffDirection=dashDirection;airborneDash=dashDirection!=0;dashJumpStarted=elapsed;
                    if(airborneDash)takeoffGravity=DashGravityAtTakeoff(dashDirection);
                    player.vy=airborneDash?DashJumpSpeed:420;player.state=CombatState.Airborne;player.knockdownOnLanding=false;
                    // Keep the existing run cycle through a short Up/Down roll. A held jump
                    // changes to its dash-air pose once the quick input window has passed.
                    if(!airborneDash || player.clip!="run"){player.clip="jump-start";player.clock=0;}
                    jumpTakeoff=.1f;airLandingClip="land";jumpBuffered=0;chain="";chainGrace=0;
                    if(springDirection==0)SpawnEffect("dust",player.x,PlayerGroundY);
                }
                bool skillStarted=skillBufferTime>0&&StartSkill(skillBuffered);if(skillStarted){skillBuffered=-1;skillBufferTime=0;}else StartBufferedCommand(input,false);
            }
            if(actionTiming!=null || AwakeningCasting || player.stun>0){fastFall=FastFallPhase.None;springDirection=0;}
            if(actionTiming!=null){if(PlainOriginalFxStunRemainingR1304<=0)AdvanceAction(PlainOriginalFxAnimationDeltaR1304(CoreHardActionDeltaV21(dt))*HamelWaterAnimation,input);}
            else if(!AwakeningCasting&&player.hp>0&&player.stun<=0&&!player.knockdownOnLanding&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp&&landing<=0)
            {
                if(fastFall==FastFallPhase.Requested)
                {
                    fastFall=FastFallPhase.Descending;
                    jumpTakeoff=0;
                    player.vy=-FastFallSpeedLimit*(NativeTerrain ? HamelGravityScale : 1);
                    if(airborneDash && dashDirection!=0 && dashDirection==takeoffDirection && elapsed-dashJumpStarted<=SpringStepWindow)
                        springDirection=dashDirection;
                }
                float move=Finite(input.move)?Clamp(input.move,-1,1):0;player.vx=LocomotionSpeed(move,input.dash,dt);
                bool keepRun=springDirection!=0 || airborneDash && dashDirection==takeoffDirection && player.clip=="run"
                    && elapsed-dashJumpStarted<=SpringStepWindow && fastFall==FastFallPhase.None;
                string clip=keepRun?"run":!Grounded(player)?(fastFall==FastFallPhase.Descending?"jump-fall":jumpTakeoff>0?"jump-start":airborneDash?"dash-jump":player.vy>0?"jump-rise":"jump-fall"):move!=0?(input.dash?"run":"walk"):"idle";
                if(player.clip!=clip){player.clip=clip;player.clock=0;}
            }
            float beforeX=player.x;
            ApplyLaunchCarry(input.move,dt);IntegrateBody(player,dt,true);
            TickHamelTriggers(simulationDt); TickPeitaFallRecovery();
            TickVelderStage(dt);
            // Keep the support foot moving with the ground even at partial stick
            // deflection, during slow effects, or as a wall stops the actor.
            if(player.clip=="walk")player.clock=walkClock+Math.Abs(player.x-beforeX)/GroundWalkSpeed;
            foreach(var e in enemies)TickEnemy(e,simulationDt);TickNpcEffects();TickMine2VoidHazardV21();
            jumpBuffered=Math.Max(0,jumpBuffered-dt);skillBufferTime=Math.Max(0,skillBufferTime-dt);            bufferedDashTime=Math.Max(0,bufferedDashTime-dt);
            if(TickBossCompletionV57(dt))return;
            if(TickEncounters(dt))return;
            TickLegacyGateTransition(dt);
        }
                // Scoped private-port policy; no claim of original native/server quantization.
        struct BelowCannonFractionalHitResultR901 {
            public string outcome;
            public bool countered;
            public float rawDamage,afterDefense,hpBefore,hpAfter,appliedHpLoss;
        }
        public int EnemyImpactSerial,PlayerImpactSerial;public bool LastImpactHeavy,LastPlayerGuard;
        public string Status(){return victory?"CLEAR":player.hp<=0?"DEFEATED":BossCompletionPendingV57?"BOSS DEFEATED":EncounterTimeRemaining>0?"SURVIVE  /  "+(int)Math.Ceiling(EncounterTimeRemaining)+"s":RoomCombatClear&&UnbrokenExitGate!=null?"BREAK THE GATE":autoEnterAfterGateBreak?"ENTERING NEXT AREA":gateOpen?"GO TO THE GATE":"DEFEAT ALL ENEMIES";}
        private static HitReaction Reaction(int raw,HitReaction fallback){return raw==1||raw==2?HitReaction.Hurt:raw==3?HitReaction.Knockdown:raw==4||raw==5?HitReaction.Launch:fallback;}

        private void Command(CommandContext context,string sequence,string clip,int damage,HitReaction reaction,float range=79,string chargeClip=null,bool land=false)
        // Both charge states declare EVENT_PROCESS_TIME0=1.8 source seconds.
        // The callback checks state time, independent of animation speed.
        {commands.Add(new CommandDefinition{id=context+":"+sequence,context=context,sequence=sequence,clip=clip,damage=damage,reaction=reaction,range=range,chargeClip=chargeClip,chargeHoldDuration=chargeClip!=null?1.8f:0,landBeforeStart=land,description=DisplayCommand(context,sequence)});}
        private void CreateCommands()
        {
            Command(CommandContext.Ground,"Z","combo-z1",42,HitReaction.Hurt);
            Command(CommandContext.Ground,"ZZ","combo-z2",44,HitReaction.Hurt);
            Command(CommandContext.Ground,"ZZZ","combo-z3",48,HitReaction.Hurt);
            Command(CommandContext.Ground,"ZZZZ","combo-z4",72,HitReaction.Knockdown,94);
            Command(CommandContext.Ground,"ZZZX","combo-z-launch",65,HitReaction.Launch,95);
            Command(CommandContext.Ground,"ZZZ>Z","combo-z-front",68,HitReaction.Hurt,110);
            Command(CommandContext.Ground,"X","combo-x1",58,HitReaction.Hurt,90);
            Command(CommandContext.Ground,"XX","combo-x2",60,HitReaction.Hurt,92);
            Command(CommandContext.Ground,"XXZ","combo-x-launch",65,HitReaction.Launch,95);
            Command(CommandContext.Ground,"XXX","combo-x3",62,HitReaction.Hurt,95);
            Command(CommandContext.Ground,"XXXX","combo-x4",64,HitReaction.Hurt,98);
            Command(CommandContext.Ground,"XXXXX","combo-x5",85,HitReaction.Hurt,105);
            Command(CommandContext.Dash,"Z","dash-z1",45,HitReaction.Hurt,85);
            Command(CommandContext.Dash,"ZZ","dash-z2",48,HitReaction.Hurt,88);
            Command(CommandContext.Dash,"ZZZ","dash-z3",72,HitReaction.Hurt,95);
            Command(CommandContext.Dash,"ZZX","dash-z-launch",65,HitReaction.Launch,95);
            Command(CommandContext.Dash,"ZX","dash-zx",68,HitReaction.Hurt,98,"dash-zx-charge");
            Command(CommandContext.Dash,"ZXX","dash-zxx",80,HitReaction.Launch,105);
            Command(CommandContext.Dash,"X","dash-x1",55,HitReaction.Launch,88);
            Command(CommandContext.Dash,"XX","dash-xx",72,HitReaction.Hurt,98,"dash-xx-charge");
            Command(CommandContext.Dash,"XXX","dash-xxx",90,HitReaction.Launch,110);
            Command(CommandContext.Air,"Z","air-z",45,HitReaction.Hurt,80);
            Command(CommandContext.Air,"X","air-x",65,HitReaction.Knockdown,90);
            Command(CommandContext.DashAir,"Z","dash-air-z1",45,HitReaction.Hurt,82);
            Command(CommandContext.DashAir,"ZZ","dash-air-z2",48,HitReaction.Hurt,85);
            Command(CommandContext.DashAir,"ZZZ","dash-air-z3",68,HitReaction.Hurt,92);
            Command(CommandContext.DashAir,"X","dash-air-x1",58,HitReaction.Hurt,90);
            Command(CommandContext.DashAir,"XX","dash-air-x2",62,HitReaction.Hurt,95);
            Command(CommandContext.DashAir,"XXX","dash-air-x3",78,HitReaction.Launch,100,null,true);
            Command(CommandContext.DashAir,"XXZ","combo-z1",42,HitReaction.Hurt,79,null,true);
        }
    
        BelowCannonFractionalHitResultR901 ApplyBelowCannonFractionalHitR901(float damage,float fromX,HitReaction reaction,float launchVelocity){
            if(!Finite(damage)||damage<=0||damage>=1||!Finite(fromX)||!Finite(launchVelocity)||!Finite(player.hp))
                throw new ArgumentException("R901 invalid scoped fractional hit");
            var result=new BelowCannonFractionalHitResultR901{outcome="blocked",rawDamage=damage,hpBefore=player.hp,hpAfter=player.hp};
            // Counter precedes the ordinary ApplyHit guards, just as the int API does.
            if(TryCounterPlayerFractionalHitR901(damage,fromX,reaction)){result.countered=true;result.outcome="countered";return result;}
            float dealt=ApplyBelowCannonFractionalPlayerDamageR901(damage,fromX,reaction,launchVelocity,ref result);
            if(dealt>0){regionLastPlayerDamage=dealt;PlayerImpactSerial++;LastPlayerGuard=PlayerHasSuperArmor;AddAwakeningCharge(2);}
            return result;
        }

        float ApplyBelowCannonFractionalPlayerDamageR901(float damage,float fromX,HitReaction reaction,float launchVelocity,ref BelowCannonFractionalHitResultR901 result){
            var f=player;
            if(f.hp<=0){result.outcome="dead";return 0;}
            if(f.invincible>0){result.outcome="invincible";return 0;}
            if(AwakeningCasting){result.outcome="awakening";return 0;}
            bool down=f.state==CombatState.Knockdown;
            bool armored=f.superArmor>0||armor>0||CommandSuperArmorActive;
            float actual=ApplyBelowCannonFractionalDefenseR901(down?damage*.5f:damage);
            result.afterDefense=actual;
            actual=Math.Min(actual,f.hp);
            float after=Math.Max(0,f.hp-actual),dealt=f.hp-after;
            // No minimum: a sub-ULP subtraction that leaves stored HP unchanged is not a HIT.
            if(dealt<=0){result.outcome="no-representable-hp-loss";return 0;}
            ApplyCombatHealth(f,after);result.hpAfter=after;result.appliedHpLoss=dealt;result.outcome="applied";
            int direction=f.x>=fromX?1:-1;
            f.impactFlashDuration=reaction==HitReaction.Launch||reaction==HitReaction.Knockdown?.10f:.075f;f.impactFlash=f.impactFlashDuration;
            SpawnBelowCannonFractionalNumberR901(f.x,f.y+55,dealt);SpawnEffect(armored?"guard":"hit",f.x,f.y+25,direction);
            if(f.hp<=0){f.state=CombatState.Dead;f.clip="death";f.clock=0;f.stun=0;CancelAction(true);return dealt;}
            // Preserve original down/armor/None handling and the complete player reaction path.
            if(down||armored||reaction==HitReaction.None)return dealt;
            CancelAction(true);landing=0;f.invincible=.22f;f.clock=0;
            if(reaction==HitReaction.Knockdown&&Grounded(f))BeginKnockdown(f,true);
            else if(reaction==HitReaction.Launch||!Grounded(f))
            {f.state=CombatState.Airborne;f.knockdownOnLanding=true;f.vy=Math.Max(f.vy,reaction==HitReaction.Launch?launchVelocity:125);f.vx=direction*34;f.stun=.18f;f.clip="launched";}
            else{f.state=CombatState.Hurt;f.stun=.28f;f.clip="hurt";}
            return dealt;
        }

        void SpawnBelowCannonFractionalNumberR901(float x,float y,float amount){
            if(!Finite(amount)||amount<=0)throw new ArgumentException("R901 invalid real fractional number");
            var number=SpawnNumberAt(x,y,-1,0,CombatNumberKind.Damage);number.hasFractionalAmountR901=true;number.fractionalAmountR901=amount;
        }

#if UNITY_EDITOR
        internal void InitializeCoreHardRoom4ComponentR908(IEnumerable<RoomDefinition> definitions)

        {
            if(coreHardV21==null||coreHardV21.difficultyValue!=1||coreHardV21.npcLevel!=37||!CoreHardEntryActivationConfiguredV21||!CoreHardLocalBalanceConfiguredV21||!CoreHardClosureConfiguredV21||!CoreStopIntroOwnerConfiguredV21)
                throw new InvalidOperationException("R908 requires original complete Hard1/level37 configuration and STOP owner before fresh initialization");


            if(definitions==null)throw new ArgumentNullException("definitions");

            var validated=new List<RoomDefinition>();

            foreach(var s in definitions)

            {

                if(s==null||!Finite(s.width)||s.width<480||!Finite(s.spawnX)||!Finite(s.exitX))throw new ArgumentException("Rooms need finite coordinates and width >= 480.");

                bool sectioned=s.encounters!=null&&s.encounters.Length>0;

                if(s.hasSpawnY&&!Finite(s.spawnY))throw new ArgumentException("Invalid authored player spawn height.");

                var r=new RoomDefinition{id=s.id,title=s.title,width=s.width,spawnX=sectioned?s.spawnX:Clamp(s.spawnX,22,s.width-80),exitX=sectioned?s.exitX:s.exitX>0?Clamp(s.exitX,80,s.width-26):s.width-56,groundNodes=CopyGroundNodes(s),surfaces=s.surfaces==null?new GroundSurface[0]:Array.ConvertAll(s.surfaces,q=>new GroundSurface{id=q.id,x1=q.x1,y1=q.y1,x2=q.x2,y2=q.y2,platform=q.platform,water=q.water,hamelTrigger=q.hamelTrigger,initiallyDisabled=q.initiallyDisabled,bungeeTargetPlusOne=q.bungeeTargetPlusOne,npcOnly=q.npcOnly}),hasSpawnY=s.hasSpawnY,spawnY=s.spawnY,entryEncounter=s.entryEncounter,encounters=CopyEncounters(s)};
                r.teleports=CopyGroundTeleports(s.teleports,r.surfaces);BindHamelSourceGroundSpeeds(r);
                r.bungeeLineIds=s.bungeeLineIds==null?null:(int[])s.bungeeLineIds.Clone();

                if(!sectioned&&r.exitX<=r.spawnX)throw new ArgumentException("Room exit must be after spawn.");

                if(s.enemies!=null)foreach(var e in s.enemies)

                {

                    if(e==null||string.IsNullOrEmpty(e.kind)||!Finite(e.x)||!Finite(e.y)||!Finite(e.aggroRange)||!Finite(e.hp))throw new ArgumentException("Invalid enemy spawn.");

                    float x=Clamp(e.x,22,r.width-26),ground=GroundAt(r,x);

                    r.enemies.Add(new EnemySpawn{kind=e.kind,x=x,y=r.surfaces.Length>0?e.y:e.y<=GroundY+.01f?ground:Math.Max(ground,e.y),aggroRange=Math.Max(0,e.aggroRange),hp=e.hp,passive=e.passive,dormant=e.dormant,groundLine=e.groundLine,facing=e.facing,nativeDepth=e.nativeDepth,visualScale=e.visualScale,aiEnabled=e.aiEnabled,spawnAfterDeath=e.spawnAfterDeath,spawnDelay=e.spawnDelay,sourceNPC=e.sourceNPC,sourceKeyCode=e.sourceKeyCode,encounterIndex=e.encounterIndex,countsForClear=e.countsForClear,isGate=e.isGate,siegeMode=e.siegeMode});

                }

                validated.Add(r);

            }

            if(validated.Count==0)throw new ArgumentException("A dungeon needs at least one room.");

            if(validated.Count!=5)throw new ArgumentException("R908 retains all five original Core rooms");
            int birthsR908=0,nodesR908=0;
            for(int i=0;i<validated.Count;i++){
                if(validated[i].id!="Dungeon_Altera_Core_Hard-"+i)throw new ArgumentException("R908 cannot renumber or substitute rooms");
                birthsR908+=validated[i].enemies.Count;nodesR908+=validated[i].encounters.Length;
            }
            if(birthsR908!=85||nodesR908!=8||validated[4].entryEncounter!=0||validated[4].encounters[0].sourceWorldId!="WI_ALTERA_CORE_BOSS_STAGE")throw new ArgumentException("R908 exact85birth/8node/original room4 world required");
            ValidateEncounterGraph(validated);

            alteraDeferredCannonDefinitions.Clear();

            rooms.Clear();rooms.AddRange(validated);roomKinds.Clear();foreach(var r in rooms)roomKinds.Add(r.enemies.Count>0?r.enemies[0].kind:"");ResetCoreHardRoom4ComponentR908();

        }

        void ResetCoreHardRoom4ComponentR908()

        {

            ResetDungeonScore(); ResetRecoveryDropRun();

            ResetNpcEffects();

            npcActionEvents.Clear();npcRandom=0x7a2189u;

            ResetSkillState();ResetAwakening();combatEvents.Clear();

            player=new Fighter();mana=MaxMana;armor=elapsed=hitstop=shake=comboTimeout=landing=0;Array.Clear(potionCooldowns,0,potionCooldowns.Length);kills=hits=combo=stageKills=0;victory=paused=false;

            skillCooldowns.Clear();Array.Clear(cooldowns,0,cooldowns.Length);effects.Clear();pendingEncounterEntry=-1;encounterRandom=0x9173a5u;EnterRoom(4);

        }
#endif
}
}
