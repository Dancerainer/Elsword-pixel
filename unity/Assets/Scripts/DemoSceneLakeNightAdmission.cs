using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string LakeNightSourceDungeonV19="Dungeon_Besma_Lake_Night_Normal";
  bool lakeNightIterationApprovedV19;
  static readonly string[] LakeNightRuntimeNpcsV19={"NUI_ARMADILLO","NUI_BESMA_TOTEM_BASE","NUI_BOX_BESMA1","NUI_FIRE_KOMODO","NUI_LIZARDMAN_HIGH_DARK_NEW","NUI_LIZARDMAN_SHAMAN","NUI_LIZARDMAN_SPEAR_DARK","NUI_SAURUS_HIGH","NUI_SMALLGATE_BESMA_LAKE_NIGHT","NUI_SMALLGATE_POST_BESMA_LAKE_NIGHT"};
  static readonly string[] LakeNightRuntimeEffectsV19={"BESMA_TOTEM_BASE_FIRE_ATTACK","DamageEffect_FIRE_KOMODO_ATTACK_A","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_ATTACK_A","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_ATTACK_B","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_ATTACK_B2","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_ATTACK_B3","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_ATTACK_C_END","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_ATTACK_C_END2","DamageEffect_LIZARDMAN_HIGH_DARK_NEW_SPECIAL_ATTACK_A","DamageEffect_LIZARDMAN_SHAMAN_ATTACK_A"};
  static readonly string[] LakeNightRuntimeWorldsV19={"WI_NEW_BESMA_LAKE_NIGHT_STAGE1","WI_NEW_BESMA_LAKE_NIGHT_STAGE2A","WI_BESMA_LAKE_NIGHT_STAGE4"};
  public bool TryBeginAdmittedLakeNightV19(){
   int index=Array.FindIndex(catalog.dungeons,d=>d.sourceGraph==LakeNightSourceDungeonV19);if(index<0)return false;
   if(mapDungeons.Count==0)InitializeDungeonMap();SelectRuntimeDungeonOnMap(index);EnterSelectedDungeon();return true;
  }
  void ValidateLakeNightIterationV19(NpcContentSetData set){
   var a=set.admission;
   if(a==null||a.schema!="besma-lake-night-normal-85-v19"||a.sourceDungeon!=LakeNightSourceDungeonV19||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,LakeNightRuntimeNpcsV19)||!ExactIds(a.effectIds,LakeNightRuntimeEffectsV19)||!ExactIds(a.sourceWorldIds,LakeNightRuntimeWorldsV19))throw new InvalidOperationException("Unreviewed Besma Lake Night scope");
   if(catalog.dungeons.Length<11||catalog.stages.Length<32)throw new InvalidOperationException("Besma Lake Night route incomplete");var route=catalog.dungeons[10];
   if(route.id!="besma-lake-night"||route.sourceGraph!=LakeNightSourceDungeonV19||route.region!="bethma"||route.start!=29||route.count!=3||route.combatZones!=5||!route.iterationPreview)throw new InvalidOperationException("Besma Lake Night source route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,p=>p.id),LakeNightRuntimeNpcsV19)||set.npcEffectRules==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,p=>p.id),LakeNightRuntimeEffectsV19)||set.npcSoundRules==null||set.npcSoundRules.assets.Length!=29||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null||set.npcMechanismRules.lakeVisual==null||set.npcMechanismRules.lakeVisual.schema!="besma-lake-visual-runtime-draft-v18"||set.npcMechanismRules.sourceLakeNight==null||set.npcMechanismRules.sourceLakeNight.schema!="besma-lake-night-source-callbacks-v19")throw new InvalidOperationException("Besma Lake Night source documents missing");
   var mechanisms=set.npcMechanismRules;string rulesHash=mechanisms.sourceRulesSha256;
   if(!EvidenceHash(rulesHash)||mechanisms.sourceLakeNight.sourceRulesSha256!=rulesHash||mechanisms.lakeVisual.sourceRulesSha256!=rulesHash)throw new InvalidOperationException("Night source sidecar rule binding changed");
   foreach(var p in set.npcRules){var art=Array.Find(catalog.monsters,m=>m.id==p.kind);if(art==null)throw new InvalidOperationException("Missing Besma Lake Night pixels "+p.id);foreach(var s in p.states)if(!Array.Exists(art.clips,c=>c.id==s.clip))throw new InvalidOperationException("Missing Besma Lake Night state "+p.id+"/"+s.clip);}
   foreach(var b in set.npcEffectRules.bindings)if(Array.IndexOf(LakeNightRuntimeNpcsV19,b.npc)<0)throw new InvalidOperationException("Unexpected Night effect actor");
   foreach(var t in set.npcSoundRules.stateTracks)if(Array.IndexOf(LakeNightRuntimeNpcsV19,t.npc)<0)throw new InvalidOperationException("Unexpected Night sound actor");
   foreach(var t in set.npcSoundRules.effectTracks)if(Array.IndexOf(LakeNightRuntimeEffectsV19,t.effect)<0)throw new InvalidOperationException("Unexpected Night effect sound");
   foreach(var t in mechanisms.states)if(Array.IndexOf(LakeNightRuntimeNpcsV19,t.npc)<0)throw new InvalidOperationException("Unexpected Night mechanism actor");
   int total=0,active=0,inactive=0,boxes=0,totems=0,gates=0,supports=0;
   for(int i=0;i<3;i++){
    var stage=catalog.stages[29+i];
    if(stage.id!=LakeNightSourceDungeonV19+"-"+i||stage.sourceWorldId!=LakeNightRuntimeWorldsV19[i]||stage.spawns.Length!=new[]{26,32,3}[i]||stage.encounters.Length!=new[]{2,2,1}[i]||stage.surfaces.Length!=new[]{31,45,3}[i]||stage.bgm!=(i==2?"music015_boss":"music012_stage"))throw new InvalidOperationException("Night original semantic room ID, world, BGM or roster changed");supports+=stage.surfaces.Length;
    for(int j=0;j<stage.spawns.Length;j++){var spawn=stage.spawns[j];total++;var p=Array.Find(set.npcRules,n=>n.id==spawn.sourceNPC);if(p==null||p.kind!=spawn.kind||spawn.visualScale!=1)throw new InvalidOperationException("Night source identity or scale changed");
     if(string.IsNullOrEmpty(p.environmentKind)){if(spawn.aiEnabled){if(!spawn.countsForClear||spawn.dormant||spawn.passive)throw new InvalidOperationException("Night combat actor disabled");active++;}else{if(i!=2||j>=2||spawn.sourceNPC!="NUI_SAURUS_HIGH"||spawn.countsForClear)throw new InvalidOperationException("Night original inactive Saurus changed");inactive++;}}
     else{if(spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Night prop became active clear target");if(p.environmentKind=="recovery-container")boxes++;else if(p.environmentKind=="source-totem")totems++;else if(p.environmentKind=="source-gate"||p.environmentKind=="gate-post")gates++;else throw new InvalidOperationException("Unknown Night source prop");}
    }
   }
   if(total!=61||active!=41||inactive!=2||boxes!=4||totems!=10||gates!=4||supports!=79)throw new InvalidOperationException("Night original activation changed");
   var groups=mechanisms.sourceLakeNight.lineGroups;if(groups==null||groups.Length!=1||groups[0].roomId!=catalog.stages[31].id||groups[0].sourceWorldId!=LakeNightRuntimeWorldsV19[2]||groups[0].surfaceIds==null||groups[0].surfaceIds.Length!=3)throw new InvalidOperationException("Night dash source-line room binding changed");for(int i=0;i<3;i++)if(groups[0].surfaceIds[i]!=catalog.stages[31].surfaces[i].id)throw new InvalidOperationException("Night dash source-line support binding changed");
   if(set.outskirtsVisual==null||set.outskirtsVisual.documentResource!="Pixel/npc-fx/besma-lake-night/visual"||set.outskirtsVisual.meshManifestResource!="Pixel/npc-fx/besma-lake-night/meshes"||set.outskirtsVisual.textures==null||set.outskirtsVisual.textures.Length!=12||set.lakeVisual==null||set.lakeVisual.documentResource!="Pixel/npc-fx/besma-lake-night/minor"||set.lakeVisual.textures==null||set.lakeVisual.textures.Length!=2)throw new InvalidOperationException("Night original source visual resources missing");
   lakeNightIterationApprovedV19=true;
  }
  Dictionary<string,Texture2D> LoadLakeNightResourceTexturesV19(OutskirtsNpcVisualResourceTexture[] rows){
   var textures=new Dictionary<string,Texture2D>();foreach(var row in rows){RequireOutskirtsVisual(row!=null&&!string.IsNullOrEmpty(row.key)&&!textures.ContainsKey(row.key)&&!string.IsNullOrEmpty(row.resource)&&row.resource.StartsWith("Pixel/npc-fx/besma-lake-night/",StringComparison.Ordinal),"Invalid Night texture binding");var texture=Resources.Load<Texture2D>(row.resource);RequireOutskirtsVisual(texture,"Missing Night source texture");textures.Add(row.key,texture);}return textures;
  }
  void ConfigurePackagedLakeNightVisualV19(NpcContentSetData set){
   RequireOutskirtsVisual(lakeNightIterationApprovedV19&&set!=null&&set.outskirtsVisual!=null&&set.lakeVisual!=null,"Night visuals lack scoped admission");var major=set.outskirtsVisual;
   var document=Resources.Load<TextAsset>(major.documentResource);var mesh=Resources.Load<TextAsset>(major.meshManifestResource);var minor=Resources.Load<TextAsset>(set.lakeVisual.documentResource);RequireOutskirtsVisual(document&&mesh&&minor,"Night visual documents missing");
   // Keep the raw JSON: shared Outskirts DTO intentionally has no stateMeshes,
   // so serializing that DTO here would erase the held-slot/Fire source data.
   ConfigureLakeNightNpcVisualV19(document.text,JsonUtility.FromJson<OutskirtsNpcMeshManifest>(mesh.text),LoadLakeNightResourceTexturesV19(major.textures),JsonUtility.FromJson<LakeMinorVisualDocumentV18>(minor.text),LoadLakeNightResourceTexturesV19(set.lakeVisual.textures));
  }
  [System.Diagnostics.Conditional("UNITY_EDITOR")]
  void ConfigureLakeNightAdmissionNoticeV19(NpcContentSetData set){
   const string label="Night Iteration Scope V19";bool active=lakeNightIterationApprovedV19&&set!=null&&set.sourceDungeon==LakeNightSourceDungeonV19&&adventureStarted&&string.IsNullOrEmpty(openMenu);
   if(active&&!labels.ContainsKey(label))labels[label]=Label(label,"BESMA LAKE NIGHT / INTERNAL PREVIEW | PIXEL ACTORS / SOURCE SCENERY | 3 MINOR + 12 AMBIENT PENDING",new Rect(0,528,960,12),8,C(0xffd985));Visible(label,active);
  }
  [System.Diagnostics.Conditional("UNITY_EDITOR")]
  void RenderLakeNightAdmissionNoticeV19(){ConfigureLakeNightAdmissionNoticeV19(CurrentNpcContentSet);}
 }
}
