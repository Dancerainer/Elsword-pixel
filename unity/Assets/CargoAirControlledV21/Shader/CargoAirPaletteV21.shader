Shader "ElswordPixel/CargoAirControlledPaletteV21" {
 Properties {
  [PerRendererData] _MainTex ("Original approved atlas",2D)="white"{}
  _AirIndexTex ("Linear point AIR index control data",2D)="black"{}
  _AirPaletteTex ("sRGB point source-specific AIR palette",2D)="white"{}
  _Color ("Tint",Color)=(1,1,1,1)
  [MaterialToggle] PixelSnap ("Pixel snap",Float)=0
  [HideInInspector] _RendererColor ("RendererColor",Color)=(1,1,1,1)
  [HideInInspector] _Flip ("Flip",Vector)=(1,1,1,1)
  [PerRendererData] _AlphaTex ("External Alpha",2D)="white"{}
  [PerRendererData] _EnableExternalAlpha ("Enable External Alpha",Float)=0
 }
 SubShader {
  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="False"}
  Cull Off Lighting Off ZWrite Off Blend One OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex SpriteVert
   #pragma fragment CargoAirFrag
   #pragma target 2.0
   #pragma multi_compile_instancing
   #pragma multi_compile _ PIXELSNAP_ON
   #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
   #include "UnitySprites.cginc"
   sampler2D _AirIndexTex;
   sampler2D _AirPaletteTex;
   fixed4 CargoAirFrag(v2f IN):SV_Target {
    fixed4 c=SampleSpriteTexture(IN.texcoord);
    float3 k=floor(tex2D(_AirIndexTex,IN.texcoord).rgb*255.0+0.5);
    if(c.a>0&&k.b>254.5&&(k.r+k.g)>0.5){
     // Palette PNG row255-high is the row sampled by native Unity UV high.
     float2 paletteUV=(k.rg+0.5)/256.0;
     c.rgb=tex2D(_AirPaletteTex,paletteUV).rgb;
    }
    c*=IN.color;c.rgb*=c.a;return c;
   }
   ENDCG
  }
 }
}
