using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const int CameraBlockVertices=48;
        static readonly string[] UnframedPartsV25={"sky","cloud","shadow","glow","fog","flare","lightalpha","lightmap","colormap","lightbeam","sunbeam"};
        sealed class CameraGeometryPartV25 {public Transform transform;public Vector3[] vertices;public Material material;public Bounds[] blocks;}
        readonly List<CameraGeometryPartV25> cameraGeometryPartsV25=new List<CameraGeometryPartV25>();
        readonly float[] cameraGeometryProbeV25=new float[3],cameraGeometryBottomsV25=new float[3];
        float cameraGeometryLastXv25=float.NaN,cameraGeometryLastPlayerXv25,cameraGeometryLastFloorV25,cameraGeometryBottomV25;
        int cameraGeometrySamplesV25;
        bool cameraGeometryValidV25;

        void InvalidateCameraGeometryV25(){cameraGeometryLastXv25=float.NaN;cameraGeometryValidV25=false;cameraGeometrySamplesV25=0;}
        void ResetCameraGeometryV25(){cameraGeometryPartsV25.Clear();InvalidateCameraGeometryV25();}
        static Bounds[] BuildCameraGeometryBlocksV25(Vector3[] vertices,string name){
            string id=(name??"").ToLowerInvariant();
            foreach(string token in UnframedPartsV25)if(id.Contains(token))return Array.Empty<Bounds>();
            // Mesh vertices arrive in source triangle order. Keep exact triangle
            // intersections, but reject whole blocks outside the 3 camera columns.
            var blocks=new Bounds[(vertices.Length+CameraBlockVertices-1)/CameraBlockVertices];
            for(int k=0;k<blocks.Length;k++){
                int start=k*CameraBlockVertices,end=Math.Min(start+CameraBlockVertices,vertices.Length);
                var bounds=new Bounds(vertices[start],Vector3.zero);
                for(int i=start+1;i<end;i++)bounds.Encapsulate(vertices[i]);blocks[k]=bounds;
            }
            return blocks;
        }
        void RegisterCameraGeometryV25(Transform transform,Vector3[] vertices,Material material,string name,Bounds[] prepared=null){
            var blocks=prepared??BuildCameraGeometryBlocksV25(vertices,name);
            if(blocks.Length!=0)cameraGeometryPartsV25.Add(new CameraGeometryPartV25{transform=transform,vertices=vertices,material=material,blocks=blocks});
        }
        CameraFramingPolicyV25.Point CameraGeometryPointV25(Vector3 vertex,Vector3 at,Vector3 right){
            return new CameraFramingPolicyV25.Point(Vector3.Dot(vertex-at,right)*nativeScene.unit,vertex.y*nativeScene.unit+nativeScene.offsetY);
        }
        bool TryCameraGeometryBottomV25(float centerX,float playerX,float floor,out float bottom){
            bottom=0;
            // Authored perspective eye/look tracks retain their separate source
            // projection. Their lens is still bounded by the gameplay height cap.
            if(!NativeWorldActive||CameraDirectorActiveV25||cameraGeometryPartsV25.Count==0){cameraGeometryValidV25=false;cameraGeometrySamplesV25=0;return false;}
            if(!float.IsNaN(cameraGeometryLastXv25)&&Mathf.Abs(centerX-cameraGeometryLastXv25)<8&&Mathf.Abs(playerX-cameraGeometryLastPlayerXv25)<8&&Mathf.Abs(floor-cameraGeometryLastFloorV25)<4){bottom=cameraGeometryBottomV25;return cameraGeometryValidV25;}
            cameraGeometryLastXv25=centerX;cameraGeometryLastPlayerXv25=playerX;cameraGeometryLastFloorV25=floor;
            Vector3 at=NativePathPoint(centerX),normal=NativeNormal(centerX),right=Vector3.Cross(Vector3.up,-normal).normalized;
            for(int i=0;i<3;i++){
                float station=Mathf.Clamp(playerX+(i-1)*96,0,game.RoomWidth);
                cameraGeometryProbeV25[i]=Vector3.Dot(NativePathPoint(station)-at,right)*nativeScene.unit;
                cameraGeometryBottomsV25[i]=float.PositiveInfinity;
            }
            foreach(var part in cameraGeometryPartsV25){
                if(!part.transform)continue;var vertices=part.vertices;var matrix=part.transform.localToWorldMatrix;
                int cull=part.material&&part.material.HasProperty("_Cull")?Mathf.RoundToInt(part.material.GetFloat("_Cull")):0;
                Vector3 axis=new Vector3(right.x*matrix.m00+right.y*matrix.m10+right.z*matrix.m20,right.x*matrix.m01+right.y*matrix.m11+right.z*matrix.m21,right.x*matrix.m02+right.y*matrix.m12+right.z*matrix.m22);
                Vector3 absAxis=new Vector3(Mathf.Abs(axis.x),Mathf.Abs(axis.y),Mathf.Abs(axis.z));
                for(int block=0;block<part.blocks.Length;block++){
                 var bounds=part.blocks[block];var center=matrix.MultiplyPoint3x4(bounds.center);
                 var point=CameraGeometryPointV25(center,at,right);
                 float radiusX=Vector3.Dot(absAxis,bounds.extents)*nativeScene.unit+.1f;
                 if(Mathf.Abs(point.x-cameraGeometryProbeV25[0])>radiusX&&Mathf.Abs(point.x-cameraGeometryProbeV25[1])>radiusX&&Mathf.Abs(point.x-cameraGeometryProbeV25[2])>radiusX)continue;
                 int end=Math.Min(vertices.Length,(block+1)*CameraBlockVertices);
                 for(int i=block*CameraBlockVertices;i+2<end;i+=3){
                    Vector3 a=matrix.MultiplyPoint3x4(vertices[i]),b=matrix.MultiplyPoint3x4(vertices[i+1]),c=matrix.MultiplyPoint3x4(vertices[i+2]);
                    float facing=Vector3.Dot(Vector3.Cross(b-a,c-a),-normal);
                    if((cull==1&&facing<0)||(cull==2&&facing>0))continue;
                    float depth=2200-Vector3.Dot((a+b+c)/3-at,normal);if(depth<1||depth>100000)continue;
                    var pa=CameraGeometryPointV25(a,at,right);var pb=CameraGeometryPointV25(b,at,right);var pc=CameraGeometryPointV25(c,at,right);
                    for(int k=0;k<3;k++)cameraGeometryBottomsV25[k]=Mathf.Min(cameraGeometryBottomsV25[k],CameraFramingPolicyV25.GeometryBottomAt(pa,pb,pc,cameraGeometryProbeV25[k],floor));
                 }
                }
            }
            cameraGeometrySamplesV25=0;bottom=float.NegativeInfinity;
            for(int i=0;i<3;i++)if(!float.IsInfinity(cameraGeometryBottomsV25[i])){bottom=Mathf.Max(bottom,cameraGeometryBottomsV25[i]);cameraGeometrySamplesV25++;}
            cameraGeometryValidV25=cameraGeometrySamplesV25>0;cameraGeometryBottomV25=bottom;
            return cameraGeometryValidV25;
        }
    }
}
