using System;
using System.Collections.Generic;
namespace ElswordPixel {
 // Value snapshots emitted by the existing source owner. They schedule no
 // gameplay effect, hit, sound, AI decision or clock advancement.
 public sealed class PlainShockParticleEventR1480 {
  public readonly Enemy Actor;
  public readonly int Serial,Generation,Room,Facing,Kind;
  public readonly float Time,X,Y,NativeDepth,LandY,VisualScale;
  internal PlainShockParticleEventR1480(Enemy actor,int serial,int generation,int room,int kind,float time){
   Actor=actor;Serial=serial;Generation=generation;Room=room;Kind=kind;Time=time;
   X=actor.x;Y=actor.y;NativeDepth=actor.nativeDepth;LandY=actor.y;Facing=actor.facing;VisualScale=actor.visualScale;
  }
 }
 public sealed partial class GameModel {
  readonly List<PlainShockParticleEventR1480> plainShockParticleEventsR1480=new List<PlainShockParticleEventR1480>();
  int plainShockParticleSerialR1480,plainShockParticleGenerationR1480=-1;
  public IList<PlainShockParticleEventR1480> PlainShockParticleEventsR1480 {get{return plainShockParticleEventsR1480.AsReadOnly();}}
  void ClearPlainShockParticleEventsR1480(){plainShockParticleEventsR1480.Clear();plainShockParticleGenerationR1480=-1;}
  void RecordPlainShockParticleEventR1480(Enemy actor,int kind,float at){
   if(actor==null||kind<0||kind>2||!PlainShockScopeR1477(actor,NpcProfile(actor))||float.IsNaN(at)||float.IsInfinity(at)||at<0)
    throw new InvalidOperationException("Changed original Plain Shock particle event scope/time");
   if(plainShockParticleGenerationR1480!=NpcPresentationGeneration){plainShockParticleEventsR1480.Clear();plainShockParticleGenerationR1480=NpcPresentationGeneration;}
   plainShockParticleEventsR1480.RemoveAll(e=>e.Time<elapsed-4);
   if(plainShockParticleEventsR1480.Count>=512)throw new InvalidOperationException("Plain Shock original visual event budget exceeded");
   plainShockParticleEventsR1480.Add(new PlainShockParticleEventR1480(actor,++plainShockParticleSerialR1480,NpcPresentationGeneration,room,kind,at));
  }
 }
}
