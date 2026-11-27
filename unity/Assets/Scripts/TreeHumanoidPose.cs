using System;
using UnityEngine;

namespace ElswordPixel {
    // A side-view skeleton, built once from the standing costume. Every joint
    // rotates rigid painted segments; source 3D foreshortening never scales them.
    internal sealed class TreeHumanoidPose {
        readonly BendersFamilyRigV36 rig;
        readonly SpriteRenderer[] members;
        readonly Transform[] joints;
        readonly Vector2[] rest;
        readonly Vector3[] scale;
        readonly float[] angle;
        readonly float[] pose=new float[9],from=new float[9],last=new float[9];
        string previous;float changed;
        internal static bool Uses(string npc){return npc=="NUI_THIEF_THIN_ZERO";}
        internal TreeHumanoidPose(BendersFamilyRigV36 rig,SpriteRenderer[] members,Transform[] axes) {
            this.rig=rig;this.members=members;int n=members.Length;
            joints=new Transform[n];rest=new Vector2[n];scale=new Vector3[n];angle=new float[n];
            for(int i=0;i<n;i++) {
                joints[i]=axes[i]?axes[i]:members[i].transform;
                rest[i]=joints[i].localPosition;scale[i]=joints[i].localScale;angle[i]=joints[i].localEulerAngles.z;
            }
        }
        static float Bearing(Vector2 p){return Mathf.Atan2(p.y,p.x)*Mathf.Rad2Deg;}
        static Vector2 Rotate(Vector2 p,float degrees){float a=degrees*Mathf.Deg2Rad,c=Mathf.Cos(a),s=Mathf.Sin(a);return new Vector2(c*p.x-s*p.y,s*p.x+c*p.y);}
        static float Ease(float t){return Mathf.SmoothStep(0,1,Mathf.Clamp01(t));}
        internal void Apply(FairyAuthoredPartsV33.Frame frame,float time) {
            Array.Clear(pose,0,pose.Length);
            string clip=frame.alias;float duration=rig.lib.Find(clip).duration,t=Mathf.Clamp01(frame.sourceClock/duration),h=rig.height;
            bool gait=clip==rig.gaitMotion||clip=="Dash"||clip=="Walk";
            if(gait) {
                float phase=t*Mathf.PI*2;
                pose[0]=Mathf.Sin(phase)*h*.085f;pose[1]=Mathf.Max(0,Mathf.Cos(phase))*h*.055f;
                pose[2]=-pose[0];pose[3]=Mathf.Max(0,-Mathf.Cos(phase))*h*.055f;
                pose[4]=-3;pose[5]=Mathf.Sin(phase)*4;pose[7]=-Mathf.Sin(phase)*10;
            } else if(clip.Contains("Ready")) {
                float a=Ease(t);pose[4]=6*a;pose[5]=25*a;pose[6]=12*a;pose[7]=-12*a;
            } else if(clip.Contains("Fire")) {
                float swing=Ease(t/.28f),recover=Ease((t-.45f)/.35f);
                pose[4]=Mathf.Lerp(6,-14,swing)*(1-recover);
                pose[5]=Mathf.Lerp(25,-30,swing)*(1-recover);
                pose[6]=Mathf.Lerp(12,-10,swing)*(1-recover);pose[7]=20*swing*(1-recover);
            } else if(clip.Contains("Down")||clip.Contains("Dying")||clip.Contains("StandUp")) {
                float fall=clip.Contains("StandUp")?1-Ease(t):Ease(t/.65f);
                pose[8]=-88*fall;pose[5]=15*fall;pose[6]=28*fall;pose[7]=150*fall;
            } else if(clip.Contains("Damage")) {float a=1-Ease(t);pose[4]=10*a;pose[5]=12*a;pose[7]=-15*a;}
            else if(clip.Contains("Jump")){pose[1]=pose[3]=h*.025f;pose[4]=-5;}
            if(previous!=clip){Array.Copy(last,from,pose.Length);changed=time;previous=clip;}
            float blend=Ease((time-changed)/.10f);
            for(int i=0;i<pose.Length;i++){pose[i]=Mathf.Lerp(from[i],pose[i],blend);last[i]=pose[i];}
            for(int i=0;i<joints.Length;i++){joints[i].localPosition=rest[i];joints[i].localScale=scale[i];joints[i].localRotation=Quaternion.Euler(0,0,angle[i]);members[i].sortingOrder=rig.lib.parts[i].order;}
            // The pelvis remains at a fixed height. Each planted foot stays on
            // its rest sole while a two-bone solve bends a constant-length leg.
            Leg(0,pose[0],pose[1]);Leg(7,pose[2],pose[3]);
            float body=pose[4];Vector2 pivot=rest[6];
            foreach(int i in new[]{5,10})Place(i,pivot+Rotate(rest[i]-pivot,body),body);
            Arm(11,12,13,body,pose[5],pose[6]);
            Arm(3,4,-1,body,pose[7],0);
            if(joints.Length==15){members[13].enabled=frame.visible;members[14].enabled=false;}
            if(Mathf.Abs(pose[8])>.001f) {
                // Rotate the complete skeleton, then put its visible silhouette
                // on the floor. No limb is shortened during knockdown or death.
                for(int i=0;i<joints.Length;i++) {
                    joints[i].localPosition=rest[6]+Rotate((Vector2)joints[i].localPosition-rest[6],pose[8]);
                    joints[i].localRotation=Quaternion.Euler(0,0,joints[i].localEulerAngles.z+pose[8]);
                }
                float floor=float.PositiveInfinity;
                for(int i=0;i<members.Length;i++)if(members[i].enabled){
                    var hull=rig.lib.parts[i].visibleHull;var m=joints[i].parent.worldToLocalMatrix*members[i].transform.localToWorldMatrix;
                    for(int v=0;v<hull.Length;v+=2)floor=Mathf.Min(floor,m.MultiplyPoint3x4(new Vector3(hull[v]*(members[i].flipX?-1:1),hull[v+1])*rig.lib.UnitsPerPixel).y);
                }
                foreach(var joint in joints)joint.localPosition+=Vector3.down*floor;
            }
        }
        void Place(int i,Vector2 position,float turn){joints[i].localPosition=position;joints[i].localRotation=Quaternion.Euler(0,0,angle[i]+turn);}
        void Arm(int upper,int lower,int hand,float body,float shoulder,float elbow) {
            Vector2 origin=rest[6]+Rotate(rest[upper]-rest[6],body);
            float a=body+shoulder,b=a+elbow;
            Vector2 mid=origin+Rotate(rest[lower]-rest[upper],a);
            Place(upper,origin,a);Place(lower,mid,b);
            if(hand>=0)Place(hand,mid+Rotate(rest[hand]-rest[lower],b),b);
        }
        void Leg(int a,float travel,float lift) {
            int b=a+1,c=a+2;Vector2 hip=rest[a],end=rest[c]+new Vector2(travel,lift),delta=end-hip;
            float upper=(rest[b]-hip).magnitude,lower=(rest[c]-rest[b]).magnitude;
            float distance=Mathf.Clamp(delta.magnitude,Mathf.Abs(upper-lower)+.001f,(upper+lower)*.999f);
            var direction=delta.normalized;end=hip+direction*distance;
            float along=(distance*distance+upper*upper-lower*lower)/(2*distance);
            Vector2 normal=new Vector2(-direction.y,direction.x);
            float sign=Mathf.Sign(Vector2.Dot(rest[b]-hip,new Vector2(-(rest[c]-hip).y,(rest[c]-hip).x)));
            Vector2 knee=hip+direction*along+normal*sign*Mathf.Sqrt(Mathf.Max(0,upper*upper-along*along));
            Place(a,hip,Mathf.DeltaAngle(Bearing(rest[b]-hip),Bearing(knee-hip)));
            Place(b,knee,Mathf.DeltaAngle(Bearing(rest[c]-rest[b]),Bearing(end-knee)));
            Place(c,end,0);
        }
    }
}
