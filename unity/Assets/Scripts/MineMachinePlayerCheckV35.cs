#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class MineMachinePlayerCheckV35:MonoBehaviour {
        const string Flag="--root-mine-machine-check-v35",Out="--root-mine-machine-output-v35",Kind="--root-mine-machine-kind-v35";
        [Serializable] sealed class Row {
            public string label,state,clip,alias;public int index,facing,line,parts;public float npcTime,clock,x,y,hp,alpha;
            public PixelProof view;
        }
        [Serializable] sealed class Audit {
            public string kind,error,last,scope="Controlled original room stage through ordinary menu and existing GameModel. Explicit world-camera, no window presentation.";
            public bool passed,originalBirth,sourceTurn,noLaunch,originalAttack,actualDeath,reentered,released,batchMode,originalVictoryHeld;
            public List<string> errors=new List<string>();public List<Row> captures=new List<Row>();
            public List<ShotPacket> farShots=new List<ShotPacket>();
        }
        [Serializable] sealed class ShotPacket {
            public int serial,facing;public string id,sourceState,effectVisualId;
            public float sourceAt,observedMotionTime,x,y,z,rotation;
        }
        readonly Audit report=new Audit();
        string dir,kind;
        bool rail,done,background;
        double start;
        Enemy actor;
        DemoScene scene;
        GameModel game,main;
        bool Active=>rail?scene.Mine2ProductSessionActiveR1233:scene.PlainProductSessionActiveR1320;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            string[] a=Environment.GetCommandLineArgs();if(Array.IndexOf(a,Flag)<0)return;
            try{
                int f=Array.IndexOf(a,Flag),o=Array.IndexOf(a,Out),k=Array.IndexOf(a,Kind);
                Need(f==Array.LastIndexOf(a,Flag)&&o>=0&&o==Array.LastIndexOf(a,Out)&&o+1<a.Length&&
                    k>=0&&k==Array.LastIndexOf(a,Kind)&&k+1<a.Length,"Machine check/output/kind once");
                string id=a[k+1];Need(id==MineMachineRigV35.Rail||id==MineMachineRigV35.Driller,"Original machine kind");
                foreach(var arg in a)Need(arg==Flag||!arg.StartsWith("--root-")||!arg.Contains("-check-"),"One driving fixture at once");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"D3D11 Standalone required");
                string path=Path.GetFullPath(a[o+1]),parent=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(parent,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh owned output");
                Directory.CreateDirectory(path);
                var m=new GameObject("Explicit Mine Machine original route V35").AddComponent<MineMachinePlayerCheckV35>();
                m.dir=path;m.kind=id;m.rail=id==MineMachineRigV35.Rail;
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        void Log(string msg,string stack,LogType type){
            if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert||msg.Contains("MINE_MACHINE_PARTS_FALLBACK"))
                report.errors.Add(msg+"\n"+stack);
        }
        void Finish(string error){
            if(done)return;done=true;report.error=error;report.passed=error==null&&report.errors.Count==0;
            Application.logMessageReceived-=Log;int code=report.passed?0:2;
            try{File.WriteAllText(Path.Combine(dir,"mine-machine-player-report.json"),JsonUtility.ToJson(report,true));}
            catch(Exception e){Debug.LogException(e);code=2;}Application.Quit(code);
        }
        IEnumerator Start(){
            report.kind=kind;report.batchMode=Application.isBatchMode;start=Time.realtimeSinceStartupAsDouble;
            background=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;
            var flow=Flow();string error=null;
            while(!done){object next=null;bool moved=false;
                try{Need(report.errors.Count==0,"Runtime errors");moved=flow.MoveNext();if(moved)next=flow.Current;}
                catch(Exception e){error=e.ToString();}
                if(error!=null||!moved)break;yield return next;
            }
            (flow as IDisposable)?.Dispose();if(!done)Finish(error);
        }
        void Update(){if(!done&&start>0&&Time.realtimeSinceStartupAsDouble-start>=280)Finish("Total wall deadline "+report.last);}
        void OnDestroy(){Application.logMessageReceived-=Log;if(start>0)Application.runInBackground=background;}
        IEnumerable Enter(){
            string route=rail?"bethma-richie-mine":"altera-plain-recycle";
            foreach(var step in EnterRoute(scene,rail?"bethma":"altera",route))yield return step;
            while(!Active)yield return null;
            game=(GameModel)Field(scene,"game");
            int room=rail?1:2,index=rail?9:2;
            Call(game,"EnterRoom",room);if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            actor=game.enemies[index];Need(actor.kind==kind&&actor.sourceNPC==(rail?MineMachineRigV35.RailId:MineMachineRigV35.DrillerId)&&
                actor.encounterIndex==0,"Exact original source birth "+room+"/"+index);
            report.originalBirth=true;Place(rail?46:-90);
            // This fixture jumps directly between original rooms. The ordinary
            // Host must rebind the room world, actor views and Recycle mine
            // spatial/presentation services before the next real model Step.
            Call(scene,"RebuildRoom");
            Need((int)Field(scene,"shownRoom")==game.room,"Original Host rebuilt the staged room");
            if(!rail)Need(game.PlainMineOriginalReadyImplementedR1269,"Actual Recycle Mine spatial and presentation owners bound");
        }
        void Place(float delta){
            game.player.x=actor.x+delta;game.player.y=actor.y;game.player.groundLine=actor.groundLine;
            game.player.vx=game.player.vy=0;game.player.invincible=100;
        }
        FairyAuthoredPartsV33 Owner(){
            var entries=(IDictionary)Field(scene,"machineViewsV35");
            return entries.Contains(actor)?Field(entries[actor],"parts") as FairyAuthoredPartsV33:null;
        }
        bool Visible(){
            var owner=Owner();if(owner==null||!owner.LastFrame.visible)return false;
            var parts=new List<SpriteRenderer>();owner.VisitRenderers(parts.Add);
            var p=((Camera)Field(scene,"worldCamera")).WorldToViewportPoint(parts[0].bounds.center);
            return p.z>0&&p.x>.01f&&p.x<.99f&&p.y>.01f&&p.y<.99f;
        }
        string ViewDiagnostic(){
            int i=game.enemies.IndexOf(actor);var carriers=(IList)Field(scene,"enemyViews");
            var c=(Camera)Field(scene,"worldCamera");var native=(Camera)Field(scene,"nativeCamera");
            var s=i>=0&&i<carriers.Count?carriers[i] as SpriteRenderer:null;var owner=Owner();
            var lists=(IDictionary)Field(scene,"libraries");var hasArt=lists.Contains(kind);
            string aliases=hasArt?((IDictionary)Field(lists[kind],"frames")).Contains(actor.clip).ToString():"no-library";
            string result=" activeSession="+Active+" modelKind="+actor.kind+" sourceNPC="+actor.sourceNPC+" kindSelected="+Field(scene,"machineKindV35")+" privatePublications="+Field(scene,"mine2RailPublishedV35")+" privatePhase="+Field(scene,"mine2RailLastPhaseV35")+" missing="+Field(scene,"machineMissingRailDrawsV35")+" camera="+c.transform.position+" native="+(native?native.transform.position.ToString():"none")+
                " player="+game.player.x+","+game.player.y+" roomViews="+carriers.Count+" index="+i+" shown="+Field(scene,"shownRoom")+
                " modelVisible="+actor.active+"/"+actor.dormant+" originalAlias="+aliases+
                " carrier="+(s?s.name+"/"+(s.sprite?s.sprite.name:"no-sprite")+" enabled="+s.enabled+" active="+s.gameObject.activeInHierarchy+
                    " forceOff="+s.forceRenderingOff+" viewport="+c.WorldToViewportPoint(s.bounds.center):"absent")+
                " bank="+(Field(scene,"machineBankV35")!=null)+" viewOwners="+((IDictionary)Field(scene,"machineViewsV35")).Count;
            if(owner!=null){var parts=new List<SpriteRenderer>();owner.VisitRenderers(parts.Add);var f=owner.LastFrame;
                result+=" actual="+f.alias+"/"+f.sourceClock+" visible="+f.visible+" part0World="+parts[0].bounds.center+" part0Viewport="+c.WorldToViewportPoint(parts[0].bounds.center);}
            return result;
        }
        Row Observe(string label){
            var owner=Owner();Need(owner!=null&&owner.LastFrame.visible,"Original visible owner "+label);
            var rows=(IDictionary)Field(scene,"machineViewsV35");var carrier=(SpriteRenderer)Field(rows[actor],"carrier");
            var views=new List<SpriteRenderer>();owner.VisitRenderers(views.Add);
            Need(views.Count==(rail?11:8)&&carrier&&carrier.forceRenderingOff&&carrier.enabled&&carrier.sprite,"Original selected carrier + registered pieces "+label);
            foreach(var v in views)Need(v&&v.enabled&&v.sprite&&v.gameObject.activeInHierarchy&&Math.Abs(v.color.a-carrier.color.a)<1e-4f,"Actual part visible alpha "+label);
            var f=owner.LastFrame;
            Need(f.facing==(carrier.flipX?-1:1),"Facing from original carrier "+label);
            var row=new Row{label=label,state=actor.npcState,clip=actor.clip,alias=f.alias,index=game.enemies.IndexOf(actor),facing=f.facing,
                parts=views.Count,npcTime=actor.npcTime,clock=f.sourceClock,x=actor.x,y=actor.y,line=actor.groundLine,hp=actor.hp,alpha=carrier.color.a};
            report.captures.Add(row);row.view=Capture((Camera)Field(scene,"worldCamera"),game,actor,views,dir,report.captures.Count.ToString("D3")+"-"+label);
            Need(row.view.count>0,"Selected parts have measurable GPU contribution "+label);return row;
        }
        IEnumerable WaitFor(Func<bool> predicate,string why,double seconds=50,bool follow=true){
            double end=Time.realtimeSinceStartupAsDouble+seconds;
            while(true){
                if(follow)Place(rail?46:-90);yield return null;
                report.last=why+" actual="+actor.npcState+"/"+actor.clip+" clock="+actor.npcTime+" hp="+actor.hp+" x="+actor.x+" y="+actor.y+" room="+game.room+ViewDiagnostic();
                if(Visible()&&predicate())yield break;Need(Time.realtimeSinceStartupAsDouble<end,"Wait "+report.last);
            }
        }
        bool CanHit(){return actor.active&&!actor.dormant&&actor.hp>0&&actor.state==CombatState.Normal&&!string.IsNullOrEmpty(actor.npcState)&&
            !game.EnemyHasSuperArmor(actor)&&!game.EnemyHasNpcInvincibility(actor)&&(bool)Call(game,"Grounded",actor);}
        void Hit(int damage,HitReaction reaction){
            Place(-actor.facing*40);float before=actor.hp;int result=game.ApplyHitToEnemy(actor,damage,reaction);
            Need(result>0&&actor.hp<before,"Public accepted source hit "+reaction);
        }
        IEnumerable RailActions(){
            foreach(var x in WaitFor(()=>actor.npcState=="NASOD_RAIL_DASH_ATTACK","original loop"))yield return x;
            var first=Observe("cart-moving");int facing=first.facing;
            foreach(var x in WaitFor(()=>actor.npcState=="NASOD_RAIL_TURN","line-end source turn",95))yield return x;
            Observe("source-turn");report.sourceTurn=true;
            foreach(var x in WaitFor(()=>actor.facing==-facing&&actor.npcState=="NASOD_RAIL_DASH_ATTACK","opposite source cart travel",55))yield return x;
            Observe("opposite-direction");report.originalAttack=true;
            foreach(var x in WaitFor(CanHit,"original vulnerable rail"))yield return x;
            Hit(1,HitReaction.Launch);float x0=actor.x;int line=actor.groundLine;
            yield return null;
            Need(actor.state==CombatState.Hurt&&actor.groundLine==line&&Math.Abs(actor.x-x0)<.08f,"Original rail forbids launch and movement during damage");
            Observe("line-locked-hurt");report.noLaunch=true;
        }
        IEnumerable DrillerActions(){
            foreach(var x in WaitFor(()=>actor.npcState=="NASOD_DRILLER_CANNON_ATTACK_A_READY","natural A ready",90))yield return x;
            Observe("ready-a");
            foreach(var x in WaitFor(()=>actor.npcState=="NASOD_DRILLER_CANNON_ATTACK_A","natural A fire",20))yield return x;
            Observe("attack-a");report.originalAttack=true;
            foreach(var x in DrillerFar(1))yield return x;
            foreach(var x in DrillerFar(-1))yield return x;
            foreach(var x in WaitFor(CanHit,"original vulnerable driller"))yield return x;
            Hit(1,HitReaction.Hurt);yield return null;Need(actor.state==CombatState.Hurt,"Original driller physical hurt");Observe("hurt");
        }
        IEnumerable DrillerFar(int direction){
            string ready="NASOD_DRILLER_CANNON_ATTACK_C_READY",fire="NASOD_DRILLER_CANNON_ATTACK_C";
            string expectedFx="DamageEffect_NASOD_DRILLER_CANNON_ATTACK_C_"+(direction>0?"RIGHT":"LEFT");
            var already=new HashSet<int>();foreach(var f in game.npcEffects)already.Add(f.serial);
            bool sawReady=false;double end=Time.realtimeSinceStartupAsDouble+65;
            while(true){
                Place(direction*220);yield return null;
                report.last="Original far C "+direction+" actual="+actor.npcState+" npcTime="+actor.npcTime+" x="+actor.x+" player="+game.player.x;
                Need(Time.realtimeSinceStartupAsDouble<end,report.last);
                if(!Visible())continue;
                if(!sawReady&&actor.npcState==ready&&actor.facing==direction){
                    Observe(direction>0?"far-ready-right":"far-ready-left");sawReady=true;
                }
                if(!sawReady||actor.npcState!=fire)continue;
                foreach(var f in game.npcEffects) {
                    if(f.owner!=actor||f.id!=expectedFx||f.emission?.sourcePlainCallbackStateR1315!=fire||already.Contains(f.serial))continue;
                    Need(f.emission.start==.05f&&f.emission.rotationZ==25&&f.emission.sourceLandPositionYArgument,"Original cannon packet .05s with land argument/25 degree launch");
                    report.farShots.Add(new ShotPacket{serial=f.serial,facing=f.facing,id=f.id,sourceState=fire,sourceAt=f.emission.start,
                        observedMotionTime=actor.npcTime,x=f.x,y=f.y,z=f.z,rotation=f.emission.rotationZ,effectVisualId=game.NpcEffectVisual(f).id});
                    Observe(direction>0?"far-fire-right":"far-fire-left");yield break;
                }
            }
        }
        IEnumerable Leave(){
            var old=new List<SpriteRenderer>();Owner()?.VisitRenderers(old.Add);
            var bank=(FairyPoseLibraryV33)Field(scene,"machineBankV35");
            var oldSprite=bank?.sharedSprites[0];var oldAtlas=bank?.renderAtlas;
            // The ordinary input layer ignores dungeon/map while a freshly
            // reentered boss is playing its opening. Exercise the same control
            // immediately after that authored gate releases.
            double until=Time.realtimeSinceStartupAsDouble+12;
            while(scene.BossIntroductionActive){yield return null;Need(Time.realtimeSinceStartupAsDouble<until,"Original boss opening released");}
            Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons"&&!Active&&ReferenceEquals(Field(scene,"game"),main)&&
                Field(scene,"machineBankV35")==null&&!oldSprite&&!oldAtlas,"Original Menu and released bank/owned texture");
            foreach(var r in old)Need(!r,"Old machine renderer released");report.released=true;
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;
            for(int i=0;i<3;i++)yield return null;
            main=(GameModel)Field(scene,"game");Need(main.paused&&(string)Field(scene,"openMenu")=="dungeons","Ordinary Main");
            foreach(var x in Enter())yield return x;
            foreach(var x in WaitFor(()=>true,"first actual display"))yield return x;
            Observe("original-birth");if(rail){foreach(var x in RailActions())yield return x;}else{foreach(var x in DrillerActions())yield return x;}
            foreach(var x in WaitFor(CanHit,"vulnerable before death",55))yield return x;
            Hit(Mathf.CeilToInt(actor.hp*2)+2,HitReaction.Hurt);Need(actor.hp<=0&&actor.state==CombatState.Dead,"Real zero-HP death");
            yield return null;var first=Observe("death-first");
            if(rail){
                foreach(var x in WaitFor(()=>actor.deathTime>=.55f,"death fade",13,false))yield return x;
                var later=Observe("death-fade");
                Need(first.alias=="source-damege" && later.alias==first.alias && later.alpha>0 && later.alpha<first.alpha,"Original Rail ground-death pose and actual fade");
            }else{
                Need(first.alias=="down-back" && game.TryMineMachineOriginalPoseV35(actor,out var original,out var t) &&
                    original==first.alias && Math.Abs(t-first.clock)<.0001f,"Real lethal rear packet selected original Back at first draw");
                // Render may publish Dead in an Update with zero fixed steps. Only
                // the existing model step evaluates the room's final clear clause.
                double limit=Time.realtimeSinceStartupAsDouble+5;
                while(!game.victory){yield return null;Need(Time.realtimeSinceStartupAsDouble<limit,"Actual Recycle victory after lethal");}
                var victory=Observe("boss-victory");
                Need(game.TryMineMachineOriginalPoseV35(actor,out original,out t) && original=="down-back" &&
                    victory.alias==original && Math.Abs(victory.clock-t)<.0001f && victory.alpha>0 &&
                    Mathf.Abs(victory.alpha-(1-actor.deathTime/1.2f))<.0001f,"Actual original boss pose and alpha on victory step");
                float fixedElapsed=game.elapsed, fixedDeath=actor.deathTime;
                for(int i=0;i<12;i++)yield return null;
                var held=Observe("boss-victory-held");
                Need(game.victory && game.elapsed==fixedElapsed && actor.deathTime==fixedDeath && held.alias==victory.alias &&
                    held.clock==victory.clock && held.alpha==victory.alpha && held.view.count>0,"Same live victory-held original pose and actual draw");
                report.originalVictoryHeld=true;
            }
            report.actualDeath=true;
            var prior=actor;foreach(var x in Leave())yield return x;
            foreach(var x in Enter())yield return x;foreach(var x in WaitFor(()=>true,"reentry actual birth"))yield return x;
            Need(!ReferenceEquals(prior,actor),"New original birth");Observe("reentry");report.reentered=true;
            foreach(var x in Leave())yield return x;
        }
    }
}
#endif
