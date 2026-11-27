#if UNITY_EDITOR
namespace ElswordPixel {
 public sealed partial class DemoScene {
  partial void ConfigureBelowTunnelEntryEffectsV21(PreviewFile binding,ref bool configured){
   configured=false;BelowEntryReadV21(binding);
   var inspected=InspectBelowVisualBindingV21(binding.path,binding.sha256);
   if(!inspected.completeDefinitionCoverage||!inspected.allHoldsExplicit||!inspected.sourceDefinitionsUnchanged)return;
   // Entry approval/proof gate remains owned by the strict entry inspector.
   // This bridge does not convert an explicit per-field HOLD into approval.
   LoadBelowVisualBindingV21(binding.path,binding.sha256);configured=BelowVisualConfiguredV21;
  }
  partial void DisposeBelowTunnelEntryEffectsV21(){DisposeBelowVisualV21();}
 }
}
#endif
