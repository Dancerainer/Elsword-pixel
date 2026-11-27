using System;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool Remaining4AirBirthV28(Enemy e,int index,Remaining4AirNativeRulesV28.Spec spec){
   if(e==null||spec==null||!Generic7OriginalBirthV28(e,index))return false;var route=catalog.dungeons[selectedDungeon];int si=route.start+game.room;var stage=catalog.stages[si];
   return index>=0&&stage.spawns!=null&&index<stage.spawns.Length&&Remaining4AirNativeRulesV28.OriginalBirth(spec,stage.id,index,stage.spawns[index].sourceSpawnIndex);
  }
  bool Remaining4AirProfileV28(Enemy e,Remaining4AirNativeRulesV28.Spec spec){return spec!=null&&(spec.guardianReuse?game.GuardianSourceProfileV28(e):game.Remaining3SourceProfileV28(e));}
  bool Remaining4AirEligibleV28(Enemy e,int index,bool visible,Remaining4AirNativeRulesV28.Spec spec,string previous){
   return e!=null&&Remaining4AirNativeRulesV28.Eligible(spec,Remaining4AirBirthV28(e,index,spec),Remaining4AirProfileV28(e,spec),visible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.npcState,e.clip,previous);
  }
  bool Remaining4AirMetadataV28(Enemy e,Library lib,Remaining4AirNativeRulesV28.Spec spec){
   if(e==null||lib==null||spec==null)return false;
   if(spec.guardianReuse){
    // Metadata only. Air must not reuse the Down eligibility/combat3 gate.
    var down=GuardianCombatNativeRulesV28.Find(3,"","knockdown");return down!=null&&down.alias==spec.alias&&down.samples==spec.samples&&down.animation==spec.animation&&down.span==spec.duration&&GuardianCombatMetadataV28(e,lib,down);
   }
   AnimationData c;Sprite[] sprites;if(lib.data.logicalScale!=1||!lib.clips.TryGetValue(spec.alias,out c)||c==null||c.id!=spec.alias||c.command==null||c.command.evidence!=spec.Evidence||c.fps!=24||!Remaining4AirNativeRulesV28.Finite(c.duration)||Math.Abs(c.duration-spec.duration)>.000001f||c.loop||c.rootDistance!=0||c.hits==null||c.hits.Length!=0||c.frames==null||c.frames.Length!=spec.samples||!Remaining4AirNativeRulesV28.Resource(spec,c.texture)||!lib.frames.TryGetValue(spec.alias,out sprites)||sprites==null||sprites.Length!=spec.samples)return false;
   for(int j=0;j<c.frames.Length;j++){
    var f=c.frames[j];var sp=sprites[j];if(f==null||!f.hasAnchor||f.rootX!=0||!Remaining4AirNativeRulesV28.Finite(f.anchorX)||!Remaining4AirNativeRulesV28.Finite(f.anchorY)||f.x<0||f.y<0||f.w<=0||f.h<=0||HasStartedNativeFrameAttackPayloadV28(f.attackGeometry)||!sp)return false;
    string resource=string.IsNullOrEmpty(f.texture)?c.texture:f.texture;if(!Remaining4AirNativeRulesV28.Resource(spec,resource))return false;var page=sp.texture;
    if(!page||!page.isReadable||page.format!=TextureFormat.RGBA32||page.filterMode!=FilterMode.Point||page.mipmapCount!=1||page.width>4096||page.height>4096||page!=AtlasTexture(resource)||f.x>page.width-f.w||f.y>page.height-f.h||sp.pixelsPerUnit!=1||sp.name!=e.kind+"/"+spec.alias+"/"+j||sp.rect.x!=f.x||sp.rect.y!=page.height-f.y-f.h||sp.rect.width!=f.w||sp.rect.height!=f.h)return false;
   }
   return true;
  }
  bool TryRemaining4AirRenderV28(Enemy e,int index,bool visible,string previous,ref float clock,out string selected){
   selected=previous;if(e==null)return false;var spec=Remaining4AirNativeRulesV28.Find(e.kind,e.sourceNPC);bool eligible=Remaining4AirEligibleV28(e,index,visible,spec,previous);Library lib=null;if(eligible)libraries.TryGetValue(e.kind,out lib);float next;
   selected=Remaining4AirNativeRulesV28.Select(spec,eligible,previous,e.clock,clock,()=>Remaining4AirMetadataV28(e,lib,spec),out next);clock=next;return selected!=previous;
  }
  bool Remaining4AirFlipV28(Enemy e,int index,string actual,out bool flip){
   flip=e!=null&&e.facing<0;if(e==null||!Remaining4AirNativeRulesV28.Finite(e.clock)||e.clock<0)return false;var spec=Remaining4AirNativeRulesV28.Find(e.kind,e.sourceNPC);if(!Remaining4AirEligibleV28(e,index,true,spec,e.clip)||actual!=spec.alias)return false;
   Library lib=null;libraries.TryGetValue(e.kind,out lib);if(!Remaining4AirMetadataV28(e,lib,spec))return false;flip=e.facing*spec.authoredFacing<0;return true;
  }
  bool Remaining4AirApplyUiV28(Enemy e,int index,SpriteRenderer view){
   if(e==null||!Remaining4AirNativeRulesV28.Finite(e.maxHp)||e.maxHp<=0||e.hp>e.maxHp||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index]||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)return false;
   var spec=Remaining4AirNativeRulesV28.Find(e.kind,e.sourceNPC);if(!Remaining4AirEligibleV28(e,index,true,spec,e.clip)||!Remaining4AirNativeRulesV28.Finite(e.clock)||e.clock<0)return false;Library lib=null;libraries.TryGetValue(e.kind,out lib);Sprite[] sprites;
   if(!Remaining4AirMetadataV28(e,lib,spec)||!lib.frames.TryGetValue(spec.alias,out sprites)||Array.IndexOf(sprites,view.sprite)<0||!view.sprite.name.StartsWith(e.kind+"/"+spec.alias+"/",StringComparison.Ordinal))return false;
   float width=e.maxHp>400?64:30,top=NativeVisibleTopYV28(view);var bar=enemyBars[index].transform.position;bar.x=view.bounds.center.x-width*(1-e.hp/e.maxHp)/2;bar.y=top+8;enemyBars[index].transform.position=bar;var text=enemyDebuffViews[index].transform.position;text.x=view.bounds.center.x;text.y=top+20;enemyDebuffViews[index].transform.position=text;return true;
  }
 }
}
