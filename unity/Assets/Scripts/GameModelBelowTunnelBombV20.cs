using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace ElswordPixel {
 [Serializable] public sealed class BelowBombFrameV20 {public string state,frameCallback;public int sourceRegistrationIndex;}
 [Serializable] public sealed class BelowBombProofV20 {public string callback,instructionsSha256;}
 [Serializable] public sealed class BelowBombProfileV20 {public string npc,initialState,initializeCallback,readyCallback,fireCallback,initCallback;public bool initializeOnEntry,initializeOnExit,initialFlag0;public int initialCount;public float[] initialVectorXYZ;public BelowBombFrameV20[] frameStates;public BelowBombProofV20[] proof;}
 [Serializable] public sealed class BelowBombSourceDocumentV20 {public string schema,sourceRulesSha256,sourceEffectsSha256,effect,sound,soundSource,particleTemplate,texture2,texture1,serviceAdapter,clockAdapter;public bool runtimeReady,runtimeAdmitted,nativeExecuted;public float hpMaximumFraction,countStepSeconds,particleNativeOffsetY,greenDropPerSecond;public int firstQualifyingCount,initialCount,completedSentinel;public BelowBombProfileV20[] rows;}
 public sealed partial class GameModel {
  BelowBombSourceDocumentV20 belowBombV20;
  sealed class BelowBombActorV20 {internal bool scheduled,initialized,flag0,armor,shown=true,worldColor=true,live,held;internal int count;internal float previous,green=1,blue=1,particleX,particleY;internal long handle;internal int followCalls;}
  readonly Dictionary<Enemy,BelowBombActorV20> belowBombActorsV20=new Dictionary<Enemy,BelowBombActorV20>();
  public readonly List<BelowTunnelSourceEventV20> belowBombEventsV20=new List<BelowTunnelSourceEventV20>();
  public bool BelowBombLogicalMajorParticleServiceV20;
  long belowBombHandleV20;
  const string BelowBombSemanticReceiptV20="1554830c4060f209dcbdd6f509ad6880ae47feaffb909d1b0ceaefd778291370";
  static string BelowBombReceiptV20(BelowBombSourceDocumentV20 d){var s=new List<string>{d.schema,d.sourceRulesSha256,d.sourceEffectsSha256,d.effect,d.sound,d.soundSource,d.particleTemplate,d.texture2,d.texture1,d.serviceAdapter,d.clockAdapter,FortBitsV20(d.hpMaximumFraction),FortBitsV20(d.countStepSeconds),FortBitsV20(d.particleNativeOffsetY),FortBitsV20(d.greenDropPerSecond),d.firstQualifyingCount.ToString(),d.initialCount.ToString(),d.completedSentinel.ToString()};foreach(var r in d.rows){s.AddRange(new[]{r.npc,r.initialState,r.initializeCallback,r.readyCallback,r.fireCallback,r.initCallback,FortBoolV20(r.initializeOnEntry),FortBoolV20(r.initializeOnExit),FortBoolV20(r.initialFlag0),r.initialCount.ToString(),FortBitsV20(r.initialVectorXYZ[0]),FortBitsV20(r.initialVectorXYZ[1]),FortBitsV20(r.initialVectorXYZ[2])});foreach(var f in r.frameStates)s.AddRange(new[]{f.state,f.frameCallback,f.sourceRegistrationIndex.ToString()});foreach(var p in r.proof)s.AddRange(new[]{p.callback,p.instructionsSha256});}using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",s.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();}
  public void SetNpcBelowBombV20(BelowBombSourceDocumentV20 doc){if(BelowDefaultV20(doc))doc=null;if(doc!=null){
   if(doc.schema!="altera-below-tunnel-selfbomb-source-draft-v20-r2"||doc.runtimeReady||doc.runtimeAdmitted||doc.nativeExecuted||doc.rows==null||doc.rows.Length!=4||doc.effect!="NASOD_MONSTER_EXPLOSION"||doc.sound!="npc-minecountdown"||doc.soundSource!="MineCountDown.ogg"||doc.hpMaximumFraction!=.3f||doc.firstQualifyingCount!=3||doc.initialCount!=4||doc.completedSentinel!=5||doc.countStepSeconds!=1||doc.particleNativeOffsetY!=300||doc.greenDropPerSecond!=.3f||!npcEffectDefinitions.ContainsKey(doc.effect)||!npcSourceSoundAssetsV17.Contains(doc.sound))throw new ArgumentException("Invalid exact Below Tunnel SelfBomb source document");
   var ids=new HashSet<string>();foreach(var r in doc.rows){if(r==null||!ids.Add(r.npc)||!npcDefinitions.ContainsKey(r.npc)||!npcStates.ContainsKey(r.npc+"/"+r.initialState)||r.frameStates==null||r.proof==null||r.proof.Length!=4||r.initialVectorXYZ==null||r.initialVectorXYZ.Length!=3)throw new ArgumentException("Missing original SelfBomb callback chain");foreach(var f in r.frameStates)if(string.IsNullOrEmpty(f.state)||string.IsNullOrEmpty(f.frameCallback))throw new ArgumentException("Invalid registered source frame callback");}
   if(BelowBombReceiptV20(doc)!=BelowBombSemanticReceiptV20)throw new ArgumentException("Changed original SelfBomb state/IR/constant receipt");
  }belowBombV20=doc;ClearBelowBombRoomV20();}
  void ClearBelowBombRoomV20(){belowBombActorsV20.Clear();belowBombEventsV20.Clear();}
  BelowBombProfileV20 BombProfileV20(Enemy e){return belowBombV20==null||e==null?null:Array.Find(belowBombV20.rows,r=>r.npc==e.sourceNPC);}
  BelowBombActorV20 BombActorV20(Enemy e){BelowBombActorV20 a;if(!belowBombActorsV20.TryGetValue(e,out a)){a=new BelowBombActorV20();belowBombActorsV20.Add(e,a);}return a;}
  void BombEventV20(string type,Enemy owner,string source,float amount=0){if(belowBombEventsV20.Count>=4096)belowBombEventsV20.RemoveAt(0);belowBombEventsV20.Add(new BelowTunnelSourceEventV20{type=type,npc=owner.sourceNPC,source=source,amount=amount,room=room,at=elapsed,x=owner.x,y=owner.y,depth=owner.nativeDepth,ownerSerial=NpcSoundActorId(owner)});}
  void InitBelowBombV20(Enemy owner,BelowBombProfileV20 r){var a=BombActorV20(owner);a.initialized=true;a.flag0=false;a.count=4;a.previous=0;a.shown=true;BombEventV20("source-InitSelfBomb-Int0-4-Flag0-false-Vector0-zero",owner,r.initCallback,4);}
  void OnBelowBombEnteredV20(Enemy owner,NpcStateDefinition state){var r=BombProfileV20(owner);if(r==null||state.id!=r.initialState)return;BombActorV20(owner).scheduled=true;if(r.initializeOnEntry)InitBelowBombV20(owner,r);}
  void OnBelowBombExitV20(Enemy owner,NpcStateDefinition state){var r=BombProfileV20(owner);if(r!=null&&state!=null&&state.id==r.initialState&&r.initializeOnExit)InitBelowBombV20(owner,r);}
  bool BelowBombArmorV20(Enemy owner){BelowBombActorV20 a;return belowBombV20!=null&&owner!=null&&belowBombActorsV20.TryGetValue(owner,out a)&&a.armor;}
  public int BelowBombCountV20(Enemy owner){return BombActorV20(owner).count;}
  public bool BelowBombShownV20(Enemy owner){return BombActorV20(owner).shown;}
  public float[] BelowBombColorV20(Enemy owner){var a=BombActorV20(owner);return new[]{1f,a.green,a.blue,1f};}
  public void NotifyBelowBombLogicalParticleExpiredV20(Enemy owner){BombActorV20(owner).live=false;}
  void BombSoundV20(Enemy owner,string callback){QueueNpcSound(new NpcTimedSound{id=owner.sourceNPC+"/"+callback+"/countdown",sound=belowBombV20.sound,sourceName=belowBombV20.soundSource,sourceField=callback+".PlaySound_LUA",sourceTime=0,candidateTime=0,candidateClock="source-GetGameTime-countdown-callback",nativeClockOwnershipProven=false},owner,++npcSoundEntrySerial,0,elapsed,owner.npcState);}
  bool FireBelowBombV20(Enemy owner,BelowBombActorV20 a,BelowBombProfileV20 r){a.shown=true;if(a.count==5)return false;
   owner.hp=0;owner.state=CombatState.Dead;owner.stun=0;InterruptNpcAction(owner);owner.clip="death";owner.clock=0;BombEventV20("source-SelfBomb-SetNowHP0-before-Create",owner,r.fireCallback);
   if(a.handle!=0){a.handle=0;a.live=false;BombEventV20("source-DestroyInstance-clear-slot0",owner,r.fireCallback);}
   var fx=SpawnNpcEffect(owner,npcEffectDefinitions[belowBombV20.effect],new NpcEffectEmission{effect=belowBombV20.effect,offsetZ=owner.nativeDepth*.28f,sourceLandPositionYArgument=true},elapsed);
   a.count=5;a.shown=false;BombEventV20("source-SelfBomb-Create-NASOD_MONSTER_EXPLOSION",owner,r.fireCallback,fx.serial);return true;
  }
  bool TickBelowBombV20(Enemy owner,float dt){var r=BombProfileV20(owner);if(r==null||!owner.active||owner.dormant||owner.hp<=0||owner.hitPause>0)return false;var a=BombActorV20(owner);
   if(!a.scheduled&&string.IsNullOrEmpty(owner.npcState)&&owner.state==CombatState.Normal&&owner.aiEnabled){if(!TryEnterNpcState(owner,npcStates[r.npc+"/"+r.initialState]))return false;}
   string sourceState=owner.npcState;if(string.IsNullOrEmpty(sourceState)){var p=NpcProfile(owner);var reaction=Array.Find(p.reactions,v=>v.combatState==owner.state);if(reaction!=null)sourceState=reaction.sourceState;}
   if(!Array.Exists(r.frameStates,f=>f.state==sourceState))return false;
   if(!a.initialized||!BelowBombLogicalMajorParticleServiceV20){if(!a.held){a.held=true;BombEventV20(a.initialized?"HOLD-logical-MajorParticle-service":"HOLD-source-InitSelfBomb-not-observed",owner,r.readyCallback);}return false;}
   if(a.handle!=0){if(a.live){a.particleX=owner.x;a.particleY=owner.y+300*.28f;a.followCalls++;}else{a.handle=0;BombEventV20("source-clear-expired-slot0",owner,r.readyCallback);}}
   if(a.count<=0)return FireBelowBombV20(owner,a,r);
   if(owner.hp>owner.maxHp*.3f)return false;
   a.armor=true;int next;
   if(a.flag0){int step=(int)Math.Floor(elapsed-a.previous);a.worldColor=false;a.green-=.3f*dt;a.blue=a.green-.3f*dt;if(step<1)return false;next=Math.Max(0,a.count-step);a.count=next;a.previous=elapsed;}
   else{a.flag0=true;a.previous=elapsed;a.count=next=3;}
   BombEventV20("source-count",owner,r.readyCallback,next);
   if(next==3){BombSoundV20(owner,r.readyCallback);a.handle=++belowBombHandleV20;a.live=true;a.particleX=owner.x;a.particleY=owner.y+300*.28f;BombEventV20("source-Mine_Countdown-SetMajorParticle-slot0",owner,r.readyCallback,300);}
   else if(next==2||next==1){BombSoundV20(owner,r.readyCallback);if(a.handle!=0&&a.live)BombEventV20(next==2?"source-ChangeTex-PC_2":"source-ChangeTex-PC_1",owner,r.readyCallback);}
   else if(next==0){if(a.handle!=0){a.handle=0;a.live=false;BombEventV20("source-DestroyInstance-clear-slot0",owner,r.readyCallback);}return FireBelowBombV20(owner,a,r);}
   return false;
  }
 }
}
