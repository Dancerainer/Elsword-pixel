#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class BattleRavenWholePlayerCheckV36:MonoBehaviour {
        const string Flag="--root-battle-raven-check-v36",Output="--root-battle-raven-output-v36";
        [Serializable] sealed class Row {
            public string label,state,alias;public float clock,hp,alpha;public int key;
            public PixelProof pixels;
        }
        [Serializable] sealed class Audit {
            public string error;
            public bool passed,hiddenDirector,actualRelease,victoryHold,returned,released,windowPresentationVerified;
            public List<Row> rows=new List<Row>();public List<string> errors=new List<string>();
        }
        readonly Audit audit=new Audit();string output;bool done,background;double started;
        DemoScene scene;GameModel game,main;Enemy actor;SpriteRenderer carrier;IDictionary frames,clips;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);if(f<0)return;
            try{
                int o=Array.IndexOf(args,Output);Need(f==Array.LastIndexOf(args,Flag)&&o>=0&&o==Array.LastIndexOf(args,Output)&&o+1<args.Length,"Single Raven check/output");
                foreach(var a in args)Need(a==Flag||!a.StartsWith("--root-")||!a.Contains("-check-"),"One driving check");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"D3D11");
                string p=Path.GetFullPath(args[o+1]),home=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(p.StartsWith(home,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(p)&&!File.Exists(p),"Fresh owned output");
                Directory.CreateDirectory(p);var test=new GameObject("Controlled Battle original Raven V36").AddComponent<BattleRavenWholePlayerCheckV36>();test.output=p;
            }catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
        }
        void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)audit.errors.Add(message+"\n"+stack);}
        void Finish(string error){
            if(done)return;done=true;audit.error=error;audit.passed=error==null&&audit.errors.Count==0;Application.logMessageReceived-=Log;
            int code=audit.passed?0:2;try{File.WriteAllText(Path.Combine(output,"battle-raven-whole-report.json"),JsonUtility.ToJson(audit,true));}
            catch(Exception ex){Debug.LogException(ex);code=2;}Application.Quit(code);
        }
        IEnumerator Start(){
            background=Application.runInBackground;Application.runInBackground=true;started=Time.realtimeSinceStartupAsDouble;
            Application.logMessageReceived+=Log;var flow=Flow();string error=null;
            while(!done){bool more=false;object next=null;
                try{Need(audit.errors.Count==0,"Player runtime error");more=flow.MoveNext();if(more)next=flow.Current;}
                catch(Exception ex){error=ex.ToString();}
                if(!more||error!=null)break;yield return next;
            }
            (flow as IDisposable)?.Dispose();if(!done)Finish(error);
        }
        void Update(){if(!done&&started>0&&Time.realtimeSinceStartupAsDouble-started>190)Finish("Battle Raven wall deadline");}
        void OnDestroy(){Application.logMessageReceived-=Log;if(started>0)Application.runInBackground=background;}
        void Place(){game.player.x=actor.x-actor.facing*40;game.player.y=actor.y;game.player.groundLine=actor.groundLine;game.player.vx=game.player.vy=0;game.player.invincible=120;}
        Row Observe(string label,bool dead){
            string state=actor.npcState,alias=actor.clip;float t=actor.clock;
            if(dead)Need(game.TryBattleRavenLethalPoseV36(actor,out state,out alias,out t)&&
                state=="RAVEN_DYING_LAND_BACK"&&alias=="down-back","Recorded real rear fatal state, existing source alias");
            var originals=(Sprite[])frames[alias];var clip=(AnimationData)clips[alias];
            if(clip.loop&&clip.duration>0)t=Mathf.Repeat(t,clip.duration);
            int index=Mathf.Clamp(Mathf.FloorToInt(t*clip.fps),0,originals.Length-1);
            Need(carrier && carrier.sprite==originals[index] && carrier.gameObject.activeInHierarchy && carrier.enabled && !carrier.forceRenderingOff,"Exact current original Battle private sprite "+label);
            var row=new Row{label=label,state=state,alias=alias,clock=t,hp=actor.hp,alpha=carrier.color.a,key=index};
            if(dead)Need(Mathf.Abs(row.alpha-(1-actor.deathTime/1.2f))<.0001f,"Actual source carrier alpha");
            audit.rows.Add(row);row.pixels=Capture((Camera)Field(scene,"worldCamera"),game,actor,new List<SpriteRenderer>{carrier},output,label);
            Need(row.pixels.count>0,"Positive actual world-camera contribution "+label);return row;
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;for(int i=0;i<3;i++)yield return null;
            main=(GameModel)Field(scene,"game");Need(main.paused&&(string)Field(scene,"openMenu")=="dungeons","Ordinary menu");
            foreach(var step in EnterRoute(scene,"altera","altera-battle-airship"))yield return step;
            while(!scene.BattleProductSessionActiveR1325)yield return null;
            game=(GameModel)Field(scene,"game");Call(game,"EnterRoom",3);if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);Call(scene,"RebuildRoom");
            actor=game.enemies[0];Need(actor.sourceNPC=="NUI_RAVEN"&&actor.kind=="raven"&&game.room==3&&game.EncounterIndex==0,"Original boss birth 3/0/0");
            carrier=(SpriteRenderer)((IList)Field(scene,"enemyViews"))[0];var lib=((IDictionary)Field(scene,"libraries"))[actor.kind];
            frames=(IDictionary)Field(lib,"frames");clips=(IDictionary)Field(lib,"clips");
            Need(frames.Count==31 && !frames.Contains("death") && !frames.Contains("idle"),"Original 31-clip Raven library");
            Place();yield return null;Need(game.TryAlteraAirshipRavenPresentationV18(actor,out bool shown,out bool gauge)&&!shown&&!gauge&&!carrier.gameObject.activeInHierarchy,"Actual hidden original director phase");
            audit.hiddenDirector=true;double limit=Time.realtimeSinceStartupAsDouble+75;
            do{Place();yield return null;game.TryAlteraAirshipRavenPresentationV18(actor,out shown,out gauge);Need(Time.realtimeSinceStartupAsDouble<limit,"Original director then vulnerable Raven");}
            while(!shown||!gauge||!actor.aiEnabled||!game.Grounded(actor)||actor.state!=CombatState.Normal||string.IsNullOrEmpty(actor.npcState)||
                  !carrier.gameObject.activeInHierarchy||game.EnemyHasNpcInvincibility(actor)||game.EnemyHasSuperArmor(actor));
            audit.actualRelease=true;Observe("released-boss",false);
            Place();int hit=game.ApplyHitToEnemy(actor,Mathf.CeilToInt(actor.hp*2)+100,HitReaction.Hurt);
            Need(hit>0&&actor.hp==0&&actor.state==CombatState.Dead,"Ordinary public lethal");yield return null;
            Observe("death-first",true);limit=Time.realtimeSinceStartupAsDouble+10;
            while(!game.victory){yield return null;Need(Time.realtimeSinceStartupAsDouble<limit,"Original final-Raven victory");}
            var won=Observe("boss-victory",true);float time=game.elapsed;
            for(int i=0;i<8;i++)yield return null;
            var held=Observe("victory-held",true);Need(game.victory&&game.elapsed==time&&won.clock==held.clock&&won.alpha==held.alpha,"Frozen original victory frame");
            audit.victoryHold=true;var retired=carrier.sprite;Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;
            Need(!scene.BattleProductSessionActiveR1325&&ReferenceEquals(main,Field(scene,"game"))&&(string)Field(scene,"openMenu")=="dungeons"&&!retired,"Original restored Main and retired private sprite");
            audit.returned=audit.released=true;
        }
    }
}
#endif
