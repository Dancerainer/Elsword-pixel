using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        FactoryRavenFocusOwnerV21 factoryRavenFocusOwnerV21;
        FactoryRavenIntroTicketV21 factoryRavenTicketV21;
        CoreStopPairedCameraScopeDraftV21 factoryRavenCameraV21;
        CoreStopWholeEnvelopeV22 factoryRavenEnvelopeV21;
        FactoryRavenIntroDrawWitnessV21 factoryRavenWitnessV21;
        SpriteRenderer factoryRavenBodyV21;
        Sprite factoryRavenSelectedSpriteV21;
        CoreStopSourceActorFrameV21 factoryRavenFramingFrameV35;
        int factoryRavenSelectionFrameV21 = -1, factoryRavenDrawFrameV21 = -1;
        int factoryRavenObservedFrameV21 = -1;
        bool factoryRavenObservationSubmittedV21;

        void PrepareFactoryRavenEnvelopeV21(SourceWholeDocumentV21 document, SourcePhaseWholeLookupV21 lookup) {
            var binding = lookup.State("RAVEN_CLONE_START");
            FactoryProductNeedR1179(lookup.SourceNpc == "NUI_RAVEN_CLONE" && !binding.Loop &&
                binding.Motion == "WaitStart" && binding.StateDuration == 3.8f,
                "Factory Raven original START binding changed");
            var motion = Array.Find(document.motions, value => value.id == binding.Motion);
            FactoryProductNeedR1179(motion != null && motion.phases != null && motion.phases.Length > 0,
                "Factory Raven complete START phase envelope missing");
            var envelope = new CoreStopWholeEnvelopeV22 {
                npc = lookup.SourceNpc, state = "RAVEN_CLONE_START", motion = binding.Motion,
                phaseCount = motion.phases.Length, minX = double.PositiveInfinity,
                minY = double.PositiveInfinity, maxX = double.NegativeInfinity, maxY = double.NegativeInfinity
            };
            foreach (var phase in motion.phases) {
                var key = Array.Find(document.keys, value => value.key == phase.paintedWholeKey);
                FactoryProductNeedR1179(key != null && phase.anchor != null && phase.anchor.Length == 2,
                    "Factory Raven complete signed source frame missing");
                double left = -phase.anchor[0], top = phase.anchor[1];
                envelope.minX = Math.Min(envelope.minX, left);
                envelope.maxX = Math.Max(envelope.maxX, left + key.width);
                envelope.minY = Math.Min(envelope.minY, top - key.height);
                envelope.maxY = Math.Max(envelope.maxY, top);
            }
            FactoryProductNeedR1179(!double.IsNaN(envelope.minX) && !double.IsInfinity(envelope.minX) &&
                !double.IsNaN(envelope.minY) && !double.IsInfinity(envelope.minY) &&
                !double.IsNaN(envelope.maxX) && !double.IsInfinity(envelope.maxX) &&
                !double.IsNaN(envelope.maxY) && !double.IsInfinity(envelope.maxY) &&
                envelope.maxX > envelope.minX && envelope.maxY > envelope.minY,
                "Factory Raven nonfinite START envelope");
            FactoryRavenRigV35.IncludeStartEnvelope(envelope);
            factoryRavenEnvelopeV21 = envelope;
        }

        void BeginFactoryRavenIntroViewV21() {
            FactoryProductNeedR1179(factoryRavenFocusOwnerV21 == null && factoryRavenEnvelopeV21 != null,
                "Factory Raven fresh view and original START envelope required");
            game.ConfigureFactoryRavenIntroV21();
            ConfigureFactoryRavenPrototypeModelR1466();
            factoryRavenFocusOwnerV21 = game.RegisterFactoryRavenFocusOwnerV21(this);
            Camera.onPostRender += ObserveFactoryRavenIntroPostRenderV21;
        }

        bool FactoryRavenIntroPendingV21(FactoryRavenIntroTicketV21 ticket) {
            if (!FactoryProductSessionActiveR1179 || ticket == null) return false;
            var status = game.FactoryRavenIntroStatusV21(ticket);
            return status != null && (status.phase == CoreStopIntroPhaseV21.AwaitingFocus ||
                status.phase == CoreStopIntroPhaseV21.PlayingSourceStart);
        }

        void PrepareFactoryRavenIntroViewV21() {
            var current = FactoryProductSessionActiveR1179 ? game.CurrentFactoryRavenIntroTicketV21() : null;
            if (!FactoryRavenIntroPendingV21(current)) current = null;
            if (!ReferenceEquals(current, factoryRavenTicketV21)) {
                ReleaseFactoryRavenIntroCameraV21();
                factoryRavenTicketV21 = current;
                factoryRavenObservationSubmittedV21 = false;
            }
            factoryRavenSelectionFrameV21 = -1;
        }

        void RecordFactoryRavenIntroSelectionV21(Enemy actor, SpriteRenderer body) {
            var ticket = factoryRavenTicketV21;
            if (ticket == null || !ReferenceEquals(ticket.Actor, actor) || actor.npcState != ticket.InitialState)
                return;
            FactoryProductNeedR1179(body && body.sprite && body.enabled && body.gameObject.activeInHierarchy,
                "Factory Raven current START renderer is unavailable");
            if (factoryRavenBodyV21 != body) {
                if (factoryRavenWitnessV21) Destroy(factoryRavenWitnessV21);
                factoryRavenBodyV21 = body;
                factoryRavenWitnessV21 = body.gameObject.AddComponent<FactoryRavenIntroDrawWitnessV21>();
                factoryRavenWitnessV21.Bind(this, actor, body);
            }
            factoryRavenSelectedSpriteV21 = body.sprite;
            factoryRavenSelectionFrameV21 = Time.frameCount;
        }

        void ApplyFactoryRavenIntroCameraV21() {
            try { ApplyFactoryRavenIntroCameraOwnedV21(); }
            catch (Exception failure) { CloseFactoryProductAfterFailureR1179(failure); throw; }
        }

        void ApplyFactoryRavenIntroCameraOwnedV21() {
            if (!FactoryRavenIntroPendingV21(factoryRavenTicketV21) ||
                factoryRavenSelectionFrameV21 != Time.frameCount) return;
            FactoryProductNeedR1179(NativeWorldActive && nativeScene.id == "WI_ALTERA_NASOD_FACTORY_BOSS" &&
                worldCamera && nativeCamera && nativeTarget && nativeScreen,
                "Factory Raven current Boss room camera is unavailable");
            if (factoryRavenCameraV21 == null) {
                float follow = cameraFollowY, velocity = cameraFollowVelocity;
                int followRoom = cameraFollowRoom;
                var context = new CoreStopCameraContextV21 {
                    world = worldCamera, native = nativeCamera, screen = nativeScreen.transform,
                    target = nativeTarget, sourceUnit = nativeScene.unit,
                    updateNativeCamera = () => UpdateNativeCamera(),
                    safeViewport = CoreStopMeasuredSafeViewportDraftV21,
                    readFollowState = () => new CoreIntroFollowStateV25 {
                        y = cameraFollowY, velocity = cameraFollowVelocity, room = cameraFollowRoom
                    },
                    restoreFollow = () => {
                        cameraFollowY = follow; cameraFollowVelocity = velocity; cameraFollowRoom = followRoom;
                    }
                };
                var root=monsterPixelSelectionsV53.TryGetValue(factoryRavenTicketV21.Actor,out var selected)?MonsterPixelLogicalRootV53(selected):RenderEnemyPosition(factoryRavenTicketV21.Actor);
                factoryRavenFramingFrameV35=new CoreStopSourceActorFrameV21{
                    actor=factoryRavenTicketV21.Actor,wholeStartEnvelopeV22=factoryRavenEnvelopeV21,
                    logicalActorRootV22=new Vector3(root.x,root.y,0)};
                factoryRavenCameraV21=new CoreStopPairedCameraScopeDraftV21(context,factoryRavenFramingFrameV35);
            }
            string reason;
            FactoryProductNeedR1179(factoryRavenCameraV21.ApplyWholeFrameV22(factoryRavenFramingFrameV35,out reason),
                "Factory Raven full-pose opening focus: "+reason);
            SyncCameraOverlayV25();
            // Keep the opening readable and put its name in the existing upper
            // HUD. RenderHud restores these ordinary labels on the release frame.
            foreach (string name in new[] { "Banner Panel", "Banner", "Banner Sub" }) Visible(name, false);
            TextValue("Title", "NASOD FACTORY / RAVEN");
            TextValue("Mission", "BOSS ENCOUNTER");
        }

        internal void ObserveFactoryRavenIntroDrawV21(Enemy actor, SpriteRenderer body, Camera camera, int frame) {
            if (factoryRavenTicketV21 == null || factoryRavenCameraV21 == null ||
                !ReferenceEquals(actor, factoryRavenTicketV21.Actor) || body != factoryRavenBodyV21 ||
                body.sprite != factoryRavenSelectedSpriteV21 || camera != worldCamera ||
                factoryRavenSelectionFrameV21 != frame || factoryRavenCameraV21.appliedFrame != frame) return;
            if(body.forceRenderingOff && (ravenDrawFrameV35!=frame || ravenDrawMaskV35!=(1<<FactoryRavenRigV35.PartCount)-1))return;
            factoryRavenDrawFrameV21 = frame;
        }

        void ObserveFactoryRavenIntroPostRenderV21(Camera camera) {
            if (!FactoryRavenIntroPendingV21(factoryRavenTicketV21) || camera != worldCamera ||
                !camera || !camera.enabled || camera.targetTexture != null || !Application.isPlaying ||
                factoryRavenCameraV21 == null || !factoryRavenCameraV21.Valid() ||
                factoryRavenDrawFrameV21 != Time.frameCount ||
                factoryRavenSelectionFrameV21 != Time.frameCount || !factoryRavenBodyV21 ||
                !factoryRavenBodyV21.enabled || !factoryRavenBodyV21.gameObject.activeInHierarchy ||
                factoryRavenBodyV21.color.a <= 0 || factoryRavenBodyV21.sprite != factoryRavenSelectedSpriteV21)
                return;
            var safe = CoreStopMeasuredSafeViewportDraftV21();
            if(!FactoryRavenOpeningBoundsV35(factoryRavenBodyV21,out var bounds))return;
            var low = camera.WorldToViewportPoint(bounds.min);
            var high = camera.WorldToViewportPoint(bounds.max);
            if (low.z < camera.nearClipPlane || high.z > camera.farClipPlane || low.x < safe.xMin ||
                high.x > safe.xMax || low.y < safe.yMin || high.y > safe.yMax) return;
            factoryRavenObservedFrameV21 = Time.frameCount;
        }

        void BeforeFactoryRavenIntroStepV21() {
            if (factoryRavenObservationSubmittedV21 || !FactoryRavenIntroPendingV21(factoryRavenTicketV21) ||
                factoryRavenObservedFrameV21 < Time.frameCount - 1 || factoryRavenCameraV21 == null ||
                !factoryRavenCameraV21.Valid()) return;
            factoryRavenObservationSubmittedV21 = game.QueueFactoryRavenFocusObservationV21(
                factoryRavenFocusOwnerV21, factoryRavenTicketV21, this, true, true);
        }

        void ReleaseFactoryRavenIntroCameraV21() {
            try {
                if (factoryRavenCameraV21 != null) factoryRavenCameraV21.Dispose();
            } finally {
                factoryRavenCameraV21 = null;
                if (factoryRavenWitnessV21) Destroy(factoryRavenWitnessV21);
                factoryRavenWitnessV21 = null; factoryRavenBodyV21 = null; factoryRavenSelectedSpriteV21 = null;
                factoryRavenSelectionFrameV21 = factoryRavenDrawFrameV21 = factoryRavenObservedFrameV21 = -1;
            }
        }

        void EndFactoryRavenIntroViewV21() {
            Camera.onPostRender -= ObserveFactoryRavenIntroPostRenderV21;
            try { ReleaseFactoryRavenIntroCameraV21(); }
            finally {
                if (factoryRavenFocusOwnerV21 != null && factoryProductComponentR1179 != null)
                    factoryProductComponentR1179.Model.EndFactoryRavenFocusOwnerV21(factoryRavenFocusOwnerV21);
                factoryRavenFocusOwnerV21 = null; factoryRavenTicketV21 = null; factoryRavenEnvelopeV21 = null;
                factoryRavenObservationSubmittedV21 = false;
            }
        }
    }
}
