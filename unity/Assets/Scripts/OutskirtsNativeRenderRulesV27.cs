using System;
namespace ElswordPixel {
 // Display selection only: source gameplay clock, position, facing and protection
 // remain owned by the unchanged GameModel and original helper tracks.
 public static class OutskirtsNativeRenderRulesV27 {
  public static bool Monkey(string npc){return npc=="NUI_MONKEY_BOMB";}
  public static bool Supported(string npc){return npc=="NUI_MONKEY_BOMB"||npc=="NUI_PPORU_WILLIAM_THIEF"||npc=="NUI_PPORU_WILLIAM_THIEF_FIRE"||npc=="NUI_PPORU_WILLIAM_B"||npc=="NUI_PPORU_WILLIAM_C";}
  public static string StateDisplayClip(string npc,string sourceState){
   if(npc=="NUI_MONKEY_BOMB") {
    if(sourceState=="MONKEY_BOMB_WAIT")return "idle";
    if(sourceState=="MONKEY_BOMB_WALK")return "walk";
    if(sourceState=="MONKEY_BOMB_ATTACK_READY")return "attack-ready";
    if(sourceState=="MONKEY_BOMB_ATTACK_A")return "attack-a";
    if(sourceState=="MONKEY_BOMB_STAND_UP_FRONT")return "get-up";
   }
   if(npc=="NUI_PPORU_WILLIAM_THIEF") {
    if(sourceState=="PPORU_WILLIAM_THIEF_WAIT")return "idle";
    if(sourceState=="PPORU_WILLIAM_THIEF_WALK")return "walk";
    if(sourceState=="PPORU_WILLIAM_THIEF_ATTACK_READY")return "attack-ready";
    if(sourceState=="PPORU_WILLIAM_THIEF_ATTACK_A")return "attack-a";
    if(sourceState=="PPORU_WILLIAM_THIEF_STAND_UP_FRONT")return "get-up";
   }
   if(npc=="NUI_PPORU_WILLIAM_THIEF_FIRE") {
    if(sourceState=="PPORU_WILLIAM_THIEF_FIRE_WAIT")return "idle";
    if(sourceState=="PPORU_WILLIAM_THIEF_FIRE_WALK")return "walk";
    if(sourceState=="PPORU_WILLIAM_THIEF_FIRE_ATTACK_READY")return "attack-ready";
    if(sourceState=="PPORU_WILLIAM_THIEF_FIRE_ATTACK_A")return "attack-a";
    if(sourceState=="PPORU_WILLIAM_THIEF_FIRE_ATTACK_A_END")return "attack-a-end";
    if(sourceState=="PPORU_WILLIAM_THIEF_FIRE_STAND_UP_FRONT")return "get-up";
   }
   if(npc=="NUI_PPORU_WILLIAM_B") {
    if(sourceState=="PPORU_WILLIAM_B_WAIT")return "idle";
    if(sourceState=="PPORU_WILLIAM_B_WALK")return "walk";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_A_READY")return "attack-a-ready";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_A")return "attack-a";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_B_READY")return "attack-b-ready";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_B")return "attack-b";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_B_END")return "attack-b-end";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_C_READY")return "attack-c-ready";
    if(sourceState=="PPORU_WILLIAM_B_ATTACK_C")return "attack-c";
    if(sourceState=="PPORU_WILLIAM_B_SPECIAL_ATTACK_READY")return "attack-special-ready";
    if(sourceState=="PPORU_WILLIAM_B_SPECIAL_ATTACK")return "attack";
    if(sourceState=="PPORU_WILLIAM_B_DEFENCE")return "defence";
    if(sourceState=="PPORU_WILLIAM_B_STAND_UP_FRONT")return "get-up";
   }
   if(npc=="NUI_PPORU_WILLIAM_C") {
    if(sourceState=="PPORU_WILLIAM_C_WAIT")return "idle";
    if(sourceState=="PPORU_WILLIAM_C_WALK")return "walk";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_A_READY")return "attack-a-ready";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_A")return "attack-a";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_B_READY")return "attack-b-ready";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_B")return "attack-b";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_B_END")return "attack-b-end";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_C_READY")return "attack-c-ready";
    if(sourceState=="PPORU_WILLIAM_C_ATTACK_C")return "attack-c";
    if(sourceState=="PPORU_WILLIAM_C_SPECIAL_ATTACK_READY")return "attack-special-ready";
    if(sourceState=="PPORU_WILLIAM_C_SPECIAL_ATTACK")return "attack";
    if(sourceState=="PPORU_WILLIAM_C_DEFENCE")return "defence";
    if(sourceState=="PPORU_WILLIAM_C_STAND_UP_FRONT")return "get-up";
   }
   return null;
  }
  public static float RenderClock(string npc,int combatState,string sourceState,string displayClip,float npcTime,float actorClock){
   string authored=StateDisplayClip(npc,sourceState);
   // Normal=0, Airborne=1. The real C dash can remain attack-b while
   // falling: its state/helper clock pauses, and the authored pose must pause.
   // Generic jump/fall and Hurt/Knockdown/GettingUp keep actorClock.
   return (combatState==0||combatState==1)&&!string.IsNullOrEmpty(authored)&&Canonical(displayClip)==authored?npcTime:actorClock;
  }
  public static float SourceClock(bool normal,string sourceState,float npcTime,float actorClock){return normal&&!string.IsNullOrEmpty(sourceState)?npcTime:actorClock;}
  static bool Allowed(string npc,string clip){
   if(npc=="NUI_MONKEY_BOMB")return clip=="attack-a"||clip=="attack-ready"||clip=="death"||clip=="down"||clip=="fall"||clip=="get-up"||clip=="hurt"||clip=="idle"||clip=="jump"||clip=="launch"||clip=="walk";
   if(npc=="NUI_PPORU_WILLIAM_THIEF")return clip=="attack-a"||clip=="attack-ready"||clip=="death"||clip=="down"||clip=="fall"||clip=="get-up"||clip=="hurt"||clip=="idle"||clip=="jump"||clip=="launch"||clip=="walk";
   if(npc=="NUI_PPORU_WILLIAM_THIEF_FIRE")return clip=="attack-a"||clip=="attack-a-end"||clip=="attack-ready"||clip=="death"||clip=="down"||clip=="fall"||clip=="get-up"||clip=="hurt"||clip=="idle"||clip=="jump"||clip=="launch"||clip=="walk";
   if(npc=="NUI_PPORU_WILLIAM_B")return clip=="attack"||clip=="attack-a"||clip=="attack-a-ready"||clip=="attack-b"||clip=="attack-b-end"||clip=="attack-b-ready"||clip=="attack-c"||clip=="attack-c-ready"||clip=="attack-special-ready"||clip=="death"||clip=="defence"||clip=="down"||clip=="fall"||clip=="get-up"||clip=="hurt"||clip=="idle"||clip=="jump"||clip=="launch"||clip=="walk";
   if(npc=="NUI_PPORU_WILLIAM_C")return clip=="attack"||clip=="attack-a"||clip=="attack-a-ready"||clip=="attack-b"||clip=="attack-b-end"||clip=="attack-b-ready"||clip=="attack-c"||clip=="attack-c-ready"||clip=="attack-special-ready"||clip=="death"||clip=="defence"||clip=="down"||clip=="fall"||clip=="get-up"||clip=="hurt"||clip=="idle"||clip=="jump"||clip=="launch"||clip=="walk";
   return false;
  }
  static string Canonical(string s){if(s=="launched")return "launch";if(s=="knockdown")return "down";if(s=="getup")return "get-up";return s;}
  public static string Select(bool outskirts,string npc,string previous,float modelClock,float previousClock,float velocityY,float deathTime,Func<string,float> duration,out float resultClock){
   resultClock=previousClock;
   if(!outskirts||!Supported(npc)||string.IsNullOrEmpty(previous)||previous.StartsWith("source-",StringComparison.Ordinal))return previous;
   string selected=Canonical(previous),target=null;
   if(selected=="jump")selected=velocityY<0?"fall":"jump";
   if(Allowed(npc,selected))target="source-"+selected;
   if(target==null)return previous;
   float span=duration(target);if(float.IsNaN(span)||float.IsInfinity(span)||span<=0)throw new InvalidOperationException("Missing Outskirts native candidate clip: "+target);
   if(float.IsNaN(modelClock)||float.IsInfinity(modelClock)||modelClock<0||float.IsNaN(deathTime)||float.IsInfinity(deathTime))throw new InvalidOperationException("Invalid Outskirts native display clock");
   // Alias exposure has already applied sourceAnimSpeed exactly once, including
   // AttackA_Ready speed1.25; no state duration or source helper is changed here.
   resultClock=modelClock;
   if(selected=="death")resultClock=Math.Max(0,Math.Min(1,deathTime/1.2f))*span;
   return target;
  }
 }
}
