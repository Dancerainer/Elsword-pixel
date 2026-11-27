using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public static class CannonRigV35 {
        const string Prefix = "Pixel/feedback-v35/";
        public static string Kind(string npc) {
            switch (npc) {
                case "NUI_CANNON_MINI": return "cannon-mini";
                case "NUI_CANNON_HEAVY_MACHINE_GUN": return "cannon-heavy-machine-gun";
                case "NUI_CANNON_BIG": return "cannon-big";
                default: return null;
            }
        }
        public static FairyPoseLibraryV33 Load(string npc) {
            string kind = Kind(npc);
            if (kind == null) throw new ArgumentException("Unknown registered airship cannon " + npc);
            string hash;
            int pieces, frames;
            string[] aliases;
            switch (kind) {
                case "cannon-mini":
                    hash = "b7c9dcba7775b0c36739ca2bc8520831b7c5bba292429341d590e1982c5d921c";
                    pieces = 4; frames = 229;
                    aliases = new[] { "idle", "start", "attack-a-ready", "attack-a" };
                    break;
                case "cannon-heavy-machine-gun":
                    hash = "06a625299806abdb4da48aa9f3250d97b5825182e6310861e7b18d46b919a05d";
                    pieces = 10; frames = 379;
                    aliases = new[] { "idle", "source-attack-ready", "source-attack1", "source-attack2", "source-attack3", "broken" };
                    break;
                default:
                    hash = "de0b1690a59ef7cb61cf62e1f549f2e68bc1179a4e5058159ff49315e66703d8";
                    pieces = 3; frames = 360;
                    aliases = new[] { "idle", "source-attack1", "source-attack3", "source-down" };
                    break;
            }
            return RegisteredPartRigV35.Load(Prefix + kind + "/",npc,hash, bank => {
                if (bank.authoredFacing != 1 || bank.nativeYaw != 0 || bank.parts.Length != pieces ||
                    bank.tracks.Length != aliases.Length) throw new InvalidOperationException("Cannon source identity " + npc);
                int count=0;
                for (int i=0;i<aliases.Length;i++) {
                    var tr=bank.tracks[i];
                    if (tr.alias!=aliases[i] || tr.fps!=30 || tr.components!=5 || tr.duration<=0)
                        throw new InvalidOperationException("Original registered cannon track " + aliases[i]);
                    count+=tr.count;
                }
                if (count!=frames) throw new InvalidOperationException("Cannon source sample count " + npc);
            });
        }
    }
}
