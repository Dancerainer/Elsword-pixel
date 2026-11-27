using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
 [Serializable] public sealed class PlainMineTextureR1302 {
  public string originalName,originalMemberSha256,unityBottomUpRgbaSha256;
  public Texture2D texture;
 }
 // A bounded two-template 2D presentation consumer, never a native particle VM.
 // Positions and sizes remain source-world XYZ and use the existing native RT.
 public sealed partial class PlainMineMajorViewR1302:IPlainMinePresentationR1302,IDisposable {
  sealed class Sequence:IPlainMineMajorSequenceR1302 {
   internal PlainMineMajorViewR1302 owner;internal Enemy actor;internal GameObject root;internal SpriteRenderer renderer;
   internal PlainMineSourcePositionR1269 position;internal string name,texture;internal float born,lifetime;internal bool dead;
   public bool IsLive {get{return !dead&&root;}}
   public void SetPosition(PlainMineSourcePositionR1269 p){owner.Require(actor);Need(IsLive,"dead original Mine handle");FinitePosition(p);position=p;}
   public void ChangeTexture(string originalName){owner.Require(actor);Need(IsLive,"dead original Mine handle");if(name!="Mine_Countdown"||originalName!="PC_3.dds"&&originalName!="PC_2.dds"&&originalName!="PC_1.dds")throw new InvalidOperationException("Unbound original Mine texture change");texture=originalName;renderer.sprite=owner.sprites[texture];}
   public void DestroyInstance(){if(dead)return;dead=true;if(root)UnityEngine.Object.Destroy(root);}
  }
  readonly Camera camera;readonly Transform parent;readonly Func<Enemy,bool> context;readonly Func<bool> mainMutedR1302;
  readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>(StringComparer.Ordinal);
  readonly List<Sequence> sequences=new List<Sequence>();
  readonly List<Material> materials=new List<Material>();
  Material lightMaterial,countdownMaterial;bool disposed;float lastTime=float.NegativeInfinity;
  public const string OriginalMajor2SHA="4556ecc2a7c2768f47e47c177a71ea33b8b8d45e0fc97ee5c198adcc606ec9c5";
  public const string OriginalTextureFreezeR1305SHA="de2752aed8934646f3369fe71942597a4b10a757c7bf2bf969601997afd490bf";
  static string OriginalTextureSHA(string name){switch(name){
   case "PC_3.dds":return "cc695a561e31076a6e6b218ebc8de9fbecac930cc018efe54d246f626f44d4f3";
   case "PC_2.dds":return "b06da1cf2ad011bc5939098d8dcb7cc8110830cf39db8b265a22de3e125a9c59";
   case "PC_1.dds":return "8bcfcd8af229b2578e0ce62c5e893d36efb4e5cf5319294ea51b6de1dd426a45";
   case "Particle_Blur.dds":return "654b5256a042de9ff57701da3f8c41c4fde632e6364b8f70e808c1faeb275df6";
   case "Explosion_Sphere.dds":return "a7b20861c31c6abdafbcf4c9f392ba2d9bd84ad8660efe29596e9bc6dba17366";
   case "Star.dds":return "ad155d522eeeae926f7f904f8aeb0fb2645dd166555a31e62e2a2ae4f7acb939";
   case "ColorBallFire.dds":return "84381a3ea5b0e45e67c118e133a80955ea5d138a3ef2fe3d3723186f45baedc0";
   case "smoke02.dds":return "0497abcc20205fd67191b70c278ffe23cbcdec54a3cedae229a6678a256fa050";
   case "Lightning02.tga":return "532948188adc15643e5a89afd06c37c0dfb6a719e55b906148427364c204ebf1";
   default:throw new InvalidOperationException("Texture is outside exact original Mine closure");
  }}
  static bool ExactConvertedTextureR1302(PlainMineTextureR1302 r){switch(r.originalName){
   case "PC_3.dds":return r.texture.width==32&&r.texture.height==32&&r.unityBottomUpRgbaSha256=="5f6d64ff04a3350778cb93b560c38b811d428cbceb43a1c5785788fee443eff6";
   case "PC_2.dds":return r.texture.width==32&&r.texture.height==32&&r.unityBottomUpRgbaSha256=="f06c2ecd09391878821e88f3bee41226683dc4814c1e20756879baa3bf6c9835";
   case "PC_1.dds":return r.texture.width==32&&r.texture.height==32&&r.unityBottomUpRgbaSha256=="6e9ae0c5581ffcdf9a6d9980a661b6d9c514a98b21a31ea9909b036d319fe2df";
   case "Particle_Blur.dds":return r.texture.width==128&&r.texture.height==128&&r.unityBottomUpRgbaSha256=="af9c48c39a5a7be56aa73c93044cf6fe133b27e950f4629f84375d524699beaa";
   case "Explosion_Sphere.dds":return r.texture.width==256&&r.texture.height==256&&r.unityBottomUpRgbaSha256=="f3f6a4a513ccb9178bffceb1acd54ecbcfa872b06b73e055d9dcf196fcfaa678";
   case "Star.dds":return r.texture.width==64&&r.texture.height==64&&r.unityBottomUpRgbaSha256=="a92fe887757e8647bcdadfec456c8012bc440e442fa41fb42dc88a58cab0f412";
   case "ColorBallFire.dds":return r.texture.width==64&&r.texture.height==64&&r.unityBottomUpRgbaSha256=="b3fbf8b3eea2799313dcae12e98c87f4272d5cac35f681bc92b4c84830e7a644";
   case "smoke02.dds":return r.texture.width==64&&r.texture.height==64&&r.unityBottomUpRgbaSha256=="1fc75687493180d79f26c1e7547899e188b3594d8561ad6b0a564c5f3b4b41f3";
   case "Lightning02.tga":return r.texture.width==128&&r.texture.height==128&&r.unityBottomUpRgbaSha256=="cdf86291d78b4c0785dc29768ff383ab7e04d409559291a3d96066ec43b13b18";
   default:return false;
  }}
  public int CreatedSequences {get;private set;}public int NullableBudgetRejections {get;private set;}
  public int LiveSequenceCount {get{return sequences.FindAll(s=>s.IsLive).Count;}}
  static void Need(bool ok,string why){if(!ok)throw new InvalidOperationException("Plain original Mine presentation: "+why);}
  static bool Finite(float n){return !float.IsNaN(n)&&!float.IsInfinity(n);}
  static void FinitePosition(PlainMineSourcePositionR1269 p){Need(Finite(p.x)&&Finite(p.y)&&Finite(p.z),"finite source XYZ required");}
  static bool SHA(string s){if(s==null||s.Length!=64)return false;foreach(char c in s)if(!Uri.IsHexDigit(c))return false;return true;}
  static string RgbaHash(Texture2D t){var colors=t.GetPixels32();var raw=new byte[checked(colors.Length*4)];for(int i=0;i<colors.Length;i++){var c=colors[i];raw[i*4]=c.r;raw[i*4+1]=c.g;raw[i*4+2]=c.b;raw[i*4+3]=c.a;}using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(raw)).Replace("-","").ToLowerInvariant();}
  public PlainMineMajorViewR1302(Camera sourceCamera,Transform sourceWorld,Shader shader,PlainMineTextureR1302[] textures,PlainMineAudioR1302[] audio,PlainMineStateMeshR1302[] stateMeshes,Func<Enemy,bool> exactContext,Func<bool> exactMuted){
   Need(sourceCamera&&sourceWorld&&shader&&shader.isSupported&&shader.name=="ElswordPixel/PlainMineSourceParticleR1302"&&exactContext!=null&&exactMuted!=null,"current source camera/world/shader/context/mute required");
   Need((sourceWorld.lossyScale-Vector3.one).sqrMagnitude<1e-8f,"source world must retain original scale1");camera=sourceCamera;parent=sourceWorld;context=exactContext;mainMutedR1302=exactMuted;
   try{
    Need(textures!=null&&textures.Length==9,"exact nine original Mine textures required");var expected=new HashSet<string>(new[]{"PC_3.dds","PC_2.dds","PC_1.dds","Particle_Blur.dds","Explosion_Sphere.dds","Star.dds","smoke02.dds","ColorBallFire.dds","Lightning02.tga"},StringComparer.Ordinal);
    foreach(var r in textures){Need(r!=null&&expected.Remove(r.originalName)&&r.originalMemberSha256==OriginalTextureSHA(r.originalName)&&SHA(r.unityBottomUpRgbaSha256)&&r.texture&&r.texture.isReadable&&r.texture.filterMode==FilterMode.Point&&r.texture.wrapMode==TextureWrapMode.Clamp&&r.texture.mipmapCount==1,"verified borrowed original texture required");Need(ExactConvertedTextureR1302(r)&&RgbaHash(r.texture)==r.unityBottomUpRgbaSha256,"decoded original RGBA mismatch "+r.originalName);var t=r.texture;sprites.Add(r.originalName,Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect));}
    InitializeOriginalAudioR1302(audio);
    lightMaterial=Material(shader,BlendMode.DstAlpha,CompareFunction.LessEqual);countdownMaterial=Material(shader,BlendMode.OneMinusSrcAlpha,CompareFunction.Always);InitializeStateMeshesR1302(stateMeshes,shader);
   }catch(Exception trigger){try{Dispose();}catch(Exception cleanup){throw new AggregateException("Mine presentation initialization and cleanup failed",trigger,cleanup);}throw;}
  }
  Material Material(Shader shader,BlendMode destination,CompareFunction depth){var m=new Material(shader);materials.Add(m);m.SetInt("_SrcBlend",(int)BlendMode.SrcAlpha);m.SetInt("_DstBlend",(int)destination);m.SetInt("_ZTest",(int)depth);return m;}
  void Require(Enemy actor){Need(!disposed&&camera&&parent&&actor!=null&&context(actor),"stale or foreign source actor/world");}
  public IPlainMineMajorSequenceR1302 Create(Enemy actor,string name,PlainMineSourcePositionR1269 position,float a0,float a1,float b0,float b1,float sourceLandY,float modelTime){
   Require(actor);FinitePosition(position);Need(Finite(modelTime)&&modelTime>=0&&Finite(sourceLandY),"finite original time/land required");
   bool light=name=="Light_Altera_Mine01",countdown=name=="Mine_Countdown";Need((light||countdown)&&a0==-1&&a1==-1&&b0==(light?1:-1)&&b1==-1,"exact original CreateSequence call arguments required");
   // Light's TriggerCount is explicitly1. Countdown is the source capacity-one
   // sequence, explicitly destroyed by its original callback before Lifetime4.
   // Literal vec2 arguments are checked, not assigned unproved ABI meanings.
   if(sequences.Count>=128){NullableBudgetRejections++;return null;}
   var s=new Sequence{owner=this,actor=actor,name=name,position=position,born=modelTime,lifetime=light?1:4,texture=light?"Particle_Blur.dds":"PC_3.dds"};
   try{s.root=new GameObject("Original Mine / "+name);s.root.layer=28;s.root.transform.SetParent(parent,false);s.renderer=s.root.AddComponent<SpriteRenderer>();s.renderer.sharedMaterial=light?lightMaterial:countdownMaterial;s.renderer.sprite=sprites[s.texture];s.renderer.sortingOrder=14;sequences.Add(s);CreatedSequences++;Apply(s,0);return s;}catch{s.DestroyInstance();throw;}
  }
  void Apply(Sequence s,float age){
   bool light=s.name=="Light_Altera_Mine01";float size=80,alpha=1;
   if(light){alpha=age<.01f?0:age<.3f?(age-.01f)/.29f:age<.7f?1:Mathf.Clamp01((1-age)/.3f);size=age<.3f?80:age<.6f?80+20*(age-.3f)/.3f:100;}
   s.root.transform.position=new Vector3(s.position.x,s.position.y,s.position.z);s.root.transform.rotation=camera.transform.rotation;
   var texture=s.renderer.sprite.texture;s.root.transform.localScale=new Vector3(size/texture.width,size/texture.height,1);s.renderer.color=new Color(1,0,0,alpha);
  }
  public void RenderFrame(float modelTime){
   Need(!disposed&&Finite(modelTime)&&modelTime>=lastTime,"monotonic current model time required");lastTime=modelTime;
   SetMutedR1302(mainMutedR1302());RenderMinorAndAudioR1302(modelTime);RenderStateParticlesR1302(modelTime);
   for(int i=sequences.Count-1;i>=0;i--){var s=sequences[i];if(!s.IsLive){sequences.RemoveAt(i);continue;}Require(s.actor);float age=modelTime-s.born;if(age<0)continue;if(age>=s.lifetime){s.DestroyInstance();sequences.RemoveAt(i);continue;}Apply(s,age);}
  }
  static void Cleanup(Action action,List<Exception> errors){try{action();}catch(Exception e){errors.Add(e);}}
  public void Clear(){var errors=new List<Exception>();foreach(var s in sequences)Cleanup(s.DestroyInstance,errors);sequences.Clear();Cleanup(ClearMinorAndAudioR1302,errors);Cleanup(ClearStateParticlesR1302,errors);lastTime=float.NegativeInfinity;if(errors.Count>0)throw new AggregateException("Mine sequence cleanup failed",errors);}
  public void Dispose(){if(disposed)return;disposed=true;var errors=new List<Exception>();Cleanup(Clear,errors);Cleanup(DisposeStateMeshesR1302,errors);foreach(var s in sprites.Values){var owned=s;Cleanup(()=>{if(owned)UnityEngine.Object.Destroy(owned);},errors);}sprites.Clear();foreach(var m in materials){var owned=m;Cleanup(()=>{if(owned)UnityEngine.Object.Destroy(owned);},errors);}materials.Clear();clipsR1302.Clear();if(errors.Count>0)throw new AggregateException("Mine owned presentation cleanup failed",errors);}
 }
}
