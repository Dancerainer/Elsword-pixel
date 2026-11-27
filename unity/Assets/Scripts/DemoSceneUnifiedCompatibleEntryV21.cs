#if UNITY_EDITOR
using System;using System.IO;using System.Collections;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 string unifiedSourceSessionDescriptorV21;
 UnifiedSourceStageV21.LoadSession unifiedSourceLoadSessionV21;
 UnifiedSourceLoadEnumeratorV21 unifiedSourceLoaderV21;
 // Last completed session counters are diagnostics only; they grant no admission.
 int unifiedSourceLoadFullValidationsV21,unifiedSourceLoadActivationsV21,unifiedSourceLoadMoveNextV21;
 [Serializable] public sealed class UnifiedCompatibleInspectionV21 {public bool passed,sourceStageVerified,historicalNaturalRunVerified,currentSharedRevisionNaturalRunVerified,actualEntryExecuted,newPublicReady;public int exactCurrentSources;public string descriptorSha256,sourceStage;public BelowTunnelEntryInspectionV21 historicalBelow;public Mine2PreviewInspectionV21 historicalMine2;}
 static UnifiedSourceStageV21 UnifiedSourceScopeV21(string path){return UnifiedSourceStageV21.Enter(Path.GetFullPath(path),UnifiedSourceStagePinsV21.Descriptor);}
 public static UnifiedCompatibleInspectionV21 InspectBelowSharedCompatibleV21(string descriptorPath){using(var s=UnifiedSourceScopeV21(descriptorPath)){var r=InspectBelowTunnelCandidateV21(s.Descriptor.belowBundle.path);ValidateBelowTunnelPreviewEntryV21(s.Descriptor.belowBundle.path);return new UnifiedCompatibleInspectionV21{passed=true,sourceStageVerified=true,historicalNaturalRunVerified=r.naturalRunVerified,currentSharedRevisionNaturalRunVerified=false,actualEntryExecuted=false,newPublicReady=false,exactCurrentSources=s.Manifest.expectedSources,descriptorSha256=UnifiedSourceStagePinsV21.Descriptor,sourceStage=s.Manifest.stageId,historicalBelow=r};}}
 public static UnifiedCompatibleInspectionV21 InspectMine2SharedCompatibleV21(string descriptorPath){using(var s=UnifiedSourceScopeV21(descriptorPath)){var r=InspectMine2CompatibleCandidateV21(s.Descriptor.mine2Bundle.path);ValidateMine2CompatibleEntryV21(s.Descriptor.mine2Bundle.path);return new UnifiedCompatibleInspectionV21{passed=true,sourceStageVerified=true,historicalNaturalRunVerified=true,currentSharedRevisionNaturalRunVerified=false,actualEntryExecuted=false,newPublicReady=false,exactCurrentSources=s.Manifest.expectedSources,descriptorSha256=UnifiedSourceStagePinsV21.Descriptor,sourceStage=s.Manifest.stageId,historicalMine2=r};}}
 public void BeginBelowSharedCompatibleV21(string descriptorPath){BeginUnifiedSharedCompatibleV21(descriptorPath,false);}
 public void BeginMine2SharedCompatibleV21(string descriptorPath){BeginUnifiedSharedCompatibleV21(descriptorPath,true);}
 void BeginUnifiedSharedCompatibleV21(string descriptorPath,bool mine2){
  if(forestPreviewActive||forestPreviewLoading)EndForestPreview();DisposeUnifiedSourceLoadSessionV21();game.SetCoreHardMechanismsV21(null);string full=Path.GetFullPath(descriptorPath);var session=UnifiedSourceStageV21.CreateLoadSession(full,UnifiedSourceStagePinsV21.Descriptor,this);unifiedSourceSessionDescriptorV21=full;unifiedSourceLoadSessionV21=session;unifiedSourceLoadMoveNextV21=0;
  try{using(session.Activate(this)){if(mine2)BeginMine2CompatiblePreviewV21(session.Mine2BundlePath);else BeginBelowTunnelPreviewV21(session.BelowBundlePath);}}catch{DisposeUnifiedSourceLoadSessionV21();session.Dispose();throw;}
 }
 // An enumerator owns one prepared session. Activation never reparses/revalidates
 // its immutable private context. Every mapped read still hashes live + snapshot.
 static bool MoveUnifiedSourceLoadV21(IEnumerator work,UnifiedSourceStageV21.LoadSession session,DemoScene owner,out object value){value=null;if(session==null){bool more=work.MoveNext();if(more)value=work.Current;return more;}using(session.Activate(owner)){bool more=work.MoveNext();if(more)value=work.Current;return more;}}
 sealed class UnifiedSourceLoadEnumeratorV21:IEnumerator,IDisposable {
  readonly DemoScene owner;IEnumerator work;UnifiedSourceStageV21.LoadSession session;object value;bool disposed;
  internal UnifiedSourceLoadEnumeratorV21(DemoScene scene,IEnumerator inner,UnifiedSourceStageV21.LoadSession context){owner=scene;work=inner;session=context;}
  public object Current{get{return value;}}public void Reset(){throw new NotSupportedException();}
  public bool MoveNext(){if(disposed)return false;try{if(session!=null)owner.unifiedSourceLoadMoveNextV21++;bool more=MoveUnifiedSourceLoadV21(work,session,owner,out value);if(!more)Dispose();return more;}catch{try{Dispose();}catch{}throw;}}
  public void Dispose(){if(disposed)return;disposed=true;IDisposable scope=null;try{if(session!=null){try{scope=session.Activate(owner);}catch{try{(work as IDisposable)?.Dispose();}catch{}throw;}}(work as IDisposable)?.Dispose();}finally{try{scope?.Dispose();}finally{work=null;value=null;var ended=session;session=null;owner.ReleaseUnifiedSourceLoadSessionV21(this,ended);}}}
 }
 IEnumerator WrapUnifiedSourceLoadV21(IEnumerator work){string descriptor=unifiedSourceSessionDescriptorV21;if(string.IsNullOrEmpty(descriptor))return new UnifiedSourceLoadEnumeratorV21(this,work,null);var session=unifiedSourceLoadSessionV21;if(session==null||session.DescriptorPath!=descriptor||unifiedSourceLoaderV21!=null)throw new InvalidDataException("Unified exact source stage: loader lacks exclusive prepared session "+descriptor);var result=new UnifiedSourceLoadEnumeratorV21(this,work,session);unifiedSourceLoaderV21=result;return result;}
 void ReleaseUnifiedSourceLoadSessionV21(UnifiedSourceLoadEnumeratorV21 loader,UnifiedSourceStageV21.LoadSession session){if(ReferenceEquals(unifiedSourceLoaderV21,loader))unifiedSourceLoaderV21=null;if(session==null)return;unifiedSourceLoadFullValidationsV21=session.FullValidationCount;unifiedSourceLoadActivationsV21=session.ActivationCount;if(ReferenceEquals(unifiedSourceLoadSessionV21,session)){unifiedSourceLoadSessionV21=null;unifiedSourceSessionDescriptorV21=null;}session.Dispose();}
 void DisposeUnifiedSourceLoadSessionV21(){var loader=unifiedSourceLoaderV21;if(loader!=null)loader.Dispose();var session=unifiedSourceLoadSessionV21;unifiedSourceLoadSessionV21=null;unifiedSourceSessionDescriptorV21=null;if(session!=null){unifiedSourceLoadFullValidationsV21=session.FullValidationCount;unifiedSourceLoadActivationsV21=session.ActivationCount;session.Dispose();}}
}}
#endif
