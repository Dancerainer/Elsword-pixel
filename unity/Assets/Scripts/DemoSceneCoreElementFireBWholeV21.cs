using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreElementFireBWholeDocumentShaV21="b3db35a4f93dce05cfc7b16e9de17635bc1e2fcefac104faf6512eb44e10e1c1";
 const string CoreElementFireBWholeReviewShaV21="002a53f80996299809215d62b03db8223dc024d3b3d8a6b4b7976e736fcbfe86";
 const string CoreElementFireBWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 [Serializable] sealed class CoreElementFireBClockStateV21 {public string id;public float sourceMotionDuration;}
 [Serializable] sealed class CoreElementFireBClockProfileV21 {public string id;public CoreElementFireBClockStateV21[] states;}
 [Serializable] sealed class CoreElementFireBClockRulesV21 {public CoreElementFireBClockProfileV21[] definitions;}
 SourcePhaseWholeLookupV21 coreElementFireBWholeLookupV21;
 readonly DeferredWholeSpritesV53 coreElementFireBWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreElementFireBWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreElementFireBWholeConfiguredV21{get{return coreElementFireBWholeLookupV21!=null;}}
 public int CoreElementFireBWholeResidentKeysV21{get{return coreElementFireBWholeSpritesV21.ResidentCount;}}
 public long CoreElementFireBWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreElementFireBWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreElementFireBWholeDrawsV21{get;private set;}public int CoreElementFireBWholeMissingStateHoldsV21{get;private set;}
 public int CoreElementFireBSourceShowApplicationsV21{get;private set;}public int CoreElementFireBSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreElementFireBWholeLastKeyV21{get;private set;}public string CoreElementFireBWholeLastStateV21{get;private set;}public string CoreElementFireBWholeLastSourcePoseV21{get;private set;}
 public float CoreElementFireBWholeLastNpcClockV21{get;private set;}public float CoreElementFireBWholeLastMotionClockV21{get;private set;}
 public double CoreElementFireBWholeLastAnchorXV21{get;private set;}public double CoreElementFireBWholeLastAnchorYV21{get;private set;}
 public bool CoreElementFireBSourceShowLastAuthoredV21{get;private set;}public bool CoreElementFireBSourceShowLastValueV21{get;private set;}
 public bool CoreElementFireBSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreElementFireBSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreElementFireBWholeEmitsGameplayCallbacksV21{get{return false;}}

 static PreviewFile CoreElementFireBPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreElementFireBWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreElementFireBWholeDocumentV21RuntimeR1147(documentPin,reviewPin,sourceRulesPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreElementFireBWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreElementFireBWholeReviewShaV21,"Core ElementFireB exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(documentPin));var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="nasod-element-fire-b"&&lookup.SourceNpc=="NUI_NASOD_ELEMENT_FIRE_B"&&lookup.SourceRulesSha256==CoreElementFireBWholeRulesShaV21&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==98&&lookup.MotionCount==28&&lookup.StateCount==33&&d.sourcePoseSamples==174&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core ElementFireB exact98whole/28motion/33state/174source-clock phase records");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==33,"Core ElementFireB original active profile required");
  PreviewRequire(sourceRulesPin!=null&&sourceRulesPin.sha256==CoreElementFireBWholeRulesShaV21,"Core ElementFireB exact original motion-clock rules pin required");var clockRules=CoreEntryJsonV21<CoreElementFireBClockRulesV21>(sourceRulesPin);var clockProfile=Array.Find(clockRules.definitions??new CoreElementFireBClockProfileV21[0],p=>p.id==lookup.SourceNpc);PreviewRequire(clockProfile!=null&&clockProfile.states!=null&&clockProfile.states.Length==33,"Core ElementFireB original raw33 motion-clock fields required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);var originalClock=Array.Find(clockProfile.states,p=>p.id==actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&originalClock!=null&&originalClock.sourceMotionDuration==expected.MotionDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Core ElementFireB exact original state/clock/no authored SHOW changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core ElementFireB relative PNG path escaped/mismatched package");PreviewRequire(key.Width>0&&key.Height>0&&key.Width<=213&&key.Height<=183,"Core ElementFireB complete native PNG canvas dimensions changed; atlas cells cells are not sprite rects");CoreEntryReadV21(CoreElementFireBPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreElementFireBWholeV21(PreviewFile documentPin,PreviewFile reviewPin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(coreElementFireBWholeLookupV21==null&&coreElementFireBWholeSpritesV21.Count==0&&coreElementFireBWholeTexturesV21.Count==0,"Core ElementFireB display owner must be empty before configure");var lookup=ValidateCoreElementFireBWholeDocumentV21(documentPin,reviewPin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreElementFireBWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-element-fire-b"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CoreElementFireBPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved ElementFireB whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreElementFireBWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core ElementFireB approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved ElementFireB whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }coreElementFireBWholeLookupV21=lookup;CoreElementFireBWholeDrawsV21=CoreElementFireBWholeMissingStateHoldsV21=CoreElementFireBSourceShowApplicationsV21=CoreElementFireBSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreElementFireBSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELEMENT_FIRE_B"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreElementFireBWholeLookupV21!=null,"Core ElementFireB visibility requires configured exact library");var state=coreElementFireBWholeLookupV21.State(e.npcState);CoreElementFireBSourceShowLastAuthoredV21=state.HasShow;CoreElementFireBSourceShowLastValueV21=state.Show;CoreElementFireBSourceShowLastVisibleBeforeV21=visible;visible=coreElementFireBWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreElementFireBSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreElementFireBSourceShowApplicationsV21++;else CoreElementFireBSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreElementFireBWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELEMENT_FIRE_B")return false;
  PreviewRequire(coreElementFireBWholeLookupV21!=null,"Core ElementFireB render requires configured exact library");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreElementFireBWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="nasod-element-fire-b"&&e.visualScale==1,"Core ElementFireB original source actor identity/scale");var selected=coreElementFireBWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/FireB",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=coreElementFireBWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Selected signed whole anchor includes the Root-approved finite source registration.
  // No SpriteOffset/floor/whole offset is added again; no actor/model field is mutated.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));
  if(SelectCoreInkV45(e.kind,coreElementFireBWholeLookupV21.State(displayState).Motion,(float)selected.SourceMotionClock,key.Key,renderer,out var projected))offset=projected;
  var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CoreElementFireBWholeLastKeyV21=key.Key;CoreElementFireBWholeLastStateV21=displayState;CoreElementFireBWholeLastSourcePoseV21=selected.SourcePose;CoreElementFireBWholeLastNpcClockV21=displayTime;CoreElementFireBWholeLastMotionClockV21=selected.SourceMotionClock;CoreElementFireBWholeLastAnchorXV21=selected.AnchorX;CoreElementFireBWholeLastAnchorYV21=selected.AnchorY;CoreElementFireBWholeDrawsV21++;return true;
 }
 void DisposeCoreElementFireBWholeV21(){
  ClearCoreInkV45("nasod-element-fire-b");
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  coreElementFireBWholeSpritesV21.Clear();coreElementFireBWholeTexturesV21.Clear();coreElementFireBWholeLookupV21=null;
 }
}}

