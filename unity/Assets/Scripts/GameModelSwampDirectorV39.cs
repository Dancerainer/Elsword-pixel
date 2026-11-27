using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        public const float SwampDirectorDurationV39 = 12.5f;
        public const float SwampStartDurationV39 = 100f / 60;
        Enemy swampIntroActorV39;
        float swampIntroBeganV39;
        bool swampActorReleasedV39;
        public Enemy SwampIntroActorV39 { get { return swampIntroActorV39; } }
        public float SwampIntroAgeV39 { get { return swampIntroActorV39 == null ? -1 : Math.Max(0, elapsed - swampIntroBeganV39); } }

        void BindSwampIntroV39() {
            swampIntroActorV39 = null;
            if (CurrentRoom.id != "swamp-3" || enemies.Count == 0) return;
            var actor = enemies[0];
            if (actor.sourceNPC != "NUI_ANCIENT_PPORU" || !Remaining3SourceProfileV28(actor)) return;
            swampIntroActorV39 = actor;
            swampIntroBeganV39 = elapsed;
            if (SimpleDangerIntroductions) {
                swampActorReleasedV39 = true;
                actor.aiEnabled = true;
                EnterNpcState(actor, npcStates[actor.sourceNPC + "/ANCIENT_PPORU_WAIT"]);
                return;
            }
            swampActorReleasedV39 = false;
            actor.aiEnabled = false;
            actor.npcState = "ANCIENT_PPORU_START_READY";
            actor.npcTime = actor.clock = 0;
            actor.vx = actor.vy = 0;
            actor.clip = "idle";
        }
        public bool SwampActorHiddenV39(Enemy actor) {
            return actor != null && actor == swampIntroActorV39 && !swampActorReleasedV39;
        }
        bool TickSwampIntroActorV39(Enemy actor) {
            if (SimpleDangerIntroductions) return false;
            if (actor != swampIntroActorV39 || actor == null) return false;
            float age = SwampIntroAgeV39;
            if (!swampActorReleasedV39) {
                actor.clock = actor.npcTime = age;
                if (age + .00001f < SwampDirectorDurationV39) return true;
                // Original separate director DYING.START callback releases this exact birth.
                swampActorReleasedV39 = true;
                actor.aiEnabled = true;
                actor.npcState = "ANCIENT_PPORU_START";
                actor.clock = actor.npcTime = 0;
                actor.clip = "idle";
            }
            if (actor.npcState != "ANCIENT_PPORU_START") return false;
            if (actor.stun > 0 || actor.state != CombatState.Normal || actor.hp <= 0) {
                actor.npcState = "";
                return false;
            }
            actor.clock = actor.npcTime = Math.Max(0, age - SwampDirectorDurationV39);
            if (actor.npcTime + .00001f < SwampStartDurationV39) return true;
            EnterNpcState(actor, npcStates[actor.sourceNPC + "/ANCIENT_PPORU_WAIT"]);
            return true;
        }
    }
}
