using System.Collections.Generic;
namespace ElswordPixel {
    public sealed partial class GameModel {
        readonly HashSet<Enemy> fairyRearReactionsV34 = new HashSet<Enemy>();
        public const float FairySmallDamageSecondsV34 = 14f / 15f;
        public const float FairyDownSecondsV34 = 19f / 15f;
        public const float FairyFrontStandSecondsV34 = 1.7f;
        public const float FairyBackStandSecondsV34 = 11f / 6f;

        bool FairyReactionScopeV34(Fighter f) {
            var e = f as Enemy;
            return e != null && GuardianCombatNativeRulesV28.Identity(e.kind, e.sourceNPC) && GuardianSourceProfileV28(e);
        }
        public bool FairyRearReactionV34(Enemy e) {
            return e != null && fairyRearReactionsV34.Contains(e);
        }
        void RememberFairyIncomingV34(Enemy e, float fromX, HitReaction reaction) {
            if (!FairyReactionScopeV34(e)) return;
            if ((reaction == HitReaction.Launch || reaction == HitReaction.Knockdown) && (fromX - e.x) * e.facing < 0)
                fairyRearReactionsV34.Add(e);
            else fairyRearReactionsV34.Remove(e);
        }
        public float FairyGetUpSecondsV34(Fighter f) {
            if (!FairyReactionScopeV34(f)) return EnemyGetUpDuration;
            return FairyRearReactionV34((Enemy)f) ? FairyBackStandSecondsV34 : FairyFrontStandSecondsV34;
        }
        bool FairySourceGroundArmorV34(Enemy e) {
            return e != null && (e.state == CombatState.Knockdown || e.state == CombatState.GettingUp) && FairyReactionScopeV34(e);
        }
    }
}

