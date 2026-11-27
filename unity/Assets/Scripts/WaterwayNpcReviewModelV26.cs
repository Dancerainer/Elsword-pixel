using System;
using System.Collections.Generic;
namespace ElswordPixel {
 public sealed class WaterwayNpcReviewStepV26 {
  public string id,targetClip;
  public int ticks,maxTicks=1200,damage;
  public float minimumClock;
  public HitReaction reaction=HitReaction.Hurt;
  public bool waitUnprotected;
 }
 // Shared by the CPU reachability probe and the separate production Player review.
 // Enemy state, clocks, HP, flags, roots and facing are only changed by GameModel.
 public static class WaterwayNpcReviewModelV26 {
  public static int totalSteps,playerHealthRestorations;
  static bool farMode;
  public static void ResetCounters(){totalSteps=playerHealthRestorations=0;farMode=false;}
  public static bool DamageWindow(GameModel g,Enemy e){return e.hp>0&&g.Grounded(e)&&e.state==CombatState.Normal&&!g.EnemyHasSuperArmor(e)&&!g.EnemyHasNpcInvincibility(e)&&e.invincible<=0;}
  public static void LocatePlayer(GameModel g,Enemy e,bool far){
   farMode=far;
   float left=g.HasEncounters?g.CurrentEncounter.leftX+2:2,right=g.HasEncounters?g.CurrentEncounter.rightX-2:g.RoomWidth-2;
   bool mouse=e.sourceNPC=="NUI_MOUSE"||e.sourceNPC=="NUI_MOUSE_NODROP";
   float distance=far?(mouse?160:208):48;
   bool placed=false;
   foreach(float offset in new[]{distance,-distance,distance+16,-distance-16}){
    float x=Math.Max(left,Math.Min(right,e.x+offset));var support=g.SurfaceBelow(x,e.y+24);
    if(support==null||Math.Abs(support.Y(x)-e.y)>96)continue;
    if(far&&Math.Abs(x-e.x)<128)continue;
    g.player.x=x;g.player.y=support.Y(x);g.player.groundLine=support.id;placed=true;break;
   }
   if(!placed){var support=g.Surface(g.player.groundLine);if(support==null||g.player.x<support.x1||g.player.x>support.x2)throw new InvalidOperationException("No real player support for "+e.sourceNPC);g.player.y=support.Y(g.player.x);}
   g.player.vx=g.player.vy=0;
   if(g.player.hp<400){g.player.hp=500;playerHealthRestorations++;}
  }
  public static void Step(GameModel g,Enemy e){
   LocatePlayer(g,e,farMode);g.paused=false;try{g.Step(1f/60,new Input{skill=-1});}finally{g.paused=true;}totalSteps++;
   if(g.player.hp<=0)throw new InvalidOperationException("Diagnostic player died between support/health maintenance steps");
  }
  static bool Matches(Enemy e,string target,float clock){
   if(target==null)return true;
   foreach(string clip in target.Split('|'))if(e.clip==clip&&e.clock+.0001f>=clock)return true;
   return false;
  }
  public static int Advance(GameModel g,Enemy e,WaterwayNpcReviewStepV26 s){
   if(s.id=="natural-walk") {LocatePlayer(g,e,false);Step(g,e);LocatePlayer(g,e,true);}
   else LocatePlayer(g,e,false);
   if(s.waitUnprotected){int n=0;while(!DamageWindow(g,e)&&n++<s.maxTicks)Step(g,e);if(!DamageWindow(g,e))throw new InvalidOperationException("No real unprotected window for "+e.sourceNPC+"/"+s.id);}
   int dealt=s.damage>0?g.ApplyHitToEnemy(e,s.damage,s.reaction):0;
   if(s.damage>0&&dealt<=0)throw new InvalidOperationException("Public damage rejected for "+e.sourceNPC+"/"+s.id);
   if(s.targetClip!=null){int n=0;while(!Matches(e,s.targetClip,s.minimumClock)&&n++<s.maxTicks)Step(g,e);if(!Matches(e,s.targetClip,s.minimumClock))throw new InvalidOperationException("Unreached normal model clip "+s.targetClip+" for "+e.sourceNPC+"/"+s.id+" actual="+e.clip+" state="+e.npcState+" xy="+e.x+","+e.y);}
   for(int n=0;n<s.ticks;n++)Step(g,e);
   return dealt;
  }
  public static List<WaterwayNpcReviewStepV26> FamilySteps(string npc){
   bool mouse=npc=="NUI_MOUSE"||npc=="NUI_MOUSE_NODROP";
   if(!mouse&&npc!="NUI_SOLDIER_THIN"&&npc!="NUI_SOLDIER_FAT"&&npc!="NUI_GHOST_MAGICIAN_RAGGED")throw new ArgumentException("Outside four Waterway families");
   string ready=mouse||npc=="NUI_SOLDIER_THIN"?"attack-ready":"attack-a-ready";
   string attack=mouse?"attack":"attack-a";
   return new List<WaterwayNpcReviewStepV26>{
    new WaterwayNpcReviewStepV26{id="actual-birth"},
    new WaterwayNpcReviewStepV26{id="natural-walk",targetClip="walk",minimumClock=.05f},
    new WaterwayNpcReviewStepV26{id="normal-ready-start",targetClip=ready},
    new WaterwayNpcReviewStepV26{id="normal-ready-middle",targetClip=ready,minimumClock=.2f},
    new WaterwayNpcReviewStepV26{id="normal-attack-start",targetClip=attack},
    new WaterwayNpcReviewStepV26{id="normal-attack-progress",ticks=8},
    new WaterwayNpcReviewStepV26{id="public-hurt-start",waitUnprotected=true,damage=1},
    new WaterwayNpcReviewStepV26{id="public-hurt-progress",ticks=8},
    new WaterwayNpcReviewStepV26{id="public-launch-start",waitUnprotected=true,damage=1,reaction=HitReaction.Launch},
    new WaterwayNpcReviewStepV26{id="physical-descending",targetClip="fall"},
    new WaterwayNpcReviewStepV26{id="physical-down",targetClip="knockdown"},
    new WaterwayNpcReviewStepV26{id="physical-get-up-start",targetClip="getup|get-up"},
    new WaterwayNpcReviewStepV26{id="physical-get-up-progress",ticks=8},
    new WaterwayNpcReviewStepV26{id="public-lethal-start",waitUnprotected=true,damage=1000000},
    new WaterwayNpcReviewStepV26{id="public-death-progress",ticks=36},
   };
  }
 }
}
