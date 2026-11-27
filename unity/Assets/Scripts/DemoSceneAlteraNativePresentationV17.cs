using System;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  // The original Altera Core shell uses the opposite face orientation to the
  // Unity world renderer. The v17 source-camera comparison retains route follow
  // and the existing 480x270 projection; front-cull opens the authored interior.
  // This changes render state only, never vertices, collision or source spawns.
  void ApplyAlteraNativePresentationV17(NativeScene scene) {
   if(scene==null||scene.id==null||!scene.id.StartsWith("WI_ALTERA_CORE_",StringComparison.Ordinal)||!nativeRoot)return;
   foreach(var renderer in nativeRoot.GetComponentsInChildren<MeshRenderer>())
    renderer.sharedMaterial.SetFloat("_Cull",(float)UnityEngine.Rendering.CullMode.Front);
  }
 }
}
