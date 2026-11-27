using System;
using System.Collections.Generic;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // Demo reaction adapter: this Cargo profile declares DamageSmall/Hurt only.
  // Generic launch/down are mapped to that available reaction, not a claim of
  // native knockdown immunity. State-local superarmor/invincibility is unchanged.
  HitReaction CargoSourceGroundReactionV1468(Enemy e,HitReaction requested){
   if(!CargoWallyBoundV21(e)||e.hp<=0||(requested!=HitReaction.Launch&&requested!=HitReaction.Knockdown))return requested;
   var p=NpcProfile(e);if(p==null||p.reactions.Length!=1||p.reactions[0].combatState!=CombatState.Hurt||p.reactions[0].sourceState!="WALLY_9TH_ARMOR_DAMAGE_SMALL")return requested;
   return HitReaction.Hurt;
  }
  // Cargo-only physical-reaction adapter. Reuse this Bee's registered source
  // samples without mutating the frozen profile, HP, AI or generic movement.
  NpcDefinition cargoBeeOriginalR1484,cargoBeeOverlayR1484;
  void ClearCargoBeeReactionR1484(){cargoBeeOriginalR1484=cargoBeeOverlayR1484=null;}
  bool CargoBeeBirthBoundR1484(Enemy e,NpcDefinition p){
   NpcDefinition registered;
   return cargoSourceV21!=null&&cargoSourceV21.sourceRulesSha256=="b2a3dd831d9cdbffd1cac400400af1cdf573a146c250a6007df6618c2018f20e"&&NativeTerrain&&e!=null&&p!=null&&e.sourceNPC=="NUI_BEE_NASOD"&&e.kind=="bee-nasod"&&p.id==e.sourceNPC&&p.kind==e.kind&&enemies.Contains(e)&&CurrentRoom!=null&&CurrentRoom.id=="Dungeon_Besma_Air_Ship_Normal-"+room&&npcDefinitions.TryGetValue(p.id,out registered)&&object.ReferenceEquals(p,registered)&&CurrentRoom.enemies.Exists(q=>q.sourceNPC==e.sourceNPC&&q.kind==e.kind&&q.sourceKeyCode==e.sourceKeyCode&&q.encounterIndex==e.encounterIndex&&q.x==e.spawnX);
  }
  NpcStateDefinition CargoBeeStateR1484(NpcDefinition p,string id,string animation,float duration,int frames){
   var s=Array.Find(p.states,q=>q.id==id);NpcStateDefinition registered;
   if(s==null||!npcStates.TryGetValue(p.id+"/"+id,out registered)||!object.ReferenceEquals(s,registered)||s.sourceAnimation!=animation||s.duration!=duration||s.sourceAnimSpeed!=1||s.loop||s.samples==null||s.samples.Length!=frames)throw new InvalidOperationException("Cargo Bee original reaction state changed "+id);
   foreach(var f in s.samples)if(f==null||f.bodySpheres==null||f.bodySpheres.Length!=12)throw new InvalidOperationException("Cargo Bee original reaction body changed "+id);
   return s;
  }
  static NpcReactionBody CargoBeeBodyR1484(CombatState state,NpcStateDefinition source){return new NpcReactionBody{combatState=state,sourceState=source.id,sourceAnimation=source.sourceAnimation,duration=source.duration,sourceAnimSpeed=source.sourceAnimSpeed,samples=source.samples};}
  NpcDefinition CargoBeeProfileR1484(Enemy e,NpcDefinition p){
   if(!CargoBeeBirthBoundR1484(e,p))return p;
   if(object.ReferenceEquals(p,cargoBeeOriginalR1484)&&cargoBeeOverlayR1484!=null)return cargoBeeOverlayR1484;
   if(p.reactions.Length!=1||p.reactions[0].combatState!=CombatState.Hurt||p.reactions[0].sourceState!="BEE_NASOD_DAMAGE"||!string.IsNullOrEmpty(p.getUpState))throw new InvalidOperationException("Cargo Bee original profile reaction contract changed");
   var down=CargoBeeStateR1484(p,"BEE_NASOD_DAMAGE_DOWN","DamageDown",47f/30,95);var up=CargoBeeStateR1484(p,"BEE_NASOD_DAMAGE_STAND_UP","DamageStandUp",28f/30,57);
   if(down.nextState!=up.id||up.nextState!=p.waitState)throw new InvalidOperationException("Cargo Bee original recovery graph changed");
   var overlay=new NpcDefinition{runtimeAdmitted=p.runtimeAdmitted,sourceMechanismsComplete=p.sourceMechanismsComplete,id=p.id,kind=p.kind,waitState=p.waitState,walkState=p.walkState,getUpState=up.id,environmentKind=p.environmentKind,initialState=p.initialState,environment=p.environment,traversal=p.traversal,summon=p.summon,senseRange=p.senseRange,lostRange=p.lostRange,approachDistance=p.approachDistance,physicalAttack=p.physicalAttack,magicAttack=p.magicAttack,bodyWidth=p.bodyWidth,bodyHeight=p.bodyHeight,bodySpheres=p.bodySpheres,states=p.states,reactions=new[]{p.reactions[0],CargoBeeBodyR1484(CombatState.Knockdown,down),CargoBeeBodyR1484(CombatState.GettingUp,up)}};
   cargoBeeOriginalR1484=p;cargoBeeOverlayR1484=overlay;return overlay;
  }
  bool CargoBeePhysicalBoundR1484(Enemy e){NpcDefinition p;return e!=null&&e.sourceNPC=="NUI_BEE_NASOD"&&npcDefinitions.TryGetValue(e.sourceNPC,out p)&&CargoBeeBirthBoundR1484(e,p);}
  float CargoBeeDownDurationR1484(Fighter f,float fallback){var e=f as Enemy;if(!CargoBeePhysicalBoundR1484(e))return fallback;return Array.Find(CargoBeeProfileR1484(e,npcDefinitions[e.sourceNPC]).reactions,q=>q.combatState==CombatState.Knockdown).duration;}
  // TickState precedes TickEnemyCore's hit-pause consumption. Keep only this
  // bound Bee's physical down/recovery timer on the same unfrozen packet time.
  float CargoBeeReactionStepR1484(Fighter f,float dt){return CargoBeePhysicalBoundR1484(f as Enemy)?Math.Max(0,dt-Math.Min(Math.Max(0,f.hitPause),dt)):dt;}
  // Same original-birth registration guard for the finite five early Cargo
  // identities. Ground reaction overlays/timers remain Bee-only above.
  NpcDefinition CargoPhysicalAirProfileR1484(Enemy e,out float duration,out int frames,out int bodyCoordinates){
   duration=0;frames=bodyCoordinates=0;if(e==null)return null;string kind;
   switch(e.sourceNPC){
    case "NUI_BEE_NASOD":kind="bee-nasod";duration=.5f;frames=31;bodyCoordinates=12;break;
    case "NUI_NASOD_HEALER":kind="nasod-healer";duration=1;frames=61;bodyCoordinates=4;break;
    case "NUI_NASOD_MINER_THIN_AIR":kind="nasod-miner-thin-air";duration=1;frames=61;bodyCoordinates=12;break;
    case "NUI_NASOD_MINER_FAT_B_AIR":kind="nasod-miner-fat-b-air";duration=28f/30;frames=57;bodyCoordinates=8;break;
    case "NUI_NASOD_MINER_FAT_C_AIR":kind="nasod-miner-fat-c-air";duration=28f/30;frames=57;bodyCoordinates=8;break;
    default:return null;
   }
   NpcDefinition p;if(cargoSourceV21==null||cargoSourceV21.sourceRulesSha256!="b2a3dd831d9cdbffd1cac400400af1cdf573a146c250a6007df6618c2018f20e"||!NativeTerrain||e.kind!=kind||!enemies.Contains(e)||CurrentRoom==null||CurrentRoom.id!="Dungeon_Besma_Air_Ship_Normal-"+room||!npcDefinitions.TryGetValue(e.sourceNPC,out p)||p.id!=e.sourceNPC||p.kind!=kind||!CurrentRoom.enemies.Exists(q=>q.sourceNPC==e.sourceNPC&&q.kind==e.kind&&q.sourceKeyCode==e.sourceKeyCode&&q.encounterIndex==e.encounterIndex&&q.x==e.spawnX))return null;
   return p;
  }
  string CargoBeePhysicalAirClipR1484(Enemy e,IDictionary<string,CargoProductStateClipR1261> clips,out float clock){
   clock=0;if(e==null||e.hp<=0||e.state!=CombatState.Airborne||!string.IsNullOrEmpty(e.npcState)||(!e.knockdownOnLanding&&e.clip!="launched"))return null;
   float duration;int frames,coordinates;var p=CargoPhysicalAirProfileR1484(e,out duration,out frames,out coordinates);if(p==null)return null;
   // The unchanged demo launch/gravity chooses each NPC's own original up/fall
   // pose; no native front/back dispatch, back-slide, armor or landing is added.
   bool up=e.vy>0;string state=p.id.Substring(4)+(up?"_DAMAGE_AIR_UP":"_DAMAGE_AIR_FALL"),animation=up?"DamageAirUp":"DamageAirFall";NpcStateDefinition s;
   if(!npcStates.TryGetValue(p.id+"/"+state,out s)||!object.ReferenceEquals(Array.Find(p.states,q=>q.id==state),s)||s.sourceAnimation!=animation||s.duration!=duration||s.sourceAnimSpeed!=1||s.loop||!s.holdForSourceEvent||s.samples==null||s.samples.Length!=frames)throw new InvalidOperationException("Cargo original airborne source state changed "+p.id+"/"+state);
   foreach(var f in s.samples)if(f==null||f.bodySpheres==null||f.bodySpheres.Length!=coordinates)throw new InvalidOperationException("Cargo original airborne body changed "+p.id+"/"+state);
   CargoProductStateClipR1261 row;if(clips==null||!clips.TryGetValue(p.id+"/"+state,out row)||row==null||row.npc!=e.sourceNPC||row.kind!=e.kind||row.state!=s.id||row.sourceAnimation!=s.sourceAnimation||row.sourceStateDuration!=s.duration||row.displayClip!=s.clip)throw new InvalidOperationException("Cargo original airborne display binding absent or changed "+p.id+"/"+state);
   clock=Clamp(e.clock,0,s.duration);return row.displayClip;
  }
  // Explicit 2D static-prop presentation policy for the one original Cargo
  // BossPost held identity. Its five named motions remain absent/unbound; this
  // selects only its OWN existing rest frame, not an idle/FSM/animation alias.
  // The caller retains real active/dormant visibility and deathTime alpha fade.
  public string CargoHeldPostStaticDisplayR1485(Enemy e){
   const string npc="NUI_SMALLGATE_BOSS_POST_BESMA_AIR_SHIP",kind="smallgate-boss-post-besma-air-ship";
   if(cargoSourceV21==null||cargoSourceV21.sourceRulesSha256!="b2a3dd831d9cdbffd1cac400400af1cdf573a146c250a6007df6618c2018f20e"||cargoSourceV21.heldNpcIds==null||cargoSourceV21.heldNpcIds.Length!=1||cargoSourceV21.heldNpcIds[0]!=npc||!NativeTerrain||room!=1||CurrentRoom==null||CurrentRoom.id!="Dungeon_Besma_Air_Ship_Normal-1"||e==null||e.sourceNPC!=npc||e.kind!=kind||!enemies.Contains(e)||!e.isGate||e.countsForClear||e.aiEnabled||npcDefinitions.ContainsKey(npc))return null;
   if(!CurrentRoom.enemies.Exists(q=>q.sourceNPC==npc&&q.kind==kind&&q.isGate&&!q.countsForClear&&!q.aiEnabled&&q.encounterIndex==e.encounterIndex&&q.sourceKeyCode==e.sourceKeyCode&&q.x==e.spawnX&&q.hp==e.maxHp))return null;
   return "unbound-rest";
  }
  // Shared production lookup is pure model/DTO code so the actual rendering
  // decision can be regressed without instantiating Unity or reading artwork.
  public string CargoSourceReactionClipV1468(Enemy e,IDictionary<string,CargoProductStateClipR1261> clips,out float clock){
   clock=0;if(cargoSourceV21==null||e==null||e.hp<=0)return null;var p=NpcProfile(e);if(p==null)return null;
   var air=CargoBeePhysicalAirClipR1484(e,clips,out clock);if(air!=null)return air;
   var body=Array.Find(p.reactions,q=>q.combatState==e.state);if(body==null)return null;
   CargoProductStateClipR1261 row;
   if(clips==null||!clips.TryGetValue(e.sourceNPC+"/"+body.sourceState,out row)||row==null||row.npc!=e.sourceNPC||row.kind!=e.kind||row.state!=body.sourceState||row.sourceAnimation!=body.sourceAnimation||row.sourceStateDuration!=body.duration||string.IsNullOrEmpty(row.displayClip))throw new InvalidOperationException("Cargo declared reaction display binding absent or changed "+e.sourceNPC+"/"+body.sourceState);
   clock=Math.Min(Math.Max(0,e.clock),body.duration);return row.displayClip;
  }
  void EnsureCargoSourceInitialStateV21(Enemy e){
   if(cargoSourceV21==null||e==null||cargoInitializedV21.Contains(e)||!e.active||e.dormant||e.hp<=0||!e.aiEnabled||e.stun>0||e.state!=CombatState.Normal)return;var init=Array.Find(cargoSourceV21.initialStates,q=>q.npc==e.sourceNPC);if(init==null)return;if(!string.IsNullOrEmpty(e.npcState)){cargoInitializedV21.Add(e);return;}if(TryEnterNpcState(e,npcStates[e.sourceNPC+"/"+init.state]))cargoInitializedV21.Add(e);
  }
  bool CargoSourceScriptedStartActiveV21(Enemy e){if(CargoWallyBoundV21(e)&&e.npcState=="WALLY_9TH_ARMOR_START")return true;return cargoSourceV21!=null&&e!=null&&Array.Exists(cargoSourceV21.initialStates,q=>q.npc==e.sourceNPC&&q.state==e.npcState)&&e.npcState!="WALLY_9TH_ARMOR_START_READY";}
  void BeginCargoSourceRoomV21(){if(cargoSourceV21==null)return;BeginCargoDirectorRoomV21();foreach(var e in enemies)if(CargoWallyBoundV21(e)&&e.active&&!e.dormant&&e.hp>0&&!e.aiEnabled){EnterNpcState(e,npcStates[e.sourceNPC+"/WALLY_9TH_ARMOR_START_READY"]);if(SimpleDangerIntroductions)e.aiEnabled=true;}}
  // Called only by a future source movie/director dispatcher. This is the
  // explicit STOP_AT_START model adapter for the original director callback;
  // no clock, proximity or auto-playtest path invents this native invocation.
  bool ReleaseCargoBossFromBoundDirectorCoreV21(string sourceCallback){
   if(cargoSourceV21==null||sourceCallback!="EVENT_NPC_WALLY_9TH_ARMOR_DYING_START_START")return false;var e=enemies.Find(q=>CargoWallyBoundV21(q)&&q.active&&!q.dormant&&q.hp>0);if(e==null||e.npcState!="WALLY_9TH_ARMOR_START_READY")return false;e.aiEnabled=true;EnterNpcState(e,npcStates[e.sourceNPC+"/WALLY_9TH_ARMOR_START"]);CargoEventV21(e,"source-director-release",sourceCallback,elapsed,e.x,e.y);return true;
  }
  static readonly NpcCondition[] CargoNoFallbackChoicesV21=new NpcCondition[0];
  bool CargoDecisionBoundV21(Enemy e,NpcStateDefinition s){return CargoWallyBoundV21(e)&&(s.id=="WALLY_9TH_ARMOR_WAIT"||s.id=="WALLY_9TH_ARMOR_WALK");}
  bool TryCargoSourceImmediateV21(Enemy e,NpcStateDefinition s){if(!CargoWallyBoundV21(e)||player.hp>0||s.id=="WALLY_9TH_ARMOR_WIN"||Array.IndexOf(cargoSourceV21.winStates,s.id)<0)return false;return TryEnterNpcState(e,npcStates[e.sourceNPC+"/WALLY_9TH_ARMOR_WIN"]);}
  IEnumerable<NpcCondition> CargoFallbackChoicesV21(Enemy e,NpcStateDefinition s,NpcCondition[] fallback){return CargoDecisionBoundV21(e,s)?CargoNoFallbackChoicesV21:fallback;}
  public static bool CargoDecisionDistanceV21(bool far,float sourceDistance,float threshold){return far?sourceDistance>threshold:sourceDistance<=threshold;}
  bool TryCargoSourceDecisionV21(Enemy e,NpcStateDefinition s,float projectedDistance){
   if(!CargoDecisionBoundV21(e,s))return false;float native=projectedDistance/.28f;
   // Source declarations give ordered condition tables; this keeps the model's
   // existing one-target/rate adapter, not an unmeasured native brain parity claim.
   foreach(var q in cargoSourceV21.decisions){if(!CargoDecisionDistanceV21(q.far,native,q.nativeDistance)||e.hp*100>=e.maxHp*q.hpLessPercent)continue;float roll=CargoConditionRollOverrideV21==null?NpcRoll():CargoConditionRollOverrideV21();if(!Finite(roll)||roll<0||roll>=100)throw new InvalidOperationException("Cargo condition RNG must be0<=roll<100");CargoEventV21(e,"source-condition-roll",q.sourceCondition,elapsed,e.x,e.y,0,null,-1,0,0,0,roll);if(roll>=q.rate)continue;if(TryEnterNpcState(e,npcStates[e.sourceNPC+"/"+q.state])){e.npcDecisionElapsed=0;return true;}}
   return false;
  }
  bool CargoSourceFlightStateV21(Enemy e,NpcStateDefinition s){return CargoWallyBoundV21(e)&&s!=null&&(s.id=="WALLY_9TH_ARMOR_JUMP_UP"||s.id=="WALLY_9TH_ARMOR_JUMP_UP_DIR"||s.id=="WALLY_9TH_ARMOR_JUMP_DOWN"||s.id=="WALLY_9TH_ARMOR_JUMP_DOWN_DIR");}
  bool TickCargoSourceFlightV21(Enemy e,float dt){
   var s=NpcState(e);if(!CargoSourceFlightStateV21(e,s)||!e.active||e.dormant||e.hp<=0||e.stun>0||e.knockdownOnLanding||e.state==CombatState.Knockdown||e.state==CombatState.GettingUp)return false;
   if(e.hitPause>0){float frozen=Math.Min(e.hitPause,dt);e.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
   bool up=s.id=="WALLY_9TH_ARMOR_JUMP_UP"||s.id=="WALLY_9TH_ARMOR_JUMP_UP_DIR",directional=s.id.EndsWith("_DIR",StringComparison.Ordinal);float from=e.npcTime,to=from+dt,step=dt;
   if(up)step=Math.Min(dt,Math.Max(0,3-from));
   if(directional)MoveAlongGround(e,e.x+e.facing*300*.28f*step);
   float before=e.y;
   if(up){e.vy=100*.28f;e.y+=e.vy*step;e.groundLine=-1;}
   else{float v=e.vy,g=4000*.28f,terminal=2000*.28f;float accelerating=Math.Min(step,Math.Max(0,(terminal+v)/g));e.y+=v*accelerating-.5f*g*accelerating*accelerating-terminal*(step-accelerating);e.vy=Math.Max(-terminal,v-g*step);var floor=NativeTerrain?SurfaceBelow(e.x,before+.05f):null;if(floor!=null&&e.vy<=0&&e.y<=floor.Y(e.x)){e.y=floor.Y(e.x);e.vy=0;e.groundLine=floor.id;CargoEventV21(e,"source-flight-real-landing",s.id,elapsed-dt+step,e.x,e.y);}else if(!NativeTerrain&&e.vy<=0&&e.y<=GroundAt(e.x)){e.y=GroundAt(e.x);e.vy=0;}}
   e.clock+=step;e.npcTime=from+step;AdvanceNpcStateSounds(e,s,from,e.npcTime,elapsed-dt);AdvanceCargoSourceCallbacksV21(e,s,from,e.npcTime,elapsed-dt);
   if(up&&e.npcTime+1e-6f>=3){string next=directional?"WALLY_9TH_ARMOR_JUMP_DOWN_DIR":"WALLY_9TH_ARMOR_JUMP_DOWN";if(TryEnterNpcState(e,npcStates[e.sourceNPC+"/"+next]))CargoEventV21(e,"source-flight-timer3",next,elapsed-dt+step,e.x,e.y);}
   else if(!up&&Grounded(e)){if(TryEnterNpcState(e,npcStates[e.sourceNPC+"/WALLY_9TH_ARMOR_JUMP_LANDING"]))CargoEventV21(e,"source-flight-land-state","SCT_FOOT_ON_LINE_TRUE",elapsed-dt+step,e.x,e.y);}
   return true;
  }
 }
}
