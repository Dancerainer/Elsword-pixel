using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 FactoryProductMapR1168 factoryProductMapR1179;CandidateDungeonArt factoryProductMapRowR1179;bool factoryProductAssetsPresentR1179;string factoryProductMenuMessageR1179="";
 bool IsFactoryProductMapRowR1179(CandidateDungeonArt row){return factoryProductMapRowR1179!=null&&ReferenceEquals(row,factoryProductMapRowR1179)&&row.id=="altera-nasod-factory"&&row.region=="altera"&&row.sourceGraph==FactoryProductResourceIndexR1168.Route;}
 void ValidateFactoryProductInstalledR1179(FactoryProductResourceIndexR1168 index){
  FactoryProductNeedR1179(Resources.Load<Shader>("Pixel/Shaders/NativeWorld")&&Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25")&&Shader.Find("Sprites/Default"),"Main world/screen/sprite shader absent");
  var seen=new HashSet<string>(StringComparer.Ordinal);foreach(var row in index.Map.files){if(!seen.Add(row.resource))continue;if(row.assetType=="TextAsset")index.Read(row.sourcePath,row.sha256);else{var clip=index.Audio(new FactoryProductAudioR1168{id=row.sourcePath,file=new FactoryProductFileR1168{path=row.sourcePath,sha256=row.sha256}});FactoryProductNeedR1179(clip,"original imported AudioClip absent");}}
 }
 void AppendFactoryProductMapRouteR1179(string candidatePath){
  factoryProductMapR1179=null;factoryProductMapRowR1179=null;factoryProductAssetsPresentR1179=false;factoryProductMenuMessageR1179="";if(!string.IsNullOrEmpty(candidatePath))return;
  foreach(var d in catalog.dungeons)if(d.id=="altera-nasod-factory"||d.sourceGraph==FactoryProductResourceIndexR1168.Route)return;
  try{
   var map=FactoryProductResourceIndexR1168.ReadMap();if(map==null)return;var index=new FactoryProductResourceIndexR1168(map);index.RequireCurrentCatalog(CurrentCatalogDigest(),catalog);var model=index.RootJson<FactoryProductModelR1168>("sourceModel");FactoryProductNeedR1179(model!=null&&model.rooms!=null&&model.rooms.Length==3,"original route rooms absent");
   var rooms=new CandidateRoomArt[3];int zones=0;for(int i=0;i<3;i++){var r=model.rooms[i];var s=map.stages[i];int z=r.encounters==null?1:Math.Max(1,r.encounters.Length);zones+=z;rooms[i]=new CandidateRoomArt{id=s.sourceWorldId,width=s.width,zones=z,spawnCount=new[]{31,32,1}[i],bgm=s.bgm,bgmAvailable=false};}
   factoryProductMapR1179=map;try{CheckMenuProductIndex(index,()=>ValidateFactoryProductInstalledR1179(index));factoryProductAssetsPresentR1179=true;}catch(Exception e){factoryProductMenuMessageR1179="CONTENT NOT INSTALLED";Debug.LogWarning("Factory Resources unavailable: "+e.Message);}
   factoryProductMapRowR1179=new CandidateDungeonArt{id="altera-nasod-factory",title="NASOD FACTORY",description="Explore the Nasod Factory.",region="altera",sourceGraph=FactoryProductResourceIndexR1168.Route,count=3,combatZones=zones,runtimeIndex=-1,runtimeAdmitted=false,rooms=rooms};mapDungeons.Add(factoryProductMapRowR1179);
  }catch(Exception e){factoryProductMapR1179=null;factoryProductMapRowR1179=null;factoryProductAssetsPresentR1179=false;factoryProductMenuMessageR1179="CONTENT UNAVAILABLE";Debug.LogException(e);}
 }
 bool TryEnterFactoryProductMapRouteR1179(){
  var row=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;if(!IsFactoryProductMapRowR1179(row))return false;
  if(factoryProductAssetsPresentR1179&&!factoryProductSessionR1179){try{BeginFactoryProductR1179();}catch(Exception e){factoryProductMenuMessageR1179="CONTENT COULD NOT BE LOADED";Debug.LogException(e);RenderMenus();}}return true;
 }
 void RenderFactoryProductMapSelectionR1179(){
  if(openMenu!="dungeons")return;var selected=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;
  for(int i=0;i<MapPageSize;i++){int at=mapPage*MapPageSize+i;if(at>=mapVisible.Count)continue;var row=mapDungeons[mapVisible[at]];if(IsFactoryProductMapRowR1179(row))TextValue("Dungeon Boss "+i,factoryProductSessionR1179?"LOADING...":factoryProductAssetsPresentR1179?"READY TO ENTER":"CONTENT NOT INSTALLED");}
  if(!IsFactoryProductMapRowR1179(selected))return;bool ready=factoryProductAssetsPresentR1179&&!factoryProductSessionR1179;TextValue("Dungeon Enter Text",factoryProductSessionR1179?"LOADING...":ready?"ENTER DUNGEON":"CONTENT NOT INSTALLED");images["Dungeon Enter"].color=C(ready?0x476b61u:0x293944u);TextValue("Dungeon Status",factoryProductSessionR1179?"LOADING...":string.IsNullOrEmpty(factoryProductMenuMessageR1179)?(ready?"READY TO ENTER":"CONTENT NOT INSTALLED"):factoryProductMenuMessageR1179);TextValue("Dungeon Route","AREA 1\n\nAREA 2\n\nAREA 3");for(int i=0;i<3;i++)Visible("Dungeon BGM "+i,false);
 }
}}
