using System;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;
namespace ElswordPixel {
 [Serializable] public sealed class BattleProductWorldLayerRowR1325 {public string world,id,decodedRgbaSha256;public float[] sourcePositionParameters,sourceColor,sourceTexUV,sourceFeedbackUV;public bool nativeLayerProjectionMeasured,runtimeConsumerImplemented;}
 [Serializable] public sealed class BattleProductWorldLayerDocumentR1325 {public string schema;public BattleProductWorldLayerRowR1325[] rows;}
 public sealed partial class DemoScene {
  BattleProductViewR1325 battleProductViewR1325;BattleProductWorldLayerDocumentR1325 battleProductLayerDocumentR1325;RectTransform battleProductViewportR1325;RawImage battleProductLayerR1325;
  readonly Dictionary<GameObject,bool> battleProductMainVisualsR1325=new Dictionary<GameObject,bool>();
  // Retain Main source objects, source clocks and materials; suspend only their displayed objects.
  // The shared generic effect/hero renderers can be reused by the ordinary renderer during Battle.
  void CaptureBattleProductMainVisualsR1325(){BattleProductNeedR1325(battleProductMainVisualsR1325.Count==0,"previous borrowed Main visual snapshot not released");if(!effectsRoot)return;foreach(Transform child in effectsRoot){battleProductMainVisualsR1325.Add(child.gameObject,child.gameObject.activeSelf);child.gameObject.SetActive(false);}}
  void RestoreBattleProductMainVisualsR1325(){foreach(var p in battleProductMainVisualsR1325)if(p.Key)p.Key.SetActive(p.Value);battleProductMainVisualsR1325.Clear();}
  static bool BattleProductArrayR1325(float[] a,params float[] b){if(a==null||a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
  static string BattleProductRgbaR1325(Texture2D texture){var pixels=texture.GetPixels32();var bytes=new byte[checked(pixels.Length*4)];int at=0;for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){var p=pixels[y*texture.width+x];bytes[at++]=p.r;bytes[at++]=p.g;bytes[at++]=p.b;bytes[at++]=p.a;}return BattleProductDataPlanR1282.Hash(bytes);}
  void ConfigureBattleProductWorldLayerR1325(BattleProductDataPlanR1282 plan){
   BattleProductNeedR1325(hudCanvas&&worldCamera&&!battleProductViewportR1325,"dedicated world viewport unavailable");var doc=BattleProductTypedJsonR1282.Read<BattleProductWorldLayerDocumentR1325>(new System.Text.UTF8Encoding(false,true).GetBytes(plan.WorldLayerSourceJson));BattleProductNeedR1325(doc.schema=="altera-airship-source-world-layers-v19"&&doc.rows!=null&&doc.rows.Length==4,"original four world layer rows absent");
   for(int i=0;i<4;i++){var r=doc.rows[i];BattleProductNeedR1325(r.world==BattleProductInputBindingsR1282.Worlds[i].id&&!r.nativeLayerProjectionMeasured&&!r.runtimeConsumerImplemented&&r.decodedRgbaSha256=="664eb846cc63a474c25f5de98b323f381b818c789665fdb796d624d9eab1c0b6"&&BattleProductArrayR1325(r.sourcePositionParameters,0,0,1,i==0||i==3?2.8f:2.5f)&&BattleProductArrayR1325(r.sourceColor,.192157f,.431373f,1,1)&&BattleProductArrayR1325(r.sourceTexUV,i==3?-2.8f:-2,0)&&BattleProductArrayR1325(r.sourceFeedbackUV,0,0),"original layer source arguments changed");}
   var texture=LoadBattleProductTextureR1325(plan.WorldLayerTexture,256,256,false,"Battle original world layer RGBA");BattleProductNeedR1325(BattleProductRgbaR1325(texture)==doc.rows[0].decodedRgbaSha256,"original layer RGBA changed");texture.wrapMode=TextureWrapMode.Repeat;
   var root=new GameObject("Battle finite2D source world viewport",typeof(RectTransform));battleProductOwnedR1325.Add(root);root.transform.SetParent(hudCanvas.transform,false);root.transform.SetAsFirstSibling();battleProductViewportR1325=root.GetComponent<RectTransform>();battleProductViewportR1325.localScale=Vector3.one;
   var image=new GameObject("Original WorldLayer_Velder_Gate / finite2D",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));image.transform.SetParent(root.transform,false);battleProductLayerR1325=image.GetComponent<RawImage>();var rect=image.GetComponent<RectTransform>();rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;battleProductLayerR1325.texture=texture;battleProductLayerR1325.raycastTarget=false;battleProductLayerR1325.maskable=false;battleProductLayerDocumentR1325=doc;root.SetActive(false);
   // Explicit finite2D choice: full normalized camera viewport, UI Default straight alpha,
   // Point Repeat, fixed source SetTexUV offset. No inference that SetPos is scale or
   // SetTexUV is velocity; original position/blend1 setters remain retained source facts.
  }
  void RenderBattleProductPresentationR1325(){
   if(!BattleProductSessionActiveR1325)return;if(battleProductViewR1325!=null)battleProductViewR1325.Render();if(!battleProductViewportR1325)return;var r=worldCamera.rect;battleProductViewportR1325.anchorMin=r.min;battleProductViewportR1325.anchorMax=r.max;battleProductViewportR1325.offsetMin=battleProductViewportR1325.offsetMax=Vector2.zero;var layer=battleProductLayerDocumentR1325.rows[game.room];battleProductViewportR1325.gameObject.SetActive(nativeScene!=null&&nativeScene.id==layer.world);battleProductLayerR1325.color=new Color(layer.sourceColor[0],layer.sourceColor[1],layer.sourceColor[2],layer.sourceColor[3]);battleProductLayerR1325.uvRect=new Rect(layer.sourceTexUV[0],layer.sourceTexUV[1],1,1);
  }
  void DisposeBattleProductPresentationR1325(){var view=battleProductViewR1325;battleProductViewR1325=null;try{if(view!=null)view.Dispose();}finally{if(battleProductViewportR1325)battleProductViewportR1325.gameObject.SetActive(false);battleProductLayerDocumentR1325=null;battleProductLayerR1325=null;battleProductViewportR1325=null;}}
 }
}
