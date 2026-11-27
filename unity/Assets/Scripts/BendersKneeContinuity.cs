using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    internal sealed partial class BendersFamilyRigV36 {
        // These rigs wear voluminous, registered pants. Keep the paintings and
        // their transverse scale. Only correct a rapid change in knee flexion.
        bool ArticulatedTrousers {
            get { return npc=="NUI_BENDERS_THIEF" || npc=="NUI_BENDERS_NASOD" ||
                         npc=="NUI_BENDERS_NORMAL" || npc=="NUI_BENDERS_UPGRADE_NORMAL"; }
        }
        bool FixLargeLegMotion(string motion) {
            return motion.StartsWith("Attack",StringComparison.Ordinal) || (npc=="NUI_BENDERS_THIEF" && motion=="Dash");
        }
        readonly Dictionary<BendersDepthTrackV36,Vector4[][]> kneeOffsets =
            new Dictionary<BendersDepthTrackV36,Vector4[][]>();

        Vector4[] BakeKneeOffsets(FairyPoseTrackV33 pose,BendersDepthTrackV36 depth,int side) {
            int count=pose.count,first=side*7;
            var shape=new Vector2[count];
            var bend=new float[count];
            var correction=new Vector4[count];
            var angles=new Vector2[count];
            var reach=new float[count];
            bool loop=pose.loop||pose.animation=="Dash"||pose.animation=="Walk";
            int period=count-1;
            // The duplicate last frame is the loop seam. Nonlooping clips have
            // fixed start/end poses to keep transitions into other clips intact.
            int Frame(int index){return loop?(index%period+period)%period:Mathf.Clamp(index,0,period);}
            if(count<4 || pose.count!=depth.count || pose.fps!=depth.fps)return correction;
            for(int i=0;i<count;i++){
                int offset=(i*this.count+first)*5;
                Vector2 hip=new Vector2(pose.values[offset]-lib.foot[0],lib.foot[1]-pose.values[offset+1])*lib.UnitsPerPixel;
                Vector2 knee=lib.limbs.KneeAtFrame(first,pose,i,out var rawAngles);
                Vector2 ankle=SampleBootPivot(depth,side,i,i,0,lib.parts[first+2].visibleHull,out _,out _,out _,out _);
                Vector2 axis=(ankle-hip).normalized;
                Vector2 normal=new Vector2(-axis.y,axis.x);
                shape[i]=new Vector2(Vector2.Dot(knee-hip,axis),Vector2.Dot(knee-hip,normal));
                bend[i]=Vector2.SignedAngle(knee-hip,ankle-knee);
                reach[i]=(ankle-hip).magnitude;
                // Relative to the waist: a whole-body rotation remains authored.
                float waist=-pose.values[(i*this.count+6)*5+2];
                angles[i]=rawAngles-Vector2.one*waist;
            }
            float threshold=Mathf.Max(20,1200f/pose.fps);
            bool Spike(float[] values,int i) {
                float incoming=Mathf.DeltaAngle(values[Frame(i-1)],values[i]);
                float outgoing=Mathf.DeltaAngle(values[i],values[Frame(i+1)]);
                return Mathf.Max(Mathf.Abs(incoming),Mathf.Abs(outgoing))>threshold &&
                       Mathf.Abs(outgoing-incoming)>threshold;
            }
            int radius=Mathf.Clamp(Mathf.RoundToInt(pose.fps*.13f),2,Mathf.Min(14,period));
            float Fade(int i){return loop ? 1 : Mathf.SmoothStep(0,1,Mathf.Min(i,period-i)/(float)radius);}
            var jointSpikes=new bool[count];
            for(int i=0;i<count;i++)jointSpikes[i]=Spike(bend,i);
            var upper=new float[count];var lower=new float[count];
            for(int i=0;i<count;i++){
                float weight=0,mix=0;var sum=Vector2.zero;
                for(int d=-radius;d<=radius;d++){
                    int j=Frame(i+d);float w=radius+1-Mathf.Abs(d);
                    sum+=shape[j]*w;weight+=w;
                    if(jointSpikes[j])mix=Mathf.Max(mix,w/(radius+1));
                }
                Vector2 move=Vector2.ClampMagnitude((sum/weight-shape[i])*mix*Fade(i),height*.20f);
                correction[i]=new Vector4(move.x,move.y,0,0);
                Vector2 shin=new Vector2(reach[i],0)-shape[i];
                float upperAngle=angles[i].x+Vector2.SignedAngle(shape[i],shape[i]+move);
                float lowerAngle=angles[i].y+Vector2.SignedAngle(shin,shin-move);
                upper[i]=i==0 ? upperAngle : upper[i-1]+Mathf.DeltaAngle(upper[i-1],upperAngle);
                lower[i]=i==0 ? lowerAngle : lower[i-1]+Mathf.DeltaAngle(lower[i-1],lowerAngle);
            }
            void CorrectPaint(float[] curve,bool thigh) {
                var spikes=new bool[count];
                for(int i=0;i<count;i++)spikes[i]=Spike(curve,i);
                for(int i=0;i<count;i++){
                    float sum=0,weight=0,mix=0;
                    for(int d=-radius;d<=radius;d++){
                        int j=Frame(i+d);float w=radius+1-Mathf.Abs(d);
                        int wrap=loop && i+d<0 ? -1 : loop && i+d>=period ? 1 : 0;
                        sum+=(curve[j]+wrap*(curve[period]-curve[0]))*w;weight+=w;
                        if(spikes[j])mix=Mathf.Max(mix,w/(radius+1));
                    }
                    float change=(sum/weight-curve[i])*mix*Fade(i);
                    if(thigh)correction[i].z=change;else correction[i].w=change;
                }
            }
            CorrectPaint(upper,true);CorrectPaint(lower,false);
            return correction;
        }

        void SmoothBendersKnees(FairyAuthoredPartsV33.Frame frame,BendersDepthTrackV36 depth,SpriteRenderer[] members,Transform[] holders) {
            if(!kneeOffsets.TryGetValue(depth,out var offsets)){
                FairyPoseTrackV33 pose=lib.Find(frame.alias);
                offsets=new[]{BakeKneeOffsets(pose,depth,0),BakeKneeOffsets(pose,depth,1)};
                kneeOffsets.Add(depth,offsets);
            }
            float at=Mathf.Clamp(frame.sourceClock*depth.fps,0,depth.count-1);
            int a=Mathf.FloorToInt(at),b=Math.Min(a+1,depth.count-1);
            for(int side=0;side<2;side++){
                Vector4 offset=Vector4.Lerp(offsets[side][a],offsets[side][b],at-a);
                if(offset.sqrMagnitude<.0001f)continue;
                int first=side*7;
                Transform thigh=holders[first] ? holders[first] : members[first].transform;
                Transform shin=holders[first+1] ? holders[first+1] : members[first+1].transform;
                Vector2 hip=thigh.localPosition,knee=shin.localPosition,ankle=holders[first+2].localPosition;
                Vector2 axis=(ankle-hip).normalized;
                Vector2 softened=knee+offset.x*axis+offset.y*new Vector2(-axis.y,axis.x);
                TurnTrouser(thigh,knee-hip,softened-hip,offset.z);
                TurnTrouser(shin,ankle-knee,ankle-softened,offset.w);
                shin.localPosition=softened;
            }
        }
        static void TurnTrouser(Transform part,Vector2 before,Vector2 after,float angleOffset) {
            if(before.sqrMagnitude<.001f || after.sqrMagnitude<.001f)return;
            part.localRotation=Quaternion.Euler(0,0,Vector2.SignedAngle(before,after)+angleOffset)*part.localRotation;
        }
    }
}
