using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool caveIterationApproved,belowPassageIterationApproved,outskirtsIterationApproved;
  const string CaveSourceDungeon="Dungeon_Elder_Benders_Cave_Normal";
  static readonly string[] CaveRuntimeNpcs={"NUI_BAT_A","NUI_BAT_B","NUI_THIEF_THIN","NUI_THIEF_FAT","NUI_THIEF_FAT_BOSS","NUI_PPORU_GIANT_RED","NUI_BENDERS_UPGRADE_NORMAL","NUI_DOWN_STONE_TRAP","NUI_SMALLGATE_POST_BENDERS_CAVE_A","NUI_SMALLGATE_BENDERS_CAVE_A","NUI_SMALLGATE_BOSS_POST_BENDERS_CAVE","NUI_SMALLGATE_BOSS_BENDERS_CAVE"};
  static readonly string[] CaveRuntimeEffects={"DamageEffect_BAT_A_ATTACK_A","DamageEffect_BENDERS_NORMAL_SPECIAL_ATTACK_A","DamageEffect_BENDERS_NORMAL_SPECIAL_ATTACK_A_EFFECTSET","DamageEffect_BENDERS_UPGRADE_NORMAL_ATTACK_A","DamageEffect_BENDERS_UPGRADE_NORMAL_ATTACK_B","DamageEffect_BENDERS_UPGRADE_NORMAL_ATTACK_B2","DamageEffect_PPORU_GIANT_WHITE_ATTACK_A","DamageEffect_PPORU_GIANT_WHITE_ATTACK_A2","DamageEffect_THIEF_FAT_ATTACK_A","DamageEffect_THIEF_FAT_ATTACK_B","DamageEffect_THIEF_THIN_ATTACK_A"};
  static readonly string[] CaveRuntimeWorlds={"WI_ELDER_BENDERS_CAVE_STAGE1","WI_ELDER_BENDERS_CAVE_STAGE1_1","WI_ELDER_BENDERS_STAGE_BOSS"};
  NpcContentSetData CurrentNpcContentSet {
   get {if(catalog.npcContentSets!=null)foreach(var set in catalog.npcContentSets)if(set.sourceDungeon==Dungeon.sourceGraph)return set;return null;}
  }
  bool PackagedNpcSoundsCover(NpcSoundDocument document){
   if(document==null||document.assets==null||catalog.npcSounds==null)return false;
   foreach(var asset in document.assets)if(!Array.Exists(catalog.npcSounds,s=>s!=null&&s.id==asset.id&&!string.IsNullOrEmpty(s.resource)&&s.resource.StartsWith("Pixel/",StringComparison.Ordinal)))return false;
   return true;
  }
  void ValidateAdditionalIterationAdmissions(){
   caveIterationApproved=belowPassageIterationApproved=outskirtsIterationApproved=castleCenterIterationApproved=dragonRoadIterationApproved=lakeIterationApprovedV18=lakeNightIterationApprovedV19=dragonNestIterationApprovedV20=false;postOutskirtsRouteCount=postOutskirtsStageCount=0;
   if(catalog.npcContentSets==null||catalog.npcContentSets.Length==0)return;
   if(catalog.npcContentSets.Length<1||catalog.npcContentSets.Length>16)throw new InvalidOperationException("Unknown additional NPC content scope");
   foreach(var content in catalog.npcContentSets){var minor=content==null?null:content.lakeVisual;if(content!=null&&content.sourceDungeon!=LakeSourceDungeonV18&&content.sourceDungeon!=LakeNightSourceDungeonV19&&minor!=null&&(!string.IsNullOrEmpty(minor.documentResource)||(minor.textures!=null&&minor.textures.Length>0)))throw new InvalidOperationException("Lake minor visuals attached to another source dungeon");}
   foreach(var content in catalog.npcContentSets)if(content!=null&&content.sourceDungeon!=DragonNestSourceDungeonV20&&DragonPackagedResourcePresentV20(content.dragonNestVisual))throw new InvalidOperationException("Dragon visual resources attached to another source dungeon");
   var set=catalog.npcContentSets[0];var a=set==null?null:set.admission;
   if(set==null||set.sourceDungeon!=CaveSourceDungeon||a==null||a.schema!="cave-normal-85-v1"||a.sourceDungeon!=CaveSourceDungeon||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,CaveRuntimeNpcs)||!ExactIds(a.effectIds,CaveRuntimeEffects)||!ExactIds(a.sourceWorldIds,CaveRuntimeWorlds))throw new InvalidOperationException("Unreviewed cave NPC content set");
   int extra=Math.Min(3,catalog.npcContentSets.Length)-1;
   if(catalog.dungeons==null||catalog.dungeons.Length<5+extra||catalog.stages.Length<15+3*extra)throw new InvalidOperationException("Cave iteration route is incomplete");
   var route=catalog.dungeons[4];
   if(route.id!="banthus-cave"||route.sourceGraph!=CaveSourceDungeon||route.start!=12||route.count!=3||route.combatZones!=4||!route.iterationPreview)throw new InvalidOperationException("Cave iteration source route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,n=>n.id),CaveRuntimeNpcs)||set.npcEffectRules==null||set.npcEffectRules.definitions==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,e=>e.id),CaveRuntimeEffects)||set.npcSoundRules==null||set.npcSoundRules.assets==null||set.npcSoundRules.assets.Length!=17||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null)throw new InvalidOperationException("Cave iteration source documents exceed the reviewed scope");
   foreach(var npc in set.npcRules){
    var art=Array.Find(catalog.monsters,m=>m.id==npc.kind);
    if(art==null)throw new InvalidOperationException("Missing cave pixel actor: "+npc.id);
    foreach(var state in npc.states)if(!Array.Exists(art.clips,c=>c.id==state.clip))throw new InvalidOperationException("Missing cave state pixels: "+npc.id+"/"+state.clip);
   }
   foreach(var fx in set.npcEffectRules.definitions)if(!string.IsNullOrEmpty(fx.visualId)&&!Array.Exists(catalog.effects,e=>e.id==fx.visualId&&Array.Exists(e.clips,c=>c.id=="play")))throw new InvalidOperationException("Missing cave effect pixels: "+fx.id);
   foreach(var binding in set.npcEffectRules.bindings)if(Array.IndexOf(CaveRuntimeNpcs,binding.npc)<0)throw new InvalidOperationException("Unexpected cave effect actor");
   foreach(var track in set.npcSoundRules.stateTracks)if(Array.IndexOf(CaveRuntimeNpcs,track.npc)<0)throw new InvalidOperationException("Unexpected cave sound actor");
   foreach(var track in set.npcSoundRules.effectTracks)if(Array.IndexOf(CaveRuntimeEffects,track.effect)<0)throw new InvalidOperationException("Unexpected cave effect sound");
   foreach(var state in set.npcMechanismRules.states)if(Array.IndexOf(CaveRuntimeNpcs,state.npc)<0)throw new InvalidOperationException("Unexpected cave mechanism actor");
   int total=0,enabled=0,inactive=0,environments=0,boxes=0;
   for(int i=0;i<3;i++){
    var stage=catalog.stages[12+i];if(stage.sourceWorldId!=CaveRuntimeWorlds[i])throw new InvalidOperationException("Cave world order changed");
    foreach(var spawn in stage.spawns){
     total++;if(spawn.sourceNPC=="NUI_BOX"){if(spawn.aiEnabled||spawn.kind!="prop-box")throw new InvalidOperationException("Wrong cave box reuse");boxes++;continue;}
     var profile=Array.Find(set.npcRules,n=>n.id==spawn.sourceNPC);if(profile==null||profile.kind!=spawn.kind)throw new InvalidOperationException("Unexpected cave spawn");
     if(!string.IsNullOrEmpty(profile.environmentKind)){if(spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Cave prop became an active combat actor");environments++;}
     else if(spawn.aiEnabled){if(!spawn.countsForClear)throw new InvalidOperationException("Inactive cave actor became active");enabled++;}else inactive++;
    }
   }
   if(total!=39||enabled!=29||inactive!=2||environments!=6||boxes!=2)throw new InvalidOperationException("Cave source roster or ACTIVE initialization changed");
   caveIterationApproved=true;
   if(extra>=1)ValidateBelowPassageIteration(catalog.npcContentSets[1]);
   if(extra>=2)ValidateOutskirtsIteration(catalog.npcContentSets[2]);
   if(catalog.npcContentSets.Length>3)ValidatePostOutskirtsAdmissions();
   else if(catalog.dungeons.Length!=5+extra||catalog.stages.Length!=15+3*extra)throw new InvalidOperationException("Historical source route has orphan stages");
  }
 }
}
