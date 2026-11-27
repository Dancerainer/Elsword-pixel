using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        // Existing whole libraries store one sprite pixel per .28 original units.
        // LK's original 182.02519-unit standing body is 67 logical pixels.
        internal const float EliteWholeScaleV35=67f/(182.0251933f*.28f);

        void FitCoreEliteWholeV35(Enemy actor,SpriteRenderer renderer,string motion,double sourceClock) {
            Vector2 root=RenderEnemyPosition(actor);
            Vector3 centre=renderer.transform.localPosition;
            // All three authored libraries draw their attack forward along +X.
            // The signed root can sit outside the trimmed frame. Reflect the
            // centre and the sprite together about that root for either facing.
            int forward=actor.facing<0?-1:1;
            centre.x=root.x+(centre.x-root.x)*forward*EliteWholeScaleV35;
            float rootLift=CoreEliteWalkRootV35.At(actor.sourceNPC,motion,sourceClock);
            centre.y=root.y+(centre.y-root.y+rootLift)*EliteWholeScaleV35;
            renderer.transform.localPosition=centre;
            renderer.transform.localScale=new Vector3(EliteWholeScaleV35,EliteWholeScaleV35,1);
            renderer.flipX=forward<0;
            if(forward<0)renderer.transform.localRotation=Quaternion.Inverse(renderer.transform.localRotation);
            RecordCoreElitePartsV36(actor,renderer,motion,sourceClock);
        }
    }
}
