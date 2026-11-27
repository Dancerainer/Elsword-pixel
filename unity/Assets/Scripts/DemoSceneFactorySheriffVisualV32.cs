using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class FactorySheriffTextureV32 {public string key,resource,sha256;public int bytes;}
 [Serializable] public sealed class FactorySheriffVisualDocumentV32 {public string schema,npc,effect,sourceMeshSha256,displayAdapter;public float unit;public OutskirtsVisualMesh mesh;public OutskirtsVisualParticle[] particles;public int[] hitCounts;public FactorySheriffTextureV32[] textures;}
 public sealed partial class DemoScene {
  FactorySheriffVisualDocumentV32 factorySheriffVisualV32;
  readonly List<UnityEngine.Object> factorySheriffAssetsV32=new List<UnityEngine.Object>();
  readonly List<SpriteRenderer> factorySheriffSpritesV32=new List<SpriteRenderer>();
  readonly List<NpcPresentationEvent> factorySheriffImpactsV32=new List<NpcPresentationEvent>();
  Sprite[] factorySheriffLaserV32,factorySheriffParticleSpritesV32;Material factorySheriffLaserMaterialV32;Material[] factorySheriffParticleMaterialsV32;
  int factorySheriffGenerationV32=-1,factorySheriffEventCursorV32;Transform factorySheriffRootV32;
  public int FactorySheriffVisibleLasersV32 {get;private set;}public int FactorySheriffVisibleParticleSpritesV32 {get;private set;}
  static byte[] FactorySheriffBytesV32(string key,string expected,int count){var a=Resources.Load<TextAsset>(key);if(!a)throw new InvalidOperationException("Missing ordinary Sheriff source visual "+key);byte[] b=a.bytes;string h;using(var sha=SHA256.Create())h=BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();if(b.Length!=count||h!=expected)throw new InvalidOperationException("Changed ordinary Sheriff source visual "+key);return b;}
  Material FactorySheriffMaterialV32(string source,string destination){
   var shader=Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite");int dst=destination=="D3DBLEND_INVSRCALPHA"?10:destination=="D3DBLEND_DESTALPHA"?7:destination=="D3DBLEND_ONE"?1:-1;
   FactoryProductNeedR1179(shader&&shader.isSupported&&source=="D3DBLEND_SRCALPHA"&&dst>=0,"ordinary Sheriff original blend unavailable");var m=new Material(shader){name="Ordinary Sheriff source blend"};m.SetInt("_SrcBlend",5);m.SetInt("_DstBlend",dst);factorySheriffAssetsV32.Add(m);return m;
  }
  void ConfigureFactorySheriffVisualV32(){
   DisposeFactorySheriffVisualV32();try{
    var bytes=FactorySheriffBytesV32("Pixel/dungeons/altera-nasod-factory/sheriff-visual-v32/document","9b12ecc00ba6bb2a9f004695d893441777428f76798606de694505775ceed697",20779);
    var d=FactoryTypedJsonR581.ReadFactorySheriffVisualV32(bytes);FactoryProductNeedR1179(d.schema=="ordinary-factory-sheriff-source-visual-v32"&&d.npc=="NUI_NASOD_SHERIFF"&&d.effect=="DamageEffect_NASOD_SHERIFF_ATTACK_A"&&d.unit==.28f&&d.mesh.sourceTemplate=="Small_Laser"&&d.mesh.durationSeconds==.4f&&d.mesh.fps==60&&d.mesh.frames.Length==24&&d.particles.Length==3&&d.hitCounts.Length==3&&d.textures.Length==4&&d.sourceMeshSha256=="124644c57594693d0a8df42122eeed947366c6eb5616fb97487d898932e9df99","exact ordinary Sheriff visual scope");
    var textures=new Dictionary<string,Texture2D>();foreach(var r in d.textures){var b=FactorySheriffBytesV32(r.resource,r.sha256,r.bytes);var t=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Original Sheriff source texture",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};factorySheriffAssetsV32.Add(t);FactoryProductNeedR1179(ImageConversion.LoadImage(t,b,true),"Sheriff source PNG decode");textures.Add(r.key,t);}
    factorySheriffLaserMaterialV32=FactorySheriffMaterialV32(d.mesh.sourceBlend,d.mesh.destinationBlend);factorySheriffLaserV32=new Sprite[24];for(int i=0;i<24;i++){var f=d.mesh.frames[i];var t=textures[f.atlas];var r=f.rect;FactoryProductNeedR1179(r.Length==4&&f.anchor.Length==2&&r[0]>=0&&r[1]>=0&&r[2]>0&&r[3]>0&&r[0]+r[2]<=t.width&&r[1]+r[3]<=t.height,"Sheriff original laser frame rectangle");var sp=Sprite.Create(t,new Rect(r[0],t.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);factorySheriffAssetsV32.Add(sp);factorySheriffLaserV32[i]=sp;}
    factorySheriffParticleSpritesV32=new Sprite[3];factorySheriffParticleMaterialsV32=new Material[3];string[] ids={"FireDamageImpact","FireDamageImpactCore","FireDamageImpactSlash"};int[] counts={10,10,2};
    for(int i=0;i<3;i++){var p=d.particles[i];FactoryProductNeedR1179(p.id==ids[i]&&d.hitCounts[i]==counts[i]&&p.triggerCount==1&&p.triggerTime==0&&p.drawCount==2&&p.particleType=="PT_3D_PLANE"&&p.billboardType=="BT_ALL"&&!p.useLand,"Sheriff original impact field");var t=textures[p.texture];var sp=Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);factorySheriffAssetsV32.Add(sp);factorySheriffParticleSpritesV32[i]=sp;factorySheriffParticleMaterialsV32[i]=FactorySheriffMaterialV32(p.sourceBlend,p.destinationBlend);}
    factorySheriffVisualV32=d;
   }catch{DisposeFactorySheriffVisualV32();throw;}
  }
  void ClearFactorySheriffVisualInstancesV32(){foreach(var s in factorySheriffSpritesV32)if(s){npcEffectDepths.Remove(s.transform);RemoveSceneObject(s.gameObject);}factorySheriffSpritesV32.Clear();factorySheriffImpactsV32.Clear();factorySheriffEventCursorV32=0;FactorySheriffVisibleLasersV32=FactorySheriffVisibleParticleSpritesV32=0;}
  void DisposeFactorySheriffVisualV32(){ClearFactorySheriffVisualInstancesV32();for(int i=factorySheriffAssetsV32.Count-1;i>=0;i--)if(factorySheriffAssetsV32[i])BelowVisualDestroyV21(factorySheriffAssetsV32[i]);factorySheriffAssetsV32.Clear();factorySheriffVisualV32=null;factorySheriffLaserV32=factorySheriffParticleSpritesV32=null;factorySheriffParticleMaterialsV32=null;factorySheriffLaserMaterialV32=null;factorySheriffGenerationV32=-1;factorySheriffRootV32=null;}
  SpriteRenderer FactorySheriffSpriteV32(ref int used,Sprite sp,Material material){
   FactoryProductNeedR1179(used<512,"bounded Sheriff visual capacity");while(factorySheriffSpritesV32.Count<=used)factorySheriffSpritesV32.Add(WorldSprite("Original ordinary Sheriff effect",effectsRoot,null,44,Vector3.zero));var s=factorySheriffSpritesV32[used++];s.gameObject.SetActive(true);s.enabled=true;s.sprite=sp;s.sharedMaterial=material;s.color=Color.white;s.flipX=false;s.sortingOrder=44;return s;
  }
  void RenderFactorySheriffVisualV32(){
   bool factory=FactoryProductSessionActiveR1179;
   if(!factory&&!(coreEntrySessionV21&&forestPreviewActive&&!forestPreviewLoading))return;
   string ownerNpc=factory?"NUI_NASOD_SHERIFF":GameModel.EliteSheriffV35;
   FactoryProductNeedR1179(factorySheriffVisualV32!=null,"Sheriff effects require source visuals before gameplay");
   if(factorySheriffRootV32!=effectsRoot||factorySheriffGenerationV32!=game.NpcPresentationGeneration){ClearFactorySheriffVisualInstancesV32();factorySheriffRootV32=effectsRoot;factorySheriffGenerationV32=game.NpcPresentationGeneration;}
   int used=0;FactorySheriffVisibleLasersV32=FactorySheriffVisibleParticleSpritesV32=0;var d=factorySheriffVisualV32;
   foreach(var e in game.npcEffects){if(e.id!=d.effect||e.owner==null||e.owner.sourceNPC!=ownerNpc||e.ended||e.age<0||e.age>=e.lifetime)continue;int frame=Mathf.Clamp(Mathf.FloorToInt(e.age*60),0,23);var f=d.mesh.frames[frame];var s=FactorySheriffSpriteV32(ref used,factorySheriffLaserV32[frame],factorySheriffLaserMaterialV32);float scale=d.unit*e.ownerScale/d.mesh.pixelsPerNativeUnit;s.flipX=e.facing<0;s.transform.localRotation=Quaternion.identity;s.transform.localScale=new Vector3(scale*e.scaleX,scale*e.scaleY,1);s.transform.localPosition=new Vector3(e.x+(f.rect[2]*.5f-f.anchor[0])*e.facing*scale*e.scaleX,e.y+(f.anchor[1]-f.rect[3]*.5f)*scale*e.scaleY,0);s.color=new Color(1,1,1,e.alpha);npcEffectDepths[s.transform]=e.owner.nativeDepth+e.z/d.unit;FactorySheriffVisibleLasersV32++;}
   foreach(var e in game.npcPresentationEvents){if(e.sequence<=factorySheriffEventCursorV32)continue;factorySheriffEventCursorV32=e.sequence;if(e.type=="hit"&&e.effect==d.effect&&e.owner!=null&&e.owner.sourceNPC==ownerNpc)factorySheriffImpactsV32.Add(e);}
   for(int b=factorySheriffImpactsV32.Count-1;b>=0;b--){var hit=factorySheriffImpactsV32[b];float age=game.elapsed-hit.time;if(age>=.4f){factorySheriffImpactsV32.RemoveAt(b);continue;}if(age<0)continue;
    for(int pindex=0;pindex<3;pindex++){var p=d.particles[pindex];for(int i=0;i<d.hitCounts[pindex];i++){int seed=hit.sequence*67+pindex*7907+i*179;if(age>=OutskirtsRange(p.lifetime,0,seed,2))continue;float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)continue;float unit=d.unit*hit.scale,scale=OutskirtsCurve(p,"Scale",0,age,seed,1);var color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);var offset=new Vector3(OutskirtsRange(p.radius,0,seed,3)+OutskirtsVelocityIntegral(p,0,age,seed),OutskirtsRange(p.radius,1,seed,3)+OutskirtsVelocityIntegral(p,1,age,seed),OutskirtsRange(p.radius,2,seed,3)+OutskirtsVelocityIntegral(p,2,age,seed))*unit;offset.x*=hit.facing;
     // Exact DrawCount two is represented as two coincident 2D billboard draws;
     // seeded RNG and billboard projection are a display adapter, not native RNG parity.
     for(int draw=0;draw<p.drawCount;draw++){var s=FactorySheriffSpriteV32(ref used,factorySheriffParticleSpritesV32[pindex],factorySheriffParticleMaterialsV32[pindex]);s.color=color;s.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*unit/s.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*unit/s.sprite.rect.height,1);s.transform.localRotation=Quaternion.Euler(0,0,hit.facing*OutskirtsCurve(p,"Rotate",2,age,seed,0));s.transform.localPosition=new Vector3(hit.x+offset.x,hit.y+offset.y,0);npcEffectDepths[s.transform]=hit.depth+(hit.z+offset.z)/d.unit;FactorySheriffVisibleParticleSpritesV32++;}
    }}
   }
   for(int i=used;i<factorySheriffSpritesV32.Count;i++)if(factorySheriffSpritesV32[i])factorySheriffSpritesV32[i].gameObject.SetActive(false);
  }
 }
}
