using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  ArtCatalog mine2ProductMainBaselineR1233;Mine2ProductMapR1211 mine2ProductMapR1233;CandidateDungeonArt mine2ProductMapRowR1233;bool mine2ProductAssetsPresentR1233;string mine2ProductMenuMessageR1233="";
  bool mine2ProductOriginalShape;
  void CaptureMine2ProductMainBaselineR1233(CatalogDigest digest,ArtCatalog original){
   // Android keeps the verified shape instead of retaining the entire original DTO graph.
   // An unavailable/different baseline only disables this optional product route.
   mine2ProductOriginalShape=digest.Matches(41191929,Mine2ProductResourceIndexR1211.CatalogSha256)&&Mine2ProductResourceIndexR1211.OriginalShape(original);
   mine2ProductMainBaselineR1233=mine2ProductOriginalShape&&!StreamMobileArt?original:null;
  }
  void RequireMine2ProductMainBaselineR1233(Mine2ProductResourceIndexR1211 index){
   bool original=StreamMobileArt?mine2ProductOriginalShape:Mine2ProductResourceIndexR1211.OriginalShape(mine2ProductMainBaselineR1233);
   Mine2ProductNeedR1233(original,"original Main Resources catalog baseline unavailable");index.RequireCurrentCatalog(CurrentCatalogDigest(),original);
  }
  bool IsMine2ProductMapRowR1233(CandidateDungeonArt row){return mine2ProductMapRowR1233!=null&&ReferenceEquals(row,mine2ProductMapRowR1233)&&row.id=="bethma-richie-mine"&&row.region=="bethma"&&row.sourceGraph==Mine2ProductResourceIndexR1211.Route;}
  void ValidateMine2ProductInstalledR1233(Mine2ProductResourceIndexR1211 index){
   Mine2ProductNeedR1233(Resources.Load<Shader>("Pixel/Shaders/NativeWorld")&&Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25")&&Shader.Find("Sprites/Default"),"Main world/screen/sprite shader absent");
   var seen=new HashSet<string>(StringComparer.Ordinal);foreach(var f in index.Map.files)if(f.assetType=="TextAsset"&&seen.Add(f.resource))index.Read(new Mine2ProductFileR1211{path=f.sourcePath,sha256=f.sha256});
   foreach(var a in index.Map.audio)index.Audio(a);foreach(var m in index.Map.music)index.Music(m);
  }
  void AppendMine2ProductMapRouteR1233(string externalCandidatePath){
   mine2ProductMapR1233=null;mine2ProductMapRowR1233=null;mine2ProductAssetsPresentR1233=false;mine2ProductMenuMessageR1233="";if(!string.IsNullOrEmpty(externalCandidatePath))return;
   try{
    foreach(var d in catalog.dungeons)if(d.id=="bethma-richie-mine"||d.sourceGraph==Mine2ProductResourceIndexR1211.Route){Mine2ProductNeedR1233(d.id=="bethma-richie-mine"&&d.sourceGraph==Mine2ProductResourceIndexR1211.Route&&d.count==3,"conflicting existing Mine2 route identity");return;}
    foreach(var d in mapDungeons)if(d.id=="bethma-richie-mine"||d.sourceGraph==Mine2ProductResourceIndexR1211.Route){Mine2ProductNeedR1233(d.id=="bethma-richie-mine"&&d.sourceGraph==Mine2ProductResourceIndexR1211.Route&&d.count==3,"conflicting Mine2 menu identity");return;}
    var map=Mine2ProductResourceIndexR1211.ReadMap();if(map==null)return;var index=new Mine2ProductResourceIndexR1211(map);RequireMine2ProductMainBaselineR1233(index);var model=index.RootJson<Mine2ProductModelR1211>("model");Mine2ProductNeedR1233(model!=null&&model.rooms!=null&&model.rooms.Length==3,"original Mine2 route rooms absent");
    var rooms=new CandidateRoomArt[3];int zones=0;for(int i=0;i<3;i++){var r=model.rooms[i];var s=map.stages[i];int z=r.encounters==null?0:r.encounters.Length;zones+=z;rooms[i]=new CandidateRoomArt{id=s.sourceWorldId,width=s.width,zones=z,spawnCount=new[]{30,35,3}[i],bgm=s.bgm,bgmAvailable=false};}
    Mine2ProductNeedR1233(zones==5,"original Mine2 five encounter zones changed");mine2ProductMapR1233=map;try{CheckMenuProductIndex(index,()=>ValidateMine2ProductInstalledR1233(index));mine2ProductAssetsPresentR1233=true;}catch(Exception e){mine2ProductMenuMessageR1233="CONTENT NOT INSTALLED";Debug.LogWarning("Mine2 Resources unavailable: "+e.Message);}
    mine2ProductMapRowR1233=new CandidateDungeonArt{id="bethma-richie-mine",title="RICHIE MINE",description="Explore Richie Mine.",region="bethma",sourceGraph=Mine2ProductResourceIndexR1211.Route,count=3,combatZones=5,runtimeIndex=-1,runtimeAdmitted=false,rooms=rooms};mapDungeons.Add(mine2ProductMapRowR1233);
   }catch(Exception e){mine2ProductMapR1233=null;mine2ProductMapRowR1233=null;mine2ProductAssetsPresentR1233=false;mine2ProductMenuMessageR1233="CONTENT UNAVAILABLE";Debug.LogException(e);}
  }
  bool TryEnterMine2ProductMapRouteR1233(){
   var row=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;if(mapRegion!="bethma"||!IsMine2ProductMapRowR1233(row))return false;
   if(mine2ProductAssetsPresentR1233&&!mine2ProductSessionR1233){try{BeginMine2ProductR1233();}catch(Exception e){mine2ProductMenuMessageR1233="CONTENT COULD NOT BE LOADED";Debug.LogException(e);RenderMenus();}}return true;
  }
  void RenderMine2ProductMapSelectionR1233(){
   if(openMenu!="dungeons")return;var selected=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;
   for(int i=0;i<MapPageSize;i++){int at=mapPage*MapPageSize+i;if(at>=mapVisible.Count)continue;var row=mapDungeons[mapVisible[at]];if(IsMine2ProductMapRowR1233(row))TextValue("Dungeon Boss "+i,mine2ProductSessionR1233?"LOADING...":mine2ProductAssetsPresentR1233?"READY TO ENTER":"CONTENT NOT INSTALLED");}
   if(!IsMine2ProductMapRowR1233(selected)||mapRegion!="bethma")return;bool ready=mine2ProductAssetsPresentR1233&&!mine2ProductSessionR1233;TextValue("Dungeon Enter Text",mine2ProductSessionR1233?"LOADING...":ready?"ENTER DUNGEON":"CONTENT NOT INSTALLED");images["Dungeon Enter"].color=C(ready?0x476b61u:0x293944u);TextValue("Dungeon Status",mine2ProductSessionR1233?"LOADING...":string.IsNullOrEmpty(mine2ProductMenuMessageR1233)?(ready?"READY TO ENTER":"CONTENT NOT INSTALLED"):mine2ProductMenuMessageR1233);TextValue("Dungeon Route","AREA 1\n\nAREA 2\n\nAREA 3");for(int i=0;i<3;i++)Visible("Dungeon BGM "+i,false);
  }
 }
}
