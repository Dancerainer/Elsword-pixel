using System;using UnityEngine;
namespace ElswordPixel {
 [Serializable]public sealed class CoreStopWorldAspectEvidenceV28 {
  public string policy="pixelRect-auto-equivalent",failure;public int cameraID;public bool admittedAutoEquivalent,writeAttempted,exactOwned,restored,autoEquivalentPolicyRestored,samePixelSurface,numericValueRestored;
  public float originalAspect,originalPixelAspect,ownedAspect,restoredAspect,restoredPixelAspect;public Rect originalRect,originalPixelRect,restoredRect,restoredPixelRect;
 }
 // Camera exposes no readable auto-aspect bit. Admit only a value equivalent
 // to its actual floating pixelRect ratio, without ResetAspect probing. A
 // manual value equal to that ratio is observationally equivalent; no stronger
 // claim about its hidden native mode is made. Native target aspect is separate.
 sealed class CoreStopWorldAspectOwnershipV28:IDisposable {
  internal const float Exact=480f/270;readonly Camera camera;internal readonly CoreStopWorldAspectEvidenceV28 evidence;bool disposed;
  internal static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
  internal static bool Equivalent(float a,float b){return Finite(a)&&Finite(b)&&Math.Abs(a-b)<=.00001f*Math.Max(1,Math.Abs(b));}
  internal static bool AutoEquivalent(float aspect,float width,float height){return Finite(aspect)&&aspect>0&&Finite(width)&&width>0&&Finite(height)&&height>0&&Equivalent(aspect,width/height);}
  internal CoreStopWorldAspectOwnershipV28(Camera c,CoreStopWorldAspectEvidenceV28 d,float requestedAspect=Exact){
   camera=c;evidence=d??new CoreStopWorldAspectEvidenceV28();if(!camera)throw new InvalidOperationException("Core world aspect ownership requires an existing camera");evidence.cameraID=c.GetInstanceID();evidence.originalAspect=c.aspect;evidence.originalRect=c.rect;evidence.originalPixelRect=c.pixelRect;var p=c.pixelRect;evidence.originalPixelAspect=p.width/p.height;
   evidence.admittedAutoEquivalent=c.targetTexture==null&&Finite(p.x)&&Finite(p.y)&&AutoEquivalent(c.aspect,p.width,p.height);if(!evidence.admittedAutoEquivalent){evidence.failure="world-aspect-not-null-target-pixelRect-auto-equivalent";throw new InvalidOperationException(evidence.failure);}
   try{evidence.writeAttempted=true;c.aspect=requestedAspect;evidence.ownedAspect=c.aspect;evidence.exactOwned=Finite(c.aspect)&&Math.Abs(c.aspect-requestedAspect)<.0000001f;if(!evidence.exactOwned)throw new InvalidOperationException("Core world and scenery cameras could not share the requested display aspect");}
   catch(Exception ex){evidence.failure=ex.ToString();try{Dispose();}catch(Exception restore){throw new AggregateException("Core world aspect acquire and rollback failed",ex,restore);}throw;}
  }
  public void Dispose(){if(disposed)return;disposed=true;if(!evidence.writeAttempted)return;if(!camera){evidence.failure="owned-world-camera-destroyed-before-aspect-return";throw new InvalidOperationException(evidence.failure);}camera.ResetAspect();evidence.restoredAspect=camera.aspect;evidence.restoredRect=camera.rect;evidence.restoredPixelRect=camera.pixelRect;var p=camera.pixelRect;evidence.restoredPixelAspect=p.width/p.height;evidence.samePixelSurface=evidence.originalPixelRect==p;evidence.numericValueRestored=Equivalent(evidence.originalAspect,camera.aspect);evidence.autoEquivalentPolicyRestored=camera.targetTexture==null&&AutoEquivalent(camera.aspect,p.width,p.height);evidence.restored=evidence.autoEquivalentPolicyRestored&&(!evidence.samePixelSurface||evidence.numericValueRestored);if(!evidence.restored){evidence.failure="Core world auto-equivalent aspect policy restoration failed";throw new InvalidOperationException(evidence.failure);}}
 }
}

