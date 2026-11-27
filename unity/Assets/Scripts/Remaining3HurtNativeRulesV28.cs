using System;
namespace ElswordPixel {
 // Three exact generic enemy display adapters. This does not implement source Hurt direction dispatch or a source NPC profile.
 public static class Remaining3HurtNativeRulesV28 {
  public const string Alias="source-hurt";
  public sealed class Spec {
   public readonly string kind,npc,sourceState,animation;public readonly float duration;public readonly int samples,primaryYaw,authoredFacing;
   public Spec(string k,string n,string s,string a,float d,int count,int yaw,int facing){kind=k;npc=n;sourceState=s;animation=a;duration=d;samples=count;primaryYaw=yaw;authoredFacing=facing;}
   public string resourcePrefix {get{return "Pixel/feedback-v25/started-hurt3-v28/"+kind+"/hurt-atlas-";}}
   public string requiredEvidence {get{return "Root-admitted-Hurt3-display-v1|"+kind+"|"+npc+"|"+sourceState+"|"+animation+"|NormalA-raster-SourceB-anchor-once";}}
  }
  static readonly Spec[] specs={
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_DAMAGE_SMALL_FRONT","DamageFront",0.9333333333333333f,24,0,1),
   new Spec("toto","NUI_THIEF_THIN_TOTO","THIEF_THIN_TOTO_DAMAGE_FRONT","Damage",0.9666666666666667f,25,180,-1),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_DAMAGE_FRONT","DamageBig",1.4333333333333333f,36,0,1),
  };
  public static Spec Find(string kind,string npc){foreach(var s in specs)if(s.kind==kind&&s.npc==npc)return s;return null;}
  public static bool Finite(float value){return !float.IsNaN(value)&&!float.IsInfinity(value);}
  public static bool Scope(bool exactExistingNormalBirth,bool hasCurrentProfile,string kind,string npc){return exactExistingNormalBirth&&!hasCurrentProfile&&Find(kind,npc)!=null;}
  public static bool Eligible(bool scope,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string npcState,string modelClip,string previous){
   return scope&&Find(kind,npc)!=null&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&hp>0&&combat==2&&string.IsNullOrEmpty(npcState)&&modelClip=="hurt"&&previous=="hurt";
  }
  public static bool Resource(Spec spec,string resource){if(spec==null||resource==null||!resource.StartsWith(spec.resourcePrefix,StringComparison.Ordinal)||resource.Length!=spec.resourcePrefix.Length+2)return false;int i=spec.resourcePrefix.Length;return resource[i]>='0'&&resource[i]<='9'&&resource[i+1]>='0'&&resource[i+1]<='9';}
  public static bool ValidateClip(Spec spec,string alias,string evidence,float libraryScale,string resource,float fps,float duration,bool loop,float rootDistance,int samples,int hits){
   return spec!=null&&alias==Alias&&evidence==spec.requiredEvidence&&libraryScale==1&&Resource(spec,resource)&&fps==24&&Finite(duration)&&Math.Abs(duration-spec.duration)<=.000001f&&!loop&&rootDistance==0&&samples==spec.samples&&hits==0;
  }
  public static string Select(bool scope,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string npcState,string modelClip,string previous,float actorClock,float previousClock,Func<Spec,bool> admittedMetadata,out float clock){
   clock=previousClock;if(!Eligible(scope,kind,npc,visible,active,dormant,passive,aiEnabled,hp,combat,npcState,modelClip,previous)||!Finite(actorClock)||actorClock<0)return previous;
   var spec=Find(kind,npc);if(admittedMetadata==null||!admittedMetadata(spec))return previous;
   // Physical Hurt remains .28 seconds. Only its naturally visible prefix is sampled; never retime the full intrinsic span.
   clock=actorClock;return Alias;
  }
  public static bool TryUi(bool eligible,string actualClip,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(!eligible||actualClip!=Alias||!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||hp<=0||maxHp<=0)return false;
   bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;
  }
 }
}
