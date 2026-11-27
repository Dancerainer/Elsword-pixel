namespace ElswordPixel {
 public sealed partial class GameModel {
  bool Remaining3SourceBirthBoundV28(Enemy e,NpcDefinition p){
   if(e==null||p==null||e.kind!=p.kind||e.sourceNPC!=p.id||!Remaining3AttackSourceRulesV28.SourceProfile(p))return false;
   int i=enemies.IndexOf(e);string stage=CurrentRoom.id;
   bool exact=p.id=="NUI_PPORU_GIANT_WHITE"?stage=="tree-3"&&i==2:p.id=="NUI_ANCIENT_PPORU"?stage=="swamp-3"&&i==0:stage=="swamp-1"&&(i==2||i==4||i==6);
   if(!exact||i<0||i>=CurrentRoom.enemies.Count)return false;var spawn=CurrentRoom.enemies[i];
   return spawn.kind==e.kind&&spawn.sourceNPC==e.sourceNPC&&spawn.sourceKeyCode==e.sourceKeyCode&&spawn.x==e.spawnX;
  }
  // Read-only view of the actual loaded model profile, never catalog absence.
  public NpcDefinition Remaining3SourceProfileDefinitionV28(Enemy e){var p=NpcProfile(e);return Remaining3AttackSourceRulesV28.SourceProfile(p)?p:null;}
  public bool Remaining3SourceProfileV28(Enemy e){return Remaining3SourceProfileDefinitionV28(e)!=null;}
  float Remaining3SourceAlertRangeV28(Enemy e,NpcDefinition p){return Remaining3SourceProfileV28(e)?p.senseRange:System.Math.Max(0,e.aggroRange);}
  bool Remaining3AllowsKnockdownV28(Enemy e){return Remaining3AttackSourceRulesV28.AllowsKnockdown(Remaining3SourceProfileDefinitionV28(e));}
  HitReaction Remaining3ReactionV28(Enemy e,HitReaction reaction){return Remaining3AttackSourceRulesV28.Reaction(Remaining3SourceProfileDefinitionV28(e),reaction);}
 }
}
