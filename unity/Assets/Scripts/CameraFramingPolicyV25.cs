using System;

namespace ElswordPixel {
    // Display-only world-pixel calculations, shared by gameplay and CPU tests.
    public static class CameraFramingPolicyV25 {
        public const float Width=640,Height=360,Aspect=16f/9,Padding=18,ContextRadius=128,RoomOverscan=64,MaximumContextDrop=160,MaximumGameplayHeight=450;
        public struct Surface {public float x1,y1,x2,y2;public Surface(float a,float b,float c,float d){x1=a;y1=b;x2=c;y2=d;}}
        public struct Point {public float x,y;public Point(float a,float b){x=a;y=b;}}
        public struct Frame {public float x,y,width,height,safeMinY,safeMaxY,lowerUsed,padding;public bool contextLimited,geometryLimited,geometryConflict;}
        static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
        static float Clamp(float v,float low,float high){return Math.Max(low,Math.Min(high,v));}
        static float Sample(Surface[] surfaces,float x,float ceiling,float fallback,float minimum){
            float result=float.NegativeInfinity;
            if(surfaces!=null)foreach(var s in surfaces){
                if(s.x2<=s.x1||x<s.x1-.02f||x>s.x2+.02f)continue;
                float y=s.y1+(s.y2-s.y1)*(x-s.x1)/(s.x2-s.x1);
                if(Finite(y)&&y>=minimum&&y<=ceiling+.02f)result=Math.Max(result,y);
            }
            return float.IsNegativeInfinity(result)?fallback:result;
        }
        public static float ContextFloor(float playerY,float physicalFloor){
            if(!Finite(playerY)||!Finite(physicalFloor))throw new ArgumentException("Camera support height invalid");
            return Math.Max(physicalFloor,playerY-MaximumContextDrop);
        }
        public static float LowerContext(Surface[] surfaces,float playerX,float floor,float roomWidth){
            if(!Finite(playerX)||!Finite(floor)||!Finite(roomWidth)||roomWidth<=0)throw new ArgumentException("Camera terrain inputs invalid");
            // Prefer the next support directly below this platform. Only when
            // absent, look nearby for the closest lower layer, never the deepest.
            float minimum=floor-MaximumContextDrop;
            float direct=Sample(surfaces,playerX,floor-8,float.NegativeInfinity,minimum);
            if(!float.IsNegativeInfinity(direct))return direct;
            float nearby=float.NegativeInfinity;
            for(int i=-2;i<=2;i++)if(i!=0)nearby=Math.Max(nearby,Sample(surfaces,Clamp(playerX+i*ContextRadius*.5f,0,roomWidth),floor-8,float.NegativeInfinity,minimum));
            return float.IsNegativeInfinity(nearby)?floor-24:nearby;
        }
        // An orthographic view can already see a lower terrace before the actor
        // is directly above it. Bring it into frame over the approach instead
        // of changing the view when a single column first hits the terrace.
        public static float PreviewLanding(Surface[] surfaces,float x,float y,float ceiling,float lower){
            if(surfaces==null)return lower;
            float result=lower;
            foreach(var s in surfaces){
                if(s.x2<=s.x1)continue;
                float probe=Clamp(x,s.x1,s.x2),distance=Math.Abs(x-probe);
                if(distance>=ContextRadius*1.5f)continue;
                float height=s.y1+(s.y2-s.y1)*(probe-s.x1)/(s.x2-s.x1);
                if(height>ceiling||y-height>ActualLowerMaximumDropV28||height>=lower)continue;
                // Leave half a context radius at full strength for the camera
                // to settle before the player walks or dashes over the edge.
                float t=Clamp(1.5f-distance/ContextRadius,0,1);t=t*t*(3-2*t);
                result=Math.Min(result,lower+(height-lower)*t);
            }
            return result;
        }
        // V28 candidate: only the delivered21-stage scope may request this extra fit.
        // The unstarted14-stage catalog tail retains the existing camera behavior.
        public const float ActualLowerMaximumDropV28=300,ActualLowerMaximumHeightV28=720;
        public static bool ActualLowerScopeV28(string stageId){
            switch(stageId){
                case "tree-1":case "tree-2":case "tree-3":
                case "ruins-1":case "ruins-2":case "ruins-3":
                case "swamp-1":case "swamp-2":case "swamp-3":
                case "forest-1":case "forest-2":case "forest-3":
                case "cave-1":case "cave-2":case "cave-3":
                case "waterway-1":case "waterway-2":case "waterway-3":
                case "outskirts-1":case "outskirts-2":case "outskirts-3":return true;
            }
            return false;
        }
        public static bool TryActualSupportBelowV28(Surface[] surfaces,float playerX,float ceiling,out float y,out int surfaceIndex){
            if(!Finite(playerX)||!Finite(ceiling))throw new ArgumentException("Actual lower support inputs invalid");
            y=float.NegativeInfinity;surfaceIndex=-1;
            if(surfaces!=null)for(int i=0;i<surfaces.Length;i++){
                var s=surfaces[i];if(s.x2<=s.x1||playerX<s.x1-.02f||playerX>s.x2+.02f)continue;
                float candidate=s.y1+(s.y2-s.y1)*(playerX-s.x1)/(s.x2-s.x1);
                // Closest real support directly under X wins before any nearby
                // layer. Never create a floor-24 surrogate when this exists.
                if(Finite(candidate)&&candidate<=ceiling+.02f&&candidate>y){y=candidate;surfaceIndex=i;}
            }
            return surfaceIndex>=0;
        }
        public static float EffectiveMaximumHeightV28(float requestedMaximum,float smoothedHeight){
            if(!Finite(requestedMaximum)||!Finite(smoothedHeight)||requestedMaximum<Height||requestedMaximum>ActualLowerMaximumHeightV28||smoothedHeight<Height)
                throw new ArgumentException("Adaptive return height invalid");
            // The target may return to450 immediately; its current smoothed
            // height must remain admissible until the gradual return finishes.
            return Math.Max(requestedMaximum,Math.Min(ActualLowerMaximumHeightV28,smoothedHeight));
        }
        static void EdgeAt(Point a,Point b,float x,ref float low,ref float high){
            if(x<Math.Min(a.x,b.x)-.01f||x>Math.Max(a.x,b.x)+.01f)return;
            if(Math.Abs(b.x-a.x)<.0001f){if(Math.Abs(x-a.x)<.01f){low=Math.Min(low,Math.Min(a.y,b.y));high=Math.Max(high,Math.Max(a.y,b.y));}return;}
            float y=a.y+(b.y-a.y)*(x-a.x)/(b.x-a.x);low=Math.Min(low,y);high=Math.Max(high,y);
        }
        public static float GeometryBottomAt(Point a,Point b,Point c,float x,float floor){
            // Local broad mesh surfaces only; a tiny decorative point must not
            // define the camera envelope. Deep unrelated floors are excluded.
            if(Math.Max(a.x,Math.Max(b.x,c.x))-Math.Min(a.x,Math.Min(b.x,c.x))<24)return float.PositiveInfinity;
            float low=float.PositiveInfinity,high=float.NegativeInfinity;
            EdgeAt(a,b,x,ref low,ref high);EdgeAt(b,c,x,ref low,ref high);EdgeAt(c,a,x,ref low,ref high);
            if(float.IsInfinity(low)||high<floor-MaximumContextDrop-32||low>floor+32)return float.PositiveInfinity;
            return Math.Max(low,floor-MaximumContextDrop);
        }
        public static Frame FitGameplay(float playerX,float roomWidth,float heroBottom,float heroTop,float requestedLower,float bottomHudFraction,float topHudFraction,float geometryBottom=float.NaN,float minimumHeight=Height,float maximumHeight=MaximumGameplayHeight,bool requireActualLower=false,float aspect=Aspect){
            if(!Finite(playerX)||!Finite(roomWidth)||roomWidth<=0||!Finite(heroBottom)||!Finite(heroTop)||heroTop<heroBottom||!Finite(requestedLower)||
               !Finite(bottomHudFraction)||!Finite(topHudFraction)||bottomHudFraction<0||topHudFraction<0||bottomHudFraction+topHudFraction>=.85f||!Finite(minimumHeight)||!Finite(maximumHeight)||maximumHeight<Height||maximumHeight>ActualLowerMaximumHeightV28)throw new ArgumentException("Gameplay camera inputs invalid");
            float usable=1-bottomHudFraction-topHudFraction;
            float requested=Math.Min(heroBottom,requestedLower);
            float required=(heroTop-requested+2*Padding)/usable;
            float height=Clamp(Math.Max(minimumHeight,(float)Math.Ceiling(required/30)*30),Height,maximumHeight),width=height*aspect;
            // On a very crowded touch HUD, reduce only padding before allowing
            // any hero clipping or another runaway zoom-out.
            float padding=Math.Min(Padding,Math.Max(0,(height*usable-(heroTop-heroBottom))*.5f));
            if(heroTop-heroBottom>height*usable+.01f)throw new ArgumentException("Hero cannot fit the visible HUD interval");
            float lower=Math.Max(requested,heroTop+2*padding-height*usable);
            float low=heroTop+padding-height*.5f+topHudFraction*height;
            float heroHigh=heroBottom-padding+height*.5f-bottomHudFraction*height;
            float high=Math.Min(heroHigh,lower-padding+height*.5f-bottomHudFraction*height);
            bool geometryLimited=false,geometryConflict=false;
            if(Finite(geometryBottom)){
                // Allow only the actual HUD strip plus padding below the local
                // mesh envelope. Never push the hero underneath the HUD.
                float wanted=geometryBottom+height*.5f-bottomHudFraction*height-padding;
                // A proven actual landing is mandatory in this explicit fit.
                // A conflicting decorative mesh envelope must be reported; it
                // cannot silently lift the landing back under the HUD.
                float geometryHigh=requireActualLower?high:heroHigh;
                geometryConflict=wanted>geometryHigh+.01f;
                float bounded=Clamp(wanted,low,geometryHigh);
                geometryLimited=bounded>high+.01f;low=Math.Max(low,bounded);high=Math.Max(high,low);
            }
            float used=Math.Max(requested,high-height*.5f+bottomHudFraction*height+padding);
            float left=width*.5f-RoomOverscan,right=roomWidth-width*.5f+RoomOverscan;
            return new Frame{x=left<=right?Clamp(playerX,left,right):roomWidth*.5f,y=high,width=width,height=height,safeMinY=low,safeMaxY=high,
                             lowerUsed=used,padding=padding,contextLimited=used>requested+.1f,geometryLimited=geometryLimited,geometryConflict=geometryConflict};
        }
        public static Frame Fit(float playerX,float roomWidth,float bottom,float top,float bottomHudFraction,float topHudFraction,float minimumHeight=Height,float aspect=Aspect){
            if(!Finite(playerX)||!Finite(roomWidth)||roomWidth<=0||!Finite(bottom)||!Finite(top)||top<bottom||!Finite(minimumHeight)||minimumHeight<Height||
               !Finite(bottomHudFraction)||!Finite(topHudFraction)||bottomHudFraction<0||topHudFraction<0||bottomHudFraction+topHudFraction>=.85f)throw new ArgumentException("Camera framing inputs invalid");
            float required=(top-bottom+2*Padding)/(1-bottomHudFraction-topHudFraction);
            float height=Math.Max(minimumHeight,(float)Math.Ceiling(required/90)*90),width=height*aspect;
            float low=top+Padding-height*.5f+topHudFraction*height;
            float high=bottom-Padding+height*.5f-bottomHudFraction*height;
            float left=width*.5f-RoomOverscan,right=roomWidth-width*.5f+RoomOverscan;
            return new Frame{x=left<=right?Clamp(playerX,left,right):roomWidth*.5f,y=(low+high)*.5f,width=width,height=height,safeMinY=low,safeMaxY=high};
        }
        public static float PerspectiveFov(float sourceFov,float height){
            if(!Finite(sourceFov)||sourceFov<=0||sourceFov>=179||!Finite(height)||height<270)throw new ArgumentException("Camera lens inputs invalid");
            return (float)(2*Math.Atan(Math.Tan(sourceFov*Math.PI/360)*(height/270))*180/Math.PI);
        }
    }
}
