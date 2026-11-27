using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  static int Mine2ProductPngIntR1233(byte[] b,int at){return checked((int)((uint)b[at]<<24|(uint)b[at+1]<<16|(uint)b[at+2]<<8|b[at+3]));}
  Texture2D LoadMine2ProductTextureR1233(Mine2ProductFileR1211 file,int width,int height,string label){
   var bytes=mine2ProductIndexR1233.Read(file);Mine2ProductNeedR1233(bytes.Length>=24&&bytes[0]==137&&bytes[1]==80&&bytes[2]==78&&bytes[3]==71&&bytes[12]==73&&bytes[13]==72&&bytes[14]==68&&bytes[15]==82,"original PNG header absent "+label);
   int w=Mine2ProductPngIntR1233(bytes,16),h=Mine2ProductPngIntR1233(bytes,20);Mine2ProductNeedR1233(w>0&&h>0&&w<=SystemInfo.maxTextureSize&&h<=SystemInfo.maxTextureSize&&(width==0||width==w)&&(height==0||height==h),"original PNG cannot be preserved on this device "+label);
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name=label,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};mine2ProductOwnedR1233.Add(texture);
   Mine2ProductNeedR1233(ImageConversion.LoadImage(texture,bytes,false)&&texture.width==w&&texture.height==h,"original PNG decode/dimensions changed "+label);return texture;
  }
  void LoadMine2ProductLibraryR1233(Mine2ProductLibraryPayloadR1211 payload){
   var row=payload.binding;var raw=payload.document;string key="mine2-product-owned-r1233/"+row.id;
   Mine2ProductNeedR1233(!atlasTextures.ContainsKey(key),"private atlas slot already occupied "+key);
   Texture2D texture=null;Func<Texture2D> originalPage=()=>{if(!texture){texture=LoadMine2ProductTextureR1233(row.atlas,payload.atlasWidth,payload.atlasHeight,"Mine2 original atlas / "+row.id);texture=CombatMachineArtV40.ProductPage(row.npc,texture);atlasTextures.Add(key,texture);}return texture;};
   RegisterPrivateAtlasV53(key,originalPage);mine2ProductAtlasKeysR1233.Add(key);if(!DeferMonsterPixelCarrierV53(raw.id))originalPage();
   Library prior;if(libraries.TryGetValue(raw.id,out prior))mine2ProductPriorLibrariesR1233.Add(raw.id,prior);mine2ProductLibrariesR1233.Add(raw.id);
   var clips=new List<AnimationData>();foreach(var c in raw.clips){var frames=new FrameData[c.frames.Length];for(int i=0;i<frames.Length;i++){var f=c.frames[i];frames[i]=new FrameData{x=f.rect[0],y=f.rect[1],w=f.rect[2],h=f.rect[3],hasAnchor=true,anchorX=f.anchor[0],anchorY=f.anchor[1]};}clips.Add(new AnimationData{id=c.id,fps=c.fps,duration=c.durationSeconds,loop=c.loop,frames=frames});}
   var data=new SpriteLibraryData{id=raw.id,texture=key,artVersion=raw.artVersion,paintedPoseCount=raw.paintedPoseCount,standingHeight=raw.standingHeight,logicalScale=raw.logicalScale,frameWidth=raw.atlas.frameWidth,frameHeight=raw.atlas.frameHeight,anchorX=raw.atlas.groundAnchor[0],anchorY=raw.atlas.groundAnchor[1],clips=clips.ToArray()};
   var lib=new Library{data=data};libraries[raw.id]=lib;
   if(!DeferPrivateFramesV53(lib,frame=>originalPage(),mine2ProductOwnedR1233,true))foreach(var clip in data.clips){if(BuildPrivateMotionV45(lib,clip,mine2ProductOwnedR1233))continue;var sprites=new Sprite[clip.frames.Length];for(int i=0;i<sprites.Length;i++){var f=clip.frames[i];var sprite=Sprite.Create(texture,new Rect(f.x,texture.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);mine2ProductOwnedR1233.Add(sprite);sprite.name=data.id+"/"+clip.id+"/"+i;spriteOffsets[sprite]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);sprites[i]=sprite;}lib.frames.Add(clip.id,sprites);lib.clips.Add(clip.id,clip);}
   var rule=Array.Find(mine2ProductPlanR1233.rules,r=>r.id==row.npc);foreach(var state in rule.states)Mine2ProductNeedR1233(lib.frames.ContainsKey(state.clip)&&lib.clips.ContainsKey(state.clip),"original state display clip absent; idle fallback forbidden "+row.npc+"/"+state.id);
   if(row.npc==MineMachineRigV35.RailId){mine2RailLibraryV35=lib;mine2RailProfileV35=rule;}
  }
  // Full PCM16 verification is independent of importer rate/frames/channel metadata.
  // The original WAV is PCM16; a lossy/resampled import cannot silently pass here.
  static void RequireMine2ProductPcmR1233(Mine2ProductAudioR1211 binding,AudioClip clip){
   int count=checked(clip.samples*clip.channels);var values=new float[count];Mine2ProductNeedR1233(clip.GetData(values,0),"imported PCM must be decompressed/readable "+binding.id);var pcm=new byte[checked(count*2)];
   for(int i=0;i<count;i++){float value=values[i];Mine2ProductNeedR1233(!float.IsNaN(value)&&!float.IsInfinity(value)&&value>=-1&&value<=1,"invalid imported PCM sample "+binding.id);int sample=checked((int)Math.Round((double)value*32768,MidpointRounding.ToEven));Mine2ProductNeedR1233(sample>=short.MinValue&&sample<=short.MaxValue,"imported PCM16 range changed "+binding.id);short s=(short)sample;pcm[i*2]=(byte)(s&255);pcm[i*2+1]=(byte)((s>>8)&255);}
   Mine2ProductNeedR1233(BelowPlayerResourceIndexV21.Hash(pcm)==binding.decodedPcm16Sha256,"imported original PCM16 bytes changed "+binding.id);
  }
  void ConfigureMine2ProductWorldLayerR1233(Mine2WorldLayerDocumentV20 document,Texture2D texture){
   Mine2ProductNeedR1233(hudCanvas&&worldCamera&&!mine2ProductViewportR1233,"dedicated Mine2 world viewport unavailable");var root=new GameObject("Mine2 product dedicated world camera viewport",typeof(RectTransform));mine2ProductOwnedR1233.Add(root);root.transform.SetParent(hudCanvas.transform,false);root.transform.SetAsFirstSibling();mine2ProductViewportR1233=root.GetComponent<RectTransform>();mine2ProductViewportR1233.localScale=Vector3.one;mine2ProductViewportR1233.localRotation=Quaternion.identity;UpdateMine2ProductViewportR1233();ConfigureMine2WorldLayerV20(document,texture,mine2ProductViewportR1233);
  }
  void UpdateMine2ProductViewportR1233(){if(!mine2ProductViewportR1233||!worldCamera)return;var rect=worldCamera.rect;mine2ProductViewportR1233.anchorMin=rect.min;mine2ProductViewportR1233.anchorMax=rect.max;mine2ProductViewportR1233.offsetMin=mine2ProductViewportR1233.offsetMax=Vector2.zero;}
  void RenderMine2ProductWorldLayerR1233(){if(!Mine2ProductSessionActiveR1233)return;UpdateMine2ProductViewportR1233();RenderMine2WorldLayerV20();}
 }
}
