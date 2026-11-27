using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Some original sheets register the *center* of a metal plate rather than
    // its endpoint. Keep those centers and the attached weapons exactly on the
    // native track, but stop a painted limb from becoming a rotating sliver.
    internal sealed class RegisteredLimbVolume {
        sealed class Plate {
            public int part;
            public Vector2 rest;
            public bool leg;
            public float MinimumConfidence {get{return leg?.72f:.55f;}}
            public readonly Dictionary<FairyPoseTrackV33,float[]> angles=new Dictionary<FairyPoseTrackV33,float[]>();
        }
        readonly Plate[] plates;
        readonly int body;
        RegisteredLimbVolume(FairyPoseLibraryV33 lib) {
            plates=new Plate[lib.parts.Length];
            body=Array.FindIndex(lib.parts,part=>part.name=="torso"||part.name=="trunk"||
                part.name=="miner-body-pack"||part.name=="ivory-carapace-three-amber-lights");
            // The mine-cart pusher's authored standing pose is the start of its
            // dash. It has no separate Wait clip.
            var wait=lib.Find(lib.rigName=="NUI_NASOD_RAIL"?"source-dash-attack-once":"Wait");
            if(wait==null)return;
            for(int i=0;i<plates.Length;i++) {
                string part=lib.parts[i].name;
                bool leg=part.EndsWith("-thigh",StringComparison.Ordinal)||part.EndsWith("-shin",StringComparison.Ordinal)||
                    part.EndsWith("-thigh-and-knee",StringComparison.Ordinal)||part.EndsWith("-leg",StringComparison.Ordinal);
                if(!(leg||
                    part.EndsWith("-upper",StringComparison.Ordinal)||part=="left-forearm"||part=="claw-fore"||
                    part=="near-grip"||part=="far-grip"||part=="near-shoulder"||part=="far-shoulder"||
                    part=="near-arm"||part=="far-arm"))continue;
                wait.SampleAxes(0,i,out _,out _,out var rest);
                if(rest.x>.0001f&&rest.y>.0001f)plates[i]=new Plate{part=i,rest=rest,leg=leg};
            }
        }
        internal static RegisteredLimbVolume From(FairyPoseLibraryV33 lib) {
            switch(lib.rigName){
                case "NUI_RAVEN_CLONE":
                case "NUI_NASOD_SPRAY":
                case "NUI_NASOD_SPRAY_ICE":
                case "NUI_NASOD_ELEMENT_ICE_SOLO":
                case "NUI_NASOD_ELITE_GUARD":
                case "NUI_NASOD_ELITE_SHERIFF":
                case "NUI_NASOD_RAIL":
                case "NUI_NASOD_GUARDIAN_SMALL":
                case "NUI_WALLY_9TH_TYPE2":return new RegisteredLimbVolume(lib);
                default:return null;
            }
        }
        static float ProjectionConfidence(Plate p,float x,float y) {
            float a=x/p.rest.x,b=y/p.rest.y;
            float major=Mathf.Max(a,b);
            // A plate can contract on both screen axes during a turn. Its aspect
            // ratio alone would then look normal even as the leg shrinks to a dot.
            return Mathf.Min(a,b)/Mathf.Max(p.leg?1f:.00001f,major);
        }
        float[] StableAngles(Plate p,FairyPoseTrackV33 t) {
            var angles=new float[t.count];int previous=-1;
            for(int i=0;i<t.count;i++){
                int k=(i*plates.Length+p.part)*5;
                angles[i]=t.values[k+2];
                if(ProjectionConfidence(p,t.values[k+3],t.values[k+4])<p.MinimumConfidence)continue;
                for(int j=previous+1;j<i;j++)angles[j]=previous<0?angles[i]:Mathf.LerpAngle(angles[previous],angles[i],(float)(j-previous)/(i-previous));
                previous=i;
            }
            if(previous>=0)for(int j=previous+1;j<t.count;j++)angles[j]=angles[previous];
            if(!p.leg)return angles;
            // Resolve ambiguous edge-on samples before measuring changes of direction.
            // Use the motion's sampling rate so 24 Hz and 60 Hz poses turn consistently.
            for(int i=0;i<t.count;i++){
                int k=(i*plates.Length+p.part)*5;
                float confidence=ProjectionConfidence(p,t.values[k+3],t.values[k+4]);
                angles[i]=Mathf.LerpAngle(t.values[k+2],angles[i],Mathf.InverseLerp(p.MinimumConfidence,.20f,confidence));
            }
            return SmoothIndependentFlips(t,angles);
        }
        float[] SmoothIndependentFlips(FairyPoseTrackV33 track,float[] angles){
            int count=angles.Length;
            var suspect=new bool[count];
            float limit=Mathf.Max(40,2100/track.fps);
            int radius=Mathf.Max(2,Mathf.CeilToInt(.10f*track.fps));
            bool found=false;
            for(int i=1;i<count;i++){
                float turn=Mathf.DeltaAngle(angles[i-1],angles[i]);
                float bodyTurn=0;
                if(body>=0){
                    int k=(i*plates.Length+body)*5+2;
                    bodyTurn=Mathf.DeltaAngle(track.values[k-plates.Length*5],track.values[k]);
                }
                angles[i]=angles[i-1]+turn;
                if(Mathf.Abs(turn)<=limit||Mathf.Abs(Mathf.DeltaAngle(bodyTurn,turn))<=limit)continue;
                // A whole-body roll remains intact. Limit filtering to the narrow
                // neighborhood of a plate that actually snaps relative to its body.
                found=true;
                for(int j=Mathf.Max(0,i-radius);j<=Mathf.Min(count-1,i+radius);j++)suspect[j]=true;
            }
            if(!found)return angles;
            var result=(float[])angles.Clone();
            // Retain both end poses for transitions and gait loops.
            for(int i=1;i<count-1;i++){
                if(!suspect[i])continue;
                float sum=0,weights=0;
                for(int j=-radius;j<=radius;j++){
                    float weight=radius+1-Mathf.Abs(j);
                    sum+=angles[Mathf.Clamp(i+j,0,count-1)]*weight;weights+=weight;
                }
                result[i]=sum/weights;
            }
            return result;
        }
        internal void Fit(int i,FairyPoseTrackV33 track,float clock,ref float angle,ref Vector2 scale) {
            var p=plates[i];
            if(p==null||track.animation.StartsWith("Wait",StringComparison.OrdinalIgnoreCase)||scale==Vector2.zero)return;
            float confidence=ProjectionConfidence(p,scale.x,scale.y);
            float floor=p.MinimumConfidence;
            if(confidence<floor){
                float major=Mathf.Min(1.20f,Mathf.Max(scale.x/p.rest.x,scale.y/p.rest.y));
                float minimum=floor*(p.leg?Mathf.Max(1,major):major);
                scale.x=Mathf.Max(scale.x,minimum*p.rest.x);scale.y=Mathf.Max(scale.y,minimum*p.rest.y);
            }else if(!p.leg)return;
            if(!p.angles.TryGetValue(track,out var stable))p.angles.Add(track,stable=StableAngles(p,track));
            int first=track.Segment(clock,out int next,out float phase);
            float aim=Mathf.LerpAngle(stable[first],stable[next],phase);
            angle=p.leg?aim:Mathf.LerpAngle(angle,aim,Mathf.InverseLerp(floor,.20f,confidence));
        }
    }
}
