using System;
using System.Collections.Generic;

namespace ElswordPixel {public sealed partial class DemoScene {
 CoreReactionPoseMapV32 coreReactionPoseMapV32;
 readonly VisibleHurtPoseV44 coreVisibleHurtPoseV44=new VisibleHurtPoseV44();
 readonly HashSet<string> coreReactionPoseGapsV32=new HashSet<string>(StringComparer.Ordinal);
 public int CoreReactionPoseSelectionsV32 {get;private set;}
 public string[] ReadCoreReactionPoseGapsV32(){var rows=new List<string>(coreReactionPoseGapsV32);rows.Sort(StringComparer.Ordinal);return rows.ToArray();}
 void ConfigureCoreReactionDisplayV32(NpcDefinition[] rules){
  var map=new CoreReactionPoseMapV32(rules);
  foreach(var entry in map.Entries()){
   string clip,motion;float duration,speed;bool loop;
   if(entry.Npc=="NUI_WALLY_9TH_TYPE2"){var s=coreWallyWholeLookupR198.State(entry.State);clip=s.Clip;motion=s.Motion;duration=s.Duration;speed=s.Speed;loop=s.Loop;}
   else if(entry.Npc=="NUI_NASOD_SPRAY_ICE"){var s=coreSprayWholeLookupR203.State(entry.State);clip=s.Clip;motion=s.Motion;duration=s.Duration;speed=s.Speed;loop=s.Loop;}
   else if(entry.Npc=="NUI_NASOD_ELITE_SHERIFF"){var s=coreSheriffWholeLookupR227.State(entry.State);clip=s.Clip;motion=s.Motion;duration=s.Duration;speed=s.Speed;loop=s.Loop;}
   else {
    var lookup=entry.Npc=="NUI_NASOD_ELITE_GUARD"?coreGuardWholeLookupV21:entry.Npc=="NUI_NASOD_HEALER_BLADER"?coreHealerWholeLookupV21:entry.Npc=="NUI_NASOD_ELEMENT_FIRE_B"?coreElementFireBWholeLookupV21:entry.Npc=="NUI_NASOD_ELEMENT_ICE_B"?coreElementIceBWholeLookupV21:entry.Npc=="NUI_RAVEN_CLONE_B"?coreRavenBWholeLookupV21:coreWatchWholeLookupV21;
    var s=lookup.State(entry.State);clip=s.ModelClip;motion=s.Motion;duration=s.StateDuration;speed=s.Speed;loop=s.Loop;
   }
   PreviewRequire(clip==entry.Clip&&motion==entry.Motion&&duration==entry.Duration&&speed==entry.Speed&&!loop,"Core reaction display differs from its existing body library: "+entry.Npc+"/"+entry.State);
  }
  coreReactionPoseMapV32=map;coreReactionPoseGapsV32.Clear();CoreReactionPoseSelectionsV32=0;
 }
 bool TryCoreReactionDisplayV32(Enemy actor,out string state,out float time){
  // Preserve exact authored NPC states even before/without a configured map.
  state=actor==null?null:actor.npcState;time=actor==null?0:actor.npcTime;
  if(!string.IsNullOrEmpty(state)){
   // This reaction has no registered Revenge drawing. Show this sheriff's own
   // heavy recoil using the live reaction clock while gameplay runs Revenge.
   if(actor.sourceNPC=="NUI_NASOD_ELITE_SHERIFF"&&state=="NASOD_ELITE_SHERIFF_DAMAGE_REVENGE")state="NASOD_ELITE_SHERIFF_DAMAGE_BIG";
   coreVisibleHurtPoseV44.Remember(actor,state,time);return true;
  }
  if(TryOriginalWholeReactionV45(actor,out state,out time)){CoreReactionPoseSelectionsV32++;coreVisibleHurtPoseV44.Remember(actor,state,time);return true;}
  bool physical;if(coreReactionPoseMapV32!=null&&coreReactionPoseMapV32.TrySelect(actor,out state,out time,out physical)){if(physical)CoreReactionPoseSelectionsV32++;coreVisibleHurtPoseV44.Remember(actor,state,time);return true;}
  if(coreVisibleHurtPoseV44.TryHold(actor,out state,out time))return true;
  if(actor!=null&&actor.state!=CombatState.Normal)coreReactionPoseGapsV32.Add(actor.sourceNPC+"/"+actor.state);return false;
 }
 void DisposeCoreReactionDisplayV32(){coreReactionPoseMapV32=null;coreReactionPoseGapsV32.Clear();CoreReactionPoseSelectionsV32=0;}
}}
