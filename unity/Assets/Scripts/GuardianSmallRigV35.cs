using System;
namespace ElswordPixel {
    public static class GuardianSmallRigV35 {
        public const string Npc="NUI_NASOD_GUARDIAN_SMALL",Kind="nasod-guardian-small";
        public const string Root="Pixel/feedback-v35/nasod-guardian-small/";
        public const string MotionSha="07d55bcfc7da876487baabd34e61dbfbf512c9248b749cd7548c0862de39ae11";
        public const string PagesSha="f63444c425f1c8a1f9145bcc12e52f27663c058f23967647033367f61c5a7c25";
        public static FairyPoseLibraryV33 Load(){
            var rig=RegisteredPartRigV35.Load(Root,Npc,PagesSha,bank=>{
                int count=0;
                foreach(var t in bank.tracks){
                    if(t.alias!=t.animation||t.fps!=60||t.components!=5||t.loop)throw new InvalidOperationException("Original Guardian clocks");
                    count+=t.count;
                }
                if(bank.motionSha256!=MotionSha||bank.parts.Length!=12||bank.parts[9].instanceOf!=7||
                    bank.tracks.Length!=20||count!=1432||bank.authoredFacing!=1||
                    Math.Abs(bank.heroHeight*bank.heightRatio-82.00020574f)>.0001f)
                    throw new InvalidOperationException("Original Guardian source rig and scale");
            });
            // These four center-mounted paintings fit inside the exposed
            // sockets on either side. At the original source scale up to four
            // battle pixels of background show through between adjacent rings.
            // Register the armor overlap once. All pivot paths, the tail curl,
            // the stinger's mounting point and the source attack clock stay
            // with the original bones.
            rig.parts[6].scale*=1.30f; // second tail ring
            rig.parts[9].scale*=1.25f; // mirrored ring on the upper curve
            rig.parts[7].scale*=1.56f; // small terminal joint under the stinger
            rig.parts[8].scale*=1.12f; // stinger socket
            return rig;
        }
    }
}
