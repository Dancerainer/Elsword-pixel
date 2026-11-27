using UnityEngine;

namespace ElswordPixel {
    // A fixed two-bone silhouette for 1-1's Benders. Native action clocks,
    // hit volumes and movement remain owned by GameModel.
    internal sealed class TreeBendersAnatomy {
        struct Bone { internal float length,angle; internal Vector3 scale; }
        readonly Bone[] rest=new Bone[15];
        readonly Transform[] joints;
        readonly float height;
        internal TreeBendersAnatomy(BendersFamilyRigV36 rig,SpriteRenderer[] members,Transform[] axes) {
            height=rig.height;joints=new Transform[members.Length];
            for(int i=0;i<joints.Length;i++)joints[i]=axes[i]?axes[i]:members[i].transform;
            var wait=rig.lib.Find("Wait");
            wait.SampleAxes(0,4,out _,out _,out var farScale);
            rest[4]=new Bone{scale=new Vector3(farScale.x,farScale.y,1)*rig.lib.parts[4].scale};
            foreach(int i in new[]{0,1,3,7,8,11,12}) {
                wait.SampleAxes(0,i,out var a,out float rotation,out var scale);
                wait.SampleAxes(0,i+1,out var b,out _,out _);
                Vector2 delta=new Vector2(b.x-a.x,a.y-b.y)*rig.lib.UnitsPerPixel;
                rest[i]=new Bone{length=delta.magnitude,angle=-rotation-Angle(delta),
                    scale=new Vector3(scale.x,scale.y,1)*rig.lib.parts[i].scale};
            }
        }
        static float Angle(Vector2 v){return Mathf.Atan2(v.y,v.x)*Mathf.Rad2Deg;}
        internal void Apply() {
            // Boots are already projected onto the ground. Knees flex forward,
            // keeping trouser width constant instead of expanding like bellows.
            Fit(0,2,height*.17f,height*.18f,true);
            Fit(7,9,height*.17f,height*.18f,true);
            Fit(11,13,height*.19f,height*.22f,false);
            // The far hand is painted into its forearm: keep that forearm's
            // volume stable and attach its elbow to a fixed upper arm.
            Vector2 shoulder=joints[3].localPosition,elbow=joints[4].localPosition;
            var ray=elbow-shoulder;
            if(ray.sqrMagnitude>.001f){
                elbow=shoulder+ray.normalized*rest[3].length;
                Place(3,shoulder,elbow,false);joints[4].localPosition=elbow;
                joints[4].localScale=rest[4].scale;
            }
        }
        void Fit(int first,int tip,float upper,float lower,bool leg) {
            Vector2 root=joints[first].localPosition,end=joints[tip].localPosition;
            Vector2 ray=end-root;float distance=ray.magnitude;if(distance<.001f)return;
            var direction=ray/distance;
            float reach=Mathf.Min(distance,(upper+lower)*.995f);
            if(reach<distance && !leg) {
                Vector2 move=root+direction*reach-end;end+=move;
                joints[tip].localPosition=end;
                if(tip==13)joints[14].localPosition=(Vector2)joints[14].localPosition+move;
            } else if(reach<distance) {
                // Only extreme air/death poses exceed the standing reach.
                // Share that extension across both segments, with no width pulse.
                float extend=distance/((upper+lower)*.995f);upper*=extend;lower*=extend;reach=distance;
            }
            float along=(reach*reach+upper*upper-lower*lower)/(2*reach);
            float bend=Mathf.Sqrt(Mathf.Max(0,upper*upper-along*along));
            Vector2 normal=new Vector2(-direction.y,direction.x);
            if(!leg && Vector2.Dot((Vector2)joints[first+1].localPosition-root,normal)<0)normal=-normal;
            Vector2 knee=root+direction*along+normal*bend;
            Place(first,root,knee,leg);Place(first+1,knee,end,leg);
        }
        void Place(int part,Vector2 root,Vector2 end,bool leg) {
            var r=rest[part];var delta=end-root;
            joints[part].localPosition=root;
            joints[part].localRotation=Quaternion.Euler(0,0,Angle(delta)+r.angle);
            var scale=r.scale;float ratio=delta.magnitude/Mathf.Max(.001f,r.length);
            if(leg)scale.y*=ratio;else scale.x*=ratio;
            scale.z=1;joints[part].localScale=scale;
        }
    }
}
