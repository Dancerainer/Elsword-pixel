// Candidate interface only. No catalog mutation, native calls, or filesystem IO.
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace ElswordPixel {
 [Serializable] public sealed class BelowPlayerFileV21 {public string path,sha256;}
 [Serializable] public sealed class BelowPlayerResourceRowV21 {public string path,sha256,resource,role;public int bytes;}
 [Serializable] public sealed class BelowPlayerRootsV21 {
  public BelowPlayerFileV21 bundle,model,rules,effects,sounds,mechanisms,soundMap,artClockProof,sourcePreservation;
  public BelowPlayerFileV21 unboundPhysicalReactionPolicy,recoveryLookupPolicy,visibility,effectPresentationBinding;
 }
 [Serializable] public sealed class BelowPlayerManifestV21 {
  public string schema,sourceDungeon,baselineCatalogSha256,approvedBundleSha256;
  public bool runtimeAdmitted,releaseReady,hostIntegrationComplete;
  public int baselineRoutes,baselineStages,baselineContentSets,appendStart,rooms,births,profiles,states,effects,audio,visibleLibraries;
  public BelowPlayerRootsV21 roots;public BelowPlayerResourceRowV21[] files;
 }
 public sealed class BelowPlayerResourceIndexV21 {
  public const string Prefix="Pixel/below-player-candidate-v21/r1/";
  public const string SourceDungeon="Dungeon_Altera_Below_Tunnel_Normal";
  public const string BaselineCatalog="6531cb2da405bab6a0afa3c397e6827c43f8a564cfeff351949719536b0a480b";
  public const string ApprovedBundle="699b1f232a77a5d471d17b0053761621b6604f172343a1049e95c6148ff5ccd2";
  readonly Dictionary<string,BelowPlayerResourceRowV21> rows=new Dictionary<string,BelowPlayerResourceRowV21>(StringComparer.Ordinal);
  readonly Func<string,byte[]> resourceBytes;
  public BelowPlayerResourceIndexV21(BelowPlayerManifestV21 manifest,Func<string,byte[]> loadResource){
   Need(manifest!=null&&loadResource!=null,"Missing frozen manifest/resource reader");
   Need(manifest.schema=="below-player-resource-interface-candidate-v21-r1"&&manifest.sourceDungeon==SourceDungeon,"Wrong route/schema");
   Need(manifest.baselineCatalogSha256==BaselineCatalog&&manifest.approvedBundleSha256==ApprovedBundle,"Wrong formal12/approved Below pins");
   Need(!manifest.runtimeAdmitted&&!manifest.releaseReady&&!manifest.hostIntegrationComplete,"This interface draft cannot grant runtime admission");
   Need(manifest.baselineRoutes==12&&manifest.baselineStages==35&&manifest.baselineContentSets==8&&manifest.appendStart==35,"Formal12 slots changed");
   Need(manifest.rooms==3&&manifest.births==78&&manifest.profiles==24&&manifest.states==279&&manifest.effects==11&&manifest.audio==67&&manifest.visibleLibraries==21,"Below exact bounded scope changed");
   Need(manifest.roots!=null&&manifest.files!=null&&manifest.files.Length>0,"Empty runtime resource closure");
   foreach(var row in manifest.files){
    Need(row!=null&&!string.IsNullOrEmpty(row.path)&&IsHash(row.sha256)&&row.resource==Prefix+row.sha256&&row.bytes>=0&&row.bytes<=64*1024*1024&&!string.IsNullOrEmpty(row.role),"Invalid content-addressed resource row");
    Need(!rows.ContainsKey(row.path),"Duplicate original reference key");
    // Keep a private copy: caller mutation must not change the trusted index.
    rows.Add(row.path,new BelowPlayerResourceRowV21{path=row.path,sha256=row.sha256,resource=row.resource,bytes=row.bytes,role=row.role});
   }
   resourceBytes=loadResource;
   CheckReference(manifest.roots.bundle);Need(manifest.roots.bundle.sha256==ApprovedBundle,"Root is not approved ready-r2");
  }
  internal BelowPlayerResourceIndexV21(BelowProductResourceMapR1130 map,Func<string,byte[]> loadResource){
   BelowProductResourcesR1130.Validate(map);Need(loadResource!=null,"Missing product resource reader");
   foreach(var row in map.files)rows.Add(row.path,new BelowPlayerResourceRowV21{path=row.path,sha256=row.sha256,resource=row.resource,bytes=row.bytes,role=row.role});
   resourceBytes=loadResource;CheckReference(map.roots.bundle);
  }
  public static void Need(bool ok,string message){if(!ok)throw new InvalidOperationException("Below Player candidate: "+message);}
  public static bool IsHash(string value){if(value==null||value.Length!=64)return false;foreach(char c in value)if(!(c>='0'&&c<='9'||c>='a'&&c<='f'))return false;return true;}
  public static string Hash(byte[] data){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(data)).Replace("-","").ToLowerInvariant();}
  public void CheckReference(BelowPlayerFileV21 file){BelowPlayerResourceRowV21 row;Need(file!=null&&!string.IsNullOrEmpty(file.path)&&rows.TryGetValue(file.path,out row)&&row.sha256==file.sha256,"Unmapped or changed original reference");}
  public byte[] Read(BelowPlayerFileV21 file){
   CheckReference(file);var row=rows[file.path];var bytes=resourceBytes(row.resource);
   Need(bytes!=null&&bytes.Length==row.bytes&&Hash(bytes)==row.sha256,"Missing or changed embedded bytes "+row.resource);
   return (byte[])bytes.Clone();
  }
  public void CheckBaseline(byte[] bytes){Need(bytes!=null&&Hash(bytes)==BaselineCatalog,"Current Resources formal12 catalog differs");}
  public void ValidateAllResources(){foreach(var row in rows.Values)Read(new BelowPlayerFileV21{path=row.path,sha256=row.sha256});}
 }
 public sealed class BelowPlayerPcm16V21 {
  public int sampleRate,channels,frames,dataOffset,dataBytes;
  static uint U32(byte[] data,int offset){return (uint)(data[offset]|data[offset+1]<<8|data[offset+2]<<16|data[offset+3]<<24);}
  static int U16(byte[] data,int offset){return data[offset]|data[offset+1]<<8;}
  static bool Tag(byte[] data,int offset,string text){for(int i=0;i<4;i++)if(data[offset+i]!=(byte)text[i])return false;return true;}
  public static BelowPlayerPcm16V21 Inspect(byte[] data){
   BelowPlayerResourceIndexV21.Need(data!=null&&data.Length>=44&&Tag(data,0,"RIFF")&&Tag(data,8,"WAVE")&&U32(data,4)==data.Length-8,"Invalid exact PCM RIFF boundary");
   var result=new BelowPlayerPcm16V21();bool format=false,payload=false;int offset=12;
   while(offset+8<=data.Length){
    uint length=U32(data,offset+4);long end=(long)offset+8+length;BelowPlayerResourceIndexV21.Need(end<=data.Length,"WAV chunk exceeds approved file");int body=offset+8;
    if(Tag(data,offset,"fmt ")){
     BelowPlayerResourceIndexV21.Need(!format&&length>=16&&U16(data,body)==1&&U16(data,body+14)==16,"Only original PCM16 WAV supported");format=true;result.channels=U16(data,body+2);result.sampleRate=(int)U32(data,body+4);
     BelowPlayerResourceIndexV21.Need(result.channels>0&&result.channels<=2&&result.sampleRate>0&&U16(data,body+12)==result.channels*2&&U32(data,body+8)==(long)result.sampleRate*result.channels*2,"Changed PCM16 format/stride");
    }else if(Tag(data,offset,"data")){BelowPlayerResourceIndexV21.Need(!payload,"Duplicate WAV data chunk");payload=true;result.dataOffset=body;result.dataBytes=(int)length;}
    offset=checked((int)(end+(length&1)));
   }
   BelowPlayerResourceIndexV21.Need(offset==data.Length&&format&&payload&&result.dataBytes>0&&result.dataBytes%(result.channels*2)==0,"Incomplete PCM16 WAV");result.frames=result.dataBytes/(result.channels*2);return result;
  }
 }
}
