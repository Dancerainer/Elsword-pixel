using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class NpcMine2MissileLaneV20 {public string bone,sourcePositionVariable,sourceHandleVariable;public float[] originOffset;public int targetSide;}
 [Serializable] public sealed class NpcMine2MissileWaveV20 {public float at,sourceAnimationTime,targetNativeDistance;public NpcMine2MissileLaneV20[] lanes;}
 [Serializable] public sealed class NpcMine2WallyRuleV20 {public string npc,state,effect,sourceCallback,sourceInstructionsSha256,semanticConstantsAndInstructionsSha256,sourceOriginAdapter,sourceLockTargetAdapter;public float sourceAnimSpeed,sourceLandYOffset;public bool sourceVariableCrossLabelsPreserved;public NpcMine2MissileWaveV20[] waves;}
 [Serializable] public sealed class NpcMine2SourceDocumentV20 {public string schema,sourceRulesSha256,sourceEffectsSha256;public bool runtimeAdmitted,nativeExecuted;public NpcMine2WallyRuleV20 wally;}
 [Serializable] public sealed class NpcMine2SourceEventV20 {public string type,npc,effect,originBone,sourceHandle;public int wave,lane,room;public float time,x,y,z,landY,targetX,targetY;}
 public sealed partial class GameModel {
  NpcMine2SourceDocumentV20 mine2SourceV20;
  sealed class Mine2CursorV20 {internal string state;internal int mask;}
  readonly Dictionary<Enemy,Mine2CursorV20> mine2CursorsV20=new Dictionary<Enemy,Mine2CursorV20>();
  readonly Dictionary<NpcEffectInstance,float[]> mine2FixedTargetsV20=new Dictionary<NpcEffectInstance,float[]>();
  public readonly List<NpcMine2SourceEventV20> mine2SourceEventsV20=new List<NpcMine2SourceEventV20>();
  public void SetNpcMine2SourceV20(NpcMine2SourceDocumentV20 doc){
   if(doc!=null&&string.IsNullOrEmpty(doc.schema)&&string.IsNullOrEmpty(doc.sourceRulesSha256)&&string.IsNullOrEmpty(doc.sourceEffectsSha256)&&!doc.runtimeAdmitted&&!doc.nativeExecuted&&(doc.wally==null||string.IsNullOrEmpty(doc.wally.npc)&&string.IsNullOrEmpty(doc.wally.state)&&string.IsNullOrEmpty(doc.wally.effect)&&string.IsNullOrEmpty(doc.wally.sourceCallback)&&string.IsNullOrEmpty(doc.wally.sourceInstructionsSha256)&&string.IsNullOrEmpty(doc.wally.semanticConstantsAndInstructionsSha256)&&string.IsNullOrEmpty(doc.wally.sourceOriginAdapter)&&string.IsNullOrEmpty(doc.wally.sourceLockTargetAdapter)&&doc.wally.sourceAnimSpeed==0&&doc.wally.sourceLandYOffset==0&&!doc.wally.sourceVariableCrossLabelsPreserved&&(doc.wally.waves==null||doc.wally.waves.Length==0)))doc=null;
   if(doc!=null){var w=doc.wally;NpcStateDefinition state;NpcEffectDefinition effect;
    if(doc.schema!="besma-mine2-source-callback-runtime-draft-v20"||doc.runtimeAdmitted||doc.nativeExecuted||!SourceSupportHashV17(doc.sourceRulesSha256)||!SourceSupportHashV17(doc.sourceEffectsSha256)||w==null||w.npc!="NUI_WALLY_8TH_MK2"||w.state!="WALLY_8TH_MK2_ATTACK_B"||w.effect!="DamageEffect_WALLY_8TH_MK2_ATTACK_B"||w.sourceCallback!="WALLY_8TH_MK2_ATTACK_B_FRAME_MOVE"||w.sourceInstructionsSha256!="efa4752c7e5727148f47325b1cdbe1da493144f1f2d2059b93cc5259dde81685"||w.semanticConstantsAndInstructionsSha256!="acd19b86fa0f1062b9a35b0e1c53bcf8023c042345d6f34a843e6e477de65f85"||!npcStates.TryGetValue(w.npc+"/"+w.state,out state)||!npcEffectDefinitions.TryGetValue(w.effect,out effect)||state.sourceAnimation!="AttackB_Fire"||state.sourceAnimSpeed!=1||w.sourceAnimSpeed!=1||w.sourceLandYOffset!=30||!w.sourceVariableCrossLabelsPreserved||w.waves==null||w.waves.Length!=4)throw new ArgumentException("Invalid exact Mine2 Wally missile source contract");
    if(w.sourceOriginAdapter!="original-current-owner+exact-native-hand-sample-unit28-npc-scale-already-baked"||w.sourceLockTargetAdapter!="captured-original-owner-land-position-plus-facing-side-distance-times28-and-y30-fixed-position-v20")throw new ArgumentException("Unrecorded Mine2 positional steering adapter");
    for(int i=0;i<4;i++){var q=w.waves[i];if(q==null||!Finite(q.at)||Math.Abs(q.at-(.133f+.4f*i))>.00001f||q.sourceAnimationTime!=q.at||q.at>state.duration||q.targetNativeDistance!=250*(1+i)||q.lanes==null||q.lanes.Length!=2)throw new ArgumentException("Changed original4wave Wally threshold or target distance");for(int j=0;j<2;j++){var lane=q.lanes[j];if(lane==null||lane.bone!=(j==0?"Bip01_L_Finger0":"Bip01_R_Finger0")||lane.sourcePositionVariable!=(j==0?"RHandPos":"LHandPos")||lane.sourceHandleVariable!=(j==0?"RightMissile":"LeftMissile")||lane.targetSide!=(j==0?-1:1)||lane.originOffset==null||lane.originOffset.Length!=3||!Finite(lane.originOffset[0])||!Finite(lane.originOffset[1])||!Finite(lane.originOffset[2]))throw new ArgumentException("Original GSET cross-labelled hand/handle mapping changed");}}
   }
   mine2SourceV20=doc;ClearMine2SourceRoomV20();
  }
  void ClearMine2SourceRoomV20(){mine2CursorsV20.Clear();mine2FixedTargetsV20.Clear();mine2SourceEventsV20.Clear();}
  void OnMine2SourceStateExitV20(Enemy owner,NpcStateDefinition state){if(owner!=null)mine2CursorsV20.Remove(owner);}
  void OnMine2SourceEffectEndV20(NpcEffectInstance effect){if(effect!=null)mine2FixedTargetsV20.Remove(effect);}
  bool TryMine2SourceFixedTargetV20(NpcEffectInstance effect,out float x,out float y){float[] p;if(!effect.reflected&&mine2FixedTargetsV20.TryGetValue(effect,out p)){x=p[0];y=p[1];return true;}x=y=0;return false;}
  void AdvanceMine2SourceV20(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   var doc=mine2SourceV20;if(doc==null||owner.sourceNPC!=doc.wally.npc||state.id!=doc.wally.state)return;Mine2CursorV20 cursor;if(!mine2CursorsV20.TryGetValue(owner,out cursor)||cursor.state!=state.id){cursor=new Mine2CursorV20{state=state.id};mine2CursorsV20[owner]=cursor;}
   for(int i=0;i<4;i++){var wave=doc.wally.waves[i];float threshold=wave.at/state.sourceAnimSpeed;if((cursor.mask&(1<<i))!=0||from>=threshold||to+1e-6f<threshold)continue;cursor.mask|=1<<i;float at=packetStart+threshold-from;
    var land=new NpcEffectInstance{owner=owner};CaptureNpcSourceLandArgumentV18(land,null,new NpcEffectEmission{sourceLandPositionYArgument=true});float landY=land.sourceLandY;
    for(int j=0;j<2;j++){var lane=wave.lanes[j];var emission=new NpcEffectEmission{effect=doc.wally.effect,lockOnMode=2,sourceLandPositionYArgument=true,offsetX=lane.originOffset[0],offsetY=lane.originOffset[1],offsetZ=lane.originOffset[2]};var effect=SpawnNpcEffect(owner,npcEffectDefinitions[emission.effect],emission,at);effect.ownerScale=1;effect.hasSourceLandY=true;effect.sourceLandY=landY+doc.wally.sourceLandYOffset*.28f;float targetX=owner.x+owner.facing*lane.targetSide*wave.targetNativeDistance*.28f,targetY=effect.sourceLandY;mine2FixedTargetsV20[effect]=new[]{targetX,targetY};
     mine2SourceEventsV20.Add(new NpcMine2SourceEventV20{type="source-wally-mk2-missile",npc=owner.sourceNPC,effect=effect.id,originBone=lane.bone,sourceHandle=lane.sourceHandleVariable,wave=i,lane=j,room=room,time=at,x=effect.x,y=effect.y,z=effect.z,landY=effect.sourceLandY,targetX=targetX,targetY=targetY});
    }
   }
  }
 }
}
