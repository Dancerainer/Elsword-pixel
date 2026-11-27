#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed class CannonPlayerCheckV35 : MonoBehaviour {
        [Serializable] sealed class Report {
            public string kind,npc,stage,error,last,projectile,selectedSprite,selectedAlias;public int recordedFrames,liveOwners,ioFrame,actualFrame;
            public string projectionStatus;public int projectionFrame,sourceSpriteId,visibleSpriteId;
            public bool sourceFromBattleProduct,sourceIsLibraryFrame,originalRenderer,visibleIsLibraryFrame,visibleFromSelectedFrame;
            public int index,room,frames,projectileSerial;
            public bool passed,ordinaryBirth,naturalAttack,publicHit,deathFade,originalGate,cleanup,returnedMain;
            public bool wreckIdle,wreckHit,wreckBreak;
            public List<string> wreckTransitions=new List<string>();
            public float projectileClock,gateClock,earlyAlpha,lateAlpha;
            public List<PixelProof> shots=new List<PixelProof>();
            public List<string> errors=new List<string>();
        }
        const string Flag="--root-cannon-check-v35",Output="--root-cannon-output-v35";
        Report report=new Report();string output;DemoScene scene;GameModel game;Enemy target;
        bool done,wasBackground;double started;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            string[] args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);
            if(f<0)return;
            try {
                int o=Array.IndexOf(args,Output);
                Need(f+1<args.Length && o>=0 && o+1<args.Length,"Exact actor and fresh report arguments");
                string mode=args[f+1];
                Need(mode=="mini"||mode=="heavy"||mode=="big","Original cannon mode");
                string path=Path.GetFullPath(args[o+1]);
                string root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh owned cannon artifact");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"Actual D3D11 Player");
                Directory.CreateDirectory(path);
                var ower=new GameObject("Explicit Airship Cannon V35").AddComponent<CannonPlayerCheckV35>();
                ower.output=path;ower.report.kind=mode;
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        void Log(string m,string stack,LogType type){
            if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert||m.Contains("CANNON_SOURCE_FALLBACK_V35"))report.errors.Add(m+" "+stack);
        }
        IEnumerator Start() {
            started=Time.realtimeSinceStartupAsDouble;wasBackground=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;
            IEnumerator flow=Flow();Exception error=null;
            while(!done) {
                bool next=false;object step=null;
                try {Need(report.errors.Count==0,"No runtime or fallback error");Need(Time.realtimeSinceStartupAsDouble-started<400,"Total cannon deadline "+report.last);
                    next=flow.MoveNext();if(next)step=flow.Current;}catch(Exception e){error=e;}
                if(error!=null||!next)break;report.frames++;yield return step;
            }
            (flow as IDisposable)?.Dispose();if(!done){report.error=error?.ToString();report.passed=error==null;Finish();}
        }
        void Update(){if(!done && started>0 && Time.realtimeSinceStartupAsDouble-started>=400){report.error="Total deadline "+report.last;Finish();}}
        void Finish(){if(done)return;done=true;Application.logMessageReceived-=Log;Application.runInBackground=wasBackground;
            try{File.WriteAllText(Path.Combine(output,"cannon-player-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}}
        IEnumerable Enter() {
            foreach(var step in EnterRoute(scene,"altera","altera-battle-airship"))yield return step;
            // The normal map button starts a multiframe resource-loading coroutine.
            // Match the background Player's real room/world readiness boundary.
            double until=Time.realtimeSinceStartupAsDouble+110;
            for(;;) {
                game=(GameModel)Field(scene,"game");
                var native=Field(scene,"nativeScene");
                string world=native==null?"none":(string)Field(native,"id");
                var camera=Field(scene,"nativeCamera") as Camera;
                string menu=(string)Field(scene,"openMenu");
                if(scene.BattleProductSessionActiveR1325 && menu=="" && game!=null && !game.paused &&
                    camera && world=="WI_ALTERA_BATTLE_AIR_SHIP_STAGE1" && !(bool)Field(scene,"calibrationRun"))break;
                report.last="admitting real Battle Airship, menu="+menu+" world="+world;
                Need(Time.realtimeSinceStartupAsDouble<until,"Original map admission/world deadline "+report.last);
                yield return null;
            }
            report.room=report.kind=="big"?2:0;report.index=report.kind=="big"?10:report.kind=="heavy"?3:2;
            report.npc=report.kind=="big"?"NUI_CANNON_BIG":report.kind=="heavy"?"NUI_CANNON_HEAVY_MACHINE_GUN":"NUI_CANNON_MINI";
            if(game.room!=report.room) {
                Call(game,"EnterRoom",report.room);if((int)Field(scene,"shownRoom")!=game.room)Call(scene,"RebuildRoom");
            }
            if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            Need(game.room==report.room&&game.enemies.Count>report.index,"Original airship birth roster");
            target=game.enemies[report.index];report.stage=game.CurrentRoom.id;
            Need(target.sourceNPC==report.npc && target.encounterIndex==0 && target.kind==CannonRigV35.Kind(report.npc),"Original room/encounter/index NPC");
            if(report.kind=="big")Need(target.sourceKeyCode==2,"Big cannon parent key2");
            report.ordinaryBirth=true;game.player.hp=game.player.maxHp=5000;game.player.invincible=180;
            Place();
        }
        void Place() {
            float x=Mathf.Clamp(target.x+(target.facing==0?1:target.facing)*95,35,game.CurrentRoom.width-35);
            var floor=game.SurfaceBelow(x,Math.Max(target.y,game.player.y)+2,true);
            if(floor==null){x=target.x;floor=game.SurfaceBelow(x,target.y+12,true);}
            Need(floor!=null,"Player stands on original ship floor");
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;
        }
        FairyAuthoredPartsV33 Owner(){
            object entry=((IDictionary)Field(scene,"cannonViewsV35"))[target];
            return entry==null?null:(FairyAuthoredPartsV33)Field(entry,"parts");
        }
        bool CanSee(FairyAuthoredPartsV33 owner) {
            if(owner==null||!owner.LastFrame.visible)return false;
            var cam=(Camera)Field(scene,"worldCamera");bool visible=false;
            owner.VisitRenderers(r=>{var p=cam.WorldToViewportPoint(r.bounds.center);visible|=r.enabled && r.transform.lossyScale.sqrMagnitude>0.01f&&p.z>0&&p.x>.015f&&p.x<.985f&&p.y>.015f&&p.y<.985f;});
            return visible;
        }
        float Screenshot(string id) {
            var owner=Owner();Need(CanSee(owner),"Original model extent in actual camera "+id);
            var rs=new List<SpriteRenderer>();owner.VisitRenderers(rs.Add);
            int expected=report.kind=="big"?3:report.kind=="heavy"?10:4;
            Need(rs.Count==expected,"Exact number of registered instances");
            var proof=Capture((Camera)Field(scene,"worldCamera"),game,target,rs,output,id);
            report.shots.Add(proof);return rs[0].color.a;
        }
        IEnumerable RunAttack() {
            double deadline=Time.realtimeSinceStartupAsDouble+86;
            bool neutral=false,attack=false;
            while(!neutral || !attack || string.IsNullOrEmpty(report.projectile)) {
                yield return null;var owner=Owner();Place();
                var list=(IList)Field(scene,"enemyViews");var body=(SpriteRenderer)list[game.enemies.IndexOf(target)];
                var selections=(IDictionary)Field(scene,"cannonFramesV35");report.recordedFrames=selections.Count;
                report.liveOwners=((IDictionary)Field(scene,"cannonViewsV35")).Count;report.actualFrame=Time.frameCount;
                var selected=selections[target];report.ioFrame=selected==null?-1:(int)Field(selected,"number");
                report.selectedAlias=selected==null?"":(string)Field(selected,"alias");
                if(selected!=null){
                    var canonical=(Sprite)Field(selected,"sprite");var carrier=(SpriteRenderer)Field(selected,"carrier");
                    report.projectionFrame=(int)Field(selected,"projectionFrame");report.projectionStatus=(string)Field(selected,"viewStatus");
                    var library=((IDictionary)Field(scene,"libraries"))[target.kind];var sprites=(Sprite[])((IDictionary)Field(library,"frames"))[report.selectedAlias];
                    var productObjects=(IList)Field(scene,"battleProductOwnedR1325");
                    report.originalRenderer=carrier==body;report.sourceIsLibraryFrame=Array.IndexOf(sprites,canonical)>=0;
                    report.sourceFromBattleProduct=productObjects.Contains(canonical)&&productObjects.Contains(canonical.texture);
                    report.visibleIsLibraryFrame=body&&body.sprite&&Array.IndexOf(sprites,body.sprite)>=0;
                    report.visibleFromSelectedFrame=body&&MonsterTextureRedrawV22.SameSourceFrameV35(body.sprite,canonical);
                    Need(report.visibleFromSelectedFrame,"Visible sprite is the selected Battle frame or its actual R22 repaint");
                    report.sourceSpriteId=canonical.GetInstanceID();report.visibleSpriteId=body&&body.sprite?body.sprite.GetInstanceID():0;
                    Need(report.originalRenderer&&report.sourceIsLibraryFrame&&report.sourceFromBattleProduct,"Live original Battle product sprite/texture/renderer provenance");
                    Need(report.projectionStatus==null||!report.projectionStatus.StartsWith("failure:",StringComparison.Ordinal),report.projectionStatus);
                }
                report.selectedSprite=body&&body.sprite?body.sprite.name:"<missing>"; 
                report.last="Attack "+target.npcState+" "+target.npcTime+" "+owner?.LastFrame.alias+
                  " originalSprite="+report.selectedSprite+" alias="+report.selectedAlias+" selections="+report.recordedFrames+
                  " owners="+report.liveOwners+" frame="+report.ioFrame+"/"+report.actualFrame+" projection="+report.projectionFrame+"/"+report.projectionStatus+" spriteIds="+report.sourceSpriteId+"/"+report.visibleSpriteId;
                foreach(var fx in game.npcEffects)if(ReferenceEquals(fx.owner,target) && string.IsNullOrEmpty(report.projectile)){
                    report.projectile=fx.id;report.projectileSerial=fx.serial;report.projectileClock=target.npcTime;
                }
                if(CanSee(owner)) {
                    string a=owner.LastFrame.alias;
                    if(!neutral&&(a=="start"||a=="idle"||a=="source-attack-ready"||a=="attack-a-ready")) {Screenshot("01-ready");neutral=true;}
                    if(!attack&&(a=="attack-a"||a=="source-attack1"||a=="source-attack2"||a=="source-attack3") && owner.LastFrame.sourceClock>.12f) {Screenshot("02-firing");attack=true;}
                }
                Need(Time.realtimeSinceStartupAsDouble<deadline,"Natural firing and original owner projectile "+report.last+" "+report.projectile);
            }
            report.naturalAttack=true;
        }
        IEnumerable PublicHit(bool lethal) {
            double end=Time.realtimeSinceStartupAsDouble+45;
            for(;;) {
                yield return null;Place();float old=target.hp;
                int actual=game.ApplyHitToEnemy(target,lethal?(int)Math.Ceiling(old)+2:1,HitReaction.Hurt);
                if(actual>0 && target.hp<old){Need(!lethal||target.hp<=0,"Actual lethal damage");break;}
                Need(Time.realtimeSinceStartupAsDouble<end,"Original cannon vulnerable window "+target.npcState);
            }
        }
        IEnumerable Death() {
            string alias=report.kind=="mini"?"idle":report.kind=="heavy"?"broken":"source-down";
            double end=Time.realtimeSinceStartupAsDouble+20;
            float gate=-1;
            while(report.lateAlpha<=0 || (report.kind=="big"&&!report.originalGate)) {
                yield return null;var owner=Owner();
                report.last="death "+target.state+" "+target.deathTime+" "+owner?.LastFrame.alias;
                if(CanSee(owner) && owner.LastFrame.alias==alias){
                    if(report.earlyAlpha<=0 && target.deathTime>.04f && target.deathTime<.3f)report.earlyAlpha=Screenshot("04-death-early");
                    if(report.lateAlpha<=0 && target.deathTime>.52f && target.deathTime<1f)report.lateAlpha=Screenshot("05-death-middle");
                }
                if(report.kind=="big"){
                    float t=game.AlteraCannonDyingLandClock(target);
                    foreach(var e in game.enemies)if(e.sourceNPC=="NUI_CANNON_BIG_DOWN"&&e.sourceKeyCode==1&&e.active&&!report.originalGate){
                        Need(t>=.899f && gate<.94f && e.npcState!="CANNON_BIG_GATE_DISABLED" &&
                             Mathf.Abs(e.x-target.x)<.02f && Mathf.Abs(e.y-target.y)<.02f,"Original key1 gate born on parent death crossing");
                        report.originalGate=true;report.gateClock=t;
                    }
                    gate=t;
                }
                Need(Time.realtimeSinceStartupAsDouble<end,"Death art and original gate "+report.last);
            }
            Need(report.earlyAlpha>report.lateAlpha && report.lateAlpha>0,"Original visible death fade");
            report.deathFade=true;
        }
        IEnumerable ReturnMenu(){
            var existing=new List<SpriteRenderer>();Owner()?.VisitRenderers(existing.Add);
            while(scene.BossIntroductionActive)yield return null;
            Press(scene,"dungeons");for(int i=0;i<6;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons" && ((IDictionary)Field(scene,"cannonViewsV35")).Count==0 && ((IDictionary)Field(scene,"cannonBanksV35")).Count==0,"Return menu releases all registered views");
            foreach(var r in existing)Need(!r,"Owned renderer released");report.cleanup=report.returnedMain=true;
        }
        IEnumerable WreckFrame(Enemy gate,string alias,string label) {
            double end=Time.realtimeSinceStartupAsDouble+18;
            var views=(IList)Field(scene,"enemyViews");
            var renderer=(SpriteRenderer)views[game.enemies.IndexOf(gate)];
            string previous=null;
            for(;;) {
                yield return null;
                var frame=scene.MagicianFrameV38(renderer);
                string status=gate.npcState+"/"+gate.clip+"/"+(frame==null?"no registered frame":frame.alias)+"/"+renderer.enabled;
                if(status!=previous){
                    report.wreckTransitions.Add(label+" "+status+" hp="+gate.hp+" clock="+gate.clock);
                    previous=status;
                }
                report.last="wreck "+label+" state "+gate.npcState+" clip "+gate.clip+" alias "+frame?.alias+" "+renderer.sprite?.name;
                Need(Time.realtimeSinceStartupAsDouble<end,report.last);
                if(frame==null||frame.alias!=alias||!renderer.enabled||renderer.color.a<=.02f)continue;
                Need(frame.enemy==gate&&frame.page.StartsWith("Pixel/feedback-v38/cannon-big-down/",StringComparison.Ordinal),"Original gate uses dedicated painted material");
                var proof=CaptureAtResolution((Camera)Field(scene,"worldCamera"),game,gate,new List<SpriteRenderer>{renderer},output,label,640,360);
                Need(proof.count>0,"Original gate changes ordinary world pixels");
                yield break;
            }
        }
        IEnumerable Wreck() {
            var gate=game.enemies.Find(e=>e.sourceNPC=="NUI_CANNON_BIG_DOWN"&&e.sourceKeyCode==1&&e.active);
            Need(gate!=null&&gate.isGate&&gate.maxHp==42,"Exact deferred destructible gate");
            foreach(var step in WreckFrame(gate,"idle","06-wreck-idle"))yield return step;
            report.wreckIdle=true;
            double end=Time.realtimeSinceStartupAsDouble+45;
            while(!game.AlteraCannonGateGaugeVisible(gate)){
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Original encounter-clear gate protection");
                foreach(var e in game.enemies.ToArray())
                    if(e!=gate&&e.active&&!e.dormant&&e.countsForClear&&e.hp>0&&!e.passive)
                        game.ApplyHitToEnemy(e,(int)Math.Ceiling(e.hp)+1,HitReaction.Hurt);
            }
            Need(game.ApplyHitToEnemy(gate,1,HitReaction.Hurt)>0,"Positive public gate damage");
            Need(gate.hp>0&&gate.npcState=="CANNON_BIG_GATE_DAMAGE","Original surviving gate enters Damage on actual contact: "+gate.npcState+"/"+gate.clip);
            foreach(var step in WreckFrame(gate,"hurt","07-wreck-hurt"))yield return step;
            report.wreckHit=true;
            end=Time.realtimeSinceStartupAsDouble+10;
            while(gate.hp>0){
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Gate vulnerable interval");
                game.ApplyHitToEnemy(gate,(int)Math.Ceiling(gate.hp)+2,HitReaction.Hurt);
            }
            Need(gate.npcState=="CANNON_BIG_GATE_DYING","Original lethal gate enters Broken: "+gate.npcState+"/"+gate.clip);
            foreach(var step in WreckFrame(gate,"broken","08-wreck-broken"))yield return step;
            report.wreckBreak=true;
        }
        IEnumerator Flow() {
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;
            for(int j=0;j<3;j++)yield return null;
            if((string)Field(scene,"openMenu")!="dungeons"){Call(scene,"Action","dungeons");yield return null;}
            foreach(var s in Enter())yield return s;
            foreach(var s in RunAttack())yield return s;
            foreach(var s in PublicHit(false))yield return s;
            for(int j=0;j<4;j++)yield return null;
            Screenshot("03-public-hurt");report.publicHit=true;
            foreach(var s in PublicHit(true))yield return s;
            foreach(var s in Death())yield return s;
            if(report.kind=="big")foreach(var s in Wreck())yield return s;
            var first=target;
            foreach(var s in ReturnMenu())yield return s;
            foreach(var s in Enter())yield return s;Need(!ReferenceEquals(first,target),"Fresh original cannon on reentry");
            double end=Time.realtimeSinceStartupAsDouble+22;
            while(!CanSee(Owner())){Place();Need(Time.realtimeSinceStartupAsDouble<end,"New registered renderer after reentry");yield return null;}
            Screenshot("06-reentered");
            foreach(var s in ReturnMenu())yield return s;
        }
    }
}
#endif
