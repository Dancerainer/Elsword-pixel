using System;
namespace ElswordPixel {
    [Serializable] public class ArtCatalog { public SpriteLibraryData hero; public SpriteLibraryData[] monsters; public SpriteLibraryData[] effects; public StageArt[] stages; public DungeonArt[] dungeons; public SkillIconData[] icons;public NpcDefinition[] npcRules;public NpcEffectDocument npcEffectRules;public NpcSoundDocument npcSoundRules;public NpcMechanismDocument npcMechanismRules;public NpcSoundResourceData[] npcSounds;public IterationAdmissionData iterationAdmission;public NpcContentSetData[] npcContentSets; }
    [Serializable] public class NpcContentSetData {public string sourceDungeon;public NpcDefinition[] npcRules;public NpcEffectDocument npcEffectRules;public NpcSoundDocument npcSoundRules;public NpcMechanismDocument npcMechanismRules;public IterationAdmissionData admission;public NpcSteamVisualData steamVisual;public OutskirtsNpcVisualResources outskirtsVisual;public LakeNpcVisualResourcesV18 lakeVisual;public AlteraDeferredCannonGate[] alteraDeferredCannonGates;
  public DragonNestPackagedVisualResourcesV20 dragonNestVisual;}
    [Serializable] public class DungeonArt {public string id,title,boss,description,region,sourceGraph;public int start,count,combatZones;public bool iterationPreview;}
    [Serializable] public class NpcSoundResourceData {public string id,resource;}
    [Serializable] public class IterationAdmissionData {public string schema,sourceDungeon,bundleSha256,reviewSha256;public bool reviewed;public string[] npcIds,effectIds,sourceWorldIds,residualNotes;}
    // Candidate documents are map metadata, never an override of admitted art.
    [Serializable] public class RegionCandidateCatalog {
        public string schema;public bool runtimeAdmitted;public CandidateDungeonArt[] dungeons;
        public static int MatchAdmittedRoute(ArtCatalog catalog,CandidateDungeonArt candidate){
            if(candidate==null||candidate.rooms==null||candidate.rooms.Length==0||catalog==null||catalog.dungeons==null||catalog.stages==null)return -1;
            for(int i=0;i<catalog.dungeons.Length;i++){
                var route=catalog.dungeons[i];if(route.count!=candidate.rooms.Length||route.start<0||route.start+route.count>catalog.stages.Length)continue;
                bool same=true;for(int j=0;j<route.count;j++)if(candidate.rooms[j]==null||string.IsNullOrEmpty(candidate.rooms[j].id)||catalog.stages[route.start+j].sourceWorldId!=candidate.rooms[j].id)same=false;
                if(same)return i;
            }return -1;
        }
    }
    [Serializable] public class CandidateDungeonArt {
        public string id,title,region,difficulty,description,sourceGraph,model;
        public int start,count,combatZones,runtimeIndex=-1;
        public bool runtimeAdmitted;public string[] missingMonsterArt;public CandidateRoomArt[] rooms;
    }
    [Serializable] public class CandidateRoomArt {
        public string id,bgm,bgmPath,bgmSha256;public float width;
        public int zones,spawnCount;public bool bgmAvailable;
    }
    [Serializable] public class SkillIconData {public string id,texture;public int sourceSkillId;}
    [Serializable] public class StageArt {
        public string id, background, foreground,flatForeground, title,nativeScene,nativeGeometry,sourceWorldId,bgm;
        public float width=480,height=270, spawnX=60, exitX=438;
        public bool hasSpawnY;public float spawnY=54;public int entryEncounter;
        public EncounterDefinition[] encounters;
        public GroundNode[] groundNodes;
        public GroundSurface[] surfaces;
        public GroundTeleport[] teleports;
        public int[] bungeeLineIds;
        public SpawnArt[] spawns;
    }
    [Serializable] public class SpawnChoiceArt { public string kind,sourceNPC; public int weight; }
    [Serializable] public class SpawnPositionArt { public float x,y,nativeDepth;public int groundLine,facing,sourceSpawnIndex; }
    [Serializable] public class SpawnArt { public SpawnPositionArt[] positions;public SpawnChoiceArt[] choices;public int spawnChance;public string kind,sourceNPC,spawnAfterDeath="";public int groundLine=-1,facing=-1,sourceSpawnIndex,encounterIndex,sourceKeyCode;public float x,y=54,aggroRange=180,nativeDepth,visualScale=1,spawnDelay,hp;public bool passive,dormant,aiEnabled=true,countsForClear=true,isGate,siegeMode; }
    [Serializable] public class SpriteLibraryData {
        public string id,texture,artVersion;
        public int paintedPoseCount;
        public float standingHeight;
        public int frameWidth=192,frameHeight=192;
        public float anchorX=96,anchorY=128;
        public float logicalScale=1;
        public AnimationData[] clips;
    }
    [Serializable] public class AnimationData {
        public string id,texture;public float fps=30,duration,rootDistance;public bool loop;
        public CommandCalibration command;
        public FrameData[] frames;public HitData[] hits;
    }
    [Serializable] public class FrameData { public int x,y,w,h;public string texture;public bool hasAnchor;public float anchorX,anchorY,rootX;public AttackGeometryData attackGeometry; }
    [Serializable] public class AttackGeometryData { public float[] weaponBase,weaponTip,weaponAttackStart,weaponAttackEnd,rightFoot,leftFoot,rightFootStart,rightFootEnd,leftFootStart,leftFootEnd; }
    [Serializable] public class HitData { public float start,end,repeatInterval,damageScale=1;public int rawReaction;public bool damageDefined=true;public string[] enabledAttackBoxes; }
}
