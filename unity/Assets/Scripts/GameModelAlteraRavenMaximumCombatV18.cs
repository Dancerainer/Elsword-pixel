using System;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraRavenMaximumEffectCombatV18 {
  public string adapter,sourceLifeType;
  public float sourceXEndTick,sourceAniSpeed,engineeringTicksPerSecond,sourceFixedLifetime;
  public int sourcePlayCount;public bool sourceForceFly,nativeTickRateDeclared;
 }
 public sealed partial class GameModel {
  const string RavenMaximumMainV18="MONSTER_RAVEN_MAXIMUM_CANNON",RavenMaximumFlyV18="MONSTER_RAVEN_MAXIMUM_CANNON_FLY";
  Enemy ravenMaximumHurtOwnerV18;int ravenMaximumHurtImpactV18=-1;float ravenMaximumHurtUntilV18;
  static bool CloseMaximumV18(float a,float b){return Finite(a)&&Math.Abs(a-b)<.0001f;}
  static bool EmptyMaximumCombatV18(AlteraRavenMaximumEffectCombatV18 r){return r!=null&&string.IsNullOrEmpty(r.adapter)&&string.IsNullOrEmpty(r.sourceLifeType)&&r.sourceXEndTick==0&&r.sourceAniSpeed==0&&r.engineeringTicksPerSecond==0&&r.sourceFixedLifetime==0&&r.sourcePlayCount==0&&!r.sourceForceFly&&!r.nativeTickRateDeclared;}
  static void ValidateAlteraRavenMaximumCombatV18(NpcEffectDefinition d){
   var r=d.alteraRavenMaximumCombat;if(r==null)return;if(EmptyMaximumCombatV18(r)){d.alteraRavenMaximumCombat=null;return;}
   bool main=d.id==RavenMaximumMainV18,fly=d.id==RavenMaximumFlyV18;
   if(!main&&!fly||r.adapter!="raven-maximum-engineering-combat-v18-r1"||r.nativeTickRateDeclared||r.sourceLifeType!=(main?"LT_PLAY_COUNT":"LT_TIME")||r.sourceXEndTick!=(main?7040:320)||!CloseMaximumV18(r.sourceAniSpeed,main?1.1f:1)||r.engineeringTicksPerSecond!=(main?4800:0)||!CloseMaximumV18(r.sourceFixedLifetime,main?0:.1f)||r.sourcePlayCount!=(main?1:0)||r.sourceForceFly!=fly||
    !CloseMaximumV18(d.lifetime,main?4f/3:.1f)||d.physical!=0||!CloseMaximumV18(d.magic,.1f)||d.reaction!=(main?HitReaction.Hurt:HitReaction.Launch)||!CloseMaximumV18(d.launch,main?0:336)||!CloseMaximumV18(d.push,main?0:448)||!d.repeat||!CloseMaximumV18(d.repeatGap,main?.2f:10)||d.stopAttacker!=0||d.stopDefender!=0||d.reflectable||d.randomYaw||d.lockOnUntil!=0||d.projectilePhysics||d.damageFrom!=0||!CloseMaximumV18(d.damageUntil,d.lifetime)||!string.IsNullOrEmpty(d.contactChild)||!string.IsNullOrEmpty(d.deathChild)||!string.IsNullOrEmpty(d.hitChild)||d.deathChildren!=null&&d.deathChildren.Length>0||!string.IsNullOrEmpty(d.visualId)||d.visualFromMesh!=main||d.baseSpheres.Length!=4)
    throw new ArgumentException("Changed exact source Maximum Cannon combat adapter");
   float[] sphere=main?new[]{33.24803894f,.000136719f,0f,33.94819583f}:new[]{0f,0f,0f,12.12435565f};
   for(int i=0;i<4;i++)if(!CloseMaximumV18(d.baseSpheres[i],sphere[i]))throw new ArgumentException("Maximum Cannon must retain original mesh helper geometry");
   if(d.geometrySamples==null||d.geometrySamples.Length!=2)throw new ArgumentException("Maximum Cannon needs its constant original helper ancestry");
   foreach(var g in d.geometrySamples){if(g.spheres==null||g.spheres.Length!=4||g.attackLines==null||g.attackLines.Length!=0)throw new ArgumentException("Changed Maximum helper shape");for(int i=0;i<4;i++)if(!CloseMaximumV18(g.spheres[i],sphere[i]))throw new ArgumentException("Invented moving Maximum helper");}
   foreach(var s in d.samples)if(!CloseMaximumV18(s.scaleX,main?1.5f:4.5f)||!CloseMaximumV18(s.scaleY,s.scaleX)||!CloseMaximumV18(s.scaleZ,s.scaleX)||s.moveX!=0)throw new ArgumentException("Changed source Maximum size or invented travel");
  }
  void ValidateRavenMaximumCombatBindingsV18(){
   foreach(string id in new[]{RavenMaximumMainV18,RavenMaximumFlyV18}){NpcEffectDefinition d;if(!npcEffectDefinitions.TryGetValue(id,out d)||d.alteraRavenMaximumCombat==null)throw new ArgumentException("Maximum source consumer requires both verified source effect definitions");ValidateAlteraRavenMaximumCombatV18(d);}
  }
  void ClearRavenMaximumCombatStateV18(){ravenMaximumHurtOwnerV18=null;ravenMaximumHurtImpactV18=-1;ravenMaximumHurtUntilV18=0;}
  AlteraRavenSourcePoseV18 EngineeringRavenMaximumPoseV18(Enemy owner){
   // Declared flat-path projection of source GetDirVector + GetIsRight. The
   // source MovePos helper adds dir*distance. This is not measured native yaw.
   return new AlteraRavenSourcePoseV18{sourceIsRight=owner.facing>0,logicalDirXPerNative=.28f};
  }
  void ConsumeRavenMaximumReleaseV18(AlteraRavenMaximumReleaseV18 row){
   if(!row.hasResolvedDirection)throw new InvalidOperationException("Maximum source direction was unresolved");
   var owner=row.owner;var emission=new NpcEffectEmission{effect=row.effect,scaleX=1,scaleY=1,scaleZ=1,sourceLandPositionYArgument=true};
   float bx,by,bz;NpcAttachment(owner,emission,row.time,out bx,out by,out bz);
   emission.offsetX=(row.x-bx)/owner.facing/owner.visualScale;emission.offsetY=(row.y-by)/owner.visualScale;emission.offsetZ=(row.z-bz)/owner.visualScale;
   var effect=SpawnNpcEffect(owner,npcEffectDefinitions[row.effect],emission,row.time);
   if(!effect.hasSourceLandY||!CloseMaximumV18(effect.sourceLandY,row.sourceLandY))throw new InvalidOperationException("Maximum lost original independent landY argument");
   row.sourceEffectConsumerAvailable=true;
  }
  bool TryContactAlteraRavenMaximumCombatV18(NpcEffectInstance effect,EffectPose pose,float at){
   var d=effect.definition;if(d.alteraRavenMaximumCombat==null)return false;
   if(effect.reflected)throw new InvalidOperationException("Unreflectable source Maximum was reflected");
   if(effect.age<d.damageFrom||effect.age>d.damageUntil||at+1e-6f<effect.nextPlayerHit)return true;
   float x,y,z,r;NpcEffectSphere(effect,pose,0,out x,out y,out z,out r);
   float px=Clamp(x,player.x-9.8f,player.x+9.8f),py=Clamp(y,player.y,player.y+42),pz=Clamp(z,-9.8f,9.8f);
   if((px-x)*(px-x)+(py-y)*(py-y)+(pz-z)*(pz-z)>r*r)return true;
   // The demo's generic Hurt adds .22s of invulnerability. Source Maximum
   // explicitly repeats every .2s and finishes at1.3. Remove only the iframe
   // this same owner's immediately preceding Maximum hit created. Preserve
   // actual skill/getup invulnerability and unrelated impact serials.
   bool bridge=player.state==CombatState.Hurt&&player.invincible>0&&ravenMaximumHurtOwnerV18==effect.owner&&ravenMaximumHurtImpactV18==PlayerImpactSerial&&elapsed<=ravenMaximumHurtUntilV18+.0001f&&Math.Abs(player.invincible-Math.Max(0,ravenMaximumHurtUntilV18-elapsed))<.002f;
   float previousInvincible=player.invincible;if(bridge)player.invincible=0;
   bool protectedReaction=PlayerHasSuperArmor||player.state==CombatState.Knockdown||player.state==CombatState.GettingUp;
   int dealt=ApplyEnemyAttackToPlayer((int)Math.Round(effect.physicalAttack*d.physical+effect.magicAttack*d.magic),effect.x-effect.facing,d.reaction,d.launch);
   if(dealt<=0){if(bridge)player.invincible=previousInvincible;return true;}
   effect.nextPlayerHit=at+d.repeatGap;
   if(!protectedReaction&&player.hp>0){
    if(d.alteraRavenMaximumCombat.sourceForceFly){player.state=CombatState.Airborne;player.knockdownOnLanding=true;player.vy=d.launch;player.vx=effect.facing*d.push;player.stun=.18f;player.clip="launched";ClearRavenMaximumCombatStateV18();}
    else{player.vx=effect.facing*d.push;ravenMaximumHurtOwnerV18=effect.owner;ravenMaximumHurtImpactV18=PlayerImpactSerial;ravenMaximumHurtUntilV18=elapsed+player.invincible;}
   }
   TraceNpcEffect("hit-player",effect,dealt,at);EmitNpcHitChild(effect,at,px,py,pz);ApplyNpcPlayerStatus(d,at);return true;
  }
 }
}
