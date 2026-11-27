using System;
using System.Collections.Generic;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool forestIterationApproved;
  bool startedSourceUnionApprovedV29;
  static readonly string[] StartedSourceUnionNpcsV29={"NUI_PPORU_GIANT_WHITE","NUI_ANCIENT_PPORU","NUI_THIEF_THIN_TOTO","NUI_FAIRY_GUARDIAN"};
  static readonly string[] StartedSourceUnionEffectsV29={"DamageEffect_ANCIENT_PPORU_ATTACK_A","DamageEffect_ANCIENT_PPORU_ATTACK_B","DamageEffect_PPORU_GIANT_WHITE_ATTACK_A","DamageEffect_PPORU_GIANT_WHITE_ATTACK_A2","DamageEffect_ANCIENT_PPORU_SPECIAL_ATTACK","DamageEffect_THIEF_THIN_TOTO_MAGIC_ATTACK_A","DamageEffect_FAIRY_GUARDIAN_ATTACK_A","DamageEffect_FAIRY_GUARDIAN_TRAP_MODE_START"};
  static readonly string[] ForestIterationEffects={"DamageEffect_ANGER_FOREST_ENT_ATTACK_B","DamageEffect_ANGER_FOREST_ENT_Attack_B","DamageEffect_ANGER_FOREST_ENT_DarkBall","DamageEffect_ANGER_FOREST_ENT_DarkBall_EXPLOSION","DamageEffect_ANGER_FOREST_ENT_Special_Attack","DamageEffect_CURSE_WISP_ATTACK_A","DamageEffect_Forest_Dark_Magician_DarkBall_EXPLOSION_S","DamageEffect_Forest_Dark_Magician_DarkBall_S"};
  static readonly string[] ForestIterationWorlds={"WI_ELDER_DARK_FOREST_STAGE_0","WI_ELDER_DARK_FOREST_STAGE_1","WI_ELDER_DARK_FOREST_BOSS_STAGE"};
  static bool ExactIds(string[] a,string[] b){if(a==null||b==null||a.Length!=b.Length)return false;var set=new HashSet<string>(a,StringComparer.Ordinal);return set.Count==a.Length&&set.SetEquals(b);}
  static bool EvidenceHash(string text){if(text==null||text.Length!=64)return false;foreach(char c in text)if(!Uri.IsHexDigit(c))return false;return true;}
  bool IterationNpc(string id){return forestIterationApproved&&(ForestIterationActivation.IsForestNpc(id)||startedSourceUnionApprovedV29&&Array.IndexOf(StartedSourceUnionNpcsV29,id)>=0);}
  bool IterationEffect(string id){return forestIterationApproved&&(Array.IndexOf(ForestIterationEffects,id)>=0||startedSourceUnionApprovedV29&&Array.IndexOf(StartedSourceUnionEffectsV29,id)>=0);}
  bool ExactStartedSourceUnionV29(string[] originalNpcs){
   if(catalog.npcRules==null||catalog.npcRules.Length!=9||catalog.npcEffectRules==null||catalog.npcEffectRules.definitions==null||catalog.npcEffectRules.definitions.Length!=16)return false;
   var npcs=new List<string>(originalNpcs);npcs.AddRange(StartedSourceUnionNpcsV29);
   var effects=new List<string>(ForestIterationEffects);effects.AddRange(StartedSourceUnionEffectsV29);
   if(!ExactIds(Array.ConvertAll(catalog.npcRules,n=>n.id),npcs.ToArray())||!ExactIds(Array.ConvertAll(catalog.npcEffectRules.definitions,e=>e.id),effects.ToArray()))return false;
   // The completed overlay bridge validates the unchanged Main raw baseline,
   // exact overlay bytes and typed delta before returning a new catalog DTO.
   return CompletedRedrawCatalogReadyV31();
  }
  void ValidateForestIterationAdmission(){
   ValidateAdditionalIterationAdmissions();
   forestIterationApproved=false;startedSourceUnionApprovedV29=false;var a=catalog.iterationAdmission;
   if(a!=null&&string.IsNullOrEmpty(a.schema)&&!a.reviewed&&(a.npcIds==null||a.npcIds.Length==0))catalog.iterationAdmission=a=null;
   if(catalog.npcSoundRules!=null&&(catalog.npcSoundRules.assets==null||catalog.npcSoundRules.assets.Length==0)&&(catalog.npcSoundRules.stateTracks==null||catalog.npcSoundRules.stateTracks.Length==0)&&(catalog.npcSoundRules.effectTracks==null||catalog.npcSoundRules.effectTracks.Length==0))catalog.npcSoundRules=null;
   if(catalog.npcMechanismRules!=null&&(catalog.npcMechanismRules.states==null||catalog.npcMechanismRules.states.Length==0)&&string.IsNullOrEmpty(catalog.npcMechanismRules.sourceRulesSha256))catalog.npcMechanismRules=null;
   if(a!=null){
    var npcs=new[]{"NUI_FOREST_DARK_MAGICIAN","NUI_CURSE_WISP","NUI_ANGER_FOREST_KNIGHT","NUI_ANGER_FOREST_FAIRY","NUI_ANGER_FOREST_ENT"};
    bool valid=a.schema=="forest-normal-85-v1"&&a.sourceDungeon=="Dungeon_Elder_Druid_Forest_Normal"&&a.reviewed&&EvidenceHash(a.bundleSha256)&&EvidenceHash(a.reviewSha256)&&ExactIds(a.npcIds,npcs)&&ExactIds(a.effectIds,ForestIterationEffects)&&ExactIds(a.sourceWorldIds,ForestIterationWorlds)&&catalog.dungeons!=null&&catalog.dungeons.Length==(outskirtsIterationApproved?7:belowPassageIterationApproved?6:caveIterationApproved?5:4)+postOutskirtsRouteCount&&catalog.stages.Length==(outskirtsIterationApproved?21:belowPassageIterationApproved?18:caveIterationApproved?15:12)+postOutskirtsStageCount;
    if(valid){var route=catalog.dungeons[3];valid=route.id=="dark-forest"&&route.sourceGraph==a.sourceDungeon&&route.start==9&&route.count==3&&route.iterationPreview;
     for(int i=0;i<3;i++)valid&=catalog.stages[9+i].sourceWorldId==ForestIterationWorlds[i];
    }
    if(!valid)throw new InvalidOperationException("Invalid scoped forest iteration admission; other candidate routes remain blocked");
    bool legacyForestRules=catalog.npcRules!=null&&catalog.npcRules.Length==5&&ExactIds(Array.ConvertAll(catalog.npcRules,n=>n.id),npcs)&&catalog.npcEffectRules!=null&&catalog.npcEffectRules.definitions!=null&&ExactIds(Array.ConvertAll(catalog.npcEffectRules.definitions,e=>e.id),ForestIterationEffects);
    startedSourceUnionApprovedV29=!legacyForestRules&&ExactStartedSourceUnionV29(npcs);
    if(!legacyForestRules&&!startedSourceUnionApprovedV29)throw new InvalidOperationException("Forest iteration source rules exceed the reviewed scope");
    int enabled=0,inactive=0,props=0,total=0;for(int i=9;i<12;i++)foreach(var s in catalog.stages[i].spawns){total++;if(ForestIterationActivation.IsForestNpc(s.sourceNPC)){if(s.aiEnabled){if(!s.countsForClear)throw new InvalidOperationException("Original inactive forest actor enabled");enabled++;}else inactive++;}else{if(s.aiEnabled||s.sourceNPC!="NUI_BOX"&&s.sourceNPC!="NUI_ELDER_DRUID_FOREST_GATE")throw new InvalidOperationException("Unreviewed forest actor activated");props++;}}
    if(total!=26||enabled!=20||inactive!=2||props!=4)throw new InvalidOperationException("Forest iteration requires 20 source-active actors, two original inactive mages and four props");
    if(catalog.npcSoundRules==null||catalog.npcMechanismRules==null||catalog.npcSounds==null||!ExpectedFairySourceSoundUnionV34(legacyForestRules)||!PackagedNpcSoundsCover(catalog.npcSoundRules))throw new InvalidOperationException("Forest sound/mechanism sidecars are missing");
    forestIterationApproved=true;
   }
   if(catalog.npcSoundRules!=null){var d=catalog.npcSoundRules;if(!forestIterationApproved&&(!d.runtimeAdmitted||!d.sourceMechanismsComplete))throw new InvalidOperationException("Unadmitted NPC sound document");if(forestIterationApproved){foreach(var t in d.stateTracks)if(!IterationNpc(t.npc))throw new InvalidOperationException("Unreviewed forest sound actor");foreach(var t in d.effectTracks)if(!IterationEffect(t.effect))throw new InvalidOperationException("Unreviewed forest effect sound");}}
   if(catalog.npcMechanismRules!=null){var d=catalog.npcMechanismRules;if(!forestIterationApproved&&(!d.runtimeAdmitted||!d.sourceMechanismsComplete))throw new InvalidOperationException("Unadmitted NPC mechanism document");if(forestIterationApproved)foreach(var s in d.states)if(!IterationNpc(s.npc))throw new InvalidOperationException("Unreviewed forest mechanism actor");}
  }
 }
}
