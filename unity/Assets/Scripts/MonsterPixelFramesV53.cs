using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // A library uses the accepted v52 format unchanged. Source time is selected by
    // the existing presentation adapters; this class never advances an actor.
    [Serializable] internal sealed class MonsterPixelAliasV53 {
        public string alias,motion,route;
        public float displayDuration,sourceDuration;
        public bool hold,loop;
    }
    [Serializable] internal sealed class MonsterPixelStateV53 {public string state,motion;public float speed,sourceDuration;}
    [Serializable] internal sealed class MonsterContactRootCurveV58 {public string motion;public float[] y;}
    [Serializable] internal sealed class MonsterPixelActorV53 {
        public string npc,kind,root,standingMotion,facingPolicy="actor",stateClockPolicy,assembly;
        public float standingHeight,scale=1,rootOffsetX,rootOffsetY;
        public int authoredFacing=1;
        public MonsterPixelAliasV53[] aliases;
        public string[] directSourceProviders;
        public MonsterPixelStateV53[] states;
        public MonsterContactRootCurveV58[] contactRootCurves;
    }
    [Serializable] internal sealed class MonsterPixelRosterV53 {public MonsterPixelActorV53[] actors;}

    internal sealed class MonsterPixelFramesV53 : IDisposable {
        internal readonly MonsterPixelActorV53 actor;
        internal readonly TreeBendersRasterData data;
        readonly Dictionary<string,TreeBendersRasterClip> clips=new Dictionary<string,TreeBendersRasterClip>(StringComparer.Ordinal);
        readonly Dictionary<string,Sprite[]> sprites=new Dictionary<string,Sprite[]>(StringComparer.Ordinal);
        readonly Dictionary<int,Texture2D> pages=new Dictionary<int,Texture2D>();
        readonly Dictionary<int,int> pageLastUse=new Dictionary<int,int>();
        readonly Action<UnityEngine.Object> destroy;
        struct BuiltFrame {internal Sprite[] slots;internal int index;}
        readonly Dictionary<int,List<BuiltFrame>> builtPages=new Dictionary<int,List<BuiltFrame>>();
        bool disposed;
        internal int ResidentPages {get{return pages.Count;}}
        internal long ResidentBytes {get{long bytes=0;foreach(var p in pages.Values)if(p)bytes+=(long)p.width*p.height*(p.isReadable?8:4);return bytes;}}

        internal Func<string,Texture2D> takePreparedPage;
        internal MonsterPixelFramesV53(MonsterPixelActorV53 actor,Action<UnityEngine.Object> destroy)
            : this(actor, destroy, null) { }
        internal MonsterPixelFramesV53(MonsterPixelActorV53 actor,Action<UnityEngine.Object> destroy,TreeBendersRasterData prepared) {
            this.actor=actor;this.destroy=destroy;
            if(actor==null||string.IsNullOrEmpty(actor.root)||actor.scale<=0||(actor.authoredFacing!=1&&actor.authoredFacing!=-1))throw new InvalidOperationException("Invalid v53 actor registration");
            if(prepared==null){
                var text=Resources.Load<TextAsset>(actor.root.TrimEnd('/')+"/library");
                if(!text)throw new InvalidOperationException("Missing v53 whole library "+actor.root);
                prepared=JsonUtility.FromJson<TreeBendersRasterData>(text.text);
            }
            data=prepared;
            if(data==null||data.pixelsPerUnit<=0||data.pages==null||data.clips==null)throw new InvalidOperationException("Invalid v53 library "+actor.root);
            foreach(var clip in data.clips){
                if(clip==null||string.IsNullOrEmpty(clip.name)||clip.fps!=60||clip.duration<0||clip.frames==null||clip.frames.Length<1)throw new InvalidOperationException("Whole source frames must be 60 Hz: "+actor.kind);
                // The final source endpoint is included, even if its duration is
                // not a whole number of 1/60 seconds. Static poses may have one frame.
                int expected=(int)Math.Ceiling(clip.duration*60-.001f)+1;
                if(clip.frames.Length<expected)throw new InvalidOperationException("Sparse v53 source motion "+actor.kind+"/"+clip.name);
                clips.Add(clip.name,clip);sprites.Add(clip.name,new Sprite[clip.frames.Length]);
                foreach(var frame in clip.frames)if(frame==null||frame.page<0||frame.page>=data.pages.Length||frame.x<0||frame.y<0||frame.w<=0||frame.h<=0||!Finite(frame.px)||!Finite(frame.py))throw new InvalidOperationException("Invalid v53 rect/pivot "+actor.kind+"/"+clip.name);
            }
        }
        internal static bool Finite(float value){return !float.IsNaN(value)&&!float.IsInfinity(value);}
        internal bool TryClip(string motion,out TreeBendersRasterClip clip){return clips.TryGetValue(motion??"",out clip);}
        internal string PagePath(int index){
            string path=data.pages[index];
            return path.StartsWith("Pixel/",StringComparison.Ordinal)?path:actor.root.TrimEnd('/')+"/"+path;
        }
        internal bool PageResident(int index){return pages.TryGetValue(index,out var texture)&&texture;}
        internal Sprite At(string motion,float sourceClock,out int frameIndex,out int pageIndex) {
            if(disposed)throw new ObjectDisposedException(actor.kind);
            if(!clips.TryGetValue(motion??"",out var clip))throw new InvalidOperationException("Unbaked v53 source motion "+actor.kind+"/"+motion);
            if(!Finite(sourceClock)||sourceClock<0)throw new InvalidOperationException("Invalid exact source clock "+motion);
            // Looping belongs to the source selector. In particular, an explicitly
            // held nonloop endpoint must not unexpectedly become frame zero here.
            float clock=Mathf.Min(sourceClock,clip.duration);
            frameIndex=clock+0.00001f>=clip.duration?clip.frames.Length-1:Mathf.Clamp(Mathf.FloorToInt(clock*60+.0001f),0,clip.frames.Length-1);
            var frame=clip.frames[frameIndex];pageIndex=frame.page;pageLastUse[pageIndex]=Time.frameCount;
            var values=sprites[motion];if(values[frameIndex]&&values[frameIndex].texture)return values[frameIndex];
            if(values[frameIndex]){destroy(values[frameIndex]);values[frameIndex]=null;}
            if(!pages.TryGetValue(pageIndex,out var texture)||!texture){
                string path=PagePath(pageIndex);
                texture=takePreparedPage?.Invoke(path);
                if(!texture)texture=Resources.Load<Texture2D>(path);if(!texture)throw new InvalidOperationException("Missing v53 source page "+path);
                texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;pages[pageIndex]=texture;
            }
            if(frame.x+frame.w>texture.width||frame.y+frame.h>texture.height)throw new InvalidOperationException("v53 frame outside imported atlas "+actor.kind+"/"+motion);
            var sprite=Sprite.Create(texture,new Rect(frame.x,frame.y,frame.w,frame.h),new Vector2(frame.px/frame.w,frame.py/frame.h),data.pixelsPerUnit,0,SpriteMeshType.FullRect);
            sprite.name="monster-v53/"+actor.kind+"/"+motion+"/"+frameIndex;values[frameIndex]=sprite;
            if(!builtPages.TryGetValue(pageIndex,out var built))builtPages.Add(pageIndex,built=new List<BuiltFrame>());
            built.Add(new BuiltFrame{slots=values,index=frameIndex});
            return sprite;
        }
        internal long Trim(HashSet<int> protectedPages,int unusedFrames) {
            long bytes=0;
            var retired=new List<int>();foreach(var page in pages.Keys)if(!protectedPages.Contains(page)&&Time.frameCount-pageLastUse[page]>=unusedFrames)retired.Add(page);
            foreach(int page in retired){
                var texture=pages[page];if(texture)bytes+=(long)texture.width*texture.height*(texture.isReadable?8:4);
                if(builtPages.TryGetValue(page,out var built)){ReleaseBuilt(built);builtPages.Remove(page);}
                // v52 may still use the same Resources texture for its intro. The
                // normal Unity unused-asset sweep releases it once all owners agree.
                pages.Remove(page);pageLastUse.Remove(page);
            }
            return bytes;
        }
        void ReleaseBuilt(List<BuiltFrame> built){
            foreach(var frame in built){var sprite=frame.slots[frame.index];if(sprite)destroy(sprite);frame.slots[frame.index]=null;}
        }
        public void Dispose(){if(disposed)return;disposed=true;foreach(var built in builtPages.Values)ReleaseBuilt(built);builtPages.Clear();sprites.Clear();clips.Clear();pages.Clear();pageLastUse.Clear();}
    }

    internal sealed class MonsterPixelViewV53 : IDisposable {
        internal readonly SpriteRenderer carrier,renderer;
        readonly Action<UnityEngine.Object> destroy;
        readonly MaterialPropertyBlock properties=new MaterialPropertyBlock();
        internal MonsterPixelViewV53(SpriteRenderer carrier,Action<UnityEngine.Object> destroy){
            this.carrier=carrier;this.destroy=destroy;
            var node=new GameObject("Monster original 60 fps / "+carrier.name){layer=carrier.gameObject.layer};
            node.transform.SetParent(carrier.transform.parent,false);renderer=node.AddComponent<SpriteRenderer>();
        }
        internal void Apply(Sprite sprite,Vector3 projectedRoot,float scale,int facing,bool visible,float sourceRotationZ=0){
            renderer.sprite=sprite;renderer.transform.position=projectedRoot;renderer.transform.localScale=new Vector3(scale*facing,scale,1);renderer.transform.localRotation=Quaternion.Euler(0,0,sourceRotationZ);
            renderer.sharedMaterial=carrier.sharedMaterial;renderer.color=carrier.color;renderer.maskInteraction=carrier.maskInteraction;renderer.sortingLayerID=carrier.sortingLayerID;renderer.sortingOrder=carrier.sortingOrder;
            carrier.GetPropertyBlock(properties);properties.SetTexture("_MainTex",sprite.texture);var t=sprite.texture;properties.SetVector("_MainTex_TexelSize",new Vector4(1f/t.width,1f/t.height,t.width,t.height));properties.SetFloat("_MonsterInk",0);renderer.SetPropertyBlock(properties);
            renderer.enabled=visible&&carrier.enabled&&carrier.gameObject.activeInHierarchy;carrier.forceRenderingOff=true;
        }
        public void Dispose(){if(carrier)carrier.forceRenderingOff=false;if(renderer){renderer.enabled=false;destroy(renderer.gameObject);}}
    }
}
