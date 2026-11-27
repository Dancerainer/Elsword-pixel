using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreRavenBWholeDocumentShaV21="d7fd74d945b85734059c5ba313325921f6f2765d26c796e050cd20921c562029";
 const string CoreRavenBWholeReviewShaV21="c4f3033d1e3d8e95b9ca277823a2191316697d362c231f028dd85cb608e5262b";
 const string CoreRavenBWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 [Serializable] sealed class CoreRavenBClockStateV21 {public string id;public float sourceMotionDuration;}
 [Serializable] sealed class CoreRavenBClockProfileV21 {public string id;public CoreRavenBClockStateV21[] states;}
 [Serializable] sealed class CoreRavenBClockRulesV21 {public CoreRavenBClockProfileV21[] definitions;}
 [Serializable] sealed class CoreRavenBDocumentClockStateV21 {public string state;public float sourceModelMotionDuration,sourceMotionDuration;}
 [Serializable] sealed class CoreRavenBDocumentClocksV21 {public CoreRavenBDocumentClockStateV21[] states;}
 SourcePhaseWholeLookupV21 coreRavenBWholeLookupV21;
 readonly DeferredWholeSpritesV53 coreRavenBWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreRavenBWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreRavenBWholeConfiguredV21{get{return coreRavenBWholeLookupV21!=null;}}
 public int CoreRavenBWholeResidentKeysV21{get{return coreRavenBWholeSpritesV21.ResidentCount;}}
 public long CoreRavenBWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreRavenBWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreRavenBWholeDrawsV21{get;private set;}public int CoreRavenBWholeMissingStateHoldsV21{get;private set;}
 public int CoreRavenBSourceShowApplicationsV21{get;private set;}public int CoreRavenBSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreRavenBWholeLastKeyV21{get;private set;}public string CoreRavenBWholeLastStateV21{get;private set;}public string CoreRavenBWholeLastSourcePoseV21{get;private set;}
 public float CoreRavenBWholeLastNpcClockV21{get;private set;}public float CoreRavenBWholeLastMotionClockV21{get;private set;}
 public double CoreRavenBWholeLastAnchorXV21{get;private set;}public double CoreRavenBWholeLastAnchorYV21{get;private set;}
 public bool CoreRavenBSourceShowLastAuthoredV21{get;private set;}public bool CoreRavenBSourceShowLastValueV21{get;private set;}
 public bool CoreRavenBSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreRavenBSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreRavenBWholeEmitsGameplayCallbacksV21{get{return false;}}

 static PreviewFile CoreRavenBPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreRavenBWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreRavenBWholeDocumentV21RuntimeR1147(documentPin,reviewPin,sourceRulesPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreRavenBWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreRavenBWholeReviewShaV21,"Core RavenB exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(documentPin));var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="raven-clone-b"&&lookup.SourceNpc=="NUI_RAVEN_CLONE_B"&&lookup.SourceRulesSha256==CoreRavenBWholeRulesShaV21&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==159&&lookup.MotionCount==37&&lookup.StateCount==48&&d.sourcePoseSamples==188&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core RavenB exact159whole/37motion/48state/188finite-source-clock phases");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==48,"Core RavenB original active profile required");
  PreviewRequire(sourceRulesPin!=null&&sourceRulesPin.sha256==CoreRavenBWholeRulesShaV21,"Core RavenB exact original motion-clock rules pin required");var clockRules=CoreEntryJsonV21<CoreRavenBClockRulesV21>(sourceRulesPin);var clockProfile=Array.Find(clockRules.definitions??new CoreRavenBClockProfileV21[0],p=>p.id==lookup.SourceNpc);PreviewRequire(clockProfile!=null&&clockProfile.states!=null&&clockProfile.states.Length==48,"Core RavenB original raw48 model-clock fields required");
  var documentClocks=CoreEntryJsonV21<CoreRavenBDocumentClocksV21>(documentPin);PreviewRequire(documentClocks.states!=null&&documentClocks.states.Length==48,"Core Raven B both original model expiry and geometry clocks required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);var originalClock=Array.Find(clockProfile.states,p=>p.id==actual.id);var documentClock=Array.Find(documentClocks.states,p=>p.state==actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&originalClock!=null&&documentClock!=null&&originalClock.sourceMotionDuration==documentClock.sourceModelMotionDuration&&documentClock.sourceMotionDuration==expected.MotionDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Core RavenB exact original state/clock/no authored SHOW changed "+actual.id);PreviewRequire(actual.id=="RAVEN_CLONE_B_SPECIAL_ATTACK_C"?expected.MotionDuration==1.5666667f&&originalClock.sourceMotionDuration==1.8431373f&&expected.Speed==.85f:originalClock.sourceMotionDuration==expected.MotionDuration,"Core Raven B original C geometry/model clock domain changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core RavenB relative PNG path escaped/mismatched package");PreviewRequire(key.Width>0&&key.Height>0&&key.Width<=220&&key.Height<=157,"Core RavenB complete native PNG canvas dimensions changed; atlas384 cells are not sprite rects");CoreEntryReadV21(CoreRavenBPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreRavenBWholeV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(coreRavenBWholeLookupV21==null&&coreRavenBWholeSpritesV21.Count==0&&coreRavenBWholeTexturesV21.Count==0,"Core RavenB display owner must be empty before configure");var lookup=ValidateCoreRavenBWholeDocumentV21(documentPin,reviewPin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreRavenBWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("raven-clone-b"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CoreRavenBPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved RavenB whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreRavenBWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core RavenB approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved RavenB whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }coreRavenBWholeLookupV21=lookup;CoreRavenBWholeDrawsV21=CoreRavenBWholeMissingStateHoldsV21=CoreRavenBSourceShowApplicationsV21=CoreRavenBSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreRavenBSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_RAVEN_CLONE_B"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreRavenBWholeLookupV21!=null,"Core RavenB visibility requires configured exact library");var state=coreRavenBWholeLookupV21.State(e.npcState);CoreRavenBSourceShowLastAuthoredV21=state.HasShow;CoreRavenBSourceShowLastValueV21=state.Show;CoreRavenBSourceShowLastVisibleBeforeV21=visible;visible=coreRavenBWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreRavenBSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreRavenBSourceShowApplicationsV21++;else CoreRavenBSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreRavenBWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_RAVEN_CLONE_B")return false;
  PreviewRequire(coreRavenBWholeLookupV21!=null,"Core RavenB render requires configured exact library");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreRavenBWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="raven-clone-b"&&e.visualScale==1,"Core RavenB original source actor identity/scale");var selected=coreRavenBWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/RavenB",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=coreRavenBWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Selected signed whole anchor includes the Root-approved finite source registration.
  // No SpriteOffset/floor/whole offset is added again; no actor/model field is mutated.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CoreRavenBWholeLastKeyV21=key.Key;CoreRavenBWholeLastStateV21=displayState;CoreRavenBWholeLastSourcePoseV21=selected.SourcePose;CoreRavenBWholeLastNpcClockV21=displayTime;CoreRavenBWholeLastMotionClockV21=selected.SourceMotionClock;CoreRavenBWholeLastAnchorXV21=selected.AnchorX;CoreRavenBWholeLastAnchorYV21=selected.AnchorY;CoreRavenBWholeDrawsV21++;return true;
 }
 void DisposeCoreRavenBWholeV21(){
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  coreRavenBWholeSpritesV21.Clear();coreRavenBWholeTexturesV21.Clear();coreRavenBWholeLookupV21=null;
 }
}}

