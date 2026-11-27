using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class NightSourceHeldMeshV19 {
  public string npc,state,sourceCallback,sourceInstructionsSha256,mesh,clock,origin,sourceStateEndCallback,sourceStateEndInstructionsSha256;
  public float at,instanceLifetime;public int slot,sourceCreateLayerArgument;
  public bool followsOwnerPositionAndRotation,clearOwnerSlotOnStateEnd,nativeInstanceDestroyOnSlotClearProven;
 }
 [Serializable] public sealed class NightSourceEmissionV19 {
  public float at;public string kind,template,bone,ownerState;public float[] offset;public bool follow;
 }
 [Serializable] public sealed class NightSourceFireStateV19 {
  public string npc,state;public float duration,unit;public NpcAttachmentTrack[] tracks;public NightSourceEmissionV19[] emissions;
 }
 [Serializable] public sealed class NightSourceVisualDocumentV19 {
  public string schema,sourceRulesSha256,sourceEffectsSha256;public NightSourceHeldMeshV19[] stateMeshes;public NightSourceFireStateV19[] stateVisuals;
 }
 public sealed partial class DemoScene {
  const string NightSourceVisualSchemaV19="besma-lake-night-source-visual-v19";
  NightSourceVisualDocumentV19 nightSourceVisualV19;
  sealed class NightFireOwnerV19 {internal string state;internal float previous=-1;internal bool live;internal int mask;internal readonly List<LakeParticleSequenceV18> sequences=new List<LakeParticleSequenceV18>();}
  sealed class NightHeldInstanceV19 {internal Enemy owner;internal float started;internal Vector3 origin;internal float depth;internal int facing;internal bool attached;}
  sealed class NightHeldOwnerV19 {internal string state;internal float previous=-1;internal bool live,created;internal NightHeldInstanceV19 slot;}
  readonly Dictionary<Enemy,NightFireOwnerV19> nightFireOwnersV19=new Dictionary<Enemy,NightFireOwnerV19>();
  readonly Dictionary<Enemy,NightHeldOwnerV19> nightHeldOwnersV19=new Dictionary<Enemy,NightHeldOwnerV19>();
  readonly List<NightHeldInstanceV19> nightHeldInstancesV19=new List<NightHeldInstanceV19>();
  Transform nightSourceWorldV19;int nightSourceRoomV19=-1,nightSourceGenerationV19=-1;float nightSourcePreviousTimeV19=-1;
  public int NightSourceHeldMeshDrawFramesV19 {get;private set;}
  public int NightSourceFireSequenceCountV19 {get;private set;}
  public int NightSourceHeldLiveInstancesV19 {get{return nightHeldInstancesV19.Count;}}
  public int NightSourceHeldOwnerSlotsV19 {get{int count=0;foreach(var c in nightHeldOwnersV19.Values)if(c.slot!=null)count++;return count;}}
  static bool NightVisualNearV19(float a,float b){return SteamFinite(a)&&Mathf.Abs(a-b)<.00001f;}
  void ConfigureLakeNightNpcVisualV19(string majorJson,OutskirtsNpcMeshManifest meshes,IDictionary<string,Texture2D> majorTextures,LakeMinorVisualDocumentV18 minor,IDictionary<string,Texture2D> minorTextures){
   var major=JsonUtility.FromJson<OutskirtsNpcVisualDocument>(majorJson);var scoped=JsonUtility.FromJson<NightSourceVisualDocumentV19>(majorJson);
   RequireOutskirtsVisual(major!=null&&scoped!=null&&major.schema==NightSourceVisualSchemaV19&&scoped.schema==NightSourceVisualSchemaV19&&EvidenceHash(scoped.sourceRulesSha256)&&EvidenceHash(scoped.sourceEffectsSha256)&&major.particles!=null&&major.particles.Length==21&&major.effectParticles!=null&&major.effectParticles.Length==40&&major.effectMeshes!=null&&major.effectMeshes.Length==1&&major.effectMeshes[0].effect=="DamageEffect_LIZARDMAN_HIGH_DARK_NEW_SPECIAL_ATTACK_A"&&major.effectMeshes[0].mesh=="dark-high-boomerang"&&meshes!=null&&meshes.components!=null&&meshes.components.Length==2&&majorTextures!=null&&majorTextures.Count==12,"Night original Major/mesh scope changed");
   RequireOutskirtsVisual(scoped.stateMeshes!=null&&scoped.stateMeshes.Length==1&&scoped.stateVisuals!=null&&scoped.stateVisuals.Length==1,"Night source state visual scope changed");
   var held=scoped.stateMeshes[0];
   RequireOutskirtsVisual(held!=null&&held.npc=="NUI_LIZARDMAN_HIGH_DARK_NEW"&&held.state=="LIZARDMAN_HIGH_DARK_NEW_ATTACK_B"&&held.mesh=="dark-high-combo-wind"&&held.sourceCallback=="LIZARDMAN_HIGH_DARK_NEW_ATTACK_B_FRAME_MOVE"&&held.sourceInstructionsSha256=="d2a79d14bc07a1c2bcd1107b977405abbd129d36d3c00b6b5f9918dc88568508"&&NightVisualNearV19(held.at,.9f)&&held.clock=="source-animation-seconds"&&held.origin=="owner-current-position"&&held.slot==0&&held.sourceCreateLayerArgument==14&&held.followsOwnerPositionAndRotation&&held.clearOwnerSlotOnStateEnd&&!held.nativeInstanceDestroyOnSlotClearProven&&held.sourceStateEndCallback=="LIZARDMAN_HIGH_DARK_NEW_ATTACK_B_STATE_END"&&held.sourceStateEndInstructionsSha256=="3c242791f160fd41406226de5ee95d043ca6cd67677a002368f987d9a0f4cfbd","Night held-slot source timing/ownership changed");
   var mesh=Array.Find(meshes.components,m=>m.id==held.mesh);RequireOutskirtsVisual(mesh!=null&&NightVisualNearV19(mesh.durationSeconds,held.instanceLifetime),"Night held instance original lifetime changed");
   var fire=scoped.stateVisuals[0];RequireOutskirtsVisual(fire!=null&&fire.npc=="NUI_FIRE_KOMODO"&&fire.state=="FIRE_KOMODO_ATTACK_A"&&fire.duration>0&&NightVisualNearV19(fire.unit,.336f)&&fire.tracks!=null&&fire.tracks.Length==1&&fire.tracks[0].bone=="Bip01_Ponytail2"&&fire.tracks[0].samples!=null&&fire.tracks[0].samples.Length>=(int)Math.Ceiling(fire.duration*60-.001f)+1&&fire.emissions!=null&&fire.emissions.Length==3,"Night FireKomodo original bone/clock scope changed");
   string[] names={"FireAttack_Comodo_AttackC01","FireAttack_Comodo_AttackC02","Flare_MagicKnight_Start02"};float[] times={.13333f,0,1.8f};float[][] offsets={new[]{0f,1.68f,0f},new[]{10.08f,5.04f,0f},new[]{10.08f,0f,0f}};
   for(int i=0;i<3;i++){var emission=fire.emissions[i];RequireOutskirtsVisual(emission!=null&&emission.kind=="particle"&&emission.template==names[i]&&NightVisualNearV19(emission.at,times[i])&&emission.bone=="Bip01_Ponytail2"&&emission.ownerState==fire.state&&emission.follow&&emission.offset!=null&&emission.offset.Length==3,"Night exact FireKomodo source emission changed");for(int c=0;c<3;c++)RequireOutskirtsVisual(NightVisualNearV19(emission.offset[c],offsets[i][c]),"Night FireKomodo original offset changed");}
   foreach(var p in fire.tracks[0].samples)RequireOutskirtsVisual(p!=null&&SteamFinite(p.x)&&SteamFinite(p.y)&&SteamFinite(p.z),"Nonfinite Night source bone");
   ConfigureOutskirtsNpcVisual(major,meshes,majorTextures);ConfigureLakeNpcVisual(minor,minorTextures);nightSourceVisualV19=scoped;NightSourceHeldMeshDrawFramesV19=NightSourceFireSequenceCountV19=0;ClearLakeNightSourceActorsV19();
  }
  void ClearLakeNightSourceActorsV19(){nightFireOwnersV19.Clear();nightHeldOwnersV19.Clear();nightHeldInstancesV19.Clear();nightSourceWorldV19=null;nightSourceRoomV19=nightSourceGenerationV19=-1;nightSourcePreviousTimeV19=-1;}
  void DisposeLakeNightNpcVisualV19(){ClearLakeNightSourceActorsV19();nightSourceVisualV19=null;}
  void EnsureLakeNightVisualWorldV19(){
   if(nightSourceWorldV19!=worldRoot||nightSourceRoomV19!=game.room||nightSourceGenerationV19!=game.NpcLakeVisualGenerationV18||game.elapsed+.0001f<nightSourcePreviousTimeV19){ClearLakeNightSourceActorsV19();nightSourceWorldV19=worldRoot;nightSourceRoomV19=game.room;nightSourceGenerationV19=game.NpcLakeVisualGenerationV18;}nightSourcePreviousTimeV19=game.elapsed;
  }
  static bool NightSourceOwnerLiveV19(Enemy owner){return owner.active&&!owner.dormant&&owner.hp>0&&owner.state==CombatState.Normal;}
  static Vector3 NightFireAttachmentV19(NightSourceFireStateV19 state,NightSourceEmissionV19 emission,Enemy owner){
   var points=state.tracks[0].samples;float at=Mathf.Clamp(owner.npcTime*60,0,points.Length-1);int a=(int)at,b=Mathf.Min(a+1,points.Length-1);float blend=at-a;
   return new Vector3(owner.x+owner.facing*(Mathf.Lerp(points[a].x,points[b].x,blend)+emission.offset[0])*owner.visualScale,owner.y+(Mathf.Lerp(points[a].y,points[b].y,blend)+emission.offset[1])*owner.visualScale,(Mathf.Lerp(points[a].z,points[b].z,blend)+emission.offset[2])*owner.visualScale);
  }
  void CollectLakeNightSourceParticlesV19(){
   if(nightSourceVisualV19==null)return;EnsureLakeNightVisualWorldV19();var state=nightSourceVisualV19.stateVisuals[0];
   foreach(var owner in game.enemies){NightFireOwnerV19 cursor;bool known=nightFireOwnersV19.TryGetValue(owner,out cursor);bool live=NightSourceOwnerLiveV19(owner)&&owner.sourceNPC==state.npc&&owner.npcState==state.state;
    bool fresh=!known||cursor.state!=owner.npcState||owner.npcTime+.0001f<cursor.previous||live!=cursor.live;
    if(fresh){if(known)foreach(var sequence in cursor.sequences)StopLakeSequenceV18(sequence,true);cursor=new NightFireOwnerV19{state=owner.npcState,live=live};nightFireOwnersV19[owner]=cursor;}
    cursor.previous=owner.npcTime;if(!live)continue;
    for(int i=0;i<state.emissions.Length;i++){var emission=state.emissions[i];var position=NightFireAttachmentV19(state,emission,owner);
     if((cursor.mask&(1<<i))==0&&owner.npcTime+.00001f>=emission.at){cursor.mask|=1<<i;float started=game.elapsed-owner.npcTime+emission.at;var sequence=AddLakeSequenceV18(emission.template,started,new Vector3(position.x,position.y,0),owner.nativeDepth+position.z/.28f,owner.facing,state.unit*owner.visualScale,owner.GetHashCode()*179+i*997);sequence.follows=true;sequence.rotateWithOwner=true;cursor.sequences.Add(sequence);NightSourceFireSequenceCountV19++;}
    }
    // TRACE follows the exact sampled owner bone. The yaw mirror is the same
    // explicit2D traced-particle adapter used by the existing source renderer.
    for(int i=0;i<cursor.sequences.Count;i++){var sequence=cursor.sequences[i];var emission=Array.Find(state.emissions,e=>e.template==sequence.view.data.id);var position=NightFireAttachmentV19(state,emission,owner);sequence.origin=new Vector3(position.x,position.y,0);sequence.depth=owner.nativeDepth+position.z/.28f;sequence.facing=owner.facing;}
   }
  }
  void RenderLakeNightHeldMeshesV19(ref int used){
   if(nightSourceVisualV19==null)return;EnsureLakeNightVisualWorldV19();var rule=nightSourceVisualV19.stateMeshes[0];
   foreach(var owner in game.enemies){NightHeldOwnerV19 cursor;bool known=nightHeldOwnersV19.TryGetValue(owner,out cursor);bool live=NightSourceOwnerLiveV19(owner)&&owner.sourceNPC==rule.npc&&owner.npcState==rule.state;
    bool fresh=!known||cursor.state!=owner.npcState||owner.npcTime+.0001f<cursor.previous||live!=cursor.live;
    if(fresh){if(known&&cursor.slot!=null){cursor.slot.attached=false;cursor.slot=null;}cursor=new NightHeldOwnerV19{state=owner.npcState,live=live};nightHeldOwnersV19[owner]=cursor;}
    cursor.previous=owner.npcTime;if(!live)continue;
    if(!cursor.created&&owner.npcTime+.00001f>=rule.at){cursor.created=true;cursor.slot=new NightHeldInstanceV19{owner=owner,started=game.elapsed-owner.npcTime+rule.at,attached=true,origin=new Vector3(owner.x,owner.y,0),depth=owner.nativeDepth,facing=owner.facing};nightHeldInstancesV19.Add(cursor.slot);}
    if(cursor.slot!=null&&game.elapsed-cursor.slot.started>=rule.instanceLifetime){cursor.slot.attached=false;cursor.slot=null;}
   }
   for(int i=nightHeldInstancesV19.Count-1;i>=0;i--){var instance=nightHeldInstancesV19[i];float age=game.elapsed-instance.started;if(age>=rule.instanceLifetime){nightHeldInstancesV19.RemoveAt(i);continue;}if(instance.attached){instance.origin=new Vector3(instance.owner.x,instance.owner.y,0);instance.depth=instance.owner.nativeDepth;instance.facing=instance.owner.facing;}
    int before=used;DrawOutskirtsMesh(ref used,rule.mesh,age,instance.origin,instance.facing,.28f,instance.depth);if(used>before){NightSourceHeldMeshDrawFramesV19++;SourceNpcMeshDrawFrames++;}
   }
  }
 }
}
