using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool feedbackReview,feedbackFinished,feedbackOriginalMute,feedbackPendingNext,feedbackDropCaptured,feedbackHpCaptured,feedbackMpCaptured;
  int feedbackCase,feedbackHitsBefore,feedbackMaterialBefore;float feedbackStarted,feedbackNextPress;
  CombatAudioProbe feedbackProbe;
  readonly List<FeedbackContactResult> feedbackContacts=new List<FeedbackContactResult>();
  static readonly string[] FeedbackKinds={"thief","anger-forest-ent","down-stone-trap","soldier-thin","curse-wisp"};
  static readonly string[] FeedbackNames={"flesh","wood","shell","metal","spirit"};
  [Serializable] sealed class FeedbackContactResult {public string material,kind;public int hits,materialCues;public bool passed;}
  [Serializable] sealed class FeedbackReviewReport {
   public bool passed;public FeedbackContactResult[] contacts;public int drops,pickups,pickupSounds,missingPickupSounds;
   public float hp,mp;public string scope="Actual Windows Player renderer, real authored sword inputs and AudioListener mix. Flat practice fixtures configure targets and initial HP380/MP120 before the recovery case; no in-flight HP, damage, position or drop edits. Full source dungeon clears are tested separately.";
  }
  void BeginFeedbackReview(){
   Directory.CreateDirectory(captureDir);adventureStarted=true;openMenu="";game.paused=false;
   game.SetNpcDefinitions(null);game.SetNpcEffectDefinitions(null);game.SetNpcSoundDefinitions(null);game.SetNpcMechanismDefinitions(null);
   feedbackOriginalMute=audioMuted;audioMuted=false;AudioListener.pause=false;AudioListener.volume=1;
   feedbackProbe=worldCamera.gameObject.AddComponent<CombatAudioProbe>();feedbackProbe.Begin();StartFeedbackCase();
  }
  void StartFeedbackCase(){
   var room=new RoomDefinition{id="feedback-lab",title="CONTACT / RECOVERY LAB",width=1440,spawnX=180,exitX=1380};
   if(feedbackCase<5)room.enemies.Add(new EnemySpawn{kind=FeedbackKinds[feedbackCase],x=224,y=54,hp=50000,aggroRange=0,aiEnabled=false});
   else{
    room.enemies.Add(new EnemySpawn{kind="prop-box",sourceNPC="NUI_BOX",x=224,y=54,hp=60,passive=true,countsForClear=false,aiEnabled=false});
    room.enemies.Add(new EnemySpawn{kind="prop-chest",sourceNPC="NUI_CHEST",x=350,y=54,hp=60,passive=true,countsForClear=false,aiEnabled=false});
   }
   game.SetRooms(new[]{room});ApplyTimings();game.roomBanner=0;
   foreach(var e in game.enemies)e.cooldown=999;
   if(feedbackCase==5){game.player.hp=380;game.mana=120;}
   feedbackStarted=wallClock;feedbackNextPress=0;feedbackHitsBefore=game.hits;
   feedbackMaterialBefore=feedbackCase<5?AudioMaterialImpactPlays[feedbackCase]:0;
   accumulator=0;ClearInput();RebuildRoom();RememberSimulationState();ResetAudioTracking();
  }
  Input FeedbackReviewInput(){
   var input=new Input{skill=-1};
   if(feedbackCase<5){if(game.hits==0&&game.elapsed>=feedbackNextPress){input.attack=true;feedbackNextPress=game.elapsed+.18f;}return input;}
   RecoveryDrop wanted=null;foreach(var item in game.recoveryDrops)if(!game.RecoveryResourceFull(item)){wanted=item;break;}
   if(wanted!=null){float gap=wanted.x-game.player.x;input.move=Math.Abs(gap)>3?Math.Sign(gap):0;return input;}
   Enemy box=null;foreach(var e in game.enemies)if(GameModel.IsRecoveryContainer(e)&&e.hp>0){box=e;break;}
   if(box!=null){float gap=box.x-game.player.x;input.move=Math.Abs(gap)>30||gap*game.player.facing<0?Math.Sign(gap):0;if(Math.Abs(gap)<65&&game.elapsed>=feedbackNextPress){input.attack=true;feedbackNextPress=game.elapsed+.18f;}}
   return input;
  }
  void UpdateFeedbackReview(){
   if(feedbackFinished)return;if(feedbackPendingNext){feedbackPendingNext=false;StartFeedbackCase();}frame++;wallClock+=Time.unscaledDeltaTime;accumulator+=Math.Min(Time.unscaledDeltaTime,.1f);int steps=0;
   while(accumulator>=1f/60&&steps++<6){RememberSimulationState();game.Step(1f/60,FeedbackReviewInput());accumulator-=1f/60;TickAudio();}
   RenderActors();RenderEffects();RenderHud();RenderCombatGeometry();RenderCombatPresentation();FinishNativeProjection();TickAudio();SamplePerformance();
   if(feedbackCase<5){
    if(wallClock-feedbackStarted>1.5f&&game.hits>0){
     var result=new FeedbackContactResult{material=FeedbackNames[feedbackCase],kind=FeedbackKinds[feedbackCase],hits=game.hits-feedbackHitsBefore,materialCues=AudioMaterialImpactPlays[feedbackCase]-feedbackMaterialBefore};result.passed=result.hits>0&&result.materialCues>0;feedbackContacts.Add(result);
     Capture("0"+(feedbackCase+1)+"-"+FeedbackNames[feedbackCase]);feedbackCase++;feedbackPendingNext=true;
    }else if(wallClock-feedbackStarted>6)FinishFeedbackReview(false);
   }else{
    if(!feedbackDropCaptured&&game.recoveryDrops.Count>0&&game.recoveryDrops[0].grounded){feedbackDropCaptured=true;Capture("06-box-drop");}
    if(!feedbackHpCaptured&&game.RecoveryDropsPickedUp>=1){feedbackHpCaptured=true;Capture("07-hp-collected");}
    if(!feedbackMpCaptured&&game.RecoveryDropsPickedUp>=2){feedbackMpCaptured=true;Capture("08-mp-collected");}
    if(game.RecoveryDropsPickedUp>=2&&game.elapsed>4){FinishFeedbackReview(true);}
    else if(wallClock-feedbackStarted>25)FinishFeedbackReview(false);
   }
  }
  void FinishFeedbackReview(bool completed){
   feedbackFinished=true;bool pass=completed&&feedbackContacts.Count==5&&game.RecoveryDropsPickedUp==2&&RecoveryPickupSoundPlays>=2&&RecoveryPickupMissingSounds==0&&game.player.hp==500;
   foreach(var contact in feedbackContacts)pass&=contact.passed;
   var report=new FeedbackReviewReport{passed=pass,contacts=feedbackContacts.ToArray(),drops=game.RecoveryDropsSpawned,pickups=game.RecoveryDropsPickedUp,pickupSounds=RecoveryPickupSoundPlays,missingPickupSounds=RecoveryPickupMissingSounds,hp=game.player.hp,mp=game.mana};
   File.WriteAllText(Path.Combine(captureDir,"feedback-review.json"),JsonUtility.ToJson(report,true));
   if(feedbackProbe){feedbackProbe.Save(Path.Combine(captureDir,"feedback-mix.wav"));Destroy(feedbackProbe);feedbackProbe=null;}
   Capture(pass?"09-feedback-pass":"09-feedback-fail");WritePerformanceReport();audioMuted=feedbackOriginalMute;
   Debug.Log("FEEDBACK_REVIEW_"+(pass?"PASS":"FAIL")+" pickups="+game.RecoveryDropsPickedUp+" sound="+RecoveryPickupSoundPlays);StartCoroutine(QuitAfterCapture());
  }
 }
}
