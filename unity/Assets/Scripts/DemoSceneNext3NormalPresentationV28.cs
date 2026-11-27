namespace ElswordPixel {
 public sealed partial class DemoScene {
  string Next3NormalRenderClipV28(Enemy e,bool sourceVisible,string previous,ref float clock){
   var content=CurrentNpcContentSet;string graph=Dungeon.sourceGraph,source=content==null?null:content.sourceDungeon;bool baseline=content==null;
   Library lib=null;if(Next3NormalNativeRulesV28.Scope(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC))libraries.TryGetValue(e.kind,out lib);
   float next;string clip=Next3NormalNativeRulesV28.Select(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC,(int)e.state,e.alerted,e.npcState,previous,e.npcTime,e.clock,clock,
    target=>{AnimationData data;return lib!=null&&lib.clips.TryGetValue(target,out data)?data.duration:float.NaN;},out next);
   float combatClock;clip=Next3CombatNativeRulesV28.Select(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC,e.active,e.dormant,e.passive,sourceVisible,e.aiEnabled,e.hp,(int)e.state,e.npcState,clip,e.npcTime,e.clock,e.recoveryTime,game.EnemyRecoveryDuration(e),next,
    target=>{AnimationData d;return lib!=null&&lib.clips.TryGetValue(target,out d)&&d.frames!=null&&d.hits!=null&&Next3CombatNativeRulesV28.ValidateMetadata(e.kind,target,d.texture,d.fps,d.duration,d.loop,d.rootDistance,d.frames.Length,d.hits.Length);},out combatClock);
   clock=combatClock;return StartedAir26RenderClipV28(e,sourceVisible,clip,ref clock,"next3");
  }
  float Next3NormalEffectiveScaleV28(Enemy e,string actualClip,float previous){
   var content=CurrentNpcContentSet;string source=content==null?null:content.sourceDungeon;bool baseline=content==null;float normal=Next3NormalNativeRulesV28.EffectiveScale(selectedDungeon,Dungeon.sourceGraph,baseline,source,e.kind,e.sourceNPC,actualClip,previous,e.visualScale);float combat=Next3CombatNativeRulesV28.EffectiveScale(selectedDungeon,Dungeon.sourceGraph,baseline,source,e.kind,e.sourceNPC,actualClip,normal,e.visualScale);return StartedAir26EffectiveScaleV28(e,actualClip,combat);
  }
  void ApplyNext3NormalUiV28(){
   var content=CurrentNpcContentSet;string graph=Dungeon.sourceGraph,source=content==null?null:content.sourceDungeon;bool baseline=content==null;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var view=enemyViews[i];if(!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy||!Next3NormalNativeRulesV28.Scope(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC))continue;
    if(StartedAir26ApplyUiV28(e,i,view,"next3"))continue;
    string name=view.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,System.StringComparison.Ordinal)||slash<=prefix.Length)continue;
    string actualClip=name.Substring(prefix.Length,slash-prefix.Length);float bx,by,dx,dy;var bounds=view.bounds;
    bool combat=Next3CombatNativeRulesV28.Alias(e.kind,actualClip);
    bool ui=combat?Next3CombatNativeRulesV28.TryUiAnchor(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC,actualClip,bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy):Next3NormalNativeRulesV28.TryUiAnchor(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC,actualClip,bounds.center.x,(Next3NormalNativeRulesV28.Alias(actualClip)?NativeVisibleTopYV28(view):bounds.max.y),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy);
    if(!ui)continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;
    var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}
