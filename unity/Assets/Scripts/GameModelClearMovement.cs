using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        DungeonClearResult walkingClear;

        void StepClearMovement(float dt, Input input) {
            if (player.hp <= 0) return;
            if (!ReferenceEquals(walkingClear, ClearResult)) {
                walkingClear = ClearResult;
                CancelAction(true);
                AwakeningCasting = false;
                hitstop = shake = player.stun = 0;
                player.knockdownOnLanding = false;
                player.state = Grounded(player) ? CombatState.Normal : CombatState.Airborne;
            }
            TickAwakening(dt);
            for (int i = effects.Count - 1; i >= 0; i--) {
                var effect = effects[i];
                effect.age += dt;
                if (effect.age >= effect.lifetime) effects.RemoveAt(i);
            }
            if (coreHardV21 == null) {
                foreach (var enemy in enemies) {
                    if (!enemy.active || enemy.hp > 0) continue;
                    enemy.clock += dt;
                    enemy.deathTime += dt;
                }
            }

            float move = Finite(input.move) ? Clamp(input.move, -1, 1) : 0;
            bool grounded = Grounded(player);
            float previousClock = player.clock;
            player.clock += dt;
            landing = Math.Max(0, landing - dt);
            if (grounded && input.jump) {
                airborneDash = input.dash && move != 0;
                if (airborneDash) takeoffGravity = DashGravityAtTakeoff(Math.Sign(move));
                player.vy = airborneDash ? DashJumpSpeed : 420;
                player.state = CombatState.Airborne;
                fastFall = FastFallPhase.None;
                landing = 0;
            } else if (input.drop && !grounded) {
                fastFall = FastFallPhase.Descending;
                player.vy = -FastFallSpeedLimit * (NativeTerrain ? HamelGravityScale : 1);
            } else if (input.drop && grounded && NativeTerrain) {
                var floor = SurfaceBelow(player.x, player.y + .1f);
                if (floor != null && floor.platform) {
                    player.dropThrough = .25f;
                    player.groundLine = -1;
                    player.y -= .1f;
                    player.vy = -20;
                    player.state = CombatState.Airborne;
                }
            }
            if (move != 0) player.facing = Math.Sign(move);
            player.vx = LocomotionSpeed(move, input.dash, dt);
            string clip = !Grounded(player) ? (player.vy > 0 ? (airborneDash ? "dash-jump" : "jump-rise") : "jump-fall")
                : move != 0 ? (input.dash ? "run" : "walk") : landing > 0 ? "land" : "idle";
            if (player.clip != clip) { player.clip = clip; previousClock = player.clock = 0; }
            float oldX = player.x;
            IntegrateBody(player, dt, true);
            TickPeitaFallRecovery();
            if (player.clip == "walk") player.clock = previousClock + Math.Abs(player.x - oldX) / GroundWalkSpeed;
        }
    }
}
