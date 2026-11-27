using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class HamelTriggerEvent {
        public float at,shake;
        public int line;
        public bool enable;
        public string sound;
    }
    [Serializable] public sealed class HamelLineTrigger {
        public int id;
        public bool onLeave;
        public HamelTriggerEvent[] events;
    }
    public sealed partial class GameModel {
        // Two invisible directors populate the timed survival room in the Ancient Waterway.
        static bool IsHamelSurvivalSpawner(Enemy e) { return e.kind == "waterway_core_respawner"; }

        void PrepareHamelSurvivalSpawner(Enemy e) {
            if(CurrentEncounter == null) return;
            // Each native summon spends 1,000 HP with a five-second minimum wait.
            // A regular monster's starting HP would exhaust the director on its first summon.
            foreach(var clause in CurrentEncounter.clauses) {
                if(clause.mode != EncounterClearMode.SurviveTime) continue;
                e.hp = e.maxHp = Math.Max(e.maxHp, 1000f * (float)Math.Ceiling(clause.seconds / 5f));
            }
        }

        // The director passes GetRandVal() % 4 + 687 instead of a named NPC_UNIT_ID.
        static string HamelSurvivalNpc(int sourceId) {
            switch(sourceId) {
                case 687: return "NUI_SHADOW_WALKER";
                case 688: return "NUI_SHADOW_CHARGER";
                case 689: return "NUI_SHADOW_SNIPER";
                case 690: return "NUI_SHADOW_DEFENDER";
                default: return "";
            }
        }

        sealed class HamelTriggerInstance {
            public HamelLineTrigger source;
            public float age;
            public int cursor;
        }
        readonly List<HamelTriggerInstance> hamelTriggers = new List<HamelTriggerInstance>();
        GroundSurface lastHamelFoot;
        bool hamelHasTriggers;

        public float HamelTriggerAge(int id) {
            foreach (var timer in hamelTriggers) if (timer.source.id == id) return timer.age;
            return -1;
        }
        void ResetHamelTriggers() {
            hamelTriggers.Clear();
            lastHamelFoot = null;
            hamelHasTriggers = false;
            if (CurrentRoom.surfaces == null) return;
            foreach (var s in CurrentRoom.surfaces) {
                s.hamelDisabled = s.initiallyDisabled;
                if (s.hamelTrigger?.events != null && s.hamelTrigger.events.Length > 0) hamelHasTriggers = true;
            }
        }
        void StartHamelTrigger(HamelLineTrigger trigger) {
            if (trigger.events != null && trigger.events.Length > 0 && HamelTriggerAge(trigger.id) < 0) hamelTriggers.Add(new HamelTriggerInstance { source=trigger });
        }
        void TickHamelTriggers(float dt) {
            if (!hamelHasTriggers) return;
            var foot = NativeGrounded(player) ? SurfaceBelow(player.x, player.y + .06f) : null;
            if (foot != lastHamelFoot) {
                if (lastHamelFoot?.hamelTrigger?.onLeave == true) StartHamelTrigger(lastHamelFoot.hamelTrigger);
                if (foot?.hamelTrigger != null && !foot.hamelTrigger.onLeave) StartHamelTrigger(foot.hamelTrigger);
                lastHamelFoot = foot;
            }
            foreach (var timer in hamelTriggers) {
                timer.age += dt;
                var events = timer.source.events;
                while (timer.cursor < events.Length && events[timer.cursor].at <= timer.age) {
                    var action = events[timer.cursor++];
                    if (action.line >= 0) {
                        var floor = Surface(action.line);
                        if (floor != null && floor.hamelDisabled == action.enable) {
                            floor.hamelDisabled = !action.enable;
                            nativeNavigationDestination = -1;
                        }
                    } else if (!string.IsNullOrEmpty(action.sound)) {
                        var sound = new NpcTimedSound {id=action.sound,sound="Pixel/hamel/sounds/"+action.sound};
                        QueueNpcSound(sound,null,++npcSoundEntrySerial,0,elapsed,"terrain-trigger");
                    } else if (action.shake > 0) shake = Math.Max(shake,action.shake);
                }
            }
        }
    }
}
