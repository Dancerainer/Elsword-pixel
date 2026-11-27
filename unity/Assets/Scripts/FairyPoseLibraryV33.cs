using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    [Serializable]
    public sealed class FairyPartLayoutV33 {
        public string name;
        public int[] rect, geometry, indices;
        public float[] pivot, position, visibleHull;
        public float scale;
        public int order, alpha64, page, instanceOf; // 1-based source part, or 0 for original pixels.
        public bool mirrorX;
    }

    [Serializable]
    public sealed class FairyPoseTrackV33 {
        public string alias, animation;
        public float duration;
        public bool loop;
        public int count, components;
        public float fps;
        public float[] values;

        public void Sample(float clock, int part, out Vector2 position, out float degrees) {
            Vector2 axes; SampleAxes(clock, part, out position, out degrees, out axes);
        }

        public void Sample(float clock, int part, out Vector2 position, out float degrees, out float scale) {
            if (components == 5) throw new InvalidOperationException("Use SampleAxes for an anisotropic registered track.");
            Vector2 axes;
            SampleAxes(clock, part, out position, out degrees, out axes);
            scale = axes.x;
        }

        internal int Segment(float clock,out int second,out float phase){
            float hz=fps>0?fps:24;
            float t=loop?Mathf.Repeat(clock,duration):Mathf.Clamp(clock,0,duration);
            int first=Mathf.Min(Mathf.FloorToInt(t*hz),count-1);
            second=Mathf.Min(first+1,count-1);
            float from=Mathf.Min(first/hz,duration),until=Mathf.Min(second/hz,duration);
            phase=until>from?Mathf.Clamp01((t-from)/(until-from)):0;
            return first;
        }

        public void SampleAxes(float clock, int part, out Vector2 position, out float degrees, out Vector2 scale) {
            int fields=components>0?components:3;
            int stride=values.Length/count;
            int first=Segment(clock,out int second,out float phase);
            int a=first*stride+part*fields,b=second*stride+part*fields;
            float x = fields >= 4 ? Mathf.Lerp(values[a + 3], values[b + 3], phase) : 1f;
            float y = fields == 5 ? Mathf.Lerp(values[a + 4], values[b + 4], phase) : x;
            scale = new Vector2(x, y);
            position = new Vector2(Mathf.Lerp(values[a], values[b], phase),
                Mathf.Lerp(values[a + 1], values[b + 1], phase));
            degrees = Mathf.LerpAngle(values[a + 2], values[b + 2], phase);
        }
    }

    [Serializable]
    public sealed partial class FairyPoseLibraryV33 {
        [NonSerialized] internal RegisteredLimbPose limbs;
        [NonSerialized] internal RegisteredLimbVolume limbVolume;
        [NonSerialized] internal RegisteredBodyPose bodyPose;
        public const string ResourceRoot = "Pixel/feedback-v33/fairy-guardian/";
        public const string AtlasSha = "dc10c1910e49e0415bdced74372eb204b5c3a1db597fbae506ad54690eb1ed87";
        public const string PoseSha = "e004f1e6a1a665da0227a3cef2e0b4f0299983f2ef268a86711ea64cfc0c01ce";
        public string schema, atlasSha256, motionSha256, rigName;
        [NonSerialized] internal Texture2D[] sourcePages;
        [NonSerialized] internal Texture2D[] ownedDecodedPages;
        public int authoredFacing, nativeYaw, atlasWidth, atlasHeight;
        public float[] foot;
        public float standingPixels, heightRatio, heroHeight;
        public FairyPartLayoutV33[] parts;
        public FairyPoseTrackV33[] tracks;
        [NonSerialized] public Texture2D atlas;
        [NonSerialized] public Texture2D renderAtlas;
        [NonSerialized] public Sprite[] sharedSprites;
        Dictionary<string, FairyPoseTrackV33> byAlias;
        public float UnitsPerPixel { get { return heroHeight * heightRatio / standingPixels; } }

        internal static void Require(bool ok, string label) {
            if (!ok) throw new InvalidOperationException("Fairy authored presentation: " + label);
        }

        public static FairyPoseLibraryV33 Load() {
            var text = Resources.Load<TextAsset>(ResourceRoot + "poses");
            Require(text != null, "missing source poses");
            using (var sha = SHA256.Create()) {
                string actual = BitConverter.ToString(sha.ComputeHash(text.bytes)).Replace("-", "").ToLowerInvariant();
                Require(actual == PoseSha, "source pose and layout hash differs");
            }
            var lib = JsonUtility.FromJson<FairyPoseLibraryV33>(text.text);
            Require(lib != null && lib.schema == "fairy-seven-source-pose-v33-r1" &&
                lib.atlasSha256 == AtlasSha && lib.authoredFacing == -1 && lib.nativeYaw == 180 &&
                lib.parts.Length == 7 && lib.tracks.Length == 10, "registration scope");
            lib.atlas = Resources.Load<Texture2D>(ResourceRoot + "atlas");
            var atlas = lib.atlas;
            Require(atlas && atlas.width == lib.atlasWidth && atlas.height == lib.atlasHeight &&
                atlas.isReadable && atlas.isDataSRGB && atlas.mipmapCount == 1 &&
                atlas.filterMode == FilterMode.Point && atlas.wrapMode == TextureWrapMode.Clamp &&
                (atlas.format == TextureFormat.RGBA32 || atlas.format == TextureFormat.ARGB32),
                "readable RGBA sRGB Point/Clamp atlas");
            lib.byAlias = new Dictionary<string, FairyPoseTrackV33>(StringComparer.Ordinal);
            foreach (var track in lib.tracks) {
                Require(track.count >= 2 && track.values.Length == track.count * 21 &&
                    track.duration > 0, "source track samples");
                lib.byAlias.Add(track.alias, track);
            }
            foreach (var spec in GuardianCombatNativeRulesV28.All) {
                FairyPoseTrackV33 track;
                Require(lib.byAlias.TryGetValue(spec.alias, out track) &&
                    track.animation == spec.animation && track.count == spec.samples &&
                    Mathf.Abs(track.duration - spec.span) < .000001f && track.loop == spec.loop,
                    spec.alias + " source binding");
            }
            try {
                lib.AppendSourceRearPosesV34();
                lib.PrepareSharedArt();
                return lib;
            } catch {
                lib.ReleaseOwned(o => {
                    if (Application.isPlaying) UnityEngine.Object.Destroy(o);
                    else UnityEngine.Object.DestroyImmediate(o);
                });
                throw;
            }
        }

        internal void PrepareRegisteredParts(Texture2D[] pages) {
            Require(pages!=null && pages.Length>0 && parts!=null && parts.Length>0 && tracks!=null,"source parts and pages");
            sourcePages=pages;atlas=pages[0];
            byAlias=new Dictionary<string,FairyPoseTrackV33>(StringComparer.Ordinal);
            foreach(var t in tracks) {
                int fields=t.components>0?t.components:3;
                Require(t.count>=2 && (fields==3||fields==4||fields==5) && t.values.Length==t.count*parts.Length*fields && t.duration>0,"registered track "+t.alias);
                if (fields == 5) {
                    for (int at = 3; at < t.values.Length; at += fields) {
                        float x = t.values[at], y = t.values[at + 1];
                        Require(x >= 0 && y >= 0 && !float.IsInfinity(x) && !float.IsInfinity(y), "finite projected axes " + t.alias);
                    }
                }
                byAlias.Add(t.alias,t);
            }
            limbs=RegisteredLimbPose.ForOther(this);
            limbVolume=RegisteredLimbVolume.From(this);
            bodyPose=RegisteredBodyPose.For(this);
            PrepareSharedArt();
        }

        void PrepareSharedArt() {
            // Place exact original texels into seven nonoverlapping UV rectangles.
            // Padding keeps neighboring body parts out of outline texture reads.
            const int width = 2048, padding = 3;
            var origins = new Vector2Int[parts.Length];
            int cursorX = padding, cursorY = padding, rowHeight = 0;
            for (int i = 0; i < parts.Length; ++i) {
                if (parts[i].instanceOf != 0) continue;
                int w = parts[i].rect[2], h = parts[i].rect[3];
                if (cursorX + w + padding > width) {
                    cursorX = padding;
                    cursorY += rowHeight + padding * 2;
                    rowHeight = 0;
                }
                origins[i] = new Vector2Int(cursorX, cursorY);
                cursorX += w + padding * 2;
                rowHeight = Math.Max(rowHeight, h);
            }
            int height = cursorY + rowHeight + padding;
            var pages=sourcePages??new[]{atlas};
            var pixels=new Color32[pages.Length][];
            for(int page=0;page<pages.Length;page++)pixels[page]=pages[page].GetPixels32();
            var packed = new Color32[width * height];
            for (int i = 0; i < parts.Length; ++i) {
                var p = parts[i];
                if (p.instanceOf != 0) continue;
                Require(p.page>=0 && p.page<pages.Length,"part sheet "+p.name);
                var pageTexture=pages[p.page];
                var original=pixels[p.page];
                int seen = 0;
                for (int q = 0; q < p.geometry.Length; q += 8) {
                    int x0 = p.geometry[q], y0 = p.geometry[q + 1];
                    int x1 = p.geometry[q + 2], y1 = p.geometry[q + 5];
                    Require(x0 >= 0 && y0 >= 0 && x1 <= p.rect[2] && y1 <= p.rect[3] &&
                        x1 > x0 && y1 > y0, "part copy bounds " + p.name);
                    for (int y = y0; y < y1; ++y) {
                        int from = (pageTexture.height - p.rect[1] - p.rect[3] + y) * pageTexture.width + p.rect[0] + x0;
                        int to = (origins[i].y + y) * width + origins[i].x + x0;
                        Array.Copy(original, from, packed, to, x1 - x0);
                        for (int x = 0; x < x1 - x0; ++x) if (original[from + x].a >= 64) ++seen;
                    }
                }
                Require(seen == p.alpha64, "original alpha64 coverage " + p.name);
                if(ThiefLegPose.UsesPart(rigName,i))
                    ThiefLegPose.PaintPart(rigName,i,p,packed,origins[i],width);
                TreeMonsterPixelStyle.Paint(this,i,packed,origins[i],width);
            }
            renderAtlas = new Texture2D(width, height, TextureFormat.RGBA32, false, false) {
                name = string.IsNullOrEmpty(rigName)?"Owned Fairy seven-part UV atlas V33":rigName+" isolated UV atlas", filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp, anisoLevel = 0
            };
            renderAtlas.SetPixels32(packed);
            renderAtlas.Apply(false, true);
            sharedSprites = new Sprite[parts.Length];
            for (int i = 0; i < parts.Length; ++i) {
                var p = parts[i];
                if (p.instanceOf != 0) {
                    sharedSprites[i] = sharedSprites[p.instanceOf - 1];
                    continue;
                }
                var rect = new Rect(origins[i].x, origins[i].y, p.rect[2], p.rect[3]);
                var pivot = new Vector2(p.pivot[0] / p.rect[2], 1 - p.pivot[1] / p.rect[3]);
                sharedSprites[i] = Sprite.Create(renderAtlas, rect, pivot, 1 / UnitsPerPixel, 0, SpriteMeshType.FullRect);
                sharedSprites[i].name = (string.IsNullOrEmpty(rigName)?"Fairy R4":rigName)+" / "+p.name;
            }
        }

        public void ReleaseOwned(Action<UnityEngine.Object> destroy) {
            if (sharedSprites != null) {
                for (int i = 0; i < sharedSprites.Length; ++i)
                    if (parts[i].instanceOf == 0 && sharedSprites[i]) destroy(sharedSprites[i]);
                sharedSprites = null;
            }
            if (renderAtlas) destroy(renderAtlas);
            renderAtlas = null;
            if (ownedDecodedPages != null) {
                foreach (var texture in ownedDecodedPages) if (texture) destroy(texture);
                ownedDecodedPages = null;
                sourcePages = null;
                atlas = null;
            }
        }

        public FairyPoseTrackV33 Find(string alias) {
            FairyPoseTrackV33 found;
            return alias != null && byAlias.TryGetValue(alias, out found) ? found : null;
        }
    }
}

