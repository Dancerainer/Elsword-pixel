using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class CargoProductSavedR1288 {
   internal GameModel game;internal ArtCatalog catalog;internal DungeonArt[] dungeons;internal StageArt[] stages;internal NpcContentSetData[] contentSets;internal SpriteLibraryData[] monsters,effects;
   internal bool paused,adventure,pausedMenu;internal int selected,preview,mapSelected,mapPage;internal string menu,origin,region;internal float accumulator;
   internal Camera camera;internal Vector3 position;internal Quaternion rotation;internal Rect rect;internal float aspect,size,fov,near,far;internal bool ortho;internal Matrix4x4 projection;internal RenderTexture target;internal int cullingMask;
   internal CargoProductAudioSavedR1288 audio;internal CargoProductMusicSavedR1288 music;
  }
  bool cargoProductSessionR1288,cargoProductLoadingR1288,cargoProductDisposingR1288;
  Coroutine cargoProductRoutineR1288;CargoProductSavedR1288 cargoProductSavedR1288;
  CargoProductResourceIndexR1261 cargoProductIndexR1288;CargoProductDataPlanR1261 cargoProductPlanR1288;GameModel cargoProductModelR1288;
  StageArt[] cargoProductStagesR1288;NativeScene[] cargoProductScenesR1288;byte[][] cargoProductGeometryR1288;
  readonly DungeonArt cargoProductDungeonR1288=new DungeonArt{id="bethma-cargo-airship",title="CARGO AIRSHIP",region="bethma",sourceGraph=CargoProductRouteR1288,count=3,boss="WALLY NO.9 ARMOR"};
  readonly List<UnityEngine.Object> cargoProductOwnedR1288=new List<UnityEngine.Object>();
  readonly Dictionary<string,Library> cargoProductPriorLibrariesR1288=new Dictionary<string,Library>(StringComparer.Ordinal);
  readonly List<string> cargoProductLibrariesR1288=new List<string>(),cargoProductAtlasKeysR1288=new List<string>(),cargoProductSoundIdsR1288=new List<string>();
  readonly Dictionary<string,AudioClip> cargoProductPriorSoundsR1288=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  readonly Dictionary<string,Texture2D> cargoProductWorldTexturesR1288=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  
  public bool CargoProductSessionOwnedR1288 {get{return cargoProductSessionR1288;}}
  public bool CargoProductSessionLoadingR1288 {get{return cargoProductSessionR1288&&cargoProductLoadingR1288;}}
  public bool CargoProductSessionActiveR1288 {get{return cargoProductSessionR1288&&!cargoProductLoadingR1288&&cargoProductModelR1288!=null&&ReferenceEquals(game,cargoProductModelR1288);}}
  static void CargoProductNeedR1288(bool valid,string why){if(!valid)throw new InvalidOperationException("Cargo ordinary product: "+why);}
  void ClearCargoProductInputR1288(){touches.Clear();pending=new Input{skill=-1};ResetBattleGesture();if(cargoProductModelR1288!=null)cargoProductModelR1288.ClearBufferedInput();}
  public void BeginCargoProductR1288(){
   RequireDevelopmentContent();
   CargoProductNeedR1288(Application.isPlaying&&!plainProductSessionR1320&&openMenu=="dungeons"&&game.paused&&!battleProductSessionR1325&&!cargoProductSessionR1288&&!cargoProductDisposingR1288&&!mine2ProductSessionR1233&&!factoryProductSessionR1179&&!coreProductSessionR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview&&!BossIntroductionActive&&worldCamera&&worldCamera.targetTexture==null,"ordinary idle dungeon menu required");
#if UNITY_EDITOR
   CargoProductNeedR1288(!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657&&string.IsNullOrEmpty(unifiedSourceSessionDescriptorV21)&&unifiedSourceLoadSessionV21==null&&unifiedSourceLoaderV21==null,"another Editor source owner is active");
#endif
   CargoProductNeedR1288(cargoProductFxR1288==null&&cargoProductDirectorViewR1288==null&&cargoProductSkinsR1288==null&&cargoProductOwnedR1288.Count==0,"previous Cargo presentation owner was not released");
   var index=TakeMobileProductIndex(CreateCargoProductIndexR1288);RequireCargoProductMainBaselineR1288();CheckHostProductFiles("bethma-cargo-airship",()=>ValidateCargoProductInstalledR1288(index));
   CargoProductNeedR1288(audioInitialized&&AudioManifestReady&&musicInitialized&&SceneMusicReady,"ordinary Main audio must be initialized");
   RequireCargoProductMusicBindingsR1288();
   var saved=new CargoProductSavedR1288{game=game,catalog=catalog,dungeons=catalog.dungeons,stages=catalog.stages,contentSets=catalog.npcContentSets,monsters=catalog.monsters,effects=catalog.effects,paused=game.paused,adventure=adventureStarted,pausedMenu=pausedBeforeMenu,selected=selectedDungeon,preview=previewDungeon,mapSelected=mapSelected,mapPage=mapPage,menu=openMenu,origin=menuOrigin,region=mapRegion,accumulator=accumulator,camera=worldCamera,position=worldCamera.transform.position,rotation=worldCamera.transform.rotation,rect=worldCamera.rect,aspect=worldCamera.aspect,ortho=worldCamera.orthographic,size=worldCamera.orthographicSize,fov=worldCamera.fieldOfView,near=worldCamera.nearClipPlane,far=worldCamera.farClipPlane,projection=worldCamera.projectionMatrix,target=worldCamera.targetTexture,cullingMask=worldCamera.cullingMask};
   // Publish ownership before any fallible AudioSource/Unity/input mutation.
   cargoProductSavedR1288=saved;cargoProductIndexR1288=index;cargoProductSessionR1288=cargoProductLoadingR1288=true;
   try{CaptureCargoProductAudioR1288(saved);CaptureCargoProductMusicR1288(saved);ClearCargoProductInputR1288();StopMapMusicPreview();cargoProductMenuMessageR1288="LOADING...";RenderMenus();cargoProductRoutineR1288=StartCoroutine(CargoProductLoadGuardR1288());}
   catch(Exception trigger){try{DisposeCargoProductR1288(true);}catch(Exception cleanup){throw new AggregateException("Cargo normal Begin and cleanup failed",trigger,cleanup);}throw;}
  }
  IEnumerator CargoProductLoadGuardR1288(){
   yield return null;var work=LoadCargoProductR1288();Exception failure=null;
   try{while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return yielded;}}
   finally{(work as IDisposable)?.Dispose();}
   cargoProductRoutineR1288=null;
   if(failure!=null){cargoProductMenuMessageR1288="CONTENT COULD NOT BE LOADED";try{DisposeCargoProductR1288(true);}catch(Exception cleanup){Debug.LogException(new AggregateException("Cargo normal loading and cleanup failed",failure,cleanup));throw;}Debug.LogException(failure);RenderMenus();}
  }
  IEnumerator LoadCargoProductR1288(){
   cargoProductPlanR1288=cargoProductIndexR1288.BuildDataPlan();var p=cargoProductPlanR1288;
   CargoProductNeedR1288(SystemInfo.maxTextureSize>=4048&&p.maximumAtlasDimension<=SystemInfo.maxTextureSize,"device cannot preserve original Cargo PNG closure");
   foreach(var l in p.libraries){LoadCargoProductLibraryR1288(l);yield return null;}
   cargoProductStagesR1288=new StageArt[3];cargoProductScenesR1288=new NativeScene[3];cargoProductGeometryR1288=new byte[3][];
   for(int i=0;i<3;i++){
    var payload=p.worlds[i];var room=p.modelBeforeAI.rooms[i];var scene=JsonUtility.FromJson<NativeScene>(new UTF8Encoding(false,true).GetString(payload.originalScene));var projection=payload.projection;
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=new NativePath[projection.path.Length];for(int j=0;j<scene.path.Length;j++){var q=projection.path[j];scene.path[j]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}
    cargoProductScenesR1288[i]=scene;cargoProductGeometryR1288[i]=payload.geometry;
    foreach(var t in payload.textures)if(!cargoProductWorldTexturesR1288.ContainsKey(t.Key))cargoProductWorldTexturesR1288.Add(t.Key,LoadCargoProductTextureR1288(t.Value,0,0,false,"Cargo world / "+payload.input.id));
    cargoProductStagesR1288[i]=new StageArt{id=room.id,sourceWorldId=payload.input.id,title="Cargo Airship / Area "+(i+1),width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,groundNodes=room.groundNodes,surfaces=room.surfaces,encounters=room.encounters,nativeScene=payload.input.id,bgm=payload.musicId};yield return null;
   }
   cargoProductAudioIndexR1288=new CargoProductAudioResourceIndexR1280();var clips=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
   foreach(var a in cargoProductAudioIndexR1288.Bindings){var source=p.originalAudioBindings[a.id];CargoProductNeedR1288(source.sha256==a.originalOggSha256,"original OGG sound identity changed "+a.id);var clip=Resources.Load<AudioClip>(a.resource);CargoProductNeedR1288(clip,"original Cargo PCM Resources clip absent "+a.id);if(clip.loadState==AudioDataLoadState.Unloaded)CargoProductNeedR1288(clip.LoadAudioData(),"original PCM load request failed "+a.id);while(clip.loadState==AudioDataLoadState.Loading)yield return null;CargoProductNeedR1288(clip.loadState==AudioDataLoadState.Loaded,"original PCM unavailable "+a.id);RequireCargoProductPcmR1288(a,clip);clips.Add(a.id,clip);}
   cargoProductDirectorAudioIndexR1296=new CargoDirectorAudioResourceIndexR1296();cargoProductDirectorAudioIndexR1296.RequireOriginalClock(p.directorClock);
   foreach(var a in cargoProductDirectorAudioIndexR1296.Bindings){var clip=Resources.Load<AudioClip>(a.resource);CargoProductNeedR1288(clip,"original director PCM Resources clip absent "+a.sourceName);if(clip.loadState==AudioDataLoadState.Unloaded)CargoProductNeedR1288(clip.LoadAudioData(),"director PCM load request failed "+a.sourceName);while(clip.loadState==AudioDataLoadState.Loading)yield return null;CargoProductNeedR1288(clip.loadState==AudioDataLoadState.Loaded,"director PCM unavailable "+a.sourceName);RequireCargoProductDirectorPcmR1296(a,clip);cargoProductDirectorClipsR1296.Add(a.sourceName,clip);}
   var fxTextures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);foreach(var t in p.originalFxTextures)fxTextures.Add(t.Key,LoadCargoProductTextureR1288(t.Value,0,0,false,"Cargo source FX / "+t.Key));
   var directorTextures=new Texture2D[p.originalDirectorAtlases.Length];for(int i=0;i<directorTextures.Length;i++)directorTextures[i]=LoadCargoProductTextureR1288(p.originalDirectorAtlases[i],0,0,false,"Cargo whole director / "+i);
   var controls=new Texture2D[3];var palettes=new Texture2D[3];for(int i=0;i<3;i++){controls[i]=LoadCargoProductTextureR1288(p.AIRMaterials[i].indexControl,0,0,true,"Cargo AIR exact index");palettes[i]=LoadCargoProductTextureR1288(p.AIRMaterials[i].paletteLUT,256,256,false,"Cargo AIR exact palette");}
   // No yield from borrowed sound/model commit through publication. All initial source state setters precede room construction.
   foreach(var a in clips){AudioClip prior;if(audioClips.TryGetValue(a.Key,out prior)){cargoProductPriorSoundsR1288.Add(a.Key,prior);audioClips.Remove(a.Key);}cargoProductSoundIdsR1288.Add(a.Key);}RegisterNpcSoundClips(clips);
   CargoProductNeedR1288(cargoProductModelR1288==null,"model already constructed");cargoProductModelR1288=new GameModel();game=cargoProductModelR1288;game.paused=true;game.ConfigureAwakening(1);
   for(int i=0;i<GameModel.SkillSlotCount;i++)game.EquipSkill(i,cargoProductSavedR1288.game.GetEquippedSkill(i).id);ApplyTimings();
   ConfigureCargoCombatV58(p);
   CargoProductNeedR1288(game.CargoSourceConfiguredV21&&game.CargoDirectorHostConfiguredV21,"complete original Cargo source/initial-state payload absent");
   ConfigureCargoProductPresentationR1288(fxTextures,directorTextures,controls,palettes);
   var rooms=CreateCargoProductRoomsR1288(p);game.SetRooms(rooms);game.EnableCargoInnerRobotV58();
   cargoProductLoadingR1288=false;adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearCargoProductInputR1288();PrepareCameraFramingV25();RebuildRoom();ResetAudioTracking();RenderMenus();cargoProductMenuMessageR1288="";game.paused=false;
  }
  static List<RoomDefinition> CreateCargoProductRoomsR1288(CargoProductDataPlanR1261 p){
   var rooms=new List<RoomDefinition>();int births=0,activated=0;
   for(int i=0;i<3;i++){var r=p.modelBeforeAI.rooms[i];var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,groundNodes=r.groundNodes,surfaces=r.surfaces,encounters=r.encounters};
    for(int j=0;j<r.enemies.Length;j++){var e=r.enemies[j];bool active=p.candidateAI[i][j];bool exactEarly=e.sourceNPC=="NUI_BEE_NASOD"||e.sourceNPC=="NUI_NASOD_HEALER"||e.sourceNPC=="NUI_NASOD_MINER_THIN_AIR"||e.sourceNPC=="NUI_NASOD_MINER_FAT_B_AIR"||e.sourceNPC=="NUI_NASOD_MINER_FAT_C_AIR";CargoProductNeedR1288(!e.aiEnabled&&active==(i<2&&p.sourceActive[i][j]&&exactEarly),"exact before-AI/57 early combat activation changed");
     room.enemies.Add(new EnemySpawn{sourceNPC=e.sourceNPC,encounterIndex=e.encounterIndex,sourceKeyCode=e.sourceKeyCode,countsForClear=e.countsForClear,isGate=e.isGate,kind=e.kind,x=e.x,y=e.y,aggroRange=e.aggroRange,hp=e.hp,groundLine=e.groundLine,facing=e.facing,nativeDepth=e.nativeDepth,visualScale=e.visualScale,aiEnabled=active,spawnAfterDeath=e.spawnAfterDeath,spawnDelay=e.spawnDelay,passive=e.passive,dormant=e.dormant});births++;if(active)activated++;
    }rooms.Add(room);
   }CargoProductNeedR1288(births==67&&activated==57,"original67 births/57 early combat activation");return rooms;
  }
  void BuildCargoProductWorldR1288(){CargoProductNeedR1288(CargoProductSessionActiveR1288&&game.room>=0&&game.room<3,"current owned Cargo world absent");BuildNativeWorld(cargoProductScenesR1288[game.room],cargoProductGeometryR1288[game.room],key=>cargoProductWorldTexturesR1288[key],cargoProductStagesR1288[game.room].id);}
  void CloseCargoProductAfterFailureR1288(Exception trigger){if(!cargoProductSessionR1288||cargoProductDisposingR1288)return;try{DisposeCargoProductR1288(true);}catch(Exception cleanup){throw new AggregateException("Cargo ordinary update/presentation and cleanup failed",trigger,cleanup);}}
  public void EndCargoProductR1288(){DisposeCargoProductR1288(true);}
  static void CargoProductCleanupR1288(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  void DisposeCargoProductR1288(bool restoreMenu){
   if(!cargoProductSessionR1288||cargoProductDisposingR1288)return;cargoProductDisposingR1288=true;var s=cargoProductSavedR1288;var errors=new List<Exception>();
   try{
    var work=cargoProductRoutineR1288;cargoProductRoutineR1288=null;if(work!=null)CargoProductCleanupR1288(()=>StopCoroutine(work),errors);if(cargoProductModelR1288!=null)cargoProductModelR1288.paused=true;
    CargoProductCleanupR1288(()=>ClearCargoProductInputR1288(),errors);CargoProductCleanupR1288(()=>CancelBossIntroduction(),errors);CargoProductCleanupR1288(()=>DetachCargoProductMusicR1288(),errors);CargoProductCleanupR1288(()=>DetachCargoProductAudioR1288(),errors);
    CargoProductCleanupR1288(()=>DisposeNativeWorld(),errors);CargoProductCleanupR1288(()=>DisposeCargoProductPresentationR1288(),errors);
    if(cargoProductModelR1288!=null){CargoProductCleanupR1288(()=>cargoProductModelR1288.SetNpcMechanismDefinitions(null),errors);CargoProductCleanupR1288(()=>cargoProductModelR1288.SetNpcEffectDefinitions(null),errors);}
    foreach(string id in cargoProductLibrariesR1288){Library prior;if(cargoProductPriorLibrariesR1288.TryGetValue(id,out prior))libraries[id]=prior;else libraries.Remove(id);}cargoProductLibrariesR1288.Clear();cargoProductPriorLibrariesR1288.Clear();
    foreach(string key in cargoProductAtlasKeysR1288)ReleasePrivateAtlasV53(key);cargoProductAtlasKeysR1288.Clear();
    foreach(string id in cargoProductSoundIdsR1288){AudioClip prior;if(cargoProductPriorSoundsR1288.TryGetValue(id,out prior))audioClips[id]=prior;else audioClips.Remove(id);}cargoProductSoundIdsR1288.Clear();cargoProductPriorSoundsR1288.Clear();
    for(int i=cargoProductOwnedR1288.Count-1;i>=0;i--){var value=cargoProductOwnedR1288[i];CargoProductCleanupR1288(()=>{var sprite=value as Sprite;if(!ReferenceEquals(sprite,null)){spriteOffsets.Remove(sprite);nativeUiVisibleLocalRectsV28.Remove(sprite);}if(value)BelowVisualDestroyV21(value);},errors);}cargoProductOwnedR1288.Clear();cargoProductWorldTexturesR1288.Clear();
    if(s!=null){game=s.game;catalog=s.catalog;catalog.dungeons=s.dungeons;catalog.stages=s.stages;catalog.npcContentSets=s.contentSets;catalog.monsters=s.monsters;catalog.effects=s.effects;}
    cargoProductSessionR1288=cargoProductLoadingR1288=false;cargoProductModelR1288=null;cargoProductPlanR1288=null;if(cargoProductIndexR1288!=null)cargoProductIndexR1288.DropByteCache();cargoProductIndexR1288=null;cargoProductAudioIndexR1288=null;cargoProductStagesR1288=null;cargoProductScenesR1288=null;cargoProductGeometryR1288=null;
    if(s!=null){selectedDungeon=s.selected;previewDungeon=s.preview;mapSelected=s.mapSelected;mapPage=s.mapPage;mapRegion=s.region;adventureStarted=false;openMenu=s.menu;menuOrigin=s.origin;pausedBeforeMenu=s.pausedMenu;accumulator=s.accumulator;game.paused=s.paused;shownRoom=previousRoom=cameraFollowRoom=-1;
     try{if(restoreMenu)CargoProductCleanupR1288(()=>{RebuildRoom();ResetAudioTracking();},errors);}
     finally{adventureStarted=s.adventure;CargoProductCleanupR1288(()=>RestoreCargoProductAudioR1288(s),errors);CargoProductCleanupR1288(()=>RestoreCargoProductMusicR1288(s),errors);
      try{if(restoreMenu)CargoProductCleanupR1288(()=>RenderMenus(),errors);}
      finally{CargoProductCleanupR1288(()=>{worldCamera=s.camera;if(worldCamera){worldCamera.targetTexture=s.target;worldCamera.rect=s.rect;worldCamera.aspect=s.aspect;worldCamera.orthographic=s.ortho;worldCamera.orthographicSize=s.size;worldCamera.fieldOfView=s.fov;worldCamera.nearClipPlane=s.near;worldCamera.farClipPlane=s.far;worldCamera.cullingMask=s.cullingMask;worldCamera.transform.SetPositionAndRotation(s.position,s.rotation);worldCamera.projectionMatrix=s.projection;}},errors);}}
    }
   }finally{cargoProductSavedR1288=null;cargoProductDisposingR1288=false;}
   if(errors.Count>0)throw new AggregateException("Cargo normal cleanup failed",errors);
  }
 }
}
