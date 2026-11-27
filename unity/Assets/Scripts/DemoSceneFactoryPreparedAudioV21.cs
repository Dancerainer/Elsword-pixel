using System;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  public int FactoryPreparedSoundConsumedV21 {get;private set;}
  public int FactoryPreparedSoundPlayedV21 {get;private set;}
  public int FactoryPreparedSoundMutedV21 {get;private set;}
  public int FactoryPreparedSoundCulledV21 {get;private set;}
  // Explicit component consumer, not called by Update/TickAudio or any menu.
  // A future Factory Begin must stage these exact source IDs with the existing
  // RegisterNpcSoundClips lifecycle before calling this once after model Step.
  // The original clip provenance/decode is a separate binding prerequisite.
  public void ConsumeFactoryPreparedAudioV21(FactoryPreparedComponentV21 component) {
   if(component==null)throw new ArgumentNullException("component");
   if(component.IsDisposed)throw new ObjectDisposedException("FactoryPreparedComponentV21");
   if(!audioInitialized||!AudioManifestReady)throw new InvalidOperationException("Factory source audio requires the initialized original voice pool");
   // Missing assets fail before advancing the component's source serial. Never
   // call a missing clip a successful consume/play, or substitute another cue.
   foreach(var id in new[]{"AlteraCutScene_LiftMove.ogg","AlteraCutScene_RobotFall.ogg","AlteraCutScene_RobotAssemble.ogg"}) {
    AudioClip clip;if(!audioClips.TryGetValue(id,out clip)||!clip)throw new InvalidOperationException("Unbound original Factory source sound: "+id);
   }
   if(game.paused||component.Model.paused)return;
   component.ConsumeCurrentDirectorSounds(request=>{
    FactoryPreparedSoundConsumedV21++;
    // Muting drains observed requests, matching ConsumeNpcSoundEvents, so that
    // unmuting does not replay a source cue whose time has already passed.
    if(audioMuted){FactoryPreparedSoundMutedV21++;return;}
    int before=AudioPlayCount;
    // Explicit display audio adapter: original argument 100 -> gain 1; retain
    // the existing pool's limiter, voice priority, mix and culling behavior.
    PlayNativeSound(new TimedSound{sound=request.asset,volume=request.intArgument/100f},140);
    if(AudioPlayCount>before)FactoryPreparedSoundPlayedV21++;else FactoryPreparedSoundCulledV21++;
   });
  }
 }
}

#if UNITY_EDITOR
namespace ElswordPixel {
 using System.Collections;
 using System.Collections.Generic;
 using System.IO;
 using System.Text;
 using UnityEngine.Networking;
 using UnityEngine.UI;
 public sealed partial class DemoScene {
#pragma warning disable 0649
  [Serializable] sealed class FactoryHostBindingsR657 {
   public string schema,sourceDungeon;public bool bound,runtimeAdmitted;
   public PreviewFile sourceModel,fullRules,fullMechanisms,populationPolicy,eventBinding,entryActivation,sourceController,pressPolicy;
   public PreviewBundle presentation;public StageArt[] stages;public PreviewFile wholeBanks,nineReferenceBanks;
  }
#pragma warning restore 0649
  sealed class FactoryHostSnapshotR657 {
   internal GameModel game;internal bool paused,adventure,pausedMenu;internal int selected,preview,mapSelected,mapPage;internal string menu,origin,region;
   internal FactoryMusicSnapshotR742 music;internal float accumulator,aspect,size,fov,near,far;internal bool ortho;internal Rect rect;internal Vector3 position;internal Quaternion rotation;internal Matrix4x4 projection;internal RenderTexture target;
  }
  bool factoryHostSessionR657;FactoryPreparedComponentV21 factoryHostComponentR657;FactoryHostBindingsR657 factoryHostBindingsR657;FactoryHostSnapshotR657 factoryHostSnapshotR657;
  FactoryPressWarningViewV569 factoryHostWarningR657;GameObject factoryHostWarningRootR657;
  readonly Dictionary<Enemy,FactoryPressFrameV569> factoryHostPressFramesR657=new Dictionary<Enemy,FactoryPressFrameV569>();
  readonly Dictionary<string,Library> factoryHostPriorLibrariesR657=new Dictionary<string,Library>(StringComparer.Ordinal);
  readonly Dictionary<string,AudioClip> factoryHostPriorSoundsR657=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  readonly List<string> factoryHostLibrariesR657=new List<string>(),factoryHostSoundsR657=new List<string>();
  string factoryHostMenuPathR657="",factoryHostMenuHashR657="";bool factoryHostMenuReadyR657;
  static readonly string FactoryHostDefaultPathR657=System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile).Replace('\\','/') + "/.codex/runtime-drafts/factory-private-original-bgm-host-source-r742/factory-eleven-bank-default-unbound-r742.json",FactoryHostDefaultHashR657="4aa488a4496273c9f9096847b5c2d991d7c1edcf43dc004121cc3ea7f971509f";
  static readonly string[] FactoryHostNPCsR657=new[]{"NUI_BOX_ALTERA","NUI_FACTORY_PRESS","NUI_FACTORY_PRESS3","NUI_NASOD_GUARD","NUI_NASOD_HEALER_BLADER","NUI_NASOD_MAKER","NUI_NASOD_SHERIFF","NUI_RAVEN_CLONE","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP","NUI_WALLY_9TH_TYPE2"},FactoryHostKindsR657=new[]{"box-altera","factory-press","factory-press3","nasod-guard","nasod-healer-blader","nasod-maker","nasod-sheriff","raven-clone","smallgate-besma-air-ship","smallgate-post-besma-air-ship","wally-9th-type2"};
  public bool FactoryPreparedHostActiveR657 {get{return factoryHostSessionR657&&factoryHostComponentR657!=null&&!factoryHostComponentR657.IsDisposed&&ReferenceEquals(game,factoryHostComponentR657.Model);}}
  FactoryHostBindingsR657 ReadFactoryHostBindingsR657(string path,string sha,bool requireBound){
   PreviewRequire(!string.IsNullOrEmpty(path)&&sha!=null&&sha.Length==64,"Factory descriptor path/hash missing");var info=new FileInfo(path);PreviewRequire(info.Exists&&info.Length<=1024*1024,"Factory descriptor missing/oversized");var bytes=File.ReadAllBytes(path);PreviewRequire(PreviewHash(bytes)==sha,"Factory descriptor changed");var b=JsonUtility.FromJson<FactoryHostBindingsR657>(Encoding.UTF8.GetString(bytes));
   PreviewRequire(b!=null&&b.schema=="factory-full-host-bindings-r657-v1"&&b.sourceDungeon=="Dungeon_Altera_Nasod_Factory_Normal"&&!b.runtimeAdmitted,"Factory descriptor scope");if(requireBound)PreviewRequire(b.bound,"Factory presentation assets are not bound");if(!b.bound)return b;
   PreviewRequire(b.sourceModel!=null&&b.fullRules!=null&&b.fullMechanisms!=null&&b.populationPolicy!=null&&b.eventBinding!=null&&b.entryActivation!=null&&b.sourceController!=null&&b.pressPolicy!=null,"Factory eight input bindings");
   int wholeCount=0;if(b.wholeBanks!=null){ReadFactoryWholeBindingsR684(b.wholeBanks);wholeCount=2;}FactoryReferenceBindingsR725 referenceBanks=null;int referenceCount=0;if(b.nineReferenceBanks!=null){PreviewRequire(wholeCount==2,"Nine reference banks require the two original pixel banks");referenceBanks=ReadFactoryReferenceBindingsR725(b.nineReferenceBanks,b.fullRules);referenceCount=referenceBanks.banks.Length;}
   var v=b.presentation;PreviewRequire(v!=null&&v.editorOnly&&!v.runtimeAdmitted&&v.worlds!=null&&v.worlds.Length==3&&v.sprites!=null&&v.sprites.Length==11-wholeCount-referenceCount&&v.audio!=null&&v.audio.Length>=3&&v.effects!=null&&v.sounds!=null&&v.soundMap!=null&&b.stages!=null&&b.stages.Length==3,"Factory complete presentation bindings missing");
   string[] worlds={"WI_ALTERA_NASOD_FACTORY_STAGE1","WI_ALTERA_NASOD_FACTORY_STAGE2","WI_ALTERA_NASOD_FACTORY_BOSS"};
   for(int i=0;i<3;i++){
    var world=v.worlds[i];var stage=b.stages[i];
    PreviewRequire(world!=null,"Factory presentation world object; room="+i+" actual=<null>");
    PreviewRequire(world.id==worlds[i],"Factory presentation world id; room="+i+" actual=["+(world.id??"<null>")+"] expected=["+worlds[i]+"]");
    PreviewRequire(stage!=null,"Factory presentation stage object; room="+i+" actual=<null>");
    PreviewRequire(stage.id==b.sourceDungeon+"-"+i,"Factory presentation stage id; room="+i+" actual=["+(stage.id??"<null>")+"] expected=["+b.sourceDungeon+"-"+i+"]");
    PreviewRequire(stage.sourceWorldId==worlds[i],"Factory presentation stage sourceWorldId; room="+i+" actual=["+(stage.sourceWorldId??"<null>")+"] expected=["+worlds[i]+"]");
    PreviewRequire(stage.spawns==null||stage.spawns.Length==0,"Factory presentation view births must be empty; room="+i+" actualLength="+(stage.spawns==null?"<null>":stage.spawns.Length.ToString()));
   }
   var seen=new HashSet<string>();if(wholeCount==2){seen.Add("NUI_WALLY_9TH_TYPE2");seen.Add("NUI_NASOD_HEALER_BLADER");}if(referenceBanks!=null)foreach(var bank in referenceBanks.banks)PreviewRequire(seen.Add(bank.npc),"Duplicate original-reference NPC coverage");foreach(var s in v.sprites){PreviewRequire(s!=null,"Factory sprite binding missing");int at=Array.IndexOf(FactoryHostNPCsR657,s.sourceNPC);PreviewRequire(at>=0&&s.id==FactoryHostKindsR657[at]&&seen.Add(s.sourceNPC),"Factory explicit NPC/art binding");}PreviewRequire(seen.Count==11,"Factory exact eleven view slots, no implied asset admission");ValidateFactoryPrivateMusicR742(b);return b;
  }
  void EnsureFactoryHostMenuR657(){
   Image root;if(!images.TryGetValue("Dungeon Root",out root))return;if(!images.ContainsKey("Factory Host R657")){Button("Factory Host R657","FACTORY / PREPARING ASSETS",new Rect(290,31,322,41),"factory-host-r657",11,root.transform);var created=root.transform.Find("Factory Host R657");images.Add("Factory Host R657",created.GetComponent<Image>());labels.Add("Factory Host R657 Text",created.Find("Factory Host R657 Text").GetComponent<Text>());}
   string path=Environment.GetEnvironmentVariable("EL_FACTORY_HOST_BUNDLE_R657"),sha=Environment.GetEnvironmentVariable("EL_FACTORY_HOST_SHA_R657");if(string.IsNullOrEmpty(path)){path=FactoryHostDefaultPathR657;sha=FactoryHostDefaultHashR657;}
   if(path!=factoryHostMenuPathR657||sha!=factoryHostMenuHashR657){factoryHostMenuPathR657=path;factoryHostMenuHashR657=sha;factoryHostMenuReadyR657=false;if(File.Exists(path)){var b=ReadFactoryHostBindingsR657(path,sha,false);factoryHostMenuReadyR657=b.bound;}}
   Visible("Factory Host R657",mapRegion=="altera");TextValue("Factory Host R657 Text",factoryHostMenuReadyR657?"ENTER FACTORY CANDIDATE":"FACTORY / PREPARING ASSETS");images["Factory Host R657"].raycastTarget=factoryHostMenuReadyR657;images["Factory Host R657"].color=C(factoryHostMenuReadyR657?0x476b61u:0x293944u);
  }
  bool FactoryHostMenuActionR657(string action){if(action!="factory-host-r657")return false;EnsureFactoryHostMenuR657();PreviewRequire(factoryHostMenuReadyR657,"Factory assets are still preparing");BeginFactoryPreparedHostR657(factoryHostMenuPathR657,factoryHostMenuHashR657);return true;}
  public void BeginFactoryPreparedHostR657(string path,string sha){
   var bindings=ReadFactoryHostBindingsR657(path,sha,true);PreviewRequire(Application.isPlaying&&!forestPreviewActive&&!forestPreviewLoading&&!factoryHostSessionR657&&string.IsNullOrEmpty(unifiedSourceSessionDescriptorV21)&&unifiedSourceLoadSessionV21==null&&unifiedSourceLoaderV21==null&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&openMenu=="dungeons"&&!BossIntroductionActive,"Factory Host requires idle private Editor dungeon menu");
   factoryHostSnapshotR657=new FactoryHostSnapshotR657{game=game,paused=game.paused,adventure=adventureStarted,pausedMenu=pausedBeforeMenu,selected=selectedDungeon,preview=previewDungeon,mapSelected=mapSelected,mapPage=mapPage,menu=openMenu,origin=menuOrigin,region=mapRegion,accumulator=accumulator,position=worldCamera.transform.position,rotation=worldCamera.transform.rotation,rect=worldCamera.rect,aspect=worldCamera.aspect,ortho=worldCamera.orthographic,size=worldCamera.orthographicSize,fov=worldCamera.fieldOfView,near=worldCamera.nearClipPlane,far=worldCamera.farClipPlane,projection=worldCamera.projectionMatrix,target=worldCamera.targetTexture};factoryHostBindingsR657=bindings;factoryHostSessionR657=true;
   try{CaptureFactoryMusicR742(factoryHostSnapshotR657);BeginSourceDungeonPreview(path,false);}catch{EndFactoryPreparedHostR657();throw;}
  }
  void ApplyFactoryHostIdentityR657(){if(!factoryHostSessionR657)return;forestPreviewDungeon.id="editor-factory-full-host-r657";forestPreviewDungeon.title="NASOD FACTORY / CANDIDATE";forestPreviewDungeon.region="altera";forestPreviewDungeon.count=3;forestPreviewDungeon.boss="RAVEN CLONE";}
  IEnumerator LoadFactoryPreparedHostR657(){
   var b=factoryHostBindingsR657;PreviewRequire(factoryHostSessionR657&&b!=null&&b.bound,"Factory bound session missing");FactoryPreparedComponentV21 component=null;bool mounted=false;
   try{
    component=FactoryPreparedComponentV21.CreateFullIndependentComponent(PreviewRead(b.sourceModel),PreviewRead(b.fullRules),PreviewRead(b.fullMechanisms),PreviewRead(b.populationPolicy),PreviewRead(b.eventBinding),PreviewRead(b.entryActivation),PreviewRead(b.sourceController),PreviewRead(b.pressPolicy));
    ConfigureFactoryWholeBanksR684(b.wholeBanks,b.fullRules);ConfigureFactoryReferenceBanksR725(b.nineReferenceBanks,b.fullRules);
    var v=b.presentation;var rawLibraries=new Dictionary<string,PreviewLibrary>();foreach(var e in v.sprites){var raw=PreviewJson<PreviewLibrary>(e.animations);PreviewRequire(raw!=null&&raw.sourceNPC==e.sourceNPC&&raw.id==e.id,"Factory explicit view identity mismatch");rawLibraries.Add(e.id,raw);PreviewTextureFile(e.atlas);}
    var effects=PreviewJson<NpcEffectDocument>(v.effects);var sounds=PreviewJson<NpcSoundDocument>(v.sounds);PreviewRead(v.soundMap);component.Model.SetNpcEffectDefinitions(effects);component.Model.SetNpcSoundDefinitions(sounds);
    forestPreviewStages=b.stages;forestPreviewScenes=new NativeScene[3];forestPreviewGeometry=new byte[3][];
    for(int i=0;i<3;i++){var w=v.worlds[i];var scene=PreviewJson<NativeScene>(w.scene);var projection=PreviewJson<PreviewProjection>(w.projection);PreviewRequire(scene!=null&&projection!=null&&w.textures!=null,"Factory source world presentation missing");scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=PreviewRead(w.binary);foreach(var t in w.textures)forestPreviewTextureBindings[t.sourcePath]=PreviewTextureFile(t.selected);forestPreviewStages[i].height=projection.height;forestPreviewStages[i].nativeScene=w.id;if(!FactoryPrivateMusicRequestedR742(b))PreviewRequire(!string.IsNullOrEmpty(forestPreviewStages[i].bgm)&&sceneMusicAssets.ContainsKey(forestPreviewStages[i].bgm)&&Resources.Load<AudioClip>(sceneMusicAssets[forestPreviewStages[i].bgm].resource),"Factory original room BGM is not staged");}
    var clips=new Dictionary<string,AudioClip>(StringComparer.Ordinal);foreach(var a in v.audio){PreviewRead(a.file);using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(a.file.path)).AbsoluteUri,AudioType.OGGVORBIS)){yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"Factory original OGG decode failed: "+a.id);var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip,"Factory empty source audio: "+a.id);forestPreviewOwned.Add(clip);clip.name=a.id;clip.LoadAudioData();while(clip.loadState==AudioDataLoadState.Loading)yield return null;PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"Factory OGG data unavailable: "+a.id);clips.Add(a.id,clip);}}
    foreach(var id in new[]{"AlteraCutScene_LiftMove.ogg","AlteraCutScene_RobotFall.ogg","AlteraCutScene_RobotAssemble.ogg"})PreviewRequire(clips.ContainsKey(id),"Factory director audio binding missing: "+id);
    // No yields after this shared commit begins; previous bindings stay owned.
    InstallFactoryMusicR742(clips);
    foreach(var e in v.sprites){Library prior;if(libraries.TryGetValue(e.id,out prior)){factoryHostPriorLibrariesR657.Add(e.id,prior);libraries.Remove(e.id);}factoryHostLibrariesR657.Add(e.id);LoadForestPreviewSprite(e,rawLibraries[e.id]);}
    foreach(var p in clips){AudioClip prior;if(audioClips.TryGetValue(p.Key,out prior)){factoryHostPriorSoundsR657.Add(p.Key,prior);audioClips.Remove(p.Key);}factoryHostSoundsR657.Add(p.Key);}RegisterNpcSoundClips(clips);forestPreviewSounds.AddRange(clips.Keys);
    factoryHostComponentR657=component;game=component.Model;mounted=true;for(int i=0;i<GameModel.SkillSlotCount;i++)game.EquipSkill(i,factoryHostSnapshotR657.game.GetEquippedSkill(i).id);ApplyTimings();
    factoryHostWarningRootR657=new GameObject("Factory private Press presentation R657");factoryHostWarningRootR657.transform.SetParent(actorsRoot,false);factoryHostWarningR657=new FactoryPressWarningViewV569(factoryHostWarningRootR657.transform);factoryHostWarningR657.Bind(game);
    forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();
   }finally{if(!mounted&&component!=null)component.Dispose();}
  }
  // R684 reuses immutable finite-display lookups already present in this catalog.
  // Only the existing Factory model's state ID/npcTime are read. No Core AI/FSM is installed.
  const string FactoryWholeBindingShaR684="ad1a5901172132b5f73af8b4692e3cf6a77a5ea4acd733338d63e47f7a08a299";
  const string FactoryWholeRulesShaR684="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159";
  static readonly string FactoryWholeFrozenRootR684=System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile).Replace('\\','/') + "/.codex/runtime-drafts/factory-two-family-finite-bank-adapter-source-r684/frozen";
#pragma warning disable 0649 // Exact pinned JSON transport; required sentinels/counts are validated.
  [Serializable] sealed class FactoryWholeFileR684 {public string path,sha256;public long bytes;}
  [Serializable] sealed class FactoryWholeKeyBindingR684 {public string npc,key,originalPath,rgbaSha256;public FactoryWholeFileR684 file;public int width,height;}
  [Serializable] sealed class FactoryWholeProvenanceR684 {public string role,originalPath;public FactoryWholeFileR684 file;}
  [Serializable] sealed class FactoryWholeVisualStateR684 {public string npc,state,clip,motion,durationBits,motionDurationBits,speedBits;public bool loop;}
  [Serializable] sealed class FactoryWholeBindingsR684 {
   public string schema,factoryRulesSha256;public FactoryWholeFileR684 wallyDocument,healerDocument;public FactoryWholeKeyBindingR684[] keys;public FactoryWholeProvenanceR684[] provenance;public FactoryWholeVisualStateR684[] states;
   public int keyCount,stateCount;public long rgbaBytes;public bool sourceRulesNeverRewritten,sourceEventTransitionsNeverImported,sourceClockChanged,automaticSpawnFacingMirror,imagesReencoded,runtimeAdmitted,FactoryArtApproved;
  }
#pragma warning restore 0649
  WallyFiniteWholeLookupR198 factoryWallyLookupR684;SourcePhaseWholeLookupV21 factoryHealerLookupR684;
  readonly Dictionary<string,Sprite> factoryWholeSpritesR684=new Dictionary<string,Sprite>(StringComparer.Ordinal);
  readonly Dictionary<string,Texture2D> factoryWholeTexturesR684=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  int factoryWholeMissingStateHoldsR684,factoryWallyOutsideFiniteHoldsR684;string factoryWholeLastNpcR684="",factoryWholeLastStateR684="",factoryWholeLastKeyR684="";float factoryWholeLastNpcTimeR684;
  public bool FactoryWholeBanksConfiguredR684 {get{return factoryWallyLookupR684!=null&&factoryHealerLookupR684!=null;}}
  public int FactoryWholeResidentKeysR684 {get{return factoryWholeSpritesR684.Count;}}
  public int FactoryWholeMissingStateHoldsR684 {get{return factoryWholeMissingStateHoldsR684;}}
  public int FactoryWallyOutsideFiniteHoldsR684 {get{return factoryWallyOutsideFiniteHoldsR684;}}
  public string FactoryWholeLastNpcR684 {get{return factoryWholeLastNpcR684;}}
  public string FactoryWholeLastStateR684 {get{return factoryWholeLastStateR684;}}
  public string FactoryWholeLastKeyR684 {get{return factoryWholeLastKeyR684;}}
  public float FactoryWholeLastNpcTimeR684 {get{return factoryWholeLastNpcTimeR684;}}
  static bool FactoryWholeSamePathR684(string a,string b){return string.Equals(Path.GetFullPath(a),Path.GetFullPath(b),StringComparison.OrdinalIgnoreCase);}
  static string FactoryWholeIdentityR684(string npc,string key){return npc+"/"+key;}
  static bool FactoryWholeNpcR684(string npc){return npc=="NUI_WALLY_9TH_TYPE2"||npc=="NUI_NASOD_HEALER_BLADER";}
  static string FactoryWholeFloatBitsR684(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0).ToString("X8",System.Globalization.CultureInfo.InvariantCulture);}
  FactoryWholeBindingsR684 ReadFactoryWholeBindingsR684(PreviewFile pin){
   PreviewRequire(pin!=null&&pin.sha256==FactoryWholeBindingShaR684&&!string.IsNullOrEmpty(pin.path),"Factory exact two-bank binding required");var info=new FileInfo(pin.path);PreviewRequire(info.Exists&&info.Length<=1048576,"Factory two-bank descriptor missing/oversized");var bytes=File.ReadAllBytes(pin.path);PreviewRequire(PreviewHash(bytes)==pin.sha256,"Factory two-bank descriptor changed");
   var b=JsonUtility.FromJson<FactoryWholeBindingsR684>(new UTF8Encoding(false,true).GetString(bytes));
   PreviewRequire(b!=null&&b.schema=="factory-two-existing-finite-whole-bank-bindings-r684-v1"&&b.factoryRulesSha256==FactoryWholeRulesShaR684&&b.keyCount==165&&b.stateCount==53&&b.rgbaBytes==3960348&&b.keys!=null&&b.keys.Length==165&&b.states!=null&&b.states.Length==53&&b.provenance!=null&&b.provenance.Length==4&&b.sourceRulesNeverRewritten&&b.sourceEventTransitionsNeverImported&&!b.sourceClockChanged&&!b.automaticSpawnFacingMirror&&!b.imagesReencoded&&!b.runtimeAdmitted&&!b.FactoryArtApproved,"Factory finite-bank identity/counts/scope");return b;
  }
  byte[] ReadFactoryFrozenWholeFileR684(FactoryWholeFileR684 pin){
   PreviewRequire(pin!=null&&!string.IsNullOrEmpty(pin.path)&&pin.sha256!=null&&pin.sha256.Length==64&&pin.bytes>=0&&pin.bytes<=67108864,"Factory frozen file pin");string root=Path.GetFullPath(FactoryWholeFrozenRootR684).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;string full=Path.GetFullPath(pin.path);PreviewRequire(full.StartsWith(root,StringComparison.OrdinalIgnoreCase),"Factory frozen asset must remain in this exact C package");var info=new FileInfo(full);PreviewRequire(info.Exists&&info.Length==pin.bytes,"Factory frozen asset length changed");byte[] bytes=File.ReadAllBytes(full);PreviewRequire(bytes.LongLength==pin.bytes&&PreviewHash(bytes)==pin.sha256,"Factory frozen asset SHA changed");return bytes;
  }
  void ValidateFactoryWholeProvenanceR684(FactoryWholeBindingsR684 b,string role,string originalPath,string sha){
   FactoryWholeProvenanceR684 found=null;foreach(var p in b.provenance)if(p.role==role){PreviewRequire(found==null,"Duplicate Factory whole provenance");found=p;}PreviewRequire(found!=null&&FactoryWholeSamePathR684(found.originalPath,originalPath)&&found.file.sha256==sha,"Original whole provenance identity changed: "+role);ReadFactoryFrozenWholeFileR684(found.file);
  }
  void ValidateFactoryWholeVisualStatesR684(FactoryWholeBindingsR684 b,WallyFiniteWholeLookupR198 wally,SourcePhaseWholeLookupV21 healer){
   var seen=new HashSet<string>(StringComparer.Ordinal);int w=0,h=0;
   foreach(var row in b.states){PreviewRequire(row!=null&&FactoryWholeNpcR684(row.npc)&&seen.Add(FactoryWholeIdentityR684(row.npc,row.state)),"Factory exact visual state identity");
    if(row.npc=="NUI_WALLY_9TH_TYPE2"){var s=wally.State(row.state);PreviewRequire(s.Clip==row.clip&&s.Motion==row.motion&&FactoryWholeFloatBitsR684(s.Duration)==row.durationBits&&FactoryWholeFloatBitsR684(s.MotionDuration)==row.motionDurationBits&&FactoryWholeFloatBitsR684(s.Speed)==row.speedBits&&s.Loop==row.loop,"Factory Wally visual fields differ");w++;}
    else{var s=healer.State(row.state);PreviewRequire(s.ModelClip==row.clip&&s.Motion==row.motion&&FactoryWholeFloatBitsR684(s.StateDuration)==row.durationBits&&FactoryWholeFloatBitsR684(s.MotionDuration)==row.motionDurationBits&&FactoryWholeFloatBitsR684(s.Speed)==row.speedBits&&s.Loop==row.loop&&!s.HasShow,"Factory Healer visual fields/no authored SHOW differ");h++;}
   }PreviewRequire(w==27&&h==26,"Factory two exact visual-state sets");
   // NextState/holdForSourceEvent/sourceEventTransitions are deliberately never
   // imported from the Core document. Factory DAMAGE_AIR_FALL keeps its own event rule.
  }
  FactoryWholeKeyBindingR684 FactoryWholeFrozenKeyR684(FactoryWholeBindingsR684 b,string npc,string key,string originalPath,string sha,string rgba,int width,int height){
   FactoryWholeKeyBindingR684 found=null;foreach(var row in b.keys)if(row.npc==npc&&row.key==key){PreviewRequire(found==null,"Duplicate Factory whole key");found=row;}
   PreviewRequire(found!=null&&FactoryWholeSamePathR684(found.originalPath,originalPath)&&found.file.sha256==sha&&found.rgbaSha256==rgba&&found.width==width&&found.height==height,"Original whole key/frozen binding mismatch");return found;
  }
  void LoadFactoryWholeSpriteR684(FactoryWholeKeyBindingR684 key){
   string id=FactoryWholeIdentityR684(key.npc,key.key);byte[] bytes=ReadFactoryFrozenWholeFileR684(key.file);var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Factory finite whole / "+id,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};forestPreviewOwned.Add(texture);factoryWholeTexturesR684.Add(id,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.width&&texture.height==key.height,"Factory full native PNG decode/dimensions");var sprite=Sprite.Create(texture,new Rect(0,0,key.width,key.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Factory finite whole / "+id;forestPreviewOwned.Add(sprite);factoryWholeSpritesR684.Add(id,sprite);
  }
  void ConfigureFactoryWholeBanksR684(PreviewFile binding,PreviewFile actualFactoryRules){
   PreviewRequire(factoryWallyLookupR684==null&&factoryHealerLookupR684==null&&factoryWholeSpritesR684.Count==0&&factoryWholeTexturesR684.Count==0,"Fresh Factory whole owner required");if(binding==null)return;
   PreviewRequire(actualFactoryRules!=null&&actualFactoryRules.sha256==FactoryWholeRulesShaR684,"Two-bank adapter binds Factory full rules, not Core behavior");var b=ReadFactoryWholeBindingsR684(binding);
   PreviewRequire(b.wallyDocument.sha256=="993de8969191996247a1be596100a34de11387dd0b8305f277e60baf36d505d9"&&b.healerDocument.sha256=="4ca179765a408bcc0cc3fec32fa682711b9da4e62b4c26ec2e7bf301ddc9f252","Exact original raw whole documents");
   var wd=FiniteWholeJsonR268.Read<WallyWholeDocumentR198>(ReadFactoryFrozenWholeFileR684(b.wallyDocument));var wally=new WallyFiniteWholeLookupR198(wd);
   var hd=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(ReadFactoryFrozenWholeFileR684(b.healerDocument));var healer=new SourcePhaseWholeLookupV21(hd);
   PreviewRequire(healer.Id=="nasod-healer-blader"&&healer.SourceNpc=="NUI_NASOD_HEALER_BLADER"&&healer.FixedSourceFacing=="+X"&&!healer.AutomaticSpawnFacingMirror&&healer.KeyCount==52&&healer.MotionCount==14&&healer.StateCount==26&&hd.sourcePoseSamples==85&&healer.SourceRulesSha256==wd.sourceRulesSha256&&wd.sourceRulesSha256=="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72","Exact same-NPC original finite families");
   ValidateFactoryWholeProvenanceR684(b,"wally-registry",wd.registry.path,wd.registry.sha256);ValidateFactoryWholeProvenanceR684(b,"wally-registry-acceptance",wd.registryAcceptance.path,wd.registryAcceptance.sha256);ValidateFactoryWholeProvenanceR684(b,"wally-source-clock",wd.sourceClock.path,wd.sourceClock.sha256);ValidateFactoryWholeProvenanceR684(b,"healer-art-and-phase-review",hd.artAndSampledPhaseReview.path,hd.artAndSampledPhaseReview.sha256);
   ValidateFactoryWholeVisualStatesR684(b,wally,healer);var seen=new HashSet<string>(StringComparer.Ordinal);long rgba=0;
   foreach(var k in wally.Keys()){var key=FactoryWholeFrozenKeyR684(b,"NUI_WALLY_9TH_TYPE2",k.Key,k.Path,k.Sha256,k.RgbaSha256,k.Width,k.Height);PreviewRequire(seen.Add(FactoryWholeIdentityR684(key.npc,key.key)),"Duplicate Wally whole binding");LoadFactoryWholeSpriteR684(key);rgba+=(long)k.Width*k.Height*4;}
   foreach(var k in healer.Keys()){var key=FactoryWholeFrozenKeyR684(b,"NUI_NASOD_HEALER_BLADER",k.Key,k.PngPath,k.PngSha256,k.RgbaSha256,k.Width,k.Height);PreviewRequire(seen.Add(FactoryWholeIdentityR684(key.npc,key.key)),"Duplicate Healer whole binding");LoadFactoryWholeSpriteR684(key);rgba+=(long)k.Width*k.Height*4;}
   PreviewRequire(seen.Count==165&&rgba==b.rgbaBytes,"Exact complete finite key/resident-pixel accounting");factoryWallyLookupR684=wally;factoryHealerLookupR684=healer;factoryWholeMissingStateHoldsR684=factoryWallyOutsideFiniteHoldsR684=0;
  }
  bool TryRenderFactoryWholeBankR684(Enemy e,SpriteRenderer renderer){
   try{
    if(TryRenderFactoryOriginalReferenceR725(e,renderer))return true;if(!FactoryPreparedHostActiveR657||factoryWallyLookupR684==null||!FactoryWholeNpcR684(e.sourceNPC))return false;
    PreviewRequire(e.visualScale==1&&((e.sourceNPC=="NUI_WALLY_9TH_TYPE2"&&e.kind=="wally-9th-type2")||(e.sourceNPC=="NUI_NASOD_HEALER_BLADER"&&e.kind=="nasod-healer-blader")),"Original Factory whole actor identity/scale");
    if(string.IsNullOrEmpty(e.npcState)){renderer.enabled=false;renderer.sprite=null;factoryWholeMissingStateHoldsR684++;return true;}
    string key;int width,height;double x,y;float diagnosticClock=0;string diagnosticMotion="",diagnosticPose="";bool diagnosticShow=true,diagnosticOutside=false;
    if(e.sourceNPC=="NUI_WALLY_9TH_TYPE2"){var selected=factoryWallyLookupR684.Select(e.npcState,e.npcTime);key=selected.Whole.Key;width=selected.Whole.Width;height=selected.Whole.Height;x=selected.Whole.RootX;y=selected.Whole.RootY;if(selected.OutsideFiniteRange)factoryWallyOutsideFiniteHoldsR684++;diagnosticClock=selected.SourceMotionClock;diagnosticMotion=selected.State.Motion;diagnosticPose=selected.SourcePose;diagnosticOutside=selected.OutsideFiniteRange;}
    else{PreviewRequire(factoryHealerLookupR684!=null,"Factory Healer whole owner missing");var selected=factoryHealerLookupR684.Select(e.npcState,e.npcTime);key=selected.Whole.Key;width=selected.Whole.Width;height=selected.Whole.Height;x=selected.AnchorX;y=selected.AnchorY;diagnosticClock=selected.SourceMotionClock;diagnosticMotion=selected.State.Motion;diagnosticPose=selected.SourcePose;diagnosticShow=!selected.State.HasShow||selected.State.Show;}
    renderer.enabled=true;renderer.sprite=factoryWholeSpritesR684[FactoryWholeIdentityR684(e.sourceNPC,key)];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
    // Same centered FullRect + signed anchor placement as the real Core renderers.
    // No clamping, rounding, SpriteOffset, scale rebake, spawn-facing mirror or rerender.
    var offset=new Vector2((float)(width*.5-x),(float)(y-height*.5));var p=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(p.x,p.y,0);
    factoryWholeLastNpcR684=e.sourceNPC;factoryWholeLastStateR684=e.npcState;factoryWholeLastKeyR684=key;factoryWholeLastNpcTimeR684=e.npcTime;RecordFactoryNpcSelectionR977(e,renderer,key,diagnosticMotion,diagnosticPose,diagnosticClock,diagnosticShow,diagnosticOutside);return true;
   }catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}
  }
  // Explicit original-reference diagnostics, separate from the two R684 pixel banks.
  // No reference document is passed to PreviewLibrary or its approved-art constructor.
  const string FactoryNineBindingShaR725="768d7514c54723f75305f14ad9780951d1c7898fb207d5704d652869ec9da4f2";
  static readonly string FactoryNineRootR725=System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile).Replace('\\','/') + "/.codex/runtime-drafts/factory-nine-original-reference-library-r718";
#pragma warning disable 0649 // Exact pinned source-reference transport, no art approval.
  [Serializable] sealed class FactoryReferenceBankR725 {
   public string npc,kind,id;public FactoryWholeFileR684 document,sourceReferenceEvidence;
   public int keyCount,motionCount,stateCount,sourcePoseSamples;public long rgbaBytes;public bool artApproved,runtimeAdmitted;
  }
  [Serializable] sealed class FactoryReferenceBindingsR725 {
   public string schema,sourceDungeon;public FactoryWholeFileR684 FactoryRules;public FactoryReferenceBankR725[] banks;
   public int bankCount,stateCount,motionCount,sourcePoseSamples,keyCount;public long rgbaBytes;
   public bool allInputsBeforeAfterExact,originalRenderingAndTransparentCropOnly,AIImagesGenerated,artApproved,sampledPhaseSequenceApproved,animationLibraryApproved,nativeEquivalent,runtimeAdmitted,HostWiringImplemented,Factory11AssetBindingComplete;
  }
  [Serializable] sealed class FactoryReferencePngR725 {public string key;public FactoryWholeFileR684 png;}
  [Serializable] sealed class FactoryReferencePngPinsR725 {public FactoryReferencePngR725[] keys;}
#pragma warning restore 0649
  readonly Dictionary<string,SourcePhaseWholeLookupV21> factoryReferenceLookupsR725=new Dictionary<string,SourcePhaseWholeLookupV21>(StringComparer.Ordinal);
  int factoryReferenceMissingBirthHoldsR725,factoryReferenceResidentKeysR725;float factoryReferenceLastSourceClockR725;
  public bool FactoryOriginalReferenceBanksConfiguredR725 {get{return factoryReferenceLookupsR725.Count==9;}}
  public int FactoryOriginalReferenceMissingBirthHoldsR725 {get{return factoryReferenceMissingBirthHoldsR725;}}
  public int FactoryOriginalReferenceResidentKeysR725 {get{return factoryReferenceResidentKeysR725;}}
  public float FactoryOriginalReferenceLastSourceClockR725 {get{return factoryReferenceLastSourceClockR725;}}
  static int FactoryReferenceNpcIndexR725(string npc){int at=Array.IndexOf(FactoryHostNPCsR657,npc);return at>=0&&!FactoryWholeNpcR684(npc)?at:-1;}
  byte[] ReadFactoryReferenceFileR725(FactoryWholeFileR684 pin){
   PreviewRequire(pin!=null&&!string.IsNullOrEmpty(pin.path)&&pin.sha256!=null&&pin.sha256.Length==64&&pin.bytes>0&&pin.bytes<=67108864,"Factory reference file pin");
   string root=Path.GetFullPath(FactoryNineRootR725).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,full=Path.GetFullPath(pin.path);
   PreviewRequire(full.StartsWith(root,StringComparison.OrdinalIgnoreCase),"Factory reference file outside exact frozen R718 package");var info=new FileInfo(full);PreviewRequire(info.Exists&&info.Length==pin.bytes,"Factory reference file length changed");var raw=File.ReadAllBytes(full);PreviewRequire(raw.LongLength==pin.bytes&&PreviewHash(raw)==pin.sha256,"Factory reference file SHA changed");return raw;
  }
  FactoryReferenceBindingsR725 ReadFactoryReferenceBindingsR725(PreviewFile binding,PreviewFile actualFactoryRules){
   PreviewRequire(binding!=null&&FactoryWholeSamePathR684(binding.path,Path.Combine(FactoryNineRootR725,"factory-nine-reference-bindings-r718.json"))&&binding.sha256==FactoryNineBindingShaR725,"Exact nine original-reference bank binding");
   var raw=ReadFactoryReferenceFileR725(new FactoryWholeFileR684{path=binding.path,sha256=binding.sha256,bytes=119284});var b=JsonUtility.FromJson<FactoryReferenceBindingsR725>(new UTF8Encoding(false,true).GetString(raw));
   PreviewRequire(b!=null&&b.schema=="factory-nine-original-reference-bank-bindings-r718-v1"&&b.sourceDungeon=="Dungeon_Altera_Nasod_Factory_Normal"&&b.FactoryRules!=null&&actualFactoryRules!=null&&b.FactoryRules.sha256==FactoryWholeRulesShaR684&&actualFactoryRules.sha256==FactoryWholeRulesShaR684&&FactoryWholeSamePathR684(b.FactoryRules.path,actualFactoryRules.path),"Nine reference banks bind the same actual Factory rules");
   PreviewRequire(b.bankCount==9&&b.banks!=null&&b.banks.Length==9&&b.stateCount==132&&b.motionCount==96&&b.sourcePoseSamples==372&&b.keyCount==253&&b.rgbaBytes==7316264&&b.allInputsBeforeAfterExact&&b.originalRenderingAndTransparentCropOnly&&!b.AIImagesGenerated&&!b.artApproved&&!b.sampledPhaseSequenceApproved&&!b.animationLibraryApproved&&!b.nativeEquivalent&&!b.runtimeAdmitted&&!b.HostWiringImplemented&&!b.Factory11AssetBindingComplete,"Exact nine diagnostic banks; no art/native/runtime approval");
   var seen=new HashSet<string>(StringComparer.Ordinal);foreach(var bank in b.banks){int at=bank==null?-1:FactoryReferenceNpcIndexR725(bank.npc);PreviewRequire(at>=0&&bank.kind==FactoryHostKindsR657[at]&&bank.id==bank.kind+"-original-reference-r718"&&seen.Add(bank.npc)&&bank.document!=null&&bank.sourceReferenceEvidence!=null&&!bank.artApproved&&!bank.runtimeAdmitted,"Exact nine NPC/kind/diagnostic identities");}
   PreviewRequire(seen.Count==9,"Nine complete exact reference NPC slots");return b;
  }
  void LoadFactoryReferenceSpriteR725(string npc,SourceWholeKeyInfoV21 key,FactoryWholeFileR684 png){
   string id=FactoryWholeIdentityR684(npc,key.Key);PreviewRequire(!factoryWholeSpritesR684.ContainsKey(id)&&!factoryWholeTexturesR684.ContainsKey(id)&&FactoryWholeSamePathR684(png.path,key.PngPath)&&png.sha256==key.PngSha256,"Exact original-reference PNG identity");var raw=ReadFactoryReferenceFileR725(png);
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);forestPreviewOwned.Add(texture);texture.name="Factory original reference / "+id;texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
   PreviewRequire(ImageConversion.LoadImage(texture,raw,true)&&texture.width==key.Width&&texture.height==key.Height,"Original-reference PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);forestPreviewOwned.Add(sprite);PreviewRequire(sprite,"Original-reference Sprite creation");sprite.name="Factory original reference / "+id;
   factoryWholeTexturesR684.Add(id,texture);factoryWholeSpritesR684.Add(id,sprite);
  }
  void ConfigureFactoryReferenceBanksR725(PreviewFile binding,PreviewFile actualFactoryRules){
   PreviewRequire(factoryReferenceLookupsR725.Count==0&&factoryReferenceResidentKeysR725==0,"Fresh nine-reference owner required");if(binding==null)return;
   PreviewRequire(factoryWallyLookupR684!=null&&factoryHealerLookupR684!=null,"Nine reference banks accompany the original two pixel banks");var b=ReadFactoryReferenceBindingsR725(binding,actualFactoryRules);int keys=0,states=0,motions=0,phases=0;long rgba=0;
   foreach(var bank in b.banks){
    var raw=ReadFactoryReferenceFileR725(bank.document);ReadFactoryReferenceFileR725(bank.sourceReferenceEvidence);var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(raw);
    PreviewRequire(d.sourceReferenceEvidence!=null&&FactoryWholeSamePathR684(d.sourceReferenceEvidence.path,bank.sourceReferenceEvidence.path)&&d.sourceReferenceEvidence.sha256==bank.sourceReferenceEvidence.sha256,"Exact original-source reference evidence pin");
    var lookup=SourcePhaseWholeLookupV21.CreateOriginalReferenceDiagnosticR718(d);PreviewRequire(lookup.OriginalSourceReferenceDiagnosticR718&&lookup.SourceNpc==bank.npc&&lookup.Id==bank.id&&lookup.SourceRulesSha256==FactoryWholeRulesShaR684&&lookup.KeyCount==bank.keyCount&&lookup.StateCount==bank.stateCount&&lookup.MotionCount==bank.motionCount&&d.sourcePoseSamples==bank.sourcePoseSamples,"Exact source-reference lookup cardinality/identity");
    // A separate simple pin-only DTO retains original integral PNG byte lengths.
    // Critical motion/anchor numerics above still use unchanged FiniteWholeJson.
    var pins=JsonUtility.FromJson<FactoryReferencePngPinsR725>(new UTF8Encoding(false,true).GetString(raw));PreviewRequire(pins!=null&&pins.keys!=null&&pins.keys.Length==bank.keyCount,"Complete reference PNG byte pins");var byKey=new Dictionary<string,FactoryWholeFileR684>(StringComparer.Ordinal);
    foreach(var p in pins.keys){PreviewRequire(p!=null&&!string.IsNullOrEmpty(p.key)&&p.png!=null,"Reference PNG pin row");byKey.Add(p.key,p.png);}
    long bankRgba=0;foreach(var key in lookup.Keys()){FactoryWholeFileR684 png;PreviewRequire(byKey.TryGetValue(key.Key,out png)&&FactoryWholeSamePathR684(key.PngPath,Path.Combine(Path.GetDirectoryName(bank.document.path),key.File)),"Reference key resolves inside its own exact bank");LoadFactoryReferenceSpriteR725(bank.npc,key,png);bankRgba+=(long)key.Width*key.Height*4;keys++;}
    PreviewRequire(bankRgba==bank.rgbaBytes,"Reference bank resident RGBA accounting");rgba+=bankRgba;states+=lookup.StateCount;motions+=lookup.MotionCount;phases+=d.sourcePoseSamples;factoryReferenceLookupsR725.Add(bank.npc,lookup);
   }
   PreviewRequire(factoryReferenceLookupsR725.Count==9&&keys==253&&states==132&&motions==96&&phases==372&&rgba==7316264&&factoryWholeSpritesR684.Count==418,"Complete eleven-bank finite image accounting");factoryReferenceResidentKeysR725=keys;factoryReferenceMissingBirthHoldsR725=0;factoryReferenceLastSourceClockR725=0;
  }
  bool TryRenderFactoryOriginalReferenceR725(Enemy e,SpriteRenderer renderer){
   if(!FactoryPreparedHostActiveR657)return false;SourcePhaseWholeLookupV21 lookup;if(!factoryReferenceLookupsR725.TryGetValue(e.sourceNPC,out lookup))return false;
   int at=FactoryReferenceNpcIndexR725(e.sourceNPC);PreviewRequire(at>=0&&game.enemies.Contains(e)&&e.kind==FactoryHostKindsR657[at]&&e.visualScale==1,"Existing Factory reference actor identity/scale");
   if(string.IsNullOrEmpty(e.npcState)){renderer.enabled=false;renderer.sprite=null;factoryReferenceMissingBirthHoldsR725++;return true;}
   SourceWholeSelectionV21 selected;
   if(e.sourceNPC=="NUI_FACTORY_PRESS"||e.sourceNPC=="NUI_FACTORY_PRESS3"){
    FactoryPressFrameV569 f;PreviewRequire(factoryHostPressFramesR657.TryGetValue(e,out f)&&f!=null&&ReferenceEquals(f.actualOwner,e)&&f.npc==e.sourceNPC&&f.state==e.npcState,"Same actual Press owner/state frame required; no reconstructed clock");selected=lookup.SelectOriginalReferenceSourceClockR718(f.state,e.npcTime,f.sourcePoseTime);
   }else selected=lookup.Select(e.npcState,e.npcTime);
   var key=selected.Whole;renderer.enabled=true;renderer.sprite=factoryWholeSpritesR684[FactoryWholeIdentityR684(e.sourceNPC,key.Key)];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
   // Keep model-owned GameObject visibility. Only the signed source anchor
   // offsets the already baked whole; no model SHOW/FSM/clock/facing is written.
   var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var p=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(p.x,p.y,0);
   factoryWholeLastNpcR684=e.sourceNPC;factoryWholeLastStateR684=e.npcState;factoryWholeLastKeyR684=key.Key;factoryWholeLastNpcTimeR684=e.npcTime;factoryReferenceLastSourceClockR725=selected.SourceMotionClock;RecordFactoryNpcSelectionR977(e,renderer,key.Key,selected.State.Motion,selected.SourcePose,selected.SourceMotionClock,!selected.State.HasShow||selected.State.Show,false);return true;
  }

  // One explicit diagnostic ordinary-Input prefix; no model/actor setter.
  // A session reference token is not a native object ID. Log messages record
  // Input selected for the following original Update's <=6 ordinary substeps;
  // they do not claim an independent per-substep history.
  [Serializable] sealed class FactoryFirstDrawInputRecordR1017 {
   public string phase,reason,npc,state,modelAssembly,modelMvid;
   public int sessionReferenceToken,frame,hostUpdate,targetUID,room,encounter;
   public bool selectedForOrdinaryUpdate,modelAvailable,sameOriginalModelReference,targetAvailable,sameActorReference,aiEnabled,alerted,dormant,active,sourceClockAvailable;
   public float modelElapsed,wallElapsed,playerX,playerY,targetX,targetY,distance,aggroRange,npcTime,actorClock,sourceClock;
   public Input originalInput,selectedInput;
  }
  GameModel factoryFirstInputModelR1017;Enemy factoryFirstInputTargetR1017;
  int factoryFirstInputStageR1017,factoryFirstInputSessionR1017,factoryFirstInputLinesR1017,factoryFirstInputBytesR1017;
  float factoryFirstInputAnchorR1017,factoryFirstInputWallR1017;
  void BeginFactoryFirstDrawInputR1017(){
   factoryFirstInputStageR1017=0;factoryFirstInputModelR1017=null;factoryFirstInputTargetR1017=null;factoryFirstInputLinesR1017=factoryFirstInputBytesR1017=0;
   if(Array.IndexOf(Environment.GetCommandLineArgs(),"--factory-first-real-npc-draw-r977")<0||!factoryNpcDrawEnabledR977||!FactoryPreparedHostActiveR657)return;
   factoryFirstInputModelR1017=game;factoryFirstInputAnchorR1017=game.elapsed;factoryFirstInputWallR1017=Time.realtimeSinceStartup;factoryFirstInputSessionR1017++;factoryFirstInputStageR1017=1;
  }
  void FailFactoryFirstDrawInputR1017(string reason){factoryFirstInputStageR1017=3;Debug.LogError("[R1017_FACTORY_ORDINARY_INPUT_FAILURE] "+reason);}
  void RecordFactoryFirstDrawInputR1017(string phase,string reason,Input original,Input selected,bool selectedForUpdate){
   var model=factoryFirstInputModelR1017;var target=factoryFirstInputTargetR1017;FactoryNpcDrawEntryR977 entry=null;
   bool known=target!=null&&factoryNpcDrawEntriesR977.TryGetValue(target,out entry),sameTarget=model!=null&&target!=null&&model.enemies.Contains(target);
   bool clock=known&&entry!=null&&entry.state==target.npcState&&entry.npcTime==target.npcTime;
   float dx=model!=null&&target!=null?model.player.x-target.x:0,dy=model!=null&&target!=null?model.player.y-target.y:0;
   var record=new FactoryFirstDrawInputRecordR1017{phase=phase,reason=reason,sessionReferenceToken=factoryFirstInputSessionR1017,frame=Time.frameCount,hostUpdate=frame,selectedForOrdinaryUpdate=selectedForUpdate,modelAvailable=model!=null,sameOriginalModelReference=ReferenceEquals(model,game),targetAvailable=target!=null,sameActorReference=sameTarget,targetUID=known?entry.uid:0,room=model!=null?model.room:-1,encounter=model!=null?model.EncounterIndex:-1,modelElapsed=model!=null?model.elapsed:0,wallElapsed=Time.realtimeSinceStartup-factoryFirstInputWallR1017,playerX=model!=null?model.player.x:0,playerY=model!=null?model.player.y:0,targetX=target!=null?target.x:0,targetY=target!=null?target.y:0,distance=(float)Math.Sqrt(dx*dx+dy*dy),aggroRange=target!=null?target.aggroRange:0,npc=target!=null?target.sourceNPC:"",state=target!=null?target.npcState:"",aiEnabled=target!=null&&target.aiEnabled,alerted=target!=null&&target.alerted,dormant=target!=null&&target.dormant,active=target!=null&&target.active,npcTime=target!=null?target.npcTime:0,actorClock=target!=null?target.clock:0,sourceClockAvailable=clock,sourceClock=clock?entry.sourceClock:0,modelAssembly=typeof(GameModel).Assembly.GetName().Name,modelMvid=typeof(GameModel).Assembly.ManifestModule.ModuleVersionId.ToString(),originalInput=original,selectedInput=selected};
   foreach(float value in new[]{record.modelElapsed,record.wallElapsed,record.playerX,record.playerY,record.targetX,record.targetY,record.distance,record.aggroRange,record.npcTime,record.actorClock,record.sourceClock,original!=null?original.move:0,selected!=null?selected.move:0})if(!FactoryDrawFiniteR977(value)){FailFactoryFirstDrawInputR1017("NONFINITE_ACTUAL_INPUT_OBSERVATION");return;}
   string message="[R1017_FACTORY_ORDINARY_INPUT] "+JsonUtility.ToJson(record,false);int bytes=Encoding.UTF8.GetByteCount(message)+1;
   if(factoryFirstInputLinesR1017>=128||bytes>2048||factoryFirstInputBytesR1017+bytes>262144){FailFactoryFirstDrawInputR1017("BOUNDED_INPUT_TRACE_CAP_128_LINES_2K_256K");return;}
   factoryFirstInputLinesR1017++;factoryFirstInputBytesR1017+=bytes;Debug.Log(message);
  }
  Input FactoryFirstDrawInputR1017(Input original){
   if(factoryFirstInputStageR1017==0||factoryFirstInputStageR1017==3)return original;
   string stop=null;
   if(!factoryNpcDrawEnabledR977||!FactoryPreparedHostActiveR657||!ReferenceEquals(game,factoryFirstInputModelR1017))stop="SESSION_OR_MODEL_CHANGED";
   else if(!string.IsNullOrEmpty(openMenu)||game.paused||game.victory||game.player.hp<=0||runAutomatedPlaythrough)stop="MANUAL_MENU_PAUSE_TERMINAL_OR_AUTOMATED_MODE";
   else if(game.room!=0||game.EncounterIndex!=0)stop="ORIGINAL_FIRST_ENCOUNTER_CHANGED";
   else if(game.elapsed-factoryFirstInputAnchorR1017>=2||Time.realtimeSinceStartup-factoryFirstInputWallR1017>=3)stop="ORDINARY_INPUT_PREFIX_2_MODEL_OR_3_WALL_LIMIT";
   else if(original==null)stop="NO_ORIGINAL_INPUT";
   else if(game.enemies.Count>64||factoryNpcDrawOverflowR977)stop="ORIGINAL_DIAGNOSTIC_ACTOR_CAP";
   if(stop!=null){factoryFirstInputStageR1017=3;RecordFactoryFirstDrawInputR1017("STOP",stop,original,original,true);return original;}
   if(factoryFirstInputTargetR1017==null){
    float nearest=float.PositiveInfinity;
    foreach(var e in game.enemies)if(e.sourceNPC=="NUI_WALLY_9TH_TYPE2"&&e.active&&!e.dormant&&e.hp>0&&e.encounterIndex==game.EncounterIndex){float dx=game.player.x-e.x,dy=game.player.y-e.y,distance=dx*dx+dy*dy;if(distance<nearest){nearest=distance;factoryFirstInputTargetR1017=e;}}
   }
   var target=factoryFirstInputTargetR1017;
   if(target==null||!game.enemies.Contains(target)||!target.active||target.dormant||target.hp<=0||target.encounterIndex!=game.EncounterIndex){factoryFirstInputStageR1017=3;RecordFactoryFirstDrawInputR1017("STOP","NO_SAME_CURRENT_LIVING_WALLY",original,original,true);return original;}
   float deltaX=game.player.x-target.x,deltaY=game.player.y-target.y,d=(float)Math.Sqrt(deltaX*deltaX+deltaY*deltaY);
   if(!FactoryDrawFiniteR977(d)||!FactoryDrawFiniteR977(target.aggroRange)||target.aggroRange<=0){FailFactoryFirstDrawInputR1017("NONFINITE_OR_NONPOSITIVE_ORIGINAL_TARGET_GEOMETRY");return original;}
   FactoryNpcDrawEntryR977 selected;bool positive=factoryNpcDrawEntriesR977.TryGetValue(target,out selected)&&selected.state==target.npcState&&selected.npcTime==target.npcTime&&selected.sourceClock>0;
   float near=Math.Min(140f,target.aggroRange*.5f);
   if(d<=near||target.x<=game.player.x){factoryFirstInputStageR1017=3;RecordFactoryFirstDrawInputR1017("STOP",target.x<=game.player.x?"TARGET_NOT_TO_RIGHT":positive?"NEAR_APPROACH_WITH_POSITIVE_SOURCE_CLOCK":"NEAR_APPROACH_HALF_AGGRO_CAPPED_140",original,original,true);return original;}
   var input=new Input{move=1,drop=original.drop,dash=original.dash,dashPressed=original.dashPressed,jump=original.jump,attack=original.attack,attackHeld=original.attackHeld,attackX=original.attackX,attackXHeld=original.attackXHeld,attackXReleased=original.attackXReleased,potion=original.potion,potionPressedMask=original.potionPressedMask,awaken=original.awaken,skill=original.skill,skillHeldMask=original.skillHeldMask,skillReleasedMask=original.skillReleasedMask};
   string phase=factoryFirstInputStageR1017==1?"START_RIGHT":"RIGHT";factoryFirstInputStageR1017=2;RecordFactoryFirstDrawInputR1017(phase,"ORDINARY_MOVE_TO_NEAR_ORIGINAL_AGGRO",original,input,true);
   return factoryFirstInputStageR1017==3?original:input;
  }
  void ReleaseFactoryFirstDrawInputR1017(){
   if(factoryFirstInputStageR1017==1||factoryFirstInputStageR1017==2){factoryFirstInputStageR1017=3;RecordFactoryFirstDrawInputR1017("RELEASE","SESSION_END_NO_INPUT_SELECTION",null,null,false);}
   factoryFirstInputStageR1017=0;factoryFirstInputModelR1017=null;factoryFirstInputTargetR1017=null;
  }

  // R977 optional private observation state. Reference UIDs belong only to this
  // diagnostic session; they are not native IDs and never enter GameModel.
  [Serializable] public sealed class FactoryNpcDrawRowR977 {
   public int actorUID,index,room,encounter,frame,hostUpdate,rendererId,spriteId,textureId,drawFrame,drawCameraId,drawPass,pressUID,pressEpoch;
   public string npc,kind,state,key,motion,sourcePose,reason;
   public bool active,dormant,living,hasSelection,sourceShowAllowed,outsideFinite,isPress,pressSameOwner,rendererEnabled,hierarchyActive,sameWorldDraw,viewportHit,drawWitnessEligible;
   public float hp,npcTime,actorClock,sourceClock,alpha,visualScale;
   public float[] actorPosition,rendererPosition,scale,bounds,screenRect;
  }
  [Serializable] public sealed class FactoryNpcDrawInventoryR977 {
   public int frame,pass,cameraId,room,encounter,actorCount,maxActors=64;public bool complete,overflow,paused;public float modelElapsed,playerHP;
   public string reason;public FactoryNpcDrawRowR977[] rows=new FactoryNpcDrawRowR977[0];
  }
  sealed class FactoryNpcDrawEntryR977 {
   internal int uid,selectionFrame,drawFrame=-1,drawCameraId,drawPass,pressUID,pressEpoch;
   internal Enemy actor;internal SpriteRenderer renderer;internal Sprite sprite;internal CoreStopActualDrawWitnessV21 witness;
   internal string state,key,motion,pose;internal float npcTime,sourceClock;internal bool sourceShowAllowed,outsideFinite,isPress,pressSameOwner;
  }
  readonly Dictionary<Enemy,FactoryNpcDrawEntryR977> factoryNpcDrawEntriesR977=new Dictionary<Enemy,FactoryNpcDrawEntryR977>();
  bool factoryNpcDrawEnabledR977,factoryNpcDrawOverflowR977;int factoryNpcDrawUIDR977,factoryNpcDrawPassR977,factoryNpcDrawPassFrameR977=-1,factoryNpcDrawCameraR977;
  static bool FactoryDrawFiniteR977(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  static bool FactoryDrawTextR977(string x){return x!=null&&x.Length<=128;}
  FactoryNpcDrawEntryR977 FactoryDrawEntryR977(Enemy e){
   FactoryNpcDrawEntryR977 d;if(factoryNpcDrawEntriesR977.TryGetValue(e,out d))return d;
   if(game.enemies.Count>64||factoryNpcDrawEntriesR977.Count>=64){factoryNpcDrawOverflowR977=true;return null;}
   d=new FactoryNpcDrawEntryR977{actor=e,uid=++factoryNpcDrawUIDR977};factoryNpcDrawEntriesR977.Add(e,d);return d;
  }
  public bool BeginFactoryFirstNpcDrawDiagnosticsR977(){
   PreviewRequire(FactoryPreparedHostActiveR657&&!factoryNpcDrawEnabledR977,"One active current Factory diagnostic session");
   factoryNpcDrawEnabledR977=true;factoryNpcDrawOverflowR977=game.enemies.Count>64;
   if(!factoryNpcDrawOverflowR977)foreach(var e in game.enemies)FactoryDrawEntryR977(e);
   BeginFactoryFirstDrawInputR1017();
   return !factoryNpcDrawOverflowR977;
  }
  void RecordFactoryNpcSelectionR977(Enemy e,SpriteRenderer renderer,string key,string motion,string pose,float sourceClock,bool showAllowed,bool outside){
   if(!factoryNpcDrawEnabledR977)return;var d=FactoryDrawEntryR977(e);if(d==null)return;
   d.renderer=renderer;d.sprite=renderer.sprite;d.state=e.npcState;d.npcTime=e.npcTime;d.key=key;d.motion=motion;d.pose=pose;d.sourceClock=sourceClock;d.sourceShowAllowed=showAllowed;d.outsideFinite=outside;d.selectionFrame=Time.frameCount;
   d.isPress=e.sourceNPC=="NUI_FACTORY_PRESS"||e.sourceNPC=="NUI_FACTORY_PRESS3";d.pressSameOwner=false;d.pressUID=d.pressEpoch=0;
   if(d.isPress){FactoryPressFrameV569 f;if(factoryHostPressFramesR657.TryGetValue(e,out f)&&f!=null&&ReferenceEquals(f.actualOwner,e)&&f.npc==e.sourceNPC&&f.state==e.npcState&&f.sourcePoseTime==sourceClock){d.pressSameOwner=true;d.pressUID=f.uid;d.pressEpoch=f.epoch;}}
   if(!d.witness){d.witness=renderer.gameObject.AddComponent<CoreStopActualDrawWitnessV21>();forestPreviewOwned.Add(d.witness);d.witness.BindFactoryR977(this,e,renderer);}
  }
  public void BeginFactoryNpcDrawPassR977(Camera camera,int pass){
   if(!factoryNpcDrawEnabledR977||!FactoryPreparedHostActiveR657||camera!=worldCamera)return;
   factoryNpcDrawPassR977=pass;factoryNpcDrawPassFrameR977=Time.frameCount;factoryNpcDrawCameraR977=camera.GetInstanceID();
  }
  internal void ObserveFactoryNpcWorldDrawR977(Enemy actor,SpriteRenderer renderer,Camera camera,int frame){
   if(!factoryNpcDrawEnabledR977||!FactoryPreparedHostActiveR657||camera!=worldCamera||!camera||factoryNpcDrawPassFrameR977!=frame||factoryNpcDrawCameraR977!=camera.GetInstanceID()||factoryNpcDrawPassR977<=0)return;
   FactoryNpcDrawEntryR977 d;if(!factoryNpcDrawEntriesR977.TryGetValue(actor,out d)||!ReferenceEquals(d.actor,actor)||d.renderer!=renderer||d.selectionFrame!=frame||d.sprite!=renderer.sprite||d.state!=actor.npcState||d.npcTime!=actor.npcTime)return;
   d.drawFrame=frame;d.drawCameraId=camera.GetInstanceID();d.drawPass=factoryNpcDrawPassR977;
  }
  public FactoryNpcDrawInventoryR977 ReadFactoryNpcDrawInventoryR977(Camera camera,int pass){
   var v=new FactoryNpcDrawInventoryR977{frame=Time.frameCount,pass=pass,cameraId=camera?camera.GetInstanceID():0,room=game.room,encounter=game.EncounterIndex,actorCount=game.enemies.Count,paused=game.paused,modelElapsed=game.elapsed,playerHP=game.player.hp};
   if(!factoryNpcDrawEnabledR977||!FactoryPreparedHostActiveR657||camera!=worldCamera||!camera){v.reason="CURRENT_HOST_OR_CAMERA_UNAVAILABLE";return v;}
   if(factoryNpcDrawOverflowR977||game.enemies.Count>64){v.overflow=true;v.reason="ACTOR_INVENTORY_CAP_64";return v;}
   var rows=new List<FactoryNpcDrawRowR977>(game.enemies.Count);Rect viewport=camera.pixelRect;
   for(int i=0;i<game.enemies.Count;i++){
    var e=game.enemies[i];var d=FactoryDrawEntryR977(e);if(d==null){v.overflow=true;v.reason="ACTOR_REFERENCE_REGISTRY_CAP_64";return v;}
    if(!FactoryDrawTextR977(e.sourceNPC)||!FactoryDrawTextR977(e.kind)||!FactoryDrawTextR977(e.npcState)||!FactoryDrawFiniteR977(e.hp)||!FactoryDrawFiniteR977(e.npcTime)||!FactoryDrawFiniteR977(e.clock)||!FactoryDrawFiniteR977(e.x)||!FactoryDrawFiniteR977(e.y)||!FactoryDrawFiniteR977(e.nativeDepth)||!FactoryDrawFiniteR977(e.visualScale)){v.reason="NONFINITE_OR_OVERSIZE_ACTOR_METADATA";return v;}
    var r=new FactoryNpcDrawRowR977{actorUID=d.uid,index=i,room=game.room,encounter=e.encounterIndex,frame=Time.frameCount,hostUpdate=frame,npc=e.sourceNPC,kind=e.kind,state=e.npcState,key=d.key??"",motion=d.motion??"",sourcePose=d.pose??"",active=e.active,dormant=e.dormant,living=e.hp>0,hp=e.hp,npcTime=e.npcTime,actorClock=e.clock,sourceClock=d.sourceClock,visualScale=e.visualScale,actorPosition=new[]{e.x,e.y,e.nativeDepth},isPress=d.isPress,pressSameOwner=d.pressSameOwner,pressUID=d.pressUID,pressEpoch=d.pressEpoch,sourceShowAllowed=d.sourceShowAllowed,outsideFinite=d.outsideFinite,drawFrame=d.drawFrame,drawCameraId=d.drawCameraId,drawPass=d.drawPass,reason="NO_CURRENT_FINITE_SELECTION"};
    var s=d.renderer;r.hasSelection=d.selectionFrame==Time.frameCount&&s&&d.sprite==s.sprite&&d.state==e.npcState&&d.npcTime==e.npcTime&&!string.IsNullOrEmpty(d.key);
    if(s){r.rendererId=s.GetInstanceID();r.spriteId=s.sprite?s.sprite.GetInstanceID():0;r.textureId=s.sprite&&s.sprite.texture?s.sprite.texture.GetInstanceID():0;r.rendererEnabled=s.enabled;r.hierarchyActive=s.gameObject.activeInHierarchy;r.alpha=s.color.a;var p=s.transform.position;var z=s.transform.lossyScale;var b=s.bounds;r.rendererPosition=new[]{p.x,p.y,p.z};r.scale=new[]{z.x,z.y,z.z};r.bounds=new[]{b.center.x,b.center.y,b.center.z,b.size.x,b.size.y,b.size.z};
     float loX=float.PositiveInfinity,loY=float.PositiveInfinity,hiX=float.NegativeInfinity,hiY=float.NegativeInfinity;bool front=true;
     for(int k=0;k<8;k++){var q=camera.WorldToScreenPoint(new Vector3((k&1)==0?b.min.x:b.max.x,(k&2)==0?b.min.y:b.max.y,(k&4)==0?b.min.z:b.max.z));if(!FactoryDrawFiniteR977(q.x)||!FactoryDrawFiniteR977(q.y)||!FactoryDrawFiniteR977(q.z)){v.reason="NONFINITE_RENDERER_PROJECTION";return v;}front&=q.z>0;loX=Mathf.Min(loX,q.x);hiX=Mathf.Max(hiX,q.x);loY=Mathf.Min(loY,q.y);hiY=Mathf.Max(hiY,q.y);}
     r.screenRect=new[]{loX,loY,hiX-loX,hiY-loY};r.viewportHit=front&&hiX>viewport.xMin&&loX<viewport.xMax&&hiY>viewport.yMin&&loY<viewport.yMax;
    }
    if(!FactoryDrawFiniteR977(r.sourceClock)||!FactoryDrawFiniteR977(r.alpha)){v.reason="NONFINITE_SELECTION_OR_ALPHA";return v;}
    foreach(var values in new[]{r.actorPosition,r.rendererPosition,r.scale,r.bounds,r.screenRect})if(values!=null)foreach(float value in values)if(!FactoryDrawFiniteR977(value)){v.reason="NONFINITE_RENDERER_METADATA";return v;}
    r.sameWorldDraw=r.hasSelection&&r.drawFrame==Time.frameCount&&r.drawCameraId==camera.GetInstanceID()&&r.drawPass==pass;
    if(game.paused)r.reason="MODEL_PAUSED_NO_NEW_FIRST_DRAW";else if(!r.active||r.dormant||e.encounterIndex!=game.EncounterIndex)r.reason="INACTIVE_DORMANT_OR_OTHER_ENCOUNTER";else if(!r.living)r.reason="NOT_LIVING";else if(e.sourceNPC=="NUI_EVENT_NPC_NASOD_FACTORY")r.reason="HIDDEN_SOURCE_DIRECTOR_NOT_ONE_OF_ELEVEN";else if(!r.hasSelection)r.reason="NO_CURRENT_FINITE_SELECTION";else if(!FactoryDrawFiniteR977(r.sourceClock)||r.sourceClock<=0)r.reason="NO_POSITIVE_SOURCE_CLOCK";else if(!r.sourceShowAllowed)r.reason="SOURCE_STATE_SHOW_FALSE";else if(r.outsideFinite)r.reason="WALLY_OUTSIDE_FINITE_DISPLAY_HOLD";else if(r.isPress&&!r.pressSameOwner)r.reason="PRESS_OWNER_CLOCK_MISMATCH";else if(!r.rendererEnabled||!r.hierarchyActive||!FactoryDrawFiniteR977(r.alpha)||r.alpha<=0)r.reason="RENDERER_NOT_VISIBLE_ENABLED";else if(!r.sameWorldDraw)r.reason="NO_MATCHING_ON_WILL_RENDER_WORLD_PASS";else if(!r.viewportHit)r.reason="NO_VIEWPORT_INTERSECTION";else{r.drawWitnessEligible=true;r.reason="ACTUAL_FINITE_RENDERER_WORLD_CALLBACK";}
    rows.Add(r);
   }
   v.rows=rows.ToArray();v.complete=v.rows.Length==v.actorCount;v.reason=v.complete?"COMPLETE":"INCOMPLETE";return v;
  }
  void ReleaseFactoryNpcDrawDiagnosticsR977(){ReleaseFactoryFirstDrawInputR1017();factoryNpcDrawEnabledR977=false;factoryNpcDrawEntriesR977.Clear();factoryNpcDrawOverflowR977=false;factoryNpcDrawUIDR977=factoryNpcDrawPassR977=factoryNpcDrawCameraR977=0;factoryNpcDrawPassFrameR977=-1;}

  void ReleaseFactoryWholeBanksR684(){ReleaseFactoryNpcDrawDiagnosticsR977();factoryWholeSpritesR684.Clear();factoryWholeTexturesR684.Clear();factoryWallyLookupR684=null;factoryHealerLookupR684=null;factoryWholeLastNpcR684=factoryWholeLastStateR684=factoryWholeLastKeyR684="";factoryWholeLastNpcTimeR684=0;factoryReferenceLookupsR725.Clear();factoryReferenceResidentKeysR725=factoryReferenceMissingBirthHoldsR725=0;factoryReferenceLastSourceClockR725=0;}

  // Private mapping to two already frozen original files; not manifest/Resources IDs.
  const string FactoryStageMusicIdR742="factory-private-original-stage-r742",FactoryBossMusicIdR742="factory-private-original-boss-r742";
  static readonly string FactoryStageMusicPathR742=System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile).Replace('\\','/') + "/.codex/runtime-drafts/factory-native-scene-lookup-r676/original-audio/music020_stage.ogg",FactoryBossMusicPathR742=System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile).Replace('\\','/') + "/.codex/runtime-drafts/factory-native-scene-lookup-r676/original-audio/music011_boss.ogg";
  const string FactoryStageMusicShaR742="2aaee411a3f051bb88375b3996dca5228c8c3e33abbcb1c779459b9aaec47d31",FactoryBossMusicShaR742="85123135e880f97f1a5f1a7c42433eb91241fcc776bd41b4395a4e320083f068";
  sealed class FactoryMusicSnapshotR742 {
   internal bool engaged,detached,initialized,ready,paused,mapOwned;internal int active,trackChanges;internal float fade,mapVolume;internal string id;
   internal readonly AudioSource[] voices=new AudioSource[2],privateVoices=new AudioSource[2];internal readonly bool[] playing=new bool[2];internal readonly float[] levels=new float[2],from=new float[2],volumes=new float[2];
   internal AudioSource map,privateMap;internal bool mapPlaying;internal GameObject privateRoot;
   internal readonly Dictionary<string,SceneMusicAsset> priorAssets=new Dictionary<string,SceneMusicAsset>(StringComparer.Ordinal);internal bool assetsInstalled;
  }
  static bool FactoryPrivateMusicIdR742(string id){return id==FactoryStageMusicIdR742||id==FactoryBossMusicIdR742;}
  bool FactoryPrivateMusicRequestedR742(FactoryHostBindingsR657 b){
   if(b==null)return false;if(b.stages!=null)foreach(var s in b.stages)if(s!=null&&FactoryPrivateMusicIdR742(s.bgm))return true;
   if(b.presentation!=null&&b.presentation.audio!=null)foreach(var a in b.presentation.audio)if(a!=null&&FactoryPrivateMusicIdR742(a.id))return true;return false;
  }
  void ValidateFactoryPrivateMusicR742(FactoryHostBindingsR657 b){
   if(!FactoryPrivateMusicRequestedR742(b))return;PreviewRequire(b.stages!=null&&b.stages.Length==3&&b.presentation!=null&&b.presentation.audio!=null,"Private original BGM room/audio binding");
   for(int i=0;i<3;i++)PreviewRequire(b.stages[i]!=null&&b.stages[i].bgm==(i<2?FactoryStageMusicIdR742:FactoryBossMusicIdR742),"Private stage/stage/boss BGM mapping");
   foreach(string id in new[]{FactoryStageMusicIdR742,FactoryBossMusicIdR742}){int count=0;foreach(var a in b.presentation.audio)if(a!=null&&a.id==id){count++;bool stage=id==FactoryStageMusicIdR742;PreviewRequire(a.file!=null&&FactoryWholeSamePathR684(a.file.path,stage?FactoryStageMusicPathR742:FactoryBossMusicPathR742)&&a.file.sha256==(stage?FactoryStageMusicShaR742:FactoryBossMusicShaR742),"Exact frozen original BGM file/SHA");}PreviewRequire(count==1,"Exactly one private binding per original BGM");}
  }
  void CaptureFactoryMusicR742(FactoryHostSnapshotR657 snapshot){
   if(!FactoryPrivateMusicRequestedR742(factoryHostBindingsR657))return;
   var s=new FactoryMusicSnapshotR742{initialized=musicInitialized,ready=SceneMusicReady,paused=musicPaused,active=musicActive,fade=musicFade,id=musicId,trackChanges=MusicTrackChanges,map=mapMusicPreview,mapOwned=mapMusicOwnsClip};snapshot.music=s;
   for(int i=0;i<2;i++){s.voices[i]=musicVoices[i];s.playing[i]=s.voices[i]&&s.voices[i].isPlaying;s.volumes[i]=s.voices[i]?s.voices[i].volume:0;s.levels[i]=musicLevels[i];s.from[i]=musicFrom[i];}
   s.mapPlaying=s.map&&s.map.isPlaying;s.mapVolume=s.map?s.map.volume:0;foreach(string id in new[]{FactoryStageMusicIdR742,FactoryBossMusicIdR742}){SceneMusicAsset prior;if(sceneMusicAssets.TryGetValue(id,out prior))s.priorAssets.Add(id,prior);}
   s.engaged=true;
   // Preserve original clip/sample/paused state inside the original objects.
   // Only objects observed playing are paused here and UnPaused on restoration.
   for(int i=0;i<2;i++)if(s.playing[i])s.voices[i].Pause();if(s.mapPlaying)s.map.Pause();
   // Invalidate an old in-flight map request without rewinding its generation.
   // The old owned clip remains on the saved source, outside every Stop call.
   mapMusicGeneration++;mapMusicPreview=null;mapMusicOwnsClip=false;
   musicInitialized=true;SceneMusicReady=false;musicPaused=false;musicActive=-1;musicFade=1;musicId="";for(int i=0;i<2;i++)musicLevels[i]=musicFrom[i]=0;
   var root=new GameObject("Factory private original BGM R742");s.privateRoot=root;forestPreviewOwned.Add(root);root.transform.SetParent(transform,false);
   for(int i=0;i<2;i++){var voice=root.AddComponent<AudioSource>();s.privateVoices[i]=voice;voice.playOnAwake=false;voice.loop=true;voice.spatialBlend=0;voice.priority=100;voice.volume=0;musicVoices[i]=voice;}
   // Original Begin/StopMapMusicPreview may touch only this new owned source.
   var map=root.AddComponent<AudioSource>();s.privateMap=map;map.playOnAwake=false;map.loop=true;map.spatialBlend=0;map.volume=0;mapMusicPreview=map;
  }
  void InstallFactoryMusicR742(IDictionary<string,AudioClip> clips){
   var snapshot=factoryHostSnapshotR657;var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged)return;
   foreach(string id in new[]{FactoryStageMusicIdR742,FactoryBossMusicIdR742}){AudioClip clip;PreviewRequire(clips.TryGetValue(id,out clip)&&clip,"Private original BGM must be actually loaded by owned OGG path");}
   s.assetsInstalled=true;foreach(string id in new[]{FactoryStageMusicIdR742,FactoryBossMusicIdR742})sceneMusicAssets[id]=new SceneMusicAsset{id=id,resource="",gain=.22f};SceneMusicReady=true;
  }
  bool TryFactoryPrivateSceneMusicR742(string id,out AudioClip clip){
   clip=null;if(!factoryHostSessionR657||!FactoryPrivateMusicIdR742(id))return false;
   try{PreviewRequire(FactoryPreparedHostActiveR657&&factoryHostSnapshotR657!=null&&factoryHostSnapshotR657.music!=null&&!factoryHostSnapshotR657.music.detached&&audioClips.TryGetValue(id,out clip)&&clip,"Factory private original BGM ownership/load unavailable");return true;}
   catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}
  }
  void DetachFactoryMusicR742(){
   var snapshot=factoryHostSnapshotR657;var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged||s.detached)return;s.detached=true;SceneMusicReady=false;
   // This runs explicitly in End BEFORE DisposeForestPreview destroys clips.
   // The ReleaseWarning entry also covers direct Dispose/loading-failure paths.
   try{if(s.privateVoices[0])try{s.privateVoices[0].Stop();}finally{s.privateVoices[0].clip=null;}}
   finally{try{if(s.privateVoices[1])try{s.privateVoices[1].Stop();}finally{s.privateVoices[1].clip=null;}}
    finally{if(s.privateMap){if(ReferenceEquals(mapMusicPreview,s.privateMap))StopMapMusicPreview();else try{s.privateMap.Stop();}finally{s.privateMap.clip=null;}}}}
  }
  void RestoreFactoryMusicR742(FactoryHostSnapshotR657 snapshot){
   var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged)return;
   try{DetachFactoryMusicR742();}finally{
    if(s.assetsInstalled)foreach(string id in new[]{FactoryStageMusicIdR742,FactoryBossMusicIdR742}){SceneMusicAsset prior;if(s.priorAssets.TryGetValue(id,out prior))sceneMusicAssets[id]=prior;else sceneMusicAssets.Remove(id);}
    for(int i=0;i<2;i++){musicVoices[i]=s.voices[i];musicLevels[i]=s.levels[i];musicFrom[i]=s.from[i];}
    musicInitialized=s.initialized;SceneMusicReady=s.ready;musicPaused=s.paused;musicActive=s.active;musicFade=s.fade;musicId=s.id;MusicTrackChanges=s.trackChanges;
    mapMusicGeneration++;mapMusicPreview=s.map;mapMusicOwnsClip=s.mapOwned;
    s.engaged=false;
    // Never restart or UnPause a source originally stopped/paused. Holding its
    // original object also preserves loop/pitch/clip/sample state without seeks.
    for(int i=0;i<2;i++)if(s.voices[i]){s.voices[i].volume=audioMuted?0:s.volumes[i];if(s.playing[i])s.voices[i].UnPause();}
    if(s.map){s.map.volume=audioMuted?0:s.mapVolume;if(s.mapPlaying)s.map.UnPause();}
   }
  }

  void CloseFactoryHostAfterFailureR657(Exception trigger){if(!factoryHostSessionR657)return;try{EndFactoryPreparedHostR657();}catch(Exception cleanup){throw new AggregateException("Factory Host hook and cleanup both failed",trigger,cleanup);}}
  void TickFactoryHostAfterStepR657(){try{if(!factoryHostSessionR657||factoryHostComponentR657==null)return;PreviewRequire(FactoryPreparedHostActiveR657,"Factory ordinary Step model ownership changed");ConsumeFactoryPreparedAudioV21(factoryHostComponentR657);}catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}}
  void PrepareFactoryHostPresentationR657(){try{factoryHostPressFramesR657.Clear();if(!FactoryPreparedHostActiveR657)return;var stage=forestPreviewStages[game.room];PreviewRequire(stage.id==game.CurrentRoom.id&&stage.width==game.CurrentRoom.width&&stage.spawnX==game.CurrentRoom.spawnX&&stage.exitX==game.CurrentRoom.exitX,"Factory view projection differs from current original 2D model room");foreach(var f in game.ReadFactoryPressPresentationFramesR1470())factoryHostPressFramesR657.Add(f.actualOwner,f);factoryHostWarningR657.Sync();}catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}}
  void ApplyFactoryHostPressPoseR657(Enemy e,ref string clip,ref float clock){try{if(!FactoryPreparedHostActiveR657)return;FactoryPressFrameV569 f;if(factoryHostPressFramesR657.TryGetValue(e,out f)){clip=e.clip;clock=f.sourcePoseTime;}}catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}}
  void ValidateFactoryHostPoseR657(Enemy e,string clip){try{if(!FactoryPreparedHostActiveR657||e.sourceNPC=="NUI_EVENT_NPC_NASOD_FACTORY")return;Library art;PreviewRequire(!string.IsNullOrEmpty(clip)&&libraries.TryGetValue(e.kind,out art)&&art.frames.ContainsKey(clip)&&art.frames[clip].Length>0,"Factory current source pose is unbound: "+e.sourceNPC+"/"+clip);}catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}}
  void ApplyFactoryHostVisibilityR657(Enemy e,ref bool visible,ref bool gauge){try{if(FactoryPreparedHostActiveR657&&e.sourceNPC=="NUI_EVENT_NPC_NASOD_FACTORY"){visible=false;gauge=false;}}catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}}
  bool RenderFactoryHostStatusR657(){if(!factoryHostSessionR657)return false;try{EnsureForestPreviewNotice();forestPreviewNotice.text=forestPreviewLoading?"NASOD FACTORY / LOADING ASSETS":"NASOD FACTORY / CANDIDATE  |  TAB: RETURN";if(FactoryPreparedHostActiveR657){TextValue("Zone","ALTERA / PREVIEW "+(game.room+1)+" OF 3");if(openMenu=="loadout")TextValue("Menu Subtitle","FACTORY CANDIDATE: loadout changes are session only. K continues; Tab returns.");}return true;}catch(Exception trigger){CloseFactoryHostAfterFailureR657(trigger);throw;}}
  void ReleaseFactoryHostWarningR657(){try{try{DetachFactoryMusicR742();}finally{if(factoryHostWarningR657!=null)factoryHostWarningR657.Dispose();}}finally{factoryHostWarningR657=null;factoryHostPressFramesR657.Clear();if(factoryHostWarningRootR657)Destroy(factoryHostWarningRootR657);factoryHostWarningRootR657=null;ReleaseFactoryWholeBanksR684();}}
  void ReleaseFactoryHostBindingsR657(){if(!factoryHostSessionR657)return;var prior=factoryHostSnapshotR657;var component=factoryHostComponentR657;try{foreach(var id in factoryHostLibrariesR657)libraries.Remove(id);foreach(var p in factoryHostPriorLibrariesR657)libraries[p.Key]=p.Value;foreach(var id in factoryHostSoundsR657)audioClips.Remove(id);foreach(var p in factoryHostPriorSoundsR657)audioClips[p.Key]=p.Value;}finally{factoryHostLibrariesR657.Clear();factoryHostPriorLibrariesR657.Clear();factoryHostSoundsR657.Clear();factoryHostPriorSoundsR657.Clear();factoryHostComponentR657=null;factoryHostBindingsR657=null;factoryHostSessionR657=false;if(prior!=null){game=prior.game;game.paused=prior.paused;}try{RestoreFactoryMusicR742(prior);}finally{factoryHostSnapshotR657=null;if(component!=null)component.Dispose();}}}
  void RestoreFactoryHostSnapshotR657(FactoryHostSnapshotR657 s){if(s==null)return;game=s.game;game.paused=s.paused;selectedDungeon=s.selected;previewDungeon=s.preview;adventureStarted=false;openMenu=s.menu;menuOrigin=s.origin;pausedBeforeMenu=s.pausedMenu;mapRegion=s.region;mapSelected=s.mapSelected;mapPage=s.mapPage;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=s.accumulator;try{ClearInput();RebuildRoom();ResetAudioTracking();}finally{adventureStarted=s.adventure;try{RenderMenus();}finally{worldCamera.targetTexture=s.target;worldCamera.rect=s.rect;worldCamera.aspect=s.aspect;worldCamera.orthographic=s.ortho;worldCamera.orthographicSize=s.size;worldCamera.fieldOfView=s.fov;worldCamera.nearClipPlane=s.near;worldCamera.farClipPlane=s.far;worldCamera.transform.SetPositionAndRotation(s.position,s.rotation);worldCamera.projectionMatrix=s.projection;}}}
  public void EndFactoryPreparedHostR657(){if(!factoryHostSessionR657)return;var s=factoryHostSnapshotR657;try{game.paused=true;try{StopAudioVoices();}finally{try{DetachFactoryMusicR742();}finally{try{DisposeNativeWorld();}finally{DisposeForestPreview();}}}}finally{try{ReleaseFactoryHostWarningR657();}finally{try{ReleaseFactoryHostBindingsR657();}finally{RestoreFactoryHostSnapshotR657(s);}}}}
 }
}
#endif
