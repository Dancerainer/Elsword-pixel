#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        [Serializable] sealed class GuardWalkWitnessV36 {
            public string motionSha,name;
            public float duration;
            public int fps,count;
            public float[] values;

            public Vector2 At(float t,int side) {
                float sample=Mathf.Clamp(t*fps,0,count-1);
                int a=Mathf.FloorToInt(sample),b=Math.Min(count-1,a+1);
                int offset=side*2;
                return new Vector2(Mathf.Lerp(values[4*a+offset],values[4*b+offset],sample-a),
                    Mathf.Lerp(values[4*a+offset+1],values[4*b+offset+1],sample-a));
            }
        }
        [Serializable] sealed class GuardGaitSampleV36 {
            public int frame,facing,side,footPixels=-1;
            public string boot,image;
            public float clock,soleX,soleY,sourceX,sourceY,slipError,worldRootX,worldRootY,worldSoleY;
            public bool supporting;
        }
        [Serializable] sealed class GuardGaitReportV36 {
            public bool windowPresentationVerified=false,offscreenStandaloneGPU=true,complete;
            public int sidesWithPixelWitness;
            public List<GuardGaitSampleV36> samples=new List<GuardGaitSampleV36>();
        }
        readonly GuardGaitReportV36 gaitV36=new GuardGaitReportV36();

        GuardWalkWitnessV36 LoadWalkWitnessV36() {
            var text=Resources.Load<TextAsset>("Pixel/feedback-v35/nasod-guard/guard-foot-check");
            Need(text,"Walk native foot witnesses");
            using(var hash=SHA256.Create())Need(BitConverter.ToString(hash.ComputeHash(text.bytes)).Replace("-","").ToLowerInvariant()
                =="242365398a1cb91af38fdc592bdf570de6613d79e493d6d6b2a76ba1e4fc4922","Pinned original independent sole witness");
            var data=JsonUtility.FromJson<GuardWalkWitnessV36>(text.text);
            Need(data.name=="Walk"&&data.motionSha==GuardRigV35.OriginalMotionSha&&data.count==145&&
                data.fps==120&&data.duration==1.2f&&data.values.Length==4*data.count,"Walk native foot sample topology");
            return data;
        }
        void WriteGaitV36() {
            File.WriteAllText(Path.Combine(output,"guard-gait-feet.json"),JsonUtility.ToJson(gaitV36,true));
        }
        GuardGaitSampleV36 SoleV36(SpriteRenderer boot,FairyPartLayoutV33 part,float unit,int side,
            FairyAuthoredPartsV33.Frame frame,GuardWalkWitnessV36 witness) {
            Transform root=boot.transform.parent;
            float min=float.PositiveInfinity,worldMin=float.PositiveInfinity;
            var hull=part.visibleHull;
            var coords=new Vector2[hull.Length/2];
            for(int i=0;i<coords.Length;i++){
                var local=new Vector3((boot.flipX?-1:1)*hull[2*i],hull[2*i+1],0)*unit;
                // Inspect actual presented children. Remove inherited camera scale
                // and restore the actual facing sign to express source logical units.
                var world=boot.transform.TransformPoint(local);
                var at=root.InverseTransformPoint(world);
                coords[i]=new Vector2(at.x*frame.facing,at.y);
                min=Math.Min(min,at.y);worldMin=Math.Min(worldMin,world.y);
            }
            double weighted=0,total=0;
            foreach(var at in coords){double weight=Math.Exp(-(at.y-min)/.22);weighted+=at.x*weight;total+=weight;}
            var src=witness.At(frame.sourceClock,side);var other=witness.At(frame.sourceClock,1-side);
            var r=new GuardGaitSampleV36{frame=Time.frameCount,facing=frame.facing,side=side,boot=boot.name,
                clock=frame.sourceClock,soleX=(float)(weighted/total),soleY=min,sourceX=frame.facing*src.x,sourceY=src.y,
                supporting=src.y<=Math.Min(src.y,other.y)+.5f&&src.y<1,
                worldRootX=root.position.x,worldRootY=root.position.y,worldSoleY=worldMin};
            gaitV36.samples.Add(r);
            // The original heel may roll forward in a fraction of a frame. Check
            // the load-bearing contact and vertical contour independently.
            Need(Math.Abs(r.soleY-r.sourceY)<.18f,"Source individual shoe height "+r.boot+
                " at "+r.clock+" actual/source "+r.soleY+"/"+r.sourceY);
            if(r.supporting)Need(Math.Abs(r.soleX-r.sourceX)<1.25f,"Source individual supporting shoe x "+r.boot+
                " at "+r.clock+" actual/source "+r.soleX+"/"+r.sourceX);
            return r;
        }

        int BootPixelsV36(SpriteRenderer boot) {
            var c=World;var target=c.targetTexture;var active=RenderTexture.active;
            var rect=c.rect;float aspect=c.aspect;bool enabled=boot.enabled;
            Texture2D on=null,off=null;
            try {
                c.targetTexture=rt;c.rect=new Rect(0,0,1,1);c.aspect=(float)Width/Height;RenderTexture.active=rt;
                post=-1;callbacks.Clear();onPass=true;GL.Clear(true,true,Color.clear);c.Render();onPass=false;
                Need(post==Time.frameCount&&callbacks.Contains(boot),"Actual boot was submitted to original camera "+boot.name);
                on=Read();boot.enabled=false;GL.Clear(true,true,Color.clear);c.Render();off=Read();
                var a=on.GetPixels32();var b=off.GetPixels32();
                var lo=c.WorldToViewportPoint(boot.bounds.min);var hi=c.WorldToViewportPoint(boot.bounds.max);
                int x0=Mathf.Clamp(Mathf.FloorToInt(Math.Min(lo.x,hi.x)*Width)-2,0,Width);
                int x1=Mathf.Clamp(Mathf.CeilToInt(Math.Max(lo.x,hi.x)*Width)+2,0,Width);
                int y0=Mathf.Clamp(Mathf.FloorToInt(Math.Min(lo.y,hi.y)*Height)-2,0,Height);
                int y1=Mathf.Clamp(Mathf.CeilToInt(Math.Max(lo.y,hi.y)*Height)+2,0,Height);
                int changed=0;
                for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++){
                    int k=y*Width+x;if(a[k].r!=b[k].r||a[k].g!=b[k].g||a[k].b!=b[k].b)changed++;
                }
                return changed;
            }finally{
                boot.enabled=enabled;onPass=false;c.targetTexture=target;c.rect=rect;c.aspect=aspect;
                RenderTexture.active=active;if(on)Destroy(on);if(off)Destroy(off);
            }
        }
        IEnumerable GuardGaitSideV36(int facing,GuardWalkWitnessV36 witness) {
            StageGuard();
            var seen=new bool[8];
            var pixels=new bool[2];var support=new bool[2];var released=new bool[2];
            var previous=new GuardGaitSampleV36[2];
            float[] lastProbe={-10,-10};int bins=0;
            double end=Time.realtimeSinceStartupAsDouble+55;
            for(;;){
                Protect(facing*200);yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Independent guard gait phases, both shoes and visible pixels; facing "+
                    facing+" bins="+bins+" draws="+pixels[0]+"/"+pixels[1]+" npc="+enemy.npcState);
                if(enemy.npcState!="NASOD_GUARD_WALK"||enemy.facing!=facing)continue;
                var owners=(IDictionary)Field(scene,"guardViewsV35");if(!owners.Contains(enemy))continue;
                var view=owners[enemy];var parts=(FairyAuthoredPartsV33)Field(view,"parts");var f=parts.LastFrame;
                var lib=(FairyPoseLibraryV33)Field(scene,"guardLibraryV35");
                if(f.alias!="Walk"||!f.visible||f.facing!=facing)continue;
                var members=(SpriteRenderer[])Field(view,"members");
                Need(members.Length==15&&members[13].name=="far-boot"&&members[14].name=="near-boot","Original two independent feet");
                int bin=Math.Min(7,(int)(8*f.sourceClock/witness.duration));
                bool record=!seen[bin]||(!pixels[0]||!pixels[1])&&Math.Abs(lastProbe[0]-f.sourceClock)>.12f;
                Observation shot=null;
                if(record){
                    shot=Row("walk-"+facing+"-"+bin+"-"+Time.frameCount);Capture(shot,true);
                    Need(shot.actualParts==15&&shot.eligible==15&&shot.callbacks==15&&shot.pixels>0,"All current owner's fifteen camera draws");
                    if(!seen[bin]){seen[bin]=true;bins++;}
                }
                for(int s=0;s<2;s++){
                    var r=SoleV36(members[13+s],lib.parts[13+s],lib.UnitsPerPixel,s,f,witness);
                    if(r.supporting)support[s]=true;else released[s]=true;
                    var prev=previous[s];
                    if(prev!=null&&prev.supporting&&r.supporting&&r.clock>prev.clock&&r.clock-prev.clock<.035f){
                        r.slipError=Math.Abs((r.soleX-prev.soleX)-(r.sourceX-prev.sourceX));
                        Need(r.slipError<1.5f,"Independent supporting-foot slide "+r.boot+" "+r.slipError);
                    }
                    previous[s]=r;
                    if(shot!=null){
                        r.image=shot.image;r.footPixels=BootPixelsV36(members[13+s]);lastProbe[s]=f.sourceClock;
                        if(r.footPixels>0)pixels[s]=true;
                    }
                }
                if(shot!=null)WriteGaitV36();
                if(bins==8&&pixels[0]&&pixels[1]&&support[0]&&support[1]&&released[0]&&released[1])break;
            }
            gaitV36.sidesWithPixelWitness+=2;WriteGaitV36();
        }
        IEnumerable GuardGaitFlowV36() {
            var witness=LoadWalkWitnessV36();
            foreach(var step in Enter("altera","altera-nasod-factory"))yield return step;
            try {
                foreach(var step in GuardGaitSideV36(1,witness))yield return step;
                foreach(var step in GuardGaitSideV36(-1,witness))yield return step;
                gaitV36.complete=gaitV36.sidesWithPixelWitness==4;
                Need(gaitV36.complete,"Each foot visible in both facings");
            }finally{WriteGaitV36();}
        }
    }
}
#endif
