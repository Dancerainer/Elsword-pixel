using UnityEngine;

namespace ElswordPixel {
    // Extends the approved far-color / group05 treatment to the remaining routes.
    // Exact tuples come from current Resources scenes; collision, alpha and depth stay native.
    public static class SceneBackdropRegionsV32 {
        public static bool TryColor(string sceneId, out Color color) {
            switch (sceneId) {
                case "WI_BESMA_DRAGON_NEST_STAGE1": color = new Color32(147,67,31,0); return true;
                case "WI_BESMA_DRAGON_NEST_STAGE2": color = new Color32(147,67,31,0); return true;
                case "WI_BESMA_DRAGON_NEST_STAGE4": color = new Color32(147,67,31,0); return true;
                case "WI_NEW_BESMA_LAKE_STAGE1": color = new Color32(116,194,219,0); return true;
                case "WI_NEW_BESMA_LAKE_STAGE2A": color = new Color32(116,194,219,0); return true;
                case "WI_BESMA_LAKE_STAGE4": color = new Color32(116,194,219,0); return true;
                case "WI_NEW_BESMA_LAKE_NIGHT_STAGE1": color = new Color32(29,48,89,0); return true;
                case "WI_NEW_BESMA_LAKE_NIGHT_STAGE2A": color = new Color32(29,48,89,0); return true;
                case "WI_BESMA_LAKE_NIGHT_STAGE4": color = new Color32(29,48,89,0); return true;
                case "WI_ELDER_WALLY_CASTLE_CENTER_STAGE3": color = new Color32(93,181,220,0); return true;
                case "WI_ELDER_WALLY_CASTLE_CENTER_STAGE4": color = new Color32(93,181,220,0); return true;
                case "WI_NEW_BESMA_DRAGON_ROAD_STAGE1": color = new Color32(116,194,219,0); return true;
                case "WI_NEW_BESMA_DRAGON_ROAD_STAGE2A": color = new Color32(116,194,219,0); return true;
                case "WI_NEW_BESMA_DRAGON_ROAD_STAGE4": color = new Color32(116,194,219,0); return true;
                case "WI_ALTERA_BELOW_TUNNEL_STAGE1": color = new Color32(28,36,38,0); return true;
                case "WI_ALTERA_BELOW_TUNNEL_STAGE2A": color = new Color32(28,36,38,0); return true;
                case "WI_ALTERA_BELOW_TUNNEL_STAGE4": color = new Color32(28,36,38,0); return true;
                case "WI_ALTERA_CORE_STAGE1": color = new Color32(17,39,51,0); return true;
                case "WI_ALTERA_CORE_STAGE1_MIDDLE": color = new Color32(17,39,51,0); return true;
                case "WI_ALTERA_CORE_STAGE2": color = new Color32(17,39,51,0); return true;
                case "WI_ALTERA_CORE_STAGE2_MIDDLE": color = new Color32(17,39,51,0); return true;
                case "WI_ALTERA_CORE_BOSS_STAGE": color = new Color32(17,39,51,0); return true;
                case "WI_ALTERA_NASOD_FACTORY_STAGE1": color = new Color32(46,41,49,0); return true;
                case "WI_ALTERA_NASOD_FACTORY_STAGE2": color = new Color32(150,102,113,0); return true;
                case "WI_ALTERA_NASOD_FACTORY_BOSS": color = new Color32(46,41,49,0); return true;
                case "WI_ALTERA_PLAIN_STAGE1": color = new Color32(103,158,191,0); return true;
                case "WI_ALTERA_PLAIN_STAGE2": color = new Color32(103,158,191,0); return true;
                case "WI_ALTERA_PLAIN_STAGE3": color = new Color32(103,158,191,0); return true;
                case "WI_ALTERA_PLAIN_RECYCLE_STAGE1": color = new Color32(150,102,113,0); return true;
                case "WI_ALTERA_PLAIN_RECYCLE_STAGE2": color = new Color32(150,102,113,0); return true;
                case "WI_ALTERA_PLAIN_RECYCLE_STAGE3": color = new Color32(150,102,113,0); return true;
                case "WI_BESMA_MINE2_STAGE1": color = new Color32(36,32,43,0); return true;
                case "WI_BESMA_MINE2_STAGE2A": color = new Color32(36,32,43,0); return true;
                case "WI_BESMA_MINE2_STAGE4": color = new Color32(36,32,43,0); return true;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE1": color = new Color32(37,44,75,0); return true;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE2": color = new Color32(37,44,75,0); return true;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE3": color = new Color32(37,44,75,0); return true;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE4": color = new Color32(37,44,75,0); return true;
                case "WI_BESMA_AIRSHIP_STAGE1": color = new Color32(101,155,188,0); return true;
                case "WI_BESMA_AIRSHIP_STAGE2A": color = new Color32(101,155,188,0); return true;
                case "WI_BESMA_AIRSHIP_STAGE5": color = new Color32(101,155,188,0); return true;
            }
            // Median opaque RGB of each Feita area's authored sky or back-wall
            // texture. This also colors uncovered space between native meshes.
            if (sceneId != null && sceneId.StartsWith("WI_PEITA_", System.StringComparison.Ordinal)) {
                if (sceneId.StartsWith("WI_PEITA_OFFERINGS_ALTER_")) color = new Color32(75,35,19,0);
                else if (sceneId.StartsWith("WI_PEITA_OFFERINGS_")) color = new Color32(103,60,66,0);
                else if (sceneId.StartsWith("WI_PEITA_SPIRAL_CORRIDOR_")) color = new Color32(36,11,47,0);
                else if (sceneId.StartsWith("WI_PEITA_UNDER_GARDEN_")) color = new Color32(40,66,60,0);
                else if (sceneId.StartsWith("WI_PEITA_TOWER_HEART_")) color = new Color32(36,22,24,0);
                else if (sceneId.StartsWith("WI_PEITA_CHAPEL_")) color = new Color32(0,0,0,0);
                else { color = default; return false; }
                return true;
            }
            color = default; return false;
        }

        public static bool IsFlatPart(string sceneId, int index, string partName, string texturePath) {
            if (sceneId != null && sceneId.StartsWith("WI_PEITA_", System.StringComparison.Ordinal))
                return partName != null && partName.EndsWith("Sky.Y", System.StringComparison.OrdinalIgnoreCase) &&
                    (texturePath == "Pixel/peita/textures/peita_offerings_sky" ||
                     texturePath == "Pixel/peita/textures/peita_spiral_corridor_sky" ||
                     texturePath == "Pixel/peita/textures/peita_offerings_alter_sky");
            switch (sceneId) {
                case "WI_BESMA_DRAGON_NEST_STAGE1":
                    if (index == 24 && partName == "Besma_Dragon_Nest_Stage1_Sky.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    if (index == 2 && partName == "Besma_Dragon_Nest_Stage1_Back3.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    return false;
                case "WI_BESMA_DRAGON_NEST_STAGE2":
                    if (index == 0 && partName == "BESMA_DRAGON_NEST_STAGE2_SKY.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    if (index == 4 && partName == "Besma_Dragon_Nest_Stage2_Back1.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    return false;
                case "WI_BESMA_DRAGON_NEST_STAGE4":
                    if (index == 0 && partName == "BESMA_DRAGON_NEST_STAGE4_SKY.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    if (index == 1 && partName == "Besma_Dragon_Nest_Stage4_Back1.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    if (index == 2 && partName == "Besma_Dragon_Nest_Stage4_Back2.Y" && texturePath == "Pixel/native-world/besma-dragon-nest/textures/a0231a1be5a45e6aa57c6421580e3fabe3bb36927e0b1a6f58a69c3661b289b6") return true;
                    return false;
                case "WI_NEW_BESMA_LAKE_STAGE1":
                    if (index == 0 && partName == "BESMA_BESMA_LAKE_STAGE1_SKY.Y" && texturePath == "Pixel/native-world/besma-lake/textures/c4ede11c306a5e8a6a8b") return true;
                    return false;
                case "WI_NEW_BESMA_LAKE_STAGE2A":
                    if (index == 0 && partName == "BESMA_BESMA_LAKE_STAGE3A_SKY.Y" && texturePath == "Pixel/native-world/besma-lake/textures/c4ede11c306a5e8a6a8b") return true;
                    return false;
                case "WI_BESMA_LAKE_STAGE4":
                    if (index == 113 && partName == "Besma_Besma_Lake_Stage4_Sky.Y" && texturePath == "Pixel/native-world/besma-lake/textures/c4ede11c306a5e8a6a8b") return true;
                    return false;
                case "WI_NEW_BESMA_LAKE_NIGHT_STAGE1":
                    if (index == 0 && partName == "BESMA_BESMA_LAKE_STAGE1_SKY_NIGHT.Y" && texturePath == "Pixel/native-world/besma-lake-night/textures/3940a2aa20be3b1d1c22") return true;
                    return false;
                case "WI_NEW_BESMA_LAKE_NIGHT_STAGE2A":
                    if (index == 0 && partName == "BESMA_BESMA_LAKE_STAGE3A_SKY_NIGHT.Y" && texturePath == "Pixel/native-world/besma-lake-night/textures/3940a2aa20be3b1d1c22") return true;
                    return false;
                case "WI_BESMA_LAKE_NIGHT_STAGE4":
                    if (index == 111 && partName == "Besma_Besma_Lake_Stage4_Sky_Night.Y" && texturePath == "Pixel/native-world/besma-lake-night/textures/3940a2aa20be3b1d1c22") return true;
                    return false;
                case "WI_ELDER_WALLY_CASTLE_CENTER_STAGE3":
                    if (index == 0 && partName == "SKY_FOE.Y" && texturePath == "Pixel/native-world/castle-center/textures/d7af8063111255440f62") return true;
                    if (index == 1 && partName == "SKY_FOE.Y" && texturePath == "Pixel/native-world/castle-center/textures/f5bd14dd1b1a47e617e6") return true;
                    return false;
                case "WI_ELDER_WALLY_CASTLE_CENTER_STAGE4":
                    if (index == 0 && partName == "SKY_FOE.Y" && texturePath == "Pixel/native-world/castle-center/textures/d7af8063111255440f62") return true;
                    if (index == 1 && partName == "SKY_FOE.Y" && texturePath == "Pixel/native-world/castle-center/textures/f5bd14dd1b1a47e617e6") return true;
                    return false;
                case "WI_NEW_BESMA_DRAGON_ROAD_STAGE1":
                    if (index == 0 && partName == "BESMA_DRAGON_ROAD_STAGE1_SKY.Y" && texturePath == "Pixel/native-world/dragon-road/textures/c4ede11c306a5e8a6a8b") return true;
                    return false;
                case "WI_NEW_BESMA_DRAGON_ROAD_STAGE2A":
                    if (index == 0 && partName == "BESMA_DRAGON_ROAD_STAGE2-1A_SKY.Y" && texturePath == "Pixel/native-world/dragon-road/textures/c4ede11c306a5e8a6a8b") return true;
                    if (index == 1 && partName == "BESMA_DRAGON_ROAD_STAGE2-1A_SKY.Y" && texturePath == "Pixel/native-world/dragon-road/textures/4b079acdf032dd251193") return true;
                    return false;
                case "WI_NEW_BESMA_DRAGON_ROAD_STAGE4":
                    if (index == 87 && partName == "Besma_Dragon_Road_Boss_Sky.Y" && texturePath == "Pixel/native-world/dragon-road/textures/c4ede11c306a5e8a6a8b") return true;
                    return false;
                case "WI_ALTERA_NASOD_FACTORY_STAGE2":
                    if (index == 403 && partName == "Altera_Nasod_Factory_Stage2_Sky.Y" && texturePath == System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\.codex\\runtime-drafts\\factory-native-scene-lookup-r676\\world-assets-r676\\textures\\Altera_Dungeon_Gate_Sky.png") return true;
                    return false;
                case "WI_ALTERA_PLAIN_STAGE1":
                    if (index == 214 && partName == "Altera_Plain_stage1_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Ruben_Field_Sky.png") return true;
                    return false;
                case "WI_ALTERA_PLAIN_STAGE2":
                    if (index == 284 && partName == "Altera_Plain_stage2_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Ruben_Field_Sky.png") return true;
                    return false;
                case "WI_ALTERA_PLAIN_STAGE3":
                    if (index == 83 && partName == "Altera_Plain_stage Boss_Sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Ruben_Field_Sky.png") return true;
                    return false;
                case "WI_ALTERA_PLAIN_RECYCLE_STAGE1":
                    if (index == 441 && partName == "Altera_Plain_stage1_Skydom.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Dungeon_Gate_Sky.png") return true;
                    return false;
                case "WI_ALTERA_PLAIN_RECYCLE_STAGE2":
                    if (index == 198 && partName == "ALTERA_PLAIN_RECYCLE_STAGE2-1B_SKY.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Dungeon_Gate_Sky.png") return true;
                    return false;
                case "WI_ALTERA_PLAIN_RECYCLE_STAGE3":
                    if (index == 208 && partName == "Altera_Plain_Recycle_Boss_Sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Dungeon_Gate_Sky.png") return true;
                    return false;
                case "WI_BESMA_MINE2_STAGE4":
                    if (index == 12 && partName == "Besma_besma_Mine_s4_bg.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\native-regions-v08\\textures\\besma_besma_mine_nightsky.png") return true;
                    return false;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE1":
                    if (index == 404 && partName == "Altera_Battle_Air_Ship_s1_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Battle_Air_sky_night.png") return true;
                    return false;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE2":
                    if (index == 554 && partName == "Altera_Battle_Air_Ship_s2_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Battle_Air_sky_night.png") return true;
                    if (index == 555 && partName == "Altera_Battle_Air_Ship_s2_sky2.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Battle_Air_sky_night.png") return true;
                    return false;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE3":
                    if (index == 468 && partName == "Altera_Battle_Air_Ship_s3_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Battle_Air_sky_night.png") return true;
                    return false;
                case "WI_ALTERA_BATTLE_AIR_SHIP_STAGE4":
                    if (index == 525 && partName == "Altera_Battle_Air_Ship_s4_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Battle_Air_sky_night.png") return true;
                    if (index == 526 && partName == "Altera_Battle_Air_Ship_s4_sky_01.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\altera-v17\\native\\textures\\Altera_Battle_Air_sky_night.png") return true;
                    return false;
                case "WI_BESMA_AIRSHIP_STAGE1":
                    if (index == 8 && partName == "Besma_Air_Ship_S2a_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\native-regions-v08\\textures\\Besma_Air_Sky01.png") return true;
                    return false;
                case "WI_BESMA_AIRSHIP_STAGE5":
                    if (index == 69 && partName == "Besma_Air_Ship_s5_sky.Y" && texturePath == "D:\\elsword_pixel\\art\\production\\lk-native-v02\\dungeon\\native-regions-v08\\textures\\Besma_Air_Sky01.png") return true;
                    return false;
            }
            return false;
        }
    }
}
