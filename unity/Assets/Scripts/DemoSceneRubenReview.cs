using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool rubenReview;int reviewPhase,reviewArea=-1,reviewTick;float reviewAt;readonly List<RubenResult> reviewResults=new List<RubenResult>();CombatAudioProbe reviewProbe;
  [Serializable] class RubenResult{public string id;public int rooms,kills,hits,hurtSounds,impactSounds;public bool clear;public float seconds,hp;}
  [Serializable] class RubenReport{public bool passed;public RubenResult[] dungeons;public int menuCards=3;public string audioEvidence="combat-mix.wav";}
  void BeginRubenReview(){if(AudioMuted)ToggleAudio();SetMenu("dungeons");reviewAt=0;reviewPhase=0;}
  void RenderReviewMenu(){RenderActors();RenderHud();RenderCombatPresentation();FinishNativeProjection();TickAudio();}
  void UpdateRubenReview(){
   frame++;wallClock+=Time.unscaledDeltaTime;reviewTick++;
   if(reviewPhase==0){RenderActors();RenderHud();RenderCombatPresentation();FinishNativeProjection();TickAudio();if(reviewTick==40){Capture("00-dungeon-select");reviewPhase=1;reviewAt=wallClock;}return;}
   if(reviewPhase==20){RenderReviewMenu();if(wallClock-reviewAt>.25f){SetMenu("dungeons");reviewPhase=1;reviewAt=wallClock;}return;}
   if(reviewPhase==1){RenderReviewMenu();if(wallClock-reviewAt<.5f)return;MenuAction("dungeon-preview:"+reviewResults.Count);MenuAction("loadout");reviewPhase=11;reviewAt=wallClock;return;}
   if(reviewPhase==11){RenderReviewMenu();if(wallClock-reviewAt>.25f){Capture("loadout-"+reviewResults.Count);reviewPhase=12;reviewAt=wallClock;}return;}
   if(reviewPhase==12){RenderReviewMenu();if(wallClock-reviewAt>.15f){MenuAction("menu-close");if(openMenu!="dungeons")throw new InvalidOperationException("Loadout must return to the adventure map.");reviewPhase=13;reviewAt=wallClock;}return;}
   if(reviewPhase==13){RenderReviewMenu();if(wallClock-reviewAt>.25f){Capture("map-"+reviewResults.Count);reviewPhase=14;reviewAt=wallClock;}return;}
   if(reviewPhase==14){RenderReviewMenu();if(wallClock-reviewAt<.15f)return;EnterSelectedDungeon();reviewPhase=2;reviewAt=wallClock;reviewArea=-1;
    if(reviewResults.Count==0){reviewProbe=worldCamera.gameObject.AddComponent<CombatAudioProbe>();reviewProbe.Begin();}return;}
   accumulator+=Math.Min(Time.unscaledDeltaTime,.1f);int steps=0;
   while(accumulator>=1f/60&&steps++<6){RememberSimulationState();Input input;
    if(reviewResults.Count==0&&wallClock-reviewAt<12&&game.PlayerImpactSerial==0){input=new Input{skill=-1};Enemy first=null;foreach(var e in game.enemies)if(!e.passive){first=e;break;}if(first!=null&&Math.Abs(first.x-game.player.x)>34)input.move=1;}
    else input=Autopilot();game.Step(1f/60,input);accumulator-=1f/60;TickAudio();}
   if(shownRoom!=game.room)RebuildRoom();RenderActors();RenderEffects();RenderHud();RenderCombatGeometry();RenderCombatPresentation();FinishNativeProjection();TickAudio();SamplePerformance();
   if(reviewArea!=game.room){reviewArea=game.room;Capture(Dungeon.id+"-area-"+(game.room+1));}
   if(reviewTick%180==0&&game.hits>0)Capture(Dungeon.id+"-combat");
   if(game.victory||game.player.hp<=0||wallClock-reviewAt>200){
    reviewResults.Add(new RubenResult{id=Dungeon.id,rooms=game.RoomCount,kills=game.kills,hits=game.hits,hp=game.player.hp,clear=game.victory,seconds=wallClock-reviewAt,hurtSounds=AudioHurtPlays,impactSounds=AudioImpactPlays});Capture(Dungeon.id+"-clear");
    if(reviewProbe){reviewProbe.Save(Path.Combine(captureDir,"combat-mix.wav"));Destroy(reviewProbe);reviewProbe=null;}
    if(reviewResults.Count<3){reviewPhase=20;reviewAt=wallClock;}
    else {bool pass=true;foreach(var r in reviewResults)pass&=r.clear;Directory.CreateDirectory(captureDir);File.WriteAllText(Path.Combine(captureDir,"ruben-review.json"),JsonUtility.ToJson(new RubenReport{passed=pass,dungeons=reviewResults.ToArray()},true));WritePerformanceReport();rubenReview=false;StartCoroutine(QuitAfterCapture());}
   }
  }
 }
}
