using System;
namespace ElswordPixel {
 // Pure ownership state used by the real public Begin and independent fault tests.
 public sealed class CoreEntryBeginOwnershipV21 {
  public enum Stage {None,SnapshotCaptured,LoadingOwned,Active}
  public Stage Phase {get;private set;}
  public void Run(Action capture,Action begin,Action pendingRollback,Action ownedRollback){
   if(Phase!=Stage.None)throw new InvalidOperationException("Core entry Begin already owns a session");
   capture();Phase=Stage.SnapshotCaptured;
   try{begin();}catch{bool owned=Phase>=Stage.LoadingOwned;try{if(owned)ownedRollback();else pendingRollback();}finally{Phase=Stage.None;}throw;}
  }
  public void MarkLoadingOwned(){if(Phase!=Stage.SnapshotCaptured)throw new InvalidOperationException("Core loader needs captured admitted snapshot");Phase=Stage.LoadingOwned;}
  public void MarkActive(){if(Phase!=Stage.LoadingOwned)throw new InvalidOperationException("Core active needs owned loader");Phase=Stage.Active;}
  public void Reset(){Phase=Stage.None;}
 }
}
