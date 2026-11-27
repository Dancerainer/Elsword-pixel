using System;
namespace ElswordPixel {
 public static class Next3NormalNativeRulesV28 {
  public const string Cave="Dungeon_Elder_Benders_Cave_Normal",Forest="Dungeon_Elder_Druid_Forest_Normal";
  public static bool Supported(string kind,string npc){return kind=="benders-upgrade-normal"&&npc=="NUI_BENDERS_UPGRADE_NORMAL"||kind=="thief-fat-boss"&&npc=="NUI_THIEF_FAT_BOSS"||kind=="forest-dark-magician"&&npc=="NUI_FOREST_DARK_MAGICIAN";}
  public static bool Scope(int dungeon,string graph,bool baseProfile,string content,string kind,string npc){
   if(!Supported(kind,npc))return false;
   if(npc=="NUI_FOREST_DARK_MAGICIAN")return dungeon==3&&graph==Forest&&baseProfile&&string.IsNullOrEmpty(content);
   return dungeon==4&&graph==Cave&&!baseProfile&&content==Cave;
  }
  public static string Prefix(string npc){return npc=="NUI_BENDERS_UPGRADE_NORMAL"?"BENDERS_UPGRADE_NORMAL":npc=="NUI_THIEF_FAT_BOSS"?"THIEF_FAT_BOSS":npc=="NUI_FOREST_DARK_MAGICIAN"?"FOREST_DARK_MAGICIAN":null;}
  public static bool Alias(string clip){return clip=="source-idle"||clip=="source-walk";}
  public static float Duration(string npc,string alias){
   if(!Alias(alias))return float.NaN;
   if(npc=="NUI_BENDERS_UPGRADE_NORMAL")return alias=="source-idle"?5f/3:2f/3;
   if(npc=="NUI_THIEF_FAT_BOSS")return alias=="source-idle"?6f/5:2f/3;
   if(npc=="NUI_FOREST_DARK_MAGICIAN")return alias=="source-idle"?1f:17f/15;
   return float.NaN;
  }
  public static string Select(int dungeon,string graph,bool baseProfile,string content,string kind,string npc,int combatState,bool alerted,string sourceState,string previous,float npcTime,float actorClock,float previousClock,Func<string,float> duration,out float resultClock){
   resultClock=previousClock;
   if(!Scope(dungeon,graph,baseProfile,content,kind,npc)||combatState!=0||(previous!="idle"&&previous!="walk"))return previous;
   bool active=sourceState==Prefix(npc)+(previous=="idle"?"_WAIT":"_WALK");
   bool birth=!alerted&&string.IsNullOrEmpty(sourceState)&&previous=="idle";
   if(!active&&!birth)return previous;
   string target="source-"+previous;float span=duration(target),expected=Duration(npc,target);
   if(float.IsNaN(span)||float.IsInfinity(span)||Math.Abs(span-expected)>.000001f)throw new InvalidOperationException("Next3 normal source duration mismatch: "+kind+"/"+target);
   float clock=active?npcTime*1f:actorClock;
   if(float.IsNaN(clock)||float.IsInfinity(clock)||clock<0)throw new InvalidOperationException("Invalid Next3 normal display clock");
   resultClock=clock;return target;
  }
  public static float EffectiveScale(int dungeon,string graph,bool baseProfile,string content,string kind,string npc,string actualClip,float legacyScale,float visualScale){
   if(!Scope(dungeon,graph,baseProfile,content,kind,npc)||kind!="thief-fat-boss"||npc!="NUI_THIEF_FAT_BOSS"||!Alias(actualClip))return legacyScale;
   if(float.IsNaN(visualScale)||float.IsInfinity(visualScale)||visualScale<=0)throw new InvalidOperationException("Invalid Next3 FatBoss visualScale");
   // Original UNIT_SCALE1.4 is already in the .392-unit native raw geometry.
   // Preserve old header logicalScale1.4 and every old clip's legacy scale.
   return visualScale;
  }
  public static bool TryUiAnchor(int dungeon,string graph,bool baseProfile,string content,string kind,string npc,string actualClip,float centerX,float topY,float fullBarWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;
   if(!Scope(dungeon,graph,baseProfile,content,kind,npc)||!Alias(actualClip))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY)||float.IsNaN(fullBarWidth)||float.IsInfinity(fullBarWidth)||fullBarWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid actual Next3 normal bounds/HP");
   bx=centerX-fullBarWidth*(1-hp/maxHp)/2;by=topY+8;dx=centerX;dy=topY+20;return true;
  }
 }
}
