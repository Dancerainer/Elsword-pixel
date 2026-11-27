Shader "ElswordPixel/NativeSkillSprite" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
 [PerRendererData] _NativeSceneClipR1461("Native scene depth clip",Float)=0 [PerRendererData] _NativeActorEyeDepthR1461("Native actor eye depth",Float)=0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination blend", Float) = 7
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off Lighting Off ZWrite Off
        // Sprite alpha remains coverage. In particular the red source layer
        // must not reduce target alpha before a DstAlpha color blend.
        Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            #pragma vertex NativeDepthSpriteVertR1461
            #pragma fragment NativeSpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            #include "NativeSceneDepthClipR1461.cginc"
            struct NativeDepthSpriteVaryingR1461 {
                float4 vertex:SV_POSITION;
                fixed4 color:COLOR;
                float2 texcoord:TEXCOORD0;
                float4 nativeSceneScreen:TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            NativeDepthSpriteVaryingR1461 NativeDepthSpriteVertR1461(appdata_t input) {
                v2f source=SpriteVert(input);
                NativeDepthSpriteVaryingR1461 output;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.vertex=source.vertex;output.color=source.color;output.texcoord=source.texcoord;
                output.nativeSceneScreen=ComputeScreenPos(source.vertex);
                return output;
            }
            fixed4 NativeSpriteFrag(NativeDepthSpriteVaryingR1461 input) : SV_Target {
                ClipNativeSceneDepthR1461(input.nativeSceneScreen);
                return SampleSpriteTexture(input.texcoord) * input.color;
            }
            ENDCG
        }
    }
}
