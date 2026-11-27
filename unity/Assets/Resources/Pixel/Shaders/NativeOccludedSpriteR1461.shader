Shader "ElswordPixel/NativeOccludedSpriteR1461" {
 Properties {
  [PerRendererData] _MainTex("Sprite Texture",2D)="white"{}
  _Color("Tint",Color)=(1,1,1,1)
  [MaterialToggle] PixelSnap("Pixel snap",Float)=0
  [HideInInspector] _RendererColor("RendererColor",Color)=(1,1,1,1)
  [HideInInspector] _Flip("Flip",Vector)=(1,1,1,1)
  [PerRendererData] _AlphaTex("External Alpha",2D)="white"{}
  [PerRendererData] _EnableExternalAlpha("Enable External Alpha",Float)=0
  [PerRendererData] _NativeSceneClipR1461("Native scene depth clip",Float)=0
  [PerRendererData] _NativeActorEyeDepthR1461("Native actor eye depth",Float)=0
 }
 SubShader {
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
  Cull Off Lighting Off ZWrite Off
  // SpriteFrag performs the standard RGB premultiplication before blending.
  Blend One OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex NativeOccludedSpriteVertR1461
   #pragma fragment NativeOccludedSpriteFragR1461
   #pragma target 2.0
   #pragma multi_compile_instancing
   #pragma multi_compile _ PIXELSNAP_ON
   #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
   #include "UnitySprites.cginc"
   #include "NativeSceneDepthClipR1461.cginc"
   struct NativeOccludedSpriteVaryingR1461 {
    float4 vertex:SV_POSITION;
    fixed4 color:COLOR;
    float2 texcoord:TEXCOORD0;
    float4 nativeSceneScreen:TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
   };
   NativeOccludedSpriteVaryingR1461 NativeOccludedSpriteVertR1461(appdata_t input) {
    v2f source=SpriteVert(input);
    NativeOccludedSpriteVaryingR1461 output;
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    output.vertex=source.vertex;output.color=source.color;output.texcoord=source.texcoord;
    output.nativeSceneScreen=ComputeScreenPos(source.vertex);
    return output;
   }
   fixed4 NativeOccludedSpriteFragR1461(NativeOccludedSpriteVaryingR1461 input):SV_Target {
    v2f source;
    UNITY_INITIALIZE_OUTPUT(v2f,source);
    source.vertex=input.vertex;source.color=input.color;source.texcoord=input.texcoord;
    fixed4 color=SpriteFrag(source);
    if(color.a<=0)discard;
    ClipNativeSceneDepthR1461(input.nativeSceneScreen);
    return color;
   }
   ENDCG
  }
 }
 Fallback Off
}
