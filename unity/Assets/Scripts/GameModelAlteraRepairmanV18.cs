using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraRepairmanRuleV18 {
  public string npc,initialState,healState,restState,sourceTimerMethod,sound,sourceSoundName;
  public string[] decisionStates=new string[0],priorityTypes=new string[0];
  public float range,enterMaximumHpFraction,healMaximumHpFractionPerCall,animationThreshold,sourceAnimationThreshold,sourceAnimationSpeed,conditionEvaluationInterval;
 }
 [Serializable] public sealed class AlteraRepairmanEventV18 {
  public string type,source,npc,target;public float time,x,y,z,before,after;
 }
 public sealed partial class GameModel {
  readonly Dictionary<string,AlteraRepairmanRuleV18> alteraRepairmanRulesV18=new Dictionary<string,AlteraRepairmanRuleV18>();
  readonly HashSet<Enemy> alteraRepairmanHiddenGaugeV18=new HashSet<Enemy>();
  public readonly List<AlteraRepairmanEventV18> alteraRepairmanEventsV18=new List<AlteraRepairmanEventV18>();
  public void SetAlteraRepairmanRulesV18(AlteraRepairmanRuleV18[] definitions){
   var next=new Dictionary<string,AlteraRepairmanRuleV18>();
   foreach(var r in definitions??new AlteraRepairmanRuleV18[0]){
    if(r==null||r.npc!="NUI_AIRSHIP_SOLDIER_REPAIRMAN"||next.ContainsKey(r.npc)||r.initialState!="AIRSHIP_SOLDIER_REPAIRMAN_START"||r.healState!="AIRSHIP_SOLDIER_REPAIRMAN_HEAL"||r.restState!="AIRSHIP_SOLDIER_REPAIRMAN_WAIT"||
      r.sourceTimerMethod!="AnimTimer_LUA"||!Finite(r.range)||Math.Abs(r.range-98)>.0001f||r.enterMaximumHpFraction!=.5f||r.healMaximumHpFractionPerCall!=.2f||r.sourceAnimationThreshold!=.9f||r.sourceAnimationSpeed!=1||Math.Abs(r.conditionEvaluationInterval-.8f)>.00001f||
      r.priorityTypes==null||r.priorityTypes.Length!=2||r.priorityTypes[0]!="NUI_CANNON_BIG"||r.priorityTypes[1]!="NUI_CANNON_HEAVY_MACHINE_GUN"||r.decisionStates==null||r.decisionStates.Length!=2||
      r.decisionStates[0]!="AIRSHIP_SOLDIER_REPAIRMAN_WAIT"||r.decisionStates[1]!="AIRSHIP_SOLDIER_REPAIRMAN_WALK")throw new ArgumentException("Changed original Airship Repairman source contract");
    NpcDefinition profile;NpcStateDefinition heal;NpcMechanismStateRule mechanism;
    if(!npcDefinitions.TryGetValue(r.npc,out profile)||!npcStates.TryGetValue(r.npc+"/"+r.healState,out heal)||heal.sourceAnimation!="Repair"||heal.sourceAnimSpeed!=r.sourceAnimationSpeed||!heal.loop||
      !Finite(r.animationThreshold)||Math.Abs(r.animationThreshold-r.sourceAnimationThreshold/heal.sourceAnimSpeed)>.00001f||
      !npcMechanismRules.TryGetValue(r.npc+"/"+r.healState,out mechanism)||!mechanism.cooldownExplicit||mechanism.cooldown!=5)
      throw new ArgumentException("Repairman profile/state/source5s mechanism is missing or different");
    foreach(string state in new[]{r.initialState,r.restState,r.decisionStates[0],r.decisionStates[1]}){
     NpcStateDefinition bound;if(!npcStates.TryGetValue(r.npc+"/"+state,out bound))throw new ArgumentException("Missing exact Repairman source state "+state);
     if(Array.Exists(r.decisionStates,s=>s==state)&&Math.Abs(bound.interval-r.conditionEvaluationInterval)>.00001f)throw new ArgumentException("Changed Repairman WAIT/WALK source interval");
    }
    if(string.IsNullOrEmpty(r.sound)!=string.IsNullOrEmpty(r.sourceSoundName)||!string.IsNullOrEmpty(r.sound)&&(r.sourceSoundName!="Airship_RepairMan_Repair.ogg"||!npcSourceSoundAssetsV17.Contains(r.sound)))throw new ArgumentException("Unbound original Repairman callback sound");
    next.Add(r.npc,r);
   }
   alteraRepairmanRulesV18.Clear();foreach(var row in next)alteraRepairmanRulesV18.Add(row.Key,row.Value);ClearAlteraRepairmanRoomV18();
  }
  void ClearAlteraRepairmanRoomV18(){alteraRepairmanEventsV18.Clear();alteraRepairmanHiddenGaugeV18.Clear();}
  public bool AlteraRepairmanGaugeHiddenV18(Enemy owner){return owner!=null&&alteraRepairmanHiddenGaugeV18.Contains(owner);}
  // Called after this source profile is found but before the ordinary target
  // alert gate. Original START and repair conditions run without player aggro.
  bool EnsureAlteraRepairmanInitialStateV18(Enemy owner,NpcDefinition profile){
   AlteraRepairmanRuleV18 rule;if(owner==null||profile==null||owner.hp<=0||!owner.active||owner.dormant||!alteraRepairmanRulesV18.TryGetValue(owner.sourceNPC??"",out rule))return false;
   if(NpcState(owner)==null)TryEnterNpcState(owner,npcStates[rule.npc+"/"+rule.initialState]);return true;
  }
  // This is an explicit candidate projection of GetNearNPCUnitUID. The host
  // must retain its boundary: native list lifetime/3D route distance is not
  // measured. Future/unspawned actors are absent; nearest type is selected
  // before wounded/live eligibility, matching the source query-then-test order.
  Enemy NearestAlteraRepairTypeV18(Enemy owner,string npc,float range){
   Enemy nearest=null;float best=range*range;
   foreach(var e in enemies){
    if(e==owner||!e.active||e.dormant||e.sourceNPC!=npc)continue;
    float dx=e.x-owner.x,dy=e.y-owner.y,dz=(e.nativeDepth-owner.nativeDepth)*.28f,d=dx*dx+dy*dy+dz*dz;
    if(d<=best&&(nearest==null||d<best)){nearest=e;best=d;}
   }
   return nearest;
  }
  Enemy AlteraRepairmanTargetV18(Enemy owner,AlteraRepairmanRuleV18 rule,bool starting){
   foreach(string type in rule.priorityTypes){
    var candidate=NearestAlteraRepairTypeV18(owner,type,rule.range);
    if(candidate==null||candidate.hp<=0||candidate.maxHp<=0)continue;
    if(starting?candidate.hp>candidate.maxHp*rule.enterMaximumHpFraction:candidate.hp>=candidate.maxHp)continue;
    return candidate;
   }
   return null;
  }
  bool CanStartAlteraRepairmanV18(Enemy owner,out Enemy target){
   AlteraRepairmanRuleV18 rule;target=null;
   if(owner==null||owner.hp<=0||!owner.active||owner.dormant||!alteraRepairmanRulesV18.TryGetValue(owner.sourceNPC??"",out rule))return false;
   target=AlteraRepairmanTargetV18(owner,rule,true);return target!=null;
  }
  void AlteraRepairmanTraceV18(string type,string source,Enemy owner,Enemy target,float at,float before=0,float after=0){
   if(alteraRepairmanEventsV18.Count>=512)alteraRepairmanEventsV18.RemoveAt(0);
   alteraRepairmanEventsV18.Add(new AlteraRepairmanEventV18{type=type,source=source,npc=owner.sourceNPC,target=target==null?"":target.sourceNPC,
    time=at,x=target==null?owner.x:target.x,y=target==null?owner.y:target.y,z=target==null?owner.nativeDepth:target.nativeDepth,before=before,after=after});
  }
  // A host evaluating the original SCT_CONDITION_FUNCTION supplies the true
  // AnimTimer_LUA result. This helper does not replace it with a world timer,
  // event crossing, area healing, or an extra resource/cooldown condition.
  bool ApplyAlteraRepairmanPredicateV18(Enemy owner,bool sourceAnimTimerResult,float at){
   AlteraRepairmanRuleV18 rule;
   if(owner==null||owner.hp<=0||!alteraRepairmanRulesV18.TryGetValue(owner.sourceNPC??"",out rule)||!sourceAnimTimerResult)return false;
   AlteraRepairmanTraceV18("source-sound-request","Airship_RepairMan_Repair.ogg",owner,null,at);
   if(!string.IsNullOrEmpty(rule.sound))QueueNpcSound(new NpcTimedSound{id="source-repairman/CF_AIRSHIP_SOLDIER_REPAIRMAN_REPAIR",sound=rule.sound,sourceName=rule.sourceSoundName,sourceField="PlaySound_LUA",
    candidateClock="source-condition-predicate",candidateTime=owner.npcTime,sourceTime=owner.npcTime*rule.sourceAnimationSpeed},owner,++npcSoundEntrySerial,0,at,owner.npcState);
   var target=AlteraRepairmanTargetV18(owner,rule,false);if(target==null)return false;
   float before=target.hp,healed=before+target.maxHp*rule.healMaximumHpFractionPerCall;bool complete=healed>target.maxHp;
   RecoverEnemyHealth(target,healed-before,rule.healMaximumHpFractionPerCall*100);
   AlteraRepairmanTraceV18("source-visual-request","Heal_Effect01",owner,target,at);
   AlteraRepairmanTraceV18("source-visual-request","LobbyLevelUp01",owner,target,at);
   AlteraRepairmanTraceV18("repair", "CF_AIRSHIP_SOLDIER_REPAIRMAN_REPAIR",owner,target,at,before,target.hp);
   return complete;
  }
  // Shared integration hooks. The source condition function precedes ordinary
  // attack choices in WAIT/WALK, including when no player target was acquired.
  bool TryAlteraRepairmanDecisionV18(Enemy owner,NpcStateDefinition state,bool sourceDecisionTick){
   AlteraRepairmanRuleV18 rule;if(!sourceDecisionTick||owner==null||state==null||!alteraRepairmanRulesV18.TryGetValue(owner.sourceNPC??"",out rule)||!Array.Exists(rule.decisionStates,s=>s==state.id))return false;
   Enemy target;if(!CanStartAlteraRepairmanV18(owner,out target))return false;
   return TryEnterNpcState(owner,npcStates[rule.npc+"/"+rule.healState]);
  }
  void OnAlteraRepairmanStateEnteredV18(Enemy owner,NpcStateDefinition state){
   AlteraRepairmanRuleV18 rule;if(owner==null||state==null||!alteraRepairmanRulesV18.TryGetValue(owner.sourceNPC??"",out rule))return;
   if(state.id==rule.restState)alteraRepairmanHiddenGaugeV18.Add(owner);
   if(state.id==rule.healState){var target=AlteraRepairmanTargetV18(owner,rule,true);if(target!=null)owner.facing=target.x>=owner.x?1:-1;}
  }
  // Candidate AnimTimer mapping is deliberately named here: level predicate
  // of current animation pose, evaluated once per real source-state packet.
  // It is not AnimEventTimer crossing and has no invented world cooldown.
  bool AdvanceAlteraRepairmanHealV18(Enemy owner,NpcStateDefinition state,float to,float at){
   AlteraRepairmanRuleV18 rule;if(owner==null||state==null||!alteraRepairmanRulesV18.TryGetValue(owner.sourceNPC??"",out rule)||state.id!=rule.healState)return false;
   if(ApplyAlteraRepairmanPredicateV18(owner,to+1e-6f>=rule.animationThreshold,at))TryEnterNpcState(owner,npcStates[rule.npc+"/"+rule.restState]);
   else if(to+1e-6f>=state.duration)owner.npcTime=0; // Original Repair is XAP_LOOP, not a held final pose.
   return true;
  }
 }
}
