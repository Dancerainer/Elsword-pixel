using System;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool StartedRemaining3HurtMetadataReadyV28(Enemy enemy,Library lib,Remaining3HurtNativeRulesV28.Spec spec){
   AnimationData clip;UnityEngine.Sprite[] sprites;if(enemy==null||spec==null||spec.kind!=enemy.kind||spec.npc!=enemy.sourceNPC||lib==null||!lib.clips.TryGetValue(Remaining3HurtNativeRulesV28.Alias,out clip)||clip==null||clip.frames==null||clip.hits==null||!lib.frames.TryGetValue(Remaining3HurtNativeRulesV28.Alias,out sprites)||sprites==null||sprites.Length!=spec.samples)return false;
   if(!Remaining3HurtNativeRulesV28.ValidateClip(spec,clip.id,clip.command==null?null:clip.command.evidence,lib.data.logicalScale,clip.texture,clip.fps,clip.duration,clip.loop,clip.rootDistance,clip.frames.Length,clip.hits.Length))return false;
   for(int j=0;j<clip.frames.Length;j++){
    var frame=clip.frames[j];var sprite=sprites[j];if(frame==null||!frame.hasAnchor||frame.rootX!=0||HasStartedNativeFrameAttackPayloadV28(frame.attackGeometry)||frame.x<0||frame.y<0||frame.w<=0||frame.h<=0||!Remaining3HurtNativeRulesV28.Finite(frame.anchorX)||!Remaining3HurtNativeRulesV28.Finite(frame.anchorY)||!Remaining3HurtNativeRulesV28.Resource(spec,frame.texture)||!sprite)return false;
    var page=sprite.texture;if(!page||page.width>4096||page.height>4096||!page.isReadable||page.mipmapCount!=1||page.filterMode!=UnityEngine.FilterMode.Point||page.format!=UnityEngine.TextureFormat.RGBA32||page!=AtlasTexture(frame.texture)||frame.x>page.width-frame.w||frame.y>page.height-frame.h||sprite.name!=enemy.kind+"/"+Remaining3HurtNativeRulesV28.Alias+"/"+j||sprite.rect.x!=frame.x||sprite.rect.y!=page.height-frame.y-frame.h||sprite.rect.width!=frame.w||sprite.rect.height!=frame.h||sprite.pixelsPerUnit!=1)return false;
   }
   return true;
  }
  bool StartedRemaining3HurtEligibleV28(Enemy enemy,int index,bool visible){
   if(enemy==null)return false;bool scope=Remaining3HurtNativeRulesV28.Scope(Generic7OriginalBirthV28(enemy,index),Generic7ProfilePresentV28(enemy),enemy.kind,enemy.sourceNPC);
   return Remaining3HurtNativeRulesV28.Eligible(scope,enemy.kind,enemy.sourceNPC,visible,enemy.active,enemy.dormant,enemy.passive,enemy.aiEnabled,enemy.hp,(int)enemy.state,enemy.npcState,enemy.clip,enemy.clip);
  }
  string StartedRemaining3HurtRenderClipV28(Enemy enemy,int index,bool visible,string previous,ref float clock){
   if(enemy==null)return previous;bool birth=Generic7OriginalBirthV28(enemy,index);bool scope=Remaining3HurtNativeRulesV28.Scope(birth,Generic7ProfilePresentV28(enemy),enemy.kind,enemy.sourceNPC);Library lib=null;if(birth)libraries.TryGetValue(enemy.kind,out lib);float next;
   string selected=Remaining3HurtNativeRulesV28.Select(scope,enemy.kind,enemy.sourceNPC,visible,enemy.active,enemy.dormant,enemy.passive,enemy.aiEnabled,enemy.hp,(int)enemy.state,enemy.npcState,enemy.clip,previous,enemy.clock,clock,spec=>StartedRemaining3HurtMetadataReadyV28(enemy,lib,spec),out next);clock=next;return selected;
  }
  bool StartedRemaining3HurtApplyUiV28(Enemy enemy,int index,UnityEngine.SpriteRenderer view){
   if(!StartedRemaining3HurtEligibleV28(enemy,index,true)||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index]||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)return false;
   var spec=Remaining3HurtNativeRulesV28.Find(enemy.kind,enemy.sourceNPC);if(!view.sprite.name.StartsWith(enemy.kind+"/"+Remaining3HurtNativeRulesV28.Alias+"/",StringComparison.Ordinal))return false;Library lib=null;libraries.TryGetValue(enemy.kind,out lib);UnityEngine.Sprite[] frames;
   if(!StartedRemaining3HurtMetadataReadyV28(enemy,lib,spec)||!lib.frames.TryGetValue(Remaining3HurtNativeRulesV28.Alias,out frames)||Array.IndexOf(frames,view.sprite)<0)return false;float bx,by,dx,dy;
   if(!Remaining3HurtNativeRulesV28.TryUi(true,Remaining3HurtNativeRulesV28.Alias,view.bounds.center.x,NativeVisibleTopYV28(view),enemy.maxHp>400?64:30,enemy.hp,enemy.maxHp,out bx,out by,out dx,out dy))return false;
   var bar=enemyBars[index].transform.position;bar.x=bx;bar.y=by;enemyBars[index].transform.position=bar;var debuff=enemyDebuffViews[index].transform.position;debuff.x=dx;debuff.y=dy;enemyDebuffViews[index].transform.position=debuff;return true;
  }
  static bool StartedAttack28FiniteArrayV28(float[] values){if(values==null)return true;foreach(float value in values)if(!Attack28NativeDisplayRulesV28.Finite(value))return false;return true;}
  static bool StartedAttack28PayloadFiniteV28(AttackGeometryData data){
   // Optional attack data is preserved, including nonempty/zero arrays. It never creates or changes model hits.
   return data==null||StartedAttack28FiniteArrayV28(data.weaponBase)&&StartedAttack28FiniteArrayV28(data.weaponTip)&&StartedAttack28FiniteArrayV28(data.weaponAttackStart)&&StartedAttack28FiniteArrayV28(data.weaponAttackEnd)&&StartedAttack28FiniteArrayV28(data.rightFoot)&&StartedAttack28FiniteArrayV28(data.leftFoot)&&StartedAttack28FiniteArrayV28(data.rightFootStart)&&StartedAttack28FiniteArrayV28(data.rightFootEnd)&&StartedAttack28FiniteArrayV28(data.leftFootStart)&&StartedAttack28FiniteArrayV28(data.leftFootEnd);
  }
  bool StartedAttack28MetadataReadyV28(Enemy enemy,Library lib,Attack28NativeDisplayRulesV28.Spec spec){
   AnimationData clip;UnityEngine.Sprite[] sprites;if(enemy==null||spec==null||lib==null||spec.kind!=enemy.kind||spec.npc!=enemy.sourceNPC||!lib.clips.TryGetValue(spec.alias,out clip)||clip==null||clip.frames==null||clip.hits==null||!lib.frames.TryGetValue(spec.alias,out sprites)||sprites==null||sprites.Length!=spec.samples)return false;
   if(!Attack28NativeDisplayRulesV28.ValidateClip(spec,clip.id,clip.command==null?null:clip.command.evidence,lib.data.logicalScale,clip.texture,clip.fps,clip.duration,clip.loop,clip.rootDistance,clip.frames.Length,clip.hits.Length,true))return false;
   for(int j=0;j<clip.frames.Length;j++){
    var frame=clip.frames[j];var sprite=sprites[j];if(frame==null||!frame.hasAnchor||frame.rootX!=0||frame.x<0||frame.y<0||frame.w<=0||frame.h<=0||!Attack28NativeDisplayRulesV28.Finite(frame.anchorX)||!Attack28NativeDisplayRulesV28.Finite(frame.anchorY)||!Attack28NativeDisplayRulesV28.Resource(spec,frame.texture)||!StartedAttack28PayloadFiniteV28(frame.attackGeometry)||!sprite)return false;
    var page=sprite.texture;if(!page||page.width>4096||page.height>4096||frame.x>page.width-frame.w||frame.y>page.height-frame.h||!page.isReadable||page.mipmapCount!=1||page.filterMode!=UnityEngine.FilterMode.Point||page.format!=UnityEngine.TextureFormat.RGBA32||page!=AtlasTexture(frame.texture)||sprite.name!=enemy.kind+"/"+spec.alias+"/"+j||sprite.rect.x!=frame.x||sprite.rect.y!=page.height-frame.y-frame.h||sprite.rect.width!=frame.w||sprite.rect.height!=frame.h||sprite.pixelsPerUnit!=1)return false;
   }
   return true;
  }
  string StartedAttack28GenericRenderClipV28(Enemy enemy,int index,bool visible,string previous,ref float clock){
   if(enemy==null)return previous;bool birth=Generic7OriginalBirthV28(enemy,index),profile=Generic7ProfilePresentV28(enemy);Library lib=null;if(birth)libraries.TryGetValue(enemy.kind,out lib);float next;
   string selected=Attack28NativeDisplayRulesV28.SelectGeneric(birth,profile,enemy.kind,enemy.sourceNPC,visible,enemy.active,enemy.dormant,enemy.passive,enemy.aiEnabled,enemy.hp,(int)enemy.state,enemy.npcState,enemy.clip,previous,enemy.telegraph,enemy.struck,clock,spec=>StartedAttack28MetadataReadyV28(enemy,lib,spec),out next);clock=next;return selected;
  }
  bool StartedAttack28ApplyUiV28(Enemy enemy,int index,UnityEngine.SpriteRenderer view,string selected){
   var spec=Attack28NativeDisplayRulesV28.Alias(enemy.kind,enemy.sourceNPC,selected);if(spec==null||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index]||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy||!view.sprite.name.StartsWith(enemy.kind+"/"+selected+"/",StringComparison.Ordinal))return false;
   Library lib=null;libraries.TryGetValue(enemy.kind,out lib);UnityEngine.Sprite[] frames;if(!StartedAttack28MetadataReadyV28(enemy,lib,spec)||!lib.frames.TryGetValue(selected,out frames)||Array.IndexOf(frames,view.sprite)<0)return false;float bx,by,dx,dy;if(!Attack28NativeDisplayRulesV28.TryUi(spec,selected,view.bounds.center.x,NativeVisibleTopYV28(view),enemy.maxHp>400?64:30,enemy.hp,enemy.maxHp,out bx,out by,out dx,out dy))return false;
   var bar=enemyBars[index].transform.position;bar.x=bx;bar.y=by;enemyBars[index].transform.position=bar;var debuff=enemyDebuffViews[index].transform.position;debuff.x=dx;debuff.y=dy;enemyDebuffViews[index].transform.position=debuff;return true;
  }
  // Air is a separate host-physics display branch; existing normal/Hurt/Ground/START rules remain unchanged.
  bool StartedAir26OriginalScopeV28(Enemy e,StartedAir26NativeRulesV28.Entry spec){
   if(e==null||spec==null||catalog==null||catalog.dungeons==null||catalog.stages==null||selectedDungeon<0||selectedDungeon>=catalog.dungeons.Length||game==null)return false;
   int i=game.enemies.IndexOf(e);var route=catalog.dungeons[selectedDungeon];int si=route.start+game.room;if(i<0||si<0||si>=catalog.stages.Length)return false;var st=catalog.stages[si];if(st.id!=game.CurrentRoom.id||st.spawns==null||i>=st.spawns.Length)return false;var sp=st.spawns[i];
   if(sp.kind!=e.kind||sp.sourceNPC!=e.sourceNPC||sp.encounterIndex!=e.encounterIndex||sp.sourceKeyCode!=e.sourceKeyCode||sp.x!=e.spawnX||sp.visualScale!=e.visualScale||!sp.aiEnabled||sp.passive||sp.dormant)return false;
   var content=CurrentNpcContentSet;bool baseline=content==null;string source=baseline?null:content.sourceDungeon;
   if(!StartedAir26NativeRulesV28.OriginalBirth(spec,selectedDungeon,route.sourceGraph,baseline,source,route.id,game.room,st.id,st.sourceWorldId,i,sp.sourceSpawnIndex))return false;
   var profiles=baseline?catalog.npcRules:content.npcRules;var definition=profiles==null?null:Array.Find(profiles,p=>p!=null&&p.id==e.sourceNPC);
   bool exact=definition!=null&&definition.id==spec.npc&&definition.kind==spec.kind,air=definition!=null&&definition.reactions!=null&&Array.Exists(definition.reactions,r=>r!=null&&(int)r.combatState==1);
   return StartedAir26NativeRulesV28.ProfileContext(spec,definition!=null,exact,air,e.npcState)&&(spec.group!="generic7"||Generic7OriginalBirthV28(e,i));
  }
  bool StartedAir26MetadataReadyV28(Enemy e,Library lib,StartedAir26NativeRulesV28.Entry spec){
   AnimationData c;UnityEngine.Sprite[] sprites;if(spec==null||lib==null||!lib.clips.TryGetValue(spec.alias,out c)||c==null||c.frames==null||c.hits==null||!lib.frames.TryGetValue(spec.alias,out sprites)||sprites==null||c.frames.Length!=spec.samples||sprites.Length!=spec.samples||c.frames.Length==0||c.frames[0]==null||c.texture!=c.frames[0].texture)return false;
   bool coherent=true;for(int j=0;j<c.frames.Length;j++){var f=c.frames[j];var sp=sprites[j];if(f==null||!f.hasAnchor||f.rootX!=0||HasStartedNativeFrameAttackPayloadV28(f.attackGeometry)||!StartedAir26NativeRulesV28.Resource(spec,f.texture)||f.x<0||f.y<0||f.w<=0||f.h<=0||!StartedAir26NativeRulesV28.Finite(f.anchorX)||!StartedAir26NativeRulesV28.Finite(f.anchorY)||!sp){coherent=false;break;}var page=sp.texture;
    if(!page||page.width<=0||page.height<=0||page.width>4096||page.height>4096||f.w>page.width||f.h>page.height||f.x>page.width-f.w||f.y>page.height-f.h||!page.isReadable||page.mipmapCount!=1||page.filterMode!=UnityEngine.FilterMode.Point||page.format!=UnityEngine.TextureFormat.RGBA32||page!=AtlasTexture(f.texture)||sp.pixelsPerUnit!=1||sp.name!=e.kind+"/"+spec.alias+"/"+j||sp.rect.x!=f.x||sp.rect.y!=page.height-f.y-f.h||sp.rect.width!=f.w||sp.rect.height!=f.h){coherent=false;break;}}
   return StartedAir26NativeRulesV28.ValidateClip(spec,c.id,lib.data.logicalScale,c.texture,c.fps,c.duration,c.loop,c.rootDistance,c.frames.Length,c.hits.Length,coherent,c.command==null?null:c.command.evidence);
  }
  string StartedAir26RenderClipV28(Enemy e,bool visible,string previous,ref float clock,string group){
   var spec=StartedAir26NativeRulesV28.Find(e.kind,e.sourceNPC,e.clip);if(spec==null||spec.group!=group)return previous;bool scope=StartedAir26OriginalScopeV28(e,spec);Library lib=null;if(scope)libraries.TryGetValue(e.kind,out lib);float next;
   string result=StartedAir26NativeRulesV28.Select(spec,scope,visible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.knockdownOnLanding,e.clip,previous,e.clock,clock,()=>StartedAir26MetadataReadyV28(e,lib,spec),out next);clock=next;return result;
  }
  float StartedAir26EffectiveScaleV28(Enemy e,string actualClip,float previous){
   if(e==null||e.kind!="thief-fat-boss")return previous;var spec=StartedAir26NativeRulesV28.Find(e.kind,e.sourceNPC,e.clip);if(spec==null||actualClip!=spec.alias)return previous;bool scope=StartedAir26OriginalScopeV28(e,spec);Library lib=null;if(scope)libraries.TryGetValue(e.kind,out lib);
   bool qualified=StartedAir26NativeRulesV28.Eligible(spec,scope,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.knockdownOnLanding,e.clip,e.clip)&&StartedAir26NativeRulesV28.Finite(e.clock)&&e.clock>=0&&StartedAir26MetadataReadyV28(e,lib,spec);return StartedAir26NativeRulesV28.EffectiveScale(spec,qualified,actualClip,previous,e.visualScale);
  }
  bool StartedAir26ApplyUiV28(Enemy e,int index,UnityEngine.SpriteRenderer view,string group){
   var spec=StartedAir26NativeRulesV28.Find(e.kind,e.sourceNPC,e.clip);if(spec==null||spec.group!=group||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index])return false;
   bool scope=StartedAir26OriginalScopeV28(e,spec);if(!StartedAir26NativeRulesV28.Eligible(spec,scope,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.knockdownOnLanding,e.clip,e.clip)||!StartedAir26NativeRulesV28.Finite(e.clock)||e.clock<0)return false;
   Library lib=null;libraries.TryGetValue(e.kind,out lib);if(!StartedAir26MetadataReadyV28(e,lib,spec))return false;string prefix=e.kind+"/"+spec.alias+"/";int frame;if(!view.sprite.name.StartsWith(prefix,StringComparison.Ordinal)||!int.TryParse(view.sprite.name.Substring(prefix.Length),out frame)||frame<0||frame>=spec.samples||view.sprite!=lib.frames[spec.alias][frame])return false;
   float bx,by,dx,dy;if(!StartedAir26NativeRulesV28.TryUi(spec,true,spec.alias,view.bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))return false;
   var b=enemyBars[index].transform.position;b.x=bx;b.y=by;enemyBars[index].transform.position=b;var d=enemyDebuffViews[index].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[index].transform.position=d;return true;
  }
  bool StartedGround24MetadataReadyV28(Enemy e,Library lib){
   var spec=StartedGround24NativeRulesV28.Find(e.kind,e.sourceNPC,(int)e.state);AnimationData c;UnityEngine.Sprite[] sprites;
   if(spec==null||lib==null||!lib.clips.TryGetValue(spec.alias,out c)||c.frames==null||c.hits==null||!lib.frames.TryGetValue(spec.alias,out sprites)||sprites==null||sprites.Length!=spec.samples)return false;
   bool coherent=true;for(int j=0;j<c.frames.Length;j++){var f=c.frames[j];if(f==null||!f.hasAnchor||f.rootX!=0||HasStartedNativeFrameAttackPayloadV28(f.attackGeometry)||!StartedGround24NativeRulesV28.Resource(spec,f.texture)||f.w<=0||f.h<=0||!StartedGround24NativeRulesV28.Finite(f.anchorX)||!StartedGround24NativeRulesV28.Finite(f.anchorY)||j>=sprites.Length||!sprites[j]){coherent=false;break;}var page=sprites[j].texture;if(!page||page.width>4096||page.height>4096||!page.isReadable||page.mipmapCount!=1||page.filterMode!=UnityEngine.FilterMode.Point||page.format!=UnityEngine.TextureFormat.RGBA32||page!=AtlasTexture(f.texture)||sprites[j].name!=e.kind+"/"+spec.alias+"/"+j||sprites[j].rect.width!=f.w||sprites[j].rect.height!=f.h){coherent=false;break;}}
   return StartedGround24NativeRulesV28.ValidateClip(spec,c.id,lib.data.logicalScale,c.texture,c.fps,c.duration,c.loop,c.rootDistance,c.frames.Length,c.hits.Length,coherent);
  }
  bool StartedGround24AlphaBirthV28(Enemy e){
   if(e==null||!StartedHurt11AlphaOriginalBirthV28(e)||catalog==null||catalog.stages==null)return false;int i=game.enemies.IndexOf(e),si=Dungeon.start+game.room;if(i<0||si<0||si>=catalog.stages.Length)return false;var st=catalog.stages[si];if(st.spawns==null||i>=st.spawns.Length)return false;var sp=st.spawns[i];
   return sp.visualScale==e.visualScale&&StartedGround24NativeRulesV28.AlphaBirth(true,selectedDungeon,Dungeon.sourceGraph,CurrentNpcContentSet==null,CurrentNpcContentSet==null?null:CurrentNpcContentSet.sourceDungeon,game.room,st.id,st.sourceWorldId,i,sp.sourceSpawnIndex,e.kind,e.sourceNPC);
  }
  bool StartedGround24AlphaSourceExactV28(NpcDefinition definition,Enemy e){var spec=StartedGround24NativeRulesV28.Find(e.kind,e.sourceNPC,(int)e.state);if(spec==null||!spec.alpha||definition==null||definition.id!=e.sourceNPC||definition.kind!=e.kind||definition.reactions==null)return false;return Array.Exists(definition.reactions,r=>r!=null&&StartedGround24NativeRulesV28.AlphaReactionExact(e.kind,e.sourceNPC,(int)e.state,(int)r.combatState,r.sourceState,r.sourceAnimation,r.sourceAnimSpeed,r.duration));}
  bool StartedGround24ApplyUiV28(Enemy e,int index,UnityEngine.SpriteRenderer view,bool scope){
   if(index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!StartedGround24NativeRulesV28.Eligible(scope,e.kind,e.sourceNPC,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,e.clip)||(int)e.state==5)return false;
   var spec=StartedGround24NativeRulesV28.Find(e.kind,e.sourceNPC,(int)e.state);if(!view.sprite.name.StartsWith(e.kind+"/"+spec.alias+"/",StringComparison.Ordinal))return false;Library lib=null;libraries.TryGetValue(e.kind,out lib);if(!StartedGround24MetadataReadyV28(e,lib))return false;float bx,by,dx,dy;if(!StartedGround24NativeRulesV28.TryUi(true,(int)e.state,spec.alias,view.bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))return false;
   var b=enemyBars[index].transform.position;b.x=bx;b.y=by;enemyBars[index].transform.position=b;var d=enemyDebuffViews[index].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[index].transform.position=d;return true;
  }
  bool StartedHurt11MetadataReadyV28(Enemy e,Library lib){
   AnimationData c;UnityEngine.Sprite[] sprites;var s=StartedHurt11NativeRulesV28.Find(e.kind,e.sourceNPC);
   if(s==null||lib==null||!lib.clips.TryGetValue("source-hurt",out c)||c.frames==null||c.hits==null||!lib.frames.TryGetValue("source-hurt",out sprites)||sprites==null||sprites.Length!=s.samples)return false;
   bool frames=true;foreach(var f in c.frames)if(f==null||!f.hasAnchor||f.rootX!=0||f.texture!=s.resource||f.w<=0||f.h<=0||!StartedHurt11NativeRulesV28.Finite(f.anchorX)||!StartedHurt11NativeRulesV28.Finite(f.anchorY)){frames=false;break;}
   return StartedHurt11NativeRulesV28.ValidateClip(e.kind,e.sourceNPC,c.id,lib.data.logicalScale,c.texture,c.fps,c.duration,c.loop,c.rootDistance,c.frames.Length,c.hits.Length,frames);
  }
  bool StartedHurt11AlphaOriginalBirthV28(Enemy e){
   if(e==null||selectedDungeon!=3||CurrentNpcContentSet!=null||catalog==null||catalog.stages==null||catalog.dungeons==null)return false;
   int i=game.enemies.IndexOf(e),si=Dungeon.start+game.room;if(i<0||si<0||si>=catalog.stages.Length)return false;var stage=catalog.stages[si];if(stage.id!=game.CurrentRoom.id||stage.spawns==null||i>=stage.spawns.Length)return false;var s=stage.spawns[i];
   return s.kind==e.kind&&s.sourceNPC==e.sourceNPC&&s.sourceKeyCode==e.sourceKeyCode&&s.encounterIndex==e.encounterIndex&&s.x==e.spawnX&&s.aiEnabled&&!s.passive&&!s.dormant;
  }
  bool StartedHurt11AlphaReactionExactV28(NpcDefinition definition,Enemy e){var s=StartedHurt11NativeRulesV28.Find(e.kind,e.sourceNPC);if(s==null||!s.alpha||definition==null||definition.reactions==null)return false;return Array.Exists(definition.reactions,r=>r.combatState==CombatState.Hurt&&r.sourceState==s.sourceState&&r.sourceAnimation==s.animation&&r.sourceAnimSpeed==1&&Math.Abs(r.duration-s.duration)<.000001f);}
  bool Generic7OriginalBirthV28(Enemy e,int index){
   if(e==null||!Generic7NormalNativeRulesV28.Supported(e.kind,e.sourceNPC)||catalog==null||catalog.dungeons==null||catalog.stages==null||selectedDungeon<0||selectedDungeon>2||selectedDungeon>=catalog.dungeons.Length||index<0||index>=game.enemies.Count||!object.ReferenceEquals(game.enemies[index],e))return false;
   var route=catalog.dungeons[selectedDungeon];int si=route.start+game.room;
   if(si<0||si>=9||si>=catalog.stages.Length)return false;var stage=catalog.stages[si];
   if(stage.id!=game.CurrentRoom.id||stage.spawns==null||index>=stage.spawns.Length)return false;var s=stage.spawns[index];
   if(s.kind!=e.kind||s.sourceNPC!=e.sourceNPC||s.encounterIndex!=e.encounterIndex||s.sourceKeyCode!=e.sourceKeyCode||s.x!=e.spawnX)return false;
   return Generic7NormalNativeRulesV28.OriginalBirth(CurrentNpcContentSet==null,selectedDungeon,route.id,route.sourceGraph,stage.id,stage.sourceWorldId,index,s.sourceSpawnIndex,e.kind,e.sourceNPC);
  }
  bool Generic7ProfilePresentV28(Enemy e){return catalog.npcRules!=null&&Array.Exists(catalog.npcRules,p=>p!=null&&p.id==e.sourceNPC);}
  string Generic7NormalRenderClipV28(Enemy e,int index,bool sourceVisible,string previous,ref float clock){
   string ruben=RubenSourceCarrierV45(e,index,sourceVisible,previous,ref clock);if(ruben!=previous)return ruben;
   string air4Selected;if(TryRemaining4AirRenderV28(e,index,sourceVisible,previous,ref clock,out air4Selected))return air4Selected;
   string remaining3; if(TryRemaining3SourceProfileRenderV28(e,index,sourceVisible,previous,ref clock,out remaining3))return remaining3;
   string guardianSelected;if(TryGuardianCombatRenderV28(e,index,sourceVisible,previous,ref clock,out guardianSelected))return guardianSelected;
   if(previous!=e.clip)return previous; // Preserve upstream fallback/clock unless the actual model clip is identical.
   bool birth=Generic7OriginalBirthV28(e,index),profile=Generic7NormalNativeRulesV28.Supported(e.kind,e.sourceNPC)&&Generic7ProfilePresentV28(e);Library lib=null;
   if(birth)libraries.TryGetValue(e.kind,out lib);float next;
   string result=Generic7NormalNativeRulesV28.Select(birth,profile,e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.npcState,previous,e.clock,clock,
    target=>{AnimationData data;if(lib==null||!lib.clips.TryGetValue(target,out data))return float.NaN;if(target=="source-idle"||target=="source-walk"){if(data.fps!=24||!data.loop||data.rootDistance!=0||lib.data.logicalScale!=1)throw new InvalidOperationException("Generic normal source metadata/scale mismatch");}return data.duration;},out next);
   bool hurtScope=StartedHurt11NativeRulesV28.GenericScope(birth,profile,e.kind,e.sourceNPC);float hurtClock;
   result=StartedHurt11NativeRulesV28.Select(hurtScope,e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,result,e.clock,next,()=>StartedHurt11MetadataReadyV28(e,lib),out hurtClock);
   bool groundScope=StartedGround24NativeRulesV28.GenericScope(birth,profile,e.kind,e.sourceNPC);float groundClock;
   result=StartedGround24NativeRulesV28.Select(groundScope,e.kind,e.sourceNPC,sourceVisible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,result,e.clock,e.deathTime,hurtClock,()=>StartedGround24MetadataReadyV28(e,lib),out groundClock);
   clock=groundClock;result=StartedRemaining3HurtRenderClipV28(e,index,sourceVisible,result,ref clock);result=Remaining3GroundRenderV28(e,index,sourceVisible,result,ref clock);result=StartedAttack28GenericRenderClipV28(e,index,sourceVisible,result,ref clock);return StartedAir26RenderClipV28(e,sourceVisible,result,ref clock,"generic7");
  }
  bool Generic7NormalFlipXV28(Enemy e,int index,string actualClip,bool previousFlip){
   bool air4Flip;if(Remaining4AirFlipV28(e,index,actualClip,out air4Flip))return air4Flip;
   bool guardianFlip;if(GuardianCombatFlipV28(e,index,actualClip,out guardianFlip))return guardianFlip;
   bool remaining3Flip;if(Remaining3SourceProfileFlipV28(e,index,actualClip,out remaining3Flip))return remaining3Flip;
   bool remainingGroundFlip;if(Remaining3GroundFlipV28(e,index,actualClip,out remainingGroundFlip))return remainingGroundFlip;
   if(e!=null&&actualClip==Remaining3HurtNativeRulesV28.Alias&&StartedRemaining3HurtEligibleV28(e,index,true)){var spec=Remaining3HurtNativeRulesV28.Find(e.kind,e.sourceNPC);Library lib=null;libraries.TryGetValue(e.kind,out lib);if(StartedRemaining3HurtMetadataReadyV28(e,lib,spec))return e.facing*spec.authoredFacing<0;}

   if(e==null||(e.kind!="toto"&&e.kind!="fairy-guardian")||!Generic7NormalNativeRulesV28.AliasFor(e.kind,e.sourceNPC,e.clip,actualClip))return previousFlip;
   bool eligible=Generic7NormalNativeRulesV28.Eligible(Generic7OriginalBirthV28(e,index),Generic7ProfilePresentV28(e),e.kind,e.sourceNPC,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.npcState,e.clip);
   return eligible?e.facing*Generic7NormalNativeRulesV28.AuthoredFacing(e.kind,e.sourceNPC)<0:previousFlip;
  }
  void ApplyGeneric7NormalUiV28(){
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];var v=enemyViews[i];if(!v||!v.sprite||!v.enabled||!v.gameObject.activeInHierarchy)continue;
    if(Remaining3SourceProfileApplyUiV28(e,i,v))continue;
    if(Remaining4AirApplyUiV28(e,i,v))continue;
    if(GuardianCombatApplyUiV28(e,i,v))continue;
    if(StartedAir26ApplyUiV28(e,i,v,"generic7"))continue;
    if(Remaining3GroundUiV28(e,i,v))continue;
    bool groundBirth=Generic7OriginalBirthV28(e,i);if(StartedGround24ApplyUiV28(e,i,v,StartedGround24NativeRulesV28.GenericScope(groundBirth,Generic7ProfilePresentV28(e),e.kind,e.sourceNPC)))continue;
    if(StartedRemaining3HurtApplyUiV28(e,i,v))continue;
    float attackClock=e.clock;string attackClip=StartedAttack28GenericRenderClipV28(e,i,true,e.clip,ref attackClock);if(StartedAttack28ApplyUiV28(e,i,v,attackClip))continue;
    bool eligible=Generic7NormalNativeRulesV28.Eligible(Generic7OriginalBirthV28(e,i),Generic7NormalNativeRulesV28.Supported(e.kind,e.sourceNPC)&&Generic7ProfilePresentV28(e),e.kind,e.sourceNPC,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.npcState,e.clip);
    bool hurt=StartedHurt11NativeRulesV28.Eligible(StartedHurt11NativeRulesV28.GenericScope(Generic7OriginalBirthV28(e,i),Generic7ProfilePresentV28(e),e.kind,e.sourceNPC),e.kind,e.sourceNPC,true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.clip,e.clip);
    Library hurtLib=null;if(hurt)libraries.TryGetValue(e.kind,out hurtLib);hurt=hurt&&StartedHurt11MetadataReadyV28(e,hurtLib);
    if(!eligible&&!hurt)continue;string name=v.sprite.name,prefix=e.kind+"/";int slash=name.LastIndexOf('/');if(!name.StartsWith(prefix,StringComparison.Ordinal)||slash<=prefix.Length)continue;
    string actual=name.Substring(prefix.Length,slash-prefix.Length);if(!(eligible&&Generic7NormalNativeRulesV28.AliasFor(e.kind,e.sourceNPC,e.clip,actual)||hurt&&actual=="source-hurt"))continue;
    float bx,by,dx,dy;
    bool ui=hurt?StartedHurt11NativeRulesV28.TryUiAnchor(hurt,actual,v.bounds.center.x,NativeVisibleTopYV28(v),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy):Generic7NormalNativeRulesV28.TryUiAnchor(eligible,e.kind,e.sourceNPC,e.clip,actual,v.bounds.center.x,NativeVisibleTopYV28(v),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy);if(!ui)continue;
    var b=enemyBars[i].transform.position;b.x=bx;b.y=by;enemyBars[i].transform.position=b;var d=enemyDebuffViews[i].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[i].transform.position=d;
   }
  }
 }
}
