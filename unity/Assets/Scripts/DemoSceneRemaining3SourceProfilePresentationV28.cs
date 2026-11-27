using System;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool Remaining3SourceProfileScopeV28(Enemy e,int index,bool visible){
   return e!=null&&Generic7OriginalBirthV28(e,index)&&game.Remaining3SourceProfileV28(e)&&visible&&e.active&&!e.dormant&&!e.passive&&e.aiEnabled&&e.hp>0;
  }
  bool Remaining3SourceProfileMetadataV28(Enemy e,Library lib,Remaining3AttackSourceRulesV28.Spec spec){
   AnimationData a;Sprite[] sprites;
   if(e==null||spec==null||lib==null||lib.data.logicalScale!=1||!lib.clips.TryGetValue(spec.alias,out a)||a==null||a.frames==null||a.hits==null||a.hits.Length!=0||!lib.frames.TryGetValue(spec.alias,out sprites)||sprites==null||sprites.Length!=spec.samples||a.frames.Length!=spec.samples||a.fps!=24||!Remaining3AttackSourceRulesV28.Finite(a.duration)||Math.Abs(a.duration-spec.sourceSpan)>.000001f||a.loop!=spec.loop||a.rootDistance!=0)return false;
   // Existing accepted normal aliases keep their original header. New attack
   // aliases require their own future typed NPC-only admission marker.
   if(!spec.Normal&&(a.command==null||a.command.evidence!=spec.Evidence||!Remaining3AttackSourceRulesV28.Resource(spec,a.texture)))return false;
   for(int i=0;i<a.frames.Length;i++){
    var f=a.frames[i];var sprite=sprites[i];
    if(f==null||!f.hasAnchor||f.rootX!=0||!Remaining3AttackSourceRulesV28.Finite(f.anchorX)||!Remaining3AttackSourceRulesV28.Finite(f.anchorY)||f.x<0||f.y<0||f.w<=0||f.h<=0||!sprite||!StartedAttack28PayloadFiniteV28(f.attackGeometry))return false;
    string resource=string.IsNullOrEmpty(f.texture)?string.IsNullOrEmpty(a.texture)?lib.data.texture:a.texture:f.texture;
    if(!spec.Normal&&!Remaining3AttackSourceRulesV28.Resource(spec,resource))return false;
    var page=sprite.texture;
    if(!page||!page.isReadable||page.format!=TextureFormat.RGBA32||page.filterMode!=FilterMode.Point||page.mipmapCount!=1||page.width>4096||page.height>4096||page!=AtlasTexture(resource)||f.x>page.width-f.w||f.y>page.height-f.h||sprite.name!=e.kind+"/"+spec.alias+"/"+i||sprite.rect.x!=f.x||sprite.rect.y!=page.height-f.y-f.h||sprite.rect.width!=f.w||sprite.rect.height!=f.h||sprite.pixelsPerUnit!=1)return false;
   }
   return true;
  }
  bool TryRemaining3SourceProfileRenderV28(Enemy e,int index,bool visible,string previous,ref float clock,out string selected){
   selected=previous;if(previous!=e.clip||!Remaining3SourceProfileScopeV28(e,index,visible))return false;
   Library lib=null;libraries.TryGetValue(e.kind,out lib);
   if(e.state==CombatState.Hurt&&e.clip=="hurt"&&string.IsNullOrEmpty(e.npcState)){
    var hurt=Remaining3HurtNativeRulesV28.Find(e.kind,e.sourceNPC);
    if(!StartedRemaining3HurtMetadataReadyV28(e,lib,hurt))return false;
    // Same admitted Hurt bank and original physical .28 prefix, actor clock.
    float next;selected=Remaining3HurtNativeRulesV28.Select(true,e.kind,e.sourceNPC,visible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,(int)e.state,e.npcState,e.clip,previous,e.clock,clock,s=>StartedRemaining3HurtMetadataReadyV28(e,lib,s),out next);clock=next;return selected!=previous;
   }
   // Source ground display is owned by Root's independent Ground5 branch.
   if(e.state!=CombatState.Normal)return false;
   var spec=Remaining3AttackSourceRulesV28.Find(e.kind,e.sourceNPC,e.npcState);
   if(spec==null||spec.modelClip!=e.clip||!Remaining3SourceProfileMetadataV28(e,lib,spec)||!Remaining3AttackSourceRulesV28.Finite(e.npcTime)||e.npcTime<0)return false;
   selected=spec.alias;clock=e.npcTime*spec.speed;return true;
  }
  bool Remaining3SourceProfileFlipV28(Enemy e,int index,string actual,out bool flip){
   flip=e!=null&&e.facing<0;if(!Remaining3SourceProfileScopeV28(e,index,true))return false;
   float clock=e.clock;string selected;if(!TryRemaining3SourceProfileRenderV28(e,index,true,e.clip,ref clock,out selected)||selected!=actual)return false;
   flip=e.facing*Remaining3AttackSourceRulesV28.AuthoredFacing(e.kind)<0;return true;
  }
  bool Remaining3SourceProfileApplyUiV28(Enemy e,int index,SpriteRenderer view){
   if(!Remaining3SourceProfileScopeV28(e,index,true)||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index]||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)return false;
   float clock=e.clock;string actual;if(!TryRemaining3SourceProfileRenderV28(e,index,true,e.clip,ref clock,out actual))return false;
   Library lib=null;Sprite[] sprites;libraries.TryGetValue(e.kind,out lib);
   if(lib==null||!lib.frames.TryGetValue(actual,out sprites)||Array.IndexOf(sprites,view.sprite)<0||!view.sprite.name.StartsWith(e.kind+"/"+actual+"/",StringComparison.Ordinal))return false;
   float bx,by,dx,dy;
   if(actual==Remaining3HurtNativeRulesV28.Alias){if(!Remaining3HurtNativeRulesV28.TryUi(true,actual,view.bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))return false;}
   else {
    // Retain the existing left HP anchor and current visible alpha top.
    float width=e.maxHp>400?64:30;bx=view.bounds.center.x-width*(1-e.hp/e.maxHp)/2;by=NativeVisibleTopYV28(view)+8;dx=view.bounds.center.x;dy=NativeVisibleTopYV28(view)+20;
   }
   var bar=enemyBars[index].transform.position;bar.x=bx;bar.y=by;enemyBars[index].transform.position=bar;var debuff=enemyDebuffViews[index].transform.position;debuff.x=dx;debuff.y=dy;enemyDebuffViews[index].transform.position=debuff;return true;
  }
 }
}
