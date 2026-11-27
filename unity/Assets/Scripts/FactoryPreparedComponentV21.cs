using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
 // Prepared component only. This does not replace DemoScene.game, activate
 // combat births, install resources, or make a route available in a menu.
 // The DTO projection follows DemoSceneForestPreview.PreviewRoom verbatim.
 public sealed class FactoryPreparedComponentV21 : IDisposable {
  [Serializable] sealed class ModelDocument {public bool runtimeAdmitted;public SourceRoom[] rooms;}
  [Serializable] sealed class SourceRoom {
   public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;
   public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;
  }
  [Serializable] sealed class RulesDocument {public bool runtimeAdmitted,sourceMechanismsComplete;public NpcDefinition[] definitions;}
  // sourceActive is provenance, not EnemySpawn.active or permission to enable AI.
  [Serializable] sealed class ActivationDocument {public ActivationRoom[] rooms;}
  [Serializable] sealed class ActivationRoom {public ActivationBirth[] enemies;}
  [Serializable] sealed class ActivationBirth {public string sourceNPC;public bool sourceActive,aiEnabled;}
  readonly GameModel model;int soundRequestCursor;bool disposed;
  const string Route="Dungeon_Altera_Nasod_Factory_Normal";
  FactoryPreparedComponentV21(GameModel ownModel){model=ownModel;}
  public GameModel Model {get{RequireLive();return model;}}
  public int LastAudioRequestSerial {get{return soundRequestCursor;}}
  public bool IsDisposed {get{return disposed;}}
  static void Require(bool condition,string text){if(!condition)throw new InvalidOperationException("Factory prepared component: "+text);}
  void RequireLive(){if(disposed)throw new ObjectDisposedException("FactoryPreparedComponentV21");}
  static T ExactJson<T>(byte[] raw,int bytes,string sha) {
   Require(raw!=null&&raw.Length==bytes,"changed fixed JSON byte length");
   using(var hash=SHA256.Create())Require(BitConverter.ToString(hash.ComputeHash(raw)).Replace("-","").ToLowerInvariant()==sha,"changed fixed JSON SHA256");
   var result=FactoryTypedJsonR581.Read<T>(raw);Require(!ReferenceEquals(result,null),"null typed JSON");return result;
  }
  public static FactoryPreparedComponentV21 CreateIndependentComponent(byte[] sourceModel,byte[] minimalRules,byte[] minimalMechanisms,byte[] populationPolicy,byte[] eventBinding) {
   var source=ExactJson<ModelDocument>(sourceModel,184519,"3c7ecf2fb085ea6cff567ab8fe3853ac951d2ac40acf5f51a8018e7a6ca81890");
   var audit=FactoryTypedJsonR581.Read<ActivationDocument>(sourceModel);
   var rules=ExactJson<RulesDocument>(minimalRules,1503399,"9f9ab2061dd68a9e99c580793d5edba6bbd30de0e7745c7d1316d9e2b417b933");
   var mechanisms=ExactJson<NpcMechanismDocument>(minimalMechanisms,12773,"32cc32a60aa804211c5b32dd6cb2ffaba9c52acc79ebfe3c74c9530c32eec543");
   var population=ExactJson<FactoryPopulationPolicyV21>(populationPolicy,19337,"27425208f68fb181195918a365ca7481f0021e93f80be7bbc724863649da2e9f");
   var binding=ExactJson<FactoryEventStartBindingV21>(eventBinding,1066,"10818476cd32c8264d5bd85ab2c686792dfbe36a72845ab21edda60195d7913c");
   Require(!source.runtimeAdmitted&&source.rooms!=null&&source.rooms.Length==3&&audit!=null&&audit.rooms!=null&&audit.rooms.Length==3,"source route scope");
   Require(!rules.runtimeAdmitted&&!rules.sourceMechanismsComplete&&rules.definitions!=null&&rules.definitions.Length==2&&rules.definitions[0].id=="NUI_NASOD_MAKER"&&rules.definitions[1].id=="NUI_WALLY_9TH_TYPE2","minimal dictionary is component-only");
   var rooms=new List<RoomDefinition>();int[] births={31,32,1};string[] worlds={"WI_ALTERA_NASOD_FACTORY_STAGE1","WI_ALTERA_NASOD_FACTORY_STAGE2","WI_ALTERA_NASOD_FACTORY_BOSS"};int inactiveDeclarations=0,total=0;
   for(int i=0;i<3;i++) {
    var r=source.rooms[i];var a=audit.rooms[i];
    Require(r!=null&&r.id==Route+"-"+i&&r.sourceWorldId==worlds[i]&&r.enemies!=null&&r.enemies.Length==births[i]&&a!=null&&a.enemies!=null&&a.enemies.Length==births[i],"room/birth identity");
    var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,groundNodes=r.groundNodes,surfaces=r.surfaces,encounters=r.encounters};
    for(int j=0;j<r.enemies.Length;j++) {
     var e=r.enemies[j];var declaration=a.enemies[j];Require(e!=null&&declaration!=null&&e.sourceNPC==declaration.sourceNPC&&!e.aiEnabled&&!declaration.aiEnabled,"original 64 AI staging flags");
     if(!declaration.sourceActive)inactiveDeclarations++;room.enemies.Add(e);total++;
    }
    rooms.Add(room);
   }
   Require(total==64&&inactiveDeclarations==12,"source activation audit");
   // Always a new model. No caller can use this method to clear an existing
   // Core/Below/Mine2 dictionary. All original setters retain their guards.
   var model=new GameModel();model.SetNpcDefinitions(rules.definitions);model.SetNpcMechanismDefinitions(mechanisms);
   model.SetFactoryPopulationPolicyV21(population);model.SetFactoryEventStartBindingV21(binding);model.SetRooms(rooms);
   return new FactoryPreparedComponentV21(model);
  }
  // R631 is a distinct full component entry. The original minimal five-raw
  // method above retains its exact two-profile contract and historical use.
  // These eight fixed raw documents produce nine explicit typed roots; the
  // original source model is read both as model data and activation provenance.
  public static FactoryPreparedComponentV21 CreateFullIndependentComponent(byte[] sourceModel,byte[] fullRules,byte[] fullMechanisms,byte[] populationPolicy,byte[] eventBinding,byte[] entryActivation,byte[] sourceController,byte[] pressPolicy) {
   var source=ExactJson<ModelDocument>(sourceModel,184519,"3c7ecf2fb085ea6cff567ab8fe3853ac951d2ac40acf5f51a8018e7a6ca81890");
   var audit=FactoryTypedJsonR581.Read<ActivationDocument>(sourceModel);
   var rules=ExactJson<RulesDocument>(fullRules,7215651,"087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159");
   var mechanisms=ExactJson<NpcMechanismDocument>(fullMechanisms,62276,"fdbed410573f41f7d895bf2687546125c50f4ab8e079cd6ed94428489d1d8d0e");
   var population=ExactJson<FactoryPopulationPolicyV21>(populationPolicy,19337,"27425208f68fb181195918a365ca7481f0021e93f80be7bbc724863649da2e9f");
   var binding=ExactJson<FactoryEventStartBindingV21>(eventBinding,1066,"10818476cd32c8264d5bd85ab2c686792dfbe36a72845ab21edda60195d7913c");
   var activation=ExactJson<FactoryEntryActivationDocumentV542>(entryActivation,25891,"2ba7923f6b9fbe57f1e6589fa3181bc76489ae2c77f7408dd8b6d67c37fe4924");
   var controller=ExactJson<FactorySourceControllerDocumentV554>(sourceController,10149,"b1917a6554dc8619570011a2493e6637889dd86a32f721a21518b30b08bc5392");
   var press=ExactJson<FactoryPressDocumentV569>(pressPolicy,3123,"126137a5bcc236db903e9c814f2023676f671cecbf03fa5950d9d454cb675021");
   Require(!source.runtimeAdmitted&&source.rooms!=null&&source.rooms.Length==3&&audit!=null&&audit.rooms!=null&&audit.rooms.Length==3,"full source route scope");
   Require(!rules.runtimeAdmitted&&!rules.sourceMechanismsComplete&&rules.definitions!=null&&rules.definitions.Length==11,"full eleven-profile source dictionary");
   string[] profileIds={"NUI_BOX_ALTERA","NUI_FACTORY_PRESS","NUI_FACTORY_PRESS3","NUI_NASOD_GUARD","NUI_NASOD_HEALER_BLADER","NUI_NASOD_MAKER","NUI_NASOD_SHERIFF","NUI_RAVEN_CLONE","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP","NUI_WALLY_9TH_TYPE2"};
   int stateCount=0;for(int i=0;i<profileIds.Length;i++){var profile=rules.definitions[i];Require(profile!=null&&profile.id==profileIds[i]&&profile.states!=null,"full exact ordered profile identity");stateCount+=profile.states.Length;}
   Require(stateCount==185&&!mechanisms.runtimeAdmitted&&!mechanisms.sourceMechanismsComplete&&mechanisms.sourceRulesSha256=="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159"&&mechanisms.states!=null&&mechanisms.states.Length==185,"full exact available mechanism-state document");
   Require(population.route==Route&&population.roomId==Route+"-1"&&binding.route==Route&&binding.roomId==Route+"-0","full original R405 route bindings");
   var rooms=new List<RoomDefinition>();int[] births={31,32,1};string[] worlds={"WI_ALTERA_NASOD_FACTORY_STAGE1","WI_ALTERA_NASOD_FACTORY_STAGE2","WI_ALTERA_NASOD_FACTORY_BOSS"};int exportedActiveDeclarations=0,inactiveDeclarations=0,total=0;
   for(int i=0;i<3;i++) {
    var r=source.rooms[i];var a=audit.rooms[i];
    Require(r!=null&&r.id==Route+"-"+i&&r.sourceWorldId==worlds[i]&&r.enemies!=null&&r.enemies.Length==births[i]&&a!=null&&a.enemies!=null&&a.enemies.Length==births[i],"full original room/birth identity");
    var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,groundNodes=r.groundNodes,surfaces=r.surfaces,encounters=r.encounters};
    for(int j=0;j<r.enemies.Length;j++) {
     var e=r.enemies[j];var declaration=a.enemies[j];Require(e!=null&&declaration!=null&&e.sourceNPC==declaration.sourceNPC&&!e.aiEnabled&&!declaration.aiEnabled,"full original 64 AI-off staging flags");
     if(declaration.sourceActive)exportedActiveDeclarations++;else inactiveDeclarations++;room.enemies.Add(e);total++;
    }
    rooms.Add(room);
   }
   Require(total==64&&exportedActiveDeclarations==52&&inactiveDeclarations==12,"full original source-active export provenance, not AI permission");
   // Only this new independent model is configured. No existing model is
   // accepted or rewritten; the original setters own actor construction and
   // retain their guarded first ordinary Step and source-role/STOP boundaries.
   var model=new GameModel();model.SetNpcDefinitions(rules.definitions);model.SetNpcMechanismDefinitions(mechanisms);
   model.SetFactoryPopulationPolicyV21(population);model.SetFactoryEventStartBindingV21(binding);model.SetRooms(rooms);
   Require(model.room==0&&model.elapsed==0&&model.NativeTerrain,"full fresh independent pre-Step model");
   model.SetFactoryEntryActivationV542(activation,rules.definitions,fullRules,mechanisms,fullMechanisms,controller,sourceController);
   model.SetFactoryPressSourceV569(press,rules.definitions,fullRules,fullMechanisms);
   Require(model.room==0&&model.elapsed==0&&model.FactoryEntryActivationConfiguredV542&&model.FactoryPressSourceConfiguredV569,"full activation and Press configured before first ordinary Step");
   return new FactoryPreparedComponentV21(model);
  }

  internal bool OwnsCurrentDirectorRequest(FactoryEventStartRequestV21 request) {
   if(request==null||request.actualOwner==null||model.room!=0||model.CurrentRoom.id!=Route+"-0"||!model.enemies.Contains(request.actualOwner))return false;
   foreach(var frame in model.ReadFactoryEventStartFramesV21())if(ReferenceEquals(frame.actualOwner,request.actualOwner)&&frame.ownerUid==request.ownerUid)return true;
   return false;
  }
  // Only the immutable source-sound requests are dispatched here. Other request
  // kinds remain in the model and receive no native ACK or model mutation.
  internal void ConsumeCurrentDirectorSounds(Action<FactoryEventStartRequestV21> consume) {
   RequireLive();if(consume==null)throw new ArgumentNullException("consume");if(model.paused)return;
   // Unbound/pre-first-Step/no-current-owner-frame is a suspended consumer,
   // even with an empty ledger. Preserve the cursor until the original
   // binding exposes its same actual owner/UID again; do not drain cues.
   if(model.ReadFactoryEventStartFramesV21().Length==0)return;
   var requests=model.ReadFactoryEventStartRequestsV21();
   if(requests.Length>0&&requests[0].serial>soundRequestCursor+1)throw new InvalidOperationException("Factory sound source request history lost before consumption");
   foreach(var r in requests) {
    if(r.serial<=soundRequestCursor)continue;
    if(r.kind=="source-sound-request"&&OwnsCurrentDirectorRequest(r)) {
     Require(r.state=="EVENT_NPC_NASOD_FACTORY_START"&&r.callback=="SOUND_PLAY"&&r.intArgument==100&&!r.nativeExecuted,"sound source identity");
     Require(r.asset=="AlteraCutScene_LiftMove.ogg"||r.asset=="AlteraCutScene_RobotFall.ogg"||r.asset=="AlteraCutScene_RobotAssemble.ogg","sound literal");
     consume(r);
    }
    soundRequestCursor=r.serial;
   }
  }
  // Disposing ownership cancels later presentation calls; it does not synthesize
  // DYING_END, retire an Enemy, reset a source clock, or clear a source room.
  public void Dispose(){disposed=true;}
 }
}
