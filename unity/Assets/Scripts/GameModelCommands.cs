using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    [Serializable] public sealed class MotionEvent { public float time,x,y;public int mode;public string label; }
    [Serializable] public sealed class MotionRootSample { public float time,x; }
    [Serializable] public sealed class ProtectionWindow { public float start,end; }
    [Serializable] public sealed class CommandTransition
    { public string button,nextClip;public float bufferStart,bufferEnd,executeAt;public bool forward,onLand; }
    [Serializable] public sealed class CommandDamageStage
    { public float time,physicalMultiplier,backSpeedX,backSpeedY,stun,attackerStop,defenderStop,repeatInterval,forceDown;public int rawReaction;public bool repeat,forceFly; }
    [Serializable] public sealed class CommandCalibration
    {
        public string sourceState,evidence;
        public float animationSpeed=1,startAt,endAt,startX,startY,passiveX=-1,passiveY=-1;
        public bool hasStartX,hasStartY,forwardRunStart,forwardAirControl,waitForLand;
        public float walkCancel=-1,backWalkCancel=-1,dashCancel=-1,attackCancel=-1,skillCancel=-1,chargeHoldSeconds;
        public MotionEvent[] motionEvents=new MotionEvent[0];
        public CommandTransition[] transitions=new CommandTransition[0];
        public CommandDamageStage[] damageStages=new CommandDamageStage[0];
        public bool applyAnimationMove;
        public MotionRootSample[] motionRoot=new MotionRootSample[0];
        public ProtectionWindow[] superArmorWindows=new ProtectionWindow[0];
    }
    [Serializable] public sealed class AttackGeometry
    {
        public float weaponBaseX,weaponBaseY,weaponStartX,weaponStartY,weaponEndX,weaponEndY,radius=3;
        public float rightFootStartX,rightFootStartY,rightFootEndX,rightFootEndY;
        public float leftFootStartX,leftFootStartY,leftFootEndX,leftFootEndY;
    }
    [Serializable] public sealed class AttackBox
    { public float left,right,bottom,top;public int windowIndex;public string shape; }
    [Serializable] public sealed class CombatEvent
    {
        public string type,clip,target,state,phase;
        public float time,clock,playerX,playerY,targetX,targetY,targetHp;
        public int windowIndex,damage,rawReaction;
    }
    public sealed partial class GameModel
    {
        public const float NativeToLogical=.28f;
        public readonly List<CombatEvent> combatEvents=new List<CombatEvent>();
        private readonly List<AttackBox> activeHitBoxes=new List<AttackBox>();
        private readonly Dictionary<Enemy,float> sourceHitMemory=new Dictionary<Enemy,float>();
        private readonly HashSet<int> commandHitWindows=new HashSet<int>(),closedHitWindows=new HashSet<int>();
        private float commandPreviousClock,commandPreviousX,commandPreviousY,commandPassiveX=-1,commandPassiveY=-1;
        private int motionEventIndex;
        private const float RecoveryInputLead=.10f;
        private int bufferedDashDirection;
        private float bufferedDashTime;

        private bool DashCancelRequested(Input input)
        {
            return bufferedDashTime>0&&input.dash&&Finite(input.move)&&
                input.move*bufferedDashDirection>.01f;
        }

        private bool CanCancelCommandMove(CommandCalibration command,Input input)
        {
            if(!Grounded(player)||!Finite(input.move)||input.move==0)return false;
            if(DashCancelRequested(input)&&command.dashCancel>=0)
                return player.clock>command.dashCancel;
            if(input.move*player.facing<0&&command.backWalkCancel>=0)
                return player.clock>command.backWalkCancel;
            return command.walkCancel>=0&&player.clock>command.walkCancel;
        }
        // An Up cancel belongs to the command that physically lifted these enemies.
        // The short Down/X gesture survives impact freeze and early floor contact.
        private enum LaunchFollowup { None, Jump, Dive, Strike, Landing, Stand }
        private LaunchFollowup launchFollowup;
        private const float MaxLaunchCarrySpeed=560,MaxLaunchCarryDistance=48;
        private float launchInputRemaining,launchCarry,launchCarryStartX,launchCarryStarted;
        private enum LaunchGesture { None, Up, Down, X, Z }
        private LaunchGesture launchGesture;
        private readonly List<Enemy> launchVictims=new List<Enemy>();

        private readonly HashSet<Enemy> slammedVictims=new HashSet<Enemy>();
        private bool keepSlamWindow,standBuffered;
        private void ClearLaunchFollowup()
        {
            launchGesture=LaunchGesture.None;launchCarry=0;
            launchVictims.Clear();
            if(keepSlamWindow)return;
            foreach(var e in slammedVictims){
                if(e.hp>0&&e.state==CombatState.Knockdown&&!FairyReactionScopeV34(e))
                    e.superArmor=Math.Max(e.superArmor,e.downTime+EnemyGetUpDuration+.55f);
            }
            slammedVictims.Clear();standBuffered=false;
            launchFollowup=LaunchFollowup.None;
            launchInputRemaining=0;
        }
        private bool AirborneLaunchVictim(Enemy e)
        {
            return e.active&&e.hp>0&&e.state==CombatState.Airborne&&!Grounded(e)
                &&!e.isGate&&!EnemyPositionFixedV25(e)&&!EnemyHasSuperArmor(e)
                &&!CoreHardShieldGuardingR1417(e,player.x);
        }
        private void RememberCommandLaunch(Enemy e,HitReaction reaction)
        {
            if(actionCommand!=null&&launchFollowup==LaunchFollowup.None&&reaction==HitReaction.Launch
                &&e.knockdownOnLanding&&AirborneLaunchVictim(e)&&!launchVictims.Contains(e))launchVictims.Add(e);
        }
        private void ReadLaunchFollowup(Input input,out bool usedUp,out bool usedDown,out bool usedX,out bool usedZ)
        {
            usedUp=usedDown=usedX=usedZ=false;
            // The launcher resolves contact later in Step. Remember an ordered gesture
            // during the existing jump buffer, without granting a cancel on a miss.
            if(launchFollowup==LaunchFollowup.None&&actionCommand!=null){
                if(jumpBuffered<=0)launchGesture=LaunchGesture.None;
                if(input.jump&&actionTiming.command!=null){
                    foreach(var stage in actionTiming.command.damageStages)
                        if(Reaction(stage.rawReaction,HitReaction.None)==HitReaction.Launch){
                            launchGesture=LaunchGesture.Up;break;
                        }
                }
                if(input.drop&&launchGesture==LaunchGesture.Up){launchGesture=LaunchGesture.Down;usedDown=true;}
                if(input.attackX&&launchGesture==LaunchGesture.Down)launchGesture=LaunchGesture.X;
                if(input.attack&&launchGesture==LaunchGesture.X)launchGesture=LaunchGesture.Z;
            }
            var gesture=launchGesture;
            if(launchFollowup==LaunchFollowup.None&&actionCommand!=null&&(input.jump||jumpBuffered>0)
                &&launchVictims.Exists(AirborneLaunchVictim)&&player.stun<=0&&!PeitaPlayerStunned&&PlainOriginalFxStunRemainingR1304<=0)
            {
                var victims=launchVictims.ToArray();
                bool onFloor=Grounded(player);
                CancelAction(true);
                launchVictims.AddRange(victims);
                // Follow the launcher's outward momentum only as part of its short
                // cancel hop. No facing change, position snap or distant target pull.
                launchCarryStartX=player.x;launchCarryStarted=elapsed;
                var nearest=NearestLaunchVictim(victims);
                if(nearest!=null)launchCarry=player.facing*Math.Min(MaxLaunchCarrySpeed,Math.Max(0,nearest.vx*player.facing));
                launchFollowup=LaunchFollowup.Jump;
                launchInputRemaining=JumpBufferDuration;
                landing=0;landingTiming=null;jumpTakeoff=.1f;
                if(onFloor){
                    airborneDash=input.dash&&Math.Abs(input.move)>.01f;
                    player.vy=airborneDash?DashJumpSpeed:420;
                    if(airborneDash)takeoffGravity=DashGravityAtTakeoff(player.facing);
                    takeoffDirection=airborneDash?player.facing:0;dashJumpStarted=elapsed;
                    player.groundLine=-1;SpawnEffect("dust",player.x,player.y);
                }
                player.state=CombatState.Airborne;player.clip="jump-start";player.clock=0;
                airLandingClip="land";usedUp=true;
                RecordCombat("launch-jump-cancel",-1,null);
            }
            if((input.drop||gesture>=LaunchGesture.Down)&&launchFollowup==LaunchFollowup.Jump){
                usedDown=true;launchFollowup=LaunchFollowup.Dive;
                launchInputRemaining=JumpBufferDuration;jumpTakeoff=0;
                // Keep the short hop through X startup. Down alone still fast-falls.
            }
            if((input.attackX||gesture>=LaunchGesture.X)&&launchFollowup==LaunchFollowup.Dive){
                var command=FindCommand(CommandContext.Air,"X");
                if(command==null)return;
                var victims=launchVictims.ToArray();
                landing=0;landingTiming=null;presses.Clear();jumpBuffered=0;
                float takeoff=player.vy,carry=launchCarry;
                BeginCommand(command,new Press{button='X',forward=input.move*player.facing>.1f});
                // A confirmed 82X cancel enters the overhead portion of the same air
                // swing. Its authored blade and active window still have to touch.
                if(actionTiming.command!=null&&actionTiming.hitWindows.Count>0){
                    var first=actionTiming.hitWindows[0];
                    // A target already overhead needs a little of the rising arc.
                    // At close height the overhead pose is ready immediately.
                    var nearest=NearestLaunchVictim(victims);
                    float height=nearest==null?0:nearest.y-player.y;
                    // Continue the ascent through an overhead X. Descending attacks
                    // never gain a second jump, and gravity remains the normal one.
                    if(takeoff>0&&nearest!=null&&Math.Abs(nearest.x-player.x)<96)
                        takeoff=Math.Min(DashJumpSpeed,takeoff+Clamp(height-32,0,64)*3);
                    float lead=Clamp((height-32)/480f,0,first.start);
                    player.clock=commandPreviousClock=Math.Min(first.end,first.start+.05f-lead);
                }
                player.vy=takeoff;launchCarry=carry;
                launchVictims.AddRange(victims);launchFollowup=LaunchFollowup.Strike;
                usedX=true;
            }
            if(launchFollowup==LaunchFollowup.Strike||launchFollowup==LaunchFollowup.Landing){
                if(input.attack||gesture==LaunchGesture.Z){standBuffered=true;usedZ=true;}
                if(standBuffered&&slammedVictims.Count>0&&Grounded(player)){
                    var firstZ=FindCommand(CommandContext.Ground,"Z");
                    if(firstZ==null)return;
                    landing=0;landingTiming=null;presses.Clear();
                    keepSlamWindow=true;
                    BeginCommand(firstZ,new Press{button='Z',forward=input.move*player.facing>.1f});
                    keepSlamWindow=false;launchFollowup=LaunchFollowup.Stand;standBuffered=false;
                }
            }
        }
        private Enemy NearestLaunchVictim(Enemy[] victims)
        {
            Enemy nearest=null;float gap=float.PositiveInfinity;
            foreach(var e in victims){
                float distance=(e.x-player.x)*player.facing;
                if(distance<0||distance>=gap||!AirborneLaunchVictim(e))continue;
                nearest=e;gap=distance;
            }
            return nearest;
        }
        private void ApplyLaunchCarry(float move,float dt)
        {
            if(launchCarry==0)return;
            int direction=Math.Sign(launchCarry);
            if(launchFollowup==LaunchFollowup.Jump&&elapsed-launchCarryStarted>.05f)return;
            bool following=launchFollowup==LaunchFollowup.Jump||launchFollowup==LaunchFollowup.Dive||
                launchFollowup==LaunchFollowup.Strike&&CatchSwingNeedsHitWindow();
            float remaining=MaxLaunchCarryDistance-(player.x-launchCarryStartX)*direction;
            if(following&&direction==player.facing&&move*direction>=0&&remaining>.25f&&dt>0)
                player.vx=direction*Math.Min(Math.Max(player.vx*direction,Math.Abs(launchCarry)),Math.Max(140*Math.Max(0,move*direction),remaining/dt));
            else{
                if(player.vx*direction>0)player.vx=direction*Math.Min(player.vx*direction,Math.Max(0,move*direction)*140);
                launchCarry=0;
            }
        }
        private bool CanCatchLaunchedEnemy(Enemy e)
        {
            return launchFollowup==LaunchFollowup.Strike&&launchVictims.Contains(e)
                &&e.knockdownOnLanding&&AirborneLaunchVictim(e);
        }
        private void CatchLaunchedEnemy(Enemy e)
        {
            if(e.hp<=0)return;
            slammedVictims.Add(e);e.knockdownOnLanding=true;launchCarry=0;player.vx=0;
            e.vx=0;e.vy=Math.Min(e.vy,-FastFallSpeedLimit);e.state=CombatState.Airborne;e.clip="fall";e.clock=0;
            player.vy=Math.Min(player.vy,-FastFallSpeedLimit*(NativeTerrain?HamelGravityScale:1));
            RecordCombat("launch-slam",-1,e);
        }
        private bool CanStandFromSlam(Enemy e)
        {
            return launchFollowup==LaunchFollowup.Stand&&slammedVictims.Contains(e)
                &&e.hp>0&&e.active&&!e.isGate&&!EnemyPositionFixedV25(e)&&!EnemyHasSuperArmor(e)
                &&!CoreHardShieldGuardingR1417(e,player.x)
                &&(e.state==CombatState.Knockdown||e.state==CombatState.Airborne&&e.knockdownOnLanding);
        }
        private void StandFromSlam(Enemy e)
        {
            if(e.hp<=0)return;
            slammedVictims.Remove(e);e.knockdownOnLanding=false;e.downTime=e.recoveryTime=0;
            e.vy=Math.Min(0,e.vy);e.clock=0;e.state=CombatState.Hurt;e.clip="hurt";e.stun=Math.Max(e.stun,.24f);
            e.hasOriginalHitV45=true;e.originalHitCodeV45=1;e.originalHitWasDownV45=false;
            e.originalHitAirV45=!Grounded(e);e.originalHitBackV45=(player.x-e.x)*e.facing<0;
            Debuff(e).knockdownGauge=0;
            RecordCombat("launch-stand",-1,e);
        }
        private bool CatchSwingNeedsHitWindow()
        {
            if(launchFollowup!=LaunchFollowup.Strike||actionTiming==null)return false;
            float end=actionTiming.hitEnd;
            if(actionTiming.hitWindows!=null&&actionTiming.hitWindows.Count>0){
                end=0;foreach(var window in actionTiming.hitWindows)end=Math.Max(end,window.end);
            }
            return player.clock<=end;
        }
        public IReadOnlyList<AttackBox> ActiveHitBoxes { get { return activeHitBoxes.AsReadOnly(); } }
        public bool CommandSuperArmorActive
        {get{if(actionTiming==null||actionTiming.command==null||charging)return false;foreach(var w in actionTiming.command.superArmorWindows)if(player.clock>=w.start&&player.clock<w.end)return true;return false;}}
        public string CurrentPhase
        {
            get
            {
                if(charging)return "charge";if(actionTiming==null)return "idle";
                if(activeHitBoxes.Count>0)return "active";
                if(actionTiming.hitWindows!=null&&actionTiming.hitWindows.Count>0&&player.clock<actionTiming.hitWindows[0].start)return "startup";
                return "recovery";
            }
        }
        public string CurrentCancelWindow
        {
            get
            {
                if(actionTiming==null||actionTiming.command==null)return "";
                var c=actionTiming.command;var labels=new List<string>();
                foreach(var t in c.transitions)if(player.clock>=t.bufferStart&&player.clock<t.bufferEnd)labels.Add((t.forward?">":"")+t.button+" ["+t.bufferStart.ToString("0.000")+","+t.bufferEnd.ToString("0.000")+")");
                if(c.walkCancel>=0&&player.clock>c.walkCancel)labels.Add("walk");
                if(c.dashCancel>=0&&player.clock>c.dashCancel)labels.Add("dash edge");
                return string.Join(" / ",labels.ToArray());
            }
        }
        public float NextCommandInputTime(char button,bool forward=false)
        {
            if(actionTiming==null)return 0;
            if(charging)return float.PositiveInfinity;
            var c=actionTiming.command;
            if(c==null)return Math.Max(0,actionTiming.duration*.65f-.05f);
            float earliest=float.PositiveInfinity;
            foreach(var t in c.transitions)if(t.button==button.ToString()&&(!t.forward||forward)&&player.clock<t.bufferEnd)earliest=Math.Min(earliest,t.bufferStart);
            return earliest;
        }
        private void RecordCombat(string type,int window,Enemy enemy,int damage=0,int raw=0)
        {
            combatEvents.Add(new CombatEvent{type=type,clip=player.clip,time=elapsed,clock=player.clock,playerX=player.x,playerY=player.y,target=enemy==null?"":enemy.kind,targetX=enemy==null?0:enemy.x,targetY=enemy==null?0:enemy.y,targetHp=enemy==null?0:enemy.hp,state=enemy==null?"":enemy.state.ToString(),phase=CurrentPhase,windowIndex=window,damage=damage,rawReaction=raw});
            if(combatEvents.Count>5000)combatEvents.RemoveRange(0,1000);
        }
        private void BeginCalibratedCommand(Press press)
        {
            bufferedDashTime=0;
            activeHitBoxes.Clear();sourceHitMemory.Clear();commandHitWindows.Clear();closedHitWindows.Clear();
            commandPreviousClock=0;commandPreviousX=player.x;commandPreviousY=player.y;motionEventIndex=0;
            commandPassiveX=commandPassiveY=-1;
            var c=actionTiming.command;if(c==null)return;
            // Charge is its own state. The release state receives its velocity
            // declarations only once the X level is released or 1.8s elapses.
            if(!charging){player.clock=c.startAt;commandPreviousClock=c.startAt;previousRoot=SampleMotionRoot(c,c.startAt);ApplyCommandStart(c,press.forward);}else{player.vx=0;player.vy=0;}
            RecordCombat("start",-1,null);
        }
        private void ApplyCommandStart(CommandCalibration c,bool forward)
        {
            commandPassiveX=c.passiveX;commandPassiveY=c.passiveY;
            if(c.hasStartX)player.vx=c.startX*player.facing;
            if(c.hasStartY)player.vy=c.startY;
            if(c.forwardRunStart&&forward)player.vx=207.2f*player.facing;
        }
        private void QueueCalibratedPress(char button,Input input)
        {
            if(charging)return;
            var c=actionTiming.command;bool forward=input.move*player.facing>.1f;
            CommandTransition chosen=null;
            foreach(var t in c.transitions){
                if(t.button!=button.ToString()||t.forward&&!forward||player.clock>=t.bufferEnd||
                    player.clock+RecoveryInputLead<t.bufferStart)continue;
                bool open=player.clock>=t.bufferStart;
                if(chosen==null||open&&player.clock<chosen.bufferStart||
                    open==(player.clock>=chosen.bufferStart)&&t.forward)chosen=t;
            }
            if(chosen!=null)
            {
                // Reserve one nearby edge. Once its capture window opens, it
                // follows this action until the execute frame or landing.
                var press=NewAttackPress(button,input,player.clock>=chosen.bufferStart?20:RecoveryInputLead);
                press.transition=chosen;press.fromClip=actionCommand.clip;
                presses.Clear();presses.Enqueue(press);
                RecordCombat("buffer",-1,null);return;
            }
            float acceptsNewAttack=c.attackCancel;
            if(acceptsNewAttack<0&&!c.waitForLand)acceptsNewAttack=c.endAt;
            if(acceptsNewAttack>=0&&player.clock+RecoveryInputLead>acceptsNewAttack)
            {
                presses.Clear();presses.Enqueue(NewAttackPress(button,input,RecoveryInputLead));
            }
            else RecordCombat("rejected-input",-1,null);
        }
        private bool StartCalibratedContinuation()
        {
            if(presses.Count==0)return false;var p=presses.Peek();var t=p.transition;
            if(t==null||p.fromClip!=actionCommand.clip)return false;
            if(player.clock<t.bufferStart)return false;
            p.expires=elapsed+20;
            if(t.onLand){if(!Grounded(player))return false;}else if(player.clock<=t.executeAt)return false;
            CommandDefinition next=null;
            foreach(var c in commands)if(c.clip==t.nextClip&&c.context==chainContext&&c.sequence.StartsWith(chain,StringComparison.Ordinal)){if(next==null||c.sequence.Length<next.sequence.Length)next=c;}
            if(next==null)foreach(var c in commands)if(c.clip==t.nextClip){next=c;break;}
            if(next==null){presses.Dequeue();return false;}
            if((next.context==CommandContext.Air||next.context==CommandContext.DashAir)&&Grounded(player)&&!next.landBeforeStart)return false;
            presses.Dequeue();BeginCommand(next,p);return true;
        }
        private CommandDamageStage DamageStage(CommandCalibration c,float clock)
        {
            CommandDamageStage found=null;
            foreach(var s in c.damageStages)if(clock>=s.time)found=s;
            return found;
        }
        private void TickCommandHitMemory(float dt)
        {
            if(actionTiming==null||actionTiming.command==null)return;
            var damage=DamageStage(actionTiming.command,player.clock);if(damage==null||!damage.repeat)return;
            var hitTargets=new List<Enemy>(sourceHitMemory.Keys);
            foreach(var e in hitTargets){sourceHitMemory[e]-=dt;if(sourceHitMemory[e]<=.000001f)sourceHitMemory.Remove(e);}
        }
        private void AdvanceCalibratedCommand(float dt,Input input)
        {
            var t=actionTiming;var c=t.command;activeHitBoxes.Clear();
            if(charging)
            {
                chargeTime+=dt;
                if(input.attackXReleased||!input.attackXHeld||chargeTime>=1.8f)
                {charging=false;player.clip=actionCommand.clip;player.clock=0;commandPreviousClock=0;ApplyCommandStart(c,input.move*player.facing>.1f);RecordCombat("release",-1,null);}
                else return;
            }
            if(c.applyAnimationMove&&c.motionRoot!=null&&c.motionRoot.Length>0)
            {float root=SampleMotionRoot(c,player.clock);MovePlayerAlongGround(player.x+player.facing*(root-previousRoot));previousRoot=root;}
            if(c.forwardAirControl&&input.move*player.facing>.1f)player.vx=player.facing*140*SkillMoveMultiplier;
            while(motionEventIndex<c.motionEvents.Length&&player.clock>=c.motionEvents[motionEventIndex].time)
            {
                var m=c.motionEvents[motionEventIndex++];
                if(m.mode==1){player.vx+=m.x*player.facing;player.vy+=m.y;}
                else if(m.mode==2){player.vx=m.x*player.facing;player.vy=m.y;}
                else if(m.mode==3){commandPassiveX=m.x;commandPassiveY=m.y;}
                else if(m.mode==4)player.vy=m.y;
            }
            var damage=DamageStage(c,player.clock);
            float mpGain=CommandMpGain(actionCommand.clip,damage);
            for(int windex=0;windex<t.hitWindows.Count;windex++)
            {
                var w=t.hitWindows[windex];
                if(player.clock>=w.start&&commandPreviousClock<=w.end&&w.damageDefined&&damage!=null&&damage.physicalMultiplier>0)
                {
                    float sample=Math.Min(w.end,player.clock);string[] boxes=w.enabledAttackBoxes;
                    if(boxes==null||boxes.Length==0)boxes=defaultSwordBoxes;
                    foreach(string shape in boxes)activeHitBoxes.Add(GeometryBounds(sample,t,shape,windex));
                    foreach(var e in enemies)
                    {
                        if(e.hp<=0||!e.active||e.dormant||(e.passive&&!CoreHardBuffDeviceDamageableR1417(e)&&!IsPeitaCrossbow(e)&&!IsRegionHitFixture(e))||sourceHitMemory.ContainsKey(e))continue;
                        bool contact=false;foreach(string shape in boxes)if(SweptGeometryIntersects(e,Math.Max(w.start,commandPreviousClock),sample,t,shape)){contact=true;break;}
                        if(!contact)continue;
                        var reaction=Reaction(damage.rawReaction,HitReaction.Hurt);
                        bool catching=CanCatchLaunchedEnemy(e),standing=CanStandFromSlam(e);
                        int dealt=ApplySourceHitFromXR1417(e,Math.Max(1,(int)Math.Round(damage.physicalMultiplier*20)),catching||standing?HitReaction.None:reaction,damage.backSpeedX,damage.backSpeedY,damage.stun,damage.attackerStop,damage.defenderStop,player.x,damage.rawReaction,mpGain);
                        if(dealt>0)
                        {
                            RememberCommandLaunch(e,reaction);
                            if(catching)CatchLaunchedEnemy(e);
                            else if(standing)StandFromSlam(e);
                            else if(damage.forceDown!=0&&RegisterKnockdownGauge(e,damage.forceDown)&&!EnemyPositionFixedV25(e)&&Reaction(damage.rawReaction,HitReaction.Hurt)!=HitReaction.Launch&&!EnemyHasSuperArmor(e)&&e.state!=CombatState.Knockdown&&CargoSourceGroundReactionV1468(e,HitReaction.Knockdown)==HitReaction.Knockdown)BeginKnockdown(e,false,player.x);
                            sourceHitMemory[e]=damage.repeat?Math.Max(.001f,damage.repeatInterval):float.PositiveInfinity;
                            commandHitWindows.Add(windex);actionHit=true;RecordCombat("hit",windex,e,dealt,damage.rawReaction);
                        }
                    }
                }
                if(player.clock>w.end&&!closedHitWindows.Contains(windex))
                {closedHitWindows.Add(windex);if(!commandHitWindows.Contains(windex))RecordCombat("miss",windex,null);}
            }
            commandPreviousClock=player.clock;commandPreviousX=player.x;commandPreviousY=player.y;
            bool moveCancel=CanCancelCommandMove(c,input);
            bool back=input.move*player.facing<0;
            if(moveCancel&&back&&c.backWalkCancel>=0)presses.Clear();
            if(StartCalibratedContinuation())return;
            if(skillBufferTime>0&&c.skillCancel>=0&&player.clock>c.skillCancel&&StartSkill(skillBuffered)){skillBufferTime=0;skillBuffered=-1;return;}
            if(presses.Count>0&&presses.Peek().transition==null&&c.attackCancel>=0&&player.clock>c.attackCancel){
                var press=presses.Peek();
                CancelAction(true);player.clip=Grounded(player)?"idle":"jump";player.clock=0;
                presses.Enqueue(press);
                return;
            }
            if(moveCancel)
            {CancelAction(true);player.vx=0;player.clip="idle";player.clock=0;return;}
            if(player.clock>=c.endAt&&!c.waitForLand){
                // Only fresh inputs roll into neutral recovery. A combo edge
                // still belongs to the command that recorded it.
                var attack=presses.Count>0&&presses.Peek().transition==null?presses.Peek():null;
                var jump=jumpBuffered>=JumpBufferDuration-RecoveryInputLead?jumpBuffered:0;
                int skill=skillBufferTime>=InputBufferDuration-RecoveryInputLead?skillBuffered:-1;
                float skillTime=skillBufferTime;
                CancelAction(true);
                if(attack!=null)presses.Enqueue(attack);
                jumpBuffered=jump;
                if(skill>=0){skillBuffered=skill;skillBufferTime=skillTime;}
                player.clip=Grounded(player)?"idle":"jump";player.clock=0;
            }
        }
        private static readonly string[] defaultSwordBoxes={"Sword"};
        private readonly AttackGeometry playerGeometrySample = new AttackGeometry();
        private AttackGeometry GeometryAt(float clock,ClipTiming timing)
        {
            if(timing.geometry==null||timing.geometry.Count==0)return null;
            if(timing.loop&&timing.duration>0)clock%=timing.duration;
            float frame=Clamp(clock*timing.fps,0,timing.geometry.Count-1);int i=(int)frame,j=Math.Min(i+1,timing.geometry.Count-1);float f=frame-i;
            var a=timing.geometry[i];var b=timing.geometry[j];
            // No caller retains this interpolated pose across another sample.
            playerGeometrySample.weaponBaseX=a.weaponBaseX+(b.weaponBaseX-a.weaponBaseX)*f;
            playerGeometrySample.weaponBaseY=a.weaponBaseY+(b.weaponBaseY-a.weaponBaseY)*f;
            playerGeometrySample.weaponStartX=a.weaponStartX+(b.weaponStartX-a.weaponStartX)*f;
            playerGeometrySample.weaponStartY=a.weaponStartY+(b.weaponStartY-a.weaponStartY)*f;
            playerGeometrySample.weaponEndX=a.weaponEndX+(b.weaponEndX-a.weaponEndX)*f;
            playerGeometrySample.weaponEndY=a.weaponEndY+(b.weaponEndY-a.weaponEndY)*f;
            playerGeometrySample.radius=a.radius;
            playerGeometrySample.rightFootStartX=a.rightFootStartX+(b.rightFootStartX-a.rightFootStartX)*f;
            playerGeometrySample.rightFootStartY=a.rightFootStartY+(b.rightFootStartY-a.rightFootStartY)*f;
            playerGeometrySample.rightFootEndX=a.rightFootEndX+(b.rightFootEndX-a.rightFootEndX)*f;
            playerGeometrySample.rightFootEndY=a.rightFootEndY+(b.rightFootEndY-a.rightFootEndY)*f;
            playerGeometrySample.leftFootStartX=a.leftFootStartX+(b.leftFootStartX-a.leftFootStartX)*f;
            playerGeometrySample.leftFootStartY=a.leftFootStartY+(b.leftFootStartY-a.leftFootStartY)*f;
            playerGeometrySample.leftFootEndX=a.leftFootEndX+(b.leftFootEndX-a.leftFootEndX)*f;
            playerGeometrySample.leftFootEndY=a.leftFootEndY+(b.leftFootEndY-a.leftFootEndY)*f;
            return playerGeometrySample;
        }
        private float SampleMotionRoot(CommandCalibration c,float clock)
        {
            var samples=c.motionRoot;if(samples.Length==0)return 0;
            for(int i=1;i<samples.Length;i++)if(clock<samples[i].time)
            {var a=samples[i-1];var b=samples[i];float u=Clamp((clock-a.time)/Math.Max(.0001f,b.time-a.time),0,1);return a.x+(b.x-a.x)*u;}
            return samples[samples.Length-1].x;
        }
        private void Segment(AttackGeometry g,string shape,float x,float y,out float ax,out float ay,out float bx,out float by)
        {
            bool r=shape=="Rfoot",l=shape=="Lfoot";
            ax=x+player.facing*(r?g.rightFootStartX:l?g.leftFootStartX:g.weaponStartX);ay=y+(r?g.rightFootStartY:l?g.leftFootStartY:g.weaponStartY);
            bx=x+player.facing*(r?g.rightFootEndX:l?g.leftFootEndX:g.weaponEndX);by=y+(r?g.rightFootEndY:l?g.leftFootEndY:g.weaponEndY);
            if(!r&&!l&&ArmageddonActive)
            {
                // Native attached-sword helper projection, not a global hitbox
                // multiplier. Feet and independent skill effect meshes stay intact.
                float dx=bx-ax,dy=by-ay,length=(float)Math.Sqrt(dx*dx+dy*dy);
                if(length>.001f){dx/=length;dy/=length;float handX=x+player.facing*g.weaponBaseX,handY=y+g.weaponBaseY;ax=handX+dx*ArmageddonAttackStart;ay=handY+dy*ArmageddonAttackStart;bx=handX+dx*ArmageddonAttackEnd;by=handY+dy*ArmageddonAttackEnd;}
            }
        }
        private AttackBox GeometryBounds(float clock,ClipTiming timing,string shape,int window)
        {
            var g=GeometryAt(clock,timing);var box=new AttackBox{shape=shape,windowIndex=window};if(g==null)return box;
            float ax,ay,bx,by;Segment(g,shape,player.x,player.y,out ax,out ay,out bx,out by);
            box.left=Math.Min(ax,bx)-g.radius;box.right=Math.Max(ax,bx)+g.radius;box.bottom=Math.Min(ay,by)-g.radius;box.top=Math.Max(ay,by)+g.radius;return box;
        }
        private AttackBox EnemyBounds(Enemy target)
        {
            AttackBox source;if(SourceEnemyBounds(target,out source))return source;
            // Pixel monster hurtboxes are explicit demo adaptation. There is no
            // assertion that these are the unavailable native monster callbacks.
            bool down=target.state==CombatState.Knockdown;
            float width=down?24:target.kind=="white-giant"?24:IsBoss(target.kind)?18:target.kind=="chicken"?13:12;
            float height=down?14:target.kind=="white-giant"?72:IsBoss(target.kind)?58:target.kind=="chicken"?30:48;
            width*=target.visualScale;height*=target.visualScale;
            float bottom=target.y+(down?0:NativeHoverLiftV58(target));
            return new AttackBox{left=target.x-width,right=target.x+width,bottom=bottom,top=bottom+height};
        }
        private void BlockPlayerBodies(float oldX){BlockPlayerBodies(oldX,Grounded(player)?GroundAt(oldX):player.y);}
        private void BlockPlayerBodies(float oldX,float oldY)
        {MoveAlongGround(player,ClipPlayerBodyMovement(oldX,oldY,player.x,player.y));}
        private void MovePlayerAlongGround(float target)
        {
            if(actionSkill!=null&&skillForwardBlocked&&(target-player.x)*player.facing>0){
                if(player.vx*player.facing>0)player.vx=0;
                return;
            }
            if(TryCargoProjectionLinkMoveR1479(player,target))return;
            float oldX=player.x,oldY=player.y,oldVy=player.vy;int oldLine=player.groundLine;CombatState oldState=player.state;
            MoveAlongGround(player,target);
            float clipped=ClipPlayerBodyMovement(oldX,oldY,player.x,player.y);
            TraceNativeMovementV17(player,"body-clip",oldX,oldY,clipped);
            if(Math.Abs(clipped-player.x)<.00001f)return;
            // A body stops the current skill lunge. Discard forward momentum
            // instead of releasing it when the blocking enemy dies.
            if(actionSkill!=null&&(player.x-oldX)*player.facing>0){
                skillForwardBlocked=true;
                if(player.vx*player.facing>0)player.vx=0;
            }
            // Roll back the speculative support change before committing the
            // reachable displacement. A blocked lunge must not borrow a higher
            // platform's Y or fall after a ledge it never actually reached.
            player.x=oldX;player.y=oldY;player.vy=oldVy;player.groundLine=oldLine;player.state=oldState;
            MoveAlongGround(player,clipped);
        }
        private float ClipPlayerBodyMovement(float oldX,float oldY,float wanted,float wantedY)
        {
            if(actionSkill!=null&&SkillAllowsPass)return wanted;
            // Source UNIT_WIDTH=70 and UNIT_HEIGHT=150, scaled by .28.
            // Opposing monster bodies remain the documented pixel adaptation.
            // Clip intended displacement at first contact. Existing overlap can
            // escape, but never causes backwards snapping or order-dependent
            // cumulative depenetration. Test vertical overlap at contact time.
            float delta=wanted-oldX,clipped=wanted;if(Math.Abs(delta)<.00001f)return wanted;
            foreach(var e in enemies)
            {
                if(!e.active||e.dormant||e.hp<=0||e.passive||EnemyInitialHurtboxHiddenV25(e)||CoreArenaBodyPassableV59(e)||IsRecoveryContainer(e))continue;
                // ESSI_KNIGHT_DASH_COMBO_Z3 declares CAN_PASS_UNIT=1. Only the
                // finisher crosses opposing actors; gates and level devices stay solid.
                if(actionCommand!=null&&actionCommand.clip=="dash-z3"&&IsCombatMonster(e)&&
                   !IsRegionHitFixture(e)&&string.IsNullOrEmpty(NpcProfile(e)?.environmentKind))continue;
                // A hit can knock the target down before a skill's remaining
                // root/velocity movement. Keep that living body solid for the
                // rest of the cast; ordinary walking can still step past it.
                if(e.state==CombatState.Knockdown&&actionSkill==null)continue;
                if(PlainOriginalShockStickPassableR1475(e))continue;
                bool pressCanPass;if(TryReadFactoryPressPassabilityV1469(e,out pressCanPass)&&pressCanPass)continue;
                var b=EnemyBlockingBounds(e);
                float center=(b.left+b.right)*.5f;
                // For the forward Armor Break lunge, a turning monster's asymmetrical
                // pose must not switch sides while the two actors already overlap.
                bool armorLunge=actionSkill!=null && actionSkill.id=="armor-break" && delta*player.facing>0;
                if(armorLunge)center=e.x;
                if(delta>0&&(oldX<center || armorLunge&&oldX==center))
                {
                    float boundary=b.left-9.8f;if(wanted<=boundary)continue;
                    float contact=Math.Max(oldX,boundary),u=Clamp((contact-oldX)/delta,0,1),atY=oldY+(wantedY-oldY)*u;
                    if(atY<b.top&&atY+42>b.bottom)clipped=Math.Min(clipped,contact);
                }
                else if(delta<0&&(oldX>center || armorLunge&&oldX==center))
                {
                    float boundary=b.right+9.8f;if(wanted>=boundary)continue;
                    float contact=Math.Min(oldX,boundary),u=Clamp((contact-oldX)/delta,0,1),atY=oldY+(wantedY-oldY)*u;
                    if(atY<b.top&&atY+42>b.bottom)clipped=Math.Max(clipped,contact);
                }
            }
            if(NativeTerrain&&Math.Abs(clipped-wanted)>.00001f)clipped=NativeBodyBlockedStop(oldX,oldY,wantedY,clipped);
            return clipped;
        }
        private bool AreaIntersects(Enemy target,float left,float bottom,float right,float top)
        {
            var spheres=NpcHurtSpheres(target);if(spheres==null){var b=EnemyBounds(target);return right>=b.left&&left<=b.right&&top>=b.bottom&&bottom<=b.top;}
            for(int i=0;i<spheres.Length;i+=4){float x,y,r;if(!NpcCircle(target,spheres,i,out x,out y,out r))continue;float dx=x-Clamp(x,left,right),dy=y-Clamp(y,bottom,top);if(dx*dx+dy*dy<=r*r)return true;}return false;
        }
        private static bool LineHitsBox(float ax,float ay,float bx,float by,AttackBox box,float radius)
        {
            float min=0,max=1,dx=bx-ax,dy=by-ay;
            return ClipLine(-dx,ax-box.left+radius,ref min,ref max)&&ClipLine(dx,box.right+radius-ax,ref min,ref max)&&ClipLine(-dy,ay-box.bottom+radius,ref min,ref max)&&ClipLine(dy,box.top+radius-ay,ref min,ref max);
        }
        private static bool ClipLine(float p,float q,ref float min,ref float max)
        {if(Math.Abs(p)<.000001f)return q>=0;float t=q/p;if(p<0){if(t>max)return false;if(t>min)min=t;}else{if(t<min)return false;if(t<max)max=t;}return true;}
        private bool WeaponIntersects(Enemy target,float clock,ClipTiming timing){return GeometryIntersects(target,clock,timing,"Sword");}
        private bool GeometryIntersects(Enemy target,float clock,ClipTiming timing,string shape)
        {var g=GeometryAt(clock,timing);if(g==null)return false;float ax,ay,bx,by;Segment(g,shape,player.x,player.y,out ax,out ay,out bx,out by);return EnemySegmentIntersects(target,ax,ay,bx,by,g.radius);}
        private bool SweptGeometryIntersects(Enemy target,float from,float to,ClipTiming timing,string shape)
        {
            if(timing.geometry==null||timing.geometry.Count==0)return false;
            // Sample the blade sweep within this physics packet, including the
            // source attack-window boundary; never extend to inactive frames.
            int samples=Math.Max(1,(int)Math.Ceiling((to-from)*timing.fps*3));
            float pax=0,pay=0,pbx=0,pby=0;
            for(int i=0;i<=samples;i++)
            {
                float u=(float)i/samples,clock=from+(to-from)*u;var g=GeometryAt(clock,timing);float ax,ay,bx,by;
                Segment(g,shape,commandPreviousX+(player.x-commandPreviousX)*u,commandPreviousY+(player.y-commandPreviousY)*u,out ax,out ay,out bx,out by);
                if(EnemySegmentIntersects(target,ax,ay,bx,by,g.radius)||(i>0&&(EnemySegmentIntersects(target,pax,pay,ax,ay,g.radius)||EnemySegmentIntersects(target,pbx,pby,bx,by,g.radius))))return true;
                pax=ax;pay=ay;pbx=bx;pby=by;
            }
            return false;
        }
        private int ApplySourceHit(Enemy target,int damage,HitReaction reaction,float backSpeedX,float backSpeedY,float stun,float attackerStop,float defenderStop){return ApplySourceHitFromXR1417(target,damage,reaction,backSpeedX,backSpeedY,stun,attackerStop,defenderStop,player.x);}
        private static float CommandMpGain(string clip,CommandDamageStage damage)
        {
            if(damage==null||!damage.repeat)return DefaultHitMpGain;
            switch(clip){
                case "dash-air-z3":
                case "dash-zx":
                case "dash-zxx":
                case "dash-xx":
                case "dash-xxx":
                    return DefaultHitMpGain*.5f;
                default:
                    return DefaultHitMpGain;
            }
        }
        private int ApplySourceHitFromXR1417(Enemy target,int damage,HitReaction reaction,float backSpeedX,float backSpeedY,float stun,float attackerStop,float defenderStop,float attackerX,int rawReaction=0,float mpGain=DefaultHitMpGain)
        {
            reaction=Mine2RailReactionV21(target,reaction);bool fixedPosition=EnemyPositionFixedV25(target);bool protectedState=target.isGate||target.state==CombatState.Knockdown||EnemyHasSuperArmor(target)||CoreHardShieldGuardingR1417(target,attackerX);
            float previousVy=target.vy;bool wasAir=!Grounded(target);
            int dealt=ApplyHitToEnemyWithMp(target,damage,reaction,backSpeedY,attackerX,rawReaction,mpGain);
            if(dealt<=0)return 0;
            if(!protectedState&&!fixedPosition&&!IsPeitaCrossbow(target)&&!IsRegionHitFixture(target)&&!PeitaTowerDragon(target)&&target.hp>0&&target.state!=CombatState.Knockdown)
            {
                int direction=target.x>=attackerX?1:-1;target.vx=direction*backSpeedX;
                if(reaction==HitReaction.Launch||reaction==HitReaction.Knockdown&&wasAir)
                {target.vy=backSpeedY;target.state=CombatState.Airborne;target.knockdownOnLanding=Remaining3AllowsKnockdownV28(target);target.clip="launched";target.stun=Math.Max(stun,.05f);}
                else if(reaction==HitReaction.Hurt)
                {
                    // Fixed-V5 selectHitReaction: near-floor Small/Big retains
                    // falling Y speed. It interrupts the old launcher state;
                    // it must not inherit its forced knockdown-on-landing flag.
                    // An absent extra stun debuff does not cancel the ordinary
                    // hurt reaction established by ApplyHit. Previously an
                    // area hit with stun=0 returned to idle on the next tick.
                    target.knockdownOnLanding=false;target.vy=Math.Min(0,previousVy);target.stun=Math.Max(target.stun,stun);target.state=CombatState.Hurt;target.clip="hurt";
                    float hurtFloor;
                    if(TryEnemyHurtFloor(target,out hurtFloor)&&target.y-hurtFloor>200*NativeToLogical)
                    {target.y-=45*NativeToLogical;target.vy+=700*NativeToLogical;target.vx+=direction*140*.33f;target.state=CombatState.Airborne;}
                }
                else{target.vy=0;target.stun=stun;}
            }
            hitstop=Math.Max(hitstop,attackerStop);
            if(!protectedState)target.hitPause=Math.Max(target.hitPause,Math.Max(defenderStop,PeitaIgnoresSkillStop(target)?0:SkillWorldStopRemaining));
            SampleCoreHardHitDisplayV39(target);shake=Math.Max(shake,2);return dealt;
        }
    }
}
