using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

namespace ElswordPixel {
    // Original authored whole sprites keep their original UVs. The owned mesh
    // moves each visible painted boot and lower leg against its own source
    // bone/sole; it never modifies a shared texture or the actor's carrier.
    internal sealed class ReusablePaintViewV37 : IDisposable {
        sealed class OwnedPose {
            internal Sprite sprite;internal Vector2[] basePoints,weights;
            internal NativeArray<Vector3> positions;internal ushort[] triangles;
            internal bool[] leftShoe,rightShoe;
            internal int[][] hulls;
        }
        readonly SpriteRenderer carrier;readonly Action<UnityEngine.Object> destroy;
        readonly ReusablePaintRigV37 art;readonly bool priorOff;
        readonly Dictionary<int,OwnedPose> cache=new Dictionary<int,OwnedPose>();
        readonly MaterialPropertyBlock block=new MaterialPropertyBlock();
        readonly GameObject root;
        internal readonly SpriteRenderer painted;
        internal ReusablePaintRigV37.Frame Last {get;private set;}
        internal Vector2[] lastRootSoles=new Vector2[2];
        internal float visibleTop;bool disposed;

        internal ReusablePaintViewV37(SpriteRenderer original,ReusablePaintRigV37 bank,Action<UnityEngine.Object> release){
            ReusablePaintRigV37.Need(original&&bank!=null&&release!=null&&!original.forceRenderingOff,"exclusive source carrier");
            carrier=original;art=bank;destroy=release;priorOff=original.forceRenderingOff;
            root=new GameObject("Owned painted "+art.npc+" V37");root.SetActive(false);root.layer=carrier.gameObject.layer;
            root.transform.SetParent(original.transform,false);painted=root.AddComponent<SpriteRenderer>();
        }
        internal bool Owns(Renderer r){return r==painted;}
        static float Smooth(float t){t=Mathf.Clamp01(t);return t*t*(3-2*t);}
        static List<float> Axis(int size,ReusablePaintKeyV37 p,bool xAxis){
            float unit=p.unit;var feet=p.feet;
            var values=new List<float>{0,size};int intervals=Math.Max(1,Mathf.CeilToInt(size*unit/2.2f));
            for(int i=1;i<intervals;i++)values.Add((float)i*size/intervals);
            if(feet.Length==4){
                int field=xAxis?0:1;values.Add(Mathf.Clamp(feet[field],0,size));values.Add(Mathf.Clamp(feet[field+2],0,size));
                if(xAxis)values.Add(Mathf.Clamp((feet[0]+feet[2])/2,0,size));
                foreach(var h in new[]{p.hullLeft,p.hullRight})for(int j=field;j<h.Length;j+=2)values.Add(Mathf.Clamp(h[j],0,size));
            }
            values.Sort();for(int i=values.Count-1;i>0;i--)if(values[i]-values[i-1]<.001f)values.RemoveAt(i);
            return values;
        }
        static Vector2 Weights(ReusablePaintKeyV37 key,float x,float y){
            if(key.feet.Length!=4)return Vector2.zero;
            float h=key.bbox[3]-key.bbox[1],hip=key.bbox[1]+h*.46f;
            float midpoint=(key.feet[0]+key.feet[2])*.5f;
            float half=Mathf.Max(.1f,Math.Abs(key.feet[0]-key.feet[2])*.14f);
            float side=Smooth((x-midpoint+half)/(2*half));if(key.feet[0]<key.feet[2])side=1-side;
            float left=Smooth((y-hip)/Mathf.Max(1,key.feet[1]-.15f*h-hip));
            float right=Smooth((y-hip)/Mathf.Max(1,key.feet[3]-.15f*h-hip));
            // Distinct exposed sole contours are rigidly owned even when the
            // facing reverses; interpolation remains smooth up the trousers.
            if(y>=Math.Min(key.feet[1],key.feet[3])-.09f*h)
                side=(x<midpoint)==(key.feet[0]<key.feet[2])?1:0;
            return new Vector2(side*left,(1-side)*right);
        }
        OwnedPose GetPose(int index){
            if(cache.TryGetValue(index,out var known))return known;
            var p=art.keys[index];var tex=art.Texture(p);int w=p.rect[2],h=p.rect[3];
            var xs=Axis(w,p,true);var ys=Axis(h,p,false);int n=xs.Count*ys.Count;
            ReusablePaintRigV37.Need(n<65535,"16-bit sprite mesh bounds");
            var owned=new OwnedPose{basePoints=new Vector2[n],weights=new Vector2[n],leftShoe=new bool[n],rightShoe=new bool[n]};
            Sprite sprite=null;
            try{
                sprite=Sprite.Create(tex,new Rect(p.rect[0],tex.height-p.rect[1]-h,w,h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                sprite.name="Registered paint "+art.kind+" "+index;owned.sprite=sprite;
                var positions=new Vector2[n];var triangles=new ushort[(xs.Count-1)*(ys.Count-1)*6];int k=0;
                for(int j=0;j<ys.Count;j++)for(int i=0;i<xs.Count;i++){
                    int id=j*xs.Count+i;float x=xs[i],y=ys[j];
                    positions[id]=new Vector2(x,h-y);owned.basePoints[id]=new Vector2((x-p.pivot[0])*p.unit,(p.pivot[1]-y)*p.unit);
                    owned.weights[id]=Weights(p,x,y);
                    if(p.feet.Length==4){
                        float mid=(p.feet[0]+p.feet[2])*.5f;bool left=(x<mid)==(p.feet[0]<p.feet[2]);
                        owned.leftShoe[id]=left&&y>=p.feet[1]-.15f*(p.bbox[3]-p.bbox[1]);
                        owned.rightShoe[id]=!left&&y>=p.feet[3]-.15f*(p.bbox[3]-p.bbox[1]);
                    }
                    if(i+1==xs.Count||j+1==ys.Count)continue;
                    triangles[k++]=(ushort)id;triangles[k++]=(ushort)(id+xs.Count);triangles[k++]=(ushort)(id+xs.Count+1);
                    triangles[k++]=(ushort)id;triangles[k++]=(ushort)(id+xs.Count+1);triangles[k++]=(ushort)(id+1);
                }
                owned.triangles=triangles;sprite.OverrideGeometry(positions,triangles);
                owned.hulls=new int[2][];
                for(int side=0;side<2;side++){
                    var hh=side==0?p.hullLeft:p.hullRight;owned.hulls[side]=new int[hh.Length/2];
                    for(int j=0;j<hh.Length;j+=2){int ix=xs.FindIndex(v=>Mathf.Abs(hh[j]-v)<.002f),iy=ys.FindIndex(v=>Mathf.Abs(hh[j+1]-v)<.002f);
                        ReusablePaintRigV37.Need(ix>=0&&iy>=0,"Registered painted sole mesh witness");owned.hulls[side][j/2]=iy*xs.Count+ix;}
                }
                owned.positions=new NativeArray<Vector3>(n,Allocator.Persistent,NativeArrayOptions.UninitializedMemory);
                cache.Add(index,owned);return owned;
            }catch{if(owned.positions.IsCreated)owned.positions.Dispose();if(sprite)destroy(sprite);throw;}
        }
        static Vector2 OriginalContact(ReusablePaintKeyV37 p,int s){
            return new Vector2((p.shoeCenters[2*s]-p.pivot[0])*p.unit,(p.pivot[1]-p.shoeCenters[2*s+1])*p.unit);
        }
        internal void Apply(ReusablePaintRigV37.Frame f,Vector3 foot,int facing){
            var key=art.keys[f.key];var pose=GetPose(f.key);var tr=f.track;
            var dl=Vector2.zero;var dr=Vector2.zero;var kl=Vector2.zero;var kr=Vector2.zero;var hips=Vector2.zero;
            if(f.move){
                lastRootSoles[0]=tr.XY(f.clock,0);lastRootSoles[1]=tr.XY(f.clock,6);
                dl=lastRootSoles[0]-OriginalContact(key,0);dr=lastRootSoles[1]-OriginalContact(key,1);
                kl=tr.XY(f.clock,2)-tr.XY(f.reference,2);kr=tr.XY(f.clock,8)-tr.XY(f.reference,8);
                hips=tr.XY(f.clock,18)-tr.XY(f.reference,18);
            }
            Vector2 lo=new Vector2(float.PositiveInfinity,float.PositiveInfinity),hi=new Vector2(float.NegativeInfinity,float.NegativeInfinity);
            for(int i=0;i<pose.basePoints.Length;i++){
                var w=f.move?pose.weights[i]:Vector2.zero;float blend=Mathf.Clamp01(w.x+w.y);
                var v=pose.basePoints[i]+hips*(1-blend)+w.x*dl+w.y*dr;
                if(blend>0&&blend<1)v+=4*(1-blend)*(w.x*kl+w.y*kr);
                pose.positions[i]=new Vector3(v.x,v.y,0);lo=Vector2.Min(lo,v);hi=Vector2.Max(hi,v);
            }
            pose.sprite.SetVertexAttribute(VertexAttribute.Position,pose.positions);
            root.transform.position=foot;root.transform.localRotation=Quaternion.identity;
            root.transform.localScale=new Vector3(facing,carrier.flipY?-1:1,1);
            painted.sprite=pose.sprite;painted.sortingLayerID=carrier.sortingLayerID;painted.sortingOrder=carrier.sortingOrder;
            painted.sharedMaterial=carrier.sharedMaterial;painted.color=carrier.color;painted.maskInteraction=carrier.maskInteraction;
            carrier.GetPropertyBlock(block);var tex=pose.sprite.texture;
            block.SetTexture("_MainTex",tex);block.SetVector("_MainTex_TexelSize",new Vector4(1f/tex.width,1f/tex.height,tex.width,tex.height));
            painted.SetPropertyBlock(block);painted.localBounds=new Bounds((lo+hi)/2,new Vector3(hi.x-lo.x,hi.y-lo.y,.02f));
            visibleTop=root.transform.TransformPoint(new Vector3(0,(key.pivot[1]-key.bbox[1])*key.unit+hips.y,0)).y;
            root.SetActive(carrier.enabled&&carrier.gameObject.activeInHierarchy);carrier.forceRenderingOff=true;Last=f;
        }
        internal Vector2 ActualSole(int side){
            var pose=cache[Last.key];var vertices=pose.sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position);
            var hull=pose.hulls[side];ReusablePaintRigV37.Need(hull.Length>=3,"two painted boot contours");
            float lowest=float.MaxValue;foreach(int i in hull)lowest=Math.Min(lowest,vertices[i].y);
            double sum=0,mass=0;
            foreach(int i in hull){var v=vertices[i];double w=Math.Exp(-(v.y-lowest)/.22);sum+=w*v.x;mass+=w;}
            return new Vector2((float)(sum/mass),lowest);
        }
#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
        internal void ShowShoeInCheck(int side,bool visible){
            var pose=cache[Last.key];var selected=pose.triangles;
            if(!visible){
                var shoe=side==0?pose.leftShoe:pose.rightShoe;
                var keep=new List<ushort>(selected.Length);
                for(int i=0;i<selected.Length;i+=3){
                    if(shoe[selected[i]]&&shoe[selected[i+1]]&&shoe[selected[i+2]])continue;
                    keep.Add(selected[i]);keep.Add(selected[i+1]);keep.Add(selected[i+2]);
                }
                ReusablePaintRigV37.Need(keep.Count<selected.Length&&keep.Count>0,"Selective real shoe triangles");
                selected=keep.ToArray();
            }
            using(var indices=new NativeArray<ushort>(selected,Allocator.Temp))
                pose.sprite.SetIndices(indices);
        }
#endif
        public void Dispose(){
            if(disposed)return;disposed=true;if(root)root.SetActive(false);if(carrier)carrier.forceRenderingOff=priorOff;
            if(painted)painted.sprite=null;
            foreach(var pose in cache.Values){if(pose.positions.IsCreated)pose.positions.Dispose();if(pose.sprite)destroy(pose.sprite);}
            cache.Clear();if(root){root.transform.SetParent(null,false);destroy(root);}
        }
    }
}
