using System;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace ElswordPixel {
 // Authored A1 display study for NUI_CHICKEN_FEMALE flock member root17.
 // This is neither a source skeleton binding nor a new gameplay action.
 // No MonoBehaviour, Update, clock, state mutation, resource load or camera render.
 public sealed class FemaleRoot17A1DrawnPartsV29 : IDisposable {
  public struct PartPose {
   public float dx,dy,rotation,scaleX,scaleY;
   public int order;public bool overrideOrder;
   public PartPose(float x,float y,float degrees,float sx=1,float sy=1){
    dx=x;dy=y;rotation=degrees;scaleX=sx;scaleY=sy;order=0;overrideOrder=false;
   }
   public static PartPose Rest {get{return new PartPose(0,0,0);}}
  }
  public sealed class Binding {
   public readonly Texture2D atlas;public readonly Shader spriteShader;
   public readonly float nativeUnitsPerArtPixel;public readonly Vector2 artAnchorPixels;
   public readonly Vector3 fixedRestAnchorOffsetNative;
   public readonly Func<int,Vector3> root17NativeOriginByFrame;
   public readonly int sortingLayerId,sortingOrderBase;
   // Arguments are (already-selected source frame 0..272, A1 part index 0..5).
   // Authored entry follows the selected source-frame phases, then a subtle settled idle.
   // Source trajectory/frame never wraps; the local curve reaches art rest at frame272.
   public readonly Func<int,int,PartPose> sampleAuthoredPose;
   public Binding(Texture2D texture,Shader shader,float fixedNativeUnitsPerArtPixel,
    Vector3 fixedRestOffsetNative,Vector2 fixedArtAnchorPixels,Func<int,Vector3> originByFrame,
    Func<int,int,PartPose> sample,int layerId=0,int orderBase=0){
    atlas=texture;spriteShader=shader;nativeUnitsPerArtPixel=fixedNativeUnitsPerArtPixel;
    fixedRestAnchorOffsetNative=fixedRestOffsetNative;root17NativeOriginByFrame=originByFrame;
    artAnchorPixels=fixedArtAnchorPixels;sampleAuthoredPose=sample;
    sortingLayerId=layerId;sortingOrderBase=orderBase;
   }
  }
  struct Layout {
   internal string name;internal Rect topLeftRect;internal Vector2 pivot,position;
   internal float scale,rotation;internal int order;
   internal Layout(string n,float x,float y,float w,float h,float px,float py,
    float atX,float atY,float s,int z){name=n;topLeftRect=new Rect(x,y,w,h);
    pivot=new Vector2(px,py);position=new Vector2(atX,atY);scale=s;rotation=0;order=z;}
  }
  // Bold A1 cells retain every absolute atlas anchor and the original fixed assembly registration.
  static readonly Layout[] Parts={
   new Layout("head",0,0,600,640,272,523,576,794,1.46f,50),
   new Layout("torso",600,0,536,640,237,246,576,845,1,30),
   new Layout("screen-left-wing",1136,0,400,640,239,280,410,850,.9f,20),
   new Layout("screen-right-wing",0,640,600,384,208,120,748,856,.9f,21),
   new Layout("screen-left-leg-foot",600,640,536,384,182,57,420,1011,.85f,10),
   new Layout("screen-right-leg-foot",1136,640,400,384,146,61,716,1011,.8f,11)
  };
  static readonly string[] SourcePartNames={"BODY/Object56","BODY/Sphere03","BODY/KSM_UnnamedFrame_30","BODY/KSM_UnnamedFrame_31"};
  // Exact art foot pivot chosen in the new Root trajectory asset, not a source joint.
  public static Vector2 AuthoredRestFootAnchorPixels {get{return new Vector2(576,1180);}}
  public static string PartName(int index){Need(index>=0&&index<6,"Part index");return Parts[index].name;}
  // Authored numeric display proposal, not a transcription of original bone poses.
  // The first column is the existing selected source frame, never a new clock.
  // Frames7/11/22/40/52/62/84 are observed native-Y direction changes or the stop.
  // Their labels carry no unverified takeoff, contact or gameplay-event claim.
  // Entry0..84 is played once. Frames84..272 have a small four-second idle wave,
  // eased in/out over12 source frames. Feet keep their original authored rest pose.
  static readonly double[,] AuthoredEntryKeys={
   {0,0,0,0,0},{3,2,1,-6,-1},{7,6,4,-14,1},{11,-7,-4,6,-2},
   {16,-4,-2,-10,1},{22,6,4,-18,2},{30,-8,-5,32,-3},
   {40,-12,-8,8,-1},{46,-4,-3,-12,2},{52,7,4,-20,3},
   {57,-9,-6,34,-3},{62,-12,-8,10,-1},{72,-7,-4,-8,2},
   {80,2,1,-20,3},{84,0,0,0,0}
  };
  static double SmoothAuthoredUnit(double value){
   if(value<=0)return 0;if(value>=1)return 1;return value*value*(3-2*value);
  }
  static double AuthoredEntryValue(int key,int column,double t){
   return AuthoredEntryKeys[key,column]+(AuthoredEntryKeys[key+1,column]-AuthoredEntryKeys[key,column])*t;
  }
  // Compatibility entry point retained for the existing registrar; its policy now
  // spans the full clamped273-frame entry+settled-idle display, rather than one second.
  public static PartPose SampleAuthoredOneSecondPose(int selectedSourceFrame,int partIndex){
   Need(selectedSourceFrame>=0&&selectedSourceFrame<=272&&partIndex>=0&&partIndex<6,"Existing source frame and A1 part index required");
   if(selectedSourceFrame==272||partIndex>=4)return PartPose.Rest;
   double headDy,bodyDy,wingAngle,headAngle;
   if(selectedSourceFrame<=84){
    int k=0;while(k<AuthoredEntryKeys.GetLength(0)-2&&selectedSourceFrame>AuthoredEntryKeys[k+1,0])k++;
    double t=SmoothAuthoredUnit((selectedSourceFrame-AuthoredEntryKeys[k,0])/(AuthoredEntryKeys[k+1,0]-AuthoredEntryKeys[k,0]));
    headDy=AuthoredEntryValue(k,1,t);bodyDy=AuthoredEntryValue(k,2,t);
    wingAngle=AuthoredEntryValue(k,3,t);headAngle=AuthoredEntryValue(k,4,t);
   }else{
    double fromStop=selectedSourceFrame-84;
    double envelope=SmoothAuthoredUnit(fromStop/12d)*SmoothAuthoredUnit((272-selectedSourceFrame)/12d);
    double phase=fromStop*(2*Math.PI)/96d;
    double breath=(1-Math.Cos(phase))*.5;
    headDy=-1.5*envelope*breath;bodyDy=-.75*envelope*breath;
    wingAngle=1.5*envelope*Math.Sin(phase);headAngle=.2*envelope*Math.Sin(phase);
   }
   if(partIndex==0)return new PartPose(0,(float)headDy,(float)headAngle);
   if(partIndex==1)return new PartPose(0,(float)bodyDy,(float)(headAngle*.12));
   if(partIndex==2)return new PartPose(0,(float)bodyDy,(float)wingAngle);
   return new PartPose(0,(float)bodyDy,(float)-wingAngle);
  }
  readonly Binding binding;readonly Transform nativeGroup;readonly Camera nativeCamera;
  readonly MeshRenderer[] sourceRenderers=new MeshRenderer[4];readonly bool[] originalEnabled=new bool[4];
  readonly Sprite[] sprites=new Sprite[6];readonly SpriteRenderer[] renderers=new SpriteRenderer[6];
  readonly PartPose[] pending=new PartPose[6];readonly Action<UnityEngine.Object> destroyOwned;
  GameObject root;Material material;bool hidden,disposed;int lastFrame=-1;
  public Binding BindingIdentity {get{return binding;}}
  static void Need(bool ok,string why){if(!ok)throw new InvalidOperationException("Female root17 authored A1: "+why);}
  static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
  static bool Finite(Vector3 v){return Finite(v.x)&&Finite(v.y)&&Finite(v.z);}
  public FemaleRoot17A1DrawnPartsV29(Transform existingNativeGroup,Camera camera,
   MeshRenderer[] exactRoot17Renderers,Binding settings,Action<UnityEngine.Object> destroy){
   binding=settings;nativeGroup=existingNativeGroup;nativeCamera=camera;destroyOwned=destroy;
   Need(binding!=null&&nativeGroup&&nativeCamera&&nativeCamera.orthographic&&destroyOwned!=null,"Explicit binding/group/camera/owner required");
   Need(binding.atlas&&binding.atlas.width==1536&&binding.atlas.height==1024&&
    binding.atlas.format==TextureFormat.RGBA32&&binding.atlas.isReadable&&binding.atlas.filterMode==FilterMode.Point&&
    binding.atlas.mipmapCount==1,"A1 shared atlas must be RGBA32/readable/Point/noMip 1536x1024");
   Need(binding.spriteShader&&binding.spriteShader.isSupported&&binding.spriteShader.name=="Sprites/Default","Supported sprite shader required");
   Need(Finite(binding.nativeUnitsPerArtPixel)&&binding.nativeUnitsPerArtPixel>0&&
    Finite(binding.artAnchorPixels.x)&&Finite(binding.artAnchorPixels.y)&&Finite(binding.fixedRestAnchorOffsetNative)&&
    binding.root17NativeOriginByFrame!=null&&binding.sampleAuthoredPose!=null,"Fixed art registration, native origin provider and authored pose required");
   Need(nativeGroup.lossyScale==Vector3.one,"Existing native group must retain unit scale");
   Need(exactRoot17Renderers!=null&&exactRoot17Renderers.Length==4,"Only four root17 mesh renderers");
   for(int i=0;i<4;i++){
    var r=exactRoot17Renderers[i];Need(r&&r.transform.parent==nativeGroup&&r.gameObject.name==SourcePartNames[i],"Exact root17 renderer identity");
    for(int j=0;j<i;j++)Need(r!=sourceRenderers[j],"Duplicate root17 renderer");
    sourceRenderers[i]=r;originalEnabled[i]=r.enabled;
   }
   try{
    root=new GameObject("NUI_CHICKEN_FEMALE root17 authored A1 six parts");root.SetActive(false);root.layer=28;
    root.transform.SetParent(nativeGroup,false);root.transform.localScale=Vector3.one;
    material=new Material(binding.spriteShader){name="Owned Female root17 A1 sprite material"};
    for(int i=0;i<6;i++){
     var p=Parts[i];var r=p.topLeftRect;
     // Authoring JSON has a top-left origin; Unity sprite rect/pivot have bottom-left origins.
     var rect=new Rect(r.x,binding.atlas.height-r.y-r.height,r.width,r.height);
     var pivot=new Vector2(p.pivot.x/r.width,1-p.pivot.y/r.height);
     sprites[i]=Sprite.Create(binding.atlas,rect,pivot,1/binding.nativeUnitsPerArtPixel,0,SpriteMeshType.FullRect);
     Need(sprites[i],"Sprite.Create failed");sprites[i].name="A1 authored "+p.name;
     var go=new GameObject(p.name);go.layer=28;go.transform.SetParent(root.transform,false);
     var sr=go.AddComponent<SpriteRenderer>();renderers[i]=sr;sr.sprite=sprites[i];sr.sharedMaterial=material;
     sr.color=Color.white;sr.flipX=false;sr.flipY=false;sr.drawMode=SpriteDrawMode.Simple;
     sr.sortingLayerID=binding.sortingLayerId;sr.sortingOrder=checked(binding.sortingOrderBase+p.order);
    }
   }catch{Dispose();throw;}
  }
  public void ApplySelectedFrame(int selectedSourceFrame,Vector3 currentRoot17NativeOrigin,Vector3 fixedRestAnchorOffsetNative){
   Need(!disposed&&root&&material,"Owner disposed");Need(selectedSourceFrame>=0&&selectedSourceFrame<=272,"Original selected frame0..272 required");
   Need(Finite(currentRoot17NativeOrigin)&&Finite(fixedRestAnchorOffsetNative)&&
    fixedRestAnchorOffsetNative==binding.fixedRestAnchorOffsetNative,"Explicit native origin and fixed registered rest offset required");
   // Billboard orientation must still follow the existing camera while source frame272 is clamped.
   root.transform.rotation=nativeCamera.transform.rotation;
   if(lastFrame==selectedSourceFrame)return;
   // Validate all six authored poses before changing visibility or committing any transform.
   for(int i=0;i<6;i++){
    var q=selectedSourceFrame==272?PartPose.Rest:binding.sampleAuthoredPose(selectedSourceFrame,i);
    Need(Finite(q.dx)&&Finite(q.dy)&&Finite(q.rotation)&&Finite(q.scaleX)&&Finite(q.scaleY)&&q.scaleX>0&&q.scaleY>0,"Finite positive authored pose scales");
    Need(renderers[i]&&sprites[i],"Owned A1 renderer/sprite missing");pending[i]=q;
   }
   // The artist asset origin is already in native units and excludes sourcePlace.
   // Parent group supplies sourcePlace once. No per-frame bbox, .28, B delta or root-motion write.
   root.transform.localPosition=currentRoot17NativeOrigin+fixedRestAnchorOffsetNative;
   float u=binding.nativeUnitsPerArtPixel;
   for(int i=0;i<6;i++){
    var p=Parts[i];var q=pending[i];var t=renderers[i].transform;
    t.localPosition=new Vector3((p.position.x+q.dx-binding.artAnchorPixels.x)*u,-(p.position.y+q.dy-binding.artAnchorPixels.y)*u,0);
    t.localRotation=Quaternion.Euler(0,0,-(p.rotation+q.rotation));
    t.localScale=new Vector3(p.scale*q.scaleX,p.scale*q.scaleY,1);
    renderers[i].sortingOrder=checked(binding.sortingOrderBase+(q.overrideOrder?q.order:p.order));
   }
   root.SetActive(true);
   // Only these four root17 renderers are hidden; the other18 meshes/five members are untouched.
   hidden=true;
   for(int i=0;i<4;i++)sourceRenderers[i].enabled=false;
   lastFrame=selectedSourceFrame;
  }
  public void Dispose(){
   if(disposed)return;disposed=true;if(root)root.SetActive(false);
   if(hidden)for(int i=0;i<4;i++)if(sourceRenderers[i])sourceRenderers[i].enabled=originalEnabled[i];
   hidden=false;
   for(int i=0;i<6;i++){if(sprites[i])destroyOwned(sprites[i]);sprites[i]=null;renderers[i]=null;}
   if(material)destroyOwned(material);material=null;
   if(root){root.transform.SetParent(null,false);destroyOwned(root);}root=null;
   // The root destroys its six child GameObjects and Renderer components.
   // Shared atlas/shader and original meshes/materials are never destroyed by this owner.
  }
 }

 // Tiny reader of the new artist display asset, not a Motion/rig binary reader.
 // Parsing has no clock and returns only the externally selected frame origin.
 public sealed class FemaleRoot17A1TrajectoryV29 {
  const string ApprovedJsonSha256="213466b9008fe8259a35723d4d168b65d6825bb1ac06347cb9f9765b4ad1c7d1";
  [Serializable] sealed class XYZ {public double x=0,y=0,z=0;}
  [Serializable] sealed class XY {public double x=0,y=0;}
  [Serializable] sealed class Frame {public int frame=0;public double time=0,x=0,y=0,z=0;}
  [Serializable] sealed class Data {
   public string schema="",sourceNpc="",sourceAnimation="";public int sourceRootIndex=0,sourceHz=0,frameCount=0;
   public double duration=0,nativeUnitsPerArtPixel=0;
   public bool sourceFramesClampedNeverWrapped=false,originsAlreadyInNativeUnits=false,sourcePlaceAlreadyIncluded=false;
   public XYZ fixedRestAnchorOffsetNative=new XYZ();public XY artFootPivot=new XY();public Frame[] frames=new Frame[0];
  }
  readonly Vector3[] origins;
  public Vector3 FixedRestAnchorOffsetNative {get;private set;}
  public float NativeUnitsPerArtPixel {get;private set;}
  public Vector2 ArtFootPivot {get;private set;}
  FemaleRoot17A1TrajectoryV29(Vector3[] frames){origins=frames;}
  static void Need(bool ok,string why){if(!ok)throw new InvalidOperationException("Female root17 artist trajectory: "+why);}
  static bool Finite(double v){return !double.IsNaN(v)&&!double.IsInfinity(v);}
  static float ToFloat(double v){Need(Finite(v),"Finite double");float f=(float)v;Need(!float.IsNaN(f)&&!float.IsInfinity(f),"Finite Unity float");return f;}
  public static FemaleRoot17A1TrajectoryV29 FromApprovedBytes(byte[] approvedJson){
   Need(approvedJson!=null&&approvedJson.Length==42698,"Exact Root artist JSON bytes");var bytes=(byte[])approvedJson.Clone();
   string hash;using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
   Need(hash==ApprovedJsonSha256,"Root trajectory JSON identity");
   var d=JsonUtility.FromJson<Data>(new UTF8Encoding(false,true).GetString(bytes));
   Need(d!=null&&d.schema=="WhiteChickenA1-existing-root17-native-trajectory-v1"&&
    d.sourceNpc=="NUI_CHICKEN_FEMALE"&&d.sourceRootIndex==17&&d.sourceAnimation=="WaitStart"&&
    d.frameCount==273&&d.sourceHz==24&&d.duration==34d/3d&&d.sourceFramesClampedNeverWrapped&&
    d.originsAlreadyInNativeUnits&&!d.sourcePlaceAlreadyIncluded&&d.frames!=null&&d.frames.Length==273&&
    d.fixedRestAnchorOffsetNative!=null&&d.artFootPivot!=null,"Root17 native trajectory contract");
   var frames=new Vector3[273];double prior=-1;
   for(int i=0;i<273;i++){
    var f=d.frames[i];Need(f!=null&&f.frame==i&&Finite(f.time)&&f.time>prior&&f.time>=0&&f.time<=d.duration&&
     (i!=0||f.time==0)&&(i!=272||f.time==d.duration),"Existing frame/time order");
    frames[i]=new Vector3(ToFloat(f.x),ToFloat(f.y),ToFloat(f.z));prior=f.time;
   }
   var result=new FemaleRoot17A1TrajectoryV29(frames);
   result.FixedRestAnchorOffsetNative=new Vector3(ToFloat(d.fixedRestAnchorOffsetNative.x),ToFloat(d.fixedRestAnchorOffsetNative.y),ToFloat(d.fixedRestAnchorOffsetNative.z));
   result.NativeUnitsPerArtPixel=ToFloat(d.nativeUnitsPerArtPixel);Need(result.NativeUnitsPerArtPixel>0,"Positive fixed registration");
   result.ArtFootPivot=new Vector2(ToFloat(d.artFootPivot.x),ToFloat(d.artFootPivot.y));return result;
  }
  public Vector3 GetNativeOrigin(int selectedFrame){Need(selectedFrame>=0&&selectedFrame<273,"Already-selected frame0..272 required");return origins[selectedFrame];}
 }

 public sealed partial class DemoScene {
  // Future Root integration supplies explicit atlas/registration/authored curve.
  // Missing binding leaves the existing complete native group unchanged.
  FemaleRoot17A1DrawnPartsV29.Binding femaleRoot17A1BindingV29;
  void FemaleNativeApplyRoot17A1V29(FemaleNativeDisplayV28 s){
   EnsureFemaleRoot17A1RegistrationV29();
   var b=femaleRoot17A1BindingV29;
   if(s.a1Root17!=null&&!ReferenceEquals(s.a1Root17.BindingIdentity,b)){s.a1Root17.Dispose();s.a1Root17=null;}
   if(b==null||ReferenceEquals(b,s.a1Root17BlockedBinding))return;
   try{
    if(s.a1Root17==null){
     FemaleNativeNeedV28(s.group&&s.group.transform.childCount==22,"A1 must bind original22 before adding its own group");
     int[] indices={3,5,8,9};var roots=new MeshRenderer[4];
     for(int i=0;i<4;i++){
      int at=indices[i];var child=s.group.transform.GetChild(at);var mf=child.GetComponent<MeshFilter>();
      FemaleNativeNeedV28(s.data.parts[at].componentRootIndex==17&&mf&&mf.sharedMesh==s.meshes[at],"A1 exact root17 mesh ownership");
      roots[i]=child.GetComponent<MeshRenderer>();
     }
     s.a1Root17=new FemaleRoot17A1DrawnPartsV29(s.group.transform,nativeCamera,roots,b,DestroyNativeWorldObject);
    }
    s.a1Root17.ApplySelectedFrame(s.frame,b.root17NativeOriginByFrame(s.frame),b.fixedRestAnchorOffsetNative);
   }catch(Exception error){
    if(s.a1Root17!=null){s.a1Root17.Dispose();s.a1Root17=null;}
    s.a1Root17BlockedBinding=b;
    Debug.LogWarning("FEMALE_ROOT17_A1_AUTHORED_FALLBACK_V29 "+error.Message);
   }
  }
 }
}
