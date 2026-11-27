using UnityEditor;
using UnityEngine;
namespace ElswordPixel.Editor {
    public sealed class SceneMusicImporter:AssetPostprocessor {
        void OnPreprocessAudio(){
            if(!assetPath.StartsWith("Assets/Resources/Pixel/music/"))return;
            var importer=(AudioImporter)assetImporter;var settings=importer.defaultSampleSettings;
            settings.loadType=AudioClipLoadType.Streaming;settings.compressionFormat=AudioCompressionFormat.Vorbis;settings.quality=.85f;settings.preloadAudioData=false;
            importer.defaultSampleSettings=settings;importer.loadInBackground=true;
        }
    }
}
