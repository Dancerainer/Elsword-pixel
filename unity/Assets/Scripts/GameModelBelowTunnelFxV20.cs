using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace ElswordPixel {
 [Serializable] public sealed class BelowFxRowV20 {
  public string id,npc,state,callback,effect,sourceBone,sourceInstructionsSha256;
  public float sourceAnimationTime,candidateAt,sourceAnimSpeed,sourceStateDuration,sourceOffsetNativeY,sourceRotationZ;
  public float[] sourceRotationAddXYZ,offsetXYZ;public bool requiresBasisProvider,sourceLandYArgument,sourceNativeClockExecuted,native3DParity;public int sourceCreateSitePC;
 }
 [Serializable] public sealed class BelowFxProjectionV20 {public float rotationZ,scaleX=1,scaleY=1,scaleZ=1;public int facingMultiplier=1;}
 [Serializable] public sealed class BelowFxSourceDocumentV20 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,clockAdapter,coordinateAdapter;
  public bool runtimeReady,runtimeAdmitted,nativeExecuted;public BelowFxRowV20[] rows;
 }
 [Serializable] public sealed class BelowFxSpawnEventV20 {
  public string type,npc,source,effect;public int room,ownerSerial,targetSerial,facing;
  public float at,amount,x,y,depth,baseX,baseY,baseZ,sourceLandY;public bool hasSourceLandY;
 }
 public sealed partial class GameModel {
  BelowFxSourceDocumentV20 belowFxV20;
  public Func<Enemy,BelowFxRowV20,BelowFxProjectionV20> BelowFxBasisProjectionProviderV20;
  public string BelowFxBasisProjectionPolicyV20;
  readonly Dictionary<Enemy,HashSet<string>> belowFxIssuedV20=new Dictionary<Enemy,HashSet<string>>();
  public readonly List<BelowFxSpawnEventV20> belowFxEventsV20=new List<BelowFxSpawnEventV20>();
  const string BelowFxSemanticReceiptV20="33efcef27f0198d5ef795c25a90e4390ef8b4d6ba9b6b034bd6da64162fd06c6";
  static string BelowFxReceiptV20(BelowFxSourceDocumentV20 d){var s=new List<string>{d.schema,d.sourceRulesSha256,d.sourceEffectsSha256,d.clockAdapter,d.coordinateAdapter};foreach(var r in d.rows)s.AddRange(new[]{r.id,r.npc,r.state,r.callback,r.effect,r.sourceBone,r.sourceInstructionsSha256,FortBitsV20(r.sourceAnimationTime),FortBitsV20(r.candidateAt),FortBitsV20(r.sourceAnimSpeed),FortBitsV20(r.sourceStateDuration),FortBitsV20(r.sourceOffsetNativeY),FortBitsV20(r.sourceRotationZ),FortBitsV20(r.offsetXYZ[0]),FortBitsV20(r.offsetXYZ[1]),FortBitsV20(r.offsetXYZ[2]),FortBitsV20(r.sourceRotationAddXYZ[0]),FortBitsV20(r.sourceRotationAddXYZ[1]),FortBitsV20(r.sourceRotationAddXYZ[2]),FortBoolV20(r.requiresBasisProvider),FortBoolV20(r.sourceLandYArgument),FortBoolV20(r.sourceNativeClockExecuted),FortBoolV20(r.native3DParity),r.sourceCreateSitePC.ToString()});using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",s.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();}
  public void SetNpcBelowFxV20(BelowFxSourceDocumentV20 doc){
   if(BelowDefaultV20(doc))doc=null;if(doc!=null){
    if(doc.schema!="altera-below-tunnel-callback-fx-source-draft-v20-r2"||doc.runtimeReady||doc.runtimeAdmitted||doc.nativeExecuted||doc.sourceRulesSha256!="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb"||doc.sourceEffectsSha256!="89e0d3e4a89d5850fd546948e2b2cf08ee2350d7503044684cbd7060c1f1fa42"||doc.rows==null||doc.rows.Length!=12)throw new ArgumentException("Invalid exact Below Tunnel callback FX draft");
    var ids=new HashSet<string>();foreach(var r in doc.rows){NpcStateDefinition state;if(r==null||string.IsNullOrEmpty(r.id)||!ids.Add(r.id)||!npcStates.TryGetValue(r.npc+"/"+r.state,out state)||!npcEffectDefinitions.ContainsKey(r.effect)||r.sourceStateDuration!=state.duration||r.sourceAnimSpeed!=state.sourceAnimSpeed||r.candidateAt<0||r.candidateAt>state.duration||Math.Abs(r.candidateAt-r.sourceAnimationTime/state.sourceAnimSpeed)>.000001f||r.sourceRotationAddXYZ==null||r.sourceRotationAddXYZ.Length!=3||r.offsetXYZ==null||r.offsetXYZ.Length!=3||r.sourceNativeClockExecuted||r.native3DParity||!r.sourceLandYArgument)throw new ArgumentException("Invalid source callback clock/bone/effect binding");foreach(float v in r.offsetXYZ)if(!Finite(v))throw new ArgumentException("Invalid exact source bone point");}
    if(BelowFxReceiptV20(doc)!=BelowFxSemanticReceiptV20)throw new ArgumentException("Changed source callback FX field/IR/bone receipt");
   }belowFxV20=doc;ClearBelowFxRoomV20();
  }
  void ClearBelowFxRoomV20(){belowFxIssuedV20.Clear();belowFxEventsV20.Clear();}
  void OnBelowFxStateEnteredV20(Enemy e){if(belowFxV20!=null)belowFxIssuedV20.Remove(e);OnBelowCandidateStateV21(e);}
  void OnBelowFxStateExitV20(Enemy e){if(belowFxV20!=null)belowFxIssuedV20.Remove(e);OnBelowCandidateStateV21(e);}
  void AdvanceBelowFxV20(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   AdvanceBelowCandidateFxV21(owner,state,from,to,packetStart);if(belowFxV20==null)return;HashSet<string> issued;if(!belowFxIssuedV20.TryGetValue(owner,out issued)){issued=new HashSet<string>();belowFxIssuedV20.Add(owner,issued);}
   foreach(var r in belowFxV20.rows){if(r.npc!=owner.sourceNPC||r.state!=state.id||issued.Contains(r.id)||r.candidateAt<from-1e-6f||r.candidateAt>to+1e-6f)continue;issued.Add(r.id);float at=packetStart+Math.Max(0,r.candidateAt-from);
    BelowFxProjectionV20 projection=null;if(r.requiresBasisProvider){if(BelowFxBasisProjectionProviderV20!=null){if(string.IsNullOrEmpty(BelowFxBasisProjectionPolicyV20))throw new InvalidOperationException("Source XYZ basis projection policy must be explicit");projection=BelowFxBasisProjectionProviderV20(owner,r);}if(projection==null){belowFxEventsV20.Add(new BelowFxSpawnEventV20{type="HOLD-source-XYZ-rotation-basis-provider",npc=owner.sourceNPC,source=r.id,effect=r.effect,room=room,at=at,amount=r.sourceAnimationTime,ownerSerial=NpcSoundActorId(owner)});continue;}if(!Finite(projection.rotationZ)||!Finite(projection.scaleX)||!Finite(projection.scaleY)||!Finite(projection.scaleZ)||projection.scaleX==0||projection.scaleY==0||projection.scaleZ==0||projection.facingMultiplier!=1&&projection.facingMultiplier!=-1)throw new ArgumentException("Invalid explicit source-basis projection");}
    var emission=new NpcEffectEmission{effect=r.effect,offsetX=r.offsetXYZ[0],offsetY=r.offsetXYZ[1],offsetZ=r.offsetXYZ[2]+owner.nativeDepth*.28f,sourceLandPositionYArgument=true,rotationZ=projection==null?r.sourceRotationZ:projection.rotationZ,scaleX=projection==null?1:projection.scaleX,scaleY=projection==null?1:projection.scaleY,scaleZ=projection==null?1:projection.scaleZ,facingMultiplier=projection==null?1:projection.facingMultiplier};
    float bx,by,bz;NpcAttachment(owner,new NpcEffectEmission(),at,out bx,out by,out bz);var fx=SpawnNpcEffect(owner,npcEffectDefinitions[r.effect],emission,at);
    belowFxEventsV20.Add(new BelowFxSpawnEventV20{type="source-callback-CreateInstance",npc=owner.sourceNPC,source=r.id,effect=fx.id,room=room,at=at,amount=r.sourceAnimationTime,x=fx.x,y=fx.y,depth=fx.z,baseX=bx,baseY=by,baseZ=bz,hasSourceLandY=fx.hasSourceLandY,sourceLandY=fx.sourceLandY,facing=fx.facing,ownerSerial=NpcSoundActorId(owner),targetSerial=fx.serial});
   }
  }
 }
}
