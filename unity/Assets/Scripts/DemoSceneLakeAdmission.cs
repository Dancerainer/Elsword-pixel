using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class LakeNpcVisualResourcesV18 {public string documentResource;public OutskirtsNpcVisualResourceTexture[] textures;}
 public sealed partial class DemoScene {
  const string LakeSourceDungeonV18="Dungeon_Besma_Lake_Normal";
  bool lakeIterationApprovedV18;
  public bool TryBeginAdmittedLakeV18(){
   int index=Array.FindIndex(catalog.dungeons,d=>d.sourceGraph==LakeSourceDungeonV18);if(index<0)return false;
   if(mapDungeons.Count==0)InitializeDungeonMap();SelectRuntimeDungeonOnMap(index);EnterSelectedDungeon();return true;
  }
  static readonly string[] LakeRuntimeNpcsV18={"NUI_BESMA_TOTEM_BASE","NUI_BOX_BESMA1","NUI_KOMODO","NUI_LIZARDMAN_OLDER","NUI_LIZARDMAN_SAURUS","NUI_LIZARDMAN_SHAMAN","NUI_LIZARDMAN_SPEAR","NUI_SCORPION","NUI_SMALLGATE_BESMA_LAKE","NUI_SMALLGATE_POST_BESMA_LAKE"};
  static readonly string[] LakeRuntimeEffectsV18={"BESMA_TOTEM_BASE_FIRE_ATTACK","DamageEffect_KOMODO_ATTACK_A","DamageEffect_LIZARDMAN_OLDER_ATTACK_A","DamageEffect_LIZARDMAN_OLDER_ATTACK_B","DamageEffect_LIZARDMAN_OLDER_SPECIAL_ATTACK_A","DamageEffect_LIZARDMAN_SAURUS_ATTACK_A","DamageEffect_LIZARDMAN_SAURUS_ATTACK_A2","DamageEffect_LIZARDMAN_SHAMAN_ATTACK_A"};
  static readonly string[] LakeRuntimeWorldsV18={"WI_NEW_BESMA_LAKE_STAGE1","WI_NEW_BESMA_LAKE_STAGE2A","WI_BESMA_LAKE_STAGE4"};
  void ValidateLakeIterationV18(NpcContentSetData set){
   var a=set.admission;
   if(a==null||a.schema!="besma-lake-normal-85-v18"||a.sourceDungeon!=LakeSourceDungeonV18||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,LakeRuntimeNpcsV18)||!ExactIds(a.effectIds,LakeRuntimeEffectsV18)||!ExactIds(a.sourceWorldIds,LakeRuntimeWorldsV18))throw new InvalidOperationException("Unreviewed Besma Lake scope");
   if(catalog.dungeons.Length<10||catalog.stages.Length<29)throw new InvalidOperationException("Besma Lake route incomplete");var route=catalog.dungeons[9];
   if(route.id!="besma-lake"||route.sourceGraph!=LakeSourceDungeonV18||route.region!="bethma"||route.start!=26||route.count!=3||route.combatZones!=5||!route.iterationPreview)throw new InvalidOperationException("Besma Lake source route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,p=>p.id),LakeRuntimeNpcsV18)||set.npcEffectRules==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,p=>p.id),LakeRuntimeEffectsV18)||set.npcSoundRules==null||set.npcSoundRules.assets.Length!=23||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null||set.npcMechanismRules.lakeVisual==null||set.npcMechanismRules.lakeVisual.schema!="besma-lake-visual-runtime-draft-v18")throw new InvalidOperationException("Besma Lake source documents missing");
   foreach(var p in set.npcRules){var art=Array.Find(catalog.monsters,m=>m.id==p.kind);if(art==null)throw new InvalidOperationException("Missing Besma Lake pixels "+p.id);foreach(var s in p.states)if(!Array.Exists(art.clips,c=>c.id==s.clip))throw new InvalidOperationException("Missing Besma Lake state "+p.id+"/"+s.clip);}
   foreach(var b in set.npcEffectRules.bindings)if(Array.IndexOf(LakeRuntimeNpcsV18,b.npc)<0)throw new InvalidOperationException("Unexpected Lake effect actor");
   foreach(var t in set.npcSoundRules.stateTracks)if(Array.IndexOf(LakeRuntimeNpcsV18,t.npc)<0)throw new InvalidOperationException("Unexpected Lake sound actor");
   foreach(var t in set.npcSoundRules.effectTracks)if(Array.IndexOf(LakeRuntimeEffectsV18,t.effect)<0)throw new InvalidOperationException("Unexpected Lake effect sound");
   foreach(var t in set.npcMechanismRules.states)if(Array.IndexOf(LakeRuntimeNpcsV18,t.npc)<0)throw new InvalidOperationException("Unexpected Lake mechanism actor");
   int total=0,active=0,inactive=0,boxes=0,totems=0,gates=0,supports=0;
   for(int i=0;i<3;i++){
    var stage=catalog.stages[26+i];if(stage.sourceWorldId!=LakeRuntimeWorldsV18[i]||stage.spawns.Length!=new[]{25,31,3}[i]||stage.encounters.Length!=new[]{2,2,1}[i]||stage.surfaces.Length!=new[]{31,44,3}[i]||stage.bgm!=(i==2?"music009_boss":"music019_stage"))throw new InvalidOperationException("Lake original world, BGM or roster changed");supports+=stage.surfaces.Length;
    for(int j=0;j<stage.spawns.Length;j++){var spawn=stage.spawns[j];total++;var p=Array.Find(set.npcRules,n=>n.id==spawn.sourceNPC);if(p==null||p.kind!=spawn.kind||spawn.visualScale!=1)throw new InvalidOperationException("Lake source identity or scale changed");
     if(string.IsNullOrEmpty(p.environmentKind)){if(spawn.aiEnabled){if(!spawn.countsForClear||spawn.dormant||spawn.passive)throw new InvalidOperationException("Lake combat actor disabled");active++;}else{if(i!=2||j>=2||spawn.sourceNPC!="NUI_LIZARDMAN_SHAMAN"||spawn.countsForClear)throw new InvalidOperationException("Lake original inactive Shaman changed");inactive++;}}
     else{if(spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Lake prop became active clear target");if(p.environmentKind=="recovery-container")boxes++;else if(p.environmentKind=="source-totem")totems++;else if(p.environmentKind=="source-gate"||p.environmentKind=="gate-post")gates++;else throw new InvalidOperationException("Unknown Lake prop");}
    }
   }
   if(total!=59||active!=41||inactive!=2||boxes!=4||totems!=8||gates!=4||supports!=78)throw new InvalidOperationException("Lake original activation changed");
   if(set.outskirtsVisual==null||set.outskirtsVisual.documentResource!="Pixel/npc-fx/besma-lake/visual"||set.outskirtsVisual.meshManifestResource!="Pixel/npc-fx/besma-lake/meshes"||set.outskirtsVisual.textures==null||set.outskirtsVisual.textures.Length!=13||set.lakeVisual==null||set.lakeVisual.documentResource!="Pixel/npc-fx/besma-lake/minor"||set.lakeVisual.textures==null||set.lakeVisual.textures.Length!=2)throw new InvalidOperationException("Lake original visible effects missing");
   lakeIterationApprovedV18=true;
  }
  Dictionary<string,Texture2D> LoadLakeResourceTexturesV18(OutskirtsNpcVisualResourceTexture[] rows){
   var textures=new Dictionary<string,Texture2D>();foreach(var row in rows){RequireOutskirtsVisual(row!=null&&!string.IsNullOrEmpty(row.key)&&!textures.ContainsKey(row.key)&&!string.IsNullOrEmpty(row.resource)&&row.resource.StartsWith("Pixel/npc-fx/besma-lake/",StringComparison.Ordinal),"Invalid Lake texture binding");var texture=Resources.Load<Texture2D>(row.resource);RequireOutskirtsVisual(texture,"Missing Lake source texture");textures.Add(row.key,texture);}return textures;
  }
  void ConfigurePackagedLakeVisualV18(NpcContentSetData set){
   RequireOutskirtsVisual(lakeIterationApprovedV18&&set!=null&&set.outskirtsVisual!=null&&set.lakeVisual!=null,"Lake visuals lack scoped admission");var major=set.outskirtsVisual;
   var document=Resources.Load<TextAsset>(major.documentResource);var mesh=Resources.Load<TextAsset>(major.meshManifestResource);var minor=Resources.Load<TextAsset>(set.lakeVisual.documentResource);RequireOutskirtsVisual(document&&mesh&&minor,"Lake visual documents missing");
   ConfigureOutskirtsNpcVisual(JsonUtility.FromJson<OutskirtsNpcVisualDocument>(document.text),JsonUtility.FromJson<OutskirtsNpcMeshManifest>(mesh.text),LoadLakeResourceTexturesV18(major.textures));
   ConfigureLakeNpcVisual(JsonUtility.FromJson<LakeMinorVisualDocumentV18>(minor.text),LoadLakeResourceTexturesV18(set.lakeVisual.textures));
  }
 }
}
