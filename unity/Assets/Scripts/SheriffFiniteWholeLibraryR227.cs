using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class SheriffWholePinR227 { public string path,sha256; public long bytes; }
 [Serializable] public sealed class SheriffWholeKeyR227 { public string key,file,rgbaSha256; public SheriffWholePinR227 png; public int width,height; public double[] root,pivot; }
 [Serializable] public sealed class SheriffWholePhaseR227 { public double at,until; public string key,sourcePose,intervalClassification,openingSampleGeometryEquality; public bool sourceSignatureKnown,sourceExactAtOpeningSample,continuousSourceEqualityClaimed; }
 [Serializable] public sealed class SheriffWholeEndpointR227 { public double at,until,duration; public string key,sourcePose,openingSampleGeometryEquality; public bool zeroDwell,sourceKnown,sourceExactToArtAnchor; }
 [Serializable] public sealed class SheriffWholeMotionR227 { public string id,outOfRangePolicy; public double duration; public SheriffWholePhaseR227[] phases; public SheriffWholeEndpointR227 endpoint; }
 [Serializable] public sealed class SheriffWholeStateR227 { public string id,clip,motion,nextState,originalStateSHA256; public double duration,motionDuration,speed,originalAddRotateZ; public bool loop,holdForSourceEvent,hasSourceShow,sourceShow; }
 [Serializable] public sealed class SheriffWholeDocumentR227 {
  public string schema,npc,kind,sourceRulesSha256; public SheriffWholePinR227 registry,registryAcceptance,sourceClock,sourceCoverage;
  public int wholeCount,motionCount,stateCount,positiveMotionPhases,positiveStatePhases,zeroDwellEndpoints,positiveWholeCount,unknownSourceKeys; public string endpointOnlyWhole;
  public double sourceScaleBaked,modelUnit,extraScale; public bool automaticSpawnFacingMirror,sourceClockChanged,displayEmitsGameplay,runtimeAdmitted,familyApproved,masterPromoted;
  public SheriffWholeKeyR227[] keys; public SheriffWholeMotionR227[] motions; public SheriffWholeStateR227[] states;
 }
 public sealed class SheriffWholeKeyInfoR227 {
  public readonly string Key,File,Path,Sha256,RgbaSha256; public readonly int Width,Height; public readonly double RootX,RootY,PivotX,PivotY;
  internal SheriffWholeKeyInfoR227(SheriffWholeKeyR227 k){Key=k.key;File=k.file;Path=k.png.path;Sha256=k.png.sha256;RgbaSha256=k.rgbaSha256;Width=k.width;Height=k.height;RootX=k.root[0];RootY=k.root[1];PivotX=k.pivot[0];PivotY=k.pivot[1];}
 }
 public sealed class SheriffWholeStateInfoR227 {
  public readonly string Id,Clip,Motion,NextState; public readonly float Duration,MotionDuration,Speed; public readonly double OriginalAddRotateZ; public readonly bool Loop,HoldForSourceEvent;
  internal SheriffWholeStateInfoR227(SheriffWholeStateR227 s){Id=s.id;Clip=s.clip;Motion=s.motion;NextState=s.nextState;Duration=(float)s.duration;MotionDuration=(float)s.motionDuration;Speed=(float)s.speed;Loop=s.loop;HoldForSourceEvent=s.holdForSourceEvent;OriginalAddRotateZ=s.originalAddRotateZ;}
 }
 public sealed class SheriffWholeSelectionR227 {
  public readonly SheriffWholeKeyInfoR227 Whole; public readonly SheriffWholeStateInfoR227 State;
  public readonly int PhaseIndex; public readonly float ExistingNpcClock,SourceMotionClock;
  public readonly string SourcePose,IntervalClassification,OpeningSampleGeometryEquality;
  public readonly bool OutsideFiniteRange,SourceExactAtOpeningSample;
  public bool EmitsGameplay {get{return false;}} public bool EndpointPositiveDwell {get{return false;}}
  public bool ContinuousSourceEqualityClaimed {get{return false;}}
  internal SheriffWholeSelectionR227(SheriffWholeKeyInfoR227 w,SheriffWholeStateInfoR227 s,int i,float npc,float clock,string pose,string classification,string geometry,bool outside,bool exact){Whole=w;State=s;PhaseIndex=i;ExistingNpcClock=npc;SourceMotionClock=clock;SourcePose=pose;IntervalClassification=classification;OpeningSampleGeometryEquality=geometry;OutsideFiniteRange=outside;SourceExactAtOpeningSample=exact;}
 }
 public sealed class SheriffEndpointInfoR227 {
  public readonly string Motion,MetadataWholeKey,SourcePose,OpeningSampleGeometryEquality; public readonly double At;
  public readonly bool SourceKnown,SourceExactToArtAnchor; public readonly SheriffWholeKeyInfoR227 Whole;
  public bool DiagnosticOnly {get{return true;}} public bool EmitsGameplay {get{return false;}} public double PositiveDwell {get{return 0;}}
  internal SheriffEndpointInfoR227(string motion,SheriffWholeEndpointR227 e,SheriffWholeKeyInfoR227 w){Motion=motion;MetadataWholeKey=e.key;SourcePose=e.sourcePose;OpeningSampleGeometryEquality=e.openingSampleGeometryEquality;At=e.at;SourceKnown=e.sourceKnown;SourceExactToArtAnchor=e.sourceExactToArtAnchor;Whole=e.sourceKnown?w:null;}
 }
 // Source/raster XY mapping: CPU (x+256,448-y), then sprite (pixelX-rootX,
 // rootY-pixelY). The two Y reversals cancel; original Z degrees retain sign
 // in Unity XY. These formulas do not claim native/GPU transform verification.
 public static class SheriffWholePlacementR227 {
  public static void Rotate(double x,double y,double degrees,out double rx,out double ry){
   if(double.IsNaN(x)||double.IsInfinity(x)||double.IsNaN(y)||double.IsInfinity(y)||double.IsNaN(degrees)||double.IsInfinity(degrees))throw new ArgumentException("Nonfinite source display placement");
   double theta=degrees*Math.PI/180,c=Math.Cos(theta),s=Math.Sin(theta);rx=c*x-s*y;ry=s*x+c*y;
  }
  public static void CenterOffset(SheriffWholeKeyInfoR227 key,double degrees,out double x,out double y){if(key==null)throw new ArgumentException("Missing whole");Rotate(key.Width*.5-key.RootX,key.RootY-key.Height*.5,degrees,out x,out y);}
 }
 // Display-only immutable snapshots. No Unity types, Enemy, callbacks or state transition API.
 public sealed class SheriffFiniteWholeLookupR227 {
  sealed class Phase {internal float at,until;internal string key,pose,classification,geometry;internal bool exact;}
  sealed class Motion {internal float duration;internal Phase[] phases;internal SheriffEndpointInfoR227 endpoint;}
  readonly Dictionary<string,SheriffWholeKeyInfoR227> keys=new Dictionary<string,SheriffWholeKeyInfoR227>(StringComparer.Ordinal);
  readonly Dictionary<string,Motion> motions=new Dictionary<string,Motion>(StringComparer.Ordinal);
  readonly Dictionary<string,SheriffWholeStateInfoR227> states=new Dictionary<string,SheriffWholeStateInfoR227>(StringComparer.Ordinal);
  public int WholeCount {get{return keys.Count;}} public int MotionCount {get{return motions.Count;}} public int StateCount {get{return states.Count;}}
  static void Need(bool yes,string why){if(!yes)throw new ArgumentException("Sheriff finite display: "+why);}
  static bool Finite(double x){return !double.IsNaN(x)&&!double.IsInfinity(x);}
  static bool Hash(string s){if(s==null||s.Length!=64)return false;foreach(char c in s)if(!(c>='0'&&c<='9'||c>='a'&&c<='f'))return false;return true;}
  static bool Name(string s){if(string.IsNullOrEmpty(s))return false;foreach(char c in s)if(!(c>='a'&&c<='z'||c>='A'&&c<='Z'||c>='0'&&c<='9'||c=='-'||c=='_'))return false;return true;}
  public SheriffFiniteWholeLookupR227(SheriffWholeDocumentR227 d){
   Need(d!=null&&d.schema=="sheriff-finite-display-source-r227"&&d.npc=="NUI_NASOD_ELITE_SHERIFF"&&d.kind=="nasod-elite-sheriff"&&Hash(d.sourceRulesSha256),"identity/schema/rules");
   Need(d.sourceScaleBaked==1&&d.modelUnit==.28&&d.extraScale==1&&!d.automaticSpawnFacingMirror&&!d.sourceClockChanged&&!d.displayEmitsGameplay&&!d.runtimeAdmitted&&!d.familyApproved&&!d.masterPromoted,"display-only registered scale/admission");
   Need(d.registry!=null&&Hash(d.registry.sha256)&&d.registryAcceptance!=null&&Hash(d.registryAcceptance.sha256)&&d.sourceClock!=null&&Hash(d.sourceClock.sha256)&&d.sourceCoverage!=null&&Hash(d.sourceCoverage.sha256),"accepted registry/clock provenance");
   Need(d.wholeCount==142&&d.motionCount==23&&d.stateCount==28&&d.positiveMotionPhases==157&&d.positiveStatePhases==189&&d.zeroDwellEndpoints==23&&d.positiveWholeCount==142&&d.endpointOnlyWhole==null&&d.unknownSourceKeys==107&&d.keys!=null&&d.keys.Length==142&&d.motions!=null&&d.motions.Length==23&&d.states!=null&&d.states.Length==28,"full142/23/28 finite cardinality");
   foreach(var k in d.keys){
    Need(k!=null&&Name(k.key)&&k.file=="whole/"+k.key+".png"&&k.png!=null&&!string.IsNullOrEmpty(k.png.path)&&Hash(k.png.sha256)&&Hash(k.rgbaSha256)&&k.width>0&&k.height>0&&k.width<=8192&&k.height<=8192,"native whole metadata");
    Need(k.root!=null&&k.root.Length==2&&Finite(k.root[0])&&Finite(k.root[1])&&k.pivot!=null&&k.pivot.Length==2&&Finite(k.pivot[0])&&Finite(k.pivot[1])&&k.pivot[0]==k.root[0]/k.width&&k.pivot[1]==1-k.root[1]/k.height,"signed root/pivot, no clamp");
    keys.Add(k.key,new SheriffWholeKeyInfoR227(k));
   }
   int positives=0,exactOpenings=0;var used=new HashSet<string>(StringComparer.Ordinal);
   foreach(var m in d.motions){
    Need(m!=null&&!string.IsNullOrEmpty(m.id)&&m.id!="DamageRevenge"&&Finite(m.duration)&&m.duration>0&&Finite((float)m.duration)&&m.phases!=null&&m.phases.Length>0&&m.outOfRangePolicy=="LAST_POSITIVE_WHOLE_DISPLAY_HOLD_UNPROVEN_NO_ENDPOINT_DWELL","motion clock/policy");
    var copy=new Motion{duration=(float)m.duration,phases=new Phase[m.phases.Length]};double last=0;
    for(int i=0;i<m.phases.Length;i++){
     var p=m.phases[i];Need(p!=null&&Finite(p.at)&&Finite(p.until)&&p.at==last&&p.until>p.at&&p.until<=m.duration&&(float)p.until>(float)p.at&&keys.ContainsKey(p.key)&&!string.IsNullOrEmpty(p.sourcePose),"positive contiguous half-open intervals/key");
     Need(p.sourceSignatureKnown&&!p.continuousSourceEqualityClaimed&&(p.openingSampleGeometryEquality=="EXACT"||p.openingSampleGeometryEquality=="NOT_EXACT"||p.openingSampleGeometryEquality=="UNKNOWN"),"sample-only source equality, separate geometry");
     Need(p.intervalClassification=="CONTINUOUS_SOURCE_CHANNEL_STATIC_DISPLAY_HOLD"||p.intervalClassification=="FINITE_DISPLAY_HOLD_CONTINUOUS_SOURCE_EQUALITY_UNPROVEN","finite HOLD classification");
     copy.phases[i]=new Phase{at=(float)p.at,until=(float)p.until,key=p.key,pose=p.sourcePose,classification=p.intervalClassification,geometry=p.openingSampleGeometryEquality,exact=p.sourceExactAtOpeningSample};last=p.until;positives++;if(p.sourceExactAtOpeningSample)exactOpenings++;used.Add(p.key);
    }
    Need(last==m.duration&&m.endpoint!=null&&m.endpoint.zeroDwell&&m.endpoint.duration==0&&m.endpoint.at==m.duration&&m.endpoint.until==m.duration,"endpoint is metadata with zero positive dwell");
    bool noWhole=m.id=="Wait"||m.id=="AttackA_Ready";Need(noWhole?m.endpoint.key==null:!string.IsNullOrEmpty(m.endpoint.key)&&keys.ContainsKey(m.endpoint.key),"Wait unknown and Ready unassigned endpoints cannot borrow art");
    Need(m.endpoint.sourceKnown==(m.id!="Wait")&&(m.endpoint.sourceKnown?!string.IsNullOrEmpty(m.endpoint.sourcePose):m.endpoint.sourcePose==null&&!m.endpoint.sourceExactToArtAnchor)&&(!noWhole||!m.endpoint.sourceExactToArtAnchor),"endpoint source and art are separate");
    copy.endpoint=new SheriffEndpointInfoR227(m.id,m.endpoint,noWhole?null:keys[m.endpoint.key]);motions.Add(m.id,copy);
   }
   Need(positives==157&&exactOpenings==154&&used.Count==142,"positive exposure accounting distinct from endpoint metadata");
   int statePositives=0;
   foreach(var s in d.states){Need(s!=null&&!string.IsNullOrEmpty(s.id)&&s.id!="NASOD_ELITE_SHERIFF_DAMAGE_REVENGE"&&s.motion!="DamageRevenge"&&!string.IsNullOrEmpty(s.clip)&&motions.ContainsKey(s.motion)&&Finite(s.duration)&&s.duration>=0&&Finite((float)s.duration)&&s.motionDuration==(double)d.motions[Array.FindIndex(d.motions,m=>m.id==s.motion)].duration&&s.speed==1&&!s.hasSourceShow&&!s.sourceShow&&Hash(s.originalStateSHA256)&&Finite(s.originalAddRotateZ)&&s.originalAddRotateZ==0,"original state clocks/speed/no authored SHOW");states.Add(s.id,new SheriffWholeStateInfoR227(s));statePositives+=motions[s.motion].phases.Length;}
   Need(statePositives==189,"state exposure accounting");
  }
  public SheriffWholeKeyInfoR227[] Keys(){var a=new SheriffWholeKeyInfoR227[keys.Count];keys.Values.CopyTo(a,0);return a;}
  public SheriffWholeStateInfoR227 State(string id){SheriffWholeStateInfoR227 s;if(!states.TryGetValue(id??"",out s))throw new ArgumentException("No Sheriff display state: "+id);return s;}
  public SheriffEndpointInfoR227 EndpointDiagnostic(string motion){Motion m;if(!motions.TryGetValue(motion??"",out m))throw new ArgumentException("No Sheriff motion endpoint: "+motion);return m.endpoint;}
  public SheriffWholeSelectionR227 Select(string state,float npcClock){
   Need(Finite(npcClock)&&npcClock>=0,"invalid existing NPC clock");var s=State(state);var m=motions[s.Motion];float clock=npcClock*s.Speed;Need(Finite(clock),"clock overflow");
   if(s.Loop)clock%=m.duration;bool outside=!s.Loop&&clock>=m.duration;
   // Never select the zero-dwell endpoint as a positive phase. When a source state
   // remains beyond its finite strip, hold its last positive whole and expose the
   // uncertainty; this is not approved strip exposure or a gameplay transition.
   int index=0;for(int i=1;i<m.phases.Length;i++)if(m.phases[i].at<=clock)index=i;else break;
   var p=m.phases[index];return new SheriffWholeSelectionR227(keys[p.key],s,index,npcClock,clock,p.pose,outside?"AFTER_FINITE_RANGE_DISPLAY_HOLD_UNPROVEN":p.classification,p.geometry,outside,!outside&&clock==p.at&&p.exact);
  }
 }
}
