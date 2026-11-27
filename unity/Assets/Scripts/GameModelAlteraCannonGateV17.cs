using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraDeferredCannonGate {
  public string roomId="",kind="";
  public int roomIndex,encounterIndex,parentKeyCode,targetKeyCode,originalMissingStart;
  public float hp;
 }
 public sealed partial class GameModel {
  const string AlteraCannonParent="NUI_CANNON_BIG",AlteraCannonGate="NUI_CANNON_BIG_DOWN";
  const string AlteraCannonGateDisabled="CANNON_BIG_GATE_DISABLED",AlteraCannonGateInvincible="CANNON_BIG_GATE_WAIT_INVINCIBLE",AlteraCannonGateWait="CANNON_BIG_GATE_WAIT";
  const float AlteraCannonGateActivationTime=.9f;
  readonly List<AlteraDeferredCannonGate> alteraDeferredCannonDefinitions=new List<AlteraDeferredCannonGate>();
  sealed class AlteraCannonPosition {
   public AlteraDeferredCannonGate definition;public Enemy parent,actor;
   public float x,y,nativeDepth;public int groundLine,facing;
   public float dyingLandClock;
   public bool activationQueued,activated;
  }
  readonly Dictionary<Enemy,AlteraCannonPosition> alteraCannonPositions=new Dictionary<Enemy,AlteraCannonPosition>();
  public int AlteraCannonEstablishedPositions {get{return alteraCannonPositions.Count;}}
  public float AlteraCannonDyingLandClock(Enemy parent){AlteraCannonPosition position;return parent!=null&&alteraCannonPositions.TryGetValue(parent,out position)?position.dyingLandClock:-1;}

  // The source key1 declaration is reserved without an Enemy or any coordinate.
  // SetRooms must run first. hp/kind are explicit caller-approved library/balance
  // inputs; this API neither assigns an art library nor admits a combat release.
  public void ConfigureAlteraDeferredCannonGates(AlteraDeferredCannonGate[] definitions) {
   var valid=new List<AlteraDeferredCannonGate>();var identities=new HashSet<string>();
   foreach(var d in definitions??new AlteraDeferredCannonGate[0]) {
    if(d==null||d.roomIndex<0||d.roomIndex>=rooms.Count||d.encounterIndex<0||d.encounterIndex>=rooms[d.roomIndex].encounters.Length||
       string.IsNullOrEmpty(d.roomId)||rooms[d.roomIndex].id!=d.roomId||string.IsNullOrEmpty(d.kind)||!Finite(d.hp)||d.hp<=0||
       d.parentKeyCode!=2||d.targetKeyCode!=1||d.originalMissingStart!=33)
     throw new ArgumentException("Altera cannon deferral must bind the original room, parent key2, target key1 and unresolved start33.");
    if(!identities.Add(d.roomIndex+"/"+d.encounterIndex+"/"+d.targetKeyCode))throw new ArgumentException("Duplicate original deferred cannon key.");
    bool parent=false,target=false;
    foreach(var spawn in rooms[d.roomIndex].enemies)if(spawn.encounterIndex==d.encounterIndex&&spawn.sourceNPC==AlteraCannonParent&&spawn.sourceKeyCode==d.parentKeyCode)parent=true;
    foreach(var clause in rooms[d.roomIndex].encounters[d.encounterIndex].clauses)if(clause.mode==EncounterClearMode.KillNpcTypes&&Array.IndexOf(clause.npcTypes,AlteraCannonGate)>=0)target=true;
    if(!parent||!target)throw new ArgumentException("Missing original cannon parent or exact source gate clear target.");
    valid.Add(new AlteraDeferredCannonGate{roomId=d.roomId,roomIndex=d.roomIndex,encounterIndex=d.encounterIndex,parentKeyCode=d.parentKeyCode,targetKeyCode=d.targetKeyCode,originalMissingStart=d.originalMissingStart,kind=d.kind,hp=d.hp});
   }
   alteraDeferredCannonDefinitions.Clear();alteraDeferredCannonDefinitions.AddRange(valid);alteraCannonPositions.Clear();
  }
  void ResetAlteraDeferredCannonState(){alteraCannonPositions.Clear();}

  // Call at the original CANNON_BIG_DYING_LAND_STATE_START. Source instructions
  // take parent.GetPos and pass the very same vector to key1.SetPosition(...,
  // false). There is no offset and no fallback to nonexistent start33.
  public bool NotifyAlteraCannonDeathStarted(Enemy parent) {
   if(parent==null||parent.sourceNPC!=AlteraCannonParent||parent.hp>0||!enemies.Contains(parent)||!HasEncounters||parent.encounterIndex!=currentEncounter)return false;
   if(alteraCannonPositions.ContainsKey(parent))return true;
   AlteraDeferredCannonGate selected=null;
   foreach(var d in alteraDeferredCannonDefinitions)if(d.roomIndex==room&&d.encounterIndex==parent.encounterIndex&&d.parentKeyCode==parent.sourceKeyCode){selected=d;break;}
   if(selected==null)return false;
   if(!Finite(parent.x)||!Finite(parent.y)||!Finite(parent.nativeDepth))throw new InvalidOperationException("Original cannon death position is not finite.");
   Enemy existing=null;
   foreach(var actor in enemies)if(actor.encounterIndex==currentEncounter&&actor.sourceKeyCode==selected.targetKeyCode){
    if(existing!=null||actor.sourceNPC!=AlteraCannonGate)throw new InvalidOperationException("Original target key1 is ambiguous or bound to another identity.");existing=actor;
   }
   var position=new AlteraCannonPosition{definition=selected,parent=parent,actor=existing,x=parent.x,y=parent.y,nativeDepth=parent.nativeDepth,groundLine=parent.groundLine,facing=parent.facing};
   if(existing!=null){existing.x=existing.spawnX=position.x;existing.y=position.y;existing.nativeDepth=position.nativeDepth;existing.groundLine=position.groundLine;existing.active=false;existing.npcState=AlteraCannonGateDisabled;}
   alteraCannonPositions.Add(parent,position);return true;
  }

  // Observe the source dying animation clock. Queue only; callers may be inside
  // an enemies foreach. Flushing outside that loop materializes the actor.
  public bool AdvanceAlteraCannonDeath(Enemy parent,float previousClock,float currentClock) {
   if(!Finite(previousClock)||!Finite(currentClock)||previousClock<0||currentClock<previousClock)throw new ArgumentException("Invalid source cannon dying clock.");
   AlteraCannonPosition position;
   if(parent==null||!alteraCannonPositions.TryGetValue(parent,out position)||position.definition.roomIndex!=room||!enemies.Contains(parent))return false;
   if(!position.activated&&previousClock<AlteraCannonGateActivationTime&&currentClock>=AlteraCannonGateActivationTime)position.activationQueued=true;
   return position.activationQueued||position.activated;
  }

  // Conservative host wrapper: generic deathTime can include an airborne death
  // and must not stand in for the source DYING_LAND animation. Start at the first
  // actual grounded death pose, then advance only the actor-clock delta actually
  // consumed by TickEnemy (zero during hit pause). The first landing frame only
  // establishes state-start/clock0, so pre-landing time cannot trigger0.9 early.
  public void TickAlteraDeferredCannonDeathV17(Enemy parent,float actualClockDelta) {
   if(!Finite(actualClockDelta)||actualClockDelta<0)throw new ArgumentException("Invalid consumed cannon actor-clock delta.");
   if(parent==null||parent.sourceNPC!=AlteraCannonParent||parent.hp>0||!enemies.Contains(parent))return;
   AlteraCannonPosition position;
   if(!alteraCannonPositions.TryGetValue(parent,out position)){
    if(Grounded(parent))NotifyAlteraCannonDeathStarted(parent);
    return;
   }
   float previous=position.dyingLandClock;position.dyingLandClock+=actualClockDelta;
   AdvanceAlteraCannonDeath(parent,previous,position.dyingLandClock);
  }

  void EnterAlteraCannonGateState(Enemy actor,string stateId) {
   NpcStateDefinition source;
   if(npcStates.TryGetValue(AlteraCannonGate+"/"+stateId,out source))EnterNpcState(actor,source);
   else {actor.npcState=stateId;actor.npcTime=0;actor.clip=CannonWreckClipV39(stateId);actor.clock=0;}
  }
  static string CannonWreckClipV39(string stateId) {
   switch(stateId) {
    case "CANNON_BIG_GATE_DAMAGE":return "hurt";
    case "CANNON_BIG_GATE_DYING":return "broken";
    case AlteraCannonGateDisabled:
    case AlteraCannonGateInvincible:
    case AlteraCannonGateWait:return "idle";
    default:throw new ArgumentOutOfRangeException(nameof(stateId),stateId,"Unknown original cannon gate state");
   }
  }
  bool OriginalCannonWreckV39(Enemy actor) {
   if(actor==null||actor.sourceNPC!=AlteraCannonGate||!actor.isGate||!actor.active||actor.dormant)return false;
   foreach(var position in alteraCannonPositions.Values)
    if(position.activated&&position.actor==actor&&position.definition.roomIndex==room)return true;
   return false;
  }
  void RecordCannonWreckImpactV39(Enemy actor) {
   if(!OriginalCannonWreckV39(actor))return;
   // Cannon_Big_Gate.lua maps every ordinary contact reaction to DAMAGE and both deaths to DYING.
   // The stationary gate still absorbs knockback through the ordinary gate damage path.
   EnterAlteraCannonGateState(actor,actor.hp>0?"CANNON_BIG_GATE_DAMAGE":"CANNON_BIG_GATE_DYING");
  }
  void TickCannonWreckStateV39(Enemy actor) {
   if(actor.npcState!="CANNON_BIG_GATE_DAMAGE"&&actor.npcState!="CANNON_BIG_GATE_DYING")return;
   if(!OriginalCannonWreckV39(actor))return;
   actor.npcTime=actor.clock;
   if(actor.hp>0&&actor.npcState=="CANNON_BIG_GATE_DAMAGE"&&actor.clock>=.7f)
    EnterAlteraCannonGateState(actor,AlteraCannonGateWait);
  }

  // Call after enemy source-clock updates and before TickEncounters. Source
  // LiveActiveNPCNum()==0 excludes the originally inactive gate itself.
  public int FlushAlteraDeferredCannonGates() {
   int created=0;
   foreach(var position in alteraCannonPositions.Values) {
    if(position.definition.roomIndex!=room||position.definition.encounterIndex!=currentEncounter||!enemies.Contains(position.parent))continue;
    if(position.activationQueued&&!position.activated) {
     var actor=position.actor;
     if(actor==null){actor=new Enemy{sourceNPC=AlteraCannonGate,sourceKeyCode=position.definition.targetKeyCode,encounterIndex=currentEncounter,
       kind=position.definition.kind,x=position.x,spawnX=position.x,y=position.y,groundLine=position.groundLine,nativeDepth=position.nativeDepth,facing=position.facing,
       hp=position.definition.hp,maxHp=position.definition.hp,active=true,aiEnabled=false,passive=false,dormant=false,isGate=true,countsForClear=false,
       visualScale=1,state=CombatState.Normal,invincible=3600};enemies.Add(actor);position.actor=actor;created++;}
     actor.active=true;actor.dormant=false;actor.passive=false;actor.aiEnabled=false;actor.isGate=true;actor.countsForClear=false;actor.invincible=3600;
     EnterAlteraCannonGateState(actor,AlteraCannonGateInvincible);position.activated=true;position.activationQueued=false;
    }
    if(position.activated&&position.actor.hp>0&&position.actor.npcState==AlteraCannonGateInvincible&&EncounterCombatClear){
     position.actor.invincible=0;EnterAlteraCannonGateState(position.actor,AlteraCannonGateWait);
    }
   }
   return created;
  }
  public bool AlteraCannonGateGaugeVisible(Enemy actor){return actor!=null&&actor.sourceNPC==AlteraCannonGate&&actor.hp>0&&actor.active&&actor.npcState!=AlteraCannonGateDisabled&&actor.npcState!=AlteraCannonGateInvincible;}
 }
}
