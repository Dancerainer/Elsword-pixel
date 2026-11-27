using System;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;
namespace ElswordPixel {
    public sealed class ForestPlantPoseBankV38 : NativePoseSpriteBankV38 {
        public ForestPlantPoseBankV38(string kind,string npc,Action<Object> destroy,Dictionary<Sprite,Vector2> offsets)
            :base(kind,npc,destroy,offsets,data=>MatchesSource(kind,data)) {}
        static bool MatchesSource(string kind,MageLibraryV38 data){
            bool ent=kind=="anger-forest-ent";
            string motion=ent?"6d3e00bde13ca122b1c3e194bb9feb22c0458dab9ca276a24e9b231bbedf8d98":"64888fe755d78ad197a9796edf55dfb9fcfda13fdd588a9fe9480800218fb34b";
            return (ent||kind=="anger-forest-fairy")&&data.artVersion=="forest-native-uv-v38"&&
                data.nativeYaw==(ent?180:0)&&data.authoredFacing==(ent?-1:1)&&data.sourceMotionSha==motion&&
                data.sourceMotionCount==(ent?18:21)&&data.paintedPoseCount==(ent?1160:419)&&
                data.clips!=null&&data.clips.Length==(ent?47:42)&&Mathf.Abs(data.logicalScale-1)<.0001f&&
                Mathf.Abs(data.standingHeight-(ent?248.9024f:86.893f))<.01f;
        }
    }
}
