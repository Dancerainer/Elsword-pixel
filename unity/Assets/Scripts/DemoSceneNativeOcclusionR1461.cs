using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  // Presentation only: reuse the already loaded scene meshes and unchanged
  // 2D billboard positions. No terrain, actor state or visibility assignment.
  RenderTexture nativeSceneEyeDepthR1461;
  CommandBuffer nativeSceneDepthCommandsR1461;
  Shader nativeSceneDepthShaderR1461,nativeDefaultOcclusionShaderR1461,nativeTextOcclusionShaderR1461;
  bool nativeOcclusionFontEventRegisteredR1461;
  // Registered scene parts own immutable meshes. Cutout properties, transforms
  // and visibility can change: compare those before reusing a depth image.
  sealed class DepthPart {
   public MeshRenderer renderer;public Material source,draw;public Shader shader;public Texture texture;
   public Matrix4x4 transform;public Color color;public float cull,coverage;public uint textureRevision;public bool visible,solid;
   public Vector3[] triangles;public bool[] walkingFaces;public string group;public bool revealsActors;
  }
  readonly List<DepthPart> nativeSceneOccludersR1461=new List<DepthPart>();
  // Native art splits each modeled building across several textures. Qualify the
  // original object by line of sight, then let each drawn fragment test its depth.
  readonly HashSet<string> blockingGroups=new HashSet<string>(StringComparer.Ordinal);
  struct SightLine {public Ray ray; public float length;}
  readonly List<SightLine> actorSightLines=new List<SightLine>();
  static readonly int RevealPart=Shader.PropertyToID("_NativePlayerRevealScenePartR1478");
  readonly Plane[] depthPlanes=new Plane[6];
  Matrix4x4 lastDepthView,lastDepthProjection;
  Vector4 lastDepthRevealArea,lastDepthRevealEye;
  bool hasDepthImage,sceneryDepthBound;
  static readonly int DepthTexture=Shader.PropertyToID("_MainTex"),DepthColor=Shader.PropertyToID("_Color"),
   DepthCull=Shader.PropertyToID("_Cull"),DepthCoverage=Shader.PropertyToID("_BackdropCoverage");
  readonly Dictionary<Material,Material> nativeSceneDepthMaterialsR1461=new Dictionary<Material,Material>();
  readonly Dictionary<Material,Material> nativeDefaultOcclusionMaterialsR1461=new Dictionary<Material,Material>();
  readonly Dictionary<Material,Material> nativeTextOcclusionMaterialsR1461=new Dictionary<Material,Material>();
  struct NativeSceneOcclusionBindingR1461 {public Renderer renderer;public Material original,assigned;}
  readonly List<NativeSceneOcclusionBindingR1461> nativeSceneOcclusionBindingsR1461=new List<NativeSceneOcclusionBindingR1461>();
  MaterialPropertyBlock nativeSceneOcclusionPropertiesR1461;
  bool nativeSceneDepthReadyR1461;
  bool hasBillboardEye;
  Vector3 lastBillboardSource;
  float lastBillboardOffset,lastBillboardEye;
  bool nativePlayerRevealCameraRegisteredR1478;float nativePlayerRevealStrengthR1478;int nativePlayerRevealFrameR1478=-1;

  void ResetNativeSceneOcclusionBindingsR1461(){
   nativeSceneDepthReadyR1461=false;Shader.SetGlobalFloat("_NativeSceneDepthEnabledR1461",0);
   if(nativeSceneOcclusionPropertiesR1461==null)nativeSceneOcclusionPropertiesR1461=new MaterialPropertyBlock();
   foreach(var binding in nativeSceneOcclusionBindingsR1461){
    if(!binding.renderer)continue;
    binding.renderer.GetPropertyBlock(nativeSceneOcclusionPropertiesR1461);nativeSceneOcclusionPropertiesR1461.SetFloat("_NativeSceneClipR1461",0);binding.renderer.SetPropertyBlock(nativeSceneOcclusionPropertiesR1461);
    if(binding.assigned&&binding.renderer.sharedMaterial==binding.assigned)binding.renderer.sharedMaterial=binding.original;
   }
   nativeSceneOcclusionBindingsR1461.Clear();
  }
  void DisposeNativeSceneOcclusionR1461(){
   ReleaseNativePlayerRevealCameraR1478();ResetNativeSceneOcclusionBindingsR1461();Shader.SetGlobalTexture("_NativeSceneEyeDepthR1461",null);
   if(nativeSceneEyeDepthR1461){nativeSceneEyeDepthR1461.Release();DestroyNativeWorldObject(nativeSceneEyeDepthR1461);}nativeSceneEyeDepthR1461=null;
   if(nativeSceneDepthCommandsR1461!=null){nativeSceneDepthCommandsR1461.Release();nativeSceneDepthCommandsR1461=null;}
   foreach(var material in nativeSceneDepthMaterialsR1461.Values)if(material)DestroyNativeWorldObject(material);nativeSceneDepthMaterialsR1461.Clear();
   foreach(var material in nativeDefaultOcclusionMaterialsR1461.Values)if(material)DestroyNativeWorldObject(material);nativeDefaultOcclusionMaterialsR1461.Clear();
   foreach(var material in nativeTextOcclusionMaterialsR1461.Values)if(material)DestroyNativeWorldObject(material);nativeTextOcclusionMaterialsR1461.Clear();
   if(nativeOcclusionFontEventRegisteredR1461){Font.textureRebuilt-=OnNativeOcclusionFontTextureRebuiltR1461;nativeOcclusionFontEventRegisteredR1461=false;}
   nativeSceneOccludersR1461.Clear();hasDepthImage=false;
  }
  // Only BuildNativeWorld original scene.parts register here. Native-root
  // Female geometry, directors and source FX retain their separate ownership.
  void RegisterNativeSceneOccluderR1461(MeshRenderer renderer,Vector3[] triangles,bool[] preparedWalkingFaces=null){
   if(!renderer)throw new ArgumentNullException("renderer");
   var filter=renderer.GetComponent<MeshFilter>();
   if(!filter||!filter.sharedMesh||filter.sharedMesh.subMeshCount!=1||renderer.sharedMaterials.Length!=1)throw new InvalidOperationException("Native scene single-part mesh contract changed.");
   string group=renderer.gameObject.name;
   // The two halves of this entrance wall meet across the hero's shoulders.
   if(group=="Velder_Gate_Stage1_Part06.Y")group="Velder_Gate_Stage1_Part05.Y";
   var part=new DepthPart{renderer=renderer,triangles=triangles,group=group};
   nativeSceneOccludersR1461.Add(part);var material=renderer.sharedMaterial;
   if(material&&material.shader&&NativeSceneSolidOccluderShaderR1461(material.shader)){
    if(!material.HasProperty("_NativePlayerRevealScenePartR1478"))throw new InvalidOperationException("Owned solid scene part lacks local player-reveal shader binding.");
    material.SetFloat(RevealPart,0);
    if(IsRevealArchitecture(part.group))part.walkingFaces=preparedWalkingFaces??ProtectWalkingFaces(filter.sharedMesh,triangles,nativeScene,game.CurrentRoom);
   }
  }
  // Source names end in an object role (e.g. Stage3_House01.Y). A cave
  // shell or platform is often in front of one ray on a multi-level route,
  // but its playable ledges must stay solid at every elevation.
  static bool IsRevealArchitecture(string name){
   if(IsHamelForegroundShell(name))return true;
   switch(name){
    case "velder_bridge_burning_Stage2-1_side_bridge01.Y":
    case "Velder_Gate_Stage1_Part02.Y":
    case "Velder_Gate_Stage1_Part05.Y":
    case "Velder_Gate_Stage1_Part06.Y":return true;
   }
   int start=name.LastIndexOf('_')+1,end=name.IndexOf('.',start);
   if(end<0)end=name.Length;
   while(end>start&&(char.IsDigit(name[end-1])||name[end-1]=='-'))end--;
   string role=name.Substring(start,end-start).ToLowerInvariant();
   switch(role){
    case "house":case "build":case "building":case "castle":case "chapel":case "chaple":case "temple":case "tent":
    case "wall":case "btwall":case "outerwall":case "sidewall":case "backwall":case "stage-wall":
    case "tower":case "sidetower":case "backtower":case "roof":case "arch":case "window":
    case "column":case "pillar":case "frontpillar":case "backpillar":case "alphapillar":case "backcastlepillar":
    case "gidung":case "giddung":case "sidegidung":case "pillerwall":case "backcastle":
    case "door":case "doors":case "doorstart":case "doorend":case "fence":
    case "gate":case "stonegate":case "cantergate":case "gateend":case "gateleft":case "gateright":
    case "gatemiddle":case "gateside":case "gatedoor":case "stage-gate":case "stage-gatebig":
     return true;
    default:return false;
   }
  }
  static bool[] ProtectWalkingFaces(Mesh mesh,Vector3[] triangles,NativeScene scene,RoomDefinition room){
   // The source contains unshared triangle vertices. Mark the tread and its
   // skirt independently even when they share a mesh and texture with a wall.
   var protection=new Vector2[triangles.Length];var walkingFaces=new bool[triangles.Length/3];
   // The Southern Gate entrance has a straight road at z=200. Fallen masonry
   // in front of its near edge includes sloping faces at the height of its tread.
   bool gateRubble=mesh.name=="Velder_Gate_Stage1_Part02.Y"&&room.id=="velder-gate-1";
   for(int i=0;i+2<triangles.Length;i+=3){
    Vector3 a=triangles[i],b=triangles[i+1],c=triangles[i+2],center=(a+b+c)/3;
    if(gateRubble&&Mathf.Max(a.z,Mathf.Max(b.z,c.z))<0)continue;
    Vector3 normal=Vector3.Cross(b-a,c-a);
    float floor=NativeRoadHeightAt(scene.path,center,out var line);
    // Long slope skirts span several elevations. Test their highest corner
    // against the road at that corner, not the elevation at the face center.
    var start=NativeV(line.start);var edge=NativeV(line.end)-start;
    float ta=RoadFraction(a,start,edge),tb=RoadFraction(b,start,edge),tc=RoadFraction(c,start,edge);
    float clearance=Mathf.Max(a.y-ta*edge.y,Mathf.Max(b.y-tb*edge.y,c.y-tc*edge.y))-start.y;
    bool horizontal=normal.y*normal.y>=.5f*normal.sqrMagnitude;
    bool tread=horizontal&&Mathf.Abs(center.y-floor)<=64;
    bool skirt=clearance>=-32 && clearance<=4;
    bool protectedFace=tread||skirt;
    // The camera path stores a single Y at each station. The collision
    // catalog also owns elevated, overlapping floors at that same station.
    if(!protectedFace&&room.surfaces!=null){
     float unit=scene.unit,baseX=scene.offsetX+line.s*unit,length=line.length*unit;
     float xa=baseX+ta*length,xb=baseX+tb*length,xc=baseX+tc*length,x=(xa+xb+xc)/3;
     foreach(var surface in room.surfaces){
      if(surface.npcOnly||x<surface.x1||x>surface.x2)continue;
      if(horizontal&&Mathf.Abs(center.y*unit+scene.offsetY-surface.Y(x))<=64*unit){protectedFace=true;break;}
      float da=a.y*unit-surface.Y(Mathf.Clamp(xa,surface.x1,surface.x2));
      float db=b.y*unit-surface.Y(Mathf.Clamp(xb,surface.x1,surface.x2));
      float dc=c.y*unit-surface.Y(Mathf.Clamp(xc,surface.x1,surface.x2));
      float high=scene.offsetY+Mathf.Max(da,Mathf.Max(db,dc));
      if(high>=-32*unit&&high<=4*unit){protectedFace=true;break;}
     }
    }
    if(protectedFace){walkingFaces[i/3]=true;protection[i]=protection[i+1]=protection[i+2]=Vector2.right;}
   }
   mesh.uv2=protection;return walkingFaces;
  }
  static float RoadFraction(Vector3 vertex,Vector3 start,Vector3 edge){
   return Mathf.Clamp01(((vertex.x-start.x)*edge.x+(vertex.z-start.z)*edge.z)/Mathf.Max(.001f,edge.x*edge.x+edge.z*edge.z));
  }
  static float NativeRoadHeightAt(NativePath[] path,Vector3 point,out NativePath road){
   road=path[0];float nearest=float.PositiveInfinity,height=0;
   foreach(var line in path){
    var a=NativeV(line.start);var edge=NativeV(line.end)-a;
    float dx=point.x-a.x,dz=point.z-a.z;
    float t=Mathf.Clamp01((dx*edge.x+dz*edge.z)/Mathf.Max(.001f,edge.x*edge.x+edge.z*edge.z));
    dx-=t*edge.x;dz-=t*edge.z;float d=dx*dx+dz*dz;
    if(d<nearest){nearest=d;road=line;height=a.y+t*edge.y;}
   }
   return height;
  }
  void AddSightLine(Vector3 sample,Vector4 area){
   var uv=nativeCamera.WorldToViewportPoint(sample);
   if(uv.z<=nativeCamera.nearClipPlane)return;
   float x=(uv.x-area.x)/area.z,y=(uv.y-area.y)/area.w;
   if(x*x+y*y>=1)return;
   var ray=nativeCamera.ViewportPointToRay(uv);
   actorSightLines.Add(new SightLine{ray=ray,length=Vector3.Dot(sample-ray.origin,ray.direction)-.5f});
  }
  void AddActorSightLines(Vector3 feet,float height,Vector4 area){
   AddSightLine(feet+Vector3.up*(height*.24f),area);
   AddSightLine(feet+Vector3.up*(height*.6f),area);
   AddSightLine(feet+Vector3.up*(height*.88f),area);
  }
  bool MeshBlocksSight(DepthPart part){
   var renderer=part.renderer;
   if(part.walkingFaces==null||!renderer.enabled||!renderer.gameObject.activeInHierarchy)return false;
   var mat=renderer.sharedMaterial;
   if(!mat.HasProperty(RevealPart)||mat.GetFloat(DepthCoverage)<=0)return false;
   var bounds=renderer.bounds;
   foreach(var sight in actorSightLines){
    if(!bounds.IntersectRay(sight.ray,out float near)||near>sight.length)continue;
    // Intersect in mesh space. Leave direction unnormalized so t is still
    // measured in world distance under a scaled moving scene transform.
    var inverse=renderer.worldToLocalMatrix;
    var origin=inverse.MultiplyPoint3x4(sight.ray.origin);
    var dir=inverse.MultiplyVector(sight.ray.direction);
    var vertices=part.triangles;
    int cull=(int)mat.GetFloat(DepthCull);
    for(int i=0;i+2<vertices.Length;i+=3){
     if(part.walkingFaces[i/3])continue;
     Vector3 a=vertices[i],edge=vertices[i+1]-a,side=vertices[i+2]-a;
     Vector3 p=Vector3.Cross(dir,side);float det=Vector3.Dot(edge,p);
     if(Mathf.Abs(det)<.00001f||(cull==1&&det>0)||(cull==2&&det<0))continue;
     var offset=origin-a;float u=Vector3.Dot(offset,p)/det;
     if(u<0||u>1)continue;
     var q=Vector3.Cross(offset,edge);float v=Vector3.Dot(dir,q)/det;
     if(v<0||u+v>1)continue;
     float t=Vector3.Dot(side,q)/det;
     if(t>=0&&t<sight.length)return true;
    }
   }
   return false;
  }
  bool SelectBlockingScenery(Vector4 area){
   actorSightLines.Clear();blockingGroups.Clear();
   AddActorSightLines(NativePlayerRevealSourcePointR1478(RenderPlayerPosition),54/nativeScene.unit,area);
   for(int i=0;i<game.enemies.Count;i++){
    var e=game.enemies[i];
    if(!e.active||e.dormant||e.hp<=0||!GameModel.IsCombatMonster(e)||e.regionalAlly||
       (game.HasEncounters&&e.encounterIndex!=game.EncounterIndex)||
       i>=enemyViews.Count||!enemyViews[i].gameObject.activeInHierarchy)continue;
    var pos=RenderEnemyPosition(e);
    Vector3 feet=NativePlayerRevealSourcePointR1478(pos);
    if(e.nativeDepth!=0)feet+=NativeNormal(pos.x)*e.nativeDepth;
    float height=libraries.TryGetValue(e.kind,out var art)&&art.data.standingHeight>0?
     art.data.standingHeight*art.data.logicalScale*e.visualScale:54;
    AddActorSightLines(feet,height/nativeScene.unit,area);
   }
   foreach(var part in nativeSceneOccludersR1461)
    if(!blockingGroups.Contains(part.group)&&MeshBlocksSight(part))blockingGroups.Add(part.group);
   foreach(var part in nativeSceneOccludersR1461){
    bool reveal=blockingGroups.Contains(part.group);
    if(part.revealsActors==reveal)continue;
    part.revealsActors=reveal;
    var mat=part.renderer.sharedMaterial;
    if(mat.HasProperty(RevealPart))mat.SetFloat(RevealPart,reveal?1:0);
    if(part.draw)part.draw.SetFloat(RevealPart,reveal?1:0);
    hasDepthImage=false;plateIsCurrent=false;
   }
   return blockingGroups.Count>0;
  }
  public static bool NativeSceneSolidOccluderShaderR1461(Shader shader){
   if(!shader)throw new ArgumentNullException("shader");
   if(shader.name=="ElswordPixel/WaterwaySourceAlphaV26"||shader.name=="ElswordPixel/SourceFactorOpaqueDestinationV27")return false;
   if(shader.name=="ElswordPixel/NativeWorld"||shader.name=="ElswordPixel/ForestGroundFaceDisplayV28")return true;
   throw new InvalidOperationException("Unreviewed native occluder shader: "+shader.name);
  }
  bool UpdateDepthPart(DepthPart part){
   var renderer=part.renderer;
   bool visible=renderer&&renderer.enabled&&renderer.gameObject.activeInHierarchy&&GeometryUtility.TestPlanesAABB(depthPlanes,renderer.bounds);
   bool changed=false;
   if(visible){
    var source=renderer.sharedMaterial;var shader=source?source.shader:null;
    if(!shader)throw new InvalidOperationException("Native scene part lacks its source material.");
    if(part.source!=source||part.shader!=shader){part.source=source;part.shader=shader;part.solid=NativeSceneSolidOccluderShaderR1461(shader);part.draw=null;changed=true;}
    visible=part.solid&&source.GetFloat(DepthCoverage)>0;
    if(visible){
     var tex=source.GetTexture(DepthTexture);uint revision=tex?tex.updateCount:0;
     var color=source.GetColor(DepthColor);float cull=source.GetFloat(DepthCull),coverage=source.GetFloat(DepthCoverage);
     if(!part.draw||part.texture!=tex||part.textureRevision!=revision||!part.color.Equals(color)||part.cull!=cull||part.coverage!=coverage){
      if(!nativeSceneDepthMaterialsR1461.TryGetValue(source,out part.draw))nativeSceneDepthMaterialsR1461[source]=part.draw=new Material(nativeSceneDepthShaderR1461){name="Native eye depth / "+source.name};
      part.draw.SetFloat(RevealPart,part.revealsActors?1:0);part.draw.SetTexture(DepthTexture,tex);part.draw.SetColor(DepthColor,color);part.draw.SetFloat(DepthCull,cull);part.draw.SetFloat(DepthCoverage,coverage);
      part.color=color;part.texture=tex;part.textureRevision=revision;part.cull=cull;part.coverage=coverage;changed=true;
     }
     var matrix=renderer.localToWorldMatrix;
     if(!part.transform.Equals(matrix)){part.transform=matrix;changed=true;}
    }
   }
   bool visibilityChanged=part.visible!=visible;part.visible=visible;return visibilityChanged||(visible&&changed);
  }
  int lastDepthCropGeneration=-1;
  Vector2Int lastDepthCropOffset;
  void EnsureNativeDepthResources(){
   if(!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat))throw new InvalidOperationException("Native per-pixel occlusion requires an RFloat render target.");
   if(!nativeSceneDepthShaderR1461)nativeSceneDepthShaderR1461=Resources.Load<Shader>("Pixel/Shaders/NativeSceneEyeDepthR1461");
   if(!nativeSceneDepthShaderR1461||!nativeSceneDepthShaderR1461.isSupported)throw new InvalidOperationException("Missing or unsupported native eye-depth shader.");
   if(!nativeSceneEyeDepthR1461||!nativeSceneEyeDepthR1461.IsCreated()||nativeSceneEyeDepthR1461.width!=nativeTarget.width||nativeSceneEyeDepthR1461.height!=nativeTarget.height){
    hasDepthImage=false;lastDepthCropGeneration=-1;
    if(nativeSceneEyeDepthR1461){nativeSceneEyeDepthR1461.Release();DestroyNativeWorldObject(nativeSceneEyeDepthR1461);}
    nativeSceneEyeDepthR1461=new RenderTexture(nativeTarget.width,nativeTarget.height,24,RenderTextureFormat.RFloat,RenderTextureReadWrite.Linear){name="Native scene positive linear eye depth",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp,antiAliasing=1,useMipMap=false,autoGenerateMips=false};
    if(!nativeSceneEyeDepthR1461.Create())throw new InvalidOperationException("Native eye-depth target creation failed.");
   }
   if(nativeSceneDepthCommandsR1461==null)nativeSceneDepthCommandsR1461=new CommandBuffer{name="Native scene occlusion: existing mesh cutout depth"};
  }
  void DrawNativeDepth(RenderTexture target,Matrix4x4 view,Matrix4x4 projection){
   var commands=nativeSceneDepthCommandsR1461;commands.Clear();commands.SetRenderTarget(target);commands.SetViewport(new Rect(0,0,target.width,target.height));
   float clear=nativeCamera.farClipPlane+1;commands.ClearRenderTarget(true,true,new Color(clear,clear,clear,clear));
   commands.SetGlobalMatrix("_NativeOcclusionWorldToViewR1461",view);
   commands.SetGlobalMatrix("_NativeOcclusionWorldToClipR1461",GL.GetGPUProjectionMatrix(projection,true)*view);
   foreach(var part in nativeSceneOccludersR1461)if(part.visible)commands.DrawRenderer(part.renderer,part.draw,0,0);
   var previousTarget=RenderTexture.active;
   try{Graphics.ExecuteCommandBuffer(commands);}finally{RenderTexture.active=previousTarget;}
  }
  Matrix4x4 ActorOcclusionProjection(Matrix4x4 projection){
   if(!CastleCornerProjection)return projection;
   // The castle color pass also draws scenery behind the camera to fill the
   // courtyard corners. Only geometry in front of the eye can hide an actor.
   // Preserve the X/Y crop used by both the live view and the padded depth cache.
   var physical=Matrix4x4.Ortho(-1,1,-1,1,nativeCamera.nearClipPlane,nativeCamera.farClipPlane);
   projection.m22=physical.m22;projection.m23=physical.m23;
   return projection;
  }
  void CaptureSceneryPlateDepth(Matrix4x4 wideProjection){
   EnsureNativeDepthResources();
   wideProjection=ActorOcclusionProjection(wideProjection);
   var view=nativeCamera.worldToCameraMatrix;
   GeometryUtility.CalculateFrustumPlanes(wideProjection*view,depthPlanes);
   foreach(var part in nativeSceneOccludersR1461)UpdateDepthPart(part);
   DrawNativeDepth(sceneryPlateDepth,view,wideProjection);hasDepthImage=false;
  }
  void CaptureNativeSceneEyeDepthR1461(){
   hasBillboardEye=false;
   nativeSceneDepthReadyR1461=false;Shader.SetGlobalFloat("_NativeSceneDepthEnabledR1461",0);
   if(!NativeWorldActive||!nativeRoot||!nativeTarget||!worldCamera)return;
   EnsureNativeDepthResources();
   // Uncut padded depth matches static color. A visible opening needs the
   // same live near-wall cut in depth as in color, including on no-copy GPUs.
   bool staticDepth=plateDepthReady&&!lastPlateColorWasLive;
   sceneryDepthBound=staticDepth&&sceneryUsesNoCopy;
   if(sceneryDepthBound){
    // The actor shader samples a positive RFloat at the tight viewport's
    // integer offset within the padded target. The reveal/dither grid stays
    // in the original viewport. No transfer is issued on a no-copy device.
    hasDepthImage=false;lastDepthCropGeneration=-1;
    TraceScenery(512,"direct-padded-eye-depth");
   }else if(staticDepth){
    // Region copy preserves the positive float eye depth, including values above 1.
    var origin=new Vector2Int(Mathf.RoundToInt(plateCropOrigin.x*sceneryPlateDepth.width),Mathf.RoundToInt(plateCropOrigin.y*sceneryPlateDepth.height));
    if(plateGeneration!=lastDepthCropGeneration||origin!=lastDepthCropOffset){
     Graphics.CopyTexture(sceneryPlateDepth,0,0,origin.x,origin.y,nativeTarget.width,nativeTarget.height,nativeSceneEyeDepthR1461,0,0,0,0);
     lastDepthCropGeneration=plateGeneration;lastDepthCropOffset=origin;
    }
    hasDepthImage=false;
   }else{
    lastDepthCropGeneration=-1;
   var view=nativeCamera.worldToCameraMatrix;var projection=ActorOcclusionProjection(nativeCamera.projectionMatrix);
   bool reveal=Shader.GetGlobalFloat("_NativePlayerRevealEnabledR1478")>.5f;
   var area=reveal?Shader.GetGlobalVector("_NativePlayerRevealViewportR1478"):Vector4.zero;
   var eye=reveal?Shader.GetGlobalVector("_NativePlayerRevealEyeStrengthR1478"):Vector4.zero;
   bool redraw=!hasDepthImage||!lastDepthView.Equals(view)||!lastDepthProjection.Equals(projection)||
    !lastDepthRevealArea.Equals(area)||!lastDepthRevealEye.Equals(eye);
   GeometryUtility.CalculateFrustumPlanes(projection*view,depthPlanes);
   foreach(var part in nativeSceneOccludersR1461)if(UpdateDepthPart(part))redraw=true;
   if(redraw){
    DrawNativeDepth(nativeSceneEyeDepthR1461,view,projection);
   lastDepthView=view;lastDepthProjection=projection;lastDepthRevealArea=area;lastDepthRevealEye=eye;hasDepthImage=true;
   }
   }
   Shader.SetGlobalTexture("_NativeSceneEyeDepthR1461",sceneryDepthBound?sceneryPlateDepth:nativeSceneEyeDepthR1461);
   Shader.SetGlobalVector("_NativeSceneDepthUVR1461",sceneryDepthBound?
    new Vector4(plateCropScale.x,plateCropScale.y,plateCropOrigin.x,plateCropOrigin.y):new Vector4(1,1,0,0));
   Shader.SetGlobalVector("_NativeSceneDepthRangeR1461",new Vector4(nativeCamera.nearClipPlane,nativeCamera.farClipPlane,0,0));
   // Native units, not display pixels; .5 units is .14px for unit .28.
   Shader.SetGlobalFloat("_NativeSceneDepthBiasR1461",.5f);Shader.SetGlobalFloat("_NativeSceneDepthEnabledR1461",1);nativeSceneDepthReadyR1461=true;
  }
  // Runs for the exact native camera immediately before its render, after all
  // ordinary/director camera policies have placed it. No camera or actor moves.
  void RegisterNativePlayerRevealCameraR1478(){if(nativePlayerRevealCameraRegisteredR1478)return;Camera.onPreCull+=PrepareNativePlayerRevealCameraR1478;nativePlayerRevealCameraRegisteredR1478=true;}
  void ReleaseNativePlayerRevealCameraR1478(){
   if(nativePlayerRevealCameraRegisteredR1478){Camera.onPreCull-=PrepareNativePlayerRevealCameraR1478;nativePlayerRevealCameraRegisteredR1478=false;}
   nativePlayerRevealStrengthR1478=0;nativePlayerRevealFrameR1478=-1;Shader.SetGlobalFloat("_NativePlayerRevealEnabledR1478",0);
  }
  Vector3 NativePlayerRevealSourcePointR1478(Vector3 world){var p=NativePathPoint(world.x);p.y=(world.y-nativeScene.offsetY)/nativeScene.unit;return p;}
  void PrepareNativePlayerRevealCameraR1478(Camera camera){
   if(camera!=nativeCamera||renderingAlternateView)return;Shader.SetGlobalFloat("_NativePlayerRevealEnabledR1478",0);
   if(!NativeWorldActive||!nativeTarget||!heroView||!heroView.enabled||!heroView.gameObject.activeInHierarchy)return;
   // Follow the torso on the route. A long sword frame or a bending path must not stretch the hole sideways.
   var center=(Vector3)RenderPlayerPosition+Vector3.up*27;var uv=nativeCamera.WorldToViewportPoint(NativePlayerRevealSourcePointR1478(center));
   float eye=uv.z;
   if(float.IsNaN(eye)||float.IsInfinity(eye)||eye<nativeCamera.nearClipPlane||eye>nativeCamera.farClipPlane||uv.x<0||uv.x>1||uv.y<0||uv.y>1)return;
   // Double the previous screen-space diameter after the camera-dependent clamp.
   var top=nativeCamera.WorldToViewportPoint(NativePlayerRevealSourcePointR1478(center+Vector3.up*108));
   float pixels=2f*Mathf.Clamp(Mathf.Abs(top.y-uv.y)*nativeTarget.height,1,Mathf.Min(nativeTarget.width,nativeTarget.height)*.38f);
   float rx=pixels/nativeTarget.width,ry=pixels/nativeTarget.height;
   if(nativePlayerRevealFrameR1478!=Time.frameCount){nativePlayerRevealFrameR1478=Time.frameCount;nativePlayerRevealStrengthR1478=Mathf.MoveTowards(nativePlayerRevealStrengthR1478,1,Mathf.Min(Time.unscaledDeltaTime,.1f)*4f);}
   var area=new Vector4(uv.x,uv.y,rx,ry);
   if(!SelectBlockingScenery(area))return;
   Shader.SetGlobalVector("_NativePlayerRevealViewportR1478",area);
   Shader.SetGlobalVector("_NativePlayerRevealEyeStrengthR1478",new Vector4(eye,nativePlayerRevealStrengthR1478,.5f,24));
   Shader.SetGlobalVector("_NativePlayerRevealPixelGridR1478",new Vector4(nativeTarget.width,nativeTarget.height,0,0));
   // Canonical CPU camera clip XY matches WorldToViewportPoint. No GPU RT
   // projection/Y flip or hardware-depth reconstruction is applied to this mask.
   Shader.SetGlobalMatrix("_NativePlayerRevealWorldToClipR1478",nativeCamera.projectionMatrix*nativeCamera.worldToCameraMatrix);
   Shader.SetGlobalFloat("_NativePlayerRevealEnabledR1478",1);
  }

  void OnNativeOcclusionFontTextureRebuiltR1461(Font font){
   if(!font)return;var source=font.material;Material material;if(source&&nativeTextOcclusionMaterialsR1461.TryGetValue(source,out material)&&material)material.CopyPropertiesFromMaterial(source);
  }
  void BindNativeSceneOcclusionR1461(Renderer renderer,Vector3 sourceWorld,float sourceDepth){
   BindProjectedSceneOcclusion(renderer,sourceWorld,sourceDepth,null);
  }
  void BindProjectedSceneOcclusion(Renderer renderer,Vector3 sourceWorld,float sourceDepth,float? projectedEye){
   if(!nativeSceneDepthReadyR1461||!renderer||renderer==sceneDim)return;
   var original=renderer.sharedMaterial;if(!original||!original.shader)return;Material assigned=null;string shader=original.shader.name;
   if(shader=="Sprites/Default"){
    if(!(renderer is SpriteRenderer))return;
    if(!nativeDefaultOcclusionShaderR1461)nativeDefaultOcclusionShaderR1461=Resources.Load<Shader>("Pixel/Shaders/NativeOccludedSpriteR1461");
    if(!nativeDefaultOcclusionShaderR1461||!nativeDefaultOcclusionShaderR1461.isSupported)throw new InvalidOperationException("Missing native-occluded default sprite shader.");
    if(!nativeDefaultOcclusionMaterialsR1461.TryGetValue(original,out assigned)||!assigned){assigned=new Material(original){name="Native occlusion / "+original.name};assigned.shader=nativeDefaultOcclusionShaderR1461;nativeDefaultOcclusionMaterialsR1461[original]=assigned;}
    assigned.CopyPropertiesFromMaterial(original);renderer.sharedMaterial=assigned;
   }else if(shader=="GUI/Text Shader"&&renderer.GetComponent<TextMesh>()){
    if(!nativeTextOcclusionShaderR1461)nativeTextOcclusionShaderR1461=Resources.Load<Shader>("Pixel/Shaders/NativeOccludedTextR1461");
    if(!nativeTextOcclusionShaderR1461||!nativeTextOcclusionShaderR1461.isSupported)throw new InvalidOperationException("Missing native-occluded world-text shader.");
    if(!nativeTextOcclusionMaterialsR1461.TryGetValue(original,out assigned)||!assigned){assigned=new Material(original){name="Native world text occlusion / "+original.name};assigned.shader=nativeTextOcclusionShaderR1461;nativeTextOcclusionMaterialsR1461[original]=assigned;}
    assigned.CopyPropertiesFromMaterial(original);renderer.sharedMaterial=assigned;
    if(!nativeOcclusionFontEventRegisteredR1461){Font.textureRebuilt+=OnNativeOcclusionFontTextureRebuiltR1461;nativeOcclusionFontEventRegisteredR1461=true;}
   }else if(shader!="ElswordPixel/ArmoredSprite"&&shader!="ElswordPixel/NativeSkillSprite"&&shader!="ElswordPixel/CargoAirControlledPaletteV21"&&shader!="ElswordPixel/CargoSourceFxV21")return;
   // The parts of a single rig share its anchor and the same captured depth view.
   // Reset on every capture, including when a different camera renders in this frame.
   float eye;
   if(projectedEye.HasValue)eye=projectedEye.Value;
   else if(hasBillboardEye&&lastBillboardOffset==sourceDepth&&lastBillboardSource.Equals(sourceWorld))eye=lastBillboardEye;
   else eye=NativeViewport(sourceWorld,sourceDepth).z;
   hasBillboardEye=true;lastBillboardSource=sourceWorld;lastBillboardOffset=sourceDepth;lastBillboardEye=eye;
   if(float.IsNaN(eye)||float.IsInfinity(eye))throw new InvalidOperationException("Non-finite native billboard eye depth.");
   if(nativeSceneOcclusionPropertiesR1461==null)nativeSceneOcclusionPropertiesR1461=new MaterialPropertyBlock();renderer.GetPropertyBlock(nativeSceneOcclusionPropertiesR1461);
   nativeSceneOcclusionPropertiesR1461.SetFloat("_NativeActorEyeDepthR1461",eye);nativeSceneOcclusionPropertiesR1461.SetFloat("_NativeSceneClipR1461",1);renderer.SetPropertyBlock(nativeSceneOcclusionPropertiesR1461);
   nativeSceneOcclusionBindingsR1461.Add(new NativeSceneOcclusionBindingR1461{renderer=renderer,original=original,assigned=assigned});
  }
 }
}
