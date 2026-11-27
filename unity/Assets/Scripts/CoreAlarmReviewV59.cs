#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class AlarmReviewV59 {
   public int room,encounter,key,inkPixels;
   public string npc;
   public float socketError,damageHeight,beamScreenLength;
  }
  int VisibleAlarmInkV59(Enemy actor){
   var visible=new List<SpriteRenderer>();
   if(!alarmLightRangesV59.TryGetValue(actor,out var range))return 0;
   for(int i=range.x;i<range.y;i++){var r=coreAlarmBeamViewsR1417[i];if(r&&r.enabled&&r.gameObject.activeInHierarchy)visible.Add(r);}
   var on=CoreReviewWorldFrameV59();Texture2D off=null;
   try{
    foreach(var r in visible)r.enabled=false;
    off=CoreReviewWorldFrameV59();var a=on.GetPixels32();var b=off.GetPixels32();int changed=0;
    for(int i=0;i<a.Length;i++)if(a[i].r!=b[i].r||a[i].g!=b[i].g||a[i].b!=b[i].b)changed++;
    return changed;
   }finally{foreach(var r in visible)r.enabled=true;Destroy(on);if(off)Destroy(off);}
  }
  internal IEnumerator ReviewCoreLasersV59(string output){
   EnterCoreReviewRoomV59(0,0);
   ReviewAlarmContactV59(output);
   var report=new List<string>();
   foreach(int room in new[]{0,2}){
    EnterCoreReviewRoomV59(room,0);
    int encounters=game.EncounterCount;
    for(int ei=0;ei<encounters;ei++){
     if(ei!=0)EnterCoreReviewRoomV59(room,ei);
     var alarms=game.enemies.FindAll(e=>IsCoreAlarmWholeIdV21(e.sourceNPC)&&e.encounterIndex==ei);
     foreach(var actor in alarms){
      game.player.x=actor.spawnX+20;game.player.y=actor.y;game.player.invincible=100;
      bool warned=false;double until=Time.realtimeSinceStartupAsDouble+30;
      for(;;){
       if(Time.realtimeSinceStartupAsDouble>until)throw new TimeoutException("Core alarm did not fire "+actor.sourceNPC+" room "+room+"/"+ei);
       yield return null;
       CoreHardAlarmBeamViewR1417 beam=null;
       foreach(var b in game.CoreHardAlarmBeamViewsR1417())if(b.ownerEnemy==actor){beam=b;break;}
       if(beam==null||!monsterPixelViewsV53.TryGetValue(actor,out var view)||!view.renderer.enabled)continue;
       if(beam.phase=="warning"){
        if(!warned&&report.Count==0){CaptureCoreReviewFrameV59(Path.Combine(output,"laser-warning.png"));warned=true;}
        continue;
       }
       // Give the standard follow camera time to catch up after changing device.
       if(!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(worldCamera),view.renderer.bounds))continue;
       Vector2 lane=AlarmLaneV59(actor);
       Vector2 projected=lane+AlarmBackV59;
       var row=new AlarmReviewV59{room=room,encounter=ei,key=actor.sourceKeyCode,npc=actor.sourceNPC,
        socketError=Vector2.Distance(projected,view.renderer.bounds.center),damageHeight=beam.startY-actor.y,inkPixels=VisibleAlarmInkV59(actor),beamScreenLength=Vector2.Distance(worldCamera.WorldToScreenPoint(lane),worldCamera.WorldToScreenPoint(projected))};
       if(row.socketError>2||row.inkPixels<24||row.beamScreenLength<16)throw new InvalidOperationException("Unclear live alarm "+JsonUtility.ToJson(row));
       if(room==0)CaptureCoreReviewFrameV59(Path.Combine(output,"laser-"+actor.kind+".png"));
       report.Add(JsonUtility.ToJson(row));break;
      }
     }
    }
   }
   if(report.Count!=11)throw new InvalidOperationException("Expected 11 alarm devices, found "+report.Count);
   File.WriteAllLines(Path.Combine(output,"laser-devices.jsonl"),report);
  }
  void ReviewAlarmContactV59(string output){
   var evidence=new List<string>();
   foreach(string mode in new[]{"blue-hit","blue-jump","green-walk"}){
    EnterCoreReviewRoomV59(0,0);
    bool green=mode=="green-walk",jump=mode=="blue-jump";
    var target=CoreReviewLivingV59(green?"NUI_ALARM_SYSTEM_B":"NUI_ALARM_SYSTEM_C");
    game.player.x=target.spawnX+(mode=="blue-hit"?-24:jump?45:25);game.player.y=target.y;
    game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;
    game.player.hp=game.player.maxHp;game.player.invincible=100;
    game.Step(1f/60,new Input());
    int before=game.CoreHardAlarmDamageHitsR1417, begin=-1;float hp=0,minDx=999,height=0;
    bool warned=false,armed=false,overlapped=false,done=false;
    for(int tick=0;tick<720;tick++){
     // Remove the other combatants through standard damage while the scanner
     // warms up. Their attacks and body blocking would invalidate an isolated
     // test of whether walking or jumping crosses this particular beam.
     if(begin<0){
      game.player.invincible=100;
      foreach(var enemy in game.enemies.ToArray())if(enemy.active&&!enemy.dormant&&enemy.hp>0&&!enemy.passive&&!enemy.isGate&&enemy.countsForClear)
       game.ApplyHitToEnemy(enemy,100000,HitReaction.Hurt);
     }
     bool start=begin<0&&armed&&(mode=="blue-hit"||jump&&target.npcState.EndsWith("_ATTACK2")&&target.npcTime>.25f||green&&target.npcState.EndsWith("_TURN2")&&target.npcTime<.09f);
     if(start){begin=tick;game.player.invincible=0;hp=game.player.hp;before=game.CoreHardAlarmDamageHitsR1417;}
     var input=new Input();
     if(begin>=0){input.move=mode=="blue-hit"?1:-1;input.jump=jump&&tick==begin;}
     game.Step(1f/60,input);
     foreach(var b in game.CoreHardAlarmBeamViewsR1417())if(b.ownerEnemy==target){
      warned|=b.phase=="warning";armed|=b.phase=="active";
      if(begin<0||b.phase!="active")continue;
      float dx=game.player.x-b.startX,dy=game.player.y-b.startY;
      if(Math.Abs(dx)<minDx){minDx=Math.Abs(dx);height=dy;}
      if(Math.Abs(dx)<9.8f+b.halfWidth)overlapped=true;
      bool hit=game.CoreHardAlarmDamageHitsR1417>before&&game.player.hp<hp;
      if(mode=="blue-hit")done=hit;
      else if(hit||game.player.hp<hp)throw new InvalidOperationException(mode+" took damage at dx="+dx+" dy="+dy+" other="+(game.CoreHardAlarmDamageHitsR1417-before));
      else done=overlapped&&dx< -13;
      if(done)break;
     }
     if(done)break;
    }
    string stats=mode+" warned="+warned+" armed="+armed+" HP "+hp+" -> "+game.player.hp+" hitCount="+(game.CoreHardAlarmDamageHitsR1417-before)+" minimumDx="+minDx+" footAboveBeam="+height+" player="+game.player.x+"/"+game.player.y+" actor="+target.x+"/"+target.y+" phase="+target.npcState+" tickBegin="+begin;
    if(!done||!warned||!armed)throw new InvalidOperationException(stats);
    evidence.Add(stats);
   }
   File.WriteAllLines(Path.Combine(output,"laser-contact.txt"),evidence);
  }
 }
}
#endif
