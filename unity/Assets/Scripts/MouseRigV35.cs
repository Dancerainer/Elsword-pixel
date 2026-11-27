using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] sealed class MouseVariantsV35 {
        public string motion;
        public int count;
        public int[] keys,flip;
        public float[] z;
    }
    [Serializable] sealed class MouseVariantDocumentV35 {
        public string schema,source;
        public int[] dimensions;
        public MouseVariantsV35[] motions;
    }
    internal sealed class MouseRigV35 {
        public const string SourceSha="4f148932342994cc5feed2620f3a5725be216896f2f3fd141466b1241032e688";
        const string Root="Pixel/feedback-v35/waterway-mouse/";
        public readonly FairyPoseLibraryV33 library;
        readonly MouseVariantDocumentV35 data;
        readonly Dictionary<string,MouseVariantsV35> byMotion=new Dictionary<string,MouseVariantsV35>(StringComparer.Ordinal);
        public static bool Identity(Enemy e){
            return e!=null&&(e.kind=="mouse"&&e.sourceNPC=="NUI_MOUSE"||e.kind=="mouse-nodrop"&&e.sourceNPC=="NUI_MOUSE_NODROP");
        }
        public static MouseRigV35 Load(){
            return LoadRegistered(Root,"NUI_MOUSE_FAMILY",SourceSha,MouseRigPinsV35.Pages,MouseRigPinsV35.Bank,MouseRigPinsV35.Height);
        }
        internal static MouseRigV35 LoadRegistered(string root,string id,string motionSha,string pagesSha,string bankSha,float height) {
            var lib=RegisteredPartRigV35.Load(root,id,pagesSha,value=>Validate(value,motionSha,height));
            try{return new MouseRigV35(lib,root,motionSha,bankSha);}
            catch{lib.ReleaseOwned(o=>{if(Application.isPlaying)UnityEngine.Object.Destroy(o);else UnityEngine.Object.DestroyImmediate(o);});throw;}
        }
        static void Validate(FairyPoseLibraryV33 lib,string motionSha,float height){
            if(lib.parts.Length!=5||lib.tracks.Length!=11||lib.motionSha256!=motionSha||lib.nativeYaw!=0||lib.authoredFacing!=1||
                Math.Abs(lib.UnitsPerPixel-1)>.00001f||Math.Abs(lib.standingPixels-height)>.00001f)throw new InvalidOperationException("Source five-mouse rig");
            for(int i=0;i<5;i++)if(lib.parts[i].name!="source-mouse-"+(i+1)||lib.parts[i].instanceOf!=(i==4?1:0))throw new InvalidOperationException("Source rigid mouse order");
            foreach(var t in lib.tracks)if(t.alias!=t.animation||t.fps!=60||t.components!=5||t.loop||t.count!=Mathf.CeilToInt(t.duration*60-.0001f)+1||t.values.Length!=t.count*25)
                throw new InvalidOperationException("Original five per-body tracks "+t.alias);
        }
        MouseRigV35(FairyPoseLibraryV33 lib,string root,string motionSha,string bankSha){
            library=lib;var t=Resources.Load<TextAsset>(root+"bank");if(!t)throw new InvalidOperationException("Missing four-drawing selection bank");
            using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(t.bytes)).Replace("-","").ToLowerInvariant()!=bankSha)throw new InvalidOperationException("Four drawing bank hash");
            data=JsonUtility.FromJson<MouseVariantDocumentV35>(t.text);
            if(data.schema!="mouse-independent-variants-v35"||data.source!=motionSha||data.dimensions.Length!=8||data.motions.Length!=11)throw new InvalidOperationException("Mouse source/variant scope");
            for(int i=0;i<8;i++)if(data.dimensions[i]<=0)throw new InvalidOperationException("Invalid real drawing extent");
            foreach(var m in data.motions){
                if(lib.Find(m.motion)==null||lib.Find(m.motion).count!=m.count||m.keys.Length!=5*m.count||m.flip.Length!=m.keys.Length||m.z.Length!=m.keys.Length)throw new InvalidOperationException("Mouse per-body selection "+m.motion);
                for(int i=0;i<m.keys.Length;i++)if(m.keys[i]<0||m.keys[i]>3||m.flip[i]<0||m.flip[i]>1||float.IsNaN(m.z[i])||float.IsInfinity(m.z[i]))throw new InvalidOperationException("Mouse display sample");
                byMotion.Add(m.motion,m);
            }
        }
        public bool TryMotion(Enemy e,string alias,float clock,float hostDuration,bool loop,out string motion,out float sourceClock){
            motion=null;sourceClock=0;
            if(!Identity(e)||float.IsNaN(clock)||float.IsInfinity(clock)||clock<0)return false;
            if(OriginalReactionV45.TryPhysicalOrCommittedV45(e,out var original,out float hitTime)&&
               library.Find(original.rendered) is FairyPoseTrackV33 exact){
                motion=exact.alias;sourceClock=Mathf.Min(hitTime,exact.duration);return true;
            }
            bool held=false;float speed=1;
            switch(alias){
                case "idle":motion="Wait";break;
                case "walk":motion="Walk";break;
                case "attack-ready":motion="AttackA_Ready";speed=e.sourceNPC=="NUI_MOUSE"?1.25f:1f;break;
                case "attack":motion="AttackA_Fire";break;
                case "source-start":motion="WaitStart";break;
                case "hurt":case "source-hurt":motion="Damage";break;
                case "launch":case "source-launch":motion="DamageAirSmall";break;
                case "down":case "source-down":case "death":case "source-death":motion="DamageDown";break;
                case "jump":case "source-jump":motion="JumpUp";break;
                case "fall":case "source-fall":motion="JumpDown";break;
                case "source-air-land":motion="JumpDownLanding";break;
                case "get-up":motion="Wait";held=true;break;
                default:return false;
            }
            var track=library.Find(motion);bool sourceLoop=alias=="idle"||alias=="walk";
            float required=held?.45f:track.duration/speed;
            if(loop!=sourceLoop||Math.Abs(hostDuration-required)>.00002f)return false;
            sourceClock=held?0:sourceLoop?Mathf.Repeat(clock*speed,track.duration):Mathf.Min(clock*speed,track.duration);
            return true;
        }
        public void ApplyVariants(string motion,float time,SpriteRenderer[] members,float[] depths,int[] selected){
            var m=byMotion[motion];int a=Mathf.Clamp(Mathf.FloorToInt(time*60),0,m.count-1),b=Math.Min(a+1,m.count-1);
            float blend=Mathf.Clamp01(time*60-a);
            for(int i=0;i<5;i++)depths[i]=Mathf.Lerp(m.z[a*5+i],m.z[b*5+i],blend);
            for(int i=0;i<5;i++){
                int ix=a*5+i,v=m.keys[ix],rank=0;var r=members[i];
                selected[i]=v;r.sprite=library.sharedSprites[v];r.flipX=false;r.flipY=m.flip[ix]!=0;
                // The track stores the source island's actual oriented width/height
                // in units of drawing zero. A new true drawing keeps those extents.
                var scale=r.transform.localScale;scale.x*=data.dimensions[0]/(float)data.dimensions[2*v];
                scale.y*=data.dimensions[1]/(float)data.dimensions[2*v+1];r.transform.localScale=scale;
                for(int j=0;j<5;j++)if(depths[j]>depths[i]||depths[j]==depths[i]&&j<i)rank++;
                r.sortingOrder=rank;
            }
        }
        public bool VisibleTop(SpriteRenderer[] members,int[] selected,out float top){
            top=float.NegativeInfinity;
            for(int i=0;i<5;i++){
                var r=members[i];if(!r||!r.enabled||!r.gameObject.activeInHierarchy||!r.sprite||r.color.a<=0)continue;
                var hull=library.parts[selected[i]].visibleHull;
                for(int n=0;n<hull.Length;n+=2){var q=r.transform.TransformPoint(new Vector3(hull[n],r.flipY?-hull[n+1]:hull[n+1],0));top=Mathf.Max(top,q.y);}
            }
            return !float.IsInfinity(top);
        }
    }
}

