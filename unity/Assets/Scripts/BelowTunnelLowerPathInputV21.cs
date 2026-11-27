using System;
namespace ElswordPixel {
 // Below preview input policy. Reads original supports and writes move/drop only.
 // The shared navigator's maximum fall, model physics, NPCs and terrain stay owned
 // by GameModel. This permits an ordinary platform drop in the source vertical shaft.
 public static class BelowTunnelLowerPathInputV21 {
  const float SideClearance=12;
  static float Clamp(float x,float lo,float hi){return Math.Max(lo,Math.Min(hi,x));}
  public static bool TryDrive(bool belowSession,GameModel game,Input input,Enemy target){
   if(!belowSession||game==null||input==null||target==null||game.RoomCount==0||!game.NativeTerrain||game.paused||game.victory||game.player.hp<=0||!game.CurrentRoom.id.StartsWith("Dungeon_Altera_Below_Tunnel_Normal-",StringComparison.Ordinal))return false;
   if(!game.enemies.Contains(target)||string.IsNullOrEmpty(target.sourceNPC)||!target.active||target.dormant||target.passive||target.hp<=0||game.HasEncounters&&target.encounterIndex!=game.EncounterIndex)return false;
   if(target.isGate?(!game.EncounterCombatClear||target.invincible>0):!target.countsForClear)return false;
   var p=game.player;var upper=game.SurfaceBelow(p.x,p.y+.1f);var lower=game.SurfaceBelow(target.x,target.y+.1f);
   if(upper==null||lower==null||!upper.platform||upper.id==lower.id||p.groundLine!=upper.id||p.dropThrough>0||p.vy>0||Math.Abs(p.y-upper.Y(p.x))>=.06f)return false;
   float lo=Math.Max(upper.x1,lower.x1),hi=Math.Min(upper.x2,lower.x2);
   if(game.HasEncounters){lo=Math.Max(lo,game.CurrentEncounter.leftX+.025f);hi=Math.Min(hi,game.CurrentEncounter.rightX-.025f);}
   if(hi-lo<=2*SideClearance)return false;
   float safeLo=lo+SideClearance,safeHi=hi-SideClearance;
   float portal=Clamp(target.x,safeLo,safeHi),dropX=Clamp(p.x,safeLo,safeHi);
   if(upper.Y(dropX)-lower.Y(dropX)<=300)return false;
   // The target's real support must also be the first support below the chosen
   // corridor; an intervening support is not assumed to be a transparent route.
   var landing=game.SurfaceBelow(dropX,upper.Y(dropX)-.1f);
   if(landing==null||landing.id!=lower.id)return false;
   if(p.x<safeLo||p.x>safeHi){input.move=Math.Sign(portal-p.x);return true;}
   input.move=0;input.drop=true;return true;
  }
 }
}
