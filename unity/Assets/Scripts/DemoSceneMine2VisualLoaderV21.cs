#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class Mine2VisualFilePinV21 {public string path,sha256;}
 [Serializable] public sealed class Mine2VisualTexturePinV21 {public string key,path,sha256;}
 [Serializable] public sealed class Mine2VisualBindingV21 {public string schema;public Mine2VisualFilePinV21 document,meshManifest;public Mine2VisualTexturePinV21[] textures;}
 public sealed partial class DemoScene {
  readonly List<Texture2D> mine2VisualLoadedTexturesV21=new List<Texture2D>();
  partial void DisposeMine2VisualExternalTexturesV21(){foreach(var t in mine2VisualLoadedTexturesV21)Mine2VisualDestroyV21(t);mine2VisualLoadedTexturesV21.Clear();}
  static byte[] Mine2VisualReadPinV21(Mine2VisualFilePinV21 p){Mine2VisualRequireV21(p!=null&&Path.IsPathRooted(p.path),"External source path must be absolute");var b=File.ReadAllBytes(p.path);Mine2VisualRequireV21(Mine2VisualHashV21(b)==p.sha256,"Changed external source file "+p.path);return b;}
  public void LoadMine2VisualBindingV21(string bindingPath){
   Mine2VisualRequireV21(Path.IsPathRooted(bindingPath),"Binding path must be absolute");var b=JsonUtility.FromJson<Mine2VisualBindingV21>(File.ReadAllText(bindingPath));Mine2VisualRequireV21(b!=null&&b.schema=="mine2-source-visual-binding-v21"&&b.textures!=null,"Invalid external Mine2 binding");
   var d=JsonUtility.FromJson<Mine2VisualDocumentV21>(Encoding.UTF8.GetString(Mine2VisualReadPinV21(b.document)));var meshes=JsonUtility.FromJson<OutskirtsNpcMeshManifest>(Encoding.UTF8.GetString(Mine2VisualReadPinV21(b.meshManifest)));var textures=new Dictionary<string,Texture2D>();var owned=new List<Texture2D>();
   try{foreach(var r in b.textures){var bytes=Mine2VisualReadPinV21(new Mine2VisualFilePinV21{path=r.path,sha256=r.sha256});var t=new Texture2D(2,2,TextureFormat.RGBA32,false);owned.Add(t);Mine2VisualRequireV21(ImageConversion.LoadImage(t,bytes,false),"Original source PNG load failed");t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;textures.Add(r.key,t);}ConfigureMine2VisualV21(d,meshes,textures);mine2VisualLoadedTexturesV21.AddRange(owned);owned.Clear();}
   finally{foreach(var t in owned)Mine2VisualDestroyV21(t);}
  }
 }
}
#endif
