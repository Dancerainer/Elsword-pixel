using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  static int BattleProductPngIntR1325(byte[] b,int at){return checked((int)((uint)b[at]<<24|(uint)b[at+1]<<16|(uint)b[at+2]<<8|b[at+3]));}
  Texture2D LoadBattleProductTextureR1325(byte[] bytes,int width,int height,bool linear,string label){
   BattleProductNeedR1325(bytes.Length>=24&&bytes[0]==137&&bytes[1]==80&&bytes[2]==78&&bytes[3]==71&&bytes[12]==73&&bytes[13]==72&&bytes[14]==68&&bytes[15]==82,"original PNG header absent "+label);
   int w=BattleProductPngIntR1325(bytes,16),h=BattleProductPngIntR1325(bytes,20);BattleProductNeedR1325(w>0&&h>0&&w<=SystemInfo.maxTextureSize&&h<=SystemInfo.maxTextureSize&&(width==0||width==w)&&(height==0||height==h),"original PNG cannot be preserved on this device "+label);
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,linear){name=label,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};battleProductOwnedR1325.Add(texture);
   BattleProductNeedR1325(ImageConversion.LoadImage(texture,bytes,false)&&texture.width==w&&texture.height==h,"original PNG decode/dimensions changed "+label);return texture;
  }
  void SelectBattleRavenLethalPoseV36(Enemy actor,ref string alias,ref float clock) {
   if(!BattleProductSessionActiveR1325 || !game.TryBattleRavenLethalPoseV36(actor,out var state,out var ownedAlias,out var sourceClock))return;
   if(!libraries.TryGetValue(actor.kind,out var library) || !library.frames.ContainsKey(ownedAlias))
    throw new InvalidOperationException("Missing original owned Battle Raven death "+state+"/"+ownedAlias);
   alias=ownedAlias;clock=sourceClock;
  }
  void LoadBattleProductLibraryR1325(BattleProductLibraryR1282 raw,byte[] atlas){
   string key="battle-product-owned-r1325/"+raw.id;BattleProductNeedR1325(!atlasTextures.ContainsKey(key),"private atlas slot already occupied "+key);
   Texture2D texture=null;Func<Texture2D> originalPage=()=>{if(!texture){texture=LoadBattleProductTextureR1325(atlas,0,0,false,"Battle original atlas / "+raw.id);atlasTextures.Add(key,texture);}return texture;};
   RegisterPrivateAtlasV53(key,originalPage);battleProductAtlasKeysR1325.Add(key);if(!DeferMonsterPixelCarrierV53(raw.id))originalPage();
   Library prior;if(libraries.TryGetValue(raw.id,out prior))battleProductPriorLibrariesR1325.Add(raw.id,prior);battleProductLibrariesR1325.Add(raw.id);
   var clips=new List<AnimationData>();foreach(var c in raw.clips){var frames=new FrameData[c.frames.Length];for(int i=0;i<frames.Length;i++){var f=c.frames[i];frames[i]=new FrameData{x=f.rect[0],y=f.rect[1],w=f.rect[2],h=f.rect[3],hasAnchor=true,anchorX=f.anchor[0],anchorY=f.anchor[1]};}clips.Add(new AnimationData{id=c.id,fps=c.fps,duration=c.durationSeconds,loop=c.loop,frames=frames});}
   var data=new SpriteLibraryData{id=raw.id,texture=key,artVersion=raw.artVersion,paintedPoseCount=raw.paintedPoseCount,standingHeight=raw.standingHeight,logicalScale=raw.logicalScale,frameWidth=raw.atlas.frameWidth,frameHeight=raw.atlas.frameHeight,anchorX=raw.atlas.groundAnchor[0],anchorY=raw.atlas.groundAnchor[1],clips=clips.ToArray()};var lib=new Library{data=data};libraries[raw.id]=lib;
   if(!DeferPrivateFramesV53(lib,frame=>originalPage(),battleProductOwnedR1325,true))foreach(var clip in data.clips){if(BuildPrivateMotionV45(lib,clip,battleProductOwnedR1325))continue;var sprites=new Sprite[clip.frames.Length];for(int i=0;i<sprites.Length;i++){var f=clip.frames[i];var sprite=Sprite.Create(texture,new Rect(f.x,texture.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);battleProductOwnedR1325.Add(sprite);sprite.name=data.id+"/"+clip.id+"/"+i;spriteOffsets[sprite]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);sprites[i]=sprite;}lib.frames.Add(clip.id,sprites);lib.clips.Add(clip.id,clip);}
   var rule=Array.Find(battleProductPlanR1325.Rules.definitions,r=>r.id==raw.sourceNPC);foreach(var state in rule.states)BattleProductNeedR1325(lib.frames.ContainsKey(state.clip)&&lib.clips.ContainsKey(state.clip),"original state clip absent; idle fallback forbidden "+raw.sourceNPC+"/"+state.id);
  }
  // Full PCM16 verification is independent of importer rate/frames/channel metadata.
  // The original WAV is PCM16; a lossy/resampled import cannot silently pass here.
  void RequireBattleProductPcmR1325(BattleProductPcmRowR1325 binding,AudioClip clip){
   BattleProductNeedR1325(clip.samples==binding.frames&&clip.channels==binding.channels&&clip.frequency==binding.sampleRate,"imported original PCM metadata changed "+binding.id);int count=checked(clip.samples*clip.channels);var values=new float[count];BattleProductNeedR1325(clip.GetData(values,0),"imported PCM must be decompressed/readable "+binding.id);var pcm=new byte[checked(count*2)];
   for(int i=0;i<count;i++){float value=values[i];BattleProductNeedR1325(!float.IsNaN(value)&&!float.IsInfinity(value)&&value>=-1&&value<=1,"invalid imported PCM sample "+binding.id);int sample=checked((int)Math.Round((double)value*32768,MidpointRounding.ToEven));BattleProductNeedR1325(sample>=short.MinValue&&sample<=short.MaxValue,"imported PCM16 range changed "+binding.id);short s=(short)sample;pcm[i*2]=(byte)(s&255);pcm[i*2+1]=(byte)((s>>8)&255);}
   BattleProductNeedR1325(BattleProductDataPlanR1282.Hash(pcm)==binding.pcmSha256,"full imported original PCM16 bytes changed "+binding.id);
  }
 }
}
