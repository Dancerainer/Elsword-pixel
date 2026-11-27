Shader "ElswordPixel/ForestGroundFaceDisplayV28" {
 Properties {_MainTex("Texture",2D)="white"{} _Color("Color",Color)=(1,1,1,1) _FogColor("Fog",Color)=(1,1,1,1) _FogNear("Near",Float)=1000000 _FogFar("Far",Float)=1000001 _FogHeightStart("Height start",Float)=100000 _FogHeightEnd("Height end",Float)=-100000 _FogBias("Fog bias",Float)=0 _Cull("Cull",Float)=0 _DetailReduction("Broad color planes",Range(0,1))=0.8 _DetailRadius("Detail radius",Range(0,4))=1.75 _ColorSteps("Color bands",Range(4,32))=12 _BackdropCoverage("Native foreground coverage",Float)=1 [HideInInspector] _NativePlayerRevealScenePartR1478("Owned scene local player reveal",Float)=0 _FlatFarEnabled("Single far layer color",Float)=0 _FlatFarColor("Single far layer color",Color)=(.18,.61,.62,1) _ForestGroundLowUpper("Low native upper boundary",Float)=754 _ForestGroundLowLower("Low native lower boundary",Float)=478 _ForestGroundContactInner("Protected contact band",Float)=24 _ForestGroundContactOuter("Contact transition outer band",Float)=48 _ForestGroundNearestEndpoint("Nearest endpoint display extension",Float)=0 _ForestGroundColor("Lower-face broad color",Color)=(0.0352941176,0.0941176471,0.1921568627,1)}
 SubShader {Tags {"Queue"="AlphaTest"} Cull [_Cull] ZWrite On Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma target 3.0
 #include "UnityCG.cginc"
 #include "NativeSceneDepthClipR1461.cginc"
 float _NativePlayerRevealScenePartR1478;
 sampler2D _MainTex;float4 _MainTex_TexelSize,_Color,_FogColor;float _FogNear,_FogFar,_FogHeightStart,_FogHeightEnd,_FogBias,_DetailReduction,_DetailRadius,_ColorSteps,_SceneDetailReviewOriginal,_BackdropCoverage,_FlatFarEnabled;float4 _FlatFarColor;

 float _ForestGroundLowUpper,_ForestGroundLowLower,_ForestGroundContactInner,_ForestGroundContactOuter,_ForestGroundNearestEndpoint,_ForestGroundReviewOriginal;float4 _ForestGroundColor;float4 _ForestSourceLines[15];
 float ForestContactBlend(float2 p){
  float contact=1e6,nearestX=1e6,endpointContact=1e6;bool covered=false;
  [unroll]for(int k=0;k<15;k++){
   float4 l=_ForestSourceLines[k];float lo=min(l.x,l.z),hi=max(l.x,l.z);
   if(p.x>=lo-.02&&p.x<=hi+.02){float t=saturate((p.x-l.x)/max(.0001,l.z-l.x));contact=min(contact,abs(p.y-lerp(l.y,l.w,t)));covered=true;}
   float d=abs(p.x-l.x);float y=abs(p.y-l.y);if(d<nearestX-.0001){nearestX=d;endpointContact=y;}else if(abs(d-nearestX)<=.0001)endpointContact=min(endpointContact,y);
   d=abs(p.x-l.z);y=abs(p.y-l.w);if(d<nearestX-.0001){nearestX=d;endpointContact=y;}else if(abs(d-nearestX)<=.0001)endpointContact=min(endpointContact,y);
  }
  if(!covered){if(_ForestGroundNearestEndpoint<.5)return 0;contact=endpointContact;}
  return smoothstep(_ForestGroundContactInner,max(_ForestGroundContactInner+1,_ForestGroundContactOuter),contact);
 }
 struct app {float4 vertex:POSITION;float2 uv:TEXCOORD0;float2 protectedFace:TEXCOORD1;};struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float depth:TEXCOORD1;float height:TEXCOORD2;float2 nativeXY:TEXCOORD3;float4 playerRevealClip:TEXCOORD4;float2 playerRevealEye:TEXCOORD5;};
 v2f vert(app v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=float2(v.uv.x,1-v.uv.y);o.depth=length(mul(unity_ObjectToWorld,v.vertex).xyz-_WorldSpaceCameraPos);o.height=mul(unity_ObjectToWorld,v.vertex).y;o.nativeXY=mul(unity_ObjectToWorld,v.vertex).xy;o.playerRevealClip=mul(_NativePlayerRevealWorldToClipR1478,mul(unity_ObjectToWorld,v.vertex));o.playerRevealEye=float2(-UnityObjectToViewPos(v.vertex).z,v.protectedFace.x);return o;}
 #include "SceneryColorSampling.cginc"
 fixed4 frag(v2f i):SV_Target {float4 source=tex2D(_MainTex,i.uv);fixed4 c=source*_Color;clip(c.a-.18);ClipNativeForegroundForPlayerR1478(i.playerRevealClip,i.playerRevealEye.x,_BackdropCoverage,_NativePlayerRevealScenePartR1478*(1-i.playerRevealEye.y));c.rgb=SceneryRgb(i.uv,source,i.height,i.depth);if(_ForestGroundReviewOriginal<.5){float low=1-smoothstep(_ForestGroundLowLower,max(_ForestGroundLowLower+1,_ForestGroundLowUpper),i.nativeXY.y);if(low>0)c.rgb=lerp(c.rgb,_ForestGroundColor.rgb,low*ForestContactBlend(i.nativeXY));}c.a=saturate(_BackdropCoverage);return c;}
 ENDCG }}
}

