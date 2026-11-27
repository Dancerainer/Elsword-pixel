using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class FactoryProductModelR1168 {public bool runtimeAdmitted;public FactoryProductRoomR1168[] rooms;}
 [Serializable] public sealed class FactoryProductRoomR1168 {public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;}
 [Serializable] public sealed class FactoryProductRulesR1168 {public bool runtimeAdmitted,sourceMechanismsComplete;public NpcDefinition[] definitions;}
 [Serializable] public sealed class FactoryProductAuditR1168 {public FactoryProductAuditRoomR1168[] rooms;}
 [Serializable] public sealed class FactoryProductAuditRoomR1168 {public FactoryProductAuditBirthR1168[] enemies;}
 [Serializable] public sealed class FactoryProductAuditBirthR1168 {public string sourceNPC;public bool sourceActive,aiEnabled;}
 [Serializable] public sealed class FactoryProductPathR1168 {public float s,length;public float[] start,end;}
 [Serializable] public sealed class FactoryProductProjectionR1168 {public string id;public float unit,offsetX,offsetY,width,height;public FactoryProductPathR1168[] path;}
 [Serializable] sealed class FactoryProductPartR1168 {public int offset,count;public string texture;}
 [Serializable] sealed class FactoryLineFields {public float SPEED;}
 [Serializable] sealed class FactorySourceLine {public int lineId;public FactoryLineFields nativeFields;}
 [Serializable] sealed class FactorySourceCollision {public FactorySourceLine[] lines;}
 [Serializable] sealed class FactoryProductSceneR1168 {public string id;public FactoryProductPartR1168[] parts;public FactorySourceCollision collision;}
 public sealed class FactoryProductWorldPayloadR1168 {public string id;public byte[] originalSceneJson,originalGeometry;public FactoryProductProjectionR1168 projection;public FactoryProductTextureR1168[] textures;internal FactorySourceLine[] sourceLines;}
 // Pre-read data only: Build creates no GameModel, Unity objects, owner, coroutine or approval.
 // The eventual owned Host commit must call the unchanged original component factory exactly once.
 public sealed class FactoryProductDataPlanR1168 {
  public FactoryProductResourceIndexR1168 index;public FactoryProductModelR1168 model;public FactoryProductRulesR1168 rules;public NpcEffectDocument effects;public NpcSoundDocument sounds;public FactoryProductWorldPayloadR1168[] worlds;
  byte[] sourceModel,fullRules,fullMechanisms,populationPolicy,eventBinding,entryActivation,sourceController,pressPolicy;
  public static FactoryProductDataPlanR1168 Build(FactoryProductResourceIndexR1168 index){
   FactoryProductResourceIndexR1168.Need(index!=null,"index absent");var m=index.Map;
   var r=new FactoryProductDataPlanR1168{index=index,sourceModel=index.RootBytes("sourceModel"),fullRules=index.RootBytes("fullRules"),fullMechanisms=index.RootBytes("fullMechanisms"),populationPolicy=index.RootBytes("populationPolicy"),eventBinding=index.RootBytes("eventBinding"),entryActivation=index.RootBytes("entryActivation"),sourceController=index.RootBytes("sourceController"),pressPolicy=index.RootBytes("pressPolicy"),effects=index.RootJson<NpcEffectDocument>("effects"),sounds=index.RootJson<NpcSoundDocument>("sounds"),worlds=new FactoryProductWorldPayloadR1168[3]};
   r.model=FactoryTypedJsonR581.ReadProductRootR1213<FactoryProductModelR1168>(r.sourceModel);var audit=FactoryTypedJsonR581.ReadProductRootR1213<FactoryProductAuditR1168>(r.sourceModel);var rules=FactoryTypedJsonR581.ReadProductRootR1213<FactoryProductRulesR1168>(r.fullRules);
   FactoryProductResourceIndexR1168.Need(r.model!=null&&!r.model.runtimeAdmitted&&r.model.rooms!=null&&r.model.rooms.Length==3&&audit!=null&&audit.rooms!=null&&audit.rooms.Length==3&&rules!=null&&!rules.runtimeAdmitted&&!rules.sourceMechanismsComplete&&rules.definitions!=null&&rules.definitions.Length==11,"original model/rules scope changed");
   string[] profiles={"NUI_BOX_ALTERA","NUI_FACTORY_PRESS","NUI_FACTORY_PRESS3","NUI_NASOD_GUARD","NUI_NASOD_HEALER_BLADER","NUI_NASOD_MAKER","NUI_NASOD_SHERIFF","NUI_RAVEN_CLONE","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP","NUI_WALLY_9TH_TYPE2"};int states=0;for(int i=0;i<profiles.Length;i++){var d=rules.definitions[i];FactoryProductResourceIndexR1168.Need(d!=null&&d.id==profiles[i]&&d.states!=null,"ordered source profile identity changed");states+=d.states.Length;}FactoryProductResourceIndexR1168.Need(states==185,"original 185-state configuration changed");
   // Retain the already validated original rules for read-only presentation
   // mappings. The component still receives the unchanged original bytes.
   r.rules=rules;
   r.effects=FactorySheriffSourceV32.LoadAndMerge(r.effects,rules.definitions);
   int[] counts={31,32,1};int births=0,active=0,inactive=0;
   for(int i=0;i<3;i++){
    var room=r.model.rooms[i];var a=audit.rooms[i];var w=m.worlds[i];var stage=m.stages[i];FactoryProductResourceIndexR1168.Need(room!=null&&room.id==FactoryProductResourceIndexR1168.Route+"-"+i&&room.sourceWorldId==w.id&&room.enemies!=null&&room.enemies.Length==counts[i]&&a!=null&&a.enemies!=null&&a.enemies.Length==counts[i]&&room.width==stage.width&&room.spawnX==stage.spawnX&&room.spawnY==stage.spawnY&&room.hasSpawnY==stage.hasSpawnY&&room.exitX==stage.exitX&&room.entryEncounter==stage.entryEncounter,"original room/stage/birth binding changed");
    for(int j=0;j<counts[i];j++){var e=room.enemies[j];var v=a.enemies[j];FactoryProductResourceIndexR1168.Need(e!=null&&v!=null&&e.sourceNPC==v.sourceNPC&&!e.aiEnabled&&!v.aiEnabled,"original AI-off staging changed");if(v.sourceActive)active++;else inactive++;births++;}
    var rawScene=index.Read(w.scene);var scene=JsonUtility.FromJson<FactoryProductSceneR1168>(new UTF8Encoding(false,true).GetString(rawScene));var projection=index.Json<FactoryProductProjectionR1168>(w.projection);var geometry=index.Read(w.binary);
    FactoryProductResourceIndexR1168.Need(scene!=null&&scene.id==w.id&&scene.parts!=null&&projection!=null&&projection.id==w.id&&projection.unit==.28f&&projection.path!=null&&projection.path.Length>0&&projection.width==room.width&&projection.height==stage.height,"original scene/projection changed");
    foreach(var p in projection.path)FactoryProductResourceIndexR1168.Need(p!=null&&p.start!=null&&p.start.Length==3&&p.end!=null&&p.end.Length==3,"source path endpoint shape changed");
    foreach(var part in scene.parts)FactoryProductResourceIndexR1168.Need(part!=null&&part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=geometry.LongLength&&(string.IsNullOrEmpty(part.texture)||Array.Exists(w.textures,t=>t.sourcePath==part.texture)),"original mesh range/texture identity changed");
    r.worlds[i]=new FactoryProductWorldPayloadR1168{id=w.id,originalSceneJson=rawScene,originalGeometry=geometry,projection=projection,textures=w.textures,sourceLines=scene.collision.lines};
   }
   FactoryProductResourceIndexR1168.Need(births==64&&active==52&&inactive==12&&r.effects!=null&&r.sounds!=null,"source role/effect/sound data incomplete");return r;
  }
  // The original constructor owns all setters/Reset/activation/NO_BRAIN/Press semantics.
  // Call only inside the eventual Host owned loading transaction; no manual Step or actor writes.
  public FactoryPreparedComponentV21 CreateOriginalComponent(){
   var component=FactoryPreparedComponentV21.CreateFullIndependentComponent(sourceModel,fullRules,fullMechanisms,populationPolicy,eventBinding,entryActivation,sourceController,pressPolicy);
   for(int i=0;i<worlds.Length;i++){
    var world=worlds[i];
    foreach(var line in world.sourceLines){
     float speed=line.nativeFields.SPEED*world.projection.unit;
     if(speed!=0)component.Model.SetFactoryConveyorSpeed(i,line.lineId,speed);
    }
   }
   return component;
  }
 }
}
