using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class InkPoseV45 {public string name,sha256;public float time,anchorX,anchorY;public int width,height,alphaX,alphaY,alphaW,alphaH;}
        [Serializable] sealed class InkWindowV45 {public string motion,signed;public float start,end;public InkPoseV45[] frames;}
        [Serializable] sealed class InkBankV45 {public string id;public InkWindowV45[] windows;}
        sealed class InkEntryV45 {public Sprite sprite;public Texture2D texture;public int used;}
        sealed class InkLoadedV45 {
            public InkBankV45 bank;
            public int peakDecoded;
            public double maxDecodeMs;
            public readonly Dictionary<string,InkEntryV45> entries=new Dictionary<string,InkEntryV45>(StringComparer.Ordinal);
        }
        readonly Dictionary<string,InkLoadedV45> coreInkV45=new Dictionary<string,InkLoadedV45>(StringComparer.Ordinal);
        readonly Dictionary<Sprite,Rect> coreInkAlpha64V45=new Dictionary<Sprite,Rect>();
        int lastUseV45;
        internal string LastCoreSignedMotionV45 {get;private set;}

        bool SelectCoreInkV45(string kind,string motion,float time,string signed,SpriteRenderer view,out Vector2 offset){
            offset=Vector2.zero;LastCoreSignedMotionV45=null;
            if(kind=="nasod-element-fire-b"&&motion!="AttackB_Fire"||
               kind=="nasod-king"&&motion!="Attack1_Left"&&motion!="Attack1_Right"&&motion!="Attack3_Left"&&motion!="Attack3_Right"&&motion!="SpellAttack")return false;
            string prefix="Pixel/core-motion-v45/"+kind+"/";
            if(!coreInkV45.TryGetValue(kind,out InkLoadedV45 set)){
                var info=Resources.Load<TextAsset>(prefix+"library");if(!info)throw new InvalidOperationException("Missing Core motion "+kind);
                set=new InkLoadedV45{bank=JsonUtility.FromJson<InkBankV45>(info.text)};Resources.UnloadAsset(info);
                if(set.bank.id!=kind||set.bank.windows==null)throw new InvalidOperationException("Invalid Core motion library "+kind);
                coreInkV45.Add(kind,set);
            }
            var f=CorePoseAtV45(set,motion,time,signed);
            if(f==null)return false;
            if(!set.entries.TryGetValue(f.name,out InkEntryV45 entry)){
                if(f.alphaW<=0||f.alphaH<=0||f.alphaX<0||f.alphaY<0||f.alphaX+f.alphaW>f.width||f.alphaY+f.alphaH>f.height)
                    throw new InvalidOperationException("Missing registered Core alpha64 footprint for "+f.name);
                long started=System.Diagnostics.Stopwatch.GetTimestamp();
                var file=Resources.Load<TextAsset>(prefix+f.name);if(!file)throw new InvalidOperationException("Missing Core registered frame "+f.name);
                var tex=new Texture2D(2,2,TextureFormat.RGBA32,false){name=kind+"/"+f.name,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
                try{
                    if(!ImageConversion.LoadImage(tex,file.bytes,true)||tex.width!=f.width||tex.height!=f.height)throw new InvalidOperationException("Invalid Core registered PNG "+f.name);
                }catch{DestroyNativeWorldObject(tex);throw;}finally{Resources.UnloadAsset(file);}
                var sprite=Sprite.Create(tex,new Rect(0,0,f.width,f.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                RegisterTightMotionBoundsV45(sprite);sprite.name="original-motion-v45/"+kind+"/"+f.name;
                coreInkAlpha64V45.Add(sprite,new Rect(f.alphaX,f.alphaY,f.alphaW,f.alphaH));
                entry=new InkEntryV45{sprite=sprite,texture=tex};set.entries.Add(f.name,entry);
                spriteOffsets[sprite]=new Vector2(f.width*.5f-f.anchorX,f.anchorY-f.height*.5f);
                double ms=(System.Diagnostics.Stopwatch.GetTimestamp()-started)*1000.0/System.Diagnostics.Stopwatch.Frequency;
                set.maxDecodeMs=Math.Max(set.maxDecodeMs,ms);
            }
            entry.used=++lastUseV45;view.sprite=entry.sprite;offset=spriteOffsets[entry.sprite];LastCoreSignedMotionV45=f.name;
            // A King texture is large. Keep only recently used decoded images; never
            // retire a frame an actual renderer still holds (two Fire Elements can coexist).
            if(set.entries.Count>4){
                string oldest=null;int tick=int.MaxValue;
                foreach(var p in set.entries)if(p.Value.used<tick&&!enemyViews.Exists(v=>v&&v.sprite==p.Value.sprite)){
                    oldest=p.Key;tick=p.Value.used;
                }
                if(oldest!=null){ReleaseInkV45(set.entries[oldest]);set.entries.Remove(oldest);}
            }
            set.peakDecoded=Math.Max(set.peakDecoded,set.entries.Count);
            return true;
        }

        static InkPoseV45 CorePoseAtV45(InkLoadedV45 set,string motion,float time,string signed){
            foreach(var w in set.bank.windows){
                if(w.motion!=motion||w.signed!=signed||time<w.start||time>=w.end)continue;
                int first=0,last=w.frames.Length;
                while(first<last){int mid=(first+last)/2;if(w.frames[mid].time<=time)first=mid+1;else last=mid;}
                return first>0?w.frames[first-1]:null;
            }
            return null;
        }

        // Resolve the ordinary source lookup again for the director's actual frame witness.
        // States without a supplemental source window (including START) require their exact original sprite.
        bool MatchesCoreWholeInkV45(string kind,string motion,float time,string signed,Sprite original,Sprite actual,out InkPoseV45 f){
            f=null;
            if(!coreInkV45.TryGetValue(kind,out var set)||(f=CorePoseAtV45(set,motion,time,signed))==null)return actual==original;
            return set.entries.TryGetValue(f.name,out var entry)&&actual==entry.sprite;
        }
        void ReleaseInkV45(InkEntryV45 entry){
            coreInkAlpha64V45.Remove(entry.sprite);nativeUiVisibleLocalRectsV28.Remove(entry.sprite);spriteOffsets.Remove(entry.sprite);DestroyNativeWorldObject(entry.sprite);DestroyNativeWorldObject(entry.texture);
        }
        void ClearCoreInkV45(string kind){
            if(!coreInkV45.TryGetValue(kind,out InkLoadedV45 set))return;
            foreach(var p in set.entries.Values)ReleaseInkV45(p);coreInkV45.Remove(kind);LastCoreSignedMotionV45=null;
        }
    }
}
