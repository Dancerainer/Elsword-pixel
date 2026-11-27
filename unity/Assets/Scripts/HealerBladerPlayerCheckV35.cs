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
    public sealed partial class HealerBladerPlayerCheckV35 : MonoBehaviour {
        const string Flag = "--root-healer-blader-check-v35";
        const string OutFlag = "--root-healer-blader-output-v35";
        const string RouteFlag = "--root-healer-blader-route-v35";
        const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        [Serializable] sealed class Shot {
            public string label, state, motion, key, onFile, offFile;
            public int frame, facing, renderers, rendered, pixels;
            public float npcTime, sourceClock, alpha, hp;
        }
        [Serializable] sealed class Report {
            public string schema = "HEALER_BLADER_SOURCE_BIRTH_AND_GPU_DIFFERENCE_V35", route, graphics, error;
            public bool passed,batchMode,windowPresentationVerified,ordinaryMenu,returned,reentered,release,healHpWrite,corePreEffect,bladeHpHit;
            public int healedAllyIndex=-1,bladeHitDamage;
            public float woundedHp,healedHp,healPacketModelTime,healClock,bladeClock;
            public int room,encounter,birthIndex,originalCameraDraws,frames;
            public List<string> actions = new List<string>(), errors = new List<string>(), motions = new List<string>();
            public List<Shot> shots = new List<Shot>();
        }
        readonly Report report = new Report();
        readonly List<SpriteRenderer> parts = new List<SpriteRenderer>();
        readonly Dictionary<SpriteRenderer,int> draws = new Dictionary<SpriteRenderer,int>();
        readonly HashSet<string> taken = new HashSet<string>();
        DemoScene scene;
        GameModel game, original;
        Camera cameraOwner;
        Enemy target;
        RenderTexture rt;
        string output, route;
        bool finished, oldBackground, evidenceDraw;
        int posted;
        double started;
        const int Width=1280, Height=720;
        const double Budget=400;
        bool Core => route=="core";
        bool RouteLive => Core ? scene.CoreHardProductSessionActiveR1147 : scene.FactoryProductSessionActiveR1179;
        static void Need(bool ok,string message) { if(!ok) throw new InvalidOperationException("Healer Blader V35 "+message); }
        static object Field(object obj,string key) {
            var f=obj?.GetType().GetField(key,Instance); Need(f!=null,"Missing field "+key); return f.GetValue(obj);
        }
        static object Call(object obj,string key,params object[] args) {
            var m=obj?.GetType().GetMethod(key,Instance); Need(m!=null,"Missing method "+key); return m.Invoke(obj,args);
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            var args=Environment.GetCommandLineArgs();
            if(Array.IndexOf(args,Flag)<0) return;
            try {
                string path=null, route="core";
                int n=0,o=0,r=0;
                for(int i=0;i<args.Length;i++){
                    if(args[i]==Flag){n++;continue;}
                    if(args[i]==OutFlag){o++;path=args[++i];continue;}
                    if(args[i]==RouteFlag){r++;route=args[++i];continue;}
                    Need(!(args[i].StartsWith("--root-") && args[i].Contains("-check-")),"Other driving fixture flag "+args[i]);
                }
                Need(n==1 && o==1 && r<=1 && (route=="core"||route=="factory"),"One flag/output, route core or factory");
                Need(SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null,"Actual Standalone GPU required");
                string full=Path.GetFullPath(path);
                Need(full.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase)&&
                    !Directory.Exists(full)&&!File.Exists(full),"Fresh owned output directory");
                Directory.CreateDirectory(full);
                var c=new GameObject("Explicit Healer Blader GPU check V35").AddComponent<HealerBladerPlayerCheckV35>();
                c.output=full;c.route=route;
            } catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        void Log(string msg,string stack,LogType type) {
            if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert||msg.Contains("HEALER_BLADER_PARTS_FALLBACK"))
                report.errors.Add(msg+"\n"+stack);
        }
        void Check() {
            Need(report.errors.Count==0,"Runtime error recorded");
            Need(Time.realtimeSinceStartupAsDouble-started<Budget,"Total wall deadline");
        }
        void Update(){
            if(!finished&&started>0&&Time.realtimeSinceStartupAsDouble-started>=Budget)Finish("Total wall deadline");
        }
        IEnumerator Start(){
            started=Time.realtimeSinceStartupAsDouble;oldBackground=Application.runInBackground;
            Application.runInBackground=true;report.route=route;report.batchMode=Application.isBatchMode;
            report.graphics=SystemInfo.graphicsDeviceType.ToString();Application.logMessageReceived+=Log;Camera.onPostRender+=Post;
            var flow=Run();string failure=null;
            while(!finished) {
                object next=null;bool more=false;
                try { Check();DrawAuthoredOriginalCamera();more=flow.MoveNext();if(more)next=flow.Current; }
                catch(Exception e) {failure=e.ToString();}
                if(failure!=null||!more)break;
                yield return next;
            }
            (flow as IDisposable)?.Dispose();
            if(!finished)Finish(failure);
        }
        void Finish(string failure){
            if(finished)return;finished=true;report.passed=failure==null&&report.errors.Count==0;report.error=failure;
            Application.logMessageReceived-=Log;Camera.onPostRender-=Post;
            int code=report.passed?0:2;
            try {File.WriteAllText(Path.Combine(output,"healer-blader-player-report.json"),JsonUtility.ToJson(report,true));}
            catch(Exception e){Debug.LogException(e);code=2;}
            Application.Quit(code);
        }
        void OnDestroy() {
            Application.logMessageReceived-=Log;Camera.onPostRender-=Post;
            if(rt){rt.Release();Destroy(rt);}if(started>0)Application.runInBackground=oldBackground;
        }
        void Press(string action){
            TouchControl control=null;
            foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))
                if(c&&c.isActiveAndEnabled&&c.gameObject.activeInHierarchy&&ReferenceEquals(c.owner,scene)&&c.action==action) {
                    Need(!control,"Duplicate ordinary control "+action);control=c;
                }
            Need(control&&EventSystem.current&&control.GetComponent<Image>()&&control.GetComponent<Image>().raycastTarget,"Ordinary UI "+action);
            var p=new PointerEventData(EventSystem.current){pointerId=-3535,button=PointerEventData.InputButton.Left,
                position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
            try{Need(ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerDownHandler),"Rejected "+action);}
            finally{if(control)ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerUpHandler);}
            report.actions.Add("ordinary-ui "+action);
        }
        IEnumerable Enter(){
            var rows=(IList)Field(scene,"mapDungeons"); int index=-1;
            string id=Core?"altera-core-hard":"altera-nasod-factory";
            for(int i=0;i<rows.Count;i++)if((string)Field(rows[i],"id")==id)index=i;
            Need(index>=0,"Original route "+id);Press("region:altera");yield return null;
            var visible=(IList)Field(scene,"mapVisible");int at=-1;
            for(int i=0;i<visible.Count;i++)if((int)visible[i]==index)at=i;
            Need(at>=0,"Visible route "+id);
            int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();
            int page=at/size,guard=0;
            while((int)Field(scene,"mapPage")!=page){
                Need(++guard<20,"Menu page bound");Press("dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1"));yield return null;
            }
            Press("dungeon-card:"+(at%size));yield return null;Need((int)Field(scene,"mapSelected")==index,"Selected route");Press("dungeon-enter");
            while(!RouteLive||(Core?scene.CoreHardProductSessionLoadingR1147:scene.FactoryProductSessionLoadingR1179))yield return null;
            game=(GameModel)Field(scene,"game");Need(game.room==0&&!game.paused,"Ordinary route room zero");
            report.ordinaryMenu=true;
        }
        void Stage(){
            Call(game,"EnterRoom",0);if(game.EncounterIndex!=1)Call(game,"BeginEncounter",1);
            // Factory birth 14 shares exact source station 42 with Press3. This
            // check needs independently measurable body pixels through a cycle.
            // Birth 17 is the same original actor at station 62 on line 18.
            report.birthIndex=Core?25:17;report.room=0;report.encounter=1;
            target=game.enemies[report.birthIndex];
            Need(target.sourceNPC==HealerBladerRigV35.Id&&target.kind==HealerBladerRigV35.Kind&&target.encounterIndex==1,"Original exact Healer Blader birth");
            game.traceNpcActions=true;Place(-90);Call(scene,"RebuildRoom");
            report.actions.Add("Controlled original room 0 encounter 1 source index "+report.birthIndex+"; no actor state/clock/AI assignment");
        }
        void Place(int offset) {
            game.player.x=target.x+offset;game.player.y=target.y;game.player.groundLine=target.groundLine;
            game.player.vx=game.player.vy=0;game.player.invincible=120;
        }
        bool CanHit(Enemy enemy) {
            return enemy!=null&&enemy.active&&!enemy.dormant&&enemy.hp>0&&enemy.state==CombatState.Normal&&
                !string.IsNullOrEmpty(enemy.npcState)&&!(bool)Call(game,"EnemyHasSuperArmor",enemy)&&
                !(bool)Call(game,"EnemyHasNpcInvincibility",enemy)&&(bool)Call(game,"Grounded",enemy);
        }
        void DrawAuthoredOriginalCamera(){
            if(!scene||game==null||!RouteLive||target==null)return;
            bool active=Core?scene.CoreHardProductAuthoredCameraActiveR1147:Field(scene,"factoryRavenTicketV21")!=null;
            if(!active)return;
            var c=(Camera)Field(scene,"worldCamera");Need(c&&c.enabled&&c.targetTexture==null,"Original owner camera");
            float time=game.elapsed;var old=RenderTexture.active;
            try{c.Render();report.originalCameraDraws++;}finally{RenderTexture.active=old;}
            Need(game.elapsed==time&&c.targetTexture==null,"Original camera cannot advance model");
        }
    }
}
#endif
