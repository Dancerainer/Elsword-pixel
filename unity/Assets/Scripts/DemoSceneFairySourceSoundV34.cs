using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    [Serializable]
    public sealed class FairySoundDeltaV34 {
        public string schema, sourceLuaSha256;
        public NpcSoundAsset[] assetsToAppend;
        public NpcStateSoundTrack[] stateTracksToAppend;
        public NpcSoundResourceData[] resourceBindingsToAppend;
    }

    public static class FairySourceSoundV34 {
        public const string Resource = "Pixel/feedback-v33/fairy-guardian-audio/source-sounds";
        public const string DocumentSha = "306dc72bf979c2d3b2686197d1972716190c8b4acb9d84ec2e793d21500c3c5c";
        static readonly string[] OriginalHashes = {
            "56d4a82b5dc263f147b4fa4377716b83f5e44277774c348d396e014e6e3cfc2f",
            "f6a9e45a9a491d1bb919ab7410af3e0ee98f27eb43a8633cfb35171549073851",
            "a901108fbc50286c701b5c274676df907f99bb58ff74112784a1a82a1c6fc0f0",
            "7e86b9cc866d9c34d944ae10ff679775dd841fab0469e61c7567d88a8092b217"
        };
        static void Require(bool condition, string reason) {
            if (!condition) throw new InvalidOperationException("Fairy source sounds: " + reason);
        }

        public static void Apply(ArtCatalog target, FairySoundDeltaV34 delta) {
            Require(delta != null && delta.schema == "fairy-three-state-four-source-sounds-v34" &&
                delta.sourceLuaSha256 == "4e8087b4bb002ea4505764b904c69bcb28508f34ed742259392f39ea1d23411f",
                "source identity");
            var sounds = target.npcSoundRules;
            Require(sounds != null && sounds.assets.Length == 21 && sounds.stateTracks.Length == 16 &&
                target.npcSounds.Length == 73 && delta.assetsToAppend.Length == 4 &&
                delta.stateTracksToAppend.Length == 3 && delta.resourceBindingsToAppend.Length == 4,
                "exact forest base and Fairy delta sizes");
            var assets = new List<NpcSoundAsset>(sounds.assets);
            var resources = new List<NpcSoundResourceData>(target.npcSounds);
            var tracks = new List<NpcStateSoundTrack>(sounds.stateTracks);
            var actor = Array.Find(target.npcRules, p => p.id == GuardianCombatNativeRulesV28.Npc);
            Require(GuardianCombatNativeRulesV28.SourceProfile(actor), "six existing source states");
            for (int i = 0; i < 4; ++i) {
                var asset = delta.assetsToAppend[i];
                var resource = delta.resourceBindingsToAppend[i];
                Require(asset.sha256 == OriginalHashes[i] && asset.id == "npc-fairy-v33-" + asset.sha256.Substring(0, 16) &&
                    resource.id == asset.id && resource.resource == "Pixel/feedback-v33/fairy-guardian-audio/" + asset.sha256 &&
                    !assets.Exists(a => a.id == asset.id) && !resources.Exists(a => a.id == asset.id), "source OGG " + i);
                assets.Add(asset);
                resources.Add(resource);
            }
            string[] states = { GuardianCombatNativeRulesV28.TrapStart, GuardianCombatNativeRulesV28.Ready,
                GuardianCombatNativeRulesV28.Fire };
            float[][] times = { new[] { .1f, 2.18f }, new[] { .1f }, new[] { .226f } };
            int assetIndex = 0;
            for (int i = 0; i < states.Length; ++i) {
                var track = delta.stateTracksToAppend[i];
                var state = Array.Find(actor.states, s => s.id == states[i]);
                Require(track.npc == actor.id && track.state == state.id && track.sourceAnimation == state.sourceAnimation &&
                    track.sourceAnimSpeed == 1 && !track.loop && Math.Abs(track.duration - state.duration) < .000001f &&
                    track.events.Length == times[i].Length && !tracks.Exists(t => t.npc == actor.id && t.state == state.id),
                    "source state track " + states[i]);
                for (int j = 0; j < track.events.Length; ++j) {
                    var e = track.events[j];
                    Require(e.sourceField == "SOUND_PLAY" + j && e.id == actor.id + "/" + state.id + "/" + e.sourceField &&
                        Math.Abs(e.sourceTime - times[i][j]) < .000001f && e.candidateTime == e.sourceTime &&
                        e.candidateClock == "npc-state-pose-seconds" && e.sound == delta.assetsToAppend[assetIndex++].id,
                        "source event " + e.id);
                }
                tracks.Add(track);
            }
            // Commit only after the complete delta has validated.
            sounds.assets = assets.ToArray();
            sounds.stateTracks = tracks.ToArray();
            target.npcSounds = resources.ToArray();
        }
    }

    public sealed partial class DemoScene {
        bool fairySourceSoundAppliedV34;
        void ApplyFairySourceSoundV34(ArtCatalog target) {
            var data = Resources.Load<TextAsset>(FairySourceSoundV34.Resource);
            if (!data || CompletedRedrawHashV31(data.bytes) != FairySourceSoundV34.DocumentSha)
                throw new InvalidOperationException("Fairy original sound delta hash");
            FairySourceSoundV34.Apply(target, JsonUtility.FromJson<FairySoundDeltaV34>(data.text));
            fairySourceSoundAppliedV34 = true;
        }
        bool ExpectedFairySourceSoundUnionV34(bool legacyForestRules) {
            return legacyForestRules ? !fairySourceSoundAppliedV34 && catalog.npcSoundRules.assets.Length == 21 :
                fairySourceSoundAppliedV34 && startedSourceUnionApprovedV29 && catalog.npcSoundRules.assets.Length == 25;
        }
    }
}

