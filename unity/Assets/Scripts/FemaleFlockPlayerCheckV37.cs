#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class FemaleFlockPlayerCheckV37:MonoBehaviour {
        [Serializable] sealed class Row { public int root,frame,pixelCount,renderers;public float playerX,clock;public string picture; }
        [Serializable] sealed class Report {
            public bool passed,ordinaryTreeEntry,passiveOriginal,returnedMain,freshEntry;public string error="";
            public List<Row> views=new List<Row>();
        }
        readonly Report report=new Report();
        DemoScene scene;GameModel game;string output;bool oldBackground;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var a=Environment.GetCommandLineArgs();
            if(Array.IndexOf(a,"--root-tree-flock-check-v37")<0)return;
            try{
                int ix=Array.IndexOf(a,"--root-tree-flock-output-v37");
                Need(ix>=0&&ix+1<a.Length,"Fresh flock output required");
                var path=Path.GetFullPath(a[ix+1]);var allowed=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allowed,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path),"Fresh runtime-drafts flock output");
                Directory.CreateDirectory(path);
                new GameObject("Tree flock source check").AddComponent<FemaleFlockPlayerCheckV37>().output=path;
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        IEnumerator Start(){
            oldBackground=Application.runInBackground;Application.runInBackground=true;
            var flow=Run();Exception error=null;
            while(true){bool more=false;object value=null;try{more=flow.MoveNext();if(more)value=flow.Current;}catch(Exception e){error=e;}
                if(error!=null||!more)break;yield return value;}
            (flow as IDisposable)?.Dispose();report.error=error==null?"":error.ToString();report.passed=error==null;
            File.WriteAllText(Path.Combine(output,"flock-player-report.json"),JsonUtility.ToJson(report,true));
            Application.runInBackground=oldBackground;Application.Quit(report.passed?0:2);
        }
        IEnumerable Enter(){
            foreach(var t in EnterRoute(scene,"ruben","tree"))yield return t;
            game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id=="tree-1"&&!game.paused,"Original Tree entry");
            Call(game,"EnterRoom",1);for(int i=0;i<4;i++)yield return null;
            Need(game.CurrentRoom.id=="tree-2","Original second room");
            report.ordinaryTreeEntry=true;
        }
        object Current(){return Field(scene,"femaleNativeDisplayV28");}
        static MeshRenderer[] Parts(object display,int root){
            var group=(GameObject)Field(display,"group");var defs=(Array)Field(Field(display,"data"),"parts");var parts=new List<MeshRenderer>();
            foreach(var def in defs)if((int)Field(def,"componentRootIndex")==root){
                var name=(string)Field(def,"name");MeshRenderer found=null;
                foreach(var r in group.GetComponentsInChildren<MeshRenderer>())if(r.gameObject.name==name){found=r;break;}
                Need(found,"Original mesh renderer "+name);parts.Add(found);
            }
            return parts.ToArray();
        }
        void PlaceNear(Vector3 point){
            float best=game.player.x,bestD=float.MaxValue;
            for(float x=35;x<game.CurrentRoom.width-35;x+=4){
                var p=(Vector3)Call(scene,"NativePathPoint",x);float d=(p.x-point.x)*(p.x-point.x)+(p.z-point.z)*(p.z-point.z);
                if(d<bestD){best=x;bestD=d;}
            }
            var support=game.SurfaceBelow(best,game.player.y+145,true)??game.SurfaceBelow(best,10000,true);
            Need(support!=null,"Original source camera support");
            game.player.x=best;game.player.y=support.Y(best);game.player.groundLine=support.id;game.player.vx=game.player.vy=0;
        }
        static Color32[] NativePixels(Camera camera,Texture2D cpu){
            var rt=camera.targetTexture;Need(rt&&rt.width==640&&rt.height==360,"Original native camera target");
            var old=RenderTexture.active;
            try{camera.Render();RenderTexture.active=rt;cpu.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0,false);cpu.Apply();return cpu.GetPixels32();}
            finally{RenderTexture.active=old;}
        }
        void CaptureRoot(object display,int root,Renderer[] views,int sequence){
            var camera=(Camera)Field(scene,"nativeCamera");var cpu=new Texture2D(640,360,TextureFormat.RGBA32,false);
            float elapsed=game.elapsed;var saved=new bool[views.Length];for(int i=0;i<views.Length;i++)saved[i]=views[i].forceRenderingOff;
            string name=sequence.ToString("D2")+"-root-"+root;
            try{
                var on=NativePixels(camera,cpu);var png=cpu.EncodeToPNG();
                foreach(var v in views)v.forceRenderingOff=true;
                var off=NativePixels(camera,cpu);int count=0;
                for(int i=0;i<on.Length;i++)if(!on[i].Equals(off[i]))count++;
                Need(count>0,"Visible contribution from exact original flock member "+root);
                File.WriteAllBytes(Path.Combine(output,name+".png"),png);
                File.WriteAllBytes(Path.Combine(output,name+"-off.png"),cpu.EncodeToPNG());
                report.views.Add(new Row{root=root,frame=(int)Field(display,"frame"),pixelCount=count,renderers=views.Length,
                    playerX=game.player.x,clock=game.enemies[0].clock,picture=name+".png"});
            }finally{
                for(int i=0;i<views.Length;i++)views[i].forceRenderingOff=saved[i];
                camera.Render();Destroy(cpu);
            }
            Need(game.elapsed==elapsed,"Paired GPU renders do not advance the original model");
        }
        IEnumerator Run(){
            double until=Time.realtimeSinceStartupAsDouble+100;
            while(!(scene=FindFirstObjectByType<DemoScene>())){Need(Time.realtimeSinceStartupAsDouble<until,"Scene start");yield return null;}
            for(int i=0;i<4;i++)yield return null;
            foreach(var t in Enter())yield return t;
            var display=Current();Need(display!=null&&Field(display,"flockShapeV37")!=null&&Field(display,"a1Root17")!=null,"Real A1 and original flock renderers");
            var original=game.enemies[0];
            Need(original.sourceNPC=="NUI_CHICKEN_FEMALE"&&original.passive&&!original.aiEnabled&&original.active,"Original passive birth");
            report.passiveOriginal=true;int seq=0;
            foreach(int root in new[]{12,13,14,15,16,17}){
                var meshes=Parts(display,root);Need(meshes.Length>=3,"Original member parts");
                Renderer[] views;
                if(root==17){views=((GameObject)Field(display,"group")).GetComponentsInChildren<SpriteRenderer>();Need(views.Length==6,"Six approved A1 drawings");}
                else{
                    views=meshes;
                    foreach(var v in views)Need(v.enabled&&v.sharedMaterial.shader.name=="ElswordPixel/TreeFlockV37","Original mesh with drawn flock material");
                }
                var bounds=views[0].bounds;foreach(var v in views)bounds.Encapsulate(v.bounds);
                PlaceNear(bounds.center);for(int i=0;i<8;i++)yield return null;
                CaptureRoot(display,root,views,++seq);
            }
            Call(game,"EnterRoom",0);for(int i=0;i<4;i++)yield return null;Need(Current()==null,"Tree-1 restores the original owner");
            Press(scene,"dungeons");for(int i=0;i<3;i++)yield return null;Need(game.paused,"Return to map");report.returnedMain=true;
            foreach(var t in Enter())yield return t;
            Need(Current()!=null&&!ReferenceEquals(Current(),display)&&!ReferenceEquals(game.enemies[0],original),"Fresh source flock on room transition");report.freshEntry=true;
        }
    }
}
#endif
