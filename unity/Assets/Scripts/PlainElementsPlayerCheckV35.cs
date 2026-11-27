#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
    public sealed class PlainElementsPlayerCheckV35:MonoBehaviour {
        const string Flag="--root-plain-element-check-v35",Out="--root-plain-element-output-v35";
        static PlainElementsPlayerCheckV35 active;
        [Serializable] sealed class Shot {
            public string id,npcState,originalMotion,sourcePose,wholeKey;
            public int frame,facing,partCount;public float sourceClock,initialModelClock,worldHeight,alpha,hp;
            public PixelProof pixels,nativeGaitPixels;
        }
        [Serializable] sealed class Report {
            public string mode,npc,roomId,last,error;
            public int room,index,encounter,frames,knockdowns,ordinaryRear,ordinaryFront,counterRear,counterFront,terrainHiddenSamples;
            public bool passed,ordinaryRoute,originalBirth,donorClock,naturalAttack,normalHit,deepCompletion,returned,reentry;
            public float nativeWaitHeight,visibleWaitHeight,originalTarget;
            public List<Shot> shots=new List<Shot>();public List<string> errors=new List<string>(),transitions=new List<string>();
        }
        Report report=new Report();DemoScene scene;GameModel game;Enemy actor;string output;
        int selectedFrame=-1;SpriteRenderer carrier;PlainVariablePhaseStateR1330 selectedState;PlainVariablePhaseMotionR1330 selectedMotion;
        double selectedClock;string sourcePose;bool done,oldBackground;double began;
        SourceWholeDocumentV21 whole;Dictionary<string,SourceWholeMotionV21> donor=new Dictionary<string,SourceWholeMotionV21>();
        internal static void Observe(DemoScene scene,Enemy e,SpriteRenderer r,PlainVariablePhaseBankR1330 b,PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 m,double clock,string pose){
            if(!active||active.scene!=scene||!ReferenceEquals(active.actor,e))return;
            active.selectedFrame=Time.frameCount;active.carrier=r;active.selectedState=state;active.selectedMotion=m;active.selectedClock=clock;active.sourcePose=pose;
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);if(f<0)return;
            try{int o=Array.IndexOf(args,Out);Need(f+1<args.Length&&o>=0&&o+1<args.Length,"Original Plain mode/output");
                string mode=args[f+1];Need(mode=="fire"||mode=="ice"||mode=="fire-solo"||mode=="ice-solo","Declared four real Plain births");
                string path=Path.GetFullPath(args[o+1]);string root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(path.StartsWith(root,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh element report");
                Need(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,"D3D11 required");
                Directory.CreateDirectory(path);active=new GameObject("Explicit Plain Element V35").AddComponent<PlainElementsPlayerCheckV35>();
                active.output=path;active.report.mode=mode;
            }catch(Exception ex){Debug.LogException(ex);Application.Quit(2);}
        }
        void Log(string msg,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert||msg.Contains("PLAIN_ICE_SOLO_RECOVERY_FALLBACK")||msg.Contains("PLAIN_ELEMENT_DRAWINGS_FALLBACK"))report.errors.Add(msg+" "+stack);}
        IEnumerator Start(){
            began=Time.realtimeSinceStartupAsDouble;oldBackground=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;
            var run=Flow();Exception error=null;
            while(!done){bool more=false;object step=null;
                try{Need(report.errors.Count==0,"Healthy original route, draw and model");Need(Time.realtimeSinceStartupAsDouble-began<550,"Plain total deadline "+report.last);
                    more=run.MoveNext();if(more)step=run.Current;
                }catch(Exception e){error=e;}
                if(error!=null||!more)break;report.frames++;yield return step;
            }
            (run as IDisposable)?.Dispose();if(!done){report.error=error?.ToString();report.passed=error==null;Finish();}
        }
        void Update(){if(!done&&began>0&&Time.realtimeSinceStartupAsDouble-began>=550){report.error="Finite element deadline "+report.last;Finish();}}
        void Finish(){
            if(done)return;done=true;Application.logMessageReceived-=Log;Application.runInBackground=oldBackground;active=null;
            try{File.WriteAllText(Path.Combine(output,"plain-element-player-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}
        }
        bool Fire{get{return report.mode.StartsWith("fire",StringComparison.Ordinal);}}
        bool Solo{get{return report.mode.EndsWith("solo",StringComparison.Ordinal);}}
        Camera Cam{get{return (Camera)Field(scene,"worldCamera");}}
        FairyAuthoredPartsV33 Owner{get{return (FairyAuthoredPartsV33)Field(scene,"plainIceRecoveryPartsV35");}}
        void LoadDonor(){
            var pin=Fire?PlainElementDrawingsV35.FireDoc:PlainElementDrawingsV35.IceDoc;var bytes=Resources.Load<TextAsset>("Pixel/dungeons/altera-core-hard/data/"+pin);
            Need(bytes,"Actual approved existing drawing library");
            whole=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(bytes.bytes);
            foreach(var m in whole.motions)donor.Add(m.id,m);
            Need(donor.Count==28&&whole.keys.Length==(Fire?98:153),"Complete original existing hand-drawn common motions");
        }
        IEnumerable Enter(){
            foreach(var step in EnterRoute(scene,"altera","altera-plain"))yield return step;
            double until=Time.realtimeSinceStartupAsDouble+110;
            for(;;){game=(GameModel)Field(scene,"game");var native=Field(scene,"nativeScene");
                if(scene.PlainProductSessionActiveR1320&&game!=null&&!game.paused&&(string)Field(scene,"openMenu")==""&&native!=null&&
                    (string)Field(native,"id")=="WI_ALTERA_PLAIN_STAGE1")break;
                Need(Time.realtimeSinceStartupAsDouble<until,"True Plain ordinary admission/world");yield return null;}
            report.ordinaryRoute=true;report.npc="NUI_NASOD_ELEMENT_"+(Fire?"FIRE":"ICE")+(Solo?"_SOLO":"");
            report.room=Solo?(Fire?0:1):2;report.index=Solo?(Fire?13:26):(Fire?0:1);report.encounter=Solo&&!Fire?1:0;
            if(game.room!=report.room){Call(game,"EnterRoom",report.room);if((int)Field(scene,"shownRoom")!=game.room)Call(scene,"RebuildRoom");}
            if(game.EncounterIndex!=report.encounter)Call(game,"BeginEncounter",report.encounter);
            actor=game.enemies[report.index];report.roomId=game.CurrentRoom.id;
            Need(actor.sourceNPC==report.npc&&actor.encounterIndex==report.encounter&&actor.sourceKeyCode==0&&actor.active&&!actor.dormant&&actor.aiEnabled,"Correct original Plain birth/AI");
            report.originalBirth=true;game.player.hp=game.player.maxHp=100000;
            report.originalTarget=Fire?153.492793f:150.634400f;
        }
        void Place(float distance,bool back=false){
            float x=Mathf.Clamp(actor.x+distance*(back?-1:1)*actor.facing,35,game.CurrentRoom.width-35);
            var floor=game.SurfaceBelow(x,Math.Max(actor.y,game.player.y)+4,true);
            if(floor==null){x=actor.x;floor=game.SurfaceBelow(x,actor.y+600,true);}Need(floor!=null,"Original player floor");
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;game.player.invincible=30;
        }
        void Status(){
            string state=actor.npcState??"physics-"+actor.state;report.last=state+" npc="+actor.npcTime+" clip="+selectedMotion?.animation+" clock="+selectedClock+" hp="+actor.hp;
            if(report.transitions.Count==0||report.transitions[report.transitions.Count-1]!=state)report.transitions.Add(state);
        }
        bool NearScreen(){
            if(selectedFrame!=Time.frameCount||!carrier||!carrier.enabled||!carrier.gameObject.activeInHierarchy)return false;
            var p=Cam.WorldToViewportPoint(carrier.bounds.center);return p.z>0&&p.x>.015f&&p.x<.985f&&p.y>.015f&&p.y<.985f;
        }
        Shot CaptureActor(string id){
            Need(NearScreen(),"Original renderer in actual camera "+id);
            bool extra=selectedMotion.animation=="StandUpAttackFront"||selectedMotion.animation=="StandUpAttackBack";
            string key="";var rs=new List<SpriteRenderer>();
            if(extra){
                var o=Owner;Need(report.mode=="ice-solo"&&o!=null&&o.LastFrame.alias==selectedMotion.animation&&
                    Math.Abs(o.LastFrame.sourceClock-selectedClock)<.00001&&o.LastFrame.visible&&carrier.forceRenderingOff,"Exact original two-motion part owner");
                o.VisitRenderers(rs.Add);Need(rs.Count==PlainIceRecoveryRigV35.PartCount,"Ice recovery body and two independent shoes");
            }else{
                var m=donor[selectedMotion.animation];var phases=m.phases;int i=0;
                while(i+1<phases.Length&&phases[i+1].sourceTimeOriginal<=selectedClock+.000001)i++;
                key=phases[i].paintedWholeKey;
                Need(carrier.sprite && !carrier.forceRenderingOff && carrier.sprite.name=="Plain element V35/"+key&&
                    carrier.flipX==(actor.facing<0),"Exact independently selected original-clock donor key "+key);
                Need(Math.Abs(carrier.transform.localScale.x-(Fire?1.3232137333f:1.3213557927f))<.00002f,"Same-plane source-relative actor scale");
                rs.Add(carrier);report.donorClock=true;
            }
            var p=Capture(Cam,game,actor,rs,output,id);
            var shot=new Shot{id=id,npcState=selectedState.state,originalMotion=selectedMotion.animation,sourcePose=sourcePose,wholeKey=key,
                frame=Time.frameCount,facing=actor.facing,partCount=rs.Count,sourceClock=(float)selectedClock,initialModelClock=actor.npcTime,
                worldHeight=p.height*2*Cam.orthographicSize/720f,alpha=rs[0].color.a,hp=actor.hp,pixels=p};
            report.shots.Add(shot);return shot;
        }
        IEnumerable WaitFor(Func<bool> predicate,string id,float distance=70){
            double end=Time.realtimeSinceStartupAsDouble+60;
            for(;;){Place(distance);yield return null;Status();if(NearScreen()&&predicate())yield break;
                Need(Time.realtimeSinceStartupAsDouble<end,"Natural source phase "+id+" "+report.last);}
        }
        bool TryVisibleCapture(string id,bool native=false){
            int before=report.shots.Count;
            try {
                var shot=CaptureActor(id);
                if(native){
                    int h=Mathf.RoundToInt(2*Cam.orthographicSize),w=Mathf.RoundToInt(h*Cam.aspect);
                    shot.nativeGaitPixels=CaptureAtResolution(Cam,game,actor,new List<SpriteRenderer>{carrier},output,id+"-native",w,h);
                }
                return true;
            }catch(InvalidOperationException ex) when(ex.Message.StartsWith("Rig Player V35: Visible actual GPU pixels ",StringComparison.Ordinal)){
                if(report.shots.Count>before)report.shots.RemoveRange(before,report.shots.Count-before);
                report.terrainHiddenSamples++;return false;
            }
        }
        IEnumerable VisibleSource(Func<bool> condition,string id,float distance){
            double until=Time.realtimeSinceStartupAsDouble+65;
            for(;;){
                Place(distance);yield return null;Status();
                if(NearScreen()&&condition()&&TryVisibleCapture(id))yield break;
                Need(Time.realtimeSinceStartupAsDouble<until,"Actual visible original source state "+id+" "+report.last);
            }
        }
        IEnumerable VisibleKnockdown(){
            double end=Time.realtimeSinceStartupAsDouble+65;
            for(;;){
                foreach(var s in PublicDown(false))yield return s;
                while(actor.state==CombatState.Knockdown){
                    yield return null;Status();
                    if(NearScreen()&&selectedMotion.animation=="DamageDownFront"&&TryVisibleCapture("03-public-knockdown"))yield break;
                }
                Need(Time.realtimeSinceStartupAsDouble<end,"Actual visible original public knockdown "+report.last);
            }
        }
        int ReadWholeWaitInk(){
            // A real terrain occluder may cover a foot or an entire torso. Keep
            // the ordinary camera evidence separately. Measure the selected live
            // sprite at its actual scale with its production material on a 1x plane.
            Need(carrier&&carrier.sprite&&carrier.sharedMaterial,"Live donor for whole-body height");
            float before=game.elapsed;string actorBefore=JsonUtility.ToJson(actor);var fallback=RenderTexture.active;
            GameObject cameraObject=null,bodyObject=null;RenderTexture rt=null;Texture2D pixels=null;
            try {
                const int width=384,height=256;Vector3 center=new Vector3(30000,30000);
                cameraObject=new GameObject("Plain original-size ink camera");var camera=cameraObject.AddComponent<Camera>();
                camera.enabled=false;camera.orthographic=true;camera.orthographicSize=height*.5f;camera.aspect=(float)width/height;
                camera.transform.position=center+new Vector3(0,0,-100);camera.clearFlags=CameraClearFlags.SolidColor;
                camera.backgroundColor=Color.clear;camera.cullingMask=1<<31;camera.allowMSAA=false;camera.allowHDR=false;
                bodyObject=new GameObject("Selected Plain original-size ink"){layer=31};
                var body=bodyObject.AddComponent<SpriteRenderer>();body.sprite=carrier.sprite;body.color=carrier.color;
                body.sharedMaterial=carrier.sharedMaterial;body.flipX=carrier.flipX;body.flipY=carrier.flipY;
                body.transform.localScale=carrier.transform.lossyScale;body.transform.rotation=carrier.transform.rotation;
                body.transform.position=center-(body.bounds.center-body.transform.position);
                var properties=new MaterialPropertyBlock();carrier.GetPropertyBlock(properties);
                properties.SetFloat("_NativeSceneClipR1461",0);body.SetPropertyBlock(properties);
                rt=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32){filterMode=FilterMode.Point,antiAliasing=1};
                Need(rt.Create(),"Actual D3D11 original-size ink target");camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
                pixels=new Texture2D(width,height,TextureFormat.RGBA32,false);pixels.ReadPixels(new Rect(0,0,width,height),0,0,false);pixels.Apply(false,false);
                int min=height,max=-1;var data=pixels.GetPixels32();for(int i=0;i<data.Length;i++)if(data[i].a>=64){min=Math.Min(min,i/width);max=Math.Max(max,i/width);}
                Need(min>1&&max>min&&max<height-2,"Complete uncropped alpha64 silhouette");
                File.WriteAllBytes(Path.Combine(output,"01-own-wait-whole-native.png"),pixels.EncodeToPNG());
                Need(game.elapsed==before&&JsonUtility.ToJson(actor)==actorBefore,"Size observation keeps the live model and camera intact");
                return max-min+1;
            }finally{
                RenderTexture.active=fallback;if(cameraObject)Destroy(cameraObject);if(bodyObject)Destroy(bodyObject);
                if(pixels)Destroy(pixels);if(rt){rt.Release();Destroy(rt);}
            }
        }
        IEnumerable WalkCycle(int side){
            bool[] seen=new bool[4];int hits=0;
            double until=Time.realtimeSinceStartupAsDouble+40;
            while(hits<4){
                // Preserve the source 168 approach radius and 336 detection radius.
                // Only place LK on an existing floor. Each original actor owns its AI.
                float x=Mathf.Clamp(actor.x+200*side,35,game.CurrentRoom.width-35);
                var floor=game.SurfaceBelow(x,Math.Max(actor.y,game.player.y)+4,true)??game.SurfaceBelow(x,actor.y+145,true);
                if(floor!=null){game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;
                    game.player.vx=game.player.vy=0;game.player.invincible=30;}
                yield return null;Status();
                Need(Time.realtimeSinceStartupAsDouble<until,"Natural own walking cycle "+side+" "+hits+" "+report.last);
                if(!NearScreen()||Math.Sign(game.player.x-actor.x)!=side || selectedMotion.animation!="FastWalk")continue;
                int quarter=Mathf.Clamp(Mathf.FloorToInt((float)(selectedClock/selectedMotion.sourceMotionDuration)*4),0,3);
                if(seen[quarter])continue;
                string id="walk-"+(side>0?"hero-right-":"hero-left-")+quarter;
                if(!TryVisibleCapture(id,true))continue;
                seen[quarter]=true;hits++;
            }
        }
        IEnumerable PublicDown(bool back){
            double until=Time.realtimeSinceStartupAsDouble+50;
            for(;;){Place(52,back);yield return null;Place(52,back);Status();
                if(actor.hp>1&&actor.invincible<=0&&!game.EnemyHasNpcInvincibility(actor)&&!game.EnemyHasSuperArmor(actor)&&game.Grounded(actor)&&actor.state==CombatState.Normal) {
                    int damage=game.ApplyHitToEnemy(actor,1,HitReaction.Knockdown);
                    if(damage>0&&actor.state==CombatState.Knockdown){report.knockdowns++;yield break;}
                }
                Need(Time.realtimeSinceStartupAsDouble<until,"Vulnerable original grounded down window "+report.last);
            }
        }
        IEnumerable Recover(bool back){
            string side=back?"BACK":"FRONT";string down="NASOD_ELEMENT_ICE_SOLO_DAMAGE_DOWN_"+side;
            string counter="NASOD_ELEMENT_ICE_SOLO_STAND_UP_ATTACK_"+side,ordinary="NASOD_ELEMENT_ICE_SOLO_STAND_UP_"+side;
            var start=back?game.iceDownCounterBackV35:game.iceDownCounterFrontV35;
            for(int attempt=0;attempt<85;attempt++){
                foreach(var s in PublicDown(back))yield return s;
                Need(actor.npcState==down,"Actual source down direction "+side);int roll=game.iceDownChoicesV35;
                bool sampleDown=attempt==0;while(game.iceDownChoicesV35==roll){yield return null;Status();
                    if(sampleDown&&NearScreen()&&actor.npcState==down&&actor.npcTime>.08f){CaptureActor("down-"+side);sampleDown=false;}}
                Need(game.iceDownChoicesV35==roll+1,"One roll per full original down play");
                if(actor.npcState==counter){
                    Need((back?game.iceDownCounterBackV35:game.iceDownCounterFrontV35)==start+1,"Original 10% counter");
                    float hit=back?.40f:.20f;
                    foreach(var s in WaitFor(()=>actor.npcState==counter&&selectedClock>=hit,"original recovery strike "+side,44))yield return s;
                    CaptureActor("counter-"+side+"-strike");
                    if(back)report.counterRear++;else report.counterFront++;yield break;
                }
                Need(actor.npcState==ordinary,"Original 90% normal recovery "+side+" "+actor.npcState);
                if(back)report.ordinaryRear++;else report.ordinaryFront++;
            }
            throw new InvalidOperationException("Original 10% condition did not select within the bounded genuine attempts "+side);
        }
        IEnumerable ReturnMenu(){
            while(scene.BossIntroductionActive)yield return null;
            Press(scene,"dungeons");for(int i=0;i<6;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons"&&Field(scene,"plainIceRecoveryPartsV35")==null&&
                Field(scene,"plainFireDrawingsV35")==null&&Field(scene,"plainIceDrawingsV35")==null,"Released all element resources on ordinary exit");report.returned=true;
        }
        IEnumerator Flow(){
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;for(int i=0;i<3;i++)yield return null;
            if((string)Field(scene,"openMenu")!="dungeons"){Call(scene,"Action","dungeons");yield return null;}
            LoadDonor();foreach(var s in Enter())yield return s;
            foreach(var s in VisibleSource(()=>selectedMotion.animation=="Wait","01-own-wait",142))yield return s;
            var idle=report.shots[report.shots.Count-1];report.visibleWaitHeight=idle.worldHeight;report.nativeWaitHeight=ReadWholeWaitInk();
            Need(Math.Abs(report.nativeWaitHeight-report.originalTarget)<Math.Max(5,report.originalTarget*.055f),"Native complete ink height "+report.nativeWaitHeight+" target "+report.originalTarget);
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--root-plain-element-gait-v35")>=0){
                int initialSide=game.CurrentRoom.width-actor.x>=actor.x?1:-1;
                foreach(var s in WalkCycle(initialSide))yield return s;
                foreach(var s in WalkCycle(-initialSide))yield return s;
            }
            foreach(var s in VisibleSource(()=>selectedState.state.Contains("_ATTACK_")&&!selectedState.state.Contains("_READY"),"02-own-attack",76))yield return s;
            report.naturalAttack=true;
            foreach(var s in VisibleKnockdown())yield return s;report.normalHit=true;
            if(report.mode=="ice-solo"){foreach(var s in Recover(false))yield return s;foreach(var s in Recover(true))yield return s;
                Need(report.counterFront==1&&report.counterRear==1,"Both own special recoveries");}
            report.deepCompletion=true;var first=actor;foreach(var s in ReturnMenu())yield return s;foreach(var s in Enter())yield return s;
            Need(!ReferenceEquals(first,actor),"Fresh original element birth");foreach(var s in VisibleSource(()=>true,"04-reentry",160))yield return s;report.reentry=true;
            foreach(var s in ReturnMenu())yield return s;
        }
    }
}
#endif
