using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 // Authored pixel warning UI only. This is never an original particle/mesh,
 // director body, native callback proof, or substitute whole-character art.
 public sealed class FactoryPressWarningViewV569:IDisposable {
  readonly Transform parent;readonly Texture2D texture;readonly Sprite sprite;readonly Material material;
  readonly Dictionary<Enemy,SpriteRenderer> owned=new Dictionary<Enemy,SpriteRenderer>();GameModel model;int epoch=-1;bool disposed;
  public FactoryPressWarningViewV569(Transform ownerRoot,int sortingOrder=38){
   if(ownerRoot==null)throw new ArgumentNullException("ownerRoot");parent=ownerRoot;Order=sortingOrder;
   try{
    texture=new Texture2D(1,1,TextureFormat.RGBA32,false);texture.name="FactoryPressWarningUIOwnedV569";texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;texture.SetPixel(0,0,Color.white);texture.Apply(false,false);
    sprite=Sprite.Create(texture,new Rect(0,0,1,1),new Vector2(.5f,.5f),1);var shader=Shader.Find("Sprites/Default");if(shader==null)throw new InvalidOperationException("Sprites/Default unavailable");material=new Material(shader);
   }catch{Drop(material);Drop(sprite);Drop(texture);throw;}
  }
  public int Order{get;private set;}public int OwnedWarningCount{get{return owned.Count;}}
  static void Drop(UnityEngine.Object value){if(value==null)return;if(Application.isPlaying)UnityEngine.Object.Destroy(value);else UnityEngine.Object.DestroyImmediate(value);}
  void Clear(){foreach(var r in owned.Values)if(r!=null)Drop(r.gameObject);owned.Clear();}
  public void Bind(GameModel value){if(disposed)throw new ObjectDisposedException("FactoryPressWarningViewV569");if(!ReferenceEquals(model,value)){Clear();model=value;epoch=-1;}}
  public void Sync(){
   if(disposed)throw new ObjectDisposedException("FactoryPressWarningViewV569");if(model==null){Clear();return;}if(epoch!=model.FactoryPressEpochV569){Clear();epoch=model.FactoryPressEpochV569;}
   var live=new HashSet<Enemy>();foreach(var f in model.ReadFactoryPressFramesV569()){
    live.Add(f.actualOwner);SpriteRenderer r;if(!owned.TryGetValue(f.actualOwner,out r)){var go=new GameObject("Factory Press windup warning");try{go.transform.SetParent(parent,false);r=go.AddComponent<SpriteRenderer>();r.sprite=sprite;r.sharedMaterial=material;r.sortingOrder=Order;owned.Add(f.actualOwner,r);}catch{Drop(go);throw;}}
    r.enabled=f.warningRemaining>0;r.color=new Color(1,(Mathf.FloorToInt(f.stateAge*8)&1)==0?.75f:.35f,.15f,.8f);r.transform.localPosition=new Vector3(Mathf.Round((f.warningLeft+f.warningRight)*.5f),Mathf.Round(f.warningY)+2,0);r.transform.localScale=new Vector3(Mathf.Max(1,Mathf.Round(f.warningRight-f.warningLeft)),2,1);
   }
   var stale=new List<Enemy>();foreach(var e in owned.Keys)if(!live.Contains(e))stale.Add(e);foreach(var e in stale){Drop(owned[e].gameObject);owned.Remove(e);}
  }
  public void ReleaseRoom(){Clear();epoch=-1;}
  public void Dispose(){if(disposed)return;Clear();model=null;Drop(material);Drop(sprite);Drop(texture);disposed=true;}
 }
}
