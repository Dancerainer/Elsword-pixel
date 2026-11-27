using System;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraCannonBigBallisticRuleV18 {
  public string sourceTemplate,sourceMesh;
  public float sourceVelocityX,sourceGravityY,sourceLifetime,sourceNoCheckLandTime,sourceRollZ;
  public bool sourceCrashLand;
 }
 public sealed partial class GameModel {
  static bool EmptyAlteraCannonBigBallisticV18(AlteraCannonBigBallisticRuleV18 r){return r!=null&&string.IsNullOrEmpty(r.sourceTemplate)&&string.IsNullOrEmpty(r.sourceMesh)&&r.sourceVelocityX==0&&r.sourceGravityY==0&&r.sourceLifetime==0&&r.sourceNoCheckLandTime==0&&r.sourceRollZ==0&&!r.sourceCrashLand;}
  static bool HasAlteraCannonBigBallisticV18(NpcEffectDefinition d){return d!=null&&d.alteraCannonBigBallistic!=null&&!EmptyAlteraCannonBigBallisticV18(d.alteraCannonBigBallistic);}
  static void ValidateAlteraCannonBigBallisticV18(NpcEffectDefinition d){
   var r=d.alteraCannonBigBallistic;if(r==null)return;if(EmptyAlteraCannonBigBallisticV18(r)){d.alteraCannonBigBallistic=null;return;}
   bool first=d.id=="DamageEffect_CANNON_BIG_ATTACK1",second=d.id=="DamageEffect_CANNON_BIG_ATTACK2";
   if(!first&&!second||r.sourceTemplate!=(first?"Mesh_CANNON_BIG_ATTACK1":"Mesh_CANNON_BIG_ATTACK2")||r.sourceMesh!="Nasod_Bullet01.X"||r.sourceVelocityX!=1500||r.sourceGravityY!=-2000||r.sourceLifetime!=1.5f||r.sourceNoCheckLandTime!=(first?.8f:.1f)||r.sourceRollZ!=(first?25:-25)||!r.sourceCrashLand||
    !d.projectilePhysics||d.lifetime!=1.5f||d.projectileGravityY!=-560||d.projectileBounceX!=0||d.projectileBounceY!=0||d.projectileVelocities.Length!=2||d.lockOnUntil!=0||d.randomYaw||!d.reflectable||d.projectileVelocities[0].time!=0||d.projectileVelocities[1].time!=1.5f)throw new ArgumentException("Changed exact source CannonBig ballistic contract");
   foreach(var v in d.projectileVelocities)if(v.x!=420||v.y!=0||v.reset)throw new ArgumentException("CannonBig source has one constant initial velocity; no invented reset/fade");
   if(d.deathChildren==null||d.deathChildren.Length!=3)throw new ArgumentException("CannonBig requires the three exact source flame children");
   foreach(string suffix in new[]{"RIGHT","LEFT","CENTER"}){var child=Array.Find(d.deathChildren,c=>c!=null&&c.effect=="DamageEffect_CANNON_BIG_ATTACK1_FLAME_"+suffix);if(child==null||child.offsetX!=0||child.offsetY!=0||child.offsetZ!=0||child.rotationZ!=-r.sourceRollZ||child.applyParentRotation||!child.rotateOffset)throw new ArgumentException("Changed original CannonBig death-child identity/transform");}
  }
  static bool SupportsAlteraCannonBigRotationV18(NpcEffectDefinition d,float roll){return HasAlteraCannonBigBallisticV18(d)&&Finite(roll)&&roll==d.alteraCannonBigBallistic.sourceRollZ;}
  static void ValidateAlteraCannonBigEmissionV18(NpcEffectDefinition d,NpcEffectBinding binding,NpcEffectEmission emission){
   if(!HasAlteraCannonBigBallisticV18(d))return;
   if(binding.npc!="NUI_CANNON_BIG"||binding.state!=(d.id=="DamageEffect_CANNON_BIG_ATTACK1"?"CANNON_BIG_ATTACK_1":"CANNON_BIG_ATTACK_2")||!SupportsAlteraCannonBigRotationV18(d,emission.rotationZ)||!emission.sourceLandPositionYArgument||emission.follow||emission.lockOnMode!=0||emission.facingMultiplier!=1)throw new ArgumentException("CannonBig ballistic emission requires its exact original callback/roll/land argument");
  }
  bool TryApplyAlteraCannonBigRotationV18(NpcEffectInstance e,float roll){
   if(!HasAlteraCannonBigBallisticV18(e.definition))return false;
   if(!SupportsAlteraCannonBigRotationV18(e.definition,roll))throw new ArgumentException("Changed CannonBig local source roll");
   float angle=roll*(float)Math.PI/180;
   // Flat projected adapter for the original full GetRotateDegree + local Z
   // roll: roll local velocity first, then facing's yaw. Y does not flip with
   // facing. Gravity remains a separate world-space Y acceleration.
   e.dirX=e.facing*(float)Math.Cos(angle);e.dirY=(float)Math.Sin(angle);
   npcSourceEffectRotationsV18[e]=e.facing*angle;return true;
  }
  bool TryInitializeAlteraCannonBigBallisticV18(NpcEffectInstance e){
   if(!HasAlteraCannonBigBallisticV18(e.definition))return false;
   if(!e.hasSourceLandY)throw new InvalidOperationException("CannonBig source CreateInstance requires independent owner landY");
   e.projectileVx=420*e.dirX;e.projectileVy=420*e.dirY;return true;
  }
  bool TryMoveAlteraCannonBigBallisticV18(NpcEffectInstance e,float from,float to){
   if(!HasAlteraCannonBigBallisticV18(e.definition))return false;
   var rule=e.definition.alteraCannonBigBallistic;float dt=to-from;if(dt<=0)return true;
   // CrashLand differs from UseLand restitution. Query only original projected
   // terrain near the actual effect position, never player elevation or an
   // immortal plane at the source owner's captured Y. The native radius90
   // plane-query and packet order remain explicitly unmeasured adapters.
   if(from>rule.sourceNoCheckLandTime){
    // A sampled position can already be slightly below the crossed plane.
    // The radius90 reference is projected to25.2. Captured source landY must
    // never raise this ceiling to a remote platform above the actual shot.
    var support=NativeTerrain?SurfaceBelow(e.x,e.y+25.2f):null;
    float floor=NativeTerrain?(support==null?float.NegativeInfinity:support.Y(e.x)):GroundAt(CurrentRoom,e.x);
    if(e.y<=floor+.00001f){e.age=from;EndNpcEffect(e,"source-crash-land",e.bornAt+from);return true;}
   }
   float vx=e.projectileVx,vy=e.projectileVy,g=e.definition.projectileGravityY;
   e.x+=vx*dt;e.y+=vy*dt+.5f*g*dt*dt;e.projectileVy=vy+g*dt;
   return true;
  }
  void OnAlteraCannonBigReflectedV18(NpcEffectInstance e){
   if(!HasAlteraCannonBigBallisticV18(e.definition))return;
   // Existing horizontal reflection is an explicit playable adapter, not a
   // claim about the unmeasured native projectile-velocity callback. Reset its
   // actual integrated velocity too, so rendered and colliding paths agree.
   e.projectileVx=420*e.facing;e.projectileVy=0;
  }
 }
}
