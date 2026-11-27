using System;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        const string PlainMouseNpcV35="NUI_NASOD_MOUSE";
        const string PlainMouseSourceShaV35="bd54a4459c73d1ddb8b2eda8135606172b8cccdf6444f548f4f4252a8fa76ed2";
        static bool PlainMouseSourceV35(Enemy e){return e!=null&&e.sourceNPC==PlainMouseNpcV35&&e.kind=="nasod-mouse";}
        bool PlainMouseRoomV35(){return PlainProductSessionActiveR1320&&plainProductRouteR1320==PlainProductResourceIndexR1320.Normal;}
        partial void UpdatePlainNasodMouseViewsV35(){if(PlainMouseRoomV35())UpdateRegisteredMouseViewsV35(true);}
        partial void DisposePlainNasodMouseViewsV35(){if(mouseFamilyV35=="plain")DisposeMouseViewsV35();}
        partial void CheckPlainNasodMouseRendererV35(Renderer renderer,ref bool owned){if(mouseFamilyV35=="plain"&&RegisteredMouseOwnsRendererV35(renderer))owned=true;}
        static MouseRigV35 LoadPlainMouseV35(){
            return MouseRigV35.LoadRegistered("Pixel/feedback-v35/plain-nasod-mouse/",PlainMouseNpcV35,PlainMouseSourceShaV35,
                PlainMouseRigPinsV35.Pages,PlainMouseRigPinsV35.Bank,PlainMouseRigPinsV35.Height);
        }
        // This is called AFTER the real Plain selector has chosen its exact
        // registered occurrence, including remembered physical-reaction poses.
        partial void RecordPlainNasodMouseSelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId){
            if(!PlainMouseSourceV35(actor)||!PlainMouseRoomV35()||actor.visualScale!=1||!renderer||!renderer.enabled||!renderer.sprite||
                bank==null||bank.npc!=PlainMouseNpcV35||bank.kind!=actor.kind||state==null||motion==null||
                motion.animation!=state.originalSourceAnimation||string.IsNullOrEmpty(poseId)||double.IsNaN(clock)||double.IsInfinity(clock)||
                clock<0||clock>motion.sourceMotionDuration+.00001)return;
            if(!mouseSelectionsV35.TryGetValue(actor,out var w)){w=new MouseSelectionV35();mouseSelectionsV35.Add(actor,w);}
            w.carrier=renderer;w.selectedSprite=renderer.sprite;w.frame=Time.frameCount;w.alias=state.state;
            w.plainMotion=motion.animation;w.clock=(float)clock;w.plainPose=poseId;
        }
    }
}

