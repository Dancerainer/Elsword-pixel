using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
namespace ElswordPixel {
 [Serializable] public sealed class CoreHardEntryActivationBirthV21 {public int room,order;public string npc,initialState;public bool sourceActive,sourceStopAtStart,managed;}
 [Serializable] public sealed class CoreHardEntryActivationDocumentV21 {public string schema,sourceRulesSha256,sourceModelSha256,policy,receipt;public int difficultyValue,npcLevel;public bool enabled,runtimeAdmitted,nativeEquivalent;public CoreHardEntryActivationBirthV21[] births;}
 [Serializable] public sealed class CoreHardEntryActivationEventV21 {public string npc,state,policy;public int room,order;public bool aiBefore,aiAfter,sourceActive,sourceStopAtStart,managed;}
 public sealed partial class GameModel {
  const string CoreHardEntryActivationReceiptExpectedV21="e660b4b1c44a202b322cd79dc780e37dfabdf42fc82bd6c3451bec058c06535c";
  CoreHardEntryActivationDocumentV21 coreHardEntryActivationV21;
  readonly HashSet<Enemy> coreHardEntryInitializedV21=new HashSet<Enemy>();
  public readonly List<CoreHardEntryActivationEventV21> coreHardEntryActivationEventsV21=new List<CoreHardEntryActivationEventV21>();
  public bool CoreHardEntryActivationConfiguredV21 {get{return coreHardEntryActivationV21!=null;}}
  public static string CoreHardEntryActivationReceiptV21(CoreHardEntryActivationDocumentV21 d){var s=new List<string>{d.schema,d.sourceRulesSha256,d.sourceModelSha256,d.policy,d.difficultyValue.ToString(CultureInfo.InvariantCulture),d.npcLevel.ToString(CultureInfo.InvariantCulture)};foreach(var b in d.births)s.AddRange(new[]{b.room.ToString(CultureInfo.InvariantCulture),b.order.ToString(CultureInfo.InvariantCulture),b.npc,b.initialState,b.sourceActive?"1":"0",b.sourceStopAtStart?"1":"0",b.managed?"1":"0"});using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",s.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();}
  public void SetCoreHardEntryActivationV21(CoreHardEntryActivationDocumentV21 d){
   if(d!=null){if(!CoreHardLocalBalanceConfiguredV21||!CoreHardClosureConfiguredV21||d.schema!="core-hard-internal85-entry-activation-draft-v21"||d.sourceRulesSha256!=coreHardV21.sourceRulesSha256||d.sourceModelSha256!=coreHardV21.sourceModelSha256||d.difficultyValue!=1||d.npcLevel!=37||!d.enabled||d.runtimeAdmitted||d.nativeEquivalent||d.births==null||d.births.Length!=85||d.receipt!=CoreHardEntryActivationReceiptExpectedV21||CoreHardEntryActivationReceiptV21(d)!=d.receipt)throw new ArgumentException("Core Hard exact entry activation boundary changed");
    foreach(var b in d.births){NpcStateDefinition state;if(!npcStates.TryGetValue(b.npc+"/"+b.initialState,out state))throw new ArgumentException("Core source initial state absent");}
   }ClearCoreStopIntroOwnerV21();ClearCoreHardStopGuardV21();coreHardEntryActivationV21=d;coreHardEntryInitializedV21.Clear();coreHardEntryActivationEventsV21.Clear();
  }
  void ClearCoreHardEntryActivationV21(){ClearCoreStopIntroOwnerV21();ClearCoreHardStopGuardV21();coreHardEntryActivationV21=null;coreHardEntryInitializedV21.Clear();coreHardEntryActivationEventsV21.Clear();coreHardHitPacketsR642.Clear();coreHardHitPacketSerialR642=0;}
  void BeginCoreHardEntryActivationRoomV21(){BeginCoreStopIntroRoomV21();ClearCoreHardStopGuardV21();coreHardEntryInitializedV21.Clear();coreHardHitPacketsR642.Clear();}
  sealed class CoreHardHitDisplayV39 {internal CoreHardRuntimeV21 runtime;internal int epoch,dealt;internal string prior,next;internal float at,sampledAt,hp,maxHp,x,y,depth;internal CombatState reaction;}
  readonly Dictionary<Enemy,CoreHardHitDisplayV39> coreHardDisplayHitsV39=new Dictionary<Enemy,CoreHardHitDisplayV39>();
  // The actual positive ApplyHitToEnemy transaction may clear npcState after
  // this actor's ordinary Tick. This metadata records that existing outcome;
  // only the next ordinary Core managed Tick chooses DAMAGE/DYING as before.
  void RecordCoreHardHitDisplayPendingV39(Enemy e,int dealt,string prior){
   if(dealt<=0||string.IsNullOrEmpty(prior)||!CoreHardClosureScopeV21()||!NativeTerrain||e==null||!e.active||e.dormant||e.encounterIndex!=0||e.visualScale!=1||!string.IsNullOrEmpty(e.npcState)||e.npcTime!=0||e.clock!=0||!enemies.Contains(e)||!coreHardBalancedActorsV21.Contains(e)&&!coreHardLocalActorsV21.Contains(e))return;
   var rule=CoreHardActorRuleV21(e);CoreHardRuntimeV21 runtime;NpcDefinition profile;if(rule==null||!coreHardActorsV21.TryGetValue(e,out runtime)||!runtime.initialized||runtime.last!=prior||!npcStates.ContainsKey(e.sourceNPC+"/"+prior)||!npcDefinitions.TryGetValue(e.sourceNPC,out profile)||profile.kind!=e.kind)return;string next=e.hp<=0?rule.dyingState:rule.damageState;NpcStateDefinition expected;if(string.IsNullOrEmpty(next)||!npcStates.TryGetValue(e.sourceNPC+"/"+next,out expected)||string.IsNullOrEmpty(runtime.last))return;
   coreHardDisplayHitsV39[e]=new CoreHardHitDisplayV39{runtime=runtime,epoch=coreHardClosureEpochV21,dealt=dealt,prior=runtime.last,next=next,at=elapsed,sampledAt=elapsed,hp=e.hp,maxHp=e.maxHp,x=e.x,y=e.y,depth=e.nativeDepth,reaction=e.state};
  }
  // Packet provenance is private metadata, never a simulation clock.
  long coreHardHitPacketSerialR642;
  readonly Dictionary<Enemy,CoreHardHitPacketR642> coreHardHitPacketsR642=new Dictionary<Enemy,CoreHardHitPacketR642>();
  sealed class CoreHardHitPacketR642 {
   internal readonly Enemy actor;internal readonly CoreHardHitDisplayV39 receipt;internal readonly CoreHardRuntimeV21 runtime;
   internal readonly NpcDefinition profile;internal readonly CoreHardActorV21 rule;internal readonly long serial;
   internal readonly int epoch,dealt;internal readonly string prior,next;internal readonly float at,sampledAt,hp,maxHp,x,y,depth;internal readonly CombatState reaction;
   internal CoreHardHitPacketR642(Enemy e,CoreHardHitDisplayV39 r,NpcDefinition p,CoreHardActorV21 a,long packet){actor=e;receipt=r;runtime=r.runtime;profile=p;rule=a;serial=packet;epoch=r.epoch;dealt=r.dealt;prior=r.prior;next=r.next;at=r.at;sampledAt=r.sampledAt;hp=r.hp;maxHp=r.maxHp;x=r.x;y=r.y;depth=r.depth;reaction=r.reaction;}
  }
  // Called only after a genuine positive-hit producer, or after the original
  // fully hitPause-consumed actor Sample. It records; it never resamples.
  void NoteCoreHardHitPacketR642(Enemy e){
   if(e==null)return;string next;CoreHardHitDisplayV39 receipt;NpcDefinition profile;var rule=CoreHardActorRuleV21(e);
   if(e.hp<=0||rule==null||!TryCoreHardHitDisplayPendingV39(e,out next)||!coreHardDisplayHitsV39.TryGetValue(e,out receipt)||next!=rule.damageState||receipt.at>receipt.sampledAt||!npcDefinitions.TryGetValue(e.sourceNPC,out profile)||profile.kind!=e.kind){coreHardHitPacketsR642.Remove(e);return;}
   coreHardHitPacketsR642[e]=new CoreHardHitPacketR642(e,receipt,profile,rule,coreHardHitPacketSerialR642);
  }
  // A second real hit may arrive before the normal owner can commit DAMAGE.
  // Capture the exact existing pending provenance before ApplyHit mutates HP.
  // This private immutable witness never initializes or advances an actor.
  sealed class CoreHardHitWitnessR642 {
   internal readonly Enemy actor;internal readonly CoreHardHitDisplayV39 receipt;internal readonly CoreHardRuntimeV21 runtime;
   internal readonly NpcDefinition profile;internal readonly CoreHardActorV21 rule;
   internal readonly int epoch,dealt;internal readonly string prior,next;
   internal readonly float at,sampledAt,hp,maxHp,x,y,depth,capturedAt,hitPause;internal readonly CombatState reaction;
   internal CoreHardHitWitnessR642(Enemy e,CoreHardHitDisplayV39 r,NpcDefinition p,CoreHardActorV21 a,float now){actor=e;receipt=r;runtime=r.runtime;profile=p;rule=a;epoch=r.epoch;dealt=r.dealt;prior=r.prior;next=r.next;at=r.at;sampledAt=r.sampledAt;hp=r.hp;maxHp=r.maxHp;x=r.x;y=r.y;depth=r.depth;reaction=r.reaction;capturedAt=now;hitPause=e.hitPause;}
  }
  CoreHardHitWitnessR642 CaptureCoreHardHitDisplayWitnessR642(Enemy e,string prior){
   if(!string.IsNullOrEmpty(prior)||e==null||e.hp<=0||!CoreHardClosureScopeV21()||!NativeTerrain||!e.active||e.dormant||e.encounterIndex!=0||e.visualScale!=1||!string.IsNullOrEmpty(e.npcState)||e.npcTime!=0||e.clock!=0||!enemies.Contains(e)||!coreHardBalancedActorsV21.Contains(e)&&!coreHardLocalActorsV21.Contains(e))return null;
   CoreHardHitDisplayV39 receipt;CoreHardRuntimeV21 runtime;NpcDefinition profile;var rule=CoreHardActorRuleV21(e);
   if(rule==null||!coreHardDisplayHitsV39.TryGetValue(e,out receipt)||receipt.dealt<=0||receipt.epoch!=coreHardClosureEpochV21||!coreHardActorsV21.TryGetValue(e,out runtime)||!ReferenceEquals(runtime,receipt.runtime)||!runtime.initialized||string.IsNullOrEmpty(receipt.prior)||runtime.last!=receipt.prior||!npcStates.ContainsKey(e.sourceNPC+"/"+receipt.prior)||!npcDefinitions.TryGetValue(e.sourceNPC,out profile)||profile.kind!=e.kind||string.IsNullOrEmpty(rule.damageState)||receipt.next!=rule.damageState||!npcStates.ContainsKey(e.sourceNPC+"/"+receipt.next))return null;
   if(e.hp!=receipt.hp||e.maxHp!=receipt.maxHp||e.x!=receipt.x||e.y!=receipt.y||e.nativeDepth!=receipt.depth||e.state!=receipt.reaction||receipt.at>receipt.sampledAt||receipt.sampledAt>elapsed)return null;
    // Same-packet admission keeps the original exact current-receipt path.
   // Crossing a packet additionally needs the complete immutable snapshot
   // from a genuine producer or the original fully paused actor return.
   if(receipt.sampledAt!=elapsed){
    CoreHardHitPacketR642 packet;
    if(!coreHardHitPacketsR642.TryGetValue(e,out packet)||!ReferenceEquals(packet.actor,e)||!ReferenceEquals(packet.receipt,receipt)||!ReferenceEquals(packet.runtime,runtime)||!ReferenceEquals(packet.profile,profile)||!ReferenceEquals(packet.rule,rule)||packet.epoch!=coreHardClosureEpochV21||packet.epoch!=receipt.epoch||packet.dealt!=receipt.dealt||packet.prior!=receipt.prior||packet.next!=receipt.next||packet.at!=receipt.at||packet.sampledAt!=receipt.sampledAt||packet.hp!=receipt.hp||packet.maxHp!=receipt.maxHp||packet.x!=receipt.x||packet.y!=receipt.y||packet.depth!=receipt.depth||packet.reaction!=receipt.reaction)return null;
    if(packet.serial!=coreHardHitPacketSerialR642-1||!(packet.sampledAt<elapsed))return null;
   }
   return new CoreHardHitWitnessR642(e,receipt,profile,rule,elapsed);
  }
  void ContinueCoreHardHitDisplayPendingR642(Enemy e,int dealt,CoreHardHitWitnessR642 witness){
   if(dealt<=0||witness==null||!ReferenceEquals(e,witness.actor)||elapsed!=witness.capturedAt||!CoreHardClosureScopeV21()||!NativeTerrain||!e.active||e.dormant||e.encounterIndex!=0||e.visualScale!=1||!string.IsNullOrEmpty(e.npcState)||e.npcTime!=0||e.clock!=0||!enemies.Contains(e)||!coreHardBalancedActorsV21.Contains(e)&&!coreHardLocalActorsV21.Contains(e))return;
   CoreHardHitDisplayV39 receipt;CoreHardRuntimeV21 runtime;NpcDefinition profile;var rule=CoreHardActorRuleV21(e);
   if(!coreHardDisplayHitsV39.TryGetValue(e,out receipt)||!ReferenceEquals(receipt,witness.receipt)||receipt.epoch!=witness.epoch||receipt.epoch!=coreHardClosureEpochV21||!ReferenceEquals(receipt.runtime,witness.runtime)||!coreHardActorsV21.TryGetValue(e,out runtime)||!ReferenceEquals(runtime,witness.runtime)||!runtime.initialized||runtime.last!=witness.prior||!npcDefinitions.TryGetValue(e.sourceNPC,out profile)||!ReferenceEquals(profile,witness.profile)||profile.kind!=e.kind||!ReferenceEquals(rule,witness.rule))return;
   if(receipt.dealt!=witness.dealt||receipt.prior!=witness.prior||receipt.next!=witness.next||receipt.at!=witness.at||receipt.sampledAt!=witness.sampledAt||receipt.hp!=witness.hp||receipt.maxHp!=witness.maxHp||receipt.x!=witness.x||receipt.y!=witness.y||receipt.depth!=witness.depth||receipt.reaction!=witness.reaction)return;
   if(!(e.hp<witness.hp)||e.hp!=Math.Max(0,witness.hp-dealt)||e.maxHp!=witness.maxHp||e.x!=witness.x||e.y!=witness.y||e.nativeDepth!=witness.depth||e.hitPause!=witness.hitPause||e.hp<=0&&e.state!=CombatState.Dead)return;
   // Reuse the unchanged positive-hit producer with the proven source prior.
   // Its original current-HP rule selects DAMAGE or DYING; no state is entered.
   RecordCoreHardHitDisplayPendingV39(e,dealt,witness.prior);
  }
  // Called only after an actual successful SourceHit, or at the precise
  // existing TickEnemyCore return where real defender hitPause consumed dt.
  // It samples that transaction/deferred owner Tick; it never extends pause.
  void SampleCoreHardHitDisplayV39(Enemy e){
   CoreHardHitDisplayV39 receipt;CoreHardRuntimeV21 runtime;if(e==null||!CoreHardClosureScopeV21()||!NativeTerrain||!e.active||e.dormant||e.encounterIndex!=0||e.visualScale!=1||!string.IsNullOrEmpty(e.npcState)||e.npcTime!=0||e.clock!=0||!enemies.Contains(e)||!coreHardDisplayHitsV39.TryGetValue(e,out receipt)||receipt.dealt<=0||receipt.epoch!=coreHardClosureEpochV21||!coreHardActorsV21.TryGetValue(e,out runtime)||!ReferenceEquals(runtime,receipt.runtime)||!runtime.initialized||runtime.last!=receipt.prior)return;
   var rule=CoreHardActorRuleV21(e);if(rule==null||receipt.next!=(e.hp<=0?rule.dyingState:rule.damageState))return;
   receipt.sampledAt=elapsed;receipt.hp=e.hp;receipt.maxHp=e.maxHp;receipt.x=e.x;receipt.y=e.y;receipt.depth=e.nativeDepth;receipt.reaction=e.state;
  }
  public bool TryCoreHardHitDisplayPendingV39(Enemy e,out string nextState){
   nextState=null;CoreHardHitDisplayV39 receipt;CoreHardRuntimeV21 runtime;NpcDefinition profile;if(e==null||!CoreHardClosureScopeV21()||!NativeTerrain||!e.active||e.dormant||e.encounterIndex!=0||e.visualScale!=1||!string.IsNullOrEmpty(e.npcState)||e.npcTime!=0||e.clock!=0||!enemies.Contains(e)||!coreHardDisplayHitsV39.TryGetValue(e,out receipt)||receipt.dealt<=0||receipt.epoch!=coreHardClosureEpochV21||receipt.sampledAt!=elapsed||e.hp!=receipt.hp||e.maxHp!=receipt.maxHp||e.x!=receipt.x||e.y!=receipt.y||e.nativeDepth!=receipt.depth||e.state!=receipt.reaction||!coreHardActorsV21.TryGetValue(e,out runtime)||!ReferenceEquals(runtime,receipt.runtime)||!runtime.initialized||runtime.last!=receipt.prior||!npcDefinitions.TryGetValue(e.sourceNPC,out profile)||profile.kind!=e.kind)return false;
   var rule=CoreHardActorRuleV21(e);if(rule==null||receipt.next!=(e.hp<=0?rule.dyingState:rule.damageState)||!npcStates.ContainsKey(e.sourceNPC+"/"+receipt.next))return false;nextState=receipt.next;return true;
  }
  // Read-only display admission for an exact fresh original birth. EnterRoom
  // can return at the end of Step before this actor's first ordinary TickEnemy.
  // EntryActivation marks it at that first Tick, before the managed state owner
  // commits the original INIT_STATE. No renderer initializes or advances it.
  public bool TryCoreHardSourceInitializationPendingV32(Enemy e,out string initialState){
   initialState=null;
   // This display admission belongs only to the original final seven births.
   // Earlier NORMAL extractors keep their existing source-entry hold behavior.
   if(room!=4||EncounterIndex!=0||e==null||!(CoreHardExtractorV21(e.sourceNPC)||e.sourceNPC=="NUI_NASOD_KING"||e.sourceNPC=="NUI_EL_POWER_PLANT"||e.sourceNPC=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"))return false;
   if(coreHardV21==null||coreHardEntryActivationV21==null||e==null||!e.active||e.dormant||e.aiEnabled||e.hp<=0||e.hp!=e.maxHp||e.clock!=0||e.npcTime!=0||e.deathTime!=0||!string.IsNullOrEmpty(e.npcState)||coreHardEntryInitializedV21.Contains(e)||!coreHardBalancedActorsV21.Contains(e)||!NativeTerrain)return false;
   int order=enemies.IndexOf(e);if(order<0||CurrentRoom.id!="Dungeon_Altera_Core_Hard-"+room||e.encounterIndex!=EncounterIndex)return false;
   var node=Array.Find(coreHardV21.nodes,n=>n.room==room&&n.encounter==EncounterIndex);
   if(node==null||CurrentEncounter==null||CurrentEncounter.id!=node.id||CurrentEncounter.sourceWorldId!=node.world)return false;
   var birth=Array.Find(coreHardV21.births,b=>b.room==room&&b.order==order);
   var activation=Array.Find(coreHardEntryActivationV21.births,b=>b.room==room&&b.order==order);
   NpcDefinition profile;npcDefinitions.TryGetValue(e.sourceNPC,out profile);var rule=CoreHardActorRuleV21(e);
   if(birth==null||activation==null||profile==null||profile.kind!=e.kind||birth.npc!=e.sourceNPC||activation.npc!=e.sourceNPC||birth.encounter!=EncounterIndex||birth.sourceKeyCode!=e.sourceKeyCode||string.IsNullOrEmpty(activation.initialState)||activation.managed!=(rule!=null)||rule!=null&&rule.startState!=activation.initialState)return false;
   CoreHardRuntimeV21 runtime;if(coreHardActorsV21.TryGetValue(e,out runtime)&&runtime.initialized)return false;
   initialState=activation.initialState;return true;
  }
  // Opt-in preparation happens in the ordinary actor Step, after unchanged
  // fresh-birth and authored balance validation. No capture/render path calls it.
  void TickCoreHardEntryActivationV21(Enemy e){
   if(coreHardEntryActivationV21==null||coreHardV21==null||e==null||e.dormant||e.encounterIndex!=EncounterIndex||coreHardEntryInitializedV21.Contains(e))return;
   int order=enemies.IndexOf(e);var b=Array.Find(coreHardEntryActivationV21.births,r=>r.room==room&&r.order==order);
   if(b==null)return; // Later exact local summons retain their own source constructor.
   if(b.npc!=e.sourceNPC||CurrentRoom.id!="Dungeon_Altera_Core_Hard-"+room||!coreHardBalancedActorsV21.Contains(e))throw new InvalidOperationException("Core activation requires this original balanced actor");
   coreHardEntryInitializedV21.Add(e);if(b.managed)return; // Original eight controller state owners stay authoritative.
   if(e.clock!=0||e.npcTime!=0||!string.IsNullOrEmpty(e.npcState)||e.aiEnabled)throw new InvalidOperationException("Core entry may initialize an original actor only once before its first ordinary Step");
   var oldHp=e.hp;var oldMax=e.maxHp;var oldX=e.x;var oldY=e.y;var oldDepth=e.nativeDepth;NpcStateDefinition state=npcStates[b.npc+"/"+b.initialState];EnterNpcState(e,state);
   if(e.hp!=oldHp||e.maxHp!=oldMax||e.x!=oldX||e.y!=oldY||e.nativeDepth!=oldDepth)throw new InvalidOperationException("Core entry initialization cannot rewrite birth HP/position");
   bool ai=b.sourceActive&&(!b.sourceStopAtStart||SimpleDangerIntroductions&&IsDangerCombatActor(e));e.aiEnabled=ai;
   if(SimpleDangerIntroductions&&b.sourceStopAtStart&&IsDangerCombatActor(e))coreHardStopIntroReleasedV21.Add(e);
   coreHardEntryActivationEventsV21.Add(new CoreHardEntryActivationEventV21{room=room,order=order,npc=e.sourceNPC,state=e.npcState,aiBefore=false,aiAfter=ai,sourceActive=b.sourceActive,sourceStopAtStart=b.sourceStopAtStart,managed=false,policy=b.sourceStopAtStart?"HOLD exact native/global STOP release owner; no timer or forced AI release":ai?"Original source ACTIVE, exact initial state, generic2D source runner":"Original source inactive body state retained; source callbacks remain separate"});
  }
 }
}
