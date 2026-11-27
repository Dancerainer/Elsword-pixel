using UnityEngine;
namespace ElswordPixel {
    // Original bAlphaBlend=true, no conflicting object control or explicit
    // src/dest factor. Exact nine prior parts plus20 Outskirts shadows; special factors remain
    // pending. Reuses the reviewed RGB source-alpha / RT coverage-preserving
    // shader; this is not an original-client blend-order parity claim.
    public static class NativeSourceAlphaPolicyV27 {
        static Shader shader;
        static readonly string[] floats={
            "_FogNear","_FogFar","_FogHeightStart","_FogHeightEnd","_FogBias","_Cull",
            "_DetailReduction","_DetailRadius","_ColorSteps","_BackdropCoverage","_FlatFarEnabled"
        };
        internal static bool Matches(string sceneId,int index,string partName,string texturePath) {
            switch(sceneId) {
                case "WI_BESMA_AIRSHIP_STAGE1":
                    return index>=0 && index<8 && partName=="Besma_Air_Ship_S2a_cloud"+(index+1)+".Y"
                        && texturePath!=null && texturePath.EndsWith("Besma_Air_cloud1.png",System.StringComparison.Ordinal);
                case "WI_BESMA_AIRSHIP_STAGE5":
                    if(index<0 || index>9 || texturePath==null || !texturePath.EndsWith("Besma_Air_cloud1.png",System.StringComparison.Ordinal))return false;
                    return partName==(index<8 ? "Besma_Air_Ship_s5_cloud"+(index+1)+".Y" : "Besma_Air_Ship_s5_cloudstay.Y");
                case "WI_ELDER_DARK_FOREST_STAGE_0":
                    return (index == 346 && partName == "Elder_Darkforest_Stage1_light_01.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 358 && partName == "Elder_Darkforest_Stage1_water02.Y" && texturePath == "Pixel/native-world/forest/textures/6d563cd26d4ebe0b401a")
                        || (index == 359 && partName == "Elder_Darkforest_Stage1_water03.Y" && texturePath == "Pixel/native-world/forest/textures/6d563cd26d4ebe0b401a")
                        || (index == 360 && partName == "Elder_Darkforest_Stage1_water04.Y" && texturePath == "Pixel/native-world/forest/textures/0a1be910f0f719b49307");
                case "WI_ELDER_BENDERS_CAVE_STAGE1":
                    return (index == 97 && partName == "Benders_Cave_Stage1_Fog1.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8")
                        || (index == 98 && partName == "Benders_Cave_Stage1_Fog1.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8")
                        || (index == 99 && partName == "Benders_Cave_Stage1_Fog1.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8");
                case "WI_ELDER_BENDERS_CAVE_STAGE1_1":
                    return (index == 276 && partName == "Benders_Cave_Stage1-1_Fog1.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8")
                        || (index == 277 && partName == "Benders_Cave_Stage1-1_Fog2.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8")
                        || (index == 278 && partName == "Benders_Cave_Stage1-1_Fog3.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8")
                        || (index == 279 && partName == "Benders_Cave_Stage1-1_Fog4.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8");
                case "WI_ELDER_BENDERS_STAGE_BOSS":
                    return (index == 115 && partName == "Elder_Benders_Cave_Boss_Fog.Y" && texturePath == "Pixel/native-world/cave/textures/ed2f2d621ea3f5831ae8");
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE1":
                    return (index == 54 && partName == "Elder_Wally_Castle_Roof_Stage1_Shadows.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 55 && partName == "Elder_Wally_Castle_Roof_Stage1_Shadows.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 56 && partName == "Elder_Wally_Castle_Roof_Stage1_Shadows.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 57 && partName == "Elder_Wally_Castle_Roof_Stage1_Shadows.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 58 && partName == "Elder_Wally_Castle_Roof_Stage1_Shadows.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 59 && partName == "Elder_Wally_Castle_Roof_Stage1_Shadows.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185");
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE2":
                    return (index == 56 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 57 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 58 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 59 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 60 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 61 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 62 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 63 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 64 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 65 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 66 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/efeb593f14529543c5d7")
                        || (index == 67 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 68 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185")
                        || (index == 69 && partName == "Elder_Wally_Castle_Roof_Stage2_Shadow01.Y" && texturePath == "Pixel/native-world/outskirts/textures/74866bb36f16d4257185");
                default:return false;
            }
        }
        public static void Configure(Material material,string sceneId,int index,string partName,string texturePath) {
            if(!material)return;
            bool cargoDeck=sceneId=="WI_BESMA_AIRSHIP_STAGE1",cargoBoss=sceneId=="WI_BESMA_AIRSHIP_STAGE5";
            if((cargoDeck && index==8 && partName=="Besma_Air_Ship_S2a_sky.Y") ||
                (cargoBoss && index==69 && partName=="Besma_Air_Ship_s5_sky.Y")){
#if !UNITY_ANDROID || UNITY_EDITOR
                material.renderQueue=(int)UnityEngine.Rendering.RenderQueue.Background;
#endif
                return;
            }
            if(!Matches(sceneId,index,partName,texturePath))return;
            if(!shader)shader=Resources.Load<Shader>("Pixel/Shaders/WaterwaySourceAlphaV26");
            if(!shader)throw new System.InvalidOperationException("Missing reviewed source-alpha coverage shader");
            var texture=material.mainTexture;var scale=material.mainTextureScale;var offset=material.mainTextureOffset;
            var color=material.GetColor("_Color");var fog=material.GetColor("_FogColor");var flat=material.GetColor("_FlatFarColor");
            var values=new float[floats.Length];for(int i=0;i<values.Length;i++)values[i]=material.GetFloat(floats[i]);
            material.shader=shader;material.mainTexture=texture;material.mainTextureScale=scale;material.mainTextureOffset=offset;
            material.SetColor("_Color",color);material.SetColor("_FogColor",fog);material.SetColor("_FlatFarColor",flat);
            for(int i=0;i<values.Length;i++)material.SetFloat(floats[i],values[i]);
            // The ship's clouds belong to the source sky layers, even when the
            // linked deck turns and puts their geometry in front of the camera.
#if !UNITY_ANDROID || UNITY_EDITOR
            if(cargoDeck || cargoBoss)material.renderQueue=(int)UnityEngine.Rendering.RenderQueue.Background+(cargoBoss && index==0 ? 2 : 1);
#endif
        }
    }
}
