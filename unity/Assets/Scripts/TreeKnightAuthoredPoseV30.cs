using System;

namespace ElswordPixel {
    // Pure authoring math. The caller supplies an existing model phase; this
    // class has no timer, model reference, source trajectory or damage command.
    public static class TreeKnightAuthoredPoseV30 {
        public enum State { Reveal, Idle, Walk, Attack, Hurt, Jump, Launch, Fall, Down, GetUp, Death }
        public struct PartPose {
            public float x, y, rotation, scaleX, scaleY;
            public static PartPose Rest { get { return new PartPose { scaleX=1, scaleY=1 }; } }
        }
        public struct RigPose {
            public float x, y, rotation, scaleX, scaleY, pivotX, pivotY;
        }
        public const int PartCount=6;
        public const float ModelAttackDuration=.52f;
        public const float ModelContactClock=.40f;
        public static float CurrentModelContactPhase { get { return ModelContactClock/ModelAttackDuration; } }
        // Fixed drawing-space root paths, authored with this atlas. They keep
        // the tumble close to its ground anchor without runtime bounds fitting,
        // support queries, foot tracking or changes to the existing model.
        static readonly double[] GetUpGroundY={-184,-182.635756,-175.051506,-164.233578,-146.719835,-137.758617,-133.975501,-127.920469,-121.49529,-114.148087,-106.470276,-98.310475,-92.769014,-95.074125,-95.370777,-94.802862,-92.702751,-89.647882,-85.839637,-80.773067,-74.687331,-67.719664,-60.091997,-52.034607,-43.797729,-35.653885,-27.923556,-20.789876,-14.467983,-9.291686,-5.143733,-3.14277,0};
        static readonly double[] DeathGroundY={0,-3.19038,-5.328276,-9.69045,-15.142402,-21.784534,-29.257555,-37.34053,-45.815295,-54.319066,-62.569551,-70.288686,-77.223059,-83.140607,-87.870214,-91.295128,-94.30259,-95.031324,-98.992159,-110.682873,-121.093146,-131.534283,-141.266648,-150.321267,-158.628211,-165.838963,-173.018263,-176.905281,-193.757228,-212.831704,-225.41531,-233.857297,-235};
        static double GroundTransitionY(double[] keys,double phase) {
            double position=phase*(keys.Length-1);
            int i=Math.Min(keys.Length-2,(int)position);double t=position-i;
            double a=keys[i],b=keys[i+1];
            double ma=i==0?0:(keys[i+1]-keys[i-1])*.5;
            double mb=i+1==keys.Length-1?0:(keys[i+2]-keys[i])*.5;
            double t2=t*t,t3=t2*t;
            return (2*t3-3*t2+1)*a+(t3-2*t2+t)*ma+(-2*t3+3*t2)*b+(t3-t2)*mb;
        }
        static readonly double[] AttackSword={0,-45,-105,-105,-40,0,0};
        static readonly double[] AttackBody={0,6,14,14,4,8,0};
        static readonly double[] AttackFace={0,-3,-5,-5,2,3,0};
        static double AttackTime(int key,double contact) {
            switch(key) {
            case 0:return 0;case 1:return contact*.32;case 2:return contact*.72;
            case 3:return contact*.90;case 4:return contact;
            case 5:return contact+(1-contact)*.30;default:return 1;
            }
        }
        static bool Finite(float v) { return !float.IsNaN(v) && !float.IsInfinity(v); }
        static void Need(bool v,string why) { if(!v)throw new ArgumentException("Tree authored pose: "+why); }
        static double Smooth(double t) { t=Math.Max(0,Math.Min(1,t));return t*t*(3-2*t); }
        static double Lerp(double a,double b,double t) { return a+(b-a)*t; }
        static double Envelope(double phase) { return phase<=0||phase>=1?0:Math.Sin(Math.PI*phase); }

        public static RigPose Sample(State state,float normalizedPhase,float attackContactPhase,
            bool strikeObserved,PartPose[] destination) {
            Need(destination!=null && destination.Length==PartCount,"Exactly six output parts");
            Need((int)state>=(int)State.Reveal&&(int)state<=(int)State.Death,"Known visual state");
            Need(Finite(normalizedPhase)&&normalizedPhase>=0&&normalizedPhase<=1,"Existing phase0..1");
            Need(Finite(attackContactPhase)&&attackContactPhase>.2f&&attackContactPhase<.95f,"Explicit model contact phase");
            for(int i=0;i<PartCount;i++)destination[i]=PartPose.Rest;
            var rig=new RigPose{scaleX=1,scaleY=1,pivotX=576,pivotY=1035};
            double p=normalizedPhase;
            if((state==State.Idle||state==State.Walk)&&normalizedPhase==1)p=0;
            double wave=Math.Sin(p*Math.PI*2),opposite=Math.Sin(p*Math.PI*2+Math.PI);
            switch(state) {
            case State.Reveal:
                // The existing renderer still provides visibility, the original
                // 24-unit reveal offset and the final projected ground clip.
                // This is only the new drawing unfolding around its foot anchor.
                double unfold=Smooth(p);rig.pivotY=1192;
                rig.scaleX=(float)Lerp(1.08,1,unfold);rig.scaleY=(float)Lerp(.55,1,unfold);
                destination[0].rotation=(float)Lerp(-5,0,unfold);
                destination[2].rotation=(float)Lerp(-18,0,unfold);
                destination[3].rotation=(float)Lerp(-28,0,unfold);break;
            case State.Idle:
                float breath=(float)(wave*3);destination[0].y=breath;destination[0].rotation=(float)(wave*1.2);
                destination[1].y=breath*.45f;destination[2].rotation=(float)(wave*1.5);
                destination[3].rotation=(float)(-wave*1.8);break;
            case State.Walk:
                float bounce=(float)(-Math.Abs(wave)*7);
                destination[0].y=bounce;destination[0].rotation=(float)(wave*2);
                destination[1].y=bounce*.65f;
                destination[2].rotation=(float)(-wave*12);destination[3].rotation=(float)(wave*8);
                destination[4].x=(float)(wave*22);destination[4].y=(float)(-Math.Max(0,wave)*22);
                destination[5].x=(float)(opposite*22);destination[5].y=(float)(-Math.Max(0,opposite)*22);
                destination[4].rotation=(float)(wave*17);destination[5].rotation=(float)(opposite*17);break;
            case State.Attack:
                // The current carrier uses logicalScale1: model .40/.52 is
                // the contact phase. A future scale2 sprite warp does not alter
                // this model-time authoring contract. struck is a witness only.
                double contact=attackContactPhase;
                int key=0;while(key<5&&p>AttackTime(key+1,contact))key++;
                double blend=Smooth((p-AttackTime(key,contact))/(AttackTime(key+1,contact)-AttackTime(key,contact)));
                float dy=(float)Lerp(AttackBody[key],AttackBody[key+1],blend);
                float tilt=(float)Lerp(AttackFace[key],AttackFace[key+1],blend);
                destination[0].y=dy;destination[0].rotation=tilt;destination[1].y=dy*.6f;destination[1].rotation=tilt*.55f;
                destination[2].y=dy*.6f;destination[2].rotation=-tilt*3;
                destination[3].y=dy*.6f;destination[3].rotation=(float)Lerp(AttackSword[key],AttackSword[key+1],blend);
                destination[4].rotation=-dy*.18f;destination[5].rotation=dy*.18f;break;
            case State.Hurt:
                double recoil=Envelope(p);rig.x=(float)(-recoil*26);rig.rotation=(float)(-recoil*8);
                destination[0].rotation=(float)(-recoil*5);destination[2].rotation=(float)(recoil*20);
                destination[3].rotation=(float)(-recoil*18);break;
            case State.Jump:
                double lift=Envelope(p);destination[0].y=(float)(-lift*5);
                destination[2].rotation=(float)(-lift*22);destination[3].rotation=(float)(-lift*32);
                destination[4].y=(float)(-lift*36);destination[5].y=(float)(-lift*31);
                destination[4].rotation=(float)(-lift*18);destination[5].rotation=(float)(lift*16);break;
            case State.Launch:
                rig.rotation=(float)Lerp(0,-24,Smooth(p));destination[2].rotation=28;
                destination[3].rotation=-42;destination[4].rotation=-22;destination[5].rotation=20;break;
            case State.Fall:
                rig.rotation=(float)Lerp(-24,-8,Smooth(p));destination[2].rotation=20;
                destination[3].rotation=-24;destination[4].rotation=-12;destination[5].rotation=12;break;
            case State.Down:
                rig.rotation=90;rig.y=-184;destination[2].rotation=12;destination[3].rotation=-114;break;
            case State.GetUp:
                double rise=Smooth(p);rig.rotation=(float)Lerp(90,0,rise);rig.y=(float)GroundTransitionY(GetUpGroundY,p);
                destination[2].rotation=(float)Lerp(12,0,rise);destination[3].rotation=(float)Lerp(-114,0,rise);break;
            case State.Death:
                double settle=Smooth(p);rig.rotation=(float)Lerp(0,95,settle);rig.y=(float)GroundTransitionY(DeathGroundY,p);
                destination[2].rotation=(float)(settle*18);destination[3].rotation=(float)Lerp(0,-114,settle);break;
            }
            return rig;
        }
    }
}
