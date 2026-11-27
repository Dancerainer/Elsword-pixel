using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  void ConfigurePackagedCastleCenterVisual(NpcContentSetData set){
   var binding=set==null?null:set.outskirtsVisual;
   if(set==null||set.sourceDungeon!=CastleCenterSource)return;
   RequireOutskirtsVisual(castleCenterIterationApproved&&binding!=null&&binding.textures!=null&&binding.textures.Length==11,"Castle Center original visible effects missing");
   var document=Resources.Load<TextAsset>(binding.documentResource);var mesh=Resources.Load<TextAsset>(binding.meshManifestResource);RequireOutskirtsVisual(document&&mesh,"Castle Center visual documents missing");
   var textures=new Dictionary<string,Texture2D>();foreach(var row in binding.textures){var texture=Resources.Load<Texture2D>(row.resource);RequireOutskirtsVisual(texture&&!textures.ContainsKey(row.key),"Castle Center original visual texture missing");textures.Add(row.key,texture);}
   ConfigureOutskirtsNpcVisual(JsonUtility.FromJson<OutskirtsNpcVisualDocument>(document.text),JsonUtility.FromJson<OutskirtsNpcMeshManifest>(mesh.text),textures);
  }
 }
}
