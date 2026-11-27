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
 public sealed class FairyGuardianPlayerCheckV33 : MonoBehaviour {
  const string Flag="--root-fairy-guardian-check-v33", OutputFlag="--root-fairy-guardian-output-v33";
  const int RenderWidth=1280, RenderHeight=720;
  const double Budget=360;
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
  [Serializable] sealed class Row {
   public int frame,index,facing,drawnParts,changedBodyPixels,pixelWidth,pixelHeight;
   public string label,alias,clip,state,combat,image,onImage,offImage,sourceSprite;
   public float sourceClock,hostClock,npcTime,hp,alpha,deathTime,barGap,statusGap,leftSoleY,rightSoleY;
   public bool worldPost,carrierHidden,pixelsProven;
   public Bounds bounds;
  }
  [Serializable] sealed class EffectRow {
   public string id,state;public int serial,facing;public float sourceThreshold,bornAt,observedNpcTime,age,alpha,x,y,z;
  }
  [Serializable] sealed class Report {
   public string schema="FAIRY_GUARDIAN_REAL_BIRTH_CAMERA_AND_EFFECT_CHECK_V33",graphics,failure;
   public bool passed,batchMode,ordinaryMenuEntry,returnedMenu,reenteredRuins,previousPartsReleased,previousSpritesReleased,previousRuntimeAtlasReleased,sharedAtlasPreserved;
   public bool controlledPlayerPlacement=true,controlledPublicHits=true,enemyStateOrClockAssigned=false,extraGameStep=false,naturalTraversalClaim=false,windowPresentationVerified=false;
   public string scope="Current six source states and existing host reactions on one original ruins-1 Fairy birth. Original 28-state parity and natural dungeon clear are separate.";
   public int observedFrames,sourceSpawnIndex=27;
   public bool allFourOriginalAudioClipPlayIssued,frontGroundPath,rearGroundPath,audibleHardwareOutputVerified=false;
   public List<string> audioPlayIds=new List<string>();
   public List<Row> captures=new List<Row>(); public List<EffectRow> effects=new List<EffectRow>();public List<string> actions=new List<string>(),errors=new List<string>();
  }
  sealed class PixelResult {internal int count,width,height;internal string on,off;}
  readonly List<SpriteRenderer> parts=new List<SpriteRenderer>();
  readonly Dictionary<SpriteRenderer,int> drawn=new Dictionary<SpriteRenderer,int>();
  readonly HashSet<int> seenSerials=new HashSet<int>();
  DemoScene scene;GameModel game;Enemy target;FairyAuthoredPartsV33 owner;string output;
  Report report=new Report();RenderTexture observationTarget;int worldFrame=-1;bool pairRender,finished,backgroundBefore;double started;
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int flags=0,outputs=0;string path=null;
   for(int i=0;i<args.Length;i++){if(args[i]==Flag)flags++;if(args[i]==OutputFlag){outputs++;if(i+1<args.Length)path=args[++i];}}
   if(flags==0)return;
   try{
    Need(flags==1&&outputs==1&&!string.IsNullOrWhiteSpace(path),"exact explicit check and output");
    Need(!Application.isEditor&&SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null,"standalone GPU");
    string full=Path.GetFullPath(path);
    Need(full.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(full)&&!File.Exists(full),"fresh C runtime-drafts child");
    foreach(var a in args)Need(a!="--root-core-reaction-check-v32"&&a!="--root-a1-chicken-check-v32"&&a!="--root-factory-sheriff-check-v32","one driving fixture per process");
    Directory.CreateDirectory(full);new GameObject("Explicit original Fairy birth review V33").AddComponent<FairyGuardianPlayerCheckV33>().output=full;
   }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
  }
  static void Need(bool ok,string what){if(!ok)throw new InvalidOperationException("Fairy Player V33: "+what);}
  static object Field(object obj,string key){var f=obj.GetType().GetField(key,Fields);Need(f!=null,"field "+key);return f.GetValue(obj);}
  void Log(string text,string stack,LogType type){if(type==LogType.Error||type==LogType.Assert||type==LogType.Exception||text.StartsWith("FAIRY_PARTS_"))report.errors.Add(text+" "+stack);}
  void PostRender(Camera c){if(!pairRender&&scene&&ReferenceEquals(c,Field(scene,"worldCamera")))worldFrame=Time.frameCount;}
  void Check(){Need(report.errors.Count==0,"runtime error or Fairy fallback");Need(Time.realtimeSinceStartupAsDouble-started<Budget,"overall deadline");}
  IEnumerator Start(){
   started=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;Application.runInBackground=true;
   report.graphics=SystemInfo.graphicsDeviceType.ToString();report.batchMode=Application.isBatchMode;
   Application.logMessageReceived+=Log;Camera.onPostRender+=PostRender;
   var flow=Flow();Exception failure=null;
   while(!finished){bool more=false;object next=null;try{more=flow.MoveNext();if(more)next=flow.Current;}catch(Exception e){failure=e;}
    if(failure!=null||!more)break;yield return next;}
   (flow as IDisposable)?.Dispose();if(finished)yield break;
   if(failure!=null)report.failure=failure.ToString();report.passed=failure==null&&report.errors.Count==0;Complete();
  }
  void Update(){if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>=Budget){report.failure="overall deadline";Complete();}}
  void Press(string action){
   TouchControl control=null;foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy&&ReferenceEquals(c.owner,scene)&&c.action==action){Need(!control,"Duplicate control "+action);control=c;}
   Need(control&&EventSystem.current,"Active ordinary control "+action);var image=control.GetComponent<Image>();Need(image&&image.raycastTarget,"Raycastable ordinary control");
   var pointer=new PointerEventData(EventSystem.current){pointerId=-3202,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
   report.actions.Add("real-touch:"+action);try{Need(ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerDownHandler),"Pointer down accepted");}finally{if(control)ExecuteEvents.Execute(control.gameObject,pointer,ExecuteEvents.pointerUpHandler);}
  }
  IEnumerable EnterRuins(){
   var routes=(IList)Field(scene,"mapDungeons");int route=-1;for(int i=0;i<routes.Count;i++)if((string)Field(routes[i],"id")=="ruins")route=i;Need(route>=0,"Ordinary Ruins route");
   Press("region:ruben");yield return null;var visible=(IList)Field(scene,"mapVisible");int index=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==route)index=i;Need(index>=0,"Visible Ruins card");
   int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();int page=index/size,attempt=0;
   while((int)Field(scene,"mapPage")!=page){Need(++attempt<20,"Menu page bound");Press("dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1"));yield return null;}
   Press("dungeon-card:"+(index%size));yield return null;Need((int)Field(scene,"mapSelected")==route,"Selected original Ruins card");Press("dungeon-enter");report.ordinaryMenuEntry=true;
  }
  bool ResolveParts(){
   parts.Clear();if(!scene||target==null||game==null)return false;
   var views=(IDictionary)Field(scene,"fairyViewsV33");if(!views.Contains(target))return false;
   owner=Field(views[target],"parts") as FairyAuthoredPartsV33;if(owner==null)return false;
   owner.VisitRenderers(r=>parts.Add(r));return parts.Count==7;
  }
  void RenderObservation(){
   Need(ResolveParts(),"Seven Fairy renderers before explicit world-camera render");
   foreach(var part in parts){Need(part,"Live Fairy part");var witness=part.GetComponent<FairyPartDrawWitnessV33>();if(!witness)witness=part.gameObject.AddComponent<FairyPartDrawWitnessV33>();witness.Bind(this,part);}
   int width=RenderWidth,height=RenderHeight;
   if(!observationTarget||observationTarget.width!=width||observationTarget.height!=height){if(observationTarget){observationTarget.Release();Destroy(observationTarget);}observationTarget=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(observationTarget.Create(),"Observation GPU target");}
   var camera=(Camera)Field(scene,"worldCamera");var oldTarget=camera.targetTexture;var oldActive=RenderTexture.active;Rect oldRect=camera.rect;float oldAspect=camera.aspect;float elapsed=game.elapsed;string actor=JsonUtility.ToJson(target);
   // Hidden standalone windows may never signal WaitForEndOfFrame. After
   // yield-null, DemoScene.Update has produced the actual scene transforms;
   // explicitly draw that unchanged original camera and witness all seven parts.
   try{pairRender=false;camera.targetTexture=observationTarget;camera.rect=new Rect(0,0,1,1);camera.aspect=(float)RenderWidth/RenderHeight;RenderTexture.active=observationTarget;GL.Clear(true,true,Color.clear);camera.Render();}
   finally{camera.targetTexture=oldTarget;camera.rect=oldRect;camera.aspect=oldAspect;RenderTexture.active=oldActive;}
   Need(game.elapsed==elapsed&&JsonUtility.ToJson(target)==actor,"Observation render cannot step or mutate actor");
  }
  internal void Draw(SpriteRenderer part,Camera camera){if(!pairRender&&!finished&&scene&&ReferenceEquals(camera,Field(scene,"worldCamera")))drawn[part]=Time.frameCount;}
  static Vector3 CaptureViewportPoint(Camera camera,Vector3 point){float aspect=camera.aspect;try{camera.aspect=(float)RenderWidth/RenderHeight;return camera.WorldToViewportPoint(point);}finally{camera.aspect=aspect;}}
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
  void Capture(Row row,string label){
   Need(string.IsNullOrEmpty(row.label)&&observationTarget,"One capture of the current original world-camera view");row.label=label;Texture2D image=null;var oldActive=RenderTexture.active;
   try{RenderTexture.active=observationTarget;image=new Texture2D(observationTarget.width,observationTarget.height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,observationTarget.width,observationTarget.height),0,0,false);image.Apply(false,false);row.image=Path.Combine(output,label+"-world-view.png");SavePng(image,row.image);}finally{RenderTexture.active=oldActive;if(image)Destroy(image);}
   var pixels=PixelPair(parts.ToArray(),row.bounds,label+"-fairy");row.changedBodyPixels=pixels.count;row.pixelWidth=pixels.width;row.pixelHeight=pixels.height;row.onImage=pixels.on;row.offImage=pixels.off;row.pixelsProven=true;
  }
  Row Observe(){
   RenderObservation();
   Need(game.enemies.Count>4&&ReferenceEquals(game.enemies[4],target)&&target.sourceNPC=="NUI_FAIRY_GUARDIAN"&&game.CurrentRoom.id=="ruins-1","original ruins-1 staged4/source27");
   var frame=owner.LastFrame;var carrier=(SpriteRenderer)((IList)Field(scene,"enemyViews"))[4];var cam=(Camera)Field(scene,"worldCamera");
   var row=new Row{frame=Time.frameCount,index=4,facing=target.facing,alias=frame.alias,clip=target.clip,state=target.npcState,combat=target.state.ToString(),sourceClock=frame.sourceClock,hostClock=target.clock,npcTime=target.npcTime,hp=target.hp,alpha=parts[0].color.a,deathTime=target.deathTime,worldPost=worldFrame==Time.frameCount,carrierHidden=carrier.forceRenderingOff,sourceSprite=carrier.sprite?carrier.sprite.name:null};
   if(frame.alias=="source-walk"||frame.alias=="source-idle"){
    var library=(FairyPoseLibraryV33)Field(owner,"lib");
    for(int i=5;i<=6;i++){
     var renderer=parts[i];var hull=library.parts[i].visibleHull;float lowest=float.PositiveInfinity;
     for(int j=0;j<hull.Length;j+=2){
      var vertex=new Vector3(renderer.flipX?-hull[j]:hull[j],hull[j+1],0)*library.UnitsPerPixel;
      lowest=Mathf.Min(lowest,renderer.transform.TransformPoint(vertex).y);
     }
     float gap=lowest-renderer.transform.parent.position.y;
     if(i==5)row.leftSoleY=gap;else row.rightSoleY=gap;
    }
    Need(Mathf.Abs(Mathf.Min(row.leftSoleY,row.rightSoleY))<.55f,"Registered original supporting root on the live floor");
   }
   row.bounds=parts[0].bounds;
   foreach(var part in parts){Need(part&&part.enabled&&part.gameObject.activeInHierarchy&&part.sprite&&!part.forceRenderingOff&&Mathf.Abs(part.color.a-row.alpha)<.00001f,"live seven-part color and visibility");row.bounds.Encapsulate(part.bounds);if(drawn.TryGetValue(part,out var when)&&when==Time.frameCount)++row.drawnParts;}
   var lo=CaptureViewportPoint(cam,row.bounds.min);var hi=CaptureViewportPoint(cam,row.bounds.max);
   Need(row.drawnParts==7&&row.worldPost&&row.carrierHidden&&row.alpha>0&&hi.x>0&&lo.x<1&&hi.y>0&&lo.y<1,"all seven same-frame real-world-camera draws");
   float expected=target.clock;
   if(target.state==CombatState.Normal)expected=target.npcTime;
   else if(target.state==CombatState.GettingUp)expected=Mathf.Min(target.clock,game.FairyGetUpSecondsV34(target));
   else if(target.state==CombatState.Dead)expected=Mathf.Clamp01(target.deathTime/1.2f)*(19f/15f);
   Need(Mathf.Abs(row.sourceClock-expected)<.00002f&&frame.facing==target.facing,"existing source clock and model facing");
   var bar=(SpriteRenderer)((IList)Field(scene,"enemyBars"))[4];var status=(TextMesh)((IList)Field(scene,"enemyDebuffViews"))[4];
   if(bar&&bar.enabled&&bar.gameObject.activeInHierarchy&&owner.TryVisibleTop(out var top)){
    row.barGap=bar.transform.position.y-top;row.statusGap=status.transform.position.y-top;
    Need(Mathf.Abs(row.barGap-8)<.003f&&Mathf.Abs(row.statusGap-20)<.003f,"labels above true part hull");
   }
   ++report.observedFrames;return row;
  }
  void ObserveEffects(){
   foreach(var effect in game.npcEffects){
    if(!ReferenceEquals(effect.owner,target)||effect.emission==null||!seenSerials.Add(effect.serial))continue;
    Need(effect.emission.bone=="Bip01"&&effect.emission.follow&&effect.lifetime==.2f,"real original attached damage effect");
    report.effects.Add(new EffectRow{id=effect.id,state=effect.emission.ownerState,serial=effect.serial,facing=effect.facing,sourceThreshold=effect.emission.start,bornAt=effect.bornAt,observedNpcTime=target.npcTime,age=effect.age,alpha=effect.alpha,x=effect.x,y=effect.y,z=effect.z});
   }
  }
  void PlacePlayer(float offset){
   float x=target.x+offset;var surface=game.SurfaceBelow(x,target.y+12,true);Need(surface!=null,"real ground for controlled player placement");
   game.player.x=x;game.player.y=surface.Y(x);game.player.groundLine=surface.id;game.player.vx=game.player.vy=0;game.player.invincible=120;
   report.actions.Add("controlled player at target.x "+offset+" on source ground "+surface.id);
  }
  IEnumerable WaitView(){
   double until=Time.realtimeSinceStartupAsDouble+25;
   while(true){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"wait for original camera and initial crop");
    if(!ResolveParts())continue;var cam=(Camera)Field(scene,"worldCamera");var p=CaptureViewportPoint(cam,parts[0].bounds.center);
    if(p.x>.06f&&p.x<.94f&&p.y>.06f&&p.y<.94f)yield break;
   }
  }
  IEnumerable WaitPose(string label,Func<Row,bool> predicate,double seconds=25){
   double until=Time.realtimeSinceStartupAsDouble+seconds;
   while(true){yield return null;Check();ObserveEffects();var row=Observe();
    Need(Time.realtimeSinceStartupAsDouble<until,label+" deadline: "+row.alias+"/"+row.state+"/"+row.sourceClock+" player="+game.player.x+" actor="+target.x);
    if(!predicate(row))continue;Capture(row,label);report.captures.Add(row);yield break;
   }
  }
  IEnumerable WaitEffect(string id,float threshold,int facing,string label){
   foreach(var next in WaitPose(label,r=>report.effects.Exists(e=>e.id==id&&Math.Abs(e.sourceThreshold-threshold)<.0001f&&(facing==0||e.facing==facing))))yield return next;
  }
  bool CanHit(){
   return target.active&&!target.dormant&&target.hp>0&&target.state==CombatState.Normal&&
    !string.IsNullOrEmpty(target.npcState)&&game.Grounded(target)&&!game.EnemyHasSuperArmor(target)&&!game.EnemyHasNpcInvincibility(target);
  }
  IEnumerable WaitHit(){
   double until=Time.realtimeSinceStartupAsDouble+30;
   while(!CanHit()){yield return null;Check();ObserveEffects();Observe();Need(Time.realtimeSinceStartupAsDouble<until,"real grounded unarmored interval");}
  }
  void Hit(HitReaction reaction,bool lethal){
   float before=target.hp;int damage=lethal?(int)Math.Ceiling(before)+1:1;
   int dealt=game.ApplyHitToEnemy(target,damage,reaction);Need(dealt>0&&target.hp<before,"public hit on original actor");
   report.actions.Add("public ApplyHitToEnemy "+reaction+" dealt="+dealt);
  }
  void BindOriginal(){
   game=(GameModel)Field(scene,"game");Need(!game.paused&&game.room==0&&game.CurrentRoom.id=="ruins-1"&&game.enemies.Count>4,"ordinary Ruins room1");
   target=game.enemies[4];Need(target.sourceNPC=="NUI_FAIRY_GUARDIAN"&&target.kind=="fairy-guardian"&&game.GuardianSourceProfileV28(target),"bound real source27 birth");
   var catalog=(ArtCatalog)Field(scene,"catalog");
   Need(catalog.stages[3].id=="ruins-1"&&catalog.stages[3].spawns[4].sourceSpawnIndex==27,"exact staged index from catalog");
  }
  IEnumerator Flow(){
   while(!(scene=FindFirstObjectByType<DemoScene>())){Check();yield return null;}
   for(int j=0;j<3;j++)yield return null;
   Need((string)Field(scene,"openMenu")=="dungeons","Main dungeon menu");
   foreach(var step in EnterRuins())yield return step;
   for(int j=0;j<4;j++)yield return null;
   BindOriginal();PlacePlayer(-170);
   foreach(var step in WaitView())yield return step;
   foreach(var step in WaitPose("01-initial-curl",r=>r.alias=="source-trap-wait"))yield return step;
   PlacePlayer(-65);
   foreach(var step in WaitPose("02-trap-start",r=>r.alias=="source-trap-start"&&r.sourceClock>.10f&&r.sourceClock<2))yield return step;
   const string opening="DamageEffect_FAIRY_GUARDIAN_TRAP_MODE_START", fire="DamageEffect_FAIRY_GUARDIAN_ATTACK_A";
   foreach(var step in WaitEffect(opening,2.35f,0,"03-trap-first"))yield return step;
   foreach(var step in WaitEffect(opening,3.3f,0,"04-trap-second"))yield return step;
   foreach(var step in WaitPose("05-normal-wait",r=>r.alias=="source-idle"))yield return step;
   foreach(int face in new[]{-1,1}){
    string side=face<0?"left":"right";PlacePlayer(face*175);
    foreach(var step in WaitPose(side+"-walk",r=>r.alias=="source-walk"&&r.facing==face))yield return step;
    Need(game.EnemyHasSuperArmor(target),"real Walk armor");
    foreach(var step in WaitPose(side+"-ready",r=>r.alias=="source-attack-ready"&&r.facing==face&&r.sourceClock>.15f))yield return step;
    Need(game.EnemyHasSuperArmor(target),"real Ready armor");
    foreach(var step in WaitEffect(fire,.32f,face,side+"-fire"))yield return step;
    Need(target.npcState==GuardianCombatNativeRulesV28.Fire&&game.EnemyHasSuperArmor(target),"real Fire armor");
    foreach(var step in WaitPose(side+"-fire-recovery",r=>r.alias=="source-idle"))yield return step;
   }
   PlacePlayer(-64);foreach(var step in WaitHit())yield return step;Hit(HitReaction.Hurt,false);
   foreach(var step in WaitPose("hurt",r=>r.alias=="source-hurt"))yield return step;
   foreach(bool rear in new[]{false,true}){
    foreach(var step in WaitHit())yield return step;
    PlacePlayer((rear?-1:1)*target.facing*64);
    Hit(HitReaction.Launch,false);
    Need(game.FairyRearReactionV34(target)==rear,"actual incoming side");
    string prefix=rear?"rear-":"front-";
    string down=rear?"source-down-back-v34":"source-down";
    string stand=rear?"source-get-up-back-v34":"source-get-up";
    foreach(var step in WaitPose(prefix+"launch",r=>r.alias==down&&target.state==CombatState.Airborne&&target.clip=="launched"))yield return step;
    foreach(var step in WaitPose(prefix+"fall",r=>r.alias==down&&target.state==CombatState.Airborne&&target.clip=="fall"))yield return step;
    foreach(var step in WaitPose(prefix+"down",r=>r.alias==down&&target.state==CombatState.Knockdown))yield return step;
    Need(game.EnemyHasSuperArmor(target)&&target.downTime<=GameModel.FairyDownSecondsV34+.0001f,"real source ground armor");
    float before=target.downTime;Hit(HitReaction.Hurt,false);Need(target.downTime==before&&target.state==CombatState.Knockdown,"down takes HP without resetting phase");
    foreach(var step in WaitPose(prefix+"getup",r=>r.alias==stand&&r.sourceClock>.04f))yield return step;
    Need(game.EnemyHasSuperArmor(target)&&Mathf.Abs(game.FairyGetUpSecondsV34(target)-(rear?11f/6:1.7f))<.00001f,"actual directional rise");
    if(rear)report.rearGroundPath=true;else report.frontGroundPath=true;
   }
   var clips=Field(scene,"audioClips") as IDictionary;
   var played=Field(scene,"audioLastPlayed") as IDictionary;
   foreach(var id in new[]{"npc-fairy-v33-56d4a82b5dc263f1","npc-fairy-v33-f6a9e45a9a491d1b","npc-fairy-v33-a901108fbc50286c","npc-fairy-v33-7e86b9cc866d9c34"}){
    Need(clips!=null&&clips[id] as AudioClip,"packaged original Fairy audio clip "+id);
    Need(played!=null&&played.Contains(id),"production AudioSource.Play issued "+id);
    report.audioPlayIds.Add(id);
   }
   report.allFourOriginalAudioClipPlayIssued=true;
   foreach(var step in WaitHit())yield return step;Hit(HitReaction.Hurt,true);
   foreach(var step in WaitPose("death-early",r=>r.alias=="source-death"&&r.deathTime>.02f))yield return step;
   float alpha=report.captures[report.captures.Count-1].alpha;
   foreach(var step in WaitPose("death-late",r=>r.alias=="source-death"&&r.deathTime>.82f))yield return step;
   Need(report.captures[report.captures.Count-1].alpha<alpha,"source death fade");
   var priorTarget=target;var previousParts=parts.ToArray();var previousRuntimeAtlas=parts[0].sprite.texture;
   var previousSprites=Array.ConvertAll(previousParts,v=>v.sprite);
   var previousAtlas=((FairyPoseLibraryV33)Field(scene,"fairyLibraryV33")).atlas;
   Press("dungeons");for(int j=0;j<3;j++)yield return null;
   report.returnedMenu=(string)Field(scene,"openMenu")=="dungeons"&&game.paused;Need(report.returnedMenu,"ordinary return to menu");
   foreach(var step in EnterRuins())yield return step;for(int j=0;j<5;j++)yield return null;
   BindOriginal();Need(!ReferenceEquals(target,priorTarget),"new ordinary source birth on reentry");
   PlacePlayer(-170);foreach(var step in WaitView())yield return step;
   foreach(var step in WaitPose("reentry-initial",r=>r.alias=="source-trap-wait"))yield return step;
   report.reenteredRuins=true;report.previousPartsReleased=Array.TrueForAll(previousParts,p=>!p);
   report.previousSpritesReleased=Array.TrueForAll(previousSprites,s=>!s);
   report.previousRuntimeAtlasReleased=!previousRuntimeAtlas;
   report.sharedAtlasPreserved=previousAtlas&&ReferenceEquals(previousAtlas,((FairyPoseLibraryV33)Field(scene,"fairyLibraryV33")).atlas);
   Need(report.previousPartsReleased&&report.previousSpritesReleased&&report.previousRuntimeAtlasReleased&&report.sharedAtlasPreserved,"old owned renderer/Sprite/runtime atlas cleanup and original Resources retention");
   Press("dungeons");for(int j=0;j<3;j++)yield return null;Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused,"final ordinary menu");
  }
  void Complete(){
   if(finished)return;finished=true;Application.logMessageReceived-=Log;Camera.onPostRender-=PostRender;int code=report.passed?0:2;
   try{File.WriteAllText(Path.Combine(output,"fairy-player-report.json"),JsonUtility.ToJson(report,true));}
   catch(Exception e){Debug.LogException(e);code=2;}finally{Application.Quit(code);}
  }
  void OnApplicationQuit(){if(started>0)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){Application.logMessageReceived-=Log;Camera.onPostRender-=PostRender;if(observationTarget){observationTarget.Release();Destroy(observationTarget);}}
 }
 public sealed class FairyPartDrawWitnessV33 : MonoBehaviour {
  FairyGuardianPlayerCheckV33 owner;SpriteRenderer rendererRef;
  internal void Bind(FairyGuardianPlayerCheckV33 target,SpriteRenderer renderer){owner=target;rendererRef=renderer;}
  void OnWillRenderObject(){if(owner&&rendererRef)owner.Draw(rendererRef,Camera.current);}
 }
}
#endif
