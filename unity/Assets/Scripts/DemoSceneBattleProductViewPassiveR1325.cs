using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ElswordPixel { public sealed partial class DemoScene {
 sealed partial class BattleProductViewR1325 {

  const string AirshipSourceVisualSchemaV19="altera-airship-source-visual-v19";
  OutskirtsNpcVisualDocument airshipSourceVisualV19;
  void DisposeAlteraAirshipSourceVisualV19(){airshipSourceVisualV19=null;}
  void ConfigureAlteraAirshipSourceVisualV19(OutskirtsNpcVisualDocument data){
   if(data==null||data.schema!=AirshipSourceVisualSchemaV19)return;
   RequireOutskirtsVisual(data.particles.Length==21&&data.effectParticles.Length==52&&data.effectMeshes!=null&&data.effectMeshes.Length==7&&outskirtsMeshes.Count==7&&data.effectPassiveParticles!=null&&data.effectPassiveParticles.Length==13&&data.passiveBoneAtRoot&&data.sourcePassiveBoneSha256=="18368b6b6f9bfe672b2b4109b6509b6a84e0e29aa0ec4cd561c18121dbdb50d0"&&data.passiveEventTimePolicy=="single-emitter-delay-display-adapter"&&data.passiveOffsetPolicy=="native-world-absolute-point28-display-adapter","Airship visible scope or declared passive adapter changed");
   var keys=new HashSet<string>();foreach(var b in data.effectPassiveParticles){
    RequireOutskirtsVisual(b!=null&&!string.IsNullOrEmpty(b.effect)&&!string.IsNullOrEmpty(b.sourceField)&&keys.Add(b.effect+"/"+b.sourceField)&&outskirtsParticles.ContainsKey(b.particle)&&SteamFinite(b.eventTime)&&b.eventTime>=0&&SteamVector(b.sourceOffset,3)&&!b.hasForceDieHit,"Unbound Airship source passive emitter");
    bool bone=b.positionType=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_BONE";
    RequireOutskirtsVisual((bone&&b.bone=="Dummy01"||!bone&&b.positionType=="DAMAGE_EFFECT_POSITION_TYPE.DEPT_POS"&&string.IsNullOrEmpty(b.bone))&&(!b.hasApplyRotate||!b.applyRotate),"Airship passive origin/rotation changed");
   }
   foreach(var p in outskirtsParticles.Values)ValidateLakeParticleV18(p.data);
   foreach(var b in data.effectMeshes){bool rain=b.effect=="RAVEN_SPECIAL_ATTACK_MISSILE_RAIN";RequireOutskirtsVisual(b.compensateSourceAngle==rain&&b.bakedTemplateRotationZ==(rain?-90:0),"Airship baked/model rotation ownership changed");}
   // Reuse the bounded source-trigger-window renderer already exercised by
   // Lake. There are no Lake callback rows or Minor particles in this scope.
   airshipSourceVisualV19=data;lakeVisualConfiguredV18=true;LakeSourceParticleDrawFramesV18=0;lakeObservedV18.Clear();
  }
  void CollectAlteraAirshipPassiveV19(HashSet<string> live,HashSet<int> liveSerials){
   var doc=airshipSourceVisualV19;if(doc==null)return;
   foreach(var effect in game.npcEffects)foreach(var binding in doc.effectPassiveParticles){
    if(effect.id!=binding.effect||effect.age+1e-6f<binding.eventTime)continue;
    string key=effect.serial+"/airship/"+binding.sourceField;live.Add(key);liveSerials.Add(effect.serial);
    float age=Mathf.Max(0,effect.age-binding.eventTime),depth=(effect.owner==null?0:effect.owner.nativeDepth)+effect.z/.28f;
    // Dummy01 is a measured static zero-relative point. These source offsets
    // use the explicitly declared absolute native-world .28 display adapter.
    var offset=new Vector3(binding.sourceOffset[0],binding.sourceOffset[1],binding.sourceOffset[2])*.28f;
    var point=new Vector3(effect.x+offset.x,effect.y+offset.y,0);depth+=offset.z/.28f;
    LakeParticleSequenceV18 sequence;
    if(!lakePassiveV18.TryGetValue(key,out sequence)){
     sequence=AddLakeSequenceV18(binding.particle,game.elapsed-effect.age+binding.eventTime,point,depth,effect.facing,.28f*effect.ownerScale,effect.serial*179+keysalt(binding.sourceField));
     sequence.passive=true;sequence.serial=effect.serial;sequence.follows=false;sequence.rotateWithOwner=!binding.hasApplyRotate||binding.applyRotate;
     foreach(var e in game.npcPresentationEvents)if(e.type=="spawn"&&e.serial==effect.serial){sequence.positions.Add(new LakeParticlePositionV18{at=0,point=new Vector3(e.x+offset.x,e.y+offset.y,0),depth=e.depth+(e.z+offset.z)/.28f});break;}
     lakePassiveV18.Add(key,sequence);
    }
    if(sequence.finished)continue;
    sequence.origin=point;sequence.depth=depth;sequence.facing=effect.facing;
    var sample=new LakeParticlePositionV18{at=age,point=point,depth=depth};
    if(sequence.positions.Count>0&&Mathf.Approximately(sequence.positions[sequence.positions.Count-1].at,age))sequence.positions[sequence.positions.Count-1]=sample;else sequence.positions.Add(sample);
    while(sequence.positions.Count>2&&sequence.positions[1].at<age-sequence.view.data.lifetime[1]-.05f)sequence.positions.RemoveAt(0);
   }
  }
  static int keysalt(string key){unchecked{int hash=17;foreach(char c in key)hash=hash*31+c;return hash;}}
 }
 }
}
