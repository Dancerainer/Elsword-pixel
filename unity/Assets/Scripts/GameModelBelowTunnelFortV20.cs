using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ElswordPixel {
 [Serializable] public sealed class BelowFortEventRuleV20 {
  public int sourceIndex,eventInterval0;public string type,target,condition;
  public float stateTimeOver,rate,nearNative,timerElapsed0;
 }
 [Serializable] public sealed class BelowFortStateRuleV20 {
  public string id,sourceAnimation;public float duration,eventIntervalSeconds;public bool loop;
  public BelowFortEventRuleV20[] events;
 }
 [Serializable] public sealed class BelowFortSpawnRuleV20 {
  public string npc;public int callPC,startNumber,groundLine,sourceGetStartPC,rawLastInteger;
  public float x,y,nativeDepth,rawTailScalar;public float[] nativeXYZ;public bool sourceFacingRight,sourceMaxHpAbsent;
 }
 [Serializable] public sealed class BelowFortPatternV20 {
  public string state,callback,sourceInstructionsSha256;public float animationAt;
  public bool clearPriorMini,onlyMissingPerType,restartTimer0;public BelowFortSpawnRuleV20[] spawns;
 }
 [Serializable] public sealed class BelowFortSourceDocumentV20 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,npc,sourceRoomId,sourceWorldId,initialState;
  public bool runtimeReady,runtimeAdmitted,nativeExecuted;public float unit;public int armorKey,bodyClearKey;
  public BelowFortStateRuleV20[] states;public BelowFortPatternV20[] patterns;
 }
 [Serializable] public sealed class BelowFortCreateRequestV20 {
  public long serial;public Enemy owner;public string npc,sourceCallback;
  public int room,encounter,startNumber,line,rawLastInteger;public float at,due,x,y,nativeDepth,rawTailScalar;
  public float[] nativeXYZ;
 }
 public sealed partial class GameModel {
  BelowFortSourceDocumentV20 belowFortV20;
  sealed class BelowFortActorV20 {internal float age,timer0At,int0At,nextPoll;internal bool initialized,issued,calledPart;}
  readonly Dictionary<Enemy,BelowFortActorV20> belowFortActorsV20=new Dictionary<Enemy,BelowFortActorV20>();
  readonly List<BelowFortCreateRequestV20> belowFortRequestsV20=new List<BelowFortCreateRequestV20>();
  long belowFortCreateSerialV20;
  const string BelowFortSemanticReceiptV20="76c82f53c771e30806d00094b304f188ab0358e232745a08e8fc4cce8cd3cf5d";
  static string FortBitsV20(float f){return BitConverter.ToString(BitConverter.GetBytes(f)).Replace("-","").ToLowerInvariant();}
  static string FortBoolV20(bool b){return b?"1":"0";}
  static string FortSemanticReceiptV20(BelowFortSourceDocumentV20 doc){
   var r=new List<string>{doc.schema,doc.sourceRulesSha256,doc.sourceEffectsSha256,doc.npc,doc.sourceRoomId,doc.sourceWorldId,doc.initialState,FortBitsV20(doc.unit),doc.armorKey.ToString(),doc.bodyClearKey.ToString()};
   foreach(var s in doc.states){r.AddRange(new[]{s.id,s.sourceAnimation,FortBitsV20(s.duration),FortBitsV20(s.eventIntervalSeconds),FortBoolV20(s.loop)});foreach(var e in s.events)r.AddRange(new[]{e.sourceIndex.ToString(),e.eventInterval0.ToString(),e.type,e.target,e.condition,FortBitsV20(e.stateTimeOver),FortBitsV20(e.rate),FortBitsV20(e.nearNative),FortBitsV20(e.timerElapsed0)});}
   foreach(var p in doc.patterns){r.AddRange(new[]{p.state,p.callback,p.sourceInstructionsSha256,FortBitsV20(p.animationAt),FortBoolV20(p.clearPriorMini),FortBoolV20(p.onlyMissingPerType),FortBoolV20(p.restartTimer0)});foreach(var s in p.spawns)r.AddRange(new[]{s.npc,s.callPC.ToString(),s.startNumber.ToString(),s.groundLine.ToString(),s.sourceGetStartPC.ToString(),s.rawLastInteger.ToString(),FortBitsV20(s.x),FortBitsV20(s.y),FortBitsV20(s.nativeDepth),FortBitsV20(s.rawTailScalar),FortBitsV20(s.nativeXYZ[0]),FortBitsV20(s.nativeXYZ[1]),FortBitsV20(s.nativeXYZ[2]),FortBoolV20(s.sourceFacingRight),FortBoolV20(s.sourceMaxHpAbsent)});}
   using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",r.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();
  }
  // No argument-position guess is made by the source runner. An explicit
  // candidate/replay policy must supply HP/facing/clear classification.
  // Source last integer is verified native signed8 KEY_CODE, never policy-owned.
  // Null is a strict HOLD: source request recorded, no actor invented.
  public Func<BelowFortCreateRequestV20,EnemySpawn> BelowFortCreateCandidateAdapterV20;
  public string BelowFortCreateAdapterDescriptionV20;
  public Func<string,int> BelowFortLiveCountReplayV20;
  public int PendingBelowFortCreatesV20 {get{return belowFortRequestsV20.Count;}}
  public readonly List<BelowTunnelSourceEventV20> belowFortEventsV20=new List<BelowTunnelSourceEventV20>();
  public void SetNpcBelowFortV20(BelowFortSourceDocumentV20 doc){
   if(BelowDefaultV20(doc))doc=null;
   if(doc!=null){
    if(doc.schema!="altera-below-tunnel-cannon-fort-source-draft-v20-r2"||doc.runtimeReady||doc.runtimeAdmitted||doc.nativeExecuted||doc.unit!=.28f||doc.npc!="NUI_CANNON_FORT_PARASITE_BODY"||doc.armorKey!=5||doc.bodyClearKey!=6||doc.sourceRoomId!="Dungeon_Altera_Below_Tunnel_Normal-2"||doc.sourceWorldId!="WI_ALTERA_BELOW_TUNNEL_STAGE4"||doc.initialState!="CANNONFORT_PARASITE_BODY_START"||doc.sourceRulesSha256!="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb"||doc.sourceEffectsSha256!="89e0d3e4a89d5850fd546948e2b2cf08ee2350d7503044684cbd7060c1f1fa42"||doc.states==null||doc.states.Length!=19||doc.patterns==null||doc.patterns.Length!=5)throw new ArgumentException("Invalid exact Below Tunnel Fort draft binding");
    var keys=new HashSet<string>();foreach(var row in doc.states){NpcStateDefinition s;if(row==null||!keys.Add(row.id)||!npcStates.TryGetValue(doc.npc+"/"+row.id,out s)||s.sourceAnimation!=row.sourceAnimation||s.duration!=row.duration||s.loop!=row.loop||row.events==null||row.eventIntervalSeconds<0)throw new ArgumentException("Changed original Fort source clock");foreach(var e in row.events)if(e==null||!npcStates.ContainsKey(doc.npc+"/"+e.target)||e.rate<0||e.rate>100||e.stateTimeOver< -1||e.timerElapsed0< -1||e.nearNative< -1)throw new ArgumentException("Invalid Fort source transition");}
    int count=0;foreach(var p in doc.patterns){if(p==null||p.animationAt!=.05f||!keys.Contains(p.state)||p.callback!=p.state+"_FRAME_MOVE"||!SourceSupportHashV17(p.sourceInstructionsSha256)||p.spawns==null||p.spawns.Length!=(p.onlyMissingPerType?2:3)||p.clearPriorMini==p.onlyMissingPerType||p.restartTimer0!=p.clearPriorMini)throw new ArgumentException("Changed Fort original Push pattern");foreach(var r in p.spawns){if(r==null||!npcDefinitions.ContainsKey(r.npc)||r.nativeXYZ==null||r.nativeXYZ.Length!=3||!r.sourceMaxHpAbsent||!Finite(r.x)||!Finite(r.y)||!Finite(r.nativeDepth)||r.groundLine<0||r.startNumber<=0||r.rawTailScalar!=(p.onlyMissingPerType?1:1.9f)||r.rawLastInteger!=(p.onlyMissingPerType?(r.npc.EndsWith("TOP_CANNON")?2:4):-1))throw new ArgumentException("Invalid exact Fort original start/argument contract");count++;}}if(count!=14)throw new ArgumentException("Lost Fort Push callsites");
    if(FortSemanticReceiptV20(doc)!=BelowFortSemanticReceiptV20)throw new ArgumentException("Changed source-executed Fort field/IR receipt");
   }belowFortV20=doc;ClearBelowFortRoomV20();
  }
  void ClearBelowFortRoomV20(){belowFortActorsV20.Clear();belowFortRequestsV20.Clear();belowFortEventsV20.Clear();}
  BelowFortActorV20 BelowFortActorV20For(Enemy e){BelowFortActorV20 a;if(!belowFortActorsV20.TryGetValue(e,out a)){a=new BelowFortActorV20();belowFortActorsV20.Add(e,a);}return a;}
  bool IsBelowFortBodyV20(Enemy e){return belowFortV20!=null&&e!=null&&e.sourceNPC==belowFortV20.npc;}
  void FortEventV20(string type,Enemy owner,Enemy target,string source,float at,float amount=0){if(belowFortEventsV20.Count>=4096)belowFortEventsV20.RemoveAt(0);var p=target??owner;belowFortEventsV20.Add(new BelowTunnelSourceEventV20{type=type,npc=owner.sourceNPC,target=target==null?"":target.sourceNPC,source=source,at=at,amount=amount,room=room,ownerSerial=NpcSoundActorId(owner),targetSerial=NpcSoundActorId(target),x=p.x,y=p.y,depth=p.nativeDepth});}
  void OnBelowFortEnteredV20(Enemy owner,NpcStateDefinition state){
   if(!IsBelowFortBodyV20(owner))return;var a=BelowFortActorV20For(owner);if(SimpleDangerIntroductions&&!a.initialized&&state.id=="CANNONFORT_PARASITE_BODY_WAIT_GUARDED")a.timer0At=elapsed;a.initialized=true;a.age=0;a.nextPoll=0;a.issued=a.calledPart=false;FortEventV20("source-state-entry",owner,null,state.id,elapsed);
   if(state.id=="CANNONFORT_PARASITE_BODY_SUMMON_NASOD"){a.int0At=elapsed;FortEventV20("source-Int0-GetGameTime",owner,null,"CANNONFORT_PARASITE_BODY_SUMMON_NASOD_STATE_START",elapsed,a.int0At);}
  }
  void OnBelowFortExitV20(Enemy owner,NpcStateDefinition state){if(!IsBelowFortBodyV20(owner)||state==null)return;if(state.id==belowFortV20.initialState){BelowFortActorV20For(owner).timer0At=elapsed;FortEventV20("source-SetTimerRestart0",owner,null,"CANNONFORT_PARASITE_BODY_START_STATE_END",elapsed);}}
  int FortLiveV20(string npc){if(BelowFortLiveCountReplayV20!=null){int count=BelowFortLiveCountReplayV20(npc);if(count<0)throw new InvalidOperationException("Invalid explicit source population replay");return count;}int n=0;foreach(var e in enemies)if(e.sourceNPC==npc&&e.hp>0)n++;return n;}
  Enemy FortKeyV20(int key){foreach(var e in enemies)if(e.encounterIndex==currentEncounter&&e.sourceKeyCode==key&&e.hp>0)return e;return null;}
  void RequestFortPartV20(Enemy owner,int key,string npc,string state,float at){var target=FortKeyV20(key);if(target==null||target.hp<=0)return;if(target.sourceNPC!=npc){FortEventV20("HOLD-source-key-identity-mismatch",owner,target,"GetKeyCodeNPC/"+key,at);return;}
   bool ok=TryRequestNpcState(target,state,false)&&CommitNpcStateRequest(target);FortEventV20(ok?"source-part-StateChange-false-committed":"HOLD-source-part-StateChange-rejected",owner,target,state,at,key);
  }
  void DispatchFortPartV20(Enemy owner,NpcStateDefinition state,BelowFortActorV20 a,float from,float to,float packetStart){
   if(a.calledPart||from>=.9f||to<.5f||from>.5f)return;float at=packetStart+Math.Max(0,.5f-from);
   if(state.id.StartsWith("CANNONFORT_PARASITE_BODY_MISSILE_ATTACK_2",StringComparison.Ordinal)){a.calledPart=true;RequestFortPartV20(owner,2,"NUI_CANNON_FORT_PARASITE_TOP_CANNON","CANNONFORT_PARASITE_TOP_CANNON_OPEN",at);RequestFortPartV20(owner,4,"NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON","CANNONFORT_PARASITE_BOTTOM_CANNON_OPEN",at);}
   else if(state.id=="CANNONFORT_PARASITE_BODY_ATTACK_LASER_TOP"){a.calledPart=true;RequestFortPartV20(owner,1,"NUI_CANNON_FORT_PARASITE_TOP_EYE","CANNONFORT_PARASITE_TOP_EYE_OPEN_LASER",at);}
   else if(state.id=="CANNONFORT_PARASITE_BODY_ATTACK_LASER_BOTTOM"){a.calledPart=true;RequestFortPartV20(owner,3,"NUI_CANNON_FORT_PARASITE_BOTTOM_EYE","CANNONFORT_PARASITE_BOTTOM_EYE_OPEN_LASER",at);FortEventV20("HOLD-source-SetNowMP0-NPC-mana-model",owner,null,"CANNONFORT_PARASITE_BODY_ATTACK_LASER_BOTTOM_FRAME_MOVE",at);}
  }
  void RunBelowFortPatternV20(Enemy owner,BelowFortPatternV20 pattern,float at){
   if(pattern.clearPriorMini)foreach(var old in enemies)if(old.hp>0&&(old.sourceNPC=="NUI_CANNON_MINI"||old.sourceNPC=="NUI_CANNON_MINI_UP")){old.hp=0;old.state=CombatState.Dead;InterruptNpcAction(old);FortEventV20("source-clear-prior-Mini-HP0",owner,old,pattern.callback,at);}
   foreach(var spawn in pattern.spawns){if(pattern.onlyMissingPerType&&FortLiveV20(spawn.npc)!=0)continue;if(!NpcMechanismIsHost){FortEventV20("source-native-PushCreate-host-rejected",owner,null,pattern.callback,at,spawn.startNumber);continue;}var request=new BelowFortCreateRequestV20{serial=++belowFortCreateSerialV20,owner=owner,npc=spawn.npc,sourceCallback=pattern.callback,room=room,encounter=owner.encounterIndex,startNumber=spawn.startNumber,line=spawn.groundLine,rawLastInteger=spawn.rawLastInteger,at=at,due=at+Math.Max(0,spawn.rawTailScalar),x=spawn.x,y=spawn.y,nativeDepth=spawn.nativeDepth,rawTailScalar=spawn.rawTailScalar,nativeXYZ=(float[])spawn.nativeXYZ.Clone()};int insert=belowFortRequestsV20.FindIndex(q=>q.due>request.due);if(insert<0)belowFortRequestsV20.Add(request);else belowFortRequestsV20.Insert(insert,request);FortEventV20("source-PushCreate-request",owner,null,pattern.callback,at,spawn.startNumber);}
   if(pattern.restartTimer0){BelowFortActorV20For(owner).timer0At=at;FortEventV20("source-SetTimerRestart0",owner,null,pattern.callback,at);}
  }
  void FlushBelowFortRequestsV20(){
   for(int i=0;i<belowFortRequestsV20.Count;){var r=belowFortRequestsV20[i];if(r.room!=room||victory){belowFortRequestsV20.RemoveAt(i);continue;}if(r.due>elapsed){i++;continue;}belowFortRequestsV20.RemoveAt(i);
    if(BelowFortCreateCandidateAdapterV20==null){FortEventV20("HOLD-CreateABI-policy-unverified",r.owner,null,r.sourceCallback,r.due,r.startNumber);continue;}
    if(string.IsNullOrEmpty(BelowFortCreateAdapterDescriptionV20))throw new InvalidOperationException("Candidate CreateABI policy must be named, never implicit native truth");var template=BelowFortCreateCandidateAdapterV20(r);if(template==null){FortEventV20("HOLD-explicit-CreateABI-policy",r.owner,null,r.sourceCallback,r.due);continue;}
    if(template.sourceNPC!=r.npc||!Finite(template.hp)||template.hp<=0||!Finite(template.visualScale)||template.visualScale<=0||template.facing!=1&&template.facing!=-1)throw new ArgumentException("CreateABI candidate substituted identity or invalid unproven stats/facing");
    var p=npcDefinitions[r.npc];var created=new Enemy{sourceNPC=r.npc,kind=p.kind,sourceKeyCode=unchecked((sbyte)r.rawLastInteger),countsForClear=template.countsForClear,x=r.x,spawnX=r.x,y=r.y,nativeDepth=r.nativeDepth,groundLine=r.line,encounterIndex=r.encounter,facing=template.facing,hp=template.hp,maxHp=template.hp,visualScale=template.visualScale,active=true,aiEnabled=template.aiEnabled,state=CombatState.Normal};enemies.Add(created);FortEventV20("candidate-CreateABI-materialized",r.owner,created,r.sourceCallback,r.due);}
  }
  bool FortConditionV20(Enemy owner,BelowFortActorV20 a,BelowFortEventRuleV20 e){
   if(e.type=="SCT_MOTION_END")return a.age+1e-6f>=NpcState(owner).duration;
   if(e.type=="SCT_CONDITION_FUNCTION"){
    if(e.condition=="CF_CANNONFORT_PARASITE_BODY_WAIT_GUARD"){var armor=FortKeyV20(belowFortV20.armorKey);return armor==null||armor.hp<=0;}
    if(e.condition=="CF_CANNONFORT_PARASITE_BODY_SUMMON_NASOD")return FortLiveV20("NUI_CANNON_FORT_PARASITE_TOP_CANNON")==0&&FortLiveV20("NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON")==0;
    if(e.condition=="CF_CANNONFORT_PARASITE_BODY_WAIT")return elapsed-a.int0At>30;
    if(e.condition=="CF_CANNONFORT_PARASITE_BODY_EXPLODE_NASOD")return elapsed-a.int0At>27;
    if(e.condition=="CF_CANNONFORT_PARASITE_BODY_ATTACK_LASER_BOTTOM")return elapsed-a.int0At>29;
    FortEventV20("HOLD-source-condition-function",owner,null,e.condition,elapsed);return false;
   }
   if(e.type!="SCT_CONDITION_TABLE")return false;
   if(e.stateTimeOver>=0&&!(a.age>e.stateTimeOver+1e-5f))return false;
   if(e.timerElapsed0>=0&&elapsed-a.timer0At<e.timerElapsed0)return false;
   if(e.nearNative>=0&&Math.Abs(player.x-owner.x)>e.nearNative*.28f)return false;
   return e.rate>=100||NpcRoll()<e.rate;
  }
  bool TickBelowFortManagedV20(Enemy owner,float dt){
   if(!IsBelowFortBodyV20(owner)||!owner.active||owner.dormant||owner.hp<=0)return false;
   if(!owner.aiEnabled)return true; // Source inactive-part/native activation is not inferred.
   var a=BelowFortActorV20For(owner);if(!a.initialized){if(!TryEnterNpcState(owner,npcStates[owner.sourceNPC+"/"+belowFortV20.initialState]))return true;}
   if(owner.state!=CombatState.Normal||owner.stun>0)return false;var state=NpcState(owner);if(state==null)return true;var rule=Array.Find(belowFortV20.states,s=>s.id==state.id);if(rule==null)throw new InvalidOperationException("Unbound original Fort state");float from=a.age;float poseFrom=owner.npcTime,poseTo=Math.Min(state.duration,poseFrom+dt);AttachNpcEffectSets(owner,state,poseFrom,poseTo,elapsed-dt);DispatchFortPartV20(owner,state,a,from,from+dt,elapsed-dt);AdvancePassagePose(owner,state,dt,elapsed-dt);a.age+=dt;owner.vx=owner.vy=0;
   var pattern=Array.Find(belowFortV20.patterns,p=>p.state==state.id);if(pattern!=null&&!a.issued&&a.age+1e-6f>=pattern.animationAt/state.sourceAnimSpeed){a.issued=true;RunBelowFortPatternV20(owner,pattern,elapsed-dt+Math.Max(0,pattern.animationAt/state.sourceAnimSpeed-from));}
   bool poll=rule.eventIntervalSeconds<=0||a.age+1e-6f>=a.nextPoll;if(poll&&rule.eventIntervalSeconds>0)a.nextPoll=a.age+rule.eventIntervalSeconds;foreach(var e in rule.events){if(e.eventInterval0==0&&!poll)continue;if(!FortConditionV20(owner,a,e))continue;if(TryEnterNpcState(owner,npcStates[owner.sourceNPC+"/"+e.target]))break;}
   return true;
  }
 }
}
