#if ROOT_CARGO_BOSS_PLAYER_R1467 && !UNITY_EDITOR
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
 // Explicit integration fixture: ordinary menu and combat controls, one original
 // ResetGame handler invocation, and two EnterRoom(2) setups. No synthetic cue,
 // director ACK, HP/state/clock assignment, or direct GameModel.Reset/Step call.
 public sealed class CargoBossPlayerCheckR1467 : MonoBehaviour {
  const string Flag="--cargo-boss-check-r1467",OutputFlag="--cargo-boss-output-r1467";
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
  const double WallCap=420;const long TraceCap=32L*1024*1024;
  [Serializable] public sealed class ActionRow {public string action,edge;public int frame;public float modelTime;public bool delivered;}
  [Serializable] public sealed class FrameRow {
   public int cycle,frame,room,encounter,acceptedReleaseCues,sourceEvents,missileLanes;
   public long epoch;public float modelTime,heroHp,heroX,bossHp,bossX,worldSize;public double movieTime;
   public string phase,bossState;public bool worldPostRendered,humanDraw,armorDraw,viewCaptured,paused;
  }
  [Serializable] public sealed class ImageRow {public string label,path,sha256;public int frame,width,height;public long bytes;}
  [Serializable] public sealed class Report {
   public string schema="CARGO_BOSS_DIRECTOR_SAME_MODEL_RETRY_AND_SOURCE_EVENT_DIAGNOSTIC_R1467",failure="",cleanupFailure="",graphics;
   public bool passed,ordinaryMenuEntry,resetViaProductionHandlerFixture,physicalKeyboardInput=false;
   public bool explicitBossRoomSetups=true,modelHPClockStateAssigned=false,syntheticDirectorCueOrAck=false;
   public bool sameModelAndPresentationHostAfterReset,newEpochAndTicketAfterReset,returnedOriginalMain;
   public bool fullNaturalCargoClear=false,pixelVisibilityOrArtApproved=false,originalOSExitNeedsOuterOwner=true;
   public bool[] directorStarted=new bool[2],twoActorsDrawn=new bool[2],singleOriginalRelease=new bool[2],viewRestored=new bool[2];
   public bool sixMissileObserved,sixMissileScreenshot;public int ownedFocusClears,ownedFocusReleasedControls;public int completeSixLaneBursts,sourceEventCount,npcAudioMissingBefore,npcAudioMissingAfter;
   public List<string> remaining=new List<string>(),errors=new List<string>();
   public List<ActionRow> actions=new List<ActionRow>();public List<ImageRow> images=new List<ImageRow>();
   public List<CargoSourceEventV21> sourceEvents=new List<CargoSourceEventV21>();
  }
  DemoScene scene;GameModel game,originalGame;Camera originalCamera;string output;Report report=new Report();
  static bool attached;static CargoBossPlayerCheckR1467 focusOwner;bool finished,quitting,backgroundSaved,backgroundBefore;double started,nextSample;
  int worldFrame=-1,cycle;float nextAttack,nextPotion;FileStream trace;IEnumerator work;
  readonly Dictionary<string,TouchControl> held=new Dictionary<string,TouchControl>();
  readonly Dictionary<Renderer,int> rendered=new Dictionary<Renderer,int>();
  readonly HashSet<string> captured=new HashSet<string>();
  readonly HashSet<string> copiedEvents=new HashSet<string>();
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int flags=0,outputs=0;string path=null;
   for(int i=0;i<args.Length;i++){if(args[i]==Flag)flags++;else if(args[i]==OutputFlag){outputs++;if(i+1<args.Length)path=args[++i];}}
   if(flags==0)return;
   try{
    Need(flags==1&&outputs==1&&!attached,"One explicit diagnostic/output required");
    Need(Application.platform==RuntimePlatform.WindowsPlayer&&!Application.isEditor&&!Application.isBatchMode,"Visible Windows Player required");
    Need(Path.IsPathRooted(path)&&string.Equals(Path.GetPathRoot(path),@"C:\",StringComparison.OrdinalIgnoreCase),"Absolute C output required");
    foreach(var part in path.Replace('\\','/').Split('/'))Need(part!=".."&&part!="."&&!part.Equals("project",StringComparison.OrdinalIgnoreCase)&&!part.Equals("cache",StringComparison.OrdinalIgnoreCase),"No project/cache/relative output");
    path=Path.GetFullPath(path);Need(path.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase),"Owned C drafts output required");
    for(var p=new DirectoryInfo(path);p!=null;p=p.Parent)if(p.Exists)Need((p.Attributes&FileAttributes.ReparsePoint)==0,"No reparse output parent");
    Need(!Directory.Exists(path)&&!File.Exists(path),"Fresh output required");Directory.CreateDirectory(path);
    Need(SceneManager.GetActiveScene().path=="Assets/Scenes/TreeOfEl.unity","Original Main scene required");attached=true;
    var owner=new GameObject("Explicit Cargo boss/retry source check R1467").AddComponent<CargoBossPlayerCheckR1467>();owner.output=path;focusOwner=owner;
   }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
  }
  // Registered only by the validated explicit CLI Attach path. The product
  // clears input first; release real held controls so the next Pilot sends Down.
  public static bool OwnedFocusPolicyActiveR1468(DemoScene candidate){var owner=focusOwner;return candidate&&owner&&owner.isActiveAndEnabled&&!owner.finished&&!owner.quitting&&ReferenceEquals(owner.scene,candidate)&&owner.gameObject.scene==candidate.gameObject.scene;}
  public static void ObserveOwnedInputClearR1468(DemoScene candidate){if(!OwnedFocusPolicyActiveR1468(candidate))return;var owner=focusOwner;owner.report.ownedFocusClears++;owner.report.ownedFocusReleasedControls+=owner.held.Count;owner.Release();}
  static void Need(bool value,string why){if(!value)throw new InvalidOperationException("Cargo Boss check: "+why);}
  static object Field(object owner,string name){Need(owner!=null,"Missing owner "+name);var f=owner.GetType().GetField(name,Fields);Need(f!=null,"Missing field "+name);return f.GetValue(owner);}
  static string Sha(byte[] data){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(data)).Replace("-","").ToLowerInvariant();}
  void OnLog(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)report.errors.Add(message+"\n"+stack);}
  void PostRender(Camera camera){if(scene&&ReferenceEquals(camera,Field(scene,"worldCamera")))worldFrame=Time.frameCount;}
  void Check(){Need(!finished&&report.errors.Count==0,"Runtime error or finished observer");Need(Time.realtimeSinceStartupAsDouble-started<WallCap,"Wall diagnostic capacity exceeded");}
  IEnumerator Start(){
   started=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;backgroundSaved=true;Application.runInBackground=true;
   report.graphics=SystemInfo.graphicsDeviceType.ToString();Application.logMessageReceived+=OnLog;Camera.onPostRender+=PostRender;
   trace=new FileStream(Path.Combine(output,"Cargo-boss-real-observations.ndjson"),FileMode.CreateNew,FileAccess.Write,FileShare.Read);
   work=Flow();Exception failure=null;while(!finished){bool more=false;object next=null;try{more=work.MoveNext();if(more)next=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return next;}
   if(finished)yield break;try{(work as IDisposable)?.Dispose();}catch(Exception e){failure=failure??e;}work=null;
   if(failure!=null)report.failure=failure.ToString();report.passed=failure==null&&report.errors.Count==0;Complete();
  }
  void Update(){if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>=WallCap){report.failure="Wall diagnostic capacity exceeded";report.passed=false;Complete();}}
  TouchControl Find(string action,bool required=true){TouchControl result=null;foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&ReferenceEquals(c.owner,scene)&&c.action==action&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy){Need(!result,"Duplicate real control "+action);result=c;}if(required)Need(result,"Missing real control "+action);return result;}
  void Edge(TouchControl control,string action,bool down){Need(control&&EventSystem.current,"Original control/EventSystem required");var pointer=new PointerEventData(EventSystem.current){pointerId=-146700-(action=="left"?1:action=="right"?2:3),button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};bool delivered=down?ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerDownHandler):ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerUpHandler);report.actions.Add(new ActionRow{action=action,edge=down?"down":"up",frame=Time.frameCount,modelTime=game==null?0:game.elapsed,delivered=delivered});Need(delivered,"Pointer event not delivered "+action);}
  void Press(string action){var c=Find(action);var image=c.GetComponent<Image>();Need(image&&image.raycastTarget,"Raycastable original control required");try{Edge(c,action,true);}finally{if(c)Edge(c,action,false);}}
  void Move(int direction){string wanted=direction>0?"right":direction<0?"left":null;foreach(var key in new List<string>(held.Keys))if(key!=wanted){var c=held[key];held.Remove(key);if(c)Edge(c,key,false);}if(wanted!=null&&!held.ContainsKey(wanted)){var c=Find(wanted);Edge(c,wanted,true);held.Add(wanted,c);}}
  void Release(){foreach(var key in new List<string>(held.Keys)){var c=held[key];held.Remove(key);try{if(c)Edge(c,key,false);}catch(Exception e){report.cleanupFailure+="\n"+e;}}}
  IEnumerator SelectCargo(){
   Need((string)Field(scene,"openMenu")=="dungeons","Ordinary Main menu required");var rows=(IList)Field(scene,"mapDungeons");int index=-1;
   for(int i=0;i<rows.Count;i++)if((string)Field(rows[i],"id")=="bethma-cargo-airship"){Need(index<0,"Duplicate Cargo route");index=i;}Need(index>=0,"Cargo route missing");
   Press("region:"+(string)Field(rows[index],"region"));yield return new WaitForEndOfFrame();var visible=(IList)Field(scene,"mapVisible");int at=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==index)at=i;Need(at>=0,"Cargo not visible");
   int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue(),page=at/size,steps=0;
   while((int)Field(scene,"mapPage")!=page){Need(++steps<=rows.Count,"Map navigation failed");int before=(int)Field(scene,"mapPage");Press("dungeon-page:"+(before<page?"1":"-1"));yield return new WaitForEndOfFrame();Need((int)Field(scene,"mapPage")!=before,"Map page did not change");}
   Press("dungeon-card:"+(at%size));yield return new WaitForEndOfFrame();Need((int)Field(scene,"mapSelected")==index,"Wrong route selected");Press("dungeon-enter");report.ordinaryMenuEntry=true;
  }
  void SetupBoss(){var method=typeof(GameModel).GetMethod("EnterRoom",Fields);Need(method!=null,"Original room setup API missing");method.Invoke(game,new object[]{2});}
  void LateUpdate(){if(finished||!scene||!scene.CargoProductSessionActiveR1288)return;var view=Field(scene,"cargoProductDirectorViewR1288");if(view==null)return;foreach(var name in new[]{"human","armor"}){var r=(SpriteRenderer)Field(view,name);if(!r)continue;var witness=r.GetComponent<CargoBossDrawR1467>();if(!witness)witness=r.gameObject.AddComponent<CargoBossDrawR1467>();witness.Bind(this,r);}}
  internal void Draw(Renderer renderer,Camera camera){if(!finished&&scene&&ReferenceEquals(camera,Field(scene,"worldCamera"))&&renderer.enabled&&renderer.gameObject.activeInHierarchy)rendered[renderer]=Time.frameCount;}
  FrameRow Observe(){
   Check();Need(scene.CargoProductSessionActiveR1288&&ReferenceEquals(game,Field(scene,"game")),"Owned Cargo session changed");
   var session=(CargoDirectorHostSessionV21)Field(scene,"cargoProductDirectorR1288");var view=Field(scene,"cargoProductDirectorViewR1288");var human=(SpriteRenderer)Field(view,"human");var armor=(SpriteRenderer)Field(view,"armor");int f;
   bool post=worldFrame==Time.frameCount;bool hd=post&&human&&human.enabled&&human.sprite&&rendered.TryGetValue(human,out f)&&f==Time.frameCount;
   bool ad=post&&armor&&armor.enabled&&armor.sprite&&rendered.TryGetValue(armor,out f)&&f==Time.frameCount;
   var row=new FrameRow{cycle=cycle,frame=Time.frameCount,room=game.room,encounter=game.EncounterIndex,epoch=game.CargoDirectorRoomEpochV21,phase=game.CargoDirectorSourcePhaseV21,movieTime=session.MovieTime,modelTime=game.elapsed,heroHp=game.player.hp,heroX=game.player.x,paused=game.paused,acceptedReleaseCues=session.AcceptedReleaseCues,worldPostRendered=post,humanDraw=hd,armorDraw=ad,viewCaptured=(bool)Field(view,"captured"),worldSize=((Camera)Field(scene,"worldCamera")).orthographicSize};
   foreach(var e in game.enemies)if(e.sourceNPC=="NUI_WALLY_9TH_ARMOR"){row.bossState=e.npcState;row.bossHp=e.hp;row.bossX=e.x;}
   row.sourceEvents=game.cargoSourceEventsV21.Count;var groups=new Dictionary<string,HashSet<int>>(StringComparer.Ordinal);
   foreach(var e in game.cargoSourceEventsV21){string key=cycle+"/"+e.type+"/"+e.time.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"/"+e.serial+"/"+e.lane;if(copiedEvents.Add(key))report.sourceEvents.Add(e);
    if(e.type=="source-six-missile"){string burst=e.time.ToString("R",System.Globalization.CultureInfo.InvariantCulture);HashSet<int> lanes;if(!groups.TryGetValue(burst,out lanes)){lanes=new HashSet<int>();groups.Add(burst,lanes);}lanes.Add(e.lane);}}
   int complete=0;foreach(var lanes in groups.Values){row.missileLanes=Math.Max(row.missileLanes,lanes.Count);if(lanes.SetEquals(new[]{0,1,2,3,4,5}))complete++;}report.completeSixLaneBursts=Math.Max(report.completeSixLaneBursts,complete);report.sourceEventCount=report.sourceEvents.Count;report.sixMissileObserved|=complete>0;
   if(row.phase==ElswordPixel.CargoWallyDirectorDraftV21.CargoWallyDirectorClockV21.StartState)report.directorStarted[cycle]=true;
   report.twoActorsDrawn[cycle]|=hd&&ad;
   if(row.acceptedReleaseCues>0){Need(row.acceptedReleaseCues==1,"Duplicate source release cue");report.singleOriginalRelease[cycle]=true;}
   report.viewRestored[cycle]|=report.singleOriginalRelease[cycle]&&!row.viewCaptured&&!human.enabled&&!armor.enabled;
   if(Time.realtimeSinceStartupAsDouble>=nextSample){nextSample=Time.realtimeSinceStartupAsDouble+.2;var raw=new UTF8Encoding(false,true).GetBytes(JsonUtility.ToJson(row)+"\n");Need(trace.Length+raw.Length<=TraceCap,"32MiB diagnostic trace capacity exceeded");trace.Write(raw,0,raw.Length);}
   return row;
  }
  void Capture(string label){if(!captured.Add(label))return;Need(worldFrame==Time.frameCount,"Actual world post-render required");Texture2D image=null;try{image=ScreenCapture.CaptureScreenshotAsTexture();Need(image&&image.width==Screen.width&&image.height==Screen.height,"Actual Player capture required");var raw=image.EncodeToPNG();var path=Path.Combine(output,label+".png");using(var f=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){f.Write(raw,0,raw.Length);}report.images.Add(new ImageRow{label=label,path=path,sha256=Sha(raw),bytes=raw.Length,frame=Time.frameCount,width=image.width,height=image.height});}finally{if(image)Destroy(image);}}
  void Pilot(){
   if(scene.BossIntroductionActive||game.paused){Move(0);return;}Enemy boss=null;foreach(var e in game.enemies)if(e.sourceNPC=="NUI_WALLY_9TH_ARMOR"&&e.hp>0&&e.active&&!e.dormant)boss=e;if(boss==null){Move(0);return;}
   float dx=boss.x-game.player.x,dy=boss.y-game.player.y;bool lower= boss.hp>boss.maxHp*.65f;
   if(lower){Move(Math.Abs(dx)>45?Math.Sign(dx):Math.Abs(dx)>1&&game.player.facing!=Math.Sign(dx)?Math.Sign(dx):0);if(Math.Abs(dx)<95&&Math.Abs(dy)<55&&game.elapsed>=nextAttack){Press("attack");nextAttack=game.elapsed+.2f;}}
   else Move(Math.Abs(dx)>130?Math.Sign(dx):Math.Abs(dx)<100?-Math.Sign(dx):0);
   if(game.player.hp<game.player.maxHp*.45f&&game.elapsed>=nextPotion&&Find("potion",false)){Press("potion");nextPotion=game.elapsed+1;}
  }
  IEnumerator Flow(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Check();yield return new WaitForEndOfFrame();}for(int i=0;i<3;i++)yield return new WaitForEndOfFrame();
   originalGame=(GameModel)Field(scene,"game");originalCamera=(Camera)Field(scene,"worldCamera");Need(originalGame.paused&&!scene.runAutomatedPlaythrough,"Original idle Main required");
   var select=SelectCargo();while(select.MoveNext()){Check();yield return select.Current;}while(!scene.CargoProductSessionActiveR1288){Check();yield return new WaitForEndOfFrame();}game=(GameModel)Field(scene,"game");report.npcAudioMissingBefore=scene.NpcAudioMissing;
   bool resumed=false;for(int i=0;i<4;i++){if(game.paused){Need(!resumed,"Repeated entry pause");Press("pause");resumed=true;}yield return new WaitForEndOfFrame();}
   if(!Find("right",false)){Press("touch");yield return new WaitForEndOfFrame();}Need(Find("left")&&Find("right")&&Find("attack"),"Original touch controls absent");
   var sameModel=game;var sameHost=Field(scene,"cargoProductDirectorR1288");object firstTicket=null;long firstEpoch=0;
   for(cycle=0;cycle<2;cycle++){
    SetupBoss();double until=Time.realtimeSinceStartupAsDouble+35;bool ready=false;
    while(!ready){Check();Need(Time.realtimeSinceStartupAsDouble<until,"Original Cargo director did not draw/release in35seconds");if(game.paused){Need(!resumed,"Repeated pause");Press("pause");resumed=true;}yield return new WaitForEndOfFrame();var row=Observe();
     if(row.humanDraw&&row.armorDraw)Capture("cycle"+cycle+"-actual-director-two-actor-draw");
     ready=report.directorStarted[cycle]&&report.twoActorsDrawn[cycle]&&report.singleOriginalRelease[cycle]&&report.viewRestored[cycle];
    }
    var ticket=Field(game,"cargoDirectorTicketV21");Need(ticket!=null,"Actual source director ticket required");
    if(cycle==0){firstTicket=ticket;firstEpoch=game.CargoDirectorRoomEpochV21;}else report.newEpochAndTicketAfterReset=game.CargoDirectorRoomEpochV21>firstEpoch&&!ReferenceEquals(firstTicket,ticket);
    Capture("cycle"+cycle+"-original-director-released");
    if(cycle==0){
     float end=game.elapsed+65;while(game.elapsed<end&&game.player.hp>0&&!report.sixMissileObserved){Check();Pilot();yield return new WaitForEndOfFrame();Observe();if(report.sixMissileObserved){Capture("actual-six-source-missile-lanes");report.sixMissileScreenshot=true;}}
     Release();
     // Explicitly authorized integration seam: the SAME production handler as
     // F5/Enter, without claiming a physical keyboard event or new model owner.
     var reset=typeof(DemoScene).GetMethod("ResetGame",Fields);Need(reset!=null,"Original F5/Enter handler absent");reset.Invoke(scene,null);report.resetViaProductionHandlerFixture=true;
     for(int i=0;i<4;i++)yield return new WaitForEndOfFrame();
     report.sameModelAndPresentationHostAfterReset=ReferenceEquals(sameModel,Field(scene,"game"))&&ReferenceEquals(sameHost,Field(scene,"cargoProductDirectorR1288"));Need(report.sameModelAndPresentationHostAfterReset,"Retry replaced the model/presentation host");
    }
   }
   Release();Need(report.newEpochAndTicketAfterReset,"Retry reused a stale director ticket/epoch");
   if(!report.sixMissileObserved)report.remaining.Add("Ordinary input did not reach a complete six-missile burst within65modelseconds; low-HP move coverage is unproven.");
   if(!report.sixMissileScreenshot)report.remaining.Add("No source-six-missile screenshot; particle visibility/occlusion not approved.");
   report.npcAudioMissingAfter=scene.NpcAudioMissing;Need(report.npcAudioMissingAfter==report.npcAudioMissingBefore,"NPC audio missing count increased during Cargo director/combat/retry");
   var oldDepth=(RenderTexture)Field(scene,"nativeSceneEyeDepthR1461");Press("dungeons");for(int i=0;i<4;i++)yield return new WaitForEndOfFrame();
   report.returnedOriginalMain=ReferenceEquals(Field(scene,"game"),originalGame)&&ReferenceEquals(Field(scene,"worldCamera"),originalCamera)&&!scene.CargoProductSessionActiveR1288&&!(bool)Field(scene,"cargoProductSessionR1288")&&Field(scene,"cargoProductDirectorR1288")==null&&(!oldDepth||!oldDepth.IsCreated())&&(string)Field(scene,"openMenu")=="dungeons";
   Need(report.returnedOriginalMain,"Cargo owner/camera/depth were not released to Main");Capture("original-main-return");Need(report.errors.Count==0&&report.cleanupFailure=="","Runtime/cleanup failure");
  }
  void Complete(){
   if(finished)return;finished=true;if(ReferenceEquals(focusOwner,this))focusOwner=null;Release();try{(work as IDisposable)?.Dispose();}catch(Exception e){report.cleanupFailure+="\n"+e;}work=null;
   Application.logMessageReceived-=OnLog;Camera.onPostRender-=PostRender;if(trace!=null){trace.Flush();trace.Dispose();trace=null;}
   if(report.cleanupFailure!="")report.passed=false;int code=report.passed?0:2;
   try{var raw=new UTF8Encoding(false,true).GetBytes(JsonUtility.ToJson(report,true));var path=Path.Combine(output,"Cargo-boss-director-retry-report.json");using(var f=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){f.Write(raw,0,raw.Length);}File.WriteAllText(Path.Combine(output,"normal-quit-terminal.json"),JsonUtility.ToJson(new Terminal{report=path,sha256=Sha(raw),bytes=raw.Length,passed=report.passed,requestedExitCode=code}),new UTF8Encoding(false));}
   catch(Exception e){Debug.LogException(e);code=2;}finally{Application.Quit(code);}
  }
  [Serializable] public sealed class Terminal {public string schema="CARGO_BOSS_NORMAL_QUIT_TERMINAL_R1467",report,sha256;public long bytes;public bool passed,originalOSExitNeedsOuterOwner=true;public int requestedExitCode;}
  void OnApplicationQuit(){if(ReferenceEquals(focusOwner,this))focusOwner=null;quitting=true;if(backgroundSaved)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){if(ReferenceEquals(focusOwner,this))focusOwner=null;if(!finished&&!quitting&&started>0){report.failure+="\nObserver destroyed before completion";report.passed=false;Complete();}Application.logMessageReceived-=OnLog;Camera.onPostRender-=PostRender;if(backgroundSaved&&quitting)Application.runInBackground=backgroundBefore;}
 }
 public sealed class CargoBossDrawR1467:MonoBehaviour {
  CargoBossPlayerCheckR1467 owner;Renderer body;internal void Bind(CargoBossPlayerCheckR1467 check,Renderer renderer){owner=check;body=renderer;}
  void OnWillRenderObject(){if(owner&&body)owner.Draw(body,Camera.current);}
 }
}
#endif
