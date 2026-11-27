using System;
using System.Collections.Generic;

namespace ElswordPixel {
 // Display projection for the nine Core whole-body libraries. Generic physical
 // reactions deliberately have no npcState; that does not mean no body exists.
 // This immutable table never enters an NPC state or changes combat clocks.
 public sealed class CoreReactionPoseEntryV32 {
  public readonly string Npc,Kind,State,Motion,Clip,Role;
  public readonly float Duration,Speed;
  internal CoreReactionPoseEntryV32(NpcDefinition profile,NpcStateDefinition state,string role){Npc=profile.id;Kind=profile.kind;State=state.id;Motion=state.sourceAnimation;Clip=state.clip;Duration=state.duration;Speed=state.sourceAnimSpeed;Role=role;}
 }
 public sealed class CoreReactionPoseMapV32 {
  static readonly string[] Npcs={"NUI_NASOD_ELITE_GUARD","NUI_NASOD_HEALER_BLADER","NUI_NASOD_ELEMENT_FIRE_B","NUI_NASOD_ELEMENT_ICE_B","NUI_RAVEN_CLONE_B","NUI_WALLY_9TH_TYPE2","NUI_NASOD_SPRAY_ICE","NUI_NASOD_ELITE_SHERIFF","NUI_NASOD_WATCH"};
  static readonly string[] Kinds={"nasod-elite-guard","nasod-healer-blader","nasod-element-fire-b","nasod-element-ice-b","raven-clone-b","wally-9th-type2","nasod-spray-ice","nasod-elite-sheriff","nasod-watch"};
  static readonly string[] Prefixes={"NASOD_ELITE_GUARD","NASOD_HEALER_BLADER","NASOD_ELEMENT_FIRE_SOLO_B","NASOD_ELEMENT_ICE_SOLO_B","RAVEN_CLONE_B","WALLY_9TH_TYPE2","NASOD_SPRAY_ICE","NASOD_ELITE_SHERIFF","NASOD_WATCH"};
  readonly Dictionary<string,Dictionary<string,CoreReactionPoseEntryV32>> entries=new Dictionary<string,Dictionary<string,CoreReactionPoseEntryV32>>(StringComparer.Ordinal);
  readonly List<CoreReactionPoseEntryV32> ordered=new List<CoreReactionPoseEntryV32>();
  public CoreReactionPoseEntryV32[] Entries(){return ordered.ToArray();}
  static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  void Add(NpcDefinition p,Dictionary<string,CoreReactionPoseEntryV32> rows,string role,string stateId){
   var state=Array.Find(p.states,s=>s!=null&&s.id==stateId);
   if(state==null||state.loop||!Finite(state.duration)||state.duration<=0||!Finite(state.sourceAnimSpeed)||state.sourceAnimSpeed<=0||string.IsNullOrEmpty(state.sourceAnimation)||rows.ContainsKey(role))throw new ArgumentException("Core physical reaction has no unique existing non-loop body: "+p.id+"/"+role);
   var entry=new CoreReactionPoseEntryV32(p,state,role);rows.Add(role,entry);ordered.Add(entry);
  }
  public CoreReactionPoseMapV32(NpcDefinition[] profiles){
   if(profiles==null)throw new ArgumentNullException("profiles");
   for(int i=0;i<Npcs.Length;i++){
    var p=Array.Find(profiles,x=>x!=null&&x.id==Npcs[i]);
    if(p==null||p.kind!=Kinds[i]||p.states==null||p.reactions==null||p.reactions.Length!=(i==0?1:3))throw new ArgumentException("Core whole reaction identity/count changed: "+Npcs[i]);
    var rows=new Dictionary<string,CoreReactionPoseEntryV32>(StringComparer.Ordinal);
    foreach(var reaction in p.reactions){
     if(reaction==null||(reaction.combatState!=CombatState.Hurt&&reaction.combatState!=CombatState.Knockdown&&reaction.combatState!=CombatState.GettingUp))throw new ArgumentException("Unsupported Core declared reaction");
     Add(p,rows,reaction.combatState.ToString(),reaction.sourceState);var entry=rows[reaction.combatState.ToString()];
     if(entry.Motion!=reaction.sourceAnimation||entry.Duration!=reaction.duration||entry.Speed!=reaction.sourceAnimSpeed)throw new ArgumentException("Core body reaction clock differs from original collision reaction");
    }
    if(i==0){
     // Elite Guard has only a declared stand-up reaction. These are explicit
     // display adapters to its own existing hit/down bodies, not new FSM rules.
     Add(p,rows,"Hurt",Prefixes[i]+"_DAMAGE_BIG");
     Add(p,rows,"Knockdown",Prefixes[i]+"_DAMAGE_DOWN_BACK");
    }
    Add(p,rows,"Launch",Prefixes[i]+(i==0?"_DAMAGE_FLY_BACK":"_DAMAGE_FLY_FRONT"));
    Add(p,rows,"Fall",Prefixes[i]+"_DAMAGE_AIR_FALL");
    Add(p,rows,"DeadGround",Prefixes[i]+"_DYING_LAND_FRONT");
    Add(p,rows,"DeadAir",Prefixes[i]+"_DYING_SKY");
    entries.Add(p.id,rows);
   }
   if(ordered.Count!=63)throw new ArgumentException("Core nine identities require seven physical display roles each");
  }
  public bool TrySelect(Enemy actor,out string state,out float time,out bool physical){
   state=null;time=0;physical=false;if(actor==null)return false;
   if(!string.IsNullOrEmpty(actor.npcState)){state=actor.npcState;time=actor.npcTime;return true;}
   Dictionary<string,CoreReactionPoseEntryV32> rows;CoreReactionPoseEntryV32 entry;
   if(!entries.TryGetValue(actor.sourceNPC??"",out rows))return false;
   string role=null;
   if(actor.hp<=0&&actor.state==CombatState.Dead)role=actor.knockdownOnLanding?"DeadAir":"DeadGround";
   else if(actor.hp>0){
    if(actor.state==CombatState.Hurt||actor.state==CombatState.Knockdown||actor.state==CombatState.GettingUp)role=actor.state.ToString();
    else if(actor.state==CombatState.Airborne&&(actor.clip=="launched"||actor.knockdownOnLanding))role=actor.clip=="fall"?"Fall":"Launch";
   }
   if(role==null||!rows.TryGetValue(role,out entry)||actor.kind!=entry.Kind)return false;
   if(!Finite(actor.clock))throw new ArgumentException("Nonfinite Core physical reaction display clock");
   state=entry.State;time=Math.Max(0,Math.Min(actor.clock,entry.Duration));physical=true;return true;
  }
 }
}
