using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  // The Geyser consumer owns its independent strong two-template document,
  // state cursors and source views. These helpers never add registrations or
  // relax the separately pinned fifteen-template/thirty-eight-field contract.
  DragonEffectParticleViewV20 CreateDragonNestGeyserSourceParticleViewV20(OutskirtsVisualParticle data,Texture2D texture,string sourceTemplateSha256){
   bool fire=data!=null&&data.id=="Major/SteamGeyser_FireAttack01",warning=data!=null&&data.id=="Major/SteamGeyser_FireAttack02";
   RequireDragonEffectParticleV20((fire||warning)&&sourceTemplateSha256==(fire?"e3bc19bbbae10526dc0ae33cb69a6f7e6fb3d4ac84940eeb485dd9e8c09b7bbb":"cb8969d764abb017aa35c7381f22d1d84dd529fa81f1de5f0209605ff11533ec"),"Geyser bridge accepts only its two original qualified Major templates");
   ValidateLakeParticleV18(data);RequireDragonEffectParticleV20(data.particleType=="PT_3D_PLANE"&&data.billboardType=="BT_ALL"&&data.sourceBlend=="D3DBLEND_SRCALPHA"&&data.destinationBlend=="D3DBLEND_DESTALPHA"&&data.layer==14&&data.triggerCount==1&&data.drawCount==1&&data.capacity==(fire?200:100)&&Mathf.Abs(data.triggerTime-(fire?.8f:.6f))<.00001f&&data.gravityRanges!=null&&data.gravityRanges.Length==6,"Unsupported Geyser particle basis/trigger/count");
   RequireDragonEffectParticleV20(texture&&texture.isReadable&&DragonEffectParticleTextureHashV20(texture)=="1040443546966a2ced629575be45091d2c1fb45e8baf2baee7837535ddc46c99","Geyser original smoke02 decoded RGBA differs");
   var view=new DragonEffectParticleViewV20{data=data};
   try{texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;view.sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),1);view.material=OutskirtsMaterial(data.sourceBlend,data.destinationBlend);outskirtsMaterials.Remove(view.material);return view;}
   catch{DestroyDragonEffectParticleObjectV20(view.sprite);DestroyDragonEffectParticleObjectV20(view.material);throw;}
  }
  void DisposeDragonNestGeyserSourceParticleViewV20(DragonEffectParticleViewV20 view){
   if(view==null)return;for(int i=dragonEffectParticleSequencesV20.Count-1;i>=0;i--)if(dragonEffectParticleSequencesV20[i].view==view)dragonEffectParticleSequencesV20.RemoveAt(i);
   for(int i=0;i<dragonEffectParticleSpritesV20.Count;i++){var sprite=dragonEffectParticleSpritesV20[i];if(sprite&&sprite.sprite==view.sprite){sprite.gameObject.SetActive(false);var transform=dragonEffectParticleSpriteTransformsV20[i];if(!ReferenceEquals(transform,null))npcEffectDepths.Remove(transform);}}
   DestroyDragonEffectParticleObjectV20(view.sprite);DestroyDragonEffectParticleObjectV20(view.material);DragonNestEffectParticleVisibleSpriteCountV20=0;foreach(var sprite in dragonEffectParticleSpritesV20)if(sprite&&sprite.enabled&&sprite.gameObject.activeSelf)DragonNestEffectParticleVisibleSpriteCountV20++;
  }
  bool EnqueueDragonNestGeyserSourceParticleV20(DragonEffectParticleViewV20 view,float at,Vector3 logicalPosition,float nativeDepth,int facing,float unit,int seed,int actorSerial,int sourceCallIndex,bool sourceSetAddRotate,bool sourceSetAxisAngle,Vector3 sourceEulerDegrees){
   if(dragonEffectParticleDocumentV20==null||!dragonEffectParticleEnabledV20)return false;
   bool fire=sourceCallIndex==2;RequireDragonEffectParticleV20(view!=null&&!view.held&&view.data.id==(fire?"Major/SteamGeyser_FireAttack01":"Major/SteamGeyser_FireAttack02")&&sourceCallIndex>=0&&sourceCallIndex<=2&&sourceSetAddRotate==fire&&sourceSetAxisAngle==fire,"Changed three source Geyser callback identities/rotation flags");
   RequireDragonEffectParticleV20(DragonEffectParticleFiniteV20(at)&&at<=game.elapsed+.0001f&&DragonEffectParticleFiniteV20(logicalPosition.x)&&DragonEffectParticleFiniteV20(logicalPosition.y)&&DragonEffectParticleFiniteV20(logicalPosition.z)&&DragonEffectParticleFiniteV20(nativeDepth)&&DragonEffectParticleFiniteV20(unit)&&unit>0&&actorSerial>0&&(facing==1||facing==-1)&&DragonEffectParticleFiniteV20(sourceEulerDegrees.x)&&DragonEffectParticleFiniteV20(sourceEulerDegrees.y)&&DragonEffectParticleFiniteV20(sourceEulerDegrees.z),"Invalid committed Geyser source point/clock/rotation");
   var p=view.data;float rate=OutskirtsRange(p.rate,0,seed,1);int per=p.triggerCount,count=p.triggerTime>0?Mathf.CeilToInt(p.triggerTime*rate)*per:per;
   dragonEffectParticleSequencesV20.Add(new DragonEffectParticleSequenceV20{view=view,field="source-geyser-callback/"+sourceCallIndex,eventSequence=sourceCallIndex,serial=actorSerial,seed=seed,facing=facing,rate=rate,count=count,perTrigger=p.triggerTime>0?per:1,started=at,unit=unit,origin=new Vector3(logicalPosition.x,logicalPosition.y,0),depth=nativeDepth+logicalPosition.z/.28f,externalGeyser=true,sourceSetAddRotate=sourceSetAddRotate,sourceSetAxisAngle=sourceSetAxisAngle,sourceEuler=sourceEulerDegrees});DragonNestEffectParticleCreatedSequencesV20++;DragonNestEffectParticleScheduledBirthsV20+=count;return true;
  }
 }
}
