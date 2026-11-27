using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        float displayWorldAspect=CameraFramingPolicyV25.Aspect;
        bool mobileSurfacePaused,mobileSurfaceUnfocused,mobileSurfaceNeedsRestore;

        void SetWorldSurface(Rect pixels,bool mobile){
            worldCamera.pixelRect=pixels;
            float aspect=mobile?pixels.width/pixels.height:CameraFramingPolicyV25.Aspect;
            if(!Mathf.Approximately(aspect,displayWorldAspect)){
                mobileSceneryTargetDirty=true;
                if(mobile)mobileSurfaceNeedsRestore=true;
                paintedPausedMenu=null;
                InvalidateCameraGeometryV25();
            }
            displayWorldAspect=aspect;
            if(mobile){worldCamera.aspect=aspect;worldCamera.backgroundColor=Color.black;}
            else worldCamera.ResetAspect();
        }

        void MarkMobileSurface(bool pauseEvent,bool unavailable){
            if(!Application.isMobilePlatform)return;
            if(pauseEvent)mobileSurfacePaused=unavailable;else mobileSurfaceUnfocused=unavailable;
            if(unavailable)mobileSurfaceNeedsRestore=true;
        }

        void RestoreMobileSurface(){
            if(mobileStartupPending||mobileLoadingWindowCount>0||Screen.width<=0||Screen.height<=0)return;
            // A requested resolution can replace the surface one frame after focus returns.
            if(nativeTarget&&!nativeTarget.IsCreated())mobileSurfaceNeedsRestore=true;
            if(mobileSurfaceNeedsRestore){lastSize=Vector2.zero;ApplyMobileResolution();}
            FitScreen();
            if(!mobileSurfaceNeedsRestore)return;
            mobileSurfaceNeedsRestore=false;accumulator=0;paintedPausedMenu=null;
            RestoreMobileScenery();
        }

        void RestoreMobileScenery(){
            if(!NativeWorldActive||mobileLoadingWindowCount>0||waitingForFirstMobileRoom)return;
            DisposeSceneryPlate();hasDepthImage=false;lastDepthCropGeneration=-1;
            ResizeSceneryTargetV25();
            if(!nativeTarget.IsCreated()){mobileSurfaceNeedsRestore=true;return;}
            // Settings and paused menus can bypass the ordinary terrain draw entirely.
            // Repaint the existing camera target without advancing gameplay or dismissing UI.
            UpdateNativeCamera(true);CaptureNativeSceneEyeDepthR1461();
        }
    }
}
