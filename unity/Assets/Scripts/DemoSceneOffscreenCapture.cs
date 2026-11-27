using System;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool UseOffscreenCaptures {get{return Array.IndexOf(Environment.GetCommandLineArgs(),"--offscreen-captures")>=0;}}
  // Hidden D3D windows may not present a back buffer. Explicitly render the
  // same game camera and HUD for verification without changing simulation or
  // requiring a foreground window. Ordinary user screenshots keep their path.
  void CaptureOffscreenFrame(string file){
   int width=Math.Max(480,Screen.width),height=Math.Max(270,Screen.height);
   var target=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32){filterMode=FilterMode.Point,antiAliasing=1};
   var oldTarget=worldCamera.targetTexture;var oldActive=RenderTexture.active;
   var oldMode=hudCanvas.renderMode;var oldCamera=hudCanvas.worldCamera;float oldPlane=hudCanvas.planeDistance;int oldCanvasSortingOrder=hudCanvas.sortingOrder;
   Texture2D pixels=null;
   try{
    target.Create();worldCamera.targetTexture=target;
    hudCanvas.renderMode=RenderMode.ScreenSpaceCamera;hudCanvas.worldCamera=worldCamera;hudCanvas.planeDistance=Math.Max(worldCamera.nearClipPlane+.1f,1);hudCanvas.sortingOrder=short.MaxValue;
    Canvas.ForceUpdateCanvases();worldCamera.Render();RenderTexture.active=target;
    pixels=new Texture2D(width,height,TextureFormat.RGBA32,false);pixels.ReadPixels(new Rect(0,0,width,height),0,0);pixels.Apply(false,false);File.WriteAllBytes(file,pixels.EncodeToPNG());
   }finally{
    hudCanvas.sortingOrder=oldCanvasSortingOrder;hudCanvas.renderMode=oldMode;hudCanvas.worldCamera=oldCamera;hudCanvas.planeDistance=oldPlane;worldCamera.targetTexture=oldTarget;RenderTexture.active=oldActive;Canvas.ForceUpdateCanvases();
    if(pixels)Destroy(pixels);target.Release();Destroy(target);
   }
  }
 }
}
