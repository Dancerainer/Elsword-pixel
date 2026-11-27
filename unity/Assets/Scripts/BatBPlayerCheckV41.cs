#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed class BatBPlayerCheckV41 : MonoBehaviour {
        const string Flag="--root-bat-b-check-v41", OutFlag="--root-bat-b-output-v41", Page="bat-b-painted-v41";
        const int Width=1280,Height=720,Budget=300;
        const BindingFlags Fields=BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance;
        [Serializable] public sealed class Shot {
            public string id,sprite,texture,phase,state,clip,onFile,offFile;
            public int frame,sourceFrame,expectedFrame,facing,pixels,xMin,xMax,yMin,yMax,draws,posts;
            public float elapsed,sourceClock,npcTime,x,y,playerX,playerY,hp,playerHp,alpha;
            public bool flipX,flipY;
        }
        [Serializable] public sealed class Report {
            public string error="",last="",graphics,room;
            public bool passed,batchMode,ordinaryEntry,originalBirth,hanging,release,hover,left,right,ready,bite,drain,
                publicHurt,publicLaunch,deathFade,returnedToMenu,reentered,offscreenStandaloneGPU=true,windowPresentationVerified=false;
            public int sourceSpawnIndex,actorIndex,contactDamage,drainDamage;
            public float damagedHp,healedHp,playerHpBefore,playerHpAfter;
            public List<string> actions=new List<string>(),errors=new List<string>();
            public List<Shot> shots=new List<Shot>();
            public List<NpcActionEvent> contacts=new List<NpcActionEvent>();
        }
        Report report=new Report();
        DemoScene scene;
        GameModel game;
        Enemy target;
        SpriteRenderer carrier;
        BatBDrawWitnessV41 witness;
        AnimationData[] sourceClips;
        string output;
        double started;
        bool done,oldBackground,oldTrace,traceSet;
        RenderTexture rt;
        static void Need(bool ok,string message) { if(!ok)throw new InvalidOperationException("Bat B Player V41 "+message); }
        static object Field(object o,string name) {
            var f=o.GetType().GetField(name,Fields); Need(f!=null,"Missing "+name); return f.GetValue(o);
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            var args=Environment.GetCommandLineArgs();
            if(Array.IndexOf(args,Flag)<0)return;
            try {
                int at=Array.IndexOf(args,OutFlag);
                Need(Array.IndexOf(args,Flag)==Array.LastIndexOf(args,Flag) && at>=0 && at==Array.LastIndexOf(args,OutFlag) && at+1<args.Length,"One explicit check/output pair");
                Need(Array.FindAll(args,a=>a.StartsWith("--root-",StringComparison.Ordinal)&&a.Contains("-check-v")).Length==1,"Separate driving checks");
                Need(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11&&!Application.isEditor,"Standalone D3D11 required");
                string path=Path.GetFullPath(args[at+1]);
                string allowed=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allowed,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh runtime-drafts output");
                Directory.CreateDirectory(path);
                new GameObject("Explicit Bat B Player V41").AddComponent<BatBPlayerCheckV41>().output=path;
            } catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
        }
        void Log(string message,string stack,LogType type) {
            if(type==LogType.Error||type==LogType.Assert||type==LogType.Exception)report.errors.Add(message+"\n"+stack);
        }
        void Check() {
            Need(report.errors.Count==0,"Runtime error/fallback");
            Need(Time.realtimeSinceStartupAsDouble-started<Budget,"Total deadline");
        }
        IEnumerator Start() {
            started=Time.realtimeSinceStartupAsDouble;
            oldBackground=Application.runInBackground; Application.runInBackground=true;
            report.graphics=SystemInfo.graphicsDeviceType.ToString(); report.batchMode=Application.isBatchMode;
            Application.logMessageReceived+=Log;
            var run=Flow(); Exception failure=null;
            try {
                while (!done) {
                    bool more=false; object next=null;
                    try { Check(); more=run.MoveNext(); if (more) next=run.Current; }
                    catch (Exception ex) { failure=ex; break; }
                    if (!more) break;
                    yield return next;
                }
            } finally { (run as IDisposable)?.Dispose(); }
            if (done) yield break;
            if (failure!=null) report.error=failure.ToString();
            report.passed=failure==null && report.errors.Count==0;
            Finish();
        }
        void Update() {
            if (!done && started>0 && Time.realtimeSinceStartupAsDouble-started>=Budget) {
                report.error="Total deadline at "+report.last; Finish();
            }
        }
        void Finish() {
            if (done) return; done=true;
            Application.logMessageReceived-=Log;
            if (traceSet && game!=null) game.traceNpcActions=oldTrace;
            Application.runInBackground=oldBackground;
            try { File.WriteAllText(Path.Combine(output,"bat-b-player-report.json"),JsonUtility.ToJson(report,true)); }
            finally { Application.Quit(report.passed?0:2); }
        }
        void OnDestroy() {
            Application.logMessageReceived-=Log;
            if (rt) {rt.Release();Destroy(rt);}
        }
        void Press(string action) {
            TouchControl control=null;
            foreach (var c in scene.GetComponentsInChildren<TouchControl>(true))
                if (c && c.isActiveAndEnabled && c.gameObject.activeInHierarchy && ReferenceEquals(c.owner,scene) && c.action==action) {
                    Need(!control,"Duplicate control "+action); control=c;
                }
            Need(control && EventSystem.current && control.GetComponent<Image>().raycastTarget,"Visible original control "+action);
            var p=new PointerEventData(EventSystem.current){pointerId=-3434,button=PointerEventData.InputButton.Left,
                position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
            report.actions.Add("touch "+action);
            try { Need(ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerDownHandler),"Pointer down "+action); }
            finally { if (control) ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerUpHandler); }
        }
        IEnumerable EnterCave() {
            var routes=(IList)Field(scene,"mapDungeons"); int route=-1;
            for (int i=0;i<routes.Count;i++) if ((string)Field(routes[i],"id")=="banthus-cave") route=i;
            Need(route>=0,"Ordinary Banthus Cave card");
            Press("region:elder"); yield return null;
            var visible=(IList)Field(scene,"mapVisible"); int index=-1;
            for (int i=0;i<visible.Count;i++) if ((int)visible[i]==route) index=i;
            Need(index>=0,"Visible Elder route");
            var pageSize=(int)scene.GetType().GetField("MapPageSize",BindingFlags.NonPublic|BindingFlags.Static).GetRawConstantValue();
            int page=index/pageSize,attempt=0;
            while ((int)Field(scene,"mapPage")!=page) {
                Need(++attempt<15,"Map page bound"); Press("dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1")); yield return null;
            }
            Press("dungeon-card:"+(index%pageSize)); yield return null;
            Need((int)Field(scene,"mapSelected")==route,"Original cave selected");
            Press("dungeon-enter");
            for (int i=0;i<4;i++) yield return null;
            game=(GameModel)Field(scene,"game");
            Need(!game.paused && game.room==0 && game.CurrentRoom.id=="cave-1","Original cave room1");
            var stage=Array.Find(((ArtCatalog)Field(scene,"catalog")).stages,s=>s.id==game.CurrentRoom.id);
            Need(stage!=null,"Cave source stage");
            int actorIndex=Array.FindIndex(stage.spawns,s=>s.sourceSpawnIndex==10 && s.sourceNPC=="NUI_BAT_B" && s.encounterIndex==0);
            Need(actorIndex>=0 && actorIndex<game.enemies.Count,"Original first encounter Bat B");
            target=game.enemies[actorIndex];
            Need(target.sourceNPC=="NUI_BAT_B" && target.kind=="bat-b" && target.encounterIndex==0 && target.active && !target.dormant,"Original Bat B birth");
            report.room=stage.id;report.actorIndex=actorIndex;report.sourceSpawnIndex=10;
            var lib=((IDictionary)Field(scene,"libraries"))["bat-b"];
            sourceClips=((SpriteLibraryData)Field(lib,"data")).clips;
            report.ordinaryEntry=report.originalBirth=true;
        }
        void Place(float x,bool floorOnly=true) {
            var floor=game.SurfaceBelow(x,target.y+(floorOnly?.2f:96),floorOnly);
            Need(floor!=null,"Original floor below controlled player at "+x);
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;
            game.player.vx=game.player.vy=0;
        }
        string Resolve(out float clock) {
            int i=game.enemies.IndexOf(target);
            clock=0;if(i<0)return "";
            carrier=(SpriteRenderer)((IList)Field(scene,"enemyViews"))[i];
            if(!carrier||!carrier.sprite)return "";
            string name=carrier.sprite.name;const string prefix="bat-b/";
            int slash=name.LastIndexOf('/');
            if(!name.StartsWith(prefix+"source-",StringComparison.Ordinal)||slash<=prefix.Length)return name;
            string alias=name.Substring(prefix.Length,slash-prefix.Length);
            clock=alias=="source-start"||alias=="source-wait-zero2"?game.EnemyInitialPoseClipClockV25(target):
                (float)scene.GetType().GetMethod("First3ActualRenderClockV25",Fields).Invoke(scene,new object[]{target});
            return alias;
        }
        Shot Capture(string id,string expect) {
            string alias=Resolve(out float clock);
            Need(alias==expect && carrier.enabled && !carrier.forceRenderingOff && carrier.gameObject.activeInHierarchy,"Visible source-selected carrier "+id);
            Need(carrier.sprite.texture.name==Page && carrier.sprite.texture.filterMode==FilterMode.Point,"Actual authored Point page "+id+" got "+carrier.sprite.texture.name);
            Need(carrier.GetComponentsInChildren<SpriteRenderer>(true).Length==1,"Single Bat B body carrier");
            if(!witness||witness.gameObject!=carrier.gameObject)witness=carrier.gameObject.AddComponent<BatBDrawWitnessV41>();
            var cam=(Camera)Field(scene,"worldCamera");Need(cam,"Original world camera");
            var info=Array.Find(sourceClips,c=>c.id==alias);Need(info!=null && info.frames.Length>0,"Source clip metadata "+alias);
            float sampled=info.loop && info.duration>0?Mathf.Repeat(clock,info.duration):clock;
            int expected=Mathf.Clamp(Mathf.FloorToInt(sampled*info.fps),0,info.frames.Length-1);
            int actual=int.Parse(carrier.sprite.name.Substring(carrier.sprite.name.LastIndexOf('/')+1));
            var row=new Shot{id=id,sprite=carrier.sprite.name,texture=carrier.sprite.texture.name,phase=game.EnemyInitialPosePhaseV25(target),state=target.npcState,
                clip=target.clip,frame=Time.frameCount,sourceFrame=actual,expectedFrame=expected,elapsed=game.elapsed,sourceClock=clock,npcTime=target.npcTime,
                x=target.x,y=target.y,playerX=game.player.x,playerY=game.player.y,hp=target.hp,playerHp=game.player.hp,facing=target.facing,
                flipX=carrier.flipX,flipY=carrier.flipY,alpha=carrier.color.a,onFile=id+"-on.png",offFile=id+"-off.png",xMin=Width,xMax=-1,yMin=Height,yMax=-1};
            report.shots.Add(row);
            Need(expected==actual,"Source frame clock "+alias+" expected "+expected+" actual "+actual);
            if(!rt){rt=new RenderTexture(Width,Height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"Actual 1280x720 GPU target");}
            var oldTarget=cam.targetTexture;var oldRect=cam.rect;float oldAspect=cam.aspect;var oldActive=RenderTexture.active;
            string snapshot=JsonUtility.ToJson(target);Texture2D on=null,off=null;
            Camera.CameraCallback post=c=>{if(c==cam && cam.targetTexture==rt)row.posts++;};
            try {
                cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);cam.aspect=(float)Width/Height;
                witness.observed=cam;witness.target=rt;witness.draws=0;Camera.onPostRender+=post;
                on=Read(cam);row.draws=witness.draws;
                carrier.enabled=false;off=Read(cam);
                var a=on.GetPixels32();var b=off.GetPixels32();
                for(int i=0;i<a.Length;i++) {
                    if(Math.Abs(a[i].r-b[i].r)+Math.Abs(a[i].g-b[i].g)+Math.Abs(a[i].b-b[i].b)<=6)continue;
                    int x=i%Width,y=i/Width;row.pixels++;row.xMin=Math.Min(x,row.xMin);row.xMax=Math.Max(x,row.xMax);row.yMin=Math.Min(y,row.yMin);row.yMax=Math.Max(y,row.yMax);
                }
                File.WriteAllBytes(Path.Combine(output,row.onFile),on.EncodeToPNG());File.WriteAllBytes(Path.Combine(output,row.offFile),off.EncodeToPNG());
                Need(row.draws>0 && witness.draws==row.draws && row.posts==2,"Exact world camera and body draw pair "+id+" draws="+row.draws+" posts="+row.posts+" pixels="+row.pixels+" viewport="+cam.WorldToViewportPoint(carrier.bounds.center)+" bounds="+carrier.bounds);
                Need(row.pixels>0,"Visible Bat B contribution "+id);
                Need(game.elapsed==row.elapsed && JsonUtility.ToJson(target)==snapshot,"GPU observes same model frame");
            } finally {
                Camera.onPostRender-=post;witness.observed=null;carrier.enabled=true;cam.targetTexture=oldTarget;cam.rect=oldRect;cam.aspect=oldAspect;RenderTexture.active=oldActive;
                if(on)Destroy(on);if(off)Destroy(off);
            }
            return row;
        }
        Texture2D Read(Camera camera) {
            RenderTexture.active=rt; GL.Clear(true,true,Color.clear); camera.Render(); RenderTexture.active=rt;
            var result=new Texture2D(Width,Height,TextureFormat.RGBA32,false);
            result.ReadPixels(new Rect(0,0,Width,Height),0,0,false);result.Apply(false,false);return result;
        }
        IEnumerable WaitAlias(string wanted,float from,string id,int facing=0,Action place=null) {
            double until=Time.realtimeSinceStartupAsDouble+32;
            while(true) {
                string alias=Resolve(out float clock);
                var cam=(Camera)Field(scene,"worldCamera");
                var vp=carrier?cam.WorldToViewportPoint(carrier.bounds.center):Vector3.zero;
                report.last=id+" observed "+alias+" "+target.npcState+"/"+target.state+" clock="+clock+" actor="+target.x+","+target.y+" player="+game.player.x+","+game.player.y+" viewport="+vp;
                if(alias==wanted && clock>=from && (facing==0||target.facing==facing) && vp.z>0 && vp.x>.01f && vp.x<.99f && vp.y>.01f && vp.y<.99f){Capture(id,wanted);yield break;}
                Need(Time.realtimeSinceStartupAsDouble<until,"Timeout "+report.last);place?.Invoke();yield return null;Check();
            }
        }
        IEnumerable CanHit() {
            double until=Time.realtimeSinceStartupAsDouble+30;
            while(target.state!=CombatState.Normal || target.invincible>0 || game.EnemyHasNpcInvincibility(target) || game.EnemyHasSuperArmor(target)) {
                yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Wait vulnerable ordinary Bat");
            }
        }
        void Hit(HitReaction reaction,bool lethal) {
            float hp=target.hp;int amount=lethal?(int)Math.Ceiling(hp)+1:1;
            int applied=game.ApplyHitToEnemy(target,amount,reaction,280);
            report.actions.Add("Public ApplyHitToEnemy "+reaction+" "+amount+" -> "+applied);
            Need(applied>0 && target.hp<hp,"Production public hit");
        }
        void ClearCompetingActors() {
            foreach(var e in game.enemies.ToArray()) {
                if(ReferenceEquals(e,target)||e.encounterIndex!=target.encounterIndex||!e.active||e.dormant||e.isGate||e.passive||e.hp<=0)continue;
                int damage=game.ApplyHitToEnemy(e,100000,HitReaction.None);
                Need(damage>0 && e.hp<=0,"Public damage removes competing "+e.sourceNPC);
                report.actions.Add("Isolated first Bat B using public damage on "+e.sourceNPC);
            }
        }
        IEnumerable Combat() {
            Place(target.x-32,false);
            foreach(var step in WaitAlias("source-attack-a-ready",.04f,"07-natural-ready"))yield return step;
            Need(target.npcState=="BAT_B_ATTACK_A_READY","Source ready");report.ready=true;
            // Allow the native bite to become visible before the original near-contact window transitions to drain.
            int approach=target.facing;
            Place(target.x+approach*153,false);
            foreach(var step in WaitAlias("source-attack-a",.05f,"08-natural-bite"))yield return step;
            Need(target.npcState=="BAT_B_ATTACK_A","Source attack");
            report.playerHpBefore=game.player.hp;game.player.invincible=0;
            int eventStart=game.npcActionEvents.Count;
            double until=Time.realtimeSinceStartupAsDouble+32;
            bool captured=false;
            while(true) {
                string alias=Resolve(out float clock);
                report.last="Natural drain "+target.npcState+" / "+alias+" t="+target.npcTime+" batHP="+target.hp+" playerHP="+game.player.hp;
                Need(Time.realtimeSinceStartupAsDouble<until,"Timeout "+report.last);
                if(alias=="source-attack-a")Place(target.x+target.facing*32,false);
                if(alias=="source-attack-a-drain" && !captured) {Capture("09-drain-start",alias);captured=true;}
                int bite=0,drain=0;
                for(int i=eventStart;i<game.npcActionEvents.Count;i++) {
                    var e=game.npcActionEvents[i];if(e.type!="hit"||e.damage<=0||e.npc!="NUI_BAT_B")continue;
                    if(e.state=="BAT_B_ATTACK_A")bite+=e.damage;
                    if(e.state=="BAT_B_ATTACK_A_DRAIN")drain+=e.damage;
                }
                if(captured && drain>0) {
                    Need(bite>0 && target.hp>report.damagedHp && Mathf.Abs(target.hp-Mathf.Min(target.maxHp,report.damagedHp+drain))<.01f,"Source damage and drain heal");
                    Need(report.playerHpBefore-game.player.hp==bite+drain,"Player HP loss matches isolated original bite and drain");
                    report.bite=report.drain=true;report.contactDamage=bite;report.drainDamage=drain;
                    report.healedHp=target.hp;report.playerHpAfter=game.player.hp;
                    for(int i=eventStart;i<game.npcActionEvents.Count;i++) {
                        var e=game.npcActionEvents[i];if(e.npc=="NUI_BAT_B" && e.type=="hit")report.contacts.Add(e);
                    }
                    Capture("10-drain-heal",alias);break;
                }
                if(captured && alias!="source-attack-a-drain")throw new InvalidOperationException("Drain ended before positive source heal");
                yield return null;Check();
            }
            foreach(var step in WaitAlias("source-attack-a-drain",2.2f,"11-drain-loop"))yield return step;
            // followHitTarget uses the player's real location until the native five-second state ends.
            game.player.invincible=120;
            until=Time.realtimeSinceStartupAsDouble+24;
            while(target.npcState=="BAT_B_ATTACK_A_DRAIN"){
                yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Native drain recovery");
            }
            Place(target.x-152);
        }
        IEnumerable ReturnMenu() {
            Press("dungeons");for(int i=0;i<5;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused,"Return to ordinary menu");
            Need(((IDictionary)Field(scene,"batBDisplayedV41")).Count==0 && ((IDictionary)Field(scene,"batBPaintV41")).Count==0,"Painted carrier and sprite cache released on return");
            report.returnedToMenu=true;
        }
        IEnumerator Flow() {
            while(!(scene=FindFirstObjectByType<DemoScene>())){Check();yield return null;}
            for(int i=0;i<3;i++)yield return null;
            game=(GameModel)Field(scene,"game");
            Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused,"Ordinary main menu start");
            foreach(var step in EnterCave())yield return step;
            oldTrace=game.traceNpcActions;game.traceNpcActions=true;traceSet=true;
            game.player.invincible=120;
            Place(target.x-155);
            foreach(var step in WaitAlias("source-start",.04f,"01-hanging"))yield return step;
            Need(game.EnemyHangingV25(target)&&game.EnemyHangingSupportV25(target)==2&&Mathf.Abs(target.y-game.Surface(2).Y(target.x))<.05f,"Original hanging support 2");
            report.hanging=true;ClearCompetingActors();Place(target.x-15);
            foreach(var step in WaitAlias("source-wait-zero2",.04f,"02-release-start"))yield return step;
            foreach(var step in WaitAlias("source-wait-zero2",.8f,"03-release-open"))yield return step;
            Place(target.x-185);
            foreach(var step in WaitAlias("source-flight",0,"04-hover"))yield return step;
            Need(game.EnemyInitialPosePhaseV25(target)=="complete","Source release complete");report.release=report.hover=true;
            Place(target.x-151);
            foreach(var step in WaitAlias("source-walk",.03f,"05-fly-left",-1,()=>Place(target.x-151)))yield return step;
            bool leftFlip=carrier.flipX;
            Place(target.x+151);
            foreach(var step in WaitAlias("source-walk",.03f,"06-fly-right",1,()=>Place(target.x+151)))yield return step;
            Need(leftFlip!=carrier.flipX,"Whole painted carrier mirrors the two natural facings");report.left=report.right=true;
            float hp=target.hp;
            int actual=game.ApplyHitToEnemy(target,40,HitReaction.None);
            Need(actual>0 && target.hp>0 && hp-target.hp==actual,"Public nonlethal wound opens measurable drain headroom");
            report.damagedHp=target.hp;
            foreach(var step in Combat())yield return step;
            foreach(var step in CanHit())yield return step;
            Hit(HitReaction.Hurt,false);
            foreach(var step in WaitAlias("source-hurt",.04f,"12-hurt"))yield return step;
            report.publicHurt=true;
            foreach(var step in CanHit())yield return step;
            Hit(HitReaction.Launch,false);
            foreach(var step in WaitAlias("source-launch",0,"13-launch"))yield return step;
            foreach(var step in WaitAlias("source-fall",0,"14-fall"))yield return step;
            foreach(var step in WaitAlias("source-down",0,"15-down"))yield return step;
            foreach(var step in WaitAlias("source-get-up",.03f,"16-rise"))yield return step;
            report.publicLaunch=true;
            foreach(var step in CanHit())yield return step;
            Hit(HitReaction.Hurt,true);
            foreach(var step in WaitAlias("source-death",.08f,"17-death-early"))yield return step;
            float early=carrier.color.a;
            foreach(var step in WaitAlias("source-death",1.35f,"18-death-fade"))yield return step;
            Need(early>carrier.color.a && carrier.color.a>0 && Mathf.Abs(carrier.color.a-(1-target.deathTime/1.2f))<.003f,"Original 1.2s fade");report.deathFade=true;
            var deceased=target;
            foreach(var step in ReturnMenu())yield return step;
            foreach(var step in EnterCave())yield return step;
            Need(!ReferenceEquals(target,deceased),"Fresh ordinary Bat B on re-entry");
            Place(target.x-155);
            foreach(var step in WaitAlias("source-start",.04f,"19-reentered"))yield return step;
            report.reentered=true;
            foreach(var step in ReturnMenu())yield return step;
        }
    }
    public sealed class BatBDrawWitnessV41 : MonoBehaviour {
        internal Camera observed;
        internal RenderTexture target;
        internal int draws;
        void OnWillRenderObject() {if(observed && Camera.current==observed && observed.targetTexture==target)draws++;}
    }
}
#endif
