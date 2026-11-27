using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class BattleProductSavedR1325 {
   internal GameModel game;internal ArtCatalog catalog;internal DungeonArt[] dungeons;internal StageArt[] stages;internal NpcContentSetData[] contentSets;internal SpriteLibraryData[] monsters,effects;
   internal float followY,followVelocity;internal int followRoom;internal bool paused,adventure,pausedMenu;internal int selected,preview,mapSelected,mapPage;internal string menu,origin,region;internal float accumulator;
   internal Camera camera;internal Vector3 position;internal Quaternion rotation;internal Rect rect;internal float aspect,size,fov,near,far;internal bool ortho;internal Matrix4x4 projection;internal RenderTexture target;internal int cullingMask;
   internal BattleProductAudioSavedR1325 audio;internal BattleProductMusicSavedR1325 music;
  }
  bool battleProductSessionR1325,battleProductLoadingR1325,battleProductDisposingR1325;
  Coroutine battleProductRoutineR1325;BattleProductSavedR1325 battleProductSavedR1325;
  BattleProductResourceIndexR1282 battleProductIndexR1325;BattleProductDataPlanR1282 battleProductPlanR1325;GameModel battleProductModelR1325;
  StageArt[] battleProductStagesR1325;NativeScene[] battleProductScenesR1325;byte[][] battleProductGeometryR1325;
  readonly DungeonArt battleProductDungeonR1325=new DungeonArt{id="altera-battle-airship",title="BATTLE AIRSHIP",region="altera",sourceGraph=BattleProductRouteR1325,count=4,boss="RAVEN"};
  readonly List<UnityEngine.Object> battleProductOwnedR1325=new List<UnityEngine.Object>();
  readonly Dictionary<string,Library> battleProductPriorLibrariesR1325=new Dictionary<string,Library>(StringComparer.Ordinal);
  readonly List<string> battleProductLibrariesR1325=new List<string>(),battleProductAtlasKeysR1325=new List<string>(),battleProductSoundIdsR1325=new List<string>();
  readonly Dictionary<string,AudioClip> battleProductPriorSoundsR1325=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  readonly Dictionary<string,Texture2D> battleProductWorldTexturesR1325=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  
  public bool BattleProductSessionOwnedR1325 {get{return battleProductSessionR1325;}}
  public bool BattleProductSessionLoadingR1325 {get{return battleProductSessionR1325&&battleProductLoadingR1325;}}
  public bool BattleProductSessionActiveR1325 {get{return battleProductSessionR1325&&!battleProductLoadingR1325&&battleProductModelR1325!=null&&ReferenceEquals(game,battleProductModelR1325);}}
  static void BattleProductNeedR1325(bool valid,string why){if(!valid)throw new InvalidOperationException("Battle ordinary product: "+why);}
  void ClearBattleProductInputR1325(){touches.Clear();pending=new Input{skill=-1};ResetBattleGesture();if(battleProductModelR1325!=null)battleProductModelR1325.ClearBufferedInput();}
  public void BeginBattleProductR1325(){
   RequireDevelopmentContent();
   BattleProductNeedR1325(Application.isPlaying&&openMenu=="dungeons"&&game.paused&&!plainProductSessionR1320&&!battleProductSessionR1325&&!battleProductDisposingR1325&&!cargoProductSessionR1288&&!mine2ProductSessionR1233&&!factoryProductSessionR1179&&!coreProductSessionR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview&&!BossIntroductionActive&&worldCamera&&worldCamera.targetTexture==null,"ordinary idle dungeon menu required");
#if UNITY_EDITOR
   BattleProductNeedR1325(!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657&&string.IsNullOrEmpty(unifiedSourceSessionDescriptorV21)&&unifiedSourceLoadSessionV21==null&&unifiedSourceLoaderV21==null,"another Editor source owner is active");
#endif
   BattleProductNeedR1325(battleProductViewR1325==null&&battleProductOwnedR1325.Count==0,"previous Battle presentation owner was not released");
   var index=TakeMobileProductIndex(CreateBattleProductIndexR1325);RequireBattleProductMainBaselineR1325();CheckHostProductFiles("altera-battle-airship",()=>ValidateBattleProductInstalledR1325(index));
   BattleProductNeedR1325(audioInitialized&&AudioManifestReady&&musicInitialized&&SceneMusicReady,"ordinary Main audio must be initialized");
   RequireBattleProductMusicBindingsR1325();
   var saved=new BattleProductSavedR1325{game=game,catalog=catalog,dungeons=catalog.dungeons,stages=catalog.stages,contentSets=catalog.npcContentSets,monsters=catalog.monsters,effects=catalog.effects,paused=game.paused,adventure=adventureStarted,pausedMenu=pausedBeforeMenu,selected=selectedDungeon,preview=previewDungeon,mapSelected=mapSelected,mapPage=mapPage,menu=openMenu,origin=menuOrigin,region=mapRegion,accumulator=accumulator,followY=cameraFollowY,followVelocity=cameraFollowVelocity,followRoom=cameraFollowRoom,camera=worldCamera,position=worldCamera.transform.position,rotation=worldCamera.transform.rotation,rect=worldCamera.rect,aspect=worldCamera.aspect,ortho=worldCamera.orthographic,size=worldCamera.orthographicSize,fov=worldCamera.fieldOfView,near=worldCamera.nearClipPlane,far=worldCamera.farClipPlane,projection=worldCamera.projectionMatrix,target=worldCamera.targetTexture,cullingMask=worldCamera.cullingMask};
   // Publish ownership before any fallible AudioSource/Unity/input mutation.
   battleProductSavedR1325=saved;battleProductIndexR1325=index;battleProductSessionR1325=battleProductLoadingR1325=true;
   try{CaptureBattleProductAudioR1325(saved);CaptureBattleProductMusicR1325(saved);CaptureBattleProductMainVisualsR1325();ClearBattleProductInputR1325();StopMapMusicPreview();battleProductMenuMessageR1325="LOADING...";RenderMenus();battleProductRoutineR1325=StartCoroutine(BattleProductLoadGuardR1325());}
   catch(Exception trigger){try{DisposeBattleProductR1325(true);}catch(Exception cleanup){throw new AggregateException("Battle normal Begin and cleanup failed",trigger,cleanup);}throw;}
  }
  IEnumerator BattleProductLoadGuardR1325(){
   yield return null;var work=LoadBattleProductR1325();Exception failure=null;
   try{while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return yielded;}}
   finally{(work as IDisposable)?.Dispose();}
   battleProductRoutineR1325=null;
   if(failure!=null){battleProductMenuMessageR1325="CONTENT COULD NOT BE LOADED";try{DisposeBattleProductR1325(true);}catch(Exception cleanup){Debug.LogException(new AggregateException("Battle normal loading and cleanup failed",failure,cleanup));throw;}Debug.LogException(failure);RenderMenus();}
  }
  IEnumerator LoadBattleProductR1325(){
   battleProductPlanR1325=battleProductIndexR1325.BuildDataPlan();var p=battleProductPlanR1325;
   BattleProductNeedR1325(p.MaximumPngDimension<=SystemInfo.maxTextureSize,"device cannot preserve original Battle PNG closure");
   for(int i=0;i<p.Libraries.Length;i++){LoadBattleProductLibraryR1325(p.Libraries[i],p.Atlases[i]);yield return null;}
   battleProductStagesR1325=new StageArt[4];battleProductScenesR1325=new NativeScene[4];battleProductGeometryR1325=p.Geometry;
   for(int i=0;i<4;i++){
    var source=p.Scenes[i];var projection=p.Projections[i];var room=p.Model.rooms[i];var parts=new NativeWorldPart[source.parts.Length];for(int j=0;j<parts.Length;j++){var q=source.parts[j];parts[j]=new NativeWorldPart{name=q.name,texture=q.texture,offset=q.offset,count=q.count,color=q.color,fogColor=q.fogColor,fogNear=q.fogNear,fogFar=q.fogFar,fogHeightStart=q.fogHeightStart,fogHeightEnd=q.fogHeightEnd,fogBias=q.fogBias};}
    var paths=new NativePath[projection.path.Length];for(int j=0;j<paths.Length;j++){var q=projection.path[j];paths[j]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}
    battleProductScenesR1325[i]=new NativeScene{id=source.id,unit=projection.unit,offsetX=projection.offsetX,offsetY=projection.offsetY,path=paths,parts=parts};
    battleProductStagesR1325[i]=new StageArt{id=room.id,sourceWorldId=source.id,title="Battle Airship / Area "+(i+1),width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,groundNodes=room.groundNodes,surfaces=room.surfaces,encounters=room.encounters,nativeScene=source.id,bgm=BattleProductRoomMusicR1325[i]};yield return null;
   }
   foreach(var t in p.WorldTextures)battleProductWorldTexturesR1325.Add(t.Key,LoadBattleProductTextureR1325(t.Value,0,0,false,"Battle original world / "+t.Key));
   var clips=new Dictionary<string,AudioClip>(StringComparer.Ordinal);var pcm=LoadBattleProductPcmMapR1325(p);
   foreach(var a in p.Audio.rows){var row=Array.Find(pcm.rows,x=>x.id==a.id);string resource=a.reusePublishedClip?a.publishedResource:row.resource;var clip=Resources.Load<AudioClip>(resource);BattleProductNeedR1325(clip,"original Battle sound Resources absent "+a.id);if(clip.loadState==AudioDataLoadState.Unloaded)BattleProductNeedR1325(clip.LoadAudioData(),"original sound load request failed "+a.id);while(clip.loadState==AudioDataLoadState.Loading)yield return null;BattleProductNeedR1325(clip.loadState==AudioDataLoadState.Loaded&&clip.samples==a.sourcePcmFrames&&clip.channels==a.channels&&clip.frequency==a.sampleRate,"original sound metadata unavailable/changed "+a.id);if(a.reusePublishedClip)RequireBattleProductBorrowedSoundR1325(a,clip);else RequireBattleProductPcmR1325(row,clip);clips.Add(a.id,clip);}
   var legacy=new Dictionary<string,Texture2D>(StringComparer.Ordinal);foreach(var t in p.LegacyTextures)legacy.Add(t.Key,LoadBattleProductTextureR1325(t.Value,0,0,false,"Battle original legacy FX / "+t.Key));
   var state=new Dictionary<string,Texture2D>(StringComparer.Ordinal);foreach(var t in p.StateTextures)state.Add(t.Key,LoadBattleProductTextureR1325(t.Value,0,0,false,"Battle original state FX / "+t.Key));
   var director=new Dictionary<string,Texture2D>(StringComparer.Ordinal);foreach(var t in p.DirectorTextures)director.Add(t.Key,LoadBattleProductTextureR1325(t.Value,0,0,false,"Battle original director / "+t.Key));
   ConfigureBattleProductWorldLayerR1325(p);
   // No yield across commit. Complete initial-state consumers are set before the one SetRooms call.
   foreach(var a in clips){AudioClip prior;if(audioClips.TryGetValue(a.Key,out prior)){battleProductPriorSoundsR1325.Add(a.Key,prior);audioClips.Remove(a.Key);}battleProductSoundIdsR1325.Add(a.Key);}RegisterNpcSoundClips(clips);
   battleProductModelR1325=new GameModel();game=battleProductModelR1325;game.paused=true;game.ConfigureAwakening(1);for(int i=0;i<GameModel.SkillSlotCount;i++)game.EquipSkill(i,battleProductSavedR1325.game.GetEquippedSkill(i).id);ApplyTimings();
   game.SetNpcDefinitions(p.Rules.definitions);game.SetNpcEffectDefinitions(p.Effects);game.SetNpcSoundDefinitions(p.Sounds);game.SetNpcMechanismDefinitions(p.Mechanisms);
   var root=new GameObject("Battle owned original presentation");battleProductOwnedR1325.Add(root);root.transform.SetParent(effectsRoot,false);battleProductViewR1325=new BattleProductViewR1325(this,game,root.transform);battleProductViewR1325.Configure(p,legacy,state,director);
   game.SetRooms(CreateBattleProductRoomsR1325(p));game.ConfigureAlteraDeferredCannonGates(p.Deferred.definitions);
   battleProductLoadingR1325=false;adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearBattleProductInputR1325();PrepareCameraFramingV25();RebuildRoom();ResetAudioTracking();RenderMenus();battleProductMenuMessageR1325="";game.paused=false;
  }
  static List<RoomDefinition> CreateBattleProductRoomsR1325(BattleProductDataPlanR1282 p){
   var rooms=new List<RoomDefinition>();int births=0,ai=0;
   for(int i=0;i<4;i++){var r=p.Model.rooms[i];var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,groundNodes=r.groundNodes,surfaces=r.surfaces,encounters=r.encounters};foreach(var e in r.enemies){room.enemies.Add(new EnemySpawn{sourceNPC=e.sourceNPC,encounterIndex=e.encounterIndex,sourceKeyCode=e.sourceKeyCode,countsForClear=e.countsForClear,isGate=e.isGate,kind=e.kind,x=e.x,y=e.y,aggroRange=e.aggroRange,hp=e.hp,groundLine=e.groundLine,facing=e.facing,nativeDepth=e.nativeDepth,visualScale=e.visualScale,aiEnabled=e.aiEnabled,spawnAfterDeath=e.spawnAfterDeath,spawnDelay=e.spawnDelay,passive=e.passive,dormant=e.dormant});births++;if(e.aiEnabled)ai++;}rooms.Add(room);}
   BattleProductNeedR1325(births==72&&ai==60,"original72 births/60 initial AI changed; Raven remains released by original director14.5");return rooms;
  }
  void BuildBattleProductWorldR1325(){BattleProductNeedR1325(BattleProductSessionActiveR1325&&game.room>=0&&game.room<4,"current owned Battle world absent");BuildNativeWorld(battleProductScenesR1325[game.room],battleProductGeometryR1325[game.room],key=>battleProductWorldTexturesR1325[key],battleProductStagesR1325[game.room].id);}
  void CloseBattleProductAfterFailureR1325(Exception trigger){if(!battleProductSessionR1325||battleProductDisposingR1325)return;try{DisposeBattleProductR1325(true);}catch(Exception cleanup){throw new AggregateException("Battle ordinary update/presentation and cleanup failed",trigger,cleanup);}}
  public void EndBattleProductR1325(){DisposeBattleProductR1325(true);}
  static void BattleProductCleanupR1325(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  void DisposeBattleProductR1325(bool restoreMenu){
   if(!battleProductSessionR1325||battleProductDisposingR1325)return;battleProductDisposingR1325=true;DisposeAirshipSoldiersV40();var s=battleProductSavedR1325;var errors=new List<Exception>();
   try{
    var work=battleProductRoutineR1325;battleProductRoutineR1325=null;if(work!=null)BattleProductCleanupR1325(()=>StopCoroutine(work),errors);if(battleProductModelR1325!=null)battleProductModelR1325.paused=true;
    BattleProductCleanupR1325(()=>ClearBattleProductInputR1325(),errors);BattleProductCleanupR1325(()=>CancelBossIntroduction(),errors);BattleProductCleanupR1325(()=>DetachBattleProductMusicR1325(),errors);BattleProductCleanupR1325(()=>DetachBattleProductAudioR1325(),errors);
    BattleProductCleanupR1325(()=>DisposeNativeWorld(),errors);BattleProductCleanupR1325(()=>DisposeBattleProductPresentationR1325(),errors);
    if(battleProductModelR1325!=null){BattleProductCleanupR1325(()=>battleProductModelR1325.SetNpcMechanismDefinitions(null),errors);BattleProductCleanupR1325(()=>battleProductModelR1325.SetNpcEffectDefinitions(null),errors);}
    foreach(string id in battleProductLibrariesR1325){Library prior;if(battleProductPriorLibrariesR1325.TryGetValue(id,out prior))libraries[id]=prior;else libraries.Remove(id);}battleProductLibrariesR1325.Clear();battleProductPriorLibrariesR1325.Clear();
    foreach(string key in battleProductAtlasKeysR1325)ReleasePrivateAtlasV53(key);battleProductAtlasKeysR1325.Clear();
    foreach(string id in battleProductSoundIdsR1325){AudioClip prior;if(battleProductPriorSoundsR1325.TryGetValue(id,out prior))audioClips[id]=prior;else audioClips.Remove(id);}battleProductSoundIdsR1325.Clear();battleProductPriorSoundsR1325.Clear();
    for(int i=battleProductOwnedR1325.Count-1;i>=0;i--){var value=battleProductOwnedR1325[i];BattleProductCleanupR1325(()=>{var sprite=value as Sprite;if(!ReferenceEquals(sprite,null)){spriteOffsets.Remove(sprite);nativeUiVisibleLocalRectsV28.Remove(sprite);}if(value)BelowVisualDestroyV21(value);},errors);}battleProductOwnedR1325.Clear();battleProductWorldTexturesR1325.Clear();
    if(s!=null){game=s.game;catalog=s.catalog;catalog.dungeons=s.dungeons;catalog.stages=s.stages;catalog.npcContentSets=s.contentSets;catalog.monsters=s.monsters;catalog.effects=s.effects;}
    battleProductSessionR1325=battleProductLoadingR1325=false;battleProductModelR1325=null;battleProductPlanR1325=null;if(battleProductIndexR1325!=null)battleProductIndexR1325.DropByteCache();battleProductIndexR1325=null;battleProductStagesR1325=null;battleProductScenesR1325=null;battleProductGeometryR1325=null;
    if(s!=null){selectedDungeon=s.selected;previewDungeon=s.preview;mapSelected=s.mapSelected;mapPage=s.mapPage;mapRegion=s.region;adventureStarted=false;openMenu=s.menu;menuOrigin=s.origin;pausedBeforeMenu=s.pausedMenu;accumulator=s.accumulator;game.paused=s.paused;shownRoom=previousRoom=cameraFollowRoom=-1;
     try{if(restoreMenu)BattleProductCleanupR1325(()=>{RebuildRoom();ResetAudioTracking();},errors);}
     finally{adventureStarted=s.adventure;BattleProductCleanupR1325(()=>RestoreBattleProductAudioR1325(s),errors);BattleProductCleanupR1325(()=>RestoreBattleProductMusicR1325(s),errors);BattleProductCleanupR1325(()=>RestoreBattleProductMainVisualsR1325(),errors);
      try{if(restoreMenu)BattleProductCleanupR1325(()=>RenderMenus(),errors);}
      finally{BattleProductCleanupR1325(()=>{cameraFollowY=s.followY;cameraFollowVelocity=s.followVelocity;cameraFollowRoom=s.followRoom;worldCamera=s.camera;if(worldCamera){worldCamera.targetTexture=s.target;worldCamera.rect=s.rect;worldCamera.aspect=s.aspect;worldCamera.orthographic=s.ortho;worldCamera.orthographicSize=s.size;worldCamera.fieldOfView=s.fov;worldCamera.nearClipPlane=s.near;worldCamera.farClipPlane=s.far;worldCamera.cullingMask=s.cullingMask;worldCamera.transform.SetPositionAndRotation(s.position,s.rotation);worldCamera.projectionMatrix=s.projection;}},errors);}}
    }
   }finally{battleProductSavedR1325=null;battleProductDisposingR1325=false;}
   if(errors.Count>0)throw new AggregateException("Battle normal cleanup failed",errors);
  }
 }
}
