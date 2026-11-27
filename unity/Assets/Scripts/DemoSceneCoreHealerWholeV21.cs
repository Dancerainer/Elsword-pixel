using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreHealerWholeDocumentShaV21="4ca179765a408bcc0cc3fec32fa682711b9da4e62b4c26ec2e7bf301ddc9f252";
 const string CoreHealerWholeReviewShaV21="1fa6aea64a552126559d1fecc1cf917c9b04d44f26b4ba4fc3b659a1dd43ba1e";
 const string CoreHealerWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 [Serializable] sealed class CoreHealerClockStateV21 {public string id;public float sourceMotionDuration;}
 [Serializable] sealed class CoreHealerClockProfileV21 {public string id;public CoreHealerClockStateV21[] states;}
 [Serializable] sealed class CoreHealerClockRulesV21 {public CoreHealerClockProfileV21[] definitions;}
 SourcePhaseWholeLookupV21 coreHealerWholeLookupV21;
 readonly DeferredWholeSpritesV53 coreHealerWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreHealerWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreHealerWholeConfiguredV21{get{return coreHealerWholeLookupV21!=null;}}
 public int CoreHealerWholeResidentKeysV21{get{return coreHealerWholeSpritesV21.ResidentCount;}}
 public long CoreHealerWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreHealerWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreHealerWholeDrawsV21{get;private set;}public int CoreHealerWholeMissingStateHoldsV21{get;private set;}
 public int CoreHealerSourceShowApplicationsV21{get;private set;}public int CoreHealerSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreHealerWholeLastKeyV21{get;private set;}public string CoreHealerWholeLastStateV21{get;private set;}public string CoreHealerWholeLastSourcePoseV21{get;private set;}
 public float CoreHealerWholeLastNpcClockV21{get;private set;}public float CoreHealerWholeLastMotionClockV21{get;private set;}
 public double CoreHealerWholeLastAnchorXV21{get;private set;}public double CoreHealerWholeLastAnchorYV21{get;private set;}
 public bool CoreHealerSourceShowLastAuthoredV21{get;private set;}public bool CoreHealerSourceShowLastValueV21{get;private set;}
 public bool CoreHealerSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreHealerSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreHealerWholeEmitsGameplayCallbacksV21{get{return false;}}

 static PreviewFile CoreHealerPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreHealerWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreHealerWholeDocumentV21RuntimeR1147(documentPin,reviewPin,sourceRulesPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreHealerWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreHealerWholeReviewShaV21,"Core Healer exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(documentPin));var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="nasod-healer-blader"&&lookup.SourceNpc=="NUI_NASOD_HEALER_BLADER"&&lookup.SourceRulesSha256==CoreHealerWholeRulesShaV21&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==52&&lookup.MotionCount==14&&lookup.StateCount==26&&d.sourcePoseSamples==85&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core Healer exact52whole/14motion/26state/85finite-source-clock phases");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==26,"Core Healer original active profile required");
  PreviewRequire(sourceRulesPin!=null&&sourceRulesPin.sha256==CoreHealerWholeRulesShaV21,"Core Healer exact original motion-clock rules pin required");var clockRules=CoreEntryJsonV21<CoreHealerClockRulesV21>(sourceRulesPin);var clockProfile=Array.Find(clockRules.definitions??new CoreHealerClockProfileV21[0],p=>p.id==lookup.SourceNpc);PreviewRequire(clockProfile!=null&&clockProfile.states!=null&&clockProfile.states.Length==26,"Core Healer original raw26 motion-clock fields required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);var originalClock=Array.Find(clockProfile.states,p=>p.id==actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&originalClock!=null&&originalClock.sourceMotionDuration==expected.MotionDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Core Healer exact original state/clock/no authored SHOW changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core Healer relative PNG path escaped/mismatched package");PreviewRequire(key.Width>0&&key.Height>0&&key.Width<=199&&key.Height<=81,"Core Healer complete native PNG canvas dimensions changed; atlas384 cells are not sprite rects");CoreEntryReadV21(CoreHealerPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreHealerWholeV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(coreHealerWholeLookupV21==null&&coreHealerWholeSpritesV21.Count==0&&coreHealerWholeTexturesV21.Count==0,"Core Healer display owner must be empty before configure");var lookup=ValidateCoreHealerWholeDocumentV21(documentPin,reviewPin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreHealerWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-healer-blader"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CoreHealerPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved Healer whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreHealerWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core Healer approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Healer whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }coreHealerWholeLookupV21=lookup;CoreHealerWholeDrawsV21=CoreHealerWholeMissingStateHoldsV21=CoreHealerSourceShowApplicationsV21=CoreHealerSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreHealerSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_NASOD_HEALER_BLADER"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreHealerWholeLookupV21!=null,"Core Healer visibility requires configured exact library");var state=coreHealerWholeLookupV21.State(e.npcState);CoreHealerSourceShowLastAuthoredV21=state.HasShow;CoreHealerSourceShowLastValueV21=state.Show;CoreHealerSourceShowLastVisibleBeforeV21=visible;visible=coreHealerWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreHealerSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreHealerSourceShowApplicationsV21++;else CoreHealerSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreHealerWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_HEALER_BLADER")return false;
  PreviewRequire(coreHealerWholeLookupV21!=null,"Core Healer render requires configured exact library");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreHealerWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="nasod-healer-blader"&&e.visualScale==1,"Core Healer original source actor identity/scale");var selected=coreHealerWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Healer",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=coreHealerWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Selected signed whole anchor includes the Root-approved finite source registration.
  // No SpriteOffset/floor/whole offset is added again; no actor/model field is mutated.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CoreHealerWholeLastKeyV21=key.Key;CoreHealerWholeLastStateV21=displayState;CoreHealerWholeLastSourcePoseV21=selected.SourcePose;CoreHealerWholeLastNpcClockV21=displayTime;CoreHealerWholeLastMotionClockV21=selected.SourceMotionClock;CoreHealerWholeLastAnchorXV21=selected.AnchorX;CoreHealerWholeLastAnchorYV21=selected.AnchorY;RecordHealerBladerSelectionV35(e,renderer,selected);CoreHealerWholeDrawsV21++;return true;
 }
 void DisposeCoreHealerWholeV21(){
  DisposeHealerBladerViewsV35();
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  coreHealerWholeSpritesV21.Clear();coreHealerWholeTexturesV21.Clear();coreHealerWholeLookupV21=null;
 }
}}

