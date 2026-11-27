using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using ElswordPixel.CargoDirectorControlledPrivateV21.Clock;
namespace ElswordPixel.CargoDirectorControlledPrivateV21.View {
 // Standalone draft view service: no DemoScene/shared hooks, no enemy/HP/AI
 // writes. Host owns source STOP/input, captions/audio and model dispatch.
 public sealed partial class CargoDirector2DViewV21:IDisposable {
  readonly Camera camera;readonly DirectorViewSpecV21 data;readonly GameObject root;readonly SpriteRenderer human,armor;readonly List<Texture2D> textures=new List<Texture2D>();readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>();bool captured;Vector3 previousPosition;Quaternion previousRotation;bool previousOrtho;float previousSize,previousAspect;
  static string Sha(byte[] raw){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(raw)).Replace("-","").ToLowerInvariant();}
  static void Free(UnityEngine.Object o){if(!o)return;if(Application.isPlaying)UnityEngine.Object.Destroy(o);else UnityEngine.Object.DestroyImmediate(o);}
  public CargoDirector2DViewV21(Camera target,Transform parent,DirectorViewSpecV21 spec){
   if(!target||spec==null)throw new ArgumentNullException("camera/spec");if(parent&&(parent.lossyScale-Vector3.one).sqrMagnitude>1e-8f)throw new InvalidOperationException("Director actor parent must preserve source world scale1");spec.Validate();camera=target;data=spec;root=new GameObject("Authored Cargo director2D / two complete actors");root.transform.SetParent(parent,false);human=Child("Human Wally4mesh complete");armor=Child("Armor6mesh complete");
   try{foreach(var input in data.assets){byte[] raw=File.ReadAllBytes(input.path);if(Sha(raw)!=input.sha256)throw new InvalidOperationException("Pinned director texture changed: "+input.path);var t=new Texture2D(2,2,TextureFormat.RGBA32,false,false);if(!ImageConversion.LoadImage(t,raw,false))throw new InvalidOperationException("Cannot load whole director atlas");t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;textures.Add(t);}foreach(var p in data.poses){var t=textures[p.assetIndex];if(p.rect[0]<0||p.rect[1]<0||p.rect[0]+p.rect[2]>t.width||p.rect[1]+p.rect[3]>t.height)throw new InvalidOperationException("Director rect outside exact atlas");var s=Sprite.Create(t,new Rect(p.rect[0],t.height-p.rect[1]-p.rect[3],p.rect[2],p.rect[3]),new Vector2(p.anchor[0]/(float)p.rect[2],1-p.anchor[1]/(float)p.rect[3]),1,0,SpriteMeshType.FullRect);s.name=p.key;sprites.Add(p.key,s);}HideAndRestore();}catch{Dispose();throw;}
  }
  SpriteRenderer Child(string name){var go=new GameObject(name);go.transform.SetParent(root.transform,false);var r=go.AddComponent<SpriteRenderer>();r.color=Color.white;return r;}
  public void Apply(DirectorPhaseV21 phase,double time,Vector2 sharedStageOrigin){
   if(phase!=DirectorPhaseV21.Start){HideAndRestore();return;}var f=data.Sample(time);if(f==null){HideAndRestore();return;}if(!captured){previousPosition=camera.transform.position;previousRotation=camera.transform.rotation;previousOrtho=camera.orthographic;previousSize=camera.orthographicSize;previousAspect=camera.aspect;captured=true;}
   camera.orthographic=true;camera.aspect=data.viewportWidth/(float)data.viewportHeight;camera.orthographicSize=(float)f.height*.5f;camera.transform.position=new Vector3((float)f.center[0]+sharedStageOrigin.x,(float)f.center[1]+sharedStageOrigin.y,previousPosition.z);camera.transform.rotation=Quaternion.identity;human.enabled=armor.enabled=true;human.sprite=sprites[f.humanKey];armor.sprite=sprites[f.armorKey];human.transform.position=new Vector3((float)f.humanRoot[0]+sharedStageOrigin.x,(float)f.humanRoot[1]+sharedStageOrigin.y,0);armor.transform.position=new Vector3((float)f.armorRoot[0]+sharedStageOrigin.x,(float)f.armorRoot[1]+sharedStageOrigin.y,0);human.transform.localScale=armor.transform.localScale=Vector3.one;human.transform.rotation=armor.transform.rotation=Quaternion.identity;human.flipX=armor.flipX=false;human.flipY=armor.flipY=false;human.sortingOrder=f.humanInFront?42:40;armor.sortingOrder=41;
  }
  public void HideAndRestore(){if(human)human.enabled=false;if(armor)armor.enabled=false;if(captured&&camera){camera.transform.position=previousPosition;camera.transform.rotation=previousRotation;camera.orthographic=previousOrtho;camera.orthographicSize=previousSize;camera.aspect=previousAspect;}captured=false;}
  public void Dispose(){HideAndRestore();foreach(var s in sprites.Values)Free(s);sprites.Clear();foreach(var t in textures)Free(t);textures.Clear();Free(root);}
 }
}
