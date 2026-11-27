using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class DragonEffectParticleEntryV20 {public string key,sourceTemplateSha256,sourceTemplateBlob,particleJson,particleJsonSha256,holdReason,textureKey,textureRgbaSha256;public bool supported;}
 [Serializable] public sealed class DragonEffectParticleBindingV20 {public string effect,sourceField,eventType,particleKey,positionType,sourceDeclarationJson,sourceDeclarationSha256;public int count;public bool held;}
 [Serializable] public sealed class DragonEffectParticleBindingsV20 {public DragonEffectParticleBindingV20[] rows;}
 [Serializable] public sealed class DragonNestEffectParticlesDocumentV20 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,sourceEvidenceSha256,particleSourceManifestSha256;
  public string planeAdapter,triggerAdapter,drawCountAdapter,seedAdapter,originAdapter,lifecycleAdapter,rgbaHashRowOrder,bindingsJson,bindingsJsonSha256;
  public float unit;public int sourceBindingCount,sourceParticleCount,supportedBindingCount,supportedParticleCount,heldBindingCount,heldParticleCount;
  public bool runtimeAdmitted,actualUnityVerified;public DragonEffectParticleEntryV20[] particles;
 }
 public sealed partial class DemoScene {
  static readonly string[] DragonEffectParticleKeysV20={"Major/Bomb_Explosionx2","Major/DamageImpactCoreRed","Major/DamageImpactRed","Major/DamageImpactRingRed","Major/DamageImpactSlashRed","Major/ExplodeImpactRingRed","Major/ExplodeImpactSlashRed","Major/FireDamageImpact","Major/FireDamageImpactCore","Major/FireDamageImpactCoreRed","Major/FireDamageImpactRed","Major/FireDamageImpactRingRed","Major/FireDamageImpactSlash","Major/FireDamageImpactSlashRed","Major/FireDamageSmoke"};
  static readonly string[] DragonEffectParticleBlobHashesV20={"6cd14bbe57ae384dcfc15d764fb673d95a9e009b150729e279f6ed67373499c9","8d3d545b1731ba89d964f423c65a8b596431d25112a96b081acf65d7bba9f78a","2cdada39c3dc8497317a8bf312ac7dae7051195b38dbea27da4f5865138e10be","f19b7ec48b9c26a391e3f26f4f3dc2bdeab7d6bdcdd911da8e65100f47b358e5","12587a04b65b681389f6d3a5cc16d13a9e687676b434e8c6de001a8fe308194a","1a27dd48ac111d3780aa3e395fde7704fa8637a66668b13e4f4f7f93b462181a","c483a7d9a7150d571281900bff3f96bf7fe036a294c9355cf1ac0ba47b4e6cd5","30804586ba8dac7da8bd0076f1d1bc8d83854906b18215a9848eeee760c0fe3a","4d71d24e4f863b98b5bbda38076560473a50ecd82bb15748be9383e6cc43bbfc","7f215b6f5702a37cab02d4472db9f1cd526c9e77829acf9f41d0a30c3a8ae9c6","48bbd5672615a5aa4eaa7dd70868b1283e843e1e7d91ff688f7ca29de20b0ec9","623052c5f35f5a9437499f2d7da71fd28ddb7dbdf5b9f53b730da98d86737434","3d24059a82a35cfd61a25b7828ad0555ba9981e4edc173b3ad912e312057938d","d2315202e64a7da3723f7cb9b75b1454aa05860a6b60cf3f4c4f4cd9d10ede44","ff19695a5e4f3ebf934631cb6ef2ae81e9ad2384016c0a5065535362c0c4ad50"};
  static readonly string[] DragonEffectParticleDataHashesV20={"110a332fcc8ad997704c0897aed8b888e3999435de4482bf84b7fc3543d1bee3","a8e5d47ff5e892d54853d4720646b5a85230585339d300a4424100e62e84fe3c","e6a663b16667878a8f85ca83549bc89894db2123367d57579f286ba12ac74aad","1c88d21b1026a6dbe7abc6aae0ea892e81e2c2843d50507803a76d4a9ec5b1dd","d8a11a2faefbb02b948085893e128a05f80beea3f7739be53f72450b37d57d0b","80b0c931d579c714aa61fb17d6257217ec7736f64aea6a1963427efc9d104e1a","a644fa6decf67638988d9e1886c3ed0bab243f0b00a0266e339d0112429983a7","0154ba730b60fc133f00cc065b5519fbb605d01ecf0d65076eef07221a3797fa","b51322647dbc6ab38d9289e08f8535a25240725b7ab047fd128dcfe469d7a3de","da8a4dbf5b9e0f68e123311a8de977632024b0b6af2a2777bb0d1c368ff90e9f","2ed163f6d6eb10b3ca8f6f35046e8f0ac2e664de2e92ac510f44f350b2487b50","6fbc447fe32cb2c65bac54c7cc5e24d510a0b834f1712f44d852799571baf8c4","ef01fd170aec7896a60590ea973907a78ccb95974195f0112666d35fce8fe276","2381f192473ea49e9f2bf002c7ce25101058cdcc72858e92f161508bb53c0eeb","dae1e27783af780448bc42e221fedbac68dd28c3c6fc83b5de3abf113528d73b"};
  static readonly string[] DragonEffectParticleRgbaHashesV20={"70fa92b257a7969676d8d5066372bcf16ca918ec28ec58a80aa3fb994b6b6a87","84c6b0d656d799e5192cd06a883f33691d91a0af2e9ff3898f167ead56a8b169","e2d51d1309642cb7fce95258144e123e7833009e66eff547f3392ef22583dd7e","23240ba37802b147c9990936a755f4ecb757443f1fce35ffdfa0ed7ac3d9b77a","df12dd4bb0d78f5008aa1b5092afc90cbb1bcb9be124c8478e896076aa4d6f8d","eb2ed17ce805184b2580355af694644977b7141dba63d7ce09080a3243cb3043","63d7ac4243562478ab259d8a7530dfbfcad0d7c671ea59f67f87842e56e76311","c887403a2acc56598ab46883359b963bf069f3a8789f3a0258d0077e5b60e337","7c0149ba3c647dad9c5c0e69a73d8d6005594803d09071c3e8ed2b7ae089b695","7c0149ba3c647dad9c5c0e69a73d8d6005594803d09071c3e8ed2b7ae089b695","ed4ceae15ad16a1c32cb1f6fd35e2a9dde24c658c2d76bdf470a36a52cb2bcc9","eb2ed17ce805184b2580355af694644977b7141dba63d7ce09080a3243cb3043","63d7ac4243562478ab259d8a7530dfbfcad0d7c671ea59f67f87842e56e76311","63d7ac4243562478ab259d8a7530dfbfcad0d7c671ea59f67f87842e56e76311","70fa92b257a7969676d8d5066372bcf16ca918ec28ec58a80aa3fb994b6b6a87"};
  sealed class DragonEffectParticleViewV20 {internal OutskirtsVisualParticle data;internal Sprite sprite;internal Material material;internal bool held;}
  sealed class DragonEffectParticleSequenceV20 {internal DragonEffectParticleViewV20 view;internal string field;internal int eventSequence,serial,seed,facing,count,perTrigger;internal float rate,started,unit,depth;internal Vector3 origin,sourceEuler;internal bool externalGeyser,sourceSetAddRotate,sourceSetAxisAngle;}
  DragonNestEffectParticlesDocumentV20 dragonEffectParticleDocumentV20;DragonEffectParticleBindingV20[] dragonEffectParticleBindingsV20;
  readonly Dictionary<string,DragonEffectParticleViewV20> dragonEffectParticleViewsV20=new Dictionary<string,DragonEffectParticleViewV20>();
  readonly List<DragonEffectParticleSequenceV20> dragonEffectParticleSequencesV20=new List<DragonEffectParticleSequenceV20>();
  readonly List<SpriteRenderer> dragonEffectParticleSpritesV20=new List<SpriteRenderer>();readonly List<Transform> dragonEffectParticleSpriteTransformsV20=new List<Transform>();readonly List<Material> dragonEffectParticleMaterialsV20=new List<Material>();readonly HashSet<string> dragonEffectParticleKindsV20=new HashSet<string>();
  Transform dragonEffectParticleWorldV20,dragonEffectParticleEffectsRootV20;int dragonEffectParticleGenerationV20=-1,dragonEffectParticleWorldSerialV20=-1,dragonEffectParticleRoomV20=-1,dragonEffectParticleLastSequenceV20;float dragonEffectParticleTimeV20=-1;bool dragonEffectParticleEnabledV20;
  partial void CollectDragonNestGeyserSourceParticlesV20();
  partial void ResetDragonNestGeyserParticleCursorsV20(bool discardPast);
  partial void DisposeDragonNestGeyserSourceParticlesV20();
  public int DragonNestEffectParticleDrawFramesV20 {get;private set;}
  public int DragonNestEffectParticleKindsDrawnV20 {get{return dragonEffectParticleKindsV20.Count;}}
  public int DragonNestEffectParticleLiveSequenceCountV20 {get{return dragonEffectParticleSequencesV20.Count;}}
  public int DragonNestEffectParticleVisibleSpriteCountV20 {get;private set;}
  public int DragonNestEffectParticleSpritePoolCountV20 {get{return dragonEffectParticleSpritesV20.Count;}}
  public int DragonNestEffectParticleHeldBindingEventsV20 {get;private set;}
  public int DragonNestEffectParticleConsumedEventsV20 {get;private set;}
  public int DragonNestEffectParticleCreatedSequencesV20 {get;private set;}
  public int DragonNestEffectParticleScheduledBirthsV20 {get;private set;}
  public int DragonNestEffectParticleRejectedEventsV20 {get;private set;}
  public int DragonNestEffectParticleExpiredSequencesV20 {get;private set;}
  public IList<SpriteRenderer> DragonNestEffectParticleSpritesV20 {get{return dragonEffectParticleSpritesV20.AsReadOnly();}}
  static void RequireDragonEffectParticleV20(bool ok,string reason){if(!ok)throw new InvalidOperationException("Dragon effect particles: "+reason);}
  static bool DragonEffectParticleFiniteV20(float value){return !float.IsNaN(value)&&!float.IsInfinity(value);}
  static string DragonEffectParticleHashV20(string value){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(value??""))).Replace("-","").ToLowerInvariant();}
  static string DragonEffectParticleTextureHashV20(Texture2D texture){var pixels=texture.GetPixels32();var bytes=new byte[pixels.Length*4];int n=0;for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){var p=pixels[y*texture.width+x];bytes[n++]=p.r;bytes[n++]=p.g;bytes[n++]=p.b;bytes[n++]=p.a;}using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  static void DestroyDragonEffectParticleObjectV20(UnityEngine.Object item){if(!item)return;if(Application.isPlaying)UnityEngine.Object.Destroy(item);else UnityEngine.Object.DestroyImmediate(item);}
  public void ConfigureDragonNestEffectParticlesV20(DragonNestEffectParticlesDocumentV20 doc,IDictionary<string,Texture2D> textures){
   if(doc==null){DisposeDragonNestEffectParticlesV20();return;}
   RequireDragonEffectParticleV20(doc.schema=="besma-dragon-nest-source-effect-particles-v20"&&doc.sourceRulesSha256=="0a78bd2021b407841b0389f47bdfce44af64ce0a2c352187b2393cbdb069dd77"&&doc.sourceEffectsSha256=="c279abde43dd2f98905a4ee93fe033d606f5f70d9b16fcd1386c1758439eaad4"&&doc.sourceEvidenceSha256=="ebbe209016cc06b5caf79e9e88b8e00de94478c9a4dfe69a686eb965698c623d"&&doc.particleSourceManifestSha256=="6133bf4fbabd5c26e4853663629a4712e9ac5832af3c3066a4501b0311f2a2b3","Changed strong scoped schema/source pins");
   RequireDragonEffectParticleV20(!doc.runtimeAdmitted&&doc.unit==.28f&&doc.sourceBindingCount==38&&doc.sourceParticleCount==15&&doc.supportedBindingCount==36&&doc.supportedParticleCount==14&&doc.heldBindingCount==2&&doc.heldParticleCount==1&&doc.particles!=null&&doc.particles.Length==15,"Changed supported/held source scope");
   RequireDragonEffectParticleV20(doc.planeAdapter=="source-BT_ALL-camera-facing-XY-original-Z-spin-hold-BT_NONE-v20"&&doc.triggerAdapter=="source-trigger-count-times-trigger-window-rate-v20"&&doc.drawCountAdapter=="single-XY-sprite-per-particle-original-multidraw-count-retained-v20"&&doc.seedAdapter=="source-event-sequence-serial-field-deterministic-display-seed-v20"&&doc.originAdapter=="committed-source-presentation-event-xyz-depth-point28-times-scale-v20"&&doc.lifecycleAdapter=="independent-generation-world-room-reset-rewind-discard-old-disable-dispose-v20"&&doc.rgbaHashRowOrder=="top-left-row-major","Undeclared particle display adapter");
   RequireDragonEffectParticleV20(doc.bindingsJsonSha256=="df3a9cbe3c0ee7666d2dd5d7cea76afcde110c6a86db7b79e75a8f4b99ad819c"&&DragonEffectParticleHashV20(doc.bindingsJson)==doc.bindingsJsonSha256,"Changed original38field declaration blob");
   var bindings=JsonUtility.FromJson<DragonEffectParticleBindingsV20>(doc.bindingsJson);RequireDragonEffectParticleV20(bindings!=null&&bindings.rows!=null&&bindings.rows.Length==38&&textures!=null,"Missing source binding/texture document");
   var views=new Dictionary<string,DragonEffectParticleViewV20>();var materials=new List<Material>();var hashes=new Dictionary<Texture2D,string>();
   try{
    for(int i=0;i<15;i++){
     var row=doc.particles[i];RequireDragonEffectParticleV20(row!=null&&row.key==DragonEffectParticleKeysV20[i]&&row.sourceTemplateSha256==DragonEffectParticleBlobHashesV20[i]&&DragonEffectParticleHashV20(row.sourceTemplateBlob)==row.sourceTemplateSha256&&row.particleJsonSha256==DragonEffectParticleDataHashesV20[i]&&DragonEffectParticleHashV20(row.particleJson)==row.particleJsonSha256&&row.textureRgbaSha256==DragonEffectParticleRgbaHashesV20[i],"Changed original template/normalized particle/RGBA pins");
     var p=JsonUtility.FromJson<OutskirtsVisualParticle>(row.particleJson);bool held=row.key=="Major/ExplodeImpactRingRed";
     RequireDragonEffectParticleV20(p!=null&&p.id==row.key&&p.texture==row.textureKey&&p.particleType=="PT_3D_PLANE"&&row.supported==!held&&(held?p.billboardType=="BT_NONE"&&!string.IsNullOrEmpty(row.holdReason):p.billboardType=="BT_ALL"&&string.IsNullOrEmpty(row.holdReason)),"Unsupported plane/basis was silently enabled");
     ValidateLakeParticleV18(p);Texture2D texture;RequireDragonEffectParticleV20(textures.TryGetValue(p.texture,out texture)&&texture&&texture.isReadable,"Missing/unreadable exact source particle texture");
     string hash;if(!hashes.TryGetValue(texture,out hash)){hash=DragonEffectParticleTextureHashV20(texture);hashes.Add(texture,hash);}RequireDragonEffectParticleV20(hash==row.textureRgbaSha256,"Particle source decoded RGBA differs");
     var view=new DragonEffectParticleViewV20{data=p,held=held};views.Add(row.key,view);
     if(!held){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;view.sprite=PixelExplosionVisuals.Plane(p,texture);view.material=OutskirtsMaterial(p.sourceBlend,p.destinationBlend);outskirtsMaterials.Remove(view.material);materials.Add(view.material);}
    }
    foreach(var b in bindings.rows)RequireDragonEffectParticleV20(b!=null&&views.ContainsKey(b.particleKey)&&b.held==views[b.particleKey].held&&b.count>0&&(b.eventType=="spawn"&&b.sourceField.StartsWith("START_PARTICLE")||b.eventType=="hit"&&b.sourceField.StartsWith("HIT_PARTICLE"))&&(b.positionType=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_POS"||b.positionType=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_IMPACT_POINT")&&DragonEffectParticleHashV20(b.sourceDeclarationJson)==b.sourceDeclarationSha256,"Unbound effect-field event/origin/count/hold");
   }catch{foreach(var view in views.Values)DestroyDragonEffectParticleObjectV20(view.sprite);foreach(var m in materials)DestroyDragonEffectParticleObjectV20(m);throw;}
   DisposeDragonNestEffectParticlesV20();dragonEffectParticleDocumentV20=doc;dragonEffectParticleBindingsV20=bindings.rows;foreach(var row in views)dragonEffectParticleViewsV20.Add(row.Key,row.Value);dragonEffectParticleMaterialsV20.AddRange(materials);dragonEffectParticleEnabledV20=true;
   DragonNestEffectParticleDrawFramesV20=DragonNestEffectParticleVisibleSpriteCountV20=DragonNestEffectParticleHeldBindingEventsV20=DragonNestEffectParticleConsumedEventsV20=DragonNestEffectParticleCreatedSequencesV20=DragonNestEffectParticleScheduledBirthsV20=DragonNestEffectParticleRejectedEventsV20=DragonNestEffectParticleExpiredSequencesV20=0;dragonEffectParticleKindsV20.Clear();
  }
  void ClearDragonEffectParticleInstancesV20(){dragonEffectParticleSequencesV20.Clear();foreach(var sprite in dragonEffectParticleSpritesV20)if(sprite)sprite.gameObject.SetActive(false);DragonNestEffectParticleVisibleSpriteCountV20=0;}
  void ClearDragonEffectParticlePoolV20(){
   // Keep CLR Transform references separately: after a parent is destroyed,
   // Unity-null SpriteRenderer.transform itself can no longer be read.
   foreach(var transform in dragonEffectParticleSpriteTransformsV20)if(!ReferenceEquals(transform,null))npcEffectDepths.Remove(transform);
   foreach(var sprite in dragonEffectParticleSpritesV20)if(sprite)RemoveSceneObject(sprite.gameObject);dragonEffectParticleSpritesV20.Clear();dragonEffectParticleSpriteTransformsV20.Clear();DragonNestEffectParticleVisibleSpriteCountV20=0;
  }
  SpriteRenderer DragonEffectParticleSpriteAtV20(int slot){
   while(dragonEffectParticleSpritesV20.Count<=slot){dragonEffectParticleSpritesV20.Add(null);dragonEffectParticleSpriteTransformsV20.Add(null);}
   var sprite=dragonEffectParticleSpritesV20[slot];
   if(!sprite||sprite.transform.parent!=effectsRoot){var old=dragonEffectParticleSpriteTransformsV20[slot];if(!ReferenceEquals(old,null))npcEffectDepths.Remove(old);if(sprite)RemoveSceneObject(sprite.gameObject);sprite=WorldSprite("Dragon original effect Major particle / display seed",effectsRoot,null,44,Vector3.zero);dragonEffectParticleSpritesV20[slot]=sprite;dragonEffectParticleSpriteTransformsV20[slot]=sprite.transform;}
   return sprite;
  }
  int DragonEffectParticleFeedLastV20(){int last=0;foreach(var e in game.npcPresentationEvents)if(e!=null)last=Mathf.Max(last,e.sequence);return last;}
  public void SetDragonNestEffectParticlesEnabledV20(bool enabled){if(dragonEffectParticleDocumentV20==null)return;if(!enabled||enabled!=dragonEffectParticleEnabledV20){ClearDragonEffectParticleInstancesV20();dragonEffectParticleLastSequenceV20=DragonEffectParticleFeedLastV20();ResetDragonNestGeyserParticleCursorsV20(true);if(dragonEffectParticleGenerationV20<0){dragonEffectParticleWorldV20=worldRoot;dragonEffectParticleEffectsRootV20=effectsRoot;dragonEffectParticleGenerationV20=game.NpcPresentationGeneration;dragonEffectParticleWorldSerialV20=game.NpcSoundWorldSerial;dragonEffectParticleRoomV20=game.room;dragonEffectParticleTimeV20=game.elapsed;}}dragonEffectParticleEnabledV20=enabled;}
  public void DisposeDragonNestEffectParticlesV20(){
   ClearDragonEffectParticleInstancesV20();ClearDragonEffectParticlePoolV20();DisposeDragonNestGeyserSourceParticlesV20();foreach(var view in dragonEffectParticleViewsV20.Values)DestroyDragonEffectParticleObjectV20(view.sprite);dragonEffectParticleViewsV20.Clear();foreach(var material in dragonEffectParticleMaterialsV20)DestroyDragonEffectParticleObjectV20(material);dragonEffectParticleMaterialsV20.Clear();
   dragonEffectParticleDocumentV20=null;dragonEffectParticleBindingsV20=null;dragonEffectParticleEnabledV20=false;dragonEffectParticleWorldV20=dragonEffectParticleEffectsRootV20=null;dragonEffectParticleGenerationV20=dragonEffectParticleWorldSerialV20=dragonEffectParticleRoomV20=-1;dragonEffectParticleLastSequenceV20=0;dragonEffectParticleTimeV20=-1;
  }
  static int DragonEffectParticleFieldSeedV20(string field){unchecked{int value=17;foreach(char c in field)value=value*31+c;return value;}}
  static float DragonEffectParticleBirthV20(DragonEffectParticleSequenceV20 sequence,int index){return(index/sequence.perTrigger)/sequence.rate;}
  void AddDragonEffectParticleSequenceV20(NpcPresentationEvent e,DragonEffectParticleBindingV20 binding){
   var view=dragonEffectParticleViewsV20[binding.particleKey];if(view.held){DragonNestEffectParticleHeldBindingEventsV20++;return;}
   int seed;unchecked{seed=e.sequence*997^e.serial*193^DragonEffectParticleFieldSeedV20(binding.sourceField);}var p=view.data;float rate=OutskirtsRange(p.rate,0,seed,1);int per=checked(binding.count*p.triggerCount),count=p.triggerTime>0?checked(Mathf.CeilToInt(p.triggerTime*rate)*per):per;
   var sequence=new DragonEffectParticleSequenceV20{view=view,field=binding.sourceField,eventSequence=e.sequence,serial=e.serial,seed=seed,facing=e.facing,rate=rate,count=count,perTrigger=p.triggerTime>0?per:1,started=e.time,unit=.28f*e.scale,origin=new Vector3(e.x,e.y,0),depth=e.depth+e.z/.28f};
   dragonEffectParticleSequencesV20.Add(sequence);DragonNestEffectParticleCreatedSequencesV20++;DragonNestEffectParticleScheduledBirthsV20+=count;
  }
  public void RenderDragonNestEffectParticlesV20(){
   if(dragonEffectParticleDocumentV20==null||!dragonEffectParticleEnabledV20)return;
   if(!worldRoot||!effectsRoot){ClearDragonEffectParticleInstancesV20();ClearDragonEffectParticlePoolV20();ResetDragonNestGeyserParticleCursorsV20(true);return;}
   bool first=dragonEffectParticleGenerationV20<0,rewind=game.elapsed+.000001f<dragonEffectParticleTimeV20;
   if(first||dragonEffectParticleWorldV20!=worldRoot||dragonEffectParticleEffectsRootV20!=effectsRoot||dragonEffectParticleGenerationV20!=game.NpcPresentationGeneration||dragonEffectParticleWorldSerialV20!=game.NpcSoundWorldSerial||dragonEffectParticleRoomV20!=game.room||rewind){
    bool newFeed=first||dragonEffectParticleGenerationV20!=game.NpcPresentationGeneration;
    ClearDragonEffectParticleInstancesV20();if(dragonEffectParticleWorldV20!=worldRoot||dragonEffectParticleEffectsRootV20!=effectsRoot)ClearDragonEffectParticlePoolV20();dragonEffectParticleWorldV20=worldRoot;dragonEffectParticleEffectsRootV20=effectsRoot;dragonEffectParticleGenerationV20=game.NpcPresentationGeneration;dragonEffectParticleWorldSerialV20=game.NpcSoundWorldSerial;dragonEffectParticleRoomV20=game.room;dragonEffectParticleLastSequenceV20=rewind||!newFeed?DragonEffectParticleFeedLastV20():0;ResetDragonNestGeyserParticleCursorsV20(rewind||!newFeed);
   }
   dragonEffectParticleTimeV20=game.elapsed;
   CollectDragonNestGeyserSourceParticlesV20();
   foreach(var e in game.npcPresentationEvents){
    if(e==null||e.sequence<=dragonEffectParticleLastSequenceV20)continue;
    if(!DragonEffectParticleFiniteV20(e.time)||e.time>game.elapsed+.0001f)continue;dragonEffectParticleLastSequenceV20=e.sequence;
    if(e.type!="spawn"&&e.type!="hit")continue;
    if(e.serial<=0||!DragonEffectParticleFiniteV20(e.x)||!DragonEffectParticleFiniteV20(e.y)||!DragonEffectParticleFiniteV20(e.z)||!DragonEffectParticleFiniteV20(e.depth)||!DragonEffectParticleFiniteV20(e.scale)||e.scale<=0||(e.facing!=1&&e.facing!=-1)){DragonNestEffectParticleRejectedEventsV20++;continue;}
    bool matched=false;foreach(var binding in dragonEffectParticleBindingsV20)if(binding.effect==GameModel.DragonImpactVisualId(e.effect)&&binding.eventType==e.type){matched=true;AddDragonEffectParticleSequenceV20(e,binding);}if(matched)DragonNestEffectParticleConsumedEventsV20++;
   }
   int used=0;
   for(int b=dragonEffectParticleSequencesV20.Count-1;b>=0;b--){
    var sequence=dragonEffectParticleSequencesV20[b];var p=sequence.view.data;float elapsed=game.elapsed-sequence.started,last=DragonEffectParticleBirthV20(sequence,sequence.count-1);
    if(elapsed>last+p.lifetime[1]){dragonEffectParticleSequencesV20.RemoveAt(b);DragonNestEffectParticleExpiredSequencesV20++;continue;}
    if(elapsed<0)continue;int firstIndex=Mathf.Max(0,Mathf.FloorToInt((elapsed-p.lifetime[1])*sequence.rate)*sequence.perTrigger),end=Mathf.Min(sequence.count-1,Mathf.FloorToInt(elapsed*sequence.rate)*sequence.perTrigger+sequence.perTrigger-1),shown=0;
    for(int i=end;i>=firstIndex&&shown<p.capacity;i--){
     float age=elapsed-DragonEffectParticleBirthV20(sequence,i);int seed=unchecked(sequence.seed+i*179);if(age<0||age>=OutskirtsRange(p.lifetime,0,seed,2))continue;shown++;
     float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)continue;
     var sprite=DragonEffectParticleSpriteAtV20(used++);sprite.gameObject.SetActive(true);sprite.enabled=true;sprite.sprite=sequence.view.sprite;sprite.sharedMaterial=sequence.view.material;sprite.flipX=sprite.flipY=false;sprite.sortingOrder=44+(p.layer-14);
     sprite.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);
     sprite.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*sequence.unit/sprite.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*sequence.unit/sprite.sprite.rect.height,1);sprite.transform.localRotation=Quaternion.Euler(0,0,OutskirtsCurve(p,"Rotate",2,age,seed,0)+(sequence.externalGeyser&&sequence.sourceSetAddRotate?sequence.sourceEuler.z:0));
     var offset=Vector3.zero;for(int channel=0;channel<3;channel++)offset[channel]=(OutskirtsRange(p.radius,channel,seed,3)+OutskirtsVelocityIntegral(p,channel,age,seed)+.5f*OutskirtsRange(p.gravityRanges,channel,seed,301)*age*age)*sequence.unit;if(sequence.externalGeyser){if(sequence.sourceSetAxisAngle)offset=Quaternion.Euler(sequence.sourceEuler)*offset;}else offset.x*=sequence.facing;
     var position=sequence.origin+new Vector3(offset.x,offset.y,0);if(p.useLand){var floor=game.SurfaceBelow(position.x,sequence.origin.y+Mathf.Max(0,offset.y));if(floor!=null)position.y=Mathf.Max(position.y,floor.Y(position.x));}
     sprite.transform.localPosition=position;npcEffectDepths[sprite.transform]=sequence.depth+offset.z/.28f;DragonNestEffectParticleDrawFramesV20++;dragonEffectParticleKindsV20.Add(p.id);
    }
   }
   for(int i=used;i<dragonEffectParticleSpritesV20.Count;i++)if(dragonEffectParticleSpritesV20[i])dragonEffectParticleSpritesV20[i].gameObject.SetActive(false);DragonNestEffectParticleVisibleSpriteCountV20=used;
  }
 }
}
