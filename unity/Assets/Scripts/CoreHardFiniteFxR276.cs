using System;
using System.Collections.Generic;

namespace ElswordPixel {
 // Finite 2D display policy only. The original source trigger literals and
 // normalized particle blobs are never used as a rewritten emission count.
 public static class CoreHardFiniteFxR276 {
  public const string Schema="R276_SOURCE_FIELD_2D_FINITE",FieldAdapter="R276_FINITE_FIELD_2D",ChildAdapter="R276_TRACE_CHILD_2D";
  public const string Meteo="Major/Meteo_Explosion03",Frozen="Major/FrozenMissile_Explosion03",Tail="Major/ExplodeTail",Smoke="Major/ExplodeTailSmoke";
  public const double MeshLife=1,ParentLife=2,ChildLife=.6,TraceEnd=2.6;
  public static bool Finite(double x){return !double.IsNaN(x)&&!double.IsInfinity(x);}
  static void Need(bool ok,string why){if(!ok)throw new ArgumentException("R276 finite FX: "+why);}
  public static int Template(string key,string adapter,string blob,string declaration,string mesh,string rawTrigger){
   if(key==Meteo){Need(adapter==FieldAdapter&&blob=="aabfc630940c5c8389d23b77792ba63ae084dc3b51a08410ebf6f78b5b36d810"&&declaration=="5445c0c3fcc16f624ff30d212b911f641ce63c017b1211d4ee4af9f1897ed428"&&mesh=="particle-major-meteo-explosion03"&&rawTrigger=="1","Meteo exact source and component");return 1;}
   if(key==Frozen){Need(adapter==FieldAdapter&&blob=="a6f401cdbe3c1a317427a6935feefbca7382f4a626b0e29c1b5e7af8a3ca716b"&&declaration=="399cafba104b567407ff13b731db521ae92cf325500121bdfc7377667fccfc77"&&mesh=="particle-major-frozenmissile-explosion03"&&rawTrigger=="0.100","Frozen original .100 and legacy int0 must stay distinct");return 2;}
   if(key==Tail){Need(adapter==FieldAdapter&&blob=="774cd2295da39c722998df814cf7cb42a14e28f20f4b17be05a4ddbb0962cedd"&&declaration=="de01fe056affdbebb46b0e237aa94ab85468d945984bc06c0945bd043631ee09"&&string.IsNullOrEmpty(mesh)&&string.IsNullOrEmpty(rawTrigger),"Tail exact parent source");return 3;}
   if(key==Smoke){Need(adapter==ChildAdapter&&blob=="f3c4741196e3bdf9a71abfda20015a005a2993d28fb17daa3cbb6831f40ae0f9"&&declaration=="09f77de9f770388f7f0771d5c2ebb8fc5b1f1914319882b17f45019c511b811a"&&string.IsNullOrEmpty(mesh)&&string.IsNullOrEmpty(rawTrigger),"Smoke exact source-derived child and explicit defaults");return 4;}
   Need(string.IsNullOrEmpty(adapter)&&string.IsNullOrEmpty(rawTrigger),"No generic finite or zero-trigger admission");return 0;
  }
  public static bool Flame(string effect){return effect=="NASOD_KING_FLAME_EDGE0"||effect=="NASOD_KING_FLAME_EDGE1"||effect=="NASOD_KING_FLAME_EDGE2"||effect=="NASOD_KING_FLAME_EDGE3"||effect=="NASOD_KING_FLAME_EDGE4";}
  public static int Field(string effect,string field,string particle,string position,string bone,string eventType,int count){
   if(particle!=Meteo&&particle!=Frozen&&particle!=Tail&&particle!=Smoke)return 0;
   Need(string.IsNullOrEmpty(bone),"Finite source fields have no bone");
   if(particle==Meteo){Need(Flame(effect)&&field=="START_PARTICLE2"&&position=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_POS"&&eventType=="spawn"&&count==5,"Exact five Meteo fields/count5");return 1;}
   if(particle==Frozen){Need(effect=="FROZENMISSILE_EXPLOSION_NONDAMAGE"&&field=="START_PARTICLE2"&&position=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_POS"&&eventType=="spawn"&&count==1,"Exact Frozen field/count1");return 2;}
   Need(particle==Tail&&(Flame(effect)||effect=="NASOD_KING_GUIDED_MISSILE")&&field=="HIT_PARTICLE7"&&position=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_IMPACT_POINT"&&eventType=="hit"&&count==3,"Exact six accepted HIT fields; child is never a new gameplay field");return 3;
  }
  public static bool ValidSnapshot(bool committed,int sequence,int serial,int facing,double started,double now,double x,double y,double depth,double unit,bool hasLand,double land,int kind){
   return committed&&sequence>0&&serial>0&&(facing==1||facing==-1)&&Finite(started)&&started>=0&&Finite(now)&&now>=0&&Finite(x)&&Finite(y)&&Finite(depth)&&Finite(unit)&&unit>0&&(kind!=3||hasLand&&Finite(land));
  }
  public static int Multiplicity(int kind){Need(kind>=1&&kind<=3,"Instance kind");return kind==1?5:kind==2?1:3;}
  public static int DrawPasses(int kind){Need(kind==1||kind==2,"Mesh draw pass kind");return kind==1?2:1;}
  public static int MeshFrame(double age){Need(Finite(age),"Finite mesh age");return age<0||age>=MeshLife?-1:Math.Min(59,(int)Math.Floor(age*60));}
  public static int TraceBirthCount(double rate){Need(Finite(rate)&&rate>=20&&rate<=40,"Source trace rate20..40");return (int)Math.Ceiling(ParentLife*rate);}
  public static double TraceBirth(int index,double rate){Need(index>=0&&index<TraceBirthCount(rate),"Trace birth ordinal within finite parent");return index/rate;}
  public static bool TraceVisible(double age,int index,double rate){Need(Finite(age),"Finite trace age");double a=age-TraceBirth(index,rate);return a>=0&&a<ChildLife;}
  public static int ChildSeed(int parentSeed,int birthIndex){Need(birthIndex>=0,"Child seed ordinal");return unchecked(parentSeed+104729+birthIndex*179);}
  // Both admitted Tail templates declare only an Initial Velocity. This is its
  // analytic integral, sampled at each child's scheduled birth, with no frame dt.
  public static double ConstantAxis(double origin,double radius,double velocity,double gravity,double age,double unit,int facing,bool xAxis,bool useLand,double land){
   Need(Finite(origin)&&Finite(radius)&&Finite(velocity)&&Finite(gravity)&&Finite(age)&&age>=0&&Finite(unit)&&unit>0&&(facing==1||facing==-1)&&(!useLand||Finite(land)),"Finite constant source trajectory");
   double offset=(radius+velocity*age+.5*gravity*age*age)*unit;if(xAxis)offset*=facing;double result=origin+offset;return useLand?Math.Max(result,land):result;
  }
 }
 public sealed class CoreHardFiniteFxInstanceR276 {
  public readonly string identity,particle;public readonly int kind,sequence,serial,facing,seed,count;
  public readonly double started,x,y,depth,unit,land;
  internal CoreHardFiniteFxInstanceR276(string identity,string particle,int kind,int sequence,int serial,int facing,int seed,double started,double x,double y,double depth,double unit,double land){this.identity=identity;this.particle=particle;this.kind=kind;this.sequence=sequence;this.serial=serial;this.facing=facing;this.seed=seed;this.started=started;this.x=x;this.y=y;this.depth=depth;this.unit=unit;this.land=land;count=CoreHardFiniteFxR276.Multiplicity(kind);}
  public double Age(double now){return now-started;}
  public bool Expired(double now){return Age(now)>=(kind==3?CoreHardFiniteFxR276.TraceEnd:CoreHardFiniteFxR276.MeshLife);}
 }
 public sealed class CoreHardFiniteFxClockR276 {
  readonly List<CoreHardFiniteFxInstanceR276> rows=new List<CoreHardFiniteFxInstanceR276>();readonly HashSet<string> active=new HashSet<string>(StringComparer.Ordinal);
  public IList<CoreHardFiniteFxInstanceR276> Instances{get{return rows.AsReadOnly();}}
  public int Count{get{return rows.Count;}}
  public bool TryStart(string effect,string field,string particle,string position,string bone,string eventType,int count,bool committed,int sequence,int serial,int facing,int seed,double started,double now,double x,double y,double depth,double unit,bool hasLand,double land){
   int kind=CoreHardFiniteFxR276.Field(effect,field,particle,position,bone,eventType,count);if(kind==0)throw new ArgumentException("R276 unrelated finite field");
   if(!CoreHardFiniteFxR276.ValidSnapshot(committed,sequence,serial,facing,started,now,x,y,depth,unit,hasLand,land,kind))throw new ArgumentException("R276 missing committed source snapshot");
   string id=sequence+"/"+serial+"/"+effect+"/"+field;var row=new CoreHardFiniteFxInstanceR276(id,particle,kind,sequence,serial,facing,seed,started,x,y,depth,unit,land);
   if(row.Expired(now)||!active.Add(id))return false;rows.Add(row);return true;
  }
  public void Retire(double now){if(!CoreHardFiniteFxR276.Finite(now))throw new ArgumentException("R276 finite current clock");for(int i=rows.Count-1;i>=0;i--)if(rows[i].Expired(now)){active.Remove(rows[i].identity);rows.RemoveAt(i);}}
  public void Clear(){rows.Clear();active.Clear();}
 }
}
