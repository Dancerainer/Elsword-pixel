using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  readonly BelowFiniteTailClockR294 belowFiniteTailR294=new BelowFiniteTailClockR294();
  public int BelowFiniteTailLiveR294{get{return belowFiniteTailR294.Count;}}
  public int BelowFiniteTailParentDrawsR294{get;private set;}
  public int BelowFiniteTailChildDrawsR294{get;private set;}
  static int BelowFiniteTailTemplateR294(BelowVisualParticleV21 p){return BelowFiniteTailFxR294.Template(p.key,p.finiteAdapter,p.particleJsonSha256,p.sourceTextSha256,p.mesh);}
  public static void ValidateBelowFiniteTailDocumentR294(BelowVisualDocumentV21 d){
   BelowVisualRequireV21(d!=null&&d.finiteTailSchema==BelowFiniteTailFxR294.Schema&&d.particles!=null&&d.particles.Length==17&&d.effectParticles!=null&&d.effectParticles.Length==52&&d.effectPassiveParticles!=null&&d.effectPassiveParticles.Length==2,"R294 exact17 templates / original52+2 fields");
   var keys=new HashSet<string>(StringComparer.Ordinal);var kinds=new HashSet<int>();
   foreach(var p in d.particles){BelowVisualRequireV21(p!=null&&keys.Add(p.key),"R294 unique template");int k=BelowFiniteTailTemplateR294(p);if(k>0)BelowVisualRequireV21(kinds.Add(k)&&p.supported&&string.IsNullOrEmpty(p.holdReason)&&p.textureKey==(k==3?"textures/Arme_smoke-d106bc56cc.png":"textures/stone-5043309909.png")&&p.textureRgbaSha256==(k==3?"70fa92b257a7969676d8d5066372bcf16ca918ec28ec58a80aa3fb994b6b6a87":"89f9d44170a3320d9d8e2422015cc3fa1daac782399b54b3def98566e0d75e6c"),"R294 exact parent/child source and existing texture");}
   BelowVisualRequireV21(kinds.Count==3,"R294 complete two parents/shared child");kinds.Clear();
   foreach(var f in d.effectParticles){BelowVisualRequireV21(f!=null,"R294 null field");int k=BelowFiniteTailFxR294.Field(f.effect,f.sourceField,f.particleKey,f.positionType,f.bone,f.eventType,f.count);if(k>0)BelowVisualRequireV21(kinds.Add(k)&&f.supported,"R294 exact two source fields");}
   BelowVisualRequireV21(kinds.Count==2,"R294 both original Tail fields required");
   foreach(var f in d.effectPassiveParticles)BelowVisualRequireV21(f!=null&&f.particleKey!=BelowFiniteTailFxR294.Tail&&f.particleKey!=BelowFiniteTailFxR294.Nasod&&f.particleKey!=BelowFiniteTailFxR294.Smoke,"R294 no invented passive/final emitter");
   var arrow=Array.Find(d.effectParticles,f=>f.effect=="CANNONFORT_PARASITE_CANNON_MISSILE"&&f.sourceField=="START_PARTICLE0");BelowVisualRequireV21(arrow!=null&&!arrow.supported&&arrow.particleKey=="Major/ArrowBackTick"&&arrow.count==5&&arrow.positionType=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_UNIT_BONE"&&arrow.bone=="Dummy01_Missile"&&!string.IsNullOrEmpty(arrow.holdReason),"R294 retain Below Arrow count5/owner-bone HOLD");
  }
  bool CollectBelowFiniteTailR294(BelowVisualEffectParticleV21 b,NpcPresentationEvent e,Vector3 point,float depth){
   int k=BelowFiniteTailFxR294.Field(b.effect,b.sourceField,b.particleKey,b.positionType,b.bone,b.eventType,b.count);if(k==0)return false;
   if(!BelowFiniteTailFxR294.ValidSnapshot(e.hasCommittedGeometry,e.sequence,e.serial,e.facing,e.time,game.elapsed,point.x,point.y,depth,.28f*e.scale,e.hasSourceLandY,e.sourceLandY)){BelowVisualHeldEventsV21++;return true;}
   // Only copied submission values. No live parent/owner lookup, or gameplay writes.
   if(belowFiniteTailR294.TryStart(b.effect,b.sourceField,b.particleKey,b.positionType,b.bone,b.eventType,b.count,e.hasCommittedGeometry,e.sequence,e.serial,e.facing,unchecked(e.sequence*997+e.serial*193+BelowVisualSeedV21(b.sourceField)),e.time,game.elapsed,point.x,point.y,depth,.28f*e.scale,e.hasSourceLandY,e.sourceLandY))BelowVisualCreatedSequencesV21++;
   return true;
  }
  void ClearBelowFiniteTailR294(){belowFiniteTailR294.Clear();BelowFiniteTailParentDrawsR294=BelowFiniteTailChildDrawsR294=0;}
  BelowVisualPointV21 BelowFiniteTailPositionR294(OutskirtsVisualParticle p,BelowVisualPointV21 origin,float age,int seed,int facing,float unit){
   var values=new double[3];for(int c=0;c<3;c++)values[c]=BelowFiniteTailFxR294.ConstantAxis(c==0?origin.point.x:c==1?origin.point.y:0,OutskirtsRange(p.radius,c,seed,3),OutskirtsCurve(p,"Velocity",c,0,seed,0),p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,c,seed,301):p.gravity[c],age,unit,facing,c==0,c==1&&p.useLand,origin.land);return new BelowVisualPointV21{point=new Vector3((float)values[0],(float)values[1],0),depth=origin.depth+(float)values[2]/.28f,land=origin.land};
  }
  bool DrawBelowFiniteTailPlaneR294(ref int used,BelowVisualViewV21 view,float age,BelowVisualPointV21 point,int seed,int facing,float unit){
   var p=view.data;float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)return false;
   var sprite=BelowVisualSpriteV21(ref used,"Original Below finite particle "+p.id);sprite.sprite=view.sprite;sprite.sharedMaterial=view.material;sprite.sortingOrder=44+p.layer-14;sprite.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);sprite.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*unit/sprite.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*unit/sprite.sprite.rect.height,1);sprite.transform.localRotation=Quaternion.Euler(0,0,facing*OutskirtsCurve(p,"Rotate",2,age,seed,0));sprite.transform.localPosition=point.point;npcEffectDepths[sprite.transform]=point.depth;BelowVisualDrawFramesV21++;belowVisualKindsV21.Add(p.id);return true;
  }
  void RenderBelowFiniteTailR294(ref int used){
   foreach(var row in belowFiniteTailR294.Instances){double age=row.Age(game.elapsed);if(age<0)continue;var view=belowVisualViewsV21[row.particle];var child=belowVisualViewsV21[BelowFiniteTailFxR294.Smoke];float unit=(float)row.unit;var origin=new BelowVisualPointV21{point=new Vector3((float)row.x,(float)row.y,0),depth=(float)row.depth,land=(float)row.land};
    for(int parent=0;parent<row.count;parent++){int seed=unchecked(row.seed+parent*179);
     if(age<BelowFiniteTailFxR294.ParentLife(row.kind)){var point=BelowFiniteTailPositionR294(view.data,origin,(float)age,seed,row.facing,unit);if(DrawBelowFiniteTailPlaneR294(ref used,view,(float)age,point,seed,row.facing,unit))BelowFiniteTailParentDrawsR294++;}
     // Half-open parent births, independent .6s children. A late frame samples
     // the original parent trajectory at each birth, never a mutable live owner.
     double rate=OutskirtsRange(child.data.rate,0,seed,1);int visible=0;
     for(int index=BelowFiniteTailFxR294.TraceBirthCount(row.kind,rate)-1;index>=0&&visible<child.data.capacity;index--){if(!BelowFiniteTailFxR294.TraceVisible(row.kind,age,index,rate))continue;double born=BelowFiniteTailFxR294.TraceBirth(row.kind,index,rate);int childSeed=BelowFiniteTailFxR294.ChildSeed(seed,index);var anchor=BelowFiniteTailPositionR294(view.data,origin,(float)born,seed,row.facing,unit);var point=BelowFiniteTailPositionR294(child.data,anchor,(float)(age-born),childSeed,row.facing,unit);if(DrawBelowFiniteTailPlaneR294(ref used,child,(float)(age-born),point,childSeed,row.facing,unit)){visible++;BelowFiniteTailChildDrawsR294++;}}
    }
   }
  }
 }
}
