using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  // The complete regional source retains bindings for NPCs outside either
  // Plain cohort. Project only its exact installed NPC cohort before calling
  // the unchanged generic state-binding validator. An unknown state for an
  // installed NPC is an error, rather than a silently dropped source row.
  // This selects existing bindings; it does not fabricate callback emissions
  // or absent bone tracks. Those require a separate exact callback contract.
  public void SetPlainOriginalNpcEffectsR1313(string route,string regionalRulesSHA,NpcEffectDocument regional){
   if(route!=plainRouteR1258||regionalRulesSHA!=PlainFxRulesR1304||plainDefinitionsR1258==null||!PlainOriginalRoomScopeR1258(route))throw new InvalidOperationException("Original Plain route/profiles must be bound before regional effects");
   if(regional==null||regional.definitions==null||regional.bindings==null||regional.runtimeAdmitted||regional.sourceMechanismsComplete)throw new ArgumentException("Missing/incorrect source-only original Plain regional effects");
   var owners=new HashSet<string>(StringComparer.Ordinal);foreach(var profile in plainDefinitionsR1258)owners.Add(profile.id);
   var bindings=new List<NpcEffectBinding>();var reactions=new List<NpcReactionEffectBinding>();
   foreach(var binding in regional.bindings){
    if(binding==null||string.IsNullOrEmpty(binding.npc)||string.IsNullOrEmpty(binding.state))throw new ArgumentException("Malformed original regional effect binding");
    if(!owners.Contains(binding.npc))continue;
    if(!npcStates.ContainsKey(binding.npc+"/"+binding.state))throw new ArgumentException("Original regional effect binding names an unknown installed Plain state");
    bindings.Add(binding);
   }
   if(regional.reactionBindings!=null)foreach(var reaction in regional.reactionBindings){
    if(reaction==null||string.IsNullOrEmpty(reaction.npc))throw new ArgumentException("Malformed original regional reaction binding");
    if(owners.Contains(reaction.npc))reactions.Add(reaction);
   }
   // Existing generic validation remains responsible for all definition,
   // child, sample, track, duplicate-state and reaction-coverage contracts.
   SetNpcEffectDefinitions(new NpcEffectDocument{runtimeAdmitted=regional.runtimeAdmitted,sourceMechanismsComplete=regional.sourceMechanismsComplete,definitions=regional.definitions,bindings=bindings.ToArray(),reactionBindings=reactions.ToArray()});
   BindPlainOriginalFxConsumersR1304(route,regionalRulesSHA);
  }
 }
}
