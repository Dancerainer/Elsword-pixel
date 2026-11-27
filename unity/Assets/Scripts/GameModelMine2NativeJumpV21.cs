using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  const float DashJumpSpeed = 644, DashAirSpeed = 331.52f;
  const float LongJumpGravity = 1120, FreeDashGravity = LongJumpGravity * 2.2f;
  float takeoffGravity = FreeDashGravity;

  bool Mine2NativeDashGravityV21 { get { return mine2MechanismsV21 != null && NativeTerrain && CurrentRoom.id.StartsWith("Dungeon_Besma_MINE2_Normal-", StringComparison.Ordinal); } }
  bool HasLongJumpTerrain {
   get { return NativeTerrain && (IsRegionalDungeonRoom(CurrentRoom.id) ||
    Mine2NativeDashGravityV21); }
  }

  // Evaluate once, while both feet are still on the takeoff surface. Heights
  // are feet-to-feet. Stick diagonals and keyboard running use the same arc.
  float DashGravityAtTakeoff(int direction) {
   if (!HasLongJumpTerrain) return FreeDashGravity;
   var origin = SurfaceBelow(player.x,player.y+.1f);
   if (origin == null) return FreeDashGravity;
   float edge = direction > 0 ? origin.x2 : origin.x1;
   float runway = (edge-player.x)*direction;
   float gravity = FreeDashGravity;
   foreach (var destination in CurrentRoom.surfaces) {
    if (destination == origin || destination.npcOnly || destination.hamelDisabled || IsFallSurface(destination)) continue;
    float lo = destination.x1, hi = destination.x2;
    if (HasEncounters) { lo=Math.Max(lo,CurrentEncounter.leftX); hi=Math.Min(hi,CurrentEncounter.rightX); }
    if (hi<=lo || (direction>0 ? hi-player.x : player.x-lo)<0) continue;
    float near = Clamp(player.x,lo,hi);
    float distance = Math.Abs(near-player.x);
    // From a continuous runway approach the step first. From a lip, the
    // opposite bank of a real gap may be farther ahead.
    if (distance>32 && (runway>34 || (near-edge)*direction<=0)) continue;
    float inset = Math.Min(8,(hi-lo)*.25f);
    float landing = Clamp(near+direction*inset,lo,hi);
    float rise = destination.Y(landing)-player.y;
    float travel = Math.Abs(landing-player.x);
    // Leave several frames of clearance for the discrete takeoff and for
    // the transition from ground run to horizontal air speed.
    float t = Math.Max((travel+8)/DashAirSpeed,2*(rise+6)/DashJumpSpeed);
    if (t<=0) continue;
    float allowed = 2*(DashJumpSpeed*t-rise-6)/(t*(t+1f/60));
    if (allowed >= FreeDashGravity || allowed < LongJumpGravity-110) continue;
    // Distant level roofs separated only by floor seams need no boost.
    if (rise<=75 && distance>32) {
     bool opening=false;
     for (float probe=edge+direction*6; (near-probe)*direction>0; probe+=direction*12) {
      var support=SurfaceBelow(probe,Math.Max(player.y,destination.Y(near))+7);
      if(support==null || IsFallSurface(support) || support.Y(probe)<Math.Min(player.y,destination.Y(near))-7) {opening=true;break;}
     }
     if(!opening) continue;
     // Extra airtime across an actual void also covers time spent against the
     // leading face of a raised intermediate ledge during takeoff.
     return LongJumpGravity;
    }
    gravity=Math.Min(gravity,Math.Max(LongJumpGravity,allowed));
   }
   return gravity;
  }

  bool Mine2DashRouteReachableV21(float gap,float rise){if(!HasLongJumpTerrain||rise>644f*644/(2*1120f)||gap>330)return false;float flight=(644+(float)Math.Sqrt(Math.Max(0,644f*644-2240*rise)))/1120;return Math.Max(0,gap)+26<=331.52f*flight;}
  bool Mine2NeedsDashRouteV21(float gap,float rise){if(!Mine2DashRouteReachableV21(gap,rise))return false;if(gap>100||rise>78)return true;if(gap<=0||rise<=7)return false;float flight=(420+(float)Math.Sqrt(Math.Max(0,420f*420-2240*rise)))/1120;return gap+26>130*flight;}
 }
}
