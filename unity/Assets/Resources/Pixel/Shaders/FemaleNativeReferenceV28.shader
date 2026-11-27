Shader "ElswordPixel/FemaleNativeReferenceV28" {
 Properties {_MainTex("Texture",2D)="white"{} _Color("Color",Color)=(1,1,1,1) _FogColor("Fog",Color)=(1,1,1,1) _FogNear("Near",Float)=1000000 _FogFar("Far",Float)=1000001 _FogHeightStart("Height start",Float)=100000 _FogHeightEnd("Height end",Float)=-100000 _FogBias("Fog bias",Float)=0 _Cull("Reference face culling",Float)=0}
 SubShader {Tags {"Queue"="AlphaTest"} Cull [_Cull] ZWrite On Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _Color,_FogColor;float _FogNear,_FogFar,_FogHeightStart,_FogHeightEnd,_FogBias;
 struct app {float4 vertex:POSITION;float2 uv:TEXCOORD0;};struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float depth:TEXCOORD1;float height:TEXCOORD2;};
 v2f vert(app v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=float2(v.uv.x,1-v.uv.y);o.depth=length(mul(unity_ObjectToWorld,v.vertex).xyz-_WorldSpaceCameraPos);o.height=mul(unity_ObjectToWorld,v.vertex).y;return o;}
 fixed4 frag(v2f i):SV_Target {fixed4 c=tex2D(_MainTex,i.uv)*_Color;clip(c.a-.18);float vertical=saturate((_FogHeightStart-i.height)/(_FogHeightStart-_FogHeightEnd+.0001));float fog=saturate(saturate((i.depth-_FogNear)/max(1,_FogFar-_FogNear))*vertical+_FogBias);c.rgb=lerp(c.rgb,_FogColor.rgb,fog);c.a=1;return c;}
 ENDCG }}
}
