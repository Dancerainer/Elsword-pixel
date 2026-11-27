using System;
namespace ElswordPixel {
 [Serializable] public sealed class BattleProductSourceRulesHeaderR1341 {public string sourceRulesSha256;}
 // Data declarations only. Original rooms, inactive actors and separate director remain distinct.
 [Serializable] public sealed class BattleProductPinR1282 {public string path,sha256;}
 [Serializable] public sealed class BattleProductModelR1282 {
  public string schema,sourceGraph,sourceLuaSha256;public bool runtimeAdmitted;public BattleProductRoomR1282[] rooms;
  public BattleProductDirectorSpawnR1282[] sourceVisualActorSpawns;
 }
 [Serializable] public sealed class BattleProductRoomR1282 {
  public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;
  public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;public string[] bgm;
 }
 [Serializable] public sealed class BattleProductDirectorSpawnR1282 {
  public int roomIndex;public string roomId,worldId,sourceProfile;public bool inOrdinaryEnemies;public EnemySpawn sourceSpawn;
 }
 [Serializable] public sealed class BattleProductAuditR1282 {public BattleProductAuditRoomR1282[] rooms;}
 [Serializable] public sealed class BattleProductAuditRoomR1282 {public BattleProductAuditBirthR1282[] enemies;}
 [Serializable] public sealed class BattleProductAuditBirthR1282 {public string sourceNPC;public bool sourceActive,aiEnabled;public int sourceSpawnIndex;}
 [Serializable] public sealed class BattleProductRulesR1282 {public bool runtimeAdmitted,sourceMechanismsComplete;public NpcDefinition[] definitions,sourceVisualActors;}
 [Serializable] public sealed class BattleProductClocksR1282 {public BattleProductClockProfileR1282[] definitions;}
 [Serializable] public sealed class BattleProductClockProfileR1282 {public string id,kind;public BattleProductClockR1282[] states;}
 [Serializable] public sealed class BattleProductClockR1282 {public string id,clip,sourceAnimation;public float duration,sourceMotionDuration,sourceAnimSpeed;public bool loop;}
 [Serializable] public sealed class BattleProductPathR1282 {public float s,length;public float[] start,end;}
 [Serializable] public sealed class BattleProductProjectionR1282 {public string id;public float unit,offsetX,offsetY,width,height;public BattleProductPathR1282[] path;}
 [Serializable] public sealed class BattleProductScenePartR1282 {public string name,texture;public int offset,count;public float[] color,fogColor;public float fogNear,fogFar,fogHeightStart,fogHeightEnd,fogBias;}
 [Serializable] public sealed class BattleProductSceneR1282 {public string id;public float unit,offsetX,offsetY;public BattleProductPathR1282[] path;public BattleProductScenePartR1282[] parts;}
 [Serializable] public sealed class BattleProductAtlasR1282 {public string image;public int frameWidth,frameHeight;public float[] groundAnchor;}
 [Serializable] public sealed class BattleProductFrameR1282 {public int[] rect;public float[] anchor;}
 [Serializable] public sealed class BattleProductClipR1282 {public string id;public float fps,durationSeconds;public bool loop;public BattleProductFrameR1282[] frames;}
 [Serializable] public sealed class BattleProductLibraryR1282 {
  public string id,sourceNPC,artVersion,sourceRulesSha256;public bool runtimeAdmitted,animationLibraryApproved;
  public float logicalScale,standingHeight;public int paintedPoseCount;public BattleProductAtlasR1282 atlas;public BattleProductClipR1282[] clips;
 }
 [Serializable] public sealed class BattleProductDeferredR1282 {
  public string schema,sourceRulesSha256;public BattleProductPinR1282 originalAdapter,normalBalance;
  public int originalHp,authoredPreviewHp,originalMissingStart;public bool coordinateInvented;public AlteraDeferredCannonGate[] definitions;
 }
 [Serializable] public sealed class BattleProductAudioR1282 {
  public string schema,sourceRulesSha256,sourceSoundsSha256;public BattleProductPinR1282 sourceSoundMap;
  public int sourceAssetCount,previewOwnedClipCount,reusePublishedClipCount;public bool resampled,normalized,trimmed;public BattleProductAudioRowR1282[] rows;
 }
 [Serializable] public sealed class BattleProductAudioRowR1282 {
  public string id,decodedPcm16Sha256,publishedResource;public string[] sourceNames;public BattleProductPinR1282 original,pcm;
  public int sourcePcmFrames,channels,sampleRate;public float durationSeconds;public bool reusePublishedClip,previewOwnsClip;
 }
}
