using System;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    [Serializable] public sealed class AncientSkinClipV37 {
        public string id,sourceAnimation;
        public float fps,duration;
        public bool loop;
        public FrameData[] frames;
    }
    [Serializable] public sealed class AncientSkinPageV37 { public string resource; public int w,h; }
    [Serializable] public sealed class AncientSkinLibraryV37 {
        public string id,artVersion,sourceMotionSha;
        public int sourceMotionCount,nativeYaw,authoredFacing,paintedPoseCount;
        public float standingHeight,logicalScale;
        public AncientSkinClipV37[] clips;
        public AncientSkinPageV37[] pages;
    }
    public sealed class AncientSkinPresenterV37 : IDisposable {
        public const string Root="Pixel/feedback-v37/ancient-phoru/";
        readonly AncientSkinLibraryV37 library;
        readonly Dictionary<string,AncientSkinClipV37> tracks=new Dictionary<string,AncientSkinClipV37>();
        readonly Dictionary<string,AncientSkinPageV37> pages=new Dictionary<string,AncientSkinPageV37>();
        readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
        readonly Dictionary<FrameData,Sprite> sprites=new Dictionary<FrameData,Sprite>();
        readonly MaterialPropertyBlock block=new MaterialPropertyBlock();
        readonly Action<Object> destroy;
        GameObject root;
        bool hidden;
        readonly bool previousForceOff;
        public SpriteRenderer Carrier {get;private set;}
        public SpriteRenderer Renderer {get;private set;}
        public string Alias {get;private set;}
        public string SourceAnimation {get;private set;}
        public float SourceClock {get;private set;}
        public int FrameIndex {get;private set;}
        public int Facing {get;private set;}
        public Vector3 ProjectedFoot {get;private set;}
        public bool Visible {get {return Renderer&&Renderer.enabled&&Renderer.gameObject.activeInHierarchy;}}
        public string PageResource {get;private set;}
        public float VisibleTop {get {return Visible?Renderer.bounds.max.y:float.NegativeInfinity;}}
        public Sprite SelectedSprite {get;private set;}

        public AncientSkinPresenterV37(SpriteRenderer carrier,Action<Object> destroyOwned) {
            if(!carrier||carrier.forceRenderingOff)throw new InvalidOperationException("Ancient skin carrier ownership");
            Carrier=carrier;previousForceOff=carrier.forceRenderingOff;destroy=destroyOwned;
            var manifest=Resources.Load<TextAsset>(Root+"library");
            if(!manifest)throw new InvalidOperationException("Ancient skin resource library missing");
            library=JsonUtility.FromJson<AncientSkinLibraryV37>(manifest.text);
            if(library==null||library.id!=AncientRigV35.Kind||library.artVersion!="native-uv-ksm-v37"||
                library.sourceMotionSha!=AncientRigV35.OriginalMotionSha||library.sourceMotionCount!=24||
                library.nativeYaw!=180||library.authoredFacing!=-1||library.paintedPoseCount!=1121||
                library.logicalScale<=0||Mathf.Abs(library.standingHeight-AncientRigV35.Height)>.005f)
                throw new InvalidOperationException("Ancient skin source registration");
            foreach(var clip in library.clips){
                if(clip==null||string.IsNullOrEmpty(clip.id)||string.IsNullOrEmpty(clip.sourceAnimation)||
                   clip.frames==null||clip.frames.Length==0||clip.fps!=24||clip.duration<=0)
                    throw new InvalidOperationException("Ancient skin clip registration");
                tracks.Add(clip.id,clip);
            }
            foreach(var page in library.pages){
                if(page==null||string.IsNullOrEmpty(page.resource)||!page.resource.StartsWith(Root,StringComparison.Ordinal)||
                   page.w<=0||page.h<=0)throw new InvalidOperationException("Ancient skin page registration");
                pages.Add(page.resource,page);
            }
            try {
                root=new GameObject("AncientPhoruOriginalSkinAnchorV37");
                root.SetActive(false);root.layer=carrier.gameObject.layer;root.transform.SetParent(carrier.transform,false);
                var view=new GameObject("AncientPhoruOriginalSkinV37");view.layer=root.layer;
                view.transform.SetParent(root.transform,false);Renderer=view.AddComponent<SpriteRenderer>();
            } catch { Dispose();throw; }
        }
        public static Texture2D LoadPage(AncientSkinPageV37 page, Action<Object> release){
            var encoded=Resources.Load<TextAsset>(page.resource);
            if(!encoded)throw new InvalidOperationException("Missing Ancient pixel page "+page.resource);
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name=page.resource,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
            try {
                if(!ImageConversion.LoadImage(texture,encoded.bytes,true)||texture.width!=page.w||texture.height!=page.h||texture.mipmapCount!=1)
                    throw new InvalidOperationException("Ancient pixel page dimensions "+page.resource);
                return texture;
            } catch { release(texture);throw; }
        }
        Sprite SpriteFor(FrameData frame, string id, int index) {
            if(frame==null)throw new InvalidOperationException("Ancient skin missing frame "+id);
            if(sprites.TryGetValue(frame,out var sprite))return sprite;
            if(!pages.TryGetValue(frame.texture,out var page)||!frame.hasAnchor||
               frame.x<0||frame.y<0||frame.w<=0||frame.h<=0||frame.x+frame.w>page.w||frame.y+frame.h>page.h)
                throw new InvalidOperationException("Ancient skin invalid crop "+id);
            if(!textures.TryGetValue(frame.texture,out var texture)){
                texture=LoadPage(page,destroy);
                textures.Add(frame.texture,texture);
            }
            sprite=Sprite.Create(texture,new Rect(frame.x,texture.height-frame.y-frame.h,frame.w,frame.h),
                new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
            sprite.name="ancient-phoru/v37/"+id+"/"+index;
            sprites.Add(frame,sprite);return sprite;
        }
        public bool Apply(string alias,float clock,int facing,AnimationData witness,Sprite[] originals,Vector3 foot,bool visible,Enemy enemy=null) {
            if(!tracks.TryGetValue(alias,out var track)||witness==null||originals==null||originals.Length!=track.frames.Length||
               witness.fps!=track.fps||witness.loop!=track.loop||Mathf.Abs(witness.duration-track.duration)>.00001f||
               clock<0||float.IsNaN(clock)||float.IsInfinity(clock)||(facing!=1&&facing!=-1))return false;
            float t=track.loop?Mathf.Repeat(clock,track.duration):Mathf.Min(clock,track.duration);
            int index=Mathf.Clamp(Mathf.FloorToInt(t*track.fps),0,track.frames.Length-1);
            if(!ReferenceEquals(Carrier.sprite,originals[index]))return false;
            if(OriginalReactionV45.TryPhysicalOrCommittedV45(enemy,out var hit,out float originalClock)&&track.sourceAnimation!=hit.rendered&&
               tracks.TryGetValue("ksm-"+hit.rendered,out var exact)){
                track=exact;alias=exact.id;clock=originalClock;
                index=Mathf.Clamp(Mathf.FloorToInt(clock*track.fps),0,track.frames.Length-1);
            }
            var frame=track.frames[index];
            var sprite=SpriteFor(frame,alias,index);
            var material=Carrier.sharedMaterial;
            if(!material||!material.shader||material.shader.name!="ElswordPixel/ArmoredSprite"||!material.shader.isSupported)
                throw new InvalidOperationException("Ancient skin source ArmoredSprite material");
            root.transform.localRotation=Quaternion.identity;
            root.transform.localScale=new Vector3(facing*library.authoredFacing*library.logicalScale,
                (Carrier.flipY?-1:1)*library.logicalScale,1);
            root.transform.position=foot;
            Renderer.transform.localPosition=new Vector3(frame.w*.5f-frame.anchorX,frame.anchorY-frame.h*.5f,0);
            Renderer.sprite=sprite;Renderer.sortingLayerID=Carrier.sortingLayerID;Renderer.sortingOrder=Carrier.sortingOrder;
            Renderer.sharedMaterial=material;Renderer.maskInteraction=Carrier.maskInteraction;Renderer.color=Carrier.color;
            var texture=sprite.texture;
            Carrier.GetPropertyBlock(block);block.SetTexture("_MainTex",texture);
            block.SetVector("_MainTex_TexelSize",new Vector4(1f/texture.width,1f/texture.height,texture.width,texture.height));
            Renderer.SetPropertyBlock(block);
            root.SetActive(visible&&Carrier.enabled&&Carrier.gameObject.activeInHierarchy);
            Carrier.forceRenderingOff=true;hidden=true;
            Alias=alias;SourceAnimation=track.sourceAnimation;SourceClock=clock;FrameIndex=index;
            Facing=facing;ProjectedFoot=foot;PageResource=frame.texture;SelectedSprite=sprite;
            return true;
        }
        public void Dispose() {
            if(root)root.SetActive(false);
            if(hidden&&Carrier)Carrier.forceRenderingOff=previousForceOff;
            hidden=false;
            if(root){root.transform.SetParent(null,false);destroy(root);}
            foreach(var sprite in sprites.Values)if(sprite)destroy(sprite);
            foreach(var texture in textures.Values)if(texture)destroy(texture);
            sprites.Clear();textures.Clear();root=null;Carrier=null;Renderer=null;SelectedSprite=null;
        }
    }
}
