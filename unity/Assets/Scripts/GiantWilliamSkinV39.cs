using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    [Serializable] public sealed class GiantWilliamPageV39 {public string resource,sha256;public int w,h,bytes;}
    [Serializable] public sealed class GiantWilliamMotionV39 {public string id,sourceAnimation;public FrameData[] frames;public float fps,duration;public bool loop;}
    [Serializable] public sealed class GiantWilliamBindingV39 {
        public string alias,motion,paintClip;public bool paint,loop;public int[] indices,paintIndices;
        public int count;public float fps,duration;
    }
    [Serializable] public sealed class GiantWilliamPaintBoundsV39 {public string texture;public int x,y,w,h,l,t,r,b;}
    [Serializable] public sealed class GiantWilliamLibraryV39 {
        public string id,npc,sourceMotionSha,artVersion;public int sourceMotionCount,paintedPoseCount,nativeYaw,authoredFacing;
        public string[] stages;public float standingHeight,logicalScale,originalPaintScaleX,originalPaintScaleY;
        public GiantWilliamPageV39[] pages;public GiantWilliamMotionV39[] clips;public GiantWilliamBindingV39[] bindings;
        public GiantWilliamPaintBoundsV39[] paintedBounds;
    }
    public struct GiantWilliamSelectionV39 {
        public Sprite sprite;public string alias,motion,page,paintClip;public bool painted;
        public int index,rawIndex,paintIndex;public float scaleX,scaleY,top;
    }
    public sealed class GiantWilliamBankV39:IDisposable {
        public readonly GiantWilliamLibraryV39 Data;
        readonly Dictionary<string,GiantWilliamPageV39> pages=new Dictionary<string,GiantWilliamPageV39>();
        readonly Dictionary<string,GiantWilliamMotionV39> motions=new Dictionary<string,GiantWilliamMotionV39>();
        readonly Dictionary<string,GiantWilliamBindingV39> bindings=new Dictionary<string,GiantWilliamBindingV39>();
        readonly Dictionary<string,GiantWilliamPaintBoundsV39> bounds=new Dictionary<string,GiantWilliamPaintBoundsV39>();
        readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
        readonly Dictionary<string,Sprite> made=new Dictionary<string,Sprite>();
        readonly Dictionary<Sprite,Vector2> offsets;
        readonly Action<Object> destroy;
        public static bool Actor(string kind,string npc){
            switch(kind){
                case "white-giant":return npc=="NUI_PPORU_GIANT_WHITE";
                case "pporu-giant-red":return npc=="NUI_PPORU_GIANT_RED";
                case "william":return npc=="NUI_PPORU_WILLIAM_NORMAL";
                case "pporu-william-b":return npc=="NUI_PPORU_WILLIAM_B";
                case "pporu-william-c":return npc=="NUI_PPORU_WILLIAM_C";
                default:return false;
            }
        }
        static string BoxKey(string texture,int x,int y,int w,int h){return texture+"@"+x+","+y+","+w+","+h;}
        public GiantWilliamBankV39(string kind,string npc,Dictionary<Sprite,Vector2> spriteOffsets,Action<Object> destroyOwned){
            offsets=spriteOffsets;destroy=destroyOwned;string prefix="Pixel/feedback-v39/"+kind+"/";
            var document=Resources.Load<TextAsset>(prefix+"library");
            if(!document)throw new InvalidOperationException("Missing giant / William library "+kind);
            Data=JsonUtility.FromJson<GiantWilliamLibraryV39>(document.text);
            int actions=kind=="white-giant"?29:kind=="pporu-giant-red"?32:kind=="william"?35:37;
            float height=kind.Contains("giant")?145.60f:92.27f;
            if(!Actor(kind,npc)||Data==null||Data.id!=kind||Data.npc!=npc||
                Data.artVersion!="original-giants-william-v39"||Data.nativeYaw!=0||Data.authoredFacing!=1||
                Data.sourceMotionCount!=actions||Data.clips==null||Data.clips.Length!=actions||
                Data.sourceMotionSha==null||Data.sourceMotionSha.Length!=64||Mathf.Abs(Data.standingHeight-height)>.03f||
                Mathf.Abs(Data.logicalScale-1)>.001f||Data.stages==null||Data.stages.Length==0||
                Data.originalPaintScaleX<=0||Data.originalPaintScaleY<=0)throw new InvalidOperationException("Giant / William rig registration "+kind);
            foreach(var p in Data.pages){
                if(!p.resource.StartsWith(prefix,StringComparison.Ordinal)||p.w<=0||p.h<=0||p.w>2048||p.h>2048||
                   p.sha256==null||p.sha256.Length!=64||p.bytes<=0)throw new InvalidOperationException("Giant / William atlas "+kind);
                pages.Add(p.resource,p);
            }
            foreach(var c in Data.clips){
                if(String.IsNullOrEmpty(c.sourceAnimation)||c.frames==null||c.frames.Length==0||c.fps!=24)
                    throw new InvalidOperationException("Giant / William KSM clock "+kind);
                motions.Add(c.sourceAnimation,c);
            }
            foreach(var b in Data.bindings){
                if(!motions.ContainsKey(b.motion)||String.IsNullOrEmpty(b.alias)||b.indices==null||b.paintIndices==null||
                   b.paint&&(String.IsNullOrEmpty(b.paintClip)||b.paintIndices.Length==0))throw new InvalidOperationException("Giant / William binding");
                bindings.Add(b.alias,b);
            }
            foreach(var b in Data.paintedBounds)bounds.Add(BoxKey(b.texture,b.x,b.y,b.w,b.h),b);
        }
        public GiantWilliamBindingV39 Binding(string alias){return alias!=null&&bindings.TryGetValue(alias,out var b)?b:null;}
        public bool Stage(string room){return Array.IndexOf(Data.stages,room)>=0;}
        public Sprite Introduction(){
            var f=motions["Wait"].frames[0];var page=pages[f.texture];var tex=LoadPage(page);
            return Make("intro",tex,new Rect(f.x,tex.height-f.y-f.h,f.w,f.h),Data.id+"/v39/intro",Vector2.zero);
        }
        Texture2D LoadPage(GiantWilliamPageV39 page){
            if(textures.TryGetValue(page.resource,out var texture))return texture;
            var a=Resources.Load<TextAsset>(page.resource);
            if(!a)throw new InvalidOperationException("Missing encoded giant / William page "+page.resource);
            byte[] bytes=a.bytes;
            if(bytes.Length!=page.bytes)throw new InvalidOperationException("Invalid PNG byte length "+page.resource);
            using(var sha=SHA256.Create())if(!String.Equals(BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-",""),page.sha256,StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Giant / William PNG pin "+page.resource);
            texture=new Texture2D(2,2,TextureFormat.RGBA32,false){name=page.resource};
            try{
                if(!ImageConversion.LoadImage(texture,bytes,true)||texture.width!=page.w||texture.height!=page.h||texture.mipmapCount!=1)
                    throw new InvalidOperationException("Giant / William page decode "+page.resource);
                texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;textures.Add(page.resource,texture);return texture;
            }catch{destroy(texture);throw;}
        }
        Sprite Make(string key,Texture2D texture,Rect rect,string name,Vector2 offset){
            if(made.TryGetValue(key,out var existing))return existing;
            var sprite=Sprite.Create(texture,rect,new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
            sprite.name=name;made.Add(key,sprite);offsets[sprite]=offset;return sprite;
        }
        public bool Select(GiantWilliamBindingV39 b,AnimationData clockClip,Sprite[] originals,Sprite carrier,float clock,
                           AnimationData painted,Sprite[] existing,out GiantWilliamSelectionV39 result){
            result=default;
            if(b==null||clockClip==null||originals==null||originals.Length==0||float.IsNaN(clock)||float.IsInfinity(clock))return false;
            if(b.count>0&&(b.count!=originals.Length||b.fps!=clockClip.fps||b.loop!=clockClip.loop||Mathf.Abs(b.duration-clockClip.duration)>.00001f))return false;
            float t=clockClip.loop&&clockClip.duration>0?Mathf.Repeat(clock,clockClip.duration):clock;
            int i=Mathf.Clamp(Mathf.FloorToInt(t*clockClip.fps),0,originals.Length-1);
            if(!ReferenceEquals(originals[i],carrier))return false;
            return SelectFrame(b,t,i,painted,existing,out result);
        }
        public bool SelectOriginalHitV45(GiantWilliamBindingV39 b,float clock,AnimationData painted,Sprite[] existing,out GiantWilliamSelectionV39 result){
            int i=b.count>0?Mathf.Clamp(Mathf.FloorToInt(clock*b.fps),0,b.count-1):0;
            return SelectFrame(b,clock,i,painted,existing,out result);
        }
        bool SelectFrame(GiantWilliamBindingV39 b,float t,int i,AnimationData painted,Sprite[] existing,out GiantWilliamSelectionV39 result){
            result=default;
            var motion=motions[b.motion];int j=b.indices.Length>0?b.indices[i]:Mathf.Clamp(Mathf.FloorToInt(t*24),0,motion.frames.Length-1);
            if(j<0||j>=motion.frames.Length)throw new InvalidOperationException("Giant / William native index "+b.alias);
            result=new GiantWilliamSelectionV39{alias=b.alias,motion=b.motion,index=i,rawIndex=j,painted=b.paint,paintClip=b.paintClip,paintIndex=-1};
            string id=Data.id+"/"+b.alias+"/"+i;
            if(b.paint){
                if(painted==null||existing==null||painted.frames.Length!=existing.Length)return false;
                int p=b.paintIndices[b.count>0?i:j];if(p<0||p>=existing.Length||!existing[p])throw new InvalidOperationException("Existing painted pose "+id);
                var f=painted.frames[p];var original=existing[p];
                var visible=bounds[BoxKey(f.texture,f.x,f.y,f.w,f.h)];
                result.sprite=Make(b.alias+"@"+p,original.texture,original.rect,id,new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f));
                result.paintIndex=p;result.page=f.texture;result.scaleX=Data.originalPaintScaleX;result.scaleY=Data.originalPaintScaleY;
                result.top=f.h*.5f-visible.t;return true;
            }
            var frame=motion.frames[j];
            if(frame==null||!frame.hasAnchor||!pages.TryGetValue(frame.texture,out var page)||frame.w<=0||frame.h<=0||
               frame.x<0||frame.y<0||frame.x+frame.w>page.w||frame.y+frame.h>page.h)throw new InvalidOperationException("Giant / William original crop "+id);
            var tex=LoadPage(page);
            result.sprite=Make(b.alias+"@"+j,tex,new Rect(frame.x,tex.height-frame.y-frame.h,frame.w,frame.h),id,
                               new Vector2(frame.w*.5f-frame.anchorX,frame.anchorY-frame.h*.5f));
            result.page=frame.texture;result.scaleX=Data.logicalScale;result.scaleY=Data.logicalScale;result.top=frame.h*.5f;return true;
        }
        public void Dispose(){
            foreach(var sprite in made.Values)if(sprite){offsets.Remove(sprite);destroy(sprite);}
            foreach(var texture in textures.Values)if(texture)destroy(texture);
            made.Clear();textures.Clear();
        }
    }
}
