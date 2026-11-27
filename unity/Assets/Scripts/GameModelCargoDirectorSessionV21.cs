using System;using System.Collections.Generic;
using ElswordPixel.CargoWallyDirectorDraftV21;
namespace ElswordPixel {
 // Host-owned explicit source movie session. GameModel.Step does not call
 // this and does not run a synthetic director release timer.
 public sealed class CargoDirectorHostSessionV21 {
  readonly GameModel model;readonly CargoWallyDirectorClockV21 clock;CargoDirectorHostTicketV21 ticket;readonly List<DirectorEventV21> pending=new List<DirectorEventV21>();public int AcceptedReleaseCues{get;private set;}public DirectorPhaseV21 Phase{get{return clock.Phase;}}public double MovieTime{get{return clock.MovieTime;}}
  public CargoDirectorHostSessionV21(GameModel game,ClockInputV21 source){if(game==null)throw new ArgumentNullException("game");model=game;clock=new CargoWallyDirectorClockV21(source);}
  public bool ObserveActualSourceRequests(string stage,string cf,string rf){CargoDirectorHostTicketV21 t;if(!model.TryBeginCargoDirectorFromSourceRequestsV21(stage,cf,rf,out t))return false;ticket=t;clock.ResetRoom(t.roomEpoch);clock.EnterBoundSourceState(t.sourceNPC,t.startState,t.roomEpoch);Drain();return true;}
  void Drain(){foreach(var e in clock.DrainEvents()){pending.Add(e);if(e.type=="boss-existing-state-change-request"){if(!model.AcceptCargoDirectorSourceCueV21(ticket,e))throw new InvalidOperationException("Actual source Clock release refused by current model Host ticket");AcceptedReleaseCues++;}else if(e.type=="director-state-ended"){if(!model.AcceptCargoDirectorSourceCueV21(ticket,e))throw new InvalidOperationException("Current source director retirement refused");}}}
  public void AdvanceSourceMovie(double sourceClockDelta){if(ticket==null)throw new InvalidOperationException("Actual source CF/RF/key1 activation required");if(ticket.roomEpoch!=model.CargoDirectorRoomEpochV21){ticket=null;clock.ResetRoom(model.CargoDirectorRoomEpochV21);pending.Clear();return;}clock.Step(sourceClockDelta);Drain();}
  public DirectorEventV21[] DrainTypedPresentationRequests(){var a=pending.ToArray();pending.Clear();return a;}
  public void ResetForRoom(){ticket=null;clock.ResetRoom(model.CargoDirectorRoomEpochV21);pending.Clear();AcceptedReleaseCues=0;}
 }
}
