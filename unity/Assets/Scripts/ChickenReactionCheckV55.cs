#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
    public sealed class ChickenReactionCheckV55 : MonoBehaviour {
        [Serializable] internal sealed class Row {
            public string test,state,npcState,clip,motion,provider,sprite,image;
            public int frames,wholeDraws,whitePartDraws;public float hp,x,y,clock;public bool wholeSelected;
        }
        [Serializable] internal sealed class Report {
            public string schema="tree-room2-chicken-reaction-identity-v55",error="",scope="Original highest-platform Tree room2 birth; normal scene updates and public hits, plus ordinary Z input";
            public bool passed,requireWhole;public int targetBirth,hitCases,whiteFrames,unselectedReactionFrames;public float birthX,birthY;public List<Row> rows=new List<Row>();
        }
        internal DemoScene scene;internal string output;internal Report report=new Report();internal Row current;
        RenderTexture target;Texture2D readback;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--chicken-reaction-review-v55");if(i<0)return;
            var run=new GameObject("Tree room2 chicken reaction check").AddComponent<ChickenReactionCheckV55>();run.output=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(run.output);run.report.requireWhole=Array.IndexOf(args,"--chicken-reaction-require-whole-v55")>=0;Application.runInBackground=true;
        }
        IEnumerator Start(){
            while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>()))yield return null;
            Application.logMessageReceived+=Log;var flow=scene.CheckChickenReactionsV55(this);Exception failure=null;
            while(true){bool more=false;object value=null;try{more=flow.MoveNext();if(more)value=flow.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return value;}
            if(failure!=null)report.error=failure.ToString();report.passed=report.error==""&&report.hitCases==6&&(!report.requireWhole||(report.whiteFrames==0&&report.unselectedReactionFrames==0));
            File.WriteAllText(Path.Combine(output,"reaction-check.json"),JsonUtility.ToJson(report,true));Application.logMessageReceived-=Log;
            if(target){target.Release();Destroy(target);}if(readback)Destroy(readback);UnityEditor.EditorApplication.Exit(report.passed?0:1);
        }
        void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert)report.error=message+"\n"+stack;}
        internal void Render(Camera camera,string image){
            if(!target){target=new RenderTexture(640,360,24){filterMode=FilterMode.Point};target.Create();readback=new Texture2D(640,360,TextureFormat.RGBA32,false);}
            var saved=camera.targetTexture;camera.targetTexture=target;try{camera.Render();}finally{camera.targetTexture=saved;}
            if(image==null)return;var prior=RenderTexture.active;RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,640,360),0,0);readback.Apply();RenderTexture.active=prior;File.WriteAllBytes(Path.Combine(output,image),readback.EncodeToPNG());
        }
        internal void Watch(SpriteRenderer body,bool white){var witness=body.GetComponent<ChickenReactionDrawV55>();if(!witness)witness=body.gameObject.AddComponent<ChickenReactionDrawV55>();witness.owner=this;witness.white=white;}
    }
    public sealed class ChickenReactionDrawV55:MonoBehaviour {
        internal ChickenReactionCheckV55 owner;internal bool white;
        void OnWillRenderObject(){var body=GetComponent<SpriteRenderer>();if(owner&&owner.current!=null&&body.enabled&&!body.forceRenderingOff&&body.color.a>0){if(white)owner.current.whitePartDraws++;else owner.current.wholeDraws++;}}
    }
    public sealed partial class DemoScene {
        public static void StartChickenReactionReviewV55(){
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--chicken-reaction-review-v55")<0)throw new ArgumentException("Explicit output required");
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(UnityEditor.EditorBuildSettings.scenes[0].path);UnityEditor.EditorApplication.EnterPlaymode();
        }
        internal IEnumerator CheckChickenReactionsV55(ChickenReactionCheckV55 check){
            yield return null;Action("dungeons");yield return null;
            int map=mapDungeons.FindIndex(d=>d.id=="tree");MenuAction("region:"+mapDungeons[map].region);RefreshMapVisible();int slot=mapVisible.IndexOf(map);mapPage=slot/MapPageSize;MenuAction("dungeon-card:"+(slot%MapPageSize));MenuAction("dungeon-enter");
            double deadline=Time.realtimeSinceStartupAsDouble+120;while(!adventureStarted||openMenu!=""||Dungeon.id!="tree"||forestPreviewLoading){if(Time.realtimeSinceStartupAsDouble>deadline)throw new TimeoutException("Tree entry");yield return null;}
            var enter=typeof(GameModel).GetMethod("EnterRoom",BindingFlags.Instance|BindingFlags.NonPublic);var hit=typeof(GameModel).GetMethod("ApplyHitToEnemyFromXR1417",BindingFlags.Instance|BindingFlags.NonPublic);
            foreach(string test in new[]{"ordinary-z","front-hurt","rear-hurt","public-launch","source-launch","knockdown"}){
                enter.Invoke(game,new object[]{1});RebuildRoom();yield return null;yield return null;
                Enemy actor=null;int birth=-1;for(int i=0;i<game.enemies.Count;i++){var e=game.enemies[i];if(e.kind=="chicken"&&e.sourceNPC=="NUI_CHICKEN"&&e.active&&!e.dormant&&(actor==null||e.y>actor.y)){actor=e;birth=i;}}
                if(actor==null)throw new InvalidOperationException("Original room2 chicken missing");check.report.targetBirth=birth;check.report.birthX=actor.x;check.report.birthY=actor.y;
                PlaceMonsterPixelReviewPlayerV53(actor,-45);game.player.invincible=120;
                double settle=Time.realtimeSinceStartupAsDouble+.9;while(Time.realtimeSinceStartupAsDouble<settle){game.player.invincible=120;yield return null;}
                float hp=actor.hp;double press=0;bool applied=false;double end=Time.realtimeSinceStartupAsDouble+(test=="ordinary-z"?4:7);
                var rows=new Dictionary<string,ChickenReactionCheckV55.Row>();
                while(Time.realtimeSinceStartupAsDouble<end&&actor.active){
                    game.player.invincible=120;
                    if(!applied){
                        if(test=="ordinary-z"){PlaceMonsterPixelReviewPlayerV53(actor,-45);if(Time.realtimeSinceStartupAsDouble>=press){Action("attack");press=Time.realtimeSinceStartupAsDouble+.3;}applied=actor.hp<hp;}
                        else if(test=="front-hurt"||test=="rear-hurt")applied=(int)hit.Invoke(game,new object[]{actor,1,HitReaction.Hurt,280f,actor.x+actor.facing*(test=="front-hurt"?32:-32),1})>0;
                        else if(test=="source-launch")applied=(int)hit.Invoke(game,new object[]{actor,1,HitReaction.Launch,280f,actor.x+actor.facing*32,5})>0;
                        else applied=game.ApplyHitToEnemy(actor,1,test=="knockdown"?HitReaction.Knockdown:HitReaction.Launch,280)>0;
                    }
                    yield return null;
                    string key=actor.state+"/"+actor.npcState+"/"+actor.clip;
                    bool whole=monsterPixelSelectionsV53.TryGetValue(actor,out var selected);
                    if(!rows.TryGetValue(key,out var row)){row=new ChickenReactionCheckV55.Row{test=test,state=actor.state.ToString(),npcState=actor.npcState,clip=actor.clip,wholeSelected=whole,motion=whole?selected.motion:"",provider=whole?selected.provider:"",sprite=whole?selected.sprite.name:"",hp=actor.hp,x=actor.x,y=actor.y,clock=actor.clock,image=test+"-"+rows.Count+".png"};rows[key]=row;check.report.rows.Add(row);}
                    check.current=row;row.frames++;
                    if(monsterPixelViewsV53.TryGetValue(actor,out var view))check.Watch(view.renderer,false);
                    bool white=false;if(chickenA1ViewsV31.TryGetValue(actor,out var legacy)&&legacy.parts!=null)legacy.parts.VisitRenderersR1461(r=>{check.Watch(r,true);if(r.enabled&&!r.forceRenderingOff&&r.color.a>0)white=true;});
                    if(white)check.report.whiteFrames++;if(actor.state!=CombatState.Normal&&!whole)check.report.unselectedReactionFrames++;
                    check.Render(worldCamera,row.frames==1?row.image:null);
                    if(applied&&actor.state==CombatState.Normal&&Time.realtimeSinceStartupAsDouble>end-5)break;
                }
                if(!applied)throw new InvalidOperationException("Hit did not connect: "+test);check.report.hitCases++;check.current=null;
            }
        }
    }
}
#endif
