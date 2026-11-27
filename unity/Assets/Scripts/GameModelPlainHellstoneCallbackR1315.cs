using System;

namespace ElswordPixel {
 public sealed partial class GameModel {
  // Finite Author2D projection of the two exact original ATTACK_B callbacks.
  // The common binder owns route/profile/effect identity and the model-domain
  // attachment bridge. This partial adds no world, model, audio or timer owner.
  void AdvancePlainOriginalHellCallbacksR1315(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
   if(!PlainElementCallbackScopeR1315()||owner==null||owner.hp<=0||state==null)return;
   string wanted;
   if(owner.sourceNPC=="NUI_NASOD_ELEMENT_ICE")wanted="NASOD_ELEMENT_ICE_ATTACK_B";
   else if(owner.sourceNPC=="NUI_NASOD_ELEMENT_ICE_SOLO")wanted="NASOD_ELEMENT_ICE_SOLO_ATTACK_B";
   else return;
   var profile=Array.Find(plainDefinitionsR1258,p=>p!=null&&p.id==owner.sourceNPC);
   if(profile==null||!ReferenceEquals(profile,NpcProfile(owner))||profile.states==null||
      !ReferenceEquals(state,Array.Find(profile.states,s=>s!=null&&s.id==wanted))||
      !ReferenceEquals(state,NpcState(owner))||state.id!=wanted||state.loop||
      state.sourceAnimation!="AttackB_Fire"||state.sourceAnimSpeed!=1f)return;
   if(!Finite(from)||!Finite(to)||!Finite(packetStart)||to<from)
    throw new InvalidOperationException("Nonfinite/reversed original Plain Hellstone callback packet");
   const float sourceEvent=.01f;
   if(!(from<sourceEvent&&sourceEvent<=to))return;
   if(owner.facing!=1&&owner.facing!=-1)
    throw new InvalidOperationException("Original Plain Hellstone requires a real facing");
   float at=packetStart+Math.Max(0,sourceEvent-from),x,y,z;
   PlainOriginalCallbackOriginR1315(owner,at,out x,out y,out z);
   if(!Finite(at)||!Finite(x)||!Finite(y)||!Finite(z))
    throw new InvalidOperationException("Nonfinite original Plain Hellstone attachment origin");
   // Original GetPos/land world offsets use the global .28 projection once.
   // They do not inherit the NPC's INIT_SYSTEM scale (1.05).
   const float sourceUnit=.28f;
   float facing=owner.facing;
   // PC128..134 / PC156..162: main first, then PC183..189 and PC192..198.
   // The source has no rotation argument on these damage CreateInstance calls.
   SpawnPlainOriginalEffectAtR1315(owner,"NASOD_ELEMENT_ICE_HELLSTONE",
    x+facing*200f*sourceUnit,y,z,at);
   SpawnPlainOriginalEffectAtR1315(owner,"NASOD_ELEMENT_ICE_HELLSTONE_SIDE",
    x+facing*450f*sourceUnit,y+250f*sourceUnit,z,at);
   SpawnPlainOriginalEffectAtR1315(owner,"NASOD_ELEMENT_ICE_HELLSTONE_SIDE",
    x-facing*50f*sourceUnit,y+250f*sourceUnit,z,at);
   // Original SetWorldColor, nullable Flare sequence (center z+200) and SetNowMP
   // remain outside this damage-only adapter; this is not a full Lua callback.
  }
 }
}
