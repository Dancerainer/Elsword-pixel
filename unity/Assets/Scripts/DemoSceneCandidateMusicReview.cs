using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class CandidateMusicCheck {public string world,bgm,sha256;public bool available,loaded,playing,loop,passed;public float position,sourceRms,listenerRms,mutedRms;}
  [Serializable] sealed class CandidateMusicReview {public bool passed;public int availableTracks,missingReferences;public string scope="Actual Player map preview loading and AudioListener output for source BGM; scene transitions and unfinished dungeon admission are not asserted.";public CandidateMusicCheck[] tracks;}
  float CandidateMusicRms(bool listener){var data=new float[1024];if(listener)AudioListener.GetOutputData(data,0);else if(mapMusicPreview)mapMusicPreview.GetOutputData(data,0);double energy=0;foreach(float value in data)energy+=value*value;return (float)Math.Sqrt(energy/data.Length);}
  IEnumerator ReviewCandidateMapMusic(){
   var result=new CandidateMusicReview{passed=true};var checks=new List<CandidateMusicCheck>();var seen=new HashSet<string>();bool wasMuted=audioMuted;
   try{
    audioMuted=false;StopAudioVoices();SetMenu("dungeons");
    var rooms=new List<CandidateRoomArt>();foreach(var dungeon in mapDungeons)foreach(var room in dungeon.rooms)if(seen.Add(room.bgmAvailable?room.bgmSha256:"missing:"+room.id))rooms.Add(room);
    // Also exercise the Resources path used by the ordinary playable map.
    rooms.Add(new CandidateRoomArt{id="admitted-resource-control",bgm=catalog.stages[0].bgm,bgmAvailable=true});
    foreach(var room in rooms){
     var check=new CandidateMusicCheck{world=room.id,bgm=room.bgm,sha256=room.bgmSha256,available=room.bgmAvailable};checks.Add(check);StartMapMusicPreview(room);
     if(!room.bgmAvailable){yield return new WaitForSecondsRealtime(.15f);check.passed=!mapMusicPreview||!mapMusicPreview.isPlaying;result.missingReferences++;result.passed&=check.passed;continue;}
     float wait=0;while((!mapMusicPreview||!mapMusicPreview.clip||!mapMusicPreview.isPlaying)&&wait<8){yield return new WaitForSecondsRealtime(.05f);wait+=.05f;}
     check.loaded=mapMusicPreview&&mapMusicPreview.clip;check.playing=mapMusicPreview&&mapMusicPreview.isPlaying;check.loop=mapMusicPreview&&mapMusicPreview.loop;
     wait=0;while(check.loaded&&wait<6){yield return new WaitForSecondsRealtime(.05f);wait+=.05f;check.sourceRms=Math.Max(check.sourceRms,CandidateMusicRms(false));check.listenerRms=Math.Max(check.listenerRms,CandidateMusicRms(true));if(check.sourceRms>1e-6f&&check.listenerRms>1e-6f)break;}
     check.position=mapMusicPreview?mapMusicPreview.time:0;audioMuted=true;yield return new WaitForSecondsRealtime(.15f);check.mutedRms=CandidateMusicRms(true);audioMuted=false;
     check.passed=check.loaded&&check.playing&&check.loop&&check.position>0&&check.sourceRms>1e-6f&&check.listenerRms>1e-6f&&check.mutedRms<1e-6f;
     result.availableTracks++;result.passed&=check.passed;Debug.Log("CANDIDATE_MUSIC_CHECK "+room.id+" "+room.bgm+" passed="+check.passed);
    }
    result.passed&=result.availableTracks==20&&result.missingReferences==1;result.tracks=checks.ToArray();Directory.CreateDirectory(captureDir);File.WriteAllText(Path.Combine(captureDir,"candidate-music-review.json"),JsonUtility.ToJson(result,true));Debug.Log("CANDIDATE_MUSIC_REVIEW passed="+result.passed+" tracks="+result.availableTracks);
   }finally{audioMuted=wasMuted;StopMapMusicPreview();}
  }
 }
}
