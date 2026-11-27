using System;
using UnityEngine;
using UnityEngine.UI;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  BelowProductResourceMapR1130 belowProductMapR1130;
  CandidateDungeonArt belowProductMapRowR1130;
  bool belowProductAssetsPresentR1130,belowProductRouteSessionR1130;
  string belowProductMenuMessageR1130="";
  public bool BelowProductRouteActiveR1130 {get{return belowProductRouteSessionR1130&&belowPlayerSessionV21&&!belowPlayerLoadingV21;}}
  void AppendBelowProductMapRouteR1130(string externalCandidatePath){
   belowProductMapR1130=null;belowProductMapRowR1130=null;belowProductAssetsPresentR1130=false;belowProductMenuMessageR1130="";
   if(!string.IsNullOrEmpty(externalCandidatePath))return;
   // Never register a second route for an already packaged identity.
   foreach(var d in catalog.dungeons)if(d.sourceGraph==BelowPlayerResourceIndexV21.SourceDungeon||d.id=="altera-below-tunnel"){BelowPlayerResourceIndexV21.Need(d.sourceGraph==BelowPlayerResourceIndexV21.SourceDungeon&&d.id=="altera-below-tunnel"&&d.count==3,"Conflicting existing Below route identity");return;}
   try{
   var map=BelowProductResourcesR1130.Read();if(map==null)return;
   BelowProductResourcesR1130.RequireCurrentCatalog(map,CurrentCatalogDigest(),catalog);
   var rooms=new CandidateRoomArt[3];for(int i=0;i<3;i++)rooms[i]=new CandidateRoomArt{id=map.rooms[i].world,zones=map.rooms[i].zones,spawnCount=map.rooms[i].spawns,bgm=map.rooms[i].bgm,bgmAvailable=false};
   belowProductMapR1130=map;belowProductAssetsPresentR1130=BelowProductResourcesR1130.RuntimeAssetsPresent(map);
   belowProductMapRowR1130=new CandidateDungeonArt{id=map.routeId,title=map.routeTitle,region=map.routeRegion,sourceGraph=map.sourceDungeon,count=3,combatZones=6,runtimeIndex=-1,runtimeAdmitted=false,rooms=rooms,description=map.routeDescription};
   mapDungeons.Add(belowProductMapRowR1130);
   }catch(Exception failure){belowProductMapR1130=null;belowProductMapRowR1130=null;belowProductAssetsPresentR1130=false;belowProductMenuMessageR1130="CONTENT UNAVAILABLE";Debug.LogException(failure);}
  }
  bool IsBelowProductMapRowR1130(CandidateDungeonArt row){return belowProductMapRowR1130!=null&&ReferenceEquals(row,belowProductMapRowR1130)&&row.id=="altera-below-tunnel"&&row.sourceGraph==BelowPlayerResourceIndexV21.SourceDungeon;}
  void EnsureBelowProductRegionTabR1130(){
   Image root;if(images.ContainsKey("Region Tab 3")||!images.TryGetValue("Dungeon Root",out root)||!root)return;
   Button("Region Tab 3","ALTERA",new Rect(576,414,168,29),"region:altera",11,root.transform);
   var tab=root.transform.Find("Region Tab 3");foreach(var t in tab.GetComponentsInChildren<Text>(true))labels[t.name]=t;foreach(var image in tab.GetComponentsInChildren<Image>(true))images[image.name]=image;
  }
  bool TryEnterBelowProductMapRouteR1130(){
   if(mapSelected<0||mapSelected>=mapDungeons.Count||mapRegion!="altera"||!IsBelowProductMapRowR1130(mapDungeons[mapSelected]))return false;
   if(!belowProductAssetsPresentR1130){belowProductMenuMessageR1130="CONTENT NOT INSTALLED";RenderMenus();return true;}
   try{BeginBelowPlayerSessionR1130(belowProductMapR1130);belowProductMenuMessageR1130="";}
   catch(Exception failure){belowProductMenuMessageR1130="CONTENT COULD NOT BE LOADED";Debug.LogException(failure);RenderMenus();}
   return true;
  }
 
  void ValidateBelowProductMainResourcesR1130(){
   // Reuse admitted Main music and shaders before acquiring Below scene ownership.
   InitSceneMusic();SceneMusicAsset binding=null;
   BelowPlayerNeedV21(SceneMusicReady&&sceneMusicAssets.TryGetValue("music015_boss",out binding)&&binding!=null,"Existing Main boss BGM binding is unavailable");
   BelowPlayerNeedV21(Resources.Load<AudioClip>(binding.resource),"Existing Main boss BGM clip is unavailable");
   BelowPlayerNeedV21(Resources.Load<Shader>("Pixel/Shaders/NativeWorld")&&Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25"),"Existing Main native scene shaders are unavailable");
  }
}
}
