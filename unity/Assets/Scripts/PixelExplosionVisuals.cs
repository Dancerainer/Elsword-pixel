using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    internal static class PixelExplosionVisuals {
        // Plane sizes and trajectories are supplied by the original emitter in world units.
        // Only its texture changes. Each source mask is reduced once and remains point filtered.
        static readonly Dictionary<string, Texture2D> masks = new Dictionary<string, Texture2D>(StringComparer.Ordinal);
        static readonly Dictionary<string, Sprite> regionSprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);

        static bool Contains(string text, string part) {
            return text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static bool BurstParticle(string id) {
            return Contains(id, "explos") || Contains(id, "explode") || Contains(id, "FireDamageImpact") || Contains(id, "FireDamageSmoke");
        }

        public static bool RegionalBurst(string id) {
            return Contains(id, "explos") || ((Contains(id, "bomb") || Contains(id, "missile") || Contains(id, "grenade")) && id.EndsWith("#hit", StringComparison.OrdinalIgnoreCase));
        }

        public static Sprite Plane(OutskirtsVisualParticle data, Texture2D original) {
            return Plane(data.id, data.texture, original);
        }

        public static Sprite Plane(string id, string key, Texture2D original) {
            var texture = BurstParticle(id) ? Mask(key, original) : original;
            return MakeSprite(texture);
        }

        public static Sprite Regional(string path, Sprite original) {
            if (!original) return null;
            if (!regionSprites.TryGetValue(path, out var sprite)) {
                sprite = MakeSprite(Mask(path, original.texture));
                regionSprites.Add(path, sprite);
            }
            return sprite;
        }

        static Sprite MakeSprite(Texture2D texture) {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 1);
        }

        static Color32[] Read(Texture2D source) {
            if (source.isReadable) return source.GetPixels32();
            var previous = RenderTexture.active;
            var target = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            var copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            try {
                Graphics.Blit(source, target, Vector2.one, Vector2.zero);
                RenderTexture.active = target;
                copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0, false);
                return copy.GetPixels32();
            } finally {
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(target);
                UnityEngine.Object.Destroy(copy);
            }
        }

        static byte Tone(long weighted, int weight) {
            return (byte)Mathf.Clamp(Mathf.RoundToInt(weighted / (float)weight / 24f) * 24, 0, 255);
        }

        static Texture2D Mask(string key, Texture2D original) {
            // Texture names include a source digest. Region indexes use stable per-texture resource paths.
            if (masks.TryGetValue(key, out var cached)) return cached;
            int size = Contains(key, "smoke") ? 13 : 23;
            float scale = Mathf.Min(1f, size / (float)Mathf.Max(original.width, original.height));
            int width = Mathf.Max(1, Mathf.RoundToInt(original.width * scale));
            int height = Mathf.Max(1, Mathf.RoundToInt(original.height * scale));
            var source = Read(original);
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++) {
                int bottom = y * original.height / height, top = (y + 1) * original.height / height;
                for (int x = 0; x < width; x++) {
                    int left = x * original.width / width, right = (x + 1) * original.width / width;
                    int a = 0;
                    long r = 0, g = 0, b = 0;
                    for (int row = bottom; row < top; row++) {
                        for (int col = left; col < right; col++) {
                            var c = source[row * original.width + col];
                            a += c.a; r += c.r * c.a; g += c.g * c.a; b += c.b * c.a;
                        }
                    }
                    var alpha = Mathf.RoundToInt(a / (float)((top - bottom) * (right - left)));
                    // A few levels make soft smoke read as discrete puffs and keep bright impact rings hollow.
                    if (alpha < 20) continue;
                    byte stepped = (byte)Mathf.Min(255, Mathf.RoundToInt(alpha / 42f) * 42);
                    pixels[y * width + x] = new Color32(Tone(r, a), Tone(g, a), Tone(b, a), stepped);
                }
            }
            var result = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                name = "Pixel explosion " + key, filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp
            };
            result.SetPixels32(pixels);
            result.Apply(false, true);
            masks.Add(key, result);
            return result;
        }
    }
}