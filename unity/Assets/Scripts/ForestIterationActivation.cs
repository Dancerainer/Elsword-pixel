namespace ElswordPixel {
 // Candidate models deliberately keep AI disabled during extraction. Only
 // this released forest identity set may consume the source ACTIVE flag.
 public static class ForestIterationActivation {
  public static bool IsForestNpc(string id){return id=="NUI_FOREST_DARK_MAGICIAN"||id=="NUI_CURSE_WISP"||id=="NUI_ANGER_FOREST_KNIGHT"||id=="NUI_ANGER_FOREST_FAIRY"||id=="NUI_ANGER_FOREST_ENT";}
  public static bool ShouldEnable(string id,bool sourceActive,bool countsForClear){return IsForestNpc(id)&&sourceActive&&countsForClear;}
 }
}
