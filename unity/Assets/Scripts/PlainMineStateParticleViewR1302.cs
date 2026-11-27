using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 // Raw original mesh geometry is supplied by the bounded C producer, never an
 // alternate silhouette. Host must verify the complete JSON asset SHA before
 // parsing; raw member SHA alone does not authenticate arbitrary mesh arrays.
 [Serializable] public sealed class PlainMineNativeMatrixR1302 {public float[] values;}
 [Serializable] public sealed class PlainMineStateMeshDocumentR1302 {public string schema;public PlainMineStateMeshR1302[] meshes;}
 [Serializable] public sealed class PlainMineStateMeshPartR1302 {
  public float[] originalVertices,originalUV,materialColor;public PlainMineNativeMatrixR1302[] nativeMatrixChain;public int[] originalTriangles;
  public string originalTexture;
 }
 [Serializable] public sealed class PlainMineStateMeshR1302 {
  public string originalName,originalMemberSha256,verifiedBindAssetSha256;
  public PlainMineStateMeshPartR1302[] parts;
 }
 public sealed partial class PlainMineMajorViewR1302 {
  sealed class StateMeshPart {internal Mesh mesh;internal string texture;internal Color color;}
  sealed class StateEmission {
   internal Enemy actor;internal string name;internal Vector3 origin;internal float landY,born,next,end,lifetime;internal int emitted,targetCount;
  }
  sealed class StateParticle {
   internal StateEmission emitter;internal GameObject root;internal SpriteRenderer sprite;internal readonly List<MeshRenderer> meshes=new List<MeshRenderer>();
   internal Vector3 origin,velocity,scale;internal float born,lifetime,rotation;internal int kind;
  }
  readonly Dictionary<string,List<StateMeshPart>> stateMeshesR1302=new Dictionary<string,List<StateMeshPart>>(StringComparer.Ordinal);
  readonly List<Mesh> ownedStateMeshesR1302=new List<Mesh>();readonly List<StateEmission> stateEmissionsR1302=new List<StateEmission>();readonly List<StateParticle> stateParticlesR1302=new List<StateParticle>();
  Material stateMeshMaterialR1302;readonly MaterialPropertyBlock statePropertiesR1302=new MaterialPropertyBlock();bool stateParticlesBoundR1302;
  public bool StateParticlesBoundR1302 {get{return stateParticlesBoundR1302;}}
  public const string OriginalMeshDataResourceShaR1302="240481fe8fb69310938037f6af4a992b46925834424ce22889294df8c25301fa";
  static Vector3 NativeRowPointR1302(float[] m,float x,float y,float z){return new Vector3(x*m[0]+y*m[4]+z*m[8]+m[12],x*m[1]+y*m[5]+z*m[9]+m[13],x*m[2]+y*m[6]+z*m[10]+m[14]);}
  void InitializeStateMeshesR1302(PlainMineStateMeshR1302[] data,Shader shader){
   stateMeshMaterialR1302=Material(shader,UnityEngine.Rendering.BlendMode.DstAlpha,UnityEngine.Rendering.CompareFunction.Always);
   Need(data!=null&&data.Length==2,"two original Y mesh bindings required");var names=new HashSet<string>(new[]{"Lightning02.Y","Lightning03.Y"},StringComparer.Ordinal);
   foreach(var d in data){Need(d!=null&&names.Remove(d.originalName)&&SHA(d.verifiedBindAssetSha256),"unique original Y asset required");
    string expected=d.originalName=="Lightning02.Y"?"c1823a972d9638b7511e2770f9df076c1df3970a085a4b9f1cc46b2fef2369d1":"ec271504a87bc23b7505578f170bedffe7a4cf65ba59169632e6de40038c8fef";
    string expectedBind=d.originalName=="Lightning02.Y"?"540bfcf7234eee83f7b624ccabd8035b1b92bdeeb109424756b20cdee2d13c82":"221a3984ef15c7d73f44d1e7f691212f843ed4a0c236dc77b4bfd18a1a6f893e";
    Need(d.verifiedBindAssetSha256==expectedBind&&d.originalMemberSha256==expected&&d.parts!=null&&d.parts.Length>0&&d.parts.Length<=64,"original mesh source/parts required");var parts=new List<StateMeshPart>();
    foreach(var p in d.parts){Need(p!=null&&p.originalVertices!=null&&p.originalVertices.Length>=9&&p.originalVertices.Length%3==0&&p.originalTriangles!=null&&p.originalTriangles.Length>0&&p.originalTriangles.Length%3==0&&p.nativeMatrixChain!=null&&p.nativeMatrixChain.Length>0&&p.nativeMatrixChain.Length<=32&&p.materialColor!=null&&p.materialColor.Length==4,"complete original mesh arrays");
     int count=p.originalVertices.Length/3;Need(count<=65536&&p.originalTriangles.Length<=196608,"finite original mesh size");var vertices=new Vector3[count];
     foreach(float f in p.originalVertices)Need(Finite(f),"finite original vertex");foreach(var matrix in p.nativeMatrixChain){Need(matrix!=null&&matrix.values!=null&&matrix.values.Length==16,"complete source matrix chain");foreach(float f in matrix.values)Need(Finite(f),"finite original matrix");}foreach(float f in p.materialColor)Need(Finite(f),"finite original material");
     for(int i=0;i<count;i++){var point=new Vector3(p.originalVertices[i*3],p.originalVertices[i*3+1],p.originalVertices[i*3+2]);foreach(var matrix in p.nativeMatrixChain)point=NativeRowPointR1302(matrix.values,point.x,point.y,point.z);vertices[i]=point;}
     foreach(int i in p.originalTriangles)Need(i>=0&&i<count,"original triangle index");var uv=new Vector2[count];
     if(p.originalUV!=null&&p.originalUV.Length>0){Need(p.originalUV.Length==count*2,"original UV extent");for(int i=0;i<count;i++){Need(Finite(p.originalUV[i*2])&&Finite(p.originalUV[i*2+1]),"finite source UV");uv[i]=new Vector2(p.originalUV[i*2],1-p.originalUV[i*2+1]);}}
     Need(string.IsNullOrEmpty(p.originalTexture)||sprites.ContainsKey(p.originalTexture),"original Y material texture not bound");
     var mesh=new Mesh{name="Original Mine / "+d.originalName};ownedStateMeshesR1302.Add(mesh);mesh.indexFormat=count>65535?UnityEngine.Rendering.IndexFormat.UInt32:UnityEngine.Rendering.IndexFormat.UInt16;mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=(int[])p.originalTriangles.Clone();var colors=new Color[count];for(int i=0;i<count;i++)colors[i]=Color.white;mesh.colors=colors;mesh.RecalculateBounds();
     parts.Add(new StateMeshPart{mesh=mesh,texture=p.originalTexture,color=new Color(p.materialColor[0],p.materialColor[1],p.materialColor[2],p.materialColor[3])});
    }stateMeshesR1302.Add(d.originalName,parts);
   }
   stateParticlesBoundR1302=true;
  }
  public void CreateStateParticleR1302(Enemy actor,string name,PlainMineSourcePositionR1269 position,float sourceLandY,float modelTime){
   Require(actor);FinitePosition(position);Need(stateParticlesBoundR1302&&Finite(sourceLandY)&&Finite(modelTime)&&modelTime>=0,"original state-particle assets/time/land bound");
   Need(name=="Meteo_Explosion01"||name=="Meteo_Explosion02"||name=="Meteo_Explosion03"||name=="Meteo_Explosion04","outside original four state particles");
   Need(stateEmissionsR1302.Count<128,"finite state emitter budget");bool first=name=="Meteo_Explosion01",second=name=="Meteo_Explosion02";
   var e=new StateEmission{actor=actor,name=name,origin=new Vector3(position.x,position.y,position.z),landY=sourceLandY,born=modelTime,next=modelTime,end=modelTime+(first?.4f:second?.3f:0),targetCount=first?80:second?60:0,lifetime=first||second?.6f:name=="Meteo_Explosion03"?1:.2f};stateEmissionsR1302.Add(e);
   // Finite Author2D scheduling, not native count/rate priority proof.
   //03 retains its one immediate particle.04 consumes both original literals:
   //15/s and TriggerCount5, at0,1/15,2/15,3/15,4/15; each Lifetime stays.2.
   // The two plane paths and DrawCount handling retain their existing rules.
   if(!first&&!second){if(name=="Meteo_Explosion03"){NewStateParticleR1302(e,modelTime);e.emitted=1;e.next=float.PositiveInfinity;}
    else{e.targetCount=5;e.end=modelTime+4f/15f;}}
  }
  void NewStateParticleR1302(StateEmission e,float at){
   if(stateParticlesR1302.Count+minorParticlesR1302.Count>=4096){NullableBudgetRejections++;return;}int kind=e.name=="Meteo_Explosion01"?0:e.name=="Meteo_Explosion02"?1:e.name=="Meteo_Explosion03"?2:3;
   var p=new StateParticle{emitter=e,kind=kind,born=at,lifetime=e.lifetime,origin=e.origin,scale=Vector3.one};
   try{p.root=new GameObject("Original Mine state / "+e.name);p.root.layer=28;p.root.transform.SetParent(parent,false);
    if(kind<2){p.origin+=new Vector3(RandomR1302(-75,75),RandomR1302(kind==0?-20:10,kind==0?75:105),RandomR1302(-75,75));p.velocity=new Vector3(RandomR1302(kind==0?-200:-500,kind==0?200:500),RandomR1302(-20,kind==0?200:700),RandomR1302(kind==0?-200:-500,kind==0?200:500));p.rotation=RandomR1302(0,360);p.sprite=p.root.AddComponent<SpriteRenderer>();p.sprite.sharedMaterial=lightMaterial;p.sprite.sortingOrder=14;p.sprite.sprite=sprites[kind==0?"smoke02.dds":"ColorBallFire.dds"];}
    else{if(kind==3)p.scale=new Vector3(RandomR1302(.7f,1),1,RandomR1302(.5f,.8f));foreach(var part in stateMeshesR1302[kind==2?"Lightning02.Y":"Lightning03.Y"]){var go=new GameObject("Original Y mesh part");go.layer=28;go.transform.SetParent(p.root.transform,false);go.AddComponent<MeshFilter>().sharedMesh=part.mesh;var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=stateMeshMaterialR1302;renderer.sortingOrder=14;p.meshes.Add(renderer);}}
    stateParticlesR1302.Add(p);ApplyStateParticleR1302(p,Math.Max(0,lastTime-at));
   }catch{if(p.root)UnityEngine.Object.Destroy(p.root);throw;}
  }
  void ApplyStateParticleR1302(StateParticle p,float age){
   float alpha,size=1;Vector3 at=p.origin+p.velocity*age;if(p.kind==1)at.y-=350*age*age;if(p.kind<2)at.y=Math.Max(p.emitter.landY,at.y);
   if(p.kind<2){alpha=age<.01f?0:age<.15f?(age-.01f)/.14f:age<.4f?1:Mathf.Clamp01((.6f-age)/.2f);float initial=p.kind==0?150:30,final=p.kind==0?100:15;size=age<.25f?initial:age<.5f?initial+(final-initial)*(age-.25f)/.25f:final;p.root.transform.rotation=camera.transform.rotation*Quaternion.Euler(0,0,p.rotation);var t=p.sprite.sprite.texture;p.root.transform.localScale=new Vector3(size/t.width,size/t.height,1);p.sprite.color=new Color(1,.6f,.4f,alpha);}
   else{alpha=p.kind==2?(age<.01f?0:age<.033f?(age-.01f)/.023f:age<.3f?1:Mathf.Clamp01((1-age)/.7f)):(age<.1f?.3f:.3f*Mathf.Clamp01((.2f-age)/.1f));p.root.transform.rotation=Quaternion.Euler(0,camera.transform.eulerAngles.y,0);p.root.transform.localScale=p.scale;var parts=stateMeshesR1302[p.kind==2?"Lightning02.Y":"Lightning03.Y"];
    for(int i=0;i<p.meshes.Count;i++){var part=parts[i];statePropertiesR1302.Clear();statePropertiesR1302.SetColor("_Color",new Color(part.color.r,part.color.g*.6f,part.color.b*.4f,part.color.a*alpha));if(!string.IsNullOrEmpty(part.texture))statePropertiesR1302.SetTexture("_MainTex",sprites[part.texture].texture);p.meshes[i].SetPropertyBlock(statePropertiesR1302);}
   }p.root.transform.position=at;
  }
  void RenderStateParticlesR1302(float modelTime){
   for(int i=stateEmissionsR1302.Count-1;i>=0;i--){var e=stateEmissionsR1302[i];Require(e.actor);while(e.next<=modelTime&&e.emitted<e.targetCount){float at=e.next;e.emitted++;e.next=e.born+e.emitted/(e.name=="Meteo_Explosion04"?15f:200f);if(modelTime-at<e.lifetime)NewStateParticleR1302(e,at);}if(modelTime>=e.end+e.lifetime)stateEmissionsR1302.RemoveAt(i);}
   for(int i=stateParticlesR1302.Count-1;i>=0;i--){var p=stateParticlesR1302[i];Require(p.emitter.actor);float age=modelTime-p.born;if(age>=p.lifetime){if(p.root)UnityEngine.Object.Destroy(p.root);stateParticlesR1302.RemoveAt(i);}else if(age>=0)ApplyStateParticleR1302(p,age);}
  }
  void ClearStateParticlesR1302(){var errors=new List<Exception>();foreach(var p in stateParticlesR1302){var root=p.root;Cleanup(()=>{if(root)UnityEngine.Object.Destroy(root);},errors);}stateParticlesR1302.Clear();stateEmissionsR1302.Clear();if(errors.Count>0)throw new AggregateException("Mine state-particle cleanup failed",errors);}
  void DisposeStateMeshesR1302(){var errors=new List<Exception>();Cleanup(ClearStateParticlesR1302,errors);foreach(var m in ownedStateMeshesR1302){var mesh=m;Cleanup(()=>{if(mesh)UnityEngine.Object.Destroy(mesh);},errors);}ownedStateMeshesR1302.Clear();stateMeshesR1302.Clear();stateParticlesBoundR1302=false;if(errors.Count>0)throw new AggregateException("Mine state-mesh cleanup failed",errors);}
 }
}
