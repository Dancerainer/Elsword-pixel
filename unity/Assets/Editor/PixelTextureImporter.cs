using UnityEditor;
using UnityEngine;
namespace ElswordPixel.Editor
{
    public sealed class PixelTextureImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/") && !assetPath.StartsWith("Assets/Art/")) return;
            var texture = (TextureImporter)assetImporter;
            bool exactCore24 = assetPath == "Assets/Resources/Pixel/feedback-v28/first3-core24-v28-r1/tree-knight/core-atlas.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v28/first3-core24-v28-r1/bat-a/core-atlas.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v28/first3-core24-v28-r1/bat-b/core-atlas.png";
            texture.filterMode = FilterMode.Point;
            texture.mipmapEnabled = false;
            texture.textureCompression = TextureImporterCompression.Uncompressed;
            texture.npotScale = TextureImporterNPOTScale.None;
            if (assetPath == "Assets/Resources/Pixel/feedback-v37/tree-flock/mother.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/tree-flock/chick.png")
            {
                texture.isReadable = false;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.alphaIsTransparency = false;
                texture.maxTextureSize = assetPath.EndsWith("/chick.png") ? 32 : 128;
                return;
            }
            bool sampledLegacyPaint = assetPath == "Assets/Resources/Pixel/monster-motion-v13/anger-forest-knight-drawn-r1/motion-atlas.png"
                || assetPath == "Assets/Resources/Pixel/monster-motion-v13/curse-wisp-drawn-r1/motion-atlas.png"
                || assetPath == "Assets/Resources/Pixel/monster-motion-v13/william-cute-drawn-r1/motion-atlas.png"
                || assetPath == "Assets/Resources/Pixel/monster-motion-v13/william-thief-drawn-r1/motion-atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/anger-forest-knight/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/curse-wisp/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/outskirts/monkey-bomb/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/outskirts/pporu-william-thief-fire/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/outskirts/pporu-william-thief/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/william-cute/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/william-thief/atlas.png";
            bool bluePoruPaint = assetPath.StartsWith("Assets/Resources/Pixel/monster-fidelity-v45/blue-poru/");
            if (sampledLegacyPaint || bluePoruPaint)
            {
                texture.textureType = TextureImporterType.Default;
                texture.isReadable = true;
                texture.sRGBTexture = true;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.alphaSource = TextureImporterAlphaSource.FromInput;
                texture.alphaIsTransparency = false;
                texture.maxTextureSize = bluePoruPaint && (assetPath.EndsWith("/native.png") || assetPath.EndsWith("/native-v2.png")) ? 4096 : 2048;
                var standalone = texture.GetPlatformTextureSettings("Standalone");
                standalone.overridden = true;
                standalone.format = TextureImporterFormat.RGBA32;
                standalone.textureCompression = TextureImporterCompression.Uncompressed;
                standalone.maxTextureSize = texture.maxTextureSize;
                texture.SetPlatformTextureSettings(standalone);
                return;
            }
            bool healerPage = assetPath == "Assets/Resources/Pixel/feedback-v35/healer-blader/page-0.png";
            bool mineOrGuardPage = assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-rail/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-driller-cannon/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/benders-equipment/cave-waterway-equipment-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/benders-equipment/cave-waterway-upper-arms-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/swamp-benders/swamp-mask-legs-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/swamp-benders/swamp-mask-upper-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/tree-benders/parts-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/tree-benders/parts-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v36/tree-benders/parts-2.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/soldier-thin/standing-donor.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/soldier-thin/upper.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/thief-thin/standing-donor.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/thief-thin/upper.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/thief/standing-donor.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/thief/upper.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/toto/standing-donor.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v37/armed/toto/upper.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-guard/parts-a.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-guard/parts-b.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-guard/parts-feet.png";
            bool cannonPage = assetPath == "Assets/Resources/Pixel/feedback-v35/cannon-mini/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/cannon-heavy-machine-gun/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/cannon-heavy-machine-gun/page-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/cannon-big/page-0.png";
            if (cannonPage || mineOrGuardPage || healerPage || assetPath == "Assets/Resources/Pixel/feedback-v32/nasod-sheriff/atlas.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/factory-raven-clone/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/waterway-mouse/poses-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/plain-nasod-mouse/machine-gaits.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-guardian-small/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/factory-raven-clone/page-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/ancient-phoru/fore-ankles-v36-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/ancient-phoru/hind-ankles-v36-r1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-spray/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-spray/page-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-spray/projectile-texture.tga"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-spray-ice/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-spray-ice/page-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/nasod-spray-ice/projectile-texture.tga"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/plain-ice-recovery/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/plain-ice-recovery/page-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/ancient-phoru/page-0.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/ancient-phoru/page-1.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v35/ancient-phoru/page-2.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v34/bat-a/dive.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v34/bat-a/ground.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v34/bat-a/hang.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v34/bat-a/parts.png"
                || assetPath == "Assets/Resources/Pixel/feedback-v33/fairy-guardian/atlas.png")
            {
                texture.textureType = TextureImporterType.Default;
                texture.isReadable = true;
                texture.sRGBTexture = true;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.alphaSource = TextureImporterAlphaSource.FromInput;
                texture.alphaIsTransparency = false;
                texture.maxTextureSize = healerPage || cannonPage ? 4096 : 2048;
                return;
            }
            if (assetPath == "Assets/Resources/Pixel/feedback-v29/white-chicken-a1/atlas.png"
                || assetPath == "Assets/Resources/Pixel/monsters/TreeKnightA1V30/sixparts.png")
            {
                texture.alphaIsTransparency = false;
                texture.maxTextureSize = 2048;
                return;
            }
            // Exact runtime redraw targets are validated against full decoded RGBA.
            texture.alphaIsTransparency = !exactCore24 && !assetPath.StartsWith("Assets/Resources/Pixel/feedback-v25/")
                && assetPath != "Assets/Resources/Pixel/feedback-v28/ent-body-depth1-normal-v28-r1/normal-atlas.png"
                && assetPath != "Assets/Resources/Pixel/native-world/waterway/textures/6e2433c43a990f6e87d9.png"
                && assetPath != "Assets/Resources/Pixel/native-world/outskirts/textures/d2c5cf5fe84dca83f61e.png"
                && assetPath != "Assets/Resources/Pixel/native-world/outskirts/textures/b942c2726273992b2dfe.png"
                && assetPath != "Assets/Resources/Pixel/native-world/outskirts/textures/48cdcbcdefd0e1890415.png"
                && assetPath != "Assets/Resources/Pixel/native-world/outskirts/textures/2a311a4624459367f9e7.png"
                && !System.Text.RegularExpressions.Regex.IsMatch(assetPath,
                @"\AAssets/Resources/Pixel/monster-texture-redraw-v22/texture-(?:00[1-9]|0[1-8][0-9]|090)\.png\z");
            texture.maxTextureSize = 4096;
            if (exactCore24) texture.isReadable = true;
        }
    }
}
