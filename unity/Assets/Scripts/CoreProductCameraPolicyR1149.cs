using System;

namespace ElswordPixel {
    // Ordinary product display policy only. All dimensions are current world
    // pixels or measured screen pixels; there is no model or Unity dependency.
    public static class CoreProductCameraPolicyR1149 {
        // Ordinary Core uses its unchanged 480x270 Point target. A 270-world-
        // pixel view therefore samples one render texel per logical pixel.
        // Ordinary rooms keep270. Boss combat targets300 with a hard330 cap;
        // large Boss bodies may be cropped at their unchanged original scale.
        public const float MinimumGameplayHeightR1266=270,MaximumGameplayHeightR1266=330;
        // The ordinary view scale is a room choice. It never depends on an
        // animation frame, nearest actor or a temporarily visible HUD label.
        // The final room stays near the hero instead of fitting King/Plant whole.
        public static float StableGameplayHeightR1417(int room){
            if(room<0||room>=5)throw new ArgumentOutOfRangeException("room");
            return room==4?300:MinimumGameplayHeightR1266;
        }
        static float ClampR1266(float v,float low,float high){return Math.Max(low,Math.Min(high,v));}
        static CameraFramingPolicyV25.Frame FitLocalGameplayR1266(Input value,float heroBottom,float heroTop,float bottom,float top){
            float usable=1-bottom-top,padding=CameraFramingPolicyV25.Padding;
            float requested=Math.Min(heroBottom,value.lowerContext);
            float height=ClampR1266(value.minimumHeight,MinimumGameplayHeightR1266,MaximumGameplayHeightR1266);
            padding=Math.Min(padding,Math.Max(0,(height*usable-(heroTop-heroBottom))*.5f));
            if(heroTop-heroBottom>height*usable+.01f)throw new ArgumentException("Hero cannot fit measured Core HUD interval");
            float lower=Math.Max(requested,heroTop+2*padding-height*usable);
            float low=heroTop+padding-height*.5f+top*height;
            float heroHigh=heroBottom-padding+height*.5f-bottom*height;
            float high=Math.Min(heroHigh,lower-padding+height*.5f-bottom*height);
            bool geometryLimited=false,geometryConflict=false;
            if(value.hasGeometry){
                float wanted=value.geometryBottom+height*.5f-bottom*height-padding;
                geometryConflict=wanted>heroHigh+.01f;
                float bounded=ClampR1266(wanted,low,heroHigh);
                geometryLimited=bounded>high+.01f;low=Math.Max(low,bounded);high=Math.Max(high,low);
            }
            float used=Math.Max(requested,high-height*.5f+bottom*height+padding);
            float width=height*(value.aspect>0?value.aspect:CameraFramingPolicyV25.Aspect);
            float left=width*.5f-CameraFramingPolicyV25.RoomOverscan,right=value.roomWidth-width*.5f+CameraFramingPolicyV25.RoomOverscan;
            return new CameraFramingPolicyV25.Frame{x=left<=right?ClampR1266(value.playerX,left,right):value.roomWidth*.5f,y=high,width=width,height=height,safeMinY=low,safeMaxY=high,
                lowerUsed=used,padding=padding,contextLimited=used>requested+.1f,geometryLimited=geometryLimited,geometryConflict=geometryConflict};
        }
        public struct Input {
            public float aspect,playerX,roomWidth,heroBottom,heroTop,lowerContext;
            public float pixelBottom,pixelHeight,hudLowerTop,hudUpperBottom;
            public bool hasGeometry,hasLocalActor;
            public float geometryBottom,actorBottom,actorTop,minimumHeight;
        }
        public struct Result {
            public CameraFramingPolicyV25.Frame frame;
            public float bottomHudFraction,topHudFraction;
            public bool localActorIncluded,localActorLimited;
        }
        static bool Finite(float v){return !float.IsNaN(v)&&!float.IsInfinity(v);}
        public static bool TryFit(Input value,out Result result,out string reason){
            result=new Result();reason="";
            if(!Finite(value.pixelBottom)||!Finite(value.pixelHeight)||value.pixelHeight<=0||
               !Finite(value.hudLowerTop)||!Finite(value.hudUpperBottom)||
               value.hudLowerTop<value.pixelBottom||value.hudUpperBottom>value.pixelBottom+value.pixelHeight||
               value.hudUpperBottom<=value.hudLowerTop){reason="measured-HUD-interval-unavailable";return false;}
            float bottom=(value.hudLowerTop-value.pixelBottom)/value.pixelHeight;
            float top=(value.pixelBottom+value.pixelHeight-value.hudUpperBottom)/value.pixelHeight;
            // V25's own feasibility boundary is preserved; a missing/crowded
            // layout never falls back to invented HUD percentages.
            if(bottom+top>=.85f){reason="measured-HUD-interval-too-crowded";return false;}
            if(!Finite(value.playerX)||!Finite(value.roomWidth)||value.roomWidth<=0||
               !Finite(value.heroBottom)||!Finite(value.heroTop)||value.heroTop<value.heroBottom||
               !Finite(value.lowerContext)||!Finite(value.minimumHeight)||
               value.minimumHeight<MinimumGameplayHeightR1266||value.minimumHeight>MaximumGameplayHeightR1266||
               value.hasGeometry&&!Finite(value.geometryBottom)){
                reason="nonfinite-or-inverted-display-input";return false;
            }
            float fitBottom=value.heroBottom,fitTop=value.heroTop;
            bool included=false,limited=false;
            if(value.hasLocalActor){
                if(!Finite(value.actorBottom)||!Finite(value.actorTop)||value.actorTop<value.actorBottom){reason="invalid-local-renderer-bounds";return false;}
                float candidateBottom=Math.Min(fitBottom,value.actorBottom),candidateTop=Math.Max(fitTop,value.actorTop);
                // V25 may reduce its own padding when a complete local body
                // fits the measured interval at the normal gameplay cap.
                float available=MaximumGameplayHeightR1266*(1-bottom-top);
                if(candidateTop-candidateBottom<=available){fitBottom=candidateBottom;fitTop=candidateTop;included=true;}
                else limited=true; // A huge Boss is cropped, never shrunk into a whole-body fit.
            }
            try{
                result.frame=FitLocalGameplayR1266(value,fitBottom,fitTop,bottom,top);
            }catch(ArgumentException){reason="V25-gameplay-envelope-cannot-fit-measured-HUD";return false;}
            result.bottomHudFraction=bottom;result.topHudFraction=top;
            result.localActorIncluded=included;result.localActorLimited=limited;
            return true;
        }
    }
}
