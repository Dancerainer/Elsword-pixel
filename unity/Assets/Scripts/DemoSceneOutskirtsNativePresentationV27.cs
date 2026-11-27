namespace ElswordPixel {
 public sealed partial class DemoScene {
  string OutskirtsNativeRenderClipV27(Enemy enemy,string previous,ref float clock){
   var content=CurrentNpcContentSet;bool scoped=content!=null&&content.sourceDungeon=="Dungeon_Elder_Wally_Castle_Roof_Normal";
   Library library=null;if(scoped&&OutskirtsNativeRenderRulesV27.Supported(enemy.sourceNPC))libraries.TryGetValue(enemy.kind,out library);
   float nextClock;string result=OutskirtsNativeRenderRulesV27.Select(scoped,enemy.sourceNPC,previous,OutskirtsNativeRenderRulesV27.RenderClock(enemy.sourceNPC,(int)enemy.state,enemy.npcState,previous,enemy.npcTime,enemy.clock),clock,enemy.vy,enemy.deathTime,
    target=>{AnimationData data;return library!=null&&library.clips.TryGetValue(target,out data)?data.duration:float.NaN;},out nextClock);
   clock=nextClock;return result;
  }
  void ApplyOutskirtsNativeUiV27(){
   var content=CurrentNpcContentSet;bool scoped=content!=null&&content.sourceDungeon=="Dungeon_Elder_Wally_Castle_Roof_Normal";if(!scoped)return;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var enemy=game.enemies[i];var view=enemyViews[i];if(!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)continue;
    float bx,by,dx,dy;var bounds=view.bounds;
    if(!OutskirtsNativeUiRulesV27.TryAnchor(scoped,enemy.sourceNPC,bounds.center.x,GiantWilliamWorldTopV39(view,out float authoredTop)?authoredTop:OutskirtsPaintExposureV40.Kinds(enemy.kind)?NativeVisibleTopYV28(view):bounds.max.y,enemy.maxHp>400?64:30,enemy.hp,enemy.maxHp,out bx,out by,out dx,out dy))continue;
    var bar=enemyBars[i].transform.position;bar.x=bx;bar.y=by;enemyBars[i].transform.position=bar;
    var debuff=enemyDebuffViews[i].transform.position;debuff.x=dx;debuff.y=dy;enemyDebuffViews[i].transform.position=debuff;
   }
  }
 }
}
