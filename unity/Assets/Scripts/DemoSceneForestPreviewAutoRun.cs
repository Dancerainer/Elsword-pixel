#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class ForestAutoVisit {public int room,entities,hits,kills;public string world;public float modelTime,wallSeconds,x,y,hp;}
  [Serializable] sealed class ForestAutoEnemy {public string sourceNPC,kind,state;public float x,y,hp,invincible;public int groundLine;public bool active,isGate,countsForClear;}
  [Serializable] sealed class ForestAutoAudio {public int room,consumed,played,muted,culled,missing;}
  [Serializable] sealed class ForestAutoReport {
   public string status,reason,bundlePath,bundleSha256,startedUtc,finishedUtc,output,targetNPC,playerState;
   public string scope="Actual Editor PlayMode through ordinary Input and the existing 60Hz GameModel.Step. Navigation, attacks, equipped skills, awakening and potion inputs only. No direct HP, damage, position, kills or clear-condition changes. Preview art remains unadmitted.";
   public string audioCountingScope="NPC cue outcomes collected at the existing audio consumer from automatic-input start through finish, grouped by the current room. Counts are playback requests accepted by the existing voice pool, not proof of audible output at the device.";
   public bool passed,victory,allThreeRoomsReached,allRoomsReached,gateOpen,combatClear;public int expectedRooms,room,hits,kills,playerGroundLine,npcSoundsConsumed,npcSoundsPlayed,npcSoundsMuted,npcSoundsCulled,npcSoundsMissing;
   public int sourceNpcMeshDrawFrames,lakeSourceParticleDrawFramesV18,lakeSourceParticleKindsV18;
   public int dragonNestMainMeshEffectDrawFramesV20,dragonNestMainMeshHeldDrawFramesV20,dragonNestMainMeshCreatedEffectSerialsV20,dragonNestMainMeshCreatedHeldInstancesV20;
   public int dragonNestEffectParticleDrawFramesV20,dragonNestEffectParticleKindsDrawnV20,dragonNestEffectParticleHeldBindingEventsV20,dragonNestEffectParticleCreatedSequencesV20;
   public int dragonNestCountdownCreatedCountV20,dragonNestCountdownTextureChangeCountV20,dragonNestCountdownDestroyedCountV20,dragonNestCountdownDrawCountV20;
   public int dragonNestGeyserCreatedSequencesV20,dragonNestGeyserWarningSequencesV20,dragonNestGeyserFireSequencesV20,dragonNestGeyserKindsDrawnV20;
   public string dragonNestVisualCountingScope;
   public int dragonNestDirectorCreatedV20,dragonNestDirectorRetiredV20,dragonNestDirectorRendererCreatedV20,dragonNestDirectorDrawFramesV20,dragonNestDirectorCameraFramesV20,dragonNestDirectorRestoreCountV20;public string dragonNestDirectorCameraPolicy;public float dragonNestDirectorDisplayFovV20,dragonNestDirectorNativeDepthV20;public DragonNestDirectorCaptionRecordV20[] dragonNestDirectorCaptionRecordsV20;
   public float modelSeconds,wallSeconds,secondsSinceProgress,x,y,hp,mana;
   public ForestAutoVisit[] rooms;public ForestAutoAudio[] roomAudio;public ForestAutoEnemy[] remaining;public string[] errors,screenshots;
  }
  bool forestAutoArmed,forestAutoRunning,forestAutoFinished,forestAutoPreviousRunInBackground;
  string forestAutoOutput,forestAutoPreviousCaptureDir,forestAutoStartedUtc,forestAutoStatus="",forestAutoTarget="";
  float forestAutoWallStart,forestAutoArmedAt,forestAutoProgressAt,forestAutoLogAt,forestAutoProgressX,forestAutoProgressY,forestAutoNextAttack;
  int forestAutoRoom=-1,forestAutoProgressHits,forestAutoProgressKills;
  readonly List<ForestAutoVisit> forestAutoVisits=new List<ForestAutoVisit>();
  readonly List<ForestAutoAudio> forestAutoAudio=new List<ForestAutoAudio>();
  readonly List<string> forestAutoErrors=new List<string>(),forestAutoScreenshots=new List<string>();
  readonly HashSet<string> forestAutoEffectCaptures=new HashSet<string>();
  public bool ForestPreviewAutoRunning {get{return forestAutoArmed||forestAutoRunning;}}
  public bool ForestPreviewAutoFinished {get{return forestAutoFinished;}}
  public string ForestPreviewAutoOutput {get{return forestAutoOutput;}}
  string SourcePreviewLogPrefix {get{return belowTunnelEntrySessionV21?"ALTERA_BELOW_TUNNEL":mine2PreviewSessionV21?"BESMA_MINE2":alteraAirshipPreviewSessionV20?"ALTERA_BATTLE_AIR_SHIP":dragonNestPreviewSession?"BESMA_DRAGON_NEST":lakeNightPreviewSession?"BESMA_LAKE_NIGHT":lakePreviewSession?"BESMA_LAKE":dragonRoadPreviewSession?"DRAGON_ROAD":castleCenterPreviewSession?"CASTLE_CENTER":outskirtsPreviewSession?"OUTSKIRTS":belowPassagePreviewSession?"WATERWAY":cavePreviewSession?"CAVE":"FOREST";}}

  public void ArmForestPreviewAutoRun(string output){
   if(!Application.isPlaying)throw new InvalidOperationException("Source dungeon automatic preview requires PlayMode");
   if(forestAutoArmed||forestAutoRunning)FinishForestAutoRun("restarted","A new automatic preview was requested",false);
   if(string.IsNullOrEmpty(output))output=Path.GetFullPath(Path.Combine(Application.dataPath,"../Reports/"+(outskirtsPreviewSession?"outskirts-interactive-auto-v14-":belowPassagePreviewSession?"waterway-interactive-auto-v11-":cavePreviewSession?"cave-interactive-auto-v10-":"forest-interactive-auto-v09-")+DateTime.Now.ToString("yyyyMMdd-HHmmss")));
   output=Path.GetFullPath(output);if(File.Exists(Path.Combine(output,"report.json")))output=Path.Combine(output,DateTime.Now.ToString("yyyyMMdd-HHmmss-fff"));
   Directory.CreateDirectory(output);forestAutoOutput=output;forestAutoPreviousCaptureDir=captureDir;captureDir=output;
   forestAutoPreviousRunInBackground=Application.runInBackground;Application.runInBackground=true;
   forestAutoVisits.Clear();forestAutoAudio.Clear();forestAutoErrors.Clear();forestAutoScreenshots.Clear();forestAutoRoom=-1;forestAutoTarget="";forestAutoNextAttack=0;
   forestAutoEffectCaptures.Clear();
   BeginSourceInputTraceV17();
   forestAutoFinished=false;forestAutoArmed=true;forestAutoRunning=false;forestAutoStatus="WAITING FOR "+SourcePreviewLogPrefix;forestAutoArmedAt=Time.realtimeSinceStartup;
   Application.logMessageReceived-=ForestAutoLog;Application.logMessageReceived+=ForestAutoLog;
   Debug.Log(SourcePreviewLogPrefix+"_AUTO_ARMED output="+output+" deadline="+(belowTunnelEntrySessionV21?BelowTunnelEntryDeadlineV21:mine2PreviewSessionV21?Mine2ValidationDeadlineV21:alteraAirshipPreviewSessionV20?600:300)+"s inputOnly=true");
  }
  Input ForestAutoInput(Input manual){
   if(forestAutoArmed){
    if(!forestPreviewActive)return manual;
    forestAutoArmed=false;forestAutoRunning=true;forestAutoStatus="RUNNING";forestAutoStartedUtc=DateTime.UtcNow.ToString("o");
    forestAutoWallStart=forestAutoProgressAt=forestAutoLogAt=Time.realtimeSinceStartup;forestAutoProgressX=game.player.x;forestAutoProgressY=game.player.y;forestAutoProgressHits=game.hits;forestAutoProgressKills=game.kills;
   }
   if(!forestAutoRunning)return manual;
   var input=new Input{skill=-1};if(game.paused||game.victory||game.player.hp<=0)return input;
   if(game.player.hp<340)input.potion=true;
   if(game.gateOpen){
    forestAutoTarget="EXIT";float exit=game.HasEncounters?game.CurrentEncounter.exitX:game.ExitX;int direction=game.HasEncounters?game.CurrentEncounter.direction:1;
    input.move=Math.Abs(exit-game.player.x)>1?Math.Sign(exit-game.player.x):direction;input.dash=true;game.NativeNavigate(input,exit,game.GroundAt(game.CurrentRoom,exit));return input;
   }
   Enemy target=null;float best=float.PositiveInfinity,targetScore=float.PositiveInfinity;
   foreach(var enemy in game.enemies){
    if(!enemy.active||enemy.dormant||enemy.passive||enemy.hp<=0||game.HasEncounters&&enemy.encounterIndex!=game.EncounterIndex)continue;
    // Preserve the actual gate lock; fight only after the model unlocks it.
    if(enemy.isGate&&(!game.EncounterCombatClear||enemy.invincible>0))continue;
    if(!enemy.isGate&&!enemy.countsForClear)continue;
    float distance=Vector2.Distance(new Vector2(enemy.x,enemy.y),new Vector2(game.player.x,game.player.y));
    float score=mine2PreviewSessionV21&&enemy.sourceNPC=="NUI_NASOD_HEALER"&&distance<360?distance-10000:distance;if(score<targetScore){targetScore=score;best=distance;target=enemy;}
   }
   if(target==null){forestAutoTarget="NO REACHABLE ACTIVE TARGET";return input;}
   // Castle Center's two source-inactive guards are real solid actors.
   // Ordinary players can strike a guard blocking their path to Wally;
   // the input driver must do the same instead of walking into its body.
   // This selects an input target only: ACTIVE, clear predicates and HP
   // remain source-owned, including activation through the real hit path.
   if(castleCenterPreviewSession||mine2PreviewSessionV21){
    float direction=Math.Sign(target.x-game.player.x);
    foreach(var obstruction in game.enemies){
     if(!obstruction.active||obstruction.dormant||obstruction.passive||obstruction.isGate||obstruction.hp<=0||obstruction.countsForClear&&!mine2PreviewSessionV21||obstruction.kind.StartsWith("prop-",StringComparison.Ordinal))continue;
     float obstructionX=obstruction.x-game.player.x,obstructionY=obstruction.y-game.player.y,distance=Mathf.Sqrt(obstructionX*obstructionX+obstructionY*obstructionY);
     if(obstructionX*direction>=0&&Math.Abs(obstructionX)<80&&Math.Abs(obstructionY)<45&&distance<best){
      if(mine2PreviewSessionV21){
       // Ordinary equipped Windmill already permits body passage during its
       // loop. Use that source skill to pass a repeatedly healed landed rock.
       for(int slot=0;slot<GameModel.SkillSlotCount;slot++){var pass=game.GetEquippedSkill(slot);if(pass==null||pass.id!="windmill")continue;
        if(game.ActiveSkillId=="windmill"){input.move=direction;input.skillHeldMask=1<<slot;forestAutoTarget="WINDMILL PAST LANDED STONE";return input;}
        if(game.cooldowns[slot]<=0&&game.mana>=pass.mpCost){input.move=direction;if(!game.Busy()){input.skill=slot;input.skillHeldMask=1<<slot;}forestAutoTarget="WINDMILL PAST LANDED STONE";return input;}
       }
      }
      target=obstruction;best=distance;
     }
    }
   }
   forestAutoTarget=target.sourceNPC;
   if(mine2PreviewSessionV21&&Mine2LowerPathInputV21.TryDrive(game,input,target))return input;
   if(BelowTunnelLowerPathInputV21.TryDrive(belowTunnelEntrySessionV21,game,input,target))return input;
   if(AutoNavigateCombat(input,target,best,ref forestAutoNextAttack))return input;
   int edgeDirection;float edgeMargin;bool edgeCombat=Mine2OpenEdgeCombatV21(target,out edgeDirection,out edgeMargin);if(edgeCombat&&edgeMargin<40&&!game.Busy()){input.move=-edgeDirection;forestAutoTarget="BACK FROM OPEN EDGE";return input;}
   float dx=target.x-game.player.x;input.move=Math.Abs(dx)>34?Math.Sign(dx):0;input.dash=best>200;
   if(dx*game.player.facing<0)input.move=Math.Sign(dx);if(edgeCombat){input.move=dx*game.player.facing<0?Math.Sign(dx):0;input.dash=false;}
   if(best<100&&Math.Abs(target.y-game.player.y)<45&&game.elapsed>=forestAutoNextAttack){input.attack=true;forestAutoNextAttack=game.elapsed+(edgeCombat?1.2f:.16f);}
   if(best<110&&game.CanAwaken){input.awaken=true;input.attack=false;return input;}
   if(best<110&&!game.Busy()){
    for(int slot=0;slot<GameModel.SkillSlotCount;slot++){
     var skill=game.GetEquippedSkill(slot);if(skill==null||game.cooldowns[slot]>0||game.mana<skill.mpCost||game.HyperEssence<skill.costItem)continue;
     if(skill.id=="counter-attack"||skill.id=="roll"||edgeCombat&&!Mine2StationarySkillV21(skill.id))continue;
     if(skill.id=="iron-body"&&(game.armor>0||game.player.hp>340))continue;
     if((skill.id=="armor-break"||skill.id=="kick")&&Math.Abs(dx)>28)continue;
     input.skill=slot;input.attack=false;break;
    }
   }
   return input;
  }
  void TickForestAutoRun(){
   if(forestAutoArmed&&!forestPreviewLoading&&!forestPreviewActive){FinishForestAutoRun("load-failed",string.IsNullOrEmpty(forestPreviewError)?"Source dungeon session did not become active":forestPreviewError,false);return;}
   if(forestAutoArmed&&Time.realtimeSinceStartup-forestAutoArmedAt>300){FinishForestAutoRun("load-timeout","External preview loading exceeded five minutes",false);return;}
   if(forestAutoFinished){if(forestPreviewNotice)forestPreviewNotice.text+="  |  AUTO: "+forestAutoStatus;return;}
   if(!forestAutoRunning)return;
   if(!forestPreviewActive){FinishForestAutoRun("preview-exited","Preview session ended before completion",false);return;}
   float now=Time.realtimeSinceStartup;
   if(forestAutoRoom!=game.room){
    forestAutoRoom=game.room;forestAutoProgressAt=now;forestAutoVisits.Add(new ForestAutoVisit{room=game.room,world=CurrentStageArt.sourceWorldId,entities=game.enemies.Count,hits=game.hits,kills=game.kills,modelTime=game.elapsed,wallSeconds=now-forestAutoWallStart,x=game.player.x,y=game.player.y,hp=game.player.hp});
    ForestAutoCapture("room-"+(game.room+1)+"-entry");Debug.Log(SourcePreviewLogPrefix+"_AUTO_ROOM room="+game.room+" world="+CurrentStageArt.sourceWorldId+" entities="+game.enemies.Count+" hits="+game.hits+" kills="+game.kills);
   }
   if(game.hits!=forestAutoProgressHits||game.kills!=forestAutoProgressKills||Math.Abs(game.player.x-forestAutoProgressX)>12||Math.Abs(game.player.y-forestAutoProgressY)>12){forestAutoProgressAt=now;forestAutoProgressHits=game.hits;forestAutoProgressKills=game.kills;forestAutoProgressX=game.player.x;forestAutoProgressY=game.player.y;}
   if(lakePreviewSession)MaybeCaptureLakeSourceVisualV18();if(lakeNightPreviewSession)MaybeCaptureLakeNightSourceVisualV19();if(dragonNestPreviewSession)MaybeCaptureDragonNestSourceVisualV20();if(alteraAirshipPreviewSessionV20)CaptureAlteraAirshipValidationV20();if(mine2PreviewSessionV21)CaptureMine2ValidationV21();if(belowTunnelEntrySessionV21)CaptureBelowTunnelEntryV21();
   if(dragonRoadPreviewSession)foreach(var effect in game.npcEffects){
    bool cactus=effect.id=="DamageEffect_CACTUS_ATTACK_A"&&effect.age>=1.78f&&effect.age<2.49f;
    bool landing=effect.id=="WALLY_8TH_JUMP_ATTACK_LANDING"&&effect.age>=.08f&&effect.age<.26f;
    if((cactus||landing)&&Math.Abs(effect.x-game.player.x)<360&&forestAutoEffectCaptures.Add(effect.id))ForestAutoCapture(cactus?"fx-cactus-rising-tree":"fx-high-landing");
   }
   if(game.victory){FinishForestAutoRun("victory","Original dungeon clear condition completed through normal inputs",true);return;}
   if(game.player.hp<=0){FinishForestAutoRun("defeated","Player HP reached zero through normal combat",true);return;}
   if(game.NativeTerrain){float lowest=float.PositiveInfinity;foreach(var surface in game.CurrentRoom.surfaces)lowest=Math.Min(lowest,Math.Min(surface.y1,surface.y2));if(game.player.y<lowest-600){FinishForestAutoRun("out-of-world","Player fell below every original support; capture the failed route without changing game position or HP",true);return;}}
   if(now-forestAutoWallStart>=(belowTunnelEntrySessionV21?BelowTunnelEntryDeadlineV21:mine2PreviewSessionV21?Mine2ValidationDeadlineV21:alteraAirshipPreviewSessionV20?600:300)){FinishForestAutoRun("timeout","Validation time limit reached; remaining actors and current navigation state are recorded",true);return;}
   if(now-forestAutoLogAt>=20){forestAutoLogAt=now;Debug.Log(SourcePreviewLogPrefix+"_AUTO_PROGRESS room="+game.room+" x="+game.player.x.ToString("F1")+" y="+game.player.y.ToString("F1")+" hp="+game.player.hp+" hits="+game.hits+" kills="+game.kills+" target="+forestAutoTarget+" idleSeconds="+(now-forestAutoProgressAt).ToString("F1"));}
   if(forestPreviewNotice)forestPreviewNotice.text+="  |  AUTO "+(int)(now-forestAutoWallStart)+"s";
  }
  void ForestAutoCapture(string name){
   if(!forestPreviewActive||!worldCamera||!hudCanvas)return;Capture(name);forestAutoScreenshots.Add(Path.Combine(forestAutoOutput,name+".png"));
  }
  void ForestAutoRecordNpcAudio(string outcome){
   if(!forestAutoRunning)return;
   var row=forestAutoAudio.Find(r=>r.room==game.room);if(row==null){row=new ForestAutoAudio{room=game.room};forestAutoAudio.Add(row);}
   row.consumed++;if(outcome=="played")row.played++;else if(outcome=="muted")row.muted++;else if(outcome=="missing")row.missing++;else row.culled++;
  }
  void ForestAutoLog(string message,string stack,LogType type){
   if(!forestAutoArmed&&!forestAutoRunning)return;
   if(type!=LogType.Error&&type!=LogType.Exception&&type!=LogType.Assert)return;
   if(forestAutoErrors.Count<12)forestAutoErrors.Add(message+(string.IsNullOrEmpty(stack)?"":"\n"+stack));
   FinishForestAutoRun("exception",message,false);
  }
  void FinishForestAutoRun(string status,string reason,bool capture){
   if(!forestAutoArmed&&!forestAutoRunning)return;
   bool started=forestAutoRunning;forestAutoArmed=forestAutoRunning=false;forestAutoFinished=true;forestAutoStatus=status.ToUpperInvariant();Application.logMessageReceived-=ForestAutoLog;
   bool all=true;for(int i=0;i<game.RoomCount;i++){int room=i;all&=forestAutoVisits.Exists(v=>v.room==room);}
   var remaining=new List<ForestAutoEnemy>();foreach(var e in game.enemies)if(e.active&&e.hp>0)remaining.Add(new ForestAutoEnemy{sourceNPC=e.sourceNPC,kind=e.kind,state=e.npcState,x=e.x,y=e.y,hp=e.hp,invincible=e.invincible,groundLine=e.groundLine,active=e.active,isGate=e.isGate,countsForClear=e.countsForClear});
   if(forestPreviewActive&&!game.victory)game.paused=true;
   if(capture){RenderHud();ForestAutoCapture(status=="victory"?"04-clear":"04-"+status);}
   foreach(var visit in forestAutoVisits)if(!forestAutoAudio.Exists(r=>r.room==visit.room))forestAutoAudio.Add(new ForestAutoAudio{room=visit.room});forestAutoAudio.Sort((a,b)=>a.room.CompareTo(b.room));
   var totals=new ForestAutoAudio();foreach(var row in forestAutoAudio){totals.consumed+=row.consumed;totals.played+=row.played;totals.muted+=row.muted;totals.culled+=row.culled;totals.missing+=row.missing;}
   var report=new ForestAutoReport{sourceNpcMeshDrawFrames=SourceNpcMeshDrawFrames,dragonNestDirectorCreatedV20=dragonNestPreviewSession?DragonNestDirectorCreatedV20:0,dragonNestDirectorRetiredV20=dragonNestPreviewSession?DragonNestDirectorRetiredV20:0,dragonNestDirectorRendererCreatedV20=dragonNestPreviewSession?DragonNestDirectorRendererCreatedV20:0,dragonNestDirectorDrawFramesV20=dragonNestPreviewSession?DragonNestDirectorDrawFramesV20:0,dragonNestDirectorCameraFramesV20=dragonNestPreviewSession?DragonNestDirectorCameraFramesV20:0,dragonNestDirectorRestoreCountV20=dragonNestPreviewSession?DragonNestDirectorRestoreCountV20:0,dragonNestDirectorNativeDepthV20=dragonNestPreviewSession?DragonNestDirectorNativeDepthV20:0,dragonNestDirectorDisplayFovV20=dragonNestPreviewSession&&dragonDirectorDocumentV20!=null?dragonDirectorDocumentV20.displayFovDegrees:0,dragonNestDirectorCameraPolicy=dragonNestPreviewSession&&dragonDirectorDocumentV20!=null?dragonDirectorDocumentV20.cameraAdapter:null,dragonNestDirectorCaptionRecordsV20=dragonNestPreviewSession?DragonNestDirectorCaptionsV20.ToArray():null,dragonNestMainMeshEffectDrawFramesV20=dragonNestPreviewSession?DragonNestMainMeshEffectDrawFramesV20:0,dragonNestMainMeshHeldDrawFramesV20=dragonNestPreviewSession?DragonNestMainMeshHeldDrawFramesV20:0,dragonNestMainMeshCreatedEffectSerialsV20=dragonNestPreviewSession?DragonNestMainMeshCreatedEffectSerialsV20:0,dragonNestMainMeshCreatedHeldInstancesV20=dragonNestPreviewSession?DragonNestMainMeshCreatedHeldInstancesV20:0,dragonNestEffectParticleDrawFramesV20=dragonNestPreviewSession?DragonNestEffectParticleDrawFramesV20:0,dragonNestEffectParticleKindsDrawnV20=dragonNestPreviewSession?DragonNestEffectParticleKindsDrawnV20:0,dragonNestEffectParticleHeldBindingEventsV20=dragonNestPreviewSession?DragonNestEffectParticleHeldBindingEventsV20:0,dragonNestEffectParticleCreatedSequencesV20=dragonNestPreviewSession?DragonNestEffectParticleCreatedSequencesV20:0,dragonNestCountdownCreatedCountV20=dragonNestPreviewSession?DragonNestCountdownCreatedCountV20:0,dragonNestCountdownTextureChangeCountV20=dragonNestPreviewSession?DragonNestCountdownTextureChangeCountV20:0,dragonNestCountdownDestroyedCountV20=dragonNestPreviewSession?DragonNestCountdownDestroyedCountV20:0,dragonNestCountdownDrawCountV20=dragonNestPreviewSession?DragonNestCountdownDrawCountV20:0,dragonNestGeyserCreatedSequencesV20=dragonNestPreviewSession?DragonNestGeyserCreatedSequencesV20:0,dragonNestGeyserWarningSequencesV20=dragonNestPreviewSession?DragonNestGeyserWarningSequencesV20:0,dragonNestGeyserFireSequencesV20=dragonNestPreviewSession?DragonNestGeyserFireSequencesV20:0,dragonNestGeyserKindsDrawnV20=dragonNestPreviewSession?DragonNestGeyserKindsDrawnV20:0,dragonNestVisualCountingScope=dragonNestPreviewSession?"Isolated consumer counters observed during ordinary route input. EffectParticle totals include the optional two-template Geyser stream; Geyser counters are its subset and must not be added again. Draw counts do not prove every template, pose or source callback was inspected.":null,lakeSourceParticleDrawFramesV18=(lakeNightPreviewSession||lakePreviewSession)?LakeSourceParticleDrawFramesV18:0,lakeSourceParticleKindsV18=(lakeNightPreviewSession||lakePreviewSession)?LakeSourceParticleKindsV18:0,status=status,reason=reason,passed=status=="victory"&&game.victory&&all&&forestAutoErrors.Count==0,victory=game.victory,allThreeRoomsReached=all&&game.RoomCount==3,allRoomsReached=all,expectedRooms=game.RoomCount,bundlePath=forestPreviewBundlePath,bundleSha256=forestPreviewBundleHash,startedUtc=forestAutoStartedUtc,finishedUtc=DateTime.UtcNow.ToString("o"),output=forestAutoOutput,room=game.room,hits=game.hits,kills=game.kills,modelSeconds=game.elapsed,wallSeconds=started?Time.realtimeSinceStartup-forestAutoWallStart:Time.realtimeSinceStartup-forestAutoArmedAt,secondsSinceProgress=started?Time.realtimeSinceStartup-forestAutoProgressAt:0,x=game.player.x,y=game.player.y,hp=game.player.hp,mana=game.mana,playerGroundLine=game.player.groundLine,playerState=game.player.state.ToString(),targetNPC=forestAutoTarget,gateOpen=game.gateOpen,combatClear=game.EncounterCombatClear,npcSoundsConsumed=totals.consumed,npcSoundsPlayed=totals.played,npcSoundsMuted=totals.muted,npcSoundsCulled=totals.culled,npcSoundsMissing=totals.missing,roomAudio=forestAutoAudio.ToArray(),rooms=forestAutoVisits.ToArray(),remaining=remaining.ToArray(),errors=forestAutoErrors.ToArray(),screenshots=forestAutoScreenshots.ToArray()};
   SaveAlteraAirshipValidationCountersV20();SaveMine2ValidationCountersV21();SaveBelowTunnelEntryCountersV21();SaveSourceInputTraceV17();File.WriteAllText(Path.Combine(forestAutoOutput,"report.json"),JsonUtility.ToJson(report,true));captureDir=forestAutoPreviousCaptureDir;Application.runInBackground=forestAutoPreviousRunInBackground;
   Debug.Log(SourcePreviewLogPrefix+"_AUTO_COMPLETE status="+status+" passed="+report.passed+" rooms="+forestAutoVisits.Count+" hits="+game.hits+" kills="+game.kills+" output="+forestAutoOutput);
  }
 }
}
#endif
