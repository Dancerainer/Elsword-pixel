namespace ElswordPixel {
 public sealed partial class GameModel {
  bool GuardianSourceBirthBoundV28(Enemy e,NpcDefinition p){
   if(e==null||!GuardianCombatNativeRulesV28.SourceProfile(p)||!GuardianCombatNativeRulesV28.Identity(e.kind,e.sourceNPC))return false;
   int i=enemies.IndexOf(e);string stage=CurrentRoom.id;
   bool exact=stage=="ruins-1"&&i==4||stage=="ruins-2"&&(i==6||i==7)||stage=="swamp-2"&&i==6;
   if(!exact||i<0||i>=CurrentRoom.enemies.Count)return false;var s=CurrentRoom.enemies[i];
   return s.kind==e.kind&&s.sourceNPC==e.sourceNPC&&s.sourceKeyCode==e.sourceKeyCode&&s.x==e.spawnX&&s.aiEnabled&&!s.dormant&&!s.passive;
  }
  // Only the actual source initial Loop consumes this condition here. It does
  // not acquire the ordinary Wait/Walk chase or invented armor/invisibility.
  bool TickGuardianTrapWaitV28(Enemy e,NpcDefinition p,NpcStateDefinition state,float distance,float dt){
   if(!GuardianSourceBirthBoundV28(e,p)||state==null||state.id!=GuardianCombatNativeRulesV28.TrapWait)return false;
   e.npcDecisionElapsed+=dt;
   if(e.npcDecisionElapsed+1e-6f>=state.interval){
    e.npcDecisionElapsed%=state.interval;
    foreach(var condition in state.choices){
     if(distance>condition.near||e.hp*100>=e.maxHp*condition.hpLessPercent)continue;
     if(condition.rate<100&&NpcRoll()>=condition.rate)continue;
     if(!TryEnterNpcState(e,npcStates[p.id+"/"+condition.nextState]))continue;e.npcDecisionElapsed=0;return true;
    }
   }
   AdvanceNpcLoopSounds(e,state,e.npcTime,dt,elapsed-dt);e.npcTime=(e.npcTime+dt)%state.duration;return true;
  }
  public NpcDefinition GuardianSourceProfileDefinitionV28(Enemy e){var p=NpcProfile(e);return GuardianCombatNativeRulesV28.SourceProfile(p)?p:null;}
  public bool GuardianSourceProfileV28(Enemy e){return GuardianSourceProfileDefinitionV28(e)!=null;}
 }
}
