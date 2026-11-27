using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ElswordPixel {
    public sealed class FairyAuthoredPartsV33 : IDisposable {
        public struct Frame {
            public string alias;
            public float sourceClock;
            public int facing;
            public bool visible;
        }

        readonly SpriteRenderer carrier;
        readonly FairyPoseLibraryV33 lib;
        readonly RegisteredHeadPoseV44 rigidHead;
        readonly Action<Object> destroy;
        readonly Sprite[] sprites;
        static readonly int MonsterInk = Shader.PropertyToID("_MonsterInk");
        readonly SpriteRenderer[] views;
        readonly bool[] paintedContour;
        internal SpriteRenderer[] Members { get { return views; } }
        readonly Vector4 atlasTexelSize;
        readonly MaterialPropertyBlock block = new MaterialPropertyBlock();
        readonly bool previousForceOff;
        GameObject root;
        SortingGroup sorting;
        bool hidden, disposed;
        public Frame LastFrame { get; private set; }

        public FairyAuthoredPartsV33(SpriteRenderer source, FairyPoseLibraryV33 library, Action<Object> destroyOwned) {
            FairyPoseLibraryV33.Require(source && library != null && destroyOwned != null &&
                !source.forceRenderingOff, "exclusive live carrier");
            carrier = source;
            lib = library;
            int width=lib.renderAtlas.width,height=lib.renderAtlas.height;
            atlasTexelSize=new Vector4(1f/width,1f/height,width,height);
            rigidHead = RegisteredHeadPoseV44.From(lib);
            sprites=new Sprite[lib.parts.Length];
            views=new SpriteRenderer[lib.parts.Length];
            paintedContour=new bool[lib.parts.Length];
            destroy = destroyOwned;
            previousForceOff = source.forceRenderingOff;
            try {
                root = new GameObject(string.IsNullOrEmpty(lib.rigName)?"Owned NUI_FAIRY_GUARDIAN seven parts V33":lib.rigName);
                root.SetActive(false);
                root.layer = carrier.gameObject.layer;
                root.transform.SetParent(carrier.transform, false);
                sorting = root.AddComponent<SortingGroup>();
                for (int i = 0; i < views.Length; ++i) BuildPart(i);
            } catch {
                Dispose();
                throw;
            }
        }

        void BuildPart(int i) {
            var p = lib.parts[i];
            paintedContour[i]=ThiefLegPose.UsesPart(lib.rigName,i);
            var sprite = lib.sharedSprites[i];
            sprites[i] = sprite;
            var child = new GameObject(p.name);
            child.layer = root.layer;
            child.transform.SetParent(root.transform, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            views[i] = renderer;
            renderer.sprite = sprite;
            renderer.flipX=p.mirrorX;
            renderer.sortingOrder = p.order;
        }

        public bool OwnsRenderer(Renderer candidate) {
            for (int i = 0; i < views.Length; ++i)
                if (ReferenceEquals(views[i], candidate)) return true;
            return false;
        }

        public void VisitRenderers(Action<SpriteRenderer> visit) {
            if (visit == null) throw new ArgumentNullException(nameof(visit));
            if (disposed) return;
            foreach (var view in views) if (view) visit(view);
        }

        public void Apply(Frame frame, Vector3 projectedFoot, float carrierPixelScale = 1) {
            var track = lib.Find(frame.alias);
            FairyPoseLibraryV33.Require(!disposed && carrier && root && track != null &&
                (frame.facing == 1 || frame.facing == -1) && !float.IsNaN(frame.sourceClock) &&
                !float.IsInfinity(frame.sourceClock) && frame.sourceClock >= 0, "valid source display frame");
            var material = carrier.sharedMaterial;
            FairyPoseLibraryV33.Require(material && material.shader &&
                material.shader.name == "ElswordPixel/ArmoredSprite" && material.shader.isSupported,
                "source ArmoredSprite material");
            carrier.GetPropertyBlock(block);
            block.SetTexture("_MainTex", lib.renderAtlas);
            block.SetVector("_MainTex_TexelSize", atlasTexelSize);
            root.transform.localRotation = Quaternion.identity;
            // Parts have their own registered units per pixel. A fallback whole sprite may
            // use different pixel units, even though both views share the same actor root.
            root.transform.localScale = new Vector3(frame.facing * lib.authoredFacing / carrierPixelScale,
                (carrier.flipY ? -1 : 1) / carrierPixelScale, 1);
            var stride=lib.bodyPose!=null?lib.bodyPose.Step(track,frame.sourceClock):default;
            if(stride.rise!=0)projectedFoot+=root.transform.TransformVector(Vector3.up*stride.rise);
            root.transform.position=projectedFoot;
            var sortingLayer=carrier.sortingLayerID;
            var tint=carrier.color;
            var mask=carrier.maskInteraction;
            float inheritedInk=block.GetFloat(MonsterInk);
            float appliedInk=inheritedInk;
            sorting.sortingLayerID = sortingLayer;
            sorting.sortingOrder = carrier.sortingOrder;
            float unit = lib.UnitsPerPixel;
            for (int i = 0; i < views.Length; ++i) {
                var p = lib.parts[i];
                Vector2 at;
                float rotation;
                Vector2 projectedScale;
                track.SampleAxes(frame.sourceClock, i, out at, out rotation, out projectedScale);
                lib.limbVolume?.Fit(i,track,frame.sourceClock,ref rotation,ref projectedScale);
                lib.bodyPose?.Fit(i,track,frame.sourceClock,at,stride,ref rotation,ref projectedScale);
                if (rigidHead != null && i == rigidHead.Part)
                    rigidHead.Fit(track, frame.sourceClock, at, ref rotation, ref projectedScale);
                var view = views[i];
                view.transform.localPosition = new Vector3((at.x - lib.foot[0]) * unit,
                    (lib.foot[1] - at.y) * unit, 0);
                view.transform.localRotation = Quaternion.Euler(0, 0, -rotation);
                view.transform.localScale = new Vector3(p.scale * projectedScale.x, p.scale * projectedScale.y, 1);
                view.sortingLayerID = sortingLayer;
                view.sharedMaterial = material;
                view.color = tint;
                view.maskInteraction = mask;
                // The painted trouser edges already describe the joined leg. A second
                // inward stroke on each moving piece covers the cloth at the knee.
                float ink=paintedContour[i]?0:inheritedInk;
                if(ink!=appliedInk){block.SetFloat(MonsterInk,ink);appliedInk=ink;}
                view.SetPropertyBlock(block);
            }
            lib.limbs?.Apply(views,track,frame.sourceClock);
            bool visible=frame.visible && carrier.enabled && carrier.gameObject.activeInHierarchy;
            if(root.activeSelf!=visible)root.SetActive(visible);
            carrier.forceRenderingOff = true;
            hidden = true;
            LastFrame = frame;
        }

        internal void SuspendPresentation(Frame frame) {
            if(root.activeSelf)root.SetActive(false);
            carrier.forceRenderingOff=true;
            hidden=true;
            LastFrame=frame;
        }

        public bool TryVisibleTop(out float top) {
            top = float.NegativeInfinity;
            if (disposed || !root || !root.activeInHierarchy) return false;
            for (int i = 0; i < views.Length; ++i) {
                var hull = lib.parts[i].visibleHull;
                var matrix = views[i].transform.localToWorldMatrix;
                float xAxis = matrix.m10 * (views[i].flipX ? -1 : 1) * lib.UnitsPerPixel;
                float yAxis = matrix.m11 * lib.UnitsPerPixel;
                for (int j = 0; j < hull.Length; j += 2) {
                    top = Mathf.Max(top, matrix.m13+xAxis*hull[j]+yAxis*hull[j+1]);
                }
            }
            return !float.IsInfinity(top);
        }

        public void Dispose() {
            if (disposed) return;
            disposed = true;
            if (root) root.SetActive(false);
            if (hidden && carrier) carrier.forceRenderingOff = previousForceOff;
            for (int i = 0; i < sprites.Length; ++i) {
                sprites[i] = null;
                views[i] = null;
            }
            if (root) {
                root.transform.SetParent(null, false);
                destroy(root);
            }
            root = null;
        }
    }
}

