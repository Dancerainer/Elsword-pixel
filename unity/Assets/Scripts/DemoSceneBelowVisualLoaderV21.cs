using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class BelowVisualFilePinV21 {public string path,sha256;}
 [Serializable] public sealed class BelowVisualTexturePinV21 {public string key,path,sha256;}
 [Serializable] public sealed class BelowVisualHoldV21 {public string effect,sourceField,particleKey,reason;}
 [Serializable] public sealed class BelowVisualBindingV21 {
  public string schema;public bool runtimeAdmitted,actualUnityVerified,nativeEquivalent;
  public BelowVisualFilePinV21 document,meshManifest,sourceEffects,sourceRules;
  public BelowVisualTexturePinV21[] textures;public BelowVisualHoldV21[] declaredHolds;
 }
 [Serializable] public sealed class BelowVisualInspectionV21 {
  public string schema="below-source-visual-inspection-v21",bindingSha256;
  public bool completeDefinitionCoverage,allHoldsExplicit,sourceDefinitionsUnchanged,nativeEquivalent=false;
  public int definitions,mainMeshes,particleTemplates,effectFields,passives,supportedFields,heldFields,heldTemplates;
  public BelowVisualHoldV21[] holds;
 }
 public sealed partial class DemoScene {
  readonly List<Texture2D> belowVisualLoadedTexturesV21=new List<Texture2D>();
  partial void DisposeBelowVisualExternalTexturesV21(){foreach(var t in belowVisualLoadedTexturesV21)BelowVisualDestroyV21(t);belowVisualLoadedTexturesV21.Clear();}
#if UNITY_EDITOR
  static byte[] BelowVisualReadPinV21(BelowVisualFilePinV21 p){BelowVisualRequireV21(p!=null&&Path.IsPathRooted(p.path),"External source path must be absolute");var b=File.ReadAllBytes(p.path);BelowVisualRequireV21(BelowVisualHashV21(b)==p.sha256,"Changed external source file "+p.path);return b;}
  static BelowVisualBindingV21 BelowVisualBindingReadV21(string path,string expectedSha256){
   var bytes=BelowVisualReadPinV21(new BelowVisualFilePinV21{path=path,sha256=expectedSha256});var b=JsonUtility.FromJson<BelowVisualBindingV21>(Encoding.UTF8.GetString(bytes));
   BelowVisualRequireV21(b!=null&&b.schema=="below-source-visual-binding-v21"&&!b.runtimeAdmitted&&!b.actualUnityVerified&&!b.nativeEquivalent&&b.textures!=null&&b.declaredHolds!=null,"Invalid external Below binding");return b;
  }
  static string BelowVisualHoldKeyV21(BelowVisualHoldV21 h){return h.effect+"/"+h.sourceField+"/"+h.particleKey+"/"+h.reason;}
  // Read-only inspection verifies exact bytes and source identities. It does
  // not create Unity objects, change the GameModel, or approve declared HOLDs.
  public static BelowVisualInspectionV21 InspectBelowVisualBindingV21(string path,string expectedSha256){
   var b=BelowVisualBindingReadV21(path,expectedSha256);var d=JsonUtility.FromJson<BelowVisualDocumentV21>(Encoding.UTF8.GetString(BelowVisualReadPinV21(b.document)));
   BelowVisualRequireV21(d!=null&&d.schema=="below-source-visual-v21"&&d.unit==.28f&&!d.runtimeAdmitted&&!d.actualUnityVerified&&d.particles!=null&&d.effectParticles!=null&&d.effectMeshes!=null&&d.effectPassiveParticles!=null,"Malformed source visual document");ValidateBelowVisualCoverageV21(d);
   var originalBytes=BelowVisualReadPinV21(b.sourceEffects);var original=JsonUtility.FromJson<NpcEffectDocument>(Encoding.UTF8.GetString(originalBytes));BelowVisualRequireV21(original!=null&&original.definitions!=null&&original.definitions.Length==11&&BelowVisualHashV21(originalBytes)==d.sourceEffectsSha256&&BelowVisualHashV21(BelowVisualReadPinV21(b.sourceRules))==d.sourceRulesSha256,"Changed original rules/effects");
   foreach(var r in d.definitions){var source=Array.Find(original.definitions,q=>q.id==r.id);BelowVisualRequireV21(source!=null&&source.lifetime==r.lifetime&&source.visualFromMesh==r.sourceVisibleMesh&&string.IsNullOrEmpty(source.visualId),"Source identity/clock/visualId altered");}
   var meshes=JsonUtility.FromJson<OutskirtsNpcMeshManifest>(Encoding.UTF8.GetString(BelowVisualReadPinV21(b.meshManifest)));BelowVisualRequireV21(meshes!=null&&meshes.components!=null&&meshes.components.Length==9,"Missing source mesh components");
   var keys=new HashSet<string>();foreach(var t in b.textures){BelowVisualRequireV21(t!=null&&!string.IsNullOrEmpty(t.key)&&keys.Add(t.key),"Duplicate source texture key");BelowVisualReadPinV21(new BelowVisualFilePinV21{path=t.path,sha256=t.sha256});}
   var holds=new List<BelowVisualHoldV21>();int heldTemplates=0,supported=0;foreach(var p in d.particles){BelowVisualRequireV21(BelowVisualHashV21(Encoding.UTF8.GetBytes(p.particleJson??""))==p.particleJsonSha256,"Changed original normalized particle");if(!p.supported)heldTemplates++;}
   foreach(var f in d.effectParticles){var p=Array.Find(d.particles,q=>q.key==f.particleKey);BelowVisualRequireV21(p!=null,"Missing typed original particle template");if(f.supported&&p.supported)supported++;else holds.Add(new BelowVisualHoldV21{effect=f.effect,sourceField=f.sourceField,particleKey=f.particleKey,reason=!f.supported?f.holdReason:p.holdReason});}
   foreach(var f in d.effectPassiveParticles){var p=Array.Find(d.particles,q=>q.key==f.particleKey);BelowVisualRequireV21(p!=null,"Missing typed original passive template");if(p.supported)supported++;else holds.Add(new BelowVisualHoldV21{effect=f.effect,sourceField=f.sourceField,particleKey=f.particleKey,reason=p.holdReason});}
   var declared=new HashSet<string>();foreach(var h in b.declaredHolds)BelowVisualRequireV21(h!=null&&!string.IsNullOrEmpty(h.reason)&&declared.Add(BelowVisualHoldKeyV21(h)),"Ambiguous/empty declared HOLD");foreach(var h in holds)BelowVisualRequireV21(declared.Remove(BelowVisualHoldKeyV21(h)),"Unreported source HOLD");BelowVisualRequireV21(declared.Count==0,"Obsolete/unmatched HOLD declaration");
   return new BelowVisualInspectionV21{bindingSha256=expectedSha256,completeDefinitionCoverage=true,allHoldsExplicit=true,sourceDefinitionsUnchanged=true,definitions=d.definitions.Length,mainMeshes=d.effectMeshes.Length,particleTemplates=d.particles.Length,effectFields=d.effectParticles.Length,passives=d.effectPassiveParticles.Length,supportedFields=supported,heldFields=holds.Count,heldTemplates=heldTemplates,holds=holds.ToArray()};
  }
  public void LoadBelowVisualBindingV21(string path,string expectedSha256){
   InspectBelowVisualBindingV21(path,expectedSha256);var b=BelowVisualBindingReadV21(path,expectedSha256);var d=JsonUtility.FromJson<BelowVisualDocumentV21>(Encoding.UTF8.GetString(BelowVisualReadPinV21(b.document)));var meshes=JsonUtility.FromJson<OutskirtsNpcMeshManifest>(Encoding.UTF8.GetString(BelowVisualReadPinV21(b.meshManifest)));var textures=new Dictionary<string,Texture2D>();var owned=new List<Texture2D>();
   try{foreach(var r in b.textures){var bytes=BelowVisualReadPinV21(new BelowVisualFilePinV21{path=r.path,sha256=r.sha256});var t=new Texture2D(2,2,TextureFormat.RGBA32,false);owned.Add(t);BelowVisualRequireV21(ImageConversion.LoadImage(t,bytes,false),"Original source PNG load failed");t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;textures.Add(r.key,t);}ConfigureBelowVisualV21(d,meshes,textures);belowVisualLoadedTexturesV21.AddRange(owned);owned.Clear();}
   finally{foreach(var t in owned)BelowVisualDestroyV21(t);}
  }
#endif
 }
}
