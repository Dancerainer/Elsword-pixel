using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  sealed class PlainProductAudioSavedR1320 {
   internal bool engaged,detached,poolPublished,paused,wasGrounded;internal SoundVoice[] voices;internal AudioSource sustain,privateSustain;internal GameObject privateRoot;internal bool[] playing;internal float[] volumes;internal bool sustainPlaying;internal float sustainVolume;
   internal readonly List<SoundVoice> privateVoices=new List<SoundVoice>();
   internal string sustainId,heroClip;internal float heroClock,lastModelTime,hp,x,y,armor,stepDistance;internal int enemyImpact,playerImpact,eventCursor,room;internal CombatEvent lastEvent;
   internal int playCount,peak,dropped,impactPlays,hurtPlays,npcConsumed,npcPlayed,npcMuted,npcCulled,npcMissing;internal int[] materialPlays;
   internal bool[] materialSeen,materialHeavy;internal float[] materialDistance;
   internal readonly Dictionary<string,float> lastPlayed=new Dictionary<string,float>();internal readonly Dictionary<Enemy,bool> enemyStruck=new Dictionary<Enemy,bool>();
  }
  sealed class PlainProductMusicSavedR1320 {
   internal bool engaged,detached,initialized,ready,paused,mapOwned;internal int active,trackChanges;internal float fade,mapVolume;internal string id;
   internal readonly AudioSource[] voices=new AudioSource[2],privateVoices=new AudioSource[2];internal readonly bool[] playing=new bool[2];internal readonly float[] levels=new float[2],from=new float[2],volumes=new float[2];
   internal AudioSource map,privateMap;internal bool mapPlaying;internal GameObject privateRoot;
  }
  void CapturePlainProductAudioR1320(PlainProductSavedR1320 snapshot){
   var s=new PlainProductAudioSavedR1320{voices=audioVoices.ToArray(),sustain=audioSustain,sustainId=audioSustainId,heroClip=audioHeroClip,heroClock=audioHeroClock,lastModelTime=audioLastModelTime,hp=audioHP,x=audioX,y=audioY,armor=audioArmor,stepDistance=audioStepDistance,enemyImpact=audioEnemyImpact,playerImpact=audioPlayerImpact,eventCursor=audioEventCursor,room=audioRoom,lastEvent=audioLastEvent,paused=audioPaused,wasGrounded=audioWasGrounded,playCount=AudioPlayCount,peak=AudioPeakVoices,dropped=AudioDroppedVoices,impactPlays=AudioImpactPlays,hurtPlays=AudioHurtPlays,npcConsumed=NpcAudioConsumed,npcPlayed=NpcAudioPlayed,npcMuted=NpcAudioMuted,npcCulled=NpcAudioCulled,npcMissing=NpcAudioMissing,materialPlays=AudioMaterialImpactPlays,materialSeen=(bool[])impactMaterialsSeen.Clone(),materialHeavy=(bool[])impactMaterialsHeavy.Clone(),materialDistance=(float[])impactMaterialsDistance.Clone()};snapshot.audio=s;
   foreach(var p in audioLastPlayed)s.lastPlayed.Add(p.Key,p.Value);foreach(var p in audioEnemyStruck)s.enemyStruck.Add(p.Key,p.Value);s.playing=new bool[s.voices.Length];s.volumes=new float[s.voices.Length];
   for(int i=0;i<s.voices.Length;i++){var source=s.voices[i].source;s.playing[i]=source&&source.isPlaying;s.volumes[i]=source?source.volume:0;}s.sustainPlaying=s.sustain&&s.sustain.isPlaying;s.sustainVolume=s.sustain?s.sustain.volume:0;s.engaged=true;
   // Preserve the original source objects, their clip/sample positions and paused state.
   for(int i=0;i<s.voices.Length;i++)if(s.playing[i])s.voices[i].source.Pause();if(s.sustainPlaying)s.sustain.Pause();
   var root=new GameObject("Plain product private original SFX voices");s.privateRoot=root;plainProductOwnedR1320.Add(root);root.transform.SetParent(transform,false);
   for(int i=0;i<7;i++){var source=root.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.volume=0;s.privateVoices.Add(new SoundVoice{source=source,impact=i<2});}
   s.privateSustain=root.AddComponent<AudioSource>();s.privateSustain.playOnAwake=false;s.privateSustain.spatialBlend=0;s.privateSustain.loop=true;s.privateSustain.priority=70;
   audioVoices.Clear();audioVoices.AddRange(s.privateVoices);audioSustain=s.privateSustain;audioSustainId="";AudioMaterialImpactPlays=new int[5];s.poolPublished=true;
  }
  void DetachPlainProductAudioR1320(){
   var snapshot=plainProductSavedR1320;var s=snapshot==null?null:snapshot.audio;if(s==null||!s.engaged||s.detached)return;s.detached=true;
   var errors=new List<Exception>();foreach(var voice in s.privateVoices){var source=voice.source;PlainProductCleanupR1320(()=>{if(source){try{source.Stop();}finally{source.clip=null;}}},errors);}PlainProductCleanupR1320(()=>{if(s.privateSustain){try{s.privateSustain.Stop();}finally{s.privateSustain.clip=null;}}},errors);
   if(errors.Count>0)throw new AggregateException("Plain private SFX detach failed",errors);
  }
  void RestorePlainProductAudioR1320(PlainProductSavedR1320 snapshot){
   var s=snapshot==null?null:snapshot.audio;if(s==null||!s.engaged)return;
   try{DetachPlainProductAudioR1320();}finally{
    audioVoices.Clear();audioVoices.AddRange(s.voices);audioSustain=s.sustain;audioSustainId=s.sustainId;audioHeroClip=s.heroClip;audioHeroClock=s.heroClock;audioLastModelTime=s.lastModelTime;audioHP=s.hp;audioX=s.x;audioY=s.y;audioArmor=s.armor;audioStepDistance=s.stepDistance;audioEnemyImpact=s.enemyImpact;audioPlayerImpact=s.playerImpact;audioEventCursor=s.eventCursor;audioRoom=s.room;audioLastEvent=s.lastEvent;audioPaused=s.paused;audioWasGrounded=s.wasGrounded;
    AudioPlayCount=s.playCount;AudioPeakVoices=s.peak;AudioDroppedVoices=s.dropped;AudioImpactPlays=s.impactPlays;AudioHurtPlays=s.hurtPlays;NpcAudioConsumed=s.npcConsumed;NpcAudioPlayed=s.npcPlayed;NpcAudioMuted=s.npcMuted;NpcAudioCulled=s.npcCulled;NpcAudioMissing=s.npcMissing;AudioMaterialImpactPlays=s.materialPlays;
    Array.Copy(s.materialSeen,impactMaterialsSeen,impactMaterialsSeen.Length);Array.Copy(s.materialHeavy,impactMaterialsHeavy,impactMaterialsHeavy.Length);Array.Copy(s.materialDistance,impactMaterialsDistance,impactMaterialsDistance.Length);audioLastPlayed.Clear();foreach(var p in s.lastPlayed)audioLastPlayed.Add(p.Key,p.Value);audioEnemyStruck.Clear();foreach(var p in s.enemyStruck)audioEnemyStruck.Add(p.Key,p.Value);
    s.engaged=false;for(int i=0;i<s.voices.Length;i++){var source=s.voices[i].source;if(source){source.volume=audioMuted?0:s.volumes[i];if(s.playing[i])source.UnPause();}}if(s.sustain){s.sustain.volume=audioMuted?0:s.sustainVolume;if(s.sustainPlaying)s.sustain.UnPause();}
   }
  }
  void CapturePlainProductMusicR1320(PlainProductSavedR1320 snapshot){
   var s=new PlainProductMusicSavedR1320{initialized=musicInitialized,ready=SceneMusicReady,paused=musicPaused,active=musicActive,fade=musicFade,id=musicId,trackChanges=MusicTrackChanges,map=mapMusicPreview,mapOwned=mapMusicOwnsClip};snapshot.music=s;
   for(int i=0;i<2;i++){s.voices[i]=musicVoices[i];s.playing[i]=s.voices[i]&&s.voices[i].isPlaying;s.volumes[i]=s.voices[i]?s.voices[i].volume:0;s.levels[i]=musicLevels[i];s.from[i]=musicFrom[i];}s.mapPlaying=s.map&&s.map.isPlaying;s.mapVolume=s.map?s.map.volume:0;s.engaged=true;
   for(int i=0;i<2;i++)if(s.playing[i])s.voices[i].Pause();if(s.mapPlaying)s.map.Pause();
   // Invalidate an in-flight old map request without destroying its borrowed/owned clip.
   mapMusicGeneration++;mapMusicPreview=null;mapMusicOwnsClip=false;musicInitialized=true;SceneMusicReady=true;musicPaused=false;musicActive=-1;musicFade=1;musicId="";for(int i=0;i<2;i++)musicLevels[i]=musicFrom[i]=0;
   var root=new GameObject("Plain product borrowed Main music");s.privateRoot=root;plainProductOwnedR1320.Add(root);root.transform.SetParent(transform,false);
   for(int i=0;i<2;i++){var voice=root.AddComponent<AudioSource>();s.privateVoices[i]=voice;voice.playOnAwake=false;voice.loop=true;voice.spatialBlend=0;voice.priority=100;voice.volume=0;musicVoices[i]=voice;}
   var map=root.AddComponent<AudioSource>();s.privateMap=map;map.playOnAwake=false;map.loop=true;map.spatialBlend=0;map.volume=0;mapMusicPreview=map;
  }
  void DetachPlainProductMusicR1320(){
   var snapshot=plainProductSavedR1320;var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged||s.detached)return;s.detached=true;SceneMusicReady=false;var errors=new List<Exception>();
   foreach(var voice in s.privateVoices){var source=voice;PlainProductCleanupR1320(()=>{if(source){try{source.Stop();}finally{source.clip=null;}}},errors);}PlainProductCleanupR1320(()=>{if(s.privateMap){try{s.privateMap.Stop();}finally{s.privateMap.clip=null;}}},errors);
   if(ReferenceEquals(mapMusicPreview,s.privateMap)){mapMusicGeneration++;mapMusicPreview=null;mapMusicOwnsClip=false;}
   if(errors.Count>0)throw new AggregateException("Plain private BGM detach failed",errors);
  }
  void RestorePlainProductMusicR1320(PlainProductSavedR1320 snapshot){
   var s=snapshot==null?null:snapshot.music;if(s==null||!s.engaged)return;
   try{DetachPlainProductMusicR1320();}finally{
    for(int i=0;i<2;i++){musicVoices[i]=s.voices[i];musicLevels[i]=s.levels[i];musicFrom[i]=s.from[i];}musicInitialized=s.initialized;SceneMusicReady=s.ready;musicPaused=s.paused;musicActive=s.active;musicFade=s.fade;musicId=s.id;MusicTrackChanges=s.trackChanges;mapMusicGeneration++;mapMusicPreview=s.map;mapMusicOwnsClip=s.mapOwned;s.engaged=false;
    for(int i=0;i<2;i++)if(s.voices[i]){s.voices[i].volume=audioMuted?0:s.volumes[i];if(s.playing[i])s.voices[i].UnPause();}if(s.map){s.map.volume=audioMuted?0:s.mapVolume;if(s.mapPlaying)s.map.UnPause();}
   }
  }
 }
}
