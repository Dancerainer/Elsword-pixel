using UnityEditor;
using UnityEngine;

namespace ElswordPixel {
    // Whole-frame pages must retain the baked pixel grid on every target.
    sealed class MonsterPixelImporterV53 : AssetPostprocessor {
        void OnPreprocessTexture(){
            if(!assetPath.StartsWith("Assets/Resources/Pixel/monster-v53/",System.StringComparison.Ordinal)||!assetPath.EndsWith(".png",System.StringComparison.OrdinalIgnoreCase))return;
            var texture=(TextureImporter)assetImporter;
            texture.textureType=TextureImporterType.Default;
            texture.sRGBTexture=true;texture.mipmapEnabled=false;
            texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
            texture.textureCompression=TextureImporterCompression.Uncompressed;
            texture.crunchedCompression=false;texture.maxTextureSize=2048;
            texture.alphaSource=TextureImporterAlphaSource.FromInput;
            texture.alphaIsTransparency=true;texture.isReadable=false;
        }
    }
}
