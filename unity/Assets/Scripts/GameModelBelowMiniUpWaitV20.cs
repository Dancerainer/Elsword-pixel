using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class BelowMiniUpStateClockV20 {public string state,clip,sourceAnimation;public float duration,sourceAnimSpeed;public bool loop;}
 [Serializable] public sealed class BelowMiniUpConditionV20 {public int sourceIndex,eventIntervalId;public string condition,targetState;public float rate;}
 [Serializable] public sealed class BelowMiniUpWaitDocumentV20 {
  public string schema,npc,state,sourceRulesSha256,sourceBodySha256,sourceWaitTableSha256,schedulerAdapter,rateAdapter;
  public bool runtimeReady,runtimeAdmitted,nativeExecuted;public float interval;
  public BelowMiniUpStateClockV20[] clocks;public BelowMiniUpConditionV20[] conditions;
 }
 [Serializable] public sealed class BelowMiniUpWaitEventV20 {public string type,state,target;public int ownerSerial,index;public float at,rate,roll;public double age,nextPoll;public bool accepted;}
 public sealed partial class GameModel {
  const string BelowMiniUpNpcV20="NUI_CANNON_MINI_UP",BelowMiniUpWaitStateV20="CANNON_MINI_UP_WAIT";
  const string BelowMiniUpRulesHashV20="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb";
  const string BelowMiniUpBodyHashV20="3ce7e57c42c731f3cd8dfaed4ffd66785a5fad52d40d0d4645a6c3fa1bd5b991",BelowMiniUpWaitHashV20="88bd7f9dc68215e4bcbf416ebd9cc1d0b9623b9de0507eb35a6841e7bef6739b";
  const string BelowMiniUpSchedulerV20="candidate-entry-age-first2-fixed2-poll-generic-core-eligibility-v20";
  const string BelowMiniUpRateV20="candidate-ordered-independent-NpcRoll-less-than-source-RATE-v20";
  sealed class BelowMiniUpWaitActorV20 {public double age,nextPoll=2;public float enteredAt;public bool entryObserved,holdLogged;}
  bool belowMiniUpWaitEnabledV20;
  readonly Dictionary<Enemy,BelowMiniUpWaitActorV20> belowMiniUpWaitActorsV20=new Dictionary<Enemy,BelowMiniUpWaitActorV20>();
  public Func<Enemy,int,float> BelowMiniUpRateReplayV20;
  public readonly List<BelowMiniUpWaitEventV20> belowMiniUpWaitEventsV20=new List<BelowMiniUpWaitEventV20>();
  public bool BelowMiniUpWaitEnabledV20 {get{return belowMiniUpWaitEnabledV20;}}
  public int BelowMiniUpWaitActorCountV20 {get{return belowMiniUpWaitActorsV20.Count;}}
  static bool EmptyBelowMiniUpV20(BelowMiniUpWaitDocumentV20 d){return d!=null&&string.IsNullOrEmpty(d.schema)&&string.IsNullOrEmpty(d.npc)&&string.IsNullOrEmpty(d.state)&&string.IsNullOrEmpty(d.sourceRulesSha256)&&string.IsNullOrEmpty(d.sourceBodySha256)&&string.IsNullOrEmpty(d.sourceWaitTableSha256)&&string.IsNullOrEmpty(d.schedulerAdapter)&&string.IsNullOrEmpty(d.rateAdapter)&&!d.runtimeReady&&!d.runtimeAdmitted&&!d.nativeExecuted&&d.interval==0&&(d.clocks==null||d.clocks.Length==0)&&(d.conditions==null||d.conditions.Length==0);}
  public void SetNpcBelowMiniUpWaitV20(BelowMiniUpWaitDocumentV20 d){
   if(EmptyBelowMiniUpV20(d))d=null;
   if(d!=null){
    if(d.schema!="below-tunnel-mini-up-wait-source-draft-v20-r2"||d.npc!=BelowMiniUpNpcV20||d.state!=BelowMiniUpWaitStateV20||d.sourceRulesSha256!=BelowMiniUpRulesHashV20||d.sourceBodySha256!=BelowMiniUpBodyHashV20||d.sourceWaitTableSha256!=BelowMiniUpWaitHashV20||d.schedulerAdapter!=BelowMiniUpSchedulerV20||d.rateAdapter!=BelowMiniUpRateV20||d.runtimeReady||d.runtimeAdmitted||d.nativeExecuted||d.interval!=2||d.conditions==null||d.conditions.Length!=3||d.clocks==null||d.clocks.Length!=7)throw new ArgumentException("Changed exact MiniUP WAIT source draft");
    var expected=new[]{"CANNON_MINI_UP_START","CANNON_MINI_UP_WAIT","CANNON_MINI_UP_ATTACK_1","CANNON_MINI_UP_ATTACK_2","CANNON_MINI_UP_ATTACK_3","CANNON_MINI_UP_DAMAGE_SMALL","CANNON_MINI_UP_DYING_LAND_FRONT"};var keys=new HashSet<string>();foreach(var c in d.clocks){NpcStateDefinition s;if(c==null||Array.IndexOf(expected,c.state)<0||!keys.Add(c.state)||!npcStates.TryGetValue(d.npc+"/"+c.state,out s)||s.clip!=c.clip||s.sourceAnimation!=c.sourceAnimation||s.duration!=c.duration||s.sourceAnimSpeed!=c.sourceAnimSpeed||s.loop!=c.loop)throw new ArgumentException("Changed MiniUP original state/clip/clock");}
    for(int i=0;i<3;i++){var c=d.conditions[i];string target="CANNON_MINI_UP_ATTACK_"+(i+1);if(c==null||c.sourceIndex!=i+1||c.eventIntervalId!=0||c.condition!="CT_"+target||c.targetState!=target||c.rate!=(i+1)*20)throw new ArgumentException("Changed MiniUP ordered independent source RATE");}
    var wait=npcStates[d.npc+"/"+d.state];if(!wait.loop||wait.duration!=1||wait.sourceAnimation!="Wait"||wait.interval!=2||wait.choices.Length!=0||wait.nextState!=d.state)throw new ArgumentException("MiniUP sidecar requires exact unprojected source WAIT");
   }
   belowMiniUpWaitEnabledV20=d!=null;ClearBelowMiniUpWaitRoomV20();
  }
  void ClearBelowMiniUpWaitRoomV20(){belowMiniUpWaitActorsV20.Clear();belowMiniUpWaitEventsV20.Clear();BelowMiniUpRateReplayV20=null;}
  void BelowMiniUpWaitEventV20For(string type,Enemy e,BelowMiniUpWaitActorV20 a,int index=0,float rate=0,float roll=0,string target=null,bool accepted=false){if(belowMiniUpWaitEventsV20.Count>=2048)belowMiniUpWaitEventsV20.RemoveAt(0);belowMiniUpWaitEventsV20.Add(new BelowMiniUpWaitEventV20{type=type,state=e.npcState,target=target??"",ownerSerial=NpcSoundActorId(e),at=elapsed,index=index,rate=rate,roll=roll,age=a==null?0:a.age,nextPoll=a==null?0:a.nextPoll,accepted=accepted});}
  void OnBelowMiniUpWaitExitV20(Enemy e,NpcStateDefinition state){if(e!=null&&e.sourceNPC==BelowMiniUpNpcV20&&state!=null&&state.id==BelowMiniUpWaitStateV20)belowMiniUpWaitActorsV20.Remove(e);}
  void OnBelowMiniUpWaitEnteredV20(Enemy e,NpcStateDefinition state){
   if(!belowMiniUpWaitEnabledV20||e==null||e.sourceNPC!=BelowMiniUpNpcV20||state==null)return;
   if(state.id!=BelowMiniUpWaitStateV20){belowMiniUpWaitActorsV20.Remove(e);return;}
   var a=new BelowMiniUpWaitActorV20{enteredAt=elapsed,entryObserved=true};belowMiniUpWaitActorsV20[e]=a;BelowMiniUpWaitEventV20For("source-WAIT-entry",e,a);
  }
  bool TickBelowMiniUpWaitV20(Enemy e,float dt){
   if(!belowMiniUpWaitEnabledV20||e==null||e.sourceNPC!=BelowMiniUpNpcV20||e.npcState!=BelowMiniUpWaitStateV20)return false;
   if(!e.active||e.dormant||e.passive||e.isGate||!e.aiEnabled||e.hp<=0||e.state!=CombatState.Normal||e.stun>0)return false;
   BelowMiniUpWaitActorV20 a;if(!belowMiniUpWaitActorsV20.TryGetValue(e,out a)){a=new BelowMiniUpWaitActorV20();belowMiniUpWaitActorsV20[e]=a;}
   var state=NpcState(e);float clock=e.clock;AdvancePassagePose(e,state,dt,elapsed-dt);e.clock=clock; // Core already advanced clock; preserve once-per-step age.
   if(!a.entryObserved){if(!a.holdLogged){a.holdLogged=true;BelowMiniUpWaitEventV20For("HOLD-missing-original-WAIT-entry-origin",e,a);}return true;}
   a.age+=dt;
   while(a.age>=a.nextPoll){a.nextPoll+=2;BelowMiniUpWaitEventV20For("candidate-source-interval0-poll",e,a);
    for(int i=1;i<=3;i++){float rate=i*20,roll=BelowMiniUpRateReplayV20==null?NpcRoll():BelowMiniUpRateReplayV20(e,i);if(!Finite(roll)||roll<0||roll>=100)throw new InvalidOperationException("MiniUP candidate RATE replay must be finite0inclusive..100exclusive");BelowMiniUpWaitEventV20For("candidate-independent-rate",e,a,i,rate,roll);
     if(roll>=rate)continue;string target="CANNON_MINI_UP_ATTACK_"+i;bool accepted=TryRequestNpcState(e,target,true)&&CommitNpcStateRequest(e);BelowMiniUpWaitEventV20For(accepted?"source-StateChange-committed":"source-StateChange-rejected",e,a,i,rate,roll,target,accepted);if(accepted)return true;
    }
   }
   return true;
  }
 }
}
