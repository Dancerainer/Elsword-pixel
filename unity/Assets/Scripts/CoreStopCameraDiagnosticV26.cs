using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel {
 // Value-only failure evidence. No Unity objects or model actor references are
 // retained, and no camera/model state is changed by collecting this snapshot.
 [Serializable]public sealed class CoreStopCameraDiagnosticV26 {
  public CoreStopWorldAspectEvidenceV28 worldAspectV28;public string stage,exceptionType,exceptionMessage,exceptionStack,actualWorld,expectedWorld;public int frame,screenWidth,screenHeight,room,encounter,sourceRenderedFrame,rendererCount;public long ticket,generation;
  public bool coreSession,previewActive,nativeWorldActive,disposed,contextPresent,worldPresent,nativePresent,screenPresent,targetPresent,updateNativeBound,safeViewportBound,restoreFollowBound,readFollowBound,targetCreated,safeViewportEvaluated;
  public CoreIntroCameraStateV25 world,native;public Rect worldPixelRect,nativePixelRect,safeViewport;public string safeViewportReadError,targetFilter;public int targetID,targetWidth,targetHeight;public float sourceUnit,expectedWorldAspect=480f/270,expectedWorldHalf=135,expectedNativeHalf;public Vector3 screenScale,screenPosition;public Quaternion screenRotation,worldRotation;public Bounds bodyBounds;public string[] invalidPredicates;
  public static CoreStopCameraDiagnosticV26 Read(CoreStopCameraContextV21 c,string stage,bool disposed=false){
   var d=new CoreStopCameraDiagnosticV26{stage=stage,frame=Time.frameCount,screenWidth=Screen.width,screenHeight=Screen.height,disposed=disposed,contextPresent=c!=null};var bad=new List<string>();if(disposed)bad.Add("scope-not-disposed");if(c==null){bad.Add("context-present");d.invalidPredicates=bad.ToArray();return d;}
   d.worldAspectV28=c.worldAspectV28;d.coreSession=c.coreSessionV26;d.previewActive=c.previewActiveV26;d.nativeWorldActive=c.nativeWorldActiveV26;d.actualWorld=c.actualWorldV26;d.expectedWorld=c.expectedWorldV26;d.room=c.roomV26;d.encounter=c.encounterV26;
   d.worldPresent=c.world;d.nativePresent=c.native;d.screenPresent=c.screen;d.targetPresent=c.target;d.updateNativeBound=c.updateNativeCamera!=null;d.safeViewportBound=c.safeViewport!=null;d.restoreFollowBound=c.restoreFollow!=null;d.readFollowBound=c.readFollowState!=null;d.sourceUnit=c.sourceUnit;d.expectedNativeHalf=c.sourceUnit>0?135/c.sourceUnit:0;
   if(!c.world)bad.Add("world-present");if(!c.native)bad.Add("native-present");if(!c.screen)bad.Add("screen-present");if(!c.target)bad.Add("target-present");if(!d.updateNativeBound)bad.Add("update-native-bound");if(!d.safeViewportBound)bad.Add("safe-viewport-bound");if(!d.restoreFollowBound)bad.Add("restore-follow-bound");if(!(c.sourceUnit>0))bad.Add("source-unit-positive");
   if(c.world){d.world=CoreIntroCameraStateV25.Read(c.world);d.worldPixelRect=c.world.pixelRect;d.worldRotation=c.world.transform.rotation;if(!c.world.orthographic)bad.Add("world-orthographic");if(!(Mathf.Abs(c.world.orthographicSize-135)<.0001f))bad.Add("world-half135");if(!(Mathf.Abs(c.world.aspect-480f/270)<.001f))bad.Add("world-aspect-near16x9");if(!(Quaternion.Angle(c.world.transform.rotation,Quaternion.identity)<.001f))bad.Add("world-rotation-identity");}
   if(c.native){d.native=CoreIntroCameraStateV25.Read(c.native);d.nativePixelRect=c.native.pixelRect;if(!c.native.orthographic)bad.Add("native-orthographic");if(!(Mathf.Abs(c.native.orthographicSize-135/c.sourceUnit)<.001f))bad.Add("native-half-source-unit");if(!(Mathf.Abs(c.native.aspect-480f/270)<.0001f))bad.Add("native-aspect16x9");if(c.target!=c.native.targetTexture)bad.Add("native-target-reference");}
   if(c.target){d.targetID=c.target.GetInstanceID();d.targetWidth=c.target.width;d.targetHeight=c.target.height;d.targetFilter=c.target.filterMode.ToString();d.targetCreated=c.target.IsCreated();if(c.target.width!=480)bad.Add("target-width480");if(c.target.height!=270)bad.Add("target-height270");if(c.target.filterMode!=FilterMode.Point)bad.Add("target-point");}
   if(c.screen){d.screenScale=c.screen.localScale;d.screenPosition=c.screen.localPosition;d.screenRotation=c.screen.localRotation;if(c.screen.localScale!=new Vector3(480,270,1))bad.Add("screen-scale480x270");if(!(Quaternion.Angle(c.screen.localRotation,Quaternion.identity)<.001f))bad.Add("screen-rotation-identity");}
   if(c.safeViewport!=null){try{d.safeViewport=c.safeViewport();d.safeViewportEvaluated=true;}catch(Exception ex){d.safeViewportReadError=ex.ToString();}}
   d.invalidPredicates=bad.ToArray();return d;
  }
 }
 public sealed class CoreStopCameraReadExceptionV26:InvalidOperationException {public readonly CoreStopCameraContextV21 Context;public CoreStopCameraReadExceptionV26(string message,CoreStopCameraContextV21 context):base(message){Context=context;}}
}

