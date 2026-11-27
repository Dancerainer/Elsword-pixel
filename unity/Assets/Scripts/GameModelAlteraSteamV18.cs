using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraSteamControllerRuleV18 {
  public string npc,environmentKind,controllerNpc,waitState,attackState,sourceAttackCallback,sourceWaitStartCallback,sourceParticle,sourceParticleBone,stopSound,fireSound;
  public int flagIndex;public float controllerShutdownHpFraction,sourceParticleOffsetY,sourceBackSpeedY;
  public bool sourceForceFly;
 }
 [Serializable] public sealed class AlteraSteamPresentationEventV18 {
  public string kind,npc,state,sourceName;public long serial,handle;public int generation,loop;
  public float time,x,y,z,sourceScaleX=100,sourceScaleY=100,sourceSentinelX=-1,sourceSentinelY=-1;
 }
 // Native FORCE_FLY is deliberately retained in the request. An unsupported
 // downward launch must not be silently dispatched as ordinary ground down.
 [Serializable] public sealed class AlteraSteamDamageRequestV18 {
  public Enemy owner;public string npc,sourceState;public HitReaction reaction;
  public float time,localTime,sourceMagicMultiplier,sourceRepeatGap,sourceBackSpeedY;
  public int damage;public bool sourceForceFly;
 }
 public sealed partial class GameModel {
  readonly Dictionary<string,AlteraSteamControllerRuleV18> alteraSteamRulesV18=new Dictionary<string,AlteraSteamControllerRuleV18>();
  sealed class AlteraSteamActorV18 {
   internal float x,y,z,nextRequest;internal int line,facing,loop;internal bool flag0;internal long particleHandle;
  }
  readonly Dictionary<Enemy,AlteraSteamActorV18> alteraSteamActorsV18=new Dictionary<Enemy,AlteraSteamActorV18>();
  readonly Queue<AlteraSteamPresentationEventV18> alteraSteamPresentationV18=new Queue<AlteraSteamPresentationEventV18>();
  readonly Queue<AlteraSteamDamageRequestV18> alteraSteamPendingDamageV18=new Queue<AlteraSteamDamageRequestV18>();
  long alteraSteamSerialV18,alteraSteamHandleV18;int alteraSteamGenerationV18;
  // Models the source client GetMajorParticle service. This does not certify
  // that the original particle templates have been rendered or admitted.
  public bool AlteraSteamMajorParticleServiceAvailableV18;
  // Return real applied damage only after source FORCE_FLY has been handled.
  // Null keeps an explicit pending request and never substitutes Knockdown.
  public Func<AlteraSteamDamageRequestV18,int> AlteraSteamDamageConsumerV18;
  public bool TryDequeueAlteraSteamPresentationV18(out AlteraSteamPresentationEventV18 row){if(alteraSteamPresentationV18.Count==0){row=null;return false;}row=alteraSteamPresentationV18.Dequeue();return true;}
  public bool TryDequeueAlteraSteamPendingDamageV18(out AlteraSteamDamageRequestV18 row){if(alteraSteamPendingDamageV18.Count==0){row=null;return false;}row=alteraSteamPendingDamageV18.Dequeue();return true;}
  public bool AlteraSteamOwnFlag0V18(Enemy owner){AlteraSteamActorV18 actor;return owner!=null&&alteraSteamActorsV18.TryGetValue(owner,out actor)&&actor.flag0;}
  public void NotifyAlteraSteamParticleExpiredV18(long handle){if(handle<=0)return;foreach(var actor in alteraSteamActorsV18.Values)if(actor.particleHandle==handle){actor.particleHandle=0;return;}}
  static bool IsAlteraSteamZeroBodyV18(NpcDefinition p){
   return p!=null&&((p.id=="NUI_STEAM_TRAP_DOWN"&&p.environmentKind=="source-steam-controller-down")||(p.id=="NUI_STEAM_TRAP_SIDE"&&p.environmentKind=="source-steam-controller-side"))&&p.bodyHeight==0&&Math.Abs(p.bodyWidth-14)<.0001f;
  }
  static bool ValidateAlteraSteamEnvironmentV18(NpcDefinition p){
   if(IsBelowTunnelZeroBodySteamV20(p))return ValidateBelowTunnelHeldSteamV20(p);if(!IsAlteraSteamZeroBodyV18(p))return false;
   bool down=p.id=="NUI_STEAM_TRAP_DOWN";string prefix=down?"STEAM_TRAP_DOWN_":"STEAM_TRAP_SIDE_";
   if(p.senseRange!=2940||p.lostRange!=3024||Math.Abs(p.approachDistance-42)>.0001f||p.bodySpheres.Length!=0||p.reactions.Length!=0||p.states.Length!=2||p.waitState!=prefix+"WAIT"||p.walkState!=p.waitState)throw new ArgumentException("Changed exact original Steam zero-body/detection/state contract");
   foreach(var s in p.states){bool attack=s.id==prefix+"ATTACK",wait=s.id==prefix+"WAIT";
    if(!attack&&!wait||!s.loop||s.sourceAnimSpeed!=1||s.sourceAnimation!=(attack?"Attack":"Wait")||Math.Abs(s.duration-(attack?199f/30:1f/15))>.00001f||s.samples.Length!=(attack?399:5)||s.invincibleStart!=0||s.invincibleEnd!=0||s.superArmor||s.followTarget||s.walkSpeed!=0||s.hits.Length!=(attack?1:0))throw new ArgumentException("Changed exact original Steam animation/state");
    foreach(var frame in s.samples){if(frame.bodySpheres.Length!=0||frame.spheres.Length!=0||frame.attackLines.Length!=6)throw new ArgumentException("Steam requires original line only, without invented hurt/attack volume");}
    if(attack){var hit=s.hits[0];if(hit.start!=.01f||Math.Abs(hit.end-s.duration)>.00001f||hit.physical!=0||hit.magic!=.2f||hit.reaction!=HitReaction.Knockdown||hit.launch!=-280||hit.push!=0||!hit.repeat||hit.repeatGap!=.05f||hit.stopAttacker!=0||hit.stopDefender!=0)throw new ArgumentException("Changed original Steam damage request");}
   }
   return true;
  }
  public void SetAlteraSteamControllerRulesV18(AlteraSteamControllerRuleV18[] definitions){
   var next=new Dictionary<string,AlteraSteamControllerRuleV18>();
   foreach(var r in definitions??new AlteraSteamControllerRuleV18[0]){
    bool down=r!=null&&r.npc=="NUI_STEAM_TRAP_DOWN",side=r!=null&&r.npc=="NUI_STEAM_TRAP_SIDE";string prefix=down?"STEAM_TRAP_DOWN_":"STEAM_TRAP_SIDE_";NpcDefinition p;
    if(!down&&!side||next.ContainsKey(r.npc)||r.environmentKind!=(down?"source-steam-controller-down":"source-steam-controller-side")||r.controllerNpc!="NUI_STEAM_CONTROLER"||r.waitState!=prefix+"WAIT"||r.attackState!=prefix+"ATTACK"||r.sourceAttackCallback!=prefix+"ATTACK_FRAME_MOVE"||r.sourceWaitStartCallback!=prefix+"WAIT_STATE_START"||r.sourceParticle!=(down?"SteamAttackDown":"SteamAttackSide")||r.sourceParticleBone!="ATTACK_LINE_START1"||r.flagIndex!=0||r.controllerShutdownHpFraction!=.3f||r.sourceParticleOffsetY!=(down?-55:0)||r.sourceBackSpeedY!=-1000||!r.sourceForceFly||r.stopSound!="npc-steamtrap_stop"||down&&r.fireSound!="npc-steamtrap_fire"||side&&!string.IsNullOrEmpty(r.fireSound))throw new ArgumentException("Changed exact source Steam controller callback");
    if(!npcDefinitions.TryGetValue(r.npc,out p)||!ValidateAlteraSteamEnvironmentV18(p)||!npcDefinitions.ContainsKey(r.controllerNpc)||!npcSourceSoundAssetsV17.Contains(r.stopSound)||down&&!npcSourceSoundAssetsV17.Contains(r.fireSound))throw new ArgumentException("Unbound Steam original profile/controller/audio");
    next.Add(r.npc,r);
   }
   alteraSteamRulesV18.Clear();foreach(var r in next)alteraSteamRulesV18.Add(r.Key,r.Value);ClearAlteraSteamActorsV18();
  }
  void ClearAlteraSteamActorsV18(){alteraSteamActorsV18.Clear();alteraSteamPresentationV18.Clear();alteraSteamPendingDamageV18.Clear();alteraSteamGenerationV18++;}
  void SteamPresentationV18(Enemy owner,AlteraSteamActorV18 actor,string kind,string source,float at){
   if(alteraSteamPresentationV18.Count>=256)alteraSteamPresentationV18.Dequeue();
   var row=new AlteraSteamPresentationEventV18{kind=kind,npc=owner.sourceNPC,state=owner.npcState,sourceName=source,serial=++alteraSteamSerialV18,handle=actor.particleHandle,generation=alteraSteamGenerationV18,loop=actor.loop,time=at,x=owner.x,y=owner.y,z=owner.nativeDepth};
   if(kind=="create-particle"){var state=NpcState(owner);int sample=Math.Min(state.samples.Length-1,(int)(owner.npcTime*60));var bone=state.samples[sample].attackLines;var rule=alteraSteamRulesV18[owner.sourceNPC];row.x+=owner.facing*bone[0]*owner.visualScale;row.y+=bone[1]*owner.visualScale+rule.sourceParticleOffsetY*.28f;row.z+=bone[2]*owner.visualScale;}
   alteraSteamPresentationV18.Enqueue(row);
  }
  void SteamSoundV18(Enemy owner,AlteraSteamActorV18 actor,string sound,string source,float sourceTime,float at){
   QueueNpcSound(new NpcTimedSound{id=owner.sourceNPC+"/"+source+"/"+sourceTime,sound=sound,sourceField=source,sourceName=source=="WAIT_STATE_START"?"SteamTrap_Stop.ogg":"SteamTrap_Fire.ogg",sourceTime=sourceTime,candidateTime=sourceTime,candidateClock="npc-state-pose-seconds",nativeClockOwnershipProven=false},owner,0,actor.loop,at,owner.npcState);
  }
  void EnterAlteraSteamStateV18(Enemy owner,AlteraSteamActorV18 actor,AlteraSteamControllerRuleV18 rule,bool attack,float at){
   EnterNpcState(owner,npcStates[rule.npc+"/"+(attack?rule.attackState:rule.waitState)]);actor.loop=0;actor.nextRequest=0;
   if(!attack&&actor.particleHandle!=0){SteamPresentationV18(owner,actor,"auto-die-and-clear-slot0",rule.sourceParticle,at);SteamSoundV18(owner,actor,rule.stopSound,"WAIT_STATE_START",0,at);actor.particleHandle=0;}
  }
  void PollAlteraSteamControllerV18(Enemy owner,AlteraSteamActorV18 actor,AlteraSteamControllerRuleV18 rule,float at){
   Enemy controller=null;foreach(var e in enemies)if(e!=null&&e.sourceNPC==rule.controllerNpc){controller=e;break;}
   if(controller==null||!AlteraSteamMajorParticleServiceAvailableV18)return;
   if(controller.hp<=controller.maxHp*rule.controllerShutdownHpFraction){actor.flag0=true;return;}
   if(actor.particleHandle==0){actor.particleHandle=++alteraSteamHandleV18;SteamPresentationV18(owner,actor,"create-particle",rule.sourceParticle,at);}
   actor.flag0=false;
  }
  void AdvanceAlteraSteamContactV18(Enemy owner,AlteraSteamActorV18 actor,AlteraSteamControllerRuleV18 rule,NpcStateDefinition state,float from,float to,float at){
   var hit=state.hits[0];float local=Math.Max(from,Math.Max(hit.start,actor.nextRequest));if(local>to||!NpcStateLinesTouchPlayer(owner,state,to))return;
   var request=new AlteraSteamDamageRequestV18{owner=owner,npc=owner.sourceNPC,sourceState=state.id,reaction=hit.reaction,time=at+Math.Max(0,local-from),localTime=local,sourceMagicMultiplier=hit.magic,sourceRepeatGap=hit.repeatGap,sourceBackSpeedY=rule.sourceBackSpeedY,sourceForceFly=rule.sourceForceFly,damage=(int)Math.Round(NpcProfile(owner).magicAttack*hit.magic)};
   if(AlteraSteamDamageConsumerV18==null){if(alteraSteamPendingDamageV18.Count>=256)alteraSteamPendingDamageV18.Dequeue();alteraSteamPendingDamageV18.Enqueue(request);actor.nextRequest=to+hit.repeatGap;return;}
   if(AlteraSteamDamageConsumerV18(request)>0)actor.nextRequest=to+hit.repeatGap;
  }
  bool TickAlteraSteamEnvironmentV18(Enemy owner,NpcDefinition profile,float dt){
   if(!ValidateAlteraSteamEnvironmentV18(profile))return false;AlteraSteamControllerRuleV18 rule;
   if(!alteraSteamRulesV18.TryGetValue(profile.id,out rule))throw new InvalidOperationException("Missing exact source Steam controller rule");
   if(!owner.active||owner.dormant)return true;AlteraSteamActorV18 actor;
   if(!alteraSteamActorsV18.TryGetValue(owner,out actor)){actor=new AlteraSteamActorV18{x=owner.x,y=owner.y,z=owner.nativeDepth,line=owner.groundLine,facing=owner.facing};alteraSteamActorsV18.Add(owner,actor);EnterAlteraSteamStateV18(owner,actor,rule,false,elapsed-dt);}
   owner.x=actor.x;owner.y=actor.y;owner.nativeDepth=actor.z;owner.groundLine=actor.line;owner.facing=actor.facing;owner.vx=owner.vy=0;owner.aiEnabled=false;
   if(owner.hp<=0){owner.deathTime+=dt;return true;}
   if(owner.hitPause>0){float freeze=Math.Min(dt,owner.hitPause);owner.hitPause-=freeze;dt-=freeze;if(dt<=.000001f)return true;}
   if(NpcState(owner)==null)EnterAlteraSteamStateV18(owner,actor,rule,false,elapsed-dt);
   if(owner.npcState==rule.waitState&&!actor.flag0)EnterAlteraSteamStateV18(owner,actor,rule,true,elapsed-dt);
   if(owner.npcState==rule.attackState){PollAlteraSteamControllerV18(owner,actor,rule,elapsed-dt);if(actor.flag0)EnterAlteraSteamStateV18(owner,actor,rule,false,elapsed-dt);}
   var state=NpcState(owner);float left=dt,at=elapsed-dt;owner.clock+=dt;
   while(left>.000001f){float from=owner.npcTime,span=Math.Min(left,state.duration-from),to=from+span;
    AdvanceNpcStateSounds(owner,state,from,to,at);
    if(state.id==rule.attackState){
     AdvanceAlteraSteamContactV18(owner,actor,rule,state,from,to,at);
     // AnimTimer_LUA's native repeated/edge behavior is not measured. These
     // two source thresholds are an explicit crossing adapter, never claimed
     // as native callback multiplicity or an accepted audio playback result.
     if(rule.npc=="NUI_STEAM_TRAP_DOWN"&&AlteraSteamMajorParticleServiceAvailableV18&&FirstAlteraSteamControllerHighV18(rule)&&Math.Abs(player.x-owner.x)<=420&&((player.x-owner.x)*(player.x-owner.x)+(player.y-owner.y)*(player.y-owner.y)<=420*420))foreach(float marker in new[]{.01f,2.97f})if(from<marker&&marker<=to)SteamSoundV18(owner,actor,rule.fireSound,"ATTACK_FRAME_MOVE.AnimTimer_LUA",marker,at+marker-from);
    }
    owner.npcTime=to;left-=span;at+=span;
    if(to>=state.duration-.000001f){owner.npcTime=0;actor.loop++;if(actor.nextRequest>0)actor.nextRequest-=state.duration;}
   }
   return true;
  }
  bool FirstAlteraSteamControllerHighV18(AlteraSteamControllerRuleV18 rule){foreach(var e in enemies)if(e!=null&&e.sourceNPC==rule.controllerNpc)return e.hp>e.maxHp*rule.controllerShutdownHpFraction;return false;}
 }
}
