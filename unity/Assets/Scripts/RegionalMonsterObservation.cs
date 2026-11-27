#if UNITY_EDITOR || ROOT_REDRAW_CHECK_V32
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class RegionalMonsterObservation {
            public string kind, source;
            public List<string> rooms = new List<string>();
            public List<string> states = new List<string>();
            public List<string> missingClips = new List<string>();
        }
        [Serializable] sealed class RegionalCombatObservations {
            public List<RegionalMonsterObservation> actors = new List<RegionalMonsterObservation>();
            public List<string> spells = new List<string>();
        }

        readonly Dictionary<string,RegionalMonsterObservation> observedRegionalKinds =
            new Dictionary<string,RegionalMonsterObservation>();
        readonly HashSet<string> observedRegionalSpells = new HashSet<string>();

        void ObserveRegionalCombat() {
            foreach (var e in game.enemies) {
                if (!e.active || e.dormant || e.hp <= 0 || string.IsNullOrEmpty(e.kind)) continue;
                if (!observedRegionalKinds.TryGetValue(e.kind,out var observation)) {
                    observation = new RegionalMonsterObservation{kind=e.kind,source=e.sourceNPC};
                    observedRegionalKinds.Add(e.kind,observation);
                }
                string location=game.CurrentRoom.id;
                if (!observation.rooms.Contains(location)) observation.rooms.Add(location);
                var actor=e.regionActor;
                if (actor==null || actor.node==null) continue;
                string state=actor.node.id;
                if (!observation.states.Contains(state)) observation.states.Add(state);
                string clip=actor.node.clip;
                if (!string.IsNullOrEmpty(clip) && (!libraries.TryGetValue(e.kind,out var library) ||
                    !library.clips.ContainsKey(clip)) && !observation.missingClips.Contains(clip))
                    observation.missingClips.Add(clip);
            }
            foreach (var spell in game.RegionSpellInstances) if (spell.launched) observedRegionalSpells.Add(spell.id);
        }

        readonly HashSet<string> capturedRegionalBattles=new HashSet<string>();

        void CaptureRegionalBattle(string output) {
            string phase=null;
            foreach(var enemy in game.enemies) {
                var a=enemy.regionActor;
                if (!enemy.active||enemy.dormant||enemy.hp<=0||a?.node==null)continue;
                if (a.spec.boss && (Math.Abs(enemy.x-game.player.x)<220) &&
                    ((a.node.hits?.Length??0)>0 || (a.node.cues?.Length??0)>0)) {
                    phase="boss";break;
                }
            }
            if(phase==null)
                foreach(var spell in game.RegionSpellInstances)
                    if(spell.launched && Math.Abs(spell.x-game.player.x)<260) {phase="projectile";break;}
            string key=Dungeon.id+"-"+phase;
            if(phase==null || !capturedRegionalBattles.Add(key))return;
            string path=Path.Combine(output,"battles");
            Directory.CreateDirectory(path);
            RestoreNativeProjection();
            accumulator=1f/60;
            DrawCombatFrame();
            CaptureOffscreenFrame(Path.Combine(path,key+".png"));
            accumulator=0;
        }

        bool SaveRegionalObservations(string path) {
            var report=new RegionalCombatObservations();
            report.actors.AddRange(observedRegionalKinds.Values);
            report.actors.Sort((a,b)=>String.CompareOrdinal(a.kind,b.kind));
            report.spells.AddRange(observedRegionalSpells);
            report.spells.Sort(StringComparer.Ordinal);
            bool valid=report.actors.Count>0;
            foreach(var actor in report.actors)valid &= actor.missingClips.Count==0;
            File.WriteAllText(Path.Combine(path,"observed-mechanics.json"),JsonUtility.ToJson(report,true));
            return valid;
        }
    }
}
#endif
