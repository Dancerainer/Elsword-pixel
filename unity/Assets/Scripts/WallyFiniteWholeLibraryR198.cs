using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class WallyWholePinR198 { public string path,sha256; public long bytes; }
 [Serializable] public sealed class WallyWholeKeyR198 { public string key,file,rgbaSha256; public WallyWholePinR198 png; public int width,height; public double[] root,pivot; }
 [Serializable] public sealed class WallyWholePhaseR198 { public double at,until; public string key,sourcePose,intervalClassification,geometryEquality; public bool sourceSignatureKnown,sourceExactAtOpeningSample,continuousSourceEqualityClaimed; }
 [Serializable] public sealed class WallyWholeEndpointR198 { public double at,until,duration; public string key; public bool zeroDwell; }
 [Serializable] public sealed class WallyWholeMotionR198 { public string id,outOfRangePolicy; public double duration; public WallyWholePhaseR198[] phases; public WallyWholeEndpointR198 endpoint; }
 [Serializable] public sealed class WallyWholeStateR198 { public string id,clip,motion,nextState,originalStateSHA256; public double duration,motionDuration,speed; public bool loop,holdForSourceEvent,hasSourceShow,sourceShow; }
 [Serializable] public sealed class WallyWholeDocumentR198 {
  public string schema,npc,kind,sourceRulesSha256; public WallyWholePinR198 registry,registryAcceptance,sourceClock;
  public int wholeCount,motionCount,stateCount,positiveMotionPhases,positiveStatePhases,zeroDwellEndpoints;
  public double sourceScaleBaked,modelUnit,extraScale; public bool automaticSpawnFacingMirror,sourceClockChanged,displayEmitsGameplay,runtimeAdmitted,familyApproved,masterPromoted;
  public WallyWholeKeyR198[] keys; public WallyWholeMotionR198[] motions; public WallyWholeStateR198[] states;
 }
 public sealed class WallyWholeKeyInfoR198 {
  public readonly string Key,File,Path,Sha256,RgbaSha256; public readonly int Width,Height; public readonly double RootX,RootY,PivotX,PivotY;
  internal WallyWholeKeyInfoR198(WallyWholeKeyR198 k){Key=k.key;File=k.file;Path=k.png.path;Sha256=k.png.sha256;RgbaSha256=k.rgbaSha256;Width=k.width;Height=k.height;RootX=k.root[0];RootY=k.root[1];PivotX=k.pivot[0];PivotY=k.pivot[1];}
 }
 public sealed class WallyWholeStateInfoR198 {
  public readonly string Id,Clip,Motion,NextState; public readonly float Duration,MotionDuration,Speed; public readonly bool Loop,HoldForSourceEvent;
  internal WallyWholeStateInfoR198(WallyWholeStateR198 s){Id=s.id;Clip=s.clip;Motion=s.motion;NextState=s.nextState;Duration=(float)s.duration;MotionDuration=(float)s.motionDuration;Speed=(float)s.speed;Loop=s.loop;HoldForSourceEvent=s.holdForSourceEvent;}
 }
 public sealed class WallyWholeSelectionR198 {
  public readonly WallyWholeKeyInfoR198 Whole; public readonly WallyWholeStateInfoR198 State;
  public readonly int PhaseIndex; public readonly float ExistingNpcClock,SourceMotionClock;
  public readonly string SourcePose,IntervalClassification,GeometryEquality;
  public readonly bool OutsideFiniteRange,SourceExactAtOpeningSample;
  public bool EmitsGameplay {get{return false;}} public bool EndpointPositiveDwell {get{return false;}}
  public bool ContinuousSourceEqualityClaimed {get{return false;}}
  internal WallyWholeSelectionR198(WallyWholeKeyInfoR198 w,WallyWholeStateInfoR198 s,int i,float npc,float clock,string pose,string classification,string geometry,bool outside,bool exact){Whole=w;State=s;PhaseIndex=i;ExistingNpcClock=npc;SourceMotionClock=clock;SourcePose=pose;IntervalClassification=classification;GeometryEquality=geometry;OutsideFiniteRange=outside;SourceExactAtOpeningSample=exact;}
 }
 // Display-only immutable snapshots. No Unity types, Enemy, callbacks or state transition API.
 public sealed class WallyFiniteWholeLookupR198 {
  sealed class Phase {internal float at,until;internal string key,pose,classification,geometry;internal bool exact;}
  sealed class Motion {internal float duration;internal Phase[] phases;internal string endpoint;}
  readonly Dictionary<string,WallyWholeKeyInfoR198> keys=new Dictionary<string,WallyWholeKeyInfoR198>(StringComparer.Ordinal);
  readonly Dictionary<string,Motion> motions=new Dictionary<string,Motion>(StringComparer.Ordinal);
  readonly Dictionary<string,WallyWholeStateInfoR198> states=new Dictionary<string,WallyWholeStateInfoR198>(StringComparer.Ordinal);
  public int WholeCount {get{return keys.Count;}} public int MotionCount {get{return motions.Count;}} public int StateCount {get{return states.Count;}}
  static void Need(bool yes,string why){if(!yes)throw new ArgumentException("Wally finite display: "+why);}
  static bool Finite(double x){return !double.IsNaN(x)&&!double.IsInfinity(x);}
  static bool Hash(string s){if(s==null||s.Length!=64)return false;foreach(char c in s)if(!(c>='0'&&c<='9'||c>='a'&&c<='f'))return false;return true;}
  static bool Name(string s){if(string.IsNullOrEmpty(s))return false;foreach(char c in s)if(!(c>='a'&&c<='z'||c>='0'&&c<='9'||c=='-'))return false;return true;}
  public WallyFiniteWholeLookupR198(WallyWholeDocumentR198 d){
   Need(d!=null&&d.schema=="wally-finite-display-source-r198"&&d.npc=="NUI_WALLY_9TH_TYPE2"&&d.kind=="wally-9th-type2"&&Hash(d.sourceRulesSha256),"identity/schema/rules");
   Need(d.sourceScaleBaked==.9&&d.modelUnit==.28&&d.extraScale==1&&!d.automaticSpawnFacingMirror&&!d.sourceClockChanged&&!d.displayEmitsGameplay&&!d.runtimeAdmitted&&!d.familyApproved&&!d.masterPromoted,"display-only registered scale/admission");
   Need(d.registry!=null&&Hash(d.registry.sha256)&&d.registryAcceptance!=null&&Hash(d.registryAcceptance.sha256)&&d.sourceClock!=null&&Hash(d.sourceClock.sha256),"accepted registry/clock provenance");
   Need(d.wholeCount==113&&d.motionCount==18&&d.stateCount==27&&d.positiveMotionPhases==178&&d.positiveStatePhases==239&&d.zeroDwellEndpoints==18&&d.keys!=null&&d.keys.Length==113&&d.motions!=null&&d.motions.Length==18&&d.states!=null&&d.states.Length==27,"full113/18/27 finite cardinality");
   foreach(var k in d.keys){
    Need(k!=null&&Name(k.key)&&k.file=="whole/"+k.key+".png"&&k.png!=null&&!string.IsNullOrEmpty(k.png.path)&&Hash(k.png.sha256)&&Hash(k.rgbaSha256)&&k.width>0&&k.height>0&&k.width<=8192&&k.height<=8192,"native whole metadata");
    Need(k.root!=null&&k.root.Length==2&&Finite(k.root[0])&&Finite(k.root[1])&&k.pivot!=null&&k.pivot.Length==2&&Finite(k.pivot[0])&&Finite(k.pivot[1])&&k.pivot[0]==k.root[0]/k.width&&k.pivot[1]==1-k.root[1]/k.height,"signed root/pivot, no clamp");
    keys.Add(k.key,new WallyWholeKeyInfoR198(k));
   }
   int positives=0;var used=new HashSet<string>(StringComparer.Ordinal);
   foreach(var m in d.motions){
    Need(m!=null&&!string.IsNullOrEmpty(m.id)&&Finite(m.duration)&&m.duration>0&&Finite((float)m.duration)&&m.phases!=null&&m.phases.Length>0&&m.outOfRangePolicy=="LAST_POSITIVE_WHOLE_DISPLAY_HOLD_UNPROVEN_NO_ENDPOINT_DWELL","motion clock/policy");
    var copy=new Motion{duration=(float)m.duration,phases=new Phase[m.phases.Length]};double last=0;
    for(int i=0;i<m.phases.Length;i++){
     var p=m.phases[i];Need(p!=null&&Finite(p.at)&&Finite(p.until)&&p.at==last&&p.until>p.at&&p.until<=m.duration&&(float)p.until>(float)p.at&&keys.ContainsKey(p.key)&&!string.IsNullOrEmpty(p.sourcePose),"positive contiguous half-open intervals/key");
     Need(p.sourceSignatureKnown&&p.sourceExactAtOpeningSample&&!p.continuousSourceEqualityClaimed&&(p.geometryEquality=="EXACT"||p.geometryEquality=="NOT_EXACT"||p.geometryEquality=="UNKNOWN"),"sample-only source equality, separate geometry");
     Need(p.intervalClassification=="INTER_KEY_SOURCE_CHANGES_DISPLAY_HOLD_APPROXIMATION"||p.intervalClassification=="CONTINUOUS_SOURCE_CHANNEL_STATIC_DISPLAY_HOLD"||p.intervalClassification=="FINITE_HOLD_CONTINUOUS_SOURCE_EQUALITY_UNPROVEN","finite HOLD classification");
     copy.phases[i]=new Phase{at=(float)p.at,until=(float)p.until,key=p.key,pose=p.sourcePose,classification=p.intervalClassification,geometry=p.geometryEquality,exact=p.sourceExactAtOpeningSample};last=p.until;positives++;used.Add(p.key);
    }
    Need(last==m.duration&&m.endpoint!=null&&m.endpoint.zeroDwell&&m.endpoint.duration==0&&m.endpoint.at==m.duration&&m.endpoint.until==m.duration&&keys.ContainsKey(m.endpoint.key),"endpoint is metadata with zero positive dwell");copy.endpoint=m.endpoint.key;motions.Add(m.id,copy);
   }
   Need(positives==178&&used.Count==113,"positive exposure accounting distinct from endpoint metadata");
   int statePositives=0;
   foreach(var s in d.states){Need(s!=null&&!string.IsNullOrEmpty(s.id)&&!string.IsNullOrEmpty(s.clip)&&motions.ContainsKey(s.motion)&&Finite(s.duration)&&s.duration>=0&&Finite((float)s.duration)&&s.motionDuration==(double)d.motions[Array.FindIndex(d.motions,m=>m.id==s.motion)].duration&&s.speed==1&&!s.hasSourceShow&&!s.sourceShow&&Hash(s.originalStateSHA256),"original state clocks/speed/no authored SHOW");states.Add(s.id,new WallyWholeStateInfoR198(s));statePositives+=motions[s.motion].phases.Length;}
   Need(statePositives==239,"state exposure accounting");
  }
  public WallyWholeKeyInfoR198[] Keys(){var a=new WallyWholeKeyInfoR198[keys.Count];keys.Values.CopyTo(a,0);return a;}
  public WallyWholeStateInfoR198 State(string id){WallyWholeStateInfoR198 s;if(!states.TryGetValue(id??"",out s))throw new ArgumentException("No Wally display state: "+id);return s;}
  public string ZeroDwellEndpointKey(string motion){Motion m;if(!motions.TryGetValue(motion??"",out m))throw new ArgumentException("No Wally motion endpoint: "+motion);return m.endpoint;}
  public WallyWholeSelectionR198 Select(string state,float npcClock){
   Need(Finite(npcClock)&&npcClock>=0,"invalid existing NPC clock");var s=State(state);var m=motions[s.Motion];float clock=npcClock*s.Speed;Need(Finite(clock),"clock overflow");
   if(s.Loop)clock%=m.duration;bool outside=!s.Loop&&clock>=m.duration;
   // Never select the zero-dwell endpoint as a positive phase. When a source state
   // remains beyond its finite strip, hold its last positive whole and expose the
   // uncertainty; this is not approved strip exposure or a gameplay transition.
   int index=0;for(int i=1;i<m.phases.Length;i++)if(m.phases[i].at<=clock)index=i;else break;
   var p=m.phases[index];return new WallyWholeSelectionR198(keys[p.key],s,index,npcClock,clock,p.pose,outside?"AFTER_FINITE_RANGE_DISPLAY_HOLD_UNPROVEN":p.classification,p.geometry,outside,!outside&&clock==p.at&&p.exact);
  }
 }
}
