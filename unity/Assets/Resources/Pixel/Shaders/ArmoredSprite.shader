Shader "ElswordPixel/ArmoredSprite" {
 Properties { [PerRendererData] _MainTex("Sprite Texture",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) [HideInInspector] _RendererColor("RendererColor",Color)=(1,1,1,1) [HideInInspector] _Flip("Flip",Vector)=(1,1,1,1) [PerRendererData] _MonsterInk("Monster contour",Float)=0 _Outline("Armor",Float)=0 _OutlineColor("Armor edge",Color)=(1,.14,.26,1) _HitFlash("Hit flash",Range(0,1))=0 [PerRendererData] _BurnTint("Enemy burn tint",Range(0,1))=0 [PerRendererData] _BurialClip("Underground clipping",Float)=0 [PerRendererData] _BurialGroundY("Projected ground height",Float)=-100000 [PerRendererData] _NativeSceneClipR1461("Native scene depth clip",Float)=0 [PerRendererData] _NativeActorEyeDepthR1461("Native actor eye depth",Float)=0 }
 SubShader { Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" } Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex FeedbackSpriteVertV25
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnitySprites.cginc"
 #include "NativeSceneDepthClipR1461.cginc"
 float4 _MainTex_TexelSize;fixed4 _OutlineColor;float _MonsterInk,_Outline,_HitFlash,_BurialClip,_BurialGroundY,_BurnTint;
 struct FeedbackSpriteV2fV25 {float4 vertex:SV_POSITION;fixed4 color:COLOR;float2 texcoord:TEXCOORD0;float worldY:TEXCOORD1;float4 nativeSceneScreen:TEXCOORD2;UNITY_VERTEX_OUTPUT_STEREO};
 FeedbackSpriteV2fV25 FeedbackSpriteVertV25(appdata_t input) {
  v2f source=SpriteVert(input);FeedbackSpriteV2fV25 output;UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
  output.vertex=source.vertex;output.color=source.color;output.texcoord=source.texcoord;output.nativeSceneScreen=ComputeScreenPos(source.vertex);
  output.worldY=mul(unity_ObjectToWorld,UnityFlipSprite(input.vertex,_Flip)).y;return output;
 }
 fixed4 frag(FeedbackSpriteV2fV25 i):SV_Target {if(_BurialClip>.5)clip(i.worldY-_BurialGroundY);fixed4 c=tex2D(_MainTex,i.texcoord)*i.color;
 // Sample the ink on a logical pixel grid. High-resolution paintings and
 // compact whole sprites get the same visible contour at any output height.
 float2 du=ddx(i.texcoord),dv=ddy(i.texcoord);
 float armorInside=0;
 if(_Outline>.5&&c.a>0){
  float width=max(1,_ScreenParams.y/max(1,unity_OrthoParams.y))*1.25;
  float2 x=du*width,y=dv*width;
  armorInside=1-min(min(tex2D(_MainTex,i.texcoord+x).a,tex2D(_MainTex,i.texcoord-x).a),min(tex2D(_MainTex,i.texcoord+y).a,tex2D(_MainTex,i.texcoord-y).a));
 }
 if(_Outline<=.5&&c.a<=0)discard;
 ClipNativeSceneDepthR1461(i.nativeSceneScreen);float2 p=_MainTex_TexelSize.xy;float a=0;
 if(_Outline>.5&&c.a!=1){a=max(a,tex2D(_MainTex,i.texcoord+float2(p.x,0)).a);a=max(a,tex2D(_MainTex,i.texcoord-float2(p.x,0)).a);a=max(a,tex2D(_MainTex,i.texcoord+float2(0,p.y)).a);a=max(a,tex2D(_MainTex,i.texcoord-float2(0,p.y)).a);a=max(a,tex2D(_MainTex,i.texcoord+p).a);a=max(a,tex2D(_MainTex,i.texcoord-p).a);a=max(a,tex2D(_MainTex,i.texcoord+float2(p.x,-p.y)).a);a=max(a,tex2D(_MainTex,i.texcoord+float2(-p.x,p.y)).a);}
 if(_MonsterInk>.5&&c.a>0){
  float cell=max(1,_ScreenParams.y/max(1,unity_OrthoParams.y));
  float2 offset=(floor(i.vertex.xy/cell)+.5)*cell-i.vertex.xy;
  float2 uv=i.texcoord+offset.x*du+offset.y*dv;
  du*=cell*1.35;dv*=cell*1.35;
  float neighbor=min(min(tex2D(_MainTex,uv+du).a,tex2D(_MainTex,uv-du).a),
                     min(tex2D(_MainTex,uv+dv).a,tex2D(_MainTex,uv-dv).a));
  if(neighbor<.25)c.rgb=min(c.rgb,c.rgb*.26+fixed3(.047,.035,.060));
 }
 // Overlay the target's painted body while retaining its dark ink and lighting.
 fixed value=max(c.r,max(c.g,c.b));
 c.rgb=lerp(c.rgb,fixed3(.28+.72*value,.05+.15*value,.07+.12*value),_BurnTint);
 c.rgb=lerp(c.rgb,fixed3(1,.96,.85),saturate(_HitFlash));
 c.rgb=lerp(c.rgb,_OutlineColor.rgb,armorInside*_Outline);
 float edge=a*(1-c.a)*_Outline*i.color.a;float alpha=c.a+edge;return fixed4((c.rgb*c.a+_OutlineColor.rgb*edge)/max(alpha,.0001),alpha);}
 ENDCG }
 }
}
