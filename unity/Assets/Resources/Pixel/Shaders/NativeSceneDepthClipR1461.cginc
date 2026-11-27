#ifndef NATIVE_SCENE_DEPTH_CLIP_R1461_INCLUDED
#define NATIVE_SCENE_DEPTH_CLIP_R1461_INCLUDED

// Positive eye distance in native-camera units, stored as linear RFloat color.
// This is not hardware depth: do not LinearEyeDepth it or reverse its Z value.
sampler2D _NativeSceneEyeDepthR1461;
float _NativeSceneDepthEnabledR1461;
float _NativeSceneDepthBiasR1461;
float4 _NativeSceneDepthUVR1461;
float4 _NativeSceneDepthRangeR1461;
float _NativeActorEyeDepthR1461;
float _NativeSceneClipR1461;

// The color and eye-depth passes cut the same opening in foreground geometry.
// Billboards then use ordinary depth, including any wall behind that opening.
float _NativePlayerRevealEnabledR1478;
float4 _NativePlayerRevealViewportR1478,_NativePlayerRevealEyeStrengthR1478,_NativePlayerRevealPixelGridR1478;
float4x4 _NativePlayerRevealWorldToClipR1478;
float NativePlayerRevealAmountR1478(float2 viewport,float sceneEye) {
 if(_NativePlayerRevealEnabledR1478<.5) return 0;
 float2 q=(viewport-_NativePlayerRevealViewportR1478.xy)/max(_NativePlayerRevealViewportR1478.zw,float2(.00001,.00001));
 float radiusSquared=dot(q,q);
 if(radiusSquared>=1)return 0;
 float halo=1-smoothstep(.68,1,sqrt(radiusSquared));
 float front=saturate((_NativePlayerRevealEyeStrengthR1478.x-sceneEye-_NativePlayerRevealEyeStrengthR1478.z)/max(1,_NativePlayerRevealEyeStrengthR1478.w));
 return halo*front*saturate(_NativePlayerRevealEyeStrengthR1478.y);
}
float NativePlayerRevealDitherR1478(float2 viewport) {
 float2 p=fmod(floor(saturate(viewport)*max(_NativePlayerRevealPixelGridR1478.xy,float2(1,1))),4);
 float4 row=p.y<1?float4(0,8,2,10):p.y<2?float4(12,4,14,6):p.y<3?float4(3,11,1,9):float4(15,7,13,5);
 float value=p.x<1?row.x:p.x<2?row.y:p.x<3?row.z:row.w;
 return (value+.5)/16;
}
void ClipNativeForegroundForPlayerR1478(float4 canonicalClip,float sceneEye,float coverage,float ownedScenePart) {
 if(ownedScenePart<.5||coverage<=0||canonicalClip.w<=0||_NativePlayerRevealEnabledR1478<.5)return;
 float2 viewport=canonicalClip.xy/canonicalClip.w*.5+.5;
 if(any(viewport<0)||any(viewport>1))return;
 // Outside this rectangle the reveal amount is identically zero.
 if(any(abs(viewport-_NativePlayerRevealViewportR1478.xy)>=_NativePlayerRevealViewportR1478.zw))return;
 clip(NativePlayerRevealDitherR1478(viewport)-NativePlayerRevealAmountR1478(viewport,sceneEye));
}

void ClipNativeSceneDepthR1461(float4 screenPosition) {
 if(_NativeSceneDepthEnabledR1461<0.5 || _NativeSceneClipR1461<0.5) return;
 // Match this native camera's original near/far clipping volume as well as
 // its scene depth. A negative/behind-camera actor must not pass as foreground.
 clip(_NativeActorEyeDepthR1461-_NativeSceneDepthRangeR1461.x);
 clip(_NativeSceneDepthRangeR1461.y-_NativeActorEyeDepthR1461);
 clip(screenPosition.w);
 float2 viewport=screenPosition.xy/screenPosition.w;
 if(any(viewport<0) || any(viewport>1)) return;
 // Start with (1,1,0,0): the RT projection already uses the GPU RT matrix,
 // and ComputeScreenPos supplies camera-local viewport coordinates. Do not
 // also unconditionally invert Y with UNITY_UV_STARTS_AT_TOP.
 float2 uv=viewport*_NativeSceneDepthUVR1461.xy+_NativeSceneDepthUVR1461.zw;
 if(any(uv<0) || any(uv>1)) return;
 float sceneEye=tex2D(_NativeSceneEyeDepthR1461,uv).r;
 // Clear color is native far+1. Positive native units are compared directly;
 // the same billboard eye plane is used across all opaque and outline pixels.
 clip(sceneEye+max(0,_NativeSceneDepthBiasR1461)-_NativeActorEyeDepthR1461);
}

#endif
