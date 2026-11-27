using UnityEngine;
namespace ElswordPixel {
 // Explicit artistic display simplification, not source-equivalent transparency.
 // Original geometry/UV/PNG and gameplay surfaces remain untouched. Exact far341 is an explicit artistic visibility adaptation by alpha0 clip.
 public static class ForestGroundFaceDisplayV28 {
  static Shader shader;
  static readonly string[] floats={"_FogNear","_FogFar","_FogHeightStart","_FogHeightEnd","_FogBias","_Cull","_DetailReduction","_DetailRadius","_ColorSteps","_BackdropCoverage","_FlatFarEnabled"};
  static readonly Vector4[] sourceLines={
            new Vector4(7617f,1400f,8212f,1400f),
            new Vector4(8212f,1400f,8506f,1450f),
            new Vector4(8506f,1450f,8914f,1450f),
            new Vector4(8284f,654f,8556f,654f),
            new Vector4(8556f,500f,9405f,500f),
            new Vector4(9405f,500f,10271f,500f),
            new Vector4(10271f,500f,10599f,500f),
            new Vector4(10599f,500f,11172f,600f),
            new Vector4(11172f,600f,12956f,600f),
            new Vector4(9405f,740f,10271f,740f),
            new Vector4(11363f,840f,11815f,840f),
            new Vector4(11815f,1080f,12451f,1080f),
            new Vector4(12956f,840f,13470f,840f),
            new Vector4(13470f,840f,13737f,918f),
            new Vector4(13737f,918f,14820.8f,918f)
  };
  internal static bool Matches(string sceneId,int index,string partName,string texturePath){return sceneId=="WI_ELDER_DARK_FOREST_STAGE_0"&&(
                (index==210&&partName=="Elder_Darkforest_Stage1_G02.Y"&&texturePath=="Pixel/native-world/forest/textures/de9ab105f57b0ddfeb3e")
                || (index==216&&partName=="Elder_Darkforest_Stage1_G02.Y"&&texturePath=="Pixel/native-world/forest/textures/de9ab105f57b0ddfeb3e")
                || (index==227&&partName=="Elder_Darkforest_Stage1_G02.Y"&&texturePath=="Pixel/native-world/forest/textures/de9ab105f57b0ddfeb3e")
                || (index==231&&partName=="Elder_Darkforest_Stage1_G02.Y"&&texturePath=="Pixel/native-world/forest/textures/de9ab105f57b0ddfeb3e")
                || (index==306&&partName=="Elder_Darkforest_Stage1_G03.Y"&&texturePath=="Pixel/native-world/forest/textures/847e6bfc9a4b6c6fcce3")
  );}
  public static void Configure(Material material,string sceneId,int index,string partName,string texturePath){
   if(!material)return;
   // Artistic hide of the complete original147vertex far chunk (31body+18upper triangles).
   // NativeWorld's original clip(c.a-.18) discards every fragment before depth/coverage writes.
   // Camera geometry registry intentionally retains originalXYZ, as with Editor Renderer-off.
   if(sceneId=="WI_ELDER_DARK_FOREST_STAGE_0"&&index==341&&partName=="Elder_Darkforest_Stage1_G03_Frap.Y"&&texturePath=="Pixel/native-world/forest/textures/9af3b601919da2af60a9"){
    var farColor=material.GetColor("_Color");farColor.a=0;material.SetColor("_Color",farColor);return;
   }
   if(!Matches(sceneId,index,partName,texturePath))return;
   if(!shader)shader=Resources.Load<Shader>("Pixel/Shaders/ForestGroundFaceDisplayV28");if(!shader)throw new System.InvalidOperationException("Missing explicit Forest1 lower-face display shader");
   var texture=material.mainTexture;var scale=material.mainTextureScale;var offset=material.mainTextureOffset;var color=material.GetColor("_Color");var fog=material.GetColor("_FogColor");var flat=material.GetColor("_FlatFarColor");var values=new float[floats.Length];for(int i=0;i<values.Length;i++)values[i]=material.GetFloat(floats[i]);
   material.shader=shader;material.mainTexture=texture;material.mainTextureScale=scale;material.mainTextureOffset=offset;material.SetColor("_Color",color);material.SetColor("_FogColor",fog);material.SetColor("_FlatFarColor",flat);for(int i=0;i<values.Length;i++)material.SetFloat(floats[i],values[i]);
   material.SetVectorArray("_ForestSourceLines",sourceLines);material.SetFloat("_ForestGroundLowUpper",754);material.SetFloat("_ForestGroundLowLower",478);material.SetFloat("_ForestGroundContactInner",24);material.SetFloat("_ForestGroundContactOuter",48);material.SetFloat("_ForestGroundNearestEndpoint",index==306?0:1);material.SetColor("_ForestGroundColor",new Color32(9,24,49,255));
  }
#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
  static void ResetReview(){Shader.SetGlobalFloat("_ForestGroundReviewOriginal",0);}
 }
}
