using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class FactoryProductSavedR1179 {
   internal GameModel game;internal bool paused,adventure,pausedMenu;internal int selected,preview,mapSelected,mapPage;internal string menu,origin,region;
   internal float accumulator,aspect,size,fov,near,far;internal bool ortho;internal Rect rect;internal Vector3 position;internal Quaternion rotation;internal Matrix4x4 projection;internal RenderTexture target;
   internal FactoryProductMusicSavedR1179 music;
  }
  bool factoryProductSessionR1179,factoryProductLoadingR1179,factoryProductDisposingR1179;
  FactoryProductResourceIndexR1168 factoryProductIndexR1179;FactoryProductDataPlanR1168 factoryProductPlanR1179;FactoryPreparedComponentV21 factoryProductComponentR1179;FactoryProductSavedR1179 factoryProductSavedR1179;Coroutine factoryProductRoutineR1179;
  StageArt[] factoryProductStagesR1179;NativeScene[] factoryProductScenesR1179;byte[][] factoryProductGeometryR1179;
  readonly DungeonArt factoryProductDungeonR1179=new DungeonArt{id="altera-nasod-factory",title="NASOD FACTORY",region="altera",sourceGraph="Dungeon_Altera_Nasod_Factory_Normal",count=3,boss="RAVEN CLONE"};
  readonly List<UnityEngine.Object> factoryProductOwnedR1179=new List<UnityEngine.Object>();
  readonly Dictionary<string,Texture2D> factoryProductWorldTexturesR1179=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  readonly Dictionary<string,AudioClip> factoryProductPriorSoundsR1179=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  readonly List<string> factoryProductSoundsR1179=new List<string>();
  sealed class FactoryBeltMesh {
   internal Mesh mesh;
   internal Vector2[] original,buffer;
   internal float uvSpeed,lastOffset=float.NaN;
  }
  readonly List<FactoryBeltMesh> factoryBelts=new List<FactoryBeltMesh>();
  void RegisterFactoryBeltMesh(string name,Mesh mesh,Vector2[] original){
   float speed;
   switch(name){
    // SetTex0UVSpeed in the original Factory Stage1/Stage2 world declarations.
    case "Altera_Nasod_Factory_S1_belt01.Y":speed=-.2f;break;
    case "Altera_Nasod_Factory_Stage2_belt.Y":speed=-.32f;break;
    case "Altera_Nasod_Factory_Stage2_beltR.Y":speed=.32f;break;
    default:return;
   }
   mesh.MarkDynamic();
   factoryBelts.Add(new FactoryBeltMesh{mesh=mesh,original=original,buffer=new Vector2[original.Length],uvSpeed=speed});
  }
  void UpdateFactoryBeltMeshes(){
   foreach(var belt in factoryBelts){
    float offset=Mathf.Repeat(game.elapsed*belt.uvSpeed,1);
    if(offset==belt.lastOffset)continue;
    for(int i=0;i<belt.original.Length;i++)belt.buffer[i]=belt.original[i]+new Vector2(offset,0);
    belt.mesh.uv=belt.buffer;
    belt.lastOffset=offset;
   }
  }
  readonly Dictionary<Enemy,FactoryPressFrameV569> factoryProductPressFramesR1179=new Dictionary<Enemy,FactoryPressFrameV569>();
  FactoryPressWarningViewV569 factoryProductWarningR1179;GameObject factoryProductWarningRootR1179;
  public bool FactoryProductSessionOwnedR1179 {get{return factoryProductSessionR1179;}}
  public bool FactoryProductSessionActiveR1179 {get{return factoryProductSessionR1179&&!factoryProductLoadingR1179&&factoryProductComponentR1179!=null&&!factoryProductComponentR1179.IsDisposed&&ReferenceEquals(game,factoryProductComponentR1179.Model);}}
  public bool FactoryProductSessionLoadingR1179 {get{return factoryProductSessionR1179&&factoryProductLoadingR1179;}}
  static void FactoryProductNeedR1179(bool valid,string why){FactoryProductResourceIndexR1168.Need(valid,why);}
  public void BeginFactoryProductR1179(){
   RequireDevelopmentContent();
   FactoryProductNeedR1179(Application.isPlaying&&!plainProductSessionR1320&&openMenu=="dungeons"&&game.paused&&!mine2ProductSessionR1233&&!battleProductSessionR1325&&!cargoProductSessionR1288&&!factoryProductSessionR1179&&!factoryProductDisposingR1179&&!coreProductSessionR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview&&!BossIntroductionActive&&worldCamera&&worldCamera.targetTexture==null,"ordinary idle dungeon menu required");
#if UNITY_EDITOR
   FactoryProductNeedR1179(!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657&&string.IsNullOrEmpty(unifiedSourceSessionDescriptorV21)&&unifiedSourceLoadSessionV21==null&&unifiedSourceLoaderV21==null,"another Editor source owner is active");
#endif
   var map=factoryProductMapR1179??FactoryProductResourceIndexR1168.ReadMap();FactoryProductNeedR1179(map!=null,"normal Resources map is absent");var index=TakeMobileProductIndex(()=>new FactoryProductResourceIndexR1168(map));index.RequireCurrentCatalog(CurrentCatalogDigest(),catalog);CheckHostProductFiles("altera-nasod-factory",()=>ValidateFactoryProductInstalledR1179(index));
   var saved=new FactoryProductSavedR1179{game=game,paused=game.paused,adventure=adventureStarted,pausedMenu=pausedBeforeMenu,selected=selectedDungeon,preview=previewDungeon,mapSelected=mapSelected,mapPage=mapPage,menu=openMenu,origin=menuOrigin,region=mapRegion,accumulator=accumulator,position=worldCamera.transform.position,rotation=worldCamera.transform.rotation,rect=worldCamera.rect,aspect=worldCamera.aspect,ortho=worldCamera.orthographic,size=worldCamera.orthographicSize,fov=worldCamera.fieldOfView,near=worldCamera.nearClipPlane,far=worldCamera.farClipPlane,projection=worldCamera.projectionMatrix,target=worldCamera.targetTexture};
   // Ownership is published before any fallible audio/input/Unity mutation.
   factoryProductSavedR1179=saved;factoryProductSessionR1179=factoryProductLoadingR1179=true;factoryProductIndexR1179=index;
   try{CaptureFactoryProductMusicR1179(saved);game.paused=true;ClearInput();StopMapMusicPreview();StopAudioVoices();factoryProductMenuMessageR1179="LOADING...";RenderMenus();factoryProductRoutineR1179=StartCoroutine(FactoryProductLoadGuardR1179());}
   catch(Exception trigger){try{DisposeFactoryProductR1179(true);}catch(Exception cleanup){throw new AggregateException("Factory normal Begin and cleanup failed",trigger,cleanup);}throw;}
  }
  IEnumerator FactoryProductLoadGuardR1179(){
   yield return null;var work=LoadFactoryProductR1179();Exception failure=null;
   try{while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return yielded;}}
   finally{(work as IDisposable)?.Dispose();}
   factoryProductRoutineR1179=null;
   if(failure!=null){factoryProductMenuMessageR1179="CONTENT COULD NOT BE LOADED";try{DisposeFactoryProductR1179(true);}catch(Exception cleanup){Debug.LogException(new AggregateException("Factory normal loading and cleanup failed",failure,cleanup));throw;}Debug.LogException(failure);RenderMenus();}
  }
  IEnumerator LoadFactoryProductR1179(){
   factoryProductPlanR1179=FactoryProductDataPlanR1168.Build(factoryProductIndexR1179);var p=factoryProductPlanR1179;
   FactoryProductNeedR1179(factoryProductComponentR1179==null,"component already constructed");factoryProductComponentR1179=p.CreateOriginalComponent();var component=factoryProductComponentR1179;
   component.Model.SetNpcEffectDefinitions(p.effects);component.Model.SetNpcSoundDefinitions(p.sounds);
   ConfigureFactoryProductBanksR1179();ConfigureFactoryReactionDisplayR1467(p.rules.definitions);PrepareFactoryProtoOverlayR1466();yield return null;
   factoryProductStagesR1179=new StageArt[3];factoryProductScenesR1179=new NativeScene[3];factoryProductGeometryR1179=new byte[3][];
   for(int i=0;i<3;i++){
    var payload=p.worlds[i];var room=p.model.rooms[i];var source=p.index.Map.stages[i];var scene=JsonUtility.FromJson<NativeScene>(new UTF8Encoding(false,true).GetString(payload.originalSceneJson));var projection=payload.projection;scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=new NativePath[projection.path.Length];for(int j=0;j<scene.path.Length;j++){var q=projection.path[j];scene.path[j]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}
    factoryProductScenesR1179[i]=scene;factoryProductGeometryR1179[i]=payload.originalGeometry;
    foreach(var t in payload.textures)if(!factoryProductWorldTexturesR1179.ContainsKey(t.sourcePath))factoryProductWorldTexturesR1179.Add(t.sourcePath,LoadFactoryProductTextureR1179(t.selected,0,0,"Factory world / "+payload.id));
    factoryProductStagesR1179[i]=new StageArt{id=room.id,sourceWorldId=payload.id,title="Nasod Factory / Area "+(i+1),width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,groundNodes=room.groundNodes,surfaces=room.surfaces,encounters=room.encounters,nativeScene=payload.id,bgm=source.bgm};yield return null;
   }
   var clips=new Dictionary<string,AudioClip>(StringComparer.Ordinal);foreach(var a in p.index.Map.audio){var clip=p.index.Audio(a);if(clip.loadState==AudioDataLoadState.Unloaded)clip.LoadAudioData();while(clip.loadState==AudioDataLoadState.Loading)yield return null;FactoryProductNeedR1179(clip&&clip.loadState==AudioDataLoadState.Loaded,"original OGG load failed "+a.id);clips.Add(a.id,clip);}
   var protoSound=LoadFactoryProtoExtraSoundR1466();if(protoSound.loadState==AudioDataLoadState.Unloaded)protoSound.LoadAudioData();while(protoSound.loadState==AudioDataLoadState.Loading)yield return null;FactoryProductNeedR1179(protoSound&&protoSound.loadState==AudioDataLoadState.Loaded,"Proto original OGG load failed");clips.Add("factory-proto-raven_dashattack2",protoSound);
   foreach(var id in new[]{"AlteraCutScene_LiftMove.ogg","AlteraCutScene_RobotFall.ogg","AlteraCutScene_RobotAssemble.ogg"})FactoryProductNeedR1179(clips.ContainsKey(id),"director source audio absent");
   // No yield from shared audio/model commit through ordinary view publication.
   InstallFactoryProductMusicR1179(clips);foreach(var a in clips){AudioClip prior;if(audioClips.TryGetValue(a.Key,out prior)){factoryProductPriorSoundsR1179.Add(a.Key,prior);audioClips.Remove(a.Key);}factoryProductSoundsR1179.Add(a.Key);}RegisterNpcSoundClips(clips);
   game=component.Model;ConfigureFactorySheriffVisualV32();BeginFactoryRavenIntroViewV21();for(int i=0;i<GameModel.SkillSlotCount;i++)game.EquipSkill(i,factoryProductSavedR1179.game.GetEquippedSkill(i).id);ApplyTimings();
   factoryProductWarningRootR1179=new GameObject("Factory Press presentation");factoryProductOwnedR1179.Add(factoryProductWarningRootR1179);factoryProductWarningRootR1179.transform.SetParent(actorsRoot,false);factoryProductWarningR1179=new FactoryPressWarningViewV569(factoryProductWarningRootR1179.transform);factoryProductWarningR1179.Bind(game);
   factoryProductLoadingR1179=false;adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();factoryProductMenuMessageR1179="";
  }
  Texture2D LoadFactoryProductTextureR1179(FactoryProductFileR1168 file,int width,int height,string label){var bytes=factoryProductIndexR1179.Read(file);var t=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name=label,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};factoryProductOwnedR1179.Add(t);FactoryProductNeedR1179(ImageConversion.LoadImage(t,bytes,true)&&(width==0||t.width==width)&&(height==0||t.height==height),"original PNG dimensions/decode changed");return t;}
  void BuildFactoryProductWorldR1179(){FactoryProductNeedR1179(FactoryProductSessionActiveR1179&&game.room>=0&&game.room<3,"current owned Factory world absent");factoryBelts.Clear();BuildNativeWorld(factoryProductScenesR1179[game.room],factoryProductGeometryR1179[game.room],key=>factoryProductWorldTexturesR1179[key],factoryProductStagesR1179[game.room].id);UpdateFactoryBeltMeshes();}
  void StepFactoryProductR1179(float dt,Input input){if(!factoryProductSessionR1179)return;try{FactoryProductNeedR1179(FactoryProductSessionActiveR1179,"ordinary Step lost actual Factory model");BeforeFactoryRavenIntroStepV21();game.Step(dt,input);ConsumeFactoryPreparedAudioV21(factoryProductComponentR1179);}catch(Exception e){CloseFactoryProductAfterFailureR1179(e);throw;}}
  void PrepareFactoryProductPresentationR1179(){if(!factoryProductSessionR1179)return;try{factoryProductPressFramesR1179.Clear();if(!FactoryProductSessionActiveR1179)return;PrepareFactoryRavenIntroViewV21();var s=factoryProductStagesR1179[game.room];FactoryProductNeedR1179(s.id==game.CurrentRoom.id&&s.width==game.CurrentRoom.width&&s.spawnX==game.CurrentRoom.spawnX&&s.exitX==game.CurrentRoom.exitX,"actual Factory room/view identity changed");foreach(var f in game.ReadFactoryPressPresentationFramesR1470())factoryProductPressFramesR1179.Add(f.actualOwner,f);factoryProductWarningR1179.Sync();UpdateFactoryBeltMeshes();}catch(Exception e){CloseFactoryProductAfterFailureR1179(e);throw;}}
  void ApplyFactoryProductVisibilityR1179(Enemy e,ref bool visible,ref bool gauge){if(FactoryProductSessionActiveR1179&&e.sourceNPC=="NUI_EVENT_NPC_NASOD_FACTORY"){visible=false;gauge=false;}}
  void ApplyFactoryProductPressPoseR1179(Enemy e,ref string clip,ref float clock){if(!FactoryProductSessionActiveR1179)return;FactoryPressFrameV569 f;if(factoryProductPressFramesR1179.TryGetValue(e,out f)){clip=e.clip;clock=f.sourcePoseTime;}}
  void CloseFactoryProductAfterFailureR1179(Exception trigger){if(!factoryProductSessionR1179||factoryProductDisposingR1179)return;try{DisposeFactoryProductR1179(true);}catch(Exception cleanup){throw new AggregateException("Factory normal presentation and cleanup failed",trigger,cleanup);}}
  public void EndFactoryProductR1179(){DisposeFactoryProductR1179(true);}
  static void FactoryProductCleanupR1179(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  void DisposeFactoryProductR1179(bool restoreMenu){
   if(!factoryProductSessionR1179||factoryProductDisposingR1179)return;factoryProductDisposingR1179=true;var s=factoryProductSavedR1179;var errors=new List<Exception>();
   try{
    var work=factoryProductRoutineR1179;factoryProductRoutineR1179=null;if(work!=null)FactoryProductCleanupR1179(()=>StopCoroutine(work),errors);game.paused=true;FactoryProductCleanupR1179(()=>ClearInput(),errors);FactoryProductCleanupR1179(()=>StopAudioVoices(),errors);FactoryProductCleanupR1179(()=>DetachFactoryProductMusicR1179(),errors);FactoryProductCleanupR1179(()=>EndFactoryRavenIntroViewV21(),errors);FactoryProductCleanupR1179(()=>DisposeNativeWorld(),errors);
    FactoryProductCleanupR1179(()=>{if(factoryProductWarningR1179!=null)factoryProductWarningR1179.Dispose();},errors);factoryProductWarningR1179=null;factoryProductWarningRootR1179=null;factoryProductPressFramesR1179.Clear();factoryBelts.Clear();FactoryProductCleanupR1179(()=>DisposeFactorySheriffVisualV32(),errors);ReleaseFactoryReactionDisplayR1467();ReleaseFactoryProtoOverlayR1466();ReleaseFactoryProductBanksR1179();
    foreach(var id in factoryProductSoundsR1179)audioClips.Remove(id);foreach(var a in factoryProductPriorSoundsR1179)audioClips[a.Key]=a.Value;factoryProductSoundsR1179.Clear();factoryProductPriorSoundsR1179.Clear();
    for(int i=factoryProductOwnedR1179.Count-1;i>=0;i--){var value=factoryProductOwnedR1179[i];FactoryProductCleanupR1179(()=>{var sp=value as Sprite;if(sp)spriteOffsets.Remove(sp);if(value)BelowVisualDestroyV21(value);},errors);}factoryProductOwnedR1179.Clear();factoryProductWorldTexturesR1179.Clear();
    var component=factoryProductComponentR1179;factoryProductComponentR1179=null;if(component!=null)FactoryProductCleanupR1179(()=>component.Dispose(),errors);
    if(s!=null){game=s.game;FactoryProductCleanupR1179(()=>RestoreFactoryProductMusicR1179(s),errors);}
    factoryProductSessionR1179=factoryProductLoadingR1179=false;factoryProductIndexR1179=null;factoryProductPlanR1179=null;factoryProductStagesR1179=null;factoryProductScenesR1179=null;factoryProductGeometryR1179=null;factoryProductSavedR1179=null;
    if(s!=null){selectedDungeon=s.selected;previewDungeon=s.preview;mapSelected=s.mapSelected;mapPage=s.mapPage;mapRegion=s.region;adventureStarted=false;openMenu=s.menu;menuOrigin=s.origin;pausedBeforeMenu=s.pausedMenu;accumulator=s.accumulator;game.paused=s.paused;shownRoom=previousRoom=cameraFollowRoom=-1;
     // Original R657 restoration order: reconstruction must not restart the admitted Boss intro.
     try{if(restoreMenu)FactoryProductCleanupR1179(()=>{ClearInput();RebuildRoom();ResetAudioTracking();},errors);}
     finally{adventureStarted=s.adventure;try{if(restoreMenu)FactoryProductCleanupR1179(()=>RenderMenus(),errors);}
      finally{FactoryProductCleanupR1179(()=>{if(worldCamera){worldCamera.targetTexture=s.target;worldCamera.rect=s.rect;worldCamera.aspect=s.aspect;worldCamera.orthographic=s.ortho;worldCamera.orthographicSize=s.size;worldCamera.fieldOfView=s.fov;worldCamera.nearClipPlane=s.near;worldCamera.farClipPlane=s.far;worldCamera.transform.SetPositionAndRotation(s.position,s.rotation);worldCamera.projectionMatrix=s.projection;}},errors);}}
    }
   }finally{factoryProductDisposingR1179=false;}
   if(errors.Count>0)throw new AggregateException("Factory normal cleanup failed",errors);
  }
 }
}
