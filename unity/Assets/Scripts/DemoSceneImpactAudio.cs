using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly bool[] impactMaterialsSeen = new bool[5], impactMaterialsHeavy = new bool[5];
        readonly float[] impactMaterialsDistance = new float[5];
        readonly ImpactTone[] impactTones = new ImpactTone[5];
        readonly HashSet<int> authoredContacts = new HashSet<int>();
        readonly List<AudioClip> ownedContactClips = new List<AudioClip>();
        public int[] AudioMaterialImpactPlays = new int[5];

        IEnumerator LoadNativeContactClip(string id) {
            #if UNITY_ANDROID && !UNITY_EDITOR
            string uri = "jar:" + new Uri(Application.dataPath).AbsoluteUri + "!/assets/impact-audio/" + id + ".wav";
#else
            string path = Application.streamingAssetsPath.TrimEnd('/') + "/impact-audio/" + id + ".wav";
            string uri = path.Contains("://") ? path : new Uri(Path.GetFullPath(path)).AbsoluteUri;
#endif
            using(var request = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV)) {
                yield return request.SendWebRequest();
                if(request.result != UnityWebRequest.Result.Success) {
                    Debug.LogWarning("Could not load original NPC contact " + id + ": " + request.error);
                    yield break;
                }
                var clip = DownloadHandlerAudioClip.GetContent(request);
                if(!clip) yield break;
                clip.name=id;
                ownedContactClips.Add(clip);
                audioClips[id]=clip;
            }
        }

        void DisposeContactClips() {
            foreach(var clip in ownedContactClips) if(clip) Destroy(clip);
            ownedContactClips.Clear();
        }

        // Only explicit contact sounds in a damage/guard state replace the ordinary material voice.
        // A roar or a later landing keeps its own source timeline.
        static bool AuthoredContact(NpcSoundEvent cue) {
            if(cue.kind!="state" || string.IsNullOrEmpty(cue.state) ||
               (cue.state.IndexOf("DAMAGE",StringComparison.OrdinalIgnoreCase)<0 && cue.state.IndexOf("GUARD",StringComparison.OrdinalIgnoreCase)<0))
                return false;
            string name=Path.GetFileNameWithoutExtension(cue.sound);
            switch(name) {
                case "durahan_shieldhit": case "durahan_hitted01": case "glitter_shieldhit":
                case "nasod_guard_shieldhit": case "hit_wood01": case "hit_wood02": case "hit_wood03":
                case "ice_pillar_trap_damage01": case "squish_hit01": case "squish_hit02": case "shadow_walkerhit01": case "hit_lightball": return true;
                default: return false;
            }
        }

        bool PlayContact(ImpactTone tone, bool heavy) {
            string sound=null, fallback;
            float gain=.58f,pitch=1;
            switch(tone) {
                case ImpactTone.IronArmor: sound="hitmetaliron1"; gain=heavy?.68f:.62f; fallback="metal"; break;
                case ImpactTone.SoldierPlate:
                    sound=heavy?"hitmetal2":"hitmetaliron1";
                    gain=heavy?.44f:.37f;pitch=heavy?1.16f:1.3f;fallback="metal";break;
                case ImpactTone.CastleMachine:
                    sound=heavy?"hitmetal2":"hitmetal";
                    gain=heavy?.48f:.41f;pitch=heavy?.84f:.91f;fallback="metal";break;
                case ImpactTone.DenseMetal: sound="hitmetal2"; fallback="metal"; break;
                case ImpactTone.DenseStone: sound="hitstone2"; fallback="shell"; break;
                case ImpactTone.Hive: sound="hitbeehouse"; fallback="wood"; break;
                case ImpactTone.CocoonHurt: sound="squish_hit02"; fallback="shell"; break;
                case ImpactTone.KnightShield: sound="Pixel/peita/sounds/durahan_shieldhit"; fallback="metal"; break;
                case ImpactTone.GlitterShield: sound="Pixel/velder/sounds/glitter_shieldhit"; fallback="metal"; break;
                default: fallback=tone.ToString().ToLowerInvariant(); break;
            }
            if(sound!=null) {
                AudioClip clip;
                if(!audioClips.TryGetValue(sound,out clip) && sound.StartsWith("Pixel/",StringComparison.Ordinal)) {
                    clip=Resources.Load<AudioClip>(sound);
                    if(clip) audioClips[sound]=clip;
                }
                // Original Gochi SOUND_PLAY0 is 0.01 s after the damage-pose entry.
                // AudioSource.Play uses 44100 samples per delay second, independently of output rate.
                if(clip) return PlayNativeSoundAt(new TimedSound{sound=sound,volume=gain},20,tone==ImpactTone.CocoonHurt?441UL:0UL,pitch);
            }
            return PlayAudioCue("hit-"+fallback+(heavy?"-heavy":""),20);
        }

        void ConsumeMaterialImpacts() {
            if(game.EnemyImpactSerial==audioEnemyImpact) return;
            Array.Clear(impactMaterialsSeen,0,impactMaterialsSeen.Length);
            Array.Clear(impactMaterialsHeavy,0,impactMaterialsHeavy.Length);
            for(int i=0;i<impactMaterialsDistance.Length;i++) impactMaterialsDistance[i]=float.PositiveInfinity;
            foreach(var impact in game.enemyImpactSounds) {
                if(impact.serial<=audioEnemyImpact || impact.serial>game.EnemyImpactSerial || impact.room!=game.room ||
                   game.elapsed-impact.time>.25f || authoredContacts.Contains(impact.soundActor)) continue;
                int index=(int)impact.material;
                float distance=Math.Abs(impact.x-game.player.x)+Math.Abs(impact.y-game.player.y);
                if(impactMaterialsSeen[index] && distance>=impactMaterialsDistance[index]) {
                    if(impact.tone==impactTones[index]) impactMaterialsHeavy[index]|=impact.heavy;
                    continue;
                }
                impactMaterialsSeen[index]=true;
                impactTones[index]=impact.tone;
                impactMaterialsHeavy[index]=impact.heavy;
                impactMaterialsDistance[index]=distance;
            }
            // Reserve at most two actual contacts per batch, each from a different family.
            for(int voice=0;voice<2;voice++) {
                int selected=-1;float nearest=float.PositiveInfinity;
                for(int i=0;i<impactMaterialsSeen.Length;i++) {
                    if(impactMaterialsSeen[i] && impactMaterialsDistance[i]<nearest) {nearest=impactMaterialsDistance[i];selected=i;}
                }
                if(selected<0) break;
                impactMaterialsSeen[selected]=false;
                if(PlayContact(impactTones[selected],impactMaterialsHeavy[selected])) {
                    AudioMaterialImpactPlays[selected]++;
                    AudioImpactPlays++;
                }
            }
            audioEnemyImpact=game.EnemyImpactSerial;
        }
    }
}
