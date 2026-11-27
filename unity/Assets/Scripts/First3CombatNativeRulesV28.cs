using System;
namespace ElswordPixel {
 // Display only. Original GameModel source states, physics, hits, FX and SFX are read only.
 public static class First3CombatNativeRulesV28 {
  public const string Dungeon="Dungeon_Elder_Benders_Cave_Normal";
  sealed class Binding {
   public readonly string kind,npc,model,state,alias,animation;public readonly float span,speed,period;public readonly int combat;public readonly bool attack,loop;
   public Binding(string k,string n,string m,string s,string a,string anim,float d,float sp,float p,int c,bool at,bool l){kind=k;npc=n;model=m;state=s;alias=a;animation=anim;span=d;speed=sp;period=p;combat=c;attack=at;loop=l;}
  }
  // Exact original source IDs; reaction state is a source-reference, not fabricated npcState.
  static readonly Binding[] table={
   new Binding("thief-thin","NUI_THIEF_THIN","hurt","THIEF_THIN_DAMAGE_FRONT","source-hurt","DamageBig",1.166666666666667f,1f,0f,2,false,false),
   new Binding("thief-thin","NUI_THIEF_THIN","down","THIEF_THIN_DAMAGE_DOWN_FRONT","source-down","DamageDownFront",1.4f,1f,0f,3,false,false),
   new Binding("thief-thin","NUI_THIEF_THIN","death","THIEF_THIN_DAMAGE_DOWN_FRONT","source-death","DamageDownFront",1.4f,1f,0f,5,false,false),
   new Binding("thief-thin","NUI_THIEF_THIN","get-up","THIEF_THIN_STAND_UP_FRONT","source-get-up","DamageStandUpFront",1.333333333333333f,1f,0f,4,false,false),
   new Binding("thief-fat","NUI_THIEF_FAT","hurt","THIEF_FAT_DAMAGE_SMALL","source-hurt","DamageSmall",0.8333333333333334f,1f,0f,2,false,false),
   new Binding("thief-fat","NUI_THIEF_FAT","down","THIEF_FAT_DAMAGE_DOWN_FRONT","source-down","DamageDownFront",1.4f,1f,0f,3,false,false),
   new Binding("thief-fat","NUI_THIEF_FAT","death","THIEF_FAT_DAMAGE_DOWN_FRONT","source-death","DamageDownFront",1.4f,1f,0f,5,false,false),
   new Binding("thief-fat","NUI_THIEF_FAT","get-up","THIEF_FAT_STAND_UP_FRONT","source-get-up","DamageStandUpFront",1.3f,1f,0f,4,false,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","hurt","PPORU_GIANT_RED_DAMAGE_FRONT","source-hurt","DamageBig",1.433333333333333f,1f,0f,2,false,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","down","PPORU_GIANT_RED_DAMAGE_DOWN_FRONT","source-down","DamageDownFront",1.566666666666667f,1f,0f,3,false,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","death","PPORU_GIANT_RED_DAMAGE_DOWN_FRONT","source-death","DamageDownFront",1.566666666666667f,1f,0f,5,false,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","get-up","PPORU_GIANT_RED_STAND_UP_FRONT","source-get-up","DamageStandUpFront",1.8f,1f,0f,4,false,false),
   new Binding("thief-thin","NUI_THIEF_THIN","attack-ready","THIEF_THIN_ATTACK_READY","source-attack-ready","AttackA_Ready",2.7f,1.25f,0f,0,true,false),
   new Binding("thief-thin","NUI_THIEF_THIN","attack-a","THIEF_THIN_ATTACK_A","source-attack-a","AttackA_Fire",0.9666666666666667f,1f,0f,0,true,false),
   new Binding("thief-fat","NUI_THIEF_FAT","attack-a-ready","THIEF_FAT_ATTACK_A_READY","source-attack-a-ready","AttackA_Ready",2.7f,1.25f,0f,0,true,false),
   new Binding("thief-fat","NUI_THIEF_FAT","attack-a","THIEF_FAT_ATTACK_A","source-attack-a","AttackA_Fire",2.066666666666667f,1f,0f,0,true,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","attack-a-ready","PPORU_GIANT_RED_ATTACK_A_READY","source-attack-a-ready","SpecialAttackA_Ready",2.7f,1f,0f,0,true,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","attack-a","PPORU_GIANT_RED_ATTACK_A","source-attack-a","SpecialAttackA_Fire",2.266666666666667f,1f,0f,0,true,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","attack-b-ready","PPORU_GIANT_RED_ATTACK_B_READY","source-attack-b-ready","DashAttackReady",1.966666666666667f,1f,0f,0,true,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","attack-b","PPORU_GIANT_RED_ATTACK_B","source-attack-b","DashAttack",0.8f,1.4f,0.5714285714285715f,0,true,true),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","attack-b-end","PPORU_GIANT_RED_ATTACK_B_END","source-attack-b-end","DashAttackEnd",1.4f,1f,0f,0,true,false),
   new Binding("thief-thin","NUI_THIEF_THIN","launch","THIEF_THIN_DAMAGE_AIR_UP","source-air-up","DamageAirUp",1f,1f,0f,1,false,false),
   new Binding("thief-thin","NUI_THIEF_THIN","fall","THIEF_THIN_DAMAGE_AIR_FALL","source-air-fall","DamageAirFall",1f,1f,0f,1,false,false),
   new Binding("thief-fat","NUI_THIEF_FAT","launch","THIEF_FAT_DAMAGE_AIR_UP","source-air-up","DamageAirUp",1f,1f,0f,1,false,false),
   new Binding("thief-fat","NUI_THIEF_FAT","fall","THIEF_FAT_DAMAGE_AIR_FALL","source-air-fall","DamageAirFall",1f,1f,0f,1,false,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","launch","PPORU_GIANT_RED_DAMAGE_AIR_UP","source-air-up","DamageAirUp",1f,1f,0f,1,false,false),
   new Binding("pporu-giant-red","NUI_PPORU_GIANT_RED","fall","PPORU_GIANT_RED_DAMAGE_AIR_FALL","source-air-fall","DamageAirFall",1f,1f,0f,1,false,false),
  };
  public static bool Supported(bool scope,string kind,string npc){return scope&&(kind=="thief-thin"&&npc=="NUI_THIEF_THIN"||kind=="thief-fat"&&npc=="NUI_THIEF_FAT"||kind=="pporu-giant-red"&&npc=="NUI_PPORU_GIANT_RED");}
  static Binding FindAlias(string kind,string npc,string alias){foreach(var b in table)if(b.kind==kind&&b.npc==npc&&b.alias==alias)return b;return null;}
  public static bool Alias(string kind,string npc,string clip){return FindAlias(kind,npc,clip)!=null;}
  public static float Duration(string kind,string npc,string alias){var b=FindAlias(kind,npc,alias);return b==null?float.NaN:b.span;}
  public static bool SourceLoop(string kind,string npc,string alias){var b=FindAlias(kind,npc,alias);if(b==null)throw new InvalidOperationException("Unknown First3 combat alias");return b.loop;}
  public static bool AirAlias(string kind,string npc,string alias){var b=FindAlias(kind,npc,alias);return b!=null&&b.combat==1;}
  public static float SourceFps(string kind,string npc,string alias){var b=FindAlias(kind,npc,alias);if(b==null)throw new InvalidOperationException("Unknown First3 source alias");return b.combat==1?60f:24f;}
  static string Canonical(string c){return c=="launched"?"launch":c=="knockdown"?"down":c=="getup"?"get-up":c;}
  static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
  static float Repeat(float t,float span){return Math.Max(0,Math.Min(span,t-(float)Math.Floor(t/span)*span));}
  public static string Select(bool scope,string kind,string npc,bool active,bool dormant,bool passive,float hp,int combat,string sourceState,string previous,float actorClock,float npcTime,float previousClock,float deathTime,Func<string,float> duration,out float clock){
   // Existing ground callers retain the exact R9 contract; only the live presentation supplies the real physical-launch flag.
   return Select(scope,kind,npc,active,dormant,passive,hp,combat,sourceState,previous,false,actorClock,npcTime,previousClock,deathTime,duration,out clock);
  }
  public static string Select(bool scope,string kind,string npc,bool active,bool dormant,bool passive,float hp,int combat,string sourceState,string previous,bool physicalLaunch,float actorClock,float npcTime,float previousClock,float deathTime,Func<string,float> duration,out float clock){
   clock=previousClock;
   if(!Supported(scope,kind,npc)||!active||dormant||passive||string.IsNullOrEmpty(previous)||previous.StartsWith("source-",StringComparison.Ordinal))return previous;
   if(combat==5?hp>0:hp<=0)return previous;
   string model=Canonical(previous);Binding selected=null;
   foreach(var b in table){if(b.kind!=kind||b.npc!=npc||b.combat!=combat||b.model!=model)continue;if(b.combat==1&&!physicalLaunch)continue;if(b.attack&&sourceState!=b.state)continue;selected=b;break;}
   // Only original hit-launched Airborne poses use the six held sprites. Ordinary jump/traversal fall and all unproven states retain legacy.
   if(selected==null)return previous;
   float span=duration(selected.alias);
   if(!Finite(span)||Math.Abs(span-selected.span)>.000001f)throw new InvalidOperationException("Missing or retimed reviewed First3 combat alias: "+selected.alias);
   if(!Finite(actorClock)||actorClock<0||!Finite(npcTime)||npcTime<0||!Finite(deathTime)||deathTime<0)throw new InvalidOperationException("Invalid First3 source/display clock");
   if(selected.attack)clock=(selected.period>0?Repeat(npcTime,selected.period):npcTime)*selected.speed;
   else if(selected.model=="death")clock=Math.Max(0,Math.Min(1,deathTime/1.2f))*span;
   // Root selected actorClock for THIS family get-up B profile. Nasod remains separate.
   else clock=actorClock;
   return selected.alias;
  }
  public static bool TryUiAnchor(bool scope,string kind,string npc,string actualClip,float centerX,float visibleTopY,float fullWidth,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(!Supported(scope,kind,npc)||!Alias(kind,npc,actualClip))return false;
   if(!Finite(centerX)||!Finite(visibleTopY)||!Finite(fullWidth)||fullWidth<=0||!Finite(hp)||!Finite(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid First3 actual visible-alpha UI");
   bx=centerX-fullWidth*(1-hp/maxHp)/2;by=visibleTopY+8;dx=centerX;dy=visibleTopY+20;return true;
  }
 }
}
