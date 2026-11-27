#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class WallyType2PlayerCheckV36:MonoBehaviour {
        const string Flag="--root-wally-type2-check-v36",Out="--root-wally-type2-output-v36";
        [Serializable] sealed class Shot {public string state,motion,label;public int facing,partCount;public float clock,x,y;public PixelProof pixels;}
        [Serializable] sealed class Report {
            public bool passed,ordinaryBirth,ownTwoBlades,gait,originalAttack,publicHurt,lethal,reentry,returned;
            public int index,occludedSamples;public string route,room,last,error;
            public List<Shot> shots=new List<Shot>();public List<string> errors=new List<string>();
        }
        readonly Report report=new Report();string output;DemoScene scene;GameModel game;Enemy actor;bool finished,background;
        double start;
        bool Core=>report.route=="core";
        bool Active=>Core?scene.CoreHardProductSessionActiveR1147:scene.FactoryProductSessionActiveR1179;
        Camera Camera=>Field(scene,"worldCamera")as Camera;
        static void NeedHere(bool ok,string why){if(!ok)throw new InvalidOperationException("Wally Type2 Player: "+why);}
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);if(f<0)return;
            try{
                int o=Array.IndexOf(args,Out);NeedHere(f+1<args.Length&&o>=0&&o+1<args.Length,"Original route and output");
                string route=args[f+1],dir=Path.GetFullPath(args[o+1]);string drafts=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                NeedHere((route=="core"||route=="factory")&&dir.StartsWith(drafts,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(dir)&&!File.Exists(dir),"Fresh owned route output");
                NeedHere(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"Original D3D11");
                Directory.CreateDirectory(dir);var c=new GameObject("Explicit Type2 source rig").AddComponent<WallyType2PlayerCheckV36>();
                c.output=dir;c.report.route=route;
            }catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
        }
        void Log(string line,string stack,LogType type){
            if(type==LogType.Error||type==LogType.Assert||type==LogType.Exception||line.Contains("CORE_ELITE_ART_FALLBACK_V36"))report.errors.Add(line+" "+stack);
        }
        void Check(){
            NeedHere(report.errors.Count==0,"Source rig errors "+(report.errors.Count>0?report.errors[0]:""));
            NeedHere(Time.realtimeSinceStartupAsDouble-start<430,"Route deadline "+report.last);
            if(actor!=null)report.last=actor.npcState+" "+actor.npcTime+" "+actor.state+" x="+actor.x+" y="+actor.y;
        }
        IEnumerator Start(){
            background=Application.runInBackground;Application.runInBackground=true;
            Application.logMessageReceived+=Log;start=Time.realtimeSinceStartupAsDouble;var run=Flow();Exception failure=null;
            for(;;){
                bool more=false;object current=null;
                try{Check();if(scene&&Core&&Active&&scene.CoreHardProductAuthoredCameraActiveR1147&&Camera&&!Camera.targetTexture)Camera.Render();
                    more=run.MoveNext();if(more)current=run.Current;
                }catch(Exception e){failure=e;}
                if(failure!=null||!more)break;yield return current;
            }
            (run as IDisposable)?.Dispose();report.error=failure?.ToString();report.passed=failure==null&&report.errors.Count==0;
            Complete();
        }
        void Complete(){
            if(finished)return;finished=true;Application.logMessageReceived-=Log;Application.runInBackground=background;
            try{File.WriteAllText(Path.Combine(output,"wally-type2-player-report.json"),JsonUtility.ToJson(report,true));}
            finally{Application.Quit(report.passed?0:2);}
        }
        IEnumerable Enter(){
            foreach(var x in EnterRoute(scene,"altera",Core?"altera-core-hard":"altera-nasod-factory"))yield return x;
            var until=Time.realtimeSinceStartupAsDouble+125;
            while(!Active||(game=(GameModel)Field(scene,"game"))==null||game.paused||(string)Field(scene,"openMenu")!=""){
                NeedHere(Time.realtimeSinceStartupAsDouble<until,"Original product admission");yield return null;
            }
            NeedHere(game.room==0&&game.EncounterIndex==0,"First original room and encounter");
            report.index=Core?2:1;actor=game.enemies[report.index];report.room=game.CurrentRoom.id;
            NeedHere(actor.active&&!actor.dormant&&actor.kind=="wally-9th-type2"&&actor.sourceNPC==CoreElitePartsV36.Wally&&actor.encounterIndex==0,"Original Wally Type2 source birth");
            game.player.hp=game.player.maxHp=100000;game.player.invincible=25;report.ordinaryBirth=true;
        }
        void Place(float delta){
            var encounter=game.CurrentEncounter;float x=actor.x+delta;
            if(x<encounter.leftX+.04f||x>encounter.rightX-.04f)x=actor.x-delta;
            NeedHere(x>=encounter.leftX+.025f&&x<=encounter.rightX-.025f,"Valid encounter position");
            var ground=game.SurfaceBelow(x,actor.y+15,true)??game.SurfaceBelow(x,actor.y+170,true);
            NeedHere(ground!=null,"Original authored floor");game.player.x=x;game.player.y=ground.Y(x);game.player.groundLine=ground.id;
            game.player.vx=game.player.vy=0;game.player.invincible=25;
        }
        bool TryShow(string motion,string label,int facing,float min,float max,string single=null){
            var views=(IDictionary)Field(scene,"eliteViewsV36");var row=views[actor];if(row==null||(int)Field(row,"frame")!=Time.frameCount)return false;
            float at=(float)Field(row,"clock");if((string)Field(row,"motion")!=motion||at<min||at>max||(facing!=0&&actor.facing!=facing))return false;
            var parts=new List<SpriteRenderer>();if(!scene.VisitCoreElitePartsV36(actor,parts.Add))return false;
            NeedHere(parts.Count==16&&((SpriteRenderer)Field(row,"carrier")).forceRenderingOff,"Actual sixteen source parts");
            var ranks=new HashSet<int>();foreach(var part in parts){NeedHere(part&&part.enabled&&part.sprite,"Live rendered Wally part");ranks.Add(part.sortingOrder);}
            NeedHere(ranks.Count==16,"Original per-pose depth has sixteen unique layers");
            if(single!=null){parts.RemoveAll(p=>p.name!=single);NeedHere(parts.Count==1,"Original Wally socket "+single);}
            bool onscreen=false;foreach(var p in parts){var v=Camera.WorldToViewportPoint(p.bounds.center);onscreen|=v.z>0&&v.x>.01f&&v.x<.99f&&v.y>.01f&&v.y<.99f;}
            if(!onscreen)return false;
            int h=Mathf.RoundToInt(2*Camera.orthographicSize),w=Mathf.RoundToInt(h*Camera.aspect);PixelProof proof;
            try{proof=CaptureAtResolution(Camera,game,actor,parts,output,label,w,h);}
            catch(InvalidOperationException ex)when(ex.Message.StartsWith("Rig Player V35: Visible actual GPU pixels ",StringComparison.Ordinal)){report.occludedSamples++;return false;}
            report.shots.Add(new Shot{state=actor.npcState,motion=motion,clock=at,facing=actor.facing,x=actor.x,y=actor.y,label=label,pixels=proof,partCount=parts.Count});return true;
        }
        IEnumerable Wait(string motion,string label,float dist,int face,float min=0,float max=float.MaxValue,string single=null){
            double until=Time.realtimeSinceStartupAsDouble+58;
            for(;;){Place(dist);yield return null;Check();if(TryShow(motion,label,face,min,max,single))yield break;
                NeedHere(Time.realtimeSinceStartupAsDouble<until,"Visible original "+label+" "+report.last);}
        }
        IEnumerable Return(){
            while(scene.BossIntroductionActive)yield return null;Press(scene,"dungeons");for(int i=0;i<7;i++)yield return null;
            NeedHere(!Active&&(string)Field(scene,"openMenu")=="dungeons"&&((IDictionary)Field(scene,"eliteViewsV36")).Count==0&&((IDictionary)Field(scene,"eliteLibrariesV36")).Count==0,"Released original Wally parts and source banks");report.returned=true;
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;for(int j=0;j<4;j++)yield return null;
            if((string)Field(scene,"openMenu")!="dungeons"){Call(scene,"Action","dungeons");yield return null;}
            foreach(var x in Enter())yield return x;
            foreach(var x in Wait("Wait","01-neutral",75,0))yield return x;
            foreach(var x in Wait("Walk","02-right-first",161,1,.08f,.46f))yield return x;
            foreach(var x in Wait("Walk","03-right-second",161,1,.55f,.92f))yield return x;
            foreach(var x in Wait("Walk","04-left-first",-161,-1,.08f,.46f))yield return x;
            foreach(var x in Wait("Walk","05-left-second",-161,-1,.55f,.92f))yield return x;report.gait=true;
            foreach(var x in Wait("Walk","06-neck-socket",-161,-1,0,1,"neck"))yield return x;
            foreach(var x in Wait("AttackA_Fire","07-source-attack",55,0,.4f,1.7f))yield return x;
            foreach(var x in Wait("AttackA_Fire","08-weapon01",55,0,0,2.6667f,"near-blade"))yield return x;
            foreach(var x in Wait("AttackA_Fire","09-weapon02",55,0,0,2.6667f,"far-blade"))yield return x;report.ownTwoBlades=report.originalAttack=true;
            var until=Time.realtimeSinceStartupAsDouble+58;
            while(!report.publicHurt){
                Place(55);yield return null;Check();
                if(game.Grounded(actor)&&actor.state==CombatState.Normal&&!game.EnemyHasSuperArmor(actor)&&!game.EnemyHasNpcInvincibility(actor))
                    game.ApplyHitToEnemy(actor,1,HitReaction.Hurt);
                if(actor.state==CombatState.Hurt&&TryShow("DamageSmall","10-public-hurt",0,0,float.MaxValue))report.publicHurt=true;
                NeedHere(Time.realtimeSinceStartupAsDouble<until,"Public Wally hurt "+report.last);
            }
            until=Time.realtimeSinceStartupAsDouble+58;
            while(actor.hp>0){Place(55);yield return null;Check();
                if(actor.state==CombatState.Normal&&!game.EnemyHasSuperArmor(actor)&&!game.EnemyHasNpcInvincibility(actor))
                    game.ApplyHitToEnemy(actor,100000,HitReaction.Hurt);
                NeedHere(Time.realtimeSinceStartupAsDouble<until,"Public Wally lethal "+report.last);}
            report.lethal=actor.state==CombatState.Dead;NeedHere(report.lethal,"Original Wally dead");
            var first=actor;foreach(var x in Return())yield return x;foreach(var x in Enter())yield return x;
            NeedHere(first!=actor,"Fresh source birth");foreach(var x in Wait("Wait","11-reentry",75,0))yield return x;report.reentry=true;
            foreach(var x in Return())yield return x;
        }
    }
}
#endif
