using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraRavenMaximumRuleV18 {
  public string npc,readyState,attackState,sourceCallback,sourceBone,runtimeBone,sound;
  public bool engineeringCombat;public float cooldown,sourceFirstAt,sourceSecondAt,sourceMoveTrue,sourceMoveFalse,sourceSecondOffsetY;
 }
 public sealed class AlteraRavenSourcePoseV18 {public bool sourceIsRight;public float logicalDirXPerNative,logicalDirYPerNative,logicalDirZPerNative;}
 public sealed class AlteraRavenMaximumReleaseV18 {
  public string kind,effect,npc,state,sourceBone;public int sequence;public float time,sourceTime,boneX,boneY,boneZ,sourceLandY,x,y,z,sourceMoveDistance,sourceOffsetY,sourceCameraMagnitude,sourceCameraDuration;
  public bool hasResolvedDirection,sourceEffectConsumerAvailable;public Enemy owner;
 }
 public sealed partial class GameModel {
  AlteraRavenMaximumRuleV18 alteraRavenMaximumRuleV18;
  readonly Dictionary<Enemy,int> alteraRavenMaximumMaskV18=new Dictionary<Enemy,int>();
  readonly Dictionary<Enemy,float> alteraRavenMaximumPreviousV18=new Dictionary<Enemy,float>();
  public readonly List<AlteraRavenMaximumReleaseV18> alteraRavenMaximumReleasesV18=new List<AlteraRavenMaximumReleaseV18>();
  int alteraRavenMaximumSequenceV18;
  // Provider must explicitly project the real GetDirVector/GetIsRight source
  // operands. Null records an unresolved request; it never guesses a muzzle.
  public Func<Enemy,AlteraRavenSourcePoseV18> AlteraRavenMaximumSourcePoseV18;
  public void SetAlteraRavenMaximumReleaseV18(AlteraRavenMaximumRuleV18 rule){
   if(rule!=null&&!rule.engineeringCombat&&string.IsNullOrEmpty(rule.npc)&&string.IsNullOrEmpty(rule.readyState)&&string.IsNullOrEmpty(rule.attackState)&&string.IsNullOrEmpty(rule.sourceCallback)&&string.IsNullOrEmpty(rule.sourceBone)&&string.IsNullOrEmpty(rule.runtimeBone)&&string.IsNullOrEmpty(rule.sound)&&rule.cooldown==0&&rule.sourceFirstAt==0&&rule.sourceSecondAt==0&&rule.sourceMoveTrue==0&&rule.sourceMoveFalse==0&&rule.sourceSecondOffsetY==0)rule=null;
   if(rule!=null){NpcDefinition profile;NpcMechanismStateRule ready;NpcStateDefinition attack;
    if(rule.npc!="NUI_RAVEN"||rule.readyState!="RAVEN_SP_MAXIMUM_CANNON_READY"||rule.attackState!="RAVEN_SP_MAXIMUM_CANNON"||rule.sourceCallback!="RAVEN_SP_MAXIMUM_CANNON_FRAME_MOVE"||rule.sourceBone!="Dummy2_Lhand"||rule.runtimeBone!="Dummy2_LHand"||rule.sound!="npc-raven_sp_maximum_cannon"||rule.cooldown!=8||rule.sourceFirstAt!=.01f||rule.sourceSecondAt!=1.3f||rule.sourceMoveTrue!=75||rule.sourceMoveFalse!=-75||rule.sourceSecondOffsetY!=15||
     !npcDefinitions.TryGetValue(rule.npc,out profile)||!npcMechanismRules.TryGetValue(rule.npc+"/"+rule.readyState,out ready)||!ready.cooldownExplicit||ready.cooldown!=8||!npcStates.TryGetValue(rule.npc+"/"+rule.attackState,out attack)||attack.sourceAnimation!="SpecialAttackA_Fire"||attack.sourceAnimSpeed!=1||Math.Abs(attack.duration-55f/30)>.00001f||!npcSourceSoundAssetsV17.Contains(rule.sound))throw new ArgumentException("Changed exact Raven Maximum release contract");
    foreach(string id in new[]{profile.waitState,profile.walkState}){var s=npcStates[rule.npc+"/"+id];var choice=Array.Find(s.choices,c=>c.nextState==rule.readyState);if(choice==null||choice.rate!=25||choice.near!=140||choice.hpLessPercent!=101)throw new ArgumentException("Maximum Cannon must retain original full-HP25% choice");}
    var binding=EffectBinding(new Enemy{sourceNPC=rule.npc},rule.attackState);if(binding==null||!Array.Exists(binding.tracks,t=>t.bone==rule.runtimeBone))throw new ArgumentException("Missing exact original Raven hand track");
   }
   if(rule!=null&&rule.engineeringCombat)ValidateRavenMaximumCombatBindingsV18();alteraRavenMaximumRuleV18=rule;ClearAlteraRavenMaximumReleaseV18();
  }
  void ClearAlteraRavenMaximumReleaseV18(){ClearRavenMaximumCombatStateV18();alteraRavenMaximumMaskV18.Clear();alteraRavenMaximumPreviousV18.Clear();alteraRavenMaximumReleasesV18.Clear();alteraRavenMaximumSequenceV18=0;}
  void AddAlteraRavenMaximumReleaseV18(AlteraRavenMaximumReleaseV18 row){row.sequence=++alteraRavenMaximumSequenceV18;if(alteraRavenMaximumReleasesV18.Count>=256)alteraRavenMaximumReleasesV18.RemoveAt(0);alteraRavenMaximumReleasesV18.Add(row);}
  void AdvanceAlteraRavenMaximumReleaseV18(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   var rule=alteraRavenMaximumRuleV18;if(rule==null||owner==null||owner.sourceNPC!=rule.npc||state.id!=rule.attackState||owner.hp<=0||!owner.active||owner.dormant)return;
   int mask;float previous;if(!alteraRavenMaximumMaskV18.TryGetValue(owner,out mask)||!alteraRavenMaximumPreviousV18.TryGetValue(owner,out previous)||from+.000001f<previous)mask=0;
   float[] times={rule.sourceFirstAt,rule.sourceSecondAt};
   for(int index=0;index<2;index++){
    float marker=times[index]/state.sourceAnimSpeed;if((mask&(1<<index))!=0||!(from<marker&&marker<=to+.000001f))continue;mask|=1<<index;
    float absolute=packetStart+marker-from;
    if(index==0){QueueNpcSound(new NpcTimedSound{id="source-raven-maximum/.01",sound=rule.sound,sourceName="Raven_SP_Maximum_Cannon.ogg",sourceField=rule.sourceCallback+".PlaySound_LUA",sourceTime=.01f,candidateTime=.01f,candidateClock="npc-state-pose-seconds"},owner,0,0,absolute,state.id);AddAlteraRavenMaximumReleaseV18(new AlteraRavenMaximumReleaseV18{kind="source-camera-request",npc=owner.sourceNPC,state=state.id,time=absolute,sourceTime=.01f,sourceCameraMagnitude=5,sourceCameraDuration=.1f,owner=owner});}
    var track=Array.Find(EffectBinding(owner,state.id).tracks,t=>t.bone==rule.runtimeBone);float frame=Clamp(marker*60,0,track.samples.Length-1);int a=(int)frame,b=Math.Min(a+1,track.samples.Length-1);float u=frame-a;
    var p=track.samples[a];var q=track.samples[b];float x=owner.x+owner.facing*(p.x+(q.x-p.x)*u)*owner.visualScale,y=owner.y+(p.y+(q.y-p.y)*u)*owner.visualScale,z=(p.z+(q.z-p.z)*u)*owner.visualScale;
    var argument=new NpcEffectInstance{owner=owner};CaptureNpcSourceLandArgumentV18(argument,null,new NpcEffectEmission{sourceLandPositionYArgument=true});
    var row=new AlteraRavenMaximumReleaseV18{kind="source-effect-request",effect=index==0?"MONSTER_RAVEN_MAXIMUM_CANNON":"MONSTER_RAVEN_MAXIMUM_CANNON_FLY",npc=owner.sourceNPC,state=state.id,sourceBone=rule.sourceBone,time=absolute,sourceTime=times[index],boneX=x,boneY=y,boneZ=z,sourceLandY=argument.sourceLandY,sourceOffsetY=index==0?0:15,owner=owner,sourceEffectConsumerAvailable=false};
    var pose=AlteraRavenMaximumSourcePoseV18==null?(rule.engineeringCombat?EngineeringRavenMaximumPoseV18(owner):null):AlteraRavenMaximumSourcePoseV18(owner);
    if(pose!=null){if(!Finite(pose.logicalDirXPerNative)||!Finite(pose.logicalDirYPerNative)||!Finite(pose.logicalDirZPerNative))throw new InvalidOperationException("Invalid real source direction projection");row.hasResolvedDirection=true;row.sourceMoveDistance=pose.sourceIsRight?rule.sourceMoveTrue:rule.sourceMoveFalse;row.x=x+row.sourceMoveDistance*pose.logicalDirXPerNative;row.y=y+row.sourceMoveDistance*pose.logicalDirYPerNative+row.sourceOffsetY*.28f;row.z=z+row.sourceMoveDistance*pose.logicalDirZPerNative;}
    AddAlteraRavenMaximumReleaseV18(row);if(rule.engineeringCombat)ConsumeRavenMaximumReleaseV18(row);
   }
   alteraRavenMaximumMaskV18[owner]=mask;alteraRavenMaximumPreviousV18[owner]=to;
  }
 }
}
