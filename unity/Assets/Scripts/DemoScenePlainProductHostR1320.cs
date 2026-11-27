using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class PlainProductSavedR1320 {
   internal GameModel game;internal ArtCatalog catalog;internal DungeonArt[] dungeons;internal StageArt[] stages;internal NpcContentSetData[] contentSets;internal SpriteLibraryData[] monsters,effects;
   internal bool paused,adventure,pausedMenu;internal int selected,preview,mapSelected,mapPage;internal string menu,origin,region;internal float accumulator;
   internal Camera camera;internal Vector3 position;internal Quaternion rotation;internal Rect rect;internal float aspect,size,fov,near,far;internal bool ortho;internal Matrix4x4 projection;internal RenderTexture target;internal int cullingMask;
   internal PlainProductAudioSavedR1320 audio;internal PlainProductMusicSavedR1320 music;
  }
  bool plainProductSessionR1320,plainProductLoadingR1320,plainProductDisposingR1320;
  Coroutine plainProductRoutineR1320;PlainProductSavedR1320 plainProductSavedR1320;
  PlainProductResourceIndexR1320 plainProductIndexR1320;PlainProductDataPlanR1320 plainProductPlanR1320;GameModel plainProductModelR1320;
  StageArt[] plainProductStagesR1320;NativeScene[] plainProductScenesR1320;byte[][] plainProductGeometryR1320;
  DungeonArt plainProductDungeonR1320;
  readonly List<UnityEngine.Object> plainProductOwnedR1320=new List<UnityEngine.Object>();
  readonly Dictionary<string,Library> plainProductPriorLibrariesR1320=new Dictionary<string,Library>(StringComparer.Ordinal);
  readonly List<string> plainProductLibrariesR1320=new List<string>(),plainProductAtlasKeysR1320=new List<string>(),plainProductSoundIdsR1320=new List<string>();
  readonly Dictionary<string,AudioClip> plainProductPriorSoundsR1320=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  readonly Dictionary<string,SceneMusicAsset> plainProductPriorMusicBindingsR1320=new Dictionary<string,SceneMusicAsset>(StringComparer.Ordinal);
  readonly HashSet<string> plainProductMusicBindingIdsR1320=new HashSet<string>(StringComparer.Ordinal);
  readonly Dictionary<string,Texture2D> plainProductWorldTexturesR1320=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  string plainProductRouteR1320;PlainMineTextureR1302[] plainProductMineTexturesR1320;PlainMineAudioR1302[] plainProductMineSoundsR1320;
  public bool PlainProductSessionOwnedR1320 {get{return plainProductSessionR1320;}}
  public bool PlainProductSessionLoadingR1320 {get{return plainProductSessionR1320&&plainProductLoadingR1320;}}
  public bool PlainProductSessionActiveR1320 {get{return plainProductSessionR1320&&!plainProductLoadingR1320&&plainProductModelR1320!=null&&ReferenceEquals(game,plainProductModelR1320);}}
  static void PlainProductNeedR1320(bool valid,string why){PlainProductResourceIndexR1320.Need(valid,why);}
  NpcEffectBinding sharedAirshipGunnerShots;
  void BindRecycleGunnerShots(NpcEffectDocument regionalEffects){
   // The two routes place the same gunner, weapon and AttackA motion. Recycle
   // carries the original projectile but its attack attachment row is empty.
   const string npc="NUI_AIRSHIP_SOLDIER_GUNNER", attack="AIRSHIP_SOLDIER_GUNNER_ATTACK_A";
   int slot=Array.FindIndex(regionalEffects.bindings,row=>row.npc==npc&&row.state==attack);
   if(slot<0||regionalEffects.bindings[slot].sets.Length!=0)return;
   if(sharedAirshipGunnerShots==null){
    var sources=CreateBattleProductIndexR1325();
    var shared=BattleProductTypedJsonR1282.Read<NpcEffectDocument>(sources.ResolveOriginalBytes(BattleProductInputBindingsR1282.Effects));
    sharedAirshipGunnerShots=Array.Find(shared.bindings,row=>row.npc==npc&&row.state==attack);
    if(sharedAirshipGunnerShots==null)throw new InvalidOperationException("Missing source gunner AttackA attachments");
   }
   regionalEffects.bindings[slot]=sharedAirshipGunnerShots;
  }
  void ClearPlainProductInputR1320(){touches.Clear();pending=new Input{skill=-1};ResetBattleGesture();if(plainProductModelR1320!=null)plainProductModelR1320.ClearBufferedInput();}
  public void BeginPlainProductR1320(string route){
   RequireDevelopmentContent();
   PlainProductNeedR1320(Application.isPlaying&&openMenu=="dungeons"&&game.paused&&!battleProductSessionR1325&&!plainProductSessionR1320&&!plainProductDisposingR1320&&!factoryProductSessionR1179&&!coreProductSessionR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!mine2ProductSessionR1233&&!cargoProductSessionR1288&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview&&!BossIntroductionActive&&worldCamera&&worldCamera.targetTexture==null,"ordinary idle dungeon menu required");
#if UNITY_EDITOR
   PlainProductNeedR1320(!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657&&string.IsNullOrEmpty(unifiedSourceSessionDescriptorV21)&&unifiedSourceLoadSessionV21==null&&unifiedSourceLoaderV21==null,"another Editor source owner is active");
#endif
   PlainProductNeedR1320(plainProductOwnedR1320.Count==0,"previous Plain resource owner was not released");
   var index=TakeMobileProductIndex(()=>{var document=PlainProductResourceIndexR1320.ReadMap(route);PlainProductNeedR1320(document!=null,"normal Plain Resources map is absent");return new PlainProductResourceIndexR1320(document,route);});var map=index.Map;RequirePlainProductMainBaselineR1320(index);CheckHostProductFiles(PlainProductResourceIndexR1320.RouteId(route),()=>ValidatePlainProductInstalledR1320(index));
   PlainProductNeedR1320(audioInitialized&&AudioManifestReady&&musicInitialized&&SceneMusicReady,"ordinary Main audio must be initialized");
   foreach(var song in map.music){SceneMusicAsset binding;if(sceneMusicAssets.TryGetValue(song.id,out binding))PlainProductNeedR1320(binding.resource==song.resource,"existing Main music binding changed "+song.id);}
   var saved=new PlainProductSavedR1320{game=game,catalog=catalog,dungeons=catalog.dungeons,stages=catalog.stages,contentSets=catalog.npcContentSets,monsters=catalog.monsters,effects=catalog.effects,paused=game.paused,adventure=adventureStarted,pausedMenu=pausedBeforeMenu,selected=selectedDungeon,preview=previewDungeon,mapSelected=mapSelected,mapPage=mapPage,menu=openMenu,origin=menuOrigin,region=mapRegion,accumulator=accumulator,camera=worldCamera,position=worldCamera.transform.position,rotation=worldCamera.transform.rotation,rect=worldCamera.rect,aspect=worldCamera.aspect,ortho=worldCamera.orthographic,size=worldCamera.orthographicSize,fov=worldCamera.fieldOfView,near=worldCamera.nearClipPlane,far=worldCamera.farClipPlane,projection=worldCamera.projectionMatrix,target=worldCamera.targetTexture,cullingMask=worldCamera.cullingMask};
   // Publish ownership before any fallible AudioSource/Unity/input mutation.
   plainProductSavedR1320=saved;plainProductIndexR1320=index;plainProductSessionR1320=plainProductLoadingR1320=true;plainProductRouteR1320=route;plainProductDungeonR1320=new DungeonArt{id=map.routeId,title=map.routeTitle,region="altera",sourceGraph=route,count=3};
   try{CapturePlainProductAudioR1320(saved);CapturePlainProductMusicR1320(saved);ClearPlainProductInputR1320();StopMapMusicPreview();plainProductMenuMessageR1320="LOADING...";RenderMenus();plainProductRoutineR1320=StartCoroutine(PlainProductLoadGuardR1320());}
   catch(Exception trigger){try{DisposePlainProductR1320(true);}catch(Exception cleanup){throw new AggregateException("Plain normal Begin and cleanup failed",trigger,cleanup);}throw;}
  }
  IEnumerator PlainProductLoadGuardR1320(){
   yield return null;var work=LoadPlainProductR1320();Exception failure=null;
   try{while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return yielded;}}
   finally{(work as IDisposable)?.Dispose();}
   plainProductRoutineR1320=null;
   if(failure!=null){plainProductMenuMessageR1320="CONTENT COULD NOT BE LOADED";try{DisposePlainProductR1320(true);}catch(Exception cleanup){Debug.LogException(new AggregateException("Plain normal loading and cleanup failed",failure,cleanup));throw;}Debug.LogException(failure);RenderMenus();}
  }
  IEnumerator LoadPlainProductR1320(){
   plainProductPlanR1320=PlainProductDataPlanR1320.Build(plainProductIndexR1320);var p=plainProductPlanR1320;
   if(p.index.Map.sourceDungeon==PlainProductResourceIndexR1320.Recycle)BindRecycleGunnerShots(p.effects);
   // Capacity is checked before constructing any original library Texture2D.
   foreach(var l in p.libraries)PlainProductNeedR1320(l.atlasWidth<=SystemInfo.maxTextureSize&&l.atlasHeight<=SystemInfo.maxTextureSize,"device cannot preserve original atlas "+l.binding.id);
   foreach(var l in p.pagedLibraries)foreach(var page in l.document.pages)PlainProductNeedR1320(page.width<=SystemInfo.maxTextureSize&&page.height<=SystemInfo.maxTextureSize,"device cannot preserve original paged atlas "+l.binding.id);
   foreach(var l in p.libraries){LoadPlainProductLibraryR1320(l);yield return null;}
   foreach(var l in p.pagedLibraries){LoadPlainProductPagedLibraryR1330(l);yield return null;}
   var phases=LoadPlainProductVariablePhasesR1330(p);try{while(phases.MoveNext())yield return phases.Current;}finally{(phases as IDisposable)?.Dispose();}
   plainProductStagesR1320=new StageArt[3];plainProductScenesR1320=new NativeScene[3];plainProductGeometryR1320=new byte[3][];
   for(int i=0;i<3;i++){
    var payload=p.worlds[i];var room=p.model.rooms[i];var stage=p.index.Map.stages[i];var scene=JsonUtility.FromJson<NativeScene>(new UTF8Encoding(false,true).GetString(payload.originalSceneJson));var projection=payload.projection;
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=new NativePath[projection.path.Length];for(int j=0;j<scene.path.Length;j++){var q=projection.path[j];scene.path[j]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}
    plainProductScenesR1320[i]=scene;plainProductGeometryR1320[i]=payload.originalGeometry;
    foreach(var t in payload.textures)if(!plainProductWorldTexturesR1320.ContainsKey(t.sourcePath))plainProductWorldTexturesR1320.Add(t.sourcePath,LoadPlainProductTextureR1320(t.selected,0,0,"Plain world / "+payload.id));
    plainProductStagesR1320[i]=new StageArt{id=room.id,sourceWorldId=payload.id,title=stage.title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,groundNodes=room.groundNodes,surfaces=room.surfaces,encounters=room.encounters,nativeScene=payload.id,bgm=stage.bgm};yield return null;
   }
   var clips=new Dictionary<string,AudioClip>(StringComparer.Ordinal);var checkedPCM=new HashSet<AudioClip>();
   foreach(var a in p.index.Map.audio){var clip=p.index.Audio(a);if(clip.loadState==AudioDataLoadState.Unloaded)PlainProductNeedR1320(clip.LoadAudioData(),"original PCM load request failed "+a.id);while(clip.loadState==AudioDataLoadState.Loading)yield return null;PlainProductNeedR1320(clip&&clip.loadState==AudioDataLoadState.Loaded,"original PCM unavailable "+a.id);if(checkedPCM.Add(clip))RequirePlainProductPcmR1320(a,clip);clips.Add(a.id,clip);}
   foreach(var m in p.index.Map.music)PlainProductNeedR1320(p.index.Music(m),"original BGM unavailable "+m.id);
   // No yield from model/borrowed binding commit through ordinary view publication.
   foreach(var a in clips){AudioClip prior;if(audioClips.TryGetValue(a.Key,out prior)){plainProductPriorSoundsR1320.Add(a.Key,prior);audioClips.Remove(a.Key);}plainProductSoundIdsR1320.Add(a.Key);}RegisterNpcSoundClips(clips);
   foreach(var m in p.index.Map.music)if(plainProductMusicBindingIdsR1320.Add(m.id)){SceneMusicAsset prior;if(sceneMusicAssets.TryGetValue(m.id,out prior))plainProductPriorMusicBindingsR1320.Add(m.id,prior);else sceneMusicAssets[m.id]=new SceneMusicAsset{id=m.id,resource=m.resource};}
   PlainProductNeedR1320(plainProductModelR1320==null,"model already constructed");plainProductModelR1320=new GameModel();game=plainProductModelR1320;game.paused=true;game.ConfigureAwakening(1);
   for(int i=0;i<GameModel.SkillSlotCount;i++)game.EquipSkill(i,plainProductSavedR1320.game.GetEquippedSkill(i).id);ApplyTimings();
   var rooms=CreatePlainProductRoomsR1320(p);game.ConfigurePlainOriginalProductContentR1320(plainProductRouteR1320,PlainProductResourceIndexR1320.RulesSHA,rooms,p.rules,p.effects,p.sounds,p.mechanisms,p.callbackPoints);
    LoadPlainProductMineMediaR1320();
    ConfigurePlainProductWatchWholeR1330(plainProductIndexR1320,plainProductIndexR1320.Document.watchWholeRows[0].document,p.rules);BindPlainProductVariablePhaseModelR1330();ConfigurePlainShockParticlesR1480();
   plainProductLoadingR1320=false;adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearPlainProductInputR1320();PrepareCameraFramingV25();RebuildRoom();ResetAudioTracking();RenderMenus();plainProductMenuMessageR1320="";game.paused=false;
  }
  static RoomDefinition[] CreatePlainProductRoomsR1320(PlainProductDataPlanR1320 p){
   var rooms=new List<RoomDefinition>();int births=0,activated=0;
   for(int i=0;i<3;i++){var r=p.model.rooms[i];var room=new RoomDefinition{id=r.id,title=r.title,width=r.width,spawnX=r.spawnX,spawnY=r.spawnY,hasSpawnY=r.hasSpawnY,exitX=r.exitX,entryEncounter=r.entryEncounter,groundNodes=r.groundNodes,surfaces=r.surfaces,encounters=r.encounters};
    for(int j=0;j<r.enemies.Length;j++){var e=r.enemies[j];bool active=p.ordinaryAI[i][j];PlainProductNeedR1320(!e.aiEnabled,"original plan AI-off declaration changed");
     room.enemies.Add(new EnemySpawn{sourceNPC=e.sourceNPC,encounterIndex=e.encounterIndex,sourceKeyCode=e.sourceKeyCode,countsForClear=e.countsForClear,isGate=e.isGate,kind=e.kind,x=e.x,y=e.y,aggroRange=e.aggroRange,hp=e.hp,groundLine=e.groundLine,facing=e.facing,nativeDepth=e.nativeDepth,visualScale=e.visualScale,aiEnabled=active,spawnAfterDeath=e.spawnAfterDeath,spawnDelay=e.spawnDelay,passive=e.passive,dormant=e.dormant});births++;if(active)activated++;
    }rooms.Add(room);
   }PlainProductNeedR1320(births==(p.index.Map.sourceDungeon==PlainProductResourceIndexR1320.Normal?92:95)&&activated==p.ordinaryAICount,"original per-role Plain activation targets changed");return rooms.ToArray();
  }
  void BuildPlainProductWorldR1320(){
    PlainProductNeedR1320(PlainProductSessionActiveR1320&&game.room>=0&&game.room<3,"current owned Plain world absent");
    BuildNativeWorld(plainProductScenesR1320[game.room],plainProductGeometryR1320[game.room],key=>plainProductWorldTexturesR1320[key],plainProductStagesR1320[game.room].id);
    if(plainProductRouteR1320==PlainProductResourceIndexR1320.Recycle){BindPlainMineOriginalWorldR1269();BindPlainMineMajorViewR1302(plainProductMineTexturesR1320,plainProductMineSoundsR1320,plainProductIndexR1320.Text(plainProductIndexR1320.Document.mineRows[0].meshes),Resources.Load<Shader>("Pixel/Shaders/PlainMineSourceParticleR1302"));}
   }
  void ClosePlainProductAfterFailureR1320(Exception trigger){if(!plainProductSessionR1320||plainProductDisposingR1320)return;try{DisposePlainProductR1320(true);}catch(Exception cleanup){throw new AggregateException("Plain ordinary update/presentation and cleanup failed",trigger,cleanup);}}
  public void EndPlainProductR1320(){DisposePlainProductR1320(true);}
  static void PlainProductCleanupR1320(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  void DisposePlainProductR1320(bool restoreMenu){
   if(!plainProductSessionR1320||plainProductDisposingR1320)return;plainProductDisposingR1320=true;DisposeAirshipSoldiersV40();var s=plainProductSavedR1320;var errors=new List<Exception>();
   try{
    var work=plainProductRoutineR1320;plainProductRoutineR1320=null;if(work!=null)PlainProductCleanupR1320(()=>StopCoroutine(work),errors);if(plainProductModelR1320!=null)plainProductModelR1320.paused=true;
    PlainProductCleanupR1320(()=>ClearPlainProductInputR1320(),errors);PlainProductCleanupR1320(()=>CancelBossIntroduction(),errors);PlainProductCleanupR1320(()=>DetachPlainProductMusicR1320(),errors);PlainProductCleanupR1320(()=>DetachPlainProductAudioR1320(),errors);
    PlainProductCleanupR1320(()=>DisposePlainShockParticlesR1480(),errors);PlainProductCleanupR1320(()=>DisposeNativeWorld(),errors);
    if(plainProductModelR1320!=null){PlainProductCleanupR1320(()=>plainProductModelR1320.SetNpcMechanismDefinitions(null),errors);PlainProductCleanupR1320(()=>plainProductModelR1320.SetNpcEffectDefinitions(null),errors);}
    foreach(string id in plainProductLibrariesR1320){Library prior;if(plainProductPriorLibrariesR1320.TryGetValue(id,out prior))libraries[id]=prior;else libraries.Remove(id);}plainProductLibrariesR1320.Clear();plainProductPriorLibrariesR1320.Clear();
    foreach(string key in plainProductAtlasKeysR1320)ReleasePrivateAtlasV53(key);plainProductAtlasKeysR1320.Clear();
    foreach(string id in plainProductSoundIdsR1320){AudioClip prior;if(plainProductPriorSoundsR1320.TryGetValue(id,out prior))audioClips[id]=prior;else audioClips.Remove(id);}plainProductSoundIdsR1320.Clear();plainProductPriorSoundsR1320.Clear();
    foreach(string id in plainProductMusicBindingIdsR1320){SceneMusicAsset prior;if(plainProductPriorMusicBindingsR1320.TryGetValue(id,out prior))sceneMusicAssets[id]=prior;else sceneMusicAssets.Remove(id);}plainProductMusicBindingIdsR1320.Clear();plainProductPriorMusicBindingsR1320.Clear();
    PlainProductCleanupR1320(()=>ClearPlainProductWatchWholeR1330(),errors);PlainProductCleanupR1320(()=>ClearPlainProductVariablePhasesR1330(),errors);
    for(int i=plainProductOwnedR1320.Count-1;i>=0;i--){var value=plainProductOwnedR1320[i];PlainProductCleanupR1320(()=>{var sprite=value as Sprite;if(!ReferenceEquals(sprite,null))spriteOffsets.Remove(sprite);if(value)BelowVisualDestroyV21(value);},errors);}plainProductOwnedR1320.Clear();plainProductWorldTexturesR1320.Clear();plainProductMineTexturesR1320=null;plainProductMineSoundsR1320=null;
    if(s!=null){game=s.game;catalog=s.catalog;catalog.dungeons=s.dungeons;catalog.stages=s.stages;catalog.npcContentSets=s.contentSets;catalog.monsters=s.monsters;catalog.effects=s.effects;}
    plainProductSessionR1320=plainProductLoadingR1320=false;plainProductModelR1320=null;plainProductPlanR1320=null;plainProductIndexR1320=null;plainProductStagesR1320=null;plainProductScenesR1320=null;plainProductGeometryR1320=null;plainProductRouteR1320=null;
    if(s!=null){selectedDungeon=s.selected;previewDungeon=s.preview;mapSelected=s.mapSelected;mapPage=s.mapPage;mapRegion=s.region;adventureStarted=false;openMenu=s.menu;menuOrigin=s.origin;pausedBeforeMenu=s.pausedMenu;accumulator=s.accumulator;game.paused=s.paused;shownRoom=previousRoom=cameraFollowRoom=-1;
     try{if(restoreMenu)PlainProductCleanupR1320(()=>{RebuildRoom();ResetAudioTracking();},errors);}
     finally{adventureStarted=s.adventure;PlainProductCleanupR1320(()=>RestorePlainProductAudioR1320(s),errors);PlainProductCleanupR1320(()=>RestorePlainProductMusicR1320(s),errors);
      try{if(restoreMenu)PlainProductCleanupR1320(()=>RenderMenus(),errors);}
      finally{PlainProductCleanupR1320(()=>{worldCamera=s.camera;if(worldCamera){worldCamera.targetTexture=s.target;worldCamera.rect=s.rect;worldCamera.aspect=s.aspect;worldCamera.orthographic=s.ortho;worldCamera.orthographicSize=s.size;worldCamera.fieldOfView=s.fov;worldCamera.nearClipPlane=s.near;worldCamera.farClipPlane=s.far;worldCamera.cullingMask=s.cullingMask;worldCamera.transform.SetPositionAndRotation(s.position,s.rotation);worldCamera.projectionMatrix=s.projection;}},errors);}}
    }
   }finally{plainProductSavedR1320=null;plainProductDisposingR1320=false;}
   if(errors.Count>0)throw new AggregateException("Plain normal cleanup failed",errors);
  }
 }
}
