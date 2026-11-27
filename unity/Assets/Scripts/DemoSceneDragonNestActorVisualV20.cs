using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  void ApplyDragonNestSourceVisibilityV20(Enemy owner,ref bool visible,ref bool gauge){
   bool sourceVisible,sourceGauge;
   if(game.TryDragonKingPresentationV20(owner,out sourceVisible,out sourceGauge)){
    visible&=sourceVisible;gauge&=sourceGauge;
   }
  }
  void ApplyDragonNestSourceColorV20(Enemy owner,SpriteRenderer sprite){
   float green,blue;
   if(!game.TryGetDragonBatSourceColorV20(owner,out green,out blue))return;
   // Preserve the ordinary red/alpha channels, including the death fade.
   // The source formula can be negative during the fifth state second. Pass
   // its exact float value to the renderer; do not stretch its four-second
   // pose clock or alter the original five-second state to hide that fact.
   var color=sprite.color;color.g=green;color.b=blue;sprite.color=color;
  }
 }
}
