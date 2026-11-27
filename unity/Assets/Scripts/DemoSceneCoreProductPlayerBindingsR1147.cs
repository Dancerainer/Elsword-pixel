#if !UNITY_EDITOR
using System;using System.Collections;using System.Collections.Generic;using System.IO;using System.Security.Cryptography;using System.Text;using UnityEngine;using UnityEngine.UI;
namespace ElswordPixel {
 [Serializable] public sealed class CoreIntroFollowStateV25 {public float y,velocity;public int room;public static bool Same(CoreIntroFollowStateV25 a,CoreIntroFollowStateV25 b){return a!=null&&b!=null&&a.y==b.y&&a.velocity==b.velocity&&a.room==b.room;}}
 [Serializable] public sealed class CoreIntroCameraStateV25 {
  public int instanceID,targetInstanceID;public bool alive,enabled,orthographic,targetNull;public Vector3 position;public Quaternion rotation;public Rect rect;public float size,aspect,fov,near,far;
  public static CoreIntroCameraStateV25 Read(Camera c){if(!c)return new CoreIntroCameraStateV25();return new CoreIntroCameraStateV25{alive=true,instanceID=c.GetInstanceID(),targetInstanceID=c.targetTexture?c.targetTexture.GetInstanceID():0,targetNull=c.targetTexture==null,enabled=c.enabled,orthographic=c.orthographic,position=c.transform.position,rotation=c.transform.rotation,rect=c.rect,size=c.orthographicSize,aspect=c.aspect,near=c.nearClipPlane,far=c.farClipPlane,fov=c.fieldOfView};}
  public static bool Same(CoreIntroCameraStateV25 a,CoreIntroCameraStateV25 b){return a!=null&&b!=null&&a.alive&&b.alive&&a.instanceID==b.instanceID&&a.targetInstanceID==b.targetInstanceID&&a.targetNull==b.targetNull&&a.enabled==b.enabled&&a.orthographic==b.orthographic&&a.position==b.position&&Quaternion.Angle(a.rotation,b.rotation)<.001f&&a.rect==b.rect&&Math.Abs(a.size-b.size)<.00001f&&Math.Abs(a.aspect-b.aspect)<.00001f&&a.near==b.near&&a.far==b.far&&a.fov==b.fov;}
 }
 [Serializable] public sealed class CoreIntroRestoreV25 {public CoreStopWorldAspectEvidenceV28 worldAspectV28;public int frame;public CoreIntroCameraStateV25 savedWorld,savedNative,restoredWorld,restoredNative;public Vector3 savedScreenPosition,savedScreenScale,restoredScreenPosition,restoredScreenScale;public CoreIntroFollowStateV25 savedFollow,restoredFollow;public bool matched;}
 [Serializable] public sealed class CoreIntroActualViewV25 {public long ticket,generation;public int room,encounter,order,frame,drawFrame,projectedFrame,appliedFrame,rootInstanceID,rendererInstanceID;public string npc,state;public float sourceClock;public Bounds bodyBounds;public Vector3 viewportMin,viewportMax;public Rect safeViewport;public CoreIntroCameraStateV25 world,native;public bool actualWorldCameraPostRender,actualWorldDrawWitness,wholeSourceBodyInSafeViewport,worldTargetNull;}
 [Serializable] public sealed class CoreIntroBridgeDiagnosticsV25 {public int frame,projectedFrame,drawFrame,observedFrame,appliedFrame,worldCallbacks,worldDraws,targetRejected,validObservations,submitted,restores;public string pendingFailure;public bool scopeActive,worldTargetNull;public CoreIntroActualViewV25 lastActual;public CoreStopWholeFramingEvidenceV22 lastWholePlanV22,lastInvalidWholeV22,lastWholeActualV22;}

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
byte[] PreviewRead(PreviewFile file){return CoreEntryReadV21(file);}
T PreviewJson<T>(PreviewFile file){return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(PreviewRead(file)));}
  Texture2D PreviewTextureFile(PreviewFile file){
   Texture2D result;if(forestPreviewTextures.TryGetValue(file.path,out result))return result;
   result=new Texture2D(2,2,TextureFormat.RGBA32,false){name="EDITOR PREVIEW / "+Path.GetFileName(file.path),filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   forestPreviewOwned.Add(result);PreviewRequire(result.LoadImage(PreviewRead(file)),"PNG decode failed "+file.path);forestPreviewTextures.Add(file.path,result);return result;
  }
  Texture2D ForestPreviewTexture(string source){Texture2D texture;PreviewRequire(forestPreviewTextureBindings.TryGetValue(source,out texture),"unbound original scene texture "+source);return texture;}
 sealed class CoreEntryAdmittedSnapshotV21 {internal int selected,preview;internal string[] loadout;internal bool paused,adventure,pausedMenu;internal string menu,origin;internal Rect viewport;}
 readonly CoreEntryBeginOwnershipV21 coreEntryBeginOwnerV21=new CoreEntryBeginOwnershipV21();
 CoreEntryAdmittedSnapshotV21 coreEntryAdmittedSnapshotV21;IEnumerator coreEntryOwnedLoadWorkV21;
 string coreEntryFaultPointV21;
 void CoreEntryFaultV21(string point){}
 void MarkCoreHardEntryActiveV21(){coreEntryBeginOwnerV21.MarkActive();}
 void DisposeCoreHardEntryOwnedLoadV21(){var work=coreEntryOwnedLoadWorkV21;coreEntryOwnedLoadWorkV21=null;try{(work as IDisposable)?.Dispose();}finally{var snap=coreEntryAdmittedSnapshotV21;if(snap!=null&&worldCamera)worldCamera.rect=snap.viewport;coreEntryBeginOwnerV21.Reset();coreEntryAdmittedSnapshotV21=null;coreRoom4ComponentRequestedR908=coreRoom4ComponentInitializedR908=false;coreRoom4ComponentInitFrameR908=-1;coreRoom4ComponentInitModelR908=0;coreRoom4ComponentInitStepR908=0;}}
 const bool coreIntroProbeAllowSubmissionV25=true;
 void ConnectCoreIntroProbeScopeV25(){}
 void RetireCoreIntroProbeScopeV25(){}

}}
#endif
