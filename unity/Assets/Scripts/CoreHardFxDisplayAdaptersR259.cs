using System;

namespace ElswordPixel {
 // Two explicitly bounded 2D display adapters. This class has no Unity,
 // GameModel, state transition, particle emission or resource ownership API.
 public static class CoreHardFxDisplayAdaptersR259 {
  public const string RingBasis="FIXED_XY_SOURCE_XYZ_RZ_RY_RX_R259";
  public const string ArrowAnchor="EXACT_MISSILE_HELPER_AT_START_2D_R259";
  public static bool IsRingBasis(string key,string type,string billboard,string adapter,string normalizedSha,string sourceDeclarationSha,string rgbaSha){
   return key=="Major/ExplodeImpactRingRed"&&type=="PT_3D_PLANE"&&billboard=="BT_NONE"&&adapter==RingBasis&&normalizedSha=="80b0c931d579c714aa61fb17d6257217ec7736f64aea6a1963427efc9d104e1a"&&sourceDeclarationSha=="5edca278f9d260df20128a9a2449db11f2b400e9f57609b6a81e5d462197444b"&&rgbaSha=="eb2ed17ce805184b2580355af694644977b7141dba63d7ce09080a3243cb3043";
  }
  public static bool IsArrowAnchor(string effect,string field,string particle,string positionType,string bone,string eventType,int count,string adapter){
   return effect=="NASOD_KING_GUIDED_MISSILE"&&field=="START_PARTICLE0"&&particle=="Major/ArrowBackTick"&&positionType=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_UNIT_BONE"&&bone=="Dummy01_Missile"&&eventType=="spawn"&&count==7&&adapter==ArrowAnchor;
  }
  static bool Finite(double x){return !double.IsNaN(x)&&!double.IsInfinity(x);}
  public static bool ValidArrowSnapshot(bool committed,int serial,int facing,float ownerScale,float sx,float sy,float sz,float dx,float dy,bool hasLand,float land){
   return committed&&serial>0&&(facing==1||facing==-1)&&Finite(ownerScale)&&ownerScale>0&&Finite(sx)&&Finite(sy)&&Finite(sz)&&sx>0&&sy>0&&sz>0&&Finite(dx)&&Finite(dy)&&(dx!=0||dy!=0)&&hasLand&&Finite(land);
  }
  public static float ArrowAngleDegrees(float dirX,float dirY,int facing){
   if(!Finite(dirX)||!Finite(dirY)||(dirX==0&&dirY==0)||(facing!=1&&facing!=-1))throw new ArgumentException("Invalid committed Arrow direction");
   // Same conversion and facing convention as the existing NpcEffectVisual;
   // arguments are the committed event values, never the current live direction.
   return (float)(Math.Atan2(dirY,dirX)*180/Math.PI)-(facing<0?180:0);
  }
  // Fixed local XY plane, active right-handed rotations Rz(facing*Z)*Ry(Y)*Rx(X).
  // Source X and Y angles are retained. Z keeps the existing display-facing rule.
  // The adapter defines a 2D camera projection; native BT_NONE ABI is not asserted.
  public static double[] RingQuaternion(double sourceX,double sourceY,double sourceZ,int facing){
   if(!Finite(sourceX)||!Finite(sourceY)||!Finite(sourceZ)||(facing!=1&&facing!=-1))throw new ArgumentException("Invalid finite source XYZ/facing for exact ring display adapter");
   double factor=Math.PI/360d,x=sourceX*factor,y=sourceY*factor,z=sourceZ*facing*factor;
   double sx=Math.Sin(x),cx=Math.Cos(x),sy=Math.Sin(y),cy=Math.Cos(y),sz=Math.Sin(z),cz=Math.Cos(z);
   return new[]{sx*cy*cz-cx*sy*sz,cx*sy*cz+sx*cy*sz,cx*cy*sz-sx*sy*cz,cx*cy*cz+sx*sy*sz};
  }
 }
}
