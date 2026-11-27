using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class PlainMineAudioR1302 {
  public string originalName,originalMemberSha256,pcm16Sha256;public int frames,channels,sampleRate;public AudioClip clip;
 }
 // Explicit finite Author2D particle scheduling/RNG/black-hole projection.
 // Every literal range/lifetime/texture below comes from the three exact Minor
 // definitions; this is not a claim about native emitter defaults or RNG.
 public sealed partial class PlainMineMajorViewR1302 {
  sealed class MinorGroup:IPlainMineMinorSequenceR1302 {
   internal PlainMineMajorViewR1302 view;internal Enemy actor;internal GameObject root;internal Vector3 origin,land;internal float born;internal bool landSet,dead;
   public bool IsLive {get{return !dead&&root;}}
   public void SetLandPosition(PlainMineSourcePositionR1269 p){view.Require(actor);Need(IsLive,"dead original Minor handle");FinitePosition(p);land=new Vector3(p.x,p.y,p.z);landSet=true;}
   public void DestroyInstance(){if(dead)return;dead=true;if(root)UnityEngine.Object.Destroy(root);}
  }
  sealed class MinorParticle {
   internal MinorGroup group;internal MinorParticle emitter;internal GameObject root;internal SpriteRenderer renderer;
   internal int kind,liveTails;internal float born,lifetime,nextTrail,trailRate,blackHoleTime,startRotation,endRotation;internal bool finalized;
   internal Vector3 origin,velocity;
  }
  sealed class Voice {internal Enemy actor;internal GameObject root;internal AudioSource source;}
  readonly List<MinorGroup> minorGroupsR1302=new List<MinorGroup>();readonly List<MinorParticle> minorParticlesR1302=new List<MinorParticle>();
  readonly List<Voice> voicesR1302=new List<Voice>();readonly Dictionary<string,AudioClip> clipsR1302=new Dictionary<string,AudioClip>(StringComparer.Ordinal);
  float effectGain=1;
  public void SetEffectGain(float gain){
   effectGain=Mathf.Clamp01(gain);
   foreach(var voice in voicesR1302)if(voice.source)voice.source.volume=effectGain;
  }
  uint particleRandomR1302=0x1302a1u;bool audioPausedR1302,audioMutedR1302;public int MutedSoundRequestsR1302 {get;private set;}
  const string ActualMediaSHA_R1302="95ea4b960a8453cfa74a87d5244e00e4dd9913a77daf5fc3a61fe58ff0f2ee03";
  float RandomR1302(float lo,float hi){particleRandomR1302^=particleRandomR1302<<13;particleRandomR1302^=particleRandomR1302>>17;particleRandomR1302^=particleRandomR1302<<5;return lo+(hi-lo)*(particleRandomR1302&0xffffffu)/16777216f;}
  Vector3 RandomXYZR1302(float lo,float hi){return new Vector3(RandomR1302(lo,hi),RandomR1302(lo,hi),RandomR1302(lo,hi));}
  static string PCMHashR1302(AudioClip clip){
   var values=new float[checked(clip.samples*clip.channels)];Need(clip.GetData(values,0),"whole borrowed PCM read required");var bytes=new byte[checked(values.Length*2)];
   for(int i=0;i<values.Length;i++){float v=values[i];Need(Finite(v)&&v>=-1&&v<=1,"finite source PCM");int n=Mathf.RoundToInt(v*32768);n=Math.Min(32767,Math.Max(-32768,n));bytes[i*2]=(byte)n;bytes[i*2+1]=(byte)(n>>8);}
   using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
  }
  void InitializeOriginalAudioR1302(PlainMineAudioR1302[] audio){
   Need(audio!=null&&audio.Length==2,"exact two original callback clips required");var names=new HashSet<string>(new[]{"MineCountDown.ogg","DieLight.ogg"},StringComparer.Ordinal);
   foreach(var a in audio){Need(a!=null&&names.Remove(a.originalName),"exact unique original sound name");bool beep=a.originalName=="MineCountDown.ogg";
    string original=beep?"fecbbb98c936f53362bf5293420d723c769e6b61d461257c8ebc860bff49c903":"e555d04c7b80dcdc99cd22a6b99c37e44d2004512c2517b2a1316692f6fe6466";
    string pcm=beep?"4cd6529c14e54cdde82914aad99402d3b6589a32347ce43f5c018fe2a00616e0":"5c0ee673e9f39503c593c644ee7d288d2f6f8a85c5f53d3f9737bfb865e2d9e2";
    int frames=beep?1117:175172,rate=beep?22050:44100;
    Need(a.originalMemberSha256==original&&a.pcm16Sha256==pcm&&a.frames==frames&&a.channels==1&&a.sampleRate==rate&&a.clip&&a.clip.loadState==AudioDataLoadState.Loaded&&a.clip.samples==frames&&a.clip.channels==1&&a.clip.frequency==rate&&PCMHashR1302(a.clip)==pcm,"actual full converted PCM identity mismatch");clipsR1302.Add(a.originalName,a.clip);
   }
  }
  public void PlayOriginalSound(Enemy actor,string originalName,PlainMineSourcePositionR1269 position,float modelTime){
   Require(actor);FinitePosition(position);Need(Finite(modelTime)&&modelTime>=0,"finite original callback time");AudioClip clip;Need(clipsR1302.TryGetValue(originalName,out clip),"sound outside original Mine closure");
   SetMutedR1302(mainMutedR1302());if(audioMutedR1302){MutedSoundRequestsR1302++;return;}
   ReapVoicesR1302();Need(voicesR1302.Count<64,"finite original sound voice budget exceeded");var v=new Voice{actor=actor};
   try{v.root=new GameObject("Original Mine sound / "+originalName);v.root.transform.SetParent(parent,false);v.root.transform.position=new Vector3(position.x,position.y,position.z);v.source=v.root.AddComponent<AudioSource>();v.source.playOnAwake=false;v.source.loop=false;v.source.clip=clip;v.source.volume=effectGain;v.source.pitch=1;v.source.spatialBlend=0;voicesR1302.Add(v);v.source.Play();if(audioPausedR1302)v.source.Pause();}
   catch{if(v.root)UnityEngine.Object.Destroy(v.root);voicesR1302.Remove(v);throw;}
  }
  public void SetMutedR1302(bool value){
   audioMutedR1302=value;if(!value)return;var errors=new List<Exception>();
   foreach(var v in voicesR1302){var voice=v;Cleanup(()=>{if(voice.source)voice.source.Stop();},errors);Cleanup(()=>{if(voice.root)UnityEngine.Object.Destroy(voice.root);},errors);}voicesR1302.Clear();
   if(errors.Count>0)throw new AggregateException("Mine owned mute cleanup failed",errors);
  }
  void ReapVoicesR1302(){for(int i=voicesR1302.Count-1;i>=0;i--){var v=voicesR1302[i];if(!v.root||!v.source||!audioPausedR1302&&!v.source.isPlaying){if(v.root)UnityEngine.Object.Destroy(v.root);voicesR1302.RemoveAt(i);}}}
  public void SetPausedR1302(bool value){if(value==audioPausedR1302)return;audioPausedR1302=value;foreach(var v in voicesR1302)if(v.source){if(value)v.source.Pause();else v.source.UnPause();}}
  public IPlainMineMinorSequenceR1302 CreateMinor(Enemy actor,string name,PlainMineSourcePositionR1269 position,float a0,float a1,float b0,float b1,float modelTime){
   Require(actor);FinitePosition(position);Need(name=="DieLight"&&a0==-1&&a1==-1&&b0==3&&b1==-1&&Finite(modelTime)&&modelTime>=0,"exact original DYING_LAND Create arguments");
   if(minorGroupsR1302.Count>=64||minorParticlesR1302.Count>4096-50){NullableBudgetRejections++;return null;}
   var g=new MinorGroup{view=this,actor=actor,origin=new Vector3(position.x,position.y,position.z),born=modelTime};
   try{g.root=new GameObject("Original Mine Minor / DieLight");g.root.layer=28;g.root.transform.SetParent(parent,false);minorGroupsR1302.Add(g);
    // Author2D: a single bounded50-particle burst at Create. NumParticles50,
    // source Emitrate1000/DrawCount1 are retained, not presented as a native
    // emission scheduler proof. The nullable source handle branch is real.
    for(int i=0;i<50;i++){var p=NewMinorParticleR1302(g,null,0,g.origin+RandomXYZR1302(-30,30),new Vector3(RandomR1302(-400,400),RandomR1302(500,1000),RandomR1302(-400,400)),modelTime,2.5f);if(p!=null){p.blackHoleTime=RandomR1302(.2f,.3f);p.trailRate=RandomR1302(20,40);p.nextTrail=modelTime+1/p.trailRate;}}
    return g;
   }catch{g.DestroyInstance();throw;}
  }
  MinorParticle NewMinorParticleR1302(MinorGroup group,MinorParticle emitter,int kind,Vector3 at,Vector3 velocity,float born,float lifetime){
   if(minorParticlesR1302.Count>=4096)return null;var p=new MinorParticle{group=group,emitter=emitter,kind=kind,origin=at,velocity=velocity,born=born,lifetime=lifetime};
   try{p.root=new GameObject(kind==0?"DieLight":kind==1?"DieLightTail":"DieLightFinal");p.root.layer=28;p.root.transform.SetParent(group.root.transform,false);p.renderer=p.root.AddComponent<SpriteRenderer>();p.renderer.sharedMaterial=lightMaterial;p.renderer.sprite=sprites[kind==2?"Star.dds":"Explosion_Sphere.dds"];p.renderer.sortingOrder=14;
    if(kind==1){p.startRotation=RandomR1302(0,100);p.endRotation=RandomR1302(220,240);if(emitter!=null)emitter.liveTails++;}minorParticlesR1302.Add(p);ApplyMinorParticleR1302(p,Math.Max(0,lastTime-born));return p;
   }catch{if(p.root)UnityEngine.Object.Destroy(p.root);throw;}
  }
  Vector3 MinorPositionR1302(MinorParticle p,float age){
   Vector3 at=p.origin+p.velocity*age;
   if(p.kind==0){at.y-=750*age*age;
    // Finite 2D black-hole adaptation: original two BLACKHOLETIME values
    // become convergence time constants, toward the source Create origin.
    // This is explicitly not a reconstruction of native black-hole forces.
    if(age>=1.5f){float tau=age<2.2f?p.blackHoleTime:.05f;at=p.group.origin+(at-p.group.origin)*Mathf.Exp(-(age-1.5f)/tau);}
    if(p.group.landSet)at.y=Math.Max(p.group.land.y,at.y); // finite UseLand projection, no invented bounce.
   }return at;
  }
  void ApplyMinorParticleR1302(MinorParticle p,float age){
   float t=Mathf.Clamp01(age/p.lifetime),size,alpha;
   if(p.kind==0){size=age<=2?150:150*Mathf.Clamp01((2.5f-age)/.5f);alpha=1;}
   else if(p.kind==1){size=150+(10-150)*t;alpha=1-t;}
   else{size=250*t;alpha=age<.25f?1:Mathf.Clamp01((.3f-age)/.05f);}
   // Missing Minor initial Color/rotation/velocity inheritance is a documented
   // Author2D white/zero convention; no native default is manufactured.
   p.root.transform.position=MinorPositionR1302(p,age);p.root.transform.rotation=camera.transform.rotation*Quaternion.Euler(0,0,p.kind==1?p.startRotation+(p.endRotation-p.startRotation)*t:0);var texture=p.renderer.sprite.texture;p.root.transform.localScale=new Vector3(size/texture.width,size/texture.height,1);p.renderer.color=p.kind==2?new Color(alpha,alpha,alpha,alpha):new Color(1,1,1,alpha);
  }
  void DestroyMinorParticleR1302(MinorParticle p){if(p.emitter!=null)p.emitter.liveTails=Math.Max(0,p.emitter.liveTails-1);if(p.root)UnityEngine.Object.Destroy(p.root);}
  void RenderMinorAndAudioR1302(float modelTime){
   ReapVoicesR1302();int count=minorParticlesR1302.Count;
   for(int i=count-1;i>=0;i--){var p=minorParticlesR1302[i];float age=modelTime-p.born;if(!p.group.IsLive){DestroyMinorParticleR1302(p);minorParticlesR1302.RemoveAt(i);continue;}Require(p.group.actor);if(age<0)continue;
    if(p.kind==0){
     while(p.nextTrail<=Math.Min(modelTime,p.born+2.5f)){
      float at=p.nextTrail;p.nextTrail+=1/p.trailRate;
      if(modelTime-at<.2f&&p.liveTails<10)NewMinorParticleR1302(p.group,p,1,MinorPositionR1302(p,at-p.born),RandomXYZR1302(-50,50),at,.2f);
     }
     if(age>=2.5f&&!p.finalized){p.finalized=true;Vector3 at=MinorPositionR1302(p,2.5f);if(modelTime-p.born-2.5f<.3f)for(int n=0;n<2;n++)NewMinorParticleR1302(p.group,null,2,at,Vector3.zero,p.born+2.5f,.3f);}
    }
    if(age>=p.lifetime){DestroyMinorParticleR1302(p);minorParticlesR1302.RemoveAt(i);continue;}ApplyMinorParticleR1302(p,age);
   }
   for(int i=minorGroupsR1302.Count-1;i>=0;i--){var g=minorGroupsR1302[i];if(!g.IsLive||modelTime-g.born>=2.8f){g.DestroyInstance();minorGroupsR1302.RemoveAt(i);}}
  }
  void ClearMinorAndAudioR1302(){var errors=new List<Exception>();foreach(var g in minorGroupsR1302)Cleanup(g.DestroyInstance,errors);minorGroupsR1302.Clear();minorParticlesR1302.Clear();foreach(var v in voicesR1302){var owned=v.root;Cleanup(()=>{if(owned)UnityEngine.Object.Destroy(owned);},errors);}voicesR1302.Clear();audioPausedR1302=false;particleRandomR1302=0x1302a1u;if(errors.Count>0)throw new AggregateException("Mine owned Minor/audio cleanup failed",errors);}
 }
}
