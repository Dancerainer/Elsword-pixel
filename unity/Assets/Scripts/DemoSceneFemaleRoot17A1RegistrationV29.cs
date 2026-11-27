using System;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string FemaleRoot17A1AtlasResourceV29="Pixel/feedback-v29/white-chicken-a1/atlas";
  const string FemaleRoot17A1TrajectoryResourceV29="Pixel/feedback-v29/white-chicken-a1/trajectory";
  bool femaleRoot17A1RegistrationAttemptedV29;

  // Called only by the existing Female display hook after its source scope/pose.
  // One lazy attempt per DemoScene instance; no retry, Update, clock or model state.
  void EnsureFemaleRoot17A1RegistrationV29(){
   if(femaleRoot17A1RegistrationAttemptedV29)return;
   femaleRoot17A1RegistrationAttemptedV29=true;
   if(femaleRoot17A1BindingV29!=null)return;
   try{
    var atlas=Resources.Load<Texture2D>(FemaleRoot17A1AtlasResourceV29);
    var asset=Resources.Load<TextAsset>(FemaleRoot17A1TrajectoryResourceV29);
    if(!atlas||!asset)throw new InvalidOperationException("A1 atlas and trajectory resources are both required");
    // The tiny reader requires the exact Root-approved beb64 JSON identity.
    var trajectory=FemaleRoot17A1TrajectoryV29.FromApprovedBytes(asset.bytes);
    var shader=Shader.Find("Sprites/Default");
    if(!shader||!shader.isSupported)throw new InvalidOperationException("Supported Sprites/Default shader required");
    // Atlas RGBA32/readable/Point/noMip/dimensions are checked by the existing
    // A1 owner before it creates or hides anything. Shared resources stay owned
    // by Resources; this registrar never destroys them or changes their pixels.
    femaleRoot17A1BindingV29=new FemaleRoot17A1DrawnPartsV29.Binding(
     atlas,shader,trajectory.NativeUnitsPerArtPixel,trajectory.FixedRestAnchorOffsetNative,
     trajectory.ArtFootPivot,trajectory.GetNativeOrigin,
     FemaleRoot17A1DrawnPartsV29.SampleAuthoredOneSecondPose);
   }catch(Exception error){
    femaleRoot17A1BindingV29=null;
    Debug.LogWarning("FEMALE_ROOT17_A1_REGISTRATION_FALLBACK_V29 "+error.Message);
   }
  }
 }
}
