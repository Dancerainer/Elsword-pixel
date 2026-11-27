using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
    // Candidate visual owner for active chicken / NUI_CHICKEN only. It does not
    // register itself, advance clocks, select aliases, project or write a model.
    public sealed class ChickenAuthoredPartsV31 : IDisposable {
        public sealed class Binding {
            public readonly Texture2D atlas;
            public readonly float unitsPerArtPixel;
            Rect[] visibleLocalRects;
            internal Rect[] VisibleLocalRects() {
                if(visibleLocalRects!=null)return visibleLocalRects;
                // The caller has validated readable RGBA32. Borrow mip 0 for
                // this calculation only and keep just the six local rectangles.
                var pixels=atlas.GetPixelData<Color32>(0);
                int stride=atlas.width,atlasHeight=atlas.height;
                var result=new Rect[PartCountForUi];
                for(int i=0;i<PartCountForUi;i++) {
                    var p=Parts[i];var r=p.topLeftRect;
                    int rx=(int)r.x,ry=atlasHeight-(int)r.y-(int)r.height,w=(int)r.width,h=(int)r.height;
                    int minX=w,minY=h,maxX=-1,maxY=-1;
                    for(int y=0;y<h;y++){
                        int row=(ry+y)*stride+rx,left=0,right=w-1;
                        while(left<w&&pixels[row+left].a<128)left++;
                        if(left==w)continue;
                        while(right>left&&pixels[row+right].a<128)right--;
                        minX=Math.Min(minX,left);maxX=Math.Max(maxX,right);
                        minY=Math.Min(minY,y);maxY=y;
                    }
                    Need(maxX>=minX&&maxY>=minY,"Visible opaque A1 part for label placement");
                    float py=h-p.pivot.y,u=unitsPerArtPixel;
                    result[i]=Rect.MinMaxRect((minX-p.pivot.x)*u,(minY-py)*u,(maxX+1-p.pivot.x)*u,(maxY+1-py)*u);
                }
                visibleLocalRects=result;return result;
            }
            public Binding(Texture2D texture,float fixedUnitsPerArtPixel) {
                atlas=texture;unitsPerArtPixel=fixedUnitsPerArtPixel;
            }
        }
        public struct Frame {
            public ChickenAuthoredPoseV31.State state;
            public float normalizedPhase,hostClock,sourceRenderClock;
            public float sourceDuration,airFallLaunchPhase;
            public string originalReaction;
            public float originalReactionClock;
            public bool visible;
        }
        struct Layout {
            internal string name;internal Rect topLeftRect;internal Vector2 pivot,position;
            internal float scale;internal int order;
            internal Layout(string n,float x,float y,float w,float h,float px,float py,float atX,float atY,float s,int z) {
                name=n;topLeftRect=new Rect(x,y,w,h);pivot=new Vector2(px,py);
                position=new Vector2(atX,atY);scale=s;order=z;
            }
        }
        // Exact accepted A1 rectangles/pivots/assembly/scales/order, extracted
        // from FemaleRoot17A1DrawnPartsV29.cs c030f374... . Not a 512-cell grid.
        static readonly Layout[] Parts={
            new Layout("head",0,0,600,640,272,523,576,794,1.46f,50),
            new Layout("torso",600,0,536,640,237,246,576,845,1,30),
            new Layout("screen-left-wing",1136,0,400,640,239,280,410,850,.9f,20),
            new Layout("screen-right-wing",0,640,600,384,208,120,748,856,.9f,21),
            new Layout("screen-left-leg-foot",600,640,536,384,182,57,420,1011,.85f,10),
            new Layout("screen-right-leg-foot",1136,640,400,384,146,61,716,1027,.8f,11)
        };
        // Offline original-PNG alpha128/224 unit-pixel corner hulls, transformed
        // by the original layout and float32 scales: minY55.239981,
        // maxY1202.250005. Fixed anchor=ceil(maxY)=1203; top=floor(minY)=55.
        // The 1-art-pixel Canvas preview alone had bottom-exclusive1202 and
        // height1147; it is not the conservative continuous registration.
        // Preserve the original standing chicken:LK body-height ratio.
        // Complete original Wait@0 body geometry gives .965195822132461;
        // the current LK opaque (alpha>=128) standing body is 67 logical px.
        // This changes only authored art registration, never model/collision
        // heights, visualScale, world foot, source clocks or the accepted art.
        public const float AuthoredStandingPixels=1148f;
        public const float SourceChickenToHeroHeightRatio=.965195822132461f;
        public const float CurrentHeroOpaqueStandingHeight=67f;
        public const float SourceRatioLogicalStandingHeight=CurrentHeroOpaqueStandingHeight*SourceChickenToHeroHeightRatio;
        // Accepted A1 art faces left; the existing chicken carrier convention
        // is +1 with flipX=false. This fixed art correction never writes either.
        public const int AuthoredFacing=-1;
        public static float InitialUnitsPerArtPixel { get { return SourceRatioLogicalStandingHeight/AuthoredStandingPixels; } }
        public static Vector2 AuthoredRestFootAnchorPixels {
            get { return new Vector2(ChickenAuthoredPoseV31.ArtFootAnchorX,ChickenAuthoredPoseV31.ArtFootAnchorY); }
        }
        static readonly Vector2 FootAnchor=AuthoredRestFootAnchorPixels;
        const int PartCountForUi=6;
        readonly Binding binding;readonly SpriteRenderer carrier;readonly Action<UnityEngine.Object> destroyOwned;
        readonly Rect[] visibleLocalRects;
        readonly Sprite[] sprites=new Sprite[6];readonly SpriteRenderer[] renderers=new SpriteRenderer[6];
        readonly ChickenAuthoredPoseV31.PartPose[] pose=new ChickenAuthoredPoseV31.PartPose[6];
        readonly MaterialPropertyBlock properties=new MaterialPropertyBlock();
        readonly bool originalForceRenderingOff;
        GameObject root;SortingGroup sortingGroup;bool disposed,hidden;
        public Frame LastCommittedFrame { get; private set; }
        public Binding BindingIdentity { get { return binding; } }
        static void Need(bool v,string why) { if(!v)throw new InvalidOperationException("Chicken authored parts: "+why); }
        static bool Finite(float v) { return !float.IsNaN(v)&&!float.IsInfinity(v); }
        static bool Finite(Vector3 v) { return Finite(v.x)&&Finite(v.y)&&Finite(v.z); }
        static bool Valid(ChickenAuthoredPoseV31.PartPose p) {
            return Finite(p.x)&&Finite(p.y)&&Finite(p.rotation)&&Finite(p.scaleX)&&Finite(p.scaleY)&&p.scaleX>0&&p.scaleY>0;
        }
        static bool Valid(ChickenAuthoredPoseV31.RigPose p) {
            return Finite(p.x)&&Finite(p.y)&&Finite(p.rotation)&&Finite(p.pivotX)&&Finite(p.pivotY)&&
                Finite(p.scaleX)&&Finite(p.scaleY)&&p.scaleX>0&&p.scaleY>0;
        }
        public ChickenAuthoredPartsV31(SpriteRenderer existingCarrier,Binding settings,Action<UnityEngine.Object> destroy) {
            carrier=existingCarrier;binding=settings;destroyOwned=destroy;
            Need(carrier&&binding!=null&&destroyOwned!=null,"Explicit carrier/binding/destroy owner");
            originalForceRenderingOff=carrier.forceRenderingOff;
            Need(!originalForceRenderingOff,"Carrier already belongs to another hidden visual owner");
            var atlas=binding.atlas;
            // This candidate declares the original A1 owner's strict actual
            // RGBA32/readable/Point/noMip format. The declaration is a runtime
            // guard, not a claim that this uncompiled candidate has run.
            string profile=atlas?"width="+atlas.width+" height="+atlas.height+" format="+atlas.format+
                " readable="+atlas.isReadable+" isDataSRGB="+atlas.isDataSRGB+" graphicsFormat="+atlas.graphicsFormat+
                " mip="+atlas.mipmapCount+" filter="+atlas.filterMode+" wrap="+atlas.wrapMode:"missing";
            Need(atlas&&atlas.width==1536&&atlas.height==1024&&atlas.format==TextureFormat.RGBA32&&
                atlas.isReadable&&atlas.mipmapCount==1&&atlas.filterMode==FilterMode.Point,
                "A1 RGBA32/readable/Point/noMip 1536x1024 atlas; actual "+profile);
            Need(Finite(binding.unitsPerArtPixel)&&binding.unitsPerArtPixel>0&&
                Finite(1/binding.unitsPerArtPixel)&&1/binding.unitsPerArtPixel>0,"Fixed finite positive registration and reciprocal");
            visibleLocalRects=binding.VisibleLocalRects();
            try {
                root=new GameObject("Owned active NUI_CHICKEN A1 six parts V31");root.SetActive(false);
                root.layer=carrier.gameObject.layer;root.transform.SetParent(carrier.transform,false);
                sortingGroup=root.AddComponent<SortingGroup>();
                for(int i=0;i<6;i++) {
                    var p=Parts[i];var r=p.topLeftRect;
                    sprites[i]=Sprite.Create(binding.atlas,new Rect(r.x,1024-r.y-r.height,r.width,r.height),
                        new Vector2(p.pivot.x/r.width,1-p.pivot.y/r.height),1/binding.unitsPerArtPixel,0,SpriteMeshType.FullRect);
                    Need(sprites[i],"Owned Sprite.Create");sprites[i].name="Active chicken A1 "+p.name;
                    var go=new GameObject(p.name);go.layer=carrier.gameObject.layer;go.transform.SetParent(root.transform,false);
                    var sr=go.AddComponent<SpriteRenderer>();renderers[i]=sr;sr.sprite=sprites[i];
                    sr.drawMode=SpriteDrawMode.Simple;sr.flipX=sr.flipY=false;sr.sortingOrder=p.order;
                }
            } catch { Dispose();throw; }
        }
        public bool OwnsRenderer(Renderer candidate) {
            if(disposed||!candidate)return false;
            for(int i=0;i<renderers.Length;i++)if(ReferenceEquals(candidate,renderers[i]))return true;
            return false;
        }
        // Call after the carrier's existing final material/MPB and original
        // enemy-nativeDepth projection. Skip these six owned renderers in the
        // original projection traversal. The carrier is projected once there.
        // Read-only renderer visitation for presentation depth binding.
        internal void VisitRenderersR1461(Action<SpriteRenderer> visitor) {
            if(disposed)return;if(visitor==null)throw new ArgumentNullException("visitor");
            for(int i=0;i<renderers.Length;i++)if(renderers[i])visitor(renderers[i]);
        }
        public void Apply(Frame frame,Vector3 projectedFoot) {
            Need(!disposed&&root&&carrier,"Live exact visual owner");
            Need(Finite(projectedFoot)&&Finite(frame.hostClock)&&Finite(frame.sourceRenderClock),"Finite original clocks/explicit projected foot");
            var material=carrier.sharedMaterial;
            Need(material&&material.shader&&material.shader.name=="ElswordPixel/ArmoredSprite"&&material.shader.isSupported,
                "Original final armored sprite material");
            ChickenAuthoredPoseV31.RigPose rig;
            if(!ChickenSourceReactionV45.Sample(frame,pose,out rig))rig=frame.state==ChickenAuthoredPoseV31.State.AirFall?
                ChickenAuthoredPoseV31.SampleAirFall(frame.normalizedPhase,frame.airFallLaunchPhase,pose):
                ChickenAuthoredPoseV31.Sample(frame.state,frame.normalizedPhase,pose);
            Need(Valid(rig),"Finite positive authored rig pose");
            for(int i=0;i<6;i++)Need(renderers[i]&&sprites[i]&&Valid(pose[i]),"Six live parts and finite positive authored poses");
            // Fetch the final carrier MPB every Apply. Preserve _HitFlash,
            // _Outline, _BurialClip, projected _BurialGroundY and other fields.
            // Only the owned atlas texture and its own texel size are replaced.
            carrier.GetPropertyBlock(properties);properties.SetTexture("_MainTex",binding.atlas);
            properties.SetVector("_MainTex_TexelSize",new Vector4(1f/1536,1f/1024,1536,1024));
            root.transform.localRotation=Quaternion.identity;
            root.transform.localScale=new Vector3(carrier.flipX?-AuthoredFacing:AuthoredFacing,carrier.flipY?-1:1,1);
            root.transform.position=projectedFoot;
            sortingGroup.sortingLayerID=carrier.sortingLayerID;sortingGroup.sortingOrder=carrier.sortingOrder;
            double angle=rig.rotation*Math.PI/180,cos=Math.Cos(angle),sin=Math.Sin(angle);float u=binding.unitsPerArtPixel;
            for(int i=0;i<6;i++) {
                var p=Parts[i];var q=pose[i];var sr=renderers[i];
                double ax=(p.position.x+q.x-rig.pivotX)*rig.scaleX;
                double ay=(p.position.y+q.y-rig.pivotY)*rig.scaleY;
                float x=(float)(rig.pivotX+ax*cos-ay*sin+rig.x),y=(float)(rig.pivotY+ax*sin+ay*cos+rig.y);
                var t=sr.transform;t.localPosition=new Vector3((x-FootAnchor.x)*u,-(y-FootAnchor.y)*u,0);
                t.localRotation=Quaternion.Euler(0,0,-(q.rotation+rig.rotation));
                t.localScale=new Vector3(p.scale*q.scaleX*rig.scaleX,p.scale*q.scaleY*rig.scaleY,1);
                sr.sortingOrder=i>=4&&frame.state==ChickenAuthoredPoseV31.State.AttackFire&&q.scaleX>1.05f?45+i:p.order;
                sr.sortingLayerID=carrier.sortingLayerID;sr.sharedMaterial=material;sr.color=carrier.color;
                sr.maskInteraction=carrier.maskInteraction;sr.SetPropertyBlock(properties);
            }
            root.SetActive(frame.visible&&carrier.enabled&&carrier.gameObject.activeInHierarchy);
            // Only rendering ownership changes. The carrier's enabled, sprite,
            // source frame, material/MPB, model root and clocks stay with caller.
            carrier.forceRenderingOff=true;hidden=true;LastCommittedFrame=frame;
        }
        // Opaque local rectangles are measured once from the unchanged atlas.
        // Transform all four corners so rotated attack/down poses remain safe.
        // Labels use this after projection; neither sprite nor model is moved.
        public bool TryVisibleTop(out float top) {
            top=float.NegativeInfinity;if(disposed||!root||!root.activeInHierarchy)return false;
            for(int i=0;i<PartCountForUi;i++) {
                var sr=renderers[i];if(!sr||!sr.enabled||!sr.gameObject.activeInHierarchy)continue;
                Rect r=visibleLocalRects[i];
                for(int corner=0;corner<4;corner++) {
                    var p=sr.transform.TransformPoint(new Vector3((corner&1)==0?r.xMin:r.xMax,(corner&2)==0?r.yMin:r.yMax,0));
                    Need(Finite(p),"Finite projected A1 label top");top=Math.Max(top,p.y);
                }
            }
            return Finite(top);
        }
        public void Dispose() {
            if(disposed)return;disposed=true;if(root)root.SetActive(false);
            if(hidden&&carrier)carrier.forceRenderingOff=originalForceRenderingOff;hidden=false;
            for(int i=0;i<6;i++){if(sprites[i])destroyOwned(sprites[i]);sprites[i]=null;renderers[i]=null;}
            if(root){root.transform.SetParent(null,false);destroyOwned(root);}root=null;sortingGroup=null;
            // Root owns its children. Shared atlas/material/shader, original
            // carrier and library sprites are never destroyed by this owner.
        }
    }
}
