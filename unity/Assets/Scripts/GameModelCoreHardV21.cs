using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace ElswordPixel {
 [Serializable] public sealed class CoreHardActorV21 {public string npc,startState,waitState,dyingState,damageState;public bool sourceAIActivationChanged;}
 [Serializable] public sealed class CoreHardTransitionV21 {public string kind,target,condition;public float after;}
 [Serializable] public sealed class CoreHardStateV21 {public string npc,state;public float duration,sourceAnimSpeed;public bool loop,hasSourceShow,sourceShow,dyingEnd;public CoreHardTransitionV21[] transitions;}
 [Serializable] public sealed class CoreHardBirthV21 {public int room,encounter,order,sourceKeyCode,sourceSpawnIndex,sourceBossGaugeLines;public string npc;public bool sourceActive,sourceStopAtStart,aiEnabled,sourceBossGauge;public float sourceNameDelay,selectedFixtureHp;}
 [Serializable] public sealed class CoreHardNodeV21 {public int room,encounter;public string id,world;}
 [Serializable] public sealed class CoreHardMechanismDocumentV21 {
  public string schema,sourceDifficulty,dungeonId,difficultyEnum,archive,member,memberSha256,sourceRulesSha256,sourceModelSha256,sourceContractSha256,balancePolicy,lookupPolicy,gameLevelPolicy,terminalPolicy;
  public int difficultyValue,npcLevel,ordinal,sourceBossGaugeLines;public float powerPlantUpSecondsHard,pdFadeAt,pdSetHpAt,sourceBossNameDelay,sourceRespawnDelay;
  public bool runtimeAdmitted,nativeExecuted,nativeStatBound,respawnNativeArgsProven,directorNativeFinishProven;
  public CoreHardActorV21[] actors;public CoreHardStateV21[] states;public CoreHardBirthV21[] births;public CoreHardNodeV21[] nodes;
 }
 [Serializable] public sealed class CoreHardEventV21 {public int sequence,room,encounter,actorSerial,targetSerial;public string kind,npc,source,state,target,template,hold;public float time,sourceTime,value,x,y,z;}
 [Serializable] public sealed class CoreHardCreateRequestV21 {public string npc,callback,hardLevelArgument="owner.GetHardLevel() UNRESOLVED",nativeArgs="hardLevel,false,startPosition,true,0.3,true,0";public int sourceStartIndex;public float delay=.3f,due;public bool nativeArgsProven=false;}
 public sealed partial class GameModel {
  CoreHardMechanismDocumentV21 coreHardV21;readonly Dictionary<Enemy,CoreHardRuntimeV21> coreHardActorsV21=new Dictionary<Enemy,CoreHardRuntimeV21>();int coreHardSequenceV21;
  sealed class CoreHardRuntimeV21 {internal string last;internal bool initialized,shown=true,callbackGauge,up;internal float openedAt;internal readonly HashSet<string> issued=new HashSet<string>();}
  public readonly List<CoreHardEventV21> coreHardEventsV21=new List<CoreHardEventV21>();public readonly List<CoreHardCreateRequestV21> coreHardCreateRequestsV21=new List<CoreHardCreateRequestV21>();
  public bool CoreHardConfiguredV21 {get{return coreHardV21!=null;}}public int CoreHardSourceNpcLevelV21 {get{return coreHardV21==null?0:coreHardV21.npcLevel;}}
  static bool IsCoreHardZeroBodyV21(NpcDefinition p){if(p==null||p.bodyHeight!=0||p.bodySpheres==null||p.bodySpheres.Length!=0)return false;if(p.id=="NUI_EVENT_NPC_ALTERA_CORE_BOSS")return p.bodyWidth==0;return(p.id=="NUI_PD_MONSTER"||p.id=="NUI_ALARM_SYSTEM_A"||p.id=="NUI_ALARM_SYSTEM_B"||p.id=="NUI_ALARM_SYSTEM_C"||p.id=="NUI_ALARM_SYSTEM_C_MOVE")&&Math.Abs(p.bodyWidth-14)<.0001f;}
  static bool IsCoreHardPassiveRangesV21(NpcDefinition p){return p!=null&&(p.id=="NUI_EL_POWER_PLANT"&&p.senseRange==0&&p.lostRange==0||((p.id=="NUI_EL_EXTRACTOR_NORMAL_A"||p.id=="NUI_EL_EXTRACTOR_NORMAL_B")&&Math.Abs(p.senseRange-420)<.0001f&&p.lostRange==0));}
  public int CoreHardSourceBossGaugeLinesV21 {get{return coreHardV21==null?0:coreHardV21.sourceBossGaugeLines;}}
  const string CoreHardExpectedReceiptV21="521c35c1c08b4da821b853340fb8cc0f69941f4c91df32ad55fb8d598572bc9f";
  static string CoreHardBitsV21(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0).ToString("X8");}
  static string CoreHardBoolV21(bool v){return v?"1":"0";}
  static string CoreHardReceiptV21(CoreHardMechanismDocumentV21 d){var s=new List<string>{d.schema,d.sourceDifficulty,d.dungeonId,d.difficultyEnum,d.archive,d.member,d.memberSha256,d.sourceRulesSha256,d.sourceModelSha256,d.sourceContractSha256,d.balancePolicy,d.lookupPolicy,d.gameLevelPolicy,d.terminalPolicy,d.difficultyValue.ToString(),d.npcLevel.ToString(),d.ordinal.ToString(),CoreHardBitsV21(d.powerPlantUpSecondsHard),CoreHardBitsV21(d.pdFadeAt),CoreHardBitsV21(d.pdSetHpAt),d.sourceBossGaugeLines.ToString(),CoreHardBitsV21(d.sourceBossNameDelay),CoreHardBitsV21(d.sourceRespawnDelay)};
   foreach(var a in d.actors)s.AddRange(new[]{a.npc,a.startState,a.waitState,a.dyingState,a.damageState,CoreHardBoolV21(a.sourceAIActivationChanged)});
   foreach(var r in d.states){s.AddRange(new[]{r.npc,r.state,CoreHardBitsV21(r.duration),CoreHardBitsV21(r.sourceAnimSpeed),CoreHardBoolV21(r.loop),CoreHardBoolV21(r.hasSourceShow),CoreHardBoolV21(r.sourceShow),CoreHardBoolV21(r.dyingEnd)});foreach(var t in r.transitions)s.AddRange(new[]{t.kind,t.target,t.condition,CoreHardBitsV21(t.after)});}
   foreach(var b in d.births)s.AddRange(new[]{b.room.ToString(),b.encounter.ToString(),b.order.ToString(),b.npc,b.sourceKeyCode.ToString(),CoreHardBoolV21(b.sourceActive),CoreHardBoolV21(b.sourceStopAtStart),CoreHardBoolV21(b.aiEnabled),b.sourceSpawnIndex.ToString(),b.sourceBossGaugeLines.ToString(),CoreHardBoolV21(b.sourceBossGauge),CoreHardBitsV21(b.sourceNameDelay),CoreHardBitsV21(b.selectedFixtureHp)});
   foreach(var n in d.nodes)s.AddRange(new[]{n.room.ToString(),n.encounter.ToString(),n.id,n.world});using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",s.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();
  }
  public void SetCoreHardMechanismsV21(CoreHardMechanismDocumentV21 d){
   if(d!=null){if(d.schema!="core-hard-source-mechanisms-v21"||d.sourceDifficulty!="Hard"||d.dungeonId!="DI_ALTERA_CORE_HARD"||d.difficultyEnum!="DIFFICULTY_LEVEL.DL_HARD"||d.difficultyValue!=1||d.npcLevel!=37||d.archive!="data035.kom"||d.member!="Dungeon_Altera_Core_Hard.lua"||d.ordinal!=8||d.memberSha256!="27d9bed812f9b4690f310faf1cbfa2ea57a27edbaf79cda2c6bccc3ec15e3186"||d.runtimeAdmitted||d.nativeExecuted||d.nativeStatBound||d.respawnNativeArgsProven||d.directorNativeFinishProven||d.actors==null||d.actors.Length!=8||d.states==null||d.states.Length!=53||d.births==null||d.births.Length!=85||d.nodes==null||d.nodes.Length!=8||CoreHardReceiptV21(d)!=CoreHardExpectedReceiptV21)throw new ArgumentException("Changed exact Core Hard source identity/contract or unproven native claim");
    foreach(var r in d.states){NpcStateDefinition state;if(!npcStates.TryGetValue(r.npc+"/"+r.state,out state)||Math.Abs(state.duration-r.duration)>.000001f||state.sourceAnimSpeed!=r.sourceAnimSpeed)throw new ArgumentException("Changed Core Hard source state clock");}
   }coreHardV21=d;ClearCoreHardV21();
  }
  void ClearCoreHardV21(){ClearCoreHardEntryActivationV21();ClearCoreHardClosureV21();ClearCoreHardLocalBalanceV21();ClearCoreHardCallbacksV21();ClearCoreHardBattlePlaneV21();ClearCoreHardProtectionV21();coreHardActorsV21.Clear();coreHardDisplayHitsV39.Clear();coreHardEventsV21.Clear();coreHardCreateRequestsV21.Clear();coreHardSequenceV21=0;ClearCoreHardSupportRulesR1417();}
  void BeginCoreHardRoomV21(){if(coreHardV21==null)return;ClearCoreHardClosureRoomV21();ClearCoreHardBalanceRoomV21();ClearCoreHardCallbackRoomV21();ClearCoreHardBattlePlaneRoomV21();coreHardActorsV21.Clear();coreHardDisplayHitsV39.Clear();coreHardCreateRequestsV21.Clear();if(CurrentRoom.id!="Dungeon_Altera_Core_Hard-"+room)throw new InvalidOperationException("Core Hard contract cannot alias another dungeon/mode");var node=Array.Find(coreHardV21.nodes,n=>n.room==room&&n.encounter==EncounterIndex);if(node==null||CurrentEncounter==null||CurrentEncounter.id!=node.id||CurrentEncounter.sourceWorldId!=node.world)throw new InvalidOperationException("Changed exact Core Hard route node");ApplyCoreHardInitialBalanceV21();BeginCoreHardEntryActivationRoomV21();}
  CoreHardActorV21 CoreHardActorRuleV21(Enemy e){return coreHardV21==null||e==null?null:Array.Find(coreHardV21.actors,a=>a.npc==e.sourceNPC);}
  CoreHardRuntimeV21 CoreHardActorRuntimeV21(Enemy e){CoreHardRuntimeV21 r;if(!coreHardActorsV21.TryGetValue(e,out r)){r=new CoreHardRuntimeV21();coreHardActorsV21[e]=r;}return r;}
  void CoreHardEventV21(string kind,Enemy e,string source,float at,float sourceTime=0,float value=0,string template=null,Enemy target=null,string hold=null){if(coreHardEventsV21.Count>=4096)coreHardEventsV21.RemoveAt(0);coreHardEventsV21.Add(new CoreHardEventV21{sequence=++coreHardSequenceV21,room=room,encounter=EncounterIndex,kind=kind,npc=e==null?null:e.sourceNPC,actorSerial=e==null?0:NpcSoundActorId(e),source=source,time=at,sourceTime=sourceTime,value=value,state=e==null?null:e.npcState,template=template,target=target==null?null:target.sourceNPC,targetSerial=target==null?0:NpcSoundActorId(target),x=e==null?0:e.x,y=e==null?0:e.y,z=e==null?0:e.nativeDepth*.28f,hold=hold});}
  bool CoreHardOnceV21(Enemy e,string key){return CoreHardActorRuntimeV21(e).issued.Add(key);}
  Enemy CoreHardNearV21(string npc,bool live=true){foreach(var e in enemies)if(e.sourceNPC==npc&&e.encounterIndex==EncounterIndex&&(!live||e.hp>0))return e;return null;}
  static bool CoreHardExtractorV21(string npc){return npc=="NUI_EL_EXTRACTOR_FIRE"||npc=="NUI_EL_EXTRACTOR_ICE"||npc=="NUI_EL_EXTRACTOR_EARTH"||npc=="NUI_EL_EXTRACTOR_NOVA";}
  // The helper exporter normalized every Plant pose against WaitStart's mesh
  // floor (-666.6604 native units). Its sprite already uses the world root.
  internal const float CorePlantHelperOriginYV59 = -666.6604f * .28f;
  internal bool CoreArenaBodyPassableV59(Enemy e){
   bool throne=e.sourceNPC=="NUI_NASOD_KING";
   if(!throne&&e.sourceNPC!="NUI_EL_POWER_PLANT"||!CoreHardClosureScopeV21())return false;
   // Its 14-wide chassis is on a separate source depth plane. Its actual
   // attacks are tested independently by the battle-plane contact consumer.
   if(throne)return true;
   // Original CAN_PASS_UNIT also covers the first tick before START is entered.
   return string.IsNullOrEmpty(e.npcState)&&!CoreHardActorRuntimeV21(e).initialized||
    e.npcState=="EL_POWER_PLANT_START"||e.npcState=="EL_POWER_PLANT_WAIT_DOWN"||
    e.npcState=="EL_POWER_PLANT_DOWN"||e.npcState=="EL_POWER_PLANT_DYING";
  }
  bool AwaitCoreKingDestructionV59(){
   if(!CoreHardClosureScopeV21())return false;
   var plant=CoreHardNearV21("NUI_EL_POWER_PLANT",false);
   if(plant==null||plant.hp>0)return false;
   // The Plant's own DYING frame callback destroys the King at 5.5 seconds.
   return plant.npcState!="EL_POWER_PLANT_DYING"||plant.npcTime<5.5f;
  }
  public bool CoreHardActorShownV21(Enemy e){return CoreHardActorRuleV21(e)==null||CoreHardActorRuntimeV21(e).shown;}
  public bool CoreHardCallbackRequestsGaugeV21(Enemy e){return CoreHardActorRuleV21(e)!=null&&CoreHardActorRuntimeV21(e).callbackGauge;}
  public bool CoreHardConditionV21(string condition){if(coreHardV21==null)return false;bool any=false;foreach(string n in new[]{"NUI_EL_EXTRACTOR_FIRE","NUI_EL_EXTRACTOR_ICE","NUI_EL_EXTRACTOR_EARTH","NUI_EL_EXTRACTOR_NOVA"})if(CoreHardNearV21(n)!=null)any=true;return condition=="CF_EL_POWER_PLANT_UP"?!any:condition=="CF_EL_POWER_PLANT_DOWN"&&(player.hp<=0||any);}
  void CoreHardEnterV21(Enemy e,string state,string source){NpcStateDefinition s;if(!npcStates.TryGetValue(e.sourceNPC+"/"+state,out s))throw new InvalidOperationException("Missing exact Core Hard source state");CoreHardEventV21("source-StateChange",e,source,elapsed,0,0,null,null);EnterNpcState(e,s);}
  void OnCoreHardStateEnteredV21(Enemy e,NpcStateDefinition state){if(CoreHardActorRuleV21(e)==null)return;coreHardDisplayHitsV39.Remove(e);OnCoreHardBattlePlaneEnteredV21(e,state);OnCoreHardCallbackEnteredV21(e,state);CoreHardClosureStateEnteredV21(e,state);var r=CoreHardActorRuntimeV21(e);r.initialized=true;r.last=state.id;r.issued.Clear();var raw=Array.Find(coreHardV21.states,s=>s.npc==e.sourceNPC&&s.state==state.id);if(raw!=null&&raw.hasSourceShow)r.shown=raw.sourceShow;
   if(CoreHardExtractorV21(e.sourceNPC)&&state.id.EndsWith("_DYING",StringComparison.Ordinal)){CoreHardEventV21("source-sound-request",e,e.sourceNPC.Substring(4)+"_DYING_LAND_STATE_START",elapsed,0,1,"Explosion.ogg",null,"Audio consumer for callback request not implemented by this phase");var templates=new[]{"Nasod_ExplodeSmoke","Nasod_ExplodeTail","ExplodeImpactRingRed","ExplodeImpactSlashRed"};var counts=new[]{40,3,1,1};for(int i=0;i<templates.Length;i++){CoreHardEventV21("source-particle-request",e,e.sourceNPC.Substring(4)+"_DYING_LAND_STATE_START",elapsed,0,counts[i],"Major/"+templates[i],null,"Native sequence override/trace rendering remains separate");var q=coreHardEventsV21[coreHardEventsV21.Count-1];q.y-=30*.28f*e.visualScale;q.z+=5*.28f*e.visualScale;}}
   if(e.sourceNPC=="NUI_NASOD_KING"){if(SimpleDangerIntroductions){r.shown=true;r.callbackGauge=true;}else{if(state.id=="NASOD_KING_START_READY"){r.shown=false;r.callbackGauge=false;}if(state.id=="NASOD_KING_START"){r.shown=true;r.callbackGauge=true;}}}
   if(e.sourceNPC=="NUI_EL_POWER_PLANT"){
    if(state.id=="EL_POWER_PLANT_START"||state.id=="EL_POWER_PLANT_DOWN")r.up=false;
    if(state.id=="EL_POWER_PLANT_UP"){r.up=true;r.openedAt=(int)elapsed;var king=CoreHardNearV21("NUI_NASOD_KING");if(king!=null)CoreHardEnterV21(king,"NASOD_KING_KNOCKDOWN","EL_POWER_PLANT_UP_STATE_START");CoreHardEventV21("source-SetFlag0-true-SetInt0-GameTime",e,"EL_POWER_PLANT_UP_STATE_START",elapsed,0,r.openedAt);}
    if(state.id=="EL_POWER_PLANT_DOWN")CoreHardQueueRespawnV21(e);
    if(state.id=="EL_POWER_PLANT_DYING"){r.up=false;CoreHardEventV21("HOLD-presentation-original-DieLight",e,"EL_POWER_PLANT_DYING_STATE_START",elapsed,0,3,"Minor/DieLight",null,"Original minor particle/land/die-seq consumer not in this model phase");var king=CoreHardNearV21("NUI_NASOD_KING");if(king!=null)CoreHardEnterV21(king,"NASOD_KING_KNOCKDOWN_WAIT","EL_POWER_PLANT_DYING_STATE_START");}
   }
  }
  void CoreHardQueueRespawnV21(Enemy plant){int a=(int)(NpcRoll()*100)%3,b=(a*11)%3;int l1=a==1?10:9,l2=a==2?10:11,r1=b==1?21:20,r2=b==2?21:22;var names=new[]{"NUI_EL_EXTRACTOR_FIRE","NUI_EL_EXTRACTOR_EARTH","NUI_EL_EXTRACTOR_NOVA","NUI_EL_EXTRACTOR_ICE"};var starts=new[]{l1,l2,r1,r2};for(int i=0;i<names.Length;i++)if(CoreHardNearV21(names[i])==null){coreHardCreateRequestsV21.Add(new CoreHardCreateRequestV21{npc=names[i],callback="EL_POWER_PLANT_DOWN_STATE_START",sourceStartIndex=starts[i],due=elapsed+.3f});CoreHardEventV21("HOLD-PushCreateNPCReq",plant,"EL_POWER_PLANT_DOWN_STATE_START",elapsed,0,starts[i],null,null,"Native owner.GetHardLevel/PushCreateNPCReq parameter ABI and final Hard stats unbound; no replacement actor synthesized");}}
  public bool DispatchCoreHardDirectorDyingV21(string originalCallback){if(coreHardV21==null||originalCallback!="EVENT_NPC_ALTERA_CORE_BOSS_DYING_START_START"||!CoreHardDirectorPermitDispatchV21())return false;var director=CoreHardNearV21("NUI_EVENT_NPC_ALTERA_CORE_BOSS");if(director==null||director.npcState!="EVENT_NPC_ALTERA_CORE_BOSS_START")return false;CoreHardEnterV21(director,"EVENT_NPC_ALTERA_CORE_BOSS_DYING",originalCallback);var king=CoreHardNearV21("NUI_NASOD_KING");if(king!=null)CoreHardEnterV21(king,"NASOD_KING_START",originalCallback);CoreHardActorRuntimeV21(director).shown=false;CoreHardEventV21("HOLD-source-SetPositionForce",director,originalCallback,elapsed,0,-5000,null,null,"Original XYZ(0,-5000,0) retained; no projected teleport without native director coordinate binding");return true;}
  bool TickCoreHardManagedV21(Enemy e,float dt){var rule=CoreHardActorRuleV21(e);if(rule==null||!e.active||e.encounterIndex!=EncounterIndex)return false;var runtime=CoreHardActorRuntimeV21(e);
   if(SimpleDangerIntroductions&&e.sourceNPC=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"){runtime.shown=false;e.dormant=true;coreHardDirectorPlayingV21=false;coreHardDirectorCompletedV21=true;return true;}
   if(!runtime.initialized||string.IsNullOrEmpty(e.npcState)){string next=e.hp<=0?rule.dyingState:!runtime.initialized?rule.startState:rule.damageState;if(string.IsNullOrEmpty(next)){CoreHardEventV21("HOLD-empty-source-damage-state",e,"original-INIT_STATE",elapsed,0,0,null,null,"No original damage transition chosen; no fake restart/WAIT");return true;}CoreHardEnterV21(e,next,"original-initial-reaction-or-physical-death");}
   if(e.hp<=0&&e.npcState!=rule.dyingState)CoreHardEnterV21(e,rule.dyingState,"original-physical-death");
   if(e.sourceNPC=="NUI_NASOD_KING"&&e.hp>0){var plant=CoreHardNearV21("NUI_EL_POWER_PLANT");if(plant!=null&&plant.hp>0&&e.hp<e.maxHp){e.hp=e.maxHp;CoreHardEventV21("source-SetNowHP-GetMaxHP",e,"NASOD_COMMON_FRAMEMOVE",elapsed,0,e.maxHp,null,plant);}}
   var source=Array.Find(coreHardV21.states,s=>s.npc==e.sourceNPC&&s.state==e.npcState);var state=NpcState(e);if(source==null||state==null)return true;float from=e.npcTime,to=from+Math.Max(0,dt),packetStart=elapsed-dt;e.npcTime=to;e.clock+=dt;if(e.hp<=0)e.deathTime+=dt;AdvanceNpcStateSounds(e,state,from,to,packetStart);AttachNpcEffectSets(e,state,from,to,packetStart);AdvanceCoreHardBattlePlaneV21(e,state,from,to,packetStart);AdvanceCoreHardCallbacksV21(e,state,from,to,packetStart);
   if(e.sourceNPC=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"&&state.id=="EVENT_NPC_ALTERA_CORE_BOSS_START"&&!coreHardV21.directorNativeFinishProven){if(to>=source.duration&&CoreHardOnceV21(e,"director-clock-hold"))CoreHardEventV21("HOLD-native-DIRECTION_STATE-motion-end-owner",e,"source-SCT_MOTION_END",elapsed,source.duration,0,null,null,"STOP_AT_CURRENT_STATE/DIRECTION_STATE/camera finish ownership unproven; explicit original callback dispatch required");return true;}
   if(e.sourceNPC=="NUI_PD_MONSTER"&&state.id=="PD_MONSTER_WAIT"){
    if(from<7&&to>=7&&CoreHardOnceV21(e,"fade7"))CoreHardEventV21("source-particle-request",e,"PD_MONSTER_WAIT_FRAME_MOVE",packetStart+7-from,7,1,"Major/Wally_9th_Death_FadeOut",null,"Presentation consumer not implemented by mechanism phase");
    if(from<8&&to>=8&&CoreHardOnceV21(e,"hp8")){e.hp=0;CoreHardEventV21("source-SetNowHP0",e,"PD_MONSTER_WAIT_FRAME_MOVE",packetStart+8-from,8);CoreHardEnterV21(e,"PD_MONSTER_DIE","PD_MONSTER_WAIT_FRAME_MOVE");}return true;
   }
   if(e.sourceNPC=="NUI_EL_POWER_PLANT"){
    if(state.id=="EL_POWER_PLANT_DYING"){
     foreach(float at in new[]{.01f,2.5f,5.5f})if(from<at&&to>=at&&CoreHardOnceV21(e,"dying"+CoreHardBitsV21(at))){if(at==5.5f){var king=CoreHardNearV21("NUI_NASOD_KING");if(king!=null)king.hp=0;CoreHardEventV21("source-SetKingNowHP0-and-destroy-presentation",e,"EL_POWER_PLANT_DYING_FRAME_MOVE",packetStart+at-from,at,0,"Altera_core_Destroy|Altera_core_Boss_Destroy",king,"Minor particle/XMesh destroy rendering held; native D3D not executed");}else CoreHardEventV21(at==2.5f?"source-particle-request":"HOLD-terminal-presentation",e,"EL_POWER_PLANT_DYING_FRAME_MOVE",packetStart+at-from,at,0,at==2.5f?"Major/Wally_9th_Death_FadeOut":null,null,"Original camera/user-focus/destroy FX need separate presentation consumer");}return true;
    }
    if(runtime.up&&elapsed-runtime.openedAt>coreHardV21.powerPlantUpSecondsHard){CoreHardEnterV21(e,"EL_POWER_PLANT_DOWN","EL_POWER_PLANT_COMMON_FRAME_MOVE-Hard1");return true;}
    if(state.id=="EL_POWER_PLANT_WAIT_DOWN"&&CoreHardConditionV21("CF_EL_POWER_PLANT_UP")){CoreHardEnterV21(e,"EL_POWER_PLANT_UP","CF_EL_POWER_PLANT_UP");return true;}
    if(state.id=="EL_POWER_PLANT_WAIT_UP"&&CoreHardConditionV21("CF_EL_POWER_PLANT_DOWN")){CoreHardEnterV21(e,"EL_POWER_PLANT_DOWN","CF_EL_POWER_PLANT_DOWN");return true;}
    if(state.id=="EL_POWER_PLANT_DOWN"&&coreHardCreateRequestsV21.Count>0&&!coreHardV21.respawnNativeArgsProven&&!CoreHardLocalReplacementRequestsSettledV21()){if(CoreHardOnceV21(e,"respawn-closure-hold"))CoreHardEventV21("HOLD-await-original-replacement-births",e,"EL_POWER_PLANT_DOWN_STATE_START",elapsed,0,coreHardCreateRequestsV21.Count,null,null,"No native parameter/stat-bound spawn consumer; do not reopen from the absence caused by this HOLD");return true;}
   }
   if(CoreHardCallbackTransitionV21(e,state,from,to))return true;foreach(var transition in source.transitions){bool ready=transition.kind=="SCT_MOTION_END"&&!source.loop&&to>=source.duration||transition.kind=="SCT_CONDITION_TABLE"&&transition.after>=0&&to>transition.after;if(!ready)continue;if(CoreHardExtractorV21(e.sourceNPC)&&state.id.EndsWith("_START",StringComparison.Ordinal)&&(e.sourceNPC=="NUI_EL_EXTRACTOR_ICE"||e.sourceNPC=="NUI_EL_EXTRACTOR_NOVA"))CoreHardEventV21("source-extractor-SetFlag0-false-SetInt0",e,e.sourceNPC.Substring(4)+"_START_STATE_END",elapsed,0,(int)elapsed);CoreHardEnterV21(e,transition.target,"source-"+transition.kind+"-"+transition.condition);return true;}
   if(e.sourceNPC=="NUI_NASOD_KING"&&!CoreHardCallbacksConfiguredV21&&CoreHardOnceV21(e,"combat-hold"))CoreHardEventV21("HOLD-source-King-brain-exclusive-callbacks",e,"King-source-contract-phase-boundary",elapsed,0,0,null,null,"This phase executes original controller coupling, not unimplemented King combat AI/casters");
   return true;
  }
  bool TickCoreHardAfterVictoryV21(float dt){if(coreHardV21==null||!victory||paused||!Finite(dt)||dt<=0)return false;var plant=CoreHardNearV21("NUI_EL_POWER_PLANT",false);
   // A real lethal hit may leave the original actor pending while its ordinary
   // hitPause consumes this packet. Victory must not prevent that first DYING
   // initialization. This is only the original final key1 Plant, never a live
   // actor or an already-entered/restarted terminal state.
   if(plant!=null&&plant.hp==0&&string.IsNullOrEmpty(plant.npcState)){
    CoreHardRuntimeV21 pendingRuntime;NpcStateDefinition pendingTerminal;var rule=CoreHardActorRuleV21(plant);
    if(!CoreHardClosureScopeV21()||!NativeTerrain||!plant.active||plant.dormant||plant.sourceKeyCode!=1||plant.encounterIndex!=0||plant.npcTime!=0||plant.clock!=0||rule==null||rule.dyingState!="EL_POWER_PLANT_DYING"||!coreHardActorsV21.TryGetValue(plant,out pendingRuntime)||!pendingRuntime.initialized||string.IsNullOrEmpty(pendingRuntime.last)||pendingRuntime.last==rule.dyingState||!npcStates.ContainsKey(plant.sourceNPC+"/"+pendingRuntime.last)||!npcStates.TryGetValue(plant.sourceNPC+"/"+rule.dyingState,out pendingTerminal)||pendingTerminal.id!=rule.dyingState)return false;
    dt=Math.Min(dt,.05f);elapsed+=dt;
    // The original core actor owner consumes hitPause, then chooses the
    // configured dyingState through TickCoreHardManagedV21. No state/clock/HP
    // is assigned here. Returning now prevents a second tick in this packet.
    TickEnemyCore(plant,dt);var pendingKing=CoreHardNearV21("NUI_NASOD_KING",false);if(pendingKing!=null&&pendingKing.hp<=0)TickCoreHardManagedV21(pendingKing,dt);return true;
   }
   if(plant==null||plant.hp>0||plant.npcState!="EL_POWER_PLANT_DYING")return false;var state=NpcState(plant);if(state==null||plant.npcTime>=state.duration)return true;dt=Math.Min(dt,.05f);elapsed+=dt;TickCoreHardManagedV21(plant,dt);var king=CoreHardNearV21("NUI_NASOD_KING",false);if(king!=null&&king.hp<=0)TickCoreHardManagedV21(king,dt);return true;}
 }
}
