using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed class BatAuthoredBankV34 {
        public const string Resource = "Pixel/feedback-v34/bat-a/";
        public const string BankSha = "a99f265c307f67b728e7c79e9a73fe663a915d8e3c253f5edd28809a5a9d3558";
        public const string AtlasFileSha = "ed7a5b7ab620dfd3866d0ca6784e068705bf7a68b9eccdf7695ea28ef7634558";
        public const float TargetHeight = 76.57548485f;
        [Serializable] public sealed class Part {
            public string name;
            public int x,y,w,h,px,py;
            public int[] hull;
        }
        [Serializable] public struct Key {
            public float time,stroke,x,y,tilt;
            public int sx,sy,w,h,ax,ay;
            public float bipX,bipY;
        }
        [Serializable] public sealed class Motion {
            public string alias,texture;
            public float duration,fps;
            public bool loop;
            public Key[] samples;
        }
        [Serializable] sealed class Document {
            public string version,atlasSha,decodedSha;
            public float nativeHeight,restArtHeight;
            public Part[] parts;
            public Motion[] motions;
        }
        static readonly string[] Aliases = {
            "source-start","source-wait-zero2","source-flight","source-walk","source-attack-a-ready",
            "source-hurt","source-launch","source-fall","source-down","source-get-up","source-get-up-back",
            "source-death","source-attack-a","source-jump"
        };
        static readonly string[] Parts = {"body","near/up","near/arched","near/edge","far/up","far/arched","far/edge"};
        public readonly Texture2D atlas;
        public readonly Part[] pieces;
        public readonly float unitsPerPixel;
        public readonly BatWholeBankV34 whole;
        readonly Dictionary<string,Motion> motions = new Dictionary<string,Motion>();
        public static bool Finite(float f) { return !float.IsInfinity(f) && !float.IsNaN(f); }
        public static string Sha(byte[] bytes) {
            using (var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
        }
        public static BatAuthoredBankV34 Load() {
            return new BatAuthoredBankV34(Resources.Load<Texture2D>(Resource+"parts"),Resources.Load<TextAsset>(Resource+"bank"));
        }
        public BatAuthoredBankV34(Texture2D texture,TextAsset text) {
            if (!text || Sha(text.bytes)!=BankSha) throw new InvalidOperationException("Bat source clock/registration bank changed");
            var doc=JsonUtility.FromJson<Document>(text.text);
            if (doc==null || doc.version!="bat-a-v34-r1" || doc.atlasSha!=AtlasFileSha || doc.parts.Length!=7
                || doc.motions.Length!=14 || Math.Abs(doc.nativeHeight-TargetHeight)>.0001f || Math.Abs(doc.restArtHeight-579.74f)>.0002f)
                throw new InvalidOperationException("Bat registration contract changed");
            CheckAtlas(texture,1254,1254,doc.decodedSha);
            atlas=texture; pieces=doc.parts; unitsPerPixel=doc.nativeHeight/doc.restArtHeight;
            for (int i=0;i<pieces.Length;i++) {
                var p=pieces[i];
                if (p==null || p.name!=Parts[i] || p.hull==null || p.hull.Length<6 || p.hull.Length%2!=0)
                    throw new InvalidOperationException("Bat part registration missing");
            }
            foreach (var m in doc.motions) {
                if (Array.IndexOf(Aliases,m.alias)<0 || m.samples==null || m.samples.Length<2 || m.samples[0].time!=0
                    || Math.Abs(m.samples[m.samples.Length-1].time-m.duration)>.00002f) throw new InvalidOperationException("Bat source endpoint changed");
                motions.Add(m.alias,m);
            }
            whole=new BatWholeBankV34(Resources.Load<TextAsset>(Resource+"whole-bank"),BatWholePinV34.Sha);
        }
        internal static void CheckAtlas(Texture2D texture,int width,int height,string digest) {
            if (!texture || texture.width!=width || texture.height!=height || !texture.isReadable || !texture.isDataSRGB
                || texture.mipmapCount!=1 || texture.filterMode!=FilterMode.Point || texture.wrapMode!=TextureWrapMode.Clamp
                || (texture.format!=TextureFormat.RGBA32 && texture.format!=TextureFormat.ARGB32))
                throw new InvalidOperationException("Bat atlas must preserve RGBA Point/Clamp/sRGB pixels");
            var colors=texture.GetPixels32();
            var decoded=new byte[colors.Length*4];
            for (int i=0;i<colors.Length;i++) {
                decoded[4*i]=colors[i].r; decoded[4*i+1]=colors[i].g;
                decoded[4*i+2]=colors[i].b; decoded[4*i+3]=colors[i].a;
            }
            if (Sha(decoded)!=digest) throw new InvalidOperationException("Bat imported pixels changed: "+texture.name);
        }
        public bool CanDraw(Motion m,float clock) {
            if (m==null || !Finite(clock) || clock<0) return false;
            BatWholeBankV34.Drawing drawing;
            return whole.Select(m.alias,BatWholeBankV34.PhaseTime(m,clock),out drawing) || HasAuthoredKeys(m.alias);
        }
        public Motion Find(string alias) {
            Motion motion;
            return alias!=null && motions.TryGetValue(alias,out motion) ? motion : null;
        }
        // The source carrier owns aliases whose pose needs an additional complete drawing.
        public static bool HasAuthoredKeys(string alias) {
            switch (alias) {
                case "source-flight": case "source-walk": case "source-attack-a-ready": case "source-hurt":
                case "source-launch": case "source-fall": case "source-jump": return true;
                default: return false;
            }
        }
        public static Key Sample(Motion m,float clock) {
            if (m==null || !Finite(clock) || clock<0) throw new ArgumentException("Invalid Bat source clock");
            float t=m.loop ? Mathf.Repeat(clock,m.duration) : Mathf.Min(clock,m.duration);
            int lo=0,hi=m.samples.Length-1;
            while (hi-lo>1) { int mid=(lo+hi)/2; if (m.samples[mid].time<=t) lo=mid; else hi=mid; }
            var a=m.samples[lo]; var b=m.samples[hi];
            float q=Mathf.InverseLerp(a.time,b.time,t);
            var k=new Key {time=t,stroke=Mathf.Lerp(a.stroke,b.stroke,q),x=Mathf.Lerp(a.x,b.x,q),
                y=Mathf.Lerp(a.y,b.y,q),tilt=Mathf.LerpAngle(a.tilt,b.tilt,q)};
            // Projected color centroids move as one wing hides the body. Their change
            // guides cosmetic lean, while the model and registered Bip own translation.
            k.x=Mathf.Clamp(k.x*.4f,-4f,4f);
            k.y=Mathf.Clamp(k.y*.4f,-5f,5f);
            k.tilt=Mathf.Clamp(k.tilt*.35f,-24f,24f);
            // The three source airborne banks hold a single projection. A small cosmetic
            // wing flex follows their real source clock, with exact endpoints at each end.
            if (m.alias=="source-jump" || m.alias=="source-launch" || m.alias=="source-fall")
                k.stroke=Mathf.Clamp01(k.stroke+(k.stroke>.5f?-.045f:.045f)*Mathf.Sin(Mathf.PI*t/m.duration));
            return k;
        }
    }
}
