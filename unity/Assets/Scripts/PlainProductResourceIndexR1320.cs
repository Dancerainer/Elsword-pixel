using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class PlainProductMineTextureR1320 {
  public string name,originalMemberSha256,unityBottomUpRgbaSha256;public Mine2ProductFileR1211 file;
 }
 [Serializable] public sealed class PlainProductMineMediaR1320 {
  public PlainProductMineTextureR1320[] textures;public Mine2ProductAudioR1211[] sounds;public Mine2ProductFileR1211 meshes;
 }
 [Serializable] public sealed class PlainProductMapR1320 {
  public string schema;public Mine2ProductMapR1211 content;public PlainPagedLibraryBindingR1330[] pagedLibraryRows=new PlainPagedLibraryBindingR1330[0];public Mine2ProductFileR1211 callbackPoints;public PlainVariablePhaseResourceR1330[] phaseRows=new PlainVariablePhaseResourceR1330[0];public PlainWatchWholeResourceR1330[] watchWholeRows=new PlainWatchWholeResourceR1330[0];public PlainProductMineMediaR1320[] mineRows=new PlainProductMineMediaR1320[0];
 }
 // The existing Mine2-named DTOs below are only the shared source JSON shapes.
 // No Mine2 model/presentation/AI adapter is configured by this resource reader.
 public sealed class PlainProductResourceIndexR1320 {
  public const string Normal="Dungeon_Altera_Plain_Normal",Recycle="Dungeon_Altera_Plain_Recycle_Normal";
  public const string RulesSHA="159ef67868b5c07a977afbf5d25ac497274d727c0c4fad5bd8f4325c20f63035";
  public const string CatalogSHA="05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3";
  readonly Dictionary<string,Mine2ProductResourceR1211> files=new Dictionary<string,Mine2ProductResourceR1211>(StringComparer.OrdinalIgnoreCase);
  readonly Dictionary<string,Mine2ProductRootR1211> roots=new Dictionary<string,Mine2ProductRootR1211>(StringComparer.Ordinal);
  public PlainProductMapR1320 Document {get;private set;}
  public Mine2ProductMapR1211 Map {get{return Document.content;}}
  public static void Need(bool ok,string why){if(!ok)throw new InvalidOperationException("Plain product: "+why);}
  static string Identity(string path){return(path??"").Replace('\\','/');}
  public static string RouteId(string route){return route==Normal?"altera-plain":route==Recycle?"altera-plain-recycle":null;}
  public static PlainProductMapR1320 ReadMap(string route){
   string id=RouteId(route);Need(id!=null,"unknown original route");var raw=Resources.Load<TextAsset>("Pixel/dungeons/"+id+"/route-r1320");if(!raw)return null;
   Need(raw.bytes.Length<=2*1024*1024,"resource map too large");
   var doc=JsonUtility.FromJson<PlainProductMapR1320>(new UTF8Encoding(false,true).GetString(raw.bytes));Validate(doc,route);return doc;
  }
  public PlainProductResourceIndexR1320(PlainProductMapR1320 doc,string route){
   Validate(doc,route);Document=doc;foreach(var f in Map.files)files.Add(Identity(f.sourcePath),f);foreach(var r in Map.roots)roots.Add(r.id,r);
  }
  static string ExpectedRootSHA(string route,string id){bool normal=route==Normal;switch(id){
   case "model":return normal?"3cfb2dd11f3dff86733f3a446a47aca254091cdb0cf75baa67a16f57b0313c2c":"f6dbe53114e6b0d1648fcad61e339c6103edbaa472a4d53ca9e9c0d69ee54f39";
   case "rules":return normal?"cb966e14c1728297464f2d05d1956ac52d8c0b3d428cb5d250a757e7a17f4c65":"07a56020b7e39258ff165a01ed5e547876aa1607bc9abbc2af3ce6b1e641c6b3";
   case "effects":return normal?"dabba8771c47d85575e62adfd4f62b7d607f8a1ee03aaa9c17c483f4d1d3e556":"54210a2a705caf69ad76a11107e24126c52e2418951d51c8cfbf861cc54b31b5";
   case "sounds":return normal?"1eae9626f2bb7a1524d5b904e0a0c86dcdbcba62f598efc6889d16d1aa014b45":"af9b5f6c98541c334c16565f3a402aa41331f2f174dcb6504e0c30cb0225f4b9";
   case "runtimeMechanisms":return normal?"2d67bc86c99d0cc983ddbc3e17442c7c1bd43291b62d1cecc49841a99bef566c":"5ad0d7644e614d81d0e2c5ac80e61418ea3ceeb680decbb89530049a837ac7b2";
   default:return null;
  }}
  static void Validate(PlainProductMapR1320 doc,string route){
   var m=doc==null?null:doc.content;bool normal=route==Normal;
   Need(doc!=null&&doc.schema=="plain-normal-product-resources-r1320"&&m!=null&&RouteId(route)!=null&&m.sourceDungeon==route&&m.routeId==RouteId(route)&&m.routeRegion=="altera","exact original Plain map required");
   Need(doc.callbackPoints!=null&&doc.callbackPoints.sha256=="0bf015c997394a9645375a719d196482cbb4de8121c934779c35afb22c6ec4f1","exact original callback point resource required");
   Need(m.dataOnly&&!m.runtimeReleaseAccepted&&!m.pixelArtAccepted&&!m.naturalRouteAccepted&&!m.nativeEquivalent&&!m.normalEntryActuallyVerified,"content metadata cannot grant route/art/native acceptance");
   Need(m.currentCatalogSha256==CatalogSHA&&m.currentCatalogBytes==41191929&&m.roomsCount==3&&m.births==(normal?92:95)&&m.profiles==(normal?15:10)&&m.states==(normal?284:187)&&m.zones==6,"original route/Main cohort changed");
   Need(m.roots!=null&&m.roots.Length==5&&m.worlds!=null&&m.worlds.Length==3&&m.stages!=null&&m.stages.Length==3&&m.libraries!=null&&m.libraries.Length+(doc.pagedLibraryRows==null?0:doc.pagedLibraryRows.Length)==m.profiles-1-PlainVariablePhaseDataR1330.RouteCount(route)&&m.audio!=null&&m.music!=null&&m.music.Length==3&&m.files!=null,"typed content arrays missing");
   int paged=doc.pagedLibraryRows==null?0:doc.pagedLibraryRows.Length;Need(paged==0||(!normal&&paged==1&&doc.pagedLibraryRows[0]!=null&&doc.pagedLibraryRows[0].id==PlainPagedLibraryDataR1330.Kind&&doc.pagedLibraryRows[0].npc==PlainPagedLibraryDataR1330.Npc&&doc.pagedLibraryRows[0].document!=null&&BelowPlayerResourceIndexV21.IsHash(doc.pagedLibraryRows[0].document.sha256)&&!Array.Exists(m.libraries,l=>l!=null&&l.npc==PlainPagedLibraryDataR1330.Npc)),"paged library is zero-or-one exact Recycle Driller, without duplicate generic profile");
   var paths=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var keys=new Dictionary<string,Mine2ProductResourceR1211>(StringComparer.Ordinal);string prefix="Pixel/dungeons/"+m.routeId+"/";
   foreach(var f in m.files){
    Need(f!=null&&!string.IsNullOrEmpty(f.sourcePath)&&paths.Add(Identity(f.sourcePath))&&BelowPlayerResourceIndexV21.IsHash(f.sha256)&&f.bytes>0&&f.bytes<=96*1024*1024,"invalid original content identity");
    Need(f.assetType=="TextAsset"||f.assetType=="AudioClip","unsupported runtime resource type");
    if(!f.sharedPublished)Need(f.resource==prefix+(f.assetType=="TextAsset"?"data/":"audio/")+f.sha256,"resource key must bind exact original content hash");
    else Need(f.assetType=="AudioClip"&&f.resource!=null&&f.resource.StartsWith("Pixel/music/",StringComparison.Ordinal),"only existing Main BGM may be borrowed as published content");
    Mine2ProductResourceR1211 prior;if(keys.TryGetValue(f.resource,out prior))Need(prior.sha256==f.sha256&&prior.bytes==f.bytes&&prior.assetType==f.assetType,"ambiguous resource alias");else keys.Add(f.resource,f);
   }
   var rootIds=new HashSet<string>(StringComparer.Ordinal);foreach(var r in m.roots)Need(r!=null&&rootIds.Add(r.id)&&r.sha256==ExpectedRootSHA(route,r.id)&&Array.Exists(m.files,f=>Identity(f.sourcePath)==Identity(r.path)&&f.sha256==r.sha256&&f.assetType=="TextAsset"),"root must bind one actual typed byte resource");
   string modelSHA=normal?"3cfb2dd11f3dff86733f3a446a47aca254091cdb0cf75baa67a16f57b0313c2c":"f6dbe53114e6b0d1648fcad61e339c6103edbaa472a4d53ca9e9c0d69ee54f39";
   Need(Array.Find(m.roots,r=>r.id=="model").sha256==modelSHA,"original route model bytes changed");
   for(int i=0;i<3;i++){
    string literal=normal?(i<2?"Music024_Stage.ogg":"Music011_Boss.ogg"):(i<2?"Music018_Stage.ogg":"Music013_Boss.ogg");
    var music=m.music[i];var stage=m.stages[i];
    Need(music!=null&&music.room==i&&music.originalLiteral==literal&&!music.originalLiteralMissing&&music.id==literal.Substring(0,literal.Length-4).ToLowerInvariant()&&stage!=null&&stage.bgm==music.id,"original per-room BGM literal changed");
   }
   Need(doc.watchWholeRows!=null&&doc.watchWholeRows.Length==1&&doc.watchWholeRows[0]!=null&&doc.watchWholeRows[0].document!=null&&doc.watchWholeRows[0].document.sha256=="1e18f95034c1f919b987cf80ec445cc976c7d61705142620509b051a8017e8cc","exact original Watch whole document required");
   Need(Array.TrueForAll(m.libraries,l=>l!=null&&l.npc!="NUI_NASOD_WATCH"&&!PlainVariablePhaseDataR1330.IsNpc(l.npc)),"Watch/eleven phase banks must not use a generic atlas");
   Need(doc.phaseRows!=null&&doc.phaseRows.Length==1&&doc.phaseRows[0]!=null&&doc.phaseRows[0].document!=null&&BelowPlayerResourceIndexV21.IsHash(doc.phaseRows[0].document.sha256),"one complete original11 phase document required");
   Need(doc.mineRows!=null&&doc.mineRows.Length==(normal?0:1),"Mine media is an exact zero-or-one scoped record");
   if(!normal){var mine=doc.mineRows[0];Need(mine!=null&&mine.textures!=null&&mine.textures.Length==9&&mine.sounds!=null&&mine.sounds.Length==2&&mine.meshes!=null,"Recycle requires the complete exact Mine media contract");}
  }
  Mine2ProductResourceR1211 Binding(Mine2ProductFileR1211 file,string type){Mine2ProductResourceR1211 row=null;Need(file!=null&&files.TryGetValue(Identity(file.path),out row)&&row.sha256==file.sha256&&row.assetType==type,"unbound source resource");return row;}
  public byte[] Read(Mine2ProductFileR1211 file){var row=Binding(file,"TextAsset");var asset=Resources.Load<TextAsset>(row.resource);Need(asset&&asset.bytes.Length==row.bytes&&BelowPlayerResourceIndexV21.Hash(asset.bytes)==row.sha256,"original resource bytes changed "+row.resource);return asset.bytes;}
  public TextAsset Text(Mine2ProductFileR1211 file){Read(file);return Resources.Load<TextAsset>(Binding(file,"TextAsset").resource);}
  public byte[] RootBytes(string id){Mine2ProductRootR1211 root=null;Need(roots.TryGetValue(id,out root),"missing root "+id);return Read(new Mine2ProductFileR1211{path=root.path,sha256=root.sha256});}
  public T Json<T>(Mine2ProductFileR1211 file){return FactoryTypedJsonR581.ReadMine2ProductRootR1226<T>(Read(file));}
  public AudioClip Audio(Mine2ProductAudioR1211 a){var row=Binding(a.file,"AudioClip");Need(!row.sharedPublished,"NPC PCM is a dedicated borrowed resource");var clip=Resources.Load<AudioClip>(row.resource);Need(clip&&clip.samples==a.samples&&clip.channels==a.channels&&clip.frequency==a.sampleRate&&BelowPlayerResourceIndexV21.IsHash(a.decodedPcm16Sha256),"original decoded PCM metadata changed");return clip;}
  public AudioClip Music(Mine2ProductMusicR1211 m){var row=Binding(m.file,"AudioClip");Need(row.resource==m.resource,"BGM resource binding changed");var clip=Resources.Load<AudioClip>(row.resource);Need(clip,"original BGM absent");return clip;}
 }
}
