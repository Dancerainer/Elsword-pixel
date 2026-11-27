namespace ElswordPixel {
 public sealed partial class DemoScene {
  string CaveNormalRenderClipV28(Enemy e,string previous,ref float clock){
   var content=CurrentNpcContentSet;string dungeon=content==null?null:content.sourceDungeon;
   Library lib=null;if(dungeon==CaveNormalNativeRulesV28.Dungeon&&CaveNormalNativeRulesV28.Supported(e.kind,e.sourceNPC))libraries.TryGetValue(e.kind,out lib);
   float next;string clip=CaveNormalNativeRulesV28.Select(dungeon,e.kind,e.sourceNPC,(int)e.state,e.alerted,e.npcState,previous,e.npcTime,e.clock,clock,
    target=>{AnimationData data;return lib!=null&&lib.clips.TryGetValue(target,out data)?data.duration:float.NaN;},out next);
   clock=next;return clip;
  }
  void ApplyCaveNormalUiV28(){
   var content=CurrentNpcContentSet;string dungeon=content==null?null:content.sourceDungeon;if(dungeon!=CaveNormalNativeRulesV28.Dungeon)return;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var view=enemyViews[i];if(!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy||!CaveNormalNativeRulesV28.Supported(e.kind,e.sourceNPC))continue;
    string name=view.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,System.StringComparison.Ordinal)||slash<=prefix.Length)continue;
    string actualClip=name.Substring(prefix.Length,slash-prefix.Length);float bx,by,dx,dy;var bounds=view.bounds;
    if(!CaveNormalNativeRulesV28.TryUiAnchor(dungeon,e.kind,e.sourceNPC,actualClip,bounds.center.x,(CaveNormalNativeRulesV28.Alias(actualClip)?NativeVisibleTopYV28(view):bounds.max.y),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;
    var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}
