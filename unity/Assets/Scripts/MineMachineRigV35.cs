using System;
using UnityEngine;
namespace ElswordPixel {
    public static class MineMachineRigV35 {
        public const string Rail="nasod-rail", Driller="nasod-driller-cannon";
        public const string RailId="NUI_NASOD_RAIL", DrillerId="NUI_NASOD_DRILLER_CANNON";
        public static string ResourceRoot(string kind) {return "Pixel/feedback-v35/"+kind+"/";}
        public static FairyPoseLibraryV33 Load(string kind){
            bool rail=kind==Rail;
            if(!rail&&kind!=Driller)throw new ArgumentException("Original machine kind",nameof(kind));
            string expected=rail?"5790345d296dab87c8d0cbe02112e825970f7a12da45ac50e200a339a1fcaf75":"0bfe68ad9b31fbba6073f7b6691502c231a22a5a56b2b582b0df90bef7a7a8f2";
            return RegisteredPartRigV35.Load(ResourceRoot(kind),rail?RailId:DrillerId,expected,bank=>{
                int count=0;foreach(var track in bank.tracks){
                    if(track.components!=5||track.fps!=60)throw new InvalidOperationException("Source projected tracks");
                    count+=track.count;
                }
                if(bank.parts.Length!=(rail?11:8)||bank.tracks.Length!=(rail?4:25)||count!=(rail?200:2173)||
                    bank.authoredFacing!=1||bank.heroHeight!=67||Math.Abs(bank.heightRatio-(rail?1.35370589f:2.83945020f))>1e-5f||
                    bank.parts[6].instanceOf!=(rail?3:6)||(!rail&&bank.parts[7].instanceOf!=5))
                    throw new InvalidOperationException("Original machine part, sample and scale contract "+kind);
            });
        }
        internal static void SeatPusherKnee(FairyPoseTrackV33 motion,SpriteRenderer[] parts){
            if(motion.animation!="DashAttack")return;
            // The registered calf opens at the knee when the push cycle wraps.
            // Fit the two armor pieces over their joint after the normal pose
            // is sampled; the planted boot and both hands keep their anchors.
            var thigh=parts[9].transform;var shin=parts[10].transform;
            thigh.localScale=Vector3.Scale(thigh.localScale,new Vector3(1.22f,1.22f,1));
            shin.localScale=Vector3.Scale(shin.localScale,new Vector3(1.15f,1.15f,1));
        }
    }
}
