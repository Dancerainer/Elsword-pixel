using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        // A straight part of an orthographic route can reuse a wider photograph
        // of the actual level and its solid eye depth. A changing angle, zoom
        // or motion through the depth axis always needs a fresh native view.
        sealed class SceneryPartState {
            public Matrix4x4 transform;
            public Bounds bounds;
            public Material material;
            public Texture texture;
            public uint textureRevision;
            public int materialCrc;
            public bool enabled, supported, revealsPlayer, insidePlate;
        }
        SceneryPartState[] sceneryStates;
        RenderTexture sceneryPlate,sceneryPlateDepth,sceneryPatch;
        int plateGeneration;
        float platePressureUntil;
        bool hasPreviousPlatePose;
        int previousSegment;
        Matrix4x4 previousProjection;
        Vector3 previousForward,previousUp,previousEye;
        bool plateDepthReady;
        Vector2 plateCropScale,plateCropOrigin;
        Vector3 plateEye,plateRight,plateUp,plateForward;
        Matrix4x4 plateProjection;
        Color plateClear;
        int plateSegment,platePaddingX,platePaddingY;
        bool plateValid,plateIsCurrent,renderingAlternateView;
        Vector2 lastPlateOffset;
        Vector4 lastRevealArea,lastRevealDepth;
        bool lastRevealEnabled,lastPlateColorWasLive;
        bool sceneryModeRead,sceneryHasCopy,sceneryUsesNoCopy,sceneryPathLogging;
        string sceneryLoggedRoom;
        int sceneryLoggedEvents;
        readonly Plane[] patchPlanes=new Plane[6];

        void ReadSceneryRenderMode(){
            if(sceneryModeRead)return;
            sceneryModeRead=true;
            var support=SystemInfo.copyTextureSupport;
            sceneryHasCopy=(support&CopyTextureSupport.Basic)!=0;
#if UNITY_ANDROID && !UNITY_EDITOR
            bool compatible=!sceneryHasCopy&&SystemInfo.graphicsDeviceType==GraphicsDeviceType.OpenGLES3&&!SystemInfo.graphicsUVStartsAtTop;
            int requested=-1;
            try{
                using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))
                using(var intent=activity?.Call<AndroidJavaObject>("getIntent")){
                    const string extra="elsword.sceneryTrial";
                    sceneryPathLogging=intent!=null&&intent.Call<bool>("hasExtra",extra);
                    if(sceneryPathLogging)requested=intent.Call<int>("getIntExtra",extra,0);
                }
            }catch(AndroidJavaException){requested=0;}
            sceneryUsesNoCopy=compatible&&(requested==-1||requested==3);
            if(sceneryPathLogging)Debug.Log("EL_SCENERY_TRIAL requested="+requested+" effective="+(sceneryUsesNoCopy?3:0)+
                " graphics="+SystemInfo.graphicsDeviceType+" topOrigin="+SystemInfo.graphicsUVStartsAtTop+" copySupport="+support);
#endif
        }
        void TraceScenery(int bit,string path){
            if(!sceneryPathLogging)return;
            string room=nativeScene==null?"":nativeScene.id;
            if(sceneryLoggedRoom!=room){sceneryLoggedRoom=room;sceneryLoggedEvents=0;}
            if((sceneryLoggedEvents&bit)!=0)return;
            sceneryLoggedEvents|=bit;Debug.Log("EL_SCENERY_PATH room="+room+" path="+path);
        }

        void ReleaseMobileSceneryOnPressure(){
            platePressureUntil=Time.realtimeSinceStartup+30f;
            if(sceneryDepthBound&&NativeWorldActive&&nativeRoot&&nativeTarget&&nativeTarget.IsCreated()&&worldCamera){
                // A pressure event can arrive after final projection, even on a
                // paused frame. Keep the current narrow eye depth valid now.
                plateDepthReady=false;hasDepthImage=false;
                CaptureNativeSceneEyeDepthR1461();
                TraceScenery(128,"pressure-redrew-narrow-depth");
            }
            DisposeSceneryPlate();
        }
        void DisposeSceneryPlate(){
            // A disappearing room needs no replacement target or draw.
            if(sceneryDepthBound){
                sceneryDepthBound=false;nativeSceneDepthReadyR1461=false;
                Shader.SetGlobalFloat("_NativeSceneDepthEnabledR1461",0);
                Shader.SetGlobalTexture("_NativeSceneEyeDepthR1461",null);
                Shader.SetGlobalVector("_NativeSceneDepthUVR1461",new Vector4(1,1,0,0));
            }
            lastPlateColorWasLive=false;
            ReleaseSceneryTexture(ref sceneryPlate);ReleaseSceneryTexture(ref sceneryPlateDepth);ReleaseSceneryTexture(ref sceneryPatch);
            plateDepthReady=false;plateGeneration++;
            plateValid=plateIsCurrent=hasPreviousPlatePose=false;sceneryStates=null;
        }
        void ReleaseSceneryTexture(ref RenderTexture texture){
            if(texture){texture.Release();DestroyNativeWorldObject(texture);}
            texture=null;
        }
        bool EnsureSceneryTexture(ref RenderTexture texture,string name,int width,int height,RenderTextureFormat format=RenderTextureFormat.ARGB32){
            if(texture&&texture.width==width&&texture.height==height&&texture.IsCreated())return true;
            ReleaseSceneryTexture(ref texture);
            texture=new RenderTexture(width,height,24,format){
                name=name,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp,antiAliasing=1};
            return texture.Create();
        }

        bool CanUseSceneryPlate(){
            return mobileDisplayReady&&resolutionMode!=MobileResolution.Native&&Time.realtimeSinceStartup>=platePressureUntil&&
                nativeCamera.orthographic&&nativeCamera.cullingMask==(1<<28)&&
                Shader.GetGlobalFloat("_SceneDetailReviewOriginal")<.5f&&Shader.GetGlobalFloat("_ForestGroundReviewOriginal")<.5f&&
                !calibrationRun&&!cameraFramingReviewOriginalV25&&!CameraDirectorActiveV25&&
                !coreProductSessionR1147&&!battleProductSessionR1325&&!cargoProductSessionR1288&&
                !factoryProductSessionR1179&&!plainProductSessionR1320&&!belowPlayerSessionV21&&
                hamelMovingParts.Count==0&&!forestPreviewActive&&nativeRoot.childCount==nativeSceneOccludersR1461.Count&&
                (sceneryHasCopy||sceneryUsesNoCopy);
        }
        bool ObserveSceneryParts(bool capture=false){
            if(sceneryStates==null||sceneryStates.Length!=nativeSceneOccludersR1461.Count){
                sceneryStates=new SceneryPartState[nativeSceneOccludersR1461.Count];
                for(int i=0;i<sceneryStates.Length;i++)sceneryStates[i]=new SceneryPartState();
                plateValid=false;
            }
            for(int i=0;i<sceneryStates.Length;i++){
                var renderer=nativeSceneOccludersR1461[i].renderer;var old=sceneryStates[i];
                bool active=renderer&&renderer.enabled&&renderer.gameObject.activeInHierarchy;
                if(old.enabled!=active){old.enabled=active;plateValid=false;}
                if(!active)continue;
                if(renderer.gameObject.layer!=28)return false;
                var matrix=renderer.localToWorldMatrix;
                if(!old.transform.Equals(matrix)){
                    old.transform=matrix;old.bounds=renderer.bounds;plateValid=false;
                }
                // Parts outside the padded image cannot change it. Their
                // visibility and transforms were still checked above, and
                // a new view validates every source before capture.
                if(plateValid&&!capture&&!old.insidePlate)continue;
                // An externally animated material or per-renderer override
                // does not have the immutable scenery contract.
                if(renderer.HasPropertyBlock())return false;
                var material=renderer.sharedMaterial;
                if(!material||!material.shader)return false;
                int crc=material.ComputeCRC();
                if(old.material!=material||old.materialCrc!=crc){
                    string shader=material.shader.name;
                    old.supported=shader=="ElswordPixel/NativeWorld"||shader=="ElswordPixel/ForestGroundFaceDisplayV28";
                    old.revealsPlayer=old.supported&&material.GetFloat(DepthCoverage)>0;
                    old.material=material;old.materialCrc=crc;plateValid=false;
                }
                if(!old.supported)return false;
                var texture=material.mainTexture;
                uint revision=texture?texture.updateCount:0;
                if(old.texture!=texture||old.textureRevision!=revision){
                    old.texture=texture;old.textureRevision=revision;plateValid=false;
                }
            }
            return true;
        }
        static Matrix4x4 SceneryWindowProjection(Matrix4x4 projection,Rect rect,int width,int height){
            var window=Matrix4x4.identity;
            window.m00=width/rect.width;window.m11=height/rect.height;
            window.m03=(width-2*rect.x-rect.width)/rect.width;
            window.m13=(height-2*rect.y-rect.height)/rect.height;
            return window*projection;
        }
        void RenderSceneryWindow(RenderTexture target,Matrix4x4 projection){
            // Keep the main native-view reveal globals during a local patch;
            // the wider capture explicitly disables the reveal cutout.
            renderingAlternateView=true;
            var old=nativeCamera.targetTexture;
            try{
                nativeCamera.targetTexture=target;nativeCamera.projectionMatrix=projection;
                nativeCamera.Render();
            }finally{
                nativeCamera.targetTexture=old;
                if(CastleCornerProjection)SetCastleCornerProjection();else nativeCamera.ResetProjectionMatrix();
                renderingAlternateView=false;
            }
        }
        bool TryDrawSceneryPlate(){
            plateDepthReady=false;
            if(!CanUseSceneryPlate()){
                if(!sceneryHasCopy&&!sceneryUsesNoCopy)TraceScenery(1,"original-no-region-copy");
                else if(nativeRoot.childCount!=nativeSceneOccludersR1461.Count)TraceScenery(2,"foreign-native-child");
                else TraceScenery(4,"original-room-or-display-policy");
                if(sceneryPlate)DisposeSceneryPlate();
                return false;
            }
            if(!ObserveSceneryParts()){
                TraceScenery(8,"mutable-or-unowned-material");
                if(sceneryPlate)DisposeSceneryPlate();
                return false;
            }
            if(!nativeTarget.IsCreated()){
                plateIsCurrent=false;
                if(!nativeTarget.Create())return false;
            }
            int w=nativeTarget.width,h=nativeTarget.height;
            int paddingX=Mathf.Max(16,w/4),paddingY=Mathf.Max(8,h/8);
            var eye=nativeCamera.transform.position;
            var forward=nativeCamera.transform.forward;
            var projection=nativeCamera.projectionMatrix;
            var clear=nativeCamera.backgroundColor;
            int segment=SegmentAt(nativeScene.path,(worldCamera.transform.position.x-nativeScene.offsetX)/nativeScene.unit);
            float unitsPerPixel=2*nativeCamera.orthographicSize/h;
            var up=nativeCamera.transform.up;
            bool stableView=hasPreviousPlatePose&&previousSegment==segment&&projection.Equals(previousProjection)&&
                (forward-previousForward).sqrMagnitude<=1e-10f&&(up-previousUp).sqrMagnitude<=1e-10f&&
                Mathf.Abs(Vector3.Dot(eye-previousEye,forward))<=unitsPerPixel*.01f;
            previousSegment=segment;previousProjection=projection;previousForward=forward;previousUp=up;previousEye=eye;hasPreviousPlatePose=true;
            var delta=eye-plateEye;
            var offset=new Vector2(Vector3.Dot(delta,plateRight),Vector3.Dot(delta,plateUp))/unitsPerPixel;
            bool capture=!plateValid||!sceneryPlate||!sceneryPlate.IsCreated()||!sceneryPlateDepth||!sceneryPlateDepth.IsCreated()||
                sceneryPlate.width!=w+2*paddingX||sceneryPlate.height!=h+2*paddingY||
                segment!=plateSegment||!projection.Equals(plateProjection)||!clear.Equals(plateClear)||
                (forward-plateForward).sqrMagnitude>1e-10f||
                (nativeCamera.transform.up-plateUp).sqrMagnitude>1e-10f||
                Mathf.Abs(Vector3.Dot(delta,plateForward))>unitsPerPixel*.01f||
                Mathf.Abs(offset.x)>paddingX-2||Mathf.Abs(offset.y)>paddingY-2;
            if(capture){
                // During a turn or a zoom the ordinary camera is cheaper: don't
                // draw a padded image that would already be obsolete next frame.
                if(!stableView){TraceScenery(16,"original-changing-view");plateValid=false;return false;}
                if(!ObserveSceneryParts(true)){plateValid=false;return false;}
                if(!EnsureSceneryTexture(ref sceneryPlate,"Static scenery / route section",w+2*paddingX,h+2*paddingY)||
                   !EnsureSceneryTexture(ref sceneryPlateDepth,"Static scenery eye depth",w+2*paddingX,h+2*paddingY,RenderTextureFormat.RFloat))return false;
                Shader.SetGlobalFloat("_NativePlayerRevealEnabledR1478",0);
                var guard=new Rect(-paddingX,-paddingY,sceneryPlate.width,sceneryPlate.height);
                var wideProjection=SceneryWindowProjection(projection,guard,w,h);
                GeometryUtility.CalculateFrustumPlanes(wideProjection*nativeCamera.worldToCameraMatrix,patchPlanes);
                foreach(var state in sceneryStates)state.insidePlate=state.enabled&&GeometryUtility.TestPlanesAABB(patchPlanes,state.bounds);
                RenderSceneryWindow(sceneryPlate,wideProjection);
                CaptureSceneryPlateDepth(wideProjection);TraceScenery(32,"captured-route-color-and-depth");
                plateGeneration++;
                plateEye=eye;plateRight=nativeCamera.transform.right;plateUp=nativeCamera.transform.up;
                plateForward=forward;plateProjection=projection;plateClear=clear;plateSegment=segment;
                platePaddingX=paddingX;platePaddingY=paddingY;offset=Vector2.zero;
                plateValid=true;plateIsCurrent=false;
            }
            // Snap only the scenery camera's two screen axes, by at most half
            // a terrain texel. The hero, NPCs, foreground cutout and depth all
            // use this same final camera on this frame. Forward depth/angle,
            // logical world camera, input and simulation remain continuous.
            var snapped=new Vector2(Mathf.Round(offset.x),Mathf.Round(offset.y));
            if(!snapped.Equals(offset)){
                nativeCamera.transform.position=eye+((snapped.x-offset.x)*plateRight+(snapped.y-offset.y)*plateUp)*unitsPerPixel;
                UpdateNativeBackdropV25();
            }
            offset=snapped;
            PrepareNativePlayerRevealCameraR1478(nativeCamera);
            bool reveal=Shader.GetGlobalFloat("_NativePlayerRevealEnabledR1478")>.5f;
            var area=Shader.GetGlobalVector("_NativePlayerRevealViewportR1478");
            var depth=Shader.GetGlobalVector("_NativePlayerRevealEyeStrengthR1478");
            plateCropScale=new Vector2((float)w/sceneryPlate.width,(float)h/sceneryPlate.height);
            plateCropOrigin=new Vector2((platePaddingX+offset.x)/sceneryPlate.width,(platePaddingY+offset.y)/sceneryPlate.height);
            if(plateIsCurrent&&offset.Equals(lastPlateOffset)&&reveal==lastRevealEnabled&&
               (!reveal||(area.Equals(lastRevealArea)&&depth.Equals(lastRevealDepth)))){
                plateDepthReady=true;return true;
            }
            var active=RenderTexture.active;
            try{
                RectInt rect=default;Matrix4x4 patchProjection=default;
                bool needsForeground=reveal&&FindSceneryRevealPatch(area,depth,out rect,out patchProjection);
                if(sceneryUsesNoCopy){
                    if(needsForeground){
                        // Render this complete color view with the current reveal;
                        // no blit or region copy is needed for this frame.
                        nativeCamera.Render();TraceScenery(64,"live-near-foreground");
                    }else if(!plateIsCurrent||lastPlateColorWasLive||!offset.Equals(lastPlateOffset)){
                        // Replacing an earlier live view also clears its old hole.
                        Graphics.Blit(sceneryPlate,nativeTarget,plateCropScale,plateCropOrigin);
                        TraceScenery(256,"sampled-static-color");
                    }
                }else{
                    Graphics.Blit(sceneryPlate,nativeTarget,plateCropScale,plateCropOrigin);
                    if(needsForeground){
                        if(!EnsureSceneryTexture(ref sceneryPatch,"Live player foreground",rect.width,rect.height))return false;
                        RenderSceneryWindow(sceneryPatch,patchProjection);
                        Graphics.CopyTexture(sceneryPatch,0,0,0,0,rect.width,rect.height,nativeTarget,0,0,rect.x,rect.y);
                    }
                }
                lastPlateColorWasLive=needsForeground;
            }finally{RenderTexture.active=active;}
            plateIsCurrent=true;plateDepthReady=true;lastPlateOffset=offset;lastRevealEnabled=reveal;
            lastRevealArea=area;lastRevealDepth=depth;
            return true;
        }
        bool FindSceneryRevealPatch(Vector4 area,Vector4 depth,out RectInt rect,out Matrix4x4 projection){
            int w=nativeTarget.width,h=nativeTarget.height;
            // Snap the patch size, not its world position. The one-pixel
            // guard includes every possible dithered foreground pixel.
            int pw=Mathf.Min(w,16*Mathf.CeilToInt((area.z*2*w+4)/16f));
            int ph=Mathf.Min(h,16*Mathf.CeilToInt((area.w*2*h+4)/16f));
            int x=Mathf.Clamp(Mathf.RoundToInt(area.x*w-pw*.5f),0,w-pw);
            int y=Mathf.Clamp(Mathf.RoundToInt(area.y*h-ph*.5f),0,h-ph);
            rect=new RectInt(x,y,pw,ph);
            projection=SceneryWindowProjection(nativeCamera.projectionMatrix,new Rect(x,y,pw,ph),w,h);
            GeometryUtility.CalculateFrustumPlanes(projection*nativeCamera.worldToCameraMatrix,patchPlanes);
            var eye=nativeCamera.transform.position;var forward=nativeCamera.transform.forward;
            var axes=new Vector3(Mathf.Abs(forward.x),Mathf.Abs(forward.y),Mathf.Abs(forward.z));
            for(int i=0;i<sceneryStates.Length;i++){
                var part=sceneryStates[i];if(!part.enabled||!part.revealsPlayer||!nativeSceneOccludersR1461[i].revealsActors)continue;
                var bounds=part.bounds;
                float near=Vector3.Dot(bounds.center-eye,forward)-Vector3.Dot(bounds.extents,axes);
                if(near>=depth.x-depth.z||!GeometryUtility.TestPlanesAABB(patchPlanes,bounds))continue;
                return true;
            }
            return false;
        }

    }
}
