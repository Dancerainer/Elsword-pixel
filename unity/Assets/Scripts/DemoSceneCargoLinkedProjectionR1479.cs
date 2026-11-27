using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  Camera cargoProjectionCameraR1479;
  GameModel cargoProjectionCameraModelR1479;
  int cargoProjectionCameraSerialR1479;
  float cargoProjectionCameraBlendAtR1479=-1;
  Vector3 cargoProjectionCameraFromR1479,cargoProjectionNormalFromR1479;
  bool CargoProjectionViewScopeR1479 {get{return CargoProductSessionActiveR1288&&game!=null&&game.room==0&&game.CurrentRoom.id=="Dungeon_Besma_Air_Ship_Normal-0"&&NativeWorldActive;}}
  void ClearCargoProjectionCameraR1479(){cargoProjectionCameraR1479=null;cargoProjectionCameraModelR1479=null;cargoProjectionCameraBlendAtR1479=-1;}
  void ReconcileCargoProjectionCameraXR1479(ref float x){
   if(CargoProjectionViewScopeR1479&&game.CargoProjectionBridgeActiveR1479){var line=game.Surface(2);x=Mathf.Clamp(RenderPlayerWorldPosition.x,line.x1,line.x2);}
  }
  bool TryCargoProjectionNativeNormalR1479(float x,out Vector3 normal){
   normal=Vector3.zero;if(!CargoProjectionViewScopeR1479)return false;var line=game.Surface(2);
   if(line==null||x<line.x1-.002f||x>line.x2+.002f)return false;
   // Original line2 has the same native endpoints as line1/3. Its own
   // tangent is meaningful at the ends; +/-35 on the concatenated camera
   // path would instead sample the explicitly non-collision connector.
   var tangent=new Vector3(9662.76f-8909.3f,0,-596.193f-(-666.193f));tangent.Normalize();normal=new Vector3(tangent.z,0,-tangent.x);return true;
  }
  void ReconcileCargoProjectionNativeCameraR1479(ref Vector3 at,ref Vector3 normal){
   if(!CargoProjectionViewScopeR1479){ClearCargoProjectionCameraR1479();return;}
   if(cargoProjectionCameraR1479!=nativeCamera||!ReferenceEquals(cargoProjectionCameraModelR1479,game)){
    cargoProjectionCameraR1479=nativeCamera;cargoProjectionCameraModelR1479=game;cargoProjectionCameraSerialR1479=game.CargoProjectionChartSerialR1479;cargoProjectionCameraBlendAtR1479=-1;return;
   }
   if(cargoProjectionCameraSerialR1479!=game.CargoProjectionChartSerialR1479){
    cargoProjectionCameraSerialR1479=game.CargoProjectionChartSerialR1479;
    cargoProjectionCameraFromR1479=nativeCamera.transform.position+nativeCamera.transform.forward*2200;
    cargoProjectionNormalFromR1479=-nativeCamera.transform.forward;cargoProjectionCameraBlendAtR1479=Time.unscaledTime;
   }
   if(cargoProjectionCameraBlendAtR1479<0)return;
   float t=Mathf.Clamp01((Time.unscaledTime-cargoProjectionCameraBlendAtR1479)/.12f);t=t*t*(3-2*t);
   at=Vector3.Lerp(cargoProjectionCameraFromR1479,at,t);normal=Vector3.Slerp(cargoProjectionNormalFromR1479,normal,t).normalized;
   if(t>=1)cargoProjectionCameraBlendAtR1479=-1;
  }
 }
}
