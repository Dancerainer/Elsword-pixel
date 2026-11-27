// Explicit test candidate adapters. These policies are not native ABI claims.
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace ElswordPixel {
 [Serializable] public sealed class BelowCandidateFxRowV21 {public string npc,state,callback,effect,bone,sourceInstructionsSha256,timerKind;public float at,sourceAt;public float[] offsetXYZ;public bool eye;}
 [Serializable] public sealed class BelowTunnelCandidateDocumentV21 {public string schema,policy;public bool enabled;public BelowCandidateFxRowV21[] remainingFx;}
 public sealed partial class GameModel {
  BelowTunnelCandidateDocumentV21 belowCandidateV21;
  readonly Dictionary<Enemy,HashSet<string>> belowCandidateIssuedV21=new Dictionary<Enemy,HashSet<string>>();
  public readonly List<BelowTunnelSourceEventV20> belowCandidateEventsV21=new List<BelowTunnelSourceEventV20>();
  const string BelowCandidatePolicyV21="pixel-route-frame-v21: local-or-explicit-placeholderHP; dynamic-nonclear; owner-facing; logical-MajorParticle; source-halfturn-sign-projection; cannon-AnimTimer-single-crossing; eye-canonical-route-dir-and-depth; player-target0; steering6-rad-per-sec-until-lifetime4; downward-FORCE_FLY";
  const string BelowCandidateReceiptV21="de645bbf02153f04a954adea1e88757d7c2212e24ee24a4012652d84c1e4bf38";
  static string CandidateReceiptV21(BelowTunnelCandidateDocumentV21 d){var a=new List<string>{d.schema,d.policy};foreach(var r in d.remainingFx)a.AddRange(new[]{r.npc,r.state,r.callback,r.effect,r.bone,r.sourceInstructionsSha256,r.timerKind,FortBitsV20(r.at),FortBitsV20(r.sourceAt),FortBitsV20(r.offsetXYZ[0]),FortBitsV20(r.offsetXYZ[1]),FortBitsV20(r.offsetXYZ[2]),FortBoolV20(r.eye)});using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",a.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();}
  public bool BelowTunnelCandidateEnabledV21 {get{return belowCandidateV21!=null;}}
  public void SetBelowTunnelCandidateV21(BelowTunnelCandidateDocumentV21 d){
   if(d!=null&&string.IsNullOrEmpty(d.schema)&&string.IsNullOrEmpty(d.policy)&&!d.enabled&&(d.remainingFx==null||d.remainingFx.Length==0))d=null;
   bool old=belowCandidateV21!=null;
   if(d!=null){if(d.schema!="below-tunnel-explicit-component-candidate-v21"||d.policy!=BelowCandidatePolicyV21||!d.enabled||belowTunnelV20==null||belowFortV20==null||belowFxV20==null||belowBombV20==null||!belowMiniUpWaitEnabledV20||d.remainingFx==null||d.remainingFx.Length!=4)throw new ArgumentException("Incomplete Below Tunnel v21 candidate");
    var ids=new HashSet<string>();foreach(var r in d.remainingFx){NpcStateDefinition s;if(r==null||!ids.Add(r.npc)||!npcStates.TryGetValue(r.npc+"/"+r.state,out s)||!npcEffectDefinitions.ContainsKey(r.effect)||r.offsetXYZ==null||r.offsetXYZ.Length!=3||r.at!=r.sourceAt/s.sourceAnimSpeed||r.at<0||r.at>s.duration||string.IsNullOrEmpty(r.sourceInstructionsSha256)||r.timerKind!=(r.eye?"AnimEventTimer_LUA":"AnimTimer_LUA-single-crossing-candidate"))throw new ArgumentException("Changed remaining callback candidate binding");foreach(float v in r.offsetXYZ)if(!Finite(v))throw new ArgumentException("Nonfinite source bone");}
    if(CandidateReceiptV21(d)!=BelowCandidateReceiptV21)throw new ArgumentException("Changed candidate source bone/clock/identity/IR receipt");
   }
   belowCandidateV21=d;ClearBelowCandidateRoomV21();PrepareBelowSteamController();
   if(d==null){if(old){BelowFortCreateCandidateAdapterV20=null;BelowFortCreateAdapterDescriptionV20=null;BelowFxBasisProjectionProviderV20=null;BelowFxBasisProjectionPolicyV20=null;BelowBombLogicalMajorParticleServiceV20=false;AlteraSteamMajorParticleServiceAvailableV18=false;AlteraSteamDamageConsumerV18=null;SetAlteraSteamControllerRulesV18(null);}return;}
   BelowFortCreateAdapterDescriptionV20=BelowCandidatePolicyV21;
   BelowFortCreateCandidateAdapterV20=BelowCandidateFortSpawnV21;
   BelowFxBasisProjectionPolicyV20=BelowCandidatePolicyV21;
   BelowFxBasisProjectionProviderV20=(owner,row)=>new BelowFxProjectionV20{rotationZ=row.sourceRotationAddXYZ[2],scaleX=row.sourceRotationAddXYZ[1]==180?-1:1,scaleY=row.sourceRotationAddXYZ[0]==180?-1:1,scaleZ=(row.sourceRotationAddXYZ[0]==180?-1:1)*(row.sourceRotationAddXYZ[1]==180?-1:1)};
   BelowBombLogicalMajorParticleServiceV20=true;
   AlteraSteamMajorParticleServiceAvailableV18=true;AlteraSteamDamageConsumerV18=BelowCandidateSteamDamageV21;
   var rules=new List<AlteraSteamControllerRuleV18>();foreach(bool down in new[]{true,false}){string stem=down?"STEAM_TRAP_DOWN":"STEAM_TRAP_SIDE";rules.Add(new AlteraSteamControllerRuleV18{npc="NUI_"+stem,environmentKind=down?"source-steam-controller-down":"source-steam-controller-side",controllerNpc="NUI_STEAM_CONTROLER",waitState=stem+"_WAIT",attackState=stem+"_ATTACK",sourceAttackCallback=stem+"_ATTACK_FRAME_MOVE",sourceWaitStartCallback=stem+"_WAIT_STATE_START",sourceParticle=down?"SteamAttackDown":"SteamAttackSide",sourceParticleBone="ATTACK_LINE_START1",flagIndex=0,controllerShutdownHpFraction=.3f,sourceParticleOffsetY=down?-55:0,sourceBackSpeedY=-1000,sourceForceFly=true,stopSound="npc-steamtrap_stop",fireSound=down?"npc-steamtrap_fire":null});}SetAlteraSteamControllerRulesV18(rules.ToArray());
  }
  void ClearBelowCandidateRoomV21(){belowCandidateIssuedV21.Clear();belowCandidateEventsV21.Clear();}
  void PrepareBelowSteamController(){
   if(belowCandidateV21==null)return;
   // The fixed controller owns DAMAGE and DYING motions. Its remaining HP
   // switches both nozzles off at 30%, independently of the room kill count.
   foreach(var e in enemies)if(e.sourceNPC=="NUI_STEAM_CONTROLER")e.passive=false;
  }
  void BelowCandidateEventV21(string type,Enemy owner,string source,float at,float amount=0){if(belowCandidateEventsV21.Count>=4096)belowCandidateEventsV21.RemoveAt(0);belowCandidateEventsV21.Add(new BelowTunnelSourceEventV20{type=type,npc=owner.sourceNPC,source=source,at=at,amount=amount,room=room,ownerSerial=NpcSoundActorId(owner),x=owner.x,y=owner.y,depth=owner.nativeDepth});}
  EnemySpawn BelowCandidateFortSpawnV21(BelowFortCreateRequestV20 q){
   EnemySpawn source=null;foreach(var r in rooms)foreach(var e in r.enemies)if(e.sourceNPC==q.npc){source=e;break;}
   // Original local HP is not present on every Fort part. The retained static
   // model placeholder is explicitly carried into dynamic births in that case.
   float hp=source==null?1000:source.hp;
   BelowCandidateEventV21("candidate-local-or-placeholder-create-stats",q.owner,q.sourceCallback,q.due,hp);
   return new EnemySpawn{sourceNPC=q.npc,hp=hp,visualScale=source==null?1:source.visualScale,facing=q.owner.facing,countsForClear=false,aiEnabled=true};
  }
  bool TickBelowCandidateSteamV21(Enemy owner,NpcDefinition profile,float dt){return belowCandidateV21!=null&&IsBelowTunnelZeroBodySteamV20(profile)&&TickAlteraSteamEnvironmentV18(owner,profile,dt);}
  int BelowCandidateSteamDamageV21(AlteraSteamDamageRequestV18 r){
   if(belowCandidateV21==null||r==null||!r.sourceForceFly||r.sourceBackSpeedY!=-1000)throw new InvalidOperationException("Changed Steam FORCE_FLY source request");
   bool protectedBefore=PlayerHasSuperArmor||player.superArmor>0;
   int dealt=ApplyEnemyAttackToPlayer(r.damage,r.owner.x-r.owner.facing,HitReaction.Launch,-280);
   if(dealt>0&&player.hp>0&&!protectedBefore){player.vy=r.sourceBackSpeedY*.28f;player.vx=0;player.knockdownOnLanding=true;BelowCandidateEventV21("candidate-source-downward-force-fly",r.owner,r.sourceState,r.time,player.vy);}return dealt;
  }
  void OnBelowCandidateStateV21(Enemy owner){if(belowCandidateV21!=null)belowCandidateIssuedV21.Remove(owner);}
  void AdvanceBelowCandidateFxV21(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(belowCandidateV21==null)return;HashSet<string> issued;if(!belowCandidateIssuedV21.TryGetValue(owner,out issued)){issued=new HashSet<string>();belowCandidateIssuedV21.Add(owner,issued);}
   foreach(var r in belowCandidateV21.remainingFx){if(r.npc!=owner.sourceNPC||r.state!=state.id||issued.Contains(r.callback)||r.at<from-1e-6f||r.at>to+1e-6f)continue;issued.Add(r.callback);float at=packetStart+Math.Max(0,r.at-from);
    var emission=new NpcEffectEmission{effect=r.effect,offsetX=r.offsetXYZ[0]+(r.eye?28:0),offsetY=r.offsetXYZ[1],offsetZ=r.offsetXYZ[2]+owner.nativeDepth*.28f+(r.eye?14:0),sourceLandPositionYArgument=true};
    // Emission mirrors offsetX once; the canonical route direction is +X,
    // with the source GetIsRight sign represented by owner-facing.
    var effect=SpawnNpcEffect(owner,npcEffectDefinitions[r.effect],emission,at);
    BelowCandidateEventV21(r.eye?"candidate-eye-source-bone-and-canonical-basis":"candidate-cannon-AnimTimer-single-crossing-target0-player",owner,r.callback,at,effect.serial);
   }
  }
 }
}
