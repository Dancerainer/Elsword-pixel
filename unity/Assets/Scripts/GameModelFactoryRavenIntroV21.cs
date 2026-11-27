using System;
namespace ElswordPixel {
 public sealed class FactoryRavenFocusOwnerV21 {
  internal readonly GameModel model;internal readonly object view;internal readonly long registration;internal bool ended;
  internal FactoryRavenFocusOwnerV21(GameModel m,object v,long n){model=m;view=v;registration=n;}
  public long Registration {get{return registration;}}
 }
 public sealed class FactoryRavenIntroTicketV21 {
  internal readonly GameModel model;internal readonly Enemy actor;internal readonly FactoryRavenFocusOwnerV21 owner;internal readonly long serial,generation;
  internal FactoryRavenIntroTicketV21(GameModel m,Enemy e,FactoryRavenFocusOwnerV21 o,long s,long g){model=m;actor=e;owner=o;serial=s;generation=g;}
  public Enemy Actor {get{return actor;}}public long Serial {get{return serial;}}public long Generation {get{return generation;}}
  public int Room {get{return 2;}}public int Encounter {get{return 0;}}public int Order {get{return 0;}}
  public string Npc {get{return "NUI_RAVEN_CLONE";}}public string InitialState {get{return "RAVEN_CLONE_START";}}
 }
 public sealed class FactoryRavenIntroStatusV21 {
  public readonly FactoryRavenIntroTicketV21 ticket;public readonly CoreStopIntroPhaseV21 phase;public readonly string reason;
  public readonly float sourceTime,duration;public readonly int releaseCount;public readonly bool focusAccepted;
  internal FactoryRavenIntroStatusV21(FactoryRavenIntroTicketV21 t,CoreStopIntroPhaseV21 p,string r,float time,int releases,bool accepted){ticket=t;phase=p;reason=r;sourceTime=time;duration=3.8f;releaseCount=releases;focusAccepted=accepted;}
 }
 public sealed partial class GameModel {
  // Exact Factory START only. This is the authored 2D focus-observation owner,
  // not a claim that the native PartsLookDirectCamera service was executed.
  bool factoryRavenIntroConfiguredV21;long factoryRavenIntroGenerationV21,factoryRavenIntroSerialV21,factoryRavenFocusSerialV21;
  FactoryRavenFocusOwnerV21 factoryRavenFocusOwnerV21;Enemy factoryRavenPreparedActorV21,factoryRavenReleasedActorV21;
  NpcStateDefinition factoryRavenStartV21,factoryRavenWaitV21;NpcDefinition factoryRavenProfileV21;
  sealed class FactoryRavenRuntimeV21 {
   internal FactoryRavenIntroTicketV21 ticket;internal CoreStopIntroPhaseV21 phase=CoreStopIntroPhaseV21.AwaitingFocus;
   internal string reason="awaiting-same-owner-actual-visible-focus";internal double time;internal float lastElapsed=-1,queuedAt;
   internal bool queuedAck,accepted;internal string queuedFailure;internal int releases;
  }
  FactoryRavenRuntimeV21 factoryRavenRuntimeV21;
  public bool FactoryRavenIntroConfiguredV21 {get{return factoryRavenIntroConfiguredV21;}}
  public void ConfigureFactoryRavenIntroV21(){
   if(factoryRavenIntroConfiguredV21||room!=0||elapsed!=0||rooms.Count!=3||factoryEntryBirthsV542==null||factoryEntryRoomsV542==null||factoryEntryRoomsV542.Length!=3||factoryEntryBirthsV542.Length!=64||!NativeTerrain)throw new InvalidOperationException("Factory Raven owner requires the fresh full original Factory component before its first ordinary Step");
   for(int i=0;i<3;i++)if(!ReferenceEquals(rooms[i],factoryEntryRoomsV542[i])||rooms[i].id!=FactoryRouteV542+"-"+i)throw new InvalidOperationException("Factory Raven owner cannot bind replaced or foreign rooms");
   var birth=Array.Find(factoryEntryBirthsV542,b=>b.room==2&&b.order==0);var stage=rooms[2];
   if(birth==null||birth.npc!="NUI_RAVEN_CLONE"||birth.kind!="raven-clone"||birth.encounter!=0||birth.sourceKeyCode!=0||birth.role!="boss-combat-stop"||birth.sourceActiveDeclaration!="ABSENT"||!birth.sourceStopAtStart||birth.initialState!="RAVEN_CLONE_START"||birth.initialAnimation!="WaitStart"||birth.initialDuration!=3.8f||stage.enemies.Count!=1||stage.encounters==null||stage.encounters.Length!=1||stage.encounters[0].id!="2:0"||stage.encounters[0].sourceWorldId!="WI_ALTERA_NASOD_FACTORY_BOSS")throw new InvalidOperationException("Factory original Raven birth/STOP identity differs");
   var spawn=stage.enemies[0];if(spawn.sourceNPC!=birth.npc||spawn.kind!=birth.kind||spawn.encounterIndex!=0||spawn.sourceKeyCode!=0||spawn.aiEnabled||!spawn.countsForClear||spawn.isGate||spawn.passive)throw new InvalidOperationException("Factory Raven source staging/clear flags differ");
   NpcDefinition profile;NpcStateDefinition start,wait;NpcStateSoundTrack sound;
   if(!npcDefinitions.TryGetValue(birth.npc,out profile)||profile.kind!=birth.kind||!npcStates.TryGetValue(birth.npc+"/RAVEN_CLONE_START",out start)||!npcStates.TryGetValue(birth.npc+"/RAVEN_CLONE_WAIT",out wait)||start.sourceAnimation!="WaitStart"||start.duration!=3.8f||start.sourceAnimSpeed!=1||start.loop||start.nextState!="RAVEN_CLONE_WAIT"||start.hits.Length!=0||start.velocities.Length!=0||start.entryOffsetY!=0||!npcStateSounds.TryGetValue(birth.npc+"/RAVEN_CLONE_START",out sound)||sound.events.Length!=2)throw new InvalidOperationException("Factory original START/motion/audio binding is unavailable");
   if(sound.events[0].candidateTime!=1.797f||sound.events[0].sound!="factory-npc-raven_dashattack"||sound.events[0].sourceName!="Raven_DashAttack.ogg"||sound.events[0].sourceField!="SOUND_PLAY0"||sound.events[1].candidateTime!=3.078f||sound.events[1].sound!="factory-npc-raven_punchattack1"||sound.events[1].sourceName!="Raven_PunchAttack1.ogg"||sound.events[1].sourceField!="SOUND_PLAY1")throw new InvalidOperationException("Factory original START sound cues differ");
   factoryRavenStartV21=start;factoryRavenWaitV21=wait;factoryRavenProfileV21=profile;factoryRavenIntroConfiguredV21=true;factoryRavenIntroGenerationV21++;
  }
  bool FactoryRavenFocusCurrentV21(FactoryRavenFocusOwnerV21 owner){return factoryRavenIntroConfiguredV21&&owner!=null&&ReferenceEquals(owner,factoryRavenFocusOwnerV21)&&ReferenceEquals(owner.model,this)&&!owner.ended;}
  public FactoryRavenFocusOwnerV21 RegisterFactoryRavenFocusOwnerV21(object actualViewOwner){
   if(!factoryRavenIntroConfiguredV21||actualViewOwner==null)throw new InvalidOperationException("A configured Factory Raven model and actual view identity are required");
   if(FactoryRavenFocusCurrentV21(factoryRavenFocusOwnerV21))throw new InvalidOperationException("End the prior Factory focus owner before replacing it");
   return factoryRavenFocusOwnerV21=new FactoryRavenFocusOwnerV21(this,actualViewOwner,++factoryRavenFocusSerialV21);
  }
  bool FactoryRavenSourceBirthV21(Enemy actor){
   return factoryRavenIntroConfiguredV21&&room==2&&ReferenceEquals(CurrentRoom,factoryEntryRoomsV542[2])&&CurrentRoom.id==FactoryRouteV542+"-2"&&HasEncounters&&EncounterIndex==0&&CurrentEncounter.id=="2:0"&&CurrentEncounter.sourceWorldId=="WI_ALTERA_NASOD_FACTORY_BOSS"&&actor!=null&&enemies.Count==1&&ReferenceEquals(enemies[0],actor)&&actor.sourceNPC=="NUI_RAVEN_CLONE"&&actor.kind=="raven-clone"&&actor.encounterIndex==0&&actor.sourceKeyCode==0&&!actor.isGate&&!actor.passive&&actor.countsForClear;
  }
  bool FactoryRavenStateBindingV21(){NpcDefinition profile;NpcStateDefinition start,wait;return npcDefinitions.TryGetValue("NUI_RAVEN_CLONE",out profile)&&ReferenceEquals(profile,factoryRavenProfileV21)&&npcStates.TryGetValue("NUI_RAVEN_CLONE/RAVEN_CLONE_START",out start)&&ReferenceEquals(start,factoryRavenStartV21)&&npcStates.TryGetValue("NUI_RAVEN_CLONE/RAVEN_CLONE_WAIT",out wait)&&ReferenceEquals(wait,factoryRavenWaitV21);}
  bool FactoryRavenTicketContextV21(FactoryRavenRuntimeV21 runtime){
   var ticket=runtime.ticket;var actor=ticket.actor;
   return FactoryRavenFocusCurrentV21(ticket.owner)&&ticket.generation==factoryRavenIntroGenerationV21&&FactoryRavenSourceBirthV21(actor)&&FactoryRavenStateBindingV21()&&ReferenceEquals(actor,factoryRavenPreparedActorV21)&&!ReferenceEquals(actor,factoryRavenReleasedActorV21)&&actor.active&&!actor.dormant&&actor.hp>0&&!actor.aiEnabled&&actor.npcState==ticket.InitialState&&actor.npcTime==(float)runtime.time&&actor.clock==(float)runtime.time;
  }
  bool FactoryRavenTicketPendingV21(FactoryRavenFocusOwnerV21 owner,FactoryRavenIntroTicketV21 ticket,out FactoryRavenRuntimeV21 runtime){
   runtime=factoryRavenRuntimeV21;return ticket!=null&&runtime!=null&&ReferenceEquals(runtime.ticket,ticket)&&ReferenceEquals(ticket.model,this)&&ReferenceEquals(ticket.owner,owner)&&(runtime.phase==CoreStopIntroPhaseV21.AwaitingFocus||runtime.phase==CoreStopIntroPhaseV21.PlayingSourceStart)&&FactoryRavenTicketContextV21(runtime);
  }
  public FactoryRavenIntroTicketV21 CurrentFactoryRavenIntroTicketV21(){var runtime=factoryRavenRuntimeV21;return runtime!=null&&runtime.ticket.generation==factoryRavenIntroGenerationV21&&FactoryRavenSourceBirthV21(runtime.ticket.actor)?runtime.ticket:null;}
  public FactoryRavenIntroStatusV21 FactoryRavenIntroStatusV21(FactoryRavenIntroTicketV21 ticket){var runtime=factoryRavenRuntimeV21;if(ticket==null||runtime==null||!ReferenceEquals(ticket.model,this)||!ReferenceEquals(runtime.ticket,ticket))return null;return new FactoryRavenIntroStatusV21(ticket,runtime.phase,runtime.reason,(float)runtime.time,runtime.releases,runtime.accepted);}
  // Observation submission never enters a state or changes AI/HP/source clocks.
  // Only a later ordinary actor Step may consume this same-owner observation.
  public bool QueueFactoryRavenFocusObservationV21(FactoryRavenFocusOwnerV21 owner,FactoryRavenIntroTicketV21 ticket,object observedViewOwner,bool visible,bool focused){
   FactoryRavenRuntimeV21 runtime;if(!visible||!focused||owner==null||!ReferenceEquals(owner.view,observedViewOwner)||!FactoryRavenTicketPendingV21(owner,ticket,out runtime)||runtime.queuedAck||runtime.accepted||runtime.queuedFailure!=null)return false;runtime.queuedAck=true;runtime.queuedAt=elapsed;return true;
  }
  public bool QueueFactoryRavenFocusFailureV21(FactoryRavenFocusOwnerV21 owner,FactoryRavenIntroTicketV21 ticket,string reason){FactoryRavenRuntimeV21 runtime;if(string.IsNullOrEmpty(reason)||!FactoryRavenTicketPendingV21(owner,ticket,out runtime)||runtime.queuedFailure!=null)return false;runtime.queuedFailure=reason;runtime.queuedAt=elapsed;return true;}
  void CancelFactoryRavenIntroV21(string reason,bool failed){var runtime=factoryRavenRuntimeV21;if(runtime==null||runtime.phase==CoreStopIntroPhaseV21.Released||runtime.phase==CoreStopIntroPhaseV21.Cancelled||runtime.phase==CoreStopIntroPhaseV21.Failed)return;runtime.phase=failed?CoreStopIntroPhaseV21.Failed:CoreStopIntroPhaseV21.Cancelled;runtime.reason=reason;runtime.queuedAck=false;runtime.queuedFailure=null;}
  public bool EndFactoryRavenFocusOwnerV21(FactoryRavenFocusOwnerV21 owner){if(!FactoryRavenFocusCurrentV21(owner))return false;owner.ended=true;CancelFactoryRavenIntroV21("presentation-owner-End; AI remains held",false);factoryRavenIntroGenerationV21++;return true;}
  void BeginFactoryRavenIntroRoomV21(){if(!factoryRavenIntroConfiguredV21)return;CancelFactoryRavenIntroV21("room/reset generation changed",false);factoryRavenRuntimeV21=null;factoryRavenPreparedActorV21=null;factoryRavenReleasedActorV21=null;factoryRavenIntroGenerationV21++;}
  // The original activation hook owns this single birth's initial state.
  // It changes neither authored HP/position/flags nor the source eight docs.
  void PrepareFactoryRavenIntroBirthV21(Enemy actor){
   if(!FactoryRavenSourceBirthV21(actor)||!FactoryRavenStateBindingV21()||actor.hp<=0||actor.hp!=actor.maxHp||actor.clock!=0||actor.npcTime!=0||actor.deathTime!=0||actor.hitPause!=0||actor.aiEnabled||!string.IsNullOrEmpty(actor.npcState)||actor.state==CombatState.Hurt||actor.state==CombatState.Knockdown||actor.state==CombatState.GettingUp)throw new InvalidOperationException("Factory Raven START belongs only to the original fresh boss's first ordinary Step");
   float hp=actor.hp,maxHp=actor.maxHp,x=actor.x,y=actor.y,depth=actor.nativeDepth;EnterNpcState(actor,factoryRavenStartV21);
   if(actor.hp!=hp||actor.maxHp!=maxHp||actor.x!=x||actor.y!=y||actor.nativeDepth!=depth)throw new InvalidOperationException("Factory Raven initialization changed birth HP/position");
   factoryRavenPreparedActorV21=actor;
   if(SimpleDangerIntroductions){factoryRavenReleasedActorV21=actor;actor.aiEnabled=true;}
  }
  // Covers the room-entry-to-first-actor-Step gap too, through the existing
  // EnemyHasNpcInvincibility entry used by ordinary commands, skills and FX.
  bool FactoryRavenIntroOpeningProtectedV21(Enemy actor){return FactoryRavenSourceBirthV21(actor)&&actor.hp>0&&!ReferenceEquals(actor,factoryRavenReleasedActorV21);}
  bool TickFactoryRavenIntroOwnerV21(Enemy actor,float dt){
   if(!factoryRavenIntroConfiguredV21||!ReferenceEquals(actor,factoryRavenPreparedActorV21)||ReferenceEquals(actor,factoryRavenReleasedActorV21))return false;
   actor.aiEnabled=false;
   var runtime=factoryRavenRuntimeV21;
   if(!FactoryRavenSourceBirthV21(actor)||!actor.active||actor.dormant||actor.hp<=0){CancelFactoryRavenIntroV21("dead/foreign/room/encounter actor",false);return true;}
   if(runtime==null){
    if(!FactoryRavenFocusCurrentV21(factoryRavenFocusOwnerV21))return true;
    runtime=new FactoryRavenRuntimeV21{ticket=new FactoryRavenIntroTicketV21(this,actor,factoryRavenFocusOwnerV21,++factoryRavenIntroSerialV21,factoryRavenIntroGenerationV21)};factoryRavenRuntimeV21=runtime;
   }
   if(runtime.lastElapsed==elapsed)return true;runtime.lastElapsed=elapsed;
   if(runtime.phase==CoreStopIntroPhaseV21.Cancelled||runtime.phase==CoreStopIntroPhaseV21.Failed)return true;
   if(!FactoryRavenTicketContextV21(runtime)){CancelFactoryRavenIntroV21("exclusive START or same presentation owner lost",false);return true;}
   IntegrateBody(actor,dt,false);
   if(runtime.queuedFailure!=null&&runtime.queuedAt<elapsed){CancelFactoryRavenIntroV21(runtime.queuedFailure,true);return true;}
   if(runtime.queuedAck&&runtime.queuedAt<elapsed){runtime.queuedAck=false;runtime.accepted=true;runtime.phase=CoreStopIntroPhaseV21.PlayingSourceStart;runtime.reason="same-owner actual visible focus accepted by ordinary Step";}
   if(runtime.phase!=CoreStopIntroPhaseV21.PlayingSourceStart||!runtime.accepted)return true;
   float from=(float)runtime.time;double next=Math.Min((double)factoryRavenStartV21.duration,runtime.time+(double)dt);float to=(float)next;
   runtime.time=next;actor.npcTime=to;actor.clock=to;AdvanceNpcStateSounds(actor,factoryRavenStartV21,from,to,elapsed-dt);
   if(next<(double)factoryRavenStartV21.duration)return true;
   if(!FactoryRavenTicketContextV21(runtime)){CancelFactoryRavenIntroV21("completion ticket invalid",false);return true;}
   EnterNpcState(actor,factoryRavenWaitV21);factoryRavenReleasedActorV21=actor;actor.aiEnabled=true;runtime.releases++;runtime.phase=CoreStopIntroPhaseV21.Released;runtime.reason="original SCT_MOTION_END to WAIT; source ACTIVE released once";
   // No leftover dt or generic WAIT AI runs on this release packet.
   return true;
  }
 }
}
