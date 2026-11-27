using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class DragonNestCountdownTextureV20 {
  public string source,sourceName,key,sourceSha256,pngSha256,rgbaSha256;public int width,height;
 }
 [Serializable] public sealed class DragonNestCountdownDocumentV20 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,sourceEvidenceSha256,particleSourceManifestSha256,sourceTemplateSha256,sourceFireInstructionsSha256,sourceEndInstructionsSha256;
  public string manager,template,npc,fireState,sourceParticleType,sourceBillboardType,sourceBlend,destinationBlend;
  public string planeAdapter,sizeAdapter,blendAdapter,lifetimeAdapter,positionAdapter,rgbaHashRowOrder;
  public int sourceCapacity,sourceLayer,sourceSlot,sourceCreateMethodPC,sourceCreateLiteralPC,sourceDestroyMethodPC,sourceClearSlotMethodPC;
  public float sourceEmitRate,sourceParticleLifetime,unit,sourcePositionOffsetY;
  public float[] sourceInitialColor,sourceInitialSize,sourceInitialVelocity,sourceAnimationThresholds;
  public int[] sourceChangeTexturePCs,sourceChangeTextureLiteralPCs;
  public bool sourceTrace,planeSupported,pendingSourcePlaneBasis,sourceTexturesVerified,nativePlaneBasisMeasured,nativeSequenceAllocationMeasured,runtimeAdmitted,actualUnityVerified;
  public DragonNestCountdownTextureV20[] textures;
 }
 public sealed partial class DemoScene {
  const string DragonCountdownSchemaV20="besma-dragon-nest-bat-source-countdown-v20";
  static readonly string[] DragonCountdownSourceHashesV20={"96fb4206d8a0fef648f1dcb7db93105b3b824a90caf4022bdedfe75c3be9aa99","cc695a561e31076a6e6b218ebc8de9fbecac930cc018efe54d246f626f44d4f3","b06da1cf2ad011bc5939098d8dcb7cc8110830cf39db8b265a22de3e125a9c59","8bcfcd8af229b2578e0ce62c5e893d36efb4e5cf5319294ea51b6de1dd426a45"};
  static readonly string[] DragonCountdownPngHashesV20={"80b0e047c1f5f266f50c7b36724c8be4bd3c590545d4be559c000d29c74d2085","cdd6174a0de4d81b94f6fbee91e064d732062d0dbcd0d4edee8b622ca7fe6481","ec68670d4eb8d88b6b3d0ac16c07adfe547045682a2749c6fe90bbc8d380cec1","8cdb9cb2c776b633b58aa3550ea36af1ad85719db0791442184d949051b934f5"};
  static readonly string[] DragonCountdownRgbaHashesV20={"3e630141394857d82f97637f2e216b8b6344c8fcf19befbf1904ebcfb3255f1f","65de8986b48c068d04ab805dd06f710de09280866ea6b4b2988a797f24187c19","d37fd03c31ab327c8502c1904b0c20a56bfba1a93d58544c35e52a5652537883","3447bf853658a823eb351cf6017953d9acabf21644757cef5d9930d75ba67d6e"};
  sealed class DragonCountdownActorV20 {internal Enemy owner;internal int actorSerial,worldSerial,index;internal float began;internal SpriteRenderer renderer;internal Transform rendererTransform;}
  DragonNestCountdownDocumentV20 dragonCountdownDocumentV20;Material dragonCountdownMaterialV20;
  readonly List<Sprite> dragonCountdownImagesV20=new List<Sprite>();
  readonly Dictionary<int,DragonCountdownActorV20> dragonCountdownActorsV20=new Dictionary<int,DragonCountdownActorV20>();
  readonly List<int> dragonCountdownRetireV20=new List<int>();
  Transform dragonCountdownWorldV20;int dragonCountdownWorldSerialV20=-1,dragonCountdownRoomV20=-1,dragonCountdownLastSequenceV20;float dragonCountdownTimeV20=-1;
  public int DragonNestCountdownLiveCountV20 {get{return dragonCountdownActorsV20.Count;}}
  public int DragonNestCountdownCreatedCountV20 {get;private set;}
  public int DragonNestCountdownTextureChangeCountV20 {get;private set;}
  public int DragonNestCountdownDestroyedCountV20 {get;private set;}
  public int DragonNestCountdownRetiredCountV20 {get;private set;}
  public int DragonNestCountdownDrawCountV20 {get;private set;}
  public int DragonNestCountdownRejectedEventCountV20 {get;private set;}
  public int DragonNestCountdownWorldResetCountV20 {get;private set;}
  public int DragonNestCountdownLastSequenceV20 {get{return dragonCountdownLastSequenceV20;}}
  public SpriteRenderer DragonNestCountdownRendererForActorV20(int actorSerial){DragonCountdownActorV20 row;return dragonCountdownActorsV20.TryGetValue(actorSerial,out row)?row.renderer:null;}
  static void RequireDragonCountdownV20(bool ok,string reason){if(!ok)throw new InvalidOperationException("Dragon countdown: "+reason);}
  static bool DragonCountdownFiniteV20(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  static bool DragonCountdownFloatsV20(float[] a,params float[] expected){if(a==null||a.Length!=expected.Length)return false;for(int i=0;i<a.Length;i++)if(!DragonCountdownFiniteV20(a[i])||Mathf.Abs(a[i]-expected[i])>.000001f)return false;return true;}
  static bool DragonCountdownIntsV20(int[] a,params int[] expected){if(a==null||a.Length!=expected.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=expected[i])return false;return true;}
  static string DragonCountdownTextureHashV20(Texture2D texture){
   // Unity pixels are bottom-left row first; source PNG/RGBA pins are top-left.
   var pixels=texture.GetPixels32();var bytes=new byte[pixels.Length*4];int at=0;
   for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){var p=pixels[y*texture.width+x];bytes[at++]=p.r;bytes[at++]=p.g;bytes[at++]=p.b;bytes[at++]=p.a;}
   using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
  }
  static void DragonCountdownDestroyObjectV20(UnityEngine.Object item){if(!item)return;if(Application.isPlaying)UnityEngine.Object.Destroy(item);else UnityEngine.Object.DestroyImmediate(item);}
  public void ConfigureDragonNestCountdownV20(DragonNestCountdownDocumentV20 doc,IDictionary<string,Texture2D> textures){
   if(doc==null){DisposeDragonNestCountdownV20();return;}
   RequireDragonCountdownV20(doc.schema==DragonCountdownSchemaV20&&doc.sourceRulesSha256=="0a78bd2021b407841b0389f47bdfce44af64ce0a2c352187b2393cbdb069dd77"&&doc.sourceEffectsSha256=="c279abde43dd2f98905a4ee93fe033d606f5f70d9b16fcd1386c1758439eaad4"&&doc.sourceEvidenceSha256=="ebbe209016cc06b5caf79e9e88b8e00de94478c9a4dfe69a686eb965698c623d"&&doc.particleSourceManifestSha256=="6133bf4fbabd5c26e4853663629a4712e9ac5832af3c3066a4501b0311f2a2b3","Changed scoped source pins/schema");
   RequireDragonCountdownV20(doc.sourceTemplateSha256=="62707d834c71b94453fbc1b0a48d9cc9d76094017361b5e50d2afe54f7cd28cc"&&doc.sourceFireInstructionsSha256=="31872a0d75649c82bde5d3b7c3421684e31a4acfa190131f655b4d52c352ec7c"&&doc.sourceEndInstructionsSha256=="120a2b78140eb126582d2a5b595e839ade78fc64467f6c800111f5c76397fa92","Changed original template/callback IR");
   RequireDragonCountdownV20(doc.manager=="Major"&&doc.template=="Bat_B_Fire_Countdown"&&doc.npc=="NUI_BAT_B_FIRE"&&doc.fireState=="BAT_B_FIRE_ATTACK_A"&&doc.sourceSlot==0&&doc.sourceLayer==14&&doc.sourceCapacity==1&&doc.sourceTrace&&doc.sourceEmitRate==1000&&doc.sourceParticleLifetime==4,"Changed source instance/count/clock/identity");
   RequireDragonCountdownV20(doc.sourceParticleType=="PT_3D_PLANE"&&doc.sourceBillboardType=="BT_ALL"&&doc.sourceBlend=="D3DBLEND_SRCALPHA"&&doc.destinationBlend=="D3DBLEND_INVSRCALPHA"&&doc.planeSupported&&!doc.pendingSourcePlaneBasis&&doc.sourceTexturesVerified&&!doc.nativePlaneBasisMeasured&&!doc.nativeSequenceAllocationMeasured&&!doc.runtimeAdmitted,"Unverified/unsupported source plane or blend");
   RequireDragonCountdownV20(doc.unit==.28f&&doc.sourcePositionOffsetY==300&&DragonCountdownFloatsV20(doc.sourceInitialColor,1,0,0,1)&&DragonCountdownFloatsV20(doc.sourceInitialSize,80,80,1)&&DragonCountdownFloatsV20(doc.sourceInitialVelocity,0,0,0)&&DragonCountdownFloatsV20(doc.sourceAnimationThresholds,.01f,1,2,3),"Changed original offset/color/size/velocity/timers");
   RequireDragonCountdownV20(doc.sourceCreateMethodPC==81&&doc.sourceCreateLiteralPC==83&&doc.sourceDestroyMethodPC==32&&doc.sourceClearSlotMethodPC==36&&DragonCountdownIntsV20(doc.sourceChangeTexturePCs,0,133,163,193)&&DragonCountdownIntsV20(doc.sourceChangeTextureLiteralPCs,0,134,164,194),"Changed source Create/ChangeTexForce/Destroy PCs");
   RequireDragonCountdownV20(doc.planeAdapter=="bounded-camera-facing-XY-single-major-slot0-v20"&&doc.sizeAdapter=="source80-native-world-times-point28-no-owner-scale-v20"&&doc.blendAdapter=="NativeSkillSprite-srcalpha-invsrcalpha-coverage-v20"&&doc.lifetimeAdapter=="source-handle-until-explicit-destroy-no-sequence-ttl-from-particle-life-v20"&&doc.positionAdapter=="owner-current-GetPos-world-plus300-times-point28-v20"&&doc.rgbaHashRowOrder=="top-left-row-major","Unbound countdown display adapter");
   RequireDragonCountdownV20(textures!=null&&doc.textures!=null&&doc.textures.Length==4,"Four exact source textures required");
   var selected=new Texture2D[4];var keys=new HashSet<string>();
   for(int i=0;i<4;i++){
    var row=doc.textures[i];string id="PC_"+(4-i);Texture2D texture;
    RequireDragonCountdownV20(row!=null&&row.source==id&&row.sourceName==id+".dds"&&row.key=="textures/pc-"+(4-i)+".png"&&keys.Add(row.key)&&row.width==32&&row.height==32&&row.sourceSha256==DragonCountdownSourceHashesV20[i]&&row.pngSha256==DragonCountdownPngHashesV20[i]&&row.rgbaSha256==DragonCountdownRgbaHashesV20[i],"Changed original countdown image pins/order");
    RequireDragonCountdownV20(textures.TryGetValue(row.key,out texture)&&texture&&texture.width==32&&texture.height==32&&texture.isReadable,"Missing/unreadable original countdown texture");
    RequireDragonCountdownV20(DragonCountdownTextureHashV20(texture)==row.rgbaSha256,"Countdown decoded RGBA differs from verified original "+id);selected[i]=texture;
   }
   var shader=Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite");RequireDragonCountdownV20(shader&&shader.name=="ElswordPixel/NativeSkillSprite","Missing explicit source blend shader");
   var images=new List<Sprite>();Material material=null;
   try{
    material=new Material(shader){name="Dragon source Bat countdown alpha blend"};material.SetInt("_SrcBlend",5);material.SetInt("_DstBlend",10);material.SetColor("_Color",Color.white);
    foreach(var texture in selected){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;images.Add(Sprite.Create(texture,new Rect(0,0,32,32),new Vector2(.5f,.5f),1));}
   }catch{foreach(var image in images)DragonCountdownDestroyObjectV20(image);DragonCountdownDestroyObjectV20(material);throw;}
   DisposeDragonNestCountdownV20();dragonCountdownDocumentV20=doc;dragonCountdownMaterialV20=material;dragonCountdownImagesV20.AddRange(images);
   DragonNestCountdownCreatedCountV20=DragonNestCountdownTextureChangeCountV20=DragonNestCountdownDestroyedCountV20=DragonNestCountdownRetiredCountV20=DragonNestCountdownDrawCountV20=DragonNestCountdownRejectedEventCountV20=DragonNestCountdownWorldResetCountV20=0;
  }
  void RetireDragonCountdownV20(int actorSerial,bool sourceDestroy){
   DragonCountdownActorV20 row;if(!dragonCountdownActorsV20.TryGetValue(actorSerial,out row))return;
   if(!ReferenceEquals(row.rendererTransform,null))npcEffectDepths.Remove(row.rendererTransform);
   if(row.renderer){row.renderer.enabled=false;RemoveSceneObject(row.renderer.gameObject);}dragonCountdownActorsV20.Remove(actorSerial);
   if(sourceDestroy)DragonNestCountdownDestroyedCountV20++;else DragonNestCountdownRetiredCountV20++;
  }
  void EnsureDragonCountdownRendererV20(DragonCountdownActorV20 row){
   if(row.renderer&&row.renderer.transform.parent==effectsRoot)return;
   if(!ReferenceEquals(row.rendererTransform,null))npcEffectDepths.Remove(row.rendererTransform);
   if(row.renderer){row.renderer.gameObject.SetActive(false);RemoveSceneObject(row.renderer.gameObject);}
   row.renderer=WorldSprite("Dragon original Major Bat countdown / slot0",effectsRoot,dragonCountdownImagesV20[row.index],44,Vector3.zero);row.rendererTransform=row.renderer.transform;
   row.renderer.sharedMaterial=dragonCountdownMaterialV20;
  }
  void ClearDragonCountdownInstancesV20(){dragonCountdownRetireV20.Clear();foreach(var row in dragonCountdownActorsV20)dragonCountdownRetireV20.Add(row.Key);foreach(int key in dragonCountdownRetireV20)RetireDragonCountdownV20(key,false);dragonCountdownRetireV20.Clear();}
  public void DisposeDragonNestCountdownV20(){
   ClearDragonCountdownInstancesV20();foreach(var sprite in dragonCountdownImagesV20)DragonCountdownDestroyObjectV20(sprite);dragonCountdownImagesV20.Clear();DragonCountdownDestroyObjectV20(dragonCountdownMaterialV20);dragonCountdownMaterialV20=null;dragonCountdownDocumentV20=null;
   dragonCountdownWorldV20=null;dragonCountdownWorldSerialV20=dragonCountdownRoomV20=-1;dragonCountdownLastSequenceV20=0;dragonCountdownTimeV20=-1;
  }
  public void RenderDragonNestCountdownV20(){
   var doc=dragonCountdownDocumentV20;if(doc==null)return;
   if(!worldRoot||!effectsRoot){ClearDragonCountdownInstancesV20();return;}
   int world=game.NpcSoundWorldSerial;
   if(dragonCountdownWorldV20!=worldRoot||dragonCountdownWorldSerialV20!=world||dragonCountdownRoomV20!=game.room||game.elapsed+1e-6f<dragonCountdownTimeV20){
    ClearDragonCountdownInstancesV20();dragonCountdownWorldV20=worldRoot;dragonCountdownWorldSerialV20=world;dragonCountdownRoomV20=game.room;dragonCountdownLastSequenceV20=0;DragonNestCountdownWorldResetCountV20++;
   }
   dragonCountdownTimeV20=game.elapsed;
   foreach(var e in game.dragonSourceEventsV20){
    if(e==null||e.worldSerial!=world||e.room!=game.room||e.sequence<=dragonCountdownLastSequenceV20)continue;
    if(!DragonCountdownFiniteV20(e.time)||e.time>game.elapsed+.0001f)continue;
    dragonCountdownLastSequenceV20=e.sequence;
    if(e.type!="source-countdown"&&e.type!="source-countdown-destroy")continue;
    if(e.npc!=doc.npc||e.owner==null||e.owner.sourceNPC!=doc.npc||e.actorSerial<=0){DragonNestCountdownRejectedEventCountV20++;continue;}
    DragonCountdownActorV20 row;
    if(e.type=="source-countdown-destroy"){
     if(e.source!="major-slot0"){DragonNestCountdownRejectedEventCountV20++;continue;}
     if(dragonCountdownActorsV20.TryGetValue(e.actorSerial,out row)&&row.owner==e.owner&&row.worldSerial==world)RetireDragonCountdownV20(e.actorSerial,true);continue;
    }
    int index=e.source=="PC_4"?0:e.source=="PC_3"?1:e.source=="PC_2"?2:e.source=="PC_1"?3:-1;
    if(index<0||e.amount!=index||!game.enemies.Contains(e.owner)||!e.owner.active||e.owner.dormant||e.owner.hp<=0||e.owner.npcState!=doc.fireState){DragonNestCountdownRejectedEventCountV20++;continue;}
    if(index==0){
     RetireDragonCountdownV20(e.actorSerial,false);
     row=new DragonCountdownActorV20{owner=e.owner,actorSerial=e.actorSerial,worldSerial=world,index=0,began=e.time};EnsureDragonCountdownRendererV20(row);
     dragonCountdownActorsV20.Add(e.actorSerial,row);DragonNestCountdownCreatedCountV20++;
    }else{
     if(!dragonCountdownActorsV20.TryGetValue(e.actorSerial,out row)||row.owner!=e.owner||row.worldSerial!=world||index!=row.index+1){DragonNestCountdownRejectedEventCountV20++;continue;}
     row.index=index;DragonNestCountdownTextureChangeCountV20++;
    }
   }
   dragonCountdownRetireV20.Clear();
   foreach(var pair in dragonCountdownActorsV20){
    var row=pair.Value;var owner=row.owner;
    if(owner==null||!game.enemies.Contains(owner)||!owner.active||owner.dormant||owner.hp<=0||owner.npcState!=doc.fireState){dragonCountdownRetireV20.Add(pair.Key);continue;}
    EnsureDragonCountdownRendererV20(row);
    var sprite=row.renderer;sprite.enabled=true;sprite.gameObject.SetActive(true);sprite.sprite=dragonCountdownImagesV20[row.index];sprite.sharedMaterial=dragonCountdownMaterialV20;sprite.flipX=false;sprite.flipY=false;sprite.sortingOrder=44;
    sprite.color=new Color(1,0,0,1);sprite.transform.localRotation=Quaternion.identity;sprite.transform.localScale=new Vector3(80*.28f/32,80*.28f/32,1);
    sprite.transform.localPosition=new Vector3(owner.x,owner.y+300*.28f,0);npcEffectDepths[sprite.transform]=owner.nativeDepth;DragonNestCountdownDrawCountV20++;
    // The original4s is particle life. A bounded single-Sprite slot remains
    // live until source DestroyInstance or owner/world disposal; no fake4s
    // sequence TTL or extra particle emission changes the gameplay state.
   }
   foreach(int key in dragonCountdownRetireV20)RetireDragonCountdownV20(key,false);dragonCountdownRetireV20.Clear();
  }
 }
}
