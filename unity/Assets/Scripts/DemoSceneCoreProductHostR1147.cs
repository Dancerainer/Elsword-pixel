using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  static CoreProductResourceIndexR1143 coreProductReadIndexR1147;
  static DemoScene coreProductReadOwnerR1147;
  static bool CoreProductResourcesActiveR1147 {get{return coreProductReadIndexR1147!=null;}}
  bool coreProductSessionR1147,coreProductDisposingR1147;
  Coroutine coreProductRoutineR1147;
  CoreProductDataPlanR1143 coreProductPlanR1147;
  public bool CoreHardProductSessionActiveR1147 {get{return coreProductSessionR1147&&coreEntrySessionV21&&forestPreviewActive&&!forestPreviewLoading;}}
  public bool CoreHardProductSessionLoadingR1147 {get{return coreProductSessionR1147&&forestPreviewLoading;}}
  public bool CoreHardProductAuthoredCameraActiveR1147 {get{return CoreHardProductSessionActiveR1147&&(game.CoreHardDirectorTrackActiveV21||coreStopIntroBridgeDraftV21!=null&&coreStopIntroBridgeDraftV21.ActiveV31||game.CurrentCoreStopIntroTicketsV21().Length>0);}}
  static byte[] ReadCoreProductOrEditorFileR1147(string path,string sha){
   if(CoreProductResourcesActiveR1147)return coreProductReadIndexR1147.Read(path,sha);
#if UNITY_EDITOR
   return File.ReadAllBytes(path);
#else
   throw new InvalidOperationException("Core filesystem loading is Editor-only");
#endif
  }
  static string CoreProductCanonicalPathR1147(string value){value=(value??"").Replace('\\','/');while(value.Contains("//"))value=value.Replace("//","/");return value;}
  static string CoreProductDirectoryR1147(string value){value=CoreProductCanonicalPathR1147(value);int last=value.LastIndexOf('/');PreviewRequire(last>0,"Original resource package identity has no directory");return value.Substring(0,last);}
  static string CoreProductRelativeFileR1147(string root,string relative){
   root=CoreProductCanonicalPathR1147(root);relative=CoreProductCanonicalPathR1147(relative);PreviewRequire(!string.IsNullOrEmpty(relative)&&!relative.StartsWith("/",StringComparison.Ordinal)&&relative.IndexOf(':')<0,"Original whole key must be relative");
   foreach(var part in relative.Split('/'))PreviewRequire(part!=".."&&part!="."&&!string.IsNullOrEmpty(part),"Original whole key cannot escape its declared package");return root+"/"+relative;
  }
  static void CoreProductValidateRelativePathR1147(string root,string relative,string declared){PreviewRequire(string.Equals(CoreProductRelativeFileR1147(root,relative),CoreProductCanonicalPathR1147(declared),StringComparison.OrdinalIgnoreCase),"Original whole PNG resource identity mismatched its declared package");}
  PreviewFile CoreProductPinR1147(string root){var p=Array.Find(coreProductReadIndexR1147.Map.roots,r=>r.id==root);PreviewRequire(p!=null,"Core product data root absent "+root);return new PreviewFile{path=p.sourcePath,sha256=p.sha256};}
  CoreEntryScopeV21 CoreProductScopeR1147(){
   var m=coreProductReadIndexR1147.Map;var b=new CoreEntryScopeV21{sourceDungeon=m.sourceDungeon,sourceDifficulty=m.sourceDifficulty,difficultyValue=1,npcLevel=37,expectedBirths=85,expectedRooms=5,expectedStates=372,expectedSubstages=8,
    model=CoreProductPinR1147("model"),rules=CoreProductPinR1147("rules"),originalDisplayRules=CoreProductPinR1147("originalDisplayRules"),effects=CoreProductPinR1147("effects"),sounds=CoreProductPinR1147("sounds"),npcMechanisms=CoreProductPinR1147("npcMechanisms"),
    mechanisms=CoreProductPinR1147("mechanisms"),protection=CoreProductPinR1147("protection"),battlePlane=CoreProductPinR1147("battlePlane"),callbacks=CoreProductPinR1147("callbacks"),balance=CoreProductPinR1147("balance"),closure=CoreProductPinR1147("closure"),localAdds=CoreProductPinR1147("localAdds"),activation=CoreProductPinR1147("activation"),sourceStartOwner=CoreProductPinR1147("sourceStartOwner"),
    closureVisual=CoreProductPinR1147("closureVisual"),localAddVisual=CoreProductPinR1147("localAddVisual"),kingWhole=CoreProductPinR1147("kingWhole"),alarmIndex=CoreProductPinR1147("alarmIndex"),extractorIndex=CoreProductPinR1147("extractorIndex"),
    plantWhole=CoreProductPinR1147("plantWhole"),eventWhole=CoreProductPinR1147("eventWhole"),watchWhole=CoreProductPinR1147("watchWhole"),healerWhole=CoreProductPinR1147("healerWhole"),elementFireBWhole=CoreProductPinR1147("elementFireBWhole"),ravenBWhole=CoreProductPinR1147("ravenBWhole"),guardWhole=CoreProductPinR1147("guardWhole"),elementIceBWhole=CoreProductPinR1147("elementIceBWhole"),wallyWhole=CoreProductPinR1147("wallyWhole"),sprayWhole=CoreProductPinR1147("sprayWhole"),sheriffWhole=CoreProductPinR1147("sheriffWhole"),worlds=new PreviewWorld[5]};
   for(int i=0;i<5;i++)b.worlds[i]=new PreviewWorld{id=m.worlds[i].id};
   // No external review/registry/Root/source-install receipt is synthesized or loaded.
   return b;
  }
  public void BeginCoreHardProductR1147(){
   RequireDevelopmentContent();
   PreviewRequire(Application.isPlaying&&!plainProductSessionR1320&&openMenu=="dungeons"&&game.paused&&!mine2ProductSessionR1233&&!battleProductSessionR1325&&!cargoProductSessionR1288&&!factoryProductSessionR1179&&!coreProductSessionR1147&&!CoreProductResourcesActiveR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!feedbackReview,"Core normal entry needs the ordinary paused dungeon menu without another owned session");
#if UNITY_EDITOR
   PreviewRequire(!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657,"Another source session owns the scene");
#endif
   var map=coreProductMapR1143??CoreProductResourceIndexR1143.ReadMap();PreviewRequire(map!=null,"Core normal Resources map absent");var index=new CoreProductResourceIndexR1143(map);index.RequireCurrentCatalog(CurrentCatalogDigest(),catalog);
   InitSceneMusic();PreviewRequire(SceneMusicReady&&sceneMusicAssets.ContainsKey("music012_stage")&&sceneMusicAssets.ContainsKey("music025_boss"),"Existing Main Core music bindings absent");
   foreach(string id in new[]{"music012_stage","music025_boss"})PreviewRequire(Resources.Load<AudioClip>(sceneMusicAssets[id].resource),"Existing Main Core BGM asset absent "+id);
   PreviewRequire(Resources.Load<Shader>("Pixel/Shaders/NativeWorld")&&Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25"),"Existing Main Core world shaders absent");index.SourceShader();
   coreEntryBeginOwnerV21.Run(()=>{
    var snap=new CoreEntryAdmittedSnapshotV21{selected=selectedDungeon,preview=previewDungeon,paused=game.paused,adventure=adventureStarted,menu=openMenu,origin=menuOrigin,pausedMenu=pausedBeforeMenu,viewport=worldCamera.rect,loadout=new string[GameModel.SkillSlotCount]};for(int i=0;i<snap.loadout.Length;i++)snap.loadout[i]=game.GetEquippedSkill(i).id;coreEntryAdmittedSnapshotV21=snap;
   },()=>{
    coreEntryBeginOwnerV21.MarkLoadingOwned();coreProductReadIndexR1147=index;coreProductReadOwnerR1147=this;coreProductSessionR1147=coreEntrySessionV21=true;forestPreviewLoading=true;forestPreviewActive=false;forestPreviewError="";game.paused=true;ClearInput();StopMapMusicPreview();StopAudioVoices();
    coreProductRoutineR1147=StartCoroutine(CoreProductLoadGuardR1147());
   },()=>{coreEntryAdmittedSnapshotV21=null;coreEntryBeginOwnerV21.Reset();},()=>DisposeCoreHardProductR1147(true));
  }
  IEnumerator CoreProductLoadGuardR1147(){
   // Publish loading before the first yield. Ordinary Update cannot step the admitted or source model here.
   yield return null;var work=LoadCoreHardProductR1147();Exception failure=null;
   try{while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception e){failure=e;}if(failure!=null||!more)break;yield return yielded;}}
   finally{(work as IDisposable)?.Dispose();}
   coreProductRoutineR1147=null;
   if(failure!=null){forestPreviewError=failure.ToString();try{DisposeCoreHardProductR1147(true);}catch(Exception cleanup){Debug.LogException(new AggregateException("Core product load and cleanup failed",failure,cleanup));throw;}Debug.LogException(failure);coreProductMenuMessageR1143="CONTENT COULD NOT BE LOADED";RenderMenus();}
  }
  IEnumerator LoadCoreHardProductR1147(){
   coreProductPlanR1147=CoreProductDataPlanR1143.Build(coreProductReadIndexR1147);var p=coreProductPlanR1147;var b=coreEntryScopeV21=CoreProductScopeR1147();CoreMergedRulePinsR215.Validate(b.rules.sha256,b.originalDisplayRules.sha256);
   // Original definition order and exact deterministic data guards are retained before room construction.
   game.SetNpcDefinitions(p.rules);game.SetNpcEffectDefinitions(CoreCombatEffectDocumentV35(p.effects));game.SetNpcSoundDefinitions(p.sounds);game.SetNpcMechanismDefinitions(p.npcMechanisms);
   game.SetCoreHardMechanismsV21(p.mechanisms);game.SetCoreHardProtectionV21(p.protection);game.SetCoreHardBattlePlaneV21(p.battlePlane);game.SetCoreHardCallbacksV21(p.callbacks);game.SetCoreHardLocalBalanceV21(p.balance);game.SetCoreHardClosureV21(p.closure);game.SetCoreHardLocalAddsV21(p.localAdds);game.SetCoreHardEntryActivationV21(p.activation);
   forestPreviewStages=new StageArt[5];forestPreviewScenes=new NativeScene[5];forestPreviewGeometry=new byte[5][];
   forestPreviewDungeon.id="altera-core-hard";forestPreviewDungeon.title="ALTERA CORE / HARD";forestPreviewDungeon.region="altera";forestPreviewDungeon.count=5;forestPreviewDungeon.boss="EL POWER PLANT";
   for(int i=0;i<5;i++){
    var w=p.worlds[i];var room=p.rooms[i];var scene=JsonUtility.FromJson<NativeScene>(System.Text.Encoding.UTF8.GetString(w.originalSceneJson));var projection=w.projection;scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=new NativePath[projection.path.Length];for(int j=0;j<scene.path.Length;j++){var q=projection.path[j];scene.path[j]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=w.originalGeometry;foreach(var t in w.textures)if(!forestPreviewTextureBindings.ContainsKey(t.sourcePath))forestPreviewTextureBindings[t.sourcePath]=PreviewTextureFile(new PreviewFile{path=t.source.path,sha256=t.source.sha256});
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=w.id,title=room.title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=w.id,bgm=w.bgm};yield return null;
   }
   coreEntrySourceViewV21=new CoreHardOriginalSourceViewV21(actorsRoot,coreProductReadIndexR1147.SourceShader(),npcEffectDepths);
   // The fixed current roster is entirely served by the original whole adapters or exact empty PD helper.
   foreach(var a in coreProductReadIndexR1147.Map.actors)PreviewRequire(a.emptySourceBody||IsCoreAlarmWholeIdV21(a.npc)||IsCoreExtractorWholeIdV21(a.npc)||Array.IndexOf(new[]{"NUI_NASOD_KING","NUI_EL_POWER_PLANT","NUI_EVENT_NPC_ALTERA_CORE_BOSS","NUI_NASOD_WATCH","NUI_NASOD_HEALER_BLADER","NUI_NASOD_ELEMENT_FIRE_B","NUI_RAVEN_CLONE_B","NUI_NASOD_ELITE_GUARD","NUI_NASOD_ELEMENT_ICE_B","NUI_WALLY_9TH_TYPE2","NUI_NASOD_SPRAY_ICE","NUI_NASOD_ELITE_SHERIFF"},a.npc)>=0,"Unmapped original Core actor "+a.npc);
   ConfigureCoreAlarmWholeV21(b.alarmIndex,p.rules);yield return null;ConfigureCoreExtractorWholeV21(b.extractorIndex,p.rules);yield return null;
   ConfigureCorePlantWholeV21(b.plantWhole,null,p.rules);yield return null;ConfigureCoreEventBossWholeV21(b.eventWhole,null,p.rules);yield return null;ConfigureCoreWatchWholeV21(b.watchWhole,null,p.rules);yield return null;
   ConfigureCoreHealerWholeV21(b.healerWhole,null,b.originalDisplayRules,p.rules);yield return null;ConfigureCoreElementFireBWholeV21(b.elementFireBWhole,null,b.originalDisplayRules,p.rules);yield return null;ConfigureCoreRavenBWholeV21(b.ravenBWhole,null,b.originalDisplayRules,p.rules);yield return null;ConfigureCoreGuardWholeV21(b.guardWhole,null,b.originalDisplayRules,p.rules);yield return null;ConfigureCoreElementIceBWholeV21(b.elementIceBWhole,null,b.originalDisplayRules,p.rules);yield return null;
   ConfigureCoreWallyWholeR198(b.wallyWhole,null,b.originalDisplayRules,p.rules);yield return null;ConfigureCoreSprayWholeR203(b.sprayWhole,null,b.originalDisplayRules,p.rules);yield return null;ConfigureCoreSheriffWholeR227(b.sheriffWhole,null,b.originalDisplayRules,p.rules);yield return null;
   ConfigureCoreReactionDisplayV32(p.rules);ConfigureFactorySheriffVisualV32();
   foreach(var a in coreProductReadIndexR1147.Map.audio){var clip=coreProductReadIndexR1147.Audio(a);AudioClip prior;if(audioClips.TryGetValue(a.id,out prior)){coreEntryPreviousAudioV21.Add(a.id,prior);audioClips.Remove(a.id);}RegisterNpcSoundClips(new Dictionary<string,AudioClip>{{a.id,clip}});forestPreviewSounds.Add(a.id);}
   LoadCoreHardClosureVisualBindingV21(b.closureVisual.path,b.closureVisual.sha256);LoadCoreHardLocalAddVisualBindingV21(b.localAddVisual.path,b.localAddVisual.sha256);ConfigureCoreKingWholeLibraryV21(b.kingWhole.path,b.kingWhole.sha256,p.rules);
   // No yield between original room0 initialization, publication of view ownership, and the original STOP owner.
   game.SetRooms(new List<RoomDefinition>(p.rooms));forestPreviewActive=true;PrepareCoreStopIntroBridgeDraftV21(p.sourceStartOwner);
   coreEntryBeginOwnerV21.MarkActive();forestPreviewLoading=false;adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();game.paused=false;
  }
  public void EndCoreHardProductR1147(){DisposeCoreHardProductR1147(true);}
  static void CoreProductCleanupR1147(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  void DisposeCoreProductBindingsR1147(List<Exception> errors){
   CoreProductCleanupR1147(()=>DisposeCoreReactionDisplayV32(),errors);
   CoreProductCleanupR1147(()=>DisposeFactorySheriffVisualV32(),errors);
   // The same original disposal operations, each allowed to finish even when an earlier native release failed.
   CoreProductCleanupR1147(()=>DisposeCoreStopIntroBridgeDraftV21(),errors);
   CoreProductCleanupR1147(()=>game.SetCoreHardEntryActivationV21(null),errors);CoreProductCleanupR1147(()=>DisposeCoreHardEntryOwnedLoadV21(),errors);
   coreFourWholeFramesV21.Clear();coreFourWholeStartEnvelopesV22.Clear();
   CoreProductCleanupR1147(()=>DisposeCoreWallyWholeR198(),errors);CoreProductCleanupR1147(()=>DisposeCoreSprayWholeR203(),errors);CoreProductCleanupR1147(()=>DisposeCoreSheriffWholeR227(),errors);
   CoreProductCleanupR1147(()=>DisposeCoreElementIceBWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreGuardWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreRavenBWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreElementFireBWholeV21(),errors);
   CoreProductCleanupR1147(()=>DisposeCoreHealerWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreWatchWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreEventBossWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCorePlantWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreExtractorWholeV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreAlarmWholeV21(),errors);
   CoreProductCleanupR1147(()=>ClearCoreKingWholeLibraryV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreHardClosureVisualV21(),errors);CoreProductCleanupR1147(()=>DisposeCoreHardLocalAddVisualV21(),errors);
   CoreProductCleanupR1147(()=>game.SetCoreHardEntryActivationV21(null),errors);CoreProductCleanupR1147(()=>game.SetCoreHardMechanismsV21(null),errors);
   CoreProductCleanupR1147(()=>{if(coreEntrySourceViewV21!=null)coreEntrySourceViewV21.Dispose();},errors);coreEntrySourceViewV21=null;coreEntryScopeV21=null;coreEntrySessionV21=false;
  }
  void DisposeCoreHardProductR1147(bool restoreMenu){
   if(coreProductDisposingR1147||!coreProductSessionR1147)return;coreProductDisposingR1147=true;var errors=new List<Exception>();var snap=coreEntryAdmittedSnapshotV21;
   try{
    var routine=coreProductRoutineR1147;coreProductRoutineR1147=null;if(routine!=null)CoreProductCleanupR1147(()=>StopCoroutine(routine),errors);game.paused=true;CoreProductCleanupR1147(()=>ClearInput(),errors);CoreProductCleanupR1147(()=>StopMapMusicPreview(),errors);CoreProductCleanupR1147(()=>StopAudioVoices(),errors);
    CoreProductCleanupR1147(()=>DisposeNativeWorld(),errors);DisposeCoreProductBindingsR1147(errors);
    // A first native-world release may have failed at a camera owner. The owner has now been retired; retry only this idempotent original cleanup, never model or loading work.
    CoreProductCleanupR1147(()=>DisposeNativeWorld(),errors);
    foreach(var id in forestPreviewSounds)audioClips.Remove(id);forestPreviewSounds.Clear();CoreProductCleanupR1147(()=>RestoreCoreHardEntryAudioV21(),errors);
    foreach(var id in forestPreviewLibraries)libraries.Remove(id);forestPreviewLibraries.Clear();foreach(var key in forestPreviewAtlasKeys)atlasTextures.Remove(key);forestPreviewAtlasKeys.Clear();
    for(int i=forestPreviewOwned.Count-1;i>=0;i--){var value=forestPreviewOwned[i];CoreProductCleanupR1147(()=>{var sprite=value as Sprite;if(sprite)spriteOffsets.Remove(sprite);if(value)BelowVisualDestroyV21(value);},errors);}forestPreviewOwned.Clear();
    forestPreviewFiles.Clear();forestPreviewHashes.Clear();forestPreviewTextures.Clear();forestPreviewTextureBindings.Clear();forestPreviewScenes=null;forestPreviewGeometry=null;forestPreviewStages=null;forestPreviewNotice=null;
    coreProductPlanR1147=null;coreEntrySessionV21=forestPreviewActive=forestPreviewLoading=false;coreProductSessionR1147=false;coreEntryScopeV21=null;
    if(ReferenceEquals(coreProductReadOwnerR1147,this)){coreProductReadIndexR1147=null;coreProductReadOwnerR1147=null;}
    if(snap!=null){selectedDungeon=snap.selected;previewDungeon=snap.preview;if(worldCamera)worldCamera.rect=snap.viewport;}
    if(restoreMenu){CoreProductCleanupR1147(()=>{game.SetNpcSoundDefinitions(null);game.SetNpcMechanismDefinitions(null);ConfigureRooms();if(snap!=null)for(int i=0;i<snap.loadout.Length;i++)game.EquipSkill(i,snap.loadout[i]);adventureStarted=false;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();SelectRuntimeDungeonOnMap(selectedDungeon);SetMenu("dungeons");},errors);}
   }finally{coreProductDisposingR1147=false;}
   if(errors.Count>0)throw new AggregateException("Core normal product cleanup failed",errors);
  }
 }
}
