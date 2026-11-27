using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  const string PlainNormalR1258="Dungeon_Altera_Plain_Normal";
  const string PlainRecycleR1258="Dungeon_Altera_Plain_Recycle_Normal";
  const string PlainRegionalRulesSHAR1258="159ef67868b5c07a977afbf5d25ac497274d727c0c4fad5bd8f4325c20f63035";
  const string PlainHouseR1258="NUI_NASOD_MOUSE_HOUSE";
  const string PlainMineR1258="NUI_ALTERA_MINE";
  string plainRouteR1258,plainPendingRouteR1258;
  NpcDefinition[] plainDefinitionsR1258,plainPendingDefinitionsR1258;
  public bool PlainOriginalSourceBoundR1258 {get{return !string.IsNullOrEmpty(plainRouteR1258);}}
  public bool PlainOriginalConsumersCompleteR1258 {get{return false;}}
  public string PlainOriginalMineHoldReasonR1258 {get{return plainRouteR1258==PlainRecycleR1258&&!PlainMineOriginalReadyImplementedR1269?
   "Original Mine source XYZ, original typed explosion and the scoped actual Major/Minor/audio consumer must be bound before ordinary play.":"";}}

  static readonly string[] plainNormalIdsR1258={
   "NUI_BARRIERMACHINE_FIRE","NUI_BARRIERMACHINE_ICE","NUI_BOX_ALTERA","NUI_NASOD_ELEMENT_FIRE","NUI_NASOD_ELEMENT_FIRE_SOLO",
   "NUI_NASOD_ELEMENT_ICE","NUI_NASOD_ELEMENT_ICE_SOLO","NUI_NASOD_GUARDIAN_SMALL","NUI_NASOD_MOUSE",PlainHouseR1258,
   "NUI_NASOD_SPRAY","NUI_NASOD_WATCH","NUI_SHOCK_STICK","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP"};
  static readonly string[] plainRecycleIdsR1258={
   "NUI_AIRSHIP_SOLDIER_GUNNER","NUI_AIRSHIP_SOLDIER_SWORDMAN",PlainMineR1258,"NUI_BOX_ALTERA","NUI_NASOD_DRILLER_CANNON",
   "NUI_NASOD_MINER_THIN_DEFECT","NUI_NASOD_SPRAY","NUI_NASOD_WATCH","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP"};

  // These are INIT_STATE.START_STATE, not WAIT_STATE or a guessed native default.
  // Runtime/native ACTIVE, NoBrain, initial SHOW and STOP defaults remain unknown.
  static string PlainOriginalStartR1258(string id,out int stateCount){
   switch(id){
    case "NUI_AIRSHIP_SOLDIER_GUNNER":stateCount=28;return "AIRSHIP_SOLDIER_GUNNER_START";
    case "NUI_AIRSHIP_SOLDIER_SWORDMAN":stateCount=27;return "AIRSHIP_SOLDIER_SWORDMAN_START";
    case PlainMineR1258:stateCount=3;return "ALTERA_MINE_WAIT";
    case "NUI_BARRIERMACHINE_FIRE":stateCount=8;return "BARRIER_CREATEMACHINE_FIRE_START";
    case "NUI_BARRIERMACHINE_ICE":stateCount=8;return "BARRIER_CREATEMACHINE_ICE_START";
    case "NUI_BOX_ALTERA":stateCount=5;return "BOX_WAIT";
    case "NUI_NASOD_DRILLER_CANNON":stateCount=33;return "NASOD_DRILLER_CANNON_START";
    case "NUI_NASOD_ELEMENT_FIRE":stateCount=35;return "NASOD_ELEMENT_FIRE_START";
    case "NUI_NASOD_ELEMENT_FIRE_SOLO":stateCount=34;return "NASOD_ELEMENT_FIRE_SOLO_START";
    case "NUI_NASOD_ELEMENT_ICE":stateCount=35;return "NASOD_ELEMENT_ICE_START";
    case "NUI_NASOD_ELEMENT_ICE_SOLO":stateCount=36;return "NASOD_ELEMENT_ICE_SOLO_START";
    case "NUI_NASOD_GUARDIAN_SMALL":stateCount=25;return "NASOD_GUARDIAN_SMALL_WAIT";
    case "NUI_NASOD_MINER_THIN_DEFECT":stateCount=26;return "NASOD_MINER_THIN_DEFECT_START";
    case "NUI_NASOD_MOUSE":stateCount=20;return "NASOD_MOUSE_START";
    case PlainHouseR1258:stateCount=6;return "NASOD_MOUSEHOUSE_START";
    case "NUI_NASOD_SPRAY":stateCount=28;return "NASOD_SPRAY_START";
    case "NUI_NASOD_WATCH":stateCount=28;return "NASOD_WATCH_START";
    case "NUI_SHOCK_STICK":stateCount=7;return "SHOCK_STICK_WAITSTART";
    case "NUI_SMALLGATE_BESMA_AIR_SHIP":stateCount=4;return "SMALL_GATE_RED_WAIT_INVINCIBLE";
    case "NUI_SMALLGATE_POST_BESMA_AIR_SHIP":stateCount=5;return "SMALL_GATE_POST_WAIT_INVINCIBLE";
    default:throw new ArgumentException("NPC is outside the exact two original Plain profile cohorts");
   }
  }

  // A shallow value projection: only initialState and House's explicit consumer
  // role differ. Original state arrays, geometry, ranges, clocks and flags remain.
  public static NpcDefinition[] ProjectPlainOriginalDefinitionsR1258(string route,string regionalRulesSHA,NpcDefinition[] definitions){
   var allowed=route==PlainNormalR1258?plainNormalIdsR1258:route==PlainRecycleR1258?plainRecycleIdsR1258:null;
   if(allowed==null||regionalRulesSHA!=PlainRegionalRulesSHAR1258||definitions==null||definitions.Length!=allowed.Length)throw new ArgumentException("Unbound original Plain route/rules/profile cohort");
   var seen=new HashSet<string>(StringComparer.Ordinal);var result=new NpcDefinition[definitions.Length];
   for(int i=0;i<definitions.Length;i++){
    var p=definitions[i];int count;
    if(p==null||!Array.Exists(allowed,id=>id==p.id)||!seen.Add(p.id)||p.states==null)throw new ArgumentException("Invalid/duplicate original Plain profile");
    string start=PlainOriginalStartR1258(p.id,out count);
    if(p.states.Length!=count||Array.FindAll(p.states,s=>s!=null&&s.id==start).Length!=1||!string.IsNullOrEmpty(p.initialState)&&p.initialState!=start)throw new ArgumentException("Changed original Plain START_STATE/state cohort");
    if(p.id==PlainHouseR1258){
     if(!string.IsNullOrEmpty(p.environmentKind)&&p.environmentKind!="source-nasod-mouse-house")throw new ArgumentException("Changed original Plain House role");
    }
    result[i]=new NpcDefinition{
     runtimeAdmitted=p.runtimeAdmitted,sourceMechanismsComplete=p.sourceMechanismsComplete,id=p.id,kind=p.kind,
     waitState=p.waitState,walkState=p.walkState,getUpState=p.getUpState,initialState=start,
     environmentKind=p.id==PlainHouseR1258?"source-nasod-mouse-house":p.environmentKind,
     environment=p.environment,traversal=p.traversal,summon=p.summon,
     senseRange=p.senseRange,lostRange=p.lostRange,approachDistance=p.approachDistance,
     physicalAttack=p.physicalAttack,magicAttack=p.magicAttack,bodyWidth=p.bodyWidth,bodyHeight=p.bodyHeight,
     bodySpheres=p.bodySpheres,states=p.states,reactions=p.reactions};
   }
   return result;
  }

  bool PlainOriginalRoomScopeR1258(string route){return !string.IsNullOrEmpty(route)&&(PlainProductInstallingScopeR1320(route)||RoomCount==3&&room>=0&&room<3&&CurrentRoom!=null&&CurrentRoom.id==route+"-"+room);}
  // Configure after the original model/rooms exist. This never calls SetRooms,
  // Step or changes a source birth's active, aiEnabled, position, HP or clock.
  public void SetPlainOriginalNpcDefinitionsR1258(string route,string regionalRulesSHA,NpcDefinition[] definitions){
   if(!PlainOriginalRoomScopeR1258(route))throw new InvalidOperationException("Original Plain rooms must be bound before their source definitions");
   var projected=ProjectPlainOriginalDefinitionsR1258(route,regionalRulesSHA,definitions);
   plainPendingRouteR1258=route;plainPendingDefinitionsR1258=projected;
   try{SetNpcDefinitions(projected);plainRouteR1258=route;plainDefinitionsR1258=projected;}
   finally{plainPendingRouteR1258=null;plainPendingDefinitionsR1258=null;}
  }
  void BeginPlainDefinitionReplacementR1258(IEnumerable<NpcDefinition> definitions){
   try{ClearPlainShockStickSourceR1477();UnbindPlainOriginalPairR1258();ClearPlainMineOriginalServicesR1269();}
   finally{plainRouteR1258=null;plainDefinitionsR1258=null;
    if(!ReferenceEquals(definitions,plainPendingDefinitionsR1258)){plainPendingRouteR1258=null;plainPendingDefinitionsR1258=null;}}
  }
  bool IsPlainOriginalMineZeroBodyR1258(NpcDefinition p){
   return plainPendingRouteR1258==PlainRecycleR1258&&plainPendingDefinitionsR1258!=null&&
    Array.Exists(plainPendingDefinitionsR1258,d=>ReferenceEquals(d,p))&&PlainOriginalRoomScopeR1258(plainPendingRouteR1258)&&
    p.id==PlainMineR1258&&p.kind=="altera-mine"&&string.IsNullOrEmpty(p.environmentKind)&&
    p.bodyWidth==14f&&p.bodyHeight==0&&p.bodySpheres!=null&&p.bodySpheres.Length==0&&
    p.senseRange==2940f&&p.lostRange==3024f&&p.approachDistance==42f&&p.physicalAttack==28f&&p.magicAttack==28f&&
    p.initialState=="ALTERA_MINE_WAIT"&&p.waitState=="ALTERA_MINE_WAIT"&&p.walkState==p.waitState&&
    p.states!=null&&p.states.Length==3&&Array.Exists(p.states,s=>s.id=="ALTERA_MINE_READY_EXPLODE")&&Array.Exists(p.states,s=>s.id=="ALTERA_MINE_EXPLODE");
  }

  // Original SHOCK_STICK CAN_PASS_UNIT is true in WAITSTART, HIDE and
  // DYING only. This reads the exact bound actor/state without ticking AI or
  // changing the source birth's NoBrain setting, invulnerability or damage.
  bool PlainOriginalShockStickPassableR1475(Enemy actor){
   if(actor==null||!NativeTerrain||plainRouteR1258!=PlainNormalR1258||!PlainOriginalRoomScopeR1258(plainRouteR1258)||
    actor.sourceNPC!="NUI_SHOCK_STICK"||actor.kind!="shock-stick"||!enemies.Contains(actor)||plainDefinitionsR1258==null)return false;
   var profile=NpcProfile(actor);var state=NpcState(actor);
   if(profile==null||state==null||!Array.Exists(plainDefinitionsR1258,p=>ReferenceEquals(p,profile))||!Array.Exists(profile.states,s=>ReferenceEquals(s,state)))return false;
   return state.id=="SHOCK_STICK_WAITSTART"||state.id=="SHOCK_STICK_HIDE"||state.id=="SHOCK_STICK_DYING";
  }

  // All original mechanism tables are installed by the original method first.
  // Its population reset is followed by this exact existing House contract.
  public void SetPlainOriginalNpcMechanismsR1258(NpcMechanismDocument document){
   if(!PlainOriginalRoomScopeR1258(plainRouteR1258)||plainDefinitionsR1258==null)throw new InvalidOperationException("Plain definitions are not bound to the original current room");
   SetNpcMechanismDefinitions(document);
   if(plainRouteR1258==PlainNormalR1258)SetAlteraNasodPopulationRulesV18(new[]{new AlteraNasodPopulationRuleV18{
    npc=PlainHouseR1258,environmentKind="source-nasod-mouse-house",spawnNpc="NUI_NASOD_MOUSE",
    initialState="NASOD_MOUSEHOUSE_START",waitState="NASOD_MOUSEHOUSE_WAIT",readyState="NASOD_MOUSEHOUSE_WAIT_RESPAWN_READY",
    respawnState="NASOD_MOUSEHOUSE_RESPAWN",damageState="NASOD_MOUSEHOUSE_DAMAGE",dyingState="NASOD_MOUSEHOUSE_DYING",
    waitSeconds=5,sourceEmissionAnimationTime=.4f,sourceAnimationSpeed=1,emitAt=.4f,maximumLiveForBeginInclusive=3,
    ownerHpCost=500,ownerHpCostHostOnly=true,sourceKeyCode=0,
    sourceCondition="CF_NASOD_MOUSEHOUSE_RESPAWN",sourceFrameCallback="NASOD_MOUSEHOUSE_RESPAWN_FRAME_MOVE"}});
  }

  bool TickPlainOriginalMineHoldR1258(Enemy enemy,NpcDefinition p,float dt){
   return TickPlainMineOriginalWaitR1269(enemy,p,dt);
  }
  // Exact original Normal pair only. Named Author2D policies below do not prove
  // native manager arithmetic, ACTIVE scheduling, timer or query semantics.
  const string PlainPairFireMachineR1258="NUI_BARRIERMACHINE_FIRE",PlainPairIceMachineR1258="NUI_BARRIERMACHINE_ICE";
  const string PlainPairFireR1258="NUI_NASOD_ELEMENT_FIRE",PlainPairIceR1258="NUI_NASOD_ELEMENT_ICE";
  const string PlainPairCaptureSHAR1258="b9b8279a387bd2bf73d87ae54655c53a50a6835eee01df944e9618e2fcd645aa";
  const string PlainPairPercentBitsR1258="666666666666f63f";
  const double PlainPairOriginalPercentR1258=1.4;
  const int PlainPairPhysicalFactorR1258=4,PlainPairMagicFactorR1258=6,PlainPairPhysicalTemplateR1258=10,PlainPairMagicTemplateR1258=12;
  const int PlainPairOriginalBctPercentR1258=3,PlainPairOriginalBrtEndR1258=7,PlainPairOriginalBftTimeR1258=1,PlainPairOriginalBdttNormalTimeR1258=1;
  const float PlainPairOriginalNormalTimeR1258=30;
  public string PlainOriginalPairPolicyR1258 { get { return "Author2D:exactRoomAliveUniqueUID;localLivingPlayersAND_USER_GEQ2;actorConsumedDt30_applyOnce_noRefresh;normalizedD100_AB60_channelPercent1.4_onceRound;safePacketFlush_opaquePoint1;REBOOT_oneCrossing.01_plus.15Max_capMax;noCOMMON"; } }
  public bool PlainOriginalPairNativeEquivalentR1258 { get { return false; } }
  // Missing bone/particle/damage presentation is an independent explicit hold.
  public bool PlainOriginalPairPresentationCompleteR1258 { get { return false; } }
  public bool PlainOriginalPairManagedReadyR1258 { get { return plainPairBindingR1258!=null; } }
  public int PlainOriginalPairPendingRequestsR1258 { get { return plainPairRequestsR1258.Count; } }
  public int PlainOriginalPairSpawnCountR1258 { get { return plainPairSpawnCountR1258; } }
  public int PlainOriginalPairPhysicalStatusCountR1258 { get { int n=0;foreach(var s in plainPairStatusesR1258.Values)if(s.physicalRemaining>0)n++;return n; } }
  public int PlainOriginalPairMagicStatusCountR1258 { get { int n=0;foreach(var s in plainPairStatusesR1258.Values)if(s.magicRemaining>0)n++;return n; } }
  sealed class PlainPairBindingR1258 {
   public NpcDefinition fireMachine,iceMachine,fire,ice;
   public RoomDefinition publishedRoom;
   public EnemySpawn fireTemplate,iceTemplate;
   public int epoch;
  }
  sealed class PlainPairMachineR1258 {
   public Enemy actor;public NpcDefinition profile;public string prefix,ownNPC,otherNPC;
   public bool flag1,flag2,flag3,rebootEdge,transitioning;public int int0,entrySerial,queuedEntry=-1;
  }
  sealed class PlainPairStatusR1258 {
   public bool flag0;public float physicalRemaining,magicRemaining;
  }
  sealed class PlainPairRequestR1258 {
   public PlainPairMachineR1258 machine;public EnemySpawn template;public RoomDefinition room;
   public int epoch,entrySerial,marker;public string ownNPC;public float x,y;public int line;
   // Original PushCreateNPCReq_Lua: ownID,GetHardLevel(),true,pos,false,.1,true,0.
   // .1 is retained as original opaque data, never a due-time or new timer.
   public readonly double originalOpaqueArgument=.1;
  }
  PlainPairBindingR1258 plainPairBindingR1258;
  readonly Dictionary<Enemy,PlainPairMachineR1258> plainPairMachinesR1258=new Dictionary<Enemy,PlainPairMachineR1258>();
  readonly Dictionary<Enemy,PlainPairStatusR1258> plainPairStatusesR1258=new Dictionary<Enemy,PlainPairStatusR1258>();
  readonly List<PlainPairRequestR1258> plainPairRequestsR1258=new List<PlainPairRequestR1258>();
  int plainPairEpochR1258,plainPairSpawnCountR1258;bool plainPairFlushingR1258;

  static bool PlainPairMachineIDR1258(string id){return id==PlainPairFireMachineR1258||id==PlainPairIceMachineR1258;}
  static bool PlainPairElementIDR1258(string id){return id==PlainPairFireR1258||id==PlainPairIceR1258;}
  NpcDefinition PlainPairDefinitionR1258(string id){NpcDefinition p;return npcDefinitions.TryGetValue(id,out p)?p:null;}
  void BindPlainOriginalPairR1258(string route,string rulesSHA,RoomDefinition[] staged){
   UnbindPlainOriginalPairR1258();
   if(route==PlainRecycleR1258)return;
   if(route!=PlainNormalR1258||rulesSHA!=PlainRegionalRulesSHAR1258||plainRouteR1258!=route||plainDefinitionsR1258==null||staged==null||staged.Length!=3||staged[2]==null||staged[2].id!=route+"-2")throw new ArgumentException("Exact original Normal pair binding required");
   if(BitConverter.DoubleToInt64Bits(PlainPairOriginalPercentR1258)!=0x3ff6666666666666L||PlainPairPercentBitsR1258!="666666666666f63f"||PlainPairOriginalBctPercentR1258!=3||PlainPairOriginalBrtEndR1258!=7||PlainPairOriginalBftTimeR1258!=1||PlainPairOriginalBdttNormalTimeR1258!=1)throw new InvalidOperationException("Changed original Plain pair factor contract");
   var b=new PlainPairBindingR1258{fireMachine=PlainPairDefinitionR1258(PlainPairFireMachineR1258),iceMachine=PlainPairDefinitionR1258(PlainPairIceMachineR1258),fire=PlainPairDefinitionR1258(PlainPairFireR1258),ice=PlainPairDefinitionR1258(PlainPairIceR1258)};
   foreach(var p in new[]{b.fireMachine,b.iceMachine,b.fire,b.ice})if(p==null||!Array.Exists(plainDefinitionsR1258,x=>ReferenceEquals(x,p)))throw new ArgumentException("Unbound original Plain pair profile reference");
   foreach(var p in new[]{b.fireMachine,b.iceMachine}){
    string prefix=p.id==PlainPairFireMachineR1258?"BARRIER_CREATEMACHINE_FIRE_":"BARRIER_CREATEMACHINE_ICE_";
    if(p.states==null||p.states.Length!=8||p.initialState!=prefix+"START")throw new ArgumentException("Changed original eight-state machine");
    foreach(string suffix in new[]{"START","WAIT","DEFENCE_UP","RESPAWN","SHOTDOWN","REBOOT","DAMAGE_SMALL","DYING"}){
     NpcStateDefinition s;if(!npcStates.TryGetValue(p.id+"/"+prefix+suffix,out s)||!Array.Exists(p.states,x=>ReferenceEquals(x,s)))throw new ArgumentException("Unbound original machine state reference");
     if(suffix=="START"&&s.duration!=1||suffix=="WAIT"&&s.duration!=10||suffix=="SHOTDOWN"&&(s.duration!=20||s.invincibleStart!=0||s.invincibleEnd!=100))throw new ArgumentException("Changed original machine timing/protection");
    }
   }
   foreach(string id in new[]{PlainPairFireMachineR1258,PlainPairIceMachineR1258,PlainPairFireR1258,PlainPairIceR1258}){
    var rows=staged[2].enemies.FindAll(e=>e.sourceNPC==id);
    if(rows.Count!=1||rows[0].aiEnabled!=PlainPairElementIDR1258(id)||rows[0].countsForClear!=PlainPairElementIDR1258(id)||rows[0].passive||rows[0].dormant||rows[0].isGate||rows[0].hp<=0)throw new ArgumentException("Changed exact original pair birth role");
   }
   plainPairBindingR1258=b;
  }
  void UnbindPlainOriginalPairR1258(){ClearPlainPairRoomR1258();plainPairBindingR1258=null;plainPairSpawnCountR1258=0;}
  void ClearPlainPairRoomR1258(){plainPairRequestsR1258.Clear();plainPairStatusesR1258.Clear();plainPairMachinesR1258.Clear();plainPairEpochR1258++;if(plainPairBindingR1258!=null){plainPairBindingR1258.publishedRoom=null;plainPairBindingR1258.fireTemplate=plainPairBindingR1258.iceTemplate=null;}}
  bool PlainPairRoomScopeR1258(){
   var b=plainPairBindingR1258;
   return b!=null&&plainRouteR1258==PlainNormalR1258&&RoomCount==3&&room==2&&CurrentRoom.id==PlainNormalR1258+"-2"&&ReferenceEquals(b.publishedRoom,CurrentRoom)&&b.epoch==plainPairEpochR1258&&ReferenceEquals(b.fireMachine,PlainPairDefinitionR1258(PlainPairFireMachineR1258))&&ReferenceEquals(b.iceMachine,PlainPairDefinitionR1258(PlainPairIceMachineR1258))&&ReferenceEquals(b.fire,PlainPairDefinitionR1258(PlainPairFireR1258))&&ReferenceEquals(b.ice,PlainPairDefinitionR1258(PlainPairIceR1258));
  }
  void BeginPlainPairRoomR1258(){
   var b=plainPairBindingR1258;if(b==null||plainRouteR1258!=PlainNormalR1258||room!=2)return;
   if(CurrentRoom.id!=PlainNormalR1258+"-2")throw new InvalidOperationException("Changed published pair room");
   b.publishedRoom=CurrentRoom;b.epoch=plainPairEpochR1258;
   b.fireTemplate=PlainPairPublishedTemplateR1258(PlainPairFireR1258);b.iceTemplate=PlainPairPublishedTemplateR1258(PlainPairIceR1258);
   foreach(var p in new[]{b.fireMachine,b.iceMachine}){
    var actors=enemies.FindAll(e=>e.sourceNPC==p.id);if(actors.Count!=1)throw new InvalidOperationException("Unique original pair machine required");
    var e=actors[0];if(e.aiEnabled||e.countsForClear||e.sourceKeyCode!=PlainPairPublishedTemplateR1258(p.id).sourceKeyCode)throw new InvalidOperationException("Changed published machine role");
    bool fire=p.id==PlainPairFireMachineR1258;
    plainPairMachinesR1258.Add(e,new PlainPairMachineR1258{actor=e,profile=p,prefix=fire?"BARRIER_CREATEMACHINE_FIRE_":"BARRIER_CREATEMACHINE_ICE_",ownNPC=fire?PlainPairFireR1258:PlainPairIceR1258,otherNPC=fire?PlainPairIceR1258:PlainPairFireR1258});
   }
  }
  EnemySpawn PlainPairPublishedTemplateR1258(string id){
   var rows=CurrentRoom.enemies.FindAll(s=>s.sourceNPC==id);if(rows.Count!=1)throw new InvalidOperationException("Unique published original pair template required");return rows[0];
  }
  PlainPairMachineR1258 PlainPairMachineActorR1258(Enemy e){PlainPairMachineR1258 m;return e!=null&&PlainPairRoomScopeR1258()&&plainPairMachinesR1258.TryGetValue(e,out m)&&ReferenceEquals(m.profile,NpcProfile(e))?m:null;}
  bool IsPlainPairManagedMachineR1258(Enemy e){return PlainPairMachineActorR1258(e)!=null;}
  // Author2D UID policy: only actual current-room, active, nondormant, live actors.
  // Original search radius999999 has no restricting effect in this finite room.
  Enemy PlainPairUniqueLiveElementR1258(string id){
   Enemy found=null;
   foreach(var e in enemies)if(e.sourceNPC==id&&e.active&&!e.dormant&&e.hp>0){
    if(!ReferenceEquals(NpcProfile(e),id==PlainPairFireR1258?plainPairBindingR1258.fire:plainPairBindingR1258.ice)||found!=null)throw new InvalidOperationException("Ambiguous original pair live UID");found=e;
   }
   return found;
  }
  void PlainPairOnStateExitedR1258(Enemy e,NpcStateDefinition state){
   var m=PlainPairMachineActorR1258(e);if(m==null||state==null)return;
   if(state.id==m.prefix+"START"){e.hp=e.maxHp;m.flag1=false;m.flag2=false;m.int0=0;m.flag3=true;TraceNpc(e,"plain-pair-start-end");}
  }
  void PlainPairOnStateEnteredR1258(Enemy e,NpcStateDefinition state){
   var m=PlainPairMachineActorR1258(e);if(m==null)return;
   if(state==null||!Array.Exists(m.profile.states,s=>ReferenceEquals(s,state)))throw new InvalidOperationException("Foreign state on managed original pair machine");
   m.entrySerial++;m.rebootEdge=false;
   if(state.id==m.prefix+"REBOOT")m.flag1=true;
   else if(state.id==m.prefix+"SHOTDOWN"){PlainPairDefenceDownR1258(m);m.flag3=false;TraceNpc(e,"plain-pair-shotdown-entry");}
   else if(state.id==m.prefix+"DEFENCE_UP")PlainPairDefenceUpR1258(m);
   else if(state.id==m.prefix+"RESPAWN")QueuePlainPairRebirthR1258(m);
  }
  void PlainPairTransitionR1258(PlainPairMachineR1258 m,string suffix){
   if(m.transitioning)throw new InvalidOperationException("Reentrant original pair transition");
   NpcStateDefinition state;if(!npcStates.TryGetValue(m.profile.id+"/"+m.prefix+suffix,out state))throw new InvalidOperationException("Missing exact registered pair target");
   m.transitioning=true;try{EnterNpcState(m.actor,state);}finally{m.transitioning=false;}
  }
  PlainPairStatusR1258 PlainPairStatusR1258For(Enemy e){PlainPairStatusR1258 s;if(!plainPairStatusesR1258.TryGetValue(e,out s)){s=new PlainPairStatusR1258();plainPairStatusesR1258.Add(e,s);}return s;}
  void PlainPairDefenceUpR1258(PlainPairMachineR1258 m){
   var e=PlainPairUniqueLiveElementR1258(m.ownNPC);if(e==null)return;var s=PlainPairStatusR1258For(e);
   if(!s.flag0){s.physicalRemaining=s.magicRemaining=PlainPairOriginalNormalTimeR1258;s.flag0=true;TraceNpc(e,"plain-pair-factor4+6-template10+12");}
   if(m.flag2){e.hp=e.maxHp*(m.int0==0?.3f:.15f);if(m.int0==0)m.int0=1;m.flag2=false;TraceNpc(e,"plain-pair-rebirth-hp");}
  }
  void PlainPairDefenceDownR1258(PlainPairMachineR1258 m){
   var e=PlainPairUniqueLiveElementR1258(m.ownNPC);if(e==null)return;PlainPairStatusR1258 s;
   if(plainPairStatusesR1258.TryGetValue(e,out s)){s.physicalRemaining=s.magicRemaining=0;s.flag0=false;TraceNpc(e,"plain-pair-erase-template10+12");}
  }
  void TickPlainPairStatusAuthor2DR1258(Enemy e,float consumedDt){
   PlainPairStatusR1258 s;if(!plainPairStatusesR1258.TryGetValue(e,out s))return;
   s.physicalRemaining=Math.Max(0,s.physicalRemaining-consumedDt);s.magicRemaining=Math.Max(0,s.magicRemaining-consumedDt);
   // Native refresh/stack policy is not proved. Original Flag0 is NOT reset by
   // expiration: only the explicitly registered DEFENCE_DOWN does that.
  }
  bool TryPlainPairDefenceAuthor2DR1258(Enemy e,int damage,bool magic,bool armorBreak,out int result){
   result=damage;PlainPairStatusR1258 s;
   if(!PlainPairRoomScopeR1258()||e==null||!PlainPairElementIDR1258(e.sourceNPC)||!plainPairStatusesR1258.TryGetValue(e,out s)||(magic?s.magicRemaining:s.physicalRemaining)<=0)return false;
   double normalizedDefence=100.0*(armorBreak&&!magic?.6:1.0)*PlainPairOriginalPercentR1258;
   result=Math.Max(1,(int)Math.Round(damage*200.0/(100.0+normalizedDefence)));return true;
  }
  static bool PlainPairFrameHasDamageClampR1258(PlainPairMachineR1258 m){
   string s=m.actor.npcState;return s==m.prefix+"START"||s==m.prefix+"WAIT"||s==m.prefix+"DAMAGE_SMALL"||s==m.prefix+"REBOOT"||s==m.prefix+"RESPAWN";
  }
  void PlainPairDamageFloorR1258(PlainPairMachineR1258 m){if(m.actor.hp*100<=m.actor.maxHp*30)m.actor.hp=m.actor.maxHp*.3f;}
  bool PlainPairAfterAttackDamageR1258(Enemy e,int actual,bool reaction){
   if(!PlainPairRoomScopeR1258()||e==null||actual<=0)return false;
   if(PlainPairElementIDR1258(e.sourceNPC)){if(e.hp<=0)plainPairStatusesR1258.Remove(e);return false;}
   var m=PlainPairMachineActorR1258(e);if(m==null)return false;
   // Finite packet policy: registered GetDamagedThisFrame clamp executes before
   // generic death/reaction. Damage reaction uses original INIT_STATE mapping.
   if(PlainPairFrameHasDamageClampR1258(m)||reaction)PlainPairDamageFloorR1258(m);
   if(e.hp<=0)return false;
   if(reaction)PlainPairTransitionR1258(m,"DAMAGE_SMALL");
   e.state=CombatState.Normal;e.stun=0;e.knockdownOnLanding=false;return true;
  }
  void PlainPairAfterBurnResourceDeltaR1258(Enemy e,int actual){
   if(!PlainPairRoomScopeR1258()||e==null||actual<=0)return;
   if(PlainPairElementIDR1258(e.sourceNPC)){if(e.hp<=0)plainPairStatusesR1258.Remove(e);return;}
   var m=PlainPairMachineActorR1258(e);if(m!=null&&PlainPairFrameHasDamageClampR1258(m))PlainPairDamageFloorR1258(m);
   // No defence, combo, hit reaction or stun-hit accounting is added to Burn.
  }
  bool PlainPairInterruptR1258(Enemy e){
   var m=PlainPairMachineActorR1258(e);if(m==null)return false;
   if(e.hp<=0){plainPairMachinesR1258.Remove(e);return false;}
   if(!m.transitioning&&!EnemyHasSuperArmor(e))PlainPairTransitionR1258(m,"DAMAGE_SMALL");return true;
  }
  // Named Author2D AND policy for original CT_REBOOT fields:
  // USER_GEQ=2,STATE_TIME_OVER=20,EVENT_INTERVAL_ID=0. Local living users are0/1.
  // Therefore this gate does not naturally open in this single-player model.
  bool PlainPairRebootConditionAuthor2DR1258(Enemy e){int livingUsers=player.hp>0?1:0;return livingUsers>=2&&e.npcTime>=20;}
  bool TickPlainPairManagedR1258(Enemy e,float consumedDt){
   if(!PlainPairRoomScopeR1258())return false;
   TickPlainPairStatusAuthor2DR1258(e,consumedDt);
   var m=PlainPairMachineActorR1258(e);if(m==null)return false;
   var state=NpcState(e);if(state==null){InitializeNpcBirthStateV26(e);state=NpcState(e);}
   if(state==null||!Array.Exists(m.profile.states,s=>ReferenceEquals(s,state)))throw new InvalidOperationException("Missing original managed pair state");
   float from=e.npcTime,to=from+consumedDt;e.npcTime=to;
   AttachNpcEffectSets(e,state,from,to,elapsed-consumedDt);
   if(state.loop)AdvanceNpcLoopSounds(e,state,from,consumedDt,elapsed-consumedDt);else AdvanceNpcStateSounds(e,state,from,to,elapsed-consumedDt);
   if(state.id==m.prefix+"START"){if(to>=1)PlainPairTransitionR1258(m,"WAIT");}
   else if(state.id==m.prefix+"WAIT"){
    if(e.hp*100<=e.maxHp*31)PlainPairTransitionR1258(m,"SHOTDOWN");
    else if(PlainPairUniqueLiveElementR1258(m.ownNPC)!=null||PlainPairUniqueLiveElementR1258(m.otherNPC)==null)PlainPairTransitionR1258(m,"DEFENCE_UP");
    else if(to>=10)PlainPairTransitionR1258(m,"RESPAWN");
   }else if(state.id==m.prefix+"SHOTDOWN"){
    if(PlainPairRebootConditionAuthor2DR1258(e))PlainPairTransitionR1258(m,"REBOOT");
   }else if(state.id==m.prefix+"REBOOT"){
    // Original registered proto8 PC28(.01),33..45: HP+=MaxHP*.15.
    // Chosen one-crossing per entry and Max cap are finite Author2D policies.
    if(!m.rebootEdge&&from<.01f&&to>=.01f){m.rebootEdge=true;RecoverEnemyHealth(e,e.maxHp*.15f,15);TraceNpc(e,"plain-pair-reboot-one-edge");}
    if(e.hp*100<=e.maxHp*31)PlainPairTransitionR1258(m,"SHOTDOWN");
    else if(e.hp*100>e.maxHp*95){e.hp=e.maxHp;PlainPairTransitionR1258(m,"START");}
   }else if(state.id==m.prefix+"DAMAGE_SMALL"){
    if(e.hp*100<=e.maxHp*31)PlainPairTransitionR1258(m,"SHOTDOWN");
    else if(m.flag1)PlainPairTransitionR1258(m,"REBOOT");
    else if(to>=state.duration)PlainPairTransitionR1258(m,"WAIT");
   }else if((state.id==m.prefix+"DEFENCE_UP"||state.id==m.prefix+"RESPAWN")&&to>=state.duration)PlainPairTransitionR1258(m,"WAIT");
   return true;
  }
  void QueuePlainPairRebirthR1258(PlainPairMachineR1258 m){
   if(m.queuedEntry==m.entrySerial)return;
   if(PlainPairUniqueLiveElementR1258(m.ownNPC)!=null||PlainPairUniqueLiveElementR1258(m.otherNPC)==null)return;
   foreach(var q in plainPairRequestsR1258)if(ReferenceEquals(q.machine,m))throw new InvalidOperationException("Duplicate outstanding pair source request");
   if(plainPairRequestsR1258.Count>=2)throw new InvalidOperationException("Pair pending packet bound exceeded");
   bool fire=m.ownNPC==PlainPairFireR1258;var b=plainPairBindingR1258;
   m.queuedEntry=m.entrySerial;
   plainPairRequestsR1258.Add(new PlainPairRequestR1258{machine=m,template=fire?b.fireTemplate:b.iceTemplate,room=CurrentRoom,epoch=plainPairEpochR1258,entrySerial=m.entrySerial,marker=fire?5:3,ownNPC=m.ownNPC,x=fire?382f:298f,y=54f,line=1});
   TraceNpc(m.actor,"plain-pair-request-marker"+(fire?"5":"3"));
  }
  void FlushPlainPairRequestsR1258(){
   if(!PlainPairRoomScopeR1258()||plainPairFlushingR1258)return;
   plainPairFlushingR1258=true;
   try{
    foreach(var dead in new List<Enemy>(plainPairStatusesR1258.Keys))if(dead.hp<=0||!enemies.Contains(dead))plainPairStatusesR1258.Remove(dead);
    for(int i=0;i<plainPairRequestsR1258.Count;){
     var q=plainPairRequestsR1258[i];plainPairRequestsR1258.RemoveAt(i);
     if(q.epoch!=plainPairEpochR1258||!ReferenceEquals(q.room,CurrentRoom)||!enemies.Contains(q.machine.actor))continue;
     // The original guard was true when Push+Flush was requested. Commit that
     // request even if the peer/owner dies later in this packet: no invented
     // simultaneous-kill cancellation window. Room ownership alone cancels it.
     if(PlainPairUniqueLiveElementR1258(q.ownNPC)!=null)throw new InvalidOperationException("Committed original pair request conflicts with an existing own Element");
     var t=q.template;if(!ReferenceEquals(t,PlainPairPublishedTemplateR1258(q.ownNPC))||t.groundLine!=q.line||!t.aiEnabled||!t.countsForClear||q.originalOpaqueArgument!=.1)throw new InvalidOperationException("Changed published original pair recreation template");
     float hp=t.hp;
     var created=new Enemy{kind=t.kind,x=q.x,spawnX=q.x,y=q.y,facing=t.facing,groundLine=t.groundLine,nativeDepth=t.nativeDepth,visualScale=t.visualScale,aiEnabled=t.aiEnabled,spawnAfterDeath=t.spawnAfterDeath,spawnDelay=t.spawnDelay,hp=hp,maxHp=hp,active=true,aggroRange=t.aggroRange,passive=t.passive,dormant=t.dormant,cooldown=1,state=q.y>BelowNpcOwnFloorV21(t.sourceNPC,q.x,q.y,false)+.01f?CombatState.Airborne:CombatState.Normal,sourceNPC=t.sourceNPC,sourceKeyCode=t.sourceKeyCode,encounterIndex=t.encounterIndex,countsForClear=t.countsForClear,isGate=t.isGate};
     ApplyDungeonBossHealth(created);enemies.Add(created);InitializeNpcBirthStateV26(created);q.machine.flag2=true;plainPairSpawnCountR1258++;TraceNpc(created,"plain-pair-recreated");
    }
   }finally{plainPairFlushingR1258=false;}
  }

 }
}
