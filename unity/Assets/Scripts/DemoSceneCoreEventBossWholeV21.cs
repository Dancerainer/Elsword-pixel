using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {public sealed partial class DemoScene {
 const string CoreEventBossWholeDocumentShaV21="128375fd2c6aaeb000553853b90d376ef2ba281b3032c1412cbf492fb49f1eac";
 const string CoreEventBossWholeReviewShaV21="562a3635ea3b2e95fd34ce660d8f48f3457a9aeb421bb5f026e62b5897d0e42b";
 const string CoreEventBossWholeRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 SourcePhaseWholeLookupV21 coreEventBossWholeLookupV21;
 readonly Dictionary<string,Sprite> coreEventBossWholeSpritesV21=new Dictionary<string,Sprite>(StringComparer.Ordinal);
 readonly Dictionary<string,Texture2D> coreEventBossWholeTexturesV21=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreEventBossWholeConfiguredV21{get{return coreEventBossWholeLookupV21!=null;}}
 public int CoreEventBossWholeResidentKeysV21{get{return coreEventBossWholeSpritesV21.Count;}}
 public long CoreEventBossWholeResidentRgbaBytesV21{get{long n=0;foreach(var t in coreEventBossWholeTexturesV21.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreEventBossWholeDrawsV21{get;private set;}public int CoreEventBossWholeMissingStateHoldsV21{get;private set;}
 public int CoreEventBossSourceShowApplicationsV21{get;private set;}public int CoreEventBossSourceShowAbsencePreservationsV21{get;private set;}
 public string CoreEventBossWholeLastKeyV21{get;private set;}public string CoreEventBossWholeLastStateV21{get;private set;}public string CoreEventBossWholeLastSourcePoseV21{get;private set;}
 public float CoreEventBossWholeLastNpcClockV21{get;private set;}public float CoreEventBossWholeLastMotionClockV21{get;private set;}
 public double CoreEventBossWholeLastAnchorXV21{get;private set;}public double CoreEventBossWholeLastAnchorYV21{get;private set;}
 public bool CoreEventBossSourceShowLastAuthoredV21{get;private set;}public bool CoreEventBossSourceShowLastValueV21{get;private set;}
 public bool CoreEventBossSourceShowLastVisibleBeforeV21{get;private set;}public bool CoreEventBossSourceShowLastVisibleAfterV21{get;private set;}
 public bool CoreEventBossNativeCallbackConsumersBoundV21{get{return false;}}

 static PreviewFile CoreEventBossPinV21(SourceWholeKeyInfoV21 k){return new PreviewFile{path=k.PngPath,sha256=k.PngSha256};}
 static SourcePhaseWholeLookupV21 ValidateCoreEventBossWholeDocumentV21(PreviewFile documentPin,PreviewFile reviewPin,NpcDefinition[] activeRules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreEventBossWholeDocumentV21RuntimeR1147(documentPin,reviewPin,activeRules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreEventBossWholeDocumentShaV21&&reviewPin!=null&&reviewPin.sha256==CoreEventBossWholeReviewShaV21,"Core Event exact reviewed art/data pins required");CoreEntryReadV21(reviewPin);
  var d=CoreEntryJsonV21<SourceWholeDocumentV21>(documentPin);var lookup=new SourcePhaseWholeLookupV21(d);
  PreviewRequire(lookup.Id=="event-npc-altera-core-boss"&&lookup.SourceNpc=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"&&lookup.SourceRulesSha256==CoreEventBossWholeRulesShaV21&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==4&&lookup.MotionCount==2&&lookup.StateCount==2&&d.sourcePoseSamples==5&&d.artAndSampledPhaseReview.path==reviewPin.path&&d.artAndSampledPhaseReview.sha256==reviewPin.sha256,"Core Event exact4whole/2motion/2state/5sample/source-facing closure");
  var profile=Array.Find(activeRules??new NpcDefinition[0],p=>p.id==lookup.SourceNpc);PreviewRequire(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==2,"Core Event original active profile required");
  foreach(var actual in profile.states){var expected=lookup.State(actual.id);PreviewRequire(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState,"Core Event existing source state/clock changed "+actual.id);PreviewRequire(!expected.HasShow&&!expected.Show,"Core Event source SHOW absence must preserve model visibility "+actual.id);}
  PreviewRequire(lookup.State("EVENT_NPC_ALTERA_CORE_BOSS_START").StateDuration==13.333333333333334f&&lookup.State("EVENT_NPC_ALTERA_CORE_BOSS_START").MotionDuration==13.333333333333334f&&lookup.State("EVENT_NPC_ALTERA_CORE_BOSS_DYING").StateDuration==.2f&&lookup.State("EVENT_NPC_ALTERA_CORE_BOSS_DYING").MotionDuration==.2f,"Core Event original Start13.333333/Dying.2 state and motion durations");
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));foreach(var key in lookup.Keys()){
   string relative=key.File.Replace('/',Path.DirectorySeparatorChar);string path=Path.GetFullPath(Path.Combine(directory,relative));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.PngPath),StringComparison.OrdinalIgnoreCase),"Core Event relative whole PNG escaped/mismatched package");
   bool exact=key.Key=="bank-01"?key.Width==1717&&key.Height==785&&key.AnchorX==859&&key.AnchorY==766:key.Key=="bank-02"?key.Width==1596&&key.Height==848&&key.AnchorX==865&&key.AnchorY==960:key.Key=="bank-03"?key.Width==1569&&key.Height==859&&key.AnchorX==822&&key.AnchorY==959:key.Key=="bank-04"&&key.Width==1564&&key.Height==899&&key.AnchorX==782&&key.AnchorY==960;
   PreviewRequire(exact,"Core Event lossless approved whole crop/root changed");CoreEntryReadV21(CoreEventBossPinV21(key));
  }return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreEventBossWholeV21(PreviewFile documentPin,PreviewFile reviewPin,NpcDefinition[] rules){
  PreviewRequire(coreEventBossWholeLookupV21==null&&coreEventBossWholeSpritesV21.Count==0&&coreEventBossWholeTexturesV21.Count==0,"Core Event display owner must be empty before configure");var lookup=ValidateCoreEventBossWholeDocumentV21(documentPin,reviewPin,rules);
  foreach(var key in lookup.Keys()){
   // Join the existing loading transaction before the fallible image decode.
   // Common cancel/End/OnDestroy owns actual Unity object destruction.
   byte[] bytes=CoreEntryReadV21(CoreEventBossPinV21(key));var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core approved Event whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);coreEventBossWholeTexturesV21.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Core Event approved lossless PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Event whole / "+key.Key;forestPreviewOwned.Add(sprite);coreEventBossWholeSpritesV21.Add(key.Key,sprite);
  }coreEventBossWholeLookupV21=lookup;CoreEventBossWholeDrawsV21=CoreEventBossWholeMissingStateHoldsV21=CoreEventBossSourceShowApplicationsV21=CoreEventBossSourceShowAbsencePreservationsV21=0;
 }
 bool ApplyCoreEventBossSourceVisibilityV21(Enemy e,ref bool visible){
  if(e==null||e.sourceNPC!="NUI_EVENT_NPC_ALTERA_CORE_BOSS"||string.IsNullOrEmpty(e.npcState))return false;
  PreviewRequire(coreEventBossWholeLookupV21!=null,"Core Event visibility requires configured exact library");var state=coreEventBossWholeLookupV21.State(e.npcState);CoreEventBossSourceShowLastAuthoredV21=state.HasShow;CoreEventBossSourceShowLastValueV21=state.Show;CoreEventBossSourceShowLastVisibleBeforeV21=visible;visible=coreEventBossWholeLookupV21.ApplySourceShow(e.npcState,visible);CoreEventBossSourceShowLastVisibleAfterV21=visible;
  if(state.HasShow)CoreEventBossSourceShowApplicationsV21++;else CoreEventBossSourceShowAbsencePreservationsV21++;return state.HasShow;
 }
 bool RenderCoreEventBossWholeV21(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_EVENT_NPC_ALTERA_CORE_BOSS")return false;
  // The production SimpleDanger policy already leaves this historical movie
  // actor hidden and without a source state. Do not request a movie clock for
  // that deliberately inactive actor; the model remains the visibility owner.
  if(game.SimpleDangerIntroductions&&!game.CoreHardActorShownV21(e)){renderer.enabled=false;renderer.sprite=null;return true;}
  PreviewRequire(coreEventBossWholeLookupV21!=null,"Core Event render requires configured exact library");
  if(!TryCoreWholeDisplayV44(e,renderer,out var displayState,out var displayTime)){CoreEventBossWholeMissingStateHoldsV21++;return true;}
  PreviewRequire(e.kind=="event-npc-altera-core-boss"&&e.visualScale==1,"Core Event original source identity/scale");var selected=coreEventBossWholeLookupV21.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/EventBoss",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
  renderer.enabled=true;renderer.sprite=coreEventBossWholeSpritesV21[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // Lossless crop anchor already restores the original source floor/common
  // origin. Never add SpriteOffset, cropOrigin, floor or common XYZ again.
  var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  CoreEventBossWholeLastKeyV21=key.Key;CoreEventBossWholeLastStateV21=displayState;CoreEventBossWholeLastSourcePoseV21=selected.SourcePose;CoreEventBossWholeLastNpcClockV21=displayTime;CoreEventBossWholeLastMotionClockV21=selected.SourceMotionClock;CoreEventBossWholeLastAnchorXV21=selected.AnchorX;CoreEventBossWholeLastAnchorYV21=selected.AnchorY;CoreEventBossWholeDrawsV21++;return true;
 }
 void DisposeCoreEventBossWholeV21(){
  // These native objects belong to forestPreviewOwned; do not double-destroy.
  coreEventBossWholeSpritesV21.Clear();coreEventBossWholeTexturesV21.Clear();coreEventBossWholeLookupV21=null;
 }
}}

