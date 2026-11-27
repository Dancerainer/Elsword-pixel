using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreWatchWholeDocumentShaV21="1e18f95034c1f919b987cf80ec445cc976c7d61705142620509b051a8017e8cc";
 const string CoreWatchWholeReviewShaV21="40ff2920836a503f36aeaf7381d79e4c71637b7d981169bfbe59d8ce5eea81f2";
 const string CoreWatchWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 SourcePhaseWholeLookupV21 coreWatchWholeLookupV21;
 readonly DeferredWholeSpritesV53 coreWatchWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreWatchWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreWatchWholeConfiguredV21{get{return coreWatchWholeLookupV21!=null;}}
 public int CoreWatchWholeResidentKeysV21{get{return coreWatchWholeSpritesV21.ResidentCount;}}
 public long CoreWatchWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreWatchWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreWatchWholeDrawsV21{get;private set;}public int CoreWatchWholeMissingStateHoldsV21{get;private set;}
 public int CoreWatchSourceShowApplicationsV21{get;private set;}public int CoreWatchSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreWatchWholeLastKeyV21{get;private set;}public string CoreWatchWholeLastStateV21{get;private set;}public string CoreWatchWholeLastSourcePoseV21{get;private set;}
 public float CoreWatchWholeLastNpcClockV21{get;private set;}public float CoreWatchWholeLastMotionClockV21{get;private set;}
 public double CoreWatchWholeLastAnchorXV21{get;private set;}public double CoreWatchWholeLastAnchorYV21{get;private set;}
 public bool CoreWatchSourceShowLastAuthoredV21{get;private set;}public bool CoreWatchSourceShowLastValueV21{get;private set;}
 public bool CoreWatchSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreWatchSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreWatchNativeCallbackConsumersBoundV21{get{return false;}}

 static PreviewFile CoreWatchPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreWatchWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreWatchWholeDocumentV21RuntimeR1147(documentPin,reviewPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreWatchWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreWatchWholeReviewShaV21,"Core Watch exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(documentPin));var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="nasod-watch"&&lookup.SourceNpc=="NUI_NASOD_WATCH"&&lookup.SourceRulesSha256==CoreWatchWholeRulesShaV21&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==44&&lookup.MotionCount==16&&lookup.StateCount==28&&d.sourcePoseSamples==79&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core Watch exact7whole/6motion/7state/source-facing closure");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==28,"Core Watch original active profile required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Core Watch exact original state/clock/no authored SHOW changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core Watch relative PNG path escaped/mismatched package");PreviewRequire(key.Width>0&&key.Height>0&&key.Width<=64&&key.Height<=64,"Core Watch complete tight whole dimensions changed");CoreEntryReadV21(CoreWatchPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreWatchWholeV21(PreviewFile documentPin,PreviewFile reviewPin,NpcDefinition[] rules){
  PreviewRequire(coreWatchWholeLookupV21==null&&coreWatchWholeSpritesV21.Count==0&&coreWatchWholeTexturesV21.Count==0,"Core Watch display owner must be empty before configure");var lookup=ValidateCoreWatchWholeDocumentV21(documentPin,reviewPin,rules);
  foreach(var key in lookup.Keys()){
   coreWatchWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-watch"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CoreWatchPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved Watch whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreWatchWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core Watch approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Watch whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }coreWatchWholeLookupV21=lookup;CoreWatchWholeDrawsV21=CoreWatchWholeMissingStateHoldsV21=CoreWatchSourceShowApplicationsV21=CoreWatchSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreWatchSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_NASOD_WATCH"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreWatchWholeLookupV21!=null,"Core Watch visibility requires configured exact library");var state=coreWatchWholeLookupV21.State(e.npcState);CoreWatchSourceShowLastAuthoredV21=state.HasShow;CoreWatchSourceShowLastValueV21=state.Show;CoreWatchSourceShowLastVisibleBeforeV21=visible;visible=coreWatchWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreWatchSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreWatchSourceShowApplicationsV21++;else CoreWatchSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreWatchWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_WATCH")return false;
  PreviewRequire(coreWatchWholeLookupV21!=null,"Core Watch render requires configured exact library");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreWatchWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="nasod-watch"&&e.visualScale==1,"Core Watch original source actor identity/scale");var selected=coreWatchWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Watch",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=coreWatchWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Selected signed whole anchor includes the Root-approved Start cap registration.
  // No SpriteOffset/floor/whole offset is added again; no actor/model field is mutated.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CoreWatchWholeLastKeyV21=key.Key;CoreWatchWholeLastStateV21=displayState;CoreWatchWholeLastSourcePoseV21=selected.SourcePose;CoreWatchWholeLastNpcClockV21=displayTime;CoreWatchWholeLastMotionClockV21=selected.SourceMotionClock;CoreWatchWholeLastAnchorXV21=selected.AnchorX;CoreWatchWholeLastAnchorYV21=selected.AnchorY;CoreWatchWholeDrawsV21++;return true;
 }
 void DisposeCoreWatchWholeV21(){
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  coreWatchWholeSpritesV21.Clear();coreWatchWholeTexturesV21.Clear();coreWatchWholeLookupV21=null;
 }
}}

