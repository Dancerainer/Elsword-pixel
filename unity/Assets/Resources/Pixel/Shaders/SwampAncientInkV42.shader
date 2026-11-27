Shader "ElswordPixel/SwampAncientInkV42" {
    Properties {
        _MainTex ("Painted skin", 2D) = "white" {}
        _Color ("Original material tint", Color) = (1,1,1,1)
        _Ink ("Silhouette ink", Color) = (0.137,0.075,0.118,1)
    }

    SubShader {
        Tags { "Queue"="AlphaTest+5" }
        Cull Off
        ZTest LEqual

        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _Color, _Ink;

        struct SkinInput {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };
        struct SkinOutput {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        SkinOutput ProjectSkin(SkinInput input, float2 offsetInPixels) {
            SkinOutput o;
            o.position = UnityObjectToClipPos(input.position);
            // This camera renders to the original Point-filtered scene target.
            // Offset in its own raster, independently of the final window size.
            o.position.xy += 2.0 * offsetInPixels * o.position.w / _ScreenParams.xy;
            o.uv = float2(input.uv.x, 1.0 - input.uv.y);
            return o;
        }
        SkinOutput FillVertex(SkinInput v) { return ProjectSkin(v, float2(0,0)); }
        SkinOutput LeftVertex(SkinInput v) { return ProjectSkin(v, float2(-1,0)); }
        SkinOutput RightVertex(SkinInput v) { return ProjectSkin(v, float2(1,0)); }
        SkinOutput UpVertex(SkinInput v) { return ProjectSkin(v, float2(0,1)); }
        SkinOutput DownVertex(SkinInput v) { return ProjectSkin(v, float2(0,-1)); }

        fixed4 FillFragment(SkinOutput i) : SV_Target {
            fixed4 color = tex2D(_MainTex, i.uv) * _Color;
            clip(color.a - .18);
            return fixed4(color.rgb, 1);
        }
        fixed4 InkFragment(SkinOutput i) : SV_Target {
            clip(tex2D(_MainTex, i.uv).a * _Color.a - .18);
            return _Ink;
        }
        ENDCG

        Pass {
            Name "AncientBody"
            ZWrite On
            Stencil { Ref 64 WriteMask 64 Comp Always Pass Replace }
            CGPROGRAM
            #pragma vertex FillVertex
            #pragma fragment FillFragment
            ENDCG
        }
        // Mark surviving body fragments first. Only uncovered neighbours
        // receive ink, and original scene depth still clips all four passes.
        Pass {
            Name "InkLeft"
            ZWrite Off
            Stencil { Ref 64 ReadMask 64 WriteMask 0 Comp NotEqual Pass Keep }
            CGPROGRAM
            #pragma vertex LeftVertex
            #pragma fragment InkFragment
            ENDCG
        }
        Pass {
            Name "InkRight"
            ZWrite Off
            Stencil { Ref 64 ReadMask 64 WriteMask 0 Comp NotEqual Pass Keep }
            CGPROGRAM
            #pragma vertex RightVertex
            #pragma fragment InkFragment
            ENDCG
        }
        Pass {
            Name "InkUp"
            ZWrite Off
            Stencil { Ref 64 ReadMask 64 WriteMask 0 Comp NotEqual Pass Keep }
            CGPROGRAM
            #pragma vertex UpVertex
            #pragma fragment InkFragment
            ENDCG
        }
        Pass {
            Name "InkDown"
            ZWrite Off
            Stencil { Ref 64 ReadMask 64 WriteMask 0 Comp NotEqual Pass Keep }
            CGPROGRAM
            #pragma vertex DownVertex
            #pragma fragment InkFragment
            ENDCG
        }
    }
}
