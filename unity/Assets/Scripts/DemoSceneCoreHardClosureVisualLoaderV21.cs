using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class CoreHardClosureVisualFilePinV21 {public string path,sha256;}
 [Serializable] public sealed class CoreHardClosureVisualTexturePinV21 {public string key,path,sha256;}

 [Serializable] public sealed class CoreHardClosureVisualBindingV21 {
  public string schema;public bool runtimeAdmitted,actualUnityVerified,nativeEquivalent;
  public CoreHardClosureVisualFilePinV21 document,meshManifest,sourceEffects,sourceRules,sourceContract;
  public CoreHardClosureVisualTexturePinV21[] textures;public CoreHardClosureVisualHoldV21[] declaredHolds;
 }
 [Serializable] public sealed class CoreHardClosureVisualInspectionV21 {
  public string schema="core-hard-closure-source-visual-inspection-v21",bindingSha256;
  public bool completeDefinitionCoverage,allHoldsExplicit,sourceDefinitionsUnchanged,nativeEquivalent=false;
  public int definitions,mainMeshes,particleTemplates,effectFields,passives,supportedFields,heldFields,heldTemplates;
  public CoreHardClosureVisualHoldV21[] holds;
 }
 public sealed partial class DemoScene {
  readonly List<Texture2D> coreHardClosureVisualLoadedTexturesV21=new List<Texture2D>();
  partial void DisposeCoreHardClosureVisualExternalTexturesV21(){foreach(var t in coreHardClosureVisualLoadedTexturesV21)CoreHardClosureVisualDestroyV21(t);coreHardClosureVisualLoadedTexturesV21.Clear();}
  static byte[] CoreHardClosureVisualReadPinV21(CoreHardClosureVisualFilePinV21 p){if(CoreProductResourcesActiveR1147){CoreHardClosureVisualRequireV21(p!=null,"Runtime source binding absent");return coreProductReadIndexR1147.Read(p.path,p.sha256);}CoreHardClosureVisualRequireV21(p!=null&&Path.IsPathRooted(p.path),"External source path must be absolute");byte[] b;if(CoreProductResourcesActiveR1147)b=coreProductReadIndexR1147.Read(p.path,p.sha256);else{
#if UNITY_EDITOR
 b=File.ReadAllBytes(p.path);
#else
 throw new InvalidOperationException("Core filesystem loader is Editor-only");
#endif
 }CoreHardClosureVisualRequireV21(CoreHardClosureVisualHashV21(b)==p.sha256,"Changed external source file "+p.path);return b;}
  static CoreHardClosureVisualBindingV21 CoreHardClosureVisualBindingReadV21(string path,string expectedSha256){
   var bytes=CoreHardClosureVisualReadPinV21(new CoreHardClosureVisualFilePinV21{path=path,sha256=expectedSha256});var b=JsonUtility.FromJson<CoreHardClosureVisualBindingV21>(Encoding.UTF8.GetString(bytes));
   CoreHardClosureVisualRequireV21(b!=null&&b.schema=="core-hard-closure-source-visual-binding-v21"&&!b.runtimeAdmitted&&!b.actualUnityVerified&&!b.nativeEquivalent&&b.textures!=null&&b.declaredHolds!=null,"Invalid external CoreHard closure binding");return b;
  }
  static string CoreHardClosureVisualHoldKeyV21(CoreHardClosureVisualHoldV21 h){return h.effect+"/"+h.sourceField+"/"+h.particleKey+"/"+h.reason;}
  // Read-only inspection verifies exact bytes and source identities. It does
  // not create Unity objects, change the GameModel, or approve declared HOLDs.
  public static CoreHardClosureVisualInspectionV21 InspectCoreHardClosureVisualBindingV21(string path,string expectedSha256){
   var b=CoreHardClosureVisualBindingReadV21(path,expectedSha256);var d=JsonUtility.FromJson<CoreHardClosureVisualDocumentV21>(Encoding.UTF8.GetString(CoreHardClosureVisualReadPinV21(b.document)));
   CoreHardClosureVisualRequireV21(d!=null&&d.schema=="core-hard-closure-source-visual-v21"&&d.unit==.28f&&!d.runtimeAdmitted&&!d.actualUnityVerified&&d.particles!=null&&d.effectParticles!=null&&d.effectMeshes!=null&&d.effectPassiveParticles!=null,"Malformed source visual document");ValidateCoreHardClosureVisualCoverageV21(d);
   var originalBytes=CoreHardClosureVisualReadPinV21(b.sourceEffects);var original=JsonUtility.FromJson<NpcEffectDocument>(Encoding.UTF8.GetString(originalBytes));CoreHardClosureVisualRequireV21(original!=null&&original.definitions!=null&&original.definitions.Length==14&&CoreHardClosureVisualHashV21(originalBytes)==d.sourceEffectsSha256&&CoreHardClosureVisualHashV21(CoreHardClosureVisualReadPinV21(b.sourceRules))==d.sourceRulesSha256,"Changed original rules/effects");
   foreach(var r in d.definitions){var source=Array.Find(original.definitions,q=>q.id==r.id);CoreHardClosureVisualRequireV21(source!=null&&source.lifetime==r.lifetime&&source.visualFromMesh==r.sourceVisibleMesh&&string.IsNullOrEmpty(source.visualId),"Source identity/clock/visualId altered");}
   var meshes=JsonUtility.FromJson<OutskirtsNpcMeshManifest>(Encoding.UTF8.GetString(CoreHardClosureVisualReadPinV21(b.meshManifest)));CoreHardClosureVisualRequireV21(meshes!=null&&meshes.components!=null&&meshes.components.Length==15,"Missing source mesh components");ValidateCoreHardFiniteDocumentR276(d,meshes);
   var keys=new HashSet<string>();foreach(var t in b.textures){CoreHardClosureVisualRequireV21(t!=null&&!string.IsNullOrEmpty(t.key)&&keys.Add(t.key),"Duplicate source texture key");CoreHardClosureVisualReadPinV21(new CoreHardClosureVisualFilePinV21{path=t.path,sha256=t.sha256});}
   if(!CoreProductResourcesActiveR1147)CoreHardClosureVisualReadPinV21(b.sourceContract);var holds=new List<CoreHardClosureVisualHoldV21>();if(d.mainHolds!=null)holds.AddRange(d.mainHolds);int heldTemplates=0,supported=0;foreach(var p in d.particles){CoreHardClosureVisualRequireV21(CoreHardClosureVisualHashV21(Encoding.UTF8.GetBytes(p.particleJson??""))==p.particleJsonSha256,"Changed original normalized particle");if(!p.supported)heldTemplates++;}
   foreach(var f in d.effectParticles){var p=Array.Find(d.particles,q=>q.key==f.particleKey);CoreHardClosureVisualRequireV21(p!=null,"Missing typed original particle template");if(f.supported&&p.supported)supported++;else holds.Add(new CoreHardClosureVisualHoldV21{effect=f.effect,sourceField=f.sourceField,particleKey=f.particleKey,reason=!f.supported?f.holdReason:p.holdReason});}
   foreach(var f in d.effectPassiveParticles){var p=Array.Find(d.particles,q=>q.key==f.particleKey);CoreHardClosureVisualRequireV21(p!=null,"Missing typed original passive template");if(f.supported&&p.supported)supported++;else holds.Add(new CoreHardClosureVisualHoldV21{effect=f.effect,sourceField=f.sourceField,particleKey=f.particleKey,reason=!f.supported?f.holdReason:p.holdReason});}
   var declared=new HashSet<string>();foreach(var h in b.declaredHolds)CoreHardClosureVisualRequireV21(h!=null&&!string.IsNullOrEmpty(h.reason)&&declared.Add(CoreHardClosureVisualHoldKeyV21(h)),"Ambiguous/empty declared HOLD");foreach(var h in holds)CoreHardClosureVisualRequireV21(declared.Remove(CoreHardClosureVisualHoldKeyV21(h)),"Unreported source HOLD");CoreHardClosureVisualRequireV21(declared.Count==0,"Obsolete/unmatched HOLD declaration");
   return new CoreHardClosureVisualInspectionV21{bindingSha256=expectedSha256,completeDefinitionCoverage=true,allHoldsExplicit=true,sourceDefinitionsUnchanged=true,definitions=d.definitions.Length,mainMeshes=d.effectMeshes.Length,particleTemplates=d.particles.Length,effectFields=d.effectParticles.Length,passives=d.effectPassiveParticles.Length,supportedFields=supported,heldFields=holds.Count,heldTemplates=heldTemplates,holds=holds.ToArray()};
  }
  public void LoadCoreHardClosureVisualBindingV21(string path,string expectedSha256){
   InspectCoreHardClosureVisualBindingV21(path,expectedSha256);var b=CoreHardClosureVisualBindingReadV21(path,expectedSha256);var d=JsonUtility.FromJson<CoreHardClosureVisualDocumentV21>(Encoding.UTF8.GetString(CoreHardClosureVisualReadPinV21(b.document)));var meshes=JsonUtility.FromJson<OutskirtsNpcMeshManifest>(Encoding.UTF8.GetString(CoreHardClosureVisualReadPinV21(b.meshManifest)));var textures=new Dictionary<string,Texture2D>();var owned=new List<Texture2D>();
   try{foreach(var r in b.textures){var bytes=CoreHardClosureVisualReadPinV21(new CoreHardClosureVisualFilePinV21{path=r.path,sha256=r.sha256});var t=new Texture2D(2,2,TextureFormat.RGBA32,false);owned.Add(t);CoreHardClosureVisualRequireV21(ImageConversion.LoadImage(t,bytes,false),"Original source PNG load failed");t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;textures.Add(r.key,t);}ConfigureCoreHardClosureVisualV21(d,meshes,textures);coreHardClosureVisualLoadedTexturesV21.AddRange(owned);owned.Clear();}
   finally{foreach(var t in owned)CoreHardClosureVisualDestroyV21(t);}
  }
 }
}

