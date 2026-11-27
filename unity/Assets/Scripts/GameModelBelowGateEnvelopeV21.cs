using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // Only the three original Below terminal envelopes use this <=1.03px
  // ceil-projection contact adapter. It changes no definition or support.
  partial void ReconcileBelowDefaultEnvelopeExitV21(EncounterDefinition definition,ref float exit){
   if(!BelowTunnelCandidateEnabledV21||!NativeTerrain||!HasEncounters||definition==null||!object.ReferenceEquals(definition,CurrentEncounter)||currentEncounter!=CurrentRoom.encounters.Length-1||definition.direction!=1)return;
   string world;float width,sourceEnd,sourceExit;int count;string encounter;
   switch(CurrentRoom.id){
    case "Dungeon_Altera_Below_Tunnel_Normal-0":world="WI_ALTERA_BELOW_TUNNEL_STAGE1";width=3705;sourceEnd=3672;sourceExit=3672.95f;count=2;encounter="0:1";break;
    case "Dungeon_Altera_Below_Tunnel_Normal-1":world="WI_ALTERA_BELOW_TUNNEL_STAGE2A";width=4744;sourceEnd=4711.08f;sourceExit=4711.95f;count=3;encounter="1:2";break;
    case "Dungeon_Altera_Below_Tunnel_Normal-2":world="WI_ALTERA_BELOW_TUNNEL_STAGE4";width=849;sourceEnd=816;sourceExit=816.95f;count=1;encounter="2:0";break;
    default:return;
   }
   if(definition.sourceWorldId!=world||CurrentRoom.width!=width||CurrentRoom.encounters.Length!=count||definition.id!=encounter||Math.Abs(definition.rightX-(CurrentRoom.width-32))>.0001f||exit!=sourceExit)return;
   float end=float.NegativeInfinity;GroundSurface terminal=null;
   foreach(var surface in CurrentRoom.surfaces)if(!surface.platform&&surface.x2>end){end=surface.x2;terminal=surface;}
   if(terminal==null||!Finite(end)||Math.Abs(end-sourceEnd)>.0005f)return;
   float reachable=NativeClampX(exit),gap=exit-reachable;
   if(gap<=0||gap>1.03f||Math.Abs(reachable-(end-.03f))>.0005f||reachable<terminal.x1||reachable>terminal.x2)return;
   exit=reachable;
  }
 }
}
