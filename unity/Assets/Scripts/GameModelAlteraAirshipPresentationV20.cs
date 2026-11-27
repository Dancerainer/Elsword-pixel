using System;
using System.Collections.Generic;
namespace ElswordPixel {
 public sealed partial class GameModel {
  sealed class AirshipActorClockV20 {internal int entry;internal string state;internal float last;internal readonly HashSet<string> fired=new HashSet<string>();}
  AirshipStateRuleBindingV20[] airshipPresentationBindingsV20;
  AirshipStateBoneStateV20[] airshipPresentationBonesV20;
  readonly Dictionary<Enemy,AirshipActorClockV20> airshipPresentationClocksV20=new Dictionary<Enemy,AirshipActorClockV20>();
  readonly Dictionary<Enemy,AirshipStatePresentationEventV20> airshipPresentationHpSlotsV20=new Dictionary<Enemy,AirshipStatePresentationEventV20>();
  readonly List<AirshipStatePresentationEventV20> airshipPresentationEventsV20=new List<AirshipStatePresentationEventV20>();
  int airshipPresentationSequenceV20,airshipPresentationEntryV20,airshipPresentationHandleV20,airshipPresentationGenerationV20;float airshipPresentationLastWorldTimeV20=-1;
  public IList<AirshipStatePresentationEventV20> AlteraAirshipStateEventsV20 {get{return airshipPresentationEventsV20.AsReadOnly();}}
  public int AlteraAirshipStateGenerationV20 {get{return airshipPresentationGenerationV20;}}
  public int AlteraAirshipHpSlotCountV20 {get{return airshipPresentationHpSlotsV20.Count;}}
  public int AlteraAirshipLastPresentationSequenceV20 {get{return airshipPresentationSequenceV20;}}
  public void SetAlteraAirshipStatePresentationV20(AirshipStateRuleBindingV20[] bindings,AirshipStateBoneStateV20[] bones){
   if(bindings==null&&bones==null){airshipPresentationBindingsV20=null;airshipPresentationBonesV20=null;ClearAlteraAirshipStatePresentationV20();return;}
   if(bindings==null||bones==null||bindings.Length!=AirshipStatePinsV20.Bindings||bones.Length!=AirshipStatePinsV20.Bones||AirshipStateDataV20.FeedHash(bindings,bones)!=AirshipStatePinsV20.Feed)throw new ArgumentException("Changed exact Airship presentation bindings/bones");
   airshipPresentationBindingsV20=Array.ConvertAll(bindings,b=>b.Copy());airshipPresentationBonesV20=Array.ConvertAll(bones,b=>b.Copy());ClearAlteraAirshipStatePresentationV20();
  }
  // Pure presentation state: no NPC state, AI, HP, damage, audio or physics writes.
  public void ClearAlteraAirshipStatePresentationV20(){airshipPresentationClocksV20.Clear();airshipPresentationHpSlotsV20.Clear();airshipPresentationEventsV20.Clear();airshipPresentationSequenceV20=airshipPresentationEntryV20=airshipPresentationHandleV20=0;airshipPresentationGenerationV20++;airshipPresentationLastWorldTimeV20=elapsed;}
  bool AirshipPresentationNpcV20(Enemy owner){if(owner==null||airshipPresentationBindingsV20==null)return false;foreach(var b in airshipPresentationBindingsV20)if(b.npc==owner.sourceNPC)return true;return false;}
  void AddAirshipPresentationV20(AirshipStatePresentationEventV20 e){e.sequence=++airshipPresentationSequenceV20;if(airshipPresentationEventsV20.Count==2048)airshipPresentationEventsV20.RemoveAt(0);airshipPresentationEventsV20.Add(e);}
  float AirshipPresentationLandV20(Enemy owner){var argument=new NpcEffectInstance{owner=owner};CaptureNpcSourceLandArgumentV18(argument,null,new NpcEffectEmission{sourceLandPositionYArgument=true});return argument.sourceLandY;}
  AirshipStatePointV20 AirshipPresentationBoneV20(Enemy owner,string state,string name,float age){
   foreach(var s in airshipPresentationBonesV20){if(s.npc!=owner.sourceNPC||s.state!=state)continue;foreach(var t in s.tracks){if(t.bone!=name)continue;float f=Math.Max(0,Math.Min(t.samples.Length-1,age*s.sampleRate));int a=(int)f,b=Math.Min(a+1,t.samples.Length-1);float p=f-a;var x=t.samples[a];var y=t.samples[b];return new AirshipStatePointV20{x=x.x+(y.x-x.x)*p,y=x.y+(y.y-x.y)*p,z=x.z+(y.z-x.z)*p};}}
   throw new InvalidOperationException("Missing exact Airship source bone "+owner.sourceNPC+"/"+state+"/"+name);
  }
  AirshipStatePresentationEventV20 AirshipPresentationCreateV20(Enemy owner,AirshipActorClockV20 clock,AirshipStateRuleBindingV20 binding,float poseTime,float absolute){
   float land=AirshipPresentationLandV20(owner),x=owner.x,y=binding.positionMethod=="actor-current-land-position"?land:owner.y,depth=owner.nativeDepth;
   if(!string.IsNullOrEmpty(binding.bone)){var p=AirshipPresentationBoneV20(owner,clock.state,binding.bone,poseTime);x+=p.x*owner.facing;y+=p.y;depth+=p.z/.28f;}
   x+=binding.offset[0]*binding.unit*owner.facing;y+=binding.offset[1]*binding.unit;depth+=binding.offset[2]*binding.unit/.28f;
   return new AirshipStatePresentationEventV20{kind=binding.kind=="camera"?"camera":"spawn",entry=clock.entry,binding=binding.id,particleKey=binding.particleKey,npc=owner.sourceNPC,state=clock.state,time=absolute,x=x,y=y,depth=depth,landY=land,hasLand=binding.sourceLandYSeparate,unit=binding.unit,facing=owner.facing,owner=owner,magnitude=binding.magnitude,cameraDuration=binding.cameraDuration};
  }
  void StopAirshipHpSlotV20(Enemy owner,float at){AirshipStatePresentationEventV20 live;if(!airshipPresentationHpSlotsV20.TryGetValue(owner,out live))return;airshipPresentationHpSlotsV20.Remove(owner);AddAirshipPresentationV20(new AirshipStatePresentationEventV20{kind="auto-die",handle=live.handle,entry=live.entry,binding=live.binding,particleKey=live.particleKey,npc=live.npc,state=live.state,time=at,owner=owner});}
  void EvaluateAirshipBindingV20(Enemy owner,AirshipActorClockV20 clock,AirshipStateRuleBindingV20 b,float from,float to,float packetStart,string phase,bool entry){
   if(!b.supported||b.npc!=owner.sourceNPC||b.state!=clock.state||b.phase!=phase)return;
   if(b.kind=="owner-slot"){
    if(entry)return;bool low=owner.hp>0&&owner.maxHp>0&&owner.hp<=owner.maxHp*b.hpMaximumFraction;
    if(!low){StopAirshipHpSlotV20(owner,packetStart+Math.Max(0,to-from));return;}
    if(airshipPresentationHpSlotsV20.ContainsKey(owner))return;
    var e=AirshipPresentationCreateV20(owner,clock,b,to,packetStart+Math.Max(0,to-from));e.handle=++airshipPresentationHandleV20;e.kind="owner-slot-start";airshipPresentationHpSlotsV20.Add(owner,e);AddAirshipPresentationV20(e);return;
   }
   float at=b.at; // Producer divides source animation thresholds once; PARTICLE_SEQ already carries candidate at.
   if(phase=="frame"&&!(entry?at==0:from<=at+.000001f&&at<=to+.000001f))return;
   if(!clock.fired.Add(b.id))return;float eventTime=phase=="frame"?packetStart+Math.Max(0,at-from):packetStart;
   AddAirshipPresentationV20(AirshipPresentationCreateV20(owner,clock,b,phase=="frame"?at:to,eventTime));
  }
  // Root calls this at the actual committed EnterNpcState, after npcState/time
  // assignment and before BeginNpcStateSounds. No traceNpcActions dependency.
  public void OnAlteraAirshipStateEnteredV20(Enemy owner,NpcStateDefinition state){
   if(!AirshipPresentationNpcV20(owner)||state==null)return;SynchronizeAlteraAirshipStatePresentationV20();
   OnAlteraAirshipStateExitedV20(owner);var clock=new AirshipActorClockV20{state=state.id,entry=++airshipPresentationEntryV20};airshipPresentationClocksV20[owner]=clock;
   foreach(var b in airshipPresentationBindingsV20){EvaluateAirshipBindingV20(owner,clock,b,0,0,elapsed,"start",true);EvaluateAirshipBindingV20(owner,clock,b,0,0,elapsed,"frame",true);}
  }
  public void OnAlteraAirshipStateExitedV20(Enemy owner){
   AirshipActorClockV20 clock;if(owner==null||!airshipPresentationClocksV20.TryGetValue(owner,out clock))return;
   foreach(var b in airshipPresentationBindingsV20)EvaluateAirshipBindingV20(owner,clock,b,clock.last,clock.last,elapsed,"end",false);
   airshipPresentationClocksV20.Remove(owner); // HP source Minor slot0 deliberately survives state exit.
  }
  public void AdvanceAlteraAirshipStatePresentationV20(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(!AirshipPresentationNpcV20(owner)||state==null)return;SynchronizeAlteraAirshipStatePresentationV20();AirshipActorClockV20 clock;
   if(!airshipPresentationClocksV20.TryGetValue(owner,out clock)||clock.state!=state.id)return; // Missing exact entry is never reconstructed from render polling.
   if(from+.000001f<clock.last){if(!state.loop)return;clock.fired.Clear();}clock.last=to;
   foreach(var b in airshipPresentationBindingsV20)EvaluateAirshipBindingV20(owner,clock,b,from,to,packetStart,"frame",false);
  }
  // Audio's loop driver can return before visiting a segment when this state
  // has no sound cursor. Root uses this fallback only for that branch; states
  // with audio already visit AdvanceNpcStateSounds for each real loop segment.
  public void AdvanceAlteraAirshipLoopPresentationV20(Enemy owner,NpcStateDefinition state,float from,float dt,float packetStart){
   if(!AirshipPresentationNpcV20(owner)||state==null||state.duration<=0||dt<=0)return;
   float left=dt,time=packetStart,position=from;
   while(left>0){float span=Math.Max(0,Math.Min(left,state.duration-position));AdvanceAlteraAirshipStatePresentationV20(owner,state,position,position+span,time);left-=span;time+=span;position+=span;
    if(position>=state.duration-.000001f){position=0;AdvanceAlteraAirshipStatePresentationV20(owner,state,0,0,time);}else break;
   }
  }
  public void SynchronizeAlteraAirshipStatePresentationV20(){
   if(airshipPresentationBindingsV20==null)return;if(elapsed+.000001f<airshipPresentationLastWorldTimeV20)ClearAlteraAirshipStatePresentationV20();airshipPresentationLastWorldTimeV20=elapsed;
   var dead=new List<Enemy>();foreach(var p in airshipPresentationHpSlotsV20)if(p.Key.hp<=0||!enemies.Contains(p.Key)||!p.Key.active)dead.Add(p.Key);foreach(var owner in dead)StopAirshipHpSlotV20(owner,elapsed);
   dead.Clear();foreach(var p in airshipPresentationClocksV20)if(!enemies.Contains(p.Key)||!p.Key.active)dead.Add(p.Key);foreach(var owner in dead)airshipPresentationClocksV20.Remove(owner);
  }
 }
}
