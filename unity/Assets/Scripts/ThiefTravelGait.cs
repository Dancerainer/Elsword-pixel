using UnityEngine;

namespace ElswordPixel {
    internal sealed partial class BendersFamilyRigV36 {
        ThiefTravelCycle travelCycle;
        internal ThiefTravelCycle TravelCycle {
            get {
                if(travelCycle==null)travelCycle=new ThiefTravelCycle(depths[gaitMotion]);
                return travelCycle;
            }
        }
        internal Vector2 TravelSole(FairyAuthoredPartsV33.Frame frame,int side) {
            var t=depths[frame.alias];
            if(t.contactHz>0) {
                float at=Mathf.Clamp(frame.sourceClock*t.contactHz,0,t.contacts.Length/4-1);
                int i=(int)at,j=Mathf.Min(i+1,t.contacts.Length/4-1),k=2*side;
                return Vector2.Lerp(new Vector2(t.contacts[4*i+k],t.contacts[4*i+k+1]),
                    new Vector2(t.contacts[4*j+k],t.contacts[4*j+k+1]),at-i);
            }
            float sample=Mathf.Clamp(frame.sourceClock*t.fps,0,t.count-1);
            int a=(int)sample,b=Mathf.Min(a+1,t.count-1),n=6*side+4;
            return Vector2.Lerp(new Vector2(t.shoeFrames[a*12+n],t.shoeFrames[a*12+n+1]),
                new Vector2(t.shoeFrames[b*12+n],t.shoeFrames[b*12+n+1]),sample-a);
        }
    }

    // A planted sole moves back relative to the torso as the actor travels
    // forward. Invert that distance trace to time the entire native running pose.
    internal sealed class ThiefTravelCycle {
        readonly BendersDepthTrackV36 track;
        readonly float[] distance;
        readonly int farPlant,nearPlant,count;
        internal float Length {get{return distance[count];}}
        internal ThiefTravelCycle(BendersDepthTrackV36 track) {
            this.track=track;
            count=track.contacts.Length/4-1; // Final contact repeats the first pose.
            distance=new float[count+1];
            farPlant=Forwardmost(0);nearPlant=Forwardmost(1);
            float usual=(FootRange(0)+FootRange(1))/count;
            var steps=new float[count];
            for(int i=0;i<count;i++) {
                int next=(i+1)%count,foot=SupportingFoot(next)*2;
                steps[i]=track.contacts[4*i+foot]-track.contacts[4*next+foot];
            }
            // The weighted contact can move from heel to toe in one sample.
            // A short rolling average keeps playback rate continuous through it.
            for(int i=0;i<count;i++) {
                float step=0;
                for(int j=-3;j<=3;j++)step+=(4-Mathf.Abs(j))*steps[(i+j+count)%count];
                distance[i+1]=distance[i]+Mathf.Clamp(step/16,usual*.34f,usual*2.4f);
            }
        }
        int Forwardmost(int side) {
            int best=0;for(int i=1;i<count;i++)if(track.contacts[4*i+side*2]>track.contacts[4*best+side*2])best=i;
            return best;
        }
        float FootRange(int side) {
            float lo=float.PositiveInfinity,hi=float.NegativeInfinity;
            for(int i=0;i<count;i++){float x=track.contacts[4*i+side*2];lo=Mathf.Min(lo,x);hi=Mathf.Max(hi,x);}
            return hi-lo;
        }
        int SupportingFoot(int sample) {
            return (sample-farPlant+count)%count < (nearPlant-farPlant+count)%count ? 0 : 1;
        }
        internal float Enter(float clock,Vector2 far,Vector2 near,bool match) {
            if(!match)return distance[Mathf.RoundToInt(Mathf.Repeat(clock*track.contactHz,count))%count];
            int best=0;float score=float.PositiveInfinity;
            for(int i=0;i<count;i++) {
                int k=i*4;
                float cost=(new Vector2(track.contacts[k],track.contacts[k+1])-far).sqrMagnitude+
                           (new Vector2(track.contacts[k+2],track.contacts[k+3])-near).sqrMagnitude;
                if(cost<score){score=cost;best=i;}
            }
            return distance[best];
        }
        internal float Sample(float travelled,out int foot,out Vector2 contact,out float stance) {
            float at=Mathf.Repeat(travelled,Length);
            int lo=0,hi=count;
            while(hi-lo>1){int mid=(lo+hi)/2;if(distance[mid]<=at)lo=mid;else hi=mid;}
            float t=(at-distance[lo])/(distance[hi]-distance[lo]);
            foot=SupportingFoot(lo);int side=foot*2;
            contact=Vector2.Lerp(new Vector2(track.contacts[4*lo+side],track.contacts[4*lo+side+1]),
                new Vector2(track.contacts[4*hi+side],track.contacts[4*hi+side+1]),t);
            int from=foot==0?farPlant:nearPlant,to=foot==0?nearPlant:farPlant;
            stance=Mathf.Repeat(lo+t-from,count)/((to-from+count)%count);
            return (lo+t)/track.contactHz;
        }
    }

    internal sealed class ThiefTravelView {
        struct Leg {
            internal Vector2 hip,knee,contact,scale;
            internal Quaternion shoeAxis,shoePaint;
        }
        readonly Leg[] previous=new Leg[2],departing=new Leg[2];
        Vector2 lastActor;
        Vector2 rootShift,soleCorrection;
        float travelled,lastTime,lastMove,transitionAt,transitionSpan,lastDrawTime,plantedX;
        float toeGround,freeBootLift;
        int facing,support=-1;
        bool initialized,running,saved,blending;

        static Vector2 World(Vector2 local,Vector2 actor,int face) {
            return new Vector2(actor.x+face*local.x,actor.y+local.y);
        }
        static Vector2 Local(Vector2 world,Vector2 actor,int face) {
            return new Vector2((world.x-actor.x)*face,world.y-actor.y);
        }

        internal Vector2 Prepare(BendersFamilyRigV36 bank,GameModel game,Vector2 point,bool grounded,float now,ref FairyAuthoredPartsV33.Frame frame) {
            bool gait=frame.alias==bank.gaitMotion&&grounded;
            float dx=initialized?(point.x-lastActor.x)*frame.facing:0;
            bool continuous=initialized&&now>=lastTime&&now-lastTime<.2f&&Mathf.Abs(dx)<bank.height;
            if(!continuous||Mathf.Abs(dx)>.035f)lastMove=now;
            bool active=gait&&(!continuous||now-lastMove<.18f);
            bool turn=continuous&&facing!=frame.facing;
            if(continuous&&saved&&(active!=running||turn)&&(active||running||turn)
                    && (gait || frame.alias.StartsWith("Wait",System.StringComparison.Ordinal)
                        || frame.alias.EndsWith("_Ready",System.StringComparison.Ordinal))) {
                departing[0]=previous[0];departing[1]=previous[1];transitionAt=now;transitionSpan=turn?.18f:.14f;blending=true;
            }
            if(!continuous||now-lastDrawTime>.2f)blending=false;
            var targetShift=Vector2.zero;var shiftToFoot=Vector2.zero;float weight=0;
            if(active) {
                var cycle=bank.TravelCycle;
                if(!continuous||!running||turn) {
                    travelled=cycle.Enter(frame.sourceClock,Local(previous[0].contact,point,frame.facing),
                        Local(previous[1].contact,point,frame.facing),continuous&&saved&&now-lastDrawTime<.2f);
                }else travelled=Mathf.Repeat(travelled+Mathf.Max(0,dx),cycle.Length);
                frame.sourceClock=cycle.Sample(travelled,out int foot,out var sole,out float stance);
                float nominalX=point.x+frame.facing*sole.x;
                if(!continuous||turn||!running||support!=foot)plantedX=nominalX+rootShift.x;
                support=foot;
                // Ease the new foot into weight bearing, then release it at toe-off.
                weight=Mathf.SmoothStep(0,1,Mathf.Min(stance,1-stance)/.12f)
                    *Mathf.InverseLerp(7,2,sole.y);
                float slip=Mathf.Clamp(plantedX-nominalX,-bank.height*.10f,bank.height*.10f);
                float floor=point.y;
                if(game.NativeTerrain){
                    var ground=game.SurfaceBelow(plantedX,floor+8);
                    if(ground!=null&&Mathf.Abs(ground.Y(plantedX)-floor)<9)floor=ground.Y(plantedX);
                }
                shiftToFoot=new Vector2(slip,floor-point.y-sole.y);
                if(bank.npc=="NUI_THIEF_THIN_TOTO") {
                    toeGround=floor-point.y;
                    // Both shoes in Toto's projected walk stay below the rest sole.
                    // Unweight the trailing boot between the two contact changes.
                    freeBootLift=Mathf.Sin(stance*Mathf.PI)*bank.height*.075f;
                }
                // The pelvis yields after contact and rises into toe-off. This moves
                // the original torso, head, grip and weapon together over the legs.
                float load=-Mathf.Sin(stance*2*Mathf.PI)*bank.height*.029f;
                targetShift.x=weight*slip*.30f;
                targetShift.y=Mathf.Clamp(-sole.y+load,-3.8f,3.8f);
            }else if(gait) {
                // If AI is still walking into a blocker, return its weight to
                // standing instead of spinning the cycle on the same spot.
                frame.alias="Wait";
                frame.sourceClock=Mathf.Repeat(Mathf.Max(0,now-lastMove-.18f),bank.lib.Find("Wait").duration);
            }
            if(!active)support=-1;
            rootShift=continuous?Vector2.MoveTowards(rootShift,targetShift,Mathf.Max(0,now-lastTime)*52):targetShift;
            soleCorrection=(shiftToFoot-rootShift)*weight;
            soleCorrection.x*=frame.facing;
            soleCorrection.y=Mathf.Clamp(soleCorrection.y,-4.5f,4.5f);
            initialized=true;running=active;facing=frame.facing;lastTime=now;lastActor=point;
            return rootShift;
        }
        static Vector2 Sole(Transform axis,Transform paint,float[] hull,float unit) {
            // Same contact definition as the native shoe projection, in rig units.
            var matrix=Matrix4x4.TRS(axis.localPosition,axis.localRotation,axis.localScale)*Matrix4x4.Rotate(paint.localRotation);
            float low=float.PositiveInfinity;
            for(int i=0;i<hull.Length;i+=2)low=Mathf.Min(low,(matrix.m10*hull[i]+matrix.m11*hull[i+1])*unit);
            double sum=0,weight=0;
            for(int i=0;i<hull.Length;i+=2) {
                float y=(matrix.m10*hull[i]+matrix.m11*hull[i+1])*unit;
                double w=System.Math.Exp(-(y-low)/.22);
                sum+=w*(matrix.m00*hull[i]+matrix.m01*hull[i+1])*unit;weight+=w;
            }
            return (Vector2)axis.localPosition+new Vector2((float)(sum/weight),low);
        }
        internal void AfterPose(BendersFamilyRigV36 bank,SpriteRenderer[] members,Transform[] axes,Vector2 actor,float now,FairyAuthoredPartsV33.Frame frame) {
            if(!running&&!frame.alias.StartsWith("Wait",System.StringComparison.Ordinal)
               &&!frame.alias.EndsWith("_Ready",System.StringComparison.Ordinal))blending=false;
            float t=blending?Mathf.SmoothStep(0,1,(now-transitionAt)/transitionSpan):1;
            bool originalBoots=bank.npc=="NUI_THIEF_THIN_TOTO";
            for(int side=0;side<2;side++) {
                int first=side==0?0:7,boot=first+2;
                var axis=axes[boot];var paint=members[boot].transform;var hull=bank.lib.parts[boot].visibleHull;
                Vector2 hipBefore=default,kneeBefore=default,ankleBefore=default;
                if(originalBoots) {
                    hipBefore=axes[first].localPosition;kneeBefore=axes[first+1].localPosition;ankleBefore=axis.localPosition;
                }
                var sole=bank.TravelSole(frame,side);
                if(side==support) {
                    axis.localPosition+=(Vector3)soleCorrection;
                    sole+=soleCorrection;
                    var knee=(Vector2)axes[first+1].localPosition+soleCorrection*.42f;
                    if(originalBoots)axes[first+1].localPosition=knee;
                    else ThiefLegPose.Fit(axes,first,bank.height,bank.lib.UnitsPerPixel,knee);
                }
                if(t<1) {
                    var from=departing[side];
                    var goal=Vector2.Lerp(Local(from.contact,actor,facing),sole,t);
                    axis.localRotation=Quaternion.Slerp(from.shoeAxis,axis.localRotation,t);
                    var scale=Vector2.Lerp(from.scale,axis.localScale,t);
                    axis.localScale=new Vector3(scale.x,scale.y,1);
                    paint.localRotation=Quaternion.Slerp(from.shoePaint,paint.localRotation,t);
                    axis.localPosition+=(Vector3)(goal-Sole(axis,paint,hull,bank.lib.UnitsPerPixel));
                    axes[first].localPosition=Vector2.Lerp(Local(from.hip,actor,facing),axes[first].localPosition,t);
                    var knee=Vector2.Lerp(Local(from.knee,actor,facing),axes[first+1].localPosition,t);
                    if(originalBoots)axes[first+1].localPosition=knee;
                    else ThiefLegPose.Fit(axes,first,bank.height,bank.lib.UnitsPerPixel,knee);
                    sole=goal;
                }
                if(originalBoots&&(running||t<1)) {
                    float wanted=toeGround+(side==support ? 0 : freeBootLift);
                    float raise=running?Mathf.Max(0,wanted-rootShift.y-sole.y):0;
                    axis.localPosition+=Vector3.up*raise;
                    axes[first+1].localPosition+=Vector3.up*(raise*.42f);
                    sole.y+=raise;
                    Vector2 hip=axes[first].localPosition,knee=axes[first+1].localPosition,ankle=axis.localPosition;
                    FollowTotoJoint(axes[first],kneeBefore-hipBefore,knee-hip);
                    FollowTotoJoint(axes[first+1],ankleBefore-kneeBefore,ankle-knee);
                }
                previous[side]=new Leg{hip=World(axes[first].localPosition,actor,facing),knee=World(axes[first+1].localPosition,actor,facing),contact=World(sole,actor,facing),scale=axis.localScale,shoeAxis=axis.localRotation,shoePaint=paint.localRotation};
            }
            if(t>=1)blending=false;
            saved=true;lastDrawTime=now;
        }
        static void FollowTotoJoint(Transform member,Vector2 before,Vector2 after) {
            if(before.sqrMagnitude<.01f||after.sqrMagnitude<.01f)return;
            // Toto's coat and boots have their own painted axes. Turn the existing
            // panel through the joint delta, preserving its width and registration.
            member.localRotation=Quaternion.Euler(0,0,Vector2.SignedAngle(before,after))*member.localRotation;
            var scale=member.localScale;
            scale.x*=Mathf.Clamp(after.magnitude/before.magnitude,.8f,1.25f);
            member.localScale=scale;
        }
    }
}
