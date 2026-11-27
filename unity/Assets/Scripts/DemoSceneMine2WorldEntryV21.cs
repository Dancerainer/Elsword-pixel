#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class Mine2EntryWorldBindingV21 {public string schema;public PreviewFile document,texture,displayQuad;public PreviewFile[] evidence;public bool runtimeAdmitted;}
  Texture2D mine2EntryWorldTextureV21;RectTransform mine2EntryWorldViewportV21;
  static byte[] ReadMine2EntryWorldV21(PreviewFile pin){if(pin==null||!Path.IsPathRooted(pin.path))throw new InvalidOperationException("Mine2 external world pin absent");var bytes=File.ReadAllBytes(pin.path);if(Mine2LayerHashV20(bytes)!=pin.sha256)throw new InvalidOperationException("Mine2 changed world dependency "+pin.path);return bytes;}
  partial void ConfigureMine2EntryWorldRuntimeV21(PreviewFile binding,ref bool configured){
   var b=JsonUtility.FromJson<Mine2EntryWorldBindingV21>(Encoding.UTF8.GetString(ReadMine2EntryWorldV21(binding)));if(b.schema!="mine2-original-worldlayer-display-binding-v20-r1"||b.runtimeAdmitted||b.evidence==null||b.evidence.Length<3)throw new InvalidOperationException("Mine2 world binding scope");
   ReadMine2EntryWorldV21(b.displayQuad);foreach(var pin in b.evidence)ReadMine2EntryWorldV21(pin);var doc=JsonUtility.FromJson<Mine2WorldLayerDocumentV20>(Encoding.UTF8.GetString(ReadMine2EntryWorldV21(b.document)));var bytes=ReadMine2EntryWorldV21(b.texture);
   DisposeMine2EntryWorldRuntimeV21();if(!hudCanvas||!worldCamera)throw new InvalidOperationException("Mine2 world viewport requires initialized HUD canvas/camera");
   var go=new GameObject("Mine2 dedicated world camera viewport / before HUD",typeof(RectTransform));go.transform.SetParent(hudCanvas.transform,false);go.transform.SetAsFirstSibling();mine2EntryWorldViewportV21=go.GetComponent<RectTransform>();mine2EntryWorldViewportV21.localScale=Vector3.one;mine2EntryWorldViewportV21.localRotation=Quaternion.identity;UpdateMine2EntryWorldViewportV21();
   mine2EntryWorldTextureV21=new Texture2D(2,2,TextureFormat.RGBA32,false);try{if(!ImageConversion.LoadImage(mine2EntryWorldTextureV21,bytes,false))throw new InvalidOperationException("Mine2 original layer PNG decode");ConfigureMine2WorldLayerV20(doc,mine2EntryWorldTextureV21,mine2EntryWorldViewportV21);configured=true;}catch{DisposeMine2EntryWorldRuntimeV21();throw;}
  }
  void UpdateMine2EntryWorldViewportV21(){if(!mine2EntryWorldViewportV21||!worldCamera)return;var rect=worldCamera.rect;mine2EntryWorldViewportV21.anchorMin=rect.min;mine2EntryWorldViewportV21.anchorMax=rect.max;mine2EntryWorldViewportV21.offsetMin=mine2EntryWorldViewportV21.offsetMax=Vector2.zero;}
  void RenderMine2EntryWorldV21(){UpdateMine2EntryWorldViewportV21();RenderMine2WorldLayerV20();}
  partial void DisposeMine2EntryWorldRuntimeV21(){DisposeMine2WorldLayerV20();if(mine2EntryWorldViewportV21)DestroyNativeWorldObject(mine2EntryWorldViewportV21.gameObject);if(mine2EntryWorldTextureV21)DestroyNativeWorldObject(mine2EntryWorldTextureV21);mine2EntryWorldViewportV21=null;mine2EntryWorldTextureV21=null;}
 }
}
#endif
