using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
#pragma warning disable 0649 // Exact pinned JSON, populated by Unity JsonUtility.
  [Serializable] sealed class CoreExtractorIndexV21 {
   public string schema;public string[] sourceNPCs;
   public int sourceStates,displayClips,storedFrameRecords,uniqueWholePNGAssets,newWholeDrawings;
   public bool requiresFixedSourceFacing,hostMustNotApplySourceFloorOrFrameOffsetTwice,animationLibraryApproved,runtimeAdmitted;
   public PreviewFile stateClips,sourceRules,staticArtApproval;public CoreExtractorSpriteBindingV21[] sprites;public CoreExtractorAtlasV21[] wholeAtlases;
  }
  [Serializable] sealed class CoreExtractorHandoffV21 {public string schema;public PreviewFile index,exactStaticApproval;public int sourceStates,displayClips,uniquePaintedPNGAssets;public bool staticArtApproved,animationLibraryApproved,runtimeAdmitted;}
  [Serializable] sealed class CoreExtractorSpriteBindingV21 {
   public string id,sourceNPC,wholeAsset,sharedPaintedPoseAssetId;public PreviewFile animations,atlas;public int sourceStates,displayClips,newDrawingContribution;public CoreExtractorOriginV21 sourceOriginEvidence;
  }
  [Serializable] sealed class CoreExtractorOriginV21 {
   public PreviewFile sourceIdentity,sourceContract;public float[] sourceCommonReferenceOffset,sourceCanvasRoot,nativeFrameAnchor,additionalWholeOffsetApplied;public int[] sourceCanvas;public float sourceDisplayRotationY;public bool sourceOriginAdapterChanged;
  }
  [Serializable] sealed class CoreExtractorAtlasV21 {public string asset;public PreviewFile atlas;public int width,height;public float[] nativeAnchor,canvasAnchor;}
  [Serializable] sealed class CoreExtractorMapV21 {
   public string schema;public int sourceStates,displayClips;public PreviewFile sourceRules;public bool usesExistingNpcStateAndNpcTime,sourceClockChanged,offsetAlreadyInFrameAnchor,motionPlaybackAndModelExpiryMustRemainSeparate;public float earthStartStateSeconds,earthStartMotionSeconds;public CoreExtractorStateV21[] rows;
  }
  [Serializable] sealed class CoreExtractorStateV21 {
   public string sourceNPC,kind,state,modelClip,displayClip,sourceAnimation,wholeAsset;
   public float sourceStateDuration,sourceMotionDuration,sourceAnimSpeed,libraryDurationSeconds,wholeBodyOffsetY;
   public bool sourceLoop,hasSourceShow,sourceShow,useExistingNpcStateAndNpcTime,sourceStateExpirySeparateFromMotionPlayback,offsetAlreadyInFrameAnchor,hostMustNotApplyOffsetAgain,sourceClockChanged;public float[] frameAnchor;
  }
  [Serializable] sealed class CoreExtractorLibraryDetailV21 {
   public string fixedSourceFacing,sharedPaintedPoseAssetId;public bool automaticSpawnFacingMirror,sourceOriginAdapterChanged,animationLibraryComplete,artApproved;public float expectedModelVisualScale;public CoreExtractorClipDetailV21[] clips;
  }
  [Serializable] sealed class CoreExtractorClipDetailV21 {
   public string id,sourceAnimation;public float sourceMotionDuration,sourceStateDuration,sourceAnimSpeed,durationSeconds;public bool loop,offsetAlreadyInFrameAnchor,sourceStateExpirySeparateFromMotionPlayback;public float[] wholeBodyOffset;public string[] sourceStateAliases;public CoreExtractorStateExpiryV21[] sourceStateExpiries;
  }
  [Serializable] sealed class CoreExtractorStateExpiryV21 {public string id;public float seconds;public bool loop;}
#pragma warning restore 0649

  const string CoreExtractorIndexShaV21="fc28220d8e265ffad2fb2bc2460130b7778e3d4e1d30aa22c46bdbb1232875d1";
  const string CoreExtractorHandoffShaV21="dbbf27ca9536c83d3df503443260c6241e22db5df605fa196bcc45a34410fd2d";
  const string CoreExtractorStaticApprovalShaV21="22c0a2f31a87a62f133f6636813519d6c723101c64bda6b9b70368fe1d186a0f";
  const string CoreExtractorRulesShaV21="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
  readonly Dictionary<string,CoreExtractorStateV21> coreExtractorStateRowsV21=new Dictionary<string,CoreExtractorStateV21>(StringComparer.Ordinal);
  readonly Dictionary<string,Sprite> coreExtractorWholeSpritesV21=new Dictionary<string,Sprite>(StringComparer.Ordinal);
  public int CoreExtractorWholeDrawsV21 {get;private set;}
  public int CoreExtractorMissingStateHoldsV21 {get;private set;}
  public int CoreExtractorSourceShowApplicationsV21 {get;private set;}
  public int CoreExtractorSourceShowAbsentPreservationsV21 {get;private set;}
  public int CoreExtractorWholeStateRowsV21 {get{return coreExtractorStateRowsV21.Count;}}
  public int CoreExtractorWholeDisplayClipsV21 {get{return coreExtractorWholeSpritesV21.Count;}}
  public string CoreExtractorWholeLastSourceNPCV21 {get;private set;}
  public string CoreExtractorWholeLastStateV21 {get;private set;}
  public string CoreExtractorWholeLastDisplayClipV21 {get;private set;}
  public string CoreExtractorWholeLastAssetV21 {get;private set;}
  public float CoreExtractorWholeLastNpcClockV21 {get;private set;}
  public float CoreExtractorWholeLastMotionPlaybackClockV21 {get;private set;}
  public float CoreExtractorWholeLastStateDurationV21 {get;private set;}
  public float CoreExtractorWholeLastMotionDurationV21 {get;private set;}
  public bool CoreExtractorWholeLastSourceLoopV21 {get;private set;}
  public bool CoreExtractorWholeLastHasSourceShowV21 {get;private set;}
  public bool CoreExtractorWholeLastSourceShowV21 {get;private set;}
  public float[] CoreExtractorWholeLastFrameAnchorV21 {get;private set;}
  public string CoreExtractorSourceShowLastSourceNPCV21 {get;private set;}
  public string CoreExtractorSourceShowLastStateV21 {get;private set;}
  public bool CoreExtractorSourceShowLastAuthoredV21 {get;private set;}
  public bool CoreExtractorSourceShowLastValueV21 {get;private set;}
  public bool CoreExtractorSourceShowLastVisibleBeforeV21 {get;private set;}
  public bool CoreExtractorSourceShowLastVisibleAfterV21 {get;private set;}
  public bool CoreExtractorGlowBuffCameraOwnersConsumedV21 {get{return false;}}

  static bool IsCoreExtractorWholeIdV21(string id){return id=="NUI_EL_EXTRACTOR_EARTH"||id=="NUI_EL_EXTRACTOR_FIRE"||id=="NUI_EL_EXTRACTOR_ICE"||id=="NUI_EL_EXTRACTOR_NOVA"||id=="NUI_EL_EXTRACTOR_NORMAL_A"||id=="NUI_EL_EXTRACTOR_NORMAL_B";}
  static bool CoreExtractorPairV21(float[] v,float x,float y){return v!=null&&v.Length==2&&v[0]==x&&v[1]==y;}
  static bool CoreExtractorZeroV21(float[] v,int size){if(v==null||v.Length!=size)return false;foreach(var n in v)if(n!=0)return false;return true;}
  static void ValidateCoreExtractorHandoffV21(PreviewFile handoffPin,PreviewFile indexPin){
   PreviewRequire(handoffPin!=null&&handoffPin.sha256==CoreExtractorHandoffShaV21&&indexPin!=null&&indexPin.sha256==CoreExtractorIndexShaV21,"Core Extractor exact handoff/index required");
   var handoff=CoreEntryJsonV21<CoreExtractorHandoffV21>(handoffPin);
   PreviewRequire(handoff!=null&&handoff.schema=="core-Extractor-six-source-ID-five-static-whole-library-handoff-v21-r1"&&handoff.index!=null&&handoff.index.path==indexPin.path&&handoff.index.sha256==indexPin.sha256&&handoff.exactStaticApproval!=null&&handoff.exactStaticApproval.sha256==CoreExtractorStaticApprovalShaV21&&handoff.sourceStates==26&&handoff.displayClips==13&&handoff.uniquePaintedPNGAssets==5&&handoff.staticArtApproved&&!handoff.animationLibraryApproved&&!handoff.runtimeAdmitted,"Core Extractor exact static/body handoff boundary");
   CoreEntryReadV21(handoff.exactStaticApproval);
  }
  void ConfigureCoreExtractorWholeV21(PreviewFile indexPin,PreviewFile handoffPin,NpcDefinition[] rules){if(!CoreProductResourcesActiveR1147)ValidateCoreExtractorHandoffV21(handoffPin,indexPin);ConfigureCoreExtractorWholeV21(indexPin,rules);}
  void ConfigureCoreExtractorWholeV21(PreviewFile indexPin,NpcDefinition[] rules){
   PreviewRequire(indexPin!=null&&indexPin.sha256==CoreExtractorIndexShaV21&&rules!=null&&coreExtractorStateRowsV21.Count==0&&coreExtractorWholeSpritesV21.Count==0,"Core Extractor exact new index/empty owner required");
   var index=CoreEntryJsonV21<CoreExtractorIndexV21>(indexPin);
   PreviewRequire(index!=null&&index.schema=="core-Extractor-six-ID-five-whole-standard-library-index-v21-r1"&&index.sourceStates==26&&index.displayClips==13&&index.storedFrameRecords==13&&index.uniqueWholePNGAssets==5&&index.newWholeDrawings==5&&index.sourceNPCs!=null&&index.sourceNPCs.Length==6&&index.sprites!=null&&index.sprites.Length==6&&index.wholeAtlases!=null&&index.wholeAtlases.Length==5&&index.requiresFixedSourceFacing&&index.hostMustNotApplySourceFloorOrFrameOffsetTwice&&!index.animationLibraryApproved&&!index.runtimeAdmitted,"Core Extractor six-ID/26state/13clip/five-static-whole boundary");
   PreviewRequire(index.stateClips!=null&&index.stateClips.sha256=="a0361da86e98fc120f68941d2bfb5f8ee194e3d394c75db9f7f92bfd6561cc9b"&&index.staticArtApproval!=null&&index.staticArtApproval.sha256==CoreExtractorStaticApprovalShaV21&&index.sourceRules!=null&&index.sourceRules.sha256==CoreExtractorRulesShaV21,"Core Extractor exact map/static approval/rules");
   if(!CoreProductResourcesActiveR1147)CoreEntryReadV21(index.staticArtApproval);CoreEntryReadV21(index.sourceRules);
   var map=CoreEntryJsonV21<CoreExtractorMapV21>(index.stateClips);
   PreviewRequire(map!=null&&map.schema=="core-Extractor-source-state-display-map-v21-r1"&&map.sourceStates==26&&map.displayClips==13&&map.rows!=null&&map.rows.Length==26&&map.sourceRules!=null&&map.sourceRules.sha256==CoreExtractorRulesShaV21&&map.usesExistingNpcStateAndNpcTime&&!map.sourceClockChanged&&map.offsetAlreadyInFrameAnchor&&map.motionPlaybackAndModelExpiryMustRemainSeparate&&map.earthStartStateSeconds==2&&map.earthStartMotionSeconds==1,"Core Extractor exact state selector and separate source clocks");
   var ids=new HashSet<string>(StringComparer.Ordinal);var atlasPins=new HashSet<string>(StringComparer.Ordinal);int showBindings=0,clipCount=0,frameCount=0;
   foreach(var a in index.wholeAtlases){PreviewRequire(a!=null&&a.atlas!=null&&atlasPins.Add(a.atlas.sha256)&&CoreExtractorPairV21(a.canvasAnchor,256,448),"Core Extractor five unique approved PNGs/root");CoreEntryReadV21(a.atlas);}
   foreach(var binding in index.sprites){
    PreviewRequire(binding!=null&&IsCoreExtractorWholeIdV21(binding.sourceNPC)&&ids.Add(binding.sourceNPC)&&Array.IndexOf(index.sourceNPCs,binding.sourceNPC)>=0,"Core Extractor exact source identity; no ID borrowing");
    var profile=Array.Find(rules,p=>p.id==binding.sourceNPC);var atlas=Array.Find(index.wholeAtlases,a=>a.asset==binding.wholeAsset);
    bool normal=binding.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_A"||binding.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_B";int stateCount=normal?5:4,expectedClips=binding.sourceNPC=="NUI_EL_EXTRACTOR_EARTH"?3:2;float anchorX=normal?17:23,anchorY=normal?78:76;
    PreviewRequire(profile!=null&&profile.kind==binding.id&&profile.states!=null&&profile.states.Length==stateCount&&binding.sourceStates==stateCount&&binding.displayClips==expectedClips&&atlas!=null&&binding.atlas!=null&&binding.atlas.path==atlas.atlas.path&&binding.atlas.sha256==atlas.atlas.sha256&&atlasPins.Contains(binding.atlas.sha256)&&atlas.width==(normal?34:46)&&atlas.height==(normal?78:76)&&CoreExtractorPairV21(atlas.nativeAnchor,anchorX,anchorY),"Core Extractor exact per-ID profile/atlas/anchor");
    PreviewRequire(binding.newDrawingContribution==(binding.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_B"?0:1)&&(!normal||binding.wholeAsset=="normal-ab"&&binding.atlas.sha256=="d1696392e33362c42cd6a10931d7d94fd960143308f48162526f362e4999cf3f"),"Core Extractor NORMAL A/B share only approved body pixels");
    var origin=binding.sourceOriginEvidence;
    PreviewRequire(origin!=null&&CoreExtractorZeroV21(origin.sourceCommonReferenceOffset,3)&&CoreExtractorZeroV21(origin.additionalWholeOffsetApplied,2)&&origin.sourceDisplayRotationY==0&&!origin.sourceOriginAdapterChanged&&origin.sourceCanvas!=null&&origin.sourceCanvas.Length==2&&origin.sourceCanvas[0]==512&&origin.sourceCanvas[1]==512&&CoreExtractorPairV21(origin.sourceCanvasRoot,256,448)&&CoreExtractorPairV21(origin.nativeFrameAnchor,anchorX,anchorY),"Core Extractor source root/facing; no second floor/body offset");
    if(!CoreProductResourcesActiveR1147){CoreEntryReadV21(origin.sourceContract);CoreEntryReadV21(origin.sourceIdentity);}
    var lib=CoreEntryJsonV21<PreviewLibrary>(binding.animations);var detail=CoreEntryJsonV21<CoreExtractorLibraryDetailV21>(binding.animations);
    PreviewRequire(lib!=null&&lib.id==binding.id&&lib.sourceNPC==binding.sourceNPC&&lib.sourceRulesSha256==CoreExtractorRulesShaV21&&lib.logicalScale==1&&!lib.runtimeAdmitted&&!lib.animationLibraryApproved&&lib.paintedPoseCount==1&&lib.clips!=null&&lib.clips.Length==expectedClips&&lib.atlas!=null&&lib.atlas.frameWidth==atlas.width&&lib.atlas.frameHeight==atlas.height&&CoreExtractorPairV21(lib.atlas.groundAnchor,anchorX,anchorY)&&detail!=null&&detail.fixedSourceFacing=="+X"&&!detail.automaticSpawnFacingMirror&&!detail.sourceOriginAdapterChanged&&detail.expectedModelVisualScale==1&&detail.animationLibraryComplete&&detail.artApproved&&detail.sharedPaintedPoseAssetId==binding.sharedPaintedPoseAssetId&&detail.clips!=null&&detail.clips.Length==expectedClips,"Core Extractor exact whole library/scale/fixed +X facing");
    var texture=PreviewTextureFile(binding.atlas);texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;PreviewRequire(texture.width==atlas.width&&texture.height==atlas.height,"Core Extractor approved native PNG dimensions");
    foreach(var clip in lib.clips){
     PreviewRequire(clip!=null&&clip.frames!=null&&clip.frames.Length==1,"Core Extractor compact immutable single frame");var frame=clip.frames[0];
     PreviewRequire(frame!=null&&frame.rect!=null&&frame.rect.Length==4&&frame.rect[0]==0&&frame.rect[1]==0&&frame.rect[2]==atlas.width&&frame.rect[3]==atlas.height&&CoreExtractorPairV21(frame.anchor,anchorX,anchorY),"Core Extractor exact whole rect and native anchor");
     string key=binding.id+"/"+clip.id;PreviewRequire(!coreExtractorWholeSpritesV21.ContainsKey(key),"Core Extractor duplicate display clip");
     var r=frame.rect;var sprite=Sprite.Create(texture,new Rect(r[0],texture.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Extractor whole / "+key;forestPreviewOwned.Add(sprite);spriteOffsets.Add(sprite,new Vector2(r[2]*.5f-frame.anchor[0],frame.anchor[1]-r[3]*.5f));coreExtractorWholeSpritesV21.Add(key,sprite);clipCount++;frameCount++;
    }
    int rows=0;
    foreach(var row in map.rows){
     if(row.sourceNPC!=binding.sourceNPC)continue;rows++;var state=Array.Find(profile.states,s=>s.id==row.state);var clip=Array.Find(lib.clips,c=>c.id==row.displayClip);var sourceClip=Array.Find(detail.clips,c=>c.id==row.displayClip);
     PreviewRequire(state!=null&&row.kind==binding.id&&row.wholeAsset==binding.wholeAsset&&row.modelClip==state.clip&&row.sourceAnimation==state.sourceAnimation&&row.sourceAnimSpeed==state.sourceAnimSpeed&&row.sourceStateDuration==state.duration&&row.sourceLoop==state.loop&&row.useExistingNpcStateAndNpcTime&&row.sourceStateExpirySeparateFromMotionPlayback&&row.offsetAlreadyInFrameAnchor&&row.hostMustNotApplyOffsetAgain&&!row.sourceClockChanged&&row.wholeBodyOffsetY==0&&CoreExtractorPairV21(row.frameAnchor,anchorX,anchorY)&&clip!=null&&sourceClip!=null,"Core Extractor exact original source-state/clock/offset mapping; no idle fallback");
     PreviewRequire(row.sourceAnimation=="Wait"&&row.sourceAnimSpeed==1&&row.sourceMotionDuration==1&&row.libraryDurationSeconds==row.sourceMotionDuration/row.sourceAnimSpeed&&clip.durationSeconds==row.libraryDurationSeconds&&clip.loop==row.sourceLoop&&sourceClip.sourceAnimation==row.sourceAnimation&&sourceClip.sourceAnimSpeed==row.sourceAnimSpeed&&sourceClip.sourceMotionDuration==row.sourceMotionDuration&&sourceClip.durationSeconds==row.libraryDurationSeconds&&sourceClip.loop==row.sourceLoop&&sourceClip.sourceStateDuration==row.sourceStateDuration&&sourceClip.offsetAlreadyInFrameAnchor&&sourceClip.sourceStateExpirySeparateFromMotionPlayback&&CoreExtractorZeroV21(sourceClip.wholeBodyOffset,2)&&sourceClip.sourceStateAliases!=null&&Array.IndexOf(sourceClip.sourceStateAliases,row.state)>=0&&sourceClip.sourceStateExpiries!=null&&Array.Exists(sourceClip.sourceStateExpiries,x=>x.id==row.state&&x.seconds==row.sourceStateDuration&&x.loop==row.sourceLoop),"Core Extractor native motion duration remains separate from model expiry");
     bool authoredShow=!normal&&row.state==binding.sourceNPC.Substring(4)+"_DAMAGE";PreviewRequire(row.hasSourceShow==authoredShow&&row.sourceShow==authoredShow,"Core Extractor preserve exact contract SHOW presence/value");if(row.hasSourceShow)showBindings++;
     if(row.state=="EL_EXTRACTOR_EARTH_START")PreviewRequire(row.sourceStateDuration==2&&row.sourceMotionDuration==1&&row.displayClip=="source-wait-once-state2s"&&!row.sourceLoop,"Core Extractor EARTH START2s state/1s motion hold");
     coreExtractorStateRowsV21.Add(row.sourceNPC+"/"+row.state,row);
    }
    PreviewRequire(rows==stateCount,"Core Extractor exact per-ID state coverage");
   }
   PreviewRequire(ids.Count==6&&coreExtractorStateRowsV21.Count==26&&clipCount==13&&frameCount==13&&coreExtractorWholeSpritesV21.Count==13&&showBindings==4,"Core Extractor selector incomplete");
   CoreExtractorWholeDrawsV21=CoreExtractorMissingStateHoldsV21=CoreExtractorSourceShowApplicationsV21=CoreExtractorSourceShowAbsentPreservationsV21=0;
  }

  // Called by ApplyCoreHardEntryVisibilityV21 after the model/activation gates.
  // Absence of SHOW is a no-op: serialized false is not an authored hide.
  bool ApplyCoreExtractorSourceVisibilityV21(Enemy e,ref bool visible){
   if(e==null||!IsCoreExtractorWholeIdV21(e.sourceNPC)||string.IsNullOrEmpty(e.npcState))return false;
   CoreExtractorStateV21 row;PreviewRequire(coreExtractorStateRowsV21.TryGetValue(e.sourceNPC+"/"+e.npcState,out row),"Core Extractor visibility requires exact current source state");
   CoreExtractorSourceShowLastSourceNPCV21=e.sourceNPC;CoreExtractorSourceShowLastStateV21=e.npcState;CoreExtractorSourceShowLastAuthoredV21=row.hasSourceShow;CoreExtractorSourceShowLastValueV21=row.sourceShow;CoreExtractorSourceShowLastVisibleBeforeV21=visible;
   if(row.hasSourceShow){visible&=row.sourceShow;CoreExtractorSourceShowApplicationsV21++;}else CoreExtractorSourceShowAbsentPreservationsV21++;
   CoreExtractorSourceShowLastVisibleAfterV21=visible;return row.hasSourceShow;
  }
  bool RenderCoreExtractorWholeV21(Enemy e,SpriteRenderer s){
   if(!IsCoreExtractorWholeIdV21(e.sourceNPC))return false;
   // Preserve R9 nonmanaged NORMAL_A/B source-entry display holds verbatim.
   // No source state/clock or original visibility predicate is initialized here.
   if((e.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_A"||e.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_B")&&string.IsNullOrEmpty(e.npcState)){s.enabled=false;s.sprite=null;CoreExtractorMissingStateHoldsV21++;return true;}
   if(!TryCoreWholeDisplayV44(e,s,out var displayState,out var displayTime)){CoreExtractorMissingStateHoldsV21++;return true;}
   CoreExtractorStateV21 row;PreviewRequire(coreExtractorStateRowsV21.TryGetValue(e.sourceNPC+"/"+displayState,out row),"Core Extractor unknown exact state; no idle fallback or ID borrowing");
   PreviewRequire(e.kind==row.kind&&e.visualScale==1&&!float.IsNaN(e.npcTime)&&!float.IsInfinity(e.npcTime)&&e.npcTime>=0,"Core Extractor original actor identity/scale/clock");
   float sourceClock=displayTime*row.sourceAnimSpeed;sourceClock=row.sourceLoop?Mathf.Repeat(sourceClock,row.sourceMotionDuration):Mathf.Min(sourceClock,row.sourceMotionDuration);
   RecordMonsterPixelSourceV53(e,s,"Core/Extractor",row.displayClip,row.sourceAnimation,sourceClock,1);
   // The approved whole is held at frame0. The derived playback clock is probe
   // evidence only; npcState/npcTime, state expiry, facing and gameplay are untouched.
   s.enabled=true;s.sprite=coreExtractorWholeSpritesV21[row.kind+"/"+row.displayClip];s.flipX=s.flipY=false;s.transform.localScale=Vector3.one;s.transform.localRotation=Quaternion.identity;var p=RenderEnemyPosition(e)+SpriteOffset(s);s.transform.localPosition=new Vector3(p.x,p.y,0);
   CoreExtractorWholeLastSourceNPCV21=e.sourceNPC;CoreExtractorWholeLastStateV21=displayState;CoreExtractorWholeLastDisplayClipV21=row.displayClip;CoreExtractorWholeLastAssetV21=row.wholeAsset;CoreExtractorWholeLastNpcClockV21=displayTime;CoreExtractorWholeLastMotionPlaybackClockV21=row.sourceLoop?Mathf.Repeat(displayTime,row.libraryDurationSeconds):Mathf.Min(displayTime,row.libraryDurationSeconds);CoreExtractorWholeLastStateDurationV21=row.sourceStateDuration;CoreExtractorWholeLastMotionDurationV21=row.sourceMotionDuration;CoreExtractorWholeLastSourceLoopV21=row.sourceLoop;CoreExtractorWholeLastHasSourceShowV21=row.hasSourceShow;CoreExtractorWholeLastSourceShowV21=row.sourceShow;CoreExtractorWholeLastFrameAnchorV21=row.frameAnchor;CoreExtractorWholeDrawsV21++;return true;
  }
  void DisposeCoreExtractorWholeV21(){foreach(var s in coreExtractorWholeSpritesV21.Values)spriteOffsets.Remove(s);coreExtractorWholeSpritesV21.Clear();coreExtractorStateRowsV21.Clear();}
 }
}

