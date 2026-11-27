using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class NpcSteamBurst {public NpcSteamEmitter source;public float started;public Vector3 origin;public float depth;public int seed;}
  NpcSteamVisualData npcSteamVisual;NpcDefinition npcSteamProfile;
  readonly List<SpriteRenderer> npcSteamSprites=new List<SpriteRenderer>();
  readonly List<NpcSteamBurst> npcSteamBursts=new List<NpcSteamBurst>();
  readonly Dictionary<Enemy,float> npcSteamCycles=new Dictionary<Enemy,float>();
  Sprite npcSteamSprite;Material npcSteamMaterial;Transform npcSteamWorldRoot;int npcSteamRoom=-1;float npcSteamLastTime=-1;
  static void RequireNpcSteam(bool condition,string message){if(!condition)throw new InvalidOperationException("NPC Steam: "+message);}
  static bool SteamFinite(float n){return !float.IsNaN(n)&&!float.IsInfinity(n);}
  static bool SteamVector(float[] value,int count){return value!=null&&value.Length==count&&Array.TrueForAll(value,SteamFinite);}
  static void ValidateSteamFades(NpcSteamFade[] fades,int width){
   RequireNpcSteam(fades!=null,"missing fade array");float previous=-1;
   foreach(var fade in fades){RequireNpcSteam(fade!=null&&SteamFinite(fade.start)&&SteamFinite(fade.end)&&fade.start>=0&&fade.end>=fade.start&&fade.start>=previous&&SteamVector(fade.value,width),"invalid source fade");previous=fade.start;}
  }
  void ConfigurePackagedNpcSteamVisual(NpcSteamVisualData data,NpcDefinition[] profiles){
#if UNITY_EDITOR
   // An external session has already verified its pinned DDS/contract; a room
   // rebuild must not replace it with the currently admitted catalog scope.
   if(forestPreviewActive||forestPreviewLoading)return;
#endif
   // JsonUtility may create an empty object for an absent optional nested DTO.
   if(data==null||string.IsNullOrEmpty(data.textureResource)&&(data.emitters==null||data.emitters.Length==0)&&data.unit==0&&data.offsetY==0){DisposeNpcSteamVisual();return;}
   RequireNpcSteam(!string.IsNullOrEmpty(data.textureResource)&&data.textureResource.StartsWith("Pixel/",StringComparison.Ordinal)&&data.textureResource.IndexOf(':')<0&&data.textureResource.IndexOf('\\')<0&&!data.textureResource.Contains(".."),"texture must be a packaged Resource path");
   var texture=Resources.Load<Texture2D>(data.textureResource);RequireNpcSteam(texture,"missing packaged texture "+data.textureResource);
   var profile=profiles==null?null:Array.Find(profiles,p=>p!=null&&p.id=="NUI_STEAM_TRAP");
   ConfigureNpcSteamVisual(data,texture,profile);
  }
  void ConfigureNpcSteamVisual(NpcSteamVisualData data,Texture2D texture,NpcDefinition profile){
   RequireNpcSteam(data!=null&&Mathf.Approximately(data.unit,.28f)&&SteamFinite(data.offsetY)&&texture&&data.emitters!=null&&data.emitters.Length==2,"invalid source visual/texture");
   RequireNpcSteam(profile!=null&&profile.id=="NUI_STEAM_TRAP"&&profile.environmentKind=="timed-steam"&&profile.environment!=null,"missing source Steam rule");
   var attack=Array.Find(profile.states,s=>s.id==profile.environment.activeState);
   RequireNpcSteam(attack!=null&&attack.samples!=null&&attack.samples.Length>0&&attack.samples[0].attackLines!=null&&attack.samples[0].attackLines.Length>=6,"missing source attack-line attachment");
   foreach(var e in data.emitters){
    RequireNpcSteam(e!=null&&!string.IsNullOrEmpty(e.id)&&SteamFinite(e.at)&&e.at>=0&&SteamFinite(e.rate)&&e.rate>0&&SteamFinite(e.lifetime)&&e.lifetime>0&&SteamFinite(e.triggerDuration)&&e.triggerDuration>0&&e.capacity>0&&SteamVector(e.radius,6)&&SteamVector(e.gravity,3)&&SteamVector(e.velocityY,2)&&SteamVector(e.initialSize,3)&&SteamVector(e.initialColor,4)&&SteamVector(e.rotationZ,2),"invalid source emitter");
    ValidateSteamFades(e.sizeFades,3);ValidateSteamFades(e.colorFades,4);ValidateSteamFades(e.rotationFades,3);
   }
   var shader=Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite");RequireNpcSteam(shader,"source particle blend shader missing");
   DisposeNpcSteamVisual();npcSteamSprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),1);
   npcSteamMaterial=new Material(shader){name="Source Steam SrcAlpha / DstAlpha"};npcSteamMaterial.SetInt("_SrcBlend",5);npcSteamMaterial.SetInt("_DstBlend",7);
   npcSteamVisual=data;npcSteamProfile=profile;
  }
  void ClearNpcSteamParticles(){
   foreach(var sprite in npcSteamSprites)if(sprite){npcEffectDepths.Remove(sprite.transform);Destroy(sprite.gameObject);}
   npcSteamSprites.Clear();npcSteamBursts.Clear();npcSteamCycles.Clear();npcSteamWorldRoot=null;npcSteamRoom=-1;npcSteamLastTime=-1;
  }
  void DisposeNpcSteamVisual(){
   ClearNpcSteamParticles();if(npcSteamSprite)Destroy(npcSteamSprite);if(npcSteamMaterial)Destroy(npcSteamMaterial);
   npcSteamSprite=null;npcSteamMaterial=null;npcSteamVisual=null;npcSteamProfile=null;
   // The source texture is owned by Resources or the external preview pool.
  }
  static float NpcSteamSample(int seed,int channel){unchecked{uint n=(uint)(seed*374761393+channel*668265263);n=(n^(n>>13))*1274126177;return(n^(n>>16))/(float)uint.MaxValue;}}
  static float NpcSteamCurve(float initial,NpcSteamFade[] fades,int channel,float age){float value=initial;foreach(var fade in fades){if(age<fade.start)break;float next=fade.value[channel];if(age<fade.end)return Mathf.Lerp(value,next,Mathf.InverseLerp(fade.start,fade.end,age));value=next;}return value;}
  void RenderNpcSteamVisual(){
   if(npcSteamVisual==null||npcSteamProfile==null)return;
   if(npcSteamRoom!=game.room||npcSteamWorldRoot!=worldRoot||game.elapsed+1e-4f<npcSteamLastTime){ClearNpcSteamParticles();npcSteamRoom=game.room;npcSteamWorldRoot=worldRoot;}
   npcSteamLastTime=game.elapsed;
   foreach(var sprite in npcSteamSprites)if(sprite)sprite.enabled=false;
   for(int actor=0;actor<game.enemies.Count;actor++){var e=game.enemies[actor];if(e.sourceNPC!="NUI_STEAM_TRAP"||!e.active||e.dormant)continue;
    if(e.npcState!=npcSteamProfile.environment.activeState){npcSteamCycles.Remove(e);continue;}
    float previous;bool fresh=!npcSteamCycles.TryGetValue(e,out previous)||e.npcTime+1e-4f<previous;npcSteamCycles[e]=e.npcTime;if(!fresh)continue;
    var state=Array.Find(npcSteamProfile.states,s=>s.id==e.npcState);var line=state.samples[0].attackLines;
    foreach(var source in npcSteamVisual.emitters)npcSteamBursts.Add(new NpcSteamBurst{source=source,started=game.elapsed-e.npcTime+source.at,origin=new Vector3(e.x+e.facing*line[0],e.y+line[1]+npcSteamVisual.offsetY,0),depth=e.nativeDepth+line[2]/npcSteamVisual.unit,seed=actor*7919+(int)(game.elapsed*1000)});
   }
   int used=0;
   for(int b=npcSteamBursts.Count-1;b>=0;b--){var burst=npcSteamBursts[b];var source=burst.source;float elapsed=game.elapsed-burst.started;if(elapsed>source.triggerDuration+source.lifetime){npcSteamBursts.RemoveAt(b);continue;}if(elapsed<0)continue;
    int first=Mathf.Max(0,Mathf.CeilToInt((elapsed-source.lifetime)*source.rate));int last=Mathf.Min(Mathf.FloorToInt(elapsed*source.rate),Mathf.CeilToInt(source.triggerDuration*source.rate)-1);
    for(int n=first;n<=last&&n-first<source.capacity;n++){float age=elapsed-n/source.rate;if(age<0||age>=source.lifetime)continue;int seed=burst.seed+n;float alpha=NpcSteamCurve(source.initialColor[3],source.colorFades,3,age);if(alpha<=0)continue;
     while(npcSteamSprites.Count<=used){var go=new GameObject("Source NPC Steam / original source texture");go.transform.SetParent(worldRoot,false);var created=go.AddComponent<SpriteRenderer>();created.sprite=npcSteamSprite;created.sharedMaterial=npcSteamMaterial;npcSteamSprites.Add(created);}
     var sprite=npcSteamSprites[used++];sprite.enabled=true;sprite.sortingOrder=source.layer;sprite.color=new Color(1,1,1,alpha);
     float dx=Mathf.Lerp(source.radius[0],source.radius[1],NpcSteamSample(seed,1)),dy=Mathf.Lerp(source.radius[2],source.radius[3],NpcSteamSample(seed,2)),dz=Mathf.Lerp(source.radius[4],source.radius[5],NpcSteamSample(seed,3));float velocity=Mathf.Lerp(source.velocityY[0],source.velocityY[1],NpcSteamSample(seed,4));
     // Values stay in original units until the single .28 projection. These are
     // visual billboards only; the model's sampled attack line owns damage.
     sprite.transform.localPosition=burst.origin+new Vector3(dx,dy+velocity*age+.5f*source.gravity[1]*age*age,0)*npcSteamVisual.unit;
     float rotation=Mathf.Lerp(source.rotationZ[0],source.rotationZ[1],NpcSteamSample(seed,5));sprite.transform.localRotation=Quaternion.Euler(0,0,NpcSteamCurve(rotation,source.rotationFades,2,age));
     sprite.transform.localScale=new Vector3(NpcSteamCurve(source.initialSize[0],source.sizeFades,0,age)*npcSteamVisual.unit/npcSteamSprite.rect.width,NpcSteamCurve(source.initialSize[1],source.sizeFades,1,age)*npcSteamVisual.unit/npcSteamSprite.rect.height,1);
     npcEffectDepths[sprite.transform]=burst.depth+dz;
    }
   }
  }
 }
}
