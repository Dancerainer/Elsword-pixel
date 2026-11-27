#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class GuardianSmallPlayerCheckV35:MonoBehaviour {
        const string Flag="--root-guardian-small-check-v35",Out="--root-guardian-small-output-v35";
        [Serializable] sealed class Row {
            public string label,sourceState,motion,poseId;public int frame,facing,n;
            public float npcClock,originalMotionClock,hp,alpha;public PixelProof pixels;
        }
        [Serializable] sealed class Hit {public string state;public int damage;public float at,sourceClock,hpBefore,hpAfter;}
        [Serializable] sealed class Audit {
            public string scope="Ordinary Plain room0 source birth, controlled player positions and public hits, existing world camera RT; no window presentation.",error,last;
            public bool passed,batchMode,entered,originalBirth,released,reentered,death;
            public List<string> errors=new List<string>();public List<Row> shots=new List<Row>();public List<Hit> hits=new List<Hit>();
        }
        readonly Audit audit=new Audit();DemoScene scene;GameModel game,main;Enemy actor;string dir;double started;bool done,bg;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var a=Environment.GetCommandLineArgs();if(Array.IndexOf(a,Flag)<0)return;
            try{
                int i=Array.IndexOf(a,Out);
                Need(Array.IndexOf(a,Flag)==Array.LastIndexOf(a,Flag)&&i>=0&&i==Array.LastIndexOf(a,Out)&&i+1<a.Length,"One Guardian flag/output pair");
                foreach(var arg in a)Need(arg==Flag||!arg.StartsWith("--root-")||!arg.Contains("-check-"),"Separate driving Player fixtures");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"Real Standalone D3D11");
                string path=Path.GetFullPath(a[i+1]),root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh owned report");
                Directory.CreateDirectory(path);new GameObject("Explicit Guardian Small source and GPU proof V35").AddComponent<GuardianSmallPlayerCheckV35>().dir=path;
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        void Log(string msg,string stack,LogType t){
            if(t==LogType.Exception||t==LogType.Error||t==LogType.Assert||msg.Contains("GUARDIAN_SMALL_ART_FALLBACK"))audit.errors.Add(msg+"\n"+stack);
        }
        void Finish(string error){
            if(done)return;done=true;audit.error=error;audit.passed=error==null&&audit.errors.Count==0;Application.logMessageReceived-=Log;int code=audit.passed?0:2;
            try{File.WriteAllText(Path.Combine(dir,"guardian-small-player-report.json"),JsonUtility.ToJson(audit,true));}catch(Exception e){Debug.LogException(e);code=2;}Application.Quit(code);
        }
        void Update(){if(!done&&started>0&&Time.realtimeSinceStartupAsDouble-started>=360)Finish("Wall deadline "+audit.last);}
        void OnDestroy(){Application.logMessageReceived-=Log;if(started>0)Application.runInBackground=bg;}
        IEnumerator Start(){
            started=Time.realtimeSinceStartupAsDouble;bg=Application.runInBackground;Application.runInBackground=true;
            audit.batchMode=Application.isBatchMode;Application.logMessageReceived+=Log;
            var flow=Flow();string error=null;
            while(!done){
                object next=null;bool more=false;try{Need(audit.errors.Count==0,"Runtime errors");more=flow.MoveNext();if(more)next=flow.Current;}
                catch(Exception e){error=e.ToString();}if(error!=null||!more)break;yield return next;
            }(flow as IDisposable)?.Dispose();if(!done)Finish(error);
        }
        IEnumerable Enter(){
            foreach(var x in EnterRoute(scene,"altera","altera-plain"))yield return x;
            while(!scene.PlainProductSessionActiveR1320)yield return null;
            game=(GameModel)Field(scene,"game");Need(game.room==0&&game.EncounterIndex==0&&game.enemies.Count>0,"Ordinary original Plain room0");
            actor=game.enemies[0];Need(actor.sourceNPC==GuardianSmallRigV35.Npc&&actor.kind==GuardianSmallRigV35.Kind,"Original room0 index0 Guardian");
            game.traceNpcActions=true;audit.entered=audit.originalBirth=true;Place(-36);
        }
        void Place(int offset){
            game.player.x=actor.x+offset;game.player.y=actor.y;game.player.groundLine=actor.groundLine;
            game.player.vx=game.player.vy=0;game.player.invincible=120;
        }
        object Entry(){var views=(IDictionary)Field(scene,"guardianViewsV35");return views.Contains(actor)?views[actor]:null;}
        bool Visible(){
            var row=Entry();if(row==null)return false;
            var p=(FairyAuthoredPartsV33)Field(row,"parts");if(!p.LastFrame.visible)return false;
            var views=new List<SpriteRenderer>();p.VisitRenderers(views.Add);var c=(Camera)Field(scene,"worldCamera");
            var v=c.WorldToViewportPoint(views[0].bounds.center);return v.z>0&&v.x>.01f&&v.x<.99f&&v.y>.01f&&v.y<.99f;
        }
        Row Observe(string label){
            var row=Entry();Need(row!=null,"Real Guardian owner "+label);
            var owner=(FairyAuthoredPartsV33)Field(row,"parts");var carrier=(SpriteRenderer)Field(row,"carrier");
            var views=new List<SpriteRenderer>();owner.VisitRenderers(views.Add);
            string poseId=(string)Field(row,"poseId");var sprites=(IDictionary)Field(scene,"plainVariableSpritesR1330");
            string key=GuardianSmallRigV35.Npc+"/"+poseId;
            Need(carrier&&carrier.forceRenderingOff&&carrier.enabled&&sprites.Contains(key)&&ReferenceEquals(carrier.sprite,sprites[key])&&views.Count==12,
                "Original exact variable-phase source carrier and registered parts "+label);
            foreach(var r in views)Need(r&&r.enabled&&r.gameObject.activeInHierarchy&&r.sprite&&Math.Abs(r.color.a-carrier.color.a)<.0001f,"Visible alpha "+label);
            var f=owner.LastFrame;Need(f.visible&&f.facing==(carrier.flipX?-1:1)&&views[6].sprite==views[9].sprite,"Original facing and duplicated tail segment");
            var result=new Row{label=label,sourceState=(string)Field(row,"state"),poseId=poseId,motion=f.alias,frame=Time.frameCount,facing=f.facing,n=views.Count,
                npcClock=actor.npcTime,originalMotionClock=f.sourceClock,hp=actor.hp,alpha=carrier.color.a};
            audit.shots.Add(result);result.pixels=Capture((Camera)Field(scene,"worldCamera"),game,actor,views,dir,audit.shots.Count.ToString("D3")+"-"+label);
            Need(result.pixels.count>0,"Exact Guardian pixel contribution");return result;
        }
        IEnumerable Wait(Func<bool> ok,string why,double seconds=40){
            double end=Time.realtimeSinceStartupAsDouble+seconds;
            while(true){
                yield return null;audit.last=why+" state="+actor.npcState+" clip="+actor.clip+" clock="+actor.npcTime+" hp="+actor.hp+
                    " owner="+(Entry()!=null)+" camera="+((Camera)Field(scene,"worldCamera")).transform.position+" actor="+actor.x+","+actor.y;
                if(Visible()&&ok())yield break;Need(Time.realtimeSinceStartupAsDouble<end,audit.last);
            }
        }
        static string AttackName(string state){
            switch(state){
                case "NASOD_GUARDIAN_SMALL_ATTACK":return "Attack";
                case "NASOD_GUARDIAN_SMALL_DASH_ATTACK":return "DashAttack";
                case "NASOD_GUARDIAN_SMALL_TAIL_ATTACK":return "TailAttack";
                default:return null;
            }
        }
        float Impact(string motion){return motion=="Attack"?1.002f:motion=="DashAttack"?.86f:.873f;}
        IEnumerable Actions(){
            var pictures=new HashSet<string>();var directions=new HashSet<int>();var hits=new HashSet<string>();
            double end=Time.realtimeSinceStartupAsDouble+140;float before=game.player.hp;
            while(true){
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Three original attacks/HP contact/both facings "+string.Join(",",pictures)+"/hits "+string.Join(",",hits));
                var action=AttackName(actor.npcState);
                if(Visible()){
                    var owner=(FairyAuthoredPartsV33)Field(Entry(),"parts");
                    if(directions.Add(owner.LastFrame.facing))Observe(owner.LastFrame.facing<0?"left":"right");
                    if(action!=null&&owner.LastFrame.sourceClock>=Impact(action)-.14f&&pictures.Add(action))Observe("attack-"+action);
                    if(game.player.hp<before)foreach(var hit in game.npcActionEvents){
                        string tag=AttackName(hit.state);
                        if(tag==null||hit.npc!=GuardianSmallRigV35.Npc||hit.type!="hit"||hit.damage<=0||
                            hit.time<game.elapsed-.12f||Math.Abs(hit.x-actor.x)>20||!hits.Add(tag))continue;
                        audit.hits.Add(new Hit{state=hit.state,damage=hit.damage,at=hit.time,sourceClock=hit.sourceTime,hpBefore=before,hpAfter=game.player.hp});
                    }
                }
                before=game.player.hp;if(pictures.Count==3&&hits.Count==3&&directions.Count==2)yield break;
                int dir=directions.Count==1?-actor.facing:actor.facing;int offset=action==null?dir*(pictures.Count==2&&!pictures.Contains("DashAttack")?64:32):actor.facing*30;
                Place(offset);if(action!=null&&actor.npcTime>Impact(action)-.24f&&!hits.Contains(action))game.player.invincible=0;
            }
        }
        bool Vulnerable(){return actor.hp>0&&actor.active&&!actor.dormant&&actor.state==CombatState.Normal&&
            !String.IsNullOrEmpty(actor.npcState)&&!game.EnemyHasSuperArmor(actor)&&!game.EnemyHasNpcInvincibility(actor)&&(bool)Call(game,"Grounded",actor);}
        void HitActor(int damage,HitReaction type){
            Place(-actor.facing*40);float hp=actor.hp;int result=game.ApplyHitToEnemy(actor,damage,type);
            Need(result>0&&actor.hp<hp,"Public source hit "+type);
        }
        IEnumerable Reactions(){
            foreach(var x in Wait(Vulnerable,"vulnerable"))yield return x;HitActor(1,HitReaction.Hurt);yield return null;
            Need(actor.state==CombatState.Hurt,"Original physical hurt");Observe("hurt");
            foreach(var x in Wait(Vulnerable,"before launcher"))yield return x;HitActor(1,HitReaction.Launch);
            var phases=new HashSet<string>();double end=Time.realtimeSinceStartupAsDouble+30;
            do{
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Launch recovery");
                string tag=actor.state==CombatState.Airborne?(actor.vy>0?"Launch":"Fall"):actor.state.ToString();
                if(actor.state!=CombatState.Normal&&phases.Add(tag))Observe("reaction-"+tag);
            }while(actor.state!=CombatState.Normal);
            foreach(var p in new[]{"Launch","Fall","Knockdown","GettingUp"})Need(phases.Contains(p),"Real reaction "+p);
            foreach(var x in Wait(Vulnerable,"before death"))yield return x;
            HitActor(Mathf.CeilToInt(actor.hp*2)+2,HitReaction.Hurt);Need(actor.hp<=0&&actor.state==CombatState.Dead,"Zero-HP death");
            yield return null;var first=Observe("death-first");
            Need(game.TryPlainGuardianLethalPoseV35(actor,out string source,out float time) &&
                source=="NASOD_GUARDIAN_SMALL_DYING_LAND_BACK" && first.motion=="DamageDownBack" &&
                Math.Abs(first.originalMotionClock-time)<.0001f,"Original recorded lethal rear packet, actual selected bank clock");
            foreach(var x in Wait(()=>actor.deathTime>=.55f,"death fade",12))yield return x;
            var later=Observe("death-mid");Need(later.alpha>0&&later.alpha<first.alpha,"Original death fade");audit.death=true;
        }
        IEnumerable Leave(){
            var row=Entry();var old=new List<SpriteRenderer>();if(row!=null)((FairyAuthoredPartsV33)Field(row,"parts")).VisitRenderers(old.Add);
            var bank=(FairyPoseLibraryV33)Field(scene,"guardianBankV35");var sprite=bank?.sharedSprites[0];var atlas=bank?.renderAtlas;
            Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;
            Need(!scene.PlainProductSessionActiveR1320&&ReferenceEquals(Field(scene,"game"),main)&&(string)Field(scene,"openMenu")=="dungeons"&&
                Field(scene,"guardianBankV35")==null&&!sprite&&!atlas,"Main restored and Guardian art released");
            foreach(var v in old)Need(!v,"Retired sprite renderer");audit.released=true;
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;for(int i=0;i<3;i++)yield return null;
            main=(GameModel)Field(scene,"game");Need(main.paused&&(string)Field(scene,"openMenu")=="dungeons","Ordinary Main start");
            foreach(var x in Enter())yield return x;foreach(var x in Wait(()=>true,"first display"))yield return x;Observe("original-birth");
            foreach(var x in Actions())yield return x;foreach(var x in Reactions())yield return x;
            var prior=actor;var priorGame=game;foreach(var x in Leave())yield return x;
            foreach(var x in Enter())yield return x;foreach(var x in Wait(()=>true,"fresh original birth"))yield return x;
            Need(!ReferenceEquals(actor,prior)&&!ReferenceEquals(game,priorGame),"Independent ordinary Plain component and original spawn");Observe("reentry");audit.reentered=true;
            foreach(var x in Leave())yield return x;
        }
    }
}
#endif
