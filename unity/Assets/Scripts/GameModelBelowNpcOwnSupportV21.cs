using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // Only these original Below actors use their own Y ceiling at the two
  // NPC-context read sites. GroundAt(x) remains player-context everywhere else.
  float BelowNpcOwnFloorV21(string npc,float x,float y,bool ignorePlatforms){
   float original=GroundAt(x);
   if(!BelowTunnelCandidateEnabledV21||!NativeTerrain||belowVisibilityV21==null||!Array.Exists(belowVisibilityV21.rows,p=>p.npc==npc))return original;
   string world;int count;
   switch(CurrentRoom.id){
    case "Dungeon_Altera_Below_Tunnel_Normal-0":world="WI_ALTERA_BELOW_TUNNEL_STAGE1";count=2;break;
    case "Dungeon_Altera_Below_Tunnel_Normal-1":world="WI_ALTERA_BELOW_TUNNEL_STAGE2A";count=3;break;
    case "Dungeon_Altera_Below_Tunnel_Normal-2":world="WI_ALTERA_BELOW_TUNNEL_STAGE4";count=1;break;
    default:return original;
   }
   if(CurrentRoom.encounters==null||CurrentRoom.encounters.Length!=count||Array.Exists(CurrentRoom.encounters,e=>e.sourceWorldId!=world))return original;
   var support=SurfaceBelow(x,y+.05f,ignorePlatforms);
   return support==null?original:support.Y(x);
  }
  bool TryEnemyHurtFloor(Enemy enemy,out float floor){
   if(!NativeTerrain){floor=GroundAt(CurrentRoom,enemy.x);return true;}
   // A hero attacking uphill can stand below the target's ground. Only the
   // target's own feet determine whether an airborne hurt reaction applies.
   var support=SurfaceBelow(enemy.x,enemy.y+.05f,enemy.dropThrough>0,UsesNpcRoad(enemy));
   floor=support==null?0:support.Y(enemy.x);return support!=null;
  }
 }
}
