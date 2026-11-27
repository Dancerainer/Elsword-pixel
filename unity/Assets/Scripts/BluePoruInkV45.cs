namespace ElswordPixel {
    internal static class BluePoruInkV45 {
        internal const string Kind="pporu-william-thief";
        internal const string Signed="Pixel/monsters/outskirts/pporu-william-thief/atlas";
        internal const string Native="Pixel/feedback-v25/outskirts-native-v27/pporu-william-thief/atlas-00";
        internal const string Ready="Pixel/motion-v45/pporu-william-thief/page-0";
        const string Root="Pixel/monster-fidelity-v45/blue-poru/";
#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
        internal static readonly bool Before=System.Array.IndexOf(System.Environment.GetCommandLineArgs(),"--blue-poru-before-v45")>=0;
#endif
        internal static string Resource(string id,string texture){
            if(id!=Kind)return texture;
#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
            if(Before)return Prior(texture);
#endif
            switch(texture){
                case Signed:return Root+"signed-v2";
                case Native:return Root+"native-v2";
                case Ready:return Root+"ready-v2";
                default:return texture;
            }
        }
        internal static string Prior(string baseResource){
            switch(baseResource){
                case Signed:return Root+"signed";
                case Native:return Root+"native";
                default:return baseResource;
            }
        }
        internal static string Pin(string texture){
            switch(texture){
                case Root+"signed":return "4b9caef62dee2e635d8277fb4f54108aebcc1465ad7887e5dfc25e7e16e85889";
                case Root+"native":return "bc52d44312493a7b09d89a174c8a5532df96318f8bc94a4e7aa70e261ab4d0c5";
                case Root+"signed-v2":return "d02a2e232cb476ff18c7a4b276e0c0b22de5fca67b12c0bfd5db755a96c6eb7c";
                case Root+"native-v2":return "02e48cb06781e88d1be49a964d13c11d51637f6f2e2d96b0a6debc75b2ea9a32";
                case Root+"ready-v2":return "52005fc34e75b82860374eecbe822f3b5c113fccca65230f19879ca93b6c0680";
                default:return null;
            }
        }
    }
}
