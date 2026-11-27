using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ElswordPixel { public sealed partial class DemoScene {
 sealed partial class BattleProductViewR1325 {

  sealed class OutskirtsMeshView {public OutskirtsVisualMesh data;public Sprite[] sprites;public Material material;}
  sealed class OutskirtsParticleView {public OutskirtsVisualParticle data;public Sprite sprite;public Material material;}
  sealed class OutskirtsParticleBurst {public OutskirtsParticleView view;public float started,unit,depth;public int count,seed,facing;public Vector3 origin;public Enemy attachedOwner;public string attachedState;}
  struct OutskirtsStateClock {public string state;public float time;}
  OutskirtsNpcVisualDocument outskirtsNpcVisual;
  readonly HashSet<string> sourceNpcMeshesObserved=new HashSet<string>();
  public int SourceNpcMeshDrawFrames {get;private set;}
  readonly Dictionary<string,OutskirtsMeshView> outskirtsMeshes=new Dictionary<string,OutskirtsMeshView>();
  readonly Dictionary<string,OutskirtsParticleView> outskirtsParticles=new Dictionary<string,OutskirtsParticleView>();
  readonly List<SpriteRenderer> outskirtsVisualSprites=new List<SpriteRenderer>();
  readonly List<OutskirtsParticleBurst> outskirtsBursts=new List<OutskirtsParticleBurst>();
  readonly Dictionary<Enemy,OutskirtsStateClock> outskirtsStateClocks=new Dictionary<Enemy,OutskirtsStateClock>();
  readonly List<Material> outskirtsMaterials=new List<Material>();
  Transform outskirtsVisualWorld;int outskirtsVisualRoom=-1,outskirtsPresentationGeneration=-1,outskirtsLastEvent;float outskirtsVisualTime=-1;
  static void RequireOutskirtsVisual(bool value,string message){if(!value)throw new InvalidOperationException("Outskirts visual: "+message);}
  Material OutskirtsMaterial(string source,string destination){
   int dst=destination=="D3DBLEND_INVSRCALPHA"?10:destination=="D3DBLEND_DESTALPHA"?7:destination=="D3DBLEND_ONE"?1:-1;
   RequireOutskirtsVisual(source=="D3DBLEND_SRCALPHA"&&dst>=0,"unsupported source blend");
   var shader=Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite");RequireOutskirtsVisual(shader,"missing original blend shader");
   var m=new Material(shader){name="Original Outskirts effect blend"};m.SetInt("_SrcBlend",5);m.SetInt("_DstBlend",dst);outskirtsMaterials.Add(m);return m;
  }
  // External preview loader owns textures and verifies file pins before this
  // call. Keys are visual.json-relative paths, including components/ for pages.
  void ConfigureOutskirtsNpcVisual(OutskirtsNpcVisualDocument data,OutskirtsNpcMeshManifest meshes,IDictionary<string,Texture2D> textures){
   DisposeOutskirtsNpcVisual();if(data==null)return;RequireOutskirtsVisual(data.schema==AirshipSourceVisualSchemaV19,"Battle owner accepts only its original Airship document");
   sourceNpcMeshesObserved.Clear();SourceNpcMeshDrawFrames=0;
   bool castle=data.schema=="castle-center-source-visual-v17";
   bool dragon=data.schema=="dragon-road-source-visual-v17";
   bool lake=data.schema=="besma-lake-source-visual-v18";bool airship=data.schema==AirshipSourceVisualSchemaV19;bool night=data.schema==NightSourceVisualSchemaV19;
   RequireOutskirtsVisual((data.schema=="outskirts-source-visual-v14"||castle||dragon||lake||airship||night)&&Mathf.Approximately(data.unit,.28f)&&data.particles!=null&&data.effectParticles!=null&&data.stateVisuals!=null&&meshes!=null&&meshes.components!=null,"invalid scoped document");
   try{
    foreach(var m in meshes.components){
     RequireOutskirtsVisual(m!=null&&m.fps==60&&m.frames!=null&&m.frames.Length>0&&m.durationSeconds>0&&m.pixelsPerNativeUnit>0&&!outskirtsMeshes.ContainsKey(m.id),"invalid mesh clip");
     var view=new OutskirtsMeshView{data=m,sprites=new Sprite[m.frames.Length],material=OutskirtsMaterial(m.sourceBlend,m.destinationBlend)};outskirtsMeshes.Add(m.id,view);
     for(int i=0;i<m.frames.Length;i++){var f=m.frames[i];Texture2D texture;RequireOutskirtsVisual(f!=null&&f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2&&textures.TryGetValue("components/"+f.atlas,out texture)&&texture,"missing source mesh page");texture=textures["components/"+f.atlas];texture.filterMode=FilterMode.Point;var r=f.rect;RequireOutskirtsVisual(r[0]>=0&&r[1]>=0&&r[2]>0&&r[3]>0&&r[0]+r[2]<=texture.width&&r[1]+r[3]<=texture.height,"invalid source mesh rectangle");view.sprites[i]=Sprite.Create(texture,new Rect(r[0],texture.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1);}
    }
    foreach(var p in data.particles){Texture2D texture;RequireOutskirtsVisual(p!=null&&!string.IsNullOrEmpty(p.id)&&!outskirtsParticles.ContainsKey(p.id)&&p.curves!=null&&p.lifetime!=null&&p.lifetime.Length==2&&p.lifetime[0]>0&&p.lifetime[1]>=p.lifetime[0]&&p.rate!=null&&p.rate.Length==2&&p.rate[0]>0&&p.radius!=null&&p.radius.Length==6&&p.gravity!=null&&p.gravity.Length==3&&textures.TryGetValue(p.texture,out texture)&&texture,"invalid source particle");
     foreach(var curve in p.curves)RequireOutskirtsVisual(curve!=null&&curve.ranges!=null&&curve.ranges.Length%2==0&&curve.start>=0&&curve.end>=curve.start,"invalid source particle curve");
     texture=textures[p.texture];texture.filterMode=FilterMode.Point;outskirtsParticles.Add(p.id,new OutskirtsParticleView{data=p,sprite=PixelExplosionVisuals.Plane(p,texture),material=OutskirtsMaterial(p.sourceBlend,p.destinationBlend)});
    }
    foreach(var b in data.effectParticles)RequireOutskirtsVisual(b!=null&&(b.eventType=="spawn"||b.eventType=="hit")&&b.count>0&&outskirtsParticles.ContainsKey(b.particle),"invalid effect particle binding");
    foreach(var s in data.stateVisuals)RequireOutskirtsVisual(s!=null&&s.duration>0&&s.unit>0&&s.tracks!=null&&s.emissions!=null,"invalid visual state");
    if(castle||dragon||lake||airship||night){int count=airship?7:castle?3:2;RequireOutskirtsVisual(data.effectMeshes!=null&&data.effectMeshes.Length==(night?1:lake?3:count)&&outskirtsMeshes.Count==count,"missing scoped source mesh bindings");foreach(var b in data.effectMeshes)RequireOutskirtsVisual(b!=null&&!string.IsNullOrEmpty(b.effect)&&outskirtsMeshes.ContainsKey(b.mesh),"unbound source effect mesh");}
    else RequireOutskirtsVisual(outskirtsMeshes.ContainsKey("bomb")&&outskirtsMeshes.ContainsKey("mega-slash"),"missing visible original meshes");outskirtsNpcVisual=data;ConfigureAlteraAirshipSourceVisualV19(data);
   }catch{DisposeOutskirtsNpcVisual();throw;}
  }
  void ClearOutskirtsNpcVisual(){
   foreach(var sprite in outskirtsVisualSprites)if(sprite){npcEffectDepths.Remove(sprite.transform);Destroy(sprite.gameObject);}outskirtsVisualSprites.Clear();outskirtsBursts.Clear();outskirtsStateClocks.Clear();outskirtsVisualWorld=null;outskirtsVisualRoom=-1;outskirtsPresentationGeneration=-1;outskirtsLastEvent=0;outskirtsVisualTime=-1;
  }
  void DisposeOutskirtsNpcVisual(){DisposeAlteraAirshipStateVisualV20();DisposeAlteraAirshipSourceVisualV19();DisposeLakeNpcVisual();ClearOutskirtsNpcVisual();foreach(var m in outskirtsMeshes.Values)foreach(var sprite in m.sprites)if(sprite)Destroy(sprite);foreach(var p in outskirtsParticles.Values)if(p.sprite)Destroy(p.sprite);foreach(var m in outskirtsMaterials)if(m)Destroy(m);outskirtsMeshes.Clear();outskirtsParticles.Clear();outskirtsMaterials.Clear();outskirtsNpcVisual=null;}
  SpriteRenderer OutskirtsSprite(ref int used,Sprite image,Material material,int layer){
   while(outskirtsVisualSprites.Count<=used)outskirtsVisualSprites.Add(WorldSprite("Original Outskirts source effect",effectsRoot,null,44,Vector3.zero));
   var s=outskirtsVisualSprites[used++];s.gameObject.SetActive(true);s.enabled=true;s.sprite=image;s.sharedMaterial=material;s.sortingOrder=44+(layer-14);s.flipX=false;s.color=Color.white;return s;
  }
  void DrawOutskirtsMesh(ref int used,string id,float age,Vector3 origin,int facing,float unit,float depth){
   var m=outskirtsMeshes[id];if(age<0||age>=m.data.durationSeconds)return;int i=Mathf.Clamp(Mathf.FloorToInt(age*m.data.fps),0,m.sprites.Length-1);var f=m.data.frames[i];float scale=unit/m.data.pixelsPerNativeUnit;
   var s=OutskirtsSprite(ref used,m.sprites[i],m.material,14);s.flipX=facing<0;s.transform.localRotation=Quaternion.identity;s.transform.localScale=Vector3.one*scale;s.transform.localPosition=origin+new Vector3((f.rect[2]*.5f-f.anchor[0])*facing,f.anchor[1]-f.rect[3]*.5f,0)*scale;npcEffectDepths[s.transform]=depth;
  }
  static float OutskirtsRandom(int seed,int channel){unchecked{uint n=(uint)(seed*374761393+channel*668265263);n=(n^(n>>13))*1274126177;return(n^(n>>16))/(float)uint.MaxValue;}}
  static float OutskirtsRange(float[] values,int channel,int seed,int salt){return Mathf.Lerp(values[channel*2],values[channel*2+1],OutskirtsRandom(seed,salt+channel));}
  static float OutskirtsCurve(OutskirtsVisualParticle p,string property,int channel,float age,int seed,float fallback){
   float value=fallback;for(int i=0;i<p.curves.Length;i++){var c=p.curves[i];if(c.property!=property||age<c.start||channel*2+1>=c.ranges.Length)continue;float target=OutskirtsRange(c.ranges,channel,seed,101+i*7);value=c.fade&&age<c.end?Mathf.Lerp(value,target,Mathf.InverseLerp(c.start,c.end,age)):target;}return value;
  }
  static float OutskirtsVelocityIntegral(OutskirtsVisualParticle p,int channel,float age,int seed){float total=0,previous=OutskirtsCurve(p,"Velocity",channel,0,seed,0);int steps=Mathf.Max(1,Mathf.CeilToInt(age*120));for(int i=1;i<=steps;i++){float value=OutskirtsCurve(p,"Velocity",channel,age*i/steps,seed,0);total+=(previous+value)*.5f*age/steps;previous=value;}return total;}
  void AddOutskirtsParticles(string id,int count,float started,Vector3 origin,int facing,float unit,float depth,int seed,Enemy attachedOwner=null,string attachedState=null){
   var view=outskirtsParticles[id];outskirtsBursts.Add(new OutskirtsParticleBurst{view=view,count=Mathf.Min(count,view.data.capacity),started=started,origin=origin,facing=facing,unit=unit,depth=depth,seed=seed,attachedOwner=attachedOwner,attachedState=attachedState});
   if(outskirtsBursts.Count>256)outskirtsBursts.RemoveAt(0);
  }
  static Vector3 OutskirtsAttachment(OutskirtsVisualState state,OutskirtsVisualEmission emission,Enemy enemy,float at){
   var track=Array.Find(state.tracks,t=>t.bone==emission.bone);RequireOutskirtsVisual(track!=null&&track.samples!=null&&track.samples.Length>0,"missing source visual bone");float frame=Mathf.Clamp(at*60,0,track.samples.Length-1);int a=(int)frame,b=Mathf.Min(a+1,track.samples.Length-1);var p=track.samples[a];var q=track.samples[b];float u=frame-a;return new Vector3(enemy.x+enemy.facing*(Mathf.Lerp(p.x,q.x,u)+emission.offset[0])*enemy.visualScale,enemy.y+(Mathf.Lerp(p.y,q.y,u)+emission.offset[1])*enemy.visualScale,(Mathf.Lerp(p.z,q.z,u)+emission.offset[2])*enemy.visualScale);
  }
  void RenderOutskirtsNpcVisual(){
   if(outskirtsNpcVisual==null)return;
   if(outskirtsVisualRoom!=game.room||outskirtsVisualWorld!=worldRoot||outskirtsPresentationGeneration!=game.NpcPresentationGeneration||game.elapsed+.0001f<outskirtsVisualTime){ClearOutskirtsNpcVisual();outskirtsVisualRoom=game.room;outskirtsVisualWorld=worldRoot;outskirtsPresentationGeneration=game.NpcPresentationGeneration;}
   outskirtsVisualTime=game.elapsed;int used=0;
   // Lake uses the source trigger windows and explicit instance ownership in
   // its own consumer. Keep these older admitted presentation paths unchanged.
   if(outskirtsNpcVisual.schema!="besma-lake-source-visual-v18"&&outskirtsNpcVisual.schema!=AirshipSourceVisualSchemaV19&&outskirtsNpcVisual.schema!=NightSourceVisualSchemaV19)foreach(var e in game.npcPresentationEvents){if(e.sequence<=outskirtsLastEvent)continue;outskirtsLastEvent=e.sequence;
    foreach(var b in outskirtsNpcVisual.effectParticles)if(e.type==b.eventType&&e.effect==b.effect)AddOutskirtsParticles(b.particle,b.count,e.time,new Vector3(e.x,e.y,0),e.facing,.28f*e.scale,e.depth+e.z/.28f,e.sequence*67);
   }
   foreach(var effect in game.npcEffects)if(effect.id=="DamageEffect_MONKEY_BOMB_ATTACK_A"){
    // Use the committed real projectile origin and lifetime. The baked mesh
    // contains source rotate/size/color only, never another flight path.
    float age=effect.age;var position=new Vector3(effect.x,effect.y,effect.z);NpcEffectSnapshot previous;
    if(!calibrationRun&&previousRoom==game.room&&previousNpcEffects.TryGetValue(effect,out previous)&&previous.reflected==effect.reflected&&age>=previous.age){age=Mathf.Lerp(previous.age,age,RenderAlpha);position=Vector3.Lerp(previous.position,position,RenderAlpha);}
    float depth=(effect.owner==null?0:effect.owner.nativeDepth)+position.z/.28f;position.z=0;DrawOutskirtsMesh(ref used,"bomb",age,position,effect.facing,.28f*effect.ownerScale,depth);
   }
   if(outskirtsNpcVisual.effectMeshes!=null)foreach(var effect in game.npcEffects)foreach(var binding in outskirtsNpcVisual.effectMeshes){
    if(effect.id!=binding.effect)continue;
    float age=effect.age;var origin=new Vector3(effect.x,effect.y,effect.z);NpcEffectSnapshot previous;
    if(!calibrationRun&&previousRoom==game.room&&previousNpcEffects.TryGetValue(effect,out previous)&&previous.reflected==effect.reflected&&age>=previous.age){age=Mathf.Lerp(previous.age,age,RenderAlpha);origin=Vector3.Lerp(previous.position,origin,RenderAlpha);}
    float depth=(effect.owner==null?0:effect.owner.nativeDepth)+origin.z/.28f;origin.z=0;int first=used;
    DrawOutskirtsMesh(ref used,binding.mesh,age,origin,effect.facing,.28f*effect.ownerScale,depth);
    if(used>first){var sprite=outskirtsVisualSprites[first];float angle=game.NpcEffectVisual(effect,age).angle;if(outskirtsNpcVisual.schema==AirshipSourceVisualSchemaV19&&binding.compensateSourceAngle)angle-=binding.bakedTemplateRotationZ;var rotation=Quaternion.Euler(0,0,angle);var scale=new Vector3(effect.scaleX,effect.scaleY,1);sprite.transform.localPosition=origin+rotation*Vector3.Scale(sprite.transform.localPosition-origin,scale);sprite.transform.localScale=Vector3.Scale(sprite.transform.localScale,scale);sprite.transform.localRotation=rotation;SourceNpcMeshDrawFrames++;if(sourceNpcMeshesObserved.Add(binding.effect))Debug.Log("SOURCE_NPC_MESH_DRAW effect="+binding.effect+" mesh="+binding.mesh+" realEffectOrigin=true");}
   }
   foreach(var enemy in game.enemies){OutskirtsStateClock old;bool known=outskirtsStateClocks.TryGetValue(enemy,out old);outskirtsStateClocks[enemy]=new OutskirtsStateClock{state=enemy.npcState,time=enemy.npcTime};if(enemy.hp<=0||!enemy.active||enemy.dormant)continue;
    if(outskirtsNpcVisual.schema!=NightSourceVisualSchemaV19)foreach(var state in outskirtsNpcVisual.stateVisuals){if(state.npc!=enemy.sourceNPC||state.state!=enemy.npcState)continue;bool fresh=!known||old.state!=enemy.npcState||old.time>enemy.npcTime+.0001f;
     foreach(var emission in state.emissions){if(enemy.npcTime<emission.at)continue;var pos=OutskirtsAttachment(state,emission,enemy,emission.follow?enemy.npcTime:emission.at);float depth=enemy.nativeDepth+pos.z/.28f;pos.z=0;
      if(emission.kind=="mesh")DrawOutskirtsMesh(ref used,"mega-slash",enemy.npcTime-emission.at,pos,enemy.facing,state.unit*enemy.visualScale,depth);
      else if(fresh||old.time<emission.at){var p=outskirtsParticles[emission.template].data;AddOutskirtsParticles(emission.template,p.triggerCount,game.elapsed-enemy.npcTime+emission.at,pos,enemy.facing,state.unit*enemy.visualScale,depth,enemy.GetHashCode(),enemy,state.state);}
     }
    }
   }
   for(int b=outskirtsBursts.Count-1;b>=0;b--){var burst=outskirtsBursts[b];var p=burst.view.data;float rate=OutskirtsRange(p.rate,0,burst.seed,1),elapsed=game.elapsed-burst.started;if(burst.attachedOwner!=null&&(burst.attachedOwner.hp<=0||burst.attachedOwner.npcState!=burst.attachedState)||elapsed>(burst.count-1)/rate+p.lifetime[1]){outskirtsBursts.RemoveAt(b);continue;}
    for(int i=0;i<burst.count;i++){float age=elapsed-i/rate;int seed=burst.seed+i*179;if(age<0||age>=OutskirtsRange(p.lifetime,0,seed,2))continue;
     float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)continue;float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);
     var s=OutskirtsSprite(ref used,burst.view.sprite,burst.view.material,p.layer);s.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);s.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*burst.unit/s.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*burst.unit/s.sprite.rect.height,1);s.transform.localRotation=Quaternion.Euler(0,0,burst.facing*OutskirtsCurve(p,"Rotate",2,age,seed,0));
     var offset=new Vector3(OutskirtsRange(p.radius,0,seed,3)+OutskirtsVelocityIntegral(p,0,age,seed)+.5f*p.gravity[0]*age*age,OutskirtsRange(p.radius,1,seed,3)+OutskirtsVelocityIntegral(p,1,age,seed)+.5f*p.gravity[1]*age*age,OutskirtsRange(p.radius,2,seed,3)+OutskirtsVelocityIntegral(p,2,age,seed)+.5f*p.gravity[2]*age*age)*burst.unit;offset.x*=burst.facing;
     var position=burst.origin+new Vector3(offset.x,offset.y,0);if(p.useLand){var floor=game.SurfaceBelow(position.x,burst.origin.y+Mathf.Max(0,offset.y));if(floor!=null)position.y=Mathf.Max(position.y,floor.Y(position.x));}s.transform.localPosition=position;npcEffectDepths[s.transform]=burst.depth+offset.z/.28f;
    }
   }
   for(int i=used;i<outskirtsVisualSprites.Count;i++)if(outskirtsVisualSprites[i])outskirtsVisualSprites[i].gameObject.SetActive(false);
  }
 }
 }
}
