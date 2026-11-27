using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace ElswordPixel {
 [Serializable] public sealed class BelowLaserPhaseDocumentV20 {
  public string schema,sourceEffect,sourceRulesSha256,sourceEffectsSha256,sourceDeclarationSha256,geometryReceiptSha256,clockAdapter;
  public bool runtimeReady,runtimeAdmitted,nativeExecuted;
  public float changeAt,beforeMagic,afterMagic,beforeRepeatGap,afterRepeatGap;
  public HitReaction beforeReaction,afterReaction;
 }
 // A value copy containing every scalar/string read by generic contact damage.
 // It contains no definition/array reference and cannot mutate another instance.
 public struct BelowEffectDamageViewV20 {
  public readonly float physical,magic,launch,push,stopAttacker,stopDefender,repeatGap,damageFrom,damageUntil;
  public readonly float onHitBurnDuration,onHitBurnPerSecond,onHitManaDrainDuration,onHitManaDrainPerSecond;
  public readonly HitReaction reaction;public readonly bool repeat,sourcePhaseSelected;public readonly int sourcePhase;
  public readonly string contactChild,hitChild;
  // Exact Plain DAMAGE_DATA_NEXT branch; the existing Below constructor is unchanged.
  internal BelowEffectDamageViewV20(NpcEffectDefinition d,PlainOriginalDamageValueR1304 next,bool after){
   physical=after?next.physical:d.physical;magic=after?next.magic:d.magic;launch=after?next.launch:d.launch;push=after?next.push:d.push;
   stopAttacker=after?next.stopAttacker:d.stopAttacker;stopDefender=after?next.stopDefender:d.stopDefender;repeatGap=after?next.repeatGap:d.repeatGap;
   damageFrom=d.damageFrom;damageUntil=d.damageUntil;reaction=after?next.reaction:d.reaction;repeat=after?next.repeat:d.repeat;contactChild=d.contactChild;hitChild=d.hitChild;
   onHitBurnDuration=d.onHitBurnDuration;onHitBurnPerSecond=d.onHitBurnPerSecond;onHitManaDrainDuration=d.onHitManaDrainDuration;onHitManaDrainPerSecond=d.onHitManaDrainPerSecond;
   sourcePhaseSelected=true;sourcePhase=after?1:0;
  }
  internal BelowEffectDamageViewV20(NpcEffectDefinition d,bool selected,bool after){
   physical=d.physical;magic=selected?(after?.4f:.3f):d.magic;launch=d.launch;push=d.push;stopAttacker=d.stopAttacker;stopDefender=d.stopDefender;
   repeatGap=selected?(after?1:.15f):d.repeatGap;damageFrom=d.damageFrom;damageUntil=d.damageUntil;
   reaction=selected?(after?HitReaction.Knockdown:HitReaction.Hurt):d.reaction;repeat=d.repeat;contactChild=d.contactChild;hitChild=d.hitChild;
   onHitBurnDuration=d.onHitBurnDuration;onHitBurnPerSecond=d.onHitBurnPerSecond;onHitManaDrainDuration=d.onHitManaDrainDuration;onHitManaDrainPerSecond=d.onHitManaDrainPerSecond;
   sourcePhaseSelected=selected;sourcePhase=selected?(after?1:0):-1;
  }
 }
 public sealed partial class GameModel {
  const string BelowLaserEffectV20="CANNONFORT_PARASITE_LASER";
  const string BelowLaserRulesShaV20="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb";
  const string BelowLaserEffectsShaV20="89e0d3e4a89d5850fd546948e2b2cf08ee2350d7503044684cbd7060c1f1fa42";
  const string BelowLaserDeclarationShaV20="71d558eceabd7d4c3afdefbfa2834a9022d4d321f641e91ceb2c6f9a505f2879";
  const string BelowLaserGeometryReceiptV20="dafc59bddef68741e6172cd7972f6fa2df4f2dbc6b2a4a218d6ad5d1de7c0014";
  const string BelowLaserClockV20="independent-effect-age-seconds-raw-change-time-inclusive-candidate-v20";
  NpcEffectDefinition belowLaserPhaseDefinitionV20;
  public bool BelowLaserPhasesEnabledV20 {get{return belowLaserPhaseDefinitionV20!=null;}}
  static bool EmptyBelowLaserPhaseV20(BelowLaserPhaseDocumentV20 d){return d!=null&&string.IsNullOrEmpty(d.schema)&&string.IsNullOrEmpty(d.sourceEffect)&&string.IsNullOrEmpty(d.sourceRulesSha256)&&string.IsNullOrEmpty(d.sourceEffectsSha256)&&string.IsNullOrEmpty(d.sourceDeclarationSha256)&&string.IsNullOrEmpty(d.geometryReceiptSha256)&&string.IsNullOrEmpty(d.clockAdapter)&&!d.runtimeReady&&!d.runtimeAdmitted&&!d.nativeExecuted&&d.changeAt==0&&d.beforeMagic==0&&d.afterMagic==0&&d.beforeRepeatGap==0&&d.afterRepeatGap==0&&d.beforeReaction==0&&d.afterReaction==0;}
  static string BelowLaserBitsV20(float value){return BitConverter.ToString(BitConverter.GetBytes(value)).Replace("-","").ToLowerInvariant();}
  static string BelowLaserGeometryV20(NpcEffectDefinition d){
   var values=new List<string>{d.id,BelowLaserBitsV20(d.lifetime)};
   foreach(var p in d.samples)values.AddRange(new[]{"curve",BelowLaserBitsV20(p.time),BelowLaserBitsV20(p.scaleX),BelowLaserBitsV20(p.scaleY),BelowLaserBitsV20(p.scaleZ),BelowLaserBitsV20(p.alpha),BelowLaserBitsV20(p.moveX)});
   foreach(float f in d.baseSpheres)values.Add("base:"+BelowLaserBitsV20(f));
   foreach(var p in d.geometrySamples){values.Add("geometry");values.Add(BelowLaserBitsV20(p.time));values.Add(p.spheres.Length.ToString());foreach(float f in p.spheres)values.Add(BelowLaserBitsV20(f));values.Add(p.attackLines.Length.ToString());foreach(float f in p.attackLines)values.Add(BelowLaserBitsV20(f));}
   using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n",values.ToArray())+"\n"))).Replace("-","").ToLowerInvariant();
  }
  public void SetNpcBelowLaserDamagePhasesV20(BelowLaserPhaseDocumentV20 doc){
   if(EmptyBelowLaserPhaseV20(doc))doc=null;
   if(doc==null){belowLaserPhaseDefinitionV20=null;return;}
   if(doc.schema!="below-tunnel-laser-damage-phases-draft-v20-r2"||doc.sourceEffect!=BelowLaserEffectV20||doc.sourceRulesSha256!=BelowLaserRulesShaV20||doc.sourceEffectsSha256!=BelowLaserEffectsShaV20||doc.sourceDeclarationSha256!=BelowLaserDeclarationShaV20||doc.geometryReceiptSha256!=BelowLaserGeometryReceiptV20||doc.clockAdapter!=BelowLaserClockV20||doc.runtimeReady||doc.runtimeAdmitted||doc.nativeExecuted||doc.changeAt!=.65f||doc.beforeMagic!=.3f||doc.afterMagic!=.4f||doc.beforeRepeatGap!=.15f||doc.afterRepeatGap!=1||doc.beforeReaction!=HitReaction.Hurt||doc.afterReaction!=HitReaction.Knockdown)throw new ArgumentException("Changed exact Below Tunnel Laser source phase draft");
   NpcEffectDefinition d;if(!npcEffectDefinitions.TryGetValue(BelowLaserEffectV20,out d)||d.magic!=.3f||d.physical!=0||d.reaction!=HitReaction.Hurt||!d.repeat||d.repeatGap!=.15f||d.push!=0||d.launch!=0||d.stopAttacker!=0||d.stopDefender!=0||d.reflectable||d.damageFrom!=0||d.damageUntil!=d.lifetime||d.baseSpheres==null||d.baseSpheres.Length!=0||d.geometrySamples==null||d.geometrySamples.Length!=93||BelowLaserGeometryV20(d)!=BelowLaserGeometryReceiptV20||!string.IsNullOrEmpty(d.contactChild)||!string.IsNullOrEmpty(d.hitChild)||d.onHitBurnDuration!=0||d.onHitManaDrainDuration!=0)throw new ArgumentException("Laser phases require unchanged exact original geometry/time/static damage fields");
   // Keep only the validated registered definition identity. Every phase scalar
   // is a readonly value at use time; caller mutation of doc cannot alter it.
   belowLaserPhaseDefinitionV20=d;
  }
  BelowEffectDamageViewV20 NpcEffectDamageViewBelowLaserV20(NpcEffectInstance effect){
   BelowEffectDamageViewV20 plain;if(TryPlainOriginalFxDamageViewR1304(effect,out plain))return plain;
   bool selected=belowLaserPhaseDefinitionV20!=null&&object.ReferenceEquals(effect.definition,belowLaserPhaseDefinitionV20)&&effect.id==BelowLaserEffectV20;
   return new BelowEffectDamageViewV20(effect.definition,selected,selected&&effect.age>=.65f);
  }
  public BelowEffectDamageViewV20 GetBelowEffectDamageViewV20(NpcEffectInstance effect){if(effect==null||effect.definition==null)throw new ArgumentException("Missing effect instance");return NpcEffectDamageViewBelowLaserV20(effect);}
 }
}
