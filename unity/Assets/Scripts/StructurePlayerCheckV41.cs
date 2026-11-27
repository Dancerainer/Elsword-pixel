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
    // Separate ordinary-menu fixture. Each mode owns one original birth, never the simulation clock.
    public sealed partial class StructurePlayerCheckV41 : MonoBehaviour {
        const string Flag="--root-structure-check-v41",OutFlag="--root-structure-output-v41";
        const int Width=1280,Height=720,Budget=300;
        const BindingFlags Fields=BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance;
        sealed class Spec {
            internal string region,route,roomId,npc,kind,page;
            internal int room,encounter,index,sourceIndex;
            internal Spec(string region,string route,string roomId,int room,int encounter,int index,int sourceIndex,string npc,string kind,string page) {
                this.region=region;this.route=route;this.roomId=roomId;this.room=room;this.encounter=encounter;this.index=index;this.sourceIndex=sourceIndex;
                this.npc=npc;this.kind=kind;this.page=page;
            }
        }
        static partial void AdditionalStructureModeV41(string mode,ref Spec selection);
        partial void AdditionalStructureStepsV41(ref IEnumerable steps);
        static Spec Find(string mode) {
            switch(mode) {
            case "press":return new Spec("altera","altera-nasod-factory","Dungeon_Altera_Nasod_Factory_Normal-1",1,0,2,24,"NUI_FACTORY_PRESS","factory-press","press");
            case "press3":return new Spec("altera","altera-nasod-factory","Dungeon_Altera_Nasod_Factory_Normal-0",0,1,12,42,"NUI_FACTORY_PRESS3","factory-press3","press");
            case "bunker":return new Spec("elder","wally-castle-center","castle-center-1",0,0,1,33,"NUI_BUNKER","bunker","castle-bunker");
            default: Spec value=null;AdditionalStructureModeV41(mode,ref value);return value;
            }
        }
        [Serializable] sealed class Shot {
            public string id,sprite,texture,state,clip,onFile,offFile,sourceKey,combat;
            public Rect atlasRect;
            public int frame,sourceFrame,facing,pixels,xMin,xMax,yMin,yMax,draws,posts,uid,epoch;
            public float elapsed,sourceClock,npcTime,x,y,playerX,playerY,hp,alpha;
            public bool flipX,canPass;
        }
        [Serializable] sealed class Report {
            public string mode,route,npc,room,page,error="",last="",graphics;
            public bool passed,batchMode,ordinaryEntry,originalBirth,changedArt,returnedToMenu,offscreenStandaloneGPU=true,windowPresentationVerified=false;
            public int index,sourceSpawnIndex,encounter,damage;
            public List<string> actions=new List<string>(),errors=new List<string>();
            public List<Shot> shots=new List<Shot>();
        }
        Report report=new Report();
        DemoScene scene;GameModel game;Enemy target;SpriteRenderer carrier;BatBDrawWitnessV41 witness;
        Spec spec;string output;double started;bool done,oldBackground;
        RenderTexture rt;
        static void Need(bool ok,string reason){if(!ok)throw new InvalidOperationException("Structure Player V41 "+reason);}
        static object Field(object o,string name){var f=o.GetType().GetField(name,Fields);Need(f!=null,name);return f.GetValue(o);}
        static void Call(object o,string name,params object[] args){var m=o.GetType().GetMethod(name,Fields);Need(m!=null,name);m.Invoke(o,args);}
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            var args=Environment.GetCommandLineArgs();
            int flag=Array.IndexOf(args,Flag);if(flag<0)return;
            try {
                int at=Array.IndexOf(args,OutFlag);Need(flag+1<args.Length && Find(args[flag+1])!=null,"Named structure mode");
                Need(Array.IndexOf(args,Flag)==Array.LastIndexOf(args,Flag) && at>=0 && at==Array.LastIndexOf(args,OutFlag) && at+1<args.Length,"One explicit check/output pair");
                Need(Array.FindAll(args,a=>a.StartsWith("--root-",StringComparison.Ordinal)&&a.Contains("-check-v")).Length==1,"Separate driving checks");
                Need(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11&&!Application.isEditor,"Standalone D3D11 required");
                string path=Path.GetFullPath(args[at+1]);
                string allowed=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allowed,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh runtime-drafts output");
                Directory.CreateDirectory(path);
                var owner=new GameObject("Explicit Structure Player V41").AddComponent<StructurePlayerCheckV41>();owner.output=path;owner.report.mode=args[flag+1];owner.spec=Find(owner.report.mode);
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
            Application.runInBackground=oldBackground;
            try { File.WriteAllText(Path.Combine(output,"structure-player-report.json"),JsonUtility.ToJson(report,true)); }
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
        IEnumerable Enter(string region,string route) {
            Press("region:"+region);yield return null;
            var routes=(IList)Field(scene,"mapDungeons");var visible=(IList)Field(scene,"mapVisible");int index=-1,card=-1;
            for(int i=0;i<routes.Count;++i)if((string)Field(routes[i],"id")==route)index=i;
            for(int i=0;i<visible.Count;++i)if((int)visible[i]==index)card=i;
            Need(index>=0&&card>=0,"Visible route "+route);
            int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();
            for(int i=0;(int)Field(scene,"mapPage")!=card/size;++i){Need(i<20,"Pages");Press("dungeon-page:"+((int)Field(scene,"mapPage")<card/size?"1":"-1"));yield return null;}
            Press("dungeon-card:"+(card%size));yield return null;Need((int)Field(scene,"mapSelected")==index,"Selected route");
            Press("dungeon-enter");yield return null;
            if(route=="altera-nasod-factory")while(!scene.FactoryProductSessionActiveR1179||scene.FactoryProductSessionLoadingR1179){Check();yield return null;}
            if(route=="altera-plain"||route=="altera-plain-recycle")while(!scene.PlainProductSessionActiveR1320||scene.PlainProductSessionLoadingR1320){Check();yield return null;}
            if(route=="bethma-richie-mine")while(!scene.Mine2ProductSessionActiveR1233||scene.Mine2ProductSessionLoadingR1233){Check();yield return null;}
            game=(GameModel)Field(scene,"game");Need(!game.paused && game.room==0 && (string)Field(scene,"openMenu")=="","Ordinary room entry");
            report.ordinaryEntry=true;report.route=route;
            if(game.room!=spec.room)Call(game,"EnterRoom",spec.room);
            if(game.EncounterIndex!=spec.encounter)Call(game,"BeginEncounter",spec.encounter);
            if(spec.room!=0||spec.encounter!=0)Call(scene,"RebuildRoom");
            Need(game.room==spec.room && game.EncounterIndex==spec.encounter && game.CurrentRoom.id==spec.roomId,"Original room and encounter");
            target=game.enemies[spec.index];
            Need(target.sourceNPC==spec.npc && target.kind==spec.kind && target.encounterIndex==spec.encounter && target.active&&!target.dormant,"Original structure birth in room table");
            var stage=route=="altera-nasod-factory"?((StageArt[])Field(scene,"factoryProductStagesR1179"))[game.room]:
                Array.Find(((ArtCatalog)Field(scene,"catalog")).stages,s=>s.id==spec.roomId);
            if(route=="altera-nasod-factory") {
                var source=Array.Find((FactoryPressBirthV569[])Field(game,"factoryPressBirthsV569"),b=>b.room==spec.room && b.order==spec.index);
                Need(source!=null && source.sourceSpawnIndex==spec.sourceIndex && source.npc==spec.npc && source.encounter==spec.encounter,"Factory original Press birth binding");
            } else if(stage!=null)Need(stage.spawns[spec.index].sourceNPC==spec.npc && stage.spawns[spec.index].sourceSpawnIndex==spec.sourceIndex,"Source stage spawn index");
            report.npc=target.sourceNPC;report.room=game.CurrentRoom.id;report.page=spec.page;report.index=spec.index;report.encounter=spec.encounter;report.sourceSpawnIndex=spec.sourceIndex;report.originalBirth=true;
            report.actions.Add("Controlled original room "+spec.room+"/encounter "+spec.encounter+"; sibling enemies remain live");
            Protect();for(int i=0;i<5;i++){yield return null;Check();}
        }
        void Protect() {
            float x=target.x-34;var floor=game.SurfaceBelow(x,target.y+.2f,true);
            if(floor==null)floor=game.Surface(target.groundLine);
            Need(floor!=null,"Original player support near "+target.sourceNPC);
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;game.player.invincible=60;
        }
        SpriteRenderer Current() {
            int i=game.enemies.IndexOf(target);Need(i==spec.index,"Stable original owner index");
            return (SpriteRenderer)((IList)Field(scene,"enemyViews"))[i];
        }
        Shot Capture(string id,FactoryPressFrameV569 press=null) {
            carrier=Current();
            Need(carrier && carrier.sprite && carrier.enabled && !carrier.forceRenderingOff && carrier.gameObject.activeInHierarchy,"Actual visible single structure carrier "+id);
            Need(carrier.sprite.texture.name==spec.page && carrier.sprite.texture.filterMode==FilterMode.Point,"Actual Point page "+spec.page+" got "+carrier.sprite.texture.name);
            if(!witness||witness.gameObject!=carrier.gameObject)witness=carrier.gameObject.AddComponent<BatBDrawWitnessV41>();
            var cam=(Camera)Field(scene,"worldCamera");Need(cam,"Original world camera");
            var row=new Shot{id=id,sprite=carrier.sprite.name,texture=carrier.sprite.texture.name,atlasRect=carrier.sprite.rect,state=target.npcState,
                clip=target.clip,combat=target.state.ToString(),frame=Time.frameCount,elapsed=game.elapsed,sourceClock=target.clock,npcTime=target.npcTime,
                x=target.x,y=target.y,playerX=game.player.x,playerY=game.player.y,hp=target.hp,facing=target.facing,flipX=carrier.flipX,alpha=carrier.color.a,
                onFile=id+"-on.png",offFile=id+"-off.png",xMin=Width,xMax=-1,yMin=Height,yMax=-1};
            if(press!=null) {
                Need(ReferenceEquals(press.actualOwner,target) && press.state==target.npcState && press.npc==spec.npc,"Same production Press frame");
                var lookup=((Dictionary<string,SourcePhaseWholeLookupV21>)Field(scene,"factoryProductReferenceR1179"))[spec.npc];
                var native=lookup.SelectOriginalReferenceSourceClockR718(press.state,target.npcTime,press.sourcePoseTime);
                row.sourceKey=native.Whole.Key;row.sourceClock=press.sourcePoseTime;row.uid=press.uid;row.epoch=press.epoch;row.canPass=press.canPassSourceRequest;
                Need(row.sprite==spec.npc+"/"+row.sourceKey,"Painted Press matches actual source key "+row.sourceKey);
            } else {
                int last=row.sprite.LastIndexOf('/');if(last>=0)int.TryParse(row.sprite.Substring(last+1),out row.sourceFrame);
                if(spec.kind=="bunker")Need(row.sprite.StartsWith("bunker/"+row.clip+"/",StringComparison.Ordinal),"Castle original clip "+row.clip+" got "+row.sprite);
            }
            report.shots.Add(row);
            if(!rt){rt=new RenderTexture(Width,Height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"Actual 1280x720 GPU target");}
            var oldTarget=cam.targetTexture;var oldRect=cam.rect;float oldAspect=cam.aspect;var oldActive=RenderTexture.active;
            string snapshot=JsonUtility.ToJson(target);Texture2D on=null,off=null;
            Camera.CameraCallback post=c=>{if(c==cam && cam.targetTexture==rt)row.posts++;};
            try {
                cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);cam.aspect=(float)Width/Height;
                witness.observed=cam;witness.target=rt;witness.draws=0;Camera.onPostRender+=post;
                on=Read(cam);row.draws=witness.draws;
                carrier.enabled=false;off=Read(cam);
                Need(row.draws>0 && witness.draws==row.draws && row.posts==2,"Exact world camera and body draw pair "+id);
                var a=on.GetPixels32();var b=off.GetPixels32();
                for(int i=0;i<a.Length;i++) {
                    if(Math.Abs(a[i].r-b[i].r)+Math.Abs(a[i].g-b[i].g)+Math.Abs(a[i].b-b[i].b)<=6)continue;
                    int x=i%Width,y=i/Width;row.pixels++;row.xMin=Math.Min(x,row.xMin);row.xMax=Math.Max(x,row.xMax);row.yMin=Math.Min(y,row.yMin);row.yMax=Math.Max(y,row.yMax);
                }
                File.WriteAllBytes(Path.Combine(output,row.onFile),on.EncodeToPNG());File.WriteAllBytes(Path.Combine(output,row.offFile),off.EncodeToPNG());
                Need(row.pixels>0,"Visible structure contribution "+id);
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
        IEnumerable Until(Func<bool> ready,string reason,double budget=35) {
            double until=Time.realtimeSinceStartupAsDouble+budget;
            while(!ready()) {
                report.last=reason+" seen "+target.npcState+"/"+target.clip+" t="+target.npcTime+" elapsed="+game.elapsed;
                Need(Time.realtimeSinceStartupAsDouble<until,"Timeout "+report.last);Protect();yield return null;Check();
            }
        }
        FactoryPressFrameV569 Frame() {
            return Array.Find(game.ReadFactoryPressPresentationFramesR1470(),f=>ReferenceEquals(f.actualOwner,target));
        }
        IEnumerable PressCycle() {
            string prefix=spec.npc=="NUI_FACTORY_PRESS"?"FACTORY_PRESSMACHINE_":"FACTORY_PRESSMACHINE3_";
            foreach(var s in Until(()=>Frame()!=null && Frame().state==prefix+"START","Original Press START"))yield return s;
            var start=Capture("01-start",Frame());
            // Wait for the original timer/cooldown even when a source collision request blocks player input.
            foreach(var s in Until(()=>Frame()!=null && Frame().state==prefix+"ATTACK" && Frame().sourcePoseTime>.12f,"Original Press ATTACK"))yield return s;
            var first=Capture("02-attack-open",Frame());
            foreach(var s in Until(()=>Frame()!=null && Frame().state==prefix+"ATTACK" && Frame().sourcePoseTime>=2.08f,"Press strike window"))yield return s;
            var strike=Capture("03-attack-strike",Frame());
            foreach(var s in Until(()=>Frame()!=null && Frame().state==prefix+"ATTACK" && Frame().sourcePoseTime>=2.42f,"Source passability close"))yield return s;
            var closed=Capture("04-blocked",Frame());Need(!closed.canPass,"Actual close request after 2.3s");
            foreach(var s in Until(()=>Frame()!=null && Frame().state==prefix+"ATTACK" && Frame().sourcePoseTime>=4.26f,"Original passability reopen"))yield return s;
            var raised=Capture("05-lift",Frame());Need(raised.canPass,"Actual reopen after 4.1s");
            foreach(var s in Until(()=>Frame()!=null && Frame().state==prefix+"WAIT","Original Press WAIT"))yield return s;
            var wait=Capture("06-wait",Frame());Need(wait.uid==start.uid && wait.epoch==start.epoch,"Single original hazard throughout");
            var actualKeys=new HashSet<Rect>{start.atlasRect,first.atlasRect,strike.atlasRect,closed.atlasRect,raised.atlasRect,wait.atlasRect};
            Need(actualKeys.Count>=2,"Original phase changes painted atlas rectangle");report.changedArt=true;
            var callbacks=Array.FindAll(game.ReadFactoryPressRequestsV569(),r=>ReferenceEquals(r.actualOwner,target)&&r.uid==start.uid&&r.epoch==start.epoch);
            foreach(float at in new[]{.01f,2.3f,4.1f})
                Need(Array.Exists(callbacks,r=>r.kind=="source-passability-request" && Mathf.Abs(r.stateTime-at)<.0001f),"Original passability callback "+at);
        }
        IEnumerable BunkerCycle() {
            Enemy bow=null;float nearest=float.PositiveInfinity;
            foreach(var e in game.enemies) {
                if(e.sourceNPC!="NUI_SOLDIER_CROSSBOW"||e.encounterIndex!=spec.encounter)continue;
                float distance=(e.x-target.x)*(e.x-target.x)+(e.y-target.y)*(e.y-target.y);
                if(distance<nearest){nearest=distance;bow=e;}
            }
            Need(bow!=null,"Original siege partner");
            foreach(var s in Until(()=>target.npcState=="BUNKER_WAIT" && target.clip=="idle" && bow.npcState.StartsWith("SOLDIER_CROSSBOW_SIEGE_",StringComparison.Ordinal),"Registered siege pair"))yield return s;
            var rest=Capture("01-idle");
            float hp=target.hp;int dealt=game.ApplyHitToEnemy(target,17,HitReaction.Hurt);
            Need(dealt==17 && target.hp==hp-17,"Original public nonlethal hit");report.damage=dealt;
            bool hurt=false;float from=game.elapsed;
            do {
                yield return null;Check();
                var view=Current();
                if(view && view.sprite && view.sprite.name.StartsWith("bunker/hurt/",StringComparison.Ordinal) && target.clock>=.04f) {
                    var wound=Capture("02-hurt");Need(wound.atlasRect!=rest.atlasRect,"Damage has distinct artwork");hurt=true;break;
                }
            } while(game.elapsed-from<.6f);
            if(!hurt)Capture("02-observed-after-hit");
            foreach(var s in Until(()=>target.stun<=0 && target.invincible<=0,"Original vulnerable fixed bunker"))yield return s;
            hp=target.hp;dealt=game.ApplyHitToEnemy(target,100000,HitReaction.Hurt);Need(dealt==Mathf.CeilToInt(hp) && target.hp==0,"Public lethal closure");
            foreach(var s in Until(()=>target.npcState=="BUNKER_DYING" && target.clip=="broken" && target.clock>=.12f,"Original bunker destruction"))yield return s;
            var breaking=Capture("03-breaking");
            foreach(var s in Until(()=>target.npcState=="BUNKER_DYING" && target.clock>=.65f,"Later destruction pose"))yield return s;
            var later=Capture("04-broken");
            Need(rest.atlasRect!=breaking.atlasRect || rest.atlasRect!=later.atlasRect,"Broken artwork differs from idle");report.changedArt=true;
            Need(!bow.npcState.StartsWith("SOLDIER_CROSSBOW_SIEGE_",StringComparison.Ordinal) && bow.hp>0,"Existing live bowman released after 0.1s");
            report.actions.Add("Verified original siege partner leaves siege on bunker death");
            Need(hurt,"Public nonlethal hit failed to show bunker/hurt in ordinary render");
        }
        IEnumerator Flow() {
            while(!(scene=FindFirstObjectByType<DemoScene>())){Check();yield return null;}
            for(int i=0;i<3;i++)yield return null;
            game=(GameModel)Field(scene,"game");Need((string)Field(scene,"openMenu")=="dungeons" && game.paused,"Ordinary main menu");
            foreach(var step in Enter(spec.region,spec.route))yield return step;
            IEnumerable activity=null;
            if(report.mode=="bunker")activity=BunkerCycle();
            else if(report.mode=="press"||report.mode=="press3")activity=PressCycle();
            else AdditionalStructureStepsV41(ref activity);
            Need(activity!=null,"Explicit production phase observations");
            foreach(var step in activity)yield return step;
            Press("dungeons");for(int i=0;i<4;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons"&&!scene.FactoryProductSessionActiveR1179&&!scene.PlainProductSessionOwnedR1320&&!scene.Mine2ProductSessionOwnedR1233,"Ordinary menu return");report.returnedToMenu=true;
        }
    }
}
#endif
