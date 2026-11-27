using System;
using System.Collections.Generic;

// Optional scoped source callbacks. Shared dispatch hooks and an exact source
// document are required before this candidate can be considered runnable.
namespace ElswordPixel {
 [Serializable] public sealed class NpcLakeShamanHealV18 {
  public string npc,state,sourceCallback,sourceInstructionsSha256,clock,origin;
  public float at,range,healFraction,resetOwnerMp,sourceMpChargeRate,sourceMpChangeRate,entryCooldown;
  public bool requireSameTeam,requireAlive,excludeFullHp,excludeSelf,rangeIsExclusive;
 }
 [Serializable] public sealed class NpcLakeShamanVectorV18 {
  public string npc,state,sourceCallback,sourceInstructionsSha256,clock,captureOrigin,directionMethod,sourceEffect,createMethod;
  public float captureAt,emitAt,sourceOffsetY,movePosTrueDistance,movePosFalseDistance,effectUnitScale;public int vectorSlot;
 }
 [Serializable] public sealed class NpcLakeTotemV18 {
  public string npc,state,sourceCallback,sourceInstructionsSha256,clock,sourceEffect,originBone,readySound,sourceSoundName;
  public float emitAt,originX,originY,originZ,movePosTrueDistance,movePosFalseDistance,sourceOffsetY,effectUnitScale,sourceWaitInterval,sourceFireInterval,sourceFireStateTimeOver;
  public bool fixedAnchor,inheritOriginalFacing,neverCountsForClear;
 }
 [Serializable] public sealed class NpcLakeSourceDocumentV18 {
  public string schema,sourceRulesSha256,teamAdapter;public bool runtimeAdmitted,nativeExecuted;
  public string[] npcUnitIds=new string[0];
  public NpcLakeShamanHealV18[] shamanHeals=new NpcLakeShamanHealV18[0];
  public NpcLakeShamanVectorV18[] shamanVectors=new NpcLakeShamanVectorV18[0];
  public NpcLakeTotemV18[] totems=new NpcLakeTotemV18[0];
  public NpcLakeOlderSummonV18[] olderSummons=new NpcLakeOlderSummonV18[0];
 }
 [Serializable] public sealed class NpcLakeOlderSummonV18 {
  public string npc,state,readyState,adapterId,sourceConditionReference,sourceConditionBody,normalSourceSha256,henirSourceSha256,sourceInstructionsSha256,henirInstructionsSha256,spawnNpc,clock,positionAdapter;
  public bool ownerHpStrictlyPositive,queueBothBeforeFlush;public int liveCountEquals,spawnFacing;public float cooldown,captureAt,emitAt,sourceNativeRange,range;
 }
 [Serializable] public sealed class NpcLakeSourceEventV18 {public string type,npc,target,effect;public float time,amount,x,y;}
 public sealed partial class GameModel {
  NpcLakeSourceDocumentV18 npcLakeSourceV18;
  sealed class LakeCallbackCursorV18 {internal string state;internal float previous;internal bool captured,emitted;internal float x,y,z,rightX,rightY;internal int leftLine,rightLine;}
  sealed class LakeTotemActorV18 {internal float x,y,age,nextDecision;internal int line,facing,readyMask;internal bool emitted;}
  readonly Dictionary<Enemy,LakeCallbackCursorV18> npcLakeCursorsV18=new Dictionary<Enemy,LakeCallbackCursorV18>();
  readonly Dictionary<Enemy,LakeTotemActorV18> npcLakeTotemsV18=new Dictionary<Enemy,LakeTotemActorV18>();
  readonly Dictionary<Enemy,float> npcLakeKnownMpV18=new Dictionary<Enemy,float>();
  public readonly List<NpcLakeSourceEventV18> npcLakeSourceEventsV18=new List<NpcLakeSourceEventV18>();
  static bool EmptyLakeArrayV18<T>(T[] a){return a==null||a.Length==0;}
  static bool LakeNearV18(float actual,float expected){return Finite(actual)&&Math.Abs(actual-expected)<.00001f;}
  void RequireLakeStateV18(string npc,string state,string hash){if(string.IsNullOrEmpty(npc)||string.IsNullOrEmpty(state)||!npcStates.ContainsKey(npc+"/"+state)||!SourceSupportHashV17(hash))throw new ArgumentException("Unbound original Lake callback/state/hash.");}
  public void SetNpcLakeSourceV18(NpcLakeSourceDocumentV18 document){
   if(document!=null&&string.IsNullOrEmpty(document.schema)&&string.IsNullOrEmpty(document.sourceRulesSha256)&&string.IsNullOrEmpty(document.teamAdapter)&&!document.runtimeAdmitted&&!document.nativeExecuted&&EmptyLakeArrayV18(document.npcUnitIds)&&EmptyLakeArrayV18(document.shamanHeals)&&EmptyLakeArrayV18(document.shamanVectors)&&EmptyLakeArrayV18(document.totems)&&EmptyLakeArrayV18(document.olderSummons))document=null;
   if(document!=null){
    if(document.schema!="besma-lake-source-callbacks-v18"||!SourceSupportHashV17(document.sourceRulesSha256)||document.runtimeAdmitted||document.nativeExecuted||document.shamanHeals==null||document.shamanVectors==null||document.totems==null)throw new ArgumentException("Invalid original Lake callback document.");
    if(document.shamanHeals.Length!=1||document.shamanVectors.Length!=1||document.totems.Length!=1||document.olderSummons==null||document.olderSummons.Length!=1)throw new ArgumentException("Lake callback scope must be complete.");
    var npcUnits=new HashSet<string>(document.npcUnitIds??new string[0],StringComparer.Ordinal);
    if(document.teamAdapter!="all-current-enemies-hostile-team-v1"||!npcUnits.SetEquals(new[]{"NUI_KOMODO","NUI_LIZARDMAN_SAURUS","NUI_LIZARDMAN_SPEAR","NUI_SCORPION","NUI_LIZARDMAN_SHAMAN","NUI_LIZARDMAN_OLDER","NUI_BESMA_TOTEM_BASE"})||document.npcUnitIds.Length!=npcUnits.Count)throw new ArgumentException("Lake original IS_NPC membership/team adapter changed.");
    var heal=document.shamanHeals[0];if(heal==null)throw new ArgumentException("Missing Shaman healing rule.");RequireLakeStateV18(heal.npc,heal.state,heal.sourceInstructionsSha256);
    if(heal.npc!="NUI_LIZARDMAN_SHAMAN"||heal.state!="LIZARDMAN_SHAMAN_ATTACK_B"||heal.sourceCallback!="LIZARDMAN_SHAMAN_SP_HEAL_FRAME_MOVE"||heal.clock!="animation-time"||heal.origin!="owner-current-position"||!LakeNearV18(heal.at,.119f)||!LakeNearV18(heal.range,224)||!LakeNearV18(heal.healFraction,.5f)||heal.excludeSelf||!heal.requireAlive||!heal.requireSameTeam||!heal.excludeFullHp||!heal.rangeIsExclusive||heal.resetOwnerMp!=0||heal.sourceMpChargeRate!=130||heal.sourceMpChangeRate!=1||heal.entryCooldown!=5)throw new ArgumentException("Shaman source heal/eligibility/resource contract changed.");
    var vector=document.shamanVectors[0];if(vector==null)throw new ArgumentException("Missing Shaman vector rule.");RequireLakeStateV18(vector.npc,vector.state,vector.sourceInstructionsSha256);
    if(vector.npc!=heal.npc||vector.state!="LIZARDMAN_SHAMAN_ATTACK_A"||vector.sourceCallback!="LIZARDMAN_SHAMAN_ATTACK_A_FRAME_MOVE"||vector.clock!="animation-time"||vector.captureOrigin!="owner-land-position"||vector.directionMethod!="GetDirVector"||vector.createMethod!="CreateInstance_LUA2"||vector.vectorSlot!=0||!LakeNearV18(vector.captureAt,.1f)||!LakeNearV18(vector.emitAt,.28f)||vector.movePosTrueDistance!=-200||vector.movePosFalseDistance!=200||vector.sourceOffsetY!=200||vector.effectUnitScale!=1||vector.sourceEffect!="DamageEffect_LIZARDMAN_SHAMAN_ATTACK_A"||!npcEffectDefinitions.ContainsKey(vector.sourceEffect))throw new ArgumentException("Shaman source captured-vector emission changed.");
    var totem=document.totems[0];if(totem==null)throw new ArgumentException("Missing Totem callback.");RequireLakeStateV18(totem.npc,totem.state,totem.sourceInstructionsSha256);
    if(totem.npc!="NUI_BESMA_TOTEM_BASE"||totem.state!="BESMA_TOTEM_BASE_YOGA_FIRE"||totem.sourceCallback!="BESMA_TOTEM_BASE_YOGA_FIRE_FRAME_MOVE"||totem.clock!="state-seconds"||totem.originBone!="Dummy1_Mouse"||!LakeNearV18(totem.emitAt,3.6f)||!Finite(totem.originX)||!Finite(totem.originY)||!Finite(totem.originZ)||totem.movePosTrueDistance!=-250||totem.movePosFalseDistance!=250||totem.sourceOffsetY!=-10||totem.effectUnitScale!=1||totem.sourceWaitInterval!=4||totem.sourceFireInterval!=3||totem.sourceFireStateTimeOver!=6||!totem.fixedAnchor||!totem.inheritOriginalFacing||!totem.neverCountsForClear||totem.sourceEffect!="BESMA_TOTEM_BASE_FIRE_ATTACK"||!npcEffectDefinitions.ContainsKey(totem.sourceEffect))throw new ArgumentException("Totem original callback/anchor/timers changed.");
    if(npcDefinitions[totem.npc].environmentKind!="source-totem"||totem.sourceSoundName!="fire_ready.ogg"||!npcSourceSoundAssetsV17.Contains(totem.readySound??""))throw new ArgumentException("Original Totem must use its scoped fixed environment runner and exact source sound.");
    var older=document.olderSummons[0];if(older==null)throw new ArgumentException("Missing Older compatibility source binding.");RequireLakeStateV18(older.npc,older.state,older.sourceInstructionsSha256);
    const string instructions="d13a1a6144b9d09ffb0b58f249e75d21e307fa9be281ff7e1547a06badcc4424";
    if(older.normalSourceSha256!="a849fc02fd49fe08327a3866ae3cb70fdc86ab80b0ffcac61fdd6e616b0e781f"||older.henirSourceSha256!="ec21599b9c0bc8111f6ae1b195d70a1607245f3d468a0ae4ec1bd12d2b103396")throw new ArgumentException("Older original Normal/Henir source bytes changed.");
    if(older.npc!="NUI_LIZARDMAN_OLDER"||older.state!="LIZARDMAN_OLDER_ATTACK_C"||older.readyState!="LIZARDMAN_OLDER_ATTACK_C_READY"||older.adapterId!="normal-older-attack-c-ready-name-v18"||older.sourceConditionReference!="CF_LIZARDMAN_OLDER_ATTACK_C_READY"||older.sourceConditionBody!="CF_LIZARDMAN_OLDER_ATTACK_C"||!SourceSupportHashV17(older.normalSourceSha256)||!SourceSupportHashV17(older.henirSourceSha256)||older.sourceInstructionsSha256!=instructions||older.henirInstructionsSha256!=instructions||older.spawnNpc!="NUI_LIZARDMAN_SHAMAN"||!older.ownerHpStrictlyPositive||older.liveCountEquals!=0||older.cooldown!=16||older.clock!="animation-time"||!LakeNearV18(older.captureAt,.01f)||!LakeNearV18(older.emitAt,.63f)||older.sourceNativeRange!=1500||older.range!=420||older.positionAdapter!="existing-source-ground-direction-v1"||!older.queueBothBeforeFlush||older.spawnFacing!=1)throw new ArgumentException("Older explicit Normal/Henir compatibility/predicate/capture contract changed.");
    NpcMechanismStateRule cooldown;if(!npcMechanismRules.TryGetValue(older.npc+"/"+older.readyState,out cooldown)||cooldown.cooldown!=16)throw new ArgumentException("Older source16second C_READY cooldown is missing.");
    foreach(string decision in new[]{npcDefinitions[older.npc].waitState,npcDefinitions[older.npc].walkState}){var choices=npcStates[older.npc+"/"+decision].choices;if(choices.Length!=4||choices[0].nextState!="LIZARDMAN_OLDER_SPECIAL_ATTACK_A_READY"||choices[1].nextState!=older.readyState||choices[2].nextState!="LIZARDMAN_OLDER_ATTACK_B_READY"||choices[3].nextState!="LIZARDMAN_OLDER_ATTACK_A_READY")throw new ArgumentException("Older source choice priority changed.");}
   }
   npcLakeSourceV18=document;ClearNpcLakeSourceActorsV18();
  }
  void ClearNpcLakeSourceActorsV18(){npcLakeCursorsV18.Clear();npcLakeTotemsV18.Clear();npcLakeKnownMpV18.Clear();npcLakeSourceEventsV18.Clear();}
  public bool TryGetNpcLakeKnownMpV18(Enemy owner,out float value){return npcLakeKnownMpV18.TryGetValue(owner,out value);}
  void LakeEventV18(string type,Enemy owner,Enemy target,float at,float amount=0,string effect=null){
   if(!traceNpcActions)return;if(npcLakeSourceEventsV18.Count>=2048)npcLakeSourceEventsV18.RemoveAt(0);
   npcLakeSourceEventsV18.Add(new NpcLakeSourceEventV18{type=type,npc=owner.sourceNPC,target=target==null?"":target.sourceNPC,time=at,amount=amount,effect=effect??"",x=target==null?owner.x:target.x,y=target==null?owner.y:target.y});
  }
  LakeCallbackCursorV18 LakeCursorV18(Enemy owner,string state,float from){
   LakeCallbackCursorV18 cursor;if(!npcLakeCursorsV18.TryGetValue(owner,out cursor)||cursor.state!=state||from+1e-6f<cursor.previous){cursor=new LakeCallbackCursorV18{state=state};npcLakeCursorsV18[owner]=cursor;}return cursor;
  }
  void EmitLakePositionEffectV18(Enemy owner,string id,float x,float y,float z,float at){
   var emission=new NpcEffectEmission{effect=id,scaleX=1,scaleY=1,scaleZ=1,bone="",ownerState=""};float bx,by,bz;NpcAttachment(owner,emission,at,out bx,out by,out bz);
   emission.offsetX=(x-bx)/owner.facing/owner.visualScale;emission.offsetY=(y-by)/owner.visualScale;emission.offsetZ=(z-bz)/owner.visualScale;
   var effect=SpawnNpcEffect(owner,npcEffectDefinitions[id],emission,at);effect.ownerScale=1;LakeEventV18("source-effect-emission",owner,null,at,1,id);
  }
  void AdvanceNpcLakeSourceV18(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(npcLakeSourceV18==null||owner==null||owner.hp<=0||!owner.active||owner.dormant)return;
   var heal=npcLakeSourceV18.shamanHeals[0];var vector=npcLakeSourceV18.shamanVectors[0];if(owner.sourceNPC=="NUI_LIZARDMAN_OLDER"){AdvanceLakeOlderSummonV18(owner,state,from,to,packetStart);return;}if(owner.sourceNPC!=heal.npc)return;
   var cursor=LakeCursorV18(owner,state.id,from);cursor.previous=to;
   if(state.id==heal.state){float at=heal.at/state.sourceAnimSpeed;if(!cursor.emitted&&from<at&&at<=to+1e-6f){cursor.emitted=true;float absolute=packetStart+at-from;
    foreach(var target in enemies){if(!target.active||target.dormant||Array.IndexOf(npcLakeSourceV18.npcUnitIds,target.sourceNPC)<0||target.hp<=0||target.hp>=target.maxHp)continue;
     float dx=target.x-owner.x,dy=target.y-owner.y,dz=target.nativeDepth-owner.nativeDepth;if(dx*dx+dy*dy+dz*dz>=heal.range*heal.range)continue;
     float amount=Math.Min(target.maxHp-target.hp,target.maxHp*heal.healFraction);EmitNpcLakeHealTargetVisualV18(owner,target,absolute);RecoverEnemyHealth(target,amount,heal.healFraction*100);LakeEventV18("shaman-heal",owner,target,absolute,amount);
    }
    // The callback explicitly resets MP. Its previous amount and130/1 native
    // default regeneration formula are unknown, so only this known value is
    // stored. It must never gate source choices using an invented threshold.
    npcLakeKnownMpV18[owner]=0;LakeEventV18("source-set-owner-mp",owner,null,absolute,0);
   }}else if(state.id==vector.state){float capture=vector.captureAt/state.sourceAnimSpeed,emit=vector.emitAt/state.sourceAnimSpeed;
    if(!cursor.captured&&from<capture&&capture<=to+1e-6f){cursor.captured=true;
     // Explicit existing2D facing adapter: source MovePos +/-200 becomes a
     //56px forward offset. The source branch constants remain in the DTO.
     float landY;if(NativeTerrain){var support=Surface(owner.groundLine);if(support==null||owner.x<support.x1-.02f||owner.x>support.x2+.02f)support=SurfaceBelow(owner.x,owner.y+.05f);if(support==null)throw new InvalidOperationException("Source Shaman has no actual land support for GetLandPosition.");landY=support.Y(owner.x);}else landY=GroundAt(CurrentRoom,owner.x);
     cursor.x=owner.x+owner.facing*200*.28f;cursor.y=landY+vector.sourceOffsetY*.28f;cursor.z=0;LakeEventV18("source-vector-capture",owner,null,packetStart+capture-from);
    }
    if(!cursor.emitted&&from<emit&&emit<=to+1e-6f){if(!cursor.captured)throw new InvalidOperationException("Shaman source vector0 was never captured in this state entry.");cursor.emitted=true;EmitLakePositionEffectV18(owner,vector.sourceEffect,cursor.x,cursor.y,cursor.z,packetStart+emit-from);}
   }
  }
  bool NpcLakeConditionAllowedV18(Enemy owner,NpcCondition condition){
   if(owner.sourceNPC!="NUI_LIZARDMAN_OLDER"||condition.nextState!="LIZARDMAN_OLDER_ATTACK_C_READY")return true;
   if(npcLakeSourceV18==null)throw new InvalidOperationException("Older condition has no explicit Normal compatibility binding.");if(owner.hp<=0)return false;var rule=npcLakeSourceV18.olderSummons[0];
   foreach(var npc in enemies)if(npc.hp>0&&npc.sourceNPC==rule.spawnNpc)return false;
   foreach(var request in npcSummonRequests)if(request.room==room&&request.alreadyEmitted&&request.npc==rule.spawnNpc)return false;return true;
  }
  EnemySpawn LakeShamanTemplateV18(string npc){return CurrentRoom.enemies.Find(s=>s.sourceNPC==npc&&s.hp>0);}
  void AdvanceLakeOlderSummonV18(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   var rule=npcLakeSourceV18.olderSummons[0];if(state.id!=rule.state)return;var cursor=LakeCursorV18(owner,state.id,from);cursor.previous=to;float capture=rule.captureAt/state.sourceAnimSpeed,emit=rule.emitAt/state.sourceAnimSpeed;
   if(!cursor.captured&&from<capture&&capture<=to+1e-6f){
    if(!SummonGroundPoint(owner,new NpcSummonSpawn{npc=rule.spawnNpc,range=rule.range,direction=-1},out cursor.x,out cursor.y,out cursor.leftLine)||!SummonGroundPoint(owner,new NpcSummonSpawn{npc=rule.spawnNpc,range=rule.range,direction=1},out cursor.rightX,out cursor.rightY,out cursor.rightLine))throw new InvalidOperationException("Older original random-position queries have no current source support.");
    cursor.captured=true;SummonEvent("older-vector-capture",owner,rule.spawnNpc,cursor.x,cursor.y,cursor.leftLine);SummonEvent("older-vector-capture",owner,rule.spawnNpc,cursor.rightX,cursor.rightY,cursor.rightLine);LakeEventV18("older-vector-capture",owner,null,packetStart+capture-from,2);
   }
   if(!cursor.emitted&&from<emit&&emit<=to+1e-6f){if(!cursor.captured)throw new InvalidOperationException("Older source vectors0/1 were not captured before.63.");var original=LakeShamanTemplateV18(rule.spawnNpc);if(original==null)throw new InvalidOperationException("Original route has no same-ID Shaman balance template.");cursor.emitted=true;float at=packetStart+emit-from;
    var template=new EnemySpawn{hp=original.hp,visualScale=original.visualScale,nativeDepth=owner.nativeDepth};
    npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=template,npc=rule.spawnNpc,room=room,encounter=owner.encounterIndex,line=cursor.leftLine,x=cursor.x,y=cursor.y,facing=rule.spawnFacing,due=at,alreadyEmitted=true});
    npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=template,npc=rule.spawnNpc,room=room,encounter=owner.encounterIndex,line=cursor.rightLine,x=cursor.rightX,y=cursor.rightY,facing=rule.spawnFacing,due=at,alreadyEmitted=true});
    // Commit remains the existing after-foreach flush. Captured points are
    // never resampled at emission, and emitted requests survive host death.
    SummonEvent("older-request",owner,rule.spawnNpc,cursor.x,cursor.y,cursor.leftLine);SummonEvent("older-request",owner,rule.spawnNpc,cursor.rightX,cursor.rightY,cursor.rightLine);LakeEventV18("older-queue-both-before-flush",owner,null,at,2);
   }
  }
  static bool ValidateNpcLakeSourceEnvironmentV18(NpcDefinition p){if(p.environmentKind!="source-totem")return false;if(p.id!="NUI_BESMA_TOTEM_BASE"||p.environment==null||!Array.Exists(p.states,s=>s.id=="BESMA_TOTEM_BASE_WAIT")||!Array.Exists(p.states,s=>s.id=="BESMA_TOTEM_BASE_YOGA_FIRE")||!Array.Exists(p.states,s=>s.id=="BESMA_TOTEM_BASE_DYING"))throw new ArgumentException("Incomplete original Totem states.");return true;}
  void EnterLakeTotemV18(Enemy owner,LakeTotemActorV18 actor,string state){actor.age=0;actor.emitted=false;actor.readyMask=0;actor.nextDecision=4;EnterNpcState(owner,npcStates[owner.sourceNPC+"/"+state]);}
  void LakeTotemReadySoundV18(Enemy owner,NpcLakeTotemV18 rule,float local,float absolute,string sourceInstruction){
   var cue=new NpcTimedSound{id="lake-totem-ready/"+sourceInstruction,sound=rule.readySound,sourceField="LUA_FRAME_MOVE_FUNC/"+rule.sourceCallback+"/"+sourceInstruction,sourceName=rule.sourceSoundName,candidateClock="source-state-seconds",candidateTime=local,sourceTime=local};
   QueueNpcSound(cue,owner,++npcSoundEntrySerial,0,absolute,rule.state);
  }
  bool TickNpcLakeSourceEnvironmentV18(Enemy owner,NpcDefinition profile,float dt){
   if(profile.environmentKind!="source-totem")return false;if(!owner.active||owner.dormant)return true;if(npcLakeSourceV18==null)throw new InvalidOperationException("Original Totem has no Lake callback document.");var rule=npcLakeSourceV18.totems[0];
   LakeTotemActorV18 actor;if(!npcLakeTotemsV18.TryGetValue(owner,out actor)){actor=new LakeTotemActorV18{x=owner.x,y=owner.y,line=owner.groundLine,facing=owner.facing};npcLakeTotemsV18.Add(owner,actor);EnterLakeTotemV18(owner,actor,"BESMA_TOTEM_BASE_WAIT");}
   owner.x=actor.x;owner.y=actor.y;owner.groundLine=actor.line;owner.facing=actor.facing;owner.vx=owner.vy=0;owner.aiEnabled=false;owner.passive=false;
   if(owner.hp<=0){if(owner.npcState!="BESMA_TOTEM_BASE_DYING")EnterLakeTotemV18(owner,actor,"BESMA_TOTEM_BASE_DYING");owner.deathTime+=dt;AdvancePassagePose(owner,NpcState(owner),dt,elapsed-dt);return true;}
   if(owner.hitPause>0){float freeze=Math.Min(dt,owner.hitPause);owner.hitPause-=freeze;dt-=freeze;if(dt<=1e-6f)return true;}
   if(owner.state!=CombatState.Normal){owner.state=CombatState.Normal;owner.stun=owner.downTime=owner.recoveryTime=0;owner.knockdownOnLanding=false;EnterLakeTotemV18(owner,actor,"BESMA_TOTEM_BASE_DAMAGE");}
   float remaining=dt,packet=elapsed-dt;while(remaining>1e-6f){float step=Math.Min(remaining,1f/120),from=actor.age;actor.age+=step;var state=NpcState(owner);AdvancePassagePose(owner,state,step,packet);
    AdvanceNpcLakeVisualV18(owner,state,from,actor.age,packet);
    if(state.id=="BESMA_TOTEM_BASE_WAIT"){if(actor.age+1e-6f>=actor.nextDecision){actor.nextDecision+=rule.sourceWaitInterval;float dx=player.x-owner.x,dy=player.y-owner.y;if(player.hp>0&&dx*dx+dy*dy<=280*280)EnterLakeTotemV18(owner,actor,rule.state);}}
    else if(state.id==rule.state){
     if((actor.readyMask&1)==0&&from<.01f&&.01f<=actor.age+1e-6f){actor.readyMask|=1;LakeTotemReadySoundV18(owner,rule,.01f,packet+.01f-from,"pc60");}
     if((actor.readyMask&2)==0&&from<.6f&&.6f<=actor.age+1e-6f){actor.readyMask|=2;LakeTotemReadySoundV18(owner,rule,.6f,packet+.6f-from,"pc14");LakeTotemReadySoundV18(owner,rule,.6f,packet+.6f-from,"pc106");}
     if(!actor.emitted&&from<rule.emitAt&&rule.emitAt<=actor.age+1e-6f){actor.emitted=true;float x=owner.x+owner.facing*(rule.originX+250*.28f),y=owner.y+rule.originY-10*.28f;EmitLakePositionEffectV18(owner,rule.sourceEffect,x,y,rule.originZ,packet+rule.emitAt-from);}
     if(actor.age+1e-6f>=rule.sourceFireStateTimeOver)EnterLakeTotemV18(owner,actor,"BESMA_TOTEM_BASE_WAIT");}
    else if(actor.age+1e-6f>=state.duration)EnterLakeTotemV18(owner,actor,"BESMA_TOTEM_BASE_WAIT");remaining-=step;packet+=step;
   }return true;
  }
 }
}
