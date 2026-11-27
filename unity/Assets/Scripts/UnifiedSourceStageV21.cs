// Exact, opt-in historical C# source projection. Non-source dependencies never project.
using System;using System.IO;using System.Text;using System.Security.Cryptography;using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class UnifiedSourcePinV21 {public string path,sha256;}
 [Serializable] public sealed class UnifiedSourceAliasV21 {public string expectedSha256;public UnifiedSourcePinV21 snapshot;public UnifiedSourcePinV21[] evidence;}
 [Serializable] public sealed class UnifiedSourceStepV21 {public string stage;public UnifiedSourcePinV21 before,after;public bool beforeAbsent;}
 [Serializable] public sealed class UnifiedSourceRowV21 {public string target,livePath,currentSha256;public bool originallyAbsent;public UnifiedSourceStepV21[] steps;public UnifiedSourceAliasV21[] historical;}
 [Serializable] public sealed class UnifiedSourceManifestV21 {public string schema,stageId,sourceRoot,liveSourceRoot,projectionPolicy;public int expectedSources;public bool runtimeAdmitted;public UnifiedSourceRowV21[] sources;public UnifiedSourcePinV21[] frozenPlans;public int expectedResources;public UnifiedCloneResourceV21[] resources;}
 [Serializable] public sealed class UnifiedSourceDescriptorV21 {public string schema,stageId,deployment;public bool actualMainAssetsIntegrated;public bool allowEditorCandidateLoad,runtimeAdmitted,releaseReady,currentSharedRevisionNaturalRunVerified;public UnifiedSourcePinV21 manifest,belowBundle,mine2Bundle;public UnifiedSourcePinV21[] modelEvidence,dataInputs;}
 [Serializable] public sealed class UnifiedCloneResourceV21 {public string target,livePath,expectedSha256;}
 internal sealed class UnifiedSourceStageV21:IDisposable {
 readonly Dictionary<string,UnifiedCloneResourceV21> resources=new Dictionary<string,UnifiedCloneResourceV21>(StringComparer.Ordinal);
  [ThreadStatic] static UnifiedSourceStageV21 current;
  readonly Dictionary<string,UnifiedSourceRowV21> rows=new Dictionary<string,UnifiedSourceRowV21>(StringComparer.Ordinal);
  public readonly UnifiedSourceDescriptorV21 Descriptor;public readonly UnifiedSourceManifestV21 Manifest;string descriptorPath;bool disposed;
#if UNIFIED_SOURCE_STAGE_TESTS
  public static Func<string,byte[]> TestReadBytes;
#endif
  static byte[] Physical(string path){
#if UNIFIED_SOURCE_STAGE_TESTS
   if(TestReadBytes!=null)return TestReadBytes(path);
#endif
   return File.ReadAllBytes(path);
  }
  public static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  static void Require(bool b,string m){if(!b)throw new InvalidDataException("Unified exact source stage: "+m);}
  // Unity JsonUtility may materialize JSON null as an empty serializable DTO.
  // Only the explicit beforeAbsent branch accepts this union. Any field value
  // other than null or the empty string rejects, including future Pin fields.
  static bool EmptyAbsentPinV21(object value){if(value==null)return true;foreach(var field in value.GetType().GetFields(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance)){object v=field.GetValue(value);if(v!=null&&(!(v is string)||((string)v).Length!=0))return false;}return true;}
  static string DescribeAbsentPinV21(object value){if(value==null)return "<null>";var values=new List<string>();foreach(var field in value.GetType().GetFields(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance)){object v=field.GetValue(value);values.Add(field.Name+"="+(v==null?"<null>":"\""+v.ToString()+"\""));}return "{"+string.Join(",",values.ToArray())+"}";}
  static bool HashValid(string h){if(h==null||h.Length!=64)return false;foreach(char c in h)if(!((c>='0'&&c<='9')||(c>='a'&&c<='f')))return false;return true;}
  static string Canon(string p){Require(!string.IsNullOrEmpty(p)&&Path.IsPathRooted(p),"absolute path required");string c=Path.GetFullPath(p);Require(c==p,"noncanonical path "+p);return c;}
  public static byte[] Strict(UnifiedSourcePinV21 p){Require(p!=null&&HashValid(p.sha256),"missing exact pin");byte[] b=Physical(Canon(p.path));Require(b.Length<=128*1024*1024&&Hash(b)==p.sha256,"strict dependency changed "+p.path);return b;}
  internal static UnifiedSourceStageV21 Enter(string descriptorPath,string descriptorHash){
   Require(current==null,"nested source context forbidden");var result=Prepare(descriptorPath,descriptorHash);current=result;return result;
  }
  static UnifiedSourceStageV21 Prepare(string descriptorPath,string descriptorHash){
   Require(descriptorHash==UnifiedSourceStagePinsV21.Descriptor,"caller descriptor is outside compiled exact pin");var d=UnityEngine.JsonUtility.FromJson<UnifiedSourceDescriptorV21>(Encoding.UTF8.GetString(Strict(new UnifiedSourcePinV21{path=descriptorPath,sha256=descriptorHash})));Require(d!=null&&d.schema=="core-cargo-shared-entry-descriptor-v21"&&d.allowEditorCandidateLoad&&!d.runtimeAdmitted&&!d.releaseReady&&!d.currentSharedRevisionNaturalRunVerified,"descriptor flags/schema changed");
   var m=UnityEngine.JsonUtility.FromJson<UnifiedSourceManifestV21>(Encoding.UTF8.GetString(Strict(d.manifest)));var result=new UnifiedSourceStageV21(d,m);result.descriptorPath=descriptorPath;return result;
  }
  // This context belongs to one explicit public Begin, never a process cache.
  // Only activation tokens set ThreadStatic current; all Unity yields are outside them.
  internal sealed class LoadSession:IDisposable {
   UnifiedSourceStageV21 context;readonly object loadOwner;int leases;bool closeRequested;
   public readonly string DescriptorPath,BelowBundlePath,Mine2BundlePath;
   public int ActivationCount{get;private set;}public int FullValidationCount{get{return 1;}}
   public bool IsClosed{get{return context==null;}}
   internal LoadSession(UnifiedSourceStageV21 value,object owner){context=value;loadOwner=owner;DescriptorPath=value.descriptorPath;BelowBundlePath=value.Descriptor.belowBundle.path;Mine2BundlePath=value.Descriptor.mine2Bundle.path;}
   internal IDisposable Activate(object owner){Require(ReferenceEquals(owner,loadOwner),"load session belongs to another entry owner "+DescriptorPath);Require(!closeRequested&&context!=null&&!context.disposed,"load session is closed "+DescriptorPath);Require(current==null||ReferenceEquals(current,context),"another source context cannot borrow load session "+DescriptorPath);context.RecheckLoadActivation();bool owns=current==null;if(owns)current=context;leases++;ActivationCount++;return new Activation(this,owns,leases);}
   sealed class Activation:IDisposable {LoadSession owner;readonly bool owns;readonly int depth;internal Activation(LoadSession value,bool active,int level){owner=value;owns=active;depth=level;}public void Dispose(){if(owner==null)return;var value=owner;Require(value.leases==depth&&ReferenceEquals(current,value.context),"load activation ownership/order changed");owner=null;if(owns)current=null;value.leases--;value.CloseIfIdle();}}
   void CloseIfIdle(){if(!closeRequested||leases!=0||context==null)return;Require(!ReferenceEquals(current,context),"closing active load context");context.disposed=true;context.rows.Clear();context.resources.Clear();context=null;}
   public void Dispose(){closeRequested=true;CloseIfIdle();}
  }
  internal static LoadSession CreateLoadSession(string path,string hash,object owner){Require(owner!=null&&current==null,"cannot prepare ownerless load session or inside another scope");return new LoadSession(Prepare(path,hash),owner);}
  void RecheckLoadActivation(){Strict(new UnifiedSourcePinV21{path=descriptorPath,sha256=UnifiedSourceStagePinsV21.Descriptor});Strict(Descriptor.manifest);foreach(var row in rows.Values)Strict(new UnifiedSourcePinV21{path=row.livePath,sha256=row.currentSha256});}
  UnifiedSourceStageV21(UnifiedSourceDescriptorV21 d,UnifiedSourceManifestV21 m){
   Descriptor=d;Manifest=m;Require(m!=null&&m.schema=="core-cargo-exact-source-transition-manifest-v21"&&m.stageId==d.stageId&&!m.runtimeAdmitted&&m.sources!=null&&m.sources.Length==m.expectedSources&&m.expectedSources>0,"manifest exact source count/schema/stage changed");
   Require(d.deployment=="isolated-clone-only-v21"&&!d.actualMainAssetsIntegrated&&m.projectionPolicy=="exact-clone-relative-source-and-enumerated-resource-v21"&&Canon(m.sourceRoot)==UnifiedSourceStagePinsV21.HistoricalAssets&&Canon(m.liveSourceRoot)==UnifiedSourceStagePinsV21.LiveAssets&&Canon(Path.GetFullPath(UnityEngine.Application.dataPath))==m.liveSourceRoot&&m.liveSourceRoot!=m.sourceRoot,"clone deployment roots/flags changed");string root=Canon(m.sourceRoot)+Path.DirectorySeparatorChar;Require(m.resources!=null&&m.resources.Length==m.expectedResources&&m.expectedResources>0,"exact resource whitelist missing");foreach(var r in m.resources){Require(r!=null&&Canon(r.target).StartsWith(root+"Resources"+Path.DirectorySeparatorChar,StringComparison.Ordinal)&&HashValid(r.expectedSha256)&&SameRelativeLive(r.target,r.livePath)&&!resources.ContainsKey(r.target),"resource projection outside exact same-relative whitelist");resources.Add(r.target,r);}Require(m.frozenPlans!=null&&m.frozenPlans.Length>=2,"frozen plan lineage missing");foreach(var p in m.frozenPlans)Strict(p);
   var verifiedEvidence=new Dictionary<string,byte[]>(StringComparer.Ordinal);foreach(var r in m.sources){Require(r!=null&&Path.GetExtension(r.target)==".cs"&&Canon(r.target).StartsWith(root,StringComparison.Ordinal)&&HashValid(r.currentSha256)&&SameRelativeLive(r.target,r.livePath)&&!rows.ContainsKey(r.target),"duplicate/outside C# target");Require(r.steps!=null&&r.steps.Length>0&&r.historical!=null,"missing exact source history");UnifiedSourcePinV21 previous=null;var stageNames=new HashSet<string>();
    for(int i=0;i<r.steps.Length;i++){var s=r.steps[i];Require(s!=null&&!string.IsNullOrEmpty(s.stage)&&stageNames.Add(s.stage)&&s.after!=null,"invalid source step");Strict(s.after);if(i==0){Require(s.beforeAbsent==r.originallyAbsent,"absent-first mismatch");if(s.beforeAbsent)Require(EmptyAbsentPinV21(s.before),"absent source has nonempty before target="+r.target+" stage="+s.stage+" before="+DescribeAbsentPinV21(s.before));else Strict(s.before);}else{Require(!s.beforeAbsent&&s.before!=null&&previous.sha256==s.before.sha256,"source stage discontinuity");Strict(s.before);}previous=s.after;}
    Require(previous.sha256==r.currentSha256,"last stage does not bind live after");var aliases=new HashSet<string>();foreach(var a in r.historical){Require(a!=null&&HashValid(a.expectedSha256)&&aliases.Add(a.expectedSha256)&&a.snapshot!=null&&a.snapshot.sha256==a.expectedSha256&&a.evidence!=null&&a.evidence.Length>0,"duplicate/unbound historical alias");Strict(a.snapshot);foreach(var e in a.evidence){string key=e.path+"\0"+e.sha256;byte[] verified;if(!verifiedEvidence.TryGetValue(key,out verified)){verified=ReadEvidence(e);verifiedEvidence.Add(key,verified);}}}rows.Add(r.target,r);Strict(new UnifiedSourcePinV21{path=r.livePath,sha256=r.currentSha256});
   }
   Strict(d.belowBundle);Strict(d.mine2Bundle);Require(d.modelEvidence!=null&&d.modelEvidence.Length>0,"fresh stage model evidence missing");foreach(var p in d.modelEvidence)Strict(p);Require(d.dataInputs!=null&&d.dataInputs.Length>0,"exact stage data inputs missing");foreach(var p in d.dataInputs)Strict(p);
  }
  public static bool TryReadHistoricalSource(string path,string expectedHash,out byte[] bytes){
   bytes=null;var c=current;if(c==null)return false;Require(!c.disposed,"disposed context");if(string.IsNullOrEmpty(path)||Path.GetExtension(path)!=".cs")return false;UnifiedSourceRowV21 r;if(!c.rows.TryGetValue(Canon(path),out r))return false;
   // Recheck the live bytes on EVERY projected read, before returning old bytes.
   Strict(new UnifiedSourcePinV21{path=r.livePath,sha256=r.currentSha256});var a=Array.Find(r.historical,x=>x.expectedSha256==expectedHash);Require(a!=null,"unknown historical source hash "+path);bytes=Strict(a.snapshot);return true;
  }
  bool SameRelativeLive(string historical,string live){string root=Canon(Manifest.sourceRoot)+Path.DirectorySeparatorChar;return Canon(historical).StartsWith(root,StringComparison.Ordinal)&&Canon(live)==Path.Combine(Canon(Manifest.liveSourceRoot),historical.Substring(root.Length));}
 byte[] ReadEvidence(UnifiedSourcePinV21 p){UnifiedCloneResourceV21 r;if(resources.TryGetValue(p.path,out r)){Require(p.sha256==r.expectedSha256,"evidence resource hash outside clone whitelist");return Strict(new UnifiedSourcePinV21{path=r.livePath,sha256=r.expectedSha256});}return Strict(p);}
 public static bool TryReadHistoricalResource(string path,string expected,out byte[] bytes){bytes=null;var c=current;if(c==null||string.IsNullOrEmpty(path))return false;string root=Canon(c.Manifest.sourceRoot)+Path.DirectorySeparatorChar+"Resources"+Path.DirectorySeparatorChar;if(!Canon(path).StartsWith(root,StringComparison.Ordinal))return false;UnifiedCloneResourceV21 r;Require(c.resources.TryGetValue(path,out r)&&r.expectedSha256==expected,"unlisted historical Resources path/hash");bytes=Strict(new UnifiedSourcePinV21{path=r.livePath,sha256=expected});return true;}
 public static string HistoricalEvidenceAssetRootV21(string actual){if(current==null)return actual;Require(Canon(Path.GetFullPath(actual))==current.Manifest.liveSourceRoot,"wrong evidence runtime root");return current.Manifest.sourceRoot;}
 public static void RequireLegacyProofContextV21(){if(Canon(Path.GetFullPath(UnityEngine.Application.dataPath))==UnifiedSourceStagePinsV21.HistoricalAssets)return;Require(current!=null&&Canon(Path.GetFullPath(UnityEngine.Application.dataPath))==current.Manifest.liveSourceRoot,"clone legacy entry requires exact compatibility scope");}
 public void Dispose(){if(disposed)return;Require(ReferenceEquals(current,this),"source scope ownership changed");current=null;disposed=true;}
  public static bool HasActiveScope{get{return current!=null;}}
  public static void RequireCurrentDescriptor(string path){Require(current!=null&&!current.disposed&&current.descriptorPath==Canon(path),"loader attempted to borrow another descriptor scope");}
 }
}
