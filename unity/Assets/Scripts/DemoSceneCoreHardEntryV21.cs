using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class CoreEntryActorV21 {public string npc,kind,sourceInitialState;public bool emptySourceBody;public PreviewFile identity,geometry,uvs;public PreviewFile[] textures;}
  [Serializable] sealed class CoreEntryMusicV21 {public int room;public string id;public PreviewFile resourceFile,original;public bool substitution;}
  [Serializable] sealed class CoreEntryScopeV21 {
   public string schema,mode,sourceDungeon,sourceDifficulty;public int difficultyValue,npcLevel,expectedBirths,expectedRooms,expectedStates,expectedSubstages;
   public bool editorOnly,pixelActorApproval,pixelSceneryApproval,playableReady,naturalRoute,runtimeAdmitted,sourceOnlyDiagnosticPrepared;
   public PreviewFile model,rules,originalDisplayRules,effects,sounds,npcMechanisms,mechanisms,protection,battlePlane,callbacks,balance,closure,localAdds,activation,closureVisual,localAddVisual,kingWhole,sourceShader,sourceStartOwner;
   public PreviewFile extractorIndex,extractorHandoff;public PreviewFile alarmIndex,alarmHandoff;public PreviewFile[] kingReviewReceipts;
   public PreviewFile plantWhole,plantReview;
   public PreviewFile eventWhole,eventReview;
   public PreviewFile watchWhole,watchReview;
   public PreviewFile healerWhole,healerReview;
   public PreviewFile elementFireBWhole,elementFireBReview;
   public PreviewFile ravenBWhole,ravenBReview;
   public PreviewFile guardWhole,guardReview;
   public PreviewFile elementIceBWhole,elementIceBReview;
   public PreviewFile wallyWhole,wallyRegistryAcceptance;
   public PreviewFile sprayWhole,sprayRegistryAcceptance;
   public PreviewFile sheriffWhole,sheriffRegistryAcceptance;
   public PreviewWorld[] worlds;public CoreEntryActorV21[] actors;public CoreEntryMusicV21[] music;public PreviewAudio[] audio;public PreviewFile[] installedSources;
  }
  [Serializable] public sealed class CoreEntryInspectionV21 {
   public string schema="core-hard-controlled-original-source-entry-inspection-v21",bundleSha256,mode="CONTROLLED_ORIGINAL_SOURCE_DIAGNOSTIC";
   public bool passed,installedSourcesMatch,actualEntryExecuted=false,pixelActorApproval=false,pixelSceneryApproval=false,naturalRoute=false,runtimeAdmitted=false;
   public int rooms,births,states,sourceVisibleBodies,emptySourceBodies;public int approvedWholeIdentities=18,remainingSourceDiagnosticIdentities=4;public int masterApprovedWholeIdentities=13,masterSourceDiagnosticIdentities=9;public bool stagedDisplayIntegration=true,completeCore=false,masterPromoted=false;public string[] limitations;
  }
  bool coreEntrySessionV21;CoreEntryScopeV21 coreEntryScopeV21;CoreHardOriginalSourceViewV21 coreEntrySourceViewV21;
  readonly Dictionary<string,AudioClip> coreEntryPreviousAudioV21=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  public bool CoreHardControlledEntryActiveV21 {get{return coreEntrySessionV21&&forestPreviewActive;}}
  public bool CoreHardEntryBindingsReleasedV21 {get{return !coreEntrySessionV21&&coreEntryScopeV21==null&&coreEntrySourceViewV21==null&&coreEntryPreviousAudioV21.Count==0&&coreKingWholeCacheV21.Count==0&&coreEntryBeginOwnerV21.Phase==CoreEntryBeginOwnershipV21.Stage.None&&coreEntryOwnedLoadWorkV21==null&&coreExtractorStateRowsV21.Count==0&&coreAlarmStateRowsV21.Count==0&&corePlantWholeLookupV21==null&&corePlantWholeSpritesV21.Count==0&&corePlantWholeTexturesV21.Count==0&&coreEventBossWholeLookupV21==null&&coreEventBossWholeSpritesV21.Count==0&&coreEventBossWholeTexturesV21.Count==0&&coreWatchWholeLookupV21==null&&coreWatchWholeSpritesV21.Count==0&&coreWatchWholeTexturesV21.Count==0&&coreHealerWholeLookupV21==null&&coreHealerWholeSpritesV21.Count==0&&coreHealerWholeTexturesV21.Count==0&&coreFourWholeFramesV21.Count==0&&coreFourWholeStartEnvelopesV22.Count==0&&coreRavenBWholeLookupV21==null&&coreRavenBWholeSpritesV21.Count==0&&coreRavenBWholeTexturesV21.Count==0&&CoreWallyWholeBindingsReleasedR198&&CoreSprayWholeBindingsReleasedR203&&CoreSheriffWholeBindingsReleasedR227&&coreElementIceBWholeLookupV21==null&&coreElementIceBWholeSpritesV21.Count==0&&coreElementIceBWholeTexturesV21.Count==0&&coreGuardWholeLookupV21==null&&coreGuardWholeSpritesV21.Count==0&&coreGuardWholeTexturesV21.Count==0&&coreElementFireBWholeLookupV21==null&&coreElementFireBWholeSpritesV21.Count==0&&coreElementFireBWholeTexturesV21.Count==0&&!CoreHardClosureVisualConfiguredV21&&!CoreHardLocalAddVisualConfiguredV21;}}
  public int CoreHardEntrySourceDrawCallsV21 {get{return coreEntrySourceViewV21==null?0:coreEntrySourceViewV21.DrawCalls;}}
  public int CoreHardEntrySourceMissingStateHoldsV21 {get{return coreEntrySourceViewV21==null?0:coreEntrySourceViewV21.MissingStateHolds;}}
  static byte[] CoreEntryReadV21(PreviewFile p){
   if(CoreProductResourcesActiveR1147){PreviewRequire(p!=null,"Core runtime resource pin missing");return coreProductReadIndexR1147.Read(p.path,p.sha256);}
#if UNITY_EDITOR
PreviewRequire(p!=null&&Path.IsPathRooted(p.path)&&!string.IsNullOrEmpty(p.sha256),"Core absolute exact dependency required");var raw=File.ReadAllBytes(p.path);PreviewRequire(PreviewHash(raw)==p.sha256,"Core dependency changed "+p.path);return raw;
#else
   throw new InvalidOperationException("Core filesystem entry is Editor-only");
#endif
  }
  static T CoreEntryJsonV21<T>(PreviewFile p){return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(CoreEntryReadV21(p)));}

#if UNITY_EDITOR
  public static CoreEntryInspectionV21 InspectCoreHardControlledEntryV21(string path){
   var raw=File.ReadAllBytes(Path.GetFullPath(path));var b=JsonUtility.FromJson<CoreEntryScopeV21>(Encoding.UTF8.GetString(raw));
   PreviewRequire(b!=null&&b.schema=="core-hard-controlled-original-source-entry-host-v21-r1"&&b.mode=="CONTROLLED_ORIGINAL_SOURCE_DIAGNOSTIC"&&b.sourceDungeon=="Dungeon_Altera_Core_Hard"&&b.sourceDifficulty=="Hard"&&b.difficultyValue==1&&b.npcLevel==37&&b.editorOnly&&!b.pixelActorApproval&&!b.pixelSceneryApproval&&!b.playableReady&&!b.naturalRoute&&!b.runtimeAdmitted&&b.sourceOnlyDiagnosticPrepared,"Core diagnostic boundary changed");
   PreviewRequire(b.rules!=null&&b.originalDisplayRules!=null,"R215 active gameplay and original display-clock pins required");CoreMergedRulePinsR215.Validate(b.rules.sha256,b.originalDisplayRules.sha256);CoreEntryReadV21(b.originalDisplayRules);
   ValidateCoreHardApprovedWholeReceiptsV21(b);var m=CoreEntryJsonV21<PreviewModel>(b.model);var r=CoreEntryJsonV21<PreviewRules>(b.rules);PreviewRequire(m.rooms.Length==5&&b.worlds.Length==5&&r.definitions.Length==23&&b.actors.Length==23,"Core5world/23identity closure");
   ValidateCorePlantWholeDocumentV21(b.plantWhole,b.plantReview,r.definitions);
   ValidateCoreEventBossWholeDocumentV21(b.eventWhole,b.eventReview,r.definitions);
   ValidateCoreWatchWholeDocumentV21(b.watchWhole,b.watchReview,r.definitions);
   ValidateCoreHealerWholeDocumentV21(b.healerWhole,b.healerReview,b.originalDisplayRules,r.definitions);
   ValidateCoreElementFireBWholeDocumentV21(b.elementFireBWhole,b.elementFireBReview,b.originalDisplayRules,r.definitions);
   ValidateCoreRavenBWholeDocumentV21(b.ravenBWhole,b.ravenBReview,b.originalDisplayRules,r.definitions);
   ValidateCoreGuardWholeDocumentV21(b.guardWhole,b.guardReview,b.originalDisplayRules,r.definitions);
   ValidateCoreElementIceBWholeDocumentV21(b.elementIceBWhole,b.elementIceBReview,b.originalDisplayRules,r.definitions);
   ValidateCoreWallyWholeDocumentR198(b.wallyWhole,b.wallyRegistryAcceptance,b.originalDisplayRules,r.definitions);
   ValidateCoreSprayWholeDocumentR203(b.sprayWhole,b.sprayRegistryAcceptance,b.originalDisplayRules,r.definitions);
   ValidateCoreSheriffWholeDocumentR227(b.sheriffWhole,b.sheriffRegistryAcceptance,b.originalDisplayRules,r.definitions);
   int births=0,states=0,nodes=0,visible=0,empty=0;var ids=new HashSet<string>(StringComparer.Ordinal);
   for(int i=0;i<m.rooms.Length;i++){births+=m.rooms[i].enemies.Length;nodes+=m.rooms[i].encounters.Length;PreviewRequire(m.rooms[i].sourceWorldId==b.worlds[i].id,"Core original world order");CoreEntryReadV21(b.worlds[i].scene);CoreEntryReadV21(b.worlds[i].projection);CoreEntryReadV21(b.worlds[i].binary);foreach(var t in b.worlds[i].textures){PreviewRequire(t.source.path==t.selected.path&&t.source.sha256==t.selected.sha256&&!t.singleMaterialTrial,"Core original-source scene diagnostic substitution");CoreEntryReadV21(t.source);}}
   foreach(var p in r.definitions)states+=p.states.Length;
   foreach(var a in b.actors){PreviewRequire(ids.Add(a.npc)&&Array.Exists(r.definitions,p=>p.id==a.npc&&p.kind==a.kind),"Core exact source actor binding");if(a.emptySourceBody){PreviewRequire(a.npc=="NUI_PD_MONSTER","Only exact empty-PD source helper");empty++;}else{var d=CoreEntryJsonV21<CoreEntrySourceIdentityV21>(a.identity);PreviewRequire(d.id==a.npc,"Core source mesh identity");CoreEntryReadV21(a.geometry);CoreEntryReadV21(a.uvs);foreach(var t in a.textures)CoreEntryReadV21(t);visible++;}}
   foreach(var p in new[]{b.effects,b.sounds,b.npcMechanisms,b.mechanisms,b.protection,b.battlePlane,b.callbacks,b.balance,b.closure,b.localAdds,b.activation,b.sourceStartOwner,b.closureVisual,b.localAddVisual,b.sourceShader})CoreEntryReadV21(p);
   foreach(var a in b.audio)CoreEntryReadV21(a.file);foreach(var s in b.music){PreviewRequire(!s.substitution,"Core original BGM substitution");CoreEntryReadV21(s.resourceFile);CoreEntryReadV21(s.original);}
   bool installed=b.installedSources!=null&&b.installedSources.Length>0;foreach(var p in b.installedSources??new PreviewFile[0])installed&=File.Exists(p.path)&&PreviewHash(File.ReadAllBytes(p.path))==p.sha256;
   PreviewRequire(births==85&&states==372&&nodes==8&&visible==22&&empty==1,"Core exact85birth/372state/8node/22body closure");
   return new CoreEntryInspectionV21{passed=true,bundleSha256=PreviewHash(raw),installedSourcesMatch=installed,rooms=5,births=85,states=372,sourceVisibleBodies=visible,emptySourceBodies=empty,limitations=new[]{"Staged display candidate:17 complete whole-body identities and5 source diagnostic identities. Master remains13approved/9source; full Core pixel art and new merged natural route are unapproved.","Three original source STOP bosses use an explicit authored Internal85 source-START owner and paired view bridge; actual ordinary-frame focus evidence remains required.","Unconsumed source callbacks, visibility and particles remain explicit holds.","First-load or room fixtures do not prove a natural route or the final Hard clear."}};
  }
#endif

  bool coreRoom4ComponentRequestedR908,coreRoom4ComponentInitializedR908;
  int coreRoom4ComponentInitFrameR908=-1;float coreRoom4ComponentInitModelR908;long coreRoom4ComponentInitStepR908;
  public bool CoreHardRoom4ComponentDiagnosticR908 {get{return coreRoom4ComponentInitializedR908&&CoreHardControlledEntryActiveV21;}}
  public bool CoreHardRoom4ComponentBindingsReleasedR908 {get{return !coreRoom4ComponentRequestedR908&&!coreRoom4ComponentInitializedR908&&coreRoom4ComponentInitFrameR908==-1;}}
  public int CoreHardRoom4ComponentInitializedFrameR908 {get{return coreRoom4ComponentInitFrameR908;}}
  public float CoreHardRoom4ComponentInitializedModelSecondsR908 {get{return coreRoom4ComponentInitModelR908;}}
  public long CoreHardRoom4ComponentInitializedOrdinaryStepR908 {get{return coreRoom4ComponentInitStepR908;}}

#if UNITY_EDITOR
  public void BeginCoreHardRoom4UiComponentDiagnosticR908(string path){
   PreviewRequire(!coreEntrySessionV21&&!forestPreviewActive&&!forestPreviewLoading,"R908 requires the admitted menu, not relocation of a running preview");
   BeginCoreHardOwnedEntryV21(path,true);
  }
#endif


#if UNITY_EDITOR
  public void BeginCoreHardControlledEntryV21(string path){
   BeginCoreHardOwnedEntryV21(path);
  }
#endif

  void ApplyCoreHardEntryIdentityV21(){if(!coreEntrySessionV21)return;forestPreviewDungeon.id="editor-core-hard-original-source-diagnostic";forestPreviewDungeon.title="ALTERA CORE HARD / SOURCE DIAGNOSTIC";forestPreviewDungeon.region="altera";forestPreviewDungeon.count=5;forestPreviewDungeon.boss="EL POWER PLANT / HARD";}

#if UNITY_EDITOR
  IEnumerator LoadCoreHardControlledEntryV21(){
   var raw=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(raw);coreEntryScopeV21=JsonUtility.FromJson<CoreEntryScopeV21>(Encoding.UTF8.GetString(raw));var b=coreEntryScopeV21;
   var check=InspectCoreHardControlledEntryV21(forestPreviewBundlePath);PreviewRequire(check.installedSourcesMatch,"Core candidate changed before loading");
   var rules=CoreEntryJsonV21<PreviewRules>(b.rules).definitions;
   CoreEntryFaultV21("before-source-model");game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(CoreCombatEffectDocumentV35(CoreEntryJsonV21<NpcEffectDocument>(b.effects)));game.SetNpcSoundDefinitions(CoreEntryJsonV21<NpcSoundDocument>(b.sounds));game.SetNpcMechanismDefinitions(CoreEntryJsonV21<NpcMechanismDocument>(b.npcMechanisms));
   game.SetCoreHardMechanismsV21(CoreEntryJsonV21<CoreHardMechanismDocumentV21>(b.mechanisms));
   game.SetCoreHardProtectionV21(CoreEntryJsonV21<CoreHardProtectionDocumentV21>(b.protection));game.SetCoreHardBattlePlaneV21(CoreEntryJsonV21<CoreHardBattlePlaneDocumentV21>(b.battlePlane));game.SetCoreHardCallbacksV21(CoreEntryJsonV21<CoreHardCallbackDocumentV21>(b.callbacks));game.SetCoreHardLocalBalanceV21(CoreEntryJsonV21<CoreHardBalanceDocumentV21>(b.balance));game.SetCoreHardClosureV21(CoreEntryJsonV21<CoreHardClosureDocumentV21>(b.closure));game.SetCoreHardLocalAddsV21(CoreEntryJsonV21<CoreHardLocalAddsDocumentV21>(b.localAdds));game.SetCoreHardEntryActivationV21(CoreEntryJsonV21<CoreHardEntryActivationDocumentV21>(b.activation));
   CoreEntryFaultV21("after-source-model");var model=CoreEntryJsonV21<PreviewModel>(b.model);forestPreviewStages=new StageArt[5];forestPreviewScenes=new NativeScene[5];forestPreviewGeometry=new byte[5][];var rooms=new List<RoomDefinition>();
   for(int i=0;i<5;i++){var room=model.rooms[i];var w=b.worlds[i];var scene=CoreEntryJsonV21<NativeScene>(w.scene);var p=CoreEntryJsonV21<PreviewProjection>(w.projection);scene.unit=p.unit;scene.offsetX=p.offsetX;scene.offsetY=p.offsetY;scene.path=p.path;forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=CoreEntryReadV21(w.binary);foreach(var t in w.textures)forestPreviewTextureBindings[t.sourcePath]=PreviewTextureFile(t.source);
    var music=Array.Find(b.music,m=>m.room==i);PreviewRequire(music!=null&&sceneMusicAssets.ContainsKey(music.id),"Core source BGM unavailable");
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=w.id,title="Core Hard / Source Room "+(i+1),width=p.width,height=p.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=w.id,bgm=music.id};
    var target=new RoomDefinition{id=room.id,title=forestPreviewStages[i].title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};foreach(var e in room.enemies)target.enemies.Add(e);rooms.Add(target);yield return null;
   }
   var shader=UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>("Assets/Editor/NativeReference.shader");PreviewRequire(shader,"Core exact original diagnostic shader missing");coreEntrySourceViewV21=new CoreHardOriginalSourceViewV21(actorsRoot,shader,npcEffectDepths);
   foreach(var a in b.actors){if(a.emptySourceBody||a.npc=="NUI_NASOD_KING"||a.npc=="NUI_EL_POWER_PLANT"||a.npc=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"||a.npc=="NUI_NASOD_WATCH"||a.npc=="NUI_NASOD_HEALER_BLADER"||a.npc=="NUI_NASOD_ELEMENT_FIRE_B"||a.npc=="NUI_RAVEN_CLONE_B"||a.npc=="NUI_NASOD_ELITE_GUARD"||a.npc=="NUI_NASOD_ELEMENT_ICE_B"||a.npc=="NUI_WALLY_9TH_TYPE2"||a.npc=="NUI_NASOD_SPRAY_ICE"||a.npc=="NUI_NASOD_ELITE_SHERIFF"||IsCoreAlarmWholeIdV21(a.npc)||IsCoreExtractorWholeIdV21(a.npc))continue;var textures=new Dictionary<string,Texture2D>();foreach(var p in a.textures){var t=PreviewTextureFile(p);t.wrapMode=TextureWrapMode.Repeat;textures.Add(p.path,t);}coreEntrySourceViewV21.Register(CoreEntryJsonV21<CoreEntrySourceIdentityV21>(a.identity),CoreEntryReadV21(a.geometry),CoreEntryReadV21(a.uvs),textures,Array.Find(rules,p=>p.id==a.npc));yield return null;}
   CoreEntryFaultV21("after-source-actors");ConfigureCoreAlarmWholeV21(b.alarmIndex,rules);ConfigureCoreExtractorWholeV21(b.extractorIndex,b.extractorHandoff,rules);ConfigureCorePlantWholeV21(b.plantWhole,b.plantReview,rules);ConfigureCoreEventBossWholeV21(b.eventWhole,b.eventReview,rules);ConfigureCoreWatchWholeV21(b.watchWhole,b.watchReview,rules);ConfigureCoreHealerWholeV21(b.healerWhole,b.healerReview,b.originalDisplayRules,rules);ConfigureCoreElementFireBWholeV21(b.elementFireBWhole,b.elementFireBReview,b.originalDisplayRules,rules);ConfigureCoreRavenBWholeV21(b.ravenBWhole,b.ravenBReview,b.originalDisplayRules,rules);ConfigureCoreGuardWholeV21(b.guardWhole,b.guardReview,b.originalDisplayRules,rules);ConfigureCoreElementIceBWholeV21(b.elementIceBWhole,b.elementIceBReview,b.originalDisplayRules,rules);ConfigureCoreWallyWholeR198(b.wallyWhole,b.wallyRegistryAcceptance,b.originalDisplayRules,rules);ConfigureCoreSprayWholeR203(b.sprayWhole,b.sprayRegistryAcceptance,b.originalDisplayRules,rules);ConfigureCoreSheriffWholeR227(b.sheriffWhole,b.sheriffRegistryAcceptance,b.originalDisplayRules,rules);
   ConfigureCoreReactionDisplayV32(rules);ConfigureFactorySheriffVisualV32();
   foreach(var a in b.audio){CoreEntryReadV21(a.file);using(var lease=CoreLifecycleAudioLeaseV24(UnityWebRequestMultimedia.GetAudioClip(new Uri(a.file.path).AbsoluteUri,AudioType.OGGVORBIS))){var request=lease.request;yield return request.SendWebRequest();CoreEntryFaultV21("during-audio");PreviewRequire(request.result==UnityWebRequest.Result.Success,"Core original audio decode failed "+a.id);var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip&&clip.loadState!=AudioDataLoadState.Failed,"Core original audio clip absent");AudioClip prior;if(audioClips.TryGetValue(a.id,out prior)){coreEntryPreviousAudioV21.Add(a.id,prior);audioClips.Remove(a.id);}RegisterNpcSoundClips(new Dictionary<string,AudioClip>{{a.id,clip}});forestPreviewSounds.Add(a.id);forestPreviewOwned.Add(clip);CoreEntryFaultV21("after-audio-register");}}
   CoreEntryFaultV21("before-fx");LoadCoreHardClosureVisualBindingV21(b.closureVisual.path,b.closureVisual.sha256);LoadCoreHardLocalAddVisualBindingV21(b.localAddVisual.path,b.localAddVisual.sha256);
   if(b.kingWhole!=null&&!string.IsNullOrEmpty(b.kingWhole.path))ConfigureCoreKingWholeLibraryV21(b.kingWhole.path,b.kingWhole.sha256,rules);
   CoreEntryFaultV21("after-fx");
   if(coreRoom4ComponentRequestedR908){
    // Loading remains true and this block has no yield: ordinary Update cannot run.
    // LocalBalance binding cleared the old balancedActor set before bridge registration.
    forestPreviewActive=true;PrepareCoreStopIntroBridgeDraftV21(CoreEntryJsonV21<CoreStopOwnerDocumentV21>(b.sourceStartOwner));
    game.InitializeCoreHardRoom4ComponentR908(rooms);
    coreRoom4ComponentInitializedR908=true;coreRoom4ComponentInitFrameR908=Time.frameCount;coreRoom4ComponentInitModelR908=game.elapsed;coreRoom4ComponentInitStepR908=game.CoreStopIntroModelStepV21;
   }else{game.SetRooms(rooms);forestPreviewActive=true;PrepareCoreStopIntroBridgeDraftV21(CoreEntryJsonV21<CoreStopOwnerDocumentV21>(b.sourceStartOwner));}
   MarkCoreHardEntryActiveV21();forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();Debug.Log("CORE_HARD_ORIGINAL_SOURCE_DIAGNOSTIC_READY rooms=5 births=85 states=372 artApproved=false naturalRoute=false");
  }
#endif

  void ApplyCoreHardEntryVisibilityV21(Enemy e,ref bool visible,ref bool gauge){if(!coreEntrySessionV21)return;string pendingInitialState;if(game.TryCoreHardSourceInitializationPendingV32(e,out pendingInitialState)){visible=false;gauge=false;return;}visible&=e.sourceNPC!="NUI_PD_MONSTER"&&game.CoreHardActorShownV21(e);ApplyCoreExtractorSourceVisibilityV21(e,ref visible);ApplyCorePlantSourceVisibilityV21(e,ref visible);ApplyCoreEventBossSourceVisibilityV21(e,ref visible);ApplyCoreWatchSourceVisibilityV21(e,ref visible);ApplyCoreHealerSourceVisibilityV21(e,ref visible);ApplyCoreRavenBSourceVisibilityV21(e,ref visible);ApplyCoreGuardSourceVisibilityV21(e,ref visible);ApplyCoreElementIceBSourceVisibilityV21(e,ref visible);ApplyCoreElementFireBSourceVisibilityV21(e,ref visible);if(e.sourceNPC=="NUI_NASOD_KING")gauge&=game.CoreHardCallbackRequestsGaugeV21(e);}
  bool TryRenderCoreHardOriginalSourceActorV21(Enemy e,SpriteRenderer sprite){
   if(!coreEntrySessionV21||!forestPreviewActive)return false;
   if(RenderCoreAlarmWholeV21(e,sprite)||RenderCoreExtractorWholeV21(e,sprite)||RenderCorePlantWholeV21(e,sprite)||RenderCoreEventBossWholeV21(e,sprite))return true;
   if(RenderCoreGuardWholeV21(e,sprite))return true;
   if(RenderCoreElementIceBWholeV21(e,sprite)){RecordCoreIceWholeStopFrameR390(e,sprite);return true;}
   if(RenderCoreWallyWholeR198(e,sprite))return true;
   if(RenderCoreSprayWholeR203(e,sprite))return true;
   if(RenderCoreSheriffWholeR227(e,sprite))return true;
   if(RenderCoreWatchWholeV21(e,sprite)||RenderCoreHealerWholeV21(e,sprite)||RenderCoreElementFireBWholeV21(e,sprite)||RenderCoreRavenBWholeV21(e,sprite)){RecordCoreFourWholeFrameV21(e,sprite);return true;}
   if(e.sourceNPC=="NUI_NASOD_KING"&&CoreKingWholeActorV21(e)){sprite.enabled=true;return false;}
   sprite.sprite=null;sprite.enabled=false;
   if(e.sourceNPC=="NUI_PD_MONSTER")return true;
   PreviewRequire(coreEntrySourceViewV21!=null&&coreEntrySourceViewV21.Render(e,RenderEnemyPosition(e),sprite.gameObject.activeSelf,(motion,clock)=>ObserveMonsterPixelV53(e,"Core/NativeSourceMesh",e.npcState,motion,clock,"pending","native-director-envelope-adapter-required")),"Core exact source actor has no diagnostic binding "+e.sourceNPC);return true;
  }
  void ReleaseCoreHardEntryRoomV21(){coreFourWholeFramesV21.Clear();coreFourWholeStartEnvelopesV22.Clear();if(coreEntrySourceViewV21!=null)coreEntrySourceViewV21.ReleaseRoom();}
  void DisposeCoreHardControlledEntryV21(){
   DisposeCoreReactionDisplayV32();DisposeFactorySheriffVisualV32();
   // End reuses the same DemoScene. Do not defer this cache/FX release to OnDestroy.
   try{DisposeCoreStopIntroBridgeDraftV21();}finally{game.SetCoreHardEntryActivationV21(null);}DisposeCoreHardEntryOwnedLoadV21();coreFourWholeFramesV21.Clear();coreFourWholeStartEnvelopesV22.Clear();DisposeCoreWallyWholeR198();DisposeCoreSprayWholeR203();DisposeCoreSheriffWholeR227();DisposeCoreElementIceBWholeV21();DisposeCoreGuardWholeV21();DisposeCoreRavenBWholeV21();DisposeCoreElementFireBWholeV21();DisposeCoreHealerWholeV21();DisposeCoreWatchWholeV21();DisposeCoreEventBossWholeV21();DisposeCorePlantWholeV21();DisposeCoreExtractorWholeV21();DisposeCoreAlarmWholeV21();ClearCoreKingWholeLibraryV21();DisposeCoreHardClosureVisualV21();game.SetCoreHardEntryActivationV21(null);game.SetCoreHardMechanismsV21(null);
   if(coreEntrySourceViewV21!=null)coreEntrySourceViewV21.Dispose();coreEntrySourceViewV21=null;coreEntryScopeV21=null;coreEntrySessionV21=false;
  }
  void RestoreCoreHardEntryAudioV21(){foreach(var p in coreEntryPreviousAudioV21)audioClips[p.Key]=p.Value;coreEntryPreviousAudioV21.Clear();}
 }
}

