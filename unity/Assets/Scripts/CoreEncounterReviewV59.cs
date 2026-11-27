#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    // Opt-in Windows Player regression. The actor placements and finishing hits
    // accelerate the fixture; source callbacks and all animation clocks run normally.
    public sealed class CoreEncounterReviewV59 : MonoBehaviour {
        internal string output;
        bool finished;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        internal static void Attach() {
            var args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(args, "--core-review-v59");
            if (index < 0 || index + 1 >= args.Length) return;
            var run = new GameObject("Core encounter review").AddComponent<CoreEncounterReviewV59>();
            run.output = Path.GetFullPath(args[index + 1]);
            Directory.CreateDirectory(run.output);
            Application.runInBackground = true;
        }

        IEnumerator Start() {
            Record("Review started");
            DemoScene scene;
            while (!(scene = FindFirstObjectByType<DemoScene>())) yield return null;
            var work = new Stack<IEnumerator>();
            work.Push(scene.ReviewCoreEncounterV59(this));
            Application.logMessageReceived += OnLog;
            while (!finished && work.Count > 0) {
                object next = null;
                try {
                    var part = work.Peek();
                    if (!part.MoveNext()) { work.Pop(); continue; }
                    next = part.Current;
                } catch (Exception ex) { Finish(ex.ToString()); yield break; }
                if (next is IEnumerator inner) work.Push(inner);
                else yield return next;
            }
            if (!finished) Finish(null);
        }

        void OnLog(string message, string trace, LogType type) {
            if (type == LogType.Exception || type == LogType.Assert || type == LogType.Error) Finish(message + "\n" + trace);
        }
        void Finish(string error) {
            if (finished) return;
            finished = true;
            Application.logMessageReceived -= OnLog;
            File.WriteAllText(Path.Combine(output, error == null ? "passed.txt" : "failure.txt"), error ?? "passed");
            Application.Quit(error == null ? 0 : 1);
        }
        internal void Record(string message) {
            File.AppendAllText(Path.Combine(output,"phase-log.txt"), message + Environment.NewLine);
        }
    }

    public sealed partial class GameModel {
        internal void EnterCoreReviewRoomV59(int index, int encounter) {
            EnterRoom(index);
            if (EncounterIndex != encounter) BeginEncounter(encounter);
        }
        internal AttackBox CoreReviewHurtboxV59(Enemy e) {
            if (SourceEnemyBounds(e, out var box) && !float.IsInfinity(box.right)) return box;
            return EnemyBlockingBounds(e);
        }
        internal string CheckCoreArenaPassageV59(Enemy actor,float platformY,float halfSpan) {
            float left=actor.x-halfSpan, right=actor.x+halfSpan;
            var start=SurfaceBelow(left,platformY+1);
            var end=SurfaceBelow(right,platformY+1);
            var center=SurfaceBelow(actor.x,platformY+1);
            if (!NativeTerrain||start==null||end==null||center==null||
                Math.Abs(start.Y(left)-platformY)>.1f||Math.Abs(end.Y(right)-platformY)>.1f||
                Math.Abs(center.Y(actor.x)-platformY)>.1f)
                throw new InvalidOperationException("Missing original platform across "+actor.sourceNPC);
            float forward=ClipPlayerBodyMovement(left,start.Y(left),right,end.Y(right));
            float reverse=ClipPlayerBodyMovement(right,end.Y(right),left,start.Y(left));
            if (Math.Abs(forward-right)>.01f||Math.Abs(reverse-left)>.01f)
                throw new InvalidOperationException("Arena body blocked an open path: "+actor.sourceNPC);
            return actor.kind+" traversable in both directions across lines "+start.id+"/"+center.id+"/"+end.id+
                ", x="+left+".."+right+", y="+center.Y(actor.x);
        }
    }

    public sealed partial class DemoScene {
        static readonly string[] coreReviewElementsV59 = {
            "NUI_EL_EXTRACTOR_FIRE", "NUI_EL_EXTRACTOR_ICE", "NUI_EL_EXTRACTOR_EARTH", "NUI_EL_EXTRACTOR_NOVA"
        };

        internal void EnterCoreReviewRoomV59(int room, int encounter) {
            game.EnterCoreReviewRoomV59(room,encounter);
            RebuildRoom();
        }
        internal void CaptureCoreReviewFrameV59(string path) {
            if(Application.isBatchMode) {
                var image=CoreReviewWorldFrameV59();
                try { File.WriteAllBytes(path,image.EncodeToPNG()); } finally { Destroy(image); }
                return;
            }
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            if (!texture) throw new InvalidOperationException("No rendered frame for " + path);
            try { File.WriteAllBytes(path, texture.EncodeToPNG()); }
            finally { Destroy(texture); }
        }
        Texture2D CoreReviewWorldFrameV59() {
            var previous=worldCamera.targetTexture;
            var oldActive=RenderTexture.active;
            var target=RenderTexture.GetTemporary(960,540,24,RenderTextureFormat.ARGB32);
            try {
                worldCamera.targetTexture=target;worldCamera.Render();RenderTexture.active=target;
                var result=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);
                result.ReadPixels(new Rect(0,0,target.width,target.height),0,0);result.Apply();return result;
            } finally {
                worldCamera.targetTexture=previous;RenderTexture.active=oldActive;RenderTexture.ReleaseTemporary(target);
            }
        }
        void CheckCoreCrystalPixelsV59(CoreEncounterReviewV59 run) {
            var plant=CoreReviewLivingV59("NUI_EL_POWER_PLANT");
            if(!monsterPixelViewsV53.TryGetValue(plant,out var body)||!body.renderer.enabled)
                throw new InvalidOperationException("Exposed crystal has no active body");
            var normal=CoreReviewWorldFrameV59(); Texture2D absent=null;
            try {
                body.renderer.enabled=false;absent=CoreReviewWorldFrameV59();
                var pixels=normal.GetPixels32();var background=absent.GetPixels32();
                int total=0,green=0;
                for(int i=0;i<pixels.Length;i++) {
                    var p=pixels[i];if(p.Equals(background[i]))continue;
                    total++;if(p.g>p.r+12&&p.g>p.b+12)green++;
                }
                run.Record("Exposed crystal contributed "+total+" screen pixels, including "+green+" green pixels.");
                CheckCoreReviewV59(green>0,"The visible green weak point is occluded");
            } finally {
                body.renderer.enabled=true;Destroy(normal);if(absent)Destroy(absent);
            }
        }
        static void CheckCoreReviewV59(bool condition,string message) {
            if (!condition) throw new InvalidOperationException(message);
        }
        Enemy CoreReviewLivingV59(string id) {
            return game.enemies.Find(e => e.sourceNPC == id && e.active && !e.dormant && e.hp > 0 && e.encounterIndex == game.EncounterIndex);
        }
        IEnumerator CoreReviewWaitV59(Func<bool> condition,string reason,float seconds=25) {
            double end = Time.realtimeSinceStartupAsDouble + seconds;
            while (!condition()) {
                if (Time.realtimeSinceStartupAsDouble > end) throw new TimeoutException(reason);
                game.player.invincible = 60;
                yield return null;
            }
        }
        IEnumerator CoreReviewCameraV59(CoreEncounterReviewV59 run,string phase) {
            yield return new WaitForEndOfFrame();
            var camera = VerifyCoreBossCameraV59(phase);
            File.WriteAllText(Path.Combine(run.output,phase+".json"),JsonUtility.ToJson(camera,true));
            CaptureCoreReviewFrameV59(Path.Combine(run.output,phase+".png"));
            if(phase=="core-exposed") CheckCoreCrystalPixelsV59(run);
        }
        IEnumerator CoreReviewStrikeV59(CoreEncounterReviewV59 run, Enemy target, bool upperPlatform) {
            var box=game.CoreReviewHurtboxV59(target);
            float x = target.x < game.RoomWidth*.5f ? box.right+17 : box.left-17;
            x=Mathf.Clamp(x,21,game.RoomWidth-21);
            var floor=game.SurfaceBelow(x,upperPlatform ? box.top : target.y+15);
            CheckCoreReviewV59(floor!=null,"No original platform beside "+target.kind);
            game.player.x=x; game.player.y=floor.Y(x)+1; game.player.groundLine=floor.id;
            game.player.vx=game.player.vy=0; game.player.facing=target.x<x?-1:1;
            game.player.invincible=60;
            float hp=target.hp; double until=Time.realtimeSinceStartupAsDouble+10, next=0;
            while(target.hp>=hp) {
                if(Time.realtimeSinceStartupAsDouble>until)
                    throw new TimeoutException("Normal attack missed "+target.kind+" in "+target.npcState+" player "+game.player.x+","+game.player.y+
                        " source bounds "+box.left+","+box.bottom+".."+box.right+","+box.top+" line "+floor.id+" command "+game.CurrentCommand);
                if(Time.realtimeSinceStartupAsDouble>=next) {
                    Action("attack");next=Time.realtimeSinceStartupAsDouble+.16;
                }
                yield return null;
            }
            run.Record("Normal attack from platform "+floor.id+" hit "+target.kind+" "+hp+" -> "+target.hp+" at player height "+game.player.y);
        }
        IEnumerator CoreReviewDevicesV59(CoreEncounterReviewV59 run,Enemy plant,bool ordinary) {
            for(int i=0;i<coreReviewElementsV59.Length;i++) {
                var device=CoreReviewLivingV59(coreReviewElementsV59[i]);
                CheckCoreReviewV59(device!=null,"Missing live original device "+coreReviewElementsV59[i]);
                if(ordinary) yield return CoreReviewStrikeV59(run,device,false);
                if(device.hp>0) game.ApplyHitToEnemy(device,1000000,HitReaction.Hurt);
                CheckCoreReviewV59(device.hp<=0,"Device could not be destroyed "+device.kind);
                yield return null;
                if(i+1<coreReviewElementsV59.Length) {
                    CheckCoreReviewV59(plant.npcState=="EL_POWER_PLANT_WAIT_DOWN" && game.ApplyHitToEnemy(plant,1)==0,
                        "Core exposed before fourth device");
                }
            }
        }
        internal IEnumerator ReviewCoreEncounterV59(CoreEncounterReviewV59 run) {
            yield return null;
            run.Record("Selecting Core from menu");
            Action("dungeons");yield return null;
            int at=mapDungeons.FindIndex(d=>d.id=="altera-core-hard");
            CheckCoreReviewV59(at>=0,"Core missing from menu");
            MenuAction("region:"+mapDungeons[at].region);RefreshMapVisible();
            int slot=mapVisible.IndexOf(at);
            CheckCoreReviewV59(slot>=0,"Core card hidden");
            mapPage=slot/MapPageSize;MenuAction("dungeon-card:"+slot%MapPageSize);MenuAction("dungeon-enter");
            run.Record("Requested core dungeon");
            yield return CoreReviewWaitV59(()=>adventureStarted&&openMenu==""&&Dungeon.id=="altera-core-hard"&&!forestPreviewLoading,"Core product menu entry",120);
#if !UNITY_EDITOR
            CheckCoreReviewV59(CoreHardProductSessionActiveR1147,"Player failed to load ordinary Core session");
#endif
            bool skipLasers=Array.IndexOf(Environment.GetCommandLineArgs(),"--core-boss-only-v59")>=0;
            run.Record("Ordinary Core session loaded");
            if(!skipLasers) yield return ReviewCoreLasersV59(run.output);
            EnterCoreReviewRoomV59(4,0);
            yield return null;yield return null;
            yield return CoreReviewWaitV59(()=>!BossIntroductionActive,"Boss entry intro");
            var plant=CoreReviewLivingV59("NUI_EL_POWER_PLANT");
            var king=CoreReviewLivingV59("NUI_NASOD_KING");
            CheckCoreReviewV59(plant!=null&&king!=null,"Boss room roster missing");
            yield return CoreReviewWaitV59(()=>plant.npcState=="EL_POWER_PLANT_WAIT_DOWN","Initial hidden core");
            run.Record(game.CheckCoreArenaPassageV59(king,plant.y,30));
            run.Record(game.CheckCoreArenaPassageV59(plant,plant.y,65));
            yield return CoreReviewCameraV59(run,"devices-intact");
            yield return CoreReviewDevicesV59(run,plant,true);
            yield return CoreReviewWaitV59(()=>plant.npcState=="EL_POWER_PLANT_WAIT_UP","First core exposure");
            CheckCoreReviewV59(king.npcState.Contains("KNOCKDOWN"),"King remained active on exposure");
            yield return CoreReviewStrikeV59(run,plant,true);
            float afterStrike=game.elapsed;
            yield return CoreReviewWaitV59(()=>game.elapsed-afterStrike>1 && plant.impactFlash<=0,"Crystal after hit flash");
            yield return CoreReviewCameraV59(run,"core-exposed");
            float hp=plant.hp;
            yield return CoreReviewWaitV59(()=>CoreReviewLivingV59(coreReviewElementsV59[0])!=null,"First device respawn",70);
            foreach(var id in coreReviewElementsV59)
                CheckCoreReviewV59(game.enemies.FindAll(e=>e.sourceNPC==id&&e.hp>0&&e.active).Count==1,"Replacement count "+id);
            yield return CoreReviewWaitV59(()=>plant.npcState=="EL_POWER_PLANT_WAIT_DOWN","Core retracts");
            CheckCoreReviewV59(plant.hp==hp&&game.ApplyHitToEnemy(plant,1)==0,"Retracted core failed to keep health or protection");
            yield return CoreReviewCameraV59(run,"devices-restored");
            yield return CoreReviewDevicesV59(run,plant,false);
            yield return CoreReviewWaitV59(()=>plant.npcState=="EL_POWER_PLANT_WAIT_UP","Second core exposure");
            CheckCoreReviewV59(game.ApplyHitToEnemy(plant,1000000)>0&&plant.hp<=0,"Core lethal hit failed");
            float death=game.elapsed;bool kingAliveAtFive=false;
            while(king.hp>0){
                if(game.elapsed-death>=5)kingAliveAtFive=true;
                CheckCoreReviewV59(game.elapsed-death<9&&!game.victory,"King death callback missing or victory premature");
                yield return null;
            }
            CheckCoreReviewV59(kingAliveAtFive&&!game.victory,"King died before the original core death callback");
            run.Record("King death after core lethal hit "+(game.elapsed-death)+" simulation seconds");
            yield return CoreReviewWaitV59(()=>game.victory,"Core automatic clear");
            run.Record("Four devices, normal core hit, respawn, second exposure and victory passed.");
            Action("dungeons");
            yield return null;yield return null;
            CheckCoreReviewV59(CoreBossCameraRestoredV59(),"Boss camera survived exit");
        }
    }
}
#endif
