using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class CoreHardSupportEventR1417 {
  public int sequence,room,encounter;public string kind,npc,state,target;
  public float at,before,after,fromX;public bool frontGuard,attackDevice,defenceDevice,productTargetOverride;
 }
 public sealed partial class GameModel {
  // Explicit local Hard balance selected for this product. The original Lua
  // names ATTACK_ACCEL/DEFENCE_ACCEL do not prove native numerical factors.
  public const float CoreHardAttackDeviceMultiplierR1417=1.25f;
  public const float CoreHardDefenceDeviceDamageTakenR1417=.70f;
  // These percentages are original GUARD_DEFENCE values in the frozen rules.
  public const float CoreHardGuardSourceDefenceR1417=70;
  public const float CoreHardGuardAttackBSourceDefenceR1417=50;
  readonly Enemy[] coreHardDotSupportOwnersR1417=new Enemy[2];
  int coreHardSupportSequenceR1417;
  public readonly List<CoreHardSupportEventR1417> coreHardSupportEventsR1417=new List<CoreHardSupportEventR1417>();
  void ClearCoreHardSupportRulesR1417(){coreHardSupportEventsR1417.Clear();coreHardSupportSequenceR1417=0;ClearCoreHardDotSupportOwnersR1417();}
  void ClearCoreHardDotSupportOwnersR1417(){for(int i=0;i<coreHardDotSupportOwnersR1417.Length;i++)coreHardDotSupportOwnersR1417[i]=null;}
  bool CoreHardSupportScopeR1417(){return CoreHardLocalBalanceRoomV21()&&coreHardV21.sourceDifficulty=="Hard"&&coreHardV21.npcLevel==37;}
  bool CoreHardSupportActorR1417(Enemy e){return e!=null&&CoreHardSupportScopeR1417()&&e.active&&!e.dormant&&e.hp>0&&e.state!=CombatState.Dead&&e.encounterIndex==EncounterIndex&&enemies.Contains(e)&&(coreHardBalancedActorsV21.Contains(e)||coreHardLocalActorsV21.Contains(e));}
  bool CoreHardCombatMonsterR1417(Enemy e){
   if(!CoreHardSupportActorR1417(e)||e.passive||e.isGate||string.IsNullOrEmpty(e.sourceNPC))return false;
   // These are the actual combat families in the original 23-profile Core set.
   // Alarm controllers, PD, event controllers, plants and extractors are not
   // monsters receiving combat support or the product healer override.
   return e.sourceNPC.StartsWith("NUI_NASOD_",StringComparison.Ordinal)||e.sourceNPC=="NUI_RAVEN_CLONE_B"||e.sourceNPC=="NUI_WALLY_9TH_TYPE2";
  }
  Enemy CoreHardLiveBuffDeviceR1417(string npc){
   if(!CoreHardSupportScopeR1417())return null;
   foreach(var e in enemies)if(e.sourceNPC==npc&&CoreHardSupportActorR1417(e))return e;
   return null;
  }
  public bool CoreHardBuffDeviceDamageableR1417(Enemy e){return CoreHardSupportActorR1417(e)&&(e.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_A"||e.sourceNPC=="NUI_EL_EXTRACTOR_NORMAL_B");}
  public bool CoreHardAttackBoostActiveR1417(Enemy recipient){return CoreHardCombatMonsterR1417(recipient)&&CoreHardLiveBuffDeviceR1417("NUI_EL_EXTRACTOR_NORMAL_A")!=null;}
  public bool CoreHardDefenceBoostActiveR1417(Enemy recipient){return CoreHardCombatMonsterR1417(recipient)&&CoreHardLiveBuffDeviceR1417("NUI_EL_EXTRACTOR_NORMAL_B")!=null;}
  // Original standard NASOD_GUARD70/50 shield policy, shared by every real
  // bound profile using these states. Core Hard buff/Elite scope stays separate.
  bool NpcSourceStandardGuardActorR1419(Enemy e){
   if(e==null||e.sourceNPC!="NUI_NASOD_GUARD"||!e.active||e.dormant||e.hp<=0||e.state==CombatState.Dead||e.passive||e.isGate||e.encounterIndex!=EncounterIndex||!enemies.Contains(e))return false;
   var profile=NpcProfile(e);var state=NpcState(e);
   return profile!=null&&profile.id==e.sourceNPC&&profile.states!=null&&state!=null&&Array.Exists(profile.states,s=>ReferenceEquals(s,state));
  }
  public float CoreHardShieldFrontDefenceR1417(Enemy e,float attackerX){
   if(!(NpcSourceStandardGuardActorR1419(e)||CoreHardCombatMonsterR1417(e)&&e.sourceNPC=="NUI_NASOD_ELITE_GUARD")||!Finite(attackerX)||e.facing==0||(attackerX-e.x)*e.facing<0)return 0;
   // A real hurt/launch/down/getup window opens the shield. No permanent
   // 'was hit' latch survives recovery to the standing source state.
   if(e.stun>0||e.state==CombatState.Hurt||e.state==CombatState.Airborne||e.state==CombatState.Knockdown||e.state==CombatState.GettingUp||!Grounded(e))return 0;
   switch(e.npcState){
    case "NASOD_GUARD_WAIT":case "NASOD_GUARD_WALK":case "NASOD_GUARD_DAMAGE_GUARD":case "NASOD_ELITE_GUARD_WAIT":case "NASOD_ELITE_GUARD_WALK":case "NASOD_ELITE_GUARD_DAMAGE_GUARD":return CoreHardGuardSourceDefenceR1417;
    case "NASOD_GUARD_ATTACK_B":case "NASOD_ELITE_GUARD_ATTACK_B":return CoreHardGuardAttackBSourceDefenceR1417;
    default:return 0;
   }
  }
  public bool CoreHardShieldGuardingR1417(Enemy e,float attackerX){return CoreHardShieldFrontDefenceR1417(e,attackerX)>0;}
  void CoreHardSupportEventR1417(string kind,Enemy actor,Enemy target,float before,float after,float fromX=0,bool front=false,bool attack=false,bool defence=false,bool targetOverride=false){
   if(coreHardSupportEventsR1417.Count>=4096)coreHardSupportEventsR1417.RemoveAt(0);
   coreHardSupportEventsR1417.Add(new CoreHardSupportEventR1417{sequence=++coreHardSupportSequenceR1417,room=room,encounter=EncounterIndex,kind=kind,npc=actor==null?null:actor.sourceNPC,state=actor==null?null:actor.npcState,target=target==null?null:target.sourceNPC,at=elapsed,before=before,after=after,fromX=fromX,frontGuard=front,attackDevice=attack,defenceDevice=defence,productTargetOverride=targetOverride});
  }
  int CoreHardIncomingEnemyDamageR1417(Enemy target,int damage,ref HitReaction reaction,float attackerX){
   if(damage<=0)return damage;
   bool coreActor=CoreHardSupportActorR1417(target);float guard=CoreHardShieldFrontDefenceR1417(target,attackerX);
   if(!coreActor&&guard<=0)return damage;
   bool defence=CoreHardDefenceBoostActiveR1417(target);
   if(guard>0)reaction=HitReaction.None;
   float factor=(defence?CoreHardDefenceDeviceDamageTakenR1417:1)*(1-guard*.01f);
   int result=Math.Max(1,(int)Math.Round(damage*factor));
   if(defence||guard>0)CoreHardSupportEventR1417(!coreActor&&guard>0?"original-standard-guard-incoming-damage-policy":"local-hard-incoming-damage-policy",target,null,damage,result,attackerX,guard>0,false,defence);
   return result;
  }
  public int CoreHardOutgoingAttackDamageR1417(Enemy owner,int damage){
   if(damage<=0||!CoreHardAttackBoostActiveR1417(owner))return damage;
   int result=Math.Max(1,(int)Math.Round(damage*CoreHardAttackDeviceMultiplierR1417));
   CoreHardSupportEventR1417("local-hard-outgoing-attack-intent",owner,null,damage,result,0,false,true);
   return result;
  }
  float CoreHardOutgoingAttackPowerR1417(Enemy owner,float power){return power>0&&CoreHardAttackBoostActiveR1417(owner)?power*CoreHardAttackDeviceMultiplierR1417:power;}
  bool CoreHardLocalAddOwnerEligibleR1417(Enemy owner){
   if(owner==null)return false;
   if(owner.sourceNPC=="NUI_NASOD_HEALER_BLADER")return CoreHardCombatMonsterR1417(owner);
   return IsCoreHardLocalBirthV21(owner);
  }
  bool CoreHardHealerTargetEligibleR1417(Enemy owner,Enemy target){
   if(ReferenceEquals(owner,target)||!CoreHardCombatMonsterR1417(owner)||!CoreHardCombatMonsterR1417(target)||owner.encounterIndex!=target.encounterIndex)return false;
   // The user explicitly includes nearby bosses. Original source JSON and its
   // historical King exclusions stay unchanged; this is the product override.
   return true;
  }
  void CoreHardRecordProductHealR1417(Enemy owner,Enemy target,float before,float after){CoreHardSupportEventR1417("product-real-heal-same-encounter-monster-including-boss",owner,target,before,after,0,false,false,false,true);}
 }
}
