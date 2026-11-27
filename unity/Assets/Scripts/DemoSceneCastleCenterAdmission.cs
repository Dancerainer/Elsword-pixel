using System;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool castleCenterIterationApproved;
  int postOutskirtsRouteCount,postOutskirtsStageCount;
  const string CastleCenterSource="Dungeon_Elder_Wally_Castle_Center_Normal";
  static readonly string[] CastleCenterNpcs={"NUI_SOLDIER_CROSSBOW","NUI_SOLDIER_BENDERS","NUI_WALLY_8TH","NUI_BUNKER","NUI_SOLDIER_THIN","NUI_SOLDIER_FAT","NUI_WOOD_POST_TRAP","NUI_SMALLGATE_WALLY_CASTLE_A","NUI_SMALLGATE_POST_WALLY_CASTLE_A"};
  static readonly string[] CastleCenterEffects={"DamageEffect_SOLDIER_BENDERS_ATTACK_A","DamageEffect_SOLDIER_CROSSBOW_SIEGE_ATTACK_A","DamageEffect_SOLDIER_THIN_ATTACK_A","DamageEffect_THIEF_FAT_ATTACK_A","DamageEffect_WALLY_8TH_ATTACK_A","DamageEffect_WALLY_8TH_ATTACK_SPECIAL_ATTACK","DamageEffect_WALLY_8TH_ATTACK_SPECIAL_ATTACK2","DamageEffect_WALLY_8TH_GUIDED_MISSILE","DamageEffect_WALLY_8TH_GUIDED_MISSILE_EXPLOSION","DamageEffect_solder_bender_attack_A"};
  static readonly string[] CastleCenterAdmittedWorlds={"WI_ELDER_WALLY_CASTLE_CENTER_STAGE3","WI_ELDER_WALLY_CASTLE_CENTER_STAGE4"};
  public bool TryBeginAdmittedCastleCenter(){
   int index=Array.FindIndex(catalog.dungeons,d=>d.sourceGraph==CastleCenterSource);if(index<0)return false;
   if(mapDungeons.Count==0)InitializeDungeonMap();SelectRuntimeDungeonOnMap(index);EnterSelectedDungeon();return true;
  }
  void SelectCastleCenterPathObstruction(ref Enemy target,ref float best){
   if(target==null||Dungeon.sourceGraph!=CastleCenterSource)return;
   float direction=Math.Sign(target.x-game.player.x);
   foreach(var e in game.enemies){
    if(!e.active||e.dormant||e.passive||e.isGate||e.hp<=0||e.countsForClear||e.kind.StartsWith("prop-",StringComparison.Ordinal))continue;
    float dx=e.x-game.player.x,dy=e.y-game.player.y,distance=(float)Math.Sqrt(dx*dx+dy*dy);
    if(dx*direction>=0&&Math.Abs(dx)<80&&Math.Abs(dy)<45&&distance<best){target=e;best=distance;}
   }
  }
  void ValidatePostOutskirtsAdmissions(){
   for(int i=3;i<catalog.npcContentSets.Length;i++){
    var set=catalog.npcContentSets[i];
    if(i==3&&set!=null&&set.sourceDungeon==CastleCenterSource){ValidateCastleCenterIteration(set);postOutskirtsRouteCount++;postOutskirtsStageCount+=2;}
    else if(i==4&&set!=null&&set.sourceDungeon==DragonRoadSource){ValidateDragonRoadIteration(set);postOutskirtsRouteCount++;postOutskirtsStageCount+=3;}
    else if(i==5&&set!=null&&set.sourceDungeon==LakeSourceDungeonV18){ValidateLakeIterationV18(set);postOutskirtsRouteCount++;postOutskirtsStageCount+=3;}
    else if(i==6&&set!=null&&set.sourceDungeon==LakeNightSourceDungeonV19){ValidateLakeNightIterationV19(set);postOutskirtsRouteCount++;postOutskirtsStageCount+=3;}
    else if(i==7&&set!=null&&set.sourceDungeon==DragonNestSourceDungeonV20){ValidateDragonNestIterationV20(set,set.dragonNestVisual);postOutskirtsRouteCount++;postOutskirtsStageCount+=3;}
    else throw new InvalidOperationException("Additional source dungeon has no scoped admission validator");
   }
   if(catalog.dungeons.Length!=4+catalog.npcContentSets.Length||catalog.stages.Length!=21+postOutskirtsStageCount)throw new InvalidOperationException("Additional source routes contain missing or orphan stages");
  }
  void ValidateCastleCenterIteration(NpcContentSetData set){
   var a=set.admission;
   if(a==null||a.schema!="castle-center-normal-85-v17"||a.sourceDungeon!=CastleCenterSource||!a.reviewed||!EvidenceHash(a.bundleSha256)||!EvidenceHash(a.reviewSha256)||!ExactIds(a.npcIds,CastleCenterNpcs)||!ExactIds(a.effectIds,CastleCenterEffects)||!ExactIds(a.sourceWorldIds,CastleCenterAdmittedWorlds))throw new InvalidOperationException("Unreviewed Castle Center content scope");
   var route=catalog.dungeons[7];
   if(route.id!="wally-castle-center"||route.sourceGraph!=CastleCenterSource||route.start!=21||route.count!=2||route.combatZones!=4||!route.iterationPreview)throw new InvalidOperationException("Castle Center source route changed");
   if(set.npcRules==null||!ExactIds(Array.ConvertAll(set.npcRules,n=>n.id),CastleCenterNpcs)||set.npcEffectRules==null||!ExactIds(Array.ConvertAll(set.npcEffectRules.definitions,e=>e.id),CastleCenterEffects)||set.npcSoundRules==null||set.npcSoundRules.assets.Length!=27||!PackagedNpcSoundsCover(set.npcSoundRules)||set.npcMechanismRules==null)throw new InvalidOperationException("Castle Center source documents incomplete");
   foreach(var p in set.npcRules){var art=Array.Find(catalog.monsters,m=>m.id==p.kind);if(art==null)throw new InvalidOperationException("Missing Castle Center pixels "+p.id);foreach(var s in p.states)if(!Array.Exists(art.clips,c=>c.id==s.clip))throw new InvalidOperationException("Missing Castle Center state "+p.id+"/"+s.clip);}
   foreach(var b in set.npcEffectRules.bindings)if(Array.IndexOf(CastleCenterNpcs,b.npc)<0)throw new InvalidOperationException("Unreviewed Castle Center effect actor");
   foreach(var t in set.npcSoundRules.stateTracks)if(Array.IndexOf(CastleCenterNpcs,t.npc)<0)throw new InvalidOperationException("Unreviewed Castle Center sound actor");
   foreach(var t in set.npcSoundRules.effectTracks)if(Array.IndexOf(CastleCenterEffects,t.effect)<0)throw new InvalidOperationException("Unreviewed Castle Center effect sound");
   foreach(var t in set.npcMechanismRules.states)if(Array.IndexOf(CastleCenterNpcs,t.npc)<0)throw new InvalidOperationException("Unreviewed Castle Center mechanism");
   int total=0,active=0,inactive=0,props=0;
   for(int i=0;i<2;i++){
    var stage=catalog.stages[21+i];if(stage.sourceWorldId!=CastleCenterAdmittedWorlds[i]||stage.spawns.Length!=new[]{37,3}[i]||stage.encounters.Length!=new[]{3,1}[i])throw new InvalidOperationException("Castle Center source world/zone/roster changed");
    foreach(var s in stage.spawns){total++;if(s.visualScale!=1)throw new InvalidOperationException("Castle Center source scale applied twice");
     if(s.sourceNPC=="NUI_BOX"){if(s.kind!="prop-box"||s.aiEnabled||s.countsForClear)throw new InvalidOperationException("Wrong Castle Center box reuse");props++;continue;}
     var p=Array.Find(set.npcRules,n=>n.id==s.sourceNPC);if(p==null||p.kind!=s.kind)throw new InvalidOperationException("Unbound Castle Center spawn");
     if(p.environmentKind=="source-gate"||p.environmentKind=="proximity-trap"){if(s.aiEnabled||s.countsForClear)throw new InvalidOperationException("Castle Center prop became a clear target");props++;}
     else if(s.aiEnabled){if(!s.countsForClear)throw new InvalidOperationException("Castle Center inactive guard activated");active++;}else {if(s.countsForClear)throw new InvalidOperationException("Castle Center inactive guard became a clear target");inactive++;}
    }
   }
   if(total!=40||active!=30||inactive!=2||props!=8)throw new InvalidOperationException("Castle Center source activation changed");
   if(set.outskirtsVisual==null||set.outskirtsVisual.textures==null||set.outskirtsVisual.textures.Length!=11||set.outskirtsVisual.documentResource!="Pixel/npc-fx/castle-center/visual")throw new InvalidOperationException("Castle Center visible source effects missing");
   castleCenterIterationApproved=true;
  }
 }
}
