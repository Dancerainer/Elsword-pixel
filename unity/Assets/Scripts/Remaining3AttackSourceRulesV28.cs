using System;
namespace ElswordPixel {
 // Candidate attack spine. It uses the existing compiled NPC runner; it is
 // not an 82-state/source-client completion claim. No alias is admitted here.
 public static class Remaining3AttackSourceRulesV28 {
  public sealed class Spec {
   public readonly string kind,npc,state,animation,modelClip,alias;
   public readonly float sourceSpan,speed,stateSpan;public readonly int samples;public readonly bool loop;
   public Spec(string k,string n,string s,string a,string c,string alias,float d,float v,float t,int count,bool repeat){kind=k;npc=n;state=s;animation=a;modelClip=c;this.alias=alias;sourceSpan=d;speed=v;stateSpan=t;samples=count;loop=repeat;}
   public bool Normal {get{return modelClip=="idle"||modelClip=="walk";}}
   public string ResourcePrefix {get{return "Pixel/feedback-v25/started-remaining3-attack13-v28/"+kind+"/attack-atlas-";}}
   public string Evidence {get{return "Root-admitted-Remaining3-attack13-v1|"+kind+"|"+npc+"|"+state+"|"+animation+"|original-state-clock";}}
  }
  static readonly Spec[] specs={
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_WAIT","Wait","idle","source-idle",1.3333333333333333f,1f,1.3333333333333333f,33,true),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_WALK","Walk","walk","source-walk",1.3333333333333333f,1f,1.3333333333333333f,33,true),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_ATTACK_A_READY","SpecialAttackA_Ready","attack","source-attack-a-ready",2.8333333333333335f,1f,2.8333333333333335f,69,false),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_ATTACK_A","SpecialAttackA_Fire","attack","source-attack-a",2.2666666666666666f,1f,2.2666666666666666f,56,false),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_ATTACK_B_READY","DashAttackReady","attack","source-attack-b-ready",1.9666666666666666f,1f,1.9666666666666666f,49,false),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_ATTACK_B","DashAttack","attack","source-attack-b",0.80000000000000004f,1.3999999999999999f,0.57142857142857151f,21,true),
   new Spec("white-giant","NUI_PPORU_GIANT_WHITE","PPORU_GIANT_WHITE_ATTACK_B_END","DashAttackEnd","attack","source-attack-b-end",1.3999999999999999f,1f,1.3999999999999999f,35,false),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_WAIT","Wait","idle","source-idle",1.6666666666666667f,1f,1.6666666666666667f,41,true),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_WALK","Walk","walk","source-walk",2.1333333333333333f,1f,2.1333333333333333f,53,true),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_ATTACK_A_READY","AttackA_Ready","attack","source-attack-a-ready",2.7999999999999998f,1f,2.7999999999999998f,69,false),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_ATTACK_A","AttackA_Fire","attack","source-attack-a",2.7999999999999998f,1f,2.7999999999999998f,69,false),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_ATTACK_B_READY","DashAttackReady","attack","source-attack-b-ready",2.7999999999999998f,1f,2.7999999999999998f,69,false),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_ATTACK_B","DashAttack","attack","source-attack-b",0.96666666666666667f,1f,0.96666666666666667f,25,false),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_SPECIAL_ATTACK_READY","SpecialAttackA_Ready","attack","source-attack-ready",3.4666666666666668f,1f,3.4666666666666668f,85,false),
   new Spec("ancient-phoru","NUI_ANCIENT_PPORU","ANCIENT_PPORU_SPECIAL_ATTACK","SpecialAttackA_Fire","attack","source-attack-special",4.0666666666666664f,1f,4.0666666666666664f,99,false),
   new Spec("toto","NUI_THIEF_THIN_TOTO","THIEF_THIN_TOTO_WAIT","Wait","idle","source-idle",1.3333333333333333f,1f,1.3333333333333333f,33,true),
   new Spec("toto","NUI_THIEF_THIN_TOTO","THIEF_THIN_TOTO_WALK","Walk","walk","source-walk",1.3333333333333333f,1f,1.3333333333333333f,33,true),
   new Spec("toto","NUI_THIEF_THIN_TOTO","THIEF_THIN_TOTO_ATTACK_READY","MagicAttackA_Ready","attack","source-attack-ready",2.6000000000000001f,1.25f,2.0800000000000001f,64,false),
   new Spec("toto","NUI_THIEF_THIN_TOTO","THIEF_THIN_TOTO_ATTACK_A","MagicAttackA_Fire","attack","source-attack-a",1.5333333333333334f,1f,1.5333333333333334f,38,false),
  };
  public static bool Supported(string kind,string npc){return kind=="ancient-phoru"&&npc=="NUI_ANCIENT_PPORU"||kind=="white-giant"&&npc=="NUI_PPORU_GIANT_WHITE"||kind=="toto"&&npc=="NUI_THIEF_THIN_TOTO";}
  public static Spec Find(string kind,string npc,string state){foreach(var s in specs)if(s.kind==kind&&s.npc==npc&&s.state==state)return s;return null;}
  public static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  static bool Near(float a,float b){return Finite(a)&&Math.Abs(a-b)<=.000001f;}
  public static bool SourceProfile(NpcDefinition p){
   if(p==null||!Supported(p.kind,p.id)||p.states==null||p.reactions==null||p.reactions.Length!=0||!string.IsNullOrEmpty(p.getUpState)||!string.IsNullOrEmpty(p.initialState))return false;
   int expected=p.kind=="ancient-phoru"?8:p.kind=="white-giant"?7:4;if(p.states.Length!=expected)return false;
   foreach(var state in p.states){var s=Find(p.kind,p.id,state.id);if(s==null||state.clip!=s.modelClip||state.sourceAnimation!=s.animation||!Near(state.sourceAnimSpeed,s.speed)||!Near(state.duration,s.stateSpan)||state.loop!=s.loop)return false;}
   var wait=Find(p.kind,p.id,p.waitState);var walk=Find(p.kind,p.id,p.walkState);
   return wait!=null&&wait.modelClip=="idle"&&walk!=null&&walk.modelClip=="walk";
  }
  // Original Ancient TARGET_RANGE2200, TARGET_LOST_RANGE1000 are different
  // concepts. Do not silently raise lostRange to satisfy the old >= guard.
  public static bool OriginalSenseLostPair(NpcDefinition p){return p!=null&&p.id=="NUI_ANCIENT_PPORU"&&p.kind=="ancient-phoru"&&Near(p.senseRange,616f)&&Near(p.lostRange,280f);}
  public static bool AllowsKnockdown(NpcDefinition p){return !SourceProfile(p)||p.id!="NUI_ANCIENT_PPORU";}
  public static HitReaction Reaction(NpcDefinition p,HitReaction reaction){return !AllowsKnockdown(p)&&reaction==HitReaction.Knockdown?HitReaction.Hurt:reaction;}
  public static int AuthoredFacing(string kind){return kind=="toto"?-1:1;}
  public static bool Resource(Spec s,string path){if(s==null||path==null||!path.StartsWith(s.ResourcePrefix,StringComparison.Ordinal)||path.Length!=s.ResourcePrefix.Length+2)return false;int i=s.ResourcePrefix.Length;return path[i]>='0'&&path[i]<='9'&&path[i+1]>='0'&&path[i+1]<='9';}

 }
}
