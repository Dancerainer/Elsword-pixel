using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        partial void UpdatePlainGuardianSmallViewsV35();
        partial void DisposePlainGuardianSmallViewsV35();
        partial void CheckPlainGuardianSmallRendererV35(Renderer renderer,ref bool owned);
        partial void UpdatePlainNasodMouseViewsV35();
        partial void DisposePlainNasodMouseViewsV35();
        partial void CheckPlainNasodMouseRendererV35(Renderer renderer,ref bool owned);
        partial void UpdatePlainIceRecoveryViewsV35();
        partial void DisposePlainIceRecoveryViewsV35();
        partial void CheckPlainIceRecoveryRendererV35(Renderer renderer,ref bool owned);
        partial void UpdatePlainSprayViewsV35();
        partial void DisposePlainSprayViewsV35();
        partial void CheckPlainSprayRendererV35(Renderer renderer,ref bool owned);

        void UpdatePlainPartViewsV35() {
            UpdatePlainGuardianSmallViewsV35();
            UpdatePlainNasodMouseViewsV35();
            UpdatePlainIceRecoveryViewsV35();
            UpdatePlainSprayViewsV35();
        }
        void DisposePlainPartViewsV35() {
            DisposePlainGuardianSmallViewsV35();
            DisposePlainNasodMouseViewsV35();
            DisposePlainIceRecoveryViewsV35();
            DisposePlainSprayViewsV35();
        }
        bool PlainPartOwnsRendererV35(Renderer renderer) {
            bool owned=false;
            CheckPlainGuardianSmallRendererV35(renderer,ref owned);
            CheckPlainNasodMouseRendererV35(renderer,ref owned);
            CheckPlainIceRecoveryRendererV35(renderer,ref owned);
            CheckPlainSprayRendererV35(renderer,ref owned);
            return owned;
        }
    }
}

