using System;
using System.Collections.Generic;

namespace ElswordPixel {
 // Optional exact-six source payload. Null preserves all older documents.
 [Serializable] public sealed class PlainOriginalFxSourceR1304 {
  public string schema,sourceEffect,sourceRulesSha256,sourceDamageLuaSha256,sourceTablesSha256,sourceDeclarationSha256,projection;
  // Optional source-only clock/geometry evidence for the three newly closed
  // original inputs. Null on older Watch/Driller documents stays supported.
  public string originalMeshSha256,nativeGeometryAndClockProofSha256,sourceClockOrigin;
  public string[] sourceRateNodes;
  public float effectiveTicksPerSecond;
  public bool nativeParityProven,runtimeAdmitted,sourceMechanismsComplete;
  public float stunSeconds,coldSeconds,coldChance,coldAnimationSpeed,coldMixCoefficient,changeAt,gravityWorldX;
  public int stunSubsequentHits;
  public float[] sourceGravity;
  public PlainOriginalNextDamageR1304 next;
 }
 [Serializable] public sealed class PlainOriginalNextDamageR1304 {
  public float physical,magic,launch,push,stopAttacker,stopDefender,repeatGap,coldSeconds,coldChance,coldAnimationSpeed,coldMixCoefficient;
  public HitReaction reaction;public bool repeat,sourceRepeatDeclared,sourceRepeatGapDeclared;
 }
 // Private binding copies prevent caller mutation of the optional payload from
 // changing an installed status/phase/acceleration branch. Geometry stays in
 // the existing original definition and is validated by its existing consumer.
 internal sealed class PlainOriginalFxBindingR1304 {
  internal readonly NpcEffectDefinition definition;
  internal readonly float stunSeconds,coldSeconds,coldChance,coldSpeed,coldCoefficient,changeAt,gravityX;
  internal readonly int stunHits;
  internal readonly PlainOriginalDamageValueR1304 next;
  internal readonly bool hasNext;
  internal PlainOriginalFxBindingR1304(NpcEffectDefinition d){
   definition=d;var s=d.plainOriginalSourceR1304;stunSeconds=s.stunSeconds;stunHits=s.stunSubsequentHits;
   coldSeconds=s.coldSeconds;coldChance=s.coldChance;coldSpeed=s.coldAnimationSpeed;coldCoefficient=s.coldMixCoefficient;changeAt=s.changeAt;gravityX=s.gravityWorldX;
   hasNext=s.next!=null;next=new PlainOriginalDamageValueR1304(s.next);
  }
 }
 internal struct PlainOriginalDamageValueR1304 {
  internal readonly float physical,magic,launch,push,stopAttacker,stopDefender,repeatGap,coldSeconds,coldChance,coldSpeed,coldCoefficient;
  internal readonly HitReaction reaction;internal readonly bool repeat;
  internal PlainOriginalDamageValueR1304(PlainOriginalNextDamageR1304 n){
   physical=n==null?0:n.physical;magic=n==null?0:n.magic;launch=n==null?0:n.launch;push=n==null?0:n.push;
   stopAttacker=n==null?0:n.stopAttacker;stopDefender=n==null?0:n.stopDefender;repeatGap=n==null?0:n.repeatGap;reaction=n==null?0:n.reaction;repeat=n!=null&&n.repeat;
   coldSeconds=n==null?0:n.coldSeconds;coldChance=n==null?0:n.coldChance;coldSpeed=n==null?0:n.coldAnimationSpeed;coldCoefficient=n==null?0:n.coldMixCoefficient;
  }
 }
 public sealed partial class GameModel {
  const string PlainFxSchemaR1304="plain-six-original-fx-finite-author2d-r1304";
  const string PlainFxProjectionR1304="Author2D: stun2s-or-2-subsequent-positive-HP-hits; cold-MIX=(P+M)/2; source-Gravity-world-vector; next-fresh-existing-damage-projector-defaults-and-retained-hit-memory";
  const string PlainFxRulesR1304="159ef67868b5c07a977afbf5d25ac497274d727c0c4fad5bd8f4325c20f63035";
  const string PlainFxLuaR1304="21747879161f19722278568dc2392c8778f88be491123a15f5f550cd63ac7397";
  const string PlainFxTablesR1304="2c9f07f025fc7267cab37478c2bee421050392ae9aa00500bdee32c54cf3cb3c";
  const string PlainWatchR1304="DamageEffect_NASOD_WATCH_ATTACK",PlainMeteoR1304="DamageEffect_NASOD_ELEMENT_FIRE_SOLO_SPECIAL_ATTACK_02";
  const string PlainHellR1304="NASOD_ELEMENT_ICE_HELLSTONE",PlainWallR1304="NASOD_ELEMENT_ICE_ICEWALL";
  const string PlainDrillerLeftR1304="DamageEffect_NASOD_DRILLER_CANNON_ATTACK_C_LEFT",PlainDrillerRightR1304="DamageEffect_NASOD_DRILLER_CANNON_ATTACK_C_RIGHT";
  readonly Dictionary<string,PlainOriginalFxBindingR1304> plainFxBindingsR1304=new Dictionary<string,PlainOriginalFxBindingR1304>(StringComparer.Ordinal);
  NpcDefinition[] plainFxProfilesR1304;string plainFxRouteR1304;
  float plainFxStunUntilR1304,plainFxColdUntilR1304,plainFxColdAnimationSpeedR1304=1;int plainFxStunInitialSerialR1304,plainFxStunSubsequentR1304,plainFxStunTargetHitsR1304;
  double plainFxColdDotUntilR1304,plainFxColdDotNextR1304;float plainFxColdDotPowerR1304;
  uint plainFxRandomR1304=0x1304a1u;
  public int PlainOriginalFxBoundCountR1304 {get{return PlainOriginalFxScopeR1304()?plainFxBindingsR1304.Count:0;}}
  public bool PlainOriginalFxNativeParityProvenR1304 {get{return false;}}
  public bool PlainOriginalFxAllSixPlayableR1304 {get{return false;}}
  public float PlainOriginalFxStunRemainingR1304 {get{return PlainOriginalFxScopeR1304()?Math.Max(0,plainFxStunUntilR1304-elapsed):0;}}
  public float PlainOriginalFxColdRemainingR1304 {get{return PlainOriginalFxScopeR1304()?Math.Max(0,plainFxColdUntilR1304-elapsed):0;}}
  public int PlainOriginalFxStunSubsequentHitsR1304 {get{return plainFxStunSubsequentR1304;}}

  static string PlainOriginalFxDeclarationR1304(string id){switch(id){
   case "DamageEffect_NASOD_DRILLER_CANNON_ATTACK_C_LEFT":return "d0faa6bf4d013881fe4c97bb13955ea521d5caeb14c705ae19e8b7ba57560eb5";
   case "DamageEffect_NASOD_DRILLER_CANNON_ATTACK_C_RIGHT":return "c2547ac19d7be21bae3697e5679a5f2c8405eb87d5456a431e41ab6f990375c2";
   case "DamageEffect_NASOD_ELEMENT_FIRE_SOLO_SPECIAL_ATTACK_02":return "f37320569dd355ed9cde7b255ef211debd2452d8ae4f72c734f54d847e22d0f3";
   case "DamageEffect_NASOD_WATCH_ATTACK":return "ffd52c76cded44eb128d09f476c8b40b4727553c44ec2b34d39a9aa70ea9cdfd";
   case "NASOD_ELEMENT_ICE_HELLSTONE":return "ff537cc30e6b2b1e38aecd7c31dd20e2b9c3e93505d102d2ee13557650d4dbbf";
   case "NASOD_ELEMENT_ICE_ICEWALL":return "963789d494db08b8305be9fb03b334795edd76f8314c2e565d7b77566fbb64dd";
   default:return null;
  }}
  // JsonUtility materializes absent inline DTOs. Only a complete default
  // legacy optional payload means absence; populated or reserved-six payloads
  // still pass every original exact identity/literal/geometry check below.
  static bool PlainOriginalNextDamageAbsentR1304(PlainOriginalNextDamageR1304 n){
   return n==null||(n.physical==0&&n.magic==0&&n.launch==0&&n.push==0&&n.stopAttacker==0&&n.stopDefender==0&&n.repeatGap==0&&n.coldSeconds==0&&n.coldChance==0&&n.coldAnimationSpeed==0&&n.coldMixCoefficient==0&&n.reaction==default(HitReaction)&&!n.repeat&&!n.sourceRepeatDeclared&&!n.sourceRepeatGapDeclared);
  }
  static bool PlainOriginalFxSourceAbsentR1304(PlainOriginalFxSourceR1304 s){
   return s==null||(string.IsNullOrEmpty(s.schema)&&string.IsNullOrEmpty(s.sourceEffect)&&string.IsNullOrEmpty(s.sourceRulesSha256)&&string.IsNullOrEmpty(s.sourceDamageLuaSha256)&&string.IsNullOrEmpty(s.sourceTablesSha256)&&string.IsNullOrEmpty(s.sourceDeclarationSha256)&&string.IsNullOrEmpty(s.projection)&&string.IsNullOrEmpty(s.originalMeshSha256)&&string.IsNullOrEmpty(s.nativeGeometryAndClockProofSha256)&&string.IsNullOrEmpty(s.sourceClockOrigin)&&
    (s.sourceRateNodes==null||s.sourceRateNodes.Length==0)&&(s.sourceGravity==null||s.sourceGravity.Length==0)&&s.effectiveTicksPerSecond==0&&!s.nativeParityProven&&!s.runtimeAdmitted&&!s.sourceMechanismsComplete&&s.stunSeconds==0&&s.coldSeconds==0&&s.coldChance==0&&s.coldAnimationSpeed==0&&s.coldMixCoefficient==0&&s.changeAt==0&&s.gravityWorldX==0&&s.stunSubsequentHits==0&&PlainOriginalNextDamageAbsentR1304(s.next));
  }
  static void ValidatePlainOriginalFxSourceR1304(NpcEffectDefinition d){
   string expected=PlainOriginalFxDeclarationR1304(d.id);var s=d.plainOriginalSourceR1304;
   if(expected==null&&PlainOriginalFxSourceAbsentR1304(s)){d.plainOriginalSourceR1304=null;return;}
   if(s==null||expected==null||s.schema!=PlainFxSchemaR1304||s.sourceEffect!=d.id||s.sourceRulesSha256!=PlainFxRulesR1304||s.sourceDamageLuaSha256!=PlainFxLuaR1304||s.sourceTablesSha256!=PlainFxTablesR1304||s.sourceDeclarationSha256!=expected||s.projection!=PlainFxProjectionR1304||s.nativeParityProven||s.runtimeAdmitted||s.sourceMechanismsComplete)throw new ArgumentException("Changed exact-six Plain source/finite projection identity");
   if(d.id!=PlainHellR1304&&PlainOriginalNextDamageAbsentR1304(s.next))s.next=null;
   foreach(float v in new[]{s.stunSeconds,s.coldSeconds,s.coldChance,s.coldAnimationSpeed,s.coldMixCoefficient,s.changeAt,s.gravityWorldX})if(!Finite(v))throw new ArgumentException("Nonfinite Plain source effect payload");
   bool watch=d.id==PlainWatchR1304,cold=d.id==PlainHellR1304||d.id==PlainWallR1304,meteo=d.id==PlainMeteoR1304,hell=d.id==PlainHellR1304;
   if(s.stunSeconds!=(watch?2:0)||s.stunSubsequentHits!=(watch?2:0)||s.coldSeconds!=(cold?6:0)||s.coldChance!=(hell?.5f:cold?1:0)||s.coldAnimationSpeed!=(cold?.85f:0)||s.coldMixCoefficient!=(hell?.1f:cold?.15f:0)||s.changeAt!=(hell?1.5f:0)||s.gravityWorldX!=(meteo?84:0)||(s.next!=null)!=hell)throw new ArgumentException("Changed original Plain literal status/phase/gravity parameters");
   if(meteo){
    if(s.sourceGravity==null||s.sourceGravity.Length!=3||s.sourceGravity[0]!=300||s.sourceGravity[1]!=-300||s.sourceGravity[2]!=0||!d.projectilePhysics||d.projectileGravityY!=-84||d.projectileBounceX!=0||d.projectileBounceY!=.1f||d.lifetime!=2.8f||d.damageUntil!=1)throw new ArgumentException("Meteo finite world-vector physics differs from its exact original template");
   }else if(s.sourceGravity!=null&&s.sourceGravity.Length!=0)throw new ArgumentException("Unscoped Plain gravity vector");
   if(hell){var n=s.next;
    if(d.lifetime<1.5f||n.physical!=0||n.magic!=.5f||n.launch!=140||n.push!=336||n.stopAttacker!=0||n.stopDefender!=0||n.reaction!=HitReaction.Launch||n.repeat||n.repeatGap!=.1f||n.sourceRepeatDeclared||n.sourceRepeatGapDeclared||n.coldSeconds!=6||n.coldChance!=1||n.coldAnimationSpeed!=.85f||n.coldMixCoefficient!=.1f)throw new ArgumentException("Changed original DAMAGE_DATA_NEXT or declared finite missing-field defaults");
   }
   if(meteo||cold){
    string mesh=meteo?"5b0aff6d4419af4a943cfa8a1f576069cc9b250549507462728fb26b5654eb73":hell?"71227ac53e1ff057505920b0f27ff416ac004f90e1d9ca267e8783b68ea665d6":"3d919bbafa1f51533206623fba0f3170995ca7b9c5efafe405f47248fb9b9f8e";
    string origin=meteo?"original-explicit-LT_TIME":"actual-original-d3dx9_43-loader-default";
    if(s.originalMeshSha256!=mesh||s.nativeGeometryAndClockProofSha256!="a773eccb0b175013b5c3831b97c0f32efb49946061e73b18314923aa5091fd39"||s.sourceClockOrigin!=origin||s.sourceRateNodes==null||s.sourceRateNodes.Length!=0||s.effectiveTicksPerSecond!=(meteo?0:4800)||d.lifetime!=(meteo||hell?2.8f:13f/30f))throw new ArgumentException("Changed exact original Plain X/default-loader-clock/empty-helper evidence");
    if(meteo){
     // The existing geometry validator already supports empty arrays. Do not
     // loosen it or introduce a sphere from the decorative sibling Mesh.
     if(d.baseSpheres==null||d.baseSpheres.Length!=0||d.geometrySamples==null||d.geometrySamples.Length!=2)throw new ArgumentException("Original empty Meteo helper must remain collision-free");
     foreach(var geometry in d.geometrySamples)if(geometry==null||geometry.spheres==null||geometry.spheres.Length!=0||geometry.attackLines==null||geometry.attackLines.Length!=0)throw new ArgumentException("Original empty Meteo helper cannot gain sphere/line contact");
    }
   }
   float physical=d.id==PlainDrillerLeftR1304||d.id==PlainDrillerRightR1304?.15f:0;
   float magic=watch?.5f:meteo?.125f:hell?.05f:d.id==PlainWallR1304?.7f:0;
   float push=watch||meteo||d.id==PlainWallR1304?28:hell?0:140;
   if(d.physical!=physical||d.magic!=magic||d.push!=push||d.launch!=(hell?224:0)||d.stopAttacker!=0||d.stopDefender!=0||d.reaction!=(d.id==PlainWallR1304?HitReaction.Knockdown:HitReaction.Hurt)||d.repeat!=cold||d.repeatGap!=(hell?.15f:.1f)||d.onHitBurnDuration!=0||d.onHitBurnPerSecond!=0||d.onHitManaDrainDuration!=0||d.onHitManaDrainPerSecond!=0)throw new ArgumentException("Changed first original Plain DAMAGE_DATA projection or duplicate legacy status");
  }
  bool PlainOriginalFxScopeR1304(){return plainFxProfilesR1304!=null&&ReferenceEquals(plainFxProfilesR1304,plainDefinitionsR1258)&&plainFxRouteR1304==plainRouteR1258&&PlainOriginalRoomScopeR1258(plainFxRouteR1304);}
  // Call after original Plain NPC definitions and the effect document have
  // been installed. Available exact contracts may be bound while unresolved
  // source geometry/clock entries remain explicitly absent; count reports this.
  public void BindPlainOriginalFxConsumersR1304(string route,string rulesSHA){
   if(route!=plainRouteR1258||rulesSHA!=PlainFxRulesR1304||plainDefinitionsR1258==null||!PlainOriginalRoomScopeR1258(route))throw new InvalidOperationException("Exact original Plain route/profiles must be bound first");
   var bindings=new Dictionary<string,PlainOriginalFxBindingR1304>(StringComparer.Ordinal);
   foreach(var d in npcEffectDefinitions.Values)if(d.plainOriginalSourceR1304!=null){ValidatePlainOriginalFxSourceR1304(d);bindings.Add(d.id,new PlainOriginalFxBindingR1304(d));}
   if(bindings.Count==0||bindings.Count>6)throw new ArgumentException("No finite exact-six source effects available to bind");
   ClearPlainOriginalFxBindingsR1304();plainFxRouteR1304=route;plainFxProfilesR1304=plainDefinitionsR1258;
   foreach(var row in bindings)plainFxBindingsR1304.Add(row.Key,row.Value);
  }
  void ClearPlainOriginalFxBindingsR1304(){ClearPlainOriginalFxCallbacksR1315();ClearPlainOriginalElementCallbacksR1315();plainFxBindingsR1304.Clear();plainFxProfilesR1304=null;plainFxRouteR1304=null;ClearPlainOriginalFxStatusesR1304();}
  void ClearPlainOriginalFxStatusesR1304(){plainFxStunUntilR1304=plainFxColdUntilR1304=plainFxColdDotPowerR1304=0;plainFxColdDotUntilR1304=plainFxColdDotNextR1304=0;plainFxStunInitialSerialR1304=plainFxStunSubsequentR1304=plainFxStunTargetHitsR1304=0;plainFxColdAnimationSpeedR1304=1;plainFxRandomR1304=0x1304a1u;}
  PlainOriginalFxBindingR1304 PlainOriginalFxBindingR1304(NpcEffectInstance e){
   PlainOriginalFxBindingR1304 b;NpcEffectDefinition registered;
   if(!PlainOriginalFxScopeR1304()||e==null||e.definition==null||e.owner==null||!plainFxBindingsR1304.TryGetValue(e.id,out b)||!ReferenceEquals(b.definition,e.definition)||!npcEffectDefinitions.TryGetValue(e.id,out registered)||!ReferenceEquals(registered,b.definition))return null;
   var p=NpcProfile(e.owner);if(p==null||!Array.Exists(plainFxProfilesR1304,x=>ReferenceEquals(x,p)))return null;
   string owner=e.owner.sourceNPC;
   bool allowed=e.id==PlainWatchR1304?owner=="NUI_NASOD_WATCH":e.id==PlainMeteoR1304?owner=="NUI_NASOD_ELEMENT_FIRE"||owner=="NUI_NASOD_ELEMENT_FIRE_SOLO":e.id==PlainHellR1304?owner=="NUI_NASOD_ELEMENT_ICE"||owner=="NUI_NASOD_ELEMENT_ICE_SOLO":e.id==PlainWallR1304?owner=="NUI_NASOD_ELEMENT_ICE_SOLO":owner=="NUI_NASOD_DRILLER_CANNON";
   return allowed?b:null;
  }
  bool TryPlainOriginalFxDamageViewR1304(NpcEffectInstance e,out BelowEffectDamageViewV20 value){
   var b=PlainOriginalFxBindingR1304(e);if(b==null||!b.hasNext){value=default(BelowEffectDamageViewV20);return false;}
   value=new BelowEffectDamageViewV20(e.definition,b.next,e.age>=b.changeAt);return true;
  }
  float PlainOriginalFxGravityWorldXR1304(NpcEffectInstance e){var b=PlainOriginalFxBindingR1304(e);return b==null?0:b.gravityX;}
  void OnPlainOriginalFxPlayerDamageR1304(){
   if(PlainOriginalFxStunRemainingR1304<=0||PlayerImpactSerial==plainFxStunInitialSerialR1304)return;
   plainFxStunSubsequentR1304++;if(plainFxStunSubsequentR1304>=plainFxStunTargetHitsR1304)plainFxStunUntilR1304=0;
  }
  float PlainOriginalFxRollR1304(){plainFxRandomR1304^=plainFxRandomR1304<<13;plainFxRandomR1304^=plainFxRandomR1304>>17;plainFxRandomR1304^=plainFxRandomR1304<<5;return(plainFxRandomR1304&0xffffffu)/16777216f;}
  void ApplyPlainOriginalFxPlayerStatusR1304(NpcEffectInstance e,float at){
   var b=PlainOriginalFxBindingR1304(e);if(b==null||e.reflected||player.hp<=0)return;
   if(b.stunSeconds>0){plainFxStunUntilR1304=at+b.stunSeconds;plainFxStunInitialSerialR1304=PlayerImpactSerial;plainFxStunSubsequentR1304=0;plainFxStunTargetHitsR1304=b.stunHits;}
   bool after=b.hasNext&&e.age>=b.changeAt;float duration=after?b.next.coldSeconds:b.coldSeconds,chance=after?b.next.coldChance:b.coldChance,coefficient=after?b.next.coldCoefficient:b.coldCoefficient,speed=after?b.next.coldSpeed:b.coldSpeed;
   if(duration<=0||chance<1&&PlainOriginalFxRollR1304()>=chance)return;
   // Finite Author2D conventions, not native MIX_ATTACK/RNG/scheduler proof:
   // equal-weight P/M base, one-second fractional resource ticks, refresh and
   // no stack. A dedicated RNG leaves older NPC/effect random streams intact.
   bool active=plainFxColdDotUntilR1304>at&&plainFxColdDotPowerR1304>0;
   plainFxColdUntilR1304=at+duration;plainFxColdDotUntilR1304=at+duration;plainFxColdAnimationSpeedR1304=speed;
   plainFxColdDotPowerR1304=(e.physicalAttack+e.magicAttack)*.5f*coefficient;
   if(!active)plainFxColdDotNextR1304=at+1;
  }
  void TickPlainOriginalFxStatusesR1304(){
   if(!PlainOriginalFxScopeR1304())return;
   if(player.hp<=0){ClearPlainOriginalFxStatusesR1304();return;}
   while(plainFxColdDotPowerR1304>0&&plainFxColdDotNextR1304<=elapsed+.000001&&plainFxColdDotNextR1304<=plainFxColdDotUntilR1304+.000001){
    float loss=Math.Min(player.hp,plainFxColdDotPowerR1304);ApplyCombatHealth(player,Math.Max(0,player.hp-loss));SpawnResourceDamage(player,loss);plainFxColdDotNextR1304+=1;
    if(player.hp<=0){player.state=CombatState.Dead;player.clip="death";player.clock=0;CancelAction(true);ClearPlainOriginalFxStatusesR1304();return;}
   }
   if(elapsed>=plainFxColdDotUntilR1304)plainFxColdDotUntilR1304=plainFxColdDotNextR1304=plainFxColdDotPowerR1304=0;
   if(elapsed>=plainFxStunUntilR1304)plainFxStunUntilR1304=0;
  }
  Input PlainOriginalFxFilterInputR1304(Input input){TickPlainOriginalFxStatusesR1304();return PlainOriginalFxStunRemainingR1304>0?EmptyInput:input;}
  float PlainOriginalFxAnimationDeltaR1304(float dt){return PlainOriginalFxStunRemainingR1304>0?0:PlainOriginalFxColdRemainingR1304>0?dt*plainFxColdAnimationSpeedR1304:dt;}
 }
}
