using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
 [Serializable] public sealed class Mine2WorldLayerDocumentV20 {
  public string schema,worldId,sourceLuaSha256,sourceTextureSha256,sourceIRJson,sourceIRSha256,rgbaSha256,sourceLayer;
  public string viewportAdapter,blendAdapter,samplerAdapter;
  public int roomIndex,textureWidth,textureHeight,sourceLayerId,sourceBlendType;
  public float[] sourceSetPos,sourceColor,sourceTexUV,sourceFeedBackUV;
  public bool sourceAlphaObject,sourceInitShow,nativeSetPosMeasured,nativeBlendTypeMeasured,nativeLayerNumericOrderMeasured;
  public bool sourceMeshDeclared,actualUnityComponentVerified,runtimeAdmitted;
 }
 public sealed partial class DemoScene {
  Mine2WorldLayerDocumentV20 mine2WorldLayerDocV20;
  Texture2D mine2WorldLayerTextureV20;
  RectTransform mine2WorldLayerViewportV20;
  RawImage mine2WorldLayerImageV20;
  bool mine2WorldLayerEnabledV20;
  int mine2WorldLayerEpochV20=-1;
  public int Mine2WorldLayerAllocationsV20 {get;private set;}
  public int Mine2WorldLayerDrawFramesV20 {get;private set;}
  public int Mine2WorldLayerViewportHoldsV20 {get;private set;}
  public int Mine2WorldLayerWorldEpochsObservedV20 {get;private set;}
  public bool Mine2WorldLayerVisibleV20 {get{return mine2WorldLayerImageV20&&mine2WorldLayerImageV20.gameObject.activeSelf;}}
  public RawImage Mine2WorldLayerImageV20 {get{return mine2WorldLayerImageV20;}}
  static void Mine2LayerRequireV20(bool value,string why){if(!value)throw new InvalidOperationException("Mine2 source world layer: "+why);}
  static string Mine2LayerHashV20(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  static string Mine2LayerRGBAHashV20(Texture2D texture){var pixels=texture.GetPixels32();var bytes=new byte[pixels.Length*4];int n=0;for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){var p=pixels[y*texture.width+x];bytes[n++]=p.r;bytes[n++]=p.g;bytes[n++]=p.b;bytes[n++]=p.a;}return Mine2LayerHashV20(bytes);}
  static bool Mine2LayerArrayV20(float[] a,params float[] b){if(a==null||a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
  void RequireMine2LayerViewportV20(RectTransform viewport){
   Mine2LayerRequireV20(viewport&&viewport.GetComponentInParent<Canvas>(),"Missing caller-supplied dedicated UI world-viewport basis; never infer a HUD/terrain rectangle");
   Mine2LayerRequireV20(viewport.childCount==0||(viewport.childCount==1&&mine2WorldLayerImageV20&&ReferenceEquals(mine2WorldLayerImageV20.transform.parent,viewport)),"Viewport slot is not dedicated to this source layer");
  }
  public void ConfigureMine2WorldLayerV20(Mine2WorldLayerDocumentV20 doc,Texture2D texture,RectTransform dedicatedWorldViewport){
   if(doc==null){DisposeMine2WorldLayerV20();return;}
   Mine2LayerRequireV20(doc.schema=="mine2-source-worldlayer-display-v20-r1"&&doc.worldId=="WI_BESMA_MINE2_STAGE1"&&doc.roomIndex==0&&!doc.runtimeAdmitted&&!doc.actualUnityComponentVerified&&!doc.sourceMeshDeclared,"Changed source scope/admission/undeclared mesh");
   Mine2LayerRequireV20(doc.sourceLuaSha256==Mine2WorldLayerPinsV20.SourceLua&&doc.sourceTextureSha256==Mine2WorldLayerPinsV20.SourceTexture&&doc.sourceIRSha256==Mine2WorldLayerPinsV20.SourceIR&&Mine2LayerHashV20(Encoding.UTF8.GetBytes(doc.sourceIRJson??""))==Mine2WorldLayerPinsV20.SourceIR&&doc.rgbaSha256==Mine2WorldLayerPinsV20.Rgba,"Changed original Lua/setterIR/texture pins");
   Mine2LayerRequireV20(doc.sourceLayer=="X2_LAYER.XL_SKY_WORLD_OBJECT_OVER_EFFECT_0"&&doc.sourceLayerId==0&&doc.sourceBlendType==0&&doc.sourceAlphaObject&&doc.sourceInitShow&&Mine2LayerArrayV20(doc.sourceSetPos,0,0,1,1)&&Mine2LayerArrayV20(doc.sourceColor,.4f,.8f,1,.568627f)&&Mine2LayerArrayV20(doc.sourceTexUV,0,0)&&Mine2LayerArrayV20(doc.sourceFeedBackUV,0,0),"Changed original ten setter arguments");
   Mine2LayerRequireV20(!doc.nativeSetPosMeasured&&!doc.nativeBlendTypeMeasured&&!doc.nativeLayerNumericOrderMeasured&&doc.viewportAdapter=="caller-dedicated-world-viewport-full-normalized-ui-quad-v20"&&doc.blendAdapter=="source-blendtype0-unmeasured-UI-Default-straight-alpha-display-v20"&&doc.samplerAdapter=="source-original-RGBA-Point-Clamp-no-color-repaint-v20","Unsupported or falsely measured display basis/blend/order");
   Mine2LayerRequireV20(texture&&texture.isReadable&&texture.width==256&&texture.height==256&&doc.textureWidth==256&&doc.textureHeight==256&&Mine2LayerRGBAHashV20(texture)==Mine2WorldLayerPinsV20.Rgba,"Missing/changed original256RGBA");
   RequireMine2LayerViewportV20(dedicatedWorldViewport);
   DisposeMine2WorldLayerV20();mine2WorldLayerDocV20=JsonUtility.FromJson<Mine2WorldLayerDocumentV20>(JsonUtility.ToJson(doc));mine2WorldLayerTextureV20=texture;mine2WorldLayerViewportV20=dedicatedWorldViewport;mine2WorldLayerEnabledV20=true;
   texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;Mine2WorldLayerAllocationsV20=Mine2WorldLayerDrawFramesV20=Mine2WorldLayerViewportHoldsV20=Mine2WorldLayerWorldEpochsObservedV20=0;
  }
  public void BindMine2WorldLayerViewportV20(RectTransform dedicatedWorldViewport){RequireMine2LayerViewportV20(dedicatedWorldViewport);if(ReferenceEquals(mine2WorldLayerViewportV20,dedicatedWorldViewport))return;DestroyMine2LayerImageV20();mine2WorldLayerViewportV20=dedicatedWorldViewport;}
  void DestroyMine2LayerImageV20(){if(mine2WorldLayerImageV20){var go=mine2WorldLayerImageV20.gameObject;if(Application.isPlaying)Destroy(go);else DestroyImmediate(go);}mine2WorldLayerImageV20=null;}
  public void DisposeMine2WorldLayerV20(){DestroyMine2LayerImageV20();mine2WorldLayerDocV20=null;mine2WorldLayerTextureV20=null;mine2WorldLayerViewportV20=null;mine2WorldLayerEnabledV20=false;mine2WorldLayerEpochV20=-1;}
  public void SetMine2WorldLayerEnabledV20(bool enabled){mine2WorldLayerEnabledV20=enabled;if(!enabled&&mine2WorldLayerImageV20)mine2WorldLayerImageV20.gameObject.SetActive(false);}
  public void RenderMine2WorldLayerV20(){
   var doc=mine2WorldLayerDocV20;if(doc==null)return;
   bool visible=mine2WorldLayerEnabledV20&&doc.sourceInitShow&&game.room==doc.roomIndex&&nativeScene!=null&&nativeScene.id==doc.worldId;
   if(!visible){if(mine2WorldLayerImageV20)mine2WorldLayerImageV20.gameObject.SetActive(false);return;}
   if(!mine2WorldLayerViewportV20){if(mine2WorldLayerImageV20)mine2WorldLayerImageV20.gameObject.SetActive(false);Mine2WorldLayerViewportHoldsV20++;return;}
   RequireMine2LayerViewportV20(mine2WorldLayerViewportV20);
   if(!mine2WorldLayerImageV20||!ReferenceEquals(mine2WorldLayerImageV20.transform.parent,mine2WorldLayerViewportV20)){
    DestroyMine2LayerImageV20();var go=new GameObject("Original Mine2 CreateWorldLayer / besma_secret_red / sourceLayerId0",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));go.transform.SetParent(mine2WorldLayerViewportV20,false);mine2WorldLayerImageV20=go.GetComponent<RawImage>();var r=(RectTransform)go.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;
    mine2WorldLayerImageV20.texture=mine2WorldLayerTextureV20;mine2WorldLayerImageV20.color=new Color(.4f,.8f,1,.568627f);mine2WorldLayerImageV20.uvRect=new Rect(0,0,1,1);mine2WorldLayerImageV20.raycastTarget=false;mine2WorldLayerImageV20.maskable=false;Mine2WorldLayerAllocationsV20++;
   }
   mine2WorldLayerImageV20.gameObject.SetActive(true);if(mine2WorldLayerEpochV20!=game.NpcSoundWorldSerial){mine2WorldLayerEpochV20=game.NpcSoundWorldSerial;Mine2WorldLayerWorldEpochsObservedV20++;}Mine2WorldLayerDrawFramesV20++;
  }
 }
}
