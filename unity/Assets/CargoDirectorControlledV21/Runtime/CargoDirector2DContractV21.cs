using System;
using ElswordPixel.CargoDirectorControlledPrivateV21.Clock;
namespace ElswordPixel.CargoDirectorControlledPrivateV21.View {
 [Serializable] public sealed class DirectorAssetV21 {public string path,sha256;}
 [Serializable] public sealed class DirectorPoseV21 {public string key,actor;public int assetIndex;public int[] rect,anchor,wholeAlphaBounds;public bool newDrawing;}
 [Serializable] public sealed class DirectorCameraV21 {public double time,height;public double[] center,humanRoot,armorRoot;public string humanKey,armorKey;public bool humanInFront;}
 [Serializable] public sealed class DirectorViewSpecV21 {
  public string sourceNPC,startState,dyingState;public double duration,dyingDuration,actorWorldScale;public int viewportWidth,viewportHeight,newWholeDrawings,approvedWholeReuse;public bool authorChoice,nativeProjectionEquivalenceClaimed,nativeGetDisabledStateABIClaimed,independentArtApproved;public DirectorAssetV21[] assets;public DirectorPoseV21[] poses;public DirectorCameraV21[] cameraFrames;
  public void Validate(){if(sourceNPC!=CargoWallyDirectorClockV21.NPC||startState!=CargoWallyDirectorClockV21.StartState||dyingState!=CargoWallyDirectorClockV21.DyingState||Math.Abs(duration-63200.0/4800)>1e-10||Math.Abs(dyingDuration-800.0/4800)>1e-10||actorWorldScale!=1||viewportWidth!=768||viewportHeight!=432||cameraFrames==null||cameraFrames.Length!=791||poses==null||poses.Length!=9||assets==null||assets.Length!=2||!authorChoice||nativeProjectionEquivalenceClaimed||nativeGetDisabledStateABIClaimed)throw new ArgumentException("Invalid exact authored director2D contract");foreach(var f in cameraFrames)if(f.height<320||f.center==null||f.center.Length!=2||f.humanRoot==null||f.humanRoot.Length!=2||f.armorRoot==null||f.armorRoot.Length!=2)throw new ArgumentException("Invalid source-root/camera data");}
  public DirectorCameraV21 Sample(double time){if(Double.IsNaN(time)||Double.IsInfinity(time)||time<0)throw new ArgumentOutOfRangeException("time");if(time>=duration)return null;return cameraFrames[Math.Min(790,(int)Math.Floor(time*60+1e-8))];}
 }
 public sealed class SourceKeyActorV21 {public int key;public string sourceNPC;}
 // Authored interpretation of the recovered STAGE2 CF/RF request chain. The
 // source method-name request is preserved, not claimed as a native ABI.
 public sealed class CargoDirector2DActivationV21 {
  public const string Stage="STAGE2/SUB_STAGE0",Condition="CF_STAGE4_SUB0_TRIGGER0",React="RF_STAGE4_SUB0_TRIGGER0";
  readonly CargoWallyDirectorClockV21 clock;long epoch;bool consumed;
  public CargoDirector2DActivationV21(CargoWallyDirectorClockV21 sourceClock){if(sourceClock==null)throw new ArgumentNullException("sourceClock");clock=sourceClock;}
  public void ResetRoom(long roomEpoch){epoch=roomEpoch;consumed=false;clock.ResetRoom(roomEpoch);}
  public bool HandleActualSourceRequests(string stage,string conditionRequest,string reactRequest,Func<int,SourceKeyActorV21[]> exactKeyLookup){
   if(consumed||stage!=Stage||conditionRequest!=Condition||reactRequest!=React||exactKeyLookup==null)return false;var a=exactKeyLookup(1);if(a==null||a.Length!=1||a[0]==null||a[0].key!=1||a[0].sourceNPC!=CargoWallyDirectorClockV21.NPC)return false;
   // Explicit author mapping: StateChangeForce(GetDisabledStateID()) enters
   // this exact actor's declared INIT_STATE.START_STATE, never another actor.
   if(!clock.EnterBoundSourceState(a[0].sourceNPC,CargoWallyDirectorClockV21.StartState,epoch))return false;consumed=true;return true;
  }
 }
}
