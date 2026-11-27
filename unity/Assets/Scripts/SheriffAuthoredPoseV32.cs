using System;
using System.Collections.Generic;
namespace ElswordPixel {
 // Cosmetic authored transforms sampled from the original carrier selection.
 // No Enemy, Unity, event emission, playback advance or gameplay ownership.
 public static class SheriffAuthoredPoseV32 {
  public const float LogicalStandingHeight=65.9752999f;
  public const float AuthoredStandingHeight=800.24f,FootY=795.52f,UnitsPerArtPixel=0.0824443915762f;
  public const string AtlasSha256="a6c59483aee79da49f78950ce1f8f96accdca440e5e2f02ca453007b5688ae7d";
  public sealed class Layout {
   public readonly string name; public readonly int x,y,width,height,order;
   public readonly float pivotX,pivotY,restX,restY,scale; internal readonly float[] hull;
   internal Layout(string n,int x0,int y0,int w,int h,float px,float py,float ax,float ay,float s,int z,float[] support){name=n;x=x0;y=y0;width=w;height=h;pivotX=px;pivotY=py;restX=ax;restY=ay;scale=s;order=z;hull=support;}
  }
  public struct PartPose {public float x,y,rotation;}
  sealed class Clip {internal readonly bool ground;internal readonly float[][] keys;internal Clip(bool g,float[][] k){ground=g;keys=k;}}
  sealed class SourceState {internal readonly string motion;internal readonly float duration;internal readonly bool loop;internal SourceState(string m,float d,bool l){motion=m;duration=d;loop=l;}}
  static readonly Layout[] parts={new Layout("head",28,34,463,452,242f,400f,0f,320f,0.82f,50,new float[]{8f,279f,63f,175f,141f,77f,170f,46f,171f,45f,200f,21f,242f,5f,260f,4f,301f,4f,308f,5f,378f,34f,393f,48f,435f,95f,451f,127f,451f,224f,331f,381f,291f,423f,249f,447f,176f,447f,87f,430f,62f,415f,37f,398f,23f,381f,8f,359f}),
new Layout("torso",571,127,288,347,148f,45f,0f,325f,0.95f,40,new float[]{4f,48f,5f,46f,22f,30f,86f,5f,186f,5f,220f,17f,239f,28f,277f,55f,278f,56f,279f,68f,279f,291f,266f,325f,252f,342f,64f,342f,36f,338f,20f,325f,19f,323f,4f,290f}),
new Layout("free-arm",129,558,260,399,175f,77f,-115f,350f,0.6f,20,new float[]{4f,190f,17f,157f,100f,33f,101f,32f,135f,4f,196f,4f,217f,17f,240f,32f,241f,33f,255f,57f,255f,118f,226f,319f,217f,338f,205f,353f,179f,380f,148f,394f,97f,394f,65f,366f,31f,323f,18f,290f,4f,254f}),
new Layout("gun-arm",921,164,599,292,81f,70f,105f,353f,0.65f,60,new float[]{4f,126f,26f,32f,41f,17f,56f,5f,82f,4f,86f,4f,119f,5f,577f,65f,578f,66f,592f,90f,592f,136f,192f,272f,48f,287f,5f,287f,4f,285f}),
new Layout("left-leg",560,547,278,419,203f,60f,-75f,565f,0.65f,10,new float[]{4f,367f,17f,328f,71f,173f,141f,35f,155f,17f,171f,5f,236f,5f,257f,19f,273f,42f,273f,94f,242f,413f,241f,414f,5f,414f,4f,413f}),
new Layout("right-leg",1159,559,283,407,107f,63f,78f,565f,0.68f,11,new float[]{4f,350f,57f,35f,72f,19f,92f,5f,150f,4f,171f,19f,187f,34f,221f,133f,278f,353f,278f,401f,276f,402f,5f,402f,4f,401f})};
  static readonly Dictionary<string,Clip> clips=new Dictionary<string,Clip>(StringComparer.Ordinal){{"Wait",new Clip(true,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.25f,0f,0f,-4f,0f,-1f,2f,17f,0f,0f,0f,0f,0f},new float[]{0.5f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.75f,0f,0f,3f,0f,1f,-2f,19f,0f,0f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"Walk",new Clip(true,new float[][]{new float[]{0f,0f,0f,-0f,0f,-0f,-0f,18f,0f,-0f,0f,0f,0f},new float[]{0.125f,0f,0f,-7f,1.41421356f,-0.707106781f,-11.3137085f,20.8284271f,9.19238816f,-9.19238816f,19.7989899f,0f,0f},new float[]{0.25f,0f,0f,-8.57252759e-16f,2f,-1f,-16f,22f,13f,-13f,28f,0f,0f},new float[]{0.375f,0f,0f,-7f,1.41421356f,-0.707106781f,-11.3137085f,20.8284271f,9.19238816f,-9.19238816f,19.7989899f,0f,0f},new float[]{0.5f,0f,0f,-1.71450552e-15f,2.4492936e-16f,-1.2246468e-16f,-1.95943488e-15f,18f,1.59204084e-15f,-1.59204084e-15f,3.42901104e-15f,0f,0f},new float[]{0.625f,0f,0f,-7f,-1.41421356f,0.707106781f,11.3137085f,15.1715729f,-9.19238816f,9.19238816f,0f,19.7989899f,0f},new float[]{0.75f,0f,0f,-2.57175828e-15f,-2f,1f,16f,14f,-13f,13f,0f,28f,0f},new float[]{0.875f,0f,0f,-7f,-1.41421356f,0.707106781f,11.3137085f,15.1715729f,-9.19238816f,9.19238816f,0f,19.7989899f,0f},new float[]{1f,0f,0f,-3.42901104e-15f,-4.8985872e-16f,2.4492936e-16f,3.91886976e-15f,18f,-3.18408168e-15f,3.18408168e-15f,0f,6.85802208e-15f,0f}})},
{"WaitStart",new Clip(true,new float[][]{new float[]{0f,0f,0f,18f,0f,9f,-9f,30f,0f,0f,0f,0f,0f},new float[]{0.22f,0f,0f,0f,0f,-7f,0f,25f,0f,0f,0f,0f,0f},new float[]{0.5f,0f,0f,0f,3f,6f,0f,20f,0f,0f,0f,0f,0f},new float[]{0.8f,0f,0f,0f,0f,-2f,0f,17f,0f,0f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"ComboA_Ready",new Clip(true,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.2f,0f,0f,8f,0f,3f,0f,24f,0f,0f,0f,0f,0f},new float[]{0.58f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.84f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{1f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f}})},
{"ComboA",new Clip(true,new float[][]{new float[]{0f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.173684206f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.187894732f,0f,-27f,10f,-8f,2f,-31f,-13f,-5f,4f,0f,0f,0f},new float[]{0.255789467f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.388421043f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.402631569f,0f,-27f,10f,-8f,2f,-31f,-13f,-5f,4f,0f,0f,0f},new float[]{0.470526304f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.581052617f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.595263143f,0f,-27f,10f,-8f,2f,-31f,-13f,-5f,4f,0f,0f,0f},new float[]{0.663157878f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.79f,0f,-6f,8f,-3f,-3f,-28f,-4f,-5f,4f,0f,0f,0f},new float[]{0.93f,0f,0f,0f,0f,0f,-8f,12f,0f,0f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"JumpUp",new Clip(false,new float[][]{new float[]{0f,0f,0f,-6f,-6f,2f,-18f,7f,-17f,19f,28f,42f,0f},new float[]{0.25f,0f,0f,-6f,-6f,2f,-18f,7f,-15f,17f,28f,42f,0f},new float[]{0.5f,0f,0f,-6f,-6f,2f,-18f,7f,-17f,19f,28f,42f,0f},new float[]{0.75f,0f,0f,-6f,-6f,2f,-18f,7f,-19f,21f,28f,42f,0f},new float[]{1f,0f,0f,-6f,-6f,2f,-18f,7f,-17f,19f,28f,42f,0f}})},
{"JumpDown",new Clip(false,new float[][]{new float[]{0f,0f,0f,0f,7f,-3f,14f,24f,-8f,8f,8f,8f,0f},new float[]{0.25f,0f,0f,0f,7f,-3f,14f,24f,-8f,8f,10f,6f,0f},new float[]{0.5f,0f,0f,0f,7f,-3f,14f,24f,-8f,8f,8f,8f,0f},new float[]{0.75f,0f,0f,0f,7f,-3f,14f,24f,-8f,8f,6f,10f,0f},new float[]{1f,0f,0f,0f,7f,-3f,14f,24f,-8f,8f,8f,8f,0f}})},
{"JumpDownLanding",new Clip(true,new float[][]{new float[]{0f,0f,0f,8f,5f,0f,14f,24f,0f,0f,0f,0f,0f},new float[]{0.16f,0f,0f,34f,8f,5f,20f,28f,-10f,10f,0f,0f,0f},new float[]{0.55f,0f,0f,-5f,0f,-3f,0f,15f,0f,0f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"DamageSmall",new Clip(true,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.12f,0f,-14f,4f,-8f,-5f,17f,-1f,0f,0f,0f,0f,0f},new float[]{0.42f,0f,-7f,0f,-4f,4f,8f,10f,0f,0f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"DamageBig",new Clip(true,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.14f,0f,-30f,12f,-17f,-9f,28f,-20f,-9f,8f,0f,0f,0f},new float[]{0.42f,0f,-18f,16f,-11f,5f,0f,-9f,0f,0f,0f,0f,0f},new float[]{0.75f,0f,0f,5f,3f,0f,0f,24f,0f,0f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"DamageDownFront",new Clip(true,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.16f,22f,0f,20f,5f,-5f,20f,-10f,0f,0f,0f,0f,0f},new float[]{0.52f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{0.65f,83f,0f,2f,0f,4f,15f,-170f,-9f,13f,0f,0f,0f},new float[]{0.85f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{1f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f}})},
{"DamageStandUpFront",new Clip(true,new float[][]{new float[]{0f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{0.18f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{0.5f,48f,0f,35f,0f,-8f,-28f,-35f,-19f,20f,0f,0f,0f},new float[]{0.8f,8f,0f,16f,0f,0f,-8f,24f,-7f,7f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"RollingStandUpFront",new Clip(true,new float[][]{new float[]{0f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{0.16f,112f,0f,18f,0f,0f,-20f,-150f,-25f,27f,0f,0f,0f},new float[]{0.4f,242f,0f,8f,0f,-6f,-40f,-58f,-30f,32f,0f,0f,0f},new float[]{0.7f,352f,0f,24f,0f,6f,-17f,-2f,-12f,12f,0f,0f,0f},new float[]{1f,360f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"DamageDownBack",new Clip(true,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f},new float[]{0.16f,-22f,0f,20f,-5f,5f,-20f,35f,0f,0f,0f,0f,0f},new float[]{0.52f,-88f,0f,8f,0f,-7f,-18f,68f,-12f,16f,0f,0f,0f},new float[]{0.65f,-83f,0f,2f,0f,-4f,-15f,66f,-9f,13f,0f,0f,0f},new float[]{0.85f,-88f,0f,8f,0f,-7f,-18f,68f,-12f,16f,0f,0f,0f},new float[]{1f,-88f,0f,8f,0f,-7f,-18f,68f,-12f,16f,0f,0f,0f}})},
{"DamageStandUpBack",new Clip(true,new float[][]{new float[]{0f,-88f,0f,8f,0f,-7f,-18f,68f,-12f,16f,0f,0f,0f},new float[]{0.18f,-88f,0f,8f,0f,-7f,-18f,68f,-12f,16f,0f,0f,0f},new float[]{0.5f,-48f,0f,35f,0f,8f,28f,44f,-19f,20f,0f,0f,0f},new float[]{0.8f,-8f,0f,16f,0f,0f,-8f,24f,-7f,7f,0f,0f,0f},new float[]{1f,0f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"RollingStandUpBack",new Clip(true,new float[][]{new float[]{0f,-88f,0f,8f,0f,-7f,-18f,68f,-12f,16f,0f,0f,0f},new float[]{0.16f,-112f,0f,18f,0f,0f,-20f,-60f,-25f,27f,0f,0f,0f},new float[]{0.4f,-242f,0f,8f,0f,-6f,-40f,-58f,-30f,32f,0f,0f,0f},new float[]{0.7f,-352f,0f,24f,0f,6f,-17f,-2f,-12f,12f,0f,0f,0f},new float[]{1f,-360f,0f,0f,0f,0f,0f,18f,0f,0f,0f,0f,0f}})},
{"DamageAirFlyFront",new Clip(false,new float[][]{new float[]{0f,-8f,0f,0f,-8f,0f,0f,-3f,0f,0f,0f,0f,0f},new float[]{0.3f,-42f,0f,0f,0f,-8f,28f,-30f,-24f,18f,24f,16f,0f},new float[]{1f,-58f,0f,0f,0f,-3f,31f,-32f,-22f,22f,20f,20f,0f}})},
{"DamageAirFlyBack",new Clip(false,new float[][]{new float[]{0f,8f,0f,0f,8f,0f,0f,27f,0f,0f,0f,0f,0f},new float[]{0.3f,41f,0f,0f,0f,8f,-23f,49f,18f,-22f,16f,24f,0f},new float[]{1f,57f,0f,0f,0f,3f,-27f,52f,22f,-22f,20f,20f,0f}})},
{"DamageAirSmall",new Clip(false,new float[][]{new float[]{0f,0f,0f,0f,0f,0f,0f,18f,-10f,12f,10f,10f,0f},new float[]{0.2f,-12f,0f,0f,0f,-8f,25f,-6f,-22f,23f,24f,18f,0f},new float[]{1f,-6f,0f,0f,0f,0f,10f,12f,-12f,14f,14f,14f,0f}})},
{"DamageAirUp",new Clip(false,new float[][]{new float[]{0f,-8f,0f,0f,0f,0f,0f,5f,-12f,14f,0f,0f,0f},new float[]{0.28f,-28f,0f,0f,-8f,8f,22f,-19f,-26f,23f,34f,30f,0f},new float[]{1f,-32f,0f,0f,0f,4f,18f,-15f,-22f,22f,30f,28f,0f}})},
{"DamageAirFall",new Clip(false,new float[][]{new float[]{0f,-32f,0f,0f,0f,4f,18f,-15f,-22f,22f,30f,28f,0f},new float[]{0.5f,-15f,0f,0f,6f,-6f,26f,25f,-14f,14f,12f,12f,0f},new float[]{1f,-6f,0f,0f,8f,-8f,28f,29f,-10f,10f,8f,8f,0f}})},
{"DamageAirDown",new Clip(false,new float[][]{new float[]{0f,8f,0f,0f,6f,-4f,18f,25f,-14f,16f,18f,18f,0f},new float[]{0.4f,46f,0f,0f,0f,8f,24f,-20f,-22f,25f,20f,16f,0f},new float[]{1f,68f,0f,0f,0f,5f,19f,-50f,-14f,18f,8f,8f,0f}})},
{"DamageAirDownLanding",new Clip(true,new float[][]{new float[]{0f,68f,0f,0f,0f,5f,19f,-50f,-14f,18f,0f,0f,0f},new float[]{0.18f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{0.3f,81f,0f,3f,0f,3f,12f,-170f,-8f,12f,0f,0f,0f},new float[]{0.5f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f},new float[]{1f,88f,0f,8f,0f,7f,18f,-170f,-12f,16f,0f,0f,0f}})}};
  static readonly Dictionary<string,SourceState> states=new Dictionary<string,SourceState>(StringComparer.Ordinal){{"NASOD_SHERIFF_START",new SourceState("WaitStart",2.73333335f,false)},
{"NASOD_SHERIFF_WAIT",new SourceState("Wait",1f,true)},
{"NASOD_SHERIFF_WALK",new SourceState("Walk",1f,true)},
{"NASOD_SHERIFF_ATTACK_A_READY",new SourceState("ComboA_Ready",2.23333335f,false)},
{"NASOD_SHERIFF_ATTACK_A",new SourceState("ComboA",3.16666675f,false)},
{"NASOD_SHERIFF_JUMP_UP",new SourceState("JumpUp",1f,true)},
{"NASOD_SHERIFF_JUMP_DOWN",new SourceState("JumpDown",1f,true)},
{"NASOD_SHERIFF_JUMP_UP_DIR",new SourceState("JumpUp",1f,true)},
{"NASOD_SHERIFF_JUMP_DOWN_DIR",new SourceState("JumpDown",1f,true)},
{"NASOD_SHERIFF_JUMP_LANDING",new SourceState("JumpDownLanding",0.966666639f,false)},
{"NASOD_SHERIFF_DAMAGE_SMALL",new SourceState("DamageSmall",0.866666675f,false)},
{"NASOD_SHERIFF_DAMAGE_BIG",new SourceState("DamageBig",1.43333328f,false)},
{"NASOD_SHERIFF_DAMAGE_DOWN_FRONT",new SourceState("DamageDownFront",1.70000005f,false)},
{"NASOD_SHERIFF_DAMAGE_DOWN_BACK",new SourceState("DamageDownBack",1.70000005f,false)},
{"NASOD_SHERIFF_DAMAGE_FLY_FRONT",new SourceState("DamageAirFlyFront",1f,false)},
{"NASOD_SHERIFF_DAMAGE_FLY_BACK",new SourceState("DamageAirFlyBack",1f,false)},
{"NASOD_SHERIFF_DAMAGE_AIR",new SourceState("DamageAirSmall",0.699999988f,false)},
{"NASOD_SHERIFF_DAMAGE_AIR_DOWN",new SourceState("DamageAirDown",0.566666663f,false)},
{"NASOD_SHERIFF_DAMAGE_AIR_UP",new SourceState("DamageAirUp",1f,false)},
{"NASOD_SHERIFF_DAMAGE_AIR_FALL",new SourceState("DamageAirFall",1f,false)},
{"NASOD_SHERIFF_DAMAGE_AIR_DOWN_LANDING",new SourceState("DamageAirDownLanding",1.5f,false)},
{"NASOD_SHERIFF_STAND_UP_FRONT",new SourceState("DamageStandUpFront",1.5f,false)},
{"NASOD_SHERIFF_STAND_UP_BACK",new SourceState("DamageStandUpBack",1.73333335f,false)},
{"NASOD_SHERIFF_ROLLING_STAND_UP_FRONT",new SourceState("RollingStandUpFront",2.4000001f,false)},
{"NASOD_SHERIFF_ROLLING_STAND_UP_BACK",new SourceState("RollingStandUpBack",2.4000001f,false)},
{"NASOD_SHERIFF_DYING_LAND_FRONT",new SourceState("DamageDownFront",1.70000005f,false)},
{"NASOD_SHERIFF_DYING_LAND_BACK",new SourceState("DamageDownBack",1.70000005f,false)},
{"NASOD_SHERIFF_DYING_SKY",new SourceState("DamageAirDownLanding",1.5f,false)}};
  public static Layout Part(int i){return parts[i];}
  public static bool ValidateSource(string state,string motion,float duration,float speed,bool loop){SourceState s;return states.TryGetValue(state??"",out s)&&s.motion==motion&&Math.Abs(s.duration-duration)<.000001f&&speed==1&&s.loop==loop;}
  // Measured r4 rifle opening: global atlas x=1513, center y=277.5.
  // The straight barrel is horizontal in the unrotated atlas; its attachment
  // pivot is a shoulder, NOT the source Small_Laser origin or a new hitbox.
  public const float GunMuzzleX=592f,GunMuzzleY=113.5f;
  static float Ease(float p){p=Math.Max(0,Math.Min(1,p));return p*p*(3-2*p);}
  public static float FiringAlignmentWeight(string motion,float clock,float duration){
   if(!Finite(clock)||!Finite(duration)||duration<=0)throw new ArgumentException("Finite firing presentation clock required");
   float p=clock/duration;
   if(motion=="ComboA_Ready")return Ease((p-.2f)/.38f);
   if(motion=="ComboA")return 1-Ease((p-.79f)/.21f);
   return 0;
  }
  public static void AlignFiringLine(string motion,float clock,float duration,float projectedLineAboveFoot,PartPose[] output){
   if(output==null||output.Length!=6||!Finite(projectedLineAboveFoot))throw new ArgumentException("Finite original firing line / six part buffer required");
   float weight=FiringAlignmentWeight(motion,clock,duration);if(weight<=0)return;
   var q=parts[3];var p=output[3];p.rotation*=1-weight;
   double dx,dy;Rotate((GunMuzzleX-q.pivotX)*q.scale,(GunMuzzleY-q.pivotY)*q.scale,p.rotation,out dx,out dy);
   float wantedY=FootY-projectedLineAboveFoot/UnitsPerArtPixel;
   p.y+=(wantedY-p.y-(float)dy)*weight;output[3]=p;
   // Only this held-arm display changes: no actor root, source clock, source
   // emission origin/trajectory, event thresholds, collision or scale changes.
  }
  static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  static void Rotate(double x,double y,double degrees,out double rx,out double ry){double a=degrees*Math.PI/180,c=Math.Cos(a),s=Math.Sin(a);rx=x*c-y*s;ry=x*s+y*c;}
  // sourceClock is already looped/clamped by SourcePhaseWholeLookupV21.
  // Ground support uses the measured alpha>=64 hull, without pixel changes.
  public static void Sample(string motion,float sourceClock,float sourceDuration,PartPose[] output,float[] scratch){
   Clip clip;if(!clips.TryGetValue(motion??"",out clip))throw new ArgumentException("Unknown Sheriff source motion");
   if(output==null||output.Length!=6||scratch==null||scratch.Length!=12||!Finite(sourceClock)||sourceClock<0||!Finite(sourceDuration)||sourceDuration<=0||sourceClock>sourceDuration+.00001f)throw new ArgumentException("Exact Sheriff source clock / six part buffer required");
   double p=Math.Min(1,sourceClock/sourceDuration);int k=0;while(k<clip.keys.Length-2&&p>clip.keys[k+1][0])k++;
   var a=clip.keys[k];var b=clip.keys[k+1];double t=Math.Max(0,Math.Min(1,(p-a[0])/(b[0]-a[0])));t=t*t*(3-2*t);
   for(int n=0;n<12;n++)scratch[n]=(float)(a[n+1]+(b[n+1]-a[n+1])*t);
   var v=scratch;double maxY=double.NegativeInfinity;
   for(int i=0;i<6;i++){
    var q=parts[i];double x,y;Rotate(q.restX,q.restY-325,v[3],out x,out y);x+=v[1];y+=325+v[2];
    if(i==0)y+=v[11];if(i==4)y-=v[9];if(i==5)y-=v[10];double rx,ry;Rotate(x,y-540,v[0],out rx,out ry);x=rx;y=ry+540;
    float local=i==0?v[4]:i==2?v[5]:i==3?v[6]:i==4?v[7]:i==5?v[8]:0;
    float rotation=v[0]+v[3]+local;output[i]=new PartPose{x=(float)x,y=(float)y,rotation=rotation};
    for(int j=0;j<q.hull.Length;j+=2){Rotate((q.hull[j]-q.pivotX)*q.scale,(q.hull[j+1]-q.pivotY)*q.scale,rotation,out rx,out ry);maxY=Math.Max(maxY,y+ry);}
   }
   float groundY=clip.ground?(float)(FootY-maxY):0;for(int i=0;i<6;i++)output[i].y+=groundY;
  }
 }
}
