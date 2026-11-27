using System;
using UnityEngine;
namespace ElswordPixel.CargoDirectorControlledPrivateV21.View {
 public sealed partial class CargoDirector2DViewV21 {
  // Ordinary Host owns these exact textures. This view owns only its root and sprites.
  public CargoDirector2DViewV21(Camera target,Transform parent,DirectorViewSpecV21 spec,Texture2D[] pinnedHostTextures){
   if(!target||spec==null||pinnedHostTextures==null||pinnedHostTextures.Length!=spec.assets.Length)throw new ArgumentNullException("camera/spec/pinnedHostTextures");if(parent&&(parent.lossyScale-Vector3.one).sqrMagnitude>1e-8f)throw new InvalidOperationException("Director actor parent must preserve source world scale1");spec.Validate();camera=target;data=spec;root=new GameObject("Authored Cargo director2D / two complete actors");
   try{root.transform.SetParent(parent,false);human=Child("Human Wally4mesh complete");armor=Child("Armor6mesh complete");foreach(var p in data.poses){var t=pinnedHostTextures[p.assetIndex];if(!t||t.filterMode!=FilterMode.Point||t.wrapMode!=TextureWrapMode.Clamp||t.mipmapCount!=1||p.rect[0]<0||p.rect[1]<0||p.rect[2]<=0||p.rect[3]<=0||p.rect[0]+p.rect[2]>t.width||p.rect[1]+p.rect[3]>t.height)throw new InvalidOperationException("Director exact Host atlas/rect changed");var s=Sprite.Create(t,new Rect(p.rect[0],t.height-p.rect[1]-p.rect[3],p.rect[2],p.rect[3]),new Vector2(p.anchor[0]/(float)p.rect[2],1-p.anchor[1]/(float)p.rect[3]),1,0,SpriteMeshType.FullRect);s.name=p.key;sprites.Add(p.key,s);}HideAndRestore();}catch{Dispose();throw;}
  }
 }
}
