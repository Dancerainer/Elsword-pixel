using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class RxFrameV45 {public int page,x,y,w,h;public float anchorX,anchorY;}
  [Serializable] sealed class RxClipV45 {public string alias,source;public int[] indices;public int fps;public float duration;}
  [Serializable] sealed class RxLibraryV45 {public string id,sourceNPC,artVersion,sourceSha256;public int pageCount;public RxFrameV45[] frames;public RxClipV45[] clips;}
  readonly Dictionary<string,RxLibraryV45> rxBanksV45=new Dictionary<string,RxLibraryV45>();
  readonly List<Sprite> rxMadeV45=new List<Sprite>();
  static bool RxKindV45(string k){
   switch(k){case "soldier-benders":case "soldier-crossbow":case "wally-8th":case "anger-forest-knight":case "william-cute":case "william-thief":case "curse-wisp":case "bat-a":case "bat-b":return true;default:return false;}
  }
  bool TryOriginalReactionInkV45(Enemy e,OriginalHurtStateV45 p,ref string alias,ref float clock){
   if(!RxKindV45(e.kind)||p==null||!libraries.TryGetValue(e.kind,out var lib))return false;
   string prefix="Pixel/reaction-ink-v45/"+e.kind+"/";if(!rxBanksV45.TryGetValue(e.kind,out var bank)){
    var file=Resources.Load<TextAsset>(prefix+"library");if(file)bank=JsonUtility.FromJson<RxLibraryV45>(file.text);rxBanksV45.Add(e.kind,bank);
   }
   if(bank==null||bank.id!=e.kind||bank.sourceNPC!=e.sourceNPC||bank.artVersion!=lib.data.artVersion)return false;
   var c=Array.Find(bank.clips,t=>t.source==p.motion);if(c==null)return false;
   if(!lib.clips.ContainsKey(c.alias)){
    if(c.fps!=24||c.duration<=0||c.indices.Length<1)throw new InvalidOperationException("Invalid original reaction clock "+p.motion);
    var data=new FrameData[c.indices.Length];var frames=new Sprite[data.Length];var idMap=new Dictionary<int,int>();
    for(int i=0;i<data.Length;i++){int ix=c.indices[i];
     if(idMap.TryGetValue(ix,out int prev)){frames[i]=frames[prev];data[i]=data[prev];continue;}
     var f=bank.frames[ix];if(f.page<0||f.page>=bank.pageCount)throw new InvalidOperationException("Original reaction page");
     string path=prefix+"page-"+f.page;var tex=AtlasTexture(path);if(!tex||tex.filterMode!=FilterMode.Point||f.w<=0||f.h<=0||f.x<0||f.y<0||f.x+f.w>tex.width||f.y+f.h>tex.height)throw new InvalidOperationException("Missing original reaction page "+path);
     data[i]=new FrameData{x=f.x,y=f.y,w=f.w,h=f.h,hasAnchor=true,anchorX=f.anchorX,anchorY=f.anchorY,texture=path};
     var s=Sprite.Create(tex,new Rect(f.x,tex.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
     s.name=e.kind+"/"+c.alias+"/"+i;frames[i]=s;spriteOffsets[s]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);
     // Every registered reaction rectangle is trimmed to its non-transparent pixels
     // on all four edges. UI can use that exact local rectangle without reading
     // an entire GPU atlas back into CPU memory during a hit.
     nativeUiVisibleLocalRectsV28[s]=new Rect(-f.w*.5f,-f.h*.5f,f.w,f.h);rxMadeV45.Add(s);idMap.Add(ix,i);
    }
    lib.clips.Add(c.alias,new AnimationData{id=c.alias,texture=prefix+"page-0",duration=c.duration,fps=c.fps,loop=false,frames=data});lib.frames[c.alias]=frames;
   }
   alias=c.alias;clock=e.clock*Math.Max(.01f,p.speed);return true;
  }
  void DisposeSourceReactionInkV45(){foreach(var s in rxMadeV45)if(s){spriteOffsets.Remove(s);nativeUiVisibleLocalRectsV28.Remove(s);DestroyNativeWorldObject(s);}rxMadeV45.Clear();rxBanksV45.Clear();}
 }
}
