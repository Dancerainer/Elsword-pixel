#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class PlainWatchWholePlayerCheckV36:MonoBehaviour {
        const string Flag="--root-plain-watch-check-v36",Output="--root-plain-watch-output-v36",Route="--root-plain-watch-route-v36";
        [Serializable] sealed class Row {
            public string label,source,bankMotion,key,pngSha;public float clock,alpha,hp;
            public PixelProof pixels;
        }
        [Serializable] sealed class Audit {
            public string route,error;
            public bool passed,ordinaryMenu,returned,originalBankReleased,windowPresentationVerified;
            public int room,index,encounter;
            public List<Row> rows=new List<Row>();public List<string> errors=new List<string>();
        }
        readonly Audit audit=new Audit();
        string output;
        bool done,background;
        double started;
        DemoScene scene;GameModel game,main;Enemy actor;
        SpriteRenderer carrier;
        SourcePhaseWholeLookupV21 lookup;
        IDictionary sprites;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,Flag)<0)return;
            try{
                int f=Array.IndexOf(args,Flag),o=Array.IndexOf(args,Output),r=Array.IndexOf(args,Route);
                Need(f==Array.LastIndexOf(args,Flag)&&o>=0&&o==Array.LastIndexOf(args,Output)&&o+1<args.Length&&
                    r>=0&&r==Array.LastIndexOf(args,Route)&&r+1<args.Length,"Single explicit Watch check, output and route");
                foreach(var value in args)Need(value==Flag||!value.StartsWith("--root-")||!value.Contains("-check-"),"One driving fixture");
                string route=args[r+1];Need(route=="altera-plain"||route=="altera-plain-recycle","Ordinary original Plain route");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"Real D3D11");
                string path=Path.GetFullPath(args[o+1]),parent=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(parent,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh owned output");
                Directory.CreateDirectory(path);var test=new GameObject("Original Plain Watch death draw V36").AddComponent<PlainWatchWholePlayerCheckV36>();
                test.output=path;test.audit.route=route;
            }catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
        }
        void Log(string message,string stack,LogType type){
            if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)audit.errors.Add(message+"\n"+stack);
        }
        void Finish(string error){
            if(done)return;done=true;audit.error=error;audit.passed=error==null&&audit.errors.Count==0;
            Application.logMessageReceived-=Log;int code=audit.passed?0:2;
            try{File.WriteAllText(Path.Combine(output,"plain-watch-whole-report.json"),JsonUtility.ToJson(audit,true));}
            catch(Exception ex){Debug.LogException(ex);code=2;}
            Application.Quit(code);
        }
        IEnumerator Start(){
            background=Application.runInBackground;Application.runInBackground=true;started=Time.realtimeSinceStartupAsDouble;
            Application.logMessageReceived+=Log;var flow=Flow();string failure=null;
            while(!done){object next=null;bool more=false;
                try{Need(audit.errors.Count==0,"Player runtime exception");more=flow.MoveNext();if(more)next=flow.Current;}
                catch(Exception ex){failure=ex.ToString();}
                if(!more||failure!=null)break;yield return next;
            }
            (flow as IDisposable)?.Dispose();if(!done)Finish(failure);
        }
        void Update(){if(!done&&started>0&&Time.realtimeSinceStartupAsDouble-started>150)Finish("Watch overall wall deadline");}
        void OnDestroy(){Application.logMessageReceived-=Log;if(started>0)Application.runInBackground=background;}
        void Place(){
            game.player.x=actor.x-actor.facing*40;game.player.y=actor.y;game.player.groundLine=actor.groundLine;
            game.player.vx=game.player.vy=0;game.player.invincible=120;
        }
        void Observe(string label,bool dead){
            string state=actor.npcState;float time=actor.npcTime;
            if(dead)Need(game.TryPlainWatchLethalPoseV36(actor,out state,out time)&&state=="NASOD_WATCH_DYING_LAND_BACK","Actual registered Watch lethal packet");
            var selected=lookup.Select(state,time);var key=selected.Whole;
            Need(carrier && carrier.enabled && carrier.gameObject.activeInHierarchy && !carrier.forceRenderingOff &&
                carrier.sprite==(Sprite)sprites[key.Key]&&!carrier.flipX&&!carrier.flipY,"Exact final original source sprite "+label);
            var row=new Row{label=label,source=state,bankMotion=selected.State.Motion,key=key.Key,pngSha=key.PngSha256,clock=time,alpha=carrier.color.a,hp=actor.hp};
            if(dead)Need(selected.State.Motion=="DamageDown" && Mathf.Abs(carrier.color.a-(1-actor.deathTime/1.2f))<.0001f,
                "Selected original death and actual inherited alpha");
            audit.rows.Add(row);
            row.pixels=Capture((Camera)Field(scene,"worldCamera"),game,actor,new List<SpriteRenderer>{carrier},output,label);
            Need(row.pixels.count>0,"Positive draw contribution "+label);
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;
            for(int i=0;i<3;i++)yield return null;
            main=(GameModel)Field(scene,"game");Need(main.paused&&(string)Field(scene,"openMenu")=="dungeons","Ordinary dungeon menu");audit.ordinaryMenu=true;
            foreach(var step in EnterRoute(scene,"altera",audit.route))yield return step;
            while(!scene.PlainProductSessionActiveR1320)yield return null;
            game=(GameModel)Field(scene,"game");actor=game.enemies.Find(e=>e.sourceNPC=="NUI_NASOD_WATCH"&&e.encounterIndex==game.EncounterIndex&&e.active&&!e.dormant);
            Need(actor!=null && game.room==0 && game.EncounterIndex==0,"Original route birth");audit.room=game.room;audit.encounter=game.EncounterIndex;audit.index=game.enemies.IndexOf(actor);
            lookup=(SourcePhaseWholeLookupV21)Field(scene,"plainWatchLookupR1330");sprites=(IDictionary)Field(scene,"plainWatchSpritesR1330");
            Need(ReferenceEquals(Field(scene,"plainWatchModelR1330"),game)&&lookup.KeyCount==44&&sprites.Count==44,"Owned source whole library");
            double limit=Time.realtimeSinceStartupAsDouble+35;
            do{
                Place();yield return null;Need(Time.realtimeSinceStartupAsDouble<limit,"Original Watch visible, grounded and vulnerable station");
                carrier=(SpriteRenderer)((IList)Field(scene,"enemyViews"))[audit.index];
            }while(!carrier || !carrier.sprite || !carrier.enabled || !carrier.gameObject.activeInHierarchy || actor.state!=CombatState.Normal ||
                string.IsNullOrEmpty(actor.npcState)||!game.Grounded(actor)||game.EnemyHasNpcInvincibility(actor)||game.EnemyHasSuperArmor(actor));
            Observe("original-birth",false);
            Place();int hit=game.ApplyHitToEnemy(actor,Mathf.CeilToInt(actor.hp*2)+100,HitReaction.Hurt);
            Need(hit>0 && actor.hp==0 && actor.state==CombatState.Dead,"Ordinary public lethal");
            yield return null;Observe("death-first",true);
            float first=carrier.color.a;limit=Time.realtimeSinceStartupAsDouble+10;
            while(actor.deathTime<.55f){yield return null;Need(Time.realtimeSinceStartupAsDouble<limit,"Watch fade timer");}
            Observe("death-mid",true);Need(carrier.color.a>0&&carrier.color.a<first,"Real original positive fade");
            var retired=carrier.sprite;Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;
            Need(!scene.PlainProductSessionActiveR1320 && ReferenceEquals(Field(scene,"game"),main) && (string)Field(scene,"openMenu")=="dungeons" &&
                Field(scene,"plainWatchLookupR1330")==null && !retired,"Main restored and private original Sprite released");
            audit.returned=audit.originalBankReleased=true;
        }
    }
}
#endif
