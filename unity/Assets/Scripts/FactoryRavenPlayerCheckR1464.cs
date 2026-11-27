#if ROOT_FACTORY_RAVEN_PLAYER_R1464 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElswordPixel {
 // CLI-only observation/pilot. The product view owns all draw ACKs. No field
 // assignments to model HP, clocks, state, AI, room, clear flags or pause exist.
 public sealed class FactoryRavenPlayerCheckR1464 : MonoBehaviour {
  const string Flag="--factory-raven-check-r1464",OutputFlag="--factory-raven-output-r1464",ModeFlag="--factory-raven-mode-r1464";
  const string Drafts=System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\";
  const double WallBudget=1500;const float ModelBudget=1200;const double NoProgress=90;
  const long TraceBudget=64L*1024*1024;
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
  [Serializable] public sealed class ActionRow {public int frame,pointer;public float time;public string action,edge;public bool delivered;}
  [Serializable] public sealed class ActorRow {public int index;public string npc,kind,state,combatState,clip,sprite;public float hp,maxHp,x,y,clock,npcTime;public bool knockdownOnLanding;public bool active,ai,countsForClear,drawnThisFrame,postRenderMatched,depthBound,inViewport;public Bounds bounds;}
  [Serializable] public sealed class FrameRow {
   public int frame,room,encounter,phaseCount,releaseCount,protoLive,protoPending,selectionFrame,drawFrame,observedFrame;
   public string world,roomId,sourceIntroPhase,reason;public float time,heroHp,heroX,heroY,sourceTime,disabledElapsed,worldHalf,nativeHalf,nativeFov;
   public bool paused,victory,worldPostRender,sourceFocusAccepted,sourceObservationSubmitted,flag0,flag1,sceneDepthReady;
   public List<ActorRow> actors=new List<ActorRow>();
  }
  [Serializable] public sealed class DrawRow {public int frame,index;public string npc,state,combatState,sprite;public float sourceTime,hp;public Bounds bounds;public bool postRenderMatched,depthBound,inViewport;}
  static readonly string[] ReactionNPCsR1469={"NUI_WALLY_9TH_TYPE2","NUI_NASOD_HEALER_BLADER","NUI_NASOD_GUARD"};
  [Serializable] public sealed class FactoryReactionCoverage {public string npc;public int frames,missingFrames,observedUncoveredFrames;public List<string> observedStates=new List<string>(),actuallyDrawnStates=new List<string>(),notObservedStates=new List<string>();}
  [Serializable] public sealed class FactoryReactionRow {public int frame,room,encounter,index;public float time,hp,clock,alpha;public string npc,kind,combatState,npcState,clip,sprite,outcome,airborneBasis;public bool knockdownOnLanding;public bool expectedOnscreen,bodyExists,rendererEnabled,hasSprite,actualDraw,worldPost,actualBoundsOverlap;public Vector3 expectedViewportMin,expectedViewportMax;}
  [Serializable] public sealed class ImageRow {public string label,path,sha256;public long bytes;public int frame,width,height;}
  [Serializable] public sealed class PostCloneRecoveryRowR1470 {public string reason,npc,kind,npcState,combatState,sprite;public int frame,index,surface,releaseCount;public float time,hp,y,surfaceY,npcTime,physicalClock;public bool ai,grounded,bodyDraw,worldPost,inViewport,depthBound;}
  [Serializable] public sealed class Report {
   public string schema="FACTORY_RAVEN_ACTUAL_DRAW_AND_ORDINARY_INPUT_PHASE_PLAYER_CHECK_R1464",mode,failure="",cleanupFailure="",graphics;
   public bool passed,ordinaryTouchMenuEntry,explicitOriginalBossRoomSetup,noDirectModelFieldWrites=true,noSyntheticFocusAck=true,normalSceneUpdateOwnsAllStepCalls=true;
   public bool sourceOpeningActualDraw,sourceOpeningReleasedOnce,cloneCycleRecovered,eachCreatedProtoActuallyDrawn,sceneDepthBindingObserved,ordinaryCameraScaleStable=true,returnedOriginalMain;
   public bool fullNaturalFactoryClear=false,pixelOcclusionOrArtApproved=false,actualOSExitMustBeCheckedByOuterOwner=true;
   public bool fixtureBossLanded,fixtureBossAIResumed,fixtureBossPostLandingTouchHit,fixtureBossPostLandingDraw,fixtureBossRecoveryComplete;
   public int fixtureBossAttackEdgesHeldForAI;public int fixtureBossIndex=-1,fixtureBossLandingFrame=-1,fixtureBossLandingSurface=-1,fixtureBossTouchAttackFrame=-1,fixtureBossHitFrame=-1,fixtureBossDrawFrame=-1;
   public float fixtureBossLandingHp,fixtureBossAfterHitHp,fixtureBossLandingTime;
   public List<PostCloneRecoveryRowR1470> fixtureBossRecoveryEvidence=new List<PostCloneRecoveryRowR1470>();
   public List<FactoryReactionCoverage> factoryReactionCoverage=new List<FactoryReactionCoverage>();public int factoryReactionFrames,factoryReactionMissingFrames,factoryReactionObservedUncoveredFrames;public List<FactoryReactionRow> factoryReactionEvidence=new List<FactoryReactionRow>();public List<string> factoryReactionObservedUncovered=new List<string>();public bool factoryReactionVisibilityComplete,factoryReactionCoverageLimitedToObservedStates=true;public List<string> factoryReactionsObserved=new List<string>(),factoryReactionsActuallyDrawn=new List<string>(),factoryReactionMissing=new List<string>(),factoryReactionsNotObserved=new List<string>();public int ownedFocusClears,ownedFocusReleasedControls;public int focusPauseResumes,protoInstances,protoInstancesDrawn,npcAudioMissingBefore,npcAudioMissingAfter;public float modelBefore,modelAfter;
   public List<string> errors=new List<string>(),visitedNodes=new List<string>();public List<ActionRow> actions=new List<ActionRow>();public List<DrawRow> draws=new List<DrawRow>();public List<ImageRow> images=new List<ImageRow>();
  }
  [Serializable] public sealed class Terminal {public string schema="FACTORY_RAVEN_PLAYER_NORMAL_QUIT_TERMINAL_R1464",report,sha256,failure;public long bytes;public int requestedExitCode;public bool passed,normalQuitRequested,actualOSExitMustBeCheckedByOuterOwner=true;}
  sealed class SeenDraw {internal int frame;internal SpriteRenderer body;internal string state,sprite;internal CombatState combatState;internal float sourceTime,hp;internal Bounds bounds;}
  DemoScene scene;GameModel game,originalGame;Camera originalCamera;string output,mode;Report report=new Report();
  static bool attached;static FactoryRavenPlayerCheckR1464 focusOwner;bool finished,quitting,backgroundSaved,backgroundBefore,traceFull;double started,lastProgress;float nextSample,nextAttack,nextJump,lastX,lastHp;int worldFrame=-1,lastRoom=-1,lastEncounter=-1,lastPhase=-1;
  FileStream trace;IEnumerator activeFlow;FactoryRavenIntroTicketV21 originalTicket;
  readonly Dictionary<string,TouchControl> held=new Dictionary<string,TouchControl>();readonly Dictionary<string,int> pointers=new Dictionary<string,int>();int nextPointer=-146400;
  readonly Dictionary<Enemy,SeenDraw> seen=new Dictionary<Enemy,SeenDraw>();readonly HashSet<Enemy> protoCreated=new HashSet<Enemy>(),protoDrawn=new HashSet<Enemy>();
  readonly HashSet<string> factoryReactionEvidenceKeys=new HashSet<string>();readonly HashSet<string> captures=new HashSet<string>();readonly Dictionary<string,float[]> scales=new Dictionary<string,float[]>();
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int flags=0,outputs=0,modes=0;string path=null,mode=null;
   for(int i=0;i<args.Length;i++){if(args[i]==Flag)flags++;else if(args[i]==OutputFlag){outputs++;if(i+1<args.Length)path=args[++i];}else if(args[i]==ModeFlag){modes++;if(i+1<args.Length)mode=args[++i];}}
   if(flags==0)return;
   try{
    Need(flags==1&&outputs==1&&modes==1&&!attached,"One explicit diagnostic/output/mode required");Need(mode=="boss-room-fixture"||mode=="natural","Explicit fixture or natural mode required");
    Need(Application.platform==RuntimePlatform.WindowsPlayer&&!Application.isEditor&&!Application.isBatchMode,"Visible standalone Windows Player required");
    Need(Path.IsPathRooted(path)&&string.Equals(Path.GetPathRoot(path),@"C:\",StringComparison.OrdinalIgnoreCase),"Absolute C output required");
    foreach(string part in path.Replace('\\','/').Split('/'))Need(part!=".."&&part!="."&&!part.Equals("project",StringComparison.OrdinalIgnoreCase)&&!part.Equals("cache",StringComparison.OrdinalIgnoreCase),"No project/cache/relative output");
    path=Path.GetFullPath(path);Need(path.StartsWith(Drafts,StringComparison.OrdinalIgnoreCase),"Owned C drafts output only");
    for(var p=new DirectoryInfo(path);p!=null;p=p.Parent)if(p.Exists)Need((p.Attributes&FileAttributes.ReparsePoint)==0,"No reparse output parent");Need(!Directory.Exists(path)&&!File.Exists(path),"Fresh output required");Directory.CreateDirectory(path);
    Need(SceneManager.GetActiveScene().path=="Assets/Scenes/TreeOfEl.unity","Original Main scene required");attached=true;
    var owner=new GameObject("Explicit Factory Raven actual draw/phase check R1464").AddComponent<FactoryRavenPlayerCheckR1464>();owner.output=path;owner.mode=mode;focusOwner=owner;
   }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
  }
  // Registered only by the validated explicit CLI Attach path. The product
  // clears input first; release real held controls so the next Pilot sends Down.
  public static bool OwnedFocusPolicyActiveR1468(DemoScene candidate){var owner=focusOwner;return candidate&&owner&&owner.isActiveAndEnabled&&!owner.finished&&!owner.quitting&&ReferenceEquals(owner.scene,candidate)&&owner.gameObject.scene==candidate.gameObject.scene;}
  public static void ObserveOwnedInputClearR1468(DemoScene candidate){if(!OwnedFocusPolicyActiveR1468(candidate))return;var owner=focusOwner;owner.report.ownedFocusClears++;owner.report.ownedFocusReleasedControls+=owner.held.Count;owner.ReleaseControls();}
  static void Need(bool value,string text){if(!value)throw new InvalidOperationException("Factory Raven Player: "+text);}
  static object Field(object owner,string name){Need(owner!=null,"Missing owner "+name);var f=owner.GetType().GetField(name,Fields);Need(f!=null,"Missing field "+name);return f.GetValue(owner);}
  static string Digest(byte[] data){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(data)).Replace("-","").ToLowerInvariant();}
  void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)report.errors.Add(message+"\n"+stack);}
  void Rendered(Camera camera){if(scene&&camera==(Camera)Field(scene,"worldCamera"))worldFrame=Time.frameCount;}
  void Check(){Need(!finished,"Observer already finished");Need(report.errors.Count==0,"Runtime error logged");Need(Time.realtimeSinceStartupAsDouble-started<WallBudget,"Bounded wall deadline exceeded");if(game!=null){Need(game.elapsed-report.modelBefore<ModelBudget,"Bounded model deadline exceeded");if(scene&&scene.FactoryProductSessionActiveR1179)Need(game.player.hp>0,"Ordinary player died; no revive or retry");}}
  void RequireReflectionContract(){
   foreach(string name in new[]{"game","worldCamera","enemyViews","openMenu","mapDungeons","mapVisible","mapPage","mapSelected","nativeScene","nativeCamera","nativeSceneDepthReadyR1461","nativeSceneOcclusionBindingsR1461","factoryRavenSelectionFrameV21","factoryRavenDrawFrameV21","factoryRavenObservedFrameV21","factoryRavenObservationSubmittedV21","factoryProductSessionR1179"})Need(scene.GetType().GetField(name,Fields)!=null,"Required current scene field absent: "+name);
   Need(scene.GetType().GetMethod("RenderEnemyPosition",Fields,null,new[]{typeof(Enemy)},null)!=null&&scene.GetType().GetMethod("ProjectNativePoint",Fields,null,new[]{typeof(Vector3),typeof(float)},null)!=null&&typeof(GameModel).GetMethod("NpcProfile",Fields,null,new[]{typeof(Enemy)},null)!=null,"Read-only actor projection/profile methods absent");
    Need(scene.GetType().GetField("worldRoot",Fields)!=null,"Read-only world root field absent");
    Need(typeof(GameModel).GetField("potionCooldown",Fields)!=null&&typeof(GameModel).GetMethod("EnterRoom",Fields,null,new[]{typeof(int)},null)!=null,"Required original model pause-free input/fixture interface absent");
   Need(scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic)!=null,"Original map page constant absent");
  }
  IEnumerator Start(){
   started=lastProgress=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;backgroundSaved=true;Application.runInBackground=true;report.mode=mode;report.graphics=SystemInfo.graphicsDeviceType.ToString();
   Application.logMessageReceived+=Log;Camera.onPostRender+=Rendered;trace=new FileStream(Path.Combine(output,"ordinary-input-and-Raven-observations.ndjson"),FileMode.CreateNew,FileAccess.Write,FileShare.Read);
   activeFlow=Flow();Exception failure=null;while(!finished){bool more=false;object next=null;try{more=activeFlow.MoveNext();if(more)next=activeFlow.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return next;}
   if(finished)yield break;try{(activeFlow as IDisposable)?.Dispose();}catch(Exception e){failure=failure??e;}activeFlow=null;
   if(failure!=null)report.failure=failure.ToString();report.passed=failure==null&&report.errors.Count==0;Complete();
  }
  void Update(){if(finished||started==0)return;if(Time.realtimeSinceStartupAsDouble-started>=WallBudget){report.failure="Bounded wall deadline exceeded";report.passed=false;Complete();}}
  TouchControl Find(string action,bool required=true){TouchControl found=null;foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&ReferenceEquals(c.owner,scene)&&c.action==action&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy){Need(!found,"Duplicate active real control "+action);found=c;}if(required)Need(found,"No active real control "+action);return found;}
  PointerEventData Pointer(string action){int id;if(!pointers.TryGetValue(action,out id)){id=--nextPointer;pointers.Add(action,id);}return new PointerEventData(EventSystem.current){pointerId=id,button=PointerEventData.InputButton.Left};}
  void Edge(TouchControl control,string action,bool down){Need(scene&&EventSystem.current&&control,"Original control/EventSystem required");var data=Pointer(action);data.position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position);bool delivered=down?ExecuteEvents.Execute(control.gameObject,data,ExecuteEvents.pointerDownHandler):ExecuteEvents.Execute(control.gameObject,data,ExecuteEvents.pointerUpHandler);report.actions.Add(new ActionRow{action=action,edge=down?"down":"up",pointer=data.pointerId,frame=Time.frameCount,time=game==null?0:game.elapsed,delivered=delivered});Need(delivered,"Original pointer event not delivered "+action);}
  void Press(string action){if(action=="attack"&&mode=="boss-room-fixture"&&postCloneBossR1470!=null&&!report.fixtureBossAIResumed){report.fixtureBossAttackEdgesHeldForAI++;return;}var c=Find(action);var image=c.GetComponent<Image>();Need(image&&image.raycastTarget,"Raycastable original control required");try{Edge(c,action,true);}finally{if(c)Edge(c,action,false);}}
  void Move(int direction){string wanted=direction>0?"right":direction<0?"left":null;foreach(var key in new List<string>(held.Keys))if(key!=wanted){var c=held[key];held.Remove(key);if(c)Edge(c,key,false);}if(wanted!=null&&!held.ContainsKey(wanted)){var c=Find(wanted);Edge(c,wanted,true);held.Add(wanted,c);}}
  void ReleaseControls(){foreach(var key in new List<string>(held.Keys)){var c=held[key];held.Remove(key);try{if(c)Edge(c,key,false);}catch(Exception e){report.cleanupFailure+="\n"+e.Message;}}}
  void ResumeOneObservedPause(){if(!game.paused)return;Need(report.focusPauseResumes==0,"Repeated pause; no field rewrite");ReleaseControls();Press("pause");report.focusPauseResumes++;}
  IEnumerator SelectFactory(){
   Need((string)Field(scene,"openMenu")=="dungeons","Ordinary Main dungeon menu required");var rows=(IList)Field(scene,"mapDungeons");int index=-1;
   for(int i=0;i<rows.Count;i++)if((string)Field(rows[i],"id")=="altera-nasod-factory"){Need(index<0,"Duplicate Factory route");index=i;}Need(index>=0,"Factory menu route absent");
   Press("region:"+(string)Field(rows[index],"region"));yield return new WaitForEndOfFrame();var visible=(IList)Field(scene,"mapVisible");int at=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==index)at=i;Need(at>=0,"Factory not visible in its region");
   int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue(),page=at/size,steps=0;
   while((int)Field(scene,"mapPage")!=page){Need(++steps<=rows.Count,"Map page navigation failed");int before=(int)Field(scene,"mapPage");Press("dungeon-page:"+(before<page?"1":"-1"));yield return new WaitForEndOfFrame();Need((int)Field(scene,"mapPage")!=before,"Map page did not advance");}
   Press("dungeon-card:"+(at%size));yield return new WaitForEndOfFrame();Need((int)Field(scene,"mapSelected")==index,"Wrong route selected");Press("dungeon-enter");report.ordinaryTouchMenuEntry=true;
  }
  void LateUpdate(){if(finished||!scene||game==null||!scene.FactoryProductSessionActiveR1179)return;var bodies=(IList)Field(scene,"enemyViews");for(int i=0;i<game.enemies.Count&&i<bodies.Count;i++){var e=game.enemies[i];if(e.sourceNPC!="NUI_RAVEN_CLONE"&&e.sourceNPC!="NUI_RAVEN_CLONE_PROTO"&&Array.IndexOf(ReactionNPCsR1469,e.sourceNPC)<0)continue;var body=bodies[i]as SpriteRenderer;if(!body)continue;
       if(e.sourceNPC=="NUI_RAVEN_CLONE" && scene.VisitFactoryRavenPartsV35(e, part=>{
         var draw=part.GetComponent<FactoryRavenDiagnosticDrawR1464>();
         if(!draw)draw=part.gameObject.AddComponent<FactoryRavenDiagnosticDrawR1464>();draw.Bind(this,e,part);}))continue;
       var witness=body.GetComponent<FactoryRavenDiagnosticDrawR1464>();if(!witness)witness=body.gameObject.AddComponent<FactoryRavenDiagnosticDrawR1464>();witness.Bind(this,e,body);if(e.sourceNPC=="NUI_RAVEN_CLONE_PROTO")protoCreated.Add(e);}}
  internal void Draw(Enemy actor,SpriteRenderer body,Camera camera,int frame){if(finished||game==null||!scene||camera!=(Camera)Field(scene,"worldCamera")||!game.enemies.Contains(actor)||!body.enabled||!body.gameObject.activeInHierarchy||!body.sprite||body.color.a<=0)return;seen[actor]=new SeenDraw{frame=frame,body=body,state=actor.npcState,combatState=actor.state,sprite=body.sprite.name,sourceTime=actor.npcTime,hp=actor.hp,bounds=body.bounds};}
  bool DepthBound(SpriteRenderer body){if(!(bool)Field(scene,"nativeSceneDepthReadyR1461"))return false;foreach(var entry in(IEnumerable)Field(scene,"nativeSceneOcclusionBindingsR1461"))if(ReferenceEquals(Field(entry,"renderer"),body)){var block=new MaterialPropertyBlock();body.GetPropertyBlock(block);return block.GetFloat("_NativeSceneClipR1461")>.5f;}return false;}
  FrameRow Observe(string reason){
   var camera=(Camera)Field(scene,"worldCamera");var native=(Camera)Field(scene,"nativeCamera");var status=game.ReadFactoryRavenClonePhaseV21();var ticket=game.CurrentFactoryRavenIntroTicketV21();if(ticket!=null)originalTicket=ticket;var intro=originalTicket==null?null:game.FactoryRavenIntroStatusV21(originalTicket);
   var row=new FrameRow{reason=reason,frame=Time.frameCount,time=game.elapsed,room=game.room,encounter=game.EncounterIndex,roomId=game.CurrentRoom.id,world=(string)Field(Field(scene,"nativeScene"),"id"),heroHp=game.player.hp,heroX=game.player.x,heroY=game.player.y,paused=game.paused,victory=game.victory,worldPostRender=worldFrame==Time.frameCount,phaseCount=status.phaseCount,releaseCount=status.releaseCount,protoLive=status.live,protoPending=status.pending,flag0=status.flag0,flag1=status.flag1,disabledElapsed=status.disabledElapsed,worldHalf=camera.orthographicSize,nativeHalf=native.orthographicSize,nativeFov=native.fieldOfView,sceneDepthReady=(bool)Field(scene,"nativeSceneDepthReadyR1461"),selectionFrame=(int)Field(scene,"factoryRavenSelectionFrameV21"),drawFrame=(int)Field(scene,"factoryRavenDrawFrameV21"),observedFrame=(int)Field(scene,"factoryRavenObservedFrameV21"),sourceObservationSubmitted=(bool)Field(scene,"factoryRavenObservationSubmittedV21"),sourceFocusAccepted=intro!=null&&intro.focusAccepted,sourceTime=intro==null?0:intro.sourceTime,sourceIntroPhase=intro==null?"none":intro.phase.ToString()};
   string node=row.room+":"+row.encounter;if(!report.visitedNodes.Contains(node))report.visitedNodes.Add(node);
   if(row.drawFrame==row.frame&&row.observedFrame==row.frame&&row.selectionFrame==row.frame&&row.worldPostRender)report.sourceOpeningActualDraw=true;
   if(intro!=null&&intro.phase==CoreStopIntroPhaseV21.Released){Need(intro.releaseCount==1&&intro.focusAccepted&&intro.sourceTime==3.8f,"Original exact3.8 single START release required");report.sourceOpeningReleasedOnce=true;}
   var safe= new Rect(0,0,1,1);for(int i=0;i<game.enemies.Count;i++){var e=game.enemies[i];if(e.sourceNPC!="NUI_RAVEN_CLONE"&&e.sourceNPC!="NUI_RAVEN_CLONE_PROTO"&&Array.IndexOf(ReactionNPCsR1469,e.sourceNPC)<0&&e.sourceNPC!="NUI_NASOD_MAKER"&&e.sourceNPC!="NUI_FACTORY_PRESS"&&e.sourceNPC!="NUI_FACTORY_PRESS3"&&e.sourceNPC!="NUI_NASOD_SHERIFF")continue;SeenDraw draw;seen.TryGetValue(e,out draw);bool drawn=draw!=null&&draw.frame==row.frame,post=drawn&&row.worldPostRender,depth=drawn&&draw.body&&DepthBound(draw.body),inside=false;if(drawn){var low=camera.WorldToViewportPoint(draw.bounds.min);var high=camera.WorldToViewportPoint(draw.bounds.max);inside=low.z>=camera.nearClipPlane&&high.z<=camera.farClipPlane&&low.x>=safe.xMin&&high.x<=safe.xMax&&low.y>=safe.yMin&&high.y<=safe.yMax;if(e.sourceNPC=="NUI_RAVEN_CLONE_PROTO"&&post&&depth&&inside)protoDrawn.Add(e);}
    row.actors.Add(new ActorRow{index=i,npc=e.sourceNPC,kind=e.kind,state=e.npcState,combatState=e.state.ToString(),clip=e.clip,knockdownOnLanding=e.knockdownOnLanding,hp=e.hp,maxHp=e.maxHp,x=e.x,y=e.y,clock=e.clock,npcTime=e.npcTime,active=e.active,ai=e.aiEnabled,countsForClear=e.countsForClear,drawnThisFrame=drawn,postRenderMatched=post,depthBound=depth,inViewport=inside,sprite=drawn?draw.sprite:null,bounds=drawn?draw.bounds:new Bounds()});
    if(post&&(e.sourceNPC=="NUI_RAVEN_CLONE_PROTO"||report.draws.Count<240))report.draws.Add(new DrawRow{frame=row.frame,index=i,npc=e.sourceNPC,state=draw.state,combatState=draw.combatState.ToString(),sprite=draw.sprite,sourceTime=draw.sourceTime,hp=draw.hp,bounds=draw.bounds,postRenderMatched=true,depthBound=depth,inViewport=inside});report.sceneDepthBindingObserved|=post&&depth;
   }
   string scaleKey=(intro!=null&&(intro.phase==CoreStopIntroPhaseV21.AwaitingFocus||intro.phase==CoreStopIntroPhaseV21.PlayingSourceStart)?"intro":"ordinary")+"-"+game.room;float[] first;if(scales.TryGetValue(scaleKey,out first)){if(Math.Abs(row.worldHalf-first[0])>.001f||Math.Abs(row.nativeHalf-first[1])>.001f||Math.Abs(row.nativeFov-first[2])>.001f)report.ordinaryCameraScaleStable=false;}else scales.Add(scaleKey,new[]{row.worldHalf,row.nativeHalf,row.nativeFov});
   var bytes=new UTF8Encoding(false,true).GetBytes(JsonUtility.ToJson(row)+"\n");if(trace.Length+bytes.Length>TraceBudget){traceFull=true;throw new InvalidOperationException("Original64MiB diagnostic trace capacity exceeded");}trace.Write(bytes,0,bytes.Length);return row;
  }
  void Capture(string label){if(!captures.Add(label))return;Need(worldFrame==Time.frameCount,"Actual world post-render required for capture");Texture2D image=null;try{image=ScreenCapture.CaptureScreenshotAsTexture();Need(image&&image.width==Screen.width&&image.height==Screen.height,"Actual full screenshot required");var data=image.EncodeToPNG();var path=Path.Combine(output,label+".png");using(var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){stream.Write(data,0,data.Length);}report.images.Add(new ImageRow{label=label,path=path,sha256=Digest(data),bytes=data.LongLength,frame=Time.frameCount,width=image.width,height=image.height});}finally{if(image)Destroy(image);}}
  // Read-only physical source-body viewport estimate, projected through the same
  // native camera mapping as production. A five-percent inset excludes edges.
  bool ExpectedFactoryReactionOnscreenR1469(Enemy actor,Camera camera,out Vector3 lo,out Vector3 hi){
   var root=(Transform)Field(scene,"worldRoot");var p=(Vector2)scene.GetType().GetMethod("RenderEnemyPosition",Fields).Invoke(scene,new object[]{actor});var profile=(NpcDefinition)typeof(GameModel).GetMethod("NpcProfile",Fields).Invoke(game,new object[]{actor});Need(profile!=null&&profile.id==actor.sourceNPC&&Array.IndexOf(ReactionNPCsR1469,actor.sourceNPC)>=0&&profile.bodyWidth>0&&profile.bodyHeight>0,"Expected matching genuine Factory source body");
   float half=profile.bodyWidth*actor.visualScale*.5f,height=profile.bodyHeight*actor.visualScale;lo=new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);hi=new Vector3(float.NegativeInfinity,float.NegativeInfinity,float.NegativeInfinity);
   foreach(float x in new[]{p.x-half,p.x+half})foreach(float y in new[]{p.y,p.y+height}){var world=root.TransformPoint(new Vector3(x,y,0));var projected=(Vector3)scene.GetType().GetMethod("ProjectNativePoint",Fields).Invoke(scene,new object[]{world,actor.nativeDepth});var v=camera.WorldToViewportPoint(projected);lo=Vector3.Min(lo,v);hi=Vector3.Max(hi,v);}
   return lo.z>=camera.nearClipPlane&&hi.z<=camera.farClipPlane&&lo.x>=.05f&&hi.x<=.95f&&lo.y>=.05f&&hi.y<=.95f;
  }
  FactoryReactionCoverage FactoryReactionCoverageR1469(string npc){var row=report.factoryReactionCoverage.Find(q=>q.npc==npc);if(row==null){row=new FactoryReactionCoverage{npc=npc};report.factoryReactionCoverage.Add(row);}return row;}
  void ObserveFactoryReactionDrawR1469(){
   if(mode!="natural")return;var camera=(Camera)Field(scene,"worldCamera");var bodies=(IList)Field(scene,"enemyViews");
   for(int i=0;i<game.enemies.Count;i++){
    var actor=game.enemies[i];bool reactionAir=actor.state==CombatState.Airborne&&string.IsNullOrEmpty(actor.npcState)&&(actor.clip=="launched"||actor.knockdownOnLanding);if(Array.IndexOf(ReactionNPCsR1469,actor.sourceNPC)<0||actor.hp<=0||!actor.active||actor.dormant||(actor.state!=CombatState.Hurt&&actor.state!=CombatState.Knockdown&&actor.state!=CombatState.GettingUp&&!reactionAir))continue;
    string state=actor.state.ToString(),identity=actor.sourceNPC+"/"+state;var coverage=FactoryReactionCoverageR1469(actor.sourceNPC);coverage.frames++;report.factoryReactionFrames++;if(!coverage.observedStates.Contains(state))coverage.observedStates.Add(state);if(!report.factoryReactionsObserved.Contains(identity))report.factoryReactionsObserved.Add(identity);
    SeenDraw draw;seen.TryGetValue(actor,out draw);bool drawn=draw!=null&&draw.frame==Time.frameCount&&draw.combatState==actor.state&&draw.hp>0,post=drawn&&worldFrame==Time.frameCount,visible=false;
    if(post){var low=camera.WorldToViewportPoint(draw.bounds.min);var high=camera.WorldToViewportPoint(draw.bounds.max);visible=low.z>=camera.nearClipPlane&&high.z<=camera.farClipPlane&&high.x>0&&low.x<1&&high.y>0&&low.y<1;}
    Vector3 lo,hi;bool expected=ExpectedFactoryReactionOnscreenR1469(actor,camera,out lo,out hi);string outcome=post&&visible?"actual-body-draw":expected?"missing-expected-onscreen-body":"observed-uncovered-offscreen-or-frustum-edge";
    if(post&&visible){if(!coverage.actuallyDrawnStates.Contains(state))coverage.actuallyDrawnStates.Add(state);if(!report.factoryReactionsActuallyDrawn.Contains(identity))report.factoryReactionsActuallyDrawn.Add(identity);Capture("reaction-"+actor.sourceNPC+"-"+state);}
    else if(expected){coverage.missingFrames++;report.factoryReactionMissingFrames++;}else{coverage.observedUncoveredFrames++;report.factoryReactionObservedUncoveredFrames++;}
    string key=game.room+":"+game.EncounterIndex+":"+i+":"+identity+":"+outcome;
    if(factoryReactionEvidenceKeys.Add(key)){var renderer=i<bodies.Count?bodies[i]as SpriteRenderer:null;report.factoryReactionEvidence.Add(new FactoryReactionRow{npc=actor.sourceNPC,kind=actor.kind,clip=actor.clip,knockdownOnLanding=actor.knockdownOnLanding,airborneBasis=reactionAir?(actor.knockdownOnLanding?"empty-source-state/knockdownOnLanding":"empty-source-state/launched-clip"):"not-airborne-reaction",frame=Time.frameCount,room=game.room,encounter=game.EncounterIndex,index=i,time=game.elapsed,hp=actor.hp,clock=actor.clock,combatState=state,npcState=actor.npcState,expectedOnscreen=expected,expectedViewportMin=lo,expectedViewportMax=hi,bodyExists=renderer,rendererEnabled=renderer&&renderer.enabled,hasSprite=renderer&&renderer.sprite,sprite=renderer&&renderer.sprite?renderer.sprite.name:"none",alpha=renderer?renderer.color.a:-1,actualDraw=drawn,worldPost=worldFrame==Time.frameCount,actualBoundsOverlap=visible,outcome=outcome});if(outcome=="missing-expected-onscreen-body")report.factoryReactionMissing.Add("frame="+Time.frameCount+" "+key);else if(outcome=="observed-uncovered-offscreen-or-frustum-edge")report.factoryReactionObservedUncovered.Add("frame="+Time.frameCount+" "+key);}
    // Native scene depth can occlude pixels even after OnWillRenderObject. This
    // witness proves draw submission, not pixel visibility or art approval.
   }
  }
  Enemy postCloneBossR1470;string postCloneAIStateR1470;float postCloneAITimeR1470,postCloneTouchTimeR1470;bool postCloneHaveTouchR1470;
  void RecordPostCloneRecoveryR1470(string reason,GroundSurface floor,bool grounded,SeenDraw draw=null,bool visible=false){
   var boss=postCloneBossR1470;report.fixtureBossRecoveryEvidence.Add(new PostCloneRecoveryRowR1470{reason=reason,frame=Time.frameCount,time=game.elapsed,index=game.enemies.IndexOf(boss),npc=boss.sourceNPC,kind=boss.kind,npcState=boss.npcState,combatState=boss.state.ToString(),hp=boss.hp,y=boss.y,surface=boss.groundLine,surfaceY=floor==null?0f:floor.Y(boss.x),npcTime=boss.npcTime,physicalClock=boss.clock,ai=boss.aiEnabled,grounded=grounded,releaseCount=game.ReadFactoryRavenClonePhaseV21().releaseCount,bodyDraw=draw!=null,worldPost=worldFrame==Time.frameCount,inViewport=visible,depthBound=draw!=null&&draw.body&&DepthBound(draw.body),sprite=draw==null?null:draw.sprite});
  }
  void ObservePostCloneCombatR1470(){
   if(mode!="boss-room-fixture"||report.fixtureBossRecoveryComplete)return;
   var phase=game.ReadFactoryRavenClonePhaseV21();
   if(postCloneBossR1470==null){
    if(phase.releaseCount<1||phase.live!=0||phase.pending!=0)return;
    Need(phase.actor!=null&&phase.actor.sourceNPC=="NUI_RAVEN_CLONE"&&phase.actor.hp>0,"Original released Raven actor must remain living");postCloneBossR1470=phase.actor;report.fixtureBossIndex=game.enemies.IndexOf(phase.actor);Need(report.fixtureBossIndex>=0,"Original released Raven belongs to this actual room");RecordPostCloneRecoveryR1470("original-clone-release-observed",null,false);
   }
   var boss=postCloneBossR1470;Need(ReferenceEquals(phase.actor,boss)&&game.enemies.Contains(boss)&&boss.active&&!boss.dormant&&boss.hp>0,"Same original living Raven required through landing and renewed combat");
   GroundSurface floor=null;foreach(var s in game.CurrentRoom.surfaces)if(s.id==boss.groundLine&&boss.x>=s.x1-.05f&&boss.x<=s.x2+.05f){floor=s;break;}
   bool grounded=floor!=null&&Math.Abs(boss.y-floor.Y(boss.x))<.2f&&Math.Abs(boss.vy)<.1f&&(bool)typeof(GameModel).GetMethod("Grounded",Fields).Invoke(game,new object[]{boss});
   if(!report.fixtureBossLanded&&grounded){report.fixtureBossLanded=true;report.fixtureBossLandingFrame=Time.frameCount;report.fixtureBossLandingSurface=floor.id;report.fixtureBossLandingHp=boss.hp;report.fixtureBossLandingTime=game.elapsed;postCloneAIStateR1470=boss.npcState;postCloneAITimeR1470=boss.npcTime;RecordPostCloneRecoveryR1470("same-boss-on-original-surface",floor,true);}
   if(!report.fixtureBossLanded)return;
   if(!report.fixtureBossAIResumed&&boss.aiEnabled&&!string.IsNullOrEmpty(boss.npcState)&&boss.npcState!="RAVEN_CLONE_DISABLED"&&(boss.npcState!=postCloneAIStateR1470||boss.npcTime>postCloneAITimeR1470+.000001f)){report.fixtureBossAIResumed=true;RecordPostCloneRecoveryR1470("original-source-AI-state-progress-after-landing",floor,grounded);}
   if(!postCloneHaveTouchR1470){foreach(var action in report.actions)if(action.delivered&&action.action=="attack"&&action.edge=="down"&&action.frame>report.fixtureBossLandingFrame){postCloneHaveTouchR1470=true;postCloneTouchTimeR1470=action.time;report.fixtureBossTouchAttackFrame=action.frame;RecordPostCloneRecoveryR1470("new-ordinary-TouchControl-attack-after-landing",floor,grounded);break;}}
   if(!report.fixtureBossPostLandingTouchHit&&postCloneHaveTouchR1470&&boss.hp<report.fixtureBossLandingHp){
    int sameKind=0;foreach(var e in game.enemies)if(e.active&&!e.dormant&&e.hp>0&&e.kind==boss.kind)sameKind++;Need(sameKind==1,"CombatEvent target kind must uniquely identify the same living original Raven");
    foreach(var hit in game.combatEvents)if(hit.type=="hit"&&hit.damage>0&&hit.target==boss.kind&&hit.time>=postCloneTouchTimeR1470&&hit.time>report.fixtureBossLandingTime){report.fixtureBossPostLandingTouchHit=true;report.fixtureBossHitFrame=Time.frameCount;report.fixtureBossAfterHitHp=boss.hp;RecordPostCloneRecoveryR1470("real-original-command-hit-and-boss-HP-decrease-after-new-touch",floor,grounded);break;}
   }
   if(report.fixtureBossAIResumed&&report.fixtureBossPostLandingTouchHit&&!report.fixtureBossPostLandingDraw&&phase.live==0&&phase.pending==0&&boss.aiEnabled&&boss.npcState!="RAVEN_CLONE_DISABLED"){
    SeenDraw draw;seen.TryGetValue(boss,out draw);bool submitted=draw!=null&&draw.frame==Time.frameCount&&draw.hp>0&&draw.body&&draw.body.enabled&&draw.body.sprite&&worldFrame==Time.frameCount;
    if(submitted){var camera=(Camera)Field(scene,"worldCamera");var lo=camera.WorldToViewportPoint(draw.bounds.min);var hi=camera.WorldToViewportPoint(draw.bounds.max);bool visible=lo.z>=camera.nearClipPlane&&hi.z<=camera.farClipPlane&&hi.x>0&&lo.x<1&&hi.y>0&&lo.y<1;
     if(visible){report.fixtureBossPostLandingDraw=true;report.fixtureBossDrawFrame=Time.frameCount;RecordPostCloneRecoveryR1470("same-boss-actual-body-draw-after-landed-touch-combat",floor,grounded,draw,true);Capture("11-original-boss-landed-renewed-touch-combat");}
    }
   }
   report.fixtureBossRecoveryComplete=report.fixtureBossLanded&&report.fixtureBossAIResumed&&report.fixtureBossPostLandingTouchHit&&report.fixtureBossPostLandingDraw;
  }
  void Pilot(){
   if(scene.BossIntroductionActive){Move(0);return;}var phase=game.ReadFactoryRavenClonePhaseV21();if(phase.actor!=null&&phase.actor.npcState=="RAVEN_CLONE_DISABLED"&&phase.disabledElapsed<4.1f){Move(0);return;}
   Enemy target=null;float score=float.PositiveInfinity;foreach(var e in game.enemies){if(e.hp<=0||!e.active||e.dormant||e.passive||e.encounterIndex!=game.EncounterIndex||!(e.countsForClear||e.isGate||e.sourceNPC!=null&&e.sourceNPC.Contains("SMALLGATE")))continue;if(e.sourceNPC=="NUI_RAVEN_CLONE"&&phase.live+phase.pending>0)continue;float candidate=Math.Abs(e.x-game.player.x)+Math.Abs(e.y-game.player.y)*2;if(e.sourceNPC=="NUI_RAVEN_CLONE_PROTO")candidate-=10000;if(candidate<score){score=candidate;target=e;}}
   float dx=target==null?(game.HasEncounters?game.CurrentRoom.encounters[game.EncounterIndex].exitX:game.ExitX)-game.player.x:target.x-game.player.x,dy=target==null?0:target.y-game.player.y;int move=target==null?(game.HasEncounters?game.CurrentRoom.encounters[game.EncounterIndex].direction:1):Math.Abs(dx)>32?Math.Sign(dx):game.player.facing!=Math.Sign(dx)&&Math.Abs(dx)>1?Math.Sign(dx):0;Move(move);
   bool grounded=false;foreach(var floor in game.CurrentRoom.surfaces)if(floor.id==game.player.groundLine&&Math.Abs(game.player.y-floor.Y(game.player.x))<.2f&&Math.Abs(game.player.vy)<.1f)grounded=true;
   if(grounded&&game.elapsed>=nextJump&&(dy>32&&Math.Abs(dx)<150||Time.realtimeSinceStartupAsDouble-lastProgress>1&&Math.Abs(dx)>35)){Press("jump");nextJump=game.elapsed+.8f;}
   if(target!=null&&Math.Abs(dx)<95&&Math.Abs(dy)<55&&game.elapsed>=nextAttack){Press("attack");nextAttack=game.elapsed+.18f;}
   if(game.player.hp<game.player.maxHp*.45f&&(float)Field(game,"potionCooldown")<=0&&Find("potion",false))Press("potion");
  }
  IEnumerator Flow(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Check();yield return new WaitForEndOfFrame();}for(int i=0;i<3;i++)yield return new WaitForEndOfFrame();RequireReflectionContract();game=(GameModel)Field(scene,"game");originalGame=game;originalCamera=(Camera)Field(scene,"worldCamera");report.modelBefore=game.elapsed;Need(!scene.runAutomatedPlaythrough&&game.paused&&(string)Field(scene,"openMenu")=="dungeons","Original ordinary Main menu required");Capture("00-main-menu");
   var select=SelectFactory();while(select.MoveNext()){Check();yield return select.Current;}while(!scene.FactoryProductSessionActiveR1179){Check();yield return new WaitForEndOfFrame();}game=(GameModel)Field(scene,"game");report.modelBefore=game.elapsed;report.npcAudioMissingBefore=scene.NpcAudioMissing;Need(game.FactoryRavenIntroConfiguredV21&&game.FactoryRavenClonePhaseConfiguredV21,"Configured original START and genuine clone model required");for(int i=0;i<3;i++){ResumeOneObservedPause();yield return new WaitForEndOfFrame();}Observe("ordinary-factory-entry");Capture("01-factory-ordinary-entry");
   if(mode=="boss-room-fixture"){
    // Explicit setup seam. Menu/host stay real; earlier room clear is NOT claimed.
    var enter=typeof(GameModel).GetMethod("EnterRoom",Fields);Need(enter!=null,"Original room setup API absent");enter.Invoke(game,new object[]{2});report.explicitOriginalBossRoomSetup=true;
   }
   if(!Find("right",false)){Press("touch");yield return new WaitForEndOfFrame();}Need(Find("left")&&Find("right")&&Find("attack")&&Find("jump"),"Original touch combat controls unavailable");
   lastProgress=Time.realtimeSinceStartupAsDouble;lastX=game.player.x;lastHp=-1;nextSample=game.elapsed;
   while(true){
    Check();Need(scene.FactoryProductSessionActiveR1179&&ReferenceEquals(game,Field(scene,"game")),"Owned Factory model/session changed");ResumeOneObservedPause();var phase=game.ReadFactoryRavenClonePhaseV21();float hp=0;foreach(var e in game.enemies)if(e.active&&e.countsForClear)hp+=e.hp;
    if(game.room!=lastRoom||game.EncounterIndex!=lastEncounter||phase.phaseCount!=lastPhase||Math.Abs(game.player.x-lastX)>.5f||Math.Abs(hp-lastHp)>.1f){lastProgress=Time.realtimeSinceStartupAsDouble;lastX=game.player.x;lastHp=hp;lastRoom=game.room;lastEncounter=game.EncounterIndex;lastPhase=phase.phaseCount;}
    Need(Time.realtimeSinceStartupAsDouble-lastProgress<NoProgress,"No physical/combat/phase progress for90seconds");Pilot();yield return new WaitForEndOfFrame();Check();ObserveFactoryReactionDrawR1469();ObservePostCloneCombatR1470();
    if(game.elapsed>=nextSample||game.room==2&&!report.sourceOpeningReleasedOnce){nextSample=game.elapsed+(game.room==2?.25f:1f);var row=Observe("ordinary-update");
     if(report.sourceOpeningActualDraw&&!captures.Contains("02-original-start-actual-draw"))Capture("02-original-start-actual-draw");
     if(row.sourceFocusAccepted&&row.sourceTime>=1.5f&&!report.sourceOpeningReleasedOnce)Capture("03-original-start-playing");
     if(report.sourceOpeningReleasedOnce)Capture("04-original-start-released");
     if(row.phaseCount>=1)Capture("05-original-hp-triggered-clone-phase");
     if(protoDrawn.Count>0)Capture("06-genuine-proto-actual-draw");
     if(row.protoLive>0&&row.disabledElapsed>=3.8f)Capture("07-disabled-held-beyond-motion");
     if(phase.releaseCount>=1&&phase.live==0&&phase.pending==0){report.cloneCycleRecovered=true;Capture("08-original-live-zero-boss-recovered");}
    }
    if(mode=="boss-room-fixture"&&report.fixtureBossRecoveryComplete)break;
    if(mode=="natural"&&game.victory){report.fullNaturalFactoryClear=report.visitedNodes.Count==6&&report.visitedNodes.Contains("0:0")&&report.visitedNodes.Contains("0:1")&&report.visitedNodes.Contains("1:0")&&report.visitedNodes.Contains("1:1")&&report.visitedNodes.Contains("1:2")&&report.visitedNodes.Contains("2:0");Need(report.fullNaturalFactoryClear,"Natural clear requires all six original nodes");Capture("09-natural-factory-victory");break;}
   }
   ReleaseControls();report.protoInstances=protoCreated.Count;report.protoInstancesDrawn=protoDrawn.Count;report.eachCreatedProtoActuallyDrawn=protoCreated.Count>=2&&protoDrawn.SetEquals(protoCreated);Need(report.sourceOpeningActualDraw&&report.sourceOpeningReleasedOnce&&report.cloneCycleRecovered,"Actual START draw/one release and real input clone cycle required");if(mode=="boss-room-fixture")Need(report.fixtureBossRecoveryComplete,"Same original Boss must land, progress source AI, take a renewed ordinary Touch command hit and draw after clone release");Need(report.eachCreatedProtoActuallyDrawn&&report.sceneDepthBindingObserved,"Every created Proto requires actual same-frame draw/postrender/depth/in-viewport evidence");Need(report.ordinaryCameraScaleStable,"Observed intro/ordinary camera scale group changed; retain failure for camera review");Need(report.images.Exists(i=>i.label=="06-genuine-proto-actual-draw"),"Recognizable Proto screenshot required for independent visual QA");
   if(mode=="natural"){foreach(var npc in ReactionNPCsR1469){var coverage=FactoryReactionCoverageR1469(npc);foreach(var state in new[]{"Hurt","Knockdown","GettingUp","Airborne"})if(!coverage.observedStates.Contains(state)){coverage.notObservedStates.Add(state);report.factoryReactionsNotObserved.Add(npc+"/"+state);}}report.factoryReactionVisibilityComplete=report.factoryReactionMissingFrames==0&&report.factoryReactionObservedUncoveredFrames==0&&report.factoryReactionsObserved.Count>0;Need(report.factoryReactionMissingFrames==0,"Expected-onscreen alive Factory reaction had no same-frame body draw; see factoryReactionEvidence exact sourceNPC/kind/index. Native depth pixel occlusion is not classified by this witness.");}
    report.modelAfter=game.elapsed;report.npcAudioMissingAfter=scene.NpcAudioMissing;Need(report.npcAudioMissingAfter==report.npcAudioMissingBefore,"NPC audio missing count increased during actual Factory intro/clone phase");Press("dungeons");for(int i=0;i<4;i++)yield return new WaitForEndOfFrame();report.returnedOriginalMain=ReferenceEquals(Field(scene,"game"),originalGame)&&ReferenceEquals(Field(scene,"worldCamera"),originalCamera)&&!scene.FactoryProductSessionActiveR1179&&!(bool)Field(scene,"factoryProductSessionR1179")&&(string)Field(scene,"openMenu")=="dungeons";Need(report.returnedOriginalMain,"Original ordinary Main return owners not restored");Capture("10-original-main-return");Need(report.errors.Count==0,"Runtime errors recorded");
  }
  void Complete(){
   if(finished)return;finished=true;if(ReferenceEquals(focusOwner,this))focusOwner=null;ReleaseControls();try{(activeFlow as IDisposable)?.Dispose();}catch(Exception e){report.cleanupFailure+="\n"+e;}activeFlow=null;Application.logMessageReceived-=Log;Camera.onPostRender-=Rendered;if(trace!=null){trace.Flush();trace.Dispose();trace=null;}
   // No trace writes occur during cleanup, including a capacity failure. Keep
   // runInBackground TRUE until Unity processes its asynchronous normal Quit.
   int code=report.passed?0:2;var terminal=new Terminal{passed=report.passed,requestedExitCode=code,normalQuitRequested=true,failure=report.failure};
   try{var data=new UTF8Encoding(false,true).GetBytes(JsonUtility.ToJson(report,true));terminal.report=Path.Combine(output,"Factory-Raven-actual-draw-and-phase-report.json");using(var f=new FileStream(terminal.report,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){f.Write(data,0,data.Length);}terminal.sha256=Digest(data);terminal.bytes=data.LongLength;var bytes=new UTF8Encoding(false,true).GetBytes(JsonUtility.ToJson(terminal,true));using(var f=new FileStream(Path.Combine(output,"player-normal-quit-terminal-r1464.json"),FileMode.CreateNew,FileAccess.Write,FileShare.Read)){f.Write(bytes,0,bytes.Length);}}
   catch(Exception e){Debug.LogException(e);code=2;}finally{Application.Quit(code);}
  }
  void OnApplicationQuit(){if(ReferenceEquals(focusOwner,this))focusOwner=null;quitting=true;if(backgroundSaved)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){if(ReferenceEquals(focusOwner,this))focusOwner=null;if(backgroundSaved&&quitting)Application.runInBackground=backgroundBefore;if(!finished&&!quitting&&started>0){report.failure+="\nOwned observer destroyed before completion";report.passed=false;Complete();}Application.logMessageReceived-=Log;Camera.onPostRender-=Rendered;}
 }
 // Observation only. No material, sprite, sorting, scale, camera or model writes.
 public sealed class FactoryRavenDiagnosticDrawR1464 : MonoBehaviour {
  FactoryRavenPlayerCheckR1464 owner;Enemy actor;SpriteRenderer body;
  internal void Bind(FactoryRavenPlayerCheckR1464 check,Enemy enemy,SpriteRenderer renderer){owner=check;actor=enemy;body=renderer;}
  void OnWillRenderObject(){if(owner&&body)owner.Draw(actor,body,Camera.current,Time.frameCount);}
 }
}
#endif
