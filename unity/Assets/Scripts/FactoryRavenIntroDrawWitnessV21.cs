using UnityEngine;

namespace ElswordPixel {
    // Rendering only records evidence. The next ordinary model Step consumes it.
    public sealed class FactoryRavenIntroDrawWitnessV21 : MonoBehaviour {
        DemoScene owner;
        Enemy actor;
        SpriteRenderer body;

        internal void Bind(DemoScene view, Enemy sourceActor, SpriteRenderer renderer) {
            owner = view;
            actor = sourceActor;
            body = renderer;
        }

        void OnWillRenderObject() {
            if (owner && body)
                owner.ObserveFactoryRavenIntroDrawV21(actor, body, Camera.current, Time.frameCount);
        }
    }
}
