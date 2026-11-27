using System;
namespace ElswordPixel {
 public static class Next3CombatNativeRulesV28 {
  public sealed class Entry {
   public readonly string kind,npc,alias,modelClip,sourceState,resource;
   public readonly float duration,speed;public readonly int frames;public readonly bool attack;
   public Entry(string k,string n,string a,string m,string s,float d,int f,float v,string r,bool t){kind=k;npc=n;alias=a;modelClip=m;sourceState=s;duration=d;frames=f;speed=v;resource=r;attack=t;}
  }
  static readonly Entry[] entries={
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-hurt","hurt","",0.90000000000000002f,23,1f,"Pixel/feedback-v25/started-reaction-v28/benders-upgrade-normal/reaction-atlas",false),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-down","down","",1.1333333333333333f,29,1f,"Pixel/feedback-v25/started-reaction-v28/benders-upgrade-normal/reaction-atlas",false),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-get-up","get-up","",1.1333333333333333f,29,1f,"Pixel/feedback-v25/started-reaction-v28/benders-upgrade-normal/reaction-atlas",false),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-death","death","",1.1333333333333333f,29,1f,"Pixel/feedback-v25/started-reaction-v28/benders-upgrade-normal/reaction-atlas",false),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-attack-a-ready","attack-a-ready","BENDERS_UPGRADE_NORMAL_ATTACK_A_READY",2.7000000000000002f,66,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-attack-a","attack-a","BENDERS_UPGRADE_NORMAL_ATTACK_A",2.2666666666666666f,56,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-attack-b-ready","attack-b-ready","BENDERS_UPGRADE_NORMAL_ATTACK_B_READY",2.7000000000000002f,66,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-attack-b","attack-b","BENDERS_UPGRADE_NORMAL_ATTACK_B",3.4333333333333331f,84,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-attack-special-ready","attack-special-ready","BENDERS_UPGRADE_NORMAL_SPECIAL_ATTACK_A_READY",2.6000000000000001f,64,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-attack-special","attack-special","BENDERS_UPGRADE_NORMAL_SPECIAL_ATTACK_A",2.0333333333333332f,50,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("benders-upgrade-normal","NUI_BENDERS_UPGRADE_NORMAL","source-summon","summon","BENDERS_UPGRADE_NORMAL_SUMMON_MONSTER",2.0666666666666669f,51,1f,"Pixel/feedback-v25/started-attack-v28/benders-upgrade-normal/attack-atlas",true),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","source-hurt","hurt","",0.83333333333333337f,21,1f,"Pixel/feedback-v25/started-reaction-v28/thief-fat-boss/reaction-atlas",false),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","source-down","down","",1.3999999999999999f,35,1f,"Pixel/feedback-v25/started-reaction-v28/thief-fat-boss/reaction-atlas",false),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","source-get-up","get-up","",1.3f,33,1f,"Pixel/feedback-v25/started-reaction-v28/thief-fat-boss/reaction-atlas",false),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","source-death","death","",1.3999999999999999f,35,1f,"Pixel/feedback-v25/started-reaction-v28/thief-fat-boss/reaction-atlas",false),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","source-attack-a-ready","attack-a-ready","THIEF_FAT_BOSS_ATTACK_A_READY",2.7000000000000002f,66,1f,"Pixel/feedback-v25/started-attack-v28/thief-fat-boss/attack-atlas",true),
   new Entry("thief-fat-boss","NUI_THIEF_FAT_BOSS","source-attack-a","attack-a","THIEF_FAT_BOSS_ATTACK_A",2.0666666666666669f,51,1f,"Pixel/feedback-v25/started-attack-v28/thief-fat-boss/attack-atlas",true),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","source-hurt","hurt","",0.8666666666666667f,22,1f,"Pixel/feedback-v25/started-reaction-v28/forest-dark-magician/reaction-atlas",false),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","source-down","down","",1.5333333333333334f,38,1f,"Pixel/feedback-v25/started-reaction-v28/forest-dark-magician/reaction-atlas",false),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","source-get-up","get-up","",1.5333333333333334f,38,1f,"Pixel/feedback-v25/started-reaction-v28/forest-dark-magician/reaction-atlas",false),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","source-death","death","",1.5333333333333334f,38,1f,"Pixel/feedback-v25/started-reaction-v28/forest-dark-magician/reaction-atlas",false),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","source-attack-a-ready","attack-a-ready","FOREST_DARK_MAGICIAN_ATTACK_A_READY",3.3666666666666667f,82,1.25f,"Pixel/feedback-v25/started-attack-v28/forest-dark-magician/attack-atlas",true),
   new Entry("forest-dark-magician","NUI_FOREST_DARK_MAGICIAN","source-attack-a","attack-a","FOREST_DARK_MAGICIAN_ATTACK_A",1.5666666666666667f,39,1f,"Pixel/feedback-v25/started-attack-v28/forest-dark-magician/attack-atlas",true),
  };
  public static string Canonical(string clip){return clip=="knockdown"?"down":clip=="getup"?"get-up":clip;}
  public static Entry Find(string kind,string alias){foreach(var e in entries)if(e.kind==kind&&e.alias==alias)return e;return null;}
  public static bool Alias(string kind,string clip){return Find(kind,clip)!=null;}
  public static bool Scope(int dungeon,string graph,bool baseline,string content,string kind,string npc){return Next3NormalNativeRulesV28.Scope(dungeon,graph,baseline,content,kind,npc);}
  public static bool Eligible(int dungeon,string graph,bool baseline,string content,string kind,string npc,bool active,bool dormant,bool passive,bool sourceVisible,float hp,int combat){return Scope(dungeon,graph,baseline,content,kind,npc)&&active&&!dormant&&!passive&&sourceVisible&&(hp>0||combat==5&&hp==0);}
  public static string Select(int dungeon,string graph,bool baseline,string content,string kind,string npc,bool active,bool dormant,bool passive,bool sourceVisible,bool aiEnabled,float hp,int combat,string state,string previous,float npcTime,float actorClock,float recoveryTime,float recoveryDuration,float previousClock,Func<string,bool> valid,out float clock){
   clock=previousClock;
   if(!Eligible(dungeon,graph,baseline,content,kind,npc,active,dormant,passive,sourceVisible,hp,combat))return previous;
   string model=Canonical(previous);Entry target=null;
   foreach(var r in entries){if(r.kind!=kind||r.npc!=npc||r.modelClip!=model)continue;
    if(r.attack){if(combat==0&&aiEnabled&&state==r.sourceState)target=r;}
    else if(combat==2&&model=="hurt"||combat==3&&model=="down"||combat==4&&model=="get-up"||combat==5&&model=="death")target=r;
   }
   if(target==null)return previous;
   if(!valid(target.alias))throw new InvalidOperationException("Next3 combat native metadata mismatch: "+kind+"/"+target.alias);
   if(target.attack)clock=npcTime*target.speed;
   else if(kind=="forest-dark-magician"&&model=="get-up"){
    if(!Finite(recoveryDuration)||recoveryDuration<=0||!Finite(recoveryTime))throw new InvalidOperationException("Invalid original Mage recovery phase");
    clock=Math.Max(0,Math.Min(1,1-recoveryTime/recoveryDuration))*target.duration;
   }else clock=actorClock;
   if(!Finite(clock)||clock<0)throw new InvalidOperationException("Invalid Next3 combat native display clock");
   return target.alias;
  }
  public static bool Finite(float n){return !float.IsNaN(n)&&!float.IsInfinity(n);}
  public static bool ValidateMetadata(string kind,string alias,string texture,float fps,float duration,bool loop,float distance,int frames,int hits){var e=Find(kind,alias);return e!=null&&texture==e.resource&&fps==24&&Finite(duration)&&Math.Abs(duration-e.duration)<=.000001f&&!loop&&distance==0&&frames==e.frames&&hits==0;}
  public static float EffectiveScale(int dungeon,string graph,bool baseline,string content,string kind,string npc,string alias,float previous,float visualScale){
   if(!Scope(dungeon,graph,baseline,content,kind,npc)||kind!="thief-fat-boss"||!Alias(kind,alias))return previous;
   if(!Finite(visualScale)||visualScale<=0)throw new InvalidOperationException("Invalid Next3 combat visualScale");return visualScale;
  }
  public static bool TryUiAnchor(int dungeon,string graph,bool baseline,string content,string kind,string npc,string alias,float center,float top,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(!Scope(dungeon,graph,baseline,content,kind,npc)||!Alias(kind,alias))return false;
   if(!Finite(center)||!Finite(top)||!Finite(width)||width<=0||!Finite(hp)||!Finite(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid native combat visible bounds/HP");
   bx=center-width*(1-hp/maxHp)/2;by=top+8;dx=center;dy=top+20;return true;
  }
 }
}
