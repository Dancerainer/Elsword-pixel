using System;
namespace ElswordPixel {
 // Stateless displayselection only. Gameplay clocks, states, sourcehelper positions,
 // facing, protection, hits and original physics remain owned by GameModel.
 public static class WaterwayNativeRenderRulesV26 {
  public static bool Mouse(string npc){return npc=="NUI_MOUSE"||npc=="NUI_MOUSE_NODROP";}
  public static bool Three(string npc){return npc=="NUI_SOLDIER_THIN"||npc=="NUI_SOLDIER_FAT"||npc=="NUI_GHOST_MAGICIAN_RAGGED";}
  static string Canonical(string s){if(s=="launched")return "launch";if(s=="knockdown")return "down";if(s=="getup")return "get-up";return s;}
  public static string Select(bool waterway,string npc,string previous,float modelClock,float previousClock,float velocityY,float deathTime,Func<string,float> duration,out float resultClock){
   resultClock=previousClock;
   if(!waterway||(!Mouse(npc)&&!Three(npc))||string.IsNullOrEmpty(previous)||previous.StartsWith("source-",StringComparison.Ordinal))return previous;
   string selected=Canonical(previous),target=null;
   if(selected=="jump")selected=velocityY<0?"fall":"jump";
   if(Mouse(npc)){
    if(selected=="hurt"||selected=="launch"||selected=="down"||selected=="jump"||selected=="fall"||selected=="death")target="source-"+selected;
    // Mouse has no original StandUp. Its existing explicit Wait recovery stays.
    // Existing30/20/64/50 dense normalcliprectangles and their clocks stay exact.
   }else if(selected=="idle"||selected=="walk"||selected=="attack-ready"||selected=="attack-a-ready"||selected=="attack-a"||selected=="get-up"||selected=="hurt"||selected=="launch"||selected=="down"||selected=="jump"||selected=="fall"||selected=="death")target="source-"+selected;
   if(target==null)return previous;
   float span=duration(target);if(float.IsNaN(span)||float.IsInfinity(span)||span<=0)throw new InvalidOperationException("Missing reviewed waterway native clip: "+npc+"/"+target);
   if(float.IsNaN(modelClock)||float.IsInfinity(modelClock)||modelClock<0||float.IsNaN(deathTime)||float.IsInfinity(deathTime))throw new InvalidOperationException("Invalid waterway native displayclock");
   // The alias exposure table already maps sourceAnimSpeed exactly once. Use
   // modelclock, including recovery where the prior generic display used a
   // shorter painted clip. This is the same clock current reactionhelpers use.
   resultClock=modelClock;
   if(selected=="death")resultClock=Math.Max(0,Math.Min(1,deathTime/1.2f))*span;
   return target;
  }
 }
}
