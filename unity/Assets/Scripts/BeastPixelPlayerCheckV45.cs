#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;
namespace ElswordPixel {
 public sealed class BeastPixelPlayerCheckV45:MonoBehaviour {
  [Serializable] class Row {public string kind,source,room,alias,motion,page;public int pose,spawn;public float hp,height;public PixelProof pixels;}
  [Serializable] class Report {public bool passed;public string error,last;public List<Row> shots=new List<Row>();}
  Report report=new Report();string output,mode;DemoScene scene;GameModel game;Enemy actor;SpriteRenderer view;Camera camera;
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
  static void Init(){
   var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,"--beast-pixel-v45");if(i<0)return;
   try{int j=Array.IndexOf(a,"--beast-output-v45");Need(i+1<a.Length&&j>=0&&j+1<a.Length,"Beast arguments");
    string dir=Path.GetFullPath(a[j+1]);Need(dir.StartsWith(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex","runtime-drafts")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&!Directory.Exists(dir),"Fresh evidence directory");
    Directory.CreateDirectory(dir);var b=new GameObject("Quadruped source artwork review").AddComponent<BeastPixelPlayerCheckV45>();b.output=dir;b.mode=a[i+1];
   }catch(Exception e){Debug.LogError(e);Application.Quit(2);}
  }
  IEnumerator Start(){Application.runInBackground=true;var it=Flow();double end=Time.realtimeSinceStartupAsDouble+240;Application.logMessageReceived+=Log;
   try{for(;;){bool more=false;object next=null;try{Need(String.IsNullOrEmpty(report.error),report.error);Need(Time.realtimeSinceStartupAsDouble<end,"Deadline "+report.last);more=it.MoveNext();if(more)next=it.Current;}catch(Exception e){report.error=e.ToString();}if(!more||!String.IsNullOrEmpty(report.error))break;yield return next;}}
   finally{Application.logMessageReceived-=Log;}
   report.passed=String.IsNullOrEmpty(report.error);File.WriteAllText(Path.Combine(output,"beast-player.json"),JsonUtility.ToJson(report,true));Application.Quit(report.passed?0:2);
  }
  void Log(string msg,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||msg.Contains("BEAST_PIXEL_FALLBACK_V45"))report.error=msg+" "+stack;}
  bool Visible(){return actor!=null&&view&&scene.BeastPixelFrameV45(view)!=null&&GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera),view.bounds);}
  void Near(){game.player.x=Mathf.Clamp(actor.x+25,1,game.CurrentRoom.width-1);var f=game.SurfaceBelow(game.player.x,actor.y+10,false);
   if(f!=null){game.player.y=f.Y(game.player.x);game.player.groundLine=f.id;game.player.vx=game.player.vy=0;}game.player.invincible=240;}
  void Shot(string label){var w=scene.BeastPixelFrameV45(view);var s=w.selected;
   Need(w.actor==actor&&s.native&&s.rawIndex>=0&&s.scale==1&&s.sprite==view.sprite&&s.page.StartsWith("Pixel/feedback-v45/"+mode+"/")&&view.flipX==(actor.facing!=1),"Original source actor and registered point sprite");
   var row=new Row{kind=mode,source=actor.sourceNPC,room=game.CurrentRoom.id,spawn=actor.sourceKeyCode,alias=s.alias,motion=s.motion,page=s.page,pose=s.rawIndex,hp=actor.hp,height=s.sprite.rect.height,pixels=Capture(camera,game,actor,new List<SpriteRenderer>{view},output,label)};
   Need(row.pixels.count>0,"Monster actually contributed pixels "+label);report.shots.Add(row);}
  IEnumerable CheckDrawnHits(){
   string region,route;string[] kinds;
   switch(mode){
    case "rx-castle":region="elder";route="wally-castle-center";kinds=new[]{"soldier-crossbow","soldier-benders","wally-8th"};break;
    case "rx-ruins":region="ruben";route="ruins";kinds=new[]{"william-cute","william-thief"};break;
    case "rx-forest":region="elder";route="dark-forest";kinds=new[]{"curse-wisp","anger-forest-knight"};break;
    default:throw new InvalidOperationException(mode);
   }
   foreach(var s in EnterRoute(scene,region,route))yield return s;
   game=(GameModel)Field(scene,"game");camera=(Camera)Field(scene,"worldCamera");var rooms=(List<RoomDefinition>)Field(game,"rooms");
   foreach(var kind in kinds){
    report.last="original birth "+kind;
    int room=rooms.FindIndex(r=>r.enemies.Exists(e=>e.kind==kind));Need(room>=0,kind+" in "+route);
    var spawn=rooms[room].enemies.Find(e=>e.kind==kind);
    if(game.room!=room)Call(game,"EnterRoom",room);if(game.EncounterIndex!=spawn.encounterIndex)Call(game,"BeginEncounter",spawn.encounterIndex);
    Call(scene,"RebuildRoom");for(int j=0;j<5;j++)yield return null;
    actor=game.enemies.Find(e=>e.kind==kind&&e.encounterIndex==spawn.encounterIndex);Need(actor!=null,"Native birth "+kind);
    game.player.hp=game.player.maxHp=5000;
    foreach(int code in new[]{1,3}){
     bool rear=code==3;report.last="wait hit "+kind+"/"+code;double deadline=Time.realtimeSinceStartupAsDouble+65;
     for(;;){Near();yield return null;var vs=(List<SpriteRenderer>)Field(scene,"enemyViews");int i=game.enemies.IndexOf(actor);
      Need(Time.realtimeSinceStartupAsDouble<deadline,report.last+" "+actor.npcState+"/"+actor.clip+" hp="+actor.hp+" active="+actor.active+" dormant="+actor.dormant+" armor="+game.EnemyHasSuperArmor(actor)+" x,y="+actor.x+","+actor.y+" view="+(view?view.bounds.ToString():"none"));
      if(i<0||i>=vs.Count)continue;
      if(kind=="soldier-crossbow"&&actor.npcState.StartsWith("SOLDIER_CROSSBOW_SIEGE_",StringComparison.Ordinal)){
       Enemy bunker=null;float distance=float.PositiveInfinity;
       foreach(var e in game.enemies)if(e.active&&e.hp>0&&e.sourceNPC=="NUI_BUNKER"&&e.encounterIndex==actor.encounterIndex){
        float dx=e.x-actor.x,dy=e.y-actor.y,dist=dx*dx+dy*dy;if(dist<distance){distance=dist;bunker=e;}}
       if(bunker!=null)game.ApplyHitToEnemy(bunker,Mathf.CeilToInt(bunker.hp)+1,HitReaction.Hurt);
       continue;
      }
      view=vs[i];if(actor.active&&!actor.dormant&&actor.state==CombatState.Normal&&actor.invincible<=0&&!game.EnemyHasSuperArmor(actor)&&!game.EnemyHasNpcInvincibility(actor)&&view.enabled&&view.gameObject.activeInHierarchy&&GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera),view.bounds))break;
     }
     int applied=(int)Call(game,"ApplyHitToEnemyFromXR1417",actor,2,code==1?HitReaction.Hurt:HitReaction.Knockdown,240f,actor.x+actor.facing*(rear?-32:32),code);
     Need(applied>0&&OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out _,out _),"Real reaction hit "+kind);OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out var expected,out _);
     report.last="select "+kind+" "+expected.motion;
     double until=Time.realtimeSinceStartupAsDouble+7;string alias="original-rx45-"+expected.motion;
     for(;;){yield return null;Need(Time.realtimeSinceStartupAsDouble<until,"Expected "+alias+", displayed "+(view.sprite?view.sprite.name:"none"));
      if(view.sprite&&view.sprite.name.Contains("/"+alias+"/")&&!view.forceRenderingOff)break;
     }
     var row=new Row{kind=kind,source=actor.sourceNPC,room=game.CurrentRoom.id,alias=alias,motion=expected.motion,page=view.sprite.texture.name,spawn=actor.sourceKeyCode,hp=actor.hp,height=view.sprite.rect.height,pixels=Capture(camera,game,actor,new List<SpriteRenderer>{view},output,kind+"-"+expected.motion)};
     Need(row.pixels.count>0,"Real body pixels");report.shots.Add(row);
    }
   }
   Press(scene,"dungeons");for(int i=0;i<8;i++)yield return null;Need((string)Field(scene,"openMenu")=="dungeons","Menu handback");
  }
  IEnumerator Flow(){while(!(scene=FindObjectOfType<DemoScene>())||Field(scene,"game")==null||(string)Field(scene,"openMenu")!="dungeons")yield return null;
   if(mode.StartsWith("rx-",StringComparison.Ordinal)){foreach(var x in CheckDrawnHits())yield return x;yield break;}
   Need(mode=="armadillo"||mode=="fire-komodo"||mode=="armadillo-earth"||mode=="bat-b-fire","Known actor");bool earth=mode=="armadillo-earth";string route=earth?"altera-below-tunnel":mode=="bat-b-fire"?"besma-dragon-nest":"besma-lake-night";
   report.last="enter "+route;foreach(var t in EnterRoute(scene,earth?"altera":"bethma",route))yield return t;
   for(;;){game=(GameModel)Field(scene,"game");if(game!=null&&(!earth||scene.BelowProductRouteActiveR1130)&&((List<RoomDefinition>)Field(game,"rooms")).Exists(r=>r.enemies.Exists(e=>e.kind==mode)))break;yield return null;}
   var rooms=(List<RoomDefinition>)Field(game,"rooms");int room=rooms.FindIndex(r=>r.enemies.Exists(e=>e.kind==mode));var spawn=rooms[room].enemies.Find(e=>e.kind==mode);
   if(game.room!=room)Call(game,"EnterRoom",room);if(game.EncounterIndex!=spawn.encounterIndex)Call(game,"BeginEncounter",spawn.encounterIndex);
   actor=game.enemies.Find(e=>e.kind==mode&&e.encounterIndex==spawn.encounterIndex);Need(actor!=null&&actor.active&&!actor.dormant,"Real encounter actor");game.player.hp=game.player.maxHp=5000;
   report.last="visible "+mode;camera=(Camera)Field(scene,"worldCamera");
   for(;;){Near();yield return null;var vs=(List<SpriteRenderer>)Field(scene,"enemyViews");int i=game.enemies.IndexOf(actor);if(i>=0&&i<vs.Count)view=vs[i];if(Visible())break;}
   Shot("ordinary");report.last="vulnerable "+actor.npcState;
   while(actor.state!=CombatState.Normal||game.EnemyHasNpcInvincibility(actor)||game.EnemyHasSuperArmor(actor)||actor.invincible>0){Near();yield return null;}
   Need(game.ApplyHitToEnemy(actor,1,HitReaction.Hurt)>0,"True gameplay damage");report.last="hurt";
   for(;;){yield return null;if(Visible()&&scene.BeastPixelFrameV45(view).selected.motion.StartsWith("Damage",StringComparison.Ordinal))break;}
   Shot("hurt");report.last="return";Press(scene,"dungeons");for(int i=0;i<8;i++)yield return null;
   Need((string)Field(scene,"openMenu")=="dungeons","Ordinary menu handback");
  }
 }
}
#endif
