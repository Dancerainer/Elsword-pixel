using System;
using System.Collections.Generic;

namespace ElswordPixel {
 public sealed partial class GameModel {
  RoomDefinition[] plainProductInstallingRoomsR1320;
  string plainProductInstallingRouteR1320;

  // Only the synchronous, exact Plain content transaction can provide this
  // provisional admission context. It neither publishes rooms nor spawns actors.
  bool PlainProductInstallingScopeR1320(string route){
   var r=plainProductInstallingRoomsR1320;
   return route==plainProductInstallingRouteR1320&&r!=null&&r.Length==3&&
    (route==PlainNormalR1258||route==PlainRecycleR1258)&&
    r[0]!=null&&r[0].id==route+"-0"&&r[1]!=null&&r[1].id==route+"-1"&&r[2]!=null&&r[2].id==route+"-2";
  }

  public void ConfigurePlainOriginalProductContentR1320(string route,string originalRegionalRulesSHA,
   RoomDefinition[] originalRooms,NpcDefinition[] profiles,NpcEffectDocument effects,NpcSoundDocument sounds,NpcMechanismDocument mechanisms,PlainOriginalFxCallbackPointR1315[] originalCallbackPoints){
   if(plainProductInstallingRoomsR1320!=null||PlainOriginalSourceBoundR1258||elapsed!=0||
      originalRooms==null||originalRooms.Length!=3||effects==null||sounds==null||mechanisms==null||originalCallbackPoints==null)
    throw new InvalidOperationException("Fresh owned Plain model and complete original content required");
   // This also validates the exact route/profile/START/state cohort before any
   // content setter. It does not alter the caller's definitions or room births.
   ProjectPlainOriginalDefinitionsR1258(route,originalRegionalRulesSHA,profiles);
   var ids=new HashSet<string>(StringComparer.Ordinal);foreach(var p in profiles)ids.Add(p.id);
   int births=0,zones=0;var staged=(RoomDefinition[])originalRooms.Clone();
   for(int i=0;i<staged.Length;i++){
    var r=staged[i];
    if(r==null||r.id!=route+"-"+i||!Finite(r.width)||r.width<480||!Finite(r.spawnX)||!Finite(r.spawnY)||
       !Finite(r.exitX)||!r.hasSpawnY||r.surfaces==null||r.groundNodes==null||r.encounters==null||r.enemies==null)
     throw new ArgumentException("Incomplete exact original Plain room");
    zones+=r.encounters.Length;
    foreach(var e in r.enemies){
     if(e==null||!ids.Contains(e.sourceNPC)||!Finite(e.x)||!Finite(e.y)||!Finite(e.hp)||
        !Finite(e.aggroRange)||!Finite(e.nativeDepth)||!Finite(e.visualScale)||e.visualScale<=0||
        (e.facing!=-1&&e.facing!=1)||!Finite(e.spawnDelay))throw new ArgumentException("Changed original Plain birth shape");
     births++;
    }
   }
   if(zones!=6||births!=(route==PlainNormalR1258?92:95))throw new ArgumentException("Original Plain 3-room/6-node/birth cohort changed");
   plainProductInstallingRoomsR1320=staged;plainProductInstallingRouteR1320=route;
   try{
    SetPlainOriginalNpcDefinitionsR1258(route,originalRegionalRulesSHA,profiles);
    SetPlainOriginalNpcEffectsR1313(route,originalRegionalRulesSHA,effects);
    SetNpcSoundDefinitions(sounds);
    SetPlainOriginalNpcMechanismsR1258(mechanisms);
     BindPlainOriginalPairR1258(route,originalRegionalRulesSHA,staged);
    // The sole original room publication/reset also performs the original V26
    // birth initialization with the correct profiles and source initial states.
    SetRooms(staged);
   }finally{plainProductInstallingRoomsR1320=null;plainProductInstallingRouteR1320=null;}
   BindPlainOriginalFxCallbacksR1315(route,originalRegionalRulesSHA,originalCallbackPoints);
   BindPlainOriginalElementCallbacksR1315(route,originalRegionalRulesSHA);
   if(!PlainOriginalRegisteredFxRouteReadyR1315)throw new InvalidOperationException("Actual registered Plain callback consumers did not bind");
  }
 }
}
