using System;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool First3CombatScopeV28(){var c=CurrentNpcContentSet;return selectedDungeon==4&&c!=null&&c.sourceDungeon==First3CombatNativeRulesV28.Dungeon;}
  string First3CombatRenderClipV28(Enemy e,bool sourceVisible,string previous,ref float clock){
   bool scope=First3CombatScopeV28()&&sourceVisible;Library lib=null;
   if(First3CombatNativeRulesV28.Supported(scope,e.kind,e.sourceNPC))libraries.TryGetValue(e.kind,out lib);
   float next;string result=First3CombatNativeRulesV28.Select(scope,e.kind,e.sourceNPC,e.active,e.dormant,e.passive,e.hp,(int)e.state,e.npcState,previous,e.knockdownOnLanding,e.clock,e.npcTime,clock,e.deathTime,
    alias=>{AnimationData data;if(lib==null||!lib.clips.TryGetValue(alias,out data))return float.NaN;if(data.fps!=First3CombatNativeRulesV28.SourceFps(e.kind,e.sourceNPC,alias)||(First3CombatNativeRulesV28.AirAlias(e.kind,e.sourceNPC,alias)&&(data.frames==null||data.frames.Length!=1))||data.loop!=First3CombatNativeRulesV28.SourceLoop(e.kind,e.sourceNPC,alias)||data.rootDistance!=0||lib.data.logicalScale!=1)throw new InvalidOperationException("First3 combat source metadata/scale mismatch");return data.duration;},out next);
   clock=next;return result;
  }
  void ApplyFirst3CombatUiV28(){
   bool scope=First3CombatScopeV28();if(!scope)return;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var v=enemyViews[i];if(!e.active||e.dormant||e.passive||!v||!v.sprite||!v.enabled||!v.gameObject.activeInHierarchy||!First3CombatNativeRulesV28.Supported(scope,e.kind,e.sourceNPC))continue;
    string name=v.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,StringComparison.Ordinal)||slash<=prefix.Length)continue;
    string clip=name.Substring(prefix.Length,slash-prefix.Length);if(!First3CombatNativeRulesV28.Alias(e.kind,e.sourceNPC,clip))continue;
    var bounds=v.bounds;float bx,by,dx,dy;
    if(!First3CombatNativeRulesV28.TryUiAnchor(scope,e.kind,e.sourceNPC,clip,bounds.center.x,NativeVisibleTopYV28(v),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;
    var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}
