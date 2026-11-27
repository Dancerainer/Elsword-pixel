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
    public sealed class BatPlayerCheckV34 : MonoBehaviour {
        const string Flag="--root-bat-a-check-v34", OutFlag="--root-bat-a-output-v34";
        const string Effect="DamageEffect_BAT_A_ATTACK_A";
        const int Width=1280,Height=720;
        const int Budget=300;
        const BindingFlags Fields=BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance;
        [Serializable] public sealed class Shot {
            public string label,alias,state,phase,clip,onFile,offFile;
            public int frame,pixels,width,height,renderers,sourceFrame;
            public float elapsed,clock,npcTime,sourceClock,hp,alpha,x,y,supportY,deathTime,labelGap;
        }
        [Serializable] public sealed class Contact {
            public int serial,damage,facing;
            public float stateStart,emitted,hit,relativeEmit,relativeHit,playerHpAtReady,playerHpAfter;
            public bool sourceOwner,spawned,positiveDamage;
        }
        [Serializable] public sealed class Report {
            public string error="",last="",graphics,room;
            public bool passed,batchMode,ordinaryEntry,originalBirth,hanging,release,naturalReady,naturalAttack,
                publicHurt,publicLaunch,deathFade,returnedToMenu,cleaned,reentered,controlledPlayerPlacement=true;
            public int sourceSpawnIndex,encounter,actorIndex;
            public List<string> actions=new List<string>(),errors=new List<string>();
            public List<Shot> shots=new List<Shot>();
            public Contact contact=new Contact();
        }
        Report report=new Report();
        DemoScene scene;
        GameModel game;
        Enemy target;
        SpriteRenderer carrier;
        BatAuthoredPartsV34 owner;
        string output;
        double started;
        bool done,oldBackground,oldTrace,traceSet;
        RenderTexture rt;
        static void Need(bool ok,string message) { if (!ok) throw new InvalidOperationException("Bat Player V34 "+message); }
        static object Field(object o,string name) {
            var f=o.GetType().GetField(name,Fields); Need(f!=null,"Missing field "+name); return f.GetValue(o);
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            var args=Environment.GetCommandLineArgs(); int flag=Array.IndexOf(args,Flag);
            if (flag<0) return;
            try {
                int at=Array.IndexOf(args,OutFlag);
                Need(Array.LastIndexOf(args,Flag)==flag && at>=0 && at==Array.LastIndexOf(args,OutFlag) && at+1<args.Length,"One explicit check/output pair");
                Need(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11 && !Application.isEditor,"Standalone D3D11 required");
                foreach (var a in args) Need(a!="--root-a1-chicken-check-v32" && a!="--root-core-reaction-check-v32" && a!="--root-factory-sheriff-check-v32","Run one driving fixture at a time");
                string path=Path.GetFullPath(args[at+1]);
                string allowed=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allowed,StringComparison.OrdinalIgnoreCase) && !Directory.Exists(path) && !File.Exists(path),"Fresh runtime-drafts output required");
                Directory.CreateDirectory(path);
                new GameObject("Explicit Bat gameplay check V34").AddComponent<BatPlayerCheckV34>().output=path;
            } catch (Exception e) { Debug.LogException(e); Application.Quit(2); }
        }
        void Log(string msg,string stack,LogType t) {
            if (t==LogType.Error || t==LogType.Exception || t==LogType.Assert || msg.Contains("BAT_AUTHORED_FALLBACK") || msg.Contains("BAT_AUTHORED_BANK"))
                report.errors.Add(msg+"\n"+stack);
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
            if (traceSet && game!=null) game.traceNpcEffects=oldTrace;
            Application.runInBackground=oldBackground;
            try { File.WriteAllText(Path.Combine(output,"bat-player-report.json"),JsonUtility.ToJson(report,true)); }
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
            Need(stage!=null && stage.spawns[0].sourceSpawnIndex==6 && stage.spawns[0].sourceNPC=="NUI_BAT_A" && game.enemies.Count>0,"Original spawn index6");
            target=game.enemies[0];
            Need(target.kind=="bat-a" && target.sourceNPC=="NUI_BAT_A" && target.encounterIndex==0,"Original first Bat birth");
            report.room=game.CurrentRoom.id; report.actorIndex=0; report.sourceSpawnIndex=6; report.encounter=0;
            report.ordinaryEntry=report.originalBirth=true;
        }
        void PlacePlayer(float x) {
            var floor=game.SurfaceBelow(x,target.y+.2f,true); Need(floor!=null,"Original floor under player placement");
            game.player.x=x; game.player.y=floor.Y(x); game.player.groundLine=floor.id;
            game.player.vx=0;game.player.vy=0;
            report.actions.Add("Controlled player on original line "+floor.id+" at x "+x);
        }
        string Resolve(out float clock) {
            clock=0; owner=null; carrier=null;
            var views=(IDictionary)Field(scene,"batViewsV34");
            if (target==null || !views.Contains(target)) return "";
            owner=(BatAuthoredPartsV34)Field(views[target],"parts"); carrier=(SpriteRenderer)Field(views[target],"carrier");
            if (!carrier || !carrier.sprite || owner==null) return "";
            string name=carrier.sprite.name; int slash=name.LastIndexOf('/');
            if (!name.StartsWith("bat-a/",StringComparison.Ordinal) || slash<=6) return "";
            string alias=name.Substring(6,slash-6);
            if (alias=="source-start" || alias=="source-wait-zero2") clock=game.EnemyInitialPoseClipClockV25(target);
            else clock=(float)scene.GetType().GetMethod("First3ActualRenderClockV25",Fields).Invoke(scene,new object[]{target});
            return alias;
        }
        Shot Capture(string id,string expect) {
            string alias=Resolve(out float sourceClock);
            Need(alias==expect && owner!=null && carrier.forceRenderingOff,"Actual original carrier selected "+expect+" got "+alias);
            var renderers=new List<SpriteRenderer>(); owner.Visit(r=>renderers.Add(r));
            Need(renderers.Count==1 || renderers.Count==3,"Whole pose or three authored controls");
            var camera=(Camera)Field(scene,"worldCamera"); Need(camera,"Original world camera");
            if (!rt) {rt=new RenderTexture(Width,Height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"GPU readback target");}
            var oldTarget=camera.targetTexture; var oldActive=RenderTexture.active; var oldRect=camera.rect; float oldAspect=camera.aspect;
            var enabled=new bool[renderers.Count]; for(int i=0;i<enabled.Length;i++) enabled[i]=renderers[i].enabled;
            var row=new Shot{label=id,alias=alias,state=target.npcState,phase=game.EnemyInitialPosePhaseV25(target),clip=target.clip,frame=Time.frameCount,
                renderers=renderers.Count,elapsed=game.elapsed,clock=target.clock,npcTime=target.npcTime,sourceClock=sourceClock,hp=target.hp,alpha=carrier.color.a,
                x=target.x,y=target.y,supportY=game.EnemyInitialSupportYV25(target),deathTime=target.deathTime,onFile=id+"-on.png",offFile=id+"-off.png"};
            int.TryParse(carrier.sprite.name.Substring(carrier.sprite.name.LastIndexOf('/')+1),out row.sourceFrame);
            string before=JsonUtility.ToJson(target);
            Texture2D on=null,off=null;
            try {
                camera.targetTexture=rt;camera.rect=new Rect(0,0,1,1);camera.aspect=(float)Width/Height;
                on=Read(camera);
                for(int i=0;i<enabled.Length;i++) renderers[i].enabled=false;
                off=Read(camera);
                File.WriteAllBytes(Path.Combine(output,row.onFile),on.EncodeToPNG());
                File.WriteAllBytes(Path.Combine(output,row.offFile),off.EncodeToPNG());
                var a=on.GetPixels32();var b=off.GetPixels32();int left=Width,right=-1,bottom=Height,top=-1;
                for(int i=0;i<a.Length;i++) {
                    int diff=Math.Abs(a[i].r-b[i].r)+Math.Abs(a[i].g-b[i].g)+Math.Abs(a[i].b-b[i].b);
                    if(diff<=6) continue;
                    int x=i%Width,y=i/Width;row.pixels++;left=Math.Min(left,x);right=Math.Max(right,x);bottom=Math.Min(bottom,y);top=Math.Max(top,y);
                }
                row.width=right>=0?right-left+1:0;row.height=top>=0?top-bottom+1:0;
                Need(row.pixels>0,"Visible GPU contribution "+id);
            } finally {
                for(int i=0;i<renderers.Count;i++)if(renderers[i])renderers[i].enabled=enabled[i];
                camera.targetTexture=oldTarget;camera.rect=oldRect;camera.aspect=oldAspect;RenderTexture.active=oldActive;
                if(on) Destroy(on);if(off) Destroy(off);
                report.shots.Add(row);
            }
            Need(game.elapsed==row.elapsed && JsonUtility.ToJson(target)==before,"GPU readback preserves actor and model time");
            foreach(var r in renderers) Need(Math.Abs(r.color.a-row.alpha)<.0001f,"Whole/parts follow carrier alpha");
            var bars=(IList)Field(scene,"enemyBars");var bar=(SpriteRenderer)bars[0];
            if(target.hp>0 && bar.enabled && owner.TryTop(out float topY)) {
                row.labelGap=bar.transform.position.y-topY;Need(Math.Abs(row.labelGap-8)<.05f,"Visible top label");
            }
            return row;
        }
        Texture2D Read(Camera camera) {
            RenderTexture.active=rt; GL.Clear(true,true,Color.clear); camera.Render(); RenderTexture.active=rt;
            var result=new Texture2D(Width,Height,TextureFormat.RGBA32,false);
            result.ReadPixels(new Rect(0,0,Width,Height),0,0,false);result.Apply(false,false);return result;
        }
        IEnumerable WaitAlias(string alias,float minTime,string shot,double limit=24) {
            double until=Time.realtimeSinceStartupAsDouble+limit;
            while(true) {
                yield return null; Check();
                string actual=Resolve(out float clock);
                report.last=alias+" pending: actual="+actual+" root="+target.x+","+target.y+" phase="+game.EnemyInitialPosePhaseV25(target)+" state="+target.npcState+" sourceClock="+clock+" elapsed="+game.elapsed;
                Need(Time.realtimeSinceStartupAsDouble<until,"Wait "+report.last);
                if(actual==alias && clock>=minTime) {if(shot!=null)Capture(shot,alias);yield break;}
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
        IEnumerable NaturalAttack() {
            var contact=report.contact;
            foreach(var step in WaitAlias("source-attack-a-ready",.02f,"04-ai-ready"))yield return step;
            Need(target.npcState=="BAT_A_ATTACK_A_READY","Natural ready from original AI");
            report.naturalReady=true;
            PlacePlayer(target.x+target.facing*36);
            game.player.invincible=0;
            contact.playerHpAtReady=game.player.hp;
            double until=Time.realtimeSinceStartupAsDouble+30;
            bool entered=false,captured=false;
            while(!contact.positiveDamage) {
                yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Natural contact event; last "+report.last);
                string alias=Resolve(out float clock);
                report.last="attack "+target.npcState+" alias="+alias+" t="+target.npcTime+" serial="+contact.serial+" playerHp="+game.player.hp;
                if(target.npcState=="BAT_A_ATTACK_A") {
                    if(!entered)contact.stateStart=game.elapsed-target.npcTime;
                    entered=true;
                    if(alias=="source-attack-a" && !captured) {Capture("05-natural-impact",alias);captured=true;}
                }
                foreach(var e in game.npcPresentationEvents)
                    if(e.type=="spawn" && ReferenceEquals(e.owner,target) && e.effect==Effect) {
                        contact.serial=e.serial;contact.emitted=e.time;contact.facing=e.facing;contact.sourceOwner=true;contact.spawned=true;
                        contact.relativeEmit=e.time-contact.stateStart;
                    }
                if(contact.spawned) {
                    foreach(var e in game.npcEffectEvents)if(e.serial==contact.serial && e.type=="hit-player" && e.damage>0) {
                        contact.damage=e.damage;contact.hit=e.time;contact.relativeHit=e.time-contact.emitted;contact.playerHpAfter=game.player.hp;
                        contact.positiveDamage=contact.playerHpAfter<contact.playerHpAtReady;
                    }
                }
                if(entered && target.npcState!="BAT_A_ATTACK_A" && !contact.positiveDamage)
                    throw new InvalidOperationException("Original Bat attack ended before confirmed player damage: "+report.last);
            }
            Need(entered && captured && contact.spawned && contact.sourceOwner,"Original visible charge and source emission");
            Need(Math.Abs(contact.relativeEmit-.02f)<.038f && contact.relativeHit>=-.002f && contact.relativeHit<=.205f,"Original .02 emission/.2 lifetime contact");
            report.naturalAttack=true;
        }
        IEnumerable ReturnMenu() {
            var old=new List<SpriteRenderer>();
            if(owner!=null)owner.Visit(r=>old.Add(r));
            Press("dungeons");for(int i=0;i<5;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons" && game.paused,"Return to ordinary Main");
            Need(((IDictionary)Field(scene,"batViewsV34")).Count==0,"Bat owners released on Main return");
            foreach(var r in old)Need(!r,"Old Bat renderers destroyed");
            report.returnedToMenu=report.cleaned=true;
        }
        IEnumerator Flow() {
            while(!(scene=FindFirstObjectByType<DemoScene>())){Check();yield return null;}
            for(int i=0;i<3;i++)yield return null;
            game=(GameModel)Field(scene,"game");
            Need((string)Field(scene,"openMenu")=="dungeons" && game.paused,"Main menu start");
            foreach(var step in EnterCave())yield return step;
            oldTrace=game.traceNpcEffects;game.traceNpcEffects=true;traceSet=true;
            game.player.maxHp=game.player.hp=5000; game.player.invincible=120;
            PlacePlayer(target.x-155);
            foreach(var step in WaitAlias("source-start",0,"01-hanging"))yield return step;
            Need(game.EnemyHangingV25(target) && game.EnemyHangingSupportV25(target)==2 && Mathf.Abs(target.y-game.Surface(2).Y(target.x))<.05f,"Original ceiling anchor");
            report.hanging=true;
            PlacePlayer(target.x-15);
            foreach(var step in WaitAlias("source-wait-zero2",.04f,"02-release-closed"))yield return step;
            foreach(var step in WaitAlias("source-wait-zero2",.62f,"03-release-turn"))yield return step;
            foreach(var step in WaitAlias("source-wait-zero2",1.2f,"03b-release-flight"))yield return step;
            double until=Time.realtimeSinceStartupAsDouble+12;
            while(game.EnemyInitialPosePhaseV25(target)!="complete"){yield return null;Need(Time.realtimeSinceStartupAsDouble<until,"Release endpoint");}
            report.release=true;
            foreach(var step in NaturalAttack())yield return step;
            game.player.invincible=120;
            foreach(var step in WaitAlias("source-get-up-back",.2f,"06-attack-recovery"))yield return step;
            foreach(var step in CanHit())yield return step;
            Hit(HitReaction.Hurt,false);
            foreach(var step in WaitAlias("source-hurt",.04f,"07-hurt"))yield return step;
            report.publicHurt=true;
            foreach(var step in CanHit())yield return step;
            Hit(HitReaction.Launch,false);
            foreach(var step in WaitAlias("source-launch",0,"08-launch"))yield return step;
            foreach(var step in WaitAlias("source-fall",0,"09-fall"))yield return step;
            foreach(var step in WaitAlias("source-down",0,"10-down"))yield return step;
            foreach(var step in WaitAlias("source-get-up",.03f,"11-rise"))yield return step;
            report.publicLaunch=true;
            foreach(var step in CanHit())yield return step;
            Hit(HitReaction.Hurt,true);
            foreach(var step in WaitAlias("source-death",.08f,"12-death-early"))yield return step;
            float early=carrier.color.a;
            foreach(var step in WaitAlias("source-death",1.35f,"13-death-middle"))yield return step;
            float middle=carrier.color.a;
            Need(early>middle && middle>0 && Mathf.Abs(middle-(1-target.deathTime/1.2f))<.003f,"Original 1.2s fade");
            report.deathFade=true;
            var deceased=target;
            foreach(var step in ReturnMenu())yield return step;
            foreach(var step in EnterCave())yield return step;
            Need(!ReferenceEquals(target,deceased),"Fresh original spawn after re-entry");
            PlacePlayer(target.x-155);
            foreach(var step in WaitAlias("source-start",0,"14-reentered"))yield return step;
            report.reentered=true;
            foreach(var step in ReturnMenu())yield return step;
        }
    }
}
#endif
