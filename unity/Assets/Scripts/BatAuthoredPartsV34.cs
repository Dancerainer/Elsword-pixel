using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
    // Visual owner with three controls and seven source drawings. The carrier continues to
    // select the source sprite and own gameplay state, visibility, color and native depth.
    public sealed class BatAuthoredPartsV34 : IDisposable {
        readonly BatAuthoredBankV34 bank;
        readonly SpriteRenderer carrier;
        readonly Action<UnityEngine.Object> destroy;
        readonly Sprite[] sprites = new Sprite[7];
        readonly SpriteRenderer[] renders = new SpriteRenderer[3];
        readonly int[] selected = new int[3];
        readonly MaterialPropertyBlock properties = new MaterialPropertyBlock();
        readonly Dictionary<BatWholeBankV34.Drawing,Sprite> wholeSprites = new Dictionary<BatWholeBankV34.Drawing,Sprite>();
        SpriteRenderer wholeRenderer;
        BatWholeBankV34.Drawing currentDrawing;
        GameObject root,rig;
        SortingGroup sorting;
        bool disposed,ownsCarrier;
        public BatAuthoredPartsV34(BatAuthoredBankV34 data,SpriteRenderer source,Action<UnityEngine.Object> destroyOwned) {
            if (data==null || !source || destroyOwned==null || source.forceRenderingOff)
                throw new ArgumentException("Bat needs an available original carrier");
            bank=data; carrier=source; destroy=destroyOwned;
            try {
                root=new GameObject("BAT_A authored V34");
                root.layer=source.gameObject.layer;
                root.SetActive(false);
                root.transform.SetParent(carrier.transform,false);
                sorting=root.AddComponent<SortingGroup>();
                rig=new GameObject("Body and attached wing roots");
                rig.layer=root.layer;
                rig.transform.SetParent(root.transform,false);
                for (int i=0;i<sprites.Length;i++) {
                    var p=bank.pieces[i];
                    var rect=new Rect(p.x,bank.atlas.height-p.y-p.h,p.w,p.h);
                    var pivot=new Vector2((float)(p.px-p.x)/p.w,1f-(float)(p.py-p.y)/p.h);
                    sprites[i]=Sprite.Create(bank.atlas,rect,pivot,1f/bank.unitsPerPixel,0,SpriteMeshType.FullRect);
                    if (!sprites[i]) throw new InvalidOperationException("Bat sprite registration failed: "+p.name);
                    sprites[i].name="bat-a-v34/"+p.name;
                }
                string[] names={"Head, body and feet","Near wing","Far wing"};
                for (int i=0;i<renders.Length;i++) {
                    var go=new GameObject(names[i]);
                    go.layer=root.layer;
                    go.transform.SetParent(rig.transform,false);
                    renders[i]=go.AddComponent<SpriteRenderer>();
                    renders[i].sortingOrder=i==0?1:i==1?2:0;
                }
                var goWhole=new GameObject("Source-clock authored whole pose");
                goWhole.layer=root.layer;
                goWhole.transform.SetParent(root.transform,false);
                wholeRenderer=goWhole.AddComponent<SpriteRenderer>();
                wholeRenderer.enabled=false;
            } catch { Dispose(); throw; }
        }
        public bool Owns(Renderer r) {
            if (ReferenceEquals(r,wholeRenderer)) return true;
            for (int i=0;i<renders.Length;i++) if (ReferenceEquals(r,renders[i])) return true;
            return false;
        }
        public void Visit(Action<SpriteRenderer> visitor) {
            if (visitor==null) throw new ArgumentNullException("visitor");
            if (disposed) return;
            if (currentDrawing!=null) { visitor(wholeRenderer); return; }
            foreach (var r in renders) if (r) visitor(r);
        }
        public bool TryTop(out float top) {
            top=float.NegativeInfinity;
            if (disposed || !root || !root.activeInHierarchy) return false;
            if (currentDrawing!=null) {
                var h=currentDrawing.hull;
                for (int j=0;j<h.Length;j+=2)
                    top=Math.Max(top,wholeRenderer.transform.TransformPoint(new Vector3(h[j]*currentDrawing.units,h[j+1]*currentDrawing.units,0)).y);
                return !float.IsInfinity(top);
            }
            for (int i=0;i<renders.Length;i++) {
                var r=renders[i]; var hull=bank.pieces[selected[i]].hull;
                for (int j=0;j<hull.Length;j+=2) {
                    var v=new Vector3(hull[j]*(r.flipX?-1:1)*bank.unitsPerPixel,hull[j+1]*bank.unitsPerPixel,0);
                    top=Math.Max(top,r.transform.TransformPoint(v).y);
                }
            }
            return !float.IsInfinity(top);
        }
        void Wing(int slot,bool far,float stroke) {
            // The edge and arched drawings include a different projected wing-tip angle.
            // Compensating it keeps the tip sweep continuous when projections change.
            int view=stroke<.24f?0:stroke<.62f?2:1;
            int index=(far?4:1)+view;
            var r=renders[slot]; selected[slot]=index; r.sprite=sprites[index];
            r.flipX=far; r.flipY=false;
            float reference=view==0?0:view==1?51f:57f;
            float angle=(112f*stroke-reference)*(far?-1f:1f);
            r.transform.localRotation=Quaternion.Euler(0,0,angle);
            float scale=far?1.02f:1.14f;
            r.transform.localScale=new Vector3(scale,scale,1);
            float px=far?653:590,py=far?149:184;
            r.transform.localPosition=new Vector3((px-625)*bank.unitsPerPixel,(165-py)*bank.unitsPerPixel,0);
        }
        public void Apply(BatAuthoredBankV34.Motion motion,float sourceClock,Vector3 projectedAnchor,int facing,bool visible) {
            if (disposed || !carrier || !root || (facing!=1 && facing!=-1)
                || !BatAuthoredBankV34.Finite(projectedAnchor.x) || !BatAuthoredBankV34.Finite(projectedAnchor.y)
                || !BatAuthoredBankV34.Finite(projectedAnchor.z) || !bank.CanDraw(motion,sourceClock))
                throw new InvalidOperationException("Invalid Bat authored registration");
            var material=carrier.sharedMaterial;
            if (!material || !material.shader || material.shader.name!="ElswordPixel/ArmoredSprite" || !material.shader.isSupported)
                throw new InvalidOperationException("Missing production Bat carrier material");
            root.transform.localRotation=Quaternion.identity;
            root.transform.localScale=new Vector3(facing,1,1);
            root.transform.position=projectedAnchor;
            sorting.sortingLayerID=carrier.sortingLayerID;
            sorting.sortingOrder=carrier.sortingOrder;
            float time=BatWholeBankV34.PhaseTime(motion,sourceClock);
            bank.whole.Select(motion.alias,time,out currentDrawing);
            rig.SetActive(currentDrawing==null);
            wholeRenderer.enabled=currentDrawing!=null;
            if (currentDrawing!=null) ApplyWhole(motion,time,material);
            else {
                var key=BatAuthoredBankV34.Sample(motion,sourceClock);
                var offset=Vector3.zero;
                if (motion.alias=="source-wait-zero2") {
                    float unused;
                    offset=BatWholeBankV34.SourcePlacement(motion,time,1,out unused);
                    // Rest already registers the artwork around the Wait foot; only its
                    // empirical source-bottom vertical offset applies during release.
                    offset.x=0;
                }
                rig.transform.localPosition=offset+new Vector3(35f*bank.unitsPerPixel+key.x,210f*bank.unitsPerPixel+key.y,0);
                rig.transform.localRotation=Quaternion.Euler(0,0,key.tilt);
                selected[0]=0; renders[0].sprite=sprites[0];
                Wing(1,false,key.stroke); Wing(2,true,key.stroke);
                SetProperties(bank.atlas);
                foreach (var r in renders) SetRenderer(r,material);
            }
            root.SetActive(visible && carrier.enabled && carrier.gameObject.activeInHierarchy);
            carrier.forceRenderingOff=true;
            ownsCarrier=true;
        }
        void SetProperties(Texture2D atlas) {
            carrier.GetPropertyBlock(properties);
            properties.SetTexture("_MainTex",atlas);
            properties.SetVector("_MainTex_TexelSize",new Vector4(1f/atlas.width,1f/atlas.height,atlas.width,atlas.height));
        }
        void SetRenderer(SpriteRenderer r,Material material) {
            r.sharedMaterial=material; r.color=carrier.color;
            r.sortingLayerID=carrier.sortingLayerID; r.maskInteraction=carrier.maskInteraction;
            r.SetPropertyBlock(properties);
        }
        void ApplyWhole(BatAuthoredBankV34.Motion motion,float time,Material material) {
            var d=currentDrawing; var sheet=bank.whole.Page(d);
            if (!wholeSprites.TryGetValue(d,out var sprite)) {
                var rect=new Rect(d.x,sheet.h-d.y-d.h,d.w,d.h);
                var pivot=new Vector2((float)(d.px-d.x)/d.w,1f-(float)(d.py-d.y)/d.h);
                sprite=Sprite.Create(sheet.atlas,rect,pivot,1f/d.units,0,SpriteMeshType.FullRect);
                if (!sprite) throw new InvalidOperationException("Cannot register Bat whole pose");
                wholeSprites.Add(d,sprite);
            }
            wholeRenderer.sprite=sprite;
            wholeRenderer.transform.localPosition=BatWholeBankV34.SourcePlacement(motion,time,d.anchorMode,out float stretch);
            wholeRenderer.transform.localScale=new Vector3(d.artScaleX,d.artScaleY*stretch,1);
            SetProperties(sheet.atlas); SetRenderer(wholeRenderer,material);
        }
        public void Dispose() {
            if (disposed) return;
            disposed=true;
            if (root) root.SetActive(false);
            if (ownsCarrier && carrier) carrier.forceRenderingOff=false;
            ownsCarrier=false;
            foreach (var sprite in sprites) if (sprite) destroy(sprite);
            foreach (var sprite in wholeSprites.Values) if (sprite) destroy(sprite);
            wholeSprites.Clear();
            if (root) { root.transform.SetParent(null,false); destroy(root); root=null; }
        }
    }
}
