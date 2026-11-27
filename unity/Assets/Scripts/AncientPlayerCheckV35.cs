#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed class AncientPlayerCheckV35:MonoBehaviour {
        const string Flag="--root-ancient-check-v35",OutputFlag="--root-ancient-output-v35";
        const double Budget=420;
        [Serializable] public sealed class Sample {
            public string label,alias,state,clip;
            public int facing,frameIndex; public string sourceAnimation,page;
            public float sourceClock,hostClock,npcTime,hp,alpha,bossY,playerY,barGap;
            public PixelProof view;
        }
        [Serializable] public sealed class Emission {
            public int serial,damage;public float born,expected,observedRelativeTime,hitTime,hitAge;
        }
        [Serializable] public sealed class Attack {
            public string kind,readyState,fireState,effect;public float hpRatioAtReady,playerHpBefore,playerHpAfter;
            public bool sourceAi,positivePlayerDamage,originalBurnApplied;public List<Emission> emissions=new List<Emission>();
        }
        [Serializable] public sealed class Report {
            public string error="",last="",gpu,scope="Ordinary Ruben Swamp entry; controlled original room3/encounter and player placement/public hits. Source model alone advances.";
            public bool passed,ordinaryMenu,originalBirth,noKnockdown,publicLaunch,deathFade,returnedMain,reentered,cleanup;
            public int sourceSpawnIndex=9,actorIndex=0,currentSourceProfileStates=8,originalDeclaredStates=25;
            public float expectedRatio=3.1045065684f,actualRatio,bossHpAtBirth;
            public string sourceBoundary="Normal swamp verified separately from Henir. The selected NA string table has no original caption text.";
            public bool hiddenReady,invincibleReady,sourceStart,cameraRestored;
            public List<PixelProof> movieFrames=new List<PixelProof>();
            public List<int> captionIds=new List<int>();
            public PixelProof heroRatio;
            public bool bothFacingsWalk,authoredIntroductionPortrait;
            public List<string> errors=new List<string>(),steps=new List<string>();public List<Sample> samples=new List<Sample>();public List<Attack> attacks=new List<Attack>();
        }
        readonly Report report=new Report();
        DemoScene scene;GameModel game;Enemy boss;string output;
        double started;bool done,originalBackground,traceSet,originalTrace;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            var a=Environment.GetCommandLineArgs();int f=Array.IndexOf(a,Flag);if(f<0)return;
            try {
                int o=Array.IndexOf(a,OutputFlag);
                Need(f==Array.LastIndexOf(a,Flag)&&o>=0&&o==Array.LastIndexOf(a,OutputFlag)&&o+1<a.Length,"One Ancient check/output pair");
                foreach(string arg in a)Need(arg!="--root-bat-a-check-v34"&&arg!="--root-a1-chicken-check-v32"&&arg!="--root-core-reaction-check-v32"&&arg!="--root-factory-sheriff-check-v32","One driving observer at a time");
                Need(!Application.isEditor&&SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11,"Standalone D3D11");
                string path=Path.GetFullPath(a[o+1]);string allow=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allow,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh runtime-drafts output");
                Directory.CreateDirectory(path);new GameObject("Explicit original Ancient boss check").AddComponent<AncientPlayerCheckV35>().output=path;
            } catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        void Log(string m,string stack,LogType type) {
            if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert||(m.Contains("ANCIENT_AUTHORED_FALLBACK")||m.Contains("ANCIENT_SKIN_FALLBACK")))report.errors.Add(m+"\n"+stack);
        }
        void Check(){Need(report.errors.Count==0,"Runtime error or authored fallback");Need(Time.realtimeSinceStartupAsDouble-started<Budget,"Finite observer deadline");}
        IEnumerator Start(){
            started=Time.realtimeSinceStartupAsDouble;originalBackground=Application.runInBackground;Application.runInBackground=true;
            report.gpu=SystemInfo.graphicsDeviceType.ToString();Application.logMessageReceived+=Log;
            var run=Flow();Exception error=null;
            while(!done){bool more=false;object value=null;try{Check();more=run.MoveNext();if(more)value=run.Current;}catch(Exception e){error=e;}if(error!=null||!more)break;yield return value;}
            (run as IDisposable)?.Dispose();if(done)yield break;
            report.error=error==null?"":error.ToString();report.passed=error==null&&report.errors.Count==0;Finish();
        }
        void Update(){if(!done&&started>0&&Time.realtimeSinceStartupAsDouble-started>=Budget){report.error="Total deadline "+report.last;Finish();}}
        void Finish(){if(done)return;done=true;Application.logMessageReceived-=Log;if(traceSet&&game!=null)game.traceNpcEffects=originalTrace;Application.runInBackground=originalBackground;
            try{File.WriteAllText(Path.Combine(output,"ancient-player-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}}
        void OnDestroy(){Application.logMessageReceived-=Log;}
        void Place(float x,bool record=true) {
            x=Mathf.Clamp(x,35,game.CurrentRoom.width-35);
            var floor=game.SurfaceBelow(x,Math.Max(boss.y,game.player.y)+2,true);Need(floor!=null,"Real original player support");
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;
            if(record)report.steps.Add("Controlled player on source floor "+floor.id+" at x "+x);
        }
        int OpenSide(float amount) { return boss.x+amount<=game.CurrentRoom.width-35 && (boss.x-amount<35 || boss.x<game.CurrentRoom.width*.5f)?1:-1; }
        IEnumerable Enter() {
            foreach(var t in EnterRoute(scene,"ruben","swamp"))yield return t;
            game=(GameModel)Field(scene,"game");Need(!game.paused&&game.CurrentRoom.id=="swamp-1","Ordinary main Swamp route");report.ordinaryMenu=true;
            Call(game,"EnterRoom",2);if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            var stage=Array.Find(((ArtCatalog)Field(scene,"catalog")).stages,s=>s.id==game.CurrentRoom.id);
            Need(game.room==2&&game.CurrentRoom.id=="swamp-3"&&stage!=null&&stage.spawns[0].sourceSpawnIndex==9&&stage.spawns[0].sourceNPC==AncientRigV35.Npc,"Original room3 birth index9");
            boss=game.enemies[0];Need(boss.sourceNPC==AncientRigV35.Npc&&boss.kind==AncientRigV35.Kind&&boss.encounterIndex==0,"Original boss");
            var profile=game.Remaining3SourceProfileDefinitionV28(boss);Need(profile!=null&&profile.states.Length==8&&string.IsNullOrEmpty(profile.initialState),"Actual admitted boss profile");
            report.originalBirth=true;report.bossHpAtBirth=boss.hp;game.player.hp=game.player.maxHp=5000;game.player.invincible=180;
            originalTrace=game.traceNpcEffects;traceSet=true;game.traceNpcEffects=true;
            Place(boss.x+OpenSide(157)*157);for(int i=0;i<2;i++)yield return null;
            Need(game.SwampIntroActorV39==boss&&game.SwampActorHiddenV39(boss)&&!boss.aiEnabled&&boss.npcState=="ANCIENT_PPORU_START_READY","Original hidden READY birth");
            Need(game.EnemyHasNpcInvincibility(boss)&&game.ApplyHitToEnemy(boss,20,HitReaction.Hurt,0)==0&&boss.hp==report.bossHpAtBirth,"No contact during the director scene");
            report.hiddenReady=report.invincibleReady=true;
            int phase=0;float[] at={1,5.5f,8.6f};
            while(game.SwampActorHiddenV39(boss)){
                yield return null;Check();
                float age=game.SwampIntroAgeV39;
                if(!game.SwampActorHiddenV39(boss))break;
                Need(scene.SwampMovieCameraV39&&scene.SwampMovieMeshCountV39==8&&Owner()==null,"Eight original director meshes and exclusive camera during READY");
                int caption=scene.SwampMovieCaptionIdV39;
                if(caption!=0&&!report.captionIds.Contains(caption))report.captionIds.Add(caption);
                if(phase<at.Length&&age>=at[phase]){
                    var camera=(Camera)Field(scene,"nativeCamera");
                    report.movieFrames.Add(CaptureRenderersAtResolution(camera,game,boss,scene.SwampVisibleMovieMeshesV39(),output,"intro-"+(report.movieFrames.Count+1),640,360));
                    phase++;
                }
                if(age>=10&&scene.BossIntroductionActive){
                    var portrait=(UnityEngine.UI.Image)Field(scene,"bossIntroductionPortrait");
                    Need(portrait&&portrait.sprite&&portrait.sprite.name=="ancient-phoru/v37/source-idle/0-intro","Delayed name and matching authored portrait");
                    report.authoredIntroductionPortrait=true;
                }
            }
            Need(phase==3&&report.captionIds.Count==3&&report.authoredIntroductionPortrait,"Original movie timings and visible shots");
            Need(boss.aiEnabled&&boss.npcState=="ANCIENT_PPORU_START"&&game.SwampIntroAgeV39>=12.49f,"Director motion end releases the original START");
            report.sourceStart=true;
            for(int i=0;i<2;i++)yield return null;
            Need(!scene.SwampMovieCameraV39&&!game.SwampActorHiddenV39(boss),"Gameplay camera and boss visibility restored");
            report.cameraRestored=true;
        }
        AncientSkinPresenterV37 Owner() {
            return ReferenceEquals(Field(scene,"ancientSkinOwnerV37"),boss)
                ? Field(scene,"ancientSkinV37") as AncientSkinPresenterV37 : null;
        }
        Sample Observe(string label,string alias) {
            var owner=Owner();
            Need(owner!=null && owner.Alias==alias && owner.Visible && owner.Carrier && owner.Carrier.forceRenderingOff,
                "Visible original-pose redraw selected for "+label);
            var carrier=owner.Carrier;var skin=owner.Renderer;
            Need(skin && skin.enabled && skin.gameObject.activeInHierarchy && skin.sprite &&
                ReferenceEquals(skin.sprite,owner.SelectedSprite) && owner.PageResource.StartsWith(AncientSkinPresenterV37.Root,StringComparison.Ordinal) &&
                owner.FrameIndex>=0 && owner.Facing==boss.facing && Mathf.Abs(skin.color.a-carrier.color.a)<.0001f,
                "Actual authored page, frame, facing and source alpha "+label);
            var sample=new Sample{label=label,alias=alias,sourceAnimation=owner.SourceAnimation,frameIndex=owner.FrameIndex,
                page=owner.PageResource,state=boss.npcState,clip=boss.clip,facing=boss.facing,sourceClock=owner.SourceClock,
                hostClock=boss.clock,npcTime=boss.npcTime,hp=boss.hp,alpha=carrier.color.a,bossY=boss.y,playerY=game.player.y};
            var camera=(Camera)Field(scene,"worldCamera");
            var views=new List<SpriteRenderer>{skin};
            sample.view=Capture(camera,game,boss,views,output,label);
            Need(sample.view.count>0,"Authored sprite changes the actual world image "+label);
            var bars=(IList)Field(scene,"enemyBars");var bar=(SpriteRenderer)bars[0];
            if(boss.hp>0&&bar.enabled){sample.barGap=bar.transform.position.y-owner.VisibleTop;
                Need(Math.Abs(sample.barGap-8)<.05f,"Visible boss health bar follows the new sprite "+label);}
            report.samples.Add(sample);return sample;
        }
        IEnumerable Wait(string alias,float min,string label,double seconds=30,Action hold=null) {
            double until=Time.realtimeSinceStartupAsDouble+seconds;
            while(true){if(hold!=null)hold();yield return null;Check();var owner=Owner();var current=owner==null?"":owner.Alias;
                report.last=label+" expected "+alias+" actual="+current+" state="+boss.npcState+" hp="+boss.hp+" npcTime="+boss.npcTime+" actorXY="+boss.x+","+boss.y+" playerXY="+game.player.x+","+game.player.y;
                Need(Time.realtimeSinceStartupAsDouble<until,"Wait "+report.last);
                if(current==alias&&owner.SourceClock>=min){if(label!=null)Observe(label,alias);yield break;}}
        }
        IEnumerable Vulnerable() {
            double until=Time.realtimeSinceStartupAsDouble+45;
            while(boss.state!=CombatState.Normal||boss.invincible>0||game.EnemyHasNpcInvincibility(boss)||game.EnemyHasSuperArmor(boss)) {
                yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Original vulnerable interval "+boss.npcState);
            }
        }
        int Hit(int damage,HitReaction reaction) {
            float hp=boss.hp;int actual=game.ApplyHitToEnemy(boss,damage,reaction,280);
            report.steps.Add("Public "+reaction+" requested="+damage+" dealt="+actual+" HP "+hp+" -> "+boss.hp);
            Need(actual>0&&boss.hp<hp,"Public hit applied");return actual;
        }
        IEnumerable Lower(float fraction) {
            for(int i=0;i<4&&boss.hp>boss.maxHp*fraction;i++) {
                foreach(var t in Vulnerable())yield return t;
                int amount=Math.Max(1,(int)Math.Ceiling(boss.hp-boss.maxHp*fraction));
                Hit(amount,HitReaction.Hurt);yield return null;
            }
            Need(boss.hp>0&&boss.hp/boss.maxHp<=fraction+.005f,"Source AI HP threshold reached through public hits");
        }
        IEnumerable Cycle(string tag,string ready,string fire,string effect,float range,float[] expected,double readyLimit) {
            var row=new Attack{kind=tag,readyState="ANCIENT_PPORU_"+(tag=="C"?"SPECIAL_ATTACK":("ATTACK_"+tag))+"_READY",
                fireState="ANCIENT_PPORU_"+(tag=="C"?"SPECIAL_ATTACK":("ATTACK_"+tag)),effect=effect};
            report.attacks.Add(row);int side=OpenSide(range);Place(boss.x+side*range);
            foreach(var t in Wait(ready,.04f,tag+"-ready",readyLimit,()=>Place(boss.x+OpenSide(range)*range,false)))yield return t;
            Need(boss.npcState==row.readyState,"Natural source readiness "+tag);row.sourceAi=true;row.hpRatioAtReady=boss.hp/boss.maxHp;
            int facing=boss.facing;game.player.invincible=0;row.playerHpBefore=game.player.hp;
            if(tag!="C")Place(boss.x+facing*(tag=="B"?231:84));
            int burnAtStart=game.npcPlayerStatusEvents.Count;
            long sequence=0;foreach(var ev in game.npcPresentationEvents)sequence=Math.Max(sequence,ev.sequence);
            var known=new Dictionary<int,Emission>();float start=0;bool entered=false,fireImage=false;double until=Time.realtimeSinceStartupAsDouble+35;
            while(true) {
                yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Source attack events "+tag+" "+report.last);
                var owner=Owner();
                if(boss.npcState==row.fireState) {
                    if(!entered){entered=true;start=game.elapsed-boss.npcTime;}
                    if(!fireImage&&owner!=null&&owner.Alias==fire){Observe(tag+"-fire",fire);fireImage=true;}
                }
                foreach(var ev in game.npcPresentationEvents) {
                    if(ev.sequence<=sequence || ev.type!="spawn" || ev.effect!=effect || !ReferenceEquals(ev.owner,boss) || known.ContainsKey(ev.serial))continue;
                    Need(row.emissions.Count<expected.Length,"No duplicate authored effect "+tag);
                    var hit=new Emission{serial=ev.serial,born=ev.time,expected=expected[row.emissions.Count],observedRelativeTime=ev.time-start};
                    known.Add(ev.serial,hit);row.emissions.Add(hit);
                }
                foreach(var ev in game.npcEffectEvents) {
                    if(ev.type!="hit-player"||ev.damage<=0||!known.TryGetValue(ev.serial,out var hit))continue;
                    hit.damage=ev.damage;hit.hitTime=ev.time;hit.hitAge=ev.time-hit.born;
                }
                foreach(var hit in row.emissions)if(hit.damage>0 && game.player.hp<row.playerHpBefore)row.positivePlayerDamage=true;
                // A controlled player can stand on the contact plane of the actual
                // emitted effect. Its original owner, geometry and damage remain in GameModel.
                if(!row.positivePlayerDamage)foreach(var live in game.npcEffects)if(live.id==effect&&ReferenceEquals(live.owner,boss)&&!live.ended)
                {
                    if(tag!="C"){Place(live.x,false);continue;}
                    // The special's original attack segment starts well past its emitter.
                    // Place only the observer's player inside the model's actual world-space line.
                    float[] line=game.NpcEffectLines(live);
                    Need(line.Length==6,"Original Ancient breath segment");
                    float lo=Mathf.Max(35,Mathf.Min(line[0],line[3]));
                    float hi=Mathf.Min(game.CurrentRoom.width-35,Mathf.Max(line[0],line[3]));
                    if(lo>hi)continue;
                    float aim=(lo+hi)*.5f;
                    float pointY=Mathf.Lerp(line[1],line[4],Mathf.InverseLerp(line[0],line[3],aim));
                    Place(aim,false);
                    if(pointY>game.player.y+42){game.player.y=pointY-21;game.player.groundLine=-1;}
                }
                report.last="cycle "+tag+" state="+boss.npcState+" emissions="+row.emissions.Count+" hit="+row.positivePlayerDamage;
                if(entered&&boss.npcState!=row.fireState)break;
            }
            row.playerHpAfter=game.player.hp;
            Need(fireImage&&row.emissions.Count==expected.Length&&row.positivePlayerDamage,"Actual "+tag+" draw, emissions and positive HP contact");
            if(tag=="C"){
                for(int i=burnAtStart;i<game.npcPlayerStatusEvents.Count;i++){
                    var status=game.npcPlayerStatusEvents[i];
                    if(status.type=="fire-start"||status.type=="fire-refresh")row.originalBurnApplied=true;
                }
                Need(row.originalBurnApplied,"Original Ancient breath burn on actual damaging line contact");
            }
            foreach(var hit in row.emissions)Need(Math.Abs(hit.observedRelativeTime-hit.expected)<.065f,"Source event time "+tag);
            game.player.invincible=180;
        }
        IEnumerable ReturnMenu() {
            var current=Owner();var oldView=current==null?null:current.Renderer;
            var oldCarrier=current==null?null:current.Carrier;
            Press(scene,"dungeons");
            for(int i=0;i<12;i++){
                yield return null;
                if((string)Field(scene,"openMenu")=="dungeons" && game.paused &&
                   Field(scene,"ancientSkinV37")==null && !oldView) break;
            }
            Need((string)Field(scene,"openMenu")=="dungeons" && game.paused &&
                Field(scene,"ancientSkinV37")==null && Field(scene,"ancientSkinOwnerV37")==null &&
                !oldView && (!oldCarrier||!oldCarrier.forceRenderingOff),
                "Ordinary Main and disposed redraw renderer, original carrier restored");
            report.returnedMain=report.cleanup=true;
        }
        IEnumerable WalkBothWays() {
            const int quarters=4;
            foreach(int facing in new[]{-1,1}) {
                var phases=new bool[quarters];int found=0;float startX=boss.x;
                double end=Time.realtimeSinceStartupAsDouble+50;
                while(found<quarters){
                    Place(boss.x+facing*235,false);
                    yield return null;Check();
                    var owner=Owner();
                    report.last="walk direction "+facing+" phases="+found+" current="+
                        (owner==null?"none":owner.Alias+"/"+owner.FrameIndex)+" x="+boss.x+" room width="+game.CurrentRoom.width;
                    Need(Time.realtimeSinceStartupAsDouble<end,report.last);
                    if(owner==null||!owner.Visible||owner.Alias!="source-walk"||owner.SourceAnimation!="Walk"||owner.Facing!=facing)continue;
                    int phase=Mathf.Clamp(Mathf.FloorToInt(Mathf.Repeat(owner.SourceClock,2.13333333f)*quarters/2.13333333f),0,quarters-1);
                    if(phases[phase])continue;
                    Observe("walk-"+(facing<0?"left":"right")+"-phase-"+phase,"source-walk");
                    phases[phase]=true;found++;
                }
                Need((boss.x-startX)*facing>8,"Original AI moved in the captured direction");
            }
            report.bothFacingsWalk=true;
        }
        IEnumerator Flow() {
            while(!(scene=FindFirstObjectByType<DemoScene>())){Check();yield return null;}
            for(int i=0;i<3;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons","Ordinary Main start");
            foreach(var t in Enter())yield return t;
            foreach(var t in Wait("source-idle",0,"01-idle-ratio",30))yield return t;
            var idle=report.samples[report.samples.Count-1];var hero=(SpriteRenderer)Field(scene,"heroView");
            Need(Math.Abs(game.player.y-boss.y)<.05f&&game.player.clip=="idle","Actual same-plane idle LK and boss");
            report.heroRatio=Capture((Camera)Field(scene,"worldCamera"),game,boss,new List<SpriteRenderer>{hero},output,"01-LK");
            report.actualRatio=idle.view.height/(float)report.heroRatio.height;
            Need(Math.Abs(report.actualRatio-report.expectedRatio)<.16f,"Observed boss/LK native proportion");
            foreach(var t in Cycle("A","source-attack-a-ready","source-attack-a","DamageEffect_ANCIENT_PPORU_ATTACK_A",84,new[]{.177f,1.273f},40))yield return t;
            foreach(var t in Vulnerable())yield return t;
            Hit(1,HitReaction.Knockdown);yield return null;
            Need(boss.state==CombatState.Hurt,"Original DAMAGE_DOWN false turns public knockdown into hurt");
            foreach(var t in Wait("source-hurt",.02f,"02-front-hurt"))yield return t;report.noKnockdown=true;
            foreach(var t in Lower(.60f))yield return t;
            Need(boss.hp/boss.maxHp>.40f,"B range remains above special threshold");
            foreach(var t in Cycle("B","source-attack-b-ready","source-attack-b","DamageEffect_ANCIENT_PPORU_ATTACK_B",185,new[]{.177f},100))yield return t;
            foreach(var t in Lower(.35f))yield return t;
            foreach(var t in Cycle("C","source-attack-ready","source-attack-special","DamageEffect_ANCIENT_PPORU_SPECIAL_ATTACK",182,new[]{.15f},45))yield return t;
            foreach(var t in Vulnerable())yield return t;
            Hit(1,HitReaction.Launch);yield return null;Need(boss.state==CombatState.Airborne&&!boss.knockdownOnLanding,"Source airborne reaction without landing knockdown");
            foreach(var t in Wait("source-air-hit",.02f,"03-air-reaction"))yield return t;
            double until=Time.realtimeSinceStartupAsDouble+20;
            while(boss.state==CombatState.Airborne){yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Source airborne return");}
            Need(boss.state!=CombatState.Knockdown,"Original launch lands without down");report.publicLaunch=true;
            foreach(var t in Vulnerable())yield return t;
            Hit((int)Math.Ceiling(boss.hp)+1,HitReaction.Hurt);
            foreach(var t in Wait("source-death",.06f,"04-death-early"))yield return t;float early=report.samples[report.samples.Count-1].alpha;
            foreach(var t in Wait("source-death",1.45f,"05-death-middle"))yield return t;float later=report.samples[report.samples.Count-1].alpha;
            Need(early>later&&later>0&&Math.Abs(later-(1-boss.deathTime/1.2f))<.004f,"Actual front land death and host fade");report.deathFade=true;
            var first=boss;foreach(var t in ReturnMenu())yield return t;
            if(traceSet){game.traceNpcEffects=originalTrace;traceSet=false;}
            foreach(var t in Enter())yield return t;Need(!ReferenceEquals(boss,first),"Fresh original room3 boss");
            foreach(var t in Wait("source-idle",0,"06-reentry"))yield return t;report.reentered=true;
            foreach(var t in WalkBothWays())yield return t;
            foreach(var t in ReturnMenu())yield return t;
        }
    }
}
#endif
