using System;
namespace ElswordPixel {
 // Host launched/fall DISPLAY only. No source FSM, phase timer, model mutation, or bodySphere parity.
 public static class StartedAir26NativeRulesV28 {
  public const string AdmissionPrefix="Root-approved-Air26-sourceB-v1|";
  public sealed class Entry {
   public readonly string kind,npc,group,alias,modelClip,sourceState,animation,resourcePrefix;public readonly float duration,libraryScale;public readonly int samples;public readonly bool held;
   public Entry(string k,string n,string g,string a,string m,string s,string animationName,float d,int count,bool hold,float scale){kind=k;npc=n;group=g;alias=a;modelClip=m;sourceState=s;animation=animationName;duration=d;samples=count;held=hold;libraryScale=scale;resourcePrefix="Pixel/feedback-v25/started-air26-v28/"+k+"/air-atlas-";}
  }
  static readonly Entry[] entries={
   new Entry("benders","NUI_BENDERS_NORMAL","generic7","source-air-up","launched","BENDERS_NORMAL_DAMAGE_AIR_UP","DamageAirUp",1f,25,false,1f),
   new Entry("benders","NUI_BENDERS_NORMAL","generic7","source-air-fall","fall","BENDERS_NORMAL_DAMAGE_AIR_FALL","DamageAirFall",0.20000000000000001f,6,false,1f),
   new Entry("benders-thief","NUI_BENDERS_THIEF","generic7","source-air-up","launched","BENDERS_THIEF_DAMAGE_AIR_UP","DamageAirUp",1f,1,true,1f),
   new Entry("benders-thief","NUI_BENDERS_THIEF","generic7","source-air-fall","fall","BENDERS_THIEF_DAMAGE_AIR_FALL","DamageAirFall",1f,1,true,1f),
   new Entry("chicken","NUI_CHICKEN","generic7","source-air-up","launched","CHICKEN_DAMAGE_AIR_UP","DamageAirUp",0.20000000000000001f,6,false,1f),
   new Entry("chicken","NUI_CHICKEN","generic7","source-air-fall","fall","CHICKEN_DAMAGE_AIR_FALL","DamageAirFall",0.20000000000000001f,6,false,1f),
   new Entry("thief","NUI_THIEF_THIN_ZERO","generic7","source-air-up","launched","THIEF_THIN_ZERO_DAMAGE_AIR_UP","DamageAirUp",1f,1,true,1f),
   new Entry("thief","NUI_THIEF_THIN_ZERO","generic7","source-air-fall","fall","THIEF_THIN_ZERO_DAMAGE_AIR_FALL","DamageAirFall",1f,1,true,1f),
   new Entry("william","NUI_PPORU_WILLIAM_NORMAL","generic7","source-air-up","launched","WILLIAM_PPORU_DAMAGE_AIR_UP","DamageAirUp",1f,1,true,1f),
   new Entry("william","NUI_PPORU_WILLIAM_NORMAL","generic7","source-air-fall","fall","WILLIAM_PPORU_DAMAGE_AIR_FALL","DamageAirFall",1f,25,false,1f),
   new Entry("william-cute","NUI_PPORU_WILLIAM_CUTE","generic7","source-air-up","launched","PPORU_WILLIAM_CUTE_DAMAGE_AIR_UP","DamageAirUp",0.20000000000000001f,6,false,1f),
   new Entry("william-cute","NUI_PPORU_WILLIAM_CUTE","generic7","source-air-fall","fall","PPORU_WILLIAM_CUTE_DAMAGE_AIR_FALL","DamageAirFall",0.20000000000000001f,6,false,1f),
   new Entry("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF","generic7","source-air-up","launched","PPORU_WILLIAM_CUTE_THIEF_DAMAGE_AIR_UP","DamageAirUp",0.20000000000000001f,6,false,1f),
   new Entry("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF","generic7","source-air-fall","fall","PPORU_WILLIAM_CUTE_THIEF_DAMAGE_AIR_FALL","DamageAirFall",0.20000000000000001f,6,false,1f),
   new Entry("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY","alpha3","source-air-up","launched","ANGER_FOREST_FAIRY_DAMAGE_AIR_UP","DamageAirUp",0.16666666666666666f,5,false,1f),
   new Entry("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY","alpha3","source-air-fall","fall","ANGER_FOREST_FAIRY_DAMAGE_AIR_FALL","DamageAirFall",0.16666666666666666f,1,true,1f),
   new Entry("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT","alpha3","source-air-up","launched","ANGER_FOREST_KNIGHT_DAMAGE_AIR_UP","DamageAirUp",0.26666666666666666f,8,false,1f),
   new Entry("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT","alpha3","source-air-fall","fall","ANGER_FOREST_KNIGHT_DAMAGE_AIR_FALL","DamageAirFall",0.26666666666666666f,8,false,1f),
   new Entry("curse-wisp","NUI_CURSE_WISP","alpha3","source-air-up","launched","CURSE_WISP_DAMAGE_AIR_UP","DamageAirUp",0.26666666666666666f,8,false,1f),
   new Entry("curse-wisp","NUI_CURSE_WISP","alpha3","source-air-fall","fall","CURSE_WISP_DAMAGE_AIR_FALL","DamageAirFall",0.26666666666666666f,8,false,1f),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","next3","source-air-up","launched","BENDERS_UPGRADE_NORMAL_DAMAGE_AIR_UP","DamageAirUp",1f,25,false,1f),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","next3","source-air-fall","fall","BENDERS_UPGRADE_NORMAL_DAMAGE_AIR_FALL","DamageAirFall",0.20000000000000001f,6,false,1f),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","next3","source-air-up","launched","THIEF_FAT_BOSS_DAMAGE_AIR_UP","DamageAirUp",1f,1,true,1.3999999999999999f),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","next3","source-air-fall","fall","THIEF_FAT_BOSS_DAMAGE_AIR_FALL","DamageAirFall",1f,1,true,1.3999999999999999f),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","next3","source-air-up","launched","FOREST_DARK_MAGICIAN_DAMAGE_AIR_UP","DamageAirUp",0.26666666666666666f,8,false,1f),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","next3","source-air-fall","fall","FOREST_DARK_MAGICIAN_DAMAGE_AIR_FALL","DamageAirFall",0.26666666666666666f,8,false,1f),
  };
  sealed class Birth {public readonly int dungeon,room,staged,source;public readonly string route,stage,world,kind,npc;public Birth(int d,string r,int rm,string st,string w,int i,int s,string k,string n){dungeon=d;route=r;room=rm;stage=st;world=w;staged=i;source=s;kind=k;npc=n;}}
  static readonly Birth[] births={
   new Birth(0,"tree",0,"tree-1","WI_RUBEN_EL_TREE_STAGE1",0,3,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",0,"tree-1","WI_RUBEN_EL_TREE_STAGE1",1,5,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",0,"tree-1","WI_RUBEN_EL_TREE_STAGE1",2,6,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",0,"tree-1","WI_RUBEN_EL_TREE_STAGE1",3,8,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",0,"tree-1","WI_RUBEN_EL_TREE_STAGE1",4,10,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",0,"tree-1","WI_RUBEN_EL_TREE_STAGE1",5,11,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",1,4,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",2,6,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",3,10,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",4,13,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",5,21,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",6,16,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",7,23,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree",1,"tree-2","WI_RUBEN_EL_TREE_STAGE2",8,24,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree",2,"tree-3","WI_RUBEN_EL_TREE_BOSS_STAGE",0,11,"benders","NUI_BENDERS_NORMAL"),
   new Birth(1,"ruins",0,"ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",0,5,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins",0,"ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",1,6,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins",0,"ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",2,10,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins",0,"ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",3,13,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins",1,"ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",0,20,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins",1,"ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",1,21,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins",1,"ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",2,32,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins",1,"ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",3,33,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins",2,"ruins-3","WI_RUBEN_RUIN_OF_ELF_BOSS_STAGE",0,5,"william","NUI_PPORU_WILLIAM_NORMAL"),
   new Birth(2,"swamp",0,"swamp-1","WI_RUBEN_SWAMP_STAGE1",0,3,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(2,"swamp",0,"swamp-1","WI_RUBEN_SWAMP_STAGE1",1,5,"benders-thief","NUI_BENDERS_THIEF"),
   new Birth(2,"swamp",0,"swamp-1","WI_RUBEN_SWAMP_STAGE1",3,11,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(2,"swamp",0,"swamp-1","WI_RUBEN_SWAMP_STAGE1",5,37,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(2,"swamp",0,"swamp-1","WI_RUBEN_SWAMP_STAGE1",7,25,"benders-thief","NUI_BENDERS_THIEF"),
   new Birth(2,"swamp",1,"swamp-2","WI_RUBEN_SWAMP_STAGE2",0,3,"chicken","NUI_CHICKEN"),
   new Birth(2,"swamp",1,"swamp-2","WI_RUBEN_SWAMP_STAGE2",2,9,"chicken","NUI_CHICKEN"),
   new Birth(2,"swamp",1,"swamp-2","WI_RUBEN_SWAMP_STAGE2",4,21,"chicken","NUI_CHICKEN"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",0,9,"forest-dark-magician","NUI_FOREST_DARK_MAGICIAN"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",1,4,"forest-dark-magician","NUI_FOREST_DARK_MAGICIAN"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",2,19,"forest-dark-magician","NUI_FOREST_DARK_MAGICIAN"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",3,34,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",4,37,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",5,26,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",6,31,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(3,"dark-forest",0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",7,41,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",0,58,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",1,59,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",2,20,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",3,23,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",4,29,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",5,38,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",6,45,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",7,55,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",8,56,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",9,6,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(3,"dark-forest",1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",10,3,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(4,"banthus-cave",0,"cave-1","WI_ELDER_BENDERS_CAVE_STAGE1",9,29,"thief-fat-boss","NUI_THIEF_FAT_BOSS"),
   new Birth(4,"banthus-cave",1,"cave-2","WI_ELDER_BENDERS_CAVE_STAGE1_1",1,26,"thief-fat-boss","NUI_THIEF_FAT_BOSS"),
   new Birth(4,"banthus-cave",1,"cave-2","WI_ELDER_BENDERS_CAVE_STAGE1_1",10,69,"thief-fat-boss","NUI_THIEF_FAT_BOSS"),
   new Birth(4,"banthus-cave",1,"cave-2","WI_ELDER_BENDERS_CAVE_STAGE1_1",11,59,"thief-fat-boss","NUI_THIEF_FAT_BOSS"),
   new Birth(4,"banthus-cave",1,"cave-2","WI_ELDER_BENDERS_CAVE_STAGE1_1",17,43,"thief-fat-boss","NUI_THIEF_FAT_BOSS"),
   new Birth(4,"banthus-cave",2,"cave-3","WI_ELDER_BENDERS_STAGE_BOSS",2,14,"benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL"),
  };
  public static Entry Find(string kind,string npc,string modelClip){foreach(var e in entries)if(e.kind==kind&&e.npc==npc&&e.modelClip==modelClip)return e;return null;}
  public static Entry FindAlias(string kind,string npc,string alias){foreach(var e in entries)if(e.kind==kind&&e.npc==npc&&e.alias==alias)return e;return null;}
  public static bool Finite(float n){return !float.IsNaN(n)&&!float.IsInfinity(n);}
  public static bool Context(Entry e,int dungeon,string graph,bool baseContext,string content){
   if(e==null)return false;
   if(e.group=="generic7")return dungeon>=0&&dungeon<=2&&baseContext&&string.IsNullOrEmpty(graph)&&string.IsNullOrEmpty(content);
   if(e.kind=="benders-upgrade-normal"||e.kind=="thief-fat-boss")return dungeon==4&&graph==Next3NormalNativeRulesV28.Cave&&!baseContext&&content==Next3NormalNativeRulesV28.Cave;
   return dungeon==3&&graph==Alpha4NormalNativeRulesV28.Forest&&baseContext&&string.IsNullOrEmpty(content);
  }
  public static bool OriginalBirth(Entry e,int dungeon,string graph,bool baseContext,string content,string route,int room,string stage,string world,int stagedIndex,int sourceIndex){
   if(!Context(e,dungeon,graph,baseContext,content))return false;
   foreach(var b in births)if(b.dungeon==dungeon&&b.route==route&&b.room==room&&b.stage==stage&&b.world==world&&b.staged==stagedIndex&&b.source==sourceIndex&&b.kind==e.kind&&b.npc==e.npc)return true;return false;
  }
  public static bool ProfileContext(Entry e,bool profilePresent,bool exactIdentity,bool hasAirReaction,string sourceState){return e!=null&&(e.group=="generic7"?!profilePresent&&string.IsNullOrEmpty(sourceState):profilePresent&&exactIdentity&&!hasAirReaction);}
  public static bool Eligible(Entry e,bool scope,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,bool knockdownOnLanding,string modelClip,string previous){return e!=null&&scope&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&hp>0&&combat==1&&knockdownOnLanding&&modelClip==e.modelClip&&previous==modelClip;}
  public static bool Resource(Entry e,string path){return e!=null&&path!=null&&path.Length==e.resourcePrefix.Length+2&&path.StartsWith(e.resourcePrefix,StringComparison.Ordinal)&&path[path.Length-2]>='0'&&path[path.Length-2]<='9'&&path[path.Length-1]>='0'&&path[path.Length-1]<='9';}
  public static string AdmissionEvidence(Entry e){return e==null?null:AdmissionPrefix+e.kind+"|"+e.alias;}
  public static bool ValidateClip(Entry e,string id,float libraryScale,string resource,float fps,float duration,bool loop,float rootDistance,int samples,int hits,bool coherentFrames,string commandEvidence){return e!=null&&id==e.alias&&libraryScale==e.libraryScale&&Resource(e,resource)&&fps==24&&Finite(duration)&&Math.Abs(duration-e.duration)<=.000001f&&!loop&&rootDistance==0&&samples==e.samples&&hits==0&&coherentFrames&&commandEvidence==AdmissionEvidence(e);}
  public static string Select(Entry e,bool scope,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,bool knockdownOnLanding,string modelClip,string previous,float actorClock,float previousClock,Func<bool> metadataReady,out float clock){
   clock=previousClock;if(!Eligible(e,scope,visible,active,dormant,passive,aiEnabled,hp,combat,knockdownOnLanding,modelClip,previous)||!Finite(actorClock)||actorClock<0||metadataReady==null||!metadataReady())return previous;
   // Preserve original actor seconds at launch-to-fall boundary. Pose owns the nonloop end clamp, including held1.
   clock=actorClock;return e.alias;
  }
  public static float EffectiveScale(Entry e,bool qualified,string actualClip,float previous,float visualScale){if(e==null||!qualified||e.kind!="thief-fat-boss"||actualClip!=e.alias||!Finite(visualScale)||visualScale<=0)return previous;return visualScale;}
  public static bool TryUi(Entry e,bool qualified,string actualClip,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(e==null||!qualified||actualClip!=e.alias||!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||maxHp<=0||hp<=0||hp>maxHp)return false;
   bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;
  }
 }
}
