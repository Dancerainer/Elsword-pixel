using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  const string CastleBossId="NUI_WALLY_8TH";
  const string CastleMissileId="DamageEffect_WALLY_8TH_GUIDED_MISSILE";
  const string CastleBeamId="DamageEffect_WALLY_8TH_ATTACK_SPECIAL_ATTACK2";
  internal const float CastleBossBeamHalfHeight=18f;

  bool IsCastleBossEffect(NpcEffectInstance effect,string id){
   return effect.id==id&&effect.owner?.sourceNPC==CastleBossId&&CurrentRoom.id=="castle-center-2";
  }
  bool IsCastleGuidedMissile(NpcEffectInstance effect){return IsCastleBossEffect(effect,CastleMissileId);}
  internal bool IsCastleBossBeam(NpcEffectInstance effect){return IsCastleBossEffect(effect,CastleBeamId);}

  static NpcEffectDefinition ConfigureCastleMissileContact(NpcEffectDefinition definition){
   if(definition.id!=CastleMissileId||string.IsNullOrEmpty(definition.deathChild))return definition;
   var missile=definition.CopyDragonEffect();
   missile.contactChild=missile.deathChild;
   missile.damageUntil=missile.lifetime;
   return missile;
  }
  void SetCastleBeamOrigin(NpcEffectInstance effect){
   if(!IsCastleBossBeam(effect))return;
   var owner=effect.owner;
   effect.x=owner.x+owner.facing*41f*owner.visualScale;
   effect.y=owner.y+48f*owner.visualScale;
   effect.z=0;
  }
  float[] CastleBossBeamLine(NpcEffectInstance effect){
   float roomEdge=effect.facing>0?RoomWidth:0;
   if(NativeTerrain)roomEdge=NativeClampX(roomEdge);
   float extension=Clamp(effect.age/.14f,0,1);
   float forwardSpace=Math.Max(0,(roomEdge-effect.x)*effect.facing);
   float length=forwardSpace*(.12f+.88f*extension);
   return new[]{effect.x,effect.y,effect.z,effect.x+effect.facing*length,effect.y,effect.z};
  }
  bool MoveCastleGuidedMissile(NpcEffectInstance missile,float distance,float step){
   if(!IsCastleGuidedMissile(missile))return false;
   float previousY=missile.y;
   missile.x+=missile.dirX*distance;
   missile.y+=missile.dirY*distance;
   // Both launchers are offset in depth. Converge onto the player's combat lane.
   float depthStep=140f*step;
   missile.z=Clamp(0,missile.z-depthStep,missile.z+depthStep);
   if(missile.dirY>=0)return true;
   var support=SurfaceBelow(missile.x,previousY+.05f);
   if(support==null)return true;
   float floor=support.Y(missile.x);
   if(missile.y<=floor){
    missile.y=floor;
    EndNpcEffect(missile,"contact",missile.bornAt+missile.age);
   }
   return true;
  }

  // Enabled only by the Castle Center source profiles. The bunker shelters
  // an existing bowman; it never manufactures another clear target.
  sealed class CastleSiegeLink {internal Enemy bunker,bow;internal float x,y,deathAge;internal int line;internal bool released;}
  readonly List<CastleSiegeLink> castleSiegeLinks=new List<CastleSiegeLink>();
  Enemy castleRoomIdentity;
  void ResetCastleSiegeLinks(){castleSiegeLinks.Clear();castleRoomIdentity=null;}
  void EnsureCastleSiegeLinks(){
   if(enemies.Count==0){ResetCastleSiegeLinks();return;}
   if(castleRoomIdentity==enemies[0])return;
   ResetCastleSiegeLinks();castleRoomIdentity=enemies[0];
   var assigned=new HashSet<Enemy>();
   foreach(var bunker in enemies){
    var p=NpcProfile(bunker);if(p==null||p.environmentKind!="siege-bunker")continue;
    Enemy nearest=null;float distance=float.PositiveInfinity;
    foreach(var bow in enemies){
     if(bow.sourceNPC!="NUI_SOLDIER_CROSSBOW"||bow.encounterIndex!=bunker.encounterIndex||assigned.Contains(bow))continue;
     float dx=bow.x-bunker.x,dy=bow.y-bunker.y,d=dx*dx+dy*dy;
     if(d<distance){nearest=bow;distance=d;}
    }
    var link=new CastleSiegeLink{bunker=bunker,bow=nearest,x=bunker.x,y=bunker.y,line=bunker.groundLine};castleSiegeLinks.Add(link);
    if(nearest!=null){assigned.Add(nearest);EnterNpcState(nearest,npcStates[nearest.sourceNPC+"/SOLDIER_CROSSBOW_SIEGE_WAIT"]);}
   }
  }
  bool TickNpcCastleCenterSpecial(Enemy enemy,float dt){
   var p=NpcProfile(enemy);if(p==null||p.environmentKind!="siege-bunker"&&p.id!="NUI_SOLDIER_CROSSBOW")return false;
   EnsureCastleSiegeLinks();
   var link=castleSiegeLinks.Find(l=>l.bunker==enemy||l.bow==enemy);
   if(link==null)return false;
   if(enemy==link.bunker){
    if(!enemy.active||enemy.dormant)return true;
    if(enemy.hitPause>0){float frozen=Math.Min(enemy.hitPause,dt);enemy.hitPause=Math.Max(0,enemy.hitPause-frozen);dt-=frozen;if(dt<=.000001f)return true;}
    enemy.x=link.x;enemy.y=link.y;enemy.groundLine=link.line;enemy.vx=enemy.vy=0;
    if(enemy.hp<=0){
     if(!link.released){
      link.deathAge+=dt;
      if(link.deathAge+1e-6f>=.1f){
       link.released=true;
       if(link.bow!=null&&link.bow.hp>0){InterruptNpcAction(link.bow);link.bow.state=CombatState.Normal;EnterNpcState(link.bow,npcStates[link.bow.sourceNPC+"/SOLDIER_CROSSBOW_WAIT"]);TraceNpc(link.bow,"siege-released",-1,0,.1f);}
      }
     }
     var dying=npcStates[p.id+"/"+p.environment.deathState];
     if(enemy.npcState!=dying.id)EnterNpcState(enemy,dying);
     enemy.state=CombatState.Dead;enemy.deathTime+=dt;AdvanceNpcEnvironmentPose(enemy,dying,dt);return true;
    }
    var wait=npcStates[p.id+"/"+p.waitState];
    // Hits interrupt the previous state before this owner ticks. Keep the
    // stationary bunker's original Damage -> motion end -> Wait transition.
    // Generic hitstun can expire before the artwork's 0.467 s have elapsed.
    if(enemy.clip=="hurt"&&enemy.stun>0||enemy.npcState=="BUNKER_DAMAGE"){
     var damage=npcStates[p.id+"/BUNKER_DAMAGE"];
     if(enemy.npcState!=damage.id)EnterNpcState(enemy,damage);
     enemy.clip=damage.clip;enemy.clock=enemy.npcTime;
     AdvanceNpcEnvironmentPose(enemy,damage,dt);
     if(enemy.npcTime+1e-6f>=damage.duration)EnterNpcState(enemy,wait);
     return true;
    }
    if(enemy.npcState!=wait.id)EnterNpcState(enemy,wait);
    if(enemy.stun>0){enemy.clock+=dt;return true;}
    enemy.state=CombatState.Normal;AdvanceNpcEnvironmentPose(enemy,wait,dt);return true;
   }
   if(link.released||enemy.hp<=0||!enemy.active||enemy.dormant)return false;
   if(enemy.hitPause>0){float frozen=Math.Min(enemy.hitPause,dt);enemy.hitPause=Math.Max(0,enemy.hitPause-frozen);dt-=frozen;if(dt<=.000001f)return true;}
   if(enemy.npcState==null||!enemy.npcState.StartsWith("SOLDIER_CROSSBOW_SIEGE_",StringComparison.Ordinal))EnterNpcState(enemy,npcStates[p.id+"/SOLDIER_CROSSBOW_SIEGE_WAIT"]);
   enemy.vx=enemy.vy=0;enemy.state=CombatState.Normal;
   var state=NpcState(enemy);
   if(state.id=="SOLDIER_CROSSBOW_SIEGE_WAIT"){
    enemy.facing=player.x>=enemy.x?1:-1;enemy.npcDecisionElapsed+=dt;
    AdvanceNpcEnvironmentPose(enemy,state,dt);
    if(enemy.npcDecisionElapsed+1e-6f>=state.interval){
     enemy.npcDecisionElapsed%=state.interval;float dx=player.x-enemy.x,dy=player.y-enemy.y;
     foreach(var c in state.choices)if(dx*dx+dy*dy<=c.near*c.near&&player.hp>0&&NpcRoll()<c.rate){EnterNpcState(enemy,npcStates[p.id+"/"+c.nextState]);break;}
    }
   }else{enemy.clock+=dt;TickNpcRule(enemy,dt);}
   return true;
  }
 }
}
