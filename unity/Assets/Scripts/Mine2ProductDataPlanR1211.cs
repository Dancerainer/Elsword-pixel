using System;
using System.Collections.Generic;
using System.Text;
namespace ElswordPixel {
 [Serializable] public sealed class Mine2ProductModelR1211 {public string sourceGraph;public bool runtimeAdmitted;public Mine2ProductRoomR1211[] rooms;}
 [Serializable] public sealed class Mine2ProductRoomR1211 {public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;}
 [Serializable] public sealed class Mine2ProductAuditR1211 {public Mine2ProductAuditRoomR1211[] rooms;}
 [Serializable] public sealed class Mine2ProductAuditRoomR1211 {public Mine2ProductAuditBirthR1211[] enemies;}
 [Serializable] public sealed class Mine2ProductAuditBirthR1211 {public string sourceNPC;public bool sourceActive,aiEnabled;}
 [Serializable] public sealed class Mine2ProductRulesR1211 {public bool runtimeAdmitted,sourceMechanismsComplete;public NpcDefinition[] definitions;}
 [Serializable] public sealed class Mine2ProductSourceClocksR1211 {public Mine2ProductSourceProfileR1211[] definitions;}
 [Serializable] public sealed class Mine2ProductSourceProfileR1211 {public string id;public Mine2ProductSourceStateR1211[] states;}
 [Serializable] public sealed class Mine2ProductSourceStateR1211 {public string id;public float sourceMotionDuration;}
 [Serializable] public sealed class Mine2ProductPathR1211 {public float s,length;public float[] start,end;}
 [Serializable] public sealed class Mine2ProductProjectionR1211 {public string id;public float unit,offsetX,offsetY,width,height;public Mine2ProductPathR1211[] path;}
 [Serializable] public sealed class Mine2ProductScenePartR1211 {public string texture;public int offset,count;}
 [Serializable] public sealed class Mine2ProductSceneR1211 {public string id;public Mine2ProductScenePartR1211[] parts;}
 [Serializable] public sealed class Mine2ProductAtlasR1211 {public string image;public int frameWidth,frameHeight;public float[] groundAnchor;}
 [Serializable] public sealed class Mine2ProductFrameR1211 {public int[] rect;public float[] anchor;}
 [Serializable] public sealed class Mine2ProductClipR1211 {public string id;public float fps,durationSeconds;public bool loop;public Mine2ProductFrameR1211[] frames;}
 [Serializable] public sealed class Mine2ProductLibraryDocumentR1211 {public string id,sourceNPC,artVersion,sourceRulesSha256;public bool runtimeAdmitted,animationLibraryApproved;public float logicalScale,standingHeight;public int paintedPoseCount;public Mine2ProductAtlasR1211 atlas;public Mine2ProductClipR1211[] clips;}
 public sealed class Mine2ProductWorldPayloadR1211 {public string id;public byte[] originalSceneJson,originalGeometry;public Mine2ProductProjectionR1211 projection;public Mine2ProductTextureR1211[] textures;}
 public sealed class Mine2ProductLibraryPayloadR1211 {public Mine2ProductLibraryR1211 binding;public Mine2ProductLibraryDocumentR1211 document;public int atlasWidth,atlasHeight;}
 // Pure pre-read plan. No GameModel construction, setters, Step, actor writes,
 // Unity object construction, coroutine, filesystem access or old QA gate.
 public sealed class Mine2ProductDataPlanR1211 {
  public Mine2ProductResourceIndexR1211 index;public Mine2ProductModelR1211 model;public NpcDefinition[] rules;public NpcEffectDocument effects;public NpcSoundDocument sounds;public NpcMechanismDocument mechanisms;
  public Mine2ProductWorldPayloadR1211[] worlds;public Mine2ProductLibraryPayloadR1211[] libraries;public bool[][] sourceActiveCombat;public Mine2ProductSourceClocksR1211 sourceClocks;
  public Mine2VisualDocumentV21 visualDocument;public OutskirtsNpcMeshManifest visualMeshes;public Mine2WorldLayerDocumentV20 worldLayerDocument;
  public byte[] originalModelJson,originalRulesJson,originalMechanismsJson;
  static bool Combat(string id){return id=="NUI_NASOD_MINER_THIN"||id=="NUI_NASOD_MINER_FAT_B"||id=="NUI_NASOD_MINER_FAT_C"||id=="NUI_NASOD_HEALER"||id=="NUI_WALLY_8TH_MK2"||id=="NUI_NASOD_RAIL";}
  static int PngInteger(byte[] b,int at){return checked((int)((uint)b[at]<<24|(uint)b[at+1]<<16|(uint)b[at+2]<<8|b[at+3]));}
  static void PngSize(byte[] b,out int w,out int h){Mine2ProductResourceIndexR1211.Need(b!=null&&b.Length>=24&&b[0]==137&&b[1]==80&&b[2]==78&&b[3]==71&&b[12]==73&&b[13]==72&&b[14]==68&&b[15]==82,"original PNG header missing");w=PngInteger(b,16);h=PngInteger(b,20);Mine2ProductResourceIndexR1211.Need(w>0&&h>0,"original PNG dimensions invalid");}
  public static Mine2ProductDataPlanR1211 Build(Mine2ProductResourceIndexR1211 index){
   Mine2ProductResourceIndexR1211.Need(index!=null,"index absent");var map=index.Map;var p=new Mine2ProductDataPlanR1211{index=index,originalModelJson=index.RootBytes("model"),originalRulesJson=index.RootBytes("rules"),originalMechanismsJson=index.RootBytes("runtimeMechanisms"),effects=index.RootJson<NpcEffectDocument>("effects"),sounds=index.RootJson<NpcSoundDocument>("sounds"),worlds=new Mine2ProductWorldPayloadR1211[3],libraries=new Mine2ProductLibraryPayloadR1211[13],sourceActiveCombat=new bool[3][]};
   p.model=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductModelR1211>(p.originalModelJson);var audit=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductAuditR1211>(p.originalModelJson);var rd=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductRulesR1211>(p.originalRulesJson);p.sourceClocks=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductSourceClocksR1211>(p.originalRulesJson);p.mechanisms=FactoryTypedJsonR581.Read<NpcMechanismDocument>(p.originalMechanismsJson);
   Mine2ProductResourceIndexR1211.Need(p.model!=null&&p.model.sourceGraph==Mine2ProductResourceIndexR1211.Route+".lua"&&!p.model.runtimeAdmitted&&p.model.rooms!=null&&p.model.rooms.Length==3&&audit!=null&&audit.rooms!=null&&audit.rooms.Length==3&&rd!=null&&!rd.runtimeAdmitted&&!rd.sourceMechanismsComplete&&rd.definitions!=null&&rd.definitions.Length==13&&p.sourceClocks!=null&&p.sourceClocks.definitions!=null&&p.sourceClocks.definitions.Length==13&&p.effects!=null&&p.sounds!=null&&p.mechanisms!=null&&p.mechanisms.sourceMine2!=null&&p.mechanisms.sourceMine2MechanismsV21!=null,"original model/rules/typed mechanism payload incomplete");p.rules=rd.definitions;
   string[] ids={"NUI_BOX_BESMA2","NUI_DOWN_STONE_TRAP_BESMA","NUI_NASOD_HEALER","NUI_NASOD_MINER_FAT_B","NUI_NASOD_MINER_FAT_C","NUI_NASOD_MINER_THIN","NUI_NASOD_RAIL","NUI_NASOD_RAIL_HOUSE","NUI_SMALLGATE_BESMA_MINE","NUI_SMALLGATE_BOSS_BESMA_MINE","NUI_SMALLGATE_BOSS_POST_BESMA_MINE","NUI_SMALLGATE_POST_BESMA_MINE","NUI_WALLY_8TH_MK2"};int stateCount=0;
   for(int i=0;i<ids.Length;i++){var r=p.rules[i];Mine2ProductResourceIndexR1211.Need(r!=null&&r.id==ids[i]&&r.states!=null,"source profile order changed");stateCount+=r.states.Length;}Mine2ProductResourceIndexR1211.Need(stateCount==178,"source state set changed");
   int births=0,active=0,supports=0,zones=0,parts=0;int[] counts={30,35,3};
   for(int i=0;i<3;i++){
    var r=p.model.rooms[i];var a=audit.rooms[i];var w=map.worlds[i];var stage=map.stages[i];Mine2ProductResourceIndexR1211.Need(r!=null&&r.id==Mine2ProductResourceIndexR1211.Route+"-"+i&&r.sourceWorldId==w.id&&r.enemies!=null&&r.enemies.Length==counts[i]&&a!=null&&a.enemies!=null&&a.enemies.Length==counts[i]&&r.width==stage.width&&r.spawnX==stage.spawnX&&r.spawnY==stage.spawnY&&r.hasSpawnY==stage.hasSpawnY&&r.exitX==stage.exitX&&r.entryEncounter==stage.entryEncounter&&r.surfaces!=null&&r.encounters!=null,"original room/stage mismatch");
    p.sourceActiveCombat[i]=new bool[counts[i]];for(int j=0;j<counts[i];j++){var e=r.enemies[j];var d=a.enemies[j];Mine2ProductResourceIndexR1211.Need(e!=null&&d!=null&&e.sourceNPC==d.sourceNPC&&!e.aiEnabled&&!d.aiEnabled,"source AI-off declarations changed");p.sourceActiveCombat[i][j]=Combat(d.sourceNPC)&&d.sourceActive;if(p.sourceActiveCombat[i][j])active++;births++;}supports+=r.surfaces.Length;zones+=r.encounters.Length;
    var sceneBytes=index.Read(w.scene);var scene=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductSceneR1211>(sceneBytes);var projection=index.Json<Mine2ProductProjectionR1211>(w.projection);var geometry=index.Read(w.binary);
    Mine2ProductResourceIndexR1211.Need(scene!=null&&scene.id==w.id&&scene.parts!=null&&projection!=null&&projection.id==w.id&&projection.unit==.28f&&projection.width==r.width&&projection.height==stage.height&&projection.path!=null&&projection.path.Length>0,"original world/projection binding changed");
    foreach(var q in projection.path)Mine2ProductResourceIndexR1211.Need(q!=null&&q.start!=null&&q.start.Length==3&&q.end!=null&&q.end.Length==3,"original 3-value source path endpoint changed");
    foreach(var q in scene.parts){Mine2ProductResourceIndexR1211.Need(q!=null&&q.offset>=0&&q.count>0&&(long)q.offset+(long)q.count*20<=geometry.LongLength&&(string.IsNullOrEmpty(q.texture)||Array.Exists(w.textures,t=>t.sourcePath==q.texture)),"original mesh/texture range changed");parts++;}
    p.worlds[i]=new Mine2ProductWorldPayloadR1211{id=w.id,originalSceneJson=sceneBytes,originalGeometry=geometry,projection=projection,textures=w.textures};
   }
   Mine2ProductResourceIndexR1211.Need(births==68&&active==45&&supports==54&&zones==5&&parts==631,"original population/support/mesh totals changed");
   var byNpc=new Dictionary<string,Mine2ProductLibraryDocumentR1211>(StringComparer.Ordinal);
   for(int i=0;i<13;i++){
    var binding=map.libraries[i];var d=index.Json<Mine2ProductLibraryDocumentR1211>(binding.document);int width,height;PngSize(index.Read(binding.atlas),out width,out height);
    Mine2ProductResourceIndexR1211.Need(d!=null&&d.id==binding.id&&d.sourceNPC==binding.npc&&!d.runtimeAdmitted&&!d.animationLibraryApproved&&d.logicalScale==1&&d.atlas!=null&&d.atlas.groundAnchor!=null&&d.atlas.groundAnchor.Length==2&&d.clips!=null&&d.clips.Length>0&&!byNpc.ContainsKey(d.sourceNPC),"original library identity/scale changed");
    if(d.sourceNPC=="NUI_WALLY_8TH_MK2")Mine2ProductResourceIndexR1211.Need(width==2008&&height==4600,"original Wally 2008x4600 atlas changed");
    var clips=new HashSet<string>(StringComparer.Ordinal);foreach(var c in d.clips){Mine2ProductResourceIndexR1211.Need(c!=null&&clips.Add(c.id)&&c.fps==60&&c.durationSeconds>0&&c.frames!=null&&c.frames.Length>0,"original clip identity/timing changed");foreach(var f in c.frames)Mine2ProductResourceIndexR1211.Need(f!=null&&f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2&&f.rect[0]>=0&&f.rect[1]>=0&&f.rect[2]>0&&f.rect[3]>0&&(long)f.rect[0]+f.rect[2]<=width&&(long)f.rect[1]+f.rect[3]<=height,"original signed anchor/atlas rect shape changed");}
    var rule=Array.Find(p.rules,r=>r.id==d.sourceNPC);Mine2ProductResourceIndexR1211.Need(rule!=null&&rule.kind==d.id,"profile/library kind mismatch");foreach(var s in rule.states)Mine2ProductResourceIndexR1211.Need(clips.Contains(s.clip),"missing original state display clip; no idle fallback");
    byNpc.Add(d.sourceNPC,d);p.libraries[i]=new Mine2ProductLibraryPayloadR1211{binding=binding,document=d,atlasWidth=width,atlasHeight=height};
   }
   foreach(var a in map.displayAliases){var r=Array.Find(p.rules,q=>q.id==a.npc);var s=r==null?null:Array.Find(r.states,q=>q.id==a.state);var raw=Array.Find(p.sourceClocks.definitions,q=>q.id==a.npc);var clock=raw==null||raw.states==null?null:Array.Find(raw.states,q=>q.id==a.state);Mine2ProductResourceIndexR1211.Need(a.onlyDisplayClipChanged&&s!=null&&clock!=null&&s.clip==a.displayClip&&s.sourceAnimation==a.sourceAnimation&&s.sourceAnimSpeed==a.sourceAnimSpeed&&clock.sourceMotionDuration==a.sourceMotionDuration&&s.duration==a.sourceStateDuration&&s.loop==a.loop,"seven original display aliases/source clocks changed");}
   p.visualDocument=index.Json<Mine2VisualDocumentV21>(map.visual.document);p.visualMeshes=index.Json<OutskirtsNpcMeshManifest>(map.visual.meshManifest);p.worldLayerDocument=index.Json<Mine2WorldLayerDocumentV20>(map.worldLayer.document);
   Mine2ProductResourceIndexR1211.Need(p.visualDocument!=null&&p.visualDocument.schema=="mine2-source-visual-v21"&&!p.visualDocument.runtimeAdmitted&&p.visualDocument.unit==.28f&&p.visualDocument.particles!=null&&p.visualDocument.particles.Length==40&&p.visualDocument.effectParticles!=null&&p.visualDocument.effectParticles.Length==59&&p.visualDocument.effectPassiveParticles!=null&&p.visualDocument.effectPassiveParticles.Length==2&&p.visualDocument.effectMeshes!=null&&p.visualDocument.effectMeshes.Length==2&&p.visualDocument.callbackDocument!=null&&p.visualMeshes!=null&&p.visualMeshes.components!=null&&p.visualMeshes.components.Length==6,"original visual data incomplete");
   foreach(var t in p.visualDocument.particles)Mine2ProductResourceIndexR1211.Need(t!=null&&BelowPlayerResourceIndexV21.Hash(new UTF8Encoding(false,true).GetBytes(t.particleJson??""))==t.particleJsonSha256,"original normalized particle JSON changed");
   Mine2ProductResourceIndexR1211.Need(p.worldLayerDocument!=null&&p.worldLayerDocument.schema=="mine2-source-worldlayer-display-v20-r1"&&p.worldLayerDocument.roomIndex==0&&p.worldLayerDocument.worldId==map.worlds[0].id&&!p.worldLayerDocument.runtimeAdmitted&&!p.worldLayerDocument.actualUnityComponentVerified&&!p.worldLayerDocument.sourceMeshDeclared,"original world-layer display adapter changed");
   return p;
  }
 }
}
