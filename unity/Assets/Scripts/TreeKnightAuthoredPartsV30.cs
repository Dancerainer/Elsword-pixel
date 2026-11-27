using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
    // Isolated visual owner for the new TreeKnight drawing. No MonoBehaviour,
    // model step, collision, clock advance, texture load or native projection.
    // Its caller commits only after the existing final burial/material pass.
    public sealed class TreeKnightAuthoredPartsV30 : IDisposable {
        public sealed class Binding {
            public readonly Texture2D atlas;
            public readonly float unitsPerArtPixel,attackContactPhase;
            public Binding(Texture2D texture,float fixedUnitsPerArtPixel,float modelContactPhase) {
                atlas=texture;unitsPerArtPixel=fixedUnitsPerArtPixel;attackContactPhase=modelContactPhase;
            }
        }
        public struct Frame {
            public TreeKnightAuthoredPoseV30.State state;
            public float normalizedPhase,hostClock,sourceRenderClock;
            public bool strikeObserved,visible;
        }
        struct Layout {
            internal string name;internal int cell,order;
            internal Vector2 pivot,position;internal float scale;
            internal Layout(string n,int c,float px,float py,float x,float y,float s,int z) {
                name=n;cell=c;pivot=new Vector2(px,py);position=new Vector2(x,y);scale=s;order=z;
            }
        }
        // Author-selected first assembly. Pixel pivots are new art coordinates,
        // not original source bones. Native-size visual acceptance is pending.
        static readonly Layout[] Parts={
            new Layout("head-crown-face",0,273,484,576,835,1.5f,60),
            new Layout("torso",1,274,195,576,825,.85f,65),
            new Layout("screen-left-arm",2,400,140,482,845,.7f,20),
            new Layout("screen-right-arm-sword",3,75,80,666,850,.95f,50),
            new Layout("screen-left-root-foot",4,285,135,500,1035,.7f,10),
            new Layout("screen-right-root-foot",5,210,130,650,1035,.7f,11)
        };
        public const float CurrentLogicalStandingHeight=56;
        public const float AuthoredStandingPixels=1052;
        public static float InitialUnitsPerArtPixel { get { return CurrentLogicalStandingHeight/AuthoredStandingPixels; } }
        static readonly Vector2 FootAnchor=new Vector2(576,1192);
        readonly Binding binding;readonly SpriteRenderer carrier;readonly Action<UnityEngine.Object> destroyOwned;
        readonly Sprite[] sprites=new Sprite[6];readonly SpriteRenderer[] renderers=new SpriteRenderer[6];
        readonly TreeKnightAuthoredPoseV30.PartPose[] pose=new TreeKnightAuthoredPoseV30.PartPose[6];
        readonly MaterialPropertyBlock properties=new MaterialPropertyBlock();
        readonly bool originalForceRenderingOff;
        GameObject root;SortingGroup sortingGroup;bool disposed,hidden;
        public Frame LastCommittedFrame { get; private set; }
        internal string VisibleHurtMotionV45 { get; private set; }
        internal string VisibleAttackMotionV45 { get; private set; }
        public Binding BindingIdentity { get { return binding; } }
        static void Need(bool v,string why) { if(!v)throw new InvalidOperationException("Tree authored parts: "+why); }
        static bool Finite(float x) { return !float.IsNaN(x)&&!float.IsInfinity(x); }
        static bool Finite(Vector3 p) { return Finite(p.x)&&Finite(p.y)&&Finite(p.z); }

        public TreeKnightAuthoredPartsV30(SpriteRenderer existingCarrier,Binding settings,Action<UnityEngine.Object> destroy) {
            carrier=existingCarrier;binding=settings;destroyOwned=destroy;
            Need(carrier&&binding!=null&&destroyOwned!=null,"Explicit carrier/binding/destroy owner");
            originalForceRenderingOff=carrier.forceRenderingOff;
            Need(!originalForceRenderingOff,"Carrier already belongs to another hidden visual owner");
            var atlas=binding.atlas;
            string profile=atlas?"width="+atlas.width+" height="+atlas.height+" format="+atlas.format+
                " readable="+atlas.isReadable+" isDataSRGB="+atlas.isDataSRGB+" graphicsFormat="+atlas.graphicsFormat+
                " mip="+atlas.mipmapCount+" filter="+atlas.filterMode+" wrap="+atlas.wrapMode:"missing";
            Need(atlas&&atlas.width==1536&&atlas.height==1024&&atlas.mipmapCount==1&&atlas.isReadable&&
                (atlas.format==TextureFormat.RGBA32||atlas.format==TextureFormat.ARGB32)&&atlas.isDataSRGB&&
                atlas.graphicsFormat==UnityEngine.Experimental.Rendering.GraphicsFormatUtility.GetGraphicsFormat(atlas.format,true)&&
                atlas.filterMode==FilterMode.Point&&atlas.wrapMode==TextureWrapMode.Clamp,
                "Readable RGBA32-or-ARGB32 dataSRGB/appropriate GPU format/PointClamp/noMip atlas; actual "+profile);
            Need(Finite(binding.unitsPerArtPixel)&&binding.unitsPerArtPixel>0,"Fixed positive registration");
            Need(Finite(binding.attackContactPhase)&&binding.attackContactPhase>.2f&&binding.attackContactPhase<.95f,"Explicit model contact phase");
            try {
                root=new GameObject("Owned NUI_TREE_KNIGHT authored six parts V30");root.SetActive(false);
                root.layer=carrier.gameObject.layer;root.transform.SetParent(carrier.transform,false);
                sortingGroup=root.AddComponent<SortingGroup>();
                for(int i=0;i<6;i++) {
                    var p=Parts[i];int x=p.cell%3*512,y=p.cell/3*512;
                    sprites[i]=Sprite.Create(binding.atlas,new Rect(x,1024-y-512,512,512),
                        new Vector2(p.pivot.x/512,1-p.pivot.y/512),1/binding.unitsPerArtPixel,0,SpriteMeshType.FullRect);
                    Need(sprites[i],"Owned Sprite.Create");sprites[i].name="Tree authored "+p.name;
                    var go=new GameObject(p.name);go.layer=carrier.gameObject.layer;go.transform.SetParent(root.transform,false);
                    var r=go.AddComponent<SpriteRenderer>();renderers[i]=r;r.sprite=sprites[i];
                    r.drawMode=SpriteDrawMode.Simple;r.flipX=r.flipY=false;r.sortingOrder=p.order;
                }
            } catch { Dispose();throw; }
        }

        // Native projection must skip exactly these renderers. The original
        // carrier still receives its original enemy nativeDepth projection once.
        public bool OwnsRenderer(Renderer candidate) {
            for(int i=0;i<renderers.Length;i++)if(ReferenceEquals(candidate,renderers[i]))return true;
            return false;
        }
        // Read-only renderer visitation for presentation depth binding.
        internal void VisitRenderersR1461(Action<SpriteRenderer> visitor) {
            if(disposed)return;if(visitor==null)throw new ArgumentNullException("visitor");
            for(int i=0;i<renderers.Length;i++)if(renderers[i])visitor(renderers[i]);
        }
        public void Apply(Frame frame,Vector3 projectedFootAnchorWorld,Enemy enemy=null) {
            Need(!disposed&&root&&carrier,"Live exact visual owner");
            Need(Finite(projectedFootAnchorWorld)&&Finite(frame.hostClock)&&Finite(frame.sourceRenderClock),"Finite original clocks/explicit projected anchor");
            var material=carrier.sharedMaterial;
            Need(material&&material.shader&&material.shader.name=="ElswordPixel/ArmoredSprite"&&material.shader.isSupported,
                "Original final armored sprite material");
            var rig=TreeKnightAuthoredPoseV30.Sample(frame.state,frame.normalizedPhase,binding.attackContactPhase,frame.strikeObserved,pose);
            var sourceHit=TreeKnightHurtV45.For(enemy,out float hurtClock);
            VisibleHurtMotionV45=sourceHit==null?null:sourceHit.motion;
            if(sourceHit==null&&frame.state==TreeKnightAuthoredPoseV30.State.Attack)
                sourceHit=TreeKnightHurtV45.OriginalAttackV45(enemy,out hurtClock);
            VisibleAttackMotionV45=VisibleHurtMotionV45==null&&frame.state==TreeKnightAuthoredPoseV30.State.Attack&&sourceHit!=null?sourceHit.motion:null;
            // Read the final existing MPB; preserve the original _BurialClip,
            // projected _BurialGroundY, _Outline and _HitFlash without changing
            // the carrier or shared material. The candidate must sample its own
            // atlas, rather than the old texture that SetArmorOutline wrote.
            carrier.GetPropertyBlock(properties);properties.SetTexture("_MainTex",binding.atlas);
            properties.SetVector("_MainTex_TexelSize",new Vector4(1f/1536,1f/1024,1536,1024));
            root.transform.localRotation=Quaternion.identity;
            root.transform.localScale=new Vector3(carrier.flipX?-1:1,carrier.flipY?-1:1,1);
            root.transform.position=projectedFootAnchorWorld;
            sortingGroup.sortingLayerID=carrier.sortingLayerID;sortingGroup.sortingOrder=carrier.sortingOrder;
            double angle=rig.rotation*Math.PI/180,cos=Math.Cos(angle),sin=Math.Sin(angle);float u=binding.unitsPerArtPixel;
            for(int i=0;i<6;i++) {
                var p=Parts[i];var q=pose[i];var r=renderers[i];Need(r&&sprites[i],"All six owned parts live");
                float px,py,partRotation=q.rotation,partScaleX=q.scaleX,partScaleY=q.scaleY;
                if(i==2) {
                    var parentLayout=Parts[1];var parent=pose[1];
                    // Current sampler keeps part scales at 1. A flat Sprite TRS
                    // cannot express a nonuniform parent scale followed by arm rotation.
                    Need(Finite(parent.scaleX)&&Finite(parent.scaleY)&&parent.scaleX>0&&parent.scaleX==parent.scaleY,
                        "Linked left shoulder requires finite positive uniform torso pose scale");
                    double parentAngle=parent.rotation*Math.PI/180,pc=Math.Cos(parentAngle),ps=Math.Sin(parentAngle);
                    // The B rest offset is [-94,20] in assembled art pixels;
                    // static torso .85 is not a parent pose scale.
                    double ox=(p.position.x-parentLayout.position.x)*parent.scaleX;
                    double oy=(p.position.y-parentLayout.position.y)*parent.scaleY;
                    px=(float)(parentLayout.position.x+parent.x+ox*pc-oy*ps);
                    py=(float)(parentLayout.position.y+parent.y+ox*ps+oy*pc);
                    // Reinterpret the original arm angle as torso-local articulation.
                    // The linked anchor replaces arm q.x/y; do not add them again.
                    partRotation=parent.rotation+q.rotation;
                    partScaleX=parent.scaleX*q.scaleX;partScaleY=parent.scaleY*q.scaleY;
                } else {px=p.position.x+q.x;py=p.position.y+q.y;}
                double ax=(px-rig.pivotX)*rig.scaleX;
                double ay=(py-rig.pivotY)*rig.scaleY;
                float x=(float)(rig.pivotX+ax*cos-ay*sin+rig.x),y=(float)(rig.pivotY+ax*sin+ay*cos+rig.y);
                float finalAngle=partRotation+rig.rotation;
                if(sourceHit!=null)sourceHit.Sample(hurtClock,i,out x,out y,out finalAngle);
                var t=r.transform;t.localPosition=new Vector3((x-FootAnchor.x)*u,-(y-FootAnchor.y)*u,0);
                // Art Y-down becomes Unity Y-up once; keep the negative sprite angle.
                t.localRotation=Quaternion.Euler(0,0,-finalAngle);
                t.localScale=sourceHit==null?new Vector3(p.scale*partScaleX*rig.scaleX,p.scale*partScaleY*rig.scaleY,1):new Vector3(p.scale,p.scale,1);
                r.sortingLayerID=carrier.sortingLayerID;r.sharedMaterial=material;r.color=carrier.color;
                r.maskInteraction=carrier.maskInteraction;r.SetPropertyBlock(properties);
            }
            root.SetActive(frame.visible&&carrier.enabled&&carrier.gameObject.activeInHierarchy);
            // This does not change enabled, the source sprite/frame/MPB, bounds,
            // model visibility, UI, HP, super armor, support or hit timing.
            carrier.forceRenderingOff=true;hidden=true;LastCommittedFrame=frame;
        }
        public void Dispose() {
            if(disposed)return;disposed=true;if(root)root.SetActive(false);
            if(hidden&&carrier)carrier.forceRenderingOff=originalForceRenderingOff;hidden=false;
            for(int i=0;i<6;i++){if(sprites[i])destroyOwned(sprites[i]);sprites[i]=null;renderers[i]=null;}
            if(root){root.transform.SetParent(null,false);destroyOwned(root);}root=null;sortingGroup=null;
            // Shared atlas/material/shader, carrier and library sprites are not
            // owned here. Destroying the root owns its six children/components.
        }
    }
}
