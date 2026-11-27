using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  readonly Dictionary<Texture2D,Color32[]> nativeUiTexturePixelsV28=new Dictionary<Texture2D,Color32[]>();
  readonly Dictionary<Sprite,Rect> nativeUiVisibleLocalRectsV28=new Dictionary<Sprite,Rect>();
  // Current axis-aligned native-normal UI only; fail closed for rotation/shear.
  // UI only. Preserve the existing horizontal anchor, pixels, sprite pivot,
  // model root and all physical bounds. Empty union-crop space must not raise HP.
  float NativeVisibleTopYV28(SpriteRenderer view){
   if(AirshipWorldTopV40(view,out float soldierTop))return soldierTop;
   if(GiantWilliamWorldTopV39(view,out float giantWilliamTop))return giantWilliamTop;
   if(view && mageWitnessV38.TryGetValue(view,out var mage) && ReferenceEquals(mage.selected,view.sprite))return view.bounds.max.y;
   if(!view||!view.sprite||view.drawMode!=SpriteDrawMode.Simple)throw new InvalidOperationException("A simple native sprite is required for visible-top UI");
   Vector3 scale=view.transform.lossyScale,axisX=view.transform.TransformVector(Vector3.right);
   if(float.IsNaN(scale.x)||float.IsInfinity(scale.x)||float.IsNaN(scale.y)||float.IsInfinity(scale.y)||float.IsNaN(scale.z)||float.IsInfinity(scale.z)||float.IsNaN(axisX.y)||float.IsInfinity(axisX.y)||axisX.y!=0)
    throw new InvalidOperationException("Visible-top UI supports only finite current axis-aligned native-normal sprites");
   Sprite sprite=view.sprite;Rect local;
   if(!nativeUiVisibleLocalRectsV28.TryGetValue(sprite,out local)){
    Texture2D texture=sprite.texture;
    if(!texture||!texture.isReadable){
     string details=texture?texture.name+" "+texture.width+"x"+texture.height+" "+texture.format+" readable="+texture.isReadable:"<missing>";
     throw new InvalidOperationException("Native UI texture must retain validated readable RGBA pixels, sprite="+sprite.name+", texture="+details);
    }
    int stride=texture.width,textureHeight=texture.height;
    // Borrow the readable RGBA mip only for this synchronous scan. Other source
    // formats need Unity's color conversion to preserve their decoded alpha.
    NativeArray<Color32> rgba=default;
    Color32[] converted=null;
    if(texture.format==TextureFormat.RGBA32)rgba=texture.GetPixelData<Color32>(0);
    else if(!nativeUiTexturePixelsV28.TryGetValue(texture,out converted)){
     converted=texture.GetPixels32();nativeUiTexturePixelsV28.Add(texture,converted);
    }
    Rect rect=sprite.rect;int rx=Mathf.RoundToInt(rect.x),ry=Mathf.RoundToInt(rect.y),w=Mathf.RoundToInt(rect.width),h=Mathf.RoundToInt(rect.height);
    int pixelCount=converted==null?rgba.Length:converted.Length;
    if(Mathf.Abs(rx-rect.x)>.0001f||Mathf.Abs(ry-rect.y)>.0001f||Mathf.Abs(w-rect.width)>.0001f||Mathf.Abs(h-rect.height)>.0001f||w<=0||h<=0||rx<0||ry<0||rx+w>stride||ry+h>textureHeight||pixelCount!=stride*textureHeight)throw new InvalidOperationException("Native sprite rectangle must stay on the validated pixel lattice");
    int minX=w,minY=h,maxX=-1,maxY=-1;
    for(int y=0;y<h;y++){
     int row=(ry+y)*stride+rx;
     for(int x=0;x<w;x++){
      byte alpha=converted==null?rgba[row+x].a:converted[row+x].a;
      if(alpha!=0){minX=Math.Min(minX,x);minY=Math.Min(minY,y);maxX=Math.Max(maxX,x);maxY=Math.Max(maxY,y);}
     }
    }
    if(maxX<0||maxY<0||(sprite.pixelsPerUnit<=0||float.IsNaN(sprite.pixelsPerUnit)||float.IsInfinity(sprite.pixelsPerUnit)))throw new InvalidOperationException("Native UI sprite has no visible source coverage");
    Vector2 pivot=sprite.pivot;float ppu=sprite.pixelsPerUnit;
    local=Rect.MinMaxRect((minX-pivot.x)/ppu,(minY-pivot.y)/ppu,(maxX+1-pivot.x)/ppu,(maxY+1-pivot.y)/ppu);nativeUiVisibleLocalRectsV28.Add(sprite,local);
   }
   float top=float.NegativeInfinity;
   for(int i=0;i<4;i++){
    float x=(i&1)==0?local.xMin:local.xMax,y=(i&2)==0?local.yMin:local.yMax;
    if(view.flipX)x=-x;if(view.flipY)y=-y;
    Vector3 point=view.transform.TransformPoint(new Vector3(x,y,0));
    if(float.IsNaN(point.x)||float.IsInfinity(point.x)||float.IsNaN(point.y)||float.IsInfinity(point.y)||float.IsNaN(point.z)||float.IsInfinity(point.z))throw new InvalidOperationException("Nonfinite native UI transform");
    top=Mathf.Max(top,point.y);
   }
   if(float.IsNaN(top)||float.IsInfinity(top))throw new InvalidOperationException("Nonfinite native UI top");
   return top;
  }
 }
}
