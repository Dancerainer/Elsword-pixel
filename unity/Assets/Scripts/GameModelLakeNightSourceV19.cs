using System;
using System.Collections.Generic;

// Scoped source predicates. Exact shared hooks are listed in the Night v19
// contract; absent documents leave the existing routes unchanged.
namespace ElswordPixel {
 [Serializable] public sealed class NpcLakeNightTimerV19 {
  public string npc,state,nextState,clock;public float threshold,rate;
 }
 [Serializable] public sealed class NpcLakeNightDashV19 {
  public string npc,state,nextState,sourceCallback,sourceInstructionsSha256,clock,sourceLineGroupQuery,projectionAdapter;
  public float minimumStateTime,nearLineGroupEndExclusive,sourceNativeNear,maximumStateTimeExclusive;
  public bool minimumStateTimeInclusive;public float[] sourceContinuousHitWindow;
 }
 [Serializable] public sealed class NpcLakeNightLineGroupV19 {
  public string roomId,sourceWorldId;public int lineSet;public int[] surfaceIds;
  public float startX,startY,endX,endY;public float[] nativeStart,nativeEnd;
 }
 [Serializable] public sealed class NpcLakeNightDocumentV19 {
  public string schema,sourceRulesSha256,teamAdapter;public bool runtimeAdmitted,nativeExecuted;
  public string[] npcUnitIds;public NpcLakeShamanHealV18[] shamanHeals;public NpcLakeShamanVectorV18[] shamanVectors;public NpcLakeTotemV18[] totems;public NpcLakeOlderSummonV18[] olderSummons;
  public NpcLakeNightTimerV19[] animationTimerTransitions;public NpcLakeNightDashV19[] dashExitConditions;public NpcLakeNightLineGroupV19[] lineGroups;
 }
 public sealed partial class GameModel {
  const string NightDashStateV19="LIZARDMAN_HIGH_DARK_NEW_ATTACK_C";
  NpcLakeNightDocumentV19 npcLakeNightV19;
  static bool EmptyNightV19<T>(T[] a){return a==null||a.Length==0;}
  static bool NearNightV19(float a,float b){return Finite(a)&&Math.Abs(a-b)<.00001f;}
  public void SetNpcLakeNightV19(NpcLakeNightDocumentV19 doc){
   if(doc!=null&&string.IsNullOrEmpty(doc.schema)&&string.IsNullOrEmpty(doc.sourceRulesSha256)&&string.IsNullOrEmpty(doc.teamAdapter)&&!doc.runtimeAdmitted&&!doc.nativeExecuted&&EmptyNightV19(doc.npcUnitIds)&&EmptyNightV19(doc.shamanHeals)&&EmptyNightV19(doc.shamanVectors)&&EmptyNightV19(doc.totems)&&EmptyNightV19(doc.olderSummons)&&EmptyNightV19(doc.animationTimerTransitions)&&EmptyNightV19(doc.dashExitConditions)&&EmptyNightV19(doc.lineGroups))doc=null;
   if(doc==null){if(npcLakeNightV19!=null&&npcLakeSourceV18!=null&&npcLakeSourceV18.sourceRulesSha256==npcLakeNightV19.sourceRulesSha256){npcLakeSourceV18=null;ClearNpcLakeSourceActorsV18();}npcLakeNightV19=null;return;}
   if(doc.schema!="besma-lake-night-source-callbacks-v19"||!SourceSupportHashV17(doc.sourceRulesSha256)||doc.runtimeAdmitted||doc.nativeExecuted||doc.teamAdapter!="all-current-enemies-hostile-team-v1"||doc.shamanHeals==null||doc.shamanHeals.Length!=1||doc.shamanVectors==null||doc.shamanVectors.Length!=1||doc.totems==null||doc.totems.Length!=1||!EmptyNightV19(doc.olderSummons)||doc.animationTimerTransitions==null||doc.animationTimerTransitions.Length!=1||doc.dashExitConditions==null||doc.dashExitConditions.Length!=1||doc.lineGroups==null||doc.lineGroups.Length!=1)throw new ArgumentException("Incomplete scoped Night source contract.");
   var expected=new HashSet<string>(new[]{"NUI_ARMADILLO","NUI_BESMA_TOTEM_BASE","NUI_FIRE_KOMODO","NUI_LIZARDMAN_HIGH_DARK_NEW","NUI_LIZARDMAN_SHAMAN","NUI_LIZARDMAN_SPEAR_DARK","NUI_SAURUS_HIGH"});
   if(doc.npcUnitIds==null||doc.npcUnitIds.Length!=expected.Count||!expected.SetEquals(doc.npcUnitIds))throw new ArgumentException("Night source IS_NPC membership changed.");
   foreach(var npc in expected)if(!npcDefinitions.ContainsKey(npc))throw new ArgumentException("Night source identity missing.");
   var heal=doc.shamanHeals[0];var vector=doc.shamanVectors[0];var totem=doc.totems[0];
   if(heal==null||vector==null||totem==null)throw new ArgumentException("Night must retain exact Shaman and Totem callbacks.");
   RequireLakeStateV18(heal.npc,heal.state,heal.sourceInstructionsSha256);RequireLakeStateV18(vector.npc,vector.state,vector.sourceInstructionsSha256);RequireLakeStateV18(totem.npc,totem.state,totem.sourceInstructionsSha256);
   if(heal.npc!="NUI_LIZARDMAN_SHAMAN"||heal.state!="LIZARDMAN_SHAMAN_ATTACK_B"||heal.sourceCallback!="LIZARDMAN_SHAMAN_SP_HEAL_FRAME_MOVE"||heal.sourceInstructionsSha256!="f14b3f297719045239fe0c213ea433d74d689028f76a77ae7ae2fd13efea57b1"||heal.clock!="animation-time"||heal.origin!="owner-current-position"||!NearNightV19(heal.at,.119f)||heal.range!=224||heal.healFraction!=.5f||heal.excludeSelf||!heal.requireAlive||!heal.requireSameTeam||!heal.excludeFullHp||!heal.rangeIsExclusive||heal.resetOwnerMp!=0||heal.sourceMpChargeRate!=130||heal.sourceMpChangeRate!=1||heal.entryCooldown!=5)throw new ArgumentException("Night Shaman source heal/resource contract changed.");
   if(vector.npc!=heal.npc||vector.state!="LIZARDMAN_SHAMAN_ATTACK_A"||vector.sourceCallback!="LIZARDMAN_SHAMAN_ATTACK_A_FRAME_MOVE"||vector.clock!="animation-time"||vector.captureOrigin!="owner-land-position"||vector.directionMethod!="GetDirVector"||vector.createMethod!="CreateInstance_LUA2"||vector.vectorSlot!=0||!NearNightV19(vector.captureAt,.1f)||!NearNightV19(vector.emitAt,.28f)||vector.movePosTrueDistance!=-200||vector.movePosFalseDistance!=200||vector.sourceOffsetY!=200||vector.effectUnitScale!=1||vector.sourceEffect!="DamageEffect_LIZARDMAN_SHAMAN_ATTACK_A"||!npcEffectDefinitions.ContainsKey(vector.sourceEffect))throw new ArgumentException("Night Shaman original vector emission changed.");
   if(totem.npc!="NUI_BESMA_TOTEM_BASE"||totem.state!="BESMA_TOTEM_BASE_YOGA_FIRE"||totem.sourceCallback!="BESMA_TOTEM_BASE_YOGA_FIRE_FRAME_MOVE"||totem.clock!="state-seconds"||totem.originBone!="Dummy1_Mouse"||!NearNightV19(totem.emitAt,3.6f)||!Finite(totem.originX)||!Finite(totem.originY)||!Finite(totem.originZ)||totem.movePosTrueDistance!=-250||totem.movePosFalseDistance!=250||totem.sourceOffsetY!=-10||totem.effectUnitScale!=1||totem.sourceWaitInterval!=4||totem.sourceFireInterval!=3||totem.sourceFireStateTimeOver!=6||!totem.fixedAnchor||!totem.inheritOriginalFacing||!totem.neverCountsForClear||totem.sourceEffect!="BESMA_TOTEM_BASE_FIRE_ATTACK"||!npcEffectDefinitions.ContainsKey(totem.sourceEffect)||npcDefinitions[totem.npc].environmentKind!="source-totem"||totem.sourceSoundName!="fire_ready.ogg"||!npcSourceSoundAssetsV17.Contains(totem.readySound??""))throw new ArgumentException("Night fixed source Totem contract changed.");
   var timer=doc.animationTimerTransitions[0];NpcStateDefinition ready;
   if(timer==null||timer.npc!="NUI_SAURUS_HIGH"||timer.state!="SAURUS_HIGH_ATTACK_A_READY"||timer.nextState!="SAURUS_HIGH_ATTACK_A"||timer.clock!="source-animation-seconds"||!NearNightV19(timer.threshold,1.9f)||timer.rate!=100||!npcStates.TryGetValue(timer.npc+"/"+timer.state,out ready)||!ready.holdForSourceEvent||!npcStates.ContainsKey(timer.npc+"/"+timer.nextState))throw new ArgumentException("Saurus High original1.9animation predicate changed.");
   var dash=doc.dashExitConditions[0];NpcStateDefinition looping;
   if(dash==null||dash.npc!="NUI_LIZARDMAN_HIGH_DARK_NEW"||dash.state!=NightDashStateV19||dash.nextState!="LIZARDMAN_HIGH_DARK_NEW_WAIT"||dash.sourceCallback!="CF_LIZARDMAN_HIGH_DARK_NEW_WAIT"||dash.sourceInstructionsSha256!="8d481400654be1e005c08dbb14d0c755a120f1de1864d36027017a0d8aed4683"||dash.clock!="state-seconds"||!NearNightV19(dash.minimumStateTime,.1f)||!dash.minimumStateTimeInclusive||dash.nearLineGroupEndExclusive!=98||dash.sourceNativeNear!=350||!NearNightV19(dash.maximumStateTimeExclusive,1.2f)||dash.sourceContinuousHitWindow==null||dash.sourceContinuousHitWindow.Length!=2||!NearNightV19(dash.sourceContinuousHitWindow[0],.01f)||dash.sourceContinuousHitWindow[1]!=100||!npcStates.TryGetValue(dash.npc+"/"+dash.state,out looping)||!looping.loop||!NearNightV19(looping.duration,.4f)||!looping.holdForSourceEvent||looping.hits.Length!=1)throw new ArgumentException("Dark High original loop/exit/hit clock changed.");
   var group=doc.lineGroups[0];
   if(group==null||group.roomId!="Dungeon_Besma_Lake_Night_Normal-2"||group.sourceWorldId!="WI_BESMA_LAKE_NIGHT_STAGE4"||group.lineSet!=0||group.surfaceIds==null||group.surfaceIds.Length!=3||group.surfaceIds[0]!=0||group.surfaceIds[1]!=1||group.surfaceIds[2]!=2||group.startX!=32||group.startY!=54||group.endX!=1068||group.endY!=54||group.nativeStart==null||group.nativeEnd==null||group.nativeStart.Length!=3||group.nativeEnd.Length!=3||group.nativeStart[0]!=-200||group.nativeStart[1]!=120||group.nativeStart[2]!=260||group.nativeEnd[0]!=3500||group.nativeEnd[1]!=120||group.nativeEnd[2]!=260)throw new ArgumentException("Night original source line-group endpoints changed.");
   // Reuse same-ID business logic and original visual ownership. This does not
   // call the Lake-only setter or smuggle absent Older actors into Night.
   npcLakeSourceV18=new NpcLakeSourceDocumentV18{schema=doc.schema,sourceRulesSha256=doc.sourceRulesSha256,teamAdapter=doc.teamAdapter,npcUnitIds=doc.npcUnitIds,shamanHeals=doc.shamanHeals,shamanVectors=doc.shamanVectors,totems=doc.totems,olderSummons=new NpcLakeOlderSummonV18[0]};
   npcLakeNightV19=doc;ClearNpcLakeSourceActorsV18();
  }
  bool NightLoopV19(NpcStateDefinition state){return npcLakeNightV19!=null&&state.id==NightDashStateV19;}
  float NpcLakeNightStepEndV19(NpcStateDefinition state,float from,float dt){return NightLoopV19(state)?from+dt:Math.Min(state.duration,from+dt);}
  static float NpcLakeNightMotionFrameV19(NpcStateDefinition state,float at){return ((state.id==NightDashStateV19||state.id=="NASOD_RAIL_DASH_ATTACK"||OriginalRushTimerV45.Active(state))&&state.loop?at%state.duration:at)*60;}
  float NpcLakeNightHitEndV19(NpcStateDefinition state,NpcHitWindow hit){return NightLoopV19(state)?npcLakeNightV19.dashExitConditions[0].sourceContinuousHitWindow[1]:hit.end;}
  public static bool SourceNightDashExitV19(float time,int facing,float x,float y,float startX,float startY,float endX,float endY){
   foreach(float v in new[]{time,x,y,startX,startY,endX,endY})if(!Finite(v))throw new ArgumentException("Invalid Night source condition input.");
   if(time<.1f)return false;float dx=x-(facing>0?endX:startX),dy=y-(facing>0?endY:startY);
   return dx*dx+dy*dy<98*98||time>1.2f;
  }
  bool AdvanceNpcLakeNightTransitionV19(Enemy owner,NpcStateDefinition state,float from,float to){
   if(npcLakeNightV19==null||owner==null||owner.hp<=0)return false;
   var timer=npcLakeNightV19.animationTimerTransitions[0];
   if(owner.sourceNPC==timer.npc&&state.id==timer.state){float at=timer.threshold/state.sourceAnimSpeed;if(from<at&&to+1e-6f>=at)return TryEnterNpcState(owner,npcStates[timer.npc+"/"+timer.nextState]);return false;}
   if(!NightLoopV19(state))return false;var group=npcLakeNightV19.lineGroups[0];
   if(CurrentRoom.id!=group.roomId||Array.IndexOf(group.surfaceIds,owner.groundLine)<0)throw new InvalidOperationException("Dark High source condition has no current original line-group binding.");
   if(!SourceNightDashExitV19(to,owner.facing,owner.x,owner.y,group.startX,group.startY,group.endX,group.endY))return false;
   return TryEnterNpcState(owner,npcStates[owner.sourceNPC+"/"+npcLakeNightV19.dashExitConditions[0].nextState]);
  }
 }
}
