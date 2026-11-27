#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed class DirectorArtPlayerCheckV41:MonoBehaviour {
        const string Flag="--root-director-art-v41", Output="--root-director-output-v41";
        [Serializable] sealed class Sample {
            public string route,actor,selected,legacy,source,hidden;
            public int originalSample,registrations,visibleActorPixels,visibleRepaintPixels,explicitVsLegacy,legacyVsSource,matchedLegacySlots;
            public float age;public bool hookActive;
        }
        [Serializable] sealed class Report {
            public bool passed;public string last="",error="",gpu;public List<Sample> samples=new List<Sample>();
            public List<string> steps=new List<string>();
        }
        sealed class Held {
            internal Renderer target;internal Material[] selected,source;internal bool enabled;
        }
        string output,mode;Report result=new Report();DemoScene scene;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            string[] args=Environment.GetCommandLineArgs();int n=Array.IndexOf(args,Flag);if(n<0)return;
            try{
                int o=Array.IndexOf(args,Output);
                Need(n+1<args.Length&&o>=0&&o+1<args.Length&&SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11,"Explicit D3D11 mode and output");
                string mode=args[n+1];Need(mode=="dragon"||mode=="raven"||mode=="all","dragon, raven or all");
                string dir=Path.GetFullPath(args[o+1]),root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(dir.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(dir)&&!File.Exists(dir),"Fresh runtime output");Directory.CreateDirectory(dir);
                var c=new GameObject("Explicit ordinary cinematic paint review").AddComponent<DirectorArtPlayerCheckV41>();c.mode=mode;c.output=dir;
            }catch(Exception e){Debug.LogError(e);Application.Quit(2);}
        }
        IEnumerator Start(){
            bool old=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;result.gpu=SystemInfo.graphicsDeviceType.ToString();
            var flow=Flow();double end=Time.realtimeSinceStartupAsDouble+(mode=="all"?310:165);
            try{
                for(;;){bool move=false;object next=null;
                    try{Need(String.IsNullOrEmpty(result.error),result.error);Need(Time.realtimeSinceStartupAsDouble<end,"Total deadline "+result.last);move=flow.MoveNext();if(move)next=flow.Current;}
                    catch(Exception e){result.error=e.ToString();}
                    if(!move||!String.IsNullOrEmpty(result.error))break;yield return next;
                }
            }finally{(flow as IDisposable)?.Dispose();Application.logMessageReceived-=Log;Application.runInBackground=old;}
            result.passed=String.IsNullOrEmpty(result.error);try{File.WriteAllText(Path.Combine(output,"director-art-player-report.json"),JsonUtility.ToJson(result,true));}finally{Application.Quit(result.passed?0:2);}
        }
        void Log(string msg,string trace,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert)result.error=msg+"\n"+trace;}
        IEnumerator Flow(){
            while(!(scene=FindObjectOfType<DemoScene>())||Field(scene,"game")==null||(string)Field(scene,"openMenu")!="dungeons")yield return null;
            foreach(string kind in new[]{"dragon","raven"}){
                if(mode!=kind&&mode!="all")continue;
                bool dragon=kind=="dragon";string route=dragon?"besma-dragon-nest":"altera-battle-airship",room=dragon?"Dungeon_Besma_Dragon_Nest_Normal-2":"Dungeon_Altera_Battle_Air_Ship_Normal-3";
                result.last=route+" original ordinary entry";
                foreach(var t in EnterRoute(scene,dragon?"bethma":"altera",route))yield return t;
                double end=Time.realtimeSinceStartupAsDouble+80;
                if(!dragon)while(!scene.BattleProductSessionActiveR1325){Need(Time.realtimeSinceStartupAsDouble<end,"Wait for owned Battle model");yield return null;}
                var game=(GameModel)Field(scene,"game");var rooms=(List<RoomDefinition>)Field(game,"rooms");int index=rooms.FindIndex(q=>q.id==room);
                Need(index>=0,"Exact authored director room "+room);if(game.room!=index)Call(game,"EnterRoom",index);
                Need(game.CurrentRoom.id==room,"Native room selected");game.player.invincible=120;
                var surfaces=new List<DirectorSurfaceV41>();float[] times=dragon?new[]{.45f,3.55f,7.55f}:new[]{.9f,4.65f,10f};int done=0;
                end=Time.realtimeSinceStartupAsDouble+40;var seen=new HashSet<int>();
                while(done<times.Length){
                    yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Source director film/camera "+kind+" observed "+done);
                    if(!scene.DirectorSubjectsV41(dragon,surfaces,out var camera,out float age,out int sample)||age<times[done]||seen.Contains(sample))continue;
                    seen.Add(sample);result.last=kind+" source sample="+sample+" age="+age;
                    var shot=Observe(game,kind,route,age,sample,surfaces,camera,done);
                    if(shot.visibleActorPixels<8||shot.visibleRepaintPixels<3)continue;
                    result.samples.Add(shot);done++;
                }
                end=Time.realtimeSinceStartupAsDouble+30;
                while(scene.DirectorSubjectsV41(dragon,surfaces,out _,out _,out _)||scene.BossIntroductionActive){
                    yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Original director/intro releases control");
                }
                Press(scene,"dungeons");for(int i=0;i<8;i++)yield return null;
                Need((string)Field(scene,"openMenu")=="dungeons"&&scene.ReleasedDirectorPaintV41(dragon),"Original menu and scoped cleanup "+kind);
                result.steps.Add(route+" original source camera, room event, materials and menu restored");
            }
        }
        Sample Observe(GameModel game,string kind,string route,float age,int frame,List<DirectorSurfaceV41> surfaces,Camera cam,int index){
            const int w=960,h=540;
            var hook=FindObjectOfType<MonsterTextureRedrawV22>();Need(hook,"Existing V22 global camera hook exists");
            var result=new Sample{route=route,actor=kind,age=age,originalSample=frame,registrations=surfaces.Count,hookActive=hook.CameraHookEnabled&&hook.IsConfigured};
            var oldRT=cam.targetTexture;var active=RenderTexture.active;var rect=cam.rect;float aspect=cam.aspect,elapsed=game.elapsed;
            var pos=cam.transform.position;var rot=cam.transform.rotation;var focus=EventSystem.current?EventSystem.current.currentSelectedGameObject:null;
            bool wasHook=hook.CameraHookEnabled;RenderTexture rt=null;var held=new Dictionary<Renderer,Held>();var temp=new List<Material>();
            string label=kind+"-"+(index+1);
            try{
                foreach(var s in surfaces){
                    Need(s.renderer&&s.renderer.enabled&&s.original&&s.painted&&s.original!=s.painted,"Actor source and edited surface");
                    if(held.ContainsKey(s.renderer))continue;
                    var hld=new Held{target=s.renderer,enabled=s.renderer.enabled};held.Add(s.renderer,hld);
                    if(s.originalSprite)continue;
                    hld.selected=s.renderer.sharedMaterials;hld.source=(Material[])hld.selected.Clone();
                    foreach(var part in surfaces)if(part.renderer==s.renderer){
                        var m=new Material(hld.selected[part.slot]){mainTexture=part.original};temp.Add(m);hld.source[part.slot]=m;
                    }
                }
                rt=new RenderTexture(w,h,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};
                Need(rt.Create(),"Camera proof target");cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);cam.aspect=(float)w/h;
                var a=Capture(cam,rt,label+"-explicit.png");result.selected=label+"-explicit.png";
                foreach(var s in surfaces)if(s.originalSprite)((SpriteRenderer)s.renderer).sprite=s.originalSprite;
                foreach(var hld in held.Values)if(hld.source!=null)hld.target.sharedMaterials=hld.source;
                var b=Capture(cam,rt,label+"-legacy-hook.png");result.legacy=label+"-legacy-hook.png";
                foreach(var s in surfaces){
                    var selected=s.originalSprite?((SpriteRenderer)s.renderer).sprite.texture:s.renderer.sharedMaterials[s.slot].mainTexture;
                    if(selected==s.painted)result.matchedLegacySlots++;
                }
                hook.EnableCameraHook(false);
                foreach(var s in surfaces)if(s.originalSprite)((SpriteRenderer)s.renderer).sprite=s.originalSprite;
                foreach(var hld in held.Values)if(hld.source!=null)hld.target.sharedMaterials=hld.source;
                var c=Capture(cam,rt,label+"-original.png");result.source=label+"-original.png";
                foreach(var hld in held.Values)hld.target.enabled=false;
                var d=Capture(cam,rt,label+"-hidden.png");result.hidden=label+"-hidden.png";
                result.visibleActorPixels=Delta(a,d);result.visibleRepaintPixels=Delta(a,c);
                result.explicitVsLegacy=Delta(a,b);result.legacyVsSource=Delta(b,c);
                Need(game.elapsed==elapsed&&cam.transform.position==pos&&cam.transform.rotation==rot&&
                    (!EventSystem.current||EventSystem.current.currentSelectedGameObject==focus),"Reading the world preserves time, camera and focus");
                return result;
            }finally{
                foreach(var s in surfaces)if(s.renderer&&s.originalSprite)((SpriteRenderer)s.renderer).sprite=s.selectedSprite;
                foreach(var hld in held.Values)if(hld.target){if(hld.selected!=null)hld.target.sharedMaterials=hld.selected;hld.target.enabled=hld.enabled;}
                hook.EnableCameraHook(wasHook);cam.targetTexture=oldRT;cam.rect=rect;cam.aspect=aspect;RenderTexture.active=active;
                foreach(var m in temp)if(m)Destroy(m);if(rt){rt.Release();Destroy(rt);}
            }
        }
        Color32[] Capture(Camera cam,RenderTexture rt,string name){
            RenderTexture.active=rt;GL.Clear(true,true,Color.clear);cam.Render();RenderTexture.active=rt;
            var tex=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);
            try{tex.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0,false);tex.Apply(false,false);File.WriteAllBytes(Path.Combine(output,name),tex.EncodeToPNG());return tex.GetPixels32();}
            finally{Destroy(tex);}
        }
        static int Delta(Color32[] a,Color32[] b){int n=0;for(int i=0;i<a.Length;i++)if(Math.Abs(a[i].r-b[i].r)+Math.Abs(a[i].g-b[i].g)+Math.Abs(a[i].b-b[i].b)>6)n++;return n;}
    }
}
#endif

