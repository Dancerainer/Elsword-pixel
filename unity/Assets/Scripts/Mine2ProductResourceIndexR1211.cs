using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class Mine2ProductFileR1211 {public string path,sha256;}
 [Serializable] public sealed class Mine2ProductResourceR1211 {public string sourcePath,sha256,resource,assetType;public int bytes;public bool sharedPublished;public string[] consumers;}
 [Serializable] public sealed class Mine2ProductRootR1211 {public string id,path,sha256;}
 [Serializable] public sealed class Mine2ProductTextureR1211 {public string sourcePath;public Mine2ProductFileR1211 selected;public bool singleMaterialTrial;}
 [Serializable] public sealed class Mine2ProductWorldR1211 {public string id;public int room;public Mine2ProductFileR1211 scene,projection,binary;public Mine2ProductTextureR1211[] textures;}
 [Serializable] public sealed class Mine2ProductLibraryR1211 {public string id,npc;public Mine2ProductFileR1211 document,atlas;}
 [Serializable] public sealed class Mine2ProductStageR1211 {public string id,sourceWorldId,title,bgm;public float width,height,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;}
 [Serializable] public sealed class Mine2ProductAudioR1211 {public string id,originalSha256,decodedPcm16Sha256;public Mine2ProductFileR1211 file;public int sampleRate,channels,samples;}
 [Serializable] public sealed class Mine2ProductMusicR1211 {public int room,inheritFromRoom;public string id,resource,originalLiteral;public bool originalLiteralMissing;public Mine2ProductFileR1211 file;}
 [Serializable] public sealed class Mine2ProductAliasR1211 {public string npc,state,originalClip,displayClip,sourceAnimation;public float sourceMotionDuration,sourceAnimSpeed,sourceStateDuration;public bool loop,onlyDisplayClipChanged;}
 [Serializable] public sealed class Mine2ProductVisualTextureR1211 {public string key;public Mine2ProductFileR1211 file;}
 [Serializable] public sealed class Mine2ProductVisualR1211 {public Mine2ProductFileR1211 document,meshManifest;public Mine2ProductVisualTextureR1211[] textures;}
 [Serializable] public sealed class Mine2ProductWorldLayerR1211 {public Mine2ProductFileR1211 document,texture;}
 [Serializable] public sealed class Mine2ProductMapR1211 {
  public string schema,sourceDungeon,sourceDescriptorSha256,currentCatalogSha256,routeId,routeTitle,routeRegion,sourceAITargetPolicy;
  public int currentCatalogBytes,roomsCount,births,profiles,states,sourceActiveDeclarations,sourceCombatDeclarations,sourceActiveCombatDeclarations,sourceInactiveDeclarations,supports,zones,displayAliasCount,sourceMissingTransitionRows,aliasCount,uniqueResources,unknownSizeResources;
  public long knownUniqueBytes;public bool dataOnly,runtimeReleaseAccepted,pixelArtAccepted,naturalRouteAccepted,nativeEquivalent,normalEntryActuallyVerified;
  public Mine2ProductRootR1211[] roots;public Mine2ProductWorldR1211[] worlds;public Mine2ProductStageR1211[] stages;public Mine2ProductLibraryR1211[] libraries;public Mine2ProductAudioR1211[] audio;public Mine2ProductMusicR1211[] music;public Mine2ProductAliasR1211[] displayAliases;public Mine2ProductVisualR1211 visual;public Mine2ProductWorldLayerR1211 worldLayer;public Mine2ProductResourceR1211[] files;
 }
 // Absolute source strings are data identities only. Runtime reads use typed
 // Resources keys; this adapter has no filesystem, Editor or QA receipt loader.
 public sealed class Mine2ProductResourceIndexR1211 {
  public const string MapResource="Pixel/dungeons/bethma-richie-mine/route-v1211",MapSha256="df2fd482f5fa85987db602c0fa112610e351bfc0625ff9b857120a6573b512f2";
  public const string CatalogSha256="05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3",Route="Dungeon_Besma_MINE2_Normal";
  readonly Dictionary<string,Mine2ProductResourceR1211> files=new Dictionary<string,Mine2ProductResourceR1211>(StringComparer.OrdinalIgnoreCase);
  readonly Dictionary<string,Mine2ProductRootR1211> roots=new Dictionary<string,Mine2ProductRootR1211>(StringComparer.Ordinal);
  readonly Func<string,byte[]> textReader;public Mine2ProductMapR1211 Map {get;private set;}
  internal static void Need(bool value,string why){if(!value)throw new InvalidOperationException("Mine2 product data: "+why);}
  internal static string Identity(string path){return(path??"").Replace('\\','/');}
  static byte[] ResourceBytes(string key){var a=Resources.Load<TextAsset>(key);return a?a.bytes:null;}
  public static Mine2ProductMapR1211 ReadMap(){var bytes=ResourceBytes(MapResource);if(bytes==null)return null;Need(bytes.Length<=512*1024&&BelowPlayerResourceIndexV21.Hash(bytes)==MapSha256,"map bytes changed");var map=JsonUtility.FromJson<Mine2ProductMapR1211>(new UTF8Encoding(false,true).GetString(bytes));ValidateMap(map);return map;}
  public Mine2ProductResourceIndexR1211(Mine2ProductMapR1211 map):this(map,ResourceBytes){}
  internal Mine2ProductResourceIndexR1211(Mine2ProductMapR1211 map,Func<string,byte[]> reader){ValidateMap(map);Need(reader!=null,"typed reader absent");Map=map;textReader=reader;foreach(var f in map.files)files.Add(Identity(f.sourcePath),f);foreach(var r in map.roots)roots.Add(r.id,r);}
  public static void ValidateMap(Mine2ProductMapR1211 m){
   Need(m!=null&&m.schema=="mine2-normal-product-runtime-data-v1211"&&m.dataOnly&&m.sourceDungeon==Route&&m.routeId=="bethma-richie-mine"&&m.routeTitle=="RICHIE MINE"&&m.routeRegion=="bethma","route identity changed");
   Need(m.sourceDescriptorSha256=="77c45bff3e719f980a0fc0231141d58bf7787b55623df1ee2aa9b53359f1d88c"&&m.currentCatalogSha256==CatalogSha256&&m.currentCatalogBytes==41191929,"current source/Main binding changed");
   Need(m.roomsCount==3&&m.births==68&&m.profiles==13&&m.states==178&&m.sourceActiveDeclarations==47&&m.sourceCombatDeclarations==47&&m.sourceActiveCombatDeclarations==45&&m.sourceInactiveDeclarations==21&&m.supports==54&&m.zones==5&&m.displayAliasCount==7&&m.sourceMissingTransitionRows==5&&m.sourceAITargetPolicy=="sourceActive-and-exact-six-combat-IDs-only-after-real-model-gate-v20","original data scope changed");
   Need(!m.runtimeReleaseAccepted&&!m.pixelArtAccepted&&!m.naturalRouteAccepted&&!m.nativeEquivalent&&!m.normalEntryActuallyVerified,"data map cannot grant runtime/art/native acceptance");
   Need(m.roots!=null&&m.roots.Length==5&&m.worlds!=null&&m.worlds.Length==3&&m.stages!=null&&m.stages.Length==3&&m.libraries!=null&&m.libraries.Length==13&&m.audio!=null&&m.audio.Length==56&&m.music!=null&&m.music.Length==3&&m.displayAliases!=null&&m.displayAliases.Length==7&&m.visual!=null&&m.visual.textures!=null&&m.visual.textures.Length==26&&m.worldLayer!=null,"typed data incomplete");
   Need(m.files!=null&&m.files.Length==170&&m.aliasCount==170&&m.uniqueResources==169&&m.knownUniqueBytes==37733676&&m.unknownSizeResources==0,"resource inventory changed");
   var paths=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var unique=new Dictionary<string,Mine2ProductResourceR1211>(StringComparer.Ordinal);long total=0;int clips=0,shared=0;
   foreach(var f in m.files){
    Need(f!=null&&!string.IsNullOrEmpty(f.sourcePath)&&paths.Add(Identity(f.sourcePath))&&BelowPlayerResourceIndexV21.IsHash(f.sha256)&&f.bytes>=0&&f.bytes<=64*1024*1024,"invalid source identity");
    string key=f.assetType=="TextAsset"?"Pixel/dungeons/bethma-richie-mine/data/"+f.sha256:f.assetType=="AudioClip"?"Pixel/dungeons/bethma-richie-mine/audio/"+f.sha256:null;
    if(f.sharedPublished)Need(f.assetType=="AudioClip"&&((f.resource=="Pixel/music/music020_stage"&&f.sha256=="2aaee411a3f051bb88375b3996dca5228c8c3e33abbcb1c779459b9aaec47d31")||(f.resource=="Pixel/music/music011_boss"&&f.sha256=="85123135e880f97f1a5f1a7c42433eb91241fcc776bd41b4395a4e320083f068")),"published music identity changed");
    else Need(key!=null&&f.resource==key,"unmapped typed resource");
    Mine2ProductResourceR1211 prior;if(unique.TryGetValue(f.resource,out prior))Need(prior.sha256==f.sha256&&prior.bytes==f.bytes&&prior.assetType==f.assetType&&prior.sharedPublished==f.sharedPublished,"conflicting shared content");else{unique.Add(f.resource,f);total+=f.bytes;if(f.assetType=="AudioClip")clips++;if(f.sharedPublished)shared++;}
   }
   Need(unique.Count==169&&total==37733676&&clips==57&&shared==2,"typed inventory totals changed");
   var rootIds=new HashSet<string>(StringComparer.Ordinal);foreach(var r in m.roots)Need(r!=null&&rootIds.Add(r.id)&&Array.Exists(m.files,f=>Identity(f.sourcePath)==Identity(r.path)&&f.sha256==r.sha256&&f.assetType=="TextAsset"),"root identity absent");
   foreach(string id in new[]{"model","rules","effects","sounds","runtimeMechanisms"})Need(rootIds.Contains(id),"missing runtime root "+id);
   string[] worlds={"WI_BESMA_MINE2_STAGE1","WI_BESMA_MINE2_STAGE2A","WI_BESMA_MINE2_STAGE4"};for(int i=0;i<3;i++)Need(m.worlds[i]!=null&&m.worlds[i].id==worlds[i]&&m.worlds[i].room==i&&m.stages[i]!=null&&m.stages[i].id==Route+"-"+i&&m.stages[i].sourceWorldId==worlds[i]&&m.music[i]!=null&&m.music[i].room==i&&m.stages[i].bgm==m.music[i].id&&m.music[i].id==(i<2?"music020_stage":"music011_boss")&&m.music[i].inheritFromRoom==(i==1?0:-1)&&m.music[i].originalLiteralMissing==(i==1),"world/music order or room2 inheritance changed");
   Need(m.music[1].originalLiteral=="Besma_Besma_Mine2_Stage2A.ogg"&&m.music[1].resource==m.music[0].resource&&m.music[1].file.sha256==m.music[0].file.sha256,"missing source literal must retain explicit inherited selection");
   var npcs=new HashSet<string>(StringComparer.Ordinal);foreach(var l in m.libraries)Need(l!=null&&npcs.Add(l.npc)&&!string.IsNullOrEmpty(l.id)&&l.document!=null&&l.atlas!=null,"library identity missing");
   var sounds=new HashSet<string>(StringComparer.Ordinal);foreach(var a in m.audio)Need(a!=null&&sounds.Add(a.id)&&a.file!=null&&BelowPlayerResourceIndexV21.IsHash(a.originalSha256)&&BelowPlayerResourceIndexV21.IsHash(a.decodedPcm16Sha256)&&a.sampleRate>0&&a.channels>0&&a.samples>0&&Array.Exists(m.files,f=>Identity(f.sourcePath)==Identity(a.file.path)&&f.sha256==a.file.sha256&&f.assetType=="AudioClip"&&!f.sharedPublished),"PCM AudioClip binding missing");
  }
  Mine2ProductResourceR1211 Resolve(Mine2ProductFileR1211 p,string type){Need(p!=null,"file binding absent");Mine2ProductResourceR1211 f;Need(files.TryGetValue(Identity(p.path),out f)&&f.sha256==p.sha256&&f.assetType==type,"unmapped exact typed identity "+p.path);return f;}
  public byte[] Read(Mine2ProductFileR1211 p){var f=Resolve(p,"TextAsset");var bytes=textReader(f.resource);Need(bytes!=null&&bytes.Length==f.bytes&&BelowPlayerResourceIndexV21.Hash(bytes)==f.sha256,"resource missing/changed "+f.resource);return bytes;}
  public T Json<T>(Mine2ProductFileR1211 p){return FactoryTypedJsonR581.ReadMine2ProductRootR1226<T>(Read(p));}
  public byte[] RootBytes(string id){Mine2ProductRootR1211 r;Need(roots.TryGetValue(id,out r),"missing root "+id);return Read(new Mine2ProductFileR1211{path=r.path,sha256=r.sha256});}
  public T RootJson<T>(string id){return FactoryTypedJsonR581.ReadMine2ProductRootR1226<T>(RootBytes(id));}
  // Imported AudioClips are borrowed. The eventual Host must restore old ID
  // bindings and never add these clips to its owned destruction collection.
  public AudioClip Audio(Mine2ProductAudioR1211 a){Need(a!=null,"sound record absent");var f=Resolve(a.file,"AudioClip");var clip=Resources.Load<AudioClip>(f.resource);Need(clip&&clip.loadState!=AudioDataLoadState.Failed&&clip.samples==a.samples&&clip.channels==a.channels&&clip.frequency==a.sampleRate,"imported original PCM metadata changed "+a.id);return clip;}
  public AudioClip Music(Mine2ProductMusicR1211 a){Need(a!=null,"music record absent");var f=Resolve(a.file,"AudioClip");Need(f.sharedPublished&&f.resource==a.resource,"published music alias changed");var clip=Resources.Load<AudioClip>(f.resource);Need(clip&&clip.loadState!=AudioDataLoadState.Failed,"published music unavailable "+a.id);return clip;}
  public void RequireCurrentCatalog(byte[] bytes,ArtCatalog current){RequireCurrentCatalog(new CatalogDigest(bytes),current);}
  internal static bool OriginalShape(ArtCatalog current){return current!=null&&current.dungeons!=null&&current.dungeons.Length==12&&current.stages!=null&&current.stages.Length==35&&current.npcContentSets!=null&&current.npcContentSets.Length==8;}
  internal void RequireCurrentCatalog(CatalogDigest digest,ArtCatalog current){RequireCurrentCatalog(digest,OriginalShape(current));}
  internal void RequireCurrentCatalog(CatalogDigest digest,bool originalShape){Need(digest.Matches(41191929,CatalogSha256),"current Main catalog changed");Need(originalShape,"current Main route counts changed");}
 }
}
