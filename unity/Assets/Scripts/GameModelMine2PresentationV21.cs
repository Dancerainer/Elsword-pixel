using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class Mine2PresentationRuleV21 {
  public string id,npc,state,sourceCallback,sourceInstructionsSha256,particleKey,phase,origin,bone,vectorArgumentAdapter;
  public float at,loopPeriod,sourceOwnerYawAdd;public float[] sourceOffset;public int slot=-1;public bool follow,boneYIsLand,autoDieOnStateEnd,ownerOrientation,endThresholdGuard;
 }
 [Serializable] public sealed class Mine2PresentationBoneStateV21 {public string npc,state;public float duration,sourceAnimSpeed;public NpcAttachmentTrack[] tracks;}
 [Serializable] public sealed class Mine2PresentationDocumentV21 {public string schema,sourceRulesSha256,sourceSitesSha256;public bool runtimeAdmitted,nativeExecuted;public Mine2PresentationRuleV21[] rows;public Mine2PresentationBoneStateV21[] states;}
 public sealed class Mine2PresentationEventV21 {public int sequence,handle,entry,facing;public string kind,particleKey,binding,npc,state;public float time,x,y,z,depth,unit,land;public bool continuous;public Enemy owner;}
 public sealed partial class GameModel {
  sealed class Mine2PresentationClockV21 {internal string state;internal int entry;internal float last;internal readonly HashSet<string> fired=new HashSet<string>();internal readonly Dictionary<int,Mine2PresentationEventV21> slots=new Dictionary<int,Mine2PresentationEventV21>();internal readonly Dictionary<int,Mine2PresentationRuleV21> rules=new Dictionary<int,Mine2PresentationRuleV21>();}
  Mine2PresentationDocumentV21 mine2PresentationV21;readonly Dictionary<Enemy,Mine2PresentationClockV21> mine2PresentationClocksV21=new Dictionary<Enemy,Mine2PresentationClockV21>();
  readonly List<Mine2PresentationEventV21> mine2PresentationEventsV21=new List<Mine2PresentationEventV21>();int mine2PresentationSequenceV21,mine2PresentationHandleV21,mine2PresentationEntryV21;
  public int Mine2PresentationGenerationV21 {get;private set;}
  public int Mine2PresentationLastSequenceV21 {get{return mine2PresentationSequenceV21;}}
  public IList<Mine2PresentationEventV21> Mine2PresentationEventsV21 {get{return mine2PresentationEventsV21.AsReadOnly();}}
  public void SetMine2PresentationV21(Mine2PresentationDocumentV21 d){
   if(d!=null){
    if(d.schema!="mine2-source-presentation-feed-v21"||d.runtimeAdmitted||d.nativeExecuted||!SourceSupportHashV17(d.sourceRulesSha256)||!SourceSupportHashV17(d.sourceSitesSha256)||d.rows==null||d.rows.Length!=29||d.states==null||d.states.Length!=5)throw new ArgumentException("Changed Mine2 presentation source scope");
    var ids=new HashSet<string>();foreach(var r in d.rows){NpcStateDefinition s;if(r==null||!ids.Add(r.id??"")||string.IsNullOrEmpty(r.npc)||!npcStates.TryGetValue(r.npc+"/"+r.state,out s)||string.IsNullOrEmpty(r.sourceCallback)||!SourceSupportHashV17(r.sourceInstructionsSha256)||string.IsNullOrEmpty(r.particleKey)||(!r.particleKey.StartsWith("Minor/")&&!r.particleKey.StartsWith("Major/"))||(r.phase!="start"&&r.phase!="frame"&&r.phase!="end"&&r.phase!="heal-target")||!Finite(r.at)||r.at<0||!Finite(r.loopPeriod)||r.loopPeriod<0||r.sourceOffset==null||r.sourceOffset.Length!=3||!Array.TrueForAll(r.sourceOffset,Finite)||r.slot< -1||r.slot>3||r.vectorArgumentAdapter!="original-two-vectors-retained-template-count-rate-display-defaults-v21")throw new ArgumentException("Malformed original Mine2 visual binding");}
    foreach(var s in d.states)foreach(var t in s.tracks){if(t.samples==null||t.samples.Length==0)throw new ArgumentException("Missing source Mine2 bone track");foreach(var p in t.samples)if(!Finite(p.x)||!Finite(p.y)||!Finite(p.z))throw new ArgumentException("Nonfinite source Mine2 bone");}
   }
   mine2PresentationV21=d;ClearMine2PresentationV21();
  }
  public void ClearMine2PresentationV21(){mine2PresentationClocksV21.Clear();mine2PresentationEventsV21.Clear();mine2PresentationSequenceV21=mine2PresentationHandleV21=mine2PresentationEntryV21=0;Mine2PresentationGenerationV21++;}
  void Mine2AddPresentationV21(Mine2PresentationEventV21 e){e.sequence=++mine2PresentationSequenceV21;if(mine2PresentationEventsV21.Count==4096)mine2PresentationEventsV21.RemoveAt(0);mine2PresentationEventsV21.Add(e);}
  float Mine2PresentationLandV21(Enemy e){var a=new NpcEffectInstance{owner=e};CaptureNpcSourceLandArgumentV18(a,null,new NpcEffectEmission{sourceLandPositionYArgument=true});return a.sourceLandY;}
  NpcAttachmentPoint Mine2PresentationBoneV21(Enemy e,string state,string bone,float at){var s=Array.Find(mine2PresentationV21.states,q=>q.npc==e.sourceNPC&&q.state==state);if(s==null)throw new InvalidOperationException("Missing exact Mine2 source bone state");var t=Array.Find(s.tracks,q=>q.bone==bone);if(t==null)throw new InvalidOperationException("Missing exact Mine2 source bone name");float f=Clamp(at*60,0,t.samples.Length-1);int a=(int)f,b=Math.Min(a+1,t.samples.Length-1);float u=f-a;return new NpcAttachmentPoint{x=t.samples[a].x+(t.samples[b].x-t.samples[a].x)*u,y=t.samples[a].y+(t.samples[b].y-t.samples[a].y)*u,z=t.samples[a].z+(t.samples[b].z-t.samples[a].z)*u};}
  Mine2PresentationEventV21 Mine2CreatePresentationV21(Enemy owner,Mine2PresentationClockV21 clock,Mine2PresentationRuleV21 r,string kind,float at,float local,int handle,bool following=false){
   float land=Mine2PresentationLandV21(owner),x=owner.x,y=r.origin=="land"?land:owner.y,z=0;
   if(r.origin=="bone"){var p=Mine2PresentationBoneV21(owner,clock.state,r.bone,r.loopPeriod>0?local%r.loopPeriod:local);x+=owner.facing*p.x*owner.visualScale;y=r.boneYIsLand?land:owner.y+p.y*owner.visualScale;z=p.z*owner.visualScale;}
   if(!following){x+=r.sourceOffset[0]*.28f*owner.facing;y+=r.sourceOffset[1]*.28f;z+=r.sourceOffset[2]*.28f;}
   var e=new Mine2PresentationEventV21{kind=kind,owner=owner,handle=handle,entry=clock.entry,binding=r.id,particleKey=r.particleKey,npc=owner.sourceNPC,state=clock.state,time=at,x=x,y=y,z=z,depth=owner.nativeDepth,unit=.28f,facing=r.ownerOrientation?owner.facing*(r.sourceOwnerYawAdd==180?-1:1):1,land=land,continuous=r.autoDieOnStateEnd};Mine2AddPresentationV21(e);return e;
  }
  public void OnMine2PresentationStateEnteredV21(Enemy e,NpcStateDefinition s){if(mine2PresentationV21==null||e==null||s==null||!Array.Exists(mine2PresentationV21.rows,r=>r.npc==e.sourceNPC&&r.state==s.id))return;var c=new Mine2PresentationClockV21{state=s.id,entry=++mine2PresentationEntryV21};mine2PresentationClocksV21[e]=c;foreach(var r in mine2PresentationV21.rows)if(r.npc==e.sourceNPC&&r.state==s.id&&(r.phase=="start"||r.phase=="frame"&&r.at==0))Mine2FirePresentationV21(e,c,r,elapsed,0,0);}
  void Mine2FirePresentationV21(Enemy e,Mine2PresentationClockV21 c,Mine2PresentationRuleV21 r,float absolute,float local,int loop){if(!c.fired.Add(r.id+"/"+loop))return;int handle=++mine2PresentationHandleV21;var v=Mine2CreatePresentationV21(e,c,r,"spawn",absolute,local,handle);if(r.slot>=0){c.slots[r.slot]=v;c.rules[r.slot]=r;}}
  public void OnMine2PresentationStateExitedV21(Enemy e){Mine2PresentationClockV21 c;if(mine2PresentationV21==null||e==null||!mine2PresentationClocksV21.TryGetValue(e,out c))return;
   foreach(var r in mine2PresentationV21.rows)if(r.npc==e.sourceNPC&&r.state==c.state&&r.phase=="end"&&(!r.endThresholdGuard||e.npcTime+1e-6f>=r.at))Mine2FirePresentationV21(e,c,r,elapsed,e.npcTime,0);
   foreach(var p in c.slots){var r=c.rules[p.Key];Mine2AddPresentationV21(new Mine2PresentationEventV21{kind=r.autoDieOnStateEnd?"auto-die":"destroy",handle=p.Value.handle,owner=e,time=elapsed,entry=c.entry,binding=r.id,particleKey=r.particleKey,npc=e.sourceNPC,state=c.state});}mine2PresentationClocksV21.Remove(e);
  }
  public void AdvanceMine2PresentationV21(Enemy e,NpcStateDefinition s,float from,float to,float packetStart){Mine2PresentationClockV21 c;if(mine2PresentationV21==null||e==null||s==null||!e.active||e.dormant||!mine2PresentationClocksV21.TryGetValue(e,out c)||c.state!=s.id)return;c.last=to;
   foreach(var r in mine2PresentationV21.rows){if(r.npc!=e.sourceNPC||r.state!=s.id||r.phase!="frame")continue;
    int first=r.loopPeriod>0?Math.Max(0,(int)Math.Floor((from-r.at)/r.loopPeriod)):0,last=r.loopPeriod>0?Math.Max(0,(int)Math.Floor((to-r.at+1e-6f)/r.loopPeriod)):0;
    for(int loop=first;loop<=last;loop++){float at=r.at+loop*r.loopPeriod;if(from<at&&at<=to+1e-6f)Mine2FirePresentationV21(e,c,r,packetStart+at-from,at,loop);}
   }
   foreach(var p in c.slots){var r=c.rules[p.Key];if(r.follow)Mine2CreatePresentationV21(e,c,r,"move",packetStart+to-from,to,p.Value.handle,true);}
  }
  public void AdvanceMine2PresentationLoopV21(Enemy e,NpcStateDefinition s,float from,float dt,float packetStart){if(mine2PresentationV21==null||s==null||s.duration<=0)return;float left=dt,pos=from,time=packetStart;while(left>.000001f){float span=Math.Max(0,Math.Min(left,s.duration-pos));AdvanceMine2PresentationV21(e,s,pos,pos+span,time);left-=span;time+=span;pos+=span;if(pos>=s.duration-.000001f)pos=0;else break;}}
  public void EmitMine2ActualHealPresentationV21(Enemy owner,Enemy target,float at){Mine2PresentationClockV21 c;if(mine2PresentationV21==null||owner==null||target==null||owner.sourceNPC!="NUI_NASOD_HEALER"||!mine2PresentationClocksV21.TryGetValue(owner,out c)||c.state!="NASOD_HEALER_HEAL")return;foreach(var r in mine2PresentationV21.rows)if(r.npc==owner.sourceNPC&&r.state==c.state&&r.phase=="heal-target")Mine2AddPresentationV21(new Mine2PresentationEventV21{kind="spawn",owner=target,entry=c.entry,handle=++mine2PresentationHandleV21,binding=r.id,particleKey=r.particleKey,npc=owner.sourceNPC,state=c.state,time=at,x=target.x,y=target.y,z=0,depth=target.nativeDepth,unit=.28f,facing=1,land=Mine2PresentationLandV21(target)});}
 }
}
