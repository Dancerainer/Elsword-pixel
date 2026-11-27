using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel.CargoPresentationV21 {
 [Serializable] public sealed class OutskirtsVisualCurve {public string property;public float start,end;public bool fade;public float[] ranges;}
 [Serializable] public sealed class OutskirtsVisualParticle {
  public string id,texture,sourceBlend,destinationBlend,particleType,billboardType;public float[] lifetime,rate,radius,gravity,gravityRanges;public float triggerTime;public int capacity,triggerCount,drawCount,layer;public bool useLand;public OutskirtsVisualCurve[] curves;
 }
 [Serializable] public sealed class SourceVisualEffectMesh {public string effect,mesh;public float bakedTemplateRotationZ;public bool compensateSourceAngle;}
 [Serializable] public sealed class OutskirtsVisualMeshFrame {public string atlas;public float timeSeconds,durationSeconds;public int[] rect;public float[] anchor;}
 [Serializable] public sealed class OutskirtsVisualMesh {public string id,sourceTemplate,sourceBlend,destinationBlend;public float durationSeconds,pixelsPerNativeUnit;public int fps;public OutskirtsVisualMeshFrame[] frames;}
 [Serializable] public sealed class OutskirtsNpcMeshManifest {public OutskirtsVisualMesh[] components;}
 public sealed partial class CargoFxViewV21 {
  sealed class OutskirtsMeshView {public OutskirtsVisualMesh data;public Sprite[] sprites;public Material material;}
  static float OutskirtsRandom(int seed,int channel){unchecked{uint n=(uint)(seed*374761393+channel*668265263);n=(n^(n>>13))*1274126177;return(n^(n>>16))/(float)uint.MaxValue;}}
  static float OutskirtsRange(float[] values,int channel,int seed,int salt){return Mathf.Lerp(values[channel*2],values[channel*2+1],OutskirtsRandom(seed,salt+channel));}
  static float OutskirtsCurve(OutskirtsVisualParticle p,string property,int channel,float age,int seed,float fallback){
   float value=fallback;for(int i=0;i<p.curves.Length;i++){var c=p.curves[i];if(c.property!=property||age<c.start||channel*2+1>=c.ranges.Length)continue;float target=OutskirtsRange(c.ranges,channel,seed,101+i*7);value=c.fade&&age<c.end?Mathf.Lerp(value,target,Mathf.InverseLerp(c.start,c.end,age)):target;}return value;
  }
  static float OutskirtsVelocityIntegral(OutskirtsVisualParticle p,int channel,float age,int seed){float total=0,previous=OutskirtsCurve(p,"Velocity",channel,0,seed,0);int steps=Mathf.Max(1,Mathf.CeilToInt(age*120));for(int i=1;i<=steps;i++){float value=OutskirtsCurve(p,"Velocity",channel,age*i/steps,seed,0);total+=(previous+value)*.5f*age/steps;previous=value;}return total;}
  static void ValidateLakeParticleV18(OutskirtsVisualParticle p){
   RequireOutskirtsVisual(p!=null&&SteamVector(p.lifetime,2)&&p.lifetime[0]>0&&p.lifetime[1]>=p.lifetime[0]&&SteamVector(p.rate,2)&&p.rate[0]>0&&p.rate[1]>=p.rate[0]&&SteamVector(p.radius,6)&&SteamVector(p.gravity,3)&&p.capacity>0&&p.triggerCount>0&&p.drawCount>0&&SteamFinite(p.triggerTime)&&p.triggerTime>=0&&p.curves!=null,"invalid Lake particle timing");
   RequireOutskirtsVisual(p.gravityRanges==null||p.gravityRanges.Length==0||SteamVector(p.gravityRanges,6),"invalid Lake random gravity");
   foreach(var c in p.curves)RequireOutskirtsVisual(c!=null&&c.ranges!=null&&c.ranges.Length%2==0&&Array.TrueForAll(c.ranges,SteamFinite)&&SteamFinite(c.start)&&SteamFinite(c.end)&&c.start>=0&&c.end>=c.start,"invalid Lake particle curve");
  }
 }}
