#if ROOT_REDRAW_CHECK_V32
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ElswordPixel {
 public sealed class Background41PlayerAuditV32 : MonoBehaviour {
  internal const string Flag="--root-background41-check-v32";
  const string RouteFlag="--root-background41-route-v32", RoomFlag="--root-background41-room-v32", ViewFlag="--root-background41-view-v32", OutputFlag="--root-background41-output-v32";
  [Serializable] internal sealed class MaterialRow {
   public int index,vertices,textureWidth,textureHeight; public string name,sourceTexture,shader,role;
   public float reduction,radius,steps,flat; public bool enabled,registered,mechanismName,groundOrStairName;
  }
  [Serializable] internal sealed class RoomRow {
   public string route,world,modelRoom,partsSha256,currentWorldPng,bypassWorldPng,currentNativePng,bypassNativePng,group05OffWorldPng,group05OffNativePng,flatWitnessPng,flatPixelStatus,protectedOffPng,coreShellOffPng;
   public int room,frame,parts,registeredFlatCount,nativeWidth,nativeHeight,worldChanged,nativeChanged,group05WorldChanged,group05NativeChanged,alphaChanged,flatWitnessPixels,flatPurePixels,flatDistinctRGB,protectedParts,protectedVisiblePixels,coreShellVisiblePixels,groundLines,groundNodes;
   public float gameClock,playerX,playerY,groundAtPlayer; public Color32 clearColor,expectedColor,flatSampleColor;
   public Vector3 nativeCameraPosition,worldCameraPosition; public bool actualNativeDraw,actualWorldDraw,restoredPixels,authoredCoreCamera,captureComplete,passed;
   public List<string> currentMechanismActors=new List<string>(),issues=new List<string>();
   public List<MaterialRow> materials=new List<MaterialRow>();
  }
  [Serializable] internal sealed class Report {
   public string schema="root-background41-gpu-audit-v32",graphics,requestedRoute,view,failure;
   public int excludedAcceptedRooms=21,eligibleRooms=41,requestedRooms,completedRooms,originalCoreCameraDraws,originalFactoryCameraDraws;
   public bool batchMode,offscreenStandaloneGPU,windowPresentationVerified=false,controlledRoomEntry=true,naturalRouteClaim=false,visualReadabilityReviewed=false,passed;
   public List<string> menuActions=new List<string>(),returnedRoutes=new List<string>(),errors=new List<string>();
   public List<RoomRow> rooms=new List<RoomRow>();
  }
  internal readonly Report report=new Report(); internal readonly List<Background41RoomV32> selected=new List<Background41RoomV32>();
  internal string output; DemoScene scene; double started; bool finished,previousBackground;
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   string[] args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,Flag)<0)return;
   try {
    Need(Array.FindAll(args,v=>v==Flag).Length==1,"One explicit audit flag required");
    Need(!Application.isEditor&&SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null,"Standalone GPU required");
    string path=Path.GetFullPath(Argument(args,OutputFlag,true));string route=Argument(args,RouteFlag,true);string roomArg=Argument(args,RoomFlag,false);string view=Argument(args,ViewFlag,false)??"spawn";
    Need(view=="spawn"||view=="middle","View must be spawn or middle");
    Need(path.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh runtime-drafts output required");
    int room=-1;if(roomArg!=null)Need(route!="all"&&int.TryParse(roomArg,out room)&&room>=0&&roomArg==room.ToString(CultureInfo.InvariantCulture),"Single canonical zero-based room within one route required");
    var targets=Array.FindAll(Background41RoomV32.All,r=>(route=="all"||r.route==route)&&(room<0||r.room==room));Need(targets.Length>0,"Requested scope outside the 41 new rooms");
    Directory.CreateDirectory(path);var owner=new GameObject("Explicit 41-room native background audit V32").AddComponent<Background41PlayerAuditV32>();
    owner.output=path;owner.selected.AddRange(targets);owner.report.requestedRoute=route;owner.report.requestedRooms=targets.Length;owner.report.view=view;
   } catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
  }
  static string Argument(string[] args,string key,bool required){
   int i=Array.IndexOf(args,key);if(i<0){Need(!required,"Missing "+key);return null;}
   Need(Array.LastIndexOf(args,key)==i&&i+1<args.Length&&!args[i+1].StartsWith("--",StringComparison.Ordinal),"Invalid "+key);return args[i+1];
  }
  internal static void Need(bool ok,string message){if(!ok)throw new InvalidOperationException("Background41: "+message);}
  void Errors(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert)report.errors.Add(message+"\n"+stack);}
  internal void Check(){Need(report.errors.Count==0,"Runtime error; see report");Need(Time.realtimeSinceStartupAsDouble-started<120+selected.Count*120,"Audit wall deadline");}
  internal void Save(){File.WriteAllText(Path.Combine(output,"report.json"),JsonUtility.ToJson(report,true),new UTF8Encoding(false));}
  IEnumerator Start(){
   previousBackground=Application.runInBackground;Application.runInBackground=true;started=Time.realtimeSinceStartupAsDouble;
   report.graphics=SystemInfo.graphicsDeviceType.ToString();report.batchMode=Application.isBatchMode;report.offscreenStandaloneGPU=true;Application.logMessageReceived+=Errors;Save();
   var steps=new Stack<IEnumerator>();steps.Push(Run());Exception failure=null;
   while(steps.Count>0&&failure==null){
    object next=null;bool advance=false;
    try{Check();if(scene)scene.Background41OriginalOwnerDraw(this);var current=steps.Peek();advance=current.MoveNext();if(advance)next=current.Current;else{(steps.Pop() as IDisposable)?.Dispose();}}
    catch(Exception ex){failure=ex;}
    if(failure!=null)break;if(!advance)continue;var child=next as IEnumerator;if(child!=null)steps.Push(child);else yield return next;
   }
   while(steps.Count>0)(steps.Pop() as IDisposable)?.Dispose();
   if(failure!=null)report.failure=failure.ToString();report.passed=failure==null&&report.errors.Count==0&&report.rooms.Count==selected.Count&&report.rooms.TrueForAll(r=>r.passed)&&report.returnedRoutes.Count>0;
   finished=true;Save();Application.logMessageReceived-=Errors;Application.runInBackground=previousBackground;Application.Quit(report.passed?0:2);
  }
  IEnumerator Run(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Check();yield return null;}
   for(int i=0;i<3;i++)yield return null;
   yield return scene.Background41Run(this);
  }
  void OnDestroy(){if(!finished&&started>0){Application.logMessageReceived-=Errors;Application.runInBackground=previousBackground;}}
 }

 public sealed partial class DemoScene {
  internal void Background41OriginalOwnerDraw(Background41PlayerAuditV32 audit){
   bool coreOwner=CoreHardProductSessionActiveR1147&&CoreHardProductAuthoredCameraActiveR1147;
   bool factoryOwner=FactoryProductSessionActiveR1179&&factoryRavenTicketV21!=null;
   if(!coreOwner&&!factoryOwner)return;
   Background41PlayerAuditV32.Need(worldCamera&&worldCamera.enabled&&worldCamera.targetTexture==null,"Original source-owned screen camera required");
   var active=RenderTexture.active;float elapsed=game.elapsed;
   try{worldCamera.Render();if(coreOwner)audit.report.originalCoreCameraDraws++;else audit.report.originalFactoryCameraDraws++;}
   finally{RenderTexture.active=active;}
   Background41PlayerAuditV32.Need(game.elapsed==elapsed&&worldCamera.targetTexture==null,"Rendering the source camera cannot advance model time");
  }
  void Background41Press(Background41PlayerAuditV32 audit,string action){
   TouchControl button=null;foreach(var control in GetComponentsInChildren<TouchControl>(true)){
    if(!control||!control.isActiveAndEnabled||!control.gameObject.activeInHierarchy||control.owner!=this||control.action!=action)continue;
    Background41PlayerAuditV32.Need(!button,"Duplicate control "+action);button=control;
   }
   Background41PlayerAuditV32.Need(button&&EventSystem.current,"Active menu control "+action);var ui=button.GetComponent<Image>();Background41PlayerAuditV32.Need(ui&&ui.raycastTarget,"Raycastable control "+action);
   var pointer=new PointerEventData(EventSystem.current){pointerId=-3206,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,button.transform.position)};
   audit.report.menuActions.Add(action);try{Background41PlayerAuditV32.Need(ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Rejected control "+action);}finally{if(button)ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
  }
  IEnumerator Background41Enter(Background41PlayerAuditV32 audit,string route){
   int index=mapDungeons.FindIndex(d=>d.id==route);Background41PlayerAuditV32.Need(index>=0&&openMenu=="dungeons","Ordinary map route "+route);
   Background41Press(audit,"region:"+mapDungeons[index].region);yield return null;int at=mapVisible.IndexOf(index);Background41PlayerAuditV32.Need(at>=0,"Visible route "+route);
   int page=at/MapPageSize;for(int i=0;mapPage!=page;i++){Background41PlayerAuditV32.Need(i<20,"Page navigation bound");Background41Press(audit,"dungeon-page:"+(mapPage<page?"1":"-1"));yield return null;}
   Background41Press(audit,"dungeon-card:"+(at%MapPageSize));yield return null;Background41PlayerAuditV32.Need(mapSelected==index,"Selected intended card");Background41Press(audit,"dungeon-enter");
   var first=Array.Find(Background41RoomV32.All,s=>s.route==route&&s.room==0);double until=Time.realtimeSinceStartupAsDouble+110;
   while(openMenu!=""||game==null||game.paused||!NativeWorldActive||nativeScene.id!=first.world){audit.Check();Background41PlayerAuditV32.Need(Time.realtimeSinceStartupAsDouble<until,"Route admission/world timeout "+route+" current="+(nativeScene==null?"null":nativeScene.id));yield return null;}
  }
  internal IEnumerator Background41Run(Background41PlayerAuditV32 audit){
   var originalGame=game;var originalCamera=worldCamera;Background41PlayerAuditV32.Need(originalGame!=null&&originalGame.paused&&openMenu=="dungeons","Original Main dungeon menu");
   var enter=typeof(GameModel).GetMethod("EnterRoom",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(int)},null);Background41PlayerAuditV32.Need(enter!=null,"Original GameModel room entry");
   int index=0;while(index<audit.selected.Count){string route=audit.selected[index].route;yield return Background41Enter(audit,route);
    while(index<audit.selected.Count&&audit.selected[index].route==route){var expected=audit.selected[index++];
     if(game.room!=expected.room){
      enter.Invoke(game,new object[]{expected.room});
      // The fixture enters a room between engine updates. Finish the normal
      // host-side room handoff now, before the next ordinary GameModel.Step.
      // In Recycle this binds the current native terrain for the original mines.
      if(shownRoom!=game.room)RebuildRoom();
     }
     double until=Time.realtimeSinceStartupAsDouble+60;int stable=0;
     while(stable<3){audit.Check();Background41PlayerAuditV32.Need(Time.realtimeSinceStartupAsDouble<until,"Room load timeout "+expected.world);yield return null;
      stable=game.room==expected.room&&NativeWorldActive&&nativeScene.id==expected.world&&nativeRoot&&nativeTarget&&nativeTarget.IsCreated()?stable+1:0;}
     if(audit.report.view=="middle"){
      // A separate opt-in viewpoint uses the live route camera and normal
      // room constraints. Its recorded player/camera coordinates are actual.
      float middle=game.RoomWidth*.5f;game.player.x=middle;game.player.y=game.GroundAt(middle);game.player.vx=game.player.vy=0;
      for(int settle=0;settle<30;settle++){audit.Check();yield return null;Background41PlayerAuditV32.Need(game.room==expected.room&&NativeWorldActive&&nativeScene.id==expected.world,"Middle viewpoint changed rooms");}
     }
     var row=new Background41PlayerAuditV32.RoomRow{route=route,room=expected.room,world=expected.world};audit.report.rooms.Add(row);audit.Save();
     Background41Capture(audit,expected,row);row.captureComplete=true;row.passed=row.issues.Count==0&&row.actualNativeDraw&&row.actualWorldDraw&&row.restoredPixels;audit.report.completedRooms++;audit.Save();
    }
    double returnUntil=Time.realtimeSinceStartupAsDouble+45;
    // Action("dungeons") is ignored during the ordinary boss introduction.
    // Wait for that same player-input gate before issuing the real touch.
    while(BossIntroductionActive){audit.Check();Background41PlayerAuditV32.Need(Time.realtimeSinceStartupAsDouble<returnUntil,"Boss introduction did not release menu input for "+route);yield return null;}
    Background41Press(audit,"dungeons");
    while(openMenu!="dungeons"||!ReferenceEquals(game,originalGame)||worldCamera!=originalCamera){audit.Check();Background41PlayerAuditV32.Need(Time.realtimeSinceStartupAsDouble<returnUntil,"Return to Main failed for "+route+" menu="+openMenu+" originalModel="+ReferenceEquals(game,originalGame)+" originalCamera="+(worldCamera==originalCamera));yield return null;}
    audit.report.returnedRoutes.Add(route);audit.Save();
   }
  }

  static bool Background41Has(string text,params string[] words){foreach(string word in words)if(text.IndexOf(word,StringComparison.OrdinalIgnoreCase)>=0)return true;return false;}
  static bool Background41Mechanism(string text){return Background41Has(text,"trap","gate","press","lever","switch","valve","cannon","gear","geyser","rail");}
  static bool Background41Ground(string text){return Background41Has(text,"ground","floor","platform","footing","road","step","stair","tile");}
  static string Background41Digest(string value){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-","").ToLowerInvariant();}
  static void Background41Issue(Background41PlayerAuditV32.RoomRow row,bool ok,string issue){if(!ok)row.issues.Add(issue);}
  List<MeshRenderer> Background41Inspect(Background41RoomV32 expected,Background41PlayerAuditV32.RoomRow row){
   var flats=new List<MeshRenderer>();var signature=new StringBuilder();row.parts=nativeScene.parts.Length;
   Background41PlayerAuditV32.Need(nativeRoot.childCount>=row.parts&&row.parts==expected.count,"Original part count "+expected.world);
   row.clearColor=nativeCamera.backgroundColor;row.expectedColor=expected.color;
   // Some production directors override the clear RGB. Preserve and report
   // that camera output independently of the explicitly registered mesh color.
   for(int i=0;i<row.parts;i++){
    var part=nativeScene.parts[i];var child=nativeRoot.GetChild(i);var renderer=child.GetComponent<MeshRenderer>();var filter=child.GetComponent<MeshFilter>();
    Background41PlayerAuditV32.Need(renderer&&filter&&filter.sharedMesh&&child.name==part.name&&filter.sharedMesh.vertexCount==part.count,"Live original mesh mismatch "+expected.world+"/"+i);
    signature.Append(i).Append('\t').Append(part.name).Append('\t').Append(part.texture).Append('\t').Append(part.offset).Append('\t').Append(part.count).Append('\n');
    var material=renderer.sharedMaterial;Background41PlayerAuditV32.Need(material&&material.shader&&material.shader.isSupported,"Original scene material "+i);var texture=material.mainTexture as Texture2D;
    var role=SceneBackgroundStyleV25.Classify(part.name,part.texture,texture?texture.width:0,texture?texture.height:0).role;
    var match=Array.Find(expected.flat,f=>f.index==i);var m=new Background41PlayerAuditV32.MaterialRow{index=i,vertices=part.count,name=part.name,sourceTexture=part.texture,shader=material.shader.name,role=role,enabled=renderer.enabled&&child.gameObject.activeInHierarchy,registered=match!=null,textureWidth=texture?texture.width:0,textureHeight=texture?texture.height:0,mechanismName=Background41Mechanism(part.name??""),groundOrStairName=Background41Ground(part.name??"")};row.materials.Add(m);
    if(!material.HasProperty("_DetailReduction")||!material.HasProperty("_FlatFarEnabled")){row.issues.Add("Missing scenery policy properties on live part "+i+" / "+m.shader);continue;}
    m.reduction=material.GetFloat("_DetailReduction");m.radius=material.GetFloat("_DetailRadius");m.steps=material.GetFloat("_ColorSteps");m.flat=material.GetFloat("_FlatFarEnabled");
    float reduction=0,radius=0,steps=16;
    switch(role){case "foliage":reduction=.4f+(.95f-.4f)*4/9;radius=1.5f+(4-1.5f)*4/9;steps=8;break;
     case "surface":reduction=.12f+(.55f-.12f)*4/9;radius=1+(2f-1)*4/9;break;
     case "scenery":reduction=.25f+(.9f-.25f)*4/9;radius=1.25f+(4-1.25f)*4/9;steps=10;break;}
    Background41Issue(row,Math.Abs(m.reduction-reduction)<.00002f&&Math.Abs(m.radius-radius)<.00002f&&Math.Abs(m.steps-steps)<.00002f,"Wrong live group05/functional material on part "+i+" / "+role);
    Background41Issue(row,(m.flat>.5f)==(match!=null),"Exact flat registration mismatch on part "+i);
    if(match!=null){Background41Issue(row,match.name==part.name&&match.texture==part.texture,"Registered source tuple mismatch "+i);var color=(Color32)material.GetColor("_FlatFarColor");Background41Issue(row,color.r==expected.color.r&&color.g==expected.color.g&&color.b==expected.color.b,"Wrong registered flat RGB on part "+i);flats.Add(renderer);}
   }
   row.partsSha256=Background41Digest(signature.ToString());Background41Issue(row,row.partsSha256==expected.partsSha256,"Ordered original part signature mismatch");row.registeredFlatCount=flats.Count;Background41Issue(row,flats.Count==expected.flat.Length,"Registered flat count mismatch");return flats;
  }
  static Texture2D Background41Read(RenderTexture target){var previous=RenderTexture.active;var t=new Texture2D(target.width,target.height,TextureFormat.RGBA32,false,false);
   try{RenderTexture.active=target;t.ReadPixels(new Rect(0,0,target.width,target.height),0,0,false);t.Apply(false,false);return t;}catch{UnityEngine.Object.Destroy(t);throw;}finally{RenderTexture.active=previous;}}
  static Color32[] Background41Pixels(RenderTexture target,string save){Texture2D t=null;try{t=Background41Read(target);if(save!=null)File.WriteAllBytes(save,t.EncodeToPNG());return t.GetPixels32();}finally{if(t)UnityEngine.Object.Destroy(t);}}
  static int Background41Delta(Color32[] a,Color32[] b,int threshold=0){Background41PlayerAuditV32.Need(a.Length==b.Length,"Comparable pixel buffers");int count=0;for(int i=0;i<a.Length;i++)if(Math.Abs(a[i].r-b[i].r)>threshold||Math.Abs(a[i].g-b[i].g)>threshold||Math.Abs(a[i].b-b[i].b)>threshold)count++;return count;}
  static int Background41RGB(Color32 c){return c.r<<16|c.g<<8|c.b;}
  static int Background41Colors(Color32[] pixels,int stop){var colors=new HashSet<int>();foreach(var pixel in pixels){colors.Add(Background41RGB(pixel));if(colors.Count>=stop)break;}return colors.Count;}
  static void Background41Draw(Camera camera){bool observed=false;Camera.CameraCallback callback=c=>{if(c==camera)observed=true;};Camera.onPostRender+=callback;try{camera.Render();}finally{Camera.onPostRender-=callback;}Background41PlayerAuditV32.Need(observed,"Source camera post-render required: "+camera.name);}
  Color32[] Background41World(RenderTexture target,string save){var old=worldCamera.targetTexture;var active=RenderTexture.active;var rect=worldCamera.rect;float aspect=worldCamera.aspect;
   try{worldCamera.targetTexture=target;worldCamera.rect=new Rect(0,0,1,1);worldCamera.aspect=target.width/(float)target.height;Background41Draw(worldCamera);return Background41Pixels(target,save);}
   finally{worldCamera.targetTexture=old;worldCamera.rect=rect;worldCamera.aspect=aspect;RenderTexture.active=active;}}
  void Background41Capture(Background41PlayerAuditV32 audit,Background41RoomV32 expected,Background41PlayerAuditV32.RoomRow row){
   row.modelRoom=game.CurrentRoom.id;row.frame=Time.frameCount;row.gameClock=game.elapsed;row.playerX=game.player.x;row.playerY=game.player.y;row.groundAtPlayer=game.GroundAt(game.player.x);
   row.groundLines=CurrentStageArt.surfaces==null?0:CurrentStageArt.surfaces.Length;row.groundNodes=CurrentStageArt.groundNodes==null?0:CurrentStageArt.groundNodes.Length;
   for(int i=0;i<game.enemies.Count;i++){var e=game.enemies[i];if(!e.isGate&&!Background41Mechanism((e.kind??"")+"/"+(e.sourceNPC??"")))continue;
    var body=i<enemyViews.Count?enemyViews[i]:null;bool enabled=body&&body.enabled&&!body.forceRenderingOff&&body.gameObject.activeInHierarchy;bool inView=false;
    if(enabled){var lo=worldCamera.WorldToViewportPoint(body.bounds.min);var hi=worldCamera.WorldToViewportPoint(body.bounds.max);inView=hi.x>=0&&hi.y>=0&&lo.x<=1&&lo.y<=1;}
    row.currentMechanismActors.Add(e.kind+"/"+e.sourceNPC+" active="+e.active+" dormant="+e.dormant+" rendererEnabled="+enabled+" viewportOverlap="+inView);
   }
   row.authoredCoreCamera=CoreHardProductAuthoredCameraActiveR1147;row.nativeCameraPosition=nativeCamera.transform.position;row.worldCameraPosition=worldCamera.transform.position;row.nativeWidth=nativeTarget.width;row.nativeHeight=nativeTarget.height;
   Background41PlayerAuditV32.Need(nativeCamera.targetTexture==nativeTarget&&nativeScreen.GetComponent<MeshRenderer>().sharedMaterial.mainTexture==nativeTarget,"Original native render-to-world binding");
   var flats=Background41Inspect(expected,row);var folder=Path.Combine(audit.output,expected.route,expected.room.ToString("00")+"-"+expected.world);Directory.CreateDirectory(folder);
   row.currentWorldPng=Path.Combine(folder,"world-current.png");row.bypassWorldPng=Path.Combine(folder,"world-shader-bypass.png");row.currentNativePng=Path.Combine(folder,"native-current.png");row.bypassNativePng=Path.Combine(folder,"native-shader-bypass.png");
   float global=Shader.GetGlobalFloat("_SceneDetailReviewOriginal");Background41PlayerAuditV32.Need(global<.5f,"Production scene review switch unexpectedly enabled");
   var savedColors=new List<Color>();foreach(var renderer in flats)savedColors.Add(renderer.sharedMaterial.GetColor("_FlatFarColor"));
   var details=new Dictionary<Material,float>();foreach(var part in row.materials)if(part.reduction>0){var material=nativeRoot.GetChild(part.index).GetComponent<MeshRenderer>().sharedMaterial;details[material]=part.reduction;}
   var protectedRenderers=new List<MeshRenderer>();foreach(var part in row.materials)if(part.enabled&&!part.registered&&(part.role=="surface"||part.role=="functional"||part.groundOrStairName||part.mechanismName))protectedRenderers.Add(nativeRoot.GetChild(part.index).GetComponent<MeshRenderer>());row.protectedParts=protectedRenderers.Count;
   var cameraTarget=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32){filterMode=FilterMode.Point,antiAliasing=1};cameraTarget.Create();
   MeshRenderer shell=expected.coreShell<0?null:nativeRoot.GetChild(expected.coreShell).GetComponent<MeshRenderer>();bool shellEnabled=shell&&shell.enabled;
   try {
    Background41Draw(nativeCamera);row.actualNativeDraw=true;var nativeCurrent=Background41Pixels(nativeTarget,row.currentNativePng);var worldCurrent=Background41World(cameraTarget,row.currentWorldPng);row.actualWorldDraw=true;
    Background41Issue(row,Background41Colors(worldCurrent,8)>=8&&Background41Colors(nativeCurrent,3)>=3,"Production view has too few RGB colors");
    SceneBackgroundStyleV25.CompareOriginalMaterials(true);Background41Draw(nativeCamera);var nativeBypass=Background41Pixels(nativeTarget,row.bypassNativePng);var worldBypass=Background41World(cameraTarget,row.bypassWorldPng);
    row.nativeChanged=Background41Delta(nativeCurrent,nativeBypass,1);row.worldChanged=Background41Delta(worldCurrent,worldBypass,1);SceneBackgroundStyleV25.CompareOriginalMaterials(false);
    foreach(var pair in details)pair.Key.SetFloat("_DetailReduction",0);Background41Draw(nativeCamera);row.group05OffNativePng=Path.Combine(folder,"native-group05-off.png");row.group05OffWorldPng=Path.Combine(folder,"world-group05-off.png");
    var onlyDetailOff=Background41Pixels(nativeTarget,row.group05OffNativePng);row.group05NativeChanged=Background41Delta(nativeCurrent,onlyDetailOff,1);row.group05WorldChanged=Background41Delta(worldCurrent,Background41World(cameraTarget,row.group05OffWorldPng),1);
    for(int i=0;i<nativeCurrent.Length;i++)if(nativeCurrent[i].a!=onlyDetailOff[i].a||nativeCurrent[i].a!=nativeBypass[i].a)row.alphaChanged++;
    Background41Issue(row,row.alphaChanged==0,"Native geometry coverage changed with color-only treatment");foreach(var pair in details)pair.Key.SetFloat("_DetailReduction",pair.Value);
    row.flatPixelStatus="no registered far part in room";
    if(flats.Count>0){foreach(var renderer in flats)renderer.sharedMaterial.SetColor("_FlatFarColor",Color.magenta);Background41Draw(nativeCamera);row.flatWitnessPng=Path.Combine(folder,"native-registered-far-witness.png");var marked=Background41Pixels(nativeTarget,row.flatWitnessPng);var flatColors=new HashSet<int>();
     for(int i=0;i<marked.Length;i++){if(Math.Abs(marked[i].r-nativeCurrent[i].r)<5&&Math.Abs(marked[i].g-nativeCurrent[i].g)<5&&Math.Abs(marked[i].b-nativeCurrent[i].b)<5)continue;row.flatWitnessPixels++;if(marked[i].r<254||marked[i].g>1||marked[i].b<254)continue;row.flatPurePixels++;flatColors.Add(Background41RGB(nativeCurrent[i]));row.flatSampleColor=nativeCurrent[i];}
     row.flatDistinctRGB=flatColors.Count;row.flatPixelStatus=row.flatWitnessPixels==0?"registered but outside sampled view or fully occluded":row.flatPurePixels==0?"visible through blended foreground only":row.flatDistinctRGB==1?"visible registered surface has one RGB":"visible registered surface has multiple RGB values";
     Background41Issue(row,row.flatDistinctRGB<=1,"Unoccluded registered far surface has multiple output RGB values; inspect native pair");
     for(int i=0;i<flats.Count;i++)flats[i].sharedMaterial.SetColor("_FlatFarColor",savedColors[i]);
    }
    if(shell){shell.enabled=false;Background41Draw(nativeCamera);row.coreShellOffPng=Path.Combine(folder,"native-unregistered-core-shell-off.png");row.coreShellVisiblePixels=Background41Delta(nativeCurrent,Background41Pixels(nativeTarget,row.coreShellOffPng),1);shell.enabled=shellEnabled;}
    if(protectedRenderers.Count>0){foreach(var renderer in protectedRenderers)renderer.enabled=false;Background41Draw(nativeCamera);row.protectedOffPng=Path.Combine(folder,"native-ground-technical-mechanism-off.png");row.protectedVisiblePixels=Background41Delta(nativeCurrent,Background41Pixels(nativeTarget,row.protectedOffPng),1);foreach(var renderer in protectedRenderers)renderer.enabled=true;}
    Background41Draw(nativeCamera);var restored=Background41Pixels(nativeTarget,null);var worldRestored=Background41World(cameraTarget,null);row.restoredPixels=Background41Delta(nativeCurrent,restored)==0&&Background41Delta(worldCurrent,worldRestored)==0;
    Background41Issue(row,game.elapsed==row.gameClock&&Time.frameCount==row.frame&&game.room==expected.room,"Scene or clock advanced during same-frame comparison");
   }finally{
    foreach(var renderer in protectedRenderers)renderer.enabled=true;foreach(var pair in details)pair.Key.SetFloat("_DetailReduction",pair.Value);for(int i=0;i<flats.Count;i++)flats[i].sharedMaterial.SetColor("_FlatFarColor",savedColors[i]);if(shell)shell.enabled=shellEnabled;Shader.SetGlobalFloat("_SceneDetailReviewOriginal",global);
    try{if(nativeCamera&&nativeCamera.targetTexture==nativeTarget)nativeCamera.Render();}finally{cameraTarget.Release();UnityEngine.Object.Destroy(cameraTarget);}
   }
  }
 }
}
#endif
