using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class FactoryProductFileR1168 {public string path,sha256;}
 [Serializable] public sealed class FactoryProductResourceR1168 {public string sourcePath,sha256,resource,assetType;public int bytes;public bool sizeKnown;public string[] consumers;}
 [Serializable] public sealed class FactoryProductRootR1168 {public string id,path,sha256;}
 [Serializable] public sealed class FactoryProductTextureR1168 {public string sourcePath;public FactoryProductFileR1168 selected;public bool singleMaterialTrial;}
 [Serializable] public sealed class FactoryProductWorldR1168 {public string id;public int room;public FactoryProductFileR1168 scene,projection,binary;public FactoryProductTextureR1168[] textures;}
 [Serializable] public sealed class FactoryProductKeyR1168 {public string key,originalPath,rgbaSha256;public FactoryProductFileR1168 file;public int width,height;}
 [Serializable] public sealed class FactoryProductBankR1168 {public string npc,id,kind;public bool originalReference,artApproved;public FactoryProductFileR1168 document;public FactoryProductKeyR1168[] keys;public int stateCount,motionCount,sourcePoseSamples;public long rgbaBytes;}
 [Serializable] public sealed class FactoryProductVisualStateR1168 {public string npc,state,clip,motion,durationBits,motionDurationBits,speedBits;public bool loop;}
 [Serializable] public sealed class FactoryProductAudioR1168 {public string id;public FactoryProductFileR1168 file;}
 [Serializable] public sealed class FactoryProductStageR1168 {public string id,sourceWorldId,bgm,title;public float width,height,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;public EnemySpawn[] spawns;}
 [Serializable] public sealed class FactoryProductMusicR1168 {public int room;public string id;}
 [Serializable] public sealed class FactoryProductMapR1168 {
  public string schema,sourceDungeon,sourceDescriptorSha256,currentCatalogSha256,routeId,routeTitle,routeRegion;
  public int currentCatalogBytes,roomsCount,births,profiles,states,sourceActiveDeclarations,sourceInactiveDeclarations,pixelBanks,originalReferenceBanks,keys,aliasCount,uniqueResources,unknownSizeResources;
  public long knownUniqueBytes;public bool dataOnly,runtimeReleaseAccepted,pixelArtAccepted,naturalRouteAccepted,nativeEquivalent,normalEntryActuallyVerified,sourceNoBrainPreserved;
  public FactoryProductRootR1168[] roots;public FactoryProductWorldR1168[] worlds;public FactoryProductBankR1168[] banks;public FactoryProductVisualStateR1168[] twoBankVisualStates;public FactoryProductAudioR1168[] audio;public FactoryProductStageR1168[] stages;public FactoryProductMusicR1168[] music;public FactoryProductResourceR1168[] files;
 }
 // Source paths are immutable data identities, never filesystem lookup targets.
 // This finite Factory adapter has no File/Directory/URI, Editor or acceptance-receipt read.
 public sealed class FactoryProductResourceIndexR1168 {
  public const string MapResource="Pixel/dungeons/altera-nasod-factory/route-v1168";
  public const string MapSha256="39f7b64ef9ad66abf92cff5950191b49c160b2bcf564ebd3860ae8556e69e644";
  public const string MakerPixelIdR1223="nasod-maker-pixel-finite-r1222";
  public const string MakerPixelDocumentSha256R1223="6c575178ce38ab43255c14521cfba71fac7024e8a0d53282b9487d6b8ef8fbdf";
  public const string GuardPixelIdR1264="nasod-guard-pixel-finite-r1264";
  public const string GuardPixelDocumentSha256R1264="da98215d4c3d35c83e4a30f81efdded11ed54d60a77f2c518f000535bac0c721";
  public const string CatalogSha256="05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3";
  public const string Route="Dungeon_Altera_Nasod_Factory_Normal";
  readonly Dictionary<string,FactoryProductResourceR1168> files=new Dictionary<string,FactoryProductResourceR1168>(StringComparer.OrdinalIgnoreCase);
  readonly Dictionary<string,FactoryProductRootR1168> roots=new Dictionary<string,FactoryProductRootR1168>(StringComparer.Ordinal);
  readonly Func<string,byte[]> readText;public FactoryProductMapR1168 Map {get;private set;}
  internal static void Need(bool valid,string why){if(!valid)throw new InvalidOperationException("Factory product data: "+why);}
  internal static string Identity(string path){string s=(path??"").Replace('\\','/');while(s.Contains("//"))s=s.Replace("//","/");return s;}
  static byte[] ResourceBytes(string key){var a=Resources.Load<TextAsset>(key);return a?a.bytes:null;}
  public static FactoryProductMapR1168 ReadMap(){var raw=ResourceBytes(MapResource);if(raw==null)return null;Need(raw.Length<=1024*1024&&BelowPlayerResourceIndexV21.Hash(raw)==MapSha256,"map identity changed");var m=JsonUtility.FromJson<FactoryProductMapR1168>(new UTF8Encoding(false,true).GetString(raw));ValidateMap(m);return m;}
  public FactoryProductResourceIndexR1168(FactoryProductMapR1168 map):this(map,ResourceBytes){}
  internal FactoryProductResourceIndexR1168(FactoryProductMapR1168 map,Func<string,byte[]> loader){ValidateMap(map);Need(loader!=null,"raw Resources reader absent");Map=map;readText=loader;foreach(var r in map.files)files.Add(Identity(r.sourcePath),r);foreach(var r in map.roots)roots.Add(r.id,r);}
  public static void ValidateMap(FactoryProductMapR1168 m){
   Need(m!=null&&m.schema=="factory-normal-product-runtime-data-v1168"&&m.dataOnly&&m.sourceDungeon==Route&&m.routeId=="altera-nasod-factory"&&m.routeRegion=="altera"&&m.routeTitle=="NASOD FACTORY","wrong product route");
   Need(m.sourceDescriptorSha256=="cf8d2dc70d7923a16225172631d712423a05b831bba6cbf00b165326af2ad6e1"&&m.currentCatalogSha256==CatalogSha256&&m.currentCatalogBytes==41191929,"original data/current Main identity changed");
   Need(m.roomsCount==3&&m.births==64&&m.profiles==11&&m.states==185&&m.sourceActiveDeclarations==52&&m.sourceInactiveDeclarations==12&&m.pixelBanks==4&&m.originalReferenceBanks==7&&m.keys==418&&m.sourceNoBrainPreserved,"source roles/births/banks changed");
   Need(!m.runtimeReleaseAccepted&&!m.pixelArtAccepted&&!m.naturalRouteAccepted&&!m.nativeEquivalent&&!m.normalEntryActuallyVerified,"data map cannot invent runtime/art/native acceptance");
   Need(m.roots!=null&&m.roots.Length==10&&m.worlds!=null&&m.worlds.Length==3&&m.stages!=null&&m.stages.Length==3&&m.banks!=null&&m.banks.Length==11&&m.twoBankVisualStates!=null&&m.twoBankVisualStates.Length==53&&m.audio!=null&&m.audio.Length==59&&m.music!=null&&m.music.Length==3,"incomplete typed data");
   Need(m.files!=null&&m.files.Length==584&&m.aliasCount==584&&m.uniqueResources==583&&m.knownUniqueBytes==22296124&&m.unknownSizeResources==0,"resource scope changed");
   var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var unique=new Dictionary<string,FactoryProductResourceR1168>(StringComparer.Ordinal);long total=0;int unknown=0,audio=0;
   foreach(var r in m.files){Need(r!=null&&!string.IsNullOrEmpty(r.sourcePath)&&seen.Add(Identity(r.sourcePath))&&BelowPlayerResourceIndexV21.IsHash(r.sha256),"duplicate/malformed source identity");Need(r.sizeKnown?r.bytes>=0&&r.bytes<=64*1024*1024:r.bytes==-1,"unknown size must be explicit -1");string expected=r.assetType=="TextAsset"?"Pixel/dungeons/altera-nasod-factory/data/"+r.sha256:r.assetType=="AudioClip"?"Pixel/dungeons/altera-nasod-factory/audio/"+r.sha256:null;Need(expected!=null&&r.resource==expected,"unmapped type or key");FactoryProductResourceR1168 prior;if(unique.TryGetValue(r.resource,out prior))Need(prior.sha256==r.sha256&&prior.bytes==r.bytes&&prior.assetType==r.assetType&&prior.sizeKnown==r.sizeKnown,"conflicting deduplicated identity");else{unique.Add(r.resource,r);if(r.sizeKnown)total+=r.bytes;else unknown++;if(r.assetType=="AudioClip")audio++;}}
   Need(unique.Count==m.uniqueResources&&total==m.knownUniqueBytes&&unknown==m.unknownSizeResources&&audio==59,"resource totals changed");
   var ids=new HashSet<string>(StringComparer.Ordinal);foreach(var r in m.roots)Need(r!=null&&ids.Add(r.id)&&Array.Exists(m.files,f=>Identity(f.sourcePath)==Identity(r.path)&&f.sha256==r.sha256&&f.assetType=="TextAsset"),"runtime root identity missing");
   string[] worlds={"WI_ALTERA_NASOD_FACTORY_STAGE1","WI_ALTERA_NASOD_FACTORY_STAGE2","WI_ALTERA_NASOD_FACTORY_BOSS"};for(int i=0;i<3;i++)Need(m.worlds[i]!=null&&m.worlds[i].room==i&&m.worlds[i].id==worlds[i]&&m.stages[i]!=null&&m.stages[i].id==Route+"-"+i&&m.stages[i].sourceWorldId==worlds[i]&&(m.stages[i].spawns==null||m.stages[i].spawns.Length==0)&&m.music[i]!=null&&m.music[i].room==i&&m.music[i].id==m.stages[i].bgm&&m.music[i].id==(i<2?"factory-private-original-stage-r742":"factory-private-original-boss-r742"),"original world/BGM/stage order changed");
   var npcs=new HashSet<string>(StringComparer.Ordinal);int keys=0,states=0,reference=0,makerPixel=0,guardPixel=0;foreach(var b in m.banks){Need(b!=null&&npcs.Add(b.npc)&&b.document!=null&&b.keys!=null&&b.keys.Length>0&&!b.artApproved,"bank identity/qualification changed");keys+=b.keys.Length;states+=b.stateCount;if(b.originalReference)reference++;if(b.npc=="NUI_NASOD_MAKER"){Need(!b.originalReference&&b.id==MakerPixelIdR1223&&b.kind=="nasod-maker"&&b.document.sha256==MakerPixelDocumentSha256R1223&&b.keys.Length==5&&b.stateCount==6&&b.motionCount==2&&b.sourcePoseSamples==8,"exact Maker finite pixel bank changed");makerPixel++;}if(b.npc=="NUI_NASOD_GUARD"){Need(!b.originalReference&&b.id==GuardPixelIdR1264&&b.kind=="nasod-guard"&&b.document.sha256==GuardPixelDocumentSha256R1264&&b.keys.Length==40&&b.stateCount==27&&b.motionCount==20&&b.sourcePoseSamples==77,"exact Guard finite pixel bank changed");guardPixel++;}var ks=new HashSet<string>(StringComparer.Ordinal);foreach(var k in b.keys)Need(k!=null&&ks.Add(k.key)&&k.file!=null&&k.width>0&&k.height>0&&BelowPlayerResourceIndexV21.IsHash(k.rgbaSha256)&&Array.Exists(m.files,f=>Identity(f.sourcePath)==Identity(k.file.path)&&f.sha256==k.file.sha256&&f.assetType=="TextAsset"),"key binding changed");}Need(keys==418&&states==185&&reference==7&&makerPixel==1&&guardPixel==1,"eleven banks require seven original references and finite Maker/Guard pixel banks");
   var sounds=new HashSet<string>(StringComparer.Ordinal);foreach(var a in m.audio)Need(a!=null&&sounds.Add(a.id)&&a.file!=null&&Array.Exists(m.files,f=>Identity(f.sourcePath)==Identity(a.file.path)&&f.sha256==a.file.sha256&&f.assetType=="AudioClip"),"sound ID/file binding changed");
  }
  FactoryProductResourceR1168 Resolve(string path,string sha,string type){FactoryProductResourceR1168 r;Need(files.TryGetValue(Identity(path),out r)&&r.sha256==sha&&r.assetType==type,"unmapped exact "+type+" identity "+path);return r;}
  public byte[] Read(FactoryProductFileR1168 p){Need(p!=null,"file binding absent");return Read(p.path,p.sha256);}
  public byte[] Read(string path,string sha){var r=Resolve(path,sha,"TextAsset");var b=readText(r.resource);Need(b!=null&&b.Length<=64*1024*1024&&(!r.sizeKnown||b.Length==r.bytes)&&BelowPlayerResourceIndexV21.Hash(b)==r.sha256,"raw resource missing/changed "+r.resource);return b;}
  public T Json<T>(FactoryProductFileR1168 p){return FactoryTypedJsonR581.ReadProductRootR1213<T>(Read(p));}
  public byte[] RootBytes(string id){FactoryProductRootR1168 r;Need(roots.TryGetValue(id,out r),"runtime root absent "+id);return Read(r.path,r.sha256);}
  public T RootJson<T>(string id){return FactoryTypedJsonR581.ReadProductRootR1213<T>(RootBytes(id));}
  public AudioClip Audio(FactoryProductAudioR1168 a){Need(a!=null&&a.file!=null,"audio record absent");var r=Resolve(a.file.path,a.file.sha256,"AudioClip");var clip=Resources.Load<AudioClip>(r.resource);Need(clip&&clip.loadState!=AudioDataLoadState.Failed,"imported original OGG absent "+a.id);return clip;}
  public void RequireCurrentCatalog(byte[] bytes,ArtCatalog current){RequireCurrentCatalog(new CatalogDigest(bytes),current);}
  internal void RequireCurrentCatalog(CatalogDigest digest,ArtCatalog current){Need(digest.Matches(41191929,CatalogSha256),"Main catalog changed");Need(current!=null&&current.dungeons!=null&&current.dungeons.Length==12&&current.stages!=null&&current.stages.Length==35&&current.npcContentSets!=null&&current.npcContentSets.Length==8,"Main baseline counts changed");}
 }
}
