using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        float cameraFollowY=135,cameraFollowVelocity;int cameraFollowRoom=-1;
        partial void ConstrainBelowTunnelCameraV21(float target,float shakeY,ref float followY,ref float velocity);
        partial void PrepareBelowFortCameraV21();
        partial void ConstrainBelowFortCameraV21(float shakeX,float shakeY,ref float x,ref float y,ref float velocity);
        void UpdateWorldCamera(){
            if(UpdateSwampMovieV39())return;
            if(TryRenderAlteraAirshipDirectorCameraV20())return;
            if(TryRenderDragonNestDirectorCameraV20())return;
#if UNITY_EDITOR
            if(TryRenderCoreStopIntroCameraDraftV21())return;
#else
            if(coreProductSessionR1147&&TryRenderCoreStopIntroCameraDraftV21())return;
#endif
            // The ordinary Core product camera writes its one gameplay view
            // after world actors are drawn; real STOP/director writers ran above.
            if(CoreProductCameraScopeR1149())return;
            PrepareBelowFortCameraV21();
            var p=RenderPlayerWorldPosition;float halfWidth=135*displayWorldAspect;float cameraX=game.RoomWidth<halfWidth*2?game.RoomWidth*.5f:Mathf.Clamp(p.x,halfWidth,game.RoomWidth-halfWidth);ReconcileCargoProjectionCameraXR1479(ref cameraX);
            float height=game.room<catalog.stages.Length?CurrentStageArt.height:270;
            float floor=game.GroundAt(p.x);
            // Follow low terrain continuously, and only lift for the upper
            // part of a jump. Ground contact does not shake the camera.
            float target=135+(floor-GameModel.GroundY)*.65f+Mathf.Max(0,p.y-floor-65)*.8f;
            if(game.NativeTerrain)target=floor+81+Mathf.Max(0,p.y-floor-60)*.8f;
            target=Mathf.Clamp(target,135,Mathf.Max(135,height-135));
            if(!calibrationRun)ApplyVelderChaseCamera(ref cameraX,ref target);
            if(calibrationRun)target=135;
            if(cameraFollowRoom!=game.room||calibrationRun){cameraFollowY=target;cameraFollowVelocity=0;cameraFollowRoom=game.room;}
            else cameraFollowY=Mathf.SmoothDamp(cameraFollowY,target,ref cameraFollowVelocity,.1f,400,Time.unscaledDeltaTime);
            bool shake=!BelowStableBodyCameraScopeR1479();
            float sx=shake&&game.shake>0?Mathf.Sin(wallClock*34)*game.shake*.5f:0,sy=shake&&game.shake>0?Mathf.Cos(wallClock*28)*game.shake*.25f:0;
            ConstrainBelowTunnelCameraV21(target,sy,ref cameraFollowY,ref cameraFollowVelocity);
            ConstrainBelowFortCameraV21(sx,sy,ref cameraX,ref cameraFollowY,ref cameraFollowVelocity);
            worldCamera.transform.localPosition=new Vector3(cameraX+sx,cameraFollowY+sy,-100);
            UpdateNativeCamera();
        }
    }
}
