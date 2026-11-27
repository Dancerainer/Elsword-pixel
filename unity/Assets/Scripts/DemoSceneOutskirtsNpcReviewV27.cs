using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class OutskirtsNpcReviewCaseV27 {
  public string id,mode,stage,npc,kind;public int dungeon,room,stagedIndex,sourceSpawnIndex,encounter;
 }
 [Serializable] public sealed class OutskirtsNpcReviewAtlasV27 {
  public string kind,resource,pngSha256,rgbaSha256;public int width,height;
 }
 [Serializable] public sealed class OutskirtsNpcReviewPlanV27 {
  public string schema,catalogSha256,sourceDungeon,diagnosticScope;public int expectedBirths,expectedImages,expectedAtlasCount;public OutskirtsNpcReviewAliasV27[] aliases;
  public OutskirtsNpcReviewCaseV27[] cases;public OutskirtsNpcReviewAtlasV27[] atlases;public string[] limitations;
 }
 [Serializable] public sealed class OutskirtsNpcReviewAliasV27 {
  public string kind,id,sourceAnimation;public float sourceAnimSpeed,sourceLoopPeriod,rawMotionDuration;
  public float[] nativeAvailableTimes,nativeSampleTimes;public string[] rawFrameIds;
 }
 public sealed partial class DemoScene {
  [Serializable] sealed class OutskirtsNpcObservationV27 {
   public string caseId,operation,stage,npc,kind,sourceIdentity,modelClip,actualClip,nativeRawFrameId,sprite,resource,rgbaSha256,npcState,combatState,image;
   public int dungeon,room,encounter,stagedIndex,sourceSpawnIndex,actualFrame,expectedFrame,frameCount,steps,actualDamage,width,height;
   public float modelTime,modelClock,renderClock,npcTime,sourceAnimSpeed,sourceClockSeconds,nativeSourceSampleTime,sourceLoopPeriod,hp,maxHp,x,y,vx,vy,nativeDepth,recoveryTime,deathTime,playerHp,playerX,playerY,spriteFps,spriteDuration,anchorErrorLogical,barProjectionError,debuffProjectionError,barWidth,expectedBarWidth;
   public bool v14PaintedAtlasVerified;public string v14PaintedClip;public bool actualLoop,actualWindowsPlayer,paused,grounded,superArmor,sourceInvincibility,fixedPosition,flipX,flipY,spriteEnabled,barEnabled,sourceAtlasVerified,modelTimeUnchangedAfterCapture,anchorVerified,uiBoundsProjectionChecked;
   public Vector2 metadataAnchorOffset;public Rect spriteRect;
   public Vector3 scale,unprojectedSpriteCenter,projectedSpriteCenter,expectedUnprojectedSpriteCenter,bodyBoundsCenterUnprojected,bodyBoundsMaxUnprojected,actualBar,expectedBar,actualDebuff,expectedDebuff;
  }
  [Serializable] sealed class OutskirtsNpcPlayerReportV27 {
   public string schema="feedback-v27-outskirts-actual-production-entry-player-review-r1",catalogSha256,planSha256,checkedUtc,error="";
   public string scope;public string[] limitations;public bool completed,actualWindowsPlayer,noEnemyAssignments=true;
   public int cases,steps,playerDiagnosticHealthRestorations;public OutskirtsNpcObservationV27[] observations;
  }
  OutskirtsNpcReviewPlanV27 outskirtsNpcPlanV27;OutskirtsNpcPlayerReportV27 outskirtsNpcReportV27;string outskirtsNpcReportPathV27;
  readonly List<OutskirtsNpcObservationV27> outskirtsNpcRowsV27=new List<OutskirtsNpcObservationV27>();
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void StartOutskirtsNpcReviewV27(){
   var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"--feedback-v27-outskirts-npc-review")<0)return;
   if(Array.IndexOf(args,"--feedback-v25-npc-review")>=0||Array.IndexOf(args,"--feedback-v25-npc-combat-review")>=0||Array.IndexOf(args,"--feedback-review")>=0||Array.IndexOf(args,"--smoke")>=0||Array.IndexOf(args,"--feedback-v26-waterway-npc-review")>=0)throw new InvalidOperationException("Independent Outskirts review cannot share another fixture");
   var scene=FindObjectOfType<DemoScene>();if(!scene)throw new InvalidOperationException("Outskirts review needs real DemoScene");scene.StartCoroutine(scene.GuardOutskirtsNpcReviewV27());
  }
  void WriteOutskirtsNpcReportV27(){
   if(outskirtsNpcReportV27==null||string.IsNullOrEmpty(outskirtsNpcReportPathV27))return;
   outskirtsNpcReportV27.observations=outskirtsNpcRowsV27.ToArray();outskirtsNpcReportV27.steps=OutskirtsNpcReviewModelV27.totalSteps;
   outskirtsNpcReportV27.playerDiagnosticHealthRestorations=OutskirtsNpcReviewModelV27.playerHealthRestorations;
   File.WriteAllText(outskirtsNpcReportPathV27,JsonUtility.ToJson(outskirtsNpcReportV27,true));
  }
  IEnumerator GuardOutskirtsNpcReviewV27(){
   var iterator=RunOutskirtsNpcReviewV27();while(true){object value=null;bool more=false;Exception failure=null;
    try{more=iterator.MoveNext();if(more)value=iterator.Current;}catch(Exception error){failure=error;}
    if(failure!=null){if(outskirtsNpcReportV27!=null){outskirtsNpcReportV27.error=failure.ToString();WriteOutskirtsNpcReportV27();}Debug.LogException(failure);if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0)Application.Quit(1);yield break;}
    if(!more)yield break;yield return value;
   }
  }
  void RequireOutskirtsV27(bool condition,string message){if(!condition)throw new InvalidOperationException("Outskirts actual Player: "+message);}
  Enemy EnterOutskirtsReviewCaseV27(OutskirtsNpcReviewCaseV27 c){
   RequireOutskirtsV27(c.dungeon>=0&&c.dungeon<catalog.dungeons.Length,"dungeon index");var route=catalog.dungeons[c.dungeon];
   RequireOutskirtsV27(route.sourceGraph=="Dungeon_Elder_Wally_Castle_Roof_Normal"&&c.room>=0&&c.room<route.count,"source route");var stage=catalog.stages[route.start+c.room];
   RequireOutskirtsV27(stage.id==c.stage&&c.stagedIndex>=0&&c.stagedIndex<stage.spawns.Length,"stage birth index");var source=stage.spawns[c.stagedIndex];
   RequireOutskirtsV27(source.sourceNPC==c.npc&&source.kind==c.kind&&source.sourceSpawnIndex==c.sourceSpawnIndex&&source.encounterIndex==c.encounter,"original source birth identity");
   RestoreNativeProjection();selectedDungeon=previewDungeon=c.dungeon;ConfigureRooms();
   typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.room});
   if(game.HasEncounters&&game.EncounterIndex!=c.encounter)typeof(GameModel).GetMethod("BeginEncounter",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.encounter});
   CancelBossIntroduction();game.paused=true;var enemy=game.enemies[c.stagedIndex];
   RequireOutskirtsV27(enemy.sourceNPC==c.npc&&enemy.kind==c.kind&&enemy.active&&!enemy.dormant&&enemy.hp>0,"actual model birth eligibility");
   OutskirtsNpcReviewModelV27.LocatePlayer(game,enemy,c.mode!="family");
   adventureStarted=true;openMenu="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();CancelBossIntroduction();RememberSimulationState();RenderMenus();hudCanvas.enabled=true;return enemy;
  }
  Vector3 BeforeOutskirtsProjectionV27(Transform target){
   if(!NativeWorldActive)return target.position;foreach(var p in projectionRestore)if(p.transform==target)return p.world;
   throw new InvalidOperationException("Actual renderer missing from production projection snapshot");
  }
  static string OutskirtsCanonicalClipV27(string clip){return clip=="launched"?"launch":clip=="knockdown"?"down":clip=="getup"?"get-up":clip;}
  string ExpectedOutskirtsClipV27(Enemy enemy,Library lib,out float clock){
   string clip=OutskirtsCanonicalClipV27(enemy.clip);if(clip=="jump"&&enemy.vy<0)clip="fall";
   clock=enemy.clock;var profile=Array.Find(CurrentNpcContentSet.npcRules,p=>p.id==enemy.sourceNPC);
   var state=profile==null?null:Array.Find(profile.states,p=>p.id==enemy.npcState);
   // Independent expected consumer uses actual current source profile, not the
   // production selector's compiled42state mapping or the shared model helper.
   if((enemy.state==CombatState.Normal||enemy.state==CombatState.Airborne)&&state!=null&&OutskirtsCanonicalClipV27(enemy.clip)==state.clip)clock=enemy.npcTime;
   string target="source-"+clip;RequireOutskirtsV27(lib.clips.ContainsKey(target),"missing native alias "+target);
   if(clip=="death")clock=Mathf.Clamp01(enemy.deathTime/1.2f)*lib.clips[target].duration;return target;
  }
  void CaptureOutskirtsNpcV27(OutskirtsNpcReviewCaseV27 c,Enemy enemy,string operation,int damage){
   RequireOutskirtsV27(game.paused,"capture must be paused");
   // Locate only the player on an actual support to frame each observed actor,
   // without advancing the model during the screenshot.
   OutskirtsNpcReviewModelV27.LocatePlayer(game,enemy,false);RememberSimulationState();accumulator=0;float before=game.elapsed;game.roomBanner=0;RenderPausedFeedbackNpcV25();
   int index=game.enemies.IndexOf(enemy);RequireOutskirtsV27(index==c.stagedIndex&&index>=0&&index<enemyViews.Count,"real actor/view index");var view=enemyViews[index];var lib=libraries[enemy.kind];
   RequireOutskirtsV27(view.sprite&&view.enabled&&view.gameObject.activeInHierarchy,"actual sprite visibility");
   var original=FeedbackNpcSourceSpriteV25(view.sprite);string prefix=enemy.kind+"/",name=original.name;int slash=name.LastIndexOf('/');
   RequireOutskirtsV27(name.StartsWith(prefix,StringComparison.Ordinal)&&slash>prefix.Length,"actual sprite name");int frame=int.Parse(name.Substring(slash+1));string clip=name.Substring(prefix.Length,slash-prefix.Length);
   float renderClock;string expectedClip=ExpectedOutskirtsClipV27(enemy,lib,out renderClock);RequireOutskirtsV27(clip==expectedClip,"actual clip "+clip+" != "+expectedClip+" at "+operation);
   var info=lib.clips[clip];float poseClock=info.loop&&info.duration>0?Mathf.Repeat(renderClock,info.duration):renderClock;
   int expectedFrame=Math.Min(info.frames.Length-1,Math.Max(0,Mathf.FloorToInt(poseClock*info.fps)));
   RequireOutskirtsV27(frame==expectedFrame&&lib.frames[clip][frame]==original,"actual source frame/clock");var painted=OutskirtsPaintExposureV40.Paint(lib.data,info);var supplemental=SourceMotionInkV45.For(lib.data,painted??info);var displayPaint=supplemental??painted;var displayed=displayPaint??info;var f=OutskirtsPaintExposureV40.Frame(info,displayPaint,frame);
   string canonical=string.IsNullOrEmpty(f.texture)?string.IsNullOrEmpty(displayed.texture)?lib.data.texture:displayed.texture:f.texture;
   var selectedResource=BluePoruInkV45.Resource(enemy.kind,canonical);
   string resource=FeedbackNpcActualTextureResourceV25(view.sprite,original,selectedResource);var texture=Resources.Load<Texture2D>(resource);
   RequireOutskirtsV27(texture&&texture==view.sprite.texture,"actual resource identity");string rgba=texture.isReadable?FeedbackNpcTextureRgbaV25(texture):"";
   var atlas=Array.Find(outskirtsNpcPlanV27.atlases,a=>a.resource==canonical);bool sourceAtlas=atlas!=null;
   if(sourceAtlas)RequireOutskirtsV27(resource==selectedResource&&texture.width==atlas.width&&texture.height==atlas.height&&rgba==(BluePoruInkV45.Pin(resource)??atlas.rgbaSha256),"exact reviewed native atlas decoded RGBA");
   bool paintedAtlas=painted!=null&&canonical==OutskirtsPaintExposureV40.Resource(enemy.kind)&&rgba==(BluePoruInkV45.Pin(resource)??OutskirtsPaintExposureV40.Rgba(enemy.kind));
   bool sourceMotionAtlas=supplemental!=null&&resource==BluePoruInkV45.Resource(enemy.kind,f.texture)&&texture.width>0;
   if(OutskirtsNativeRenderRulesV27.Supported(enemy.sourceNPC))RequireOutskirtsV27(sourceMotionAtlas||(painted==null?sourceAtlas:paintedAtlas),"registered source-clock atlas");
   Vector3 originalCenter=BeforeOutskirtsProjectionV27(view.transform);float scale=lib.data.logicalScale*enemy.visualScale;float paintedRootY=RegisterLegacyBodyV39(enemy,ref scale);
   Vector2 offset=f.hasAnchor?new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f):Vector2.zero;
   if(!f.hasAnchor){Vector2 stored;if(spriteOffsets.TryGetValue(original,out stored))offset=stored;}
   offset.x*=view.flipX?-1:1;Vector3 expectedCenter=actorsRoot.TransformPoint(new Vector3(enemy.x+offset.x*scale,enemy.y+offset.y*scale+paintedRootY,0));
   float anchorError=Vector3.Distance(originalCenter,expectedCenter);RequireOutskirtsV27(anchorError<.001f,"actual metadata anchor/model position "+anchorError);
   bool nativeFamily=OutskirtsNativeRenderRulesV27.Supported(enemy.sourceNPC);
   var bounds=view.bounds;Vector3 delta=originalCenter-view.transform.position;Vector3 center=bounds.center+delta,max=bounds.max+delta;if(painted!=null)max.y=NativeVisibleTopYV28(view)+delta.y;
   var bar=enemyBars[index];var status=enemyDebuffViews[index];Vector3 expectedBar=bar.transform.position,expectedDebuff=status.transform.position;float barError=0,statusError=0,expectedWidth=(enemy.maxHp>400?64:30)*enemy.hp/enemy.maxHp;
   if(nativeFamily){
    float bx=center.x-(enemy.maxHp>400?64:30)*(1-enemy.hp/enemy.maxHp)*.5f;
    expectedBar=ProjectNativePoint(new Vector3(bx,max.y+8,BeforeOutskirtsProjectionV27(bar.transform).z),enemy.nativeDepth);
    expectedDebuff=ProjectNativePoint(new Vector3(center.x,max.y+20,BeforeOutskirtsProjectionV27(status.transform).z),enemy.nativeDepth);
    barError=Vector2.Distance(bar.transform.position,expectedBar);statusError=Vector2.Distance(status.transform.position,expectedDebuff);
    RequireOutskirtsV27(barError<.001f&&statusError<.001f,"actual HP/debuff same-depth projection");
    RequireOutskirtsV27(Mathf.Abs(bar.bounds.size.x-expectedWidth)<.001f,"actual original left-fixed HP width");
   }
   var content=CurrentNpcContentSet;var profile=Array.Find(content.npcRules,p=>p.id==enemy.sourceNPC);var state=profile==null?null:Array.Find(profile.states,s=>s.id==enemy.npcState);float speed=state==null?1:state.sourceAnimSpeed;
   var alias=Array.Find(outskirtsNpcPlanV27.aliases,a=>a.kind==enemy.kind&&a.id==clip);
   RequireOutskirtsV27(alias!=null&&alias.nativeSampleTimes!=null&&alias.nativeSampleTimes.Length==info.frames.Length&&alias.rawFrameIds.Length==info.frames.Length&&alias.nativeAvailableTimes.Length>0,"exact alias source provenance");
   float modelPose=frame/info.fps,period=alias.sourceLoopPeriod;
   int loopReferences=period>0?Mathf.RoundToInt(period*info.fps):0;
   if(period>0)RequireOutskirtsV27(loopReferences>0&&Mathf.Abs(period*info.fps-loopReferences)<.0001f,"source period must be an exact display-reference count");
   float sourceAt=Mathf.Min((period>0?(frame%loopReferences)/info.fps:modelPose)*alias.sourceAnimSpeed,alias.rawMotionDuration),expectedSample=0;
   foreach(float at in alias.nativeAvailableTimes)if(at<=sourceAt+.00001f)expectedSample=Mathf.Max(expectedSample,at);
   RequireOutskirtsV27(Mathf.Abs(alias.nativeSampleTimes[frame]-expectedSample)<.0001f,"native source-time phase, including B/C .6loop");
   bool sourceStateDisplay=state!=null&&state.clip==OutskirtsCanonicalClipV27(enemy.clip)&&(enemy.state==CombatState.Normal||enemy.state==CombatState.Airborne);
   if(sourceStateDisplay)RequireOutskirtsV27(Mathf.Abs(alias.sourceAnimSpeed-state.sourceAnimSpeed)<.0001f&&Mathf.Abs(period-((enemy.sourceNPC=="NUI_PPORU_WILLIAM_B"||enemy.sourceNPC=="NUI_PPORU_WILLIAM_C")&&state.id.EndsWith("_ATTACK_B",StringComparison.Ordinal)?.6f:0))<.0001f,"current source speed/loop period unchanged");
   string image=c.id+"-"+operation;RequireOutskirtsV27(image.IndexOfAny(Path.GetInvalidFileNameChars())<0&&!File.Exists(Path.Combine(captureDir,image+".png"))&&!File.Exists(Path.Combine(captureDir,image+".json")),"preserve capture outputs");
   Capture(image);RequireOutskirtsV27(game.elapsed==before&&game.paused&&File.Exists(Path.Combine(captureDir,image+".png")),"capture time stable/image exists");
   outskirtsNpcRowsV27.Add(new OutskirtsNpcObservationV27{caseId=c.id,operation=operation,stage=game.CurrentRoom.id,npc=enemy.sourceNPC,kind=enemy.kind,sourceIdentity=index==c.stagedIndex?"main-stage.spawns[sourceSpawnIndex]":"unexpected-nonstage-actor",dungeon=c.dungeon,room=game.room,encounter=game.EncounterIndex,stagedIndex=index,sourceSpawnIndex=index==c.stagedIndex?c.sourceSpawnIndex:-1,modelClip=enemy.clip,actualClip=clip,sprite=view.sprite.name,resource=resource,rgbaSha256=rgba,npcState=enemy.npcState,combatState=enemy.state.ToString(),image=image+".png",actualFrame=frame,expectedFrame=expectedFrame,frameCount=info.frames.Length,steps=OutskirtsNpcReviewModelV27.totalSteps,actualDamage=damage,width=texture.width,height=texture.height,modelTime=game.elapsed,modelClock=enemy.clock,renderClock=renderClock,npcTime=enemy.npcTime,sourceAnimSpeed=speed,sourceClockSeconds=(period>0?renderClock%period:renderClock)*alias.sourceAnimSpeed,nativeSourceSampleTime=alias.nativeSampleTimes[frame],sourceLoopPeriod=period,nativeRawFrameId=alias.rawFrameIds[frame],hp=enemy.hp,maxHp=enemy.maxHp,x=enemy.x,y=enemy.y,vx=enemy.vx,vy=enemy.vy,nativeDepth=enemy.nativeDepth,recoveryTime=enemy.recoveryTime,deathTime=enemy.deathTime,playerHp=game.player.hp,playerX=game.player.x,playerY=game.player.y,spriteFps=info.fps,spriteDuration=info.duration,anchorErrorLogical=anchorError,barProjectionError=barError,debuffProjectionError=statusError,barWidth=bar.bounds.size.x,expectedBarWidth=expectedWidth,actualLoop=info.loop,actualWindowsPlayer=!Application.isEditor,paused=game.paused,grounded=game.Grounded(enemy),superArmor=game.EnemyHasSuperArmor(enemy),sourceInvincibility=game.EnemyHasNpcInvincibility(enemy),fixedPosition=game.EnemyPositionFixedV25(enemy),flipX=view.flipX,flipY=view.flipY,spriteEnabled=view.enabled,barEnabled=bar.enabled,sourceAtlasVerified=sourceAtlas,v14PaintedAtlasVerified=paintedAtlas,v14PaintedClip=painted==null?null:painted.id,modelTimeUnchangedAfterCapture=game.elapsed==before,anchorVerified=true,uiBoundsProjectionChecked=nativeFamily,metadataAnchorOffset=offset,spriteRect=view.sprite.rect,scale=view.transform.lossyScale,unprojectedSpriteCenter=originalCenter,projectedSpriteCenter=view.transform.position,expectedUnprojectedSpriteCenter=expectedCenter,bodyBoundsCenterUnprojected=center,bodyBoundsMaxUnprojected=max,actualBar=bar.transform.position,expectedBar=expectedBar,actualDebuff=status.transform.position,expectedDebuff=expectedDebuff});WriteOutskirtsNpcReportV27();
   // All seven Outskirts cases retain the shared helper near-support policy.

  }
  IEnumerator CaptureOutskirtsCaseV27(OutskirtsNpcReviewCaseV27 c,Enemy enemy){
   yield return null;yield return new WaitForEndOfFrame();
   if(c.mode=="family"){
    foreach(var step in OutskirtsNpcReviewModelV27.FamilySteps(enemy.sourceNPC)){
     if(step.id=="public-lethal-start")foreach(string healthCase in new[]{"public-half-hp-ui","public-one-hp-ui"}){
      RestoreNativeProjection();int amount=healthCase=="public-half-hp-ui"?Math.Max(1,(int)(enemy.hp-enemy.maxHp*.5f)):Math.Max(1,(int)enemy.hp-1);
      int dealt=OutskirtsNpcReviewModelV27.Advance(game,enemy,new OutskirtsNpcReviewStepV27{id=healthCase,waitUnprotected=true,damage=amount,ticks=6});RememberSimulationState();accumulator=0;
      RequireOutskirtsV27(healthCase=="public-half-hp-ui"?Mathf.Abs(enemy.hp-enemy.maxHp*.5f)<.001f:Mathf.Abs(enemy.hp-1)<.001f,"real public half/1HP amount");
      yield return null;yield return new WaitForEndOfFrame();CaptureOutskirtsNpcV27(c,enemy,healthCase,dealt);
     }
     RestoreNativeProjection();int damage=OutskirtsNpcReviewModelV27.Advance(game,enemy,step);RememberSimulationState();accumulator=0;
     yield return null;yield return new WaitForEndOfFrame();CaptureOutskirtsNpcV27(c,enemy,step.id,damage);
    }
   }else if(c.mode=="dash-source-clock"){
    string state=enemy.sourceNPC.Substring(4)+"_ATTACK_B";
    RestoreNativeProjection();OutskirtsNpcReviewModelV27.Advance(game,enemy,new OutskirtsNpcReviewStepV27{id="natural-attack-b-entry",targetState=state});RememberSimulationState();accumulator=0;
    yield return new WaitForEndOfFrame();CaptureOutskirtsNpcV27(c,enemy,"natural-attack-b-entry",0);
    foreach(float boundary in new[]{.58f,.6f,.62f,1.18f,1.2f,1.22f,1.78f,1.8f,1.82f,1.95f}){
     if(enemy.sourceNPC=="NUI_PPORU_WILLIAM_C"&&Mathf.Abs(boundary-1.78f)<.001f){
      foreach(float actorBoundary in new[]{1.78f,1.8f,1.82f,1.95f}){
       RestoreNativeProjection();int ticks=0;while(enemy.clock+.0001f<actorBoundary&&ticks++<6000)OutskirtsNpcReviewModelV27.Step(game,enemy);
       RequireOutskirtsV27(enemy.npcState==state&&enemy.clip=="attack-b"&&enemy.state==CombatState.Airborne&&enemy.npcTime<1.7f,"real retained source dash while falling, not forged phase");
       RememberSimulationState();accumulator=0;yield return new WaitForEndOfFrame();CaptureOutskirtsNpcV27(c,enemy,"retained-air-actor-"+actorBoundary.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture),0);
      }
     }
     RestoreNativeProjection();OutskirtsNpcReviewModelV27.Advance(game,enemy,new OutskirtsNpcReviewStepV27{id="source-boundary",targetState=state,minimumClock=boundary});RememberSimulationState();accumulator=0;
     RequireOutskirtsV27(!game.EnemyHasSuperArmor(enemy)&&Mathf.Abs(enemy.npcVelocityX-224)<.001f,"original unarmored passive source dash224");
     yield return new WaitForEndOfFrame();CaptureOutskirtsNpcV27(c,enemy,"source-boundary-"+boundary.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture),0);
    }
    RestoreNativeProjection();OutskirtsNpcReviewModelV27.Advance(game,enemy,new OutskirtsNpcReviewStepV27{id="natural-attack-b-end",targetState=state+"_END"});RememberSimulationState();accumulator=0;
    yield return new WaitForEndOfFrame();CaptureOutskirtsNpcV27(c,enemy,"natural-attack-b-end",0);
   }else throw new InvalidOperationException("Unknown Outskirts review case mode");
  }
  IEnumerator RunOutskirtsNpcReviewV27(){
   Application.runInBackground=true;RequireOutskirtsV27(!Application.isEditor,"actual Windows Player only");RequireOutskirtsV27(UseOffscreenCaptures&&!calibrationRun,"requires real nativeworld offscreen captures");
   string path=Environment.GetEnvironmentVariable("EL_FEEDBACK_V27_OUTSKIRTS_NPC_PLAN");RequireOutskirtsV27(!string.IsNullOrEmpty(path)&&File.Exists(path),"explicit plan file");byte[] bytes=File.ReadAllBytes(path);outskirtsNpcPlanV27=JsonUtility.FromJson<OutskirtsNpcReviewPlanV27>(System.Text.Encoding.UTF8.GetString(bytes));
   string hash=FeedbackNpcHashV25(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes);
   RequireOutskirtsV27(outskirtsNpcPlanV27!=null&&outskirtsNpcPlanV27.schema=="feedback-v27-outskirts-real-model-review-plan-r1"&&outskirtsNpcPlanV27.catalogSha256==hash&&outskirtsNpcPlanV27.sourceDungeon=="Dungeon_Elder_Wally_Castle_Roof_Normal"&&outskirtsNpcPlanV27.expectedBirths==47&&outskirtsNpcPlanV27.expectedImages==113&&outskirtsNpcPlanV27.cases!=null&&outskirtsNpcPlanV27.cases.Length==7&&outskirtsNpcPlanV27.atlases.Length==outskirtsNpcPlanV27.expectedAtlasCount&&outskirtsNpcPlanV27.expectedAtlasCount>=5&&outskirtsNpcPlanV27.aliases!=null&&outskirtsNpcPlanV27.aliases.Length==72,"exact catalog/Outskirts7case/113capture/native atlas+alias plan");
   Directory.CreateDirectory(captureDir);outskirtsNpcReportPathV27=Path.Combine(captureDir,"feedback-v27-outskirts-npcs.json");RequireOutskirtsV27(!File.Exists(outskirtsNpcReportPathV27),"preserve existing report");
   outskirtsNpcReportV27=new OutskirtsNpcPlayerReportV27{catalogSha256=hash,planSha256=FeedbackNpcHashV25(bytes),checkedUtc=DateTime.UtcNow.ToString("o"),actualWindowsPlayer=!Application.isEditor,scope=outskirtsNpcPlanV27.diagnosticScope,limitations=outskirtsNpcPlanV27.limitations};
   monsterArtReview=true;runAutomatedPlaythrough=false;game.paused=true;OutskirtsNpcReviewModelV27.ResetCounters();
   var ids=new HashSet<string>(StringComparer.Ordinal);foreach(var c in outskirtsNpcPlanV27.cases){RequireOutskirtsV27(ids.Add(c.id),"unique case names");var enemy=EnterOutskirtsReviewCaseV27(c);var run=CaptureOutskirtsCaseV27(c,enemy);while(run.MoveNext())yield return run.Current;outskirtsNpcReportV27.cases++;WriteOutskirtsNpcReportV27();}
   RequireOutskirtsV27(outskirtsNpcRowsV27.Count==outskirtsNpcPlanV27.expectedImages,"complete actual113 observations");outskirtsNpcReportV27.completed=true;WriteOutskirtsNpcReportV27();Debug.Log("FEEDBACK_V27_OUTSKIRTS_NPC_REVIEW_COMPLETE images="+outskirtsNpcRowsV27.Count);
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0){yield return new WaitForSecondsRealtime(.5f);Application.Quit();}
  }
 }
}
