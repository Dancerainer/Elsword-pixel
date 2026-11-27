using System;
using System.Collections.Generic;
namespace ElswordPixel {
 public struct PlainMineSourcePositionR1269 {
  public float x,y,z;public PlainMineSourcePositionR1269(float x,float y,float z){this.x=x;this.y=y;this.z=z;}
 }
 public sealed partial class GameModel {
  sealed class PlainMineServiceActorR1269 {
   internal readonly int[] ints=new int[10]; // Original Init3252bdb0 clears 663c+4*i, i=0..9.
   internal float previous,current,highWater,decision;internal string state;
   internal bool shown=true,deathCallback,ended;internal IPlainMineMajorSequenceR1302 major0;internal IPlainMineMinorSequenceR1302 dieSequence;
   internal int GetInt(int index){if(index<0||index>=ints.Length)throw new ArgumentOutOfRangeException("index");return ints[index];}
   internal void SetInt(int index,int value){if(index<0||index>=ints.Length)throw new ArgumentOutOfRangeException("index");ints[index]=value;}
   internal void ObserveAnimation(float from,float to){previous=from;current=to;highWater=Math.Max(highWater,from);}
   internal void ResetAnimation(string id){state=id;previous=current=highWater=decision=0;}
   internal bool AnimTimer(float at){return previous<at&&at<=current;}
   internal bool AnimEventTimer(float at){return at>highWater&&AnimTimer(at);}
  }
  readonly Dictionary<Enemy,PlainMineServiceActorR1269> plainMineServicesR1269=new Dictionary<Enemy,PlainMineServiceActorR1269>();
  Func<Fighter,PlainMineSourcePositionR1269> plainMineSourcePositionR1269;Func<Enemy,float> plainMineSourceLandYR1269;
  public bool PlainMineOriginalSpatialBoundR1269 {get{return plainMineSourcePositionR1269!=null&&plainMineSourceLandYR1269!=null;}}
  public bool PlainMineOriginalReadyImplementedR1269 {get{var p=plainMineMajorR1302 as IPlainMinePresentationR1302;return PlainMineOriginalSpatialBoundR1269&&p!=null&&p.StateParticlesBoundR1302;}}
  public void SetPlainMineOriginalSpatialR1269(Func<Fighter,PlainMineSourcePositionR1269> position,Func<Enemy,float> landY){
   if(plainRouteR1258!=PlainRecycleR1258||!PlainOriginalRoomScopeR1258(plainRouteR1258)||plainDefinitionsR1258==null||position==null||landY==null)throw new ArgumentException("Unbound exact original Plain Recycle spatial context");
   plainMineSourcePositionR1269=position;plainMineSourceLandYR1269=landY;
  }
  void ClearPlainMineOriginalServicesR1269(){try{ClearPlainMineMajorConsumerR1302();}finally{plainMineServicesR1269.Clear();plainMineSourcePositionR1269=null;plainMineSourceLandYR1269=null;}}
  static void RequirePlainMineSourcePositionR1269(PlainMineSourcePositionR1269 p){if(!Finite(p.x)||!Finite(p.y)||!Finite(p.z))throw new InvalidOperationException("Nonfinite original Mine source XYZ");}
  static float PlainMineOriginalDistanceR1269(PlainMineSourcePositionR1269 a,PlainMineSourcePositionR1269 b){
   RequirePlainMineSourcePositionR1269(a);RequirePlainMineSourcePositionR1269(b);float x=b.x-a.x,y=b.y-a.y,z=b.z-a.z;float squared=(float)((double)x*x+(double)y*y+(double)z*z);
   if(!Finite(squared)||squared<0)throw new InvalidOperationException("Nonfinite original Mine source distance");return(float)Math.Sqrt(squared);
  }
  bool PlainMineOriginalReadyConditionR1269(Enemy e){
   var mine=plainMineSourcePositionR1269(e);var user=plainMineSourcePositionR1269(player);float landY=plainMineSourceLandYR1269(e);if(!Finite(landY))throw new InvalidOperationException("Nonfinite original Mine land Y");
   return PlainMineOriginalDistanceR1269(mine,user)<=100&&user.y<=landY+3;
  }
  bool PlainMineScopeR1302(Enemy e,NpcDefinition p){return plainRouteR1258==PlainRecycleR1258&&PlainOriginalRoomScopeR1258(plainRouteR1258)&&e!=null&&p!=null&&p.id==PlainMineR1258&&plainDefinitionsR1258!=null&&Array.Exists(plainDefinitionsR1258,d=>ReferenceEquals(d,p));}
  public void ApplyPlainMineSourceVisibilityR1302(Enemy e,ref bool visible,ref bool gauge){
   if(e!=null&&departedSoloElementsV45.TryGetValue(e,out _)){visible=false;gauge=false;return;}
   if(!PlainMineScopeR1302(e,NpcProfile(e)))return;PlainMineServiceActorR1269 a;if(plainMineServicesR1269.TryGetValue(e,out a)&&!a.shown){visible=false;gauge=false;}
  }
  void EnterPlainMineStateR1302(Enemy e,PlainMineServiceActorR1269 a,string id){
   NpcStateDefinition next;if(!npcStates.TryGetValue(PlainMineR1258+"/"+id,out next))throw new InvalidOperationException("Original Mine state missing");
   EnterNpcState(e,next);a.ResetAnimation(id);if(id=="ALTERA_MINE_READY_EXPLODE")a.SetInt(0,0); // Original STATE_START only.
  }
  float PlainMineEventAtR1302(PlainMineServiceActorR1269 a,float threshold,float dt){return elapsed-dt+Math.Max(0,threshold-a.previous);}
  void PlainMineBeepR1302(Enemy e,float at){RequirePlainMinePresentationR1302().PlayOriginalSound(e,"MineCountDown.ogg",plainMineSourcePositionR1269(e),at);}
  void PlainMineDeathStartR1302(Enemy e,PlainMineServiceActorR1269 a){
   if(a.deathCallback||!Grounded(e))return;var consumer=RequirePlainMinePresentationR1302();var position=plainMineSourcePositionR1269(e);var land=position;land.y=plainMineSourceLandYR1269(e);position.y+=100;
   // Original DYING_LAND: nullable Create -> SetLandPosition -> SetDieSeq;
   // the original PlaySound is outside that nullable branch.
   var sequence=consumer.CreateMinor(e,"DieLight",position,-1,-1,3,-1,elapsed);
   if(sequence!=null){sequence.SetLandPosition(land);a.dieSequence=sequence;}a.deathCallback=true;
   consumer.PlayOriginalSound(e,"DieLight.ogg",plainMineSourcePositionR1269(e),elapsed);
  }
  void PlainMineExplosionR1302(Enemy e,float at){
   ValidatePlainMineExplosionR1302();RequirePlainMineSourcePositionR1269(plainMineSourcePositionR1269(e));float sourceLand=plainMineSourceLandYR1269(e);if(!Finite(sourceLand))throw new InvalidOperationException("Original Mine explosion land unavailable");
   // Existing finite effect projection takes logical station/Y plus nativeDepth;
   // it must not receive raw source XYZ as if those were logical coordinates.
   var effect=SpawnNpcEffect(e,npcEffectDefinitions["MINE_EXPLOSION"],new NpcEffectEmission(),at);var surface=NativeTerrain?Surface(e.groundLine):null;if(surface==null&&NativeTerrain)surface=SurfaceBelow(e.x,e.y+.05f);
   if(NativeTerrain&&surface==null)throw new InvalidOperationException("Original Mine explosion lacks current supporting surface");
   effect.hasSourceLandY=true;effect.sourceLandY=surface==null?GroundAt(e.x):surface.Y(e.x);
  }
  bool TickPlainMineOriginalWaitR1269(Enemy e,NpcDefinition p,float dt){
   if(!PlainMineScopeR1302(e,p))return false;if(e.sourceNPC!=PlainMineR1258||!enemies.Contains(e))throw new InvalidOperationException("Foreign actor in original Plain Mine consumer");
   if(!e.active||e.dormant)return true;if(!Finite(dt)||dt<=0||dt>.05f)throw new InvalidOperationException("Mine requires the original bounded Step packet");
   if(!PlainMineOriginalSpatialBoundR1269)throw new InvalidOperationException("Original Mine scene XYZ/land bridge is not bound");
   PlainMineServiceActorR1269 a;if(!plainMineServicesR1269.TryGetValue(e,out a)){
    var stale=new List<Enemy>();foreach(var old in plainMineServicesR1269.Keys)if(!enemies.Contains(old))stale.Add(old);foreach(var old in stale)plainMineServicesR1269.Remove(old);a=new PlainMineServiceActorR1269();plainMineServicesR1269.Add(e,a);
   }
   if(e.hp<=0){PlainMineDeathStartR1302(e,a);e.clock+=dt;e.deathTime+=dt;return true;}
   // Keep the existing reaction/integration branch for actual interruptions.
   if(e.stun>0||e.state==CombatState.Hurt||e.state==CombatState.Knockdown||e.state==CombatState.GettingUp||e.knockdownOnLanding)return false;
   if(e.hitPause>0){float frozen=Math.Min(dt,e.hitPause);e.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
   var state=NpcState(e);if(state==null){EnterPlainMineStateR1302(e,a,"ALTERA_MINE_WAIT");state=NpcState(e);}
   bool wait=state.id=="ALTERA_MINE_WAIT",ready=state.id=="ALTERA_MINE_READY_EXPLODE",explode=state.id=="ALTERA_MINE_EXPLODE";
   if(!(wait||ready||explode)||state.sourceAnimation!="Wait"||state.sourceAnimSpeed!=1||Math.Abs(state.duration-34f/30)>.00001f||state.walkSpeed!=0||state.loop==explode)throw new InvalidOperationException("Changed original Mine three-state motion");
   if(a.state!=state.id){a.ResetAnimation(state.id);if(ready)a.SetInt(0,0);}
   if(wait){a.decision+=dt;if(a.decision>=state.interval){a.decision%=state.interval;if(PlainMineOriginalReadyConditionR1269(e)){RequirePlainMinePresentationR1302();EnterPlainMineStateR1302(e,a,"ALTERA_MINE_READY_EXPLODE");return true;}}}
   float previous=e.npcTime,current=state.loop?(previous+dt)%state.duration:Math.Min(state.duration,previous+dt);a.ObserveAnimation(previous,current);AdvanceNpcEnvironmentPose(e,state,dt);
   if(wait){if(a.AnimTimer(.8f)){if(a.GetInt(1)<1)a.SetInt(1,a.GetInt(1)+1);else{if(plainMineMajorR1302==null)throw new InvalidOperationException("Original Mine Major consumer not bound");CreatePlainMineOriginalLightR1302(e,PlainMineEventAtR1302(a,.8f,dt));a.SetInt(1,0);}}return true;}
   if(ready){
    if(a.major0!=null){if(plainMineMajorR1302!=null&&a.major0.IsLive){var pos=plainMineSourcePositionR1269(e);pos.y+=100;a.major0.SetPosition(pos);}else a.major0=null;}
    if(a.AnimEventTimer(.01f)&&a.GetInt(0)==0){var at=PlainMineEventAtR1302(a,.01f,dt);var pos=plainMineSourcePositionR1269(e);pos.y+=100;var consumer=RequirePlainMinePresentationR1302();PlainMineBeepR1302(e,at);var created=consumer.Create(e,"Mine_Countdown",pos,-1,-1,-1,-1,plainMineSourceLandYR1269(e),at);if(created!=null)a.major0=created;}
    bool half=a.AnimTimer(.5f),one=a.AnimEventTimer(1);if(half||one){float at=PlainMineEventAtR1302(a,half?.5f:1,dt);if(plainMineMajorR1302!=null)CreatePlainMineOriginalLightR1302(e,at);int count=a.GetInt(0);
     if(count==0){PlainMineBeepR1302(e,at);if(a.major0!=null&&a.major0.IsLive)a.major0.ChangeTexture("PC_2.dds");a.SetInt(0,1);}
     else if(count==1){PlainMineBeepR1302(e,at);if(a.major0!=null&&a.major0.IsLive)a.major0.ChangeTexture("PC_1.dds");a.SetInt(0,2);EnterPlainMineStateR1302(e,a,"ALTERA_MINE_EXPLODE");}
     else{a.SetInt(0,3);EnterPlainMineStateR1302(e,a,"ALTERA_MINE_EXPLODE");}
    }return true;
   }
   if(a.AnimEventTimer(.7f)&&a.major0!=null){a.major0.DestroyInstance();a.major0=null;}
   // Original state PARTICLE_SEQ and original frame callback are dispatched
   // in source-time order. Equal .8 uses the finite state-visual-before-frame
   // convention, without claiming an independently measured native ordering.
   for(int i=0;i<4;i++){float when=i==0?.78f:i==1?.79f:i==2?.8f:.81f;if(!a.AnimEventTimer(when))continue;
    RequirePlainMinePresentationR1302().CreateStateParticleR1302(e,"Meteo_Explosion0"+(i+1),plainMineSourcePositionR1269(e),plainMineSourceLandYR1269(e),PlainMineEventAtR1302(a,when,dt));
    if(i==2){a.major0=null;PlainMineExplosionR1302(e,PlainMineEventAtR1302(a,.8f,dt));a.shown=false;}
   }
   if(!a.ended&&e.npcTime>=state.duration){
    // Exact source EXPLODE declares DYING_END=true. This is its finite 2D
    // self-destruction, not a change to authored HP, attacks or damage values.
    a.ended=true;e.hp=0;e.state=CombatState.Dead;e.clip="death";e.clock=0;e.stun=0;PlainMineDeathStartR1302(e,a);
   }return true;
  }
 }
}
