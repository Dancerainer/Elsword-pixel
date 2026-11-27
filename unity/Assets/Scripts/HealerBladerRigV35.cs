using System;
namespace ElswordPixel {
    public static class HealerBladerRigV35 {
        public const string Id = "NUI_NASOD_HEALER_BLADER";
        public const string Kind = "nasod-healer-blader";
        public const string ResourceRoot = "Pixel/feedback-v35/healer-blader/";
        public const string PagesSha = "9c946dc231760e9ede423b320d9729b34af34e4467a35b08b1527dc1444a415e";
        public const string MotionSha = "a572a273ac73bc69b2a3a86c4e393a00e8e2490a87c10f147ea0f038ba782bd3";
        public const float OriginalHoverGap = 39.0254903f;
        public const float SourceHeightRatio = .66571065f;

        public static FairyPoseLibraryV33 Load() {
            return RegisteredPartRigV35.Load(ResourceRoot, Id, PagesSha, Validate);
        }
        static void Need(bool ok, string reason) {
            if (!ok) throw new InvalidOperationException("Healer Blader registration: " + reason);
        }
        static void Validate(FairyPoseLibraryV33 bank) {
            Need(bank.motionSha256 == MotionSha && bank.authoredFacing == 1 &&
                Math.Abs(bank.heightRatio - SourceHeightRatio) < .000001f &&
                bank.heroHeight == 67 && bank.standingPixels == 665, "source scale and direction");
            Need(bank.parts.Length == 9 && bank.tracks.Length == 14 &&
                bank.parts[7].instanceOf == 6 && bank.parts[8].instanceOf == 7, "seven original drawings, four radial blade instances");
            string[] required = {"Wait", "Walk", "DashAttackReady", "DashAttack", "DashAttackEnd",
                "DamageStandUp", "Healing", "Damage", "DamageDown", "DamageAirUp", "DamageAirFall",
                "DamageAirDown", "DamageAirDownLanding", "JumpLanding"};
            int samples = 0;
            for (int i = 0; i < required.Length; ++i) {
                var track = bank.tracks[i];
                Need(track.alias == required[i] && track.animation == required[i] && track.components == 5 &&
                    track.fps == 60 && !track.loop && track.duration > 0 && track.count >= 2, "source motion " + required[i]);
                samples += track.count;
            }
            Need(samples == 904, "complete registered samples");
        }
    }
}

