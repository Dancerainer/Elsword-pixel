using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraNasodPopulationRuleV18 {
  public string npc,environmentKind,spawnNpc,initialState,waitState,readyState,respawnState,damageState,dyingState,sourceCondition,sourceFrameCallback;
  public float waitSeconds,sourceEmissionAnimationTime,sourceAnimationSpeed,emitAt,ownerHpCost;
  public int maximumLiveForBeginInclusive,sourceKeyCode;public bool ownerHpCostHostOnly;
 }
 [Serializable] public sealed class AlteraPopulationEventV18 {public string kind,npc,spawn;public float time,x,y,z,before,after;public int line,count;}
 public sealed partial class GameModel {
  readonly Dictionary<string,AlteraNasodPopulationRuleV18> alteraNasodPopulationRulesV18=new Dictionary<string,AlteraNasodPopulationRuleV18>();
  sealed class AlteraPopulationActorV18 {internal float age,x,y,depth;internal int line,facing;internal bool emitted;}
  readonly Dictionary<Enemy,AlteraPopulationActorV18> alteraPopulationActorsV18=new Dictionary<Enemy,AlteraPopulationActorV18>();
  public readonly List<AlteraPopulationEventV18> alteraPopulationEventsV18=new List<AlteraPopulationEventV18>();
  static bool ValidateAlteraPopulationEnvironmentV18(NpcDefinition p){
   if(ValidateAlteraSteamEnvironmentV18(p))return true;
   if(ValidateAlteraStaticShellEnvironmentV18(p))return true;
   bool house=p.id=="NUI_NASOD_MOUSE_HOUSE"&&p.environmentKind=="source-nasod-mouse-house",maker=p.id=="NUI_NASOD_MAKER"&&p.environmentKind=="source-nasod-maker";
   if(!house&&!maker)return false;
   if(p.senseRange!=0||p.lostRange!=0||p.approachDistance!=0||Math.Abs(p.bodyHeight-42)>.0001f||Math.Abs(p.bodyWidth-(house?14:42))>.0001f||p.bodySpheres.Length!=(house?4:8))throw new ArgumentException("Changed original stationary Nasod population geometry/detection");
   string prefix=house?"NASOD_MOUSEHOUSE":"NASOD_MAKER";
   foreach(string suffix in new[]{"START","WAIT","WAIT_RESPAWN_READY","RESPAWN","DAMAGE","DYING"})if(!Array.Exists(p.states,s=>s.id==prefix+"_"+suffix))throw new ArgumentException("Missing exact Nasod population source state");
   return true;
  }
  static bool ValidateAlteraStaticShellEnvironmentV18(NpcDefinition p){
   if(p.id!="NUI_CANNON_FORT_PARASITE_BODY_SHELL"||p.environmentKind!="source-cannon-fort-static-shell")return false;
   const string prefix="CANNON_FORT_PARASITE_BODY_SHELL_";
   if(p.bodyWidth!=140||p.bodyHeight!=280||p.senseRange!=0||p.lostRange!=0||Math.Abs(p.approachDistance-42)>.0001f||p.bodySpheres.Length!=0||p.reactions.Length!=0||p.states.Length!=3||p.waitState!=prefix+"WAIT"||p.walkState!=p.waitState)throw new ArgumentException("Changed exact source invincible static shell geometry/AI");
   foreach(string suffix in new[]{"WAIT","DAMAGE","DYING"}){var s=Array.Find(p.states,v=>v.id==prefix+suffix);if(s==null||s.sourceAnimation!="Body"||s.duration!=1||s.sourceAnimSpeed!=1||s.loop||s.hits.Length!=0||s.invincibleStart!=0||s.invincibleEnd!=(suffix=="DYING"?100:99999)||s.superArmor!=(suffix!="DYING")||s.samples[0].bodySpheres.Length!=0)throw new ArgumentException("Changed original shell state/invincibility");}
   return true;
  }
  bool TickAlteraStaticShellV18(Enemy owner,NpcDefinition profile,float dt){
   if(!ValidateAlteraStaticShellEnvironmentV18(profile))return false;if(!owner.active||owner.dormant)return true;
   AlteraPopulationActorV18 actor;if(!alteraPopulationActorsV18.TryGetValue(owner,out actor)){actor=new AlteraPopulationActorV18{x=owner.x,y=owner.y,depth=owner.nativeDepth,line=owner.groundLine,facing=owner.facing};alteraPopulationActorsV18.Add(owner,actor);EnterNpcState(owner,npcStates[profile.id+"/"+profile.waitState]);}
   owner.x=actor.x;owner.y=actor.y;owner.nativeDepth=actor.depth;owner.groundLine=actor.line;owner.facing=actor.facing;owner.aiEnabled=false;owner.vx=owner.vy=0;
   if(owner.hitPause>0){float frozen=Math.Min(dt,owner.hitPause);owner.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
   var state=NpcState(owner);if(owner.hp<=0&&state.id!="CANNON_FORT_PARASITE_BODY_SHELL_DYING"){EnterNpcState(owner,npcStates[profile.id+"/CANNON_FORT_PARASITE_BODY_SHELL_DYING"]);state=NpcState(owner);}
   owner.clock+=dt;float from=owner.npcTime;owner.npcTime=Math.Min(state.duration,from+dt);AdvanceNpcStateSounds(owner,state,from,owner.npcTime,elapsed-dt);
   if(owner.hp<=0)owner.deathTime+=dt;
   else if(state.id=="CANNON_FORT_PARASITE_BODY_SHELL_DAMAGE"&&owner.npcTime>=state.duration)EnterNpcState(owner,npcStates[profile.id+"/"+profile.waitState]);
   return true;
  }
  public void SetAlteraNasodPopulationRulesV18(AlteraNasodPopulationRuleV18[] definitions){
   var next=new Dictionary<string,AlteraNasodPopulationRuleV18>();
   foreach(var r in definitions??new AlteraNasodPopulationRuleV18[0]){
    bool house=r!=null&&r.npc=="NUI_NASOD_MOUSE_HOUSE",maker=r!=null&&r.npc=="NUI_NASOD_MAKER";string prefix=house?"NASOD_MOUSEHOUSE":"NASOD_MAKER";
    if(!house&&!maker||next.ContainsKey(r.npc)||r.environmentKind!=(house?"source-nasod-mouse-house":"source-nasod-maker")||r.spawnNpc!=(house?"NUI_NASOD_MOUSE":"NUI_WALLY_9TH_TYPE2")||r.initialState!=prefix+"_START"||r.waitState!=prefix+"_WAIT"||r.readyState!=prefix+"_WAIT_RESPAWN_READY"||r.respawnState!=prefix+"_RESPAWN"||r.damageState!=prefix+"_DAMAGE"||r.dyingState!=prefix+"_DYING"||
     r.waitSeconds!=(house?5:3)||r.sourceEmissionAnimationTime!=(house?.4f:2)||r.sourceAnimationSpeed!=1||r.emitAt!=r.sourceEmissionAnimationTime/r.sourceAnimationSpeed||r.maximumLiveForBeginInclusive!=(house?3:4)||r.ownerHpCost!=(house?500:0)||r.ownerHpCostHostOnly!=house||r.sourceKeyCode!=0||r.sourceCondition!="CF_"+prefix+"_RESPAWN"||r.sourceFrameCallback!=prefix+"_RESPAWN_FRAME_MOVE")throw new ArgumentException("Changed exact original Nasod population contract");
    NpcDefinition profile;NpcStateDefinition active;
    if(!npcDefinitions.TryGetValue(r.npc,out profile)||!ValidateAlteraPopulationEnvironmentV18(profile)||!npcDefinitions.ContainsKey(r.spawnNpc)||!npcStates.TryGetValue(r.npc+"/"+r.respawnState,out active)||active.sourceAnimSpeed!=r.sourceAnimationSpeed||r.emitAt>active.duration||npcStates[r.npc+"/"+r.waitState].duration!=r.waitSeconds)throw new ArgumentException("Unbound source Nasod population state/identity");
    next.Add(r.npc,r);
   }
   alteraNasodPopulationRulesV18.Clear();foreach(var r in next)alteraNasodPopulationRulesV18.Add(r.Key,r.Value);ClearAlteraPopulationActorsV18();
  }
  void ClearAlteraPopulationActorsV18(){alteraPopulationActorsV18.Clear();alteraPopulationEventsV18.Clear();ClearAlteraSteamActorsV18();}
  void PopulationTraceV18(string kind,Enemy owner,AlteraNasodPopulationRuleV18 rule,float at,int count=0,float before=0,float after=0){
   if(alteraPopulationEventsV18.Count>=256)alteraPopulationEventsV18.RemoveAt(0);
   alteraPopulationEventsV18.Add(new AlteraPopulationEventV18{kind=kind,npc=owner.sourceNPC,spawn=rule.spawnNpc,time=at,x=owner.x,y=owner.y,z=owner.nativeDepth,line=owner.groundLine,count=count,before=before,after=after});
  }
  int AlteraPopulationCountV18(string npc){int factoryCount;if(TryFactoryPopulationCountV21(npc,out factoryCount))return factoryCount;int count=0;foreach(var e in enemies)if(e.active&&!e.dormant&&e.hp>0&&e.sourceNPC==npc)count++;foreach(var r in npcSummonRequests)if(r.room==room&&r.alreadyEmitted&&r.npc==npc)count++;return count;}
  void EnterAlteraPopulationStateV18(Enemy owner,AlteraPopulationActorV18 actor,AlteraNasodPopulationRuleV18 rule,string state){actor.age=0;actor.emitted=false;EnterNpcState(owner,npcStates[rule.npc+"/"+state]);PopulationTraceV18("state",owner,rule,elapsed);}
  void EmitAlteraPopulationV18(Enemy owner,AlteraNasodPopulationRuleV18 rule,float at){
   var original=SummonTemplate(rule.spawnNpc);if(original==null)throw new InvalidOperationException("Source population request has no exact current-room identity/balance template: "+rule.spawnNpc);
   // Preserve the exact authored normal-room balance template, while dynamic
   // position/depth/facing belongs to the source owner.GetPos/GetIsRight call.
   var template=new EnemySpawn{kind=original.kind,sourceNPC=original.sourceNPC,hp=original.hp,visualScale=original.visualScale,nativeDepth=owner.nativeDepth};
   npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=template,npc=rule.spawnNpc,room=room,encounter=owner.encounterIndex,line=owner.groundLine,facing=owner.facing,x=owner.x,y=owner.y,due=at,alreadyEmitted=true});
   PopulationTraceV18("source-create-request",owner,rule,at);
   if(rule.ownerHpCost>0&&(!rule.ownerHpCostHostOnly||NpcMechanismIsHost)){
    float before=owner.hp;owner.hp=Math.Max(0,owner.hp-rule.ownerHpCost);PopulationTraceV18("source-host-hp-cost",owner,rule,at,0,before,owner.hp);
    if(owner.hp<=0){owner.state=CombatState.Dead;owner.stun=0;InterruptNpcAction(owner);}
   }
  }
  // Exact source-specific cave hook. Only these two original source identities
  // and their separately validated contracts are accepted.
  bool TickAlteraPopulationEnvironmentV18(Enemy owner,NpcDefinition profile,float dt){
   if(TickAlteraSteamEnvironmentV18(owner,profile,dt))return true;
   if(TickAlteraStaticShellV18(owner,profile,dt))return true;
   if(!ValidateAlteraPopulationEnvironmentV18(profile))return false;
   AlteraNasodPopulationRuleV18 rule;if(!alteraNasodPopulationRulesV18.TryGetValue(profile.id,out rule))throw new InvalidOperationException("Missing exact source Nasod population consumer contract");
   if(!owner.active||owner.dormant)return true;
   AlteraPopulationActorV18 actor;if(!alteraPopulationActorsV18.TryGetValue(owner,out actor)){actor=new AlteraPopulationActorV18{x=owner.x,y=owner.y,depth=owner.nativeDepth,line=owner.groundLine,facing=owner.facing};alteraPopulationActorsV18.Add(owner,actor);EnterAlteraPopulationStateV18(owner,actor,rule,rule.initialState);}
   owner.x=actor.x;owner.y=actor.y;owner.nativeDepth=actor.depth;owner.groundLine=actor.line;owner.facing=actor.facing;owner.vx=owner.vy=0;
   if(owner.hitPause>0){float frozen=Math.Min(dt,owner.hitPause);owner.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
   var state=NpcState(owner);
   if(owner.hp<=0){if(state==null||state.id!=rule.dyingState){EnterAlteraPopulationStateV18(owner,actor,rule,rule.dyingState);state=NpcState(owner);}owner.deathTime+=dt;}
   else if(owner.state!=CombatState.Normal&&(state==null||state.id!=rule.damageState)){EnterAlteraPopulationStateV18(owner,actor,rule,rule.damageState);state=NpcState(owner);}
   else if(state==null){EnterAlteraPopulationStateV18(owner,actor,rule,rule.waitState);state=NpcState(owner);}
   if(owner.hp>0&&state.id==rule.readyState){int population=AlteraPopulationCountV18(rule.spawnNpc);if(population<=rule.maximumLiveForBeginInclusive){PopulationTraceV18("source-population-condition",owner,rule,elapsed,population);EnterAlteraPopulationStateV18(owner,actor,rule,rule.respawnState);return true;}}
   float from=actor.age;actor.age+=dt;float to=Math.Min(state.duration,actor.age);owner.clock+=dt;owner.npcTime=to;AdvanceNpcStateSounds(owner,state,Math.Min(from,state.duration),to,elapsed-dt);
   if(owner.hp>0&&state.id==rule.respawnState&&!actor.emitted&&from<rule.emitAt&&rule.emitAt<=actor.age){actor.emitted=true;EmitAlteraPopulationV18(owner,rule,elapsed-dt+rule.emitAt-from);if(owner.hp<=0)return true;}
   if(owner.hp<=0)return true;
   if(state.id==rule.waitState&&actor.age+1e-6f>=rule.waitSeconds)EnterAlteraPopulationStateV18(owner,actor,rule,rule.readyState);
   else if(state.id!=rule.waitState&&actor.age+1e-6f>=state.duration){if(state.id==rule.damageState){owner.state=CombatState.Normal;owner.stun=0;}EnterAlteraPopulationStateV18(owner,actor,rule,rule.waitState);}
   return true;
  }
 }
}
