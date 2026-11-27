#if UNITY_EDITOR
using System;using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool Mine2OpenEdgeCombatV21(Enemy target,out int direction,out float margin){
   direction=Math.Sign(target.x-game.player.x);margin=float.PositiveInfinity;if(!mine2PreviewSessionV21||!game.NativeTerrain||!game.Grounded(game.player)||direction==0)return false;
   var s=game.SurfaceBelow(game.player.x,game.player.y+.1f);if(s==null)return false;float edge=direction<0?s.x1:s.x2;margin=Math.Abs(edge-game.player.x);if(margin>110)return false;
   return game.SurfaceBelow(edge+direction*.05f,s.Y(edge)+7)==null;
  }
  static bool Mine2StationarySkillV21(string id){return id=="flame-geyser"||id=="armor-break"||id=="iron-body"||id=="kick"||id=="armageddon-blade";}
 }
}
#endif
