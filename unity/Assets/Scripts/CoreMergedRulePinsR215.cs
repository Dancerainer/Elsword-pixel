using System;

namespace ElswordPixel {
 // The original document is finite display/source-clock evidence only.
 // It is never installed as the active gameplay rule set by this candidate.
 public static class CoreMergedRulePinsR215 {
  public const string ActiveGameplayRulesSha256="1466d19a6ccd90b6286ea95f1e012badde922ab5ddce7583dcbb12cdd2d14c6d";
  public const string OriginalDisplayClockRulesSha256="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
  public static void Validate(string activeGameplayRulesSha256,string originalDisplayClockRulesSha256){
   if(activeGameplayRulesSha256!=ActiveGameplayRulesSha256||originalDisplayClockRulesSha256!=OriginalDisplayClockRulesSha256)
    throw new ArgumentException("R215 requires exact R105 active gameplay rules and exact original finite display-clock evidence.");
  }
 }
}
