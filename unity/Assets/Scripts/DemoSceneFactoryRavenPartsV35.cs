using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        FairyPoseLibraryV33 ravenRigV35;
        FairyAuthoredPartsV33 ravenPartsV35;
        Enemy ravenOwnerV35,ravenSelectedV35;
        SpriteRenderer ravenCarrierV35;
        readonly List<SpriteRenderer> ravenRenderersV35=new List<SpriteRenderer>(FactoryRavenRigV35.PartCount);
        SourceWholeSelectionV21 ravenSelectionV35;
        Sprite ravenSelectedSpriteV35;
        int ravenSelectedFrameV35=-1,ravenDrawFrameV35=-1,ravenDrawMaskV35;
        bool ravenFailedV35;
        void RecordFactoryRavenSelectionV35(Enemy actor,SpriteRenderer carrier,SourceWholeSelectionV21 selected) {
            if(actor.sourceNPC!=FactoryRavenRigV35.Npc||actor.kind!="raven-clone"||!FactoryProductSessionActiveR1179)return;
            ravenSelectedV35=actor;ravenSelectionV35=selected;ravenSelectedSpriteV35=carrier.sprite;ravenSelectedFrameV35=Time.frameCount;
        }
        bool FactoryRavenOwnsRendererV35(Renderer renderer) {return ravenPartsV35!=null&&ravenPartsV35.OwnsRenderer(renderer);}
        void ClearFactoryRavenOwnerV35(){
            if(ravenPartsV35!=null){ravenPartsV35.Dispose();ravenPartsV35=null;}
            ravenOwnerV35=null;ravenCarrierV35=null;ravenRenderersV35.Clear();ravenDrawFrameV35=-1;ravenDrawMaskV35=0;
        }
        void DisposeFactoryRavenViewsV35(){
            ClearFactoryRavenOwnerV35();ravenSelectedV35=null;ravenSelectionV35=null;ravenSelectedSpriteV35=null;ravenSelectedFrameV35=-1;
            ravenFailedV35=false;if(ravenRigV35!=null){ravenRigV35.ReleaseOwned(DestroyNativeWorldObject);ravenRigV35=null;}
        }
        void UpdateFactoryRavenViewsV35(){
            if(!FactoryProductSessionActiveR1179 || game==null || !actorsRoot) {
                if(ravenRigV35!=null||ravenSelectedV35!=null||ravenFailedV35)DisposeFactoryRavenViewsV35();
                return;
            }
            var actor=ravenSelectedV35;
            if(MonsterPixelOwnsActorV53(actor)){ClearFactoryRavenOwnerV35();return;}
            if(actor==null||ravenSelectedFrameV35!=Time.frameCount||ravenSelectionV35==null){ClearFactoryRavenOwnerV35();return;}
            if(ravenFailedV35)return;
            int index=game.enemies.IndexOf(actor);if(index<0||index>=enemyViews.Count){ClearFactoryRavenOwnerV35();return;}
            var carrier=enemyViews[index];
            if(!carrier || !MonsterTextureRedrawV22.SameSourceFrameV35(carrier.sprite,ravenSelectedSpriteV35)){ClearFactoryRavenOwnerV35();return;}
            try {
                if(ravenOwnerV35!=actor || ravenCarrierV35!=carrier){
                    ClearFactoryRavenOwnerV35();if(ravenRigV35==null)ravenRigV35=FactoryRavenRigV35.Load();
                    ravenPartsV35=new FairyAuthoredPartsV33(carrier,ravenRigV35,DestroyNativeWorldObject);
                    ravenOwnerV35=actor;ravenCarrierV35=carrier;ravenPartsV35.VisitRenderers(ravenRenderersV35.Add);
                    for(int i=0;i<ravenRenderersV35.Count;i++)
                        ravenRenderersV35[i].gameObject.AddComponent<FactoryRavenPartWitnessV35>().Bind(this,i,ravenRenderersV35[i]);
                }
                var selected=ravenSelectionV35;var tr=ravenRigV35.Find(selected.State.Motion);
                if(tr==null||Math.Abs(tr.duration-selected.State.MotionDuration)>.00002f)
                    throw new InvalidOperationException("Factory original Raven motion "+selected.State.Motion);
                var local=RenderEnemyPosition(actor);var world=actorsRoot.TransformPoint(new Vector3(local.x,local.y,0));
                var point=NativeWorldActive?ProjectNativePoint(world,actor.nativeDepth):world;
                // The original fixed-camera bank explicitly forbids automatic spawn-facing mirror.
                ravenPartsV35.Apply(new FairyAuthoredPartsV33.Frame{alias=selected.State.Motion,
                    sourceClock=selected.SourceMotionClock,facing=1,
                    visible=selected.ApplySourceShow(actor.active&&!actor.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy)},point);
                foreach(var part in ravenRenderersV35)if(NativeWorldActive)BindNativeSceneOcclusionR1461(part,world,actor.nativeDepth);
                if(actor.hp>0&&enemyBars[index].enabled&&ravenPartsV35.TryVisibleTop(out var top)){
                    var bar=enemyBars[index].transform.position;bar.y=top+8;enemyBars[index].transform.position=bar;
                    var label=enemyDebuffViews[index].transform.position;label.y=top+20;enemyDebuffViews[index].transform.position=label;
                }
            } catch(Exception ex){ClearFactoryRavenOwnerV35();ravenFailedV35=true;Debug.LogWarning("FACTORY_RAVEN_PARTS_FALLBACK_V35 "+ex.Message);}
        }
        internal void ObserveFactoryRavenPartV35(int index,SpriteRenderer part,Camera camera,int frame) {
            if(ravenOwnerV35==null||camera!=worldCamera||!camera||camera.targetTexture!=null||ravenSelectedFrameV35!=frame||
                index<0||index>=ravenRenderersV35.Count||ravenRenderersV35[index]!=part||!part.enabled||
                part.forceRenderingOff||part.color.a<=0||!part.gameObject.activeInHierarchy||!part.sprite)return;
            if(ravenDrawFrameV35!=frame){ravenDrawFrameV35=frame;ravenDrawMaskV35=0;}
            ravenDrawMaskV35|=1<<index;
            if(ravenDrawMaskV35==(1<<FactoryRavenRigV35.PartCount)-1)ObserveFactoryRavenIntroDrawV21(ravenOwnerV35,ravenCarrierV35,camera,frame);
        }
        bool FactoryRavenOpeningBoundsV35(SpriteRenderer original,out Bounds bounds){
            bounds=original.bounds;
            if(ravenPartsV35==null||original!=ravenCarrierV35)return true;
            if(ravenDrawFrameV35!=Time.frameCount||ravenDrawMaskV35!=(1<<FactoryRavenRigV35.PartCount)-1||ravenRenderersV35.Count!=FactoryRavenRigV35.PartCount)return false;
            bounds=ravenRenderersV35[0].bounds;
            for(int j=1;j<ravenRenderersV35.Count;j++)bounds.Encapsulate(ravenRenderersV35[j].bounds);
            return true;
        }
        internal bool VisitFactoryRavenPartsV35(Enemy actor,Action<SpriteRenderer> visit) {
            if(ravenOwnerV35!=actor||ravenPartsV35==null||!ravenPartsV35.LastFrame.visible)return false;
            ravenPartsV35.VisitRenderers(visit);return true;
        }
    }
    public sealed class FactoryRavenPartWitnessV35:MonoBehaviour {
        DemoScene scene;SpriteRenderer part;int index;
        internal void Bind(DemoScene owner,int partIndex,SpriteRenderer renderer){scene=owner;index=partIndex;part=renderer;}
        void OnWillRenderObject(){if(scene&&part)scene.ObserveFactoryRavenPartV35(index,part,Camera.current,Time.frameCount);}
    }
}
