#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    // An isolated visual diagnostic. It replays the recorded source selection and
    // transforms; it never creates a GameModel or changes an enemy state.
    public static class MonsterPixelFortCompositeCheckV54 {
        [Serializable] sealed class RecordedPart {public string npc,kind,motion;public bool visible,sourceVisible,rendererEnabled,rendererActive;public int frame;public float clock,scale,rootOffsetY,rendererAlpha;public Vector3 projectedPosition,worldScale;public Quaternion rotation;}
        [Serializable] sealed class Capture {public RecordedPart[] parts;}
        [Serializable] sealed class Part {public string name,texture;public int count,uvOffset;}
        [Serializable] sealed class Frame {public long offset;public float time;}
        [Serializable] sealed class Clip {public string id;public Frame[] frames;}
        [Serializable] sealed class Source {public string geometry,uvs;public Part[] parts;public Clip[] clips;}
        [Serializable] sealed class Pin {public string path,sha256;}
        [Serializable] sealed class ImageRow {public string file,rgbaSha256,policy;public int differingPixels;}
        [Serializable] sealed class Report {public string schema="monster-pixel-v54-fort-composite-diagnostic-1",error;public bool passed,gameplayCreated=false,sourceReferenceIsStyleParity=false;public Pin capture,roster,implementation,sortingPolicy;public Pin[] sources;public ImageRow[] images;public string[] backToFront;public int fixedOrderPermutationCases,recordedComponentInstances,recordedIdentities;}
        static string Arg(string name,string fallback=null){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,name);return i>=0&&i+1<args.Length?args[i+1]:fallback;}
        static Pin FilePin(string path){path=Path.GetFullPath(path);using(var s=File.OpenRead(path))using(var h=SHA256.Create())return new Pin{path=path,sha256=BitConverter.ToString(h.ComputeHash(s)).Replace("-","").ToLowerInvariant()};}
        static string Hash(Color32[] pixels){var bytes=new byte[pixels.Length*4];for(int i=0;i<pixels.Length;i++){bytes[i*4]=pixels[i].r;bytes[i*4+1]=pixels[i].g;bytes[i*4+2]=pixels[i].b;bytes[i*4+3]=pixels[i].a;}using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
        static int Difference(Color32[] a,Color32[] b){int n=0;for(int i=0;i<a.Length;i++)if(a[i].r!=b[i].r||a[i].g!=b[i].g||a[i].b!=b[i].b||a[i].a!=b[i].a)n++;return n;}
        public static void Run(){
            string output=Path.GetFullPath(Arg("--fort-composite-output","Reports/monster-pixel-v54/fort-composite-diagnostic"));Directory.CreateDirectory(output);
            string capturePath=Arg("--fort-composite-capture","Reports/monster-pixel-v54/routes-tree-fort/fort-seven-part-transparent.json");
            string sourceRoot=Path.GetFullPath(Arg("--fort-composite-source","../art/production/lk-native-v02/dungeon/monster-pixel-v53/source-extensions"));
            string captures=Arg("--fort-composite-captures");int exit=0;
            if(String.IsNullOrEmpty(captures)){if(!RunOne(output,capturePath,sourceRoot))exit=1;}
            else foreach(string path in captures.Split(','))if(!RunOne(Path.Combine(output,Path.GetFileNameWithoutExtension(path)),path,sourceRoot))exit=1;
            EditorApplication.Exit(exit);
        }
        static bool RunOne(string output,string capturePath,string sourceRoot){
            Directory.CreateDirectory(output);
            var owned=new List<Object>();var libs=new List<MonsterPixelFramesV53>();var rows=new List<ImageRow>();var pins=new List<Pin>();var report=new Report();int exit=1;
            try{
                report.capture=FilePin(capturePath);report.roster=FilePin("Assets/Resources/Pixel/monster-v53/roster.json");report.implementation=FilePin("Assets/Scripts/MonsterPixelFortCompositeCheckV54.cs");
                report.sortingPolicy=FilePin("Assets/Scripts/DemoSceneFortPixelSortingV54.cs");
                var capture=JsonUtility.FromJson<Capture>(File.ReadAllText(capturePath));if(capture.parts==null||capture.parts.Length<7||capture.parts.Length>32)throw new InvalidOperationException("Recorded original assembly components required");report.recordedComponentInstances=capture.parts.Length;
                var roster=JsonUtility.FromJson<MonsterPixelRosterV53>(File.ReadAllText(report.roster.path));
                var identities=new HashSet<string>();var visibleIdentities=new HashSet<string>();var sorted=new List<RecordedPart>(capture.parts);foreach(var part in sorted){if(!FortPixelLayerV54.TryOrder(part.npc,part.kind,part.motion,out _))throw new InvalidOperationException("Unexpected recorded identity");identities.Add(part.kind);if(part.visible&&!visibleIdentities.Add(part.kind))throw new InvalidOperationException("Ambiguous duplicate live component identity");}report.recordedIdentities=identities.Count;if(identities.Count!=7)throw new InvalidOperationException("All seven original component identities required, retaining recorded dead/reborn instances");sorted.Sort((a,b)=>{FortPixelLayerV54.TryOrder(a.npc,a.kind,a.motion,out int ar);FortPixelLayerV54.TryOrder(b.npc,b.kind,b.motion,out int br);return ar.CompareTo(br);});report.backToFront=sorted.ConvertAll(p=>p.kind+"/"+p.motion+(p.visible?"/visible":"/hidden")).ToArray();
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                var sourceNodes=new List<GameObject>();var sourceMaterials=new List<Material>();var sprites=new Sprite[capture.parts.Length];
                Shader reference=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/NativeReference.shader");Shader spriteShader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Resources/Pixel/Shaders/ArmoredSprite.shader");if(!reference||!spriteShader)throw new InvalidOperationException("Reference/render shader missing");
                Bounds bounds=default;bool hasBounds=false;
                for(int p=0;p<capture.parts.Length;p++){
                    var selected=capture.parts[p];bool originalHidden=selected.sourceVisible&&!selected.visible&&(!selected.rendererEnabled||!selected.rendererActive||selected.rendererAlpha==0);if(!identities.Contains(selected.kind)||selected.visible!=selected.sourceVisible&&!originalHidden)throw new InvalidOperationException("Identity or recorded original visibility mismatch");
                    var definition=Array.Find(roster.actors,a=>a.kind==selected.kind&&a.npc==selected.npc);if(definition==null)throw new InvalidOperationException("Installed identity absent");
                    var library=new MonsterPixelFramesV53(definition,o=>Object.DestroyImmediate(o));libs.Add(library);sprites[p]=library.At(selected.motion,selected.clock,out int fi,out _);if(fi!=selected.frame)throw new InvalidOperationException("Recorded source frame changed");
                    string sourcePath=Path.Combine(sourceRoot,selected.kind,"monster.json");pins.Add(FilePin(sourcePath));var doc=JsonUtility.FromJson<Source>(File.ReadAllText(sourcePath));var clip=Array.Find(doc.clips,c=>c.id==selected.motion);if(clip==null||selected.frame>=clip.frames.Length)throw new InvalidOperationException("Original source frame missing");
                    var node=new GameObject("Original source / "+selected.kind);owned.Add(node);sourceNodes.Add(node);node.transform.position=selected.projectedPosition;node.transform.rotation=selected.rotation;node.transform.localScale=selected.worldScale;node.SetActive(selected.visible);
                    var uvBytes=File.ReadAllBytes(doc.uvs);int vertexCount=0;foreach(var part in doc.parts)vertexCount+=part.count;var bytes=new byte[vertexCount*12];using(var stream=File.OpenRead(doc.geometry)){stream.Position=clip.frames[selected.frame].offset;int got=0;while(got<bytes.Length){int n=stream.Read(bytes,got,bytes.Length-got);if(n==0)throw new EndOfStreamException();got+=n;}}
                    int at=0;foreach(var part in doc.parts){
                        var go=new GameObject(part.name);go.transform.SetParent(node.transform,false);var vertices=new Vector3[part.count];var uv=new Vector2[part.count];var triangles=new int[part.count];
                        for(int i=0;i<part.count;i++){vertices[i]=new Vector3(BitConverter.ToSingle(bytes,at),BitConverter.ToSingle(bytes,at+4),BitConverter.ToSingle(bytes,at+8));at+=12;uv[i]=new Vector2(BitConverter.ToSingle(uvBytes,part.uvOffset+i*8),BitConverter.ToSingle(uvBytes,part.uvOffset+i*8+4));triangles[i]=i;}
                        var mesh=new Mesh{indexFormat=UnityEngine.Rendering.IndexFormat.UInt32,vertices=vertices,uv=uv,triangles=triangles};owned.Add(mesh);mesh.RecalculateBounds();go.AddComponent<MeshFilter>().sharedMesh=mesh;
                        var texture=new Texture2D(2,2);owned.Add(texture);texture.LoadImage(File.ReadAllBytes(part.texture));texture.filterMode=FilterMode.Point;var material=new Material(reference){mainTexture=texture};owned.Add(material);sourceMaterials.Add(material);var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;
                        if(selected.visible){if(hasBounds)bounds.Encapsulate(renderer.bounds);else{bounds=renderer.bounds;hasBounds=true;}}
                    }
                }
                if(!hasBounds)throw new InvalidOperationException("No recorded visible components");
                const int width=768,height=1024;var cameraNode=new GameObject("Fort isolated comparison camera");owned.Add(cameraNode);var camera=cameraNode.AddComponent<Camera>();camera.orthographic=true;camera.orthographicSize=Mathf.Max(bounds.extents.y+12,(bounds.extents.x+12)*height/width);camera.aspect=(float)width/height;camera.transform.position=new Vector3(bounds.center.x,bounds.center.y,bounds.min.z-200);camera.nearClipPlane=1;camera.farClipPlane=bounds.size.z+400;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.allowHDR=camera.allowMSAA=false;
                var target=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear){filterMode=FilterMode.Point};owned.Add(target);target.Create();camera.targetTexture=target;var readback=new Texture2D(width,height,TextureFormat.RGBA32,false,true);owned.Add(readback);
                Color32[] Render(string name,string policy,Color32[] compare=null){camera.Render();RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,width,height),0,0);readback.Apply();RenderTexture.active=null;var data=readback.GetPixels32();string file=Path.Combine(output,name+".png");File.WriteAllBytes(file,readback.EncodeToPNG());rows.Add(new ImageRow{file=file,policy=policy,rgbaSha256=Hash(data),differingPixels=compare==null?0:Difference(data,compare)});return data;}
                for(int cull=0;cull<=2;cull++){foreach(var material in sourceMaterials)material.SetFloat("_Cull",cull);Render("source-3d-cull-"+cull,"Original source textures/triangles, exact recorded source sample and placement; cull="+cull+" is diagnostic, not declared native policy");}
                foreach(var node in sourceNodes)node.SetActive(false);
                var temp=new List<Object>();
                Color32[] SpritePass(string name,bool fixedOrder,int permutation,Color32[] compare=null){
                    var indices=new int[capture.parts.Length];for(int i=0;i<indices.Length;i++)indices[i]=i;if(permutation==1)Array.Reverse(indices);if(permutation==2){var shuffled=new List<int>();for(int i=0;i<indices.Length;i+=2)shuffled.Add(i);for(int i=1;i<indices.Length;i+=2)shuffled.Add(i);indices=shuffled.ToArray();}
                    GameObject assembly=null;if(fixedOrder){assembly=new GameObject("Production Fort sorting group");temp.Add(assembly);assembly.AddComponent<SortingGroup>().sortingOrder=15;}
                    foreach(int p in indices){var selected=capture.parts[p];var node=new GameObject("Pixel component / "+selected.kind);temp.Add(node);node.transform.position=selected.projectedPosition;node.transform.rotation=selected.rotation;node.transform.localScale=selected.worldScale;var renderer=node.AddComponent<SpriteRenderer>();renderer.enabled=selected.visible;renderer.sortingOrder=15;renderer.spriteSortPoint=SpriteSortPoint.Pivot;
                        if(fixedOrder){if(!FortPixelLayerV54.TryOrder(selected.npc,selected.kind,selected.motion,out int actualOrder))throw new InvalidOperationException("Candidate differs from production Fort-only layer policy");node.transform.SetParent(assembly.transform,true);renderer.sortingOrder=actualOrder;}
                        var original=sprites[p];var texture=Object.Instantiate(original.texture);texture.name="Permuted clone / "+selected.kind;temp.Add(texture);var sprite=Sprite.Create(texture,original.rect,new Vector2(original.pivot.x/original.rect.width,original.pivot.y/original.rect.height),original.pixelsPerUnit,0,SpriteMeshType.FullRect);temp.Add(sprite);renderer.sprite=sprite;var material=new Material(spriteShader);temp.Add(material);renderer.sharedMaterial=material;
                        var block=new MaterialPropertyBlock();block.SetTexture("_MainTex",texture);block.SetVector("_MainTex_TexelSize",new Vector4(1f/texture.width,1f/texture.height,texture.width,texture.height));block.SetFloat("_Outline",0);block.SetFloat("_HitFlash",0);block.SetFloat("_MonsterInk",0);block.SetFloat("_NativeSceneClipR1461",0);renderer.SetPropertyBlock(block);
                    }
                    var data=Render(name,(fixedOrder?"Explicit candidate back-to-front":"Unspecified coplanar order 15")+"; original source SHOW; creation/material/texture instance permutation="+permutation,compare);for(int i=temp.Count-1;i>=0;i--)if(temp[i])Object.DestroyImmediate(temp[i]);temp.Clear();return data;
                }
                var loose=SpritePass("coplanar-forward",false,0);SpritePass("coplanar-reverse",false,1,loose);SpritePass("coplanar-shuffled",false,2,loose);
                var stable=SpritePass("candidate-forward",true,0);for(int p=1;p<=2;p++){var data=SpritePass(p==1?"candidate-reverse":"candidate-shuffled",true,p,stable);if(Difference(stable,data)!=0)throw new InvalidOperationException("Explicit candidate differs under creation/material/texture permutation");report.fixedOrderPermutationCases++;}
                report.passed=true;exit=0;
            }catch(Exception error){report.error=error.ToString();Debug.LogException(error);}finally{report.images=rows.ToArray();report.sources=pins.ToArray();File.WriteAllText(Path.Combine(output,"composite-check.json"),JsonUtility.ToJson(report,true));foreach(var library in libs)library.Dispose();for(int i=owned.Count-1;i>=0;i--)if(owned[i])Object.DestroyImmediate(owned[i]);}
            return exit==0;
        }
    }
}
#endif
