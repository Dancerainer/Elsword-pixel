namespace ElswordPixel {
    internal static class ArmedHumanoidPinsV37 {
        sealed class Birth{
            public readonly string npc,kind,room;public readonly int dungeon,index,source;
            public Birth(string npc,string kind,string room,int dungeon,int index,int source){this.npc=npc;this.kind=kind;this.room=room;this.dungeon=dungeon;this.index=index;this.source=source;}
        }
        static readonly Birth[] births={
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-1",0,3,8),
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-1",0,4,10),
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-1",0,5,11),
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-2",0,4,13),
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-2",0,5,21),
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-2",0,6,16),
            new Birth("NUI_THIEF_THIN_ZERO","thief","tree-2",0,8,24),
            new Birth("NUI_THIEF_THIN_ZERO","thief","swamp-1",2,0,3),
            new Birth("NUI_THIEF_THIN_ZERO","thief","swamp-1",2,3,11),
            new Birth("NUI_THIEF_THIN_ZERO","thief","swamp-1",2,5,37),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-1",4,3,18),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-1",4,6,25),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-1",4,8,42),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-2",4,0,3),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-2",4,2,20),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-2",4,12,90),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-2",4,14,55),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-2",4,16,42),
            new Birth("NUI_THIEF_THIN","thief-thin","cave-3",4,1,18),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-1",5,0,3),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-1",5,3,31),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-1",5,6,17),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-2",5,0,2),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-2",5,11,66),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-2",5,14,32),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-2",5,18,43),
            new Birth("NUI_SOLDIER_THIN","soldier-thin","waterway-2",5,20,82),
            new Birth("NUI_THIEF_THIN_TOTO","toto","swamp-1",2,2,9),
            new Birth("NUI_THIEF_THIN_TOTO","toto","swamp-1",2,4,14),
            new Birth("NUI_THIEF_THIN_TOTO","toto","swamp-1",2,6,31),
        };
        internal static string Root(string id){switch(id){
                case "NUI_THIEF_THIN_ZERO":return "Pixel/feedback-v37/armed/thief/";
                case "NUI_THIEF_THIN":return "Pixel/feedback-v37/armed/thief-thin/";
                case "NUI_SOLDIER_THIN":return "Pixel/feedback-v37/armed/soldier-thin/";
                case "NUI_THIEF_THIN_TOTO":return "Pixel/feedback-v37/armed/toto/";
                default:return null;}}
        internal static string Binding(string id){switch(id){
                case "NUI_THIEF_THIN_ZERO":return "f7145a1c5373a2f490a0f4f2b86df85e39c52ad9be3481cea35dcd714364355e";
                case "NUI_THIEF_THIN":return "bcc2c51b450c563805e8867da5216f400879b453245234d9cdadd2c9416efbd0";
                case "NUI_SOLDIER_THIN":return "dc626f06eb6b994d4f3c6c82450e7fcd937c2c83ba72ba89a241c50a7a4d755b";
                case "NUI_THIEF_THIN_TOTO":return "e772167eaf451795f1c56762ac9f650a87e1dec0d0f329e29bf77ef03d7bcec5";
                default:return null;}}
        internal static string WeaponProof(string id){switch(id){
                case "NUI_THIEF_THIN_ZERO":return "00b8f7747703f99e6092d91312518bc81d51c37be901c1b5ff1d20189e6c4f3e";
                case "NUI_THIEF_THIN":return "e7083668fc287db01a8f50a45ddcfdd6a9652901035b6d188ca663612118e2de";
                case "NUI_SOLDIER_THIN":return "99a820b7280c09a78689b468324083f5044301876140bdcaa2042ec1f384f7b6";
                case "NUI_THIEF_THIN_TOTO":return "fe7cb5c4f008366ebb4d019f0c023c77755d447b47ae485894dbc6e1aa67ca5c";
                default:return null;}}
        internal static bool HasRoom(string room){foreach(var b in births)if(b.room==room)return true;return false;}
        internal static int Native(string id,int slot,string room){foreach(var b in births)if(b.npc==id&&b.index==slot&&b.room==room)return b.source;return -1;}
        internal static bool Eligible(string id,string kind,string room,int slot,int dungeon){
            foreach(var b in births)if(b.npc==id&&b.kind==kind&&b.room==room&&b.index==slot&&b.dungeon==dungeon)return true;return false;
        }
    }
}
