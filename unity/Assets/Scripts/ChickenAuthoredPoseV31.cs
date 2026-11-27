using System;

namespace ElswordPixel {
    // New A1 battle-chicken authoring math only. These are six reused drawings
    // with continuous local pose curves, not newly hand-drawn source frames.
    // No Unity, GameModel, timer, trajectory, contact or damage dependency.
    public static class ChickenAuthoredPoseV31 {
        public enum State { Idle, Walk, AttackReady, AttackFire, Hurt, AirUp, AirFall, Down, GetUp, Death }
        public struct PartPose {
            public float x,y,rotation,scaleX,scaleY;
            public static PartPose Rest {
                get { PartPose p=default(PartPose);p.scaleX=p.scaleY=1;return p; }
            }
        }
        public struct RigPose {
            public float x,y,rotation,scaleX,scaleY,pivotX,pivotY;
        }
        public const int PartCount=6;
        // Active candidate registration only. The original Female root17 owner
        // retains its original (576,1180) anchor and all its original bytes.
        public const float ArtFootAnchorX=576f,ArtFootAnchorY=1203f;

        // Offline-authored local roll path, every five degrees from 0 to 90.
        // Derived once from original PNG alpha>=128 unit-pixel corner hulls
        // under the fixed A1 layout and flat Sprite TRS; art floor is Y1203.
        // Key clearance is 2*sin(angle)+.01 art pixels, with a conservative
        // fixed interpolation clearance measured offline, never fitted live.
        // This is a fixed drawing curve. It does not inspect bounds or track
        // feet at runtime, and never moves the caller's projected world foot.
        static readonly double[] GroundRollY={
            0, -16.848489, -33.558828, -50.01384, -66.088292, -81.659848, -96.61, -110.824968, -124.662005, -141.148514, -156.560873, -170.781786, -183.703023, -195.226244, -227.835141, -261.704681, -309.468958, -365.911836, -419.570011
        };
        static bool Finite(float v) { return !float.IsNaN(v)&&!float.IsInfinity(v); }
        static void Need(bool ok,string why) { if(!ok)throw new ArgumentException("Chicken authored pose: "+why); }
        static double Smooth(double t) { t=Math.Max(0,Math.Min(1,t));return t*t*(3-2*t); }
        static double Lerp(double a,double b,double t) { return a+(b-a)*t; }
        static void Part(PartPose[] d,int i,double x,double y,double r,double sx=1,double sy=1) {
            d[i].x=(float)x;d[i].y=(float)y;d[i].rotation=(float)r;
            d[i].scaleX=(float)sx;d[i].scaleY=(float)sy;
        }
        static void BlendToRest(PartPose[] d,double t) {
            for(int i=0;i<PartCount;i++) {
                d[i].x=(float)Lerp(d[i].x,0,t);d[i].y=(float)Lerp(d[i].y,0,t);
                d[i].rotation=(float)Lerp(d[i].rotation,0,t);
                d[i].scaleX=(float)Lerp(d[i].scaleX,1,t);
                d[i].scaleY=(float)Lerp(d[i].scaleY,1,t);
            }
        }
        // Six alternating claw contacts follow CHICKEN_ATTACK_A's existing
        // .185.. .77 second, .1-second repeat window. The beak stays upright.
        static void ClawGuard(PartPose[] d,double lift) {
            Part(d,0,30*lift,-35*lift,8*lift);
            Part(d,1,25*lift,-22*lift,7*lift);
            Part(d,2,-12*lift,-40*lift,-48*lift);
            Part(d,3,12*lift,-40*lift,48*lift);
            Part(d,4,0,-35*lift,22*lift);
            Part(d,5,-15*lift,-25*lift,-12*lift);
        }
        static void ClawCombo(PartPose[] d,double seconds) {
            double lift=1-Smooth((seconds-.80)/.38);
            ClawGuard(d,lift);
            if(seconds<.10||seconds>.80)return;
            for(int strike=0;strike<6;strike++) {
                double contact=.185+.10*strike;
                double delta=seconds-contact;
                double reach=delta<0?Smooth((delta+.075)/.075):1-Smooth(delta/.075);
                if(reach<=0)continue;
                int foot=4+strike%2;
                // Art faces left. Lift the ankle, then rake outward with the
                // spread orange toes leading; the other foot stays tucked.
                d[foot].x-=(float)((foot==5?145:65)*reach);
                d[foot].y-=(float)((foot==5?35:5)*reach);
                d[foot].rotation+=(float)((foot==5?98:64)*reach);
                d[foot].scaleX=d[foot].scaleY=(float)(1+.35*reach);
                d[0].rotation-=(float)(3*reach);
                d[1].x+=(float)(12*reach);
            }
        }
        static RigPose GroundedRoll(RigPose rig,double degrees) {
            rig.rotation=(float)degrees;
            // Recenter the rotating drawing around the same fixed world foot;
            // this is an art-space offset, never a model/physics root update.
            rig.x=(float)(-574*Math.Sin(degrees*Math.PI/180));
            double at=degrees/5;int k=(int)at;
            rig.y=k>=18?(float)GroundRollY[18]:(float)Lerp(GroundRollY[k],GroundRollY[k+1],at-k);
            return rig;
        }

        // The original Hurt atlas lasts 2/3 s, while the live combat model
        // releases a normal Hurt at .28 s. Sample cosmetic recovery from the
        // model's existing elapsed/remaining pair, without retiming that atlas,
        // changing its selected frame or advancing another animation clock.
        // Using remaining stun also follows a legitimate changed recovery span
        // and freezes naturally when the owning model freezes both values.
        public static float HurtRecoveryPhase(float hostClock,float remainingStun) {
            Need(Finite(hostClock)&&hostClock>=0&&Finite(remainingStun)&&remainingStun>=0,
                "Finite nonnegative existing Hurt recovery clocks");
            double total=(double)hostClock+remainingStun;
            return total>0?(float)(hostClock/total):1f;
        }

        // Per-view presentation history. Read the owner's existing clock; do
        // not advance a second timer or reset the original source clip clock.
        public struct AirFallContinuity {
            bool hasPrevious;
            State previousState;
            float previousHostClock,previousPhase,fallOrigin,fallLaunchPhase;
            public float Resolve(State state,float hostClock,float sourcePhase,float sourceDuration,out float launchPhase) {
                Need((int)state>=(int)State.Idle&&(int)state<=(int)State.Death,"Known continuity state");
                Need(Finite(hostClock)&&hostClock>=0&&Finite(sourcePhase)&&sourcePhase>=0&&sourcePhase<=1,
                    "Finite existing continuity clock and phase");
                float phase=sourcePhase;launchPhase=1;
                bool sameTimeline=hasPrevious&&hostClock>=previousHostClock;
                if(state==State.AirFall) {
                    Need(Finite(sourceDuration)&&sourceDuration>0,"Positive existing AirFall span");
                    if(!sameTimeline||previousState!=State.AirFall) {
                        if(sameTimeline&&previousState==State.AirUp) {
                            // The first falling draw exactly matches the last
                            // rendered launch pose, including short launches.
                            fallOrigin=hostClock;fallLaunchPhase=previousPhase;
                        } else {
                            // If first observed already falling, preserve the
                            // existing source phase rather than invent an apex.
                            fallOrigin=hostClock-sourcePhase*sourceDuration;fallLaunchPhase=1;
                        }
                    }
                    phase=(float)Math.Max(0,Math.Min(1,((double)hostClock-fallOrigin)/sourceDuration));
                    launchPhase=fallLaunchPhase;
                }
                hasPrevious=true;previousState=state;previousHostClock=hostClock;previousPhase=sourcePhase;
                return phase;
            }
        }

        static void AirFallFromLaunch(PartPose[] destination,double phase,double launchPhase) {
            double open=Smooth(phase),tuck=Smooth(launchPhase);
            Part(destination,0,0,Lerp(-12*tuck,-4,open),Lerp(-5*tuck,4,open));
            Part(destination,1,0,Lerp(-3*tuck,0,open),Lerp(-3*tuck,2,open));
            Part(destination,2,Lerp(-8*tuck,-15,open),Lerp(-8*tuck,0,open),Lerp(-32*tuck,-20,open));
            Part(destination,3,Lerp(8*tuck,15,open),Lerp(-8*tuck,0,open),Lerp(32*tuck,20,open));
            Part(destination,4,0,Lerp(-60*tuck,-14,open),Lerp(-18*tuck,-6,open));
            Part(destination,5,0,Lerp(-60*tuck,-14,open),Lerp(18*tuck,6,open));
        }
        public static RigPose SampleAirFall(float phase,float launchPhase,PartPose[] destination) {
            Need(Finite(launchPhase)&&launchPhase>=0&&launchPhase<=1,"Finite last launch pose phase");
            var rig=Sample(State.AirFall,phase,destination);
            AirFallFromLaunch(destination,phase,launchPhase);return rig;
        }

        // Hot valid calls allocate no heap objects or arrays. The caller owns
        // exactly six destination structs and supplies the existing phase.
        // Idle/Walk accept phase1 as their identical phase0 loop seam.
        public static RigPose Sample(State state,float normalizedPhase,PartPose[] destination) {
            Need(destination!=null&&destination.Length==PartCount,"Exactly six destination parts");
            Need((int)state>=(int)State.Idle&&(int)state<=(int)State.Death,"Known state");
            Need(Finite(normalizedPhase)&&normalizedPhase>=0&&normalizedPhase<=1,"Finite existing phase0..1");
            for(int i=0;i<PartCount;i++)destination[i]=PartPose.Rest;
            RigPose rig=default(RigPose);rig.scaleX=rig.scaleY=1;
            rig.pivotX=ArtFootAnchorX;rig.pivotY=ArtFootAnchorY;
            double p=normalizedPhase;
            if((state==State.Idle||state==State.Walk)&&normalizedPhase==1)p=0;
            double phase=p*Math.PI*2,wave=Math.Sin(phase);
            switch(state) {
            case State.Idle:
                double breath=(1-Math.Cos(phase))*.5;
                Part(destination,0,0,-5*breath,.6*wave);
                Part(destination,1,0,-2*breath,0,1+.004*breath,1-.004*breath);
                Part(destination,2,0,-2*breath,2*wave);Part(destination,3,0,-2*breath,-2*wave);
                break;
            case State.Walk:
                double bounce=-9*(1-Math.Cos(phase*2))*.5;
                Part(destination,0,7*wave,bounce,2.5*wave);
                Part(destination,1,0,bounce*.65,-1.2*wave);
                Part(destination,2,0,bounce*.65,-13*wave);Part(destination,3,0,bounce*.65,13*wave);
                // The support foot stays at its fixed rest Y; the opposite
                // foot lifts. Horizontal art offsets do not move the model.
                // Squared half-waves also give zero lift velocity at each
                // footfall and the loop seam, without a vertical snap.
                double leftLift=Math.Max(0,wave),rightLift=Math.Max(0,-wave);
                Part(destination,4,28*wave,-28*leftLift*leftLift,0);
                Part(destination,5,-28*wave,-28*rightLift*rightLift,0);
                break;
            case State.AttackReady:
                ClawGuard(destination,Smooth((p-.55)/.45));
                break;
            case State.AttackFire:
                ClawCombo(destination,p*1.8);
                break;
            case State.Hurt:
                double recoil=(1-p)*(1-p);
                Part(destination,0,35*recoil,-18*recoil,13*recoil,1+.02*recoil,1-.02*recoil);
                Part(destination,1,0,3*recoil,4*recoil);
                Part(destination,2,-5*recoil,-8*recoil,28*recoil);
                Part(destination,3,5*recoil,-8*recoil,-28*recoil);
                // Rig/world translation stays zero: the original knockback,
                // protection and support trajectory remain the sole owners.
                break;
            case State.AirUp:
                double tuck=Smooth(p);
                Part(destination,0,0,-12*tuck,-5*tuck);
                Part(destination,1,0,-3*tuck,-3*tuck);
                Part(destination,2,-8*tuck,-8*tuck,-32*tuck);
                Part(destination,3,8*tuck,-8*tuck,32*tuck);
                Part(destination,4,0,-60*tuck,-18*tuck);Part(destination,5,0,-60*tuck,18*tuck);
                break;
            case State.AirFall:
                double open=Smooth(p);
                Part(destination,0,0,Lerp(-12,-4,open),Lerp(-5,4,open));
                Part(destination,1,0,Lerp(-3,0,open),Lerp(-3,2,open));
                Part(destination,2,Lerp(-8,-15,open),Lerp(-8,0,open),Lerp(-32,-20,open));
                Part(destination,3,Lerp(8,15,open),Lerp(-8,0,open),Lerp(32,20,open));
                Part(destination,4,0,Lerp(-60,-14,open),Lerp(-18,-6,open));
                Part(destination,5,0,Lerp(-60,-14,open),Lerp(18,6,open));
                break;
            case State.Down:
                rig=GroundedRoll(rig,90*Smooth(p/.32));break;
            case State.GetUp:
                // Down1 and GetUp0 share the same grounded end pose. Hold the
                // first 12% before a continuous local roll back to standing.
                rig=GroundedRoll(rig,90*(1-Smooth((p-.12)/.88)));break;
            case State.Death:
                rig=GroundedRoll(rig,90*Smooth(p/.78));break;
            }
            return rig;
        }
    }
}
