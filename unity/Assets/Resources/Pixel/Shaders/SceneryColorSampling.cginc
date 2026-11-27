// Only the scenery RGB is smoothed. The caller keeps the original center
// alpha for cutouts, blending, player reveal and foreground depth.
float3 BroadMaterialColor(float2 uv,float4 center) {
 float2 stepUv=max(abs(ddx(uv))+abs(ddy(uv)),_MainTex_TexelSize.xy)*max(1,_DetailRadius)*0.25;
 float3 sum=0; float weight=0;
#if defined(SHADER_API_MOBILE) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLES)
 // The four equal-weight taps have the same XY variance as the 9x9 box
 // (mean squared offset of -4..4 is 20/3), at the original texture scale.
 float2 d=stepUv*2.581989;
 float4 a=tex2D(_MainTex,uv+d), b=tex2D(_MainTex,uv-d);
 float4 c=tex2D(_MainTex,uv+float2(d.x,-d.y)), e=tex2D(_MainTex,uv+float2(-d.x,d.y));
 sum=a.rgb*a.a+b.rgb*b.a+c.rgb*c.a+e.rgb*e.a;
 weight=a.a+b.a+c.a+e.a;
#else
 [unroll] for(int y=-4;y<=4;y++) {
  [unroll] for(int x=-4;x<=4;x++) {
   float4 tap=tex2D(_MainTex,uv+float2(x,y)*stepUv);
   sum+=tap.rgb*tap.a;weight+=tap.a;
  }
 }
#endif
 return lerp(center.rgb,weight>0.0001?sum/weight:center.rgb,saturate(_DetailReduction));
}

float3 SceneryRgb(float2 uv,float4 source,float height,float depth) {
 // Flat distant layers use a fixed artist color. They still use the original
 // center alpha in the caller for cutouts, depth and player readability.
 if(_FlatFarEnabled>0.5&&_SceneDetailReviewOriginal<0.5)return _FlatFarColor.rgb;
 fixed3 rgb=source.rgb*_Color.rgb;
 if(_DetailReduction>0.0001&&_SceneDetailReviewOriginal<0.5&&min(_MainTex_TexelSize.z,_MainTex_TexelSize.w)>=16)
  rgb=BroadMaterialColor(uv,source)*_Color.rgb;
 float vertical=saturate((_FogHeightStart-height)/(_FogHeightStart-_FogHeightEnd+.0001));
 float fog=saturate(saturate((depth-_FogNear)/max(1,_FogFar-_FogNear))*vertical+_FogBias);
 return lerp(rgb,_FogColor.rgb,fog);
}
