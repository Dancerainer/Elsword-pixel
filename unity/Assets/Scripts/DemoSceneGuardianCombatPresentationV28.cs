using System;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool GuardianSourceScopeV28(Enemy e,int index,bool visible){
   return e!=null&&GuardianCombatNativeRulesV28.Identity(e.kind,e.sourceNPC)&&Generic7OriginalBirthV28(e,index)&&game.GuardianSourceProfileV28(e)&&visible&&e.active&&!e.dormant&&!e.passive&&e.aiEnabled&&(e.state==CombatState.Dead?e.hp<=0:e.hp>0);
  }
  bool GuardianCombatMetadataV28(Enemy e,Library lib,GuardianCombatNativeRulesV28.Spec spec){
   AnimationData c;Sprite[] sprites;if(e==null||spec==null||lib==null||lib.data.logicalScale!=1||!lib.clips.TryGetValue(spec.alias,out c)||c==null||c.id!=spec.alias||c.frames==null||c.hits==null||c.hits.Length!=0||c.frames.Length!=spec.samples||!lib.frames.TryGetValue(spec.alias,out sprites)||sprites==null||sprites.Length!=spec.samples||c.fps!=24||!GuardianCombatNativeRulesV28.Finite(c.duration)||Math.Abs(c.duration-spec.span)>.000001f||c.loop!=spec.loop||c.rootDistance!=0||!GuardianCombatNativeRulesV28.Resource(spec,c.texture))return false;
   if(!spec.normal&&(c.command==null||c.command.evidence!=spec.Evidence))return false;
   for(int j=0;j<c.frames.Length;j++){
    var f=c.frames[j];var sp=sprites[j];if(f==null||!f.hasAnchor||f.rootX!=0||!GuardianCombatNativeRulesV28.Finite(f.anchorX)||!GuardianCombatNativeRulesV28.Finite(f.anchorY)||f.x<0||f.y<0||f.w<=0||f.h<=0||HasStartedNativeFrameAttackPayloadV28(f.attackGeometry)||!sp)return false;
    string resource=string.IsNullOrEmpty(f.texture)?c.texture:f.texture;if(!GuardianCombatNativeRulesV28.Resource(spec,resource))return false;var page=sp.texture;
    if(!page||!page.isReadable||page.format!=TextureFormat.RGBA32||page.filterMode!=FilterMode.Point||page.mipmapCount!=1||page.width>4096||page.height>4096||page!=AtlasTexture(resource)||f.x>page.width-f.w||f.y>page.height-f.h||sp.pixelsPerUnit!=1||sp.name!=e.kind+"/"+spec.alias+"/"+j||sp.rect.x!=f.x||sp.rect.y!=page.height-f.y-f.h||sp.rect.width!=f.w||sp.rect.height!=f.h)return false;
   }
   return true;
  }
  bool TryGuardianCombatRenderV28(Enemy e,int index,bool visible,string previous,ref float clock,out string selected){
   selected=previous;if(e==null||previous!=e.clip||!GuardianSourceScopeV28(e,index,visible))return false;
   var spec=GuardianCombatNativeRulesV28.Find((int)e.state,e.npcState,e.clip);if(spec==null)return false;
   float next=GuardianCombatNativeRulesV28.Clock(spec,e.npcTime,e.clock,e.deathTime,game.FairyGetUpSecondsV34(e));if(!GuardianCombatNativeRulesV28.Finite(next)||next<0)return false;
   Library lib=null;libraries.TryGetValue(e.kind,out lib);if(!GuardianCombatMetadataV28(e,lib,spec))return false;
   selected=spec.alias;clock=next;return true;
  }
  bool GuardianCombatFlipV28(Enemy e,int index,string actual,out bool flip){
   flip=e!=null&&e.facing<0;float clock=e==null?0:e.clock;string selected;
   if(e==null||!TryGuardianCombatRenderV28(e,index,true,e.clip,ref clock,out selected)||selected!=actual)return false;flip=e.facing>0;return true;
  }
  bool GuardianCombatApplyUiV28(Enemy e,int index,SpriteRenderer view){
   if(e==null||e.state==CombatState.Dead||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index]||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)return false;
   float clock=e.clock;string selected;if(!TryGuardianCombatRenderV28(e,index,true,e.clip,ref clock,out selected))return false;
   Library lib=null;Sprite[] sprites;libraries.TryGetValue(e.kind,out lib);if(lib==null||!lib.frames.TryGetValue(selected,out sprites)||Array.IndexOf(sprites,view.sprite)<0||!view.sprite.name.StartsWith(e.kind+"/"+selected+"/",StringComparison.Ordinal))return false;
   float width=e.maxHp>400?64:30,top=NativeVisibleTopYV28(view);var bar=enemyBars[index].transform.position;bar.x=view.bounds.center.x-width*(1-e.hp/e.maxHp)/2;bar.y=top+8;enemyBars[index].transform.position=bar;var text=enemyDebuffViews[index].transform.position;text.x=view.bounds.center.x;text.y=top+20;enemyDebuffViews[index].transform.position=text;return true;
  }
 }
}
