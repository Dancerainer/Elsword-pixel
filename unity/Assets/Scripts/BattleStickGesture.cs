using System;

namespace ElswordPixel {
    // Shared by Unity pointer events and timestamped Android touch replay.
    // Only distance controls sprint; a second touch has no special meaning.
    internal sealed class BattleStickGesture {
        internal int Horizontal {get;private set;}
        internal int Vertical {get;private set;}
        internal bool Dashing {get;private set;}
        static int Axis(float value,int previous,float threshold){
            if(value>=threshold)return 1;if(value<=-threshold)return -1;
            return previous!=0&&value*previous>threshold*.60f?previous:0;
        }
        internal bool Apply(float x,float y,out int verticalPress){
            int horizontal=Axis(x,Horizontal,.32f),vertical=Axis(y,Vertical,.52f);
            // Different entry/release bands avoid flickering at the outer ring.
            bool dash=horizontal!=0&&x*x+y*y>=(Dashing?.76f*.76f:.88f*.88f)
                &&Math.Abs(x)>=(Dashing?.44f:.55f);
            bool changed=horizontal!=Horizontal||dash!=Dashing;
            Horizontal=horizontal;Dashing=dash;
            verticalPress=vertical!=Vertical?vertical:0;Vertical=vertical;
            return changed;
        }
        internal void Reset(){Horizontal=Vertical=0;Dashing=false;}
    }
}
