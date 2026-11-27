using System;
namespace ElswordPixel {
 public static class BelowFortCameraFitV21 {
  public const float BaseWidth=480,BaseHeight=270,Padding=24;
  static bool Finite(float f){return !float.IsNaN(f)&&!float.IsInfinity(f);}
  public static void Fit(float left,float bottom,float right,float top,float aspect,float bottomHudFraction,float topHudFraction,float minimumHeight,out float x,out float y,out float width,out float height){
   if(!Finite(left)||!Finite(bottom)||!Finite(right)||!Finite(top)||right<left||top<bottom||!Finite(aspect)||aspect<=0||!Finite(bottomHudFraction)||!Finite(topHudFraction)||bottomHudFraction<0||topHudFraction<0||bottomHudFraction+topHudFraction>=.9f||!Finite(minimumHeight)||minimumHeight<BaseHeight)throw new ArgumentException("Below Fort camera fit inputs invalid");
   float need=Math.Max((top-bottom+2*Padding)/(1-bottomHudFraction-topHudFraction),(right-left+2*Padding)/aspect);
   height=Math.Max(minimumHeight,(float)Math.Ceiling(need/BaseHeight)*BaseHeight);width=height*aspect;
   x=(left+right)*.5f;float low=top+Padding-height*.5f+topHudFraction*height,high=bottom-Padding+height*.5f-bottomHudFraction*height;
   if(low>high+.001f)throw new InvalidOperationException("Below Fort fit cannot contain full actor footprint");y=(low+high)*.5f;
  }
 }
}
