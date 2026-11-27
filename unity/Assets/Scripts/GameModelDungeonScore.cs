using System;
using System.Collections.Generic;
using System.Globalization;

namespace ElswordPixel {
    public sealed class DungeonClearResult {
        public readonly int TimeCentiseconds, MaxCombo, HitsTaken;
        public readonly int ParCentiseconds, ComboTarget;
        public readonly int TimeScore, ComboScore, DefenseScore, TotalScore;
        public readonly string Grade;

        internal DungeonClearResult(float seconds, int combo, int hitsTaken, int roomCount,
                                    int monsterCount, float survivalSeconds, double bossHealth, bool wallyCastle) {
            TimeCentiseconds = Centiseconds(seconds);
            MaxCombo = combo;
            HitsTaken = hitsTaken;

            // Allow travel and gate animation per room, crowd fighting per actor, and
            // sustained damage against each authored boss body. Timed encounters add their wait.
            ParCentiseconds = Centiseconds(12 * roomCount + 4 * monsterCount + bossHealth / 130 + survivalSeconds);
            ComboTarget = Math.Min(20, 2 * monsterCount);
            TimeScore = TimeCentiseconds <= ParCentiseconds ? 100 : Score(100.0 * ParCentiseconds / TimeCentiseconds);
            ComboScore = MaxCombo >= ComboTarget ? 100 : Score(100.0 * MaxCombo / ComboTarget);
            int penaltyPerHit = Math.Max(2, 7 - roomCount);
            DefenseScore = wallyCastle
                ? Score(100 - 1.25 * Math.Max(0, hitsTaken - 8))
                : Score(100 - Math.Min(2, hitsTaken) - (double)Math.Max(0, hitsTaken - 2) * penaltyPerHit);
            TotalScore = (4 * TimeScore + 3 * ComboScore + 3 * DefenseScore + 5) / 10;
            Grade = GradeForScore(TotalScore);
        }

        static int Centiseconds(double seconds) {
            return (int)Math.Min(int.MaxValue, Math.Round(Math.Max(0, seconds) * 100, MidpointRounding.AwayFromZero));
        }

        static int Score(double value) {
            return (int)Math.Round(Math.Max(0, Math.Min(100, value)), MidpointRounding.AwayFromZero);
        }

        public static string GradeForScore(int score) {
            if (score >= 98) return "SSS";
            if (score >= 94) return "SS";
            if (score >= 88) return "S";
            if (score >= 80) return "A";
            if (score >= 70) return "B";
            if (score >= 60) return "C";
            if (score >= 50) return "D";
            if (score >= 40) return "E";
            return "F";
        }

        public static string FormatTime(int centiseconds) {
            int t = Math.Max(0, centiseconds);
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}.{2:00}", t / 6000, t / 100 % 60, t % 100);
        }
    }

    public sealed partial class GameModel {
        readonly HashSet<RoomDefinition> scoredRooms = new HashSet<RoomDefinition>();
        readonly HashSet<EncounterDefinition> scoredEncounters = new HashSet<EncounterDefinition>();
        readonly List<Enemy> routeBossBodies = new List<Enemy>();
        int runMaxCombo, runHitsTaken, routeMonsterCount;
        float routeSurvivalSeconds;
        public DungeonClearResult ClearResult { get; private set; }

        void ResetDungeonScore() {
            runMaxCombo = runHitsTaken = routeMonsterCount = 0;
            routeSurvivalSeconds = 0;
            scoredRooms.Clear();
            scoredEncounters.Clear();
            routeBossBodies.Clear();
            ClearResult = null;
        }

        void CountScoreEncounter() {
            bool newRoom = scoredRooms.Add(CurrentRoom);
            var encounter = CurrentEncounter;
            if (encounter == null ? !newRoom : !scoredEncounters.Add(encounter)) return;

            bossHealthRules.TryGetValue(CurrentRoom.id ?? "", out var bossRule);
            foreach (var e in enemies) {
                if (encounter != null && e.encounterIndex != EncounterIndex) continue;
                if (e.countsForClear && !e.regionalAlly && IsCombatMonster(e)) routeMonsterCount++;
                // Initial encounter entry precedes boss HP scaling. Keep the authored body
                // to read its final max HP, even after leaving this room or changing phase.
                if (e.recoverySpawn != null && bossRule != null && Array.IndexOf(bossRule.actors, e.sourceNPC) >= 0)
                    routeBossBodies.Add(e);
            }
        }

        void ApplyCombatHealth(Fighter target, float nextHp) {
            if (target == player && nextHp < target.hp && ClearResult == null) runHitsTaken++;
            target.hp = nextHp;
        }

        void FinishDungeonScore() {
            if (ClearResult == null) {
                double bossHealth = 0;
                foreach (var e in routeBossBodies)
                    if (e.dungeonBoss) bossHealth += e.maxHp;
                bool wallyCastle = rooms[0].id == "castle-center-1" || rooms[0].id == "outskirts-1";
                ClearResult = new DungeonClearResult(elapsed, runMaxCombo, runHitsTaken, scoredRooms.Count,
                                                     routeMonsterCount, routeSurvivalSeconds, bossHealth, wallyCastle);
            }
            victory = true;
        }
    }
}
