using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        static bool IsHamelForegroundShell(string name) {
            // These exported groups contain camera-facing masonry or cave
            // walls alongside traversable ledges. The shared reveal pass
            // classifies their individual faces against the room's floors.
            switch (name) {
                case "Hamel_Resiam_Underwater_Stage1_Mesh03.Y":
                case "ANCIENT_WATERWAY_S3_Gr_up1.Y":
                case "ANCIENT_WATERWAY_S3_Gr_up11.Y":
                case "Hamel_Water_Temple_Entry_S1_Cavewall.Y":
                case "Hamel_Frozen_Water_Temple_S4_gr2.Y":
                case "Hamel_Frozen_Water_Temple_S4_gr3.Y":
                case "Hamel_Frozen_Water_Hall_Stage5_DoorLR.Y":
                    return true;
                default:
                    return false;
            }
        }
        [Serializable] sealed class HamelPose {
            public int offset, frames;
            public float duration;
            public float[] origin, unit;
        }
        [Serializable] sealed class HamelTrap {
            public int trigger;
            public float delay;
            public HamelPose wait, play;
        }
        sealed class HamelMovingPart {
            public Mesh mesh;
            public Vector3[] bind, output;
            public HamelTrap trap;
            public int lastOffset = -1;
        }
        readonly List<HamelMovingPart> hamelMovingParts = new List<HamelMovingPart>();
        byte[] hamelPoseBytes;

        void BuildHamelTriggers(NativeScene source) {
            if (string.IsNullOrEmpty(source.triggerBytes)) return;
            hamelPoseBytes = Resources.Load<TextAsset>(source.triggerBytes).bytes;
            for (int i=0; i<source.parts.Length; i++) {
                var trap = source.parts[i].trap;
                if (trap?.wait == null || trap.wait.frames == 0) continue;
                var mesh = nativeRoot.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
                mesh.MarkDynamic();
                hamelMovingParts.Add(new HamelMovingPart {
                    mesh=mesh, bind=mesh.vertices, output=new Vector3[mesh.vertexCount], trap=trap
                });
            }
            UpdateHamelTriggers();
        }
        void UpdateHamelTriggers() {
            foreach (var item in hamelMovingParts) {
                float time = game.HamelTriggerAge(item.trap.trigger) - item.trap.delay;
                var pose = time < 0 ? item.trap.wait : item.trap.play;
                int frame = pose.frames <= 1 || time <= 0 ? 0 :
                    Mathf.RoundToInt(Mathf.Clamp01(time / pose.duration) * (pose.frames - 1));
                bool skin = pose.origin != null && pose.origin.Length == 3;
                int offset = pose.offset + frame * (skin ? item.output.Length * 6 : 48);
                if (item.lastOffset == offset) continue;
                item.lastOffset = offset;
                if (skin) {
                    Vector3 origin=NativeV(pose.origin), unit=NativeV(pose.unit);
                    for (int i=0; i<item.output.Length; i++, offset+=6)
                        item.output[i] = origin + Vector3.Scale(unit, new Vector3(
                            BitConverter.ToUInt16(hamelPoseBytes,offset), BitConverter.ToUInt16(hamelPoseBytes,offset+2),
                            BitConverter.ToUInt16(hamelPoseBytes,offset+4)));
                } else {
                    var matrix = Matrix4x4.identity;
                    for (int row=0; row<3; row++)
                        for (int col=0; col<4; col++, offset+=4)
                            matrix[row,col]=BitConverter.ToSingle(hamelPoseBytes,offset);
                    for (int i=0; i<item.output.Length; i++) item.output[i]=matrix.MultiplyPoint3x4(item.bind[i]);
                }
                item.mesh.vertices = item.output;
                item.mesh.RecalculateBounds();
            }
        }
        void ClearHamelTriggers() {
            hamelMovingParts.Clear();
            hamelPoseBytes = null;
        }
    }
}
