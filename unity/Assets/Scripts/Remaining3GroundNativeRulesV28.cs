using System;
namespace ElswordPixel {
 // Dense source pictures. Common host reaction/death lifetimes remain explicit display adapters.
 public static class Remaining3GroundNativeRulesV28 {
  public sealed class Spec {
   public readonly string kind,npc,sourceState,animation,alias,modelClip;public readonly int combat,samples,authoredFacing;public readonly float duration;
   public Spec(string k,string n,int c,string s,string a,float d,int count,int facing){kind=k;npc=n;combat=c;sourceState=s;animation=a;duration=d;samples=count;authoredFacing=facing;alias=c==3?"source-down":c==4?"source-get-up":"source-death";modelClip=c==3?"knockdown":c==4?"getup":"death";}
   public string resourcePrefix{get{return "Pixel/feedback-v25/started-remaining3-ground5-v28/"+kind+"/ground-atlas-";}}
   public string admissionCommand{get{return "Root-admitted-Remaining3Ground5-display-v1|"+kind+"|"+npc+"|"+sourceState+"|"+animation+"|preserve-sourceY";}}
  }
  static readonly Spec[] specs={
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU",5,"ANCIENT_PPORU_DYING_LAND_FRONT","DamageDownFront",3.0666666666666667f,75,1),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE",3,"PPORU_GIANT_WHITE_DAMAGE_DOWN_FRONT","DamageDownFront",1.5666666666666667f,39,1),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE",5,"PPORU_GIANT_WHITE_DYING_LAND_FRONT","DamageDownFront",1.5666666666666667f,39,1),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE",4,"PPORU_GIANT_WHITE_STAND_UP_FRONT","DamageStandUpFront",1.8f,45,1),
   new Spec("toto","NUI_THIEF_THIN_TOTO",3,"THIEF_THIN_TOTO_DAMAGE_DOWN_FRONT","DamageDownFront",1.1666666666666667f,29,-1),
   new Spec("toto","NUI_THIEF_THIN_TOTO",5,"THIEF_THIN_TOTO_DYING_LAND_FRONT","DamageDownFront",1.1666666666666667f,29,-1),
   new Spec("toto","NUI_THIEF_THIN_TOTO",4,"THIEF_THIN_TOTO_STAND_UP_FRONT","DamageStandUpFront",1.8666666666666667f,46,-1)
  };
  public static Spec Find(string kind,string npc,int combat){foreach(var s in specs)if(s.kind==kind&&s.npc==npc&&s.combat==combat)return s;return null;}
  public static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
  public static bool Eligible(Spec s,bool originalBirth,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,string modelClip,string previous){return s!=null&&originalBirth&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&(s.combat==5?hp<=0:hp>0)&&modelClip==s.modelClip&&previous==modelClip;}
  public static bool Resource(Spec s,string r){return s!=null&&r!=null&&r.StartsWith(s.resourcePrefix,StringComparison.Ordinal)&&r.Length==s.resourcePrefix.Length+2&&r[r.Length-2]>='0'&&r[r.Length-2]<='9'&&r[r.Length-1]>='0'&&r[r.Length-1]<='9';}
  public static bool Validate(Spec s,string alias,string command,float scale,string texture,float fps,float duration,bool loop,float rootDistance,int count,int hits,bool coherent){return s!=null&&alias==s.alias&&command==s.admissionCommand&&scale==1&&Resource(s,texture)&&fps==24&&Finite(duration)&&Math.Abs(duration-s.duration)<=.000001f&&!loop&&rootDistance==0&&count==s.samples&&hits==0&&coherent;}
  public static string Select(Spec s,bool originalBirth,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,string modelClip,string previous,float actorClock,float deathTime,float previousClock,Func<bool> metadata,out float clock){
   clock=previousClock;if(!Eligible(s,originalBirth,visible,active,dormant,passive,aiEnabled,hp,modelClip,previous)||!metadata())return previous;
   float t=s.combat==5?deathTime:actorClock;if(!Finite(t)||t<0)return previous;
   // This preserves the common host visual lifetime policy already used by Ground24.
   // It is not original reaction-state execution: death spans 1.2 host seconds, get-up .45.
   clock=s.combat==5?Math.Min(1,t/1.2f)*s.duration:s.combat==4?Math.Min(1,t/GameModel.EnemyGetUpDuration)*s.duration:Math.Min(t,s.duration);return s.alias;
  }
  public static bool Ui(Spec s,string actual,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(s==null||s.combat==5||actual!=s.alias||!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||hp<=0||maxHp<=0)return false;
   bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;
  }
 }
}

