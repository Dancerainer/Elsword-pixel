#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed class MagicianPlayerCheckV38:MonoBehaviour {
        const string Flag="--root-magician-check-v38",Output="--root-magician-output-v38";
        [Serializable] public sealed class Register { public string page,motion;public int frame,x,y,anchorX,anchorY;
            public float gripX,gripY,tipX,tipY,robeFloor,staffFloor; }
        [Serializable] sealed class Registers {public string npc;public Register[] rows;}
        [Serializable] public sealed class Sample {public string label,npc,alias,motion,page;public int index,facing;
            public float clock,health,barGap,robeClearance,staffClearance;public int gripNeighbors,tipNeighbors;
            public Vector2 localGrip,localTip;public PixelProof gpu;}
        [Serializable] public sealed class Result {public string error="",last="",gpu;public bool passed;
            public List<string> steps=new List<string>();public List<Sample> samples=new List<Sample>();}
        readonly Result report=new Result();string output,mode;bool done,priorBackground;double start;
        DemoScene scene;GameModel game;Enemy actor;SpriteRenderer view;Dictionary<string,Register> register;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);if(f<0)return;
            try{
                int o=Array.IndexOf(args,Output);
                Need(f==Array.LastIndexOf(args,Flag)&&o>=0&&o+1<args.Length&&f+1<args.Length,"One magician check with mode and output");
                string m=args[f+1];Need(m=="forest"||m=="waterway"||m=="both","Magician mode forest / waterway / both");
                string path=Path.GetFullPath(args[o+1]);
                string allow=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allow,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh runtime-drafts output");
                Need(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11,"Ordinary standalone Direct3D11");
                Directory.CreateDirectory(path);var check=new GameObject("Explicit magician player check").AddComponent<MagicianPlayerCheckV38>();
                check.output=path;check.mode=m;
            }catch(Exception ex){Debug.LogError(ex);Application.Quit(2);}
        }
        IEnumerator Start(){
            start=Time.realtimeSinceStartupAsDouble;priorBackground=Application.runInBackground;Application.runInBackground=true;
            report.gpu=SystemInfo.graphicsDeviceType.ToString();Application.logMessageReceived+=Log;
            var flow=Flow();
            while(!done){bool more=false;object yielded=null;try{
                    Need(Time.realtimeSinceStartupAsDouble-start<170,"Finite magician player deadline: "+report.last);
                    if(!String.IsNullOrEmpty(report.error))throw new InvalidOperationException(report.error);
                    more=flow.MoveNext();if(more)yielded=flow.Current;
                }catch(Exception ex){report.error=ex.ToString();}if(!more||!String.IsNullOrEmpty(report.error))break;yield return yielded;}
            (flow as IDisposable)?.Dispose();Finish();
        }
        void Update(){if(!done&&start>0&&Time.realtimeSinceStartupAsDouble-start>175){report.error="Observer deadline "+report.last;Finish();}}
        void Log(string msg,string trace,LogType type){if(type==LogType.Exception||type==LogType.Assert||type==LogType.Error||msg.Contains("MAGICIAN_SKIN_FALLBACK"))report.error=msg+"\n"+trace;}
        void Finish(){if(done)return;done=true;Application.logMessageReceived-=Log;Application.runInBackground=priorBackground;
            report.passed=String.IsNullOrEmpty(report.error);
            try{File.WriteAllText(Path.Combine(output,"magician-player-report.json"),JsonUtility.ToJson(report,true));}
            finally{Application.Quit(report.passed?0:2);}
        }
        void OnDestroy(){Application.logMessageReceived-=Log;}
        IEnumerator Flow(){
            while(!(scene=FindObjectOfType<DemoScene>())||Field(scene,"game")==null)yield return null;
            while((string)Field(scene,"openMenu")!="dungeons")yield return null;
            if(mode!="waterway")foreach(var t in CheckRoute("dark-forest","forest-dark-magician","forest-1"))yield return t;
            if(mode!="forest")foreach(var t in CheckRoute("underground-waterway","ghost-magician-ragged","waterway-1"))yield return t;
        }
        IEnumerable Enter(string route,string kind,string room){
            report.last="Enter "+route;
            foreach(var t in EnterRoute(scene,"elder",route))yield return t;
            game=(GameModel)Field(scene,"game");
            Need(!game.paused&&game.CurrentRoom.id==room,"Ordinary "+route+" first encounter");
            actor=game.enemies.Find(e=>e.kind==kind&&e.active&&!e.dormant);
            Need(actor!=null,"Originally spawned "+kind);
            view=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
            var reference=Resources.Load<TextAsset>("Pixel/feedback-v38/"+kind+"/registration-check");
            Need(reference,"Magician grip and robe source records");
            var rows=JsonUtility.FromJson<Registers>(reference.text);Need(rows.npc==actor.sourceNPC,"Source NPC");
            register=new Dictionary<string,Register>();foreach(var r in rows.rows)register.Add(r.page+"/"+r.x+"/"+r.y,r);
            game.player.hp=game.player.maxHp=5000;game.player.invincible=120;Place(235);
            for(int i=0;i<3;i++)yield return null;
        }
        void Place(float distance){
            float sign=actor.x>game.CurrentRoom.width*.5f?-1:1;
            float x=Mathf.Clamp(actor.x+sign*distance,32,game.CurrentRoom.width-32);
            var floor=game.SurfaceBelow(x,Mathf.Max(actor.y,game.player.y)+2,true);
            Need(floor!=null,"Hero original ground for natural cast");
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;
            game.player.vx=game.player.vy=0;game.player.invincible=120;
        }
        Sample Observe(string label){
            var w=scene.MagicianFrameV38(view);
            Need(w!=null&&ReferenceEquals(w.enemy,actor)&&view.enabled&&view.gameObject.activeInHierarchy&&
                 ReferenceEquals(w.selected,view.sprite)&&w.carrier&&!ReferenceEquals(w.selected,w.carrier),"Selected original clock + live rewritten mage "+label);
            Need(w.facing==actor.facing&&view.flipX==(actor.facing!=1)&&Mathf.Abs(view.transform.localScale.x-actor.visualScale)<.001f,"Source facing & unit");
            var rect=w.selected.rect;int px=(int)rect.x,py=(int)(view.sprite.texture.height-rect.y-rect.height);
            var r=register[w.page+"/"+px+"/"+py];
            Need(r.motion==w.sourceAnimation,"Registered original staff frame");
            var foot=w.foot;
            float scale=actor.visualScale;
            Vector3 position=view.transform.localPosition;
            float wantedX=foot.x+(rect.width*.5f-r.anchorX)*(view.flipX?-1:1)*scale;
            float wantedY=foot.y+(r.anchorY-rect.height*.5f)*scale;
            Need(Mathf.Abs(wantedX-position.x)<.02f&&Mathf.Abs(wantedY-position.y)<.02f,"Original world pivot / hover");
            if(r.motion=="Wait")Need(r.robeFloor>6&&r.staffFloor>1,"Floating robe and staff clear the floor");
            var camera=(Camera)Field(scene,"worldCamera");
            var bar=((List<SpriteRenderer>)Field(scene,"enemyBars"))[game.enemies.IndexOf(actor)];
            float gap=bar.transform.position.y-view.bounds.max.y;
            if(bar.enabled)Need(Mathf.Abs(gap-8)<1.1f,"Registered live HP top "+gap);
            var targetBefore=camera.targetTexture;var focusBefore=EventSystem.current?EventSystem.current.currentSelectedGameObject:null;
            var viewBefore=view.enabled;
            var sample=new Sample{label=label,npc=actor.sourceNPC,alias=w.alias,motion=w.sourceAnimation,index=w.index,clock=w.clock,
                facing=w.facing,page=w.page,health=actor.hp,barGap=gap,robeClearance=r.robeFloor,staffClearance=r.staffFloor,
                localGrip=new Vector2(r.gripX*w.facing,r.gripY),localTip=new Vector2(r.tipX*w.facing,r.tipY),
                gpu=Capture(camera,game,actor,new List<SpriteRenderer>{view},output,label)};
            var encoded=Resources.Load<TextAsset>(w.page);
            var decoded=new Texture2D(2,2,TextureFormat.RGBA32,false);
            try{
                Need(encoded&&ImageConversion.LoadImage(decoded,encoded.bytes,false),"Actual selected material page");
                sample.gripNeighbors=Neighbors(decoded,r,rect,r.gripX,r.gripY);
                sample.tipNeighbors=Neighbors(decoded,r,rect,r.tipX,r.tipY);
                if(r.motion=="Wait")Need(sample.gripNeighbors>0&&sample.tipNeighbors>0,"Visible source staff grip and unshortened tip");
            }finally{Destroy(decoded);}
            Need(sample.gpu.count>0&&view.enabled==viewBefore&&camera.targetTexture==targetBefore&&
                 (!EventSystem.current||EventSystem.current.currentSelectedGameObject==focusBefore),"Actual scene on/off capture restores camera/focus");
            report.samples.Add(sample);return sample;
        }
        static int Neighbors(Texture2D decoded,Register r,Rect rect,float x,float y){
            int cx=Mathf.RoundToInt(r.anchorX+x),cy=Mathf.RoundToInt(r.anchorY-y);int count=0;
            for(int j=cy-2;j<=cy+2;j++)for(int i=cx-2;i<=cx+2;i++){
                if(i>=0&&j>=0&&i<rect.width&&j<rect.height&&decoded.GetPixel(r.x+i,decoded.height-1-r.y-j).a>.5f)count++;
            }
            return count;
        }
        IEnumerable WaitFor(string alias,string label,float seconds){
            report.last=label+" waiting for "+alias;double end=Time.realtimeSinceStartupAsDouble+seconds;
            for(;;){yield return null;var w=scene.MagicianFrameV38(view);
                var cam=(Camera)Field(scene,"worldCamera");
                if(w!=null&&w.alias==alias&&cam&&GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam),view.bounds)){Observe(label);yield break;}
                Need(Time.realtimeSinceStartupAsDouble<end,report.last+" state="+actor.npcState+" displayed="+(w==null?"none":w.alias));}
        }
        IEnumerable Menu(string route){
            var w=scene.MagicianFrameV38(view);var previous=w==null?null:w.carrier;var painted=w==null?null:w.selected;
            var renderer=view;Press(scene,"dungeons");
            for(int i=0;i<8;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons"&&game.paused&&((IDictionary)Field(scene,"mageBanksV38")).Count==0&&
                 ((IDictionary)Field(scene,"mageWitnessV38")).Count==0&&!painted,"Ordinary map return releases decoded pages and sprites");
            if(renderer&&previous)Need(renderer.sprite!=painted&&!renderer.forceRenderingOff,"Carrier restored on return");
            var routes=(IList)Field(scene,"mapDungeons");int selected=(int)Field(scene,"mapSelected");
            Need(selected>=0&&(string)Field(routes[selected],"id")==route,"Map selection restored "+route);
            report.steps.Add("Returned and released "+route);
        }
        IEnumerable CheckRoute(string route,string kind,string room){
            foreach(var t in Enter(route,kind,room))yield return t;
            foreach(var t in WaitFor("source-idle",kind+"-01-idle",15))yield return t;
            Place(112);
            foreach(var t in WaitFor("source-attack-a-ready",kind+"-02-natural-ready",40))yield return t;
            foreach(var t in WaitFor("source-attack-a",kind+"-03-natural-fire",18))yield return t;
            var oldActor=actor;
            double end=Time.realtimeSinceStartupAsDouble+18;
            int amount=0;while(amount==0){amount=game.ApplyHitToEnemy(actor,1,HitReaction.Hurt);
                if(amount==0){Need(Time.realtimeSinceStartupAsDouble<end,"Reach natural vulnerable mage");yield return null;}}
            // Both play DamageSmall. Waterway selects the original named clip on impact.
            string hurtAlias=kind=="ghost-magician-ragged"?"source-native-DamageSmall":"source-hurt";
            foreach(var t in WaitFor(hurtAlias,kind+"-04-public-hit",8))yield return t;
            foreach(var t in Menu(route))yield return t;
            foreach(var t in Enter(route,kind,room))yield return t;
            Need(!ReferenceEquals(actor,oldActor),"Fresh original actor on re-entry");
            foreach(var t in WaitFor("source-idle",kind+"-05-reentry",15))yield return t;
            foreach(var t in Menu(route))yield return t;
        }
    }
}
#endif
