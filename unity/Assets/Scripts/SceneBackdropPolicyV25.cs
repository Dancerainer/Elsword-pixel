using System;
using UnityEngine;

namespace ElswordPixel {
    // Explicit backdrop and model-detail policies for reviewed scenes.
    // Geometry, source textures, alpha tests and native depth are unchanged.
    public static class SceneBackdropPolicyV25 {
        static bool SceneColor(string sceneId, out Color color) {
            switch (sceneId) {
                // V27 Outskirts candidate: Root-selected shared source-sky blue; ClearColor uses these same values.
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE1": color = new Color32(130,200,240,0); return true;
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE2": color = new Color32(130,200,240,0); return true;
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE4": color = new Color32(130,200,240,0); return true;
                case "tree-1": color = new Color32(45,155,157,0); return true;
                case "tree-2": color = new Color32(20,65,59,0); return true;
                case "tree-3": color = new Color32(133,210,178,0); return true;
                case "WI_ELDER_BENDERS_CAVE_STAGE1": color = new Color32(17,51,82,0); return true;
                case "WI_ELDER_BENDERS_CAVE_STAGE1_1": color = new Color32(17,51,82,0); return true;
                case "WI_ELDER_BENDERS_STAGE_BOSS": color = new Color32(17,51,82,0); return true;
                case "ruins-1": color = new Color32(111,219,234,0); return true;
                case "ruins-2": color = new Color32(212,244,244,0); return true;
                case "ruins-3": color = new Color32(223,193,143,0); return true;
                case "swamp-1": color = new Color32(212,230,211,0); return true;
                case "swamp-2": color = new Color32(173,163,111,0); return true;
                case "swamp-3": color = new Color32(185,210,209,0); return true;
                case "WI_ELDER_DARK_FOREST_STAGE_0": color = new Color32(9,24,49,0); return true;
                case "WI_ELDER_DARK_FOREST_STAGE_1": color = new Color32(30,48,92,0); return true;
                case "WI_ELDER_DARK_FOREST_BOSS_STAGE": color = new Color32(32,51,98,0); return true;
            }
            return SceneBackdropRegionsV32.TryColor(sceneId, out color);
        }

        public static Color ClearColor(string sceneId, Color fallback) {
            return SceneColor(sceneId, out var color) ? color : fallback;
        }

        static bool IsFlatPart(string sceneId, int index, string partName, string texturePath) {
            switch (sceneId) {
                // V27 Outskirts: only the three evidenced sky-dome tuples. Clouds/architectural facades stay original roles.
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE1":
                    return index == 2 && partName == "SKY_WC2.Y" && texturePath == "Pixel/native-world/outskirts/textures/2da61123ce0235b9ec85";
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE2":
                    return index == 2 && partName == "SKY_WC2.Y" && texturePath == "Pixel/native-world/outskirts/textures/2da61123ce0235b9ec85";
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE4":
                    return index == 2 && partName == "SKY_WC2.Y" && texturePath == "Pixel/native-world/outskirts/textures/2da61123ce0235b9ec85";
                case "tree-1":
                    if (index == 0 && partName == "RUBEN_EL_TREE_STAGE1_CLOUD01.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Down_Cloud04") return true;
                    if (index == 1 && partName == "RUBEN_EL_TREE_STAGE1_SKY.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Down_Sky") return true;
                    if (index == 2 && partName == "RUBEN_EL_TREE_STAGE1_CLOUD02.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Down_Cloud03") return true;
                    if (index == 16 && partName == "Ruben_El_Tree_Stage1_Bigtree01.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Down_Bigtree02") return true;
                    return false;
                case "tree-2":
                    if (index == 133 && partName == "Ruben_El_Tree_Stage2_Tree01.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall02") return true;
                    if (index == 134 && partName == "Ruben_El_Tree_Stage2_Tree01.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall01") return true;
                    if (index == 139 && partName == "Ruben_El_Tree_Stage2_Tree02.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall02") return true;
                    if (index == 140 && partName == "Ruben_El_Tree_Stage2_Tree02.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall01") return true;
                    if (index == 150 && partName == "Ruben_El_Tree_Stage2_Tree03.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall02") return true;
                    if (index == 151 && partName == "Ruben_El_Tree_Stage2_Tree03.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall01") return true;
                    if (index == 160 && partName == "Ruben_El_Tree_Stage2_Tree04.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall02") return true;
                    if (index == 161 && partName == "Ruben_El_Tree_Stage2_Tree04.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall01") return true;
                    if (index == 165 && partName == "Ruben_El_Tree_Stage2_Tree05.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall02") return true;
                    if (index == 166 && partName == "Ruben_El_Tree_Stage2_Tree05.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Middle_Tree_Back_Wall01") return true;
                    return false;
                case "tree-3":
                    if (index == 0 && partName == "RUBEN_EL_TREE_BOSS_CLOUD01.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Top_Cloud01") return true;
                    if (index == 1 && partName == "RUBEN_EL_TREE_BOSS_CLOUD02.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Top_Cloud02") return true;
                    if (index == 2 && partName == "RUBEN_EL_TREE_BOSS_SKY.Y" && texturePath == "Pixel/native-world/textures/Ruben_El_Tree_Top_Sky") return true;
                    return false;
                case "WI_ELDER_BENDERS_CAVE_STAGE1_1":
                    if (index == 315 && partName == "BENDERS_CAVE_STAGE1-1_SKY.Y" && texturePath == "Pixel/native-world/cave/textures/c6425fb192ef566b3ddc") return true;
                    if (index == 316 && partName == "BENDERS_CAVE_STAGE1-1_SKY.Y" && texturePath == "Pixel/native-world/cave/textures/4192df08474ec08cf3da") return true;
                    return false;
                case "WI_ELDER_BENDERS_STAGE_BOSS":
                    if (index == 0 && partName == "BENDERS_CAVE_STAGE3_SKY.Y" && texturePath == "Pixel/native-world/cave/textures/4192df08474ec08cf3da") return true;
                    if (index == 167 && partName == "Elder_Benders_Cave_Boss_Sky1.Y" && texturePath == "Pixel/native-world/cave/textures/c6425fb192ef566b3ddc") return true;
                    return false;
                case "ruins-1":
                    if (index == 107 && partName == "Ruben_Ruin_Of_Elf_S1_Mt2.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_MT1") return true;
                    if (index == 127 && partName == "Ruben_Ruin_Of_Elf_S1_Sky.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_of_Sky") return true;
                    return false;
                case "ruins-2":
                    if (index == 151 && partName == "Ruben_Ruin_Of_Elf_S2_Mt1.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_MT1") return true;
                    if (index == 153 && partName == "Ruben_Ruin_Of_Elf_S2_Mt2.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_MT1") return true;
                    if (index == 155 && partName == "Ruben_Ruin_Of_Elf_S2_Sky.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_of_Sky") return true;
                    return false;
                case "ruins-3":
                    if (index == 72 && partName == "Ruben_Ruin_Of_Elf_Boss_Mt.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_MT1") return true;
                    if (index == 73 && partName == "Ruben_Ruin_Of_Elf_Boss_Sky.Y" && texturePath == "Pixel/native-world/textures/Ruben_Ruin_Of_Elf_of_Sky") return true;
                    return false;
                case "swamp-1":
                    if (index == 159 && partName == "Ruben_Swamp_S1_mt1.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 161 && partName == "Ruben_Swamp_S1_mt2.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 163 && partName == "Ruben_Swamp_S1_mt3.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 165 && partName == "Ruben_Swamp_S1_Sky.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_Sky") return true;
                    return false;
                case "swamp-2":
                    if (index == 160 && partName == "Ruben_Swamp_S2_mt1.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 162 && partName == "Ruben_Swamp_S2_mt2.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 165 && partName == "Ruben_Swamp_S2_mt3.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 168 && partName == "Ruben_Swamp_S2_mt4.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 172 && partName == "Ruben_Swamp_S2_mt5.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 176 && partName == "Ruben_Swamp_S2_Sky.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_Sky") return true;
                    return false;
                case "swamp-3":
                    if (index == 114 && partName == "RUBEN_SWAMP_StageBoss_mt1.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 116 && partName == "RUBEN_SWAMP_StageBoss_mt2.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 118 && partName == "RUBEN_SWAMP_StageBoss_mt3.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 120 && partName == "RUBEN_SWAMP_StageBoss_mt4.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_MT1") return true;
                    if (index == 122 && partName == "RUBEN_SWAMP_StageBoss_sky.Y" && texturePath == "Pixel/native-world/textures/RUBEN_SWAMP_Sky") return true;
                    return false;
                case "WI_ELDER_DARK_FOREST_STAGE_0":
                    if (index == 354 && partName == "Elder_Darkforest_Stage1_Sky.Y" && texturePath == "Pixel/native-world/forest/textures/2955a8785e3d532ad5c0") return true;
                    return false;
                case "WI_ELDER_DARK_FOREST_STAGE_1":
                    if (index == 256 && partName == "Elder_Darkforest_Stage2_Sky.Y" && texturePath == "Pixel/native-world/forest/textures/2955a8785e3d532ad5c0") return true;
                    return false;
                case "WI_ELDER_DARK_FOREST_BOSS_STAGE":
                    if (index == 110 && partName == "Druid_Forest_Stage_Boss_Sky.Y" && texturePath == "Pixel/native-world/forest/textures/2955a8785e3d532ad5c0") return true;
                    return false;
            }
            return SceneBackdropRegionsV32.IsFlatPart(sceneId, index, partName, texturePath);
        }

        // Model detail uses the accepted group05 values even when a scene has
        // no separately identified flat backdrop and keeps its clear fallback.
        static bool IsModelScene(string sceneId) {
            switch (sceneId) {
                // V27 Outskirts model scope: reuse the unchanged accepted group05 formulas.
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE1":
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE2":
                case "WI_ELDER_WALLY_CASTLE_ROOF_STAGE4":
                case "WI_ELDER_BELOW_PASSAGE_STAGE2_1A":
                case "WI_ELDER_BELOW_PASSAGE_STAGE3_1A":
                case "WI_ELDER_BELOW_PASSAGE_STAGE4":
                    return true;
            }
            return SceneColor(sceneId, out _);
        }

        // Retained effect and uniform technical inputs keep the base settings
        // already supplied by SceneBackgroundStyleV25; no extra group05 pass.
        static bool IsRetainedWaterwaySource(string sceneId, string texturePath) {
            if (texturePath != "Pixel/native-world/waterway/textures/9400956c9be24e75f297"
                && texturePath != "Pixel/native-world/waterway/textures/c2869fc146fbe4fc05d1") return false;
            switch (sceneId) {
                case "WI_ELDER_BELOW_PASSAGE_STAGE2_1A":
                case "WI_ELDER_BELOW_PASSAGE_STAGE3_1A":
                case "WI_ELDER_BELOW_PASSAGE_STAGE4":
                    return true;
            }
            return false;
        }

        // Exact original object/material evidence: these16 waterway parts had
        // bAlphaBlend=true / SetAlphaObject(true); no additive-factor evidence.
        static bool IsWaterwaySourceAlphaPart(string sceneId, int index, string partName, string texturePath) {
            switch (sceneId) {
                case "WI_ELDER_BELOW_PASSAGE_STAGE2_1A":
                    return (index == 173 && partName == "Elder_Below_Passage_stage2_1A_lightalpha1.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 174 && partName == "Elder_Below_Passage_stage2_1A_lightalpha2.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 175 && partName == "Elder_Below_Passage_stage2_1A_lightalpha3.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 177 && partName == "SPIDER_BG.X" && texturePath == "Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9")
                        || (index == 179 && partName == "SPIDER_BG.X" && texturePath == "Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9")
                        || (index == 181 && partName == "SPIDER_BG.X" && texturePath == "Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9");
                case "WI_ELDER_BELOW_PASSAGE_STAGE3_1A":
                    return (index == 231 && partName == "Elder_Below_Passage_stage3_1A_Lightalpha1.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 232 && partName == "Elder_Below_Passage_stage3_1A_Lightalpha2.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 233 && partName == "Elder_Below_Passage_stage3_1A_Lightalpha3.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 234 && partName == "Elder_Below_Passage_stage3_1A_Lightalpha4.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 235 && partName == "Elder_Below_Passage_stage3_1A_Lightalpha5.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 237 && partName == "SPIDER_BG.X" && texturePath == "Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9")
                        || (index == 239 && partName == "SPIDER_BG.X" && texturePath == "Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9");
                case "WI_ELDER_BELOW_PASSAGE_STAGE4":
                    return (index == 120 && partName == "Elder_Below_Passage_stage4_lightalpha1.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 121 && partName == "Elder_Below_Passage_stage4_lightalpha2.Y" && texturePath == "Pixel/native-world/waterway/textures/9400956c9be24e75f297")
                        || (index == 123 && partName == "SPIDER_BG.X" && texturePath == "Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9");
            }
            return false;
        }

        static Shader waterwaySourceAlphaShader;
        static readonly string[] sourceAlphaFloats = {
            "_FogNear", "_FogFar", "_FogHeightStart", "_FogHeightEnd", "_FogBias", "_Cull",
            "_DetailReduction", "_DetailRadius", "_ColorSteps", "_BackdropCoverage", "_FlatFarEnabled"
        };
        static void ConfigureWaterwaySourceAlpha(Material material, string sceneId, int index, string partName, string texturePath) {
            if (!IsWaterwaySourceAlphaPart(sceneId, index, partName, texturePath)) return;
            if (!waterwaySourceAlphaShader)
                waterwaySourceAlphaShader = Resources.Load<Shader>("Pixel/Shaders/WaterwaySourceAlphaV26");
            if (!waterwaySourceAlphaShader)
                throw new InvalidOperationException("Missing exact waterway source-alpha shader resource");
            // Preserve every property of the source material across the shader swap.
            var tex = material.mainTexture; var scale = material.mainTextureScale; var offset = material.mainTextureOffset;
            var color = material.GetColor("_Color"); var fog = material.GetColor("_FogColor"); var flat = material.GetColor("_FlatFarColor");
            var values = new float[sourceAlphaFloats.Length];
            for (int i = 0; i < values.Length; i++) values[i] = material.GetFloat(sourceAlphaFloats[i]);
            material.shader = waterwaySourceAlphaShader;
            material.mainTexture = tex; material.mainTextureScale = scale; material.mainTextureOffset = offset;
            material.SetColor("_Color", color); material.SetColor("_FogColor", fog); material.SetColor("_FlatFarColor", flat);
            for (int i = 0; i < values.Length; i++) material.SetFloat(sourceAlphaFloats[i], values[i]);
        }

        public static void Configure(Material material, string sceneId, int index, string partName, string texturePath) {
            if (!material || !IsModelScene(sceneId)) return;
            ConfigureWaterwaySourceAlpha(material, sceneId, index, partName, texturePath);
            NativeSourceAlphaPolicyV27.Configure(material, sceneId, index, partName, texturePath);
            NativeExplicitAlphaPolicyV27.Configure(material, sceneId, index, partName, texturePath);
            if (IsRetainedWaterwaySource(sceneId, texturePath)) return;
            var texture = material.mainTexture as Texture2D;
            var settings = SceneBackgroundStyleV25.Classify(partName, texturePath, texture ? texture.width : 0, texture ? texture.height : 0);
            const float t = (5 - 1) / 9f;
            if (settings.role == "foliage") {
                material.SetFloat("_DetailReduction", Mathf.Lerp(.4f, .95f, t));
                material.SetFloat("_DetailRadius", Mathf.Lerp(1.5f, 4, t));
            } else if (settings.role == "surface") {
                material.SetFloat("_DetailReduction", Mathf.Lerp(.12f, .55f, t));
                material.SetFloat("_DetailRadius", Mathf.Lerp(1, 2, t));
            } else if (settings.role == "scenery") {
                material.SetFloat("_DetailReduction", Mathf.Lerp(.25f, .9f, t));
                material.SetFloat("_DetailRadius", Mathf.Lerp(1.25f, 4, t));
            }
            if (SceneColor(sceneId, out var color) && IsFlatPart(sceneId, index, partName, texturePath)) {
                material.SetFloat("_FlatFarEnabled", 1);
                material.SetColor("_FlatFarColor", color);
            }
            ForestGroundFaceDisplayV28.Configure(material, sceneId, index, partName, texturePath);
        }
    }
}
