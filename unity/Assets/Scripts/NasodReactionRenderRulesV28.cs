using System;
namespace ElswordPixel {
 // Display only: GameModel owns clocks, physics, protection, hits and state lifetimes.
 public static class NasodReactionRenderRulesV28 {
  public static bool Supported(bool dungeon5,string npc){return dungeon5&&npc=="NUI_BENDERS_NASOD";}
  public static string Canonical(string s){return s=="launched"?"launch":s=="knockdown"?"down":s=="getup"?"get-up":s;}
  public static bool Alias(string s){return s=="source-hurt"||s=="source-launch"||s=="source-fall"||s=="source-down"||s=="source-get-up"||s=="source-death";}
  public static string Select(bool dungeon5,string npc,string previous,int combatState,bool knockdownOnLanding,float actorClock,float previousClock,float deathTime,Func<string,float> duration,out float clock){
   clock=previousClock;if(!Supported(dungeon5,npc)||string.IsNullOrEmpty(previous)||previous.StartsWith("source-",StringComparison.Ordinal))return previous;
   string selected=Canonical(previous);
   bool reaction=selected=="hurt"&&combatState==2||(selected=="launch"||selected=="fall")&&combatState==1&&knockdownOnLanding||selected=="down"&&combatState==3||selected=="get-up"&&combatState==4||selected=="death"&&combatState==5;
   if(!reaction)return previous;
   string target="source-"+selected;float span=duration(target);
   if(float.IsNaN(span)||float.IsInfinity(span)||span<=0)throw new InvalidOperationException("Missing reviewed Nasod reaction: "+target);
   if(float.IsNaN(actorClock)||float.IsInfinity(actorClock)||actorClock<0||float.IsNaN(deathTime)||float.IsInfinity(deathTime))throw new InvalidOperationException("Invalid Nasod display clock");
   clock=selected=="death"?Math.Max(0,Math.Min(1,deathTime/1.2f))*span:selected=="get-up"?previousClock:actorClock;
   return target;
  }
  public static bool TryUiAnchor(bool dungeon5,string npc,string actualClip,float centerX,float topY,float width,float hp,float maxHp,out float bx,out float by,out float dx,out float dy){
   bx=by=dx=dy=0;if(!Supported(dungeon5,npc)||!Alias(actualClip))return false;
   if(float.IsNaN(centerX)||float.IsInfinity(centerX)||float.IsNaN(topY)||float.IsInfinity(topY)||float.IsNaN(width)||float.IsInfinity(width)||width<=0||float.IsNaN(hp)||float.IsInfinity(hp)||float.IsNaN(maxHp)||float.IsInfinity(maxHp)||maxHp<=0)throw new InvalidOperationException("Invalid actual Nasod sprite/UI bounds");
   bx=centerX-width*(1-hp/maxHp)/2;by=topY+8;dx=centerX;dy=topY+20;return true;
  }
 }
}

