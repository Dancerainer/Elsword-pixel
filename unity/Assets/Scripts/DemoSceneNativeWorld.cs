using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] class NativeWorldPart {public string name,texture;public int offset,count,blend;public HamelTrap trap;public float[] color,fogColor;public float fogNear,fogFar,fogHeightStart,fogHeightEnd,fogBias;}
  [Serializable] class NativePath {public float s,length;public float[] start,end;}
  [Serializable] class NativeScene {public string id;public float unit,offsetX,offsetY;public NativeWorldPart[] parts;public NativePath[] path;public HamelBubble[] bubbles;public string triggerBytes;}
  NativeScene nativeScene;Transform nativeRoot;Camera nativeCamera;RenderTexture nativeTarget;GameObject nativeScreen;
  readonly List<UnityEngine.Object> nativeObjects=new List<UnityEngine.Object>();
  struct ProjectionRestore {public Renderer renderer;public Transform transform;public Vector3 local,world;}
  readonly List<ProjectionRestore> projectionRestore=new List<ProjectionRestore>(256);
  readonly List<Renderer> projectionCandidates=new List<Renderer>(256);
  readonly Dictionary<Transform,float> projectionActorDepth=new Dictionary<Transform,float>();
  readonly HashSet<Transform> hiddenActorIndicators=new HashSet<Transform>();
  bool NativeWorldActive {get{return nativeCamera&&nativeScene!=null&&!calibrationRun;}}
  static Vector3 NativeV(float[] a){return new Vector3(a[0],a[1],a[2]);}
  static Color NativeColor(float[] a){return a!=null&&a.Length>=4?new Color(a[0],a[1],a[2],a[3]):Color.white;}
  void RestoreNativeProjection(){ResetNativeSceneOcclusionBindingsR1461();foreach(var p in projectionRestore)if(p.transform)p.transform.localPosition=p.local;projectionRestore.Clear();}
  static void DestroyNativeWorldObject(UnityEngine.Object target){if(!target)return;if(Application.isPlaying)Destroy(target);else DestroyImmediate(target);}
  partial void RestoreBelowFortCameraV21();
  partial void AdjustBelowFortProjectionV21(ref float width,ref float height);
  partial void ApplyCoreProductCameraPolicyR1149();
  void DisposeNativeWorld(){
   DisposeSceneryPlate(); ClearHamelTriggers();
   DisposeSwampMovieV39();
   DisposeGiantWilliamV39();
   DisposeMagicianSkinsV38();
   ClearCargoProjectionCameraR1479();
   try{DisposePlainMineMajorViewR1302();}finally{
   DisposeTreeKnightA1ViewsV30();DisposeChickenA1ViewsV31();DisposeBatViewsV34();DisposeAncientViewsV35();DisposeMouseViewsV35();DisposePlainPartViewsV35();DisposeFairyAuthoredViewsV33();DisposeSheriffAuthoredViewsV32();DisposeGuardViewsV35();DisposeCoreElitePartsV36();DisposeHealerBladerViewsV35();DisposeFactoryRavenViewsV35();DisposeCannonViewsV35();DisposeMachineViewsV35();DisposeFemaleNativeDisplayV28();
   #if UNITY_EDITOR
   ReleaseCoreStopCameraForWorldChangeDraftV21();
#else
   if(coreProductSessionR1147){ReleaseCoreStopCameraForWorldChangeDraftV21();}
#endif
   RestoreCoreArenaCameraV21();
   ResetCameraGeometryV25();
   RestoreBelowFortCameraV21();
   SuspendAlteraAirshipDirectorNativeWorldV20();
   ResetDragonNestDirectorWorldV20();
   RestoreNativeProjection();DisposeNativeSceneOcclusionR1461();if(nativeRoot)RemoveSceneObject(nativeRoot.gameObject);if(nativeCamera)RemoveSceneObject(nativeCamera.gameObject);if(nativeScreen)RemoveSceneObject(nativeScreen);
   if(nativeTarget){nativeTarget.Release();DestroyNativeWorldObject(nativeTarget);}foreach(var o in nativeObjects)if(o)DestroyNativeWorldObject(o);nativeObjects.Clear();nativeScene=null;nativeRoot=null;nativeCamera=null;nativeScreen=null;nativeTarget=null;
   }
  }
  void RebuildNativeWorld(){
   DisposeNativeWorld();if(calibrationRun)return;
   if(BattleProductSessionActiveR1325){BuildBattleProductWorldR1325();return;}
   if(CargoProductSessionActiveR1288){BuildCargoProductWorldR1288();return;}
   if(PlainProductSessionActiveR1320){BuildPlainProductWorldR1320();return;}if(Mine2ProductSessionActiveR1233){BuildMine2ProductWorldR1233();return;}
   if(FactoryProductSessionActiveR1179){BuildFactoryProductWorldR1179();return;}
   if(coreProductSessionR1147&&forestPreviewActive){BuildNativeWorld(forestPreviewScenes[game.room],forestPreviewGeometry[game.room],ForestPreviewTexture,forestPreviewStages[game.room].id);return;}
   if(StageIndex>=catalog.stages.Length)return;
   if(belowPlayerSessionV21){BuildBelowPlayerWorldV21();return;}
   var art=CurrentStageArt;if(string.IsNullOrEmpty(art.nativeScene))return;
#if UNITY_EDITOR
   if(forestPreviewActive){BuildNativeWorld(forestPreviewScenes[game.room],forestPreviewGeometry[game.room],ForestPreviewTexture,art.id);return;}
#endif
   var warm=TakeMobileRoomWarmup(art);
   try{
    var scene=warm?.scene;var bytes=warm?.geometry;
    if(scene==null||bytes==null){
     var json=Resources.Load<TextAsset>(art.nativeScene);var binary=Resources.Load<TextAsset>(art.nativeGeometry);
     if(!json||!binary)throw new InvalidOperationException("Missing original world geometry: "+art.id);
     scene=JsonUtility.FromJson<NativeScene>(json.text);bytes=binary.bytes;
    }
    BuildNativeWorld(scene,bytes,path=>warm!=null&&warm.textures.TryGetValue(path,out var texture)?texture:Resources.Load<Texture2D>(path),art.id,warm);
   }finally{warm?.Release();}
   StartNextMobileRoomWarmup(art);
  }
  MeshRenderer CreateNativeRoomRenderer(NativeScene scene,NativeWorldPart part,int index,Mesh mesh,Shader shader,Texture2D texture,Transform parent){
   var go=new GameObject(part.name);go.layer=28;go.transform.SetParent(parent,false);
   go.AddComponent<MeshFilter>().sharedMesh=mesh;
   var mat=new Material(part.blend==0?shader:HamelBackdropShader(part.blend));
   var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=mat;
   mat.SetColor("_Color",NativeColor(part.color));mat.SetColor("_FogColor",NativeColor(part.fogColor));
   mat.SetFloat("_FogNear",part.fogNear);mat.SetFloat("_FogFar",part.fogFar);
   mat.SetFloat("_FogHeightStart",part.fogHeightStart);mat.SetFloat("_FogHeightEnd",part.fogHeightEnd);mat.SetFloat("_FogBias",part.fogBias);
   BindNativeRoomTexture(mat,scene,part,index,texture);
   return renderer;
  }
  static void BindNativeRoomTexture(Material mat,NativeScene scene,NativeWorldPart part,int index,Texture2D texture){
   if(texture){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Repeat;mat.mainTexture=texture;}
   SceneBackgroundStyleV25.Configure(mat,part.name,part.texture,texture);
   SceneBackdropPolicyV25.Configure(mat,scene.id,index,part.name,part.texture);
  }
  void BuildNativeWorld(NativeScene scene,byte[] bytes,Func<string,Texture2D> textureLoader,string id,MobileRoomWarmup warm=null){
   nativeScene=scene;var shader=Resources.Load<Shader>("Pixel/Shaders/NativeWorld");if(!shader)throw new InvalidOperationException("Missing NativeWorld shader");
   nativeRoot=new GameObject("2016 NA original world / "+id).transform;
   bool caveShells=scene.id!=null&&scene.id.StartsWith("WI_ELDER_BENDERS",StringComparison.Ordinal);
   int nativePartIndex=0;
   foreach(var p in nativeScene.parts){
    Mesh mesh;Vector3[] v;
    if(warm==null||!warm.TakeMesh(nativePartIndex,out mesh,out v)){
     v=new Vector3[p.count];var uv=new Vector2[p.count];var ix=new int[p.count];
     DecodeNativeVertices(p,bytes,v,uv,ix,0,p.count);mesh=MakeNativePartMesh(p,v,uv,ix);
     if(FactoryProductSessionActiveR1179)RegisterFactoryBeltMesh(p.name,mesh,uv);
    }
    nativeObjects.Add(mesh);
    MeshRenderer sceneRenderer=warm?.renderers?[nativePartIndex];
    if(sceneRenderer){
     warm.renderers[nativePartIndex]=null;sceneRenderer.transform.SetParent(nativeRoot,false);
     if(!string.IsNullOrEmpty(p.texture)&&!warm.textures.ContainsKey(p.texture))
      BindNativeRoomTexture(sceneRenderer.sharedMaterial,scene,p,nativePartIndex,textureLoader(p.texture));
    }else{
     var texture=string.IsNullOrEmpty(p.texture)?null:textureLoader(p.texture);
     sceneRenderer=CreateNativeRoomRenderer(scene,p,nativePartIndex,mesh,shader,texture,nativeRoot);
    }
    var go=sceneRenderer.gameObject;var mat=sceneRenderer.sharedMaterial;nativeObjects.Add(mat);
    RegisterNativeSceneOccluderR1461(sceneRenderer,v,warm?.walkingFaces?[nativePartIndex]);nativePartIndex++;
    // This closed tunnel shell surrounds the playable lane. Its exterior
    // face hides the actors and the interior walls at the boss-room bend.
    if(scene.id=="WI_ELDER_BELOW_PASSAGE_STAGE4" && p.name=="Elder_Below_Passage_stage4_group2.Y" &&
       (p.texture=="Pixel/native-world/waterway/textures/2ff10765505d44009d02" ||
        p.texture=="Pixel/native-world/waterway/textures/0c19edbefef02caf7406"))
     mat.SetFloat("_Cull",(float)UnityEngine.Rendering.CullMode.Front);
    if(caveShells){
     // The exporter groups closed room shells and playable cliffs under the
     // same Ground object. Only the two open rock strips in room 2 need both
     // faces as the camera crosses their bends.
     bool openCliff=scene.id=="WI_ELDER_BENDERS_CAVE_STAGE1_1"&&
      (p.name=="Benders_Cave_Stage1-1_Ground1.Y"||p.name=="Benders_Cave_Stage1-1_Ground2.Y")&&
      p.texture=="Pixel/native-world/cave/textures/eccd6c6cb74644a676a7";
     mat.SetFloat("_Cull",(float)(openCliff?UnityEngine.Rendering.CullMode.Off:UnityEngine.Rendering.CullMode.Front));
    }
    if(!string.IsNullOrEmpty(p.texture)&&IsPeitaInteriorWall(p))
     mat.SetFloat("_Cull",(float)UnityEngine.Rendering.CullMode.Front);
    // The outer side of this courtyard wall sits in front of the descent.
    // Keep its inner face when the route looks across the courtyard.
    if(scene.id=="WI_ELDER_WALLY_CASTLE_CENTER_STAGE3"&&
       (p.name=="Wally_Castle_Center_Stage3_Wall03.Y"||p.name=="Wally_Castle_Center_Stage3_Wall03_BTWall01.Y"))
     mat.SetFloat("_Cull",(float)UnityEngine.Rendering.CullMode.Front);
    RegisterCameraGeometryV25(go.transform,v,mat,p.name,warm?.cameraBlocks?[nativePartIndex-1]);
   }
   // The original Stage1 sky object uses XL_SKY_WORLD_BASE_0. The source
   // geometry adapter otherwise flattens it into a double-sided depth mesh:
   // its near outer wall then occludes the actual terrain from the route
   // camera. Keep only this exact sky object's inward-facing surface.
   if(scene.id=="WI_BESMA_DRAGON_NEST_STAGE1")
    foreach(var renderer in nativeRoot.GetComponentsInChildren<MeshRenderer>())
     if(renderer.gameObject.name=="Besma_Dragon_Nest_Stage1_Sky.Y"){
      if(renderer.GetComponent<MeshFilter>().sharedMesh.vertexCount!=36)throw new InvalidOperationException("Dragon Nest original sky mesh changed");
      renderer.sharedMaterial.SetFloat("_Cull",(float)UnityEngine.Rendering.CullMode.Front);
     }
   ApplyAlteraNativePresentationV17(scene); BuildHamelTriggers(scene); BuildHamelBubbleVents(scene);
   var cameraObject=new GameObject("Native route camera");nativeCamera=cameraObject.AddComponent<Camera>();nativeCamera.enabled=false;nativeCamera.orthographic=true;nativeCamera.orthographicSize=CameraLogicalHeightV25*.5f/nativeScene.unit;nativeCamera.aspect=displayWorldAspect;nativeCamera.cullingMask=1<<28;nativeCamera.nearClipPlane=1;nativeCamera.farClipPlane=100000;nativeCamera.clearFlags=CameraClearFlags.SolidColor;nativeCamera.backgroundColor=SceneBackdropPolicyV25.ClearColor(scene.id,new Color(.67f,.87f,.94f,0));nativeCamera.allowHDR=false;nativeCamera.allowMSAA=false;
   nativeTarget=new RenderTexture(CameraNativePixelWidthV25,CameraNativePixelHeightV25,24,RenderTextureFormat.ARGB32){name="Native scenery at current framing pixel resolution",filterMode=FilterMode.Point};nativeTarget.Create();nativeCamera.targetTexture=nativeTarget;RegisterNativePlayerRevealCameraR1478();worldCamera.cullingMask&=~(1<<28);
   nativeScreen=new GameObject("Native scenery pixel screen");nativeScreen.transform.SetParent(worldRoot,false);var quad=new Mesh{name="Pixel screen quad"};quad.vertices=new[]{new Vector3(-.5f,-.5f,0),new Vector3(-.5f,.5f,0),new Vector3(.5f,.5f,0),new Vector3(.5f,-.5f,0)};quad.uv=new[]{new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};quad.triangles=new[]{0,1,2,0,2,3};quad.RecalculateBounds();nativeObjects.Add(quad);nativeScreen.AddComponent<MeshFilter>().sharedMesh=quad;var screenMaterial=CreateNativeBackdropMaterialV25();screenMaterial.mainTexture=nativeTarget;nativeObjects.Add(screenMaterial);nativeScreen.AddComponent<MeshRenderer>().sharedMaterial=screenMaterial;nativeScreen.GetComponent<MeshRenderer>().sortingOrder=-100;nativeScreen.transform.localScale=new Vector3(CameraLogicalHeightV25*displayWorldAspect,CameraLogicalHeightV25,1);
   stagesRoot.gameObject.SetActive(false);
  }
  NativePath[] indexedPath;
  bool orderedPath;
  int lastSegment;

  int SegmentAt(NativePath[] path,float station){
   if(!ReferenceEquals(path,indexedPath)){
    indexedPath=path;orderedPath=true;lastSegment=0;
    for(int i=0;i<path.Length;i++){
     if(float.IsNaN(path[i].s+path[i].length)||
        (i>0&&path[i].s+path[i].length<path[i-1].s+path[i-1].length))orderedPath=false;
    }
   }
   int last=path.Length-1;
   if(!orderedPath){
    for(int i=0;i<last;i++)if(station<=path[i].s+path[i].length)return i;
    return last;
   }
   if(!(station<=path[last].s+path[last].length))return last;
   if(station<=path[lastSegment].s+path[lastSegment].length&&
      (lastSegment==0||station>path[lastSegment-1].s+path[lastSegment-1].length))return lastSegment;
   int low=0,high=last;
   while(low<high){int middle=low+(high-low)/2;if(station<=path[middle].s+path[middle].length)high=middle;else low=middle+1;}
   lastSegment=low;return low;
  }
  Vector3 NativePathPoint(float worldX){
   float station=(worldX-nativeScene.offsetX)/nativeScene.unit;
   var segment=nativeScene.path[SegmentAt(nativeScene.path,station)];
   float t=Mathf.Clamp01((station-segment.s)/Mathf.Max(.001f,segment.length));
   return Vector3.Lerp(NativeV(segment.start),NativeV(segment.end),t);
  }
  Vector3 NativeNormal(float x){Vector3 linked;if(TryCargoProjectionNativeNormalR1479(x,out linked))return linked;Vector3 tangent=NativePathPoint(x+35)-NativePathPoint(x-35);tangent.y=0;if(tangent.sqrMagnitude<.0001f)tangent=Vector3.right;tangent.Normalize();return new Vector3(tangent.z,0,-tangent.x);}
  Vector3 NativeViewport(Vector3 p,float depth){
   var point=NativePathPoint(p.x);
   if(depth!=0)point+=NativeNormal(p.x)*depth;
   point.y=(p.y-nativeScene.offsetY)/nativeScene.unit;
   return nativeCamera.WorldToViewportPoint(point);
  }
  Vector3 ProjectNativePoint(Vector3 p,float depth=0){return ProjectNativePointWithEye(p,depth,out _);}
  Vector3 ProjectNativePointWithEye(Vector3 p,float depth,out float eye){
   if(!NativeWorldActive){eye=0;return p;}
   var uv=NativeViewport(p,depth);eye=uv.z;var c=worldCamera.transform.position;float width=480,height=270;AdjustBelowFortProjectionV21(ref width,ref height);CameraProjectionDimensionsV25(ref width,ref height);
#if UNITY_EDITOR
   AdjustCoreArenaProjectionV21(ref width,ref height);AdjustCoreStopWholeProjectionV22(ref width,ref height);
#else
   if(coreProductSessionR1147){AdjustCoreArenaProjectionV21(ref width,ref height);AdjustCoreStopWholeProjectionV22(ref width,ref height);}
#endif
if(factoryRavenCameraV21!=null && factoryRavenCameraV21.WholeSpriteScopeV22){
    height=factoryRavenCameraV21.WholeProjectionHeightV22;width=height*480f/270;
   }
   if(displayWorldAspect!=CameraFramingPolicyV25.Aspect)width=height*displayWorldAspect;
   return new Vector3(c.x+(uv.x-.5f)*width,c.y+(uv.y-.5f)*height,p.z);
  }
  void ApplyPendingScenerySize(){
   if(mobileSceneryTargetDirty&&nativeTarget){ResizeSceneryTargetV25();mobileSceneryTargetDirty=false;}
  }
  bool CastleCornerProjection {
   get{return nativeScene!=null&&nativeScene.id=="WI_ELDER_WALLY_CASTLE_CENTER_STAGE3"&&nativeCamera.orthographic;}
  }
  void SetCastleCornerProjection(){
   // The folded courtyard uses the opposite wall to complete its painted backdrop.
   float halfHeight=nativeCamera.orthographicSize,halfWidth=halfHeight*nativeCamera.aspect;
   const float wallDepthBehindEye=6000;
   nativeCamera.projectionMatrix=Matrix4x4.Ortho(-halfWidth,halfWidth,-halfHeight,halfHeight,nativeCamera.nearClipPlane-wallDepthBehindEye,nativeCamera.farClipPlane);
  }
  float CastleWalkwayCameraPitch(float station){
   const float downwardSlope=.1763f; // tan(10°)
   switch(nativeScene.id){
    case "WI_ELDER_WALLY_CASTLE_CENTER_STAGE3":
     // Show the switchback tread without pulling the upper gallery across
     // the actor as the camera looks through the first down slope.
     return .0524f*Mathf.SmoothStep(0,1,Mathf.InverseLerp(5292,5680,station));
    case "WI_ELDER_WALLY_CASTLE_CENTER_STAGE4":
     return downwardSlope;
    default:
     return 0;
   }
  }
  void UpdateNativeCamera(bool render=false){
   if(render){UpdateHamelTriggers();ReadSceneryRenderMode();plateDepthReady=false;ApplyPendingScenerySize();}
   NativeClearCoverageV25();
#if UNITY_EDITOR
   if(ApplyCoreArenaPreparedNativeV21(render)){if(render)plateIsCurrent=false;UpdateNativeBackdropV25();return;}
#else
   if(coreProductSessionR1147&&ApplyCoreArenaPreparedNativeV21(render)){if(render)plateIsCurrent=false;UpdateNativeBackdropV25();return;}
#endif
   if(TryUpdateFramedDirectorCameraV25(render)){if(render){plateIsCurrent=false;TraceScenery(1024,"original-director");}UpdateNativeBackdropV25();return;}
   if(!NativeWorldActive)return;
   SyncNativeCameraFramingV25();
   var c=worldCamera.transform.position;
   var at=NativePathPoint(c.x);at.y=(c.y-nativeScene.offsetY)/nativeScene.unit;
   var normal=NativeNormal(c.x);
   ReconcileCargoProjectionNativeCameraR1479(ref at,ref normal);
   float pitch=CastleWalkwayCameraPitch(c.x);
   if(pitch>0)normal=(normal+Vector3.up*pitch).normalized;
   nativeCamera.transform.SetPositionAndRotation(at+normal*2200,Quaternion.LookRotation(-normal,Vector3.up));
   if(CastleCornerProjection)SetCastleCornerProjection();
   nativeScreen.transform.localPosition=new Vector3(c.x,c.y,10);UpdateNativeBackdropV25();RenderPlainMineMajorViewR1302();if(render&&!TryDrawSceneryPlate()){plateIsCurrent=false;nativeCamera.Render();}
  }
  void FinishNativeProjection(){
   // Apply a menu change when drawing resumes, before actors validate their camera target.
   ApplyPendingScenerySize();
   SyncCameraOverlayV25();UpdateFemaleNativeDisplayV28();if(!NativeWorldActive){FinishEnemyInitialProjectionV25();PrepareMonsterPixelOwnersV53();UpdateSheriffAuthoredViewsV32();TryUpdateAncientSkinV37();UpdateGuardViewsV35();UpdateCoreElitePartsV36();UpdateBendersViewsV36();UpdateReusablePaintV37();UpdateBatViewsV34();UpdateMouseViewsV35();UpdatePlainPartViewsV35();UpdateHealerBladerViewsV35();UpdateFactoryRavenViewsV35();UpdateCannonViewsV35();UpdateMachineViewsV35();if(!OwnedSourceFramingR1111){UpdateTreeKnightA1ViewsV30();UpdateChickenA1ViewsV31();UpdateFairyAuthoredViewsV33();}UpdateMonsterPixelViewsV53();return;}
   PrepareMonsterPixelDirectorFramesV53();
#if UNITY_EDITOR
   PrepareCoreStopWholeCurrentFrameV22();
#else
   if(coreProductSessionR1147){PrepareCoreStopWholeCurrentFrameV22();}
#endif
   ApplyCoreProductCameraPolicyR1149();
   ApplyFactoryRavenIntroCameraV21();
   UpdateNativeCamera(true);CaptureNativeSceneEyeDepthR1461();
   // Snapshot all positions before changing parents. Restoring local positions
   // at the next Update keeps simulation and pooled effect transforms untouched.
   projectionActorDepth.Clear();
   hiddenActorIndicators.Clear();
   bool skipEmptyIndicators=StreamMobileArt;
   for(int i=0;i<enemyViews.Count;i++){
    float depth=game.enemies[i].nativeDepth;
    var bar=enemyBars[i];var warning=warnings[i];var status=enemyDebuffViews[i];
    projectionActorDepth[enemyViews[i].transform]=depth;
    projectionActorDepth[bar.transform]=depth;
    projectionActorDepth[warning.transform]=depth;
    projectionActorDepth[status.transform]=depth;
    var armorIcon=armorBreakIcons[i];
    if(armorIcon&&armorIcon.gameObject.activeSelf)projectionActorDepth[armorIcon.transform]=depth;
    if(!skipEmptyIndicators)continue;
    if(!bar.enabled&&bar.transform.childCount==0)hiddenActorIndicators.Add(bar.transform);
    if(!warning.enabled&&warning.transform.childCount==0)hiddenActorIndicators.Add(warning.transform);
    if(string.IsNullOrEmpty(status.text)&&status.transform.childCount==0)hiddenActorIndicators.Add(status.transform);
   }
   worldRoot.GetComponentsInChildren(false,projectionCandidates);
   foreach(var renderer in projectionCandidates){
    var t=renderer.transform;
    if(hiddenActorIndicators.Contains(t))continue;
    if(MonsterPixelOwnsRendererV53(renderer)||PlainPartOwnsRendererV35(renderer)||MachineOwnsRendererV35(renderer)||CannonOwnsRendererV35(renderer)||FactoryRavenOwnsRendererV35(renderer)||HealerBladerOwnsRendererV35(renderer)||MouseOwnsRendererV35(renderer)||AncientOwnsRendererV35(renderer)||BendersOwnsRendererV36(renderer)||ReusablePaintOwnsV37(renderer)||BatOwnsRendererV34(renderer)||TreeKnightA1OwnsRendererV30(renderer)||ChickenA1OwnsRendererV31(renderer)||FairyAuthoredOwnsRendererV33(renderer)||SheriffAuthoredOwnsRendererV32(renderer)||GuardOwnsRendererV35(renderer)||CoreEliteOwnsRendererV36(renderer))continue;
    if(renderer is LineRenderer||renderer==swordRibbonRenderer||renderer==sceneDim||renderer.transform.IsChildOf(stagesRoot)||renderer.gameObject==nativeScreen)continue;
    projectionRestore.Add(new ProjectionRestore{renderer=renderer,transform=t,local=t.localPosition,world=t.position});
   }
   foreach(var p in projectionRestore){
    if(!npcEffectDepths.TryGetValue(p.transform,out var depth))projectionActorDepth.TryGetValue(p.transform,out depth);
    var projected=ProjectNativePointWithEye(p.world,depth,out float eye);
    BindProjectedSceneOcclusion(p.renderer,p.world,depth,eye);
    p.transform.position=projected;
   }
   FinishEnemyInitialProjectionV25();PrepareMonsterPixelOwnersV53();UpdateSheriffAuthoredViewsV32();TryUpdateAncientSkinV37();UpdateGuardViewsV35();UpdateCoreElitePartsV36();UpdateBendersViewsV36();UpdateReusablePaintV37();UpdateBatViewsV34();UpdateMouseViewsV35();UpdatePlainPartViewsV35();UpdateHealerBladerViewsV35();UpdateFactoryRavenViewsV35();UpdateCannonViewsV35();UpdateMachineViewsV35();if(!OwnedSourceFramingR1111){UpdateTreeKnightA1ViewsV30();UpdateChickenA1ViewsV31();UpdateFairyAuthoredViewsV33();}UpdateMonsterPixelViewsV53();
#if UNITY_EDITOR
   ProjectFemalePixelV53();ObserveCoreStopAfterNativeProjectionDraftV21();
#else
   ProjectFemalePixelV53();
   if(coreProductSessionR1147){ObserveCoreStopAfterNativeProjectionDraftV21();}
#endif
  }
 }
}


