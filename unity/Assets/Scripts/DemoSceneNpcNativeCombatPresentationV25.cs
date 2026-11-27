using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly Dictionary<Enemy,float> first3NativeRenderClocksV25 = new Dictionary<Enemy,float>();

        static bool First3NativeCombatActorV25(Enemy enemy) {
            return enemy.sourceNPC == "NUI_TREE_KNIGHT" || InitialBatV25(enemy);
        }

        static string First3CanonicalModelClipV25(string selected) {
            if (selected == "launched") return "launch";
            if (selected == "knockdown") return "down";
            if (selected == "getup") return "get-up";
            return selected;
        }

        void BeginFirst3NativeRenderFrameV25() { first3NativeRenderClocksV25.Clear();first3BoneObservationsV25.Clear(); }

        float First3ActualRenderClockV25(Enemy enemy) {
            float clock;
            if (!first3NativeRenderClocksV25.TryGetValue(enemy,out clock))
                throw new InvalidOperationException("First3 actor has no actual render-clock observation");
            return clock;
        }

        string NativeCombatRenderClipV25(Enemy enemy,string previousSelected,ref float clock) {
            if (!First3NativeCombatActorV25(enemy)) return previousSelected;
            string result=previousSelected;
            bool complete=enemy.hp<=0 || game.EnemyInitialPosePhaseV25(enemy)=="complete";
            if (complete) {
                bool tree=enemy.sourceNPC=="NUI_TREE_KNIGHT";
                string selected=First3CanonicalModelClipV25(enemy.clip),target=null;
                if (selected=="idle") target=tree?"source-idle":"source-flight";
                else if (selected=="walk" || selected=="hurt" || selected=="launch" || selected=="fall"
                    || selected=="down" || selected=="get-up" || selected=="death") target="source-"+selected;
                else if (selected=="jump") target=enemy.vy<0?"source-fall":"source-jump";
                else if (tree && selected=="attack") target="source-attack";
                else if (!tree && (selected=="attack-a-ready" || selected=="attack-a" || selected=="get-up-back"
                    || enemy.sourceNPC=="NUI_BAT_B" && selected=="attack-a-drain")) target="source-"+selected;
                if (target!=null) {
                    Library library;
                    if (!libraries.TryGetValue(enemy.kind,out library) || !library.clips.ContainsKey(target))
                        throw new InvalidOperationException("Missing first3 native combat display: "+enemy.kind+"/"+target);
                    var info=library.clips[target];
                    if (selected=="down") clock=(info.frames.Length-1)/info.fps;
                    else if (selected=="death") clock=Mathf.Clamp01(clock/1.2f)*info.duration;
                    else if (tree && game.RubenTreeMotionClockV45(enemy,selected,out float originalClock)) clock=originalClock;
                    else if (tree && selected!="idle") {
                        string old=library.clips.ContainsKey(selected)?selected:selected=="fall"?"launch":null;
                        if (old==null || library.clips[old].duration<=0)
                            throw new InvalidOperationException("Missing original generic Tree clock: "+selected);
                        // The existing generic model retains its own durations,
                        // hits and physics. Only display spans the complete raw
                        // source animation; native Tree state parity is not claimed.
                        clock=clock*info.duration/library.clips[old].duration;
                    } else if (selected=="attack-a-ready") clock*=1.25f;
                    // Fractional source endpoints are diagnostic extra frames
                    // after the regular 12fps samples. A finished one-shot may
                    // explicitly display that final source endpoint.
                    if (!info.loop && selected!="down" && clock+0.000001f>=info.duration)
                        clock=(info.frames.Length-1)/info.fps;
                    result=target;
                }
            }
            first3NativeRenderClocksV25[enemy]=clock;
            return result;
        }

        bool First3NativeNormalSpriteV25(Enemy enemy,SpriteRenderer view) {
            if (!First3NativeCombatActorV25(enemy) || !view.sprite) return false;
            string prefix=enemy.kind+"/source-",name=view.sprite.name;
            return name.StartsWith(prefix,StringComparison.Ordinal)
                && !name.StartsWith(enemy.kind+"/source-start/",StringComparison.Ordinal)
                && !name.StartsWith(enemy.kind+"/source-wait-zero2/",StringComparison.Ordinal);
        }
    }
}
