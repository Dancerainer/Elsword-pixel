using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ElswordPixel.CargoPresentationV21 {
 [Serializable] public sealed class CargoFilePinV21 {public string path,sha256;}
 [Serializable] public sealed class CargoTexturePinV21 {public string key,path,sha256;}
 [Serializable] public sealed class CargoVisualBindingV21 {public string schema;public bool runtimeAdmitted,actualUnityVerified,nativeEquivalent;public CargoFilePinV21 document,meshManifest,sourceEffects,sourceRules;public CargoTexturePinV21[] textures;}
 public sealed partial class CargoFxViewV21 {
  readonly CargoFxFeedV21 game;Transform worldRoot,effectsRoot;readonly Shader shader;readonly Dictionary<Transform,float> npcEffectDepths=new Dictionary<Transform,float>();readonly List<Material> outskirtsMaterials=new List<Material>();readonly List<Texture2D> loadedTextures=new List<Texture2D>();
  public CargoFxViewV21(CargoFxFeedV21 feed,Transform root,Shader sourceShader){if(feed==null||!root||!sourceShader||sourceShader.name!="ElswordPixel/CargoSourceFxV21"||!sourceShader.isSupported)throw new ArgumentException("Cargo feed/unitRoot/exact shader required");if((root.lossyScale-Vector3.one).sqrMagnitude>1e-8f)throw new ArgumentException("Cargo effect root must preserve worldscale1");game=feed;worldRoot=effectsRoot=root;shader=sourceShader;}
  static bool SteamFinite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}static bool SteamVector(float[] a,int count){return a!=null&&a.Length==count&&Array.TrueForAll(a,SteamFinite);}static void RequireOutskirtsVisual(bool b,string s){if(!b)throw new InvalidOperationException(s);}
  Material OutskirtsMaterial(string src,string dst){int d=dst=="D3DBLEND_INVSRCALPHA"?10:dst=="D3DBLEND_DESTALPHA"?7:dst=="D3DBLEND_ONE"?1:-1;if(src!="D3DBLEND_SRCALPHA"||d<0)throw new ArgumentException("Unsupported original Cargo source blend");var m=new Material(shader){name="Exact Cargo original particle blend"};m.SetInt("_SrcBlend",5);m.SetInt("_DstBlend",d);outskirtsMaterials.Add(m);return m;}
  SpriteRenderer WorldSprite(string name,Transform parent,Sprite sprite,int order,Vector3 pos){var go=new GameObject(name);go.transform.SetParent(parent,false);go.layer=parent.gameObject.layer;var r=go.AddComponent<SpriteRenderer>();r.sprite=sprite;r.sortingOrder=order;r.transform.localPosition=pos;return r;}
  void RemoveSceneObject(GameObject go){CargoVisualDestroyV21(go);}
  static byte[] ReadPin(CargoFilePinV21 p){if(p==null||!Path.IsPathRooted(p.path))throw new ArgumentException("Absolute source file pin required");byte[] b=File.ReadAllBytes(p.path);if(CargoVisualHashV21(b)!=p.sha256)throw new InvalidOperationException("Changed Cargo source file "+p.path);return b;}
  public void LoadBinding(string file,string expectedSha256){var b=JsonUtility.FromJson<CargoVisualBindingV21>(Encoding.UTF8.GetString(ReadPin(new CargoFilePinV21{path=file,sha256=expectedSha256})));if(b==null||b.schema!="cargo-source-visual-binding-v21"||b.runtimeAdmitted||b.actualUnityVerified||b.nativeEquivalent||b.textures==null)throw new ArgumentException("Wrong Cargo presentation binding");byte[] source=ReadPin(b.sourceEffects);ReadPin(b.sourceRules);if(CargoVisualHashV21(source)!=CargoFxFeedV21.ExactEffectsSha256)throw new ArgumentException("Same name with wrong Cargo source definition");var doc=JsonUtility.FromJson<CargoVisualDocumentV21>(Encoding.UTF8.GetString(ReadPin(b.document)));var manifest=JsonUtility.FromJson<OutskirtsNpcMeshManifest>(Encoding.UTF8.GetString(ReadPin(b.meshManifest)));var textures=new Dictionary<string,Texture2D>();var owned=new List<Texture2D>();try{foreach(var p in b.textures){byte[] bytes=ReadPin(new CargoFilePinV21{path=p.path,sha256=p.sha256});var t=new Texture2D(2,2,TextureFormat.RGBA32,false,false);owned.Add(t);if(!ImageConversion.LoadImage(t,bytes,false))throw new InvalidOperationException("Cannot decode source texture");t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;textures.Add(p.key,t);}ConfigureCargoVisualV21(doc,manifest,textures);loadedTextures.AddRange(owned);owned.Clear();}finally{foreach(var t in owned)CargoVisualDestroyV21(t);}}
  partial void DisposeCargoVisualExternalTexturesV21(){foreach(var t in loadedTextures)CargoVisualDestroyV21(t);loadedTextures.Clear();}
  public void Render(){RenderCargoVisualV21();foreach(var p in npcEffectDepths)if(p.Key){var v=p.Key.localPosition;v.z=-p.Value*.001f;p.Key.localPosition=v;}}
  public void ReleaseRoom(){ClearCargoVisualInstancesV21();ClearCargoVisualPoolV21();cargoVisualGenerationV21=-1;cargoVisualEventCursorV21=cargoVisualStateCursorV21=0;}
  public void SetRoot(Transform root){if(!root||(root.lossyScale-Vector3.one).sqrMagnitude>1e-8f)throw new ArgumentException("Unit root required");worldRoot=effectsRoot=root;}
  public void Dispose(){DisposeCargoVisualV21();}
 }
}
