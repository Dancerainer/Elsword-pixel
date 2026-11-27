using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] sealed class GuardDepthTrackV35 {
        public string motion;
        public int count, fps;
        public float[] values;
    }
    [Serializable] sealed class GuardDepthV35 {
        public string schema,motionSha,poseSha;
        public GuardDepthTrackV35[] tracks;
        readonly Dictionary<string,GuardDepthTrackV35> motions=new Dictionary<string,GuardDepthTrackV35>(StringComparer.Ordinal);

        public static GuardDepthV35 Load(FairyPoseLibraryV33 lib) {
            var text=Resources.Load<TextAsset>("Pixel/feedback-v35/nasod-guard/depth");
            if(!text)throw new InvalidOperationException("Missing original guard mesh depth");
            using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(text.bytes)).Replace("-","").ToLowerInvariant()!=GuardPagesPinV35.DepthSha)
                throw new InvalidOperationException("Original guard mesh depth hash");
            var data=JsonUtility.FromJson<GuardDepthV35>(text.text);
            if(data.schema!="factory-guard-source-depth-v35"||data.motionSha!=GuardRigV35.OriginalMotionSha||data.poseSha!=GuardPagesPinV35.PoseSha||data.tracks.Length!=20)
                throw new InvalidOperationException("Original guard depth scope");
            foreach(var t in data.tracks) {
                var track=lib.Find(t.motion);
                if(track==null||track.count!=t.count||track.fps!=t.fps||t.values.Length!=GuardRigV35.PartCount*t.count)throw new InvalidOperationException("Original guard depth samples");
                data.motions.Add(t.motion,t);
            }
            return data;
        }
        public void Apply(string motion,float time,SpriteRenderer[] views,float[] depth) {
            var t=motions[motion];int a=Mathf.Clamp(Mathf.FloorToInt(time*t.fps),0,t.count-1),b=Math.Min(a+1,t.count-1);
            float phase=Mathf.Clamp01(time*t.fps-a);
            for(int i=0;i<GuardRigV35.PartCount;i++)depth[i]=Mathf.Lerp(t.values[a*GuardRigV35.PartCount+i],t.values[b*GuardRigV35.PartCount+i],phase);
            for(int i=0;i<GuardRigV35.PartCount;i++) {
                int rank=0;
                // Match the source camera depth test (smaller Z is closer).
                // Source cutout order provides a stable tie-break.
                for(int j=0;j<GuardRigV35.PartCount;j++)if(depth[j]>depth[i]||depth[j]==depth[i]&&j<i)rank++;
                views[i].sortingOrder=rank;
            }
        }
    }
}

