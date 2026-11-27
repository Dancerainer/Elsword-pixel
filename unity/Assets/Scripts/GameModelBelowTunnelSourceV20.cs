// Limited Below Tunnel source adapter draft; never installed by the harness.
using System;
using System.Collections.Generic;
using System.Reflection;
namespace ElswordPixel {
 [Serializable] public sealed class BelowTunnelProofV20 {public string callback,npc,instructionsSha256,semanticConstantsAndInstructionsSha256;}
 [Serializable] public sealed class BelowTunnelParasiteV20 {
  public string npc,initialState,waitState,walkState,wakeState,healState,deathState,targetTiePolicy,targetPolicy,rangeAdapter,teamAdapter,targetClockAdapter,uidAdapter,rebirthAdapter,deadTargetRebirth,targetMovementAdapter;
  public float targetInterval,activeUserNativeRange,corpseInitialNativeSearchRange,healNativeSearchRange,wakeNativeRange,healConditionNativeRange,healEntryNativeRange,healTargetHpFraction,healMaxHpFraction,healParticleNativeY;
  public int targetSlot,corpseReadyFlagSlot;public bool healInnerRangeExclusive,corpseReadyFlagValue;public BelowTunnelProofV20[] manualTargetCallbacks;
 }
 [Serializable] public sealed class BelowTunnelCorpseV20 {public string npc,initialState,waitCorpseState,rebirthState,awakeWaitState;public bool sourceShowsOnRebirthEnd,sourceEmptyEventHold,rebirthEndFlagValue;public int rebirthEndFlagSlot;public BelowTunnelProofV20 initialCallback,rebirthEndCallback;}
 [Serializable] public sealed class BelowTunnelHouseV20 {
  public string npc,spawnNpc,initialState,waitState,summonState,deathState,unverifiedTrailing19,candidateHpBoundary,positionAdapter,cooldownAdapter,sourceEmitSound,sourceEmitSoundName;
  public float sourceQueueDelayScalar;public bool nativeEnqueueHostOnly;public float emitAnimationAt,sourceCooldown,sourceUserNativeRange,sourceRandomPositionNativeRange,sourceOwnerHpCost,candidateSummonHp;
  public int sourceMaxLivePopulationInclusive;public bool sourceHostOnlyHpCost,sourceCreationBeforeHpCost;public bool[] sourceRandomDirectionArguments;public BelowTunnelProofV20[] callbacks;
 }
 [Serializable] public sealed class NpcBelowTunnelSourceDocumentV20 {public string schema,sourceRulesSha256,sourceEffectsSha256;public bool runtimeAdmitted,nativeExecuted,sourceMechanismsComplete;public float unit;public BelowTunnelParasiteV20 parasite;public BelowTunnelCorpseV20[] corpses;public BelowTunnelHouseV20 house;}
 [Serializable] public sealed class BelowTunnelSourceEventV20 {public string type,npc,target,source;public int room,ownerSerial,targetSerial;public float at,amount,x,y,depth;}
 public sealed partial class GameModel {
  NpcBelowTunnelSourceDocumentV20 belowTunnelV20;
  sealed class BelowTunnelActorV20 {internal bool initialized,flag1,flag2,houseEmitted,show=true;internal string state;internal Enemy target;internal float stateAge,nextTargetAt,nextHouseAt;internal int team=1;}
  readonly Dictionary<Enemy,BelowTunnelActorV20> belowTunnelActorsV20=new Dictionary<Enemy,BelowTunnelActorV20>();
  public readonly List<BelowTunnelSourceEventV20> belowTunnelSourceEventsV20=new List<BelowTunnelSourceEventV20>();
  // Explicit replay primitive result. No native server primitive is claimed.
  public Func<Enemy,bool> BelowTunnelRebirthResultOverrideV20;
  static bool IsBelowTunnelZeroSenseV20(NpcDefinition p){return p!=null&&p.id=="NUI_PARASITE_HOUSE"&&string.IsNullOrEmpty(p.environmentKind)&&p.senseRange==0&&p.lostRange==0&&p.approachDistance==0&&p.bodyWidth==56&&p.bodyHeight==42&&p.states.Length==5&&p.waitState=="PARASITE_HOUSE_WAIT"&&p.walkState==p.waitState&&Array.Exists(p.states,s=>s.id=="PARASITE_HOUSE_RESPAWN"&&s.sourceAnimation=="Summon"&&Math.Abs(s.duration-1.5666667f)<.00001f);}
  // Below-Tunnel's original zero-sense declarations differ from the admitted
  // Airship steam profiles. Keep these exact declarations; no positive radius
  // or hurt body is fabricated merely to pass the shared profile validator.
  static bool IsBelowTunnelZeroBodySteamV20(NpcDefinition p){return p!=null&&(p.id=="NUI_STEAM_TRAP_DOWN"||p.id=="NUI_STEAM_TRAP_SIDE")&&p.environmentKind=="source-flag-steam"&&p.bodyWidth==14&&p.bodyHeight==0;}
  static bool ValidateBelowTunnelHeldSteamV20(NpcDefinition p){
   if(!IsBelowTunnelZeroBodySteamV20(p))return false;
   string stem=p.id.Substring(4)+"_";
   if(p.senseRange!=0||p.lostRange!=0||p.approachDistance!=42||p.bodySpheres.Length!=0||p.reactions.Length!=0||p.states.Length!=2||p.waitState!=stem+"WAIT"||p.walkState!=p.waitState)throw new ArgumentException("Changed Below Tunnel source zero-body steam declaration");
   foreach(var s in p.states){bool attack=s.id==stem+"ATTACK";if(!attack&&s.id!=stem+"WAIT"||!s.loop||s.sourceAnimation!=(attack?"Attack":"Wait")||s.sourceAnimSpeed!=1||Math.Abs(s.duration-(attack?199f/30:1f/15))>.00001f||s.samples.Length!=(attack?399:5)||s.hits.Length!=(attack?1:0))throw new ArgumentException("Changed Below Tunnel held source steam clocks");foreach(var f in s.samples)if(f.bodySpheres.Length!=0||f.spheres.Length!=0||f.attackLines.Length!=6)throw new ArgumentException("Below Tunnel steam has only its original line");}
   return true;
  }
  static bool BelowProofMatchesV20(BelowTunnelProofV20 p){if(p==null)return false;switch(p.callback){
   case "PARASITE_MANUAL_TARGET":return p.npc=="NUI_PARASITE"&&p.instructionsSha256=="b87558d0b2f51a1e7b0c3ad40d66ff9421c1ca99e1c9e757b7b3231b8d98872c"&&p.semanticConstantsAndInstructionsSha256=="27cc10c317b06e86b6241e41d317d08e598151f44873e93014f9405ae5d3eb72";
   case "CF_PARASITE_WAKE_UP":return p.npc=="NUI_PARASITE"&&p.instructionsSha256=="a167591fc1b3da254561eb5ce72beb6bdc268e91d05e8e62b3f15346a8cacc63"&&p.semanticConstantsAndInstructionsSha256=="72aa969425070b708c25a595007aa74b6c7a89d30f27e9cd6c387ec557ce7c7d";
   case "CF_PARASITE_HEAL":return p.npc=="NUI_PARASITE"&&p.instructionsSha256=="a3534bc423b5ef8ab103fe00e02725e53c576111ee7d37faf88db4546850a686"&&p.semanticConstantsAndInstructionsSha256=="b1d485b052c60e1d1d366cc1d4820ed238dc447c907e68b346fc0d9b0e6b1618";
   case "PARASITE_WAKE_UP_STATE_START":return p.npc=="NUI_PARASITE"&&p.instructionsSha256=="cfa9000e4453d6ef09958d97c06d225bce149263a4616d11dbe0f0394eb4d104"&&p.semanticConstantsAndInstructionsSha256=="4c55f87191b19dd01705e22176d2755313bd9ceee8065ea4fb27414297d03f69";
   case "PARASITE_HEAL_STATE_START":return p.npc=="NUI_PARASITE"&&p.instructionsSha256=="64a83f4da09c9b20443ad0f16b5f78b381d508eed2a6584307cc772868ca69b4"&&p.semanticConstantsAndInstructionsSha256=="7489773cef1256e8cc399e58cb3fea7edb83c58c0a435ee657a6032a1fe16a51";
   case "CF_PARASITE_HOUSE_RESPAWN":return p.npc=="NUI_PARASITE_HOUSE"&&p.instructionsSha256=="5166cb4348b5289873c2c91b92d2925ae977f967abb4764a9ea3d3941806af80"&&p.semanticConstantsAndInstructionsSha256=="35057193f819c3e3e7d2ca608485ac969a7b31431e2da1eff481639debe4aca7";
   case "PARASITE_HOUSE_START_STATE_START":return p.npc=="NUI_PARASITE_HOUSE"&&p.instructionsSha256=="1df52e08515ea9f2d1db8d204b650b307e3eb509baacfafa3499b24d52d0f384"&&p.semanticConstantsAndInstructionsSha256=="be428d39038fc6f1a760403b8c67234db1642d3d88fba1955f788c1d5ed0d8fe";
   case "PARASITE_HOUSE_RESPAWN_FRAME_MOVE":return p.npc=="NUI_PARASITE_HOUSE"&&p.instructionsSha256=="5935c2320ac4b00a8ec88b63fb36cd5fd71e37d9f1815086aaa81dd928db2e3f"&&p.semanticConstantsAndInstructionsSha256=="102cdf0d22f80f91f40a8ff64c823c381da300169e448b7d03e1c1662980c628";
   case "NASOD_MINER_THIN_DEFECT_DIE_START_STATE_START":return p.npc=="NUI_NASOD_MINER_THIN_DEFECT_DIE"&&p.instructionsSha256=="8a889d90cdeff5753b675e8617ed4a879928b0ab3f7e33fe33a297e363453b8e"&&p.semanticConstantsAndInstructionsSha256=="10950fef039f777ebb0cc1f658007fb612ca2d03f21b48232756170ebcccb76e";
   case "NASOD_MINER_THIN_DEFECT_DIE_REBIRTH_STATE_END":return p.npc=="NUI_NASOD_MINER_THIN_DEFECT_DIE"&&p.instructionsSha256=="1631c3c705edeba5203f430134cf9c680c4219cc33c87f7170ee9de03cfa4c23"&&p.semanticConstantsAndInstructionsSha256=="0a6bb402da8a3293ad1fa1f170456f2455da2e1582b2197032271bf768cacbcb";
   case "NASOD_MINER_FAT_C_DEFECT_DIE_START_STATE_START":return p.npc=="NUI_NASOD_MINER_FAT_C_DEFECT_DIE"&&p.instructionsSha256=="8a889d90cdeff5753b675e8617ed4a879928b0ab3f7e33fe33a297e363453b8e"&&p.semanticConstantsAndInstructionsSha256=="7dc4f605ccd808402a91fb9c81ad3e0c1a096bb5f3179cbb7b1c6f352a54d643";
   case "NASOD_MINER_FAT_C_DEFECT_DIE_REBIRTH_STATE_END":return p.npc=="NUI_NASOD_MINER_FAT_C_DEFECT_DIE"&&p.instructionsSha256=="bf46cec48e797db349f9b0edd7634ded63f9c77af2bacf9b1bccc172259c7c34"&&p.semanticConstantsAndInstructionsSha256=="d59acea862895f9d2e56c1f103946b937d973283abd4a65125fbdbee0f6cdc32";
   default:return false;}}
  static bool BelowDefaultV20(object value){if(value==null)return true;if(value is string)return string.IsNullOrEmpty((string)value);if(value is bool)return !(bool)value;if(value is float)return (float)value==0;if(value is int)return (int)value==0;var a=value as Array;if(a!=null)return a.Length==0;foreach(var f in value.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public))if(!BelowDefaultV20(f.GetValue(value)))return false;return true;}
  public void SetNpcBelowTunnelSourceV20(NpcBelowTunnelSourceDocumentV20 doc){
   if(BelowDefaultV20(doc))doc=null;
   if(doc!=null){var p=doc.parasite;var h=doc.house;
    if(doc.schema!="altera-below-tunnel-source-callback-draft-v20"||doc.runtimeAdmitted||doc.nativeExecuted||doc.sourceMechanismsComplete||doc.unit!=.28f||doc.sourceRulesSha256!="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb"||doc.sourceEffectsSha256!="89e0d3e4a89d5850fd546948e2b2cf08ee2350d7503044684cbd7060c1f1fa42"||p==null||h==null||doc.corpses==null||doc.corpses.Length!=2)throw new ArgumentException("Invalid Below Tunnel exact source binding");
    if(p.npc!="NUI_PARASITE"||p.initialState!="PARASITE_START"||p.waitState!="PARASITE_WAIT"||p.walkState!="PARASITE_WALK"||p.wakeState!="PARASITE_WAKE_UP"||p.healState!="PARASITE_HEAL"||p.deathState!="PARASITE_DYING"||p.targetInterval!=3||p.activeUserNativeRange!=1000||p.corpseInitialNativeSearchRange!=9999||p.healNativeSearchRange!=5000||p.wakeNativeRange!=50||p.healConditionNativeRange!=100||p.healEntryNativeRange!=300||p.healTargetHpFraction!=.8f||p.healMaxHpFraction!=.6f||p.healParticleNativeY!=100||p.targetSlot!=0||p.corpseReadyFlagSlot!=1||p.corpseReadyFlagValue||!p.healInnerRangeExclusive)throw new ArgumentException("Changed original Parasite predicates");
    if(p.rangeAdapter!="projected-route-arc-XY-plus-nativeDepth-times-unit28-not-native3D-distance-parity"||p.uidAdapter!="stable-current-room-Enemy-reference-for-native-GetNPCUnit-index-and-GetUID"||p.targetTiePolicy!="last-listed-on-equal-distance-from-ISGT-only-skip"||p.rebirthAdapter!="exact-same-source-ID-live-positive-HP-actor-into-own-INIT_STATE.REBIRTH_STATE-preserveHP-coordinates-no-server-resurrection-inference")throw new ArgumentException("Unrecorded Parasite2D primitive adapter");
    string[] ids={"NUI_NASOD_MINER_THIN_DEFECT_DIE","NUI_NASOD_MINER_FAT_C_DEFECT_DIE"};for(int i=0;i<2;i++){var c=doc.corpses[i];string stem=ids[i].Substring(4);if(c==null||c.npc!=ids[i]||c.initialState!=stem+"_START"||c.waitCorpseState!=stem+"_DIEWAIT"||c.rebirthState!=stem+"_REBIRTH"||c.awakeWaitState!=stem+"_WAIT"||!c.sourceEmptyEventHold||c.rebirthEndFlagSlot!=1||!c.rebirthEndFlagValue||c.sourceShowsOnRebirthEnd!=(i==0))throw new ArgumentException("Changed exact DieMiner rebirth branch");foreach(var s in new[]{c.initialState,c.waitCorpseState,c.rebirthState,c.awakeWaitState})if(!npcStates.ContainsKey(c.npc+"/"+s))throw new ArgumentException("Missing positive corpse source clock");}
    if(h.npc!="NUI_PARASITE_HOUSE"||h.spawnNpc!=p.npc||h.initialState!="PARASITE_HOUSE_START"||h.waitState!="PARASITE_HOUSE_WAIT"||h.summonState!="PARASITE_HOUSE_RESPAWN"||h.deathState!="PARASITE_HOUSE_DYING"||h.emitAnimationAt!=1||h.sourceCooldown!=10||h.sourceMaxLivePopulationInclusive!=2||h.sourceUserNativeRange!=3000||h.sourceRandomPositionNativeRange!=500||h.sourceOwnerHpCost!=100||!h.sourceHostOnlyHpCost||!h.sourceCreationBeforeHpCost||h.candidateSummonHp!=2000||h.sourceRandomDirectionArguments==null||h.sourceRandomDirectionArguments.Length!=2||!h.sourceRandomDirectionArguments[0]||!h.sourceRandomDirectionArguments[1])throw new ArgumentException("Changed original ParasiteHouse create/cost order");
    foreach(string s in new[]{p.initialState,p.waitState,p.walkState,p.wakeState,p.healState,p.deathState})if(!npcStates.ContainsKey(p.npc+"/"+s))throw new ArgumentException("Missing positive Parasite source clock");foreach(string s in new[]{h.initialState,h.waitState,h.summonState,h.deathState})if(!npcStates.ContainsKey(h.npc+"/"+s))throw new ArgumentException("Missing positive house source clock");
    if(h.sourceQueueDelayScalar!=1.9f||!h.nativeEnqueueHostOnly)throw new ArgumentException("Changed verified native queue delay/host gate");
    if(h.sourceEmitSound!="npc-parasite_house_summon"||h.sourceEmitSoundName!="Parasite_House_Summon.ogg"||!npcSourceSoundAssetsV17.Contains(h.sourceEmitSound))throw new ArgumentException("Changed exact original House summon sound");
    if(p.manualTargetCallbacks==null||p.manualTargetCallbacks.Length!=5||h.callbacks==null||h.callbacks.Length!=3)throw new ArgumentException("Missing source callback receipts");foreach(var c in p.manualTargetCallbacks)if(!BelowProofMatchesV20(c))throw new ArgumentException("Invalid original Parasite callback receipt");foreach(var c in h.callbacks)if(!BelowProofMatchesV20(c))throw new ArgumentException("Invalid original house callback receipt");foreach(var c in doc.corpses)if(!BelowProofMatchesV20(c.initialCallback)||!BelowProofMatchesV20(c.rebirthEndCallback))throw new ArgumentException("Invalid exact DieMiner callback receipt");
   }belowTunnelV20=doc;ClearBelowTunnelRoomV20();
  }
  void ClearBelowTunnelRoomV20(){ClearBelowCandidateRoomV21();belowTunnelActorsV20.Clear();belowTunnelSourceEventsV20.Clear();}
  BelowTunnelActorV20 BelowActorV20(Enemy owner){BelowTunnelActorV20 a;if(!belowTunnelActorsV20.TryGetValue(owner,out a)){a=new BelowTunnelActorV20();belowTunnelActorsV20.Add(owner,a);}return a;}
  BelowTunnelCorpseV20 BelowCorpseV20(Enemy e){return belowTunnelV20==null||e==null?null:Array.Find(belowTunnelV20.corpses,r=>r.npc==e.sourceNPC);}
  bool BelowManagedV20(Enemy e){return belowTunnelV20!=null&&e!=null&&(e.sourceNPC==belowTunnelV20.parasite.npc||e.sourceNPC==belowTunnelV20.house.npc||BelowCorpseV20(e)!=null);}
  public bool BelowTunnelFlagV20(Enemy e,int slot){if(e==null||slot<1||slot>2)throw new ArgumentException("Below source flag slot");var a=BelowActorV20(e);return slot==1?a.flag1:a.flag2;}
  public Enemy BelowTunnelTargetV20(Enemy e){return BelowActorV20(e).target;}
  public void SetBelowTunnelReplayTeamV20(Enemy e,int team){BelowActorV20(e).team=team;}
  void BelowEventV20(string type,Enemy owner,Enemy target,float at,float amount,string source){if(belowTunnelSourceEventsV20.Count>=4096)belowTunnelSourceEventsV20.RemoveAt(0);var p=target??owner;belowTunnelSourceEventsV20.Add(new BelowTunnelSourceEventV20{type=type,npc=owner.sourceNPC,target=target==null?"":target.sourceNPC,source=source,room=room,ownerSerial=NpcSoundActorId(owner),targetSerial=NpcSoundActorId(target),at=at,amount=amount,x=p.x,y=p.y,depth=p.nativeDepth});}
  float BelowDistanceV20(Enemy a,Enemy b,float sourceYOffset=0){float dx=a.x-b.x,dy=a.y+sourceYOffset*.28f-b.y,dz=(a.nativeDepth-b.nativeDepth)*.28f;return (float)Math.Sqrt(dx*dx+dy*dy+dz*dz);}
  bool BelowUserNearV20(Enemy owner,float nativeRange){float dx=owner.x-player.x,dy=owner.y-player.y,dz=owner.nativeDepth*.28f,r=nativeRange*.28f;return player.hp>0&&dx*dx+dy*dy+dz*dz<=r*r;}
  int BelowLivePopulationV20(){int n=0;foreach(var e in enemies)if(e.sourceNPC==belowTunnelV20.house.spawnNpc&&e.hp>0)n++;return n;}
  // Later encounter births already occupy this room's array, but are absent
  // from the original live actor query until their encounter opens.
  bool BelowTargetPresentV20(Enemy target){return target!=null&&target.active&&!target.dormant&&enemies.Contains(target);}
  void BelowManualTargetV20(Enemy owner,BelowTunnelActorV20 a){
   var p=belowTunnelV20.parasite;if(!BelowUserNearV20(owner,p.activeUserNativeRange))return;Enemy target=null;float nearest=p.corpseInitialNativeSearchRange*.28f;
   foreach(var e in enemies)if(e.active&&!e.dormant&&BelowCorpseV20(e)!=null&&!BelowActorV20(e).flag1){float d=BelowDistanceV20(owner,e);if(d>nearest)continue;target=e;nearest=d;}
   if(target==null){nearest=p.healNativeSearchRange*.28f;foreach(var e in enemies){if(!e.active||e.dormant||e.sourceNPC==p.npc||e.sourceNPC==belowTunnelV20.house.npc||e.hp>=e.maxHp*p.healTargetHpFraction)continue;float d=BelowDistanceV20(owner,e);if(d>nearest)continue;target=e;nearest=d;}}
   if(target!=null&&(!BelowActorV20(target).flag1||target.hp<target.maxHp*p.healTargetHpFraction)){a.target=target;BelowEventV20("source-manual-target",owner,target,elapsed,nearest,"PARASITE_MANUAL_TARGET/SetInt0");}
  }
  bool BelowWakePredicateV20(Enemy owner,Enemy target){return BelowTargetPresentV20(target)&&BelowCorpseV20(target)!=null&&!BelowActorV20(target).flag1&&BelowDistanceV20(owner,target)<=belowTunnelV20.parasite.wakeNativeRange*.28f+1e-6f;}
  bool BelowHealPredicateV20(Enemy owner,Enemy target){var p=belowTunnelV20.parasite;return BelowTargetPresentV20(target)&&target.sourceNPC!=p.npc&&target.sourceNPC!=belowTunnelV20.house.npc&&BelowDistanceV20(owner,target)<=p.healConditionNativeRange*.28f+1e-6f&&target.hp<target.maxHp*p.healTargetHpFraction;}
  void BelowConsumeV20(Enemy owner,string reason){owner.hp=0;owner.state=CombatState.Dead;owner.stun=0;InterruptNpcAction(owner);BelowEventV20("source-owner-SetNowHP0",owner,null,elapsed,0,reason);}
  void OnBelowTunnelStateEnteringV20(Enemy owner,NpcStateDefinition state){
   if(!BelowManagedV20(owner))return;var a=BelowActorV20(owner);a.initialized=true;a.state=state.id;a.stateAge=0;a.houseEmitted=false;var p=belowTunnelV20.parasite;var h=belowTunnelV20.house;var c=BelowCorpseV20(owner);
   if(c!=null&&state.id==c.initialState){a.flag1=a.flag2=false;BelowEventV20("source-flag-init",owner,null,elapsed,0,c.initialCallback.callback);}
   if(owner.sourceNPC==p.npc&&state.id==p.initialState){a.target=null;a.nextTargetAt=elapsed;}
   if(owner.sourceNPC==h.npc&&state.id==h.summonState)a.nextHouseAt=elapsed+h.sourceCooldown;
   if(owner.sourceNPC!=p.npc)return;var target=a.target;if(target!=null&&!BelowTargetPresentV20(target))target=a.target=null;
   if(state.id==p.wakeState&&BelowWakePredicateV20(owner,target)){
    if(!(target.hp>0)){BelowEventV20("source-native-rebirth-zero-HP-false",owner,target,elapsed,0,"ChangeRebirthStateNPCUnit_LUA");return;}
    bool ok=BelowTunnelRebirthResultOverrideV20==null||BelowTunnelRebirthResultOverrideV20(target);if(!ok){BelowEventV20("source-rebirth-result-false",owner,target,elapsed,0,"ChangeRebirthStateNPCUnit_LUA");return;}
    var rule=BelowCorpseV20(target);bool accepted=TryRequestNpcState(target,rule.rebirthState,false)&&CommitNpcStateRequest(target);BelowEventV20(accepted?"source-rebirth-same-ID":"source-rebirth-request-rejected-but-primitive-true",owner,target,elapsed,target.hp,"INIT_STATE.REBIRTH_STATE/native-positiveHP-eligibility/independent-StateChange-result");BelowConsumeV20(owner,"PARASITE_WAKE_UP_STATE_START");
   }else if(state.id==p.healState&&target!=null){
    float outer=BelowDistanceV20(owner,target),inner=BelowDistanceV20(owner,target,p.healParticleNativeY);if(outer>p.healEntryNativeRange*.28f+1e-6f||target.hp>=target.maxHp*p.healTargetHpFraction||ReferenceEquals(owner,target)||BelowActorV20(owner).team!=BelowActorV20(target).team||target.hp<=0||inner>=p.healEntryNativeRange*.28f||target.hp>=target.maxHp)return;
    float amount=Math.Min(target.maxHp-target.hp,target.maxHp*p.healMaxHpFraction);RecoverEnemyHealth(target,amount,p.healMaxHpFraction*100);BelowEventV20("source-heal60pct-max",owner,target,elapsed,amount,"PARASITE_HEAL_STATE_START");BelowConsumeV20(owner,"PARASITE_HEAL_STATE_START");
   }
  }
  void OnBelowTunnelStateExitV20(Enemy owner,NpcStateDefinition state){if(!BelowManagedV20(owner)||state==null)return;var c=BelowCorpseV20(owner);if(c!=null&&state.id==c.rebirthState){var a=BelowActorV20(owner);a.flag1=true;if(c.sourceShowsOnRebirthEnd)a.show=true;BelowEventV20("source-rebirth-end-flag1",owner,null,elapsed,1,c.rebirthEndCallback.callback);}}
  bool BelowEnterV20(Enemy owner,string state){return TryEnterNpcState(owner,npcStates[owner.sourceNPC+"/"+state]);}
  void EmitBelowHouseV20(Enemy owner,float at){var h=belowTunnelV20.house;QueueNpcSound(new NpcTimedSound{id=h.npc+"/PARASITE_HOUSE_RESPAWN_FRAME_MOVE/1.0",sound=h.sourceEmitSound,sourceName=h.sourceEmitSoundName,sourceField="PARASITE_HOUSE_RESPAWN_FRAME_MOVE.AnimEventTimer_LUA",sourceTime=1,candidateTime=1,candidateClock="npc-state-pose-seconds",nativeClockOwnershipProven=false},owner,++npcSoundEntrySerial,0,at,owner.npcState);float x,y;int line;if(!SummonGroundPoint(owner,new NpcSummonSpawn{npc=h.spawnNpc,range=h.sourceRandomPositionNativeRange*.28f,direction=1},out x,out y,out line)){BelowEventV20("HOLD-source-random-position-no-support",owner,null,at,0,"GetRandomPositionDir");return;}
   if(NpcMechanismIsHost){npcSummonRequests.Add(new NpcSummonRequest{owner=owner,npc=h.spawnNpc,room=room,encounter=owner.encounterIndex,line=line,x=x,y=y,facing=1,due=at+h.sourceQueueDelayScalar,alreadyEmitted=true,belowManagerOwnedV20=true,template=new EnemySpawn{hp=h.candidateSummonHp,visualScale=1,nativeDepth=owner.nativeDepth}});BelowEventV20("source-house-create-submitted",owner,null,at,1,"PushCreateNPCReq_Lua/native-delay/Flush-noop");}else BelowEventV20("source-native-PushCreate-host-rejected",owner,null,at,0,"Native same IsHost UID predicate; Lua callback sound/visual still executed");
   if(NpcMechanismIsHost){float before=owner.hp;owner.hp=Math.Max(0,owner.hp-h.sourceOwnerHpCost);BelowEventV20("source-house-host-cost100",owner,null,at,before-owner.hp,"SetNowHP(nowHP-100)");if(owner.hp<=0)BelowConsumeV20(owner,"PARASITE_HOUSE_RESPAWN_FRAME_MOVE");}
  }
  bool TickBelowTunnelHeldSteamV20(Enemy owner,NpcDefinition profile,float dt){
   if(TickBelowCandidateSteamV21(owner,profile,dt))return true;if(belowTunnelV20==null||!ValidateBelowTunnelHeldSteamV20(profile))return false;
   if(!owner.active||owner.dormant||owner.hp<=0)return true;
   var a=BelowActorV20(owner);if(!a.initialized){a.initialized=true;EnterNpcState(owner,npcStates[profile.id+"/"+profile.waitState]);BelowEventV20("HOLD-source-steam-controller-flags",owner,null,elapsed,0,"Original FLAG_FALSE0/FLAG_TRUE0 and controller/particle-service callback not projected by this draft");}
   AdvancePassagePose(owner,NpcState(owner),dt,elapsed-dt);return true;
  }
  bool TickBelowTunnelManagedV20(Enemy owner,float dt){
   if(!BelowManagedV20(owner)||!owner.active||owner.dormant||owner.hp<=0)return false;var profile=NpcProfile(owner);var a=BelowActorV20(owner);var p=belowTunnelV20.parasite;var h=belowTunnelV20.house;var c=BelowCorpseV20(owner);
   if(!a.initialized){string initial=c!=null?c.initialState:owner.sourceNPC==p.npc?p.initialState:h.initialState;if(!BelowEnterV20(owner,initial))return true;}
   if(owner.hp<=0)return true;if(owner.state!=CombatState.Normal||owner.stun>0)return false;var state=NpcState(owner);if(state==null){if(!BelowEnterV20(owner,profile.waitState))return true;state=NpcState(owner);}
   if(c!=null){if(state.id!=c.initialState&&state.id!=c.waitCorpseState&&state.id!=c.rebirthState)return false;AdvancePassagePose(owner,state,dt,elapsed-dt);a.stateAge+=dt;owner.vx=owner.vy=0;if(state.id==c.initialState&&a.stateAge+1e-6f>=state.duration)BelowEnterV20(owner,c.waitCorpseState);else if(state.id==c.rebirthState&&a.stateAge+1e-6f>=state.duration)BelowEnterV20(owner,c.awakeWaitState);return true;}
   if(owner.sourceNPC==h.npc){float from=a.stateAge;AdvancePassagePose(owner,state,dt,elapsed-dt);a.stateAge+=dt;owner.vx=owner.vy=0;if(state.id==h.waitState){if(elapsed+1e-6f>=a.nextHouseAt&&BelowUserNearV20(owner,h.sourceUserNativeRange)&&BelowLivePopulationV20()<=h.sourceMaxLivePopulationInclusive)BelowEnterV20(owner,h.summonState);}else if(state.id==h.summonState){float at=h.emitAnimationAt/state.sourceAnimSpeed;if(!a.houseEmitted&&from<at&&at<=a.stateAge+1e-6f){a.houseEmitted=true;EmitBelowHouseV20(owner,elapsed-dt+Math.Max(0,at-from));}if(owner.hp>0&&a.stateAge+1e-6f>=state.duration)BelowEnterV20(owner,h.waitState);}else if(a.stateAge+1e-6f>=state.duration)BelowEnterV20(owner,h.waitState);return true;}
   IntegrateBody(owner,dt,false);if(owner.hp<=0)return true;state=NpcState(owner);
   if(state.id==p.waitState||state.id==p.walkState){if(a.target!=null&&!BelowTargetPresentV20(a.target)){a.target=null;a.nextTargetAt=elapsed;}if(elapsed+1e-6f>=a.nextTargetAt){a.nextTargetAt=elapsed+p.targetInterval;BelowManualTargetV20(owner,a);}if(BelowWakePredicateV20(owner,a.target)){BelowEnterV20(owner,p.wakeState);return true;}if(BelowHealPredicateV20(owner,a.target)){BelowEnterV20(owner,p.healState);return true;}if(a.target!=null){float dx=a.target.x-owner.x;owner.facing=dx>=0?1:-1;string desired=Math.Abs(dx)>p.wakeNativeRange*.28f?p.walkState:p.waitState;if(state.id!=desired){BelowEnterV20(owner,desired);state=NpcState(owner);}if(state.id==p.walkState)MoveAlongGround(owner,owner.x+owner.facing*state.walkSpeed*dt);}AdvancePassagePose(owner,state,dt,elapsed-dt);a.stateAge+=dt;return true;}
   AdvancePassagePose(owner,state,dt,elapsed-dt);a.stateAge+=dt;if(a.stateAge+1e-6f>=state.duration)BelowEnterV20(owner,state.nextState);return true;
  }
 }
}
