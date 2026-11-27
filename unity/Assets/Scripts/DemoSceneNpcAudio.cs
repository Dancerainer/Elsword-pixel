using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        public int NpcAudioConsumed {get;private set;}
        public int NpcAudioPlayed {get;private set;}
        public int NpcAudioMuted {get;private set;}
        public int NpcAudioCulled {get;private set;}
        public int NpcAudioMissing {get;private set;}

        // Accept already-loaded clips only. Candidate filesystem paths belong
        // to Editor diagnostics, never the runtime catalog or this consumer.
        public void RegisterNpcSoundClips(IDictionary<string,AudioClip> clips){
            if(clips==null)throw new ArgumentNullException("clips");
            if(!audioInitialized||!AudioManifestReady)throw new InvalidOperationException("Initialize the existing audio pool before registering NPC clips");
            foreach(var p in clips){AudioClip existing;
                if(string.IsNullOrEmpty(p.Key)||!p.Value)throw new ArgumentException("Invalid NPC sound clip");
                if(audioClips.TryGetValue(p.Key,out existing)&&existing!=p.Value)throw new ArgumentException("NPC sound ID conflicts with a loaded clip: "+p.Key);
            }
            foreach(var p in clips)audioClips[p.Key]=p.Value;
        }

        void ConsumeNpcSoundEvents(){
            if(game.PendingNpcSoundEvents==0)return;
            NpcSoundEvent cue;
            while(game.TryDequeueNpcSound(out cue)){
                NpcAudioConsumed++;
                if(cue.sourceField=="PlaySound_LUA" || cue.sourceField=="Play2DSound_LUA" || cue.sourceField=="StopSound_LUA") {
                    if(activeDungeonRegion==null)continue;
                    cue.sound="Pixel/"+activeDungeonRegion+"/sounds/"+cue.sound;
                    if(cue.sourceField=="StopSound_LUA") {
                        if(audioClips.TryGetValue(cue.sound,out var playing))
                            foreach(var voice in audioVoices)
                                if(voice.source.clip==playing && voice.source.isPlaying)voice.source.Stop();
                        continue;
                    }
                }
                // Draining while muted prevents stale cues when unmuting.
                if(audioMuted){NpcAudioMuted++;
#if UNITY_EDITOR
                    ForestAutoRecordNpcAudio("muted");
#endif
                    continue;}
                AudioClip clip;
                if (!audioClips.TryGetValue(cue.sound,out clip) &&
                    (cue.sound.StartsWith("Pixel/peita/sounds/",StringComparison.Ordinal) ||
                     cue.sound.StartsWith("Pixel/velder/sounds/",StringComparison.Ordinal) ||
                     cue.sound.StartsWith("Pixel/hamel/sounds/",StringComparison.Ordinal))) {
                    clip=Resources.Load<AudioClip>(cue.sound);
                    if(clip) audioClips[cue.sound]=clip;
                }
                if(!clip){NpcAudioMissing++;
#if UNITY_EDITOR
                    ForestAutoRecordNpcAudio("missing");
#endif
                    continue;}
                int before=AudioPlayCount;
                // Existing pool reserves two impact voices (importance <=20).
                // Hero skills (80) can replace NPC cues (140), never vice versa.
                bool contact=AuthoredContact(cue);
                bool played=PlayNativeSound(new TimedSound{sound=cue.sound,volume=contact?.58f:.38f},contact?20:140);
                if(contact&&played){authoredContacts.Add(cue.actorSerial);AudioImpactPlays++;}
                if(AudioPlayCount>before)NpcAudioPlayed++;else NpcAudioCulled++;
#if UNITY_EDITOR
                ForestAutoRecordNpcAudio(AudioPlayCount>before?"played":"culled");
#endif
            }
        }
    }
}
