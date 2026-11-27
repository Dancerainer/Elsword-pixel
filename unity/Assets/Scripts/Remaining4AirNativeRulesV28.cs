using System;
namespace ElswordPixel {
 // Same source INIT_FLY motion across the current host launched/fall route.
 // No new AIR_UP/AIR_FALL state, phase timer, AI jump or physical change.
 public static class Remaining4AirNativeRulesV28 {
  public sealed class Spec {
   public readonly string kind,npc,state,animation,alias;public readonly float duration;public readonly int samples,authoredFacing;public readonly bool guardianReuse;
   public Spec(string k,string n,string s,string a,float d,int c,int facing,bool reuse=false){kind=k;npc=n;state=s;animation=a;duration=d;samples=c;authoredFacing=facing;guardianReuse=reuse;alias=reuse?"source-down":"source-air-hit";}
   public string Evidence {get{return "Remaining4Air3-v28:"+kind+":"+alias;}}
   public string ResourcePrefix {get{return "Pixel/feedback-v25/started-remaining4-air3-v28/"+kind+"/air-atlas-";}}
  }
  public static readonly Spec[] All={
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_DAMAGE_SMALL_FRONT","DamageFront",.9333333333333333f,24,1),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_DAMAGE_FLY_FRONT","DamageAirFlyFront",1f,25,1),
   new Spec("toto","NUI_THIEF_THIN_TOTO","THIEF_THIN_TOTO_DAMAGE_FLY_FRONT","DamageAirUp",.2f,6,-1),
   new Spec("fairy-guardian","NUI_FAIRY_GUARDIAN","FAIRY_GUARDIAN_DAMAGE_DOWN_FRONT","DamageDownFront",1.2666666666666666f,32,-1,true)};
  public static bool Finite(float value){return !float.IsNaN(value)&&!float.IsInfinity(value);}
  public static Spec Find(string kind,string npc){return Array.Find(All,s=>s.kind==kind&&s.npc==npc);}
  public static bool OriginalBirth(Spec s,string stage,int staged,int sourceIndex){
   if(s==null)return false;
   if(s.kind=="ancient-phoru")return stage=="swamp-3"&&staged==0&&sourceIndex==9;
   if(s.kind=="white-giant")return stage=="tree-3"&&staged==2&&sourceIndex==10;
   if(s.kind=="toto")return stage=="swamp-1"&&(staged==2&&sourceIndex==9||staged==4&&sourceIndex==14||staged==6&&sourceIndex==31);
   return s.guardianReuse&&(stage=="ruins-1"&&staged==4&&sourceIndex==27||stage=="ruins-2"&&(staged==6&&sourceIndex==35||staged==7&&sourceIndex==47)||stage=="swamp-2"&&staged==6&&sourceIndex==29);
  }
  public static bool Eligible(Spec s,bool birth,bool exactLoadedProfile,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string npcState,string modelClip,string previous){
   return s!=null&&birth&&exactLoadedProfile&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&hp>0&&combat==1&&string.IsNullOrEmpty(npcState)&&(modelClip=="launched"||modelClip=="fall")&&previous==modelClip;
   // Deliberately no knockdownOnLanding gate: Ancient original DAMAGE_DOWN=false
   // preserves legitimate public Launch while declining the landing knockdown.
  }
  public static bool Resource(Spec s,string resource){
   if(s==null||s.guardianReuse||string.IsNullOrEmpty(resource)||!resource.StartsWith(s.ResourcePrefix,StringComparison.Ordinal))return false;
   string suffix=resource.Substring(s.ResourcePrefix.Length);int page;return suffix.Length==2&&int.TryParse(suffix,out page)&&page>=0&&page<100;
  }
  public static string Select(Spec s,bool eligible,string previous,float actorClock,float previousClock,Func<bool> metadata,out float clock){
   clock=previousClock;if(!eligible||!Finite(actorClock)||actorClock<0||metadata==null||!metadata())return previous;clock=actorClock;return s.alias;
  }
 }
}
