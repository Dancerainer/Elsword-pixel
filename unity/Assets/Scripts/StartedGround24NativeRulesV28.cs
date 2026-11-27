using System;
namespace ElswordPixel {
 // Display only. SourceB is already in each atlas anchor; this class never moves or scales an actor.
 public static class StartedGround24NativeRulesV28 {
  public sealed class Spec {
   public readonly string kind,npc,sourceState,animation;public readonly int combat,sourceMotionIndex,samples;public readonly float duration;public readonly bool alpha;
   public Spec(string k,string n,int c,int i,string s,string a,float d,int count,bool p){kind=k;npc=n;combat=c;sourceMotionIndex=i;sourceState=s;animation=a;duration=d;samples=count;alpha=p;}
   public string alias {get{return combat==3?"source-down":combat==4?"source-get-up":"source-death";}}
   public string modelClip {get{return combat==3?"knockdown":combat==4?"getup":"death";}}
   public bool deathReusesDown {get{return combat==5&&animation=="DamageDownFront";}}
   public string resourcePrefix {get{return "Pixel/feedback-v25/started-ground24-v28/"+kind+"/ground-atlas-";}}
  }
  static readonly Spec[] specs={
   new Spec("benders","NUI_BENDERS_NORMAL",3,0,"BENDERS_NORMAL_DAMAGE_DOWN_FRONT","DamageDownFront",1.1333333333333333f,29,false),
   new Spec("benders","NUI_BENDERS_NORMAL",4,1,"BENDERS_NORMAL_STAND_UP_FRONT","DamageStandUpFront",1.1333333333333333f,29,false),
   new Spec("benders","NUI_BENDERS_NORMAL",5,2,"BENDERS_NORMAL_DYING","Dying",3.8999999999999999f,95,false),
   new Spec("benders-thief","NUI_BENDERS_THIEF",3,3,"BENDERS_THIEF_DAMAGE_DOWN_FRONT","DamageDownFront",1.6000000000000001f,40,false),
   new Spec("benders-thief","NUI_BENDERS_THIEF",5,3,"BENDERS_THIEF_DAMAGE_DOWN_FRONT","DamageDownFront",1.6000000000000001f,40,false),
   new Spec("benders-thief","NUI_BENDERS_THIEF",4,4,"BENDERS_THIEF_STAND_UP_FRONT","DamageStandUpFront",1.4666666666666666f,37,false),
   new Spec("chicken","NUI_CHICKEN",3,5,"CHICKEN_DAMAGE_DOWN_FRONT","DamageDownFront",0.76666666666666672f,20,false),
   new Spec("chicken","NUI_CHICKEN",5,5,"CHICKEN_DAMAGE_DOWN_FRONT","DamageDownFront",0.76666666666666672f,20,false),
   new Spec("chicken","NUI_CHICKEN",4,6,"CHICKEN_STAND_UP_FRONT","DamageStandUpFront",1.3333333333333333f,33,false),
   new Spec("thief","NUI_THIEF_THIN_ZERO",3,7,"THIEF_THIN_ZERO_DAMAGE_DOWN_FRONT","DamageDownFront",1.3999999999999999f,35,false),
   new Spec("thief","NUI_THIEF_THIN_ZERO",5,7,"THIEF_THIN_ZERO_DAMAGE_DOWN_FRONT","DamageDownFront",1.3999999999999999f,35,false),
   new Spec("thief","NUI_THIEF_THIN_ZERO",4,8,"THIEF_THIN_ZERO_STAND_UP_FRONT","DamageStandUpFront",1.3333333333333333f,33,false),
   new Spec("william","NUI_PPORU_WILLIAM_NORMAL",3,9,"WILLIAM_PPORU_DAMAGE_DOWN_FRONT","DamageDownFront",1.5666666666666667f,39,false),
   new Spec("william","NUI_PPORU_WILLIAM_NORMAL",4,10,"WILLIAM_PPORU_STAND_UP_FRONT","DamageStandUpFront",1.5666666666666667f,39,false),
   new Spec("william","NUI_PPORU_WILLIAM_NORMAL",5,11,"WILLIAM_PPORU_DYING","Dying",4.833333333333333f,117,false),
   new Spec("william-cute","NUI_PPORU_WILLIAM_CUTE",3,12,"PPORU_WILLIAM_CUTE_DAMAGE_DOWN_FRONT","DamageDownFront",1.1333333333333333f,29,false),
   new Spec("william-cute","NUI_PPORU_WILLIAM_CUTE",5,12,"PPORU_WILLIAM_CUTE_DAMAGE_DOWN_FRONT","DamageDownFront",1.1333333333333333f,29,false),
   new Spec("william-cute","NUI_PPORU_WILLIAM_CUTE",4,13,"PPORU_WILLIAM_CUTE_STAND_UP_FRONT","DamageStandUpFront",0.96666666666666667f,25,false),
   new Spec("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF",3,14,"PPORU_WILLIAM_CUTE_THIEF_DAMAGE_DOWN_FRONT","DamageDownFront",1.1333333333333333f,29,false),
   new Spec("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF",5,14,"PPORU_WILLIAM_CUTE_THIEF_DAMAGE_DOWN_FRONT","DamageDownFront",1.1333333333333333f,29,false),
   new Spec("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF",4,15,"PPORU_WILLIAM_CUTE_THIEF_STAND_UP_FRONT","DamageStandUpFront",0.96666666666666667f,25,false),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT",3,16,"ANGER_FOREST_ENT_DAMAGE_DOWN_FRONT","DamageDownFront",1.3666666666666667f,34,true),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT",5,16,"ANGER_FOREST_ENT_DAMAGE_DOWN_FRONT","DamageDownFront",1.3666666666666667f,34,true),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT",4,17,"ANGER_FOREST_ENT_STAND_UP_FRONT","DamageStandUpFront",1.8666666666666667f,46,true),
   new Spec("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY",3,18,"ANGER_FOREST_FAIRY_DAMAGE_DOWN_FRONT","DamageDownFront",0.96666666666666667f,25,true),
   new Spec("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY",5,18,"ANGER_FOREST_FAIRY_DAMAGE_DOWN_FRONT","DamageDownFront",0.96666666666666667f,25,true),
   new Spec("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY",4,19,"ANGER_FOREST_FAIRY_STAND_UP_FRONT","DamageStandUpFront",1.0333333333333334f,26,true),
   new Spec("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT",3,20,"ANGER_FOREST_KNIGHT_DAMAGE_DOWN_FRONT","DamageDownFront",1.5333333333333334f,38,true),
   new Spec("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT",5,20,"ANGER_FOREST_KNIGHT_DAMAGE_DOWN_FRONT","DamageDownFront",1.5333333333333334f,38,true),
   new Spec("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT",4,21,"ANGER_FOREST_KNIGHT_STAND_UP_FRONT","DamageStandUpFront",2.2999999999999998f,57,true),
   new Spec("curse-wisp","NUI_CURSE_WISP",3,22,"CURSE_WISP_DAMAGE_DOWN_FRONT","DamageDownFront",1.5333333333333334f,38,true),
   new Spec("curse-wisp","NUI_CURSE_WISP",5,22,"CURSE_WISP_DAMAGE_DOWN_FRONT","DamageDownFront",1.5333333333333334f,38,true),
   new Spec("curse-wisp","NUI_CURSE_WISP",4,23,"CURSE_WISP_STAND_UP_FRONT","DamageStandUpFront",1.6666666666666667f,41,true),
  };
  sealed class Birth {public readonly int room,staged,source;public readonly string stage,world,kind,npc;public Birth(int r,string s,string w,int i,int x,string k,string n){room=r;stage=s;world=w;staged=i;source=x;kind=k;npc=n;}}
  static readonly Birth[] alphaBirths={
   new Birth(0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",3,34,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",4,37,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",5,26,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",6,31,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(0,"forest-1","WI_ELDER_DARK_FOREST_STAGE_0",7,41,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",0,58,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",1,59,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",2,20,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",3,23,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",4,29,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",5,38,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",6,45,"anger-forest-fairy","NUI_ANGER_FOREST_FAIRY"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",7,55,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",8,56,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",9,6,"anger-forest-knight","NUI_ANGER_FOREST_KNIGHT"),
   new Birth(1,"forest-2","WI_ELDER_DARK_FOREST_STAGE_1",10,3,"curse-wisp","NUI_CURSE_WISP"),
   new Birth(2,"forest-3","WI_ELDER_DARK_FOREST_BOSS_STAGE",0,9,"anger-forest-ent","NUI_ANGER_FOREST_ENT"),
  };
  public static Spec Find(string kind,string npc,int combat){foreach(var s in specs)if(s.kind==kind&&s.npc==npc&&s.combat==combat)return s;return null;}
  public static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  public static bool GenericScope(bool birth,bool profile,string kind,string npc){var s=Find(kind,npc,3);return s!=null&&!s.alpha&&birth&&!profile;}
  public static bool AlphaBirth(bool identity,int dungeon,string graph,bool baseProfile,string content,int room,string stage,string world,int staged,int source,string kind,string npc){if(!identity||dungeon!=3||graph!="Dungeon_Elder_Druid_Forest_Normal"||!baseProfile||!string.IsNullOrEmpty(content))return false;foreach(var b in alphaBirths)if(b.room==room&&b.stage==stage&&b.world==world&&b.staged==staged&&b.source==source&&b.kind==kind&&b.npc==npc)return true;return false;}
  // No invented reaction5: Down-reuse death is verified against its explicit packet Down/reaction3 source.
  public static bool AlphaReactionExact(string kind,string npc,int combat,int reactionCombat,string sourceState,string animation,float speed,float duration){var s=Find(kind,npc,combat);if(s==null||!s.alpha)return false;int expected=combat==5?(s.deathReusesDown?3:-1):combat;return expected>=0&&reactionCombat==expected&&sourceState==s.sourceState&&animation==s.animation&&speed==1&&Finite(duration)&&Math.Abs(duration-s.duration)<=.000001f;}
  public static bool Eligible(bool scope,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string modelClip,string previous){var s=Find(kind,npc,combat);return scope&&s!=null&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&(combat==5?hp<=0:hp>0)&&modelClip==s.modelClip&&previous==modelClip;}
  public static bool Resource(Spec s,string resource){return s!=null&&resource!=null&&resource.StartsWith(s.resourcePrefix,StringComparison.Ordinal)&&resource.Length==s.resourcePrefix.Length+2&&resource[resource.Length-2]>='0'&&resource[resource.Length-2]<='9'&&resource[resource.Length-1]>='0'&&resource[resource.Length-1]<='9';}
  public static bool ValidateClip(Spec s,string alias,float libraryScale,string resource,float fps,float duration,bool loop,float rootDistance,int samples,int hits,bool coherent){return s!=null&&alias==s.alias&&libraryScale==1&&Resource(s,resource)&&fps==24&&Finite(duration)&&Math.Abs(duration-s.duration)<=.000001f&&!loop&&rootDistance==0&&samples==s.samples&&hits==0&&coherent;}
  public static string Select(bool scope,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string modelClip,string previous,float actorClock,float deathTime,float previousClock,Func<bool> reviewedMetadata,out float clock){clock=previousClock;if(!Eligible(scope,kind,npc,visible,active,dormant,passive,aiEnabled,hp,combat,modelClip,previous)||!reviewedMetadata())return previous;var s=Find(kind,npc,combat);float t=combat==5?deathTime:actorClock;if(!Finite(t)||t<0)throw new InvalidOperationException("Invalid original Ground actor/death clock");clock=combat==5?Math.Max(0,Math.Min(1,t/1.2f))*s.duration:combat==4?Math.Max(0,Math.Min(1,actorClock/GameModel.EnemyGetUpDuration))*s.duration:t;return s.alias;}
  public static bool TryUi(bool eligible,int combat,string actualAlias,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){bx=by=dx=dy=0;if(!eligible||(combat!=3&&combat!=4)||actualAlias!=(combat==3?"source-down":"source-get-up")||hp<=0)return false;if(!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid Ground alpha-top UI");bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;}
 }
}
