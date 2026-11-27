using System;
namespace ElswordPixel {
 [Serializable]public sealed class FactoryPopulationPolicyV21 {public string schema,route,roomId,world,npc,spawnNpc,sourceCountMethod,sourceConditionHash,sourceFrameHash,adapter;public bool enabled,nativeExecuted,nativeEquivalent,runtimeAdmitted;public int roomIndex,sourceExclusiveMaximum;public float sourceFrameMoveAt;}
 [Serializable]public sealed class FactoryPopulationInspectionV21 {public int allAliveActors,excludedFutureInactiveActors,eligibleActors,alreadyEmittedRequests,returnedCount,encounter;public string adapter;}
 public sealed partial class GameModel {
  FactoryPopulationPolicyV21 factoryPopulationPolicyV21;
  public void SetFactoryPopulationPolicyV21(FactoryPopulationPolicyV21 d){
   if(d!=null){AlteraNasodPopulationRuleV18 rule;
    if(d.schema!="factory-v17-future-placeholder-count-policy-v21"||!d.enabled||d.nativeExecuted||d.nativeEquivalent||d.runtimeAdmitted||d.route!="Dungeon_Altera_Nasod_Factory_Normal"||d.roomIndex!=1||d.roomId!="Dungeon_Altera_Nasod_Factory_Normal-1"||d.world!="WI_ALTERA_NASOD_FACTORY_STAGE2"||d.npc!="NUI_NASOD_MAKER"||d.spawnNpc!="NUI_WALLY_9TH_TYPE2"||d.sourceCountMethod!="LiveNPCNumType_LUA"||d.sourceExclusiveMaximum!=5||d.sourceFrameMoveAt!=2||d.sourceConditionHash!="c790bdf950e4675f63f3b844f20b88b34e214924308b4533281a4b5eea8ad317"||d.sourceFrameHash!="7038248db52ca5c17d219305d6a05ee31d066c9eb928eff048f55dbf5efdfbcb"||d.adapter!="exclude-only-unentered-future-inactive-v17-placeholders-factory-room1-v21"||!alteraNasodPopulationRulesV18.TryGetValue(d.npc,out rule)||rule.spawnNpc!=d.spawnNpc||rule.emitAt!=2||rule.maximumLiveForBeginInclusive!=4||rule.ownerHpCost!=0||rule.sourceKeyCode!=0)throw new ArgumentException("Changed exact Factory-only count/source emission policy");
   }
   factoryPopulationPolicyV21=d;
  }
  bool FactoryPopulationScopeV21(string npc){var d=factoryPopulationPolicyV21;return d!=null&&npc==d.spawnNpc&&room==d.roomIndex&&CurrentRoom.id==d.roomId&&HasEncounters&&CurrentEncounter.sourceWorldId==d.world&&CurrentEncounter.id=="1:"+EncounterIndex;}
  public bool TryInspectFactoryPopulationV21(out FactoryPopulationInspectionV21 result){return InspectFactoryPopulationV21("NUI_WALLY_9TH_TYPE2",out result);}
  bool InspectFactoryPopulationV21(string npc,out FactoryPopulationInspectionV21 result){
   result=null;if(!FactoryPopulationScopeV21(npc))return false;var r=new FactoryPopulationInspectionV21{encounter=EncounterIndex,adapter=factoryPopulationPolicyV21.adapter};
   foreach(var e in enemies)if(e.hp>0&&e.sourceNPC==npc){r.allAliveActors++;if(e.encounterIndex>EncounterIndex&&!e.active)r.excludedFutureInactiveActors++;else r.eligibleActors++;}
   // Already issued requests are not future inactive display placeholders.
   // Keep the original room/alreadyEmitted/type predicate byte-for-byte.
   foreach(var request in npcSummonRequests)if(request.room==room&&request.alreadyEmitted&&request.npc==npc)r.alreadyEmittedRequests++;
   r.returnedCount=r.eligibleActors+r.alreadyEmittedRequests;result=r;return true;
  }
  bool TryFactoryPopulationCountV21(string npc,out int count){FactoryPopulationInspectionV21 r;bool supported=InspectFactoryPopulationV21(npc,out r);count=supported?r.returnedCount:0;return supported;}
 }
}
