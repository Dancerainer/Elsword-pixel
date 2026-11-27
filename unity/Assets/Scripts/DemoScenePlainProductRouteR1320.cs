using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool plainProductMainBaselineR1320;
  readonly Dictionary<string,CandidateDungeonArt> plainProductRowsR1320=new Dictionary<string,CandidateDungeonArt>(StringComparer.Ordinal);
  readonly HashSet<string> plainProductInstalledR1320=new HashSet<string>(StringComparer.Ordinal);
  string plainProductMenuMessageR1320="";
  void CapturePlainProductMainBaselineR1320(CatalogDigest digest,ArtCatalog original){
   plainProductMainBaselineR1320=digest.Matches(41191929,PlainProductResourceIndexR1320.CatalogSHA)&&original!=null&&original.dungeons!=null&&original.dungeons.Length==12&&original.stages!=null&&original.stages.Length==35&&original.npcContentSets!=null&&original.npcContentSets.Length==8;
  }
  void RequirePlainProductMainBaselineR1320(PlainProductResourceIndexR1320 index){PlainProductNeedR1320(index!=null&&plainProductMainBaselineR1320&&CurrentCatalogDigest().Matches(41191929,PlainProductResourceIndexR1320.CatalogSHA),"original Main Resources baseline unavailable");}
  bool IsPlainProductMapRowR1320(CandidateDungeonArt row){CandidateDungeonArt original;return row!=null&&!string.IsNullOrEmpty(row.sourceGraph)&&plainProductRowsR1320.TryGetValue(row.sourceGraph,out original)&&ReferenceEquals(row,original)&&row.id==PlainProductResourceIndexR1320.RouteId(row.sourceGraph)&&row.region=="altera";}
  void ValidatePlainProductInstalledR1320(PlainProductResourceIndexR1320 index){
   PlainProductNeedR1320(Resources.Load<Shader>("Pixel/Shaders/NativeWorld")&&Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25")&&Shader.Find("Sprites/Default"),"original Main native/screen/sprite shader absent");
   var keys=new HashSet<string>(StringComparer.Ordinal);foreach(var f in index.Map.files)if(f.assetType=="TextAsset"&&keys.Add(f.resource))index.Read(new Mine2ProductFileR1211{path=f.sourcePath,sha256=f.sha256});
   foreach(var a in index.Map.audio)index.Audio(a);foreach(var m in index.Map.music)index.Music(m);
   if(index.Map.sourceDungeon==PlainProductResourceIndexR1320.Recycle)PlainProductNeedR1320(Resources.Load<Shader>("Pixel/Shaders/PlainMineSourceParticleR1302"),"exact finite Mine shader absent");
  }
  void AppendPlainProductMapRoutesR1320(string externalCandidatePath,string selectedRoute=null){
   if(selectedRoute==null){plainProductRowsR1320.Clear();plainProductInstalledR1320.Clear();}
   else{plainProductRowsR1320.Remove(selectedRoute);plainProductInstalledR1320.Remove(selectedRoute);}
   plainProductMenuMessageR1320="";if(!string.IsNullOrEmpty(externalCandidatePath))return;
   foreach(string route in new[]{PlainProductResourceIndexR1320.Normal,PlainProductResourceIndexR1320.Recycle})try{
    if(selectedRoute!=null&&route!=selectedRoute)continue;
    string id=PlainProductResourceIndexR1320.RouteId(route);
    foreach(var old in catalog.dungeons)PlainProductNeedR1320(old.id!=id&&old.sourceGraph!=route,"conflicting pre-existing Plain catalog route");
    foreach(var old in mapDungeons)PlainProductNeedR1320(old.id!=id&&old.sourceGraph!=route,"conflicting pre-existing Plain menu route");
    var document=PlainProductResourceIndexR1320.ReadMap(route);if(document==null)continue;var index=new PlainProductResourceIndexR1320(document,route);RequirePlainProductMainBaselineR1320(index);var map=index.Map;
    var model=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductModelR1211>(index.RootBytes("model"));PlainProductNeedR1320(model!=null&&model.rooms!=null&&model.rooms.Length==3,"original Plain three rooms absent");
    var rooms=new CandidateRoomArt[3];int zones=0;for(int i=0;i<3;i++){var r=model.rooms[i];zones+=r.encounters.Length;rooms[i]=new CandidateRoomArt{id=r.sourceWorldId,width=r.width,zones=r.encounters.Length,spawnCount=r.enemies.Length,bgm=map.stages[i].bgm,bgmAvailable=false};}
    PlainProductNeedR1320(zones==6,"original Plain six encounter nodes changed");
    try{CheckMenuProductIndex(index,()=>ValidatePlainProductInstalledR1320(index));plainProductInstalledR1320.Add(route);}catch(Exception e){Debug.LogWarning("Plain Resources unavailable: "+e.Message);}
    var row=new CandidateDungeonArt{id=id,title=map.routeTitle,description="Explore the Altera plains.",region="altera",sourceGraph=route,count=3,combatZones=6,runtimeIndex=-1,runtimeAdmitted=false,rooms=rooms};plainProductRowsR1320.Add(route,row);mapDungeons.Add(row);
   }catch(Exception e){Debug.LogException(e);}
  }
  bool TryEnterPlainProductMapRouteR1320(){
   var row=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;if(mapRegion!="altera"||!IsPlainProductMapRowR1320(row))return false;
   if(plainProductInstalledR1320.Contains(row.sourceGraph)&&!plainProductSessionR1320)try{BeginPlainProductR1320(row.sourceGraph);}catch(Exception e){plainProductMenuMessageR1320="CONTENT COULD NOT BE LOADED";Debug.LogException(e);RenderMenus();}return true;
  }
  void RenderPlainProductMapSelectionR1320(){
   if(openMenu!="dungeons")return;var selected=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;
   for(int i=0;i<MapPageSize;i++){int at=mapPage*MapPageSize+i;if(at>=mapVisible.Count)continue;var row=mapDungeons[mapVisible[at]];if(IsPlainProductMapRowR1320(row))TextValue("Dungeon Boss "+i,plainProductSessionR1320?"LOADING...":plainProductInstalledR1320.Contains(row.sourceGraph)?"READY TO ENTER":"CONTENT NOT INSTALLED");}
   if(mapRegion!="altera"||!IsPlainProductMapRowR1320(selected))return;bool ready=plainProductInstalledR1320.Contains(selected.sourceGraph)&&!plainProductSessionR1320;
   TextValue("Dungeon Enter Text",plainProductSessionR1320?"LOADING...":ready?"ENTER DUNGEON":"CONTENT NOT INSTALLED");images["Dungeon Enter"].color=C(ready?0x476b61u:0x293944u);TextValue("Dungeon Status",plainProductSessionR1320?"LOADING...":!string.IsNullOrEmpty(plainProductMenuMessageR1320)?plainProductMenuMessageR1320:ready?"READY TO ENTER":"CONTENT NOT INSTALLED");TextValue("Dungeon Route","AREA 1\n\nAREA 2\n\nAREA 3");for(int i=0;i<3;i++)Visible("Dungeon BGM "+i,false);
  }
 }
}
