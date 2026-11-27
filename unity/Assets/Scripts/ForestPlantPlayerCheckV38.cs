#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class ForestPlantPlayerCheckV38:MonoBehaviour {
        [Serializable] sealed class Row { public string label,sourceState,motion,alias;public int index,direction;public float clock,y,hp;public PixelProof pixels; }
        [Serializable] sealed class Report {
            public bool passed,naturalAttack,hit,returned,freshEntry,sourceStartCompleted; public string error="",last="",kind;
            public List<Row> views=new List<Row>();
        }
        readonly Report proof=new Report();DemoScene scene;GameModel game;Enemy actor;SpriteRenderer view;string dest;bool oldBackground;
        double start;bool ent;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,"--root-forest-plant-v38");if(f<0)return;
            try{
                int o=Array.IndexOf(args,"--root-forest-plant-output-v38");Need(f+1<args.Length&&o>=0&&o+1<args.Length,"Plant check mode and destination");
                Need(args[f+1]=="ent"||args[f+1]=="fairy","Exact forest plant");
                var outDir=Path.GetFullPath(args[o+1]);var root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(outDir.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(outDir)&&
                    SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11,"Fresh ordinary D3D11 output");Directory.CreateDirectory(outDir);
                var self=new GameObject("Forest plant original pose check").AddComponent<ForestPlantPlayerCheckV38>();
                self.ent=args[f+1]=="ent";self.dest=outDir;self.proof.kind=self.ent?"anger-forest-ent":"anger-forest-fairy";
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        IEnumerator Start(){
            oldBackground=Application.runInBackground;Application.runInBackground=true;start=Time.realtimeSinceStartupAsDouble;
            Exception error=null;var enumerator=Run();
            while(true){bool more=false;object cur=null;try{
                Need(Time.realtimeSinceStartupAsDouble-start<360,"Total forest check deadline "+proof.last);
                more=enumerator.MoveNext();if(more)cur=enumerator.Current;
            }catch(Exception e){error=e;}if(error!=null||!more)break;yield return cur;}
            (enumerator as IDisposable)?.Dispose();proof.error=error==null?"":error.ToString();proof.passed=error==null;
            File.WriteAllText(Path.Combine(dest,"forest-plant-report.json"),JsonUtility.ToJson(proof,true));
            Application.runInBackground=oldBackground;Application.Quit(proof.passed?0:2);
        }
        IEnumerable Enter(){
            foreach(var t in EnterRoute(scene,"elder","dark-forest"))yield return t;
            game=(GameModel)Field(scene,"game");int room=ent?2:0;
            if(game.room!=room)Call(game,"EnterRoom",room);
            if(game.EncounterIndex!=0&&game.HasEncounters)Call(game,"BeginEncounter",0);
            actor=game.enemies.Find(e=>e.kind==proof.kind&&e.active&&!e.dormant);Need(actor!=null,"Real forest room birth "+proof.kind);
            for(int i=0;i<2;i++)yield return null;
            view=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
            game.player.maxHp=game.player.hp=5000;game.player.invincible=180;
        }
        void Place(float separation){
            int side=actor.x>game.CurrentRoom.width/2?-1:1;
            float x=Mathf.Clamp(actor.x+side*separation,35,game.CurrentRoom.width-35);
            var support=game.SurfaceBelow(x,Mathf.Max(game.player.y,actor.y)+45,true);
            Need(support!=null,"Original forest ground under controlled hero");
            game.player.x=x;game.player.y=support.Y(x);game.player.groundLine=support.id;game.player.vx=game.player.vy=0;
        }
        MagicianSkinFrameV38 Frame(){return scene.MagicianFrameV38(view);}
        bool Capture(string label,MagicianSkinFrameV38 f,bool optional=false){
            Need(f!=null&&ReferenceEquals(f.enemy,actor)&&f.selected&&ReferenceEquals(f.selected,view.sprite)&&
                f.page.StartsWith("Pixel/feedback-v38/"+proof.kind+"/",StringComparison.Ordinal),"Actual forest plant sprite "+label);
            var p=CaptureAtResolution((Camera)Field(scene,"worldCamera"),game,actor,new List<SpriteRenderer>{view},dest,label,640,360,!optional);
            if(optional&&p.count==0)return false;
            Need(p.count>0,"Visible forest world pixels "+label);
            proof.views.Add(new Row{label=label,alias=f.alias,sourceState=actor.npcState,motion=f.sourceAnimation,index=f.index,direction=f.facing,
                clock=f.clock,y=actor.y,hp=actor.hp,pixels=p});
            return true;
        }
        IEnumerator Run(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;
            for(int i=0;i<3;i++)yield return null;
            foreach(var t in Enter())yield return t;
            var startObserved=new bool[3];bool attack=false,hurt=false,hover=false,walk=false;double until=Time.realtimeSinceStartupAsDouble+125;
            while(!attack||(!proof.sourceStartCompleted&&ent)){
                Place(ent?88:64);yield return null;
                var f=Frame();proof.last="open/attack "+actor.npcState+" "+(f==null?"no skin":f.alias+"/"+f.clock);
                Need(Time.realtimeSinceStartupAsDouble<until,proof.last);
                if(f==null)continue;
                if(!ent&&!hover&&f.sourceAnimation=="Wait")hover=Capture("hover",f,true);
                if(!ent&&!walk&&f.sourceAnimation=="Walk")walk=Capture("walk",f,true);
                if(ent&&f.sourceAnimation=="Start"){
                    int phase=f.clock<1.5f?0:f.clock<4.1f?1:2;
                    if(!startObserved[phase]){Capture("enter-"+phase,f);startObserved[phase]=true;}
                }else if(ent&&startObserved[0]&&startObserved[1]&&startObserved[2])proof.sourceStartCompleted=true;
                if(!attack&&f.alias.StartsWith("source-attack",StringComparison.Ordinal)&&
                    !f.alias.Contains("ready")&&f.clock>.08f){Capture("attack",f);attack=true;}
            }
            proof.naturalAttack=true;until=Time.realtimeSinceStartupAsDouble+80;
            while(!hurt){
                Place(112);yield return null;
                var f=Frame();proof.last="hit "+actor.npcState+" "+(f==null?"no skin":f.alias);
                Need(Time.realtimeSinceStartupAsDouble<until,proof.last);
                if(f!=null&&f.alias=="source-hurt"){Capture("hurt",f);hurt=true;continue;}
                if(actor.state==CombatState.Normal&&actor.invincible<=0&&!game.EnemyHasNpcInvincibility(actor)&&!game.EnemyHasSuperArmor(actor))game.ApplyHitToEnemy(actor,1,HitReaction.Hurt);
            }
            proof.hit=true;var prior=actor;
            Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;
            Need(game.paused&&(string)Field(scene,"openMenu")=="dungeons","Ordinary map exit");proof.returned=true;
            foreach(var t in Enter())yield return t;Need(!ReferenceEquals(prior,actor),"Fresh forest unit");
            until=Time.realtimeSinceStartupAsDouble+35;
            while(Frame()==null){yield return null;Need(Time.realtimeSinceStartupAsDouble<until,"Reentry skin");}
            if(!ent){
                Place(74);yield return null;
                while(Frame()==null||!Capture("reentry",Frame(),true)){
                    Place(64);yield return null;Need(Time.realtimeSinceStartupAsDouble<until,"Visible forest fairy on reentry");
                }
            }
            proof.freshEntry=true;
        }
    }
}
#endif
