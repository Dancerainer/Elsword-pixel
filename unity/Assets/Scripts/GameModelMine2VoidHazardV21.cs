using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  public int Mine2VoidDefeatsV21 {get;private set;}
  // Authored2D pit hazard, not a recovered native death-plane constant.
  // The same600px margin already stops validation below all original floors.
  // Real falls defeat ordinary clear-counting enemies through normal damage;
  // no floor, position repair, gate unlock or clear predicate is supplied.
  void TickMine2VoidHazardV21(){
   if(mine2MechanismsV21==null||!NativeTerrain||!CurrentRoom.id.StartsWith("Dungeon_Besma_MINE2_Normal-",StringComparison.Ordinal))return;
   float bottom=float.PositiveInfinity;foreach(var s in CurrentRoom.surfaces)bottom=Math.Min(bottom,Math.Min(s.y1,s.y2));
   foreach(var e in enemies)if(e.active&&!e.dormant&&!e.passive&&!e.isGate&&e.countsForClear&&e.hp>0&&e.y<bottom-600){int dealt=ApplyHitToEnemy(e,(int)Math.Ceiling(e.hp),HitReaction.None);if(dealt>0&&e.hp<=0){Mine2VoidDefeatsV21++;Mine2EventV21(e,"authored-pit-defeat",elapsed,dealt);}}
  }
 }
}
