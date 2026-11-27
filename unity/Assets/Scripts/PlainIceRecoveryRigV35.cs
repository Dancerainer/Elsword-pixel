using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public static class PlainIceRecoveryRigV35 {
        public const string Npc="NUI_NASOD_ELEMENT_ICE_SOLO",Kind="nasod-element-ice-solo",
            Root="Pixel/feedback-v35/plain-ice-recovery/",MotionSha="83ede79e31640f05fa2a30502249c3289b181abf24811d3ca5a27c3c8f36f08f";
        public const int PartCount=12;
        const string Pages="9636e54b35f1ff4cdd2a5dabd65535d30ec2cd3d36ea4ebade6848de79ce7ff1";
        public static FairyPoseLibraryV33 Load(){
            return RegisteredPartRigV35.Load(Root,Npc,Pages,b=>{
                if(b.parts.Length!=PartCount||b.tracks.Length!=3||b.authoredFacing!=1||b.nativeYaw!=0||b.motionSha256!=MotionSha||
                    b.parts[10].name!="near-boot"||b.parts[11].name!="far-boot")
                    throw new InvalidOperationException("Ice solo ten drawings with two independently articulated ankles");
                string[] motions={"Wait","StandUpAttackBack","StandUpAttackFront"};int[] counts={31,62,52};
                for(int i=0;i<3;i++)if(b.tracks[i].alias!=motions[i]||b.tracks[i].animation!=motions[i]||b.tracks[i].fps!=30 ||
                    b.tracks[i].count!=counts[i]||b.tracks[i].components!=5||b.tracks[i].loop)
                    throw new InvalidOperationException("Ice solo source motion "+motions[i]);
            });
        }
    }
    public sealed partial class DemoScene {
        FairyPoseLibraryV33 plainIceRecoveryBankV35;
        FairyAuthoredPartsV33 plainIceRecoveryPartsV35;
        Enemy plainIceRecoveryActorV35,plainIceRecoverySelectionActorV35;
        SpriteRenderer plainIceRecoveryCarrierV35,plainIceRecoverySelectedCarrierV35;
        Sprite plainIceRecoveryOriginalV35;string plainIceRecoveryMotionV35,plainIceRecoveryPoseV35;
        float plainIceRecoveryClockV35;int plainIceRecoveryFrameV35=-1;bool plainIceRecoveryFailedV35;
        partial void RecordPlainIceRecoverySelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId) {
#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
            PlainElementsPlayerCheckV35.Observe(this,actor,renderer,bank,state,motion,clock,poseId);
#endif
            if(plainProductRouteR1320!=PlainProductResourceIndexR1320.Normal || actor.sourceNPC!=PlainIceRecoveryRigV35.Npc ||
                actor.kind!=PlainIceRecoveryRigV35.Kind || bank.npc!=actor.sourceNPC || bank.kind!=actor.kind)return;
            bool front=motion.animation=="StandUpAttackFront",back=motion.animation=="StandUpAttackBack";
            if(!front&&!back)return;
            if(bank.sourceMotion.sha256!=PlainIceRecoveryRigV35.MotionSha || state.state!=(front?"NASOD_ELEMENT_ICE_SOLO_STAND_UP_ATTACK_FRONT":"NASOD_ELEMENT_ICE_SOLO_STAND_UP_ATTACK_BACK") ||
                motion.frameKeys==null||motion.frameKeys.Length!=12 || state.originalSourceAnimation!=motion.animation ||
                clock<0||clock>motion.sourceMotionDuration+.000001||double.IsNaN(clock)||double.IsInfinity(clock)) {
                if(!plainIceRecoveryFailedV35)Debug.LogWarning("PLAIN_ICE_SOLO_RECOVERY_FALLBACK_V35 original state/clock changed");
                plainIceRecoveryFailedV35=true;return;
            }
            plainIceRecoverySelectionActorV35=actor;plainIceRecoverySelectedCarrierV35=renderer;
            plainIceRecoveryOriginalV35=renderer.sprite;plainIceRecoveryMotionV35=motion.animation;
            plainIceRecoveryPoseV35=poseId;plainIceRecoveryClockV35=(float)clock;plainIceRecoveryFrameV35=Time.frameCount;
        }
        void ReleasePlainIceRecoveryOwnerV35(){
            if(plainIceRecoveryPartsV35!=null){plainIceRecoveryPartsV35.Dispose();plainIceRecoveryPartsV35=null;}
            plainIceRecoveryActorV35=null;plainIceRecoveryCarrierV35=null;
        }
        partial void DisposePlainIceRecoveryViewsV35() {
            ReleasePlainIceRecoveryOwnerV35();plainIceRecoverySelectionActorV35=null;plainIceRecoverySelectedCarrierV35=null;
            plainIceRecoveryOriginalV35=null;plainIceRecoveryFrameV35=-1;plainIceRecoveryFailedV35=false;
            if(plainIceRecoveryBankV35!=null){plainIceRecoveryBankV35.ReleaseOwned(DestroyNativeWorldObject);plainIceRecoveryBankV35=null;}
        }
        partial void CheckPlainIceRecoveryRendererV35(Renderer renderer, ref bool owned){
            if(plainIceRecoveryPartsV35!=null&&plainIceRecoveryPartsV35.OwnsRenderer(renderer))owned=true;
        }
        partial void UpdatePlainIceRecoveryViewsV35(){
            if(!PlainProductSessionActiveR1320||plainProductRouteR1320!=PlainProductResourceIndexR1320.Normal||game==null||!actorsRoot) {
                if(plainIceRecoverySelectionActorV35!=null||plainIceRecoveryBankV35!=null||plainIceRecoveryFailedV35)DisposePlainIceRecoveryViewsV35();
                return;
            }
            var actor=plainIceRecoverySelectionActorV35;
            if(MonsterPixelOwnsActorV53(actor)){ReleasePlainIceRecoveryOwnerV35();return;}
            if(plainIceRecoveryFailedV35||plainIceRecoveryFrameV35!=Time.frameCount||actor==null||
                !plainIceRecoverySelectedCarrierV35||!MonsterTextureRedrawV22.SameSourceFrameV35(plainIceRecoverySelectedCarrierV35.sprite,plainIceRecoveryOriginalV35)) {
                ReleasePlainIceRecoveryOwnerV35();return;
            }
            int index=game.enemies.IndexOf(actor);
            if(index<0||index>=enemyViews.Count||enemyViews[index]!=plainIceRecoverySelectedCarrierV35){ReleasePlainIceRecoveryOwnerV35();return;}
            try {
                if(plainIceRecoveryActorV35!=actor||plainIceRecoveryCarrierV35!=plainIceRecoverySelectedCarrierV35){
                    ReleasePlainIceRecoveryOwnerV35();if(plainIceRecoveryBankV35==null)plainIceRecoveryBankV35=PlainIceRecoveryRigV35.Load();
                    plainIceRecoveryCarrierV35=plainIceRecoverySelectedCarrierV35;plainIceRecoveryActorV35=actor;
                    plainIceRecoveryPartsV35=new FairyAuthoredPartsV33(plainIceRecoveryCarrierV35,plainIceRecoveryBankV35,DestroyNativeWorldObject);
                }
                var c=plainIceRecoveryCarrierV35;var pos=RenderEnemyPosition(actor);var world=actorsRoot.TransformPoint(new Vector3(pos.x,pos.y,0));
                var point=NativeWorldActive?ProjectNativePoint(world,actor.nativeDepth):world;
                plainIceRecoveryPartsV35.Apply(new FairyAuthoredPartsV33.Frame{alias=plainIceRecoveryMotionV35,sourceClock=plainIceRecoveryClockV35,
                    facing=c.flipX?-1:1,visible=c.enabled&&c.gameObject.activeInHierarchy},point);
                if(NativeWorldActive)plainIceRecoveryPartsV35.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,world,actor.nativeDepth));
                if(actor.hp>0&&enemyBars[index].enabled&&plainIceRecoveryPartsV35.TryVisibleTop(out var top)){
                    var b=enemyBars[index].transform.position;b.y=top+8;enemyBars[index].transform.position=b;
                    var t=enemyDebuffViews[index].transform.position;t.y=top+20;enemyDebuffViews[index].transform.position=t;
                }
            } catch(Exception ex) {
                ReleasePlainIceRecoveryOwnerV35();plainIceRecoveryFailedV35=true;Debug.LogWarning("PLAIN_ICE_SOLO_RECOVERY_FALLBACK_V35 "+ex.Message);
            }
        }
    }
}
