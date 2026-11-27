using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class CoreHardLocalAddPointV21 {public float at,x,y,z;}
 [Serializable] public sealed class CoreHardLocalAddsDocumentV21 {public string schema,sourceDifficulty,healState,sourceCallback,healCallbackHash,sourceBone,preEffect;public bool enabled,nativeExecuted,nativeEquivalent,runtimeAdmitted,hostOnlyHpWrites;public float healAt,preEffectAt,sourceRange,range,healFraction,sourceCooldown;public string[] excludes,targetEffects;public CoreHardLocalAddPointV21[] points;}
 [Serializable] public sealed class CoreHardLocalAddEventV21 {public string kind,npc,target,sourceCallback,particle;public float at,x,y,z,amount;public bool authoritativeHpWrite,visualConsumerBound;}
 public sealed partial class GameModel {
  CoreHardLocalAddsDocumentV21 coreHardLocalAddsV21;
  public readonly List<CoreHardLocalAddEventV21> coreHardLocalAddEventsV21=new List<CoreHardLocalAddEventV21>();
  public bool CoreHardLocalAddsConfiguredV21{get{return coreHardLocalAddsV21!=null&&CoreHardClosureConfiguredV21;}}
  public void SetCoreHardLocalAddsV21(CoreHardLocalAddsDocumentV21 d){
   if(d!=null){NpcStateDefinition heal,front,back,attack;NpcMechanismStateRule cd;
    if(!CoreHardClosureConfiguredV21||d.schema!="core-hard-local-add-source-callbacks-v21"||d.sourceDifficulty!="Hard"||!d.enabled||d.nativeExecuted||d.nativeEquivalent||d.runtimeAdmitted||!d.hostOnlyHpWrites||d.healState!="NASOD_HEALER_BLADER_HEAL"||d.sourceCallback!="NASOD_HEALER_BLADER_HEAL_FRAME_MOVE"||d.sourceBone!="Bone_Jewelry"||d.healAt!=.63f||d.preEffectAt!=.43f||d.sourceRange!=700||d.range!=196||d.healFraction!=.5f||d.sourceCooldown!=7||d.preEffect!="TimeStop1"||d.points==null||d.points.Length!=2||d.targetEffects==null||d.targetEffects.Length!=2||d.targetEffects[0]!="Heal_Effect01"||d.targetEffects[1]!="LobbyLevelUp01"||d.excludes==null||d.excludes.Length!=8)throw new ArgumentException("Changed exact Core add source callback contract");
    if(d.healCallbackHash!="59f5c398d0bf642b98ed67e1f6a7a3c97e163e1c0c8861c6c8ab86cad5d40477")throw new ArgumentException("Changed exact original HealerBlader callback words");
    var exact=new HashSet<string>(new[]{"NUI_NASOD_KING","NUI_NASOD_KING_HENIR","NUI_EL_POWER_PLANT","NUI_EL_POWER_PLANT_HENIR","NUI_EL_EXTRACTOR_FIRE","NUI_EL_EXTRACTOR_ICE","NUI_EL_EXTRACTOR_EARTH","NUI_EL_EXTRACTOR_NOVA"},StringComparer.Ordinal);foreach(string id in d.excludes)if(!exact.Remove(id))throw new ArgumentException("Changed exact HealerBlader exclusions");
    for(int i=0;i<2;i++){var p=d.points[i];if(p==null||p.at!=(i==0?.43f:.63f)||Math.Abs(p.x-.0415170355f)>.000001f||Math.Abs(p.y-16.9835088f)>.00001f||Math.Abs(p.z-.0239690772f)>.000001f)throw new ArgumentException("Changed exact original Bone_Jewelry source samples");}
    if(!npcStates.TryGetValue("NUI_NASOD_HEALER_BLADER/"+d.healState,out heal)||heal.sourceAnimation!="Healing"||heal.sourceAnimSpeed!=1||!npcMechanismRules.TryGetValue("NUI_NASOD_HEALER_BLADER/"+d.healState,out cd)||!cd.cooldownExplicit||cd.cooldown!=7||cd.initialElapsed!=0||!npcStates.TryGetValue("NUI_WALLY_9TH_TYPE2/WALLY_9TH_TYPE2_STAND_UP_FRONT",out front)||!npcStates.TryGetValue("NUI_WALLY_9TH_TYPE2/WALLY_9TH_TYPE2_STAND_UP_BACK",out back)||!front.superArmor||!back.superArmor||!npcStates.TryGetValue("NUI_WALLY_9TH_TYPE2/WALLY_9TH_TYPE2_ATTACK",out attack)||attack.damageChanges.Length!=2||attack.damageChanges[0].time!=.4f||attack.damageChanges[1].time!=1||!attack.damageChanges[0].clearHitList||!attack.damageChanges[1].clearHitList)throw new ArgumentException("Missing exact source add clocks/cooldown/phase/armor bindings");
   }
   coreHardLocalAddsV21=d;ClearCoreHardLocalAddActorsV21();
  }
  void ClearCoreHardLocalAddActorsV21(){coreHardLocalAddEventsV21.Clear();}
  void CoreHardLocalAddEventV21(string kind,Enemy owner,Enemy target,string particle,float at,float x,float y,float z,float amount=0,bool hpWrite=false){if(coreHardLocalAddEventsV21.Count>=4096)coreHardLocalAddEventsV21.RemoveAt(0);coreHardLocalAddEventsV21.Add(new CoreHardLocalAddEventV21{kind=kind,npc=owner.sourceNPC,target=target==null?null:target.sourceNPC,particle=particle,sourceCallback=owner.sourceNPC=="NUI_NASOD_HEALER_BLADER"?"NASOD_HEALER_BLADER_HEAL_FRAME_MOVE":owner.npcState+"_FRAME_MOVE",at=at,x=x,y=y,z=z,amount=amount,authoritativeHpWrite=hpWrite,visualConsumerBound=false});}
  void AdvanceCoreHardLocalAddsV21(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(coreHardLocalAddsV21==null||owner==null||owner.hp<=0||state==null||!CoreHardLocalAddOwnerEligibleR1417(owner))return;
   var d=coreHardLocalAddsV21;
   if(owner.sourceNPC=="NUI_WALLY_9TH_TYPE2"){
    if(state.id=="WALLY_9TH_TYPE2_ATTACK")foreach(var change in state.damageChanges)if(from<change.time&&change.time<=to)CoreHardLocalAddEventV21("existing-source-damage-phase",owner,null,change.damageProfile,packetStart+change.time-from,owner.x,owner.y,owner.nativeDepth*.28f);
    if((state.id=="WALLY_9TH_TYPE2_STAND_UP_FRONT"||state.id=="WALLY_9TH_TYPE2_STAND_UP_BACK")&&from*state.sourceAnimSpeed<=1.5f&&to*state.sourceAnimSpeed>1.5f)CoreHardLocalAddEventV21("source-idempotent-superarmor-true",owner,null,null,packetStart+1.5f/state.sourceAnimSpeed-from,owner.x,owner.y,owner.nativeDepth*.28f);
    return;
   }
   if(owner.sourceNPC!="NUI_NASOD_HEALER_BLADER"||state.id!=d.healState)return;
   for(int i=0;i<2;i++){
    var bone=d.points[i];if(!(from<bone.at&&bone.at<=to))continue;
    float at=packetStart+bone.at-from,x=owner.x+owner.facing*bone.x*owner.visualScale,y=owner.y+bone.y*owner.visualScale,z=owner.nativeDepth*.28f+bone.z*owner.visualScale;
    if(i==0){CoreHardLocalAddEventV21("source-particle-request-HOLD",owner,null,d.preEffect,at,x,y,z);continue;}
    CoreHardLocalAddEventV21("source-heal-query",owner,null,null,at,x,y,z);
    foreach(var target in enemies){
     if(!CoreHardHealerTargetEligibleR1417(owner,target)||target.hp>=target.maxHp)continue;
     float dx=target.x-x,dy=target.y-y,dz=target.nativeDepth*.28f-z;if(dx*dx+dy*dy+dz*dz>=d.range*d.range)continue;
     foreach(string effect in d.targetEffects)CoreHardLocalAddEventV21("source-particle-request-HOLD",owner,target,effect,at,target.x,target.y,target.nativeDepth*.28f);
     float amount=Math.Min(target.maxHp-target.hp,target.maxHp*d.healFraction);
     if(NpcMechanismIsHost){float beforeR1417=target.hp;RecoverEnemyHealth(target,amount,d.healFraction*100);CoreHardRecordProductHealR1417(owner,target,beforeR1417,target.hp);CoreHardLocalAddEventV21("source-real-heal",owner,target,null,at,target.x,target.y,target.nativeDepth*.28f,amount,true);}
     else CoreHardLocalAddEventV21("local-nonhost-hp-write-rejected",owner,target,null,at,target.x,target.y,target.nativeDepth*.28f,0,false);
    }
   }
  }
 }
}
