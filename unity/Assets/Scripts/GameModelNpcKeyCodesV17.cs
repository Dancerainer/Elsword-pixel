using System;

namespace ElswordPixel {
 public sealed partial class GameModel {
  // CC_KILL_KEYCODE_NPC names original spawn KEY_CODEs, not monster types.
  // An inactive/dormant or not-yet-created key actor must not count as dead.
  // Original client native scheduling is not claimed by this state predicate.
  bool SourceNpcKeyCodesSatisfied(int[] sourceKeyCodes) {
   if(!HasEncounters||sourceKeyCodes==null||sourceKeyCodes.Length==0)return false;
   foreach(int key in sourceKeyCodes) {
    if(key<=0)return false; // zero means no source KEY_CODE, never a target.
    bool found=false;
    foreach(var actor in enemies) {
     if(actor.encounterIndex!=currentEncounter||actor.sourceKeyCode!=key)continue;
     found=true;
     if(actor.hp>0)return false;
    }
    if(!found)return false;
   }
   return true;
  }
 }
}
