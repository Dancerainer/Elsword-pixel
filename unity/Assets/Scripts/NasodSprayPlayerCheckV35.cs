#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class NasodSprayPlayerCheckV35:MonoBehaviour {
        const string Flag="--root-nasod-spray-check-v35",Out="--root-nasod-spray-output-v35";
        [Serializable] sealed class Shot {
            public string state,animation,sourcePose;public float clock,rotation;public int actorFacing,drawFacing;
            public PixelProof pixels;
        }
        [Serializable] sealed class Report {
            public bool passed,ordinaryBirth,naturalB,naturalA,publicLaunch,death,returned,reentry;
            public string mode,npc,roomId,error,last,projectile;
            public float groundedArgument,spawnX,spawnY,spawnZ;public int birthRoom,index,frame,serial;
            public PixelProof genuineProjectile;public int occludedActorSamples;
            public List<Shot> shots=new List<Shot>();public List<string> errors=new List<string>();
        }
        Report report=new Report();DemoScene scene;GameModel game;Enemy actor;double began;string output;
        bool finished,priorBackground;
        bool Ice{get{return report.mode=="core";}}
        static void Require(bool yes,string label){if(!yes)throw new InvalidOperationException("Nasod Spray Player: "+label);}
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var a=Environment.GetCommandLineArgs();int k=Array.IndexOf(a,Flag);if(k<0)return;
            try{
                int i=Array.IndexOf(a,Out);Require(k+1<a.Length && i>=0 && i+1<a.Length,"Output and route arguments");
                string id=a[k+1],dir=Path.GetFullPath(a[i+1]),root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Require((id=="plain"||id=="recycle"||id=="core")&&dir.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(dir)&&!File.Exists(dir),"Fresh original route");
                Require(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"Actual D3D11");
                Directory.CreateDirectory(dir);var c=new GameObject("Explicit Nasod Spray original route").AddComponent<NasodSprayPlayerCheckV35>();c.report.mode=id;c.output=dir;
            }catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
        }
        void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert||message.Contains("NASOD_SPRAY_ART_FALLBACK_V35"))report.errors.Add(message+" "+stack);}
        Camera Cam{get{return (Camera)Field(scene,"worldCamera");}}
        object Row(){return actor==null?null:((IDictionary)Field(scene,"nasodSprayViewsV35"))[actor];}
        void Check(){
            Require(report.errors.Count==0,"Original health "+(report.errors.Count==0?"":report.errors[0]));
            Require(Time.realtimeSinceStartupAsDouble-began<420,"Finite original route "+report.last);
            if(game!=null&&actor!=null)report.last=actor.npcState+" "+actor.npcTime+" hp="+actor.hp+" room="+game.room;
        }
        IEnumerator Start(){
            priorBackground=Application.runInBackground;Application.runInBackground=true;began=Time.realtimeSinceStartupAsDouble;Application.logMessageReceived+=Log;
            var flow=Flow();Exception failure=null;
            for(;;){
                bool more=false;object next=null;
                try {
                    Check();
                    if(scene && Ice && scene.CoreHardProductSessionActiveR1147 && scene.CoreHardProductAuthoredCameraActiveR1147 && Cam && !Cam.targetTexture)Cam.Render();
                    more=flow.MoveNext();if(more)next=flow.Current;
                }catch(Exception ex){failure=ex;}
                if(failure!=null||!more)break;report.frame++;yield return next;
            }
            (flow as IDisposable)?.Dispose();report.error=failure?.ToString();report.passed=failure==null&&report.errors.Count==0;Complete();
        }
        void Complete(){
            if(finished)return;finished=true;Application.logMessageReceived-=Log;Application.runInBackground=priorBackground;
            try{File.WriteAllText(Path.Combine(output,"nasod-spray-player-report.json"),JsonUtility.ToJson(report,true));}
            finally{Application.Quit(report.passed?0:2);}
        }
        IEnumerable Enter(){
            string route=Ice?"altera-core-hard":report.mode=="plain"?"altera-plain":"altera-plain-recycle";
            foreach(var x in EnterRoute(scene,"altera",route))yield return x;
            double end=Time.realtimeSinceStartupAsDouble+115;
            while(!(Ice?scene.CoreHardProductSessionActiveR1147:scene.PlainProductSessionActiveR1320)||
                (game=(GameModel)Field(scene,"game"))==null||game.paused||(string)Field(scene,"openMenu")!=""){
                Require(Time.realtimeSinceStartupAsDouble<end,"Real map admission "+route);yield return null;
            }
            report.birthRoom=Ice?2:report.mode=="plain"?0:1;report.index=Ice?7:report.mode=="plain"?9:13;
            if(game.room!=report.birthRoom){Call(game,"EnterRoom",report.birthRoom);if((int)Field(scene,"shownRoom")!=game.room)Call(scene,"RebuildRoom");}
            if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            actor=game.enemies[report.index];report.roomId=game.CurrentRoom.id;report.npc=Ice?NasodSprayRigV35.Ice:NasodSprayRigV35.Plain;
            Require(actor.sourceNPC==report.npc&&actor.encounterIndex==0&&actor.active&&!actor.dormant,"Original Spray birth "+route);
            game.player.hp=game.player.maxHp=100000;game.player.invincible=50;report.ordinaryBirth=true;
        }
        void Place(float distance){
            var encounter=game.CurrentEncounter;
            float left=encounter==null?35:encounter.leftX+.03f;
            float right=encounter==null?game.CurrentRoom.width-35:encounter.rightX-.03f;
            int side=report.mode=="recycle"?(right-actor.x>=actor.x-left?1:-1):
                (game.CurrentRoom.width-actor.x>actor.x?1:-1);
            float x=actor.x+side*distance;
            if(x<left||x>right)x=actor.x-side*distance;
            Require(x>=left&&x<=right,"Requested player distance fits the active original encounter");
            var floor=game.SurfaceBelow(x,actor.y+14,true)??game.SurfaceBelow(x,actor.y+170,true);
            if(floor==null){
                x=2*actor.x-x;
                Require(x>=left&&x<=right,"Opposite source platform lies in the active encounter");
                floor=game.SurfaceBelow(x,actor.y+14,true)??game.SurfaceBelow(x,actor.y+170,true);
            }
            Require(floor!=null,"Existing original surface");game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;
            game.player.vx=game.player.vy=0;game.player.invincible=50;
        }
        bool View(string alias=null){
            var r=Row();if(r==null||(int)Field(r,"frame")!=Time.frameCount)return false;
            var owner=Field(r,"parts") as FairyAuthoredPartsV33;var c=Field(r,"carrier") as SpriteRenderer;
            if(owner==null||!c||!c.enabled||!c.forceRenderingOff||!owner.LastFrame.visible||alias!=null&&owner.LastFrame.alias!=alias)return false;
            if(Math.Abs(owner.LastFrame.sourceClock-(float)Field(r,"partClock"))>.00001f)return false;
            bool seen=false;owner.VisitRenderers(s=>{var at=Cam.WorldToViewportPoint(s.bounds.center);seen|=s.enabled&&s.gameObject.activeInHierarchy&&at.z>Cam.nearClipPlane&&at.x>.03f&&at.x<.97f&&at.y>.03f&&at.y<.97f;});return seen;
        }
        Shot Capture(string id){
            var row=Row();Require(row!=null&&View(),"Actual selected live Spray "+id);var owner=(FairyAuthoredPartsV33)Field(row,"parts");var rs=new List<SpriteRenderer>();owner.VisitRenderers(rs.Add);
            Require(rs.Count==NasodSprayRigV35.PartCount&&rs[11].name=="nozzle"&&rs[13].name=="left-boot"&&rs[14].name=="right-boot","Original isolated registration");
            var shot=new Shot{state=(string)Field(row,"state"),animation=owner.LastFrame.alias,sourcePose=(string)Field(row,"pose"),clock=(float)Field(row,"sourceClock"),
                actorFacing=actor.facing,drawFacing=owner.LastFrame.facing,rotation=Mathf.DeltaAngle(0,rs[0].transform.parent.eulerAngles.z)};
            Require(shot.drawFacing==(Ice?1:actor.facing),"Original fixed Core / mirrored Plain presentation");
            int h=Mathf.RoundToInt(2*Cam.orthographicSize),w=Mathf.RoundToInt(h*Cam.aspect);
            shot.pixels=CaptureAtResolution(Cam,game,actor,rs,output,id,w,h);report.shots.Add(shot);return shot;
        }
        bool TryCaptureVisible(string id,out Shot shot){
            try {shot=Capture(id);return true;}
            catch(InvalidOperationException ex) when(ex.Message.StartsWith("Rig Player V35: Visible actual GPU pixels ",StringComparison.Ordinal)){
                shot=null;report.occludedActorSamples++;return false;
            }
        }
        IEnumerable Wait(string alias,float distance,string id){
            double end=Time.realtimeSinceStartupAsDouble+60;
            for(;;){Place(distance);yield return null;Check();if(View(alias)&&TryCaptureVisible(id,out _))yield break;
                Require(Time.realtimeSinceStartupAsDouble<end,"Live original "+alias+" "+report.last);}
        }
        IEnumerable FireB(){
            double end=Time.realtimeSinceStartupAsDouble+65;bool drawn=false,emitted=false;string effect=Ice?GameModel.IceSprayShot:GameModel.SprayShot;
            while(!drawn||!emitted){
                // The Recycle birth stands beside a drop and the end of the
                // encounter. Stay on the side with usable ground. 147 lies in
                // its genuine B range (>140) before Walk pursuit begins (>168).
                Place(report.mode=="recycle"?147:182);yield return null;Check();
                foreach(var p in game.npcEffects)if(p.owner==actor&&p.id==effect&&p.hasSourceLandY){
                    var mesh=scene.NasodSprayBallRendererV35(p);if(!mesh||emitted)continue;
                    var position=Cam.WorldToViewportPoint(mesh.bounds.center);
                    if(position.z<=0||position.x<.03f||position.x>.97f||position.y<.03f||position.y>.97f)continue;
                    int h=Mathf.RoundToInt(2*Cam.orthographicSize),w=Mathf.RoundToInt(h*Cam.aspect);
                    try{report.genuineProjectile=CaptureRenderersAtResolution(Cam,game,actor,new List<MeshRenderer>{mesh},output,"03-real-B-projectile",w,h);}
                    catch(InvalidOperationException ex){if(ex.Message.Contains("Visible actual GPU pixels"))continue;throw;}
                    emitted=true;report.serial=p.serial;report.projectile=p.id;report.groundedArgument=p.sourceLandY;report.spawnX=p.x;report.spawnY=p.y;report.spawnZ=p.z;
                }
                if(!drawn&&View("AttackB_Fire")&&actor.npcTime>=.015f&&TryCaptureVisible("03-original-nozzle-B",out _))drawn=true;
                Require(Time.realtimeSinceStartupAsDouble<end,"Original far READY and Bone_Rhand B projectile "+report.last);
            }
            report.naturalB=true;
        }
        IEnumerable Hit(HitReaction react,bool lethal=false){
            double end=Time.realtimeSinceStartupAsDouble+50;
            for(;;){
                Place(55);yield return null;Check();
                if(actor.invincible<=0&&!game.EnemyHasNpcInvincibility(actor)&&!game.EnemyHasSuperArmor(actor)&&game.Grounded(actor)){
                    int dealt=game.ApplyHitToEnemy(actor,lethal?(int)Math.Ceiling(actor.hp)+2:1,react);
                    if(dealt>0&&(!lethal||actor.hp<=0))yield break;
                }
                Require(Time.realtimeSinceStartupAsDouble<end,"Actual public reaction "+report.last);
            }
        }
        IEnumerable Return(){
            while(scene.BossIntroductionActive)yield return null;Press(scene,"dungeons");for(int i=0;i<7;i++)yield return null;
            Require((string)Field(scene,"openMenu")=="dungeons"&&((IDictionary)Field(scene,"nasodSprayViewsV35")).Count==0&&
                ((IDictionary)Field(scene,"nasodSprayRigsV35")).Count==0,"Released physical Spray owners and banks");report.returned=true;
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;for(int i=0;i<3;i++)yield return null;
            if((string)Field(scene,"openMenu")!="dungeons"){Call(scene,"Action","dungeons");yield return null;}
            foreach(var s in Enter())yield return s;
            foreach(var s in Wait("Wait",115,"01-own-neutral"))yield return s;
            foreach(var s in Wait("Walk",218,"02-own-walk"))yield return s;
            foreach(var s in FireB())yield return s;
            foreach(var s in Wait("AttackA_Fire",63,"04-close-spray"))yield return s;report.naturalA=true;
            double end=Time.realtimeSinceStartupAsDouble+90;
            while(!report.publicLaunch){
                foreach(var s in Hit(HitReaction.Launch))yield return s;
                do{
                    yield return null;Check();
                    if((View("DamageAirFlyFront")||View("DamageAirFlyBack"))&&TryCaptureVisible("05-public-launch",out var rise)){
                        if(rise.animation=="DamageAirFlyFront")Require(Mathf.Abs(rise.rotation+45)<.02f,"Exact source ADD_ROTATE_Z -45");
                        report.publicLaunch=true;break;
                    }
                }while(!game.Grounded(actor));
                Require(Time.realtimeSinceStartupAsDouble<end,"Actually visible original public launch "+report.last);
            }
            foreach(var s in Hit(HitReaction.Hurt,true))yield return s;Require(actor.hp<=0&&actor.state==CombatState.Dead,"Public lethal death");report.death=true;
            var first=actor;foreach(var s in Return())yield return s;foreach(var s in Enter())yield return s;
            Require(first!=actor,"Fresh original birth");foreach(var s in Wait("Wait",115,"06-reentry"))yield return s;report.reentry=true;
            foreach(var s in Return())yield return s;
        }
    }
}
#endif
