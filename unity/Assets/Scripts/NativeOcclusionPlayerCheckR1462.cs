#if ROOT_OCCLUSION_CHECK_R1462 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElswordPixel {
 // Explicit diagnostics only. With no flag this type does not create an object,
 // alter application state, navigate menus, or request exit.
 public sealed class NativeOcclusionPlayerCheckR1462 : MonoBehaviour {
  const string Flag="--native-occlusion-check-r1462", OutputFlag="--native-occlusion-output-r1462", ReadabilityFlagR1482="--below-core-readability-check-r1482";
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
  [Serializable] public sealed class Snapshot {
   public string route,world,room,image,nativeRevealImage,nativeSolidImage;public int nativeChangedPixels,belowFootChangedPixels;public Vector3 modelFootProjectedViewport; public int frame,bindings,width,height;
   public bool playerRevealBound,playerRevealCallbackRegistered;public int heroForegroundPixels;public Vector4 revealViewport,revealEyeStrength;public float modelTime,playerX,playerY; public Vector3 nativeCameraAngles; public bool actualWorldRendered,depthReady,oldDepthReleased;
   public List<string> materials=new List<string>();
  }
  [Serializable] public sealed class Report {
   public string schema="NATIVE_OCCLUSION_ORDINARY_MENU_PLAYER_CHECK_R1462",failure="",graphics;
   public bool passed,ordinaryMenuEntry=true,modelStateAssigned=false,fullNaturalClear=false,pixelSemanticsProven=false;
   public List<string> errors=new List<string>(),actions=new List<string>();
   public List<Snapshot> snapshots=new List<Snapshot>();
   public bool controlledRoomSetupR1482,directActorHpAiClockWritesR1482=false,visualRegressionOnlyR1482;
   public List<ReadabilityShotR1482> readabilityShots=new List<ReadabilityShotR1482>();
   public List<ReadabilityCameraSampleR1482> readabilityCamera=new List<ReadabilityCameraSampleR1482>();
   public List<ReadabilityReturnR1482> readabilityReturns=new List<ReadabilityReturnR1482>();
  }
  DemoScene scene; GameModel game; string output; Report report=new Report();bool readabilityR1482;
  readonly HashSet<string> foregroundObserved=new HashSet<string>();int worldFrame=-1; bool finished,backgroundBefore; double started;
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int count=0,readability=0,outputs=0;string path=null;
   for(int i=0;i<args.Length;i++){if(args[i]==Flag)count++;if(args[i]==ReadabilityFlagR1482)readability++;if(args[i]==OutputFlag){outputs++;if(i+1<args.Length)path=args[++i];}}
   if(count==0&&readability==0)return;
   try{
    Need(count+readability==1&&outputs==1&&!string.IsNullOrWhiteSpace(path),"Exactly one occlusion/readability diagnostic flag and one output required");
    Need(!Application.isEditor&&!Application.isBatchMode,"Visible standalone Player required");
    var full=Path.GetFullPath(path);Need(full.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase),"Diagnostic output outside owned C drafts");
    Need(!Directory.Exists(full)&&!File.Exists(full),"Fresh diagnostic output required");Directory.CreateDirectory(full);
    var check=new GameObject(readability==1?"Explicit Below/Core controlled visual check R1482":"Explicit Cargo/Battle occlusion check R1462").AddComponent<NativeOcclusionPlayerCheckR1462>();check.output=full;check.readabilityR1482=readability==1;
    if(check.readabilityR1482){check.report.schema="BELOW_CORE_ORDINARY_ENTRY_CONTROLLED_BOSS_VISUAL_REGRESSION_R1482";check.report.controlledRoomSetupR1482=true;check.report.visualRegressionOnlyR1482=true;check.report.modelStateAssigned=true;}
   }catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
  }
  static void Need(bool valid,string reason){if(!valid)throw new InvalidOperationException("Occlusion Player check: "+reason);}
  static object Field(object owner,string name){Need(owner!=null,"Missing field owner "+name);var f=owner.GetType().GetField(name,Fields);Need(f!=null,"Missing field "+name);return f.GetValue(owner);}
  void OnLog(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)report.errors.Add(message+"\n"+stack);}
  void Rendered(Camera camera){if(scene&&ReferenceEquals(camera,Field(scene,"worldCamera")))worldFrame=Time.frameCount;}
  IEnumerator Start(){
   started=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;Application.runInBackground=true;
   report.graphics=SystemInfo.graphicsDeviceType.ToString();Application.logMessageReceived+=OnLog;Camera.onPostRender+=Rendered;
   var flow=readabilityR1482?ReadabilityFlowR1482():Flow();Exception failure=null;
   while(true){bool more=false;object next=null;try{more=flow.MoveNext();if(more)next=flow.Current;}catch(Exception ex){failure=ex;}
    if(failure!=null||!more)break;yield return next;
   }
   (flow as IDisposable)?.Dispose();if(failure!=null)report.failure=failure.ToString();
   report.passed=failure==null&&report.errors.Count==0;Complete();
  }
  void Update(){if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>180){report.failure+="\nDiagnostic deadline exceeded";report.passed=false;Complete();}}
  void CheckProgress(){Need(report.errors.Count==0,"Runtime error logged");Need(Time.realtimeSinceStartupAsDouble-started<180,"Diagnostic deadline exceeded");}
  void Press(string action){
   Need(scene&&EventSystem.current,"Active scene/EventSystem required");TouchControl control=null;int count=0;
   foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy&&ReferenceEquals(c.owner,scene)&&c.action==action){control=c;count++;}
   Need(count==1&&control,"One active real control for "+action);var image=control.GetComponent<Image>();Need(image&&image.raycastTarget,"Raycastable real control required");
   var pointer=new PointerEventData(EventSystem.current){pointerId=-1462,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
   report.actions.Add(action);
   try{Need(ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Control rejected down");}
   finally{if(control)ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
  }
  IEnumerator Select(string id){
   Need((string)Field(scene,"openMenu")=="dungeons","Ordinary dungeon menu required");var rows=(IList)Field(scene,"mapDungeons");int index=-1;
   for(int i=0;i<rows.Count;i++)if((string)Field(rows[i],"id")==id){Need(index<0,"Duplicate route");index=i;}Need(index>=0,"Missing route "+id);
   Press("region:"+(string)Field(rows[index],"region"));yield return new WaitForEndOfFrame();
   var visible=(IList)Field(scene,"mapVisible");int at=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==index)at=i;Need(at>=0,"Route not visible in region");
   var pageField=scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic);Need(pageField!=null,"Map page size unavailable");int pageSize=(int)pageField.GetRawConstantValue();Need(pageSize>0,"Invalid page size");
   int page=at/pageSize,steps=0;
   while((int)Field(scene,"mapPage")!=page){Need(++steps<=rows.Count,"Map navigation failed");int before=(int)Field(scene,"mapPage");Press("dungeon-page:"+(before<page?"1":"-1"));yield return new WaitForEndOfFrame();Need((int)Field(scene,"mapPage")!=before,"No map page progress");}
   Press("dungeon-card:"+(at%pageSize));yield return new WaitForEndOfFrame();Need((int)Field(scene,"mapSelected")==index,"Wrong route selected");Press("dungeon-enter");
  }
  bool Active(string id){return id=="altera-below-tunnel"?scene.BelowProductRouteActiveR1130&&!((bool)Field(scene,"belowPlayerLoadingV21")):id=="bethma-cargo-airship"?scene.CargoProductSessionActiveR1288:scene.BattleProductSessionActiveR1325;}
  IEnumerator Walk(string route,string action,float seconds){
   TouchControl control=null;
   foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&ReferenceEquals(c.owner,scene)&&c.action==action){Need(control==null,"Duplicate movement control");control=c;}
   Need(control,"Missing original movement control");
   if(!control.isActiveAndEnabled){Press("touch");yield return new WaitForEndOfFrame();}
   Need(control.isActiveAndEnabled&&control.gameObject.activeInHierarchy,"Original touch toggle did not expose movement");
   var pointer=new PointerEventData(EventSystem.current){pointerId=-1463,button=PointerEventData.InputButton.Left};float until=game.elapsed+seconds;
   try{
    Need(ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Movement down was not consumed");report.actions.Add("hold:"+action);
    while(game.elapsed<until){CheckProgress();Need(game.player.hp>0&&!game.paused,"Movement probe stopped by death/pause");yield return new WaitForEndOfFrame();if(!foregroundObserved.Contains(route)&&HeroForegroundPixelsR1478()>0){Observe(route,route+"-player-under-foreground");foregroundObserved.Add(route);}}
   }finally{if(control)ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
   Observe(route,route+"-walk-"+action);
  }
  Snapshot Observe(string id,string label,RenderTexture oldDepth=null){
   CheckProgress();Need(worldFrame==Time.frameCount,"Actual world camera frame required");
   var depth=(RenderTexture)Field(scene,"nativeSceneEyeDepthR1461");var native=Field(scene,"nativeScene");
   var bindings=(IList)Field(scene,"nativeSceneOcclusionBindingsR1461");
   var row=new Snapshot{route=id,world=(string)Field(native,"id"),room=game.CurrentRoom.id,frame=Time.frameCount,modelTime=game.elapsed,playerX=game.player.x,playerY=game.player.y,nativeCameraAngles=((Camera)Field(scene,"nativeCamera")).transform.eulerAngles,bindings=bindings.Count,actualWorldRendered=true,depthReady=(bool)Field(scene,"nativeSceneDepthReadyR1461"),width=depth?depth.width:0,height=depth?depth.height:0,oldDepthReleased=!oldDepth||!oldDepth.IsCreated()};
   Need(depth&&depth.IsCreated()&&row.depthReady&&row.bindings>0,"Live native depth and actor bindings required");
   Need(depth.format==RenderTextureFormat.RFloat&&depth.filterMode==FilterMode.Point,"Actual RFloat Point target required");
   var seen=new HashSet<string>();var block=new MaterialPropertyBlock();int enabled=0;
   foreach(var binding in bindings){var renderer=(Renderer)Field(binding,"renderer");if(!renderer||!renderer.enabled||!renderer.gameObject.activeInHierarchy)continue;
    renderer.GetPropertyBlock(block);Need(block.GetFloat("_NativeSceneClipR1461")>.5f,"Actor binding lost per-renderer clip");Need(renderer.sharedMaterial&&renderer.sharedMaterial.shader,"Bound actor shader absent");seen.Add(renderer.sharedMaterial.shader.name);enabled++;
   }
   var hero=(SpriteRenderer)Field(scene,"heroView");Need(hero,"Original player renderer missing");hero.GetPropertyBlock(block);row.playerRevealBound=block.GetFloat("_NativeSceneClipR1461")>.5f;row.playerRevealCallbackRegistered=(bool)Field(scene,"nativePlayerRevealCameraRegisteredR1478");row.revealViewport=Shader.GetGlobalVector("_NativePlayerRevealViewportR1478");row.revealEyeStrength=Shader.GetGlobalVector("_NativePlayerRevealEyeStrengthR1478");row.heroForegroundPixels=HeroForegroundPixelsR1478();
   Need(row.playerRevealBound&&row.playerRevealCallbackRegistered&&Shader.GetGlobalFloat("_NativePlayerRevealEnabledR1478")>.5f&&row.revealEyeStrength.y>0,"Actual main player must receive active foreground readability");
   Need(Mathf.Abs(row.revealViewport.z*depth.width-row.revealViewport.w*depth.height)<.1f,"Foreground opening must be circular in native pixels");
   Need(enabled>0,"No active bound actor");row.materials.AddRange(seen);row.materials.Sort(StringComparer.Ordinal);
   Texture2D image=null;try{image=ScreenCapture.CaptureScreenshotAsTexture();Need(image&&image.width==Screen.width&&image.height==Screen.height,"Actual full Player capture required");row.image=Path.Combine(output,label+".png");File.WriteAllBytes(row.image,image.EncodeToPNG());}finally{if(image)Destroy(image);}
   if(id=="altera-below-tunnel")ObserveNativeForegroundPairR1479(row,label);report.snapshots.Add(row);return row;
  }


  void ObserveNativeForegroundPairR1479(Snapshot row,string label){
   // Same source geometry/camera/actor state. The second native-color draw
   // suppresses only this owner's reveal callback and its shader mask.
   // It renders into a temporary target, never replacing the live native image.
   var camera=(Camera)Field(scene,"nativeCamera");var target=(RenderTexture)Field(scene,"nativeTarget");var previous=RenderTexture.active;var oldTarget=camera.targetTexture;
   var method=scene.GetType().GetMethod("PrepareNativePlayerRevealCameraR1478",Fields);Need(method!=null&&(bool)Field(scene,"nativePlayerRevealCameraRegisteredR1478"),"Live native reveal owner required");
   var callback=(Camera.CameraCallback)Delegate.CreateDelegate(typeof(Camera.CameraCallback),scene,method);float enabled=Shader.GetGlobalFloat("_NativePlayerRevealEnabledR1478");
   RenderTexture temporary=null;Texture2D on=null,off=null;bool removed=false;float modelTime=game.elapsed;float x=game.player.x,y=game.player.y;
   try{
    RenderTexture.active=target;on=new Texture2D(target.width,target.height,TextureFormat.RGBA32,false);on.ReadPixels(new Rect(0,0,target.width,target.height),0,0,false);on.Apply(false,false);
    temporary=new RenderTexture(target.width,target.height,24,RenderTextureFormat.ARGB32){filterMode=FilterMode.Point};Need(temporary.Create(),"Native comparison target creation");
    Camera.onPreCull-=callback;removed=true;Shader.SetGlobalFloat("_NativePlayerRevealEnabledR1478",0);camera.targetTexture=temporary;camera.Render();RenderTexture.active=temporary;
    off=new Texture2D(target.width,target.height,TextureFormat.RGBA32,false);off.ReadPixels(new Rect(0,0,target.width,target.height),0,0,false);off.Apply(false,false);
    var projection=scene.GetType().GetMethod("ProjectNativePoint",Fields,null,new[]{typeof(Vector3),typeof(float)},null);Need(projection!=null,"Original projection method required");var foot=(Vector3)projection.Invoke(scene,new object[]{new Vector3(x,y,0),0f});row.modelFootProjectedViewport=((Camera)Field(scene,"worldCamera")).WorldToViewportPoint(foot);
    var a=on.GetPixels32();var b=off.GetPixels32();for(int py=0;py<target.height;py++)for(int px=0;px<target.width;px++){int i=py*target.width+px;if(a[i].Equals(b[i]))continue;row.nativeChangedPixels++;float u=(px+.5f)/target.width,v=(py+.5f)/target.height;if(Mathf.Abs(u-row.modelFootProjectedViewport.x)<.09f&&v<row.modelFootProjectedViewport.y+.01f&&v>row.modelFootProjectedViewport.y-.14f)row.belowFootChangedPixels++;}
    row.nativeRevealImage=Path.Combine(output,label+"-native-reveal.png");row.nativeSolidImage=Path.Combine(output,label+"-native-solid.png");File.WriteAllBytes(row.nativeRevealImage,on.EncodeToPNG());File.WriteAllBytes(row.nativeSolidImage,off.EncodeToPNG());
    Need(game.elapsed==modelTime&&game.player.x==x&&game.player.y==y,"Read-only native comparison advanced model state");
   }finally{camera.targetTexture=oldTarget;Shader.SetGlobalFloat("_NativePlayerRevealEnabledR1478",enabled);if(removed)Camera.onPreCull+=callback;RenderTexture.active=previous;if(temporary){temporary.Release();Destroy(temporary);}if(on)Destroy(on);if(off)Destroy(off);}
  }
  int HeroForegroundPixelsR1478(){
   var target=(RenderTexture)Field(scene,"nativeSceneEyeDepthR1461");if(!target||!target.IsCreated())return 0;
   var hero=(SpriteRenderer)Field(scene,"heroView");var camera=(Camera)Field(scene,"worldCamera");if(!hero||!camera)return 0;
   var a=camera.WorldToViewportPoint(hero.bounds.min);var b=camera.WorldToViewportPoint(hero.bounds.max);var block=new MaterialPropertyBlock();hero.GetPropertyBlock(block);float actorEye=block.GetFloat("_NativeActorEyeDepthR1461");
   var previous=RenderTexture.active;Texture2D pixels=null;
   try{RenderTexture.active=target;pixels=new Texture2D(target.width,target.height,TextureFormat.RFloat,false,true);pixels.ReadPixels(new Rect(0,0,target.width,target.height),0,0,false);pixels.Apply(false,false);int count=0;
    int x1=Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.x,b.x)*target.width),0,target.width),x2=Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.x,b.x)*target.width),0,target.width),y1=Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.y,b.y)*target.height),0,target.height),y2=Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.y,b.y)*target.height),0,target.height);
    for(int y=y1;y<y2;y++)for(int x=x1;x<x2;x++)if(pixels.GetPixel(x,y).r+.5f<actorEye)count++;return count;
   }finally{RenderTexture.active=previous;if(pixels)Destroy(pixels);}
  }
  IEnumerator Flow(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){CheckProgress();yield return new WaitForEndOfFrame();}
   for(int i=0;i<3;i++)yield return new WaitForEndOfFrame();game=(GameModel)Field(scene,"game");
   Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused&&!scene.runAutomatedPlaythrough,"Natural ordinary Main menu required");
   var originalGame=game;var originalCamera=(Camera)Field(scene,"worldCamera");
   foreach(string id in new[]{"altera-below-tunnel","bethma-cargo-airship","altera-battle-airship"}){
    var select=Select(id);while(select.MoveNext())yield return select.Current;
    while(!Active(id)){CheckProgress();yield return new WaitForEndOfFrame();}
    game=(GameModel)Field(scene,"game");float begin=game.elapsed;int first=Time.frameCount;bool resumed=false;
    while(Time.frameCount<first+8||game.elapsed<begin+.15f){CheckProgress();if(game.paused){Need(!resumed,"Repeated pause");resumed=true;Press("pause");}yield return new WaitForEndOfFrame();}
    Observe(id,id+"-entry");float target=game.elapsed+1;
    while(game.elapsed<target){CheckProgress();Need(!game.paused,"Unexpected paused route");yield return new WaitForEndOfFrame();}
    Observe(id,id+"-later");
    if(id=="bethma-cargo-airship"||id=="altera-below-tunnel")foreach(string direction in new[]{"right","left"}){var walk=Walk(id,direction,2);while(walk.MoveNext())yield return walk.Current;}
    var oldDepth=(RenderTexture)Field(scene,"nativeSceneEyeDepthR1461");
    Press("dungeons");for(int i=0;i<4;i++)yield return new WaitForEndOfFrame();
    Need(ReferenceEquals(Field(scene,"game"),originalGame)&&ReferenceEquals(Field(scene,"worldCamera"),originalCamera),"Original Main owners were not restored");
    Need(!oldDepth||!oldDepth.IsCreated(),"Old route depth target leaked after return");game=originalGame;Observe("main",id+"-return",oldDepth);
   }
  }
  [Serializable] public sealed class ReadabilityMinerR1482 {
   public int index,groundLine;public string npc,sourceState,combatState,sprite;
   public bool active,grounded,rendererEnabled,viewportOverlap;public float x,y,nativeDepth,hp,sourceClock;
   public Vector3 rendererOrigin,modelFootViewport,viewportMin,viewportMax;
  }
  [Serializable] public sealed class ReadabilityShotR1482 {
   public string route,label,room,image,heroSprite;public int frame,roomIndex,nativeWidth,nativeHeight;
   public bool controlledRoomSetup,actualWorldRendered,depthReady,heroViewportOverlap,authoredIntroActive;
   public float modelTime,halfHeight,playerX,playerY,playerHp,heroSpriteViewportHeight;
   public Vector3 cameraPosition,heroViewportMin,heroViewportMax;
   public List<ReadabilityMinerR1482> miners=new List<ReadabilityMinerR1482>();
  }
  [Serializable] public sealed class ReadabilityCameraSampleR1482 {
   public string route,phase;public int frame,room;public float modelTime,halfHeight,playerX,playerY;public Vector3 cameraPosition;public bool authoredIntroActive;
  }
  [Serializable] public sealed class ReadabilityReturnR1482 {
   public string route;public bool originalGameRestored,originalCameraRestored,oldDepthReleased,sessionInactive,ownerResourcesReleased;
  }
  bool ReadabilityActiveR1482(string id){return id=="altera-below-tunnel"?scene.BelowProductRouteActiveR1130&&!((bool)Field(scene,"belowPlayerLoadingV21")):scene.CoreHardProductSessionActiveR1147;}
  bool ReadabilityIntroR1482(string id){return scene.BossIntroductionActive||id=="altera-core-hard"&&scene.CoreHardProductAuthoredCameraActiveR1147;}
  float readabilitySampleAtR1482=-1;
  void SampleReadabilityCameraR1482(string id,string phase){
   if(worldFrame!=Time.frameCount||Time.unscaledTime<readabilitySampleAtR1482||report.readabilityCamera.Count>=2048)return;
   readabilitySampleAtR1482=Time.unscaledTime+.1f;var camera=(Camera)Field(scene,"worldCamera");
   report.readabilityCamera.Add(new ReadabilityCameraSampleR1482{route=id,phase=phase,frame=Time.frameCount,room=game.room,modelTime=game.elapsed,halfHeight=camera.orthographicSize,playerX=game.player.x,playerY=game.player.y,cameraPosition=camera.transform.position,authoredIntroActive=ReadabilityIntroR1482(id)});
  }
  IEnumerator SettleReadabilityR1482(string id,int room,float seconds,string phase){
   double began=Time.realtimeSinceStartupAsDouble;float stableSince=-1;int first=Time.frameCount;
   while(true){
    CheckProgress();Need(Time.realtimeSinceStartupAsDouble-began<65,"Original room/intro did not settle: "+id+"/"+phase);
    Need(ReadabilityActiveR1482(id)&&game.room==room&&game.player.hp>0,"Room changed or player died during visual probe");
    Need(!game.paused,"Unexpected pause during visual probe");yield return new WaitForEndOfFrame();SampleReadabilityCameraR1482(id,phase);
    bool ready=Time.frameCount>=first+8&&(int)Field(scene,"shownRoom")==room&&!ReadabilityIntroR1482(id)&&worldFrame==Time.frameCount;
    if(!ready){stableSince=-1;continue;}if(stableSince<0)stableSince=game.elapsed;
    if(game.elapsed-stableSince>=seconds)break;
   }
  }
  static void ReadabilityViewportR1482(Camera camera,Bounds bounds,out Vector3 minimum,out Vector3 maximum){
   minimum=new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);maximum=new Vector3(float.NegativeInfinity,float.NegativeInfinity,float.NegativeInfinity);
   for(int i=0;i<8;i++){var corner=new Vector3((i&1)==0?bounds.min.x:bounds.max.x,(i&2)==0?bounds.min.y:bounds.max.y,(i&4)==0?bounds.min.z:bounds.max.z);var point=camera.WorldToViewportPoint(corner);minimum=Vector3.Min(minimum,point);maximum=Vector3.Max(maximum,point);}
  }
  static bool ReadabilityOverlapR1482(Vector3 a,Vector3 b){return b.z>0&&b.x>0&&a.x<1&&b.y>0&&a.y<1;}
  ReadabilityShotR1482 CaptureReadabilityR1482(string id,string label,bool staged){
   CheckProgress();Need(worldFrame==Time.frameCount,"Actual EOF world-camera draw required");Need(!ReadabilityIntroR1482(id),"Authored intro must finish naturally before gameplay capture");
   var camera=(Camera)Field(scene,"worldCamera");var hero=(SpriteRenderer)Field(scene,"heroView");var depth=(RenderTexture)Field(scene,"nativeSceneEyeDepthR1461");
   Need(camera&&hero&&hero.sprite&&hero.enabled&&hero.gameObject.activeInHierarchy,"Live original hero sprite required");
   Need(depth&&depth.IsCreated()&&depth.format==RenderTextureFormat.RFloat&&(bool)Field(scene,"nativeSceneDepthReadyR1461"),"Live original RFloat scene depth required");
   var row=new ReadabilityShotR1482{route=id,label=label,room=game.CurrentRoom.id,roomIndex=game.room,frame=Time.frameCount,controlledRoomSetup=staged,actualWorldRendered=true,depthReady=true,authoredIntroActive=false,modelTime=game.elapsed,halfHeight=camera.orthographicSize,playerX=game.player.x,playerY=game.player.y,playerHp=game.player.hp,cameraPosition=camera.transform.position,heroSprite=hero.sprite.name,nativeWidth=depth.width,nativeHeight=depth.height};
   ReadabilityViewportR1482(camera,hero.bounds,out row.heroViewportMin,out row.heroViewportMax);row.heroViewportOverlap=ReadabilityOverlapR1482(row.heroViewportMin,row.heroViewportMax);row.heroSpriteViewportHeight=row.heroViewportMax.y-row.heroViewportMin.y;
   Need(row.halfHeight>=134.9f&&row.halfHeight<=(staged?165.01f:135.05f),"Gameplay half-height outside bounded camera policy");
   Need(row.heroViewportOverlap,"Original hero renderer is outside the actual gameplay viewport");
   // Bounds are geometric evidence only. Screenshots require visual review;
   // these gates never declare opaque-pixel visibility or all-Boss containment.
   if(id=="altera-below-tunnel"){
    var views=(IList)Field(scene,"enemyViews");var project=scene.GetType().GetMethod("ProjectNativePoint",Fields,null,new[]{typeof(Vector3),typeof(float)},null);Need(project!=null,"Original native point projection missing");
    for(int i=0;i<game.enemies.Count;i++){
     var enemy=game.enemies[i];if(enemy.sourceNPC!="NUI_NASOD_MINER_THIN_DEFECT_DIE")continue;Need(i<views.Count,"Original miner renderer missing");var view=(SpriteRenderer)views[i];
     var miner=new ReadabilityMinerR1482{index=i,groundLine=enemy.groundLine,npc=enemy.sourceNPC,sourceState=enemy.npcState,combatState=enemy.state.ToString(),sprite=view&&view.sprite?view.sprite.name:"",active=enemy.active,grounded=game.Grounded(enemy),rendererEnabled=view&&view.enabled&&view.gameObject.activeInHierarchy,x=enemy.x,y=enemy.y,nativeDepth=enemy.nativeDepth,hp=enemy.hp,sourceClock=enemy.npcTime};
     miner.modelFootViewport=camera.WorldToViewportPoint((Vector3)project.Invoke(scene,new object[]{new Vector3(enemy.x,enemy.y,0),enemy.nativeDepth}));
     if(view){miner.rendererOrigin=view.transform.position;ReadabilityViewportR1482(camera,view.bounds,out miner.viewportMin,out miner.viewportMax);miner.viewportOverlap=miner.rendererEnabled&&ReadabilityOverlapR1482(miner.viewportMin,miner.viewportMax);}row.miners.Add(miner);
    }
   }
   Texture2D capture=null;try{capture=ScreenCapture.CaptureScreenshotAsTexture();Need(capture&&capture.width==Screen.width&&capture.height==Screen.height,"Actual full-window EOF image required");row.image=Path.Combine(output,label+".png");File.WriteAllBytes(row.image,capture.EncodeToPNG());}finally{if(capture)Destroy(capture);}
   report.readabilityShots.Add(row);return row;
  }
  IEnumerator WalkReadabilityR1482(string id,string direction,float seconds,string label,bool staged){
   TouchControl control=null;foreach(var value in scene.GetComponentsInChildren<TouchControl>(true))if(value&&ReferenceEquals(value.owner,scene)&&value.action==direction){Need(control==null,"Duplicate original movement control");control=value;}
   Need(control,"Original movement control missing");if(!control.isActiveAndEnabled){Press("touch");yield return new WaitForEndOfFrame();}
   Need(control.isActiveAndEnabled&&control.gameObject.activeInHierarchy,"Original touch movement remains hidden");var image=control.GetComponent<Image>();Need(image&&image.raycastTarget,"Original movement is not raycastable");
   var pointer=new PointerEventData(EventSystem.current){pointerId=-1482,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
   float until=game.elapsed+seconds;int room=game.room;report.actions.Add("real-touch-hold:"+id+":"+direction+":"+seconds);
   try{Need(ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Original movement down rejected");while(game.elapsed<until){CheckProgress();Need(game.room==room&&game.player.hp>0&&!game.paused,"Visual movement interrupted by room/death/pause");yield return new WaitForEndOfFrame();SampleReadabilityCameraR1482(id,label);}}
   finally{if(control)ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
   var settle=SettleReadabilityR1482(id,room,.15f,label+"-release");while(settle.MoveNext())yield return settle.Current;CaptureReadabilityR1482(id,label,staged);
  }
  void StageReadabilityRoomR1482(string id,int room){
   Need(ReadabilityActiveR1482(id)&&!game.paused&&game.player.hp>0,"Active original route required for controlled room setup");
   var enter=typeof(GameModel).GetMethod("EnterRoom",Fields,null,new[]{typeof(int)},null);Need(enter!=null,"Sole original room-setup seam missing");
   report.actions.Add("CONTROLLED_ROOM_SETUP:GameModel.EnterRoom("+room+"):"+id+"; no direct actor/HP/AI/clock writes");
   enter.Invoke(game,new object[]{room});Need(game.room==room,"Original controlled room setup failed");
  }
  void CheckReadabilityReturnR1482(string id,GameModel originalGame,Camera originalCamera,RenderTexture oldDepth){
   var row=new ReadabilityReturnR1482{route=id,originalGameRestored=ReferenceEquals(Field(scene,"game"),originalGame),originalCameraRestored=ReferenceEquals(Field(scene,"worldCamera"),originalCamera),oldDepthReleased=!oldDepth||!oldDepth.IsCreated(),sessionInactive=!ReadabilityActiveR1482(id)};
   if(id=="altera-below-tunnel"){var state=scene.InspectBelowPlayerCandidateV21();row.ownerResourcesReleased=!state.active&&!state.loading&&state.ownedObjects==0&&state.ownedLibraries==0&&state.ownedAtlasKeys==0&&state.ownedAudioSlots==0&&Field(scene,"belowPlayerPlanV21")==null&&Field(scene,"belowPlayerDataV21")==null&&Field(scene,"belowPlayerFxV21")==null;}
   else{
    bool released=!scene.CoreHardProductSessionActiveR1147&&!scene.CoreHardProductSessionLoadingR1147&&Field(scene,"coreProductPlanR1147")==null&&Field(scene,"coreEntryScopeV21")==null&&Field(scene,"coreEntrySourceViewV21")==null&&Field(scene,"forestPreviewScenes")==null&&Field(scene,"forestPreviewGeometry")==null&&Field(scene,"forestPreviewStages")==null;
    foreach(var name in new[]{"forestPreviewOwned","forestPreviewLibraries","forestPreviewAtlasKeys","forestPreviewSounds","forestPreviewFiles","forestPreviewHashes","forestPreviewTextures","forestPreviewTextureBindings"})released&=((ICollection)Field(scene,name)).Count==0;
    foreach(var name in new[]{"coreProductReadIndexR1147","coreProductReadOwnerR1147"}){var field=scene.GetType().GetField(name,BindingFlags.Static|BindingFlags.NonPublic);Need(field!=null,"Original Core static resource owner missing");released&=field.GetValue(null)==null;}row.ownerResourcesReleased=released;
   }
   report.readabilityReturns.Add(row);Need(row.originalGameRestored&&row.originalCameraRestored&&row.oldDepthReleased&&row.sessionInactive&&row.ownerResourcesReleased,"Original product owner/resource cleanup incomplete: "+id);
   Need((string)Field(scene,"openMenu")=="dungeons"&&originalGame.paused,"Original paused dungeon menu not restored");
  }
  IEnumerator ReadabilityFlowR1482(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){CheckProgress();yield return new WaitForEndOfFrame();}
   for(int i=0;i<3;i++)yield return new WaitForEndOfFrame();game=(GameModel)Field(scene,"game");
   Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused&&!scene.runAutomatedPlaythrough,"Original ordinary Main menu required");
   var originalGame=game;var originalCamera=(Camera)Field(scene,"worldCamera");
   foreach(var id in new[]{"altera-below-tunnel","altera-core-hard"}){
    var select=Select(id);while(select.MoveNext())yield return select.Current;
    while(!ReadabilityActiveR1482(id)){CheckProgress();yield return new WaitForEndOfFrame();}game=(GameModel)Field(scene,"game");
    Need(game.room==0,"Ordinary menu must enter original room zero");
    var settle=SettleReadabilityR1482(id,0,3.5f,"ordinary-entry");while(settle.MoveNext())yield return settle.Current;
    CaptureReadabilityR1482(id,id+"-ordinary-entry",false);
    var walk=WalkReadabilityR1482(id,"right",1.2f,id+"-ordinary-right",false);while(walk.MoveNext())yield return walk.Current;
    int boss=id=="altera-below-tunnel"?2:4;StageReadabilityRoomR1482(id,boss);
    settle=SettleReadabilityR1482(id,boss,2f,"controlled-boss-original-intro-and-settle");while(settle.MoveNext())yield return settle.Current;
    CaptureReadabilityR1482(id,id+"-CONTROLLED-boss-entry",true);
    // Below's source spawn is more than600 units from Fort's roots. A real
    // short right walk brings the nearby fight into the retained close view.
    walk=WalkReadabilityR1482(id,"right",id=="altera-below-tunnel"?3f:1f,id+"-CONTROLLED-boss-right",true);while(walk.MoveNext())yield return walk.Current;
    walk=WalkReadabilityR1482(id,"left",1f,id+"-CONTROLLED-boss-left",true);while(walk.MoveNext())yield return walk.Current;
    var oldDepth=(RenderTexture)Field(scene,"nativeSceneEyeDepthR1461");Press("dungeons");for(int i=0;i<4;i++)yield return new WaitForEndOfFrame();
    CheckReadabilityReturnR1482(id,originalGame,originalCamera,oldDepth);game=originalGame;
   }
  }
  void Complete(){
   if(finished)return;finished=true;Application.logMessageReceived-=OnLog;Camera.onPostRender-=Rendered;
   // Quit is processed on a subsequent player loop. Keep that loop running
   // while the diagnostic window is unfocused; restore only during teardown.
   int code=report.passed?0:2;try{File.WriteAllText(Path.Combine(output,"player-report.json"),JsonUtility.ToJson(report,true),new System.Text.UTF8Encoding(false));}catch(Exception ex){Debug.LogException(ex);code=2;}finally{Application.Quit(code);}
  }
  void OnApplicationQuit(){if(started>0)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){Application.logMessageReceived-=OnLog;Camera.onPostRender-=Rendered;if(started>0)Application.runInBackground=backgroundBefore;}
 }
}
#endif
