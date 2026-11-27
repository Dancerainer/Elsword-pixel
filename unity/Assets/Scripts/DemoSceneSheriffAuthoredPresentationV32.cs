using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string SheriffAtlasResourceV32="Pixel/feedback-v32/nasod-sheriff/atlas";
  sealed class SheriffSelectionV32 {internal SpriteRenderer carrier;internal Sprite originalSprite;internal SourceWholeSelectionV21 selected;internal int renderFrame;}
  sealed class SheriffViewV32 {internal SpriteRenderer carrier;internal SheriffAuthoredPartsV32 parts;}
  readonly Dictionary<Enemy,SheriffSelectionV32> sheriffSelectionsV32=new Dictionary<Enemy,SheriffSelectionV32>();
  readonly Dictionary<Enemy,SheriffViewV32> sheriffViewsV32=new Dictionary<Enemy,SheriffViewV32>();
  readonly HashSet<Enemy> sheriffSeenV32=new HashSet<Enemy>();readonly HashSet<Enemy> sheriffBlockedV32=new HashSet<Enemy>();readonly List<Enemy> sheriffStaleV32=new List<Enemy>();
  SheriffAuthoredPartsV32.Binding sheriffBindingV32;bool sheriffLoadAttemptedV32;
  static bool SheriffIdentityV32(Enemy e){return e!=null&&e.kind=="nasod-sheriff"&&e.sourceNPC=="NUI_NASOD_SHERIFF";}
  // Called by the EXISTING Factory reference renderer after actual selection,
  // sprite and signed source anchor have been assigned. Never reselect FSM/time.
  void RecordSheriffAuthoredSelectionV32(Enemy e,SpriteRenderer carrier,SourceWholeSelectionV21 selected){
   if(!FactoryProductSessionActiveR1179||!SheriffIdentityV32(e)||!carrier||!carrier.sprite||selected==null)return;
   sheriffSelectionsV32[e]=new SheriffSelectionV32{carrier=carrier,originalSprite=carrier.sprite,selected=selected,renderFrame=Time.frameCount};
  }
  bool SheriffAuthoredOwnsRendererV32(Renderer renderer){foreach(var v in sheriffViewsV32.Values)if(v.parts!=null&&v.parts.OwnsRenderer(renderer))return true;return false;}
  void ReleaseSheriffViewV32(Enemy e){SheriffViewV32 v;if(!sheriffViewsV32.TryGetValue(e,out v))return;sheriffViewsV32.Remove(e);if(v.parts!=null)v.parts.Dispose();}
  void DisposeSheriffAuthoredViewsV32(){foreach(var v in sheriffViewsV32.Values)if(v.parts!=null)v.parts.Dispose();sheriffViewsV32.Clear();sheriffSelectionsV32.Clear();sheriffSeenV32.Clear();sheriffBlockedV32.Clear();sheriffStaleV32.Clear();}
  void UpdateSheriffLabelTopV32(Enemy e,int index,SheriffAuthoredPartsV32 parts){
   if(e.hp<=0||index<0||index>=enemyBars.Count||!enemyBars[index]||!enemyBars[index].enabled)return;
   float top;if(!parts.TryVisibleTop(out top))return;
   // Preserve existing gauge/status visibility and projected X/Z. The old
   // 52px carrier cannot anchor labels above this source-ratio 65.98px rig.
   var bar=enemyBars[index].transform.position;bar.y=top+8;enemyBars[index].transform.position=bar;
   if(index<enemyDebuffViews.Count&&enemyDebuffViews[index]){var status=enemyDebuffViews[index].transform.position;status.y=top+20;enemyDebuffViews[index].transform.position=status;}
  }
  bool TrySheriffSourceFiringLineV32(Enemy e,SourceWholeSelectionV21 selected,Vector2 point,Vector3 projectedFoot,out float aboveFoot){
   aboveFoot=0;string motion=selected.State.Motion;if(motion!="ComboA_Ready"&&motion!="ComboA")return false;
   // Read the same already-validated original attachment track as the bullet
   // mechanism. The new arm follows its Y plane; the source projectile stays
   // untouched. A centered Small_Laser origin is deliberately not a muzzle X.
   var plan=factoryProductPlanR1179;var d=factorySheriffVisualV32;
   if(plan==null||plan.effects==null||d==null||!effectsRoot)throw new InvalidOperationException("Sheriff original firing display data missing");
   var binding=Array.Find(plan.effects.bindings,b=>b.npc=="NUI_NASOD_SHERIFF"&&b.state=="NASOD_SHERIFF_ATTACK_A");
   var track=binding==null?null:Array.Find(binding.tracks,t=>t.bone=="AirShip_Soldier_Gunner_Weapon");
   if(track==null||track.samples==null||track.samples.Length!=191)throw new InvalidOperationException("Sheriff original firing attachment track missing");
   float clock=motion=="ComboA_Ready"?0:selected.SourceMotionClock;
   float f=Mathf.Clamp(clock*60,0,track.samples.Length-1);int a=(int)f,bIndex=Math.Min(a+1,track.samples.Length-1);float blend=f-a;
   var p=track.samples[a];var q=track.samples[bIndex];float x=(p.x+(q.x-p.x)*blend)*e.visualScale,y=(p.y+(q.y-p.y)*blend)*e.visualScale,z=(p.z+(q.z-p.z)*blend)*e.visualScale;
   var world=effectsRoot.TransformPoint(new Vector3(point.x+e.facing*x,point.y+y,0));
   var projected=NativeWorldActive?ProjectNativePoint(world,e.nativeDepth+z/d.unit):world;
   aboveFoot=projected.y-projectedFoot.y;return true;
  }
  // Call after final original armor/burial and native projection in BOTH paths.
  // Factory uses OwnedSourceFramingR1111: this call must be outside !Owned guard.
  void UpdateSheriffAuthoredViewsV32(){
   if(!FactoryProductSessionActiveR1179||game==null||!actorsRoot||!worldRoot){DisposeSheriffAuthoredViewsV32();return;}
   bool legacy=false;foreach(var actor in game.enemies)if(SheriffIdentityV32(actor)&&!MonsterPixelOwnsActorV53(actor)){legacy=true;break;}if(!legacy){DisposeSheriffAuthoredViewsV32();return;}
   if(!sheriffLoadAttemptedV32){sheriffLoadAttemptedV32=true;var atlas=Resources.Load<Texture2D>(SheriffAtlasResourceV32);if(atlas)sheriffBindingV32=new SheriffAuthoredPartsV32.Binding(atlas);else Debug.LogWarning("SHERIFF_AUTHORED_ATLAS_MISSING_V32");}
   var binding=sheriffBindingV32;if(binding==null){DisposeSheriffAuthoredViewsV32();return;}sheriffSeenV32.Clear();
   for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
    var e=game.enemies[i];if(MonsterPixelOwnsActorV53(e)){ReleaseSheriffViewV32(e);continue;}if(!SheriffIdentityV32(e))continue;sheriffSeenV32.Add(e);if(sheriffBlockedV32.Contains(e))continue;
    try{SheriffSelectionV32 witnessed;var carrier=enemyViews[i];
     if(!sheriffSelectionsV32.TryGetValue(e,out witnessed)||witnessed.renderFrame!=Time.frameCount||witnessed.carrier!=carrier||!carrier||carrier.sprite!=witnessed.originalSprite){ReleaseSheriffViewV32(e);continue;}
     var selected=witnessed.selected;var s=selected.State;
     if(!SheriffAuthoredPoseV32.ValidateSource(s.State,s.Motion,s.MotionDuration,s.Speed,s.Loop)){ReleaseSheriffViewV32(e);continue;}
     SheriffViewV32 entry;if(sheriffViewsV32.TryGetValue(e,out entry)&&(entry.carrier!=carrier||entry.parts==null||!ReferenceEquals(entry.parts.BindingIdentity,binding))){ReleaseSheriffViewV32(e);entry=null;}
     var point=RenderEnemyPosition(e);var worldFoot=actorsRoot.TransformPoint(new Vector3(point.x,point.y,0));var projectedFoot=NativeWorldActive?ProjectNativePoint(worldFoot,e.nativeDepth):worldFoot;
     if(!sheriffViewsV32.TryGetValue(e,out entry)){entry=new SheriffViewV32{carrier=carrier,parts=new SheriffAuthoredPartsV32(carrier,binding,DestroyNativeWorldObject)};sheriffViewsV32.Add(e,entry);}
     float firingLine;bool hasFiringLine=TrySheriffSourceFiringLineV32(e,selected,point,projectedFoot,out firingLine);
     entry.parts.Apply(new SheriffAuthoredPartsV32.Frame{state=s.State,motion=s.Motion,sourceClock=selected.SourceMotionClock,sourceDuration=s.MotionDuration,hostClock=selected.ExistingNpcClock,facing=e.facing,hasSourceFiringLine=hasFiringLine,sourceFiringLineAboveFoot=firingLine,visible=selected.ApplySourceShow(e.active&&!e.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy)},projectedFoot);
     entry.parts.VisitRenderersR1461(r=>BindNativeSceneOcclusionR1461(r,worldFoot,e.nativeDepth));
     UpdateSheriffLabelTopV32(e,i,entry.parts);
    }catch(Exception ex){ReleaseSheriffViewV32(e);sheriffBlockedV32.Add(e);Debug.LogWarning("SHERIFF_AUTHORED_FALLBACK_V32 "+ex.Message);}
   }
   sheriffStaleV32.Clear();foreach(var e in sheriffViewsV32.Keys)if(!sheriffSeenV32.Contains(e))sheriffStaleV32.Add(e);foreach(var e in sheriffStaleV32)ReleaseSheriffViewV32(e);
   sheriffStaleV32.Clear();foreach(var e in sheriffBlockedV32)if(!sheriffSeenV32.Contains(e))sheriffStaleV32.Add(e);foreach(var e in sheriffStaleV32)sheriffBlockedV32.Remove(e);sheriffSelectionsV32.Clear();
  }
 }
}
