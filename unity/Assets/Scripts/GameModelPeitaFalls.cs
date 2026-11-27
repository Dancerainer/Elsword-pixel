using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        EncounterDefinition peitaLastSafeEncounter;
        GroundSurface peitaLastSafeSurface;
        float peitaLastSafeX;
        RoomDefinition peitaFallRoom;
        float peitaVoidY;

        static bool IsRegionalDungeonRoom(string id) {
            return id != null && (id.StartsWith("peita-", StringComparison.Ordinal) ||
                id.StartsWith("velder-", StringComparison.Ordinal) || id.StartsWith("hamel-", StringComparison.Ordinal));
        }

        void TickPeitaFallRecovery() {
            var level = CurrentRoom;
            if (level.bungeeLineIds == null || !IsRegionalDungeonRoom(level.id)
                || !NativeTerrain || !HasEncounters || player.hp <= 0) return;
            var encounter = CurrentEncounter;
            if (peitaFallRoom != level) {
                peitaFallRoom = level;
                peitaVoidY = float.PositiveInfinity;
                foreach (var s in level.surfaces)
                    peitaVoidY = Math.Min(peitaVoidY, Math.Min(s.y1, s.y2) - 60);
                peitaLastSafeEncounter = null;
            }
            if (peitaLastSafeEncounter != encounter) {
                peitaLastSafeEncounter = encounter;
                peitaLastSafeSurface = null;
            }
            var support = Grounded(player) ? SurfaceBelow(player.x, player.y + .06f) : null;
            bool onBungee = support != null && Array.IndexOf(level.bungeeLineIds, support.id) >= 0;
            if (support != null && !onBungee) {
                peitaLastSafeSurface = support;
                peitaLastSafeX = player.x;
            }
            if (onBungee || player.y < peitaVoidY) {
                var floor = peitaLastSafeSurface;
                if (floor != null && floor.hamelDisabled) floor = null;
                if (onBungee && support.bungeeTargetPlusOne > 0) {
                    var target = Surface(support.bungeeTargetPlusOne - 1);
                    if (target != null && !target.hamelDisabled && target.x1 < encounter.rightX && target.x2 > encounter.leftX) {
                        floor = target;
                        peitaLastSafeX = (floor.x1 + floor.x2) * .5f;
                    }
                }
                if (floor == null) {
                    float best = float.PositiveInfinity;
                    foreach (var s in level.surfaces) {
                        float lo = Math.Max(encounter.leftX, s.x1), hi = Math.Min(encounter.rightX, s.x2);
                        if (hi <= lo || s.hamelDisabled || s.npcOnly || Array.IndexOf(level.bungeeLineIds, s.id) >= 0) continue;
                        float offset = Math.Abs(Clamp(player.x, lo, hi) - player.x);
                        if (offset < best) { best = offset; floor = s; peitaLastSafeX = player.x; }
                    }
                }
                if (floor != null) {
                    float lo = Math.Max(encounter.leftX, floor.x1), hi = Math.Min(encounter.rightX, floor.x2);
                    float margin = Math.Min(12f, Math.Max(0, (hi - lo) * .4f));
                    CancelAction(true);
                    PlacePeitaFighter(player, floor, Clamp(peitaLastSafeX, lo + margin, hi - margin));
                    player.invincible = Math.Max(player.invincible, .55f);
                }
            }
        }

        RoomDefinition objectiveBoundsRoom;
        EncounterDefinition objectiveBoundsEncounter;
        GroundSurface[] objectiveBoundsSurfaces;
        float objectiveBottom;

        bool IsEncounterObjective(Enemy enemy) {
            if (enemy.countsForClear) return true;
            if (!HasEncounters) return false;
            foreach (var clause in CurrentEncounter.clauses) {
                if (clause.mode == EncounterClearMode.KillNpcTypes && Array.IndexOf(clause.npcTypes, enemy.sourceNPC) >= 0) return true;
                if (clause.mode == EncounterClearMode.KillNpcKeyCodes && Array.IndexOf(clause.npcKeyCodes, enemy.sourceKeyCode) >= 0) return true;
            }
            return false;
        }

        bool IsFallSurface(GroundSurface surface) {
            var ids = CurrentRoom.bungeeLineIds;
            return ids != null && Array.IndexOf(ids, surface.id) >= 0;
        }

        void RecoverEscapedObjective(Enemy enemy) {
            if (!enemy.active || enemy.dormant || enemy.isGate || enemy.regionalAlly || enemy.hp <= 0 ||
                HasEncounters && enemy.encounterIndex != currentEncounter) return;
            var level = CurrentRoom;
            var encounter = CurrentEncounter;
            float left = encounter == null ? 0 : encounter.leftX;
            float right = encounter == null ? level.width : encounter.rightX;
            if (level != objectiveBoundsRoom || encounter != objectiveBoundsEncounter || level.surfaces != objectiveBoundsSurfaces) {
                objectiveBoundsRoom = level;
                objectiveBoundsEncounter = encounter;
                objectiveBoundsSurfaces = level.surfaces;
                objectiveBottom = float.PositiveInfinity;
                if (NativeTerrain) {
                    foreach (var s in level.surfaces) {
                        float lo = Math.Max(left, s.x1), hi = Math.Min(right, s.x2);
                        if (lo <= hi) objectiveBottom = Math.Min(objectiveBottom, Math.Min(s.Y(lo), s.Y(hi)));
                    }
                } else {
                    objectiveBottom = Math.Min(GroundAt(level,left), GroundAt(level,right));
                    foreach (var n in level.groundNodes)
                        if (n.x >= left && n.x <= right) objectiveBottom = Math.Min(objectiveBottom,n.y);
                }
            }
            bool outside = enemy.x < left - 1 || enemy.x > right + 1;
            bool below = objectiveBottom != float.PositiveInfinity && enemy.y < objectiveBottom - 60;
            bool bungee = enemy.groundLine >= 0 && level.bungeeLineIds != null &&
                Array.IndexOf(level.bungeeLineIds,enemy.groundLine) >= 0 && NativeGrounded(enemy);
            if ((!outside && !below && !bungee) || !IsEncounterObjective(enemy)) return;
            // Richie Mine owns its ordinary NPC pit defeat through its damage path.
            if (below && !outside && mine2MechanismsV21 != null && level.id.StartsWith("Dungeon_Besma_MINE2_Normal-",StringComparison.Ordinal)) return;

            var spawn = enemy.recoverySpawn;
            float nearPlayer = Clamp(player.x + 42 * player.facing, left + .03f, right - .03f);
            if (!NativeTerrain) {
                float x = spawn != null && spawn.x >= left && spawn.x <= right ? spawn.x : nearPlayer;
                ResetFallenFighter(enemy,x,GroundAt(level,x),-1);
                return;
            }
            var origin = spawn == null ? null : Surface(spawn.groundLine);
            if (origin != null && !origin.hamelDisabled && !origin.npcOnly && !IsFallSurface(origin) &&
                spawn.x >= left && spawn.x <= right && spawn.x >= origin.x1 && spawn.x <= origin.x2 &&
                spawn.y + .1f >= origin.Y(spawn.x)) {
                PlacePeitaFighter(enemy,origin,spawn.x);
                return;
            }
            GroundSurface best = null;
            float bestDistance = float.PositiveInfinity, bestX = 0;
            foreach (var s in level.surfaces) {
                if (s.hamelDisabled || s.npcOnly || IsFallSurface(s)) continue;
                float lo = Math.Max(left+.03f,s.x1+.03f), hi = Math.Min(right-.03f,s.x2-.03f);
                if (lo > hi) continue;
                float x = Clamp(nearPlayer,lo,hi);
                float distance = Math.Abs(nearPlayer-x) + Math.Abs(player.y-s.Y(x));
                // On a steep slope the point directly in front can be a full
                // storey below the hero. Also compare the point at foot height.
                if (s.y2 != s.y1) {
                    float sameHeight = Clamp(s.x1 + (s.x2-s.x1) * ((player.y-s.y1)/(s.y2-s.y1)),lo,hi);
                    float heightDistance = Math.Abs(nearPlayer-sameHeight) + Math.Abs(player.y-s.Y(sameHeight));
                    if (heightDistance < distance) { x = sameHeight; distance = heightDistance; }
                }
                if (distance >= bestDistance) continue;
                best = s; bestDistance = distance; bestX = x;
            }
            if (best != null) PlacePeitaFighter(enemy,best,bestX);
        }

        static void PlacePeitaFighter(Fighter fighter, GroundSurface floor, float x) {
            ResetFallenFighter(fighter,x,floor.Y(x),floor.id);
        }

        static void ResetFallenFighter(Fighter fighter, float x, float y, int line) {
            fighter.x = x;
            fighter.y = y;
            fighter.groundLine = line;
            fighter.vx = fighter.vy = fighter.dropThrough = 0;
            fighter.stun = fighter.downTime = fighter.recoveryTime = 0;
            fighter.knockdownOnLanding = false;
            fighter.state = CombatState.Normal;
            fighter.clip = "idle";
            fighter.clock = 0;
        }
    }
}
