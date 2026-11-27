#if UNITY_EDITOR
using System;using System.Collections;using System.IO;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 sealed class CoreEntryAdmittedSnapshotV21 {internal int selected,preview;internal string[] loadout;internal bool paused,adventure,pausedMenu;internal string menu,origin;internal Rect viewport;}
 readonly CoreEntryBeginOwnershipV21 coreEntryBeginOwnerV21=new CoreEntryBeginOwnershipV21();
 CoreEntryAdmittedSnapshotV21 coreEntryAdmittedSnapshotV21;IEnumerator coreEntryOwnedLoadWorkV21;
 // Private Editor-only fault points; default empty. They never affect model clocks.
 string coreEntryFaultPointV21;
 void CoreEntryFaultV21(string point){coreLifecycleObserveV24?.Invoke(point);if(coreEntryFaultPointV21==point)throw new InvalidDataException("Core controlled injected load failure: "+point);}
 void BeginCoreHardOwnedEntryV21(string path,bool componentRoom4R908=false){
  // Reject all synchronous policy cases before closing a preview or publishing Core ownership.
  PreviewRequire(Application.isPlaying,"Core controlled entry needs PlayMode");
  PreviewRequire(!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview,"Core controlled entry cannot combine with AutoTest/Calibration/Review");
  string full=Path.GetFullPath(path);var check=InspectCoreHardControlledEntryV21(full);PreviewRequire(check.installedSourcesMatch,"Core controlled entry needs exact installed candidate");
  if(forestPreviewActive||forestPreviewLoading)EndForestPreview();
  coreEntryBeginOwnerV21.Run(()=>{
   var snap=new CoreEntryAdmittedSnapshotV21{selected=selectedDungeon,preview=previewDungeon,paused=game.paused,adventure=adventureStarted,menu=openMenu,origin=menuOrigin,pausedMenu=pausedBeforeMenu,viewport=worldCamera.rect,loadout=new string[GameModel.SkillSlotCount]};
   for(int i=0;i<snap.loadout.Length;i++)snap.loadout[i]=game.GetEquippedSkill(i).id;
   coreEntryAdmittedSnapshotV21=snap;
  },()=>{
   CoreEntryFaultV21("after-snapshot");coreRoom4ComponentRequestedR908=componentRoom4R908;coreRoom4ComponentInitializedR908=false;coreRoom4ComponentInitFrameR908=-1;coreEntrySessionV21=true;CoreEntryFaultV21("before-common-begin");BeginSourceDungeonPreview(full,false);
   if(!coreEntrySessionV21&&!string.IsNullOrEmpty(forestPreviewError))throw new InvalidDataException(forestPreviewError);
  },()=>{
   // No owned loader/model exists: preserve the admitted GameModel and its current actors.
   var snap=coreEntryAdmittedSnapshotV21;coreRoom4ComponentRequestedR908=coreRoom4ComponentInitializedR908=false;coreRoom4ComponentInitFrameR908=-1;coreRoom4ComponentInitModelR908=0;coreRoom4ComponentInitStepR908=0;coreEntrySessionV21=false;coreEntryScopeV21=null;coreEntryAdmittedSnapshotV21=null;
   if(snap!=null){selectedDungeon=snap.selected;previewDungeon=snap.preview;game.paused=snap.paused;adventureStarted=snap.adventure;openMenu=snap.menu;menuOrigin=snap.origin;pausedBeforeMenu=snap.pausedMenu;worldCamera.rect=snap.viewport;}
  },()=>{
   // Loading-owned means the shared captured original dungeon/loadout is valid.
   // Ordinary End restores admitted definitions/rooms; pending rollback never calls it.
   EndForestPreview();
  });
 }
 void MarkCoreHardEntryLoadOwnedV21(){if(!coreEntrySessionV21)return;coreEntryBeginOwnerV21.MarkLoadingOwned();CoreEntryFaultV21("after-load-owned");}
 IEnumerator CreateCoreHardEntryOwnedLoadV21(){if(coreEntryOwnedLoadWorkV21!=null)throw new InvalidOperationException("Core loader already owned");coreEntryOwnedLoadWorkV21=CoreLifecycleWrapOwnedWorkV24(LoadCoreHardControlledEntryV21());return coreEntryOwnedLoadWorkV21;}
 void MarkCoreHardEntryActiveV21(){coreEntryBeginOwnerV21.MarkActive();}
 void DisposeCoreHardEntryOwnedLoadV21(){var work=coreEntryOwnedLoadWorkV21;coreEntryOwnedLoadWorkV21=null;try{(work as IDisposable)?.Dispose();}finally{var snap=coreEntryAdmittedSnapshotV21;if(snap!=null&&worldCamera)worldCamera.rect=snap.viewport;coreEntryBeginOwnerV21.Reset();coreEntryAdmittedSnapshotV21=null;coreRoom4ComponentRequestedR908=coreRoom4ComponentInitializedR908=false;coreRoom4ComponentInitFrameR908=-1;coreRoom4ComponentInitModelR908=0;coreRoom4ComponentInitStepR908=0;}}
 public string CoreHardEntryOwnershipStageV21 {get{return coreEntryBeginOwnerV21.Phase.ToString();}}
}}
#endif
