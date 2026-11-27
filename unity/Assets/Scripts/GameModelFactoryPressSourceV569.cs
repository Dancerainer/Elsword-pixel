using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ElswordPixel {
 [Serializable] public sealed class FactoryPressBirthV569 {
  public int room,order,encounter,line,sourceSpawnIndex,facing;public string npc,kind;
  public float x,y,depth,hp,visualScale;public bool sourceActive,aiEnabled,countsForClear;
 }
 [Serializable] public sealed class FactoryPressDocumentV569 {
  public string schema,policy,sourceModelSha256,rulesSha256,mechanismsSha256,callbackInstructionsSha256,birthReceipt;
  public bool enabled,nativeEquivalent,runtimeAdmitted;public FactoryPressBirthV569[] births;
 }
 public sealed class FactoryPressFrameV569 {
  public Enemy actualOwner;public int uid,epoch;public string npc,state;
  public float stateAge,sourcePoseTime,attackCooldownElapsed,warningRemaining,warningLeft,warningRight,warningY;
  public bool canPassSourceRequest,sourceBodyBlockerMissing;
 }
 public sealed class FactoryPressRequestV569 {
  public Enemy actualOwner;public int uid,serial,epoch;public string npc,state,kind,asset,sourceField,callback;
  public float stateTime,packetAt;public bool boolArgument,nativeExecuted,hostConsumerExecuted;
 }
 public sealed partial class GameModel {
  const string FactoryPressRouteV569="Dungeon_Altera_Nasod_Factory_Normal";
  const string FactoryPressBirthReceiptV569="e4e7fa37c5096dc9ece3530f6205db7d9eaac97b0215fcc37fc9ece176cea0d6";
  FactoryPressBirthV569[] factoryPressBirthsV569;RoomDefinition[] factoryPressRoomsV569;Enemy factoryPressRoomAnchorV569;
  sealed class FactoryPressActorV569 {internal int uid;internal string state;internal double age;internal bool hit,canPass=true;}
  readonly Dictionary<Enemy,FactoryPressActorV569> factoryPressActorsV569=new Dictionary<Enemy,FactoryPressActorV569>();
  readonly List<FactoryPressRequestV569> factoryPressRequestsV569=new List<FactoryPressRequestV569>();
  int factoryPressUidV569,factoryPressSerialV569,factoryPressEpochV569,factoryPressObservedRoomV569=-1;
  float factoryPressAttackBlockedUntilV569;
  public int FactoryPressEpochV569 {get{return factoryPressEpochV569;}}
  public float FactoryPressAttackImpossibleRemainingV569 {get{return Math.Max(0,factoryPressAttackBlockedUntilV569-elapsed);}}
  public bool FactoryPressSourceConfiguredV569 {get{return factoryPressBirthsV569!=null;}}
  static bool FactoryPressIdV569(string id){return id=="NUI_FACTORY_PRESS"||id=="NUI_FACTORY_PRESS3";}
  static string FactoryPressPrefixV569(string id){return id=="NUI_FACTORY_PRESS"?"FACTORY_PRESSMACHINE_":"FACTORY_PRESSMACHINE3_";}
  static string FactoryPressHashV569(byte[] raw){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(raw)).Replace("-","").ToLowerInvariant();}
  static string FactoryPressBitsV569(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0).ToString("X8",CultureInfo.InvariantCulture);}
  static string FactoryPressBirthTextV569(FactoryPressDocumentV569 d){var rows=new List<string>{d.schema,d.policy,d.sourceModelSha256};foreach(var b in d.births)rows.Add(string.Join("|",new[]{b.room.ToString(CultureInfo.InvariantCulture),b.order.ToString(CultureInfo.InvariantCulture),b.encounter.ToString(CultureInfo.InvariantCulture),b.line.ToString(CultureInfo.InvariantCulture),b.sourceSpawnIndex.ToString(CultureInfo.InvariantCulture),b.facing.ToString(CultureInfo.InvariantCulture),b.npc,b.kind,FactoryPressBitsV569(b.x),FactoryPressBitsV569(b.y),FactoryPressBitsV569(b.depth),FactoryPressBitsV569(b.hp),FactoryPressBitsV569(b.visualScale),b.sourceActive?"1":"0",b.aiEnabled?"1":"0",b.countsForClear?"1":"0"}));return string.Join("\n",rows.ToArray())+"\n";}
  // Authored2D scope: entered source-inactive Press environment state clocks,
  // never a generic ACTIVE/AI enable. Other six inactive identities stay owned
  // by their separate controllers. The exact raw-to-object loader is still pending.
  public void SetFactoryPressSourceV569(FactoryPressDocumentV569 d,NpcDefinition[] sameLoadedProfiles,byte[] rulesRaw,byte[] mechanismsRaw){
   if(d==null){ReleaseFactoryPressRoomV569();factoryPressBirthsV569=null;factoryPressRoomsV569=null;return;}
   if(d.schema!="factory-six-press-source-owner-r569"||d.policy!="authored2D-entered-press-state-owner-AI-remains-false"||!d.enabled||d.nativeEquivalent||d.runtimeAdmitted||d.sourceModelSha256!="3c7ecf2fb085ea6cff567ab8fe3853ac951d2ac40acf5f51a8018e7a6ca81890"||d.rulesSha256!="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159"||rulesRaw==null||FactoryPressHashV569(rulesRaw)!=d.rulesSha256||d.mechanismsSha256!="fdbed410573f41f7d895bf2687546125c50f4ab8e079cd6ed94428489d1d8d0e"||mechanismsRaw==null||FactoryPressHashV569(mechanismsRaw)!=d.mechanismsSha256||d.callbackInstructionsSha256!="94d216f33b812d04dd2a4757e7cd9fa6181c09a985f434c6f846a649c0b9e4e3"||d.births==null||d.births.Length!=6||d.birthReceipt!=FactoryPressBirthReceiptV569||FactoryPressHashV569(Encoding.UTF8.GetBytes(FactoryPressBirthTextV569(d)))!=d.birthReceipt)throw new ArgumentException("Changed exact Factory Press source contract");
   if(!FactoryEntryActivationConfiguredV542||room!=0||elapsed!=0||rooms.Count!=3||!NativeTerrain||sameLoadedProfiles==null||sameLoadedProfiles.Length!=11||npcDefinitions.Count!=11)throw new InvalidOperationException("Factory Press binds only a fresh R554 role-aware independent model before Step");
   var profileIds=new HashSet<string>();foreach(var p in sameLoadedProfiles){NpcDefinition installed;if(p==null||!profileIds.Add(p.id)||!npcDefinitions.TryGetValue(p.id,out installed)||!ReferenceEquals(p,installed))throw new ArgumentException("Factory Press must share the exact installed R554 profile objects");}
   var seen=new HashSet<string>();var copied=new FactoryPressBirthV569[6];int normal=0,third=0;
   for(int i=0;i<6;i++){var b=d.births[i];if(b==null||!FactoryPressIdV569(b.npc)||b.room<0||b.room>1||b.order<0||b.order>=rooms[b.room].enemies.Count||!seen.Add(b.room+"/"+b.order)||b.sourceActive||b.aiEnabled||b.countsForClear)throw new ArgumentException("Factory Press is exactly the original six source-inactive environment births");
    var s=rooms[b.room].enemies[b.order];if(s.sourceNPC!=b.npc||s.kind!=b.kind||s.encounterIndex!=b.encounter||s.sourceKeyCode!=0||s.groundLine!=b.line||s.facing!=b.facing||s.x!=b.x||s.y!=b.y||s.nativeDepth!=b.depth||s.hp!=b.hp||s.visualScale!=b.visualScale||s.aiEnabled||s.countsForClear||s.isGate||s.passive)throw new ArgumentException("Original Press birth HP/position/clear/identity changed");
    NpcDefinition p=npcDefinitions[b.npc];ValidateFactoryPressProfileV569(p);if(b.npc=="NUI_FACTORY_PRESS")normal++;else third++;
    copied[i]=new FactoryPressBirthV569{room=b.room,order=b.order,encounter=b.encounter,line=b.line,sourceSpawnIndex=b.sourceSpawnIndex,facing=b.facing,npc=b.npc,kind=b.kind,x=b.x,y=b.y,depth=b.depth,hp=b.hp,visualScale=b.visualScale};
   }
   if(normal!=4||third!=2)throw new ArgumentException("Factory Press4/Press3two source roster changed");
   ReleaseFactoryPressRoomV569();factoryPressBirthsV569=copied;factoryPressRoomsV569=rooms.ToArray();factoryPressObservedRoomV569=room;factoryPressRoomAnchorV569=enemies.Count==0?null:enemies[0];
  }
  void ValidateFactoryPressProfileV569(NpcDefinition p){
   string prefix=FactoryPressPrefixV569(p.id);if(p.states.Length!=3||p.bodySpheres.Length!=0||p.waitState!=prefix+"WAIT"||p.walkState!=p.waitState||p.physicalAttack!=28||p.magicAttack!=28)throw new ArgumentException("Changed exact Press profile or authored attack power");
   foreach(var s in p.states){NpcMechanismStateRule m;if(!npcMechanismRules.TryGetValue(p.id+"/"+s.id,out m)||m.duration!=s.duration||m.loop!=s.loop||m.initialElapsed!=0||m.initialElapsedExplicit||s.sourceAnimSpeed!=1||s.entryOffsetY!=0||s.samples.Length==0)throw new ArgumentException("Missing exact Press source state/mechanism");}
   var start=npcStates[p.id+"/"+prefix+"START"];var wait=npcStates[p.id+"/"+prefix+"WAIT"];var attack=npcStates[p.id+"/"+prefix+"ATTACK"];var cd=npcMechanismRules[p.id+"/"+attack.id];
   if(start.duration!=(p.id=="NUI_FACTORY_PRESS"?1.5f:4f)||wait.duration!=3||start.sourceAnimation!="Wait"||wait.sourceAnimation!="Wait"||!start.loop||!wait.loop||attack.sourceAnimation!="Attack"||attack.loop||attack.duration!=181f/30||attack.hits.Length!=1||cd.cooldown!=(p.id=="NUI_FACTORY_PRESS"?10:0)||cd.cooldownExplicit!=(p.id=="NUI_FACTORY_PRESS"))throw new ArgumentException("Press logical START/WAIT,2s Wait pose,ATTACK or cooldown conflated");
   var hit=attack.hits[0];if(hit.start!=2||hit.end!=2.3f||hit.physical!=1||hit.magic!=0||hit.reaction!=HitReaction.Knockdown||hit.repeat||hit.launch!=0||hit.push!=0||hit.nextDamageCallbackPending)throw new ArgumentException("Changed exact Press source damage window");
  }
  public void ReleaseFactoryPressRoomV569(){foreach(var e in factoryPressActorsV569.Keys){CancelNpcStateSounds(e);e.telegraph=0;}factoryPressActorsV569.Clear();factoryPressRequestsV569.Clear();factoryPressAttackBlockedUntilV569=0;factoryPressObservedRoomV569=-1;factoryPressRoomAnchorV569=null;factoryPressEpochV569++;}
  void PrepareFactoryPressRoomV569(){if(factoryPressBirthsV569==null)return;var anchor=enemies.Count==0?null:enemies[0];if(factoryPressObservedRoomV569!=room||!ReferenceEquals(factoryPressRoomAnchorV569,anchor)){ReleaseFactoryPressRoomV569();factoryPressObservedRoomV569=room;factoryPressRoomAnchorV569=anchor;}}
  Input FilterFactoryPressInputV569(Input input,float dt){
   if(factoryPressBirthsV569==null)return input;PrepareFactoryPressRoomV569();
   if(room<0||room>=factoryPressRoomsV569.Length||!ReferenceEquals(CurrentRoom,factoryPressRoomsV569[room])){ReleaseFactoryPressRoomV569();return input;}
   if(elapsed>=factoryPressAttackBlockedUntilV569)return input;
   presses.Clear();skillBuffered=-1;skillBufferTime=0;
   // Source BBT_ATTACK_IMPOSSIBLE; movement/jump/potion/awakening are preserved.
   return new Input{move=input.move,drop=input.drop,dash=input.dash,dashPressed=input.dashPressed,jump=input.jump,potion=input.potion,potionPressedMask=input.potionPressedMask,awaken=input.awaken,attack=false,attackHeld=false,attackX=false,attackXHeld=false,attackXReleased=false,skill=-1,skillHeldMask=0,skillReleasedMask=0};
  }
  void FactoryPressRequestV569(Enemy e,FactoryPressActorV569 a,string kind,string asset,float stateTime,float packetAt,bool argument=false,string field=""){
   if(factoryPressRequestsV569.Count>=512)factoryPressRequestsV569.RemoveAt(0);factoryPressRequestsV569.Add(new FactoryPressRequestV569{actualOwner=e,uid=a.uid,epoch=factoryPressEpochV569,serial=++factoryPressSerialV569,npc=e.sourceNPC,state=a.state,kind=kind,asset=asset,stateTime=stateTime,packetAt=packetAt,boolArgument=argument,sourceField=field,callback=(kind=="source-passability-request"||kind=="source-camera-request")?FactoryPressPrefixV569(e.sourceNPC)+"ATTACK_FRAME_MOVE":"",nativeExecuted=false,hostConsumerExecuted=false});
  }
  void EnterFactoryPressStateV569(Enemy e,FactoryPressActorV569 a,string suffix){a.state=FactoryPressPrefixV569(e.sourceNPC)+suffix;a.age=0;a.hit=false;a.canPass=true;EnterNpcState(e,npcStates[e.sourceNPC+"/"+a.state]);e.telegraph=0;}
  void FactoryPressCallbackThresholdsV569(Enemy e,FactoryPressActorV569 a,double from,double to,float packetAt){
   // Explicit finite2D crossing interpretation. Native AnimEventTimer/boolean
   // opcode ABI is not proved; the original IR and each KPRI are kept in data.
   float[] times={.01f,2.3f,4.1f};bool[] pass={true,false,true};
   for(int i=0;i<3;i++)if(from<times[i]&&times[i]<=to){a.canPass=pass[i];FactoryPressRequestV569(e,a,"source-passability-request","SetCanPassUnit",times[i],packetAt+(float)(times[i]-from),pass[i]);}
   if(from<2&&2<=to){double dx=(double)player.x-e.x,dy=(double)player.y-e.y;double distance=Math.Sqrt(dx*dx+dy*dy)/NativeToLogical;if(distance<1200)FactoryPressRequestV569(e,a,"source-camera-request","UpDownCrashCameraNoReset(15,.5)",2,packetAt+(float)(2-from));}
   // Exact original particle slots are emitted as typed requests below by their
   // source times. No substitute particle, mesh or camera is drawn/applied.
   float[] at={2.0666f,1.2333f,1.2333f,1.2333f,1.2333f,2.3333f,.01f,.01f,.01f};
   string[] names={"Smoke_Nasod_Factory_Press04","Flare_Nasod_Factory_Press01","Flare_Nasod_Factory_Press01","Flare_Nasod_Factory_Press01","Flare_Nasod_Factory_Press01","Flare_Nasod_Factory_Press02","Steam_Nasod_Factory_Press05","Steam_Nasod_Factory_Press05","Steam_Nasod_Factory_Press05"};
   for(int i=0;i<9;i++)if(from<at[i]&&at[i]<=to)FactoryPressRequestV569(e,a,"source-particle-request",names[i],at[i],packetAt+(float)(at[i]-from),field:"PARTICLE_SEQ/"+(i+1));
  }
  bool FactoryPressTouchesHeroV569(Enemy e,NpcStateDefinition s,float at){
   float frame=Clamp(at*60,0,s.samples.Length-1);int i=(int)frame;float blend=frame-i;var x=s.samples[i].spheres;var y=s.samples[Math.Min(i+1,s.samples.Length-1)].spheres;
   // Both Press variants use the same motion. Its baked helper samples have
   // the Wait mesh floor removed, while the placed actor is at the native root.
   const float waitFloor = 552.647524f * NativeToLogical;
   for(int k=0;k<x.Length;k+=4){
    float scale=e.visualScale;
    float cx=e.x+e.facing*(x[k]+(y[k]-x[k])*blend)*scale;
    float cy=e.y+(waitFloor+x[k+1]+(y[k+1]-x[k+1])*blend)*scale;
    float cz=(x[k+2]+(y[k+2]-x[k+2])*blend)*scale;
    float r=(x[k+3]+(y[k+3]-x[k+3])*blend)*scale;
    float px=Clamp(cx,player.x-9.8f,player.x+9.8f),py=Clamp(cy,player.y,player.y+42),pz=Clamp(cz,-9.8f,9.8f);
    if((px-cx)*(px-cx)+(py-cy)*(py-cy)+(pz-cz)*(pz-cz)<=r*r)return true;
   }
   return false;
  }
  void FactoryPressDamageV569(Enemy e,FactoryPressActorV569 a,NpcStateDefinition state,double from,double to,float packetAt){
   var h=state.hits[0];if(a.hit||to<h.start||from>h.end||player.hp<=0)return;float lo=(float)Math.Max(from,h.start),hi=(float)Math.Min(to,h.end);
   for(float at=lo;;at=Math.Min(hi,at+1f/120)){
    if(FactoryPressTouchesHeroV569(e,state,at)){
     var p=NpcProfile(e);int dealt=ApplyHitToPlayer((int)Math.Round(p.physicalAttack*h.physical+p.magicAttack*h.magic),e.x,h.reaction,h.launch);
     if(dealt>0){a.hit=true;float actualAt=packetAt+at-(float)from;float sourceBuffSeconds=e.sourceNPC=="NUI_FACTORY_PRESS"?4:7;factoryPressAttackBlockedUntilV569=Math.Max(factoryPressAttackBlockedUntilV569,actualAt+sourceBuffSeconds);presses.Clear();skillBuffered=-1;skillBufferTime=0;FactoryPressRequestV569(e,a,"actual-2D-source-hit-with-attack-impossible","BTI_DEBUFF_PRESS",at,actualAt,field:"DAMAGE_DATA/BUFF_FACTOR");TraceNpc(e,"factory-press-source-hit",0,dealt);break;}
    }
    if(at>=hi)break;
   }
  }
  bool TickFactoryPressSourceV569(Enemy e,float dt){
   if(factoryPressBirthsV569==null||e==null||!FactoryPressIdV569(e.sourceNPC))return false;PrepareFactoryPressRoomV569();
   if(room<0||room>=3||CurrentRoom.id!=FactoryPressRouteV569+"-"+room||!ReferenceEquals(CurrentRoom,factoryPressRoomsV569[room]))return false;
   if(!e.active||e.dormant||e.encounterIndex!=EncounterIndex)return true;
   int order=enemies.IndexOf(e);var b=Array.Find(factoryPressBirthsV569,v=>v.room==room&&v.order==order);if(b==null||b.npc!=e.sourceNPC||b.encounter!=e.encounterIndex||b.kind!=e.kind||e.sourceKeyCode!=0||e.aiEnabled||e.countsForClear||e.isGate||e.passive)throw new InvalidOperationException("Factory Press birth mismatch: "+e.sourceNPC+" in "+room+"/"+e.encounterIndex+" slot "+order+" expected "+(b==null?"missing":b.npc+"/"+b.encounter)+" key "+e.sourceKeyCode+" AI "+e.aiEnabled+" clear "+e.countsForClear+" gate "+e.isGate+" passive "+e.passive);
   var stale=new List<Enemy>();foreach(var old in factoryPressActorsV569.Keys)if(!enemies.Contains(old))stale.Add(old);foreach(var old in stale){CancelNpcStateSounds(old);factoryPressActorsV569.Remove(old);}
   if(e.hp<=0){CancelNpcStateSounds(e);e.telegraph=0;e.deathTime+=dt;return true;}
   FactoryPressActorV569 a;if(!factoryPressActorsV569.TryGetValue(e,out a)){if(!string.IsNullOrEmpty(e.npcState)||e.clock!=0||e.npcTime!=0)throw new InvalidOperationException("Press source owner requires fresh first eligible actor Step");a=new FactoryPressActorV569{uid=++factoryPressUidV569};factoryPressActorsV569.Add(e,a);EnterFactoryPressStateV569(e,a,"START");}
   var mechanism=MechanismActor(e);var attack=npcMechanismRules[e.sourceNPC+"/"+FactoryPressPrefixV569(e.sourceNPC)+"ATTACK"];
   bool stopped=e.hitPause>0;float clockDt=dt;if(stopped){float frozen=Math.Min(clockDt,e.hitPause);e.hitPause-=frozen;clockDt-=frozen;}
   try{
    if((e.state!=CombatState.Normal&&e.state!=CombatState.Airborne)||e.stun>0||e.knockdownOnLanding){e.telegraph=0;return true;} // source NEVER_MOVE also owns suspended birth poses; no invented damage/getup state
    if(clockDt<=0)return true;
    var state=npcStates[e.sourceNPC+"/"+a.state];
    if(e.npcState!=a.state){e.npcState=a.state;e.clip=state.clip;e.npcTime=(float)Math.Min(a.age,state.duration);} // resume owned phase without replaying thresholds
    if(!a.state.EndsWith("ATTACK",StringComparison.Ordinal)&&a.age>=state.duration&&mechanism.elapsed[attack.state]>=attack.cooldown&&NpcMechanismIsHost){mechanism.elapsed[attack.state]=0;EnterFactoryPressStateV569(e,a,"ATTACK");state=npcStates[e.sourceNPC+"/"+a.state];}
    double from=a.age,to=from+clockDt;float packetAt=elapsed-clockDt;
    if(a.state.EndsWith("ATTACK",StringComparison.Ordinal)){
     double end=Math.Min(to,state.duration);FactoryPressCallbackThresholdsV569(e,a,from,end,packetAt);AdvanceNpcStateSounds(e,state,(float)from,(float)end,packetAt);FactoryPressDamageV569(e,a,state,from,end,packetAt);a.age=end;
     if(to>=state.duration){double rest=to-state.duration;EnterFactoryPressStateV569(e,a,"WAIT");a.age=rest;state=npcStates[e.sourceNPC+"/"+a.state];}
    }else a.age=to;
    e.vx=e.vy=0;e.npcTime=(float)Math.Min(a.age,state.duration);e.clock=state.sourceAnimation=="Wait"?(float)(a.age%2):(float)a.age;
    // Warning is a separate authored player UI; do not set telegraph, whose
    // existing DemoScene path retimes attacks. The new consumer reads frames.
    e.telegraph=0;
   }finally{foreach(var rule in npcMechanismRules.Values)if(rule.npc==e.sourceNPC&&rule.cooldown>0)mechanism.elapsed[rule.state]+=dt;}
   return true;
  }
  // Read the already-owned source passability flag without advancing a
  // clock or activating another actor. Before its first eligible Tick the
  // original START default is passable, just like EnterFactoryPressState.
  public bool TryReadFactoryPressPassabilityV1469(Enemy e,out bool canPass){
   canPass=false;
   if(factoryPressBirthsV569==null||factoryPressRoomsV569==null||e==null||!FactoryPressIdV569(e.sourceNPC)||!e.active||e.dormant||e.hp<=0||e.encounterIndex!=EncounterIndex||room<0||room>=factoryPressRoomsV569.Length||!ReferenceEquals(CurrentRoom,factoryPressRoomsV569[room]))return false;
   int order=enemies.IndexOf(e);if(order<0)return false;var b=Array.Find(factoryPressBirthsV569,v=>v.room==room&&v.order==order);
   if(b==null||b.npc!=e.sourceNPC||b.encounter!=e.encounterIndex||b.kind!=e.kind||e.sourceKeyCode!=0||e.aiEnabled||e.countsForClear||e.isGate||e.passive)return false;
   FactoryPressActorV569 owner;canPass=!factoryPressActorsV569.TryGetValue(e,out owner)||owner.canPass;return true;
  }
  public FactoryPressRequestV569[] ReadFactoryPressRequestsV569(){return factoryPressRequestsV569.ToArray();}
  public FactoryPressFrameV569[] ReadFactoryPressFramesV569(){return ReadFactoryPressFramesCoreR1470(false);}
  // Presentation may read already-owned, frozen previous-encounter poses.
  // This never initializes an owner, advances a clock, or expands warning/hazard scope.
  public FactoryPressFrameV569[] ReadFactoryPressPresentationFramesR1470(){return ReadFactoryPressFramesCoreR1470(true);}
  FactoryPressFrameV569[] ReadFactoryPressFramesCoreR1470(bool includeOwnedPreviousEncounters){
   var result=new List<FactoryPressFrameV569>();if(factoryPressBirthsV569==null)return result.ToArray();
   foreach(var pair in factoryPressActorsV569){var e=pair.Key;var a=pair.Value;if(!enemies.Contains(e)||!e.active||e.dormant||e.hp<=0||e.encounterIndex>EncounterIndex||!includeOwnedPreviousEncounters&&e.encounterIndex!=EncounterIndex)continue;var attack=npcStates[e.sourceNPC+"/"+FactoryPressPrefixV569(e.sourceNPC)+"ATTACK"];
    float lo=float.PositiveInfinity,hi=float.NegativeInfinity;for(int i=120;i<=138&&i<attack.samples.Length;i++){var s=attack.samples[i].spheres;for(int k=0;k<s.Length;k+=4){lo=Math.Min(lo,s[k]-s[k+3]);hi=Math.Max(hi,s[k]+s[k+3]);}}
    bool warning=e.encounterIndex==EncounterIndex&&a.state.EndsWith("ATTACK",StringComparison.Ordinal)&&a.age<2&&(e.state==CombatState.Normal||e.state==CombatState.Airborne)&&e.stun<=0&&!e.knockdownOnLanding;float left=e.x+Math.Min(e.facing*lo,e.facing*hi)*e.visualScale,right=e.x+Math.Max(e.facing*lo,e.facing*hi)*e.visualScale;
    result.Add(new FactoryPressFrameV569{actualOwner=e,uid=a.uid,epoch=factoryPressEpochV569,npc=e.sourceNPC,state=a.state,stateAge=(float)a.age,sourcePoseTime=e.clock,attackCooldownElapsed=NpcSourceStateElapsed(e,FactoryPressPrefixV569(e.sourceNPC)+"ATTACK"),warningRemaining=warning?(float)(2-a.age):0,warningLeft=left,warningRight=right,warningY=e.y,canPassSourceRequest=a.canPass,sourceBodyBlockerMissing=false});
   }
   return result.ToArray();
  }
 }
}
