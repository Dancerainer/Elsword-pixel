#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class FemalePixelCheckV53 {public bool passed,sourceRootProjectionEquivalent,sourceClockEquivalent,bodyYawEquivalenceClaim;public int roots,frames,continuousFrames,projectionCases,gpuMarkerPairs;public float maxViewportError,maxMarkerPixelError;public string error;}
        public static void ReviewFemalePixelProjectionV53(){
            var args=Environment.GetCommandLineArgs();int flag=Array.IndexOf(args,"--female-pixel-review");string output=Path.GetFullPath(flag>=0&&flag+1<args.Length?args[flag+1]:"Reports/monster-pixel-v53/female-projection");Directory.CreateDirectory(output);
            var report=CheckFemalePixelProjectionV53(output);EditorApplication.Exit(report.passed?0:1);
        }
        static FemalePixelCheckV53 CheckFemalePixelProjectionV53(string output){
            Directory.CreateDirectory(output);var report=new FemalePixelCheckV53();GameObject fixture=null;
            void Need(bool ok,string reason){if(!ok)throw new InvalidOperationException("Female pixel projection: "+reason);}
            try{
                fixture=new GameObject("Female six-root projection fixture");fixture.SetActive(false);var scene=fixture.AddComponent<DemoScene>();
                var actor=new Enemy{sourceNPC="NUI_CHICKEN_FEMALE",kind="chicken-female",active=true,passive=true,hp=1,maxHp=1,visualScale=1,facing=1};
                Need(scene.MonsterPixelRegisteredV53(actor,out var definition)&&definition.assembly=="female-six-root","Six-root production registration required");
                using(var gpu=new MonsterPixelGpuCheckV53(output)){
                    scene.actorsRoot=scene.worldRoot=gpu.Anchor;scene.worldCamera=gpu.Camera;
                    scene.nativeRoot=new GameObject("Native original coordinate root").transform;
                    scene.nativeScreen=new GameObject("Native pixel viewport rectangle");scene.nativeScreen.transform.SetParent(gpu.Anchor);scene.nativeScreen.transform.position=new Vector3(0,123,10);scene.nativeScreen.transform.localScale=new Vector3(512,256,1);
                    scene.nativeCamera=new GameObject("Native projection comparison camera").AddComponent<Camera>();var sourceCamera=scene.nativeCamera;
                    sourceCamera.enabled=false;sourceCamera.orthographic=true;sourceCamera.aspect=2;sourceCamera.nearClipPlane=.1f;sourceCamera.farClipPlane=10000;sourceCamera.clearFlags=CameraClearFlags.SolidColor;sourceCamera.backgroundColor=Color.clear;sourceCamera.cullingMask=1<<30;sourceCamera.allowHDR=sourceCamera.allowMSAA=false;
                    scene.nativeTarget=new RenderTexture(512,256,24){antiAliasing=1,filterMode=FilterMode.Point};scene.nativeTarget.Create();sourceCamera.targetTexture=scene.nativeTarget;
                    scene.nativeScene=JsonUtility.FromJson<NativeScene>(Resources.Load<TextAsset>("Pixel/native-world/tree-2/scene").text);
                    var carrier=gpu.Carrier();var owner=scene.LoadFemalePixelV53(actor,carrier,scene.game.CurrentRoom,definition);scene.femalePixelOwnerV53=owner;
                    for(int i=0;i<6;i++){owner.views[i]=new MonsterPixelViewV53(carrier,Object.DestroyImmediate);scene.monsterPixelRenderersV53.Add(owner.views[i].renderer);}
                    report.roots=6;report.frames=owner.assembly.frameCount;
                    foreach(var library in owner.libraries){
                        Need(library.TryClip("WaitStart",out var clip),"Root has no original complete motion");
                        for(int i=0;i<clip.frames.Length;i++){
                            var sprite=library.At("WaitStart",Mathf.Min(clip.duration,i/60f),out int actual,out int page);
                            Need(sprite&&actual==i,"Root did not expose every 60 Hz frame "+library.actor.kind+"/"+i);report.continuousFrames++;
                            if(library.ResidentPages>2){library.Trim(new System.Collections.Generic.HashSet<int>{page},0);EditorUtility.UnloadUnusedAssetsImmediate();}
                        }
                        library.Trim(new System.Collections.Generic.HashSet<int>(),0);EditorUtility.UnloadUnusedAssetsImmediate();
                    }
                    var marker=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,1,1),new Vector2(.5f,.5f),1);var markerMaterial=new Material(Shader.Find("Sprites/Default"));var nativeMarkers=new SpriteRenderer[6];var worldMarkers=new SpriteRenderer[6];
                    var colors=new[]{Color.red,Color.green,Color.blue,Color.yellow,Color.magenta,Color.cyan};
                    for(int i=0;i<6;i++){
                        nativeMarkers[i]=new GameObject("Original native root marker "+i){layer=30}.AddComponent<SpriteRenderer>();nativeMarkers[i].sprite=marker;nativeMarkers[i].sharedMaterial=markerMaterial;nativeMarkers[i].color=colors[i];
                        worldMarkers[i]=gpu.Carrier();worldMarkers[i].sprite=marker;worldMarkers[i].sharedMaterial=markerMaterial;worldMarkers[i].color=colors[i];worldMarkers[i].enabled=false;
                    }
                    var readback=new Texture2D(512,256,TextureFormat.RGBA32,false);
                    try{
                        foreach(float time in new[]{0f,.5f,2.9f,6.7f,owner.assembly.duration})foreach(float yaw in new[]{0f,31f,79f}){
                            actor.clock=time;Need(scene.TrySelectFemalePixelV53(actor,carrier,scene.game.CurrentRoom),"Six source bodies failed selection");owner=scene.femalePixelOwnerV53;
                            int expected=time+.00001f>=owner.assembly.duration?owner.assembly.frameCount-1:Mathf.FloorToInt(time*60+.0001f);Vector3 center=Vector3.zero;foreach(var point in owner.nativePoints)center+=point/6;
                            sourceCamera.transform.rotation=Quaternion.Euler(0,yaw,0);sourceCamera.transform.position=center-sourceCamera.transform.forward*2200;float half=128/.28f;
                            foreach(var point in owner.nativePoints){var local=sourceCamera.transform.InverseTransformPoint(point)-new Vector3(0,0,2200);half=Mathf.Max(half,Mathf.Abs(local.x)/2*1.2f,Mathf.Abs(local.y)*1.2f);}sourceCamera.orthographicSize=half;
                            scene.nativeSceneDepthReadyR1461=true;scene.ProjectFemalePixelV53();scene.nativeSceneDepthReadyR1461=false;
                            for(int i=0;i<6;i++){
                                Need(owner.indices[i]==expected,"Root/source exposure index changed");var body=owner.views[i].renderer;var reference=sourceCamera.WorldToViewportPoint(owner.nativePoints[i]);var actual=gpu.Camera.WorldToViewportPoint(body.transform.position);
                                float error=Vector2.Distance(reference,actual);report.maxViewportError=Mathf.Max(report.maxViewportError,error);Need(error<.00001f,"Native root position/spacing differs");
                                var block=new MaterialPropertyBlock();body.GetPropertyBlock(block);Need(Mathf.Abs(block.GetFloat("_NativeActorEyeDepthR1461")-reference.z)<.001f,"Native root eye depth differs");
                                float sourceHeight=owner.assembly.roots[i].neutralBodyHeight/owner.assembly.sourceLogicalUnit;
                                float oldHeight=sourceHeight/(2*half),newHeight=owner.assembly.roots[i].neutralBodyHeight*body.transform.lossyScale.y/(2*gpu.Camera.orthographicSize);Need(Mathf.Abs(oldHeight-newHeight)<.000001f,"Root body scale changed");
                                nativeMarkers[i].transform.position=owner.nativePoints[i];nativeMarkers[i].transform.rotation=sourceCamera.transform.rotation;nativeMarkers[i].transform.localScale=Vector3.one*(2*half/256*5);nativeMarkers[i].sortingOrder=i;
                                worldMarkers[i].transform.position=body.transform.position;worldMarkers[i].transform.localScale=Vector3.one*5;worldMarkers[i].sortingOrder=i;worldMarkers[i].enabled=false;body.enabled=false;report.projectionCases++;
                            }
                            for(int i=0;i<6;i++){
                                for(int j=0;j<6;j++){nativeMarkers[j].enabled=j==i;worldMarkers[j].enabled=j==i;}
                                sourceCamera.Render();var prior=RenderTexture.active;RenderTexture.active=scene.nativeTarget;readback.ReadPixels(new Rect(0,0,512,256),0,0);readback.Apply();RenderTexture.active=prior;var nativePixels=readback.GetPixels32();var worldPixels=gpu.CapturePixels();
                                Color32 color=colors[i];Vector2 Centroid(Color32[] pixels){Vector2 sum=Vector2.zero;int count=0;for(int p=0;p<pixels.Length;p++)if(pixels[p].Equals(color)){sum+=new Vector2(p%512,p/512);count++;}Need(count>0,"Root marker not drawn "+i);return sum/count;}
                                float error=Vector2.Distance(Centroid(nativePixels),Centroid(worldPixels));report.maxMarkerPixelError=Mathf.Max(report.maxMarkerPixelError,error);Need(error<=1.01f,"GPU native/projected root marker differs");report.gpuMarkerPairs++;
                            }
                            for(int i=0;i<6;i++){worldMarkers[i].enabled=false;owner.views[i].renderer.enabled=true;}
                            gpu.CapturePixels("female-six-roots-t"+time.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture)+"-yaw"+yaw);
                        }
                        report.sourceRootProjectionEquivalent=report.sourceClockEquivalent=report.passed=true;
                    }finally{foreach(var m in nativeMarkers)if(m)Object.DestroyImmediate(m.gameObject);foreach(var m in worldMarkers)if(m)Object.DestroyImmediate(m.gameObject);Object.DestroyImmediate(marker);Object.DestroyImmediate(markerMaterial);Object.DestroyImmediate(readback);}
                    scene.DisposeFemalePixelV53();Object.DestroyImmediate(carrier.gameObject);scene.worldCamera=null;scene.actorsRoot=scene.worldRoot=null;
                }
            }catch(Exception error){report.error=error.ToString();Debug.LogException(error);}
            finally{if(fixture)Object.DestroyImmediate(fixture);File.WriteAllText(Path.Combine(output,"female-projection-check.json"),JsonUtility.ToJson(report,true));}
            return report;
        }
    }
}
#endif
