using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    internal static class LegacyBodyRegistrationV39 {
        internal sealed class Receipt {
            internal readonly string kind,npc,route,version,idle;
            internal readonly int width,height,top,bottom;
            internal readonly float neutralFoot,target,factor;
            internal Receipt(string kind,string npc,string route,string version,string idle,int width,int height,int top,int bottom,float neutralFoot,float target){
                this.kind=kind;this.npc=npc;this.route=route;this.version=version;this.idle=idle;
                this.width=width;this.height=height;this.top=top;this.bottom=bottom;this.neutralFoot=neutralFoot;this.target=target;factor=target/(bottom-top);
            }
        }
        static readonly Receipt[] receipts={
            new Receipt("nasod-miner-thin-defect","NUI_NASOD_MINER_THIN_DEFECT","altera-below-tunnel","below-tunnel-thin-independent-whole-pose-v21-r1","idle",192,192,86,142,2f,75.89116112f),
            new Receipt("nasod-miner-thin-defect","NUI_NASOD_MINER_THIN_DEFECT","altera-plain-recycle","below-tunnel-thin-independent-whole-pose-v21-r1","idle",192,192,86,142,2f,75.89116112f),
            new Receipt("komodo","NUI_KOMODO","besma-lake","dragon-road-whole-pose-v17-r1","idle",63,57,13,41,0f,39.12412119f),
            new Receipt("komodo","NUI_KOMODO","dragon-road","dragon-road-whole-pose-v17-r1","idle",63,57,13,41,0f,39.12412119f),
            new Receipt("saurus-high","NUI_SAURUS_HIGH","besma-dragon-nest","besma-lake-night-complete-whole-pose-v19-r1","idle",148,138,18,106,0f,121.02440764f),
            new Receipt("saurus-high","NUI_SAURUS_HIGH","besma-lake-night","besma-lake-night-complete-whole-pose-v19-r1","idle",148,138,18,106,0f,121.02440764f),
            new Receipt("lizardman-shaman","NUI_LIZARDMAN_SHAMAN","besma-lake","lizardman-shaman-motion-overlay-v18-r1","idle",101,90,14,71,0f,90.84810752f),
            new Receipt("lizardman-shaman","NUI_LIZARDMAN_SHAMAN","besma-lake-night","lizardman-shaman-motion-overlay-v18-r1","idle",101,90,14,71,0f,90.84810752f),
            new Receipt("fire-komodo","NUI_FIRE_KOMODO","besma-dragon-nest","besma-lake-night-fire-komodo-whole-pose-v19-r1","idle",136,108,52,81,0f,39.12412119f),
            new Receipt("fire-komodo","NUI_FIRE_KOMODO","besma-lake-night","besma-lake-night-fire-komodo-whole-pose-v19-r1","idle",136,108,52,81,0f,39.12412119f),
            new Receipt("lizardman-saurus","NUI_LIZARDMAN_SAURUS","besma-lake","dragon-road-whole-pose-v17-r1","idle",134,103,3,87,0f,110.02218795f),
            new Receipt("lizardman-saurus","NUI_LIZARDMAN_SAURUS","dragon-road","dragon-road-whole-pose-v17-r1","idle",134,103,3,87,0f,110.02218795f),
            new Receipt("nasod-miner-thin","NUI_NASOD_MINER_THIN","bethma-richie-mine","mine2-two-miners-whole-pose-v20-r1","idle",68,66,7,58,1f,75.89115938f),
            new Receipt("nasod-miner-fat-b","NUI_NASOD_MINER_FAT_B","bethma-richie-mine","mine2-two-miners-whole-pose-v20-r1","idle",99,91,18,80,0f,81.64778091f),
            new Receipt("lizardman-spear","NUI_LIZARDMAN_SPEAR","besma-lake","dragon-road-whole-pose-v17-r1","idle",128,90,5,79,0f,98.16624736f),
            new Receipt("lizardman-spear","NUI_LIZARDMAN_SPEAR","dragon-road","dragon-road-whole-pose-v17-r1","idle",128,90,5,79,0f,98.16624736f),
            new Receipt("nasod-healer","NUI_NASOD_HEALER","bethma-richie-mine","mine2-fat-c-healer-whole-pose-v20-r1","idle",38,38,2,35,-15f,44.6026551f),
            new Receipt("nasod-miner-thin-defect-die","NUI_NASOD_MINER_THIN_DEFECT_DIE","altera-below-tunnel","below-defect-die-approved-pose-reuse-plus-whole-deltas-v21-r1","idle",192,192,86,142,0f,75.89116438f),
            new Receipt("pporu-william-thief","NUI_PPORU_WILLIAM_THIEF","wally-castle-outskirts","outskirts-whole-pose-v14-r1","source-idle",114,53,1,48,0f,61.51108299f),
            new Receipt("pporu-william-thief-fire","NUI_PPORU_WILLIAM_THIEF_FIRE","wally-castle-outskirts","outskirts-whole-pose-v14-r1","source-idle",117,55,4,51,0f,61.51107283f),
            new Receipt("soldier-benders","NUI_SOLDIER_BENDERS","wally-castle-center","castle-center-whole-pose-v15-r1","idle",83,84,14,76,0f,88.27531325f),
            new Receipt("lizardman-shaman-fire","NUI_LIZARDMAN_SHAMAN_FIRE","besma-dragon-nest","besma-dragon-nest-whole-pose-v20-r1","idle",83,77,15,69,0f,90.84810752f),
            new Receipt("monkey-bomb","NUI_MONKEY_BOMB","wally-castle-outskirts","outskirts-whole-pose-v14-r1","source-idle",86,54,0,36,0f,48.15780693f),
            new Receipt("lizardman-spear-dark","NUI_LIZARDMAN_SPEAR_DARK","besma-lake-night","besma-lake-night-complete-whole-pose-v19-r1","idle",117,111,17,92,0f,98.38574267f),
            new Receipt("lizardman-spear-red","NUI_LIZARDMAN_SPEAR_RED","besma-dragon-nest","besma-dragon-nest-whole-pose-v20-r1","idle",150,100,4,78,0f,98.16622469f),
            new Receipt("armadillo-earth","NUI_ARMADILLO_EARTH","altera-below-tunnel","below-tunnel-armadillo-original-available-whole-pose-v21-r1","idle",192,192,108,144,0f,47.52311043f),
            new Receipt("curse-wisp","NUI_CURSE_WISP","dark-forest","pixel-source-state-candidate-v08","idle",48,52,5,45,0f,52.75222315f),
            new Receipt("nasod-miner-fat-c-defect-die","NUI_NASOD_MINER_FAT_C_DEFECT_DIE","altera-below-tunnel","below-defect-die-approved-pose-reuse-plus-whole-deltas-v21-r1","idle",192,192,74,143,1f,90.68823839f),
            new Receipt("scorpion-fire","NUI_SCORPION_FIRE","besma-dragon-nest","besma-dragon-nest-whole-pose-v20-r1","idle",86,80,2,62,0f,81.50135956f),
            new Receipt("william-thief","NUI_PPORU_WILLIAM_CUTE_THIEF","ruins","pixel-whole-pose-v07","idle",75,71,9,57,0f,65.71072772f),
            new Receipt("armadillo","NUI_ARMADILLO","besma-lake-night","besma-lake-night-armadillo-whole-pose-v19-r1","idle",63,58,17,53,0f,47.52308845f),
            new Receipt("bat-b-fire","NUI_BAT_B_FIRE","besma-dragon-nest","besma-dragon-nest-whole-pose-v20-r1","idle",73,67,0,57,1f,76.57544154f),
            new Receipt("scorpion","NUI_SCORPION","besma-lake","dragon-road-whole-pose-v17-r1","idle",89,88,4,65,0f,81.50135956f),
            new Receipt("scorpion","NUI_SCORPION","dragon-road","dragon-road-whole-pose-v17-r1","idle",89,88,4,65,0f,81.50135956f),
            new Receipt("anger-forest-knight","NUI_ANGER_FOREST_KNIGHT","dark-forest","pixel-source-state-candidate-v08","idle",110,89,5,85,0f,105.06769287f),
            new Receipt("bat-sonic","NUI_BAT_SONIC","altera-below-tunnel","below-tunnel-fat-bat-whole-pose-v21-r1","idle",256,256,105,192,0f,114.93862947f),
            new Receipt("cactus","NUI_CACTUS","dragon-road","dragon-road-whole-pose-v17-r1","idle",65,74,5,68,0f,83.86051459f),
            new Receipt("nasod-miner-fat-c","NUI_NASOD_MINER_FAT_C","bethma-richie-mine","mine2-fat-c-healer-whole-pose-v20-r1","idle",122,119,20,100,-1f,108.82587279f),
            new Receipt("nasod-miner-fat-c-defect","NUI_NASOD_MINER_FAT_C_DEFECT","altera-below-tunnel","below-tunnel-fat-bat-whole-pose-v21-r1","idle",192,192,74,143,1f,90.68823839f),
            new Receipt("soldier-crossbow","NUI_SOLDIER_CROSSBOW","wally-castle-center","castle-center-whole-pose-v15-r1","idle",94,62,3,51,0f,63.17426163f),
            new Receipt("william-cute","NUI_PPORU_WILLIAM_CUTE","ruins","pixel-whole-pose-v07","idle",75,81,21,68,0f,65.7107277f),
            new Receipt("lizardman-high","NUI_LIZARDMAN_HIGH","dragon-road","dragon-road-whole-pose-v17-r1","idle",124,119,4,99,0f,126.90372393f),
            new Receipt("lizardman-high-dark-new","NUI_LIZARDMAN_HIGH_DARK_NEW","besma-lake-night","besma-lake-night-complete-whole-pose-v19-r1","idle",133,139,6,112,0f,141.92923853f),
            new Receipt("lizardman-older","NUI_LIZARDMAN_OLDER","besma-lake","besma-lake-combined-motion-v18-r2","idle",142,115,4,108,0f,129.17247313f),
            new Receipt("lizardman-shaman-king","NUI_LIZARDMAN_SHAMAN_KING","besma-dragon-nest","besma-dragon-nest-whole-pose-v20-r1","idle",124,110,4,103,0f,134.06258124f),
            new Receipt("wally-8th","NUI_WALLY_8TH","wally-castle-center","castle-center-whole-pose-v15-r1","idle",134,133,3,114,0f,146.3230625f),
            new Receipt("wally-8th-mk2","NUI_WALLY_8TH_MK2","bethma-richie-mine","mine2-wally-mk2-whole-pose-v20-r2","idle",143,147,5,128,-1f,162.2719728f),
            new Receipt("parasite","NUI_PARASITE","altera-below-tunnel","below-parasite-v21-r1","idle",512,512,357,382,18f,41.22506196f),
        };
        static readonly Dictionary<string,List<Receipt>> actors=Build();
        static Dictionary<string,List<Receipt>> Build(){
            var result=new Dictionary<string,List<Receipt>>(StringComparer.Ordinal);
            foreach(var receipt in receipts){
                if(!result.TryGetValue(receipt.kind,out var entries)){entries=new List<Receipt>();result.Add(receipt.kind,entries);}
                entries.Add(receipt);
            }
            return result;
        }
        internal static Receipt Find(Enemy actor,string route,string version){
            if(actor==null||actor.visualScale!=1||string.IsNullOrEmpty(actor.kind)||!actors.TryGetValue(actor.kind,out var entries))return null;
            foreach(var row in entries)if(row.npc==actor.sourceNPC&&row.route==route&&row.version==version)return row;
            return null;
        }
    }
    public sealed partial class DemoScene {
        float RegisterLegacyBodyV39(Enemy actor,ref float scale){
            if(!libraries.TryGetValue(actor.kind,out var bank)||bank.data.logicalScale!=1)return 0;
            var receipt=LegacyBodyRegistrationV39.Find(actor,Dungeon.id,bank.data.artVersion);
            if(receipt==null)return 0;
            if(SourceMotionInkV45.UsesScorpionPages(actor.kind)){
                // The registered neutral canvas includes transparent borders. The painted
                // atlas trims those borders while keeping the same signed foot and height.
                if(!bank.clips.TryGetValue(receipt.idle,out var idle)||idle.frames.Length==0||
                    idle.frames[0].w!=receipt.width||idle.frames[0].h!=receipt.height)return 0;
            }else if(!bank.frames.TryGetValue(receipt.idle,out var frames)||frames.Length==0||!frames[0]||
                frames[0].rect.width!=receipt.width||frames[0].rect.height!=receipt.height)return 0;
            float initial=scale;scale*=receipt.factor;
            // Scale around the measured standing sole, or the existing neutral
            // hover baseline for flying actors. Every active original clip keeps
            // its own anchor, facing, clock and relative height above that root.
            return receipt.neutralFoot*(initial-scale);
        }
    }
}
