using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const string FlockRootV37="Pixel/feedback-v37/tree-flock/";
        [Serializable] internal sealed class FlockPartV37 { public int part; public bool whole; public float[] weights; }
        [Serializable] internal sealed class FlockHeadV37 { public int root; public float widen,height; public FlockPartV37[] parts; }
        [Serializable] internal sealed class FlockSkinV37 { public FlockHeadV37[] groups; }

        // Component 17 already uses the separately approved six-part A1 sprite.
        // The other mother and four chicks keep their own native poses and travel.
        static void LoadFemaleFlockV37(FemaleNativeDisplayV28 display){
            var registration=Resources.Load<TextAsset>(FlockRootV37+"registration");
            var mother=Resources.Load<Texture2D>(FlockRootV37+"mother");
            var chick=Resources.Load<Texture2D>(FlockRootV37+"chick");
            var ink=Resources.Load<Shader>(FlockRootV37+"TreeFlockV37");
            if(!registration||!mother||!chick||!ink||!ink.isSupported)return;
            var shape=JsonUtility.FromJson<FlockSkinV37>(registration.text);
            if(shape==null||shape.groups==null||shape.groups.Length!=5)throw new InvalidOperationException("Invalid tree flock registration");
            foreach(var g in shape.groups){
                if(g.widen<1||g.widen>1.2f||g.height<1||g.height>1.05f||g.parts==null||g.parts.Length==0)
                    throw new InvalidOperationException("Invalid tree flock head proportions");
                foreach(var p in g.parts){
                    var m=display.data.parts[p.part];
                    if(m.componentRootIndex!=g.root||g.root==17||(!p.whole&&(p.weights==null||p.weights.Length!=m.count)))
                        throw new InvalidOperationException("Tree flock registration does not match the original mesh");
                }
            }
            display.flockShapeV37=shape;
            display.flockMaterialsV37=new[]{mother,chick};display.flockShaderV37=ink;
        }

        static void UpdateFlockInkOriginsV37(FemaleNativeDisplayV28 display){
            if(display.flockShapeV37==null)return;
            foreach(var group in display.flockShapeV37.groups){
                var bounds=display.meshes[group.parts[0].part].bounds;
                for(int i=0;i<display.meshes.Length;i++)if(display.data.parts[i].componentRootIndex==group.root)
                    bounds.Encapsulate(display.meshes[i].bounds);
                for(int i=0;i<display.materials.Length;i++)if(display.data.parts[i].componentRootIndex==group.root)
                    display.materials[i].SetVector("_InkCenter",bounds.center);
            }
        }
        static void ShapeFemaleFlockHeadsV37(FemaleNativeDisplayV28 display){
            var shape=display.flockShapeV37;if(shape==null)return;
            foreach(var group in shape.groups){
                // A centroid of skinned vertices follows the source in 3D, even while
                // a chick tilts or turns its back. Every foot vertex has zero weight.
                Vector3 center=Vector3.zero;float total=0;
                foreach(var part in group.parts){
                    var points=display.positions[part.part];
                    for(int i=0;i<points.Length;i++){float w=part.whole?1:part.weights[i];center+=points[i]*w;total+=w;}
                }
                if(total<=0)throw new InvalidOperationException("Empty tree flock head registration");
                center/=total;
                foreach(var part in group.parts){
                    var points=display.positions[part.part];
                    for(int i=0;i<points.Length;i++){
                        float w=part.whole?1:part.weights[i];if(w<=0)continue;
                        var offset=points[i]-center;
                        offset.x*=(group.widen-1)*w;offset.z*=(group.widen-1)*w;offset.y*=(group.height-1)*w;
                        points[i]+=offset;
                    }
                }
            }
        }
    }
}

