// Resources-backed candidate bridge. Native parser/decode and Player proof remain separate.
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class BelowPlayerSpriteV21 {public string id,sourceNPC,role;public BelowPlayerFileV21 animations,atlas;}
 [Serializable] public sealed class BelowPlayerTextureV21 {public string sourcePath;public BelowPlayerFileV21 source,selected;}
 [Serializable] public sealed class BelowPlayerWorldV21 {public string id;public BelowPlayerFileV21 scene,projection,binary;public BelowPlayerTextureV21[] textures;}
 [Serializable] public sealed class BelowPlayerMusicV21 {public int room,sampleRate,channels,sourcePcmFrames;public string id,resource,world;public bool externalOriginal,replacementAssigned;public BelowPlayerFileV21 file,pcm;}
 [Serializable] public sealed class BelowPlayerBundleV21 {public string schema,sourceDungeon;public bool editorOnly,runtimeAdmitted,releaseReady,playableReady;public BelowPlayerSpriteV21[] sprites;public BelowPlayerWorldV21[] worlds;public BelowPlayerMusicV21[] originalMusic;}
 [Serializable] public sealed class BelowPlayerRoomV21 {public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;}
 [Serializable] public sealed class BelowPlayerModelV21 {public bool runtimeAdmitted;public BelowPlayerRoomV21[] rooms;}
 [Serializable] public sealed class BelowPlayerRulesV21 {public NpcDefinition[] definitions;}
 [Serializable] public sealed class BelowPlayerSoundRowV21 {public string id;public BelowPlayerFileV21 original,pcm;public int sampleRate,channels,sourcePcmFrames;}
 [Serializable] public sealed class BelowPlayerSoundMapV21 {public string schema;public BelowPlayerSoundRowV21[] rows;}
 [Serializable] public sealed class BelowPlayerPathV21 {public float s,length;public float[] start,end;}
 [Serializable] public sealed class BelowPlayerProjectionV21 {public string id;public float unit,offsetX,offsetY,width,height;public BelowPlayerPathV21[] path;}
 [Serializable] public sealed class BelowPlayerWorldPartV21 {public string texture;public int offset,count;}
 [Serializable] public sealed class BelowPlayerWorldSceneV21 {public string id;public BelowPlayerWorldPartV21[] parts;}
 public sealed class BelowPlayerWorldPayloadV21 {public BelowPlayerWorldV21 binding;public BelowPlayerProjectionV21 projection;public byte[] scene,geometry;public Dictionary<string,byte[]> textures;}
 public sealed class BelowPlayerDataPlanV21 {
  public DungeonArt route;public RoomDefinition[] rooms;public StageArt[] stages;
  public NpcDefinition[] rules;public NpcEffectDocument effects;public NpcSoundDocument sounds;public NpcMechanismDocument mechanisms;
  public BelowPlayerSpriteV21[] sprites;public BelowPlayerWorldPayloadV21[] worlds;public BelowPlayerSoundRowV21[] audio;public BelowPlayerMusicV21[] music;
  public BelowSourceStateClipV21 sourceClips;public BelowPhysicalReactionClipV21 physicalClips;public BelowRecoveryLookupV21 recoveryLookup;
  public bool runtimeAdmitted {get{return false;}}public bool hostIntegrationComplete {get{return false;}}
 }
 public sealed class BelowPlayerDataBridgeV21 {
  readonly BelowPlayerManifestV21 manifest;readonly BelowPlayerResourceIndexV21 index;readonly Func<string,byte[]> resourceReader;readonly Func<byte[],Type,object> parse;
  static readonly string[] Worlds={"WI_ALTERA_BELOW_TUNNEL_STAGE1","WI_ALTERA_BELOW_TUNNEL_STAGE2A","WI_ALTERA_BELOW_TUNNEL_STAGE4"};
  static readonly string[] NoBody={"NUI_STEAM_TRAP","NUI_STEAM_TRAP_DOWN","NUI_STEAM_TRAP_SIDE"};
  BelowPlayerDataBridgeV21(BelowPlayerManifestV21 m,BelowPlayerResourceIndexV21 i,Func<string,byte[]> reader,Func<byte[],Type,object> parser){manifest=m;index=i;resourceReader=reader;parse=parser;}
  public static BelowPlayerDataBridgeV21 Open(Func<string,byte[]> reader,Func<byte[],Type,object> parser){BelowPlayerResourceIndexV21.Need(reader!=null&&parser!=null,"Missing resource/parser dependency");var bytes=reader(BelowPlayerBootstrapV21.ManifestResource);BelowPlayerResourceIndexV21.Need(BelowPlayerResourceIndexV21.Hash(bytes)==BelowPlayerBootstrapV21.ManifestSha256,"Manifest differs from compiled bootstrap");var m=(BelowPlayerManifestV21)parser(bytes,typeof(BelowPlayerManifestV21));return new BelowPlayerDataBridgeV21(m,new BelowPlayerResourceIndexV21(m,reader),reader,parser);}
  public BelowPlayerFileV21 VisualBinding {get{return new BelowPlayerFileV21{path=manifest.roots.effectPresentationBinding.path,sha256=manifest.roots.effectPresentationBinding.sha256};}}
  public T Parse<T>(byte[] bytes){return (T)parse(bytes,typeof(T));}
  static byte[] ResourceBytes(string key){var asset=Resources.Load<TextAsset>(key);BelowPlayerResourceIndexV21.Need(asset!=null,"Embedded TextAsset unavailable "+key);return asset.bytes;}
  public static BelowPlayerDataBridgeV21 OpenFrozen(){
   return Open(ResourceBytes,(bytes,type)=>JsonUtility.FromJson(Encoding.UTF8.GetString(bytes),type));
  }
  public byte[] Read(BelowPlayerFileV21 file){return index.Read(file);}
  public T ReadJson<T>(BelowPlayerFileV21 file){return Parse<T>(Read(file));}
  // This validates and returns exact data. It does not append to catalog, set
  // reviewed flags, enable AI, enter a room, or acquire scene/library ownership.
  public BelowPlayerDataPlanV21 BuildReviewPlan(){
   var baselineBytes=resourceReader("Pixel/runtime-catalog");index.CheckBaseline(baselineBytes);var baseline=Parse<ArtCatalog>(baselineBytes);
   return BuildPlanR1130(baseline);
  }
  BelowPlayerPcm16V21 CheckPcm(BelowPlayerFileV21 file,int rate,int channels,int frames){var pcm=BelowPlayerPcm16V21.Inspect(Read(file));BelowPlayerResourceIndexV21.Need(pcm.sampleRate==rate&&pcm.channels==channels&&pcm.frames==frames,"Original decoded PCM shape changed");return pcm;}
  // Returned Unity objects belong to the future caller; no global registry or
  // borrowed catalog object is touched by these constructors.
  public AudioClip CreatePcmAudio(BelowPlayerFileV21 file,string id,int rate,int channels,int frames){
   var bytes=Read(file);var pcm=BelowPlayerPcm16V21.Inspect(bytes);BelowPlayerResourceIndexV21.Need(pcm.sampleRate==rate&&pcm.channels==channels&&pcm.frames==frames,"PCM decode dimensions differ");var samples=new float[pcm.dataBytes/2];for(int i=0;i<samples.Length;i++){int at=pcm.dataOffset+i*2;short value=(short)(bytes[at]|bytes[at+1]<<8);samples[i]=value/32768f;}var clip=AudioClip.Create(id,frames,channels,rate,false);try{BelowPlayerResourceIndexV21.Need(clip!=null&&clip.SetData(samples,0),"Embedded PCM AudioClip unavailable");return clip;}catch{if(clip)UnityEngine.Object.Destroy(clip);throw;}
  }
  public Texture2D CreatePointTexture(BelowPlayerFileV21 file){var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);try{BelowPlayerResourceIndexV21.Need(texture.LoadImage(Read(file),false),"Embedded PNG decode failed");texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;return texture;}catch{UnityEngine.Object.Destroy(texture);throw;}}
 
  BelowPlayerDataPlanV21 BuildPlanR1130(ArtCatalog baseline){

   BelowPlayerResourceIndexV21.Need(baseline.dungeons.Length==12&&baseline.stages.Length==35&&baseline.npcContentSets.Length==8,"Baseline route/content slots changed");
   foreach(var route in baseline.dungeons)BelowPlayerResourceIndexV21.Need(route.sourceGraph!=BelowPlayerResourceIndexV21.SourceDungeon,"Below already admitted in baseline");
   var bundle=ReadJson<BelowPlayerBundleV21>(manifest.roots.bundle);
   BelowPlayerResourceIndexV21.Need(bundle.schema=="altera-below-tunnel-editor-entry-v21"&&bundle.sourceDungeon==BelowPlayerResourceIndexV21.SourceDungeon&&bundle.editorOnly&&bundle.playableReady&&!bundle.runtimeAdmitted&&!bundle.releaseReady,"Original approved Editor bundle metadata changed");
   BelowPlayerResourceIndexV21.Need(bundle.sprites.Length==21&&bundle.worlds.Length==3&&bundle.originalMusic.Length==3,"Approved visible/world/music closure changed");
   var model=ReadJson<BelowPlayerModelV21>(manifest.roots.model);var rules=ReadJson<BelowPlayerRulesV21>(manifest.roots.rules).definitions;var effects=ReadJson<NpcEffectDocument>(manifest.roots.effects);var sounds=ReadJson<NpcSoundDocument>(manifest.roots.sounds);var mechanisms=ReadJson<NpcMechanismDocument>(manifest.roots.mechanisms);
   BelowPlayerResourceIndexV21.Need(!model.runtimeAdmitted&&model.rooms.Length==3&&rules.Length==24&&effects.definitions.Length==11&&sounds.assets.Length==67,"Model/source rule closure changed");
   var identities=new HashSet<string>(StringComparer.Ordinal);int states=0;foreach(var rule in rules){BelowPlayerResourceIndexV21.Need(identities.Add(rule.id),"Duplicate source identity");states+=rule.states.Length;}BelowPlayerResourceIndexV21.Need(states==279,"Source state count changed");
   var plan=new BelowPlayerDataPlanV21{route=new DungeonArt{id="altera-below-tunnel",title="Altera Below Tunnel",region="altera",sourceGraph=BelowPlayerResourceIndexV21.SourceDungeon,start=35,count=3,combatZones=6,iterationPreview=true},rooms=new RoomDefinition[3],stages=new StageArt[3],worlds=new BelowPlayerWorldPayloadV21[3],rules=rules,effects=effects,sounds=sounds,mechanisms=mechanisms,sprites=bundle.sprites,music=bundle.originalMusic};
   plan.sourceClips=new BelowSourceStateClipV21(rules);plan.physicalClips=new BelowPhysicalReactionClipV21(ReadJson<BelowPhysicalReactionDocumentV21>(manifest.roots.unboundPhysicalReactionPolicy),rules,manifest.roots.rules.sha256);plan.recoveryLookup=new BelowRecoveryLookupV21(ReadJson<BelowRecoveryLookupDocumentV21>(manifest.roots.recoveryLookupPolicy),rules,manifest.roots.rules.sha256);
   int births=0,supports=0,zones=0;
   for(int i=0;i<3;i++){
    var r=model.rooms[i];var w=bundle.worlds[i];var projection=ReadJson<BelowPlayerProjectionV21>(w.projection);var sceneBytes=Read(w.scene);var scene=Parse<BelowPlayerWorldSceneV21>(sceneBytes);var geometry=Read(w.binary);
    BelowPlayerResourceIndexV21.Need(r.id==BelowPlayerResourceIndexV21.SourceDungeon+"-"+i&&r.sourceWorldId==Worlds[i]&&w.id==Worlds[i]&&projection.id==w.id&&scene.id==w.id&&projection.unit==.28f&&projection.path!=null&&projection.path.Length>0,"Original room/world/projection changed");
    BelowPlayerResourceIndexV21.Need(r.enemies.Length==new[]{35,36,7}[i]&&r.surfaces.Length==new[]{40,26,12}[i]&&r.encounters.Length==new[]{2,3,1}[i],"Original births/supports/clear zones changed");
    var textures=new Dictionary<string,byte[]>(StringComparer.Ordinal);foreach(var texture in w.textures){BelowPlayerResourceIndexV21.Need(!textures.ContainsKey(texture.sourcePath),"Duplicate world texture source key");textures.Add(texture.sourcePath,Read(texture.selected));}
    foreach(var part in scene.parts)BelowPlayerResourceIndexV21.Need(part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=geometry.Length&&(string.IsNullOrEmpty(part.texture)||textures.ContainsKey(part.texture)),"Source world vertex/texture binding incomplete");
    var song=bundle.originalMusic[i];BelowPlayerResourceIndexV21.Need(song.room==i&&song.world==w.id&&!song.replacementAssigned&&song.id==(i==2?"music015_boss":"music023_stage"),"Original BGM identity/policy changed");
    if(song.externalOriginal)CheckPcm(song.pcm,song.sampleRate,song.channels,song.sourcePcmFrames);else BelowPlayerResourceIndexV21.Need(song.resource=="Pixel/music/music015_boss","Existing admitted BGM resource changed");
    var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,encounters=r.encounters,groundNodes=r.groundNodes,surfaces=r.surfaces};
    foreach(var enemy in r.enemies){BelowPlayerResourceIndexV21.Need(identities.Contains(enemy.sourceNPC),"Unbound original source birth");room.enemies.Add(enemy);}plan.rooms[i]=room;
    plan.stages[i]=new StageArt{id=r.id,title="Altera Below Tunnel / Source Room "+(i+1),sourceWorldId=w.id,width=projection.width,height=projection.height,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,encounters=r.encounters,groundNodes=r.groundNodes,surfaces=r.surfaces,nativeScene=w.id,bgm=song.id};
    plan.worlds[i]=new BelowPlayerWorldPayloadV21{binding=w,projection=projection,scene=sceneBytes,geometry=geometry,textures=textures};births+=r.enemies.Length;supports+=r.surfaces.Length;zones+=r.encounters.Length;
   }
   BelowPlayerResourceIndexV21.Need(births==78&&supports==78&&zones==6,"Original route aggregate changed");
   var map=ReadJson<BelowPlayerSoundMapV21>(manifest.roots.soundMap);BelowPlayerResourceIndexV21.Need(map.rows.Length==67,"Exact source audio map changed");var soundIds=new HashSet<string>(StringComparer.Ordinal);foreach(var row in map.rows){BelowPlayerResourceIndexV21.Need(soundIds.Add(row.id)&&Array.Exists(sounds.assets,a=>a.id==row.id&&a.sha256==row.original.sha256),"Unbound/duplicate original audio identity");CheckPcm(row.pcm,row.sampleRate,row.channels,row.sourcePcmFrames);}plan.audio=map.rows;
   // Validate every runtime payload hash, including exact sprite/FX source
   // documents; host-specific typed FX/atlas construction is a remaining step.
   foreach(var sprite in bundle.sprites){Read(sprite.animations);Read(sprite.atlas);BelowPlayerResourceIndexV21.Need(Array.Exists(rules,r=>r.id==sprite.sourceNPC&&r.kind==sprite.id)&&Array.IndexOf(NoBody,sprite.sourceNPC)<0,"Wrong visible exact sprite identity");}
   index.ValidateAllResources();return plan;
  }

  internal static BelowPlayerDataBridgeV21 OpenCurrentProductR1130(BelowProductResourceMapR1130 map){
   BelowProductResourcesR1130.Validate(map);
   var data=new BelowPlayerManifestV21{schema=map.schema,sourceDungeon=map.sourceDungeon,roots=map.roots,files=map.files};
   return new BelowPlayerDataBridgeV21(data,new BelowPlayerResourceIndexV21(map,ResourceBytes),ResourceBytes,(bytes,type)=>JsonUtility.FromJson(Encoding.UTF8.GetString(bytes),type));
  }
  internal BelowPlayerDataPlanV21 BuildCurrentProductPlanR1130(ArtCatalog current,BelowProductResourceMapR1130 map){
   BelowProductResourcesR1130.RequireCurrentCatalog(map,resourceReader("Pixel/runtime-catalog"),current);
   var plan=BuildPlanR1130(current);plan.route.title=map.routeTitle;plan.route.description=map.routeDescription;
   for(int i=0;i<3;i++)plan.stages[i].title=map.routeTitle+" / AREA "+(i+1);
   return plan;
  }

}
}
