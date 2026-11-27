using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class SceneMusicManifest {public SceneMusicAsset[] assets=new SceneMusicAsset[0];}
        [Serializable] sealed class SceneMusicAsset {public string id,resource;public float gain=.22f;}
        readonly Dictionary<string,SceneMusicAsset> sceneMusicAssets=new Dictionary<string,SceneMusicAsset>();
        readonly AudioSource[] musicVoices=new AudioSource[2];
        readonly float[] musicLevels=new float[2],musicFrom=new float[2];
        bool musicInitialized,musicPaused;int musicActive=-1;float musicFade=1;
        string musicId="";const float MusicFadeSeconds=.65f;
        const string LobbyTrack="lobby";
        AudioClip lobbyTrackClip,menuButtonClip,resultCountClip;
        AudioSource interfaceAudio;
        public string CurrentMusicId {get{return musicId;}}
        public bool SceneMusicReady {get;private set;}
        public int MusicTrackChanges {get;private set;}
        public AudioSource CurrentMusicSource {get{return musicActive>=0?musicVoices[musicActive]:null;}}
        AudioSource mapMusicPreview;bool mapMusicOwnsClip;int mapMusicGeneration;
        void StopMapMusicPreview(){
            mapMusicGeneration++;if(!mapMusicPreview)return;mapMusicPreview.Stop();if(mapMusicOwnsClip&&mapMusicPreview.clip)Destroy(mapMusicPreview.clip);mapMusicPreview.clip=null;mapMusicOwnsClip=false;
        }
        void StartMapMusicPreview(CandidateRoomArt room){
            StopMapMusicPreview();if(!room.bgmAvailable)return;
            if(!mapMusicPreview){var host=new GameObject("Map / original BGM preview");host.transform.SetParent(transform,false);mapMusicPreview=host.AddComponent<AudioSource>();mapMusicPreview.playOnAwake=false;mapMusicPreview.loop=true;mapMusicPreview.spatialBlend=0;}
            mapMusicPreview.volume=audioMuted?0:.22f*musicVolume;
            if(string.IsNullOrEmpty(room.bgmPath)){
                if(!musicInitialized)InitSceneMusic();SceneMusicAsset binding;
                if(sceneMusicAssets.TryGetValue(room.bgm,out binding)){mapMusicPreview.clip=Resources.Load<AudioClip>(binding.resource);if(mapMusicPreview.clip)mapMusicPreview.Play();}
            }else StartCoroutine(LoadMapMusicPreview(room,mapMusicGeneration));
        }
        IEnumerator LoadMapMusicPreview(CandidateRoomArt room,int generation){
            string uri;
            try{
                var info=new FileInfo(room.bgmPath);if(!info.Exists||info.Length>64*1024*1024)throw new InvalidDataException("Candidate BGM missing or oversized");
                string digest;using(var hash=SHA256.Create())using(var input=File.OpenRead(info.FullName))digest=BitConverter.ToString(hash.ComputeHash(input)).Replace("-","").ToLowerInvariant();
                if(digest!=room.bgmSha256)throw new InvalidDataException("Candidate BGM no longer matches original source bytes");
                uri=new Uri(info.FullName).AbsoluteUri;
            }catch(Exception ex){Debug.LogWarning("Candidate music rejected: "+ex.Message);yield break;}
            using(var request=UnityWebRequestMultimedia.GetAudioClip(uri,AudioType.OGGVORBIS)){
                yield return request.SendWebRequest();
                if(generation!=mapMusicGeneration||openMenu!="dungeons")yield break;
                if(request.result!=UnityWebRequest.Result.Success){Debug.LogWarning("Candidate music unavailable: "+request.error);yield break;}
                var clip=DownloadHandlerAudioClip.GetContent(request);if(!clip)yield break;
                mapMusicOwnsClip=true;mapMusicPreview.clip=clip;mapMusicPreview.Play();Debug.Log("CANDIDATE_BGM_PREVIEW "+room.id+" -> "+room.bgm);
            }
        }

        void InitSceneMusic(){
            if(musicInitialized)return;musicInitialized=true;
            var text=Resources.Load<TextAsset>("Pixel/music/manifest");
            if(!text){Debug.LogWarning("Scene BGM manifest missing; extract and stage original SetBGM assets.");return;}
            var manifest=JsonUtility.FromJson<SceneMusicManifest>(text.text);
            foreach(var entry in manifest.assets)sceneMusicAssets[entry.id]=entry;
            RegisterRegionalMusic();
            var group=new GameObject("Original scene BGM / streaming crossfade");group.transform.SetParent(transform,false);
            for(int i=0;i<2;i++){var voice=group.AddComponent<AudioSource>();voice.playOnAwake=false;voice.loop=true;voice.spatialBlend=0;voice.priority=100;voice.volume=0;musicVoices[i]=voice;}
            SceneMusicReady=sceneMusicAssets.Count>0;
            StartCoroutine(LoadInterfaceAudio(LobbyTrack,true,clip=>{
                lobbyTrackClip=clip;
                sceneMusicAssets[LobbyTrack]=new SceneMusicAsset{id=LobbyTrack,gain=.22f};
            }));
            StartCoroutine(LoadInterfaceAudio("click",false,clip=>menuButtonClip=clip));
            StartCoroutine(LoadInterfaceAudio("count",false,clip=>resultCountClip=clip));
        }

        IEnumerator LoadInterfaceAudio(string name,bool compressed,Action<AudioClip> install){
#if UNITY_ANDROID && !UNITY_EDITOR
            string uri="jar:"+new Uri(Application.dataPath).AbsoluteUri+"!/assets/lobby-audio/"+name+".ogg";
#else
            string path=Path.Combine(Application.streamingAssetsPath,"lobby-audio",name+".ogg");
            string uri=new Uri(Path.GetFullPath(path)).AbsoluteUri;
#endif
            using(var request=UnityWebRequestMultimedia.GetAudioClip(uri,AudioType.OGGVORBIS)){
#if !UNITY_ANDROID || UNITY_EDITOR
                ((DownloadHandlerAudioClip)request.downloadHandler).compressed=compressed;
#endif
                yield return request.SendWebRequest();
                if(request.result!=UnityWebRequest.Result.Success){
                    Debug.LogWarning("Unable to load interface audio "+name+": "+request.error);
                    yield break;
                }
                var clip=DownloadHandlerAudioClip.GetContent(request);
                if(!clip)yield break;
                clip.name=name;
                ownedContactClips.Add(clip);
                install(clip);
            }
        }

        void PlayMenuButtonSound(){PlayInterfaceSound(menuButtonClip,.46f);}
        void PlayResultCountSound(){PlayInterfaceSound(resultCountClip,.34f);}
        void PlayInterfaceSound(AudioClip clip,float gain){
            if(!clip||audioMuted||effectsVolume<=0)return;
            if(!interfaceAudio){
                var host=new GameObject("Interface audio");
                host.transform.SetParent(transform,false);
                interfaceAudio=host.AddComponent<AudioSource>();
                interfaceAudio.playOnAwake=false;
#if !UNITY_ANDROID || UNITY_EDITOR
                interfaceAudio.ignoreListenerPause=true;
#endif
                interfaceAudio.spatialBlend=0;
                interfaceAudio.priority=30;
            }
            interfaceAudio.PlayOneShot(clip,gain*effectsVolume);
            AudioPlayCount++;
        }
        void TickSceneMusic(){
            if(mapMusicPreview){if(openMenu!="dungeons"&&mapMusicPreview.clip)StopMapMusicPreview();else mapMusicPreview.volume=audioMuted?0:.22f*musicVolume;}
            if(!musicInitialized)InitSceneMusic();if(!SceneMusicReady)return;
            bool directory=openMenu=="dungeons"||menuOrigin=="dungeons"&&!string.IsNullOrEmpty(openMenu);
            bool lobby=directory||!adventureStarted;
            string desired=lobby?(lobbyTrackClip?LobbyTrack:""):
                catalog!=null&&StageIndex>=0&&StageIndex<catalog.stages.Length?CurrentStageArt.bgm:"";
            desired=desired??"";
            bool pauseMusic=game.paused&&!lobby;
            if(desired!=musicId){
                for(int i=0;i<2;i++)musicFrom[i]=musicLevels[i];musicFade=0;
                if(string.IsNullOrEmpty(desired)){musicId="";musicActive=-1;}
                else{
                    SceneMusicAsset binding;
                    if(!sceneMusicAssets.TryGetValue(desired,out binding))throw new InvalidOperationException("Unknown original scene BGM "+desired);
                    int next=musicActive==0?1:0;var voice=musicVoices[next];voice.Stop();
                    voice.clip=desired==LobbyTrack?lobbyTrackClip:BelowTunnelEntrySceneMusicV21(desired,binding.resource);
                    if(!voice.clip)throw new InvalidOperationException("Missing scene BGM "+binding.resource);
                    voice.Play();musicActive=next;musicId=desired;musicLevels[next]=musicFrom[next]=0;
                    // Switching while paused must not advance the newly started stream.
                    if(pauseMusic)voice.Pause();MusicTrackChanges++;
                    Debug.Log("SCENE_BGM "+(lobby?"lobby":CurrentStageArt.id)+" -> "+desired);
                }
            }
            if(musicPaused!=pauseMusic){musicPaused=pauseMusic;foreach(var voice in musicVoices){if(musicPaused)voice.Pause();else voice.UnPause();}}
            if(!musicPaused)musicFade=Mathf.Min(1,musicFade+Time.unscaledDeltaTime/MusicFadeSeconds);
            bool previewPlaying=lobby&&mapMusicPreview&&mapMusicPreview.isPlaying;
            for(int i=0;i<2;i++){
                float target=i==musicActive?sceneMusicAssets[musicId].gain:0;
                musicLevels[i]=Mathf.Lerp(musicFrom[i],target,musicFade);
                musicVoices[i].volume=audioMuted||previewPlaying?0:musicLevels[i]*musicVolume;
                if(i!=musicActive&&musicFade>=1&&musicVoices[i].isPlaying)musicVoices[i].Stop();
            }
        }
    }
}
