using System;
namespace ElswordPixel {
 // Guardian only. Data admission, source event effects, and atlas art are separate.
 public static class GuardianCombatNativeRulesV28 {
  public const string Npc="NUI_FAIRY_GUARDIAN",Kind="fairy-guardian",Wait="FAIRY_GUARDIAN_WAIT",Walk="FAIRY_GUARDIAN_WALK",Ready="FAIRY_GUARDIAN_ATTACK_READY",Fire="FAIRY_GUARDIAN_ATTACK_A",TrapWait="FAIRY_GUARDIAN_TRAP_MODE_WAIT",TrapStart="FAIRY_GUARDIAN_TRAP_MODE_START";
  public sealed class Spec {
   public readonly string state,modelClip,alias,animation;public readonly float span;public readonly int samples,combat;public readonly bool normal,loop;
   public Spec(string s,string m,string a,string n,float d,int count,int c,bool existing=false){state=s;modelClip=m;alias=a;animation=n;span=d;samples=count;combat=c;normal=existing;loop=existing||s==TrapWait;}
   public string Evidence {get{return "GuardianCombat8-v28:"+Kind+":"+alias;}}
  }
  public static readonly Spec[] All={
   new Spec(TrapWait,"idle","source-trap-wait","TrapModeWait",.16666666666666666f,5,0),
   new Spec(TrapStart,"attack","source-trap-start","TrapModeStart",4.8f,117,0),
   new Spec(Wait,"idle","source-idle","Wait",1.3333333333333333f,33,0,true),
   new Spec(Walk,"walk","source-walk","Walk",2f,49,0,true),
   new Spec(Ready,"attack","source-attack-ready","AttackA_Ready",2.6f,64,0),
   new Spec(Fire,"attack","source-attack-a","AttackA_Fire",2f,49,0),
   new Spec("","hurt","source-hurt","DamageSmall",.9333333333333333f,24,2),
   new Spec("","knockdown","source-down","DamageDownFront",1.2666666666666666f,32,3),
   new Spec("","getup","source-get-up","DamageStandUpFront",1.7f,42,4),
   new Spec("","death","source-death","DamageDownBack",1.2666666666666666f,32,5)};
  public static bool Finite(float f){return !float.IsNaN(f)&&!float.IsInfinity(f);}
  public static bool Identity(string kind,string npc){return kind==Kind&&npc==Npc;}
  public static bool SourceProfile(NpcDefinition p){
   if(p==null||!Identity(p.kind,p.id)||p.waitState!=Wait||p.walkState!=Walk||p.initialState!=TrapWait||!string.IsNullOrEmpty(p.getUpState)||p.states==null||p.states.Length!=6||p.reactions==null||p.reactions.Length!=0||p.physicalAttack!=18||p.magicAttack!=0)return false;
   foreach(var spec in All){if(spec.combat!=0)continue;var s=Array.Find(p.states,x=>x!=null&&x.id==spec.state);
    if(s==null||s.clip!=spec.modelClip||s.sourceAnimation!=spec.animation||s.sourceAnimSpeed!=1||Math.Abs(s.duration-spec.span)>.000001f||s.loop!=spec.loop||s.superArmor!=(spec.state==Walk||spec.state==Ready||spec.state==Fire))return false;}
   return true;
  }
  public static Spec Find(int combat,string state,string modelClip){return Array.Find(All,s=>s.combat==combat&&s.modelClip==modelClip&&(combat==0?state==s.state:string.IsNullOrEmpty(state)));}
  public static bool Resource(Spec s,string resource){
   if(s==null||string.IsNullOrEmpty(resource))return false;
   if(s.normal)return resource=="Pixel/feedback-v25/started-normal-v28/fairy-guardian/normal-atlas";
   string prefix="Pixel/feedback-v25/started-guardian-combat-v28/fairy-guardian/combat-atlas-";
   if(!resource.StartsWith(prefix,StringComparison.Ordinal))return false;string suffix=resource.Substring(prefix.Length);int page;return suffix.Length==2&&int.TryParse(suffix,out page)&&page>=0&&page<100;
  }
  public static float Clock(Spec s,float npcTime,float actorClock,float deathTime,float getUpDuration=GameModel.EnemyGetUpDuration){
   if(s.combat==0)return npcTime; // Source speed1 exactly once, no .85 compression.
   if(s.combat==4)return Math.Max(0,Math.Min(1,actorClock/getUpDuration))*s.span;
   if(s.combat==5)return Math.Max(0,Math.Min(1,deathTime/1.2f))*s.span;
   return actorClock; // Hurt .28 and Down .8 keep the original visible prefix.
  }
 }
}
