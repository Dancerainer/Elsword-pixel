namespace ElswordPixel {
    internal static class ReusablePaintPinsV37 {
        internal static string ForKind(string kind){switch(kind){
                case "thief-fat":return "a92954c25016793075a9a60f3bc884602882eb86f6f0cf0d22ae373c11b0dd44";
                case "thief-fat-boss":return "13fceda089f899aa03c1a5c4654852f7eb45429cce34795ef6906f8a101a45cf";
                case "soldier-fat":return "015e9669cfc3eba5af248937868bb025ed2a92d10426f67d72c75a36c8f9c735";
                default:return null;}}
        internal static bool InScope(string kind,string npc,string room){switch(kind){
                case "thief-fat":return npc=="NUI_THIEF_FAT"&&(room=="cave-1"||room=="cave-2"||room=="cave-3");
                case "thief-fat-boss":return npc=="NUI_THIEF_FAT_BOSS"&&(room=="cave-1"||room=="cave-2");
                case "soldier-fat":return npc=="NUI_SOLDIER_FAT"&&(room=="waterway-1"||room=="waterway-2"||room=="waterway-3");
                default:return false;}}
        internal static bool HasRoom(string room){switch(room){
                case "cave-1":case "cave-2":case "cave-3":case "waterway-1":case "waterway-2":case "waterway-3":return true;
                default:return false;}}
    }
}
