using System;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  string WaterwayNativeRenderClipV26(Enemy enemy,string previous,ref float clock){
   var content=CurrentNpcContentSet;bool scoped=content!=null&&content.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal";
   // Source START is already selected by its actual state. House remains the
   // exact existing Wait alias. R10Tree/Bat selectors and registration stay intact.
   Library library=null;
   if(scoped&&(WaterwayNativeRenderRulesV26.Mouse(enemy.sourceNPC)||WaterwayNativeRenderRulesV26.Three(enemy.sourceNPC)))libraries.TryGetValue(enemy.kind,out library);
   float nextClock;
   string result=WaterwayNativeRenderRulesV26.Select(scoped,enemy.sourceNPC,previous,enemy.clock,clock,enemy.vy,enemy.deathTime,
    target=>{AnimationData data;return library!=null&&library.clips.TryGetValue(target,out data)?data.duration:float.NaN;},out nextClock);
   clock=nextClock;return result;
  }
 }
}
