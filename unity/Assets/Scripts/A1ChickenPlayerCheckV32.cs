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
 // Explicit component fixture only. Ordinary builds without the flag never
 // attach, navigate, move the player, apply damage, capture, or quit.
 public sealed class A1ChickenPlayerCheckV32 : MonoBehaviour {
  const string Flag="--root-a1-chicken-check-v32",OutFlag="--root-a1-chicken-output-v32";
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
  const double Budget=240;
  const int RenderWidth=1280,RenderHeight=720;
  [Serializable] public sealed class FrameRow {
   public int frame,index,facing,drawnParts,changedBodyPixels,pixelWidth,pixelHeight;
   public string label,pose,modelClip,sourceAlias,sourceSprite,image,onImage,offImage;
   public int sourceFrame;public float hostClock,sourceClock,phase,fallLaunchPhase,hp,deathTime,alpha,visibleTop,barY,statusY,barGap,statusGap;
   public bool sixParts,carrierHidden,worldPost,viewportOverlap,pixelsProven,labelVisible,sourceClockMatches,airFallSeamChecked;
   public float airFallLocalSeamError,leftSoleY,rightSoleY;
   public Bounds bounds;public Vector3 viewportMin,viewportMax,modelFoot,projectedFoot;
   public Vector3[] localPositions,localScales;public Quaternion[] localRotations;
  }
  [Serializable] public sealed class RatioRow {
   public bool measured,sameGround,passed;public float heroY,chickenY,expectedRatio=.965195822132461f,actualPixelRatio,registrationHeight,unitsPerArtPixel;
   public int heroPixelsHigh,chickenPixelsHigh;public string worldView,heroOn,heroOff;
  }
  [Serializable] public sealed class Report {
   public string schema="A1_CHICKEN_TEN_STATE_ACTUAL_PART_DRAW_V32",failure="",graphics;
   public bool passed,ordinaryTreeMenuEntry,returnedMenu,controlledPlayerPlacement=true,playerProtectionForFixture=true,naturalRouteClaim=false,enemyStateClockHpAiAssigned=false,productionMechanismsModified=false,repeatedHurtVerified,leftWalk,rightWalk,leftAttack,rightAttack,deathFadeVerified;
   public bool windowPresentationVerified=false,offscreenStandaloneGPU=true,originalWorldCameraReadback=true,captureViewportFixed=true,batchMode;
   public int captureWidth=RenderWidth,captureHeight=RenderHeight,reportedScreenWidth,reportedScreenHeight;
   public List<string> actions=new List<string>(),errors=new List<string>(),observed=new List<string>();public List<FrameRow> frames=new List<FrameRow>();public RatioRow ratio=new RatioRow();
  }
  sealed class PixelResult {internal int count,width,height;internal string on,off;}
  readonly Dictionary<SpriteRenderer,int> drawn=new Dictionary<SpriteRenderer,int>();
  readonly List<SpriteRenderer> parts=new List<SpriteRenderer>();
  DemoScene scene;GameModel game;Enemy target;ChickenAuthoredPartsV31 owner;Report report=new Report();
  string output;bool finished,pairRender,backgroundBefore;double started;int worldFrame=-1;FrameRow previous;RenderTexture observationTarget;

#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int flags=0,outputs=0;string path=null;
   for(int i=0;i<args.Length;i++){if(args[i]==Flag)flags++;if(args[i]==OutFlag){outputs++;if(i+1<args.Length)path=args[++i];}}
   if(flags==0)return;
   try{
    Need(flags==1&&outputs==1&&!string.IsNullOrEmpty(path),"Exactly one explicit check/output pair");Need(!Application.isEditor&&SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null,"GPU standalone required; batchmode with D3D11 is supported");
    string full=Path.GetFullPath(path);Need(full.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase),"Owned C runtime-drafts output required");Need(!Directory.Exists(full)&&!File.Exists(full),"Fresh output required");
    foreach(string arg in args)Need(arg!="--root-core-reaction-check-v32","Do not combine mutually driving fixtures");
    Directory.CreateDirectory(full);new GameObject("Explicit A1 chicken actual parts check V32").AddComponent<A1ChickenPlayerCheckV32>().output=full;
   }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
  }
  static void Need(bool ok,string why){if(!ok)throw new InvalidOperationException("A1 chicken check V32: "+why);}
  static object Field(object obj,string name){var f=obj.GetType().GetField(name,Fields);Need(f!=null,"Missing field "+name);return f.GetValue(obj);}
  static object Call(object obj,string name,params object[] args){var m=obj.GetType().GetMethod(name,Fields);Need(m!=null,"Missing method "+name);return m.Invoke(obj,args);}
  void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert||message.Contains("CHICKEN_A1_")&&message.Contains("FALLBACK"))report.errors.Add(message+"\n"+stack);}
  void PostRender(Camera camera){if(!pairRender&&scene&&ReferenceEquals(camera,Field(scene,"worldCamera")))worldFrame=Time.frameCount;}
  void Check(){Need(report.errors.Count==0,"Runtime error/fallback recorded");Need(Time.realtimeSinceStartupAsDouble-started<Budget,"Wall deadline");}
  IEnumerator Start(){
   started=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;Application.runInBackground=true;report.graphics=SystemInfo.graphicsDeviceType.ToString();report.batchMode=Application.isBatchMode;report.reportedScreenWidth=Screen.width;report.reportedScreenHeight=Screen.height;Application.logMessageReceived+=Log;Camera.onPostRender+=PostRender;
   var flow=Flow();Exception error=null;
   while(!finished){bool more=false;object next=null;try{more=flow.MoveNext();if(more)next=flow.Current;}catch(Exception e){error=e;}if(error!=null||!more)break;yield return next;}
   (flow as IDisposable)?.Dispose();if(finished)yield break;if(error!=null)report.failure=error.ToString();report.passed=error==null&&report.errors.Count==0;Complete();
  }
  void Update(){if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>=Budget){report.failure="Wall deadline";report.passed=false;Complete();}}
  void Press(string action){
   TouchControl control=null;foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy&&ReferenceEquals(c.owner,scene)&&c.action==action){Need(!control,"Duplicate control "+action);control=c;}
   Need(control&&EventSystem.current,"Active ordinary control "+action);var image=control.GetComponent<Image>();Need(image&&image.raycastTarget,"Raycastable ordinary control");
   var pointer=new PointerEventData(EventSystem.current){pointerId=-3202,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
   report.actions.Add("real-touch:"+action);try{Need(ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Pointer down accepted");}finally{if(control)ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
  }
  IEnumerable EnterTree(){
   var routes=(IList)Field(scene,"mapDungeons");int route=-1;for(int i=0;i<routes.Count;i++)if((string)Field(routes[i],"id")=="tree")route=i;Need(route>=0,"Ordinary Tree route");
   Press("region:ruben");yield return null;var visible=(IList)Field(scene,"mapVisible");int index=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==route)index=i;Need(index>=0,"Visible Tree card");
   int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();int page=index/size,attempt=0;
   while((int)Field(scene,"mapPage")!=page){Need(++attempt<20,"Menu page bound");Press("dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1"));yield return null;}
   Press("dungeon-card:"+(index%size));yield return null;Need((int)Field(scene,"mapSelected")==route,"Selected original Tree card");Press("dungeon-enter");report.ordinaryTreeMenuEntry=true;
  }
  bool ResolveParts(){
   parts.Clear();if(!scene||target==null||game==null)return false;var views=(IDictionary)Field(scene,"chickenA1ViewsV31");if(!views.Contains(target))return false;
   owner=Field(views[target],"parts") as ChickenAuthoredPartsV31;if(owner==null)return false;owner.VisitRenderersR1461(r=>parts.Add(r));return parts.Count==6;
  }
  void RenderObservation(){
   if(!ResolveParts()){
    int i=game.enemies.IndexOf(target);var v=i>=0?((IList)Field(scene,"enemyViews"))[i] as SpriteRenderer:null;
    Need(false,"Six A1 renderers before world-camera render: "+target.npcState+"/"+target.clip+"/"+target.state+" displayed="+(v&&v.sprite?v.sprite.name:"none"));
   }
   foreach(var part in parts){Need(part,"Live A1 part");var witness=part.GetComponent<A1ChickenPartDrawWitnessV32>();if(!witness)witness=part.gameObject.AddComponent<A1ChickenPartDrawWitnessV32>();witness.Bind(this,part);}
   int width=RenderWidth,height=RenderHeight;
   if(!observationTarget||observationTarget.width!=width||observationTarget.height!=height){if(observationTarget){observationTarget.Release();Destroy(observationTarget);}observationTarget=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(observationTarget.Create(),"Observation GPU target");}
   var camera=(Camera)Field(scene,"worldCamera");var oldTarget=camera.targetTexture;var oldActive=RenderTexture.active;Rect oldRect=camera.rect;float oldAspect=camera.aspect;float elapsed=game.elapsed;string actor=JsonUtility.ToJson(target);
   // Hidden standalone windows may never signal WaitForEndOfFrame. After
   // yield-null, DemoScene.Update has produced the actual scene transforms;
   // explicitly draw that unchanged original camera and witness all six parts.
   try{pairRender=false;camera.targetTexture=observationTarget;camera.rect=new Rect(0,0,1,1);camera.aspect=(float)RenderWidth/RenderHeight;RenderTexture.active=observationTarget;GL.Clear(true,true,Color.clear);camera.Render();}
   finally{camera.targetTexture=oldTarget;camera.rect=oldRect;camera.aspect=oldAspect;RenderTexture.active=oldActive;}
   Need(game.elapsed==elapsed&&JsonUtility.ToJson(target)==actor,"Observation render cannot step or mutate actor");
  }
  internal void Draw(SpriteRenderer part,Camera camera){if(!pairRender&&!finished&&scene&&ReferenceEquals(camera,Field(scene,"worldCamera")))drawn[part]=Time.frameCount;}
  static Vector3 CaptureViewportPoint(Camera camera,Vector3 point){float aspect=camera.aspect;try{camera.aspect=(float)RenderWidth/RenderHeight;return camera.WorldToViewportPoint(point);}finally{camera.aspect=aspect;}}
  FrameRow Observe(){
   RenderObservation();int index=game.enemies.IndexOf(target);Need(index==0&&target.kind=="chicken"&&target.sourceNPC=="NUI_CHICKEN","Original tree-1 birth index0");
   var carrier=((IList)Field(scene,"enemyViews"))[index] as SpriteRenderer;var camera=(Camera)Field(scene,"worldCamera");var frame=owner.LastCommittedFrame;var views=(IDictionary)Field(scene,"chickenA1ViewsV31");var entry=views[target];
   var row=new FrameRow{frame=Time.frameCount,index=index,facing=target.facing,pose=frame.state.ToString(),modelClip=target.clip,sourceAlias=(string)Field(entry,"actualAlias"),sourceFrame=(int)Field(entry,"actualSourceFrame"),sourceSprite=carrier&&carrier.sprite?carrier.sprite.name:null,hostClock=frame.hostClock,sourceClock=frame.sourceRenderClock,phase=frame.normalizedPhase,fallLaunchPhase=frame.airFallLaunchPhase,hp=target.hp,deathTime=target.deathTime,sixParts=parts.Count==6,carrierHidden=carrier&&carrier.forceRenderingOff,worldPost=worldFrame==Time.frameCount,modelFoot=new Vector3(target.x,target.y,0),projectedFoot=(Vector3)Field(entry,"projectedWorldFoot"),localPositions=new Vector3[6],localRotations=new Quaternion[6],localScales=new Vector3[6]};
   row.bounds=parts[0].bounds;row.alpha=parts[0].color.a;
   for(int i=0;i<parts.Count;i++){
    var p=parts[i];Need(p&&p.enabled&&p.gameObject.activeInHierarchy&&p.sprite&&!p.forceRenderingOff,"A1 part actually eligible");Need(Mathf.Abs(p.color.a-row.alpha)<.000001f,"Shared death alpha");
    int seen;if(drawn.TryGetValue(p,out seen)&&seen==Time.frameCount)row.drawnParts++;row.bounds.Encapsulate(p.bounds);row.localPositions[i]=p.transform.localPosition;row.localRotations[i]=p.transform.localRotation;row.localScales[i]=p.transform.localScale;
   }
   row.viewportMin=CaptureViewportPoint(camera,row.bounds.min);row.viewportMax=CaptureViewportPoint(camera,row.bounds.max);row.viewportOverlap=row.viewportMax.x>0&&row.viewportMin.x<1&&row.viewportMax.y>0&&row.viewportMin.y<1&&row.viewportMin.z>0;
   if(row.pose=="Idle"||row.pose=="Walk"){
    var solid=owner.BindingIdentity.VisibleLocalRects();
    row.leftSoleY=parts[4].transform.TransformPoint(new Vector3(0,solid[4].yMin,0)).y-row.projectedFoot.y;
    row.rightSoleY=parts[5].transform.TransformPoint(new Vector3(0,solid[5].yMin,0)).y-row.projectedFoot.y;
    Need(Mathf.Abs(Mathf.Min(row.leftSoleY,row.rightSoleY))<.07f,"A1 bearing foot touches original floor");
   }
   row.sourceClockMatches=Mathf.Abs(row.hostClock-target.clock)<.000001f;
   if(frame.state==ChickenAuthoredPoseV31.State.AirUp||frame.state==ChickenAuthoredPoseV31.State.AirFall||frame.state==ChickenAuthoredPoseV31.State.Hurt)row.sourceClockMatches&=Mathf.Abs(row.sourceClock-target.clock)<.000001f;
   var bar=((IList)Field(scene,"enemyBars"))[index] as SpriteRenderer;var status=((IList)Field(scene,"enemyDebuffViews"))[index] as TextMesh;
   row.labelVisible=bar&&bar.enabled&&bar.gameObject.activeInHierarchy;if(owner.TryVisibleTop(out row.visibleTop)){row.barY=bar?bar.transform.position.y:0;row.statusY=status?status.transform.position.y:0;row.barGap=row.barY-row.visibleTop;row.statusGap=row.statusY-row.visibleTop;}
   report.frames.Add(row);if(!report.observed.Contains(row.pose))report.observed.Add(row.pose);
   Need(row.sixParts&&row.carrierHidden&&row.drawnParts==6&&row.worldPost&&row.viewportOverlap&&row.alpha>0,"All six authored parts need same-frame camera draw");Need(row.sourceClockMatches,"Production display retains actual source/host clocks");
   if(row.labelVisible)Need(Mathf.Abs(row.barGap-8)<.002f&&Mathf.Abs(row.statusGap-20)<.002f,"Labels follow visible A1 top");
   if(previous!=null&&previous.pose=="AirUp"&&row.pose=="AirFall"){
    row.airFallSeamChecked=true;for(int i=0;i<6;i++){row.airFallLocalSeamError=Mathf.Max(row.airFallLocalSeamError,(row.localPositions[i]-previous.localPositions[i]).magnitude);row.airFallLocalSeamError=Mathf.Max(row.airFallLocalSeamError,Quaternion.Angle(row.localRotations[i],previous.localRotations[i]));row.airFallLocalSeamError=Mathf.Max(row.airFallLocalSeamError,(row.localScales[i]-previous.localScales[i]).magnitude);}
    Need(row.phase==0&&row.airFallLocalSeamError<.001f&&row.sourceClock>=previous.sourceClock&&row.hostClock>=previous.hostClock,"Actual six-part AirUp/Fall seam and unchanged continuous source clock");
   }
   previous=row;return row;
  }
  static void SavePng(Texture2D image,string path){var bytes=image.EncodeToPNG();using(var f=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read))f.Write(bytes,0,bytes.Length);}
  PixelResult PixelPair(SpriteRenderer[] renderers,Bounds bounds,string label){
   var result=new PixelResult();var camera=(Camera)Field(scene,"worldCamera");var oldTarget=camera.targetTexture;var oldActive=RenderTexture.active;Rect oldRect=camera.rect;float oldAspect=camera.aspect;var enabled=new bool[renderers.Length];for(int i=0;i<renderers.Length;i++)enabled[i]=renderers[i].enabled;
   Texture2D on=null,off=null;RenderTexture rt=null;float elapsed=game.elapsed;string actor=JsonUtility.ToJson(target);int oldWorldFrame=worldFrame;
   try{
    rt=new RenderTexture(RenderWidth,RenderHeight,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"1280x720 pixel target");camera.targetTexture=rt;camera.rect=new Rect(0,0,1,1);camera.aspect=(float)RenderWidth/RenderHeight;pairRender=true;
    RenderTexture.active=rt;GL.Clear(true,true,Color.clear);foreach(var r in renderers)r.enabled=true;camera.Render();on=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);on.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0,false);on.Apply(false,false);
    RenderTexture.active=rt;GL.Clear(true,true,Color.clear);foreach(var r in renderers)r.enabled=false;camera.Render();off=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);off.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0,false);off.Apply(false,false);
    var a=on.GetPixels32();var b=off.GetPixels32();var min=camera.WorldToViewportPoint(bounds.min);var max=camera.WorldToViewportPoint(bounds.max);int w=rt.width,h=rt.height;
    int x0=Mathf.Clamp(Mathf.FloorToInt(min.x*w)-2,0,w),x1=Mathf.Clamp(Mathf.CeilToInt(max.x*w)+2,0,w),y0=Mathf.Clamp(Mathf.FloorToInt(min.y*h)-2,0,h),y1=Mathf.Clamp(Mathf.CeilToInt(max.y*h)+2,0,h);int left=w,right=-1,bottom=h,top=-1;
    for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++){int n=y*w+x;if(Mathf.Abs(a[n].r-b[n].r)+Mathf.Abs(a[n].g-b[n].g)+Mathf.Abs(a[n].b-b[n].b)>6){result.count++;left=Math.Min(left,x);right=Math.Max(right,x);bottom=Math.Min(bottom,y);top=Math.Max(top,y);}}
    result.width=right>=left?right-left+1:0;result.height=top>=bottom?top-bottom+1:0;result.on=Path.Combine(output,label+"-on.png");result.off=Path.Combine(output,label+"-off.png");SavePng(on,result.on);SavePng(off,result.off);
   }finally{for(int i=0;i<renderers.Length;i++)if(renderers[i])renderers[i].enabled=enabled[i];camera.targetTexture=oldTarget;camera.rect=oldRect;camera.aspect=oldAspect;RenderTexture.active=oldActive;pairRender=false;worldFrame=oldWorldFrame;if(rt){rt.Release();Destroy(rt);}if(on)Destroy(on);if(off)Destroy(off);}
   Need(game.elapsed==elapsed&&JsonUtility.ToJson(target)==actor,"Pixel pair cannot step or mutate original actor");Need(result.count>0&&result.width>0&&result.height>0,"Authored parts need real GPU pixels");return result;
  }
  void Capture(FrameRow row,string label){
   Need(string.IsNullOrEmpty(row.label)&&observationTarget,"One capture of the current original world-camera view");row.label=label;Texture2D image=null;var oldActive=RenderTexture.active;
   try{RenderTexture.active=observationTarget;image=new Texture2D(observationTarget.width,observationTarget.height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,observationTarget.width,observationTarget.height),0,0,false);image.Apply(false,false);row.image=Path.Combine(output,label+"-world-view.png");SavePng(image,row.image);}finally{RenderTexture.active=oldActive;if(image)Destroy(image);}
   var pixels=PixelPair(parts.ToArray(),row.bounds,label+"-a1");row.changedBodyPixels=pixels.count;row.pixelWidth=pixels.width;row.pixelHeight=pixels.height;row.onImage=pixels.on;row.offImage=pixels.off;row.pixelsProven=true;
  }
  void MeasureRatio(FrameRow row){
   Need(row.pose=="Idle"&&row.pixelsProven,"Idle A1 camera view and pixel contribution");var hero=(SpriteRenderer)Field(scene,"heroView");Need(hero&&hero.sprite&&hero.enabled&&!hero.forceRenderingOff&&game.player.clip=="idle","Actual visible idle LK");var measured=PixelPair(new[]{hero},hero.bounds,"02-same-ground-lk");
   var r=report.ratio;r.measured=true;r.heroY=game.player.y;r.chickenY=target.y;r.sameGround=Mathf.Abs(r.heroY-r.chickenY)<.05f;r.heroPixelsHigh=measured.height;r.chickenPixelsHigh=row.pixelHeight;r.actualPixelRatio=(float)row.pixelHeight/measured.height;r.registrationHeight=ChickenAuthoredPartsV31.SourceRatioLogicalStandingHeight;r.unitsPerArtPixel=owner.BindingIdentity.unitsPerArtPixel;r.worldView=row.image;r.heroOn=measured.on;r.heroOff=measured.off;
   r.passed=r.sameGround&&Mathf.Abs(r.actualPixelRatio-r.expectedRatio)<.08f&&Math.Abs(r.registrationHeight-64.66812008287489)<.00001;Need(r.passed,"Actual same-ground LK:A1 pixel-height ratio within raster tolerance");
  }
  void PlacePlayer(float offset,bool walking=false){
   bool ratio=!walking&&Mathf.Abs(offset)>=150;
   float best=float.PositiveInfinity,chosenX=0,chosenY=0;int line=-1;
   for(int side=0;side<(ratio?2:1);side++)for(int i=0;i<=44;i++){
    float delta=i==0?0:((i+1)/2)*3*(i%2==1?1:-1);
    float distance=Mathf.Abs(offset)+delta;
    if(distance<(ratio?145:walking?137:22)||walking&&distance>208)continue;
    float x=target.x+Mathf.Sign(offset)*(side==0?1:-1)*distance;
    if(x<20||x>game.CurrentRoom.width-20)continue;
    var surface=game.SurfaceBelow(x,target.y+24,true);if(surface==null)continue;
    float y=surface.Y(x),vertical=Mathf.Abs(y-target.y);
    if(ratio&&vertical>=.05f)continue;
    float score=vertical*4+Mathf.Abs(delta)+side;
    if(score>=best)continue;
    best=score;line=surface.id;chosenX=x;chosenY=y;
   }
   Need(line>=0,"Original floor under the player at the requested range");
   game.player.x=chosenX;game.player.y=chosenY;game.player.groundLine=line;game.player.vx=game.player.vy=0;game.player.invincible=120;
   report.actions.Add("CONTROLLED player on source line "+line+" at "+chosenX+","+chosenY+" relative to chicken "+target.x+","+target.y);
  }
  bool CanHit(){return target.active&&!target.dormant&&target.hp>0&&target.state==CombatState.Normal&&(bool)Call(game,"Grounded",target)&&!(bool)Call(game,"EnemyHasSuperArmor",target)&&!(bool)Call(game,"EnemyHasNpcInvincibility",target);}
  void Hit(HitReaction reaction,bool lethal){int damage=lethal?(int)Math.Ceiling(target.hp)+1:1;float oldHp=target.hp;int dealt=game.ApplyHitToEnemy(target,damage,reaction);report.actions.Add("PUBLIC ApplyHitToEnemy original tree-1 index0 "+reaction+" damage="+damage+" dealt="+dealt);Need(dealt>0&&target.hp<oldHp,"Public damage must apply");}
  IEnumerable WaitPose(string pose,float minPhase,string label,double seconds=15){
   double until=Time.realtimeSinceStartupAsDouble+seconds;
   while(true){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Wait pose "+pose);var row=Observe();if(row.pose!=pose||row.phase<minPhase)continue;if(!string.IsNullOrEmpty(label))Capture(row,label);yield break;}
  }
  IEnumerable WaitCanHit(){double until=Time.realtimeSinceStartupAsDouble+15;while(true){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Wait unarmored grounded target");Observe();if(CanHit())yield break;}}
  IEnumerable SourceReactionsV45(){
   var records=new List<string>();var poses=new[]{new[]{"DamageAirFlyFront","DamageDownFront","DamageStandUpFront"},new[]{"DamageAirFlyBack","DamageDownBack","DamageStandUpBack"}};
   for(int side=0;side<2;side++){
    foreach(var s in WaitCanHit())yield return s;
    while(target.invincible>0){yield return null;Check();}
    // Keep the hero far enough from the contact to see the chicken's feet in
    // the readback; the directional hit still comes through the game's normal API.
    PlacePlayer(-115);
    float x=target.x+target.facing*(side==0?35:-35);
    int damage=(int)Call(game,"ApplyHitToEnemyFromXR1417",target,1,HitReaction.Launch,240f,x,5);Need(damage>0,"A1 original fly damage");
    foreach(var motion in poses[side]){
     double end=Time.realtimeSinceStartupAsDouble+15;
     for(;;){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<end,"A1 real mapped "+motion+" actual "+owner.LastCommittedFrame.originalReaction+" "+target.state);
      var actual=owner.LastCommittedFrame;
      // Capture the landed or rising body, not just the upright entry drawing.
      float proofAt=motion.Contains("StandUp")?.98f:motion.Contains("Down")?.28f:.035f;
      if(actual.originalReaction!=motion||actual.originalReactionClock<proofAt)continue;
      var row=Observe();Capture(row,"v45-"+motion);Need(row.pixelsProven&&row.changedBodyPixels>0,"Visible six A1 parts "+motion);
      records.Add(motion+":"+row.changedBodyPixels);break;
     }
    }
   }
   File.WriteAllLines(Path.Combine(output,"a1-source-reactions.txt"),records.ToArray());
  }
  IEnumerator Flow(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Check();yield return null;}for(int i=0;i<3;i++)yield return null;
   game=(GameModel)Field(scene,"game");Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused,"Ordinary Main menu");foreach(var step in EnterTree())yield return step;
   for(int i=0;i<4;i++){Check();yield return null;}game=(GameModel)Field(scene,"game");Need(game.room==0&&game.CurrentRoom.id=="tree-1"&&game.enemies.Count>0&&!game.paused,"Ordinary Tree room1 running");target=game.enemies[0];Need(target.sourceNPC=="NUI_CHICKEN"&&target.kind=="chicken","Unchanged original chicken birth");
   PlacePlayer(-180);
   var idleHero=(SpriteRenderer)Field(scene,"heroView");double ratioEnd=Time.realtimeSinceStartupAsDouble+15;
   while(game.player.clip!="idle"||!idleHero||!idleHero.sprite||!idleHero.enabled||idleHero.forceRenderingOff){
    yield return null;Need(Time.realtimeSinceStartupAsDouble<ratioEnd,"Same-floor visible LK idle: "+game.player.clip);
   }
   foreach(var step in WaitPose("Idle",0,"01-idle-same-ground"))yield return step;MeasureRatio(previous);
   PlacePlayer(-155,true);foreach(var step in WaitPose("Walk",.15f,"03-walk-left"))yield return step;Need(target.facing<0,"Natural left walk");report.leftWalk=true;
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--source-chicken-hits-v45")>=0){
    PlacePlayer(-58);foreach(var step in WaitPose("AttackReady",.35f,"04-ready-left"))yield return step;
    foreach(var step in WaitPose("AttackFire",0,"05-fire-left"))yield return step;report.leftAttack=true;
    foreach(var step in SourceReactionsV45())yield return step;target=null;Press("dungeons");for(int i=0;i<3;i++)yield return null;
    report.returnedMenu=(string)Field(scene,"openMenu")=="dungeons"&&game.paused;Need(report.returnedMenu,"Return to ordinary menu");yield break;
   }
   foreach(var step in WaitPose("AttackReady",.35f,"04-ready-left"))yield return step;Need(target.facing<0,"Natural left attack");foreach(var step in WaitPose("AttackFire",0,"05-fire-left"))yield return step;report.leftAttack=true;
   PlacePlayer(155,true);foreach(var step in WaitPose("Walk",.15f,"06-walk-right"))yield return step;Need(target.facing>0,"Natural right walk");report.rightWalk=true;
   foreach(var step in WaitPose("AttackReady",.35f,"07-ready-right"))yield return step;Need(target.facing>0,"Natural right attack");foreach(var step in WaitPose("AttackFire",0,"08-fire-right"))yield return step;report.rightAttack=true;
   foreach(var step in WaitCanHit())yield return step;PlacePlayer(-45);Hit(HitReaction.Hurt,false);foreach(var step in WaitPose("Hurt",.12f,"09-hurt-first"))yield return step;
   float oldClock=target.clock;Need(target.state==CombatState.Hurt&&target.stun>0,"Repeat while actually hurt");Hit(HitReaction.Hurt,false);Need(target.clock==0&&oldClock>0,"Public repeated hit resets original recovery clock");foreach(var step in WaitPose("Hurt",.12f,"10-hurt-repeat"))yield return step;report.repeatedHurtVerified=true;
   foreach(var step in WaitCanHit())yield return step;Hit(HitReaction.Launch,false);foreach(var step in WaitPose("AirUp",.3f,"11-launch"))yield return step;foreach(var step in WaitPose("AirFall",0,"12-fall-entry"))yield return step;
   Need(previous.airFallSeamChecked,"Actual launch-to-fall seam observed");foreach(var step in WaitPose("AirFall",.4f,"13-fall-open"))yield return step;foreach(var step in WaitPose("Down",.45f,"14-down"))yield return step;foreach(var step in WaitPose("GetUp",.3f,"15-get-up"))yield return step;
   foreach(var step in WaitCanHit())yield return step;Hit(HitReaction.Hurt,true);foreach(var step in WaitPose("Death",.04f,"16-death-early"))yield return step;float a=previous.alpha;
   foreach(var step in WaitPose("Death",.45f,"17-death-middle"))yield return step;float b=previous.alpha;foreach(var step in WaitPose("Death",.8f,"18-death-late"))yield return step;float c=previous.alpha;
   report.deathFadeVerified=a>b&&b>c&&c>0&&Mathf.Abs(c-Mathf.Max(0,1-target.deathTime/1.2f))<.00001f;Need(report.deathFadeVerified,"Actual six-part alpha fades with original death clock");
   foreach(string state in new[]{"Idle","Walk","AttackReady","AttackFire","Hurt","AirUp","AirFall","Down","GetUp","Death"})Need(report.observed.Contains(state),"Missing actual A1 state "+state);
   target=null;Press("dungeons");for(int i=0;i<3;i++)yield return null;report.returnedMenu=(string)Field(scene,"openMenu")=="dungeons"&&game.paused;Need(report.returnedMenu,"Return to ordinary menu");
  }
  void Complete(){if(finished)return;finished=true;Application.logMessageReceived-=Log;Camera.onPostRender-=PostRender;int code=report.passed?0:2;try{File.WriteAllText(Path.Combine(output,"a1-chicken-player-report.json"),JsonUtility.ToJson(report,true),new System.Text.UTF8Encoding(false));}catch(Exception e){Debug.LogException(e);code=2;}finally{Application.Quit(code);}}
  void OnApplicationQuit(){if(started>0)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){Application.logMessageReceived-=Log;Camera.onPostRender-=PostRender;if(observationTarget){observationTarget.Release();Destroy(observationTarget);observationTarget=null;}}
 }
 public sealed class A1ChickenPartDrawWitnessV32 : MonoBehaviour {
  A1ChickenPlayerCheckV32 owner;SpriteRenderer body;
  internal void Bind(A1ChickenPlayerCheckV32 check,SpriteRenderer renderer){owner=check;body=renderer;}
  void OnWillRenderObject(){if(owner&&body)owner.Draw(body,Camera.current);}
 }
}
#endif
