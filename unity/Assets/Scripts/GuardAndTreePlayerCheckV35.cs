#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ElswordPixel {
    // Explicit controlled checks. All enemies, AI, positions, hitboxes and
    // source clocks are owned by the normal GameModel and DemoScene Update.
    public sealed partial class GuardAndTreePlayerCheckV35 : MonoBehaviour {
        const BindingFlags Fields=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
        const int Width=1280,Height=720;
        const string Guard="NUI_NASOD_GUARD",Tree="NUI_TREE_KNIGHT";
        [Serializable] public sealed class Observation {
            public string label,state,combat,sourceMotion,sourceKey,phase,image;
            public int facing,frontDefence,damage,actualParts,eligible,callbacks,otherCallbacks,pixels;
            public float hpBefore,hpAfter,sourceTime,revealTime,revealProgress,deathTime,maxVisibleAlpha;
            public bool hidden,armor,visible,pairVerified,footInCamera,barVisible,warningVisible;
            public string status;
        }
        [Serializable] public sealed class Report {
            public string mode,failure="",graphics,room,npc;
            public bool passed,offscreenStandaloneGPU=true,windowPresentationVerified=false,naturalClear=false;
            public List<Observation> observations=new List<Observation>();
            public List<string> events=new List<string>();
        }
        DemoScene scene;GameModel game;Enemy enemy;string output;double started;RenderTexture rt;
        bool done,onPass;int post;readonly HashSet<SpriteRenderer> callbacks=new HashSet<SpriteRenderer>();
        string runtimeError;
        readonly Report report=new Report();

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach() {
            var args=Environment.GetCommandLineArgs();int flag=Array.IndexOf(args,"--root-guard-tree-check-v35");
            int mouse=Array.IndexOf(args,"--root-waterway-mouse-check-v35");int plainMouse=Array.IndexOf(args,"--root-plain-mouse-check-v35");if(flag<0&&mouse<0&&plainMouse<0)return;
            try {
                Need((flag>=0?1:0)+(mouse>=0?1:0)+(plainMouse>=0?1:0)==1,"One controlled route observer");int to=Array.IndexOf(args,plainMouse>=0?"--root-plain-mouse-output-v35":mouse>=0?"--root-waterway-mouse-output-v35":"--root-guard-tree-output-v35");
                Need(to>=0&&to+1<args.Length&&(plainMouse>=0||mouse>=0||flag+1<args.Length),"Mode and fresh output required");
                string mode=plainMouse>=0?"plain-mouse":mouse>=0?"waterway-mouse":args[flag+1],path=Path.GetFullPath(args[to+1]);
                Need(PaintModeV40(mode)||LegacySizeModeV39(mode)||ArmedModeV37(mode)||FatPaintModeV37(mode)||BendersModeV36(mode)||mode=="guard-current"||mode=="guard-parts"||mode=="guard-gait"||mode=="knight"||mode=="waterway-mouse"||mode=="plain-mouse","Explicit mode");
                Need(!Application.isEditor&&SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null,"Standalone GPU");
                Need(path.StartsWith(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\.codex\runtime-drafts\",StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(path)&&!File.Exists(path),"Fresh owned output");
                Directory.CreateDirectory(path);var owner=new GameObject("Controlled Guard/Tree check V35").AddComponent<GuardAndTreePlayerCheckV35>();
                owner.report.mode=mode;owner.output=path;
            }catch(Exception e){Debug.LogException(e);Application.Quit(2);}
        }
        static void Need(bool b,string why){if(!b)throw new InvalidOperationException("GUARD_TREE_V35 "+why);}
        static object Field(object owner,string name){var f=owner.GetType().GetField(name,Fields);Need(f!=null,name);return f.GetValue(owner);}
        static object Call(object owner,string name,params object[] args){var m=owner.GetType().GetMethod(name,Fields);Need(m!=null,name);return m.Invoke(owner,args);}
        void RuntimeLog(string text,string stack,LogType type){
            if(runtimeError==null&&(type==LogType.Exception||type==LogType.Error||type==LogType.Assert))runtimeError=text+"\n"+stack;
        }
        void Budget(){Need(runtimeError==null,"Player runtime error: "+runtimeError);Need(Time.realtimeSinceStartupAsDouble-started<360,"360s watchdog");}
        void Press(string action) {
            TouchControl c=null;
            foreach(var t in scene.GetComponentsInChildren<TouchControl>(true))if(t&&t.isActiveAndEnabled&&ReferenceEquals(t.owner,scene)&&t.action==action){Need(c==null,"Duplicate UI "+action);c=t;}
            Need(c&&EventSystem.current&&c.GetComponent<Image>().raycastTarget,"Active UI "+action);
            var p=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,pointerId=-3535,position=RectTransformUtility.WorldToScreenPoint(null,c.transform.position)};
            try{Need(ExecuteEvents.Execute(c.gameObject,p,ExecuteEvents.pointerDownHandler),"UI down");}finally{if(c)ExecuteEvents.Execute(c.gameObject,p,ExecuteEvents.pointerUpHandler);}
            report.events.Add("UI "+action);
        }
        IEnumerable Enter(string region,string route) {
            Press("region:"+region);yield return null;
            var routes=(IList)Field(scene,"mapDungeons");var visible=(IList)Field(scene,"mapVisible");int index=-1,card=-1;
            for(int i=0;i<routes.Count;++i)if((string)Field(routes[i],"id")==route)index=i;
            for(int i=0;i<visible.Count;++i)if((int)visible[i]==index)card=i;
            Need(index>=0&&card>=0,"Visible route "+route);
            int size=(int)scene.GetType().GetField("MapPageSize",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue();
            for(int i=0;(int)Field(scene,"mapPage")!=card/size;++i){Need(i<20,"Pages");Press("dungeon-page:"+((int)Field(scene,"mapPage")<card/size?"1":"-1"));yield return null;}
            Press("dungeon-card:"+(card%size));yield return null;Need((int)Field(scene,"mapSelected")==index,"Selected route");
            Press("dungeon-enter");yield return null;
            if(route=="altera-nasod-factory"){
                while(!scene.FactoryProductSessionActiveR1179||scene.FactoryProductSessionLoadingR1179){Budget();yield return null;}
                game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id=="Dungeon_Altera_Nasod_Factory_Normal-0","Original first factory room");
            }else if(route=="altera-plain"){
                while(!scene.PlainProductSessionActiveR1320||scene.PlainProductSessionLoadingR1320){Budget();yield return null;}
                game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id=="Dungeon_Altera_Plain_Normal-0","Original normal Plain entry");
            }else if(PaintModeV40(report.mode)){game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id==PaintFirstRoomV40(),"Original painted route entry");}
            else if(LegacySizeModeV39(report.mode)){while(!SizeRouteReadyV39()){Budget();yield return null;}game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id==SizeFirstRoomV39(),"Original size route entry");}
            else{game=(GameModel)Field(scene,"game");Need(game.CurrentRoom.id==(BendersFirstRoomV36(route)),"Original first room");}
            Need(!game.paused,"Active game");
        }
        void Protect(float offset) {
            game.player.x=enemy.x+offset;game.player.y=enemy.y;game.player.groundLine=enemy.groundLine;
            game.player.vx=game.player.vy=0;game.player.invincible=30;
        }
        Camera World {get{return (Camera)Field(scene,"worldCamera");}}
        void After(Camera c){if(onPass&&c==World)post=Time.frameCount;}
        internal void Will(SpriteRenderer r,Camera c){if(onPass&&c==World)callbacks.Add(r);}
        List<SpriteRenderer> Parts(out int authored) {
            int index=game.enemies.IndexOf(enemy);Need(index>=0,"Registered game enemy");
            var group=new List<SpriteRenderer>();
            group.Add(((IList)Field(scene,"enemyViews"))[index] as SpriteRenderer);Need(group[0],"Carrier");
            if(PaintModeV40(report.mode)||LegacySizeModeV39(report.mode)){authored=1;return group;}
            if(ArmedModeV37(report.mode))return ArmedMembersV37(group,out authored);
            if(FatPaintModeV37(report.mode))return FatPaintMembersV37(group,out authored);
            if(BendersModeV36(report.mode))return BendersPartsV36(group,out authored);
            string owner=enemy.sourceNPC==Tree?"treeKnightA1ViewsV30":(MouseRigV35.Identity(enemy)||enemy.sourceNPC=="NUI_NASOD_MOUSE")?"mouseViewsV35":"guardViewsV35";
            var fv=scene.GetType().GetField(owner,Fields);
            if(fv!=null){
                var dict=(IDictionary)fv.GetValue(scene);
                if(dict.Contains(enemy)){
                    var o=Field(dict[enemy],"parts");
                    if(enemy.sourceNPC==Tree)((TreeKnightAuthoredPartsV30)o).VisitRenderersR1461(r=>group.Add(r));
                    else ((FairyAuthoredPartsV33)o).VisitRenderers(r=>group.Add(r));
                }
            }
            authored=group.Count-1;
            if(report.mode=="waterway-mouse"||report.mode=="plain-mouse")Need(authored==5&&group[0].forceRenderingOff,"Original five registered mouse bodies and hidden carrier");
            if(report.mode=="guard-parts"||report.mode=="guard-gait")Need(authored==GuardRigV35.PartCount&&group[0].forceRenderingOff,"Registered fifteen-part guard and hidden carrier");
            return group;
        }
        Observation Row(string tag){
            var r=new Observation{label=tag,state=enemy.npcState,combat=enemy.state.ToString(),facing=enemy.facing,sourceTime=enemy.npcTime,hpBefore=enemy.hp,hpAfter=enemy.hp};
            if(PaintModeV40(report.mode))PaintRowV40(r);
            else if(LegacySizeModeV39(report.mode))SizeRowV39(r);
            else if(FatPaintModeV37(report.mode))FatPaintRowV37(r);
            else if(ArmedModeV37(report.mode)||BendersModeV36(report.mode))BendersRowV36(r);
            else if(enemy.sourceNPC==Tree){
                r.phase=game.EnemyInitialPosePhaseV25(enemy);r.revealTime=game.EnemyInitialPoseElapsedV25(enemy);r.revealProgress=game.EnemyInitialRevealProgressV25(enemy);
                r.hidden=game.EnemyInitialHurtboxHiddenV25(enemy);r.armor=game.EnemyInitialSuperArmorV25(enemy);r.visible=game.EnemyInitialPresentationVisibleV25(enemy);
            }else if(MouseRigV35.Identity(enemy)||enemy.sourceNPC=="NUI_NASOD_MOUSE"){
                var states=(IDictionary)Field(scene,"mouseViewsV35");Need(states.Contains(enemy),"Mouse source owner");
                var own=states[enemy];r.sourceMotion=(string)Field(own,"motion");r.sourceTime=(float)Field(own,"sourceClock");
                r.sourceKey=string.Join(",",(int[])Field(own,"keys"));r.phase=(string)Field(own,"actualAlias");
            }else{
                var map=(FactoryReactionPoseMapR1467)Field(scene,"factoryReactionPoseMapR1467");string state;float t;bool reaction;
                if(map.TrySelect(enemy,out state,out t,out reaction)){
                    var banks=(Dictionary<string,SourcePhaseWholeLookupV21>)Field(scene,"factoryProductReferenceR1179");
                    var selected=banks[Guard].Select(state,t);r.sourceMotion=selected.State.Motion;r.sourceKey=selected.Whole.Key;r.sourceTime=selected.SourceMotionClock;
                }
            }
            report.observations.Add(r);return r;
        }
        Texture2D Read() {
            var t=new Texture2D(Width,Height,TextureFormat.RGBA32,false);
            t.ReadPixels(new Rect(0,0,Width,Height),0,0);t.Apply(false,false);return t;
        }
        void Capture(Observation row,bool expectVisible) {
            var group=Parts(out row.actualParts);var prior=new bool[group.Count];
            int index=game.enemies.IndexOf(enemy);
            var bar=((IList)Field(scene,"enemyBars"))[index] as SpriteRenderer;
            var warn=((IList)Field(scene,"warnings"))[index] as SpriteRenderer;
            var status=((IList)Field(scene,"enemyDebuffViews"))[index] as TextMesh;
            row.barVisible=bar&&bar.enabled&&bar.gameObject.activeInHierarchy;row.warningVisible=warn&&warn.enabled&&warn.gameObject.activeInHierarchy;row.status=status?status.text:"";
            if(enemy.sourceNPC==Tree&&row.hidden)Need(!row.barVisible&&!row.warningVisible&&string.IsNullOrEmpty(row.status),"Hidden tree labels and warning");
            row.deathTime=enemy.deathTime;
            for(int i=0;i<group.Count;++i){var r=group[i];prior[i]=r.enabled;if(r.enabled&&!r.forceRenderingOff&&r.gameObject.activeInHierarchy&&r.sprite&&r.color.a>0){row.eligible++;row.maxVisibleAlpha=Math.Max(row.maxVisibleAlpha,r.color.a);}
                var witness=r.GetComponent<GuardTreeDrawV35>();if(!witness)witness=r.gameObject.AddComponent<GuardTreeDrawV35>();witness.Bind(this,r);}
            if(!rt){rt=new RenderTexture(Width,Height,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(rt.Create(),"D3D11 target");}
            var c=World;var old=c.targetTexture;var oldActive=RenderTexture.active;var rect=c.rect;float aspect=c.aspect;
            string actorBefore=JsonUtility.ToJson(enemy);float clock=game.elapsed;Texture2D on=null,off=null;
            try {
                c.targetTexture=rt;c.rect=new Rect(0,0,1,1);c.aspect=(float)Width/Height;RenderTexture.active=rt;callbacks.Clear();post=-1;onPass=true;
                GL.Clear(true,true,Color.clear);c.Render();onPass=false;Need(post==Time.frameCount,"Original world camera post-render");on=Read();
                // Other previously observed instances may still be visible during
                // death fade. Count only this exact owner's intersecting group;
                // keep foreign draws as an explicit diagnostic rather than
                // letting them add to or compensate for missing target members.
                foreach(var r in group)if(callbacks.Contains(r))row.callbacks++;
                row.otherCallbacks=callbacks.Count-row.callbacks;
                foreach(var r in group)r.enabled=false;GL.Clear(true,true,Color.clear);c.Render();off=Read();
                var first=on.GetPixels32();var second=off.GetPixels32();
                var local=(Vector2)Call(scene,"RenderEnemyPosition",enemy);
                var actors=(Transform)Field(scene,"actorsRoot");var foot=actors.TransformPoint(new Vector3(local.x,local.y,0));
                var projected=(Vector3)Call(scene,"ProjectNativePoint",foot,enemy.nativeDepth);
                var center=c.WorldToViewportPoint(projected);row.footInCamera=center.x>0&&center.x<1&&center.y>=0&&center.y<1;
                var area=new Bounds(projected+Vector3.up*45,new Vector3(180,160,0));
                foreach(var r in group)if(r&&r.sprite&&r.gameObject.activeInHierarchy)area.Encapsulate(r.bounds);
                var lo=c.WorldToViewportPoint(area.min);var hi=c.WorldToViewportPoint(area.max);
                int x0=Mathf.Clamp(Mathf.FloorToInt(Math.Min(lo.x,hi.x)*Width)-2,0,Width),x1=Mathf.Clamp(Mathf.CeilToInt(Math.Max(lo.x,hi.x)*Width)+2,0,Width);
                int y0=Mathf.Clamp(Mathf.FloorToInt(Math.Min(lo.y,hi.y)*Height)-2,0,Height),y1=Mathf.Clamp(Mathf.CeilToInt(Math.Max(lo.y,hi.y)*Height)+2,0,Height);
                for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++){int i=y*Width+x;if(Math.Abs(first[i].r-second[i].r)+Math.Abs(first[i].g-second[i].g)+Math.Abs(first[i].b-second[i].b)>6)row.pixels++;}
                if(row.label!="buried")Need(row.footInCamera,"Sample must be on-camera");
                row.image=(report.observations.Count-1).ToString("D3")+"-"+row.label+".png";
                File.WriteAllBytes(Path.Combine(output,row.image),on.EncodeToPNG());
                row.pairVerified=expectVisible?row.eligible>0&&row.callbacks>0&&row.pixels>0:row.eligible==0&&row.callbacks==0&&row.pixels==0;
                Need(row.pairVerified,"Owner on/off area and actual world callbacks "+row.label+" pixels="+row.pixels+" draws="+row.callbacks);
            }finally{onPass=false;for(int i=0;i<group.Count;i++)if(group[i])group[i].enabled=prior[i];c.targetTexture=old;c.rect=rect;c.aspect=aspect;RenderTexture.active=oldActive;if(on)Destroy(on);if(off)Destroy(off);}
            Need(clock==game.elapsed&&actorBefore==JsonUtility.ToJson(enemy),"Capture cannot write actor/step");
        }
        Observation Hit(string name,int amount,int expectedPercent,HitReaction reaction,bool capture=false){
            var row=Row(name);row.frontDefence=(int)game.CoreHardShieldFrontDefenceR1417(enemy,game.player.x);
            Need(row.frontDefence==expectedPercent,"Defense "+name+" state="+enemy.npcState);
            if(capture)Capture(row,true);
            row.damage=game.ApplyHitToEnemy(enemy,amount,reaction,280);row.hpAfter=enemy.hp;
            Need(row.damage==amount*(100-expectedPercent)/100&&row.hpBefore-row.hpAfter==row.damage,"HP delta "+name);
            return row;
        }
        void StageGuard(){
            Call(game,"EnterRoom",1);if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            // A second controlled entry has the same room ID, but may remove
            // summoned enemies left from the first run. Reconcile the ordinary
            // room presentation as the real route entry does before rendering.
            Call(scene,"RebuildRoom");
            Need(((IList)Field(scene,"enemyViews")).Count==game.enemies.Count,"Fresh room actor pool");
            enemy=game.enemies[0];Need(game.CurrentRoom.id=="Dungeon_Altera_Nasod_Factory_Normal-1"&&enemy.sourceNPC==Guard&&enemy.kind=="nasod-guard"&&enemy.encounterIndex==0,"Original Factory room 1 first Guard");
            report.room=game.CurrentRoom.id;report.npc=enemy.sourceNPC;
        }
        IEnumerable Natural(string state,int facing,float distance){
            double end=Time.realtimeSinceStartupAsDouble+55;
            for(;;){
                Protect(facing*distance);yield return null;Budget();Need(enemy.hp>0&&Time.realtimeSinceStartupAsDouble<end,"Natural "+state+" actual="+enemy.npcState);
                if(enemy.npcState==state&&enemy.facing==facing&&enemy.stun<=0&&enemy.state==CombatState.Normal&&enemy.npcTime>.06f&&enemy.npcTime<.9f)yield break;
            }
        }
        IEnumerable Shield(int facing){
            StageGuard();
            foreach(var s in Natural("NASOD_GUARD_WALK",facing,200))yield return s;
            Hit("walk-front-"+facing,100,70,HitReaction.Hurt,true);
            foreach(var s in Natural("NASOD_GUARD_ATTACK_B",facing,80))yield return s;
            Hit("attack-b-front-"+facing,100,50,HitReaction.Hurt,true);
            foreach(var s in Natural("NASOD_GUARD_ATTACK_A",facing,80))yield return s;
            Hit("attack-a-open-"+facing,100,0,HitReaction.None,true);
            foreach(var s in Natural("NASOD_GUARD_WALK",facing,200))yield return s;
            Protect(-facing*60);Hit("walk-behind-"+facing,100,0,HitReaction.Hurt);
            Need(enemy.state==CombatState.Hurt,"Real rear hurt");Protect(facing*60);Hit("hurt-open-"+facing,100,0,HitReaction.None);
            bool airborne=facing<0;
            if(airborne){
                foreach(var s in Natural("NASOD_GUARD_WALK",facing,200))yield return s;
                Protect(-facing*60);Hit("launch-behind",100,0,HitReaction.Launch);
                yield return null;Need(enemy.state==CombatState.Airborne&&enemy.knockdownOnLanding,"Public force-fly");
            }
            Protect(-facing*60);
            int lethal=game.ApplyHitToEnemy(enemy,100000,HitReaction.None);
            Need(lethal>0&&enemy.hp==0&&enemy.state==CombatState.Dead,"Actual lethal packet");
            yield return null;
            string phase=airborne?"sky":"ground";
            var early=Row("death-"+phase+"-early");Capture(early,true);
            Need(early.sourceMotion==(airborne?"DamageAirDownLanding":"DamageDownBack")&&early.maxVisibleAlpha>0,"Genuine original death whole");
            while(enemy.deathTime<.55f){Budget();yield return null;}
            var late=Row("death-"+phase+"-mid");Capture(late,true);
            Need(late.maxVisibleAlpha>0&&late.maxVisibleAlpha<early.maxVisibleAlpha&&late.sourceMotion==early.sourceMotion,"Existing death fade inherited");
        }
        IEnumerable Knight(){
            Need(game.enemies.Count>5,"Original ruins room list");enemy=game.enemies[5];
            Need(enemy.sourceNPC==Tree&&enemy.kind=="tree-knight","Original Ruins Knight birth index5");report.room=game.CurrentRoom.id;report.npc=enemy.sourceNPC;
            Need(game.EnemyInitialPosePhaseV25(enemy)=="buried-wait","Natural original buried start");var initial=Row("buried");
            Need(initial.hidden&&!initial.visible,"Underground visibility/hurtbox");float hp=enemy.hp;initial.damage=game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt);initial.hpAfter=enemy.hp;
            Need(initial.damage==0&&enemy.hp==hp,"Buried creature has no hurtbox");Capture(initial,false);
            bool before=false,after=false;
            while(game.EnemyInitialPosePhaseV25(enemy)!="complete"){
                Protect(-35);yield return null;Budget();
                if(game.EnemyInitialPosePhaseV25(enemy)=="complete")break;
                float t=game.EnemyInitialPoseElapsedV25(enemy);
                if(!before&&t>3.15f&&t<GameModel.TreeKnightRevealStartV25){
                    var r=Row("before-3_5");Need(r.hidden&&!r.visible&&r.armor&&r.revealProgress==0,"Last buried phase");Capture(r,false);before=true;
                }
                if(!after&&t>=3.79f&&t<GameModel.TreeKnightStartDurationV25){
                    var r=Row("after-3_5");Need(!r.hidden&&r.visible&&r.armor&&r.revealProgress>0&&r.revealProgress<1,"Visible opening with genuine armor");
                    Capture(r,true);r.damage=game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt);r.hpAfter=enemy.hp;Need(r.damage==1&&enemy.hp==r.hpBefore-1&&game.EnemyInitialSuperArmorV25(enemy),"Visible emerging body takes HP");after=true;
                }
            }
            Need(before&&after,"Both sides of reveal sampled");var fin=Row("complete-4_433");Need(!fin.hidden&&fin.visible&&!fin.armor&&fin.revealProgress==1&&Math.Abs(fin.revealTime-GameModel.TreeKnightStartDurationV25)<.0001f,"Original opening complete at 266/60");
            Capture(fin,true);
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--root-tree-original-hits-v45")>=0)
                foreach(var tick in OriginalTreeHitsV45())yield return tick;
        }
        IEnumerator Flow(){
            while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>())){Budget();yield return null;}
            for(int i=0;i<3;i++)yield return null;
            game=(GameModel)Field(scene,"game");Need(game.paused&&(string)Field(scene,"openMenu")=="dungeons","Ordinary main menu");
            if(report.mode=="guard-gait"){foreach(var x in GuardGaitFlowV36())yield return x;}
            else if(report.mode=="plain-mouse"){foreach(var x in PlainMouseFlowV35())yield return x;}
            else if(PaintModeV40(report.mode)){foreach(var x in LegacyPaintFlowV40())yield return x;}
            else if(LegacySizeModeV39(report.mode)){foreach(var x in LegacySizeFlowV39())yield return x;}
            else if(ArmedModeV37(report.mode)){foreach(var x in ArmedFlowV37())yield return x;}
            else if(FatPaintModeV37(report.mode)){foreach(var x in FatPaintFlowV37())yield return x;}
            else if(BendersModeV36(report.mode)){foreach(var x in BendersFlowV36())yield return x;}
            else if(report.mode=="waterway-mouse"){foreach(var x in WaterwayMouseFlowV35())yield return x;}
            else if(report.mode=="knight"){foreach(var x in Enter("ruben","ruins"))yield return x;foreach(var x in Knight())yield return x;}
            else{foreach(var x in Enter("altera","altera-nasod-factory"))yield return x;foreach(var x in Shield(1))yield return x;foreach(var x in Shield(-1))yield return x;}
            Press("dungeons");for(int i=0;i<4;i++)yield return null;Need((string)Field(scene,"openMenu")=="dungeons"&&!scene.FactoryProductSessionActiveR1179&&!scene.PlainProductSessionActiveR1320,"Ordinary menu return");report.passed=true;
        }
        IEnumerator Start(){
            started=Time.realtimeSinceStartupAsDouble;report.graphics=SystemInfo.graphicsDeviceType.ToString();Camera.onPostRender+=After;Application.logMessageReceived+=RuntimeLog;var seq=Flow();
            while(!done){
                bool more=false;object value=null;try{Budget();more=seq.MoveNext();if(more)value=seq.Current;}catch(Exception e){report.failure=e.ToString();}
                if(!more)break;yield return value;
            }
            (seq as IDisposable)?.Dispose();done=true;Camera.onPostRender-=After;Application.logMessageReceived-=RuntimeLog;
            try{File.WriteAllText(Path.Combine(output,report.mode=="plain-mouse"?"plain-mouse-report.json":report.mode=="waterway-mouse"?"waterway-mouse-report.json":"guard-tree-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}
        }
        void OnDestroy(){Camera.onPostRender-=After;Application.logMessageReceived-=RuntimeLog;if(rt){rt.Release();Destroy(rt);}}
    }
    public sealed class GuardTreeDrawV35 : MonoBehaviour {
        GuardAndTreePlayerCheckV35 check;SpriteRenderer sprite;
        internal void Bind(GuardAndTreePlayerCheckV35 c,SpriteRenderer r){check=c;sprite=r;}
        void OnWillRenderObject(){if(check&&sprite)check.Will(sprite,Camera.current);}
    }
}
#endif

