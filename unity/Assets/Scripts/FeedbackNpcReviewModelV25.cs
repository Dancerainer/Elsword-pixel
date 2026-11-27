using System;
namespace ElswordPixel {
 [Serializable] public sealed class FeedbackNpcSupportV25 {public int surface;public float x;}
 [Serializable] public sealed class FeedbackNpcSampleV25 {public string id,phase;public float phaseElapsed;public int minimumSteps,maxSteps=360;public bool triggerNear;}
 [Serializable] public sealed class FeedbackNpcCaseV25 {public string id,stage,npc;public int dungeon,room,stagedIndex,sourceSpawnIndex,encounter;public FeedbackNpcSupportV25 far,near;public FeedbackNpcSampleV25[] samples;}
 [Serializable] public sealed class FeedbackNpcPlanV25 {public string schema,catalogSha256;public FeedbackNpcCaseV25[] cases;}
 // Shared by the Windows capture partial and the actual-GameModel CPU preflight.
 // Only player support is deliberately positioned. No enemy/HP/clock is assigned.
 public static class FeedbackNpcModelStepsV25 {
  public static void LocatePlayer(GameModel g,FeedbackNpcSupportV25 point){
   if(point==null)throw new ArgumentException("Missing explicit player support");var line=g.Surface(point.surface);
   if(line==null||point.x<line.x1||point.x>line.x2)throw new ArgumentException("Player locator outside original support");
   if(g.HasEncounters&&(point.x<g.CurrentEncounter.leftX||point.x>g.CurrentEncounter.rightX))throw new ArgumentException("Player locator outside actual source encounter");
   g.player.x=point.x;g.player.y=line.Y(point.x);g.player.groundLine=line.id;g.player.vx=g.player.vy=0;
  }
  public static Enemy Subject(GameModel g,FeedbackNpcCaseV25 c){
   if(g.CurrentRoom.id!=c.stage||c.stagedIndex<0||c.stagedIndex>=g.enemies.Count)throw new ArgumentException("Actual stage/birth index mismatch");
   var e=g.enemies[c.stagedIndex];if(e.sourceNPC!=c.npc||!e.active||e.dormant||!e.aiEnabled||e.hp<=0)throw new ArgumentException("Actual birth identity/eligibility changed");
   if(c.npc!="NUI_TREE_KNIGHT"&&c.npc!="NUI_BAT_A"&&c.npc!="NUI_BAT_B")throw new ArgumentException("Only current first3 identities");return e;
  }
  public static int Advance(GameModel g,Enemy e,FeedbackNpcCaseV25 c,FeedbackNpcSampleV25 s){
   if(s==null||s.maxSteps<1||s.maxSteps>1200||s.minimumSteps<0||s.minimumSteps>s.maxSteps||s.phaseElapsed<0)throw new ArgumentException("Invalid explicit sample bounds");
   if(s.triggerNear)LocatePlayer(g,c.near);int steps=0;
   try{while(steps<s.maxSteps){string phase=g.EnemyInitialPosePhaseV25(e);float clock=g.EnemyInitialPoseElapsedV25(e);
     if(steps>=s.minimumSteps&&phase==s.phase&&(s.phase=="complete"||clock+.0001f>=s.phaseElapsed))return steps;
     if(e.hp<=0||!e.active||g.player.hp<=0||g.room!=c.room||g.victory)throw new InvalidOperationException("Actual model changed room/died before requested phase");
     g.paused=false;g.Step(1f/60,new Input{skill=-1});g.paused=true;steps++;
   }}finally{g.paused=true;}
   throw new InvalidOperationException("Requested source phase not reached: "+c.id+"/"+s.id+" actual="+g.EnemyInitialPosePhaseV25(e)+" t="+g.EnemyInitialPoseElapsedV25(e));
  }
 }
}
