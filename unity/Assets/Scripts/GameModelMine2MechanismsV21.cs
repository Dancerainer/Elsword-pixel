using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class Mine2LineGroupV21 {public string roomId;public int[] surfaceIds;public float startX,startY,endX,endY;public float[] nativeStart,nativeEnd;}
 [Serializable] public sealed class Mine2StoneBirthV21 {public string roomId;public int spawnIndex,line,addNativeY,sourceRate;public float x,baseY;public bool sourceActive;}
 [Serializable] public sealed class Mine2MechanismDocumentV21 {
  public string schema,sourceAuditSha256,sourceModelSha256,railCallbackSha256,lineGroupAdapter,stoneSpawnAdapter,wallyTimerDefault,missingWaitPolicy;
  public bool runtimeAdmitted,nativeExecuted;public float minimumRailStateTime,railDistanceNativeExclusive,stoneGravityNative,stoneTerminalNative,stoneInterval,stoneRate,stoneNearNative,stoneWarningSeconds;
  public Mine2LineGroupV21[] lineGroups;public Mine2StoneBirthV21[] stoneBirths;
 }
 [Serializable] public sealed class Mine2MechanismEventV21 {public string type,npc,state;public float time,x,y,vy,amount;public int line,facing,spawnIndex;}
 public sealed partial class GameModel {
  Mine2MechanismDocumentV21 mine2MechanismsV21;
  sealed class Mine2StoneActorV21 {internal Mine2StoneBirthV21 birth;internal float age,decision;internal int facing;internal bool hitPlayer,dust;internal readonly HashSet<Enemy> hitEnemies=new HashSet<Enemy>();}
  readonly Dictionary<Enemy,Mine2StoneActorV21> mine2StonesV21=new Dictionary<Enemy,Mine2StoneActorV21>();
  public readonly List<Mine2MechanismEventV21> mine2MechanismEventsV21=new List<Mine2MechanismEventV21>();
  public Func<float> Mine2StoneRollOverrideV21;
  public void SetMine2MechanismsV21(Mine2MechanismDocumentV21 d){
   if(d!=null&&string.IsNullOrEmpty(d.schema)&&string.IsNullOrEmpty(d.sourceAuditSha256)&&string.IsNullOrEmpty(d.sourceModelSha256)&&string.IsNullOrEmpty(d.railCallbackSha256)&&string.IsNullOrEmpty(d.lineGroupAdapter)&&string.IsNullOrEmpty(d.stoneSpawnAdapter)&&string.IsNullOrEmpty(d.wallyTimerDefault)&&string.IsNullOrEmpty(d.missingWaitPolicy)&&!d.runtimeAdmitted&&!d.nativeExecuted&&d.minimumRailStateTime==0&&d.railDistanceNativeExclusive==0&&d.stoneGravityNative==0&&d.stoneTerminalNative==0&&d.stoneInterval==0&&d.stoneRate==0&&d.stoneNearNative==0&&d.stoneWarningSeconds==0&&NoNpcRows(d.lineGroups)&&NoNpcRows(d.stoneBirths))d=null;
   if(d!=null){
    if(d.schema!="mine2-mechanisms-runtime-draft-v21"||d.runtimeAdmitted||d.nativeExecuted||!SourceSupportHashV17(d.sourceAuditSha256)||!SourceSupportHashV17(d.sourceModelSha256)||d.railCallbackSha256!="6ebdf505cd19b1975ae5bb0eaa0e0850ded375e17241eefa31b0dbcb08727b66"||d.lineGroupAdapter!="source-before-next-chain-projected2d-v21"||d.stoneSpawnAdapter!="source-add-pos-y-unit28-once-v21"||d.minimumRailStateTime!=.1f||d.railDistanceNativeExclusive!=280||d.stoneGravityNative!=4000||d.stoneTerminalNative!=2000||d.stoneInterval!=2||d.stoneRate!=50||d.stoneNearNative!=2000||d.stoneWarningSeconds!=2||d.wallyTimerDefault!="verified-shared-native-constructor-zero-enabled-v21"||d.missingWaitPolicy!="report-unresolved-no-foreign-callback-no-timer-restart-v21"||d.lineGroups==null||d.lineGroups.Length==0||d.stoneBirths==null||d.stoneBirths.Length==0)throw new ArgumentException("Changed Mine2 source mechanism/adapter contract");
    var seen=new HashSet<string>();foreach(var g in d.lineGroups){if(g==null||string.IsNullOrEmpty(g.roomId)||g.surfaceIds==null||g.surfaceIds.Length==0||g.nativeStart==null||g.nativeEnd==null||g.nativeStart.Length!=3||g.nativeEnd.Length!=3||!Finite(g.startX)||!Finite(g.startY)||!Finite(g.endX)||!Finite(g.endY)||g.endX<g.startX)throw new ArgumentException("Invalid original Mine2 line chain");foreach(int id in g.surfaceIds)if(!seen.Add(g.roomId+"/"+id))throw new ArgumentException("Ambiguous original Mine2 support chain");}
    seen.Clear();foreach(var b in d.stoneBirths)if(b==null||string.IsNullOrEmpty(b.roomId)||!Finite(b.x)||!Finite(b.baseY)||b.line<0||b.sourceActive||b.sourceRate!=40||(b.addNativeY!=800&&b.addNativeY!=1100&&b.addNativeY!=1200)||!seen.Add(b.roomId+"/"+b.spawnIndex))throw new ArgumentException("Changed original Mine2 suspended spawn declaration");
   }
   mine2MechanismsV21=d;Mine2VoidDefeatsV21=0;ClearMine2MechanismsRoomV21();
  }
  void ClearMine2MechanismsRoomV21(){mine2StonesV21.Clear();mine2MechanismEventsV21.Clear();}
  void Mine2EventV21(Enemy e,string type,float at,float amount=0,int spawn=-1){if(mine2MechanismEventsV21.Count>=4096)mine2MechanismEventsV21.RemoveAt(0);mine2MechanismEventsV21.Add(new Mine2MechanismEventV21{type=type,npc=e.sourceNPC,state=e.npcState,time=at,x=e.x,y=e.y,vy=e.vy,line=e.groundLine,facing=e.facing,amount=amount,spawnIndex=spawn});}
  bool Mine2RailBoundV21(Enemy e){return mine2MechanismsV21!=null&&e!=null&&e.sourceNPC=="NUI_NASOD_RAIL";}
  bool Mine2RailLoopV21(NpcStateDefinition s){return mine2MechanismsV21!=null&&s.id=="NASOD_RAIL_DASH_ATTACK";}
  public static bool SourceMine2RailTurnV21(float time,int facing,float x,float y,float startX,float startY,float endX,float endY){
   foreach(float v in new[]{time,x,y,startX,startY,endX,endY})if(!Finite(v))throw new ArgumentException("Invalid Mine2 rail predicate input");if(facing!=1&&facing!=-1)throw new ArgumentException("Invalid original rail direction");if(time<.1f)return false;float dx=x-(facing>0?endX:startX),dy=y-(facing>0?endY:startY);return dx*dx+dy*dy<(280*.28f)*(280*.28f);
  }
  bool AdvanceMine2RailTransitionV21(Enemy e,NpcStateDefinition state,float to){
   if(!Mine2RailBoundV21(e)||!Mine2RailLoopV21(state)||e.hp<=0)return false;
   var g=Array.Find(mine2MechanismsV21.lineGroups,q=>q.roomId==CurrentRoom.id&&Array.IndexOf(q.surfaceIds,e.groundLine)>=0);
   if(g==null){if(e.groundLine<0)return false;throw new InvalidOperationException("Rail lacks its exact original current BEFORE/NEXT support chain");}
   if(!SourceMine2RailTurnV21(to,e.facing,e.x,e.y,g.startX,g.startY,g.endX,g.endY))return false;
   if(!TryEnterNpcState(e,npcStates[e.sourceNPC+"/NASOD_RAIL_TURN"]))return false;Mine2EventV21(e,"rail-turn",elapsed);return true;
  }
  void BeginMine2MechanismsRoomV21(){if(mine2MechanismsV21==null)return;foreach(var e in enemies)if(e.sourceNPC=="NUI_DOWN_STONE_TRAP_BESMA")EnsureMine2StoneV21(e);}
  Mine2StoneActorV21 EnsureMine2StoneV21(Enemy e){
   Mine2StoneActorV21 a;if(mine2StonesV21.TryGetValue(e,out a))return a;
   var b=Array.Find(mine2MechanismsV21.stoneBirths,q=>q.roomId==CurrentRoom.id&&q.line==e.groundLine&&Math.Abs(q.x-e.x)<.002f&&Math.Abs(q.baseY-e.y)<.002f);
   if(b==null)throw new InvalidOperationException("Falling stone has no exact source ADD_POS_Y birth contract");
   a=new Mine2StoneActorV21{birth=b,facing=e.facing};mine2StonesV21.Add(e,a);e.y=b.baseY+b.addNativeY*.28f;e.vx=e.vy=0;e.state=CombatState.Normal;
   EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_WAIT",elapsed);Mine2EventV21(e,"source-add-pos-y",elapsed,b.addNativeY,b.spawnIndex);return a;
  }
  void EnterMine2StoneV21(Enemy e,Mine2StoneActorV21 a,string id,float at){a.age=0;EnterNpcState(e,npcStates[e.sourceNPC+"/"+id]);Mine2EventV21(e,"stone-state",at,0,a.birth.spawnIndex);}
  void Mine2SoundV21(Enemy e,string name,float at,float sourceTime){string id="npc-"+name.Substring(0,name.LastIndexOf('.')).ToLowerInvariant();if(!npcSourceSoundAssetsV17.Contains(id))throw new InvalidOperationException("Missing exact Mine2 original callback sound "+name);QueueNpcSound(new NpcTimedSound{id="mine2/"+e.npcState+"/"+name,sound=id,sourceName=name,sourceField="original-Mine2-Lua-callback.PlaySound_LUA",sourceTime=sourceTime,candidateTime=sourceTime,candidateClock="source-callback"},e,++npcSoundEntrySerial,0,at,e.npcState);}
  bool TickMine2StoneV21(Enemy e,NpcDefinition p,float dt){
   if(mine2MechanismsV21==null||e.sourceNPC!="NUI_DOWN_STONE_TRAP_BESMA")return false;if(!e.active||e.dormant)return true;
   var a=EnsureMine2StoneV21(e);if(e.hp<=0){if(e.npcState!="DOWN_STONE_TRAP_BESMA_BROKEN")EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_BROKEN",elapsed-dt);e.deathTime+=dt;AdvancePassagePose(e,NpcState(e),dt,elapsed-dt);return true;}
   if(e.hitPause>0){float frozen=Math.Min(e.hitPause,dt);e.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
   float left=dt,at=elapsed-dt;while(left>.000001f){float step=Math.Min(left,1f/120);var s=NpcState(e);
    if(s==null||e.state==CombatState.Hurt&&e.npcState!="DOWN_STONE_TRAP_BESMA_ATTACK"){e.state=CombatState.Normal;e.stun=0;e.vx=e.vy=0;EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_DAMAGE",at);s=NpcState(e);}
    float beforeAge=a.age;a.age+=step;e.x=a.birth.x;e.facing=a.facing;e.vx=0;AdvancePassagePose(e,s,step,at);
    if(s.id=="DOWN_STONE_TRAP_BESMA_WAIT"){
     e.y=a.birth.baseY+a.birth.addNativeY*.28f;e.vy=0;e.groundLine=a.birth.line;e.passive=true;
     a.decision+=step;if(a.decision+1e-6f>=2){a.decision-=2;float dx=player.x-e.x,dy=player.y-e.y;bool same=player.groundLine==a.birth.line;
      if(same&&dx*dx+dy*dy<=560*560){float roll=Mine2StoneRollOverrideV21==null?NpcRoll():Mine2StoneRollOverrideV21();if(!Finite(roll)||roll<0||roll>=100)throw new InvalidOperationException("Mine2 stone roll outside original percentage domain");Mine2EventV21(e,"stone-condition-roll",at+step,roll);if(roll<50)EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_ATTACK_BEFORE",at+step);}
     }
    }else if(s.id=="DOWN_STONE_TRAP_BESMA_ATTACK_BEFORE"){
     e.y=a.birth.baseY+a.birth.addNativeY*.28f;e.vy=0;e.groundLine=a.birth.line;e.passive=true;
     if(!a.dust&&beforeAge<.01f&&a.age>=.01f){a.dust=true;Mine2SoundV21(e,"DownStoneDust.ogg",at+Math.Max(0,.01f-beforeAge),.01f);}
     if(a.age+1e-6f>=2){EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_ATTACK",at+step);e.state=CombatState.Airborne;e.groundLine=-1;e.passive=false;e.vy=0;Mine2EventV21(e,"stone-support-release",at+step);}
    }else if(s.id=="DOWN_STONE_TRAP_BESMA_ATTACK"){
     float before=e.y;float v=e.vy,g=1120,terminal=560;float accelerationTime=Math.Min(step,Math.Max(0,(terminal+v)/g));e.y+=v*accelerationTime-.5f*g*accelerationTime*accelerationTime-terminal*(step-accelerationTime);e.vy=Math.Max(-terminal,v-g*step);
     var floor=NativeTerrain?SurfaceBelow(e.x,before+.00001f):null;bool landed=floor!=null&&e.y<=floor.Y(e.x);if(landed){e.y=floor.Y(e.x);e.vy=0;e.groundLine=floor.id;}
     if(!a.hitPlayer&&CaveStoneTouches(e,s,before,new AttackBox{left=player.x-9.8f,right=player.x+9.8f,bottom=player.y,top=player.y+42})){var hit=s.hits[0];int dealt=ApplyHitToPlayer((int)Math.Round(p.physicalAttack*hit.physical),e.x,hit.reaction,hit.launch);if(dealt>0){a.hitPlayer=true;Mine2EventV21(e,"stone-hit-player",at+step,dealt);}}
     foreach(var target in enemies){if(target==e||target.hp<=0||!target.active||target.dormant||target.isGate||target.passive||NpcProfile(target)==null||EnemyHasNpcInvincibility(target)||a.hitEnemies.Contains(target))continue;if(CaveStoneTouches(e,s,before,EnemyBounds(target))){var hit=s.hits[0];int dealt=ApplyHit(target,(int)Math.Round(p.physicalAttack*hit.physical),hit.reaction,e.x,hit.launch,false);if(dealt>0){a.hitEnemies.Add(target);Mine2EventV21(e,"stone-hit-npc",at+step,dealt);}}}
     if(landed){Mine2EventV21(e,"stone-real-landing",at+step);Mine2SoundV21(e,"DownStone.ogg",at+step,a.age);e.state=CombatState.Normal;e.passive=false;EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_WAIT2",at+step);}
    }else if(s.id=="DOWN_STONE_TRAP_BESMA_DAMAGE"){e.vy=0;if(a.age+1e-6f>=s.duration)EnterMine2StoneV21(e,a,"DOWN_STONE_TRAP_BESMA_WAIT2",at+step);}
    else{e.vy=0;e.passive=false;e.aiEnabled=false;}
    left-=step;at+=step;
   }return true;
  }
 }
}
