using System;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  string BelowTunnelEntryRenderClipV21(Enemy enemy,ref float renderClock){
   if(belowPlayerSessionV21)return BelowPlayerRenderClipV21(enemy,ref renderClock);
#if UNITY_EDITOR
   if(belowTunnelEntrySessionV21&&!BelowEntryNoBodyIdV21(enemy.sourceNPC)){
    PreviewRequire(belowEntrySourceClipsV21!=null,"Below verified source-state clip cache absent");Library art;PreviewRequire(libraries.TryGetValue(enemy.kind,out art),"Below exact identity library absent "+enemy.sourceNPC);
    string selected=BelowPixelStateClipV21.OverrideClip(enemy.sourceNPC,enemy.npcState);bool loaded=selected==null||art.frames.ContainsKey(selected)&&art.clips.ContainsKey(selected)&&!art.clips[selected].loop;
    PreviewRequire(belowEntryPhysicalReactionsV21!=null&&belowEntryRecoveryLookupV21!=null,"Below exact physical/recovery presentation policy absent");bool hold;string clip;
    if(!belowEntryRecoveryLookupV21.TryResolveLocal(belowTunnelEntrySessionV21,game.CurrentRoom.id,enemy,belowEntrySourceClipsV21,belowEntryPhysicalReactionsV21,loaded,key=>art.frames.ContainsKey(key)&&art.clips.ContainsKey(key),game.Grounded(enemy),game.NpcPendingStateRequest(enemy),out clip,out hold))clip=belowEntryPhysicalReactionsV21.Resolve(enemy,belowEntrySourceClipsV21,loaded,key=>art.frames.ContainsKey(key)&&art.clips.ContainsKey(key),out hold);
    PreviewRequire(art.frames.ContainsKey(clip)&&art.clips.ContainsKey(clip),"Below runtime exact state/reaction clip absent; idle fallback forbidden "+enemy.sourceNPC+"/"+enemy.npcState+"/"+clip);if(hold){var info=art.clips[clip];renderClock=BelowPhysicalReactionClipV21.OnceThenHoldClock(renderClock,info.duration,info.fps,art.frames[clip].Length);}return clip;
   }
#endif
   return enemy.clip;
  }
  AudioClip BelowTunnelEntrySceneMusicV21(string id,string resource){
   AudioClip factoryClip;if(TryFactoryProductSceneMusicR1179(id,out factoryClip))return factoryClip;
   AudioClip ownedClip;if(belowPlayerSessionV21&&belowPlayerMusicV21.TryGetValue(id,out ownedClip))return ownedClip;
#if UNITY_EDITOR
   AudioClip clip;if(TryFactoryPrivateSceneMusicR742(id,out clip))return clip;if(belowTunnelEntrySessionV21&&belowEntryMusicClipsV21.TryGetValue(id,out clip))return clip;
#endif
   return Resources.Load<AudioClip>(resource);
  }
 }
}
