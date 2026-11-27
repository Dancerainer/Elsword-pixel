using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        // Keep at most the immediate next ordinary room. Imported textures are
        // shared Resources assets; the meshes created here belong to this job
        // until BuildNativeWorld takes them.
        const int WarmJsonBytes=512*1024,WarmGeometryBytes=2*1024*1024;
        const int WarmTextureCount=32,WarmPartCount=512,WarmPartVertices=4096;
        const long WarmTextureBytes=16L*1024*1024,WarmMeshBytes=4L*1024*1024;
        static readonly long WarmFrameTicks=Math.Max(1,Stopwatch.Frequency/500);

        sealed class MobileRoomWarmup {
            public StageArt stage;
            public RoomDefinition room;
            public Transform stagingRoot;
            public MeshRenderer[] renderers;
            public bool[][] walkingFaces;
            public Bounds[][] cameraBlocks;
            public Coroutine routine;
            public NativeScene scene;
            public byte[] geometry;
            public Mesh[] meshes;
            public Vector3[][] vertices;
            public readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);

            public bool TakeMesh(int i,out Mesh mesh,out Vector3[] positions){
                mesh=null;positions=null;
                if(meshes==null||!meshes[i])return false;
                mesh=meshes[i];positions=vertices[i];meshes[i]=null;vertices[i]=null;
                return true;
            }
            public void Release(){
                if(meshes!=null)foreach(var mesh in meshes)if(mesh)DestroyNativeWorldObject(mesh);
                // Renderers that were adopted have already moved out of the inactive hierarchy.
                if(renderers!=null)foreach(var renderer in renderers)
                    if(renderer && renderer.sharedMaterial)DestroyNativeWorldObject(renderer.sharedMaterial);
                if(stagingRoot)DestroyNativeWorldObject(stagingRoot.gameObject);
                renderers=null;walkingFaces=null;cameraBlocks=null;stagingRoot=null;room=null;
                meshes=null;vertices=null;geometry=null;scene=null;textures.Clear();
            }
        }
        MobileRoomWarmup mobileRoomWarmup;

        void CancelMobileRoomWarmup(){
            var job=mobileRoomWarmup;mobileRoomWarmup=null;
            if(job==null)return;
            if(job.routine!=null)StopCoroutine(job.routine);
            job.Release();
        }

        MobileRoomWarmup TakeMobileRoomWarmup(StageArt stage){
            var job=mobileRoomWarmup;
            if(job==null)return null;
            if(!ReferenceEquals(job.stage,stage)){CancelMobileRoomWarmup();return null;}
            mobileRoomWarmup=null;
            if(job.routine!=null)StopCoroutine(job.routine);
            job.routine=null;
            return job;
        }

        void StartNextMobileRoomWarmup(StageArt current){
            if(!StreamMobileArt||!Application.isPlaying||calibrationRun||!adventureStarted||
               selectedDungeon<0||selectedDungeon>=catalog.dungeons.Length)return;
            var route=catalog.dungeons[selectedDungeon];
            if(game.room+1>=route.count)return;
            int index=route.start+game.room;
            if(index<0||index+1>=catalog.stages.Length||!ReferenceEquals(current,catalog.stages[index]))return;
            var next=catalog.stages[index+1];
            if(string.IsNullOrEmpty(next.nativeScene)||string.IsNullOrEmpty(next.nativeGeometry))return;
            CancelMobileRoomWarmup();
            var job=new MobileRoomWarmup{stage=next,room=game.NextRoomForWarmup};
            mobileRoomWarmup=job;job.routine=StartCoroutine(WarmMobileRoom(job));
        }

        IEnumerator WarmMobileRoom(MobileRoomWarmup job){
            var prepare=PrepareMobileRoom(job);
            try{
                while(true){
                    bool more;
                    try{more=prepare.MoveNext();}
                    catch(Exception error){
                        // A speculative load must not prevent normal room entry;
                        // already built parts remain eligible for handoff.
                        Debug.LogWarning("Skipping remaining room warmup: "+error.Message);
                        break;
                    }
                    if(!more)break;
                    yield return prepare.Current;
                }
            }finally{
                (prepare as IDisposable)?.Dispose();
                job.routine=null;
            }
        }

        IEnumerator PrepareMobileRoom(MobileRoomWarmup job){
            // Let the newly entered room render before starting any work for its successor.
            yield return null;
            var sceneRequest=Resources.LoadAsync<TextAsset>(job.stage.nativeScene);
            var binaryRequest=Resources.LoadAsync<TextAsset>(job.stage.nativeGeometry);
            sceneRequest.priority=binaryRequest.priority=-1;
            yield return sceneRequest;yield return binaryRequest;
            var document=sceneRequest.asset as TextAsset;
            var binary=binaryRequest.asset as TextAsset;
            if(!document||!binary||document.GetData<byte>().Length>WarmJsonBytes||
               binary.GetData<byte>().Length>WarmGeometryBytes)yield break;
            string json=document.text;
            // The string overload operates on a plain CLR object and can be
            // parsed off the main thread. Unity objects stay on the main thread.
            var parsed=Task.Run(()=>JsonUtility.FromJson<NativeScene>(json));
            while(!parsed.IsCompleted)yield return null;
            if(parsed.IsFaulted){Debug.LogWarning("Skipping room warmup: "+parsed.Exception.GetBaseException().Message);yield break;}
            if(parsed.IsCanceled||parsed.Result?.parts==null||parsed.Result.parts.Length>WarmPartCount)yield break;
            job.scene=parsed.Result;job.geometry=binary.bytes;
            var parts=job.scene.parts;
            job.meshes=new Mesh[parts.Length];job.vertices=new Vector3[parts.Length][];
            job.renderers=new MeshRenderer[parts.Length];job.walkingFaces=new bool[parts.Length][];job.cameraBlocks=new Bounds[parts.Length][];
            var names=new List<string>();var seen=new HashSet<string>(StringComparer.Ordinal);
            foreach(var part in parts)if(!string.IsNullOrEmpty(part.texture)&&seen.Add(part.texture))names.Add(part.texture);
            int nextTexture=0;
            long textureBytes=0;
            var meshWork=WarmNativeMeshes(job);
            bool pendingMeshes=true;
            ResourceRequest textureRequest=null;string texturePath=null;
            while(pendingMeshes||textureRequest!=null||nextTexture<names.Count){
                if(textureRequest!=null&&textureRequest.isDone){
                    var texture=textureRequest.asset as Texture2D;
                    if(texture){
                        long bytes=Profiler.GetRuntimeMemorySizeLong(texture);
                        if(bytes<=0)bytes=(long)texture.width*texture.height*(texture.isReadable?8:4);
                        if(textureBytes+bytes<=WarmTextureBytes){job.textures.Add(texturePath,texture);textureBytes+=bytes;}
                        else nextTexture=names.Count;
                    }
                    textureRequest=null;
                }
                if(textureRequest==null&&nextTexture<names.Count){
                    if(job.textures.Count>=WarmTextureCount||textureBytes>=WarmTextureBytes)nextTexture=names.Count;
                    else{
                        texturePath=names[nextTexture++];textureRequest=Resources.LoadAsync<Texture2D>(texturePath);
                        textureRequest.priority=-1;
                    }
                }
                if(pendingMeshes)pendingMeshes=meshWork.MoveNext();
                yield return null;
            }
            // Even if a texture exceeds the prefetch budget its mesh and material can be
            // staged without it. Bind that texture only when the new room actually opens.
            job.stagingRoot=new GameObject("Prepared native scenery").transform;
            job.stagingRoot.gameObject.SetActive(false);
            var shader=Resources.Load<Shader>("Pixel/Shaders/NativeWorld");
            long start=Stopwatch.GetTimestamp();
            for(int i=0;i<parts.Length;i++){
                var p=parts[i];Texture2D texture=null;
                if(!job.meshes[i])continue;
                if(!string.IsNullOrEmpty(p.texture))job.textures.TryGetValue(p.texture,out texture);
                var renderer=CreateNativeRoomRenderer(job.scene,p,i,job.meshes[i],shader,texture,job.stagingRoot);
                job.renderers[i]=renderer;
                job.cameraBlocks[i]=BuildCameraGeometryBlocksV25(job.vertices[i],p.name);
                if(IsRevealArchitecture(p.name)&&NativeSceneSolidOccluderShaderR1461(renderer.sharedMaterial.shader))
                    job.walkingFaces[i]=ProtectWalkingFaces(job.meshes[i],job.vertices[i],job.scene,job.room);
                if(Stopwatch.GetTimestamp()-start>=WarmFrameTicks){yield return null;start=Stopwatch.GetTimestamp();}
            }
        }

        IEnumerator WarmNativeMeshes(MobileRoomWarmup job){
            long start=Stopwatch.GetTimestamp(),bytes=0;
            for(int partIndex=0;partIndex<job.scene.parts.Length;partIndex++){
                var part=job.scene.parts[partIndex];
                long estimate=4096L+part.count*64L;
                if(part.count<=0||part.count>WarmPartVertices||bytes+estimate>WarmMeshBytes||
                   part.offset<0||part.offset+(long)part.count*20>job.geometry.Length)continue;
                var v=new Vector3[part.count];var uv=new Vector2[part.count];var ix=new int[part.count];
                for(int first=0;first<part.count;first+=64){
                    DecodeNativeVertices(part,job.geometry,v,uv,ix,first,Math.Min(first+64,part.count));
                    if(Stopwatch.GetTimestamp()-start>=WarmFrameTicks){yield return null;start=Stopwatch.GetTimestamp();}
                }
                var mesh=MakeNativePartMesh(part,v,uv,ix);
                long actual;
                try{
                    // Upload while still off-screen. Keep the same readable
                    // CPU mesh contract as a synchronously constructed part.
                    mesh.UploadMeshData(false);
                    actual=Profiler.GetRuntimeMemorySizeLong(mesh)+v.Length*12L;
                }catch{DestroyNativeWorldObject(mesh);throw;}
                if(bytes+Math.Max(actual,estimate)>WarmMeshBytes){DestroyNativeWorldObject(mesh);yield break;}
                bytes+=Math.Max(actual,estimate);job.meshes[partIndex]=mesh;job.vertices[partIndex]=v;
                if(Stopwatch.GetTimestamp()-start>=WarmFrameTicks){yield return null;start=Stopwatch.GetTimestamp();}
            }
        }

        static void DecodeNativeVertices(NativeWorldPart p,byte[] data,Vector3[] vertices,Vector2[] uv,int[] indices,int begin,int end){
            for(int i=begin;i<end;i++){
                int at=p.offset+i*20;
                vertices[i]=new Vector3(BitConverter.ToSingle(data,at),BitConverter.ToSingle(data,at+4),BitConverter.ToSingle(data,at+8));
                uv[i]=new Vector2(BitConverter.ToSingle(data,at+12),BitConverter.ToSingle(data,at+16));indices[i]=i;
            }
        }
        static Mesh MakeNativePartMesh(NativeWorldPart p,Vector3[] vertices,Vector2[] uv,int[] indices){
            var mesh=new Mesh{name=p.name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};
            try{mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=indices;mesh.RecalculateBounds();return mesh;}
            catch{DestroyNativeWorldObject(mesh);throw;}
        }
    }
}
