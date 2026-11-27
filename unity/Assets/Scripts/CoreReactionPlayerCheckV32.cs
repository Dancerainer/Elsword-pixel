#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElswordPixel {
 // Opt-in, bounded component verification. Absent its exact CLI flag this type
 // creates no object and never navigates, changes state, captures, or exits.
 public sealed partial class CoreReactionPlayerCheckV32 : MonoBehaviour {
  const string Flag="--root-core-reaction-check-v32",OutFlag="--root-core-reaction-output-v32",SlotsFlag="--root-core-reaction-slots-v32";
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
  const double Budget=480;
  [Serializable] public sealed class FrameRow {
   public string npc,phase,state,clip,sprite,image,onImage,offImage;public int frame,room,encounter,index,changedBodyPixels,rendererCount;
   public float hp,clock,npcTime,alpha,deathTime;public bool enabled,active,hasSprite,actualBodyDraw,worldPost,viewportOverlap,pixelsProven,sourceStatePreserved;
   public Vector3 viewportMin,viewportMax;public Bounds bounds;
  }
  [Serializable] public sealed class CaseRow {public string npc;public List<string> observed=new List<string>();public int frames,missingBodyFrames;public bool completed,returnedNormal,lethalBodySeen;}
  [Serializable] public sealed class Report {
   public string schema="CORE_REACTION_CONTROLLED_EXPLICIT_CAMERA_DRAW_AND_PIXELS_V32_R4",failure="",graphics;
   public bool batchMode,fullNineRequested,allNineCompleted;public int cameraOutputWidth=1280,cameraOutputHeight=720,authoredOriginalTargetDraws;public int[] requestedSlots;public List<WaitRow> waits=new List<WaitRow>();
   public bool windowPresentationVerified=false,screenCaptureClaim=false,explicitOriginalWorldCameraRender=true,ordinaryEngineUpdateDrivesGame=true;
   public bool passed,ordinaryMenuEntry,returnedMain,controlledRoomAndPlayerSetup=true,playerProtectionForFixture=true,naturalRouteClaim=false,enemyStateClockHpAiAssigned=false,sourceGameMechanismModified=false;
   public List<string> actions=new List<string>(),errors=new List<string>();public List<FrameRow> captures=new List<FrameRow>();public List<CaseRow> cases=new List<CaseRow>();
  }
  [Serializable] public sealed class WaitRow {
   public int actualMemberCount;public bool carrierRenderingSuppressed;
   public string npc,reason,state,clip,bridge;public int frame,room,encounter,index,line,playerLine;public float elapsed,hp,x,y,vx,vy,playerX,playerY,clock,npcTime,invincible;public bool active,dormant,ai,normal,initialized,grounded,armor,npcInvincible,canHit,onscreen,rendererEnabled,sprite,authoredCamera;public Vector3 camera,viewportMin,viewportMax;public Bounds bounds;
  }
  sealed class DrawSeen {internal int frame;internal SpriteRenderer body;internal CombatState combat;internal string state;}
  static readonly string[] Npcs={"NUI_NASOD_WATCH","NUI_WALLY_9TH_TYPE2","NUI_NASOD_ELITE_SHERIFF","NUI_NASOD_HEALER_BLADER","NUI_RAVEN_CLONE_B","NUI_NASOD_ELITE_GUARD","NUI_NASOD_SPRAY_ICE","NUI_NASOD_ELEMENT_FIRE_B","NUI_NASOD_ELEMENT_ICE_B"};
  static readonly int[] Rooms={0,0,0,0,0,2,2,2,2},Encounters={0,0,0,1,1,0,0,1,2},Indices={0,2,14,25,32,5,7,30,41};
  readonly Dictionary<Enemy,DrawSeen> draws=new Dictionary<Enemy,DrawSeen>();readonly HashSet<string> captureKeys=new HashSet<string>();
  int[] selectedSlots;double nextWaitLog;DemoScene scene;GameModel game;Enemy target;CaseRow current;Report report=new Report();string output;bool finished,pairRender,backgroundBefore;double started;int worldFrame=-1;RenderTexture observationTarget;
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int flags=0,outputs=0,subsets=0;string path=null,slots=null;for(int i=0;i<args.Length;i++){if(args[i]==Flag)flags++;if(args[i]==OutFlag){outputs++;if(i+1<args.Length)path=args[++i];}if(args[i]==SlotsFlag){subsets++;if(i+1<args.Length)slots=args[++i];}}
   if(flags==0)return;
   try{Need(flags==1&&outputs==1&&subsets<=1&&(subsets==0||!string.IsNullOrEmpty(slots))&&!string.IsNullOrEmpty(path),"Exactly one explicit check/output flag required");Need(!Application.isEditor&&SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null,"Standalone GPU required; batchmode allowed, nographics forbidden");string full=Path.GetFullPath(path);Need(full.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase),"Output must be an owned C runtime-drafts child");Need(!Directory.Exists(full)&&!File.Exists(full),"Fresh output required");var chosen=ParseSlots(slots);Directory.CreateDirectory(full);var check=new GameObject("Explicit controlled Core physical-body check V32").AddComponent<CoreReactionPlayerCheckV32>();check.output=full;check.selectedSlots=chosen;}
   catch(Exception e){Debug.LogException(e);Application.Quit(2);}
  }
  static int[] ParseSlots(string value){
   if(value==null)return new[]{0,1,2,3,4,5,6,7,8};var parts=value.Split(',');var result=new int[parts.Length];int last=-1;
   for(int i=0;i<parts.Length;i++){int n;Need(int.TryParse(parts[i],out n)&&parts[i]==n.ToString()&&n>=0&&n<Npcs.Length&&n>last,"Slots must be unique ascending integers 0..8");result[i]=n;last=n;}Need(result.Length>0,"Nonempty slots");return result;
  }
  static void Need(bool condition,string why){if(!condition)throw new InvalidOperationException("Core reaction check V32: "+why);}
  static object Field(object owner,string name){var f=owner.GetType().GetField(name,Fields);Need(f!=null,"Missing field "+name);return f.GetValue(owner);}
  static object Call(object owner,string name,params object[] args){var m=owner.GetType().GetMethod(name,Fields);Need(m!=null,"Missing method "+name);return m.Invoke(owner,args);}
  void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert||
   message.Contains("CORE_ELITE_ART_FALLBACK_V36")||message.Contains("HEALER_BLADER_PARTS_FALLBACK_V35")||
   message.Contains("NASOD_SPRAY_ART_FALLBACK_V35"))report.errors.Add(message+"\n"+stack);}
  void Rendered(Camera c){if(!pairRender&&scene&&ReferenceEquals(c,Field(scene,"worldCamera")))worldFrame=Time.frameCount;}
  void Check(){Need(report.errors.Count==0,"Runtime error recorded");Need(Time.realtimeSinceStartupAsDouble-started<Budget,"Wall deadline");}
  IEnumerator Start(){
   started=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;Application.runInBackground=true;report.graphics=SystemInfo.graphicsDeviceType.ToString();report.batchMode=Application.isBatchMode;report.requestedSlots=selectedSlots;report.fullNineRequested=selectedSlots.Length==Npcs.Length;Application.logMessageReceived+=Log;Camera.onPostRender+=Rendered;
   var flow=Flow();Exception failure=null;while(!finished){bool more=false;object next=null;try{DrawAuthoredOriginalTarget();more=flow.MoveNext();if(more)next=flow.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return next;}
   (flow as IDisposable)?.Dispose();if(finished)yield break;if(failure!=null)report.failure=failure.ToString();report.passed=failure==null&&report.errors.Count==0;Complete();
  }
  // The source STOP bridge deliberately accepts only the original null-target
  // world-camera draw. RT evidence is not a substitute for its gameplay owner.
  // Batchmode omits automatic presentation, so explicitly draw that same original
  // camera while an authored owner is active; its real OnWillRenderObject and
  // onPostRender callbacks feed the existing next ordinary Step. No fake focus,
  // bridge acknowledgement, Enemy assignment, or extra Step is used here.
  void DrawAuthoredOriginalTarget(){
   if(!scene||game==null||target==null||!game.enemies.Contains(target)||!scene.CoreHardProductSessionActiveR1147||!scene.CoreHardProductAuthoredCameraActiveR1147)return;
   var c=(Camera)Field(scene,"worldCamera");Need(c&&c.enabled&&c.targetTexture==null,"Authored source camera must retain its original enabled/null-target contract");
   string before=JsonUtility.ToJson(target);float time=game.elapsed;var active=RenderTexture.active;
   try{c.Render();report.authoredOriginalTargetDraws++;}finally{RenderTexture.active=active;}
   Need(c.targetTexture==null&&game.elapsed==time&&JsonUtility.ToJson(target)==before,"Authored camera draw cannot step or mutate actor");
  }
  void Update(){if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>=Budget){report.failure="Wall deadline";report.passed=false;Complete();}}
  void Press(string action){
   TouchControl c=null;foreach(var q in scene.GetComponentsInChildren<TouchControl>(true))if(q&&q.isActiveAndEnabled&&q.gameObject.activeInHierarchy&&ReferenceEquals(q.owner,scene)&&q.action==action){Need(!c,"Duplicate control "+action);c=q;}
   Need(c&&EventSystem.current,"Active ordinary control "+action);var image=c.GetComponent<Image>();Need(image&&image.raycastTarget,"Raycastable control");var pointer=new PointerEventData(EventSystem.current){pointerId=-3201,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,c.transform.position)};
   report.actions.Add("real-touch:"+action);try{Need(ExecuteEvents.Execute(c.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Pointer down rejected");}finally{if(c)ExecuteEvents.Execute(c.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
  }
  IEnumerator SelectCore(){
   var rows=(IList)Field(scene,"mapDungeons");int index=-1;for(int i=0;i<rows.Count;i++)if((string)Field(rows[i],"id")=="altera-core-hard")index=i;Need(index>=0,"Core menu route");Press("region:altera");yield return null;
   var visible=(IList)Field(scene,"mapVisible");int at=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==index)at=i;Need(at>=0,"Core visible menu card");int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();int page=at/size,guard=0;
   while((int)Field(scene,"mapPage")!=page){Need(++guard<20,"Menu page bound");Press("dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1"));yield return null;}
   Press("dungeon-card:"+(at%size));yield return null;Need((int)Field(scene,"mapSelected")==index,"Selected Core");Press("dungeon-enter");report.ordinaryMenuEntry=true;
  }
  void LateUpdate(){BindBodies();}
  SpriteRenderer[] ActualBodies(Enemy actor,SpriteRenderer carrier) {
   if(CoreElitePartsV36.Owns(actor.sourceNPC)){
    var members=new List<SpriteRenderer>();if(!scene.VisitCoreElitePartsV36(actor,members.Add))return Array.Empty<SpriteRenderer>();
    Need(members.Count==CoreElitePartsV36.PartCount(actor.sourceNPC)&&carrier&&carrier.forceRenderingOff,"Actual original elite painted pieces");
    var partRoot=members[0].transform.parent;
    foreach(var axis in new[]{Vector3.right,Vector3.up}){
     float actual=partRoot.TransformVector(axis).magnitude;
     float sceneUnit=carrier.transform.parent.TransformVector(axis).magnitude;
     Need(Mathf.Abs(actual-sceneUnit)<.001f,"Visible registered parts must apply the scene ruler once: "+actor.sourceNPC);
    }
    return members.ToArray();
   }
   if(actor.sourceNPC==NasodSprayRigV35.Ice){
    var visible=new List<SpriteRenderer>();if(!scene.VisitNasodSprayPartsV35(actor,visible.Add))return Array.Empty<SpriteRenderer>();
    Need(visible.Count==NasodSprayRigV35.PartCount&&carrier&&carrier.forceRenderingOff,"Actual original Core Spray multipart body");
    return visible.ToArray();
   }
   if(actor.sourceNPC!=HealerBladerRigV35.Id)return new[]{carrier};
   var owners=(IDictionary)Field(scene,"healerViewsV35");
   if(!owners.Contains(actor))return Array.Empty<SpriteRenderer>();
   var parts=Field(owners[actor],"parts") as FairyAuthoredPartsV33;
   var result=new List<SpriteRenderer>();parts?.VisitRenderers(result.Add);
   Need(result.Count==9&&carrier&&carrier.forceRenderingOff,"Actual nine-part Healer body required");
   return result.ToArray();
  }
  static Bounds GroupBounds(SpriteRenderer[] bodies){var bounds=bodies[0].bounds;for(int i=1;i<bodies.Length;i++)bounds.Encapsulate(bodies[i].bounds);return bounds;}
  void BindBodies(){
   if(finished||!scene||game==null||!scene.CoreHardProductSessionActiveR1147)return;
   var carriers=(IList)Field(scene,"enemyViews");
   for(int i=0;i<game.enemies.Count&&i<carriers.Count;i++){
    var actor=game.enemies[i];if(Array.IndexOf(Npcs,actor.sourceNPC)<0)continue;
    foreach(var s in ActualBodies(actor,carriers[i] as SpriteRenderer)){
     if(!s)continue;var w=s.GetComponent<CoreReactionDrawWitnessV32>();
     if(!w)w=s.gameObject.AddComponent<CoreReactionDrawWitnessV32>();w.Bind(this,actor,s);
    }
   }
  }
  internal void Draw(Enemy actor,SpriteRenderer renderer,Camera c){if(pairRender||finished||!scene||!ReferenceEquals(c,Field(scene,"worldCamera"))||!game.enemies.Contains(actor))return;draws[actor]=new DrawSeen{frame=Time.frameCount,body=renderer,combat=actor.state,state=actor.npcState};}
  string Phase(Enemy e){if(e.state==CombatState.Airborne&&(e.clip=="launched"||e.knockdownOnLanding))return e.clip=="fall"?"Fall":"Launch";return e.state.ToString();}
  // Hidden Players may not present a backbuffer/end-of-frame. Ordinary engine
  // Update still advances GameModel; this read-only camera draw supplies GPU
  // evidence independently of window presentation. It does not call Step.
  void RenderObservation(Camera camera){
   const int width=1280,height=720; // Fixed 64:36 GPU evidence viewport; independent of OS window/backbuffer.
   if(!observationTarget||observationTarget.width!=width||observationTarget.height!=height){if(observationTarget){observationTarget.Release();Destroy(observationTarget);}observationTarget=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(observationTarget.Create(),"Observation target");}
   BindBodies();var priorTarget=camera.targetTexture;var priorActive=RenderTexture.active;float elapsedBefore=game.elapsed;string actorBefore=JsonUtility.ToJson(target);
   draws.Remove(target);worldFrame=-1;
   try{camera.targetTexture=observationTarget;RenderTexture.active=observationTarget;GL.Clear(true,true,Color.clear);camera.Render();}
   finally{camera.targetTexture=priorTarget;RenderTexture.active=priorActive;}
   Need(game.elapsed==elapsedBefore&&JsonUtility.ToJson(target)==actorBefore,"Observation draw must not step or mutate actor");
  }
  FrameRow Observe(string phase,bool capture){
   Need(target!=null&&game.enemies.Contains(target),"Same original actor");int index=game.enemies.IndexOf(target);var bodies=(IList)Field(scene,"enemyViews");Need(index<bodies.Count,"Body index");var real=ActualBodies(target,bodies[index] as SpriteRenderer);Need(real.Length>0,"Original owned body group");var s=real[0];var camera=(Camera)Field(scene,"worldCamera");RenderObservation(camera);DrawSeen d;draws.TryGetValue(target,out d);
   var row=new FrameRow{npc=target.sourceNPC,phase=phase,state=target.npcState,clip=target.clip,frame=Time.frameCount,room=game.room,encounter=game.EncounterIndex,index=index,hp=target.hp,clock=target.clock,npcTime=target.npcTime,deathTime=target.deathTime,enabled=s&&s.enabled,active=s&&s.gameObject.activeInHierarchy,hasSprite=s&&s.sprite,alpha=s?s.color.a:0,sprite=s&&s.sprite?s.sprite.name:null,actualBodyDraw=d!=null&&d.frame==Time.frameCount&&d.combat==target.state&&d.state==target.npcState,worldPost=worldFrame==Time.frameCount};
   row.rendererCount=real.Length;foreach(var renderer in real)Need(renderer&&renderer.enabled&&renderer.gameObject.activeInHierarchy&&renderer.sprite&&!renderer.forceRenderingOff&&renderer.color.a>0,"Actual body member");if(s){row.bounds=GroupBounds(real);row.viewportMin=camera.WorldToViewportPoint(row.bounds.min);row.viewportMax=camera.WorldToViewportPoint(row.bounds.max);row.viewportOverlap=row.viewportMax.x>0&&row.viewportMin.x<1&&row.viewportMax.y>0&&row.viewportMin.y<1&&row.viewportMin.z>0;}
   row.sourceStatePreserved=!string.IsNullOrEmpty(target.npcState);
   current.frames++;if(!row.enabled||!row.active||!row.hasSprite||row.alpha<=0){current.missingBodyFrames++;report.captures.Add(row);Need(false,"Reaction body disabled/missing: "+row.npc+"/"+phase);}
   if(!current.observed.Contains(phase))current.observed.Add(phase);
   Need(row.actualBodyDraw&&row.worldPost&&row.viewportOverlap,"Expected controlled actor requires actual same-frame draw: "+row.npc+"/"+phase);
   if(denseRequestedV44)CheckBodyContributionV44(real,camera,row);
   if(capture){string label=(report.cases.Count-1).ToString("D2")+"-"+row.npc+"-"+phase;report.captures.Add(row);CapturePixels(real,camera,row,label);}
   return row;
  }
  static void SavePng(Texture2D image,string path){var bytes=image.EncodeToPNG();using(var f=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read))f.Write(bytes,0,bytes.Length);}
  void CapturePixels(SpriteRenderer[] real,Camera camera,FrameRow row,string label){
   // The first PNG is the explicit original world-camera RT from Observe.
   // It is not a screen/window capture. Matched additional camera draws prove
   // this exact body's pixel contribution and restore its renderer immediately.
   Texture2D view=null,on=null,off=null;RenderTexture temporary=null;var oldTarget=camera.targetTexture;var oldActive=RenderTexture.active;bool[] oldEnabled=Array.ConvertAll(real,r=>r.enabled);float clock=game.elapsed;string actorBefore=JsonUtility.ToJson(target);int postBefore=worldFrame;
   try{
    Need(observationTarget&&observationTarget.IsCreated(),"Actual world-camera render target");RenderTexture.active=observationTarget;view=new Texture2D(observationTarget.width,observationTarget.height,TextureFormat.RGBA32,false);view.ReadPixels(new Rect(0,0,observationTarget.width,observationTarget.height),0,0,false);view.Apply(false,false);row.image=Path.Combine(output,label+"-world-camera-view.png");SavePng(view,row.image);
    temporary=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(temporary.Create(),"Pixel pair target");camera.targetTexture=temporary;pairRender=true;
    RenderTexture.active=temporary;GL.Clear(true,true,Color.clear);foreach(var body in real)body.enabled=true;camera.Render();on=new Texture2D(temporary.width,temporary.height,TextureFormat.RGBA32,false);on.ReadPixels(new Rect(0,0,temporary.width,temporary.height),0,0,false);on.Apply(false,false);
    RenderTexture.active=temporary;GL.Clear(true,true,Color.clear);foreach(var body in real)body.enabled=false;camera.Render();off=new Texture2D(temporary.width,temporary.height,TextureFormat.RGBA32,false);off.ReadPixels(new Rect(0,0,temporary.width,temporary.height),0,0,false);off.Apply(false,false);
    var a=on.GetPixels32();var b=off.GetPixels32();int w=temporary.width,h=temporary.height;int x0=Mathf.Clamp(Mathf.FloorToInt(row.viewportMin.x*w)-2,0,w),x1=Mathf.Clamp(Mathf.CeilToInt(row.viewportMax.x*w)+2,0,w),y0=Mathf.Clamp(Mathf.FloorToInt(row.viewportMin.y*h)-2,0,h),y1=Mathf.Clamp(Mathf.CeilToInt(row.viewportMax.y*h)+2,0,h);
    for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++){int i=y*w+x;if(Mathf.Abs(a[i].r-b[i].r)+Mathf.Abs(a[i].g-b[i].g)+Mathf.Abs(a[i].b-b[i].b)>6)row.changedBodyPixels++;}
    row.onImage=Path.Combine(output,label+"-body-on.png");row.offImage=Path.Combine(output,label+"-body-off.png");SavePng(on,row.onImage);SavePng(off,row.offImage);row.pixelsProven=row.changedBodyPixels>0;
   }finally{for(int i=0;i<real.Length;i++)if(real[i])real[i].enabled=oldEnabled[i];camera.targetTexture=oldTarget;RenderTexture.active=oldActive;pairRender=false;worldFrame=postBefore;if(temporary){temporary.Release();Destroy(temporary);}if(view)Destroy(view);if(on)Destroy(on);if(off)Destroy(off);}
   Need(game.elapsed==clock&&JsonUtility.ToJson(target)==actorBefore,"Pixel pair must not mutate or step actor");Need(row.pixelsProven,"Body draw has no GPU pixel contribution; inspect actual foreground occlusion: "+row.npc+"/"+row.phase);
  }
  void Stage(int slot){
   Call(game,"EnterRoom",Rooms[slot]);if(Encounters[slot]!=game.EncounterIndex)Call(game,"BeginEncounter",Encounters[slot]);
   // EnterRoom creates fresh Enemy objects even on a repeated room number.
   // Use the ordinary scene rebuild to release the previous renderer owners.
   Call(scene,"RebuildRoom");
   Need(game.enemies.Count>Indices[slot],"Original source actor index");target=game.enemies[Indices[slot]];Need(target.sourceNPC==Npcs[slot]&&target.encounterIndex==Encounters[slot],"Original identity at original source birth");
   // Controlled player setup only: no Enemy HP/state/AI/clock/position writes.
   game.player.x=target.x-45;game.player.y=target.y;game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;game.player.invincible=30;
   nextWaitLog=0;report.actions.Add("CONTROLLED original EnterRoom("+Rooms[slot]+")/BeginEncounter("+Encounters[slot]+"), original actor index="+Indices[slot]+", player placed nearby and protected for fixture");
  }
  bool CanHit(){return target.active&&!target.dormant&&target.hp>0&&target.state==CombatState.Normal&&!string.IsNullOrEmpty(target.npcState)&&!(bool)Call(game,"EnemyHasSuperArmor",target)&&!(bool)Call(game,"EnemyHasNpcInvincibility",target)&&(bool)Call(game,"Grounded",target);}
  bool ReadyOnscreen(){var bodies=(IList)Field(scene,"enemyViews");int index=game.enemies.IndexOf(target);if(index<0||index>=bodies.Count)return false;var s=bodies[index] as SpriteRenderer;if(!s||!s.enabled||!s.sprite)return false;var actual=ActualBodies(target,s);if(actual.Length==0)return false;var bounds=GroupBounds(actual);var c=(Camera)Field(scene,"worldCamera");var a=c.WorldToViewportPoint(bounds.min);var b=c.WorldToViewportPoint(bounds.max);return a.x>.03f&&b.x<.97f&&a.y>.03f&&b.y<.97f;}
  void RecordWait(string reason,bool force=false){
   if(!force&&Time.realtimeSinceStartupAsDouble<nextWaitLog)return;nextWaitLog=Time.realtimeSinceStartupAsDouble+1;
   var c=(Camera)Field(scene,"worldCamera");int index=game.enemies.IndexOf(target);var bodies=(IList)Field(scene,"enemyViews");var body=index>=0&&index<bodies.Count?bodies[index] as SpriteRenderer:null;
   var bridge=Field(scene,"coreStopIntroBridgeDraftV21") as CoreStopIntroViewBridgeDraftV21;
   var r=new WaitRow{npc=target.sourceNPC,reason=reason,state=target.npcState,clip=target.clip,frame=Time.frameCount,room=game.room,encounter=game.EncounterIndex,index=index,line=target.groundLine,playerLine=game.player.groundLine,elapsed=game.elapsed,hp=target.hp,x=target.x,y=target.y,vx=target.vx,vy=target.vy,playerX=game.player.x,playerY=game.player.y,clock=target.clock,npcTime=target.npcTime,invincible=target.invincible,active=target.active,dormant=target.dormant,ai=target.aiEnabled,normal=target.state==CombatState.Normal,initialized=!string.IsNullOrEmpty(target.npcState),grounded=(bool)Call(game,"Grounded",target),armor=(bool)Call(game,"EnemyHasSuperArmor",target),npcInvincible=(bool)Call(game,"EnemyHasNpcInvincibility",target),canHit=CanHit(),onscreen=ReadyOnscreen(),rendererEnabled=body&&body.enabled,sprite=body&&body.sprite,authoredCamera=scene.CoreHardProductAuthoredCameraActiveR1147,camera=c.transform.position,bridge=bridge==null?null:JsonUtility.ToJson(bridge.ReadProbeDiagnosticsV25())};
   if(body){var actual=ActualBodies(target,body);r.actualMemberCount=actual.Length;r.carrierRenderingSuppressed=body.forceRenderingOff;
    r.bounds=actual.Length==0?body.bounds:GroupBounds(actual);r.viewportMin=c.WorldToViewportPoint(r.bounds.min);r.viewportMax=c.WorldToViewportPoint(r.bounds.max);}report.waits.Add(r);
  }
  void Hit(HitReaction reaction,bool lethal){game.player.x=target.x-target.facing*45;game.player.y=target.y;game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;game.player.invincible=30;float hp=target.hp;float factor=(game.CoreHardDefenceBoostActiveR1417(target)?GameModel.CoreHardDefenceDeviceDamageTakenR1417:1f)*(1-game.CoreHardShieldFrontDefenceR1417(target,game.player.x)*.01f);Need(!lethal||factor>0,"Lethal test cannot bypass a fully blocked hit");int damage=lethal?(int)Math.Ceiling((hp+1)/factor):1;int dealt=game.ApplyHitToEnemy(target,damage,reaction);report.actions.Add("public ApplyHitToEnemy "+target.sourceNPC+" "+reaction+" damage="+damage+" dealt="+dealt);Need(dealt>0&&target.hp<hp&&string.IsNullOrEmpty(target.npcState),"Real unarmored damage must reproduce empty source state");if(lethal)Need(target.hp<=0&&target.state==CombatState.Dead,"Lethal source hit must produce Dead/zero HP before death evidence");}
  IEnumerator Flow(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Check();yield return null;}for(int i=0;i<3;i++)yield return null;game=(GameModel)Field(scene,"game");Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused,"Ordinary Main menu");var originalGame=game;var originalCamera=(Camera)Field(scene,"worldCamera");var select=SelectCore();while(select.MoveNext())yield return select.Current;
   while(!scene.CoreHardProductSessionActiveR1147||scene.CoreHardProductSessionLoadingR1147){Check();yield return null;}game=(GameModel)Field(scene,"game");Need(game.room==0,"Ordinary Core room0 entry");
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--root-core-pending-hurt-v44")>=0){var pending=CheckPendingHurtV44();while(pending.MoveNext())yield return pending.Current;yield break;}
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--root-original-hits-v45")>=0){var check=CheckOriginalHitsV45();while(check.MoveNext())yield return check.Current;yield break;}
   foreach(int slot in selectedSlots){
    Check();current=new CaseRow{npc=Npcs[slot]};report.cases.Add(current);Stage(slot);double deadline=Time.realtimeSinceStartupAsDouble+25;
    for(int i=0;i<10;i++){Check();yield return null;}while(!CanHit()||!ReadyOnscreen()){Check();RecordWait("initial-ready",Time.realtimeSinceStartupAsDouble>=deadline);Need(Time.realtimeSinceStartupAsDouble<deadline,"Wait for source actor initialized/grounded/nonarmored/onscreen "+target.sourceNPC);yield return null;}
    RecordWait("ready",true);Observe("AuthoredState",true);
    if(denseRequestedV44&&CoreElitePartsV36.Owns(target.sourceNPC)){
     var end=Time.realtimeSinceStartupAsDouble+15;
     while(!target.npcState.EndsWith("_WAIT",StringComparison.Ordinal)||!ReadyOnscreen()){
      Check();Need(Time.realtimeSinceStartupAsDouble<end,"Standing ruler: wait for an original idle frame "+target.sourceNPC);yield return null;
     }
     Observe("Standing",true);
    }
    if(slot==2&&Array.IndexOf(Environment.GetCommandLineArgs(),"--root-core-elite-sheriff-v35")>=0){var check=CheckEliteSheriffV35();while(check.MoveNext())yield return check.Current;}
    deadline=Time.realtimeSinceStartupAsDouble+25;Hit(HitReaction.Hurt,false);
    do{yield return null;Check();string phase=Phase(target);if(target.state!=CombatState.Normal)Observe(phase,captureKeys.Add(target.sourceNPC+"/"+phase));Need(Time.realtimeSinceStartupAsDouble<deadline,"Hurt recovery deadline");}while(target.state!=CombatState.Normal);
    deadline=Time.realtimeSinceStartupAsDouble+25;while(!CanHit()){Check();RecordWait("before-launcher",Time.realtimeSinceStartupAsDouble>=deadline);Need(Time.realtimeSinceStartupAsDouble<deadline,"Wait before launcher");yield return null;}Hit(HitReaction.Launch,false);
    do{yield return null;Check();string phase=Phase(target);if(target.state!=CombatState.Normal)Observe(phase,captureKeys.Add(target.sourceNPC+"/"+phase));Need(Time.realtimeSinceStartupAsDouble<deadline,"Launch/down/getup recovery deadline");}while(target.state!=CombatState.Normal);
    current.returnedNormal=true;deadline=Time.realtimeSinceStartupAsDouble+25;while(!CanHit()){Check();RecordWait("before-lethal",Time.realtimeSinceStartupAsDouble>=deadline);Need(Time.realtimeSinceStartupAsDouble<deadline,"Wait before lethal");yield return null;}Hit(HitReaction.Hurt,true);yield return null;Observe("Dead",true);current.lethalBodySeen=true;
    foreach(string phase in new[]{"AuthoredState","Hurt","Launch","Fall","Knockdown","GettingUp","Dead"})Need(current.observed.Contains(phase),"Missing required actual reaction phase "+target.sourceNPC+"/"+phase);current.completed=current.missingBodyFrames==0;
   }
   target=null;Press("dungeons");for(int i=0;i<4;i++)yield return null;report.returnedMain=ReferenceEquals(Field(scene,"game"),originalGame)&&ReferenceEquals(Field(scene,"worldCamera"),originalCamera)&&!scene.CoreHardProductSessionActiveR1147&&Field(scene,"coreReactionPoseMapV32")==null&&(string)Field(scene,"openMenu")=="dungeons";Need(report.returnedMain,"Core return and reaction map disposal");Need(report.cases.Count==selectedSlots.Length&&report.cases.TrueForAll(c=>c.completed),"Every explicitly requested identity required");report.allNineCompleted=report.fullNineRequested&&report.cases.Count==9;
  }
  void Complete(){if(finished)return;finished=true;Application.logMessageReceived-=Log;Camera.onPostRender-=Rendered;int code=report.passed?0:2;try{File.WriteAllText(Path.Combine(output,"core-reaction-player-report.json"),JsonUtility.ToJson(report,true),new System.Text.UTF8Encoding(false));}catch(Exception e){Debug.LogException(e);code=2;}finally{Application.Quit(code);}}
  void OnApplicationQuit(){if(started>0)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){Application.logMessageReceived-=Log;Camera.onPostRender-=Rendered;ReleaseDenseEvidenceV44();if(observationTarget){observationTarget.Release();Destroy(observationTarget);observationTarget=null;}}
 }
 public sealed class CoreReactionDrawWitnessV32 : MonoBehaviour {
  CoreReactionPlayerCheckV32 owner;Enemy actor;SpriteRenderer body;
  internal void Bind(CoreReactionPlayerCheckV32 check,Enemy e,SpriteRenderer s){owner=check;actor=e;body=s;}
  void OnWillRenderObject(){if(owner&&body)owner.Draw(actor,body,Camera.current);}
 }
}
#endif
