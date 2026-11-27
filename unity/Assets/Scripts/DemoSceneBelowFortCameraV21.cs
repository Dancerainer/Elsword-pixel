using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  static readonly string[] BelowFortIdsV21={"NUI_CANNON_FORT_PARASITE_BODY_SHELL","NUI_CANNON_FORT_PARASITE_BODY","NUI_CANNON_FORT_PARASITE_BODY_ARMOR","NUI_CANNON_FORT_PARASITE_TOP_EYE","NUI_CANNON_FORT_PARASITE_TOP_CANNON","NUI_CANNON_FORT_PARASITE_BOTTOM_EYE","NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON"};
  sealed class BelowFortCameraSavedV21 {public Camera world,native;public Transform screen;public Vector3 worldPosition,nativePosition,screenPosition,screenScale;public Quaternion worldRotation,nativeRotation;public float worldHalf,nativeHalf,nativeAspect,followY,followVelocity;public int followRoom;public SpriteRenderer dim;public Vector3 dimScale;public RenderTexture target;public int targetWidth,targetHeight;public FilterMode targetFilter;}
  readonly Dictionary<string,Rect> belowFortFootprintsV21=new Dictionary<string,Rect>(StringComparer.Ordinal);
  BelowFortCameraSavedV21 belowFortCameraSavedV21;
  float belowFortWidthV21=480,belowFortHeightV21=270,belowFortBottomFractionV21,belowFortTopFractionV21;
  Vector2 belowFortContentMinV21,belowFortContentMaxV21;int belowFortFitFramesV21,belowFortRestoreCountV21;
  bool BelowFortCameraScopeV21(){return BelowPresentationSessionV21&&!calibrationRun&&game!=null&&game.NativeTerrain&&game.CurrentRoom.id=="Dungeon_Altera_Below_Tunnel_Normal-2"&&game.CurrentRoom.width==849&&game.CurrentRoom.enemies.Count==7&&nativeScene!=null&&nativeScene.id=="WI_ALTERA_BELOW_TUNNEL_STAGE4"&&NativeWorldActive&&worldCamera&&nativeCamera&&nativeScreen&&nativeTarget&&worldCamera.orthographic&&nativeCamera.orthographic;}
   Rect BelowFortApprovedFootprintV21(Enemy enemy){
    Rect found;if(belowFortFootprintsV21.TryGetValue(enemy.sourceNPC,out found))return found;
    Library art;BelowPlayerNeedV21(libraries.TryGetValue(enemy.kind,out art)&&art.data.logicalScale==1&&enemy.visualScale==1,"Below Fort full-size exact approved library absent");
    var seen=new HashSet<string>();float left=float.PositiveInfinity,bottom=float.PositiveInfinity,right=float.NegativeInfinity,top=float.NegativeInfinity;
    foreach(var frames in art.frames.Values)foreach(var sprite in frames){
     BelowPlayerNeedV21(sprite&&sprite.texture&&sprite.texture.filterMode==FilterMode.Point,"Below Fort footprint requires actual Point sprite");var rect=sprite.rect;Vector2 offset;spriteOffsets.TryGetValue(sprite,out offset);
     string key=sprite.texture.GetInstanceID()+"/"+rect.x+"/"+rect.y+"/"+rect.width+"/"+rect.height+"/"+offset.x+"/"+offset.y;if(!seen.Add(key))continue;
     // Match the retained renderer/full-canvas gate, including transparent margins.
     var bounds=sprite.bounds;left=Math.Min(left,bounds.min.x+offset.x);right=Math.Max(right,bounds.max.x+offset.x);bottom=Math.Min(bottom,bounds.min.y+offset.y);top=Math.Max(top,bounds.max.y+offset.y);
    }
    BelowPlayerNeedV21(!float.IsInfinity(left)&&right>left&&top>bottom,"Below Fort approved full-clip canvas footprint empty");found=Rect.MinMaxRect(left,bottom,right,top);belowFortFootprintsV21.Add(enemy.sourceNPC,found);return found;
   }
  void BelowFortCollectV21(bool projected,out Vector2 low,out Vector2 high){
   var hero=heroView.bounds;low=new Vector2(hero.min.x,hero.min.y);high=new Vector2(hero.max.x,hero.max.y);int count=0;
   foreach(string id in BelowFortIdsV21){Enemy enemy=null;foreach(var e in game.enemies)if(e.sourceNPC==id){BelowPlayerNeedV21(enemy==null,"Below Fort duplicated original part");enemy=e;}BelowPlayerNeedV21(enemy!=null&&enemy.visualScale==1,"Below Fort missing original full-size part");var local=BelowFortApprovedFootprintV21(enemy);var p=RenderEnemyPosition(enemy);Vector3 root=projected?ProjectNativePoint(new Vector3(p.x,p.y,0),enemy.nativeDepth):new Vector3(p.x,p.y,0);
    float left=enemy.facing<0?-local.xMax:local.xMin,right=enemy.facing<0?-local.xMin:local.xMax;low=Vector2.Min(low,new Vector2(root.x+left,root.y+local.yMin));high=Vector2.Max(high,new Vector2(root.x+right,root.y+local.yMax));count++;
   }BelowPlayerNeedV21(count==7,"Below Fort camera original seven-part scope mismatch");
  }
  void BelowFortApplySizesV21(){worldCamera.orthographicSize=belowFortHeightV21*.5f;nativeCamera.orthographicSize=belowFortHeightV21*.5f/nativeScene.unit;nativeCamera.aspect=displayWorldAspect;nativeScreen.transform.localScale=new Vector3(belowFortWidthV21,belowFortHeightV21,1);if(sceneDim){var saved=belowFortCameraSavedV21;if(saved.dim==null){saved.dim=sceneDim;saved.dimScale=sceneDim.transform.localScale;}sceneDim.transform.localScale=Vector3.Scale(saved.dimScale,new Vector3(belowFortWidthV21/480,belowFortHeightV21/270,1));}}
  void BelowFortFitV21(ref float x,ref float y,ref float velocity){
   BelowFortCollectV21(false,out belowFortContentMinV21,out belowFortContentMaxV21);var pixel=worldCamera.pixelRect;
   belowFortBottomFractionV21=BelowCameraViewportV21.BottomHudMargin/270f;belowFortTopFractionV21=BelowCameraViewportV21.TopHudMargin/270f;
   MeasureBelowHudMarginV21();if(belowHudGeometryMeasuredV21&&pixel.height>0)belowFortBottomFractionV21=Math.Max(belowFortBottomFractionV21,(belowHudTopScreenV21-pixel.yMin)/pixel.height+BelowCameraViewportV21.HudGeometryBuffer/270f);
   BelowFortCameraFitV21.Fit(belowFortContentMinV21.x,belowFortContentMinV21.y,belowFortContentMaxV21.x,belowFortContentMaxV21.y,displayWorldAspect,belowFortBottomFractionV21,belowFortTopFractionV21,belowFortHeightV21,out x,out y,out belowFortWidthV21,out belowFortHeightV21);velocity=0;BelowFortApplySizesV21();belowFortFitFramesV21++;
  }
  [Serializable] sealed class BelowFortCameraCaptureV21 {public string schema="below-Fort-full-assembly-camera-observation-v21",name;public bool actualPlayMode,sourceNativeEquivalent=false,modelChanged=false,actorScaleChanged=false,active,fullAssemblyAndHeroInSafeViewport,originalPointTargetUnchanged;public int originalParts=7,fitFrames,restoreCount,nativeTargetWidth,nativeTargetHeight;public float halfHeight,nativeHalfHeight,sceneUnit,width,height,bottomHudFraction,topHudFraction;public Vector2 logicalMin,logicalMax,projectedMin,projectedMax,viewportMin,viewportMax;public Vector3 screenScale;}
#if UNITY_EDITOR
  void SaveBelowFortCameraSnapshotV21(string name){
   if(belowFortCameraSavedV21==null||string.IsNullOrEmpty(forestAutoOutput))return;BelowFortCollectV21(true,out var low,out var high);var a=worldCamera.WorldToViewportPoint(new Vector3(low.x,low.y,0));var b=worldCamera.WorldToViewportPoint(new Vector3(high.x,high.y,0));var s=belowFortCameraSavedV21;
   var r=new BelowFortCameraCaptureV21{name=name,actualPlayMode=Application.isPlaying,active=BelowFortCameraScopeV21(),fitFrames=belowFortFitFramesV21,restoreCount=belowFortRestoreCountV21,halfHeight=worldCamera.orthographicSize,nativeHalfHeight=nativeCamera.orthographicSize,sceneUnit=nativeScene.unit,width=belowFortWidthV21,height=belowFortHeightV21,bottomHudFraction=belowFortBottomFractionV21,topHudFraction=belowFortTopFractionV21,logicalMin=belowFortContentMinV21,logicalMax=belowFortContentMaxV21,projectedMin=low,projectedMax=high,viewportMin=new Vector2(a.x,a.y),viewportMax=new Vector2(b.x,b.y),screenScale=nativeScreen.transform.localScale,nativeTargetWidth=nativeTarget.width,nativeTargetHeight=nativeTarget.height,originalPointTargetUnchanged=nativeTarget==s.target&&nativeTarget.width==s.targetWidth&&nativeTarget.height==s.targetHeight&&nativeTarget.filterMode==s.targetFilter&&nativeTarget.filterMode==FilterMode.Point,fullAssemblyAndHeroInSafeViewport=a.x>=0&&b.x<=1&&a.y>=belowFortBottomFractionV21-.001f&&b.y<=1-belowFortTopFractionV21+.001f};
   File.WriteAllText(Path.Combine(forestAutoOutput,name+".below-fort-camera.json"),JsonUtility.ToJson(r,true));
  }
#endif
  float belowFortCombatXR1482,belowFortCombatXVelocityR1482;RoomDefinition belowFortCombatRoomR1482;
  void ConstrainBelowFortCombatCameraR1482(ref float x){
   // The ordinary Player follows the hero and nearby combat roots. Giant
   // full-canvas Boss parts may extend beyond the viewport at original scale.
   var player=RenderPlayerWorldPosition;float nearest=240,contextX=player.x;
   foreach(var actor in game.enemies){
    if(actor==null||!actor.active||actor.dormant||actor.hp<=0||actor.passive||Array.IndexOf(BelowFortIdsV21,actor.sourceNPC)<0)continue;
    float distance=Mathf.Abs(actor.x-player.x);if(distance>nearest)continue;nearest=distance;contextX=actor.x;
   }
   float wanted=player.x+Mathf.Clamp((contextX-player.x)*.3f,-40,40);
   float halfWidth=worldCamera.orthographicSize*CameraFramingPolicyV25.Aspect;
   float left=halfWidth-CameraFramingPolicyV25.RoomOverscan,right=game.RoomWidth-halfWidth+CameraFramingPolicyV25.RoomOverscan;
   wanted=left<=right?Mathf.Clamp(wanted,left,right):game.RoomWidth*.5f;
   if(!ReferenceEquals(belowFortCombatRoomR1482,game.CurrentRoom)){belowFortCombatRoomR1482=game.CurrentRoom;belowFortCombatXR1482=wanted;belowFortCombatXVelocityR1482=0;}
   else belowFortCombatXR1482=Mathf.SmoothDamp(belowFortCombatXR1482,wanted,ref belowFortCombatXVelocityR1482,.22f,500,Mathf.Min(Time.unscaledDeltaTime,.1f));
   x=belowFortCombatXR1482;
   // Y already follows the source support and stable original idle envelope
   // through ConstrainBelowTunnelCameraV21. No seven-part or action-frame fit.
  }
  partial void PrepareBelowFortCameraV21(){
   if(belowPlayerSessionV21){RestoreBelowFortCameraV21();if(!BelowFortCameraScopeV21())belowFortCombatRoomR1482=null;return;}
   if(!BelowFortCameraScopeV21()){RestoreBelowFortCameraV21();return;}if(belowFortCameraSavedV21!=null)return;
   belowFortCameraSavedV21=new BelowFortCameraSavedV21{world=worldCamera,native=nativeCamera,screen=nativeScreen.transform,worldPosition=worldCamera.transform.position,worldRotation=worldCamera.transform.rotation,nativePosition=nativeCamera.transform.position,nativeRotation=nativeCamera.transform.rotation,screenPosition=nativeScreen.transform.localPosition,screenScale=nativeScreen.transform.localScale,dim=sceneDim,dimScale=sceneDim?sceneDim.transform.localScale:Vector3.one,worldHalf=worldCamera.orthographicSize,nativeHalf=nativeCamera.orthographicSize,nativeAspect=nativeCamera.aspect,followY=cameraFollowY,followVelocity=cameraFollowVelocity,followRoom=cameraFollowRoom,target=nativeTarget,targetWidth=nativeTarget.width,targetHeight=nativeTarget.height,targetFilter=nativeTarget.filterMode};
   float x=worldCamera.transform.position.x,y=cameraFollowY,v=cameraFollowVelocity;BelowFortFitV21(ref x,ref y,ref v);
  }
  partial void ConstrainBelowFortCameraV21(float shakeX,float shakeY,ref float x,ref float y,ref float velocity){
   if(belowPlayerSessionV21&&BelowFortCameraScopeV21()){ConstrainBelowFortCombatCameraR1482(ref x);return;}
   if(belowFortCameraSavedV21==null||!BelowFortCameraScopeV21())return;BelowFortFitV21(ref x,ref y,ref velocity);x-=shakeX;y-=shakeY;
  }
  partial void AdjustBelowFortProjectionV21(ref float width,ref float height){
   if(belowFortCameraSavedV21!=null&&BelowFortCameraScopeV21()){width=belowFortWidthV21;height=belowFortHeightV21;}
  }
  partial void RestoreBelowFortCameraV21(){
   var s=belowFortCameraSavedV21;if(s==null)return;if(s.world){s.world.orthographicSize=s.worldHalf;s.world.transform.SetPositionAndRotation(s.worldPosition,s.worldRotation);}if(s.native){s.native.orthographicSize=s.nativeHalf;s.native.aspect=s.nativeAspect;s.native.transform.SetPositionAndRotation(s.nativePosition,s.nativeRotation);}if(s.screen){s.screen.localPosition=s.screenPosition;s.screen.localScale=s.screenScale;}if(s.dim)s.dim.transform.localScale=s.dimScale;cameraFollowY=s.followY;cameraFollowVelocity=s.followVelocity;cameraFollowRoom=s.followRoom;belowFortWidthV21=480;belowFortHeightV21=270;belowFortCameraSavedV21=null;belowFortFootprintsV21.Clear();belowFortRestoreCountV21++;
  }
 }
}
