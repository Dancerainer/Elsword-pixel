using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraAirshipCaptionV18 {public float time,duration;public string sourceString;}
 [Serializable] public sealed class AlteraAirshipSoundV18 {public float time,sourceVolume;public string sourceName,sourceField,sound;}
 [Serializable] public sealed class AlteraAirshipIntroRuleV18 {
  public string roomId,ravenNpc,directorNpc,ravenReadyState,ravenStartState,ravenWaitState;
  public bool directorPresent;public float directorDuration,directorDyingDuration,ravenStartDuration,directorSourceAnimSpeed;
  public AlteraAirshipCaptionV18[] captionEvents=new AlteraAirshipCaptionV18[0];
  public AlteraAirshipSoundV18[] sourceSoundEvents=new AlteraAirshipSoundV18[0];
 }
 [Serializable] public sealed class AlteraAirshipIntroEventV18 {public string type,npc,source;public float time,duration,x,y,z;}
 public sealed partial class GameModel {
  readonly Dictionary<string,AlteraAirshipIntroRuleV18> alteraAirshipIntroRulesV18=new Dictionary<string,AlteraAirshipIntroRuleV18>();
  AlteraAirshipIntroRuleV18 alteraAirshipIntroRuleV18;Enemy alteraAirshipIntroRavenV18;
  float alteraAirshipIntroBeganV18,alteraAirshipIntroPreviousV18;bool alteraAirshipIntroReleasedV18;
  Enemy alteraAirshipDirectorSoundActorV18;long alteraAirshipDirectorSoundEntryV18;
  public readonly List<AlteraAirshipIntroEventV18> alteraAirshipIntroEventsV18=new List<AlteraAirshipIntroEventV18>();
  public void SetAlteraAirshipIntroDefinitionsV18(AlteraAirshipIntroRuleV18[] definitions){
   var next=new Dictionary<string,AlteraAirshipIntroRuleV18>();
   foreach(var r in definitions??new AlteraAirshipIntroRuleV18[0]){
    if(r==null||r.roomId!="Dungeon_Altera_Battle_Air_Ship_Normal-3"||next.ContainsKey(r.roomId)||r.ravenNpc!="NUI_RAVEN"||r.directorNpc!="NUI_EVENT_NPC_BATTLE_AIR_SHIP"||!r.directorPresent||
     r.ravenReadyState!="RAVEN_START_READY"||r.ravenStartState!="RAVEN_START"||r.ravenWaitState!="RAVEN_WAIT"||r.directorDuration!=14.5f||r.directorDyingDuration!=.2f||r.ravenStartDuration!=3.8f||r.directorSourceAnimSpeed!=1||
     r.captionEvents==null||r.captionEvents.Length!=2||r.captionEvents[0].time!=3||r.captionEvents[0].duration!=4.5f||r.captionEvents[0].sourceString!="STR_ID_31533"||r.captionEvents[1].time!=8.75f||r.captionEvents[1].duration!=3.5f||r.captionEvents[1].sourceString!="STR_ID_31534")throw new ArgumentException("Changed original Airship Raven intro contract");
    NpcDefinition profile;if(!npcDefinitions.TryGetValue(r.ravenNpc,out profile))throw new ArgumentException("Missing original Raven combat profile");
    foreach(string sid in new[]{r.ravenReadyState,r.ravenStartState,r.ravenWaitState})if(!npcStates.ContainsKey(r.ravenNpc+"/"+sid))throw new ArgumentException("Missing original Raven intro state "+sid);
    var start=npcStates[r.ravenNpc+"/"+r.ravenStartState];if(start.duration!=r.ravenStartDuration||start.sourceAnimation!="Boss_WaitStart"||start.sourceAnimSpeed!=1||start.nextState!=r.ravenWaitState)throw new ArgumentException("Raven source START duration/transition changed");
    string[] names={"AlteraCutScene_Whoosh.ogg","AlteraCutScene_ArmourRattle.ogg","Raven_PunchAttack1.ogg","AlteraCutScene_Grab.ogg","AlteraCutScene_BigJump.ogg","AlteraCutScene_JumpLand.ogg"};float[] times={.1f,5.7f,7.9f,8.24f,11,12.4f};
    if(r.sourceSoundEvents==null||r.sourceSoundEvents.Length!=6)throw new ArgumentException("Missing original director sound timeline");
    for(int i=0;i<6;i++){var cue=r.sourceSoundEvents[i];if(cue==null||cue.sourceName!=names[i]||cue.time!=times[i]||cue.sourceField!="SOUND_PLAY"+i||cue.sourceVolume!=100||!npcSourceSoundAssetsV17.Contains(cue.sound??""))throw new ArgumentException("Director sound is not bound to exact loaded original audio");}
    next.Add(r.roomId,r);
   }
   alteraAirshipIntroRulesV18.Clear();foreach(var r in next)alteraAirshipIntroRulesV18.Add(r.Key,r.Value);ClearAlteraAirshipIntroRoomV18();
  }
  void ClearAlteraAirshipIntroRoomV18(){alteraAirshipIntroRuleV18=null;alteraAirshipIntroRavenV18=null;alteraAirshipDirectorSoundActorV18=null;alteraAirshipDirectorSoundEntryV18=0;alteraAirshipIntroBeganV18=elapsed;alteraAirshipIntroPreviousV18=0;alteraAirshipIntroReleasedV18=false;alteraAirshipIntroEventsV18.Clear();}
  void AlteraAirshipIntroEventV18(string type,string npc,string source,float at,float duration=0,float x=0,float y=0,float z=0){
   if(alteraAirshipIntroEventsV18.Count>=256)alteraAirshipIntroEventsV18.RemoveAt(0);
   alteraAirshipIntroEventsV18.Add(new AlteraAirshipIntroEventV18{type=type,npc=npc,source=source,time=at,duration=duration,x=x,y=y,z=z});
  }
  // Hook once after EnterRoom constructs the original roster/encounter. The
  // director is a separate script/visual declaration; no fake enemy is added.
  void BeginAlteraAirshipIntroRoomV18(){
   AlteraAirshipIntroRuleV18 rule;if(!alteraAirshipIntroRulesV18.TryGetValue(CurrentRoom.id??"",out rule))return;
   var raven=enemies.Find(e=>e.sourceNPC==rule.ravenNpc&&e.active&&!e.dormant);if(raven==null)throw new InvalidOperationException("Original Airship intro has no actual Raven instance");
   if(SimpleDangerIntroductions){alteraAirshipIntroRuleV18=rule;alteraAirshipIntroRavenV18=raven;alteraAirshipIntroReleasedV18=true;raven.aiEnabled=true;EnterNpcState(raven,npcStates[rule.ravenNpc+"/"+rule.ravenWaitState]);return;}
   if(raven.aiEnabled)throw new InvalidOperationException("Original STOP_AT_START_STATE Raven was prematurely AI-enabled");
   alteraAirshipIntroRuleV18=rule;alteraAirshipIntroRavenV18=raven;alteraAirshipIntroBeganV18=elapsed;alteraAirshipIntroPreviousV18=0;alteraAirshipIntroReleasedV18=false;
   EnterNpcState(raven,npcStates[rule.ravenNpc+"/"+rule.ravenReadyState]);
   // Audio identity for the separate source visual actor. It is never inserted
   // into enemies and cannot affect collision, population, HP or clear clauses.
   alteraAirshipDirectorSoundActorV18=new Enemy{sourceNPC=rule.directorNpc,x=0,y=0};alteraAirshipDirectorSoundEntryV18=++npcSoundEntrySerial;
   AlteraAirshipIntroEventV18("director-start",rule.directorNpc,"EVENT_NPC_BATTLE_AIR_SHIP_START",elapsed);
   AlteraAirshipIntroEventV18("source-force-position",rule.directorNpc,"SetPositionForce",elapsed,0,0,0,0);
  }
  // Independent original director age; game pause stops Step/elapsed. Enemy
  // hit pause cannot move the motion-end release earlier or alter this actor.
  void TickAlteraAirshipIntroV18(){
   if(SimpleDangerIntroductions)return;
   var r=alteraAirshipIntroRuleV18;var raven=alteraAirshipIntroRavenV18;if(r==null||raven==null||!enemies.Contains(raven))return;
   float age=Math.Max(0,elapsed-alteraAirshipIntroBeganV18),before=alteraAirshipIntroPreviousV18;
   foreach(var cue in r.sourceSoundEvents)if(before<cue.time&&cue.time<=age){
    var sound=new NpcTimedSound{id="source-director/"+r.roomId+"/"+cue.sourceField,sound=cue.sound,sourceName=cue.sourceName,sourceField=cue.sourceField,
      candidateClock="source-director-animation-seconds",candidateTime=cue.time,sourceTime=cue.time*r.directorSourceAnimSpeed};
    QueueNpcSound(sound,alteraAirshipDirectorSoundActorV18,alteraAirshipDirectorSoundEntryV18,0,alteraAirshipIntroBeganV18+cue.time,"EVENT_NPC_BATTLE_AIR_SHIP_START");
    AlteraAirshipIntroEventV18("source-sound",r.directorNpc,cue.sourceName,alteraAirshipIntroBeganV18+cue.time);
   }
   foreach(var cue in r.captionEvents)if(before<cue.time&&cue.time<=age)AlteraAirshipIntroEventV18("source-caption",r.directorNpc,cue.sourceString,alteraAirshipIntroBeganV18+cue.time,cue.duration);
   if(!alteraAirshipIntroReleasedV18&&age+1e-6f>=r.directorDuration){
    if(raven.hp<=0)throw new InvalidOperationException("Invincible source Raven was killed during its hidden intro");
    EnterNpcState(raven,npcStates[r.ravenNpc+"/"+r.ravenStartState]);raven.aiEnabled=true;alteraAirshipIntroReleasedV18=true;
    float at=alteraAirshipIntroBeganV18+r.directorDuration;
    AlteraAirshipIntroEventV18("director-dying",r.directorNpc,"EVENT_NPC_BATTLE_AIR_SHIP_DYING",at,r.directorDyingDuration);
    AlteraAirshipIntroEventV18("source-raven-start",r.ravenNpc,"StateChange_LUA(RAVEN_START)",at);
    AlteraAirshipIntroEventV18("source-force-position",r.directorNpc,"SetPositionForce",at,0,0,-5000,0);
    alteraAirshipDirectorSoundActorV18.y=-5000;
   }
   alteraAirshipIntroPreviousV18=age;
  }
  public bool TryAlteraAirshipRavenPresentationV18(Enemy enemy,out bool visible,out bool gauge){visible=gauge=true;if(enemy!=alteraAirshipIntroRavenV18||alteraAirshipIntroRuleV18==null)return false;visible=gauge=alteraAirshipIntroReleasedV18;return true;}
  bool AlteraAirshipScriptedStartActiveV18(Enemy enemy){return enemy!=null&&enemy==alteraAirshipIntroRavenV18&&alteraAirshipIntroReleasedV18&&enemy.npcState=="RAVEN_START";}
  bool AlteraAirshipDirectorInvincibleV18(Enemy enemy){return enemy!=null&&enemy.sourceNPC=="NUI_EVENT_NPC_BATTLE_AIR_SHIP"&&alteraAirshipIntroRulesV18.ContainsKey(CurrentRoom.id??"");}
 }
}
