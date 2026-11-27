using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class CoreProductFileR1143 {public string path,sha256;}
 [Serializable] public sealed class CoreProductResourceRowR1143 {public string sourcePath,sha256,resource,assetType;public int bytes;public bool sizeKnown;}
 [Serializable] public sealed class CoreProductRootR1143 {public string id,sourcePath,sha256;}
 [Serializable] public sealed class CoreProductTextureR1143 {public string sourcePath;public CoreProductFileR1143 source,selected;public bool singleMaterialTrial;}
 [Serializable] public sealed class CoreProductWorldR1143 {public string id;public int room;public CoreProductFileR1143 scene,projection,binary;public CoreProductTextureR1143[] textures;}
 [Serializable] public sealed class CoreProductActorR1143 {public string npc,kind,sourceInitialState;public bool emptySourceBody;}
 [Serializable] public sealed class CoreProductAudioR1143 {public string id;public CoreProductFileR1143 file;}
 [Serializable] public sealed class CoreProductMusicR1143 {public int room;public string id;}
 [Serializable] public sealed class CoreProductMapR1143 {
  public string schema,sourceDungeon,sourceDifficulty,sourceBundleSha256,currentCatalogSha256,routeId,routeRegion,routeTitle,routeDescription;
  public int difficultyValue,npcLevel,currentCatalogBytes,roomsCount,births,states,nodes,masterApprovedWholeIdentities,masterSourceDiagnosticIdentities,aliasCount,uniqueResources,unknownSizeResources;
  public long knownUniqueBytes;
  public bool dataOnly,masterPromoted,runtimeReleaseAccepted,pixelActorApproval,pixelSceneryApproval,naturalRouteAccepted,nativeEquivalent,viewBindingsComplete;
  public CoreProductRootR1143[] roots;public CoreProductWorldR1143[] worlds;public CoreProductActorR1143[] actors;public CoreProductMusicR1143[] music;public CoreProductAudioR1143[] audio;public CoreProductResourceRowR1143[] files;
 }
 // Original source-path strings are lookup identities only. This class has no filesystem path reader.
 public sealed class CoreProductResourceIndexR1143 {
  public const string MapResource="Pixel/dungeons/altera-core-hard/route-v1147";
  public const string MapSha256="6b3321325d465280253843056beb4c80b4c2a1335acec774fe6d60dc26caf79a";
  public const string CatalogSha256="05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3";
  readonly Dictionary<string,CoreProductResourceRowR1143> files=new Dictionary<string,CoreProductResourceRowR1143>(StringComparer.OrdinalIgnoreCase);
  readonly Dictionary<string,CoreProductRootR1143> roots=new Dictionary<string,CoreProductRootR1143>(StringComparer.Ordinal);
  readonly Func<string,byte[]> readText;
  public CoreProductMapR1143 Map {get;private set;}
  static string Key(string path){var value=(path??"").Replace('\\','/');while(value.Contains("//"))value=value.Replace("//","/");return value;}
  internal static void Need(bool valid,string why){if(!valid)throw new InvalidOperationException("Core Hard product data: "+why);}
  static byte[] ResourceBytes(string resource){var a=Resources.Load<TextAsset>(resource);return a?a.bytes:null;}
  public static CoreProductMapR1143 ReadMap(){
   var raw=ResourceBytes(MapResource);if(raw==null)return null;
   Need(raw.Length<=1024*1024&&BelowPlayerResourceIndexV21.Hash(raw)==MapSha256,"mapping identity changed");
   var m=JsonUtility.FromJson<CoreProductMapR1143>(Encoding.UTF8.GetString(raw));ValidateMap(m);return m;
  }
  public CoreProductResourceIndexR1143(CoreProductMapR1143 map):this(map,ResourceBytes){}
  // The delegate is the same byte-only seam used by the normal Resources reader; it cannot authorize a route.
  internal CoreProductResourceIndexR1143(CoreProductMapR1143 map,Func<string,byte[]> loader){
   ValidateMap(map);Need(loader!=null,"resource reader absent");Map=map;readText=loader;
   foreach(var p in map.files)files.Add(Key(p.sourcePath),p);
   foreach(var p in map.roots)roots.Add(p.id,p);
  }
  public static void ValidateMap(CoreProductMapR1143 m){
   Need(m!=null&&m.schema=="core-hard-normal-product-runtime-data-v1147"&&m.dataOnly&&m.sourceDungeon=="Dungeon_Altera_Core_Hard"&&m.sourceDifficulty=="Hard"&&m.difficultyValue==1&&m.npcLevel==37,"wrong source route/difficulty");
   Need(m.sourceBundleSha256=="4371cb78a67bbbd10d729ba9899cd5937c81a8349e1e929ce8b86de2a3de27b0"&&m.currentCatalogSha256==CatalogSha256&&m.currentCatalogBytes==41191929,"source/current-catalog identity changed");
   Need(m.roomsCount==5&&m.births==85&&m.states==372&&m.nodes==8&&m.masterApprovedWholeIdentities==13&&m.masterSourceDiagnosticIdentities==9&&!m.masterPromoted,"original source scope changed");
   Need(!m.runtimeReleaseAccepted&&!m.pixelActorApproval&&!m.pixelSceneryApproval&&!m.naturalRouteAccepted&&!m.nativeEquivalent&&!m.viewBindingsComplete,"data projection cannot invent view or release qualification");
   Need(m.routeId=="altera-core-hard"&&m.routeRegion=="altera"&&m.routeTitle=="ALTERA CORE / HARD"&&m.worlds!=null&&m.worlds.Length==5&&m.actors!=null&&m.actors.Length==23&&m.music!=null&&m.music.Length==5&&m.audio!=null&&m.audio.Length==93,"incomplete route records");
   Need(m.files!=null&&m.files.Length==1301&&m.aliasCount==1301&&m.uniqueResources==1299&&m.roots!=null&&m.roots.Length==31,"resource scope changed");
   var paths=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var unique=new Dictionary<string,CoreProductResourceRowR1143>(StringComparer.Ordinal);long knownBytes=0;int unknown=0;
   foreach(var p in m.files){
    Need(p!=null&&!string.IsNullOrEmpty(p.sourcePath)&&paths.Add(Key(p.sourcePath))&&BelowPlayerResourceIndexV21.IsHash(p.sha256),"duplicate/invalid source identity");
    Need(p.sizeKnown?p.bytes>=0&&p.bytes<=64*1024*1024:p.bytes==-1,"size must be exact known metadata or explicit unknown -1");
    string expected=p.assetType=="TextAsset"?"Pixel/dungeons/altera-core-hard/data/"+p.sha256:p.assetType=="AudioClip"?"Pixel/dungeons/altera-core-hard/audio/"+p.sha256:p.assetType=="Shader"?"Pixel/Shaders/CoreOriginalSourceV1143":null;
    Need(expected!=null&&p.resource==expected,"invalid typed Resources key");CoreProductResourceRowR1143 prior;
    if(unique.TryGetValue(p.resource,out prior))Need(prior.sha256==p.sha256&&prior.bytes==p.bytes&&prior.assetType==p.assetType,"conflicting deduplicated resource");else{unique.Add(p.resource,p);if(p.sizeKnown)knownBytes+=p.bytes;else unknown++;}
   }
   Need(unique.Count==m.uniqueResources&&knownBytes==m.knownUniqueBytes&&unknown==m.unknownSizeResources,"resource counts changed");
   var rootIds=new HashSet<string>(StringComparer.Ordinal);foreach(var p in m.roots)Need(p!=null&&rootIds.Add(p.id)&&Array.Exists(m.files,f=>Key(f.sourcePath)==Key(p.sourcePath)&&f.sha256==p.sha256&&f.assetType=="TextAsset"),"missing exact runtime data root");
   for(int i=0;i<5;i++)Need(m.worlds[i]!=null&&m.worlds[i].room==i&&!string.IsNullOrEmpty(m.worlds[i].id)&&m.music[i]!=null&&m.music[i].room==i&&m.music[i].id==(i==4?"music025_boss":"music012_stage"),"source room/BGM order changed");
  }
  CoreProductResourceRowR1143 Resolve(string path,string sha,string type){CoreProductResourceRowR1143 row;Need(files.TryGetValue(Key(path),out row)&&row.sha256==sha&&row.assetType==type,"unmapped exact "+type+" identity "+path);return row;}
  public byte[] Read(CoreProductFileR1143 p){Need(p!=null,"missing file binding");return Read(p.path,p.sha256);}
  public byte[] Read(string sourcePath,string expectedSha256){
   var row=Resolve(sourcePath,expectedSha256,"TextAsset");var b=readText(row.resource);
   Need(b!=null&&b.Length<=64*1024*1024&&(!row.sizeKnown||b.Length==row.bytes)&&BelowPlayerResourceIndexV21.Hash(b)==row.sha256,"missing/changed raw resource "+row.resource);return b;
  }
  public T Json<T>(CoreProductFileR1143 p){return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(Read(p)));}
  public T RootJson<T>(string id){CoreProductRootR1143 root;Need(roots.TryGetValue(id,out root),"runtime root absent "+id);return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(Read(root.sourcePath,root.sha256)));}
  public byte[] RootBytes(string id){CoreProductRootR1143 root;Need(roots.TryGetValue(id,out root),"runtime root absent "+id);return Read(root.sourcePath,root.sha256);}
  public AudioClip Audio(CoreProductAudioR1143 a){Need(a!=null&&a.file!=null,"audio binding absent");var row=Resolve(a.file.path,a.file.sha256,"AudioClip");var clip=Resources.Load<AudioClip>(row.resource);Need(clip&&clip.loadState!=AudioDataLoadState.Failed,"original AudioClip resource missing "+a.id);return clip;}
  public Shader SourceShader(){var row=Array.Find(Map.files,p=>p.assetType=="Shader");Need(row!=null,"source shader binding absent");var shader=Resources.Load<Shader>(row.resource);Need(shader,"original source Shader resource missing");return shader;}
  public void RequireCurrentCatalog(byte[] raw,ArtCatalog current){RequireCurrentCatalog(new CatalogDigest(raw),current);}
  internal void RequireCurrentCatalog(CatalogDigest digest,ArtCatalog current){
   Need(digest.Matches(Map.currentCatalogBytes,CatalogSha256),"current Main catalog changed");
   Need(current!=null&&current.dungeons!=null&&current.stages!=null&&current.npcContentSets!=null&&current.dungeons.Length==12&&current.stages.Length==35&&current.npcContentSets.Length==8,"baseline Main catalog counts changed");
  }
 }
}
