namespace ElswordPixel {
    internal static class BendersFamilyPinsV36 {
        internal static bool HasRoom(string room){return room=="tree-3"||room=="swamp-1"||room=="cave-3"||room=="waterway-3"||ArmedHumanoidPinsV37.HasRoom(room);}
        internal static bool Known(string npc){return Root(npc)!=null;}
        internal static string Root(string npc){switch(npc){
                case "NUI_BENDERS_NORMAL":return "Pixel/feedback-v36/tree-benders/";
                case "NUI_BENDERS_THIEF":return "Pixel/feedback-v36/swamp-benders/";
                case "NUI_BENDERS_UPGRADE_NORMAL":return "Pixel/feedback-v36/cave-benders/";
                case "NUI_BENDERS_NASOD":return "Pixel/feedback-v36/waterway-benders/";
                default:return ArmedHumanoidPinsV37.Root(npc);}}
        internal static string Binding(string npc){switch(npc){
                case "NUI_BENDERS_NORMAL":return "815bd5aa6e6fe352b7193844d41e14e2c9448eebfc21bc0654a41c5128c2b83c";
                case "NUI_BENDERS_THIEF":return "e3c2448e454c1f4f9af63fa8bb19a4a73558a8bae9fcb2a3e2228e387e3b587b";
                case "NUI_BENDERS_UPGRADE_NORMAL":return "e28f1734eb67ec334f99bc9ee69369efc3dcede45c4b6ca17cd259a091da414f";
                case "NUI_BENDERS_NASOD":return "593578a09f01542b8f153850987ed2e6c0e7126fead98c169ed7b2056838df16";
                default:return ArmedHumanoidPinsV37.Binding(npc);}}
        internal static bool Eligible(string npc,string kind,string room,int index,int dungeon){switch(npc){
                case "NUI_BENDERS_NORMAL":return kind=="benders"&&room=="tree-3"&&dungeon==0&&(index==0);
                case "NUI_BENDERS_THIEF":return kind=="benders-thief"&&room=="swamp-1"&&dungeon==2&&(index==1||index==7);
                case "NUI_BENDERS_UPGRADE_NORMAL":return kind=="benders-upgrade-normal"&&room=="cave-3"&&dungeon==4&&(index==2);
                case "NUI_BENDERS_NASOD":return kind=="benders-nasod"&&room=="waterway-3"&&dungeon==5&&(index==2);
                default:return ArmedHumanoidPinsV37.Eligible(npc,kind,room,index,dungeon);}}
        internal static int NativeSpawn(string npc,int index,string room=null){switch(npc){
                case "NUI_BENDERS_NORMAL":return 11;
                case "NUI_BENDERS_THIEF":return index==1?5:25;
                case "NUI_BENDERS_UPGRADE_NORMAL":return 14;
                case "NUI_BENDERS_NASOD":return 2;
                default:return ArmedHumanoidPinsV37.Native(npc,index,room);}}
    }
}
