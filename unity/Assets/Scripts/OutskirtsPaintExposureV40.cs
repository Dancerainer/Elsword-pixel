using System;
using UnityEngine;

namespace ElswordPixel {
    internal static class OutskirtsPaintExposureV40 {
        internal static string Resource(string kind){return "Pixel/monsters/outskirts/"+kind+"/atlas";}
        internal static string Rgba(string kind){return kind=="pporu-william-thief"?"ef9eb8739331c00c52c323f49f424d2d65e7254e862368767921565d619d11a9":kind=="pporu-william-thief-fire"?"e83ed877159ee0610a8e6f34ee9b8f53c571fe37e57ab841cb40f16cecbfb806":kind=="monkey-bomb"?"8eabf0429b956236344455f2d87b9cd72d10edf890cfe5eac10a5ab43072df80":null;}
        internal static bool Kinds(string kind){return kind=="pporu-william-thief"||kind=="pporu-william-thief-fire"||kind=="monkey-bomb";}
        internal static AnimationData Paint(SpriteLibraryData bank,AnimationData selected){
            if(bank==null||selected==null||bank.artVersion!="outskirts-whole-pose-v14-r1"||!Kinds(bank.id)||bank.texture!=Resource(bank.id)
                ||!selected.id.StartsWith("source-",StringComparison.Ordinal)||selected.id.StartsWith("source-native-",StringComparison.Ordinal))return null;
            string key=selected.id.Substring("source-".Length);
            var drawn=Array.Find(bank.clips,c=>c.id==key);
            if(drawn==null||drawn.frames==null||drawn.frames.Length==0||drawn.fps<=0||drawn.duration<=0||selected.fps<=0||selected.duration<=0||selected.loop!=drawn.loop)
                throw new InvalidOperationException("Missing installed Outskirts V14 exposure "+bank.id+"/"+selected.id);
            return drawn;
        }
        internal static int Map(AnimationData selected,AnimationData drawn,int sourceIndex){
            double sourceTime=(double)sourceIndex/selected.fps;
            // Cast through double so equal integer frame boundaries map to
            // that same painted frame. Source clips already applied ANIM_SPEED.
            double exact=Math.Min(1,sourceTime/selected.duration)*drawn.duration*drawn.fps;
            return Math.Max(0,Math.Min(drawn.frames.Length-1,(int)Math.Floor(exact+.00001)));
        }
        internal static FrameData Frame(AnimationData selected,AnimationData drawn,int index){
            return drawn==null?selected.frames[index]:drawn.frames[Map(selected,drawn,index)];
        }
    }
}
