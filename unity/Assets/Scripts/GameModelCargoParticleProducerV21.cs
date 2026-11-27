using System;
using System.Collections.Generic;
namespace ElswordPixel {
 // Value snapshots of requests committed by the existing source callback clock.
 // This is a 2D source adapter; no native callback or particle ABI execution is claimed.
 [Serializable] public struct CargoCommittedParticleV21 {
  public int sequence,ownerSerial,sourceEffectSerial,sourceLiteralPC,lane,facing;
  public string bindingId,npc,state,sourceCallback,sourceInstructionsSha256,particleKey,bone;
  public float time,sourceLocalTime,x,y,z,depth,landY,unit;
  public bool hasSourceLandY;
 }
 public sealed partial class GameModel {
  readonly List<CargoCommittedParticleV21> cargoCommittedParticlesV21=new List<CargoCommittedParticleV21>();
  int cargoParticleSequenceV21;
  public int CargoCommittedParticleSequenceV21 {get{return cargoParticleSequenceV21;}}
  public CargoCommittedParticleV21[] CopyCargoCommittedParticlesV21(int afterSequence){
   if(afterSequence<0||afterSequence>cargoParticleSequenceV21)throw new ArgumentOutOfRangeException("afterSequence");
   if(cargoCommittedParticlesV21.Count>0&&afterSequence<cargoCommittedParticlesV21[0].sequence-1)throw new InvalidOperationException("Cargo committed particle consumer fell behind retained source packets");
   return cargoCommittedParticlesV21.FindAll(p=>p.sequence>afterSequence).ToArray();
  }
  void ClearCargoCommittedParticlesV21(){cargoCommittedParticlesV21.Clear();cargoParticleSequenceV21=0;}
  // site0: prototype8/PC36; site1: ATTACK_C/PC126;
  // site2/3: SPECIAL_ATTACK_A/PC71 and PC148, in original +700/-700 order.
  void CommitCargoParticleV21(Enemy owner,int site,float at,float x,float y,float z,int effectSerial=0,string bone=null,int lane=-1,bool hasLand=false,float land=0){
   if(!CargoWallyBoundV21(owner)||site<0||site>3||!Finite(at)||at<0||at>elapsed+.0001f||!Finite(x)||!Finite(y)||!Finite(z)||!Finite(owner.nativeDepth)||hasLand&&!Finite(land))throw new InvalidOperationException("Unbound Cargo source particle commit");
   int ownerSerial=enemies.IndexOf(owner)+1;if(ownerSerial<=0)throw new InvalidOperationException("Cargo source particle owner outside current roster");
   bool missile=site<2;string state=missile?"WALLY_9TH_ARMOR_ATTACK_C":"WALLY_9TH_ARMOR_SPECIAL_ATTACK_A";
   if(owner.npcState!=state)throw new InvalidOperationException("Wrong Cargo callback state at particle commit");
   string callback=site==0?"CreateMissileAttackEffect/prototype8":state+"_FRAME_MOVE";
   int pc=site==0?36:site==1?126:site==2?71:148;
   string hash=site==0?cargoSourceV21.visualHelperSha256:missile?cargoSourceV21.missileCallbackSha256:cargoSourceV21.specialCallbackSha256;
   string key=site==0?"Minor/Wally9th_MissileAttack02":site==1?"Minor/LED_Wally9th_Armor_MissileAttack_EyeLed01":"Major/Wally9th_Armor_ChargeBall02";
   var packet=new CargoCommittedParticleV21{sequence=++cargoParticleSequenceV21,ownerSerial=ownerSerial,sourceEffectSerial=effectSerial,sourceLiteralPC=pc,lane=lane,facing=owner.facing,bindingId=owner.sourceNPC+"/"+state+"/"+callback+"/"+pc,npc=owner.sourceNPC,state=state,sourceCallback=callback,sourceInstructionsSha256=hash,particleKey=key,bone=bone,time=at,sourceLocalTime=missile?cargoSourceV21.missileAt:cargoSourceV21.specialAt,x=x,y=y,z=z,depth=owner.nativeDepth,hasSourceLandY=hasLand,landY=land,unit=cargoSourceV21.sourceUnitScale};
   if(cargoCommittedParticlesV21.Count==4096)cargoCommittedParticlesV21.RemoveAt(0);cargoCommittedParticlesV21.Add(packet);
  }
 }
}
