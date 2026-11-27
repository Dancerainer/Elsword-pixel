using System;
namespace ElswordPixel {
 [Serializable] public sealed class NativeMovementTraceSampleV17 {
  public string kind,state,clip,command;public bool isPlayer,grounded;
  public float time,oldX,oldY,targetX,x,y,vx,vy;public int room,groundLine;
 }
 public sealed partial class GameModel {
  // Optional diagnostic sink. Ordinary gameplay allocates no trace samples.
  public Action<NativeMovementTraceSampleV17> NativeMovementTraceV17;
  void TraceNativeMovementV17(Fighter actor,string kind,float oldX,float oldY,float targetX){
   var sink=NativeMovementTraceV17;if(sink==null)return;
   sink(new NativeMovementTraceSampleV17{kind=kind,isPlayer=object.ReferenceEquals(actor,player),time=elapsed,room=room,oldX=oldX,oldY=oldY,targetX=targetX,x=actor.x,y=actor.y,vx=actor.vx,vy=actor.vy,groundLine=actor.groundLine,state=actor.state.ToString(),clip=actor.clip,command=CurrentCommand,grounded=Grounded(actor)});
  }
 }
}
