using System;
namespace ElswordPixel {
 // Explicit generic DISPLAY retime only; no source FSM/bodySphere/AI parity claim.
 public static class Generic7NormalNativeRulesV28 {
  sealed class Binding {public readonly string kind,npc;public readonly float idle,walk;public Binding(string k,string n,float i,float w){kind=k;npc=n;idle=i;walk=w;}}
  sealed class Birth {public readonly int dungeon,staged,source;public readonly string route,stage,world,kind,npc;public Birth(int d,string r,string s,string w,int i,int x,string k,string n){dungeon=d;route=r;stage=s;world=w;staged=i;source=x;kind=k;npc=n;}}
  static readonly Binding[] table={
   new Binding("benders","NUI_BENDERS_NORMAL",1.6666666666666667f,0.6666666666666666f),
   new Binding("benders-thief","NUI_BENDERS_THIEF",1.3333333333333333f,0.8f),
   new Binding("chicken","NUI_CHICKEN",1.0f,0.5333333333333333f),
   new Binding("thief","NUI_THIEF_THIN_ZERO",1.0f,0.5333333333333333f),
   new Binding("william","NUI_PPORU_WILLIAM_NORMAL",0.6666666666666666f,0.6f),
   new Binding("william-cute","NUI_PPORU_WILLIAM_CUTE",1.3333333333333333f,0.6f),
   new Binding("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF",1.3333333333333333f,0.6f),
   new Binding("white-giant","NUI_PPORU_GIANT_WHITE",1.3333333333333333f,1.3333333333333333f),
   new Binding("ancient-phoru","NUI_ANCIENT_PPORU",1.6666666666666667f,2.1333333333333333f),
   new Binding("toto","NUI_THIEF_THIN_TOTO",1.3333333333333333f,1.3333333333333333f),
   new Binding("fairy-guardian","NUI_FAIRY_GUARDIAN",1.3333333333333333f,2.0f),
  };
  // Exact41 eligible births; original37 unchanged. Fairy4 is trueWait/Walk display-only; yaw0 material admission pending.
  static readonly Birth[] births={
   new Birth(0,"tree","tree-3","WI_RUBEN_EL_TREE_BOSS_STAGE",0,11,"benders","NUI_BENDERS_NORMAL"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",1,5,"benders-thief","NUI_BENDERS_THIEF"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",7,25,"benders-thief","NUI_BENDERS_THIEF"),
   new Birth(0,"tree","tree-1","WI_RUBEN_EL_TREE_STAGE1",0,3,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-1","WI_RUBEN_EL_TREE_STAGE1",1,5,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-1","WI_RUBEN_EL_TREE_STAGE1",2,6,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",1,4,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",2,6,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",3,10,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",7,23,"chicken","NUI_CHICKEN"),
   new Birth(2,"swamp","swamp-2","WI_RUBEN_SWAMP_STAGE2",0,3,"chicken","NUI_CHICKEN"),
   new Birth(2,"swamp","swamp-2","WI_RUBEN_SWAMP_STAGE2",2,9,"chicken","NUI_CHICKEN"),
   new Birth(2,"swamp","swamp-2","WI_RUBEN_SWAMP_STAGE2",4,21,"chicken","NUI_CHICKEN"),
   new Birth(0,"tree","tree-1","WI_RUBEN_EL_TREE_STAGE1",3,8,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree","tree-1","WI_RUBEN_EL_TREE_STAGE1",4,10,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree","tree-1","WI_RUBEN_EL_TREE_STAGE1",5,11,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",4,13,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",5,21,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",6,16,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(0,"tree","tree-2","WI_RUBEN_EL_TREE_STAGE2",8,24,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",0,3,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",3,11,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",5,37,"thief","NUI_THIEF_THIN_ZERO"),
   new Birth(1,"ruins","ruins-3","WI_RUBEN_RUIN_OF_ELF_BOSS_STAGE",0,5,"william","NUI_PPORU_WILLIAM_NORMAL"),
   new Birth(1,"ruins","ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",0,5,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins","ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",2,10,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins","ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",0,20,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins","ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",2,32,"william-cute","NUI_PPORU_WILLIAM_CUTE"),
   new Birth(1,"ruins","ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",1,6,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins","ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",3,13,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins","ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",1,21,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(1,"ruins","ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",3,33,"william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF"),
   new Birth(0,"tree","tree-3","WI_RUBEN_EL_TREE_BOSS_STAGE",2,10,"white-giant","NUI_PPORU_GIANT_WHITE"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",2,9,"toto","NUI_THIEF_THIN_TOTO"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",4,14,"toto","NUI_THIEF_THIN_TOTO"),
   new Birth(2,"swamp","swamp-1","WI_RUBEN_SWAMP_STAGE1",6,31,"toto","NUI_THIEF_THIN_TOTO"),
   new Birth(2,"swamp","swamp-3","WI_RUBEN_SWAMP_BOSS_STAGE",0,9,"ancient-phoru","NUI_ANCIENT_PPORU"),
   new Birth(1,"ruins","ruins-1","WI_RUBEN_RUIN_OF_ELF_STAGE1",4,27,"fairy-guardian","NUI_FAIRY_GUARDIAN"),
   new Birth(1,"ruins","ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",6,35,"fairy-guardian","NUI_FAIRY_GUARDIAN"),
   new Birth(1,"ruins","ruins-2","WI_RUBEN_RUIN_OF_ELF_STAGE2",7,47,"fairy-guardian","NUI_FAIRY_GUARDIAN"),
   new Birth(2,"swamp","swamp-2","WI_RUBEN_SWAMP_STAGE2",6,29,"fairy-guardian","NUI_FAIRY_GUARDIAN"),
  };
  static Binding Find(string kind,string npc){foreach(var b in table)if(b.kind==kind&&b.npc==npc)return b;return null;}
  static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
  public static bool Supported(string kind,string npc){return Find(kind,npc)!=null;}
  // Fixed authored view applies only after the same reviewed alias and birth gates succeed.
  public static int AuthoredFacing(string kind,string npc){return !Supported(kind,npc)?0:(kind=="toto"||kind=="fairy-guardian")?-1:1;}
  public static bool OriginalBirth(bool baseContext,int dungeon,string route,string sourceGraph,string stage,string sourceWorld,int stagedIndex,int sourceSpawnIndex,string kind,string npc){
   if(!baseContext||!string.IsNullOrEmpty(sourceGraph)||!Supported(kind,npc))return false;
   foreach(var b in births)if(b.dungeon==dungeon&&b.route==route&&b.stage==stage&&b.world==sourceWorld&&b.staged==stagedIndex&&b.source==sourceSpawnIndex&&b.kind==kind&&b.npc==npc)return true;
   return false;
  }
  public static bool Eligible(bool originalBirth,bool hasCurrentProfile,string kind,string npc,bool sourceVisible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string npcState,string previous){
   return originalBirth&&!hasCurrentProfile&&Supported(kind,npc)&&sourceVisible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&hp>0&&combat==0&&string.IsNullOrEmpty(npcState)&&(previous=="idle"||previous=="walk");
  }
  public static string SourceAlias(string kind,string npc,string modelClip){return Supported(kind,npc)&&(modelClip=="idle"||modelClip=="walk")?"source-"+modelClip:null;}
  public static bool AliasFor(string kind,string npc,string modelClip,string actualClip){string a=SourceAlias(kind,npc,modelClip);return a!=null&&a==actualClip;}
  public static float SourceDuration(string kind,string npc,string modelClip){var b=Find(kind,npc);return b==null?float.NaN:modelClip=="idle"?b.idle:modelClip=="walk"?b.walk:float.NaN;}
  static float Repeat(float t,float span){return Math.Max(0,Math.Min(span,t-(float)Math.Floor(t/span)*span));}
  public static string Select(bool originalBirth,bool hasCurrentProfile,string kind,string npc,bool sourceVisible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string npcState,string previous,float actorClock,float previousClock,Func<string,float> duration,out float clock){
   clock=previousClock;
   if(!Eligible(originalBirth,hasCurrentProfile,kind,npc,sourceVisible,active,dormant,passive,aiEnabled,hp,combat,npcState,previous))return previous;
   if(!Finite(actorClock)||actorClock<0)throw new InvalidOperationException("Invalid generic normal actor clock");
   string alias=SourceAlias(kind,npc,previous);float oldSpan=previous=="idle"?1.4f:.48f,span=SourceDuration(kind,npc,previous),actualOld=duration(previous),actualSource=duration(alias);
   if(!Finite(actualOld)||!Finite(actualSource)||Math.Abs(actualOld-oldSpan)>.000001f||Math.Abs(actualSource-span)>.000001f)throw new InvalidOperationException("Missing or retimed reviewed generic normal metadata");
   clock=span*Repeat(actorClock,oldSpan)/oldSpan;return alias;
  }
  public static bool TryUiAnchor(bool normalEligible,string kind,string npc,string modelClip,string actualClip,float centerX,float visibleTopY,float fullWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(!normalEligible||!AliasFor(kind,npc,modelClip,actualClip))return false;
   if(!Finite(centerX)||!Finite(visibleTopY)||!Finite(fullWidth)||fullWidth<=0||!Finite(hp)||!Finite(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid generic normal visible-alpha UI");
   bx=centerX-fullWidth*(1-hp/maxHp)/2;by=visibleTopY+8;dx=centerX;dy=visibleTopY+20;return true;
  }
 }
}
