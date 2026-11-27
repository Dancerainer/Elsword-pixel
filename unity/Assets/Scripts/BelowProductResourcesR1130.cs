using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class BelowProductRoomR1130 {
  public string id,world,title,bgm;public int zones,spawns;
 }
 [Serializable] public sealed class BelowProductResourceMapR1130 {
  public string schema,sourceDungeon,currentCatalogSha256,sourceManifestSha256,historicalCatalogSha256,sourceBundleSha256;
  public bool dataOnly,runtimeReleaseAccepted,pixelArtAccepted,naturalRouteAccepted,nativeEquivalent;
  public int currentCatalogBytes,baselineRoutes,baselineStages,baselineContentSets,appendStart,roomCount,births,profiles,states,effectCount,audioCount,visibleLibraries,runtimeAliasCount,uniqueResourceCount;
  public long uniqueResourceBytes;
  public string routeId,routeTitle,routeRegion,routeDescription;
  public BelowProductRoomR1130[] rooms;public BelowPlayerRootsV21 roots;public BelowPlayerResourceRowV21[] files;
 }
 // A content-addressed runtime data map, not an art/release/Root permission receipt.
 public static class BelowProductResourcesR1130 {
  public const string Resource="Pixel/dungeons/altera-below-tunnel/route-v1130";
  public const string Prefix="Pixel/dungeons/altera-below-tunnel/data/";
  public const string MappingSha256="e706bc9f8c1b897c65f70dee1f6b55fb7e530471e224210613090779ee79e4a1";
  // The historical source bundle remains pinned separately from restored runtime geometry.
  public const string CurrentBundleSha256="c10a149153b4e32e6b67b9a8431a449e2b6306893de62278f7a22328896eabbd";
  public const string CurrentCatalogSha256="05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3";
  static readonly HashSet<string> RuntimeRoles=new HashSet<string>(StringComparer.Ordinal){"exact-whole-atlas","exact-whole-atlas;physical-display-policy-atlas","exact-whole-library","exact-whole-library;physical-display-policy-animations","original-decoded-BGM-PCM16","original-decoded-NPC-PCM16","root-bundle","root-effectPresentationBinding","root-effects","root-mechanisms","root-model;source-preservation-activationModel","root-recoveryLookupPolicy","root-rules","root-soundMap","root-sounds","root-unboundPhysicalReactionPolicy","typed-FX-Point-texture","typed-FX-document","typed-FX-meshManifest","typed-FX-sourceEffects","typed-FX-sourceRules","world-Point-texture","world-binary","world-projection","world-scene"};
  static readonly string[] Worlds={"WI_ALTERA_BELOW_TUNNEL_STAGE1","WI_ALTERA_BELOW_TUNNEL_STAGE2A","WI_ALTERA_BELOW_TUNNEL_STAGE4"};
  static void Need(bool ok,string why){BelowPlayerResourceIndexV21.Need(ok,"Below route data: "+why);}
  static bool IsAbsentFile(BelowPlayerFileV21 f){return f==null||(string.IsNullOrEmpty(f.path)&&string.IsNullOrEmpty(f.sha256));}
  public static BelowProductResourceMapR1130 Read(){
   var asset=Resources.Load<TextAsset>(Resource);if(!asset)return null;
   var bytes=asset.bytes;Need(bytes.Length<=256*1024&&BelowPlayerResourceIndexV21.Hash(bytes)==MappingSha256,"resource mapping identity changed");
   var map=JsonUtility.FromJson<BelowProductResourceMapR1130>(Encoding.UTF8.GetString(bytes));Validate(map);return map;
  }
  internal static void Validate(BelowProductResourceMapR1130 m){
   Need(m!=null&&m.schema=="below-normal-product-resource-data-v1130"&&m.sourceDungeon==BelowPlayerResourceIndexV21.SourceDungeon,"wrong mapping scope");
   Need(m.dataOnly&&!m.runtimeReleaseAccepted&&!m.pixelArtAccepted&&!m.naturalRouteAccepted&&!m.nativeEquivalent,"data binding cannot promote historical qualifications");
   Need(m.currentCatalogSha256==CurrentCatalogSha256&&m.currentCatalogBytes==41191929&&m.sourceManifestSha256==BelowPlayerBootstrapV21.ManifestSha256&&m.historicalCatalogSha256==BelowPlayerResourceIndexV21.BaselineCatalog&&m.sourceBundleSha256==BelowPlayerResourceIndexV21.ApprovedBundle,"current baseline or historical data identity changed");
   Need(m.baselineRoutes==12&&m.baselineStages==35&&m.baselineContentSets==8&&m.appendStart==35&&m.roomCount==3&&m.births==78&&m.profiles==24&&m.states==279&&m.effectCount==11&&m.audioCount==67&&m.visibleLibraries==21,"original source data scope changed");
   Need(m.routeId=="altera-below-tunnel"&&m.routeRegion=="altera"&&!string.IsNullOrEmpty(m.routeTitle)&&m.rooms!=null&&m.rooms.Length==3,"route identity changed");
   for(int i=0;i<3;i++)Need(m.rooms[i]!=null&&m.rooms[i].id==m.sourceDungeon+"-"+i&&m.rooms[i].world==Worlds[i]&&m.rooms[i].zones==new[]{2,3,1}[i]&&m.rooms[i].spawns==new[]{35,36,7}[i]&&m.rooms[i].bgm==(i==2?"music015_boss":"music023_stage"),"room identity/order changed");
   Need(m.files!=null&&m.files.Length==201&&m.runtimeAliasCount==201&&m.uniqueResourceCount==194&&m.uniqueResourceBytes==74925692,"runtime-only resource counts changed");
   Need(m.roots!=null&&IsAbsentFile(m.roots.artClockProof)&&IsAbsentFile(m.roots.sourcePreservation)&&IsAbsentFile(m.roots.visibility),"audit-only documents cannot become product assets");
   var paths=new HashSet<string>(StringComparer.Ordinal);var unique=new Dictionary<string,int>(StringComparer.Ordinal);long total=0;
   foreach(var r in m.files){
    Need(r!=null&&!string.IsNullOrEmpty(r.path)&&paths.Add(r.path)&&BelowPlayerResourceIndexV21.IsHash(r.sha256)&&r.resource==Prefix+r.sha256&&r.bytes>=0&&r.bytes<=64*1024*1024&&RuntimeRoles.Contains(r.role),"invalid runtime resource row");
    int size;if(unique.TryGetValue(r.resource,out size))Need(size==r.bytes,"conflicting duplicate resource");else{unique.Add(r.resource,r.bytes);total+=r.bytes;}
   }
   Need(unique.Count==194&&total==m.uniqueResourceBytes,"runtime resource deduplication changed");
   foreach(var p in new[]{m.roots.bundle,m.roots.model,m.roots.rules,m.roots.effects,m.roots.sounds,m.roots.mechanisms,m.roots.soundMap,m.roots.unboundPhysicalReactionPolicy,m.roots.recoveryLookupPolicy,m.roots.effectPresentationBinding}){
    Need(p!=null&&Array.Exists(m.files,r=>r.path==p.path&&r.sha256==p.sha256),"unmapped runtime root");
   }
   Need(m.roots.bundle.sha256==CurrentBundleSha256,"current runtime bundle identity changed");
  }
  internal static void RequireCurrentCatalog(BelowProductResourceMapR1130 m,byte[] bytes,ArtCatalog catalog){RequireCurrentCatalog(m,new CatalogDigest(bytes),catalog);}
  internal static void RequireCurrentCatalog(BelowProductResourceMapR1130 m,CatalogDigest digest,ArtCatalog catalog){
   Validate(m);Need(digest.Matches(m.currentCatalogBytes,CurrentCatalogSha256),"current Main catalog changed");
   Need(catalog!=null&&catalog.dungeons!=null&&catalog.stages!=null&&catalog.npcContentSets!=null&&catalog.dungeons.Length==12&&catalog.stages.Length==35&&catalog.npcContentSets.Length==8,"current Main catalog slots changed");
   foreach(var d in catalog.dungeons)Need(d!=null&&d.sourceGraph!=m.sourceDungeon&&d.id!=m.routeId,"Below already exists in current catalog");
  }
  // Availability only; the real entry still checks every exact byte hash through the data index.
  internal static bool RuntimeAssetsPresent(BelowProductResourceMapR1130 m){
   Validate(m);var seen=new HashSet<string>(StringComparer.Ordinal);
   foreach(var r in m.files)if(seen.Add(r.resource)&&!Resources.Load<TextAsset>(r.resource))return false;
   return true;
  }
  internal static ArtCatalog AppendCurrentCatalog(ArtCatalog original,BelowPlayerDataPlanV21 plan,BelowProductResourceMapR1130 map){
   var result=BelowPlayerCatalogProjectionV21.AppendCandidate(original,plan);
   // This is a new data-binding record. The old manifest and its false approval fields are untouched.
   result.npcContentSets[8].admission=new IterationAdmissionData{schema="below-normal-product-data-binding-v1130",sourceDungeon=map.sourceDungeon,bundleSha256=map.roots.bundle.sha256,reviewed=false,residualNotes=new[]{"Runtime data mapping only; historical art, native, natural-route and release qualifications are unchanged."}};
   Need(ReferenceEquals(result.hero,original.hero)&&ReferenceEquals(result.monsters,original.monsters)&&ReferenceEquals(result.effects,original.effects)&&ReferenceEquals(result.icons,original.icons)&&ReferenceEquals(result.npcRules,original.npcRules)&&ReferenceEquals(result.npcEffectRules,original.npcEffectRules)&&ReferenceEquals(result.npcSoundRules,original.npcSoundRules)&&ReferenceEquals(result.npcMechanismRules,original.npcMechanismRules)&&ReferenceEquals(result.npcSounds,original.npcSounds)&&ReferenceEquals(result.iterationAdmission,original.iterationAdmission),"baseline catalog data references changed");
   for(int i=0;i<12;i++)Need(ReferenceEquals(result.dungeons[i],original.dungeons[i]),"baseline route changed");
   for(int i=0;i<35;i++)Need(ReferenceEquals(result.stages[i],original.stages[i]),"baseline stage changed");
   for(int i=0;i<8;i++)Need(ReferenceEquals(result.npcContentSets[i],original.npcContentSets[i]),"baseline content set changed");
   return result;
  }
 }
}
