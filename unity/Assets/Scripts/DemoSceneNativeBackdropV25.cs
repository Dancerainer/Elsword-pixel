using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        Material nativeBackdropMaterialV25;

        Material CreateNativeBackdropMaterialV25() {
            var shader=Resources.Load<Shader>("Pixel/Shaders/NativeScreenBackdropV25");
            if(!shader)throw new InvalidOperationException("Missing native screen backdrop shader");
            nativeBackdropMaterialV25=new Material(shader);
            return nativeBackdropMaterialV25;
        }

        void NativeClearCoverageV25() {
            if(!nativeCamera)return;
            // RGB is retained, including the existing below-tunnel override.
            // Only the clear coverage bit changes; native geometry writes 1.
            var color=nativeCamera.backgroundColor;color.a=0;nativeCamera.backgroundColor=color;
        }

        static Color NativeBackdropPaletteV25(string id) {
            // Exact colors observed in each existing r6 native scene palette.
            if(id!=null&&id.StartsWith("tree-",StringComparison.Ordinal))return new Color32(18,106,63,255);
            if(id!=null&&id.StartsWith("ruins-",StringComparison.Ordinal))return new Color32(59,95,46,255);
            if(id!=null&&id.StartsWith("swamp-",StringComparison.Ordinal))return new Color32(36,69,51,255);
            if(id!=null&&id.StartsWith("forest-",StringComparison.Ordinal))return new Color32(9,24,49,255);
            if(id!=null&&id.StartsWith("cave-",StringComparison.Ordinal))return new Color32(17,51,82,255);
            if(id!=null&&id.StartsWith("waterway-",StringComparison.Ordinal))return new Color32(53,30,20,255);
            return new Color32(32,43,42,255);
        }

        void UpdateNativeBackdropV25() {
            if(!NativeWorldActive||!nativeBackdropMaterialV25||!worldCamera)return;
            var c=worldCamera.transform.position;
            float bottom=cameraGeometryValidV25?cameraGeometryBottomV25:game.GroundAt(RenderPlayerWorldPosition.x)-24;
            // Three local geometry columns need a small overlap at view-edge seams.
            // Coverage still excludes every foreground fragment.
            bottom += 8;
            // Use the actual native projection, including authored directors.
            // The fragment test also requires no native geometry coverage.
            var projected=ProjectNativePoint(new Vector3(c.x,bottom,0));
            float threshold=Mathf.Clamp01(.5f+(projected.y-c.y)/CameraLogicalHeightV25);
            nativeBackdropMaterialV25.SetFloat("_GroundViewportY",threshold);
            var belowColor=NativeBackdropPaletteV25(CurrentStageArt.id);
            if(regionalSession||CameraFramingPolicyV25.ActualLowerScopeV28(CurrentStageArt.id)){belowColor=SceneBackdropPolicyV25.ClearColor(nativeScene.id,belowColor);belowColor.a=1;}
            nativeBackdropMaterialV25.SetColor("_BelowGroundColor",belowColor);
            nativeBackdropMaterialV25.SetFloat("_BackdropEnabled",cameraFramingReviewOriginalV25?0:1);
        }
    }
}
