using System.Runtime.CompilerServices;

namespace ElswordPixel {
    // The combat model can clear npcState for hit-stop or on the last tick of recovery.
    // Keep an actual pose from this actor's own bank across that handoff. A new spawn
    // has no previous pose and continues to use its authored birth visibility.
    internal sealed class VisibleHurtPoseV44 {
        sealed class Sample {
            public string state;
            public float time;
        }

        readonly ConditionalWeakTable<Enemy, Sample> samples = new ConditionalWeakTable<Enemy, Sample>();

        internal void Remember(Enemy actor, string state, float time) {
            if (actor == null || string.IsNullOrEmpty(state) || !actor.active || actor.dormant) return;
            var sample = samples.GetValue(actor, _ => new Sample());
            sample.state = state;
            sample.time = time;
        }

        internal bool TryHold(Enemy actor, out string state, out float time) {
            state = null;
            time = 0;
            if (actor == null || !actor.active || actor.dormant || !samples.TryGetValue(actor, out var sample)) return false;
            state = sample.state;
            time = sample.time;
            return true;
        }
    }
}
