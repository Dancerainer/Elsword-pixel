using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
namespace ElswordPixel {
 [Serializable] public sealed class AlteraAirshipStateDocumentV20 {
  public string schema,payloadJson,payloadSha256,feedSha256,sourceJson,sourceJsonSha256,sourceDocumentSha256,sourceRulesSha256,sourceEffectsSha256;
  public string particleAdapter,sequenceAdapter,triggerAdapter,drawCountAdapter,meshAdapter,ownerSlotAdapter,cameraAdapter,lifecycleAdapter;public bool runtimeAdmitted;
 }
 [Serializable] public sealed class AirshipStateTextureV20 {public string key,path,sha256,rgbaSha256;public int width,height;}
 [Serializable] public sealed class AirshipStateTemplateV20 {public string key,holdReason,particleJson,meshComponent;public bool supported;}
 [Serializable] public sealed class AirshipStateRuleBindingV20 {
  public string id,kind,npc,state,callback,phase,particleKey,bone,positionMethod,holdReason,sourceRowJson;
  public float at,unit,hpMaximumFraction,sourceAnimSpeed,duration,magnitude,cameraDuration;public int ownerSlot;
  public bool followOwner,sourceLandYSeparate,supported,loop;public float[] offset;
  internal AirshipStateRuleBindingV20 Copy(){var value=(AirshipStateRuleBindingV20)MemberwiseClone();value.offset=(float[])offset.Clone();return value;}
 }
 [Serializable] public sealed class AirshipStatePointV20 {public float x,y,z;}
 [Serializable] public sealed class AirshipStateTrackV20 {public string bone;public AirshipStatePointV20[] samples;}
 [Serializable] public sealed class AirshipStateBoneStateV20 {
  public string npc,state;public float duration,sourceAnimSpeed,unit;public int sampleRate;public AirshipStateTrackV20[] tracks;
  internal AirshipStateBoneStateV20 Copy(){var r=(AirshipStateBoneStateV20)MemberwiseClone();r.tracks=new AirshipStateTrackV20[tracks.Length];for(int i=0;i<tracks.Length;i++){var t=tracks[i];var points=new AirshipStatePointV20[t.samples.Length];for(int n=0;n<points.Length;n++){var p=t.samples[n];points[n]=new AirshipStatePointV20{x=p.x,y=p.y,z=p.z};}r.tracks[i]=new AirshipStateTrackV20{bone=t.bone,samples=points};}return r;}
 }
 [Serializable] public sealed class AirshipStateMeshFrameV20 {public string atlas;public float timeSeconds,durationSeconds;public int[] rect;public float[] anchor;}
 [Serializable] public sealed class AirshipStateMeshV20 {public string id,sourceTemplate,sourceBlend,destinationBlend;public float durationSeconds,pixelsPerNativeUnit;public int fps;public AirshipStateMeshFrameV20[] frames;}
 [Serializable] public sealed class AirshipStateRepairKindV20 {public string template,particleKey;public int[] sourceRowIndices;}
 [Serializable] public sealed class AirshipStatePayloadV20 {public string schema;public AirshipStateTemplateV20[] templates;public AirshipStateRuleBindingV20[] bindings;public AirshipStateBoneStateV20[] bones;public AirshipStateMeshV20[] meshes;public AirshipStateTextureV20[] textures;public AirshipStateRepairKindV20[] repairRequests;}
 public static class AirshipStateDataV20 {
  static void S(BinaryWriter w,string value){byte[] b=Encoding.UTF8.GetBytes(value??"");w.Write(b.Length);w.Write(b);}
  public static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  public static string Hash(string value){return Hash(Encoding.UTF8.GetBytes(value??""));}
  public static string FeedHash(AirshipStateRuleBindingV20[] bindings,AirshipStateBoneStateV20[] bones){
   using(var m=new MemoryStream()){using(var w=new BinaryWriter(m,Encoding.UTF8,true)){
    w.Write(bindings.Length);foreach(var b in bindings){foreach(var s in new[]{b.id,b.kind,b.npc,b.state,b.callback,b.phase,b.particleKey,b.bone,b.positionMethod,b.holdReason,b.sourceRowJson})S(w,s);foreach(float n in new[]{b.at,b.unit,b.hpMaximumFraction,b.sourceAnimSpeed,b.duration,b.magnitude,b.cameraDuration})w.Write(n);w.Write(b.ownerSlot);foreach(bool n in new[]{b.followOwner,b.sourceLandYSeparate,b.supported,b.loop})w.Write(n);w.Write(b.offset.Length);foreach(float n in b.offset)w.Write(n);}
    w.Write(bones.Length);foreach(var b in bones){S(w,b.npc);S(w,b.state);w.Write(b.duration);w.Write(b.sourceAnimSpeed);w.Write(b.sampleRate);w.Write(b.unit);w.Write(b.tracks.Length);foreach(var t in b.tracks){S(w,t.bone);w.Write(t.samples.Length);foreach(var p in t.samples){w.Write(p.x);w.Write(p.y);w.Write(p.z);}}}
   }return Hash(m.ToArray());}
  }
 }
 public sealed class AirshipStatePresentationEventV20 {
  public int sequence,entry,handle,facing;public string kind,binding,particleKey,npc,state;public float time,x,y,depth,landY,unit,magnitude,cameraDuration;public bool hasLand;public Enemy owner;
 }
 public sealed class AirshipStateCameraRequestV20 {public int sequence;public string source;public float time,magnitude,duration;public bool nativeProjectionImplemented=false;}
}
