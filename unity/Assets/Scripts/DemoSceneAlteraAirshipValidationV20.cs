#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool airshipControlledValidationV20;
  int airshipValidationRoomV20=-1;float airshipValidationRoomBeganV20;
  [Serializable] sealed class AirshipValidationCountersV20 {
   public string schema="altera-airship-natural-presentation-counters-v20";
   public int stateDraws,stateKinds,stateSequences,repairRequests,maximumCameraRequests,directorStarts,directorDying,directorDraws,directorCameraFrames,directorRestores;
   public long particleBirths;public AirshipDirectorCaptionRecordV20[] captions;
  }
  void CaptureAlteraAirshipValidationV20(){
   if(!alteraAirshipPreviewSessionV20)return;
   if(airshipValidationRoomV20!=game.room){airshipValidationRoomV20=game.room;airshipValidationRoomBeganV20=game.elapsed;}
   foreach(int age in new[]{3,10})if(game.elapsed-airshipValidationRoomBeganV20>=age&&forestAutoEffectCaptures.Add("airship-room-"+game.room+"-"+age))ForestAutoCapture("airship-room-"+(game.room+1)+"-stable-"+age+"s");
   if(AlteraAirshipDirectorActiveV20){
    foreach(float age in new[]{0f,5f,14.3f})if(AlteraAirshipDirectorAgeV20>=age&&forestAutoEffectCaptures.Add("airship-director-"+age))ForestAutoCapture("airship-director-"+age.ToString("0.0",System.Globalization.CultureInfo.InvariantCulture)+"s");
   }
  }
  void SaveAlteraAirshipValidationCountersV20(){
   if(!alteraAirshipPreviewSessionV20)return;
   var result=new AirshipValidationCountersV20{stateDraws=AlteraAirshipStateDrawFramesV20,stateKinds=AlteraAirshipStateKindsDrawnV20,stateSequences=AlteraAirshipStateCreatedSequencesV20,repairRequests=AlteraAirshipStateRepairRequestsV20,maximumCameraRequests=AlteraAirshipStateMaximumCameraRequestsV20,particleBirths=AlteraAirshipStateParticleBirthsV20,directorStarts=AlteraAirshipDirectorSourceStartsV20,directorDying=AlteraAirshipDirectorSourceDyingV20,directorDraws=AlteraAirshipDirectorSkinFramesV20,directorCameraFrames=AlteraAirshipDirectorCameraFramesV20,directorRestores=AlteraAirshipDirectorRestoreCountV20,captions=AlteraAirshipDirectorCaptionsV20.ToArray()};
   File.WriteAllText(Path.Combine(forestAutoOutput,"airship-presentation-counters.json"),JsonUtility.ToJson(result,true));
  }

  // The first real load cannot truthfully claim a preceding load pass. This
  // batch-only entry requires the actual scoped component proofs and keeps
  // external playableReady/runtimeIntegrationVerified admission closed.
  public void BeginAlteraAirshipControlledValidationV20(string path){
   ValidateAlteraAirshipControlledValidationV20(path);
   if(forestPreviewActive||forestPreviewLoading)EndForestPreview();
   alteraAirshipPreviewSessionV20=true;airshipControlledValidationV20=true;
   try{BeginSourceDungeonPreview(path,false);}catch{alteraAirshipPreviewSessionV20=false;airshipControlledValidationV20=false;throw;}
  }

  static void ValidateAlteraAirshipControlledValidationV20(string path){
   PreviewRequire(Application.isBatchMode&&Application.isPlaying,"Airship controlled validation requires actual batch PlayMode");
   var inspection=InspectAlteraAirshipPreviewCandidateV20(path);
   PreviewRequire(inspection.passed&&!inspection.playableReady&&!inspection.visualGateOpen,"Controlled first-load candidate must retain external admission closed");
   var text=File.ReadAllText(path);var b=JsonUtility.FromJson<PreviewBundle>(text);var scope=JsonUtility.FromJson<AirshipPreviewScopeV20>(text);
   var visual=AirshipJsonV20<AirshipVisualIntegrationV20>(scope.visualIntegration);
   PreviewRequire(visual.stateConsumerImplemented&&visual.directorConsumerImplemented&&visual.stateRuntime!=null&&visual.directorRuntime!=null&&visual.componentEvidence!=null&&visual.componentEvidence.Length==2,"Actual state and director component proofs required before first load");
   AirshipReadV20(visual.stateRuntime);AirshipReadV20(visual.directorRuntime);
   foreach(var file in visual.componentEvidence){
    var proof=AirshipJsonV20<AirshipComponentGateV20>(file);
    PreviewRequire(proof.schema=="altera-airship-actual-component-gate-v20"&&proof.passed&&proof.actualUnityComponent&&!proof.runtimeAdmitted&&proof.sourceRulesSha256==b.rules.sha256&&proof.sourceEffectsSha256==b.effects.sha256,"Controlled entry requires actual component/source proof");
   }
  }
 }
}
#endif
