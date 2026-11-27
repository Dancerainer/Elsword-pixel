#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class Mine2EntryIntegrationV21 {
   public string schema,sourceRulesSha256,sourceEffectsSha256;
   public bool passed,actualAssetsIntegrated,actualSharedHooksVerified,hookInjection,runtimeAdmitted;
   public PreviewFile runtimeBinding,executionBinding,sharedSourceCompatibility;public PreviewFile[] evidence;
  }
  [Serializable] sealed class Mine2EntryMusicPolicyV21 {
   public string schema,authorization,missingLiteral,selectedMusicId,resource,note;
   public int room,inheritFromRoom;public bool userAuthorized,sourceDeclarationsChanged;
   public PreviewFile literalAudit,sourceModel,sourcePreservation,selectedFile;
  }
  [Serializable] sealed class Mine2EntrySuiteV21 {public string name;public PreviewFile report,executable;public int exitCode;}
  [Serializable] sealed class Mine2EntryStepProofV21 {public bool passed,actualGameModelStep,actualAssetsIntegrated,hookInjection,naturalRoute;public int checks;}
  [Serializable] sealed class Mine2EntryModelExecutionV21 {public string schema;public bool passed,actualAssetsIntegrated,hookInjection,sourcesUnchanged,naturalRoute;public PreviewFile[] sources;public Mine2EntrySuiteV21[] suites;public PreviewFile sourceArchive;}
  [Serializable] sealed class Mine2EntryVisualExecutionV21 {public string schema;public bool passed,actualUnityExecuted,actualAssetsIntegrated,hookInjection,sourcesUnchanged,runtimeAdmitted,naturalRoute;public PreviewFile report,binding,sourceArchive,unityLog;public int unityExitCode;public PreviewFile[] sources,dependencies;}
  [Serializable] sealed class Mine2EntryWorldProofV21 {public string schema,failure;public bool passed,actualUnityComponent,naturalRoute,runtimeAdmitted;public int checks,drawFrames;}
  [Serializable] sealed class Mine2EntryCompositionFileV21 {public PreviewFile source;public string afterSha256;}
  [Serializable] sealed class Mine2EntryCompositionV21 {public string schema;public bool appliedToAssets;public Mine2EntryCompositionFileV21[] files;}
  [Serializable] sealed class Mine2EntryShotV21 {public string name,path,sha256;public int changedPixels;}
  [Serializable] sealed class Mine2EntryVisualProofV21 {public string schema,failure;public bool passed,controlledFixture,actualUnityExecuted,naturalRoute,runtimeAdmitted;public int checks,drawFrames,mainMeshDrawFrames,createdSequences,kinds;public Mine2EntryShotV21[] captures;}
  static void Mine2CheckMusicPolicyV21(Mine2ScopeV21 s){
   var p=Mine2JsonV21<Mine2EntryMusicPolicyV21>(s.musicPolicy);
   PreviewRequire(p.schema=="mine2-user-authorized-music-policy-v21"&&p.userAuthorized&&!p.sourceDeclarationsChanged&&p.authorization=="human-user-explicit-room2-inherit-room1-mining-bgm"&&p.room==1&&p.inheritFromRoom==0&&p.missingLiteral=="Besma_Besma_Mine2_Stage2A.ogg"&&p.selectedMusicId=="music020_stage"&&p.resource=="Pixel/music/music020_stage"&&Mine2SameV21(p.literalAudit,s.bgmLiteralAudit)&&Mine2SameV21(p.sourceModel,s.sourceModel)&&Mine2SameV21(p.sourcePreservation,s.sourcePreservation)&&Mine2SameV21(p.selectedFile,s.originalMusic[0].file),"Mine2 music policy is not the exact user-authorized inheritance");
   Mine2ReadV21(p.selectedFile);Mine2ReadV21(p.literalAudit);
  }
  static string Mine2ResolveMusicPolicyV21(Mine2ScopeV21 s,int room){
   Mine2CheckMusicPolicyV21(s);if(room==1)return Mine2JsonV21<Mine2EntryMusicPolicyV21>(s.musicPolicy).selectedMusicId;
   var original=s.originalMusic[room];PreviewRequire(!original.originalFileMissing&&!original.replacementAssigned&&original.file!=null,"Mine2 existing original music changed");Mine2ReadV21(original.file);return original.id;
  }
  static void Mine2CheckActualIntegrationsV21(PreviewBundle b,Mine2ScopeV21 s){
   var model=Mine2JsonV21<Mine2EntryIntegrationV21>(s.modelIntegration);var visual=Mine2JsonV21<Mine2EntryIntegrationV21>(s.visualIntegration);
   foreach(var p in new[]{model,visual}){
    PreviewRequire(p.passed&&p.actualAssetsIntegrated&&p.actualSharedHooksVerified&&!p.hookInjection&&!p.runtimeAdmitted&&p.sourceRulesSha256==b.rules.sha256&&p.sourceEffectsSha256==b.effects.sha256&&p.runtimeBinding!=null&&p.executionBinding!=null&&p.evidence!=null&&p.evidence.Length>0,"Mine2 installed integration flags/source closure missing");
    Mine2ReadV21(p.runtimeBinding);foreach(var f in p.evidence)Mine2ReadV21(f);
   }
   PreviewRequire(model.schema=="mine2-actual-source-model-integration-v21"&&visual.schema=="mine2-actual-source-visual-integration-v21","Mine2 integration schema");
   var m=Mine2JsonV21<Mine2EntryModelExecutionV21>(model.executionBinding);
   PreviewRequire(m.schema=="mine2-installed-model-execution-v21"&&m.passed&&m.actualAssetsIntegrated&&!m.hookInjection&&m.sourcesUnchanged&&!m.naturalRoute&&m.sources!=null&&m.sources.Length>=58&&m.suites!=null&&m.suites.Length==2,"Mine2 actual installed model proof missing");
   var shared=Mine2CheckBelowSharedCompatibilityV21(model.sharedSourceCompatibility);
   foreach(var p in m.sources)Mine2ReadHistoricalComponentSourceV21(p,shared);Mine2ReadV21(m.sourceArchive);int checks=0;var names=new System.Collections.Generic.HashSet<string>();
   foreach(var row in m.suites){var r=Mine2JsonV21<Mine2EntryStepProofV21>(row.report);PreviewRequire(names.Add(row.name)&&row.exitCode==0&&r.passed&&r.actualGameModelStep&&r.actualAssetsIntegrated&&!r.hookInjection&&!r.naturalRoute&&(row.name=="mechanisms"&&r.checks==431||row.name=="missiles"&&r.checks==137),"Mine2 installed source suite rejected");checks+=r.checks;Mine2ReadV21(row.executable);}PreviewRequire(checks==568,"Mine2 actual568 checks required");
   var v=Mine2JsonV21<Mine2EntryVisualExecutionV21>(visual.executionBinding);
   PreviewRequire(v.schema=="mine2-installed-visual-execution-v21"&&v.passed&&v.actualUnityExecuted&&v.actualAssetsIntegrated&&!v.hookInjection&&v.sourcesUnchanged&&!v.runtimeAdmitted&&!v.naturalRoute&&Mine2SameV21(v.binding,visual.runtimeBinding)&&v.sources!=null&&v.sources.Length>=10,"Mine2 actual scoped Unity/source binder missing");
   // The new entry changes only the explicitly composed generic preview/render
   // dispatch sites. Component source archives describe the preceding actual
   // tests, not execution of these new entry files. Natural validation freezes
   // the complete installed entry separately; there is no candidate/pins cycle.
   Mine2CheckComponentSourcesV21(v,s,shared);PreviewRequire(v.dependencies!=null&&v.dependencies.Length>0,"Mine2 visual data input pins absent");foreach(var p in v.dependencies)Mine2ReadV21(p);Mine2ReadV21(v.sourceArchive);Mine2ReadV21(v.unityLog);PreviewRequire(v.unityExitCode==0,"Mine2 actual Unity failed exit");
   var proof=Mine2JsonV21<Mine2EntryVisualProofV21>(v.report);PreviewRequire(proof.schema=="mine2-controlled-visual-v21"&&proof.passed&&proof.controlledFixture&&proof.actualUnityExecuted&&!proof.naturalRoute&&!proof.runtimeAdmitted&&string.IsNullOrEmpty(proof.failure)&&proof.checks>=48&&proof.drawFrames>0&&proof.mainMeshDrawFrames>0&&proof.createdSequences>0&&proof.captures!=null&&proof.captures.Length>=20,"Mine2 actual Unity visual proof failed or incomplete");
   var shots=new System.Collections.Generic.HashSet<string>();foreach(var p in proof.captures){PreviewRequire(shots.Add(p.name)&&p.changedPixels>0,"Mine2 visual proof contains empty/duplicate capture");Mine2ReadV21(new PreviewFile{path=p.path,sha256=p.sha256});}foreach(var name in new[]{"main-missile-up","main-missile-steer","main-laser","stone-warning","stone-landing-source-fields","wally-hand-boost","rail-source-dust-smoke"})PreviewRequire(shots.Contains(name),"Mine2 scoped actual capture absent "+name);
   var world=Mine2JsonV21<Mine2EntryIntegrationV21>(s.worldIntegration);PreviewRequire(world.schema=="mine2-root-worldlayer-entry-binding-v21"&&world.sourceRulesSha256==b.rules.sha256&&world.sourceEffectsSha256==b.effects.sha256&&world.runtimeBinding!=null&&world.executionBinding!=null,"Mine2 root world binding absent");Mine2ReadV21(world.runtimeBinding);
   var w=Mine2JsonV21<Mine2EntryVisualExecutionV21>(world.executionBinding);PreviewRequire(w.schema=="mine2-installed-world-execution-v21"&&w.passed&&w.actualUnityExecuted&&w.actualAssetsIntegrated&&w.sourcesUnchanged&&!w.hookInjection&&!w.runtimeAdmitted&&!w.naturalRoute&&w.unityExitCode==0&&Mine2SameV21(w.binding,world.runtimeBinding),"Mine2 world component execution binder rejected");Mine2CheckComponentSourcesV21(w,s,shared);foreach(var p in w.dependencies)Mine2ReadV21(p);Mine2ReadV21(w.sourceArchive);Mine2ReadV21(w.unityLog);var wp=Mine2JsonV21<Mine2EntryWorldProofV21>(w.report);PreviewRequire(wp.schema=="mine2-worldlayer-actual-component-v21"&&wp.passed&&wp.actualUnityComponent&&!wp.naturalRoute&&!wp.runtimeAdmitted&&wp.checks>=17&&wp.drawFrames>0&&string.IsNullOrEmpty(wp.failure),"Mine2 actual world component failed");
  }
  static void Mine2CheckComponentSourcesV21(Mine2EntryVisualExecutionV21 proof,Mine2ScopeV21 scope,Mine2BelowCompatibilityV21 shared){
   // Read the composition from the already exact-hash-pinned current bundle.
   var c=Mine2JsonV21<Mine2EntryCompositionV21>(scope.entrySourceComposition);
   PreviewRequire(c.schema=="mine2-entry-exact-source-composition-v21"&&!c.appliedToAssets&&c.files!=null&&c.files.Length==9,"Mine2 bounded entry composition changed");
   foreach(var p in proof.sources){var row=Array.Find(c.files,x=>x.source.path==p.path);if(row==null)Mine2ReadHistoricalComponentSourceV21(p,shared);else{PreviewRequire(row.source.sha256==p.sha256,"Mine2 component baseline differs from entry hook baseline");Mine2ReadHistoricalComponentSourceV21(new PreviewFile{path=p.path,sha256=row.afterSha256},shared);}}
  }
  [Serializable] sealed class Mine2BelowCompatibilityRowV21 {public PreviewFile previousSource,currentSource,beforeSnapshot,afterSnapshot;}
  [Serializable] sealed class Mine2BelowCompatibilityV21 {
   public string schema;public bool passed,actualAssetsIntegrated,actualUnityComponentsVerified,hookInjection,naturalRoute,currentSharedRevisionNaturalRunVerified;
   public PreviewFile previousReadyBundle,installReceipt,actualModel,actualComponents,plan;public Mine2BelowCompatibilityRowV21[] rows;
  }
  [Serializable] sealed class Mine2BelowPlanRowV21 {public string file,target,kind;public PreviewFile before,after;}
  [Serializable] sealed class Mine2BelowPlanV21 {public string schema;public Mine2BelowPlanRowV21[] rows;}
  [Serializable] sealed class Mine2BelowInstallV21 {public bool passed,actualAssetsInstalled;public PreviewFile plan;public PreviewFile[] installedFiles;}
  [Serializable] sealed class Mine2BelowModelV21 {
   public string schema,executionMode;public bool passed,actualAssetsIntegrated,hookInjection,snapshotComposition,naturalRoute,sourcesUnchanged,inputsUnchanged;
   public int checks,mine2RegressionChecks;public PreviewFile plan,sourceArchive;public PreviewFile[] installedSourcesBefore;public Mine2EntrySuiteV21[] suites;
  }
  [Serializable] sealed class Mine2BelowComponentsV21 {public string schema;public bool passed,actualAssetsIntegrated,actualUnityExecuted,hookInjection,naturalRoute,sourcesUnchanged;public int unityExitCode;public PreviewFile componentExecution,sourceArchive,unityLog;public PreviewFile[] sources;}
  [Serializable] sealed class Mine2BelowUnityV21 {public bool passed,actualAssetsIntegrated,actualUnityExecuted,hookInjection,naturalRoute;public int candidateExitCode,visibilityExitCode;public PreviewFile candidateReport,visibilityReport;}
  [Serializable] sealed class Mine2BelowCountV21 {public bool passed;public int checks;}
  static Mine2BelowCompatibilityV21 Mine2CheckBelowSharedCompatibilityV21(PreviewFile file){
   PreviewRequire(file!=null&&file.sha256==Mine2PreviewPinsV21.SharedSourceCompatibility,"Mine2 exact shared-source compatibility revision required");
   var c=Mine2JsonV21<Mine2BelowCompatibilityV21>(file);
   PreviewRequire(c.schema=="mine2-below-exact-shared-source-compatibility-v21"&&c.passed&&c.actualAssetsIntegrated&&c.actualUnityComponentsVerified&&!c.hookInjection&&!c.naturalRoute&&!c.currentSharedRevisionNaturalRunVerified&&c.rows!=null&&c.rows.Length==11,"Mine2 shared compatibility scope changed");
   Mine2ReadV21(c.previousReadyBundle);var plan=Mine2JsonV21<Mine2BelowPlanV21>(c.plan);var install=Mine2JsonV21<Mine2BelowInstallV21>(c.installReceipt);
   PreviewRequire(plan.schema=="below-tunnel-combined-exact-install-plan-v21"&&plan.rows!=null&&plan.rows.Length==20&&install.passed&&install.actualAssetsInstalled&&Mine2SameV21(install.plan,c.plan)&&install.installedFiles!=null&&install.installedFiles.Length==20,"Mine2 Below exact installed20 lineage required");
   var m=Mine2JsonV21<Mine2BelowModelV21>(c.actualModel);var v=Mine2JsonV21<Mine2BelowComponentsV21>(c.actualComponents);
   PreviewRequire(m.schema=="below-tunnel-combined-model-execution-v21"&&m.passed&&m.actualAssetsIntegrated&&!m.hookInjection&&!m.snapshotComposition&&!m.naturalRoute&&m.executionMode=="actual"&&m.sourcesUnchanged&&m.inputsUnchanged&&m.checks==4619&&m.mine2RegressionChecks==568&&Mine2SameV21(m.plan,c.plan)&&m.installedSourcesBefore!=null&&m.suites!=null,"Mine2 current shared model execution is not actual4619+568");
   PreviewRequire(v.schema=="below-installed-components-root-v21"&&v.passed&&v.actualAssetsIntegrated&&v.actualUnityExecuted&&!v.hookInjection&&!v.naturalRoute&&v.sourcesUnchanged&&v.unityExitCode==0&&v.sources!=null,"Mine2 current shared Unity components rejected");
   Mine2ReadV21(m.sourceArchive);foreach(var suite in m.suites){PreviewRequire(suite.exitCode==0,"Mine2 current shared model suite failed");Mine2ReadV21(suite.report);Mine2ReadV21(suite.executable);}
   Mine2ReadV21(v.sourceArchive);Mine2ReadV21(v.unityLog);var u=Mine2JsonV21<Mine2BelowUnityV21>(v.componentExecution);
   PreviewRequire(u.passed&&u.actualAssetsIntegrated&&u.actualUnityExecuted&&!u.hookInjection&&!u.naturalRoute&&u.candidateExitCode==0&&u.visibilityExitCode==0,"Mine2 current actual Unity component execution failed");
   var a=Mine2JsonV21<Mine2BelowCountV21>(u.candidateReport);var b=Mine2JsonV21<Mine2BelowCountV21>(u.visibilityReport);PreviewRequire(a.passed&&a.checks==250&&b.passed&&b.checks==603,"Mine2 current actual Unity250+603 reports missing");
   var unique=new System.Collections.Generic.HashSet<string>();
   foreach(var r in c.rows){
    PreviewRequire(r.previousSource!=null&&r.currentSource!=null&&unique.Add(r.currentSource.path)&&r.previousSource.path==r.currentSource.path,"Mine2 duplicate/path-shift shared source");
    var p=Array.Find(plan.rows,x=>x.target==r.currentSource.path);var receipt=Array.Find(install.installedFiles,x=>x.path==r.currentSource.path);var tested=Array.Find(v.sources,x=>x.path==r.currentSource.path);
    PreviewRequire(p!=null&&p.kind=="existing-hook-edit"&&p.before!=null&&p.before.sha256==r.previousSource.sha256&&p.after.sha256==r.currentSource.sha256&&Mine2SameV21(p.before,r.beforeSnapshot)&&Mine2SameV21(p.after,r.afterSnapshot)&&Mine2SameV21(receipt,r.currentSource)&&Mine2SameV21(tested,r.currentSource),"Mine2 unknown shared source delta or untested current bytes");
    if(Path.GetFileName(r.currentSource.path).StartsWith("GameModel",StringComparison.Ordinal))PreviewRequire(Mine2SameV21(Array.Find(m.installedSourcesBefore,x=>x.path==r.currentSource.path),r.currentSource),"Mine2 changed shared model source was not actually tested");
    Mine2ReadV21(r.beforeSnapshot);Mine2ReadV21(r.afterSnapshot);Mine2ReadV21(r.currentSource);
   }
   foreach(var p in plan.rows){var installed=Array.Find(install.installedFiles,x=>x.path==p.target);PreviewRequire(installed!=null&&installed.sha256==p.after.sha256,"Mine2 incomplete Below install receipt");Mine2ReadV21(new PreviewFile{path=p.target,sha256=p.after.sha256});}
   return c;
  }
  static void Mine2ReadHistoricalComponentSourceV21(PreviewFile expected,Mine2BelowCompatibilityV21 c){
   var row=Array.Find(c.rows,x=>x.previousSource.path==expected.path&&x.previousSource.sha256==expected.sha256);
   if(row==null)Mine2ReadV21(expected);else Mine2ReadV21(row.currentSource);
  }

  partial void ConfigureMine2ModelRuntimeV21(PreviewFile binding,ref bool configured){
   PreviewRequire(binding!=null&&binding.sha256==Mine2PreviewPinsV21.ModelRuntime,"Mine2 exact timer adapter binding required");
   var d=PreviewJson<NpcMechanismDocument>(binding);PreviewRequire(d.sourceRulesSha256==Mine2PreviewPinsV21.Rules&&d.sourceMine2!=null&&d.sourceMine2MechanismsV21!=null,"Mine2 typed original+mechanism payload missing");
   game.SetNpcMechanismDefinitions(d);configured=true;
  }
  partial void ConfigureMine2VisualRuntimeV21(PreviewFile binding,ref bool configured){
   PreviewRequire(binding!=null&&binding.sha256==Mine2PreviewPinsV21.VisualRuntime,"Mine2 exact visual binding required");Mine2ReadV21(binding);LoadMine2VisualBindingV21(binding.path);configured=mine2VisualDocumentV21!=null;
  }
  partial void DisposeMine2VisualRuntimeV21(){DisposeMine2VisualV21();if(game!=null){game.SetNpcMine2SourceV20(null);game.SetMine2MechanismsV21(null);}}
 }
}
#endif
