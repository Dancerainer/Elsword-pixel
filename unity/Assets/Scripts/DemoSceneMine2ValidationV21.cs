#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool mine2ControlledValidationV21;
  int mine2ValidationRoomV21=-1;float mine2ValidationRoomBeganV21;
  readonly System.Collections.Generic.HashSet<string> mine2ValidationKindsV21=new System.Collections.Generic.HashSet<string>();
  const float Mine2ValidationDeadlineV21=1800;
  [Serializable] sealed class Mine2ValidationCountersV21 {
   public string schema="mine2-natural-presentation-counters-v21",scope="Observed during ordinary input/60Hz Step. Counters do not prove every original callback or template. Missing original callbacks and held templates remain held.";
   public bool actualPlayMode,inputOnly=true,runtimeAdmitted=false,naturalRouteVerified=false,musicPolicyUserAuthorized=true;
   public int drawFrames,mainMeshDrawFrames,createdSequences,heldEvents,particleKinds,pool,presentationGeneration,mechanismEvents,missileEvents,worldDrawFrames,worldAllocations,worldEpochs,worldViewportHolds,pitDefeats;
   public string[] observedNpcStates;public string missingOriginalBgm="Besma_Besma_Mine2_Stage2A.ogg",authorizedRoom2Music="Music020_Stage.ogg";
   public string bundleSha256;public float modelSeconds;public string[] selectedMusicByRoom={"Music020_Stage.ogg","Music020_Stage.ogg (user-authorized inherited; original Besma_Besma_Mine2_Stage2A.ogg missing)","Music011_Boss.ogg"};
  }
  // Read-only preflight for the batch launcher. It deliberately does not grant public readiness.
  public static Mine2PreviewInspectionV21 VerifyMine2ControlledCandidateV21(string path){
   var r=InspectMine2PreviewCandidateV21(path);var text=File.ReadAllText(path);var b=JsonUtility.FromJson<PreviewBundle>(text);var s=JsonUtility.FromJson<Mine2ScopeV21>(text);
   PreviewRequire(r.passed&&!r.playableReady&&!r.visualGateOpen&&!s.runtimeIntegrationVerified&&s.actualAssetsIntegrated&&!s.naturalRunVerified&&r.musicPolicyAuthorized&&r.pendingArtLibraries==0,"Mine2 first-load candidate must keep public admission and natural verification closed");
   Mine2CheckActualIntegrationsV21(b,s);r.componentProofsVerified=true;return r;
  }
  static void ValidateMine2ControlledValidationV21(string path){
   PreviewRequire(Application.isBatchMode&&Application.isPlaying,"Mine2 first validation requires actual batch PlayMode");VerifyMine2ControlledCandidateV21(path);
  }
  public void BeginMine2ControlledValidationV21(string path){
   ValidateMine2ControlledValidationV21(path);if(forestPreviewActive||forestPreviewLoading)EndForestPreview();
   mine2PreviewSessionV21=true;mine2ControlledValidationV21=true;
   try{BeginSourceDungeonPreview(path,false);}catch{mine2PreviewSessionV21=false;mine2ControlledValidationV21=false;throw;}
  }
  void CaptureMine2ValidationV21(){
   if(!mine2PreviewSessionV21)return;
   if(mine2ValidationRoomV21!=game.room){mine2ValidationRoomV21=game.room;mine2ValidationRoomBeganV21=game.elapsed;}
   foreach(int age in new[]{3,10})if(game.elapsed-mine2ValidationRoomBeganV21>=age&&forestAutoEffectCaptures.Add("mine2-room-"+game.room+"-"+age))ForestAutoCapture("mine2-room-"+(game.room+1)+"-stable-"+age+"s");
   foreach(var e in game.enemies){if(!e.active||string.IsNullOrEmpty(e.npcState))continue;mine2ValidationKindsV21.Add(e.sourceNPC+"/"+e.npcState);
    if(e.sourceNPC=="NUI_WALLY_8TH_MK2"&&e.hp>0&&e.aiEnabled&&e.npcState.StartsWith("WALLY_8TH_MK2_ATTACK_",StringComparison.Ordinal)&&!e.npcState.EndsWith("READY",StringComparison.Ordinal)&&worldCamera&&worldRoot){var p=worldCamera.WorldToViewportPoint(worldRoot.TransformPoint(new Vector3(e.x,e.y+60,0)));if(p.z>0&&p.x>0&&p.x<1&&p.y>0&&p.y<1&&forestAutoEffectCaptures.Add("mine2-wally-first-ordinary-combat-camera-visible"))ForestAutoCapture("mine2-wally-first-ordinary-combat-camera-visible");}
    bool visible=Math.Abs(e.x-game.player.x)<400;bool eventState=e.sourceNPC=="NUI_DOWN_STONE_TRAP_BESMA"&&(e.npcState=="DOWN_STONE_TRAP_BESMA_START"||e.npcState=="DOWN_STONE_TRAP_BESMA_ATTACK"||e.npcState=="DOWN_STONE_TRAP_BESMA_BROKEN")||e.sourceNPC=="NUI_WALLY_8TH_MK2"&&(e.npcState=="WALLY_8TH_MK2_ATTACK_B"||e.npcState=="WALLY_8TH_MK2_ATTACK_C"||e.npcState=="WALLY_8TH_MK2_SPECIAL_ATTACK_A");
    if(visible&&eventState&&forestAutoEffectCaptures.Add("mine2-state-"+e.npcState))ForestAutoCapture("mine2-natural-"+e.npcState);
   }
   CaptureMine2StallV21();
  }
  void CaptureMine2StallV21(){
   float idle=Time.realtimeSinceStartup-forestAutoProgressAt;if(idle>15&&forestAutoEffectCaptures.Add("mine2-stall-"+game.room+"-"+game.hits+"-"+game.kills))ForestAutoCapture("mine2-room-"+(game.room+1)+"-stalled");
   if(idle>90)FinishForestAutoRun("stalled","No movement/hit/kill progress for90seconds; actual state captured, without changing simulation.",true);
  }
  void SaveMine2ValidationCountersV21(){
   if(!mine2PreviewSessionV21)return;var states=new string[mine2ValidationKindsV21.Count];mine2ValidationKindsV21.CopyTo(states);Array.Sort(states,StringComparer.Ordinal);
   var r=new Mine2ValidationCountersV21{actualPlayMode=Application.isPlaying,drawFrames=Mine2VisualDrawFramesV21,mainMeshDrawFrames=Mine2VisualMainMeshDrawFramesV21,createdSequences=Mine2VisualCreatedSequencesV21,heldEvents=Mine2VisualHeldEventsV21,particleKinds=Mine2VisualKindsDrawnV21,pool=Mine2VisualPoolCountV21,worldDrawFrames=Mine2WorldLayerDrawFramesV20,worldAllocations=Mine2WorldLayerAllocationsV20,worldEpochs=Mine2WorldLayerWorldEpochsObservedV20,worldViewportHolds=Mine2WorldLayerViewportHoldsV20,pitDefeats=game.Mine2VoidDefeatsV21,presentationGeneration=game.Mine2PresentationGenerationV21,mechanismEvents=game.mine2MechanismEventsV21.Count,missileEvents=game.mine2SourceEventsV20.Count,observedNpcStates=states,bundleSha256=forestPreviewBundleHash,modelSeconds=game.elapsed};
   File.WriteAllText(Path.Combine(forestAutoOutput,"mine2-presentation-counters.json"),JsonUtility.ToJson(r,true));
  }
 }
}
#endif
