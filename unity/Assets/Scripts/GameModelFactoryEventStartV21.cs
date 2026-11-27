using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable]public sealed class FactoryEventStartBindingV21 {
  public string schema,route,roomId,world,npc,sourceLuaSha256,sourceMotionSha256,sourceFrameSha256,sourceStartSha256,sourceDyingSha256,clockAdapter;
  public bool enabled,nativeExecuted,nativeEquivalent,runtimeAdmitted,finalBalanceApproved;
  public float readySeconds,startMotionSeconds,dyingMotionSeconds,sourceMaxHp;
 }
 public sealed class FactoryEventStartFrameV21 {
  public readonly Enemy actualOwner;public readonly int ownerUid;public readonly string state,sourceAnimation;public readonly float stateAge,sourcePoseTime;public readonly bool invincible,nativePositionPending,nativeCameraPending,nativeDyingEndPending;
  internal FactoryEventStartFrameV21(Enemy e,int uid,string s,double age){actualOwner=e;ownerUid=uid;state=s;stateAge=(float)age;sourceAnimation=s=="EVENT_NPC_NASOD_FACTORY_START"?"Start":"Dying";sourcePoseTime=s=="EVENT_NPC_NASOD_FACTORY_START_READY"?(float)(age%(11d/30)):(float)age;invincible=age<100;nativePositionPending=s!="EVENT_NPC_NASOD_FACTORY_START_READY";nativeCameraPending=s!="EVENT_NPC_NASOD_FACTORY_START_READY";nativeDyingEndPending=s=="EVENT_NPC_NASOD_FACTORY_DYING"&&age>=11d/30;}
 }
 public sealed class FactoryEventStartRequestV21 {
  public readonly Enemy actualOwner;public readonly int ownerUid,serial,intArgument;public readonly string kind,state,asset,callback;public readonly float at,x,y,z;public readonly bool nativeExecuted=false;
  internal FactoryEventStartRequestV21(Enemy owner,int uid,int seq,string k,string s,string a,string cb,float time,float px=0,float py=0,float pz=0,int argument=0){actualOwner=owner;ownerUid=uid;serial=seq;kind=k;state=s;asset=a;callback=cb;at=time;x=px;y=py;z=pz;intArgument=argument;}
 }
 public sealed partial class GameModel {
  const string FactoryEventNpcV21="NUI_EVENT_NPC_NASOD_FACTORY",FactoryEventReadyV21="EVENT_NPC_NASOD_FACTORY_START_READY",FactoryEventStartV21="EVENT_NPC_NASOD_FACTORY_START",FactoryEventDyingV21="EVENT_NPC_NASOD_FACTORY_DYING";
  FactoryEventStartBindingV21 factoryEventBindingV21;
  sealed class FactoryEventActorV21 {internal int uid;internal string state;internal double age;internal bool dyingEndRequested;}
  readonly Dictionary<Enemy,FactoryEventActorV21> factoryEventActorsV21=new Dictionary<Enemy,FactoryEventActorV21>();
  readonly List<FactoryEventStartRequestV21> factoryEventRequestsV21=new List<FactoryEventStartRequestV21>();int factoryEventOwnerSerialV21,factoryEventRequestSerialV21;
  public void SetFactoryEventStartBindingV21(FactoryEventStartBindingV21 r){
   if(r!=null&&(r.schema!="factory-event-no-brain-start-finite-2d-v21-r1"||!r.enabled||r.nativeExecuted||r.nativeEquivalent||r.runtimeAdmitted||r.finalBalanceApproved||r.route!="Dungeon_Altera_Nasod_Factory_Normal"||r.roomId!=r.route+"-0"||r.world!="WI_ALTERA_NASOD_FACTORY_STAGE1"||r.npc!=FactoryEventNpcV21||r.sourceLuaSha256!="4e263fcbfde3ba88b1dda0095b74116c98bb5984d4d75a6af7cc02e3eab14354"||r.sourceMotionSha256!="16f0507cd0496bc1a71339bbec398cbf0688a06df4a36a3c60e2803a9f40a61a"||r.sourceStartSha256!="53abb2ab1ba6872c762983f2902fe77375241208bdc35da4556f4567bd303d19"||r.sourceFrameSha256!="25c09e72f8f2e6ccc4872e17ecb3a61926735950224f41f5cf44b12e41f808e7"||r.sourceDyingSha256!="9b7f0c6a488a80661f886f8462ef8f21390b6d1685263b833b64315de8cd772e"||r.readySeconds!=5||r.startMotionSeconds!=11.6f||r.dyingMotionSeconds!=11f/30||r.sourceMaxHp!=30000||r.clockAdapter!="ordinary-Step-threshold-crossing-with-remainder-finite2D-v21"))throw new ArgumentException("Changed exact Factory event source or finite2D boundary");
   // Configuration is not an ACK or state reset. The same actual Enemy UID
   // retains its consumed thresholds across disable/re-enable or rebinding.
   factoryEventBindingV21=r;
  }
  bool FactoryEventScopeV21(){var r=factoryEventBindingV21;return r!=null&&room==0&&CurrentRoom.id==r.roomId&&HasEncounters&&CurrentEncounter.sourceWorldId==r.world;}
  bool FactoryEventSourceInvincibleV21(Enemy e){if(!FactoryEventScopeV21()||e==null||!enemies.Contains(e)||e.sourceNPC!=FactoryEventNpcV21||e.encounterIndex!=0||!e.active||e.dormant||e.hp<=0)return false;FactoryEventActorV21 a;return !factoryEventActorsV21.TryGetValue(e,out a)||a.age<100;}
  void FactoryEventRequestV21(Enemy e,FactoryEventActorV21 a,string kind,string asset,string callback,float at,float x=0,float y=0,float z=0,int argument=0){if(factoryEventRequestsV21.Count>=256)factoryEventRequestsV21.RemoveAt(0);factoryEventRequestsV21.Add(new FactoryEventStartRequestV21(e,a.uid,++factoryEventRequestSerialV21,kind,a.state,asset,callback,at,x,y,z,argument));}
  void EnterFactoryEventStateV21(Enemy e,FactoryEventActorV21 a,string state,float at){a.state=state;a.age=0;e.npcState=state;e.npcTime=0;e.clock=0;FactoryEventRequestV21(e,a,"source-state",state,"INIT_STATE",at);
   if(state==FactoryEventStartV21){FactoryEventRequestV21(e,a,"native-position-request","SetPositionForce","EVENT_NPC_NASOD_FACTORY_STATE_START",at);FactoryEventRequestV21(e,a,"native-parts-camera-request","Dummy_Camera01->Dummy_Camera_Target","CAMERA.SUB_CAMERA0",at,argument:20);}
   else if(state==FactoryEventDyingV21)FactoryEventRequestV21(e,a,"native-position-request","SetPositionForce","EVENT_NPC_NASOD_FACTORY_DYING_START_START",at,y:-5000);
  }
  void AdvanceFactoryEventSourceRequestsV21(Enemy e,FactoryEventActorV21 a,double from,double to,float packetAt){
   // Explicit finite2D interpretation of the shipped AnimEventTimer call.
   // Native boolean opcode/clock ABI and actual localized text are still HOLD.
   float[] times={.1f,1.5f,3.5f,6.4f,7,8.93f};string[] assets={"AlteraCutScene_LiftMove.ogg","STR_ID_31535","AlteraCutScene_RobotFall.ogg","AlteraCutScene_RobotAssemble.ogg","STR_ID_31536","AlteraCutScene_RobotAssemble.ogg"};
   for(int i=0;i<times.Length;i++)if(from<times[i]&&times[i]<=to){bool description=i==1||i==4;FactoryEventRequestV21(e,a,description?"native-description-request":"source-sound-request",assets[i],description?"EVENT_NPC_NASOD_FACTORY_FRAME_MOVE":"SOUND_PLAY",packetAt+(float)(times[i]-from),argument:description?(i==1?3:2):100);}
  }
  bool TickFactoryEventStartV21(Enemy e,float dt){
   if(SimpleDangerIntroductions&&e!=null&&e.sourceNPC==FactoryEventNpcV21){e.dormant=true;return true;}
   if(factoryEventActorsV21.Count>0){var retired=new List<Enemy>();foreach(var old in factoryEventActorsV21.Keys)if(!enemies.Contains(old))retired.Add(old);foreach(var old in retired)factoryEventActorsV21.Remove(old);}
   if(!FactoryEventScopeV21()||e.sourceNPC!=FactoryEventNpcV21||e.encounterIndex!=0)return false;
   if(e.sourceKeyCode!=0||e.maxHp!=30000||e.aiEnabled||e.countsForClear||e.passive||e.isGate)throw new InvalidOperationException("Changed original NO_BRAIN Factory director identity/HP/staging/clear flags");
   if(!e.active||e.dormant||e.hp<=0)return true;
   FactoryEventActorV21 a;if(!factoryEventActorsV21.TryGetValue(e,out a)){a=new FactoryEventActorV21{uid=++factoryEventOwnerSerialV21};factoryEventActorsV21.Add(e,a);EnterFactoryEventStateV21(e,a,FactoryEventReadyV21,elapsed-dt);}
   if(e.hitPause>0){float stopped=Math.Min(dt,e.hitPause);e.hitPause-=stopped;dt-=stopped;if(dt<=.000001f)return true;}
   e.vx=e.vy=0;double left=dt;float packetAt=elapsed-dt;
   while(left>1e-9){double duration=a.state==FactoryEventReadyV21?5:a.state==FactoryEventStartV21?11.6:11d/30;double from=a.age,consume=Math.Min(left,Math.Max(0,duration-from));double to=from+consume;
    if(a.state==FactoryEventStartV21)AdvanceFactoryEventSourceRequestsV21(e,a,from,to,packetAt);
    a.age=to;e.npcTime=(float)to;e.clock+=((float)consume);left-=consume;packetAt+=(float)consume;
    if(to+1e-9<duration)break;
    if(a.state==FactoryEventReadyV21){EnterFactoryEventStateV21(e,a,FactoryEventStartV21,packetAt);continue;}
    if(a.state==FactoryEventStartV21){EnterFactoryEventStateV21(e,a,FactoryEventDyingV21,packetAt);continue;}
    if(!a.dyingEndRequested){a.dyingEndRequested=true;FactoryEventRequestV21(e,a,"native-dying-end-request","DYING_END","EVENT_NPC_NASOD_FACTORY_DYING",packetAt);}
    // Native retirement/SetPositionForce and camera were not executed. Keep
    // the source Dying pose as a visible HOLD; never HP=0 or clear the room.
    break;
   }
   return true;
  }
  public FactoryEventStartFrameV21[] ReadFactoryEventStartFramesV21(){var result=new List<FactoryEventStartFrameV21>();if(FactoryEventScopeV21())foreach(var pair in factoryEventActorsV21)if(enemies.Contains(pair.Key)&&pair.Key.active&&!pair.Key.dormant)result.Add(new FactoryEventStartFrameV21(pair.Key,pair.Value.uid,pair.Value.state,pair.Value.age));return result.ToArray();}
  public FactoryEventStartRequestV21[] ReadFactoryEventStartRequestsV21(){return factoryEventRequestsV21.ToArray();}
 }
}
