using System;
namespace ElswordPixel {
 // Presentation-only primitives. No Enemy/GameModel object is accepted or mutated.
 public static class Alpha4NormalNativeRulesV28 {
  public const string Forest="Dungeon_Elder_Druid_Forest_Normal";
  public static bool Supported(string kind,string npc){return kind=="anger-forest-ent"&&npc=="NUI_ANGER_FOREST_ENT"||kind=="anger-forest-fairy"&&npc=="NUI_ANGER_FOREST_FAIRY"||kind=="anger-forest-knight"&&npc=="NUI_ANGER_FOREST_KNIGHT"||kind=="curse-wisp"&&npc=="NUI_CURSE_WISP";}
  public static bool Scope(int dungeon,string graph,bool baseProfile,string contentSource,string kind,string npc){return dungeon==3&&graph==Forest&&baseProfile&&string.IsNullOrEmpty(contentSource)&&Supported(kind,npc);}
  public static string Prefix(string npc){return npc=="NUI_ANGER_FOREST_ENT"?"ANGER_FOREST_ENT":npc=="NUI_ANGER_FOREST_FAIRY"?"ANGER_FOREST_FAIRY":npc=="NUI_ANGER_FOREST_KNIGHT"?"ANGER_FOREST_KNIGHT":npc=="NUI_CURSE_WISP"?"CURSE_WISP":null;}
  public static bool Alias(string clip){return clip=="source-idle"||clip=="source-walk";}
  public static float Duration(string npc,string alias){if(!Alias(alias))return float.NaN;if(npc=="NUI_ANGER_FOREST_ENT")return alias=="source-idle"?8f/3:2;if(npc=="NUI_ANGER_FOREST_FAIRY")return alias=="source-idle"?4f/3:14f/15;if(npc=="NUI_ANGER_FOREST_KNIGHT"||npc=="NUI_CURSE_WISP")return alias=="source-idle"?4f/3:19f/15;return float.NaN;}
  public static int SampleCount(string npc,string alias){if(!Alias(alias))return 0;if(npc=="NUI_ANGER_FOREST_ENT")return alias=="source-idle"?65:49;if(npc=="NUI_ANGER_FOREST_FAIRY")return alias=="source-idle"?33:24;if(npc=="NUI_ANGER_FOREST_KNIGHT"||npc=="NUI_CURSE_WISP")return alias=="source-idle"?33:32;return 0;}
  public static bool ActiveTuple(int dungeon,string graph,bool baseProfile,string contentSource,string kind,string npc,bool active,bool dormant,bool visible,int combatState,string sourceState,string previous){
   if(!Scope(dungeon,graph,baseProfile,contentSource,kind,npc)||!active||dormant||!visible||combatState!=0||(previous!="idle"&&previous!="walk"))return false;
   // No empty-unalerted-idle fallback. In particular Ent START5.7/hidden/birth
   // is not implemented or certified by this display adapter.
   return sourceState==Prefix(npc)+(previous=="idle"?"_WAIT":"_WALK");
  }
  public static float ValidateClip(string npc,string alias,float libraryScale,float fps,bool loop,float rootDistance,int samples,int hits,float duration){
   float expected=Duration(npc,alias);
   if(float.IsNaN(expected)||libraryScale!=1||fps!=24||!loop||rootDistance!=0||samples!=SampleCount(npc,alias)||hits!=0||float.IsNaN(duration)||float.IsInfinity(duration)||Math.Abs(duration-expected)>.000001f)throw new InvalidOperationException("Alpha4 normal exact atlas/24Hz/header contract mismatch: "+npc+"/"+alias);
   return duration;
  }
  public static string Select(int dungeon,string graph,bool baseProfile,string contentSource,string kind,string npc,bool active,bool dormant,bool visible,int combatState,string sourceState,string previous,string sourceAnimation,bool sourceLoop,float sourceSpeed,float sourceDuration,float npcTime,float previousClock,Func<string,float> atlasDuration,out float resultClock){
   resultClock=previousClock;
   if(!ActiveTuple(dungeon,graph,baseProfile,contentSource,kind,npc,active,dormant,visible,combatState,sourceState,previous))return previous;
   string target="source-"+previous;float expected=Duration(npc,target);
   if(sourceAnimation!=(previous=="idle"?"Wait":"Walk")||!sourceLoop||sourceSpeed!=1||float.IsNaN(sourceDuration)||float.IsInfinity(sourceDuration)||Math.Abs(sourceDuration-expected)>.000001f)throw new InvalidOperationException("Alpha4 normal current source profile mismatch: "+npc+"/"+sourceState);
   float span=atlasDuration(target);if(float.IsNaN(span)||float.IsInfinity(span)||Math.Abs(span-expected)>.000001f)throw new InvalidOperationException("Alpha4 normal exact atlas duration missing: "+npc+"/"+target);
   // npcTime is model seconds. These eight pinned states have source speed1.
   // Apply that source speed once; existing Pose owns Repeat(duration)/floor24.
   float clock=npcTime*sourceSpeed;if(float.IsNaN(clock)||float.IsInfinity(clock)||clock<0)throw new InvalidOperationException("Invalid Alpha4 normal source clock");
   resultClock=clock;return target;
  }
  public static bool TryUiAnchor(bool exactActiveTuple,string actualClip,float centerX,float topY,float fullBarWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(!exactActiveTuple||!Alias(actualClip))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY)||float.IsNaN(fullBarWidth)||float.IsInfinity(fullBarWidth)||fullBarWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid Alpha4 actual bounds/HP");
   bx=centerX-fullBarWidth*(1-hp/maxHp)/2;by=topY+8;dx=centerX;dy=topY+20;return true;
  }
 }
}
