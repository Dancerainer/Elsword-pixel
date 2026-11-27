#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class FactoryRavenPlayerCheckV35:MonoBehaviour {
        const string Flag="--root-factory-raven-check-v35",Out="--root-factory-raven-output-v35";
        [Serializable] sealed class Report {
            public bool passed,ordinaryBirth,sourceStartVisible,releaseOnce,publicHit,publicLaunch,deathFade,returnedMain,cleanup,reentry;
            public string error,last;public int frames,completedPhases,originalProtos;public float releaseSeconds,earlyAlpha,midAlpha;
            public List<string> states=new List<string>(),errors=new List<string>();
            public List<GaitShot> naturalWalk=new List<GaitShot>();
            public bool originalVictoryHeld;public float victoryClock,victoryDeathTime,victoryElapsed;
            public List<string> phaseDamage=new List<string>();
            public List<PixelProof> shots=new List<PixelProof>();
        }
        Report report=new Report();DemoScene scene;GameModel game;Enemy boss;FactoryRavenIntroTicketV21 ticket;
        string output;bool done,oldBackground;double began;
        static bool CheckNaturalGait {get{return Array.IndexOf(Environment.GetCommandLineArgs(),"--root-factory-raven-gait-v35")>=0;}}
        [Serializable] sealed class GaitShot {
            public int direction,quarter,modelFacing,drawFacing,width,height;
            public float clock,bossX,heroX,projectedX,projectedY;
            public string originalState;public PixelProof whole;
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var a=Environment.GetCommandLineArgs();if(Array.IndexOf(a,Flag)<0)return;
            try{int i=Array.IndexOf(a,Out);Need(i>=0&&i+1<a.Length,"Fresh Raven report");
                var p=Path.GetFullPath(a[i+1]);var root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(p.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(p)&&!File.Exists(p),"Fresh owned Raven target");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"Actual D3D11");
                Directory.CreateDirectory(p);new GameObject("Explicit Factory Raven V35").AddComponent<FactoryRavenPlayerCheckV35>().output=p;
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        void Log(string m,string stack,LogType t){if(t==LogType.Error||t==LogType.Assert||t==LogType.Exception||m.Contains("FACTORY_RAVEN_PARTS_FALLBACK_V35"))report.errors.Add(m+" "+stack);}
        IEnumerator Start(){
            began=Time.realtimeSinceStartupAsDouble;oldBackground=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;
            var flow=Flow();Exception error=null;
            while(!done){
                bool more=false;object step=null;
                try{Need(report.errors.Count==0,"No standalone/render fallback errors");Need(Time.realtimeSinceStartupAsDouble-began<500,"Raven driver deadline "+report.last);
                    more=flow.MoveNext();if(more)step=flow.Current;
                }catch(Exception e){error=e;}
                if(error!=null||!more)break;
                report.frames++;yield return step;
            }
            (flow as IDisposable)?.Dispose();if(!done){report.error=error?.ToString();report.passed=error==null;Finish();}
        }
        void Update(){if(!done&&began>0&&Time.realtimeSinceStartupAsDouble-began>=500){report.error="Raven deadline "+report.last;Finish();}}
        void Finish(){if(done)return;done=true;Application.logMessageReceived-=Log;Application.runInBackground=oldBackground;
            try{File.WriteAllText(Path.Combine(output,"factory-raven-player-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}}
        Camera Cam{get{return (Camera)Field(scene,"worldCamera");}}
        FairyAuthoredPartsV33 Owner{get{return (FairyAuthoredPartsV33)Field(scene,"ravenPartsV35");}}
        void Record(){
            if(boss==null)return;string value=boss.npcState??("physics-"+boss.state);
            if(report.states.Count==0||report.states[report.states.Count-1]!=value)report.states.Add(value);
            report.last=value+" hp="+boss.hp+" time="+boss.npcTime+" view="+Owner?.LastFrame.alias+" pos="+boss.x+","+boss.y;
        }
        void Place(Enemy e,float side=1){
            float x=Mathf.Clamp(e.x+side*65,35,game.CurrentRoom.width-35);
            var floor=game.SurfaceBelow(x,Math.Max(e.y,game.player.y)+3,true);
            if(floor==null){x=e.x;floor=game.SurfaceBelow(x,Math.Max(e.y,game.player.y)+600,true);}
            Need(floor!=null,"Controlled player grounded on original Factory support");
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;
        }
        bool InView(SpriteRenderer r){
            if(!r||!r.enabled||!r.gameObject.activeInHierarchy||!r.sprite||r.color.a<=0)return false;
            var v=Cam.WorldToViewportPoint(r.bounds.center);
            return v.z>Cam.nearClipPlane&&v.x>.015f&&v.x<.985f&&v.y>.015f&&v.y<.985f;
        }
        bool CanSee(){
            var owner=Owner;if(owner==null||!owner.LastFrame.visible)return false;bool yes=false;
            owner.VisitRenderers(r=>{if(InView(r))yes=true;});return yes;
        }
        float Shot(string id){
            Need(CanSee(),"Original Raven in camera "+id);var rs=new List<SpriteRenderer>();Owner.VisitRenderers(rs.Add);
            Need(rs.Count==FactoryRavenRigV35.PartCount,"Full original Raven with independent boots");report.shots.Add(Capture(Cam,game,boss,rs,output,id));
            return rs[0].color.a;
        }
        void DrawTrueWorld() {
            // -batchmode has no presentation window swap. The real world camera's null-target
            // draw invokes the same draw witnesses and post-render consumer as ordinary play.
            Need(Cam && Cam.enabled && Cam.targetTexture==null,"Original world camera direct draw");Cam.Render();
        }
        IEnumerable Enter() {
            foreach(var step in EnterRoute(scene,"altera","altera-nasod-factory"))yield return step;
            double end=Time.realtimeSinceStartupAsDouble+110;
            for(;;){
                game=(GameModel)Field(scene,"game");var n=Field(scene,"nativeScene");
                if(scene.FactoryProductSessionActiveR1179&&game!=null&&!game.paused&&(string)Field(scene,"openMenu")==""&&
                    n!=null&&(string)Field(n,"id")!="WI_ALTERA_NASOD_FACTORY_BOSS")break;
                Need(Time.realtimeSinceStartupAsDouble<end,"Normal Factory admission and initial world");yield return null;
            }
            Call(game,"EnterRoom",2);
            if((int)Field(scene,"shownRoom")!=game.room)Call(scene,"RebuildRoom");
            if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            Need(game.room==2&&game.enemies.Count==1&&game.CurrentRoom.id=="Dungeon_Altera_Nasod_Factory_Normal-2","Original Factory boss room");
            boss=game.enemies[0];Need(boss.sourceNPC==FactoryRavenRigV35.Npc&&boss.kind=="raven-clone"&&boss.sourceKeyCode==0,"Original boss key0");
            game.player.hp=game.player.maxHp=5000;game.player.invincible=180;report.ordinaryBirth=true;
        }
        IEnumerable Opening(string label) {
            double end=Time.realtimeSinceStartupAsDouble+60;bool seen=false;
            while(true) {
                yield return null;Record();if(ticket==null)ticket=game.CurrentFactoryRavenIntroTicketV21();
                Need(ticket!=null&&ReferenceEquals(ticket.Actor,boss),"Original STOP ticket on the actual boss");
                if(Owner!=null && (int)Field(scene,"ravenSelectedFrameV35")==Time.frameCount)DrawTrueWorld();
                var status=game.FactoryRavenIntroStatusV21(ticket);
                Need(status!=null&&status.phase!=CoreStopIntroPhaseV21.Failed,"Original opening pending/success");
                if(!seen&&status.focusAccepted&&status.sourceTime>.15f&&Owner!=null&&Owner.LastFrame.alias=="WaitStart"&&CanSee()&&
                   (int)Field(scene,"ravenDrawMaskV35")==(1<<FactoryRavenRigV35.PartCount)-1){
                    Shot(label);seen=true;report.sourceStartVisible=true;
                }
                if(status.phase==CoreStopIntroPhaseV21.Released){
                    Need(seen&&status.focusAccepted&&status.releaseCount==1&&status.sourceTime>=3.79f,"Real camera draw releases original 3.8s START once");
                    report.releaseSeconds=status.sourceTime;report.releaseOnce=true;yield break;
                }
                Need(Time.realtimeSinceStartupAsDouble<end,"Original fourteen real draws and release "+report.last);
            }
        }
        IEnumerable WalkTowardHero(int side) {
            var seen=new bool[4];int got=0;
            double end=Time.realtimeSinceStartupAsDouble+38;
            while(got<4){
                // Original wait/walk AI starts its approach at 168. Keep the hero
                // on a legitimate floor in the same room, in sensing range.
                float x=Mathf.Clamp(boss.x+side*204,35,game.CurrentRoom.width-35);
                var floor=game.SurfaceBelow(x,boss.y+160,true);
                if(floor==null){x=Mathf.Clamp(boss.x+side*180,35,game.CurrentRoom.width-35);floor=game.SurfaceBelow(x,boss.y+400,true);}
                Need(floor!=null,"Existing factory floor for the approaching hero");
                game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;
                yield return null;Record();var owner=Owner;
                Need(Time.realtimeSinceStartupAsDouble<end,"Natural original walk direction "+side+" quarters "+got+" "+report.last);
                if(owner==null||!CanSee()||boss.npcState!="RAVEN_CLONE_WALK"||owner.LastFrame.alias!="Walk"||
                    Math.Sign(game.player.x-boss.x)!=side)continue;
                float clock=owner.LastFrame.sourceClock;int phase=Mathf.Clamp(Mathf.FloorToInt(clock*4/.8f),0,3);
                if(seen[phase])continue;
                var views=new List<SpriteRenderer>();owner.VisitRenderers(views.Add);
                Need(views.Count==FactoryRavenRigV35.PartCount,"Original registered Raven walk parts");
                int h=Mathf.RoundToInt(2*Cam.orthographicSize),w=Mathf.RoundToInt(h*Cam.aspect);
                var root=views[0].transform.parent;var record=new GaitShot{direction=side,quarter=phase,clock=clock,
                    originalState=boss.npcState,modelFacing=boss.facing,drawFacing=owner.LastFrame.facing,bossX=boss.x,heroX=game.player.x,
                    projectedX=root.position.x,projectedY=root.position.y,width=w,height=h};
                string name="raven-walk-native-"+(side<0?"hero-left-":"hero-right-")+phase;
                record.whole=CaptureAtResolution(Cam,game,boss,views,output,name,w,h);
                report.naturalWalk.Add(record);seen[phase]=true;got++;
            }
        }
        IEnumerable Visible(string alias,string name) {
            double end=Time.realtimeSinceStartupAsDouble+25;
            for(;;){Record();Place(boss);
                if(CanSee()&&(alias==null||Owner.LastFrame.alias==alias)){Shot(name);yield break;}
                Need(Time.realtimeSinceStartupAsDouble<end,"Original visible "+alias+" "+report.last);yield return null;}
        }
        IEnumerable Hit(Enemy actor,int damage,HitReaction reaction) {
            double end=Time.realtimeSinceStartupAsDouble+35;
            for(;;){
                Place(actor,-actor.facing);float hp=actor.hp;
                int amount=game.ApplyHitToEnemy(actor,damage,reaction);
                if(amount>0 && actor.hp<hp)yield break;
                Need(Time.realtimeSinceStartupAsDouble<end,"Original public damage window "+actor.sourceNPC+"/"+actor.npcState);
                yield return null;
            }
        }
        IEnumerable Phase(int number,float hpFraction){
            float threshold=boss.maxHp*(number==1?.70f:.30f);
            while(boss.hp>=threshold){
                // A landed launch can still be in the original half-damage down state.
                // The model compares actual remaining HP when deciding its own summon.
                float before=boss.hp;var state=boss.state;
                int requested=(int)Math.Ceiling(before-boss.maxHp*hpFraction);
                Need(requested>0&&before-requested>0,"Live original phase threshold "+number);
                foreach(var step in Hit(boss,requested,HitReaction.Hurt))yield return step;
                Need(boss.hp>0&&boss.hp<before,"Public nonlethal phase damage "+number);
                report.phaseDamage.Add(number+" "+state+" requested="+requested+" hp="+before+" -> "+boss.hp+" threshold="+threshold);
            }
            double end=Time.realtimeSinceStartupAsDouble+100;bool firstDraw=false;var originals=new HashSet<Enemy>();
            while(true){
                yield return null;Record();var s=game.ReadFactoryRavenClonePhaseV21();
                if(s.phaseCount<number){Place(boss);Need(Time.realtimeSinceStartupAsDouble<end,"Original natural summon decision "+number+" "+report.last);continue;}
                foreach(var e in game.enemies)if(e.sourceNPC=="NUI_RAVEN_CLONE_PROTO"&&e.hp>0)originals.Add(e);
                if(originals.Count==2) {
                    Enemy p=null;foreach(var e in originals)if(e.hp>0){p=e;break;}
                    if(p!=null){
                        Place(p);var body=((IList)Field(scene,"enemyViews"))[game.enemies.IndexOf(p)] as SpriteRenderer;
                        if(InView(body) && !firstDraw) {
                            Need(!body.forceRenderingOff && p.kind=="raven-clone-proto" && !scene.VisitFactoryRavenPartsV35(p,r=>{}),"Original white Proto owns its own renderer");
                            report.shots.Add(Capture(Cam,game,p,new List<SpriteRenderer>{body},output,"phase-"+number+"-original-proto"));
                            firstDraw=true;
                        }
                        if(firstDraw){foreach(var step in Hit(p,(int)Math.Ceiling(p.hp)+2,HitReaction.Hurt))yield return step;}
                    }
                }
                if(s.releaseCount>=number&&s.pending==0&&s.live==0 && boss.npcState!="RAVEN_CLONE_DISABLED" && game.Grounded(boss)){
                    Need(originals.Count==2&&firstDraw&&s.flag0&&(number==1||s.flag1),"Two authentic Proto births and original flag/release");
                    report.originalProtos+=originals.Count;report.completedPhases=number;break;
                }
                Need(Time.realtimeSinceStartupAsDouble<end,"Original clone deaths, release and landing "+number+" "+report.last);
            }
            foreach(var step in Visible(null,"phase-"+number+"-boss-recovered"))yield return step;
        }
        IEnumerable Death(){
            while(boss.hp>0){foreach(var s in Hit(boss,(int)Math.Ceiling(boss.hp)+2,HitReaction.Hurt))yield return s;}
            Need(boss.hp<=0&&boss.state==CombatState.Dead,"Public lethal enters actual Dead");
            double end=Time.realtimeSinceStartupAsDouble+15;
            for(;;){
                yield return null;Record();
                if(game.victory&&CanSee()&&Owner.LastFrame.alias=="DamageDownFront")break;
                Need(Time.realtimeSinceStartupAsDouble<end,"Original Factory victory and visible grounded death "+report.last);
            }
            report.earlyAlpha=Shot("06-grounded-death-at-victory");
            report.victoryClock=Owner.LastFrame.sourceClock;report.victoryDeathTime=boss.deathTime;report.victoryElapsed=game.elapsed;
            for(int i=0;i<12;i++)yield return null;
            Record();Need(game.victory&&CanSee()&&Owner.LastFrame.alias=="DamageDownFront"&&
                Owner.LastFrame.sourceClock==report.victoryClock&&game.elapsed==report.victoryElapsed&&boss.deathTime==report.victoryDeathTime,
                "Original Factory victory holds the model, actor and selected death pose");
            report.midAlpha=Shot("07-grounded-death-held");
            Need(report.earlyAlpha>0&&report.midAlpha==report.earlyAlpha,"Same drawable Raven on the live final victory frame");
            report.originalVictoryHeld=true;
        }
        IEnumerable ReturnMenu(){
            var old=new List<SpriteRenderer>();Owner?.VisitRenderers(old.Add);
            while(scene.BossIntroductionActive)yield return null;
            Press(scene,"dungeons");for(int i=0;i<6;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons"&&Field(scene,"ravenPartsV35")==null&&Field(scene,"ravenRigV35")==null,"Return to ordinary Main and release");
            foreach(var r in old)Need(!r,"Released Raven actor renderer");report.cleanup=report.returnedMain=true;
        }
        IEnumerator Flow() {
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;
            for(int i=0;i<3;i++)yield return null;
            if((string)Field(scene,"openMenu")!="dungeons"){Call(scene,"Action","dungeons");yield return null;}
            foreach(var s in Enter())yield return s;
            foreach(var s in Opening("01-source-opening"))yield return s;
            if(CheckNaturalGait) {
                int side=game.CurrentRoom.width-boss.x>=boss.x?1:-1;
                foreach(var s in WalkTowardHero(side))yield return s;
                foreach(var s in WalkTowardHero(-side))yield return s;
            }
            foreach(var s in Visible(null,"02-boss-live"))yield return s;
            foreach(var s in Hit(boss,1,HitReaction.Hurt))yield return s;
            yield return null;Record();foreach(var s in Visible(null,"03-public-hurt"))yield return s;report.publicHit=true;
            foreach(var s in Hit(boss,1,HitReaction.Launch))yield return s;
            yield return null;Record();foreach(var s in Visible("DamageAirFlyFront","04-public-launch"))yield return s;report.publicLaunch=true;
            double until=Time.realtimeSinceStartupAsDouble+25;
            while(!game.Grounded(boss)){yield return null;Need(Time.realtimeSinceStartupAsDouble<until,"Actual boss landed after launch");}
            foreach(var s in Phase(1,.64f))yield return s;foreach(var s in Phase(2,.25f))yield return s;
            foreach(var s in Death())yield return s;
            var first=boss;foreach(var s in ReturnMenu())yield return s;ticket=null;
            foreach(var s in Enter())yield return s;Need(!ReferenceEquals(first,boss),"Fresh original key0 boss");
            foreach(var s in Opening("08-reentered-original-opening"))yield return s;report.reentry=true;
            foreach(var s in ReturnMenu())yield return s;
        }
    }
}
#endif
