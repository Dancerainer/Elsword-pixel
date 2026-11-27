using UnityEngine;
namespace ElswordPixel {
    // These original world objects declare SrcAlpha/DestAlpha.
    // Keep those source facts unchanged. This presentation adapter uses
    // SrcAlpha/One RGB for an assumed opaque display destination because
    // this port's RT alpha stores geometry coverage, not original framebuffer
    // alpha. Zero/One RT alpha preserves that coverage bit. No original-client
    // alpha, draw-order or additive-blend parity is claimed.
    public static class NativeExplicitAlphaPolicyV27 {
        static Shader shader;
        static readonly string[] floats={
            "_FogNear","_FogFar","_FogHeightStart","_FogHeightEnd","_FogBias","_Cull",
            "_DetailReduction","_DetailRadius","_ColorSteps","_BackdropCoverage","_FlatFarEnabled"
        };
        internal static bool Matches(string sceneId,int index,string partName,string texturePath) {
            switch(sceneId) {
                case "WI_ALTERA_PLAIN_STAGE3":
                    return (index == 4 && partName == "Altera_Plain_stage Boss_C01.Y" ||
                            index == 5 && partName == "Altera_Plain_stage Boss_C02.Y") &&
                           System.IO.Path.GetFileName(texturePath?.Replace('\\','/')) == "Hamel_DG_Cloud_Low.png";
                case "WI_ELDER_DARK_FOREST_STAGE_0":
                    return (index == 347 && partName == "Elder_Darkforest_Stage1_light_02.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 348 && partName == "Elder_Darkforest_Stage1_light_03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 349 && partName == "Elder_Darkforest_Stage1_light_04.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 350 && partName == "Elder_Darkforest_Stage1_light_05.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297");
                case "WI_ELDER_DARK_FOREST_STAGE_1":
                    return (index == 245 && partName == "Elder_Darkforest_Stage2_Light01.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 246 && partName == "Elder_Darkforest_Stage2_Light02.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 247 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 248 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 249 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 250 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 251 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 252 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 253 && partName == "Elder_Darkforest_Stage2_Light03.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297");
                case "WI_ELDER_DARK_FOREST_BOSS_STAGE":
                    return (index == 105 && partName == "Druid_Forest_Stage_Boss_Light.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 106 && partName == "Druid_Forest_Stage_Boss_Light.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 107 && partName == "Druid_Forest_Stage_Boss_Light2.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297")
                        || (index == 108 && partName == "Druid_Forest_Stage_Boss_Light3.Y" && texturePath == "Pixel/native-world/forest/textures/9400956c9be24e75f297");
                case "WI_ELDER_BENDERS_CAVE_STAGE1":
                    return (index == 130 && partName == "Benders_Cave_Stage1_Flare.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 131 && partName == "Benders_Cave_Stage1_Flare.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 132 && partName == "Benders_Cave_Stage1_Flare.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 133 && partName == "Benders_Cave_Stage1_Flare.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 134 && partName == "Benders_Cave_Stage1_Flare.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d");
                case "WI_ELDER_BENDERS_CAVE_STAGE1_1":
                    return (index == 304 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 305 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 306 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 307 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 308 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 309 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 310 && partName == "Benders_Cave_Stage1-1_Lantern1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 311 && partName == "Benders_Cave_Stage1-1_Lantern2.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 312 && partName == "Benders_Cave_Stage1-1_Lantern2.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d");
                case "WI_ELDER_BENDERS_STAGE_BOSS":
                    return (index == 171 && partName == "Elder_Benders_Cave_Boss_Flare1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 172 && partName == "Elder_Benders_Cave_Boss_Flare1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 173 && partName == "Elder_Benders_Cave_Boss_Flare1.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 174 && partName == "Elder_Benders_Cave_Boss_Flare2.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 175 && partName == "Elder_Benders_Cave_Boss_Flare3.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 176 && partName == "Elder_Benders_Cave_Boss_Flare3.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 177 && partName == "Elder_Benders_Cave_Boss_Flare4.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 178 && partName == "Elder_Benders_Cave_Boss_Flare4.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 179 && partName == "Elder_Benders_Cave_Boss_Flare5.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 180 && partName == "Elder_Benders_Cave_Boss_Flare6.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d")
                        || (index == 181 && partName == "Elder_Benders_Cave_Boss_Flare7.Y" && texturePath == "Pixel/native-world/cave/textures/fbb8c8f742d9d8dd5f6d");
                default:return false;
            }
        }
        public static void Configure(Material material,string sceneId,int index,string partName,string texturePath) {
            if(!material)return;
            bool plainBoss = sceneId == "WI_ALTERA_PLAIN_STAGE3";
            // The two source sky layers sit between the sky dome and the arena.
            // Their near-camera geometry must composite before the platforms.
            if(plainBoss && index == 83 && partName == "Altera_Plain_stage Boss_Sky.Y"){
#if !UNITY_ANDROID || UNITY_EDITOR
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Background;
#endif
                return;
            }
            if(!Matches(sceneId,index,partName,texturePath))return;
            if(!shader)shader=Resources.Load<Shader>("Pixel/Shaders/SourceFactorOpaqueDestinationV27");
            if(!shader)throw new System.InvalidOperationException("Missing source-factor opaque-destination display adapter shader");
            var texture=material.mainTexture;var scale=material.mainTextureScale;var offset=material.mainTextureOffset;
            var color=material.GetColor("_Color");var fog=material.GetColor("_FogColor");var flat=material.GetColor("_FlatFarColor");
            var values=new float[floats.Length];for(int i=0;i<values.Length;i++)values[i]=material.GetFloat(floats[i]);
            material.shader=shader;material.mainTexture=texture;material.mainTextureScale=scale;material.mainTextureOffset=offset;
            material.SetColor("_Color",color);material.SetColor("_FogColor",fog);material.SetColor("_FlatFarColor",flat);
            for(int i=0;i<values.Length;i++)material.SetFloat(floats[i],values[i]);
#if !UNITY_ANDROID || UNITY_EDITOR
            if(plainBoss)material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Background + (index == 4 ? 1 : 2);
#endif
        }
    }
}
