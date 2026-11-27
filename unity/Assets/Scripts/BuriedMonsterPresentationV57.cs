using System;

namespace ElswordPixel {
    // Display-space offsets only. Actor roots, source damage and AI clocks stay
    // on their authored terrain. Both sprites are clipped against that terrain.
    public static class BuriedMonsterPresentationV57 {
        public const float GuardianDepth = 80f;
        public const float GuardianRiseSeconds = 1f;
        public const float SaplingDepth = 18f;
        static float Smooth(float t) { t = Math.Max(0, Math.Min(1, t)); return t * t * (3 - 2 * t); }
        public static float GuardianOffset(string motion, float seconds) {
            if (motion == "TrapModeWait") return -GuardianDepth;
            return motion == "TrapModeStart" ? -GuardianDepth * (1 - Smooth(seconds / GuardianRiseSeconds)) : 0;
        }
        public static float SaplingOffset(float reveal) { return -SaplingDepth * (1 - Smooth(reveal)); }
        public static float SaplingClock(float clock) { return Math.Max(GameModel.TreeKnightRevealStartV25, clock); }
    }
}
