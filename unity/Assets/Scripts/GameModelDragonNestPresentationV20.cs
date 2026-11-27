namespace ElswordPixel {
 public sealed partial class GameModel {
  public bool DragonNestSourceEnabledV20 {get{return dragonNestV20!=null;}}
  // BAT_B_FIRE_ATTACK_A_FRAME_MOVE, original instructions SHA256
  // 31872a0d75649c82bde5d3b7c3421684e31a4acfa190131f655b4d52c352ec7c.
  // GetStateTime drives colour, independently of the four-second pose clip.
  // Source STATE_END resets G/B to one. Leaving this exact live state returns
  // control to the ordinary actor colour without storing a stale tint.
  public bool TryGetDragonBatSourceColorV20(Enemy owner,out float green,out float blue){
   green=blue=1;
   if(dragonNestV20==null||owner==null||owner.sourceNPC!=dragonNestV20.bat.npc||owner.npcState!=dragonNestV20.bat.fireState||!owner.active||owner.dormant||owner.hp<=0||owner.state!=CombatState.Normal)return false;
   green=blue=(4-owner.npcTime)/4;
   return true;
  }
 }
}
