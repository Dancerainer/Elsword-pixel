using System;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string BelowPassageSourceDungeon="Dungeon_Elder_Below_Passage_Normal";
  static readonly string[] BelowPassageRuntimeNpcs={"NUI_GHOST_MAGICIAN_RAGGED","NUI_MOUSE","NUI_MOUSE_NODROP","NUI_SOLDIER_THIN","NUI_SOLDIER_FAT","NUI_BENDERS_NASOD","NUI_MOUSEHOUSE","NUI_STEAM_TRAP","NUI_LATTICE_GATE_BELOW_PASSAGE"};
  static readonly string[] BelowPassageRuntimeEffects={"BENDERSNASOD_LASER_ATTACK_INVISIBLE","BENDERS_NASWORD_SPECIAL_ATTACK_A","DamageEffect_BENDERS_NASOD_ATTACK_A","DamageEffect_BENDERS_NASOD_ATTACK_A_READY","DamageEffect_BENDERS_NASOD_ATTACK_B","DamageEffect_BENDERS_NASOD_ATTACK_B2","DamageEffect_Forest_Dark_Magician_DarkBall_S","DamageEffect_Forest_Dark_Magician_DarkBall_EXPLOSION_S","DamageEffect_SOLDIER_THIN_ATTACK_A","DamageEffect_THIEF_FAT_ATTACK_A"};
  static readonly string[] BelowPassageRuntimeWorlds={"WI_ELDER_BELOW_PASSAGE_STAGE2_1A","WI_ELDER_BELOW_PASSAGE_STAGE3_1A","WI_ELDER_BELOW_PASSAGE_STAGE4"};
  void ValidateBelowPassageIteration(NpcContentSetData set){
   var a=set==null?null:set.admission;
   if(set==null||set.sourceDungeon!=BelowPassageSourceDungeon||a==null||a.schema!="below-passage-normal-85-v1"||a.sourceDungeon!=BelowPassageSourceDungeon||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,BelowPassageRuntimeNpcs)||!ExactIds(a.effectIds,BelowPassageRuntimeEffects)||!ExactIds(a.sourceWorldIds,BelowPassageRuntimeWorlds))throw new InvalidOperationException("Unreviewed waterway iteration scope");
   var route=catalog.dungeons[5];
   if(route.id!="underground-waterway"||route.sourceGraph!=BelowPassageSourceDungeon||route.start!=15||route.count!=3||route.combatZones!=4||!route.iterationPreview)throw new InvalidOperationException("Waterway source route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,n=>n.id),BelowPassageRuntimeNpcs)||set.npcEffectRules==null||set.npcEffectRules.definitions==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,e=>e.id),BelowPassageRuntimeEffects)||set.npcSoundRules==null||set.npcSoundRules.assets==null||set.npcSoundRules.assets.Length!=10||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null)throw new InvalidOperationException("Waterway source documents incomplete");
   foreach(var npc in set.npcRules){var art=Array.Find(catalog.monsters,m=>m.id==npc.kind);if(art==null)throw new InvalidOperationException("Missing waterway pixels: "+npc.id);foreach(var state in npc.states)if(!Array.Exists(art.clips,c=>c.id==state.clip))throw new InvalidOperationException("Missing waterway state: "+npc.id+"/"+state.clip);}
   foreach(var fx in set.npcEffectRules.definitions)if(!string.IsNullOrEmpty(fx.visualId)&&!Array.Exists(catalog.effects,e=>e.id==fx.visualId&&Array.Exists(e.clips,c=>c.id=="play")))throw new InvalidOperationException("Missing waterway effect pixels: "+fx.id);
   foreach(var binding in set.npcEffectRules.bindings)if(Array.IndexOf(BelowPassageRuntimeNpcs,binding.npc)<0)throw new InvalidOperationException("Unexpected waterway effect actor");
   foreach(var track in set.npcSoundRules.stateTracks)if(Array.IndexOf(BelowPassageRuntimeNpcs,track.npc)<0)throw new InvalidOperationException("Unexpected waterway sound actor");
   foreach(var track in set.npcSoundRules.effectTracks)if(Array.IndexOf(BelowPassageRuntimeEffects,track.effect)<0)throw new InvalidOperationException("Unexpected waterway effect sound");
   foreach(var state in set.npcMechanismRules.states)if(Array.IndexOf(BelowPassageRuntimeNpcs,state.npc)<0)throw new InvalidOperationException("Unexpected waterway mechanism actor");
   if(set.steamVisual==null||set.steamVisual.textureResource!="Pixel/npc-fx/waterway/Steam_BP"||set.steamVisual.emitters==null||set.steamVisual.emitters.Length!=2||Math.Abs(set.steamVisual.unit-.28f)>.000001f)throw new InvalidOperationException("Missing original waterway steam visual");
   int total=0,active=0,inactive=0,houses=0,steam=0,gates=0,boxes=0,movie=0;
   for(int i=0;i<3;i++){
    var stage=catalog.stages[15+i];if(stage.sourceWorldId!=BelowPassageRuntimeWorlds[i])throw new InvalidOperationException("Waterway source world order changed");
    foreach(var spawn in stage.spawns){
     total++;if(spawn.sourceNPC=="NUI_BOX"){if(spawn.kind!="prop-box"||spawn.aiEnabled)throw new InvalidOperationException("Wrong waterway box reuse");boxes++;continue;}
     if(spawn.sourceNPC=="NUI_EVENT_NPC_BENDERS_ELDER"){if(!spawn.dormant||!spawn.passive||spawn.aiEnabled||spawn.countsForClear||!Array.Exists(catalog.monsters,m=>m.id==spawn.kind))throw new InvalidOperationException("Waterway director identity changed");movie++;continue;}
     var profile=Array.Find(set.npcRules,n=>n.id==spawn.sourceNPC);if(profile==null||profile.kind!=spawn.kind||spawn.visualScale!=1)throw new InvalidOperationException("Unexpected waterway spawn or duplicated scale");
     if(profile.environmentKind=="mouse-house"){if(spawn.aiEnabled||!spawn.countsForClear)throw new InvalidOperationException("Mouse house source semantics changed");houses++;}
     else if(profile.environmentKind=="timed-steam"){if(spawn.aiEnabled||spawn.countsForClear)throw new InvalidOperationException("Steam became a combat body");steam++;}
     else if(profile.environmentKind=="source-gate"){if(spawn.aiEnabled||!spawn.isGate)throw new InvalidOperationException("Waterway gate changed");gates++;}
     else if(!string.IsNullOrEmpty(profile.environmentKind))throw new InvalidOperationException("Unexpected waterway environment");
     else if(spawn.aiEnabled){if(!spawn.countsForClear)throw new InvalidOperationException("Inactive waterway actor became active");active++;}else inactive++;
    }
   }
   if(total!=44||active!=32||inactive!=2||houses!=2||steam!=1||gates!=2||boxes!=4||movie!=1)throw new InvalidOperationException("Waterway original roster or ACTIVE semantics changed");
   belowPassageIterationApproved=true;
  }
 }
}
