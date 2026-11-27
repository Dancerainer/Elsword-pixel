using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 internal static class ChickenSourceReactionV45 {
  [Serializable] sealed class Track {public string name;public int count,fps;public float duration;public float[] data;}
  [Serializable] sealed class Bank {public string npc,sourceSha,artSha;public Track[] tracks;}
  static Dictionary<string,Track> tracks;
  internal static bool Select(Enemy e,ref ChickenAuthoredPartsV31.Frame f){
   if(!OriginalReactionV45.TryPhysicalOrCommittedV45(e,out var p,out float seconds))return false;
   if(tracks==null){var json=Resources.Load<TextAsset>("Pixel/feedback-v45/chicken-source/tracks");
    if(!json)throw new InvalidOperationException("Missing A1 reaction tracks");
    var d=JsonUtility.FromJson<Bank>(json.text);if(d.npc!=e.sourceNPC||d.sourceSha.Length!=64||d.artSha.Length!=64)throw new InvalidOperationException("Unregistered A1 reaction bank");
    var index=new Dictionary<string,Track>(StringComparer.Ordinal);
    foreach(var t in d.tracks){if(t.count<1||t.fps!=24||t.data.Length!=20*t.count)throw new InvalidOperationException("Invalid A1 reaction track");index.Add(t.name,t);}tracks=index;
   }
   if(!tracks.TryGetValue(p.motion,out var track))return false;
   f.originalReaction=p.motion;
   // Existing hitstun, recovery and death own the combat clock. For the two
   // canonical phases whose carrier already maps its remaining interval, keep that mapping.
   f.originalReactionClock=string.IsNullOrEmpty(e.npcState)&&(f.state==ChickenAuthoredPoseV31.State.GetUp||f.state==ChickenAuthoredPoseV31.State.Hurt)?
    Mathf.Clamp01(f.normalizedPhase)*track.duration:seconds;
   return true;
  }
  internal static bool Sample(ChickenAuthoredPartsV31.Frame f,ChickenAuthoredPoseV31.PartPose[] parts,out ChickenAuthoredPoseV31.RigPose rig){
   rig=default;if(string.IsNullOrEmpty(f.originalReaction)||tracks==null||!tracks.TryGetValue(f.originalReaction,out var t))return false;
   int i=Mathf.Clamp(Mathf.FloorToInt(f.originalReactionClock*t.fps+1e-5f),0,t.count-1)*20;
   rig=new ChickenAuthoredPoseV31.RigPose{pivotX=576,pivotY=845,rotation=t.data[i],y=t.data[i+1],scaleX=1,scaleY=1};
   for(int j=0;j<parts.Length;j++){int k=i+2+j*3;parts[j]=new ChickenAuthoredPoseV31.PartPose{x=t.data[k],y=t.data[k+1],rotation=t.data[k+2],scaleX=1,scaleY=1};}
   return true;
  }
 }
}
