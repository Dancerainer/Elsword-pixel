using System;
using System.Collections.Generic;
namespace ElswordPixel {
 // Immutable presentation lookup built only after the entry verifies its exact
 // compiled source-rules file. Traversal model clips do not rename source motion.
 public sealed class BelowSourceStateClipV21 {
  sealed class Profile {internal string kind;internal readonly Dictionary<string,string> clips=new Dictionary<string,string>(StringComparer.Ordinal);internal readonly Dictionary<CombatState,string> reactions=new Dictionary<CombatState,string>();}
  readonly Dictionary<string,Profile> profiles=new Dictionary<string,Profile>(StringComparer.Ordinal);
  public BelowSourceStateClipV21(NpcDefinition[] rules){
   if(rules==null)throw new ArgumentNullException("rules");
   foreach(var rule in rules){
    if(rule==null||string.IsNullOrEmpty(rule.id)||string.IsNullOrEmpty(rule.kind)||rule.states==null||profiles.ContainsKey(rule.id))throw new ArgumentException("Below exact source clip profile invalid/duplicated");
    var profile=new Profile{kind=rule.kind};
    foreach(var state in rule.states){if(state==null||string.IsNullOrEmpty(state.id)||string.IsNullOrEmpty(state.clip)||profile.clips.ContainsKey(state.id))throw new ArgumentException("Below exact source clip state invalid/duplicated");profile.clips.Add(state.id,state.clip);}
    foreach(var reaction in rule.reactions){if(!profile.clips.ContainsKey(reaction.sourceState)||profile.reactions.ContainsKey(reaction.combatState))throw new ArgumentException("Below declared source reaction state missing/duplicated");profile.reactions.Add(reaction.combatState,reaction.sourceState);}
    profiles.Add(rule.id,profile);
   }
  }
  public string SourceClip(Enemy enemy){
   if(enemy==null)throw new ArgumentNullException("enemy");
   // InterruptNpcAction clears npcState on real hit/launch/death. Preserve that
   // reaction clip; an empty source state does not authorize an idle substitute.
   if(string.IsNullOrEmpty(enemy.npcState))return enemy.clip;
   Profile profile;string clip;
   if(!profiles.TryGetValue(enemy.sourceNPC,out profile)||profile.kind!=enemy.kind||!profile.clips.TryGetValue(enemy.npcState,out clip))throw new InvalidOperationException("Below unknown exact identity/source state: "+enemy.sourceNPC+"/"+enemy.kind+"/"+enemy.npcState);
   return clip;
  }
  public bool TryDeclaredReactionClip(Enemy enemy,out string clip){
   clip=null;if(enemy==null||!string.IsNullOrEmpty(enemy.npcState))return false;Profile profile;string state;
   if(!profiles.TryGetValue(enemy.sourceNPC,out profile)||profile.kind!=enemy.kind)throw new InvalidOperationException("Below unknown physical reaction identity "+enemy.sourceNPC+"/"+enemy.kind);
   string generic=enemy.state==CombatState.Hurt?"hurt":enemy.state==CombatState.Knockdown?"knockdown":enemy.state==CombatState.GettingUp?"getup":null;
   if(enemy.clip!=generic||generic==null||!profile.reactions.TryGetValue(enemy.state,out state))return false;clip=profile.clips[state];return true;
  }
  public string Resolve(Enemy enemy,bool nonLoopOverrideLoaded){return BelowPixelStateClipV21.Resolve(enemy.sourceNPC,enemy.npcState,SourceClip(enemy),nonLoopOverrideLoaded);}
 }
}
