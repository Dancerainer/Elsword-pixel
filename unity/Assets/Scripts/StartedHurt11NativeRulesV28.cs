using System;
namespace ElswordPixel {
 // Strict display-only Hurt. Missing/unreviewed metadata preserves legacy Hurt.
 public static class StartedHurt11NativeRulesV28 {
  public sealed class Spec {public readonly string kind,npc,sourceState,animation;public readonly float duration;public readonly int samples;public readonly bool alpha;
   public Spec(string k,string n,string s,string a,float d,int c,bool p){kind=k;npc=n;sourceState=s;animation=a;duration=d;samples=c;alpha=p;}
   public string resource {get{return "Pixel/feedback-v25/started-hurt-v28/"+kind+"/hurt-atlas";}}
  }
  static readonly Spec[] specs={
   new Spec("benders","NUI_BENDERS_NORMAL","BENDERS_NORMAL_DAMAGE_SMALL","DamageSmall",0.90000000000000002f,23,false),
   new Spec("benders-thief","NUI_BENDERS_THIEF","BENDERS_THIEF_DAMAGE_FRONT","DamageSmall",0.83333333333333337f,21,false),
   new Spec("chicken","NUI_CHICKEN","CHICKEN_DAMAGE_FRONT","Damage",0.66666666666666663f,17,false),
   new Spec("thief","NUI_THIEF_THIN_ZERO","THIEF_THIN_ZERO_DAMAGE_FRONT","DamageBig",1.1666666666666667f,29,false),
   new Spec("william","NUI_PPORU_WILLIAM_NORMAL","WILLIAM_PPORU_DAMAGE_SMALL","DamageSmall",1.1666666666666667f,29,false),
   new Spec("william-cute","NUI_PPORU_WILLIAM_CUTE","PPORU_WILLIAM_CUTE_DAMAGE_FRONT","Damage",1.0333333333333334f,26,false),
   new Spec("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF","PPORU_WILLIAM_CUTE_THIEF_DAMAGE_FRONT","Damage",1.0333333333333334f,26,false),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_DAMAGE","Damage",1.0666666666666667f,27,true),
   new Spec("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY","ANGER_FOREST_FAIRY_DAMAGE_FRONT","DamageFront",1.0666666666666667f,27,true),
   new Spec("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT","ANGER_FOREST_KNIGHT_DAMAGE_FRONT","DamageFront",0.96666666666666667f,25,true),
   new Spec("curse-wisp","NUI_CURSE_WISP","CURSE_WISP_DAMAGE_FRONT","DamageFront",0.96666666666666667f,25,true),
  };
  public static Spec Find(string kind,string npc){foreach(var s in specs)if(s.kind==kind&&s.npc==npc)return s;return null;}
  public static bool GenericScope(bool originalBirth,bool hasCurrentProfile,string kind,string npc){var s=Find(kind,npc);return s!=null&&!s.alpha&&originalBirth&&!hasCurrentProfile;}
  public static bool AlphaScope(int dungeon,string graph,bool baseline,string content,bool originalBirth,bool hasCurrentProfile,string kind,string npc){var s=Find(kind,npc);return s!=null&&s.alpha&&originalBirth&&hasCurrentProfile&&Alpha4NormalNativeRulesV28.Scope(dungeon,graph,baseline,content,kind,npc);}
  public static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  public static bool Eligible(bool exactScope,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string modelClip,string previous){return exactScope&&Find(kind,npc)!=null&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&hp>0&&combat==2&&modelClip=="hurt"&&previous=="hurt";}
  public static bool ValidateClip(string kind,string npc,string alias,float libraryScale,string resource,float fps,float duration,bool loop,float rootDistance,int samples,int hits,bool allFramesAnchoredRootZeroAndExactResource){var s=Find(kind,npc);return s!=null&&alias=="source-hurt"&&libraryScale==1&&resource==s.resource&&fps==24&&Finite(duration)&&Math.Abs(duration-s.duration)<=.000001f&&!loop&&rootDistance==0&&samples==s.samples&&hits==0&&allFramesAnchoredRootZeroAndExactResource;}
  public static string Select(bool exactScope,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string modelClip,string previous,float actorClock,float previousClock,Func<bool> reviewedFullFrameMetadata,out float clock){clock=previousClock;if(!Eligible(exactScope,kind,npc,visible,active,dormant,passive,aiEnabled,hp,combat,modelClip,previous)||!reviewedFullFrameMetadata())return previous;if(!Finite(actorClock)||actorClock<0)throw new InvalidOperationException("Invalid original Hurt actor clock");clock=actorClock;return "source-hurt";}
  public static bool TryUiAnchor(bool eligible,string actualClip,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){bx=by=dx=dy=0;if(!eligible||actualClip!="source-hurt")return false;if(!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid actual Hurt visible alpha/HP");bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;}
 }
}
