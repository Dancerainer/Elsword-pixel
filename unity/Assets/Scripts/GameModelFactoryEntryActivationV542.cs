using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ElswordPixel {
 [Serializable] public sealed class FactoryEntryActivationBirthV542 {
  public int room,order,encounter,sourceKeyCode;
  public string npc,kind,initialState,initialAnimation,role,sourceActiveDeclaration;
  public float initialDuration;
  public bool sourceStopAtStart;
 }
 [Serializable] public sealed class FactoryEntryActivationDocumentV542 {
  public string schema,policy,sourceModelSha256,sourceInitialStatesSha256,fullRulesSha256,mechanismsSha256,sourceControllersSha256,receipt;
  public bool enabled,fullRulesAvailable,runtimeAdmitted,nativeEquivalent;
  public FactoryEntryActivationBirthV542[] births;
 }
 [Serializable] public sealed class FactorySourceControllerStateV554 {
  public string id,sourceAnimation,owner;public float sourceMotionSeconds,logicalSeconds;public bool loop;
 }
 [Serializable] public sealed class FactorySourceControllerDocumentV554 {
  public string schema,npc,ownerModule,sourceBodySha256,sourceContractSha256,sourceBindingSha256;
  public bool genericProfileRequired,sourceNoBrain,nativePositionCameraRetirementConsumersImplemented;
  public float sourceMaxHp;public FactorySourceControllerStateV554[] states;
 }
 public struct FactoryEntryActivationRecordV542 {
  public Enemy actualOwner;public int room,order;
  public string npc,initialState,role,sourceActiveDeclaration,outcome;
  public bool aiBefore,aiAfter;
 }
 public sealed partial class GameModel {
  const string FactoryRouteV542="Dungeon_Altera_Nasod_Factory_Normal";
  const string FactoryActivationReceiptV542="786bf75f350358d53812f2c03c6b41d6723e3a60ecd965b3912ef3da60694b1b";
  FactoryEntryActivationBirthV542[] factoryEntryBirthsV542;
  RoomDefinition[] factoryEntryRoomsV542;
  readonly HashSet<Enemy> factoryEntryObservedV542=new HashSet<Enemy>();
  readonly List<FactoryEntryActivationRecordV542> factoryEntryRecordsV542=new List<FactoryEntryActivationRecordV542>();
  public bool FactoryEntryActivationConfiguredV542 {get{return factoryEntryBirthsV542!=null;}}
  public FactoryEntryActivationRecordV542[] ReadFactoryEntryActivationRecordsV542(){return factoryEntryRecordsV542.ToArray();}
  static string FactoryHashV542(byte[] raw){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(raw)).Replace("-","").ToLowerInvariant();}
  static string FactoryBirthTextV542(FactoryEntryActivationDocumentV542 d){
   var values=new List<string>{d.schema,d.policy,d.sourceModelSha256,d.sourceInitialStatesSha256};
   foreach(var b in d.births)values.Add(string.Join("|",new[]{b.room.ToString(CultureInfo.InvariantCulture),b.order.ToString(CultureInfo.InvariantCulture),b.encounter.ToString(CultureInfo.InvariantCulture),b.sourceKeyCode.ToString(CultureInfo.InvariantCulture),b.npc,b.kind,b.initialState,b.initialAnimation,BitConverter.ToInt32(BitConverter.GetBytes(b.initialDuration),0).ToString("X8",CultureInfo.InvariantCulture),b.role,b.sourceActiveDeclaration,b.sourceStopAtStart?"1":"0"}));
   return string.Join("\n",values.ToArray())+"\n";
  }
  static string FactoryRoleV542(string npc){
   switch(npc){
    case "NUI_WALLY_9TH_TYPE2":case "NUI_NASOD_GUARD":case "NUI_NASOD_SHERIFF":case "NUI_NASOD_HEALER_BLADER":return "ordinary-combat";
    case "NUI_NASOD_MAKER":return "stationary-maker";
    case "NUI_RAVEN_CLONE":return "boss-combat-stop";
    case "NUI_EVENT_NPC_NASOD_FACTORY":return "no-brain-director";
    case "NUI_FACTORY_PRESS":case "NUI_FACTORY_PRESS3":return "scripted-press";
    case "NUI_BOX_ALTERA":return "static-box";
    case "NUI_SMALLGATE_BESMA_AIR_SHIP":return "source-gate";
    case "NUI_SMALLGATE_POST_BESMA_AIR_SHIP":return "gate-post";
    default:return null;
   }
  }
  static FactoryEntryActivationBirthV542 CopyFactoryBirthV542(FactoryEntryActivationBirthV542 b){return new FactoryEntryActivationBirthV542{room=b.room,order=b.order,encounter=b.encounter,sourceKeyCode=b.sourceKeyCode,npc=b.npc,kind=b.kind,initialState=b.initialState,initialAnimation=b.initialAnimation,initialDuration=b.initialDuration,role=b.role,sourceActiveDeclaration=b.sourceActiveDeclaration,sourceStopAtStart=b.sourceStopAtStart};}
  // R554 role-aware revision: eleven unchanged real profiles plus the original
  // R405 NO_BRAIN event controller. No fabricated director geometry/profile.
  // A future reviewed loader must deserialize these exact three pinned raw
  // documents and pass the same objects installed by the original setters.
  // JSON-to-object loading, API and Step are not proved by this source candidate.
  public void SetFactoryEntryActivationV542(FactoryEntryActivationDocumentV542 d,NpcDefinition[] loadedFullProfiles,byte[] fullRulesJson,NpcMechanismDocument loadedMechanisms,byte[] mechanismsJson,FactorySourceControllerDocumentV554 controller,byte[] controllerJson){
   if(d==null){factoryEntryBirthsV542=null;factoryEntryRoomsV542=null;factoryEntryObservedV542.Clear();factoryEntryRecordsV542.Clear();return;}
   if(d.schema!="factory-normal-role-aware-entry-activation-r554"||d.policy!="authored2d-original-initial-state-no-hp-position-rewrite"||d.sourceModelSha256!="3c7ecf2fb085ea6cff567ab8fe3853ac951d2ac40acf5f51a8018e7a6ca81890"||d.sourceInitialStatesSha256!="35cd811db000e7ad6be9d2633046312df3186cc5543e1dd0ae6a296f7f708e8c"||!d.enabled||!d.fullRulesAvailable||d.runtimeAdmitted||d.nativeEquivalent||d.births==null||d.births.Length!=64||d.fullRulesSha256!="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159"||fullRulesJson==null||FactoryHashV542(fullRulesJson)!=d.fullRulesSha256||d.mechanismsSha256!="fdbed410573f41f7d895bf2687546125c50f4ab8e079cd6ed94428489d1d8d0e"||mechanismsJson==null||FactoryHashV542(mechanismsJson)!=d.mechanismsSha256||d.sourceControllersSha256!="b1917a6554dc8619570011a2493e6637889dd86a32f721a21518b30b08bc5392"||controllerJson==null||FactoryHashV542(controllerJson)!=d.sourceControllersSha256||d.receipt!=FactoryActivationReceiptV542||FactoryHashV542(Encoding.UTF8.GetBytes(FactoryBirthTextV542(d)))!=d.receipt)throw new ArgumentException("Factory complete source-rule/activation data binding is unavailable or changed");
   if(loadedFullProfiles==null||loadedFullProfiles.Length!=11||npcDefinitions.Count!=11||room!=0||elapsed!=0||rooms.Count!=3||!NativeTerrain||factoryPopulationPolicyV21==null||factoryEventBindingV21==null)throw new InvalidOperationException("Factory activation requires a fresh independent11-profile plus source-controller Normal model, original R405 owner bindings and first Step");
   var profiles=new HashSet<string>();foreach(var profile in loadedFullProfiles){NpcDefinition current;if(profile==null||FactoryRoleV542(profile.id)==null||FactoryRoleV542(profile.id)=="no-brain-director"||!profiles.Add(profile.id)||!npcDefinitions.TryGetValue(profile.id,out current)||!ReferenceEquals(current,profile))throw new ArgumentException("Factory exact loaded profile identity/object differs");
    foreach(var state in profile.states)if(!npcMechanismRules.ContainsKey(profile.id+"/"+state.id))throw new InvalidOperationException("Factory full available source mechanism-state data is not loaded");
   }
   if(loadedMechanisms==null||loadedMechanisms.states==null||loadedMechanisms.states.Length!=185||npcMechanismRules.Count!=185||loadedMechanisms.sourceRulesSha256!=d.fullRulesSha256||loadedMechanisms.runtimeAdmitted||loadedMechanisms.sourceMechanismsComplete)throw new ArgumentException("Factory exact complete mechanism document differs");
   foreach(var row in loadedMechanisms.states){NpcMechanismStateRule installed;if(row==null||!npcMechanismRules.TryGetValue(row.npc+"/"+row.state,out installed)||!ReferenceEquals(installed,row))throw new ArgumentException("Factory loaded mechanism row identity/object differs");}
   ValidateFactoryControllerRoleV554(controller);
   int[] counts={31,32,1};int cursor=0,ordinary=0,maker=0,clone=0,inactive=0;var copy=new FactoryEntryActivationBirthV542[64];
   for(int r=0;r<3;r++){
    if(rooms[r].id!=FactoryRouteV542+"-"+r||rooms[r].enemies.Count!=counts[r])throw new ArgumentException("Factory original room/birth order changed");
    for(int order=0;order<counts[r];order++){
     var b=d.births[cursor];var spawn=rooms[r].enemies[order];NpcStateDefinition initial;NpcDefinition profile;
     if(b==null||b.room!=r||b.order!=order||b.role!=FactoryRoleV542(b.npc)||spawn.sourceNPC!=b.npc||spawn.kind!=b.kind||spawn.encounterIndex!=b.encounter||spawn.sourceKeyCode!=b.sourceKeyCode||spawn.aiEnabled)throw new ArgumentException("Factory original source birth boundary differs");
     if(b.role=="no-brain-director"){
      if(npcDefinitions.ContainsKey(b.npc)||b.initialState!="EVENT_NPC_NASOD_FACTORY_START_READY"||b.initialAnimation!="Dying"||b.initialDuration!=11f/30)throw new ArgumentException("Factory director belongs only to the original source-owned controller");
     }else{
      float logicalInitial=b.npc=="NUI_FACTORY_PRESS"?1.5f:b.npc=="NUI_FACTORY_PRESS3"?4f:b.initialDuration;
      if(!npcDefinitions.TryGetValue(b.npc,out profile)||profile.kind!=b.kind||!npcStates.TryGetValue(b.npc+"/"+b.initialState,out initial)||initial.sourceAnimation!=b.initialAnimation||initial.duration!=logicalInitial||initial.entryOffsetY!=0)throw new ArgumentException("Factory exact source INIT_STATE/profile clock differs");
     }
     if(b.role=="ordinary-combat"){ordinary++;if(b.sourceActiveDeclaration!="ABSENT"||b.sourceStopAtStart)throw new ArgumentException("Ordinary Factory authored2D activation cannot override explicit source inactive/STOP");}
     else if(b.role=="stationary-maker")maker++;else if(b.role=="boss-combat-stop")clone++;
     if(b.sourceActiveDeclaration=="EXPLICIT_FALSE")inactive++;else if(b.sourceActiveDeclaration!="ABSENT")throw new ArgumentException("Factory ACTIVE provenance must remain absent or explicit false");
     if(b.sourceStopAtStart!=(b.npc=="NUI_RAVEN_CLONE"))throw new ArgumentException("Factory original Clone STOP declaration changed");
     copy[cursor++]=CopyFactoryBirthV542(b);
    }
   }
   if(ordinary!=47||maker!=4||clone!=1||inactive!=12)throw new ArgumentException("Factory exact47+4+1/12source-inactive assignment differs");
   foreach(var e in enemies)if(e.clock!=0||e.npcTime!=0||e.deathTime!=0||e.hitPause!=0||e.aiEnabled||!string.IsNullOrEmpty(e.npcState)||e.hp<=0||e.hp!=e.maxHp)throw new InvalidOperationException("Factory activation is never a late actor reset or balance operation");
   factoryEntryBirthsV542=copy;factoryEntryRoomsV542=rooms.ToArray();factoryEntryObservedV542.Clear();factoryEntryRecordsV542.Clear();
  }
  // Role data is checked separately from the generic profile dictionary.
  // Ready's source Dying loop is11/30, its explicit source condition is5s.
  // This only binds the existing owner; position/camera/retirement remain HOLD.
  void ValidateFactoryControllerRoleV554(FactorySourceControllerDocumentV554 c){
   var r=factoryEventBindingV21;
   if(c==null||c.schema!="factory-source-owned-controller-role-r554"||c.npc!="NUI_EVENT_NPC_NASOD_FACTORY"||c.ownerModule!="GameModelFactoryEventStartV21"||c.genericProfileRequired||!c.sourceNoBrain||c.nativePositionCameraRetirementConsumersImplemented||c.sourceMaxHp!=30000||c.sourceBodySha256!="d66c1da2ecc777859c705670ebe611ad93365eccc46b2e45b148bb4207bad84b"||c.sourceContractSha256!="8bf4f25fc56d318b6c6e5f4e13da16bb6c5c44f4aa7464afa545195ed3496146"||c.sourceBindingSha256!="10818476cd32c8264d5bd85ab2c686792dfbe36a72845ab21edda60195d7913c"||c.states==null||c.states.Length!=3)throw new ArgumentException("Factory exact NO_BRAIN controller role differs");
   string[] ids={"EVENT_NPC_NASOD_FACTORY_START_READY","EVENT_NPC_NASOD_FACTORY_START","EVENT_NPC_NASOD_FACTORY_DYING"};
   for(int i=0;i<3;i++){var s=c.states[i];float motion=i==1?11.6f:11f/30;float logical=i==0?5:motion;
    if(s==null||s.id!=ids[i]||s.sourceAnimation!=(i==1?"Start":"Dying")||s.sourceMotionSeconds!=motion||s.logicalSeconds!=logical||s.loop!=(i==0)||s.owner!="GameModelFactoryEventStartV21")throw new ArgumentException("Factory controller source-state clocks differ");}
   if(r==null||r.schema!="factory-event-no-brain-start-finite-2d-v21-r1"||!r.enabled||r.nativeExecuted||r.nativeEquivalent||r.runtimeAdmitted||r.finalBalanceApproved||r.route!=FactoryRouteV542||r.roomId!=FactoryRouteV542+"-0"||r.world!="WI_ALTERA_NASOD_FACTORY_STAGE1"||r.npc!=c.npc||r.readySeconds!=5||r.startMotionSeconds!=11.6f||r.dyingMotionSeconds!=11f/30||r.sourceMaxHp!=30000||r.clockAdapter!="ordinary-Step-threshold-crossing-with-remainder-finite2D-v21"||r.sourceLuaSha256!="4e263fcbfde3ba88b1dda0095b74116c98bb5984d4d75a6af7cc02e3eab14354"||r.sourceMotionSha256!="16f0507cd0496bc1a71339bbec398cbf0688a06df4a36a3c60e2803a9f40a61a"||r.sourceStartSha256!="53abb2ab1ba6872c762983f2902fe77375241208bdc35da4556f4567bd303d19"||r.sourceFrameSha256!="25c09e72f8f2e6ccc4872e17ecb3a61926735950224f41f5cf44b12e41f808e7"||r.sourceDyingSha256!="9b7f0c6a488a80661f886f8462ef8f21390b6d1685263b833b64315de8cd772e")throw new ArgumentException("Factory original R405 source-owned event binding differs");
  }
  // Single hook: ordinary actor Step, before cave/environment/mechanism clocks.
  // A null configuration is a return-only path for every existing route.
  // The room's activation table owns its static actors. A damage packet
  // must not turn an inactive machine into a generic combat NPC.
  bool FactoryOwnsBirthAiV58(Enemy e){
   if(factoryEntryBirthsV542==null||room<0||room>=factoryEntryRoomsV542.Length||!ReferenceEquals(CurrentRoom,factoryEntryRoomsV542[room]))return false;
   int order=enemies.IndexOf(e);
   return order>=0&&order<CurrentRoom.enemies.Count;
  }
  void TickFactoryEntryActivationV542(Enemy e){
   if(factoryEntryBirthsV542==null||e==null)return;
   if(CurrentRoom.id!=FactoryRouteV542+"-"+room)return;
   if(room<0||room>=3||!ReferenceEquals(CurrentRoom,factoryEntryRoomsV542[room]))throw new InvalidOperationException("Factory activation cannot follow a replaced room/model binding");
   var stale=new List<Enemy>();foreach(var old in factoryEntryObservedV542)if(!enemies.Contains(old))stale.Add(old);foreach(var old in stale)factoryEntryObservedV542.Remove(old);
   if(!e.active||e.dormant||e.encounterIndex!=EncounterIndex||factoryEntryObservedV542.Contains(e))return;
   int order=enemies.IndexOf(e);if(order<0)throw new InvalidOperationException("Foreign actor cannot enter Factory source activation");
   if(order>=CurrentRoom.enemies.Count)return; // Appended local summons retain their own constructor/owner.
   var b=Array.Find(factoryEntryBirthsV542,q=>q.room==room&&q.order==order);
   if(b==null||b.npc!=e.sourceNPC||b.kind!=e.kind||b.encounter!=e.encounterIndex||b.sourceKeyCode!=e.sourceKeyCode)throw new InvalidOperationException("Factory activation birth identity/order mismatch");
   bool before=e.aiEnabled;string outcome;
   if(b.role=="ordinary-combat"){
    if(e.hp<=0||e.hp!=e.maxHp||e.clock!=0||e.npcTime!=0||e.deathTime!=0||e.hitPause!=0||e.aiEnabled||!string.IsNullOrEmpty(e.npcState))throw new InvalidOperationException("Factory combat INIT_STATE belongs only to a fresh actor's first ordinary Step");
    var hp=e.hp;var max=e.maxHp;var x=e.x;var y=e.y;var depth=e.nativeDepth;
    EnterNpcState(e,npcStates[b.npc+"/"+b.initialState]);
    if(e.hp!=hp||e.maxHp!=max||e.x!=x||e.y!=y||e.nativeDepth!=depth)throw new InvalidOperationException("Factory entry must not rewrite source birth HP or position");
    e.aiEnabled=true;outcome="authored2d-generic47-exact-source-initial-state";
   }else if(b.role=="boss-combat-stop"&&factoryRavenIntroConfiguredV21){
    PrepareFactoryRavenIntroBirthV21(e);outcome="authored2d-original-START-held-for-same-view-focus";
   }else{
    if(e.aiEnabled)throw new InvalidOperationException("Factory source-owned/STOP actor cannot be generically enabled");
    outcome=b.role=="boss-combat-stop"?"HOLD-Factory-specific-originalSTOP-and-actualview-ACK-not-implemented":b.role=="stationary-maker"?"source-owned-original-R405-population-initial-state-unmodified":"source-owned-callback-or-host-consumer-remains-unimplemented-here";
   }
   factoryEntryObservedV542.Add(e);
   factoryEntryRecordsV542.Add(new FactoryEntryActivationRecordV542{actualOwner=e,room=room,order=order,npc=b.npc,initialState=b.initialState,role=b.role,sourceActiveDeclaration=b.sourceActiveDeclaration,outcome=outcome,aiBefore=before,aiAfter=e.aiEnabled});
  }
 }
}
