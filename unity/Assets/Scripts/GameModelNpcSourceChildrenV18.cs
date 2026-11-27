using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  readonly Dictionary<NpcEffectInstance,float> npcSourceEffectRotationsV18=new Dictionary<NpcEffectInstance,float>();
  void ClearNpcSourceDeathChildrenV18(){npcSourceEffectRotationsV18.Clear();}
  void RemoveNpcSourceEffectRotationV18(NpcEffectInstance effect){if(effect!=null)npcSourceEffectRotationsV18.Remove(effect);}
  // Relative world-space XY already contains facing and scale. Only new source
  // child transforms enter this map; old guided or reflected effects retain
  // their existing behavior until explicitly compiled with the new contract.
  void ApplyNpcSourceEffectRotationV18(NpcEffectInstance effect,ref float x,ref float y){
   float angle;if(effect==null||!npcSourceEffectRotationsV18.TryGetValue(effect,out angle))return;
   float cs=(float)Math.Cos(angle),sn=(float)Math.Sin(angle),a=x;x=a*cs-y*sn;y=a*sn+y*cs;
  }
  static float NpcSourceParentAngleV18(NpcEffectInstance parent){
   float angle=(float)Math.Atan2(parent.dirY,parent.dirX);if(parent.facing<0)angle-=(float)Math.PI;
   return angle;
  }
  void EmitNpcSourceDeathChildrenV18(NpcEffectInstance parent,float at){if(TryEmitAlteraRavenMissileRainChildrenV18(parent,at))return;
   var rows=parent.definition.deathChildren;if(rows==null||rows.Length==0)return;
   float parentAngle=NpcSourceParentAngleV18(parent);
   foreach(var row in rows){
    var child=SpawnNpcEffect(parent.owner,npcEffectDefinitions[row.effect],null,at,parent);
    float angle=row.rotationZ*(float)Math.PI/180+(row.applyParentRotation?parentAngle:0);
    float x=parent.facing*row.offsetX*parent.ownerScale,y=row.offsetY*parent.ownerScale;
    if(row.rotateOffset){float cs=(float)Math.Cos(parentAngle),sn=(float)Math.Sin(parentAngle),a=x;x=a*cs-y*sn;y=a*sn+y*cs;}
    child.x=parent.x+x;child.y=parent.y+y;child.z=parent.z+row.offsetZ*parent.ownerScale;
    child.dirX=child.facing*(float)Math.Cos(angle);child.dirY=child.facing*(float)Math.Sin(angle);
    // The source Cannon flame children use curve-driven movement, not gravity.
    // Do not pretend that rotating an initial velocity alone implements later
    // source velocity resets in an independently integrated ballistic payload.
    if(child.definition.projectilePhysics&&Math.Abs(angle)>.000001f)throw new InvalidOperationException("Rotated ballistic source child needs a verified velocity-reset consumer.");
    npcSourceEffectRotationsV18[child]=angle;
    TraceNpcEffect("source-death-child",child,0,at);
   }
  }
 }
}
