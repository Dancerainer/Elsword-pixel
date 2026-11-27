using System;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool Remaining3GroundMetadataV28(Enemy e,Library lib,Remaining3GroundNativeRulesV28.Spec spec){
   AnimationData c;UnityEngine.Sprite[] frames;
   if(e==null||spec==null||lib==null||!lib.clips.TryGetValue(spec.alias,out c)||c==null||c.frames==null||c.hits==null||!lib.frames.TryGetValue(spec.alias,out frames)||frames==null||frames.Length!=spec.samples||c.frames.Length!=spec.samples||c.frames.Length==0||c.frames[0]==null||c.texture!=c.frames[0].texture)return false;
   bool coherent=true;for(int j=0;j<c.frames.Length;j++){var f=c.frames[j];var sp=frames[j];
    if(f==null||!f.hasAnchor||f.rootX!=0||HasStartedNativeFrameAttackPayloadV28(f.attackGeometry)||!Remaining3GroundNativeRulesV28.Resource(spec,f.texture)||f.x<0||f.y<0||f.w<=0||f.h<=0||!Remaining3GroundNativeRulesV28.Finite(f.anchorX)||!Remaining3GroundNativeRulesV28.Finite(f.anchorY)||!sp){coherent=false;break;}
    var page=sp.texture;if(!page||page.width<=0||page.height<=0||page.width>4096||page.height>4096||!page.isReadable||page.mipmapCount!=1||page.filterMode!=UnityEngine.FilterMode.Point||page.format!=UnityEngine.TextureFormat.RGBA32||page!=AtlasTexture(f.texture)||f.w>page.width||f.h>page.height||f.x>page.width-f.w||f.y>page.height-f.h||sp.pixelsPerUnit!=1||sp.name!=e.kind+"/"+spec.alias+"/"+j||sp.rect.x!=f.x||sp.rect.y!=page.height-f.y-f.h||sp.rect.width!=f.w||sp.rect.height!=f.h){coherent=false;break;}
   }
   return Remaining3GroundNativeRulesV28.Validate(spec,c.id,c.command==null?null:c.command.evidence,lib.data.logicalScale,c.texture,c.fps,c.duration,c.loop,c.rootDistance,c.frames.Length,c.hits.Length,coherent);
  }
  string Remaining3GroundRenderV28(Enemy e,int index,bool visible,string previous,ref float clock){
   if(e==null)return previous;var s=Remaining3GroundNativeRulesV28.Find(e.kind,e.sourceNPC,(int)e.state);bool birth=s!=null&&Generic7OriginalBirthV28(e,index);Library lib=null;if(birth)libraries.TryGetValue(e.kind,out lib);float next;
   string selected=Remaining3GroundNativeRulesV28.Select(s,birth,visible,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,e.clip,previous,e.clock,e.deathTime,clock,()=>Remaining3GroundMetadataV28(e,lib,s),out next);clock=next;return selected;
  }
  bool Remaining3GroundFlipV28(Enemy e,int index,string alias,out bool flip){
   flip=false;if(e==null)return false;var s=Remaining3GroundNativeRulesV28.Find(e.kind,e.sourceNPC,(int)e.state);
   if(s==null||alias!=s.alias||!Remaining3GroundNativeRulesV28.Eligible(s,Generic7OriginalBirthV28(e,index),true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,e.clip,e.clip))return false;
   Library lib=null;libraries.TryGetValue(e.kind,out lib);if(!Remaining3GroundMetadataV28(e,lib,s))return false;flip=e.facing*s.authoredFacing<0;return true;
  }
  bool Remaining3GroundUiV28(Enemy e,int index,UnityEngine.SpriteRenderer view){
   if(e==null||index<0||index>=enemyBars.Count||index>=enemyDebuffViews.Count||!enemyBars[index]||!enemyBars[index].enabled||!enemyBars[index].gameObject.activeInHierarchy||!enemyDebuffViews[index]||!view||!view.sprite||!view.enabled||!view.gameObject.activeInHierarchy)return false;
   var s=Remaining3GroundNativeRulesV28.Find(e.kind,e.sourceNPC,(int)e.state);if(!Remaining3GroundNativeRulesV28.Eligible(s,Generic7OriginalBirthV28(e,index),true,e.active,e.dormant,e.passive,e.aiEnabled,e.hp,e.clip,e.clip)||s.combat==5)return false;
   Library lib=null;libraries.TryGetValue(e.kind,out lib);UnityEngine.Sprite[] frames;
   if(!Remaining3GroundMetadataV28(e,lib,s)||!view.sprite.name.StartsWith(e.kind+"/"+s.alias+"/",StringComparison.Ordinal)||!lib.frames.TryGetValue(s.alias,out frames)||Array.IndexOf(frames,view.sprite)<0)return false;
   float bx,by,dx,dy;if(!Remaining3GroundNativeRulesV28.Ui(s,s.alias,view.bounds.center.x,NativeVisibleTopYV28(view),e.maxHp>400?64:30,e.hp,e.maxHp,out bx,out by,out dx,out dy))return false;
   var b=enemyBars[index].transform.position;b.x=bx;b.y=by;enemyBars[index].transform.position=b;var d=enemyDebuffViews[index].transform.position;d.x=dx;d.y=dy;enemyDebuffViews[index].transform.position=d;return true;
  }
 }
}

