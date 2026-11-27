#if UNITY_EDITOR
using System;
namespace ElswordPixel {
 [Serializable]public sealed class CoreIntroModelOwnershipV25 {public bool configured,focusOwnerPresent,focusOwnerEnded,completelyReleased;public int actors,retired,cues,releasedActors;}
 public sealed partial class GameModel {
  public CoreIntroModelOwnershipV25 CoreIntroReadOwnershipV25(){return new CoreIntroModelOwnershipV25{configured=coreStopOwnerV21!=null,focusOwnerPresent=coreStopFocusOwnerV21!=null,focusOwnerEnded=coreStopFocusOwnerV21!=null&&coreStopFocusOwnerV21.ended,actors=coreStopActorsV21.Count,retired=coreStopRetiredV21.Count,cues=coreStopCuesV21.Count,releasedActors=coreHardStopIntroReleasedV21.Count,completelyReleased=coreStopOwnerV21==null&&coreStopFocusOwnerV21==null&&coreStopActorsV21.Count==0&&coreStopRetiredV21.Count==0&&coreStopCuesV21.Count==0&&coreHardStopIntroReleasedV21.Count==0};}
 }
}
#endif
