using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class BelowRecoveryFallingRowV21 {public string npc,kind,physicalState,modelClip,lookupClip,displayClip;public bool sourceStateMustBeEmpty,nativeEquivalent;public float maximumVy;}
 [Serializable] public sealed class BelowRecoveryGroundedRowV21 {public string npc,kind,physicalState,modelClip,sourceReactionState,sourceAnimation,displayClip;public bool sourceStateMustBeEmpty,requiresPreviouslyVerifiedNonemptySourceState,requiresNativeGrounded,requiresPositiveResidualStun,requiresNoPendingStateRequest,requiresNoKnockdownOnLanding,nativeEquivalent;public float requiredVy;}
 [Serializable] public sealed class BelowRecoveryLookupDocumentV21 {public string schema,producer,sourceRulesSha256;public bool sourceNativeEquivalent,runtimeAdmitted;public BelowRecoveryFallingRowV21[] descendingJumpRows;public BelowRecoveryGroundedRowV21[] groundedMiniTransitionRows;}
 // Local rendering lookup only. Neither the Enemy nor its source clock/queue is
 // rewritten. The old36 policy and source-state selector remain authoritative.
 public sealed class BelowRecoveryLookupV21 {
  readonly Dictionary<string,BelowRecoveryFallingRowV21> falling=new Dictionary<string,BelowRecoveryFallingRowV21>(StringComparer.Ordinal);
  readonly Dictionary<string,BelowRecoveryGroundedRowV21> grounded=new Dictionary<string,BelowRecoveryGroundedRowV21>(StringComparer.Ordinal);
  readonly HashSet<Enemy> initialized=new HashSet<Enemy>();
  static string Key(string npc,string kind){return npc+"/"+kind;}
  public BelowRecoveryLookupV21(BelowRecoveryLookupDocumentV21 d,NpcDefinition[] rules,string rulesSha){
   if(d==null||rules==null||d.schema!="below-bounded-recovery-local-lookup-policy-v21"||d.sourceRulesSha256!=rulesSha||d.sourceNativeEquivalent||d.runtimeAdmitted||d.descendingJumpRows==null||d.descendingJumpRows.Length!=8||d.groundedMiniTransitionRows==null||d.groundedMiniTransitionRows.Length!=2)throw new ArgumentException("Below recovery lookup exact policy absent");
   var expected=new HashSet<string>(new[]{"NUI_ARMADILLO_EARTH","NUI_BAT_SONIC","NUI_CANNON_MINI","NUI_CANNON_MINI_UP","NUI_NASOD_MINER_FAT_C_DEFECT","NUI_NASOD_MINER_THIN_DEFECT","NUI_PARASITE_HOUSE","NUI_PARASITE"},StringComparer.Ordinal);
   foreach(var r in d.descendingJumpRows){var p=r==null?null:Array.Find(rules,x=>x.id==r.npc&&x.kind==r.kind);if(p==null||!expected.Remove(r.npc)||!r.sourceStateMustBeEmpty||r.physicalState!="Airborne"||r.modelClip!="jump"||r.maximumVy!=0||r.lookupClip!="fall"||r.nativeEquivalent)throw new ArgumentException("Below recovery unknown falling tuple");falling.Add(Key(r.npc,r.kind),r);}
   foreach(var r in d.groundedMiniTransitionRows){var p=r==null?null:Array.Find(rules,x=>x.id==r.npc&&x.kind==r.kind);var declared=p==null?null:Array.Find(p.reactions,x=>x.combatState==CombatState.Hurt);var state=declared==null?null:Array.Find(p.states,x=>x.id==declared.sourceState);
    if(p==null||(p.id!="NUI_CANNON_MINI"&&p.id!="NUI_CANNON_MINI_UP")||!r.sourceStateMustBeEmpty||r.physicalState!="Normal"||r.modelClip!="hurt"||!r.requiresPreviouslyVerifiedNonemptySourceState||!r.requiresNativeGrounded||r.requiredVy!=0||!r.requiresPositiveResidualStun||!r.requiresNoPendingStateRequest||!r.requiresNoKnockdownOnLanding||r.nativeEquivalent||state==null||state.id!=r.sourceReactionState||state.sourceAnimation!=r.sourceAnimation||state.sourceAnimation!="Wait"||state.clip!=r.displayClip||state.clip!="idle")throw new ArgumentException("Below recovery unknown grounded transition");grounded.Add(Key(r.npc,r.kind),r);
   }
  }
  public void Clear(){initialized.Clear();}
  public bool TryResolveLocal(bool belowSession,string roomId,Enemy enemy,BelowSourceStateClipV21 source,BelowPhysicalReactionClipV21 physical,bool nonLoopOverrideLoaded,Func<string,bool> hasClip,bool nativeGrounded,string pendingState,out string clip,out bool hold){
   clip=null;hold=false;if(!belowSession||roomId!="Dungeon_Altera_Below_Tunnel_Normal-0"&&roomId!="Dungeon_Altera_Below_Tunnel_Normal-1"&&roomId!="Dungeon_Altera_Below_Tunnel_Normal-2")return false;
   if(enemy==null||source==null||physical==null||hasClip==null)throw new ArgumentNullException("Below recovery presentation inputs");
   // Verify first, then record initialization. An unknown/nonloop-invalid source
   // state cannot authorize the later transitional display rule.
   if(!string.IsNullOrEmpty(enemy.npcState)){source.Resolve(enemy,nonLoopOverrideLoaded);initialized.Add(enemy);return false;}
   BelowRecoveryFallingRowV21 fall;if(enemy.state==CombatState.Airborne&&enemy.clip=="jump"&&enemy.vy<=0&&falling.TryGetValue(Key(enemy.sourceNPC,enemy.kind),out fall)){
    clip=physical.ResolveFallingLookupV21(enemy,source,hasClip,out hold);if(clip!=fall.displayClip)throw new InvalidOperationException("Below recovery changed approved fall resolution");return true;
   }
   BelowRecoveryGroundedRowV21 rest;if(enemy.state==CombatState.Normal&&enemy.clip=="hurt"&&nativeGrounded&&enemy.vy==0&&enemy.stun>0&&!enemy.knockdownOnLanding&&string.IsNullOrEmpty(pendingState)&&initialized.Contains(enemy)&&grounded.TryGetValue(Key(enemy.sourceNPC,enemy.kind),out rest)){
    if(!hasClip(rest.displayClip))throw new InvalidOperationException("Below recovery declared Wait display clip absent");clip=rest.displayClip;hold=true;return true;
   }
   return false;
  }
 }
}
