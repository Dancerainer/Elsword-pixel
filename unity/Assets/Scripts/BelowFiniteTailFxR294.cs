using System;
using System.Collections.Generic;
namespace ElswordPixel {
 // Source-constrained finite 2D display, not native particle ABI equivalence.
 public static class BelowFiniteTailFxR294 {
  public const string Schema="R294_BELOW_TWO_TAIL_FINITE_2D",Adapter="R294_FINITE_TAIL_2D",ChildAdapter="R294_TRACE_CHILD_2D";
  public const string Tail="Major/ExplodeTail",Nasod="Major/Nasod_ExplodeTail",Smoke="Major/ExplodeTailSmoke";
  public const double ChildLife=.6;
  public static bool Finite(double v){return !double.IsNaN(v)&&!double.IsInfinity(v);}
  static void Need(bool ok,string why){if(!ok)throw new ArgumentException("R294 Below finite Tail: "+why);}
  public static int Template(string key,string adapter,string blob,string declaration,string mesh){
   if(key==Tail){Need(adapter==Adapter&&blob=="774cd2295da39c722998df814cf7cb42a14e28f20f4b17be05a4ddbb0962cedd"&&declaration=="de01fe056affdbebb46b0e237aa94ab85468d945984bc06c0945bd043631ee09"&&string.IsNullOrEmpty(mesh),"Exact Tail source");return 1;}
   if(key==Nasod){Need(adapter==Adapter&&blob=="94f4351e45e9106246ceb85bd9b556536defcb99db59f1cd1f533b7431de6fc4"&&declaration=="24602cb8b00c67a36b5a1b21a26e657fc009da9e9930e7a0a015063f85757c5d"&&string.IsNullOrEmpty(mesh),"Exact Nasod 2.5s/cap70/40-to20 source");return 2;}
   if(key==Smoke){Need(adapter==ChildAdapter&&blob=="f3c4741196e3bdf9a71abfda20015a005a2993d28fb17daa3cbb6831f40ae0f9"&&declaration=="09f77de9f770388f7f0771d5c2ebb8fc5b1f1914319882b17f45019c511b811a"&&string.IsNullOrEmpty(mesh),"Exact shared trace smoke and explicit defaults");return 3;}
   Need(string.IsNullOrEmpty(adapter),"No generic finite admission");return 0;
  }
  public static int Field(string effect,string field,string particle,string position,string bone,string eventType,int count){
   if(particle!=Tail&&particle!=Nasod&&particle!=Smoke)return 0;
   Need(string.IsNullOrEmpty(bone)&&count==3,"Exact bone-free count3");
   if(particle==Tail){Need(effect=="CANNONFORT_PARASITE_CANNON_MISSILE"&&field=="HIT_PARTICLE7"&&eventType=="hit"&&position=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_IMPACT_POINT","Exact Below impact Tail");return 1;}
   Need(particle==Nasod&&effect=="NASOD_MONSTER_EXPLOSION"&&field=="START_PARTICLE1"&&eventType=="spawn"&&position=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_POS","Exact Below Nasod START; smoke is not a gameplay field");return 2;
  }
  public static double ParentLife(int kind){Need(kind==1||kind==2,"Parent kind");return kind==1?2:2.5;}
  public static double TraceEnd(int kind){return ParentLife(kind)+ChildLife;}
  public static int ParentCapacity(int kind){return ParentLife(kind)==2?50:70;}
  public static int TraceBirthCount(int kind,double rate){Need(Finite(rate)&&rate>=20&&rate<=40,"Source smoke rate20..40");return (int)Math.Ceiling(ParentLife(kind)*rate);}
  public static double TraceBirth(int kind,int index,double rate){Need(index>=0&&index<TraceBirthCount(kind,rate),"Birth inside half-open parent lifetime");return index/rate;}
  public static bool TraceVisible(int kind,double age,int index,double rate){Need(Finite(age),"Finite age");double a=age-TraceBirth(kind,index,rate);return a>=0&&a<ChildLife;}
  public static int ChildSeed(int seed,int index){Need(index>=0,"Child index");return unchecked(seed+104729+index*179);}
  public static double ConstantAxis(double origin,double radius,double velocity,double gravity,double age,double unit,int facing,bool xAxis,bool useLand,double land){
   Need(Finite(origin)&&Finite(radius)&&Finite(velocity)&&Finite(gravity)&&Finite(age)&&age>=0&&Finite(unit)&&unit>0&&(facing==1||facing==-1)&&(!useLand||Finite(land)),"Finite source trajectory");double offset=(radius+velocity*age+.5*gravity*age*age)*unit;if(xAxis)offset*=facing;double result=origin+offset;return useLand?Math.Max(result,land):result;
  }
  public static bool ValidSnapshot(bool committed,int sequence,int serial,int facing,double started,double now,double x,double y,double depth,double unit,bool hasLand,double land){return committed&&sequence>0&&serial>0&&(facing==1||facing==-1)&&Finite(started)&&started>=0&&Finite(now)&&now>=0&&Finite(x)&&Finite(y)&&Finite(depth)&&Finite(unit)&&unit>0&&hasLand&&Finite(land);}
 }
 public sealed class BelowFiniteTailInstanceR294 {
  public readonly string identity,particle;public readonly int kind,sequence,serial,facing,seed,count=3;public readonly double started,x,y,depth,unit,land;
  internal BelowFiniteTailInstanceR294(string id,string particle,int kind,int sequence,int serial,int facing,int seed,double started,double x,double y,double depth,double unit,double land){identity=id;this.particle=particle;this.kind=kind;this.sequence=sequence;this.serial=serial;this.facing=facing;this.seed=seed;this.started=started;this.x=x;this.y=y;this.depth=depth;this.unit=unit;this.land=land;}
  public double Age(double now){return now-started;}
  public bool Expired(double now){return Age(now)>=BelowFiniteTailFxR294.TraceEnd(kind);}
 }
 public sealed class BelowFiniteTailClockR294 {
  readonly List<BelowFiniteTailInstanceR294> rows=new List<BelowFiniteTailInstanceR294>();readonly HashSet<string> active=new HashSet<string>(StringComparer.Ordinal);
  public IList<BelowFiniteTailInstanceR294> Instances{get{return rows.AsReadOnly();}}public int Count{get{return rows.Count;}}
  public bool TryStart(string effect,string field,string particle,string position,string bone,string eventType,int count,bool committed,int sequence,int serial,int facing,int seed,double started,double now,double x,double y,double depth,double unit,bool hasLand,double land){
   int kind=BelowFiniteTailFxR294.Field(effect,field,particle,position,bone,eventType,count);if(kind==0)throw new ArgumentException("R294 unrelated field");if(!BelowFiniteTailFxR294.ValidSnapshot(committed,sequence,serial,facing,started,now,x,y,depth,unit,hasLand,land))throw new ArgumentException("R294 missing committed value snapshot");string id=sequence+"/"+serial+"/"+effect+"/"+field;var row=new BelowFiniteTailInstanceR294(id,particle,kind,sequence,serial,facing,seed,started,x,y,depth,unit,land);if(row.Expired(now)||!active.Add(id))return false;rows.Add(row);return true;
  }
  public void Retire(double now){if(!BelowFiniteTailFxR294.Finite(now))throw new ArgumentException("R294 finite current time");for(int i=rows.Count-1;i>=0;i--)if(rows[i].Expired(now)){active.Remove(rows[i].identity);rows.RemoveAt(i);}}
  public void Clear(){rows.Clear();active.Clear();}
 }
}
