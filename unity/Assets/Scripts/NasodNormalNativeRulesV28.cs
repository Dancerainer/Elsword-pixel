using System;
namespace ElswordPixel {
 // Display selection only. Source WAIT/WALK clocks and model motion stay intact.
 public static class NasodNormalNativeRulesV28 {
  public const string Dungeon="Dungeon_Elder_Below_Passage_Normal";
  public static bool Supported(bool scope,string kind,string npc){return scope&&kind=="benders-nasod"&&npc=="NUI_BENDERS_NASOD";}
  public static bool Alias(string clip){return clip=="source-idle"||clip=="source-walk";}
  public static float Duration(string alias){return alias=="source-idle"?5f/3:alias=="source-walk"?2f/3:float.NaN;}
  public static string Select(bool scope,string kind,string npc,int combatState,string sourceState,string previous,float npcTime,float previousClock,Func<string,float> duration,out float resultClock){
   resultClock=previousClock;
   if(!Supported(scope,kind,npc)||combatState!=0||(previous!="idle"&&previous!="walk"))return previous;
   string state=previous=="idle"?"BENDERS_NASOD_WAIT":"BENDERS_NASOD_WALK";
   // Empty birth, START_READY, attacks and reactions retain their existing paths.
   if(sourceState!=state)return previous;
   string target="source-"+previous;float span=duration(target),expected=Duration(target);
   if(float.IsNaN(span)||float.IsInfinity(span)||Math.Abs(span-expected)>.000001f)throw new InvalidOperationException("Nasod normal source duration mismatch");
   if(float.IsNaN(npcTime)||float.IsInfinity(npcTime)||npcTime<0)throw new InvalidOperationException("Invalid Nasod normal source clock");
   // Source speed is1. Pose owns Repeat(duration); no RootDistance is applied.
   resultClock=npcTime;return target;
  }
  public static bool TryUiAnchor(bool scope,string kind,string npc,string actualClip,float centerX,float topY,float fullBarWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;
   if(!Supported(scope,kind,npc)||!Alias(actualClip))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY)||float.IsNaN(fullBarWidth)||float.IsInfinity(fullBarWidth)||fullBarWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid Nasod normal bounds/HP");
   bx=centerX-fullBarWidth*(1-hp/maxHp)/2;by=topY+8;dx=centerX;dy=topY+20;return true;
  }
 }
}
