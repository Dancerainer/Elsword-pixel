using System;
using System.Collections.Generic;
namespace ElswordPixel {
 [Serializable] public sealed class GroundSurface {public int id,bungeeTargetPlusOne;[NonSerialized] internal float conveyorSpeed;public float x1,y1,x2,y2;public bool platform,npcOnly;public HamelWaterLine water;public HamelLineTrigger hamelTrigger;public bool initiallyDisabled;[NonSerialized] public bool hamelDisabled;public float Y(float x){return y1+(y2-y1)*(x-x1)/Math.Max(.00001f,x2-x1);}}
 [Serializable] public sealed class GroundTeleport {
  public int fromLine,toLine;
  public float entryX,entryY,exitX,exitY;
 }
 public sealed partial class GameModel {
  int arrivalTeleportLine=-1;
  static GroundTeleport[] CopyGroundTeleports(GroundTeleport[] source,GroundSurface[] surfaces){
   if(source==null||source.Length==0)return null;
   var result=new GroundTeleport[source.Length];
   for(int i=0;i<source.Length;i++){
    var p=source[i];
    if(p==null||!Finite(p.entryX)||!Finite(p.entryY)||!Finite(p.exitX)||!Finite(p.exitY)||
       !Array.Exists(surfaces,s=>s.id==p.fromLine)||!Array.Exists(surfaces,s=>s.id==p.toLine))
     throw new ArgumentException("Ground teleport refers to an invalid surface or coordinate.");
    result[i]=new GroundTeleport{fromLine=p.fromLine,toLine=p.toLine,entryX=p.entryX,entryY=p.entryY,exitX=p.exitX,exitY=p.exitY};
   }
   return result;
  }
  void TryGroundTeleport(float previousX,float target){
   if(previousX==target||!NativeGrounded(player))return;
   var standing=Surface(player.groundLine)??SurfaceBelow(player.x,player.y+.05f);
   if(standing==null)return;
   if(standing.id!=arrivalTeleportLine)arrivalTeleportLine=-1;
   foreach(var p in CurrentRoom.teleports){
    if(p.fromLine!=standing.id||Math.Abs(ConstrainEncounterX(player,p.exitX)-p.exitX)>.05f)continue;
    if(arrivalTeleportLine==standing.id){
     if(Math.Abs(player.x-p.entryX)>18)arrivalTeleportLine=-1;
     return;
    }
    if(Math.Min(previousX,player.x)>p.entryX+6||Math.Max(previousX,player.x)<p.entryX-6)continue;
    var landing=Surface(p.toLine);
    player.x=Clamp(p.exitX,landing.x1,landing.x2);
    player.y=landing.Y(player.x);
    player.vy=0;
    player.groundLine=arrivalTeleportLine=p.toLine;
    return;
   }
  }
  int nativeNavigationRoom=-1,nativeNavigationGoal=-1,nativeNavigationOrigin=-1,nativeNavigationDestination=-1;
  float nativeNavigationExpires;
  public bool NativeTerrain {get{return CurrentRoom.surfaces!=null&&CurrentRoom.surfaces.Length>0;}}
  public GroundSurface Surface(int id){if(CurrentRoom.surfaces!=null)foreach(var s in CurrentRoom.surfaces)if(s.id==id)return s;return null;}
  float NativeClampX(float x,bool includeNpcRoads=false){float lo=float.PositiveInfinity,hi=float.NegativeInfinity;foreach(var s in CurrentRoom.surfaces)if(!s.platform&&(includeNpcRoads||!s.npcOnly)){lo=Math.Min(lo,s.x1);hi=Math.Max(hi,s.x2);}return Clamp(x,lo+.03f,hi-.03f);}
  public GroundSurface SurfaceBelow(float x,float ceiling,bool ignorePlatforms=false,bool includeNpcRoads=false){GroundSurface found=null;float y=float.NegativeInfinity;if(CurrentRoom.surfaces!=null)foreach(var s in CurrentRoom.surfaces){if(s.hamelDisabled||s.npcOnly&&!includeNpcRoads||x<s.x1-.02f||x>s.x2+.02f||ignorePlatforms&&s.platform)continue;float h=s.Y(x);if(h<=ceiling+.02f&&h>y){found=s;y=h;}}return found;}
  static bool UsesNpcRoad(Fighter fighter){return fighter is Enemy enemy && enemy.usesNpcRoad;}
  float NativeFloor(Fighter f){var s=SurfaceBelow(f.x,f.y+.05f,f.dropThrough>0,UsesNpcRoad(f));return s==null?-1000:s.Y(f.x);}
  bool NativeGrounded(Fighter f){var s=SurfaceBelow(f.x,f.y+.05f,f.dropThrough>0,UsesNpcRoad(f));return s!=null&&Math.Abs(f.y-s.Y(f.x))<.06f&&f.vy<=0;}
  void ResolveNativePlayerJoinContactV17(){
   if(player.dropThrough>0||SurfaceBelow(player.x,player.y+.05f)!=null)return;
   // A centre can enter a narrow join while leaving the higher support, then
   // reverse or release direction after groundLine has cleared. Resolve the
   // source-width body's overlap with that tall wall onto the real lower
   // edge. This is horizontal contact correction only: no Y change, new
   // floor, extended span, or recovery from below the lower platform.
   foreach(var lower in CurrentRoom.surfaces){if(lower.hamelDisabled||lower.platform||lower.npcOnly)continue;
    foreach(var upper in CurrentRoom.surfaces){if(upper.hamelDisabled||upper.platform||upper.npcOnly||lower.id==upper.id)continue;
     float gap=upper.x1-lower.x2;
     if(gap>0&&gap<=9.8f&&player.x>lower.x2&&player.x<upper.x1&&upper.y1>lower.y2+7&&player.y>=lower.y2-.02f&&player.y<upper.y1-.02f){player.x=lower.x2-.025f;player.groundLine=lower.id;return;}
     gap=lower.x1-upper.x2;
     if(gap>0&&gap<=9.8f&&player.x>upper.x2&&player.x<lower.x1&&upper.y2>lower.y1+7&&player.y>=lower.y1-.02f&&player.y<upper.y2-.02f){player.x=lower.x1+.025f;player.groundLine=lower.id;return;}
    }
   }
  }
  const float NativePlayerHalfWidth = 9.8f;
  const float NativeWalkStepHeight = 7f;

  // An open curtain can share its authored edge with the face of a higher step.
  bool TouchesNativeCurtainRiser(float curtainX,int direction){
   if(Math.Abs(player.x-curtainX)>NativePlayerHalfWidth+.05f)return false;
   foreach(var surface in CurrentRoom.surfaces){
    if(surface.hamelDisabled||surface.platform||surface.npcOnly)continue;
    float edge=direction>0?surface.x1:surface.x2;
    float top=direction>0?surface.y1:surface.y2;
    if(Math.Abs(edge-curtainX)<=.1f&&top>player.y+NativeWalkStepHeight)return true;
   }
   return false;
  }

  void MoveNativeGround(Fighter f,float target){
   float oldX=f.x,oldY=f.y;TraceNativeMovementV17(f,"move-before",oldX,oldY,target);
   try{MoveNativeGroundCoreV17(f,target);}finally{TraceNativeMovementV17(f,"move-after",oldX,oldY,target);}
  }
  void MoveNativeGroundCoreV17(Fighter f,float target){
   if(object.ReferenceEquals(f,player))ResolveNativePlayerJoinContactV17();
   target=Mine2RailNativeMoveV21(f,ConstrainEncounterX(f,NativeClampX(target,UsesNpcRoad(f))));
   if(ReferenceEquals(f,player))target=ConstrainVoidCrossing(target);
   bool grounded=NativeGrounded(f);float old=f.x;var standing=Surface(f.groundLine);
   if(grounded&&(standing==null||standing.hamelDisabled||standing.npcOnly&&!UsesNpcRoad(f)||old<standing.x1-.03f||old>standing.x2+.03f||Math.Abs(standing.Y(old)-f.y)>.1f))standing=SurfaceBelow(old,f.y+.05f,f.dropThrough>0,UsesNpcRoad(f));
   // Native tall risers are walls until the player's feet clear the top.
   int direction=Math.Sign(target-old);float stepClearance=grounded?NativeWalkStepHeight:.02f;
   bool followPeitaSlope=grounded&&standing!=null&&IsRegionalDungeonRoom(CurrentRoom.id);
   if(direction!=0)foreach(var s in CurrentRoom.surfaces){
    if(s.hamelDisabled||s.platform||s.npcOnly&&!UsesNpcRoad(f))continue;float edge=direction>0?s.x1:s.x2,h=direction>0?s.y1:s.y2;if(h<=f.y+stepClearance)continue;
    // The slope under the leading edge of the body meets the next segment
    // at its exact endpoint. That continuous incline is walkable.
    if(followPeitaSlope&&Math.Abs(edge-(direction>0?standing.x2:standing.x1))<.05f&&
       Math.Abs(h-(direction>0?standing.y2:standing.y1))<.1f)continue;
    // The shallow-step allowance belongs to supported walking. An airborne
    // body below the top must meet the wall before entering its solid span;
    // otherwise a backward/forward skill root can slip6px under the ledge.
    if(direction*(edge-old)<-.001f)continue;
    var lower=SurfaceBelow(edge+direction*.04f,f.y+stepClearance,false,UsesNpcRoad(f));
    if(lower!=null && !(ReferenceEquals(f,player) && Mine2NativeDashGravityV21 &&
       ShortMineRiserOverlap(edge,direction,f.y+stepClearance)))continue;
    // Stop the player's leading body edge at a solid riser, including skill
    // lunges. Stopping only the actor centre left half the body inside walls.
    float contact=object.ReferenceEquals(f,player)?edge-direction*NativePlayerHalfWidth:edge;
    // The source player has a19.6px body. A narrow join before a tall solid
    // riser must stop that body on its original support, even during the
    // lower part of a jump. A wall contact inside the gap is not footing.
    // No support is extended; clearing the wall top still permits the jump,
    // and descending edges and real gaps retain their original behavior.
    float joinReach=object.ReferenceEquals(f,player)?9.8f:1f;
    if(standing!=null&&(grounded||old>=standing.x1-.03f&&old<=standing.x2+.03f)){float end=direction>0?standing.x2:standing.x1,gap=direction*(edge-end);if(gap>.02f&&gap<=joinReach)contact=direction>0?Math.Min(contact,end):Math.Max(contact,end);}
    if(object.ReferenceEquals(f,player))contact=direction>0?Math.Max(old,contact):Math.Min(old,contact);
    if(direction*(contact-old)<0||Math.Abs(contact-old)>Math.Abs(target-old)+.001f)continue;
    float stop=contact-direction*.025f;
    if(object.ReferenceEquals(f,player))stop=direction>0?Math.Max(old,stop):Math.Min(old,stop);
    target=direction>0?Math.Min(target,stop):Math.Max(target,stop);
   }
   f.x=target;
   if(grounded&&standing!=null&&target>=standing.x1-.02f&&target<=standing.x2+.02f){f.y=standing.Y(target);f.groundLine=standing.id;return;}
   // Horizontal movement (including a skill push) happens before gravity.
   // Sweep the entire sole path, even when this tick ends beyond the crest.
   if(target!=old) ResolveUphillContact(f,old,target);
   if(grounded){var next=SurfaceBelow(target,f.y+7,f.dropThrough>0,UsesNpcRoad(f));if(next!=null&&Math.Abs(next.Y(target)-f.y)<=7){f.y=next.Y(target);f.groundLine=next.id;}else{f.groundLine=-1;f.state=CombatState.Airborne;f.vy=Math.Min(0,f.vy);}}
  }
  bool ShortMineRiserOverlap(float edge,int direction,float feet){
   // Overlapping the end of the lower solid by less than half the hero's
   // width closes the side of a step. A deck continuing underneath is a passage.
   var floor=SurfaceBelow(edge+direction*.04f,feet,true);
   if(floor==null)return false;
   float inside=direction*((direction>0?floor.x2:floor.x1)-edge);
   const float halfWidth=9.8f;
   return inside>=0 && inside<=halfWidth && SurfaceBelow(edge+direction*(halfWidth+.04f),feet)==null;
  }
  void ResolveUphillContact(Fighter f,float oldX,float newX){
   bool right=newX>oldX;
   float cursor=oldX,height=f.y;
   while(cursor!=newX) {
    float low=Math.Min(cursor,newX),high=Math.Max(cursor,newX);
    float nearest=float.PositiveInfinity,contactEnd=cursor,contactHeight=height;
    foreach(var surface in CurrentRoom.surfaces){
     if(surface.hamelDisabled||surface.npcOnly&&!UsesNpcRoad(f)||surface.platform&&f.dropThrough>0)continue;
     float left=Math.Max(low,surface.x1),end=Math.Min(high,surface.x2);
     if(end<=left)continue;
     float start=right?left:end,finish=right?end:left;
     float entry=surface.Y(start),exit=surface.Y(finish);
     // Only cross a top face from above. An overhead one-way platform and
     // the far wall of a pit are independent of the current support.
     if(entry>height+.02f || exit<=height || exit<=entry)continue;
     float t=Clamp((height-entry)/(exit-entry),0,1);
     float distance=Math.Abs(start+(finish-start)*t-cursor);
     if(distance>nearest || distance==nearest && exit<=contactHeight)continue;
     nearest=distance;contactEnd=finish;contactHeight=exit;
    }
    if(contactEnd==cursor)break;
    cursor=contactEnd;height=contactHeight;
   }
   if(height<=f.y)return;
   f.y=height;
   if(f.vy>0)return;
   var floor=SurfaceBelow(newX,height+.02f,f.dropThrough>0,UsesNpcRoad(f));
   if(floor!=null && Math.Abs(floor.Y(newX)-height)<=.05f){
    f.groundLine=floor.id;f.vy=0;
   }
  }
  float NativeBodyBlockedStop(float oldX,float oldY,float wantedY,float clipped){
   if(SurfaceBelow(clipped,Math.Max(oldY,wantedY)+.05f)!=null)return clipped;
   var support=SurfaceBelow(oldX,oldY+.05f);if(support==null)return clipped;
   // A monster on the upper lip may block a jumping player's centre inside
   // a narrow source join. Clip that attempted displacement at the existing
   // lower edge instead, so a failed jump can land on its real support.
   if(clipped>oldX&&clipped>support.x2&&clipped-support.x2<=9.8f)return Math.Max(oldX,support.x2-.025f);
   if(clipped<oldX&&clipped<support.x1&&support.x1-clipped<=9.8f)return Math.Min(oldX,support.x1+.025f);
   return clipped;
  }
  internal void SetFactoryConveyorSpeed(int roomIndex,int line,float speed){
   var surface=Array.Find(rooms[roomIndex].surfaces,s=>s.id==line);
   if(surface==null)throw new ArgumentException("Factory conveyor line missing from walkable geometry: "+line);
   surface.conveyorSpeed=speed;
  }
  void IntegrateNativeBody(Fighter f,float dt,bool isPlayer){
   float oldX=f.x,oldY=f.y;TraceNativeMovementV17(f,"physics-before",oldX,oldY,f.x);
   try{IntegrateNativeBodyCoreV17(f,dt,isPlayer);}finally{TraceNativeMovementV17(f,"physics-after",oldX,oldY,f.x);}
  }
  void IntegrateNativeBodyCoreV17(Fighter f,float dt,bool isPlayer){
   // Both Waterway mirrors have FIX_POS in every source state. The left one
   // stands on a platform extending beyond the horizontal walking clamp.
   if(f is Enemy mirror&&IsRegionMirror(mirror)){mirror.x=mirror.spawnX;mirror.vx=mirror.vy=0;return;}
   if(isPlayer&&f.y< -120){RecoverFromNativeFall(f,true);return;}
   if(isPlayer)ResolveNativePlayerJoinContactV17();
   f.dropThrough=Math.Max(0,f.dropThrough-dt);
   if(f.state==CombatState.Knockdown||f.state==CombatState.GettingUp){
    var support=Surface(f.groundLine);
    if(support!=null&&support.conveyorSpeed!=0&&NativeGrounded(f))MoveNativeGround(f,f.x+support.conveyorSpeed*dt);
    return;
   }
   float conveyorSpeed=0;bool grounded=NativeGrounded(f);if(grounded){var s=SurfaceBelow(f.x,f.y+.05f,f.dropThrough>0,UsesNpcRoad(f));conveyorSpeed=s.conveyorSpeed;if(isPlayer)RememberNativeFooting(s);f.y=s.Y(f.x);f.groundLine=s.id;f.vy=0;if(f.knockdownOnLanding&&f.state==CombatState.Airborne){BeginKnockdown(f,isPlayer);return;}if(f.state==CombatState.Airborne){f.state=CombatState.Normal;if(isPlayer)LandPlayer();}}
   int cargoChartBeforeR1479=cargoProjectionChartSerialR1479;float oldX=f.x,oldY=f.y;f.vx=Math.Sign(f.vx)*Math.Max(0,Math.Abs(f.vx)-RegionalAcceleration(f,560,false)*dt);if(isPlayer)MovePlayerAlongGround(f.x+(f.vx*HamelWaterResistance+conveyorSpeed)*dt);else MoveNativeGround(f,f.x+(f.vx+conveyorSpeed)*dt);
   if(!NativeGrounded(f)&&!(isPlayer&&SkillSuppressGravity)){
    float gravity=isPlayer&&airborneDash&&actionTiming==null?takeoffGravity:RegionalAcceleration(f,1120,true);f.vy=Math.Max(-FallSpeedLimit(isPlayer)*(isPlayer?HamelGravityScale:1),f.vy-gravity*dt*(isPlayer?HamelGravityScale:1));float before=f.y;f.y+=f.vy*dt;
    if(f.knockdownOnLanding&&f.hp>0)f.clip=f.vy>0?"launched":"fall";
    if(f.vy<=0){var landingSurface=SurfaceBelow(f.x,before+.05f,f.dropThrough>0,UsesNpcRoad(f));if(landingSurface!=null&&f.y<=landingSurface.Y(f.x)){f.y=landingSurface.Y(f.x);f.vy=0;f.groundLine=landingSurface.id;if(!isPlayer||springDirection==0)SpawnEffect("dust",f.x,f.y);if(f.hp>0&&f.knockdownOnLanding)BeginKnockdown(f,isPlayer);else if(f.hp>0){f.state=CombatState.Normal;if(isPlayer)LandPlayer();}}}
   }
   if(isPlayer&&cargoChartBeforeR1479==cargoProjectionChartSerialR1479)BlockPlayerBodies(oldX,oldY);f.x=ConstrainEncounterX(f,NativeClampX(f.x,UsesNpcRoad(f)));
   // Resolve the portal after body contact. A distant landing has no swept
   // path through the enemies or closed doors between the two source lines.
   if(isPlayer&&CurrentRoom.teleports!=null)TryGroundTeleport(oldX,f.x);
   if(f.y< -120 && (isPlayer || !IsEncounterObjective((Enemy)f))) RecoverFromNativeFall(f,isPlayer);
   if(isPlayer&&actionTiming!=null&&actionTiming.command!=null){if(commandPassiveX>=0)f.vx=commandPassiveX*f.facing;if(commandPassiveY>=0)f.vy=commandPassiveY;}
  }
  void NavigateAgainstGroundMotion(Input input,GroundSurface standing,GroundSurface destination=null){
   if(input.dash||input.move==0||standing==null||!Grounded(player))return;
   int direction=Math.Sign(input.move);
   // The normal input map runs at 60 Hz. Ground drag is applied after setting walking velocity.
   float walk=Math.Max(0,GroundWalkSpeed*Math.Abs(input.move)*SkillMoveMultiplier-RegionalAcceleration(player,560,false)/60f)*HamelWaterResistance;
   float opposing=-direction*standing.conveyorSpeed;
   if(destination!=null&&destination!=standing&&destination.conveyorSpeed*direction<0){
    float entry=direction>0?destination.x1:destination.x2;
    float distance=(entry-player.x)*direction;
    if(distance>=0&&distance<=14&&(input.jump||Math.Abs(destination.Y(entry)-player.y)<=7))
     opposing=Math.Max(opposing,-direction*destination.conveyorSpeed);
   }
   if(opposing>0&&opposing>=walk)input.dash=true;
  }
  public bool NativeApproachEnemy(Input input,Enemy target){
   // Combat approaches aim at a moving body; geometry probes aim at a support
   // point. Preserve the existing close-range attack opportunity while keeping
   // the navigator active for nearby steps/landings in geometry-only travel.
   if(target==null)return false;
   if(Math.Abs(target.x-player.x)<100&&Math.Abs(target.y-player.y)<32){
    // Being beside a target at the top of a jump does not mean its ledge has
    // been reached. Keep steering across the riser until both feet project to
    // the same support; otherwise the combat driver's short stopping distance
    // repeatedly releases direction just before landing on the upper floor.
    var support=SurfaceBelow(player.x,player.y+.1f);var goal=SurfaceBelow(target.x,target.y+.1f);
    if(!NativeTerrain||support==null||goal==null)return false;
    if(support.id==goal.id){NavigateAgainstGroundMotion(input,support);return false;}
    // A Feita cliff can stop the player's feet before the weapon reaches a
    // ranged monster on the upper lip. Let the navigator supply the jump.
    if(Grounded(player) && !(IsRegionalDungeonRoom(CurrentRoom.id)
       && target.y-player.y>7 && Math.Abs(target.x-player.x)>55))return false;
   }
   return NativeNavigate(input,target.x,target.y);
  }
  public bool NativeNavigate(Input input,float targetX,float targetY){
   if(!NativeTerrain)return false;ReconcileCargoProjectionNavigationGoalR1479(ref targetX,ref targetY);var start=SurfaceBelow(player.x,player.y+.1f);var goal=SurfaceBelow(targetX,targetY+.1f);
   if(goal==null&&HasEncounters&&gateOpen&&targetX==CurrentEncounter.exitX){
    // A source room envelope can end just past its final walkable line. Navigate
    // to the same physical contact used by the curtain and room transition.
    targetX=EncounterExitContactX(CurrentEncounter);
    goal=SurfaceBelow(targetX,targetY+.1f);
   }
   bool peitaNavigation=IsRegionalDungeonRoom(CurrentRoom.id);
   // A nearer enemy on another level can become the target mid-jump.
   // Keep the landing chosen at takeoff until the player touches a surface.
   bool landingInProgress=peitaNavigation&&!Grounded(player)&&nativeNavigationRoom==room&&
    nativeNavigationDestination>=0&&elapsed<nativeNavigationExpires;
   // Continue steering to the chosen landing while no support lies beneath a gap.
   if(peitaNavigation&&start==null&&goal!=null&&nativeNavigationRoom==room&&(nativeNavigationGoal==goal.id||landingInProgress)&&elapsed<nativeNavigationExpires)
    start=Surface(nativeNavigationOrigin);
   if(start==null||goal==null)return false;
   if(start.id==goal.id&&!landingInProgress){if(Grounded(player))nativeNavigationDestination=-1;NavigateAgainstGroundMotion(input,start);return false;}
   GroundSurface destination=null,origin=null;
   if(nativeNavigationRoom==room&&(nativeNavigationGoal==goal.id||landingInProgress)&&nativeNavigationDestination>=0&&elapsed<nativeNavigationExpires){
    destination=Surface(nativeNavigationDestination);origin=Surface(nativeNavigationOrigin);
    if(destination!=null&&Grounded(player)&&player.groundLine==destination.id){destination=null;nativeNavigationDestination=-1;}
   }
   if(destination==null){
   var queue=new Queue<int>();var previous=new Dictionary<int,int>();queue.Enqueue(start.id);previous[start.id]=-1;
   while(queue.Count>0&&!previous.ContainsKey(goal.id)){var a=Surface(queue.Dequeue());
    if(CurrentRoom.teleports!=null)foreach(var p in CurrentRoom.teleports){
     if(p.fromLine!=a.id||Surface(p.toLine).hamelDisabled||previous.ContainsKey(p.toLine)||Math.Abs(ConstrainEncounterX(player,p.exitX)-p.exitX)>.05f)continue;
     previous[p.toLine]=a.id;queue.Enqueue(p.toLine);
    }
    foreach(var b in CurrentRoom.surfaces){if(b.hamelDisabled||b.npcOnly||previous.ContainsKey(b.id)||a.id==b.id)continue;if(CurrentRoom.bungeeLineIds!=null&&Array.IndexOf(CurrentRoom.bungeeLineIds,b.id)>=0)continue;if(HasEncounters&&(b.x2<CurrentEncounter.leftX||b.x1>CurrentEncounter.rightX))continue;float left=Math.Max(a.x1,b.x1),right=Math.Min(a.x2,b.x2),gap=left-right;float x=(left+right)*.5f,dy=b.Y(Clamp(x,b.x1,b.x2))-a.Y(Clamp(x,a.x1,a.x2));bool waterJump=CanHamelWaterJump(a,gap,dy);if(((gap>100||dy>78)&&!waterJump&&!Mine2DashRouteReachableV21(gap,dy))||(dy< -300 && !(peitaNavigation && gap>0 && Mine2DashRouteReachableV21(gap,dy))))continue;
    // A tall upward gap has less horizontal airtime than a level jump. Do not
    // skip a real intermediate step merely because both gaps are under 100px.
    if(gap>0&&CurrentRoom.teleports!=null&&HasImpassableWallBetween(a,b,left,right))continue;
    if(gap>0&&dy>7){float flight=(420+(float)Math.Sqrt(Math.Max(0,420*420-2*1120*dy)))/1120;if(gap+26>130*flight&&!waterJump&&!Mine2DashRouteReachableV21(gap,dy))continue;}
    if(dy< -8&&gap<0&&!a.platform&&b.platform)continue;previous[b.id]=a.id;queue.Enqueue(b.id);}}
   if(!previous.ContainsKey(goal.id))return false;int next=goal.id;while(previous[next]!=start.id)next=previous[next];destination=Surface(next);origin=start;
   // Keep the selected physical transition while crossing a short intermediate
   // ramp/platform. Replanning on every crossed line used to choose the reverse
   // edge and oscillate at the original forest's starting ledge.
   nativeNavigationRoom=room;nativeNavigationGoal=goal.id;nativeNavigationOrigin=start.id;nativeNavigationDestination=next;nativeNavigationExpires=elapsed+4;
   }
   if(origin==null)return false;
   if(CurrentRoom.teleports!=null)foreach(var p in CurrentRoom.teleports){
    if(p.fromLine!=origin.id||p.toLine!=destination.id)continue;
    input.jump=input.drop=input.dash=input.attack=input.attackX=false;input.skill=-1;
    input.move=arrivalTeleportLine==origin.id?(player.x<p.entryX?-1:1):(player.x<p.entryX?1:-1);NavigateAgainstGroundMotion(input,start);
    return true;
   }
   float lo=Math.Max(origin.x1,destination.x1),hi=Math.Min(origin.x2,destination.x2),portal=lo<=hi?Clamp(targetX,lo+Math.Min(15,(hi-lo)*.25f),hi-Math.Min(15,(hi-lo)*.25f)):destination.x1>=origin.x2?origin.x2-14:origin.x1+14;
   float delta=destination.Y(Clamp(portal,destination.x1,destination.x2))-origin.Y(Clamp(portal,origin.x1,origin.x2));input.skill=-1;input.attack=input.attackX=false;input.dash=false;
   float landing=Clamp(targetX,destination.x1+Math.Min(12,(destination.x2-destination.x1)*.25f),destination.x2-Math.Min(12,(destination.x2-destination.x1)*.25f));
   if(!Grounded(player)){input.move=Math.Abs(player.x-landing)>5?Math.Sign(landing-player.x):0;return true;}
   bool canDropThrough=peitaNavigation?hi-lo>6:lo<=hi;
   if(delta< -8&&origin.platform&&canDropThrough){if(player.x>=lo+3&&player.x<=hi-3){input.drop=true;input.move=0;}else input.move=Math.Sign(portal-player.x);}
   else if(delta<=7){input.move=Math.Sign(landing-player.x);if(lo>hi+.1f&&player.groundLine==origin.id&&Math.Abs(player.x-portal)<10)input.jump=true;}
   else if(lo<=hi&&player.x>=lo+3&&player.x<=hi-3){input.jump=true;input.move=0;}
   else if(Math.Abs(player.x-portal)>10)input.move=Math.Sign(portal-player.x);
   else{input.jump=true;input.move=Math.Abs(landing-player.x)>5?Math.Sign(landing-player.x):0;}
   NavigateAgainstGroundMotion(input,start,destination);
   if(input.jump&&Mine2NeedsDashRouteV21(Math.Max(0,lo-hi),delta)){input.dash=true;if(input.move==0)input.move=Math.Abs(landing-player.x)>1?Math.Sign(landing-player.x):player.facing;}
   return true;
  }
  bool HasImpassableWallBetween(GroundSurface from,GroundSurface to,float gapEnd,float gapStart){
   float peak=from.Y(Clamp((gapStart+gapEnd)*.5f,from.x1,from.x2))+644f*644/(2*1120f);
   foreach(var wall in CurrentRoom.surfaces){
    if(wall.hamelDisabled||wall.npcOnly||wall.platform||wall==from||wall==to)continue;
    float lo=Math.Max(wall.x1,gapStart),hi=Math.Min(wall.x2,gapEnd);
    if(hi<=lo)continue;
    float probe=(lo+hi)*.5f;
    if(wall.Y(probe)>peak&&SurfaceBelow(probe,peak)==null)return true;
   }
   return false;
  }
 }
}
