#if UNITY_EDITOR
using System;using System.IO;using System.Text;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 int coreNaturalPassSlotV38=-1;bool coreNaturalPassRequestedV38;bool coreNaturalArmedV21,coreNaturalRunningV21;StreamWriter coreNaturalTraceV21;float coreNaturalNextAttackV21;long coreNaturalStepsV21;string coreNaturalTargetV21="";
 [Serializable]sealed class CoreNaturalInputRowV21 {public long step;public int frame,room,encounter;public float time,dt,x,y,hp;public bool paused;public string target;public Input input;}
 public bool CoreNaturalInputRunningV21{get{return coreNaturalArmedV21||coreNaturalRunningV21;}}
 public long CoreNaturalInputStepsV21{get{return coreNaturalStepsV21;}}
 public string CoreNaturalInputTargetV21{get{return coreNaturalTargetV21;}}
 public void ArmCoreHardNaturalInputV21(string output){if(!Application.isPlaying||!coreEntrySessionV21||coreNaturalArmedV21||coreNaturalRunningV21||forestAutoArmed||forestAutoRunning)throw new InvalidOperationException("Core natural input needs one public Core load and no other pilot");ResetCoreNaturalBlockerV522();ResetCoreNaturalFocusedV578();coreNaturalTraceV21=new StreamWriter(Path.Combine(output,"actual-inputs.jsonl"),false,new UTF8Encoding(false),65536);coreNaturalArmedV21=true;coreNaturalStepsV21=0;coreNaturalNextAttackV21=0;coreNaturalTargetV21="PUBLIC LOAD";}
 void StopCoreHardNaturalInputV21(){ResetCoreNaturalBlockerV522();ResetCoreNaturalFocusedV578();coreNaturalPassSlotV38=-1;coreNaturalPassRequestedV38=false;coreNaturalArmedV21=coreNaturalRunningV21=false;if(coreNaturalTraceV21!=null){coreNaturalTraceV21.Flush();coreNaturalTraceV21.Dispose();coreNaturalTraceV21=null;}}
 public void FlushCoreHardNaturalInputV21(){coreNaturalTraceV21?.Flush();}
 static bool CoreNaturalElementV21(Enemy e){return e.sourceNPC=="NUI_EL_EXTRACTOR_EARTH"||e.sourceNPC=="NUI_EL_EXTRACTOR_FIRE"||e.sourceNPC=="NUI_EL_EXTRACTOR_ICE"||e.sourceNPC=="NUI_EL_EXTRACTOR_NOVA";}
 bool CoreNaturalClearTargetV21(Enemy e){if(e.countsForClear)return true;foreach(var clause in game.CurrentEncounter.clauses){if(clause.mode==EncounterClearMode.KillNpcTypes&&Array.IndexOf(clause.npcTypes,e.sourceNPC)>=0)return true;if(clause.mode==EncounterClearMode.KillNpcKeyCodes&&Array.IndexOf(clause.npcKeyCodes,e.sourceKeyCode)>=0)return true;}return false;}
 bool CoreNaturalOriginalPassSkillV38(Input input,Enemy target){
  if(game.room!=4||game.EncounterIndex!=0||game.CurrentRoom.id!="Dungeon_Altera_Core_Hard-4"||game.CurrentEncounter.sourceWorldId!="WI_ALTERA_CORE_BOSS_STAGE"||!game.NativeTerrain)return false;
  if(coreNaturalPassRequestedV38&&(game.ActiveSkillId=="windmill"||game.ActiveSkillId=="roll")){
   if(!game.NativeNavigate(input,target.x,target.y)){input.move=Math.Abs(target.x-game.player.x)>34?Math.Sign(target.x-game.player.x):0;input.dash=false;}
   if(game.ActiveSkillId=="windmill"&&coreNaturalPassSlotV38>=0)input.skillHeldMask|=1<<coreNaturalPassSlotV38;return true;
  }
  coreNaturalPassRequestedV38=false;int direction=Math.Sign(target.x-game.player.x);if(direction==0)return false;bool contact=false;
  foreach(var blocker in game.enemies){if(blocker==target||!blocker.active||blocker.dormant||blocker.hp<=0||blocker.state==CombatState.Knockdown||blocker.visualScale!=1)continue;bool king=blocker.sourceNPC=="NUI_NASOD_KING",plant=blocker.sourceNPC=="NUI_EL_POWER_PLANT";if(!king&&!plant||game.CoreArenaBodyPassableV59(blocker))continue;float half=king?7:42,height=king?300.03972f:112;float boundary=blocker.x-direction*(9.8f+half);if(direction*(blocker.x-game.player.x)>0&&direction*(target.x-blocker.x)>0&&direction*(game.player.x-boundary)>=-2&&game.player.y<blocker.y+height&&game.player.y+42>blocker.y){contact=true;break;}}
  if(!contact)return false;int slot=-1;for(int i=0;i<GameModel.SkillSlotCount;i++){var skill=game.GetEquippedSkill(i);if(skill!=null&&skill.id=="roll"){slot=i;break;}if(skill!=null&&skill.id=="windmill")slot=i;}
  if(slot<0){coreNaturalTargetV21="HOLD ORIGINAL BODY: NO EQUIPPED PASS SKILL";input.move=0;return true;}var pass=game.GetEquippedSkill(slot);input.move=direction;input.dash=false;if(game.mana<pass.mpCost){if(game.potionCooldown<=0)input.potion=true;return true;}if(game.cooldowns[slot]>0||game.Busy())return true;
  input.skill=slot;coreNaturalPassSlotV38=slot;coreNaturalPassRequestedV38=true;if(pass.id=="windmill")input.skillHeldMask|=1<<slot;return true;
 }

 // R578 keeps only pilot-local observations. Public terrain queries are reads;
 // no actor, native-navigation cache, geometry or model clock is assigned here.
 GameModel coreNaturalScopeModelV578;int coreNaturalScopeRoomV578=-1,coreNaturalScopeEncounterV578=-1;
 float coreNaturalScopeLastModelV578;
 Enemy coreNaturalPlantExposureV578,coreNaturalDescentTargetV578;
 GroundSurface coreNaturalDescentUpperV578,coreNaturalDescentLowerV578;
 float coreNaturalDescentXV578;bool coreNaturalDescentRequestedV578;
 int coreNaturalBannerRenderV578=-1,coreNaturalBannerFrameV578=-1;
 void ResetCoreNaturalDescentV578(){coreNaturalDescentTargetV578=null;coreNaturalDescentUpperV578=coreNaturalDescentLowerV578=null;coreNaturalDescentXV578=0;coreNaturalDescentRequestedV578=false;}
 void ResetCoreNaturalFocusedV578(){ResetCoreNaturalDescentV578();coreNaturalPlantExposureV578=null;coreNaturalScopeModelV578=null;coreNaturalScopeRoomV578=coreNaturalScopeEncounterV578=-1;coreNaturalScopeLastModelV578=0;coreNaturalBannerRenderV578=coreNaturalBannerFrameV578=-1;}
 void ObserveCoreNaturalScopeV578(){
  if(!object.ReferenceEquals(coreNaturalScopeModelV578,game)||coreNaturalScopeRoomV578!=game.room||coreNaturalScopeEncounterV578!=game.EncounterIndex||game.elapsed<coreNaturalScopeLastModelV578)ResetCoreNaturalFocusedV578();
  coreNaturalScopeModelV578=game;coreNaturalScopeRoomV578=game.room;coreNaturalScopeEncounterV578=game.EncounterIndex;coreNaturalScopeLastModelV578=game.elapsed;
  var e=coreNaturalPlantExposureV578;
  if(e!=null&&(!game.enemies.Contains(e)||!e.active||e.dormant||e.passive||e.hp<=0||e.encounterIndex!=game.EncounterIndex||e.sourceNPC!="NUI_EL_POWER_PLANT"||(e.npcState!="EL_POWER_PLANT_UP"&&e.npcState!="EL_POWER_PLANT_WAIT_UP"&&e.npcState!="EL_POWER_PLANT_DAMAGE")))coreNaturalPlantExposureV578=null;
 }
 bool CoreNaturalPlantEligibleV578(Enemy e){
  if(e.sourceNPC!="NUI_EL_POWER_PLANT")return false;
  if(e.npcState=="EL_POWER_PLANT_UP"||e.npcState=="EL_POWER_PLANT_WAIT_UP"){coreNaturalPlantExposureV578=e;return true;}
  return e.npcState=="EL_POWER_PLANT_DAMAGE"&&object.ReferenceEquals(coreNaturalPlantExposureV578,e);
 }
 bool CoreNaturalBannerWaitV578(){
  if(game.room!=1||game.EncounterIndex!=0||game.CurrentRoom.id!="Dungeon_Altera_Core_Hard-1"||game.CurrentEncounter.sourceWorldId!="WI_ALTERA_CORE_STAGE1_MIDDLE")return false;
  if(game.roomBanner>0){coreNaturalBannerRenderV578=coreNaturalBannerFrameV578=-1;return true;}
  if(coreNaturalBannerRenderV578<0){coreNaturalBannerRenderV578=Time.renderedFrameCount;coreNaturalBannerFrameV578=Time.frameCount;return true;}
  // This global render counter is not a world-camera capture receipt. The
  // original probe still decides all camera/same-frame/quality acceptance.
  return Time.renderedFrameCount<=coreNaturalBannerRenderV578||Time.frameCount<=coreNaturalBannerFrameV578;
 }
 bool CoreNaturalElementDescentV578(Input input,Enemy target){
  if(game.room!=4||game.EncounterIndex!=0||game.CurrentRoom.id!="Dungeon_Altera_Core_Hard-4"||game.CurrentEncounter.sourceWorldId!="WI_ALTERA_CORE_BOSS_STAGE"||!game.NativeTerrain||!game.enemies.Contains(target)||!target.active||target.dormant||target.passive||target.hp<=0||target.encounterIndex!=game.EncounterIndex||!CoreNaturalElementV21(target)||!game.CoreHardActorShownV21(target)||game.Busy()){ResetCoreNaturalDescentV578();return false;}
  if(coreNaturalDescentTargetV578!=null&&!object.ReferenceEquals(coreNaturalDescentTargetV578,target))ResetCoreNaturalDescentV578();
  var goal=game.SurfaceBelow(target.x,target.y+.1f);
  if(goal==null||Math.Abs(target.y-goal.Y(target.x))>=.06f){ResetCoreNaturalDescentV578();return false;}
  if(coreNaturalDescentTargetV578!=null){
   if(!object.ReferenceEquals(coreNaturalDescentLowerV578,goal)||!object.ReferenceEquals(game.Surface(coreNaturalDescentUpperV578.id),coreNaturalDescentUpperV578)){ResetCoreNaturalDescentV578();return false;}
   if(coreNaturalDescentRequestedV578){
    // After the ordinary drop request, steer toward the same actual interior
    // until observed landing. Do not hand the fall back to the stale portal.
    if(game.Grounded(game.player)){ResetCoreNaturalDescentV578();return false;}
    input.move=Math.Abs(coreNaturalDescentXV578-game.player.x)>3?Math.Sign(coreNaturalDescentXV578-game.player.x):0;return true;
   }
  }
  if(target.y>=game.player.y-45||!game.Grounded(game.player)||game.player.dropThrough>0){ResetCoreNaturalDescentV578();return false;}
  var start=game.SurfaceBelow(game.player.x,game.player.y+.1f);
  if(start==null||!start.platform||Math.Abs(start.Y(game.player.x)-game.player.y)>=.06f||Math.Abs(start.y2-start.y1)>.02f){ResetCoreNaturalDescentV578();return false;}
  if(coreNaturalDescentTargetV578==null){
   GroundSurface chosen=null;float chosenX=0,best=float.PositiveInfinity;
   foreach(var upper in game.CurrentRoom.surfaces){
    // Only this support or a truly touching/overlapping coplanar neighbour.
    // No +/-x tolerance creates a bridge over an actual source gap.
    if(!upper.platform||Math.Abs(upper.y1-start.y1)>.02f||Math.Abs(upper.y2-start.y1)>.02f||upper.x1>start.x2||upper.x2<start.x1)continue;
    float lo=Math.Max(Math.Max(upper.x1,goal.x1),game.CurrentEncounter.leftX),hi=Math.Min(Math.Min(upper.x2,goal.x2),game.CurrentEncounter.rightX);
    if(hi-lo<=6)continue;float margin=Math.Max(3,Math.Min(12,(hi-lo)*.25f)),x=Math.Max(lo+margin,Math.Min(hi-margin,target.x));
    float delta=goal.Y(x)-upper.Y(x);if(delta>=-8||delta< -300)continue;
    if(!object.ReferenceEquals(game.SurfaceBelow(x,upper.Y(x)+.05f),upper)||!object.ReferenceEquals(game.SurfaceBelow(x,upper.Y(x)-.1f),goal))continue;
    float distance=Math.Abs(x-game.player.x);if(distance<best){chosen=upper;chosenX=x;best=distance;}
   }
   if(chosen==null)return false;
   coreNaturalDescentTargetV578=target;coreNaturalDescentUpperV578=chosen;coreNaturalDescentLowerV578=goal;coreNaturalDescentXV578=chosenX;
  }
  var route=coreNaturalDescentUpperV578;
  if(!start.platform||Math.Abs(start.y1-route.y1)>.02f||Math.Abs(start.y2-route.y1)>.02f||start.x1>route.x2||start.x2<route.x1){ResetCoreNaturalDescentV578();return false;}
  float left=Math.Max(Math.Max(route.x1,goal.x1),game.CurrentEncounter.leftX),right=Math.Min(Math.Min(route.x2,goal.x2),game.CurrentEncounter.rightX);
  if(right-left<=6){ResetCoreNaturalDescentV578();return false;}
  if(game.player.x>=left+3&&game.player.x<=right-3&&object.ReferenceEquals(start,route)&&object.ReferenceEquals(game.SurfaceBelow(game.player.x,game.player.y-.1f),goal)){
   // The freshly constructed Input has no jump/attack/skill. Only the model's
   // original accept/Grounded/platform drop branch can perform the descent.
   input.move=0;input.drop=true;coreNaturalDescentRequestedV578=true;return true;
  }
  input.move=Math.Sign(coreNaturalDescentXV578-game.player.x);return true;
 }

 // Original WAIT_UP body helpers are above the UNIT_WIDTH/UNIT_HEIGHT
 // blocker: upper sphere y140.49997/r48.49742 at source scale1. Navigate to
 // that visible upper helper's existing support, then jump/attack normally.
 // This only chooses Input; it never moves or changes the target/helper.
 bool CoreNaturalPlantCombatV41(Input input,Enemy target){
  if(game.room!=4||game.EncounterIndex!=0||game.CurrentRoom.id!="Dungeon_Altera_Core_Hard-4"||game.CurrentEncounter.sourceWorldId!="WI_ALTERA_CORE_BOSS_STAGE"||!game.NativeTerrain||!CoreNaturalPlantEligibleV578(target))return false;
  float upperY=target.y+(327.16488f+GameModel.CorePlantHelperOriginYV59)*target.visualScale,combatY=upperY-21,dx=target.x-game.player.x;
  bool navigating=game.NativeNavigate(input,target.x,upperY);
  if(!navigating){input.move=Math.Abs(dx)>34?Math.Sign(dx):0;input.dash=false;if(dx*game.player.facing<0)input.move=Math.Sign(dx);}
  if(Math.Abs(dx)<120&&game.Grounded(game.player)&&game.player.y+42<upperY-48.49742f*target.visualScale)input.jump=true;
  float combatDistance=(float)Math.Sqrt(dx*dx+(combatY-game.player.y)*(combatY-game.player.y));
  if(combatDistance<110&&Math.Abs(combatY-game.player.y)<48&&game.elapsed>=coreNaturalNextAttackV21){input.attack=true;coreNaturalNextAttackV21=game.elapsed+.16f;}
  if(combatDistance<110&&game.CanAwaken){input.awaken=true;input.attack=false;return true;}
  if(combatDistance<110&&!game.Busy()&&!input.jump)for(int slot=0;slot<GameModel.SkillSlotCount;slot++){
   var skill=game.GetEquippedSkill(slot);if(skill==null||game.cooldowns[slot]>0||game.mana<skill.mpCost||game.HyperEssence<skill.costItem||!skill.airborne&&!game.Grounded(game.player))continue;
   if(skill.id=="counter-attack"||skill.id=="roll"||skill.id=="windmill")continue;if(skill.id=="iron-body"&&(game.armor>0||game.player.hp>340))continue;if((skill.id=="armor-break"||skill.id=="kick")&&Math.Abs(dx)>28)continue;input.skill=slot;input.attack=false;break;
  }
  return true;
 }

 // R522 observes only this pilot's requested Input and model-clock progress.
 // Half a pixel is tracking tolerance, never an attack/body/terrain radius.
 const float CoreNaturalBlockerWaitV522=1f,CoreNaturalBlockerProgressSquaredV522=.25f;
 Enemy coreNaturalBlockerGoalV522,coreNaturalBlockerTargetV522;
 int coreNaturalBlockerRoomV522=-1,coreNaturalBlockerEncounterV522=-1,coreNaturalBlockerRequestedDirectionV522;
 float coreNaturalBlockerAnchorXV522,coreNaturalBlockerAnchorYV522,coreNaturalBlockerProgressModelV522,coreNaturalBlockerLastModelV522;
 void ResetCoreNaturalBlockerV522(){coreNaturalBlockerGoalV522=coreNaturalBlockerTargetV522=null;coreNaturalBlockerRoomV522=coreNaturalBlockerEncounterV522=-1;coreNaturalBlockerRequestedDirectionV522=0;coreNaturalBlockerAnchorXV522=coreNaturalBlockerAnchorYV522=coreNaturalBlockerProgressModelV522=coreNaturalBlockerLastModelV522=0;}
 Input RememberCoreNaturalBlockerInputV522(Input input){if(coreNaturalBlockerGoalV522!=null)coreNaturalBlockerRequestedDirectionV522=Math.Sign(input.move);return input;}
 bool CoreNaturalBlockerEligibleV522(Enemy e,Enemy goal,int direction,out float distance){
  distance=float.PositiveInfinity;
  if(e==null||object.ReferenceEquals(e,goal)||!e.active||e.dormant||e.passive||e.hp<=0||e.encounterIndex!=game.EncounterIndex)return false;
  if(e.isGate&&(!game.EncounterCombatClear||e.invincible>0))return false;
  if(!e.isGate&&!CoreNaturalClearTargetV21(e))return false;
  if(direction*(e.x-game.player.x)<=0||Math.Abs(e.y-game.player.y)>=45)return false;
  distance=Vector2.Distance(new Vector2(e.x,e.y),new Vector2(game.player.x,game.player.y));return distance<100;
 }
 void SelectCoreNaturalBlockerTargetV522(ref Enemy target,ref float distance){
  bool healer=target.sourceNPC=="NUI_NASOD_HEALER"||target.sourceNPC=="NUI_NASOD_HEALER_BLADER"||target.sourceNPC=="NUI_NASOD_HEALER_BLADE";
  if(game.room>=3||!healer||distance<100){ResetCoreNaturalBlockerV522();return;}
  int direction=Math.Sign(target.x-game.player.x);if(direction==0){ResetCoreNaturalBlockerV522();return;}
  if(coreNaturalBlockerRoomV522!=game.room||coreNaturalBlockerEncounterV522!=game.EncounterIndex||!object.ReferenceEquals(coreNaturalBlockerGoalV522,target)||game.elapsed<coreNaturalBlockerLastModelV522){
   ResetCoreNaturalBlockerV522();coreNaturalBlockerRoomV522=game.room;coreNaturalBlockerEncounterV522=game.EncounterIndex;coreNaturalBlockerGoalV522=target;coreNaturalBlockerAnchorXV522=game.player.x;coreNaturalBlockerAnchorYV522=game.player.y;coreNaturalBlockerProgressModelV522=game.elapsed;
  }
  coreNaturalBlockerLastModelV522=game.elapsed;
  float dx=game.player.x-coreNaturalBlockerAnchorXV522,dy=game.player.y-coreNaturalBlockerAnchorYV522;
  if(dx*dx+dy*dy>=CoreNaturalBlockerProgressSquaredV522){coreNaturalBlockerAnchorXV522=game.player.x;coreNaturalBlockerAnchorYV522=game.player.y;coreNaturalBlockerProgressModelV522=game.elapsed;coreNaturalBlockerTargetV522=null;}
  if(coreNaturalBlockerTargetV522!=null){
   float near;if(CoreNaturalBlockerEligibleV522(coreNaturalBlockerTargetV522,target,direction,out near)){target=coreNaturalBlockerTargetV522;distance=near;return;}
   coreNaturalBlockerTargetV522=null;coreNaturalBlockerAnchorXV522=game.player.x;coreNaturalBlockerAnchorYV522=game.player.y;coreNaturalBlockerProgressModelV522=game.elapsed;
  }
  if(coreNaturalBlockerRequestedDirectionV522!=direction){coreNaturalBlockerAnchorXV522=game.player.x;coreNaturalBlockerAnchorYV522=game.player.y;coreNaturalBlockerProgressModelV522=game.elapsed;return;}
  if(game.elapsed-coreNaturalBlockerProgressModelV522<CoreNaturalBlockerWaitV522)return;
  Enemy blocker=null;float best=100;
  foreach(var e in game.enemies){float near;if(CoreNaturalBlockerEligibleV522(e,target,direction,out near)&&near<best){blocker=e;best=near;}}
  if(blocker!=null){coreNaturalBlockerTargetV522=blocker;target=blocker;distance=best;}
 }

 Input CoreHardNaturalInputV21(Input manual){
  if(coreNaturalArmedV21&&forestPreviewActive){coreNaturalArmedV21=false;coreNaturalRunningV21=true;}
  if(!coreNaturalRunningV21){ResetCoreNaturalFocusedV578();return manual;}var input=new Input{skill=-1};if(game.paused||game.victory||game.player.hp<=0){ResetCoreNaturalBlockerV522();ResetCoreNaturalFocusedV578();return input;}
  ObserveCoreNaturalScopeV578();
  if(game.player.hp<340)input.potion=true;
  // Presentation is the real existing GameView/bridge owner. Waiting here only
  // withholds player attacks; it neither manufactures nor queues a focus ack.
  if(game.CurrentCoreStopIntroTicketsV21().Length>0){ResetCoreNaturalBlockerV522();ResetCoreNaturalFocusedV578();coreNaturalTargetV21="WAIT FOR REAL STOP VIEW OWNER";return input;}
  if(game.room==3){ResetCoreNaturalBlockerV522();ResetCoreNaturalFocusedV578();coreNaturalTargetV21="WAIT ORIGINAL PD 8 SECOND SELF-DEATH KEY1";if(game.roomBanner<=0)input.move=1;return input;}
  if(game.gateOpen&&game.room!=4){ResetCoreNaturalBlockerV522();ResetCoreNaturalDescentV578();coreNaturalPlantExposureV578=null;if(CoreNaturalBannerWaitV578()){coreNaturalTargetV21="WAIT ORIGINAL ROOM BANNER AND RENDER";return new Input{skill=-1};}coreNaturalTargetV21="ORIGINAL EXIT";float exit=game.HasEncounters?game.CurrentEncounter.exitX:game.ExitX;int direction=game.HasEncounters?game.CurrentEncounter.direction:1;input.move=Math.Abs(exit-game.player.x)>1?Math.Sign(exit-game.player.x):direction;input.dash=true;game.NativeNavigate(input,exit,game.GroundAt(game.CurrentRoom,exit));return input;}
  Enemy target=null;float distance=float.PositiveInfinity,score=float.PositiveInfinity;
  foreach(var e in game.enemies){
   if(!e.active||e.dormant||e.passive||e.hp<=0||e.encounterIndex!=game.EncounterIndex)continue;
   if(e.isGate&&(!game.EncounterCombatClear||e.invincible>0))continue;
   if(game.room==4){
    // All seven original final births have countsForClear=false. Clear four
    // element devices before striking Plant in its original30s exposed window.
    // King regenerates while Plant lives and is never the pilot's final target.
    if(!CoreNaturalElementV21(e)&&e.sourceNPC!="NUI_EL_POWER_PLANT")continue;
    if(!game.CoreHardActorShownV21(e))continue;
    if(e.sourceNPC=="NUI_EL_POWER_PLANT"&&!CoreNaturalPlantEligibleV578(e))continue;
   }else if(!e.isGate&&!CoreNaturalClearTargetV21(e))continue;
   float d=Vector2.Distance(new Vector2(e.x,e.y),new Vector2(game.player.x,game.player.y));float s=d;
   if(game.room==4&&CoreNaturalElementV21(e))s-=100000;
   if((e.sourceNPC=="NUI_NASOD_HEALER"||e.sourceNPC=="NUI_NASOD_HEALER_BLADER"||e.sourceNPC=="NUI_NASOD_HEALER_BLADE")&&d<360)s-=10000;
   if(s<score){target=e;distance=d;score=s;}
  }
  if(target==null){ResetCoreNaturalBlockerV522();ResetCoreNaturalFocusedV578();coreNaturalTargetV21="WAIT SOURCE CONDITION OR DEVICE SHOW";return input;}
  SelectCoreNaturalBlockerTargetV522(ref target,ref distance);
  coreNaturalTargetV21=target.sourceNPC;
  if(CoreNaturalElementDescentV578(input,target))return input;
  if(CoreNaturalPlantCombatV41(input,target))return input;
  if(CoreNaturalOriginalPassSkillV38(input,target))return input;
  // Reuse the mature Below/Airship ordinary-input navigator and close combat
  // opportunities. NativeNavigate updates navigation caches and Input only.
  if(AutoNavigateCombat(input,target,distance,ref coreNaturalNextAttackV21))return RememberCoreNaturalBlockerInputV522(input);
  float dx=target.x-game.player.x;input.move=Math.Abs(dx)>34?Math.Sign(dx):0;input.dash=distance>200;if(dx*game.player.facing<0)input.move=Math.Sign(dx);
  if(distance<100&&Math.Abs(target.y-game.player.y)<45&&game.elapsed>=coreNaturalNextAttackV21){input.attack=true;coreNaturalNextAttackV21=game.elapsed+.16f;}
  if(distance<110&&game.CanAwaken){input.awaken=true;input.attack=false;return RememberCoreNaturalBlockerInputV522(input);}
  if(distance<110&&!game.Busy())for(int slot=0;slot<GameModel.SkillSlotCount;slot++){
   var skill=game.GetEquippedSkill(slot);if(skill==null||game.cooldowns[slot]>0||game.mana<skill.mpCost||game.HyperEssence<skill.costItem)continue;
   if(skill.id=="counter-attack"||skill.id=="roll")continue;if(skill.id=="iron-body"&&(game.armor>0||game.player.hp>340))continue;if((skill.id=="armor-break"||skill.id=="kick")&&Math.Abs(dx)>28)continue;input.skill=slot;input.attack=false;break;
  }
  return RememberCoreNaturalBlockerInputV522(input);
 }
 void RecordCoreHardNaturalStepV21(Input input,float dt){if(!coreNaturalRunningV21||coreNaturalTraceV21==null)return;coreNaturalTraceV21.WriteLine(JsonUtility.ToJson(new CoreNaturalInputRowV21{step=++coreNaturalStepsV21,frame=Time.frameCount,room=game.room,encounter=game.EncounterIndex,time=game.elapsed,dt=dt,x=game.player.x,y=game.player.y,hp=game.player.hp,paused=game.paused,target=coreNaturalTargetV21,input=input}));if(coreNaturalStepsV21%300==0)coreNaturalTraceV21.Flush();}
}}
#endif
