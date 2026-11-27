using System;
namespace ElswordPixel {
 public interface IPlainMineMajorSequenceR1302 {
  bool IsLive {get;}void SetPosition(PlainMineSourcePositionR1269 position);void ChangeTexture(string originalName);void DestroyInstance();
 }
 public interface IPlainMineMinorSequenceR1302 {
  bool IsLive {get;}void SetLandPosition(PlainMineSourcePositionR1269 position);void DestroyInstance();
 }
 public interface IPlainMineMajorConsumerR1302 {
  IPlainMineMajorSequenceR1302 Create(Enemy owner,string name,PlainMineSourcePositionR1269 position,float a0,float a1,float b0,float b1,float sourceLandY,float modelTime);
  void Clear();
 }
 public interface IPlainMinePresentationR1302:IPlainMineMajorConsumerR1302 {
  bool StateParticlesBoundR1302 {get;}
  void CreateStateParticleR1302(Enemy owner,string name,PlainMineSourcePositionR1269 position,float sourceLandY,float modelTime);
  IPlainMineMinorSequenceR1302 CreateMinor(Enemy owner,string name,PlainMineSourcePositionR1269 position,float a0,float a1,float b0,float b1,float modelTime);
  void PlayOriginalSound(Enemy owner,string originalName,PlainMineSourcePositionR1269 position,float modelTime);
 }
 public sealed partial class GameModel {
  IPlainMineMajorConsumerR1302 plainMineMajorR1302;
  public bool PlainMineMajorConsumerBoundR1302 {get{return plainMineMajorR1302!=null;}}
  public void SetPlainMineMajorConsumerR1302(IPlainMineMajorConsumerR1302 consumer){
   if(plainRouteR1258!=PlainRecycleR1258||!PlainOriginalRoomScopeR1258(plainRouteR1258)||plainDefinitionsR1258==null||consumer==null)
    throw new ArgumentException("Exact original Plain Recycle Major consumer context required");
   if(plainMineMajorR1302!=null&&!ReferenceEquals(plainMineMajorR1302,consumer))throw new InvalidOperationException("Original Mine Major consumer already owned");
   ValidatePlainMineExplosionR1302();plainMineMajorR1302=consumer;
  }
  public void UnbindPlainMineMajorConsumerR1302(IPlainMineMajorConsumerR1302 consumer){if(ReferenceEquals(plainMineMajorR1302,consumer))plainMineMajorR1302=null;}
  void ClearPlainMineMajorConsumerR1302(){var old=plainMineMajorR1302;plainMineMajorR1302=null;if(old!=null)old.Clear();}
  IPlainMinePresentationR1302 RequirePlainMinePresentationR1302(){var p=plainMineMajorR1302 as IPlainMinePresentationR1302;if(p==null||!p.StateParticlesBoundR1302)throw new InvalidOperationException("Original Mine scoped Major/Minor/state-particle/audio consumer is not bound");return p;}
  void CreatePlainMineOriginalLightR1302(Enemy owner,float at){
   if(plainMineMajorR1302==null)throw new InvalidOperationException("Original Mine Major service is unavailable");
   var position=plainMineSourcePositionR1269(owner);position.y+=25;position.z-=10;
   // WAIT/READY original nullable result is deliberately ignored by Lua.
   plainMineMajorR1302.Create(owner,"Light_Altera_Mine01",position,-1,-1,1,-1,plainMineSourceLandYR1269(owner),at);
  }
  void ValidatePlainMineExplosionR1302(){
   NpcEffectDefinition d;if(!npcEffectDefinitions.TryGetValue("MINE_EXPLOSION",out d)||d.id!="MINE_EXPLOSION"||d.lifetime!=.333f||d.physical!=0||d.magic!=.5f||d.repeat||d.launch!=504||d.push!=0||d.damageFrom!=0||d.damageUntil!=.333f||d.baseSpheres==null||d.baseSpheres.Length!=4||d.baseSpheres[0]!=0||d.baseSpheres[1]!=0||d.baseSpheres[2]!=0||d.baseSpheres[3]!=12.12435565f)
    throw new InvalidOperationException("Exact existing original MINE_EXPLOSION definition is not installed");
  }
 }
}
