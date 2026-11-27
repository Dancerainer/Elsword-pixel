namespace ElswordPixel {
 public sealed partial class DemoScene {
  void ApplyWaterwayNativeUiV26(){
   var content=CurrentNpcContentSet;bool scoped=content!=null&&content.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal";
   if(!scoped)return;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var enemy=game.enemies[i];var view=enemyViews[i];
    if(!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)continue;
    float bx,by,dx,dy;var bounds=view.bounds;
    if(!WaterwayNativeUiRulesV26.TryAnchor(scoped,enemy.sourceNPC,bounds.center.x,bounds.max.y,enemy.maxHp>400?64:30,enemy.hp,enemy.maxHp,out bx,out by,out dx,out dy))continue;
    var bar=enemyBars[i].transform.position;bar.x=bx;bar.y=by;enemyBars[i].transform.position=bar;
    var debuff=enemyDebuffViews[i].transform.position;debuff.x=dx;debuff.y=dy;enemyDebuffViews[i].transform.position=debuff;
   }
  }
 }
}
