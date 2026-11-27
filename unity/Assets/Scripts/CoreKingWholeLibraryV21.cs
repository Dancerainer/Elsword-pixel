using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class CoreKingWholePinV21 {public string path,sha256;}
 [Serializable] public sealed class CoreKingWholeKeyV21 {
  public string key,file,rgbaSha256;public CoreKingWholePinV21 png;public int width,height;public float[] anchor;
 }
 [Serializable] public sealed class CoreKingWholeFrameV21 {public string atlas,paintedPoseKey,sourcePose;public int[] rect;public float[] anchor;public double sourcePhaseTime,exposureTime;}
 [Serializable] public sealed class CoreKingWholeClipV21 {
  public string id,sourceAnimation;public double fps,durationSeconds,sourceAnimSpeed,sourceMotionDuration;public bool loop;public CoreKingWholeFrameV21[] frames;
 }
 [Serializable] public sealed class CoreKingWholeStateV21 {
  public string state,modelClip,displayClip,sourceAnimation;public double sourceAnimSpeed,sourceMotionDuration,sourceStateDuration,libraryDurationSeconds,fps;
  public int exposures;public bool sourceLoop,holdAfterNativeMotionEnd,stateCanExitBeforeMotionEnd;
 }
 [Serializable] public sealed class CoreKingWholeDocumentV21 {
  public string schema,id,sourceNPC,fixedSourceFacing,sourceRulesSha256;public float logicalScale,expectedModelVisualScale,standingHeight;
  public bool artApproved,animationLibraryComplete,animationLibraryApproved,runtimeAdmitted,nativeEquivalent,automaticSpawnFacingMirror;
  public int paintedPoseCount,sourcePoseSamples,sourceMotionCount,approvedStaticKeyCount;
  public CoreKingWholeKeyV21[] keys;public CoreKingWholeClipV21[] clips;public CoreKingWholeStateV21[] states;
 }
 // Pure clock/lookup code is shared by the display adapter and standalone tests.
 // Reads the existing NPC clock. It never ticks, truncates or writes model time.
 public sealed class CoreKingWholeLookupV21 {
  public readonly CoreKingWholeDocumentV21 document;
  readonly Dictionary<string,CoreKingWholeKeyV21> keys=new Dictionary<string,CoreKingWholeKeyV21>(StringComparer.Ordinal);
  readonly Dictionary<string,CoreKingWholeClipV21> clips=new Dictionary<string,CoreKingWholeClipV21>(StringComparer.Ordinal);
  readonly Dictionary<string,CoreKingWholeStateV21> states=new Dictionary<string,CoreKingWholeStateV21>(StringComparer.Ordinal);
  public static bool Near(double a,double b){return Math.Abs(a-b)<.00001;}
  static void Require(bool value,string message){if(!value)throw new ArgumentException("Core King whole library: "+message);}
  public CoreKingWholeLookupV21(CoreKingWholeDocumentV21 d,bool requireStaticApproval=true){
   Require(d!=null&&d.schema=="core-King-whole-runtime-library-candidate-v21"&&d.id=="nasod-king"&&d.sourceNPC=="NUI_NASOD_KING","identity");
   Require(d.logicalScale==1&&d.expectedModelVisualScale==1&&d.fixedSourceFacing=="+X"&&!d.automaticSpawnFacingMirror,"scale/source facing");
   Require(!d.runtimeAdmitted&&!d.nativeEquivalent&&!d.animationLibraryApproved&&d.animationLibraryComplete,"candidate boundary");
   Require(d.paintedPoseCount==89&&d.sourcePoseSamples==125&&d.sourceMotionCount==19&&d.keys!=null&&d.keys.Length==89&&d.clips!=null&&d.clips.Length==20&&d.states!=null&&d.states.Length==25,"complete cardinality");
   Require(!requireStaticApproval||d.artApproved&&d.approvedStaticKeyCount==89,"all89 exact static reviews required before display configuration");
   document=d;
   foreach(var k in d.keys){Require(k!=null&&!string.IsNullOrEmpty(k.key)&&k.width>0&&k.height>0&&k.anchor!=null&&k.anchor.Length==2&&k.file=="whole/"+k.key+".png"&&k.png!=null&&!string.IsNullOrEmpty(k.png.sha256),"whole key");keys.Add(k.key,k);}
   var motions=new HashSet<string>(StringComparer.Ordinal);
   foreach(var c in d.clips){Require(c!=null&&!string.IsNullOrEmpty(c.id)&&c.fps==60&&c.sourceAnimSpeed>0&&c.durationSeconds>0&&Near(c.durationSeconds,c.sourceMotionDuration/c.sourceAnimSpeed)&&c.frames!=null&&Near(c.frames.Length/c.fps,c.durationSeconds),"native motion clock");clips.Add(c.id,c);motions.Add(c.sourceAnimation);
    for(int i=0;i<c.frames.Length;i++){var f=c.frames[i];CoreKingWholeKeyV21 k;Require(f!=null&&keys.TryGetValue(f.paintedPoseKey,out k),"whole frame key");k=keys[f.paintedPoseKey];Require(f.atlas==k.file&&f.rect!=null&&f.rect.Length==4&&f.rect[0]==0&&f.rect[1]==0&&f.rect[2]==k.width&&f.rect[3]==k.height&&f.anchor!=null&&f.anchor.Length==2&&f.anchor[0]==k.anchor[0]&&f.anchor[1]==k.anchor[1]&&Near(f.exposureTime,i/c.fps),"RGBA rect/anchor/exposure");}
   }
   Require(motions.Count==19,"19 original motions");
   foreach(var s in d.states){CoreKingWholeClipV21 c;Require(s!=null&&clips.TryGetValue(s.displayClip,out c),"exact state clip; no fallback");c=clips[s.displayClip];Require(s.sourceAnimation==c.sourceAnimation&&Near(s.sourceAnimSpeed,c.sourceAnimSpeed)&&s.sourceLoop==c.loop&&Near(s.sourceMotionDuration,c.sourceMotionDuration)&&Near(s.libraryDurationSeconds,c.durationSeconds)&&s.exposures==c.frames.Length&&s.fps==60,"state source clocks");states.Add(s.state,s);}
  }
  public CoreKingWholeStateV21 State(string state){CoreKingWholeStateV21 s;if(!states.TryGetValue(state??"",out s))throw new ArgumentException("Core King source state has no whole display binding: "+state);return s;}
  public CoreKingWholeClipV21 Clip(string id){return clips[id];}
  public CoreKingWholeKeyV21 Key(string id){return keys[id];}
  public static int FrameIndex(CoreKingWholeClipV21 c,double clock){
   if(double.IsNaN(clock)||double.IsInfinity(clock)||clock<0)throw new ArgumentException("Invalid existing King NPC clock");
   if(c.loop)clock%=c.durationSeconds;
   return Math.Min(c.frames.Length-1,Math.Max(0,(int)Math.Min(int.MaxValue,Math.Floor(clock*c.fps))));
  }
  public CoreKingWholeFrameV21 Frame(string state,double npcClock){var s=State(state);var c=clips[s.displayClip];return c.frames[FrameIndex(c,npcClock)];}
 }
}
