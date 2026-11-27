using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 // Display-only projection of the current Plain profiles. Generic physical
 // reactions clear npcState; their real actor clock still selects the actor's
 // own original reaction state. No native attacker-side argument is retained:
 // force-fly uses the exact INIT_STATE.FLY_DAMAGE_FRONT display default.
 public sealed class PlainReactionPoseEntryR1473 {
  public readonly string Npc,Kind,State,Motion,Clip;public readonly CombatState Combat;public readonly float Duration,Speed;public readonly bool Loop;
  internal PlainReactionPoseEntryR1473(NpcDefinition p,CombatState combat,NpcStateDefinition s){Npc=p.id;Kind=p.kind;State=s.id;Motion=s.sourceAnimation;Clip=s.clip;Combat=combat;Duration=s.duration;Speed=s.sourceAnimSpeed;Loop=s.loop;}
 }
 public sealed class PlainReactionPoseMapR1473 {
  readonly Dictionary<string,Dictionary<CombatState,PlainReactionPoseEntryR1473>> entries=new Dictionary<string,Dictionary<CombatState,PlainReactionPoseEntryR1473>>(StringComparer.Ordinal);
  readonly List<PlainReactionPoseEntryR1473> ordered=new List<PlainReactionPoseEntryR1473>();
  public PlainReactionPoseEntryR1473[] Entries(){return ordered.ToArray();}
  public PlainReactionPoseMapR1473(NpcDefinition[] profiles){
   if(profiles==null)throw new ArgumentException("Original Plain profiles required for physical display");
   int optionalIdentities=0;
   foreach(string npc in new[]{"NUI_NASOD_WATCH","NUI_NASOD_SPRAY","NUI_NASOD_GUARDIAN_SMALL","NUI_NASOD_ELEMENT_FIRE","NUI_NASOD_ELEMENT_FIRE_SOLO","NUI_NASOD_ELEMENT_ICE","NUI_NASOD_ELEMENT_ICE_SOLO","NUI_NASOD_MOUSE","NUI_NASOD_MOUSE_HOUSE"}){
    var p=Array.Find(profiles,x=>x!=null&&x.id==npc);bool watch=npc=="NUI_NASOD_WATCH",spray=npc=="NUI_NASOD_SPRAY",guardian=npc=="NUI_NASOD_GUARDIAN_SMALL",mouse=npc=="NUI_NASOD_MOUSE",house=npc=="NUI_NASOD_MOUSE_HOUSE";
    if(p==null&&!watch&&!spray)continue;if(!watch&&!spray)optionalIdentities++;
    string kind=watch?"nasod-watch":spray?"nasod-spray":mouse?"nasod-mouse":house?"nasod-mouse-house":guardian?"nasod-guardian-small":npc=="NUI_NASOD_ELEMENT_FIRE"?"nasod-element-fire":npc=="NUI_NASOD_ELEMENT_FIRE_SOLO"?"nasod-element-fire-solo":npc=="NUI_NASOD_ELEMENT_ICE"?"nasod-element-ice":"nasod-element-ice-solo";
    if(p==null||p.kind!=kind||p.states==null||p.reactions==null||p.reactions.Length!=(mouse||house?1:3))throw new ArgumentException("Exact own Plain original reaction identity/count required");
    var byState=new Dictionary<CombatState,PlainReactionPoseEntryR1473>();
    foreach(var r in p.reactions){
     var s=r==null?null:Array.Find(p.states,x=>x!=null&&x.id==r.sourceState);
     if(r==null||s==null||byState.ContainsKey(r.combatState)||(r.combatState!=CombatState.Hurt&&r.combatState!=CombatState.Knockdown&&r.combatState!=CombatState.GettingUp)||s.loop||s.sourceAnimation!=r.sourceAnimation||s.duration!=r.duration||s.sourceAnimSpeed!=r.sourceAnimSpeed||r.duration<=0||r.sourceAnimSpeed<=0||float.IsNaN(r.duration)||float.IsInfinity(r.duration))throw new ArgumentException("Physical display requires a unique original non-loop reaction state");
     var entry=new PlainReactionPoseEntryR1473(p,r.combatState,s);byState.Add(entry.Combat,entry);ordered.Add(entry);
    }
    // Guardian's INIT names a FLY state absent from its current original bank.
    // Its three actual ground reactions are bound; no Air pose is invented.
    if(mouse){var down=Array.Find(p.states,x=>x!=null&&x.id=="NASOD_MOUSE_DAMAGE_DOWN");
     if(down==null||down.sourceAnimation!="DamageDown"||down.clip!="source-damage-down"||down.loop||down.sourceAnimSpeed!=1||down.duration!=1.3333333333333333f||down.nextState!="NASOD_MOUSE_WAIT")throw new ArgumentException("Original Mouse INIT_STATE.DOWN_DAMAGE_LAND_FRONT differs");
     var landed=new PlainReactionPoseEntryR1473(p,CombatState.Knockdown,down);byState.Add(landed.Combat,landed);ordered.Add(landed);
    }
    if(!guardian&&!house){string prefix=npc.Substring(4);var fly=Array.Find(p.states,x=>x!=null&&x.id==prefix+(mouse?"_DAMAGE_FLY":"_DAMAGE_FLY_FRONT"));
     if(fly==null||fly.sourceAnimation!=(watch?"DamageAirFly":mouse?"DamageAirSmall":"DamageAirFlyFront")||fly.clip!=(watch?"source-damage-air-fly":mouse?"hurt-air":"launch-front")||fly.loop||fly.sourceAnimSpeed!=1||fly.duration!=(mouse?.3333333333333333f:1)||fly.nextState!=prefix+"_WAIT")throw new ArgumentException("Original own INIT_STATE.FLY_DAMAGE_FRONT differs");
     var airborne=new PlainReactionPoseEntryR1473(p,CombatState.Airborne,fly);byState.Add(airborne.Combat,airborne);ordered.Add(airborne);
    }entries.Add(p.id,byState);
   }
   if(optionalIdentities!=0&&optionalIdentities!=7||ordered.Count!=(optionalIdentities==0?8:31))throw new ArgumentException("Exact Recycle eight or Plain thirty-one original display bindings required");
   var shock=Array.Find(profiles,p=>p!=null&&p.id=="NUI_SHOCK_STICK");
   if(shock!=null){
    var damage=shock.states==null?null:Array.Find(shock.states,s=>s!=null&&s.id=="SHOCK_STICK_DAMAGE");
    if(optionalIdentities!=7||shock.kind!="shock-stick"||shock.states.Length!=7||shock.reactions==null||shock.reactions.Length!=1||
     shock.reactions[0].combatState!=CombatState.Hurt||shock.reactions[0].sourceState!="SHOCK_STICK_DAMAGE"||damage==null||damage.sourceAnimation!="Damage"||
     damage.clip!="hurt"||damage.loop||damage.sourceAnimSpeed!=1||damage.duration!=38f/60||damage.nextState!="SHOCK_STICK_WAIT")
     throw new ArgumentException("Original ShockStick INIT damage/default bank differs");
    // All original INIT damage directions/types select this same Damage.
    // This also covers the empty-npcState interval between a real hit and the
    // model owner's next ordinary tick; the renderer never mutates the actor.
    var mapped=new Dictionary<CombatState,PlainReactionPoseEntryR1473>();
    foreach(var combat in new[]{CombatState.Hurt,CombatState.Knockdown,CombatState.GettingUp,CombatState.Airborne}){
     var entry=new PlainReactionPoseEntryR1473(shock,combat,damage);mapped.Add(combat,entry);ordered.Add(entry);
    }entries.Add(shock.id,mapped);
   }
  }
  public bool TrySelect(Enemy actor,out string state,out float time,out bool physical){
   state=null;time=0;physical=false;if(actor==null)return false;Dictionary<CombatState,PlainReactionPoseEntryR1473> byState;
   if(!entries.TryGetValue(actor.sourceNPC??"",out byState))return false;
   if(actor.kind!=byState[CombatState.Hurt].Kind)throw new ArgumentException("Own Plain physical display kind differs");
   if(!string.IsNullOrEmpty(actor.npcState)){state=actor.npcState;time=actor.npcTime;return true;}
   if(actor.hp<=0||actor.state==CombatState.Airborne&&actor.clip!="launched"&&!actor.knockdownOnLanding)return false;
   PlainReactionPoseEntryR1473 entry;if(!byState.TryGetValue(actor.state,out entry))return false;
   if(float.IsNaN(actor.clock)||float.IsInfinity(actor.clock))throw new ArgumentException("Nonfinite actual Plain physical reaction clock");
   state=entry.State;time=Math.Max(0,Math.Min(actor.clock,entry.Duration));physical=true;return true;
  }
 }
 // Explicit product visibility adapter for the three finite identities below.
 // A missing physical-state binding retains this Enemy's own last valid bank
 // pose. With no history it displays its own validated WAIT at time zero.
 // Neither path claims to be the original hit/get-up animation.
 public sealed class PlainReactionVisibilityCacheR1475 {
  sealed class Remembered {public Enemy Actor;public PlainVariablePhaseBankR1330 Bank;public string Npc,Kind,State;public float Time;}
  readonly object owner;bool retired;readonly List<Remembered> remembered=new List<Remembered>();
  readonly Dictionary<string,PlainReactionPoseEntryR1473> defaults=new Dictionary<string,PlainReactionPoseEntryR1473>(StringComparer.Ordinal);
  public PlainReactionVisibilityCacheR1475(object currentOwner,NpcDefinition[] profiles){
   if(currentOwner==null||profiles==null)throw new ArgumentException("Owned Plain visibility adapter requires current profiles");owner=currentOwner;
   foreach(string npc in new[]{"NUI_NASOD_GUARDIAN_SMALL","NUI_NASOD_MOUSE","NUI_NASOD_MOUSE_HOUSE"}){
    var p=Array.Find(profiles,x=>x!=null&&x.id==npc);if(p==null)continue;
    string kind=npc=="NUI_NASOD_GUARDIAN_SMALL"?"nasod-guardian-small":npc=="NUI_NASOD_MOUSE"?"nasod-mouse":"nasod-mouse-house";
    var s=p.states==null?null:Array.Find(p.states,x=>x!=null&&x.id==p.waitState);
    if(p.kind!=kind||s==null||s.id!=(npc=="NUI_NASOD_MOUSE_HOUSE"?"NASOD_MOUSEHOUSE_WAIT":npc.Substring(4)+"_WAIT")||s.sourceAnimation!="Wait"||s.clip!="idle"||s.duration<=0||s.sourceAnimSpeed!=1)throw new ArgumentException("Own original WAIT visibility default differs");
    defaults.Add(npc,new PlainReactionPoseEntryR1473(p,CombatState.Normal,s));
   }if(defaults.Count!=0&&defaults.Count!=3)throw new ArgumentException("Exact finite Plain visibility cohort required");
  }
  public PlainReactionPoseEntryR1473[] Defaults(){var all=new List<PlainReactionPoseEntryR1473>();foreach(var p in defaults.Values)all.Add(p);return all.ToArray();}
  bool Own(Enemy actor,object currentOwner,PlainVariablePhaseBankR1330 bank,out PlainReactionPoseEntryR1473 entry){
   entry=null;return !retired&&ReferenceEquals(owner,currentOwner)&&actor!=null&&bank!=null&&defaults.TryGetValue(actor.sourceNPC??"",out entry)&&actor.kind==entry.Kind&&bank.npc==entry.Npc&&bank.kind==entry.Kind;
  }
  public void RememberOriginal(Enemy actor,object currentOwner,PlainVariablePhaseBankR1330 bank,string state,float time){
   PlainReactionPoseEntryR1473 own;if(!Own(actor,currentOwner,bank,out own))return;
   if(string.IsNullOrEmpty(state)||float.IsNaN(time)||float.IsInfinity(time)||time<0||bank.stateDisplayBindingsOnly==null||!Array.Exists(bank.stateDisplayBindingsOnly,x=>x!=null&&x.state==state))throw new ArgumentException("Only an own validated original bank pose may be remembered");
   var r=remembered.Find(x=>ReferenceEquals(x.Actor,actor));if(r==null){r=new Remembered{Actor=actor};remembered.Add(r);}r.Bank=bank;r.Npc=actor.sourceNPC;r.Kind=actor.kind;r.State=state;r.Time=time;
  }
  public bool TrySelect(Enemy actor,object currentOwner,PlainVariablePhaseBankR1330 bank,out string state,out float time,out bool previousPose){
   state=null;time=0;previousPose=false;PlainReactionPoseEntryR1473 own;
   if(!Own(actor,currentOwner,bank,out own)||actor.hp<=0||!string.IsNullOrEmpty(actor.npcState))return false;
   if(actor.state!=CombatState.Hurt&&actor.state!=CombatState.Knockdown&&actor.state!=CombatState.GettingUp&&(actor.state!=CombatState.Airborne||actor.clip!="launched"&&!actor.knockdownOnLanding))return false;
   var r=remembered.Find(x=>ReferenceEquals(x.Actor,actor));
   if(r!=null&&ReferenceEquals(r.Bank,bank)&&r.Npc==actor.sourceNPC&&r.Kind==actor.kind&&bank.stateDisplayBindingsOnly!=null&&Array.Exists(bank.stateDisplayBindingsOnly,x=>x!=null&&x.state==r.State)){state=r.State;time=r.Time;previousPose=true;return true;}
   state=own.State;time=0;return true;
  }
  public void Clear(){remembered.Clear();defaults.Clear();retired=true;}
 }
 [Serializable] public sealed class PlainWatchWholeResourceR1330 {public Mine2ProductFileR1211 document;}
 public sealed partial class DemoScene {
  const string PlainWatchDocumentShaR1330="1e18f95034c1f919b987cf80ec445cc976c7d61705142620509b051a8017e8cc";
  SourcePhaseWholeLookupV21 plainWatchLookupR1330;GameModel plainWatchModelR1330;PlainReactionPoseMapR1473 plainReactionPoseMapR1473;PlainReactionVisibilityCacheR1475 plainReactionVisibilityCacheR1475;
  readonly DeferredWholeSpritesV53 plainWatchSpritesR1330=new DeferredWholeSpritesV53();
  readonly Dictionary<string,Texture2D> plainWatchTexturesR1330=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
  public bool PlainWatchWholeConfiguredR1330 {get{return plainWatchLookupR1330!=null;}}
  public int PlainWatchWholeResidentKeysR1330 {get{return plainWatchSpritesR1330.ResidentCount;}}
  public int PlainWatchWholeDrawsR1330 {get;private set;}
  public int PlainWatchWholeMissingStateHoldsR1330 {get;private set;}

  void ConfigurePlainProductWatchWholeR1330(PlainProductResourceIndexR1320 index,Mine2ProductFileR1211 document,NpcDefinition[] originalPlainProfiles){
   PlainProductNeedR1320(index!=null&&document!=null&&document.sha256==PlainWatchDocumentShaR1330&&
    plainProductSessionR1320&&ReferenceEquals(game,plainProductModelR1320)&&game.PlainOriginalSourceBoundR1258&&
    plainWatchLookupR1330==null&&plainWatchSpritesR1330.Count==0&&plainWatchTexturesR1330.Count==0,"fresh owned Plain Watch binding required");
   // The original Core-approved document and the pure lookup stay byte-exact.
   // Its legacy source-rules pin is provenance, not a request to install Core rules.
   var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(index.Read(document));var lookup=new SourcePhaseWholeLookupV21(d);
   PlainProductNeedR1320(lookup.Id=="nasod-watch"&&lookup.SourceNpc=="NUI_NASOD_WATCH"&&
    lookup.SourceRulesSha256=="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72"&&
    lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==44&&lookup.MotionCount==16&&lookup.StateCount==28&&d.sourcePoseSamples==79,"original complete Watch whole bank changed");
   var profile=Array.Find(originalPlainProfiles??new NpcDefinition[0],p=>p!=null&&p.id==lookup.SourceNpc);
   PlainProductNeedR1320(profile!=null&&profile.kind==lookup.Id&&profile.states!=null&&profile.states.Length==28,"exact Plain Watch source profile absent");
   foreach(var actual in profile.states){var expected=lookup.State(actual.id);PlainProductNeedR1320(actual.clip==expected.ModelClip&&actual.sourceAnimation==expected.Motion&&actual.sourceAnimSpeed==expected.Speed&&actual.duration==expected.StateDuration&&actual.loop==expected.Loop&&actual.nextState==expected.NextState&&!expected.HasShow,"Plain Watch source state/clock/SHOW differs from original whole index "+actual.id);}
   var reactionMap=new PlainReactionPoseMapR1473(originalPlainProfiles);
   var visibilityCache=new PlainReactionVisibilityCacheR1475(game,originalPlainProfiles);var displayEntries=new List<PlainReactionPoseEntryR1473>(reactionMap.Entries());displayEntries.AddRange(visibilityCache.Defaults());
   foreach(var entry in displayEntries){
    if(entry.Npc=="NUI_NASOD_WATCH"){var visual=lookup.State(entry.State);PlainProductNeedR1320(visual.ModelClip==entry.Clip&&visual.Motion==entry.Motion&&visual.StateDuration==entry.Duration&&visual.Speed==entry.Speed&&!visual.Loop,"Original Watch physical display bank differs");}
    else{PlainVariablePhaseBankR1330 bank;PlainProductNeedR1320(plainVariableBanksR1330.TryGetValue(entry.Npc,out bank),"Own original variable phase bank must precede physical display binding");var visual=Array.Find(bank.stateDisplayBindingsOnly,x=>x.state==entry.State);PlainProductNeedR1320(visual!=null&&visual.modelClip==entry.Clip&&visual.originalSourceAnimation==entry.Motion&&(float)visual.originalStateDuration==entry.Duration&&(float)visual.originalSourceAnimSpeed==entry.Speed&&visual.originalLoop==entry.Loop,"Own original variable physical display bank differs");}
   }
   foreach(var key in lookup.Keys()){
    plainWatchSpritesR1330.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-watch"),()=>{
    PlainProductNeedR1320(key.Width>0&&key.Height>0&&key.Width<=64&&key.Height<=64,"complete original Watch whole dimensions changed");
    var bytes=index.Read(new Mine2ProductFileR1211{path=key.PngPath,sha256=key.PngSha256});
    var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Plain original Watch whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
    plainProductOwnedR1320.Add(texture);plainWatchTexturesR1330.Add(key.Key,texture);
    PlainProductNeedR1320(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"original Watch PNG decode/dimensions changed");
    var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
    plainProductOwnedR1320.Add(sprite);return sprite;
    });
   }
   plainReactionVisibilityCacheR1475=visibilityCache;plainReactionPoseMapR1473=reactionMap;plainWatchLookupR1330=lookup;plainWatchModelR1330=game;PlainWatchWholeDrawsR1330=PlainWatchWholeMissingStateHoldsR1330=0;
  }
  readonly VisibleHurtPoseV44 plainVisibleHurtPoseV44=new VisibleHurtPoseV44();
  bool TryRenderPlainProductWatchWholeR1330(Enemy e,SpriteRenderer renderer){
   if(!PlainProductSessionActiveR1320||e==null||e.sourceNPC!="NUI_NASOD_WATCH")return false;
   PlainProductNeedR1320(ReferenceEquals(game,plainWatchModelR1330)&&game.enemies.Contains(e)&&
    game.CurrentRoom!=null&&game.CurrentRoom.id==plainProductRouteR1320+"-"+game.room&&
    plainWatchLookupR1330!=null&&renderer&&e.kind=="nasod-watch"&&e.visualScale==1,"unbound current Plain Watch renderer");
   string displayState;float displayTime;bool physical;PlainProductNeedR1320(plainReactionPoseMapR1473!=null,"Original Plain physical display binding absent");
   if(!game.TryPlainWatchLethalPoseV36(e,out displayState,out displayTime) && !TryOriginalWholeReactionV45(e,out displayState,out displayTime) && !plainReactionPoseMapR1473.TrySelect(e,out displayState,out displayTime,out physical) && !plainVisibleHurtPoseV44.TryHold(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;PlainWatchWholeMissingStateHoldsR1330++;return true;}
   var selected=plainWatchLookupR1330.Select(displayState,displayTime);var key=selected.Whole;plainVisibleHurtPoseV44.Remember(e,displayState,displayTime);
   RecordMonsterPixelSourceV53(e,renderer,"Plain/Watch",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
   renderer.enabled=true;renderer.sprite=plainWatchSpritesR1330[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
   // Same signed full-PNG anchor formula as the original Core renderer. The
   // selected anchor already contains its display offset; do not add it again.
   var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;
   renderer.transform.localPosition=new Vector3(position.x,position.y,0);PlainWatchWholeDrawsR1330++;return true;
  }
  void ClearPlainProductWatchWholeR1330(){
   // Textures/sprites belong to the ordinary Plain load transaction ledger.
   // The normal Host destroys them once, including failed/cancelled loads.
   if(plainReactionVisibilityCacheR1475!=null)plainReactionVisibilityCacheR1475.Clear();plainReactionVisibilityCacheR1475=null;plainWatchSpritesR1330.Clear();plainWatchTexturesR1330.Clear();plainWatchLookupR1330=null;plainWatchModelR1330=null;plainReactionPoseMapR1473=null;
  }
 }
}

namespace ElswordPixel {
 [Serializable] public sealed class PlainVariablePhaseResourceR1330 {public Mine2ProductFileR1211 document;}
 [Serializable] public sealed class PlainVariablePhaseTextureR1330 {public string textureKey,rgbaSha256;public Mine2ProductFileR1211 file;public int width,height;}
 [Serializable] public sealed class PlainVariablePhaseMotionSourceR1330 {public string name,sha256;}
 [Serializable] public sealed class PlainVariablePhaseKeyR1330 {public int variablePhaseIndex,originalGeometryOffset,originalWidth,originalHeight;public string poseID,textureKey;public double originalTime,originalDuration;public int[] rect;public double[] anchor;public Mine2ProductFileR1211 originalPng;}
 [Serializable] public sealed class PlainVariablePhaseMotionR1330 {public string animation;public double sourceMotionDuration;public PlainVariablePhaseKeyR1330[] frameKeys;}
 [Serializable] public sealed class PlainVariablePhaseStateR1330 {public string state,modelClip,originalSourceAnimation,originalNextState;public double originalSourceAnimSpeed,originalStateDuration,originalSourceMotionDuration;public bool originalLoop,phaseAnimationPresent;public int[] referencePhaseIndices;}
 [Serializable] public sealed class PlainVariablePhaseBankR1330 {
  public string npc,kind;public Mine2ProductFileR1211 originalGeometryIdentity,referenceViewSettings,referenceManifest;public PlainVariablePhaseMotionSourceR1330 sourceMotion;
  public int originalCanvasWidth,originalCanvasHeight;public int[] unionCrop;public double referenceYShift,displayAnchorX,displayAnchorY;
  public PlainVariablePhaseTextureR1330[] textures;public PlainVariablePhaseMotionR1330[] motions;public PlainVariablePhaseStateR1330[] stateDisplayBindingsOnly;
 }
 [Serializable] public sealed class PlainVariablePhaseDocumentR1330 {
  public string schema,sourceClockDesignSha256,sourceClockDesignJson,textureRgbaHashDomain;public bool sourceOnly,dataOnly,pixelArtAccepted,nativeEquivalent,runtimeReleaseAccepted,naturalRouteAccepted;
  public Mine2ProductFileR1211 actualReferenceRenderReceipt;public PlainVariablePhaseBankR1330[] rows;
 }
 public static class PlainVariablePhaseDataR1330 {
  public const string ClockDesignSHA="bd553d075b6cd16b079657857b6634837b54afb973dbac115a7ea16527ba7001";
  public const string ActualReferenceSHA="ba7051380d8a3165ce0cad597798f7c9ebb9b20fd408ffa6ab6edd93c332f1b9";
  public static bool IsNpc(string id){switch(id){case "NUI_BARRIERMACHINE_FIRE":case "NUI_BARRIERMACHINE_ICE":case "NUI_NASOD_ELEMENT_FIRE":case "NUI_NASOD_ELEMENT_FIRE_SOLO":case "NUI_NASOD_ELEMENT_ICE":case "NUI_NASOD_ELEMENT_ICE_SOLO":case "NUI_NASOD_GUARDIAN_SMALL":case "NUI_NASOD_MOUSE":case "NUI_NASOD_MOUSE_HOUSE":case "NUI_NASOD_SPRAY":case "NUI_SHOCK_STICK":return true;default:return false;}}
  public static int RouteCount(string route){return route==PlainProductResourceIndexR1320.Normal?11:route==PlainProductResourceIndexR1320.Recycle?1:0;}
  static void Need(bool ok,string why){PlainProductResourceIndexR1320.Need(ok,"variable phase: "+why);}
  static bool Finite(double x){return !double.IsNaN(x)&&!double.IsInfinity(x);}
  static bool Pin(Mine2ProductFileR1211 q){return q!=null&&!string.IsNullOrEmpty(q.path)&&BelowPlayerResourceIndexV21.IsHash(q.sha256);}
  static bool SamePin(Mine2ProductFileR1211 a,Mine2ProductFileR1211 b){return Pin(a)&&Pin(b)&&a.path==b.path&&a.sha256==b.sha256;}
  public static PlainVariablePhaseDocumentR1330 Read(PlainProductResourceIndexR1320 index,NpcDefinition[] profiles){
   var file=index.Document.phaseRows[0].document;var d=FactoryTypedJsonR581.ReadPlainVariablePhaseR1330(index.Read(file));
   Need(d!=null&&d.schema=="plain-original-variable-phase-bank-r1330"&&d.dataOnly&&!d.pixelArtAccepted&&!d.nativeEquivalent&&!d.runtimeReleaseAccepted&&!d.naturalRouteAccepted,"bank data flags changed");
   Need(d.sourceClockDesignSha256==ClockDesignSHA&&d.sourceClockDesignJson!=null&&BelowPlayerResourceIndexV21.Hash(new System.Text.UTF8Encoding(false,true).GetBytes(d.sourceClockDesignJson))==ClockDesignSHA,"original complete source clock design bytes changed");
   var original=FactoryTypedJsonR581.ReadPlainVariablePhaseR1330(new System.Text.UTF8Encoding(false,true).GetBytes(d.sourceClockDesignJson));
   Need(original.schema=="PLAIN_ELEVEN_SOURCE_POSE_CLOCKS_AND_VARIABLE_PHASE_DISPLAY_DESIGN"&&original.sourceOnly&&original.rows!=null&&original.rows.Length==11&&d.rows!=null&&d.rows.Length==11&&Pin(d.actualReferenceRenderReceipt)&&d.actualReferenceRenderReceipt.sha256==ActualReferenceSHA,"actual1019 reference/source11 cohort absent");
   Need(d.textureRgbaHashDomain=="sha256-top-down-row-major-RGBA8-no-dimensions-prefix","page RGBA hash domain must be explicit");
   var npcs=new HashSet<string>(StringComparer.Ordinal);int motions=0,poses=0,states=0,applicable=0;
   foreach(var row in d.rows){
    Need(row!=null&&IsNpc(row.npc)&&npcs.Add(row.npc),"unknown/duplicate bank NPC");var source=Array.Find(original.rows,r=>r.npc==row.npc);
    Need(source!=null&&row.kind==source.kind&&SamePin(row.originalGeometryIdentity,source.originalGeometryIdentity)&&row.sourceMotion!=null&&source.sourceMotion!=null&&row.sourceMotion.name==source.sourceMotion.name&&row.sourceMotion.sha256==source.sourceMotion.sha256,"original identity/geometry/motion provenance changed");
    double y=row.npc=="NUI_NASOD_GUARDIAN_SMALL"?(448.0-18.51209960892487):448,shift=row.npc=="NUI_NASOD_GUARDIAN_SMALL"?18.51209960892487:0;
    Need(row.originalCanvasWidth==512&&row.originalCanvasHeight==512&&row.displayAnchorX==256&&row.displayAnchorY==y&&row.referenceYShift==shift&&Pin(row.referenceViewSettings)&&Pin(row.referenceManifest),"one original canvas/finite foot-domain anchor changed");
    var crop=row.unionCrop;Need(crop!=null&&crop.Length==4&&crop[0]>=0&&crop[1]>=0&&crop[2]>0&&crop[3]>0&&crop[0]+crop[2]<=512&&crop[1]+crop[3]<=512,"one per-NPC union crop required");
    Need(row.textures!=null&&row.textures.Length>0&&row.motions!=null&&row.motions.Length==source.motions.Length&&row.stateDisplayBindingsOnly!=null&&row.stateDisplayBindingsOnly.Length==source.stateDisplayBindingsOnly.Length,"complete original bank arrays required");
    var pages=new Dictionary<string,PlainVariablePhaseTextureR1330>(StringComparer.Ordinal);foreach(var t in row.textures){Need(t!=null&&!string.IsNullOrEmpty(t.textureKey)&&!pages.ContainsKey(t.textureKey)&&Pin(t.file)&&t.width>0&&t.height>0&&t.width<=4096&&t.height<=4096&&BelowPlayerResourceIndexV21.IsHash(t.rgbaSha256),"original Point page binding invalid");pages.Add(t.textureKey,t);}
    var animations=new HashSet<string>(StringComparer.Ordinal);var ids=new HashSet<string>(StringComparer.Ordinal);
    foreach(var m in row.motions){var expected=Array.Find(source.motions,x=>x.animation==m.animation);Need(m!=null&&animations.Add(m.animation)&&expected!=null&&Finite(m.sourceMotionDuration)&&m.sourceMotionDuration>0&&m.sourceMotionDuration==expected.sourceMotionDuration&&m.frameKeys!=null&&m.frameKeys.Length==expected.frameKeys.Length&&m.frameKeys.Length>0,"original animation duration/frame sequence changed");
     for(int i=0;i<m.frameKeys.Length;i++){var k=m.frameKeys[i];var e=expected.frameKeys[i];PlainVariablePhaseTextureR1330 texture=null;
      Need(k!=null&&k.variablePhaseIndex==e.variablePhaseIndex&&k.poseID==e.poseID&&ids.Add(k.poseID)&&k.originalGeometryOffset==e.originalGeometryOffset&&k.originalDuration==e.originalDuration&&k.originalTime==e.originalTime&&Finite(k.originalTime)&&k.originalTime>=0&&k.originalTime<=m.sourceMotionDuration&&(i==0?k.originalTime==0:k.originalTime>m.frameKeys[i-1].originalTime),"original nonuniform key/time/geometry offset changed");
      Need(Pin(k.originalPng)&&k.originalWidth==512&&k.originalHeight==512&&k.rect!=null&&k.rect.Length==4&&k.rect[0]>=0&&k.rect[1]>=0&&k.rect[2]==crop[2]&&k.rect[3]==crop[3]&&pages.TryGetValue(k.textureKey??"",out texture)&&k.rect[0]+k.rect[2]<=texture.width&&k.rect[1]+k.rect[3]<=texture.height&&k.anchor!=null&&k.anchor.Length==2&&k.anchor[0]==256-crop[0]&&k.anchor[1]==y-crop[1],"one crop/Point rect/signed anchor or original PNG provenance changed");poses++;
     }motions++;
    }
    var profile=Array.Find(profiles,r=>r.id==row.npc);if(profile!=null){Need(profile.kind==row.kind,"current source profile kind changed");applicable++;}
    var stateIds=new HashSet<string>(StringComparer.Ordinal);foreach(var b in row.stateDisplayBindingsOnly){var e=Array.Find(source.stateDisplayBindingsOnly,x=>x.state==b.state);Need(b!=null&&stateIds.Add(b.state)&&e!=null&&b.modelClip==e.modelClip&&b.originalSourceAnimation==e.originalSourceAnimation&&b.originalSourceAnimSpeed==e.originalSourceAnimSpeed&&b.originalStateDuration==e.originalStateDuration&&b.originalSourceMotionDuration==e.originalSourceMotionDuration&&b.originalLoop==e.originalLoop&&b.originalNextState==e.originalNextState&&b.phaseAnimationPresent&&animations.Contains(b.originalSourceAnimation)&&Finite(b.originalSourceAnimSpeed)&&b.originalSourceAnimSpeed>0&&b.referencePhaseIndices!=null&&e.referencePhaseIndices!=null&&b.referencePhaseIndices.Length==e.referencePhaseIndices.Length,"original state/animation/speed/loop/nextState changed");for(int i=0;i<b.referencePhaseIndices.Length;i++)Need(b.referencePhaseIndices[i]==e.referencePhaseIndices[i],"original phase references changed");states++;}
    if(profile!=null)foreach(var a in profile.states){var b=Array.Find(row.stateDisplayBindingsOnly,x=>x.state==a.id);Need(b!=null&&a.clip==b.modelClip&&a.sourceAnimation==b.originalSourceAnimation&&a.sourceAnimSpeed==(float)b.originalSourceAnimSpeed&&a.duration==(float)b.originalStateDuration&&a.loop==b.originalLoop&&a.nextState==b.originalNextState,"current Plain state is outside original source clock contract "+a.id);}
   }
   Need(npcs.Count==11&&motions==180&&poses==1019&&states==242&&applicable==RouteCount(index.Map.sourceDungeon),"original180/1019/242 and exact route phase scope required");return d;
  }
 }
 public sealed partial class DemoScene {
  PlainVariablePhaseDocumentR1330 plainVariablePhasesR1330;GameModel plainVariablePhaseModelR1330;
  readonly Dictionary<string,PlainVariablePhaseBankR1330> plainVariableBanksR1330=new Dictionary<string,PlainVariablePhaseBankR1330>(StringComparer.Ordinal);
  readonly DeferredWholeSpritesV53 plainVariableSpritesR1330=new DeferredWholeSpritesV53();
  public int PlainVariablePhaseResidentPagesR1330 {get;private set;}public int PlainVariablePhaseDrawsR1330 {get;private set;}public int PlainVariablePhaseMissingStateHoldsR1330 {get;private set;}
  static string PlainPhaseKeyR1330(string npc,string pose){return npc+"/"+pose;}
  static string PlainPhaseRgbaR1330(Texture2D texture){var pixels=texture.GetPixels32();var raw=new byte[checked(pixels.Length*4)];int at=0;for(int y=texture.height-1;y>=0;y--)for(int x=0;x<texture.width;x++){var p=pixels[y*texture.width+x];raw[at++]=p.r;raw[at++]=p.g;raw[at++]=p.b;raw[at++]=p.a;}return BelowPlayerResourceIndexV21.Hash(raw);}
  System.Collections.IEnumerator LoadPlainProductVariablePhasesR1330(PlainProductDataPlanR1320 p){
   PlainProductNeedR1320(plainProductSessionR1320&&plainProductLoadingR1320&&plainVariablePhasesR1330==null&&plainVariableBanksR1330.Count==0&&plainVariableSpritesR1330.Count==0,"fresh Plain phase transaction required");plainVariablePhasesR1330=p.variablePhases;PlainVariablePhaseResidentPagesR1330=PlainVariablePhaseDrawsR1330=PlainVariablePhaseMissingStateHoldsR1330=0;
   foreach(var bank in plainVariablePhasesR1330.rows){if(!Array.Exists(p.rules,r=>r.id==bank.npc))continue;plainVariableBanksR1330.Add(bank.npc,bank);var pages=new Dictionary<string,Texture2D>(StringComparer.Ordinal);var pageFactories=new Dictionary<string,Func<Texture2D>>(StringComparer.Ordinal);
    foreach(var page in bank.textures){PlainProductNeedR1320(page.width<=SystemInfo.maxTextureSize&&page.height<=SystemInfo.maxTextureSize,"device cannot preserve original phase page");Func<Texture2D> openPage=()=>{if(!pages.TryGetValue(page.textureKey,out var texture)){texture=LoadPlainProductTextureR1320(page.file,page.width,page.height,"Plain original variable phase / "+bank.npc+" / "+page.textureKey);PlainProductNeedR1320(PlainPhaseRgbaR1330(texture)==page.rgbaSha256,"original atlas page bare RGBA changed");texture.Apply(false,true);texture=CombatMachineArtV40.ProductPage(bank.npc,texture);pages.Add(page.textureKey,texture);PlainVariablePhaseResidentPagesR1330++;}return texture;};pageFactories.Add(page.textureKey,openPage);if(!DeferMonsterPixelCarrierV53(bank.kind))openPage();yield return null;}
    foreach(var motion in bank.motions)foreach(var key in motion.frameKeys){plainVariableSpritesR1330.Register(PlainPhaseKeyR1330(bank.npc,key.poseID),DeferMonsterPixelCarrierV53(bank.kind),()=>{var r=key.rect;var texture=pageFactories[key.textureKey]();var sprite=Sprite.Create(texture,new Rect(r[0],texture.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);plainProductOwnedR1320.Add(sprite);sprite.name=bank.npc+"/"+key.poseID;return sprite;});}
   }
   PlainProductNeedR1320(plainVariableBanksR1330.Count==PlainVariablePhaseDataR1330.RouteCount(plainProductRouteR1320),"current Plain phase banks incomplete");
  }
  void BindPlainProductVariablePhaseModelR1330(){PlainProductNeedR1320(plainVariablePhasesR1330!=null&&ReferenceEquals(game,plainProductModelR1320)&&game.PlainOriginalSourceBoundR1258,"owned Plain initial-state content required");plainVariablePhaseModelR1330=game;}
  bool TryRenderPlainProductVariablePhaseR1330(Enemy e,SpriteRenderer renderer){
   if(!PlainProductSessionActiveR1320||e==null||!PlainVariablePhaseDataR1330.IsNpc(e.sourceNPC))return false;PlainVariablePhaseBankR1330 bank;
   PlainProductNeedR1320(ReferenceEquals(game,plainVariablePhaseModelR1330)&&game.PlainOriginalSourceBoundR1258&&game.enemies.Contains(e)&&game.CurrentRoom.id==plainProductRouteR1320+"-"+game.room&&plainVariableBanksR1330.TryGetValue(e.sourceNPC,out bank)&&renderer&&e.kind==bank.kind&&e.visualScale==1,"unbound current Plain variable-phase actor");bank=plainVariableBanksR1330[e.sourceNPC];
   // SHOCK_STICK_DYING_STATE_START explicitly calls SetShow(false).
   if(e.sourceNPC=="NUI_SHOCK_STICK"&&e.hp<=0){renderer.enabled=false;renderer.sprite=null;return true;}
   string displayState=e.npcState;float displayTime=e.npcTime;bool physical;bool originalPose=!string.IsNullOrEmpty(displayState),visibilityAdapter=false,previousPose;
   if(!originalPose)originalPose=game.TryPlainGuardianLethalPoseV35(e,out displayState,out displayTime);
   if(!originalPose){originalPose=TryOriginalWholeReactionV45(e,out displayState,out displayTime);}
   if(!originalPose){originalPose=plainReactionPoseMapR1473!=null&&plainReactionPoseMapR1473.TrySelect(e,out displayState,out displayTime,out physical);
    if(!originalPose){visibilityAdapter=plainReactionVisibilityCacheR1475!=null&&plainReactionVisibilityCacheR1475.TrySelect(e,game,bank,out displayState,out displayTime,out previousPose);if(!visibilityAdapter&&!plainVisibleHurtPoseV44.TryHold(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;PlainVariablePhaseMissingStateHoldsR1330++;return true;}}
   }
   var state=Array.Find(bank.stateDisplayBindingsOnly,x=>x.state==displayState);PlainProductNeedR1320(state!=null&&!float.IsNaN(displayTime)&&!float.IsInfinity(displayTime)&&displayTime>=0,"selected own source pose/time absent; finite WAIT visibility adaptation is explicit");var motion=Array.Find(bank.motions,x=>x.animation==state.originalSourceAnimation);
   plainVisibleHurtPoseV44.Remember(e,displayState,displayTime);
   if(originalPose&&plainReactionVisibilityCacheR1475!=null)plainReactionVisibilityCacheR1475.RememberOriginal(e,game,bank,displayState,displayTime);
   // The committed model state owns nextState and hit-pause. A renderer never
   // advances that state. Source motion keys remain original double times.
   // Actual/mapped poses use the model clock; explicit visibility adaptation
   // freezes this actor's remembered own pose/time, or its verified WAIT0.
   double clock=(double)displayTime*(double)(float)state.originalSourceAnimSpeed;if(state.originalLoop)clock%=motion.sourceMotionDuration;else clock=Math.Min(clock,motion.sourceMotionDuration);
   RecordMonsterPixelSourceV53(e,renderer,"Plain/VariablePhase",displayState,motion.animation,(float)clock,e.facing<0?-1:1,e.sourceNPC==NasodSprayRigV35.Plain&&displayState=="NASOD_SPRAY_DAMAGE_FLY_FRONT"?-45:0);
   int lo=0,hi=motion.frameKeys.Length;while(lo+1<hi){int mid=lo+(hi-lo)/2;if(motion.frameKeys[mid].originalTime<=clock)lo=mid;else hi=mid;}var key=motion.frameKeys[lo];
   renderer.enabled=true;renderer.sprite=plainVariableSpritesR1330[PlainPhaseKeyR1330(bank.npc,key.poseID)];renderer.flipX=e.facing<0;renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
   // Original scale is baked once. One full-image mirror also mirrors the
   // signed anchor offset once. Guardian referenceY is removed by its fixed
   // anchor; no Waitfloor restoration, per-pose fit or actor mutation occurs.
   float sign=e.facing<0?-1:1;var p=RenderEnemyPosition(e);renderer.transform.localPosition=new Vector3(p.x+(float)(key.rect[2]*.5-key.anchor[0])*sign,p.y+(float)(key.anchor[1]-key.rect[3]*.5),0);
   RecordPlainAuthoredSelectionV35(e,renderer,bank,state,motion,clock,key.poseID);
   PlainVariablePhaseDrawsR1330++;return true;
  }
  void ClearPlainProductVariablePhasesR1330(){DisposePlainElementDrawingsV35();plainVariableBanksR1330.Clear();plainVariableSpritesR1330.Clear();plainVariablePhasesR1330=null;plainVariablePhaseModelR1330=null;PlainVariablePhaseResidentPagesR1330=0;}
 }
}
