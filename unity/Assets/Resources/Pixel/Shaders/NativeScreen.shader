Shader "ElswordPixel/NativeScreen" {
 Properties {_MainTex("Pixel world",2D)="white"{}}
 SubShader {Tags {"Queue"="Background"} Cull Off ZWrite Off Pass {CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
 struct output {float4 position:SV_POSITION;float2 uv:TEXCOORD0;};
 output vert(input v){output o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
 fixed4 frag(output i):SV_Target{return tex2D(_MainTex,i.uv);}
 ENDCG}}
}
