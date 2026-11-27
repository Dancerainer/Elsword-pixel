using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class CoreKingCachedWholeV21 {public Sprite sprite;public Texture2D texture;public long used;}
  CoreKingWholeLookupV21 coreKingWholeLookupV21;
  string coreKingWholeDirectoryV21;
  long coreKingWholeUseV21;
  readonly Dictionary<string,CoreKingCachedWholeV21> coreKingWholeCacheV21=new Dictionary<string,CoreKingCachedWholeV21>(StringComparer.Ordinal);
  public int CoreKingWholeResidentKeyCountV21 {get{return coreKingWholeCacheV21.Count;}}
  public long CoreKingWholeResidentRgbaBytesV21 {get{long n=0;foreach(var p in coreKingWholeCacheV21.Values)n+=(long)p.texture.width*p.texture.height*4;return n;}}
  public int CoreKingWholeTextureLoadsV21 {get;private set;}
  public int CoreKingWholeDrawFramesV21 {get;private set;}
  public int CoreKingWholeTextureRetireRequestsV21 {get;private set;}
  public bool CoreKingWholeConfiguredV21 {get{return coreKingWholeLookupV21!=null;}}
  public string CoreKingWholeLastKeyV21 {get;private set;}
  public int CoreKingWholeLastFrameIndexV21 {get;private set;}
  public string CoreKingWholeLastStateV21 {get;private set;}
  public string CoreKingWholeLastDisplayClipV21 {get;private set;}
  public float CoreKingWholeLastNpcClockV21 {get;private set;}
  static string CoreKingWholeShaV21(byte[] bytes){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  // Explicit candidate activation after the existing Core loader has supplied
  // the same active source rules. It admits no combat or source-clock changes.
  public void ConfigureCoreKingWholeLibraryV21(string animationsPath,string expectedSha256,NpcDefinition[] activeSourceRules){
   var bytes=ReadCoreProductOrEditorFileR1147(animationsPath,expectedSha256);if(CoreKingWholeShaV21(bytes)!=expectedSha256)throw new ArgumentException("King whole document pin changed");
   var d=JsonUtility.FromJson<CoreKingWholeDocumentV21>(System.Text.Encoding.UTF8.GetString(bytes));
   var lookup=new CoreKingWholeLookupV21(d);
   var profile=Array.Find(activeSourceRules??new NpcDefinition[0],p=>p.id==d.sourceNPC);
   if(profile==null||profile.kind!=d.id||profile.states.Length!=25)throw new ArgumentException("King active source profile missing");
   foreach(var actual in profile.states){var expected=lookup.State(actual.id);if(actual.clip!=expected.modelClip||actual.sourceAnimation!=expected.sourceAnimation||!CoreKingWholeLookupV21.Near(actual.sourceAnimSpeed,expected.sourceAnimSpeed)||!CoreKingWholeLookupV21.Near(actual.duration,expected.sourceStateDuration)||actual.loop!=expected.sourceLoop)throw new ArgumentException("King active source clock/state changed: "+actual.id);}
   var directory=CoreProductResourcesActiveR1147?CoreProductDirectoryR1147(animationsPath):Path.GetDirectoryName(Path.GetFullPath(animationsPath));
   // Verify every encoded PNG hash without decoding/eager texture allocation.
   foreach(var key in d.keys){var p=CoreKingWholePathV21(directory,key.file);if(CoreKingWholeShaV21(ReadCoreProductOrEditorFileR1147(p,key.png.sha256))!=key.png.sha256)throw new ArgumentException("King whole PNG pin changed: "+key.key);}
   ClearCoreKingWholeLibraryV21();coreKingWholeDirectoryV21=directory;coreKingWholeLookupV21=lookup;
   CoreKingWholeTextureLoadsV21=CoreKingWholeDrawFramesV21=0;CoreKingWholeLastKeyV21="";CoreKingWholeLastFrameIndexV21=-1;CoreKingWholeLastStateV21=CoreKingWholeLastDisplayClipV21="";CoreKingWholeLastNpcClockV21=0;
  }
  static string CoreKingWholePathV21(string root,string relative){if(CoreProductResourcesActiveR1147)return CoreProductRelativeFileR1147(root,relative);var path=Path.GetFullPath(Path.Combine(root,relative));if(!path.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new ArgumentException("King whole path escaped package");return path;}
  static void CoreKingWholeDestroyV21(UnityEngine.Object value){if(Application.isPlaying)Destroy(value);else DestroyImmediate(value);}
  void ReleaseCoreKingWholeV21(CoreKingCachedWholeV21 value){spriteOffsets.Remove(value.sprite);CoreKingWholeTextureRetireRequestsV21++;CoreKingWholeDestroyV21(value.sprite);CoreKingWholeDestroyV21(value.texture);}
  public void ClearCoreKingWholeLibraryV21(){ClearCoreInkV45("nasod-king");foreach(var value in coreKingWholeCacheV21.Values)ReleaseCoreKingWholeV21(value);coreKingWholeCacheV21.Clear();coreKingWholeLookupV21=null;coreKingWholeDirectoryV21=null;}
  Sprite CoreKingWholeSpriteV21(string id){
   CoreKingCachedWholeV21 old;if(coreKingWholeCacheV21.TryGetValue(id,out old)){old.used=++coreKingWholeUseV21;return old.sprite;}
   if(coreKingWholeCacheV21.Count==2){string victim=null;long oldest=long.MaxValue;foreach(var entry in coreKingWholeCacheV21)if(entry.Value.used<oldest){victim=entry.Key;oldest=entry.Value.used;}ReleaseCoreKingWholeV21(coreKingWholeCacheV21[victim]);coreKingWholeCacheV21.Remove(victim);}
   var key=coreKingWholeLookupV21.Key(id);var bytes=ReadCoreProductOrEditorFileR1147(CoreKingWholePathV21(coreKingWholeDirectoryV21,key.file),key.png.sha256);if(CoreKingWholeShaV21(bytes)!=key.png.sha256)throw new ArgumentException("King whole PNG changed after configure");
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="King whole "+id,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   if(!ImageConversion.LoadImage(texture,bytes,true)||texture.width!=key.width||texture.height!=key.height){CoreKingWholeDestroyV21(texture);throw new ArgumentException("King whole PNG dimensions/decode");}
   var sprite=Sprite.Create(texture,new Rect(0,0,key.width,key.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="nasod-king/whole/"+id;
   spriteOffsets.Add(sprite,new Vector2(key.width*.5f-key.anchor[0],key.anchor[1]-key.height*.5f));
   coreKingWholeCacheV21.Add(id,new CoreKingCachedWholeV21{sprite=sprite,texture=texture,used=++coreKingWholeUseV21});CoreKingWholeTextureLoadsV21++;return sprite;
  }
  bool CoreKingWholeActorV21(Enemy e){return coreKingWholeLookupV21!=null&&e!=null&&e.sourceNPC=="NUI_NASOD_KING";}
  // Only the initial invisible birth has no picture. An interrupted actor already
  // has a verified, per-instance next DAMAGE/DYING state. Show its first pose
  // through hit-stop; the next simulation tick still owns the actual transition.
  bool TryCoreWholeDisplayV44(Enemy e, SpriteRenderer renderer, out string state, out float time){
   state=e.npcState;time=e.npcTime;
   if(!string.IsNullOrEmpty(state))return true;
   time=0;
   if(game!=null && game.TryCoreHardHitDisplayPendingV39(e,out state))return true;
   if(game==null || !game.TryCoreHardSourceInitializationPendingV32(e,out state))throw new ArgumentException("Unknown Core whole handoff: "+e.sourceNPC);
   renderer.enabled=false;renderer.sprite=null;return false;
  }
  bool TryPoseCoreKingWholeV21(SpriteRenderer renderer,Enemy e){
   if(!CoreKingWholeActorV21(e))return false;
   if(e.kind!="nasod-king"||e.visualScale!=1)throw new ArgumentException("King source full world scale changed");
   if(!TryCoreWholeDisplayV44(e,renderer,out var displayState,out var displayTime))return true;
   // npcTime is the existing model's state clock, already advanced by the
   // original runner. Speed is represented exactly once by the clip duration.
   var state=coreKingWholeLookupV21.State(displayState);var clip=coreKingWholeLookupV21.Clip(state.displayClip);int index=CoreKingWholeLookupV21.FrameIndex(clip,displayTime);var frame=clip.frames[index];renderer.enabled=true;renderer.sprite=CoreKingWholeSpriteV21(frame.paintedPoseKey);renderer.flipY=false;renderer.transform.localRotation=Quaternion.identity;
   SelectCoreInkV45(e.kind,clip.sourceAnimation,(float)(displayTime*clip.sourceAnimSpeed),frame.paintedPoseKey,renderer,out _);
   float sourceClock=(float)(displayTime*clip.sourceAnimSpeed);if(clip.loop&&clip.sourceMotionDuration>0)sourceClock=Mathf.Repeat(sourceClock,(float)clip.sourceMotionDuration);
   RecordMonsterPixelSourceV53(e,renderer,"Core/King",displayState,clip.sourceAnimation,sourceClock,1);
   CoreKingWholeLastKeyV21=frame.paintedPoseKey;CoreKingWholeLastFrameIndexV21=index;CoreKingWholeLastStateV21=displayState;CoreKingWholeLastDisplayClipV21=state.displayClip;CoreKingWholeLastNpcClockV21=displayTime;CoreKingWholeDrawFramesV21++;return true;
  }
  // The normal RenderActors hook and the private GPU fixture call exactly this
  // same production helper. It reads the existing actor/state clock and writes
  // only the renderer and this display adapter's cache/counters.
  public bool RenderCoreKingWholeCandidateV21(SpriteRenderer renderer,Enemy e,Vector2 actorRoot){
   if(!TryPoseCoreKingWholeV21(renderer,e))return false;
   renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
   Vector2 position=actorRoot+SpriteOffset(renderer);renderer.transform.localPosition=new Vector3(position.x,position.y,0);return true;
  }
 }
}
