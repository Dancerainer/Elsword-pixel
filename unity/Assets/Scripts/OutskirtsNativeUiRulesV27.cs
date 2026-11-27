using System;
namespace ElswordPixel {
 public static class OutskirtsNativeUiRulesV27 {
  public static bool TryAnchor(bool outskirts,string npc,float centerX,float topY,float barWidth,float hp,float maxHp,out float barX,out float barY,out float debuffX,out float debuffY){
   barX=barY=debuffX=debuffY=0;
   if(!outskirts||!OutskirtsNativeRenderRulesV27.Supported(npc))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY))throw new InvalidOperationException("Invalid actual Outskirts sprite bounds");
   if(float.IsNaN(barWidth)||float.IsInfinity(barWidth)||barWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid Outskirts HP bar metadata");
   barX=centerX-barWidth*(1-hp/maxHp)/2;debuffX=centerX;barY=topY+8;debuffY=topY+20;return true;
  }
 }
}
