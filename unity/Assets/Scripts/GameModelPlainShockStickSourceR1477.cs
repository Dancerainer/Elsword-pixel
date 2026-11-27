using System;
using System.Collections.Generic;

namespace ElswordPixel {
 // A read-only observation of the separately owned source state clock. The
 // original birth's AI/clear flags are not used as the trap's event scheduler.
 public sealed class PlainShockStickSourceFrameR1477 {
  public string State;public double StateAge,DecisionElapsed;public float PoseTime;
  public int AttackCallbacks,DamageEffects;public bool BornAI,CurrentAI,Hidden;
 }
 public sealed partial class GameModel {
  const string PlainShockNpcR1477="NUI_SHOCK_STICK";
  const string PlainShockWaitStartR1477="SHOCK_STICK_WAITSTART",PlainShockReadyR1477="SHOCK_STICK_ATTACK_A_READY";
  const string PlainShockAttackR1477="SHOCK_STICK_ATTACK",PlainShockWaitR1477="SHOCK_STICK_WAIT",PlainShockHideR1477="SHOCK_STICK_HIDE";
  const string PlainShockDamageR1477="SHOCK_STICK_DAMAGE",PlainShockDyingR1477="SHOCK_STICK_DYING";
  // The old generic rules compiler floors this loop's duration at .5. Its
  // actual WaitStart motion lasts .4; state age and the .8 event are separate.
  const float PlainShockWaitMotionR1477=.4f,PlainShockEventIntervalR1477=.8f;
  // Actual original Dummy01 at AttackA_Fire .1, with the existing ObjectMotion
  // actor-foot projection. Proof 1b320a9069726b989cd3e1a6e8a21418ed03feee1ad9d389db88cd7afcf48f0c.
  // Motion 8e23baa922f0833ee097abfb0c9f5d6e7400ff3d4ff11fa3d55396b60e29a8ec.
  const float PlainShockBoneXR1477=1.0327817525621366f,PlainShockBoneYR1477=37.121305502277195f,PlainShockBoneZR1477=-.00009298773547089584f;
  sealed class PlainShockActorR1477 {
   internal Enemy enemy;internal NpcDefinition profile;internal RoomDefinition room;
   internal string state;internal double age,decision;internal float x,y;internal int line;
   internal bool bornAI,emitted,hidden;internal int attackCallbacks,damageEffects;
  }
  readonly Dictionary<Enemy,PlainShockActorR1477> plainShockActorsR1477=new Dictionary<Enemy,PlainShockActorR1477>();

  bool PlainShockScopeR1477(Enemy enemy,NpcDefinition profile){
   return enemy!=null&&profile!=null&&NativeTerrain&&plainRouteR1258==PlainNormalR1258&&PlainOriginalRoomScopeR1258(plainRouteR1258)&&
    enemy.sourceNPC==PlainShockNpcR1477&&enemy.kind=="shock-stick"&&enemies.Contains(enemy)&&plainDefinitionsR1258!=null&&
    profile.id==enemy.sourceNPC&&profile.kind==enemy.kind&&ReferenceEquals(NpcProfile(enemy),profile)&&Array.Exists(plainDefinitionsR1258,p=>ReferenceEquals(p,profile));
  }
  // Ordinary incoming attacks otherwise enable disabled combat brains. This
  // exact Plain trap keeps its original value; the owner below does not set it.
  bool PreservePlainShockStickAiR1477(Enemy enemy){return enemy!=null&&PlainShockScopeR1477(enemy,NpcProfile(enemy));}
  void ClearPlainShockStickSourceR1477(){plainShockActorsR1477.Clear();ClearPlainShockParticleEventsR1480();}
  public bool TryReadPlainShockStickSourceR1477(Enemy enemy,out PlainShockStickSourceFrameR1477 frame){
   frame=null;if(enemy==null)return false;PlainShockActorR1477 owner;
   if(!plainShockActorsR1477.TryGetValue(enemy,out owner)||!PlainShockScopeR1477(enemy,owner.profile)||!ReferenceEquals(owner.room,CurrentRoom))return false;
   frame=new PlainShockStickSourceFrameR1477{State=enemy.npcState,StateAge=owner.age,DecisionElapsed=owner.decision,PoseTime=enemy.npcTime,
    AttackCallbacks=owner.attackCallbacks,DamageEffects=owner.damageEffects,BornAI=owner.bornAI,CurrentAI=enemy.aiEnabled,Hidden=owner.hidden};return true;
  }
  static void ValidatePlainShockProfileR1477(NpcDefinition p){
   if(p.initialState!=PlainShockWaitStartR1477||p.states==null||p.states.Length!=7||p.reactions==null||p.reactions.Length!=1||
    p.reactions[0].combatState!=CombatState.Hurt||p.reactions[0].sourceState!=PlainShockDamageR1477||p.reactions[0].sourceAnimation!="Damage"||p.reactions[0].duration!=38f/60||p.reactions[0].sourceAnimSpeed!=1)
    throw new InvalidOperationException("Exact original Plain ShockStick START/seven states/Damage reaction required");
   string[] ids={PlainShockWaitStartR1477,PlainShockReadyR1477,PlainShockAttackR1477,PlainShockWaitR1477,PlainShockHideR1477,PlainShockDamageR1477,PlainShockDyingR1477};
   string[] clips={"WaitStart","AttackA_Ready","AttackA_Fire","Wait","Hide","Damage","WaitStart"};
   string[] next={PlainShockReadyR1477,PlainShockAttackR1477,PlainShockWaitR1477,PlainShockHideR1477,PlainShockWaitStartR1477,PlainShockWaitR1477,PlainShockWaitR1477};
   float[] durations={.5f,1.8f,28f/60,134f/60,158f/60,38f/60,.4f};
   for(int i=0;i<ids.Length;i++){
    var s=Array.Find(p.states,x=>x!=null&&x.id==ids[i]);
    if(s==null||s.sourceAnimation!=clips[i]||s.sourceAnimSpeed!=1||s.duration!=durations[i]||s.loop!=(i==0)||s.nextState!=next[i]||
     s.followTarget!=(i==1)||s.superArmor!=(i==2)||s.hits==null||s.hits.Length!=0||s.choices==null||s.choices.Length!=0||s.velocities==null||s.velocities.Length!=(i==2||i==3?1:0))
     throw new InvalidOperationException("Changed original ShockStick state/motion/clock contract "+ids[i]);
    if(s.velocities.Length==1){var v=s.velocities[0];if(v==null||v.time!=0||v.x!=0||v.y!=0||v.mode!=2||!v.sourceSetX||!v.sourceSetY)throw new InvalidOperationException("Changed original ShockStick zero-speed entry");}
   }
   var wait=Array.Find(p.states,s=>s.id==PlainShockWaitStartR1477);var hide=Array.Find(p.states,s=>s.id==PlainShockHideR1477);
   if(wait.interval!=.8f||wait.invincibleStart!=0||wait.invincibleEnd!=100||hide.invincibleStart!=2.37f||hide.invincibleEnd!=100)
    throw new InvalidOperationException("Changed original ShockStick waiting/poll/invulnerability contract");
  }
  void EnterPlainShockStateR1477(PlainShockActorR1477 owner,string id){
   NpcStateDefinition state;if(!npcStates.TryGetValue(PlainShockNpcR1477+"/"+id,out state)||!Array.Exists(owner.profile.states,s=>ReferenceEquals(s,state)))
    throw new InvalidOperationException("Original ShockStick state is not installed "+id);
   owner.state=id;owner.age=owner.decision=0;owner.emitted=false;
   EnterNpcState(owner.enemy,state);
   if(id==PlainShockDyingR1477){
    // Original DYING_STATE_START creates four MajorParticle sequences, then
    // hides the actor. This is a value-only presentation event; the existing
    // sound/show/AI/damage/clock behavior below remains the source owner.
    RecordPlainShockParticleEventR1480(owner.enemy,2,elapsed);
    owner.hidden=true;
    if(!npcSourceSoundAssetsV17.Contains("npc-explosion"))throw new InvalidOperationException("Original ShockStick dying sound is not installed");
    QueueNpcSound(new NpcTimedSound{id=PlainShockNpcR1477+"/SHOCK_STICK_DYING_STATE_START/PlaySound_LUA",sound="npc-explosion",sourceField="SHOCK_STICK_DYING_STATE_START.PlaySound_LUA",sourceName="Explosion.ogg",sourceTime=0,candidateTime=0},
     owner.enemy,++npcSoundEntrySerial,0,elapsed,id);
    TraceNpc(owner.enemy,"source-show-false",-1,0,0);
   }
  }
  void ValidatePlainShockEffectsR1477(Enemy enemy){
   if(!PlainOriginalFxScopeR1304())throw new InvalidOperationException("Original Plain effects must precede the ShockStick source owner");
   for(int i=1;i<=2;i++){
    NpcEffectDefinition d;string id="SHOCK_STICK_ATTACK"+i;
    if(!npcEffectDefinitions.TryGetValue(id,out d)||d.lifetime!=.5f||d.physical!=0||d.magic!=(i==1?.5f:1)||d.repeat||d.reaction!=HitReaction.Hurt)
     throw new InvalidOperationException("Changed original ShockStick damage effect "+id);
   }
   var binding=EffectBinding(enemy,PlainShockAttackR1477);
   if(binding==null||binding.unitScale!=1||binding.tracks.Length!=0||binding.sets.Length!=0)throw new InvalidOperationException("Changed original ShockStick empty track/set binding");
  }
  void EmitPlainShockAttackR1477(PlainShockActorR1477 owner,float at){
   ValidatePlainShockEffectsR1477(owner.enemy);
   // The original callback copies Dummy01 twice. Its x-=90 and x+=90 are
   // world-X operations after GetBonePos, not actor-local facing offsets.
   // NpcAttachment already mirrors the bone, so cancel that mirror only for
   // the additional world-side displacement. LandPosition.y is a separate
   // preserved emission argument; it does not replace Dummy01's birth height.
   for(int i=1;i<=2;i++){
    string id="SHOCK_STICK_ATTACK"+i;float worldSide=(i==1?-90:90)*.28f;
    var emission=new NpcEffectEmission{effect=id,sourcePlainCallbackStateR1315=PlainShockAttackR1477,start=.1f,
     offsetX=PlainShockBoneXR1477+owner.enemy.facing*worldSide,offsetY=PlainShockBoneYR1477,offsetZ=PlainShockBoneZR1477,sourceLandPositionYArgument=true};
    SpawnNpcEffect(owner.enemy,npcEffectDefinitions[id],emission,at);owner.damageEffects++;
   }
   owner.attackCallbacks++;TraceNpc(owner.enemy,"source-attack-frame-callback",-1,0,.1f);
  }
  bool TickPlainShockStickSourceR1477(Enemy enemy,NpcDefinition profile,float dt){
   if(!PlainShockScopeR1477(enemy,profile))return false;
   if(!enemy.active||enemy.dormant)return true;
   PlainShockActorR1477 owner;
   if(!plainShockActorsR1477.TryGetValue(enemy,out owner)){
    ValidatePlainShockProfileR1477(profile);
    var stale=new List<Enemy>();foreach(var pair in plainShockActorsR1477)if(!enemies.Contains(pair.Key)||!ReferenceEquals(pair.Value.room,CurrentRoom))stale.Add(pair.Key);foreach(var old in stale)plainShockActorsR1477.Remove(old);
    owner=new PlainShockActorR1477{enemy=enemy,profile=profile,room=CurrentRoom,x=enemy.x,y=enemy.y,line=enemy.groundLine,bornAI=enemy.aiEnabled};plainShockActorsR1477.Add(enemy,owner);
   }
   if(!ReferenceEquals(owner.profile,profile)||!ReferenceEquals(owner.room,CurrentRoom))throw new InvalidOperationException("Stale original ShockStick source owner");
   // This is the source's stationary zero-speed mechanism. No generic AI,
   // patrol, gravity or damage-reaction displacement runs in addition to it.
   enemy.x=owner.x;enemy.y=owner.y;enemy.groundLine=owner.line;enemy.vx=enemy.vy=enemy.npcVelocityX=0;enemy.knockdownOnLanding=false;
   if(enemy.hp<=0){
    if(owner.state!=PlainShockDyingR1477||enemy.npcState!=PlainShockDyingR1477)EnterPlainShockStateR1477(owner,PlainShockDyingR1477);
    enemy.deathTime+=dt;
   }else if(string.IsNullOrEmpty(enemy.npcState)){
    // INIT_STATE maps every ordinary land/air/front/back damage type to this
    // same real Damage motion. The physical CombatState/stun are not cleared.
    EnterPlainShockStateR1477(owner,PlainShockDamageR1477);
   }else if(owner.state!=enemy.npcState){
    if(owner.state!=null)throw new InvalidOperationException("Unexpected external ShockStick source-state replacement");
    if(enemy.npcState!=PlainShockWaitStartR1477)throw new InvalidOperationException("Original ShockStick must start at WAITSTART");
    owner.state=enemy.npcState;owner.age=owner.decision=0;
   }
   float frozen=Math.Min(Math.Max(0,enemy.hitPause),dt);enemy.hitPause-=frozen;
   float remaining=dt-frozen,at=elapsed-dt+frozen;
   while(remaining>.000001f){
    var state=NpcState(enemy);if(state==null)throw new InvalidOperationException("Missing owned ShockStick state");
    if(state.id==PlainShockReadyR1477&&!Grounded(enemy))throw new InvalidOperationException("Original SHOCK_STICK_JUMP_DOWN target is absent from the seven-state source contract");
    float span=Math.Min(remaining,1f/120);if(!state.loop)span=Math.Min(span,Math.Max(0,state.duration-enemy.npcTime));
    if(span<=.000001f){if(enemy.hp<=0)break;EnterPlainShockStateR1477(owner,state.nextState);continue;}
    float from=enemy.npcTime;owner.age+=span;owner.decision+=span;enemy.clock+=span;
    if(state.id==PlainShockWaitStartR1477){
     // No sound/event exists in this loop. Wrap its actual .4-second motion,
     // while the independent condition age continues past .5 and 100 seconds.
     enemy.npcTime=(from+span)%PlainShockWaitMotionR1477;
    }else{
     if(state.followTarget&&player.hp>0&&Math.Abs(player.x-enemy.x)>.001f)enemy.facing=player.x>=enemy.x?1:-1;
     enemy.npcTime=Math.Min(state.duration,from+span);AdvanceNpcStateSounds(enemy,state,from,enemy.npcTime,at);
     if(state.id==PlainShockAttackR1477&&!owner.emitted&&from<.1f&&enemy.npcTime>=.1f){owner.emitted=true;EmitPlainShockAttackR1477(owner,at+Math.Max(0,.1f-from));RecordPlainShockParticleEventR1480(enemy,0,at+Math.Max(0,.1f-from));}
     if(state.id==PlainShockAttackR1477&&from<.233f&&enemy.npcTime>=.233f)RecordPlainShockParticleEventR1480(enemy,1,at+Math.Max(0,.233f-from));
    }
    at+=span;remaining-=span;
    if(state.id==PlainShockWaitStartR1477){
     if(owner.decision+1e-6>=PlainShockEventIntervalR1477){owner.decision-=PlainShockEventIntervalR1477;
      float dx=player.x-enemy.x,dy=player.y-enemy.y;
      if(owner.age>.5&&player.hp>0&&dx*dx+dy*dy<=280f*280f)EnterPlainShockStateR1477(owner,PlainShockReadyR1477);
     }
    }else if(enemy.npcTime+1e-6f>=state.duration&&enemy.hp>0)EnterPlainShockStateR1477(owner,state.nextState);
   }
   return true;
  }
 }
}
