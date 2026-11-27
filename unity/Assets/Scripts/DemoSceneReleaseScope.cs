using System;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const int ReleaseRoomCount=23;
        const int ReleaseContentSetCount=4;
        static readonly string[] ReleaseDungeonIds={
            "tree","ruins","swamp","dark-forest","banthus-cave","underground-waterway",
            "wally-castle-outskirts","wally-castle-center"
        };
        static bool IsRubenElderRelease {get{return true;}}

        static string ReleaseDungeonRegion(int index){return index<3?"ruben":"elder";}

        static bool IsReleaseDungeon(int index,string id){
            return index>=0&&index<ReleaseDungeonIds.Length&&ReleaseDungeonIds[index]==id;
        }

        static void RequireDevelopmentContent(){
            if(IsRubenElderRelease)throw new InvalidOperationException("This edition contains only Ruben and Elder.");
        }

        void ValidateReleaseCatalog(){
            if(catalog==null||catalog.dungeons==null||catalog.stages==null||catalog.npcContentSets==null||
                catalog.dungeons.Length!=ReleaseDungeonIds.Length||catalog.stages.Length!=ReleaseRoomCount||
                catalog.npcContentSets.Length!=ReleaseContentSetCount)
                throw new InvalidOperationException("Unexpected Ruben and Elder content catalog.");

            int next=0;
            for(int i=0;i<catalog.dungeons.Length;i++){
                var route=catalog.dungeons[i];
                int expectedRooms=i==ReleaseDungeonIds.Length-1?2:3;
                if(route==null||!IsReleaseDungeon(i,route.id)||route.start!=next||route.count!=expectedRooms||
                    DungeonRegion(route)!=ReleaseDungeonRegion(i))
                    throw new InvalidOperationException("Unexpected Ruben or Elder route at index "+i+".");
                next+=route.count;
            }
        }
    }
}
