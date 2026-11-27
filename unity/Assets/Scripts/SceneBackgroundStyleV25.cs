using System;
using UnityEngine;

namespace ElswordPixel {
    // Scenery materials only: actors, hit effects and the HUD keep their own shaders.
    public static class SceneBackgroundStyleV25 {
        public struct Settings {
            public string role;
            public float reduction, radius, colorSteps;
            public Settings(string role, float reduction, float radius, float colorSteps) {
                this.role = role; this.reduction = reduction;
                this.radius = radius; this.colorSteps = colorSteps;
            }
        }

        static bool ContainsAny(string value, params string[] terms) {
            foreach (var term in terms)
                if (value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        public static Settings Classify(string partName, string texturePath, int width, int height) {
            string identity = (partName ?? "") + "/" + (texturePath ?? "");
            // Lookup colors, lighting profiles and source effects are not surface detail.
            if (width < 16 || height < 16 || ContainsAny(identity,
                "colormap", "lightmap", "shadow", "glow", "fog", "flare", "lightbeam", "sunbeam", "halo"))
                return new Settings("functional", 0, 0, 16);
            if (ContainsAny(identity, "leaf", "leaves", "bush", "grass", "foliage", "ivy", "canopy", "vine", "moss"))
                return new Settings("foliage", 0.4f, 1.5f, 8);
            // Keep the contact surfaces more legible than distant decorative material.
            if (ContainsAny(identity, "ground", "floor", "platform", "footing", "land", "road", "step", "stair", "tile"))
                return new Settings("surface", 0.12f, 1, 16);
            return new Settings("scenery", 0.25f, 1.25f, 10);
        }

        // Only explicitly named sky geometry may share the no-foreground
        // coverage bit. RGB, alpha-test and native depth remain untouched.
        static bool IsSkyBackdrop(string partName, string texturePath) {
            string part = partName ?? "";
            return part.IndexOf("_SKY.", StringComparison.OrdinalIgnoreCase) >= 0
                || part.EndsWith("_SKY", StringComparison.OrdinalIgnoreCase)
                || part.StartsWith("SKY.", StringComparison.OrdinalIgnoreCase)
                || (texturePath ?? "").EndsWith("_Sky", StringComparison.OrdinalIgnoreCase)
                // Actual part-ID evidence: this named distant cloud also spans
                // below the local floor in Tree3. Only that explicit pair is
                // eligible there; foreground cloud-shaped objects are retained.
                || (part.Equals("RUBEN_EL_TREE_BOSS_CLOUD01.Y", StringComparison.OrdinalIgnoreCase)
                    && (texturePath ?? "").Equals("Pixel/native-world/textures/Ruben_El_Tree_Top_Cloud01", StringComparison.OrdinalIgnoreCase));
        }

        public static void Configure(Material material, string partName, string texturePath, Texture2D texture) {
            if (!material || !material.HasProperty("_DetailReduction")) return;
            if (material.HasProperty("_BackdropCoverage"))
                material.SetFloat("_BackdropCoverage", IsSkyBackdrop(partName, texturePath) ? 0 : 1);
            var settings = Classify(partName, texturePath, texture ? texture.width : 0, texture ? texture.height : 0);
            material.SetFloat("_DetailReduction", settings.reduction);
            material.SetFloat("_DetailRadius", settings.radius);
            material.SetFloat("_ColorSteps", settings.colorSteps);
        }

        // An isolated rendering comparison can switch color treatment without
        // changing any texture, alpha test, mesh, UV, original tint or fog input.
        internal static void CompareOriginalMaterials(bool original) {
            Shader.SetGlobalFloat("_SceneDetailReviewOriginal", original ? 1 : 0);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetReview() { CompareOriginalMaterials(false); }
    }
}
