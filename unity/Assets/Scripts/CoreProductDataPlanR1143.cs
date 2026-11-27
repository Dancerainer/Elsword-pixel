using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class CoreProductRulesR1143 {public NpcDefinition[] definitions;}
 [Serializable] public sealed class CoreProductModelR1143 {public bool runtimeAdmitted;public CoreProductRoomR1143[] rooms;}
 [Serializable] public sealed class CoreProductRoomR1143 {
  public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;
  public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;public string[] bgm;
 }
 [Serializable] public sealed class CoreProductPathR1143 {public float s,length;public float[] start,end;}
 [Serializable] public sealed class CoreProductProjectionR1143 {public string id;public float unit,offsetX,offsetY,width,height;public CoreProductPathR1143[] path;}
 [Serializable] sealed class CoreProductScenePartR1143 {public int offset,count;public string texture;}
 [Serializable] sealed class CoreProductSceneIdentityR1143 {public string id;public CoreProductScenePartR1143[] parts;}
 public sealed class CoreProductWorldPayloadR1143 {public string id,bgm;public byte[] originalSceneJson,originalGeometry;public CoreProductProjectionR1143 projection;public CoreProductTextureR1143[] textures;}
 // A prepared data plan only. No GameModel, actor, camera or original source clock is mutated by Build.
 public sealed class CoreProductDataPlanR1143 {
  public CoreProductResourceIndexR1143 index;
  public NpcDefinition[] rules;public NpcEffectDocument effects;public NpcSoundDocument sounds;public NpcMechanismDocument npcMechanisms;
  public CoreHardMechanismDocumentV21 mechanisms;public CoreHardProtectionDocumentV21 protection;public CoreHardBattlePlaneDocumentV21 battlePlane;
  public CoreHardCallbackDocumentV21 callbacks;public CoreHardBalanceDocumentV21 balance;public CoreHardClosureDocumentV21 closure;
  public CoreHardLocalAddsDocumentV21 localAdds;public CoreHardEntryActivationDocumentV21 activation;public CoreStopOwnerDocumentV21 sourceStartOwner;
  public RoomDefinition[] rooms;public CoreProductWorldPayloadR1143[] worlds;
  public static CoreProductDataPlanR1143 Build(CoreProductResourceIndexR1143 index){
   CoreProductResourceIndexR1143.Need(index!=null,"resource index absent");var m=index.Map;
   var model=index.RootJson<CoreProductModelR1143>("model");var defs=index.RootJson<CoreProductRulesR1143>("rules");
   CoreProductResourceIndexR1143.Need(model!=null&&model.rooms!=null&&model.rooms.Length==5&&defs!=null&&defs.definitions!=null&&defs.definitions.Length==23,"original five-room/23-profile model absent");
   var ids=new HashSet<string>(StringComparer.Ordinal);int states=0;foreach(var r in defs.definitions){CoreProductResourceIndexR1143.Need(r!=null&&ids.Add(r.id)&&r.states!=null,"duplicate/malformed profile");states+=r.states.Length;}
   CoreProductResourceIndexR1143.Need(states==372,"original state inventory changed");foreach(var a in m.actors)CoreProductResourceIndexR1143.Need(a!=null&&Array.Exists(defs.definitions,p=>p.id==a.npc&&p.kind==a.kind)&&(!a.emptySourceBody||a.npc=="NUI_PD_MONSTER"),"source actor identity changed");
   var result=new CoreProductDataPlanR1143{index=index,rules=defs.definitions,
    effects=index.RootJson<NpcEffectDocument>("effects"),sounds=index.RootJson<NpcSoundDocument>("sounds"),npcMechanisms=index.RootJson<NpcMechanismDocument>("npcMechanisms"),
    mechanisms=index.RootJson<CoreHardMechanismDocumentV21>("mechanisms"),protection=index.RootJson<CoreHardProtectionDocumentV21>("protection"),battlePlane=index.RootJson<CoreHardBattlePlaneDocumentV21>("battlePlane"),
    callbacks=index.RootJson<CoreHardCallbackDocumentV21>("callbacks"),balance=index.RootJson<CoreHardBalanceDocumentV21>("balance"),closure=index.RootJson<CoreHardClosureDocumentV21>("closure"),
    localAdds=index.RootJson<CoreHardLocalAddsDocumentV21>("localAdds"),activation=index.RootJson<CoreHardEntryActivationDocumentV21>("activation"),sourceStartOwner=index.RootJson<CoreStopOwnerDocumentV21>("sourceStartOwner"),rooms=new RoomDefinition[5],worlds=new CoreProductWorldPayloadR1143[5]};
   CoreProductResourceIndexR1143.Need(result.balance!=null&&result.balance.difficultyValue==1&&result.balance.npcLevel==37&&result.balance.births!=null&&result.balance.births.Length==85&&result.activation!=null&&result.activation.difficultyValue==1&&result.activation.npcLevel==37&&result.activation.births!=null&&result.activation.births.Length==85,"Hard1/level37 original initialization data changed");
   int births=0,nodes=0;
   for(int i=0;i<5;i++){
    var source=model.rooms[i];var w=m.worlds[i];CoreProductResourceIndexR1143.Need(source!=null&&source.id==m.sourceDungeon+"-"+i&&source.sourceWorldId==w.id&&source.enemies!=null&&source.encounters!=null,"source room identity changed");
    var sceneBytes=index.Read(w.scene);var scene=JsonUtility.FromJson<CoreProductSceneIdentityR1143>(Encoding.UTF8.GetString(sceneBytes));var p=index.Json<CoreProductProjectionR1143>(w.projection);var geometry=index.Read(w.binary);
    CoreProductResourceIndexR1143.Need(scene!=null&&scene.id==w.id&&scene.parts!=null&&p!=null&&p.id==w.id&&p.unit==.28f&&p.path!=null&&p.path.Length>0,"original scene/projection binding changed");
    foreach(var part in scene.parts)CoreProductResourceIndexR1143.Need(part!=null&&part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=geometry.Length&&(string.IsNullOrEmpty(part.texture)||Array.Exists(w.textures,t=>t.sourcePath==part.texture)),"scene mesh/texture range changed");
    foreach(var t in w.textures)CoreProductResourceIndexR1143.Need(t!=null&&t.source!=null&&t.selected!=null&&t.source.path==t.selected.path&&t.source.sha256==t.selected.sha256&&!t.singleMaterialTrial,"scene texture substitution forbidden");
    var room=new RoomDefinition{id=source.id,title="Altera Core / Hard / Area "+(i+1),width=source.width,spawnX=source.spawnX,spawnY=source.spawnY,hasSpawnY=source.hasSpawnY,exitX=source.exitX,entryEncounter=source.entryEncounter,encounters=source.encounters,groundNodes=source.groundNodes,surfaces=source.surfaces};
    foreach(var e in source.enemies)room.enemies.Add(e);births+=source.enemies.Length;nodes+=source.encounters.Length;result.rooms[i]=room;
    result.worlds[i]=new CoreProductWorldPayloadR1143{id=w.id,bgm=m.music[i].id,originalSceneJson=sceneBytes,originalGeometry=geometry,projection=p,textures=w.textures};
   }
   CoreProductResourceIndexR1143.Need(births==85&&nodes==8,"original 85-birth/eight-node route changed");
   return result;
  }
 }
}
