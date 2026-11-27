using System;
namespace ElswordPixel {
 // Both the HUD and the preview controls use the same walk-off and drop-through points.
 public static class Mine2LowerPathInputV21 {
  const float EdgeClearance=12;
  static float Clamp(float x,float lo,float hi){return Math.Max(lo,Math.Min(hi,x));}
  static bool Within(float x,GroundSurface s){return x>=s.x1+.02f&&x<=s.x2-.02f;}
  public static bool TryDrive(GameModel game,Input input,Enemy target){
   if(input==null||!TryGetDescent(game,target,out float x,out _,out bool drop))return false;
   input.move=Math.Sign(x-game.player.x);input.drop=drop;return true;
  }
  internal static bool TryGetDescent(GameModel game,Enemy target,out float x,out float y,out bool drop){
   x=y=0;drop=false;
   if(game==null||target==null||target.sourceNPC!="NUI_NASOD_RAIL_HOUSE"||!target.active||target.dormant||target.hp<=0||game.RoomCount==0||!game.NativeTerrain||!game.CurrentRoom.id.StartsWith("Dungeon_Besma_MINE2_Normal-",StringComparison.Ordinal))return false;
   var p=game.player;var upper=game.SurfaceBelow(p.x,p.y+.1f);var lower=game.SurfaceBelow(target.x,target.y+.1f);
   if(upper==null||lower==null||upper.id==lower.id)return false;
   float lo=Math.Max(upper.x1,lower.x1),hi=Math.Min(upper.x2,lower.x2);if(lo>=hi||lower.Y(Clamp(p.x,lower.x1,lower.x2))>=upper.Y(Clamp(p.x,upper.x1,upper.x2))-8)return false;
   bool grounded=Math.Abs(p.y-upper.Y(p.x))<.06f&&p.vy<=0;
   if(!grounded){
    // Hold the walked-off edge over its real intervening platform. Replanning
    // horizontally in mid-fall can turn back under the solid before landing.
    if(p.vy<=0&&upper.platform&&Within(p.x,upper)&&Within(p.x,lower)&&p.y>upper.Y(p.x)+.06f&&p.y-upper.Y(p.x)<300){x=p.x;y=upper.Y(x);return true;}return false;
   }
   if(upper.platform){
    if(Within(p.x,lower)){x=p.x;y=lower.Y(x);drop=true;return true;}
    float margin=Math.Min(EdgeClearance,(hi-lo)*.25f);x=Clamp(target.x,lo+margin,hi-margin);y=upper.Y(x);return true;
   }
   float best=float.PositiveInfinity,exit=0;
   foreach(int side in new[]{-1,1}){
    float edge=side<0?upper.x1:upper.x2;
    float walkOff=edge+side*EdgeClearance;if(!Within(walkOff,lower))continue;
    if(game.HasEncounters&&(walkOff<game.CurrentEncounter.leftX||walkOff>game.CurrentEncounter.rightX))continue;
    var catchSurface=game.SurfaceBelow(walkOff,upper.Y(edge)+.1f);
    if(catchSurface==null||catchSurface.Y(walkOff)>=upper.Y(edge)-8)continue;
    float cost=Math.Abs(walkOff-p.x)+Math.Abs(target.x-walkOff);if(cost<best){best=cost;exit=walkOff;y=upper.Y(edge);}
   }
   if(float.IsPositiveInfinity(best))return false;x=exit;return true;
  }
 }
}
