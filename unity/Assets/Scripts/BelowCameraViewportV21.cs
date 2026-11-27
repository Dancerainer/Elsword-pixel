using System;
namespace ElswordPixel {
 // Display-only guard after the existing target and SmoothDamp. HUD occupancy
 // is measured in actual screen pixels, then converted through camera.pixelRect.
 public static class BelowCameraViewportV21 {
  public const float BottomHudMargin=86,TopHudMargin=40,HudGeometryBuffer=8;
  static bool Finite(float f){return !float.IsNaN(f)&&!float.IsInfinity(f);}
  public static float MarginFromScreenHud(float hudTopScreen,float cameraBottomScreen,float cameraPixelHeight,float halfHeight){
   if(!Finite(hudTopScreen)||!Finite(cameraBottomScreen)||!Finite(cameraPixelHeight)||cameraPixelHeight<=0||!Finite(halfHeight)||halfHeight<=0)throw new ArgumentException("Below HUD screen geometry invalid");
   return Math.Max(BottomHudMargin,Math.Max(0,hudTopScreen-cameraBottomScreen)/cameraPixelHeight*(2*halfHeight)+HudGeometryBuffer);
  }
  public static bool InScope(bool belowSession,string room,string world){return belowSession&&room!=null&&room.StartsWith("Dungeon_Altera_Below_Tunnel_Normal-",StringComparison.Ordinal)&&(world=="WI_ALTERA_BELOW_TUNNEL_STAGE1"||world=="WI_ALTERA_BELOW_TUNNEL_STAGE2A"||world=="WI_ALTERA_BELOW_TUNNEL_STAGE4");}
  public static bool Clamp(bool belowSession,string room,string world,float spriteBottom,float spriteTop,float halfHeight,float shakeY,ref float followY,ref float followVelocity,float bottomMargin=BottomHudMargin){
   if(!InScope(belowSession,room,world))return false;
   if(!Finite(spriteBottom)||!Finite(spriteTop)||spriteTop<spriteBottom||!Finite(halfHeight)||halfHeight<=0||!Finite(shakeY)||!Finite(followY)||!Finite(followVelocity)||!Finite(bottomMargin)||bottomMargin<BottomHudMargin)throw new ArgumentException("Below camera viewport inputs invalid");
   float low=spriteTop-halfHeight+TopHudMargin,high=spriteBottom+halfHeight-bottomMargin;
   if(low>high)throw new ArgumentException("Below complete hero sprite exceeds safe viewport; scaling forbidden");
   float current=followY+shakeY,next=Math.Max(low,Math.Min(high,current));if(next==current)return false;
   followY=next-shakeY;followVelocity=0;return true;
  }
 }
}
