using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  static int PlainProductPngIntR1320(byte[] b,int at){return checked((int)((uint)b[at]<<24|(uint)b[at+1]<<16|(uint)b[at+2]<<8|b[at+3]));}
  Texture2D LoadPlainProductTextureR1320(Mine2ProductFileR1211 file,int width,int height,string label){
   var bytes=plainProductIndexR1320.Read(file);PlainProductNeedR1320(bytes.Length>=24&&bytes[0]==137&&bytes[1]==80&&bytes[2]==78&&bytes[3]==71&&bytes[12]==73&&bytes[13]==72&&bytes[14]==68&&bytes[15]==82,"original PNG header absent "+label);
   int w=PlainProductPngIntR1320(bytes,16),h=PlainProductPngIntR1320(bytes,20);PlainProductNeedR1320(w>0&&h>0&&w<=SystemInfo.maxTextureSize&&h<=SystemInfo.maxTextureSize&&(width==0||width==w)&&(height==0||height==h),"original PNG cannot be preserved on this device "+label);
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name=label,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};plainProductOwnedR1320.Add(texture);
   PlainProductNeedR1320(ImageConversion.LoadImage(texture,bytes,false)&&texture.width==w&&texture.height==h,"original PNG decode/dimensions changed "+label);return texture;
  }
  void LoadPlainProductLibraryR1320(Mine2ProductLibraryPayloadR1211 payload){
   var row=payload.binding;var raw=payload.document;string key="plain-product-owned-r1320/"+row.id;
   PlainProductNeedR1320(!atlasTextures.ContainsKey(key),"private atlas slot already occupied "+key);
   Texture2D texture=null;Func<Texture2D> originalPage=()=>{if(!texture){texture=LoadPlainProductTextureR1320(row.atlas,payload.atlasWidth,payload.atlasHeight,"Plain original atlas / "+row.id);if(row.npc=="NUI_ALTERA_MINE")texture=CombatMachineArtV40.ProductPage(row.npc,texture);atlasTextures.Add(key,texture);}return texture;};
   RegisterPrivateAtlasV53(key,originalPage);plainProductAtlasKeysR1320.Add(key);if(!DeferMonsterPixelCarrierV53(raw.id))originalPage();
   Library prior;if(libraries.TryGetValue(raw.id,out prior))plainProductPriorLibrariesR1320.Add(raw.id,prior);plainProductLibrariesR1320.Add(raw.id);
   var clips=new List<AnimationData>();foreach(var c in raw.clips){var frames=new FrameData[c.frames.Length];for(int i=0;i<frames.Length;i++){var f=c.frames[i];frames[i]=new FrameData{x=f.rect[0],y=f.rect[1],w=f.rect[2],h=f.rect[3],hasAnchor=true,anchorX=f.anchor[0],anchorY=f.anchor[1]};}clips.Add(new AnimationData{id=c.id,fps=c.fps,duration=c.durationSeconds,loop=c.loop,frames=frames});}
   var data=new SpriteLibraryData{id=raw.id,texture=key,artVersion=raw.artVersion,paintedPoseCount=raw.paintedPoseCount,standingHeight=raw.standingHeight,logicalScale=raw.logicalScale,frameWidth=raw.atlas.frameWidth,frameHeight=raw.atlas.frameHeight,anchorX=raw.atlas.groundAnchor[0],anchorY=raw.atlas.groundAnchor[1],clips=clips.ToArray()};
   var lib=new Library{data=data};libraries[raw.id]=lib;
   if(!DeferPrivateFramesV53(lib,frame=>originalPage(),plainProductOwnedR1320,false))foreach(var clip in data.clips){var sprites=new Sprite[clip.frames.Length];for(int i=0;i<sprites.Length;i++){var f=clip.frames[i];var sprite=Sprite.Create(texture,new Rect(f.x,texture.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);plainProductOwnedR1320.Add(sprite);sprite.name=data.id+"/"+clip.id+"/"+i;spriteOffsets[sprite]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);sprites[i]=sprite;}lib.frames.Add(clip.id,sprites);lib.clips.Add(clip.id,clip);}
   var rule=Array.Find(plainProductPlanR1320.rules,r=>r.id==row.npc);foreach(var state in rule.states)PlainProductNeedR1320(lib.frames.ContainsKey(state.clip)&&lib.clips.ContainsKey(state.clip),"original state display clip absent; idle fallback forbidden "+row.npc+"/"+state.id);
  }
  // Full PCM16 verification is independent of importer rate/frames/channel metadata.
  // The original WAV is PCM16; a lossy/resampled import cannot silently pass here.
  static void RequirePlainProductPcmR1320(Mine2ProductAudioR1211 binding,AudioClip clip){
   int count=checked(clip.samples*clip.channels);var values=new float[count];PlainProductNeedR1320(clip.GetData(values,0),"imported PCM must be decompressed/readable "+binding.id);var pcm=new byte[checked(count*2)];
   for(int i=0;i<count;i++){float value=values[i];PlainProductNeedR1320(!float.IsNaN(value)&&!float.IsInfinity(value)&&value>=-1&&value<=1,"invalid imported PCM sample "+binding.id);int sample=checked((int)Math.Round((double)value*32768,MidpointRounding.ToEven));PlainProductNeedR1320(sample>=short.MinValue&&sample<=short.MaxValue,"imported PCM16 range changed "+binding.id);short s=(short)sample;pcm[i*2]=(byte)(s&255);pcm[i*2+1]=(byte)((s>>8)&255);}
   PlainProductNeedR1320(BelowPlayerResourceIndexV21.Hash(pcm)==binding.decodedPcm16Sha256,"imported original PCM16 bytes changed "+binding.id);
  }
  void LoadPlainProductMineMediaR1320(){
   if(plainProductRouteR1320!=PlainProductResourceIndexR1320.Recycle)return;
   var media=plainProductIndexR1320.Document.mineRows[0];plainProductMineTexturesR1320=new PlainMineTextureR1302[media.textures.Length];plainProductMineSoundsR1320=new PlainMineAudioR1302[media.sounds.Length];
   for(int i=0;i<media.textures.Length;i++){var t=media.textures[i];plainProductMineTexturesR1320[i]=new PlainMineTextureR1302{originalName=t.name,originalMemberSha256=t.originalMemberSha256,unityBottomUpRgbaSha256=t.unityBottomUpRgbaSha256,texture=LoadPlainProductTextureR1320(t.file,0,0,"Original Plain Mine / "+t.name)};}
   for(int i=0;i<media.sounds.Length;i++){var a=media.sounds[i];var clip=plainProductIndexR1320.Audio(a);PlainProductNeedR1320(clip.loadState==AudioDataLoadState.Loaded,"Mine PCM must be loaded during the shared audio pre-read");RequirePlainProductPcmR1320(a,clip);plainProductMineSoundsR1320[i]=new PlainMineAudioR1302{originalName=a.id,originalMemberSha256=a.originalSha256,pcm16Sha256=a.decodedPcm16Sha256,frames=a.samples,channels=a.channels,sampleRate=a.sampleRate,clip=clip};}
  }
 }
 public sealed partial class DemoScene {

  void LoadPlainProductPagedLibraryR1330(PlainPagedLibraryPayloadR1330 payload){
   var row=payload.binding;var raw=payload.document;var pages=new Dictionary<string,Texture2D>(StringComparer.Ordinal);var keys=new Dictionary<string,string>(StringComparer.Ordinal);var pageFactories=new Dictionary<string,Func<Texture2D>>(StringComparer.Ordinal);
   foreach(var page in raw.pages){string key="plain-product-owned-r1320/"+row.id+"/"+page.pageKey;PlainProductNeedR1320(!atlasTextures.ContainsKey(key),"private paged atlas slot occupied "+key);Func<Texture2D> originalPage=()=>{if(!pages.TryGetValue(page.pageKey,out var texture)){texture=LoadPlainProductTextureR1320(page.file,page.width,page.height,"Plain original uniform page / "+row.id+" / "+page.pageKey);PlainProductNeedR1320(PlainPhaseRgbaR1330(texture)==page.rgbaSha256,"original paged atlas RGBA changed");texture.Apply(false,true);atlasTextures.Add(key,texture);pages.Add(page.pageKey,texture);}return texture;};pageFactories.Add(key,originalPage);RegisterPrivateAtlasV53(key,originalPage);plainProductAtlasKeysR1320.Add(key);keys.Add(page.pageKey,key);if(!DeferMonsterPixelCarrierV53(raw.id))originalPage();}
   Library prior;if(libraries.TryGetValue(raw.id,out prior))plainProductPriorLibrariesR1320.Add(raw.id,prior);plainProductLibrariesR1320.Add(raw.id);
   var clips=new List<AnimationData>();foreach(var c in raw.clips){var frames=new FrameData[c.frames.Length];for(int i=0;i<frames.Length;i++){var f=c.frames[i];frames[i]=new FrameData{x=f.rect[0],y=f.rect[1],w=f.rect[2],h=f.rect[3],texture=keys[f.pageKey],hasAnchor=true,anchorX=(float)f.anchor[0],anchorY=(float)f.anchor[1]};}clips.Add(new AnimationData{id=c.id,fps=(float)c.fps,duration=(float)c.durationSeconds,loop=c.loop,frames=frames});}
   var data=new SpriteLibraryData{id=raw.id,texture=keys[raw.pages[0].pageKey],artVersion=raw.artVersion,paintedPoseCount=raw.paintedPoseCount,standingHeight=raw.standingHeight,logicalScale=raw.logicalScale,frameWidth=305,frameHeight=244,anchorX=(float)raw.croppedAnchor[0],anchorY=(float)raw.croppedAnchor[1],clips=clips.ToArray()};var lib=new Library{data=data};libraries[raw.id]=lib;
   if(!DeferPrivateFramesV53(lib,frame=>pageFactories[frame.texture](),plainProductOwnedR1320,false))foreach(var clip in data.clips){var sprites=new Sprite[clip.frames.Length];for(int i=0;i<sprites.Length;i++){var f=clip.frames[i];var texture=atlasTextures[f.texture];var sprite=Sprite.Create(texture,new Rect(f.x,texture.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);plainProductOwnedR1320.Add(sprite);sprite.name=data.id+"/"+clip.id+"/"+i;spriteOffsets[sprite]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);sprites[i]=sprite;}lib.frames.Add(clip.id,sprites);lib.clips.Add(clip.id,clip);}
   var rule=Array.Find(plainProductPlanR1320.rules,r=>r.id==row.npc);foreach(var state in rule.states)PlainProductNeedR1320(lib.frames.ContainsKey(state.clip)&&lib.clips.ContainsKey(state.clip),"original paged state display clip absent "+state.id);
  }

 }

}
