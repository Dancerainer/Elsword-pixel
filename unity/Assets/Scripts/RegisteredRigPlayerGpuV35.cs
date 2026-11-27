#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ElswordPixel {
    public static class RegisteredRigPlayerGpuV35 {
        const BindingFlags Flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
        public static void Need(bool ok,string why) { if(!ok)throw new InvalidOperationException("Rig Player V35: "+why); }
        public static object Field(object o,string name) { var f=o.GetType().GetField(name,Flags);Need(f!=null,"Field "+name);return f.GetValue(o); }
        public static object Call(object o,string name,params object[] args) { var f=o.GetType().GetMethod(name,Flags);Need(f!=null,"Method "+name);return f.Invoke(o,args); }
        public static void Press(DemoScene scene,string action) {
            TouchControl control=null;
            foreach(var c in scene.GetComponentsInChildren<TouchControl>(true))
                if(c && c.isActiveAndEnabled && c.gameObject.activeInHierarchy && ReferenceEquals(c.owner,scene) && c.action==action) {
                    Need(!control,"Duplicate active control "+action);control=c;
                }
            Need(control && EventSystem.current && control.GetComponent<Image>().raycastTarget,"Original menu control "+action);
            var p=new PointerEventData(EventSystem.current){pointerId=-3535,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,control.transform.position)};
            try{Need(ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerDownHandler),"Original pointer "+action);}
            finally{if(control)ExecuteEvents.Execute(control.gameObject,p,ExecuteEvents.pointerUpHandler);}
        }
        public static IEnumerable EnterRoute(DemoScene scene,string region,string id) {
            var routes=(IList)Field(scene,"mapDungeons");int route=-1;
            for(int i=0;i<routes.Count;i++)if((string)Field(routes[i],"id")==id)route=i;
            Need(route>=0,"Route "+id);Press(scene,"region:"+region);yield return null;
            var visible=(IList)Field(scene,"mapVisible");int index=-1;
            for(int i=0;i<visible.Count;i++)if((int)visible[i]==route)index=i;
            Need(index>=0,"Visible route "+id);
            int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.NonPublic|BindingFlags.Static).GetRawConstantValue();int page=index/size,attempt=0;
            while((int)Field(scene,"mapPage")!=page){Need(++attempt<20,"Map page bound");Press(scene,"dungeon-page:"+((int)Field(scene,"mapPage")<page?"1":"-1"));yield return null;}
            Press(scene,"dungeon-card:"+(index%size));yield return null;Need((int)Field(scene,"mapSelected")==route,"Selected route "+id);
            Press(scene,"dungeon-enter");for(int i=0;i<4;i++)yield return null;
        }
        [Serializable] public sealed class PixelProof {
            public string label,on,off;public int count,width,height,x,y,renderers;
        }
        public static PixelProof Capture(Camera camera,GameModel game,Enemy actor,List<SpriteRenderer> views,string dir,string id) {
            return CaptureAtResolution(camera,game,actor,views,dir,id,1280,720);
        }
        internal static PixelProof CaptureAtResolution(Camera camera,GameModel game,Enemy actor,List<SpriteRenderer> views,string dir,string id,int w,int h,bool requireVisible=true) {
            return CaptureRenderersAtResolution(camera,game,actor,views,dir,id,w,h,requireVisible);
        }
        internal static PixelProof CaptureRenderersAtResolution<T>(Camera camera,GameModel game,Enemy actor,List<T> views,string dir,string id,int w,int h,bool requireVisible=true) where T:Renderer {
            Need(camera && game!=null && actor!=null && views.Count>0 && w>0 && h>0,"Original camera and actual renderers");var previous=camera.targetTexture;var active=RenderTexture.active;var rect=camera.rect;float aspect=camera.aspect,elapsed=game.elapsed;
            string oldActor=JsonUtility.ToJson(actor);var enabled=new bool[views.Count];
            for(int i=0;i<views.Count;i++)enabled[i]=views[i].enabled;
            RenderTexture rt=null;Texture2D on=null,off=null;
            var result=new PixelProof {label=id,on=id+"-on.png",off=id+"-off.png",renderers=views.Count};
            try {
                rt=new RenderTexture(w,h,24,RenderTextureFormat.ARGB32){filterMode=FilterMode.Point,antiAliasing=1};Need(rt.Create(),"GPU review target");
                camera.targetTexture=rt;camera.rect=new Rect(0,0,1,1);camera.aspect=(float)w/h;
                on=Read(camera,rt);foreach(var r in views)r.enabled=false;off=Read(camera,rt);
                var a=on.GetPixels32();var b=off.GetPixels32();int xmin=w,ymin=h,xmax=-1,ymax=-1;
                for(int i=0;i<a.Length;i++){
                    if(Math.Abs(a[i].r-b[i].r)+Math.Abs(a[i].g-b[i].g)+Math.Abs(a[i].b-b[i].b)<=6)continue;
                    int x=i%w,y=i/w;result.count++;xmin=Math.Min(xmin,x);xmax=Math.Max(xmax,x);ymin=Math.Min(ymin,y);ymax=Math.Max(ymax,y);
                }
                result.x=xmin;result.y=ymin;result.width=xmax<0?0:xmax-xmin+1;result.height=ymax<0?0:ymax-ymin+1;
                if(result.count>0||requireVisible){
                    File.WriteAllBytes(Path.Combine(dir,result.on),on.EncodeToPNG());File.WriteAllBytes(Path.Combine(dir,result.off),off.EncodeToPNG());
                }
                if(requireVisible)Need(result.count>0,"Visible actual GPU pixels "+id);
                Need(game.elapsed==elapsed && JsonUtility.ToJson(actor)==oldActor,"GPU observation preserves model and actor");
                return result;
            } finally {
                for(int i=0;i<views.Count;i++)if(views[i])views[i].enabled=enabled[i];
                camera.targetTexture=previous;camera.rect=rect;camera.aspect=aspect;RenderTexture.active=active;
                if(on)UnityEngine.Object.Destroy(on);if(off)UnityEngine.Object.Destroy(off);if(rt){rt.Release();UnityEngine.Object.Destroy(rt);}
            }
        }
        static Texture2D Read(Camera c,RenderTexture rt) {
            RenderTexture.active=rt;GL.Clear(true,true,Color.clear);c.Render();RenderTexture.active=rt;
            var image=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0,false);image.Apply(false,false);return image;
        }
    }
}
#endif
