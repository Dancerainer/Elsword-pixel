#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        internal void VerifyBesmaMotionCatalogV45(out int identities,out int actions,out int samples) {
            identities=actions=samples=0;
            foreach(var pair in libraries){
                var bank=SourceMotionInkV45.Library(pair.Key);
                if(bank==null)continue;
                var rendered=pair.Value;
                Need(rendered.data.artVersion==bank.artVersion,"Ordinary runtime bank "+pair.Key);
                var pages=new Texture2D[bank.pageCount];
                for(int i=0;i<pages.Length;i++)pages[i]=Resources.Load<Texture2D>(BluePoruInkV45.Resource(pair.Key,"Pixel/motion-v45/"+pair.Key+"/page-"+i));
                identities++;
                foreach(var track in bank.clips){
                    actions++;
                    var sprites=rendered.frames[track.alias];
                    Need(sprites.Length==track.indices.Length,"Exact ordinary frame clock "+track.alias);
                    for(int i=0;i<sprites.Length;i++){
                        var sprite=sprites[i];var f=bank.frames[track.indices[i]];var tex=pages[f.page];
                        Need(sprite&&tex&&ReferenceEquals(sprite.texture,tex)&&sprite.rect==new Rect(f.x,tex.height-f.y-f.h,f.w,f.h)
                            &&spriteOffsets.TryGetValue(sprite,out var foot)&&foot==new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f),
                            "Original clock / source pose / registered root "+pair.Key+"/"+track.alias+"["+i+"]");
                        samples++;
                    }
                }
            }
        }
        internal bool IsBesmaMotionV45(Enemy actor,SpriteRenderer view) {
            if(!view||!view.sprite||!view.enabled||view.forceRenderingOff||!view.gameObject.activeInHierarchy)return false;
            var bank=SourceMotionInkV45.Library(actor.kind);
            if(bank==null)return false;
            for(int i=0;i<bank.pageCount;i++)if(ReferenceEquals(view.sprite.texture,Resources.Load<Texture2D>(BluePoruInkV45.Resource(actor.kind,"Pixel/motion-v45/"+actor.kind+"/page-"+i))))return true;
            return false;
        }
        internal SpriteRenderer ActualElderHitBodyV45(Enemy actor,SpriteRenderer carrier){
            return paintOwnersV37.TryGetValue(actor,out var holder)&&holder.view!=null&&holder.view.painted?holder.view.painted:carrier;
        }
        internal void CoreMotionCacheMetricV45(string kind,out int peak,out double maxDecodeMs) {
            peak=0;maxDecodeMs=0;
            if(coreInkV45.TryGetValue(kind,out var cache)){peak=cache.peakDecoded;maxDecodeMs=cache.maxDecodeMs;}
        }
        internal bool ConfirmCoreMotionFrameV45(Enemy e,SpriteRenderer view,out string source,out string key,out float time){
            source=key=null;time=0;
            if(!view||!view.sprite||string.IsNullOrEmpty(e.npcState))return false;
            if(e.kind=="nasod-king"){
                if(!game.CoreHardDirectorCompletedV21||CoreKingWholeLastStateV21!=e.npcState)return false;
                var state=coreKingWholeLookupV21.State(e.npcState);var clip=coreKingWholeLookupV21.Clip(state.displayClip);
                var f=clip.frames[CoreKingWholeLookupV21.FrameIndex(clip,CoreKingWholeLastNpcClockV21)];
                source=clip.sourceAnimation;time=(float)(CoreKingWholeLastNpcClockV21*clip.sourceAnimSpeed);key=f.paintedPoseKey;
            }else if(e.kind=="nasod-element-fire-b"){
                var state=coreElementFireBWholeLookupV21.State(e.npcState);var selected=coreElementFireBWholeLookupV21.Select(e.npcState,e.npcTime);
                source=state.Motion;time=(float)selected.SourceMotionClock;key=selected.Whole.Key;
            }else return false;
            return MatchesCoreWholeInkV45(e.kind,source,time,key,null,view.sprite,out var registered)&&registered!=null;
        }
    }

    public sealed partial class SourceMotionPlayerCheckV45:MonoBehaviour {
        [Serializable] sealed class Report {
            public bool passed,entered,differentNaturalPoses,damaged,returned;
            public int identities,actions,exposures;public string mode,error="",last="";public int originalCameraDraws;
            public int routeIdentities,routeActions,routeExposures;
            public bool controlledSourceState,naturallyObservedSourceAttack;public string requestedState="";
            public int birthIndex,cachePeakAfterEviction,cacheTarget=4;
            public double maxResourceDecodeMs,proofIntervalMs;
            public string lastSourceObservation="";
            public List<Shot> shots=new List<Shot>();
        }
        [Serializable] sealed class Shot {
            public string label,state,clip,bodySprite,texture,sourceMotion,originalSignedKey;public float clock,hp,sourceClock;public PixelProof pixels;
            public int unityFrame,decodedWidth,decodedHeight;public long decodedRGBABytes;
            public double realtime,proofReadbackSeconds;
        }
        readonly Report report=new Report();string dest,kind,route,region="bethma",controlClip,controlMotion;bool elder,privateRoute,coreRoute;DemoScene scene;GameModel game;Enemy actor;SpriteRenderer view;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,"--source-motion-v45");
            if(f<0)return;
            try{
                int o=Array.IndexOf(args,"--source-motion-output-v45");Need(f+1<args.Length&&o>=0&&o+1<args.Length,"Motion mode/output");
                var mode=args[f+1];Need(Array.IndexOf(new[]{"road","night","nest","water","cave","caveboss","forest","center","crossbow","guard","monkey","thief","fire","mineboss","cargoboss","raven","wisp","coreking","corefire"},mode)>=0,"Motion mode");
                string path=Path.GetFullPath(args[o+1]);
                string parent=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(parent,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh motion output");
                Directory.CreateDirectory(path);
                var c=new GameObject("Besma original motion inspection").AddComponent<SourceMotionPlayerCheckV45>();
                c.dest=path;c.report.mode=mode;
                switch(mode){
                    case "road":c.kind="lizardman-saurus";c.route="dragon-road";break;
                    case "night":c.kind="lizardman-spear-dark";c.route="besma-lake-night";break;
                    case "nest":c.kind="scorpion-fire";c.route="besma-dragon-nest";break;
                    case "water":c.kind="soldier-fat";c.route="underground-waterway";c.elder=true;break;
                    case "cave":c.kind="thief-fat";c.route="banthus-cave";c.elder=true;break;
                    case "caveboss":c.kind="thief-fat-boss";c.route="banthus-cave";c.elder=true;break;
                    case "forest":c.kind="anger-forest-knight";c.route="dark-forest";c.elder=true;break;
                    case "center":c.kind="wally-8th";c.route="wally-castle-center";c.elder=true;c.controlClip="attack-special";break;
                    case "crossbow":c.kind="soldier-crossbow";c.route="wally-castle-center";c.elder=true;c.controlClip="attack-a-ready";break;
                    case "guard":c.kind="soldier-benders";c.route="wally-castle-center";c.elder=true;c.controlClip="attack-b";break;
                    case "monkey":c.kind="monkey-bomb";c.route="wally-castle-outskirts";c.elder=true;c.controlClip="attack-ready";break;
                    case "thief":c.kind="pporu-william-thief";c.route="wally-castle-outskirts";c.elder=true;c.controlClip="attack-ready";break;
                    case "fire":c.kind="pporu-william-thief-fire";c.route="wally-castle-outskirts";c.elder=true;c.controlClip="attack-ready";break;
                    case "mineboss":c.kind="wally-8th-mk2";c.route="bethma-richie-mine";c.privateRoute=true;c.controlClip="get-up";break;
                    case "cargoboss":c.kind="wally-9th-armor";c.route="bethma-cargo-airship";c.privateRoute=true;c.controlClip="source-attack-c-ready";break;
                    case "wisp":c.kind="curse-wisp";c.route="dark-forest";c.elder=true;c.controlClip="attack-a-ready";break;
                    case "coreking":c.kind="nasod-king";c.route="altera-core-hard";c.region="altera";c.coreRoute=true;break;
                    case "corefire":c.kind="nasod-element-fire-b";c.route="altera-core-hard";c.region="altera";c.coreRoute=true;c.controlMotion="AttackB_Fire";break;
                    case "raven":c.kind="raven";c.route="altera-battle-airship";c.privateRoute=true;c.region="altera";c.controlClip="attack-special-ready";break;
                }
                if(c.elder)c.region="elder";
            }catch(Exception e){Debug.LogError(e);Application.Quit(2);}
        }
        void Log(string msg,string trace,LogType type){
            if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert)report.error=msg+"\n"+trace;
        }
        IEnumerator Start(){
            bool bg=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;
            double end=Time.realtimeSinceStartupAsDouble+155;var work=Flow();
            try{
                while(true){
                    bool more=false;object next=null;
                    try{Need(report.error==""&&Time.realtimeSinceStartupAsDouble<end,"Motion test "+report.last+" "+report.error);DrawCoreOriginalCamera();more=work.MoveNext();if(more)next=work.Current;}
                    catch(Exception e){report.error=e.ToString();}
                    if(!more||report.error!="")break;yield return next;
                }
            }finally{(work as IDisposable)?.Dispose();Application.logMessageReceived-=Log;Application.runInBackground=bg;}
            report.passed=report.error==""&&report.differentNaturalPoses&&(report.damaged||report.controlledSourceState||report.naturallyObservedSourceAttack)&&report.returned;
            File.WriteAllText(Path.Combine(dest,"source-motion-player.json"),JsonUtility.ToJson(report,true));Application.Quit(report.passed?0:2);
        }
        void DrawCoreOriginalCamera(){
            if(!coreRoute||!scene||game==null||actor==null||!game.enemies.Contains(actor)||
                !scene.CoreHardProductSessionActiveR1147||!scene.CoreHardProductAuthoredCameraActiveR1147)return;
            var cam=(Camera)Field(scene,"worldCamera");
            Need(cam&&cam.enabled&&cam.targetTexture==null,"Original Core camera with its authored null-target");
            var prior=RenderTexture.active;try{cam.Render();report.originalCameraDraws++;}finally{RenderTexture.active=prior;}
        }
        bool OriginalMotionBody(){
            if(!view||!view.enabled||view.forceRenderingOff||!view.sprite||!view.gameObject.activeInHierarchy)return false;
            return coreRoute?scene.ConfirmCoreMotionFrameV45(actor,view,out _,out _,out _):scene.IsBesmaMotionV45(actor,view);
        }
        string DescribeCore(){
            if(!coreRoute)return "";
            var cam=(Camera)Field(scene,"worldCamera");
            return " birth="+game.enemies.IndexOf(actor)+" ai="+actor.aiEnabled+" initializedState="+actor.npcState+"@"+actor.npcTime+
                " model="+game.elapsed+" paused="+game.paused+" actor="+actor.x+","+actor.y+" hero="+game.player.x+","+game.player.y+
                " camera="+cam.transform.position+" body="+(view&&view.sprite?view.sprite.name:"<none>")+
                " visible="+(view&&view.enabled&&!view.forceRenderingOff)+" frustum="+(view&&GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam),view.bounds));
        }
        void Place(){
            if(coreRoute){
                game.player.x=actor.x-35;game.player.y=actor.y;game.player.groundLine=actor.groundLine;
                game.player.vx=game.player.vy=0;game.player.invincible=120;return;
            }
            float x=actor.x+(actor.x>game.CurrentRoom.width*.5f?-1:1)*48;
            var floor=game.SurfaceBelow(x,actor.y+90,true);if(floor==null)return;
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;game.player.invincible=120;
        }
        bool Snap(string label,bool previousOwner=false){
            var cam=(Camera)Field(scene,"worldCamera");
            var actual=previousOwner?scene.ActualElderHitBodyV45(actor,view):view;
            if(!actual||!actual.enabled||actual.forceRenderingOff||!actual.sprite||(!previousOwner&&!OriginalMotionBody())||
                !GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam),actual.bounds))return false;
            double at=Time.realtimeSinceStartupAsDouble;
            var shot=CaptureAtResolution(cam,game,actor,new List<SpriteRenderer>{actual},dest,label,1280,720,false);
            if(shot.count<=0)return false;
            var tex=actual.sprite.texture;
            string source=null,key=null;float sourceTime=0;
            if(coreRoute)Need(scene.ConfirmCoreMotionFrameV45(actor,actual,out source,out key,out sourceTime),"Observed source key and registered sprite");
            report.shots.Add(new Shot{label=label,state=actor.npcState,clip=actor.clip,clock=coreRoute?actor.npcTime:actor.clock,hp=actor.hp,
                bodySprite=actual.sprite.name,texture=tex.name,pixels=shot,unityFrame=Time.frameCount,realtime=at,
                decodedWidth=tex.width,decodedHeight=tex.height,decodedRGBABytes=4L*tex.width*tex.height,
                sourceMotion=source,originalSignedKey=key,sourceClock=sourceTime,proofReadbackSeconds=Time.realtimeSinceStartupAsDouble-at});
            if(coreRoute){
                scene.CoreMotionCacheMetricV45(kind,out report.cachePeakAfterEviction,out report.maxResourceDecodeMs);
                if(report.shots.Count==2)report.proofIntervalMs=(at-report.shots[0].realtime)*1000;
            }
            return true;
        }
        IEnumerator Flow(){
            while(!(scene=FindObjectOfType<DemoScene>())||Field(scene,"game")==null||(string)Field(scene,"openMenu")!="dungeons")yield return null;
            scene.VerifyBesmaMotionCatalogV45(out report.identities,out report.actions,out report.exposures);
            Need(report.identities>=27&&report.actions>=209,"All expected catalog banks");
            foreach(var t in EnterRoute(scene,region,route))yield return t;
            if(coreRoute){
                while(!scene.CoreHardProductSessionActiveR1147||scene.CoreHardProductSessionLoadingR1147)yield return null;
            }
            if(privateRoute){
                while(report.mode=="mineboss"&&!scene.Mine2ProductSessionActiveR1233||
                      report.mode=="cargoboss"&&!scene.CargoProductSessionActiveR1288||
                      report.mode=="raven"&&!scene.BattleProductSessionActiveR1325)yield return null;
                scene.VerifyBesmaMotionCatalogV45(out report.routeIdentities,out report.routeActions,out report.routeExposures);
                Need(report.routeIdentities>0,"Original private product actually loaded a source-motion library");
            }
            game=(GameModel)Field(scene,"game");report.entered=true;
            var rooms=(List<RoomDefinition>)Field(game,"rooms");int row=report.mode=="coreking"?4:report.mode=="corefire"?2:report.mode=="raven"?3:report.mode=="cargoboss"?2:rooms.FindIndex(r=>r.enemies.Exists(e=>e.kind==kind));
            Need(row>=0,"Natural source birth of "+kind);
            if(game.room!=row)Call(game,"EnterRoom",row);
            int encounter=report.mode=="corefire"?1:rooms[row].enemies.Find(e=>e.kind==kind).encounterIndex;
            if(game.EncounterIndex!=encounter)Call(game,"BeginEncounter",encounter);
            Call(scene,"RebuildRoom");if(!coreRoute)for(int i=0;i<5;i++)yield return null;
            actor=report.mode=="corefire"?game.enemies[30]:game.enemies.Find(e=>e.kind==kind&&e.encounterIndex==encounter);
            Need(actor!=null&&actor.kind==kind&&actor.encounterIndex==encounter,"Original encounter actor");report.birthIndex=game.enemies.IndexOf(actor);
            view=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];Place();
            bool naturalKing=report.mode=="coreking";
            if(!naturalKing&&(controlClip!=null||controlMotion!=null)){
                report.last="selecting original source-state fixture";
                double ready=Time.realtimeSinceStartupAsDouble+50;
                // The cargo movie owns the armored boss until its one source release. Starting
                // an attack while the actor is visually present but AI-disabled invalidates that ticket.
                while(!actor.active||actor.dormant||String.IsNullOrEmpty(actor.npcState)||(!coreRoute&&scene.BossIntroductionActive)||
                    (report.mode=="cargoboss"&&(game.CargoDirectorSourcePhaseV21!="source-director-ended"||!actor.aiEnabled))||
                    (report.mode=="corefire"&&(!actor.aiEnabled||actor.npcState=="NASOD_ELEMENT_FIRE_SOLO_B_START"))){
                    if(coreRoute)report.lastSourceObservation=DescribeCore();
                    Need(Time.realtimeSinceStartupAsDouble<ready,"Original source actor active before fixture"+report.lastSourceObservation);yield return null;
                }
                var profile=(NpcDefinition)Call(game,"NpcProfile",actor);
                var selected=controlMotion!=null?Array.Find(profile.states,s=>s.sourceAnimation==controlMotion):Array.Find(profile.states,s=>s.clip==controlClip)??
                    Array.Find(profile.states,s=>s.clip=="source-"+controlClip);
                Need(selected!=null,"Missing original source branch "+kind+"/"+controlClip);
                report.controlledSourceState=true;report.requestedState=selected.id;
                Call(game,"EnterNpcState",actor,selected);
            }
            report.last="observing ordinary attack poses "+kind;
            Rect first=default;Texture texture=null;double until=Time.realtimeSinceStartupAsDouble+(naturalKing?100:60);
            while(!report.differentNaturalPoses){
                if(coreRoute)report.lastSourceObservation=DescribeCore();
                Need(Time.realtimeSinceStartupAsDouble<until,"Two visible natural poses: "+actor.npcState+"/"+actor.clip+report.lastSourceObservation);yield return null;
                if(Time.frameCount%20==0)Place();
                if(!OriginalMotionBody())continue;
                if(naturalKing&&texture!=null&&(actor.npcState!=report.requestedState||actor.npcTime<report.shots[0].clock-.00001f)){
                    texture=null;report.shots.Clear();
                }
                if(texture==null){if(Snap("01-natural")){
                    first=view.sprite.rect;texture=view.sprite.texture;
                    if(naturalKing){report.naturallyObservedSourceAttack=true;report.requestedState=actor.npcState;}
                }}
                else if((view.sprite.rect!=first||view.sprite.texture!=texture)&&Snap("02-natural-next"))report.differentNaturalPoses=true;
            }
            if(report.controlledSourceState||report.naturallyObservedSourceAttack){
                Need(report.shots.TrueForAll(s=>s.state==report.requestedState),"Requested source branch actually drawn");
                if(kind==BluePoruInkV45.Kind&&Array.IndexOf(Environment.GetCommandLineArgs(),"--blue-poru-ink-v45")>=0)
                    foreach(var draw in BluePoruCheckV45())yield return draw;
                Press(scene,"dungeons");for(int i=0;i<4;i++)yield return null;
                report.returned=(string)Field(scene,"openMenu")=="dungeons";yield break;
            }
            report.last="public damage and displayed body";until=Time.realtimeSinceStartupAsDouble+38;
            while(!report.damaged){
                Need(Time.realtimeSinceStartupAsDouble<until,"Original damage reaction: "+actor.npcState+"/"+actor.clip);yield return null;Place();
                if(actor.state==CombatState.Normal&&actor.invincible<=0&&!game.EnemyHasNpcInvincibility(actor)&&
                    !game.EnemyHasSuperArmor(actor)&&game.ApplyHitToEnemy(actor,1,HitReaction.Knockdown)>0&&actor.state!=CombatState.Normal)break;
            }
            while(!report.damaged){
                Need(Time.realtimeSinceStartupAsDouble<until,"Visible down / recovery "+actor.clip);yield return null;
                if(actor.state!=CombatState.Normal&&Snap("03-knockdown-or-rise",elder))report.damaged=true;
            }
            Press(scene,"dungeons");for(int i=0;i<4;i++)yield return null;
            report.returned=(string)Field(scene,"openMenu")=="dungeons";
        }
    }
}
#endif
