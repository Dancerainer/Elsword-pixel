using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using System.IO;

namespace ElswordPixel {
 [Serializable]public sealed class CoreHardLocalAddVisualRowV21 {public string template,key,particleJson,particleJsonSha256,textureKey,rgbaSha256;public float[] sourceVectorA,sourceVectorB;}
 [Serializable]public sealed class CoreHardLocalAddVisualDocumentV21 {public string schema,vectorAdapter;public bool runtimeAdmitted,actualUnityVerified,nativeEquivalent;public float unit;public CoreHardLocalAddVisualRowV21[] rows;}
 public sealed partial class DemoScene {
  CoreHardLocalAddVisualDocumentV21 coreHardLocalAddVisualDocumentV21;
  readonly HashSet<CoreHardLocalAddEventV21> coreHardLocalAddVisualSeenV21=new HashSet<CoreHardLocalAddEventV21>();
  readonly List<Texture2D> coreHardLocalAddVisualLoadedTexturesV21=new List<Texture2D>();
  Transform coreHardLocalAddVisualWorldV21,coreHardLocalAddVisualRootV21;int coreHardLocalAddVisualEpochV21=-1,coreHardLocalAddVisualRoomV21=-1;float coreHardLocalAddVisualTimeV21=-1;
  public bool CoreHardLocalAddVisualConfiguredV21{get{return coreHardLocalAddVisualDocumentV21!=null&&CoreHardClosureVisualConfiguredV21;}}
  public int CoreHardLocalAddVisualRequestsV21{get;private set;}
  public void ConfigureCoreHardLocalAddVisualV21(CoreHardLocalAddVisualDocumentV21 d,IDictionary<string,Texture2D> textures){
   CoreHardClosureVisualRequireV21(CoreHardClosureVisualConfiguredV21&&coreHardLocalAddVisualDocumentV21==null,"Load exact Core main visual first; reload both documents to replace add visuals");
   CoreHardClosureVisualRequireV21(d!=null&&d.schema=="core-hard-local-add-source-visual-v21"&&d.vectorAdapter=="retain-original-call-vector-pairs-source-template-curves-only-unmeasured-native-v21"&&d.unit==.28f&&!d.runtimeAdmitted&&!d.actualUnityVerified&&!d.nativeEquivalent&&d.rows!=null&&d.rows.Length==3&&textures!=null&&textures.Count==3,"Changed exact add display contract");
   var exact=new HashSet<string>(new[]{"Minor/TimeStop1","Major/Heal_Effect01","Major/LobbyLevelUp01"},StringComparer.Ordinal);var views=new Dictionary<string,CoreHardClosureVisualViewV21>();var materials=new List<Material>();
   try{
    foreach(var row in d.rows){
     CoreHardClosureVisualRequireV21(row!=null&&exact.Remove(row.key)&&!coreHardClosureVisualViewsV21.ContainsKey(row.key)&&CoreHardClosureVisualHashV21(Encoding.UTF8.GetBytes(row.particleJson))==row.particleJsonSha256,"Changed original add particle blob/key");
     bool minor=row.key=="Minor/TimeStop1";CoreHardClosureVisualRequireV21(row.template==(minor?"TimeStop1":row.key.Substring(6))&&row.sourceVectorA!=null&&row.sourceVectorA.Length==2&&row.sourceVectorB!=null&&row.sourceVectorB.Length==2&&row.sourceVectorA[0]==(minor?100:-1)&&row.sourceVectorA[1]==(minor?100:-1)&&row.sourceVectorB[0]==(minor?1:-1)&&row.sourceVectorB[1]==-1,"Changed exact original call vector arguments");
     var p=JsonUtility.FromJson<OutskirtsVisualParticle>(row.particleJson);ValidateLakeParticleV18(p);Texture2D t;
     CoreHardClosureVisualRequireV21(p.id==row.key&&p.texture==row.textureKey&&p.particleType=="PT_3D_PLANE"&&p.billboardType=="BT_ALL"&&textures.TryGetValue(row.textureKey,out t)&&t&&t.isReadable,"Unbound original add plane texture/basis");t=textures[row.textureKey];
     CoreHardClosureVisualRequireV21(CoreHardClosureVisualRgbaHashV21(t)==row.rgbaSha256,"Changed original add texture RGBA");t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;
     var material=OutskirtsMaterial(p.sourceBlend,p.destinationBlend);outskirtsMaterials.Remove(material);materials.Add(material);views.Add(row.key,new CoreHardClosureVisualViewV21{data=p,sprite=PixelExplosionVisuals.Plane(p,t),material=material,held=false});
    }
    CoreHardClosureVisualRequireV21(exact.Count==0,"Incomplete exact three add templates");foreach(var p in views)coreHardClosureVisualViewsV21.Add(p.Key,p.Value);coreHardClosureVisualMaterialsV21.AddRange(materials);
   }catch{foreach(var p in views.Values)CoreHardClosureVisualDestroyV21(p.sprite);foreach(var m in materials)CoreHardClosureVisualDestroyV21(m);throw;}
   coreHardLocalAddVisualDocumentV21=d;CoreHardLocalAddVisualRequestsV21=0;coreHardLocalAddVisualSeenV21.Clear();foreach(var e in game.coreHardLocalAddEventsV21)coreHardLocalAddVisualSeenV21.Add(e);coreHardLocalAddVisualEpochV21=game.CoreHardClosureEpochV21;coreHardLocalAddVisualRoomV21=game.room;coreHardLocalAddVisualWorldV21=worldRoot;coreHardLocalAddVisualRootV21=effectsRoot;coreHardLocalAddVisualTimeV21=game.elapsed;
  }
  void DisposeCoreHardLocalAddVisualV21(){coreHardLocalAddVisualDocumentV21=null;coreHardLocalAddVisualSeenV21.Clear();foreach(var t in coreHardLocalAddVisualLoadedTexturesV21)CoreHardClosureVisualDestroyV21(t);coreHardLocalAddVisualLoadedTexturesV21.Clear();coreHardLocalAddVisualEpochV21=coreHardLocalAddVisualRoomV21=-1;coreHardLocalAddVisualTimeV21=-1;coreHardLocalAddVisualWorldV21=coreHardLocalAddVisualRootV21=null;CoreHardLocalAddVisualRequestsV21=0;}
  void CollectCoreHardLocalAddVisualV21(){
   if(coreHardLocalAddVisualDocumentV21==null)return;
   bool newEpoch=coreHardLocalAddVisualEpochV21!=game.CoreHardClosureEpochV21||coreHardLocalAddVisualRoomV21!=game.room;
   bool replacement=coreHardLocalAddVisualWorldV21!=worldRoot||coreHardLocalAddVisualRootV21!=effectsRoot||game.elapsed+1e-6f<coreHardLocalAddVisualTimeV21;
   if(newEpoch||replacement){coreHardLocalAddVisualSeenV21.Clear();if(!newEpoch)foreach(var e in game.coreHardLocalAddEventsV21)coreHardLocalAddVisualSeenV21.Add(e);}
   coreHardLocalAddVisualEpochV21=game.CoreHardClosureEpochV21;coreHardLocalAddVisualRoomV21=game.room;coreHardLocalAddVisualWorldV21=worldRoot;coreHardLocalAddVisualRootV21=effectsRoot;coreHardLocalAddVisualTimeV21=game.elapsed;
   coreHardLocalAddVisualSeenV21.IntersectWith(game.coreHardLocalAddEventsV21);
   foreach(var e in game.coreHardLocalAddEventsV21){
    if(!coreHardLocalAddVisualSeenV21.Add(e)||e.kind!="source-particle-request-HOLD")continue;
    CoreHardClosureVisualRequireV21(game.CoreHardConfiguredV21&&game.CoreHardSourceNpcLevelV21==37&&game.CoreHardLocalBalanceConfiguredV21&&game.CoreHardLocalAddsConfiguredV21&&game.NativeTerrain&&game.room>=0&&game.room<5&&game.CurrentRoom.id=="Dungeon_Altera_Core_Hard-"+game.room&&e.npc=="NUI_NASOD_HEALER_BLADER"&&e.sourceCallback=="NASOD_HEALER_BLADER_HEAL_FRAME_MOVE","Unreviewed add particle event source");
    var row=Array.Find(coreHardLocalAddVisualDocumentV21.rows,r=>r.template==e.particle);CoreHardClosureVisualRequireV21(row!=null,"Unknown source add template request");
    var floor=game.SurfaceBelow(e.x,e.y+.05f);float land=floor==null?e.y:floor.Y(e.x);
    var sequence=AddCoreHardClosureSequenceV21(row.key,e.at,new Vector3(e.x,e.y,0),e.z/.28f,land,1,.28f,unchecked(++CoreHardLocalAddVisualRequestsV21*997+CoreHardClosureVisualSeedV21(row.key)));
    // Match the accepted Airship/Lake finite-sequence adapter: an untriggered
    // count is distributed at the source rate; no source curve is rewritten.
    if(sequence!=null&&sequence.view.data.triggerTime==0)sequence.per=1;
   }
  }

  [Serializable]sealed class CoreHardLocalAddVisualBindingV21 {public string schema;public bool runtimeAdmitted,actualUnityVerified,nativeEquivalent;public CoreHardClosureVisualFilePinV21 document,sourceContract;public CoreHardClosureVisualTexturePinV21[] textures;}
  public void LoadCoreHardLocalAddVisualBindingV21(string path,string expectedSha256){
   var binding=JsonUtility.FromJson<CoreHardLocalAddVisualBindingV21>(Encoding.UTF8.GetString(CoreHardClosureVisualReadPinV21(new CoreHardClosureVisualFilePinV21{path=path,sha256=expectedSha256})));
   CoreHardClosureVisualRequireV21(binding!=null&&binding.schema=="core-hard-local-add-source-visual-binding-v21"&&!binding.runtimeAdmitted&&!binding.actualUnityVerified&&!binding.nativeEquivalent&&binding.textures!=null&&binding.textures.Length==3,"Changed original add visual binding");if(!CoreProductResourcesActiveR1147)CoreHardClosureVisualReadPinV21(binding.sourceContract);
   var d=JsonUtility.FromJson<CoreHardLocalAddVisualDocumentV21>(Encoding.UTF8.GetString(CoreHardClosureVisualReadPinV21(binding.document)));var textures=new Dictionary<string,Texture2D>();var owned=new List<Texture2D>();
   try{foreach(var r in binding.textures){var bytes=CoreHardClosureVisualReadPinV21(new CoreHardClosureVisualFilePinV21{path=r.path,sha256=r.sha256});var t=new Texture2D(2,2,TextureFormat.RGBA32,false);owned.Add(t);CoreHardClosureVisualRequireV21(ImageConversion.LoadImage(t,bytes,false),"Original add source PNG load failed");textures.Add(r.key,t);}ConfigureCoreHardLocalAddVisualV21(d,textures);coreHardLocalAddVisualLoadedTexturesV21.AddRange(owned);owned.Clear();}finally{foreach(var t in owned)CoreHardClosureVisualDestroyV21(t);}
  }

 }
}
