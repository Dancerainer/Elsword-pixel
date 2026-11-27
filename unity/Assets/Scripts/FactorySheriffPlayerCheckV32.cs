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
 // Explicit opt-in controlled fixture. No Enemy writes, FSM injection, manual
 // GameModel.Step or synthetic source-owner acknowledgements. Window presentation
 // is not verified: all images are actual original world-camera GPU readbacks.
 public sealed class FactorySheriffPlayerCheckV32 : MonoBehaviour {
  const string Flag="--root-factory-sheriff-check-v32",OutFlag="--root-factory-sheriff-output-v32",ModeFlag="--root-factory-sheriff-mode-v32";
  const string Npc="NUI_NASOD_SHERIFF",Effect="DamageEffect_NASOD_SHERIFF_ATTACK_A",Ready="NASOD_SHERIFF_ATTACK_A_READY",Attack="NASOD_SHERIFF_ATTACK_A";
  const int Width=1280,Height=720;const double Budget=480;
  const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
  [Serializable] public sealed class PixelRow {public string label,on,off;public int renderers,drawn,pixels,width,height;public bool worldPost;public Bounds bounds;}
  [Serializable] public sealed class FrameRow {
   public string label,phase,state,motion,clip,image,actor;public int frame,facing,drawnParts;public float elapsed,npcTime,clock,sourceClock,hostClock,hp,deathTime,alpha,visibleTop,barGap,statusGap;
   public bool carrierHidden,sourceClockMatches,labelVisible,worldPost;public Vector3 foot,rigFoot,gunBarrel;public Bounds bounds;public PixelRow pixels;
  }
  [Serializable] public sealed class ShotRow {
   public int cycle,facing,serial;public float expectedTime,previousNpcTime,observedNpcTime,bornAt,age,playerHp,spawnElapsed;public string state,worldImage;
   public Vector3 originalSourceSpawn,projectedSourceSpawn,authoredGunBarrel;public float projectedGunGap, verticalGunGap;public float[] actualAttackLines;public PixelRow laser;
  }
  [Serializable] public sealed class HitRow {public int serial,damage,facing;public float time,playerHp,playerHpAtCycleStart;public Vector3 lineContact;public PixelRow[] particles;}
  [Serializable] public sealed class CycleRow {public int facing;public bool naturalAiEntry,threeOriginalEmissions,positiveHpHit,laserPixels,threeParticleTypes;public float playerHpBefore,playerHpAfter;public List<int> serials=new List<int>();}
  [Serializable] public sealed class Report {
   public string schema="FACTORY_SHERIFF_CONTROLLED_ORIGINAL_WORLD_CAMERA_GPU_V32",mode,failure="",graphics;
   public bool passed,batchMode,windowPresentationVerified=false,screenCaptureClaim=false,offscreenStandaloneGPU=true,naturalRouteClaim=false,controlledOriginalRoomAndPlayer=true,enemyAssigned=false,manualStep=false;
   public bool ordinaryMenuEntry,artCompleted,shotsCompleted,labelsVerified,leftRigVerified,rightRigVerified,deathFadeVerified,returnedMain,reenteredFresh,cleanupVerified;
   public int width=Width,height=Height,actualOriginalTargetDraws;
   public List<string> actions=new List<string>(),errors=new List<string>(),waits=new List<string>(),observed=new List<string>();
   public List<FrameRow> frames=new List<FrameRow>();public List<ShotRow> shots=new List<ShotRow>();public List<HitRow> hits=new List<HitRow>();public List<CycleRow> cycles=new List<CycleRow>();
  }
  DemoScene scene;GameModel game,originalGame;Camera originalCamera;Enemy target;SheriffAuthoredPartsV32 owner;
  readonly List<SpriteRenderer> parts=new List<SpriteRenderer>();readonly Dictionary<SpriteRenderer,int> drawn=new Dictionary<SpriteRenderer,int>();
  Report report=new Report();string output,mode;bool finished,pairRender,backgroundBefore;double started,nextWait;int worldFrame=-1,imageId;RenderTexture rt;FrameRow previous;
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Attach(){
   var args=Environment.GetCommandLineArgs();int n=0,o=0,m=0;string path=null,mode="art";
   for(int i=0;i<args.Length;i++){if(args[i]==Flag)n++;if(args[i]==OutFlag){o++;if(i+1<args.Length)path=args[++i];}if(args[i]==ModeFlag){m++;if(i+1<args.Length)mode=args[++i];}}
   if(n==0)return;
   try{Need(n==1&&o==1&&m<=1&&!string.IsNullOrEmpty(path)&&(mode=="art"||mode=="shots"),"One flag/output pair and mode art or shots");
    foreach(var arg in args)Need(arg!="--root-core-reaction-check-v32"&&arg!="--root-a1-chicken-check-v32"&&arg!="--root-native-occlusion-check-r1462","Mutually driving fixtures must run separately");
    Need(!Application.isEditor&&SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null,"Standalone GPU required; batchmode supported; no nographics");string full=Path.GetFullPath(path);
    Need(full.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(full)&&!File.Exists(full),"Fresh owned C runtime-drafts child required");Directory.CreateDirectory(full);
    var c=new GameObject("Explicit Factory Sheriff actual GPU check V32").AddComponent<FactorySheriffPlayerCheckV32>();c.output=full;c.mode=mode;
   }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
  }
  static void Need(bool ok,string why){if(!ok)throw new InvalidOperationException("Factory Sheriff check V32: "+why);}
  static object Field(object obj,string name){Need(obj!=null,"Owner for "+name);var f=obj.GetType().GetField(name,Fields);Need(f!=null,"Field "+name);return f.GetValue(obj);}
  static object Call(object obj,string name,params object[] args){var m=obj.GetType().GetMethod(name,Fields);Need(m!=null,"Method "+name);return m.Invoke(obj,args);}
  void Log(string s,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert||s.Contains("SHERIFF_AUTHORED_FALLBACK"))report.errors.Add(s+"\n"+stack);}
  void Post(Camera c){if(!pairRender&&scene&&ReferenceEquals(c,Field(scene,"worldCamera")))worldFrame=Time.frameCount;}
  internal void Draw(SpriteRenderer r,Camera c){if(!pairRender&&scene&&ReferenceEquals(c,Field(scene,"worldCamera")))drawn[r]=Time.frameCount;}
  void Check(){Need(report.errors.Count==0,"Runtime error recorded");Need(Time.realtimeSinceStartupAsDouble-started<Budget,"480s total wall deadline");}
  IEnumerator Start(){
   started=Time.realtimeSinceStartupAsDouble;backgroundBefore=Application.runInBackground;Application.runInBackground=true;report.mode=mode;report.graphics=SystemInfo.graphicsDeviceType.ToString();report.batchMode=Application.isBatchMode;Application.logMessageReceived+=Log;Camera.onPostRender+=Post;
   var flow=Flow();Exception error=null;while(!finished){bool more=false;object next=null;try{DrawOriginalOwnerCamera();more=flow.MoveNext();if(more)next=flow.Current;}catch(Exception e){error=e;}if(error!=null||!more)break;yield return next;}
   (flow as IDisposable)?.Dispose();if(finished)yield break;report.failure=error==null?"":error.ToString();report.passed=error==null&&report.errors.Count==0;Complete();
  }
  void Update(){if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>=Budget){report.failure="480s total wall deadline";Complete();}}
  void DrawOriginalOwnerCamera(){
   if(!scene||game==null||!scene.FactoryProductSessionActiveR1179||Field(scene,"factoryRavenTicketV21")==null)return;
   var c=(Camera)Field(scene,"worldCamera");Need(c&&c.enabled&&c.targetTexture==null,"Original authored camera enabled/null-target contract");float clock=game.elapsed;var old=RenderTexture.active;
   try{c.Render();report.actualOriginalTargetDraws++;}finally{RenderTexture.active=old;}Need(game.elapsed==clock&&c.targetTexture==null,"Original camera draw cannot step model");
  }
  void Press(string action){
   TouchControl control=null;foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))if(c&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy&&ReferenceEquals(c.owner,scene)&&c.action==action){Need(!control,"Duplicate active control "+action);control=c;}
   Need(control&&EventSystem.current,"Active ordinary UI "+action);var img=control.GetComponent<Image>();Need(img&&img.raycastTarget,"Raycastable ordinary UI");var p=new PointerEventData(EventSystem.current){pointerId=-3232,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
   report.actions.Add("real-touch:"+action);try{Need(ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerDownHandler),"Pointer down rejected");}finally{if(control)ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerUpHandler);}
  }
  IEnumerable EnterFactory(){
   var routes=(IList)Field(scene,"mapDungeons");int index=-1;for(int i=0;i<routes.Count;i++)if((string)Field(routes[i],"id")=="altera-nasod-factory")index=i;Need(index>=0,"Ordinary Factory route");Press("region:altera");yield return null;
   var visible=(IList)Field(scene,"mapVisible");int at=-1;for(int i=0;i<visible.Count;i++)if((int)visible[i]==index)at=i;Need(at>=0,"Visible Factory route");int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();int page=at/size,guard=0;
   while((int)Field(scene,"mapPage")!=page){Need(++guard<20,"Menu page bound");Press("dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1"));yield return null;}Press("dungeon-card:"+(at%size));yield return null;Need((int)Field(scene,"mapSelected")==index,"Selected Factory card");Press("dungeon-enter");
   while(!scene.FactoryProductSessionActiveR1179||scene.FactoryProductSessionLoadingR1179){Check();yield return null;}game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id=="Dungeon_Altera_Nasod_Factory_Normal-0"&&!game.paused,"Original Factory room0 running");report.ordinaryMenuEntry=true;
  }
  void Stage(){
   Call(game,"EnterRoom",0);if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);Need(game.enemies.Count>2,"Original Factory birth list");target=game.enemies[2];Need(target.kind=="nasod-sheriff"&&target.sourceNPC==Npc&&target.encounterIndex==0,"Original room0 encounter0 index2 Sheriff");game.traceNpcEffects=true;previous=null;drawn.Clear();parts.Clear();owner=null;nextWait=0;PlacePlayer(-160,true);report.actions.Add("CONTROLLED original EnterRoom(0)/BeginEncounter(0), original birth index2; no Enemy assignment");
  }
  void PlacePlayer(float offset,bool protectedPlayer){
   game.player.x=target.x+offset;game.player.y=target.y;game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;game.player.invincible=protectedPlayer?120:0;
   report.actions.Add("CONTROLLED player x=actor.x+"+offset+", same support; velocity0; invincible="+(protectedPlayer?120:0));
  }
  void HoldShotPlayer(int facing){game.player.x=target.x+facing*160;game.player.y=target.y;game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;}
  bool CanHit(){return target!=null&&target.active&&!target.dormant&&target.hp>0&&target.state==CombatState.Normal&&!string.IsNullOrEmpty(target.npcState)&&(bool)Call(game,"Grounded",target)&&!(bool)Call(game,"EnemyHasSuperArmor",target)&&!(bool)Call(game,"EnemyHasNpcInvincibility",target);}
  bool Resolve(){parts.Clear();if(!scene||game==null||target==null)return false;var views=(IDictionary)Field(scene,"sheriffViewsV32");if(!views.Contains(target))return false;owner=Field(views[target],"parts") as SheriffAuthoredPartsV32;if(owner==null)return false;owner.VisitRenderersR1461(r=>parts.Add(r));return parts.Count==6;}
  void WaitLog(string why){if(Time.realtimeSinceStartupAsDouble<nextWait)return;nextWait=Time.realtimeSinceStartupAsDouble+1;report.waits.Add(why+" elapsed="+(game==null?0:game.elapsed)+" actor="+(target==null?"null":JsonUtility.ToJson(target)));}
  IEnumerable WaitReady(){double end=Time.realtimeSinceStartupAsDouble+35;while(true){yield return null;Check();if(CanHit()&&Resolve())yield break;WaitLog("source-initialized-grounded-parts");Need(Time.realtimeSinceStartupAsDouble<end,"Wait original source actor and authored parts");}}
  void Bind(IEnumerable<SpriteRenderer> list){foreach(var r in list){Need(r,"Live draw renderer");var w=r.GetComponent<FactorySheriffDrawWitnessV32>();if(!w)w=r.gameObject.AddComponent<FactorySheriffDrawWitnessV32>();w.Bind(this,r);}}
  void Render(IEnumerable<SpriteRenderer> list){
   Bind(list);if(!rt){rt=new RenderTexture(Width,Height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"1280x720 GPU target");}
   var c=(Camera)Field(scene,"worldCamera");var old=c.targetTexture;var active=RenderTexture.active;var rect=c.rect;float aspect=c.aspect,elapsed=game.elapsed;string before=JsonUtility.ToJson(target);drawn.Clear();worldFrame=-1;
   try{pairRender=false;c.targetTexture=rt;c.rect=new Rect(0,0,1,1);c.aspect=(float)Width/Height;RenderTexture.active=rt;GL.Clear(true,true,Color.clear);c.Render();}
   finally{c.targetTexture=old;c.rect=rect;c.aspect=aspect;RenderTexture.active=active;}Need(game.elapsed==elapsed&&JsonUtility.ToJson(target)==before,"Readback render cannot step or mutate actor");
  }
  static void Save(Texture2D image,string path){using(var f=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){var b=image.EncodeToPNG();f.Write(b,0,b.Length);}}
  Texture2D Read(){var t=new Texture2D(Width,Height,TextureFormat.RGBA32,false);t.ReadPixels(new Rect(0,0,Width,Height),0,0,false);t.Apply(false,false);return t;}
  string WorldImage(string label){var active=RenderTexture.active;Texture2D image=null;try{RenderTexture.active=rt;image=Read();var path=Path.Combine(output,(imageId++).ToString("D3")+"-"+label+"-world.png");Save(image,path);return path;}finally{RenderTexture.active=active;if(image)Destroy(image);}}
  static Bounds BoundsOf(IList<SpriteRenderer> renderers){Need(renderers.Count>0,"Nonempty renderer group");var b=renderers[0].bounds;for(int i=1;i<renderers.Count;i++)b.Encapsulate(renderers[i].bounds);return b;}
  PixelRow Pixels(SpriteRenderer[] renderers,string label){
   Need(renderers.Length>0,"Pixel group "+label);Render(renderers);var row=new PixelRow{label=label,renderers=renderers.Length,bounds=BoundsOf(renderers),worldPost=worldFrame==Time.frameCount};
   foreach(var r in renderers){Need(r.enabled&&r.gameObject.activeInHierarchy&&r.sprite&&!r.forceRenderingOff&&r.color.a>0,"Actual eligible renderer "+label);int frame;if(drawn.TryGetValue(r,out frame)&&frame==Time.frameCount)row.drawn++;}
   Need(row.worldPost&&row.drawn>0,"Actual same-frame draw "+label);var c=(Camera)Field(scene,"worldCamera");var old=c.targetTexture;var active=RenderTexture.active;var rect=c.rect;float aspect=c.aspect,elapsed=game.elapsed;string before=JsonUtility.ToJson(target);Texture2D on=null,off=null;var enabled=new bool[renderers.Length];for(int i=0;i<renderers.Length;i++)enabled[i]=renderers[i].enabled;
   try{pairRender=true;c.targetTexture=rt;c.rect=new Rect(0,0,1,1);c.aspect=(float)Width/Height;RenderTexture.active=rt;GL.Clear(true,true,Color.clear);c.Render();on=Read();foreach(var r in renderers)r.enabled=false;GL.Clear(true,true,Color.clear);c.Render();off=Read();
    var a=on.GetPixels32();var b=off.GetPixels32();var lo=c.WorldToViewportPoint(row.bounds.min);var hi=c.WorldToViewportPoint(row.bounds.max);int x0=Mathf.Clamp(Mathf.FloorToInt(lo.x*Width)-2,0,Width),x1=Mathf.Clamp(Mathf.CeilToInt(hi.x*Width)+2,0,Width),y0=Mathf.Clamp(Mathf.FloorToInt(lo.y*Height)-2,0,Height),y1=Mathf.Clamp(Mathf.CeilToInt(hi.y*Height)+2,0,Height);int left=Width,right=-1,bottom=Height,top=-1;
    for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++){int n=y*Width+x;if(Math.Abs(a[n].r-b[n].r)+Math.Abs(a[n].g-b[n].g)+Math.Abs(a[n].b-b[n].b)>6){row.pixels++;left=Math.Min(left,x);right=Math.Max(right,x);bottom=Math.Min(bottom,y);top=Math.Max(top,y);}}
    row.width=right>=left?right-left+1:0;row.height=top>=bottom?top-bottom+1:0;string stem=(imageId++).ToString("D3")+"-"+label;row.on=Path.Combine(output,stem+"-on.png");row.off=Path.Combine(output,stem+"-off.png");Save(on,row.on);Save(off,row.off);
   }finally{for(int i=0;i<renderers.Length;i++)if(renderers[i])renderers[i].enabled=enabled[i];c.targetTexture=old;c.rect=rect;c.aspect=aspect;RenderTexture.active=active;pairRender=false;if(on)Destroy(on);if(off)Destroy(off);}
   Need(game.elapsed==elapsed&&JsonUtility.ToJson(target)==before,"Pixel difference cannot step or mutate actor");Need(row.pixels>0&&row.width>0&&row.height>0,"No actual GPU pixel contribution: "+label);return row;
  }
  Vector3 GunBarrel(){var q=SheriffAuthoredPoseV32.Part(3);float u=SheriffAuthoredPoseV32.UnitsPerArtPixel;return parts[3].transform.TransformPoint(new Vector3((SheriffAuthoredPoseV32.GunMuzzleX-q.pivotX)*u,(q.pivotY-SheriffAuthoredPoseV32.GunMuzzleY)*u,0));}
  string Phase(){if(target.state==CombatState.Airborne&&(target.clip=="launched"||target.knockdownOnLanding))return target.vy>0?"Launch":"Fall";return target.state.ToString();}
  FrameRow Observe(string label,bool capture){
   var row=new FrameRow{label=label,phase=Phase(),state=target.npcState,clip=target.clip,frame=Time.frameCount,facing=target.facing,elapsed=game.elapsed,npcTime=target.npcTime,clock=target.clock,hp=target.hp,deathTime=target.deathTime,actor=JsonUtility.ToJson(target)};report.frames.Add(row);
   Need(Resolve(),"Six-part owner missing: "+label+" "+row.phase+"/"+row.state);Render(parts);var committed=owner.LastCommittedFrame;row.state=committed.state;row.motion=committed.motion;row.hostClock=committed.hostClock;row.sourceClock=committed.sourceClock;row.alpha=parts[0].color.a;row.worldPost=worldFrame==Time.frameCount;row.bounds=BoundsOf(parts);row.gunBarrel=GunBarrel();row.rigFoot=parts[0].transform.parent.position;
   int index=game.enemies.IndexOf(target);var carrier=((IList)Field(scene,"enemyViews"))[index] as SpriteRenderer;row.carrierHidden=carrier&&carrier.forceRenderingOff;
   var root=(Transform)Field(scene,"actorsRoot");var local=(Vector2)Call(scene,"RenderEnemyPosition",target);var foot=root.TransformPoint(new Vector3(local.x,local.y,0));row.foot=(Vector3)Call(scene,"ProjectNativePoint",foot,target.nativeDepth);Need(Vector3.Distance(row.foot,row.rigFoot)<.001f,"Original projected foot preserved");
   foreach(var r in parts){int frame;Need(r.enabled&&r.gameObject.activeInHierarchy&&r.sprite&&!r.forceRenderingOff&&Math.Abs(r.color.a-row.alpha)<.000001f,"Eligible parts/share death alpha");if(drawn.TryGetValue(r,out frame)&&frame==Time.frameCount)row.drawnParts++;}
   Need(committed.facing==target.facing&&Math.Sign(parts[0].transform.parent.localScale.x)==(target.facing<0?-1:1),"Owned rig follows actual Enemy.facing");if(target.facing<0)report.leftRigVerified=true;else report.rightRigVerified=true;
   var map=(FactoryReactionPoseMapR1467)Field(scene,"factoryReactionPoseMapR1467");string state;float clock;bool reaction;Need(map.TrySelect(target,out state,out clock,out reaction),"Actual reaction/source selection");var banks=(Dictionary<string,SourcePhaseWholeLookupV21>)Field(scene,"factoryProductReferenceR1179");var selected=banks[Npc].Select(state,clock);row.sourceClockMatches=state==committed.state&&Math.Abs(committed.hostClock-selected.ExistingNpcClock)<.000001f&&Math.Abs(committed.sourceClock-selected.SourceMotionClock)<.000001f;Need(row.sourceClockMatches,"Actual existing selected state/host/source clocks");
   var bar=((IList)Field(scene,"enemyBars"))[index] as SpriteRenderer;var status=((IList)Field(scene,"enemyDebuffViews"))[index] as TextMesh;row.labelVisible=bar&&bar.enabled&&bar.gameObject.activeInHierarchy;
   if(owner.TryVisibleTop(out row.visibleTop)){row.barGap=bar?bar.transform.position.y-row.visibleTop:0;row.statusGap=status?status.transform.position.y-row.visibleTop:0;}
   if(row.labelVisible){Need(Math.Abs(row.barGap-8)<.002f&&Math.Abs(row.statusGap-20)<.002f,"Labels anchored above visible top");report.labelsVerified=true;}
   Need(row.drawnParts==6&&row.worldPost&&row.alpha>0&&row.carrierHidden,"Actual six parts drawn in original world camera");if(!report.observed.Contains(row.phase))report.observed.Add(row.phase);
   if(capture){row.image=WorldImage(label);row.pixels=Pixels(parts.ToArray(),label+"-six-parts");}previous=row;return row;
  }
  IEnumerable WaitPose(string state,int facing,string label){double end=Time.realtimeSinceStartupAsDouble+25;while(true){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<end,"Natural AI pose "+state+" facing "+facing);if(target.npcState==state&&target.facing==facing&&target.npcTime>.08f){Observe(label,true);yield break;}WaitLog("natural-ai "+state);}}
  void Hit(HitReaction reaction,bool lethal=false){int amount=lethal?(int)Math.Ceiling(target.hp)+1:1;float hp=target.hp;int dealt=game.ApplyHitToEnemy(target,amount,reaction,280);report.actions.Add("PUBLIC ApplyHitToEnemy "+reaction+" damage="+amount+" dealt="+dealt);Need(dealt>0&&target.hp<hp,"Public hit connects");}
  IEnumerable Art(){
   foreach(var x in WaitReady())yield return x;Observe("initial",true);PlacePlayer(-160,true);foreach(var x in WaitPose(Ready,-1,"ready-left"))yield return x;
   PlacePlayer(160,true);foreach(var x in WaitPose(Ready,1,"ready-right"))yield return x;
   foreach(var x in WaitReady())yield return x;PlacePlayer(-80,true);Hit(HitReaction.Hurt);yield return null;Check();Need(target.state==CombatState.Hurt,"Observed actual first hurt");Observe("hurt-first",true);float old=target.clock;Need(old>0,"First hurt advances with ordinary engine Update");Hit(HitReaction.Hurt);Need(target.clock==0,"Public repeated hurt resets its actual clock");yield return null;Check();Observe("hurt-repeat",true);
   foreach(var x in WaitReady())yield return x;Hit(HitReaction.Launch);double end=Time.realtimeSinceStartupAsDouble+25;var seen=new HashSet<string>();
   do{yield return null;Check();string p=Phase();if(target.state!=CombatState.Normal)Observe("reaction-"+p,seen.Add(p));Need(Time.realtimeSinceStartupAsDouble<end,"Public launch/down/getup recovery");}while(target.state!=CombatState.Normal);
   foreach(string p in new[]{"Launch","Fall","Knockdown","GettingUp"})Need(seen.Contains(p),"Missing real reaction "+p);
   foreach(var x in WaitReady())yield return x;Hit(HitReaction.Hurt,true);yield return null;Check();var early=Observe("death-early",true);end=Time.realtimeSinceStartupAsDouble+8;
   while(target.deathTime<.55f){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<end,"Death clock advance");}var mid=Observe("death-middle",true);Need(early.alpha>mid.alpha&&mid.alpha>0,"Original death alpha decreases on actual parts");report.deathFadeVerified=true;
   Need(report.leftRigVerified&&report.rightRigVerified&&report.labelsVerified,"Both facings and actual visible HP labels");report.artCompleted=true;
  }
  List<SpriteRenderer> LaserGroup(Enemy wanted){
   var pool=(IList)Field(scene,"factorySheriffSpritesV32");var result=new List<SpriteRenderer>();int i=0;
   foreach(var e in game.npcEffects){if(e.id!=Effect||e.owner==null||e.owner.sourceNPC!=Npc||e.ended||e.age<0||e.age>=e.lifetime)continue;Need(i<pool.Count,"Production active laser pool index");var r=pool[i++] as SpriteRenderer;if(ReferenceEquals(e.owner,wanted))result.Add(r);}return result;
  }
  List<SpriteRenderer> ParticleGroup(int type){var sprites=(Sprite[])Field(scene,"factorySheriffParticleSpritesV32");var result=new List<SpriteRenderer>();foreach(SpriteRenderer r in (IList)Field(scene,"factorySheriffSpritesV32"))if(r&&r.enabled&&r.gameObject.activeInHierarchy&&r.sprite==sprites[type]&&r.color.a>0)result.Add(r);return result;}
  Vector3 ProjectEffect(NpcPresentationEvent e){var root=(Transform)Field(scene,"actorsRoot");return (Vector3)Call(scene,"ProjectNativePoint",root.TransformPoint(new Vector3(e.x,e.y,0)),e.depth+e.z/.28f);}
  IEnumerable Shots(int facing){
   Stage();foreach(var x in WaitReady())yield return x;PlacePlayer(facing*160,true);foreach(var x in WaitPose(Ready,facing,"shots-ready-"+facing))yield return x;
   var cycle=new CycleRow{facing=facing,naturalAiEntry=true};report.cycles.Add(cycle);int cycleId=report.cycles.Count;var seen=new HashSet<int>();var hitSeen=new HashSet<int>();float previousClock=0;bool entered=false;HitRow pendingParticleHit=null;double end=Time.realtimeSinceStartupAsDouble+35;
   // Controlled high HP prevents unrelated nearby actors ending the fixture.
   // Invincibility is cleared once; ordinary hit invincibility remains intact.
   game.player.hp=game.player.maxHp=5000;game.player.invincible=0;cycle.playerHpBefore=game.player.hp;report.actions.Add("CONTROLLED shot cycle: player hp/maxHp5000, invincibility cleared once; hold original support at actor.x+facing*160; no Enemy changes");
   while(true){
    HoldShotPlayer(facing);yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<end,"Natural AI triple shot deadline");Need(target.hp>0&&target.state==CombatState.Normal,"Original firing actor must not be interrupted");
    if(target.npcState==Attack){Need(target.facing==facing,"AI attack direction");entered=true;}else if(entered)break;
    var spawns=new List<NpcPresentationEvent>();foreach(var p in game.npcPresentationEvents)if(p.type=="spawn"&&p.effect==Effect&&ReferenceEquals(p.owner,target)&&!seen.Contains(p.serial))spawns.Add(p);
    foreach(var spawn in spawns){
     Need(entered&&target.npcState==Attack,"Emission during actual original attack");seen.Add(spawn.serial);cycle.serials.Add(spawn.serial);int n=cycle.serials.Count-1;Need(n<3,"Exactly three source emissions");float expected=new[]{.55f,1.23f,1.84f}[n];var live=game.npcEffects.Find(e=>e.serial==spawn.serial&&ReferenceEquals(e.owner,target));Need(live!=null&&!live.ended,"Actual new effect instance retained");
     var f=Observe("shot-"+cycleId+"-"+(n+1),true);var row=new ShotRow{cycle=cycleId,facing=facing,serial=spawn.serial,expectedTime=expected,previousNpcTime=previousClock,observedNpcTime=target.npcTime,bornAt=live.bornAt,age=live.age,playerHp=game.player.hp,spawnElapsed=spawn.time,state=target.npcState,originalSourceSpawn=new Vector3(spawn.x,spawn.y,spawn.z),projectedSourceSpawn=ProjectEffect(spawn),authoredGunBarrel=f.gunBarrel,actualAttackLines=game.NpcEffectLines(live),worldImage=f.image};report.shots.Add(row);
     var binding=(NpcEffectBinding)Call(game,"EffectBinding",target,Attack);Need(binding!=null&&binding.sets.Length==3&&binding.sets[n].attachAt==expected&&ReferenceEquals(binding.sets[n].emissions[0],live.emission),"Actual instance uses exact original callback emission");Need(previousClock<=expected+.00001f&&target.npcTime>=expected-.00001f&&live.emission.start==0&&live.facing==facing,"Real source emission threshold crossed at correct source clock/facing");row.projectedGunGap=Vector3.Distance(row.projectedSourceSpawn,row.authoredGunBarrel);row.verticalGunGap=Mathf.Abs(row.projectedSourceSpawn.y-row.authoredGunBarrel.y);Need(row.verticalGunGap<=2f,"Authored gun axis must meet the source laser within two logical pixels at emission");var lasers=LaserGroup(target);Need(lasers.Count>0,"Actual target-owner laser renderer");row.laser=Pixels(lasers.ToArray(),"shot-"+cycleId+"-"+(n+1)+"-laser");cycle.laserPixels=true;
    }
    foreach(var hit in game.npcPresentationEvents){if(hit.type!="hit"||hit.effect!=Effect||!ReferenceEquals(hit.owner,target)||!seen.Contains(hit.serial)||!hitSeen.Add(hit.serial))continue;
     var damage=game.npcEffectEvents.Find(e=>e.type=="hit-player"&&e.effect==Effect&&e.serial==hit.serial);Need(damage!=null&&damage.damage>0&&game.player.hp<cycle.playerHpBefore,"Actual original source line hit caused positive HP loss");
     var row=new HitRow{serial=hit.serial,damage=damage.damage,facing=facing,time=hit.time,playerHp=game.player.hp,playerHpAtCycleStart=cycle.playerHpBefore,lineContact=new Vector3(hit.x,hit.y,hit.z)};report.hits.Add(row);cycle.positiveHpHit=true;
     if(!cycle.threeParticleTypes&&pendingParticleHit==null)pendingParticleHit=row;
    }
    if(pendingParticleHit!=null&&!cycle.threeParticleTypes){
     var groups=new[]{ParticleGroup(0),ParticleGroup(1),ParticleGroup(2)};bool available=groups[0].Count>0&&groups[1].Count>0&&groups[2].Count>0;
     if(available){foreach(NpcPresentationEvent activeHit in (IList)Field(scene,"factorySheriffImpactsV32"))Need(ReferenceEquals(activeHit.owner,target),"Particle pixel proof requires target-only live impacts; unrelated owner is present");pendingParticleHit.particles=new PixelRow[3];for(int i=0;i<3;i++)pendingParticleHit.particles[i]=Pixels(groups[i].ToArray(),"shot-"+cycleId+"-impact-type-"+i);cycle.threeParticleTypes=true;}
     else Need(game.elapsed-pendingParticleHit.time<.35f,"All three actual impact particle classes must appear within source lifetime");
    }
    if(target.npcState==Attack)previousClock=target.npcTime;
   }
   cycle.playerHpAfter=game.player.hp;cycle.threeOriginalEmissions=cycle.serials.Count==3;Need(entered&&cycle.threeOriginalEmissions&&cycle.positiveHpHit&&cycle.laserPixels&&cycle.threeParticleTypes,"Three real shots, positive HP hit, laser and all three particle classes required");
  }
  IEnumerable ReturnAndReenter(){
   var priorModel=game;var priorActor=target;var oldParts=parts.ToArray();var oldFx=new List<SpriteRenderer>();foreach(SpriteRenderer r in (IList)Field(scene,"factorySheriffSpritesV32"))oldFx.Add(r);target=null;Press("dungeons");for(int i=0;i<4;i++)yield return null;
   report.returnedMain=ReferenceEquals(Field(scene,"game"),originalGame)&&ReferenceEquals(Field(scene,"worldCamera"),originalCamera)&&!scene.FactoryProductSessionActiveR1179&&(string)Field(scene,"openMenu")=="dungeons";
   Need(report.returnedMain,"Original Main model/camera restored");Need(((IDictionary)Field(scene,"sheriffViewsV32")).Count==0&&((IList)Field(scene,"factorySheriffSpritesV32")).Count==0&&Field(scene,"factorySheriffVisualV32")==null&&Field(scene,"factoryReactionPoseMapR1467")==null,"Owned parts/effect pools/config released");foreach(var r in oldParts)Need(!r,"Old part destroyed");foreach(var r in oldFx)Need(!r,"Old effect renderer destroyed");report.cleanupVerified=true;
   foreach(var x in EnterFactory())yield return x;Stage();foreach(var x in WaitReady())yield return x;Need(!ReferenceEquals(game,priorModel)&&!ReferenceEquals(target,priorActor),"Fresh original component and birth after reentry");Observe("reentry-fresh",true);report.reenteredFresh=true;target=null;Press("dungeons");for(int i=0;i<4;i++)yield return null;Need(!scene.FactoryProductSessionActiveR1179&&ReferenceEquals(Field(scene,"game"),originalGame),"Final ordinary Main return");
  }
  IEnumerator Flow(){
   while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Check();yield return null;}for(int i=0;i<3;i++)yield return null;game=(GameModel)Field(scene,"game");originalGame=game;originalCamera=(Camera)Field(scene,"worldCamera");Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused,"Ordinary Main menu");
   foreach(var x in EnterFactory())yield return x;if(mode=="art"){Stage();foreach(var x in Art())yield return x;}else{foreach(var x in Shots(-1))yield return x;foreach(var x in Shots(1))yield return x;report.shotsCompleted=true;}
   foreach(var x in ReturnAndReenter())yield return x;
  }
  void Complete(){if(finished)return;finished=true;Application.logMessageReceived-=Log;Camera.onPostRender-=Post;int exit=report.passed?0:2;try{File.WriteAllText(Path.Combine(output,"factory-sheriff-player-report.json"),JsonUtility.ToJson(report,true),new System.Text.UTF8Encoding(false));}catch(Exception e){Debug.LogException(e);exit=2;}finally{Application.Quit(exit);}}
  void OnApplicationQuit(){if(started>0)Application.runInBackground=backgroundBefore;}
  void OnDestroy(){Application.logMessageReceived-=Log;Camera.onPostRender-=Post;if(rt){rt.Release();Destroy(rt);rt=null;}}
 }
 public sealed class FactorySheriffDrawWitnessV32 : MonoBehaviour {FactorySheriffPlayerCheckV32 owner;SpriteRenderer body;internal void Bind(FactorySheriffPlayerCheckV32 o,SpriteRenderer r){owner=o;body=r;}void OnWillRenderObject(){if(owner&&body)owner.Draw(body,Camera.current);}}
}
#endif
