using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class WaterwayNpcReviewCaseV26 {
  public string id,mode,stage,npc,kind;public int dungeon,room,stagedIndex,sourceSpawnIndex,encounter;
 }
 [Serializable] public sealed class WaterwayNpcReviewAtlasV26 {
  public string kind,resource,pngSha256,rgbaSha256;public int width,height;
 }
 [Serializable] public sealed class WaterwayNpcReviewPlanV26 {
  public string schema,catalogSha256,sourceDungeon,diagnosticScope;public int expectedBirths;
  public WaterwayNpcReviewCaseV26[] cases;public WaterwayNpcReviewAtlasV26[] atlases;public string[] limitations;
 }
 public sealed partial class DemoScene {
  [Serializable] sealed class WaterwayNpcObservationV26 {
   public string caseId,operation,stage,npc,kind,sourceIdentity,modelClip,actualClip,sprite,resource,rgbaSha256,npcState,combatState,image;
   public int dungeon,room,encounter,stagedIndex,sourceSpawnIndex,actualFrame,expectedFrame,frameCount,steps,actualDamage,width,height;
   public float modelTime,modelClock,renderClock,npcTime,sourceAnimSpeed,sourceClockSeconds,hp,maxHp,x,y,vx,vy,nativeDepth,recoveryTime,deathTime,playerHp,playerX,playerY,spriteFps,spriteDuration,anchorErrorLogical,barProjectionError,debuffProjectionError,barWidth,expectedBarWidth;
   public bool actualLoop,actualWindowsPlayer,paused,grounded,superArmor,sourceInvincibility,fixedPosition,flipX,flipY,spriteEnabled,barEnabled,sourceAtlasVerified,modelTimeUnchangedAfterCapture,anchorVerified,uiBoundsProjectionChecked;
   public Vector2 metadataAnchorOffset;public Rect spriteRect;
   public Vector3 scale,unprojectedSpriteCenter,projectedSpriteCenter,expectedUnprojectedSpriteCenter,bodyBoundsCenterUnprojected,bodyBoundsMaxUnprojected,actualBar,expectedBar,actualDebuff,expectedDebuff;
  }
  [Serializable] sealed class WaterwayNpcPlayerReportV26 {
   public string schema="feedback-v26-waterway-actual-production-entry-player-review-r1",catalogSha256,planSha256,checkedUtc,error="";
   public string scope;public string[] limitations;public bool completed,actualWindowsPlayer,noEnemyAssignments=true;
   public int cases,steps,playerDiagnosticHealthRestorations,naturalNoDropChildren;public WaterwayNpcObservationV26[] observations;
  }
  WaterwayNpcReviewPlanV26 waterwayNpcPlanV26;WaterwayNpcPlayerReportV26 waterwayNpcReportV26;string waterwayNpcReportPathV26;
  readonly List<WaterwayNpcObservationV26> waterwayNpcRowsV26=new List<WaterwayNpcObservationV26>();
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void StartWaterwayNpcReviewV26(){
   var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"--feedback-v26-waterway-npc-review")<0)return;
   if(Array.IndexOf(args,"--feedback-v25-npc-review")>=0||Array.IndexOf(args,"--feedback-v25-npc-combat-review")>=0||Array.IndexOf(args,"--feedback-review")>=0||Array.IndexOf(args,"--smoke")>=0)throw new InvalidOperationException("Independent Waterway review cannot share another fixture");
   var scene=FindObjectOfType<DemoScene>();if(!scene)throw new InvalidOperationException("Waterway review needs real DemoScene");scene.StartCoroutine(scene.GuardWaterwayNpcReviewV26());
  }
  void WriteWaterwayNpcReportV26(){
   if(waterwayNpcReportV26==null||string.IsNullOrEmpty(waterwayNpcReportPathV26))return;
   waterwayNpcReportV26.observations=waterwayNpcRowsV26.ToArray();waterwayNpcReportV26.steps=WaterwayNpcReviewModelV26.totalSteps;
   waterwayNpcReportV26.playerDiagnosticHealthRestorations=WaterwayNpcReviewModelV26.playerHealthRestorations;
   File.WriteAllText(waterwayNpcReportPathV26,JsonUtility.ToJson(waterwayNpcReportV26,true));
  }
  IEnumerator GuardWaterwayNpcReviewV26(){
   var iterator=RunWaterwayNpcReviewV26();while(true){object value=null;bool more=false;Exception failure=null;
    try{more=iterator.MoveNext();if(more)value=iterator.Current;}catch(Exception error){failure=error;}
    if(failure!=null){if(waterwayNpcReportV26!=null){waterwayNpcReportV26.error=failure.ToString();WriteWaterwayNpcReportV26();}Debug.LogException(failure);if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0)Application.Quit(1);yield break;}
    if(!more)yield break;yield return value;
   }
  }
  void RequireWaterwayV26(bool condition,string message){if(!condition)throw new InvalidOperationException("Waterway actual Player: "+message);}
  Enemy EnterWaterwayReviewCaseV26(WaterwayNpcReviewCaseV26 c){
   RequireWaterwayV26(c.dungeon>=0&&c.dungeon<catalog.dungeons.Length,"dungeon index");var route=catalog.dungeons[c.dungeon];
   RequireWaterwayV26(route.sourceGraph=="Dungeon_Elder_Below_Passage_Normal"&&c.room>=0&&c.room<route.count,"source route");var stage=catalog.stages[route.start+c.room];
   RequireWaterwayV26(stage.id==c.stage&&c.stagedIndex>=0&&c.stagedIndex<stage.spawns.Length,"stage birth index");var source=stage.spawns[c.stagedIndex];
   RequireWaterwayV26(source.sourceNPC==c.npc&&source.kind==c.kind&&source.sourceSpawnIndex==c.sourceSpawnIndex&&source.encounterIndex==c.encounter,"original source birth identity");
   RestoreNativeProjection();selectedDungeon=previewDungeon=c.dungeon;ConfigureRooms();
   typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.room});
   if(game.HasEncounters&&game.EncounterIndex!=c.encounter)typeof(GameModel).GetMethod("BeginEncounter",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.encounter});
   CancelBossIntroduction();game.paused=true;var enemy=game.enemies[c.stagedIndex];
   RequireWaterwayV26(enemy.sourceNPC==c.npc&&enemy.kind==c.kind&&enemy.active&&!enemy.dormant&&enemy.hp>0,"actual model birth eligibility");
   WaterwayNpcReviewModelV26.LocatePlayer(game,enemy,c.mode!="family");
   adventureStarted=true;openMenu="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();CancelBossIntroduction();RememberSimulationState();RenderMenus();hudCanvas.enabled=true;return enemy;
  }
  Vector3 BeforeWaterwayProjectionV26(Transform target){
   if(!NativeWorldActive)return target.position;foreach(var p in projectionRestore)if(p.transform==target)return p.world;
   throw new InvalidOperationException("Actual renderer missing from production projection snapshot");
  }
  static string WaterwayCanonicalClipV26(string clip){return clip=="launched"?"launch":clip=="knockdown"?"down":clip=="getup"?"get-up":clip;}
  string ExpectedWaterwayClipV26(Enemy enemy,Library lib,out float clock){
   string clip=WaterwayCanonicalClipV26(enemy.clip);clock=enemy.clock;
   if(clip.StartsWith("source-",StringComparison.Ordinal))return clip;
   if(clip=="jump"&&enemy.vy<0)clip="fall";
   bool three=WaterwayNativeRenderRulesV26.Three(enemy.sourceNPC),mouse=WaterwayNativeRenderRulesV26.Mouse(enemy.sourceNPC);
   if(three||mouse&&(clip=="hurt"||clip=="launch"||clip=="down"||clip=="jump"||clip=="fall"||clip=="death")){
    string native="source-"+clip;RequireWaterwayV26(lib.clips.ContainsKey(native),"missing source alias "+native);
    if(clip=="death")clock=Mathf.Clamp01(enemy.deathTime/1.2f)*lib.clips[native].duration;return native;
   }
   // Mouse has no original StandUp. Its existing explicit Wait recovery clock
   // remains an adapter and is recorded separately rather than called new art.
   if(enemy.state==CombatState.GettingUp&&lib.clips.ContainsKey("get-up"))clock=(1-enemy.recoveryTime/game.EnemyRecoveryDuration(enemy))*lib.clips["get-up"].duration;
   if(!lib.frames.ContainsKey(clip)){if(clip=="fall")clip="launch";}
   RequireWaterwayV26(lib.frames.ContainsKey(clip),"unexpected idle fallback "+clip);return clip;
  }
  void CaptureWaterwayNpcV26(WaterwayNpcReviewCaseV26 c,Enemy enemy,string operation,int damage){
   RequireWaterwayV26(game.paused,"capture must be paused");
   // Locate only the player on an actual support to frame each observed actor,
   // including the second naturally spawned child. No model tick happens here.
   WaterwayNpcReviewModelV26.LocatePlayer(game,enemy,false);RememberSimulationState();accumulator=0;float before=game.elapsed;game.roomBanner=0;RenderPausedFeedbackNpcV25();
   int index=game.enemies.IndexOf(enemy);RequireWaterwayV26(index>=0&&index<enemyViews.Count,"real actor/view index");var view=enemyViews[index];var lib=libraries[enemy.kind];
   RequireWaterwayV26(view.sprite&&view.enabled&&view.gameObject.activeInHierarchy,"actual sprite visibility");
   var original=FeedbackNpcSourceSpriteV25(view.sprite);string prefix=enemy.kind+"/",name=original.name;int slash=name.LastIndexOf('/');
   RequireWaterwayV26(name.StartsWith(prefix,StringComparison.Ordinal)&&slash>prefix.Length,"actual sprite name");int frame=int.Parse(name.Substring(slash+1));string clip=name.Substring(prefix.Length,slash-prefix.Length);
   float renderClock;string expectedClip=ExpectedWaterwayClipV26(enemy,lib,out renderClock);RequireWaterwayV26(clip==expectedClip,"actual clip "+clip+" != "+expectedClip+" at "+operation);
   var info=lib.clips[clip];float poseClock=info.loop&&info.duration>0?Mathf.Repeat(renderClock,info.duration):renderClock;
   int expectedFrame=Math.Min(info.frames.Length-1,Math.Max(0,Mathf.FloorToInt(poseClock*info.fps)));
   RequireWaterwayV26(frame==expectedFrame&&lib.frames[clip][frame]==original,"actual source frame/clock");var f=info.frames[frame];
   string canonical=string.IsNullOrEmpty(f.texture)?string.IsNullOrEmpty(info.texture)?lib.data.texture:info.texture:f.texture;
   string resource=FeedbackNpcActualTextureResourceV25(view.sprite,original,canonical);var texture=Resources.Load<Texture2D>(resource);
   RequireWaterwayV26(texture&&texture==view.sprite.texture,"actual resource identity");string rgba=texture.isReadable?FeedbackNpcTextureRgbaV25(texture):"";
   var atlas=Array.Find(waterwayNpcPlanV26.atlases,a=>a.resource==canonical);bool sourceAtlas=atlas!=null;
   if(sourceAtlas)RequireWaterwayV26(resource==canonical&&texture.width==atlas.width&&texture.height==atlas.height&&rgba==atlas.rgbaSha256,"exact reviewed native atlas decoded RGBA");
   if(WaterwayNativeRenderRulesV26.Three(enemy.sourceNPC)||WaterwayNativeRenderRulesV26.Mouse(enemy.sourceNPC)&&clip.StartsWith("source-",StringComparison.Ordinal))RequireWaterwayV26(sourceAtlas,"new family source alias must use reviewed new atlas");
   Vector3 originalCenter=BeforeWaterwayProjectionV26(view.transform);float scale=lib.data.logicalScale*enemy.visualScale;
   Vector2 offset=f.hasAnchor?new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f):Vector2.zero;
   if(!f.hasAnchor){Vector2 stored;if(spriteOffsets.TryGetValue(original,out stored))offset=stored;}
   offset.x*=view.flipX?-1:1;Vector3 expectedCenter=actorsRoot.TransformPoint(new Vector3(enemy.x+offset.x*scale,enemy.y+offset.y*scale,0));
   float anchorError=Vector3.Distance(originalCenter,expectedCenter);RequireWaterwayV26(anchorError<.001f,"actual metadata anchor/model position "+anchorError);
   bool nativeFamily=WaterwayNativeRenderRulesV26.Three(enemy.sourceNPC)||WaterwayNativeRenderRulesV26.Mouse(enemy.sourceNPC);
   var bounds=view.bounds;Vector3 delta=originalCenter-view.transform.position;Vector3 center=bounds.center+delta,max=bounds.max+delta;
   var bar=enemyBars[index];var status=enemyDebuffViews[index];Vector3 expectedBar=bar.transform.position,expectedDebuff=status.transform.position;float barError=0,statusError=0,expectedWidth=(enemy.maxHp>400?64:30)*enemy.hp/enemy.maxHp;
   if(nativeFamily){
    float bx=center.x-(enemy.maxHp>400?64:30)*(1-enemy.hp/enemy.maxHp)*.5f;
    expectedBar=ProjectNativePoint(new Vector3(bx,max.y+8,BeforeWaterwayProjectionV26(bar.transform).z),enemy.nativeDepth);
    expectedDebuff=ProjectNativePoint(new Vector3(center.x,max.y+20,BeforeWaterwayProjectionV26(status.transform).z),enemy.nativeDepth);
    barError=Vector2.Distance(bar.transform.position,expectedBar);statusError=Vector2.Distance(status.transform.position,expectedDebuff);
    RequireWaterwayV26(barError<.001f&&statusError<.001f,"actual HP/debuff same-depth projection");
    RequireWaterwayV26(Mathf.Abs(bar.bounds.size.x-expectedWidth)<.001f,"actual original left-fixed HP width");
   }
   var content=CurrentNpcContentSet;var profile=Array.Find(content.npcRules,p=>p.id==enemy.sourceNPC);var state=profile==null?null:Array.Find(profile.states,s=>s.id==enemy.npcState);float speed=state==null?1:state.sourceAnimSpeed;
   string image=c.id+"-"+operation;RequireWaterwayV26(image.IndexOfAny(Path.GetInvalidFileNameChars())<0&&!File.Exists(Path.Combine(captureDir,image+".png"))&&!File.Exists(Path.Combine(captureDir,image+".json")),"preserve capture outputs");
   Capture(image);RequireWaterwayV26(game.elapsed==before&&game.paused&&File.Exists(Path.Combine(captureDir,image+".png")),"capture time stable/image exists");
   waterwayNpcRowsV26.Add(new WaterwayNpcObservationV26{caseId=c.id,operation=operation,stage=game.CurrentRoom.id,npc=enemy.sourceNPC,kind=enemy.kind,sourceIdentity=index==c.stagedIndex?"main-stage.spawns[sourceSpawnIndex]":"natural-house-child-source-parent",dungeon=c.dungeon,room=game.room,encounter=game.EncounterIndex,stagedIndex=index,sourceSpawnIndex=index==c.stagedIndex?c.sourceSpawnIndex:-1,modelClip=enemy.clip,actualClip=clip,sprite=view.sprite.name,resource=resource,rgbaSha256=rgba,npcState=enemy.npcState,combatState=enemy.state.ToString(),image=image+".png",actualFrame=frame,expectedFrame=expectedFrame,frameCount=info.frames.Length,steps=WaterwayNpcReviewModelV26.totalSteps,actualDamage=damage,width=texture.width,height=texture.height,modelTime=game.elapsed,modelClock=enemy.clock,renderClock=renderClock,npcTime=enemy.npcTime,sourceAnimSpeed=speed,sourceClockSeconds=enemy.npcTime*speed,hp=enemy.hp,maxHp=enemy.maxHp,x=enemy.x,y=enemy.y,vx=enemy.vx,vy=enemy.vy,nativeDepth=enemy.nativeDepth,recoveryTime=enemy.recoveryTime,deathTime=enemy.deathTime,playerHp=game.player.hp,playerX=game.player.x,playerY=game.player.y,spriteFps=info.fps,spriteDuration=info.duration,anchorErrorLogical=anchorError,barProjectionError=barError,debuffProjectionError=statusError,barWidth=bar.bounds.size.x,expectedBarWidth=expectedWidth,actualLoop=info.loop,actualWindowsPlayer=!Application.isEditor,paused=game.paused,grounded=game.Grounded(enemy),superArmor=game.EnemyHasSuperArmor(enemy),sourceInvincibility=game.EnemyHasNpcInvincibility(enemy),fixedPosition=game.EnemyPositionFixedV25(enemy),flipX=view.flipX,flipY=view.flipY,spriteEnabled=view.enabled,barEnabled=bar.enabled,sourceAtlasVerified=sourceAtlas,modelTimeUnchangedAfterCapture=game.elapsed==before,anchorVerified=true,uiBoundsProjectionChecked=nativeFamily,metadataAnchorOffset=offset,spriteRect=view.sprite.rect,scale=view.transform.lossyScale,unprojectedSpriteCenter=originalCenter,projectedSpriteCenter=view.transform.position,expectedUnprojectedSpriteCenter=expectedCenter,bodyBoundsCenterUnprojected=center,bodyBoundsMaxUnprojected=max,actualBar=bar.transform.position,expectedBar=expectedBar,actualDebuff=status.transform.position,expectedDebuff=expectedDebuff});WriteWaterwayNpcReportV26();
   // Restore the far-support environment stepping policy after the screenshot;
   // this does not advance clocks or alert an NPC between source birth ticks.
   if(c.mode!="family")WaterwayNpcReviewModelV26.LocatePlayer(game,game.enemies[c.stagedIndex],true);
  }
  void WaterwayStepsV26(Enemy enemy,int ticks){for(int i=0;i<ticks;i++)WaterwayNpcReviewModelV26.Step(game,enemy);RememberSimulationState();accumulator=0;}
  void RequireFixedWaterwayV26(Enemy e,Vector2 root,int line){RequireWaterwayV26(game.EnemyPositionFixedV25(e)&&Mathf.Abs(e.x-root.x)<.001f&&Mathf.Abs(e.y-root.y)<.001f&&e.groundLine==line&&e.vx==0&&e.vy==0&&!e.knockdownOnLanding,"actual fixed entity anchor/physics");}
  IEnumerator CaptureWaterwayCaseV26(WaterwayNpcReviewCaseV26 c,Enemy enemy){
   yield return null;yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"production-birth",0);
   if(c.mode=="family"){
    foreach(var step in WaterwayNpcReviewModelV26.FamilySteps(enemy.sourceNPC)){
     if(step.id=="public-lethal-start")foreach(string healthCase in new[]{"public-half-hp-ui","public-one-hp-ui"}){
      RestoreNativeProjection();int amount=healthCase=="public-half-hp-ui"?Math.Max(1,(int)(enemy.hp-enemy.maxHp*.5f)):Math.Max(1,(int)enemy.hp-1);
      int healthDealt=WaterwayNpcReviewModelV26.Advance(game,enemy,new WaterwayNpcReviewStepV26{id=healthCase,waitUnprotected=true,damage=amount});RememberSimulationState();accumulator=0;
      yield return null;yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,healthCase,healthDealt);
     }
     RestoreNativeProjection();int dealt=WaterwayNpcReviewModelV26.Advance(game,enemy,step);RememberSimulationState();accumulator=0;
     yield return null;yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,step.id,dealt);
    }
   }else if(c.mode=="mouse-birth"){
    RequireWaterwayV26(enemy.npcState=="MOUSE_START"&&enemy.clock==0&&game.EnemyHasNpcInvincibility(enemy),"real Mouse protected source START");
    RequireWaterwayV26(game.ApplyHitToEnemy(enemy,1,HitReaction.Launch)==0,"source START rejects public hit");
    WaterwayStepsV26(enemy,39);RequireWaterwayV26(enemy.npcState=="MOUSE_START"&&game.EnemyHasNpcInvincibility(enemy),"source START at39ticks");
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"source-start-tick39",0);
    WaterwayStepsV26(enemy,1);RequireWaterwayV26(enemy.npcState=="MOUSE_WAIT"&&!game.EnemyHasNpcInvincibility(enemy),"original40tick motionend toWAIT");
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"source-start-motion-end",0);
    int dealt=game.ApplyHitToEnemy(enemy,1);RequireWaterwayV26(dealt==1,"source protection ends without invented timer");
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"post-start-public-hurt",dealt);
   }else if(c.mode=="house-population"){
    var root=new Vector2(enemy.x,enemy.y);int line=enemy.groundLine;
    var owners=game.enemies.FindAll(e=>e.sourceNPC=="NUI_MOUSEHOUSE");float ownersHpBefore=0;foreach(var owner in owners)ownersHpBefore+=owner.hp;
    RequireWaterwayV26(enemy.npcState=="MOUSE_HOUSE_START"&&!game.EnemyHasNpcInvincibility(enemy),"House true unprotected1second START");
    WaterwayStepsV26(enemy,59);RequireWaterwayV26(enemy.npcState=="MOUSE_HOUSE_START","House START59ticks");
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"house-start-tick59",0);
    WaterwayStepsV26(enemy,1);RequireWaterwayV26(enemy.npcState=="MOUSE_HOUSE_WAIT","House original60tick motionend");
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"house-separate-wait",0);
    var mouse=game.enemies.Find(e=>e.sourceNPC=="NUI_MOUSE"&&e.hp>0);RequireWaterwayV26(mouse!=null&&game.ApplyHitToEnemy(mouse,100000)>0,"natural population deficiency caused by publichit");
    int wait=0;while(!game.enemies.Exists(e=>e.sourceNPC=="NUI_MOUSE_NODROP")&&wait++<400)WaterwayStepsV26(enemy,1);
    var children=game.enemies.FindAll(e=>e.sourceNPC=="NUI_MOUSE_NODROP");RequireWaterwayV26(children.Count==2,"actual two Houses naturally emit two NODROP");RequireFixedWaterwayV26(enemy,root,line);waterwayNpcReportV26.naturalNoDropChildren+=children.Count;
    float ownersHpAfter=0;foreach(var owner in owners)ownersHpAfter+=owner.hp;RequireWaterwayV26(Mathf.Abs(ownersHpBefore-ownersHpAfter-500*children.Count)<.001f,"actual original500HP cost per emitted child");
    foreach(var child in children)RequireWaterwayV26(owners.Exists(owner=>Mathf.Abs(owner.spawnX-child.spawnX)<.001f),"natural child uses original source House root");
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"house-real-emission",0);
    foreach(var child in children){RequireWaterwayV26(child.npcState=="MOUSE_NODROP_START"&&child.npcTime==0&&game.EnemyHasNpcInvincibility(child)&&game.ApplyHitToEnemy(child,1)==0,"natural NODROP source START");yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,child,"child-"+game.enemies.IndexOf(child)+"-source-birth",0);}
    WaterwayStepsV26(enemy,39);foreach(var child in children){RequireWaterwayV26(child.npcState=="MOUSE_NODROP_START"&&game.EnemyHasNpcInvincibility(child),"natural child39ticks protected");yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,child,"child-"+game.enemies.IndexOf(child)+"-tick39",0);}
    WaterwayStepsV26(enemy,1);foreach(var child in children){RequireWaterwayV26(child.npcState=="MOUSE_NODROP_WAIT"&&!game.EnemyHasNpcInvincibility(child),"natural child40tick motionend");yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,child,"child-"+game.enemies.IndexOf(child)+"-wait",0);}
    int hurt=game.ApplyHitToEnemy(enemy,1,HitReaction.Launch);RequireWaterwayV26(hurt==1,"House public launch consumesHP");RequireFixedWaterwayV26(enemy,root,line);WaterwayStepsV26(enemy,1);
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"house-fixed-source-damage",hurt);
   }else if(c.mode=="steam-cycle"){
    var root=new Vector2(enemy.x,enemy.y);int line=enemy.groundLine;RequireWaterwayV26(game.ApplyHitToEnemy(enemy,100000,HitReaction.Launch)==0,"Steam source no-body protection");
    int wait=0;while(enemy.npcState!="STEAM_TRAP_ATTACK"&&wait++<500)WaterwayStepsV26(enemy,1);RequireWaterwayV26(enemy.npcState=="STEAM_TRAP_ATTACK","source Steam attacks fromrealclock");RequireFixedWaterwayV26(enemy,root,line);
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"steam-source-attack",0);
    WaterwayStepsV26(enemy,8);RequireFixedWaterwayV26(enemy,root,line);yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"steam-source-attack-progress",0);
    wait=0;while(enemy.npcState!="STEAM_TRAP_WAIT"&&wait++<500)WaterwayStepsV26(enemy,1);RequireWaterwayV26(enemy.npcState=="STEAM_TRAP_WAIT","source Steam returnsWAIT");RequireFixedWaterwayV26(enemy,root,line);
    yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"steam-return-wait",0);
   }else if(c.mode=="box-fixed"||c.mode=="gate-unlock"){
    WaterwayStepsV26(enemy,61);var root=new Vector2(enemy.x,enemy.y);int line=enemy.groundLine;
    if(c.mode=="gate-unlock"){
     RequireWaterwayV26(game.ApplyHitToEnemy(enemy,1)==0,"locked gate rejects public hit while actualcombat remains");
     foreach(var other in new List<Enemy>(game.enemies))if(other.active&&other.encounterIndex==game.EncounterIndex&&!other.dormant&&other.countsForClear&&!other.isGate&&!other.passive&&other.hp>0)RequireWaterwayV26(game.ApplyHitToEnemy(other,100000)>0,"public damage clears real source combat body");
     WaterwayStepsV26(enemy,60);yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"real-combat-clear-gate-unlock",0);
    }
    int dealt=game.ApplyHitToEnemy(enemy,1,HitReaction.Launch);RequireWaterwayV26(dealt==1,"fixed body public launch losesHP");RequireFixedWaterwayV26(enemy,root,line);
    WaterwayStepsV26(enemy,1);yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"public-launch-fixed-damage",dealt);
    WaterwayStepsV26(enemy,60);RequireFixedWaterwayV26(enemy,root,line);yield return new WaitForEndOfFrame();CaptureWaterwayNpcV26(c,enemy,"source-anchor-after-reaction",0);
   }else throw new InvalidOperationException("Unknown Waterway review case mode");
  }
  IEnumerator RunWaterwayNpcReviewV26(){
   Application.runInBackground=true;RequireWaterwayV26(UseOffscreenCaptures&&!calibrationRun,"requires real nativeworld offscreen captures");
   string path=Environment.GetEnvironmentVariable("EL_FEEDBACK_V26_WATERWAY_NPC_PLAN");RequireWaterwayV26(!string.IsNullOrEmpty(path)&&File.Exists(path),"explicit plan file");byte[] bytes=File.ReadAllBytes(path);waterwayNpcPlanV26=JsonUtility.FromJson<WaterwayNpcReviewPlanV26>(System.Text.Encoding.UTF8.GetString(bytes));
   string hash=FeedbackNpcHashV25(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes);
   RequireWaterwayV26(waterwayNpcPlanV26!=null&&waterwayNpcPlanV26.schema=="feedback-v26-waterway-real-model-review-plan-r1"&&waterwayNpcPlanV26.catalogSha256==hash&&waterwayNpcPlanV26.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal"&&waterwayNpcPlanV26.expectedBirths==44&&waterwayNpcPlanV26.cases!=null&&waterwayNpcPlanV26.cases.Length==9&&waterwayNpcPlanV26.atlases.Length==4,"exact catalog/Waterway9case/4atlas plan");
   Directory.CreateDirectory(captureDir);waterwayNpcReportPathV26=Path.Combine(captureDir,"feedback-v26-waterway-npcs.json");RequireWaterwayV26(!File.Exists(waterwayNpcReportPathV26),"preserve existing report");
   waterwayNpcReportV26=new WaterwayNpcPlayerReportV26{catalogSha256=hash,planSha256=FeedbackNpcHashV25(bytes),checkedUtc=DateTime.UtcNow.ToString("o"),actualWindowsPlayer=!Application.isEditor,scope=waterwayNpcPlanV26.diagnosticScope,limitations=waterwayNpcPlanV26.limitations};
   monsterArtReview=true;runAutomatedPlaythrough=false;game.paused=true;WaterwayNpcReviewModelV26.ResetCounters();
   var ids=new HashSet<string>(StringComparer.Ordinal);foreach(var c in waterwayNpcPlanV26.cases){RequireWaterwayV26(ids.Add(c.id),"unique case names");var enemy=EnterWaterwayReviewCaseV26(c);var run=CaptureWaterwayCaseV26(c,enemy);while(run.MoveNext())yield return run.Current;waterwayNpcReportV26.cases++;WriteWaterwayNpcReportV26();}
   waterwayNpcReportV26.completed=true;WriteWaterwayNpcReportV26();Debug.Log("FEEDBACK_V26_WATERWAY_NPC_REVIEW_COMPLETE images="+waterwayNpcRowsV26.Count);
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0){yield return new WaitForSecondsRealtime(.5f);Application.Quit();}
  }
 }
}
