using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] internal sealed class ReusablePaintKeyV37 {
        public string texture;
        public int[] rect,bbox;
        public float[] pivot,feet,shoeCenters,hullLeft,hullRight;
        public float unit;
    }
    [Serializable] internal sealed class ReusablePaintClipV37 {
        public string alias;
        public float fps,duration;
        public bool loop;
        public int[] keys;
    }
    [Serializable] internal sealed class ReusablePaintAliasV37 {
        public string alias,motion,paint;
        public float span;
        public bool loop,hold;
    }
    [Serializable] internal sealed class ReusableSourceStateV37 { public string state,motion;public float speed; }
    [Serializable] internal sealed class ReusablePaintBirthV37 { public string room;public int index,source; }
    [Serializable] internal sealed class ReusablePaintTrackV37 {
        public string motion;
        public int count,fps;
        public float duration;
        public float[] values;

        internal Vector2 XY(float clock,int field){
            float at=Mathf.Clamp(clock*fps,0,count-1);int a=Mathf.FloorToInt(at),b=Math.Min(a+1,count-1);
            return new Vector2(Mathf.Lerp(values[a*22+field],values[b*22+field],at-a),
                Mathf.Lerp(values[a*22+field+1],values[b*22+field+1],at-a));
        }
    }
    [Serializable] internal sealed class ReusablePaintTrackBankV37 {
        public string kind,npc,motionSha;
        public int fields;
        public ReusablePaintTrackV37[] tracks;
    }
    [Serializable] internal sealed class ReusablePaintRigV37 {
        public string kind,npc,sourceSha,tracksSha;
        public float height;
        public ReusablePaintKeyV37[] keys;
        public ReusablePaintClipV37[] clips;
        public ReusablePaintAliasV37[] aliases;
        public ReusableSourceStateV37[] states;
        public ReusablePaintBirthV37[] stages;
        readonly Dictionary<string,ReusablePaintClipV37> paint=new Dictionary<string,ReusablePaintClipV37>(StringComparer.Ordinal);
        readonly Dictionary<string,ReusablePaintAliasV37> selected=new Dictionary<string,ReusablePaintAliasV37>(StringComparer.Ordinal);
        readonly Dictionary<string,ReusablePaintTrackV37> motion=new Dictionary<string,ReusablePaintTrackV37>(StringComparer.Ordinal);
        readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
        internal static void Need(bool valid,string reason){if(!valid)throw new InvalidOperationException("Reusable painted NPC V37 "+reason);}
        internal static string Digest(TextAsset file){
            using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(file.bytes)).Replace("-","").ToLowerInvariant();
        }
        internal static ReusablePaintRigV37 Load(string kind){
            string root="Pixel/feedback-v37/"+kind+"/";
            var file=Resources.Load<TextAsset>(root+"bindings");Need(file&&Digest(file)==ReusablePaintPinsV37.ForKind(kind),"pinned original painted exposures "+kind);
            var lib=JsonUtility.FromJson<ReusablePaintRigV37>(file.text);
            var original=Resources.Load<TextAsset>(root+"tracks");Need(original&&Digest(original)==lib.tracksSha,"pinned original KSM "+kind);
            var src=JsonUtility.FromJson<ReusablePaintTrackBankV37>(original.text);
            Need(lib.kind==kind&&src.kind==kind&&lib.npc==src.npc&&lib.sourceSha==src.motionSha&&src.fields==22,"source geometry identity");
            foreach(var t in src.tracks){Need(t.values.Length==t.count*22&&t.count>=2&&t.fps==(t.motion=="Dash"?480:60),"original control count");lib.motion.Add(t.motion,t);}
            foreach(var c in lib.clips){Need(c.duration>0&&c.fps>0&&c.keys.Length>0,"painted exposure");foreach(int k in c.keys)Need(k>=0&&k<lib.keys.Length,"painted index");lib.paint.Add(c.alias,c);}
            foreach(var a in lib.aliases){Need(lib.motion.ContainsKey(a.motion)&&lib.paint.ContainsKey(a.paint)&&a.span>0,"real selected mapping "+a.alias);lib.selected.Add(a.alias,a);}
            return lib;
        }
        internal Texture2D Texture(ReusablePaintKeyV37 key){
            if(!textures.TryGetValue(key.texture,out var atlas)){atlas=Resources.Load<Texture2D>(key.texture);Need(atlas,"genuine painted page "+key.texture);textures.Add(key.texture,atlas);}
            return atlas;
        }
        internal bool ExactBirth(Enemy e,string room,int staged,int original){
            if(e.sourceNPC!=npc||e.kind!=kind||e.visualScale!=1)return false;
            foreach(var b in stages)if(room==b.room&&staged==b.index&&original==b.source)return true;
            return false;
        }
        internal struct Frame {
            internal int key;
            internal string alias,paint;
            internal float clock,reference;
            internal ReusablePaintTrackV37 track;
            internal bool move;
        }
        internal bool Sample(string alias,float selectedClock,float realSpan,out Frame frame,Enemy enemy=null){
            frame=default;
            if(alias==null||!selected.TryGetValue(alias,out var binding)||Math.Abs(binding.span-realSpan)>.0002f||
                float.IsNaN(selectedClock)||float.IsInfinity(selectedClock)||selectedClock<0)return false;
            var tr=motion[binding.motion];var cl=paint[binding.paint];
            float t=binding.hold?0:binding.loop?Mathf.Repeat(selectedClock,binding.span):Mathf.Min(selectedClock,binding.span);
            float normalized=t/binding.span;float actualClock=normalized*tr.duration;
            // A signed whole painting supplies the ink. The original motion supplies its
            // registration, even when this stage exposed only one generic hurt carrier.
            if(OriginalReactionV45.TryPhysicalOrCommittedV45(enemy,out var hit,out float seconds)&&hit.rendered!=binding.motion&&
               motion.TryGetValue(hit.rendered,out var actual)&&actual.duration>0){
                tr=actual;actualClock=Mathf.Clamp(seconds,0,tr.duration);
                normalized=actualClock/tr.duration;
            }
            int i=Mathf.Clamp(Mathf.FloorToInt(normalized*cl.duration*cl.fps),0,cl.keys.Length-1);int key=cl.keys[i];
            int lo=i,hi=i;
            while(lo>0&&cl.keys[lo-1]==key)lo--;
            while(hi+1<cl.keys.Length&&cl.keys[hi+1]==key)hi++;
            float center=Mathf.Clamp01((lo+hi+1)/(2f*cl.fps*cl.duration))*tr.duration;
            frame=new Frame{key=key,alias=alias,paint=binding.paint,clock=actualClock,reference=center,track=tr,
                move=(tr.motion=="Dash"||tr.motion=="Walk"||tr.motion=="Wait")&&keys[key].feet.Length==4};
            return true;
        }
    }
}
