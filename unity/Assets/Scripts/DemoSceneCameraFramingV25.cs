using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        RoomDefinition cameraFramingRoomV25;
        bool cameraFramingReviewOriginalV25;
        bool cameraFramingLayerLimitedV25,cameraFramingGeometryLimitedV25,cameraFramingGeometryConflictV25;
        CameraFramingPolicyV25.Surface[] cameraFramingSurfacesV25;
        bool cameraFramingActualLowerFoundV28,cameraFramingActualLowerOutOfRangeV28,cameraFramingAdaptiveV28,cameraFramingAdaptiveReturnV28;
        int cameraFramingActualLowerSurfaceV28=-1;
        float cameraFramingActualLowerYV28,cameraFramingActualLowerDropV28,cameraFramingEffectiveMaximumV28=450;
        float cameraFramingYV25,cameraFramingVelocityV25,cameraFramingHeightV25=360,cameraFramingZoomVelocityV25;
        float cameraFramingBottomV25=.18f,cameraFramingTopV25=.14f,cameraFramingLowerV25,cameraFramingLowerUsedV25,cameraFramingPaddingV25,cameraFramingSourceFovV25,cameraFramingPhysicalFloorV25,cameraFramingContextFloorV25;
        readonly Vector3[] cameraFramingHudCornersV25=new Vector3[4];
        static readonly string[] CameraFramingLowerHudV25={"Skill Bar","Medicine case","Movement pad","Touch Light","Touch Heavy","Footer","Touch Left","Touch Right","Touch Dash","Touch Jump","Touch Attack","Touch Attack X"};
        float CameraLogicalHeightV25 {get{return worldCamera?worldCamera.orthographicSize*2:CameraFramingPolicyV25.Height;}}
        // Only explicitly owned source sessions retain their authored 480x270
        // framing. The admitted Main routes continue through the original V25 policy.
        bool OwnedSourceFramingR1111 {get{
            if(game==null)return false;
            if(BattleProductSessionActiveR1325&&game.CurrentRoom.id==BattleProductRouteR1325+"-"+game.room)return true;
            if(CargoProductSessionActiveR1288&&game.CurrentRoom.id==CargoProductRouteR1288+"-"+game.room)return true;
            if(plainProductSessionR1320&&ReferenceEquals(game,plainProductModelR1320)&&game.CurrentRoom.id==plainProductRouteR1320+"-"+game.room)return true;if(mine2ProductSessionR1233&&ReferenceEquals(game,mine2ProductModelR1233)&&game.CurrentRoom.id==Mine2ProductResourceIndexR1211.Route+"-"+game.room)return true;
            if(FactoryProductSessionActiveR1179&&game.CurrentRoom.id==FactoryProductResourceIndexR1168.Route+"-"+game.room)return true;
            if(coreProductSessionR1147&&game.CurrentRoom.id=="Dungeon_Altera_Core_Hard-"+game.room)return true;
            if(BelowPresentationSessionV21&&game.CurrentRoom.id=="Dungeon_Altera_Below_Tunnel_Normal-"+game.room)return true;
#if UNITY_EDITOR
            return coreEntrySessionV21&&game.CurrentRoom.id=="Dungeon_Altera_Core_Hard-"+game.room||factoryHostSessionR657&&game.CurrentRoom.id=="Dungeon_Altera_Nasod_Factory_Normal-"+game.room;
#else
            return false;
#endif
        }}
        bool CameraDirectorActiveV25 {get{return AlteraAirshipDirectorActiveV20||DragonNestDirectorActiveV20;}}
        // Pixel density only: orthographic size, aspect, projection and HUD use their original logical units.
        bool CompactSceneryTargetV25 {get{return smallerMobileScenery||cameraFramingReviewOriginalV25||calibrationRun||OwnedSourceFramingR1111;}}
        int CameraNativePixelWidthV25 {get{return Mathf.Max(1,Mathf.RoundToInt(CameraNativePixelHeightV25*displayWorldAspect));}}
        int CameraNativePixelHeightV25 {get{return CompactSceneryTargetV25?270:360;}}
        void ResizeSceneryTargetV25(){
            if(!nativeTarget)return;
            int w=CameraNativePixelWidthV25,h=CameraNativePixelHeightV25;
            if(nativeTarget.width==w&&nativeTarget.height==h&&nativeTarget.IsCreated())return;
            plateValid=plateIsCurrent=plateDepthReady=false;nativeTarget.Release();nativeTarget.width=w;nativeTarget.height=h;
            nativeTarget.filterMode=FilterMode.Point;nativeTarget.Create();
        }
        // Internal review API, deliberately absent from the product menus.
        public void SetCameraFramingReviewOriginalV25(bool original){
            RestoreBelowFortCameraV21();
            cameraFramingReviewOriginalV25=original;cameraFramingRoomV25=null;cameraFollowRoom=-1;
            InvalidateCameraGeometryV25();
            cameraFramingHeightV25=original?270:360;cameraFramingZoomVelocityV25=cameraFramingVelocityV25=0;
            ResizeSceneryTargetV25();
            PrepareCameraFramingV25();SyncNativeCameraFramingV25();
        }
        void PrepareCameraFramingV25(){
            if(!worldCamera)return;
            if(coreStopIntroBridgeDraftV21!=null&&coreStopIntroBridgeDraftV21.ActiveV31)return;
            if(factoryRavenCameraV21!=null&&factoryRavenCameraV21.WholeSpriteScopeV22)return;
            if(PrepareBelowCloseCombatHeightR1482())return;
            if(CoreProductCameraScopeR1149()){PrepareCoreCloseCombatHeightR1482();return;}
            if(calibrationRun||cameraFramingReviewOriginalV25||OwnedSourceFramingR1111){worldCamera.orthographicSize=135;return;}
            worldCamera.orthographicSize=Mathf.Max(180,cameraFramingHeightV25*.5f);
        }
        void MeasureCameraHudV25(){
            cameraFramingBottomV25=.18f;cameraFramingTopV25=.14f;
            if(!hudCanvas||!hudCanvas.isActiveAndEnabled||!worldCamera||worldCamera.pixelRect.height<=0)return;
            Camera ui=hudCanvas.renderMode==RenderMode.ScreenSpaceOverlay?null:hudCanvas.worldCamera;
            if(hudCanvas.renderMode!=RenderMode.ScreenSpaceOverlay&&!ui)return;
            var pixel=worldCamera.pixelRect;float highest=pixel.yMin;
            foreach(string id in CameraFramingLowerHudV25)if(images.TryGetValue(id,out var image)&&image&&image.isActiveAndEnabled&&image.color.a>0){
                image.rectTransform.GetWorldCorners(cameraFramingHudCornersV25);
                foreach(var corner in cameraFramingHudCornersV25)highest=Mathf.Max(highest,RectTransformUtility.WorldToScreenPoint(ui,corner).y);
            }
            cameraFramingBottomV25=Mathf.Clamp(Mathf.Max(.18f,(highest-pixel.yMin)/pixel.height+.025f),.18f,.55f);
        }
        void ApplyCameraFramingV25(){
            if(!worldCamera||calibrationRun||cameraFramingReviewOriginalV25||OwnedSourceFramingR1111)return;
            bool reset=!ReferenceEquals(cameraFramingRoomV25,game.CurrentRoom);
            if(reset){cameraFramingRoomV25=game.CurrentRoom;cameraFramingVelocityV25=cameraFramingZoomVelocityV25=0;var source=game.CurrentRoom.surfaces;
                cameraFramingSurfacesV25=new CameraFramingPolicyV25.Surface[source==null?0:source.Length];
                for(int i=0;i<cameraFramingSurfacesV25.Length;i++){var s=source[i];cameraFramingSurfacesV25[i]=new CameraFramingPolicyV25.Surface(s.x1,s.y1,s.x2,s.y2);}
            }
            MeasureCameraHudV25();var p=RenderPlayerWorldPosition;cameraFramingPhysicalFloorV25=game.GroundAt(p.x);
            float floor=cameraFramingContextFloorV25=CameraFramingPolicyV25.ContextFloor(p.y,cameraFramingPhysicalFloorV25);
            cameraFramingLowerV25=CameraFramingPolicyV25.LowerContext(cameraFramingSurfacesV25,p.x,floor,game.RoomWidth);
            if(!game.NativeTerrain){float nearest=float.NegativeInfinity;for(int i=-2;i<=2;i++){float y=game.GroundAt(Mathf.Clamp(p.x+i*64,0,game.RoomWidth));if(y<=floor-8&&y>=floor-CameraFramingPolicyV25.MaximumContextDrop)nearest=Mathf.Max(nearest,y);}if(!float.IsNegativeInfinity(nearest))cameraFramingLowerV25=nearest;}
            cameraFramingLowerV25=Mathf.Max(cameraFramingLowerV25,p.y-CameraFramingPolicyV25.MaximumContextDrop);
            cameraFramingActualLowerFoundV28=cameraFramingActualLowerOutOfRangeV28=cameraFramingAdaptiveV28=cameraFramingAdaptiveReturnV28=false;
            cameraFramingActualLowerSurfaceV28=-1;cameraFramingActualLowerYV28=cameraFramingActualLowerDropV28=0;
            // Scaffolds, castle stairs and rooftops overlap other floors on the same X.
            // Keep actor scale constant as the camera follows their elevation.
            string stageId=CurrentStageArt.id;
            bool fixedPlatformLens=(stageId=="cave-2"||stageId=="castle-center-1"||
                stageId=="outskirts-1"||stageId=="outskirts-2")&&game.NativeTerrain&&!CameraDirectorActiveV25;
            bool actualLowerScope=!fixedPlatformLens&&CameraFramingPolicyV25.ActualLowerScopeV28(stageId)&&game.NativeTerrain&&!CameraDirectorActiveV25;
            bool supported=game.Grounded(game.player);
            float actualCeiling=supported?cameraFramingPhysicalFloorV25-8:p.y-.1f;
            if(actualLowerScope&&CameraFramingPolicyV25.TryActualSupportBelowV28(cameraFramingSurfacesV25,p.x,actualCeiling,out var actualLower,out var actualSurface)){
                cameraFramingActualLowerFoundV28=true;cameraFramingActualLowerSurfaceV28=game.CurrentRoom.surfaces[actualSurface].id;
                cameraFramingActualLowerYV28=actualLower;cameraFramingActualLowerDropV28=p.y-actualLower;
                cameraFramingActualLowerOutOfRangeV28=cameraFramingActualLowerDropV28>CameraFramingPolicyV25.ActualLowerMaximumDropV28;
                if(!cameraFramingActualLowerOutOfRangeV28)cameraFramingLowerV25=actualLower;
            }
            bool terraceFollow=actualLowerScope&&CurrentStageArt.id=="forest-1";
            if(terraceFollow)cameraFramingLowerV25=CameraFramingPolicyV25.PreviewLanding(cameraFramingSurfacesV25,p.x,p.y,actualCeiling,cameraFramingLowerV25);
            float heroBottom=p.y-12,top=p.y+96,minHeight=360;
            if(!fixedPlatformLens&&heroView&&heroView.sprite){heroBottom=Mathf.Min(heroBottom,heroView.bounds.min.y);top=Mathf.Max(top,heroView.bounds.max.y);}
            if(terraceFollow||cameraFramingActualLowerFoundV28&&!cameraFramingActualLowerOutOfRangeV28){
                float required=(top-Mathf.Min(heroBottom,cameraFramingLowerV25)+2*CameraFramingPolicyV25.Padding)/(1-cameraFramingBottomV25-cameraFramingTopV25);
                cameraFramingAdaptiveV28=required>CameraFramingPolicyV25.MaximumGameplayHeight;
            }
            float maximumHeight=fixedPlatformLens?CameraFramingPolicyV25.Height:
                cameraFramingAdaptiveV28?CameraFramingPolicyV25.ActualLowerMaximumHeightV28:CameraFramingPolicyV25.MaximumGameplayHeight;
            float geometryReferenceFloor=cameraFramingAdaptiveV28?cameraFramingLowerV25:floor;
            float bottom=Mathf.Min(heroBottom,cameraFramingLowerV25);
            bool assembly=false;
#if UNITY_EDITOR
            assembly=BelowFortCameraScopeV21();
            if(assembly){bottom=Mathf.Min(bottom,belowFortContentMinV21.y);top=Mathf.Max(top,belowFortContentMaxV21.y);minHeight=Mathf.Max(minHeight,belowFortHeightV21);
                cameraFramingBottomV25=Mathf.Max(cameraFramingBottomV25,belowFortBottomFractionV21);cameraFramingTopV25=Mathf.Max(cameraFramingTopV25,belowFortTopFractionV21);}
#endif
            var frame=assembly?CameraFramingPolicyV25.Fit(p.x,game.RoomWidth,bottom,top,cameraFramingBottomV25,cameraFramingTopV25,minHeight,displayWorldAspect):
                CameraFramingPolicyV25.FitGameplay(p.x,game.RoomWidth,heroBottom,top,cameraFramingLowerV25,cameraFramingBottomV25,cameraFramingTopV25,float.NaN,360,maximumHeight,cameraFramingAdaptiveV28,displayWorldAspect);
            // Only optional lower context may be limited. Hero scale stays
            // bounded; the authored assembly retains its existing full fit.
            if(terraceFollow&&!reset){
                float goal=frame.height;
                // Maintain the wide shot during a fall. Close back in after
                // landing so zoom and descent don't pull in opposite directions.
                if(!supported&&game.player.vy<0&&cameraFramingActualLowerFoundV28&&goal<cameraFramingHeightV25){
                    goal=cameraFramingHeightV25;cameraFramingZoomVelocityV25=0;
                }
                cameraFramingHeightV25=Mathf.Clamp(Mathf.SmoothDamp(cameraFramingHeightV25,goal,ref cameraFramingZoomVelocityV25,.38f,360,Time.unscaledDeltaTime),360,CameraFramingPolicyV25.ActualLowerMaximumHeightV28);
                if(Mathf.Abs(cameraFramingHeightV25-goal)<.02f){cameraFramingHeightV25=goal;cameraFramingZoomVelocityV25=0;}
            }
            else if(reset||frame.height>=cameraFramingHeightV25){cameraFramingHeightV25=frame.height;cameraFramingZoomVelocityV25=0;}
            else cameraFramingHeightV25=Mathf.Max(frame.height,Mathf.SmoothDamp(cameraFramingHeightV25,frame.height,ref cameraFramingZoomVelocityV25,.45f,1600,Time.unscaledDeltaTime));
            cameraFramingAdaptiveReturnV28=!cameraFramingAdaptiveV28&&cameraFramingHeightV25>CameraFramingPolicyV25.MaximumGameplayHeight+.01f;
            cameraFramingEffectiveMaximumV28=CameraFramingPolicyV25.EffectiveMaximumHeightV28(maximumHeight,cameraFramingHeightV25);
            bool holdActualLower=(cameraFramingAdaptiveV28||cameraFramingAdaptiveReturnV28)&&cameraFramingActualLowerFoundV28&&!cameraFramingActualLowerOutOfRangeV28;
            if(holdActualLower)geometryReferenceFloor=cameraFramingActualLowerYV28;
            float drawMax=terraceFollow?cameraFramingHeightV25:cameraFramingEffectiveMaximumV28;
            frame=assembly?CameraFramingPolicyV25.Fit(p.x,game.RoomWidth,bottom,top,cameraFramingBottomV25,cameraFramingTopV25,cameraFramingHeightV25,displayWorldAspect):
                CameraFramingPolicyV25.FitGameplay(p.x,game.RoomWidth,heroBottom,top,cameraFramingLowerV25,cameraFramingBottomV25,cameraFramingTopV25,float.NaN,cameraFramingHeightV25,drawMax,holdActualLower,displayWorldAspect);
            // Query geometry at the actual smoothed view center, including its
            // width-dependent room clamp, rather than at the future zoom target.
            if(assembly){cameraGeometryValidV25=false;cameraGeometrySamplesV25=0;}
            else if(TryCameraGeometryBottomV25(frame.x,p.x,geometryReferenceFloor,out var localBottom))
                frame=CameraFramingPolicyV25.FitGameplay(p.x,game.RoomWidth,heroBottom,top,cameraFramingLowerV25,cameraFramingBottomV25,cameraFramingTopV25,localBottom,cameraFramingHeightV25,drawMax,holdActualLower,displayWorldAspect);
            cameraFramingLayerLimitedV25=frame.contextLimited;cameraFramingLowerUsedV25=assembly?bottom:frame.lowerUsed;cameraFramingPaddingV25=frame.padding;
            cameraFramingGeometryLimitedV25=frame.geometryLimited;cameraFramingGeometryConflictV25=frame.geometryConflict;
            // Approach changes in landing/mesh context while keeping the actor
            // inside the HUD-safe interval on every frame.
            float minY=frame.safeMinY,maxY=frame.safeMaxY;
            if(terraceFollow||fixedPlatformLens){
                minY=top+frame.padding-frame.height*(.5f-cameraFramingTopV25);
                maxY=heroBottom-frame.padding+frame.height*(.5f-cameraFramingBottomV25);
            }
            cameraFramingYV25=reset?frame.y:Mathf.Clamp(Mathf.SmoothDamp(cameraFramingYV25,frame.y,ref cameraFramingVelocityV25,terraceFollow?.22f:.12f,terraceFollow?420:700,Time.unscaledDeltaTime),minY,maxY);
            float x=assembly?worldCamera.transform.localPosition.x:frame.x;
            bool shake=!assembly&&!CameraDirectorActiveV25;
            float sx=shake&&game.shake>0?Mathf.Sin(wallClock*34)*game.shake*.5f:0,sy=shake&&game.shake>0?Mathf.Cos(wallClock*28)*game.shake*.25f:0;
            cameraFollowY=cameraFramingYV25;cameraFollowVelocity=cameraFramingVelocityV25;
            worldCamera.orthographicSize=frame.height*.5f;worldCamera.transform.localPosition=new Vector3(x+sx,Mathf.Clamp(cameraFramingYV25+sy,minY,maxY),-100);
#if UNITY_EDITOR
            if(assembly){belowFortWidthV21=frame.width;belowFortHeightV21=frame.height;}
#endif
            if(sceneDim)sceneDim.transform.localScale=new Vector3(frame.width*.5f,frame.height*.5f,1);
            UpdateNativeCamera();
        }
        void SyncNativeCameraFramingV25(){
            if(!NativeWorldActive||!worldCamera||!nativeScreen)return;
            float height=CameraLogicalHeightV25;
            nativeCamera.orthographicSize=height*.5f/nativeScene.unit;nativeCamera.aspect=displayWorldAspect;
            nativeScreen.transform.localScale=new Vector3(height*displayWorldAspect,height,1);
        }
        void CameraProjectionDimensionsV25(ref float width,ref float height){
            height=CameraLogicalHeightV25;width=height*displayWorldAspect;
        }
        void SyncCameraOverlayV25(){
            if(sceneDim&&worldCamera)sceneDim.transform.localScale=new Vector3(CameraLogicalHeightV25*displayWorldAspect*.5f,CameraLogicalHeightV25*.5f,1);
        }
        bool TryUpdateFramedDirectorCameraV25(bool render){
            bool dragon=false,active=UpdateSwampMovieV39()||TryApplyAlteraAirshipDirectorNativeCameraV20(false);
            if(!active){dragon=TryApplyDragonNestDirectorNativeCameraV20(false);active=dragon;}
            if(!active)return false;
            SyncNativeCameraFramingV25();cameraFramingSourceFovV25=nativeCamera.fieldOfView;
            // Keep every source eye/look sample. tan(FOV/2) grows by H/270:
            // this cancels H/270 in screen mapping, retaining the existing
            // source billboard's uniform world scale while widening its view.
            nativeCamera.fieldOfView=CameraFramingPolicyV25.PerspectiveFov(cameraFramingSourceFovV25,CameraLogicalHeightV25);
            if(dragon&&dragonDirectorRendererV20){var uv=nativeCamera.WorldToViewportPoint(DragonNestDirectorNativeCenterV20);var sprite=dragonDirectorRendererV20;
                float h=CameraLogicalHeightV25,w=h*displayWorldAspect,scale=sprite.transform.localScale.y;
                DragonNestDirectorViewportBoundsV20=new Rect(uv.x-sprite.sprite.rect.width*scale/(2*w),uv.y-sprite.sprite.rect.height*scale/(2*h),sprite.sprite.rect.width*scale/w,sprite.sprite.rect.height*scale/h);
                if(render&&!ReferenceEquals(dragonDirectorRendererTransformV20,null))npcEffectDepths[dragonDirectorRendererTransformV20]=dragonDirectorNativeDepthV20;
            }
            if(render)nativeCamera.Render();return true;
        }
        [Serializable] sealed class CameraFramingCaptureV25 {
            public string policy="wide-gameplay640x360-bounded450-v25",mode;public float width,height,lowerContextY,lowerContextUsedY,actualSupportY,contextFloorY,maximumLookDown=160,maximumGameplayHeight=450,bottomHudFraction,topHudFraction,paddingUsed,nativeGeometryBottom,sourceFov,actualFov,nativeHalfHeight;
            public bool actualLowerSourceFound,actualLowerOutOfRange,actualLowerAboveHud,actualLowerAdaptiveFit,actualLowerAdaptiveReturn;
            public int actualLowerSurfaceId;public float actualLowerY,actualLowerDrop,effectiveGameplayHeightCap,actualLowerMaximumDrop=300,actualLowerMaximumGameplayHeight=720;public Vector3 actualLowerViewport;
            public int targetWidth,targetHeight,nativeGeometryProbeCount;public Vector3 heroViewportMin,heroViewportMax,lowerContextViewport;public bool heroAboveHud,lowerContextAboveHud,farSupportRangeLimited,lowerLayerRangeLimited,nativeGeometryEnvelopeValid,nativeGeometryLimited,nativeGeometryConflict;
        }
        CameraFramingCaptureV25 CameraFramingObservationV25(){
            if(!worldCamera)return null;var r=new CameraFramingCaptureV25{mode=calibrationRun?"calibration480x270":OwnedSourceFramingR1111?"source-owned-original-framing":cameraFramingReviewOriginalV25?"review-original480x270":CameraDirectorActiveV25?"perspective-source-track-wide-lens":"orthographic-wide-follow",width=CameraLogicalHeightV25*displayWorldAspect,height=CameraLogicalHeightV25,lowerContextY=cameraFramingLowerV25,actualSupportY=cameraFramingPhysicalFloorV25,contextFloorY=cameraFramingContextFloorV25,farSupportRangeLimited=cameraFramingContextFloorV25>cameraFramingPhysicalFloorV25,bottomHudFraction=cameraFramingBottomV25,topHudFraction=cameraFramingTopV25,sourceFov=CameraDirectorActiveV25?cameraFramingSourceFovV25:0,actualFov=nativeCamera?nativeCamera.fieldOfView:0,nativeHalfHeight=nativeCamera?nativeCamera.orthographicSize:0,targetWidth=nativeTarget?nativeTarget.width:0,targetHeight=nativeTarget?nativeTarget.height:0};
            if(heroView&&heroView.sprite){r.heroViewportMin=worldCamera.WorldToViewportPoint(heroView.bounds.min);r.heroViewportMax=worldCamera.WorldToViewportPoint(heroView.bounds.max);r.heroAboveHud=r.heroViewportMin.y>=r.bottomHudFraction-.002f&&r.heroViewportMax.y<=1-r.topHudFraction+.002f;}
            r.lowerLayerRangeLimited=cameraFramingLayerLimitedV25;
            r.lowerContextUsedY=cameraFramingLowerUsedV25;r.paddingUsed=cameraFramingPaddingV25;r.nativeGeometryEnvelopeValid=cameraGeometryValidV25;r.nativeGeometryProbeCount=cameraGeometrySamplesV25;
            r.nativeGeometryBottom=cameraGeometryValidV25?cameraGeometryBottomV25:0;r.nativeGeometryLimited=cameraFramingGeometryLimitedV25;r.nativeGeometryConflict=cameraFramingGeometryConflictV25;
            var p=RenderPlayerWorldPosition;r.lowerContextViewport=worldCamera.WorldToViewportPoint(ProjectNativePoint(new Vector3(p.x,r.lowerContextY,0)));r.lowerContextAboveHud=r.lowerContextViewport.y>=r.bottomHudFraction-.002f;
            if(!calibrationRun&&!cameraFramingReviewOriginalV25&&!OwnedSourceFramingR1111){
            r.actualLowerSourceFound=cameraFramingActualLowerFoundV28;r.actualLowerOutOfRange=cameraFramingActualLowerOutOfRangeV28;
            r.actualLowerAdaptiveFit=cameraFramingAdaptiveV28;r.actualLowerSurfaceId=cameraFramingActualLowerSurfaceV28;r.actualLowerY=cameraFramingActualLowerYV28;r.actualLowerDrop=cameraFramingActualLowerDropV28;
            r.actualLowerAdaptiveReturn=cameraFramingAdaptiveReturnV28;r.effectiveGameplayHeightCap=cameraFramingEffectiveMaximumV28;
            if(cameraFramingAdaptiveV28||cameraFramingAdaptiveReturnV28){
                r.policy="wide-gameplay640x360-actual-lower-adaptive720-v28";
                r.maximumLookDown=CameraFramingPolicyV25.ActualLowerMaximumDropV28;r.maximumGameplayHeight=CameraFramingPolicyV25.ActualLowerMaximumHeightV28;
            }
            if(cameraFramingActualLowerFoundV28){r.actualLowerViewport=worldCamera.WorldToViewportPoint(ProjectNativePoint(new Vector3(p.x,cameraFramingActualLowerYV28,0)));r.actualLowerAboveHud=r.actualLowerViewport.y>=r.bottomHudFraction-.002f;}
            }
            return r;
        }
    }
}
