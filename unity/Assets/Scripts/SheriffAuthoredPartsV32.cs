using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace ElswordPixel {
 // Presentation-only owner; source whole renderer remains the clock/visibility witness.
 public sealed class SheriffAuthoredPartsV32 : IDisposable {
  public sealed class Binding {public readonly Texture2D atlas;public Binding(Texture2D a){atlas=a;}}
  public struct Frame {public string state,motion;public float sourceClock,sourceDuration,hostClock;public int facing;public bool visible,hasSourceFiringLine;public float sourceFiringLineAboveFoot;}
  readonly Binding binding;readonly SpriteRenderer carrier;readonly Action<UnityEngine.Object> destroy;
  readonly Sprite[] sprites=new Sprite[6];readonly SpriteRenderer[] renderers=new SpriteRenderer[6];
  readonly SheriffAuthoredPoseV32.PartPose[] pose=new SheriffAuthoredPoseV32.PartPose[6];readonly float[] scratch=new float[12];
  readonly MaterialPropertyBlock properties=new MaterialPropertyBlock();readonly bool oldForceOff;
  GameObject root;SortingGroup group;bool disposed,hidden;
  public Binding BindingIdentity{get{return binding;}}public Frame LastCommittedFrame{get;private set;}
  static void Need(bool v,string why){if(!v)throw new InvalidOperationException("Sheriff authored parts: "+why);}
  public SheriffAuthoredPartsV32(SpriteRenderer c,Binding b,Action<UnityEngine.Object> destroyOwned){
   carrier=c;binding=b;destroy=destroyOwned;Need(carrier&&b!=null&&destroy!=null,"explicit original carrier/binding/destroy owner");oldForceOff=carrier.forceRenderingOff;Need(!oldForceOff,"original carrier already hidden by another owner");
   var atlas=b.atlas;Need(atlas&&atlas.width==1536&&atlas.height==1024&&atlas.isReadable&&atlas.isDataSRGB&&atlas.mipmapCount==1&&(atlas.format==TextureFormat.RGBA32||atlas.format==TextureFormat.ARGB32)&&atlas.filterMode==FilterMode.Point&&atlas.wrapMode==TextureWrapMode.Clamp,"unaltered readable RGBA/sRGB/1536x1024/PointClamp/noMip atlas");
   try{root=new GameObject("Owned NUI_NASOD_SHERIFF six parts V32");root.SetActive(false);root.layer=carrier.gameObject.layer;root.transform.SetParent(carrier.transform,false);group=root.AddComponent<SortingGroup>();
    for(int i=0;i<6;i++){var q=SheriffAuthoredPoseV32.Part(i);sprites[i]=Sprite.Create(atlas,new Rect(q.x,1024-q.y-q.height,q.width,q.height),new Vector2(q.pivotX/q.width,1-q.pivotY/q.height),1/SheriffAuthoredPoseV32.UnitsPerArtPixel,0,SpriteMeshType.FullRect);Need(sprites[i],"Sprite.Create");sprites[i].name="Sheriff authored / "+q.name;var go=new GameObject(q.name);go.layer=carrier.gameObject.layer;go.transform.SetParent(root.transform,false);var r=go.AddComponent<SpriteRenderer>();renderers[i]=r;r.sprite=sprites[i];r.drawMode=SpriteDrawMode.Simple;r.sortingOrder=q.order;}
   }catch{Dispose();throw;}
  }
  public bool OwnsRenderer(Renderer r){for(int i=0;i<6;i++)if(ReferenceEquals(r,renderers[i]))return true;return false;}
  internal void VisitRenderersR1461(Action<SpriteRenderer> visit){if(disposed)return;for(int i=0;i<6;i++)if(renderers[i])visit(renderers[i]);}
  // The authored atlas faces +X. The fixed-yaw source carrier intentionally
  // keeps flipX=false, so only this owned rig mirrors the actual actor facing.
  public bool TryVisibleTop(out float top){
   top=float.NegativeInfinity;if(disposed||!root||!root.activeInHierarchy)return false;
   float unit=SheriffAuthoredPoseV32.UnitsPerArtPixel;
   for(int i=0;i<6;i++){var r=renderers[i];if(!r||!r.enabled||r.forceRenderingOff||!r.gameObject.activeInHierarchy||r.color.a<=0)continue;
    var q=SheriffAuthoredPoseV32.Part(i);var hull=q.hull;
    // Exact alpha>=64 unit-pixel convex support, in the real sprite pivot.
    // TransformPoint includes the current pose, part scale, facing and foot.
    for(int j=0;j<hull.Length;j+=2){var point=r.transform.TransformPoint(new Vector3((hull[j]-q.pivotX)*unit,(q.pivotY-hull[j+1])*unit,0));top=Math.Max(top,point.y);}
   }
   return !float.IsNaN(top)&&!float.IsInfinity(top);
  }
  public void Apply(Frame frame,Vector3 projectedFoot){
   Need(!disposed&&root&&carrier,"live presentation owner");Need(!float.IsNaN(projectedFoot.x)&&!float.IsInfinity(projectedFoot.x)&&!float.IsNaN(projectedFoot.y)&&!float.IsInfinity(projectedFoot.y)&&!float.IsNaN(projectedFoot.z)&&!float.IsInfinity(projectedFoot.z),"finite projected original foot");
   var material=carrier.sharedMaterial;Need(material&&material.shader&&material.shader.name=="ElswordPixel/ArmoredSprite"&&material.shader.isSupported,"original final armored material");
   SheriffAuthoredPoseV32.Sample(frame.motion,frame.sourceClock,frame.sourceDuration,pose,scratch);
   if(frame.hasSourceFiringLine)SheriffAuthoredPoseV32.AlignFiringLine(frame.motion,frame.sourceClock,frame.sourceDuration,frame.sourceFiringLineAboveFoot,pose);
   carrier.GetPropertyBlock(properties);properties.SetTexture("_MainTex",binding.atlas);properties.SetVector("_MainTex_TexelSize",new Vector4(1f/1536,1f/1024,1536,1024));
   root.transform.localRotation=Quaternion.identity;root.transform.localScale=new Vector3(frame.facing<0?-1:1,1,1);root.transform.position=projectedFoot;group.sortingLayerID=carrier.sortingLayerID;group.sortingOrder=carrier.sortingOrder;
   for(int i=0;i<6;i++){var p=pose[i];var q=SheriffAuthoredPoseV32.Part(i);var r=renderers[i];r.transform.localPosition=new Vector3(p.x*SheriffAuthoredPoseV32.UnitsPerArtPixel,-(p.y-SheriffAuthoredPoseV32.FootY)*SheriffAuthoredPoseV32.UnitsPerArtPixel,0);r.transform.localRotation=Quaternion.Euler(0,0,-p.rotation);r.transform.localScale=new Vector3(q.scale,q.scale,1);r.sortingLayerID=carrier.sortingLayerID;r.sharedMaterial=material;r.color=carrier.color;r.maskInteraction=carrier.maskInteraction;r.SetPropertyBlock(properties);}
   root.SetActive(frame.visible&&carrier.enabled&&carrier.gameObject.activeInHierarchy);carrier.forceRenderingOff=true;hidden=true;LastCommittedFrame=frame;
  }
  public void Dispose(){if(disposed)return;disposed=true;if(root)root.SetActive(false);if(hidden&&carrier)carrier.forceRenderingOff=oldForceOff;hidden=false;for(int i=0;i<6;i++){if(sprites[i])destroy(sprites[i]);sprites[i]=null;renderers[i]=null;}if(root){root.transform.SetParent(null,false);destroy(root);}root=null;group=null;}
 }
}
