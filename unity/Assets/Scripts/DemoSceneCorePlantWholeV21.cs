using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CorePlantWholeDocumentShaV21="306d627a2d5dc1a6cc4401a36e3863a7e5fd85ea0ab592e19dbfd54c9f305c13";
 const string CorePlantWholeReviewShaV21="b526ec085284cbfb8669bfe255bbd1c05e5399376a3b5e42a2c68bcc6b311671";
 const string CorePlantWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 SourcePhaseWholeLookupV21 corePlantWholeLookupV21;
 readonly DeferredWholeSpritesV53 corePlantWholeSpritesV21=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> corePlantWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CorePlantWholeConfiguredV21{get{return corePlantWholeLookupV21!=null;}}
 public int CorePlantWholeResidentKeysV21{get{return corePlantWholeSpritesV21.ResidentCount;}}
 public long CorePlantWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in corePlantWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CorePlantWholeDrawsV21{get;private set;}public int CorePlantWholeMissingStateHoldsV21{get;private set;}
 public int CorePlantSourceShowApplicationsV21{get;private set;}public int CorePlantSourceShowAbsencePreservationsV21{get;private set;}
 public string CorePlantWholeLastKeyV21{get;private set;}public string CorePlantWholeLastStateV21{get;private set;}public string CorePlantWholeLastSourcePoseV21{get;private set;}
 public float CorePlantWholeLastNpcClockV21{get;private set;}public float CorePlantWholeLastMotionClockV21{get;private set;}
 public double CorePlantWholeLastAnchorXV21{get;private set;}public double CorePlantWholeLastAnchorYV21{get;private set;}
 public bool CorePlantSourceShowLastAuthoredV21{get;private set;}public bool CorePlantSourceShowLastValueV21{get;private set;}
 public bool CorePlantSourceShowLastVisibleBeforeV21{get;private set;}public bool CorePlantSourceShowLastVisibleAfterV21{get;private set;}
 public bool CorePlantNativeCallbackConsumersBoundV21{get{return false;}}

 static PreviewFile CorePlantPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCorePlantWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCorePlantWholeDocumentV21RuntimeR1147(documentPin,reviewPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CorePlantWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CorePlantWholeReviewShaV21,"Core Plant exact approved art/data pins required");CoreEntryReadV21(reviewPin);
  var d=CoreEntryJsonV21<SourceWholeDocumentV21>(documentPin);var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="el-power-plant"&&lookup.SourceNpc=="NUI_EL_POWER_PLANT"&&lookup.SourceRulesSha256==CorePlantWholeRulesShaV21&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==7&&lookup.MotionCount==6&&lookup.StateCount==7&&d.sourcePoseSamples==21&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core Plant exact7whole/6motion/7state/source-facing closure");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==7,"Core Plant original active profile required");
  int falseShows=0,trueShows=0;foreach(var actual in profile.states){var expected=lookup.State(actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState,"Core Plant existing source state/clock changed "+actual.id);bool show=actual.id!="EL_POWER_PLANT_START"&&actual.id!="EL_POWER_PLANT_WAIT_DOWN";PreviewRequire(expected.HasShow&&expected.Show==show,"Core Plant explicit source SHOW presence/value changed "+actual.id);if(show)trueShows++;else falseShows++;}
  PreviewRequire(falseShows==2&&trueShows==5&&lookup.State("EL_POWER_PLANT_DOWN").StateDuration==20&&lookup.State("EL_POWER_PLANT_DOWN").MotionDuration==2.6f,"Core Plant hide/start and separateDOWN20sstate/2.6motion");
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core Plant relative PNG path escaped/mismatched package");PreviewRequire((key.Key=="approved-neutral"?key.Width==512&&key.Height==512&&key.AnchorX==256&&key.AnchorY==448:key.Width==1024&&key.Height==1024&&key.AnchorX==512&&key.AnchorY==512),"Core Plant approved per-key canvas/root changed");CoreEntryReadV21(CorePlantPinV21(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCorePlantWholeV21(PreviewFile documentPin,PreviewFile reviewPin,NpcDefinition[] rules){
  PreviewRequire(corePlantWholeLookupV21==null&&corePlantWholeSpritesV21.Count==0&&corePlantWholeTexturesV21.Count==0,"Core Plant display owner must be empty before configure");var lookup=ValidateCorePlantWholeDocumentV21(documentPin,reviewPin,rules);
  foreach(var key in lookup.Keys()){
   corePlantWholeSpritesV21.Register(key.Key,DeferMonsterPixelCarrierV53("el-power-plant"),()=>{
   // Attach each native object to the existing loading transaction before any
   // fallible decode. Common cancel/End/OnDestroy owns actual object retirement.
   byte[] bytes=CoreEntryReadV21(CorePlantPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved Plant whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);corePlantWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core Plant approved PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Plant whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }corePlantWholeLookupV21=lookup;CorePlantWholeDrawsV21=CorePlantWholeMissingStateHoldsV21=CorePlantSourceShowApplicationsV21=CorePlantSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCorePlantSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_EL_POWER_PLANT"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(corePlantWholeLookupV21!=null,"Core Plant visibility requires configured exact library");var state=corePlantWholeLookupV21.State(e.npcState);CorePlantSourceShowLastAuthoredV21=state.HasShow;CorePlantSourceShowLastValueV21=state.Show;CorePlantSourceShowLastVisibleBeforeV21=visible;visible=corePlantWholeLookupV21.ApplySourceShow(e.npcState,visible);CorePlantSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CorePlantSourceShowApplicationsV21++;else CorePlantSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCorePlantWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_EL_POWER_PLANT")return false;
  PreviewRequire(corePlantWholeLookupV21!=null,"Core Plant render requires configured exact library");
  if(!TryCoreWholeDisplayV44(e,renderer,out var displayState,out var displayTime)){CorePlantWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="el-power-plant"&&e.visualScale==1,"Core Plant original source actor identity/scale");var selected=corePlantWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Plant",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=corePlantWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Frame anchor already restores source floor/reference origin and includes
  // the Up-mid wholeY delta. No SpriteOffset/floor/whole offset is added again.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CorePlantWholeLastKeyV21=key.Key;CorePlantWholeLastStateV21=displayState;CorePlantWholeLastSourcePoseV21=selected.SourcePose;CorePlantWholeLastNpcClockV21=displayTime;CorePlantWholeLastMotionClockV21=selected.SourceMotionClock;CorePlantWholeLastAnchorXV21=selected.AnchorX;CorePlantWholeLastAnchorYV21=selected.AnchorY;CorePlantWholeDrawsV21++;return true;
 }
 void DisposeCorePlantWholeV21(){
  // Native textures/sprites are in forestPreviewOwned; do not double-destroy.
  corePlantWholeSpritesV21.Clear();corePlantWholeTexturesV21.Clear();corePlantWholeLookupV21=null;
 }
}}

