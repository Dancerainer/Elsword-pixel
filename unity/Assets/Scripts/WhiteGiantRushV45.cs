using System;

namespace ElswordPixel {
    internal static class WhiteGiantRushV45 {
        internal const string Npc = "NUI_PPORU_GIANT_WHITE";
        internal const string Ready = "PPORU_GIANT_WHITE_ATTACK_B_READY";
        internal const string Rush = "PPORU_GIANT_WHITE_ATTACK_B";
        internal const string End = "PPORU_GIANT_WHITE_ATTACK_B_END";

        internal static void RestoreChoices(NpcDefinition[] profiles) {
            var giant = Array.Find(profiles, p => p.id == Npc);
            if (giant == null) throw new InvalidOperationException("Missing original White Giant profile");
            foreach (var state in giant.states) {
                if (state.id != giant.waitState && state.id != giant.walkState) continue;
                if (state.choices.Length != 1 || state.choices[0].nextState != "PPORU_GIANT_WHITE_ATTACK_A_READY")
                    throw new InvalidOperationException("Unexpected base White Giant attack choices");
                // The original Wait and Walk tables try B before the always-eligible A.
                // The existing state request service applies B's separate 13s cooldown.
                state.choices = new[] {
                    new NpcCondition { nextState = Ready, near = 500f * .28f, hpLessPercent = 70f, rate = 35f },
                    state.choices[0]
                };
            }
        }

    }

    internal static class OriginalRushTimerV45 {
        internal const string WilliamRush="WILLIAM_PPORU_ATTACK_B";
        // The original source state timers run through the shorter animation loops.
        internal static bool Active(NpcStateDefinition state) {
            return state.loop && (state.id==WhiteGiantRushV45.Rush || state.id==WilliamRush);
        }
        internal static string EndState(NpcStateDefinition state,float elapsed) {
            if(state.id==WhiteGiantRushV45.Rush && elapsed>1f)return WhiteGiantRushV45.End;
            if(state.id==WilliamRush && elapsed>2f)return "WILLIAM_PPORU_ATTACK_B_END";
            return null;
        }

        // Intersect this simulation tick with the repeating source damage window.
        // Keep the next-hit time absolute so a loop boundary cannot reset its gap.
        internal static bool HitSpan(NpcStateDefinition state, NpcHitWindow hit,
                                     float from, float to, float nextHit, out float lo, out float hi) {
            float earliest = Math.Max(from, nextHit);
            float cycle = (float)Math.Floor(from / state.duration) * state.duration;
            if (earliest > cycle + hit.end) cycle += state.duration;
            lo = Math.Max(earliest, cycle + hit.start);
            hi = Math.Min(to, cycle + hit.end);
            return lo <= hi;
        }
    }
}
