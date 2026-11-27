using System;
using System.Collections.Generic;

namespace ElswordPixel {
 // Immutable presentation projection of the ORIGINAL eleven-profile rules.
 // Fourteen compiled reactions, two Guard original INIT_STATE ground defaults,
 // and four exact source FLY_DAMAGE_FRONT display defaults for real force-fly.
 // Generic reactions do not retain native attacker-side selection: use the
 // original LAND_FRONT default without changing combat physics.
 // The three additional Proto reactions remain with its separate overlay.
 public sealed class FactoryReactionPoseEntryR1467 {
  public readonly string Npc,State,Motion,Clip;public readonly CombatState Combat;
  public readonly float Duration,Speed;
  internal FactoryReactionPoseEntryR1467(string npc,NpcReactionBody reaction,NpcStateDefinition source){Npc=npc;State=reaction.sourceState;Motion=reaction.sourceAnimation;Clip=source.clip;Combat=reaction.combatState;Duration=reaction.duration;Speed=reaction.sourceAnimSpeed;}
  internal FactoryReactionPoseEntryR1467(string npc,CombatState combat,NpcStateDefinition source){Npc=npc;State=source.id;Motion=source.sourceAnimation;Clip=source.clip;Combat=combat;Duration=source.duration;Speed=source.sourceAnimSpeed;}
 }
 public sealed class FactoryReactionPoseMapR1467 {
  static readonly string[] Npcs={"NUI_BOX_ALTERA","NUI_FACTORY_PRESS","NUI_FACTORY_PRESS3","NUI_NASOD_GUARD","NUI_NASOD_HEALER_BLADER","NUI_NASOD_MAKER","NUI_NASOD_SHERIFF","NUI_RAVEN_CLONE","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP","NUI_WALLY_9TH_TYPE2"};
  static readonly int[] Counts={0,0,0,1,3,1,3,3,0,0,3};
  readonly Dictionary<string,Dictionary<CombatState,FactoryReactionPoseEntryR1467>> entries=new Dictionary<string,Dictionary<CombatState,FactoryReactionPoseEntryR1467>>(StringComparer.Ordinal);
  readonly List<FactoryReactionPoseEntryR1467> ordered=new List<FactoryReactionPoseEntryR1467>();
  readonly FactoryReactionPoseEntryR1467 sheriffDeadGround,sheriffDeadAir,guardDeadGround,guardDeadAir,ravenDeadGround,ravenDeadAir;
  public FactoryReactionPoseEntryR1467[] Entries(){return ordered.ToArray();}
  public FactoryReactionPoseMapR1467(NpcDefinition[] originalElevenProfiles){
   if(originalElevenProfiles==null||originalElevenProfiles.Length!=11)throw new ArgumentException("Original eleven Factory profiles required for reaction display");
   int originalDeclared=0;for(int i=0;i<Npcs.Length;i++){
    var p=originalElevenProfiles[i];if(p==null||p.id!=Npcs[i]||p.reactions==null||p.reactions.Length!=Counts[i]||p.states==null)throw new ArgumentException("Original Factory reaction identity/count differs");
    var states=new Dictionary<CombatState,FactoryReactionPoseEntryR1467>();
    foreach(var reaction in p.reactions){
     if(reaction==null)throw new ArgumentException("Missing original Factory reaction row");
     var source=Array.Find(p.states,s=>s!=null&&s.id==reaction.sourceState);
     if(reaction==null||states.ContainsKey(reaction.combatState)||(reaction.combatState!=CombatState.Hurt&&reaction.combatState!=CombatState.Knockdown&&reaction.combatState!=CombatState.GettingUp)||source==null||source.loop||source.sourceAnimation!=reaction.sourceAnimation||source.duration!=reaction.duration||source.sourceAnimSpeed!=reaction.sourceAnimSpeed||reaction.duration<=0||reaction.sourceAnimSpeed<=0||float.IsNaN(reaction.duration)||float.IsInfinity(reaction.duration))throw new ArgumentException("Factory reaction is not a unique existing original non-loop source state");
     var entry=new FactoryReactionPoseEntryR1467(p.id,reaction,source);states.Add(entry.Combat,entry);ordered.Add(entry);originalDeclared++;
    }
    if(p.id=="NUI_NASOD_GUARD"){
     // Source INIT_STATE maps SMALL/BIG_DAMAGE_LAND_FRONT and
     // DOWN_DAMAGE_LAND_FRONT to DAMAGE_BIG (real motion DamageBack).
     // This display alias does not add body samples or alter model durations.
     var source=Array.Find(p.states,q=>q!=null&&q.id=="NASOD_GUARD_DAMAGE_BIG");
     if(source==null||source.sourceAnimation!="DamageBack"||source.clip!="hurt-back"||source.loop||source.sourceAnimSpeed!=1||Math.Abs(source.duration-7f/6f)>.000001f||source.nextState!="NASOD_GUARD_WAIT")throw new ArgumentException("Guard original INIT_STATE damage display state differs");
     foreach(var combat in new[]{CombatState.Hurt,CombatState.Knockdown}){if(states.ContainsKey(combat))throw new ArgumentException("Guard display default must not replace a declared reaction");var entry=new FactoryReactionPoseEntryR1467(p.id,combat,source);states.Add(combat,entry);ordered.Add(entry);}
    }
    if(p.id=="NUI_WALLY_9TH_TYPE2"||p.id=="NUI_NASOD_HEALER_BLADER"||p.id=="NUI_NASOD_GUARD"){
     // Exact original INIT_STATE.FLY_DAMAGE_FRONT. Current generic combat has
     // no retained native front/back argument, so this is the explicit front
     // display default; it neither changes motion physics nor enters an FSM.
     bool wally=p.id=="NUI_WALLY_9TH_TYPE2",healer=p.id=="NUI_NASOD_HEALER_BLADER";
     string id=wally?"WALLY_9TH_TYPE2_DAMAGE_FLY_FRONT":healer?"NASOD_HEALER_BLADER_DAMAGE_FLY_FRONT":"NASOD_GUARD_DAMAGE_BIG";
     string motion=wally?"DamageBig":healer?"DamageAirFall":"DamageBack",clip=wally?"hurt-big":healer?"fall":"hurt-back";float duration=wally?41f/30f:healer?1f:7f/6f;
     var source=Array.Find(p.states,q=>q!=null&&q.id==id);
     if(source==null||source.sourceAnimation!=motion||source.clip!=clip||source.loop||source.sourceAnimSpeed!=1||Math.Abs(source.duration-duration)>.000001f||states.ContainsKey(CombatState.Airborne))throw new ArgumentException("Original Factory FLY_DAMAGE_FRONT display state differs");
     var entry=new FactoryReactionPoseEntryR1467(p.id,CombatState.Airborne,source);states.Add(entry.Combat,entry);ordered.Add(entry);
    }
    if(p.id=="NUI_NASOD_SHERIFF"){
     // Exact original INIT_STATE.FLY_DAMAGE_FRONT. Generic force-fly has no
     // retained attacker-side subtype: use this original front display state
     // without entering its FSM, resetting clock, or altering physical landing.
     var source=Array.Find(p.states,q=>q!=null&&q.id=="NASOD_SHERIFF_DAMAGE_FLY_FRONT");
     if(source==null||source.sourceAnimation!="DamageAirFlyFront"||source.clip!="launch-front"||source.loop||source.sourceAnimSpeed!=1||Math.Abs(source.duration-1f)>.000001f||source.nextState!="NASOD_SHERIFF_WAIT"||!source.holdForSourceEvent||source.sourceEventTransitions==null||source.sourceEventTransitions.Length!=1||source.sourceEventTransitions[0].condition!="SCT_FOOT_ON_LINE_TRUE"||source.sourceEventTransitions[0].nextState!="NASOD_SHERIFF_DAMAGE_DOWN_FRONT"||states.ContainsKey(CombatState.Airborne))throw new ArgumentException("Sheriff original INIT_STATE force-fly display state differs");
     var entry=new FactoryReactionPoseEntryR1467(p.id,CombatState.Airborne,source);states.Add(entry.Combat,entry);ordered.Add(entry);
    }
    if(p.id=="NUI_NASOD_SHERIFF"){
     // Original INIT_STATE DYING_LAND_FRONT / DYING_SKY. Generic death retains
     // force-fly landing intent but no attacker-side front/back selector. Use
     // the original front default on ground, as the existing Core adapter does.
     foreach(bool air in new[]{false,true}){
      string id=air?"NASOD_SHERIFF_DYING_SKY":"NASOD_SHERIFF_DYING_LAND_FRONT";
      var source=Array.Find(p.states,q=>q!=null&&q.id==id);
      if(source==null||source.sourceAnimation!=(air?"DamageAirDownLanding":"DamageDownFront")||source.clip!=(air?"air-landing":"down")||source.loop||source.sourceAnimSpeed!=1||Math.Abs(source.duration-(air?1.5f:1.7f))>.000001f||source.invincibleStart!=0||source.invincibleEnd!=100)throw new ArgumentException("Sheriff original INIT_STATE death display state differs");
      var entry=new FactoryReactionPoseEntryR1467(p.id,CombatState.Dead,source);if(air)sheriffDeadAir=entry;else sheriffDeadGround=entry;ordered.Add(entry);
     }
    }
    if(p.id=="NUI_NASOD_GUARD"){
     // Original ordinary Guard INIT_STATE death defaults. The front/back land
     // states share DamageDownBack and duration. Use the original front default
     // when the generic death has no stored attacker-side argument.
     foreach(bool air in new[]{false,true}){
      string id=air?"NASOD_GUARD_DYING_SKY":"NASOD_GUARD_DYING_LAND_FRONT";
      var source=Array.Find(p.states,q=>q!=null&&q.id==id);
      if(source==null||source.sourceAnimation!=(air?"DamageAirDownLanding":"DamageDownBack")||source.clip!=(air?"air-landing":"down-back")||source.loop||source.sourceAnimSpeed!=1||Math.Abs(source.duration-(air?22f/15f:1.6f))>.000001f||source.invincibleStart!=0||source.invincibleEnd!=100)throw new ArgumentException("Guard original INIT_STATE death display state differs");
      var entry=new FactoryReactionPoseEntryR1467(p.id,CombatState.Dead,source);if(air)guardDeadAir=entry;else guardDeadGround=entry;ordered.Add(entry);
     }
    }
    if(p.id=="NUI_RAVEN_CLONE"){
     var fly=Array.Find(p.states,q=>q!=null&&q.id=="RAVEN_CLONE_DAMAGE_FLY_FRONT");
     if(fly==null||fly.sourceAnimation!="DamageAirFlyFront"||fly.clip!="launch-front"||fly.loop||
       fly.sourceAnimSpeed!=1||Math.Abs(fly.duration-1f/3)>.000001f||states.ContainsKey(CombatState.Airborne))
       throw new ArgumentException("Factory Raven original force-fly front display");
     var airborne=new FactoryReactionPoseEntryR1467(p.id,CombatState.Airborne,fly);
     states.Add(airborne.Combat,airborne);ordered.Add(airborne);
     foreach(bool air in new[]{false,true}){
      string id=air?"RAVEN_CLONE_DYING_SKY":"RAVEN_CLONE_DYING_LAND_FRONT";
      var source=Array.Find(p.states,q=>q!=null&&q.id==id);
      if(source==null||source.sourceAnimation!=(air?"DamageAirDownLanding":"DamageDownFront")||source.clip!=(air?"air-landing":"down")||
        source.loop||source.sourceAnimSpeed!=1||Math.Abs(source.duration-(air?1.5f:38f/30f))>.000001f||
        source.invincibleStart!=0||source.invincibleEnd!=100)throw new ArgumentException("Factory Raven original death display");
      var death=new FactoryReactionPoseEntryR1467(p.id,CombatState.Dead,source);
      if(air)ravenDeadAir=death;else ravenDeadGround=death;ordered.Add(death);
     }
    }
    entries.Add(p.id,states);
   }
   if(originalDeclared!=14||ordered.Count!=27)throw new ArgumentException("Original fourteen reactions plus two Guard ground, five force-fly and six Sheriff/Guard/Raven death display defaults required");
  }
  // This is the same model-owned reaction clock used by NpcHurtSpheres. It
  // returns display values only: no EnterNpcState, flags or Enemy writes.
  public bool TrySelect(Enemy actor,out string state,out float time,out bool reaction){
   state=null;time=0;reaction=false;if(actor==null)return false;
   if(!string.IsNullOrEmpty(actor.npcState)){state=actor.npcState;time=actor.npcTime;return true;}
   if(actor.hp<=0){
    if(actor.state!=CombatState.Dead)return false;
    FactoryReactionPoseEntryR1467 death;
    if(actor.sourceNPC=="NUI_NASOD_SHERIFF"&&actor.kind=="nasod-sheriff")death=actor.knockdownOnLanding?sheriffDeadAir:sheriffDeadGround;
    else if(actor.sourceNPC=="NUI_NASOD_GUARD"&&actor.kind=="nasod-guard")death=actor.knockdownOnLanding?guardDeadAir:guardDeadGround;
    else if(actor.sourceNPC=="NUI_RAVEN_CLONE"&&actor.kind=="raven-clone")death=actor.knockdownOnLanding?ravenDeadAir:ravenDeadGround;
    else return false;
    if(death==null||float.IsNaN(actor.clock)||float.IsInfinity(actor.clock))throw new ArgumentException("Invalid original Factory death display clock");
    // Keep the model's existing death-reset clock in source seconds. The caller
    // still owns alpha=max(0,1-deathTime/1.2); no stretched motion or new timer.
    state=death.State;time=Math.Max(0,Math.Min(actor.clock,death.Duration));reaction=true;return true;
   }
   if(actor.state==CombatState.Airborne&&actor.clip!="launched"&&!actor.knockdownOnLanding)return false;Dictionary<CombatState,FactoryReactionPoseEntryR1467> byState;FactoryReactionPoseEntryR1467 entry;
   if(!entries.TryGetValue(actor.sourceNPC??"",out byState)||!byState.TryGetValue(actor.state,out entry))return false;
   if(float.IsNaN(actor.clock)||float.IsInfinity(actor.clock))throw new ArgumentException("Nonfinite actual Factory reaction clock");
   state=entry.State;time=Math.Max(0,Math.Min(actor.clock,entry.Duration));reaction=true;return true;
  }
 }
 public sealed partial class DemoScene {
  FactoryReactionPoseMapR1467 factoryReactionPoseMapR1467;
  readonly HashSet<string> factoryReactionPoseGapsR1467=new HashSet<string>(StringComparer.Ordinal);
  public int FactoryReactionPoseSelectionsR1467 {get;private set;}
  public string[] ReadFactoryReactionPoseGapsR1467(){var rows=new List<string>(factoryReactionPoseGapsR1467);rows.Sort(StringComparer.Ordinal);return rows.ToArray();}
  void ConfigureFactoryReactionDisplayR1467(NpcDefinition[] originalElevenProfiles){
   FactoryProductNeedR1179(factoryReactionPoseMapR1467==null&&factoryProductReferenceR1179.Count==9&&factoryProductSpritesR1179.Count==418&&factoryProductWallyR1179!=null&&factoryProductHealerR1179!=null,"Original eleven banks must precede reaction display configuration");
   var map=new FactoryReactionPoseMapR1467(originalElevenProfiles);
   foreach(var entry in map.Entries()){
    if(entry.Npc=="NUI_WALLY_9TH_TYPE2"){
     var visual=factoryProductWallyR1179.State(entry.State);FactoryProductNeedR1179(visual.Clip==entry.Clip&&visual.Motion==entry.Motion&&visual.Duration==entry.Duration&&visual.Speed==entry.Speed&&!visual.Loop,"Original Wally reaction pose binding differs");
    }else{
     var lookup=entry.Npc=="NUI_NASOD_HEALER_BLADER"?factoryProductHealerR1179:factoryProductReferenceR1179[entry.Npc];var visual=lookup.State(entry.State);
     FactoryProductNeedR1179(visual.ModelClip==entry.Clip&&visual.Motion==entry.Motion&&visual.StateDuration==entry.Duration&&visual.Speed==entry.Speed&&!visual.Loop,"Original Factory reaction pose binding differs: "+entry.Npc+"/"+entry.State);
    }
   }
   factoryReactionPoseMapR1467=map;factoryReactionPoseGapsR1467.Clear();FactoryReactionPoseSelectionsR1467=0;
  }
  bool TryFactoryReactionDisplayR1467(Enemy actor,out string displayState,out float displayTime){
   FactoryProductNeedR1179(factoryReactionPoseMapR1467!=null,"Original Factory reaction display is not configured");
   if(game.TryFactoryHealerLethalPoseV35(actor,out displayState,out displayTime)){
    var state=factoryProductHealerR1179.State(displayState);
    bool sky=displayState=="NASOD_HEALER_BLADER_DYING_SKY";
    FactoryProductNeedR1179(state.Motion==(sky?"DamageAirDownLanding":"DamageDown") && state.ModelClip==(sky?"air-landing":"source-damage-down") &&
     !state.Loop && state.Speed==1 && Math.Abs(state.StateDuration-(sky?52f/30f:68f/30f))<.00001f,"Own Factory Healer lethal whole source binding");
    FactoryReactionPoseSelectionsR1467++;return true;
   }
   if(TryOriginalWholeReactionV45(actor,out displayState,out displayTime)){FactoryReactionPoseSelectionsR1467++;factoryVisibleHurtPoseV44.Remember(actor,displayState,displayTime);return true;}
   bool reaction;if(factoryReactionPoseMapR1467.TrySelect(actor,out displayState,out displayTime,out reaction)){if(reaction)FactoryReactionPoseSelectionsR1467++;factoryVisibleHurtPoseV44.Remember(actor,displayState,displayTime);return true;}
   // The presses have an independent source owner, even during a player hit.
   if(actor!=null&&actor.sourceNPC!="NUI_FACTORY_PRESS"&&actor.sourceNPC!="NUI_FACTORY_PRESS3"&&factoryVisibleHurtPoseV44.TryHold(actor,out displayState,out displayTime))return true;
   if(actor!=null&&actor.hp>0&&actor.state!=CombatState.Normal&&string.IsNullOrEmpty(actor.npcState))factoryReactionPoseGapsR1467.Add(actor.sourceNPC+"/"+actor.state);
   return false;
  }
  readonly VisibleHurtPoseV44 factoryVisibleHurtPoseV44=new VisibleHurtPoseV44();
  void ReleaseFactoryReactionDisplayR1467(){factoryReactionPoseMapR1467=null;factoryReactionPoseGapsR1467.Clear();FactoryReactionPoseSelectionsR1467=0;}
 }
}
