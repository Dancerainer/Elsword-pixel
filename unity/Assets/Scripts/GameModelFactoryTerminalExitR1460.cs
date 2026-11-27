using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ElswordPixel {
 public sealed partial class GameModel {
  static string FactoryExitBitsR1460(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0).ToString("X8",CultureInfo.InvariantCulture);}
  static string FactoryExitSurfacesR1460(GroundSurface[] rows){
   if(rows==null)return "";var values=new List<string>();
   foreach(var s in rows){if(s==null)return "";values.AddRange(new[]{s.id.ToString(CultureInfo.InvariantCulture),FactoryExitBitsR1460(s.x1),FactoryExitBitsR1460(s.y1),FactoryExitBitsR1460(s.x2),FactoryExitBitsR1460(s.y2),s.platform?"1":"0"});}
   return FactoryHashV542(Encoding.UTF8.GetBytes(string.Join("\n",values.ToArray())+"\n"));
  }
  // The original room envelope ends 0.3125 display pixels beyond its native
  // support. Reconcile this exact admitted terminal only; keep the source
  // exit, real floor, clear prerequisite, .05 contact and .9s delay intact.
  partial void ReconcileFactoryDefaultEnvelopeExitR1460(EncounterDefinition definition,ref float exit){
   if(factoryEntryBirthsV542==null||factoryEntryBirthsV542.Length!=64||factoryEntryRoomsV542==null||factoryEntryRoomsV542.Length!=3||rooms.Count!=3||room!=1||!ReferenceEquals(CurrentRoom,factoryEntryRoomsV542[1])||factoryPopulationPolicyV21==null||factoryEventBindingV21==null||!NativeTerrain||!HasEncounters||!gateOpen||!EncounterCombatClear)return;
   if(CurrentRoom.id!=FactoryRouteV542+"-1"||CurrentRoom.width!=4923f||CurrentRoom.encounters.Length!=3||currentEncounter!=2||definition==null||!ReferenceEquals(definition,CurrentEncounter)||definition.id!="1:2"||definition.sourceWorldId!="WI_ALTERA_NASOD_FACTORY_STAGE2"||definition.mainLineSet!=4||definition.direction!=1||FactoryExitBitsR1460(definition.leftX)!="457149A0"||FactoryExitBitsR1460(definition.rightX)!="4598D800"||FactoryExitBitsR1460(exit)!="4598D79A"||exit!=definition.exitX)return;
   if(FactoryExitSurfacesR1460(CurrentRoom.surfaces)!="717645d5c69a11bf1f5873013b86b6d2530f12a1abca116538694d1c9e7015f7")return;
   var clauses=definition.clauses;if(clauses==null||clauses.Length!=1)return;var clause=clauses[0];
   if(clause==null||clause.id!="CLEAR_COND0"||clause.mode!=EncounterClearMode.KillAllActive||!clause.requiresExit||clause.npcTypes==null||clause.npcTypes.Length!=0||clause.npcKeyCodes==null||clause.npcKeyCodes.Length!=0||clause.transitions==null||clause.transitions.Length!=1||!ReferenceEquals(ResolvedEncounterClause(),clause))return;
   var edge=clause.transitions[0];if(edge==null||edge.completeDungeon||edge.targetRoom!=2||edge.targetEncounter!=0||edge.weight!=100)return;
   var terminal=Surface(2);if(terminal==null||terminal.platform)return;float reachable=NativeClampX(exit);
   if(!Finite(reachable)||FactoryExitBitsR1460(reachable)!=FactoryExitBitsR1460(terminal.x2-.03f)||reachable<terminal.x1||reachable>terminal.x2)return;
   exit=reachable;
  }
 }
}
