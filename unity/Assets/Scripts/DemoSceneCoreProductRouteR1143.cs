using System;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  CandidateDungeonArt coreProductMapRowR1143;CoreProductMapR1143 coreProductMapR1143;
  // An explicit source boundary. Data availability cannot admit a model without its original STOP/view owner.
  public bool CoreHardProductRuntimeBridgeReadyR1143 {get{return coreProductMapR1143!=null;}}
  string coreProductMenuMessageR1143="";
  bool IsCoreProductMapRowR1143(CandidateDungeonArt row){return coreProductMapRowR1143!=null&&ReferenceEquals(row,coreProductMapRowR1143)&&row.id=="altera-core-hard"&&row.sourceGraph=="Dungeon_Altera_Core_Hard";}
  void AppendCoreProductMapRouteR1143(string externalCandidatePath){
   coreProductMapRowR1143=null;coreProductMapR1143=null;coreProductMenuMessageR1143="";if(!string.IsNullOrEmpty(externalCandidatePath))return;
   foreach(var d in catalog.dungeons)if(d.id=="altera-core-hard"||d.sourceGraph=="Dungeon_Altera_Core_Hard")return;
   try{
    var map=CoreProductResourceIndexR1143.ReadMap();if(map==null)return;
    var rooms=new CandidateRoomArt[5];for(int i=0;i<5;i++)rooms[i]=new CandidateRoomArt{id=map.worlds[i].id,bgm=map.music[i].id,bgmAvailable=false};
    coreProductMapR1143=map;coreProductMapRowR1143=new CandidateDungeonArt{id=map.routeId,sourceGraph=map.sourceDungeon,title=map.routeTitle,region=map.routeRegion,count=5,combatZones=8,runtimeIndex=-1,runtimeAdmitted=false,rooms=rooms,description=map.routeDescription};mapDungeons.Add(coreProductMapRowR1143);
   }catch(Exception error){coreProductMapR1143=null;coreProductMenuMessageR1143="CONTENT UNAVAILABLE";Debug.LogException(error);}
  }
  bool TryEnterCoreProductMapRouteR1143(){
   if(mapSelected<0||mapSelected>=mapDungeons.Count||mapRegion!="altera"||!IsCoreProductMapRowR1143(mapDungeons[mapSelected]))return false;
   // Ordinary product entry has its own Resources-backed loading transaction. No private diagnostic entry or pilot is invoked.
   try{BeginCoreHardProductR1147();coreProductMenuMessageR1143="";}catch(Exception error){coreProductMenuMessageR1143="CONTENT COULD NOT BE LOADED";Debug.LogException(error);RenderMenus();}return true;
  }
  public CoreProductDataPlanR1143 PrepareCoreHardProductDataR1143(){
   var map=coreProductMapR1143??CoreProductResourceIndexR1143.ReadMap();CoreProductResourceIndexR1143.Need(map!=null,"normal Resources map is not installed");
   var index=new CoreProductResourceIndexR1143(map);index.RequireCurrentCatalog(CurrentCatalogDigest(),catalog);return CoreProductDataPlanR1143.Build(index);
  }
 }
}
