using System;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        float coreProductZoomVelocityR1482;int coreProductZoomFrameR1482=-1;
        void PrepareCoreCloseCombatHeightR1482(){
            if(coreProductZoomFrameR1482!=Time.frameCount){
                // Resume after a real authored intro from a bounded view. The
                // ordinary scale depends only on room, never animation bounds.
                if(coreProductCameraRoomR1149==null){coreProductCameraHeightR1149=Mathf.Clamp(worldCamera.orthographicSize*2,270,330);coreProductZoomVelocityR1482=0;}
                float target=CoreProductCameraPolicyR1149.StableGameplayHeightR1417(game.room);
                coreProductCameraHeightR1149=Mathf.Clamp(Mathf.SmoothDamp(coreProductCameraHeightR1149,target,ref coreProductZoomVelocityR1482,.45f,180,Mathf.Min(Time.unscaledDeltaTime,.1f)),270,330);
                coreProductZoomFrameR1482=Time.frameCount;
            }
            worldCamera.orthographicSize=coreProductCameraHeightR1149*.5f;
        }
        RoomDefinition coreProductCameraRoomR1149;
        CameraFramingPolicyV25.Surface[] coreProductCameraSurfacesR1149;
        float coreProductCameraHeightR1149=CoreProductCameraPolicyR1149.MinimumGameplayHeightR1266,coreProductCameraYvR1149,coreProductCameraVelocityR1149;
        float coreProductStandingBottomR1417,coreProductStandingTopR1417;
        int coreProductCameraFrameR1149=-1,coreProductHudRectsR1149;
        Rect coreProductSafePixelsR1149;
        string coreProductCameraReasonR1149="inactive",coreProductLocalActorR1149="";
        bool coreProductLocalActorIncludedR1149,coreProductLocalActorLimitedR1149;
        readonly Vector3[] coreProductHudCornersR1149=new Vector3[4];
        static readonly string[] CoreProductUpperImagesR1149={"Portrait","HP Track","MP Track","Dungeon Panel","Pause Button","Progress Track","Awakening Gauge Track","Awakening Button","Touch Toggle","Sound Button"};
        static readonly string[] CoreProductUpperLabelsR1149={"Name","HP","MP","Zone","Title","Mission"};
        static readonly string[] CoreProductWorldsR1149={"WI_ALTERA_CORE_STAGE1","WI_ALTERA_CORE_STAGE1_MIDDLE","WI_ALTERA_CORE_STAGE2","WI_ALTERA_CORE_STAGE2_MIDDLE","WI_ALTERA_CORE_BOSS_STAGE"};
        static bool CoreProductFiniteR1149(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
        bool CoreProductCameraScopeR1149(){
            return CoreHardProductSessionActiveR1147&&!CoreHardProductAuthoredCameraActiveR1147&&
                game!=null&&!calibrationRun&&string.IsNullOrEmpty(openMenu)&&!BossIntroductionActive&&
                game.room>=0&&game.room<5&&game.CurrentRoom.id=="Dungeon_Altera_Core_Hard-"+game.room&&
                NativeWorldActive&&nativeScene!=null&&nativeScene.id==CoreProductWorldsR1149[game.room]&&
                worldCamera&&nativeCamera&&nativeScreen&&nativeTarget&&worldCamera.orthographic&&nativeCamera.orthographic&&
                heroView&&heroView.enabled&&heroView.gameObject.activeInHierarchy&&heroView.sprite;
        }
        bool CoreProductHudRectR1149(Graphic graphic,Camera ui,Rect pixel,bool upper,ref float edge){
            if(!graphic||!graphic.isActiveAndEnabled||graphic.color.a<=0)return false;
            var text=graphic as Text;if(text&&string.IsNullOrEmpty(text.text))return false;
            graphic.rectTransform.GetWorldCorners(coreProductHudCornersR1149);
            float minX=float.PositiveInfinity,minY=float.PositiveInfinity,maxX=float.NegativeInfinity,maxY=float.NegativeInfinity;
            foreach(var corner in coreProductHudCornersR1149){var p=RectTransformUtility.WorldToScreenPoint(ui,corner);if(!CoreProductFiniteR1149(p.x)||!CoreProductFiniteR1149(p.y))return false;minX=Mathf.Min(minX,p.x);minY=Mathf.Min(minY,p.y);maxX=Mathf.Max(maxX,p.x);maxY=Mathf.Max(maxY,p.y);}
            if(maxX<=pixel.xMin||minX>=pixel.xMax||maxY<=pixel.yMin||minY>=pixel.yMax)return false;
            edge=upper?Mathf.Min(edge,Mathf.Clamp(minY,pixel.yMin,pixel.yMax)):Mathf.Max(edge,Mathf.Clamp(maxY,pixel.yMin,pixel.yMax));
            coreProductHudRectsR1149++;return true;
        }
        bool MeasureCoreProductHudR1149(out float lower,out float upper){
            lower=upper=0;coreProductHudRectsR1149=0;
            if(!hudCanvas||!hudCanvas.isActiveAndEnabled||!worldCamera)return false;
            var pixel=worldCamera.pixelRect;if(pixel.width<=0||pixel.height<=0)return false;
            Camera ui=hudCanvas.renderMode==RenderMode.ScreenSpaceOverlay?null:hudCanvas.worldCamera;
            if(hudCanvas.renderMode!=RenderMode.ScreenSpaceOverlay&&!ui)return false;
            lower=pixel.yMin;upper=pixel.yMax;bool skill=false,player=false,dungeon=false;
            foreach(string id in CameraFramingLowerHudV25)if(images.TryGetValue(id,out var image)){bool measured=CoreProductHudRectR1149(image,ui,pixel,false,ref lower);if(id=="Skill Bar"||id=="Medicine case")skill|=measured;}
            for(int i=0;i<8;i++)if(labels.TryGetValue("Skill Key "+i,out var key))CoreProductHudRectR1149(key,ui,pixel,false,ref lower);
            foreach(string id in CoreProductUpperImagesR1149)if(images.TryGetValue(id,out var image)){bool measured=CoreProductHudRectR1149(image,ui,pixel,true,ref upper);if(id=="Portrait")player=measured;if(id=="Dungeon Panel")dungeon=measured;}
            foreach(string id in CoreProductUpperLabelsR1149)if(labels.TryGetValue(id,out var label))CoreProductHudRectR1149(label,ui,pixel,true,ref upper);
            coreProductSafePixelsR1149=new Rect(pixel.xMin,lower,pixel.width,Mathf.Max(0,upper-lower));
            return skill&&player&&dungeon&&upper>lower;
        }
        bool CoreProductActorBoundsR1149(int index,out Bounds bounds){
            bounds=new Bounds();var actor=game.enemies[index];
            if(TryMonsterPixelRendererV53(actor,out var whole)){bounds=whole.bounds;return true;}
            if(coreEntrySourceViewV21!=null){
                var frame=coreEntrySourceViewV21.ReadStopActorFrameDraftV21(actor);
                if(frame!=null&&frame.renderedFrame==Time.frameCount&&frame.root&&frame.root.activeInHierarchy&&frame.renderers!=null){
                    bool any=false;foreach(var renderer in frame.renderers)if(renderer&&renderer.enabled&&renderer.gameObject.activeInHierarchy){if(!any){bounds=renderer.bounds;any=true;}else bounds.Encapsulate(renderer.bounds);}if(any)return true;
                }
            }
            if(index>=enemyViews.Count)return false;var sprite=enemyViews[index];
            if(!sprite||!sprite.enabled||!sprite.gameObject.activeInHierarchy||!sprite.sprite||sprite.color.a<=0)return false;
            bounds=sprite.bounds;return true;
        }
        bool CoreProductLocalBoundsR1149(out Bounds bounds){
            bounds=new Bounds();coreProductLocalActorR1149="";float nearest=CameraFramingPolicyV25.Width*.5f;
            for(int i=0;i<game.enemies.Count;i++){
                var actor=game.enemies[i];if(actor==null||!actor.active||actor.dormant||actor.hp<=0||actor.encounterIndex!=game.EncounterIndex)continue;
                float distance=Mathf.Abs(actor.x-RenderPlayerWorldPosition.x);if(distance>nearest)continue;
                Bounds candidate;if(!CoreProductActorBoundsR1149(i,out candidate))continue;
                // Full Renderer.bounds is conservative display context only;
                // it is never an alpha/opacity or original quality proof.
                nearest=distance;bounds=candidate;coreProductLocalActorR1149=actor.sourceNPC;
            }
            return !string.IsNullOrEmpty(coreProductLocalActorR1149);
        }
        // This stable envelope uses the original idle-frame anchors, not the
        // currently displayed attack/jump sprite bounds. Main SpriteOffset's
        // exact vertical convention is bottom=anchorY-h and top=anchorY.
        bool TryCoreProductStandingEnvelopeR1417(out float bottom,out float top){
            bottom=float.PositiveInfinity;top=float.NegativeInfinity;
            if(catalog==null||catalog.hero==null||catalog.hero.clips==null||!heroView)return false;
            AnimationData idle=null;foreach(var clip in catalog.hero.clips)if(clip!=null&&clip.id=="idle"){if(idle!=null)return false;idle=clip;}
            if(idle==null||idle.frames==null||idle.frames.Length==0)return false;
            float scale=Mathf.Abs(heroView.transform.lossyScale.y);if(!CoreProductFiniteR1149(scale)||scale<=0)return false;
            foreach(var frame in idle.frames){if(frame==null||frame.h<=0)return false;float anchor=frame.hasAnchor?frame.anchorY:catalog.hero.anchorY;if(!CoreProductFiniteR1149(anchor))return false;bottom=Mathf.Min(bottom,(anchor-frame.h)*scale);top=Mathf.Max(top,anchor*scale);}
            return CoreProductFiniteR1149(bottom)&&CoreProductFiniteR1149(top)&&top>bottom;
        }
        public int CoreProductStableGameplayRoomR1417 {get{return CoreHardProductSessionActiveR1147&&game!=null&&ReferenceEquals(coreProductCameraRoomR1149,game.CurrentRoom)?game.room:-1;}}
        public float CoreProductStableGameplayHeightR1417 {get{return CoreProductStableGameplayRoomR1417<0?0:coreProductCameraHeightR1149;}}
        partial void ApplyCoreProductCameraPolicyR1149(){
            coreProductCameraFrameR1149=-1;coreProductLocalActorIncludedR1149=coreProductLocalActorLimitedR1149=false;coreProductLocalActorR1149="";coreProductHudRectsR1149=0;coreProductSafePixelsR1149=new Rect();
            // The final room is one stationary multi-part encounter. The arena
            // owner has already framed this frame's King, hero and devices on
            // the same projection that FinishNativeProjection is about to draw.
            if(CoreArenaScopeV21()&&coreArenaPreparedFrameV21==Time.frameCount&&coreArenaSavedV21!=null){
                coreProductCameraRoomR1149=null;
                coreProductCameraReasonR1149="core-boss-arena";
                return;
            }
            if(!CoreProductCameraScopeR1149()){coreProductCameraRoomR1149=null;coreProductCameraReasonR1149=CoreHardProductSessionActiveR1147?"authored-intro-or-other-view-owner":"inactive";return;}
            float lower,upper;if(!MeasureCoreProductHudR1149(out lower,out upper)){coreProductCameraReasonR1149="measured-HUD-interval-unavailable";return;}
            bool reset=!ReferenceEquals(coreProductCameraRoomR1149,game.CurrentRoom);
            if(reset&&!TryCoreProductStandingEnvelopeR1417(out coreProductStandingBottomR1417,out coreProductStandingTopR1417)){coreProductCameraReasonR1149="original-standing-envelope-unavailable";return;}
            if(reset){coreProductCameraRoomR1149=game.CurrentRoom;coreProductCameraVelocityR1149=0;var surfaces=game.CurrentRoom.surfaces;coreProductCameraSurfacesR1149=new CameraFramingPolicyV25.Surface[surfaces==null?0:surfaces.Length];for(int i=0;i<coreProductCameraSurfacesR1149.Length;i++){var s=surfaces[i];coreProductCameraSurfacesR1149[i]=new CameraFramingPolicyV25.Surface(s.x1,s.y1,s.x2,s.y2);}}
            var player=RenderPlayerWorldPosition;float floor=CameraFramingPolicyV25.ContextFloor(player.y,game.GroundAt(player.x));
            // A grounded Core play view keeps the current support floor. The lower
            // platform is optional context, not a reason to reveal more foreground.
            float context=Mathf.Max(floor,CameraFramingPolicyV25.LowerContext(coreProductCameraSurfacesR1149,player.x,floor,game.RoomWidth));
            var pixel=worldCamera.pixelRect;
            var input=new CoreProductCameraPolicyR1149.Input{aspect=displayWorldAspect,playerX=player.x,roomWidth=game.RoomWidth,heroBottom=player.y+coreProductStandingBottomR1417,heroTop=player.y+coreProductStandingTopR1417,lowerContext=context,pixelBottom=pixel.yMin,pixelHeight=pixel.height,hudLowerTop=lower,hudUpperBottom=upper,minimumHeight=coreProductCameraHeightR1149,hasLocalActor=false};
            CoreProductCameraPolicyR1149.Result result;string reason;if(!CoreProductCameraPolicyR1149.TryFit(input,out result,out reason)){coreProductCameraReasonR1149=reason;return;}
            // Only a bounded room-target transition changes zoom. Actual
            // support/player motion controls position; animation bounds and
            // whole-arena geometry never widen this ordinary combat view.
            // This is after the original whole/START prepare. Release only
            // the legacy ordinary whole-arena fit; authored scopes were rejected above.
            RestoreCoreArenaCameraV21();
            var frame=result.frame;coreProductCameraYvR1149=reset?frame.y:Mathf.Clamp(Mathf.SmoothDamp(coreProductCameraYvR1149,frame.y,ref coreProductCameraVelocityR1149,.12f,700,Time.unscaledDeltaTime),frame.safeMinY,frame.safeMaxY);
            float y=Mathf.Clamp(coreProductCameraYvR1149,frame.safeMinY,frame.safeMaxY);
            worldCamera.orthographicSize=frame.height*.5f;worldCamera.transform.localPosition=new Vector3(frame.x,y,worldCamera.transform.localPosition.z);
            // Use the existing Main dark clear and V25 uncovered-geometry
            // backdrop, rather than leaving the source export's sky-blue clear.
            var clear=worldCamera.backgroundColor;clear.a=0;nativeCamera.clearFlags=CameraClearFlags.SolidColor;nativeCamera.backgroundColor=clear;
            if(sceneDim){SyncCameraOverlayV25();sceneDim.transform.localPosition=new Vector3(frame.x,y,sceneDim.transform.localPosition.z);}
            coreProductLocalActorIncludedR1149=result.localActorIncluded;coreProductLocalActorLimitedR1149=result.localActorLimited;
            coreProductCameraReasonR1149="bounded-close-room-scale-original-standing-envelope-physics-follow";coreProductCameraFrameR1149=Time.frameCount;
            // The caller continues through its original UpdateNativeCamera(true)
            // and projection. No Render, model Step or actor setter is called here.
        }
        [Serializable] public sealed class CoreProductCameraObservationR1149 {
            public string scope="normal-product-Core-only",reason,localActor,boundsKind="actual-Renderer.bounds-not-opacity";
            public bool applied,localActorIncluded,localActorLimited,sourceActorScaleChanged=false,sourceModelChanged=false,oldQualityReinterpreted=false;
            public int frame,room,hudRects,targetWidth,targetHeight;public Rect measuredSafePixels,cameraPixelRect;public float logicalHeight;
            public float minimumGameplayHeightR1266=CoreProductCameraPolicyR1149.MinimumGameplayHeightR1266,maximumGameplayHeightR1266=CoreProductCameraPolicyR1149.MaximumGameplayHeightR1266,renderTexelsPerWorldPixelR1266;
            public Vector3 actualHeroViewportMin,actualHeroViewportMax;
            public int stableGameplayRoomR1417=-1;public float stableGameplayHeightR1417,standingBottomR1417,standingTopR1417;public bool animationOrNearestActorControlsZoomR1417=false;
        }
        public CoreProductCameraObservationR1149 ObserveCoreProductCameraR1149(){
            var value=new CoreProductCameraObservationR1149{applied=CoreHardProductSessionActiveR1147&&coreProductCameraFrameR1149==Time.frameCount,reason=coreProductCameraReasonR1149,frame=Time.frameCount,room=game==null?-1:game.room,hudRects=coreProductHudRectsR1149,measuredSafePixels=coreProductSafePixelsR1149,localActor=coreProductLocalActorR1149,localActorIncluded=coreProductLocalActorIncludedR1149,localActorLimited=coreProductLocalActorLimitedR1149,targetWidth=nativeTarget?nativeTarget.width:0,targetHeight=nativeTarget?nativeTarget.height:0,cameraPixelRect=worldCamera?worldCamera.pixelRect:new Rect(),logicalHeight=worldCamera?worldCamera.orthographicSize*2:0};
            value.stableGameplayRoomR1417=CoreProductStableGameplayRoomR1417;value.stableGameplayHeightR1417=CoreProductStableGameplayHeightR1417;value.standingBottomR1417=coreProductStandingBottomR1417;value.standingTopR1417=coreProductStandingTopR1417;
            if(worldCamera&&nativeTarget&&value.logicalHeight>0)value.renderTexelsPerWorldPixelR1266=nativeTarget.height/value.logicalHeight;
            if(worldCamera&&heroView&&heroView.sprite){value.actualHeroViewportMin=worldCamera.WorldToViewportPoint(heroView.bounds.min);value.actualHeroViewportMax=worldCamera.WorldToViewportPoint(heroView.bounds.max);}
            return value;
        }
    }
}
