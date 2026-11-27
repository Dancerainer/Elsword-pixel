using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] internal sealed class TreeBendersRasterFrame {public int page,x,y,w,h;public float px,py;}
    [Serializable] internal sealed class TreeBendersRasterClip {public string name;public float fps,duration;public bool loop;public TreeBendersRasterFrame[] frames;}
    [Serializable] internal sealed class TreeBendersRasterData {public float pixelsPerUnit;public string[] pages;public TreeBendersRasterClip[] clips;}
    // Original complete-model poses are baked at 60 Hz. No runtime limb fitting,
    // per-foot scaling, synthetic gait or cross-shoulder hand placement remains.
    internal sealed class TreeBendersFrames : IDisposable {
        const string Root="Pixel/tree-v52/benders/";
        static TreeBendersRasterData data;
        static Texture2D[] textures;static int users;
        static readonly Dictionary<string,Sprite[]> clips=new Dictionary<string,Sprite[]>(StringComparer.Ordinal);
        static readonly Dictionary<string,TreeBendersRasterClip> timing=new Dictionary<string,TreeBendersRasterClip>(StringComparer.Ordinal);
        readonly SpriteRenderer carrier,view;
        readonly Action<UnityEngine.Object> destroy;
        readonly MaterialPropertyBlock block=new MaterialPropertyBlock();
        bool disposed;
        internal SpriteRenderer Renderer {get{return view;}}
        internal static TreeBendersRasterData Data {get{Load();return data;}}
        internal static Sprite StandingSprite {get{Load();return clips["Wait"][0];}}
        static void Load() {
            if(data!=null)return;
            var asset=Resources.Load<TextAsset>(Root+"library");
            if(!asset)throw new InvalidOperationException("Missing Benders 60 Hz raster library");
            var parsed=JsonUtility.FromJson<TreeBendersRasterData>(asset.text);
            var pages=new Texture2D[parsed.pages.Length];
            for(int i=0;i<pages.Length;i++){pages[i]=Resources.Load<Texture2D>(Root+parsed.pages[i]);if(!pages[i])throw new InvalidOperationException("Missing Benders frame page");}
            foreach(var c in parsed.clips) {
                if(c.fps!=60||c.frames.Length<2)throw new InvalidOperationException("Benders must use dense 60 Hz source frames: "+c.name);
                var sprites=new Sprite[c.frames.Length];
                for(int i=0;i<sprites.Length;i++){var f=c.frames[i];sprites[i]=Sprite.Create(pages[f.page],new Rect(f.x,f.y,f.w,f.h),new Vector2(f.px/f.w,f.py/f.h),parsed.pixelsPerUnit,0,SpriteMeshType.FullRect);}
                clips.Add(c.name,sprites);timing.Add(c.name,c);
            }
            textures=pages;data=parsed;
        }
        internal TreeBendersFrames(SpriteRenderer carrier,Action<UnityEngine.Object> destroy) {
            Load();users++;this.carrier=carrier;this.destroy=destroy;
            var go=new GameObject("Benders original 60 fps whole frames"){layer=carrier.gameObject.layer};
            go.transform.SetParent(carrier.transform.parent,false);view=go.AddComponent<SpriteRenderer>();
        }
        internal void Apply(FairyAuthoredPartsV33.Frame frame,Vector3 projectedFoot) {
            if(!timing.TryGetValue(frame.alias,out var clip))throw new InvalidOperationException("Unbaked Benders source clip "+frame.alias);
            float t=Mathf.Clamp(frame.sourceClock,0,clip.duration);
            int index=Mathf.Clamp(Mathf.FloorToInt(t*60+.0001f),0,clip.frames.Length-1);
            view.sprite=clips[frame.alias][index];view.transform.position=projectedFoot;
            view.transform.localScale=new Vector3(frame.facing,1,1);view.transform.localRotation=Quaternion.identity;
            view.sharedMaterial=carrier.sharedMaterial;view.color=carrier.color;view.maskInteraction=carrier.maskInteraction;
            view.sortingLayerID=carrier.sortingLayerID;view.sortingOrder=carrier.sortingOrder;
            carrier.GetPropertyBlock(block);block.SetTexture("_MainTex",view.sprite.texture);
            var texture=view.sprite.texture;block.SetVector("_MainTex_TexelSize",new Vector4(1f/texture.width,1f/texture.height,texture.width,texture.height));
            // Layered pixel ink, costume seams and clustered shade are baked together.
            // Keep armor/flash/depth inherited without double-stroking the art.
            block.SetFloat("_MonsterInk",0);view.SetPropertyBlock(block);
            view.enabled=frame.visible&&carrier.enabled&&carrier.gameObject.activeInHierarchy;carrier.forceRenderingOff=true;
        }
        internal void Suspend(){view.enabled=false;carrier.forceRenderingOff=true;}
        public void Dispose(){
            if(disposed)return;disposed=true;
            if(carrier)carrier.forceRenderingOff=false;if(view){view.enabled=false;destroy(view.gameObject);}
            if(--users!=0)return;
            foreach(var sprites in clips.Values)foreach(var sprite in sprites)destroy(sprite);
            clips.Clear();timing.Clear();if(textures!=null)foreach(var page in textures)Resources.UnloadAsset(page);
            textures=null;data=null;
        }
    }
}
