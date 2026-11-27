namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool NasodNormalScopeV28(){var c=CurrentNpcContentSet;return selectedDungeon==5&&c!=null&&c.sourceDungeon==NasodNormalNativeRulesV28.Dungeon;}
  bool NasodAttack6OriginalBirthV28(Enemy e){
   if(e==null||game==null||catalog==null||catalog.stages==null||catalog.dungeons==null||selectedDungeon!=5||game.room!=2||game.CurrentRoom.id!="waterway-3"||!NasodNormalScopeV28()||e.kind!="benders-nasod"||e.sourceNPC!="NUI_BENDERS_NASOD")return false;
   var route=catalog.dungeons[5];int si=route.start+game.room,i=game.enemies.IndexOf(e);if(route.id!="underground-waterway"||si<0||si>=catalog.stages.Length||i!=2)return false;
   var stage=catalog.stages[si];if(stage==null||stage.id!="waterway-3"||stage.spawns==null||stage.spawns.Length<=2)return false;var sp=stage.spawns[2];
   return sp.kind==e.kind&&sp.sourceNPC==e.sourceNPC&&sp.sourceSpawnIndex==2&&sp.encounterIndex==0&&e.encounterIndex==0&&sp.sourceKeyCode==e.sourceKeyCode&&sp.x==e.spawnX&&sp.visualScale==e.visualScale&&sp.visualScale==1&&sp.aiEnabled&&!sp.passive&&!sp.dormant;
  }
  string NasodAttack6RenderClipV28(Enemy e,string previous,ref float clock){
   if(!NasodAttack6OriginalBirthV28(e))return previous;var content=CurrentNpcContentSet;var definition=content.npcRules==null?null:System.Array.Find(content.npcRules,p=>p!=null&&p.id==e.sourceNPC&&p.kind==e.kind);var state=definition==null||definition.states==null?null:System.Array.Find(definition.states,s=>s!=null&&s.id==e.npcState);
   int i=game.enemies.IndexOf(e);var view=i>=0&&i<enemyViews.Count?enemyViews[i]:null;bool visible=view&&view.enabled&&view.gameObject.activeInHierarchy;Library lib=null;libraries.TryGetValue(e.kind,out lib);float next;
   string selected=Attack28NativeDisplayRulesV28.SelectAlpha(true,definition!=null&&state!=null,e.kind,e.sourceNPC,visible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.npcState,e.clip,previous,state==null?null:state.sourceAnimation,state!=null&&state.loop,state==null?float.NaN:state.sourceAnimSpeed,state==null?float.NaN:state.duration,e.npcTime,clock,spec=>StartedAttack28MetadataReadyV28(e,lib,spec),out next);clock=next;return selected;
  }
  bool NasodAttack6ApplyUiV28(Enemy e,int index,UnityEngine.SpriteRenderer view){
   if(!NasodAttack6OriginalBirthV28(e)||!view||!view.sprite)return false;string name=view.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,System.StringComparison.Ordinal)||slash<=prefix.Length)return false;return StartedAttack28ApplyUiV28(e,index,view,name.Substring(prefix.Length,slash-prefix.Length));
  }
  string NasodNormalRenderClipV28(Enemy e,string previous,ref float clock){
   previous=NasodAttack6RenderClipV28(e,previous,ref clock);bool scoped=NasodNormalScopeV28();Library lib=null;if(NasodNormalNativeRulesV28.Supported(scoped,e.kind,e.sourceNPC))libraries.TryGetValue(e.kind,out lib);
   float next;string clip=NasodNormalNativeRulesV28.Select(scoped,e.kind,e.sourceNPC,(int)e.state,e.npcState,previous,e.npcTime,clock,
    target=>{AnimationData data;return lib!=null&&lib.clips.TryGetValue(target,out data)?data.duration:float.NaN;},out next);clock=next;return clip;
  }
  void ApplyNasodNormalUiV28(){
   bool scoped=NasodNormalScopeV28();if(!scoped)return;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var v=enemyViews[i];if(!v||!v.sprite||!v.enabled||!v.gameObject.activeInHierarchy||!NasodNormalNativeRulesV28.Supported(scoped,e.kind,e.sourceNPC))continue;
    if(NasodAttack6ApplyUiV28(e,i,v))continue;string name=v.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,System.StringComparison.Ordinal)||slash<=prefix.Length)continue;
    string clip=name.Substring(prefix.Length,slash-prefix.Length);float bx,by,dx,dy;var bounds=v.bounds;
    if(!NasodNormalNativeRulesV28.TryUiAnchor(scoped,e.kind,e.sourceNPC,clip,bounds.center.x,(NasodNormalNativeRulesV28.Alias(clip)?NativeVisibleTopYV28(v):bounds.max.y),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;
    var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}
