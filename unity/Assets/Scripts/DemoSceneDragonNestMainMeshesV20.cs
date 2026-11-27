using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
 [Serializable] public sealed class DragonNestMainMeshTextureV20 {public string key,path,pngSha256,rgbaSha256;public int width,height;}
 [Serializable] public sealed class DragonNestHeldMeshV20 {
  public string npc,state,sourceCallback,sourceInstructionsSha256,sourceTemplate,mesh,clock,origin,sourceStateEndCallback,sourceStateEndInstructionsSha256;
  public float at,instanceLifetime;public int slot,sourceCreateLayerArgument;
  public bool followsOwnerPositionAndRotation,moveAxisAngleFollowsOwnerRotation,clearOwnerSlotOnStateEnd,nativeInstanceDestroyOnSlotClearProven;
 }
 [Serializable] public sealed class DragonNestMainMeshDocumentV20 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,sourceEvidenceSha256,frozenBindingSha256,sourceDocumentSha256,sourceMeshManifestSha256,sourceHeldProofSha256,sourceCreateSitesSha256,typedMeshIrSha256,typedMeshIrAlgorithm;
  public string ownerRotationAdapter,heldOwnerScaleAdapter,sourceDrawCountAdapter;public float unit;public bool runtimeAdmitted,actualUnityVerified,nativeD3dxMeasured;
  public SourceVisualEffectMesh[] effectMeshes;public DragonNestHeldMeshV20 held;public DragonNestMainMeshTextureV20[] textures;
 }
 public sealed partial class DemoScene {
  const string DragonMainMeshSchemaV20="besma-dragon-nest-main-mesh-consumer-v20";
  const string DragonMainMeshIrV20="ad8cc035512b9a0ad19cb0b01dc32a93cfb74cc775ef8cb500c6848af8de2711";
  static readonly string[] DragonMainMeshIdsV20={"fire-shaman-ball","king-fire-ball","king-big-fire-ball","king-triple-geyser","high-dark-landing","high-dark-held-combo-wind"};
  static readonly string[] DragonMainMeshTemplatesV20={"Mesh_LIZARDMAN_SHAMAN_FIRE_ATTACK_A","MonsterFireBall","MonsterBigFireBall","TripleGeyser","Wally8thJumpAttackLanding","LizardManHighComboA2"};
  static readonly string[] DragonMainMeshEffectsV20={"DamageEffect_LIZARDMAN_SHAMAN_FIRE_ATTACK_A","DamageEffect_LIZARDMAN_SHAMAN_KING_ATTACK_A","DamageEffect_LIZARDMAN_SHAMAN_KING_ATTACK_A_BIG","DamageEffect_LIZARDMAN_SHAMAN_KING_SPECIAL_ATTACK_A","WALLY_8TH_JUMP_ATTACK_LANDING"};
  static readonly string[] DragonMainMeshTextureKeysV20={"components/fire-shaman-ball/atlas-0.png","components/high-dark-held-combo-wind/atlas-0.png","components/high-dark-landing/atlas-0.png","components/king-big-fire-ball/atlas-0.png","components/king-fire-ball/atlas-0.png","components/king-triple-geyser/atlas-0.png"};
  static readonly string[] DragonMainMeshPngV20={"c4e99063e3da9a8d22d7089b845751d5691c26acd30305f3359dabb265957840","1b8d088018bdb7424ce242a5ef00ca0f556852e1774c0da88bb45eb0de25ae0d","8d0f04d9d28c393286c1c17cbaeb0f1320a249c261569b67b272b5dfc35be5b9","63480eca0f985746fc1e0c5923bc434e5fac08f032699ecc2eeae7da5a0ac668","d1b500c22a52c4d9e44c624ae0debb5f899518616953aa9ab03b5c6c9eb7fee9","5ce14b6ddfc6760df797036d1eddc60fe784c2aa56f1baa9b5de86cb814e1343"};
  static readonly string[] DragonMainMeshRgbaV20={"069cdb574e321ce444abf6980776ae22557e7cab1d38d02d6646b489f377e5f7","f3d0cc4f935676aaf0ca7035927d38e748e3da6d4c496c350d7e04eae2ba4168","70ee6495b9d06156e4721ca34430114820962ed21eea867411ff366ca68a9042","27e93284b19eb46ede577a72fdf1f740d662394baea521d48c0b8d9f46dbd15a","393cf2a34b1adbcffd9737f891de02b57578b5253ec2731bb0d64567d83433ae","126d57fecc8e85f4b5aa9ecb565fb2bdb2defde91fb090b6bbeaec80a68b0d3d"};
  static readonly int[] DragonMainMeshWidthsV20={2040,2048,2048,2037,2013,1944},DragonMainMeshHeightsV20={408,728,652,384,120,512};
  sealed class DragonMainMeshViewV20 {internal OutskirtsVisualMesh data;internal Sprite[] sprites;internal Material material;}
  sealed class DragonMainHeldOwnerV20 {internal string state;internal float previous=-1;internal bool created;internal DragonMainHeldInstanceV20 slot;}
  sealed class DragonMainHeldInstanceV20 {internal int serial,facing;internal Enemy owner;internal float started,depth;internal Vector3 origin;internal bool attached;}
  DragonNestMainMeshDocumentV20 dragonMainMeshDocumentV20;
  readonly Dictionary<string,DragonMainMeshViewV20> dragonMainMeshViewsV20=new Dictionary<string,DragonMainMeshViewV20>(StringComparer.Ordinal);
  readonly Dictionary<string,string> dragonMainMeshEffectMapV20=new Dictionary<string,string>(StringComparer.Ordinal);
  readonly List<SpriteRenderer> dragonMainMeshSpritesV20=new List<SpriteRenderer>();
  readonly Dictionary<int,NpcEffectInstance> dragonMainMeshSerialsV20=new Dictionary<int,NpcEffectInstance>();
  readonly Dictionary<int,SpriteRenderer> dragonMainMeshEffectRenderersV20=new Dictionary<int,SpriteRenderer>(),dragonMainMeshHeldRenderersV20=new Dictionary<int,SpriteRenderer>();
  readonly Dictionary<Enemy,DragonMainHeldOwnerV20> dragonMainHeldOwnersV20=new Dictionary<Enemy,DragonMainHeldOwnerV20>();
  readonly List<DragonMainHeldInstanceV20> dragonMainHeldInstancesV20=new List<DragonMainHeldInstanceV20>();
  readonly HashSet<int> dragonMainMeshSeenSerialsV20=new HashSet<int>();readonly HashSet<Enemy> dragonMainMeshSeenOwnersV20=new HashSet<Enemy>();
  readonly List<int> dragonMainMeshRetiredSerialsV20=new List<int>();readonly List<Enemy> dragonMainMeshRetiredOwnersV20=new List<Enemy>();
  Transform dragonMainMeshWorldV20,dragonMainMeshEffectsRootV20;int dragonMainMeshRoomV20=-1,dragonMainMeshGenerationV20=-1;float dragonMainMeshTimeV20=-1;
  public int DragonNestMainMeshEffectDrawFramesV20 {get;private set;}public int DragonNestMainMeshHeldDrawFramesV20 {get;private set;}
  public int DragonNestMainMeshCreatedEffectSerialsV20 {get;private set;}public int DragonNestMainMeshCreatedHeldInstancesV20 {get;private set;}public int DragonNestMainMeshWorldResetCountV20 {get;private set;}
  public int DragonNestMainMeshLiveEffectsV20 {get{return dragonMainMeshSerialsV20.Count;}}public int DragonNestMainMeshHeldLiveInstancesV20 {get{return dragonMainHeldInstancesV20.Count;}}
  public int DragonNestMainMeshSpritePoolCountV20 {get{return dragonMainMeshSpritesV20.Count;}}public int DragonNestMainMeshHeldOwnerSlotsV20 {get{int n=0;foreach(var c in dragonMainHeldOwnersV20.Values)if(c.slot!=null)n++;return n;}}
  public SpriteRenderer DragonNestMainMeshRendererForEffectV20(int serial){SpriteRenderer r;return dragonMainMeshEffectRenderersV20.TryGetValue(serial,out r)?r:null;}
  public SpriteRenderer DragonNestMainMeshRendererForHeldV20(int serial){SpriteRenderer r;return dragonMainMeshHeldRenderersV20.TryGetValue(serial,out r)?r:null;}
  static bool DragonMainFiniteV20(float n){return !float.IsNaN(n)&&!float.IsInfinity(n);}static bool DragonMainNearV20(float a,float b){return DragonMainFiniteV20(a)&&Mathf.Abs(a-b)<.00001f;}
  static void RequireDragonMainV20(bool ok,string reason){if(!ok)throw new InvalidOperationException("Dragon MainMesh: "+reason);}
  static string DragonMainHashV20(byte[] bytes){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  static void DragonMainStringV20(BinaryWriter writer,string s){RequireDragonMainV20(s!=null,"null mesh IR string");var b=Encoding.UTF8.GetBytes(s);writer.Write(b.Length);writer.Write(b);}
  static string DragonMainTypedIrV20(OutskirtsNpcMeshManifest meshes){
   using(var stream=new MemoryStream())using(var writer=new BinaryWriter(stream,Encoding.UTF8,true)){
    DragonMainStringV20(writer,"dragon-nest-main-mesh-typed-ir-v20");writer.Write(meshes.components.Length);
    foreach(var m in meshes.components){RequireDragonMainV20(m!=null&&m.frames!=null,"null mesh IR");foreach(var s in new[]{m.id,m.sourceTemplate,m.sourceBlend,m.destinationBlend})DragonMainStringV20(writer,s);writer.Write(m.fps);writer.Write(m.durationSeconds);writer.Write(m.pixelsPerNativeUnit);writer.Write(m.frames.Length);
     foreach(var f in m.frames){RequireDragonMainV20(f!=null&&f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2,"invalid typed frame IR");DragonMainStringV20(writer,f.atlas);writer.Write(f.timeSeconds);writer.Write(f.durationSeconds);foreach(int n in f.rect)writer.Write(n);foreach(float n in f.anchor)writer.Write(n);}}
    writer.Flush();return DragonMainHashV20(stream.ToArray());
   }
  }
  static string DragonMainTextureHashV20(Texture2D texture){var p=texture.GetPixels32();var b=new byte[p.Length*4];int at=0;for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){var c=p[y*texture.width+x];b[at++]=c.r;b[at++]=c.g;b[at++]=c.b;b[at++]=c.a;}return DragonMainHashV20(b);}
  static void DragonMainDestroyV20(UnityEngine.Object obj){if(!obj)return;if(Application.isPlaying)UnityEngine.Object.Destroy(obj);else UnityEngine.Object.DestroyImmediate(obj);}
  public void ConfigureDragonNestMainMeshesV20(DragonNestMainMeshDocumentV20 doc,OutskirtsNpcMeshManifest meshes,IDictionary<string,Texture2D> textures){
   if(doc==null){DisposeDragonNestMainMeshesV20();return;}
   RequireDragonMainV20(doc.schema==DragonMainMeshSchemaV20&&!doc.runtimeAdmitted&&!doc.nativeD3dxMeasured&&DragonMainNearV20(doc.unit,.28f),"wrong scoped schema/unit");
   RequireDragonMainV20(doc.sourceRulesSha256=="0a78bd2021b407841b0389f47bdfce44af64ce0a2c352187b2393cbdb069dd77"&&doc.sourceEffectsSha256=="c279abde43dd2f98905a4ee93fe033d606f5f70d9b16fcd1386c1758439eaad4"&&doc.sourceEvidenceSha256=="ebbe209016cc06b5caf79e9e88b8e00de94478c9a4dfe69a686eb965698c623d","changed final source pins");
   RequireDragonMainV20(doc.frozenBindingSha256=="ae3c2a82cc6ec318077e4f27284d2716b392099973abc4a4a3fa6b3c6a9d8705"&&doc.sourceDocumentSha256=="883a25b4ccec9d63112e29a083faf9794910f747e4b8a1f3bd8b43d6d0c87df1"&&doc.sourceMeshManifestSha256=="925404c8ba40056d2ebe33045a557b0f7bb643221c2ef914ed161026221f749f"&&doc.sourceHeldProofSha256=="e3fc05f3af6219fb4e282cc73085d66b426bfcc3c615bcd65222ac3fa63e519e"&&doc.sourceCreateSitesSha256=="8539ae9daaaa4a9ba0986705cdcba1e6393ee53912f283e564d063f2207398eb","changed original visual/held creation proof pins");
   RequireDragonMainV20(meshes!=null&&meshes.components!=null&&meshes.components.Length==6&&doc.typedMeshIrSha256==DragonMainMeshIrV20&&DragonMainTypedIrV20(meshes)==DragonMainMeshIrV20,"mesh/frame/anchor IR changed");
   RequireDragonMainV20(doc.effectMeshes!=null&&doc.effectMeshes.Length==5&&doc.textures!=null&&doc.textures.Length==6&&textures!=null&&textures.Count==6,"wrong exact five effects/six pages");
   for(int i=0;i<5;i++){var b=doc.effectMeshes[i];RequireDragonMainV20(b!=null&&b.effect==DragonMainMeshEffectsV20[i]&&b.mesh==DragonMainMeshIdsV20[i]&&!b.compensateSourceAngle&&DragonMainNearV20(b.bakedTemplateRotationZ,0),"effect ID/source mesh mapping changed");}
   var held=doc.held;RequireDragonMainV20(held!=null&&held.npc=="NUI_LIZARDMAN_HIGH_DARK"&&held.state=="LIZARDMAN_HIGH_DARK_SPECIAL_ATTACK_A"&&held.mesh==DragonMainMeshIdsV20[5]&&held.sourceTemplate=="LizardManHighComboA2"&&held.sourceCallback=="LIZARDMAN_HIGH_DARK_SPECIAL_ATTACK_A_FRAME_MOVE"&&held.sourceInstructionsSha256=="eed81ad9fe823c60bb7d74ed4fe5a50eec59982155815a09ab6057b929c7668b"&&held.sourceStateEndCallback=="LIZARDMAN_HIGH_DARK_SPECIAL_ATTACK_A_STATE_END"&&held.sourceStateEndInstructionsSha256=="507f67791dbe861a65553fc6de8997c57f4a3cbb9d7a74b1140679a04db9acd3"&&held.slot==0&&held.sourceCreateLayerArgument==14&&DragonMainNearV20(held.at,.9f)&&DragonMainNearV20(held.instanceLifetime,1.0666666667f)&&held.clock=="source-animation-seconds"&&held.origin=="owner-current-position"&&held.followsOwnerPositionAndRotation&&held.moveAxisAngleFollowsOwnerRotation&&held.clearOwnerSlotOnStateEnd&&!held.nativeInstanceDestroyOnSlotClearProven,"held original timing/IR/slot ownership changed");
   RequireDragonMainV20(doc.ownerRotationAdapter=="Original owner yaw/facing projected to2D facing mirror; no invented X/Z rotation."&&doc.heldOwnerScaleAdapter=="CreateInstance source has no owner-scale argument; template source Size is baked, unit.28 once. Do not apply original NPC UNIT_SCALE or raw spawn1.9 again.","unproved owner/spawn scale or rotation adapter");
   for(int i=0;i<6;i++){var t=doc.textures[i];Texture2D image;RequireDragonMainV20(t!=null&&t.key==DragonMainMeshTextureKeysV20[i]&&t.pngSha256==DragonMainMeshPngV20[i]&&t.rgbaSha256==DragonMainMeshRgbaV20[i]&&t.width==DragonMainMeshWidthsV20[i]&&t.height==DragonMainMeshHeightsV20[i]&&textures.TryGetValue(t.key,out image)&&image,"texture source pin/key changed");image=textures[t.key];RequireDragonMainV20(image.isReadable&&image.width==t.width&&image.height==t.height&&DragonMainTextureHashV20(image)==t.rgbaSha256,"actual texture RGBA differs");}
   DisposeDragonNestMainMeshesV20();DragonNestMainMeshEffectDrawFramesV20=DragonNestMainMeshHeldDrawFramesV20=DragonNestMainMeshCreatedEffectSerialsV20=DragonNestMainMeshCreatedHeldInstancesV20=DragonNestMainMeshWorldResetCountV20=0;
   try{
    foreach(var m in meshes.components){RequireDragonMainV20(m.fps==60&&m.durationSeconds>0&&m.pixelsPerNativeUnit>0,"invalid source clip");var shader=Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite");RequireDragonMainV20(shader,"original blend shader missing");int dst=m.destinationBlend=="D3DBLEND_INVSRCALPHA"?10:m.destinationBlend=="D3DBLEND_DESTALPHA"?7:-1;RequireDragonMainV20(m.sourceBlend=="D3DBLEND_SRCALPHA"&&dst>=0,"unsupported source blend");
     var view=new DragonMainMeshViewV20{data=m,material=new Material(shader){name="Original Dragon MainMesh "+m.id},sprites=new Sprite[m.frames.Length]};view.material.SetInt("_SrcBlend",5);view.material.SetInt("_DstBlend",dst);dragonMainMeshViewsV20.Add(m.id,view);
     for(int i=0;i<m.frames.Length;i++){var f=m.frames[i];var texture=textures["components/"+f.atlas];texture.filterMode=FilterMode.Point;var r=f.rect;RequireDragonMainV20(r[0]>=0&&r[1]>=0&&r[2]>0&&r[3]>0&&r[0]+r[2]<=texture.width&&r[1]+r[3]<=texture.height&&DragonMainFiniteV20(f.anchor[0])&&DragonMainFiniteV20(f.anchor[1]),"invalid source rectangle/finite anchor");view.sprites[i]=Sprite.Create(texture,new Rect(r[0],texture.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1);}
    }
    foreach(var b in doc.effectMeshes)dragonMainMeshEffectMapV20.Add(b.effect,b.mesh);dragonMainMeshDocumentV20=doc;
   }catch{DisposeDragonNestMainMeshesV20();throw;}
  }
  void ClearDragonMainInstancesV20(bool destroyPool){
   dragonMainMeshSerialsV20.Clear();dragonMainMeshEffectRenderersV20.Clear();dragonMainMeshHeldRenderersV20.Clear();dragonMainHeldOwnersV20.Clear();dragonMainHeldInstancesV20.Clear();dragonMainMeshSeenSerialsV20.Clear();dragonMainMeshSeenOwnersV20.Clear();
   foreach(var s in dragonMainMeshSpritesV20)if(s){npcEffectDepths.Remove(s.transform);if(destroyPool)DragonMainDestroyV20(s.gameObject);else s.gameObject.SetActive(false);}if(destroyPool)dragonMainMeshSpritesV20.Clear();
  }
  public void DisposeDragonNestMainMeshesV20(){ClearDragonMainInstancesV20(true);foreach(var v in dragonMainMeshViewsV20.Values){foreach(var s in v.sprites)DragonMainDestroyV20(s);DragonMainDestroyV20(v.material);}dragonMainMeshViewsV20.Clear();dragonMainMeshEffectMapV20.Clear();dragonMainMeshDocumentV20=null;dragonMainMeshWorldV20=dragonMainMeshEffectsRootV20=null;dragonMainMeshRoomV20=dragonMainMeshGenerationV20=-1;dragonMainMeshTimeV20=-1;}
  SpriteRenderer DrawDragonMainMeshV20(ref int used,string id,float age,Vector3 origin,int facing,float unit,float depth,float sx,float sy,float angle){
   var v=dragonMainMeshViewsV20[id];if(age<0||age>=v.data.durationSeconds)return null;int n=Mathf.Clamp(Mathf.FloorToInt(age*v.data.fps),0,v.sprites.Length-1);var f=v.data.frames[n];
   if(used==dragonMainMeshSpritesV20.Count)dragonMainMeshSpritesV20.Add(null);var s=dragonMainMeshSpritesV20[used];if(!s||s.transform.parent!=effectsRoot){if(s){npcEffectDepths.Remove(s.transform);s.gameObject.SetActive(false);DragonMainDestroyV20(s.gameObject);}s=WorldSprite("Original Dragon MainMesh",effectsRoot,null,44,Vector3.zero);dragonMainMeshSpritesV20[used]=s;}used++;
   float scale=unit/v.data.pixelsPerNativeUnit;var rotation=Quaternion.Euler(0,0,angle);var offset=new Vector3((f.rect[2]*.5f-f.anchor[0])*facing*scale*sx,(f.anchor[1]-f.rect[3]*.5f)*scale*sy,0);
   s.gameObject.SetActive(true);s.enabled=true;s.sprite=v.sprites[n];s.sharedMaterial=v.material;s.sortingOrder=44;s.flipX=facing<0;s.color=Color.white;s.transform.localScale=new Vector3(scale*sx,scale*sy,scale);s.transform.localRotation=rotation;s.transform.localPosition=origin+rotation*offset;npcEffectDepths[s.transform]=depth;return s;
  }
  static bool DragonMainOwnerLiveV20(Enemy e){return e.active&&!e.dormant&&e.hp>0&&e.state==CombatState.Normal;}
  void CollectDragonMainHeldV20(){
   var rule=dragonMainMeshDocumentV20.held;dragonMainMeshSeenOwnersV20.Clear();
   foreach(var owner in game.enemies){if(owner.sourceNPC!=rule.npc)continue;dragonMainMeshSeenOwnersV20.Add(owner);DragonMainHeldOwnerV20 c;bool known=dragonMainHeldOwnersV20.TryGetValue(owner,out c);bool fresh=!known||c.state!=owner.npcState||owner.npcTime+.0001f<c.previous;
    if(fresh){if(known&&c.slot!=null){c.slot.attached=false;c.slot=null;}c=new DragonMainHeldOwnerV20{state=owner.npcState};dragonMainHeldOwnersV20[owner]=c;}c.previous=owner.npcTime;
    bool live=DragonMainOwnerLiveV20(owner)&&owner.npcState==rule.state;if(!live){if(c.slot!=null){c.slot.attached=false;c.slot=null;}continue;}
    if(!c.created&&owner.npcTime+.00001f>=rule.at){c.created=true;float began=game.elapsed-owner.npcTime+rule.at;if(game.elapsed-began<rule.instanceLifetime){c.slot=new DragonMainHeldInstanceV20{serial=++DragonNestMainMeshCreatedHeldInstancesV20,owner=owner,started=began,attached=true,origin=new Vector3(owner.x,owner.y,0),depth=owner.nativeDepth,facing=owner.facing};dragonMainHeldInstancesV20.Add(c.slot);}}
   }
   dragonMainMeshRetiredOwnersV20.Clear();foreach(var pair in dragonMainHeldOwnersV20)if(!dragonMainMeshSeenOwnersV20.Contains(pair.Key)){if(pair.Value.slot!=null)pair.Value.slot.attached=false;dragonMainMeshRetiredOwnersV20.Add(pair.Key);}foreach(var owner in dragonMainMeshRetiredOwnersV20)dragonMainHeldOwnersV20.Remove(owner);
   for(int i=dragonMainHeldInstancesV20.Count-1;i>=0;i--){var instance=dragonMainHeldInstancesV20[i];if(game.elapsed-instance.started>=rule.instanceLifetime){DragonMainHeldOwnerV20 c;if(dragonMainHeldOwnersV20.TryGetValue(instance.owner,out c)&&c.slot==instance)c.slot=null;dragonMainHeldInstancesV20.RemoveAt(i);continue;}if(instance.attached){instance.origin=new Vector3(instance.owner.x,instance.owner.y,0);instance.depth=instance.owner.nativeDepth;instance.facing=instance.owner.facing;}}
  }
  public void RenderDragonNestMainMeshesV20(){
   if(dragonMainMeshDocumentV20==null)return;RequireDragonMainV20(game!=null&&worldRoot&&effectsRoot,"missing actual render world");
   if(dragonMainMeshWorldV20!=worldRoot||dragonMainMeshEffectsRootV20!=effectsRoot||dragonMainMeshRoomV20!=game.room||dragonMainMeshGenerationV20!=game.NpcPresentationGeneration||game.elapsed+.0001f<dragonMainMeshTimeV20){ClearDragonMainInstancesV20(dragonMainMeshWorldV20!=worldRoot||dragonMainMeshEffectsRootV20!=effectsRoot);dragonMainMeshWorldV20=worldRoot;dragonMainMeshEffectsRootV20=effectsRoot;dragonMainMeshRoomV20=game.room;dragonMainMeshGenerationV20=game.NpcPresentationGeneration;DragonNestMainMeshWorldResetCountV20++;}dragonMainMeshTimeV20=game.elapsed;
   int used=0;dragonMainMeshSeenSerialsV20.Clear();dragonMainMeshEffectRenderersV20.Clear();dragonMainMeshHeldRenderersV20.Clear();
   foreach(var effect in game.npcEffects){string id;if(effect==null||!dragonMainMeshEffectMapV20.TryGetValue(GameModel.DragonImpactVisualId(effect.id),out id)||effect.ended||effect.age<0||effect.age>=effect.lifetime)continue;RequireDragonMainV20(effect.serial>0&&dragonMainMeshSeenSerialsV20.Add(effect.serial),"duplicate live effect serial");
    NpcEffectInstance original;if(!dragonMainMeshSerialsV20.TryGetValue(effect.serial,out original)){dragonMainMeshSerialsV20.Add(effect.serial,effect);DragonNestMainMeshCreatedEffectSerialsV20++;}else RequireDragonMainV20(ReferenceEquals(original,effect),"effect serial reused inside same world");
    float age=effect.age;var origin=new Vector3(effect.x,effect.y,effect.z);NpcEffectSnapshot previous;if(!calibrationRun&&previousRoom==game.room&&previousNpcEffects.TryGetValue(effect,out previous)&&previous.reflected==effect.reflected&&age>=previous.age){age=Mathf.Lerp(previous.age,age,RenderAlpha);origin=Vector3.Lerp(previous.position,origin,RenderAlpha);}
    float depth=(effect.owner==null?0:effect.owner.nativeDepth)+origin.z/.28f;origin.z=0;float angle=game.NpcEffectVisual(effect,age).angle;var renderer=DrawDragonMainMeshV20(ref used,id,age,origin,effect.facing,.28f*effect.ownerScale,depth,effect.scaleX,effect.scaleY,angle);
    if(renderer){dragonMainMeshEffectRenderersV20.Add(effect.serial,renderer);DragonNestMainMeshEffectDrawFramesV20++;SourceNpcMeshDrawFrames++;}
   }
   dragonMainMeshRetiredSerialsV20.Clear();foreach(int serial in dragonMainMeshSerialsV20.Keys)if(!dragonMainMeshSeenSerialsV20.Contains(serial))dragonMainMeshRetiredSerialsV20.Add(serial);foreach(int serial in dragonMainMeshRetiredSerialsV20)dragonMainMeshSerialsV20.Remove(serial);
   CollectDragonMainHeldV20();foreach(var instance in dragonMainHeldInstancesV20){var renderer=DrawDragonMainMeshV20(ref used,dragonMainMeshDocumentV20.held.mesh,game.elapsed-instance.started,instance.origin,instance.facing,.28f,instance.depth,1,1,0);if(renderer){dragonMainMeshHeldRenderersV20.Add(instance.serial,renderer);DragonNestMainMeshHeldDrawFramesV20++;SourceNpcMeshDrawFrames++;}}
   for(int i=used;i<dragonMainMeshSpritesV20.Count;i++)if(dragonMainMeshSpritesV20[i]){npcEffectDepths.Remove(dragonMainMeshSpritesV20[i].transform);dragonMainMeshSpritesV20[i].gameObject.SetActive(false);}
  }
 }
}
