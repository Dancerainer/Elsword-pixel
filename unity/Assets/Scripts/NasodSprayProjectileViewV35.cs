using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class NasodSprayBallDataV35 {
            public string effect,sourceMesh,sourceSha,textureSha;
            public float duration;public int count,fps,vertices;public int[] triangles;public float[] uv,values;
        }
        sealed class NasodSprayBallSetV35 {internal NasodSprayBallDataV35 data;internal Material material;}
        sealed class NasodSprayBallViewV35 {
            internal NasodSprayBallSetV35 set;internal GameObject root;internal Mesh mesh;internal MeshRenderer renderer;
            internal Vector3[] vertices;internal readonly List<int> front=new List<int>(672);
        }
        readonly Dictionary<string,NasodSprayBallSetV35> sprayBallSetsV35=new Dictionary<string,NasodSprayBallSetV35>();
        readonly Dictionary<NpcEffectInstance,NasodSprayBallViewV35> sprayBallsV35=new Dictionary<NpcEffectInstance,NasodSprayBallViewV35>();
        readonly HashSet<NpcEffectInstance> sprayBallSeenV35=new HashSet<NpcEffectInstance>();
        readonly List<NpcEffectInstance> sprayBallStaleV35=new List<NpcEffectInstance>();
        static string NasodSprayShaV35(byte[] bytes){using(var sh=SHA256.Create())return BitConverter.ToString(sh.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
        NasodSprayBallSetV35 LoadNasodSprayBallV35(string npc){
            bool ice=npc==NasodSprayRigV35.Ice;string root=NasodSprayRigV35.Root(npc);
            var a=Resources.Load<TextAsset>(root+"projectile");var atlas=Resources.Load<Texture2D>(root+"projectile-texture");
            string pin=ice?"71518a25439d9d15a82c6f09af89b69741586fd4afb63179c1d457371fc5d849":"12478d6d43ae304ceee31d2b63d8c8fb53244ff1f227e019690481acb0e996e4";
            if(!a||!atlas||atlas.width!=128||atlas.height!=128||!atlas.isReadable||NasodSprayShaV35(a.bytes)!=pin)
                throw new InvalidOperationException("Original Nasod Spray B mesh/texture "+npc);
            var d=JsonUtility.FromJson<NasodSprayBallDataV35>(a.text);
            if(d.effect!=(ice?GameModel.IceSprayShot:GameModel.SprayShot)||d.vertices!=153||d.uv.Length!=306||
                d.triangles.Length!=672||d.values.Length!=d.count*153*3||d.fps!=30||d.duration<=0)
                throw new InvalidOperationException("Original 224 visible Sphere01 triangles "+npc);
            var shader=Resources.Load<Shader>("Pixel/Shaders/CargoSourceFxV21");
            if(!shader||!shader.isSupported)throw new InvalidOperationException("Source alpha/destination alpha projectile material");
            var material=new Material(shader);material.mainTexture=atlas;
            material.SetInt("_SrcBlend",(int)BlendMode.SrcAlpha);material.SetInt("_DstBlend",(int)BlendMode.DstAlpha);
            material.color=ice?new Color(.8f,.8f,.8f,.8f):Color.white;
            return new NasodSprayBallSetV35{data=d,material=material};
        }
        NasodSprayBallViewV35 CreateNasodSprayBallV35(NpcEffectInstance fx){
            string npc=fx.owner.sourceNPC;
            if(!sprayBallSetsV35.TryGetValue(npc,out var set)){set=LoadNasodSprayBallV35(npc);sprayBallSetsV35.Add(npc,set);}
            var obj=new GameObject("Original "+fx.id+" Sphere01"){layer=effectsRoot.gameObject.layer};
            obj.transform.SetParent(effectsRoot,false);var mesh=new Mesh{name="Original Normal "+fx.id};mesh.MarkDynamic();
            var points=new Vector3[set.data.vertices];mesh.vertices=points;var uv=new Vector2[points.Length];var colors=new Color32[points.Length];
            for(int i=0;i<points.Length;i++){uv[i]=new Vector2(set.data.uv[2*i],set.data.uv[2*i+1]);colors[i]=new Color32(255,255,255,255);}
            mesh.uv=uv;mesh.colors32=colors;obj.AddComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=obj.AddComponent<MeshRenderer>();renderer.sharedMaterial=set.material;renderer.sortingOrder=44;
            return new NasodSprayBallViewV35{set=set,root=obj,mesh=mesh,renderer=renderer,vertices=points};
        }
        void DrawNasodSprayBallV35(NpcEffectInstance fx,float age,Vector3 position){
            if(!sprayBallsV35.TryGetValue(fx,out var view)){view=CreateNasodSprayBallV35(fx);sprayBallsV35.Add(fx,view);}
            var d=view.set.data;float f=Mathf.Repeat(age,d.duration)*d.fps;int a=Mathf.Clamp(Mathf.FloorToInt(f),0,d.count-1),b=Math.Min(a+1,d.count-1);float mix=f-a;
            for(int i=0;i<d.vertices;i++){
                int ia=(a*d.vertices+i)*3,ib=(b*d.vertices+i)*3;
                view.vertices[i]=new Vector3(Mathf.Lerp(d.values[ia],d.values[ib],mix),Mathf.Lerp(d.values[ia+1],d.values[ib+1],mix),Mathf.Lerp(d.values[ia+2],d.values[ib+2],mix));
            }
            view.front.Clear();
            for(int i=0;i<d.triangles.Length;i+=3){
                int ia=d.triangles[i],ib=d.triangles[i+1],ic=d.triangles[i+2];Vector3 u=view.vertices[ib]-view.vertices[ia],v=view.vertices[ic]-view.vertices[ia];
                // The original opaque textured sphere uses only camera-facing faces.
                // Cargo's shared blend shader also serves two-sided particle cards.
                if((u.x*v.y-u.y*v.x)*fx.facing>=0)continue;
                view.front.Add(ia);view.front.Add(ib);view.front.Add(ic);
            }
            view.mesh.vertices=view.vertices;view.mesh.SetTriangles(view.front,0,true);
            var art=game.NpcEffectVisual(fx,age);var tr=view.root.transform;
            tr.localPosition=new Vector3(position.x,position.y,0);tr.localRotation=Quaternion.Euler(0,0,art.angle);
            tr.localScale=new Vector3(art.scaleX*fx.facing,art.scaleY,fx.ownerScale*fx.scaleZ*fx.facing)*.28f;
            npcEffectDepths[tr]=fx.owner.nativeDepth+position.z/(NativeWorldActive?nativeScene.unit:.28f);
        }
        void ReleaseNasodSprayBallV35(NasodSprayBallViewV35 view){
            if(view.root){npcEffectDepths.Remove(view.root.transform);view.root.SetActive(false);DestroyNativeWorldObject(view.root);}
            if(view.mesh)DestroyNativeWorldObject(view.mesh);
        }
        void ClearNasodSprayBallsV35(){
            foreach(var v in sprayBallsV35.Values)ReleaseNasodSprayBallV35(v);sprayBallsV35.Clear();sprayBallSeenV35.Clear();sprayBallStaleV35.Clear();
            foreach(var v in sprayBallSetsV35.Values)if(v.material)DestroyNativeWorldObject(v.material);sprayBallSetsV35.Clear();
        }
        void RenderNasodSprayBallsV35(){
            if(game==null||!effectsRoot||(!PlainProductSessionActiveR1320&&!CoreHardProductSessionActiveR1147)){
                if(sprayBallSetsV35.Count>0)ClearNasodSprayBallsV35();return;
            }
            sprayBallSeenV35.Clear();
            foreach(var fx in game.npcEffects){
                if(fx.owner==null||fx.ended||fx.age>=fx.lifetime||
                    !(fx.id==GameModel.SprayShot&&fx.owner.sourceNPC==NasodSprayRigV35.Plain ||
                      fx.id==GameModel.IceSprayShot&&fx.owner.sourceNPC==NasodSprayRigV35.Ice))continue;
                float age=fx.age;var at=new Vector3(fx.x,fx.y,fx.z);
                if(!calibrationRun&&previousRoom==game.room&&previousNpcEffects.TryGetValue(fx,out var prior)&&prior.reflected==fx.reflected&&fx.age>=prior.age){
                    age=Mathf.Lerp(prior.age,age,RenderAlpha);at=Vector3.Lerp(prior.position,at,RenderAlpha);
                }
                sprayBallSeenV35.Add(fx);DrawNasodSprayBallV35(fx,age,at);
            }
            sprayBallStaleV35.Clear();foreach(var pair in sprayBallsV35)if(!sprayBallSeenV35.Contains(pair.Key))sprayBallStaleV35.Add(pair.Key);
            foreach(var fx in sprayBallStaleV35){ReleaseNasodSprayBallV35(sprayBallsV35[fx]);sprayBallsV35.Remove(fx);}
        }
        internal MeshRenderer NasodSprayBallRendererV35(NpcEffectInstance fx){
            return fx!=null&&sprayBallsV35.TryGetValue(fx,out var view)?view.renderer:null;
        }
    }
}
