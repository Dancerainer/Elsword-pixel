using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraRavenCannonBladeRuleV18 {
  public string adapter,sourceTemplate,sourceMesh,sourceReaction;
  public bool sourceFollowLine,sourceArrangedFly;
  public float sourceVelocityX,sourceLifetime,sourceQueryRadius,sourceBackX,sourceBackY;
 }
 public struct AlteraRavenCannonBladeStateV18 {public int birthFacing,lineId,queries,hits;public bool foundSupport,lastHitFromFront;public float initialY,latestFloor,previousX,previousZ;}
 public sealed partial class GameModel {
  const string RavenCannonBladeIdV18="MONSTER_RAVEN_CANNON_BLADE";
  readonly Dictionary<NpcEffectInstance,AlteraRavenCannonBladeStateV18> ravenCannonBladesV18=new Dictionary<NpcEffectInstance,AlteraRavenCannonBladeStateV18>();
  Enemy ravenCannonBladeHurtOwnerV18;int ravenCannonBladeHurtImpactV18=-1;float ravenCannonBladeHurtUntilV18;
  static bool BladeCloseV18(float a,float b){return Finite(a)&&Math.Abs(a-b)<.0001f;}
  static bool EmptyRavenCannonBladeV18(AlteraRavenCannonBladeRuleV18 r){return r!=null&&string.IsNullOrEmpty(r.adapter)&&string.IsNullOrEmpty(r.sourceTemplate)&&string.IsNullOrEmpty(r.sourceMesh)&&string.IsNullOrEmpty(r.sourceReaction)&&!r.sourceFollowLine&&!r.sourceArrangedFly&&r.sourceVelocityX==0&&r.sourceLifetime==0&&r.sourceQueryRadius==0&&r.sourceBackX==0&&r.sourceBackY==0;}
  static void ValidateAlteraRavenCannonBladeV18(NpcEffectDefinition d){
   var r=d.alteraRavenCannonBlade;if(r==null)return;if(EmptyRavenCannonBladeV18(r)){d.alteraRavenCannonBlade=null;return;}
   if(d.id!=RavenCannonBladeIdV18||r.adapter!="raven-cannon-blade-unrolled-line-v18-r1"||r.sourceTemplate!="Raven_CannonBlade"||r.sourceMesh!="DummyAttackBox_50x50x50.X"||r.sourceReaction!="RT_BIG_DAMAGE"||!r.sourceFollowLine||!r.sourceArrangedFly||r.sourceVelocityX!=800||r.sourceLifetime!=1||r.sourceQueryRadius!=30||r.sourceBackX!=600||r.sourceBackY!=850||
    d.lifetime!=1||d.physical!=0||!BladeCloseV18(d.magic,.17f)||d.reaction!=HitReaction.Hurt||!BladeCloseV18(d.launch,238)||!BladeCloseV18(d.push,168)||!d.repeat||!BladeCloseV18(d.repeatGap,.15f)||d.reflectable||d.randomYaw||d.lockOnUntil!=0||d.projectilePhysics||d.stopAttacker!=0||d.stopDefender!=0||d.damageFrom!=0||d.damageUntil!=1||d.visualFromMesh||!string.IsNullOrEmpty(d.visualId)||!string.IsNullOrEmpty(d.contactChild)||!string.IsNullOrEmpty(d.deathChild)||!string.IsNullOrEmpty(d.hitChild)||d.deathChildren!=null&&d.deathChildren.Length>0||d.baseSpheres.Length!=4)
    throw new ArgumentException("Changed exact first-Airship CannonBlade source contract");
   float[] sphere={0,0,0,12.12435565f};for(int i=0;i<4;i++)if(!BladeCloseV18(d.baseSpheres[i],sphere[i]))throw new ArgumentException("CannonBlade must retain actual source dummy helper");
   if(d.geometrySamples==null||d.geometrySamples.Length!=2)throw new ArgumentException("CannonBlade lacks constant original helper ancestry");
   foreach(var g in d.geometrySamples){if(g.spheres.Length!=4||g.attackLines.Length!=0)throw new ArgumentException("Changed CannonBlade helper shape");for(int i=0;i<4;i++)if(!BladeCloseV18(g.spheres[i],sphere[i]))throw new ArgumentException("Invented CannonBlade helper motion");}
   foreach(var s in d.samples)if(s.scaleX!=3||s.scaleY!=3||s.scaleZ!=3||s.alpha!=0||!BladeCloseV18(s.moveX,s.time*224))throw new ArgumentException("Changed source CannonBlade invisible carrier size/velocity");
  }
  static void ValidateAlteraRavenCannonBladeEmissionV18(NpcEffectDefinition d,NpcEffectBinding b,NpcEffectEmission e){
   if(d.alteraRavenCannonBlade==null)return;
   if(b.npc!="NUI_RAVEN"||b.state!="RAVEN_SP_CANNON_BLADE"||b.unitScale!=1.25f||b.sets.Length!=1||b.sets[0].id!="EffectSet_NPC_Raven_Cannon_Blade"||b.sets[0].attachAt!=0||b.sets[0].emissions.Length!=1||!object.ReferenceEquals(e,b.sets[0].emissions[0])||e.bone!="Dummy2_LHand"||e.start!=0||e.offsetX!=0||e.offsetY!=-17.5f||e.offsetZ!=0||e.rotationZ!=0||e.follow||e.lockOnMode!=0||e.facingMultiplier!=1||e.scaleX!=1||e.scaleY!=1||e.scaleZ!=1||!string.IsNullOrEmpty(e.ownerState))throw new ArgumentException("CannonBlade requires its original Raven source set/bone/scale/offset");
  }
  AlteraRavenCannonBladeStateV18 CannonBladeStateV18(NpcEffectInstance e){
   AlteraRavenCannonBladeStateV18 s;if(ravenCannonBladesV18.TryGetValue(e,out s))return s;
   if(e.owner==null||e.owner.sourceNPC!="NUI_RAVEN"||e.reflected)throw new InvalidOperationException("Invalid exact CannonBlade source owner or reflection");
   s=new AlteraRavenCannonBladeStateV18{birthFacing=e.facing,lineId=-1,initialY=e.y};ravenCannonBladesV18.Add(e,s);FollowRavenCannonBladeV18(e,true);return ravenCannonBladesV18[e];
  }
  void FollowRavenCannonBladeV18(NpcEffectInstance e,bool initial){
   var s=ravenCannonBladesV18[e];s.queries++;
   // Fixed-V5 FollowLine only preserves position Y unless FOLLOW_LINE_NEW or
   // FOLLOW_LINE_GRAVITY is declared. Neither flag, FOLLOW_ON_LINE, MUST_ON_LINE
   // nor elasticity exists on this original2016 effect. Do not invent a slope
   // climb, drop, gap stop or reflection. Original3D CanDown/yaw is explicitly
   // projected to the room's unrolled2D path; query from the effect, not player.
   var line=NativeTerrain?SurfaceBelow(e.x,e.y+8.4f):null;
   s.foundSupport=NativeTerrain?line!=null:true;s.lineId=line==null?-1:line.id;s.latestFloor=NativeTerrain?(line==null?float.NegativeInfinity:line.Y(e.x)):GroundAt(CurrentRoom,e.x);
   e.dirX=s.birthFacing;e.dirY=0;e.facing=s.birthFacing;
   if(!initial){s.previousX=e.x;s.previousZ=e.z;}
   ravenCannonBladesV18[e]=s;
  }
  public bool TryGetAlteraRavenCannonBladeStateV18(NpcEffectInstance e,out AlteraRavenCannonBladeStateV18 state){return ravenCannonBladesV18.TryGetValue(e,out state);}
  bool TryMoveAlteraRavenCannonBladeV18(NpcEffectInstance e,float from,float to){
   if(e.definition.alteraRavenCannonBlade==null)return false;var s=CannonBladeStateV18(e);
   var a=EffectAt(e.definition,from);var b=EffectAt(e.definition,to);e.x+=s.birthFacing*(b.move-a.move);FollowRavenCannonBladeV18(e,false);return true;
  }
  void RemoveAlteraRavenCannonBladeV18(NpcEffectInstance e){ravenCannonBladesV18.Remove(e);}
  void ClearAlteraRavenCannonBladeV18(){ravenCannonBladesV18.Clear();ravenCannonBladeHurtOwnerV18=null;ravenCannonBladeHurtImpactV18=-1;ravenCannonBladeHurtUntilV18=0;}
  float CannonBladeHeightV18(Fighter f){var line=NativeTerrain?SurfaceBelow(f.x,f.y+.05f):null;return NativeTerrain?(line==null?float.PositiveInfinity:Math.Max(0,f.y-line.Y(f.x))):Math.Max(0,f.y-GroundAt(CurrentRoom,f.x));}
  bool TryContactAlteraRavenCannonBladeV18(NpcEffectInstance e,EffectPose pose,float at){
   var d=e.definition;if(d.alteraRavenCannonBlade==null)return false;var s=CannonBladeStateV18(e);
   if(e.age<d.damageFrom||e.age>d.damageUntil||at+1e-6f<e.nextPlayerHit)return true;
   float x,y,z,r;NpcEffectSphere(e,pose,0,out x,out y,out z,out r);float px=Clamp(x,player.x-9.8f,player.x+9.8f),py=Clamp(y,player.y,player.y+42),pz=Clamp(z,-9.8f,9.8f);
   if((px-x)*(px-x)+(py-y)*(py-y)+(pz-z)*(pz-z)>r*r)return true;
   float expected=Math.Max(0,ravenCannonBladeHurtUntilV18-elapsed),oldInv=player.invincible;
   bool bridge=(player.state==CombatState.Hurt||player.state==CombatState.Airborne)&&oldInv>0&&ravenCannonBladeHurtOwnerV18==e.owner&&ravenCannonBladeHurtImpactV18==PlayerImpactSerial&&Math.Abs(oldInv-expected)<.002f;
   if(bridge)player.invincible=0;
   bool protectedReaction=PlayerHasSuperArmor||player.state==CombatState.Knockdown||player.state==CombatState.GettingUp,grounded=Grounded(player);
   float vy=player.vy;bool nearLine=CannonBladeHeightV18(player)<42||CannonBladeHeightV18(e.owner)<25.2f;
   // ARRANGED_FLY supplies the source effect's birth direction to both guard
   // orientation and reaction. Comparing impact/owner positions after crossing
   // a body would reverse it. This does not convert RT_BIG_DAMAGE to RT_FLY.
   int dealt=ApplyEnemyAttackToPlayer((int)Math.Round(e.physicalAttack*d.physical+e.magicAttack*d.magic),player.x-s.birthFacing,HitReaction.Hurt,d.launch);
   if(dealt<=0){if(bridge)player.invincible=oldInv;return true;}
   e.nextPlayerHit=at+d.repeatGap;s.hits++;s.lastHitFromFront=s.birthFacing!=player.facing;ravenCannonBladesV18[e]=s;
   if(!protectedReaction&&player.hp>0){
    player.vx=s.birthFacing*d.push;player.vy=nearLine?Math.Min(0,vy):d.launch;
    if(!grounded||!nearLine){player.state=CombatState.Airborne;player.knockdownOnLanding=false;}
    ravenCannonBladeHurtOwnerV18=e.owner;ravenCannonBladeHurtImpactV18=PlayerImpactSerial;ravenCannonBladeHurtUntilV18=elapsed+player.invincible;
   }
   TraceNpcEffect("hit-player",e,dealt,at);EmitNpcHitChild(e,at,px,py,pz);ApplyNpcPlayerStatus(d,at);return true;
  }
 }
}
