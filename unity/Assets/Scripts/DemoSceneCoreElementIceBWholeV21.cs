using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreElementIceBWholeDocumentShaV21="71d4f55e3b9a11d0bf6812120e09679ef2a8c61353b40a9e302debdcd817ee95";
 const string CoreElementIceBWholeReviewShaV21="ba2da70d567854feff0ef9924072ed515dca25130ba4906830d9940e6158be4a";
 const string CoreElementIceBWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 [Serializable] sealed class CoreElementIceBClockStateV21 {public string id;public float sourceMotionDuration;}
 [Serializable] sealed class CoreElementIceBClockProfileV21 {public string id;public CoreElementIceBClockStateV21[] states;}
 [Serializable] sealed class CoreElementIceBClockRulesV21 {public CoreElementIceBClockProfileV21[] definitions;}
 [Serializable] sealed class CoreElementIceBDocumentClockStateV21 {public string state;public float sourceModelMotionDuration,sourceMotionDuration;}
 [Serializable] sealed class CoreElementIceBDocumentClocksV21 {public CoreElementIceBDocumentClockStateV21[] states;}
 SourcePhaseWholeLookupV21 coreElementIceBWholeLookupV21;
 readonly DeferredWholeSpritesV53 coreElementIceBWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreElementIceBWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreElementIceBWholeConfiguredV21{get{return coreElementIceBWholeLookupV21!=null;}}
 public int CoreElementIceBWholeResidentKeysV21{get{return coreElementIceBWholeSpritesV21.ResidentCount;}}
 public long CoreElementIceBWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreElementIceBWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreElementIceBWholeDrawsV21{get;private set;}public int CoreElementIceBWholeMissingStateHoldsV21{get;private set;}
 public int CoreElementIceBSourceShowApplicationsV21{get;private set;}public int CoreElementIceBSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreElementIceBWholeLastKeyV21{get;private set;}public string CoreElementIceBWholeLastStateV21{get;private set;}public string CoreElementIceBWholeLastSourcePoseV21{get;private set;}
 public float CoreElementIceBWholeLastNpcClockV21{get;private set;}public float CoreElementIceBWholeLastMotionClockV21{get;private set;}
 public double CoreElementIceBWholeLastAnchorXV21{get;private set;}public double CoreElementIceBWholeLastAnchorYV21{get;private set;}
 public bool CoreElementIceBSourceShowLastAuthoredV21{get;private set;}public bool CoreElementIceBSourceShowLastValueV21{get;private set;}
 public bool CoreElementIceBSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreElementIceBSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreElementIceBWholeEmitsGameplayCallbacksV21{get{return false;}}

 static PreviewFile CoreElementIceBPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreElementIceBWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreElementIceBWholeDocumentV21RuntimeR1147(documentPin,reviewPin,sourceRulesPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreElementIceBWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreElementIceBWholeReviewShaV21,"Core IceB exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(documentPin));var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="nasod-element-ice-b"&&lookup.SourceNpc=="NUI_NASOD_ELEMENT_ICE_B"&&lookup.SourceRulesSha256==CoreElementIceBWholeRulesShaV21&&lookup.FixedSourceFacing=="original-source-orientation"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==153&&lookup.MotionCount==28&&lookup.StateCount==33&&d.sourcePoseSamples==441&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core IceB exact153whole/28motion/33state/441finite-source-clock phases");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==33,"Core IceB original active profile required");
  PreviewRequire(sourceRulesPin!=null&&sourceRulesPin.sha256==CoreElementIceBWholeRulesShaV21,"Core IceB exact original motion-clock rules pin required");var clockRules=CoreEntryJsonV21<CoreElementIceBClockRulesV21>(sourceRulesPin);var clockProfile=Array.Find(clockRules.definitions??new CoreElementIceBClockProfileV21[0],p=>p.id==lookup.SourceNpc);PreviewRequire(clockProfile!=null&&clockProfile.states!=null&&clockProfile.states.Length==33,"Core IceB original raw33 model-clock fields required");
  var documentClocks=CoreEntryJsonV21<CoreElementIceBDocumentClocksV21>(documentPin);PreviewRequire(documentClocks.states!=null&&documentClocks.states.Length==33,"Core IceB both original model expiry and geometry clocks required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);var originalClock=Array.Find(clockProfile.states,p=>p.id==actual.id);var documentClock=Array.Find(documentClocks.states,p=>p.state==actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&originalClock!=null&&documentClock!=null&&originalClock.sourceMotionDuration==documentClock.sourceModelMotionDuration&&documentClock.sourceMotionDuration==expected.MotionDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Core IceB exact original state/clock/no authored SHOW changed "+actual.id);PreviewRequire(originalClock.sourceMotionDuration==expected.MotionDuration&&expected.Speed==1,"Core IceB unchanged source geometry/model clocks with NPC1.05 already baked and extra1 "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core IceB relative PNG path escaped/mismatched package");PreviewRequire(key.Width>0&&key.Height>0&&key.Width<=267&&key.Height<=264,"Core IceB complete native PNG canvas dimensions changed; atlas384 cells are not sprite rects");CoreEntryReadV21(CoreElementIceBPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreElementIceBWholeV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(coreElementIceBWholeLookupV21==null&&coreElementIceBWholeSpritesV21.Count==0&&coreElementIceBWholeTexturesV21.Count==0,"Core IceB display owner must be empty before configure");var lookup=ValidateCoreElementIceBWholeDocumentV21(documentPin,reviewPin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreElementIceBWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-element-ice-b"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CoreElementIceBPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved IceB whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreElementIceBWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core IceB approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved IceB whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }coreElementIceBWholeLookupV21=lookup;CoreElementIceBWholeDrawsV21=CoreElementIceBWholeMissingStateHoldsV21=CoreElementIceBSourceShowApplicationsV21=CoreElementIceBSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreElementIceBSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELEMENT_ICE_B"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreElementIceBWholeLookupV21!=null,"Core IceB visibility requires configured exact library");var state=coreElementIceBWholeLookupV21.State(e.npcState);CoreElementIceBSourceShowLastAuthoredV21=state.HasShow;CoreElementIceBSourceShowLastValueV21=state.Show;CoreElementIceBSourceShowLastVisibleBeforeV21=visible;visible=coreElementIceBWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreElementIceBSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreElementIceBSourceShowApplicationsV21++;else CoreElementIceBSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreElementIceBWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELEMENT_ICE_B")return false;
  PreviewRequire(coreElementIceBWholeLookupV21!=null,"Core IceB render requires configured exact library");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreElementIceBWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="nasod-element-ice-b"&&e.visualScale==1,"Core IceB original source actor identity/scale");var selected=coreElementIceBWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/IceB",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=coreElementIceBWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Selected signed whole anchor includes the Root-approved finite source registration.
  // No SpriteOffset/floor/whole offset is added again; no actor/model field is mutated.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CoreElementIceBWholeLastKeyV21=key.Key;CoreElementIceBWholeLastStateV21=displayState;CoreElementIceBWholeLastSourcePoseV21=selected.SourcePose;CoreElementIceBWholeLastNpcClockV21=displayTime;CoreElementIceBWholeLastMotionClockV21=selected.SourceMotionClock;CoreElementIceBWholeLastAnchorXV21=selected.AnchorX;CoreElementIceBWholeLastAnchorYV21=selected.AnchorY;CoreElementIceBWholeDrawsV21++;return true;
 }
 void DisposeCoreElementIceBWholeV21(){
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  coreElementIceBWholeSpritesV21.Clear();coreElementIceBWholeTexturesV21.Clear();coreElementIceBWholeLookupV21=null;
 }
}}

