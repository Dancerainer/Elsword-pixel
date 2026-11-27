Shader "ElswordPixel/NativeSceneEyeDepthR1461" {
 Properties {
  _MainTex("Original native texture",2D)="white"{}
  _Color("Original native tint",Color)=(1,1,1,1)
  _Cull("Original native culling",Float)=0
  _BackdropCoverage("Native foreground coverage",Float)=1
  _NativePlayerRevealScenePartR1478("Occluding foreground part",Float)=0
 }
 SubShader {
  Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
  Cull [_Cull] ZWrite On ZTest LEqual Blend Off
  Pass {
   CGPROGRAM
   #pragma vertex NativeSceneDepthVertexR1461
   #pragma fragment NativeSceneDepthFragmentR1461
   #pragma target 3.0
   #include "UnityCG.cginc"
   #include "NativeSceneDepthClipR1461.cginc"
   sampler2D _MainTex;
   float4 _Color;
   float _BackdropCoverage,_NativePlayerRevealScenePartR1478;
   float4x4 _NativeOcclusionWorldToViewR1461;
   float4x4 _NativeOcclusionWorldToClipR1461;
   struct NativeSceneDepthInputR1461 { float4 vertex:POSITION; float2 uv:TEXCOORD0; float2 protectedFace:TEXCOORD1; };
   struct NativeSceneDepthVaryingR1461 { float4 position:SV_POSITION; float2 uv:TEXCOORD0; float2 eyeDepth:TEXCOORD1; float4 revealClip:TEXCOORD2; };
   NativeSceneDepthVaryingR1461 NativeSceneDepthVertexR1461(NativeSceneDepthInputR1461 input) {
    NativeSceneDepthVaryingR1461 output;
    float4 world=mul(unity_ObjectToWorld,input.vertex);
    // Explicit matrices come from this frame's final native camera. GPU clip
    // uses GL.GetGPUProjectionMatrix(projection,true), exactly once in C#.
    output.position=mul(_NativeOcclusionWorldToClipR1461,world);
    output.revealClip=mul(_NativePlayerRevealWorldToClipR1478,world);
    output.eyeDepth=float2(-mul(_NativeOcclusionWorldToViewR1461,world).z,input.protectedFace.x);
    // Match NativeWorld's source texture orientation and alpha test exactly.
    output.uv=float2(input.uv.x,1-input.uv.y);
    return output;
   }
   float NativeSceneDepthFragmentR1461(NativeSceneDepthVaryingR1461 input):SV_Target {
    // Named sky/clear-fill geometry is visible color, never a solid occluder.
    clip(_BackdropCoverage-0.5);
    clip(tex2D(_MainTex,input.uv).a*_Color.a-0.18);
    ClipNativeForegroundForPlayerR1478(input.revealClip,input.eyeDepth.x,_BackdropCoverage,_NativePlayerRevealScenePartR1478*(1-input.eyeDepth.y));
    return max(0,input.eyeDepth.x);
   }
   ENDCG
  }
 }
 Fallback Off
}
