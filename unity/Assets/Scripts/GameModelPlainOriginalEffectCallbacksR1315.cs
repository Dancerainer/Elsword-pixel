using System;
using System.Collections.Generic;

namespace ElswordPixel {
 [Serializable] public sealed class PlainOriginalFxCallbackPointR1315 {
  public string npc,state,animation,callback,bone,motionSha256,luaSha256,callbackWordsSha256,projection,actualOriginalPointProofSha256;
  public float at,animationSpeed,unitScale,x,y,z;
 }
 [Serializable] public sealed class PlainOriginalFxCallbackPointDocumentR1315 {
  public string schema;public PlainOriginalFxCallbackPointR1315[] points=new PlainOriginalFxCallbackPointR1315[0];
 }
 public sealed partial class GameModel {
  sealed class PlainCallbackPointR1315 {
   internal readonly string npc,state,animation;internal readonly float at,x,y,z;
   internal PlainCallbackPointR1315(PlainOriginalFxCallbackPointR1315 p){npc=p.npc;state=p.state;animation=p.animation;at=p.at;x=p.x;y=p.y;z=p.z;}
  }
  const string PlainCallbackProjectionR1315="Author2D original event bone: source world minus animated root plus original reference root minus original textured/skinned render floor, times .28*UNIT_SCALE; existing owner facing/visualScale";
  readonly Dictionary<string,PlainCallbackPointR1315> plainCallbackPointsR1315=new Dictionary<string,PlainCallbackPointR1315>(StringComparer.Ordinal);
  readonly Dictionary<string,NpcEffectDefinition> plainCallbackEffectsR1315=new Dictionary<string,NpcEffectDefinition>(StringComparer.Ordinal);
  NpcDefinition[] plainCallbackProfilesR1315;string plainCallbackRouteR1315;
  int plainCallbackSpawnCountR1315;
  public int PlainOriginalCallbackPointCountR1315{get{return PlainCallbackScopeR1315()?plainCallbackPointsR1315.Count:0;}}
  public int PlainOriginalCallbackSpawnCountR1315{get{return plainCallbackSpawnCountR1315;}}
  bool PlainCallbackScopeR1315(){
   if(plainCallbackProfilesR1315==null||!ReferenceEquals(plainCallbackProfilesR1315,plainDefinitionsR1258)||plainCallbackRouteR1315!=plainRouteR1258||!PlainOriginalFxScopeR1304())return false;
   foreach(var row in plainCallbackEffectsR1315){NpcEffectDefinition current;if(!npcEffectDefinitions.TryGetValue(row.Key,out current)||!ReferenceEquals(row.Value,current))return false;}return true;
  }
  void ClearPlainOriginalFxCallbacksR1315(){plainCallbackPointsR1315.Clear();plainCallbackEffectsR1315.Clear();plainCallbackProfilesR1315=null;plainCallbackRouteR1315=null;plainCallbackSpawnCountR1315=0;}
  // A future resource owner must authenticate the actual original-bone data
  // JSON bytes before deserializing these typed rows. This source does not
  // invent missing points and does not claim to hash external JSON bytes.
  public void BindPlainOriginalFxCallbacksR1315(string route,string rulesSHA,PlainOriginalFxCallbackPointR1315[] originalPoints){
   if(route!=plainRouteR1258||rulesSHA!=PlainFxRulesR1304||!PlainOriginalFxScopeR1304()||originalPoints==null||originalPoints.Length!=3)throw new ArgumentException("Exact installed Plain effects and three actual original callback points required");
   var selected=new Dictionary<string,PlainCallbackPointR1315>(StringComparer.Ordinal);var effects=new Dictionary<string,NpcEffectDefinition>(StringComparer.Ordinal);var seen=new HashSet<string>(StringComparer.Ordinal);
   foreach(var p in originalPoints){
    if(p==null||!Finite(p.x)||!Finite(p.y)||!Finite(p.z)||p.projection!=PlainCallbackProjectionR1315||p.callbackWordsSha256!="76992415bccd68861b683a6b54a7d43aeae6b573704d621c22a5495119af700d")throw new ArgumentException("Changed original callback point projection/source identity");
    bool drill=p.npc=="NUI_NASOD_DRILLER_CANNON";bool watch=p.npc=="NUI_NASOD_WATCH";
    string motion=drill?"e6497edaea43dffe6bdd9a90eedd565df2ae2f71f23e01fe6db1dfb9f6d1feaa":"7fdf655ac83a38104a2aa93bbc3cd42c6c05489ff3435a0d1f1d4b3c72f0b758";
    string lua=drill?"fc1cae6e7f7390c7d7af52dbf8b576e52c6e703924b3efdabac5649588e8879a":"ff045cd8cfd9d0c022d2abbf85d3ca3d6c38f5cd7e7bf2e241a65f7f4e2b31db";
    bool state=drill?p.state=="NASOD_DRILLER_CANNON_ATTACK_C":watch&&(p.state=="NASOD_WATCH_ATTACK"||p.state=="NASOD_WATCH_THUNDERBALL_ATTACK");
    if(p.actualOriginalPointProofSha256!="4b89ef2dc47057faacf29d19fb24ad02cbe4af5efc7af87964321bfd0c68a81b"||p.x!=(drill?50.45202537883252f:9.213827938757357f)||p.y!=(drill?133.47707210776892f:32.69967195155445f)||p.z!=(drill?3.1658458927331394f:.05488582390753629f))throw new ArgumentException("Changed actual original KSM source-to-scene callback point");
    string key=p.npc+"/"+p.state;
    if(!state||!seen.Add(key)||p.callback!=p.state+"_FRAME_MOVE"||p.animation!=(drill?"AttackC_Fire":"AttackA_Fire")||p.bone!=(drill?"Dummy2_Cannon":"Dummy1_Eye")||p.motionSha256!=motion||p.luaSha256!=lua||p.animationSpeed!=1||p.at!=(drill?.05f:.001f)||p.unitScale!=(drill?1.45f:1))throw new ArgumentException("Changed exact original Watch/Driller callback/state/bone/time");
    var owner=Array.Find(plainDefinitionsR1258,q=>q.id==p.npc);if(owner==null)continue;
    var s=Array.Find(owner.states,q=>q.id==p.state);NpcEffectBinding binding;
    if(s==null||s.loop||s.sourceAnimation!=p.animation||s.sourceAnimSpeed!=1||s.duration!=(drill?49f/30f:1.1f)||!npcEffectBindings.TryGetValue(key,out binding)||binding.unitScale!=p.unitScale)throw new ArgumentException("Changed installed original callback state/binding clock");
    string effect=drill?PlainDrillerLeftR1304:p.state=="NASOD_WATCH_ATTACK"?PlainWatchR1304:"DamageEffect_NASOD_WATCH_THUNDERBALL_ATTACK";
    if(!npcEffectDefinitions.ContainsKey(effect)||drill&&!npcEffectDefinitions.ContainsKey(PlainDrillerRightR1304))throw new ArgumentException("Original callback effect definition is absent");
    if(!effects.ContainsKey(effect))effects.Add(effect,npcEffectDefinitions[effect]);
    if(drill&&!effects.ContainsKey(PlainDrillerRightR1304))effects.Add(PlainDrillerRightR1304,npcEffectDefinitions[PlainDrillerRightR1304]);
    selected.Add(key,new PlainCallbackPointR1315(p));
   }
   if(seen.Count!=3||selected.Count!=(route==PlainRecycleR1258?3:2))throw new ArgumentException("Incomplete exact original callback cohort");
   ClearPlainOriginalFxCallbacksR1315();plainCallbackProfilesR1315=plainDefinitionsR1258;plainCallbackRouteR1315=route;foreach(var p in selected)plainCallbackPointsR1315.Add(p.Key,p.Value);foreach(var e in effects)plainCallbackEffectsR1315.Add(e.Key,e.Value);
  }
  void AdvancePlainOriginalEffectCallbacksR1315(Enemy enemy,NpcStateDefinition state,float from,float to,float packetStart){
   if(!PlainCallbackScopeR1315()||enemy==null||state==null||enemy.hp<=0)return;
   var profile=NpcProfile(enemy);if(profile==null||!Array.Exists(plainCallbackProfilesR1315,p=>ReferenceEquals(p,profile))||!Array.Exists(profile.states,s=>ReferenceEquals(s,state)))return;
   PlainCallbackPointR1315 p;if(!plainCallbackPointsR1315.TryGetValue(enemy.sourceNPC+"/"+state.id,out p)||from>=p.at||to<p.at)return;
   // Three original ONE_WAIT callbacks use actual animation speed 1. Each
   // monotonic state packet crosses its literal event at most once. No cursor
   // or fake EFFECT_SET is added, and interrupted states emit no later event.
   bool drill=enemy.sourceNPC=="NUI_NASOD_DRILLER_CANNON";
   string id=drill?(enemy.facing>0?PlainDrillerRightR1304:PlainDrillerLeftR1304):state.id=="NASOD_WATCH_ATTACK"?PlainWatchR1304:"DamageEffect_NASOD_WATCH_THUNDERBALL_ATTACK";
   float at=packetStart+Math.Max(0,p.at-from);
   var emission=new NpcEffectEmission{effect=id,sourcePlainCallbackStateR1315=state.id,start=p.at,offsetX=p.x,offsetY=p.y,offsetZ=p.z,rotationZ=drill?25:0,sourceLandPositionYArgument=true};
   SpawnNpcEffect(enemy,npcEffectDefinitions[id],emission,at);plainCallbackSpawnCountR1315++;
  }
  bool PlainDrillerCallbackBallisticR1315(NpcEffectInstance e){
   if(!PlainCallbackScopeR1315()||e==null||e.owner==null||e.owner.sourceNPC!="NUI_NASOD_DRILLER_CANNON"||e.emission==null||e.emission.sourcePlainCallbackStateR1315!="NASOD_DRILLER_CANNON_ATTACK_C"||!e.emission.sourceLandPositionYArgument||e.emission.rotationZ!=25||(e.id!=PlainDrillerLeftR1304&&e.id!=PlainDrillerRightR1304)||PlainOriginalFxBindingR1304(e)==null)return false;
   return true;
  }
  bool TryApplyPlainDrillerSourceRotationR1315(NpcEffectInstance e,float degrees){
   if(!PlainDrillerCallbackBallisticR1315(e))return false;
   if(degrees!=25)throw new ArgumentException("Changed original Driller callback planar angle");
   float angle=degrees*(float)Math.PI/180;e.dirX=e.facing*(float)Math.Cos(angle);e.dirY=e.facing*(float)Math.Sin(angle);npcSourceEffectRotationsV18[e]=angle;return true;
  }
  bool TryInitializePlainDrillerBallisticR1315(NpcEffectInstance e){
   if(!PlainDrillerCallbackBallisticR1315(e))return false;
   var d=e.definition;if(!d.projectilePhysics||d.projectileGravityY!=-560||d.projectileBounceX!=0||d.projectileBounceY!=.5f||d.lifetime!=1||d.projectileVelocities==null||d.projectileVelocities.Length!=2)throw new ArgumentException("Changed exact original Driller-C finite ballistic template");
   for(int i=0;i<2;i++){var v=d.projectileVelocities[i];if(v==null||v.time!=i||v.x!=336||v.y!=0||v.reset)throw new ArgumentException("Changed original Driller initial constant velocity");}
   // Explicit finite Author2D convention: apply the existing world-XY source
   // angle convention to initial velocity and helper orientation, preserve
   // literal world Y gravity and existing UseLand/bounce rule. Original full
   // template 3D rotation/owner ABI equality is not claimed.
   e.projectileVx=e.dirX*336;e.projectileVy=e.dirY*336;return true;
  }
  bool TryMovePlainDrillerBallisticR1315(NpcEffectInstance e,float from,float to){
   if(!PlainDrillerCallbackBallisticR1315(e))return false;
   float dt=to-from;if(dt<=0)return true;float oldY=e.y,vy=e.projectileVy;
   e.projectileVy+=e.definition.projectileGravityY*dt;e.x+=e.projectileVx*dt;e.y+=(vy+e.projectileVy)*.5f*dt;
   var surface=NativeTerrain?SurfaceBelow(e.x,oldY+.05f):null;float floor=NativeTerrain?(surface==null?float.NegativeInfinity:surface.Y(e.x)):GroundAt(CurrentRoom,e.x);
   if(e.projectileVy<=0&&oldY>=floor-.05f&&e.y<=floor){e.y=floor;e.projectileVy=-e.projectileVy*e.definition.projectileBounceY;e.projectileHorizontalRetention*=e.definition.projectileBounceX;e.projectileVx*=e.definition.projectileBounceX;if(e.projectileVy<.5f)e.projectileVy=0;TraceNpcEffect("projectile-bounce",e,0,e.bornAt+to);}
   return true;
  }
 }
}
