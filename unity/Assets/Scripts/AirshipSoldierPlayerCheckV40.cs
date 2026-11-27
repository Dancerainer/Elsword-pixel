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
    public sealed class AirshipSoldierPlayerCheckV40 : MonoBehaviour {
        const string Flag="--root-airship-soldier-check-v40", Output="--root-airship-soldier-output-v40";
        static readonly string[] Modes={"battle-gunner","battle-high","battle-repairman","battle-swordman","recycle-gunner","recycle-swordman"};
        [Serializable] sealed class Birth {public string route,room;public int spawn,encounter,key;public float spawnX,scale;}
        [Serializable] sealed class Feet {public string motion;public int index;public float left,right;public bool flight;}
        [Serializable] sealed class Weapon {public string motion,page;public int index;public float gripX,gripY,tipX,tipY,trueTipDistance;}
        [Serializable] sealed class Proof {public string npc,sourceGrip;public float bodyHeight,fullEquipmentHeight;public Birth[] births;public Feet[] feet;public Weapon[] weapon;}
        [Serializable] sealed class Shot {public string label,route,room,alias,motion,page;public int spawn,exposure,raw,facing;public bool native,flight,barVisible;
            public float clock,scale,hp,barGap,left,right,weaponLength;public Vector2 foot,grip,tip;public PixelProof gpu;}
        [Serializable] sealed class Report {public bool passed; public string error="",last="",gpu;public List<string> steps=new List<string>();public List<Shot> shots=new List<Shot>();}
        readonly Report report=new Report();string mode,output,run,kind,route,id; DemoScene scene;GameModel game,menuGame;
        object menuLibrary;Proof proof;Birth birth;Enemy actor;SpriteRenderer view;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void Attach(){
            var args=Environment.GetCommandLineArgs();int f=Array.IndexOf(args,Flag);if(f<0)return;
            try {int o=Array.IndexOf(args,Output);Need(f+1<args.Length&&o>=0&&o+1<args.Length&&SystemInfo.graphicsDeviceType==GraphicsDeviceType.Direct3D11,"D3D11 / explicit output");
                string mode=args[f+1];Need(mode=="all"||Array.IndexOf(Modes,mode)>=0,"Known original Battle / Recycle entry");
                string dir=Path.GetFullPath(args[o+1]);string allowed=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar;
                Need(dir.StartsWith(allowed,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(dir)&&!File.Exists(dir),"Fresh runtime-drafts output");
                Directory.CreateDirectory(dir);var c=new GameObject("Airship original-soldier runtime review").AddComponent<AirshipSoldierPlayerCheckV40>();c.mode=mode;c.output=dir;
            }catch(Exception e){Debug.LogError(e);Application.Quit(2);}
        }
        IEnumerator Start(){
            bool background=Application.runInBackground;Application.runInBackground=true;Application.logMessageReceived+=Log;report.gpu=SystemInfo.graphicsDeviceType.ToString();
            var work=Flow();double end=Time.realtimeSinceStartupAsDouble+(mode=="all"?1350:240);
            try{
                for(;;){bool move=false;object next=null;
                    try {Need(String.IsNullOrEmpty(report.error),report.error);Need(Time.realtimeSinceStartupAsDouble<end,"Overall deadline "+report.last);move=work.MoveNext();if(move)next=work.Current;}
                    catch(Exception e){report.error=e.ToString();}if(!move||!String.IsNullOrEmpty(report.error))break;yield return next;
                }
            }finally{(work as IDisposable)?.Dispose();Application.logMessageReceived-=Log;Application.runInBackground=background;}
            report.passed=String.IsNullOrEmpty(report.error);
            try{File.WriteAllText(Path.Combine(output,"airship-soldier-player-report.json"),JsonUtility.ToJson(report,true));}finally{Application.Quit(report.passed?0:2);}
        }
        void Log(string msg,string trace,LogType type){if(type==LogType.Error||type==LogType.Assert||type==LogType.Exception||msg.Contains("AIRSHIP_SOLDIER_SKIN_FALLBACK"))report.error=msg+"\n"+trace;}
        IEnumerator Flow(){
            while(!(scene=FindObjectOfType<DemoScene>())||Field(scene,"game")==null||(string)Field(scene,"openMenu")!="dungeons")yield return null;
            foreach(string target in Modes){
                if(mode!="all"&&mode!=target)continue;
                run=target;route=target.StartsWith("battle",StringComparison.Ordinal)?"Battle":"Recycle";
                id=route=="Battle"?"altera-battle-airship":"altera-plain-recycle";kind="airship-soldier-"+target.Substring(target.IndexOf('-')+1);
                var text=Resources.Load<TextAsset>("Pixel/feedback-v40/"+kind+"/source-check");Need(text,"Original source measurements");
                proof=JsonUtility.FromJson<Proof>(text.text);Need(proof.sourceGrip=="Dummy1_Rhand","Original right hand socket");
                birth=Array.Find(proof.births,b=>b.route==route&&(kind!="airship-soldier-repairman"||b.spawn==18));
                Need(birth!=null&&birth.scale==1,"Registered source birth");
                foreach(var t in Enter())yield return t;
                foreach(var t in Await(w=>w.selected.native&&w.selected.motion.EndsWith("Wait",StringComparison.Ordinal),"01-idle",30))yield return t;
                Place(kind.EndsWith("gunner")?215:185);
                foreach(var t in Await(w=>w.selected.alias=="walk","02-walk",32))yield return t;
                Place(56);
                foreach(var t in Await(w=>w.selected.alias=="attack-a-ready","03-natural-ready",32))yield return t;
                foreach(var t in Await(w=>w.selected.alias=="attack-a","04-natural-fire",12))yield return t;
                foreach(var t in Vulnerable())yield return t;Need(game.ApplyHitToEnemy(actor,1,HitReaction.Hurt)>0,"Public hurt");
                foreach(var t in Await(w=>w.selected.native&&w.selected.alias.StartsWith("hurt",StringComparison.Ordinal)&&w.selected.motion.StartsWith("Damage",StringComparison.Ordinal),"05-registered-hurt",12))yield return t;
                foreach(var t in Vulnerable())yield return t;Need(game.ApplyHitToEnemy(actor,1,HitReaction.Launch,280)>0,"Public launch");
                foreach(var t in Await(w=>w.selected.native&&w.selected.motion=="DamageAirUp","05b-registered-launch",12))yield return t;
                foreach(var t in Await(w=>w.selected.native&&(w.selected.motion.StartsWith("DamageStandUp",StringComparison.Ordinal)||w.selected.motion.StartsWith("RollingStandUp",StringComparison.Ordinal)),"05c-registered-recovery",18))yield return t;
                if(kind.EndsWith("repairman"))foreach(var t in Repair())yield return t;
                var original=actor;foreach(var t in Leave())yield return t;foreach(var t in Enter())yield return t;
                Need(!ReferenceEquals(actor,original),"Fresh original actor on reentry");
                foreach(var t in Await(w=>w.selected.native&&w.selected.motion.EndsWith("Wait",StringComparison.Ordinal),"07-reentry",30))yield return t;
                foreach(var t in Leave())yield return t;
            }
        }
        bool Active(){return route=="Battle"?scene.BattleProductSessionActiveR1325:scene.PlainProductSessionActiveR1320;}
        IEnumerable Enter(){
            report.last=run+" ordinary entry";menuGame=(GameModel)Field(scene,"game");var libraries=(IDictionary)Field(scene,"libraries");menuLibrary=libraries[kind];
            foreach(var t in EnterRoute(scene,"altera",id))yield return t;
            double end=Time.realtimeSinceStartupAsDouble+75;
            while(!Active()){Need(Time.realtimeSinceStartupAsDouble<end,report.last+" private resource session");yield return null;}
            game=(GameModel)Field(scene,"game");Need(!ReferenceEquals(menuGame,game),"Actual private product model");
            var rooms=(List<RoomDefinition>)Field(game,"rooms");int i=rooms.FindIndex(r=>r.id==birth.room);Need(i>=0,"Original room");
            if(game.room!=i)Call(game,"EnterRoom",i);if(game.EncounterIndex!=birth.encounter)Call(game,"BeginEncounter",birth.encounter);
            actor=game.enemies.Find(e=>e.kind==kind&&e.sourceNPC==proof.npc&&e.encounterIndex==birth.encounter&&e.sourceKeyCode==birth.key&&Mathf.Abs(e.spawnX-birth.spawnX)<.02f);
            Need(actor!=null&&actor.active&&!actor.dormant&&actor.aiEnabled&&Mathf.Abs(actor.visualScale-1)<.0001f,"Original active AI, spawn "+birth.spawn+" at "+birth.spawnX);
            game.player.hp=game.player.maxHp=5000;Place(kind.EndsWith("high")||kind.EndsWith("repairman")?202:250);
            for(int k=0;k<2;k++)yield return null;
            view=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
            Need(view&&libraries[kind]!=menuLibrary,"Private carrier");report.steps.Add(run+" original spawn "+birth.spawn+" source X "+actor.spawnX+" encounter "+birth.encounter);
        }
        GroundSurface Support(float x,float y){return game.SurfaceBelow(x,y+.05f,false);}
        void Place(float distance){
            var room=game.CurrentRoom;var enc=room.encounters[game.EncounterIndex];
            float best=float.PositiveInfinity,chosen=0;GroundSurface floor=null;
            foreach(var s in room.surfaces){float left=Mathf.Max(1,enc.leftX+.2f,s.x1+.2f),right=Mathf.Min(room.width-1,enc.rightX-.2f,s.x2-.2f);
                if(right<left)continue;
                foreach(int side in new[]{-1,1}){float x=Mathf.Clamp(actor.x+side*distance,left,right),y=s.Y(x);var real=Support(x,y);
                    if(real==null||Mathf.Abs(real.Y(x)-y)>.08f)continue;
                    float score=Mathf.Abs(Mathf.Abs(x-actor.x)-distance)+Mathf.Abs(y-actor.y)*.8f;
                    if(score<best){best=score;chosen=x;floor=real;}
                }}
            Need(floor!=null,"Original encounter surface");game.player.x=chosen;game.player.y=floor.Y(chosen);
            game.player.groundLine=floor.id;game.player.vx=game.player.vy=0;game.player.invincible=180;
            Need(Support(chosen,game.player.y)==floor,"Original hero support");
            report.steps.Add("Hero floor "+floor.id+" X "+chosen+" Y "+game.player.y+" true separation "+Mathf.Abs(chosen-actor.x));
        }
        IEnumerable Vulnerable(){
            double end=Time.realtimeSinceStartupAsDouble+36;while(actor.state!=CombatState.Normal||actor.invincible>0||game.EnemyHasNpcInvincibility(actor)||game.EnemyHasSuperArmor(actor)){
                yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Natural vulnerable state "+actor.npcState);}
        }
        IEnumerable Repair(){
            Enemy cannon=null;float near=float.PositiveInfinity;
            foreach(var e in game.enemies){if(!e.active||e.dormant||e.hp<=0||(e.sourceNPC!="NUI_CANNON_BIG"&&e.sourceNPC!="NUI_CANNON_HEAVY_MACHINE_GUN"))continue;
                float distance=Vector2.Distance(new Vector2(actor.x,actor.y),new Vector2(e.x,e.y));if(distance<near){near=distance;cannon=e;}}
            Need(cannon!=null,"A real source repair target is present");
            if(near>92){var floor=Support(cannon.x,cannon.y);Need(floor!=null,"Original cannon support");
                float x=Mathf.Clamp(cannon.x+30,floor.x1+.15f,floor.x2-.15f);float y=floor.Y(x);Need(Support(x,y)==floor,"Controlled repair proximity on original support");
                report.steps.Add("Moved actual repairman from "+actor.x+","+actor.y+" to original cannon platform "+x+","+y+" for repair-range fixture");
                actor.x=x;actor.y=y;actor.groundLine=floor.id;actor.vx=actor.vy=0;actor.nativeDepth=cannon.nativeDepth;
            }
            // Keep the hero on the cannon's own original platform. A distant lower platform lures the worker away before its source decision.
            var support=Support(cannon.x,cannon.y);Need(support!=null,"Eligible cannon remains on original support");
            float heroX=Mathf.Clamp(cannon.x+9,support.x1+.15f,support.x2-.15f);
            game.player.x=heroX;game.player.y=support.Y(heroX);game.player.groundLine=support.id;game.player.vx=game.player.vy=0;game.player.invincible=180;
            Need(Support(heroX,game.player.y)==support&&Mathf.Abs(heroX-cannon.x)<15,"Hero and damaged target share original cannon platform");
            report.steps.Add("Hero and repair target share original platform "+support.id+" X "+heroX+" Y "+game.player.y);
            double end=Time.realtimeSinceStartupAsDouble+30;
            while(cannon.invincible>0||game.EnemyHasNpcInvincibility(cannon)){yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Original cannon accepts combat hit");}
            int damage=Mathf.CeilToInt(cannon.hp-cannon.maxHp*.4f);
            if(damage>0)Need(game.ApplyHitToEnemy(cannon,damage,HitReaction.None)>0,"Real damage on repair target");
            Need(cannon.hp>0&&cannon.hp<=cannon.maxHp*.5f,"Source repair HP threshold");
            var args=new object[]{actor,null};
            Need((bool)Call(game,"CanStartAlteraRepairmanV18",args)&&ReferenceEquals(args[1],cannon),"Unmodified source repair rule resolves the real damaged cannon");
            report.steps.Add("Actual source rule selected "+cannon.sourceNPC+" HP "+cannon.hp+"/"+cannon.maxHp+" at "+cannon.x+","+cannon.y);
            foreach(var t in Await(w=>w.selected.alias=="source-repair"&&w.selected.motion=="Repair","06-real-repair",30))yield return t;
            end=Time.realtimeSinceStartupAsDouble+18;AlteraRepairmanEventV18 outcome=null;
            while(outcome==null){outcome=game.alteraRepairmanEventsV18.Find(e=>e.type=="repair"&&e.npc==actor.sourceNPC&&e.target==cannon.sourceNPC&&e.after>e.before);
                if(outcome!=null)break;Need(Time.realtimeSinceStartupAsDouble<end,"Actual repair timer / target HP increase");yield return null;}
            report.steps.Add("Source repair "+outcome.source+" healed "+outcome.target+" "+outcome.before+" -> "+outcome.after);
        }
        IEnumerable Await(Func<AirshipSoldierFrameV40,bool> match,string tag,int seconds){
            report.last=run+" "+tag;double end=Time.realtimeSinceStartupAsDouble+seconds;
            for(;;){yield return null;var camera=(Camera)Field(scene,"worldCamera");var w=scene.AirshipSoldierFrameForV40(view);
                if(w!=null&&match(w)&&camera&&GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera),view.bounds)){
                    Observe(w,camera,run+"-"+tag);yield break;}
                Need(Time.realtimeSinceStartupAsDouble<end,report.last+" source state="+actor.npcState+" selected="+(w==null?"none":w.selected.alias)+" dx="+(actor.x-game.player.x)+" dy="+(actor.y-game.player.y));
            }
        }
        void Observe(AirshipSoldierFrameV40 w,Camera camera,string name){
            var s=w.selected;Need(ReferenceEquals(w.actor,actor)&&w.route==route&&w.carrier&&view.enabled&&!view.forceRenderingOff&&view.gameObject.activeInHierarchy&&view.sprite==s.sprite,"Actual original actor renderer");
            Need(view.flipX==(actor.facing!=1)&&Mathf.Abs(view.transform.localScale.x-s.scale)<.0001f,"Original facing / calibrated scale");
            var shot=new Shot{label=name,route=id,room=game.CurrentRoom.id,spawn=birth.spawn,alias=s.alias,motion=s.motion,page=s.page,exposure=s.index,raw=s.rawIndex,
                facing=w.facing,native=s.native,clock=w.clock,scale=s.scale,hp=actor.hp,foot=w.foot};
            if(s.native){var wp=Array.Find(proof.weapon,p=>p.motion==s.motion&&p.index==s.rawIndex);
                Need(wp!=null&&wp.page==s.page&&wp.trueTipDistance>27&&s.scale==1,"Full original WEAPON0 socket and endpoint");
                shot.grip=w.foot+new Vector2(wp.gripX*w.facing,wp.gripY);shot.tip=w.foot+new Vector2(wp.tipX*w.facing,wp.tipY);shot.weaponLength=wp.trueTipDistance;
                Need(Vector2.Distance(shot.grip,shot.tip)<=wp.trueTipDistance+.2f,"Projected tip bounded by original 3D length");
                var f=Array.Find(proof.feet,p=>p.motion==s.motion&&p.index==s.rawIndex);
                if(f!=null){shot.left=f.left;shot.right=f.right;shot.flight=f.flight;if(!f.flight)Need(Mathf.Abs(Mathf.Min(f.left,f.right))<.501f,"Correct source supporting sole");}
                if(s.motion.EndsWith("Wait",StringComparison.Ordinal))Need(Mathf.Abs(s.sprite.rect.height-proof.fullEquipmentHeight)<6,"Original standing height incl. equipment");
            }else Need(ReferenceEquals(s.sprite,w.carrier),"Approved original whole drawing by identity");
            var bar=((List<SpriteRenderer>)Field(scene,"enemyBars"))[game.enemies.IndexOf(actor)];shot.barVisible=bar.enabled;
            shot.barGap=bar.transform.position.y-view.transform.TransformPoint(new Vector3(0,s.top,0)).y;
            if(bar.enabled)Need(Mathf.Abs(shot.barGap-8)<1.1f,"Actual visible HP gap "+shot.barGap);
            if(game.AlteraRepairmanGaugeHiddenV18(actor))Need(!bar.enabled,"Source hidden gauge policy");
            var previous=camera.targetTexture;var focus=EventSystem.current?EventSystem.current.currentSelectedGameObject:null;
            shot.gpu=Capture(camera,game,actor,new List<SpriteRenderer>{view},output,name);
            Need(shot.gpu.count>0&&camera.targetTexture==previous&&view.enabled&&(!EventSystem.current||EventSystem.current.currentSelectedGameObject==focus),"GPU pixels and original focus restored");
            report.shots.Add(shot);
        }
        IEnumerable Leave(){
            double end=Time.realtimeSinceStartupAsDouble+25;while(scene.BossIntroductionActive){yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Original introduction releases menu");}
            var w=scene.AirshipSoldierFrameForV40(view);Sprite used=w!=null&&w.selected.native?w.selected.sprite:null;
            Press(scene,"dungeons");for(int i=0;i<8;i++)yield return null;
            var libs=(IDictionary)Field(scene,"libraries");Need(!Active()&&(string)Field(scene,"openMenu")=="dungeons"&&ReferenceEquals(Field(scene,"game"),menuGame)&&ReferenceEquals(libs[kind],menuLibrary),"Ordinary menu returns original model/library");
            Need(!used&&((IDictionary)Field(scene,"airshipSoldierBanksV40")).Count==0&&((IDictionary)Field(scene,"airshipSoldierWitnessV40")).Count==0,"Owned sprite and page caches released");
            var routes=(IList)Field(scene,"mapDungeons");int selected=(int)Field(scene,"mapSelected");Need(selected>=0&&(string)Field(routes[selected],"id")==id,"Selected ordinary route restored");
            report.steps.Add(run+" restored menu focus, original model, original library, released pages");
        }
    }
}
#endif

