Shader "ElswordPixel/NativeOccludedTextR1461" {
 Properties {
  _MainTex("Font atlas",2D)="white"{}
  _Color("Text color",Color)=(1,1,1,1)
  [PerRendererData] _NativeSceneClipR1461("Native scene depth clip",Float)=0
  [PerRendererData] _NativeActorEyeDepthR1461("Native actor eye depth",Float)=0
 }
 SubShader {
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
  Lighting Off Cull Off ZWrite Off ZTest Always
  Blend SrcAlpha OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex NativeOccludedTextVertR1461
   #pragma fragment NativeOccludedTextFragR1461
   #pragma target 2.0
   #include "UnityCG.cginc"
   #include "NativeSceneDepthClipR1461.cginc"
   sampler2D _MainTex;
   fixed4 _Color;
   struct NativeOccludedTextInputR1461 {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
   struct NativeOccludedTextVaryingR1461 {float4 position:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;float4 nativeSceneScreen:TEXCOORD1;};
   NativeOccludedTextVaryingR1461 NativeOccludedTextVertR1461(NativeOccludedTextInputR1461 input) {
    NativeOccludedTextVaryingR1461 output;
    output.position=UnityObjectToClipPos(input.vertex);output.uv=input.uv;output.color=input.color*_Color;
    output.nativeSceneScreen=ComputeScreenPos(output.position);return output;
   }
   fixed4 NativeOccludedTextFragR1461(NativeOccludedTextVaryingR1461 input):SV_Target {
    // The glyph atlas contributes coverage only. TextMesh vertex tint and
    // the original material color own RGB; transparent atlas RGB is ignored.
    fixed alpha=input.color.a*tex2D(_MainTex,input.uv).a;
    if(alpha<=0)discard;
    ClipNativeSceneDepthR1461(input.nativeSceneScreen);
    return fixed4(input.color.rgb,alpha);
   }
   ENDCG
  }
 }
 Fallback Off
}
