using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string DragonRoadSource="Dungeon_Besma_Dragon_Road_Normal";
  bool dragonRoadIterationApproved;
  static readonly string[] DragonRoadRuntimeNpcs={"NUI_BOX_BESMA1","NUI_CACTUS","NUI_KOMODO","NUI_LIZARDMAN_HIGH","NUI_LIZARDMAN_SAURUS","NUI_LIZARDMAN_SPEAR","NUI_SCORPION","NUI_SMALLGATE_BESMA_DRAGON_ROAD","NUI_SMALLGATE_POST_BESMA_DRAGON_ROAD","NUI_SPIKEBOARD_HERB"};
  static readonly string[] DragonRoadRuntimeEffects={"DamageEffect_CACTUS_ATTACK_A","DamageEffect_KOMODO_ATTACK_A","DamageEffect_LIZARDMAN_HIGH_ATTACK_B","DamageEffect_LIZARDMAN_HIGH_SPECIAL_ATTACK_A","DamageEffect_LIZARDMAN_SAURUS_ATTACK_A","DamageEffect_LIZARDMAN_SAURUS_ATTACK_A2","WALLY_8TH_JUMP_ATTACK_LANDING"};
  static readonly string[] DragonRoadRuntimeWorlds={"WI_NEW_BESMA_DRAGON_ROAD_STAGE1","WI_NEW_BESMA_DRAGON_ROAD_STAGE2A","WI_NEW_BESMA_DRAGON_ROAD_STAGE4"};
  void ValidateDragonRoadIteration(NpcContentSetData set){
   var a=set.admission;
   if(a==null||a.schema!="dragon-road-normal-85-v17"||a.sourceDungeon!=DragonRoadSource||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,DragonRoadRuntimeNpcs)||!ExactIds(a.effectIds,DragonRoadRuntimeEffects)||!ExactIds(a.sourceWorldIds,DragonRoadRuntimeWorlds))throw new InvalidOperationException("Unreviewed Dragon Road content scope");
   if(catalog.dungeons.Length<9||catalog.stages.Length<26)throw new InvalidOperationException("Dragon Road route incomplete");
   var route=catalog.dungeons[8];
   if(route.id!="dragon-road"||route.sourceGraph!=DragonRoadSource||route.region!="bethma"||route.start!=23||route.count!=3||route.combatZones!=5||!route.iterationPreview)throw new InvalidOperationException("Dragon Road original route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,n=>n.id),DragonRoadRuntimeNpcs)||set.npcEffectRules==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,e=>e.id),DragonRoadRuntimeEffects)||set.npcSoundRules==null||set.npcSoundRules.assets.Length!=24||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null)throw new InvalidOperationException("Dragon Road source documents incomplete");
   foreach(var profile in set.npcRules){var art=Array.Find(catalog.monsters,m=>m.id==profile.kind);if(art==null)throw new InvalidOperationException("Missing Dragon Road pixels "+profile.id);foreach(var state in profile.states)if(!Array.Exists(art.clips,c=>c.id==state.clip))throw new InvalidOperationException("Missing Dragon Road source state pixels "+profile.id+"/"+state.clip);}
   foreach(var b in set.npcEffectRules.bindings)if(Array.IndexOf(DragonRoadRuntimeNpcs,b.npc)<0)throw new InvalidOperationException("Unexpected Dragon Road effect actor");
   foreach(var t in set.npcSoundRules.stateTracks)if(Array.IndexOf(DragonRoadRuntimeNpcs,t.npc)<0)throw new InvalidOperationException("Unexpected Dragon Road sound actor");
   foreach(var t in set.npcSoundRules.effectTracks)if(Array.IndexOf(DragonRoadRuntimeEffects,t.effect)<0)throw new InvalidOperationException("Unexpected Dragon Road effect sound");
   foreach(var t in set.npcMechanismRules.states)if(Array.IndexOf(DragonRoadRuntimeNpcs,t.npc)<0)throw new InvalidOperationException("Unexpected Dragon Road mechanism actor");
   int total=0,active=0,boxes=0,traps=0,gates=0;
   for(int i=0;i<3;i++){
    var stage=catalog.stages[23+i];
    if(stage.sourceWorldId!=DragonRoadRuntimeWorlds[i]||stage.spawns.Length!=new[]{22,20,2}[i]||stage.encounters.Length!=new[]{2,2,1}[i]||stage.bgm!=(i==2?"music009_boss":"music039_stage"))throw new InvalidOperationException("Dragon Road original world, BGM or roster changed");
    foreach(var spawn in stage.spawns){
     total++;var profile=Array.Find(set.npcRules,p=>p.id==spawn.sourceNPC);
     if(profile==null||profile.kind!=spawn.kind||spawn.visualScale!=1)throw new InvalidOperationException("Dragon Road source identity or scale changed");
     if(string.IsNullOrEmpty(profile.environmentKind)){if(!spawn.aiEnabled||!spawn.countsForClear||spawn.dormant||spawn.passive)throw new InvalidOperationException("Dragon Road combat actor disabled");active++;}
     else {if(spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Dragon Road prop became an active combat actor");if(profile.environmentKind=="recovery-container")boxes++;else if(profile.environmentKind=="proximity-trap")traps++;else if(profile.environmentKind=="source-gate"||profile.environmentKind=="gate-post")gates++;else throw new InvalidOperationException("Unknown Dragon Road environment");}
    }
   }
   if(total!=44||active!=34||boxes!=4||traps!=2||gates!=4)throw new InvalidOperationException("Dragon Road original activation changed");
   if(set.outskirtsVisual==null||set.outskirtsVisual.textures==null||set.outskirtsVisual.textures.Length!=6||set.outskirtsVisual.documentResource!="Pixel/npc-fx/dragon-road/visual")throw new InvalidOperationException("Dragon Road original visible effects missing");
   dragonRoadIterationApproved=true;
  }
  void ConfigurePackagedDragonRoadVisual(NpcContentSetData set){
   var binding=set==null?null:set.outskirtsVisual;
   RequireOutskirtsVisual(dragonRoadIterationApproved&&binding!=null&&binding.textures!=null&&binding.textures.Length==6,"Dragon Road original visible effects missing");
   var document=Resources.Load<TextAsset>(binding.documentResource);var mesh=Resources.Load<TextAsset>(binding.meshManifestResource);RequireOutskirtsVisual(document&&mesh,"Dragon Road visual documents missing");
   var textures=new Dictionary<string,Texture2D>();foreach(var row in binding.textures){var texture=Resources.Load<Texture2D>(row.resource);RequireOutskirtsVisual(texture&&!textures.ContainsKey(row.key),"Dragon Road original visual texture missing");textures.Add(row.key,texture);}
   ConfigureOutskirtsNpcVisual(JsonUtility.FromJson<OutskirtsNpcVisualDocument>(document.text),JsonUtility.FromJson<OutskirtsNpcMeshManifest>(mesh.text),textures);
  }
 }
}
