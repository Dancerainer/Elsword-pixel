using System;
namespace ElswordPixel {
 public static class BelowPixelStateClipV21 {
  public static string OverrideClip(string npc,string sourceState) {
   if(npc=="NUI_BAT_SONIC"&&sourceState=="BAT_SONIC_JUMP_LANDING")return "idle-land";
   if(npc=="NUI_NASOD_MINER_THIN_DEFECT_DIE"&&sourceState=="NASOD_MINER_THIN_DEFECT_DIE_START")return "start-idle";
   if(npc=="NUI_NASOD_MINER_FAT_C_DEFECT_DIE"&&sourceState=="NASOD_MINER_FAT_C_DEFECT_DIE_START")return "start-idle";
   if(npc=="NUI_PARASITE"&&(sourceState=="PARASITE_START"||sourceState=="PARASITE_HEAL"||sourceState=="PARASITE_JUMP_LANDING"))return "idle-once";
   return null;
  }
  public static string Resolve(string npc,string sourceState,string modelClip,bool nonLoopOverrideLoaded) {
   string selected=OverrideClip(npc,sourceState);if(selected==null)return modelClip;
   if(!nonLoopOverrideLoaded)throw new InvalidOperationException("Original source state requires its reviewed nonloop visual clip: "+npc+"/"+sourceState+"/"+selected);
   return selected;
  }
 }
}
