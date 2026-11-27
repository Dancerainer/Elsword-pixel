#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    // Explicit opt-in fork of the fixed v53 rasterizer. Reviewed source surface
    // visibility is applied before the unchanged palette/pixel treatment.
    // Legacy bake code, shaders and fingerprints remain byte-for-byte intact.
    public static class MonsterPixelSurfaceBakeV53 {
        const int PageSize=2048;
        [Serializable] sealed class Part{public string name,texture;public int count,uvOffset;}
        [Serializable] sealed class Frame{public long offset;public float time,bodyFloor;public float[] min,max;}
        [Serializable] sealed class Clip{public string id;public float sourceDuration,fps;public bool loop;public Frame[] frames;}
        [Serializable] sealed class Document{public string id,sourceNPC,geometry,uvs,groundPolicy;public float neutralBodyHeight,nativeYawDegrees;public Part[] parts;public Clip[] clips;}
        [Serializable] sealed class CullBinding{public string part,texture;public int mode;}
        [Serializable] sealed class AlphaBinding{public string texture;public double alphaCutoff;}
        [Serializable] sealed class SurfacePolicy{public string bakeImplementation;public CullBinding[] surfaceCullBindings;public AlphaBinding[] surfaceAlphaBindings;}
        [Serializable] sealed class SurfaceAssignment{public string part,texture;public int cullMode,alphaThreshold=-1,alphaPromotedPixels,alphaClippedPixels;}
        [Serializable] sealed class Report {
            public string source,recipe,id,sourceNPC,output,groundPolicy,inputFingerprint,librarySha256;
            public string bakeImplementation;
            public SurfaceAssignment[] surfaceAssignments;
            public string[] pageSha256;
            public bool passed,preview,visualReviewPassed;
            public int frames,uniqueRasters,clips,pages,width,height;
            public float standingHeight,worldUnitsPerPixel,outerInkWorldUnits,shadowClusterWorldUnits,depthRange;
            public MonsterPixelRampV53[] palette;
        }
        [Serializable] sealed class Job {public string kind,action,source,recipe,output,report;}
        [Serializable] sealed class Queue {public Job[] actors;}
        [Serializable] sealed class JobResult {public string kind,status,error;}
        [Serializable] sealed class QueueResult {public bool passed;public int completed,reused,missing,deferred,failed;public JobResult[] actors;}
        sealed class Raster{public int page,x,y,w,h;}
        static string Arg(string[] args,string name,string fallback=null){int i=Array.IndexOf(args,name);if(i<0)return fallback;if(i+1==args.Length)throw new ArgumentException("Missing value for "+name);return args[i+1];}
        static bool Same(Color32 a,Color32 b){return a.r==b.r&&a.g==b.g&&a.b==b.b;}
        static SurfaceAssignment[] ResolveSurfaces(Document doc,string recipeJson){
            var policy=JsonUtility.FromJson<SurfacePolicy>(recipeJson);
            if(policy==null||policy.bakeImplementation!="surface-cull-v1")throw new ArgumentException("Explicit bakeImplementation=surface-cull-v1 required");
            var culls=policy.surfaceCullBindings??new CullBinding[0];var alphas=policy.surfaceAlphaBindings??new AlphaBinding[0];
            if(culls.Length+alphas.Length==0)throw new ArgumentException("At least one reviewed surface binding is required");
            foreach(var binding in culls)if(binding==null||String.IsNullOrWhiteSpace(binding.part)||String.IsNullOrWhiteSpace(binding.texture)||(binding.mode!=1&&binding.mode!=2))throw new ArgumentException("Surface cull bindings need an exact part, texture substring and mode 1 or 2");
            foreach(var binding in alphas)if(binding==null||String.IsNullOrWhiteSpace(binding.texture)||Double.IsNaN(binding.alphaCutoff)||Double.IsInfinity(binding.alphaCutoff)||binding.alphaCutoff<=0||binding.alphaCutoff>1)throw new ArgumentException("Surface alpha bindings need a texture substring and finite cutoff in (0,1]");
            var cullHits=new int[culls.Length];var alphaHits=new int[alphas.Length];var result=new SurfaceAssignment[doc.parts.Length];
            for(int i=0;i<doc.parts.Length;i++){
                var part=doc.parts[i];var row=new SurfaceAssignment{part=part.name,texture=part.texture};bool cullFound=false,alphaFound=false;
                for(int j=0;j<culls.Length;j++){var binding=culls[j];if(part.name==binding.part&&part.texture.IndexOf(binding.texture,StringComparison.OrdinalIgnoreCase)>=0){if(cullFound)throw new ArgumentException("Ambiguous cull bindings for "+part.name);cullFound=true;cullHits[j]++;row.cullMode=binding.mode;}}
                for(int j=0;j<alphas.Length;j++){var binding=alphas[j];if(part.texture.IndexOf(binding.texture,StringComparison.OrdinalIgnoreCase)>=0){if(alphaFound)throw new ArgumentException("Ambiguous alpha bindings for "+part.name);alphaFound=true;alphaHits[j]++;row.alphaThreshold=(int)Math.Ceiling(binding.alphaCutoff*255.0);}}
                result[i]=row;
            }
            for(int i=0;i<cullHits.Length;i++)if(cullHits[i]!=1)throw new ArgumentException("Cull binding must match exactly one source part: "+culls[i].part);
            for(int i=0;i<alphaHits.Length;i++)if(alphaHits[i]==0)throw new ArgumentException("Alpha binding matches no source texture: "+alphas[i].texture);
            return result;
        }
        static void NormalizeReviewedAlpha(Texture2D texture,SurfaceAssignment surface){
            if(surface.alphaThreshold<0)return;
            var pixels=texture.GetPixels32();
            for(int i=0;i<pixels.Length;i++){
                byte original=pixels[i].a;byte alpha=(byte)(original>=surface.alphaThreshold?255:0);
                if(alpha==255&&original<255)surface.alphaPromotedPixels++;else if(alpha==0&&original>0)surface.alphaClippedPixels++;
                // Preserve every original RGB byte, including clipped texels.
                pixels[i].a=alpha;
            }
            texture.SetPixels32(pixels);texture.Apply();
        }
        static bool FineTexture(string name,MonsterPixelRecipeV53 recipe){
            if(recipe.detailTextures!=null){foreach(var pattern in recipe.detailTextures)if(name.IndexOf(pattern,StringComparison.OrdinalIgnoreCase)>=0)return true;return false;}
            return name.IndexOf("face",StringComparison.OrdinalIgnoreCase)>=0||name.IndexOf("eye",StringComparison.OrdinalIgnoreCase)>=0;
        }
        static Vector3[] SmoothNormals(Vector3[] vertices){
            var keys=new Vector3Int[vertices.Length];var sums=new Dictionary<Vector3Int,Vector3>();
            for(int i=0;i<vertices.Length;i++){var v=vertices[i]*1000;keys[i]=new Vector3Int(Mathf.RoundToInt(v.x),Mathf.RoundToInt(v.y),Mathf.RoundToInt(v.z));}
            for(int i=0;i<vertices.Length;i+=3){var n=Vector3.Cross(vertices[i+1]-vertices[i],vertices[i+2]-vertices[i]);for(int j=0;j<3;j++){var k=keys[i+j];sums.TryGetValue(k,out var sum);sums[k]=sum+n;}}
            var result=new Vector3[vertices.Length];for(int i=0;i<result.Length;i++)result[i]=sums[keys[i]].normalized;return result;
        }
        static void PrepareTexture(Texture2D source,string name,MonsterPixelPaletteV53 palette,MonsterPixelRecipeV53 recipe,out Texture2D paint,out Texture2D mask){
            int w=source.width,h=source.height;var original=source.GetPixels32();var colors=new Color32[original.Length];var groups=new Color32[original.Length];var detail=new bool[original.Length];
            bool face=FineTexture(name,recipe);
            for(int y=0;y<h;y++)for(int x=0;x<w;x++){int i=y*w+x;palette.Paint(original[i],name,new Vector2((x+.5f)/w,(y+.5f)/h),out colors[i],out byte material,out detail[i]);detail[i]|=face;groups[i]=new Color32(material,(byte)(detail[i]?255:0),0,original[i].a);}
            var clean=(Color32[])colors.Clone();int size=recipe.uvClusterPixels;
            // Material-majority source clusters travel with the surface. Fine
            // face/eye details and source ink are never smeared by this step.
            if(size>1)for(int by=0;by<h;by+=size)for(int bx=0;bx<w;bx+=size){
                for(int y=by;y<Math.Min(h,by+size);y++)for(int x=bx;x<Math.Min(w,bx+size);x++){
                    int i=y*w+x;if(detail[i]||groups[i].r==0||colors[i].a<128)continue;
                    int chosen=i,best=0;
                    for(int yy=by;yy<Math.Min(h,by+size);yy++)for(int xx=bx;xx<Math.Min(w,bx+size);xx++){
                        int j=yy*w+xx;if(groups[j].r!=groups[i].r)continue;int votes=0;
                        for(int sy=by;sy<Math.Min(h,by+size);sy++)for(int sx=bx;sx<Math.Min(w,bx+size);sx++){int k=sy*w+sx;if(groups[k].r==groups[i].r&&Same(colors[j],colors[k]))votes++;}
                        if(votes>best){chosen=j;best=votes;}
                    }
                    if(best>=3)clean[i]=colors[chosen];
                }
            }
            paint=new Texture2D(w,h,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};paint.SetPixels32(clean);paint.Apply();
            mask=new Texture2D(w,h,TextureFormat.RGBA32,false,true){filterMode=FilterMode.Point};mask.SetPixels32(groups);mask.Apply();
        }
        static Color32[] Finish(Color32[] colors,Color32[] info,int width,int height,float depthRange,bool[] fineParts,MonsterPixelPaletteV53 palette,MonsterPixelRecipeV53 recipe){
            int sampleWidth=width*2;var result=new Color32[width*height];var classes=new byte[result.Length];var depth=new byte[result.Length];var parts=new byte[result.Length];var light=new byte[result.Length];var details=new bool[result.Length];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++){
                int a=y*2*sampleWidth+x*2,b=a+1,c=a+sampleWidth,d=c+1,i=y*width+x;
                int coverage=(colors[a].a>127?1:0)+(colors[b].a>127?1:0)+(colors[c].a>127?1:0)+(colors[d].a>127?1:0);if(coverage<2)continue;
                int chosen=a,best=-1,inks=0,inkAt=a;bool fine=false;
                for(int k=0;k<4;k++){int at=k==0?a:k==1?b:k==2?c:d;if(colors[at].a<128)continue;fine|=fineParts[info[at].b]||info[at].a==255;if(info[at].r==0){inks++;inkAt=at;}
                    int votes=0;for(int j=0;j<4;j++){int other=j==0?a:j==1?b:j==2?c:d;if(colors[other].a>127&&Same(colors[at],colors[other]))votes++;}if(votes>best){best=votes;chosen=at;}}
                if(inks>=2||fine&&inks>0)chosen=inkAt;
                result[i]=colors[chosen];result[i].a=255;classes[i]=info[chosen].r;depth[i]=info[chosen].g;parts[i]=info[chosen].b;light[i]=info[chosen].a;details[i]=fineParts[parts[i]]||light[i]==255;
            }
            int cluster=recipe.shadowClusterPixels;
            for(int y=0;y<height;y+=cluster)for(int x=0;x<width;x+=cluster)for(int oy=0;oy<Math.Min(cluster,height-y);oy++)for(int ox=0;ox<Math.Min(cluster,width-x);ox++){
                int i=(y+oy)*width+x+ox;if(result[i].a==0||classes[i]==0||details[i])continue;int sum=0,count=0;
                for(int yy=0;yy<Math.Min(cluster,height-y);yy++)for(int xx=0;xx<Math.Min(cluster,width-x);xx++){int j=(y+yy)*width+x+xx;if(result[j].a>0&&classes[j]==classes[i]&&!details[j]){sum+=light[j];count++;}}
                float value=sum/(255f*count);result[i]=palette.Shade(result[i],classes[i],value<.39f?0:value<.68f?1:2);
            }
            var outlined=(Color32[])result.Clone();int radius=recipe.outerInkPixels;float depthGap=recipe.occlusionDepthWorldUnits*255/depthRange;
            for(int y=radius;y<height-radius;y++)for(int x=radius;x<width-radius;x++){
                int i=y*width+x;if(result[i].a==0){bool edge=false;for(int yy=-radius;yy<=radius&&!edge;yy++)for(int xx=-radius;xx<=radius;xx++){if(xx*xx+yy*yy<=radius*radius+Math.Max(0,radius-1)&&result[i+yy*width+xx].a>0){edge=true;break;}}if(edge)outlined[i]=palette.ink;continue;}
                bool boundary=false,fold=false;
                for(int n=0;n<4;n++){int j=n==0?i-1:n==1?i+1:n==2?i-width:i+width;if(result[j].a==0)continue;
                    // Automatic hue buckets reduce paint, but are not material
                    // identities. A blue-to-purple shade on one bark surface
                    // must not acquire a black seam. Only reviewed semantic UV
                    // classes or actual source-part boundaries authorize ink.
                    bool materialBoundary=classes[i]>=64||classes[j]>=64||parts[i]!=parts[j];
                    if(materialBoundary&&classes[i]!=0&&classes[j]!=0&&classes[i]!=classes[j]&&MonsterPixelPaletteV53.Luma(result[i])+8<MonsterPixelPaletteV53.Luma(result[j]))boundary=true;
                    if(depth[j]-depth[i]>depthGap&&classes[i]!=0){if(classes[i]!=classes[j]||parts[i]!=parts[j])boundary=true;else fold=true;}}
                if(boundary)outlined[i]=palette.ink;else if(fold&&!Same(outlined[i],palette.ink))outlined[i]=palette.Shade(result[i],classes[i],0);
            }
            return outlined;
        }
        static string Fingerprint(string source,string recipe){
            ResolveSurfaces(JsonUtility.FromJson<Document>(File.ReadAllText(source)),File.ReadAllText(recipe));
            using(var hash=SHA256.Create()){
                var files=new[]{source,recipe,Path.Combine(Path.GetDirectoryName(source),"provenance.json"),Path.Combine(Application.dataPath,"Scripts/MonsterPixelSurfaceBakeV53.cs"),Path.Combine(Application.dataPath,"Scripts/MonsterPixelPaletteV53.cs"),Path.Combine(Application.dataPath,"Editor/MonsterPixelSurfaceEdgeV53.shader"),Path.Combine(Application.dataPath,"Editor/NativeSurfaceReferenceV53.shader")};
                var text="";foreach(var path in files)text+=Convert.ToBase64String(hash.ComputeHash(File.ReadAllBytes(path)))+"\n";
                return Convert.ToBase64String(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)));
            }
        }
        static string FileHash(string path){using(var stream=File.OpenRead(path))using(var hash=SHA256.Create())return Convert.ToBase64String(hash.ComputeHash(stream));}
        static bool ValidOutput(Report receipt,string output){
            string library=Path.Combine(output,"library.json");if(!File.Exists(library)||receipt.librarySha256!=FileHash(library))return false;
            var data=JsonUtility.FromJson<TreeBendersRasterData>(File.ReadAllText(library));if(receipt.pageSha256==null||receipt.pageSha256.Length!=data.pages.Length)return false;
            for(int i=0;i<data.pages.Length;i++){string page=Path.Combine(output,data.pages[i]+".png");if(!File.Exists(page)||receipt.pageSha256[i]!=FileHash(page))return false;}return true;
        }
        public static void Run(){EditorApplication.Exit(Execute(Environment.GetCommandLineArgs()));}
        public static void RunQueue(){
            int exit=1;
            try{
                var args=Environment.GetCommandLineArgs();string queuePath=Path.GetFullPath(Arg(args,"--monster-queue")??throw new ArgumentException("--monster-queue required"));
                string repo=Path.GetFullPath(Path.Combine(Application.dataPath,"../.."));
                string Resolve(string path){return Path.IsPathRooted(path)?path:Path.Combine(repo,path);}
                var queue=JsonUtility.FromJson<Queue>(File.ReadAllText(queuePath));var results=new List<JobResult>();var summary=new QueueResult();
                bool preview=Array.IndexOf(args,"--monster-preview")>=0;
                string status=Arg(args,"--monster-queue-report",Path.Combine(Path.GetDirectoryName(queuePath),preview?"preview-progress.json":"bake-progress.json"));
                void Save(){summary.actors=results.ToArray();File.WriteAllText(status,JsonUtility.ToJson(summary,true));}
                foreach(var job in queue.actors){
                    if(job.action=="preserve-baseline"){results.Add(new JobResult{kind=job.kind,status="preserved-v52"});summary.reused++;Save();continue;}
                    if(job.action!=null&&job.action.StartsWith("defer-",StringComparison.Ordinal)){results.Add(new JobResult{kind=job.kind,status=job.action});summary.deferred++;Save();continue;}
                    string source=Resolve(job.source),recipe=Resolve(job.recipe),output=Resolve(job.output),report=Resolve(job.report);
                    if(!File.Exists(source)){results.Add(new JobResult{kind=job.kind,status="source-not-ready"});summary.missing++;Save();continue;}
                    string receipt=Path.Combine(report,preview?"preview.json":"bake.json");
                    if(File.Exists(receipt)){
                        var prior=JsonUtility.FromJson<Report>(File.ReadAllText(receipt));
                        if(prior.passed&&prior.preview==preview&&prior.inputFingerprint==Fingerprint(source,recipe)&&(preview||ValidOutput(prior,output))){summary.reused++;results.Add(new JobResult{kind=job.kind,status="fingerprint-current"});Save();continue;}
                    }
                    var entry=new JobResult{kind=job.kind,status="running"};results.Add(entry);Save();
                    var command=new List<string>{"--monster-source",source,"--monster-recipe",recipe,"--monster-output",output,"--monster-report",report};if(preview)command.Add("--monster-preview");
                    int code=Execute(command.ToArray());entry.status=code==0?"baked-awaiting-review":"bake-failed";if(code==0)summary.completed++;else summary.failed++;Save();GC.Collect();
                }
                summary.passed=summary.failed==0&&summary.missing==0&&summary.deferred==0;Save();exit=summary.failed>0?1:summary.missing+summary.deferred>0?2:0;
            }catch(Exception error){Debug.LogException(error);}finally{EditorApplication.Exit(exit);}
        }
        static int Execute(string[] args){
            int exit=1;var owned=new List<Object>();
            try{
                string source=Path.GetFullPath(Arg(args,"--monster-source")??throw new ArgumentException("--monster-source required"));
                string recipePath=Path.GetFullPath(Arg(args,"--monster-recipe")??throw new ArgumentException("--monster-recipe required"));
                string destination=Path.GetFullPath(Arg(args,"--monster-output")??throw new ArgumentException("--monster-output required"));
                string output=Path.GetFullPath(Arg(args,"--monster-report",Path.Combine(Path.GetDirectoryName(source),"bake")));
                bool preview=Array.IndexOf(args,"--monster-preview")>=0;Directory.CreateDirectory(destination);Directory.CreateDirectory(output);
                var doc=JsonUtility.FromJson<Document>(File.ReadAllText(source));string recipeJson=File.ReadAllText(recipePath);var recipe=JsonUtility.FromJson<MonsterPixelRecipeV53>(recipeJson);
                if(doc.parts==null||doc.parts.Length==0)throw new ArgumentException("Original source parts required");
                var surfaces=ResolveSurfaces(doc,recipeJson);
                string fingerprint=Fingerprint(source,recipePath),receiptPath=Path.Combine(output,preview?"preview.json":"bake.json");
                File.WriteAllText(receiptPath,JsonUtility.ToJson(new Report{source=source,recipe=recipePath,id=doc.id,passed=false,preview=preview,inputFingerprint=fingerprint,bakeImplementation="surface-cull-v1",surfaceAssignments=surfaces},true));
                if(doc.parts==null||doc.parts.Length==0||doc.parts.Length>254||doc.neutralBodyHeight<=0||recipe.height<=0||recipe.units<=0||recipe.outerInkPixels<1||recipe.outerInkPixels>4||recipe.shadowClusterPixels<1||recipe.shadowClusterPixels>4)throw new ArgumentException("Invalid source or recipe dimensions");
                if(Mathf.Abs(doc.nativeYawDegrees)>.001f)throw new ArgumentException("Exporter must apply the audited source yaw to bounds and geometry");
                if(recipe.groundPolicy!="preserve-authored-relative-y")throw new ArgumentException("Unsupported ground policy; use explicit groundedClips with the source-relative baseline");
                float scale=recipe.height/doc.neutralBodyHeight;var grounded=new HashSet<string>(recipe.groundedClips??new string[0],StringComparer.Ordinal);
                float Shift(Clip clip,Frame frame){return grounded.Contains(clip.id)?-frame.bodyFloor*scale:0;}
                var lo=new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);var hi=new Vector3(float.NegativeInfinity,float.NegativeInfinity,float.NegativeInfinity);
                foreach(var clip in doc.clips){if(clip.fps<60||clip.frames==null||clip.frames.Length<2)throw new ArgumentException("Dense original frames required: "+clip.id);foreach(var frame in clip.frames){float shift=Shift(clip,frame);lo=Vector3.Min(lo,new Vector3(frame.min[0]*scale,frame.min[1]*scale+shift,frame.min[2]*scale));hi=Vector3.Max(hi,new Vector3(frame.max[0]*scale,frame.max[1]*scale+shift,frame.max[2]*scale));}}
                // One fixed canvas/scale/pivot for all poses. Never normalize
                // individual frame bounds: that stretches limbs and causes bob.
                int margin=recipe.outerInkPixels+5;
                float left=Mathf.Floor(lo.x/recipe.units)-margin,bottom=Mathf.Floor(lo.y/recipe.units)-margin;
                int width=Mathf.CeilToInt(hi.x/recipe.units-left)+margin,height=Mathf.CeilToInt(hi.y/recipe.units-bottom)+margin;
                width=(width+1)/2*2;height=(height+1)/2*2;
                if(width>PageSize-4||height>PageSize-4)throw new ArgumentException("Source action extent exceeds atlas page; review root policy and scale: "+width+"x"+height);
                float centerX=(left+width*.5f)*recipe.units,centerY=(bottom+height*.5f)*recipe.units,depthRange=Mathf.Max(256,2*Mathf.Max(Mathf.Abs(lo.z),Mathf.Abs(hi.z))+32);
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                var root=new GameObject("Original complete "+doc.id);owned.Add(root);root.transform.localScale=Vector3.one*scale;
                var originals=new List<Texture2D>();for(int partIndex=0;partIndex<doc.parts.Length;partIndex++){var part=doc.parts[partIndex];var texture=new Texture2D(2,2);texture.LoadImage(File.ReadAllBytes(part.texture));texture.filterMode=FilterMode.Point;NormalizeReviewedAlpha(texture,surfaces[partIndex]);owned.Add(texture);originals.Add(texture);}
                var palette=new MonsterPixelPaletteV53(recipe,originals);var fineParts=new bool[256];
                var meshes=new List<Mesh>();var vertices=new List<Vector3[]>();var renderers=new List<MeshRenderer>();var paints=new List<Material>();var masks=new List<Material>();
                byte[] uvBytes=File.ReadAllBytes(doc.uvs);int vertexCount=0;
                var colorShader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/NativeSurfaceReferenceV53.shader");var edgeShader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/MonsterPixelSurfaceEdgeV53.shader");if(!colorShader||!edgeShader)throw new InvalidOperationException("Baking shaders unavailable");
                for(int partIndex=0;partIndex<doc.parts.Length;partIndex++){
                    var part=doc.parts[partIndex];vertexCount+=part.count;var go=new GameObject(part.name);go.transform.SetParent(root.transform,false);
                    var mesh=new Mesh{indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};owned.Add(mesh);var uv=new Vector2[part.count];var ix=new int[part.count];var v=new Vector3[part.count];
                    for(int i=0;i<part.count;i++){uv[i]=new Vector2(BitConverter.ToSingle(uvBytes,part.uvOffset+i*8),BitConverter.ToSingle(uvBytes,part.uvOffset+i*8+4));ix[i]=i;}
                    mesh.vertices=v;mesh.uv=uv;mesh.triangles=ix;go.AddComponent<MeshFilter>().sharedMesh=mesh;
                    string name=Path.GetFileNameWithoutExtension(part.texture);fineParts[partIndex+1]=FineTexture(name,recipe);
                    PrepareTexture(originals[partIndex],name,palette,recipe,out var paint,out var mask);owned.Add(paint);owned.Add(mask);File.WriteAllBytes(Path.Combine(output,"texture-"+partIndex+"-toon.png"),paint.EncodeToPNG());
                    var material=new Material(colorShader){mainTexture=paint};owned.Add(material);var edge=new Material(edgeShader){mainTexture=mask};owned.Add(edge);edge.SetFloat("_Part",(partIndex+1)/255f);edge.SetFloat("_DepthRange",depthRange);
                    material.SetFloat("_Cull",surfaces[partIndex].cullMode);edge.SetFloat("_Cull",surfaces[partIndex].cullMode);
                    var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderers.Add(renderer);paints.Add(material);masks.Add(edge);meshes.Add(mesh);vertices.Add(v);
                }
                var camera=new GameObject("Whole frame pixel camera").AddComponent<Camera>();owned.Add(camera.gameObject);camera.orthographic=true;camera.orthographicSize=height*recipe.units*.5f;camera.aspect=(float)width/height;camera.transform.position=new Vector3(centerX,centerY,lo.z-100);camera.nearClipPlane=1;camera.farClipPlane=hi.z-lo.z+200;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.allowHDR=camera.allowMSAA=false;
                int sw=width*2,sh=height*2;var target=new RenderTexture(sw,sh,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear){filterMode=FilterMode.Point};target.Create();owned.Add(target);camera.targetTexture=target;
                var readback=new Texture2D(sw,sh,TextureFormat.RGBA32,false,true);owned.Add(readback);var review=new Texture2D(width,height,TextureFormat.RGBA32,false);owned.Add(review);
                var page=new Texture2D(PageSize,PageSize,TextureFormat.RGBA32,false);owned.Add(page);page.SetPixels32(new Color32[PageSize*PageSize]);
                int x=0,y=0,row=0,pageIndex=0,total=0;var pageNames=new List<string>();var clips=new List<TreeBendersRasterClip>();var unique=new Dictionary<string,Raster>();
                void Flush(){page.Apply();string name="page-"+pageIndex;File.WriteAllBytes(Path.Combine(destination,name+".png"),page.EncodeToPNG());pageNames.Add(name);pageIndex++;page.SetPixels32(new Color32[PageSize*PageSize]);x=y=row=0;}
                Color32[] Render(bool info){for(int m=0;m<renderers.Count;m++)renderers[m].sharedMaterial=info?masks[m]:paints[m];camera.Render();RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,sw,sh),0,0);readback.Apply();RenderTexture.active=null;return readback.GetPixels32();}
                using(var geometry=new FileStream(doc.geometry,FileMode.Open,FileAccess.Read,FileShare.Read))using(var hash=SHA256.Create()){
                    byte[] frameBytes=new byte[vertexCount*12];float[] floats=new float[vertexCount*3];
                    foreach(var clip in doc.clips){var frames=new List<TreeBendersRasterFrame>();
                        for(int f=0;f<clip.frames.Length;f++){
                            bool sampleReview=f==0||f==clip.frames.Length-1||f%(Math.Max(1,(clip.frames.Length-1)/8))==0;if(preview&&!sampleReview)continue;
                            var sample=clip.frames[f];geometry.Position=sample.offset;int read=0;while(read<frameBytes.Length){int n=geometry.Read(frameBytes,read,frameBytes.Length-read);if(n==0)throw new EndOfStreamException("Incomplete geometry "+clip.id+" "+f);read+=n;}Buffer.BlockCopy(frameBytes,0,floats,0,frameBytes.Length);
                            int at=0;for(int m=0;m<meshes.Count;m++){var v=vertices[m];for(int i=0;i<v.Length;i++){v[i]=new Vector3(floats[at],floats[at+1],floats[at+2]);at+=3;}meshes[m].vertices=v;meshes[m].normals=SmoothNormals(v);meshes[m].RecalculateBounds();}
                            root.transform.position=Vector3.up*Shift(clip,sample);
                            var colors=Render(false);var finished=Finish(colors,Render(true),width,height,depthRange,fineParts,palette,recipe);
                            int l=width,r=-1,b=height,t=-1;for(int yy=0;yy<height;yy++)for(int xx=0;xx<width;xx++)if(finished[yy*width+xx].a>0){l=Math.Min(l,xx);r=Math.Max(r,xx);b=Math.Min(b,yy);t=Math.Max(t,yy);}
                            if(r<l)throw new InvalidOperationException("Invisible original frame "+clip.id+" "+f);if(l<2||r>width-3||b<2||t>height-3)throw new InvalidOperationException("Clipped original frame "+clip.id+" "+f);
                            if(sampleReview){review.SetPixels32(finished);review.Apply();File.WriteAllBytes(Path.Combine(output,clip.id+"-"+f.ToString("D4")+".png"),review.EncodeToPNG());}
                            total++;if(preview)continue;l-=2;r+=2;b-=2;t+=2;int w=r-l+1,h=t-b+1;
                            var crop=new Color32[w*h];var bytes=new byte[crop.Length*4];for(int yy=0;yy<h;yy++)Array.Copy(finished,(b+yy)*width+l,crop,yy*w,w);
                            for(int i=0;i<crop.Length;i++){bytes[i*4]=crop[i].r;bytes[i*4+1]=crop[i].g;bytes[i*4+2]=crop[i].b;bytes[i*4+3]=crop[i].a;}
                            string key=w+"x"+h+":"+Convert.ToBase64String(hash.ComputeHash(bytes));
                            if(!unique.TryGetValue(key,out var raster)){if(x+w>PageSize){x=0;y+=row;row=0;}if(y+h>PageSize)Flush();page.SetPixels32(x,y,w,h,crop);raster=new Raster{page=pageIndex,x=x,y=y,w=w,h=h};unique.Add(key,raster);x+=w+2;row=Math.Max(row,h+2);}
                            frames.Add(new TreeBendersRasterFrame{page=raster.page,x=raster.x,y=raster.y,w=w,h=h,px=-left-l,py=-bottom-b});
                        }
                        if(!preview)clips.Add(new TreeBendersRasterClip{name=clip.id,duration=clip.sourceDuration,fps=clip.fps,loop=clip.loop,frames=frames.ToArray()});Debug.Log("MONSTER_BAKE "+doc.id+" "+clip.id+" "+clip.frames.Length);
                    }
                }
                if(!preview){Flush();File.WriteAllText(Path.Combine(destination,"library.json"),JsonUtility.ToJson(new TreeBendersRasterData{pixelsPerUnit=1/recipe.units,pages=pageNames.ToArray(),clips=clips.ToArray()},true));
                    string assets=Path.GetFullPath(Application.dataPath)+Path.DirectorySeparatorChar;
                    if(destination.StartsWith(assets,StringComparison.OrdinalIgnoreCase)){AssetDatabase.Refresh();foreach(string name in pageNames){string asset="Assets/"+Path.Combine(destination.Substring(assets.Length),name+".png").Replace('\\','/');var importer=(TextureImporter)AssetImporter.GetAtPath(asset);importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=PageSize;importer.alphaIsTransparency=true;importer.SaveAndReimport();}}
                }
                var pageHashes=new List<string>();foreach(var name in pageNames)pageHashes.Add(FileHash(Path.Combine(destination,name+".png")));
                File.WriteAllText(receiptPath,JsonUtility.ToJson(new Report{source=source,recipe=recipePath,id=doc.id,sourceNPC=doc.sourceNPC,output=destination,groundPolicy=recipe.groundPolicy,inputFingerprint=fingerprint,bakeImplementation="surface-cull-v1",surfaceAssignments=surfaces,librarySha256=preview?null:FileHash(Path.Combine(destination,"library.json")),pageSha256=pageHashes.ToArray(),passed=true,preview=preview,visualReviewPassed=false,frames=total,uniqueRasters=unique.Count,clips=doc.clips.Length,pages=pageNames.Count,width=width,height=height,standingHeight=recipe.height,worldUnitsPerPixel=recipe.units,outerInkWorldUnits=recipe.units*recipe.outerInkPixels,shadowClusterWorldUnits=recipe.units*recipe.shadowClusterPixels,depthRange=depthRange,palette=palette.ExportRamps()},true));exit=0;
            }catch(Exception error){Debug.LogException(error);}finally{foreach(var item in owned)if(item)Object.DestroyImmediate(item);}return exit;
        }
    }
}
#endif
