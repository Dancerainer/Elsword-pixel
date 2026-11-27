using System;
using System.Collections.Generic;
namespace ElswordPixel {
 public sealed class OutskirtsNpcReviewStepV27 {
  public string id,targetClip,targetState,placement="near";
  public int ticks,maxTicks=6000,damage;
  public float minimumClock;
  public HitReaction reaction=HitReaction.Hurt;
  public bool waitUnprotected;
 }
 // Shared CPU/Player review plan. Only public GameModel simulation and hit entry
 // change enemies; review support placement/velocity and bounded player HP are explicit.
 public static class OutskirtsNpcReviewModelV27 {
  public static int totalSteps,playerHealthRestorations;
  static bool farMode;
  public static void ResetCounters(){totalSteps=playerHealthRestorations=0;farMode=false;}
  public static float ModelClock(Enemy e){return OutskirtsNativeRenderRulesV27.RenderClock(e.sourceNPC,(int)e.state,e.npcState,e.clip,e.npcTime,e.clock);}
  public static bool DamageWindow(GameModel g,Enemy e){return e.hp>0&&g.Grounded(e)&&e.state==CombatState.Normal&&!g.EnemyHasSuperArmor(e)&&!g.EnemyHasNpcInvincibility(e)&&e.invincible<=0;}
  public static void LocatePlayer(GameModel g,Enemy e,bool far){
   farMode=far;float left=g.HasEncounters?g.CurrentEncounter.leftX+2:2,right=g.HasEncounters?g.CurrentEncounter.rightX-2:g.RoomWidth-2;
   float distance=far?208:48;bool placed=false;
   foreach(float offset in new[]{distance,-distance,distance+16,-distance-16}){
    float x=Math.Max(left,Math.Min(right,e.x+offset));var support=g.SurfaceBelow(x,e.y+24);
    if(support==null||Math.Abs(support.Y(x)-e.y)>96)continue;
    if(far&&Math.Abs(x-e.x)<180)continue;
    g.player.x=x;g.player.y=support.Y(x);g.player.groundLine=support.id;placed=true;break;
   }
   if(!placed){var support=g.Surface(g.player.groundLine);if(support==null||g.player.x<support.x1||g.player.x>support.x2)throw new InvalidOperationException("No actual player support for "+e.sourceNPC);g.player.y=support.Y(g.player.x);}
   g.player.vx=g.player.vy=0;if(g.player.hp<400){g.player.hp=500;playerHealthRestorations++;}
  }
  public static void Step(GameModel g,Enemy e){
   LocatePlayer(g,e,farMode);bool paused=g.paused;g.paused=false;try{g.Step(1f/60,new Input{skill=-1});}finally{g.paused=paused;}totalSteps++;
   if(g.player.hp<=0)throw new InvalidOperationException("Diagnostic player died during bounded support maintenance");
  }
  static bool Matches(Enemy e,OutskirtsNpcReviewStepV27 s){
   if(s.targetState!=null&&e.npcState!=s.targetState)return false;
   if(s.targetClip!=null){bool found=false;foreach(string clip in s.targetClip.Split('|'))if(e.clip==clip)found=true;if(!found)return false;}
   // An explicit source-state threshold must observe that state's npcTime,
   // including real air pauses; actor.clock cannot stand in for it.
   return (s.targetState!=null?e.npcTime:ModelClock(e))+.0001f>=s.minimumClock;
  }
  public static int Advance(GameModel g,Enemy e,OutskirtsNpcReviewStepV27 s){
   if(s.id=="natural-walk"){LocatePlayer(g,e,false);Step(g,e);LocatePlayer(g,e,true);}else LocatePlayer(g,e,s.placement=="far");
   if(s.waitUnprotected){int n=0;while(!DamageWindow(g,e)&&n++<s.maxTicks)Step(g,e);if(!DamageWindow(g,e))throw new InvalidOperationException("No actual unprotected window "+e.sourceNPC+"/"+s.id);}
   int dealt=s.damage>0?g.ApplyHitToEnemy(e,s.damage,s.reaction):0;if(s.damage>0&&dealt<=0)throw new InvalidOperationException("Public damage rejected "+e.sourceNPC+"/"+s.id);
   if(s.targetClip!=null||s.targetState!=null){int n=0;while(!Matches(e,s)&&n++<s.maxTicks)Step(g,e);if(!Matches(e,s))throw new InvalidOperationException("Unreached "+e.sourceNPC+"/"+s.id+" actual="+e.clip+" "+e.npcState+" state="+e.state+" clocks="+e.clock+","+e.npcTime+" xy="+e.x+","+e.y);}
   for(int n=0;n<s.ticks;n++)Step(g,e);return dealt;
  }
  public static List<OutskirtsNpcReviewStepV27> FamilySteps(string npc){
   if(!OutskirtsNativeRenderRulesV27.Supported(npc))throw new ArgumentException("Outside five Outskirts families");bool boss=npc=="NUI_PPORU_WILLIAM_B"||npc=="NUI_PPORU_WILLIAM_C";string ready=boss?"attack-a-ready":"attack-ready";
   return new List<OutskirtsNpcReviewStepV27>{
    new OutskirtsNpcReviewStepV27{id="actual-birth"},
    new OutskirtsNpcReviewStepV27{id="natural-walk",targetClip="walk",minimumClock=.05f},
    new OutskirtsNpcReviewStepV27{id="normal-ready-start",targetClip=ready},
    new OutskirtsNpcReviewStepV27{id="normal-ready-middle",targetClip=ready,minimumClock=.3f},
    new OutskirtsNpcReviewStepV27{id="normal-attack-start",targetClip="attack-a"},
    new OutskirtsNpcReviewStepV27{id="normal-attack-progress",ticks=8},
    new OutskirtsNpcReviewStepV27{id="public-hurt-start",waitUnprotected=true,damage=1},
    new OutskirtsNpcReviewStepV27{id="public-hurt-progress",ticks=8},
    new OutskirtsNpcReviewStepV27{id="public-launch-start",waitUnprotected=true,damage=1,reaction=HitReaction.Launch},
    new OutskirtsNpcReviewStepV27{id="physical-descending",targetClip="fall"},
    new OutskirtsNpcReviewStepV27{id="physical-down",targetClip="knockdown"},
    new OutskirtsNpcReviewStepV27{id="physical-get-up-start",targetClip="getup|get-up"},
    new OutskirtsNpcReviewStepV27{id="physical-get-up-progress",ticks=8},
    new OutskirtsNpcReviewStepV27{id="public-lethal-start",waitUnprotected=true,damage=1000000},
    new OutskirtsNpcReviewStepV27{id=npc=="NUI_PPORU_WILLIAM_C"?"public-victory-frozen-death":"public-death-progress",ticks=36}
   };
  }
 }
}
