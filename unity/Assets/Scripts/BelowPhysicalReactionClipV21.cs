using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class BelowReactionFileV21 {public string path,sha256;}
 [Serializable] public sealed class BelowPhysicalReactionRowV21 {
  public string npc,kind,physicalStateName,modelClip,sourceState,sourceAnimation,displayClip,reason,timePolicy;
  public int physicalState;public bool nativeEquivalent,libraryLoop;public float libraryDurationSeconds,sourceNativeMotionSeconds,sourceAnimSpeed,sourceEffectivePlaybackSeconds;
  public BelowReactionFileV21 animations,atlas,sourceApprovedLibrary;
 }
 [Serializable] public sealed class BelowPhysicalReactionDocumentV21 {
  public string schema,sourceRulesSha256;public bool sourceNativeEquivalent,runtimeAdmitted;
  public BelowPhysicalReactionRowV21[] rows;public BelowReactionFileV21 actualReproductionExecution;
 }
 // Explicit display adaptations for source-state-empty generic physics only.
 // No new animation, native reaction identity, source clock or model edit.
 public sealed class BelowPhysicalReactionClipV21 {
  readonly Dictionary<string,BelowPhysicalReactionRowV21> rows=new Dictionary<string,BelowPhysicalReactionRowV21>(StringComparer.Ordinal);
  static string Key(string npc,string kind,int state,string clip){return npc+"/"+kind+"/"+state+"/"+clip;}
  public BelowPhysicalReactionClipV21(BelowPhysicalReactionDocumentV21 document,NpcDefinition[] rules,string rulesSha){
   if(document==null||document.schema!="below-unbound-physical-reaction-display-policy-v21"||document.sourceRulesSha256!=rulesSha||document.sourceNativeEquivalent||document.runtimeAdmitted||document.rows==null||document.rows.Length!=36)throw new ArgumentException("Below exact bounded physical reaction policy rejected");
   foreach(var row in document.rows){
    var p=Array.Find(rules,x=>x.id==row.npc&&x.kind==row.kind);var state=p==null?null:Array.Find(p.states,x=>x.id==row.sourceState);
    bool physical=row.physicalState==(int)CombatState.Airborne&&(row.modelClip=="launched"||row.modelClip=="fall")||row.physicalState==(int)CombatState.Knockdown&&row.modelClip=="knockdown"||row.physicalState==(int)CombatState.GettingUp&&row.modelClip=="getup"||row.physicalState==(int)CombatState.Dead&&row.modelClip=="death";
    if(state==null||!physical||state.clip!=row.displayClip||state.sourceAnimation!=row.sourceAnimation||row.nativeEquivalent||row.timePolicy!="play-approved-clip-once-then-hold-final-frame"||string.IsNullOrEmpty(row.reason)||row.libraryDurationSeconds<=0)throw new ArgumentException("Below undeclared physical reaction/source display row");
    rows.Add(Key(row.npc,row.kind,row.physicalState,row.modelClip),row);
   }
  }
  public bool TryResolve(Enemy enemy,out BelowPhysicalReactionRowV21 row){row=null;return enemy!=null&&string.IsNullOrEmpty(enemy.npcState)&&rows.TryGetValue(Key(enemy.sourceNPC,enemy.kind,(int)enemy.state,enemy.clip),out row);}
  public string Resolve(Enemy enemy,BelowSourceStateClipV21 source,bool nonLoopOverrideLoaded,Func<string,bool> hasClip,out bool hold){
   hold=false;if(enemy==null||source==null||hasClip==null)throw new ArgumentNullException("Below physical presentation input");
   if(!string.IsNullOrEmpty(enemy.npcState))return source.Resolve(enemy,nonLoopOverrideLoaded);
   string clip;if(source.TryDeclaredReactionClip(enemy,out clip)){hold=true;return clip;}
   if(hasClip(enemy.clip))return enemy.clip;
   BelowPhysicalReactionRowV21 row;if(!TryResolve(enemy,out row))throw new InvalidOperationException("Below unknown unbound physical reaction "+enemy.sourceNPC+"/"+enemy.state+"/"+enemy.clip);
   hold=true;return row.displayClip;
  }
  public string ResolveFallingLookupV21(Enemy enemy,BelowSourceStateClipV21 source,Func<string,bool> hasClip,out bool hold){
   hold=false;if(enemy==null||source==null||hasClip==null)throw new ArgumentNullException("Below falling lookup inputs");
   if(!string.IsNullOrEmpty(enemy.npcState))return source.Resolve(enemy,true);
   if(enemy.state!=CombatState.Airborne||enemy.clip!="jump"||!(enemy.vy<=0))throw new InvalidOperationException("Below falling lookup physical direction rejected");
   const string lookupClip="fall";if(hasClip(lookupClip))return lookupClip;
   BelowPhysicalReactionRowV21 row;if(!rows.TryGetValue(Key(enemy.sourceNPC,enemy.kind,(int)enemy.state,lookupClip),out row))throw new InvalidOperationException("Below unknown normalized fall tuple "+enemy.sourceNPC+"/"+enemy.kind);
   hold=true;return row.displayClip;
  }
  public static float OnceThenHoldClock(float clock,float duration,float fps,int frameCount){
   if(float.IsNaN(clock)||float.IsInfinity(clock)||duration<=0||fps<=0||frameCount<1)throw new ArgumentException("Below physical reaction display clock invalid");
   // Sample inside the final exposure, strictly below duration, even when the
   // approved source library is a looping static Wait. Pose must not Repeat it.
   float last=Math.Min(duration*.999999f,(frameCount-.5f)/fps);return Math.Max(0,Math.Min(clock,last));
  }
 }
}
