using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel { public sealed partial class DemoScene {
 const string FactoryProductStageMusicIdR1179="factory-private-original-stage-r742",FactoryProductBossMusicIdR1179="factory-private-original-boss-r742";
 sealed class FactoryProductMusicSavedR1179 {
  internal bool engaged,detached,initialized,ready,paused,mapOwned;internal int active,trackChanges;internal float fade,mapVolume;internal string id;
  internal readonly AudioSource[] voices=new AudioSource[2],privateVoices=new AudioSource[2];internal readonly bool[] playing=new bool[2];internal readonly float[] levels=new float[2],from=new float[2],volumes=new float[2];
  internal AudioSource map,privateMap;internal bool mapPlaying;internal GameObject privateRoot;internal readonly Dictionary<string,SceneMusicAsset> priorAssets=new Dictionary<string,SceneMusicAsset>(StringComparer.Ordinal);internal bool assetsInstalled;
 }
  static bool FactoryProductMusicIdR1179(string id){return id==FactoryProductStageMusicIdR1179||id==FactoryProductBossMusicIdR1179;}
  void CaptureFactoryProductMusicR1179(FactoryProductSavedR1179 snapshot){
   var s=new FactoryProductMusicSavedR1179{initialized=musicInitialized,ready=SceneMusicReady,paused=musicPaused,active=musicActive,fade=musicFade,id=musicId,trackChanges=MusicTrackChanges,map=mapMusicPreview,mapOwned=mapMusicOwnsClip};snapshot.music=s;
   for(int i=0;i<2;i++){s.voices[i]=musicVoices[i];s.playing[i]=s.voices[i]&&s.voices[i].isPlaying;s.volumes[i]=s.voices[i]?s.voices[i].volume:0;s.levels[i]=musicLevels[i];s.from[i]=musicFrom[i];}
   s.mapPlaying=s.map&&s.map.isPlaying;s.mapVolume=s.map?s.map.volume:0;foreach(string id in new[]{FactoryProductStageMusicIdR1179,FactoryProductBossMusicIdR1179}){SceneMusicAsset prior;if(sceneMusicAssets.TryGetValue(id,out prior))s.priorAssets.Add(id,prior);}
   s.engaged=true;
   // Preserve original clip/sample/paused state inside the original objects.
   // Only objects observed playing are paused here and UnPaused on restoration.
   for(int i=0;i<2;i++)if(s.playing[i])s.voices[i].Pause();if(s.mapPlaying)s.map.Pause();
   // Invalidate an old in-flight map request without rewinding its generation.
   // The old owned clip remains on the saved source, outside every Stop call.
   mapMusicGeneration++;mapMusicPreview=null;mapMusicOwnsClip=false;
   musicInitialized=true;SceneMusicReady=false;musicPaused=false;musicActive=-1;musicFade=1;musicId="";for(int i=0;i<2;i++)musicLevels[i]=musicFrom[i]=0;
   var root=new GameObject("Factory original music");s.privateRoot=root;factoryProductOwnedR1179.Add(root);root.transform.SetParent(transform,false);
   for(int i=0;i<2;i++){var voice=root.AddComponent<AudioSource>();s.privateVoices[i]=voice;voice.playOnAwake=false;voice.loop=true;voice.spatialBlend=0;voice.priority=100;voice.volume=0;musicVoices[i]=voice;}
   // Original Begin/StopMapMusicPreview may touch only this new owned source.
   var map=root.AddComponent<AudioSource>();s.privateMap=map;map.playOnAwake=false;map.loop=true;map.spatialBlend=0;map.volume=0;mapMusicPreview=map;
  }
  void InstallFactoryProductMusicR1179(IDictionary<string,AudioClip> clips){
   var snapshot=factoryProductSavedR1179;var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged)return;
   foreach(string id in new[]{FactoryProductStageMusicIdR1179,FactoryProductBossMusicIdR1179}){AudioClip clip;FactoryProductNeedR1179(clips.TryGetValue(id,out clip)&&clip,"Private original BGM must be actually loaded by owned OGG path");}
   s.assetsInstalled=true;foreach(string id in new[]{FactoryProductStageMusicIdR1179,FactoryProductBossMusicIdR1179})sceneMusicAssets[id]=new SceneMusicAsset{id=id,resource="",gain=.22f};SceneMusicReady=true;
  }
  bool TryFactoryProductSceneMusicR1179(string id,out AudioClip clip){
   clip=null;if(!factoryProductSessionR1179||!FactoryProductMusicIdR1179(id))return false;
   try{FactoryProductNeedR1179(FactoryProductSessionActiveR1179&&factoryProductSavedR1179!=null&&factoryProductSavedR1179.music!=null&&!factoryProductSavedR1179.music.detached&&audioClips.TryGetValue(id,out clip)&&clip,"Factory private original BGM ownership/load unavailable");return true;}
   catch(Exception trigger){CloseFactoryProductAfterFailureR1179(trigger);throw;}
  }
  void DetachFactoryProductMusicR1179(){
   var snapshot=factoryProductSavedR1179;var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged||s.detached)return;s.detached=true;SceneMusicReady=false;
   // This runs explicitly in End BEFORE DisposeForestPreview destroys clips.
   // The ReleaseWarning entry also covers direct Dispose/loading-failure paths.
   try{if(s.privateVoices[0])try{s.privateVoices[0].Stop();}finally{s.privateVoices[0].clip=null;}}
   finally{try{if(s.privateVoices[1])try{s.privateVoices[1].Stop();}finally{s.privateVoices[1].clip=null;}}
    finally{if(s.privateMap){if(ReferenceEquals(mapMusicPreview,s.privateMap))StopMapMusicPreview();else try{s.privateMap.Stop();}finally{s.privateMap.clip=null;}}}}
  }
  void RestoreFactoryProductMusicR1179(FactoryProductSavedR1179 snapshot){
   var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged)return;
   try{DetachFactoryProductMusicR1179();}finally{
    if(s.assetsInstalled)foreach(string id in new[]{FactoryProductStageMusicIdR1179,FactoryProductBossMusicIdR1179}){SceneMusicAsset prior;if(s.priorAssets.TryGetValue(id,out prior))sceneMusicAssets[id]=prior;else sceneMusicAssets.Remove(id);}
    for(int i=0;i<2;i++){musicVoices[i]=s.voices[i];musicLevels[i]=s.levels[i];musicFrom[i]=s.from[i];}
    musicInitialized=s.initialized;SceneMusicReady=s.ready;musicPaused=s.paused;musicActive=s.active;musicFade=s.fade;musicId=s.id;MusicTrackChanges=s.trackChanges;
    mapMusicGeneration++;mapMusicPreview=s.map;mapMusicOwnsClip=s.mapOwned;
    s.engaged=false;
    // Never restart or UnPause a source originally stopped/paused. Holding its
    // original object also preserves loop/pitch/clip/sample state without seeks.
    for(int i=0;i<2;i++)if(s.voices[i]){s.voices[i].volume=audioMuted?0:s.volumes[i];if(s.playing[i])s.voices[i].UnPause();}
    if(s.map){s.map.volume=audioMuted?0:s.mapVolume;if(s.mapPlaying)s.map.UnPause();}
   }
  }
}}
