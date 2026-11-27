Shader "ElswordPixel/NativeScreenBackdropV25" {
 Properties {
  _MainTex("Pixel world",2D)="white"{}
  _BelowGroundColor("Existing scene lower plane",Color)=(0.067,0.2,0.32,1)
  _GroundViewportY("Projected local geometry bottom",Float)=0
  _BackdropEnabled("Below-ground clear fill",Float)=1
 }
 SubShader {Tags {"Queue"="Background"} Cull Off ZWrite Off Pass {CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _BelowGroundColor;float _GroundViewportY,_BackdropEnabled;
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
 struct output {float4 position:SV_POSITION;float2 uv:TEXCOORD0;};
 output vert(input v){output o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
 fixed4 frag(output i):SV_Target {
  fixed4 source=tex2D(_MainTex,i.uv);
  // Alpha 0 marks untouched clear or explicitly named sky geometry.
  // Foreground geometry and all pixels above the local floor stay unchanged.
  if(_BackdropEnabled>0.5&&source.a<0.0001&&i.uv.y<_GroundViewportY)
   source.rgb=_BelowGroundColor.rgb;
  source.a=1;
  return source;
 }
 ENDCG}}
}
