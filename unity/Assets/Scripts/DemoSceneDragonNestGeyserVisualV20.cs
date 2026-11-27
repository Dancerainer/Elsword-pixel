using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class DragonNestGeyserCallV20 {public int index,timerPC,thresholdPC,createPC,literalPC;public float at;public string key;public bool sourceSetAddRotate,sourceSetAxisAngle;}
 [Serializable] public sealed class DragonNestGeyserSpawnV20 {public string roomId;public float x,y,nativeDepth;public int groundLine,sourceSpawnIndex,facing;}
 [Serializable] public sealed class DragonNestGeyserSpawnRowsV20 {public DragonNestGeyserSpawnV20[] rows;}
 [Serializable] public sealed class DragonNestGeyserDocumentV20 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,sourceEvidenceSha256,particleSourceManifestSha256,sourceInstructionsSha256,sourceSpawnsJson,sourceSpawnsJsonSha256;
  public bool runtimeAdmitted,actualUnityVerified;public string npc,state,manager,bone,clock,rotationAdapter,seedAdapter,planeAdapter,lifecycleAdapter;
  public float unit,sourceAttackDuration,visualOffsetY,damageOnlyOffsetY;public float[] boneOffset;
  public DragonNestGeyserCallV20[] calls;public DragonEffectParticleEntryV20[] particles;
 }
 public sealed class DragonNestGeyserEmissionV20 {
  public int sequence,actorSerial,callIndex,sourceAttackSequence;public float time,x,y,z,depth;public string key;public bool sourceSetAddRotate,sourceSetAxisAngle;public Vector3 logicalSourceEuler;public Enemy owner;
 }
 public sealed partial class DemoScene {
  DragonNestGeyserDocumentV20 dragonGeyserDocumentV20;DragonNestGeyserSpawnV20[] dragonGeyserSourceSpawnsV20;
  readonly Dictionary<string,DragonEffectParticleViewV20> dragonGeyserViewsV20=new Dictionary<string,DragonEffectParticleViewV20>();
  sealed class DragonGeyserCursorV20 {internal Enemy owner;internal int actorSerial,attackSequence,mask;internal float began;}
  readonly Dictionary<Enemy,DragonGeyserCursorV20> dragonGeyserCursorsV20=new Dictionary<Enemy,DragonGeyserCursorV20>();
  readonly List<Enemy> dragonGeyserPruneV20=new List<Enemy>();int dragonGeyserLastSequenceV20,dragonGeyserEmissionSequenceV20;
  public readonly List<DragonNestGeyserEmissionV20> DragonNestGeyserEmissionsV20=new List<DragonNestGeyserEmissionV20>();
  public int DragonNestGeyserCreatedSequencesV20 {get;private set;}
  public int DragonNestGeyserWarningSequencesV20 {get;private set;}
  public int DragonNestGeyserFireSequencesV20 {get;private set;}
  public int DragonNestGeyserAttackEventsV20 {get;private set;}
  public int DragonNestGeyserRejectedEventsV20 {get;private set;}
  public int DragonNestGeyserCursorCountV20 {get{return dragonGeyserCursorsV20.Count;}}
  public int DragonNestGeyserVisibleSpriteCountV20 {get{int n=0;foreach(var sprite in dragonEffectParticleSpritesV20)if(sprite&&sprite.enabled&&sprite.gameObject.activeInHierarchy)foreach(var view in dragonGeyserViewsV20.Values)if(sprite.sprite==view.sprite)n++;return n;}}
  public int DragonNestGeyserKindsDrawnV20 {get{int n=0;foreach(var view in dragonGeyserViewsV20.Values)if(dragonEffectParticleKindsV20.Contains(view.data.id))n++;return n;}}
  static void RequireDragonGeyserV20(bool value,string reason){if(!value)throw new InvalidOperationException("Dragon original Geyser: "+reason);}
  static bool NearDragonGeyserV20(float a,float b){return DragonEffectParticleFiniteV20(a)&&Mathf.Abs(a-b)<.0001f;}
  public void ConfigureDragonNestGeyserCallbacksV20(DragonNestGeyserDocumentV20 doc,IDictionary<string,Texture2D> textures){
   RequireDragonGeyserV20(doc!=null&&doc.schema=="besma-dragon-nest-original-geyser-callback-visual-v20"&&!doc.runtimeAdmitted&&!doc.actualUnityVerified&&
    doc.sourceRulesSha256=="0a78bd2021b407841b0389f47bdfce44af64ce0a2c352187b2393cbdb069dd77"&&doc.sourceEffectsSha256=="c279abde43dd2f98905a4ee93fe033d606f5f70d9b16fcd1386c1758439eaad4"&&doc.sourceEvidenceSha256=="ebbe209016cc06b5caf79e9e88b8e00de94478c9a4dfe69a686eb965698c623d"&&doc.particleSourceManifestSha256=="6133bf4fbabd5c26e4853663629a4712e9ac5832af3c3066a4501b0311f2a2b3"&&doc.sourceInstructionsSha256=="0d1e8fbd06ab056bd6943071ebcf8f1607c7ba7ead9cc37cd8734b9d08af113b","Changed source/typed schema binding");
   RequireDragonGeyserV20(doc.npc=="NUI_STEAM_GEYSER"&&doc.state=="STEAM_GEYSER_ATTACK"&&doc.manager=="Major"&&doc.bone=="Dummy01"&&doc.clock=="source-state-seconds"&&doc.unit==.28f&&doc.sourceAttackDuration==5&&doc.visualOffsetY==0&&doc.damageOnlyOffsetY==230&&doc.boneOffset!=null&&doc.boneOffset.Length==3&&NearDragonGeyserV20(doc.boneOffset[0],0)&&NearDragonGeyserV20(doc.boneOffset[1],2.5199997f)&&NearDragonGeyserV20(doc.boneOffset[2],-.00000028f),"State clock or original Dummy01 was replaced by a motion frame/sphere/damage offset");
   RequireDragonGeyserV20(doc.rotationAdapter=="source-logical-X-forward-facing-yaw0-or180-degrees-v20"&&doc.seedAdapter=="attack-event-sequence-actor-call-deterministic-display-seed-v20"&&doc.planeAdapter=="original-BT_ALL-XY-billboard-source-Zspin-owner-Euler-offset-projection-v20"&&doc.lifecycleAdapter=="source-attack-event-with-nine-fixed-source-owners-state-seconds-once-independent-particle-lifetime-room-reset-v20","Unrecorded native rotation/seed/lifecycle claim");
   float[] times={.01f,1.01f,3};int[] timer={1,38,75},threshold={2,39,76},create={24,61,98},literal={26,63,100};RequireDragonGeyserV20(doc.calls!=null&&doc.calls.Length==3,"Missing original callback callsites");
   for(int i=0;i<3;i++){var c=doc.calls[i];RequireDragonGeyserV20(c!=null&&c.index==i&&c.at==times[i]&&c.key==(i==2?"Major/SteamGeyser_FireAttack01":"Major/SteamGeyser_FireAttack02")&&c.timerPC==timer[i]&&c.thresholdPC==threshold[i]&&c.createPC==create[i]&&c.literalPC==literal[i]&&c.sourceSetAddRotate==(i==2)&&c.sourceSetAxisAngle==(i==2),"Changed original callback time/manager/flags");}
   RequireDragonGeyserV20(doc.sourceSpawnsJsonSha256=="b91125601c12c8798b663879b730950513dc1aa0c55cdc022930b2cbbcd0667c"&&DragonEffectParticleHashV20(doc.sourceSpawnsJson)==doc.sourceSpawnsJsonSha256,"Original nine source positions changed");var source=JsonUtility.FromJson<DragonNestGeyserSpawnRowsV20>(doc.sourceSpawnsJson);RequireDragonGeyserV20(source!=null&&source.rows!=null&&source.rows.Length==9,"Missing original source population");
   RequireDragonGeyserV20(doc.particles!=null&&doc.particles.Length==2&&textures!=null,"Missing two original Major templates");string[] keys={"Major/SteamGeyser_FireAttack02","Major/SteamGeyser_FireAttack01"},hashes={"9f29c4dd54f3098df113acf64589bdc311e2e5c3b369e2c6fad04412dd118882","0c133201305df6b4bd26f01da7e58114afa5b54da1f357528d5d911bafc973fd"};var next=new Dictionary<string,DragonEffectParticleViewV20>();
   try{for(int i=0;i<2;i++){var p=doc.particles[i];RequireDragonGeyserV20(p!=null&&p.key==keys[i]&&p.supported&&p.particleJsonSha256==hashes[i]&&DragonEffectParticleHashV20(p.particleJson)==p.particleJsonSha256&&DragonEffectParticleHashV20(p.sourceTemplateBlob)==p.sourceTemplateSha256&&p.textureKey=="textures/smoke02.png"&&p.textureRgbaSha256=="1040443546966a2ced629575be45091d2c1fb45e8baf2baee7837535ddc46c99","Changed original normalized template/texture");Texture2D texture;RequireDragonGeyserV20(textures.TryGetValue(p.textureKey,out texture),"Missing original smoke02");next.Add(p.key,CreateDragonNestGeyserSourceParticleViewV20(JsonUtility.FromJson<OutskirtsVisualParticle>(p.particleJson),texture,p.sourceTemplateSha256));}}
   catch{foreach(var v in next.Values)DisposeDragonNestGeyserSourceParticleViewV20(v);throw;}
   DisposeDragonNestGeyserSourceParticlesV20();dragonGeyserDocumentV20=doc;dragonGeyserSourceSpawnsV20=source.rows;foreach(var p in next)dragonGeyserViewsV20.Add(p.Key,p.Value);DragonNestGeyserCreatedSequencesV20=DragonNestGeyserWarningSequencesV20=DragonNestGeyserFireSequencesV20=DragonNestGeyserAttackEventsV20=DragonNestGeyserRejectedEventsV20=0;ResetDragonNestGeyserParticleCursorsV20(false);
  }
  bool DragonGeyserOriginalOwnerV20(Enemy owner){
   if(owner==null||dragonGeyserDocumentV20==null||owner.sourceNPC!=dragonGeyserDocumentV20.npc||!game.enemies.Contains(owner)||!owner.active||owner.dormant||owner.hp<=0)return false;
   foreach(var p in dragonGeyserSourceSpawnsV20)if(p.roomId==game.CurrentRoom.id&&NearDragonGeyserV20(owner.spawnX,p.x)&&NearDragonGeyserV20(owner.x,p.x)&&NearDragonGeyserV20(owner.y,p.y)&&owner.groundLine==p.groundLine&&owner.facing==p.facing&&owner.visualScale==1&&NearDragonGeyserV20(owner.nativeDepth,p.nativeDepth))return true;return false;
  }
  static Vector3 DragonGeyserLogicalEulerV20(Enemy owner){return new Vector3(0,owner.facing<0?180:0,0);}
  bool DragonGeyserAttackEventV20(DragonSourceEventV20 e){return e!=null&&e.type=="source-geyser-attack"&&e.npc=="NUI_STEAM_GEYSER"&&e.worldSerial==game.NpcSoundWorldSerial&&e.room==game.room&&e.actorSerial>0&&DragonEffectParticleFiniteV20(e.time)&&e.time<=game.elapsed+.0001f&&DragonGeyserOriginalOwnerV20(e.owner);}
  partial void ResetDragonNestGeyserParticleCursorsV20(bool discardPast){
   dragonGeyserCursorsV20.Clear();dragonGeyserPruneV20.Clear();DragonNestGeyserEmissionsV20.Clear();dragonGeyserLastSequenceV20=0;
   if(!discardPast||dragonGeyserDocumentV20==null)return;
   foreach(var e in game.dragonSourceEventsV20){dragonGeyserLastSequenceV20=Mathf.Max(dragonGeyserLastSequenceV20,e.sequence);if(!DragonGeyserAttackEventV20(e)||e.owner.npcState!=dragonGeyserDocumentV20.state)continue;var c=new DragonGeyserCursorV20{owner=e.owner,actorSerial=e.actorSerial,attackSequence=e.sequence,began=e.time};float age=game.elapsed-c.began;for(int i=0;i<3;i++)if(dragonGeyserDocumentV20.calls[i].at<=age+.00001f)c.mask|=1<<i;dragonGeyserCursorsV20[e.owner]=c;}
  }
  partial void DisposeDragonNestGeyserSourceParticlesV20(){
   foreach(var view in dragonGeyserViewsV20.Values)DisposeDragonNestGeyserSourceParticleViewV20(view);dragonGeyserViewsV20.Clear();dragonGeyserDocumentV20=null;dragonGeyserSourceSpawnsV20=null;dragonGeyserCursorsV20.Clear();dragonGeyserPruneV20.Clear();DragonNestGeyserEmissionsV20.Clear();dragonGeyserLastSequenceV20=0;
  }
  partial void CollectDragonNestGeyserSourceParticlesV20(){
   var doc=dragonGeyserDocumentV20;if(doc==null||!game.DragonNestSourceEnabledV20)return;
   foreach(var e in game.dragonSourceEventsV20){if(e==null||e.sequence<=dragonGeyserLastSequenceV20||e.time>game.elapsed+.0001f)continue;dragonGeyserLastSequenceV20=e.sequence;if(e.type!="source-geyser-attack")continue;if(!DragonGeyserAttackEventV20(e)){DragonNestGeyserRejectedEventsV20++;continue;}dragonGeyserCursorsV20[e.owner]=new DragonGeyserCursorV20{owner=e.owner,actorSerial=e.actorSerial,attackSequence=e.sequence,began=e.time};DragonNestGeyserAttackEventsV20++;}
   dragonGeyserPruneV20.Clear();foreach(var pair in dragonGeyserCursorsV20){var c=pair.Value;var owner=c.owner;float age=game.elapsed-c.began;if(!DragonGeyserOriginalOwnerV20(owner)||age>5.31f||owner.npcState!=doc.state&&age<doc.sourceAttackDuration-.001f){dragonGeyserPruneV20.Add(pair.Key);continue;}
    float stateTime=owner.npcState==doc.state?owner.npcTime:age;
    for(int i=0;i<3;i++){var call=doc.calls[i];if((c.mask&(1<<i))!=0||stateTime+.00001f<call.at)continue;float at=c.began+call.at;Vector3 position=new Vector3(owner.x+owner.facing*doc.boneOffset[0],owner.y+doc.boneOffset[1],doc.boneOffset[2]),euler=DragonGeyserLogicalEulerV20(owner);int seed=unchecked(c.attackSequence*997^c.actorSerial*193^i*179);
     if(!EnqueueDragonNestGeyserSourceParticleV20(dragonGeyserViewsV20[call.key],at,position,owner.nativeDepth,owner.facing,doc.unit,seed,c.actorSerial,i,call.sourceSetAddRotate,call.sourceSetAxisAngle,euler))continue;c.mask|=1<<i;DragonNestGeyserCreatedSequencesV20++;if(i==2)DragonNestGeyserFireSequencesV20++;else DragonNestGeyserWarningSequencesV20++;
     if(DragonNestGeyserEmissionsV20.Count>=2048)DragonNestGeyserEmissionsV20.RemoveAt(0);DragonNestGeyserEmissionsV20.Add(new DragonNestGeyserEmissionV20{sequence=++dragonGeyserEmissionSequenceV20,actorSerial=c.actorSerial,sourceAttackSequence=c.attackSequence,callIndex=i,time=at,x=position.x,y=position.y,z=position.z,depth=owner.nativeDepth,key=call.key,sourceSetAddRotate=call.sourceSetAddRotate,sourceSetAxisAngle=call.sourceSetAxisAngle,logicalSourceEuler=euler,owner=owner});
    }
   }foreach(var owner in dragonGeyserPruneV20)dragonGeyserCursorsV20.Remove(owner);dragonGeyserPruneV20.Clear();
  }
 }
}
