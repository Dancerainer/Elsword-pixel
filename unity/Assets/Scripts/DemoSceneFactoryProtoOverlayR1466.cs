using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
    // Additive imagegen pixel bank. The frozen eleven banks and their
    // nine lookup/418 sprite counters remain owned by the existing host.
    public sealed partial class DemoScene {
        [Serializable] sealed class FactoryProtoFileR1466 {
            public string resource, sha256; public int bytes;
        }
        [Serializable] sealed class FactoryProtoKeyR1466 {
            public string key, originalPath, rgbaSha256;
            public int width, height; public FactoryProtoFileR1466 png;
        }
        [Serializable] sealed class FactoryProtoOverlayR1466 {
            public string schema, sourceNPC, kind, bankID, sourceRulesSha256, soundID;
            public bool artApproved, animationLibraryComplete, nativeEquivalent;
            public FactoryProtoFileR1466 document, evidence, modelRules, sound, artReview;
            public FactoryProtoKeyR1466[] keys;
        }
        const string FactoryProtoNpcR1466 = "NUI_RAVEN_CLONE_PROTO";
        const string FactoryProtoRulesR1466 = "a44ac614f4a636b1732cee34e2a810e40662f6acadd44e9b6b96cb3227afd588";
        const string FactoryProtoOverlayKeyR1466 = "Pixel/dungeons/altera-factory-normal/data/dd555c7f0b65b6e114cc0d7557e65b2386a7d2b83c43e8ef93fe94481de735b4";
        const string FactoryProtoOverlayShaR1466 = "dd555c7f0b65b6e114cc0d7557e65b2386a7d2b83c43e8ef93fe94481de735b4";
        const int FactoryProtoOverlayBytesR1466 = 71845;
        // Explicit demo balance; the source Proto contract does not give MAX_HP.
        const float FactoryProtoAuthoredHpR1466 = 1000f;
        sealed class FactoryProtoReactionR1466 {
            internal readonly string state; internal readonly float duration;
            internal FactoryProtoReactionR1466(NpcReactionBody source) {
                state = source.sourceState; duration = source.duration;
            }
        }
        readonly Dictionary<CombatState, FactoryProtoReactionR1466> factoryProtoReactionsR1466 =
            new Dictionary<CombatState, FactoryProtoReactionR1466>();
        FactoryProtoOverlayR1466 factoryProtoOverlayR1466;
        byte[] factoryProtoModelRulesR1466;
        SourcePhaseWholeLookupV21 factoryProtoLookupR1466;
        readonly Dictionary<string, Sprite> factoryProtoSpritesR1466 =
            new Dictionary<string, Sprite>(StringComparer.Ordinal);
        public int FactoryProtoExtraKeysR1466 { get { return factoryProtoSpritesR1466.Count; } }
        public string FactoryProtoLastStateR1466 { get; private set; }
        public string FactoryProtoLastKeyR1466 { get; private set; }

        static byte[] ReadFactoryProtoRawR1466(FactoryProtoFileR1466 file) {
            FactoryProductNeedR1179(file != null && file.bytes > 0 && file.bytes <= 64 * 1024 * 1024 &&
                BelowPlayerResourceIndexV21.IsHash(file.sha256) &&
                file.resource == "Pixel/dungeons/altera-factory-normal/data/" + file.sha256,
                "exact Proto raw-resource binding required");
            var asset = Resources.Load<TextAsset>(file.resource);
            var bytes = asset ? asset.bytes : null;
            FactoryProductNeedR1179(bytes != null && bytes.Length == file.bytes &&
                BelowPlayerResourceIndexV21.Hash(bytes) == file.sha256,
                "Proto raw resource missing or changed: " + file.resource);
            return bytes;
        }

        void PrepareFactoryProtoOverlayR1466() {
            FactoryProductNeedR1179(factoryProtoOverlayR1466 == null && factoryProtoLookupR1466 == null &&
                factoryProtoModelRulesR1466 == null && factoryProtoSpritesR1466.Count == 0 && factoryProtoReactionsR1466.Count == 0 &&
                factoryProductReferenceR1179.Count == 9 && factoryProductSpritesR1179.Count == 418 &&
                factoryProductOriginalReferenceBanksR1223 == 7 &&
                factoryProductMakerPixelBanksR1223 == 1 && factoryProductGuardPixelBanksR1264 == 1,
                "fresh additive Proto bank after the unchanged eleven banks required");
            var asset = Resources.Load<TextAsset>(FactoryProtoOverlayKeyR1466);
            var raw = asset ? asset.bytes : null;
            FactoryProductNeedR1179(raw != null && raw.Length == FactoryProtoOverlayBytesR1466 &&
                BelowPlayerResourceIndexV21.Hash(raw) == FactoryProtoOverlayShaR1466,
                "exact additive Proto resource manifest required");
            var overlay = JsonUtility.FromJson<FactoryProtoOverlayR1466>(new UTF8Encoding(false, true).GetString(raw));
            FactoryProductNeedR1179(overlay != null && overlay.schema == "factory-extra-Proto-runtime-pixel-overlay-r1480-v1" &&
                overlay.sourceNPC == FactoryProtoNpcR1466 && overlay.kind == "raven-clone-proto" &&
                overlay.bankID == "raven-clone-proto-pixel-redraw-r1480-v1" &&
                overlay.sourceRulesSha256 == FactoryProtoRulesR1466 && overlay.artApproved &&
                overlay.animationLibraryComplete && !overlay.nativeEquivalent &&
                overlay.keys != null && overlay.keys.Length == 106,
                "Proto identity/counts or pixel-bank qualification changed");
            var document = FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(ReadFactoryProtoRawR1466(overlay.document));
            ReadFactoryProtoRawR1466(overlay.evidence);
            ReadFactoryProtoRawR1466(overlay.artReview);
            FactoryProductNeedR1179(document != null && document.artAndSampledPhaseReview != null &&
                document.artAndSampledPhaseReview.sha256 == overlay.artReview.sha256,
                "exact Proto pixel and retained-phase review changed");
            factoryProtoModelRulesR1466 = ReadFactoryProtoRawR1466(overlay.modelRules);
            var model = FactoryTypedJsonR581.ReadFactoryRavenClonePhaseV21(factoryProtoModelRulesR1466);
            var profile = model == null ? null : model.prototype;
            var lookup = new SourcePhaseWholeLookupV21(document);
            FactoryProductNeedR1179(lookup.SourceNpc == FactoryProtoNpcR1466 && lookup.Id == overlay.bankID &&
                lookup.SourceRulesSha256 == FactoryProtoRulesR1466 && lookup.KeyCount == 106 &&
                lookup.MotionCount == 36 && lookup.StateCount == 46 && document.sourcePoseSamples == 141 &&
                document.sourceReferenceEvidence.sha256 == overlay.evidence.sha256 &&
                profile != null && profile.id == FactoryProtoNpcR1466 && profile.kind == overlay.kind &&
                profile.initialState == "RAVEN_CLONE_WAIT" && profile.bodyWidth == 28 && profile.bodyHeight == 42 &&
                profile.states != null && profile.states.Length == 46,
                "genuine Proto profile/whole identity or scale differs");
            // Check only presentation fields. The model keeps its own nextState,
            // event transitions, collision samples, HP and AI; the bank does not replace them.
            var states = new HashSet<string>(StringComparer.Ordinal);
            foreach (var state in profile.states) {
                FactoryProductNeedR1179(state != null && states.Add(state.id), "duplicate Proto model state");
                var visual = lookup.State(state.id);
                FactoryProductNeedR1179(visual.ModelClip == state.clip && visual.Motion == state.sourceAnimation &&
                    visual.StateDuration == state.duration && visual.Speed == state.sourceAnimSpeed &&
                    visual.Loop == state.loop, "Proto visual/source model state mismatch: " + state.id);
            }
            FactoryProductNeedR1179(profile.reactions != null && profile.reactions.Length == 3,
                "exact three original Proto reaction bindings required");
            foreach (var reaction in profile.reactions) {
                FactoryProductNeedR1179(reaction != null, "missing Proto reaction");
                string expected = reaction.combatState == CombatState.Hurt ? "RAVEN_CLONE_DAMAGE_SMALL_FRONT" :
                    reaction.combatState == CombatState.Knockdown ? "RAVEN_CLONE_DAMAGE_DOWN_FRONT" :
                    reaction.combatState == CombatState.GettingUp ? "RAVEN_CLONE_STAND_UP_FRONT" : null;
                FactoryProductNeedR1179(expected != null && reaction.sourceState == expected &&
                    !factoryProtoReactionsR1466.ContainsKey(reaction.combatState),
                    "changed or duplicate original Proto reaction identity");
                var visual = lookup.State(reaction.sourceState);
                FactoryProductNeedR1179(visual.Motion == reaction.sourceAnimation &&
                    visual.StateDuration == reaction.duration && visual.Speed == reaction.sourceAnimSpeed &&
                    !visual.Loop, "Proto reaction source pose/duration/speed differs");
                factoryProtoReactionsR1466.Add(reaction.combatState, new FactoryProtoReactionR1466(reaction));
            }
            var keys = new Dictionary<string, FactoryProtoKeyR1466>(StringComparer.Ordinal);
            foreach (var key in overlay.keys) {
                FactoryProductNeedR1179(key != null && !keys.ContainsKey(key.key), "duplicate Proto whole key");
                keys.Add(key.key, key);
            }
            foreach (var whole in lookup.Keys()) {
                FactoryProtoKeyR1466 key;
                FactoryProductNeedR1179(keys.TryGetValue(whole.Key, out key) && key.png != null &&
                    key.png.sha256 == whole.PngSha256 && key.rgbaSha256 == whole.RgbaSha256 &&
                    key.width == whole.Width && key.height == whole.Height &&
                    FactoryProductResourceIndexR1168.Identity(key.originalPath) ==
                        FactoryProductResourceIndexR1168.Identity(whole.PngPath),
                    "Proto pixel whole PNG/anchor binding changed: " + whole.Key);
                var bytes = ReadFactoryProtoRawR1466(key.png);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false) {
                    name = "Factory pixel Proto / " + whole.Key,
                    filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp
                };
                factoryProductOwnedR1179.Add(texture);
                FactoryProductNeedR1179(ImageConversion.LoadImage(texture, bytes, true) &&
                    texture.width == whole.Width && texture.height == whole.Height,
                    "Proto raw PNG decode/dimensions changed: " + whole.Key);
                var sprite = Sprite.Create(texture, new Rect(0, 0, whole.Width, whole.Height),
                    new Vector2(.5f, .5f), 1, 0, SpriteMeshType.FullRect);
                FactoryProductNeedR1179(sprite, "Proto FullRect sprite creation failed");
                factoryProductOwnedR1179.Add(sprite);
                factoryProtoSpritesR1466.Add(whole.Key, sprite);
            }
            FactoryProductNeedR1179(factoryProtoSpritesR1466.Count == 106, "incomplete independent Proto bank");
            factoryProtoOverlayR1466 = overlay;
            factoryProtoLookupR1466 = lookup;
        }

        AudioClip LoadFactoryProtoExtraSoundR1466() {
            var overlay = factoryProtoOverlayR1466;
            FactoryProductNeedR1179(overlay != null && overlay.soundID == "Raven_DashAttack2.ogg" &&
                overlay.sound != null && overlay.sound.bytes == 11299 &&
                overlay.sound.sha256 == "505b5993ce624a2926350209dbf2e67def44da996de6b67e3f0c6d6c15f2f01d" &&
                overlay.sound.resource == "Pixel/dungeons/altera-factory-normal/audio/" + overlay.sound.sha256,
                "exact additional Proto source audio binding required");
            // AudioClip import does not expose original OGG bytes. The staged
            // resource receipt proves those bytes; this gate checks actual decode availability.
            var clip = Resources.Load<AudioClip>(overlay.sound.resource);
            FactoryProductNeedR1179(clip && clip.loadState != AudioDataLoadState.Failed,
                "original Raven_DashAttack2 OGG unavailable");
            return clip;
        }

        void ConfigureFactoryRavenPrototypeModelR1466() {
            FactoryProductNeedR1179(factoryProtoLookupR1466 != null && factoryProtoModelRulesR1466 != null &&
                factoryProductComponentR1179 != null && ReferenceEquals(game, factoryProductComponentR1179.Model),
                "Proto model must be the same fresh Factory component");
            game.ConfigureFactoryRavenClonePhaseV21(factoryProtoModelRulesR1466, FactoryProtoAuthoredHpR1466);
        }

        bool TryRenderFactoryProductProtoR1466(Enemy actor, SpriteRenderer renderer) {
            if (!FactoryProductSessionActiveR1179 || actor.sourceNPC != FactoryProtoNpcR1466) return false;
            FactoryProductNeedR1179(factoryProtoLookupR1466 != null && game.FactoryRavenClonePhaseConfiguredV21 &&
                game.enemies.Contains(actor) && actor.kind == "raven-clone-proto" && actor.visualScale == 1,
                "current genuine Proto actor/whole/clock required");
            string displayState = actor.npcState; float displayClock = actor.npcTime;
            if (string.IsNullOrEmpty(displayState)) {
                FactoryProtoReactionR1466 reaction;
                if (actor.hp <= 0 || !factoryProtoReactionsR1466.TryGetValue(actor.state, out reaction)) {
                    // No fabricated death/unsupported airborne pose. Source only
                    // declares Hurt/Knockdown/GettingUp reaction mappings here.
                    renderer.enabled = false; renderer.sprite = null;
                    FactoryProtoLastStateR1466 = actor.npcState; FactoryProtoLastKeyR1466 = null;
                    return true;
                }
                displayState = reaction.state;
                // Exactly the reaction body's NpcHurtSpheres time convention:
                // clamp actor.clock to [0,duration], then the unchanged lookup
                // applies the source animation speed and non-loop phase clamp.
                displayClock = Mathf.Clamp(actor.clock, 0, reaction.duration);
            }
            var selected = factoryProtoLookupR1466.Select(displayState, displayClock);
            RecordMonsterPixelSourceV53(actor,renderer,"Factory/Proto",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
            var key = selected.Whole;
            renderer.enabled = true;
            renderer.sprite = factoryProtoSpritesR1466[key.Key];
            renderer.flipX = renderer.flipY = false;
            renderer.transform.localScale = Vector3.one;
            renderer.transform.localRotation = Quaternion.identity;
            // PNG geometry already has .28 * original UNIT_SCALE(1). Apply the
            // signed source anchor once; never borrow Boss scale(1.5) or mirror its pose.
            var offset = new Vector2((float)(key.Width * .5 - selected.AnchorX),
                (float)(selected.AnchorY - key.Height * .5));
            var position = RenderEnemyPosition(actor) + offset;
            renderer.transform.localPosition = new Vector3(position.x, position.y, 0);
            FactoryProtoLastStateR1466 = displayState;
            FactoryProtoLastKeyR1466 = key.Key;
            return true;
        }

        void ReleaseFactoryProtoOverlayR1466() {
            // The ordinary Factory owned-object/audio lifecycle destroys only
            // this session's sprites/textures and restores prior sound IDs.
            factoryProtoSpritesR1466.Clear(); factoryProtoReactionsR1466.Clear();
            factoryProtoLookupR1466 = null; factoryProtoOverlayR1466 = null;
            factoryProtoModelRulesR1466 = null;
            FactoryProtoLastStateR1466 = FactoryProtoLastKeyR1466 = null;
        }
    }
}
