using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class AlteraRavenMissileRainRuleV18 {public string npc,state,sourceCallback,sound;public float firstAt,secondAt,spacing,firstCenter,secondCenter,skyHeight,cooldown;public int loopLast;}
 [Serializable] public sealed class AlteraRavenMissileRainEffectV18 {public string kind;public float sourceLifetime,sourceVelocityX,sourceVelocityY,sourceRotateZ,sourceOffsetY;public bool sourceUseLand,sourceApplyRotate;}
 public sealed class AlteraRavenMissileRainReleaseV18 {public Enemy owner;public int volley,index,side;public string kind;public float time,x,y,z,landY,sourceOffset;public bool resolved;}
 public sealed partial class GameModel {
  const string RavenRainIdV18="RAVEN_SPECIAL_ATTACK_MISSILE_RAIN",RavenRainLeftV18="RAVEN_SPECIAL_ATTACK_MISSILE_RAIN_WAVE_LEFT",RavenRainRightV18="RAVEN_SPECIAL_ATTACK_MISSILE_RAIN_WAVE_RIGHT";
  AlteraRavenMissileRainRuleV18 ravenMissileRainRuleV18;
  readonly Dictionary<Enemy,int> ravenRainMaskV18=new Dictionary<Enemy,int>();readonly Dictionary<Enemy,float> ravenRainPreviousV18=new Dictionary<Enemy,float>();readonly Dictionary<Enemy,float> ravenRainSourceMPV18=new Dictionary<Enemy,float>();
  public readonly List<AlteraRavenMissileRainReleaseV18> alteraRavenMissileRainReleasesV18=new List<AlteraRavenMissileRainReleaseV18>();
  static bool RainCloseV18(float a,float b){return Finite(a)&&Math.Abs(a-b)<.0001f;}
  static bool EmptyRainEffectV18(AlteraRavenMissileRainEffectV18 r){return r!=null&&string.IsNullOrEmpty(r.kind)&&r.sourceLifetime==0&&r.sourceVelocityX==0&&r.sourceVelocityY==0&&r.sourceRotateZ==0&&r.sourceOffsetY==0&&!r.sourceUseLand&&!r.sourceApplyRotate;}
  static void ValidateAlteraRavenMissileRainEffectV18(NpcEffectDefinition d){
   var r=d.alteraRavenMissileRain;if(r==null)return;if(EmptyRainEffectV18(r)){d.alteraRavenMissileRain=null;return;}
   bool main=d.id==RavenRainIdV18,left=d.id==RavenRainLeftV18,right=d.id==RavenRainRightV18;
   if(!main&&!left&&!right||r.kind!=(main?"main":left?"left":"right")||r.sourceLifetime!=(main?2:.25f)||r.sourceVelocityX!=(main?0:left?-600:600)||r.sourceVelocityY!=(main?-800:0)||r.sourceRotateZ!=(main?-90:0)||r.sourceOffsetY!=(main?50:0)||r.sourceUseLand!=main||r.sourceApplyRotate||
    d.lifetime!=r.sourceLifetime||d.physical!=0||!RainCloseV18(d.magic,main?.01f:.625f)||d.reaction!=(main?HitReaction.None:HitReaction.Launch)||!RainCloseV18(d.launch,main?0:336)||!RainCloseV18(d.push,main?0:224)||d.repeat||d.reflectable||d.projectilePhysics||d.randomYaw||d.lockOnUntil!=0||d.stopAttacker!=0||d.stopDefender!=0||d.damageFrom!=0||d.damageUntil!=d.lifetime||!string.IsNullOrEmpty(d.contactChild)||!string.IsNullOrEmpty(d.deathChild)||!string.IsNullOrEmpty(d.hitChild)||!string.IsNullOrEmpty(d.visualId)||d.visualFromMesh!=main||d.baseSpheres.Length!=4)throw new ArgumentException("Changed exact Raven MissileRain effect source contract");
   float[] sphere={main?11.19910492f:0,0,0,12.12435565f};for(int i=0;i<4;i++)if(!RainCloseV18(d.baseSpheres[i],sphere[i]))throw new ArgumentException("MissileRain needs original helper geometry");
   if(d.geometrySamples==null||d.geometrySamples.Length!=2)throw new ArgumentException("Missing constant original MissileRain helper ancestry");foreach(var g in d.geometrySamples){if(g.spheres.Length!=4||g.attackLines.Length!=0)throw new ArgumentException("Changed rain helper shape");for(int i=0;i<4;i++)if(!RainCloseV18(g.spheres[i],sphere[i]))throw new ArgumentException("Invented moving MissileRain helper");}
   foreach(var s in d.samples)if(!RainCloseV18(s.scaleX,main?.7f:2)||!RainCloseV18(s.scaleY,s.scaleX)||!RainCloseV18(s.scaleZ,s.scaleX)||!RainCloseV18(s.moveX,s.time*r.sourceVelocityX*.28f))throw new ArgumentException("Changed original MissileRain size/velocity");
   if(main){if(d.deathChildren==null||d.deathChildren.Length!=2)throw new ArgumentException("MissileRain requires exact two original OFFSET_Y children");foreach(string id in new[]{RavenRainLeftV18,RavenRainRightV18}){var row=Array.Find(d.deathChildren,c=>c!=null&&c.effect==id);if(row==null||row.offsetX!=0||row.offsetY!=14||row.offsetZ!=0||row.rotationZ!=0||row.applyParentRotation||row.rotateOffset)throw new ArgumentException("Changed source rain OFFSET_Y50/APPLY_ROTATE false");}}
   else if(d.deathChildren!=null&&d.deathChildren.Length>0)throw new ArgumentException("Original MissileRain waves have no damage children");
  }
  public void SetAlteraRavenMissileRainV18(AlteraRavenMissileRainRuleV18 rule){
   if(rule!=null&&string.IsNullOrEmpty(rule.npc)&&string.IsNullOrEmpty(rule.state)&&string.IsNullOrEmpty(rule.sourceCallback)&&string.IsNullOrEmpty(rule.sound)&&rule.firstAt==0&&rule.secondAt==0&&rule.spacing==0&&rule.firstCenter==0&&rule.secondCenter==0&&rule.skyHeight==0&&rule.cooldown==0&&rule.loopLast==0)rule=null;
   if(rule!=null){NpcStateDefinition state;NpcMechanismStateRule ready;
    if(rule.npc!="NUI_RAVEN"||rule.state!="RAVEN_SP_SPECIAL_ATTACK"||rule.sourceCallback!="RAVEN_SP_SPECIAL_ATTACK_FRAME_MOVE"||rule.sound!="npc-raven_sp_specialattack"||rule.firstAt!=.3f||rule.secondAt!=1.5f||rule.spacing!=800||rule.firstCenter!=200||rule.secondCenter!=-200||rule.skyHeight!=1600||rule.cooldown!=20||rule.loopLast!=3||!npcStates.TryGetValue(rule.npc+"/"+rule.state,out state)||state.sourceAnimation!="SpecialAttackC_Fire"||state.sourceAnimSpeed!=1||!RainCloseV18(state.duration,38f/30)||!npcMechanismRules.TryGetValue(rule.npc+"/RAVEN_SP_SPECIAL_ATTACK_READY",out ready)||!ready.cooldownExplicit||ready.cooldown!=20||!npcSourceSoundAssetsV17.Contains(rule.sound))throw new ArgumentException("Changed source Raven MissileRain callback/cooldown/motion contract");
    var profile=npcDefinitions[rule.npc];foreach(string id in new[]{profile.waitState,profile.walkState}){var choice=Array.Find(npcStates[rule.npc+"/"+id].choices,c=>c.nextState=="RAVEN_SP_SPECIAL_ATTACK_READY");if(choice==null||choice.rate!=25||choice.near!=140||choice.hpLessPercent!=40)throw new ArgumentException("Changed source MissileRain RATE25/HP<40 selection");}
    foreach(string id in new[]{RavenRainIdV18,RavenRainLeftV18,RavenRainRightV18}){NpcEffectDefinition d;if(!npcEffectDefinitions.TryGetValue(id,out d)||d.alteraRavenMissileRain==null)throw new ArgumentException("Missing exact MissileRain effect chain");ValidateAlteraRavenMissileRainEffectV18(d);}
   }
   ravenMissileRainRuleV18=rule;ClearAlteraRavenMissileRainV18();
  }
  void ClearAlteraRavenMissileRainV18(){ravenRainMaskV18.Clear();ravenRainPreviousV18.Clear();ravenRainSourceMPV18.Clear();alteraRavenMissileRainReleasesV18.Clear();}
  public bool TryGetAlteraRavenMissileRainMPV18(Enemy owner,out float mp){return ravenRainSourceMPV18.TryGetValue(owner,out mp);}
  void AddRainReleaseV18(AlteraRavenMissileRainReleaseV18 row){if(alteraRavenMissileRainReleasesV18.Count>=256)alteraRavenMissileRainReleasesV18.RemoveAt(0);alteraRavenMissileRainReleasesV18.Add(row);}
  void AdvanceAlteraRavenMissileRainV18(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   var rule=ravenMissileRainRuleV18;if(rule==null||owner==null||owner.sourceNPC!=rule.npc||state.id!=rule.state||owner.hp<=0||!owner.active||owner.dormant)return;
   int mask;float previous;if(!ravenRainMaskV18.TryGetValue(owner,out mask)||!ravenRainPreviousV18.TryGetValue(owner,out previous)||from+.000001f<previous)mask=0;
   for(int volley=0;volley<2;volley++){
    float marker=(volley==0?rule.firstAt:rule.secondAt)/state.sourceAnimSpeed;if((mask&(1<<volley))!=0||!(from<marker&&marker<=to+.000001f))continue;mask|=1<<volley;float absolute=packetStart+marker-from;
    QueueNpcSound(new NpcTimedSound{id="source-raven-rain/"+volley,sound=rule.sound,sourceName="Raven_SP_SpecialAttack.ogg",sourceField=rule.sourceCallback+".PlaySound_LUA",sourceTime=marker,candidateTime=marker,candidateClock="npc-state-pose-seconds"},owner,0,0,absolute,state.id);
    if(volley==0)ravenRainSourceMPV18[owner]=0;
    var landArgument=new NpcEffectInstance{owner=owner};CaptureNpcSourceLandArgumentV18(landArgument,null,new NpcEffectEmission{sourceLandPositionYArgument=true});
    for(int i=0;i<=rule.loopLast;i++)for(int side=-1;side<=1;side+=2){if(i==0&&side==1)continue;
     float offset=side*rule.spacing*i+(volley==0?rule.firstCenter:rule.secondCenter),x=owner.x+offset*.28f,ceiling=landArgument.sourceLandY+rule.skyHeight*.28f;
     var support=NativeTerrain?SurfaceBelow(x,ceiling):null;bool resolved=!NativeTerrain||support!=null;
     var row=new AlteraRavenMissileRainReleaseV18{owner=owner,volley=volley,index=i,side=side,time=absolute,sourceOffset=offset,x=x,z=0,resolved=resolved,kind=resolved?"source-parent":"source-position-unresolved"};
     if(resolved){row.landY=NativeTerrain?support.Y(x):GroundAt(CurrentRoom,x);row.y=row.landY+rule.skyHeight*.28f;
      var emission=new NpcEffectEmission{effect=RavenRainIdV18,scaleX=1,scaleY=1,scaleZ=1,rotationZ=-90};float bx,by,bz;NpcAttachment(owner,emission,absolute,out bx,out by,out bz);emission.offsetX=(row.x-bx)/owner.facing/owner.visualScale;emission.offsetY=(row.y-by)/owner.visualScale;emission.offsetZ=(row.z-bz)/owner.visualScale;
      var effect=SpawnNpcEffect(owner,npcEffectDefinitions[RavenRainIdV18],emission,absolute);effect.hasSourceLandY=true;effect.sourceLandY=row.landY;
     }
     AddRainReleaseV18(row);
    }
   }
   ravenRainMaskV18[owner]=mask;ravenRainPreviousV18[owner]=to;
  }
  bool TryMoveAlteraRavenMissileRainV18(NpcEffectInstance e,float from,float to){
   var rule=e.definition.alteraRavenMissileRain;if(rule==null||rule.kind!="main")return false;
   if(!e.hasSourceLandY)throw new InvalidOperationException("MissileRain requires its original queried land scalar");
   // Original Velocity(0,-800,0), no gravity, UseLand=true and zero elasticity.
   // The local mesh's visual -90 roll does not become horizontal flight in this
   // bounded2D adapter. Saved emission landY is independent of player elevation.
   e.y=Math.Max(e.sourceLandY,e.sourceLandY+448-224*to);return true;
  }
  bool TryEmitAlteraRavenMissileRainChildrenV18(NpcEffectInstance parent,float at){
   var r=parent.definition.alteraRavenMissileRain;if(r==null||r.kind!="main")return false;
   foreach(string id in new[]{RavenRainLeftV18,RavenRainRightV18}){
    var child=SpawnNpcEffect(parent.owner,npcEffectDefinitions[id],null,at,parent);
    child.x=parent.x;child.y=parent.y+14;child.z=parent.z;child.facing=1;child.dirX=1;child.dirY=0;child.yaw=0;RemoveNpcSourceEffectRotationV18(child);
    if(child.hasSourceLandY)child.sourceLandY=Math.Min(parent.sourceLandY,child.y);
    TraceNpcEffect("source-death-child",child,0,at);
   }return true;
  }
  bool TryContactAlteraRavenMissileRainV18(NpcEffectInstance e,EffectPose pose,float at){
   var d=e.definition;var rule=d.alteraRavenMissileRain;if(rule==null||rule.kind=="main")return false;
   if(e.age<d.damageFrom||e.age>d.damageUntil||at<e.nextPlayerHit)return true;
   float x,y,z,r;NpcEffectSphere(e,pose,0,out x,out y,out z,out r);float px=Clamp(x,player.x-9.8f,player.x+9.8f),py=Clamp(y,player.y,player.y+42),pz=Clamp(z,-9.8f,9.8f);
   if((px-x)*(px-x)+(py-y)*(py-y)+(pz-z)*(pz-z)>r*r)return true;
   bool protectedReaction=PlayerHasSuperArmor||player.state==CombatState.Knockdown||player.state==CombatState.GettingUp;int direction=player.x>e.x?1:-1;
   int dealt=ApplyEnemyAttackToPlayer((int)Math.Round(e.physicalAttack*d.physical+e.magicAttack*d.magic),player.x-direction,d.reaction,d.launch);
   if(dealt>0){e.nextPlayerHit=float.PositiveInfinity;if(!protectedReaction&&player.hp>0){player.vx=direction*d.push;player.vy=d.launch;}TraceNpcEffect("hit-player",e,dealt,at);EmitNpcHitChild(e,at,px,py,pz);ApplyNpcPlayerStatus(d,at);}return true;
  }
 }
}
