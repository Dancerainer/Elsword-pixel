using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed class CoreHardAlarmBeamViewR1417 {
  public readonly Enemy ownerEnemy;
  public readonly float startX,startY,endX,endY,halfWidth,nativeDepth;
  public readonly string phase;
  // The native mesh points through the lane along Z. Both endpoints have
  // the same logical X/Y. Depth is in native units; X/Y are player units.
  public const float SocketDepth = -372, TipDepth = SocketDepth + 4100;
  internal CoreHardAlarmBeamViewR1417(Enemy owner,float height,bool armed) {
   ownerEnemy=owner; startX=endX=owner.x; startY=endY=owner.y+height;
   halfWidth=3; nativeDepth=owner.nativeDepth; phase=armed?"active":"warning";
  }
 }
 public sealed partial class GameModel {
  sealed class CoreHardAlarmRuntimeR1417 {
   internal float bornAt, originX, armedAt=float.NaN, nextHit;
   internal bool touching;
   internal string state;
  }
  readonly Dictionary<Enemy,CoreHardAlarmRuntimeR1417> coreHardAlarmRuntimeR1417=new Dictionary<Enemy,CoreHardAlarmRuntimeR1417>();
  readonly List<CoreHardAlarmBeamViewR1417> coreHardAlarmViewsR1417=new List<CoreHardAlarmBeamViewR1417>();
  RoomDefinition coreHardAlarmRoomR1417;
  public int CoreHardAlarmTriggerCountR1417 {get;private set;}
  public int CoreHardAlarmDamageHitsR1417 {get;private set;}
  bool CoreHardAlarmScopeR1417(){
   if(coreHardV21==null||coreHardV21.sourceDifficulty!="Hard"||coreHardV21.difficultyValue!=1||coreHardV21.npcLevel!=37||coreHardV21.nodes==null||coreHardV21.nodes.Length!=8||RoomCount!=5||room<0||room>=5||CurrentEncounter==null)return false;
   for(int i=0;i<5;i++)if(rooms[i]==null||rooms[i].id!="Dungeon_Altera_Core_Hard-"+i)return false;
   var node=Array.Find(coreHardV21.nodes,n=>n.room==room&&n.encounter==EncounterIndex);
   return node!=null&&CurrentEncounter.id==node.id&&CurrentEncounter.sourceWorldId==node.world;
  }
  public IEnumerable<CoreHardAlarmBeamViewR1417> CoreHardAlarmBeamViewsR1417(){
   if(!CoreHardAlarmScopeR1417())yield break;
   foreach(var beam in coreHardAlarmViewsR1417)yield return beam;
  }
  void ClearCoreHardAlarmTrapsR1417(bool resetCounters=true){
   coreHardAlarmRuntimeR1417.Clear();coreHardAlarmViewsR1417.Clear();coreHardAlarmRoomR1417=null;
   if(resetCounters){CoreHardAlarmTriggerCountR1417=0;CoreHardAlarmDamageHitsR1417=0;}
  }
  static string CoreHardAlarmPrefixR1417(string npc){
   switch(npc){case "NUI_ALARM_SYSTEM_A":return "ALARM_SYSTEMA_";case "NUI_ALARM_SYSTEM_B":return "ALARM_SYSTEMB_";case "NUI_ALARM_SYSTEM_C":return "ALARM_SYSTEMC_";case "NUI_ALARM_SYSTEM_C_MOVE":return "ALARM_SYSTEMC_MOVE_";default:return null;}
  }
  // Dummy01 world heights from the original Motion_Alarm_System[A/B/C].X
  // converted with the route's .28 unit. A's mirrored one-second Wait bobs.
  static readonly float[] alarmSocketA={28.51f,32.81f,43.12f,56.69f,70.26f,80.58f,84.88f};
  internal static float CoreAlarmSocketHeightV59(Enemy e){
   if(e.sourceNPC=="NUI_ALARM_SYSTEM_A"){
    float age=e.npcTime%1, sample=Math.Min(age,1-age)*12;
    int first=Math.Min(5,(int)sample);
    return alarmSocketA[first]+(alarmSocketA[first+1]-alarmSocketA[first])*(sample-first);
   }
   if(e.sourceNPC=="NUI_ALARM_SYSTEM_B")return e.npcState=="ALARM_SYSTEMB_ATTACK1"||e.npcState=="ALARM_SYSTEMB_ATTACK2"||e.npcState=="ALARM_SYSTEMB_TURN1"||e.npcState=="ALARM_SYSTEMB_TURN2"?83.78f:84.26f;
   return 42.3f;
  }
  static bool AlarmContactV59(CoreHardAlarmBeamViewR1417 beam,Fighter target){
   return Math.Abs(target.x-beam.startX)<=9.8f+beam.halfWidth&&target.y<=beam.startY+beam.halfWidth&&target.y+42>=beam.startY-beam.halfWidth;
  }
  void TickCoreHardAlarmTrapsR1417(){
   if(!CoreHardAlarmScopeR1417()){ClearCoreHardAlarmTrapsR1417(false);return;}
   if(!ReferenceEquals(coreHardAlarmRoomR1417,CurrentRoom)){ClearCoreHardAlarmTrapsR1417(false);coreHardAlarmRoomR1417=CurrentRoom;}
   if(paused)return;
   coreHardAlarmViewsR1417.Clear();
   if(victory||player==null||player.hp<=0)return;
   foreach(var e in enemies){
    string prefix=CoreHardAlarmPrefixR1417(e.sourceNPC);
    if(prefix==null||!e.active||e.dormant||e.hp<=0||!e.passive||e.aiEnabled||e.encounterIndex!=EncounterIndex)continue;
    var profile=NpcProfile(e);
    if(profile==null||e.npcState==prefix+"SUMMON"||e.npcState==prefix+"DYING")continue;
    if(!coreHardAlarmRuntimeR1417.TryGetValue(e,out var run)){
     run=new CoreHardAlarmRuntimeR1417{bornAt=elapsed,originX=e.spawnX};coreHardAlarmRuntimeR1417.Add(e,run);
    }
    bool bob=e.sourceNPC=="NUI_ALARM_SYSTEM_A";
    float age=elapsed-run.bornAt;
    float start=npcStates[profile.id+"/"+prefix+"START"].duration;
    bool near=Math.Abs(player.x-run.originX)<=profile.senseRange;
    if(float.IsNaN(run.armedAt)&&age>=start&&near){run.armedAt=elapsed;CoreHardAlarmTriggerCountR1417++;}
    float clock=age;
    string suffix=age<start?"START":"WAIT";
    bool armed=false;
    if(!float.IsNaN(run.armedAt)){
     float warn=bob?1:npcStates[profile.id+"/"+prefix+"ATTACK_READY"].duration;
     float t=elapsed-run.armedAt;
     armed=t>=warn;
     if(!bob){
      suffix="ATTACK_READY";clock=t;
      if(armed){
       // ATTACK1 and ATTACK2 move in opposite directions, with stationary
       // turn states between them. Motion falls on the source's half-second cycle.
       float phase=(t-warn)%2;int side=(int)(phase*2);clock=phase-side*.5f;
       suffix=side==0?"ATTACK1":side==1?"TURN1":side==2?"ATTACK2":"TURN2";
       float distance=side==0?clock:side==1?.5f:side==2?.5f-clock:0;
       float speed=e.sourceNPC=="NUI_ALARM_SYSTEM_C_MOVE"?700:900;
       e.x=run.originX-e.facing*speed*NativeToLogical*distance;
      }
     }
    }
    var state=npcStates[profile.id+"/"+prefix+suffix];
    if(run.state!=state.id){EnterNpcState(e,state);run.state=state.id;}
    e.npcTime=state.loop?clock%state.duration:Math.Min(clock,state.duration);e.clock=e.npcTime*state.sourceAnimSpeed;
    if(float.IsNaN(run.armedAt))continue;
    var beam=new CoreHardAlarmBeamViewR1417(e,CoreAlarmSocketHeightV59(e),armed);coreHardAlarmViewsR1417.Add(beam);
    bool touching=armed&&AlarmContactV59(beam,player);
    if(!touching){run.touching=false;continue;}
    bool repeat=e.sourceKeyCode==3;
    if(run.touching&&(!repeat||elapsed<run.nextHit))continue;
    // Core Hard gives the passive housing zero offensive stats. The laser
    // is a separate DamageEffect with MAGIC=1, using the authored effect base.
    int effectDamage=(int)Math.Round(npcDefinitions[e.sourceNPC].magicAttack);
    if(ApplyHitToPlayer(effectDamage,e.x,HitReaction.Knockdown)<=0)continue;
    run.touching=true;run.nextHit=elapsed+.2f;CoreHardAlarmDamageHitsR1417++;
   }
  }
 }
}
