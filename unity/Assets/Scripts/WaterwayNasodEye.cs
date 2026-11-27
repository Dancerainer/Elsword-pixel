using System;

namespace ElswordPixel {
    public static class WaterwayNasodEye {
        // SpecialAttackA_Fire, sampled at the same 60 Hz as MonsterPixelFramesV53.
        // Each pair is the centre of the iris texel relative to that frame's signed
        // atlas pivot, in half pixels. The original faces right, at 0.8 pixels/unit.
        static readonly short[] fireEyeHalfPixels = {
            11, 97, 27, 103, 41, 107, 51, 113, 57, 119, 59, 125, 57, 129, 61, 135,
            61, 141, 59, 143, 57, 145, 55, 145, 55, 145, 55, 145, 53, 145, 53, 145,
            53, 145, 51, 145, 51, 145, 51, 145, 51, 147, 51, 147, 49, 147, 49, 147,
            49, 147, 49, 147, 49, 147, 49, 147, 49, 147, 49, 147, 49, 147, 49, 147,
            49, 147, 49, 147, 49, 147, 49, 147, 49, 147, 49, 147, 49, 147, 49, 147,
            49, 145, 49, 145, 49, 145, 49, 145, 49, 145, 49, 145, 49, 145, 51, 145,
            51, 145, 51, 145, 51, 143, 51, 143, 51, 143, 51, 143, 51, 143, 51, 143,
            53, 145, 53, 145, 53, 145, 53, 145, 53, 147, 53, 147, 53, 147, 53, 147,
            51, 149, 51, 149, 51, 149, 49, 149, 49, 151, 47, 151, 45, 151, 43, 151,
            43, 153, 41, 153, 39, 153, 37, 153, 35, 153, 31, 155, 29, 155, 27, 155,
            25, 155, 23, 155, 21, 155, 17, 155, 15, 155, 15, 155, 15, 155, 15, 155,
            15, 155, 15, 155, 15, 155, 15, 155, 15, 155, 15, 155, 15, 155, 15, 155,
            15, 155,
        };

        public static void FireOffset(float sourceSeconds, out float x, out float y) {
            int frame = sourceSeconds + .00001f >= 1.6f ? 96 :
                Math.Max(0, Math.Min(96, (int)Math.Floor(sourceSeconds * 60f + .0001f)));
            const float halfPixelInModelUnits = .5f / .8f;
            x = fireEyeHalfPixels[frame * 2] * halfPixelInModelUnits;
            y = fireEyeHalfPixels[frame * 2 + 1] * halfPixelInModelUnits;
        }
    }
}
