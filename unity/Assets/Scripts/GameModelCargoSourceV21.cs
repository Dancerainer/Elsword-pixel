using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class CargoMissileLaneV21 {public string bone;public float[] originOffset;public bool sourceDirectionBoolean;public int sourceSlotArgument;public float sourceAngleArgument,rotationZ;}
 [Serializable] public sealed class CargoDecisionV21 {public string state,sourceCondition;public bool far;public float nativeDistance,rate,hpLessPercent;}
 [Serializable] public sealed class CargoSoundV21 {public string sourceName,sound;}
 [Serializable] public sealed class CargoInitialStateV21 {public string npc,state;}
 [Serializable] public sealed class CargoSourceDocumentV21 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,wallyNpc,missileEffect,specialEffect,missileCallbackSha256,missileHelperSha256,visualHelperSha256,specialCallbackSha256,sourcePhysicsAdapter,sourceRotationAdapter,sourceBrainAdapter,sourceSummonPolicy;
  public bool runtimeAdmitted,nativeExecuted;public string[] npcUnitIds,heldNpcIds,winStates;
  public float sourceUnitScale,missileAt,specialAt,specialNativeDistance,specialNativeHeight,jumpUpSeconds,jumpPassiveNativeY,jumpEntryNativeY,jumpDirectionalNativeX,sourceGravityNative,sourceTerminalNative;
  public CargoMissileLaneV21[] missileLanes;public CargoDecisionV21[] decisions;public CargoSoundV21[] sounds;public CargoInitialStateV21[] initialStates;public float[] eyeOriginOffset;
 }
 [Serializable] public sealed class CargoSourceEventV21 {public string type,npc,state,source,bone;public float time,x,y,z,landY,rotationZ,amount;public int lane,serial;}
 public sealed partial class GameModel {
  CargoSourceDocumentV21 cargoSourceV21;
  sealed class CargoCursorV21 {internal string state;internal int emitted;}
  readonly Dictionary<Enemy,CargoCursorV21> cargoCursorsV21=new Dictionary<Enemy,CargoCursorV21>();
  readonly HashSet<Enemy> cargoInitializedV21=new HashSet<Enemy>();
  public readonly List<CargoSourceEventV21> cargoSourceEventsV21=new List<CargoSourceEventV21>();
  public bool CargoSourceConfiguredV21 {get{return cargoSourceV21!=null;}}
  public Func<float> CargoConditionRollOverrideV21;
  public void SetCargoSourceV21(CargoSourceDocumentV21 d){
   if(EmptyDragonDtoV20(d))d=null;
   if(d!=null){
    if(d.initialStates==null||d.initialStates.Length!=6||d.eyeOriginOffset==null||d.eyeOriginOffset.Length!=3||d.winStates==null||d.winStates.Length==0)throw new ArgumentException("Missing Cargo original initial states/eye bone/win references");foreach(var init in d.initialStates)if(init==null||string.IsNullOrEmpty(init.npc)||string.IsNullOrEmpty(init.state)||!npcStates.ContainsKey(init.npc+"/"+init.state))throw new ArgumentException("Unbound original Cargo initial state");foreach(float v in d.eyeOriginOffset)if(!Finite(v))throw new ArgumentException("Unbound original Cargo eye bone");foreach(string id in d.winStates)if(!npcStates.ContainsKey(d.wallyNpc+"/"+id))throw new ArgumentException("Unbound Cargo original WIN callback site");
    if(d.schema!="cargo-original-callbacks-runtime-v21"||d.runtimeAdmitted||d.nativeExecuted||!SourceSupportHashV17(d.sourceRulesSha256)||!SourceSupportHashV17(d.sourceEffectsSha256)||d.wallyNpc!="NUI_WALLY_9TH_ARMOR"||d.sourceUnitScale!=.28f||d.missileAt!=2.1f||d.specialAt!=.01f||d.specialNativeDistance!=700||d.specialNativeHeight!=50||d.jumpUpSeconds!=3||d.jumpPassiveNativeY!=100||d.jumpEntryNativeY!=45||d.jumpDirectionalNativeX!=300||d.sourceGravityNative!=4000||d.sourceTerminalNative!=2000||d.missileLanes==null||d.missileLanes.Length!=6||d.decisions==null||d.decisions.Length!=4||d.sounds==null||d.npcUnitIds==null||d.npcUnitIds.Length!=12||d.heldNpcIds==null||d.heldNpcIds.Length!=1||d.heldNpcIds[0]!="NUI_SMALLGATE_BOSS_POST_BESMA_AIR_SHIP")throw new ArgumentException("Changed Cargo source scalar/identity contract");
    if(d.missileEffect!="DamageEffect_WALLY_9TH_ARMOR_ATTACK_C"||d.specialEffect!="DamageEffect_WALLY_9TH_ARMOR_SPECIAL_ATTACK_A"||!npcEffectDefinitions.ContainsKey(d.missileEffect)||!npcEffectDefinitions.ContainsKey(d.specialEffect)||d.missileCallbackSha256!="6e680b9d43316ed8e21e87ea1c1b5c7f6fb94175d79a5e835f2f49cf82e05f9f"||d.missileHelperSha256!="c4e66d3bfd3aa26f4b9d0e019aa04c288e0deca7ca0849cf68b5a79d6e659d21"||d.visualHelperSha256!="e74c424f013a1ee5561b53a4d88cc383e64f49e09c5c197a6043e820342af414"||d.specialCallbackSha256!="dd249cf762decd6b25a72e9774250e8bb27a67883fb46be7487ab2e67d62f9fb")throw new ArgumentException("Changed Cargo original callback/closure binding");
    if(d.sourcePhysicsAdapter!="explicit-source-state-passive-up3s-and-existing2D-gravity-real-ground-v21"||d.sourceRotationAdapter!="original-six-z-angle-plus-owner-facing-projected2D-existing-effect-rotation-v18"||d.sourceBrainAdapter!="original-EVENT_PROCESS-order-conditions-with-existing2D-rate-and-target-adapter-v21"||d.sourceSummonPolicy!="no-reachable-CreateNPC-in-original12-cargo-static-closure-v21")throw new ArgumentException("Unrecorded Cargo source projection adapter");
    float[] angles={-30,-15,0,210,195,180},args={-30,-15,0,30,15,0};for(int i=0;i<6;i++){var q=d.missileLanes[i];if(q==null||q.bone!="Dummy"+(6+i)+"_Skirt"||q.sourceSlotArgument!=i||q.sourceDirectionBoolean!=(i>=3)||q.sourceAngleArgument!=args[i]||q.rotationZ!=angles[i]||q.originOffset==null||q.originOffset.Length!=3)throw new ArgumentException("Changed original six Cargo missile lanes");foreach(float v in q.originOffset)if(!Finite(v))throw new ArgumentException("Unbound original skirt bone");}
    string[] suffix={"SPECIAL_ATTACK_A_READY","ATTACK_C_READY","ATTACK_B_READY","ATTACK_A_READY"};float[] hp={40,70,101,101};for(int i=0;i<4;i++){var q=d.decisions[i];if(q==null||q.state!="WALLY_9TH_ARMOR_"+suffix[i]||q.sourceCondition!="CT_"+q.state||q.nativeDistance!=600||q.rate!=25||q.hpLessPercent!=hp[i]||q.far!=(i==0||i==2)||!npcStates.ContainsKey(d.wallyNpc+"/"+q.state))throw new ArgumentException("Changed Cargo declared EVENT_PROCESS decision order");}
    foreach(var sound in d.sounds)if(sound==null||string.IsNullOrEmpty(sound.sourceName)||string.IsNullOrEmpty(sound.sound))throw new ArgumentException("Invalid Cargo callback sound map");
   }
   cargoSourceV21=d;ClearCargoSourceRoomV21();
  }
  void ClearCargoSourceRoomV21(){ClearCargoBeeReactionR1484();ClearCargoProjectionChartsR1479();ClearCargoCommittedParticlesV21();ClearCargoDirectorRoomV21();cargoCursorsV21.Clear();cargoInitializedV21.Clear();cargoSourceEventsV21.Clear();}
  void CargoEventV21(Enemy e,string type,string source,float at,float x,float y,float z=0,string bone=null,int lane=-1,float rotation=0,int serial=0,float landY=0,float amount=0){if(cargoSourceEventsV21.Count>=4096)cargoSourceEventsV21.RemoveAt(0);cargoSourceEventsV21.Add(new CargoSourceEventV21{type=type,npc=e.sourceNPC,state=e.npcState,source=source,time=at,x=x,y=y,z=z,bone=bone,lane=lane,rotationZ=rotation,serial=serial,landY=landY,amount=amount});}
  bool CargoWallyBoundV21(Enemy e){return cargoSourceV21!=null&&e!=null&&e.sourceNPC==cargoSourceV21.wallyNpc;}
  // Retain fractional HP loss only for a genuine original Cargo six-lane missile.
  // Do not require a living owner: an already-emitted missile keeps its source lifetime.
  bool CargoMissileFractionalScopeR1473(NpcEffectInstance effect){
   if(cargoSourceV21==null||!NativeTerrain||room!=2||CurrentRoom.id!="Dungeon_Besma_Air_Ship_Normal-2"||EncounterIndex!=0||effect==null||effect.ended||effect.reflected||!npcEffects.Contains(effect))return false;
   var owner=effect.owner;var emission=effect.emission;NpcEffectDefinition registered;
   if(owner==null||!enemies.Contains(owner)||owner.encounterIndex!=EncounterIndex||!CargoWallyBoundV21(owner)||!cargoInitializedV21.Contains(owner)||effect.id!=cargoSourceV21.missileEffect||!npcEffectDefinitions.TryGetValue(effect.id,out registered)||!object.ReferenceEquals(effect.definition,registered))return false;
   var profile=NpcProfile(owner);if(profile==null||profile.id!=cargoSourceV21.wallyNpc)return false;
   if(emission==null||emission.effect!=effect.id||!emission.sourceLandPositionYArgument||!effect.hasSourceLandY||!string.IsNullOrEmpty(emission.ownerState))return false;
   foreach(var lane in cargoSourceV21.missileLanes)if(emission.rotationZ==lane.rotationZ&&emission.offsetX==lane.originOffset[0]&&emission.offsetY==lane.originOffset[1]&&emission.offsetZ==lane.originOffset[2])return true;
   return false;
  }
  void OnCargoSourceStateExitedV21(Enemy e){if(e!=null)cargoCursorsV21.Remove(e);}
  void OnCargoSourceStateEnteredV21(Enemy e,NpcStateDefinition s){
   if(cargoSourceV21!=null&&e!=null&&Array.Exists(cargoSourceV21.initialStates,q=>q.npc==e.sourceNPC))cargoInitializedV21.Add(e);if(!CargoWallyBoundV21(e))return;
   if(s.id=="WALLY_9TH_ARMOR_START_READY"){var a=MechanismActor(e);if(a!=null){a.timer=a.previousTimer=0;}CargoEventV21(e,"source-visibility-request","START_READY SetShow(FALSE)/SetShowGage(FALSE)",elapsed,e.x,e.y);}
   if(s.id=="WALLY_9TH_ARMOR_START"||s.id=="WALLY_9TH_ARMOR_START_HENIR")CargoEventV21(e,"source-visibility-request","START SetShow(TRUE)/SetShowGage(TRUE)",elapsed,e.x,e.y,0,null,-1,0,0,0,1);
   if(CargoSourceFlightStateV21(e,s)){
    bool up=s.id.EndsWith("JUMP_UP",StringComparison.Ordinal)||s.id.EndsWith("JUMP_UP_DIR",StringComparison.Ordinal);if(up){if(s.entryOffsetY==0)e.y+=45*.28f;e.groundLine=-1;e.vy=100*.28f;e.vx=e.npcVelocityX=0;}CargoEventV21(e,"source-flight-entry",s.id,elapsed,e.x,e.y,0,null,-1,0,0,0,e.vy);
   }
  }
  void CargoCallbackSoundV21(Enemy e,string name,float at,float sourceTime){var s=Array.Find(cargoSourceV21.sounds,q=>q.sourceName==name);if(s==null||!npcSourceSoundAssetsV17.Contains(s.sound))throw new InvalidOperationException("Missing exact Cargo callback sound "+name);QueueNpcSound(new NpcTimedSound{id="cargo/"+e.npcState+"/"+name,sound=s.sound,sourceName=name,sourceField="original-Cargo-Lua-PlaySound_LUA",sourceTime=sourceTime,candidateTime=sourceTime,candidateClock="source-animation-callback"},e,++npcSoundEntrySerial,0,at,e.npcState);}
  void AdvanceCargoSourceCallbacksV21(Enemy e,NpcStateDefinition s,float from,float to,float packetStart){
   if(!CargoWallyBoundV21(e))return;CargoCursorV21 cursor;if(!cargoCursorsV21.TryGetValue(e,out cursor)||cursor.state!=s.id){cursor=new CargoCursorV21{state=s.id};cargoCursorsV21[e]=cursor;}
   if(s.id=="WALLY_9TH_ARMOR_ATTACK_C"&&(cursor.emitted&1)==0&&from<cargoSourceV21.missileAt&&to+1e-6f>=cargoSourceV21.missileAt){cursor.emitted|=1;float at=packetStart+cargoSourceV21.missileAt-from;CargoCallbackSoundV21(e,"Wally9Armor_MissileFly.ogg",at,2.1f);
    for(int i=0;i<6;i++){var q=cargoSourceV21.missileLanes[i];var emission=new NpcEffectEmission{effect=cargoSourceV21.missileEffect,offsetX=q.originOffset[0],offsetY=q.originOffset[1],offsetZ=q.originOffset[2],rotationZ=q.rotationZ,sourceLandPositionYArgument=true};var fx=SpawnNpcEffect(e,npcEffectDefinitions[emission.effect],emission,at);CargoEventV21(e,"source-six-missile",emission.effect,at,fx.x,fx.y,fx.z,q.bone,i,q.rotationZ,fx.serial,fx.sourceLandY);CargoEventV21(e,"source-particle-request","Wally9th_MissileAttack02",at,fx.x+e.facing*2.8f,fx.y,fx.z-14,q.bone,i,q.rotationZ);CommitCargoParticleV21(e,0,at,fx.x+e.facing*2.8f,fx.y,fx.z-14,fx.serial,q.bone,i,fx.hasSourceLandY,fx.sourceLandY);}
    var eye=cargoSourceV21.eyeOriginOffset;CargoEventV21(e,"source-particle-request","LED_Wally9th_Armor_MissileAttack_EyeLed01",at,e.x+e.facing*eye[0],e.y+eye[1],e.nativeDepth+eye[2]-2.8f,"Dummy1_Eye");CommitCargoParticleV21(e,1,at,e.x+e.facing*eye[0],e.y+eye[1],eye[2]-2.8f,0,"Dummy1_Eye");
   }
   if(s.id=="WALLY_9TH_ARMOR_SPECIAL_ATTACK_A"&&(cursor.emitted&2)==0&&from<cargoSourceV21.specialAt&&to+1e-6f>=cargoSourceV21.specialAt){cursor.emitted|=2;float at=packetStart+cargoSourceV21.specialAt-from;var ground=new NpcEffectInstance{owner=e};CaptureNpcSourceLandArgumentV18(ground,null,new NpcEffectEmission{sourceLandPositionYArgument=true});float land=ground.sourceLandY+50*.28f;
    foreach(int side in new[]{1,-1}){var emission=new NpcEffectEmission{effect=cargoSourceV21.specialEffect,offsetX=side*700*.28f,offsetY=land-e.y,sourceLandPositionYArgument=true};var fx=SpawnNpcEffect(e,npcEffectDefinitions[emission.effect],emission,at);fx.hasSourceLandY=true;fx.sourceLandY=land;CargoEventV21(e,"source-special-ball",emission.effect,at,fx.x,fx.y,fx.z,null,side,0,fx.serial,land);CargoEventV21(e,"source-particle-request","Wally9th_Armor_ChargeBall02",at,fx.x,fx.y,fx.z,null,side);CommitCargoParticleV21(e,side==1?2:3,at,fx.x,fx.y,fx.z,fx.serial,null,side,true,land);}
   }
  }
 }
}
