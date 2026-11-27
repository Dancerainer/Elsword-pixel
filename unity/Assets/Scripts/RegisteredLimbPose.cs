using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Fit two painted segments between the existing shoulder/hip and wrist/ankle.
    // The native projection still selects both endpoints, the weapon and foot
    // contacts. A limb pointed towards the camera bends instead of collapsing
    // a side-view painting into a thin line.
    internal sealed class RegisteredLimbPose {
        sealed class Bone {
            public int part;
            public float length, angleBias;
            public Vector2 scale;
            public void Remember(Vector2 vector, float angle, Vector2 axes) {
                float squared=vector.sqrMagnitude;
                if(squared<=length*length)return;
                length=Mathf.Sqrt(squared);
                angleBias=-angle-Bearing(vector);
                scale=axes;
            }
            public void Place(Transform member,Vector2 pivot,Vector2 tip,float weight) {
                Vector2 delta=tip-pivot;
                float ratio=delta.magnitude/length;
                member.localPosition=pivot;
                float rotation=Mathf.LerpAngle(member.localEulerAngles.z,Bearing(delta)+angleBias,weight);
                member.localRotation=Quaternion.Euler(0,0,rotation);
                var original=member.localScale;
                member.localScale=new Vector3(Mathf.Lerp(original.x,scale.x*ratio,weight),Mathf.Lerp(original.y,scale.y*Mathf.Clamp(ratio,.82f,1.12f),weight),1);
            }
        }
        sealed class Chain {
            public Bone upper,lower;
            public int end;
            public float farTip;
            public readonly Dictionary<FairyPoseTrackV33,Vector2[]> bends=new Dictionary<FairyPoseTrackV33,Vector2[]>();
        }
        readonly List<Chain> chains=new List<Chain>(4);
        readonly FairyPoseLibraryV33 library;
        static float Bearing(Vector2 v){return Mathf.Atan2(v.y,v.x)*Mathf.Rad2Deg;}
        RegisteredLimbPose(FairyPoseLibraryV33 lib){library=lib;}

        internal static RegisteredLimbPose ForArmed(FairyPoseLibraryV33 lib,float[] paintAxes) {
            var fit=new RegisteredLimbPose(lib);
            if(!ThiefLegPose.UsesRig(lib.rigName)) {
                fit.Add(0,1,2);
                fit.Add(7,8,9);
            }
            fit.Add(11,12,13);
            // The far hand is part of the forearm painting. Its tip is the
            // furthest painted texel along the registered forearm's own axis.
            float a=paintAxes[4]*Mathf.Deg2Rad,reach=0;
            var hull=lib.parts[4].visibleHull;
            for(int j=0;j<hull.Length;j+=2)reach=Mathf.Max(reach,Mathf.Cos(a)*hull[j]-Mathf.Sin(a)*hull[j+1]);
            fit.Add(3,4,-1,reach*lib.UnitsPerPixel);
            return fit;
        }
        void SourceSegments(Chain c,FairyPoseTrackV33 t,int frame,out int a,out int b,out Vector2 upper,out Vector2 lower){
            int start=frame*library.parts.Length*5;
            a=start+5*c.upper.part;b=start+5*c.lower.part;
            var v=t.values;
            upper=new Vector2(v[b]-v[a],v[a+1]-v[b+1])*library.UnitsPerPixel;
            if(c.end>=0){
                int end=start+5*c.end;
                lower=new Vector2(v[end]-v[b],v[b+1]-v[end+1])*library.UnitsPerPixel;
            }else{
                float angle=-v[b+2]*Mathf.Deg2Rad;
                lower=c.farTip*library.parts[c.lower.part].scale*v[b+3]*new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
            }
        }
        void Add(int first,int second,int tip,float farTip=0) {
            var chain=new Chain{upper=new Bone{part=first},lower=new Bone{part=second},end=tip,farTip=farTip};
            foreach(var track in library.tracks){
                if(track.components!=5)continue;
                int step=Mathf.Max(1,Mathf.RoundToInt(track.fps/15));
                for(int frame=0;frame<track.count;frame+=step){
                    SourceSegments(chain,track,frame,out int a,out int b,out var upper,out var lower);
                    var v=track.values;
                    chain.upper.Remember(upper,v[a+2],library.parts[first].scale*new Vector2(v[a+3],v[a+4]));
                    chain.lower.Remember(lower,v[b+2],library.parts[second].scale*new Vector2(v[b+3],v[b+4]));
                }
            }
            if(chain.upper.length>.01f&&chain.lower.length>.01f)chains.Add(chain);
        }

        internal static RegisteredLimbPose ForOther(FairyPoseLibraryV33 lib) {
            if(lib.rigName!="NUI_ANCIENT_PPORU"&&lib.rigName!="NUI_NASOD_GUARD")return null;
            var fit=new RegisteredLimbPose(lib);
            // In these two sources the painted segments are registered from one
            // joint to the next. The boot, paw, shield and blade are distal
            // anchors, never adjustable intermediate joints.
            switch(lib.rigName){
                case "NUI_ANCIENT_PPORU":
                    fit.Add(5,6,7);fit.Add(8,9,10);
                    fit.Add(11,12,13);fit.Add(14,15,16);
                    break;
                case "NUI_NASOD_GUARD":
                    fit.Add(0,1,13);fit.Add(6,7,14);
                    fit.Add(2,3,12);fit.Add(9,10,11);
                    break;
                default:return null;
            }
            return fit;
        }
        internal float CullPadding {
            get{float reach=0;foreach(var chain in chains)reach=Mathf.Max(reach,chain.upper.length+chain.lower.length);return 2*reach;}
        }
        static float Weight(Chain c,float upper,float lower){
            float ratio=Mathf.Min(upper/c.upper.length,lower/c.lower.length);
            return Mathf.SmoothStep(0,1,Mathf.Clamp01((.93f-ratio)/.46f));
        }
        // A projected two-bone linkage has two possible knees/elbows. Choose
        // the continuous sequence across the authored motion; choosing by the
        // cross product on each frame flips when the wrist passes the shoulder.
        Vector2[] Bake(Chain c,FairyPoseTrackV33 t){
            int n=t.count;
            var choices=new Vector2[2*n];
            var parent=new byte[2*n];var source=new Vector2[n];
            for(int i=0;i<n;i++){
                SourceSegments(c,t,i,out _,out _,out var upper,out var lower);
                var span=upper+lower;float reach=span.magnitude,ua=upper.magnitude,lb=lower.magnitude;
                float la=Mathf.Max(ua,.83f*c.upper.length),ls=Mathf.Max(lb,.83f*c.lower.length);
                if(la>ls+reach)ls=la-reach;else if(ls>la+reach)la=ls-reach;
                var direction=reach>.001f?span/reach:Vector2.right;
                float along=reach>.001f?Mathf.Clamp((reach*reach+la*la-ls*ls)/(2*reach),-la,la):0;
                var normal=new Vector2(-direction.y,direction.x)*Mathf.Sqrt(Mathf.Max(0,la*la-along*along));
                float weight=Weight(c,ua,lb);source[i]=upper;
                choices[2*i]=Vector2.Lerp(upper,direction*along+normal,weight);
                choices[2*i+1]=Vector2.Lerp(upper,direction*along-normal,weight);
            }
            bool cyclic=t.loop||t.animation=="Dash"||t.animation=="Walk";
            var output=new Vector2[n];float best=float.PositiveInfinity;
            // For looping source clocks, charge the seam just like an ordinary
            // frame transition. State changes carry no history between actors.
            for(int first=0;first<(cyclic?2:1);first++){
                var cost=new Vector2((choices[0]-source[0]).sqrMagnitude,(choices[1]-source[0]).sqrMagnitude);
                if(cyclic)cost[1-first]=float.PositiveInfinity;
                for(int i=1;i<n;i++){
                    var next=Vector2.zero;
                    for(int sign=0;sign<2;sign++){
                        int o=2*i+sign;
                        float p0=cost.x+(choices[o]-choices[2*i-2]).sqrMagnitude;
                        float p1=cost.y+(choices[o]-choices[2*i-1]).sqrMagnitude;
                        parent[o]=(byte)(p1<p0?1:0);
                        // A momentary edge-on projection should not outweigh
                        // a discontinuous turn. Scale the source cost by time.
                        next[sign]=Mathf.Min(p0,p1)+(choices[o]-source[i]).sqrMagnitude*(1.6f/t.fps);
                    }
                    cost=next;
                }
                for(int last=0;last<2;last++){
                    int sign=last;float total=cost[sign];
                    if(cyclic)total+=(choices[2*n-2+sign]-choices[first]).sqrMagnitude;
                    if(total>=best)continue;
                    best=total;
                    for(int i=n-1;i>=0;i--){int o=2*i+sign;output[i]=choices[o];sign=parent[o];}
                }
            }
            SoftenTurns(c,t,output,cyclic);
            return output;
        }
        static Vector2[] SmoothSharpTurns(Vector2[] segments,float fps,bool cyclic){
            int n=segments.Length,radius=Mathf.Clamp(Mathf.RoundToInt(fps*.05f),1,6);
            var angles=new float[n];var smoothed=new Vector2[n];
            for(int i=0;i<n;i++){
                float bearing=Bearing(segments[i]);
                angles[i]=i==0?bearing:angles[i-1]+Mathf.DeltaAngle(angles[i-1],bearing);
            }
            for(int i=0;i<n;i++){
                float sum=0,weights=0,peak=0;
                for(int offset=-radius;offset<=radius;offset++){
                    int j=i+offset;float turn=0;
                    if(j<0||j>=n){
                        if(!cyclic||n-1<radius)continue;
                        int period=n-1;
                        if(j<0){j+=period;turn=angles[0]-angles[n-1];}
                        else{j-=period;turn=angles[n-1]-angles[0];}
                    }
                    float w=radius+1-Mathf.Abs(offset);sum+=(angles[j]+turn)*w;weights+=w;
                    if(j>0)peak=Mathf.Max(peak,Mathf.Abs(angles[j]-angles[j-1]));
                }
                if(peak<=1200f/fps){smoothed[i]=segments[i];continue;}
                float angle=sum/weights*Mathf.Deg2Rad;
                smoothed[i]=segments[i].magnitude*new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
            }
            return smoothed;
        }
        void SoftenTurns(Chain c,FairyPoseTrackV33 t,Vector2[] elbows,bool cyclic){
            var wrists=new Vector2[t.count];var lower=new Vector2[t.count];
            for(int i=0;i<t.count;i++){
                SourceSegments(c,t,i,out _,out _,out var arm,out var forearm);
                wrists[i]=arm+forearm;lower[i]=wrists[i]-elbows[i];
            }
            // Establish a smooth upper-arm guide first, limiting its arc so the
            // forearm can still reach the original wrist. Filtering just one
            // segment transfers its sudden turn to the other segment.
            var firstGuide=SmoothSharpTurns(elbows,t.fps,cyclic);
            for(int i=0;i<t.count;i++){
                if(firstGuide[i]==elbows[i])continue;
                float elbowDistance=elbows[i].magnitude,wristDistance=wrists[i].magnitude;
                if(elbowDistance*wristDistance<=.0001f)continue;
                float maxReach=Mathf.Max(lower[i].magnitude,1.30f*c.lower.length);
                float arc=Mathf.Acos(Mathf.Clamp((elbowDistance*elbowDistance+wristDistance*wristDistance-maxReach*maxReach)/(2*elbowDistance*wristDistance),-1,1))*Mathf.Rad2Deg;
                float aim=Bearing(wrists[i]);
                float angle=(aim+Mathf.Clamp(Mathf.DeltaAngle(aim,Bearing(firstGuide[i])),-arc,arc))*Mathf.Deg2Rad;
                elbows[i]=elbowDistance*new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));lower[i]=wrists[i]-elbows[i];
            }
            var upperGuide=SmoothSharpTurns(elbows,t.fps,cyclic);
            var lowerGuide=SmoothSharpTurns(lower,t.fps,cyclic);
            // A short forearm turns farther for a given elbow displacement.
            // Balance the two angular errors while retaining the original wrist.
            float a=c.upper.length,b=c.lower.length;
            float w=Mathf.Clamp(a*a/(a*a+b*b),.30f,.82f);
            for(int i=0;i<t.count;i++){
                var joint=Vector2.Lerp(upperGuide[i],wrists[i]-lowerGuide[i],w);
                var forearm=wrists[i]-joint;
                float min=Mathf.Min(lower[i].magnitude,.60f*b);
                if(forearm.sqrMagnitude<min*min&&min>0){
                    var direction=forearm.sqrMagnitude>.000001f?forearm.normalized:lowerGuide[i].normalized;
                    joint=wrists[i]-direction*min;
                }
                elbows[i]=joint;
            }
        }
        // Sample the same knee position that Apply would draw at a source frame.
        // Large trouser sprites subsequently attach to a separately projected boot.
        internal Vector2 KneeAtFrame(int first,FairyPoseTrackV33 track,int frame,out Vector2 paintAngles) {
            foreach(var chain in chains){
                if(chain.upper.part!=first)continue;
                SourceSegments(chain,track,frame,out int a,out int b,out var thigh,out var shin);
                Vector2 hip=new Vector2(track.values[a]-library.foot[0],library.foot[1]-track.values[a+1])*library.UnitsPerPixel;
                paintAngles=new Vector2(-track.values[a+2],-track.values[b+2]);
                float weight=Weight(chain,thigh.magnitude,shin.magnitude);
                if(weight<=0)return hip+thigh;
                if(!chain.bends.TryGetValue(track,out var bends))chain.bends.Add(track,bends=Bake(chain,track));
                Vector2 fitted=bends[frame];
                paintAngles.x=Mathf.LerpAngle(paintAngles.x,Bearing(fitted)+chain.upper.angleBias,weight);
                paintAngles.y=Mathf.LerpAngle(paintAngles.y,Bearing(thigh+shin-fitted)+chain.lower.angleBias,weight);
                return hip+fitted;
            }
            throw new InvalidOperationException("Missing registered Benders trouser chain.");
        }
        internal void Apply(SpriteRenderer[] parts,FairyPoseTrackV33 track,float clock){
            if(track.animation.StartsWith("Wait",StringComparison.OrdinalIgnoreCase))return;
            int i=track.Segment(clock,out int next,out float fraction);
            foreach(var c in chains){
                var u=parts[c.upper.part].transform;var l=parts[c.lower.part].transform;
                Vector2 root=u.localPosition,middle=l.localPosition;
                Vector2 tip=c.end>=0?(Vector2)parts[c.end].transform.localPosition:
                    middle+(Vector2)(l.localRotation*Vector3.right)*(c.farTip*l.localScale.x);
                float weight=Weight(c,(middle-root).magnitude,(tip-middle).magnitude);
                if(weight<=0)continue;
                if(!c.bends.TryGetValue(track,out var bends))c.bends.Add(track,bends=Bake(c,track));
                Vector2 elbow=root+Vector2.Lerp(bends[i],bends[next],fraction);
                c.upper.Place(u,root,elbow,weight);c.lower.Place(l,elbow,tip,weight);
            }
        }
    }
}
