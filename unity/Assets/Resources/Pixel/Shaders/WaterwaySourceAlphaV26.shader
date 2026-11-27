Shader "ElswordPixel/WaterwaySourceAlphaV26" {
 Properties {_MainTex("Texture",2D)="white"{} _Color("Color",Color)=(1,1,1,1) _FogColor("Fog",Color)=(1,1,1,1) _FogNear("Near",Float)=1000000 _FogFar("Far",Float)=1000001 _FogHeightStart("Height start",Float)=100000 _FogHeightEnd("Height end",Float)=-100000 _FogBias("Fog bias",Float)=0 _Cull("Cull",Float)=0 _DetailReduction("Broad color planes",Range(0,1))=0.8 _DetailRadius("Detail radius",Range(0,4))=1.75 _ColorSteps("Color bands",Range(4,32))=12 _BackdropCoverage("Native foreground coverage",Float)=1 _FlatFarEnabled("Single far layer color",Float)=0 _FlatFarColor("Single far layer color",Color)=(.18,.61,.62,1)}
 SubShader {Tags {"Queue"="Transparent" "RenderType"="Transparent"} Cull [_Cull] ZWrite Off Blend SrcAlpha OneMinusSrcAlpha, Zero One Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma target 3.0
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _MainTex_TexelSize,_Color,_FogColor;float _FogNear,_FogFar,_FogHeightStart,_FogHeightEnd,_FogBias,_DetailReduction,_DetailRadius,_ColorSteps,_SceneDetailReviewOriginal,_BackdropCoverage,_FlatFarEnabled;float4 _FlatFarColor;
 struct app {float4 vertex:POSITION;float2 uv:TEXCOORD0;};struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float depth:TEXCOORD1;float height:TEXCOORD2;};
 v2f vert(app v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=float2(v.uv.x,1-v.uv.y);o.depth=length(mul(unity_ObjectToWorld,v.vertex).xyz-_WorldSpaceCameraPos);o.height=mul(unity_ObjectToWorld,v.vertex).y;return o;}
 #include "SceneryColorSampling.cginc"
 fixed4 frag(v2f i):SV_Target {float4 source=tex2D(_MainTex,i.uv);fixed4 c=source*_Color;c.rgb=SceneryRgb(i.uv,source,i.height,i.depth);c.a=saturate(source.a*_Color.a)*saturate(_BackdropCoverage);return c;}
 ENDCG }}
}

