using System;
using System.Collections.Generic;
namespace ElswordPixel.CargoDirectorControlledPrivateV21.Clock {
 [Serializable] public sealed class CueV21 {public string type,name,payload;public double time,duration;}
 [Serializable] public sealed class CameraFrameV21 {public double time;public double[] eye,target,humanRoot,armorRoot;}
 [Serializable] public sealed class ClockInputV21 {public string sourceNPC,startState,dyingState,releaseCallback;public double startSeconds,dyingSeconds,unit,bossNameDelayRoomSeconds;public double[] commonReferenceOffset;public CueV21[] cues;public CameraFrameV21[] cameraFrames;}
 public sealed class DirectorEventV21 {public string type,name,payload;public double movieTime,duration;public double[] nativePosition;public long roomEpoch;}
 public enum DirectorPhaseV21 {Unbound,Start,Dying,Ended}
 // Explicit source-state presentation adapter. This never calls a Lua/native
 // ABI, guesses room trigger activation, spawns an actor, or writes boss HP.
 public sealed class CargoWallyDirectorClockV21 {
  public const string NPC="NUI_EVENT_NPC_WALLY_9TH_ARMOR",StartState="EVENT_NPC_WALLY_9TH_ARMOR_START",DyingState="EVENT_NPC_WALLY_9TH_ARMOR_DYING",ReleaseCallback="EVENT_NPC_WALLY_9TH_ARMOR_DYING_START_START";
  readonly ClockInputV21 input;readonly List<DirectorEventV21> pending=new List<DirectorEventV21>();int nextCue;bool consumed;long epoch;double clock;DirectorPhaseV21 phase;
  public CargoWallyDirectorClockV21(ClockInputV21 data){if(data==null||data.sourceNPC!=NPC||data.startState!=StartState||data.dyingState!=DyingState||data.releaseCallback!=ReleaseCallback||Math.Abs(data.startSeconds-63200.0/4800)>1e-10||Math.Abs(data.dyingSeconds-800.0/4800)>1e-10||data.cameraFrames==null||data.cameraFrames.Length!=791||data.cues==null)throw new ArgumentException("Wrong source director binding");input=data;for(int i=1;i<data.cues.Length;i++)if(data.cues[i].time<data.cues[i-1].time)throw new ArgumentException("Cue order must retain source chronology");phase=DirectorPhaseV21.Unbound;}
  public DirectorPhaseV21 Phase{get{return phase;}}public double MovieTime{get{return clock;}}public long RoomEpoch{get{return epoch;}}
  void Emit(string type,string name,double at,string payload=null,double duration=0,double[] position=null){pending.Add(new DirectorEventV21{type=type,name=name,movieTime=at,payload=payload,duration=duration,nativePosition=position,roomEpoch=epoch});}
  public void ResetRoom(long roomEpoch){pending.Clear();epoch=roomEpoch;clock=0;nextCue=0;consumed=false;phase=DirectorPhaseV21.Unbound;Emit("room-reset","clear-director-owned-camera-caption-effects",0);}
  public bool EnterBoundSourceState(string sourceNPC,string state,long roomEpoch){
   if(sourceNPC!=NPC||state!=StartState||roomEpoch!=epoch||consumed)return false;consumed=true;phase=DirectorPhaseV21.Start;clock=0;nextCue=0;Emit("state-enter",StartState,0);Emit("source-position-request","SetPositionForce",0,null,0,new double[]{0,0,0});Emit("source-movie-declaration","Start camera/focus/stop/direction/effectset",0);DispatchCues(0);return true;
  }
  void DispatchCues(double through){while(nextCue<input.cues.Length&&input.cues[nextCue].time<=through+1e-10){var c=input.cues[nextCue++];Emit(c.type,c.name,c.time,c.payload,c.duration);}}
  public void Step(double dt){
   if(Double.IsNaN(dt)||Double.IsInfinity(dt)||dt<0)throw new ArgumentOutOfRangeException("dt");if(phase!=DirectorPhaseV21.Start&&phase!=DirectorPhaseV21.Dying)return;double next=Math.Min(input.startSeconds+input.dyingSeconds,clock+dt);
   if(phase==DirectorPhaseV21.Start){DispatchCues(Math.Min(next,input.startSeconds));if(next>=input.startSeconds-1e-10){phase=DirectorPhaseV21.Dying;Emit("state-enter",DyingState,input.startSeconds);Emit("boss-existing-state-change-request",ReleaseCallback,input.startSeconds,"NUI_WALLY_9TH_ARMOR/WALLY_9TH_ARMOR_START");Emit("source-position-request","SetPositionForce",input.startSeconds,null,0,new double[]{0,-5000,0});Emit("source-state-camera-declaration-ended",DyingState,input.startSeconds);}}
   clock=next;if(phase==DirectorPhaseV21.Dying&&clock>=input.startSeconds+input.dyingSeconds-1e-10){phase=DirectorPhaseV21.Ended;Emit("director-state-ended",DyingState,input.startSeconds+input.dyingSeconds);}
  }
  // Source60Hz extraction uses documented software SRT interpolation. Hold
  // exact extracted sample; no unproved camera FOV/native interpolation.
  public CameraFrameV21 CurrentSourceCamera(){if(phase!=DirectorPhaseV21.Start)return null;int i=Math.Min(input.cameraFrames.Length-1,Math.Max(0,(int)Math.Floor(clock*60+1e-8)));return input.cameraFrames[i];}
  public DirectorEventV21[] DrainEvents(){var a=pending.ToArray();pending.Clear();return a;}
 }
}
