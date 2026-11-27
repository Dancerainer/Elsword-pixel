using System;using System.Collections.Generic;using System.Security.Cryptography;using System.Text;
namespace ElswordPixel {public sealed partial class GameModel {
  static float CoreHardFloat32V31(float value){return BitConverter.ToSingle(BitConverter.GetBytes(value),0);}
  // Projection adapter for three exact Hard terminal ceil envelopes only.
  // Original source data, clear prerequisites, .05 contact and .9s transition pacing stay intact.
  static string CoreHardSurfaceReceiptV29(GroundSurface[] rows){if(rows==null)return "";var s=new List<string>();foreach(var r in rows){if(r==null)return "";s.AddRange(new[]{r.id.ToString(System.Globalization.CultureInfo.InvariantCulture),CoreHardBitsV21(r.x1),CoreHardBitsV21(r.y1),CoreHardBitsV21(r.x2),CoreHardBitsV21(r.y2),r.platform?"1":"0"});}using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",s.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();}
  partial void ReconcileCoreHardDefaultEnvelopeExitV29(EncounterDefinition definition,ref float exit){
   if(coreHardV21==null||coreHardV21.dungeonId!="DI_ALTERA_CORE_HARD"||coreHardV21.sourceDifficulty!="Hard"||coreHardV21.difficultyValue!=1||coreHardV21.sourceModelSha256!="6d0a134a37c7affd31473051c067757b8a1b0fbfc9a51b7f4a45be4fce30eeba"||coreHardV21.sourceRulesSha256!="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72"||!NativeTerrain||!HasEncounters||definition==null||!object.ReferenceEquals(definition,CurrentEncounter)||currentEncounter!=CurrentRoom.encounters.Length-1||definition.direction!=1||!gateOpen||!EncounterCombatClear)return;
   string world,node,left,right,sourceExit,receipt;float width;int count,line;
   switch(room){
    case 0:world="WI_ALTERA_CORE_STAGE1";width=5070f;count=2;node="0:1";left="452ED8B4";right="459D7000";sourceExit="459D6F9A";receipt="904e2b9f60e34744e37a7dc0039113907c4dc112cbf7ddd2db2d3b312f27f56d";line=9;break;
    case 1:world="WI_ALTERA_CORE_STAGE1_MIDDLE";width=559f;count=1;node="1:0";left="42000000";right="4403C000";sourceExit="4403BCCD";receipt="1650cd36dd02b8e1452ba3af6088f056d15a4e13ae78a6ea4a977fcf93dfa0db";line=2;break;
    case 2:world="WI_ALTERA_CORE_STAGE2";width=5489f;count=3;node="2:2";left="455EE2FF";right="45AA8800";sourceExit="45AA879A";receipt="63d319e2c73519f89c1848b021bd74e680ca55fdedb0c2aa3f532316954d0b03";line=20;break;
    default:return;
   }
   if(CurrentRoom.id!="Dungeon_Altera_Core_Hard-"+room||CurrentRoom.width!=width||CurrentRoom.encounters.Length!=count||definition.id!=node||definition.sourceWorldId!=world||CoreHardBitsV21(definition.leftX)!=left||CoreHardBitsV21(definition.rightX)!=right||CoreHardBitsV21(exit)!=sourceExit||exit!=definition.exitX||definition.rightX!=width-32||Math.Abs(exit-CoreHardFloat32V31(definition.rightX-.05f))>.0001f||CoreHardSurfaceReceiptV29(CurrentRoom.surfaces)!=receipt)return;
   var clauses=definition.clauses;if(clauses==null||clauses.Length!=1)return;var clause=clauses[0];if(clause==null||clause.id!="CLEAR_COND0"||clause.mode!=EncounterClearMode.KillAllActive||!clause.requiresExit||clause.npcTypes==null||clause.npcTypes.Length!=0||clause.npcKeyCodes==null||clause.npcKeyCodes.Length!=0||clause.transitions==null||clause.transitions.Length!=1||!object.ReferenceEquals(ResolvedEncounterClause(),clause))return;var edge=clause.transitions[0];if(edge==null||edge.completeDungeon||edge.targetRoom!=room+1||edge.targetEncounter!=0||edge.weight!=100)return;
   var terminal=Array.Find(CurrentRoom.surfaces,s=>s.id==line);if(terminal==null||terminal.platform)return;float reachable=CoreHardFloat32V31(NativeClampX(exit));if(!Finite(reachable)||CoreHardBitsV21(reachable)!=CoreHardBitsV21(terminal.x2-.03f)||reachable<terminal.x1||reachable>terminal.x2)return;
   exit=reachable;
  }
}}
