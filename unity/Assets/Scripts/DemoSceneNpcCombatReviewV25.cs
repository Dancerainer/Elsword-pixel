using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class NativeCombatObservationV25 {
   public string caseId,npc,kind,operation,image,modelClip,actualClip,sprite,textureResource,npcState,combatState,textureTopDownRgbaSha256;
   public int sourceSpawnIndex,stagedIndex,dungeon,room,actualFrame,frameCount,modelSteps,textureWidth,textureHeight,actualDamage;
   public float modelTime,modelClock,actualRenderClock,npcTime,x,y,vx,vy,hp,playerHp,playerX,playerY,spriteFps,spriteDuration;
   public bool actualLoop,flipX,flipY,modelPaused,roomBannerCleared;
   public First3BoneObservationV25 rootRegistration;
  }
  [Serializable] sealed class NativeCombatReportV25 {
   public string schema="feedback-v25-first3-actual-main-birth-model-combat-r1",catalogSha256,planSha256,lastPlayerSupportSearch,error="";
   public bool completed,actualWindowsPlayer;public int cases,playerDiagnosticHealthRestorations;
   public string scope="Actual main Ruins/Cave births and GameModel Step/public damage APIs, rendered by the real Windows Player. Only player support/health are explicitly maintained for bounded diagnostics; no enemy clocks, flags, HP, roots or states are assigned. This is a paused model-trigger replay, not a natural route, frame-rate benchmark, all-source-mechanism or all-animation-frame acceptance.";
   public NativeCombatObservationV25[] observations;
  }
  sealed class NativeCombatStepV25 {public string label,target;public int ticks,max=900,damage;public float minimumClock;public HitReaction reaction=HitReaction.Hurt;public bool unprotected;}
  NativeCombatReportV25 nativeCombatReportV25;string nativeCombatReportPathV25;int nativeCombatStepsV25;
  readonly List<NativeCombatObservationV25> nativeCombatRowsV25=new List<NativeCombatObservationV25>();
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void StartNativeCombatReviewV25(){
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--feedback-v25-npc-combat-review")<0)return;
   var scene=FindObjectOfType<DemoScene>();if(!scene)throw new InvalidOperationException("Native combat review needs real scene");scene.StartCoroutine(scene.GuardNativeCombatReviewV25());
  }
  IEnumerator GuardNativeCombatReviewV25(){
   var iterator=RunNativeCombatReviewV25();
   while(true){object value=null;bool next=false;Exception failure=null;try{next=iterator.MoveNext();if(next)value=iterator.Current;}catch(Exception error){failure=error;}
    if(failure!=null){if(nativeCombatReportV25!=null&&nativeCombatReportPathV25!=null){nativeCombatReportV25.error=failure.ToString();WriteNativeCombatReportV25();}Debug.LogException(failure);if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0)Application.Quit(1);yield break;}
    if(!next)yield break;yield return value;
   }
  }
  void WriteNativeCombatReportV25(){nativeCombatReportV25.observations=nativeCombatRowsV25.ToArray();File.WriteAllText(nativeCombatReportPathV25,JsonUtility.ToJson(nativeCombatReportV25,true));}
  void PlaceCombatDiagnosticPlayerV25(Enemy enemy){
   float left=game.HasEncounters?game.CurrentEncounter.leftX+2:2,right=game.HasEncounters?game.CurrentEncounter.rightX-2:game.RoomWidth-2;
   bool tree=enemy.sourceNPC=="NUI_TREE_KNIGHT",batB=enemy.sourceNPC=="NUI_BAT_B";float[] offsets=tree?new[]{-38f,-42f,-32f,38f,32f}:batB?new[]{-32f,-28f,32f,28f,-80f,80f}:new[]{-38f,-80f,-112f,80f,38f,112f};bool placed=false,followHitTarget=batB&&enemy.npcState=="BAT_B_ATTACK_A_DRAIN";
   string search="enemy=("+enemy.x+","+enemy.y+")";
   if(!followHitTarget)foreach(float offset in offsets){
    float x=Mathf.Clamp(enemy.x+offset,left,right);var support=game.SurfaceBelow(x,enemy.y+(tree?34:96));
    search+=" candidate="+x+"/"+(support==null?"none":support.id+":"+support.Y(x));
    if(support==null)continue;float dx=x-enemy.x,dy=support.Y(x)-enemy.y;
    if(tree?(Mathf.Abs(dx)>=43||Mathf.Abs(dy)>=38):(dx*dx+dy*dy>=140*140))continue;
    game.player.x=x;game.player.y=support.Y(x);game.player.groundLine=support.id;placed=true;break;
   }
   // An authored airborne action can temporarily leave the point directly
   // below its actor. Keep the last real player support instead of moving an
   // enemy or treating that diagnostic locator gap as a model failure.
   if(!placed){var support=game.Surface(game.player.groundLine);if(support==null||game.player.x<support.x1||game.player.x>support.x2)throw new InvalidOperationException("Combat diagnostic has no real player support: "+search);game.player.y=support.Y(game.player.x);search+=" retained-player-support="+support.id;}
   nativeCombatReportV25.lastPlayerSupportSearch=search;game.player.vx=game.player.vy=0;
   if(game.player.hp<400){game.player.hp=500;nativeCombatReportV25.playerDiagnosticHealthRestorations++;}
  }
  void StepCombatDiagnosticV25(Enemy enemy){
   if(enemy.hp<=0||game.player.hp<=0||game.victory)throw new InvalidOperationException("Live combat target/player/route ended before planned transition");
   PlaceCombatDiagnosticPlayerV25(enemy);game.paused=false;try{game.Step(1f/60,new Input{skill=-1});}finally{game.paused=true;}nativeCombatStepsV25++;
  }
  bool CombatOrdinaryDamageWindowV25(Enemy e){return e.hp>0&&game.Grounded(e)&&e.state==CombatState.Normal&&!game.EnemyHasSuperArmor(e)&&!game.EnemyHasNpcInvincibility(e)&&e.invincible<=0;}
  int AdvanceCombatDiagnosticV25(Enemy enemy,NativeCombatStepV25 step){
   if(step.unprotected){int wait=0;while(!CombatOrdinaryDamageWindowV25(enemy)&&wait++<step.max)StepCombatDiagnosticV25(enemy);if(!CombatOrdinaryDamageWindowV25(enemy))throw new InvalidOperationException("No real unprotected reaction window: "+step.label);}
   int dealt=0;if(step.damage>0){dealt=game.ApplyHitToEnemy(enemy,step.damage,step.reaction);if(dealt<=0)throw new InvalidOperationException("Public diagnostic damage was rejected: "+step.label);}
   if(step.target!=null){int wait=0;while((enemy.clip!=step.target||enemy.clock+.0001f<step.minimumClock)&&wait++<step.max)StepCombatDiagnosticV25(enemy);if(enemy.clip!=step.target)throw new InvalidOperationException("Normal model did not reach "+step.target+" for "+step.label);}
   for(int i=0;i<step.ticks;i++){
    if(enemy.hp>0)StepCombatDiagnosticV25(enemy);
    else{game.paused=false;try{game.Step(1f/60,new Input{skill=-1});}finally{game.paused=true;}nativeCombatStepsV25++;}
   }
   return dealt;
  }
  List<NativeCombatStepV25> CombatDiagnosticPlanV25(string npc){
   bool tree=npc=="NUI_TREE_KNIGHT";var steps=new List<NativeCombatStepV25>();
   steps.Add(new NativeCombatStepV25{label="settled-native"});
   steps.Add(new NativeCombatStepV25{label=tree?"normal-attack-start":"normal-ready-start",target=tree?"attack":"attack-a-ready"});
   if(!tree){steps.Add(new NativeCombatStepV25{label="normal-ready-middle",target="attack-a-ready",minimumClock=1});steps.Add(new NativeCombatStepV25{label="normal-attack-start",target="attack-a"});}
   steps.Add(new NativeCombatStepV25{label="normal-attack-middle-or-source-transition",ticks=30});
   if(npc=="NUI_BAT_A")steps.Add(new NativeCombatStepV25{label="normal-attack-back-recovery",target="get-up-back"});
   if(npc=="NUI_BAT_B") {
    steps.Add(new NativeCombatStepV25{label="normal-hit-triggered-drain",target="attack-a-drain"});
    steps.Add(new NativeCombatStepV25{label="normal-drain-late-root-peak",target="attack-a-drain",minimumClock=3.8f});
   }
   steps.Add(new NativeCombatStepV25{label="public-hurt-start",unprotected=true,damage=1});
   steps.Add(new NativeCombatStepV25{label="public-hurt-progress",ticks=10});
   steps.Add(new NativeCombatStepV25{label="public-launch-start",unprotected=true,damage=1,reaction=HitReaction.Launch});
   steps.Add(new NativeCombatStepV25{label="physical-descending",target="fall"});
   steps.Add(new NativeCombatStepV25{label="physical-down-hold",target="knockdown"});
   steps.Add(new NativeCombatStepV25{label="physical-get-up-start",target=tree?"getup":"get-up"});
   steps.Add(new NativeCombatStepV25{label="physical-get-up-progress",ticks=12});
   steps.Add(new NativeCombatStepV25{label="public-lethal-start",unprotected=true,damage=1000000});
   steps.Add(new NativeCombatStepV25{label="public-death-progress",ticks=36});return steps;
  }
  void CaptureCombatDiagnosticV25(FeedbackNpcCaseV25 c,Enemy enemy,NativeCombatStepV25 step,int dealt){
   game.roomBanner=0;RenderPausedFeedbackNpcV25();var view=enemyViews[c.stagedIndex];var source=FeedbackNpcSourceSpriteV25(view.sprite);
   string prefix=enemy.kind+"/",name=source.name;if(!name.StartsWith(prefix+"source-",StringComparison.Ordinal))throw new InvalidOperationException("Combat display used old/fallback art: "+name);
   int slash=name.LastIndexOf('/'),frame=int.Parse(name.Substring(slash+1));string clip=name.Substring(prefix.Length,slash-prefix.Length);var lib=libraries[enemy.kind];var info=lib.clips[clip];var data=info.frames[frame];
   string resource=string.IsNullOrEmpty(data.texture)?(string.IsNullOrEmpty(info.texture)?lib.data.texture:info.texture):data.texture;
   if(resource!="Pixel/feedback-v25/"+enemy.kind+"/atlas"||Resources.Load<Texture2D>(resource)!=view.sprite.texture)throw new InvalidOperationException("Actual combat source/texture identity differs");
   var bone=First3ActualBoneObservationV25(enemy);if(bone==null||bone.frame!=frame||bone.sprite!=name||bone.registeredBipErrorLogical>.001f)throw new InvalidOperationException("Actual combat same-frame registered Bip differs from model coordinate reference");
   string image=c.id+"-combat-"+step.label+".png";float time=game.elapsed;Capture(Path.GetFileNameWithoutExtension(image));if(game.elapsed!=time||!File.Exists(Path.Combine(captureDir,image)))throw new InvalidOperationException("Combat capture drifted model clock or image absent");
   nativeCombatRowsV25.Add(new NativeCombatObservationV25{caseId=c.id,npc=c.npc,kind=enemy.kind,operation=step.label,image=image,modelClip=enemy.clip,actualClip=clip,sprite=name,textureResource=resource,npcState=enemy.npcState,combatState=enemy.state.ToString(),sourceSpawnIndex=c.sourceSpawnIndex,stagedIndex=c.stagedIndex,dungeon=c.dungeon,room=c.room,actualFrame=frame,frameCount=info.frames.Length,modelSteps=nativeCombatStepsV25,textureWidth=view.sprite.texture.width,textureHeight=view.sprite.texture.height,actualDamage=dealt,modelTime=game.elapsed,modelClock=enemy.clock,actualRenderClock=First3ActualRenderClockV25(enemy),npcTime=enemy.npcTime,x=enemy.x,y=enemy.y,vx=enemy.vx,vy=enemy.vy,hp=enemy.hp,playerHp=game.player.hp,playerX=game.player.x,playerY=game.player.y,spriteFps=info.fps,spriteDuration=info.duration,actualLoop=info.loop,flipX=view.flipX,flipY=view.flipY,modelPaused=game.paused,roomBannerCleared=game.roomBanner==0,rootRegistration=bone,textureTopDownRgbaSha256=FeedbackNpcTextureRgbaV25(view.sprite.texture)});WriteNativeCombatReportV25();
  }
  IEnumerator RunNativeCombatReviewV25(){
   Application.runInBackground=true;if(!UseOffscreenCaptures)throw new InvalidOperationException("Combat diagnostics require offscreen captures");
   string path=Environment.GetEnvironmentVariable("EL_FEEDBACK_V25_NPC_PLAN");byte[] bytes=File.ReadAllBytes(path);var plan=JsonUtility.FromJson<FeedbackNpcPlanV25>(System.Text.Encoding.UTF8.GetString(bytes));
   string catalogHash=FeedbackNpcHashV25(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes);if(plan.catalogSha256!=catalogHash)throw new InvalidOperationException("Combat plan catalog differs");
   Directory.CreateDirectory(captureDir);nativeCombatReportPathV25=Path.Combine(captureDir,"feedback-v25-npc-combat.json");if(File.Exists(nativeCombatReportPathV25))throw new InvalidOperationException("Preserve existing combat report");
   nativeCombatReportV25=new NativeCombatReportV25{catalogSha256=catalogHash,planSha256=FeedbackNpcHashV25(bytes),actualWindowsPlayer=!Application.isEditor};
   monsterArtReview=true;runAutomatedPlaythrough=false;game.paused=true;
   foreach(string npc in new[]{"NUI_TREE_KNIGHT","NUI_BAT_A","NUI_BAT_B"}){
    var c=Array.Find(plan.cases,x=>x.npc==npc);if(c==null)throw new InvalidOperationException("Actual main combat birth absent");
    selectedDungeon=previewDungeon=c.dungeon;ConfigureRooms();typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.room});if(game.HasEncounters&&game.EncounterIndex!=c.encounter)typeof(GameModel).GetMethod("BeginEncounter",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.encounter});
    CancelBossIntroduction();game.paused=true;var enemy=FeedbackNpcModelStepsV25.Subject(game,c);FeedbackNpcModelStepsV25.LocatePlayer(game,c.far);adventureStarted=true;openMenu="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();CancelBossIntroduction();RememberSimulationState();RenderMenus();hudCanvas.enabled=true;nativeCombatStepsV25=0;
    foreach(var initial in c.samples){RestoreNativeProjection();nativeCombatStepsV25+=FeedbackNpcModelStepsV25.Advance(game,enemy,c,initial);}
    foreach(var step in CombatDiagnosticPlanV25(npc)){
     RestoreNativeProjection();int dealt=AdvanceCombatDiagnosticV25(enemy,step);RememberSimulationState();accumulator=0;for(int i=0;i<3;i++)yield return null;yield return new WaitForEndOfFrame();CaptureCombatDiagnosticV25(c,enemy,step,dealt);
    }
    nativeCombatReportV25.cases++;
   }
   nativeCombatReportV25.completed=true;WriteNativeCombatReportV25();Debug.Log("FEEDBACK_V25_NPC_COMBAT_COMPLETE images="+nativeCombatRowsV25.Count);if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0){yield return new WaitForSecondsRealtime(.5f);Application.Quit();}
  }
 }
}
