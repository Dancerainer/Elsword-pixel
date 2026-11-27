using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  readonly CoreHardFiniteFxClockR276 coreHardFiniteFxR276=new CoreHardFiniteFxClockR276();
  public int CoreHardFiniteFxLiveR276{get{return coreHardFiniteFxR276.Count;}}
  public int CoreHardFiniteFxMeshDrawsR276{get;private set;}
  public int CoreHardFiniteFxTraceDrawsR276{get;private set;}
  static int CoreHardFiniteTemplateR276(CoreHardClosureVisualParticleV21 p){return CoreHardFiniteFxR276.Template(p.key,p.finiteAdapter,p.particleJsonSha256,p.sourceTextSha256,p.mesh,p.sourceTriggerCountLiteral);}
  static void ValidateCoreHardFiniteDocumentR276(CoreHardClosureVisualDocumentV21 d,OutskirtsNpcMeshManifest meshes){
   CoreHardClosureVisualRequireV21(d!=null&&d.finiteFxSchema==CoreHardFiniteFxR276.Schema&&d.particles!=null&&d.particles.Length==30&&meshes!=null&&meshes.components!=null&&meshes.components.Length==15,"R276 exact30 templates/15 components");
   var seen=new HashSet<string>(StringComparer.Ordinal);var kinds=new HashSet<int>();
   foreach(var p in d.particles){CoreHardClosureVisualRequireV21(p!=null&&seen.Add(p.key),"R276 unique template");int kind=CoreHardFiniteTemplateR276(p);if(kind>0)CoreHardClosureVisualRequireV21(kinds.Add(kind)&&p.supported&&string.IsNullOrEmpty(p.holdReason),"R276 four exact source-backed templates");}
   CoreHardClosureVisualRequireV21(kinds.Count==4,"R276 complete mesh/parent/child group");int meteo=0,frozen=0,tail=0;seen.Clear();
   foreach(var f in d.effectParticles){int k=CoreHardFiniteFxR276.Field(f.effect,f.sourceField,f.particleKey,f.positionType,f.bone,f.eventType,f.count);if(k==0)continue;CoreHardClosureVisualRequireV21(f.supported&&!f.hasLocalEmitterWindow&&!f.localForceDieOnHit&&seen.Add(f.effect+"/"+f.sourceField),"R276 original finite source field only");if(k==1)meteo++;else if(k==2)frozen++;else tail++;}
   CoreHardClosureVisualRequireV21(meteo==5&&frozen==1&&tail==6,"R276 exact6 START and6 HIT fields");
   foreach(var f in d.effectPassiveParticles)CoreHardClosureVisualRequireV21(f.particleKey!=CoreHardFiniteFxR276.Meteo&&f.particleKey!=CoreHardFiniteFxR276.Frozen&&f.particleKey!=CoreHardFiniteFxR276.Tail&&f.particleKey!=CoreHardFiniteFxR276.Smoke,"R276 no passive or invented child field");
   foreach(string id in new[]{"particle-major-meteo-explosion03","particle-major-frozenmissile-explosion03"}){int count=0;foreach(var m in meshes.components)if(m.id==id){count++;CoreHardClosureVisualRequireV21(m.sourceTemplate==(id=="particle-major-meteo-explosion03"?"__Major_Meteo_Explosion03":"__Major_FrozenMissile_Explosion03")&&m.fps==60&&m.durationSeconds==1&&m.frames!=null&&m.frames.Length==60&&m.pixelsPerNativeUnit>0&&m.sourceBlend=="D3DBLEND_SRCALPHA"&&m.destinationBlend=="D3DBLEND_DESTALPHA","R276 exact own-template R274 component");}CoreHardClosureVisualRequireV21(count==1,"R276 exact one component per template");}
  }
  static void ValidateCoreHardFiniteMeshParticleR276(OutskirtsVisualParticle p,int kind){
   CoreHardClosureVisualRequireV21((kind==1||kind==2)&&p.particleType=="PT_MESH"&&p.billboardType=="BT_Y"&&p.triggerCount==(kind==1?1:0)&&p.triggerTime==0&&p.drawCount==(kind==1?2:1)&&p.capacity==(kind==1?5:1)&&p.lifetime!=null&&p.lifetime.Length==2&&p.lifetime[0]==1&&p.lifetime[1]==1&&p.rate!=null&&p.rate.Length==2&&p.rate[0]==(kind==1?200:100)&&p.rate[1]==p.rate[0]&&p.radius!=null&&p.radius.Length==6&&p.gravity!=null&&p.gravity.Length==3&&p.curves!=null&&p.curves.Length==3&&!p.useLand,"R276 original mesh values; legacy Frozen int0 never enters burst divisor");
  }
  bool CollectCoreHardFiniteFxR276(CoreHardClosureVisualEffectParticleV21 b,NpcPresentationEvent e,Vector3 point,float depth){
   int kind=CoreHardFiniteFxR276.Field(b.effect,b.sourceField,b.particleKey,b.positionType,b.bone,b.eventType,b.count);if(kind==0)return false;
   if(!CoreHardFiniteFxR276.ValidSnapshot(e.hasCommittedGeometry,e.sequence,e.serial,e.facing,e.time,game.elapsed,point.x,point.y,depth,.28f*e.scale,e.hasSourceLandY,e.sourceLandY,kind)){CoreHardClosureVisualHeldEventsV21++;return true;}
   // Value snapshot only. Parent gameplay lifetime, future direction and effect
   // removal do not shorten these finite source display instances.
   if(coreHardFiniteFxR276.TryStart(b.effect,b.sourceField,b.particleKey,b.positionType,b.bone,b.eventType,b.count,e.hasCommittedGeometry,e.sequence,e.serial,e.facing,unchecked(e.sequence*997+e.serial*193+CoreHardClosureVisualSeedV21(b.sourceField)),e.time,game.elapsed,point.x,point.y,depth,.28f*e.scale,e.hasSourceLandY,e.sourceLandY))CoreHardClosureVisualCreatedSequencesV21++;
   return true;
  }
  void ClearCoreHardFiniteFxR276(){coreHardFiniteFxR276.Clear();CoreHardFiniteFxMeshDrawsR276=CoreHardFiniteFxTraceDrawsR276=0;}
  CoreHardClosureVisualPointV21 CoreHardFinitePositionR276(OutskirtsVisualParticle p,CoreHardClosureVisualPointV21 origin,float age,int seed,int facing,float unit){
   var values=new double[3];for(int c=0;c<3;c++)values[c]=CoreHardFiniteFxR276.ConstantAxis(c==0?origin.point.x:c==1?origin.point.y:0,OutskirtsRange(p.radius,c,seed,3),OutskirtsCurve(p,"Velocity",c,0,seed,0),p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,c,seed,301):p.gravity[c],age,unit,facing,c==0,c==1&&p.useLand,origin.land);return new CoreHardClosureVisualPointV21{point=new Vector3((float)values[0],(float)values[1],0),depth=origin.depth+(float)values[2]/.28f,land=origin.land};
  }
  bool DrawCoreHardFinitePlaneR276(ref int used,CoreHardClosureVisualViewV21 view,float age,CoreHardClosureVisualPointV21 point,int seed,int facing,float unit){
   var p=view.data;float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)return false;
   var sprite=CoreHardClosureVisualSpriteV21(ref used,"Original CoreHard finite particle "+p.id);sprite.sprite=view.sprite;sprite.sharedMaterial=view.material;sprite.sortingOrder=44+p.layer-14;sprite.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);sprite.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*unit/sprite.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*unit/sprite.sprite.rect.height,1);sprite.transform.localRotation=Quaternion.Euler(0,0,facing*OutskirtsCurve(p,"Rotate",2,age,seed,0));sprite.transform.localPosition=point.point;npcEffectDepths[sprite.transform]=point.depth;CoreHardClosureVisualDrawFramesV21++;coreHardClosureVisualKindsV21.Add(p.id);return true;
  }
  void RenderCoreHardFiniteFxR276(ref int used){
   foreach(var row in coreHardFiniteFxR276.Instances){double elapsed=row.Age(game.elapsed);if(elapsed<0)continue;var view=coreHardClosureVisualViewsV21[row.particle];float unit=(float)row.unit;var origin=new CoreHardClosureVisualPointV21{point=new Vector3((float)row.x,(float)row.y,0),depth=(float)row.depth,land=(float)row.land};
    if(row.kind==1||row.kind==2){if(CoreHardFiniteFxR276.MeshFrame(elapsed)<0)continue;for(int i=0;i<row.count;i++)for(int pass=0;pass<CoreHardFiniteFxR276.DrawPasses(row.kind);pass++){CoreHardClosureVisualMeshV21(ref used,view.mesh,(float)elapsed,origin.point,origin.depth,row.facing,unit,0);CoreHardFiniteFxMeshDrawsR276++;}coreHardClosureVisualKindsV21.Add(row.particle);continue;}
    var child=coreHardClosureVisualViewsV21[CoreHardFiniteFxR276.Smoke];for(int parent=0;parent<row.count;parent++){int seed=unchecked(row.seed+parent*179);
     if(elapsed<CoreHardFiniteFxR276.ParentLife){var position=CoreHardFinitePositionR276(view.data,origin,(float)elapsed,seed,row.facing,unit);DrawCoreHardFinitePlaneR276(ref used,view,(float)elapsed,position,seed,row.facing,unit);}
     // Births are a finite deterministic schedule, not frame-delta emission.
     // Sampling at birth preserves parent trajectory when a render frame skips.
     double rate=OutskirtsRange(child.data.rate,0,seed,1);int visible=0;
     for(int index=CoreHardFiniteFxR276.TraceBirthCount(rate)-1;index>=0&&visible<child.data.capacity;index--){if(!CoreHardFiniteFxR276.TraceVisible(elapsed,index,rate))continue;double born=CoreHardFiniteFxR276.TraceBirth(index,rate);int childSeed=CoreHardFiniteFxR276.ChildSeed(seed,index);var anchor=CoreHardFinitePositionR276(view.data,origin,(float)born,seed,row.facing,unit);var position=CoreHardFinitePositionR276(child.data,anchor,(float)(elapsed-born),childSeed,row.facing,unit);if(DrawCoreHardFinitePlaneR276(ref used,child,(float)(elapsed-born),position,childSeed,row.facing,unit)){visible++;CoreHardFiniteFxTraceDrawsR276++;}}
    }
   }
  }
 }
}
