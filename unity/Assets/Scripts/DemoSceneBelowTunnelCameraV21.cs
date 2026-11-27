using System;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  Camera belowBackgroundCameraV21;Color belowBackgroundOriginalV21;int belowBackgroundApplyCountV21,belowBackgroundRestoreCountV21;
  void RestoreBelowTunnelBackgroundV21(){if(ReferenceEquals(belowBackgroundCameraV21,null))return;if(belowBackgroundCameraV21)belowBackgroundCameraV21.backgroundColor=belowBackgroundOriginalV21;belowBackgroundCameraV21=null;belowBackgroundRestoreCountV21++;}
  void ApplyBelowTunnelBackgroundV21(){
   // Explicit 2D presentation: use the existing project dark clear color.
   // This is not an assertion about the original native-client clear color.
   if(!BelowCameraViewportV21.InScope(BelowPresentationSessionV21,game.CurrentRoom.id,nativeScene.id)){RestoreBelowTunnelBackgroundV21();return;}
   if(belowBackgroundCameraV21!=nativeCamera){RestoreBelowTunnelBackgroundV21();belowBackgroundCameraV21=nativeCamera;belowBackgroundOriginalV21=nativeCamera.backgroundColor;belowBackgroundApplyCountV21++;}
   nativeCamera.backgroundColor=worldCamera.backgroundColor;
  }
  float belowCombatHeightR1482=270,belowCombatZoomVelocityR1482;int belowCombatHeightFrameR1482=-1;
  bool PrepareBelowCloseCombatHeightR1482(){
   if(!belowPlayerSessionV21||!BelowStableBodyCameraScopeR1479()){
    belowCombatHeightR1482=270;belowCombatZoomVelocityR1482=0;belowCombatHeightFrameR1482=-1;return false;
   }
   if(belowCombatHeightFrameR1482!=Time.frameCount){
    float target=game.room==2?300:270;
    belowCombatHeightR1482=Mathf.Clamp(Mathf.SmoothDamp(belowCombatHeightR1482,target,ref belowCombatZoomVelocityR1482,.45f,180,Mathf.Min(Time.unscaledDeltaTime,.1f)),270,330);
    belowCombatHeightFrameR1482=Time.frameCount;
   }
   worldCamera.orthographicSize=belowCombatHeightR1482*.5f;return true;
  }
  RoomDefinition belowCameraStandingRoomR1479;float belowCameraStandingBottomR1479,belowCameraStandingTopR1479;
  bool BelowStableBodyCameraScopeR1479(){return BelowPresentationSessionV21&&!calibrationRun&&game!=null&&game.NativeTerrain&&NativeWorldActive&&nativeScene!=null&&BelowCameraViewportV21.InScope(true,game.CurrentRoom.id,nativeScene.id);}
  void BelowStableStandingEnvelopeR1479(out float bottom,out float top){
   if(!ReferenceEquals(belowCameraStandingRoomR1479,game.CurrentRoom)){
    // The shared reader uses original idle anchors only; it has no Core-session
    // dependency. Weapon/attack/reaction canvases must not move the camera.
    BelowPlayerNeedV21(TryCoreProductStandingEnvelopeR1417(out belowCameraStandingBottomR1479,out belowCameraStandingTopR1479),"Original player standing camera envelope missing");belowCameraStandingRoomR1479=game.CurrentRoom;
   }
   float y=RenderPlayerWorldPosition.y;bottom=y+belowCameraStandingBottomR1479;top=y+belowCameraStandingTopR1479;
  }
  int belowCameraClampFramesV21;float belowCameraRawFollowV21,belowCameraTargetV21;
  float belowCameraBottomMarginV21=BelowCameraViewportV21.BottomHudMargin,belowHudTopScreenV21;
  bool belowHudGeometryMeasuredV21;int belowHudMeasuredRectsV21;
  readonly Vector3[] belowHudCornersV21=new Vector3[4];
  static string[] BelowLowerHudImagesV21 {get{return CameraFramingLowerHudV25;}}
  [Serializable] sealed class BelowCameraCaptureV21 {
   public string schema="below-camera-actual-viewport-observation-v21",name,sourceWorld,sourceRoom,hudMeasurementMode;
   public bool actualPlayMode,sourceNativeEquivalent=false,modelChanged=false,safeViewport,hudGeometryMeasured;
   public int clampFrames,hudMeasuredRects;public float modelTime,modelPlayerY,renderPlayerY,groundY,targetY,rawFollowY,followY,actualCameraY,halfHeight,bottomHudMargin,topHudMargin,minimumBottomHudMargin,hudGeometryBuffer,hudTopScreen;
   public Rect cameraPixelRect;public Vector3 spriteWorldMin,spriteWorldMax,spriteViewportMin,spriteViewportMax;
  }
  void IncludeBelowHudRectV21(RectTransform rect,Camera uiCamera,ref float top,ref int count){
   rect.GetWorldCorners(belowHudCornersV21);float highest=float.NegativeInfinity;
   for(int i=0;i<4;i++){float y=RectTransformUtility.WorldToScreenPoint(uiCamera,belowHudCornersV21[i]).y;if(float.IsNaN(y)||float.IsInfinity(y))throw new InvalidOperationException("Below lower HUD screen corner invalid");highest=Math.Max(highest,y);}
   top=Math.Max(top,highest);count++;
  }
  void MeasureBelowHudMarginV21(){
   belowCameraBottomMarginV21=BelowCameraViewportV21.BottomHudMargin;belowHudGeometryMeasuredV21=false;belowHudMeasuredRectsV21=0;belowHudTopScreenV21=0;
   if(!hudCanvas||!hudCanvas.isActiveAndEnabled||!worldCamera)return;
   var pixel=worldCamera.pixelRect;if(pixel.height<=0)return;
   Camera uiCamera=hudCanvas.renderMode==RenderMode.ScreenSpaceOverlay?null:hudCanvas.worldCamera;
   if(hudCanvas.renderMode!=RenderMode.ScreenSpaceOverlay&&!uiCamera)return;
   float top=pixel.yMin;bool skillBarMeasured=false;
   foreach(string id in BelowLowerHudImagesV21)if(images.TryGetValue(id,out var image)&&image&&image.isActiveAndEnabled&&image.color.a>0){IncludeBelowHudRectV21(image.rectTransform,uiCamera,ref top,ref belowHudMeasuredRectsV21);if(id=="Skill Bar"||id=="Medicine case")skillBarMeasured=true;}
   for(int i=0;i<8;i++)if(labels.TryGetValue("Skill Key "+i,out var label)&&label&&label.isActiveAndEnabled&&label.color.a>0)IncludeBelowHudRectV21(label.rectTransform,uiCamera,ref top,ref belowHudMeasuredRectsV21);
   belowHudTopScreenV21=top;belowHudGeometryMeasuredV21=skillBarMeasured;
   if(belowHudMeasuredRectsV21>0)belowCameraBottomMarginV21=BelowCameraViewportV21.MarginFromScreenHud(top,pixel.yMin,pixel.height,worldCamera.orthographicSize);
  }
  partial void ConstrainBelowTunnelCameraV21(float target,float shakeY,ref float followY,ref float velocity){
   if(!BelowPresentationSessionV21||calibrationRun||!game.NativeTerrain||!NativeWorldActive||!worldCamera||!worldCamera.orthographic||!nativeCamera.orthographic||!heroView||!heroView.sprite){RestoreBelowTunnelBackgroundV21();return;}
   ApplyBelowTunnelBackgroundV21();
   belowCameraRawFollowV21=followY;belowCameraTargetV21=target;MeasureBelowHudMarginV21();float bottom,top;BelowStableStandingEnvelopeR1479(out bottom,out top);
   if(BelowCameraViewportV21.Clamp(BelowPresentationSessionV21,game.CurrentRoom.id,nativeScene.id,bottom,top,worldCamera.orthographicSize,shakeY,ref followY,ref velocity,belowCameraBottomMarginV21))belowCameraClampFramesV21++;
  }
#if UNITY_EDITOR
  void SaveBelowCameraViewportSnapshotV21(string name){
   if(!belowTunnelEntrySessionV21||!worldCamera||!heroView||!heroView.sprite||string.IsNullOrEmpty(forestAutoOutput))return;
   var bounds=heroView.bounds;var lo=worldCamera.WorldToViewportPoint(bounds.min);var hi=worldCamera.WorldToViewportPoint(bounds.max);float height=worldCamera.orthographicSize*2;
   var r=new BelowCameraCaptureV21{name=name,sourceWorld=nativeScene==null?"":nativeScene.id,sourceRoom=game.CurrentRoom.id,actualPlayMode=Application.isPlaying,modelTime=game.elapsed,modelPlayerY=game.player.y,renderPlayerY=RenderPlayerWorldPosition.y,groundY=game.GroundAt(RenderPlayerWorldPosition.x),targetY=belowCameraTargetV21,rawFollowY=belowCameraRawFollowV21,followY=cameraFollowY,actualCameraY=worldCamera.transform.position.y,halfHeight=worldCamera.orthographicSize,bottomHudMargin=belowCameraBottomMarginV21,topHudMargin=BelowCameraViewportV21.TopHudMargin,minimumBottomHudMargin=BelowCameraViewportV21.BottomHudMargin,hudGeometryBuffer=BelowCameraViewportV21.HudGeometryBuffer,hudGeometryMeasured=belowHudGeometryMeasuredV21,hudMeasuredRects=belowHudMeasuredRectsV21,hudTopScreen=belowHudTopScreenV21,cameraPixelRect=worldCamera.pixelRect,hudMeasurementMode="Current runtime canvas during camera constraint; offscreen capture may temporarily switch Canvas mode. Minimum86 also covers observed r9 offscreen skill top.",clampFrames=belowCameraClampFramesV21,spriteWorldMin=bounds.min,spriteWorldMax=bounds.max,spriteViewportMin=lo,spriteViewportMax=hi,safeViewport=lo.y>=belowCameraBottomMarginV21/height-.001f&&hi.y<=1-BelowCameraViewportV21.TopHudMargin/height+.001f};
   File.WriteAllText(Path.Combine(forestAutoOutput,name+".below-camera.json"),JsonUtility.ToJson(r,true));
   SaveBelowFortCameraSnapshotV21(name);
  }
#endif
 }
}
