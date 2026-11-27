namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool NasodReactionScopeV28(){var c=CurrentNpcContentSet;return selectedDungeon==5&&c!=null&&c.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal";}
  string NasodReactionRenderClipV28(Enemy e,string previous,ref float clock){
   bool scoped=NasodReactionScopeV28();Library lib=null;if(NasodReactionRenderRulesV28.Supported(scoped,e.sourceNPC))libraries.TryGetValue(e.kind,out lib);
   float next;string clip=NasodReactionRenderRulesV28.Select(scoped,e.sourceNPC,previous,(int)e.state,e.knockdownOnLanding,e.clock,clock,e.deathTime,
    target=>{AnimationData data;return lib!=null&&lib.clips.TryGetValue(target,out data)?data.duration:float.NaN;},out next);clock=next;return clip;
  }
  void ApplyNasodReactionUiV28(){
   bool scoped=NasodReactionScopeV28();if(!scoped)return;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var v=enemyViews[i];if(!v||!v.sprite||!v.enabled||!v.gameObject.activeInHierarchy||!NasodReactionRenderRulesV28.Supported(scoped,e.sourceNPC))continue;
    string n=v.sprite.name,prefix=e.kind+"/";if(!n.StartsWith(prefix,System.StringComparison.Ordinal))continue;int slash=n.LastIndexOf('/');if(slash<=prefix.Length)continue;string clip=n.Substring(prefix.Length,slash-prefix.Length);
    float bx,by,dx,dy;var bounds=v.bounds;if(!NasodReactionRenderRulesV28.TryUiAnchor(scoped,e.sourceNPC,clip,bounds.center.x,bounds.max.y,e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}

