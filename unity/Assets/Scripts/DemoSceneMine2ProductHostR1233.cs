using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class Mine2ProductSavedR1233 {
   internal GameModel game;internal ArtCatalog catalog;internal DungeonArt[] dungeons;internal StageArt[] stages;internal NpcContentSetData[] contentSets;internal SpriteLibraryData[] monsters,effects;
   internal bool paused,adventure,pausedMenu;internal int selected,preview,mapSelected,mapPage;internal string menu,origin,region;internal float accumulator;
   internal Camera camera;internal Vector3 position;internal Quaternion rotation;internal Rect rect;internal float aspect,size,fov,near,far;internal bool ortho;internal Matrix4x4 projection;internal RenderTexture target;internal int cullingMask;
   internal Mine2ProductAudioSavedR1233 audio;internal Mine2ProductMusicSavedR1233 music;
  }
  bool mine2ProductSessionR1233,mine2ProductLoadingR1233,mine2ProductDisposingR1233;
  Coroutine mine2ProductRoutineR1233;Mine2ProductSavedR1233 mine2ProductSavedR1233;
  Mine2ProductResourceIndexR1211 mine2ProductIndexR1233;Mine2ProductDataPlanR1211 mine2ProductPlanR1233;GameModel mine2ProductModelR1233;
  StageArt[] mine2ProductStagesR1233;NativeScene[] mine2ProductScenesR1233;byte[][] mine2ProductGeometryR1233;
  readonly DungeonArt mine2ProductDungeonR1233=new DungeonArt{id="bethma-richie-mine",title="RICHIE MINE",region="bethma",sourceGraph=Mine2ProductResourceIndexR1211.Route,count=3,boss="WALLY MK2"};
  readonly List<UnityEngine.Object> mine2ProductOwnedR1233=new List<UnityEngine.Object>();
  readonly Dictionary<string,Library> mine2ProductPriorLibrariesR1233=new Dictionary<string,Library>(StringComparer.Ordinal);
  readonly List<string> mine2ProductLibrariesR1233=new List<string>(),mine2ProductAtlasKeysR1233=new List<string>(),mine2ProductSoundIdsR1233=new List<string>();
  readonly Dictionary<string,AudioClip> mine2ProductPriorSoundsR1233=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  readonly Dictionary<string,Texture2D> mine2ProductWorldTexturesR1233=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  RectTransform mine2ProductViewportR1233;
  public bool Mine2ProductSessionOwnedR1233 {get{return mine2ProductSessionR1233;}}
  public bool Mine2ProductSessionLoadingR1233 {get{return mine2ProductSessionR1233&&mine2ProductLoadingR1233;}}
  public bool Mine2ProductSessionActiveR1233 {get{return mine2ProductSessionR1233&&!mine2ProductLoadingR1233&&mine2ProductModelR1233!=null&&ReferenceEquals(game,mine2ProductModelR1233);}}
  static void Mine2ProductNeedR1233(bool valid,string why){Mine2ProductResourceIndexR1211.Need(valid,why);}
  void ClearMine2ProductInputR1233(){touches.Clear();pending=new Input{skill=-1};ResetBattleGesture();if(mine2ProductModelR1233!=null)mine2ProductModelR1233.ClearBufferedInput();}
  public void BeginMine2ProductR1233(){
   RequireDevelopmentContent();
   Mine2ProductNeedR1233(Application.isPlaying&&!plainProductSessionR1320&&openMenu=="dungeons"&&game.paused&&!mine2ProductSessionR1233&&!mine2ProductDisposingR1233&&!battleProductSessionR1325&&!cargoProductSessionR1288&&!factoryProductSessionR1179&&!coreProductSessionR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview&&!BossIntroductionActive&&worldCamera&&worldCamera.targetTexture==null,"ordinary idle dungeon menu required");
#if UNITY_EDITOR
   Mine2ProductNeedR1233(!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657&&string.IsNullOrEmpty(unifiedSourceSessionDescriptorV21)&&unifiedSourceLoadSessionV21==null&&unifiedSourceLoaderV21==null,"another Editor source owner is active");
#endif
   Mine2ProductNeedR1233(mine2VisualDocumentV21==null&&mine2WorldLayerDocV20==null&&mine2VisualViewsV21.Count==0&&mine2VisualMeshesV21.Count==0&&mine2VisualMaterialsV21.Count==0&&mine2VisualSpritesV21.Count==0&&mine2ProductOwnedR1233.Count==0,"previous Mine2 presentation owner was not released");
   var map=mine2ProductMapR1233??Mine2ProductResourceIndexR1211.ReadMap();Mine2ProductNeedR1233(map!=null,"normal Resources map is absent");var index=TakeMobileProductIndex(()=>new Mine2ProductResourceIndexR1211(map));RequireMine2ProductMainBaselineR1233(index);CheckHostProductFiles("bethma-richie-mine",()=>ValidateMine2ProductInstalledR1233(index));
   Mine2ProductNeedR1233(audioInitialized&&AudioManifestReady&&musicInitialized&&SceneMusicReady,"ordinary Main audio must be initialized");
   foreach(var song in map.music){SceneMusicAsset binding;Mine2ProductNeedR1233(sceneMusicAssets.TryGetValue(song.id,out binding)&&binding.resource==song.resource,"existing Main music binding changed "+song.id);}
   var saved=new Mine2ProductSavedR1233{game=game,catalog=catalog,dungeons=catalog.dungeons,stages=catalog.stages,contentSets=catalog.npcContentSets,monsters=catalog.monsters,effects=catalog.effects,paused=game.paused,adventure=adventureStarted,pausedMenu=pausedBeforeMenu,selected=selectedDungeon,preview=previewDungeon,mapSelected=mapSelected,mapPage=mapPage,menu=openMenu,origin=menuOrigin,region=mapRegion,accumulator=accumulator,camera=worldCamera,position=worldCamera.transform.position,rotation=worldCamera.transform.rotation,rect=worldCamera.rect,aspect=worldCamera.aspect,ortho=worldCamera.orthographic,size=worldCamera.orthographicSize,fov=worldCamera.fieldOfView,near=worldCamera.nearClipPlane,far=worldCamera.farClipPlane,projection=worldCamera.projectionMatrix,target=worldCamera.targetTexture,cullingMask=worldCamera.cullingMask};
   // Publish ownership before any fallible AudioSource/Unity/input mutation.
   mine2ProductSavedR1233=saved;mine2ProductIndexR1233=index;mine2ProductSessionR1233=mine2ProductLoadingR1233=true;
   try{CaptureMine2ProductAudioR1233(saved);CaptureMine2ProductMusicR1233(saved);ClearMine2ProductInputR1233();StopMapMusicPreview();mine2ProductMenuMessageR1233="LOADING...";RenderMenus();mine2ProductRoutineR1233=StartCoroutine(Mine2ProductLoadGuardR1233());}
   catch(Exception trigger){try{DisposeMine2ProductR1233(true);}catch(Exception cleanup){throw new AggregateException("Mine2 normal Begin and cleanup failed",trigger,cleanup);}throw;}
  }
  IEnumerator Mine2ProductLoadGuardR1233(){
   yield return null;var work=LoadMine2ProductR1233();Exception failure=null;
   try{while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return yielded;}}
   finally{(work as IDisposable)?.Dispose();}
   mine2ProductRoutineR1233=null;
   if(failure!=null){mine2ProductMenuMessageR1233="CONTENT COULD NOT BE LOADED";try{DisposeMine2ProductR1233(true);}catch(Exception cleanup){Debug.LogException(new AggregateException("Mine2 normal loading and cleanup failed",failure,cleanup));throw;}Debug.LogException(failure);RenderMenus();}
  }
  IEnumerator LoadMine2ProductR1233(){
   mine2ProductPlanR1233=Mine2ProductDataPlanR1211.Build(mine2ProductIndexR1233);var p=mine2ProductPlanR1233;
   // Capacity is checked before constructing any original library Texture2D.
   foreach(var l in p.libraries)Mine2ProductNeedR1233(l.atlasWidth<=SystemInfo.maxTextureSize&&l.atlasHeight<=SystemInfo.maxTextureSize,"device cannot preserve original atlas "+l.binding.id);
   Mine2ProductNeedR1233(SystemInfo.maxTextureSize>=4600,"original Wally 2008x4600 atlas requires device capacity");
   foreach(var l in p.libraries){LoadMine2ProductLibraryR1233(l);yield return null;}
   mine2ProductStagesR1233=new StageArt[3];mine2ProductScenesR1233=new NativeScene[3];mine2ProductGeometryR1233=new byte[3][];
   for(int i=0;i<3;i++){
    var payload=p.worlds[i];var room=p.model.rooms[i];var stage=p.index.Map.stages[i];var scene=JsonUtility.FromJson<NativeScene>(new UTF8Encoding(false,true).GetString(payload.originalSceneJson));var projection=payload.projection;
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=new NativePath[projection.path.Length];for(int j=0;j<scene.path.Length;j++){var q=projection.path[j];scene.path[j]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}
    mine2ProductScenesR1233[i]=scene;mine2ProductGeometryR1233[i]=payload.originalGeometry;
    foreach(var t in payload.textures)if(!mine2ProductWorldTexturesR1233.ContainsKey(t.sourcePath))mine2ProductWorldTexturesR1233.Add(t.sourcePath,LoadMine2ProductTextureR1233(t.selected,0,0,"Mine2 world / "+payload.id));
    mine2ProductStagesR1233[i]=new StageArt{id=room.id,sourceWorldId=payload.id,title=stage.title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,groundNodes=room.groundNodes,surfaces=room.surfaces,encounters=room.encounters,nativeScene=payload.id,bgm=stage.bgm};yield return null;
   }
   var clips=new Dictionary<string,AudioClip>(StringComparer.Ordinal);var checkedPCM=new HashSet<AudioClip>();
   foreach(var a in p.index.Map.audio){var clip=p.index.Audio(a);if(clip.loadState==AudioDataLoadState.Unloaded)Mine2ProductNeedR1233(clip.LoadAudioData(),"original PCM load request failed "+a.id);while(clip.loadState==AudioDataLoadState.Loading)yield return null;Mine2ProductNeedR1233(clip&&clip.loadState==AudioDataLoadState.Loaded,"original PCM unavailable "+a.id);if(checkedPCM.Add(clip))RequireMine2ProductPcmR1233(a,clip);clips.Add(a.id,clip);}
   foreach(var m in p.index.Map.music)Mine2ProductNeedR1233(p.index.Music(m),"original Main BGM unavailable "+m.id);
   var visualTextures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);foreach(var t in p.index.Map.visual.textures)visualTextures.Add(t.key,LoadMine2ProductTextureR1233(t.file,0,0,"Mine2 source visual / "+t.key));
   var layerTexture=LoadMine2ProductTextureR1233(p.index.Map.worldLayer.texture,256,256,"Mine2 original world layer");
   // No yield from model/borrowed binding commit through ordinary view publication.
   foreach(var a in clips){AudioClip prior;if(audioClips.TryGetValue(a.Key,out prior)){mine2ProductPriorSoundsR1233.Add(a.Key,prior);audioClips.Remove(a.Key);}mine2ProductSoundIdsR1233.Add(a.Key);}RegisterNpcSoundClips(clips);
   Mine2ProductNeedR1233(mine2ProductModelR1233==null,"model already constructed");mine2ProductModelR1233=new GameModel();game=mine2ProductModelR1233;game.paused=true;game.ConfigureAwakening(1);
   for(int i=0;i<GameModel.SkillSlotCount;i++)game.EquipSkill(i,mine2ProductSavedR1233.game.GetEquippedSkill(i).id);ApplyTimings();
   game.SetNpcDefinitions(p.rules);game.SetNpcEffectDefinitions(p.effects);game.SetNpcSoundDefinitions(p.sounds);game.SetNpcMechanismDefinitions(p.mechanisms);
   ConfigureMine2VisualV21(p.visualDocument,p.visualMeshes,visualTextures);
   Mine2ProductNeedR1233(mine2VisualDocumentV21!=null,"original Mine2 visual consumer did not configure");
   ConfigureMine2ProductWorldLayerR1233(p.worldLayerDocument,layerTexture);
   var rooms=CreateMine2ProductRoomsR1233(p);game.SetRooms(rooms);
   mine2ProductLoadingR1233=false;adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearMine2ProductInputR1233();PrepareCameraFramingV25();RebuildRoom();ResetAudioTracking();RenderMenus();mine2ProductMenuMessageR1233="";game.paused=false;
  }
  static List<RoomDefinition> CreateMine2ProductRoomsR1233(Mine2ProductDataPlanR1211 p){
   var rooms=new List<RoomDefinition>();int births=0,activated=0;
   for(int i=0;i<3;i++){var r=p.model.rooms[i];var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,groundNodes=r.groundNodes,surfaces=r.surfaces,encounters=r.encounters};
    for(int j=0;j<r.enemies.Length;j++){var e=r.enemies[j];bool active=p.sourceActiveCombat[i][j];Mine2ProductNeedR1233(!e.aiEnabled,"original plan AI-off declaration changed");
     room.enemies.Add(new EnemySpawn{sourceNPC=e.sourceNPC,encounterIndex=e.encounterIndex,sourceKeyCode=e.sourceKeyCode,countsForClear=e.countsForClear,isGate=e.isGate,kind=e.kind,x=e.x,y=e.y,aggroRange=e.aggroRange,hp=e.hp,groundLine=e.groundLine,facing=e.facing,nativeDepth=e.nativeDepth,visualScale=e.visualScale,aiEnabled=active,spawnAfterDeath=e.spawnAfterDeath,spawnDelay=e.spawnDelay,passive=e.passive,dormant=e.dormant});births++;if(active)activated++;
    }rooms.Add(room);
   }Mine2ProductNeedR1233(births==68&&activated==45,"original 68 births / 45 activation targets changed");return rooms;
  }
  void BuildMine2ProductWorldR1233(){Mine2ProductNeedR1233(Mine2ProductSessionActiveR1233&&game.room>=0&&game.room<3,"current owned Mine2 world absent");BuildNativeWorld(mine2ProductScenesR1233[game.room],mine2ProductGeometryR1233[game.room],key=>mine2ProductWorldTexturesR1233[key],mine2ProductStagesR1233[game.room].id);}
  void CloseMine2ProductAfterFailureR1233(Exception trigger){if(!mine2ProductSessionR1233||mine2ProductDisposingR1233)return;try{DisposeMine2ProductR1233(true);}catch(Exception cleanup){throw new AggregateException("Mine2 ordinary update/presentation and cleanup failed",trigger,cleanup);}}
  public void EndMine2ProductR1233(){DisposeMine2ProductR1233(true);}
  static void Mine2ProductCleanupR1233(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  void DisposeMine2ProductR1233(bool restoreMenu){
   if(!mine2ProductSessionR1233||mine2ProductDisposingR1233)return;mine2ProductDisposingR1233=true;var s=mine2ProductSavedR1233;var errors=new List<Exception>();
   try{
    var work=mine2ProductRoutineR1233;mine2ProductRoutineR1233=null;if(work!=null)Mine2ProductCleanupR1233(()=>StopCoroutine(work),errors);if(mine2ProductModelR1233!=null)mine2ProductModelR1233.paused=true;
    Mine2ProductCleanupR1233(()=>ClearMine2ProductInputR1233(),errors);Mine2ProductCleanupR1233(()=>CancelBossIntroduction(),errors);Mine2ProductCleanupR1233(()=>DetachMine2ProductMusicR1233(),errors);Mine2ProductCleanupR1233(()=>DetachMine2ProductAudioR1233(),errors);
    Mine2ProductCleanupR1233(()=>DisposeNativeWorld(),errors);Mine2ProductCleanupR1233(()=>DisposeMine2WorldLayerV20(),errors);Mine2ProductCleanupR1233(()=>DisposeMine2VisualV21(),errors);
    if(mine2ProductModelR1233!=null){Mine2ProductCleanupR1233(()=>mine2ProductModelR1233.SetNpcMechanismDefinitions(null),errors);Mine2ProductCleanupR1233(()=>mine2ProductModelR1233.SetNpcEffectDefinitions(null),errors);}
    foreach(string id in mine2ProductLibrariesR1233){Library prior;if(mine2ProductPriorLibrariesR1233.TryGetValue(id,out prior))libraries[id]=prior;else libraries.Remove(id);}mine2ProductLibrariesR1233.Clear();mine2ProductPriorLibrariesR1233.Clear();mine2RailLibraryV35=null;mine2RailProfileV35=null;
    foreach(string key in mine2ProductAtlasKeysR1233)ReleasePrivateAtlasV53(key);mine2ProductAtlasKeysR1233.Clear();
    foreach(string id in mine2ProductSoundIdsR1233){AudioClip prior;if(mine2ProductPriorSoundsR1233.TryGetValue(id,out prior))audioClips[id]=prior;else audioClips.Remove(id);}mine2ProductSoundIdsR1233.Clear();mine2ProductPriorSoundsR1233.Clear();
    for(int i=mine2ProductOwnedR1233.Count-1;i>=0;i--){var value=mine2ProductOwnedR1233[i];Mine2ProductCleanupR1233(()=>{var sprite=value as Sprite;if(!ReferenceEquals(sprite,null)){spriteOffsets.Remove(sprite);nativeUiVisibleLocalRectsV28.Remove(sprite);}if(value)BelowVisualDestroyV21(value);},errors);}mine2ProductOwnedR1233.Clear();mine2ProductWorldTexturesR1233.Clear();mine2ProductViewportR1233=null;
    if(s!=null){game=s.game;catalog=s.catalog;catalog.dungeons=s.dungeons;catalog.stages=s.stages;catalog.npcContentSets=s.contentSets;catalog.monsters=s.monsters;catalog.effects=s.effects;}
    mine2ProductSessionR1233=mine2ProductLoadingR1233=false;mine2ProductModelR1233=null;mine2ProductPlanR1233=null;mine2ProductIndexR1233=null;mine2ProductStagesR1233=null;mine2ProductScenesR1233=null;mine2ProductGeometryR1233=null;
    if(s!=null){selectedDungeon=s.selected;previewDungeon=s.preview;mapSelected=s.mapSelected;mapPage=s.mapPage;mapRegion=s.region;adventureStarted=false;openMenu=s.menu;menuOrigin=s.origin;pausedBeforeMenu=s.pausedMenu;accumulator=s.accumulator;game.paused=s.paused;shownRoom=previousRoom=cameraFollowRoom=-1;
     try{if(restoreMenu)Mine2ProductCleanupR1233(()=>{RebuildRoom();ResetAudioTracking();},errors);}
     finally{adventureStarted=s.adventure;Mine2ProductCleanupR1233(()=>RestoreMine2ProductAudioR1233(s),errors);Mine2ProductCleanupR1233(()=>RestoreMine2ProductMusicR1233(s),errors);
      try{if(restoreMenu)Mine2ProductCleanupR1233(()=>RenderMenus(),errors);}
      finally{Mine2ProductCleanupR1233(()=>{worldCamera=s.camera;if(worldCamera){worldCamera.targetTexture=s.target;worldCamera.rect=s.rect;worldCamera.aspect=s.aspect;worldCamera.orthographic=s.ortho;worldCamera.orthographicSize=s.size;worldCamera.fieldOfView=s.fov;worldCamera.nearClipPlane=s.near;worldCamera.farClipPlane=s.far;worldCamera.cullingMask=s.cullingMask;worldCamera.transform.SetPositionAndRotation(s.position,s.rotation);worldCamera.projectionMatrix=s.projection;}},errors);}}
    }
   }finally{mine2ProductSavedR1233=null;mine2ProductDisposingR1233=false;}
   if(errors.Count>0)throw new AggregateException("Mine2 normal cleanup failed",errors);
  }
 }
}
