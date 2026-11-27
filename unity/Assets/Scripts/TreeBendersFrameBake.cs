#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    public static class TreeBendersFrameBake {
        const string Source="art/production/lk-native-v02/dungeon/tree-tuning-v50/source-frames";
        const int Size=416,SampleSize=Size*2,PageSize=2048;
        const float Units=1.25f,Height=115.18f,CameraY=130;
        static readonly Color32 Ink=new Color32(27,12,26,255);
        [Serializable] sealed class Part{public string name,texture;public int count,uvOffset;}
        [Serializable] sealed class Frame{public int offset;public float time,bodyFloor;}
        [Serializable] sealed class Clip{public string id;public float sourceDuration,fps;public bool loop;public Frame[] frames;}
        [Serializable] sealed class Document{public string geometry,uvs;public float neutralBodyHeight,nativeBodyFloor;public Part[] parts;public Clip[] clips;}
        static int Luma(Color32 c){return (c.r*2+c.g*3+c.b)/6;}
        static bool Same(Color32 a,Color32 b){return a.r==b.r&&a.g==b.g&&a.b==b.b;}
        static Vector3[] SmoothNormals(Vector3[] vertices) {
            // Expanded native triangles duplicate shared vertices, including UV
            // seams. Weld their normals by position so lighting never draws a
            // separate triangular shade on each source polygon.
            var keys=new Vector3Int[vertices.Length];var sums=new Dictionary<Vector3Int,Vector3>();
            for(int i=0;i<vertices.Length;i++){var v=vertices[i]*1000;keys[i]=new Vector3Int(Mathf.RoundToInt(v.x),Mathf.RoundToInt(v.y),Mathf.RoundToInt(v.z));}
            for(int i=0;i<vertices.Length;i+=3){var n=Vector3.Cross(vertices[i+1]-vertices[i],vertices[i+2]-vertices[i]);for(int j=0;j<3;j++){var k=keys[i+j];sums.TryGetValue(k,out var sum);sums[k]=sum+n;}}
            var result=new Vector3[vertices.Length];for(int i=0;i<result.Length;i++)result[i]=sums[keys[i]].normalized;return result;
        }
        // Project material classes along with the picture. This preserves actual
        // costume seams and overlaps without drawing the mesh's triangle edges.
        static Color32[] FinishPixels(Color32[] colors,Color32[] info) {
            var result=new Color32[Size*Size];var classes=new byte[result.Length];var depth=new byte[result.Length];
            var parts=new byte[result.Length];var light=new byte[result.Length];
            for(int y=0;y<Size;y++)for(int x=0;x<Size;x++) {
                int a=y*2*SampleSize+x*2,b=a+1,c=a+SampleSize,d=c+1,i=y*Size+x;
                int count=(colors[a].a>127?1:0)+(colors[b].a>127?1:0)+(colors[c].a>127?1:0)+(colors[d].a>127?1:0);
                if(count<2)continue;
                int chosen=a,best=-1,inks=0,inkAt=a;bool face=false;
                for(int k=0;k<4;k++) {
                    int at=k==0?a:k==1?b:k==2?c:d;if(colors[at].a<128)continue;
                    face|=info[at].b==4;
                    if(info[at].r==0){inks++;inkAt=at;}
                    int votes=0;for(int j=0;j<4;j++){int other=j==0?a:j==1?b:j==2?c:d;if(colors[other].a>127&&Same(colors[at],colors[other]))votes++;}
                    if(votes>best){best=votes;chosen=at;}
                }
                // Facial lines can be narrower than a destination pixel. Retain
                // their covered sample instead of averaging them into the skin.
                if(inks>=2||face&&inks>0)chosen=inkAt;
                result[i]=colors[chosen];result[i].a=255;classes[i]=info[chosen].r;depth[i]=info[chosen].g;parts[i]=info[chosen].b;light[i]=info[chosen].a;
            }
            // The body has 1.25-unit detail pixels, but painted shadow edges
            // use 2x2 clusters (2.5 units), close to the chicken's visible grid.
            // Average only matching materials; never cross the face or an ink line.
            for(int y=0;y<Size;y+=2)for(int x=0;x<Size;x+=2)for(int oy=0;oy<2;oy++)for(int ox=0;ox<2;ox++){
                int i=(y+oy)*Size+x+ox;if(result[i].a==0||classes[i]==0||parts[i]==4)continue;
                int sum=0,count=0;
                for(int yy=0;yy<2;yy++)for(int xx=0;xx<2;xx++){
                    int j=(y+yy)*Size+x+xx;if(result[j].a>0&&classes[j]==classes[i]&&parts[j]!=4){sum+=light[j];count++;}
                }
                float value=sum/(255f*count);int band=value<.39f?0:value<.68f?1:2;
                result[i]=TreeBendersToonPalette.Shade(result[i],classes[i]/16,band);
            }
            var outlined=(Color32[])result.Clone();
            for(int y=2;y<Size-2;y++)for(int x=2;x<Size-2;x++) {
                int i=y*Size+x;
                if(result[i].a==0) {
                    // Two detail pixels of outer ink, entirely outside the
                    // painted figure. Small facial/hand areas are not eroded.
                    bool edge=false;
                    for(int yy=-2;yy<=2&&!edge;yy++)for(int xx=-2;xx<=2;xx++){
                        if(xx*xx+yy*yy<=5&&result[i+yy*Size+xx].a>0){edge=true;break;}
                    }
                    if(edge)outlined[i]=Ink;
                    continue;
                }
                bool boundary=false,fold=false;
                for(int n=0;n<4;n++) {
                    int j=n==0?i-1:n==1?i+1:n==2?i-Size:i+Size;
                    if(result[j].a==0)continue;
                    // Place a single seam on the darker material; leave the
                    // small skin/white-cloth/metal highlights readable.
                    if(classes[i]!=0&&classes[j]!=0&&classes[i]!=classes[j]&&Luma(result[i])+8<Luma(result[j])){
                        boundary=true;
                        // Main garment seams have a second inside pixel. Keep
                        // eyes, fingertips, brass and blade ornament lines fine.
                        int k=i+i-j;
                        if(parts[i]!=4&&(classes[i]==48||classes[i]==96)&&classes[k]==classes[i]&&result[k].a>0)outlined[k]=Ink;
                    }
                    // Frontmost surface gets an ink edge where it occludes a
                    // separated surface. Smooth depths on one limb are untouched.
                    if(depth[j]-depth[i]>7&&classes[i]!=0){
                        if(classes[i]!=classes[j]||parts[i]!=parts[j])boundary=true;else fold=true;
                    }
                }
                if(boundary)outlined[i]=Ink;
                else if(fold&&!Same(outlined[i],Ink))outlined[i]=TreeBendersToonPalette.Shade(result[i],classes[i]/16,0);
            }
            return outlined;
        }
        static void PrepareTexture(Texture2D source,string name,out Texture2D color,out Texture2D mask) {
            var original=source.GetPixels32();var paint=new Color32[original.Length];var groups=new Color32[original.Length];
            int w=source.width,h=source.height;
            for(int y=0;y<h;y++)for(int x=0;x<w;x++){
                int i=y*w+x;var uv=new Vector2((x+.5f)/w,(y+.5f)/h);
                paint[i]=TreeBendersToonPalette.Colorize(original[i],name,uv);
                groups[i]=new Color32((byte)(TreeBendersToonPalette.MaterialId(original[i],name,uv)*16),0,0,original[i].a);
            }
            // Remove isolated texture shading in UV space, before projection,
            // so a patch remains attached to its surface throughout the motion.
            var clean=(Color32[])paint.Clone();
            if(!name.Contains("face"))for(int y=1;y<h-1;y++)for(int x=1;x<w-1;x++){
                int i=y*w+x;if(groups[i].r==0||paint[i].a<128)continue;
                int same=0,best=0,chosen=i;
                for(int oy=-1;oy<=1;oy++)for(int ox=-1;ox<=1;ox++){
                    if(ox==0&&oy==0)continue;int j=i+oy*w+ox;
                    if(groups[j].r!=groups[i].r)continue;
                    if(Same(paint[i],paint[j]))same++;
                    int votes=0;for(int yy=-1;yy<=1;yy++)for(int xx=-1;xx<=1;xx++){int q=i+yy*w+xx;if(groups[q].r==groups[i].r&&Same(paint[j],paint[q]))votes++;}
                    if(votes>best){best=votes;chosen=j;}
                }
                if(same==0&&best>=5)clean[i]=paint[chosen];
            }
            // Organize source shadows in stable UV clusters before projection.
            // Ink, face features and tiny metal highlights keep their fine grid.
            if(!name.Contains("face"))for(int by=0;by<h;by+=3)for(int bx=0;bx<w;bx+=3){
                for(int y=by;y<Math.Min(h,by+3);y++)for(int x=bx;x<Math.Min(w,bx+3);x++){
                    int i=y*w+x;if(groups[i].r==0||groups[i].r==128||paint[i].a<128)continue;
                    int chosen=i,best=0;
                    for(int yy=by;yy<Math.Min(h,by+3);yy++)for(int xx=bx;xx<Math.Min(w,bx+3);xx++){
                        int j=yy*w+xx;if(groups[j].r!=groups[i].r)continue;int votes=0;
                        for(int sy=by;sy<Math.Min(h,by+3);sy++)for(int sx=bx;sx<Math.Min(w,bx+3);sx++){int k=sy*w+sx;if(groups[k].r==groups[i].r&&Same(paint[j],paint[k]))votes++;}
                        if(votes>best){chosen=j;best=votes;}
                    }
                    if(best>=3)clean[i]=paint[chosen];
                }
            }
            color=new Texture2D(w,h,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};color.SetPixels32(clean);color.Apply();
            mask=new Texture2D(w,h,TextureFormat.RGBA32,false,true){filterMode=FilterMode.Point};mask.SetPixels32(groups);mask.Apply();
        }
        public static void Run() {
            int exit=1;var owned=new List<Object>();
            try {
                string repo=Path.GetFullPath("..");var args=Environment.GetCommandLineArgs();bool preview=Array.IndexOf(args,"--benders-preview")>=0;
                string output=Path.Combine(repo,"unity/Reports/tree-tuning-v52/bake");Directory.CreateDirectory(output);
                string destination=Path.Combine(Application.dataPath,"Resources/Pixel/tree-v52/benders");Directory.CreateDirectory(destination);
                var doc=JsonUtility.FromJson<Document>(File.ReadAllText(Path.Combine(repo,Source,"monster.json")));
                byte[] geometry=File.ReadAllBytes(doc.geometry),uvBytes=File.ReadAllBytes(doc.uvs);
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                var root=new GameObject("Original complete Benders model");owned.Add(root);root.transform.rotation=Quaternion.identity;
                float unit=Height/doc.neutralBodyHeight;root.transform.localScale=Vector3.one*unit;
                var meshes=new List<Mesh>();var vertices=new List<Vector3[]>();var renderers=new List<MeshRenderer>();
                var paints=new List<Material>();var masks=new List<Material>();
                foreach(var part in doc.parts) {
                    var go=new GameObject(part.name);go.transform.SetParent(root.transform,false);
                    var mesh=new Mesh{indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};owned.Add(mesh);
                    var uv=new Vector2[part.count];var ix=new int[part.count];var v=new Vector3[part.count];
                    for(int i=0;i<part.count;i++){uv[i]=new Vector2(BitConverter.ToSingle(uvBytes,part.uvOffset+i*8),BitConverter.ToSingle(uvBytes,part.uvOffset+i*8+4));ix[i]=i;}
                    mesh.vertices=v;mesh.uv=uv;mesh.triangles=ix;go.AddComponent<MeshFilter>().sharedMesh=mesh;
                    var texture=new Texture2D(2,2);texture.LoadImage(File.ReadAllBytes(part.texture));texture.filterMode=FilterMode.Point;owned.Add(texture);
                    PrepareTexture(texture,Path.GetFileNameWithoutExtension(part.texture),out var paint,out var mask);owned.Add(paint);owned.Add(mask);
                    if(preview)File.WriteAllBytes(Path.Combine(output,Path.GetFileNameWithoutExtension(part.texture)+"-toon.png"),paint.EncodeToPNG());
                    var material=new Material(AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/NativeReference.shader"));owned.Add(material);material.mainTexture=paint;
                    var edge=new Material(AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/TreeBendersEdge.shader"));owned.Add(edge);edge.mainTexture=mask;edge.SetFloat("_Part",(meshes.Count+1)/255f);
                    var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderers.Add(renderer);paints.Add(material);masks.Add(edge);meshes.Add(mesh);vertices.Add(v);
                }
                var camera=new GameObject("Whole frame pixel camera").AddComponent<Camera>();owned.Add(camera.gameObject);
                camera.orthographic=true;camera.orthographicSize=Size*Units*.5f;camera.aspect=1;
                camera.transform.position=new Vector3(0,CameraY,-1000);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;
                camera.nearClipPlane=1;camera.farClipPlane=3000;camera.allowHDR=camera.allowMSAA=false;
                var target=new RenderTexture(SampleSize,SampleSize,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear){filterMode=FilterMode.Point};target.Create();owned.Add(target);camera.targetTexture=target;
                var readback=new Texture2D(SampleSize,SampleSize,TextureFormat.RGBA32,false,true);owned.Add(readback);
                var previewFrame=new Texture2D(Size,Size,TextureFormat.RGBA32,false);owned.Add(previewFrame);
                var page=new Texture2D(PageSize,PageSize,TextureFormat.RGBA32,false);owned.Add(page);page.SetPixels32(new Color32[PageSize*PageSize]);
                int x=0,y=0,row=0,pageIndex=0,total=0;var pageNames=new List<string>();var clips=new List<TreeBendersRasterClip>();
                void Flush(){page.Apply();string name="page-"+pageIndex;File.WriteAllBytes(Path.Combine(destination,name+".png"),page.EncodeToPNG());pageNames.Add(name);pageIndex++;page.SetPixels32(new Color32[PageSize*PageSize]);x=y=row=0;}
                foreach(var clip in doc.clips) {
                    if(preview&&clip.id!="Wait"&&clip.id!="Dash"&&clip.id!="AttackA_Ready"&&clip.id!="AttackA_Fire")continue;
                    var frames=new List<TreeBendersRasterFrame>();
                    for(int f=0;f<clip.frames.Length;f++) {
                        if(preview&&f%Math.Max(1,(clip.frames.Length-1)/8)!=0&&f!=clip.frames.Length-1)continue;
                        var sample=clip.frames[f];int at=sample.offset;
                        for(int m=0;m<meshes.Count;m++) {
                            var v=vertices[m];for(int i=0;i<v.Length;i++){v[i]=new Vector3(BitConverter.ToSingle(geometry,at),BitConverter.ToSingle(geometry,at+4),BitConverter.ToSingle(geometry,at+8));at+=12;}
                            meshes[m].vertices=v;meshes[m].normals=SmoothNormals(v);meshes[m].RecalculateBounds();
                        }
                        bool airborne=clip.id.Contains("Air")||clip.id=="JumpUp"||clip.id=="JumpDown";
                        root.transform.position=Vector3.down*(airborne?doc.nativeBodyFloor:sample.bodyFloor)*unit;
                        for(int m=0;m<renderers.Count;m++)renderers[m].sharedMaterial=paints[m];
                        camera.Render();RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,SampleSize,SampleSize),0,0);readback.Apply();RenderTexture.active=null;
                        var source=readback.GetPixels32();
                        if(preview&&clip.id=="Wait"&&f==0)File.WriteAllBytes(Path.Combine(output,"Wait-000-toon-render.png"),readback.EncodeToPNG());
                        for(int m=0;m<renderers.Count;m++)renderers[m].sharedMaterial=masks[m];
                        camera.Render();RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,SampleSize,SampleSize),0,0);readback.Apply();RenderTexture.active=null;
                        var ink=FinishPixels(source,readback.GetPixels32());
                        int left=Size,right=-1,bottom=Size,top=-1;
                        for(int yy=0;yy<Size;yy++)for(int xx=0;xx<Size;xx++)if(ink[yy*Size+xx].a>0){left=Math.Min(left,xx);right=Math.Max(right,xx);bottom=Math.Min(bottom,yy);top=Math.Max(top,yy);}
                        if(left<2||right>Size-3||bottom<2||top>Size-3)throw new Exception("Clipped source frame "+clip.id+" "+f);
                        left-=2;right+=2;bottom-=2;top+=2;int w=right-left+1,h=top-bottom+1;
                        var crop=new Color32[w*h];for(int yy=0;yy<h;yy++)Array.Copy(ink,(bottom+yy)*Size+left,crop,yy*w,w);
                        if(preview){previewFrame.SetPixels32(ink);previewFrame.Apply();File.WriteAllBytes(Path.Combine(output,clip.id+"-"+f.ToString("D3")+"-pixel.png"),previewFrame.EncodeToPNG());continue;}
                        if(x+w>PageSize){x=0;y+=row;row=0;}if(y+h>PageSize)Flush();
                        page.SetPixels32(x,y,w,h,crop);
                        frames.Add(new TreeBendersRasterFrame{page=pageIndex,x=x,y=y,w=w,h=h,px=Size*.5f-left,py=(Size*Units*.5f-CameraY)/Units-bottom});
                        x+=w+2;row=Math.Max(row,h+2);total++;
                    }
                    if(!preview)clips.Add(new TreeBendersRasterClip{name=clip.id,duration=clip.sourceDuration,fps=60,loop=clip.loop,frames=frames.ToArray()});
                }
                if(!preview){Flush();File.WriteAllText(Path.Combine(destination,"library.json"),JsonUtility.ToJson(new TreeBendersRasterData{pixelsPerUnit=1/Units,pages=pageNames.ToArray(),clips=clips.ToArray()},true));
                    AssetDatabase.Refresh();foreach(string name in pageNames){string asset="Assets/Resources/Pixel/tree-v52/benders/"+name+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(asset);importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=PageSize;importer.alphaIsTransparency=true;importer.SaveAndReimport();}
                }
                File.WriteAllText(Path.Combine(output,preview?"preview.json":"bake.json"),"{\"passed\":true,\"fps\":60,\"frames\":"+total+",\"clips\":"+clips.Count+",\"pages\":"+pageNames.Count+",\"worldUnitsPerPixel\":1.25,\"coverageSamples\":4,\"outerInkWorldUnits\":2.5,\"shadowClusterWorldUnits\":2.5,\"uvShadowClusterPixels\":3,\"materialAndDepthInk\":true}");exit=0;
            }catch(Exception e){Debug.LogException(e);}
            finally{foreach(var o in owned)if(o)Object.DestroyImmediate(o);EditorApplication.Exit(exit);}
        }
    }
}
#endif
