using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel
{
    public sealed partial class DemoScene
    {
        [Serializable] sealed class AudioManifest {public NativeSound[] assets=new NativeSound[0];public SoundTimeline[] clips=new SoundTimeline[0],cues=new SoundTimeline[0];}
        [Serializable] sealed class NativeSound {public string id="",resource="";}
        [Serializable] sealed class SoundTimeline {public string id="";public TimedSound[] events=new TimedSound[0];}
        [Serializable] sealed class TimedSound {public string sound="";public float time=0,volume=.4f,repeat=0;public bool sustain=false;}
        sealed class SoundVoice {public AudioSource source;public float baseGain;public int importance;public bool impact;public float started;}
        readonly Dictionary<string,AudioClip> audioClips=new Dictionary<string,AudioClip>();
        readonly Dictionary<string,SoundTimeline> audioTimelines=new Dictionary<string,SoundTimeline>(),audioCues=new Dictionary<string,SoundTimeline>();
        readonly Dictionary<string,float> audioLastPlayed=new Dictionary<string,float>();
        readonly Dictionary<Enemy,bool> audioEnemyStruck=new Dictionary<Enemy,bool>();
        readonly List<SoundVoice> audioVoices=new List<SoundVoice>();
        AudioSource audioSustain;string audioSustainId="",audioHeroClip="";float audioHeroClock,audioLastModelTime,audioHP,audioX,audioY,audioArmor,audioStepDistance;
        int audioEnemyImpact,audioPlayerImpact;public int AudioImpactPlays {get;private set;} public int AudioHurtPlays {get;private set;}
        CombatEvent audioLastEvent;int audioEventCursor,audioRoom=-1;bool audioInitialized,audioMuted,audioPaused,audioWasGrounded;
        public bool AudioMuted {get{return audioMuted;}}
        public bool AudioManifestReady {get;private set;}
        public int AudioLoadedClips {get{return audioClips.Count;}}
        public int AudioPlayCount {get;private set;}
        public int AudioPeakVoices {get;private set;}
        public int AudioDroppedVoices {get;private set;}

        public void InitAudio()
        {
            if(audioInitialized)return;audioInitialized=true;audioMuted=PlayerPrefs.GetInt("ElswordPixel.AudioMuted",0)!=0;InitSceneMusic();
            var manifest=Resources.Load<TextAsset>("Pixel/audio/manifest");
            if(!manifest){Debug.LogWarning("Audio manifest absent: run the native audio extraction and Unity staging scripts.");return;}
            var data=JsonUtility.FromJson<AudioManifest>(manifest.text);if(data==null||data.assets==null)return;
            foreach(var entry in data.assets){var clip=Resources.Load<AudioClip>(entry.resource);if(clip)audioClips[entry.id]=clip;else Debug.LogWarning("Missing native SFX: "+entry.resource);}
            if(data.clips!=null)foreach(var t in data.clips)audioTimelines[t.id]=t;
            if(data.cues!=null)foreach(var t in data.cues)audioCues[t.id]=t;
            var group=new GameObject("Native SFX - 8 voice budget");group.transform.SetParent(transform,false);
            for(int i=0;i<7;i++){var a=group.AddComponent<AudioSource>();a.playOnAwake=false;a.spatialBlend=0;a.volume=0;audioVoices.Add(new SoundVoice{source=a,impact=i<2});}
            audioSustain=group.AddComponent<AudioSource>();audioSustain.playOnAwake=false;audioSustain.spatialBlend=0;audioSustain.loop=true;audioSustain.priority=70;
            if(!FindFirstObjectByType<AudioListener>()){var receiver=worldCamera?worldCamera.gameObject:gameObject;receiver.AddComponent<AudioListener>();}
            AudioListener.pause=false;AudioListener.volume=1;AudioManifestReady=audioClips.Count==data.assets.Length;
            if(catalog.npcSounds!=null&&catalog.npcSounds.Length>0){var sourceClips=new Dictionary<string,AudioClip>();foreach(var entry in catalog.npcSounds){if(entry==null||string.IsNullOrEmpty(entry.id)||string.IsNullOrEmpty(entry.resource)||!entry.resource.StartsWith("Pixel/",StringComparison.Ordinal))throw new InvalidOperationException("Invalid packaged NPC sound binding");var clip=Resources.Load<AudioClip>(entry.resource);if(!clip)throw new InvalidOperationException("Missing packaged NPC sound "+entry.resource);sourceClips.Add(entry.id,clip);}RegisterNpcSoundClips(sourceClips);}
            StartCoroutine(LoadNativeContactClip("hitmetaliron1"));StartCoroutine(LoadNativeContactClip("hitbeehouse"));StartCoroutine(LoadNativeContactClip("squish_hit02"));
            ResetAudioTracking();
            Debug.Log("Native SFX ready: "+audioClips.Count+" verified local assets; voice/music excluded.");
        }
        void ResetAudioTracking()
        {
            audioHeroClip="";audioHeroClock=-.00001f;audioEventCursor=game.combatEvents.Count;audioLastEvent=audioEventCursor>0?game.combatEvents[audioEventCursor-1]:null;
            audioRoom=game.room;audioHP=game.player.hp;audioX=game.player.x;audioY=game.player.y;audioArmor=game.armor;audioStepDistance=0;audioLastModelTime=game.elapsed;audioEnemyStruck.Clear();
            audioWasGrounded=game.Grounded(game.player);audioEnemyImpact=game.EnemyImpactSerial;audioPlayerImpact=game.PlayerImpactSerial;
            StopAudioVoices();audioLastPlayed.Clear();
        }
        void StopAudioVoices(){foreach(var v in audioVoices)if(v.source)v.source.Stop();if(audioSustain)audioSustain.Stop();audioSustainId="";}
        public void ToggleAudio()
        {audioMuted=!audioMuted;PlayerPrefs.SetInt("ElswordPixel.AudioMuted",audioMuted?1:0);PlayerPrefs.Save();if(audioMuted){StopAudioVoices();if(interfaceAudio)interfaceAudio.Stop();}else{AudioListener.pause=false;AudioListener.volume=1;}TickSceneMusic();SyncPlainMineAudioMutedR1302();}
        public void TickAudio()
        {
            TickSceneMusic();
            if(!audioInitialized)InitAudio();if(!AudioManifestReady)return;
            if(game.elapsed+.0001f<audioLastModelTime||game.room!=audioRoom)ResetAudioTracking();
            bool paused=game.paused;
            if(paused!=audioPaused)
            {
                audioPaused=paused;foreach(var v in audioVoices){if(paused)v.source.Pause();else v.source.UnPause();}
                if(paused)audioSustain.Pause();else audioSustain.UnPause();
            }
            if(paused)return;
            string clipId=game.player.clip;float current=game.player.clock;
            bool changed=clipId!=audioHeroClip||current+.0001f<audioHeroClock;
            float before=changed?-.00001f:audioHeroClock;
            SoundTimeline timeline;
            if(audioTimelines.TryGetValue(clipId,out timeline)&&timeline.events!=null)
            {
                foreach(var sound in timeline.events)
                {
                    if(sound.sustain)continue;
                    bool crossed=sound.time>before&&sound.time<=current+.00001f;
                    if(sound.repeat>.05f&&before>=sound.time)
                        crossed=(int)Math.Floor((current-sound.time)/sound.repeat)>(int)Math.Floor((before-sound.time)/sound.repeat);
                    if(crossed)PlayNativeSound(sound,80);
                }
            }
            MaintainSustain(timeline,current);
            if(game.armor>audioArmor+.1f&&audioTimelines.TryGetValue("iron-body",out timeline))foreach(var s in timeline.events)PlayNativeSound(s,55);
            if(game.PlayerImpactSerial!=audioPlayerImpact){PlayAudioCue(game.LastPlayerGuard?"guard":"hurt",10);audioPlayerImpact=game.PlayerImpactSerial;AudioHurtPlays++;}
            authoredContacts.Clear();
            ConsumeNpcSoundEvents();
            ConsumeMaterialImpacts();
            bool grounded=game.Grounded(game.player);
            if(!grounded&&audioWasGrounded&&game.player.vy>0)PlayAudioCue("jump",150);
            if(grounded&&!audioWasGrounded)
            {
                SoundTimeline landingSounds;bool authored=audioTimelines.TryGetValue(clipId,out landingSounds)&&landingSounds.events!=null&&landingSounds.events.Length>0;
                if(!authored)PlayAudioCue("land",120);
            }
            float travel=Math.Abs(game.player.x-audioX);
            if(clipId=="run"&&grounded&&travel<40)
            {audioStepDistance+=travel;if(audioStepDistance>=36){audioStepDistance%=36;PlayAudioCue("step",180);}}
            else audioStepDistance=0;
            // Successful contact counters do not depend on diagnostic trace retention.
            foreach(var enemy in game.enemies)
            {bool was;bool known=audioEnemyStruck.TryGetValue(enemy,out was);if(known&&!was&&enemy.struck)PlayAudioCue("enemy-swing",150);audioEnemyStruck[enemy]=enemy.struck;}
            audioHeroClip=clipId;audioHeroClock=current;audioLastModelTime=game.elapsed;audioHP=game.player.hp;audioX=game.player.x;audioY=game.player.y;audioArmor=game.armor;audioWasGrounded=grounded;
            int active=audioSustain.isPlaying?1:0;foreach(var voice in audioVoices)if(voice.source.isPlaying)active++;
            AudioPeakVoices=Math.Max(AudioPeakVoices,active);effectsMix=.85f/Mathf.Sqrt(Math.Max(1,active*.35f));
            ApplyEffectsMix();
        }
        float effectsMix=.85f,lastAppliedMix=-1;
        void ApplyEffectsMix(){
            float gain=effectsMix*effectsVolume;
            if(gain==lastAppliedMix)return;lastAppliedMix=gain;
            foreach(var voice in audioVoices)if(voice.source)voice.source.volume=voice.baseGain*gain;
            if(audioSustain)audioSustain.volume=.28f*gain;
        }
        void ConsumeAudioCombatEvents()
        {
            var events=game.combatEvents;
            if(audioEventCursor>events.Count||(audioEventCursor>0&&events[audioEventCursor-1]!=audioLastEvent))
            {int found=audioLastEvent==null?-1:events.IndexOf(audioLastEvent);audioEventCursor=found<0?0:found+1;}
            for(;audioEventCursor<events.Count;audioEventCursor++)
            {
                var e=events[audioEventCursor];if(e.damage<=0||e.type.IndexOf("burn",StringComparison.OrdinalIgnoreCase)>=0)continue;
                PlayAudioCue(e.state=="Knockdown"||e.rawReaction>=3?"hit-heavy":"hit",110);
            }
            audioLastEvent=events.Count>0?events[events.Count-1]:null;
        }
        bool PlayAudioCue(string id,int importance)
        {bool played=false;SoundTimeline t;if(audioCues.TryGetValue(id,out t)&&t.events!=null)foreach(var s in t.events)played|=PlayNativeSound(s,importance);return played;}
        bool PlayNativeSound(TimedSound sound,int importance) {return PlayNativeSoundAt(sound,importance,0);}
        bool PlayNativeSoundAt(TimedSound sound,int importance,ulong delayAt44100,float pitch=1)
        {
            if(audioMuted||game.paused)return false;AudioClip clip;if(!audioClips.TryGetValue(sound.sound,out clip))return false;
            float now=Time.unscaledTime,last;float minimum=importance>=150?.12f:.065f;
            if(audioLastPlayed.TryGetValue(sound.sound,out last)&&now-last<minimum)return false;
            SoundVoice selected=null;
            if(importance==20)foreach(var voice in audioVoices)
                if(voice.impact&&voice.source.isPlaying&&voice.source.clip==clip&&voice.importance==importance){selected=voice;break;}
            if(selected==null)foreach(var v in audioVoices)if(v.impact==(importance<=20)&&!v.source.isPlaying){selected=v;break;}
            if(selected==null){foreach(var v in audioVoices)if(v.impact==(importance<=20)&&v.importance>=importance&&(selected==null||v.importance>selected.importance||v.importance==selected.importance&&v.started<selected.started))selected=v;}
            if(selected==null){AudioDroppedVoices++;return false;}
            selected.source.Stop();selected.source.clip=clip;selected.source.loop=false;selected.source.pitch=pitch;selected.baseGain=Mathf.Clamp01(sound.volume);selected.importance=importance;selected.source.priority=importance;selected.source.volume=selected.baseGain*effectsMix*effectsVolume;selected.source.Play(delayAt44100);audioLastPlayed[sound.sound]=now;selected.started=now;AudioPlayCount++;return true;
        }
        void MaintainSustain(SoundTimeline timeline,float clock)
        {
            string wanted="";if(!audioMuted&&timeline!=null&&timeline.events!=null)foreach(var s in timeline.events)if(s.sustain&&clock>=s.time)wanted=s.sound;
            if(wanted==audioSustainId)return;audioSustain.Stop();audioSustainId=wanted;AudioClip clip;
            if(wanted!=""&&audioClips.TryGetValue(wanted,out clip)){audioSustain.clip=clip;audioSustain.volume=.28f*effectsMix*effectsVolume;audioSustain.Play();AudioPlayCount++;}
        }
    }
}
