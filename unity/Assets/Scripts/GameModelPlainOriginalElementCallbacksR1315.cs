using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  sealed class PlainMeteorRunR1315 {internal NpcStateDefinition state;internal float lastTo;internal readonly float[] x=new float[4],y=new float[4],z=new float[4];internal readonly int[] slot=new int[4];}
  readonly Dictionary<Enemy,PlainMeteorRunR1315> plainMeteorRunsR1315=new Dictionary<Enemy,PlainMeteorRunR1315>();
  readonly Dictionary<string,NpcEffectDefinition> plainElementEffectsR1315=new Dictionary<string,NpcEffectDefinition>(StringComparer.Ordinal);
  NpcDefinition[] plainElementProfilesR1315;string plainElementRouteR1315;uint plainElementRandomR1315=0x1315f17u;
  int plainElementStateCountR1315,plainElementSpawnCountR1315,plainElementNearestSurfaceFallbacksR1315;
  const string PlainMeteor01R1315="DamageEffect_NASOD_ELEMENT_FIRE_SOLO_SPECIAL_ATTACK_01";
  const string PlainHellSideR1315="NASOD_ELEMENT_ICE_HELLSTONE_SIDE";
  public int PlainOriginalRegisteredElementCallbacksR1315{get{return PlainElementCallbackScopeR1315()?plainElementStateCountR1315:0;}}
  public int PlainOriginalElementSpawnCountR1315{get{return plainElementSpawnCountR1315;}}
  public int PlainOriginalElementNearestSurfaceFallbacksR1315{get{return plainElementNearestSurfaceFallbacksR1315;}}
  public bool PlainOriginalUnregisteredIceWallAutomaticallyCalledR1315{get{return false;}}
  // Only the current route's real registered damage consumers are bound.
  // This is a read-only configuration predicate, not runtime/native/art proof.
  public bool PlainOriginalRegisteredFxRouteReadyR1315{get{
   if(!PlainCallbackScopeR1315()||!PlainElementCallbackScopeR1315())return false;
   bool normal=plainRouteR1258==PlainNormalR1258,recycle=plainRouteR1258==PlainRecycleR1258;
   if(!normal&&!recycle||plainCallbackPointsR1315.Count!=(normal?2:3)||plainElementStateCountR1315!=(normal?4:0))return false;
   string[] required=normal?new[]{PlainWatchR1304,PlainMeteoR1304,PlainHellR1304}:new[]{PlainWatchR1304,PlainDrillerLeftR1304,PlainDrillerRightR1304};
   foreach(string id in required){PlainOriginalFxBindingR1304 binding;NpcEffectDefinition current;if(!plainFxBindingsR1304.TryGetValue(id,out binding)||!npcEffectDefinitions.TryGetValue(id,out current)||!ReferenceEquals(binding.definition,current))return false;}
   return true;
  }}

  bool PlainElementCallbackScopeR1315(){
   if(plainElementProfilesR1315==null||!ReferenceEquals(plainElementProfilesR1315,plainDefinitionsR1258)||plainElementRouteR1315!=plainRouteR1258||!PlainOriginalFxScopeR1304())return false;
   foreach(var row in plainElementEffectsR1315){NpcEffectDefinition current;if(!npcEffectDefinitions.TryGetValue(row.Key,out current)||!ReferenceEquals(current,row.Value))return false;}return true;
  }
  void ClearPlainOriginalElementCallbackWorldR1315(){ClearPlainIceRecoveryModelV35();plainMeteorRunsR1315.Clear();plainElementRandomR1315=0x1315f17u;plainElementSpawnCountR1315=plainElementNearestSurfaceFallbacksR1315=0;}
  void ClearPlainOriginalElementCallbacksR1315(){ClearPlainOriginalElementCallbackWorldR1315();plainElementEffectsR1315.Clear();plainElementProfilesR1315=null;plainElementRouteR1315=null;plainElementStateCountR1315=0;}
  public void BindPlainOriginalElementCallbacksR1315(string route,string rulesSHA){
   if(route!=plainRouteR1258||rulesSHA!=PlainFxRulesR1304||!PlainOriginalFxScopeR1304()||!NativeTerrain)throw new ArgumentException("Exact installed original Plain profiles/effects/surfaces required");
   var effects=new Dictionary<string,NpcEffectDefinition>(StringComparer.Ordinal);foreach(string id in new[]{PlainMeteor01R1315,PlainMeteoR1304,PlainHellR1304,PlainHellSideR1315}){NpcEffectDefinition d;if(!npcEffectDefinitions.TryGetValue(id,out d))throw new ArgumentException("Missing original registered Plain element callback effect "+id);effects.Add(id,d);}
   int count=0;foreach(var profile in plainDefinitionsR1258){
    bool fire=profile.id=="NUI_NASOD_ELEMENT_FIRE"||profile.id=="NUI_NASOD_ELEMENT_FIRE_SOLO",ice=profile.id=="NUI_NASOD_ELEMENT_ICE"||profile.id=="NUI_NASOD_ELEMENT_ICE_SOLO";if(!fire&&!ice)continue;
    string state=profile.id.Substring(4)+(fire?"_SPECIAL_ATTACK":"_ATTACK_B");var s=Array.Find(profile.states,q=>q.id==state);NpcEffectBinding binding;
    if(s==null||s.loop||s.sourceAnimSpeed!=1||s.sourceAnimation!=(fire?"SpecialAttackA_Fire":"AttackB_Fire")||!npcEffectBindings.TryGetValue(profile.id+"/"+state,out binding)||binding.unitScale!=1.05f)throw new ArgumentException("Changed exact original registered element callback state/animation/scale");count++;
   }
   if(count!=(route==PlainNormalR1258?4:0))throw new ArgumentException("Changed original Plain element profile cohort");
   ClearPlainOriginalElementCallbacksR1315();plainElementProfilesR1315=plainDefinitionsR1258;plainElementRouteR1315=route;plainElementStateCountR1315=count;foreach(var row in effects)plainElementEffectsR1315.Add(row.Key,row.Value);
  }
  void PlainOriginalCallbackOriginR1315(Enemy owner,float at,out float x,out float y,out float z){NpcAttachment(owner,new NpcEffectEmission(),at,out x,out y,out z);}
  NpcEffectInstance SpawnPlainOriginalEffectAtR1315(Enemy owner,string id,float x,float y,float z,float at,float rotationZ=0){
   NpcEffectDefinition d;if(!PlainElementCallbackScopeR1315()||owner==null||!plainElementEffectsR1315.TryGetValue(id,out d)||!Finite(x)||!Finite(y)||!Finite(z)||!Finite(at)||!Finite(rotationZ)||!Finite(owner.visualScale)||owner.visualScale<=0||owner.facing!=1&&owner.facing!=-1)throw new ArgumentException("Unbound/nonfinite original element callback spawn");
   float ox,oy,oz;PlainOriginalCallbackOriginR1315(owner,at,out ox,out oy,out oz);
   // The callback-created instance has its own original lifetime. ownerState
   // is the generic DELETE_EFFECT_SET cancellation key and stays null here.
   var emission=new NpcEffectEmission{effect=id,sourcePlainCallbackStateR1315=owner.npcState,offsetX=(x-ox)/(owner.facing*owner.visualScale),offsetY=(y-oy)/owner.visualScale,offsetZ=(z-oz)/owner.visualScale,sourceLandPositionYArgument=true,rotationZ=rotationZ};
   var effect=SpawnNpcEffect(owner,d,emission,at);plainElementSpawnCountR1315++;return effect;
  }
  uint PlainElementRandomValueR1315(){plainElementRandomR1315^=plainElementRandomR1315<<13;plainElementRandomR1315^=plainElementRandomR1315>>17;plainElementRandomR1315^=plainElementRandomR1315<<5;return plainElementRandomR1315;}
  float PlainElementRandomUnitR1315(){return(PlainElementRandomValueR1315()&0xffffffu)/16777216f;}
  void PlainOriginalRandomSurfaceR1315(float center,float radius,float ceiling,out float x,out float y){
   // Finite Author2D replacement for the unmeasured native random service:
   // sample actual original surface intervals intersecting the literal range.
   // No floor, room span or offset is invented. If the requested interval has
   // no actual surface, choose the nearest real source endpoint and report it.
   float total=0;foreach(var s in CurrentRoom.surfaces){float lo=Math.Max(center-radius,s.x1),hi=Math.Min(center+radius,s.x2);if(hi>lo&&s.Y((lo+hi)*.5f)<=ceiling+.02f)total+=hi-lo;}
   if(total>0){float position=PlainElementRandomUnitR1315()*total;foreach(var s in CurrentRoom.surfaces){float lo=Math.Max(center-radius,s.x1),hi=Math.Min(center+radius,s.x2);if(hi<=lo||s.Y((lo+hi)*.5f)>ceiling+.02f)continue;if(position<=hi-lo){x=lo+position;y=s.Y(x);return;}position-=hi-lo;}}
   GroundSurface best=null;float nearest=float.PositiveInfinity,at=0;foreach(var s in CurrentRoom.surfaces){float q=Clamp(center,s.x1,s.x2),distance=Math.Abs(q-center);if(distance<nearest){best=s;nearest=distance;at=q;}}
   if(best==null)throw new InvalidOperationException("Original Plain random-position projection has no real source surface");plainElementNearestSurfaceFallbacksR1315++;x=at;y=best.Y(at);
  }
  PlainMeteorRunR1315 BeginPlainOriginalMeteorR1315(Enemy owner,NpcStateDefinition state,float at){
   float x,y,z;PlainOriginalCallbackOriginR1315(owner,at,out x,out y,out z);var support=Surface(owner.groundLine)??SurfaceBelow(x,y+.05f);if(support==null)throw new InvalidOperationException("Original Fire callback has no source land argument");float landY=support.Y(x);
   // Original source tests kind==2 twice with a RET after the first match.
   // Preserve the shipped fallback for kind3 instead of repairing it to call
   // the otherwise unreachable SMALLMETEO3_STATE_START function.
   int kind=(int)(PlainElementRandomValueR1315()%3)+1;bool mode2=kind==2;float step=owner.facing>0?1300:-1300;float[] offsets={-2200,-1500,-900,-200},radius={300,250,200,300};var run=new PlainMeteorRunR1315{state=state};
   for(int i=0;i<4;i++){
    float center=x+owner.facing*(offsets[i]+(mode2?step:0))*.28f;
    PlainOriginalRandomSurfaceR1315(center,radius[i]*.28f,landY,out run.x[i],out run.y[i]);run.y[i]+=1200*.28f;run.z[i]=z;run.slot[i]=(int)(PlainElementRandomValueR1315()%3)+1;
   }
   return run;
  }
  void AdvancePlainOriginalElementCallbacksR1315(Enemy enemy,NpcStateDefinition state,float from,float to,float packetStart){
   if(!PlainElementCallbackScopeR1315()||enemy==null||state==null||enemy.hp<=0||to<=from)return;
   var profile=NpcProfile(enemy);if(profile==null||!Array.Exists(plainElementProfilesR1315,p=>ReferenceEquals(p,profile))||!Array.Exists(profile.states,s=>ReferenceEquals(s,state)))return;
   AdvancePlainOriginalHellCallbacksR1315(enemy,state,from,to,packetStart);
   if(enemy.sourceNPC!="NUI_NASOD_ELEMENT_FIRE"&&enemy.sourceNPC!="NUI_NASOD_ELEMENT_FIRE_SOLO"||state.id!=enemy.sourceNPC.Substring(4)+"_SPECIAL_ATTACK")return;
   PlainMeteorRunR1315 run;if(!plainMeteorRunsR1315.TryGetValue(enemy,out run)||!ReferenceEquals(run.state,state)||from==0&&run.lastTo>0){
    if(from!=0)throw new InvalidOperationException("Original Fire callback consumer must bind before state entry");run=BeginPlainOriginalMeteorR1315(enemy,state,packetStart);plainMeteorRunsR1315[enemy]=run;
   }
   for(int i=0;i<4;i++){
    int choice=((8-run.slot[i])*(i+run.slot[i]))%3+1;float cut=choice*.2f;if(from>=cut||to<cut)continue;float at=packetStart+Math.Max(0,cut-from);
    // Native vRot.y is retained as an unproved full-3D orientation boundary;
    // the existing generic 2D source-facing projection remains unchanged.
    SpawnPlainOriginalEffectAtR1315(enemy,PlainMeteoR1304,run.x[i],run.y[i],run.z[i],at);SpawnPlainOriginalEffectAtR1315(enemy,PlainMeteor01R1315,run.x[i],run.y[i],run.z[i],at);
   }
   run.lastTo=to;
  }
 }
}
