using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        const string Nephilim = "dark_nephilim";
        const string Switch = "velder_stage6_boss_trap_switch";
        RoomDefinition velderAlarmRoom;
        float velderPalaceTime;
        Enemy velderPursuer;

        bool VelderChaseRoom { get { return CurrentRoom.id == "velder-gate-3" && EncounterIndex == 1; } }
        internal bool TryNephilimGuideBounds(Enemy enemy, out AttackBox bounds) {
            bounds = null;
            if (enemy.kind != Nephilim || !VelderChaseRoom) return false;
            bounds = EnemyBounds(enemy);
            return bounds.left <= bounds.right && bounds.bottom <= bounds.top;
        }
        public bool VelderPalaceAlarm { get { return velderAlarmRoom == CurrentRoom &&
            CurrentRoom.id == "velder-palace-entrance-4" && velderPalaceTime >= 11 && velderPalaceTime < 21 && !victory; } }

        bool NearVelderChaseEnd(Enemy boss, float distance) {
            // Native x=-28123 at the corridor's left end. IsNearLineEndPos
            // first checks x<=-20000, then measures the facing endpoint.
            var end = Surface(50);
            if (end == null || boss.facing > 0) return false;
            float fromEnd = Math.Abs(boss.x - end.x1);
            return fromEnd <= 8123 * .28f && fromEnd < distance * .28f;
        }

        void TickVelderStage(float dt) {
            var level = CurrentRoom;
            if (velderAlarmRoom != level) {
                velderAlarmRoom = level;
                velderPalaceTime = 0;
                velderPursuer = null;
            }
            if (level.id == "velder-palace-entrance-4" && EncounterIndex == 0) velderPalaceTime += dt;
            if (!VelderChaseRoom) return;
            if (velderPursuer == null) velderPursuer = enemies.Find(e => e.active && e.kind == Nephilim);
            var boss = velderPursuer;
            if (boss == null || boss.hp <= 0 || boss.npcState == "DARK_NEPHILIM_START_READY" ||
                String.IsNullOrEmpty(boss.npcState) || NearVelderChaseEnd(boss, 2000)) return;
            // The common frame callback returns a lagging player 200 native
            // units ahead. Its key-11 wall follows 3000 units ahead of the boss.
            if (player.x > boss.x - 300 * .28f) player.x = Math.Max(32, boss.x - 200 * .28f);
            var wall = enemies.Find(e => e.active && e.hp > 0 && e.sourceKeyCode == 11 &&
                e.kind == "velder_stage6_boss_wall");
            if (wall != null && player.x < wall.x) player.x = wall.x;
        }

        public bool VelderChaseCamera(out float x, out float y) {
            var boss = velderPursuer;
            x = y = 0;
            if (boss == null || !VelderChaseRoom || boss.hp <= 0 || victory ||
                boss.npcState == "DARK_NEPHILIM_START_READY" || String.IsNullOrEmpty(boss.npcState) ||
                NearVelderChaseEnd(boss, 2000)) return false;
            float ahead = boss.npcState == "DARK_NEPHILIM_CRASH_GATE" && boss.clock < 1 ? 400 : 1200;
            x = boss.x - ahead * .28f;
            y = boss.y + 300 * .28f;
            return true;
        }

        static int GuardVelderSwitch(Enemy e, int damage, ref HitReaction reaction) {
            if (e.kind != Switch) return damage;
            reaction = HitReaction.None;
            // Let the switch poll its HP before a single large skill can kill
            // the fixture outright. The Lua callback restores HP and sets flag 0.
            return e.regionActor != null && e.regionActor.flags.Contains(0) ? 0 :
                Math.Min(damage, Math.Max(0, (int)e.hp - 1));
        }

        bool TryVelderStageNative(string name, Fighter subject, object p, object q, object third, out object value) {
            value = null;
            switch (name) {
                case "GetNearNPCUnitUID_LUA":
                    if (!(p is RegionPoint)) return false;
                    var point = (RegionPoint)p;
                    float radius = RegionNumber(third) * .28f;
                    Enemy closest = null;
                    foreach (var other in enemies) {
                        if (!other.active || other.hp <= 0 || other.sourceNPC != RegionSymbol(q)) continue;
                        float dx = other.x - point.x * .28f, dy = other.y - point.y * .28f;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                        if (distance <= radius) { radius = distance; closest = other; }
                    }
                    value = closest == null ? (object)0f : closest;
                    return true;
                case "GetNPCUnitByUID": value = p as Enemy; return true;
                case "GetStateID_LUA": value = RegionSymbol(p); return true;
                case "GetXDistanceFrom":
                    float nativeX = RegionNumber(p);
                    if (Single.IsNaN(nativeX)) return false;
                    value = Math.Abs(subject.x / .28f - nativeX);
                    return true;
                default: return false;
            }
        }
    }
}
