using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class OutskirtsNpcVisualResourceTexture {public string key,resource;}
 [Serializable] public sealed class OutskirtsNpcVisualResources {public string documentResource,meshManifestResource;public OutskirtsNpcVisualResourceTexture[] textures;}
 public sealed partial class DemoScene {
  const string OutskirtsSourceDungeon="Dungeon_Elder_Wally_Castle_Roof_Normal";
  static readonly string[] OutskirtsRuntimeNpcs={"NUI_MONKEY_BOMB","NUI_PPORU_WILLIAM_THIEF","NUI_PPORU_WILLIAM_THIEF_FIRE","NUI_PPORU_WILLIAM_B","NUI_PPORU_WILLIAM_C","NUI_WOOD_POST_TRAP","NUI_SMALLGATE_WALLY_CASTLE_A","NUI_SMALLGATE_POST_WALLY_CASTLE_A"};
  static readonly string[] OutskirtsRuntimeEffects={"DamageEffect_MONKEY_BOMB_ATTACK_A","DamageEffect_MONKEY_BOMB_ATTACK_A_EXPLOSION","DamageEffect_PPORU_WILLIAM_B_ATTACK_A","DamageEffect_PPORU_WILLIAM_B_ATTACK_B","DamageEffect_PPORU_WILLIAM_B_ATTACK_C","DamageEffect_PPORU_WILLIAM_B_ATTACK_C2","DamageEffect_PPORU_WILLIAM_THIEF_ATTACK_A","DamageEffect_PPORU_WILLIAM_THIEF_FIRE_ATTACK_A","DamageEffect_WILLIAM_PPORU_SPECIAL_ATTACK"};
  static readonly string[] OutskirtsRuntimeWorlds={"WI_ELDER_WALLY_CASTLE_ROOF_STAGE1","WI_ELDER_WALLY_CASTLE_ROOF_STAGE2","WI_ELDER_WALLY_CASTLE_ROOF_STAGE4"};

  void ValidateOutskirtsIteration(NpcContentSetData set){
   var a=set==null?null:set.admission;
   if(set==null||set.sourceDungeon!=OutskirtsSourceDungeon||a==null||a.schema!="outskirts-normal-85-v14"||a.sourceDungeon!=OutskirtsSourceDungeon||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,OutskirtsRuntimeNpcs)||!ExactIds(a.effectIds,OutskirtsRuntimeEffects)||!ExactIds(a.sourceWorldIds,OutskirtsRuntimeWorlds))throw new InvalidOperationException("Unreviewed outskirts content scope");
   var route=catalog.dungeons[6];
   if(route.id!="wally-castle-outskirts"||route.sourceGraph!=OutskirtsSourceDungeon||route.start!=18||route.count!=3||route.combatZones!=4||!route.iterationPreview)throw new InvalidOperationException("Outskirts source route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,n=>n.id),OutskirtsRuntimeNpcs)||set.npcEffectRules==null||set.npcEffectRules.definitions==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,e=>e.id),OutskirtsRuntimeEffects)||set.npcSoundRules==null||set.npcSoundRules.assets==null||set.npcSoundRules.assets.Length!=14||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null||set.outskirtsVisual==null||set.outskirtsVisual.textures==null||set.outskirtsVisual.textures.Length!=8)throw new InvalidOperationException("Incomplete outskirts source documents or visuals");
   foreach(var npc in set.npcRules){
    var art=Array.Find(catalog.monsters,m=>m.id==npc.kind);if(art==null)throw new InvalidOperationException("Missing outskirts pixels: "+npc.id);
    foreach(var state in npc.states)if(!Array.Exists(art.clips,c=>c.id==state.clip))throw new InvalidOperationException("Missing outskirts state pixels: "+npc.id+"/"+state.clip);
    if(string.IsNullOrEmpty(npc.environmentKind))foreach(string clip in new[]{"hurt","launch","down","death","jump","fall"})if(!Array.Exists(art.clips,c=>c.id==clip))throw new InvalidOperationException("Missing outskirts reaction pixels: "+npc.id+"/"+clip);
   }
   foreach(var track in set.npcEffectRules.bindings)if(Array.IndexOf(OutskirtsRuntimeNpcs,track.npc)<0)throw new InvalidOperationException("Unexpected outskirts effect actor");
   foreach(var track in set.npcSoundRules.stateTracks)if(Array.IndexOf(OutskirtsRuntimeNpcs,track.npc)<0)throw new InvalidOperationException("Unexpected outskirts sound actor");
   foreach(var track in set.npcSoundRules.effectTracks)if(Array.IndexOf(OutskirtsRuntimeEffects,track.effect)<0)throw new InvalidOperationException("Unexpected outskirts effect sound");
   foreach(var state in set.npcMechanismRules.states)if(Array.IndexOf(OutskirtsRuntimeNpcs,state.npc)<0)throw new InvalidOperationException("Unexpected outskirts mechanism actor");
   int total=0,active=0,inactive=0,traps=0,gates=0,boxes=0,movie=0;
   for(int i=0;i<3;i++){
    var stage=catalog.stages[18+i];if(stage.sourceWorldId!=OutskirtsRuntimeWorlds[i]||stage.spawns.Length!=new[]{15,29,3}[i]||stage.encounters.Length!=new[]{1,2,1}[i])throw new InvalidOperationException("Outskirts world/roster/zone order changed");
    foreach(var spawn in stage.spawns){
     total++;if(!Mathf.Approximately(spawn.visualScale,1))throw new InvalidOperationException("Outskirts source scale applied twice");
     if(spawn.sourceNPC=="NUI_BOX"){if(spawn.kind!="prop-box"||spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Wrong outskirts box reuse");boxes++;continue;}
     if(spawn.sourceNPC=="NUI_EVENT_NPC_WILLIAM_PPORU"){if(spawn.kind!="event-npc-william-pporu"||!spawn.passive||!spawn.dormant||spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Inactive outskirts movie became a combat actor");movie++;continue;}
     var p=Array.Find(set.npcRules,n=>n.id==spawn.sourceNPC);if(p==null||p.kind!=spawn.kind)throw new InvalidOperationException("Unbound outskirts source identity");
     if(p.environmentKind=="proximity-trap"){if(!spawn.passive||spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Wood trap became a clear target");traps++;}
     else if(p.environmentKind=="source-gate"){if(!spawn.isGate||spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Outskirts source gate lock changed");gates++;}
     else if(spawn.aiEnabled){if(!spawn.countsForClear)throw new InvalidOperationException("Inactive outskirts combat enabled");active++;}else inactive++;
    }
   }
   if(total!=47||active!=32||inactive!=2||traps!=5||gates!=4||boxes!=3||movie!=1)throw new InvalidOperationException("Outskirts original roster or activation changed");
   outskirtsIterationApproved=true;
  }

  void ConfigurePackagedOutskirtsVisual(OutskirtsNpcVisualResources binding){
   // JsonUtility materializes omitted inline optional DTOs as empty objects.
   // Only a wholly empty binding is absent; partial bindings still need admission.
   if(binding==null||(string.IsNullOrEmpty(binding.documentResource)&&string.IsNullOrEmpty(binding.meshManifestResource)&&(binding.textures==null||binding.textures.Length==0))){DisposeOutskirtsNpcVisual();return;}
   RequireOutskirtsVisual(outskirtsIterationApproved&&binding.textures!=null&&binding.textures.Length==8,"unreviewed packaged scope");
   var document=Resources.Load<TextAsset>(binding.documentResource);var meshes=Resources.Load<TextAsset>(binding.meshManifestResource);
   RequireOutskirtsVisual(document&&meshes,"missing packaged visual documents");
   var textures=new Dictionary<string,Texture2D>();foreach(var row in binding.textures){var texture=Resources.Load<Texture2D>(row.resource);RequireOutskirtsVisual(texture&&!textures.ContainsKey(row.key),"missing/duplicate packaged source texture");textures.Add(row.key,texture);}
   ConfigureOutskirtsNpcVisual(JsonUtility.FromJson<OutskirtsNpcVisualDocument>(document.text),JsonUtility.FromJson<OutskirtsNpcMeshManifest>(meshes.text),textures);
  }
 }
}
