using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    [Serializable] public sealed class MagePageV38 {public string resource,encodedSha256;public int w,h,encodedBytes;}
    [Serializable] public sealed class MageClipV38 {
        public string id,sourceAnimation;public float fps,duration;public bool loop;public FrameData[] frames;
    }
    [Serializable] public sealed class MageLibraryV38 {
        public string id,npc,artVersion,sourceMotionSha;public int sourceMotionCount,nativeYaw,authoredFacing,paintedPoseCount;
        public float standingHeight,logicalScale;public MagePageV38[] pages;public MageClipV38[] clips;
    }
    public class NativePoseSpriteBankV38 {
        public readonly MageLibraryV38 Data;
        readonly Dictionary<string,MagePageV38> pages=new Dictionary<string,MagePageV38>();
        readonly Dictionary<string,MageClipV38> clips=new Dictionary<string,MageClipV38>();
        readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
        readonly Dictionary<FrameData,Sprite> sprites=new Dictionary<FrameData,Sprite>();
        readonly Action<Object> destroy;
        readonly Dictionary<Sprite,Vector2> offsets;
        protected NativePoseSpriteBankV38(string kind,string npc,Action<Object> destroyOwned,Dictionary<Sprite,Vector2> spriteOffsets,Func<MageLibraryV38,bool> matchesSource) {
            destroy=destroyOwned;offsets=spriteOffsets;
            string prefix="Pixel/feedback-v38/"+kind+"/";
            var asset=Resources.Load<TextAsset>(prefix+"library");
            if(!asset)throw new InvalidOperationException("Missing magician sprite library "+kind);
            Data=JsonUtility.FromJson<MageLibraryV38>(asset.text);
            if(Data==null||Data.id!=kind||Data.npc!=npc||!matchesSource(Data))
                throw new InvalidOperationException("Original pose bank registration "+kind);
            foreach(var page in Data.pages){
                if(!page.resource.StartsWith(prefix,StringComparison.Ordinal)||page.w<=0||page.h<=0||
                   page.encodedBytes<=0||page.encodedSha256==null||page.encodedSha256.Length!=64)throw new InvalidOperationException("Invalid magician atlas metadata");
                pages.Add(page.resource,page);
            }
            foreach(var c in Data.clips){
                if(c==null||string.IsNullOrEmpty(c.sourceAnimation)||string.IsNullOrEmpty(c.id)||c.frames==null||
                   c.frames.Length==0||c.duration<=0||c.fps<=0)throw new InvalidOperationException("Invalid magician pose registration");
                clips.Add(c.id,c);
            }
        }
        Texture2D Page(MagePageV38 page){
            if(textures.TryGetValue(page.resource,out var texture))return texture;
            var asset=Resources.Load<TextAsset>(page.resource);
            if(!asset)throw new InvalidOperationException("Missing magician PNG bytes "+page.resource);
            byte[] bytes=asset.bytes;
            if(bytes.Length!=page.encodedBytes)throw new InvalidOperationException("Truncated magician PNG "+page.resource);
            using(var hash=SHA256.Create()){
                if(!String.Equals(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-",""),page.encodedSha256,StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Magician PNG hash "+page.resource);
            }
            texture=new Texture2D(2,2,TextureFormat.RGBA32,false){name=page.resource,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
            try{
                if(!ImageConversion.LoadImage(texture,bytes,true)||texture.width!=page.w||texture.height!=page.h||texture.mipmapCount!=1)
                    throw new InvalidOperationException("Magician PNG decode "+page.resource);
                texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
                textures.Add(page.resource,texture);return texture;
            }catch{destroy(texture);throw;}
        }
        public bool Select(string alias,float clock,AnimationData carrierClip,Sprite[] carrierFrames,Sprite carrierPose,
                           out MageClipV38 c,out int index,out Sprite pose){
            c=null;index=-1;pose=null;
            if(string.IsNullOrEmpty(alias)||!clips.TryGetValue(alias,out c)||carrierClip==null||carrierFrames==null||
               c.frames.Length!=carrierFrames.Length||c.fps!=carrierClip.fps||c.loop!=carrierClip.loop||
               Mathf.Abs(c.duration-carrierClip.duration)>.00001f||float.IsNaN(clock)||float.IsInfinity(clock))return false;
            float t=c.loop?Mathf.Repeat(clock,c.duration):clock;
            index=Mathf.Clamp(Mathf.FloorToInt(t*c.fps),0,c.frames.Length-1);
            if(!ReferenceEquals(carrierFrames[index],carrierPose))return false;
            var frame=c.frames[index];
            if(frame==null)throw new InvalidOperationException("Missing magician crop "+alias);
            if(sprites.TryGetValue(frame,out pose))return true;
            if(!frame.hasAnchor||!pages.TryGetValue(frame.texture,out var page)||frame.x<0||frame.y<0||
               frame.w<=0||frame.h<=0||frame.x+frame.w>page.w||frame.y+frame.h>page.h)throw new InvalidOperationException("Magician crop "+alias);
            var texture=Page(page);
            pose=Sprite.Create(texture,new Rect(frame.x,texture.height-frame.y-frame.h,frame.w,frame.h),
                               new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
            pose.name=Data.id+"/"+alias+"/"+index;
            sprites.Add(frame,pose);offsets[pose]=new Vector2(frame.w*.5f-frame.anchorX,frame.anchorY-frame.h*.5f);
            return true;
        }
        public void Dispose(){
            foreach(var pose in sprites.Values){if(pose){offsets.Remove(pose);destroy(pose);}}
            foreach(var texture in textures.Values)if(texture)destroy(texture);
            sprites.Clear();textures.Clear();
        }
    }
    public sealed class DeferredCannonSkinV39 : NativePoseSpriteBankV38 {
        public DeferredCannonSkinV39(Action<Object> destroy,Dictionary<Sprite,Vector2> offsets)
            :base("cannon-big-down","NUI_CANNON_BIG_DOWN",destroy,offsets,data=>
                data.artVersion=="registered-cannon-deferred-uv-v39"&&data.clips.Length==3&&data.pages.Length==4&&
                data.sourceMotionCount==3&&data.paintedPoseCount==170&&data.nativeYaw==0&&data.authoredFacing==1&&
                Mathf.Abs(data.standingHeight-133.65313f)<.005f&&data.logicalScale==1&&
                data.sourceMotionSha=="918d8acc08b4f4f061670f59f455f64dde2d7f5a7a428b6f6d6197723e73d42d") {}
    }
    public sealed class MagicianSkinBankV38 : NativePoseSpriteBankV38 {
        public MagicianSkinBankV38(string kind,string npc,Action<Object> destroy,Dictionary<Sprite,Vector2> offsets)
            :base(kind,npc,destroy,offsets,data=>MatchesSource(kind,data)) {}
        static bool MatchesSource(string kind,MageLibraryV38 data){
            bool forest=kind=="forest-dark-magician";
            string motion=forest?"0b680bf8ad4c85cb12b11531dfd63aefae173cc73ee15daaddb23f5d79652018":"52508113afdfbac4d1cdf3cfa892e710d402a5598611109c4a3d6a8c21e4fa00";
            return (forest||kind=="ghost-magician-ragged")&&data.artVersion=="original-mage-uv-v38"&&
                data.nativeYaw==0&&data.authoredFacing==1&&data.sourceMotionCount==20&&data.sourceMotionSha==motion&&
                data.clips!=null&&data.clips.Length==(forest?41:42)&&data.paintedPoseCount==(forest?587:584)&&
                Mathf.Abs(data.standingHeight-61.71606f)<=.005f&&Mathf.Abs(data.logicalScale-1)<=.0001f;
        }
    }

}
