using System;
using UnityEngine;

namespace ElswordPixel {
    internal readonly struct CatalogDigest {
        readonly int length;
        readonly string sha256;

        internal CatalogDigest(byte[] bytes) {
            length = bytes == null ? -1 : bytes.Length;
            sha256 = bytes == null ? null : BelowPlayerResourceIndexV21.Hash(bytes);
        }

        internal bool Matches(int expectedLength, string expectedHash) {
            return sha256 != null && length == expectedLength &&
                   string.Equals(sha256, expectedHash, StringComparison.Ordinal);
        }
    }

    public sealed partial class DemoScene {
        CatalogDigest installedCatalogDigest;

        CatalogDigest CurrentCatalogDigest() {
#if UNITY_EDITOR
            // During Editor play an import can replace an asset between two menu visits.
            var asset = Resources.Load<TextAsset>("Pixel/runtime-catalog");
            return new CatalogDigest(asset ? asset.bytes : null);
#else
            // In a built player Resources is immutable for the duration of the process.
            return installedCatalogDigest;
#endif
        }
    }
}
