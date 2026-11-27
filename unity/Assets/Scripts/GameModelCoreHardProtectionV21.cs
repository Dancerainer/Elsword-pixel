using System;
namespace ElswordPixel {
 [Serializable] public sealed class CoreHardProtectionDocumentV21 {
  public string schema,sourceDifficulty,dungeonId,phase1RulesSha256,phase1MechanismsSha256,phase1CoreAfterSha256,clockPolicy,aiPolicy;
  public int enumValue,npcLevel;public bool enabled,nativeExecuted,runtimeAdmitted;
 }
 public sealed partial class GameModel {
  CoreHardProtectionDocumentV21 coreHardProtectionV21;
  public bool CoreHardProtectionConfiguredV21 {get{return coreHardProtectionV21!=null;}}
  public void SetCoreHardProtectionV21(CoreHardProtectionDocumentV21 d){
   if(d!=null&&(coreHardV21==null||d.schema!="core-hard-source-protection-v21"||d.sourceDifficulty!="Hard"||d.dungeonId!="DI_ALTERA_CORE_HARD"||d.enumValue!=1||d.npcLevel!=37||d.nativeExecuted||d.runtimeAdmitted||!d.enabled||d.phase1RulesSha256!=coreHardV21.sourceRulesSha256||d.phase1RulesSha256!="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72"||d.phase1MechanismsSha256!="ef37ff6b1f5d03ab3e804dcf4502d1de856ceed72ac80974622edb5d6f2eabf0"||d.phase1CoreAfterSha256!="128574a9b783993bd74e02a976d467c085c306bd7d468818a1d6144681999ec8"||d.clockPolicy!="loop-source-pose-mod-duration;unwrapped-callback-clock-retained;native-ABI-unexecuted"||d.aiPolicy!="retain-current-source-AI-flag-on-actual-hit-for-exact-Core-managed-actors"))throw new ArgumentException("Changed exact Core Hard source protection adapter");
   coreHardProtectionV21=d;
  }
  void ClearCoreHardProtectionV21(){coreHardProtectionV21=null;}
  bool CoreHardPreserveSourceAiV21(Enemy e){return coreHardProtectionV21!=null&&coreHardV21!=null&&CoreHardActorRuleV21(e)!=null;}
  // Core Lua callbacks keep their independent unwrapped state clock. Source
  // pose protection uses the same looped pose clock as the existing ordinary
  // source NPC runner. Original interval literals remain unchanged.
  float CoreHardProtectionClockV21(Enemy e,NpcStateDefinition s){if(coreHardProtectionV21==null||coreHardV21==null||e==null||s==null||CoreHardActorRuleV21(e)==null||!s.loop||s.duration<=0)return e==null?0:e.npcTime;return e.npcTime%s.duration;}
 }
}
