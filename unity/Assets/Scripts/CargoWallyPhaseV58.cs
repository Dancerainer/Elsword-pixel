using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GameModel {
        internal const string CargoWallyInnerV58 = "NUI_WALLY_9TH";
        internal const string CargoWallyArmorV58 = "NUI_WALLY_9TH_ARMOR";
        // The source death callback hands off the shell's current position at
        // motion end. The original Death animation contains 164 frames at 30 Hz.
        internal const float CargoWallyShellSecondsV58 = 164f / 30f;

        internal void EnableCargoInnerRobotV58() {
            var room = rooms.Find(r => r.id == "Dungeon_Besma_Air_Ship_Normal-2");
            if (room == null) throw new InvalidOperationException("Missing Cargo final room");
            var shell = room.enemies.Find(e => e.sourceNPC == CargoWallyArmorV58);
            if (shell == null) throw new InvalidOperationException("Missing Wally armor birth");
            var encounter = room.encounters[shell.encounterIndex];
            foreach (var clause in encounter.clauses) {
                if (Array.Exists(clause.transitions, edge => edge.completeDungeon)) {
                    clause.mode = EncounterClearMode.KillNpcTypes;
                    clause.npcTypes = new[] {CargoWallyInnerV58};
                }
            }
            encounter.clearMode = EncounterClearMode.KillNpcTypes;
            encounter.clearNpcTypes = new[] {CargoWallyInnerV58};
            // Both AddExtraNPCStat rows use HP_VALUE=36 and both births use
            // MONSTER_GRADE=1. The 3/2 gauge lines are presentation settings.
            room.enemies.Add(new EnemySpawn {
                kind = "wally-9th", sourceNPC = CargoWallyInnerV58, encounterIndex = shell.encounterIndex,
                x = shell.x, y = shell.y, facing = shell.facing, groundLine = shell.groundLine,
                nativeDepth = shell.nativeDepth, visualScale = shell.visualScale,
                hp = shell.hp, aggroRange = shell.aggroRange,
                countsForClear = true, aiEnabled = true, dormant = true,
                spawnAfterDeath = shell.kind, spawnDelay = CargoWallyShellSecondsV58
            });
        }

        void ReleaseCargoInnerRobotV58(Enemy inner, Enemy shell) {
            if (inner.sourceNPC != CargoWallyInnerV58 || shell.sourceNPC != CargoWallyArmorV58) return;
            inner.x = inner.spawnX = shell.x;
            inner.y = shell.y;
            inner.nativeDepth = shell.nativeDepth;
            inner.groundLine = shell.groundLine;
            inner.facing = shell.facing;
            inner.vx = inner.vy = 0;
            inner.alerted = true;
            EnterNpcState(inner, npcStates[CargoWallyInnerV58 + "/WALLY_9TH_START"]);
        }

        void TickCargoDeathSoundsV58(Enemy actor, float dt) {
            if ((actor.sourceNPC != CargoWallyArmorV58 && actor.sourceNPC != CargoWallyInnerV58) ||
                !npcDefinitions.ContainsKey(CargoWallyInnerV58)) return;
            string suffix = actor.originalHitAirV45 ? "_DYING_SKY" :
                actor.originalHitBackV45 ? "_DYING_LAND_BACK" : "_DYING_LAND_FRONT";
            var state = npcStates[actor.sourceNPC + "/" + actor.sourceNPC.Substring(4) + suffix];
            if (actor.deathTime >= state.duration) return;
            if (actor.deathTime == 0) BeginNpcStateSounds(actor, state);
            AdvanceNpcStateSounds(actor, state, actor.deathTime, Math.Min(state.duration, actor.deathTime + dt), elapsed - dt);
        }

        internal string CargoInnerMotionV58(Enemy actor, out float clock) {
            clock = actor.clock;
            switch (actor.state) {
                case CombatState.Dead:
                    clock = actor.deathTime;
                    return actor.originalHitAirV45 ? "DamageAirDownLanding" :
                        actor.originalHitBackV45 ? "DamageDownBack" : "DamageDownFront";
                case CombatState.Hurt: return actor.originalHitAirV45 ? "DamageAirSmall" : "DamageSmall";
                case CombatState.Airborne:
                    if (actor.knockdownOnLanding) return actor.vy > 0 ? "DamageAirUp" : "DamageAirFall";
                    break;
                case CombatState.Knockdown: return actor.originalHitBackV45 ? "DamageDownBack" : "DamageDownFront";
                case CombatState.GettingUp: return actor.originalHitBackV45 ? "DamageStandUpBack" : "DamageStandUpFront";
            }
            var state = NpcState(actor);
            if (state == null) return "Wait";
            clock = actor.npcTime * state.sourceAnimSpeed;
            return state.sourceAnimation;
        }
    }

    public sealed partial class DemoScene {
        const string CargoInnerRootV58 = "Pixel/monster-v53/wally-9th/";

        static T CargoInnerDataV58<T>(string name) {
            var file = Resources.Load<TextAsset>(CargoInnerRootV58 + name);
            if (!file) throw new InvalidOperationException("Missing Cargo Wally " + name);
            return JsonUtility.FromJson<T>(file.text);
        }

        void ConfigureCargoCombatV58(CargoProductDataPlanR1261 plan) {
            var profiles = new List<NpcDefinition>(plan.rules.definitions) { CargoInnerDataV58<NpcDefinition>("combat") };
            game.SetNpcDefinitions(profiles);
            var innerEffects = CargoInnerDataV58<NpcEffectDocument>("effects");
            foreach (var effect in innerEffects.definitions) {
                if (string.IsNullOrEmpty(effect.visualId)) continue;
                var art = CargoInnerDataV58<SpriteLibraryData>(effect.visualId);
                cargoProductLibrariesR1288.Add(art.id);
                foreach (var frame in art.clips[0].frames)
                    if (!cargoProductAtlasKeysR1288.Contains(frame.texture)) cargoProductAtlasKeysR1288.Add(frame.texture);
                MakeLibrary(art);
                cargoProductOwnedR1288.AddRange(libraries[art.id].frames["play"]);
            }
            var effects = new List<NpcEffectDefinition>(plan.effects.definitions);
            effects.AddRange(innerEffects.definitions);
            var bindings = new List<NpcEffectBinding>(plan.effects.bindings);
            bindings.AddRange(innerEffects.bindings);
            game.SetNpcEffectDefinitions(new NpcEffectDocument {
                definitions = effects.ToArray(), bindings = bindings.ToArray(),
                reactionBindings = plan.effects.reactionBindings
            });
            var innerSounds = CargoInnerDataV58<NpcSoundDocument>("sound");
            var clips = new Dictionary<string, AudioClip>();
            foreach (var sound in innerSounds.assets) {
                var clip = Resources.Load<AudioClip>(CargoInnerRootV58 + "audio/" + sound.id);
                if (!clip) throw new InvalidOperationException("Missing Wally sound " + sound.id);
                clips.Add(sound.id, clip);
                cargoProductSoundIdsR1288.Add(sound.id);
            }
            RegisterNpcSoundClips(clips);
            var assets = new List<NpcSoundAsset>(plan.sounds.assets);
            assets.AddRange(innerSounds.assets);
            var tracks = new List<NpcStateSoundTrack>(plan.sounds.stateTracks);
            tracks.AddRange(innerSounds.stateTracks);
            game.SetNpcSoundDefinitions(new NpcSoundDocument {
                assets = assets.ToArray(), stateTracks = tracks.ToArray(), effectTracks = plan.sounds.effectTracks
            });
            var cooldowns = new List<NpcMechanismStateRule>(plan.mechanisms.states);
            cooldowns.AddRange(CargoInnerDataV58<NpcMechanismDocument>("cooldowns").states);
            var originalStates = plan.mechanisms.states;
            try {
                plan.mechanisms.states = cooldowns.ToArray();
                game.SetNpcMechanismDefinitions(plan.mechanisms);
            } finally { plan.mechanisms.states = originalStates; }
        }

        bool CargoWallyPoseV58(Enemy actor, SpriteRenderer carrier) {
            if (!cargoProductSessionR1288 || actor.dormant) return false;
            if (actor.sourceNPC == GameModel.CargoWallyInnerV58) {
                var motion = game.CargoInnerMotionV58(actor, out var sourceClock);
                return RecordMonsterPixelSourceV53(actor, carrier, "Cargo/inner", actor.clip, motion, sourceClock);
            }
            return actor.sourceNPC == GameModel.CargoWallyArmorV58 && actor.hp <= 0 &&
                RecordMonsterPixelSourceV53(actor, carrier, "Cargo/shell-death", "death", "Death", actor.deathTime);
        }

        float CargoDeathAlphaV58(Enemy actor) {
            if (actor.hp > 0) return 1;
            if (cargoProductSessionR1288 && actor.sourceNPC == GameModel.CargoWallyArmorV58)
                return actor.deathTime < GameModel.CargoWallyShellSecondsV58 ? 1 : 0;
            return Mathf.Max(0, 1 - actor.deathTime / 1.2f);
        }
    }
}
