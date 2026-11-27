#if UNITY_EDITOR
using System;using System.IO;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class Mine2FastEnemyV21 {public string npc,state,combat;public float x,y,hp,clock,invincible,hitPause,recovery;public int line;public bool countsForClear;}
  [Serializable] sealed class Mine2FastTickV21 {public float time,x,y,mana,worldStop,move;public int hits,kills,skill,held;public bool busy,attack,jump;public string target,activeSkill,command,phase;}
  [Serializable] sealed class Mine2FastReportV21 {public string schema="mine2-controlled-fast-input-probe-v21",status,failure,target;public bool passed,actualUnityPlayMode=true,inputOnly=true,naturalRoute=false,renderAudioVerified=false;public int room,hits,kills,ticks;public float seconds,x,y,hp;public Mine2FastEnemyV21[] remaining;public List<Mine2FastTickV21> trace=new List<Mine2FastTickV21>();}
  public void RunMine2FastInputProbeV21(string output){
   if(!Application.isBatchMode||!Application.isPlaying||!Mine2PreviewActiveV21)throw new InvalidOperationException("Mine2 fast probe requires actual loaded controlled PlayMode");
   var r=new Mine2FastReportV21();try{for(int tick=0;tick<1200*60;tick++){
    if(game.victory){r.status="victory";r.passed=true;break;}if(game.player.hp<=0){r.status="defeated";break;}
    float floor=float.PositiveInfinity;foreach(var s in game.CurrentRoom.surfaces)floor=Math.Min(floor,Math.Min(s.y1,s.y2));if(game.player.y<floor-600){r.status="out-of-world";break;}
    var input=ForestAutoInput(new Input{skill=-1});if(tick%300==0)r.trace.Add(new Mine2FastTickV21{time=game.elapsed,x=game.player.x,y=game.player.y,mana=game.mana,worldStop=game.SkillWorldStopRemaining,move=input.move,hits=game.hits,kills=game.kills,skill=input.skill,held=input.skillHeldMask,busy=game.Busy(),attack=input.attack,jump=input.jump,target=forestAutoTarget,activeSkill=game.ActiveSkillId,command=game.CurrentCommand,phase=game.SkillPhaseLabel});game.Step(1f/60,input);r.ticks=tick+1;
   }if(string.IsNullOrEmpty(r.status))r.status="model-time-budget";}catch(Exception e){r.status="exception";r.failure=e.ToString();}
   forestAutoArmed=forestAutoRunning=false;r.room=game.room;r.hits=game.hits;r.kills=game.kills;r.seconds=game.elapsed;r.x=game.player.x;r.y=game.player.y;r.hp=game.player.hp;r.target=forestAutoTarget;var remaining=new List<Mine2FastEnemyV21>();foreach(var e in game.enemies)if(e.active&&e.hp>0)remaining.Add(new Mine2FastEnemyV21{npc=e.sourceNPC,state=e.npcState,combat=e.state.ToString(),x=e.x,y=e.y,hp=e.hp,line=e.groundLine,clock=e.clock,invincible=e.invincible,hitPause=e.hitPause,recovery=e.recoveryTime,countsForClear=e.countsForClear});r.remaining=remaining.ToArray();File.WriteAllText(Path.Combine(output,"fast-report.json"),JsonUtility.ToJson(r,true));
  }
 }
}
#endif
