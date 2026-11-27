#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ElswordPixel {
 public sealed partial class DemoScene {
#pragma warning disable 0649 // JSON DTO fields are populated by Unity JsonUtility.
  [Serializable] sealed class PreviewFile {public string path,sha256;}
  [Serializable] sealed class PreviewSprite {public string id,sourceNPC,role;public PreviewFile animations,atlas;}
  [Serializable] sealed class PreviewTexture {public string sourcePath;public PreviewFile source,selected;public bool singleMaterialTrial;}
  [Serializable] sealed class PreviewWorld {public string id;public PreviewFile scene,projection,binary;public PreviewTexture[] textures;}
  [Serializable] sealed class PreviewAudio {public string id;public PreviewFile file;}
  [Serializable] sealed class PreviewBundle {
   public string schema,sourceDungeon,inheritedArtEffectsSha256,loadedEffectsSha256;public bool editorOnly,runtimeAdmitted,playableReady;public string[] blockers;
   public PreviewFile model,artManifest,rules,effects,sounds,soundMap,mechanisms,admittedCatalog,materialOverrides;
   public PreviewFile[] reviewEvidence;public PreviewSprite[] sprites,visualSprites;public PreviewWorld[] worlds;public PreviewAudio[] audio;
   public PreviewFile visualManifest,steamVisual,outskirtsVisual;
   public int originalEntityCount,originalCombatCount;public string[] boundaries,reusedAdmittedVisuals;
  }
  [Serializable] sealed class PreviewRules {public NpcDefinition[] definitions;}
  [Serializable] sealed class PreviewSourceBinding {public string sourceRulesSha256;}
  [Serializable] sealed class PreviewModel {public bool runtimeAdmitted;public PreviewRoom[] rooms;}
  [Serializable] sealed class PreviewActivationModel {public PreviewActivationRoom[] rooms;}
  [Serializable] sealed class PreviewActivationRoom {public PreviewActivationSpawn[] enemies;}
  [Serializable] sealed class PreviewActivationSpawn {public string sourceNPC;public bool sourceActive;}
  [Serializable] sealed class PreviewRoom {
   public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;
   public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;public string[] bgm;
  }
  [Serializable] sealed class PreviewProjection {public string id;public float unit,offsetX,offsetY,width,height;public NativePath[] path;}
  [Serializable] sealed class PreviewAtlas {public string image;public int frameWidth,frameHeight;public float[] groundAnchor;}
  [Serializable] sealed class PreviewFrame {public int[] rect;public float[] anchor;}
  [Serializable] sealed class PreviewClip {public string id;public float fps,durationSeconds;public bool loop;public PreviewFrame[] frames;}
  [Serializable] sealed class PreviewLibrary {
   public string id,sourceNPC,artVersion,sourceRulesSha256;public bool runtimeAdmitted,animationLibraryApproved;
   public float logicalScale,standingHeight;public int paintedPoseCount;public PreviewAtlas atlas;public PreviewClip[] clips;
  }
  [Serializable] sealed class PreviewArtManifest {public string sourceRulesSha256,effectsSha256;public bool runtimeAdmitted;}
  [Serializable] sealed class PreviewCapture {
   public string scope="Editor-only source dungeon preview; candidate pixel actors over source scenery. Unmapped effects retain diagnostic lines. Not production admission",bundlePath,bundleSha256,world;
   public int room,entities,hits,kills,npcSoundsPlayed,npcSoundsMissing;public float elapsed,playerX,playerY,hp;public bool victory;
  }
#pragma warning restore 0649
  bool forestPreviewActive,forestPreviewLoading,cavePreviewSession,belowPassagePreviewSession,outskirtsPreviewSession,castleCenterPreviewSession;
  Coroutine forestPreviewRoutine;
  string forestPreviewBundlePath,forestPreviewBundleHash,forestPreviewError="";
  int forestPreviewOriginalDungeon;
  string[] forestPreviewOriginalLoadout;
  StageArt[] forestPreviewStages;
  NativeScene[] forestPreviewScenes;
  byte[][] forestPreviewGeometry;
  readonly DungeonArt forestPreviewDungeon=new DungeonArt{id="editor-forest-preview",title="DARK FOREST / PREVIEW",region="elder",count=3};
  readonly Dictionary<string,byte[]> forestPreviewFiles=new Dictionary<string,byte[]>();
  readonly Dictionary<string,string> forestPreviewHashes=new Dictionary<string,string>();
  readonly Dictionary<string,Texture2D> forestPreviewTextures=new Dictionary<string,Texture2D>();
  readonly Dictionary<string,Texture2D> forestPreviewTextureBindings=new Dictionary<string,Texture2D>();
  readonly List<UnityEngine.Object> forestPreviewOwned=new List<UnityEngine.Object>();
  readonly List<string> forestPreviewLibraries=new List<string>(),forestPreviewAtlasKeys=new List<string>(),forestPreviewSounds=new List<string>();
  readonly List<LineRenderer> forestPreviewLines=new List<LineRenderer>();
  Text forestPreviewNotice;
  public bool ForestPreviewActive {get{return forestPreviewActive;}}
  public bool ForestPreviewLoading {get{return forestPreviewLoading;}}
  public string ForestPreviewError {get{return forestPreviewError;}}
  public GameModel ForestPreviewGame {get{return forestPreviewActive?game:null;}}
  static void PreviewRequire(bool condition,string message){if(!condition)throw new InvalidDataException("Source dungeon preview: "+message);}
  static string PreviewHash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  byte[] PreviewRead(PreviewFile file){
   if(CoreProductResourcesActiveR1147)return CoreEntryReadV21(file);
   PreviewRequire(file!=null&&!string.IsNullOrEmpty(file.path)&&!string.IsNullOrEmpty(file.sha256),"missing pinned dependency");
   string path=Path.GetFullPath(file.path);byte[] bytes;if(UnifiedSourceStageV21.TryReadHistoricalSource(path,file.sha256,out bytes)||UnifiedSourceStageV21.TryReadHistoricalResource(path,file.sha256,out bytes))return bytes;
   if(forestPreviewFiles.TryGetValue(path,out bytes)){PreviewRequire(forestPreviewHashes[path]==file.sha256,"conflicting dependency hash "+path);return bytes;}
   var info=new FileInfo(path);PreviewRequire(info.Exists&&info.Length<=64*1024*1024,"missing/oversized dependency "+path);
   bytes=File.ReadAllBytes(path);PreviewRequire(PreviewHash(bytes)==file.sha256,"dependency changed "+path);
   forestPreviewFiles.Add(path,bytes);forestPreviewHashes.Add(path,file.sha256);return bytes;
  }
  T PreviewJson<T>(PreviewFile file){return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(PreviewRead(file)));}
  Texture2D PreviewTextureFile(PreviewFile file){
   Texture2D result;if(forestPreviewTextures.TryGetValue(file.path,out result))return result;
   result=new Texture2D(2,2,TextureFormat.RGBA32,false){name="EDITOR PREVIEW / "+Path.GetFileName(file.path),filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   forestPreviewOwned.Add(result);PreviewRequire(result.LoadImage(PreviewRead(file)),"PNG decode failed "+file.path);forestPreviewTextures.Add(file.path,result);return result;
  }
  Texture2D ForestPreviewTexture(string source){Texture2D texture;PreviewRequire(forestPreviewTextureBindings.TryGetValue(source,out texture),"unbound original scene texture "+source);return texture;}
  void LoadForestPreviewSprite(PreviewSprite entry,PreviewLibrary raw){
   PreviewRequire(raw!=null&&raw.id==entry.id&&!raw.runtimeAdmitted&&!raw.animationLibraryApproved,"invalid preview visual identity/admission");
   PreviewRequire(!libraries.ContainsKey(raw.id),"candidate may not replace admitted sprite "+raw.id);
   PreviewRequire(raw.logicalScale>0&&raw.logicalScale<=8&&raw.atlas!=null&&raw.atlas.groundAnchor!=null&&raw.atlas.groundAnchor.Length==2&&raw.clips!=null&&raw.clips.Length>0,"invalid candidate atlas metadata");
   string textureKey="editor-forest/"+raw.id;var texture=PreviewTextureFile(entry.atlas);PreviewRequire(texture.width<=4096&&texture.height<=4096,"candidate atlas exceeds 4096");
   atlasTextures.Add(textureKey,texture);forestPreviewAtlasKeys.Add(textureKey);
   var clips=new List<AnimationData>();var ids=new HashSet<string>();foreach(var source in raw.clips){
    PreviewRequire(!string.IsNullOrEmpty(source.id)&&ids.Add(source.id)&&source.fps>0&&source.durationSeconds>0&&source.frames!=null&&source.frames.Length>0,"invalid candidate clip");
    var frames=new List<FrameData>();foreach(var f in source.frames){
     PreviewRequire(f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2,"invalid candidate frame");
     PreviewRequire(f.rect[0]>=0&&f.rect[1]>=0&&f.rect[2]>0&&f.rect[3]>0&&f.rect[0]+f.rect[2]<=texture.width&&f.rect[1]+f.rect[3]<=texture.height,"candidate frame outside atlas");
     frames.Add(new FrameData{x=f.rect[0],y=f.rect[1],w=f.rect[2],h=f.rect[3],hasAnchor=true,anchorX=f.anchor[0],anchorY=f.anchor[1]});
    }
    clips.Add(new AnimationData{id=source.id,fps=source.fps,duration=source.durationSeconds,loop=source.loop,frames=frames.ToArray()});
   }
   MakeLibrary(new SpriteLibraryData{id=raw.id,texture=textureKey,artVersion=raw.artVersion,paintedPoseCount=raw.paintedPoseCount,standingHeight=raw.standingHeight,logicalScale=raw.logicalScale,frameWidth=raw.atlas.frameWidth,frameHeight=raw.atlas.frameHeight,anchorX=raw.atlas.groundAnchor[0],anchorY=raw.atlas.groundAnchor[1],clips=clips.ToArray()});forestPreviewLibraries.Add(raw.id);
   foreach(var list in libraries[raw.id].frames.Values)foreach(var sprite in list)forestPreviewOwned.Add(sprite);
  }
  void EnsureForestPreviewNotice(){
   if(forestPreviewNotice)return;
   forestPreviewNotice=Label("Editor Forest Preview Notice","",new Rect(6,529,948,11),8,new Color(1,.86f,.45f),TextAnchor.MiddleCenter);
   forestPreviewNotice.transform.SetAsLastSibling();forestPreviewOwned.Add(forestPreviewNotice.gameObject);
  }
  public void BeginForestPreview(string bundlePath){BeginSourceDungeonPreview(bundlePath,false);}
  void BeginSourceDungeonPreview(string bundlePath,bool cave,bool belowPassage=false,bool outskirts=false,bool castleCenter=false,bool dragonRoad=false,bool lake=false,bool lakeNight=false,bool dragonNest=false){
   RequireDevelopmentContent();
   if(!Application.isPlaying)throw new InvalidOperationException("Source preview requires Editor Play Mode");
   if(forestPreviewActive||forestPreviewLoading)EndForestPreview();
   dragonNestPreviewSession=dragonNest;lakeNightPreviewSession=lakeNight;lakePreviewSession=lake;dragonRoadPreviewSession=dragonRoad;castleCenterPreviewSession=castleCenter;cavePreviewSession=cave;belowPassagePreviewSession=belowPassage;outskirtsPreviewSession=outskirts;forestPreviewDungeon.id=dragonNest?"editor-besma-dragon-nest-preview":lakeNight?"editor-besma-lake-night-preview":lake?"editor-besma-lake-preview":dragonRoad?"editor-dragon-road-preview":castleCenter?"editor-castle-center-preview":outskirts?"editor-outskirts-preview":belowPassage?"editor-below-passage-preview":cave?"editor-cave-preview":"editor-forest-preview";forestPreviewDungeon.title=dragonNest?"DRAGON NEST / PREVIEW":lakeNight?"BESMA LAKE NIGHT / PREVIEW":lake?"BESMA LAKE / PREVIEW":dragonRoad?"DRAGON ROAD / PREVIEW":castleCenter?"WALLY CASTLE CENTER / PREVIEW":outskirts?"WALLY CASTLE OUTSKIRTS / PREVIEW":belowPassage?"UNDERGROUND WATERWAY / PREVIEW":cave?"BANTHUS CAVE / PREVIEW":"DARK FOREST / PREVIEW";forestPreviewDungeon.count=castleCenter?2:3;forestPreviewDungeon.boss=dragonNest?"LIZARDMAN SHAMAN KING":lakeNight?"DARK LIZARDMAN WARRIOR":lake?"LIZARDMAN OLDER":dragonRoad?"LIZARDMAN WARRIORS":castleCenter?"WALLY NO.8":"";forestPreviewDungeon.region=dragonNest||lakeNight||lake||dragonRoad?"bethma":"elder";
   ApplyAlteraAirshipPreviewIdentityV20();
   ApplyMine2PreviewIdentityV21();
   ApplyBelowTunnelEntryIdentityV21();ApplyCargoEntryIdentityV21();ApplyCoreHardEntryIdentityV21();ApplyFactoryHostIdentityR657();
   forestAutoFinished=false;
   PreviewRequire(!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview,"cannot combine with automated production review");
   forestPreviewOriginalDungeon=selectedDungeon;forestPreviewOriginalLoadout=new string[GameModel.SkillSlotCount];for(int i=0;i<GameModel.SkillSlotCount;i++)forestPreviewOriginalLoadout[i]=game.GetEquippedSkill(i).id;
   forestPreviewBundlePath=Path.GetFullPath(bundlePath);forestPreviewError="";forestPreviewLoading=true;MarkCoreHardEntryLoadOwnedV21();game.paused=true;ClearInput();StopMapMusicPreview();StopAudioVoices();EnsureForestPreviewNotice();
   StartCoreAwareForestPreviewRoutineV24();
  }
  IEnumerator ForestPreviewLoadGuard(){
   var work=factoryHostSessionR657?LoadFactoryPreparedHostR657():coreEntrySessionV21?CreateCoreHardEntryOwnedLoadV21():cargoEntrySessionV21?LoadCargoControlledEntryV21():belowTunnelEntrySessionV21?LoadBelowTunnelEntryV21():mine2PreviewSessionV21?LoadMine2PreviewV21():alteraAirshipPreviewSessionV20?LoadAlteraAirshipPreviewV20():dragonNestPreviewSession?LoadDragonNestPreviewV20():lakeNightPreviewSession?LoadLakeNightPreview():lakePreviewSession?LoadLakePreview():dragonRoadPreviewSession?LoadDragonRoadPreview():castleCenterPreviewSession?LoadCastleCenterPreview():outskirtsPreviewSession?LoadOutskirtsPreview():belowPassagePreviewSession?LoadBelowPassagePreview():cavePreviewSession?LoadCavePreview():LoadForestPreview();work=WrapUnifiedSourceLoadV21(work);Exception error=null;
   try {while(true){bool more=false;object yielded=null;try{more=work.MoveNext();if(more)yielded=work.Current;}catch(Exception ex){error=ex;}
    if(error!=null||!more)break;yield return yielded;
   }}finally{(work as IDisposable)?.Dispose();}
   forestPreviewRoutine=null;
   if(error!=null){forestPreviewError=error.ToString();Debug.LogError(SourcePreviewLogPrefix+"_PREVIEW_REJECTED "+forestPreviewError);EndForestPreview();}else CompleteCoreLifecycleOwnedGuardV24();
  }
  IEnumerator LoadForestPreview(){
   byte[] manifestBytes=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(manifestBytes);
   var bundle=JsonUtility.FromJson<PreviewBundle>(Encoding.UTF8.GetString(manifestBytes));
   PreviewRequire(bundle!=null&&bundle.schema=="forest-editor-preview-v1"&&bundle.editorOnly&&!bundle.runtimeAdmitted,"wrong session schema/admission boundary");
   PreviewRequire(bundle.worlds.Length==3&&bundle.sprites.Length==5&&bundle.audio.Length==21&&bundle.originalEntityCount==26,"wrong fixed forest scope");
   PreviewRead(bundle.admittedCatalog);PreviewRequire(PreviewHash(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes)==bundle.admittedCatalog.sha256,"running admitted catalog differs from pinned baseline");
   PreviewRead(bundle.materialOverrides);PreviewRead(bundle.soundMap);foreach(var f in bundle.reviewEvidence)PreviewRead(f);
   var artManifest=PreviewJson<PreviewArtManifest>(bundle.artManifest);
   PreviewRequire(!artManifest.runtimeAdmitted&&artManifest.sourceRulesSha256==bundle.rules.sha256&&artManifest.effectsSha256==bundle.inheritedArtEffectsSha256&&bundle.loadedEffectsSha256==bundle.effects.sha256,"art/sidecar provenance differs");
   var rules=PreviewJson<PreviewRules>(bundle.rules).definitions;var effects=PreviewJson<NpcEffectDocument>(bundle.effects);
   var sounds=PreviewJson<NpcSoundDocument>(bundle.sounds);var mechanisms=PreviewJson<NpcMechanismDocument>(bundle.mechanisms);
   PreviewRequire(rules.Length==5&&!effects.runtimeAdmitted&&!sounds.runtimeAdmitted&&!mechanisms.runtimeAdmitted,"candidate flags changed");
   PreviewRequire(PreviewJson<PreviewSourceBinding>(bundle.sounds).sourceRulesSha256==bundle.rules.sha256&&mechanisms.sourceRulesSha256==bundle.rules.sha256,"sidecar source rule mismatch");
   var allowedNPCs=new HashSet<string>{"NUI_FOREST_DARK_MAGICIAN","NUI_CURSE_WISP","NUI_ANGER_FOREST_KNIGHT","NUI_ANGER_FOREST_FAIRY","NUI_ANGER_FOREST_ENT"};
   foreach(var entry in bundle.sprites){
    var raw=PreviewJson<PreviewLibrary>(entry.animations);PreviewRequire(allowedNPCs.Remove(entry.sourceNPC)&&raw.id==entry.id&&raw.sourceNPC==entry.sourceNPC&&!raw.runtimeAdmitted&&!raw.animationLibraryApproved&&raw.sourceRulesSha256==bundle.rules.sha256,"unexpected candidate sprite identity");
    LoadForestPreviewSprite(entry,raw);
   }
   PreviewRequire(allowedNPCs.Count==0,"missing source NPC identity");
   if(bundle.visualSprites!=null&&bundle.visualSprites.Length>0){
    PreviewRead(bundle.visualManifest);var allowedVisuals=new HashSet<string>();foreach(var definition in effects.definitions)if(!string.IsNullOrEmpty(definition.visualId))allowedVisuals.Add(definition.visualId);
    foreach(var entry in bundle.visualSprites){var raw=PreviewJson<PreviewLibrary>(entry.animations);
     if(entry.role=="gate")PreviewRequire(entry.id=="elder-druid-forest-gate"&&entry.sourceNPC=="NUI_ELDER_DRUID_FOREST_GATE"&&raw.sourceNPC==entry.sourceNPC,"unexpected forest gate identity");
     else PreviewRequire(entry.role=="effect"&&allowedVisuals.Remove(entry.id),"unexpected forest effect identity");
     LoadForestPreviewSprite(entry,raw);var library=libraries[entry.id];
     PreviewRequire(entry.role=="gate"?library.frames.ContainsKey("idle")&&library.frames.ContainsKey("death"):library.frames.ContainsKey("play"),"preview visual has no required clip "+entry.id);
    }
   }
   foreach(var rule in rules){PreviewRequire(!rule.runtimeAdmitted&&libraries.ContainsKey(rule.kind),"rule has no candidate art");foreach(var state in rule.states)PreviewRequire(libraries[rule.kind].frames.ContainsKey(state.clip),"state would fall back to idle: "+rule.id+"/"+state.clip);}
   var model=PreviewJson<PreviewModel>(bundle.model);PreviewRequire(!model.runtimeAdmitted&&model.rooms.Length==3,"wrong source route");
   var activation=PreviewJson<PreviewActivationModel>(bundle.model);int enabledAI=0,inactiveAI=0;
   forestPreviewStages=new StageArt[3];forestPreviewScenes=new NativeScene[3];forestPreviewGeometry=new byte[3][];
   var rooms=new List<RoomDefinition>();int entities=0,combat=0,gates=0,boxes=0;
   for(int i=0;i<3;i++){
    var room=model.rooms[i];var world=bundle.worlds[i];var scene=PreviewJson<NativeScene>(world.scene);var projection=PreviewJson<PreviewProjection>(world.projection);var bytes=PreviewRead(world.binary);
    PreviewRequire(room.sourceWorldId==world.id&&scene.id==world.id&&projection.id==world.id&&projection.path.Length>0&&Mathf.Abs(projection.unit-.28f)<.000001f,"source scene/projection mismatch");
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;
    foreach(var binding in world.textures){PreviewRead(binding.source);var texture=PreviewTextureFile(binding.selected);forestPreviewTextureBindings[binding.sourcePath]=texture;}
    foreach(var part in scene.parts)PreviewRequire(part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=bytes.Length&&(string.IsNullOrEmpty(part.texture)||forestPreviewTextureBindings.ContainsKey(part.texture)),"incomplete scene geometry/texture binding");
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=bytes;
    string music=Path.GetFileNameWithoutExtension(room.bgm[0]).ToLowerInvariant();PreviewRequire(music=="music063_stage"||music=="music013_boss","unexpected forest BGM");PreviewRequire(sceneMusicAssets.ContainsKey(music),"admitted BGM unavailable "+music);
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=world.id,title="Dark Forest / "+(i==2?"Ent Grove":i==0?"Entrance":"Deep Woods"),width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=world.id,bgm=music};
    var target=new RoomDefinition{id=room.id,title=forestPreviewStages[i].title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};
    for(int j=0;j<room.enemies.Length;j++){var enemy=room.enemies[j];var sourceActivation=activation.rooms[i].enemies[j];PreviewRequire(sourceActivation.sourceNPC==enemy.sourceNPC,"source activation identity mismatch");
     enemy.aiEnabled=ForestIterationActivation.ShouldEnable(enemy.sourceNPC,sourceActivation.sourceActive,enemy.countsForClear);
     if(ForestIterationActivation.IsForestNpc(enemy.sourceNPC)){if(enemy.aiEnabled)enabledAI++;else inactiveAI++;}
     if(enemy.sourceNPC=="NUI_ELDER_DRUID_FOREST_GATE"){PreviewRequire(enemy.isGate&&enemy.kind=="elder-druid-forest-gate","gate identity/clear condition changed");gates++;}
     else if(enemy.sourceNPC=="NUI_BOX"){PreviewRequire(enemy.kind=="prop-box"&&libraries.ContainsKey("prop-box"),"box must reuse its exact admitted identity");boxes++;}
     else {PreviewRequire(Array.Exists(rules,r=>r.id==enemy.sourceNPC&&r.kind==enemy.kind),"unbound original spawn "+enemy.sourceNPC);combat++;}
     target.enemies.Add(enemy);entities++;
    }rooms.Add(target);
   }
   PreviewRequire(entities==26&&combat==22&&gates==2&&boxes==2,"original route roster changed");
   PreviewRequire(enabledAI==20&&inactiveAI==2,"forest source ACTIVE initialization changed");
   var clipsLoaded=new Dictionary<string,AudioClip>();foreach(var audio in bundle.audio){
    PreviewRead(audio.file);PreviewRequire(Array.Exists(sounds.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"unbound source sound");
    using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(audio.file.path)).AbsoluteUri,AudioType.OGGVORBIS)){
     yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"Ogg decode failed "+audio.id+" "+request.error);
     var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip!=null,"empty Ogg "+audio.id);clip.name=audio.id;forestPreviewOwned.Add(clip);clip.LoadAudioData();while(clip.loadState==AudioDataLoadState.Loading)yield return null;PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"Ogg data unavailable "+audio.id);clipsLoaded.Add(audio.id,clip);
    }
   }
   RegisterNpcSoundClips(clipsLoaded);forestPreviewSounds.AddRange(clipsLoaded.Keys);
   game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(effects);game.SetNpcSoundDefinitions(sounds);game.SetNpcMechanismDefinitions(mechanisms);game.SetRooms(rooms);
   forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;
   ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();
   Debug.Log("FOREST_PREVIEW_READY worlds=3 entities=26 combat=22 sourceActiveAI=20 originalInactiveAI=2 sourceSounds=21 candidateLibraries="+forestPreviewLibraries.Count+" bundle="+forestPreviewBundleHash+" runtimeAdmitted=false");
  }
  public void EndForestPreview(){
   if(factoryHostSessionR657){EndFactoryPreparedHostR657();return;}
   if(!forestPreviewActive&&!forestPreviewLoading)return;
   game.SetCoreHardMechanismsV21(null);string[] loadout=forestPreviewOriginalLoadout;int selected=forestPreviewOriginalDungeon;string prefix=SourcePreviewLogPrefix;
   DisposeAlteraAirshipPreviewV20();DisposeNativeWorld();DisposeForestPreview();selectedDungeon=Mathf.Clamp(selected,0,catalog.dungeons.Length-1);previewDungeon=selectedDungeon;
   game.SetNpcSoundDefinitions(null);game.SetNpcMechanismDefinitions(null);ConfigureRooms();if(loadout!=null)for(int i=0;i<loadout.Length;i++)game.EquipSkill(i,loadout[i]);
   adventureStarted=false;openMenu="";menuOrigin="";pausedBeforeMenu=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();SelectRuntimeDungeonOnMap(selectedDungeon);SetMenu("dungeons");
   Debug.Log(prefix+"_PREVIEW_EXIT admittedDungeons="+catalog.dungeons.Length+" candidateAdmissionUnchanged=true");
  }
  void DisposeForestPreview(){try{try{ReleaseFactoryHostWarningR657();}finally{StopCoreHardNaturalInputV21();DisposeUnifiedSourceLoadSessionV21();
   if(forestAutoArmed||forestAutoRunning)FinishForestAutoRun("preview-exited","Preview session was closed by the user or Editor",false);
   if(forestPreviewRoutine!=null){StopCoroutine(forestPreviewRoutine);forestPreviewRoutine=null;}
   DisposeCoreHardControlledEntryV21();
   DisposeCargoControlledEntryV21();
   DisposeBelowTunnelEntryV21();
   DisposeMine2PreviewV21();
   DisposeAlteraAirshipPreviewV20();
   forestPreviewActive=false;forestPreviewLoading=false;cavePreviewSession=false;belowPassagePreviewSession=false;outskirtsPreviewSession=false;castleCenterPreviewSession=false;dragonRoadPreviewSession=false;lakePreviewSession=false;lakeNightPreviewSession=false;dragonNestPreviewSession=false;dragonNestSourceVisualConfiguredV20=false;StopAudioVoices();DisposeBelowPassageSteam();DisposeOutskirtsNpcVisual();
   foreach(var id in forestPreviewSounds)audioClips.Remove(id);forestPreviewSounds.Clear();
   foreach(var id in forestPreviewLibraries)libraries.Remove(id);forestPreviewLibraries.Clear();
   foreach(var key in forestPreviewAtlasKeys)atlasTextures.Remove(key);forestPreviewAtlasKeys.Clear();
   foreach(var line in forestPreviewLines)if(line)Destroy(line.gameObject);forestPreviewLines.Clear();
   foreach(var item in forestPreviewOwned){if(item is Sprite sprite)spriteOffsets.Remove(sprite);if(item)Destroy(item);}forestPreviewOwned.Clear();
   forestPreviewFiles.Clear();forestPreviewHashes.Clear();forestPreviewTextures.Clear();forestPreviewTextureBindings.Clear();forestPreviewNotice=null;
   forestPreviewScenes=null;forestPreviewGeometry=null;forestPreviewStages=null;RestoreCargoEntryLibrariesV21();RestoreCoreHardEntryAudioV21();
   }}finally{ReleaseFactoryHostBindingsR657();}
  }
  void RenderForestPreviewStatus(){
   if(!forestPreviewActive&&!forestPreviewLoading)return;EnsureForestPreviewNotice();
   forestPreviewNotice.text=forestPreviewLoading?"LOADING...":DisplayDungeonTitleR1111()+"  |  "+(game.room+1)+" / "+forestPreviewStages.Length;
   if(forestPreviewActive){TextValue("Zone",DungeonRegion(Dungeon).ToUpperInvariant()+" / STAGE "+(game.room+1).ToString("D2"));if(openMenu=="loadout")TextValue("Menu Subtitle","Select skills; K continues, Tab returns to the map.");}
  }
  LineRenderer ForestPreviewLine(int index,Color color){
   while(forestPreviewLines.Count<=index)forestPreviewLines.Add(CombatLine("EDITOR PREVIEW / real model helper",.8f,color));
   var line=forestPreviewLines[index];line.enabled=true;line.startColor=line.endColor=color;return line;
  }
  void RenderForestPreviewHelpers(){
   int used=0;
   foreach(var effect in game.npcEffects){if(coreEntrySessionV21||cargoEntrySessionV21||belowTunnelEntrySessionV21||mine2PreviewSessionV21&&mine2RuntimeVisualConfiguredV21||alteraAirshipPreviewSessionV20&&alteraAirshipRuntimeVisualConfiguredV20||dragonNestPreviewSession&&dragonNestSourceVisualConfiguredV20||(lakeNightPreviewSession||lakePreviewSession||outskirtsPreviewSession||dragonRoadPreviewSession)&&outskirtsNpcVisual!=null)continue;var art=game.NpcEffectVisual(effect);if(!string.IsNullOrEmpty(art.id)&&libraries.ContainsKey(art.id))continue;var spheres=game.NpcEffectSpheres(effect);Color color=effect.reflected?new Color(.2f,1,1,.95f):game.NpcEffectCanDamage(effect)?new Color(1,.25f,.3f,.95f):new Color(.65f,.72f,.78f,.65f);
    for(int s=0;s<spheres.Length;s+=4){float r=spheres[s+3];if(r<=0)continue;var line=ForestPreviewLine(used++,color);line.positionCount=33;float depth=(effect.owner==null?0:effect.owner.nativeDepth)+spheres[s+2]/.28f;
     for(int j=0;j<=32;j++){float angle=j*Mathf.PI/16;line.SetPosition(j,ProjectNativePoint(new Vector3(spheres[s]+Mathf.Cos(angle)*r,spheres[s+1]+Mathf.Sin(angle)*r,0),depth));}
    }
   }
   for(int i=0;i<game.enemies.Count;i++){var enemy=game.enemies[i];if(!enemy.isGate)continue;if(libraries.ContainsKey(enemy.kind)){enemyViews[i].enabled=true;continue;}enemyViews[i].enabled=false;enemyDebuffViews[i].text=enemy.active&&enemy.hp>0?"GATE / DIAGNOSTIC":"";
    if(!enemy.active||enemy.hp<=0)continue;var line=ForestPreviewLine(used++,new Color(1,.75f,.2f,.95f));line.positionCount=5;
    var p=new[]{new Vector3(enemy.x-18,enemy.y,0),new Vector3(enemy.x-18,enemy.y+80,0),new Vector3(enemy.x+18,enemy.y+80,0),new Vector3(enemy.x+18,enemy.y,0),new Vector3(enemy.x-18,enemy.y,0)};
    for(int j=0;j<5;j++)line.SetPosition(j,ProjectNativePoint(p[j],enemy.nativeDepth));
   }
   if(belowPassagePreviewSession)RenderBelowPassageHelpers(ref used);
   if(dragonRoadPreviewSession)RenderDragonRoadHelpers(ref used);
   for(int i=used;i<forestPreviewLines.Count;i++)forestPreviewLines[i].enabled=false;
  }
  void SaveForestPreviewCapture(string name){
   var data=new PreviewCapture{bundlePath=forestPreviewBundlePath,bundleSha256=forestPreviewBundleHash,world=CurrentStageArt.sourceWorldId,room=game.room,entities=game.enemies.Count,hits=game.hits,kills=game.kills,elapsed=game.elapsed,playerX=game.player.x,playerY=game.player.y,hp=game.player.hp,victory=game.victory,npcSoundsPlayed=NpcAudioPlayed,npcSoundsMissing=NpcAudioMissing};
   File.WriteAllText(Path.Combine(captureDir,name+".preview.json"),JsonUtility.ToJson(data,true));
  }
 }
}
#endif
