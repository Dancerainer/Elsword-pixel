using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
namespace ElswordPixel {
    internal static class WallyType2PartsV36 {
        internal static FairyPoseLibraryV33 LoadRig(){
            return RegisteredPartRigV35.Load(CoreElitePartsV36.Root(CoreElitePartsV36.Wally),CoreElitePartsV36.Wally,
                "11b08bdb1a69d84baa03a44248634317345db6b0e026b82b0df9138be0ee7e6f",lib=>{
                    if(lib.motionSha256!="6d976475206359e425973f41be58a8d4a35c68f58bc6ec11b251316598c74d6e"||lib.parts.Length!=16||lib.tracks.Length!=18||lib.authoredFacing!=1||lib.nativeYaw!=0)
                        throw new InvalidOperationException("Original Wally ninth Type2 assembly");
                    foreach(var required in new[]{"head","neck","near-boot","far-boot","near-grip","far-grip","near-blade","far-blade"})
                        if(!Array.Exists(lib.parts,q=>q.name==required))throw new InvalidOperationException("Wally Type2 missing "+required);
                    foreach(var t in lib.tracks)if(t.animation!=t.alias||t.fps!=24||t.components!=5||t.loop)
                        throw new InvalidOperationException("Original Wally Type2 source motion "+t.alias);
                });
        }
    }
    [Serializable] internal sealed class WallyType2DepthTrackV36 {
        public string motion;public int count,fps;public float[] values;
    }
    [Serializable] internal sealed class WallyType2DepthV36 {
        public string id,poseSha;public WallyType2DepthTrackV36[] tracks;
        Dictionary<string,WallyType2DepthTrackV36> byMotion;
        Vector2[][] outlines;
        const int Chest=1,Waist=2,FarThigh=6,FarShin=7,NearUpper=9,FarUpper=11,FarGrip=12;
        internal static WallyType2DepthV36 Load(FairyPoseLibraryV33 lib){
            var asset=Resources.Load<TextAsset>(CoreElitePartsV36.Root(CoreElitePartsV36.Wally)+"depth");
            if(!asset)throw new InvalidOperationException("Wally Type2 depth samples missing");
            using(var h=SHA256.Create())
                if(BitConverter.ToString(h.ComputeHash(asset.bytes)).Replace("-","").ToLowerInvariant()!="e3daa611103f98f2d71ab50300fea06e994c8b349706377143b283e995980ebf")
                    throw new InvalidOperationException("Wally Type2 source depth changed");
            var d=JsonUtility.FromJson<WallyType2DepthV36>(asset.text);
            if(d.id!=CoreElitePartsV36.Wally||d.poseSha!="f396a907e92c1b2cfd25d0e8253b5bc4690b4e619ff3c1669a5fb29682b71d2f"||d.tracks.Length!=18)
                throw new InvalidOperationException("Wally Type2 pose-depth identity");
            d.byMotion=new Dictionary<string,WallyType2DepthTrackV36>(StringComparer.Ordinal);
            foreach(var tr in d.tracks){
                var pose=lib.Find(tr.motion);
                if(pose==null||pose.count!=tr.count||tr.fps!=24||tr.values.Length!=tr.count*16)
                    throw new InvalidOperationException("Wally Type2 original arm/weapon depth "+tr.motion);
                d.byMotion.Add(tr.motion,tr);
            }
            d.outlines=new Vector2[lib.parts.Length][];
            for(int i=0;i<lib.parts.Length;i++){
                var part=lib.parts[i];var hull=part.visibleHull;
                var outline=new Vector2[hull.Length/2];
                for(int k=0;k<outline.Length;k++)outline[k]=new Vector2(hull[k*2]*(part.mirrorX?-1:1),hull[k*2+1])*lib.UnitsPerPixel;
                d.outlines[i]=outline;
            }
            return d;
        }
        // Intersect the ray between two mounted pieces with their painted
        // outlines. The sprite rectangle includes transparent atlas space.
        float Reach(int part,Transform view,Vector2 toward){
            Vector2 ray=Quaternion.Inverse(view.localRotation)*toward;
            var scale=view.localScale;if(scale.x<=0||scale.y<=0)return 0;
            ray=new Vector2(ray.x/scale.x,ray.y/scale.y);
            var hull=outlines[part];float reach=0;
            var previous=hull[hull.Length-1];
            for(int i=0;i<hull.Length;i++){
                var point=hull[i];var edge=point-previous;
                float cross=ray.x*edge.y-ray.y*edge.x;
                if(Mathf.Abs(cross)>.000001f){
                    float u=(previous.x*ray.y-previous.y*ray.x)/cross;
                    if(u>=0&&u<=1)reach=Mathf.Max(reach,(previous.x*edge.y-previous.y*edge.x)/cross);
                }
                previous=point;
            }
            return reach;
        }
        void Seat(SpriteRenderer[] parts,int plate,int joint,float limit,float overlap=1.6f){
            var cover=parts[plate].transform;var socket=parts[joint].transform;
            Vector2 delta=socket.localPosition-cover.localPosition;
            float distance=delta.magnitude;if(distance<1)return;
            var direction=delta/distance;
            float radius=Reach(plate,cover,direction);
            if(radius<.5f)return;
            float gap=distance-radius-Reach(joint,socket,-direction);
            if(gap<= -overlap)return;
            var scale=cover.localScale;
            float grow=Mathf.Min(limit,1+(gap+overlap)/radius);
            cover.localScale=new Vector3(scale.x*grow,scale.y*grow,1);
        }
        internal void Apply(string motion,float clock,SpriteRenderer[] parts,float[] z){
            var t=byMotion[motion];float sample=Mathf.Clamp(clock*t.fps,0,t.count-1);int a=(int)sample,b=Mathf.Min(a+1,t.count-1);
            for(int i=0;i<parts.Length;i++)z[i]=Mathf.Lerp(t.values[a*parts.Length+i],t.values[b*parts.Length+i],sample-a);
            if(motion!="Wait"&&motion!="Walk"){
                // Only open mounts need cover. Keep blade tips, hand centers,
                // planted boots and the original attack/recovery timing.
                Seat(parts,Waist,Chest,1.42f);
                Seat(parts,Chest,Waist,1.28f);
                Seat(parts,NearUpper,Chest,1.38f);
                Seat(parts,FarUpper,Chest,1.38f);
                Seat(parts,FarGrip,FarUpper,1.35f);
                Seat(parts,FarShin,FarThigh,1.28f);
                // The concave dark rim on the knee extends inside its convex
                // outline. Give the upper plate enough overlap to cover it.
                Seat(parts,FarThigh,FarShin,1.35f,3.5f);
            }
            for(int i=0;i<parts.Length;i++){
                int rank=0;
                for(int j=0;j<parts.Length;j++)if(z[j]>z[i]||z[j]==z[i]&&j<i)rank++;
                parts[i].sortingOrder=rank;
            }
        }
    }
}
