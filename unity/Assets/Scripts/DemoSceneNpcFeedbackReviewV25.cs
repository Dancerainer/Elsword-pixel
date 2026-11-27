using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class FeedbackNpcObservationV25 {
   public string caseId,sampleId,stage,npc,kind,phase,sourceState,modelClip,actualClip,spriteName,textureName,textureResource,textureRuntimeContentHash,image,productionState,textureTopDownRgbaSha256,sourceTextureResource;
   public bool usedAdmittedRedrawCopy,textureRgbaReadable,usedSourceFlightPresentation,usedSourceIdlePresentation,usedSourceWalkPresentation,diagnosticRoomBannerSuppressed;
   public string presentationClipMapping;public First3BoneObservationV25 rootRegistration;
   public float roomBannerBeforeDiagnosticSuppression,roomBannerAfterDiagnosticSuppression;
   public int dungeon,room,encounter,stagedIndex,sourceSpawnIndex,support,groundLine,actualFrame,frameCount,textureWidth,textureHeight,textureInstanceId,stepsSinceLastCapture,totalFixtureSteps;
   public float actualRenderClock,modelTime,phaseElapsed,sourceClipClock,revealProgress,enemyClock,npcClock,x,y,nativeDepth,vx,vy,hp,supportY,playerX,playerY,spriteFps,spriteDuration;
   public bool grounded,hiddenHurtbox,presentationVisibleWindow,superArmor,fixedPosition,spriteEnabled,spriteGameObjectActive,flipX,flipY,actualLoop,usedIdleFallback,sameModelTimeAfterCapture;
   public Vector3 spriteLocalPosition,spriteWorldPosition,spriteScale;public Rect spriteRect;public Vector2 spriteAnchorOffset;
  }
  [Serializable] sealed class FeedbackNpcReviewReportV25 {
   public string schema="feedback-v25-first3-actual-birth-windows-mechanism-fixture-native-combat-r4",checkedUtc,catalogSha256,planSha256,error;
   public bool completed,actualWindowsPlayer;public int actualBatBirths;
   public string scope="Diagnostic actual main Ruins/Cave births and source encounters. Player support is explicitly located; model Step alone advances phases. Every Capture clears only diagnostic roomBanner UI; it does not advance model time or change enemy flags/HP. Completed idle uses native source-flight/source-idle; completed walk uses native source-walk, with the actual final render clock recorded separately from model time. Not a natural route/clear, art approval or full-NPC-mechanism proof.";
   public FeedbackNpcObservationV25[] observations;
  }
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void StartFeedbackNpcReviewV25(){
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--feedback-v25-npc-review")<0)return;
   var scene=FindObjectOfType<DemoScene>();if(!scene)throw new InvalidOperationException("NPC review needs real DemoScene");
   scene.StartCoroutine(scene.RunFeedbackNpcReviewGuardedV25());
  }
  static string FeedbackNpcHashV25(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  readonly Dictionary<Texture2D,string> feedbackNpcTextureHashesV25=new Dictionary<Texture2D,string>();
  string FeedbackNpcTextureRgbaV25(Texture2D texture){
   string cached;if(feedbackNpcTextureHashesV25.TryGetValue(texture,out cached))return cached;
   var pixels=texture.GetPixels32();var bytes=new byte[pixels.Length*4];int at=0;
   for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){
    var p=pixels[y*texture.width+x];bytes[at++]=p.r;bytes[at++]=p.g;bytes[at++]=p.b;bytes[at++]=p.a;
   }
   cached=FeedbackNpcHashV25(bytes);feedbackNpcTextureHashesV25.Add(texture,cached);return cached;
  }
  Sprite FeedbackNpcSourceSpriteV25(Sprite actual){
   var redraw=FindObjectOfType<MonsterTextureRedrawV22>();
   if(!redraw)return OriginalRubenForestSpriteV40(actual);
   var source=typeof(MonsterTextureRedrawV22).GetMethod("SourceSprite",BindingFlags.Instance|BindingFlags.NonPublic);
   if(source==null)throw new InvalidOperationException("Missing admitted redraw source lookup");
   return OriginalRubenForestSpriteV40((Sprite)source.Invoke(redraw,new object[]{actual}));
  }
  string FeedbackNpcActualTextureResourceV25(Sprite actual,Sprite source,string originalResource){
   var original=Resources.Load<Texture2D>(originalResource);
   if(actual&&rubenForestByPaintV40.TryGetValue(actual,out var paint)){if(paint.source!=source||!original||original!=source.texture||Resources.Load<Texture2D>(paint.resource)!=actual.texture)throw new InvalidOperationException("Registered painted source clock/texture");return paint.resource;}
   if(!original||original!=source.texture)throw new InvalidOperationException("Canonical sprite resource differs");
   if(actual==source){if(actual.texture!=original)throw new InvalidOperationException("Direct sprite texture differs");return originalResource;}
   if(actual.name!=source.name||actual.rect!=source.rect||actual.pivot!=source.pivot)
    throw new InvalidOperationException("Redraw copy changed source sprite identity/rect/pivot");
   var text=Resources.Load<TextAsset>(MonsterTextureRedrawV22.ManifestResource);
   var manifest=text?JsonUtility.FromJson<MonsterTextureRedrawManifestV22>(text.text):null;
   if(manifest==null||manifest.schema!="monster-texture-redraw-v22")throw new InvalidOperationException("Missing exact redraw manifest");
   string selected=null,sourceHash=FeedbackNpcTextureRgbaV25(original),actualHash=FeedbackNpcTextureRgbaV25(actual.texture);
   foreach(var entry in manifest.entries){
    if(!entry.approved||entry.sourceRgbaSha256!=sourceHash||entry.targetRgbaSha256!=actualHash
     ||entry.width!=actual.texture.width||entry.height!=actual.texture.height
     ||Resources.Load<Texture2D>(entry.targetResource)!=actual.texture)continue;
    if(selected!=null&&selected!=entry.targetResource)throw new InvalidOperationException("Ambiguous admitted redraw resource");
    selected=entry.targetResource;
   }
   if(selected==null)throw new InvalidOperationException("Actual sprite is not an exact admitted redraw copy");return selected;
  }
  void RenderPausedFeedbackNpcV25(){
   RestoreNativeProjection();RenderActors();RenderEffects();RenderHud();RenderCombatGeometry();RenderCombatPresentation();FinishNativeProjection();
  }
  FeedbackNpcObservationV25 ObserveFeedbackNpcV25(FeedbackNpcCaseV25 c,FeedbackNpcSampleV25 sample,Enemy enemy,int steps,int total,string image){
   int index=game.enemies.IndexOf(enemy);if(index!=c.stagedIndex||index>=enemyViews.Count)throw new InvalidOperationException("Actual actor/view index drift");
   var view=enemyViews[index];Library lib;if(!libraries.TryGetValue(enemy.kind,out lib)||!view.sprite)throw new InvalidOperationException("Actual library/sprite absent");
   var sourceSprite=FeedbackNpcSourceSpriteV25(view.sprite);
   string clip=null;int frameIndex=-1;foreach(var pair in lib.frames){int found=Array.IndexOf(pair.Value,sourceSprite);if(found>=0){clip=pair.Key;frameIndex=found;break;}}
   if(clip==null)throw new InvalidOperationException("Actual sprite is outside the bound library");var info=lib.clips[clip];var f=info.frames[frameIndex];
   string resource=string.IsNullOrEmpty(f.texture)?(string.IsNullOrEmpty(info.texture)?lib.data.texture:info.texture):f.texture;
   string phase=game.EnemyInitialPosePhaseV25(enemy);
   bool completedBat=phase=="complete"&&(enemy.sourceNPC=="NUI_BAT_A"||enemy.sourceNPC=="NUI_BAT_B");
   bool sourceFlightPresentation=completedBat&&enemy.clip=="idle"&&clip=="source-flight";
   bool sourceWalkPresentation=phase=="complete"&&enemy.clip=="walk"&&clip=="source-walk";
   if(completedBat&&!sourceFlightPresentation&&!sourceWalkPresentation)throw new InvalidOperationException("Completed Bat must consume its exact native idle or walk display");
   bool completedTree=phase=="complete"&&enemy.sourceNPC=="NUI_TREE_KNIGHT";
   bool sourceIdlePresentation=completedTree&&enemy.clip=="idle"&&clip=="source-idle";
   if(completedTree&&!sourceIdlePresentation&&!sourceWalkPresentation)throw new InvalidOperationException("Completed Tree must consume its exact native idle or walk display");
   string contentHash=""; // The decoded RGBA SHA below is available in the Windows Player.
   var result=new FeedbackNpcObservationV25{caseId=c.id,sampleId=sample.id,stage=game.CurrentRoom.id,npc=enemy.sourceNPC,kind=enemy.kind,phase=game.EnemyInitialPosePhaseV25(enemy),sourceState=game.EnemyInitialPoseSourceStateV25(enemy),modelClip=enemy.clip,actualClip=clip,spriteName=view.sprite.name,textureName=view.sprite.texture.name,textureResource=resource,textureRuntimeContentHash=contentHash,textureWidth=view.sprite.texture.width,textureHeight=view.sprite.texture.height,textureInstanceId=view.sprite.texture.GetInstanceID(),image=image,productionState=Path.ChangeExtension(image,".json"),dungeon=c.dungeon,room=game.room,encounter=game.EncounterIndex,stagedIndex=index,sourceSpawnIndex=c.sourceSpawnIndex,support=game.EnemyHangingSupportV25(enemy),groundLine=enemy.groundLine,actualFrame=frameIndex,frameCount=lib.frames[clip].Length,stepsSinceLastCapture=steps,totalFixtureSteps=total,actualRenderClock=First3ActualRenderClockV25(enemy),modelTime=game.elapsed,phaseElapsed=game.EnemyInitialPoseElapsedV25(enemy),sourceClipClock=game.EnemyInitialPoseClipClockV25(enemy),revealProgress=game.EnemyInitialRevealProgressV25(enemy),presentationVisibleWindow=game.EnemyInitialPresentationVisibleV25(enemy),enemyClock=enemy.clock,npcClock=enemy.npcTime,x=enemy.x,y=enemy.y,nativeDepth=enemy.nativeDepth,vx=enemy.vx,vy=enemy.vy,hp=enemy.hp,supportY=game.EnemyInitialSupportYV25(enemy),playerX=game.player.x,playerY=game.player.y,spriteFps=info.fps,spriteDuration=info.duration,grounded=game.Grounded(enemy),hiddenHurtbox=game.EnemyInitialHurtboxHiddenV25(enemy),superArmor=game.EnemyHasSuperArmor(enemy),fixedPosition=game.EnemyPositionFixedV25(enemy),spriteEnabled=view.enabled,spriteGameObjectActive=view.gameObject.activeInHierarchy,flipX=view.flipX,flipY=view.flipY,actualLoop=info.loop,usedIdleFallback=clip!=enemy.clip&&!sourceFlightPresentation&&!sourceIdlePresentation&&!sourceWalkPresentation,spriteLocalPosition=view.transform.localPosition,spriteWorldPosition=view.transform.position,spriteScale=view.transform.localScale,spriteRect=view.sprite.rect,spriteAnchorOffset=SpriteOffset(view)};
   result.usedSourceFlightPresentation=sourceFlightPresentation;result.usedSourceIdlePresentation=sourceIdlePresentation;result.usedSourceWalkPresentation=sourceWalkPresentation;
   result.presentationClipMapping=sourceFlightPresentation?"completed-bat-idle-to-source-flight-v25":sourceIdlePresentation?"completed-tree-idle-to-source-idle-v25":sourceWalkPresentation?"completed-first3-walk-to-source-walk-v25":"identity";
   result.rootRegistration=First3ActualBoneObservationV25(enemy);
   if(phase=="complete"&&(result.rootRegistration==null||result.rootRegistration.registeredBipErrorLogical>.001f))throw new InvalidOperationException("Actual native settled registration differs from model reference");
   result.sourceTextureResource=resource;result.usedAdmittedRedrawCopy=view.sprite!=sourceSprite;
   result.textureResource=FeedbackNpcActualTextureResourceV25(view.sprite,sourceSprite,resource);
   var actualTexture=Resources.Load<Texture2D>(result.textureResource);
   if(!actualTexture||actualTexture!=view.sprite.texture)throw new InvalidOperationException("Actual sprite texture does not match exact resource binding");
   Vector2 sourceOffset;if(spriteOffsets.TryGetValue(sourceSprite,out sourceOffset))result.spriteAnchorOffset=new Vector2(sourceOffset.x*(view.flipX?-1:1),sourceOffset.y);
   result.textureRgbaReadable=actualTexture.isReadable;
   if(!result.textureRgbaReadable&&result.textureResource.StartsWith("Pixel/feedback-v25/",StringComparison.Ordinal))
    throw new InvalidOperationException("New native atlas must support exact runtime RGBA verification");
   result.textureTopDownRgbaSha256=result.textureRgbaReadable?FeedbackNpcTextureRgbaV25(actualTexture):"";return result;
  }
  IEnumerator RunFeedbackNpcReviewGuardedV25(){
   var execution=RunFeedbackNpcReviewV25();
   while(true){object next=null;bool more=false;Exception failure=null;
    try{more=execution.MoveNext();if(more)next=execution.Current;}catch(Exception error){failure=error;}
    if(failure!=null){
     Directory.CreateDirectory(captureDir);string file=Path.Combine(captureDir,"feedback-v25-npcs-failure.json");
     if(!File.Exists(file))File.WriteAllText(file,JsonUtility.ToJson(new FeedbackNpcReviewReportV25{checkedUtc=DateTime.UtcNow.ToString("o"),completed=false,actualWindowsPlayer=!Application.isEditor,error=failure.ToString()},true));
     Debug.LogException(failure);if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0)Application.Quit(1);yield break;
    }
    if(!more)yield break;yield return next;
   }
  }
  IEnumerator RunFeedbackNpcReviewV25(){
   Application.runInBackground=true;
   string planPath=Environment.GetEnvironmentVariable("EL_FEEDBACK_V25_NPC_PLAN");if(string.IsNullOrEmpty(planPath)||!File.Exists(planPath))throw new InvalidOperationException("Explicit NPC review plan required");
   byte[] bytes=File.ReadAllBytes(planPath);var plan=JsonUtility.FromJson<FeedbackNpcPlanV25>(System.Text.Encoding.UTF8.GetString(bytes));
   string catalogHash=FeedbackNpcHashV25(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes);
   if(plan==null||plan.schema!="feedback-v25-first3-actual-birth-plan-r1"||plan.catalogSha256!=catalogHash||plan.cases==null||plan.cases.Length!=9)throw new InvalidOperationException("Exact main catalog/first3 nine-case plan required");
   if(!UseOffscreenCaptures)throw new InvalidOperationException("Diagnostic fixture requires --offscreen-captures");
   Directory.CreateDirectory(captureDir);string reportPath=Path.Combine(captureDir,"feedback-v25-npcs.json");if(File.Exists(reportPath))throw new InvalidOperationException("Preserve existing NPC review");
   var ids=new HashSet<string>(StringComparer.Ordinal);var actualBirths=new HashSet<string>(StringComparer.Ordinal);var outputNames=new HashSet<string>(StringComparer.Ordinal);int bats=0;
   foreach(var c in plan.cases){
    if(c==null||string.IsNullOrEmpty(c.id)||!ids.Add(c.id)||c.dungeon<0||c.dungeon>=catalog.dungeons.Length||c.room<0||c.room>=catalog.dungeons[c.dungeon].count||catalog.stages[catalog.dungeons[c.dungeon].start+c.room].id!=c.stage||c.samples==null||c.samples.Length==0)throw new InvalidOperationException("Actual route/stage plan mismatch");
    if(!actualBirths.Add(c.stage+"/"+c.stagedIndex))throw new InvalidOperationException("Duplicate actual birth");
    if(c.npc=="NUI_TREE_KNIGHT"?c.dungeon!=1||c.stage!="ruins-1":c.dungeon!=4||!c.stage.StartsWith("cave-",StringComparison.Ordinal))throw new InvalidOperationException("Outside current first3 scope");
    var stage=catalog.stages[catalog.dungeons[c.dungeon].start+c.room];if(c.stagedIndex<0||c.stagedIndex>=stage.spawns.Length||stage.spawns[c.stagedIndex].sourceNPC!=c.npc)throw new InvalidOperationException("Actual birth identity changed");
    if(c.npc=="NUI_BAT_A"||c.npc=="NUI_BAT_B")bats++;
    foreach(var sample in c.samples){string name=c.id+"-"+sample.id;if(!outputNames.Add(name)||name.IndexOfAny(Path.GetInvalidFileNameChars())>=0||File.Exists(Path.Combine(captureDir,name+".png"))||File.Exists(Path.Combine(captureDir,name+".json")))throw new InvalidOperationException("Preserve planned outputs/invalid sample id");}
   }
   if(bats!=8)throw new InvalidOperationException("All eight real main-catalog bat births required");
   var report=new FeedbackNpcReviewReportV25{checkedUtc=DateTime.UtcNow.ToString("o"),catalogSha256=catalogHash,planSha256=FeedbackNpcHashV25(bytes),actualWindowsPlayer=!Application.isEditor,actualBatBirths=bats};var rows=new List<FeedbackNpcObservationV25>();
   monsterArtReview=true;runAutomatedPlaythrough=false;game.paused=true;
   foreach(var c in plan.cases){
    selectedDungeon=previewDungeon=c.dungeon;ConfigureRooms();typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.room});
    if(game.HasEncounters&&game.EncounterIndex!=c.encounter)typeof(GameModel).GetMethod("BeginEncounter",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{c.encounter});
    CancelBossIntroduction();game.paused=true;var enemy=FeedbackNpcModelStepsV25.Subject(game,c);FeedbackNpcModelStepsV25.LocatePlayer(game,c.far);
    adventureStarted=true;openMenu="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();CancelBossIntroduction();RememberSimulationState();RenderMenus();hudCanvas.enabled=true;int total=0;
    foreach(var sample in c.samples){
     RestoreNativeProjection();int steps=FeedbackNpcModelStepsV25.Advance(game,enemy,c,sample);total+=steps;RememberSimulationState();accumulator=0;
     for(int settle=0;settle<3;settle++)yield return null;
     yield return new WaitForEndOfFrame();
     // Diagnostic UI only. Do not alter phase clocks, enemy flags, HP or physics.
     float bannerBefore=game.roomBanner,bannerModelTime=game.elapsed;game.roomBanner=0;
     RenderPausedFeedbackNpcV25();string name=c.id+"-"+sample.id;
     var observation=ObserveFeedbackNpcV25(c,sample,enemy,steps,total,name+".png");
     observation.roomBannerBeforeDiagnosticSuppression=bannerBefore;observation.roomBannerAfterDiagnosticSuppression=game.roomBanner;
     observation.diagnosticRoomBannerSuppressed=game.roomBanner==0&&game.elapsed==bannerModelTime&&game.paused;
     if(!observation.diagnosticRoomBannerSuppressed)throw new InvalidOperationException("Diagnostic roomBanner suppression drifted model time/UI");
     float before=game.elapsed;Capture(name);
     observation.sameModelTimeAfterCapture=game.elapsed==before&&game.paused&&game.roomBanner==0;if(!observation.sameModelTimeAfterCapture)throw new InvalidOperationException("Capture drifted simulation time");
     if(!File.Exists(Path.Combine(captureDir,name+".png")))throw new InvalidOperationException("Offscreen capture did not produce image");rows.Add(observation);
     report.observations=rows.ToArray();File.WriteAllText(reportPath,JsonUtility.ToJson(report,true));
    }
   }
   report.completed=true;report.observations=rows.ToArray();File.WriteAllText(reportPath,JsonUtility.ToJson(report,true));Debug.Log("FEEDBACK_V25_NPC_REVIEW_COMPLETE images="+rows.Count);
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0){yield return new WaitForSecondsRealtime(.5f);Application.Quit();}
  }
 }
}
