using System.Collections.Generic;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // This safety boundary has no public release method. A separately reviewed
  // START owner must commit a legitimate once-only release before adding here.
  // Until that owner is installed the exact three original STOP births stay held.
  readonly HashSet<Enemy> coreHardStopIntroReleasedV21=new HashSet<Enemy>();
  void ClearCoreHardStopGuardV21(){coreHardStopIntroReleasedV21.Clear();}
  CoreHardEntryActivationBirthV21 CoreHardStopIntroBirthV21(Enemy e){
   if(coreHardV21==null||coreHardEntryActivationV21==null||e==null||
      !coreHardBalancedActorsV21.Contains(e)||CurrentRoom.id!="Dungeon_Altera_Core_Hard-"+room)return null;
   int order=enemies.IndexOf(e);
   foreach(var b in coreHardEntryActivationV21.births){
    if(b.room!=room||b.order!=order||b.npc!=e.sourceNPC||b.managed||!b.sourceActive||!b.sourceStopAtStart)continue;
    if((room==0&&order==32&&b.npc=="NUI_RAVEN_CLONE_B"&&b.initialState=="RAVEN_CLONE_B_START")||
       (room==2&&order==30&&b.npc=="NUI_NASOD_ELEMENT_FIRE_B"&&b.initialState=="NASOD_ELEMENT_FIRE_SOLO_B_START")||
       (room==2&&order==41&&b.npc=="NUI_NASOD_ELEMENT_ICE_B"&&b.initialState=="NASOD_ELEMENT_ICE_SOLO_B_START"))return b;
   }return null;
  }
  bool CoreHardStopIntroPreserveAiV21(Enemy e){return CoreHardStopIntroBirthV21(e)!=null&&!coreHardStopIntroReleasedV21.Contains(e);}
 }
}
