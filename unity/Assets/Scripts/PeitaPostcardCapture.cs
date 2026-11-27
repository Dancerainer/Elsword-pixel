#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        // Editor-only artwork capture. Each frame is taken from the native scenery
        // camera and composited through the same pixel backdrop as gameplay.
        public static void CapturePeitaPostcards() {
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            EditorApplication.isPlaying = true;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void BeginPeitaPostcardCapture() {
            var args=Environment.GetCommandLineArgs();
            int flag=Array.IndexOf(args,"--region-card-scenes");
            if(flag<0)flag=Array.IndexOf(args,"--peita-card-scenes");
            if(flag<0 || flag+1>=args.Length)return;
            var scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>();
            scene.StartCoroutine(GuardPeitaCheck(scene.CapturePeitaCardScenes(Path.GetFullPath(args[flag+1]))));
        }

        bool capturingRegionalScenery;
        IEnumerator CapturePeitaCardScenes(string output) {
            capturingRegionalScenery=true;
            Directory.CreateDirectory(output);
            yield return null;
            var enter=typeof(GameModel).GetMethod("EnterRoom",BindingFlags.Instance|BindingFlags.NonPublic);
            var args=Environment.GetCommandLineArgs();
            int flag=Array.IndexOf(args,"--region");
            string region=flag>=0 && flag+1<args.Length ? args[flag+1] : "peita";
            MenuAction("dungeons");
            MenuAction("region:"+region);
            int routeCount=mapVisible.Count;
            if(routeCount==0)throw new InvalidOperationException("Empty region "+region);
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--postcards-menu-only")>=0) {
                yield return null;
                CaptureOffscreenFrame(Path.Combine(output,"menu.png"));
                EditorApplication.Exit(0);
                yield break;
            }
            for(int slot=0;slot<routeCount;slot++) {
                for(int page=0;page<slot/MapPageSize;page++)MenuAction("dungeon-page:1");
                MenuAction("dungeon-card:"+(slot%MapPageSize));
                int count=mapDungeons[mapSelected].count;
                MenuAction("dungeon-enter");
                for(int room=0;room<count;room++) {
                    if(room>0) {
                        enter.Invoke(game,new object[]{room});
                        RestoreNativeProjection();
                        RebuildRoom();
                    }
                    yield return null;
                    if(!NativeWorldActive||nativeScene.id!=CurrentStageArt.sourceWorldId)
                        throw new InvalidOperationException("Native postcard scenery is absent in "+CurrentStageArt.id);
                    var pos=worldCamera.transform.position;
                    foreach(float fraction in new[]{.22f,.52f}) {
                        float x=Mathf.Clamp(game.RoomWidth*fraction,180,game.RoomWidth-180);
                        float y=game.GroundAt(game.CurrentRoom,x)+67;
                        worldCamera.transform.position=new Vector3(x,y,pos.z);
                        UpdateNativeCamera(true);
                        WritePeitaCardScenery(Path.Combine(output,nativeScene.id+"-"+Mathf.RoundToInt(fraction*100)+".png"));
                    }
                    worldCamera.transform.position=pos;
                }
                MenuAction("dungeons");
                MenuAction("region:"+region);
            }
            EditorApplication.Exit(0);
        }

        void WritePeitaCardScenery(string path) {
            const int layer=27;
            var target=RenderTexture.GetTemporary(480,270,24,RenderTextureFormat.ARGB32);
            var previousTarget=worldCamera.targetTexture;
            var previousActive=RenderTexture.active;
            var rect=worldCamera.rect;
            int mask=worldCamera.cullingMask, oldLayer=nativeScreen.layer;
            var pixels=new Texture2D(480,270,TextureFormat.RGB24,false);
            try {
                nativeScreen.layer=layer;
                worldCamera.cullingMask=1<<layer;
                worldCamera.rect=new Rect(0,0,1,1);
                worldCamera.targetTexture=target;
                worldCamera.Render();
                RenderTexture.active=target;
                pixels.ReadPixels(new Rect(0,0,480,270),0,0);
                pixels.Apply();
                File.WriteAllBytes(path,pixels.EncodeToPNG());
            } finally {
                nativeScreen.layer=oldLayer;
                worldCamera.cullingMask=mask;
                worldCamera.targetTexture=previousTarget;
                worldCamera.rect=rect;
                RenderTexture.active=previousActive;
                RenderTexture.ReleaseTemporary(target);
                Destroy(pixels);
            }
        }
    }
}
#endif
