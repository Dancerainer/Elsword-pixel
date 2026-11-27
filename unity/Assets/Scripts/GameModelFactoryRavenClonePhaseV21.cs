using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ElswordPixel {
 [Serializable] public sealed class FactoryRavenUnresolvedSoundV21 {
  public string state,id,sourceField,sourceName,candidateClock;public float sourceTime,candidateTime;
 }
 [Serializable] public sealed class FactoryRavenClonePhaseDocumentV21 {
  public string adapter;public NpcDefinition prototype;public NpcMechanismStateRule[] mechanisms;
  public NpcSoundDocument sounds;public FactoryRavenUnresolvedSoundV21[] unresolvedSounds;
 }
 public sealed class FactoryRavenCloneEventV21 {
  public readonly long serial;public readonly Enemy actor;public readonly string kind,asset;
  public readonly float time,sourceTime,x,y,scale;public readonly int sourceStartPosition;
  internal FactoryRavenCloneEventV21(long s,Enemy e,string k,string a,float t,float st,float px,float py,float z,int start){serial=s;actor=e;kind=k;asset=a;time=t;sourceTime=st;x=px;y=py;scale=z;sourceStartPosition=start;}
 }
 public sealed class FactoryRavenCloneStatusV21 {
  public readonly Enemy actor;public readonly bool flag0,flag1;public readonly int phaseCount,releaseCount,pending,live;
  public readonly float disabledElapsed,limitHp,sourceMp,authoredPrototypeHp;public readonly string state;
  internal FactoryRavenCloneStatusV21(Enemy e,bool f0,bool f1,int p,int r,int q,int n,float age,float lim,float mp,float hp){actor=e;flag0=f0;flag1=f1;phaseCount=p;releaseCount=r;pending=q;live=n;disabledElapsed=age;limitHp=lim;sourceMp=mp;authoredPrototypeHp=hp;state=e==null?null:e.npcState;}
 }
 public sealed partial class GameModel {
  const string FactoryRavenCloneBossV21="NUI_RAVEN_CLONE",FactoryRavenProtoV21="NUI_RAVEN_CLONE_PROTO";
  const string FactoryRavenReadyV21="RAVEN_CLONE_SUMMON_CLONE_READY",FactoryRavenSummonV21="RAVEN_CLONE_SUMMON_CLONE",FactoryRavenDisabledV21="RAVEN_CLONE_DISABLED";
  bool factoryRavenCloneConfiguredV21;float factoryRavenPrototypeHpV21;long factoryRavenCloneEventSerialV21;
  FactoryRavenClonePhaseDocumentV21 factoryRavenCloneDocumentV21;
  sealed class FactoryRavenCloneActorV21 {internal Enemy actor;internal bool flag0,flag1,summoned;internal int phases,releases;internal float disabledElapsed,limitHp,sourceMp;}
  FactoryRavenCloneActorV21 factoryRavenCloneActorV21;
  readonly List<FactoryRavenCloneEventV21> factoryRavenCloneEventsV21=new List<FactoryRavenCloneEventV21>();
  public bool FactoryRavenClonePhaseConfiguredV21 {get{return factoryRavenCloneConfiguredV21;}}
  // The exact independently generated Proto document is additional to the
  // immutable eleven-profile/eight-document Factory constructor. HP is an
  // explicit demo balance value: neither original Proto contract gives MAX_HP.
  public void ConfigureFactoryRavenClonePhaseV21(byte[] exactPrototypeDocument,float authoredPrototypeHp){
   if(factoryRavenCloneConfiguredV21||!FactoryRavenIntroConfiguredV21||room!=0||elapsed!=0||!NativeTerrain||npcDefinitions.Count!=11||npcDefinitions.ContainsKey(FactoryRavenProtoV21)||!Finite(authoredPrototypeHp)||authoredPrototypeHp<=0)throw new InvalidOperationException("Factory clone phases require the fresh original full Factory and an explicit positive authored Proto HP");
   if(exactPrototypeDocument==null||exactPrototypeDocument.Length!=1593517)throw new ArgumentException("Factory Proto document length differs");
   using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(exactPrototypeDocument)).Replace("-","").ToLowerInvariant()!="d94654f64be40757a875266e863c2341fe0239aa1a66acc115620007420cd827")throw new ArgumentException("Factory Proto document SHA differs");
   var doc=FactoryTypedJsonR581.ReadFactoryRavenClonePhaseV21(exactPrototypeDocument);var p=doc==null?null:doc.prototype;
   if(doc==null||doc.adapter!="factory-raven-original-clone-phases-normal-v21-r1"||p==null||p.id!=FactoryRavenProtoV21||p.kind!="raven-clone-proto"||p.initialState!="RAVEN_CLONE_WAIT"||p.states==null||p.states.Length!=46||p.bodyWidth!=28||p.bodyHeight!=42||doc.mechanisms==null||doc.mechanisms.Length!=46||doc.sounds==null||doc.unresolvedSounds==null||doc.unresolvedSounds.Length!=1)throw new ArgumentException("Factory Proto source identity/scope differs");
   var absent=doc.unresolvedSounds[0];if(absent.sourceName!="NPCRaven_StepLanding.ogg"||absent.state!="RAVEN_CLONE_DASH_JUMP_COMBO_Z2_LANDING"||absent.sourceField!="SOUND_PLAY0"||absent.sourceTime!=.63f||absent.candidateTime!=.63f)throw new ArgumentException("The source-missing Proto audio cue must remain explicit");
   foreach(string id in new[]{"RAVEN_CLONE_WAIT","RAVEN_CLONE_WALK"}){
    NpcStateDefinition state;if(!npcStates.TryGetValue(FactoryRavenCloneBossV21+"/"+id,out state)||state.choices.Length<2||state.choices[0].nextState!=FactoryRavenReadyV21||state.choices[0].hpLessPercent!=30||state.choices[0].near!=280||state.choices[0].rate!=100||state.choices[1].nextState!=FactoryRavenReadyV21||state.choices[1].hpLessPercent!=70||state.choices[1].near!=280||state.choices[1].rate!=100)throw new ArgumentException("Factory original ordered 30/70 FLAG_FALSE conditions differ");
   }
   foreach(string id in new[]{FactoryRavenReadyV21,FactoryRavenSummonV21,FactoryRavenDisabledV21}){
    NpcStateDefinition state;if(!npcStates.TryGetValue(FactoryRavenCloneBossV21+"/"+id,out state)||state.hits.Length!=0||state.velocities.Length!=0||state.entryOffsetY!=0||state.damageChanges.Length!=0||state.sourceEventTransitions.Length!=0||state.followTarget||state.walkSpeed!=0)throw new ArgumentException("Factory original clone phase is not the bound no-attack/no-velocity state");
   }
   // Validate on an isolated model with the normal complete validators. Only
   // its validated additional dictionaries are appended; no live setter clears
   // the original Factory rules, mechanisms, sound queues or birth flags.
   var validator=new GameModel();validator.SetNpcDefinitions(new[]{p});validator.SetNpcMechanismDefinitions(new NpcMechanismDocument{states=doc.mechanisms});validator.SetNpcSoundDefinitions(doc.sounds);
   foreach(var pair in validator.npcStates)if(npcStates.ContainsKey(pair.Key))throw new ArgumentException("Proto state collision");
   foreach(var pair in validator.npcMechanismRules)if(npcMechanismRules.ContainsKey(pair.Key))throw new ArgumentException("Proto mechanism collision");
   foreach(var pair in validator.npcStateSounds)if(npcStateSounds.ContainsKey(pair.Key))throw new ArgumentException("Proto sound collision");
   npcDefinitions.Add(p.id,p);foreach(var pair in validator.npcStates)npcStates.Add(pair.Key,pair.Value);foreach(var pair in validator.npcMechanismRules)npcMechanismRules.Add(pair.Key,pair.Value);foreach(var pair in validator.npcStateSounds)npcStateSounds.Add(pair.Key,pair.Value);foreach(string id in validator.npcSourceSoundAssetsV17)npcSourceSoundAssetsV17.Add(id);
   factoryRavenPrototypeHpV21=authoredPrototypeHp;factoryRavenCloneDocumentV21=doc;factoryRavenCloneConfiguredV21=true;
  }
  bool FactoryRavenCloneBossContextV21(Enemy e){return factoryRavenCloneConfiguredV21&&room==2&&factoryEntryRoomsV542!=null&&ReferenceEquals(CurrentRoom,factoryEntryRoomsV542[2])&&CurrentRoom.id==FactoryRouteV542+"-2"&&HasEncounters&&EncounterIndex==0&&e!=null&&ReferenceEquals(e,factoryRavenReleasedActorV21)&&enemies.Contains(e)&&e.sourceNPC==FactoryRavenCloneBossV21&&e.kind=="raven-clone"&&e.encounterIndex==0&&e.sourceKeyCode==0&&!e.isGate&&!e.passive&&e.countsForClear;}
  FactoryRavenCloneActorV21 FactoryRavenCloneActorForV21(Enemy e){
   if(!FactoryRavenCloneBossContextV21(e))return null;
   if(factoryRavenCloneActorV21==null)factoryRavenCloneActorV21=new FactoryRavenCloneActorV21{actor=e};
   if(!ReferenceEquals(factoryRavenCloneActorV21.actor,e))throw new InvalidOperationException("Factory clone flags cannot migrate to a replacement actor");return factoryRavenCloneActorV21;
  }
  void FactoryRavenCloneEventV21(Enemy actor,string kind,string asset="",float sourceTime=0,float scale=1,int sourceStartPosition=-1){
   if(factoryRavenCloneEventsV21.Count>=256)factoryRavenCloneEventsV21.RemoveAt(0);
   factoryRavenCloneEventsV21.Add(new FactoryRavenCloneEventV21(++factoryRavenCloneEventSerialV21,actor,kind,asset,elapsed,sourceTime,actor.x,actor.y,scale,sourceStartPosition));
  }
  public FactoryRavenCloneEventV21[] ReadFactoryRavenCloneEventsV21(){return factoryRavenCloneEventsV21.ToArray();}
  int FactoryRavenLiveProtoV21(){int count=0;foreach(var e in enemies)if(e.sourceNPC==FactoryRavenProtoV21&&e.hp>0)count++;return count;}
  int FactoryRavenPendingProtoV21(){int count=0;foreach(var r in npcSummonRequests)if(r.npc==FactoryRavenProtoV21&&r.room==room)count++;return count;}
  public FactoryRavenCloneStatusV21 ReadFactoryRavenClonePhaseV21(){var r=factoryRavenCloneActorV21;return new FactoryRavenCloneStatusV21(r==null?null:r.actor,r!=null&&r.flag0,r!=null&&r.flag1,r==null?0:r.phases,r==null?0:r.releases,FactoryRavenPendingProtoV21(),FactoryRavenLiveProtoV21(),r==null?0:r.disabledElapsed,r==null?0:r.limitHp,r==null?0:r.sourceMp,factoryRavenPrototypeHpV21);}
  void ClearFactoryRavenClonePhaseRoomV21(){factoryRavenCloneActorV21=null;factoryRavenCloneEventsV21.Clear();}
  bool FactoryRavenCloneConditionAllowedV21(Enemy e,NpcCondition c){
   if(!factoryRavenCloneConfiguredV21||e.sourceNPC!=FactoryRavenCloneBossV21||c.nextState!=FactoryRavenReadyV21)return true;
   var r=FactoryRavenCloneActorForV21(e);if(r==null)return false;
   if(c.hpLessPercent==30)return !r.flag1;if(c.hpLessPercent==70)return !r.flag0;throw new InvalidOperationException("Unbound Factory clone FLAG_FALSE condition");
  }
  void OnFactoryRavenCloneStateEnteredV21(Enemy e,NpcStateDefinition state){
   var r=FactoryRavenCloneActorForV21(e);if(r==null)return;
   if(state.id==FactoryRavenReadyV21){
    if(e.hp>e.maxHp*.7f)throw new InvalidOperationException("Factory source clone READY requires actual HP at or below70 percent");
    r.flag0=true;if(e.hp<=e.maxHp*.3f)r.flag1=true;r.phases++;r.summoned=false;r.disabledElapsed=0;
    FactoryRavenCloneEventV21(e,"source-ready-sound","Nasod_Element_Teleport.ogg");FactoryRavenCloneEventV21(e,"source-minor-xmesh","RespawnLanding01",0,2);FactoryRavenCloneEventV21(e,"source-minor-xmesh","RespawnLanding02",0,2);
    QueueNpcSound(new NpcTimedSound{id="factory-raven-clone-ready/"+r.phases,sound="factory-npc-nasod_element_teleport",sourceName="Nasod_Element_Teleport.ogg",sourceField="RAVEN_CLONE_SUMMON_CLONE_READY_STATE_START.PlaySound_LUA",candidateClock="npc-state-pose-seconds"},e,0,0,elapsed,state.id);
   }else if(state.id==FactoryRavenDisabledV21){r.disabledElapsed=0;FactoryRavenCloneEventV21(e,"source-disabled");}
   else if(state.id=="RAVEN_CLONE_WAIT"&&r.phases>0)FactoryRavenCloneEventV21(e,"source-wait");
  }
  bool FactoryRavenCloneAirborneStateV21(Enemy e){return FactoryRavenCloneBossContextV21(e)&&(e.npcState==FactoryRavenSummonV21||e.npcState==FactoryRavenDisabledV21);}
  bool FactoryRavenCloneAirborneRequestV21(Enemy e,NpcStateDefinition current,string target){
   if(!FactoryRavenCloneBossContextV21(e)||current==null||e.npcTime+1e-6f<current.duration)return false;
   var r=factoryRavenCloneActorV21;if(r==null||!ReferenceEquals(r.actor,e))return false;
   return current.id==FactoryRavenReadyV21&&target==FactoryRavenSummonV21&&r.limitHp>0||current.id==FactoryRavenSummonV21&&target==FactoryRavenDisabledV21&&r.summoned;
  }
  static void FactoryRavenSourcePositionV21(Enemy e,float sourceX,float sourceY){e.x=32+sourceX*.28f;e.y=54+sourceY*.28f;e.groundLine=-1;}
  void FactoryRavenRequestProtoV21(Enemy boss,int start,float sourceX,float sourceY,float delay,float at){
   float x=32+sourceX*.28f,y=54+sourceY*.28f;
   var template=new EnemySpawn{sourceNPC=FactoryRavenProtoV21,kind="raven-clone-proto",hp=factoryRavenPrototypeHpV21,visualScale=1,nativeDepth=0,aiEnabled=true,countsForClear=true};
   npcSummonRequests.Add(new NpcSummonRequest{owner=boss,template=template,npc=FactoryRavenProtoV21,room=room,encounter=boss.encounterIndex,line=-1,x=x,y=y,due=at+delay});SummonEvent("request",boss,FactoryRavenProtoV21,x,y,-1);
   FactoryRavenCloneEventV21(boss,"source-proto-request",FactoryRavenProtoV21,.1f,1,start);
  }
  bool TickFactoryRavenDisabledV21(Enemy e,NpcStateDefinition state,float dt){
   if(state.id!=FactoryRavenDisabledV21||!FactoryRavenCloneBossContextV21(e))return false;
   var r=FactoryRavenCloneActorForV21(e);if(!r.summoned)throw new InvalidOperationException("Factory DISABLED entered without its original summon callback");
   r.disabledElapsed+=dt;AdvanceNpcLoopSounds(e,state,e.npcTime,dt,elapsed-dt);e.npcTime=(e.npcTime+dt)%state.duration;
   // AnimTimer_LUA(3) is represented by this explicit 2D elapsed threshold;
   // subsequent ordinary packets poll the same original live-type predicate.
   // Native VM polling cadence is not claimed reproduced.
   if(r.disabledElapsed<3)return true;
   // Original Normal reFill is MaxHP*0. This cap is the original vector0.x,
   // never invented healing or a reset to maximum health.
   e.hp=Math.Min(e.hp,r.limitHp);
   if(FactoryRavenLiveProtoV21()!=0||FactoryRavenPendingProtoV21()!=0)return true;
   FactoryRavenSourcePositionV21(e,2765.91f,140+2500);r.sourceMp=0;r.releases++;FactoryRavenCloneEventV21(e,"source-live-zero-release","",3,1,15);
   EnterNpcState(e,npcStates[FactoryRavenCloneBossV21+"/RAVEN_CLONE_WAIT"]);e.npcDecisionElapsed=0;return true;
  }
  void AdvanceFactoryRavenClonePhaseV21(Enemy e,NpcStateDefinition state,float from,float to,float packetStart){
   var r=FactoryRavenCloneActorForV21(e);if(r!=null){
    if(state.id==FactoryRavenReadyV21&&from<state.duration&&to+1e-6f>=state.duration){FactoryRavenSourcePositionV21(e,1487.08f,3000+100);r.limitHp=e.hp+(e.maxHp-e.hp)*.2f;FactoryRavenCloneEventV21(e,"source-ready-end-position","",state.duration,1,20);}
    if(state.id==FactoryRavenSummonV21&&!r.summoned&&from<.1f&&.1f<=to+1e-6f){float at=packetStart+.1f-from;r.summoned=true;r.sourceMp=0;FactoryRavenRequestProtoV21(e,8,276.233f,140+1000,.1f,at);FactoryRavenRequestProtoV21(e,15,2765.91f,140+1000,.2f,at);FactoryRavenCloneEventV21(e,"source-flush-two-normal-protos","",.1f);}
   }
   if(factoryRavenCloneConfiguredV21&&e.sourceNPC==FactoryRavenProtoV21)foreach(var cue in factoryRavenCloneDocumentV21.unresolvedSounds)if(state.id==cue.state&&from<cue.candidateTime&&cue.candidateTime<=to+1e-6f)FactoryRavenCloneEventV21(e,"unresolved-original-source-audio",cue.sourceName,cue.sourceTime);
  }
 }
}
