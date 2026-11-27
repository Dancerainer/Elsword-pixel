#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class AirshipStateFilePinV20 {public string path,sha256;}
  [Serializable] sealed class AirshipStateBindingFileV20 {public string schema;public bool runtimeAdmitted;public AirshipStateFilePinV20 document;public AirshipStateFilePinV20[] sourceInputs;public AirshipStateTextureV20[] textures;}
  // A1 declares these private Editor-only bridges. An absent implementation
  // leaves its ref bool false; core APIs are independent of PreviewFile.
  partial void ConfigureAlteraAirshipStateRuntimeV20(PreviewFile binding,ref bool configured){
   configured=false;var b=AirshipJsonV20<AirshipStateBindingFileV20>(binding);
   PreviewRequire(b!=null&&b.schema=="altera-airship-state-runtime-binding-v20"&&!b.runtimeAdmitted&&b.sourceInputs!=null&&b.sourceInputs.Length==8&&b.textures!=null&&b.textures.Length==AirshipStatePinsV20.Textures,"Missing/changed Airship A2 source binding");
   foreach(var p in b.sourceInputs)AirshipReadV20(new PreviewFile{path=p.path,sha256=p.sha256});
   var doc=AirshipJsonV20<AlteraAirshipStateDocumentV20>(new PreviewFile{path=b.document.path,sha256=b.document.sha256});var textures=new Dictionary<string,Texture2D>();
   foreach(var row in b.textures){PreviewRequire(!textures.ContainsKey(row.key),"Duplicate A2 source texture");textures.Add(row.key,PreviewTextureFile(new PreviewFile{path=row.path,sha256=row.sha256}));}
   ConfigureAlteraAirshipStateVisualV20(doc,textures);configured=true;
  }
  partial void DisposeAlteraAirshipStateRuntimeV20(){DisposeAlteraAirshipStateVisualV20();}
 }
}
#endif
