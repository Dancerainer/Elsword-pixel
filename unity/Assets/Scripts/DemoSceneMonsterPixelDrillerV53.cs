using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class DrillerPixelClockTrackV53 {public string alias,animation;public float duration,fps;public bool loop;public int count,components;}
        [Serializable] sealed class DrillerPixelClockBankV53 {public string schema,rigName;public DrillerPixelClockTrackV53[] tracks;}
        DrillerPixelClockBankV53 drillerPixelClockBankV53;

        void ReadDrillerPixelClockV53(string alias,float displayClock,out string motion,out float sourceClock){
            if(drillerPixelClockBankV53==null){
                var source=Resources.Load<TextAsset>("Pixel/feedback-v35/nasod-driller-cannon/poses");
                string digest="";if(source)using(var hash=System.Security.Cryptography.SHA256.Create())digest=BitConverter.ToString(hash.ComputeHash(source.bytes)).Replace("-","").ToLowerInvariant();
                if(digest!="bab6ecc7ce589276c70d36a9dcfd8925f55d6d13dc67b85ac87add9a988d82f8")throw new InvalidOperationException("Original Driller registered source tracks changed");
                var bank=JsonUtility.FromJson<DrillerPixelClockBankV53>(source.text);
                if(bank.schema!="original-bone-part-rig-v35"||bank.rigName!=MineMachineRigV35.DrillerId||bank.tracks==null||bank.tracks.Length!=25)throw new InvalidOperationException("Original Driller clock identities changed");
                int frames=0;foreach(var track in bank.tracks){if(track.fps!=60||track.components!=5||track.duration<=0)throw new InvalidOperationException("Original Driller exposure clock changed");frames+=track.count;}
                if(frames!=2173)throw new InvalidOperationException("Original Driller exposure count changed");
                drillerPixelClockBankV53=bank;
            }
            var selected=Array.Find(drillerPixelClockBankV53.tracks,t=>t.alias==alias);
            if(selected==null||!MonsterPixelFramesV53.Finite(displayClock)||displayClock<0)throw new InvalidOperationException("Missing original displayed Driller track "+alias);
            // Exactly FairyPoseTrackV33.Segment: the producer's displayed clock,
            // then this track's own Repeat/clamp. Never substitute NPC time.
            motion=selected.animation;
            sourceClock=selected.loop?Mathf.Repeat(displayClock,selected.duration):Mathf.Clamp(displayClock,0,selected.duration);
        }
    }
}
