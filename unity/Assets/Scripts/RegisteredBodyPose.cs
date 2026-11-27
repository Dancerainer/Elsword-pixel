using System;
using UnityEngine;

namespace ElswordPixel {
    // These robots use center-registered armor paintings with independent source
    // matrices. Keep their body together as it steps and turns.
    internal sealed class RegisteredBodyPose {
        readonly FairyPoseLibraryV33 lib;
        readonly int leftBoot,rightBoot,head,chest,belt;
        readonly float restFloor,restBearing,restLength,leftRest,rightRest,leftHeight,rightHeight;
        readonly Vector2 chestScale,beltScale;
        readonly float chestTurn,beltTurn;
        readonly GripSocket[] weapons;

        internal struct Stride {
            internal bool active;
            internal float rise,leftTurn,rightTurn;
        }

        sealed class GripSocket {
            internal int hand;
            internal float turn,bearing,reach;
            internal Vector2 scale,hilt;
        }

        internal static RegisteredBodyPose For(FairyPoseLibraryV33 lib) {
            switch(lib.rigName) {
                case "NUI_RAVEN_CLONE":case "NUI_NASOD_SPRAY":case "NUI_NASOD_SPRAY_ICE":
                case "NUI_NASOD_GUARD":case "NUI_NASOD_ELITE_GUARD":
                case "NUI_NASOD_ELITE_SHERIFF":case "NUI_WALLY_9TH_TYPE2":
                    return new RegisteredBodyPose(lib);
                default:return null;
            }
        }
        RegisteredBodyPose(FairyPoseLibraryV33 lib) {
            this.lib=lib;
            leftBoot=Array.FindIndex(lib.parts,p=>p.name.EndsWith("-boot",StringComparison.Ordinal));
            rightBoot=Array.FindIndex(lib.parts,leftBoot+1,p=>p.name.EndsWith("-boot",StringComparison.Ordinal));
            var idle=lib.Find("Wait");
            if(leftBoot<0||rightBoot<0||idle==null)throw new InvalidOperationException("Missing grounded shoes for "+lib.rigName);
            idle.SampleAxes(0,leftBoot,out var lp,out leftRest,out var ls);
            idle.SampleAxes(0,rightBoot,out var rp,out rightRest,out var rs);
            float leftFloor=SoleEdge(leftBoot,lp,leftRest,ls,false),rightFloor=SoleEdge(rightBoot,rp,rightRest,rs,false);
            leftHeight=SoleEdge(leftBoot,lp,leftRest,ls,true)-leftFloor;
            rightHeight=SoleEdge(rightBoot,rp,rightRest,rs,true)-rightFloor;
            restFloor=Mathf.Min(leftFloor,rightFloor);
            if(lib.rigName=="NUI_WALLY_9TH_TYPE2"||lib.rigName=="NUI_RAVEN_CLONE"){
                weapons=new GripSocket[lib.parts.Length];
                if(lib.rigName=="NUI_RAVEN_CLONE")AddWeapon(idle,"claw","claw-fore");
                else {AddWeapon(idle,"far-blade","far-grip");AddWeapon(idle,"near-blade","near-grip");}
            }
            head=Array.FindIndex(lib.parts,p=>p.name=="head");
            chest=Array.FindIndex(lib.parts,p=>p.name=="torso");
            belt=Array.FindIndex(lib.parts,p=>p.name=="waist");
            if(head<0||chest<0||belt<0||lib.rigName=="NUI_NASOD_GUARD") {chest=belt=-1;return;}
            idle.SampleAxes(0,head,out var top,out _,out _);
            idle.SampleAxes(0,belt,out var basePoint,out beltTurn,out beltScale);
            idle.SampleAxes(0,chest,out _,out chestTurn,out chestScale);
            var spine=top-basePoint;restBearing=Bearing(spine);restLength=spine.magnitude;
            if(restLength<.01f||chestScale.x<=0||chestScale.y<=0||beltScale.x<=0||beltScale.y<=0)
                throw new InvalidOperationException("Missing standing armor sockets for "+lib.rigName);
        }
        static float Bearing(Vector2 direction){return Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;}
        static Vector2 Rotate(Vector2 v,float degrees){
            float a=degrees*Mathf.Deg2Rad,s=Mathf.Sin(a),c=Mathf.Cos(a);
            return new Vector2(c*v.x-s*v.y,s*v.x+c*v.y);
        }
        void AddWeapon(FairyPoseTrackV33 rest,string itemName,string handName){
            int item=Array.FindIndex(lib.parts,p=>p.name==itemName),hand=Array.FindIndex(lib.parts,p=>p.name==handName);
            if(item<0||hand<0)throw new InvalidOperationException("Missing grip for "+lib.rigName);
            rest.SampleAxes(0,item,out var at,out float turn,out var scale);
            rest.SampleAxes(0,hand,out var grip,out _,out _);
            Vector2 ray=grip-at;
            if(scale.x<=0||scale.y<=0||ray.sqrMagnitude<.01f)return;
            var local=Rotate(ray,-turn);
            weapons[item]=new GripSocket{hand=hand,turn=turn,bearing=Bearing(ray),reach=ray.magnitude,
                scale=scale,hilt=new Vector2(local.x/scale.x,local.y/scale.y)};
        }
        float SoleEdge(int part,Vector2 at,float turn,Vector2 axes,bool top){
            var paint=lib.parts[part];var hull=paint.visibleHull;
            float angle=-turn*Mathf.Deg2Rad;
            float x=Mathf.Sin(angle)*axes.x*paint.scale*(paint.mirrorX?-1:1),y=Mathf.Cos(angle)*axes.y*paint.scale;
            float sign=top?-1:1,value=float.PositiveInfinity;
            for(int j=0;j<hull.Length;j+=2)value=Mathf.Min(value,sign*(hull[j]*x+hull[j+1]*y));
            return (lib.foot[1]-at.y+sign*value)*lib.UnitsPerPixel;
        }
        static float GroundedRoll(float source,float rest,float lift,float shoeHeight){
            // A boot can roll onto its heel or toe while the other leg swings,
            // but a projection-axis reversal must not make it point backwards.
            float tilt=Mathf.Asin(Mathf.Sin((source-rest)*Mathf.Deg2Rad))*Mathf.Rad2Deg;
            float range=Mathf.Lerp(34,66,Mathf.Clamp01(lift/Mathf.Max(1,shoeHeight)));
            return rest+Mathf.Clamp(tilt,-range,range);
        }
        internal Stride Step(FairyPoseTrackV33 track,float clock){
            bool walk=track.animation=="Walk";
            bool ravenDash=lib.rigName=="NUI_RAVEN_CLONE"&&track.animation=="Dash";
            if(!walk&&!ravenDash)return default;
            track.SampleAxes(clock,leftBoot,out var lp,out var la,out var ls);
            track.SampleAxes(clock,rightBoot,out var rp,out var ra,out var rs);
            float left=SoleEdge(leftBoot,lp,la,ls,false),right=SoleEdge(rightBoot,rp,ra,rs,false);
            var step=new Stride{active=true,leftTurn=GroundedRoll(la,leftRest,Mathf.Max(0,left-right),leftHeight),
                rightTurn=GroundedRoll(ra,rightRest,Mathf.Max(0,right-left),rightHeight)};
            left=SoleEdge(leftBoot,lp,step.leftTurn,ls,false);right=SoleEdge(rightBoot,rp,step.rightTurn,rs,false);
            float shift=restFloor-Mathf.Min(left,right);
            // During a dash both feet may legitimately leave the ground.
            step.rise=walk?shift:Mathf.Max(0,shift);
            return step;
        }
        internal void Fit(int part,FairyPoseTrackV33 track,float clock,Vector2 at,Stride step,ref float turn,ref Vector2 axes){
            if(step.active&&(part==leftBoot||part==rightBoot)){turn=part==leftBoot?step.leftTurn:step.rightTurn;return;}
            if(track.animation=="Wait"||axes==Vector2.zero)return;
            if(weapons!=null&&weapons[part]!=null){
                FitGrip(weapons[part],track,clock,at,ref turn,ref axes);return;
            }
            if(part!=chest&&part!=belt)return;
            Vector2 rest=part==chest?chestScale:beltScale;
            float x=axes.x/rest.x,y=axes.y/rest.y;
            float quality=Mathf.Min(x,y)/Mathf.Max(1,Mathf.Max(x,y));
            if(quality>=.85f)return;
            track.SampleAxes(clock,head,out var top,out _,out _);
            track.SampleAxes(clock,belt,out var hip,out _,out _);
            var spine=top-hip;
            if(spine.sqrMagnitude<.0001f)return;
            float blend=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.23f,.85f,quality));
            float length=Mathf.Clamp(spine.magnitude/restLength,.85f,1.2f);
            // The rigid panel follows the shoulder-to-pelvis turn. This keeps
            // the belt and neck sockets occupied even during a side-on impact.
            float aim=(part==chest?chestTurn:beltTurn)+Mathf.DeltaAngle(restBearing,Bearing(spine));
            turn=Mathf.LerpAngle(turn,aim,blend);
            axes=Vector2.Lerp(axes,rest*length,blend);
        }
        static void FitGrip(GripSocket socket,FairyPoseTrackV33 track,float clock,Vector2 at,ref float turn,ref Vector2 axes){
            float x=axes.x/socket.scale.x,y=axes.y/socket.scale.y;
            float confidence=Mathf.Min(x,y)/Mathf.Max(1,Mathf.Max(x,y));
            if(confidence>=.8f)return;
            track.SampleAxes(clock,socket.hand,out var grip,out _,out _);
            Vector2 ray=grip-at;
            if(ray.sqrMagnitude<.001f)return;
            var projected=Rotate(Vector2.Scale(axes,socket.hilt),turn);
            float miss=(projected-ray).magnitude/socket.reach;
            if(miss<=.25f)return;
            // A blade may shorten when pointed at the camera. Keep its authored
            // center, and restore the painted hilt to the hand only if it pulls away.
            float blend=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.25f,.65f,miss))*
                (1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.3f,.8f,confidence)));
            float length=Mathf.Clamp(ray.magnitude/socket.reach,.12f,1.35f);
            turn=Mathf.LerpAngle(turn,socket.turn+Mathf.DeltaAngle(socket.bearing,Bearing(ray)),blend);
            axes=Vector2.Lerp(axes,socket.scale*length,blend);
        }
    }
}
