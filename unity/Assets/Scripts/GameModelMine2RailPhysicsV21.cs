using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // Original NUI_NASOD_RAIL: FALL_DOWN=FALSE, DAMAGE_DOWN=FALSE,
  // NASOD_RAIL_DAMAGE.NEVER_MOVE=TRUE and ONLY_THIS_LINE_GROUP=TRUE.
  // This scoped 2D adapter preserves real support spans; native flag handling
  // is not claimed measured. It neither supplies a floor nor repairs a fall.
  HitReaction Mine2RailReactionV21(Enemy e,HitReaction reaction){reaction=CargoSourceGroundReactionV1468(e,Remaining3ReactionV28(e,reaction));return Mine2RailBoundV21(e)&&(reaction==HitReaction.Launch||reaction==HitReaction.Knockdown)?HitReaction.Hurt:reaction;}
  bool Mine2RailRejectDownV21(Enemy e){return Mine2RailBoundV21(e);}
  float Mine2RailNativeMoveV21(Fighter fighter,float target){
   var e=fighter as Enemy;if(!Mine2RailBoundV21(e)||e.hp<=0||!NativeGrounded(e))return target;
   if(e.state==CombatState.Hurt||e.npcState=="NASOD_RAIL_DAMAGE")return e.x;
   var group=Array.Find(mine2MechanismsV21.lineGroups,g=>g.roomId==CurrentRoom.id&&Array.IndexOf(g.surfaceIds,e.groundLine)>=0);
   return group==null?target:Clamp(target,group.startX+.03f,group.endX-.03f);
  }
 }
}
