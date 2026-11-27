using System;
namespace ElswordPixel.CargoPresentationV21 {
 // Cargo-only missing source channels. Existing model moveX/DirSpeed/angle
 // remains the owner of X/Y lane travel; this cannot double-add its X motion.
 // Ordinary unreflected Bee needs nativeY-330; Wally missile needs nativeZ
 // -200 until.2 then+400 until.4. No native reflection behavior is asserted.
 public static class CargoFlightV21 {
  public const string SourceEffectsSha256=CargoFxFeedV21.ExactEffectsSha256;
  public static double[] AdditionalChannels(string effect,string owner,double from,double to,double ownerScale,bool reflected){if(Double.IsNaN(from)||Double.IsInfinity(from)||Double.IsNaN(to)||Double.IsInfinity(to)||from<0||to<from||ownerScale<=0||Double.IsNaN(ownerScale)||Double.IsInfinity(ownerScale))throw new ArgumentException("Invalid source flight interval");if(CargoFxFeedV21.OwnerForEffect(effect)!=owner)throw new ArgumentException("Wrong exact Cargo effect owner");if(reflected)return new double[]{0,0,0};if(effect=="DamageEffect_BEE_NASOD_ATTACK_A"){from=Math.Min(.2,from);to=Math.Min(.2,to);return new double[]{0,-330*.28*(to-from)*ownerScale,0};}if(effect=="DamageEffect_WALLY_9TH_ARMOR_ATTACK_C"){double a=Math.Max(0,Math.Min(.2,to)-Math.Min(.2,from));double b=Math.Max(0,Math.Min(.4,to)-Math.Max(.2,from));return new double[]{0,0,(-200*a+400*b)*.28*ownerScale};}return new double[]{0,0,0};}
  // Controlled component input only: source moveX samples have already
  // integrated X/DirSpeed. Renderer never calls this to advance live attacks.
  public static CargoLiveEffectV21 ControlledSnapshot(string id,string owner,int serial,int actorSerial,float age,float originX,float originY,float originZ,int facing,float laneAngle,float moveX,float lifetime,float land){double angle=laneAngle*Math.PI/180;var delta=AdditionalChannels(id,owner,0,age,1,false);return new CargoLiveEffectV21{id=id,serial=serial,age=age,lifetime=lifetime,owner=new CargoFxOwnerV21{npc=owner,serial=actorSerial},x=originX+(float)Math.Cos(angle)*moveX*facing,y=originY+(float)Math.Sin(angle)*moveX*facing+(float)delta[1],z=originZ+(float)delta[2],angle=laneAngle,facing=facing,hasSourceLandY=true,sourceLandY=land};}
 }
}
