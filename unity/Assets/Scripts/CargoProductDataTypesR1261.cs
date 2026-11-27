using System;
namespace ElswordPixel {
 // Ordinary data DTOs copied from the original Cargo/Forest C consumer fields.
 // No model construction, actor writes, state assignment or activation policy.
 [Serializable] public sealed class CargoProductModelR1261 {public string sourceGraph;public bool runtimeAdmitted;public CargoProductRoomR1261[] rooms;}
 [Serializable] public sealed class CargoProductRoomR1261 {
  public string id,title,sourceWorldId;public float width,spawnX,spawnY,exitX;public bool hasSpawnY;public int entryEncounter;
  public GroundNode[] groundNodes;public GroundSurface[] surfaces;public EncounterDefinition[] encounters;public EnemySpawn[] enemies;public string[] bgm;
 }
 [Serializable] public sealed class CargoProductAuditR1261 {public CargoProductAuditRoomR1261[] rooms;}
 [Serializable] public sealed class CargoProductAuditRoomR1261 {public CargoProductAuditBirthR1261[] enemies;}
 [Serializable] public sealed class CargoProductAuditBirthR1261 {public string sourceNPC;public bool sourceActive,aiEnabled;}
 [Serializable] public sealed class CargoProductRulesR1261 {public bool runtimeAdmitted,sourceMechanismsComplete;public NpcDefinition[] definitions;}
 [Serializable] public sealed class CargoProductPathR1261 {public float s,length;public float[] start,end;}
 [Serializable] public sealed class CargoProductProjectionR1261 {public string id;public float unit,offsetX,offsetY,width,height;public CargoProductPathR1261[] path;}
 [Serializable] public sealed class CargoProductScenePartR1261 {public string texture;public int offset,count;}
 [Serializable] public sealed class CargoProductSceneR1261 {public string id;public CargoProductScenePartR1261[] parts;}
 [Serializable] public sealed class CargoProductAtlasR1261 {public string image;public int frameWidth,frameHeight;public float[] groundAnchor;}
 [Serializable] public sealed class CargoProductFrameR1261 {public int[] rect;public float[] anchor;}
 [Serializable] public sealed class CargoProductClipR1261 {public string id;public float fps,durationSeconds;public bool loop;public CargoProductFrameR1261[] frames;}
 [Serializable] public sealed class CargoProductLibraryR1261 {
  public string id,sourceNPC,artVersion,sourceRulesSha256;public bool runtimeAdmitted,animationLibraryApproved;
  public float logicalScale,standingHeight;public int paintedPoseCount;public CargoProductAtlasR1261 atlas;public CargoProductClipR1261[] clips;
 }
 [Serializable] public sealed class CargoProductSourcePinR1261 {public string path,sha256;}
 [Serializable] public sealed class CargoProductStateClipR1261 {
  public string npc,kind,state,sourceAnimation,compiledClip,displayClip;public bool sourceStateClockUnchanged;
  public float sourceStateDuration,sourceAnimSpeed,displayDuration;public bool displayLoop;public CargoProductSourcePinR1261 animations;
 }
 [Serializable] public sealed class CargoProductStateClipsR1261 {public int sourceStates;public CargoProductStateClipR1261[] rows;public bool physicalUnboundPolicyReady;public string[] problems;}
}
