using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] internal sealed class BendersBindingV36 {
        public string key,motion;
        public float speed,span;public bool hold;
    }
    [Serializable] internal sealed class BendersDepthTrackV36 {
        public string motion;
        public int count,fps;
        public float[] values;
        public float[] shoeFrames;
        public int contactHz;
        public float[] contacts;
        public int[] wide;
        [NonSerialized] internal readonly bool[] stabilizeShoe=new bool[2];
        [NonSerialized] internal readonly float[][] shoeAngles=new float[2][];
        [NonSerialized] internal readonly Vector2[][] knees=new Vector2[2][];
    }
    [Serializable] internal sealed class BendersDepthBankV36 {
        public string id,motionSha,poseSha;
        public BendersDepthTrackV36[] tracks;
    }
    [Serializable] internal sealed partial class BendersFamilyRigV36 {
        public string npc,kind,stage,root,pagesSha,poseSha,motionSha,depthSha,gaitMotion,equipmentPart;
        public float height;
        public int count;
        public string[] sharedPages;
        public float[] paintAxes;
        public BendersBindingV36[] states,aliases;
        internal FairyPoseLibraryV33 lib;
        internal float cullRadius;
        ArmedWeaponProjectionV37 equippedProjection;
        readonly float[] restHandedness=new float[2];
        readonly float[] restToeWidth=new float[2];
        readonly Dictionary<string,BendersBindingV36> displays=new Dictionary<string,BendersBindingV36>(StringComparer.Ordinal);
        readonly Dictionary<string,BendersBindingV36> source=new Dictionary<string,BendersBindingV36>(StringComparer.Ordinal);
        readonly Dictionary<string,BendersDepthTrackV36> depths=new Dictionary<string,BendersDepthTrackV36>(StringComparer.Ordinal);

        static void Need(bool ok,string reason){if(!ok)throw new InvalidOperationException("Original Benders V36 "+reason);}
        // Full original quads, projected shoes and equipped weapon tracks. The
        // reproducible bounds in tools/android-performance/rig_bounds.py are
        // tied to BOTH the binding and the separately pinned weapon proof.
        static float MobileCullRadius(string npc) {
            string weapon=ArmedHumanoidPinsV37.WeaponProof(npc);
            switch(BendersFamilyPinsV36.Binding(npc)){
                case "815bd5aa6e6fe352b7193844d41e14e2c9448eebfc21bc0654a41c5128c2b83c":return 343;
                case "e3c2448e454c1f4f9af63fa8bb19a4a73558a8bae9fcb2a3e2228e387e3b587b":return 218;
                case "e28f1734eb67ec334f99bc9ee69369efc3dcede45c4b6ca17cd259a091da414f":return 338;
                case "593578a09f01542b8f153850987ed2e6c0e7126fead98c169ed7b2056838df16":return 333;
                case "f7145a1c5373a2f490a0f4f2b86df85e39c52ad9be3481cea35dcd714364355e":return weapon=="00b8f7747703f99e6092d91312518bc81d51c37be901c1b5ff1d20189e6c4f3e"?231:0;
                case "bcc2c51b450c563805e8867da5216f400879b453245234d9cdadd2c9416efbd0":return weapon=="e7083668fc287db01a8f50a45ddcfdd6a9652901035b6d188ca663612118e2de"?224:0;
                case "dc626f06eb6b994d4f3c6c82450e7fcd937c2c83ba72ba89a241c50a7a4d755b":return weapon=="99a820b7280c09a78689b468324083f5044301876140bdcaa2042ec1f384f7b6"?239:0;
                case "e772167eaf451795f1c56762ac9f650a87e1dec0d0f329e29bf77ef03d7bcec5":return weapon=="fe7cb5c4f008366ebb4d019f0c023c77755d447b47ae485894dbc6e1aa67ca5c"?194:0;
                default:return 0;
            }
        }
        static string Pin(TextAsset text){
            using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(text.bytes)).Replace("-","").ToLowerInvariant();
        }
        internal static BendersFamilyRigV36 Load(string id,Action<UnityEngine.Object> destroy){
            string root=BendersFamilyPinsV36.Root(id);
            var file=Resources.Load<TextAsset>(root+"binding");
            Need(file&&Pin(file)==BendersFamilyPinsV36.Binding(id),"pinned family identity");
            var art=JsonUtility.FromJson<BendersFamilyRigV36>(file.text);
            art.cullRadius=MobileCullRadius(id);
            Need(art.npc==id&&art.root==root&&art.count==(id=="NUI_BENDERS_THIEF"||ArmedHumanoidPinsV37.Root(id)!=null?14:15)&&art.paintAxes.Length==art.count,"scope");
            foreach(var b in art.aliases)art.displays.Add(b.key,b);
            foreach(var b in art.states){Need(b.speed>0,"native animation speed");art.source.Add(b.key,b);}
            try{
                art.lib=RegisteredPartRigV35.Load(root,id,art.pagesSha,art.Validate,art.sharedPages);
                art.lib.limbs=RegisteredLimbPose.ForArmed(art.lib,art.paintAxes);
                if(art.cullRadius>0)art.cullRadius+=art.lib.limbs.CullPadding;
                var d=Resources.Load<TextAsset>(root+"depth");Need(d&&Pin(d)==art.depthSha,"source depth");
                var data=JsonUtility.FromJson<BendersDepthBankV36>(d.text);
                Need(data.id==id&&data.poseSha==art.poseSha&&data.motionSha==art.motionSha&&data.tracks.Length==art.lib.tracks.Length,"depth/pose scope");
                foreach(var t in data.tracks){
                    var tr=art.lib.Find(t.motion);
                    Need(tr!=null&&tr.fps==t.fps&&tr.count==t.count&&t.values.Length==t.count*art.count&&t.wide.Length==t.count&&t.shoeFrames.Length==12*t.count,"original member depth and foot projection "+t.motion);
                    if(t.motion==(string.IsNullOrEmpty(art.gaitMotion)?"Dash":art.gaitMotion))Need(t.contactHz==480&&t.contacts.Length==4*(Mathf.RoundToInt(tr.duration*t.contactHz)+1),"dense independent original Dash contacts");
                    art.depths.Add(t.motion,t);
                }
                var wait=art.depths["Wait"];
                for(int side=0;side<2;side++) {
                    int s=side*6;
                    var toe=new Vector2(wait.shoeFrames[s],wait.shoeFrames[s+2]);
                    art.restToeWidth[side]=toe.magnitude;
                    art.restHandedness[side]=Mathf.Sign(ShoeDeterminant(wait,0,side));
                    foreach(var t in data.tracks) {
                        if(art.ArticulatedTrousers && !art.FixLargeLegMotion(t.motion))continue;
                        float previous=ShoeDeterminant(t,0,side);
                        for(int i=1;i<t.count;i++) {
                            float current=ShoeDeterminant(t,i,side);
                            if(previous*current<0){t.stabilizeShoe[side]=true;break;}
                            if(current!=0)previous=current;
                        }
                        if(t.stabilizeShoe[side])
                            t.shoeAngles[side]=StableToeAngles(t,side,toe);
                    }
                }
                if(ArmedHumanoidPinsV37.Root(id)!=null)art.equippedProjection=ArmedWeaponProjectionV37.Load(art);
                return art;
            }catch{if(art.lib!=null)art.lib.ReleaseOwned(destroy);throw;}
        }
        void Validate(FairyPoseLibraryV33 bank){
            Need(bank.nativeYaw==0&&bank.authoredFacing==1&&bank.motionSha256==motionSha&&bank.parts.Length==count&&
                Math.Abs(bank.standingPixels*bank.UnitsPerPixel-height)<.001f,"original standing scale and body source");
            Need(bank.parts[2].name=="far-boot"&&bank.parts[9].name=="near-boot","independent native soles");
            Need(count==14?bank.parts[13].name==(string.IsNullOrEmpty(equipmentPart)?"wooden-club":equipmentPart):bank.parts[13].name=="cleaver-wide"&&bank.parts[14].name=="cleaver-edge","equipped original weapon");
            foreach(var tr in bank.tracks)Need(tr.alias==tr.animation&&tr.components==5&&tr.fps==(tr.alias==(string.IsNullOrEmpty(gaitMotion)?"Dash":gaitMotion)?120:60)&&
                tr.duration>0&&tr.count==Mathf.CeilToInt(tr.duration*tr.fps-.0001f)+1&&tr.values.Length==tr.count*count*5&&!tr.loop,"source track "+tr.alias);
            var available=new HashSet<string>(StringComparer.Ordinal);
            foreach(var tr in bank.tracks)Need(available.Add(tr.alias),"unique motion "+tr.alias);
            foreach(var b in source.Values)Need(available.Contains(b.motion),"source state "+b.key);
            foreach(var b in displays.Values)Need(available.Contains(b.motion),"actual alias "+b.key);
        }
        internal bool Frame(Enemy e,string alias,float selectedClock,float duration,bool loop,out FairyAuthoredPartsV33.Frame frame){
            frame=default;
            if(e.sourceNPC!=npc||e.kind!=kind||e.visualScale!=1||(e.facing!=1&&e.facing!=-1)||
                string.IsNullOrEmpty(alias)||float.IsNaN(selectedClock)||float.IsInfinity(selectedClock)||selectedClock<0)return false;
            string motion;float clock;
            if(OriginalReactionV45.TryPhysicalOrCommittedV45(e,out var originalHurt,out float seconds) && lib.Find(originalHurt.rendered) is FairyPoseTrackV33 hitTrack){
                motion=hitTrack.alias;
                clock=Mathf.Min(seconds,hitTrack.duration);
            }else if(displays.TryGetValue(alias,out var binding)){
                motion=binding.motion;var t=lib.Find(motion);
                float span=binding.span>0?binding.span:t.duration;
                if(Math.Abs(duration-span)>.00002f)return false;
                // This is the final selected source-motion clock. Nasod A's
                // 1.4 speed is already incorporated by the product selector.
                clock=binding.hold?0:(loop?Mathf.Repeat(selectedClock,span):Mathf.Min(selectedClock,span))*t.duration/span;
            }else if(!string.IsNullOrEmpty(e.npcState)&&source.TryGetValue(e.npcState,out var exact)){
                motion=exact.motion;
                if(e.npcTime<0||float.IsNaN(e.npcTime)||float.IsInfinity(e.npcTime))return false;
                clock=Mathf.Min(e.npcTime*exact.speed,lib.Find(motion).duration);
            }else return false;
            frame=new FairyAuthoredPartsV33.Frame{alias=motion,sourceClock=clock,facing=e.facing,visible=e.active&&!e.dormant};
            return true;
        }
        internal void Depth(FairyAuthoredPartsV33.Frame frame,SpriteRenderer[] members,float[] depth,Transform[] holders){
            var t=depths[frame.alias];float at=Mathf.Clamp(frame.sourceClock*t.fps,0,t.count-1);
            int a=Mathf.FloorToInt(at),b=Math.Min(a+1,t.count-1);
            for(int i=0;i<count;i++)depth[i]=Mathf.Lerp(t.values[a*count+i],t.values[b*count+i],at-a);
            for(int i=0;i<count;i++){
                int z=0;for(int j=0;j<count;j++)if(depth[j]>depth[i]||depth[j]==depth[i]&&j<i)z++;
                members[i].sortingOrder=z;
            }
            for(int side=0;side<2;side++){
                int n=side==0?2:9;
                ProjectBoot(t,side,a,b,at-a,members[n].transform,holders[n],lib.parts[n].visibleHull);
                if(ThiefLegPose.UsesRig(npc)) {
                    Vector2 knee=holders[n-1].localPosition;
                    if(!frame.alias.StartsWith("Wait",StringComparison.Ordinal)) {
                        if(t.knees[side]==null)t.knees[side]=BakeKnees(t,side);
                        knee=Vector2.Lerp(t.knees[side][a],t.knees[side][b],at-a);
                    }
                    ThiefLegPose.Fit(holders,n-2,height,lib.UnitsPerPixel,knee);
                }
            }
            if(ArticulatedTrousers && FixLargeLegMotion(frame.alias)) SmoothBendersKnees(frame,t,members,holders);
            if(equippedProjection!=null)equippedProjection.Apply(frame,holders[13]?holders[13]:members[13].transform,holders[12]?holders[12]:members[12].transform);
            if(count==15){bool broad=t.wide[Mathf.RoundToInt(at)]!=0;members[13].enabled=broad;members[14].enabled=!broad;}
        }
        static float ShoeDeterminant(BendersDepthTrackV36 t,int frame,int side) {
            int k=frame*12+side*6;
            return t.shoeFrames[k]*t.shoeFrames[k+3]-t.shoeFrames[k+1]*t.shoeFrames[k+2];
        }
        static float[] StableToeAngles(BendersDepthTrackV36 track,int side,Vector2 rest) {
            // A toe pointed into the camera has no reliable screen bearing.
            // Interpolate across just these samples between the visible poses.
            float threshold=rest.sqrMagnitude*.16f;
            int prev=-1;
            var angles=new float[track.count];
            for(int i=0;i<track.count;i++) {
                int k=i*12+side*6;
                var axis=new Vector2(track.shoeFrames[k],track.shoeFrames[k+2]);
                if(axis.sqrMagnitude<threshold)continue;
                float angle=Mathf.Atan2(axis.y,axis.x)*Mathf.Rad2Deg;
                for(int j=prev+1;j<i;j++)angles[j]=prev<0?angle:Mathf.LerpAngle(angles[prev],angle,(float)(j-prev)/(i-prev));
                angles[i]=angle;
                prev=i;
            }
            float last=prev>=0?angles[prev]:Mathf.Atan2(rest.y,rest.x)*Mathf.Rad2Deg;
            for(int i=prev+1;i<track.count;i++)angles[i]=last;
            return angles;
        }
        internal Vector2 SampleBootPivot(BendersDepthTrackV36 track,int side,int a,int b,float amount,float[] hull,
                out float xx,out float xy,out float yx,out float yy) {
            int first=a*12+side*6,next=b*12+side*6;
            float Entry(int k){return Mathf.Lerp(track.shoeFrames[first+k],track.shoeFrames[next+k],amount);}
            xx=Entry(0);xy=Entry(1);yx=Entry(2);yy=Entry(3);
            if(track.stabilizeShoe[side]) {
                // The source shoe is a 3D object. At the heel roll the projected
                // side axis can change sign in a single frame. A side-view boot
                // keeps one painted face while the sole below follows that contact.
                float width=Mathf.Sqrt(xx*xx+yx*yx);
                float minWidth=.4f*restToeWidth[side];
                var angles=track.shoeAngles[side];
                if(width<minWidth && angles!=null) {
                    float heading=Mathf.LerpAngle(angles[a],angles[b],amount)*Mathf.Deg2Rad;
                    xx=minWidth*Mathf.Cos(heading);yx=minWidth*Mathf.Sin(heading);
                    width=minWidth;
                }
                width=Mathf.Max(.001f,width);
                float thickness=Mathf.Clamp(Mathf.Sqrt(xy*xy+yy*yy),width*.8f,width*1.14f);
                xy=-yx/width*thickness*restHandedness[side];yy=xx/width*thickness*restHandedness[side];
            }
            float low=float.PositiveInfinity,unit=lib.UnitsPerPixel;
            for(int j=0;j<hull.Length;j+=2)low=Math.Min(low,(yx*hull[j]+yy*hull[j+1])*unit);
            double sum=0,weight=0;
            for(int j=0;j<hull.Length;j+=2){
                float x=(xx*hull[j]+xy*hull[j+1])*unit,y=(yx*hull[j]+yy*hull[j+1])*unit;
                double w=Math.Exp(-(y-low)/.22);weight+=w;sum+=w*x;
            }
            float sx=Entry(4),sy=Entry(5);
            if(track.contactHz>0){
                float time=(a+amount)/track.fps;
                int n=track.contacts.Length/4;
                float at=Mathf.Clamp(time*track.contactHz,0,n-1);
                int l=Mathf.FloorToInt(at),h=Math.Min(l+1,n-1),offset=2*side;
                sx=Mathf.Lerp(track.contacts[4*l+offset],track.contacts[4*h+offset],at-l);
                sy=Mathf.Lerp(track.contacts[4*l+offset+1],track.contacts[4*h+offset+1],at-l);
            }
            return new Vector2(sx-(float)(sum/weight),sy-low);
        }
        Vector2[] BakeKnees(BendersDepthTrackV36 track,int side) {
            var source=lib.Find(track.motion);
            int first=side==0?0:7,n=source.count;
            var hips=new Vector2[n];var ankles=new Vector2[n];var knees=new Vector2[n];
            for(int i=0;i<n;i++) {
                Vector2 Position(int part) {
                    int at=(i*count+part)*5;
                    return new Vector2(source.values[at]-lib.foot[0], lib.foot[1]-source.values[at+1])*lib.UnitsPerPixel;
                }
                hips[i]=Position(first);
                ankles[i]=SampleBootPivot(track,side,i,i,0,lib.parts[first+2].visibleHull,out _,out _,out _,out _);
                var spine=Position(10)-(Position(0)+Position(7))*.5f;
                var front=spine.sqrMagnitude>.001f?new Vector2(spine.y,-spine.x).normalized:Vector2.right;
                knees[i]=ThiefLegPose.Bend(hips[i],Position(first+1),ankles[i],front,height);
            }
            ThiefLegPose.Smooth(hips,ankles,knees,track);
            return knees;
        }
        void ProjectBoot(BendersDepthTrackV36 track,int side,int a,int b,float amount,Transform boot,Transform anchor,float[] hull){
            anchor.localPosition=SampleBootPivot(track,side,a,b,amount,hull,out float xx,out float xy,out float yx,out float yy);
            // SVD of the projected 2x2 foot map. The child keeps the source
            // sprite; two ordinary Unity transforms represent its full affine
            // projection, including genuine foreshortening and reflections.
            float angle=.5f*Mathf.Atan2(2*(xx*xy+yx*yy),xx*xx+yx*yx-xy*xy-yy*yy);
            float cx=Mathf.Cos(angle),cy=Mathf.Sin(angle);
            var major=new Vector2(xx*cx+xy*cy,yx*cx+yy*cy);
            float length=major.magnitude,minor=length>.000001f?(xx*yy-xy*yx)/length:0;
            anchor.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(major.y,major.x)*Mathf.Rad2Deg);
            anchor.localScale=new Vector3(length,minor,1);boot.localPosition=Vector3.zero;
            boot.localRotation=Quaternion.Euler(0,0,-angle*Mathf.Rad2Deg);boot.localScale=Vector3.one;
        }
        internal Transform[] CreateMemberAxes(SpriteRenderer[] members){
            var holders=new Transform[count];
            for(int i=0;i<count;i++){
                if(paintAxes[i]==0&&i!=2&&i!=9)continue;
                var item=members[i].transform;
                var axis=new GameObject("Source limb axis "+members[i].name);
                axis.layer=item.gameObject.layer;axis.transform.SetParent(item.parent,false);item.SetParent(axis.transform,false);holders[i]=axis.transform;
            }
            return holders;
        }
        internal void FinishMemberAxes(SpriteRenderer[] members,Transform[] holders){
            for(int i=0;i<count;i++){
                var axis=holders[i];if(!axis||i==2||i==9)continue;var member=members[i].transform;
                // FairyAuthoredParts supplies the sampled local triplet each
                // frame. Orient the scale axes along the actual painted limb,
                // then undo the constant paint angle on the original sprite.
                axis.localPosition=member.localPosition;axis.localRotation=member.localRotation;axis.localScale=member.localScale;
                member.localPosition=Vector3.zero;member.localScale=Vector3.one;
                member.localRotation=ThiefLegPose.UsesPart(npc,i)?Quaternion.identity:Quaternion.Euler(0,0,paintAxes[i]);
            }
        }
        internal float VisibleTop(SpriteRenderer[] members){
            float top=float.NegativeInfinity;
            for(int i=0;i<count;i++){
                var r=members[i];if(!r.enabled||!r.gameObject.activeInHierarchy)continue;
                var h=lib.parts[i].visibleHull;
                var m=r.transform.localToWorldMatrix;
                float xx=m.m10*(r.flipX?-1:1)*lib.UnitsPerPixel,yy=m.m11*lib.UnitsPerPixel;
                for(int j=0;j<h.Length;j+=2)top=Math.Max(top,m.m13+xx*h[j]+yy*h[j+1]);
            }
            return top;
        }
    }
}
