namespace ElswordPixel {
 public static class EntStartNativeDisplayRulesV28 {
  public const string Kind="anger-forest-ent",Npc="NUI_ANGER_FOREST_ENT",State="ANGER_FOREST_ENT_START",Alias="source-start",Animation="Start",Resource="Pixel/feedback-v25/started-initial-v28/anger-forest-ent/start-atlas";
  public static bool OriginalBirth(bool originalIdentity,int dungeon,string graph,bool baseProfile,string contentSource,string stage,int room,int stagedIndex,int sourceIndex,string kind,string npc){return originalIdentity&&dungeon==3&&graph=="Dungeon_Elder_Druid_Forest_Normal"&&baseProfile&&string.IsNullOrEmpty(contentSource)&&stage=="forest-3"&&room==2&&stagedIndex==0&&sourceIndex==9&&kind==Kind&&npc==Npc;}
  public static bool Eligible(bool birth,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,float visualScale,int combatState,string sourceState,string modelClip,string previous){return birth&&kind==Kind&&npc==Npc&&visible&&active&&!dormant&&!passive&&aiEnabled&&hp>0&&visualScale==1&&combatState==0&&sourceState==State&&modelClip==Alias&&previous==modelClip;}
  public static float ValidateMetadata(string resource,float libraryScale,float fps,float duration,bool loop,float rootDistance,int frameCount,int hits,int width,int height){if(resource!=Resource||libraryScale!=1||fps!=24||float.IsNaN(duration)||float.IsInfinity(duration)||System.Math.Abs(duration-5.7f)>.000001f||loop||rootDistance!=0||frameCount!=138||hits!=0||width!=2048||height!=2095)throw new System.InvalidOperationException("Ent START exact atlas/138 source frames/24fps/5.7/scale1 contract mismatch");return duration;}
  public static string Select(bool birth,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,float visualScale,int combatState,string sourceState,string modelClip,string previous,string profileInitial,string sourceAnimation,bool sourceLoop,float sourceSpeed,float sourceSpan,float npcTime,float previousClock,System.Func<float>metadata,out float clock){clock=previousClock;if(!Eligible(birth,kind,npc,visible,active,dormant,passive,aiEnabled,hp,visualScale,combatState,sourceState,modelClip,previous))return previous;if(profileInitial!=State||sourceAnimation!=Animation||sourceLoop||sourceSpeed!=1||float.IsNaN(sourceSpan)||float.IsInfinity(sourceSpan)||System.Math.Abs(sourceSpan-5.7f)>.000001f)throw new System.InvalidOperationException("Ent START exact current source profile mismatch");float span=metadata();if(float.IsNaN(span)||float.IsInfinity(span)||System.Math.Abs(span-5.7f)>.000001f)throw new System.InvalidOperationException("Missing Ent START approved atlas");float t=npcTime*sourceSpeed;if(float.IsNaN(t)||float.IsInfinity(t)||t<0||t>5.7f+.0005f)throw new System.InvalidOperationException("Invalid original Ent START source clock");clock=t;return Alias;}
  public static bool TryUi(bool eligible,string actualClip,float centerX,float actualVisibleTop,float fullWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){bx=by=dx=dy=0;if(!eligible||actualClip!=Alias)return false;if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(actualVisibleTop)||float.IsInfinity(actualVisibleTop)||float.IsNaN(fullWidth)||float.IsInfinity(fullWidth)||fullWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new System.InvalidOperationException("Invalid Ent START current alpha top/HP");bx=centerX-fullWidth*(1-hp/maxHp)/2;by=actualVisibleTop+8;dx=centerX;dy=actualVisibleTop+20;return true;}
 }
 public sealed partial class DemoScene {
  static bool HasStartedNativeFrameAttackPayloadV28(AttackGeometryData g){
   if(g==null)return false;
   return (g.weaponBase!=null&&g.weaponBase.Length>0)||(g.weaponTip!=null&&g.weaponTip.Length>0)||(g.weaponAttackStart!=null&&g.weaponAttackStart.Length>0)||(g.weaponAttackEnd!=null&&g.weaponAttackEnd.Length>0)||(g.rightFoot!=null&&g.rightFoot.Length>0)||(g.leftFoot!=null&&g.leftFoot.Length>0)||(g.rightFootStart!=null&&g.rightFootStart.Length>0)||(g.rightFootEnd!=null&&g.rightFootEnd.Length>0)||(g.leftFootStart!=null&&g.leftFootStart.Length>0)||(g.leftFootEnd!=null&&g.leftFootEnd.Length>0);
  }
  bool entStartGeometryPayloadLoggedV28=false;
  void LogEntStartGeometryPayloadOnceV28(FrameData[] frames){
   if(entStartGeometryPayloadLoggedV28||System.Array.IndexOf(System.Environment.GetCommandLineArgs(),"--ent-start-v28-r12-review")<0)return;
   entStartGeometryPayloadLoggedV28=true;int nullCount=0,emptyInlineCount=0,nonemptyCount=0;
   foreach(var f in frames){if(f.attackGeometry==null)nullCount++;else if(HasStartedNativeFrameAttackPayloadV28(f.attackGeometry))nonemptyCount++;else emptyInlineCount++;}
   UnityEngine.Debug.Log("ENT_START_GEOMETRY_PAYLOAD_V28 null="+nullCount+" emptyInline="+emptyInlineCount+" nonempty="+nonemptyCount+" total="+frames.Length);
  }
  bool EntStartOriginalBirthV28(Enemy e){
   if(e==null||!StartedHurt11AlphaOriginalBirthV28(e)||game.room!=2||catalog==null||catalog.stages==null)return false;
   int i=game.enemies.IndexOf(e),si=Dungeon.start+game.room;if(i!=0||si<0||si>=catalog.stages.Length)return false;var stage=catalog.stages[si];if(stage.spawns==null||stage.spawns.Length==0||stage.id!=game.CurrentRoom.id)return false;var s=stage.spawns[0];
   return s.encounterIndex==0&&e.encounterIndex==0&&s.visualScale==e.visualScale&&EntStartNativeDisplayRulesV28.OriginalBirth(true,selectedDungeon,Dungeon.sourceGraph,CurrentNpcContentSet==null,CurrentNpcContentSet==null?null:CurrentNpcContentSet.sourceDungeon,stage.id,game.room,i,s.sourceSpawnIndex,e.kind,e.sourceNPC);
  }
  bool EntStartEligibleV28(Enemy e,bool sourceVisible,string previous){return e!=null&&EntStartNativeDisplayRulesV28.Eligible(EntStartOriginalBirthV28(e),e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,e.visualScale,(int)e.state,e.npcState,e.clip,previous);}
  float EntStartMetadataV28(Library lib){
   AnimationData a;if(lib==null||!lib.clips.TryGetValue(EntStartNativeDisplayRulesV28.Alias,out a))throw new System.InvalidOperationException("Ent START requires its approved source bank with the profile");
   var page=AtlasTexture(EntStartNativeDisplayRulesV28.Resource);float span=EntStartNativeDisplayRulesV28.ValidateMetadata(a.texture,lib.data.logicalScale,a.fps,a.duration,a.loop,a.rootDistance,a.frames==null?0:a.frames.Length,a.hits==null?0:a.hits.Length,page?page.width:0,page?page.height:0);
   if(!page||!page.isReadable||page.mipmapCount!=1||page.filterMode!=UnityEngine.FilterMode.Point||page.format!=UnityEngine.TextureFormat.RGBA32)throw new System.InvalidOperationException("Ent START original RGBA32/Point/readable/noMip atlas required");
   LogEntStartGeometryPayloadOnceV28(a.frames);
   foreach(var f in a.frames)if(!f.hasAnchor||f.rootX!=0||f.texture!=a.texture||HasStartedNativeFrameAttackPayloadV28(f.attackGeometry))throw new System.InvalidOperationException("Ent START independent sourceB anchors/no root or hit payload required");return span;
  }
  string StartedAttack28AlphaRenderClipV28(Enemy enemy,bool visible,string previous,NpcDefinition definition,NpcStateDefinition state,ref float clock){
   if(enemy==null)return previous;bool birth=StartedGround24AlphaBirthV28(enemy),profile=definition!=null&&definition.id==enemy.sourceNPC&&definition.kind==enemy.kind&&state!=null&&state.id==enemy.npcState;Library lib=null;if(birth)libraries.TryGetValue(enemy.kind,out lib);float next;
   string selected=Attack28NativeDisplayRulesV28.SelectAlpha(birth,profile,enemy.kind,enemy.sourceNPC,visible,enemy.active,enemy.dormant,enemy.passive,enemy.aiEnabled,enemy.hp,(int)enemy.state,enemy.npcState,enemy.clip,previous,state==null?null:state.sourceAnimation,state!=null&&state.loop,state==null?float.NaN:state.sourceAnimSpeed,state==null?float.NaN:state.duration,enemy.npcTime,clock,spec=>StartedAttack28MetadataReadyV28(enemy,lib,spec),out next);clock=next;return selected;
  }
  string Alpha4NormalRenderClipV28(Enemy e,bool sourceVisible,string previous,ref float clock){
   var content=CurrentNpcContentSet;string graph=Dungeon.sourceGraph,source=content==null?null:content.sourceDungeon;bool baseline=content==null;
   Library lib=null;NpcStateDefinition state=null;NpcDefinition definition=null;
   if(Alpha4NormalNativeRulesV28.Scope(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC)){
    libraries.TryGetValue(e.kind,out lib);
    definition=catalog.npcRules==null?null:System.Array.Find(catalog.npcRules,p=>p.id==e.sourceNPC&&p.kind==e.kind);
    if(definition!=null&&definition.states!=null)state=System.Array.Find(definition.states,s=>s.id==e.npcState);
   }
   if(EntStartEligibleV28(e,sourceVisible,previous)){
    float startClock;string startClip=EntStartNativeDisplayRulesV28.Select(true,e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,e.visualScale,(int)e.state,e.npcState,e.clip,previous,definition==null?null:definition.initialState,state==null?null:state.sourceAnimation,state!=null&&state.loop,state==null?float.NaN:state.sourceAnimSpeed,state==null?float.NaN:state.duration,e.npcTime,clock,()=>EntStartMetadataV28(lib),out startClock);clock=startClock;return startClip;
   }
   float next;string clip=Alpha4NormalNativeRulesV28.Select(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC,e.active&&e.aiEnabled&&e.hp>0,e.dormant,sourceVisible,(int)e.state,e.npcState,previous,
    state==null?null:state.sourceAnimation,state!=null&&state.loop,state==null?float.NaN:state.sourceAnimSpeed,state==null?float.NaN:state.duration,e.npcTime,clock,
    target=>{AnimationData data;if(lib==null||!lib.clips.TryGetValue(target,out data))return float.NaN;return Alpha4NormalNativeRulesV28.ValidateClip(e.sourceNPC,target,lib.data.logicalScale,data.fps,data.loop,data.rootDistance,data.frames==null?0:data.frames.Length,data.hits==null?0:data.hits.Length,data.duration);},out next);
   bool hurtScope=StartedHurt11NativeRulesV28.AlphaScope(selectedDungeon,graph,baseline,source,StartedHurt11AlphaOriginalBirthV28(e),StartedHurt11AlphaReactionExactV28(definition,e),e.kind,e.sourceNPC);float hurtClock;
   clip=StartedHurt11NativeRulesV28.Select(hurtScope,e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,clip,e.clock,next,()=>StartedHurt11MetadataReadyV28(e,lib),out hurtClock);clock=hurtClock;
   bool groundScope=StartedGround24AlphaBirthV28(e)&&StartedGround24AlphaSourceExactV28(definition,e);float groundClock;
   clip=StartedGround24NativeRulesV28.Select(groundScope,e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,clip,e.clock,e.deathTime,hurtClock,()=>StartedGround24MetadataReadyV28(e,lib),out groundClock);clock=groundClock;clip=StartedAttack28AlphaRenderClipV28(e,sourceVisible,clip,definition,state,ref clock);return StartedAir26RenderClipV28(e,sourceVisible,clip,ref clock,"alpha3");
  }
  void ApplyAlpha4NormalUiV28(){
   var content=CurrentNpcContentSet;string graph=Dungeon.sourceGraph,source=content==null?null:content.sourceDungeon;bool baseline=content==null;
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var view=enemyViews[i];if(!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)continue;
    if(EntStartEligibleV28(e,true,e.clip)&&view.sprite.name.StartsWith(e.kind+"/source-start/",System.StringComparison.Ordinal)){
     Library startLib=null;libraries.TryGetValue(e.kind,out startLib);EntStartMetadataV28(startLib);float sx,sy,tx,ty;
     if(EntStartNativeDisplayRulesV28.TryUi(true,EntStartNativeDisplayRulesV28.Alias,view.bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out sx,out sy,out tx,out ty)){var bar=enemyBars[i].transform.position;bar.x=sx;bar.y=sy;enemyBars[i].transform.position=bar;var debuff=enemyDebuffViews[i].transform.position;debuff.x=tx;debuff.y=ty;enemyDebuffViews[i].transform.position=debuff;}continue;
    }
    if(StartedAir26ApplyUiV28(e,i,view,"alpha3"))continue;
    var groundDefinition=catalog.npcRules==null?null:System.Array.Find(catalog.npcRules,p=>p.id==e.sourceNPC&&p.kind==e.kind);
    if(StartedGround24ApplyUiV28(e,i,view,StartedGround24AlphaBirthV28(e)&&StartedGround24AlphaSourceExactV28(groundDefinition,e)))continue;
    var attackState=groundDefinition==null||groundDefinition.states==null?null:System.Array.Find(groundDefinition.states,s=>s.id==e.npcState);float attackClock=e.clock;string attackClip=StartedAttack28AlphaRenderClipV28(e,true,e.clip,groundDefinition,attackState,ref attackClock);if(StartedAttack28ApplyUiV28(e,i,view,attackClip))continue;
    bool eligible=Alpha4NormalNativeRulesV28.ActiveTuple(selectedDungeon,graph,baseline,source,e.kind,e.sourceNPC,e.active&&e.aiEnabled&&e.hp>0,e.dormant,true,(int)e.state,e.npcState,e.clip);
    var definition=catalog.npcRules==null?null:System.Array.Find(catalog.npcRules,p=>p.id==e.sourceNPC&&p.kind==e.kind);Library hurtLib=null;libraries.TryGetValue(e.kind,out hurtLib);
    bool hurtScope=StartedHurt11NativeRulesV28.AlphaScope(selectedDungeon,graph,baseline,source,StartedHurt11AlphaOriginalBirthV28(e),StartedHurt11AlphaReactionExactV28(definition,e),e.kind,e.sourceNPC);
    bool hurt=StartedHurt11NativeRulesV28.Eligible(hurtScope,e.kind,e.sourceNPC,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,e.clip)&&StartedHurt11MetadataReadyV28(e,hurtLib);if(!eligible&&!hurt)continue;
    string name=view.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,System.StringComparison.Ordinal)||slash<=prefix.Length)continue;
    string actualClip=name.Substring(prefix.Length,slash-prefix.Length);float bx,by,dx,dy;var bounds=view.bounds;
    bool ui=hurt?StartedHurt11NativeRulesV28.TryUiAnchor(hurt,actualClip,bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy):Alpha4NormalNativeRulesV28.TryUiAnchor(eligible,actualClip,bounds.center.x,(Alpha4NormalNativeRulesV28.Alias(actualClip)?NativeVisibleTopYV28(view):bounds.max.y),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy);if(!ui)continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;
    var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}
