using System;
using System.Collections.Generic;
using System.Reflection;

// Optional, exact-identity Dragon Nest adapters. No document means no behavior.
namespace ElswordPixel {
 [Serializable] public sealed class DragonGeyserV20 {
  public string npc,waitState,attackState,clock,bone,effect,callback,instructionsSha256;
  public float motionDuration,waitInterval,near,attackDuration,emitAt,sourceOffsetY;
  public float[] boneOffset;
 }
 [Serializable] public sealed class DragonBatV20 {
  public string npc,readyState,fireState,explodeState,followAdapter,onFireVictimGone,effect,callback,instructionsSha256;
  public float fireMotionDuration,fireStateDuration,explodeDuration,onExplodeExitHp;public int followHitterArgument;public float[] beepTimes;
 }
 [Serializable] public sealed class DragonShamanV20 {
  public string npc,attackState,clock,bone,effect,healState,callback,instructionsSha256;
  public float emitAt,healAt,healRange,healFraction,resetOwnerMp;public bool excludeSelf;public float[] boneOffset;
 }
 [Serializable] public sealed class DragonKingV20 {
  public string npc,summonNpc,readyState,summonState,summonPositionAdapter,callback,instructionsSha256;
  public float hpLessPercentExclusive,cooldown,emitAt,sourceRandomRange,range,attackAFarExclusive,candidateSummonHp;
  public int liveCountEquals,spawnCount,sourceDirectionPolarity;public bool spawnCountsForClear;
  public float attackBAt;public float[] attackBOffsets,attackBTargetOffsets,specialTimes,specialNativeDistances;
 }
 [Serializable] public sealed class DragonTimerV20 {public string npc,state,nextState;public float threshold;}
 [Serializable] public sealed class DragonCallbackSoundV20 {public string sourceName,sound;}
 [Serializable] public sealed class DragonIntroV20 {
  public string roomId,kingNpc,directorNpc,releaseCallback;public float directorDuration,kingStartDuration;
  public float[] captionTimes,captionDurations;public string[] captionIds;public AlteraAirshipSoundV18[] sourceSoundEvents;
 }
 [Serializable] public sealed class NpcDragonNestDocumentV20 {
  public string schema,sourceRulesSha256,teamAdapter;public bool runtimeAdmitted,nativeExecuted;public string[] npcUnitIds;
  public DragonCallbackSoundV20[] callbackSounds;
  public DragonGeyserV20 geyser;public DragonBatV20 bat;public DragonShamanV20 shamanFire;public DragonKingV20 king;public DragonTimerV20 saurusAnimationTimer;public DragonIntroV20 intro;
 }
 [Serializable] public sealed class DragonSourceEventV20 {
  public string type,npc,target,source;public float time,x,y,z,depth,amount;public int sequence,room,worldSerial,actorSerial,targetActorSerial;
  [NonSerialized] public Enemy owner,targetActor;
 }
 public sealed partial class GameModel {
  NpcDragonNestDocumentV20 dragonNestV20;
  sealed class DragonCursorV20 {internal string state;internal int emitted;}
  sealed class DragonGeyserActorV20 {internal float age,decision;internal bool attack;}
  readonly Dictionary<Enemy,DragonCursorV20> dragonCursorsV20=new Dictionary<Enemy,DragonCursorV20>();
  readonly Dictionary<Enemy,DragonGeyserActorV20> dragonGeysersV20=new Dictionary<Enemy,DragonGeyserActorV20>();
  readonly Dictionary<NpcEffectInstance,float[]> dragonFixedTargetsV20=new Dictionary<NpcEffectInstance,float[]>();
  readonly Dictionary<Enemy,float> dragonKnownMpV20=new Dictionary<Enemy,float>();
  Enemy dragonIntroKingV20,dragonIntroSoundV20;float dragonIntroBeganV20,dragonIntroPreviousV20;bool dragonIntroReleasedV20;long dragonIntroSoundEntryV20;
  int dragonEventSequenceV20;
  public readonly List<DragonSourceEventV20> dragonSourceEventsV20=new List<DragonSourceEventV20>();
  static bool EmptyDragonDtoV20(object value){
   if(value==null)return true;var t=value.GetType();if(t==typeof(string))return string.IsNullOrEmpty((string)value);
   if(t==typeof(bool))return !(bool)value;if(t.IsPrimitive)return Convert.ToDouble(value)==0;
   var a=value as Array;if(a!=null)return a.Length==0;
   foreach(var f in t.GetFields(BindingFlags.Public|BindingFlags.Instance))if(!EmptyDragonDtoV20(f.GetValue(value)))return false;return true;
  }
  static bool DragonNearV20(float a,float b){return Finite(a)&&Math.Abs(a-b)<.00001f;}
  void DragonStateV20(string npc,string state){if(!npcStates.ContainsKey(npc+"/"+state))throw new ArgumentException("Missing exact Dragon source state "+npc+"/"+state);}
  static void DragonVectorV20(float[] v){if(v==null||v.Length!=3||!Finite(v[0])||!Finite(v[1])||!Finite(v[2]))throw new ArgumentException("Unbound original Dragon bone position");}
  public void SetNpcDragonNestV20(NpcDragonNestDocumentV20 doc){
   if(EmptyDragonDtoV20(doc))doc=null;
   if(doc==null){dragonNestV20=null;ClearDragonNestRoomV20();return;}
   if(doc.schema!="besma-dragon-nest-source-callbacks-v20"||!SourceSupportHashV17(doc.sourceRulesSha256)||doc.runtimeAdmitted||doc.nativeExecuted||doc.teamAdapter!="all-current-enemies-hostile-team-v1"||doc.geyser==null||doc.bat==null||doc.shamanFire==null||doc.king==null||doc.saurusAnimationTimer==null||doc.intro==null)throw new ArgumentException("Incomplete scoped Dragon Nest source contract");
   string[] units={"NUI_BAT_B_FIRE","NUI_FIRE_KOMODO","NUI_LIZARDMAN_SHAMAN_FIRE","NUI_LIZARDMAN_SHAMAN_KING","NUI_LIZARDMAN_SPEAR_RED","NUI_SAURUS_HIGH","NUI_SCORPION_FIRE","NUI_LIZARDMAN_HIGH_DARK"};
   if(doc.npcUnitIds==null||doc.npcUnitIds.Length!=units.Length||!new HashSet<string>(doc.npcUnitIds).SetEquals(units))throw new ArgumentException("Dragon original IS_NPC membership changed");foreach(string npc in units)if(!npcDefinitions.ContainsKey(npc))throw new ArgumentException("Missing exact Dragon identity");
   var g=doc.geyser;DragonStateV20(g.npc,g.waitState);DragonStateV20(g.npc,g.attackState);DragonVectorV20(g.boneOffset);
   if(g.npc!="NUI_STEAM_GEYSER"||g.waitState!="STEAM_GEYSER_WAIT"||g.attackState!="STEAM_GEYSER_ATTACK"||!DragonNearV20(g.motionDuration,1f/15)||g.waitInterval!=6.5f||g.near!=224||g.attackDuration!=5||!DragonNearV20(g.emitAt,3.3f)||g.clock!="state-seconds"||g.bone!="Dummy01"||g.sourceOffsetY!=230||g.effect!="DamageEffect_STEAM_GEYSER_ATTACK"||g.callback!="STEAM_GEYSER_ATTACK_FRAME_MOVE"||g.instructionsSha256!="0d1e8fbd06ab056bd6943071ebcf8f1607c7ba7ead9cc37cd8734b9d08af113b")throw new ArgumentException("Changed original Geyser contract");
   var b=doc.bat;DragonStateV20(b.npc,b.readyState);DragonStateV20(b.npc,b.fireState);DragonStateV20(b.npc,b.explodeState);
   if(b.npc!="NUI_BAT_B_FIRE"||b.readyState!="BAT_B_FIRE_ATTACK_A_READY"||b.fireState!="BAT_B_FIRE_ATTACK_A"||b.explodeState!="BAT_B_FIRE_ATTACK_A_EXPLODE"||b.fireMotionDuration!=4||b.fireStateDuration!=5||!DragonNearV20(b.explodeDuration,.1f)||b.followHitterArgument!=5||b.followAdapter!="actual-hit-player-feet-v20"||b.onFireVictimGone!="BAT_B_FIRE_WAIT"||b.onExplodeExitHp!=0||b.effect!="DamageEffect_BAT_B_FIRE_ATTACK_A_EXPLODE"||b.beepTimes==null||b.beepTimes.Length!=4)throw new ArgumentException("Changed original Bat hit/follow/explosion contract");
   var s=doc.shamanFire;DragonStateV20(s.npc,s.attackState);DragonStateV20(s.npc,s.healState);DragonVectorV20(s.boneOffset);
   if(b.callback!="BAT_B_FIRE_ATTACK_A_EXPLODE_STATE_START"||b.instructionsSha256!="c97bf35d7c3d4083f0e138e8b480f13b02d9143b0151e60758e67c437157e9ef"||s.callback!="LIZARDMAN_SHAMAN_FIRE_SP_HEAL_FRAME_MOVE"||s.instructionsSha256!="beedb69a9aef9b65c0ba488146a4b652f2d22635e15d8f35411b38d1a18f65d6")throw new ArgumentException("Changed original Dragon callback bytecode binding");
   if(!DragonNearV20(g.boneOffset[0],0)||!DragonNearV20(g.boneOffset[1],2.5199997f)||!DragonNearV20(g.boneOffset[2],-.00000028f)||!DragonNearV20(s.boneOffset[0],26.916922f)||!DragonNearV20(s.boneOffset[1],46.0255f)||!DragonNearV20(s.boneOffset[2],5.83862f))throw new ArgumentException("Changed sampled original Dragon emitter bone");
   if(s.npc!="NUI_LIZARDMAN_SHAMAN_FIRE"||s.attackState!="LIZARDMAN_SHAMAN_FIRE_ATTACK_A"||s.healState!="LIZARDMAN_SHAMAN_FIRE_ATTACK_B"||s.clock!="animation-time"||!DragonNearV20(s.emitAt,.254f)||!DragonNearV20(s.healAt,.119f)||s.healRange!=224||s.healFraction!=.5f||s.excludeSelf||s.resetOwnerMp!=0||s.bone!="Bip01_L_Hand"||s.effect!="DamageEffect_LIZARDMAN_SHAMAN_FIRE_ATTACK_A")throw new ArgumentException("Changed exact Fire Shaman source contract");
   var k=doc.king;DragonStateV20(k.npc,k.readyState);DragonStateV20(k.npc,k.summonState);
   if(k.callback!="CF_LIZARDMAN_SHAMAN_KING_ATTACK_C_READY"||k.instructionsSha256!="155e3dfb0d2cdd249bb523a5d92b9ef07a9b90ec6f1f8b3bbfd46c788530bc05")throw new ArgumentException("Changed original King population predicate bytecode binding");
   if(k.npc!="NUI_LIZARDMAN_SHAMAN_KING"||k.summonNpc!="NUI_LIZARDMAN_HIGH_DARK"||k.readyState!="LIZARDMAN_SHAMAN_KING_ATTACK_C_READY"||k.summonState!="LIZARDMAN_SHAMAN_KING_ATTACK_C"||k.hpLessPercentExclusive!=40||k.liveCountEquals!=0||k.cooldown!=16||!DragonNearV20(k.emitAt,.8f)||k.range!=140||k.sourceRandomRange!=500||k.spawnCount!=1||!k.spawnCountsForClear||k.attackAFarExclusive!=168||k.summonPositionAdapter!="existing-source-ground-direction-v1"||k.sourceDirectionPolarity!=1||k.candidateSummonHp!=600)throw new ArgumentException("Changed original King summon identity/conditions or explicit candidate balance");
   if(k.attackBAt!=.1f||k.attackBOffsets==null||k.attackBOffsets.Length!=2||k.attackBOffsets[0]!=100||k.attackBOffsets[1]!=-100||k.attackBTargetOffsets==null||k.attackBTargetOffsets.Length!=2||k.attackBTargetOffsets[0]!=500||k.attackBTargetOffsets[1]!=-500||k.specialTimes==null||k.specialTimes.Length!=4||k.specialNativeDistances==null||k.specialNativeDistances.Length!=4)throw new ArgumentException("Missing original King effect waves");
   for(int i=0;i<4;i++)if(!DragonNearV20(k.specialTimes[i],.3f+.25f*i)||k.specialNativeDistances[i]!=170*(i+1)||!DragonNearV20(b.beepTimes[i],i==0?.01f:i))throw new ArgumentException("Changed original Dragon callback clock");
   var timer=doc.saurusAnimationTimer;if(timer.npc!="NUI_SAURUS_HIGH"||timer.state!="SAURUS_HIGH_ATTACK_A_READY"||timer.nextState!="SAURUS_HIGH_ATTACK_A"||timer.threshold!=1.9f)throw new ArgumentException("Changed original Saurus1.9 animation timer");
   var intro=doc.intro;if(intro.roomId!="Dungeon_Besma_Dragon_Nest_Normal-2"||intro.kingNpc!=k.npc||intro.directorNpc!="NUI_EVENT_NPC_LIZARDMAN_SHAMAN_KING"||!DragonNearV20(intro.directorDuration,10.23333333f)||intro.kingStartDuration!=3||intro.releaseCallback!="EVENT_NPC_LIZARDMAN_SHAMAN_KING_DYING_START_START"||intro.captionTimes==null||intro.captionTimes.Length!=3||intro.captionIds==null||intro.captionIds.Length!=3||intro.captionDurations==null||intro.captionDurations.Length!=3||intro.sourceSoundEvents==null||intro.sourceSoundEvents.Length!=5)throw new ArgumentException("Changed original Dragon director release");
   float[] captionTimes={2.5f,6.5f,8.5f},captionDurations={3,2,.7f};string[] captionIds={"STR_ID_31194","STR_ID_31195","STR_ID_31196"};for(int i=0;i<3;i++)if(intro.captionTimes[i]!=captionTimes[i]||intro.captionDurations[i]!=captionDurations[i]||intro.captionIds[i]!=captionIds[i])throw new ArgumentException("Changed source director caption clock");
   float[] soundTimes={.329f,1.126f,6.609f,6.788f,7.12f};string[] soundNames={"fire_ready.ogg","LizardSaurus_AttackB_Ready.ogg","Swing_Big06.ogg","LizardOlder_MagicAttackA.ogg","Besma_Rock_Ready2.ogg"};for(int i=0;i<5;i++){var cue=intro.sourceSoundEvents[i];if(cue==null||cue.time!=soundTimes[i]||cue.sourceName!=soundNames[i]||cue.sourceField!="SOUND_PLAY"+i||!npcSourceSoundAssetsV17.Contains(cue.sound??""))throw new ArgumentException("Original Dragon director sound not loaded");}
   string[] callbackNames={"Bat_Beep.ogg","fire.ogg","fire_ready.ogg","LizardHigh_HighJumpLand.ogg"};if(doc.callbackSounds==null||doc.callbackSounds.Length!=4)throw new ArgumentException("Missing original callback sound bindings");for(int i=0;i<4;i++){var cue=doc.callbackSounds[i];if(cue==null||cue.sourceName!=callbackNames[i]||!npcSourceSoundAssetsV17.Contains(cue.sound??""))throw new ArgumentException("Unloaded callback sound");}
   foreach(string id in new[]{g.effect,b.effect,s.effect,"DamageEffect_LIZARDMAN_SHAMAN_FIRE_ATTACK_A_EXPLOSION","DamageEffect_LIZARDMAN_SHAMAN_KING_ATTACK_B","DamageEffect_LIZARDMAN_SHAMAN_KING_SPECIAL_ATTACK_A","WALLY_8TH_JUMP_ATTACK_LANDING"})if(!npcEffectDefinitions.ContainsKey(id))throw new ArgumentException("Missing Dragon dynamic effect closure "+id);
   dragonNestV20=doc;ClearDragonNestRoomV20();
  }
  void DragonEventV20(string type,Enemy owner,float at,string source="",float amount=0,Enemy target=null,float x=float.NaN,float y=float.NaN,float z=0){if(dragonSourceEventsV20.Count>=4096)dragonSourceEventsV20.RemoveAt(0);var point=target??owner;dragonSourceEventsV20.Add(new DragonSourceEventV20{type=type,npc=owner==null?"":owner.sourceNPC,target=target==null?"":target.sourceNPC,source=source,time=at,amount=amount,x=float.IsNaN(x)?point==null?0:point.x:x,y=float.IsNaN(y)?point==null?0:point.y:y,z=z,depth=point==null?0:point.nativeDepth,sequence=++dragonEventSequenceV20,room=room,worldSerial=npcSoundWorldSerial,actorSerial=NpcSoundActorId(owner),targetActorSerial=NpcSoundActorId(target),owner=owner,targetActor=target});}
  void ClearDragonNestRoomV20(){dragonCursorsV20.Clear();dragonGeysersV20.Clear();dragonFixedTargetsV20.Clear();dragonKnownMpV20.Clear();dragonSourceEventsV20.Clear();dragonIntroKingV20=dragonIntroSoundV20=null;dragonIntroPreviousV20=0;dragonIntroReleasedV20=false;}
  void ObserveDragonMissingBranchV20(Enemy owner){
   if(dragonNestV20==null||owner.sourceNPC!="NUI_LIZARDMAN_HIGH_DARK"||owner.npcState!="LIZARDMAN_HIGH_DARK_ATTACK_B_READY"||Grounded(owner)||owner.vy>=0)return;
   var c=DragonCursorV20For(owner,owner.npcState);if((c.emitted&256)!=0)return;c.emitted|=256;
   DragonEventV20("source-defect-missing-air-target",owner,elapsed,"LIZARDMAN_HIGH_DARK_ATTACK_A_JUMP_DOWN");
   // No invented jump state. Existing body physics continues while this
   // unbound source branch is explicitly reported to the candidate consumer.
  }
  void OnDragonDamageFrameV20(Enemy owner){
   if(dragonNestV20==null||owner==null||owner.sourceNPC!=dragonNestV20.king.npc||owner.hp<=0||owner.maxHp<=0||owner.hp>=owner.maxHp*.7f)return;
   if(owner.npcState!="LIZARDMAN_SHAMAN_KING_WAIT"&&owner.npcState!="LIZARDMAN_SHAMAN_KING_WALK")return;
   foreach(var other in enemies)if(other.hp>0&&other.sourceNPC==dragonNestV20.king.summonNpc)other.alerted=true;
   DragonEventV20("source-call-other-npc",owner,elapsed,dragonNestV20.king.summonNpc);
  }
  static bool IsDragonGeyserZeroBodyV20(NpcDefinition p){return p!=null&&p.id=="NUI_STEAM_GEYSER"&&p.environmentKind=="source-dragon-geyser"&&p.bodySpheres.Length==0;}
  static bool ValidateDragonEnvironmentV20(NpcDefinition p){if(p.environmentKind!="source-dragon-geyser")return false;if(!IsDragonGeyserZeroBodyV20(p)||p.bodyHeight!=0||p.environment==null||p.environment.initialState!="STEAM_GEYSER_WAIT"||p.environment.activeState!="STEAM_GEYSER_ATTACK")throw new ArgumentException("Geyser must remain an exact no-body environment");return true;}
  DragonCursorV20 DragonCursorV20For(Enemy e,string state){DragonCursorV20 c;if(!dragonCursorsV20.TryGetValue(e,out c)||c.state!=state){c=new DragonCursorV20{state=state};dragonCursorsV20[e]=c;}return c;}
  float DragonLandYV20(Enemy e){var arg=new NpcEffectInstance{owner=e};CaptureNpcSourceLandArgumentV18(arg,null,new NpcEffectEmission{sourceLandPositionYArgument=true});return arg.sourceLandY;}
  NpcEffectInstance EmitDragonEffectV20(Enemy owner,string id,float x,float y,float z,float at,bool fixedTarget=false){
   var emission=new NpcEffectEmission{effect=id,sourceLandPositionYArgument=true,lockOnMode=fixedTarget?2:0};float bx,by,bz;NpcAttachment(owner,emission,at,out bx,out by,out bz);
   emission.offsetX=(x-bx)/owner.facing/owner.visualScale;emission.offsetY=(y-by)/owner.visualScale;emission.offsetZ=(z-bz)/owner.visualScale;
   var effect=SpawnNpcEffect(owner,npcEffectDefinitions[id],emission,at);effect.ownerScale=1;DragonEventV20("source-effect",owner,at,id,1,null,x,y,z);return effect;
  }
  void DragonSoundV20(Enemy owner,string source,float at,string state){var asset=Array.Find(dragonNestV20.callbackSounds,s=>s.sourceName==source);if(asset==null)throw new InvalidOperationException("Missing exact Dragon source sound "+source);var cue=new NpcTimedSound{id="dragon-callback/"+state+"/"+source,sound=asset.sound,sourceName=source,sourceField="original-Lua-callback",candidateClock="source-callback",candidateTime=0,sourceTime=0};QueueNpcSound(cue,owner,++npcSoundEntrySerial,0,at,state);DragonEventV20("source-sound",owner,at,source);}
  public bool TryGetDragonKnownMpV20(Enemy owner,out float value){return dragonKnownMpV20.TryGetValue(owner,out value);}
  void OnDragonStateExitV20(Enemy owner,NpcStateDefinition state){
   if(dragonNestV20==null||state==null)return;
   if(owner.sourceNPC==dragonNestV20.bat.npc){if(state.id==dragonNestV20.bat.fireState)DragonEventV20("source-countdown-destroy",owner,elapsed,"major-slot0");if(state.id==dragonNestV20.bat.explodeState&&owner.hp>0){owner.hp=0;owner.state=CombatState.Dead;DragonEventV20("source-set-owner-hp",owner,elapsed,"SetNowHP_LUA",0);}}
   if(owner.sourceNPC=="NUI_LIZARDMAN_HIGH_DARK"&&state.id=="LIZARDMAN_HIGH_DARK_ATTACK_B"){
    float y=DragonLandYV20(owner);var fx=EmitDragonEffectV20(owner,"WALLY_8TH_JUMP_ATTACK_LANDING",owner.x,y,0,elapsed);fx.hasSourceLandY=true;fx.sourceLandY=y+2*.28f;
    DragonSoundV20(owner,"LizardHigh_HighJumpLand.ogg",elapsed,state.id);DragonEventV20("source-landing-end",owner,elapsed,"unconditional-original-state-end");
   }
   dragonCursorsV20.Remove(owner);
  }
  void OnDragonStateEnteredV20(Enemy owner,NpcStateDefinition state){if(dragonNestV20==null)return;if(owner.sourceNPC==dragonNestV20.bat.npc&&state.id==dragonNestV20.bat.explodeState)EmitDragonEffectV20(owner,dragonNestV20.bat.effect,owner.x,owner.y,0,elapsed);}
  void OnDragonSummonedV20(Enemy owner){if(dragonNestV20!=null&&owner.sourceNPC==dragonNestV20.king.summonNpc){EnterNpcState(owner,npcStates[owner.sourceNPC+"/LIZARDMAN_HIGH_DARK_START"]);owner.alerted=true;DragonEventV20("source-summoned-start",owner,elapsed,"LIZARDMAN_HIGH_DARK_START");}}
  bool DragonConditionAllowedV20(Enemy owner,NpcCondition c,float distance){
   if(dragonNestV20==null||owner.sourceNPC!=dragonNestV20.king.npc)return true;var k=dragonNestV20.king;
   if(c.nextState=="LIZARDMAN_SHAMAN_KING_ATTACK_A_READY")return distance>k.attackAFarExclusive;
   if(c.nextState!=k.readyState)return true;if(owner.hp<=0||owner.maxHp<=0||owner.hp*100>=owner.maxHp*k.hpLessPercentExclusive)return false;
   foreach(var e in enemies)if(e.hp>0&&e.sourceNPC==k.summonNpc)return false;foreach(var r in npcSummonRequests)if(r.room==room&&r.npc==k.summonNpc)return false;return true;
  }
  bool TryDragonFixedTargetV20(NpcEffectInstance effect,out float x,out float y){float[] p;if(!effect.reflected&&dragonFixedTargetsV20.TryGetValue(effect,out p)){x=p[0];y=p[1];return true;}x=y=0;return false;}
  const string KingFireball="DamageEffect_LIZARDMAN_SHAMAN_KING_ATTACK_A";
  const string KingBigFireball=KingFireball+"_BIG";
  const string FireShamanExplosion="DamageEffect_LIZARDMAN_SHAMAN_FIRE_ATTACK_A_EXPLOSION";
  public static string DragonImpactVisualId(string id){
   return id==KingFireball+"_EXPLOSION"||id==KingBigFireball+"_EXPLOSION"?FireShamanExplosion:id;
  }
  static void BindDragonKingImpactEffects(Dictionary<string,NpcEffectDefinition> defs){
   NpcEffectDefinition shape;
   if(!defs.TryGetValue(FireShamanExplosion,out shape))return;
   foreach(string id in new[]{KingFireball,KingBigFireball}){
    NpcEffectDefinition carrier;if(!defs.TryGetValue(id,out carrier))continue;
    string burst=id+"_EXPLOSION";
    if(!defs.ContainsKey(burst)){
     // The original tables share the Fire Shaman explosion template and reaction.
     var child=shape.CopyDragonEffect();child.id=burst;child.magic=id==KingFireball?.25f:.5f;
     defs.Add(burst,child);
    }
    if(string.IsNullOrEmpty(carrier.hitChild)){
     var copy=carrier.CopyDragonEffect();copy.hitChild=burst;defs[id]=copy;
    }
   }
  }
  void EmitDragonCarrierImpact(NpcEffectInstance carrier,float at){
   var child=SpawnNpcEffect(carrier.owner,npcEffectDefinitions[carrier.definition.hitChild],null,at,carrier);
   npcSourceEffectRotationsV18[child]=NpcSourceParentAngleV18(carrier);child.dirX=carrier.dirX;child.dirY=carrier.dirY;
   TraceNpcEffect("hit-child",child,0,at);DragonEventV20("source-zero-hit-child",carrier.owner,at,child.id,0,null,child.x,child.y,child.z);
  }
  bool TryContactDragonFireShamanV20(NpcEffectInstance effect,EffectPose pose,float at){
   if(dragonNestV20==null)return false;
   bool king=effect.id==KingFireball||effect.id==KingBigFireball;
   if(!king&&effect.id!=dragonNestV20.shamanFire.effect)return false;
   var d=effect.definition;if(d.physical!=0||d.magic!=0||d.hitChild!=(king?effect.id+"_EXPLOSION":FireShamanExplosion)||d.reflectable!=king)throw new InvalidOperationException("Changed original zero-damage Fire Shaman payload");
   if(effect.ended||effect.age<d.damageFrom||d.damageUntil>0&&effect.age>d.damageUntil||at<effect.nextPlayerHit)return true;
   if(effect.reflected){
    if(!king)return true;
    for(int i=0;i<d.baseSpheres.Length;i+=4){
     float x,y,z,r;NpcEffectSphere(effect,pose,i,out x,out y,out z,out r);if(r<=0)continue;
     foreach(var target in enemies){
      if(target.hp<=0||!target.active||target.dormant||target.passive||target.invincible>0||EnemyHasNpcInvincibility(target))continue;
      var body=EnemyBounds(target);float dx=x-Clamp(x,body.left,body.right),dy=y-Clamp(y,body.bottom,body.top);
      float dz=z-Clamp(z,target.nativeDepth-9.8f,target.nativeDepth+9.8f);if(dx*dx+dy*dy+dz*dz>r*r)continue;
      effect.nextPlayerHit=float.PositiveInfinity;EmitDragonCarrierImpact(effect,at);return true;
     }
    }
    return true;
   }
   if(player.hp<=0||player.invincible>0||AwakeningCasting)return true;
   for(int i=0;i<d.baseSpheres.Length;i+=4){float x,y,z,r;NpcEffectSphere(effect,pose,i,out x,out y,out z,out r);if(r<=0)continue;float px=Clamp(x,player.x-9.8f,player.x+9.8f),py=Clamp(y,player.y,player.y+42),pz=Clamp(z,-9.8f,9.8f);if((px-x)*(px-x)+(py-y)*(py-y)+(pz-z)*(pz-z)>r*r)continue;
    // Source RT_DUMMY_DAMAGE/MAGIC0 still produces a successful contact. It
    // never invents one point of damage merely to make the HIT callback run.
    effect.nextPlayerHit=float.PositiveInfinity;RecordNpcPresentationHit(effect,at,px,py,pz);TraceNpcEffect("source-zero-damage-hit",effect,0,at);
    // This exact table declares DEPT_POS and APPLY_ROTATE=TRUE. Preserve the
    // projectile position and angle; generic IMPACT_POINT placement differs.
    EmitDragonCarrierImpact(effect,at);break;
   }return true;
  }
  void AdvanceDragonCallbacksV20(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(dragonNestV20==null||owner.hp<=0)return;var cursor=DragonCursorV20For(owner,state.id);var doc=dragonNestV20;
   if(owner.sourceNPC==doc.bat.npc&&state.id==doc.bat.fireState){for(int i=0;i<4;i++){float at=doc.bat.beepTimes[i]/state.sourceAnimSpeed;if((cursor.emitted&(1<<i))==0&&from<at&&at<=to+1e-6f){cursor.emitted|=1<<i;DragonSoundV20(owner,"Bat_Beep.ogg",packetStart+at-from,state.id);DragonEventV20("source-countdown",owner,packetStart+at-from,"PC_"+(4-i),i);}}}
   if(owner.sourceNPC==doc.shamanFire.npc){var s=doc.shamanFire;
    if(state.id==s.attackState){float at=s.emitAt/state.sourceAnimSpeed;if(cursor.emitted==0&&from<at&&at<=to+1e-6f){cursor.emitted=1;EmitDragonEffectV20(owner,s.effect,owner.x+owner.facing*s.boneOffset[0],owner.y+s.boneOffset[1],s.boneOffset[2],packetStart+at-from);}}
    if(state.id==s.healState){float at=s.healAt/state.sourceAnimSpeed;if(cursor.emitted==0&&from<at&&at<=to+1e-6f){cursor.emitted=1;float absolute=packetStart+at-from;
     foreach(var target in enemies){if(!target.active||target.dormant||Array.IndexOf(doc.npcUnitIds,target.sourceNPC)<0||target.hp<=0||target.hp>=target.maxHp)continue;float dx=target.x-owner.x,dy=target.y-owner.y,dz=target.nativeDepth-owner.nativeDepth;if(dx*dx+dy*dy+dz*dz>=s.healRange*s.healRange)continue;
      float amount=Math.Min(target.maxHp-target.hp,target.maxHp*s.healFraction);DragonEventV20("source-heal-target",owner,absolute,"Heal_Effect01/LobbyLevelUp01",amount,target);RecoverEnemyHealth(target,amount,s.healFraction*100);}
     dragonKnownMpV20[owner]=0;DragonEventV20("source-set-owner-mp",owner,absolute,"SetNowMP",0);
    }}
   }
   if(owner.sourceNPC==doc.king.npc){var k=doc.king;
    if(state.id==k.summonState){float at=k.emitAt/state.sourceAnimSpeed;if(cursor.emitted==0&&from<at&&at<=to+1e-6f){float x,y;int line;if(!SummonGroundPoint(owner,new NpcSummonSpawn{npc=k.summonNpc,range=k.range,direction=k.sourceDirectionPolarity},out x,out y,out line))throw new InvalidOperationException("King original directional random-position request has no source support");cursor.emitted=1;float absolute=packetStart+at-from;
     npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=new EnemySpawn{hp=k.candidateSummonHp,visualScale=1,nativeDepth=owner.nativeDepth},npc=k.summonNpc,room=room,encounter=owner.encounterIndex,line=line,x=x,y=y,due=absolute+1.9f,alreadyEmitted=true});DragonEventV20("source-summon-request",owner,absolute,k.summonNpc,1,null,x,y);
    }}
    if(state.id=="LIZARDMAN_SHAMAN_KING_ATTACK_B"){float at=k.attackBAt/state.sourceAnimSpeed;if(cursor.emitted==0&&from<at&&at<=to+1e-6f){cursor.emitted=1;for(int i=0;i<2;i++){var effect=EmitDragonEffectV20(owner,"DamageEffect_LIZARDMAN_SHAMAN_KING_ATTACK_B",owner.x+owner.facing*k.attackBOffsets[i]*.28f,owner.y,0,packetStart+at-from,true);dragonFixedTargetsV20[effect]=new[]{owner.x+owner.facing*k.attackBTargetOffsets[i]*.28f,owner.y};}}}
    if(state.id=="LIZARDMAN_SHAMAN_KING_SPECIAL_ATTACK_A"){for(int i=0;i<4;i++){float at=k.specialTimes[i]/state.sourceAnimSpeed;if((cursor.emitted&(1<<i))==0&&from<at&&at<=to+1e-6f){cursor.emitted|=1<<i;float absolute=packetStart+at-from;foreach(int side in new[]{-1,1})EmitDragonEffectV20(owner,"DamageEffect_LIZARDMAN_SHAMAN_KING_SPECIAL_ATTACK_A",owner.x+owner.facing*side*k.specialNativeDistances[i]*.28f,owner.y,0,absolute);dragonKnownMpV20[owner]=0;DragonEventV20("source-set-owner-mp",owner,absolute,"SetNowMP",0);DragonSoundV20(owner,"fire.ogg",absolute,state.id);}}}
   }
  }
  bool AdvanceDragonTimerV20(Enemy owner,NpcStateDefinition state,float from,float to){if(dragonNestV20==null)return false;var t=dragonNestV20.saurusAnimationTimer;float at=t.threshold/state.sourceAnimSpeed;return owner.sourceNPC==t.npc&&state.id==t.state&&from<at&&to+1e-6f>=at&&TryEnterNpcState(owner,npcStates[t.npc+"/"+t.nextState]);}
  bool TickDragonGeyserV20(Enemy owner,NpcDefinition p,float dt){
   if(p.environmentKind!="source-dragon-geyser")return false;if(dragonNestV20==null)throw new InvalidOperationException("Source Geyser needs exact callback document");if(!owner.active||owner.dormant||owner.hp<=0)return true;var g=dragonNestV20.geyser;DragonGeyserActorV20 a;
   if(!dragonGeysersV20.TryGetValue(owner,out a)){a=new DragonGeyserActorV20{decision=g.waitInterval};dragonGeysersV20.Add(owner,a);EnterNpcState(owner,npcStates[g.npc+"/"+g.waitState]);}
   float left=dt,absolute=elapsed-dt;while(left>1e-7f){float end=a.attack?g.attackDuration:a.decision,part=Math.Min(left,Math.Max(0,end-a.age)),before=a.age;a.age+=part;left-=part;var state=NpcState(owner);owner.clock+=part;
    if(a.attack){var c=DragonCursorV20For(owner,state.id);for(int i=0;i<2;i++){float at=i==0?.01f:1.01f;int bit=2<<i;if((c.emitted&bit)==0&&before<at&&at<=a.age+1e-6f){c.emitted|=bit;DragonSoundV20(owner,"fire_ready.ogg",absolute+at-before,state.id);}}
     if((c.emitted&1)==0&&before<g.emitAt&&g.emitAt<=a.age+1e-6f){c.emitted|=1;EmitDragonEffectV20(owner,g.effect,owner.x+owner.facing*g.boneOffset[0],owner.y+g.boneOffset[1]+g.sourceOffsetY*.28f,g.boneOffset[2],absolute+g.emitAt-before);}}
    owner.npcTime=Math.Min(a.age,state.duration);absolute+=part;if(a.age+1e-6f<end)break;
    if(a.attack){a.attack=false;a.age=0;a.decision=g.waitInterval;EnterNpcState(owner,npcStates[g.npc+"/"+g.waitState]);}
    else{float dx=player.x-owner.x,dy=player.y-owner.y;if(dx*dx+dy*dy<=g.near*g.near){a.attack=true;a.age=0;EnterNpcState(owner,npcStates[g.npc+"/"+g.attackState]);DragonEventV20("source-geyser-attack",owner,absolute);}else a.decision+=g.waitInterval;}
   }return true;
  }
  void BeginDragonNestRoomV20(){
   if(dragonNestV20==null||CurrentRoom.id!=dragonNestV20.intro.roomId)return;var r=dragonNestV20.intro;var king=enemies.Find(e=>e.sourceNPC==r.kingNpc&&e.active&&!e.dormant);if(king==null)throw new InvalidOperationException("Original Dragon boss room has no King");
   if(SimpleDangerIntroductions){dragonIntroKingV20=king;dragonIntroReleasedV20=true;king.aiEnabled=true;EnterNpcState(king,npcStates[r.kingNpc+"/LIZARDMAN_SHAMAN_KING_WAIT"]);return;}
   dragonIntroKingV20=king;dragonIntroBeganV20=elapsed;dragonIntroPreviousV20=0;dragonIntroReleasedV20=false;king.aiEnabled=false;EnterNpcState(king,npcStates[king.sourceNPC+"/LIZARDMAN_SHAMAN_KING_START_READY"]);dragonIntroSoundV20=new Enemy{sourceNPC=r.directorNpc};dragonIntroSoundEntryV20=++npcSoundEntrySerial;DragonEventV20("source-director-start",dragonIntroSoundV20,elapsed,"force-position(0,0,0)");
  }
  void TickDragonIntroV20(){
   if(SimpleDangerIntroductions)return;
   if(dragonNestV20==null||dragonIntroKingV20==null)return;var r=dragonNestV20.intro;float age=Math.Max(0,elapsed-dragonIntroBeganV20),before=dragonIntroPreviousV20;
   foreach(var s in r.sourceSoundEvents)if(before<s.time&&s.time<=age){var cue=new NpcTimedSound{id="dragon-director/"+s.sourceField,sound=s.sound,sourceName=s.sourceName,sourceField=s.sourceField,candidateClock="source-director-animation-seconds",candidateTime=s.time,sourceTime=s.time};QueueNpcSound(cue,dragonIntroSoundV20,dragonIntroSoundEntryV20,0,dragonIntroBeganV20+s.time,"EVENT_NPC_LIZARDMAN_SHAMAN_KING_START");}
   for(int i=0;i<3;i++)if(before<r.captionTimes[i]&&r.captionTimes[i]<=age)DragonEventV20("source-caption",dragonIntroSoundV20,dragonIntroBeganV20+r.captionTimes[i],r.captionIds[i],r.captionDurations[i]);
   if(!dragonIntroReleasedV20&&age+1e-6f>=r.directorDuration){dragonIntroReleasedV20=true;EnterNpcState(dragonIntroKingV20,npcStates[r.kingNpc+"/LIZARDMAN_SHAMAN_KING_START"]);dragonIntroKingV20.aiEnabled=true;dragonIntroKingV20.alerted=true;DragonEventV20("source-king-release",dragonIntroKingV20,dragonIntroBeganV20+r.directorDuration,r.releaseCallback);DragonEventV20("source-director-dying",dragonIntroSoundV20,dragonIntroBeganV20+r.directorDuration,"force-position(0,-5000,0)");}
   dragonIntroPreviousV20=age;
  }
  public bool TryDragonKingPresentationV20(Enemy enemy,out bool visible,out bool gauge){visible=gauge=true;if(dragonNestV20==null||enemy!=dragonIntroKingV20)return false;visible=gauge=dragonIntroReleasedV20;return true;}
 }
}
