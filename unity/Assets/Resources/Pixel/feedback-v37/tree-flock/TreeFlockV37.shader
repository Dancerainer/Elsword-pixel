Shader "ElswordPixel/TreeFlockV37" {
 Properties {
  _MainTex("Drawn feather map",2D)="white"{}
  _InkCenter("Component center",Vector)=(0,0,0,0)
  _Ink("Contour color",Color)=(.23,.15,.18,1)
 }
 SubShader {
  Tags {"Queue"="AlphaTest" "RenderType"="Opaque"}
  Cull Off
  ZWrite On
  Pass {
   Offset 1, 1
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   float4 _InkCenter,_Ink;
   float4 vert(float4 vertex:POSITION):SV_POSITION {
    float4 p=UnityObjectToClipPos(vertex);
    float4 center=UnityObjectToClipPos(float4(_InkCenter.xyz,1));
    float2 direction=(p.xy/p.w-center.xy/center.w)*_ScreenParams.xy;
    p.xy+=direction/max(1,length(direction))*1.5/_ScreenParams.xy*p.w;
    return p;
   }
   fixed4 frag():SV_Target{return _Ink;}
   ENDCG
  }
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex;
   struct Input {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
   struct Output {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;};
   Output vert(Input v){Output o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=float2(v.uv.x,1-v.uv.y);return o;}
   fixed4 frag(Output p):SV_Target{return tex2D(_MainTex,p.uv);}
   ENDCG
  }
 }
}
