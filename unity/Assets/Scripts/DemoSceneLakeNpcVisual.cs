using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class LakeMinorVisualDocumentV18 {public string schema;public bool omitUnverifiedPlaneOrientation;public OutskirtsVisualParticle[] particles;}
 public sealed partial class DemoScene {
  // Source callback ownership is provided by GameModel. These bounded, seeded
  // sprite sequences are an explicit display adapter, not native allocation or
  // DrawCount multi-draw emulation. They never create damage or a flight path.
  sealed class LakeParticlePositionV18 {public float at,depth;public Vector3 point;}
  sealed class LakeParticleSequenceV18 {
   public OutskirtsParticleView view;public int handle,serial,seed,facing,count,perTrigger;
   public float started,unit,rate,stopAge=float.PositiveInfinity;
   public bool follows,passive,rotateWithOwner,stopped,finished;public Vector3 origin;public float depth;
   public readonly List<LakeParticlePositionV18> positions=new List<LakeParticlePositionV18>();
  }
  readonly Dictionary<string,OutskirtsParticleView> lakeMinorViewsV18=new Dictionary<string,OutskirtsParticleView>();
  readonly List<LakeParticleSequenceV18> lakeSequencesV18=new List<LakeParticleSequenceV18>();
  readonly Dictionary<int,LakeParticleSequenceV18> lakeHandlesV18=new Dictionary<int,LakeParticleSequenceV18>();
  readonly Dictionary<string,LakeParticleSequenceV18> lakePassiveV18=new Dictionary<string,LakeParticleSequenceV18>();
  readonly HashSet<int> lakeForceHitV18=new HashSet<int>();
  readonly List<SpriteRenderer> lakeSpritesV18=new List<SpriteRenderer>();
  readonly HashSet<string> lakeObservedV18=new HashSet<string>();
  Transform lakeWorldV18;int lakeRoomV18=-1,lakeGenerationV18=-1,lakeLastEventV18,lakeLastEffectEventV18;
  float lakeLastTimeV18=-1;bool lakeVisualConfiguredV18;
  public int LakeSourceParticleDrawFramesV18 {get;private set;}
  public int LakeSourceParticleKindsV18 {get{return lakeObservedV18.Count;}}
  void ConfigureLakeNpcVisual(LakeMinorVisualDocumentV18 minor,IDictionary<string,Texture2D> textures){
   DisposeLakeNpcVisual();
   RequireOutskirtsVisual(outskirtsNpcVisual!=null&&(outskirtsNpcVisual.schema=="besma-lake-source-visual-v18"||outskirtsNpcVisual.schema==NightSourceVisualSchemaV19),"Lake requires its bound source visual document");
   RequireOutskirtsVisual(minor!=null&&minor.schema=="besma-lake-minor-source-sprites-v18"&&minor.omitUnverifiedPlaneOrientation&&minor.particles!=null&&minor.particles.Length==2,"Lake minor sprite scope/pending plane declaration changed");
   foreach(var p in minor.particles){
    bool held=p!=null&&p.id=="LizardMan_ShamanKing_SP_Heal01";
    RequireOutskirtsVisual(p!=null&&(held||p.id=="LizardMan_ShamanKing_SP_Heal04")&&p.particleType=="PT_3D_PLANE"&&p.billboardType==(held?"BT_all":"BT_NONE")&&!lakeMinorViewsV18.ContainsKey(p.id),"unsupported Lake minor plane");
    ValidateLakeParticleV18(p);Texture2D texture;RequireOutskirtsVisual(textures.TryGetValue(p.texture,out texture)&&texture,"missing Lake minor original texture");
    texture.filterMode=FilterMode.Point;
    lakeMinorViewsV18.Add(p.id,new OutskirtsParticleView{data=p,sprite=PixelExplosionVisuals.Plane(p,texture),material=OutskirtsMaterial(p.sourceBlend,p.destinationBlend)});
   }
   foreach(var p in outskirtsParticles.Values)ValidateLakeParticleV18(p.data);
   lakeVisualConfiguredV18=true;LakeSourceParticleDrawFramesV18=0;lakeObservedV18.Clear();
  }
  static void ValidateLakeParticleV18(OutskirtsVisualParticle p){
   RequireOutskirtsVisual(p!=null&&SteamVector(p.lifetime,2)&&p.lifetime[0]>0&&p.lifetime[1]>=p.lifetime[0]&&SteamVector(p.rate,2)&&p.rate[0]>0&&p.rate[1]>=p.rate[0]&&SteamVector(p.radius,6)&&SteamVector(p.gravity,3)&&p.capacity>0&&p.triggerCount>0&&p.drawCount>0&&SteamFinite(p.triggerTime)&&p.triggerTime>=0&&p.curves!=null,"invalid Lake particle timing");
   RequireOutskirtsVisual(p.gravityRanges==null||p.gravityRanges.Length==0||SteamVector(p.gravityRanges,6),"invalid Lake random gravity");
   foreach(var c in p.curves)RequireOutskirtsVisual(c!=null&&c.ranges!=null&&c.ranges.Length%2==0&&Array.TrueForAll(c.ranges,SteamFinite)&&SteamFinite(c.start)&&SteamFinite(c.end)&&c.start>=0&&c.end>=c.start,"invalid Lake particle curve");
  }
  void ClearLakeNpcVisual(){
   foreach(var s in lakeSpritesV18)if(s){npcEffectDepths.Remove(s.transform);Destroy(s.gameObject);}lakeSpritesV18.Clear();
   lakeSequencesV18.Clear();lakeHandlesV18.Clear();lakePassiveV18.Clear();lakeForceHitV18.Clear();
   lakeWorldV18=null;lakeRoomV18=lakeGenerationV18=-1;lakeLastEventV18=lakeLastEffectEventV18=0;lakeLastTimeV18=-1;
  }
  void DisposeLakeNpcVisual(){ClearLakeNpcVisual();foreach(var p in lakeMinorViewsV18.Values)if(p.sprite)Destroy(p.sprite);lakeMinorViewsV18.Clear();lakeVisualConfiguredV18=false;}
  OutskirtsParticleView LakeViewV18(string id){OutskirtsParticleView view;if(lakeMinorViewsV18.TryGetValue(id,out view)||outskirtsParticles.TryGetValue(id,out view))return view;return null;}
  LakeParticleSequenceV18 AddLakeSequenceV18(string id,float started,Vector3 origin,float depth,int facing,float unit,int seed,int multiplier=1){
   // The source Heal04 plane has RotateX90, but its initial PT_3D_PLANE
   // basis has not been established. Applying it to Unity's XY sprite makes
   // it edge-on. Keep this explicitly pending rather than invent an angle.
   if(id=="LizardMan_ShamanKing_SP_Heal04")return null;
   var view=LakeViewV18(id);
   if(view==null){
    // Both source PT_MESH templates are explicitly unfinished in this internal
    // preview. A malformed source number is never silently corrected here.
    RequireOutskirtsVisual(id=="LizardMan_ShamanKing_SP_Heal02"||id=="LizardMan_ShamanKing_SP_Heal03","unbound Lake particle "+id);return null;
   }
   var p=view.data;float rate=OutskirtsRange(p.rate,0,seed,1);
   int per=Mathf.Max(1,p.triggerCount*multiplier),count=p.triggerTime>0?Mathf.CeilToInt(p.triggerTime*rate)*per:per;
   var sequence=new LakeParticleSequenceV18{view=view,started=started,origin=origin,depth=depth,facing=facing,unit=unit,seed=seed,rate=rate,count=count,perTrigger=p.triggerTime>0?per:1};
   lakeSequencesV18.Add(sequence);return sequence;
  }
  static float LakeBirthV18(LakeParticleSequenceV18 sequence,int index){return (index/sequence.perTrigger)/sequence.rate;}
  static Vector3 LakeParticleOriginV18(LakeParticleSequenceV18 sequence,float birth,out float depth){
   depth=sequence.depth;if(!sequence.passive||sequence.follows||sequence.positions.Count==0)return sequence.origin;
   var a=sequence.positions[0];foreach(var b in sequence.positions){if(b.at>=birth){float blend=b.at>a.at?Mathf.InverseLerp(a.at,b.at,birth):0;depth=Mathf.Lerp(a.depth,b.depth,blend);return Vector3.Lerp(a.point,b.point,blend);}a=b;}
   depth=a.depth;return a.point;
  }
  void StopLakeSequenceV18(LakeParticleSequenceV18 sequence,bool destroy){
   if(sequence==null)return;sequence.stopped=true;sequence.stopAge=Mathf.Min(sequence.stopAge,game.elapsed-sequence.started);
   if(destroy){sequence.finished=true;lakeSequencesV18.Remove(sequence);if(sequence.handle>0)lakeHandlesV18.Remove(sequence.handle);}
  }
  void RenderLakeNpcVisual(){
   if(!lakeVisualConfiguredV18)return;
   if(lakeRoomV18!=game.room||lakeWorldV18!=worldRoot||lakeGenerationV18!=game.NpcLakeVisualGenerationV18||game.elapsed+.0001f<lakeLastTimeV18){ClearLakeNpcVisual();lakeRoomV18=game.room;lakeWorldV18=worldRoot;lakeGenerationV18=game.NpcLakeVisualGenerationV18;}
   lakeLastTimeV18=game.elapsed;CollectLakeNightSourceParticlesV19();
   foreach(var e in game.npcLakeVisualEventsV18){
    if(e.sequence<=lakeLastEventV18)continue;lakeLastEventV18=e.sequence;LakeParticleSequenceV18 sequence;
    if(e.type=="create"){
     sequence=AddLakeSequenceV18(e.template,e.time,new Vector3(e.x,e.y,0),e.depth+e.z/.28f,e.facing,e.unit,e.sequence*67);
     if(sequence==null)continue;sequence.handle=e.handle;sequence.follows=e.callIndex==0;sequence.rotateWithOwner=e.ownerSetAddRotate||e.ownerSetAxisAngle;lakeHandlesV18.Add(e.handle,sequence);
    }else if(lakeHandlesV18.TryGetValue(e.handle,out sequence)){
     if(e.type=="move"){sequence.origin=new Vector3(e.x,e.y,0);sequence.depth=e.depth+e.z/.28f;sequence.facing=e.facing;}
     else if(e.type=="destroy"||e.type=="clear-expired")StopLakeSequenceV18(sequence,true);
    }
   }
   foreach(var e in game.npcPresentationEvents){
    if(e.sequence<=lakeLastEffectEventV18)continue;lakeLastEffectEventV18=e.sequence;
    if(e.type=="hit"&&e.effect=="DamageEffect_LIZARDMAN_OLDER_ATTACK_A"){
     lakeForceHitV18.Add(e.serial);LakeParticleSequenceV18 hitSequence;
     if(lakePassiveV18.TryGetValue(e.serial+"/LizardMan_Older_FireAttack_SmallRing",out hitSequence))StopLakeSequenceV18(hitSequence,true);
    }
    foreach(var b in outskirtsNpcVisual.effectParticles)if(b.effect==e.effect&&b.eventType==e.type)
     AddLakeSequenceV18(b.particle,e.time,new Vector3(e.x,e.y,0),e.depth+e.z/.28f,e.facing,.28f*e.scale,e.sequence*997,b.count);
   }
   var live=new HashSet<string>();var liveSerials=new HashSet<int>();
   foreach(var effect in game.npcEffects)foreach(var name in new[]{"LizardMan_Shamanking_DrainHit02","LizardMan_Shamanking_DrainHit03","LizardMan_Older_FireAttack_SmallRing"}){
    NpcLakePassivePoseV18 pose;if(!game.TryGetNpcLakePassivePoseV18(effect,name,out pose))continue;string key=pose.serial+"/"+name;live.Add(key);liveSerials.Add(pose.serial);
    LakeParticleSequenceV18 sequence;lakePassiveV18.TryGetValue(key,out sequence);
    if(pose.sourceHasForceDieHit&&pose.sourceForceDieHit&&lakeForceHitV18.Contains(pose.serial)){StopLakeSequenceV18(sequence,true);continue;}
    if(sequence==null){
     sequence=AddLakeSequenceV18(name,game.elapsed-pose.age,new Vector3(pose.x,pose.y,0),pose.depth+pose.z/.28f,pose.facing,pose.unit,pose.serial*179);sequence.passive=true;sequence.serial=pose.serial;sequence.follows=name=="LizardMan_Older_FireAttack_SmallRing";
     // Seed the initial path from the same real effect serial. A late first
     // render must not place every earlier trail particle at its current tip.
     foreach(var spawn in game.npcPresentationEvents)if(spawn.type=="spawn"&&spawn.serial==pose.serial){sequence.positions.Add(new LakeParticlePositionV18{at=0,point=new Vector3(spawn.x,spawn.y,0),depth=spawn.depth+spawn.z/.28f});break;}
     lakePassiveV18.Add(key,sequence);
    }
    if(sequence.finished)continue;
    sequence.origin=new Vector3(pose.x,pose.y,0);sequence.depth=pose.depth+pose.z/.28f;sequence.facing=pose.facing;
    var sample=new LakeParticlePositionV18{at=pose.age,point=new Vector3(pose.x,pose.y,0),depth=pose.depth+pose.z/.28f};
    if(sequence.positions.Count>0&&Mathf.Approximately(sequence.positions[sequence.positions.Count-1].at,pose.age))sequence.positions[sequence.positions.Count-1]=sample;
    else sequence.positions.Add(sample);
    // Keep enough observed path for all living particles. This interpolation
    // joins real effect samples; it does not simulate another projectile.
    while(sequence.positions.Count>2&&sequence.positions[1].at<pose.age-sequence.view.data.lifetime[1]-.05f)sequence.positions.RemoveAt(0);
   }
   CollectAlteraAirshipPassiveV19(live,liveSerials);
   var retired=new List<string>();foreach(var pair in lakePassiveV18)if(!live.Contains(pair.Key)){StopLakeSequenceV18(pair.Value,false);retired.Add(pair.Key);}
   foreach(var key in retired)lakePassiveV18.Remove(key);lakeForceHitV18.RemoveWhere(serial=>!liveSerials.Contains(serial));
   int used=0;var expired=new List<int>();
   for(int b=lakeSequencesV18.Count-1;b>=0;b--){
    var sequence=lakeSequencesV18[b];var p=sequence.view.data;float elapsed=game.elapsed-sequence.started;
    float last=Mathf.Min(LakeBirthV18(sequence,sequence.count-1),sequence.stopAge);
    if(elapsed>last+p.lifetime[1]){sequence.finished=true;sequence.positions.Clear();lakeSequencesV18.RemoveAt(b);if(sequence.handle>0){lakeHandlesV18.Remove(sequence.handle);expired.Add(sequence.handle);}continue;}
    int first=Mathf.Max(0,Mathf.FloorToInt((elapsed-p.lifetime[1])*sequence.rate)*sequence.perTrigger);
    int end=Mathf.Min(sequence.count-1,Mathf.FloorToInt(Mathf.Min(elapsed,sequence.stopAge)*sequence.rate)*sequence.perTrigger+sequence.perTrigger-1),shown=0;
    for(int i=end;i>=first&&shown<p.capacity;i--){
     float birth=LakeBirthV18(sequence,i),age=elapsed-birth;int seed=sequence.seed+i*179;
     if(age<0||age>=OutskirtsRange(p.lifetime,0,seed,2))continue;shown++;
     float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)continue;
     while(lakeSpritesV18.Count<=used)lakeSpritesV18.Add(WorldSprite("Lake original particle / display adapter",effectsRoot,null,44,Vector3.zero));
     var sprite=lakeSpritesV18[used++];sprite.gameObject.SetActive(true);sprite.enabled=true;sprite.sprite=sequence.view.sprite;sprite.sharedMaterial=sequence.view.material;sprite.sortingOrder=44+(p.layer-14);sprite.flipX=false;
     sprite.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);
     float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);sprite.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*sequence.unit/sprite.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*sequence.unit/sprite.sprite.rect.height,1);
     float rx=OutskirtsCurve(p,"Rotate",0,age,seed,0),ry=OutskirtsCurve(p,"Rotate",1,age,seed,0),rz=OutskirtsCurve(p,"Rotate",2,age,seed,0);
     // BT_NONE retains its original XYZ plane orientation (Heal04 RotateX90).
     // Camera-facing particles use their source Z spin. Only source-marked
     // Totem calls receive the projected facing yaw; no adjacent-row copying.
     sprite.transform.localRotation=p.billboardType=="BT_NONE"?Quaternion.Euler(rx,ry,rz):Quaternion.Euler(0,0,rz);
     if(sequence.rotateWithOwner&&sequence.facing<0)sprite.transform.localRotation=Quaternion.Euler(0,180,0)*sprite.transform.localRotation;
     var offset=Vector3.zero;for(int c=0;c<3;c++){float gravity=p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,c,seed,301):p.gravity[c];offset[c]=(OutskirtsRange(p.radius,c,seed,3)+OutskirtsVelocityIntegral(p,c,age,seed)+.5f*gravity*age*age)*sequence.unit;}
     if(sequence.rotateWithOwner)offset.x*=sequence.facing;
     float depth;var origin=LakeParticleOriginV18(sequence,birth,out depth);var position=origin+new Vector3(offset.x,offset.y,0);
     if(p.useLand){var floor=game.SurfaceBelow(position.x,origin.y+Mathf.Max(0,offset.y));if(floor!=null)position.y=Mathf.Max(position.y,floor.Y(position.x));}
     sprite.transform.localPosition=position;npcEffectDepths[sprite.transform]=depth+offset.z/.28f;LakeSourceParticleDrawFramesV18++;
     if(lakeObservedV18.Add(p.id))Debug.Log("LAKE_SOURCE_PARTICLE_DRAW template="+p.id+" originalTexture=true displayAdapter=true");
    }
   }
   for(int i=used;i<lakeSpritesV18.Count;i++)if(lakeSpritesV18[i])lakeSpritesV18[i].gameObject.SetActive(false);
   // Notification appends a model event: do it after the event enumeration.
   foreach(int handle in expired)game.NotifyNpcLakeVisualExpiredV18(handle);
  }
 }
}
