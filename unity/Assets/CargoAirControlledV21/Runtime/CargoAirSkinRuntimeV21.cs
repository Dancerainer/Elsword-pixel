using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel.CargoAirControlledPrivateV21 {
 // Source-specific material consumer. It never writes original texture pixels,
 // animation clocks, source physics, Sprite geometry, anchors, tint or flipX.
 public sealed class CargoAirSkinRuntimeV21 : IDisposable {
  sealed class Binding { public Texture2D original,index,palette;public Material material; }
  readonly Dictionary<string,Binding> bindings=new Dictionary<string,Binding>();
  readonly Dictionary<SpriteRenderer,Material> previous=new Dictionary<SpriteRenderer,Material>();
  readonly Shader shader;
  public CargoAirSkinRuntimeV21(Shader paletteShader){if(!paletteShader)throw new ArgumentNullException("paletteShader");shader=paletteShader;}
  public void Register(string exactNPC,Texture2D originalApprovedAtlas,Texture2D linearIndexControl,Texture2D srgbPalette){
   if(!CargoAirPaletteLookupV21.IsExactAir(exactNPC))throw new ArgumentException("Only exact Cargo AIR source NPCs may register");
   if(bindings.ContainsKey(exactNPC))throw new InvalidOperationException("Duplicate AIR material binding: "+exactNPC);
   if(!originalApprovedAtlas||!linearIndexControl||!srgbPalette)throw new ArgumentNullException("Missing AIR original/control/palette texture");
   if(originalApprovedAtlas.width!=linearIndexControl.width||originalApprovedAtlas.height!=linearIndexControl.height||srgbPalette.width!=256||srgbPalette.height!=256)throw new InvalidOperationException("AIR LUT dimensions do not match pinned original atlas");
   if(originalApprovedAtlas.filterMode!=FilterMode.Point||linearIndexControl.filterMode!=FilterMode.Point||srgbPalette.filterMode!=FilterMode.Point)throw new InvalidOperationException("AIR material requires Point filtering");
   if(linearIndexControl.mipmapCount!=1||srgbPalette.mipmapCount!=1||linearIndexControl.wrapMode!=TextureWrapMode.Clamp||srgbPalette.wrapMode!=TextureWrapMode.Clamp)throw new InvalidOperationException("AIR control/palette require Clamp and no mipmaps");
   var material=new Material(shader);material.name="Cargo AIR exact source skin / "+exactNPC;material.SetTexture("_AirIndexTex",linearIndexControl);material.SetTexture("_AirPaletteTex",srgbPalette);bindings.Add(exactNPC,new Binding{original=originalApprovedAtlas,index=linearIndexControl,palette=srgbPalette,material=material});
  }
  public bool Apply(string exactNPC,SpriteRenderer renderer){
   if(!renderer)throw new ArgumentNullException("renderer");
   if(!CargoAirPaletteLookupV21.IsExactAir(exactNPC)){Restore(renderer);return false;}
   Binding b;if(!bindings.TryGetValue(exactNPC,out b))throw new InvalidOperationException("Required exact AIR material not registered: "+exactNPC);
   if(!renderer.sprite||renderer.sprite.texture!=b.original)throw new InvalidOperationException("AIR material rejected wrong/unpinned source atlas: "+exactNPC);
   if(!previous.ContainsKey(renderer))previous.Add(renderer,renderer.sharedMaterial);renderer.sharedMaterial=b.material;return true;
  }
  public void Restore(SpriteRenderer renderer){Material material;if(renderer&&previous.TryGetValue(renderer,out material)){renderer.sharedMaterial=material;previous.Remove(renderer);}}
  // Invoke when releasing room views, before reusing/destroying them. The same
  // registered skins remain available for the next room without stale owners.
  public void ReleaseRoom(){foreach(var p in previous)if(p.Key)p.Key.sharedMaterial=p.Value;previous.Clear();}
  public void Dispose(){ReleaseRoom();foreach(var b in bindings.Values)if(b.material)UnityEngine.Object.Destroy(b.material);bindings.Clear();}
 }
}
