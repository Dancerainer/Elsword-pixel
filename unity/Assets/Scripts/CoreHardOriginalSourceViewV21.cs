using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 // Controlled original-source display only. No art acceptance is implied.
 [Serializable] public sealed class CoreEntrySourcePartV21 {public string name,texture;public int count,uvOffset;}
 [Serializable] public sealed class CoreEntrySourcePoseV21 {public string id,animation;public float time,duration;public int offset;}
 [Serializable] public sealed class CoreEntrySourceShiftV21 {public float x,y,z;}
 [Serializable] public sealed class CoreEntrySourceIdentityV21 {
  public string id;public float nativeUnitScale,unit,sourceWaitFloor;public float[] sourceCommonReferenceOffset;
  public CoreEntrySourceShiftV21 referenceUniformTranslation;public CoreEntrySourcePartV21[] parts;public CoreEntrySourcePoseV21[] poses;
 }
 public sealed class CoreHardOriginalSourceViewV21:IDisposable {
  sealed class Actor {internal GameObject root;internal Mesh[] meshes;internal Material[] materials;internal Transform[] transforms;internal string pose,state;internal int renderedFrame=-1;}
  sealed class Identity {internal CoreEntrySourceIdentityV21 data;internal byte[] geometry,uvs;internal Dictionary<string,Texture2D> textures;internal Dictionary<string,NpcStateDefinition> states;}
  readonly Transform parent;readonly Shader shader;readonly IDictionary<Transform,float> depths;
  readonly Dictionary<string,Identity> identities=new Dictionary<string,Identity>(StringComparer.Ordinal);
  readonly Dictionary<Enemy,Actor> actors=new Dictionary<Enemy,Actor>();
  public int DrawCalls {get;private set;}public int MissingStateHolds {get;private set;}
  public int LiveActors {get{return actors.Count;}}public bool Disposed {get;private set;}
  public CoreHardOriginalSourceViewV21(Transform owner,Shader sourceShader,IDictionary<Transform,float> sourceDepths){if(!owner||!sourceShader)throw new ArgumentException("Core source diagnostic owner/shader missing");parent=owner;shader=sourceShader;depths=sourceDepths;}
  public void Register(CoreEntrySourceIdentityV21 identity,byte[] geometry,byte[] uvs,Dictionary<string,Texture2D> textures,NpcDefinition profile){
   if(Disposed||identity==null||profile==null||identity.id!=profile.id||identity.parts==null||identity.poses==null||identity.unit!=.28f)throw new ArgumentException("Core exact source actor identity");
   int count=0;foreach(var part in identity.parts){if(part.count<=0||part.count%3!=0||!textures.ContainsKey(part.texture)||part.uvOffset<0||part.uvOffset+part.count*8>uvs.Length)throw new ArgumentException("Core source mesh/UV/texture closure");count+=part.count;}
   foreach(var pose in identity.poses)if(pose.offset<0||(long)pose.offset+count*12L>geometry.Length)throw new ArgumentException("Core source pose range");
   var states=new Dictionary<string,NpcStateDefinition>(StringComparer.Ordinal);foreach(var state in profile.states){if(!Array.Exists(identity.poses,p=>p.animation==state.sourceAnimation))throw new ArgumentException("Core source state missing original motion "+state.id);states.Add(state.id,state);}
   identities.Add(profile.id,new Identity{data=identity,geometry=geometry,uvs=uvs,textures=textures,states=states});
  }
  static void Remove(UnityEngine.Object value){if(!value)return;if(Application.isPlaying)UnityEngine.Object.Destroy(value);else UnityEngine.Object.DestroyImmediate(value);}
  Actor Create(Identity identity){
   var root=new GameObject("CONTROLLED ORIGINAL SOURCE / "+identity.data.id);root.transform.SetParent(parent,false);
   int length=identity.data.parts.Length;var actor=new Actor{root=root,meshes=new Mesh[length],materials=new Material[length],transforms=new Transform[length]};
   for(int n=0;n<length;n++){var part=identity.data.parts[n];var child=new GameObject(part.name);child.transform.SetParent(root.transform,false);actor.transforms[n]=child.transform;
    var mesh=new Mesh{name="Core exact source "+part.name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};actor.meshes[n]=mesh;var indices=new int[part.count];var uv=new Vector2[part.count];for(int i=0;i<part.count;i++){indices[i]=i;uv[i]=new Vector2(BitConverter.ToSingle(identity.uvs,part.uvOffset+i*8),BitConverter.ToSingle(identity.uvs,part.uvOffset+i*8+4));}
    mesh.vertices=new Vector3[part.count];mesh.uv=uv;mesh.triangles=indices;child.AddComponent<MeshFilter>().sharedMesh=mesh;
    var material=new Material(shader){mainTexture=identity.textures[part.texture]};actor.materials[n]=material;var renderer=child.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.sortingOrder=15;
   }return actor;
  }
  // Sparse source samples are causal whole-assembly holds. This method reads
  // the existing clock and never enters a state, advances time, or writes HP.
  public bool Render(Enemy enemy,Vector2 actorRoot,bool visible,Action<string,float> sourceWitness=null){
   Identity identity;if(Disposed||!identities.TryGetValue(enemy.sourceNPC,out identity))return false;
   Actor actor;if(!actors.TryGetValue(enemy,out actor)){actor=Create(identity);actors.Add(enemy,actor);}
   NpcStateDefinition state;if(string.IsNullOrEmpty(enemy.npcState)||!identity.states.TryGetValue(enemy.npcState,out state)){actor.root.SetActive(false);MissingStateHolds++;return true;}
   if(enemy.visualScale!=1)throw new ArgumentException("Core source geometry already includes native UNIT_SCALE");
   var poses=Array.FindAll(identity.data.poses,p=>p.animation==state.sourceAnimation);Array.Sort(poses,(a,b)=>a.time.CompareTo(b.time));
   float clock=Math.Max(0,enemy.npcTime)*state.sourceAnimSpeed;float duration=poses[0].duration;if(state.loop&&duration>0)clock%=duration;
   sourceWitness?.Invoke(state.sourceAnimation,Mathf.Min(clock,duration));
   var pose=poses[0];foreach(var p in poses)if(p.time<=clock+1e-6f)pose=p;
   actor.root.SetActive(visible);actor.root.transform.localPosition=new Vector3(actorRoot.x,actorRoot.y,0);actor.root.transform.localScale=Vector3.one;actor.root.transform.localRotation=Quaternion.identity;
   if(actor.pose!=pose.id){
    int cursor=pose.offset;var shift=identity.data.referenceUniformTranslation;float shiftX=shift==null?0:shift.x,shiftY=shift==null?0:shift.y,shiftZ=shift==null?0:shift.z;
    var common=identity.data.sourceCommonReferenceOffset;float commonX=common!=null&&common.Length>=3?common[0]*identity.data.unit*identity.data.nativeUnitScale:0;
    float commonZ=common!=null&&common.Length>=3?common[2]*identity.data.unit*identity.data.nativeUnitScale:0;
    float floor=identity.data.sourceWaitFloor*identity.data.unit*identity.data.nativeUnitScale;
    for(int n=0;n<actor.meshes.Length;n++){int count=identity.data.parts[n].count;var vertices=new Vector3[count];for(int i=0;i<count;i++,cursor+=12){float x=BitConverter.ToSingle(identity.geometry,cursor)-shiftX+commonX;float y=BitConverter.ToSingle(identity.geometry,cursor+4)-shiftY+floor;float z=BitConverter.ToSingle(identity.geometry,cursor+8)-shiftZ+commonZ;vertices[i]=new Vector3(x,y,z*.001f);}actor.meshes[n].vertices=vertices;actor.meshes[n].RecalculateBounds();}actor.pose=pose.id;
   }
   foreach(var t in actor.transforms)depths[t]=enemy.nativeDepth;
   float alpha=enemy.hp<=0?Mathf.Max(0,1-enemy.deathTime/1.2f):1;foreach(var m in actor.materials)m.color=new Color(1,1,1,alpha);actor.renderedFrame=Time.frameCount;actor.state=state.id;DrawCalls++;return true;
  }
  // Read-only view identity/mesh witness for the same original Enemy object.
  public CoreStopSourceActorFrameV21 ReadStopActorFrameDraftV21(Enemy enemy){Actor a;if(Disposed||enemy==null||!actors.TryGetValue(enemy,out a)||!a.root)return null;return new CoreStopSourceActorFrameV21{actor=enemy,root=a.root,renderers=a.root.GetComponentsInChildren<Renderer>(false),state=a.state,pose=a.pose,renderedFrame=a.renderedFrame};}
  public string ActorPoseV21(Enemy e){Actor a;return actors.TryGetValue(e,out a)?a.pose:null;}
  public bool ActorPartsProjectedV21(Enemy e,HashSet<Transform> projected){Actor a;if(!actors.TryGetValue(e,out a)||!a.root||!a.root.activeInHierarchy)return false;int seen=0;foreach(var t in a.transforms){if(!t)continue;var r=t.GetComponent<MeshRenderer>();if(r&&r.enabled&&r.gameObject.activeInHierarchy){if(!projected.Contains(t))return false;seen++;}}return seen==a.transforms.Length&&seen>0;}
  public bool ActorVisibleV21(Enemy e){Actor actor;return actors.TryGetValue(e,out actor)&&actor.root&&actor.root.activeInHierarchy;}
  public bool TryActorBoundsV21(Enemy e,out Bounds bounds,out Vector3 root,out int parts){bounds=new Bounds();root=Vector3.zero;parts=0;Actor actor;if(!actors.TryGetValue(e,out actor)||!actor.root)return false;root=actor.root.transform.position;bool any=false;foreach(var t in actor.transforms){if(!t)continue;var r=t.GetComponent<MeshRenderer>();if(!r)continue;if(!any){bounds=r.bounds;any=true;}else bounds.Encapsulate(r.bounds);parts++;}return any;}
  public void ReleaseRoom(){foreach(var a in actors.Values){a.root.SetActive(false);foreach(var t in a.transforms)depths.Remove(t);foreach(var mesh in a.meshes)Remove(mesh);foreach(var m in a.materials)Remove(m);Remove(a.root);}actors.Clear();}
  public void Dispose(){if(Disposed)return;ReleaseRoom();identities.Clear();Disposed=true;}
 }
}

