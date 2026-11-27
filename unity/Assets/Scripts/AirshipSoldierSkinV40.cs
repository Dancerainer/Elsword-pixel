using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    [Serializable] public sealed class AirshipPageV40 {public string resource,sha256;public int w,h,bytes;}
    [Serializable] public sealed class AirshipMotionV40 {public string id,sourceAnimation;public FrameData[] frames;public float fps,duration;}
    [Serializable] public sealed class AirshipBindingV40 {
        public string alias,motion,sourceState;public int[] indices;public bool useNative,loop;public float fps,duration;public int count;
    }
    [Serializable] public sealed class AirshipBoundsV40 {public int x,y,w,h,l,t,r,b;}
    [Serializable] public sealed class AirshipLibraryV40 {
        public string id,npc,sourceMotionSha,sourceAnimationDocumentSha,sourceWholeAtlasSha,artVersion,waitName;
        public string[] routes;public int sourceMotionCount,paintedPoseCount,originalKsmFrameCount,nativeYaw,authoredFacing;
        public float standingHeight,bodyHeight,paintedScale,logicalScale;public AirshipPageV40[] pages;
        public AirshipMotionV40[] clips;public AirshipBindingV40[] bindings;public AirshipBoundsV40[] paintBounds;
    }
    public struct AirshipSelectionV40 {
        public Sprite sprite;public string alias,motion,page;public bool native;public int index,rawIndex;
        public float scale,top;
    }
    public sealed class AirshipSoldierBankV40:IDisposable {
        readonly Dictionary<string,AirshipPageV40> pages=new Dictionary<string,AirshipPageV40>();
        readonly Dictionary<string,AirshipMotionV40> native=new Dictionary<string,AirshipMotionV40>();
        readonly Dictionary<string,AirshipBindingV40> bindings=new Dictionary<string,AirshipBindingV40>();
        readonly Dictionary<long,AirshipBoundsV40> bounds=new Dictionary<long,AirshipBoundsV40>();
        readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
        readonly Dictionary<string,Sprite> made=new Dictionary<string,Sprite>();
        readonly Dictionary<Sprite,Vector2> offsets;readonly Action<Object> destroy;
        public readonly AirshipLibraryV40 Data;
        public static bool Known(string kind,string npc){
            switch(kind){
                case "airship-soldier-gunner":return npc=="NUI_AIRSHIP_SOLDIER_GUNNER";
                case "airship-soldier-high":return npc=="NUI_AIRSHIP_SOLDIER_HIGH";
                case "airship-soldier-repairman":return npc=="NUI_AIRSHIP_SOLDIER_REPAIRMAN";
                case "airship-soldier-swordman":return npc=="NUI_AIRSHIP_SOLDIER_SWORDMAN";
                default:return false;
            }
        }
        static long Key(int x,int y){return ((long)y<<32)|(uint)x;}
        public AirshipSoldierBankV40(string kind,string npc,Dictionary<Sprite,Vector2> spriteOffsets,Action<Object> destroyOwned,bool beast=false){
            offsets=spriteOffsets;destroy=destroyOwned;string prefix=(beast?"Pixel/feedback-v45/":"Pixel/feedback-v40/")+kind+"/";
            var text=Resources.Load<TextAsset>(prefix+"library");if(!text)throw new InvalidOperationException("Missing Airship soldier bank "+kind);
            Data=JsonUtility.FromJson<AirshipLibraryV40>(text.text);
            var source=Array.Find(BattleProductInputBindingsR1282.Sprites,s=>s.id==kind&&s.npc==npc);
            if(beast){
                if(Data==null||Data.id!=kind||Data.npc!=npc||Data.artVersion!="registered-beast-ink-v45"||Data.nativeYaw!=0||
                   Data.authoredFacing!=1||Mathf.Abs(Data.logicalScale-1)>.001f||Data.standingHeight<1||Data.sourceMotionSha.Length!=64||
                   Data.pages==null||Data.bindings==null||Data.routes==null)throw new InvalidOperationException("Quadruped source registration "+kind);
            }else if(source==null||!Known(kind,npc)||Data==null||Data.id!=kind||Data.npc!=npc||Data.artVersion!="original-airship-soldiers-v40"||
               Data.nativeYaw!=0||Data.authoredFacing!=1||Data.sourceMotionSha==null||Data.sourceMotionSha.Length!=64||
               Data.sourceAnimationDocumentSha!=source.animations||Data.sourceWholeAtlasSha!=source.atlas||
               Data.bodyHeight<=0||Data.standingHeight<Data.bodyHeight||Data.paintedScale<=0||Mathf.Abs(Data.logicalScale-1)>.001f||
               Data.routes==null||Array.IndexOf(Data.routes,"Battle")<0)throw new InvalidOperationException("Airship source/product registration "+kind);
            foreach(var page in Data.pages){
                if(!page.resource.StartsWith(prefix,StringComparison.Ordinal)||page.w<=0||page.h<=0||page.w>2048||page.h>2048||
                   page.bytes<=0||page.sha256==null||page.sha256.Length!=64)throw new InvalidOperationException("Airship replacement page "+kind);
                pages.Add(page.resource,page);
            }
            foreach(var c in Data.clips){
                if(c.fps!=24||String.IsNullOrEmpty(c.sourceAnimation)||c.frames==null||c.frames.Length==0)throw new InvalidOperationException("Airship raw pose clock");
                native.Add(c.sourceAnimation,c);
            }
            foreach(var c in Data.bindings){
                if(c.fps<=0||c.count<=0||c.duration<=0||c.useNative&&(c.indices==null||c.indices.Length!=c.count||!native.ContainsKey(c.motion)))
                    throw new InvalidOperationException("Airship exposure table "+c.alias);
                bindings.Add(c.alias,c);
            }
            foreach(var b in Data.paintBounds)bounds.Add(Key(b.x,b.y),b);
        }
        Texture2D Page(AirshipPageV40 p){
            if(textures.TryGetValue(p.resource,out var tex))return tex;
            var encoded=Resources.Load<TextAsset>(p.resource);
            if(!encoded)throw new InvalidOperationException("Missing Airship PNG bytes "+p.resource);
            var bytes=encoded.bytes;if(bytes.Length!=p.bytes)throw new InvalidOperationException("Wrong Airship PNG length");
            using(var sha=SHA256.Create())if(!String.Equals(BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-",""),p.sha256,StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Airship PNG hash mismatch");
            tex=new Texture2D(2,2,TextureFormat.RGBA32,false){name=p.resource};
            try{
                if(!ImageConversion.LoadImage(tex,bytes,true)||tex.width!=p.w||tex.height!=p.h||tex.mipmapCount!=1)
                    throw new InvalidOperationException("Airship PNG decode "+p.resource);
                tex.filterMode=FilterMode.Point;tex.wrapMode=TextureWrapMode.Clamp;textures.Add(p.resource,tex);return tex;
            }catch{destroy(tex);throw;}
        }
        public bool Select(string alias,float clock,AnimationData carrierClip,Sprite[] originals,Sprite actual,out AirshipSelectionV40 result){
            result=default;
            if(!bindings.TryGetValue(alias,out var binding)||carrierClip==null||originals==null||
               carrierClip.frames==null||carrierClip.frames.Length!=binding.count||originals.Length!=binding.count||
               carrierClip.fps!=binding.fps||carrierClip.loop!=binding.loop||Mathf.Abs(carrierClip.duration-binding.duration)>.00001f||
               float.IsNaN(clock)||float.IsInfinity(clock))return false;
            float t=binding.loop?Mathf.Repeat(clock,binding.duration):clock;
            int i=Mathf.Clamp(Mathf.FloorToInt(t*binding.fps),0,binding.count-1);
            if(!ReferenceEquals(actual,originals[i]))return false;
            result=new AirshipSelectionV40{alias=alias,motion=binding.motion,native=binding.useNative,index=i,rawIndex=-1};
            if(!binding.useNative){
                var f=carrierClip.frames[i];var b=bounds[Key(f.x,f.y)];
                if(f.w!=b.w||f.h!=b.h)throw new InvalidOperationException("Airship original whole-pose rectangle changed");
                result.sprite=actual;result.scale=Data.paintedScale;result.top=f.h*.5f-b.t;result.page=actual.texture.name;
                return true;
            }
            var clip=native[binding.motion];int j=binding.indices[i];
            if(j<0||j>=clip.frames.Length)throw new InvalidOperationException("Airship original action time index");
            var frame=clip.frames[j];var page=pages[frame.texture];string key=binding.motion+"@"+j;
            if(!frame.hasAnchor||frame.w<=0||frame.h<=0||frame.x<0||frame.y<0||frame.x+frame.w>page.w||frame.y+frame.h>page.h)
                throw new InvalidOperationException("Airship original crop "+alias);
            if(!made.TryGetValue(key,out var sprite)){
                var texture=Page(page);sprite=Sprite.Create(texture,new Rect(frame.x,page.h-frame.y-frame.h,frame.w,frame.h),
                    new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                sprite.name=Data.id+"/"+alias+"/"+j;made.Add(key,sprite);
                offsets[sprite]=new Vector2(frame.w*.5f-frame.anchorX,frame.anchorY-frame.h*.5f);
            }
            result.sprite=sprite;result.rawIndex=j;result.scale=Data.logicalScale;result.top=frame.h*.5f;result.page=page.resource;return true;
        }
        public void Dispose(){
            foreach(var s in made.Values)if(s){offsets.Remove(s);destroy(s);}
            foreach(var t in textures.Values)if(t)destroy(t);
            made.Clear();textures.Clear();
        }
    }
}
