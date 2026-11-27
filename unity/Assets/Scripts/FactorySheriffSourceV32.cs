using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
namespace ElswordPixel {
 // Exact ordinary Sheriff source supplement. No Core Elite identity is admitted.
 // The original 185-state/role/AI/route data is passed through unchanged.
 public static class FactorySheriffSourceV32 {
  const string Npc="NUI_NASOD_SHERIFF",State="NASOD_SHERIFF_ATTACK_A",Effect="DamageEffect_NASOD_SHERIFF_ATTACK_A";
  public static NpcEffectDocument LoadAndMerge(NpcEffectDocument original,NpcDefinition[] profiles){
   var asset=Resources.Load<TextAsset>("Pixel/dungeons/altera-nasod-factory/sheriff-source-v32");
   if(!asset)throw new InvalidOperationException("Factory ordinary Sheriff source supplement missing");
   byte[] bytes=asset.bytes;string hash;using(var h=SHA256.Create())hash=BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
   if(bytes.Length!=71811||hash!="1ffe8e522f3751d1e78517c21331dbd6c40e8ee42b19c0c8cd0c0bb6fa9a5535")throw new InvalidOperationException("Factory ordinary Sheriff source supplement changed");
   return Merge(original,FactoryTypedJsonR581.ReadProductRootR1213<NpcEffectDocument>(bytes),profiles);
  }
  public static NpcEffectDocument Merge(NpcEffectDocument original,NpcEffectDocument supplement,NpcDefinition[] profiles){
   Need(original!=null&&original.definitions!=null&&original.bindings!=null&&profiles!=null,"base source documents");
   var p=Array.Find(profiles,x=>x!=null&&x.id==Npc);Need(p!=null&&p.kind=="nasod-sheriff"&&p.magicAttack==28&&p.states!=null,"ordinary source profile");
   var s=Array.Find(p.states,x=>x.id==State);Need(s!=null&&s.sourceAnimation=="ComboA"&&s.sourceAnimSpeed==1&&s.duration==(float)(19.0/6)&&s.hits.Length==0,"original attack timing");
   Need(supplement!=null&&!supplement.runtimeAdmitted&&!supplement.sourceMechanismsComplete&&supplement.definitions!=null&&supplement.definitions.Length==1&&supplement.bindings!=null&&supplement.bindings.Length==1&&supplement.reactionBindings!=null&&supplement.reactionBindings.Length==3,"exact supplement scope");
   var d=supplement.definitions[0];Need(d.id==Effect&&d.lifetime==.4f&&d.physical==0&&d.magic==.17f&&d.push==140&&d.launch==0&&!d.reflectable&&!d.repeat&&d.reaction==HitReaction.Hurt,"ordinary bullet damage");
   Need(d.geometrySamples!=null&&d.geometrySamples.Length==25&&d.baseSpheres.Length==0&&d.geometrySamples[0].attackLines.Length==6,"original line helper");
   var b=supplement.bindings[0];Need(b.npc==Npc&&b.state==State&&b.unitScale==1&&b.tracks.Length==1&&b.tracks[0].bone=="AirShip_Soldier_Gunner_Weapon"&&b.tracks[0].samples.Length==191&&b.sets.Length==3,"original weapon track");
   var times=new[]{.55f,1.23f,1.84f};for(int i=0;i<3;i++){var set=b.sets[i];Need(set.attachAt==times[i]&&set.emissions.Length==1,"three callback thresholds");var e=set.emissions[0];Need(e.effect==Effect&&e.bone==b.tracks[0].bone&&e.start==0&&!e.follow&&string.IsNullOrEmpty(e.ownerState)&&e.sourceLandPositionYArgument&&e.facingMultiplier==1&&e.rotationZ==0&&e.offsetX==0&&e.offsetY==0&&e.offsetZ==0&&e.scaleX==1&&e.scaleY==1&&e.scaleZ==1,"detached source callback emission");}
   foreach(var r in supplement.reactionBindings)Need(r.npc==Npc&&Array.Exists(p.reactions,x=>x.combatState==r.combatState&&x.sourceState==r.sourceState&&x.sourceAnimation==r.sourceAnimation&&x.duration==r.duration&&x.sourceAnimSpeed==r.sourceAnimSpeed),"source reaction attachment");
   Need(!Array.Exists(original.definitions,x=>x.id==Effect)&&!Array.Exists(original.bindings,x=>x.npc==Npc&&x.state==State),"duplicate supplement");
   return new NpcEffectDocument{runtimeAdmitted=original.runtimeAdmitted,sourceMechanismsComplete=false,definitions=Append(original.definitions,supplement.definitions),bindings=Append(original.bindings,supplement.bindings),reactionBindings=Append(original.reactionBindings??new NpcReactionEffectBinding[0],supplement.reactionBindings)};
  }
  static T[] Append<T>(T[] a,T[] b){var rows=new List<T>(a);rows.AddRange(b);return rows.ToArray();}
  static void Need(bool valid,string why){if(!valid)throw new InvalidOperationException("Factory ordinary Sheriff: "+why);}
 }
}
