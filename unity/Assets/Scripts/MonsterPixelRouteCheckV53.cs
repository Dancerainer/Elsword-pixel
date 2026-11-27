#if UNITY_EDITOR || ROOT_MONSTER_PIXEL_CHECK_V53
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    // CLI-only controlled route coverage. The normal Scene Update retains all
    // Steps. Room/encounter selection and player placement are explicit fixture
    // setup, so this report never claims a natural clear or native equivalence.
    public sealed class MonsterPixelRouteCheckV53:MonoBehaviour {
        [Serializable] public sealed class DrawRow {public string route,room,npc,kind,state,motion,provider,sprite;public int frames,rootParts,dynamicFrames,sourceFrame;public float firstSourceClock,lastSourceClock;}
        [Serializable] public sealed class RouteRow {public string route,error;public int rooms,encounters,actorsObserved,dynamicActorsObserved,peakResidentPages,privateWholeDeclared,privateWholeDeferredAtEntry,privateWholeResidentAtEntry,privateWholeDeferredAtEnd,privateWholeResidentAtEnd;public long peakResidentBytes;public bool entered,controlledFixturesFinished,supplemental;}
        [Serializable] public sealed class DynamicProbeRow {public string route,room,owner;public float hpBefore,hpAfter;public int ordinaryAttackPresses,sourceSpawnEvents;public bool targetConditionReached;public string[] newActorNpcs;}
        [Serializable] public sealed class SourceDamageSnapshot {public string playerClip,playerState,command,phase,contextNpc,contextState,contextCombat,targetNpc,targetState;public float playerX,playerY,playerClock,contextX,contextY,contextClock,contextHp,contextInvincible,targetHp,targetX,targetY;public bool grounded,paused,victory,navigating;public int playerGroundLine,contextGroundLine,activeHitBoxes,targetFacing,playerFacing;public AttackBox targetBounds;public AttackBox[] attackBoxes;}
        [Serializable] public sealed class SourcePlacementProof {public string npc,command;public float x,releaseY,sourceFloorY;public int facing,settleFrames;public bool sawUngrounded,landedNormally;}
        [Serializable] public class SourceDamageProbe {public int ordinaryAttackPresses;public List<SourceDamageSnapshot> snapshots=new List<SourceDamageSnapshot>();public List<SourcePlacementProof> placements=new List<SourcePlacementProof>();}
        [Serializable] public sealed class FortProbeRow:SourceDamageProbe {public float armorHpBefore,armorHpAfter,topCannonHpBefore,topCannonHpAfter,bottomCannonHpBefore,bottomCannonHpAfter;public int sourceEvents,closedAssemblySortingChecks;public bool closedAssemblyOnly,armorDestroyed,cannonsDestroyed,allRequiredPartsDrawn,bothOriginalHiddenEyesDrawn,topCannonAttackDrawn,miniUpDrawn;public string[] observedStates;}
        [Serializable] public sealed class CorePlantProbeRow:SourceDamageProbe {public string[] extractorNpcs;public float[] hpBefore,hpAfter;public string plantState;public bool allFourDestroyed,sourceUpObserved,plantDrawn,plantWaitUpDrawn;}
        [Serializable] public sealed class Report {
            public string schema="monster-pixel-v53-controlled-route-runtime",error,rosterSha256;
            public bool passed,coverageComplete,naturalClear,supplemental,controlledRoomFixtures=true,sourceClocksAdvancedByNormalScene=true,syntheticEnemySpawns=false,coverArtifactDrawsSuppressed=true;
            public int suppressedCoverDrawCallbacks;
            public int expectedRoutes=20,expectedActors=105,expectedHostileParts=7,expectedHostileDevices=22,expectedEventIdentities=6;
            public int peakResidentPages;public long peakResidentBytes;
            public long initialUnityAllocatedBytes,finalUnityAllocatedBytes;public int initialUnityObjects,finalUnityObjects,deferredLegacyLibraries,residentLegacyClips,builtLegacySprites;
            public List<RouteRow> routes=new List<RouteRow>();public List<DrawRow> draws=new List<DrawRow>();public string[] limitations;
            public List<DynamicProbeRow> dynamicProbes=new List<DynamicProbeRow>();
            public List<FortProbeRow> fortProbes=new List<FortProbeRow>();
            public List<CorePlantProbeRow> corePlantProbes=new List<CorePlantProbeRow>();
            public List<DemoScene.CoreArenaObservationV21> coreArena=new List<DemoScene.CoreArenaObservationV21>();public float coreArenaMaxBoundsError,coreArenaMaxRendererDepthPadding;
            public List<string> supplementalFailures=new List<string>();
        }
        internal readonly Report report=new Report();internal string output;internal bool suppressDrawWitness;DemoScene scene;bool done;double started;RenderTexture reviewTarget;Texture2D reviewReadback;readonly Dictionary<string,DrawRow> draws=new Dictionary<string,DrawRow>();readonly HashSet<Enemy> originalBirths=new HashSet<Enemy>();
        readonly HashSet<Enemy> observedActors=new HashSet<Enemy>(),observedDynamic=new HashSet<Enemy>();internal RouteRow current;
        internal static readonly string[] Routes={"tree","ruins","swamp","dark-forest","banthus-cave","underground-waterway","wally-castle-outskirts","wally-castle-center","dragon-road","besma-lake","besma-lake-night","besma-dragon-nest","bethma-richie-mine","bethma-cargo-airship","altera-plain","altera-plain-recycle","altera-battle-airship","altera-below-tunnel","altera-nasod-factory","altera-core-hard"};
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int flag=Array.IndexOf(args,"--monster-pixel-route-review");if(flag<0)return;
            if(flag+1>=args.Length)throw new ArgumentException("--monster-pixel-route-review requires output");
            var run=new GameObject("Monster pixel actual route observer V53").AddComponent<MonsterPixelRouteCheckV53>();run.output=Path.GetFullPath(args[flag+1]);Directory.CreateDirectory(run.output);Application.runInBackground=true;
        }
        IEnumerator Start(){
            started=Time.realtimeSinceStartupAsDouble;while(!scene){scene=Object.FindFirstObjectByType<DemoScene>();yield return null;}
            report.rosterSha256=scene.MonsterPixelRosterShaV53;
            report.initialUnityAllocatedBytes=UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();report.initialUnityObjects=Resources.FindObjectsOfTypeAll<UnityEngine.Object>().Length;
            Application.logMessageReceived+=Log;var work=scene.ReviewMonsterPixelRoutesV53(this);Exception error=null;
            while(true){object currentYield=null;bool more=false;try{more=work.MoveNext();if(more)currentYield=work.Current;}catch(Exception failure){error=failure;}if(error!=null||!more)break;yield return currentYield;}
            (work as IDisposable)?.Dispose();Finish(error);
        }
        void Log(string text,string stack,LogType type){if((type==LogType.Error||type==LogType.Exception||type==LogType.Assert)&&report.error==null)report.error=text+"\n"+stack;}
        internal void Budget(){if(report.error!=null)throw new InvalidOperationException(report.error);if(Time.realtimeSinceStartupAsDouble-started>3600)throw new TimeoutException("Controlled 20-route wall budget exceeded");}
        internal void BeginRoute(string route){observedActors.Clear();observedDynamic.Clear();originalBirths.Clear();current=new RouteRow{route=route};report.routes.Add(current);}
        internal void SampleMemory(int pages,long bytes){report.peakResidentPages=Math.Max(report.peakResidentPages,pages);report.peakResidentBytes=Math.Max(report.peakResidentBytes,bytes);if(current!=null){current.peakResidentPages=Math.Max(current.peakResidentPages,pages);current.peakResidentBytes=Math.Max(current.peakResidentBytes,bytes);}}
        internal void BeginRoom(GameModel model){originalBirths.Clear();foreach(var actor in model.enemies)originalBirths.Add(actor);}
        internal void Draw(Enemy actor,Renderer renderer,Camera camera,int frame){
            if(suppressDrawWitness){report.suppressedCoverDrawCallbacks++;return;}
            if(done||current==null||!scene.MonsterPixelReviewWorldCameraV53(camera)||!actor.active||actor.dormant)return;
            var sprite=renderer as SpriteRenderer;if(!scene.TryMonsterPixelReviewSelectionV53(actor,sprite,out var motion,out var provider,out float clock,out int sourceFrame))return;
            scene.RequireMonsterPixelReviewProviderV53(actor,provider);
            observedActors.Add(actor);if(!originalBirths.Contains(actor))observedDynamic.Add(actor);current.actorsObserved=observedActors.Count;current.dynamicActorsObserved=observedDynamic.Count;
            string image=sprite.sprite.name;string key=current.route+"/"+scene.MonsterPixelReviewRoomV53+"/"+actor.sourceNPC+"/"+actor.npcState+"/"+image;
            if(!draws.TryGetValue(key,out var row)){row=new DrawRow{route=current.route,room=scene.MonsterPixelReviewRoomV53,npc=actor.sourceNPC,kind=actor.kind,state=actor.npcState,sprite=image,motion=motion,provider=provider,sourceFrame=sourceFrame,firstSourceClock=clock};draws.Add(key,row);report.draws.Add(row);}row.lastSourceClock=clock;row.frames++;row.rootParts=actor.sourceNPC=="NUI_CHICKEN_FEMALE"?6:1;if(!originalBirths.Contains(actor))row.dynamicFrames++;
        }
        internal void Watch(Enemy actor,SpriteRenderer renderer){if(!renderer)return;var witness=renderer.GetComponent<MonsterPixelRouteDrawWitnessV53>();if(!witness)witness=renderer.gameObject.AddComponent<MonsterPixelRouteDrawWitnessV53>();witness.owner=this;witness.actor=actor;witness.body=renderer;}
        internal void RenderView(){
            if(!reviewTarget){int width=scene.MonsterPixelReviewWidthV53,height=scene.MonsterPixelReviewHeightV53;reviewTarget=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};reviewTarget.Create();reviewReadback=new Texture2D(width,height,TextureFormat.RGBA32,false);}
            // Batch PlayMode has no automatic Game-view camera pass. Request a
            // real pass from the same ordinary world camera after normal Update;
            // this changes no source/model clock and uses the current projection.
            scene.RenderMonsterPixelReviewV53(reviewTarget);
        }
        internal void CaptureView(string name){if(!reviewTarget)return;var prior=RenderTexture.active;RenderTexture.active=reviewTarget;reviewReadback.ReadPixels(new Rect(0,0,reviewTarget.width,reviewTarget.height),0,0);reviewReadback.Apply();RenderTexture.active=prior;File.WriteAllBytes(Path.Combine(output,name+".png"),reviewReadback.EncodeToPNG());}
        internal Color32[] ReadViewV58(){
            RenderView();var prior=RenderTexture.active;
            try{RenderTexture.active=reviewTarget;reviewReadback.ReadPixels(new Rect(0,0,reviewTarget.width,reviewTarget.height),0,0);reviewReadback.Apply();return reviewReadback.GetPixels32();}
            finally{RenderTexture.active=prior;}
        }
        internal void Save(){if(scene){scene.WriteMonsterPixelCoverageV53(Path.Combine(output,"observed-selectors.json"));scene.ReadLegacyCarrierMemoryV53(out report.deferredLegacyLibraries,out report.residentLegacyClips,out report.builtLegacySprites);}File.WriteAllText(Path.Combine(output,"route-check.json"),JsonUtility.ToJson(report,true));}
        void Finish(Exception error){if(done)return;done=true;Application.logMessageReceived-=Log;if(error!=null){report.error=error.ToString();if(current!=null)current.error=report.error;}if(report.error==null&&report.supplementalFailures.Count>0)report.error=string.Join("\n",report.supplementalFailures);if(report.error==null&&draws.Count==0)report.error="No current whole body reached the actual world-camera draw callback";report.passed=report.error==null;report.coverageComplete=false;report.finalUnityAllocatedBytes=UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();report.finalUnityObjects=Resources.FindObjectsOfTypeAll<UnityEngine.Object>().Length;report.limitations=new[]{"Controlled original room/encounter fixtures; not a natural clear.","Headless rendering explicitly requests the ordinary world camera after normal Scene Update. Draw rows require that camera's actual callback and the same selected source sprite/clock. Offscreen and source-hidden actors may have no draw rows.","Dynamic actors are counted only when the normal model adds new Enemy objects after room initialization; zero observations do not prove summon coverage.","The production RebuildRoom enables SimpleDangerIntroductions. The original Battle/Cargo/Dragon/Swamp/Core movies and Core/Factory START focus owners are disabled by that policy; source assets are retained, not force-enabled by this harness.","The inventory keeps 134 actor/device identities and six event identities. The six events and removed white-giant birth require explicit inactive/not-applicable classification; missing reachable identities, states and dynamic actors remain incomplete regardless of this harness exit code."};Save();if(reviewTarget){reviewTarget.Release();Destroy(reviewTarget);}if(reviewReadback)Destroy(reviewReadback);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(report.passed?0:1);
#else
            Application.Quit(report.passed?0:1);
#endif
        }
    }
    public sealed class MonsterPixelRouteDrawWitnessV53:MonoBehaviour {
        internal MonsterPixelRouteCheckV53 owner;internal Enemy actor;internal SpriteRenderer body;
        void OnWillRenderObject(){if(owner&&body&&body.enabled&&!body.forceRenderingOff)owner.Draw(actor,body,Camera.current,Time.frameCount);}
    }
    public sealed partial class DemoScene {
        [Serializable] sealed class FortCoverPartV53 {public string npc,kind,state,motion,provider,sprite,sourceShowReason,spriteSortPoint,sortingGroup;public bool visible,sourceVisible,rendererEnabled,rendererActive;public int frame,sortingLayerID,sortingOrder,groupSortingLayerID,groupSortingOrder;public Vector2 spritePivotPixels;public float pixelsPerUnit,rendererAlpha;public float clock,actorX,actorY,nativeDepth,scale,rootOffsetX,rootOffsetY;public Vector3 projectedPosition,worldScale;public Quaternion rotation;}
        [Serializable] sealed class FortCoverCaptureV53 {public string schema="monster-pixel-v53-actual-seven-part-cover",route,room;public bool originalSelectedSprites=true,originalProjectedPartTransforms=true,cameraReframedOnly=true,sceneOcclusionRemoved=true,hiddenPartsPreserved=true,combatTintRemoved=true,gameplayChanged=false;public FortCoverPartV53[] parts;public int sharedRenderFrame;public Vector3 cameraPosition;public float orthographicSize;}
        bool fortCoverCapturedV53;
        bool CaptureFortAssemblyV53(MonsterPixelRouteCheckV53 observer,string stem="fort-seven-part-transparent",bool cover=true){
            if(cover&&fortCoverCapturedV53||Dungeon.id!="altera-below-tunnel"||!worldCamera.orthographic)return false;
            var keep=new HashSet<Renderer>();var rows=new List<FortCoverPartV53>();Bounds bounds=default;bool stableBody=false;
            foreach(var pair in monsterPixelSelectionsV53){var actor=pair.Key;var selected=pair.Value;if(!actor.kind.StartsWith("cannon-fort-parasite-",StringComparison.Ordinal)||!monsterPixelViewsV53.TryGetValue(actor,out var view))continue;
                if(!game.TryBelowTunnelPresentationV21(actor,out var sourceShow))return false;
                var renderer=view.renderer;bool visible=cover?sourceShow.visible:renderer.enabled&&renderer.gameObject.activeInHierarchy&&renderer.color.a>0;if(cover&&visible!=renderer.enabled)return false;
                if(visible){if(renderer.color.a<.999f)return false;var visibility=new MaterialPropertyBlock();renderer.GetPropertyBlock(visibility);if(visibility.GetFloat("_BurialClip")>.5f)return false;}
                if(actor.kind=="cannon-fort-parasite-body"&&(selected.motion=="Wait"||selected.motion=="WaitHabit"))stableBody=true;
                if(visible){if(keep.Count==0)bounds=renderer.bounds;else bounds.Encapsulate(renderer.bounds);keep.Add(renderer);}
                var group=renderer.GetComponentInParent<UnityEngine.Rendering.SortingGroup>();
                rows.Add(new FortCoverPartV53{npc=actor.sourceNPC,kind=actor.kind,state=actor.npcState,motion=selected.motion,provider=selected.provider,sprite=renderer.sprite.name,visible=visible,sourceVisible=sourceShow.visible,rendererEnabled=renderer.enabled,rendererActive=renderer.gameObject.activeInHierarchy,rendererAlpha=renderer.color.a,sourceShowReason=sourceShow.source,frame=selected.frame,spritePivotPixels=renderer.sprite.pivot,pixelsPerUnit=renderer.sprite.pixelsPerUnit,clock=selected.clock,actorX=actor.x,actorY=actor.y,nativeDepth=actor.nativeDepth,scale=selected.definition.scale,rootOffsetX=selected.definition.rootOffsetX,rootOffsetY=selected.definition.rootOffsetY,projectedPosition=renderer.transform.position,worldScale=renderer.transform.lossyScale,rotation=renderer.transform.rotation,sortingLayerID=renderer.sortingLayerID,sortingOrder=renderer.sortingOrder,spriteSortPoint=renderer.spriteSortPoint.ToString(),sortingGroup=group?group.name:"",groupSortingLayerID=group?group.sortingLayerID:0,groupSortingOrder=group?group.sortingOrder:0});
            }
            if(keep.Count==0||cover&&(rows.Count!=7||!stableBody))return false;
            // Export the seven actual selected SpriteRenderers at their current
            // projected transforms. Only camera framing, other scene renderers,
            // and environmental depth clipping are suppressed for the cover.
            var disabled=new List<Renderer>();var blocks=new Dictionary<Renderer,MaterialPropertyBlock>();
            var cameraPosition=worldCamera.transform.position;float size=worldCamera.orthographicSize,aspect=worldCamera.aspect;var target=worldCamera.targetTexture;var clear=worldCamera.clearFlags;var color=worldCamera.backgroundColor;var active=RenderTexture.active;
            var image=new RenderTexture(1024,1024,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Texture2D pixels=null;
            try{
                foreach(var renderer in Object.FindObjectsOfType<Renderer>()){if(keep.Contains(renderer)){var saved=new MaterialPropertyBlock();renderer.GetPropertyBlock(saved);blocks.Add(renderer,saved);var clean=new MaterialPropertyBlock();renderer.GetPropertyBlock(clean);clean.SetFloat("_NativeSceneClipR1461",0);clean.SetFloat("_Outline",0);clean.SetFloat("_HitFlash",0);renderer.SetPropertyBlock(clean);}else if(renderer.enabled){renderer.enabled=false;disabled.Add(renderer);}}
                image.Create();worldCamera.targetTexture=image;worldCamera.clearFlags=CameraClearFlags.SolidColor;worldCamera.backgroundColor=Color.clear;worldCamera.aspect=1;worldCamera.orthographicSize=Math.Max(bounds.extents.x,bounds.extents.y)*1.08f;worldCamera.transform.position=new Vector3(bounds.center.x,bounds.center.y,cameraPosition.z);observer.suppressDrawWitness=true;worldCamera.Render();observer.suppressDrawWitness=false;
                RenderTexture.active=image;pixels=new Texture2D(1024,1024,TextureFormat.RGBA32,false);pixels.ReadPixels(new Rect(0,0,1024,1024),0,0);pixels.Apply();
                File.WriteAllBytes(Path.Combine(observer.output,stem+".png"),pixels.EncodeToPNG());
                var report=new FortCoverCaptureV53{route=Dungeon.id,room=game.CurrentRoom.id,parts=rows.ToArray(),sharedRenderFrame=Time.frameCount,cameraPosition=worldCamera.transform.position,orthographicSize=worldCamera.orthographicSize};if(!cover)report.schema="monster-pixel-v54-fort-actual-visible-components";File.WriteAllText(Path.Combine(observer.output,stem+".json"),JsonUtility.ToJson(report,true));if(cover)fortCoverCapturedV53=true;
            }finally{observer.suppressDrawWitness=false;worldCamera.targetTexture=target;worldCamera.clearFlags=clear;worldCamera.backgroundColor=color;worldCamera.aspect=aspect;worldCamera.orthographicSize=size;worldCamera.transform.position=cameraPosition;RenderTexture.active=active;foreach(var renderer in disabled)if(renderer)renderer.enabled=true;foreach(var pair in blocks)if(pair.Key)pair.Key.SetPropertyBlock(pair.Value);if(pixels)Object.Destroy(pixels);image.Release();Object.Destroy(image);}
            return true;
        }
        internal bool MonsterPixelReviewWorldCameraV53(Camera value){return value==worldCamera;}
        internal int MonsterPixelReviewWidthV53 {get{return Math.Max(1,worldCamera.pixelWidth);}}
        internal int MonsterPixelReviewHeightV53 {get{return Math.Max(1,worldCamera.pixelHeight);}}
        internal void RenderMonsterPixelReviewV53(RenderTexture target){var prior=worldCamera.targetTexture;try{worldCamera.targetTexture=target;worldCamera.Render();}finally{worldCamera.targetTexture=prior;}}
        internal void ReadLegacyCarrierMemoryV53(out int deferred,out int clips,out int sprites){deferred=clips=sprites=0;var active=new List<MobileClip>();foreach(var library in libraries.Values)if(library.frames.open!=null){deferred++;library.frames.Collect(active);}clips=active.Count;foreach(var clip in active)foreach(var sprite in clip.frames)if(sprite)sprites++;}
        internal string MonsterPixelReviewRoomV53 {get{return game.CurrentRoom.id;}}
        internal void RequireMonsterPixelReviewProviderV53(Enemy actor,string provider){
            if(MonsterPixelRegisteredV53(actor,out var definition)&&definition.directSourceProviders!=null&&definition.directSourceProviders.Length>0&&Array.IndexOf(definition.directSourceProviders,provider)<0)
                throw new InvalidOperationException("Whole body bypassed its reviewed original direct source selector: "+actor.kind+"/"+provider);
        }
        internal bool TryMonsterPixelReviewSelectionV53(Enemy actor,SpriteRenderer renderer,out string motion,out string provider,out float clock,out int index){
            motion=provider="";clock=0;index=-1;if(!renderer||!renderer.sprite)return false;
            if(monsterPixelSelectionsV53.TryGetValue(actor,out var selected)&&monsterPixelViewsV53.TryGetValue(actor,out var view)&&view.renderer==renderer&&selected.sprite==renderer.sprite){motion=selected.motion;provider=selected.provider;clock=selected.clock;index=selected.frame;return true;}
            var owner=femalePixelOwnerV53;if(owner!=null&&owner.actor==actor&&owner.selectedFrame==Time.frameCount)for(int i=0;i<6;i++)if(owner.views[i].renderer==renderer&&owner.sprites[i]==renderer.sprite){motion="WaitStart";provider="Female/NativeRoot"+owner.assembly.roots[i].rootIndex;clock=owner.sourceClock;index=owner.indices[i];return true;}
            return false;
        }
        void SampleMonsterPixelRouteV53(MonsterPixelRouteCheckV53 observer){
            observer.Budget();game.player.invincible=5;
            foreach(var pair in monsterPixelViewsV53)observer.Watch(pair.Key,pair.Value.renderer);
            if(femalePixelOwnerV53!=null)foreach(var view in femalePixelOwnerV53.views)observer.Watch(femalePixelOwnerV53.actor,view.renderer);
            int pages=0;long bytes=0;foreach(var library in monsterPixelLibrariesV53.Values){pages+=library.ResidentPages;bytes+=library.ResidentBytes;}observer.SampleMemory(pages,bytes);observer.RenderView();
        }
        void PlaceMonsterPixelReviewPlayerV53(Enemy target,float gap){
            game.player.x=target.x+gap;game.player.y=target.y;game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;game.player.facing=gap<0?1:-1;
        }
        void HoldMonsterPixelReviewV53(int pointer,string action,bool down){if(down){if(!touches.ContainsKey(pointer))TouchDown(pointer,action);}else TouchUp(pointer,action);}
        IEnumerator ReviewSupplementSafelyV53(MonsterPixelRouteCheckV53 observer,IEnumerator work){
            Exception failure=null;while(true){bool more=false;object current=null;try{more=work.MoveNext();if(more)current=work.Current;}catch(Exception error){failure=error;}if(failure!=null||!more)break;yield return current;}
            (work as IDisposable)?.Dispose();if(failure!=null){observer.current.error=failure.ToString();observer.report.supplementalFailures.Add(observer.current.route+": "+failure.Message);observer.Save();}
        }
        IEnumerator ReviewSourcePartDamageV53(MonsterPixelRouteCheckV53 observer,MonsterPixelRouteCheckV53.SourceDamageProbe row,Enemy target,Enemy context){
            if(target.hp<=0)yield break;
            var blocking=typeof(GameModel).GetMethod("EnemyBlockingBounds",BindingFlags.Instance|BindingFlags.NonPublic);
            var sourceBounds=typeof(GameModel).GetMethod("SourceEnemyBounds",BindingFlags.Instance|BindingFlags.NonPublic);
            var initial=(AttackBox)blocking.Invoke(game,new object[]{target});var sphereArgs=new object[]{target,null};
            if((bool)sourceBounds.Invoke(game,sphereArgs)){var hurt=(AttackBox)sphereArgs[1];initial.left=Math.Min(initial.left,hurt.left);initial.right=Math.Max(initial.right,hurt.right);}
            // One explicit observer placement outside both the original body
            // blocker and hurt volume. Physics and normal inputs own all motion
            // afterwards; no enemy HP, state, clock or collision edits.
            bool fromRight=Dungeon.id=="altera-core-hard"||initial.left-24<16;float placeX=fromRight?initial.right+24:initial.left-24;
            if(placeX>game.RoomWidth-16){fromRight=false;placeX=initial.left-24;}
            if(placeX<0||placeX>game.RoomWidth)throw new InvalidOperationException("No original in-room source attack placement");
            game.player.x=placeX;game.player.y=target.y+12;game.player.groundLine=target.groundLine;game.player.vx=game.player.vy=0;game.player.facing=fromRight?-1:1;
            var placement=new MonsterPixelRouteCheckV53.SourcePlacementProof{npc=target.sourceNPC,x=placeX,releaseY=game.player.y,sourceFloorY=target.y,facing=game.player.facing};row.placements.Add(placement);
            // Release a small real fall before any attack. Directly placing an
            // existing air command on the floor bypasses its original landing
            // event; never cancel that command or edit its clock privately.
            double settleUntil=Time.realtimeSinceStartupAsDouble+5;
            while(Time.realtimeSinceStartupAsDouble<settleUntil){
                placement.sawUngrounded|=!game.Grounded(game.player);placement.command=game.CurrentCommand;placement.settleFrames++;SampleMonsterPixelRouteV53(observer);
                if(placement.sawUngrounded&&game.Grounded(game.player)&&game.player.state==CombatState.Normal&&string.IsNullOrEmpty(game.CurrentCommand)){placement.landedNormally=true;break;}yield return null;
            }
            observer.Save();if(!placement.landedNormally)throw new InvalidOperationException("Source attack fixture did not complete an ordinary landing with idle command");
            double until=Time.realtimeSinceStartupAsDouble+120,nextAttack=0,nextSave=0,nextJump=0,nextDrop=0;
            try{while(target.hp>0&&Time.realtimeSinceStartupAsDouble<until){
                var input=new Input{skill=-1};float dx=target.x-game.player.x;input.move=Math.Abs(dx)>24?Math.Sign(dx):Math.Abs(dx)>1&&game.player.facing!=Math.Sign(dx)?Math.Sign(dx):0;
                bool navigating=BelowTunnelLowerPathInputV21.TryDrive(Dungeon.id=="altera-below-tunnel",game,input,target);
#if UNITY_EDITOR
                if(!navigating&&Dungeon.id=="altera-core-hard")navigating=CoreNaturalElementDescentV578(input,target);
#endif
                if(!navigating)navigating=game.NativeApproachEnemy(input,target);
                HoldMonsterPixelReviewV53(-5301,"left",input.move<0);HoldMonsterPixelReviewV53(-5302,"right",input.move>0);HoldMonsterPixelReviewV53(-5303,"dash",input.dash);
                double now=Time.realtimeSinceStartupAsDouble;if(input.jump&&now>=nextJump){Action("jump");nextJump=now+.3;}if(input.drop&&now>=nextDrop){Action("drop");nextDrop=now+.35;}
                var box=(AttackBox)blocking.Invoke(game,new object[]{target});var hurtArgs=new object[]{target,null};if((bool)sourceBounds.Invoke(game,hurtArgs))box=(AttackBox)hurtArgs[1];
                float gap=game.player.x<box.left?box.left-game.player.x:game.player.x>box.right?game.player.x-box.right:0;bool vertical=game.player.y+48>=box.bottom&&game.player.y<=box.top+12;
                if(!navigating&&gap<85&&vertical&&now>=nextAttack){Action("attack");row.ordinaryAttackPresses++;nextAttack=now+.2;}
                SampleMonsterPixelRouteV53(observer);
                if(now>=nextSave){var boxes=new List<AttackBox>();foreach(var hit in game.ActiveHitBoxes)boxes.Add(new AttackBox{left=hit.left,right=hit.right,bottom=hit.bottom,top=hit.top,windowIndex=hit.windowIndex,shape=hit.shape});row.snapshots.Add(new MonsterPixelRouteCheckV53.SourceDamageSnapshot{targetNpc=target.sourceNPC,targetState=target.npcState,targetFacing=target.facing,targetX=target.x,targetY=target.y,targetHp=target.hp,playerClip=game.player.clip,playerState=game.player.state.ToString(),command=game.CurrentCommand,phase=game.CurrentPhase,playerX=game.player.x,playerY=game.player.y,playerClock=game.player.clock,playerFacing=game.player.facing,grounded=game.Grounded(game.player),playerGroundLine=game.player.groundLine,activeHitBoxes=game.ActiveHitBoxes.Count,attackBoxes=boxes.ToArray(),contextNpc=context.sourceNPC,contextState=context.npcState,contextCombat=context.state.ToString(),contextX=context.x,contextY=context.y,contextGroundLine=context.groundLine,contextClock=context.npcTime,contextHp=context.hp,contextInvincible=context.invincible,targetBounds=box,paused=game.paused,victory=game.victory,navigating=navigating});observer.Save();nextSave=now+2;}yield return null;
            }}finally{TouchUp(-5301,"left");TouchUp(-5302,"right");TouchUp(-5303,"dash");}
            if(target.hp>0)throw new InvalidOperationException("Normal player attacks did not destroy original source part "+target.sourceNPC+" within fixture budget");
        }
        IEnumerator ReviewCorePlantPixelV53(MonsterPixelRouteCheckV53 observer){
            var plant=game.enemies.Find(e=>e.sourceNPC=="NUI_EL_POWER_PLANT");if(plant==null)throw new InvalidOperationException("Original Core Plant birth missing");
            var row=new MonsterPixelRouteCheckV53.CorePlantProbeRow{extractorNpcs=new[]{"NUI_EL_EXTRACTOR_FIRE","NUI_EL_EXTRACTOR_ICE","NUI_EL_EXTRACTOR_EARTH","NUI_EL_EXTRACTOR_NOVA"},hpBefore=new float[4],hpAfter=new float[4]};observer.report.corePlantProbes.Add(row);
            for(int i=0;i<row.extractorNpcs.Length;i++){
                var actor=game.enemies.Find(e=>e.sourceNPC==row.extractorNpcs[i]&&e.encounterIndex==game.EncounterIndex&&e.hp>0);if(actor==null)throw new InvalidOperationException("Original Core extractor birth missing "+row.extractorNpcs[i]);
                row.hpBefore[i]=row.hpAfter[i]=actor.hp;var damage=ReviewSourcePartDamageV53(observer,row,actor,plant);while(damage.MoveNext())yield return damage.Current;row.hpAfter[i]=actor.hp;observer.Save();
            }
            row.allFourDestroyed=true;double until=Time.realtimeSinceStartupAsDouble+20;
            while(Time.realtimeSinceStartupAsDouble<until){
                PlaceMonsterPixelReviewPlayerV53(plant,28);SampleMonsterPixelRouteV53(observer);row.plantState=plant.npcState;row.sourceUpObserved|=plant.npcState=="EL_POWER_PLANT_UP"||plant.npcState=="EL_POWER_PLANT_WAIT_UP";
                row.plantDrawn=observer.report.draws.Exists(d=>d.route=="altera-core-hard"&&d.kind==plant.kind&&(d.state=="EL_POWER_PLANT_UP"||d.state=="EL_POWER_PLANT_WAIT_UP"));row.plantWaitUpDrawn=observer.report.draws.Exists(d=>d.route=="altera-core-hard"&&d.kind==plant.kind&&d.state=="EL_POWER_PLANT_WAIT_UP");if(row.sourceUpObserved&&row.plantWaitUpDrawn)break;yield return null;
            }
            observer.Save();observer.CaptureView("core-plant-original-extractor-release");if(!row.sourceUpObserved||!row.plantDrawn||!row.plantWaitUpDrawn)throw new InvalidOperationException("Original stable Plant WAIT_UP did not reach ordinary world camera after four extractor deaths");
        }
        IEnumerator ReviewFortCombatPixelV53(MonsterPixelRouteCheckV53 observer){
            var armor=game.enemies.Find(e=>e.sourceNPC=="NUI_CANNON_FORT_PARASITE_BODY_ARMOR");var body=game.enemies.Find(e=>e.sourceNPC=="NUI_CANNON_FORT_PARASITE_BODY");
            if(armor==null||body==null)throw new InvalidOperationException("Actual Fort original armor/body birth missing");
            var row=new MonsterPixelRouteCheckV53.FortProbeRow{armorHpBefore=armor.hp};observer.report.fortProbes.Add(row);var states=new HashSet<string>();int eventsBefore=game.belowFortEventsV20.Count;
            double settle=Time.realtimeSinceStartupAsDouble+3;
            while(Time.realtimeSinceStartupAsDouble<settle){SampleMonsterPixelRouteV53(observer);CaptureFortAssemblyV53(observer);yield return null;}
            if(!fortCoverCapturedV53)throw new InvalidOperationException("Fort stable original SHOW cover was not captured");
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--monster-pixel-fort-closed-review")>=0){
                // Use the same explicit observer placement as the larger route
                // fixture, then let the original world camera follow normally.
                // A reframed transparent cover is never a world-draw witness.
                PlaceMonsterPixelReviewPlayerV53(body,-100);if(MonsterPixelRegisteredV53(body,out var coreRegistration))game.player.y+=coreRegistration.rootOffsetY;
                double followUntil=Time.realtimeSinceStartupAsDouble+.75;while(Time.realtimeSinceStartupAsDouble<followUntil){SampleMonsterPixelRouteV53(observer);yield return null;}
                row.closedAssemblyOnly=true;UnityEngine.Rendering.SortingGroup common=null;
                foreach(var selected in monsterPixelSelectionsV53.Values){if(!FortPixelLayerV54.TryOrder(selected.actor.sourceNPC,selected.actor.kind,selected.motion,out int order))continue;if(!monsterPixelViewsV53.TryGetValue(selected.actor,out var view))throw new InvalidOperationException("Fort closed scene missing actual view");var group=view.renderer.GetComponentInParent<UnityEngine.Rendering.SortingGroup>();if(!group||group.sortingLayerID!=selected.carrier.sortingLayerID||group.sortingOrder!=selected.carrier.sortingOrder||view.renderer.sortingOrder!=order||view.renderer.spriteSortPoint!=SpriteSortPoint.Pivot||common&&common!=group)throw new InvalidOperationException("Fort closed scene does not use exact production sorting group/layers");common=group;row.closedAssemblySortingChecks++;}
                if(row.closedAssemblySortingChecks!=7)throw new InvalidOperationException("Fort closed scene needs all seven original component selections");if(!observer.report.draws.Exists(d=>d.route=="altera-below-tunnel"&&d.kind.StartsWith("cannon-fort-parasite-",StringComparison.Ordinal)))throw new InvalidOperationException("Fort closed scene lacks an ordinary world camera draw");row.armorHpAfter=armor.hp;row.sourceEvents=game.belowFortEventsV20.Count-eventsBefore;observer.CaptureView("fort-closed-original-world");observer.Save();yield break;
            }
            var top=game.enemies.Find(e=>e.sourceNPC=="NUI_CANNON_FORT_PARASITE_TOP_CANNON");var bottom=game.enemies.Find(e=>e.sourceNPC=="NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON");
            if(top==null||bottom==null)throw new InvalidOperationException("Actual Fort original cannons missing");
            row.topCannonHpBefore=row.topCannonHpAfter=top.hp;row.bottomCannonHpBefore=row.bottomCannonHpAfter=bottom.hp;
            // Guarded BODY normally requests both OPEN callbacks at .5 s. Let
            // that original AI run while the ordinary camera follows an
            // explicitly placed observer near the upper visible body. The
            // artifact-only cover render above is excluded from this witness.
            double topWindow=Time.realtimeSinceStartupAsDouble+45,topSave=0;
            while(Time.realtimeSinceStartupAsDouble<topWindow){
                PlaceMonsterPixelReviewPlayerV53(top,28);if(MonsterPixelRegisteredV53(top,out var topRegistration))game.player.y+=topRegistration.rootOffsetY;
                SampleMonsterPixelRouteV53(observer);row.topCannonAttackDrawn=observer.report.draws.Exists(d=>d.route=="altera-below-tunnel"&&d.kind==top.kind&&d.motion=="Attack");
                row.sourceEvents=game.belowFortEventsV20.Count-eventsBefore;if(Time.realtimeSinceStartupAsDouble>=topSave){observer.Save();topSave=Time.realtimeSinceStartupAsDouble+2;}
                if(row.topCannonAttackDrawn)break;yield return null;
            }
            if(!row.topCannonAttackDrawn)throw new InvalidOperationException("Original guarded upper-cannon Attack did not reach the ordinary world camera");
            observer.CaptureView("fort-original-cannon-attack");CaptureFortAssemblyV53(observer,"fort-original-cannon-attack-transparent",false);
            foreach(var target in new[]{bottom,top,armor}){
                var attack=ReviewSourcePartDamageV53(observer,row,target,armor);while(attack.MoveNext())yield return attack.Current;
                row.armorHpAfter=armor.hp;row.topCannonHpAfter=top.hp;row.bottomCannonHpAfter=bottom.hp;observer.Save();
            }
            row.armorDestroyed=armor.hp<=0;row.cannonsDestroyed=top.hp<=0&&bottom.hp<=0;
            if(!row.armorDestroyed||!row.cannonsDestroyed)throw new InvalidOperationException("Original armor/cannon destruction prerequisites missing");
            observer.CaptureView("fort-original-armor-destroyed");bool exposedCoreCaptured=CaptureFortAssemblyV53(observer,"fort-original-armor-destroyed-transparent",false);
            double until,nextSave=0;
            var hidden=new[]{"cannon-fort-parasite-top-cannon","cannon-fort-parasite-top-eye","cannon-fort-parasite-bottom-eye"};
            bool Drawn(string kind){return observer.report.draws.Exists(d=>d.route=="altera-below-tunnel"&&d.kind==kind);}
            bool topEyeCaptured=false,bottomEyeCaptured=false;until=Time.realtimeSinceStartupAsDouble+180;
            while(Time.realtimeSinceStartupAsDouble<until){
                Enemy focus=null;foreach(var actor in game.enemies){if(actor.kind.StartsWith("cannon-fort-parasite-",StringComparison.Ordinal))states.Add(actor.sourceNPC+"/"+actor.npcState);if(actor.hp<=0||!actor.active||actor.dormant||Drawn(actor.kind))continue;
                    if((Array.IndexOf(hidden,actor.kind)>=0||actor.kind=="cannon-mini-up")&&game.TryBelowTunnelPresentationV21(actor,out var shown)&&shown.visible){focus=actor;break;}}
                PlaceMonsterPixelReviewPlayerV53(focus??body,28);
                // Shared-root fort parts may be hundreds of logical units above
                // their actor foot. Observer placement follows their actual
                // registered root only to bring that original view into frame.
                if(focus!=null&&MonsterPixelRegisteredV53(focus,out var registration))game.player.y+=Math.Max(0,registration.rootOffsetY);
                SampleMonsterPixelRouteV53(observer);row.allRequiredPartsDrawn=Array.TrueForAll(hidden,Drawn);row.bothOriginalHiddenEyesDrawn=Drawn("cannon-fort-parasite-top-eye")&&Drawn("cannon-fort-parasite-bottom-eye");row.topCannonAttackDrawn=observer.report.draws.Exists(d=>d.route=="altera-below-tunnel"&&d.kind=="cannon-fort-parasite-top-cannon"&&d.motion=="Attack");row.miniUpDrawn=Drawn("cannon-mini-up");row.sourceEvents=game.belowFortEventsV20.Count-eventsBefore;row.observedStates=new List<string>(states).ToArray();
                if(!exposedCoreCaptured)exposedCoreCaptured=CaptureFortAssemblyV53(observer,"fort-original-armor-destroyed-transparent",false);
                foreach(var pair in monsterPixelSelectionsV53){var actor=pair.Key;var selected=pair.Value;if(selected.motion!="Attack"||!monsterPixelViewsV53.TryGetValue(actor,out var view)||!view.renderer.enabled)continue;if(actor.kind=="cannon-fort-parasite-top-eye"&&!topEyeCaptured)topEyeCaptured=CaptureFortAssemblyV53(observer,"fort-original-top-eye-attack-transparent",false);if(actor.kind=="cannon-fort-parasite-bottom-eye"&&!bottomEyeCaptured)bottomEyeCaptured=CaptureFortAssemblyV53(observer,"fort-original-bottom-eye-attack-transparent",false);}
                if(Time.realtimeSinceStartupAsDouble>=nextSave){observer.Save();nextSave=Time.realtimeSinceStartupAsDouble+2;}if(row.allRequiredPartsDrawn&&row.miniUpDrawn&&topEyeCaptured&&bottomEyeCaptured)break;yield return null;
            }
            observer.Save();observer.CaptureView("fort-after-original-part-callbacks");
            if(!row.allRequiredPartsDrawn||!row.miniUpDrawn||!topEyeCaptured||!bottomEyeCaptured)throw new InvalidOperationException("Fort original callbacks did not expose all required parts/mini-up and both eye Attack captures within fixture budget");
        }
        IEnumerator ReviewDrillerWalkPixelV53(MonsterPixelRouteCheckV53 observer,Enemy actor){
            double until=Time.realtimeSinceStartupAsDouble+2;while(Time.realtimeSinceStartupAsDouble<until){PlaceMonsterPixelReviewPlayerV53(actor,70);SampleMonsterPixelRouteV53(observer);yield return null;}
            until=Time.realtimeSinceStartupAsDouble+30;bool drawn=false;
            while(Time.realtimeSinceStartupAsDouble<until){PlaceMonsterPixelReviewPlayerV53(actor,240);SampleMonsterPixelRouteV53(observer);drawn=observer.report.draws.Exists(d=>d.route=="altera-plain-recycle"&&d.kind==actor.kind&&d.state=="NASOD_DRILLER_CANNON_WALK"&&d.provider=="Plain/Driller");if(drawn)break;yield return null;}
            observer.Save();observer.CaptureView("driller-original-walk");if(!drawn)throw new InvalidOperationException("Original Driller WALK did not reach the actual world camera");
        }
        IEnumerator ReviewMonsterPixelDynamicOwnerV53(MonsterPixelRouteCheckV53 observer,Enemy target){
            if(target==null||!target.active||target.dormant||target.hp<=0)yield break;
            string route=observer.current.route;float fraction=-1;double wait=0;
            if(route=="underground-waterway"&&target.kind=="mouse"){fraction=0;wait=9;}
            else if(route=="besma-dragon-nest"&&target.kind=="lizardman-shaman-king"){fraction=.399f;wait=20;}
            else if(route=="altera-nasod-factory"&&target.kind=="raven-clone"){fraction=.7f;wait=12;}
            else if(target.kind=="parasite-house"||target.kind=="nasod-maker"){wait=12;}
            else yield break;
            var before=new HashSet<Enemy>(game.enemies);int eventsBefore=game.npcSummonEvents.Count;
            var row=new MonsterPixelRouteCheckV53.DynamicProbeRow{route=route,room=game.CurrentRoom.id,owner=target.sourceNPC,hpBefore=target.hp};observer.report.dynamicProbes.Add(row);
            // Damage only through the normal player action queue and Scene Step.
            // Enemy HP, state, clock, summon requests and birth lists are untouched.
            double limit=Time.realtimeSinceStartupAsDouble+60,nextPress=0;
            while(fraction>=0&&target.hp>target.maxHp*fraction&&target.active&&Time.realtimeSinceStartupAsDouble<limit){
                PlaceMonsterPixelReviewPlayerV53(target,-55);if(Time.realtimeSinceStartupAsDouble>=nextPress){Action("attack");row.ordinaryAttackPresses++;nextPress=Time.realtimeSinceStartupAsDouble+.2;}
                SampleMonsterPixelRouteV53(observer);yield return null;
            }
            row.hpAfter=target.hp;row.targetConditionReached=fraction<0||target.hp<=target.maxHp*fraction;
            double until=Time.realtimeSinceStartupAsDouble+wait;
            while(Time.realtimeSinceStartupAsDouble<until){SampleMonsterPixelRouteV53(observer);yield return null;}
            var created=new List<Enemy>();var names=new List<string>();foreach(var actor in game.enemies)if(!before.Contains(actor)){created.Add(actor);names.Add(actor.sourceNPC);}row.newActorNpcs=names.ToArray();
            for(int i=eventsBefore;i<game.npcSummonEvents.Count;i++)if(game.npcSummonEvents[i].kind=="spawn")row.sourceSpawnEvents++;
            foreach(var child in created){if(!child.active||child.dormant||child.hp<=0)continue;PlaceMonsterPixelReviewPlayerV53(child,90);until=Time.realtimeSinceStartupAsDouble+2;while(Time.realtimeSinceStartupAsDouble<until){SampleMonsterPixelRouteV53(observer);yield return null;}observer.CaptureView(route+"-dynamic-"+child.kind+"-"+created.IndexOf(child));}
            observer.Save();
        }
        internal IEnumerator ReviewMonsterPixelRoutesV53(MonsterPixelRouteCheckV53 observer){
            var args=Environment.GetCommandLineArgs();int flag=Array.IndexOf(args,"--monster-pixel-route");var chosen=flag>=0&&flag+1<args.Length?args[flag+1].Split(','):MonsterPixelRouteCheckV53.Routes;
            const BindingFlags flags=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
            var enterRoom=typeof(GameModel).GetMethod("EnterRoom",flags);var beginEncounter=typeof(GameModel).GetMethod("BeginEncounter",flags);
            yield return null;
            foreach(string id in chosen){
                if(Array.IndexOf(MonsterPixelRouteCheckV53.Routes,id)<0)throw new InvalidOperationException("Unknown route "+id);
                observer.BeginRoute(id);var result=observer.current;Action("dungeons");yield return null;
                int index=mapDungeons.FindIndex(d=>d.id==id);if(index<0)throw new InvalidOperationException("Missing actual menu route "+id);
                MenuAction("region:"+mapDungeons[index].region);RefreshMapVisible();int slot=mapVisible.IndexOf(index);if(slot<0)throw new InvalidOperationException("Route absent from actual map "+id);mapPage=slot/MapPageSize;MenuAction("dungeon-card:"+(slot%MapPageSize));MenuAction("dungeon-enter");
                double deadline=Time.realtimeSinceStartupAsDouble+180;
                while(!adventureStarted||openMenu!=""||Dungeon.id!=id||forestPreviewLoading){observer.Budget();if(Time.realtimeSinceStartupAsDouble>deadline)throw new TimeoutException("Actual route entry failed "+id);yield return null;}
                result.entered=true;ReadPrivateWholeMemoryV53(out result.privateWholeDeclared,out result.privateWholeDeferredAtEntry,out result.privateWholeResidentAtEntry);yield return null;
                if(Array.IndexOf(args,"--hover-v58")>=0){
                    var hunt=ReviewHealerHoverV58(observer,enterRoom,beginEncounter);
                    while(hunt.MoveNext())yield return hunt.Current;
                    result.controlledFixturesFinished=true;observer.Save();continue;
                }
                int roomCount=game.RoomCount;
                bool fortOnly=id=="altera-below-tunnel"&&Array.IndexOf(Environment.GetCommandLineArgs(),"--monster-pixel-fort-review")>=0;
                bool drillerOnly=id=="altera-plain-recycle"&&Array.IndexOf(Environment.GetCommandLineArgs(),"--monster-pixel-driller-walk-review")>=0;
                bool plantOnly=id=="altera-core-hard"&&Array.IndexOf(Environment.GetCommandLineArgs(),"--monster-pixel-core-plant-review")>=0;
                result.supplemental=fortOnly||drillerOnly||plantOnly;
                for(int room=fortOnly||drillerOnly||plantOnly?roomCount-1:0;room<roomCount;room++){
                    observer.Budget();enterRoom.Invoke(game,new object[]{room});RebuildRoom();observer.BeginRoom(game);result.rooms++;yield return null;
                    int encounterCount=game.EncounterCount;
                    for(int encounter=0;encounter<encounterCount;encounter++){
                        if(game.room!=room)enterRoom.Invoke(game,new object[]{room});if(game.EncounterIndex!=encounter){beginEncounter.Invoke(game,new object[]{encounter});RebuildRoom();}observer.BeginRoom(game);result.encounters++;
                        if(fortOnly){yield return null;yield return null;while(BossIntroductionActive){observer.Budget();game.player.invincible=5;yield return null;}var fortReview=ReviewSupplementSafelyV53(observer,ReviewFortCombatPixelV53(observer));while(fortReview.MoveNext())yield return fortReview.Current;observer.Save();continue;}
                        if(plantOnly){yield return null;yield return null;while(BossIntroductionActive){observer.Budget();game.player.invincible=5;yield return null;}var plantReview=ReviewSupplementSafelyV53(observer,ReviewCorePlantPixelV53(observer));while(plantReview.MoveNext())yield return plantReview.Current;observer.Save();continue;}
                        if(drillerOnly){var target=game.enemies.Find(e=>e.kind=="nasod-driller-cannon"&&e.active&&!e.dormant);if(target!=null){yield return null;yield return null;while(BossIntroductionActive){observer.Budget();game.player.invincible=5;yield return null;}var walkReview=ReviewSupplementSafelyV53(observer,ReviewDrillerWalkPixelV53(observer,target));while(walkReview.MoveNext())yield return walkReview.Current;}observer.Save();continue;}
                        var targets=new List<Enemy>();var identities=new HashSet<string>();foreach(var actor in game.enemies)if(actor.active&&!actor.dormant&&actor.hp>0&&identities.Add(MonsterPixelIdentityV53(actor.sourceNPC,actor.kind)))targets.Add(actor);
                        if(targets.Count==0)targets.Add(null);
                        foreach(var target in targets){
                            if(target!=null)PlaceMonsterPixelReviewPlayerV53(target,90);
                            // Give the normal Update its first chance to start
                            // DANGER, then observe combat after the overlay ends.
                            yield return null;yield return null;
                            while(BossIntroductionActive){observer.Budget();game.player.invincible=5;yield return null;}
                            double until=Time.realtimeSinceStartupAsDouble+2.1;
                            while(Time.realtimeSinceStartupAsDouble<until){
                                SampleMonsterPixelRouteV53(observer);
                                yield return null;
                            }
                            observer.CaptureView(id+"-room"+room+"-encounter"+encounter+"-"+(target==null?"empty":target.kind));
                            if(id=="altera-core-hard"&&room==4){
                                var arena=CoreArenaObserveV21("v53-final-source-body-"+(target==null?"empty":target.kind));observer.report.coreArena.Add(arena);
                                foreach(var body in arena.actors)if(body.visible&&body.fitPolicy.Contains("current v53 selected raster")){
                                    if(!body.actualProjectedBoundsAvailable)throw new InvalidOperationException("Visible Core pixel root was not projected exactly once");
                                    var lo=body.projectedMin-body.predictedProjectedMin;var hi=body.projectedMax-body.predictedProjectedMax;
                                    float difference=Math.Max(new Vector2(lo.x,lo.y).magnitude,new Vector2(hi.x,hi.y).magnitude);observer.report.coreArenaMaxBoundsError=Math.Max(observer.report.coreArenaMaxBoundsError,difference);
                                    observer.report.coreArenaMaxRendererDepthPadding=Math.Max(observer.report.coreArenaMaxRendererDepthPadding,Math.Max(Math.Abs(lo.z),Math.Abs(hi.z)));
                                }
                                // The existing product close camera may release
                                // the arena fit after prepare. Only an active
                                // arena fit promises simultaneous containment;
                                // exact once-projected bounds are always required.
                                observer.Save();if(arena.active&&!arena.allBodiesContained||observer.report.coreArenaMaxBoundsError>.02f)throw new InvalidOperationException("Current Core pixel bodies do not match the original single projection/active arena containment");
                            }
                            CaptureFortAssemblyV53(observer);
                            var dynamicReview=ReviewMonsterPixelDynamicOwnerV53(observer,target);while(dynamicReview.MoveNext())yield return dynamicReview.Current;
                        }
                        observer.Save();
                    }
                }
                ReadPrivateWholeMemoryV53(out result.privateWholeDeclared,out result.privateWholeDeferredAtEnd,out result.privateWholeResidentAtEnd);result.controlledFixturesFinished=string.IsNullOrEmpty(result.error);observer.Save();
            }
        }
#if UNITY_EDITOR
        public static void StartMonsterPixelRouteReviewV53(){
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--monster-pixel-route-review")<0)throw new ArgumentException("Explicit route review output flag required");
            var scenes=UnityEditor.EditorBuildSettings.scenes;if(scenes.Length==0)throw new InvalidOperationException("No actual build scene");UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenes[0].path);UnityEditor.EditorApplication.EnterPlaymode();
        }
#endif
    }
}
#endif
