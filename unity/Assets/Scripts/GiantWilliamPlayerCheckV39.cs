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
    public sealed partial class GiantWilliamPlayerCheckV39:MonoBehaviour {
        const string Flag="--root-giant-william-check-v39",Output="--root-giant-william-output-v39";
        [Serializable] public sealed class Feet {public string motion;public int index;public float left,right;public bool flight;}
        [Serializable] public sealed class Weapon {public string motion,page;public int index;public float gripX,gripY,tipX,tipY;}
        [Serializable] sealed class Ref {public string npc;public Feet[] feet;public Weapon[] weapon;}
        [Serializable] public sealed class Shot {public string label,kind,alias,motion,page,paintClip;public int index,rawIndex,paintIndex,facing,entry;
            public bool painted,flight;public float clock,hp,left,right,barGap,anchorX,anchorY,visibleBodyFraction;public Vector2 grip,tip,groundRoot;public PixelProof gpu;}
        [Serializable] sealed class Result {public string error="",last="",gpu;public bool passed;public List<Shot> shots=new List<Shot>();public List<string> steps=new List<string>();}
        sealed class Target {
            public readonly string mode,kind,region,route,room;public readonly int offset,encounter;
            public Target(string m,string k,string r,string id,string s,int n,int e){mode=m;kind=k;region=r;route=id;room=s;offset=n;encounter=e;}
        }
        static readonly Target[] Targets={new Target("white","white-giant","ruben","tree","tree-3",2,0),
            new Target("red","pporu-giant-red","elder","banthus-cave","cave-2",1,0),
            new Target("normal","william","ruben","ruins","ruins-3",2,0),
            new Target("b","pporu-william-b","elder","wally-castle-outskirts","outskirts-2",1,1),
            new Target("c","pporu-william-c","elder","wally-castle-outskirts","outskirts-3",2,0)};
        Result report=new Result();string output,mode;double started;bool done,oldBackground;Target target;
        DemoScene scene;GameModel game;Enemy actor;SpriteRenderer rendererView;Ref proof;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);if(f<0)return;
            try{int o=Array.IndexOf(args,Output);Need(f+1<args.Length&&o>=0&&o+1<args.Length&&SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11,"Explicit D3D11 mode/output");
                string m=args[f+1];Need(m=="all"||Array.Exists(Targets,t=>t.mode==m),"Mode white/red/normal/b/c/all");
                string path=Path.GetFullPath(args[o+1]);string allow=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(allow,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh runtime-drafts output");Directory.CreateDirectory(path);
                var check=new GameObject("Explicit giant / William player check").AddComponent<GiantWilliamPlayerCheckV39>();check.mode=m;check.output=path;
            }catch(Exception e){Debug.LogError(e);Application.Quit(2);}
        }
        IEnumerator Start(){
            started=Time.realtimeSinceStartupAsDouble;oldBackground=Application.runInBackground;Application.runInBackground=true;
            report.gpu=SystemInfo.graphicsDeviceType.ToString();Application.logMessageReceived+=Log;var it=Flow();
            while(!done){bool move=false;object next=null;try{Need(String.IsNullOrEmpty(report.error),report.error);
                    Need(Time.realtimeSinceStartupAsDouble-started<(mode=="all"?620:mode=="red"?235:145),"Total deadline "+report.last);
                    move=it.MoveNext();if(move)next=it.Current;
                }catch(Exception e){report.error=e.ToString();}if(!move||!String.IsNullOrEmpty(report.error))break;yield return next;}
            (it as IDisposable)?.Dispose();if(done)yield break;done=true;Application.logMessageReceived-=Log;Application.runInBackground=oldBackground;
            report.passed=String.IsNullOrEmpty(report.error);try{File.WriteAllText(Path.Combine(output,"giant-william-player-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}
        }
        void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Assert||type==LogType.Exception||message.Contains("GIANT_WILLIAM_SKIN_FALLBACK"))report.error=message+"\n"+trace;}
        void OnDestroy(){Application.logMessageReceived-=Log;}
        IEnumerator Flow(){
            while(!(scene=FindObjectOfType<DemoScene>())||Field(scene,"game")==null)yield return null;
            while((string)Field(scene,"openMenu")!="dungeons")yield return null;
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--source-ruben-paint-v45")>=0){
                foreach(var x in RubenPaintV45())yield return x;
                yield break;
            }
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--source-altera-choices-v45")>=0){
                foreach(var x in AlteraChoicesV45())yield return x;
                yield break;
            }
            foreach(var row in Targets)if(mode=="all"||mode==row.mode){
                target=row;entryNumber=0;foreach(var x in Enter())yield return x;foreach(var x in Await(w=>w.selected.motion=="Wait","01-idle",20))yield return x;
                if(mode=="white"&&Array.IndexOf(Environment.GetCommandLineArgs(),"--source-white-rush-v45")>=0){
                    foreach(var x in WhiteRushV45())yield return x;
                    foreach(var x in Leave())yield return x;
                    yield break;
                }
                if(mode=="normal"&&Array.IndexOf(Environment.GetCommandLineArgs(),"--source-ruben-special-v45")>=0){
                    foreach(var x in RubenSpecialV45())yield return x;
                    yield break;
                }
                Place(205);foreach(var x in Await(w=>w.selected.alias.Contains("walk"),"02-walk",35))yield return x;
                if(target.mode=="red")foreach(var x in ClearRedSightline())yield return x;
                Place(94);
                foreach(var x in Await(w=>w.selected.alias.Contains("attack")&&!w.selected.alias.Contains("ready")&&!w.selected.alias.EndsWith("end"),"03-natural-attack",42))yield return x;
                if(target.mode=="red")foreach(var x in RedPublicHurt())yield return x;
                else{
                    foreach(var x in Vulnerable())yield return x;Need(game.ApplyHitToEnemy(actor,1,HitReaction.Hurt)>0,"Public hurt");
                    foreach(var x in Await(w=>w.selected.alias.Contains("hurt"),"04-hurt",12))yield return x;
                }
                foreach(var x in Vulnerable())yield return x;Need(game.ApplyHitToEnemy(actor,1,HitReaction.Launch,280)>0,"Public launch");
                foreach(var x in Await(w=>w.selected.motion.StartsWith("DamageAir",StringComparison.Ordinal),"05-air",16))yield return x;
                var old=actor;foreach(var x in Leave())yield return x;foreach(var x in Enter())yield return x;
                Need(!ReferenceEquals(old,actor),"Fresh original actor");foreach(var x in Await(w=>w.selected.motion=="Wait","06-reentry",20))yield return x;
                if(target.mode=="red"){
                    foreach(var x in ClearRedSightline("07-clear-second-birth"))yield return x;
                    Place(94);foreach(var x in RedAttackPhases())yield return x;
                }
                foreach(var x in Leave())yield return x;
            }
        }
        int entryNumber;
        IEnumerable Enter(){
            entryNumber++;
            report.last=target.kind+" ordinary route";foreach(var t in EnterRoute(scene,target.region,target.route))yield return t;
            game=(GameModel)Field(scene,"game");Call(game,"EnterRoom",target.offset);if(game.EncounterIndex!=target.encounter)Call(game,"BeginEncounter",target.encounter);
            Need(game.CurrentRoom.id==target.room,"Original room");for(int i=0;i<3;i++)yield return null;
            actor=game.enemies.Find(e=>e.kind==target.kind);
            Need(actor!=null,"Original birth record "+target.kind);
            game.player.hp=game.player.maxHp=5000;game.player.invincible=180;
            if(target.mode=="white")foreach(var t in WakeWhiteGiant())yield return t;
            Need(actor.active&&!actor.dormant,"Live original birth "+target.kind);
            rendererView=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
            var text=Resources.Load<TextAsset>("Pixel/feedback-v39/"+target.kind+"/source-check");Need(text,"Source sole / sword records");
            proof=JsonUtility.FromJson<Ref>(text.text);Need(proof.npc==actor.sourceNPC,"Source identity");
            Place(238);for(int i=0;i<3;i++)yield return null;
        }
        IEnumerable WakeWhiteGiant(){
            Need(actor.spawnAfterDeath=="benders"&&Mathf.Abs(actor.spawnDelay-2.2f)<.001f&&actor.dormant,"Original Benders-death birth gate");
            double end=Time.realtimeSinceStartupAsDouble+25;
            while(scene.BossIntroductionActive){yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Boss entry releases ordinary combat");}
            var predecessor=game.enemies.Find(e=>e.kind==actor.spawnAfterDeath&&e.active&&!e.dormant);
            Need(predecessor!=null&&predecessor.hp>0,"Original Benders target for white giant birth");
            while(predecessor.invincible>0||game.EnemyHasNpcInvincibility(predecessor)){
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Benders accepts the public hit");}
            int damage=game.ApplyHitToEnemy(predecessor,Mathf.CeilToInt(predecessor.hp+predecessor.maxHp+50),HitReaction.None);
            Need(damage>0&&predecessor.hp<=0,"Benders defeated by public combat");
            while(actor.dormant){
                Need(Time.realtimeSinceStartupAsDouble<end,"White giant original delayed birth "+predecessor.deathTime);
                yield return null;
            }
            Need(predecessor.deathTime+.02f>=actor.spawnDelay,"Original death delay elapsed");
            report.steps.Add("White giant original Benders death gate, delay "+predecessor.deathTime);
        }
        void Place(float distance,int requiredSide=0){
            float preferred=actor.x>=game.CurrentRoom.width*.55f?-1:1;
            float best=float.PositiveInfinity,x=0;GroundSurface floor=null;
            foreach(var s in game.CurrentRoom.surfaces){
                float left=Mathf.Max(34,s.x1+.15f),right=Mathf.Min(game.CurrentRoom.width-34,s.x2-.15f);
                if(right<left)continue;
                foreach(float side in new[]{preferred,-preferred}){
                    if(requiredSide!=0&&side!=requiredSide)continue;
                    float candidate=Mathf.Clamp(actor.x+side*distance,left,right),y=s.Y(candidate);
                    if(requiredSide!=0&&(candidate-actor.x)*requiredSide<30)continue;
                    var support=game.SurfaceBelow(candidate,y+.05f,false);
                    if(support==null||Mathf.Abs(support.Y(candidate)-y)>.08f)continue;
                    float score=Mathf.Abs(Mathf.Abs(candidate-actor.x)-distance)+Mathf.Abs(y-actor.y)*.5f+(side==preferred?0:2);
                    if(score<best){best=score;x=candidate;floor=support;}
                }
            }
            Need(floor!=null,"An actual room floor or platform exists for "+actor.kind);
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;game.player.invincible=180;
            Need(game.SurfaceBelow(x,game.player.y+.05f,false)==floor,"Verified original player support "+floor.id);
            report.steps.Add("Hero on original support "+floor.id+" at "+x+","+game.player.y+" distance "+Mathf.Abs(x-actor.x));
        }
        IEnumerable ClearRedSightline(string clearLabel="02b-clear-face-and-body"){
            double end=Time.realtimeSinceStartupAsDouble+68;float start=actor.x,previous=actor.x;int direction=1;
            double nextGuide=0,nextCapture=0;int attempts=0;
            var readable=new Dictionary<string,Texture2D>();
            try{
                while(Time.realtimeSinceStartupAsDouble<end){
                    double now=Time.realtimeSinceStartupAsDouble;
                    if(now>=nextGuide){
                        // Follow ordinary room support on the next clear stretch. Move only the hero through this fixture.
                        if(actor.x>start+380||actor.x>game.CurrentRoom.width-270)direction=-1;
                        if(actor.x<start-490||actor.x<290)direction=1;
                        Place(178,direction);nextGuide=now+1.25;
                    }
                    yield return null;
                    var w=scene.GiantWilliamFrameV39For(rendererView);var cam=(Camera)Field(scene,"worldCamera");
                    if(w==null||w.selected.painted||w.selected.motion!="Walk"||Mathf.Abs(actor.x-start)<30||
                       Mathf.Abs(actor.x-previous)<23||now<nextCapture||!cam)continue;
                    nextCapture=now+.48;previous=actor.x;attempts++;
                    if(!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam),rendererView.bounds))continue;
                    string probeName="red-sightline-probe-entry"+entryNumber;PixelProof sample;
                    try{sample=Capture(cam,game,actor,new List<SpriteRenderer>{rendererView},output,probeName);}
                    catch(InvalidOperationException ex) when(ex.Message.Contains("Visible actual GPU pixels")){
                        report.steps.Add("Source occluder completely covers actor at "+actor.x);continue;
                    }
                    var ratio=ActualRedSightline(w,cam,sample,readable);
                    report.steps.Add("Original red giant travel "+(actor.x-start)+" world on/off coverage body="+ratio.x+" face="+ratio.y+" attempt="+attempts);
                    if(ratio.x<.93f||ratio.y<.94f)continue;
                    report.last=target.kind+" "+clearLabel;
                    Observe(w,cam,target.kind+"-"+clearLabel);yield break;
                }
                Need(false,"Original AI movement did not yield a clear face/body in the neighboring surface; captures "+attempts);
            }finally{foreach(var tex in readable.Values)if(tex)Destroy(tex);}
        }
        Vector2 ActualRedSightline(GiantWilliamFrameV39 w,Camera camera,PixelProof pair,Dictionary<string,Texture2D> readable,bool walkFace=true){
            var sprite=w.selected.sprite;var rect=sprite.textureRect;int width=(int)rect.width,height=(int)rect.height;
            if(!readable.TryGetValue(w.selected.page,out var mask)){
                var asset=Resources.Load<TextAsset>(w.selected.page);Need(asset,"Exact encoded sprite mask");
                mask=new Texture2D(2,2,TextureFormat.RGBA32,false);readable[w.selected.page]=mask;
                Need(ImageConversion.LoadImage(mask,asset.bytes,false),"Readable copy of original rendered mask");
            }
            var alpha=mask.GetPixels((int)rect.x,(int)rect.y,width,height);
            var on=new Texture2D(2,2,TextureFormat.RGB24,false);var off=new Texture2D(2,2,TextureFormat.RGB24,false);
            float aspect=camera.aspect;
            try{
                Need(ImageConversion.LoadImage(on,File.ReadAllBytes(Path.Combine(output,pair.on)),false)&&
                     ImageConversion.LoadImage(off,File.ReadAllBytes(Path.Combine(output,pair.off)),false),"Readable actual scene on/off");
                var a=on.GetPixels32();var b=off.GetPixels32();int found=0,total=0,headFound=0,headTotal=0;
                camera.aspect=(float)on.width/on.height;
                for(int y=0;y<height;y+=2)for(int x=0;x<width;x+=2){
                    if(alpha[y*width+x].a<.75f)continue;
                    float lx=(x+.5f-sprite.pivot.x)/sprite.pixelsPerUnit,ly=(y+.5f-sprite.pivot.y)/sprite.pixelsPerUnit;
                    var world=rendererView.transform.TransformPoint(new Vector3(rendererView.flipX?-lx:lx,rendererView.flipY?-ly:ly,0));
                    var v=camera.WorldToViewportPoint(world);
                    int ix=Mathf.FloorToInt(v.x*on.width),iy=Mathf.FloorToInt(v.y*on.height);bool visible=false;
                    if(v.z>0&&ix>=0&&iy>=0&&ix<on.width&&iy<on.height){
                        int k=iy*on.width+ix;visible=Math.Abs(a[k].r-b[k].r)+Math.Abs(a[k].g-b[k].g)+Math.Abs(a[k].b-b[k].b)>6;
                    }
                    total++;if(visible)found++;
                    // Muzzle, eye and cheek occupy this registered area in the unflipped source Walk side view.
                    float nx=(x+.5f)/width,ny=1-(y+.5f)/height;
                    if(nx>=.66f&&nx<=.93f&&ny>=.13f&&ny<=.43f){headTotal++;if(visible)headFound++;}
                }
                Need(total>400&&(!walkFace||headTotal>25),"Registered native body and face sampling region");
                return new Vector2((float)found/total,headTotal==0?0:(float)headFound/headTotal);
            }finally{camera.aspect=aspect;Destroy(on);Destroy(off);}
        }
        IEnumerable Vulnerable(){double end=Time.realtimeSinceStartupAsDouble+36;
            while(actor.state!=CombatState.Normal||actor.invincible>0||game.EnemyHasNpcInvincibility(actor)||game.EnemyHasSuperArmor(actor)){
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Reach natural vulnerable state "+actor.npcState);}}
        IEnumerable Await(Func<GiantWilliamFrameV39,bool> match,string tag,int seconds){
            report.last=target.kind+" "+tag;double end=Time.realtimeSinceStartupAsDouble+seconds;
            for(;;){yield return null;var cam=(Camera)Field(scene,"worldCamera");var w=scene.GiantWilliamFrameV39For(rendererView);
                if(w!=null&&match(w)&&cam&&GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam),rendererView.bounds)){
                    Observe(w,cam,target.kind+"-"+tag);yield break;}
                Need(Time.realtimeSinceStartupAsDouble<end,report.last+" AI="+actor.npcState+" selected="+(w==null?"none":w.selected.alias));}
        }
        IEnumerable RedPublicHurt(){
            foreach(var x in Vulnerable())yield return x;
            float originalHp=actor.hp;
            Need(game.ApplyHitToEnemy(actor,1,HitReaction.Hurt)>0,"Public red hurt");
            int attempts=1;double nextHit=Time.realtimeSinceStartupAsDouble+1,end=Time.realtimeSinceStartupAsDouble+40;
            for(;;){
                yield return null;var w=scene.GiantWilliamFrameV39For(rendererView);var camera=(Camera)Field(scene,"worldCamera");
                if(w!=null&&w.selected.alias.Contains("hurt")&&camera&&
                   GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera),rendererView.bounds)&&
                   Observe(w,camera,target.kind+"-04-hurt",true)){
                    Need(actor.hp<originalHp,"Original red HP decreased");yield break;
                }
                double now=Time.realtimeSinceStartupAsDouble;
                Need(now<end,"Original visible red hurt after "+attempts+" public hits; selected "+(w==null?"none":w.selected.alias));
                if(now<nextHit)continue;
                if(w!=null&&attempts<3&&(w.selected.motion=="Walk"||w.selected.motion=="Wait")&&
                   actor.state==CombatState.Normal&&actor.invincible<=0&&!game.EnemyHasNpcInvincibility(actor)&&!game.EnemyHasSuperArmor(actor)){
                    if(game.ApplyHitToEnemy(actor,1,HitReaction.Hurt)>0)attempts++;
                }else if(Mathf.Abs(actor.x-game.player.x)<120)Place(170);
                nextHit=now+.7;
            }
        }
        IEnumerable RedAttackPhases(){
            // Observe the original AI in its ordinary encounter. It owns the choice of attack and its clock.
            bool ready=false,start=false,swing=false,recover=false;
            double end=Time.realtimeSinceStartupAsDouble+80,nextApproach=Time.realtimeSinceStartupAsDouble+6;
            while(!(ready&&start&&swing&&recover)){
                yield return null;
                var w=scene.GiantWilliamFrameV39For(rendererView);var camera=(Camera)Field(scene,"worldCamera");
                Need(Time.realtimeSinceStartupAsDouble<end,"Visible red attack phases: "+ready+"/"+start+"/"+swing+"/"+recover+" "+actor.npcState);
                if(Time.realtimeSinceStartupAsDouble>nextApproach){
                    if(Mathf.Abs(actor.x-game.player.x)>160)Place(94);
                    nextApproach=Time.realtimeSinceStartupAsDouble+6;
                }
                if(w==null||!camera||!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera),rendererView.bounds))continue;
                var s=w.selected;int i=s.rawIndex;
                if(s.motion=="SpecialAttackA_Ready" && s.alias=="source-attack-a-ready" && i>=14 && i<=54 && !ready)
                    ready=Observe(w,camera,target.kind+"-07a-ready-A",true);
                if(s.motion!="SpecialAttackA_Fire" || s.alias!="source-attack-a")continue;
                if(i<=10&&!start)start=Observe(w,camera,target.kind+"-07b-fire-A",true);
                if(i>=19&&i<=38&&!swing)swing=Observe(w,camera,target.kind+"-07c-swing-A",true);
                if(i>=43&&!recover)recover=Observe(w,camera,target.kind+"-07d-recover-A",true);
            }
        }
        bool Observe(GiantWilliamFrameV39 w,Camera camera,string label,bool allowOccluded=false){
            var r=rendererView;var s=w.selected;
            Need(ReferenceEquals(w.enemy,actor)&&r.sprite==s.sprite&&w.carrier&&r.enabled&&!r.forceRenderingOff&&r.gameObject.activeInHierarchy,"Visible source-clock carrier "+label);
            Need(r.flipX==(actor.facing!=1)&&Mathf.Abs(r.transform.localScale.x-s.scaleX)<.001f&&
                Mathf.Abs(r.transform.localScale.y-s.scaleY)<.001f,"Source facing and one registered world scale");
            if(!s.painted&&s.motion=="Wait")Need(Mathf.Abs(s.sprite.rect.height*s.scaleY-(actor.kind.Contains("giant")?145.60f:92.27f))<6,
                "Actual standing sprite preserves original size "+s.sprite.rect.height*s.scaleY);
            var shot=new Shot{label=label,kind=actor.kind,alias=s.alias,motion=s.motion,page=s.page,index=s.index,rawIndex=s.rawIndex,
                painted=s.painted,paintClip=s.paintClip,paintIndex=s.paintIndex,facing=w.facing,clock=w.clock,hp=actor.hp,groundRoot=w.foot,entry=entryNumber};
            var offsets=(IDictionary)Field(scene,"spriteOffsets");
            if(offsets.Contains(s.sprite)){
                var offset=(Vector2)offsets[s.sprite];shot.anchorX=s.sprite.rect.width*.5f-offset.x;shot.anchorY=s.sprite.rect.height*.5f+offset.y;
            }
            var sole=Array.Find(proof.feet,f=>f.motion==s.motion&&f.index==s.rawIndex);
            if(sole!=null){shot.left=sole.left;shot.right=sole.right;shot.flight=sole.flight;if(!sole.flight)Need(Mathf.Abs(Mathf.Min(sole.left,sole.right))<=.501f,"Original supporting foot");}
            if(!s.painted&&proof.weapon.Length>0){var wp=Array.Find(proof.weapon,p=>p.motion==s.motion&&p.index==s.rawIndex);
                Need(wp!=null&&wp.page==s.page,"Original weapon frame");shot.grip=new Vector2(wp.gripX*w.facing,wp.gripY);shot.tip=new Vector2(wp.tipX*w.facing,wp.tipY);
                if(s.motion=="Wait")Need(Vector2.Distance(shot.grip,shot.tip)>94,"Original long sword reach");}
            var bar=((List<SpriteRenderer>)Field(scene,"enemyBars"))[game.enemies.IndexOf(actor)];
            float top=r.transform.TransformPoint(new Vector3(0,s.top,0)).y;shot.barGap=bar.transform.position.y-top;
            if(bar.enabled)Need(Mathf.Abs(shot.barGap-8)<1.1f,"Visible HP gap "+shot.barGap);
            var texture=camera.targetTexture;var focus=EventSystem.current?EventSystem.current.currentSelectedGameObject:null;
            shot.gpu=CaptureAtResolution(camera,game,actor,new List<SpriteRenderer>{r},output,label,1280,720,!allowOccluded);
            if(shot.gpu.count==0 && allowOccluded)return false;
            if(allowOccluded&&s.motion.StartsWith("SpecialAttackA",StringComparison.Ordinal)){
                var masks=new Dictionary<string,Texture2D>();
                try{shot.visibleBodyFraction=ActualRedSightline(w,camera,shot.gpu,masks,false).x;}
                finally{foreach(var mask in masks.Values)if(mask)Destroy(mask);}
                if(shot.visibleBodyFraction<.5f)return false;
            }
            Need(shot.gpu.count>0&&camera.targetTexture==texture&&r.enabled&&(!EventSystem.current||EventSystem.current.currentSelectedGameObject==focus),"Actual scene on/off and focus restored");report.shots.Add(shot);return true;
        }
        IEnumerable Leave(){
            double end=Time.realtimeSinceStartupAsDouble+12;
            while(scene.BossIntroductionActive){yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Boss introduction releases the ordinary menu input");}
            var w=scene.GiantWilliamFrameV39For(rendererView);var old=w==null?null:w.selected.sprite;Press(scene,"dungeons");for(int i=0;i<8;i++)yield return null;
            report.steps.Add("Exit "+target.route+" menu="+Field(scene,"openMenu")+" paused="+game.paused+" oldSprite="+(bool)old+
                " banks="+((IDictionary)Field(scene,"giantWilliamBanksV39")).Count+" witnesses="+((IDictionary)Field(scene,"giantWilliamWitnessV39")).Count);
            Need(game.paused&&(string)Field(scene,"openMenu")=="dungeons"&&!old&&((IDictionary)Field(scene,"giantWilliamBanksV39")).Count==0&&
                 ((IDictionary)Field(scene,"giantWilliamWitnessV39")).Count==0,"Menu releases every owned sprite/page");
            var routes=(IList)Field(scene,"mapDungeons");int selected=(int)Field(scene,"mapSelected");Need(selected>=0&&(string)Field(routes[selected],"id")==target.route,"Route focus restored");
            report.steps.Add("Released "+target.route);
        }
    }
}
#endif
