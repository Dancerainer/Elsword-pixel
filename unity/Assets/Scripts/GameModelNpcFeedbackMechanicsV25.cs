using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  // Position constraints do not grant armor or damage immunity. Explicit source
  // callbacks may still place an actor; only generic physics/root/push is held.
  readonly HashSet<Enemy> fixedNpcReactionsV25=new HashSet<Enemy>();
  readonly HashSet<Enemy> feedbackMiniUpInitialEnteredV25=new HashSet<Enemy>();
  sealed class InitialNpcPoseV25 {
   internal int phase,support=-1;
   internal float time,decision,rootY;
   internal bool tree,done,preserveDisabledCombatAi;
  }
  readonly Dictionary<Enemy,InitialNpcPoseV25> initialNpcPosesV25=new Dictionary<Enemy,InitialNpcPoseV25>();
  public const float TreeKnightStartDurationV25=266f/60f;
  // User-feedback presentation/contact adapter, not a native SHOW event.
  public const float TreeKnightRevealStartV25=3.5f;
  public const float BatReleaseDurationV25=96f/60f;

  void ResetNpcFeedbackMechanicsV25(){
   fairyRearReactionsV34.Clear();fixedNpcReactionsV25.Clear();initialNpcPosesV25.Clear();feedbackMiniUpInitialEnteredV25.Clear();
   foreach(var enemy in enemies){EnsureInitialNpcPoseV25(enemy);HoldNpcFixedPhysicsV25(enemy);}
  }
  InitialNpcPoseV25 EnsureInitialNpcPoseV25(Enemy enemy){
   if(enemy==null)return null;
   InitialNpcPoseV25 pose;if(initialNpcPosesV25.TryGetValue(enemy,out pose))return pose;
   bool tree=enemy.sourceNPC=="NUI_TREE_KNIGHT";
   if(!tree&&enemy.sourceNPC!="NUI_BAT_A"&&enemy.sourceNPC!="NUI_BAT_B")return null;
   pose=new InitialNpcPoseV25{tree=tree,preserveDisabledCombatAi=tree&&!enemy.aiEnabled&&!enemy.passive,rootY=enemy.y,support=enemy.groundLine};
   if(!tree){
    // The native bat spawn root is ON its authored line. WaitZero geometry
    // hangs below that root; this is head support, never foot-ground contact.
    var support=Surface(enemy.groundLine);
    bool namedPlatform=support!=null&&support.platform&&enemy.x>=support.x1-.02f&&enemy.x<=support.x2+.02f;
    // Current main-catalog Cave births retain a historical render-foot
    // projection: rootY=lineY-UNIT_HEIGHT*.28/2 (all eight are exactly21px).
    // Accept only that exact half-height delta on the declared source line;
    // never reparent an arbitrary airborne/floor bat by proximity.
    var profile=NpcProfile(enemy);
    float halfHeight=(profile==null?150*NativeToLogical:profile.bodyHeight)*enemy.visualScale*.5f;
    float delta=namedPlatform?support.Y(enemy.x)-enemy.y:float.PositiveInfinity;
    bool exactNativeRoot=namedPlatform&&Math.Abs(delta)<=.1f;
    bool exactLegacyFeet=namedPlatform&&Math.Abs(delta-halfHeight)<=.1f;
    if(!exactNativeRoot&&!exactLegacyFeet){pose.done=true;}
    else{pose.support=support.id;pose.rootY=support.Y(enemy.x);}
   }
   initialNpcPosesV25.Add(enemy,pose);
   if(!pose.done){enemy.y=pose.rootY;enemy.vx=enemy.vy=0;enemy.state=CombatState.Normal;enemy.clip="source-start";enemy.clock=0;}
   return pose;
  }
  bool PreserveOriginalDisabledTreeCombatAiV25(Enemy enemy){
   InitialNpcPoseV25 pose;return enemy!=null&&initialNpcPosesV25.TryGetValue(enemy,out pose)&&pose.preserveDisabledCombatAi;
  }
  public bool EnemyHangingV25(Enemy enemy){
   InitialNpcPoseV25 pose;return enemy!=null&&initialNpcPosesV25.TryGetValue(enemy,out pose)&&!pose.done&&!pose.tree&&pose.phase==0;
  }
  public string EnemyInitialPosePhaseV25(Enemy enemy){
   var pose=EnsureInitialNpcPoseV25(enemy);if(pose==null)return "none";if(pose.done)return "complete";
   return pose.tree?(pose.phase==0?"buried-wait":"emerging"):(pose.phase==0?"hanging":"releasing");
  }
  public float EnemyInitialPoseElapsedV25(Enemy enemy){var pose=EnsureInitialNpcPoseV25(enemy);return pose==null?0:pose.time;}
  public float EnemyInitialPoseClipClockV25(Enemy enemy){var pose=EnsureInitialNpcPoseV25(enemy);return pose==null?0:!pose.tree&&!pose.done&&pose.phase==0?pose.time%2.1f:pose.time;}
  public string EnemyInitialPoseSourceStateV25(Enemy enemy){
   var pose=EnsureInitialNpcPoseV25(enemy);if(pose==null||pose.done)return "";
   return pose.tree?"TREE_KNIGHT_START":enemy.sourceNPC.Substring(4)+(pose.phase==0?"_START":"_WAIT_ZERO2");
  }
  public float EnemyInitialSupportYV25(Enemy enemy){var pose=EnsureInitialNpcPoseV25(enemy);if(pose==null)return enemy==null?0:enemy.y;var support=Surface(pose.support);return support==null?pose.rootY:support.Y(enemy.x);}
  public int EnemyHangingSupportV25(Enemy enemy){InitialNpcPoseV25 pose;return enemy!=null&&initialNpcPosesV25.TryGetValue(enemy,out pose)&&EnemyHangingV25(enemy)?pose.support:-1;}
  public bool EnemyInitialPresentationVisibleV25(Enemy enemy){
   if(enemy==null)return false;if(enemy.hp<=0)return true;
   var pose=EnsureInitialNpcPoseV25(enemy);
   return pose==null||pose.done||!pose.tree||pose.phase>0&&pose.time>=TreeKnightRevealStartV25;
  }
  public float EnemyInitialRevealProgressV25(Enemy enemy){
   var pose=EnsureInitialNpcPoseV25(enemy);
   if(pose==null||pose.done||!pose.tree||enemy.hp<=0)return 1;
   return pose.phase==0?0:Clamp((pose.time-TreeKnightRevealStartV25)/(TreeKnightStartDurationV25-TreeKnightRevealStartV25),0,1);
  }
  public bool EnemyInitialHurtboxHiddenV25(Enemy enemy){
   if(SwampActorHiddenV39(enemy))return true;
   var pose=EnsureInitialNpcPoseV25(enemy);
   // No contact body exists while completely buried. This is not an
   // INVINCIBLE timer; exposed emergence can take HP damage under true armor.
   return enemy!=null&&enemy.hp>0&&pose!=null&&!pose.done&&pose.tree&&(pose.phase==0||pose.time<TreeKnightRevealStartV25);
  }
  public bool EnemyInitialSuperArmorV25(Enemy enemy){
   if(enemy==null||enemy.hp<=0)return false;
   // The Guardian's wake animation is one uninterrupted ambush. Key this
   // gameplay rule to the actual state so armor ends on the transition to Wait.
   if(enemy.state==CombatState.Normal&&enemy.npcState==GuardianCombatNativeRulesV28.TrapStart
      &&GuardianCombatNativeRulesV28.Identity(enemy.kind,enemy.sourceNPC))return true;
   var pose=EnsureInitialNpcPoseV25(enemy);
   // Explicit user-requested adapter. Native TREE_KNIGHT_START does not
   // declare SUPER_ARMOR; do not misrepresent this feedback fix as source data.
   return pose!=null&&!pose.done&&pose.tree;
  }
  public bool EnemyPositionFixedV25(Enemy enemy){
   if(enemy==null)return false;
   if(PreservePlainShockStickAiR1477(enemy))return true;
   if(enemy.isGate||IsRecoveryContainer(enemy)||EnemyHangingV25(enemy))return true;
   InitialNpcPoseV25 pose;
   if(initialNpcPosesV25.TryGetValue(enemy,out pose)&&!pose.done&&pose.tree)return true;
   if(fixedNpcReactionsV25.Contains(enemy)){
    // Hold through the short empty-state handoff. Otherwise gravity could
    // displace a fixed actor before its ordinary WAIT is entered again.
    if(enemy.state==CombatState.Hurt||NpcState(enemy)==null)return true;
    fixedNpcReactionsV25.Remove(enemy);
   }
   var state=NpcState(enemy);
   if(state!=null)return state.neverMove;
   return sourcePermanentPositionNpcsV25.Contains(enemy.sourceNPC);
  }
  bool HoldNpcFixedPhysicsV25(Enemy enemy){
   if(!EnemyPositionFixedV25(enemy))return false;
   enemy.vx=enemy.vy=enemy.npcVelocityX=0;enemy.knockdownOnLanding=false;
   if(enemy.hp>0&&enemy.state==CombatState.Airborne)enemy.state=CombatState.Normal;
   return true;
  }
  void RememberFixedNpcReactionV25(Enemy enemy){if(enemy!=null)fixedNpcReactionsV25.Add(enemy);}
  float HangingNpcBodyHeightV25(Enemy enemy){var profile=NpcProfile(enemy);return (profile==null?42:profile.bodyHeight)*enemy.visualScale;}
  void ReleaseInitialNpcPoseForHitV25(Enemy enemy){
   InitialNpcPoseV25 pose;
   if(enemy==null||!initialNpcPosesV25.TryGetValue(enemy,out pose)||pose.done||pose.tree)return;
   pose.done=true;enemy.npcState="";enemy.npcTime=0;enemy.groundLine=-1;
   // The source departure ADD_POS_Y=-45 uses the existing .28 projection.
   enemy.y-=45*NativeToLogical;
  }
  // Candidate only: use the already loaded exact native START, not invented
  // timing, immunity, geometry or art. Other NPCs/directors remain untouched.
  bool FeedbackMiniUpSourceStartActiveV25(Enemy e){return e!=null&&e.sourceNPC=="NUI_CANNON_MINI_UP"&&e.npcState=="CANNON_MINI_UP_START"&&feedbackMiniUpInitialEnteredV25.Contains(e);}
  void EnsureFeedbackMiniUpSourceStartV25(Enemy e){
   if(e==null||e.sourceNPC!="NUI_CANNON_MINI_UP"||!belowMiniUpWaitEnabledV20||!e.active||e.dormant||!e.aiEnabled||e.passive||e.hp<=0||feedbackMiniUpInitialEnteredV25.Contains(e))return;
   if(!string.IsNullOrEmpty(e.npcState)){feedbackMiniUpInitialEnteredV25.Add(e);return;}
   NpcStateDefinition start;if(!npcStates.TryGetValue(e.sourceNPC+"/CANNON_MINI_UP_START",out start))throw new InvalidOperationException("Missing exact bound MiniUP source START");
   if(TryEnterNpcState(e,start))feedbackMiniUpInitialEnteredV25.Add(e);
  }
  bool TickInitialNpcPoseV25(Enemy enemy,float dt){
   EnsureFeedbackMiniUpSourceStartV25(enemy);
   var pose=EnsureInitialNpcPoseV25(enemy);
   if(pose==null||pose.done||!enemy.active||enemy.dormant||enemy.hp<=0)return false;
   if(enemy.passive||!enemy.aiEnabled&&!pose.preserveDisabledCombatAi)return true;
   if(enemy.hitPause>0){float stop=Math.Min(dt,enemy.hitPause);enemy.hitPause-=stop;dt-=stop;if(dt<=0)return true;}
   if(pose.tree){
    enemy.vx=enemy.vy=0;enemy.state=CombatState.Normal;enemy.clip="source-start";
    // Nearby activation preserves the underground opening offscreen. The
    // subsequent WaitHabit duration is the actual source 21280/4800 seconds.
    if(pose.phase==0){if(Math.Abs(player.x-enemy.x)>enemy.aggroRange)return true;pose.phase=1;enemy.alerted=true;}
    pose.time=Math.Min(TreeKnightStartDurationV25,pose.time+dt);enemy.clock=pose.time;
    if(pose.time+1e-6f>=TreeKnightStartDurationV25){pose.done=true;enemy.npcState="";enemy.npcTime=0;enemy.clip="idle";enemy.clock=0;}
    return true;
   }
   if(pose.phase==0){
    var support=Surface(pose.support);
    if(support==null||enemy.x<support.x1-.02f||enemy.x>support.x2+.02f){pose.done=true;enemy.groundLine=-1;return false;}
    enemy.y=support.Y(enemy.x);enemy.groundLine=support.id;enemy.vx=enemy.vy=0;enemy.state=CombatState.Normal;enemy.clip="source-start";
    pose.time+=dt;enemy.clock=pose.time;pose.decision+=dt;
    if(pose.decision+1e-6f>=1){
     pose.decision%=1;
     float dx=player.x-enemy.x,dy=player.y-enemy.y;
     if(dx*dx+dy*dy<=500*NativeToLogical*500*NativeToLogical){
      pose.phase=1;pose.time=0;enemy.clock=0;enemy.clip="source-wait-zero2";enemy.y-=45*NativeToLogical;enemy.groundLine=-1;enemy.state=CombatState.Airborne;
     }
    }
    return true;
   }
   // WAIT_ZERO2 overrides gravity to source2000*.28; it must not land back
   // onto the departed upper line. SurfaceBelow tests the pre-step root.
   float before=enemy.y;enemy.vy=Math.Max(-560,enemy.vy-2000*NativeToLogical*dt);enemy.y+=enemy.vy*dt;
   var floor=NativeTerrain?SurfaceBelow(enemy.x,before+.02f):null;
   float floorY=floor==null?(NativeTerrain?float.NegativeInfinity:GroundAt(enemy.x)):floor.Y(enemy.x);
   if(enemy.y<=floorY){enemy.y=floorY;enemy.vy=0;enemy.groundLine=floor==null?-1:floor.id;enemy.state=CombatState.Normal;}
   pose.time+=dt;enemy.clock=pose.time;enemy.clip="source-wait-zero2";
   if(pose.time+1e-6f>=BatReleaseDurationV25){pose.done=true;enemy.npcState="";enemy.npcTime=0;enemy.clock=0;enemy.clip=Grounded(enemy)?"idle":"jump";}
   return true;
  }
 }
}
