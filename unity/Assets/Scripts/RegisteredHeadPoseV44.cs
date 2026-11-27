using System;
using UnityEngine;

namespace ElswordPixel {
    // These heads are one rigid painting each. The fitted screen bounds of a 3D
    // joint can approach zero during a turn. Applying those bounds as two scales
    // makes the lens or faceplate stretch sideways during hits and recovery.
    internal sealed class RegisteredHeadPoseV44 {
        readonly int part, support;
        readonly Vector2 neutralScale;
        readonly float neutralHeadAngle, neutralBearing, neutralDistance, neutralSupportAngle;
        readonly bool rigid;
        internal int Part { get { return part; } }

        internal static RegisteredHeadPoseV44 From(FairyPoseLibraryV33 lib) {
            string headName, supportName;
            bool rigid = true;
            switch (lib.rigName) {
                case "NUI_WALLY_9TH_TYPE2":
                case "NUI_NASOD_ELITE_GUARD":
                case "NUI_NASOD_ELITE_SHERIFF":
                    headName = "head"; supportName = "torso"; break;
                case "NUI_NASOD_SPRAY":
                case "NUI_NASOD_SPRAY_ICE":
                case "NUI_NASOD_ELEMENT_ICE_SOLO":
                    headName = "head"; supportName = "trunk"; break;
                case "NUI_RAVEN_CLONE":
                    headName = "helmet"; supportName = "trunk"; break;
                case "NUI_NASOD_GUARDIAN_SMALL":
                    // This scorpion's eyes sit in its upper shell; the moving
                    // pincers and stinger each have independent original tracks.
                    headName = "ivory-carapace-three-amber-lights"; supportName = null; break;
                case "NUI_NASOD_GUARD":
                    headName = "head"; supportName = "torso"; break;
                case "NUI_BENDERS_UPGRADE_NORMAL":
                case "NUI_BENDERS_THIEF":
                case "NUI_BENDERS_NORMAL":
                case "NUI_BENDERS_NASOD":
                case "NUI_SOLDIER_THIN":
                case "NUI_THIEF_THIN_ZERO":
                case "NUI_THIEF_THIN":
                case "NUI_THIEF_THIN_TOTO":
                    // Their head drawings already have a rigid aspect. Preserve
                    // the expressive turn and stop the occasional backward twist.
                    rigid = false; headName = "head"; supportName = "torso"; break;
                default:
                    return null;
            }
            int head = Array.FindIndex(lib.parts, p => p.name == headName);
            int body = supportName == null ? -1 : Array.FindIndex(lib.parts, p => p.name == supportName);
            var wait = lib.Find("Wait");
            if (head < 0 || (supportName != null && body < 0) || wait == null || wait.components != 5)
                throw new InvalidOperationException("Missing neutral mechanical head registration: " + lib.rigName);
            return new RegisteredHeadPoseV44(wait, head, body, rigid);
        }

        RegisteredHeadPoseV44(FairyPoseTrackV33 wait, int head, int body, bool rigid) {
            part = head;
            support = body;
            this.rigid = rigid;
            wait.SampleAxes(0, part, out var at, out neutralHeadAngle, out neutralScale);
            if (neutralScale.x <= 0 || neutralScale.y <= 0)
                throw new InvalidOperationException("Empty mechanical head at rest");
            if (body < 0) return;
            wait.Sample(0, body, out var seat, out neutralSupportAngle);
            var neck = at - seat;
            neutralDistance = neck.magnitude;
            neutralBearing = Bearing(neck);
        }

        static float Bearing(Vector2 v) {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        internal void Fit(FairyPoseTrackV33 track, float clock, Vector2 at, ref float angle, ref Vector2 axes) {
            if (rigid) {
                float x = axes.x / neutralScale.x, y = axes.y / neutralScale.y;
                float size = Mathf.Clamp(Mathf.Sqrt(x * y), .90f, 1.08f);
                float aspect = Mathf.Clamp(x / Mathf.Max(.00001f, y), .90f, 1.10f);
                float width = Mathf.Sqrt(aspect);
                axes = new Vector2(neutralScale.x * size * width, neutralScale.y * size / width);
            }
            if (support < 0) return;

            track.Sample(clock, support, out var seat, out float supportAngle);
            var neck = at - seat;
            float reach = neck.magnitude / Mathf.Max(.00001f, neutralDistance);
            float bodyTurn = Mathf.DeltaAngle(neutralSupportAngle, supportAngle);
            // When the camera looks down the neck axis the two centers may
            // coincide. Blend to torso roll before its projected bearing flips.
            float neckTurn = Mathf.DeltaAngle(neutralBearing, Bearing(neck));
            float baseAngle = neutralHeadAngle + Mathf.LerpAngle(bodyTurn, neckTurn, Mathf.InverseLerp(.20f, .48f, reach));
            float twist = Mathf.DeltaAngle(baseAngle, angle);
            float magnitude = Mathf.Abs(twist);
            // A rolled body can turn all the way over. An individual faceplate
            // can tilt but a backward-projected head cannot swivel 180 degrees.
            float turn = Mathf.Min(Mathf.Min(magnitude, rigid ? 32f : 62f), (180f - magnitude) * (rigid ? .75f : 1.5f));
            angle = baseAngle + Mathf.Sign(twist) * turn;
        }
    }
}
