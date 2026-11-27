using System;

namespace ElswordPixel {
 public sealed partial class GameModel {
  // Preserve the independent GetLandPosition_LUA().y creation argument.
  // It is not the muzzle Y and does not authorize invented terrain collision.
  void CaptureNpcSourceLandArgumentV18(NpcEffectInstance effect,NpcEffectInstance parent,NpcEffectEmission emission){
   if(parent!=null){effect.hasSourceLandY=parent.hasSourceLandY;effect.sourceLandY=parent.sourceLandY;return;}
   if(emission==null||!emission.sourceLandPositionYArgument)return;
   var owner=effect.owner;float y;
   if(NativeTerrain){
    var support=Surface(owner.groundLine);
    if(support==null||owner.x<support.x1-.02f||owner.x>support.x2+.02f||support.Y(owner.x)>owner.y+.07f)
     support=SurfaceBelow(owner.x,owner.y+.05f,owner.dropThrough>0);
    if(support==null)throw new InvalidOperationException("Source effect land-position argument has no original support beneath its owner");
    y=support.Y(owner.x);
   }else y=GroundAt(CurrentRoom,owner.x);
   if(!Finite(y))throw new InvalidOperationException("Invalid source effect land-position height");
   effect.hasSourceLandY=true;effect.sourceLandY=y;
  }
  // Original callback vRot.z. Zero keeps older emissions unchanged; the same
  // transform drives movement direction, visible meshes and damage geometry.
  void ApplyNpcSourceEmissionRotationV18(NpcEffectInstance effect,float sourceRotationZ){
   if(effect==null||!Finite(sourceRotationZ))throw new ArgumentException("Invalid source emission rotation");
   if(TryApplyAlteraCannonBigRotationV18(effect,sourceRotationZ))return;
   if(TryApplyPlainDrillerSourceRotationR1315(effect,sourceRotationZ))return;
   if(Math.Abs(sourceRotationZ)<=.000001f)return;
   if(effect.definition.projectilePhysics)throw new ArgumentException("Rotated ballistic emission needs a verified velocity-reset consumer");
   float angle=sourceRotationZ*(float)Math.PI/180;
   effect.dirX=effect.facing*(float)Math.Cos(angle);effect.dirY=effect.facing*(float)Math.Sin(angle);
   npcSourceEffectRotationsV18[effect]=angle;
  }
 }
}
