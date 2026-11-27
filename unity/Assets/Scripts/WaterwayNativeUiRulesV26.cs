using System;
namespace ElswordPixel {
 public static class WaterwayNativeUiRulesV26 {
  public static bool TryAnchor(bool waterway,string npc,float centerX,float topY,float barWidth,float hp,float maxHp,out float barX,out float barY,out float debuffX,out float debuffY){
   barX=barY=debuffX=debuffY=0;
   if(!waterway||(!WaterwayNativeRenderRulesV26.Mouse(npc)&&!WaterwayNativeRenderRulesV26.Three(npc)))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY))throw new InvalidOperationException("Invalid actual waterway sprite bounds");
   if(float.IsNaN(barWidth)||float.IsInfinity(barWidth)||barWidth<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid waterway HPbar metadata");
   // Keep the exact original leftedge while width shrinks with remainingHP.
   barX=centerX-barWidth*(1-hp/maxHp)/2;debuffX=centerX;barY=topY+8;debuffY=topY+20;return true;
  }
 }
}
