using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string BattleProductRouteR1325="Dungeon_Altera_Battle_Air_Ship_Normal";
  bool battleProductMainBaselineR1325,battleProductAssetsPresentR1325;CandidateDungeonArt battleProductMapRowR1325;string battleProductMenuMessageR1325="";
  void CaptureBattleProductMainBaselineR1325(CatalogDigest digest,ArtCatalog original){battleProductMainBaselineR1325=digest.Matches(41191929,"05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3")&&original!=null&&original.dungeons!=null&&original.dungeons.Length==12&&original.stages!=null&&original.stages.Length==35&&original.npcContentSets!=null&&original.npcContentSets.Length==8;}
  void RequireBattleProductMainBaselineR1325(){BattleProductNeedR1325(battleProductMainBaselineR1325&&CurrentCatalogDigest().Matches(41191929,"05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3"),"original Main raw catalog baseline unavailable");}
  static BattleProductResourceIndexR1282 CreateBattleProductIndexR1325(){return new BattleProductResourceIndexR1282(key=>{var asset=Resources.Load<TextAsset>(key);return asset?asset.bytes:null;});}
  static readonly string[] BattleProductRoomMusicR1325={"music021_stage","music020_stage","music021_stage","music009_boss"};
  static readonly string[] BattleProductMusicIdsR1325={"music021_stage","music020_stage","music009_boss"};
  static readonly string[] BattleProductMusicResourcesR1325={"Pixel/music/music021_stage","Pixel/music/music020_stage","Pixel/music/music009_boss"};
  void RequireBattleProductMusicBindingsR1325(){for(int i=0;i<3;i++){SceneMusicAsset binding;string id=BattleProductMusicIdsR1325[i];BattleProductNeedR1325(sceneMusicAssets.TryGetValue(id,out binding)&&binding.resource==BattleProductMusicResourcesR1325[i]&&Resources.Load<AudioClip>(binding.resource),"original borrowed Main BGM unavailable "+id);}}
  void ValidateBattleProductInstalledR1325(BattleProductResourceIndexR1282 index){
   BattleProductNeedR1325(Resources.Load<Shader>("Pixel/Shaders/NativeWorld")&&Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25")&&Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite"),"original Resources shaders absent");var p=index.BuildDataPlan();var pcm=LoadBattleProductPcmMapR1325(p);foreach(var row in pcm.rows)BattleProductNeedR1325(Resources.Load<AudioClip>(row.resource),"original Battle PCM Resources absent "+row.id);foreach(var row in p.Audio.rows)if(row.reusePublishedClip)BattleProductNeedR1325(Resources.Load<AudioClip>(row.publishedResource),"borrowed published Main source sound absent "+row.id);for(int i=0;i<3;i++)BattleProductNeedR1325(Resources.Load<AudioClip>(BattleProductMusicResourcesR1325[i]),"original borrowed Main BGM absent "+BattleProductMusicIdsR1325[i]);
  }
  bool IsBattleProductMapRowR1325(CandidateDungeonArt row){return row!=null&&ReferenceEquals(row,battleProductMapRowR1325)&&row.id=="altera-battle-airship"&&row.region=="altera"&&row.sourceGraph==BattleProductRouteR1325;}
  void AppendBattleProductMapRouteR1325(string externalCandidatePath){
   battleProductMapRowR1325=null;battleProductAssetsPresentR1325=false;battleProductMenuMessageR1325="";if(!string.IsNullOrEmpty(externalCandidatePath))return;
   // Default Main remains unchanged when this optional route's original model is not installed.
   var marker=Resources.Load<TextAsset>("Pixel/dungeons/altera-battle-airship/source/"+BattleProductInputBindingsR1282.Model);if(!marker)return;
   try{
    foreach(var row in mapDungeons)BattleProductNeedR1325(row.id!="altera-battle-airship"&&row.sourceGraph!=BattleProductRouteR1325,"conflicting existing Battle menu identity");RequireBattleProductMainBaselineR1325();var index=CreateBattleProductIndexR1325();var model=BattleProductTypedJsonR1282.Read<BattleProductModelR1282>(index.ResolveOriginalBytes(BattleProductInputBindingsR1282.Model));BattleProductNeedR1325(model.rooms.Length==4,"original Battle four rooms");
    var rooms=new CandidateRoomArt[4];int zones=0;for(int i=0;i<4;i++){var r=model.rooms[i];int count=r.encounters.Length;zones+=count;rooms[i]=new CandidateRoomArt{id=r.sourceWorldId,width=r.width,zones=count,spawnCount=new[]{24,31,12,5}[i],bgm=BattleProductRoomMusicR1325[i],bgmAvailable=false};}BattleProductNeedR1325(zones==6,"original Battle six encounters");
    try{CheckMenuProductIndex(index,()=>ValidateBattleProductInstalledR1325(index));battleProductAssetsPresentR1325=true;}catch(Exception e){battleProductMenuMessageR1325="CONTENT NOT INSTALLED";Debug.LogWarning("Battle Resources unavailable: "+e.Message);}finally{if(!ReferenceEquals(preparedMobileIndex,index))index.DropByteCache();}
    battleProductMapRowR1325=new CandidateDungeonArt{id="altera-battle-airship",title="BATTLE AIRSHIP",description="Board the Battle Airship.",region="altera",sourceGraph=BattleProductRouteR1325,count=4,combatZones=6,runtimeIndex=-1,runtimeAdmitted=false,rooms=rooms};mapDungeons.Add(battleProductMapRowR1325);
   }catch(Exception e){battleProductMapRowR1325=null;battleProductAssetsPresentR1325=false;battleProductMenuMessageR1325="CONTENT UNAVAILABLE";Debug.LogException(e);}
  }
  bool TryEnterBattleProductMapRouteR1325(){var row=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;if(mapRegion!="altera"||!IsBattleProductMapRowR1325(row))return false;if(battleProductAssetsPresentR1325&&!battleProductSessionR1325){try{BeginBattleProductR1325();}catch(Exception e){battleProductMenuMessageR1325="CONTENT COULD NOT BE LOADED";Debug.LogException(e);RenderMenus();}}return true;}
  void RenderBattleProductMapSelectionR1325(){
   if(openMenu!="dungeons")return;var selected=mapSelected>=0&&mapSelected<mapDungeons.Count?mapDungeons[mapSelected]:null;for(int i=0;i<MapPageSize;i++){int at=mapPage*MapPageSize+i;if(at>=mapVisible.Count)continue;var row=mapDungeons[mapVisible[at]];if(IsBattleProductMapRowR1325(row))TextValue("Dungeon Boss "+i,battleProductSessionR1325?"LOADING...":battleProductAssetsPresentR1325?"READY TO ENTER":"CONTENT NOT INSTALLED");}
   if(!IsBattleProductMapRowR1325(selected)||mapRegion!="altera")return;bool ready=battleProductAssetsPresentR1325&&!battleProductSessionR1325;TextValue("Dungeon Enter Text",battleProductSessionR1325?"LOADING...":ready?"ENTER DUNGEON":"CONTENT NOT INSTALLED");images["Dungeon Enter"].color=C(ready?0x476b61u:0x293944u);TextValue("Dungeon Status",battleProductSessionR1325?"LOADING...":string.IsNullOrEmpty(battleProductMenuMessageR1325)?(ready?"READY TO ENTER":"CONTENT NOT INSTALLED"):battleProductMenuMessageR1325);TextValue("Dungeon Route","AREA 1\n\nAREA 2\n\nAREA 3\n\nAREA 4");for(int i=0;i<3;i++)Visible("Dungeon BGM "+i,false);
  }
 }
}
