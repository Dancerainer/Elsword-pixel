using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ElswordPixel { public sealed partial class DemoScene {
 sealed partial class BattleProductViewR1325 {

  // Captures the owned model. Source rendering never mutates or disposes Main's shared visual owner.
  readonly DemoScene host;readonly GameModel model;readonly Transform root;
  public BattleProductViewR1325(DemoScene owner,GameModel ownedModel,Transform ownedRoot){if(owner==null||ownedModel==null||!ownedRoot||(ownedRoot.lossyScale-Vector3.one).sqrMagnitude>1e-8f)throw new ArgumentException("Battle owned model/unit presentation root required");host=owner;model=ownedModel;root=ownedRoot;}
  GameModel game {get{return model;}}Transform worldRoot {get{return host.worldRoot;}}Transform effectsRoot {get{return root;}}
  Transform nativeRoot {get{return host.nativeRoot;}}Camera nativeCamera {get{return host.nativeCamera;}}Camera worldCamera {get{return host.worldCamera;}}
  GameObject nativeScreen {get{return host.nativeScreen;}}NativeScene nativeScene {get{return host.nativeScene;}}bool NativeWorldActive {get{return host.NativeWorldActive;}}
  Dictionary<Transform,float> npcEffectDepths {get{return host.npcEffectDepths;}}
  Dictionary<NpcEffectInstance,NpcEffectSnapshot> previousNpcEffects {get{return host.previousNpcEffects;}}
  bool calibrationRun {get{return host.calibrationRun;}}int previousRoom {get{return host.previousRoom;}}float RenderAlpha {get{return host.RenderAlpha;}}
  float cameraFollowY {get{return host.cameraFollowY;}set{host.cameraFollowY=value;}}float cameraFollowVelocity {get{return host.cameraFollowVelocity;}set{host.cameraFollowVelocity=value;}}int cameraFollowRoom {get{return host.cameraFollowRoom;}set{host.cameraFollowRoom=value;}}
  static bool SteamFinite(float f){return !float.IsNaN(f)&&!float.IsInfinity(f);}static bool SteamVector(float[] a,int n){return a!=null&&a.Length==n&&Array.TrueForAll(a,SteamFinite);}
  const string NightSourceVisualSchemaV19="besma-lake-night-source-visual-v19";
  static Vector3 NativeV(float[] a){return DemoScene.NativeV(a);}
  static void Destroy(UnityEngine.Object value){BelowVisualDestroyV21(value);}static void DestroyImmediate(UnityEngine.Object value){BelowVisualDestroyV21(value);}
  SpriteRenderer WorldSprite(string name,Transform parent,Sprite sprite,int order,Vector3 position){return host.WorldSprite(name,parent,sprite,order,position);}
  void RemoveSceneObject(GameObject value){DemoScene.RemoveSceneObject(value);}
  internal void Configure(BattleProductDataPlanR1282 plan,IDictionary<string,Texture2D> legacy,IDictionary<string,Texture2D> state,IDictionary<string,Texture2D> director){
   ConfigureOutskirtsNpcVisual(plan.LegacyEffects,plan.LegacyMeshes,legacy);
   ConfigureAlteraAirshipStateVisualV20(plan.StateDocument,plan.StatePayload,state);
   var paths=new NativePath[plan.DirectorRoute.path.Length];for(int i=0;i<paths.Length;i++){var p=plan.DirectorRoute.path[i];paths[i]=new NativePath{s=p.s,length=p.length,start=p.start,end=p.end};}
   ConfigureAlteraAirshipDirectorV20(plan.DirectorDocument,plan.DirectorMatrices,plan.DirectorGeometry,plan.DirectorCamera,paths,director);
  }
  internal void Render(){RenderOutskirtsNpcVisual();RenderAlteraAirshipStateVisualV20();RenderLakeNpcVisual();}
  internal bool RenderDirectorCamera(){return TryRenderAlteraAirshipDirectorCameraV20();}
  internal bool ApplyDirectorCamera(bool render){return TryApplyAlteraAirshipDirectorNativeCameraV20(render);}
  internal void SuspendNativeWorld(){SuspendAlteraAirshipDirectorNativeWorldV20();}
  internal void Dispose(){var errors=new List<Exception>();BattleProductCleanupR1325(()=>DisposeAlteraAirshipDirectorV20(),errors);BattleProductCleanupR1325(()=>DisposeOutskirtsNpcVisual(),errors);if(errors.Count>0)throw new AggregateException("Battle owned source visual cleanup failed",errors);}

 }
 }
}
