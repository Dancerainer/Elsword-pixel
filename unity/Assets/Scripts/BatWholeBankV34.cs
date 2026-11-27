using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed class BatWholeBankV34 {
        [Serializable] public sealed class Sheet {
            public string resource,decodedSha;
            public int w,h;
            [NonSerialized] public Texture2D atlas;
        }
        [Serializable] public sealed class Drawing {
            public int sheet,x,y,w,h,px,py,anchorMode;
            public float units,artScaleX,artScaleY;
            public int[] hull;
        }
        [Serializable] public sealed class Span {
            public string alias;
            public float begin,end;
            public int drawing; // -1 uses the three controls at this part of the source clock.
        }
        [Serializable] sealed class Document {
            public string version;
            public Sheet[] sheets;
            public Drawing[] drawings;
            public Span[] spans;
        }
        readonly Document doc;
        public BatWholeBankV34(TextAsset text,string digest) {
            if (!text || BatAuthoredBankV34.Sha(text.bytes)!=digest) throw new InvalidOperationException("Bat whole-pose registration SHA differs");
            doc=JsonUtility.FromJson<Document>(text.text);
            if (doc==null || doc.version!="bat-a-whole-v34-r1" || doc.sheets==null || doc.drawings==null || doc.spans==null)
                throw new InvalidOperationException("Missing Bat whole-pose bank");
            foreach (var sheet in doc.sheets) {
                if (!sheet.resource.StartsWith(BatAuthoredBankV34.Resource,StringComparison.Ordinal)) throw new InvalidOperationException("Invalid Bat sheet path");
                sheet.atlas=Resources.Load<Texture2D>(sheet.resource);
                BatAuthoredBankV34.CheckAtlas(sheet.atlas,sheet.w,sheet.h,sheet.decodedSha);
            }
            foreach (var d in doc.drawings) {
                if (d.sheet<0 || d.sheet>=doc.sheets.Length || d.units<=0 || d.artScaleX<=0 || d.artScaleY<=0 || d.hull==null || d.hull.Length<6 || d.hull.Length%2!=0
                    || d.x<0 || d.y<0 || d.w<=0 || d.h<=0 || d.x+d.w>doc.sheets[d.sheet].w || d.y+d.h>doc.sheets[d.sheet].h)
                    throw new InvalidOperationException("Invalid Bat whole pose bounds");
            }
            foreach (var span in doc.spans) {
                if (span.drawing < -1 || span.drawing>=doc.drawings.Length || span.begin<0 || span.end<=span.begin)
                    throw new InvalidOperationException("Invalid Bat whole pose interval");
            }
        }
        public Sheet Page(Drawing drawing) { return doc.sheets[drawing.sheet]; }
        public bool Select(string alias,float time,out Drawing d) {
            d=null;
            foreach (var span in doc.spans) {
                if (alias!=span.alias || time<span.begin || time>=span.end) continue;
                if (span.drawing>=0) d=doc.drawings[span.drawing];
                return true;
            }
            return false;
        }
        public static float PhaseTime(BatAuthoredBankV34.Motion m,float clock) {
            return m.loop ? Mathf.Repeat(clock,m.duration) : Mathf.Clamp(clock,0,m.duration);
        }
        public static Vector3 SourcePlacement(BatAuthoredBankV34.Motion m,float t,int anchorMode,out float stretch) {
            var keys=m.samples;
            int i=0;
            while (i+1<keys.Length-1 && keys[i+1].time<t) i++;
            var a=keys[i]; var b=keys[i+1];
            float q=Mathf.InverseLerp(a.time,b.time,t);
            stretch=m.alias=="source-start" ? Mathf.Lerp(a.h,b.h,q)/36f : 1f;
            if (anchorMode==0) return new Vector3(0,Mathf.Lerp(a.ay,b.ay,q),0);
            bool opening=m.alias=="source-wait-zero2";
            float floor=opening?0:18.480877598f;
            float x=Mathf.Lerp(a.w*.5f-a.ax+a.bipX,b.w*.5f-b.ax+b.bipX,q);
            float y=Mathf.Lerp(a.ay-a.h+a.bipY,b.ay-b.h+b.bipY,q)-floor;
            return new Vector3(x,y,0);
        }
    }
}
