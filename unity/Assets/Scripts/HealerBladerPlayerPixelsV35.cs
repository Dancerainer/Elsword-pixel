#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class HealerBladerPlayerCheckV35 {
        void Post(Camera c) { if(evidenceDraw&&scene&&c==Field(scene,"worldCamera") as Camera) posted=Time.frameCount; }
        internal void Draw(SpriteRenderer r,Camera c) {
            if(evidenceDraw&&scene&&c==Field(scene,"worldCamera") as Camera)draws[r]=Time.frameCount;
        }
        bool Resolve(out FairyAuthoredPartsV33 owner,out object entry){
            owner=null;entry=null;parts.Clear();
            var rows=(IDictionary)Field(scene,"healerViewsV35");
            if(!rows.Contains(target))return false;
            entry=rows[target];owner=Field(entry,"parts") as FairyAuthoredPartsV33;
            if(owner==null)return false;
            owner.VisitRenderers(parts.Add);
            return parts.Count==9;
        }
        static Texture2D ReadRT(RenderTexture rt) {
            RenderTexture.active=rt;var t=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);
            t.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0,false);t.Apply(false,false);return t;
        }
        static void SaveNew(Texture2D t,string file) {
            var b=t.EncodeToPNG();using(var stream=new FileStream(file,FileMode.CreateNew,FileAccess.Write,FileShare.Read))
                stream.Write(b,0,b.Length);
        }
        void Observe(string label) {
            Need(Resolve(out var owner,out var entry),"No parts for "+label);
            var carrier=(SpriteRenderer)Field(entry,"carrier");
            string key=(string)Field(entry,"key"),state=(string)Field(entry,"state");
            float clock=(float)Field(entry,"motionClock");var frame=owner.LastFrame;
            Need(carrier&&carrier.enabled&&carrier.forceRenderingOff&&carrier.sprite,"Original carrier and owned rig "+label);
            var originals=(IDictionary)Field(scene,Core?"coreHealerWholeSpritesV21":"factoryProductSpritesR1179");
            string ownedKey=Core?key:HealerBladerRigV35.Id+"/"+key;
            Need(originals.Contains(ownedKey)&&ReferenceEquals(originals[ownedKey],carrier.sprite),"Selected original Sprite instance "+ownedKey+" "+label);
            Need(!string.IsNullOrEmpty(state)&&frame.visible,"Original state and visible rig "+label);
            Need(Mathf.Abs(frame.sourceClock-clock)<1e-5f,"Original source motion clock "+clock+"/"+frame.sourceClock+" "+label);
            Need(frame.facing==target.facing,"Original actor facing "+target.facing+"/"+frame.facing+" "+label);
            var cam=(Camera)Field(scene,"worldCamera");
            if(!rt){rt=new RenderTexture(Width,Height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"GPU target");}
            var shot=new Shot{label=label,frame=Time.frameCount,state=state,motion=frame.alias,key=key,
                facing=frame.facing,npcTime=target.npcTime,sourceClock=frame.sourceClock,renderers=parts.Count,hp=target.hp,alpha=carrier.color.a};
            var oldTarget=cam.targetTexture;var oldActive=RenderTexture.active;var oldRect=cam.rect;float aspect=cam.aspect;
            string before=JsonUtility.ToJson(target);float time=game.elapsed;
            Texture2D on=null,off=null;var enabled=new bool[parts.Count];var bounds=parts[0].bounds;
            for(int i=0;i<parts.Count;i++){
                var view=parts[i];enabled[i]=view.enabled;
                Need(view.enabled&&view.gameObject.activeInHierarchy&&view.sprite&&!view.forceRenderingOff&&
                    Mathf.Abs(view.color.a-carrier.color.a)<.001f,"Visible alpha-matched part "+i);
                bounds.Encapsulate(view.bounds);var w=view.GetComponent<HealerBladerWitnessV35>();
                if(!w)w=view.gameObject.AddComponent<HealerBladerWitnessV35>();w.Bind(this,view);
            }
            Need(parts[5].sprite==parts[7].sprite&&parts[6].sprite==parts[8].sprite,"Four original blade branches reuse two drawings");
            string stem=report.shots.Count.ToString("D3")+"-"+label;
            try{
                cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);cam.aspect=(float)Width/Height;
                draws.Clear();posted=-1;evidenceDraw=true;RenderTexture.active=rt;GL.Clear(true,true,Color.clear);cam.Render();evidenceDraw=false;
                on=ReadRT(rt);foreach(var v in parts)if(draws.TryGetValue(v,out int f)&&f==Time.frameCount)shot.rendered++;
                Need(posted==Time.frameCount&&shot.rendered>0,"Actual original camera and rendered parts "+label);
                foreach(var v in parts)v.enabled=false;
                RenderTexture.active=rt;GL.Clear(true,true,Color.clear);cam.Render();off=ReadRT(rt);
                var first=on.GetPixels32();var last=off.GetPixels32();
                var lo=cam.WorldToViewportPoint(bounds.min);var hi=cam.WorldToViewportPoint(bounds.max);
                int x0=Mathf.Clamp(Mathf.FloorToInt(lo.x*Width)-2,0,Width),x1=Mathf.Clamp(Mathf.CeilToInt(hi.x*Width)+2,0,Width);
                int y0=Mathf.Clamp(Mathf.FloorToInt(lo.y*Height)-2,0,Height),y1=Mathf.Clamp(Mathf.CeilToInt(hi.y*Height)+2,0,Height);
                for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++){int i=y*Width+x;var p=first[i];var q=last[i];
                    if(Math.Abs(p.r-q.r)+Math.Abs(p.g-q.g)+Math.Abs(p.b-q.b)>6)shot.pixels++;}
                shot.onFile=stem+"-world-on.png";shot.offFile=stem+"-world-off.png";
                SaveNew(on,Path.Combine(output,shot.onFile));SaveNew(off,Path.Combine(output,shot.offFile));report.shots.Add(shot);
                Need(shot.pixels>0,"No actual GPU pixels from actor "+label);
            } finally {
                for(int i=0;i<parts.Count;i++)if(parts[i])parts[i].enabled=enabled[i];
                cam.targetTexture=oldTarget;cam.rect=oldRect;cam.aspect=aspect;RenderTexture.active=oldActive;evidenceDraw=false;
                if(on)Destroy(on);if(off)Destroy(off);
            }
            Need(game.elapsed==time&&JsonUtility.ToJson(target)==before,"Pixel observation cannot step or mutate actor");
        }
    }
    public sealed class HealerBladerWitnessV35:MonoBehaviour {
        HealerBladerPlayerCheckV35 owner;SpriteRenderer part;
        internal void Bind(HealerBladerPlayerCheckV35 o,SpriteRenderer p){owner=o;part=p;}
        void OnWillRenderObject(){if(owner&&part)owner.Draw(part,Camera.current);}
    }
}
#endif
