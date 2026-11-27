using System;
using UnityEngine;
namespace ElswordPixel {
    public static class FactoryRavenRigV35 {
        public const string Npc="NUI_RAVEN_CLONE",Root="Pixel/feedback-v35/factory-raven-clone/";
        public const int PartCount=14;
        const string Pages="d60d68ebbd41117c76c8bc2071c55fdc34d4e1b783c2823655f1ac74b67232b8";
        public static FairyPoseLibraryV33 Load(){
            return RegisteredPartRigV35.Load(Root,Npc,Pages,lib=>{
                if(lib.authoredFacing!=1||lib.nativeYaw!=0||lib.parts.Length!=PartCount||lib.tracks.Length!=38)
                    throw new InvalidOperationException("Factory Raven registered identity");
                int total=0;foreach(var t in lib.tracks){
                    if(t.alias!=t.animation||t.loop||t.fps!=30||t.components!=5)throw new InvalidOperationException("Original Raven motion "+t.alias);
                    total+=t.count;
                }
                if(total!=1240 || lib.parts[3].name!="blade" || lib.parts[11].name!="claw" ||
                    lib.parts[12].name!="right-boot" || lib.parts[13].name!="left-boot")
                    throw new InvalidOperationException("Factory Raven original blade, claw and 1240 source samples");
            });
        }
        // Exact WaitStart registered opaque hull envelope in gameplay units at all 115
        // source samples. Include two logical pixels at each edge for the sprite outline.
        internal static void IncludeStartEnvelope(CoreStopWholeEnvelopeV22 envelope) {
            envelope.minX=Math.Min(envelope.minX,-102.58);envelope.maxX=Math.Max(envelope.maxX,77.12);
            envelope.minY=Math.Min(envelope.minY,-15.72);envelope.maxY=Math.Max(envelope.maxY,134.2);
        }
    }
}
