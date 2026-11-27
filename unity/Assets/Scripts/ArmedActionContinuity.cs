using System;
using UnityEngine;

namespace ElswordPixel {
    // The source actions are separate poses. Bridge the upper-body pose only at
    // a state change; ordinary samples and the damaging weapon arc use the
    // authored clock. Each enemy owns its own transition history.
    internal sealed class ArmedActionContinuity {
        struct Part {
            public Vector2 at;
            public float angle;
            public Vector3 scale;
        }
        readonly Transform[] anchors;
        readonly Part[] last,from,target,shown,turnFrom,upright;
        bool pivoting,hasUpright;
        string previousClip;
        int previousFacing;
        float previousTime,started,duration;

        internal ArmedActionContinuity(SpriteRenderer[] members,Transform[] axes) {
            anchors=new Transform[members.Length];
            for(int i=0;i<members.Length;i++)anchors[i]=axes[i] ? axes[i] : members[i].transform;
            last=new Part[members.Length];from=new Part[members.Length];
            target=new Part[members.Length];shown=new Part[members.Length];
            turnFrom=new Part[members.Length];upright=new Part[members.Length];
        }
        static bool Upper(int i){return (i>=3 && i<=6) || i>=10;}
        static bool Gait(string alias){return alias=="Dash"||alias=="Walk";}
        static bool Strike(string alias){return alias.EndsWith("_Fire",StringComparison.Ordinal);}
        static bool Linked(string alias){
            return alias!=null && (alias=="Wait" || Gait(alias) || Strike(alias) || alias.EndsWith("_Ready",StringComparison.Ordinal));
        }
        static Vector2 Rotate(Vector2 v,float degrees){
            float rad=degrees*Mathf.Deg2Rad,s=Mathf.Sin(rad),c=Mathf.Cos(rad);
            return new Vector2(v.x*c-v.y*s,v.x*s+v.y*c);
        }
        static float Heading(Vector2 v){return Mathf.Atan2(v.y,v.x)*Mathf.Rad2Deg;}
        static Vector2 Polar(Vector2 a,Vector2 b,float phase) {
            if(a.sqrMagnitude<.01f||b.sqrMagnitude<.01f)return Vector2.Lerp(a,b,phase);
            return Rotate(Vector2.right,Mathf.LerpAngle(Heading(a),Heading(b),phase))*
                Mathf.Lerp(a.magnitude,b.magnitude,phase);
        }
        internal void Apply(BendersFamilyRigV36 rig,FairyAuthoredPartsV33.Frame frame,float now) {
            if(!frame.visible){previousClip=null;duration=0;return;}
            for(int i=3;i<anchors.Length;i++){
                if(!Upper(i))continue;
                Transform t=anchors[i];
                target[i]=new Part{at=t.localPosition,angle=t.localEulerAngles.z,scale=t.localScale};
            }
            bool loading=LoadPose(rig,frame);
            bool continuous=previousClip!=null && now>=previousTime && now-previousTime<.17f;
            bool turning=previousFacing!=frame.facing;
            if(!continuous){duration=0;pivoting=false;}
            bool finishingOldFace=pivoting && now-started<duration*.5f && Linked(frame.alias);
            if(continuous && (turning||previousClip!=frame.alias&&!finishingOldFace)){
                duration=Linked(previousClip)&&Linked(frame.alias)?(Strike(frame.alias)?.070f:turning?.19f:.15f):0;
                pivoting=duration>0 && turning && last[6].scale.y>=0 && !Strike(frame.alias);
                if(duration>0){
                    Array.Copy(last,from,last.Length);started=now;
                    // Convert the last visible shoulder, elbow and grip into the
                    // new facing's coordinates. The hand sweeps around the body
                    // instead of teleporting across it as the actor root mirrors.
                    if(turning){
                        if(pivoting){
                            Array.Copy(last,turnFrom,last.Length);
                            if(!hasUpright)ReadUprightPose(rig);
                        }
                        ReflectPreviousPose();
                    }
                }
            }
            Array.Copy(target,shown,shown.Length);
            bool blending=duration>0 && now-started<duration;
            if(blending)Bridge(Mathf.SmoothStep(0,1,(now-started)/duration),Strike(frame.alias));
            if(blending || loading){
                for(int i=3;i<anchors.Length;i++){
                    if(!Upper(i))continue;
                    var t=anchors[i];var pose=shown[i];
                    t.localPosition=pose.at;t.localRotation=Quaternion.Euler(0,0,pose.angle);t.localScale=pose.scale;
                }
            }
            Array.Copy(shown,last,shown.Length);
            previousClip=frame.alias;previousFacing=frame.facing;previousTime=now;
        }
        void ReadUprightPose(BendersFamilyRigV36 rig) {
            var wait=rig.lib.Find("Wait");
            for(int i=3;i<upright.Length;i++){
                if(!Upper(i))continue;
                wait.SampleAxes(0,i,out var position,out float angle,out var axes);
                upright[i]=new Part{
                    at=new Vector2(position.x-rig.lib.foot[0],rig.lib.foot[1]-position.y)*rig.lib.UnitsPerPixel,
                    angle=-angle,scale=new Vector3(axes.x*rig.lib.parts[i].scale,axes.y*rig.lib.parts[i].scale,1)
                };
            }
            Vector2 waist=upright[6].at;
            float lean=Mathf.Clamp(Mathf.DeltaAngle(Heading(upright[10].at-waist),90),-30,30);
            for(int i=3;i<upright.Length;i++){
                if(!Upper(i))continue;
                upright[i].at=waist+Rotate(upright[i].at-waist,lean);upright[i].angle+=lean;
            }
            hasUpright=true;
        }
        void ReflectPreviousPose() {
            // The pictures are packed at different angles. Align the actual
            // shoulder-to-elbow and waist-to-neck spans when a facing changes;
            // a "zero degrees means upright" assumption turns some torso art
            // upside down while leaving its pivot in the right place.
            for(int i=3;i<from.Length;i++){
                if(!Upper(i))continue;
                Vector2 axis;
                switch(i){
                    case 3: axis=from[4].at-from[3].at;break;
                    case 11:axis=from[12].at-from[11].at;break;
                    case 12:axis=from[13].at-from[12].at;break;
                    case 5:case 6:case 10:axis=from[10].at-from[6].at;break;
                    default:axis=Rotate(Vector2.right,from[i].angle);break;
                }
                if((i==5||i==6||i==10) && from[i].scale.y<0){
                    from[i].angle=180-from[i].angle;from[i].scale.y=-from[i].scale.y;
                }else if(axis.sqrMagnitude>.001f)from[i].angle+=Vector2.SignedAngle(axis,new Vector2(-axis.x,axis.y));
            }
            for(int i=3;i<from.Length;i++)if(Upper(i))from[i].at.x=-from[i].at.x;
        }
        bool LoadPose(BendersFamilyRigV36 rig,FairyAuthoredPartsV33.Frame frame) {
            // These three long wind-ups reach their drawn-back pose midway and
            // hold it. Settle the chest, shoulders, head and held weapon onto
            // the back leg together, returning to the exact strike-ready pose.
            if(!ThiefLegPose.UsesRig(rig.npc) || !frame.alias.EndsWith("_Ready",StringComparison.Ordinal))return false;
            var clip=rig.lib.Find(frame.alias);
            if(clip.duration<1.2f)return false;
            float phase=frame.sourceClock/clip.duration;
            if(phase<=.42f||phase>=.99f)return false;
            float load=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.42f,.72f,phase))*
                       (1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.79f,.99f,phase)));
            float tilt=5.5f*load;
            Vector2 pelvis=target[6].at;
            Vector2 far=anchors[2].localPosition,near=anchors[9].localPosition;
            Vector2 support=far.x<near.x ? far : near;
            if(Mathf.Abs(far.y-near.y)>rig.height*.13f)support=far.y<near.y ? far : near;
            Vector2 shift=load*new Vector2(Mathf.Clamp((support.x-pelvis.x)*.24f,-rig.height*.05f,rig.height*.05f),
                                          -rig.height*.035f);
            BraceLeg(rig,0,shift);
            BraceLeg(rig,7,shift);
            for(int i=3;i<target.Length;i++){
                if(!Upper(i))continue;
                target[i].at=pelvis+shift+Rotate(target[i].at-pelvis,tilt);
                target[i].angle+=i==6?tilt*.35f:i==10?tilt*.78f:tilt;
            }
            return true;
        }
        void BraceLeg(BendersFamilyRigV36 rig,int first,Vector2 shift) {
            Vector2 hip=anchors[first].localPosition,knee=anchors[first+1].localPosition;
            Vector2 sole=anchors[first+2].localPosition;
            float thigh=(knee-hip).magnitude,shin=(sole-knee).magnitude;
            Vector2 moved=hip+shift,span=sole-moved;
            float length=span.magnitude;
            if(length<.001f)return;
            Vector2 along=span/length,across=new Vector2(-along.y,along.x);
            float reach=Mathf.Clamp((length*length+thigh*thigh-shin*shin)/(2*length),-thigh,thigh);
            float bow=Mathf.Sqrt(Mathf.Max(0,thigh*thigh-reach*reach));
            if(Vector2.Dot(knee-hip,across)<0)bow=-bow;
            anchors[first].localPosition=moved;
            ThiefLegPose.Fit(anchors,first,rig.height,rig.lib.UnitsPerPixel,moved+along*reach+across*bow);
        }
        // Mix the offsets in the parent's frame. Interpolating the wrist and
        // elbow independently in world space can collapse an arm through the
        // chest when the club switches between a front and rear carry.
        void Bridge(float t,bool liveStrike) {
            for(int i=3;i<target.Length;i++){
                if(!Upper(i))continue;
                shown[i].angle=Mathf.LerpAngle(from[i].angle,target[i].angle,t);
                shown[i].scale=Vector3.Lerp(from[i].scale,target[i].scale,t);
            }
            // The waist follows the actor's current footwork.
            if(pivoting)TurnThroughStanding(t);
            else {
                Offset(5,6,t,false);Offset(10,5,t,false);
                Offset(3,5,t,false);Offset(11,5,t,false);
            }
            Offset(4,3,t,true);Offset(12,11,t,true);
            if(liveStrike){shown[13]=target[13];if(target.Length>14)shown[14]=target[14];}
            else {
                Offset(13,12,t,true);
                if(target.Length>14)Offset(14,12,t,true);
            }
            AimJoined(3,4,t);AimJoined(11,12,t);AimJoined(12,13,t);
        }
        void TurnThroughStanding(float t) {
            // Lean up from the previous stride in its own facing. Exchange the
            // side-view drawing at the upright pose, then lean into the next
            // stride. A paper-plane half-turn would tip the head towards the
            // floor. Each drawing keeps its full width all the way through.
            bool oldSide=t<.5f;
            var a=oldSide?turnFrom:upright;var b=oldSide?upright:target;
            float phase=Mathf.SmoothStep(0,1,oldSide?t*2:(t-.5f)*2);
            Vector2 waist=target[6].at;
            for(int i=3;i<=11;i++){
                if(i!=3&&i!=5&&i!=6&&i!=10&&i!=11)continue;
                Vector2 offset=Vector2.Lerp(a[i].at-a[6].at,b[i].at-b[6].at,phase);
                if(oldSide)offset.x=-offset.x;
                shown[i].at=waist+offset;
                if(i==3||i==11)continue; // Their pictures follow the elbow/hand arc.
                float angle=Mathf.LerpAngle(a[i].angle,b[i].angle,phase);
                var scale=Vector3.Lerp(a[i].scale,b[i].scale,phase);
                if(oldSide){angle=180-angle;scale.y=-scale.y;}
                shown[i].angle=angle;shown[i].scale=scale;
            }
        }
        void Offset(int part,int parent,float t,bool joint) {
            Vector2 a=Rotate(from[part].at-from[parent].at,-from[parent].angle);
            Vector2 b=Rotate(target[part].at-target[parent].at,-target[parent].angle);
            var v=joint?Polar(a,b,t):Vector2.Lerp(a,b,t);
            shown[part].at=shown[parent].at+Rotate(v,shown[parent].angle);
        }
        void AimJoined(int part,int end,float t) {
            Vector2 a=from[end].at-from[part].at,b=target[end].at-target[part].at;
            Vector2 v=shown[end].at-shown[part].at;
            if(a.sqrMagnitude<.01f||b.sqrMagnitude<.01f||v.sqrMagnitude<.01f)return;
            float bias=Mathf.LerpAngle(from[part].angle-Heading(a),target[part].angle-Heading(b),t);
            shown[part].angle=Heading(v)+bias;
            // The near forearm's painted grip must reach the live weapon even
            // when a strike starts during the transition.
            if(part==12){
                var scale=shown[part].scale;
                scale.x=v.magnitude*Mathf.Lerp(from[part].scale.x/a.magnitude,target[part].scale.x/b.magnitude,t);
                shown[part].scale=scale;
            }
        }
    }
}
