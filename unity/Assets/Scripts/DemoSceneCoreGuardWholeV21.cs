using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreGuardWholeDocumentShaV21="6a1e9e2851160336b00e20caa31995b031631e2a0e067ce9f1bbdeabe0a2e4ed";
 const string CoreGuardWholeReviewShaV21="38e6de9f709601df3890085202d72e7bbbe177e2485b5f6f59a44ec1a410f968";
 const string CoreGuardWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 [Serializable] sealed class CoreGuardClockStateV21 {public string id;public float sourceMotionDuration;}
 [Serializable] sealed class CoreGuardClockProfileV21 {public string id;public CoreGuardClockStateV21[] states;}
 [Serializable] sealed class CoreGuardClockRulesV21 {public CoreGuardClockProfileV21[] definitions;}
 [Serializable] sealed class CoreGuardDocumentClockStateV21 {public string state;public float sourceModelMotionDuration,sourceMotionDuration;}
 [Serializable] sealed class CoreGuardDocumentClocksV21 {public CoreGuardDocumentClockStateV21[] states;}
 SourcePhaseWholeLookupV21 coreGuardWholeLookupV21;
 readonly DeferredWholeSpritesV53 coreGuardWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreGuardWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreGuardWholeConfiguredV21{get{return coreGuardWholeLookupV21!=null;}}
 public int CoreGuardWholeResidentKeysV21{get{return coreGuardWholeSpritesV21.ResidentCount;}}
 public long CoreGuardWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreGuardWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreGuardWholeDrawsV21{get;private set;}public int CoreGuardWholeMissingStateHoldsV21{get;private set;}
 public int CoreGuardSourceShowApplicationsV21{get;private set;}public int CoreGuardSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreGuardWholeLastKeyV21{get;private set;}public string CoreGuardWholeLastStateV21{get;private set;}public string CoreGuardWholeLastSourcePoseV21{get;private set;}
 public float CoreGuardWholeLastNpcClockV21{get;private set;}public float CoreGuardWholeLastMotionClockV21{get;private set;}
 public double CoreGuardWholeLastAnchorXV21{get;private set;}public double CoreGuardWholeLastAnchorYV21{get;private set;}
 public bool CoreGuardSourceShowLastAuthoredV21{get;private set;}public bool CoreGuardSourceShowLastValueV21{get;private set;}
 public bool CoreGuardSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreGuardSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreGuardWholeEmitsGameplayCallbacksV21{get{return false;}}

 static PreviewFile CoreGuardPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreGuardWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreGuardWholeDocumentV21RuntimeR1147(documentPin,reviewPin,sourceRulesPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreGuardWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreGuardWholeReviewShaV21,"Core Guard exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(documentPin));var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="nasod-elite-guard"&&lookup.SourceNpc=="NUI_NASOD_ELITE_GUARD"&&lookup.SourceRulesSha256==CoreGuardWholeRulesShaV21&&lookup.FixedSourceFacing=="original-source-orientation"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==93&&lookup.MotionCount==20&&lookup.StateCount==27&&d.sourcePoseSamples==108&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core Guard exact93whole/20motion/27state/108finite-source-clock phases");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==27,"Core Guard original active profile required");
  PreviewRequire(sourceRulesPin!=null&&sourceRulesPin.sha256==CoreGuardWholeRulesShaV21,"Core Guard exact original motion-clock rules pin required");var clockRules=CoreEntryJsonV21<CoreGuardClockRulesV21>(sourceRulesPin);var clockProfile=Array.Find(clockRules.definitions??new CoreGuardClockProfileV21[0],p=>p.id==lookup.SourceNpc);PreviewRequire(clockProfile!=null&&clockProfile.states!=null&&clockProfile.states.Length==27,"Core Guard original raw27 model-clock fields required");
  var documentClocks=CoreEntryJsonV21<CoreGuardDocumentClocksV21>(documentPin);PreviewRequire(documentClocks.states!=null&&documentClocks.states.Length==27,"Core Guard both original model expiry and geometry clocks required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);var originalClock=Array.Find(clockProfile.states,p=>p.id==actual.id);var documentClock=Array.Find(documentClocks.states,p=>p.state==actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&originalClock!=null&&documentClock!=null&&originalClock.sourceMotionDuration==documentClock.sourceModelMotionDuration&&documentClock.sourceMotionDuration==expected.MotionDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Core Guard exact original state/clock/no authored SHOW changed "+actual.id);PreviewRequire(originalClock.sourceMotionDuration==expected.MotionDuration&&expected.Speed==1,"Core Guard unchanged source geometry/model clocks "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core Guard relative PNG path escaped/mismatched package");PreviewRequire(key.Width>0&&key.Height>0&&key.Width<=146&&key.Height<=136,"Core Guard complete native PNG canvas dimensions changed; atlas384 cells are not sprite rects");CoreEntryReadV21(CoreGuardPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreGuardWholeV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(coreGuardWholeLookupV21==null&&coreGuardWholeSpritesV21.Count==0&&coreGuardWholeTexturesV21.Count==0,"Core Guard display owner must be empty before configure");var lookup=ValidateCoreGuardWholeDocumentV21(documentPin,reviewPin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreGuardWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-elite-guard"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CoreGuardPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved Guard whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreGuardWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core Guard approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Guard whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }coreGuardWholeLookupV21=lookup;CoreGuardWholeDrawsV21=CoreGuardWholeMissingStateHoldsV21=CoreGuardSourceShowApplicationsV21=CoreGuardSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreGuardSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELITE_GUARD"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreGuardWholeLookupV21!=null,"Core Guard visibility requires configured exact library");var state=coreGuardWholeLookupV21.State(e.npcState);CoreGuardSourceShowLastAuthoredV21=state.HasShow;CoreGuardSourceShowLastValueV21=state.Show;CoreGuardSourceShowLastVisibleBeforeV21=visible;visible=coreGuardWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreGuardSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreGuardSourceShowApplicationsV21++;else CoreGuardSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreGuardWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELITE_GUARD")return false;
  PreviewRequire(coreGuardWholeLookupV21!=null,"Core Guard render requires configured exact library");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreGuardWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="nasod-elite-guard"&&e.visualScale==1,"Core Guard original source actor identity/scale");var selected=coreGuardWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Guard",displayState,selected.State.Motion,(float)selected.SourceMotionClock,e.facing<0?-1:1);
  renderer.enabled=true;renderer.sprite=coreGuardWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Selected signed whole anchor includes the Root-approved finite source registration.
  // No SpriteOffset/floor/whole offset is added again; no actor/model field is mutated.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  FitCoreEliteWholeV35(e,renderer,selected.State.Motion,selected.SourceMotionClock);
  CoreGuardWholeLastKeyV21=key.Key;CoreGuardWholeLastStateV21=displayState;CoreGuardWholeLastSourcePoseV21=selected.SourcePose;CoreGuardWholeLastNpcClockV21=displayTime;CoreGuardWholeLastMotionClockV21=selected.SourceMotionClock;CoreGuardWholeLastAnchorXV21=selected.AnchorX;CoreGuardWholeLastAnchorYV21=selected.AnchorY;CoreGuardWholeDrawsV21++;return true;
 }
 void DisposeCoreGuardWholeV21(){
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  coreGuardWholeSpritesV21.Clear();coreGuardWholeTexturesV21.Clear();coreGuardWholeLookupV21=null;
 }
}}

