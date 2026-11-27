using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class SourceWholePinV21 {public string path,sha256;}
 [Serializable] public sealed class SourceWholeKeyV21 {
  public string key,file,rgbaSha256;public SourceWholePinV21 png;public int width,height;public double[] anchor;
 }
 [Serializable] public sealed class SourceWholePhaseV21 {
  public string sourcePose,paintedWholeKey,reuseBasis;public double sourceTimeOriginal;public float sourcePhaseClock;
  public double[] anchor,wholeDisplayOffsetXY;public bool offsetAlreadyInFrameAnchor,hostMustNotApplyOffsetAgain;
 }
 [Serializable] public sealed class SourceWholeMotionV21 {
  public string id;public float sourceMotionDuration;public double sourceMotionDurationOriginal;public SourceWholePhaseV21[] phases;
 }
 [Serializable] public sealed class SourceWholeStateV21 {
  public string state,modelClip,sourceAnimation,nextState;public float sourceStateDuration,sourceMotionDuration,sourceAnimSpeed;
  public double sourceStateDurationOriginal;public bool sourceLoop,hasSourceShow,sourceShow;
 }
 [Serializable] public sealed class SourceWholeDocumentV21 {
  public string schema,id,sourceNPC,sourceRulesSha256,fixedSourceFacing;
  public float logicalScale,expectedModelVisualScale,sourceVisualSampleEpsilon;
  public int paintedWholeKeys,sourceMotionCount,sourceStateCount,sourcePoseSamples;
  public bool automaticSpawnFacingMirror,sourceOriginAlreadyRegistered,sourceClockChanged,artApproved,sampledPhaseSequenceApproved,animationLibraryComplete,animationLibraryApproved,nativeEquivalent,runtimeAdmitted;
  public bool originalSourceReferenceDiagnostic;public SourceWholePinV21 sourceReferenceEvidence;
  public SourceWholePinV21 artAndSampledPhaseReview;public SourceWholeKeyV21[] keys;public SourceWholeMotionV21[] motions;public SourceWholeStateV21[] states;
 }
 // Immutable snapshots prevent caller mutation of parsed JSON from changing
 // a configured display lookup. This module does not reference Unity or Enemy.
 public sealed class SourceWholeKeyInfoV21 {
  public readonly string Key,File,PngPath,PngSha256,RgbaSha256;public readonly int Width,Height;public readonly double AnchorX,AnchorY;
  internal SourceWholeKeyInfoV21(SourceWholeKeyV21 k){Key=k.key;File=k.file;PngPath=k.png.path;PngSha256=k.png.sha256;RgbaSha256=k.rgbaSha256;Width=k.width;Height=k.height;AnchorX=k.anchor[0];AnchorY=k.anchor[1];}
 }
 public sealed class SourceWholeStateInfoV21 {
  public readonly string State,ModelClip,Motion,NextState;public readonly float StateDuration,MotionDuration,Speed;public readonly bool Loop,HasShow,Show;
  internal SourceWholeStateInfoV21(SourceWholeStateV21 s){State=s.state;ModelClip=s.modelClip;Motion=s.sourceAnimation;NextState=s.nextState;StateDuration=s.sourceStateDuration;MotionDuration=s.sourceMotionDuration;Speed=s.sourceAnimSpeed;Loop=s.sourceLoop;HasShow=s.hasSourceShow;Show=s.sourceShow;}
 }
 public sealed class SourceWholeSelectionV21 {
  public readonly SourceWholeKeyInfoV21 Whole;public readonly SourceWholeStateInfoV21 State;
  public readonly string SourcePose;public readonly int PhaseIndex;public readonly float ExistingNpcClock,SourceMotionClock,SourcePhaseClock;public readonly double AnchorX,AnchorY;
  internal SourceWholeSelectionV21(SourceWholeKeyInfoV21 k,SourceWholeStateInfoV21 s,string pose,int index,float npc,float motion,float at,double x,double y){Whole=k;State=s;SourcePose=pose;PhaseIndex=index;ExistingNpcClock=npc;SourceMotionClock=motion;SourcePhaseClock=at;AnchorX=x;AnchorY=y;}
  public bool ApplySourceShow(bool modelVisible){return State.HasShow?modelVisible&&State.Show:modelVisible;}
 }
 public sealed class SourcePhaseWholeLookupV21 {
  sealed class Phase {internal string pose,key;internal float at;internal double x,y;}
  sealed class Motion {internal float duration;internal Phase[] phases;}
  readonly Dictionary<string,SourceWholeKeyInfoV21> keys=new Dictionary<string,SourceWholeKeyInfoV21>(StringComparer.Ordinal);
  readonly Dictionary<string,SourceWholeStateInfoV21> states=new Dictionary<string,SourceWholeStateInfoV21>(StringComparer.Ordinal);
  readonly Dictionary<string,Motion> motions=new Dictionary<string,Motion>(StringComparer.Ordinal);
  readonly float epsilon;
  public readonly bool OriginalSourceReferenceDiagnosticR718;
  public readonly string Id,SourceNpc,SourceRulesSha256,FixedSourceFacing;public readonly bool AutomaticSpawnFacingMirror;
  public int KeyCount{get{return keys.Count;}}public int StateCount{get{return states.Count;}}public int MotionCount{get{return motions.Count;}}
  static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}static bool Finite(double x){return !double.IsNaN(x)&&!double.IsInfinity(x);}
  static void Need(bool yes,string why){if(!yes)throw new ArgumentException("Source whole phase library: "+why);}
  static bool Hash(string x){if(x==null||x.Length!=64)return false;foreach(char c in x)if(!(c>='0'&&c<='9'||c>='a'&&c<='f'))return false;return true;}
  static bool KeyName(string x){if(string.IsNullOrEmpty(x))return false;foreach(char c in x)if(!(char.IsLetterOrDigit(c)||c=='-'||c=='_'))return false;return true;}
  static bool OriginalReferenceIdentityR718(SourceWholeDocumentV21 d){
   if(d.sourceNPC=="NUI_RAVEN_CLONE_PROTO")return d.sourceRulesSha256=="a44ac614f4a636b1732cee34e2a810e40662f6acadd44e9b6b96cb3227afd588"&&d.id=="raven-clone-proto-original-reference-r1463";
   if(d.sourceRulesSha256!="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159")return false;
   switch(d.sourceNPC){
    case "NUI_NASOD_GUARD":return d.id=="nasod-guard-original-reference-r718";
    case "NUI_NASOD_SHERIFF":return d.id=="nasod-sheriff-original-reference-r718";
    case "NUI_NASOD_MAKER":return d.id=="nasod-maker-original-reference-r718";
    case "NUI_FACTORY_PRESS":return d.id=="factory-press-original-reference-r718";
    case "NUI_FACTORY_PRESS3":return d.id=="factory-press3-original-reference-r718";
    case "NUI_BOX_ALTERA":return d.id=="box-altera-original-reference-r718";
    case "NUI_RAVEN_CLONE":return d.id=="raven-clone-original-reference-r718";
    case "NUI_SMALLGATE_BESMA_AIR_SHIP":return d.id=="smallgate-besma-air-ship-original-reference-r718";
    case "NUI_SMALLGATE_POST_BESMA_AIR_SHIP":return d.id=="smallgate-post-besma-air-ship-original-reference-r718";
    default:return false;
   }
  }
  // Original callers retain the approved-art gate. Reference data uses an explicit opt-in, never a fake review.
  public SourcePhaseWholeLookupV21(SourceWholeDocumentV21 d):this(d,false){}
  public static SourcePhaseWholeLookupV21 CreateOriginalReferenceDiagnosticR718(SourceWholeDocumentV21 d){return new SourcePhaseWholeLookupV21(d,true);}
  SourcePhaseWholeLookupV21(SourceWholeDocumentV21 d,bool originalReference){
   Need(d!=null&&(originalReference?d.schema=="source-reference-diagnostic-library-r718-v1":d.schema=="source-phase-whole-library-candidate-v21-r1")&&!string.IsNullOrEmpty(d.id)&&!string.IsNullOrEmpty(d.sourceNPC),"identity/schema");
   Need(d.logicalScale==1&&d.expectedModelVisualScale==1&&d.sourceOriginAlreadyRegistered&&!d.sourceClockChanged&&!string.IsNullOrEmpty(d.fixedSourceFacing),"registered source scale/facing/clocks");
   if(originalReference)Need(d.originalSourceReferenceDiagnostic&&OriginalReferenceIdentityR718(d)&&!d.artApproved&&!d.sampledPhaseSequenceApproved&&!d.animationLibraryComplete&&!d.animationLibraryApproved&&!d.runtimeAdmitted&&!d.nativeEquivalent&&!d.automaticSpawnFacingMirror&&d.artAndSampledPhaseReview==null&&d.sourceReferenceEvidence!=null&&!string.IsNullOrEmpty(d.sourceReferenceEvidence.path)&&Hash(d.sourceReferenceEvidence.sha256),"explicit original reference evidence; no art/native/runtime approval");
   else Need(!d.originalSourceReferenceDiagnostic&&d.artApproved&&d.sampledPhaseSequenceApproved&&d.animationLibraryComplete&&!d.animationLibraryApproved&&!d.runtimeAdmitted&&!d.nativeEquivalent&&d.artAndSampledPhaseReview!=null&&Hash(d.artAndSampledPhaseReview.sha256)&&Hash(d.sourceRulesSha256),"external art review and unadmitted executable boundary");
   OriginalSourceReferenceDiagnosticR718=originalReference;
   Need(d.keys!=null&&d.keys.Length>0&&d.keys.Length==d.paintedWholeKeys&&d.motions!=null&&d.motions.Length>0&&d.motions.Length==d.sourceMotionCount&&d.states!=null&&d.states.Length>0&&d.states.Length==d.sourceStateCount&&d.sourcePoseSamples>0,"declared cardinality");
   Need(d.sourceVisualSampleEpsilon==.000001f,"same source diagnostic visual-sample epsilon");
   Id=d.id;SourceNpc=d.sourceNPC;SourceRulesSha256=d.sourceRulesSha256;FixedSourceFacing=d.fixedSourceFacing;AutomaticSpawnFacingMirror=d.automaticSpawnFacingMirror;epsilon=d.sourceVisualSampleEpsilon;
   foreach(var k in d.keys){Need(k!=null&&KeyName(k.key)&&k.file=="whole/"+k.key+".png"&&k.png!=null&&!string.IsNullOrEmpty(k.png.path)&&Hash(k.png.sha256)&&Hash(k.rgbaSha256)&&k.width>0&&k.height>0&&k.width<=8192&&k.height<=8192&&k.anchor!=null&&k.anchor.Length==2&&Finite(k.anchor[0])&&Finite(k.anchor[1]),"complete whole key/relative PNG/anchor");keys.Add(k.key,new SourceWholeKeyInfoV21(k));}
   int phaseCount=0;
   foreach(var m in d.motions){
    Need(m!=null&&!string.IsNullOrEmpty(m.id)&&Finite(m.sourceMotionDuration)&&m.sourceMotionDuration>0&&Finite(m.sourceMotionDurationOriginal)&&m.sourceMotionDurationOriginal>0&&(float)m.sourceMotionDurationOriginal==m.sourceMotionDuration&&m.phases!=null&&m.phases.Length>0,"source motion duration/phases");
    var copy=new Motion{duration=m.sourceMotionDuration,phases=new Phase[m.phases.Length]};float previous=-1;
    for(int i=0;i<m.phases.Length;i++){
     var p=m.phases[i];Need(p!=null&&!string.IsNullOrEmpty(p.sourcePose)&&keys.ContainsKey(p.paintedWholeKey)&&Finite(p.sourceTimeOriginal)&&p.sourceTimeOriginal>=0&&Finite(p.sourcePhaseClock)&&p.sourcePhaseClock==(float)p.sourceTimeOriginal&&p.sourcePhaseClock>=0&&p.sourcePhaseClock<=m.sourceMotionDuration&&p.sourcePhaseClock>previous&&(i!=0||p.sourcePhaseClock==0),"source phase order/clock/key; no idle fallback");
     Need(p.anchor!=null&&p.anchor.Length==2&&p.wholeDisplayOffsetXY!=null&&p.wholeDisplayOffsetXY.Length==2&&Finite(p.anchor[0])&&Finite(p.anchor[1])&&Finite(p.wholeDisplayOffsetXY[0])&&Finite(p.wholeDisplayOffsetXY[1])&&p.offsetAlreadyInFrameAnchor&&p.hostMustNotApplyOffsetAgain,"whole source offset must be in frame anchor once");
     var key=keys[p.paintedWholeKey];Need(Math.Abs(p.anchor[0]-key.AnchorX-p.wholeDisplayOffsetXY[0])<.000000001&&Math.Abs(p.anchor[1]-key.AnchorY-p.wholeDisplayOffsetXY[1])<.000000001,"effective frame anchor/whole offset mismatch");
     copy.phases[i]=new Phase{pose=p.sourcePose,key=p.paintedWholeKey,at=p.sourcePhaseClock,x=p.anchor[0],y=p.anchor[1]};previous=p.sourcePhaseClock;phaseCount++;
    }motions.Add(m.id,copy);
   }Need(phaseCount==d.sourcePoseSamples,"source sample count; repeated wholes are not new drawings");
   foreach(var s in d.states){Need(s!=null&&!string.IsNullOrEmpty(s.state)&&!string.IsNullOrEmpty(s.modelClip)&&motions.ContainsKey(s.sourceAnimation)&&Finite(s.sourceStateDuration)&&s.sourceStateDuration>=0&&Finite(s.sourceStateDurationOriginal)&&(float)s.sourceStateDurationOriginal==s.sourceStateDuration&&Finite(s.sourceAnimSpeed)&&s.sourceAnimSpeed>0&&s.sourceMotionDuration==motions[s.sourceAnimation].duration,"exact state/source motion/independent expiry binding");states.Add(s.state,new SourceWholeStateInfoV21(s));}
  }
  public SourceWholeKeyInfoV21[] Keys(){var result=new SourceWholeKeyInfoV21[keys.Count];keys.Values.CopyTo(result,0);return result;}
  public SourceWholeStateInfoV21 State(string state){SourceWholeStateInfoV21 result;if(!states.TryGetValue(state??"",out result))throw new ArgumentException("Source whole display has no exact state binding: "+state);return result;}
  public SourceWholeSelectionV21 Select(string state,float npcTime){
   Need(Finite(npcTime)&&npcTime>=0,"invalid existing NPC clock");var s=State(state);var m=motions[s.Motion];
   // Match the existing source-view float clock and causal sample epsilon.
   // State duration is deliberately not used to expire/enter any model state.
   float clock=npcTime*s.Speed;Need(Finite(clock),"source playback clock overflow");
   if(s.Loop)clock%=m.duration;else clock=Math.Min(clock,m.duration);
   int index=0;for(int i=1;i<m.phases.Length;i++)if(m.phases[i].at<=clock+epsilon)index=i;else break;
   var p=m.phases[index];return new SourceWholeSelectionV21(keys[p.key],s,p.pose,index,npcTime,clock,p.at,p.x,p.y);
  }
  // The existing Press sourcePoseTime is already its real owned raw-motion
  // clock. Keep existing npcTime separately; never multiply/re-loop this input.
  public SourceWholeSelectionV21 SelectOriginalReferenceSourceClockR718(string state,float existingNpcTime,float actualSourceClock){
   Need(OriginalSourceReferenceDiagnosticR718&&(SourceNpc=="NUI_FACTORY_PRESS"||SourceNpc=="NUI_FACTORY_PRESS3"),"direct source clock is only the two original-reference Press actors");
   Need(Finite(existingNpcTime)&&existingNpcTime>=0&&Finite(actualSourceClock)&&actualSourceClock>=0,"invalid existing Press source clock");var s=State(state);var m=motions[s.Motion];Need(s.Speed==1&&actualSourceClock<=m.duration,"Press actual raw source phase outside motion");
   int index=0;for(int i=1;i<m.phases.Length;i++)if(m.phases[i].at<=actualSourceClock+epsilon)index=i;else break;
   var p=m.phases[index];return new SourceWholeSelectionV21(keys[p.key],s,p.pose,index,existingNpcTime,actualSourceClock,p.at,p.x,p.y);
  }
  public bool ApplySourceShow(string state,bool modelVisible){var s=State(state);return s.HasShow?modelVisible&&s.Show:modelVisible;}
 }
}
