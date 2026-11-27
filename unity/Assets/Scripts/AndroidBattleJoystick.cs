using UnityEngine;

namespace ElswordPixel {
    // A window can dispatch several touches between two rendered Unity frames.
    // Drain them in order with their original timestamps before the 60 Hz game step.
    public sealed partial class BattleJoystick {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass nativeStick;
        AndroidJavaObject nativeActivity;
        bool nativeUnavailable,nativeInput;
        float nextInstall;
        RectTransform padRect;
        System.IntPtr pendingMethod,drainMethod;
        readonly object[] pollBounds=new object[4];
        readonly object[] noArguments=new object[0];
        int boundsVersion=-1,boundsWidth,boundsHeight;

        void Awake(){
            padRect=(RectTransform)transform;
            try{
                nativeStick=new AndroidJavaClass("com.elswordpixel.input.PixelStickEvents");
                pendingMethod=AndroidJNIHelper.GetMethodID(nativeStick.GetRawClass(),"pending","(FFFF)I",true);
                drainMethod=AndroidJNIHelper.GetMethodID(nativeStick.GetRawClass(),"drain","()[J",true);
                InstallObserver();
            }catch(AndroidJavaException e){NativeFailed(e);}
        }
        void InstallObserver(){
            if(nativeActivity!=null)nativeActivity.Dispose();
            using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                nativeActivity=player.GetStatic<AndroidJavaObject>("currentActivity");
            if(nativeActivity!=null)nativeStick.CallStatic("install",nativeActivity);
            nextInstall=Time.unscaledTime+1;
        }
        void NativeFailed(AndroidJavaException error){
            Debug.LogWarning("Android stick events unavailable: "+error.Message);
            nativeUnavailable=true;nativeInput=false;Release();
        }
        internal void PollAndroid(){
            if(!isActiveAndEnabled||!owner.CanMoveStick){if(nativeInput)ResetGesture();return;}
            if(nativeUnavailable||pointer.HasValue||Screen.width<=0||Screen.height<=0)return;
            try{
                int width=Screen.width,height=Screen.height;
                if(boundsVersion!=owner.HudGeometryVersion||width!=boundsWidth||height!=boundsHeight){
                    var low=RectTransformUtility.WorldToScreenPoint(null,padRect.TransformPoint(padRect.rect.min));
                    var high=RectTransformUtility.WorldToScreenPoint(null,padRect.TransformPoint(padRect.rect.max));
                    pollBounds[0]=low.x/width;pollBounds[1]=1f-high.y/height;
                    pollBounds[2]=high.x/width;pollBounds[3]=1f-low.y/height;
                    boundsVersion=owner.HudGeometryVersion;boundsWidth=width;boundsHeight=height;
                }
                int pending=nativeStick.CallStatic<int>(pendingMethod,pollBounds);
                if(pending<0){
                    if(nativeInput){Release();nativeInput=false;}
                    if(Time.unscaledTime>=nextInstall)InstallObserver();
                    return;
                }
                nativeInput=true;
                if(pending>0)ReplayStickSamples(nativeStick.CallStatic<long[]>(drainMethod,noArguments));
            }catch(AndroidJavaException e){NativeFailed(e);}
        }
        void OnDestroy(){
            try{
                if(nativeStick!=null && nativeActivity!=null)nativeStick.CallStatic("uninstall",nativeActivity);
            }finally{
                if(nativeStick!=null)nativeStick.Dispose();
                if(nativeActivity!=null)nativeActivity.Dispose();
            }
        }
#else
        const bool nativeInput=false;
#endif
    }
}
