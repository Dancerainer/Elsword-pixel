using System;
namespace ElswordPixel {
 // Display only. Generic clock is a host visual adapter; Alpha clock is its existing source state clock.
 public static class Attack28NativeDisplayRulesV28 {
  public sealed class Spec {
   public readonly string kind,npc,sourceState,animation,modelClip,alias;
   public readonly float duration,speed,stateSpan;public readonly int samples;public readonly bool alpha,fire;
   public Spec(string k,string n,string s,string a,string m,string id,float d,float v,float span,int count,bool p,bool q){kind=k;npc=n;sourceState=s;animation=a;modelClip=m;alias=id;duration=d;speed=v;stateSpan=span;samples=count;alpha=p;fire=q;}
   public string resourcePrefix {get{return "Pixel/feedback-v25/started-attack28-v28/"+kind+"/attack-atlas-";}}
   // The candidate bank leaves command empty. Only a Root-approved final registry may write this marker.
   // This is an explicit admission marker inside the separately pinned catalog, not a cryptographic signature.
   public string admissionCommand {get{return "Root-admitted-Attack28-display-v1|"+kind+"|"+npc+"|"+sourceState+"|"+animation+"|NormalA-unit-once";}}
  }
  static readonly Spec[] specs={
   new Spec("benders","NUI_BENDERS_NORMAL","BENDERS_NORMAL_ATTACK_A_READY","AttackA_Ready","attack","source-attack-ready",2.6000000000000001f,1f,2.6000000000000001f,64,false,false),
   new Spec("benders","NUI_BENDERS_NORMAL","BENDERS_NORMAL_ATTACK_A","AttackA_Fire","attack","source-attack-fire",2.2666666666666666f,1f,2.2666666666666666f,56,false,true),
   new Spec("benders-thief","NUI_BENDERS_THIEF","BENDERS_THIEF_ATTACK_READY","AttackA_Ready","attack","source-attack-ready",2.6000000000000001f,1f,2.6000000000000001f,64,false,false),
   new Spec("benders-thief","NUI_BENDERS_THIEF","BENDERS_THIEF_ATTACK_A","AttackA_Fire","attack","source-attack-fire",3.2999999999999998f,1f,3.2999999999999998f,81,false,true),
   new Spec("chicken","NUI_CHICKEN","CHICKEN_ATTACK_READY","AttackA_Ready","attack","source-attack-ready",2.8999999999999999f,1.25f,2.3199999999999998f,71,false,false),
   new Spec("chicken","NUI_CHICKEN","CHICKEN_ATTACK_A","AttackA_Fire","attack","source-attack-fire",1.8f,1f,1.8f,45,false,true),
   new Spec("thief","NUI_THIEF_THIN_ZERO","THIEF_THIN_ZERO_ATTACK_READY","AttackA_Ready","attack","source-attack-ready",2.6000000000000001f,1.25f,2.0800000000000001f,64,false,false),
   new Spec("thief","NUI_THIEF_THIN_ZERO","THIEF_THIN_ZERO_ATTACK_A","AttackA_Fire","attack","source-attack-fire",0.83333333333333337f,1f,0.83333333333333337f,21,false,true),
   new Spec("william","NUI_PPORU_WILLIAM_NORMAL","WILLIAM_PPORU_ATTACK_A_READY","Attack_Ready","attack","source-attack-ready",2.6000000000000001f,1f,2.6000000000000001f,64,false,false),
   new Spec("william","NUI_PPORU_WILLIAM_NORMAL","WILLIAM_PPORU_ATTACK_A","Attack_Fire","attack","source-attack-fire",1.5333333333333334f,1f,1.5333333333333334f,38,false,true),
   new Spec("william-cute","NUI_PPORU_WILLIAM_CUTE","PPORU_WILLIAM_CUTE_ATTACK_READY","AttackA_Ready","attack","source-attack-ready",2.8999999999999999f,1.25f,2.3199999999999998f,71,false,false),
   new Spec("william-cute","NUI_PPORU_WILLIAM_CUTE","PPORU_WILLIAM_CUTE_ATTACK_A","AttackA_Fire","attack","source-attack-fire",1.0333333333333334f,1f,1.0333333333333334f,26,false,true),
   new Spec("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF","PPORU_WILLIAM_CUTE_THIEF_ATTACK_READY","MagicAttackA_Ready","attack","source-attack-ready",3f,1.25f,2.3999999999999999f,73,false,false),
   new Spec("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF","PPORU_WILLIAM_CUTE_THIEF_ATTACK_A","MagicAttackA_Fire","attack","source-attack-fire",1.1666666666666667f,1f,1.1666666666666667f,29,false,true),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_ATTACK_A_READY","Attack_A_Ready","attack-a-ready","source-attack-a-ready",3.1666666666666665f,1f,3.1666666666666665f,77,true,false),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_ATTACK_A","Attack_A_Fire","attack-a","source-attack-a",3.1666666666666665f,1f,3.1666666666666665f,77,true,true),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_ATTACK_B_READY","Attack_B_Ready","attack-b-ready","source-attack-b-ready",2.7999999999999998f,1f,2.7999999999999998f,69,true,false),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_ATTACK_B","Attack_B_Fire","attack-b","source-attack-b",2.6000000000000001f,1.5f,1.7333333333333334f,64,true,true),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_ATTACK_C_READY","Attack_C_Ready","attack-c-ready","source-attack-c-ready",2.6000000000000001f,1f,2.6000000000000001f,64,true,false),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_ATTACK_C","Attack_C_Fire","attack-c","source-attack-c",1.5f,1f,1.5f,37,true,true),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_SPECIAL_ATTACK_READY","Special_Attack_Ready","attack-special-ready","source-attack-special-ready",4f,1f,4f,97,true,false),
   new Spec("anger-forest-ent","NUI_ANGER_FOREST_ENT","ANGER_FOREST_ENT_SPECIAL_ATTACK","Special_Attack_Fire","attack-special","source-attack-special",3.2333333333333334f,1f,3.2333333333333334f,79,true,true),
   new Spec("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY","ANGER_FOREST_FAIRY_ATTACK_READY","Attack_A_Ready","attack-ready","source-attack-ready",3.1666666666666665f,1.25f,2.5333333333333332f,77,true,false),
   new Spec("anger-forest-fairy","NUI_ANGER_FOREST_FAIRY","ANGER_FOREST_FAIRY_ATTACK_A","Attack_A","attack-a","source-attack-a",1.1666666666666667f,1f,1.1666666666666667f,29,true,true),
   new Spec("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT","ANGER_FOREST_KNIGHT_ATTACK_A_READY","Attack_A_Ready","attack-a-ready","source-attack-a-ready",2.9333333333333331f,1.25f,2.3466666666666667f,72,true,false),
   new Spec("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT","ANGER_FOREST_KNIGHT_ATTACK_A","Attack_A","attack-a","source-attack-a",1.7333333333333334f,1f,1.7333333333333334f,43,true,true),
   new Spec("curse-wisp","NUI_CURSE_WISP","CURSE_WISP_ATTACK_A_READY","Attack_A_Ready","attack-a-ready","source-attack-a-ready",3.2666666666666666f,1.25f,2.6133333333333333f,80,true,false),
   new Spec("curse-wisp","NUI_CURSE_WISP","CURSE_WISP_ATTACK_A","Attack_A","attack-a","source-attack-a",2.5f,1f,2.5f,61,true,true),
   new Spec("benders-nasod","NUI_BENDERS_NASOD","BENDERS_NASOD_ATTACK_A_READY","AttackA_Ready","attack-a-ready","source-attack-a-ready",2.7000000000000002f,1f,2.7000000000000002f,66,true,false),
   new Spec("benders-nasod","NUI_BENDERS_NASOD","BENDERS_NASOD_ATTACK_A","AttackA_Fire","attack-a","source-attack-a",1.1666666666666667f,1.3999999999999999f,0.83333333333333348f,29,true,true),
   new Spec("benders-nasod","NUI_BENDERS_NASOD","BENDERS_NASOD_ATTACK_B_READY","AttackB_Ready","attack-b-ready","source-attack-b-ready",2.7000000000000002f,1f,2.7000000000000002f,66,true,false),
   new Spec("benders-nasod","NUI_BENDERS_NASOD","BENDERS_NASOD_ATTACK_B","AttackB_Fire","attack-b","source-attack-b",3.4333333333333331f,1f,3.4333333333333331f,84,true,true),
   new Spec("benders-nasod","NUI_BENDERS_NASOD","BENDERS_NASOD_SPECIAL_ATTACK_A_READY","SpecialAttackA_Ready","attack-special-ready","source-attack-special-ready",2.7000000000000002f,1f,2.7000000000000002f,66,true,false),
   new Spec("benders-nasod","NUI_BENDERS_NASOD","BENDERS_NASOD_SPECIAL_ATTACK_A","SpecialAttackA_Fire","attack-special","source-attack-special",1.6000000000000001f,1f,1.6000000000000001f,40,true,true),
  };
  public static bool Finite(float value){return !float.IsNaN(value)&&!float.IsInfinity(value);}
  static float Clamp01(float value){return Math.Max(0,Math.Min(1,value));}
  public static Spec Generic(string kind,string npc,bool fire){foreach(var s in specs)if(!s.alpha&&s.kind==kind&&s.npc==npc&&s.fire==fire)return s;return null;}
  public static Spec Alpha(string kind,string npc,string state,string modelClip){foreach(var s in specs)if(s.alpha&&s.kind==kind&&s.npc==npc&&s.sourceState==state&&s.modelClip==modelClip)return s;return null;}
  public static Spec Alias(string kind,string npc,string actualAlias){foreach(var s in specs)if(s.kind==kind&&s.npc==npc&&s.alias==actualAlias)return s;return null;}
  public static bool Resource(Spec s,string resource){if(s==null||resource==null||!resource.StartsWith(s.resourcePrefix,StringComparison.Ordinal)||resource.Length!=s.resourcePrefix.Length+2)return false;int n=resource.Length;return resource[n-2]>='0'&&resource[n-2]<='9'&&resource[n-1]>='0'&&resource[n-1]<='9';}
  public static bool ValidateClip(Spec s,string id,string command,float libraryScale,string texture,float fps,float duration,bool loop,float rootDistance,int frames,int hits,bool coherent){return s!=null&&id==s.alias&&command==s.admissionCommand&&libraryScale==1&&Resource(s,texture)&&fps==24&&Finite(duration)&&Math.Abs(duration-s.duration)<=.000001f&&!loop&&rootDistance==0&&frames==s.samples&&hits==0&&coherent;}
  static bool Active(bool scope,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string modelClip,string previous){return scope&&visible&&active&&!dormant&&!passive&&aiEnabled&&Finite(hp)&&hp>0&&combat==0&&previous==modelClip;}
  public static string SelectGeneric(bool originalBirth,bool profile,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string sourceState,string modelClip,string previous,float remaining,bool struck,float previousClock,Func<Spec,bool> admittedMetadata,out float clock){
   clock=previousClock;if(profile||!Active(originalBirth,visible,active,dormant,passive,aiEnabled,hp,combat,modelClip,previous)||!string.IsNullOrEmpty(sourceState)||modelClip!="attack")return previous;
   var s=Generic(kind,npc,struck);if(s==null)return previous;float total=kind=="benders"||kind=="william"?.85f:.52f;
   if(!Finite(remaining)||remaining<=0||remaining>total+.0005f||struck&&remaining>.12f+.0005f||!struck&&remaining<=.12f||!admittedMetadata(s))return previous;
   // Original sourceSpeed is provenance. Full-span retime gives (progress*(D/S))*S=progress*D; never multiply S again.
   float progress=struck?Clamp01((.12f-remaining)/.12f):Clamp01((total-remaining)/(total-.12f));clock=progress*s.duration;return s.alias;
  }
  public static string SelectAlpha(bool originalBirth,bool exactProfile,string kind,string npc,bool visible,bool active,bool dormant,bool passive,bool aiEnabled,float hp,int combat,string sourceState,string modelClip,string previous,string animation,bool sourceLoop,float sourceSpeed,float stateSpan,float npcTime,float previousClock,Func<Spec,bool> admittedMetadata,out float clock){
   clock=previousClock;if(!exactProfile||!Active(originalBirth,visible,active,dormant,passive,aiEnabled,hp,combat,modelClip,previous))return previous;
   var s=Alpha(kind,npc,sourceState,modelClip);if(s==null||sourceLoop||animation!=s.animation||sourceSpeed!=s.speed||!Finite(stateSpan)||Math.Abs(stateSpan-s.stateSpan)>.0005f||!Finite(npcTime)||npcTime<0||npcTime>stateSpan+.0005f||!admittedMetadata(s))return previous;
   float t=npcTime*sourceSpeed;if(!Finite(t)||t>s.duration+.0005f)return previous;clock=Math.Max(0,Math.Min(s.duration,t));return s.alias;
  }
  public static bool TryUi(Spec s,string actualAlias,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){bx=by=dx=dy=0;if(s==null||actualAlias!=s.alias||!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||hp<=0||maxHp<=0)return false;bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;}
 }
}
