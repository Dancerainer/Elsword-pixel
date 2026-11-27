using System;
using System.Collections.Generic;

// Scoped display-command draft. No shared hook is added here, and no particle
// template, damage, movement, healing recipient or native allocation is invented.
namespace ElswordPixel {
 [Serializable] public sealed class NpcLakeVisualCallV18 {
  public int index,createCallPc;public string npc,state,callback,sourceInstructionsSha256,clock,manager,template,origin,bone;
  public float threshold;public bool storedMinorSlot0,ownerSetAddRotate,ownerSetAxisAngle;
  public int[] argumentSentinels=new int[0];
 }
 [Serializable] public sealed class NpcLakeVisualStateV18 {
  public string npc,state;public float duration,sourceAnimSpeed;public NpcAttachmentTrack[] tracks=new NpcAttachmentTrack[0];
 }
 [Serializable] public sealed class NpcLakeVisualDocumentV18 {
  public string schema,sourceRulesSha256,sourceEffectsSha256,sourceCallTableSha256,sourcePassiveBoneSha256;
  public bool runtimeAdmitted,nativeExecuted,staticPassiveBoneAtRoot;public NpcLakeVisualCallV18[] rows=new NpcLakeVisualCallV18[0];public NpcLakeVisualStateV18[] states=new NpcLakeVisualStateV18[0];
 }
 public sealed class NpcLakeVisualEventV18 {
  public int sequence,handle,callIndex,facing,entry;public string type,manager,template,npc,state,bone;
  public float time,x,y,z,depth,unit;public bool ownerSetAddRotate,ownerSetAxisAngle;
  public Enemy owner,target;
 }
 public sealed class NpcLakePassivePoseV18 {
  public int serial,facing;public string effect,particle,positionType,bone;
  public float x,y,z,age,lifetime,unit,depth;public bool sourceHasApplyRotate,sourceApplyRotate,sourceHasForceDieHit,sourceForceDieHit;
 }
 public sealed partial class GameModel {
  sealed class LakeVisualCursorV18 {
   internal string state;internal float previous;internal int entry,emittedMask,heldMinor0;
   internal readonly HashSet<Enemy> healedTargets=new HashSet<Enemy>();
  }
  NpcLakeVisualDocumentV18 npcLakeVisualV18;
  readonly Dictionary<Enemy,LakeVisualCursorV18> lakeVisualCursorsV18=new Dictionary<Enemy,LakeVisualCursorV18>();
  public readonly List<NpcLakeVisualEventV18> npcLakeVisualEventsV18=new List<NpcLakeVisualEventV18>();
  public int NpcLakeVisualGenerationV18 {get;private set;}
  public bool NpcLakeMinorVisualManagerAvailableV18=true,NpcLakeMajorVisualManagerAvailableV18=true;
  int lakeVisualSequenceV18,lakeVisualHandleV18,lakeVisualEntryV18;
  static bool LakeVisualEmptyV18<T>(T[] a){return a==null||a.Length==0;}
  public void SetNpcLakeVisualV18(NpcLakeVisualDocumentV18 doc){
   if(doc!=null&&string.IsNullOrEmpty(doc.schema)&&string.IsNullOrEmpty(doc.sourceRulesSha256)&&string.IsNullOrEmpty(doc.sourceEffectsSha256)&&string.IsNullOrEmpty(doc.sourceCallTableSha256)&&string.IsNullOrEmpty(doc.sourcePassiveBoneSha256)&&!doc.staticPassiveBoneAtRoot&&!doc.runtimeAdmitted&&!doc.nativeExecuted&&LakeVisualEmptyV18(doc.rows)&&LakeVisualEmptyV18(doc.states))doc=null;
   if(doc!=null){
    if(doc.schema!="besma-lake-visual-runtime-draft-v18"||!SourceSupportHashV17(doc.sourceRulesSha256)||!SourceSupportHashV17(doc.sourceEffectsSha256)||!SourceSupportHashV17(doc.sourceCallTableSha256)||doc.runtimeAdmitted||doc.nativeExecuted||doc.rows==null||doc.rows.Length!=10||doc.states==null||doc.states.Length!=2||npcLakeSourceV18==null||doc.sourceRulesSha256!=npcLakeSourceV18.sourceRulesSha256||!doc.staticPassiveBoneAtRoot||doc.sourcePassiveBoneSha256!="ff6d2d8232892f81761cbd923764d89eb459ff358063e24007c5021fcf3c48a1")throw new ArgumentException("Incomplete/unbound original Lake visual calls.");
    var names=new[]{"LizardMan_ShamanKing_SP_Heal01","LizardMan_ShamanKing_SP_Heal02","LizardMan_ShamanKing_SP_Heal03","LizardMan_ShamanKing_SP_Heal04","Heal_Effect01","LobbyLevelUp01","Besma_Totem_FireAttack02","Besma_Totem_FireAttack03","Besma_Totem_FireAttack04","Besma_Totem_FireAttack01"};
    var times=new[]{.001f,.034f,.034f,.119f,.119f,.119f,.6f,.01f,.6f,3.4f};var pcs=new[]{58,95,109,137,210,226,35,81,124,156};
    for(int i=0;i<10;i++){
     var r=doc.rows[i];bool shaman=i<6,rotate=i==6||i==7||i==9;string npc=shaman?"NUI_LIZARDMAN_SHAMAN":"NUI_BESMA_TOTEM_BASE",state=shaman?"LIZARDMAN_SHAMAN_ATTACK_B":"BESMA_TOTEM_BASE_YOGA_FIRE";
     string origin=i==0||i>=6?"bone":i>=4?"eligible-target":"owner";string bone=i==0?"Dummy2_Lhand":i>=6?"Dummy1_Mouse":"";
     if(r==null||r.index!=i||r.template!=names[i]||!LakeNearV18(r.threshold,times[i])||r.createCallPc!=pcs[i]||r.npc!=npc||r.state!=state||r.origin!=origin||(r.bone??"")!=bone||r.storedMinorSlot0!=(i==0)||r.ownerSetAddRotate!=rotate||r.ownerSetAxisAngle!=rotate||r.manager!=(i<4?"GetMinorParticle":"GetMajorParticle")||r.clock!=(shaman?"source-animation-seconds":"source-state-seconds")||r.callback!=(shaman?"LIZARDMAN_SHAMAN_SP_HEAL_FRAME_MOVE":"BESMA_TOTEM_BASE_YOGA_FIRE_FRAME_MOVE")||r.sourceInstructionsSha256!=(shaman?"f14b3f297719045239fe0c213ea433d74d689028f76a77ae7ae2fd13efea57b1":"493c17aefe52e3741c032ffac07939c1a575ea16ca6fd7ddfe204348984b2c4e")||r.argumentSentinels==null||r.argumentSentinels.Length!=4||Array.Exists(r.argumentSentinels,v=>v!=-1))throw new ArgumentException("Original Lake visual timing/origin/rotation/ownership changed.");
    }
    var unique=new HashSet<string>();
    foreach(var s in doc.states){NpcStateDefinition state;string key=s==null?"":s.npc+"/"+s.state;
     if(s==null||!unique.Add(key)||!npcStates.TryGetValue(key,out state)||!LakeNearV18(s.duration,state.duration)||!LakeNearV18(s.sourceAnimSpeed,state.sourceAnimSpeed)||s.tracks==null||s.tracks.Length!=1||s.tracks[0]==null||s.tracks[0].bone!=(s.npc=="NUI_LIZARDMAN_SHAMAN"?"Dummy2_Lhand":"Dummy1_Mouse")||s.tracks[0].samples==null||s.tracks[0].samples.Length<(int)Math.Ceiling(s.duration*60-.001f)+1)throw new ArgumentException("Missing original Lake visual bone track.");
     foreach(var p in s.tracks[0].samples)if(p==null||!Finite(p.x)||!Finite(p.y)||!Finite(p.z))throw new ArgumentException("Invalid original Lake visual bone point.");
    }
    if(!unique.SetEquals(new[]{"NUI_LIZARDMAN_SHAMAN/LIZARDMAN_SHAMAN_ATTACK_B","NUI_BESMA_TOTEM_BASE/BESMA_TOTEM_BASE_YOGA_FIRE"}))throw new ArgumentException("Lake visual source states changed.");
   }
   npcLakeVisualV18=doc;ClearNpcLakeVisualV18();
  }
  void ClearNpcLakeVisualV18(){lakeVisualCursorsV18.Clear();npcLakeVisualEventsV18.Clear();lakeVisualSequenceV18=lakeVisualHandleV18=lakeVisualEntryV18=0;NpcLakeVisualGenerationV18++;}
  bool LakeVisualManagerV18(NpcLakeVisualCallV18 row){return row.manager=="GetMinorParticle"?NpcLakeMinorVisualManagerAvailableV18:NpcLakeMajorVisualManagerAvailableV18;}
  NpcLakeVisualEventV18 LakeVisualCommandV18(string type,NpcLakeVisualCallV18 row,Enemy owner,Enemy target,LakeVisualCursorV18 cursor,float at,int handle,float local){
   float x=target==null?owner.x:target.x,y=target==null?owner.y:target.y,z=0;
   if(row.origin=="bone"){
    var s=Array.Find(npcLakeVisualV18.states,v=>v.npc==owner.sourceNPC&&v.state==row.state);var points=s.tracks[0].samples;float frame=Clamp(local*60,0,points.Length-1);int a=(int)frame,b=Math.Min(a+1,points.Length-1);float blend=frame-a;
    x+=owner.facing*(points[a].x+(points[b].x-points[a].x)*blend)*owner.visualScale;y+=(points[a].y+(points[b].y-points[a].y)*blend)*owner.visualScale;z=(points[a].z+(points[b].z-points[a].z)*blend)*owner.visualScale;
   }
   var e=new NpcLakeVisualEventV18{sequence=++lakeVisualSequenceV18,type=type,handle=handle,callIndex=row.index,npc=row.npc,state=row.state,manager=row.manager,template=row.template,bone=row.bone,owner=owner,target=target,entry=cursor.entry,time=at,x=x,y=y,z=z,facing=owner.facing,depth=target==null?owner.nativeDepth:target.nativeDepth,unit=.28f,ownerSetAddRotate=row.ownerSetAddRotate,ownerSetAxisAngle=row.ownerSetAxisAngle};
   if(npcLakeVisualEventsV18.Count>=2048)npcLakeVisualEventsV18.RemoveAt(0);npcLakeVisualEventsV18.Add(e);return e;
  }
  void OnNpcLakeVisualStateEnteringV18(Enemy owner,NpcStateDefinition next){
   if(npcLakeVisualV18==null||owner==null)return;EndNpcLakeVisualStateV18(owner,elapsed);
   lakeVisualCursorsV18[owner]=new LakeVisualCursorV18{state=next.id,entry=++lakeVisualEntryV18};
  }
  void EndNpcLakeVisualStateV18(Enemy owner,float at){
   LakeVisualCursorV18 cursor;if(npcLakeVisualV18==null||!lakeVisualCursorsV18.TryGetValue(owner,out cursor)||cursor.heldMinor0==0)return;
   // Only the actual stored Heal01 slot has an explicit STATE_END destroy.
   if(NpcLakeMinorVisualManagerAvailableV18)LakeVisualCommandV18("destroy",npcLakeVisualV18.rows[0],owner,null,cursor,at,cursor.heldMinor0,cursor.previous);
   cursor.heldMinor0=0;
  }
  public void NotifyNpcLakeVisualExpiredV18(int handle){
   if(npcLakeVisualV18==null||handle<=0)return;
   foreach(var pair in lakeVisualCursorsV18)if(pair.Value.heldMinor0==handle){LakeVisualCommandV18("clear-expired",npcLakeVisualV18.rows[0],pair.Key,null,pair.Value,elapsed,handle,pair.Value.previous);pair.Value.heldMinor0=0;return;}
  }
  LakeVisualCursorV18 LakeVisualCursorForV18(Enemy owner,NpcStateDefinition state,float from){
   LakeVisualCursorV18 cursor;
   if(!lakeVisualCursorsV18.TryGetValue(owner,out cursor)||cursor.state!=state.id||from+1e-6f<cursor.previous){OnNpcLakeVisualStateEnteringV18(owner,state);cursor=lakeVisualCursorsV18[owner];}
   return cursor;
  }
  void AdvanceNpcLakeVisualV18(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(npcLakeVisualV18==null||owner==null||owner.hp<=0||!owner.active||owner.dormant)return;
   bool bound=Array.Exists(npcLakeVisualV18.states,s=>s.npc==owner.sourceNPC&&s.state==state.id);if(!bound)return;
   var cursor=LakeVisualCursorForV18(owner,state,from);
   // The source callback follows its stored handle before timer processing.
   if(cursor.heldMinor0!=0&&NpcLakeMinorVisualManagerAvailableV18)LakeVisualCommandV18("move",npcLakeVisualV18.rows[0],owner,null,cursor,packetStart+to-from,cursor.heldMinor0,to);
   foreach(var row in npcLakeVisualV18.rows){if(row.npc!=owner.sourceNPC||row.state!=state.id||row.origin=="eligible-target")continue;float at=row.clock=="source-animation-seconds"?row.threshold/state.sourceAnimSpeed:row.threshold;
    if((cursor.emittedMask&(1<<row.index))!=0||from>=at||at>to+1e-6f)continue;cursor.emittedMask|=1<<row.index;if(!LakeVisualManagerV18(row))continue;
    int handle=++lakeVisualHandleV18;LakeVisualCommandV18("create",row,owner,null,cursor,packetStart+at-from,handle,at);if(row.storedMinorSlot0)cursor.heldMinor0=handle;
   }
   cursor.previous=to;
  }
  void EmitNpcLakeHealTargetVisualV18(Enemy owner,Enemy target,float absolute){
   if(npcLakeVisualV18==null)return;
   if(owner==null||target==null||owner.sourceNPC!="NUI_LIZARDMAN_SHAMAN"||owner.npcState!="LIZARDMAN_SHAMAN_ATTACK_B"||target.hp<=0||target.hp>=target.maxHp)throw new InvalidOperationException("Lake heal visual must use the actual eligible target before HP mutation.");
   var state=NpcState(owner);LakeVisualCursorV18 cursor;if(!lakeVisualCursorsV18.TryGetValue(owner,out cursor)||cursor.state!=state.id||(cursor.emittedMask&(1<<3))==0)throw new InvalidOperationException("Original.119 visual timer must precede the actual eligible-target loop.");
   if(!cursor.healedTargets.Add(target))return;
   for(int i=4;i<=5;i++){var row=npcLakeVisualV18.rows[i];if(LakeVisualManagerV18(row))LakeVisualCommandV18("create",row,owner,target,cursor,absolute,++lakeVisualHandleV18,row.threshold/state.sourceAnimSpeed);}
  }
  // Call after real damage/actor updates, even on frames without normal AI.
  // Removes only the explicit held handle after an actual interruption/death.
  void TickNpcLakeVisualOwnershipV18(){
   if(npcLakeVisualV18==null)return;
   foreach(var pair in lakeVisualCursorsV18)if(pair.Value.heldMinor0!=0&&(pair.Key.hp<=0||pair.Key.state!=CombatState.Normal||pair.Key.npcState!=pair.Value.state||!enemies.Contains(pair.Key)))EndNpcLakeVisualStateV18(pair.Key,elapsed);
  }
  // A renderer consumes the actual moving effect and real source age. It must
  // not generate another flight path or infer omitted native flag defaults.
  public bool TryGetNpcLakePassivePoseV18(NpcEffectInstance effect,string particle,out NpcLakePassivePoseV18 pose){
   pose=null;if(npcLakeVisualV18==null||effect==null||effect.ended||!npcEffects.Contains(effect))return false;
   bool shaman=effect.id=="DamageEffect_LIZARDMAN_SHAMAN_ATTACK_A"&&(particle=="LizardMan_Shamanking_DrainHit02"||particle=="LizardMan_Shamanking_DrainHit03");
   bool older=effect.id=="DamageEffect_LIZARDMAN_OLDER_ATTACK_A"&&particle=="LizardMan_Older_FireAttack_SmallRing";if(!shaman&&!older)return false;
   // Shaman ATTACK_SPHERE01 is a direct-root identity frame at every source
   // matrix key. Its relative origin is exactly zero regardless of X clock.
   pose=new NpcLakePassivePoseV18{serial=effect.serial,effect=effect.id,particle=particle,positionType=shaman?"DEPT_BONE":"DEPT_POS",bone=shaman?"ATTACK_SPHERE01":"",x=effect.x,y=effect.y,z=effect.z,age=effect.age,lifetime=effect.lifetime,facing=effect.facing,unit=.28f*effect.ownerScale,depth=effect.owner==null?0:effect.owner.nativeDepth,sourceHasApplyRotate=older,sourceApplyRotate=false,sourceHasForceDieHit=older,sourceForceDieHit=older};return true;
  }
 }
}
