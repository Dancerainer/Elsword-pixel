using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  // Proposed hook in SetNpcMechanismDefinitions after generic validation.
  // Only Raven's two original FRONT registrations authorize this qualifier.
  void ValidateAlteraRavenFrontDamageRuleV18(NpcRetaliationRule r,Dictionary<string,NpcMechanismStateRule> sourceStates){
   if(r.npc!="NUI_RAVEN"){if(r.sourceDamageFromFrontOnly)throw new ArgumentException("Front-only Raven callback cannot qualify an unrelated identity");return;}
   NpcDefinition p;NpcMechanismStateRule cannon,rain;
   if(!r.sourceDamageFromFrontOnly||r.sourceState!="RAVEN_DAMAGE_SMALL_FRONT"||r.sourceCallback!="RAVEN_DAMAGE_STATE_START_FUNC"||r.cState!="RAVEN_SP_CANNON_BLADE_READY"||r.specialState!="RAVEN_SP_SPECIAL_ATTACK_READY"||r.timerIndex!=1||r.timerInitial!=0||!r.timerEnabled||!r.stateChangeArgument||r.minimumTimerExclusive!=1||r.maximumSourceDistance!=500||r.cHpMaximum!=.7||r.specialHpMaximum!=.4||r.cRollExclusive!=10||r.specialRollExclusive!=15||
    !npcDefinitions.TryGetValue(r.npc,out p)||!sourceStates.TryGetValue(r.npc+"/"+r.cState,out cannon)||!sourceStates.TryGetValue(r.npc+"/"+r.specialState,out rain)||!cannon.cooldownExplicit||cannon.cooldown!=10||!rain.cooldownExplicit||rain.cooldown!=20)throw new ArgumentException("Changed or unbound exact Raven front-damage callback");
   foreach(string state in new[]{"RAVEN_DAMAGE_SMALL_FRONT","RAVEN_DAMAGE_BIG_FRONT","RAVEN_DAMAGE_SMALL_BACK","RAVEN_DAMAGE_BIG_BACK"})if(!Array.Exists(p.states,s=>s.id==state))throw new ArgumentException("Missing exact Raven front/back source state identity");
  }
  // Proposed damage-entry signature receives the actual ApplyHit fromX.
  // This runs before any callback RNG/timer query or state request.
  bool AlteraRavenFrontDamageEntryAllowedV18(Enemy enemy,NpcRetaliationRule rule,float sourceDamageFromX){
   if(!rule.sourceDamageFromFrontOnly)return true;
   if(enemy.sourceNPC!="NUI_RAVEN")throw new InvalidOperationException("Unbound front-only source damage identity");
   if(!Finite(sourceDamageFromX))throw new InvalidOperationException("Raven front-only callback requires actual damage-origin X");
   // Flat facing projection is explicit. Native 3D front/back classification
   // and the coincident-origin tie are unmeasured; this candidate uses front
   // when origin==actorX, matching its documented inclusive half-space.
   if((sourceDamageFromX-enemy.x)*enemy.facing>=0)return true;
   MechanismEvent(enemy,"source-back-damage-callback-skipped");return false;
  }
 }
}
