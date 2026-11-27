using UnityEngine;

namespace ElswordPixel {
    internal static class ThiefLegPose {
        // Horizontal authored pixel tiles, from the hip/knee to the knee/boot.
        // The short rounded ends overlap so a bent joint still shows trouser fabric.
        static readonly string[] thigh = {
            "..111111.......",
            ".1234433111....",
            "12344444333111.",
            "123544433343331",
            "123444333433321",
            "123433334333321",
            "123333343332221",
            "123333333322211",
            ".1233333322211.",
            ".12233222211...",
            "..12222211.....",
            "...11111......."
        };
        static readonly string[] shin = {
            "..1111111......",
            ".233333333111..",
            "333433333333211",
            "333343333333321",
            "333333333333221",
            "233333332222211",
            ".1222222222111.",
            "...11111111...."
        };
        // Deep cloth edge, shadow, midtone, lit plane and a small raised fold.
        static readonly int[] greenFront = { 0x263119,0x33441e,0x435925,0x59712d,0x6e8338 };
        static readonly int[] greenRear = { 0x202a1a,0x2c391c,0x374921,0x465925,0x56692b };
        static readonly int[] redFront = { 0x421e26,0x60242c,0x802d36,0x9d3d44,0xb05154 };
        static readonly int[] redRear = { 0x351b24,0x4b2129,0x672731,0x80343d,0x98434c };
        static readonly int[] leatherFront = { 0x2c221c,0x403024,0x53402c,0x675039,0x81674b };
        static readonly int[] leatherRear = { 0x251e19,0x322820,0x423326,0x50402f,0x62503c };
        static readonly string[] greave = {
            "...111111......",
            ".234433333111..",
            "234443322333211",
            "233333323333321",
            "223442233333221",
            "222333332222211",
            ".1222222222111.",
            "...11111111...."
        };

        internal static bool UsesRig(string id) {
            return id=="NUI_THIEF_THIN_ZERO" || id=="NUI_THIEF_THIN" || id=="NUI_SOLDIER_THIN";
        }
        internal static bool UsesPart(string id,int part) {
            return UsesRig(id) && (part==0 || part==1 || part==7 || part==8);
        }
        internal static void PaintPart(string id,int index,FairyPartLayoutV33 layout,Color32[] atlas,
                Vector2Int origin,int stride) {
            bool upper=index==0||index==7,near=index>=7,green=id=="NUI_THIEF_THIN_ZERO";
            bool soldier=id=="NUI_SOLDIER_THIN";
            var rows=upper?thigh:soldier?greave:shin;
            var colors=soldier?(near?leatherFront:leatherRear):green?(near?greenFront:greenRear):(near?redFront:redRear);
            int width=layout.rect[2],height=layout.rect[3],tileWidth=rows[0].Length;
            int left=(width-tileWidth)/2,top=(height-rows.Length)/2;
            float pivotX=left+3.5f,pivotY=top+rows.Length*.5f;
            layout.pivot=new[]{pivotX,pivotY};
            float x0=left-pivotX,y0=pivotY-top-rows.Length;
            float x1=x0+tileWidth,y1=y0+rows.Length;
            layout.visibleHull=new[]{x0,y0,x1,y0,x1,y1,x0,y1};
            for(int y=0;y<height;y++) {
                int row=y-top;
                int dst=(origin.y+height-y-1)*stride+origin.x;
                for(int x=0;x<width;x++) {
                    int col=x-left;
                    char ink=row>=0&&row<rows.Length&&col>=0&&col<tileWidth?rows[row][col]:'.';
                    if(ink=='.'){atlas[dst+x]=default;continue;}
                    int rgb=colors[ink-'1'];
                    atlas[dst+x]=new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255);
                }
            }
        }

        // The boot has its own affine projection and a separate contact track.
        // Solve against its final painted pivot, in the carrier's unmirrored space.
        internal static Vector2 Bend(Vector2 hip,Vector2 nativeKnee,Vector2 ankle,Vector2 front,float height) {
            var span=ankle-hip;float distance=span.magnitude;
            var direction=distance>.001f?span/distance:new Vector2(front.y,-front.x);
            // An upright thief bends his knee forward. When the foot folds up to
            // the belt, the spine supplies the facing rather than a near-zero leg.
            var bend=new Vector2(-direction.y,direction.x);
            float nearHip=Mathf.SmoothStep(0,1,Mathf.InverseLerp(height*.14f,height*.04f,distance));
            bend=Vector2.Lerp(bend,front,nearHip);
            var source=nativeKnee-hip;
            float along=Mathf.Clamp(Vector2.Dot(source,direction),.23f*distance,.73f*distance);
            float bow=Mathf.Abs(Vector2.Dot(source,new Vector2(-direction.y,direction.x)));
            // Keep the native bend magnitude; only a nearly edge-on segment
            // needs extra room so it doesn't turn into a flat strip.
            float shorter=Mathf.Min(along,distance-along),minLength=height*.096f;
            float roomForFold=Mathf.Sqrt(Mathf.Max(0,minLength*minLength-shorter*shorter));
            bow=Mathf.Clamp(bow,roomForFold,height*.20f);
            return hip+direction*along+bend*bow;
        }
        // Filter both ends of the leg: averaging just the thigh would move the
        // sudden rotation to the shin. These samples depend only on the motion
        // clock, so loading mid-swing or skipping a render tick uses the same pose.
        internal static void Smooth(Vector2[] hips,Vector2[] ankles,Vector2[] knees,BendersDepthTrackV36 track) {
            int n=knees.Length,radius=Mathf.Clamp(Mathf.RoundToInt(track.fps/30f),1,4);
            bool cyclic=track.motion=="Dash"||track.motion=="Walk";
            var next=new Vector2[n];
            for(int pass=0;pass<2;pass++) {
                for(int i=0;i<n;i++) {
                    Vector2 up=Vector2.zero,low=Vector2.zero;
                    for(int d=-radius;d<=radius;d++) {
                        int j=i+d;
                        if(cyclic && n>radius+1){if(j<0)j+=n-1;else if(j>=n)j-=n-1;}
                        j=Mathf.Clamp(j,0,n-1);
                        float weight=radius+1-Mathf.Abs(d);
                        up+=weight*(knees[j]-hips[j]).normalized;
                        low+=weight*(ankles[j]-knees[j]).normalized;
                    }
                    var u=hips[i]+up.normalized*(knees[i]-hips[i]).magnitude;
                    var l=ankles[i]-low.normalized*(ankles[i]-knees[i]).magnitude;
                    var k=Vector2.Lerp(u,l,.65f);
                    var span=ankles[i]-hips[i];
                    var normal=new Vector2(-span.y,span.x).normalized;
                    next[i]=k-normal*Mathf.Min(0,Vector2.Dot(k-hips[i],normal));
                }
                System.Array.Copy(next,knees,n);
            }
        }
        internal static void Fit(Transform[] axes,int first,float height,float unitsPerPixel,Vector2 knee) {
            var upper=axes[first];var lower=axes[first+1];var boot=axes[first+2];
            Vector2 hip=upper.localPosition,ankle=boot.localPosition;
            float width=height*(first==7?.163f:.150f);
            Place(upper,hip,knee,unitsPerPixel,width,thigh.Length);
            Place(lower,knee,ankle,unitsPerPixel,width*.72f,shin.Length);
        }
        static void Place(Transform axis,Vector2 root,Vector2 end,float units,float width,int tileHeight) {
            const float jointSpan=9; // tile pivot x=3.5, next joint at x=12.5
            var delta=end-root;
            axis.localPosition=root;
            axis.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
            axis.localScale=new Vector3(delta.magnitude/(jointSpan*units),width/(tileHeight*units),1);
        }
    }
}
