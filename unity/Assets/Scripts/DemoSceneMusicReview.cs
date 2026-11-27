using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        bool musicReview;
        [Serializable] class MusicReviewScene {public string scene,world,expected,actual,clip;public bool playing,loop;public float position,volume,rms;}
        [Serializable] class MusicReviewResult {
            public bool passed;public int pauseChecks,muteChecks,resumeChecks,continuousSceneChecks,bossSwitchChecks,loopChecks,trackChanges;
            public string scope="Actual Windows Player streaming BGM for currently playable Ruben scenes. Elder source tracks are inventoried separately; this is not Elder scene implementation evidence.";
            public MusicReviewScene[] scenes;public string mixedAudio="scene-music-mix.wav";
        }
        float MusicSignalRms(){
            var source=CurrentMusicSource;if(!source||!source.isPlaying)return 0;var buffer=new float[1024];source.GetOutputData(buffer,0);
            double power=0;foreach(var sample in buffer)power+=sample*sample;return (float)Math.Sqrt(power/buffer.Length);
        }
        MusicReviewScene MusicSceneSnapshot(){
            var stage=catalog.stages[StageIndex];var source=CurrentMusicSource;
            return new MusicReviewScene{scene=stage.id,world=stage.sourceWorldId,expected=stage.bgm,actual=CurrentMusicId,clip=source&&source.clip?source.clip.name:"",
                playing=source&&source.isPlaying,loop=source&&source.loop,position=source?source.time:0,volume=source?source.volume:0,rms=MusicSignalRms()};
        }
        void PrepareMusicReviewRoom(int room){
            typeof(GameModel).GetMethod("EnterRoom",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(game,new object[]{room});
            foreach(var enemy in game.enemies){enemy.active=false;enemy.aiEnabled=false;enemy.passive=true;}
            game.player.invincible=9999;game.paused=false;
        }
        IEnumerator ReviewSceneMusic(){
            musicReview=true;Application.runInBackground=true;bool wasMuted=AudioMuted;
            var samples=new List<MusicReviewScene>();var result=new MusicReviewResult{passed=true};
            var probe=worldCamera.gameObject.AddComponent<CombatAudioProbe>();probe.Begin();
            try{
                if(AudioMuted)ToggleAudio();
                // Existing playable region only. New Elder routes must be tested
                // after their scene and reviewed monster resources are actually added.
                for(int dungeon=0;dungeon<3;dungeon++){
                    SelectRuntimeDungeonOnMap(dungeon);EnterSelectedDungeon();PrepareMusicReviewRoom(0);
                    yield return new WaitForSecondsRealtime(1);
                    float wait=0;while(MusicSignalRms()<.000001f&&wait<3){yield return new WaitForSecondsRealtime(.1f);wait+=.1f;}
                    var first=MusicSceneSnapshot();samples.Add(first);
                    result.passed&=SceneMusicReady&&first.actual==first.expected&&first.playing&&first.loop&&first.rms>.000001f;
                    var source=CurrentMusicSource;
                    game.paused=true;yield return new WaitForSecondsRealtime(.1f);int pausedAt=source.timeSamples;
                    yield return new WaitForSecondsRealtime(.25f);
                    bool paused=Math.Abs(source.timeSamples-pausedAt)<=source.clip.frequency/50;result.passed&=paused;if(paused)result.pauseChecks++;
                    ToggleAudio();yield return null;bool muted=source.volume==0;result.passed&=muted;if(muted)result.muteChecks++;
                    game.paused=false;yield return new WaitForSecondsRealtime(.25f);ToggleAudio();yield return new WaitForSecondsRealtime(.2f);
                    bool resumed=source.isPlaying&&source.volume>.01f&&source.timeSamples>pausedAt;result.passed&=resumed;if(resumed)result.resumeChecks++;
                    int trackChanges=MusicTrackChanges;int priorSample=source.timeSamples;PrepareMusicReviewRoom(1);
                    yield return new WaitForSecondsRealtime(.35f);var second=MusicSceneSnapshot();samples.Add(second);
                    bool seamless=CurrentMusicSource==source&&second.actual==second.expected&&MusicTrackChanges==trackChanges&&source.timeSamples>priorSample;
                    result.passed&=seamless;if(seamless)result.continuousSceneChecks++;
                    PrepareMusicReviewRoom(2);yield return new WaitForSecondsRealtime(1);
                    wait=0;while(MusicSignalRms()<.000001f&&wait<3){yield return new WaitForSecondsRealtime(.1f);wait+=.1f;}
                    var boss=MusicSceneSnapshot();samples.Add(boss);
                    bool switched=CurrentMusicSource!=source&&boss.actual==boss.expected&&boss.actual!=second.actual&&boss.playing&&boss.rms>.000001f&&!source.isPlaying&&source.volume==0;
                    result.passed&=switched;if(switched)result.bossSwitchChecks++;
                    source=CurrentMusicSource;source.timeSamples=Math.Max(0,source.clip.samples-source.clip.frequency/4);
                    yield return new WaitForSecondsRealtime(.8f);
                    bool wrapped=source.isPlaying&&source.timeSamples<source.clip.frequency*3;result.passed&=wrapped;if(wrapped)result.loopChecks++;
                }
                result.scenes=samples.ToArray();result.trackChanges=MusicTrackChanges;
                Directory.CreateDirectory(captureDir);probe.Save(Path.Combine(captureDir,result.mixedAudio));
                File.WriteAllText(Path.Combine(captureDir,"scene-music-review.json"),JsonUtility.ToJson(result,true));
                Debug.Log("SCENE_MUSIC_REVIEW_COMPLETE passed="+result.passed+" scenes="+samples.Count);
            }finally{if(AudioMuted!=wasMuted)ToggleAudio();if(probe)Destroy(probe);}
            yield return new WaitForSecondsRealtime(.5f);Application.Quit();
        }
    }
}
