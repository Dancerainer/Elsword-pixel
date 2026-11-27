#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    internal sealed class DirectorSurfaceV41 {
        internal Renderer renderer;internal int slot;
        internal Sprite originalSprite,selectedSprite;internal Texture2D original,painted;
    }
    public sealed partial class DemoScene {
        internal bool DirectorSubjectsV41(bool dragon,List<DirectorSurfaceV41> surfaces,out Camera camera,out float age,out int sample){
            surfaces.Clear();
            if(dragon){
                camera=worldCamera;age=DragonNestDirectorAgeV20;sample=DragonNestDirectorFrameIndexV20;
                if(!DragonNestDirectorActiveV20||!dragonDirectorSourceFrameV41||!dragonDirectorRendererV20.sprite||
                   dragonDirectorRendererV20.sprite.texture!=dragonDirectorPaintV41)return false;
                surfaces.Add(new DirectorSurfaceV41{renderer=dragonDirectorRendererV20,originalSprite=dragonDirectorSourceFrameV41,
                    selectedSprite=dragonDirectorRendererV20.sprite,original=dragonDirectorOriginalV41,painted=dragonDirectorPaintV41});
                return camera;
            }
            camera=null;age=0;sample=-1;
            return battleProductViewR1325!=null&&battleProductViewR1325.DirectorSubjectsV41(surfaces,out camera,out age,out sample);
        }
        internal bool ReleasedDirectorPaintV41(bool dragon){
            if(dragon)return !DragonNestDirectorActiveV20&&dragonDirectorSpritesV41.Count==0&&!dragonDirectorPaintV41&&!dragonDirectorOriginalV41;
            return battleProductViewR1325==null;
        }
        sealed partial class BattleProductViewR1325 {
            internal bool DirectorSubjectsV41(List<DirectorSurfaceV41> surfaces,out Camera camera,out float age,out int sample){
                camera=airshipDirectorNativeCameraV20;age=AlteraAirshipDirectorAgeV20;
                sample=Mathf.FloorToInt(age*60);
                if(!AlteraAirshipDirectorActiveV20||!camera)return false;
                foreach(var view in airshipDirectorViewsV20){
                    if(!view.renderer||!view.renderer.enabled||!view.renderer.gameObject.activeInHierarchy)continue;
                    var materials=view.renderer.sharedMaterials;
                    for(int i=0;i<view.sourceTexturesV41.Length;i++){
                        if(!view.sourceTexturesV41[i])continue;
                        var painted=view.materials[i].mainTexture as Texture2D;
                        if(!painted||i>=materials.Length||!materials[i]||materials[i].mainTexture!=painted)return false;
                        surfaces.Add(new DirectorSurfaceV41{renderer=view.renderer,slot=i,original=view.sourceTexturesV41[i],painted=painted});
                    }
                }
                return surfaces.Count==8;
            }
        }
    }
}
#endif

