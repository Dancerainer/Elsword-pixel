using System;
namespace ElswordPixel {
 // Presentation only. No Enemy/GameModel object is accepted or mutated here.
 public static class CaveNormalNativeRulesV28 {
  public const string Dungeon="Dungeon_Elder_Benders_Cave_Normal";
  public static bool Supported(string kind,string npc){return kind=="thief-thin"&&npc=="NUI_THIEF_THIN"||kind=="thief-fat"&&npc=="NUI_THIEF_FAT"||kind=="pporu-giant-red"&&npc=="NUI_PPORU_GIANT_RED";}
  public static string Prefix(string npc){return npc=="NUI_THIEF_THIN"?"THIEF_THIN":npc=="NUI_THIEF_FAT"?"THIEF_FAT":npc=="NUI_PPORU_GIANT_RED"?"PPORU_GIANT_RED":null;}
  public static bool Alias(string clip){return clip=="source-idle"||clip=="source-walk";}
  public static float Duration(string npc,string alias){
   if(!Alias(alias))return float.NaN;
   if(npc=="NUI_THIEF_THIN")return alias=="source-idle"?1f:8f/15;
   if(npc=="NUI_THIEF_FAT")return alias=="source-idle"?6f/5:2f/3;
   if(npc=="NUI_PPORU_GIANT_RED")return 4f/3;
   return float.NaN;
  }
  public static string Select(string sourceDungeon,string kind,string npc,int combatState,bool alerted,string sourceState,string previous,float npcTime,float actorClock,float previousClock,Func<string,float> duration,out float resultClock){
   resultClock=previousClock;
   if(sourceDungeon!=Dungeon||!Supported(kind,npc)||combatState!=0||(previous!="idle"&&previous!="walk"))return previous;
   string prefix=Prefix(npc);bool active=sourceState==prefix+(previous=="idle"?"_WAIT":"_WALK");
   bool unalertedEmptyIdle=!alerted&&string.IsNullOrEmpty(sourceState)&&previous=="idle";
   if(!active&&!unalertedEmptyIdle)return previous;
   string target="source-"+previous;float span=duration(target),expected=Duration(npc,target);
   if(float.IsNaN(span)||float.IsInfinity(span)||Math.Abs(span-expected)>.000001f)throw new InvalidOperationException("Cave normal source duration mismatch: "+kind+"/"+target);
   // All exact current six source speeds are 1. Pose owns Repeat(duration).
   float clock=active?npcTime*1f:actorClock;
   if(float.IsNaN(clock)||float.IsInfinity(clock)||clock<0)throw new InvalidOperationException("Invalid Cave normal display clock");
   resultClock=clock;return target;
  }
  public static bool TryUiAnchor(string sourceDungeon,string kind,string npc,string actualClip,float centerX,float topY,float fullBarWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;
   if(sourceDungeon!=Dungeon||!Supported(kind,npc)||!Alias(actualClip))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY)||float.IsNaN(fullBarWidth)||float.IsInfinity(fullBarWidth)||fullBarWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid actual Cave normal bounds/HP");
   bx=centerX-fullBarWidth*(1-hp/maxHp)/2;by=topY+8;dx=centerX;dy=topY+20;return true;
  }
 }
}
