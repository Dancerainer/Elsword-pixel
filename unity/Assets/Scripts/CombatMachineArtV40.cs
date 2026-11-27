using System;
using UnityEngine;

namespace ElswordPixel {
    internal static class CombatMachineArtV40 {
        const string Root = "Pixel/combat-machines-v40/";

        // Ordinary waterway and lake libraries share their source frame layout with the drawings.
        internal static Texture2D LoadBase(string path) {
            switch(path) {
                case "Pixel/monsters/mousehouse/atlas": return Load("mousehouse");
                case "Pixel/monsters/besma-lake/besma-totem-base/atlas": return Load("besma-totem-base");
                case "Pixel/monsters/down-stone-trap/atlas": return Load("down-stone-trap");
                case "Pixel/monsters/dragon-road/spikeboard-herb/atlas": return Load("spikeboard-herb");
                case "Pixel/monsters/outskirts/wood-post-trap/atlas": return Load("wood-post-trap");
                default: return null;
            }
        }

        // The owning dungeon loaders verify the original page before requesting its display art.
        internal static Texture2D ProductPage(string npc, Texture2D source) {
            string kind;
            switch(npc) {
                case "NUI_SHOCK_STICK": kind="shock-stick"; break;
                case "NUI_BARRIERMACHINE_FIRE": kind="barriermachine-fire"; break;
                case "NUI_BARRIERMACHINE_ICE": kind="barriermachine-ice"; break;
                case "NUI_NASOD_MOUSE_HOUSE": kind="nasod-mouse-house"; break;
                case "NUI_NASOD_RAIL_HOUSE": kind="nasod-rail-house"; break;
                case "NUI_DOWN_STONE_TRAP_BESMA": kind="down-stone-trap-besma"; break;
                case "NUI_ALTERA_MINE": kind="altera-mine"; break;
                default: return source;
            }
            var painted=Load(kind);
            if(painted.width!=source.width || painted.height!=source.height)
                throw new InvalidOperationException("Combat mechanism source page dimensions changed: "+npc);
            return painted;
        }

        static Texture2D Load(string kind) {
            var page=Resources.Load<Texture2D>(Root+kind);
            if(!page || page.filterMode!=FilterMode.Point)
                throw new InvalidOperationException("Missing point-sampled combat mechanism drawing: "+kind);
            return page;
        }
    }
}
