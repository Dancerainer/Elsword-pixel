using System;
namespace ElswordPixel.CargoPresentationV21 {
 // Converts only committed model value snapshots. Repeated display calls do not
 // advance source clocks or recreate requests; a room reset resets the cursor.
 public static class CargoCommittedParticleFeedV21 {
  public static int Consume(GameModel model,CargoFxFeedV21 feed,ref int cursor){
   if(model==null||feed==null)throw new ArgumentNullException();int count=0;
   foreach(var p in model.CopyCargoCommittedParticlesV21(cursor)){
    feed.SubmitObservedParticle(new CargoObservedParticleV21{sequence=feed.LastParticleSequence+1,ownerSerial=p.ownerSerial,kind="spawn",bindingId=p.bindingId,npc=p.npc,state=p.state,sourceInstructionsSha256=p.sourceInstructionsSha256,particleKey=p.particleKey,time=p.time,sourceLocalTime=p.sourceLocalTime,x=p.x,y=p.y,z=p.z,depth=p.depth,facing=p.facing,unit=p.unit,hasSourceLandY=p.hasSourceLandY,landY=p.landY});
    cursor=p.sequence;count++;
   }
   return count;
  }
 }
}
