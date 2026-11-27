using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class PlainShockTextureR1480 {
  public string resource,sha256,encoding,rgbaSHA256;public int bytes,width,height;
 }
 [Serializable] public sealed class PlainShockParticleRowR1480 {
  public string key,particleJson,particleJsonSHA256;public PlainShockTextureR1480 texture;
 }
 [Serializable] public sealed class PlainShockBonePointR1480 {public float time,x,y,z;}
 [Serializable] public sealed class PlainShockParticleDocumentR1480 {
  public string schema,npc;public bool nativeEquivalent;public float unit;
  public PlainShockBonePointR1480[] boneTrack;public PlainShockParticleRowR1480[] particles;
 }
 public sealed partial class DemoScene {
  const string PlainShockDocumentSHA_R1480="542fd6f6e5636ac124275e78432408486d64eeb306f1c72b1f83cd822f6ca59c";
  const string PlainShockAttackStateR1480="SHOCK_STICK_ATTACK";
  sealed class PlainShockViewR1480 {internal OutskirtsVisualParticle data;internal Sprite sprite;internal Material material;}
  sealed class PlainShockSequenceR1480 {
   internal PlainShockViewR1480 view;internal PlainShockParticleEventR1480 source;
   internal float start,rate,land,unit,sourceAt,lastPose;internal int count,seed;internal bool trace,tail;
   internal Vector3 origin;internal float depth;
  }
  readonly Dictionary<string,PlainShockViewR1480> plainShockViewsR1480=new Dictionary<string,PlainShockViewR1480>(StringComparer.Ordinal);
  readonly List<UnityEngine.Object> plainShockOwnedR1480=new List<UnityEngine.Object>();
  readonly List<PlainShockSequenceR1480> plainShockSequencesR1480=new List<PlainShockSequenceR1480>();
  readonly List<SpriteRenderer> plainShockSpritesR1480=new List<SpriteRenderer>();
  readonly List<Transform> plainShockTransformsR1480=new List<Transform>();
  PlainShockParticleDocumentR1480 plainShockDocumentR1480;GameModel plainShockModelR1480;
  Transform plainShockWorldR1480,plainShockRootR1480;int plainShockGenerationR1480=-1,plainShockRoomR1480=-1,plainShockCursorR1480;
  float plainShockTimeR1480=-1;
  public int PlainShockParticleVisibleSpritesR1480 {get;private set;}
  public int PlainShockParticleEventsObservedR1480 {get;private set;}
  public int PlainShockParticleDrawSubmissionsR1480 {get;private set;}
  public IList<SpriteRenderer> PlainShockParticleSpritesR1480 {get{return plainShockSpritesR1480.AsReadOnly();}}
  static void PlainShockNeedR1480(bool valid,string why){if(!valid)throw new InvalidOperationException("Plain Shock original particles: "+why);}
  static string PlainShockHashR1480(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  static byte[] PlainShockAssetR1480(string key,string sha,int count){
   var asset=Resources.Load<TextAsset>(key);PlainShockNeedR1480(asset,"Missing exact original resource "+key);var bytes=asset.bytes;
   PlainShockNeedR1480(bytes.Length==count&&PlainShockHashR1480(bytes)==sha,"Changed exact original resource "+key);return bytes;
  }
  Texture2D PlainShockTextureLoadR1480(PlainShockTextureR1480 d){
   byte[] bytes=PlainShockAssetR1480(d.resource,d.sha256,d.bytes);Texture2D texture=null;
   try{
    if(d.encoding=="RGBA32_TOP_DOWN"){
     PlainShockNeedR1480(d.width>0&&d.height>0&&d.width<=4096&&d.height<=4096&&bytes.Length==checked(d.width*d.height*4)&&d.sha256==d.rgbaSHA256,"Exact source RGBA extent");
     var bottomUp=new byte[bytes.Length];for(int y=0;y<d.height;y++)Buffer.BlockCopy(bytes,y*d.width*4,bottomUp,(d.height-1-y)*d.width*4,d.width*4);
     texture=new Texture2D(d.width,d.height,TextureFormat.RGBA32,false);texture.LoadRawTextureData(bottomUp);texture.Apply(false,false);
    }else{
     PlainShockNeedR1480(d.encoding=="PNG_EXISTING_ORIGINAL","Unsupported source texture encoding");texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
     PlainShockNeedR1480(ImageConversion.LoadImage(texture,bytes,false),"Existing original texture decode");
    }
    PlainShockNeedR1480(BelowVisualRgbaHashV21(texture)==d.rgbaSHA256,"Original decoded RGBA changed");
    texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;texture.name="Original Shock / "+d.sha256;plainShockOwnedR1480.Add(texture);return texture;
   }catch{if(texture)BelowVisualDestroyV21(texture);throw;}
  }
  void ConfigurePlainShockParticlesR1480(){
   DisposePlainShockParticlesR1480();if(plainProductRouteR1320!=PlainProductResourceIndexR1320.Normal)return;
   PlainShockNeedR1480(plainProductSessionR1320&&ReferenceEquals(game,plainProductModelR1320),"Owned normal Plain model required");
   var bytes=PlainShockAssetR1480("Pixel/dungeons/altera-plain/shock-r1480/document",PlainShockDocumentSHA_R1480,46205);
   var d=JsonUtility.FromJson<PlainShockParticleDocumentR1480>(new UTF8Encoding(false,true).GetString(bytes));
   PlainShockNeedR1480(d!=null&&d.schema=="PLAIN_SHOCK_ORIGINAL_PARTICLE_DISPLAY_R1480"&&!d.nativeEquivalent&&d.npc=="NUI_SHOCK_STICK"&&d.unit==.28f&&d.particles!=null&&d.particles.Length==8&&d.boneTrack!=null&&d.boneTrack.Length==30,"Exact finite original document");
   var textures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
   try{
    foreach(var row in d.particles){
     PlainShockNeedR1480(row!=null&&row.texture!=null&&PlainShockHashR1480(Encoding.UTF8.GetBytes(row.particleJson))==row.particleJsonSHA256&&!plainShockViewsR1480.ContainsKey(row.key),"Unique original source template");
     var p=JsonUtility.FromJson<OutskirtsVisualParticle>(row.particleJson);ValidateLakeParticleV18(p);
     PlainShockNeedR1480(p.id==row.key&&p.particleType=="PT_3D_PLANE"&&(p.billboardType=="BT_ALL"||p.id=="Major/ExplodeImpactRingRed"&&p.billboardType=="BT_NONE"),"Original particle plane basis");
     Texture2D texture;if(!textures.TryGetValue(row.texture.resource,out texture)){texture=PlainShockTextureLoadR1480(row.texture);textures.Add(row.texture.resource,texture);}
     var sprite=PixelExplosionVisuals.Plane(p,texture);plainShockOwnedR1480.Add(sprite);
     var material=OutskirtsMaterial(p.sourceBlend,p.destinationBlend);outskirtsMaterials.Remove(material);plainShockOwnedR1480.Add(material);
     plainShockViewsR1480.Add(row.key,new PlainShockViewR1480{data=p,sprite=sprite,material=material});
    }
    plainShockDocumentR1480=d;plainShockModelR1480=game;PlainShockParticleEventsObservedR1480=PlainShockParticleDrawSubmissionsR1480=0;
   }catch{DisposePlainShockParticlesR1480();throw;}
  }
  void ClearPlainShockInstancesR1480(){plainShockSequencesR1480.Clear();foreach(var s in plainShockSpritesR1480)if(s)s.gameObject.SetActive(false);PlainShockParticleVisibleSpritesR1480=0;}
  void ClearPlainShockPoolR1480(){
   foreach(var t in plainShockTransformsR1480)if(!ReferenceEquals(t,null))npcEffectDepths.Remove(t);
   foreach(var s in plainShockSpritesR1480)if(s)RemoveSceneObject(s.gameObject);plainShockSpritesR1480.Clear();plainShockTransformsR1480.Clear();
  }
  void DisposePlainShockParticlesR1480(){
   ClearPlainShockInstancesR1480();ClearPlainShockPoolR1480();for(int i=plainShockOwnedR1480.Count-1;i>=0;i--)BelowVisualDestroyV21(plainShockOwnedR1480[i]);plainShockOwnedR1480.Clear();plainShockViewsR1480.Clear();
   plainShockDocumentR1480=null;plainShockModelR1480=null;plainShockWorldR1480=plainShockRootR1480=null;plainShockGenerationR1480=plainShockRoomR1480=-1;plainShockCursorR1480=0;plainShockTimeR1480=-1;
  }
  Vector3 PlainShockBoneR1480(float time){
   var a=plainShockDocumentR1480.boneTrack[0];foreach(var b in plainShockDocumentR1480.boneTrack){if(b.time>=time){float u=b.time>a.time?Mathf.InverseLerp(a.time,b.time,time):0;return new Vector3(Mathf.Lerp(a.x,b.x,u),Mathf.Lerp(a.y,b.y,u),Mathf.Lerp(a.z,b.z,u));}a=b;}return new Vector3(a.x,a.y,a.z);
  }
  Vector3 PlainShockBirthOriginR1480(PlainShockSequenceR1480 sequence,float sourcePose,out float depth){
   var e=sequence.source;depth=sequence.depth;if(sequence.sourceAt<0)return sequence.origin;
   var bone=PlainShockBoneR1480(sourcePose);depth=e.NativeDepth+bone.z*e.VisualScale/.28f+(sequence.view.data.id=="Major/Light_Lightning_Shock02"?-15:0);
   return new Vector3(e.X+bone.x*e.Facing*e.VisualScale,e.Y+bone.y*e.VisualScale,0);
  }
  void StartPlainShockSequenceR1480(PlainShockParticleEventR1480 e,string key,float sourceAt,int count,bool trace=false,bool tail=false){
   PlainShockNeedR1480(plainShockSequencesR1480.Count<256,"Finite original sequence capacity");var view=plainShockViewsR1480[key];var p=view.data;int seed=unchecked(e.Serial*997+key.Length*193);
   var s=new PlainShockSequenceR1480{source=e,view=view,start=e.Time,sourceAt=sourceAt,lastPose=sourceAt,unit=.28f*e.VisualScale,land=e.LandY,rate=OutskirtsRange(p.rate,0,seed,1),count=count,seed=seed,trace=trace,tail=tail,origin=new Vector3(e.X,e.Y-50*.28f,0),depth=e.NativeDepth+5};
   if(sourceAt>=0)s.origin=PlainShockBirthOriginR1480(s,sourceAt,out s.depth);plainShockSequencesR1480.Add(s);
  }
  void CollectPlainShockParticlesR1480(){
   foreach(var e in game.PlainShockParticleEventsR1480){if(e.Serial<=plainShockCursorR1480)continue;plainShockCursorR1480=e.Serial;
    if(e.Generation!=game.NpcPresentationGeneration||e.Room!=game.room||e.Time>game.elapsed||game.elapsed-e.Time>4)continue;
    PlainShockNeedR1480(e.Actor!=null&&e.Actor.sourceNPC=="NUI_SHOCK_STICK"&&(e.Facing==1||e.Facing==-1)&&e.VisualScale>0,"Committed original actor snapshot");
    if(e.Kind==0){StartPlainShockSequenceR1480(e,"Major/Light_Lightning_Shock02",.1f,18,true);StartPlainShockSequenceR1480(e,"Major/Counter_Particle02",.1f,30);}
    else if(e.Kind==1)StartPlainShockSequenceR1480(e,"Major/Lightning_Shock01",.233f,1);
    else if(e.Kind==2){StartPlainShockSequenceR1480(e,"Major/Nasod_ExplodeSmoke",-1,40);StartPlainShockSequenceR1480(e,"Major/Nasod_ExplodeTail",-1,3,false,true);StartPlainShockSequenceR1480(e,"Major/ExplodeImpactRingRed",-1,1);StartPlainShockSequenceR1480(e,"Major/ExplodeImpactSlashRed",-1,1);}
    else throw new InvalidOperationException("Unknown Shock original particle event");PlainShockParticleEventsObservedR1480++;
   }
  }
  SpriteRenderer PlainShockSpriteR1480(ref int used){
   PlainShockNeedR1480(used<4096,"Finite original particle display capacity");while(plainShockSpritesR1480.Count<=used){plainShockSpritesR1480.Add(null);plainShockTransformsR1480.Add(null);}
   var s=plainShockSpritesR1480[used];if(!s||s.transform.parent!=effectsRoot){var old=plainShockTransformsR1480[used];if(!ReferenceEquals(old,null))npcEffectDepths.Remove(old);if(s)RemoveSceneObject(s.gameObject);s=WorldSprite("Original Plain Shock particle",effectsRoot,null,44,Vector3.zero);plainShockSpritesR1480[used]=s;plainShockTransformsR1480[used]=s.transform;}
   used++;s.gameObject.SetActive(true);s.enabled=true;s.flipX=false;return s;
  }
  Vector3 PlainShockParticlePositionR1480(OutskirtsVisualParticle p,Vector3 origin,float depth,float age,int seed,float unit,float land,out float resultDepth,bool initialVelocityOnly=false){
   var offset=Vector3.zero;for(int axis=0;axis<3;axis++){float gravity=p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,axis,seed,301):p.gravity[axis];offset[axis]=(OutskirtsRange(p.radius,axis,seed,3)+(initialVelocityOnly?OutskirtsCurve(p,"Velocity",axis,0,seed,0)*age:OutskirtsVelocityIntegral(p,axis,age,seed))+.5f*gravity*age*age)*unit;}
   var point=origin+new Vector3(offset.x,offset.y,0);if(p.useLand)point.y=Mathf.Max(point.y,land);resultDepth=depth+offset.z/.28f;return point;
  }
  bool DrawPlainShockParticleR1480(ref int used,PlainShockViewR1480 view,Vector3 point,float depth,float age,int seed,float unit){
   var p=view.data;float alpha=Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1));if(alpha<=0)return false;
   for(int pass=0;pass<p.drawCount;pass++){
    var s=PlainShockSpriteR1480(ref used);s.sprite=view.sprite;s.sharedMaterial=view.material;s.sortingOrder=44+p.layer-14;
    s.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);
    float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);s.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*unit/s.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*unit/s.sprite.rect.height,1);
    float rx=OutskirtsCurve(p,"Rotate",0,age,seed,0),ry=OutskirtsCurve(p,"Rotate",1,age,seed,0),rz=OutskirtsCurve(p,"Rotate",2,age,seed,0);s.transform.localRotation=p.billboardType=="BT_NONE"?Quaternion.Euler(rx,ry,rz):Quaternion.Euler(0,0,rz);s.transform.localPosition=point;npcEffectDepths[s.transform]=depth;PlainShockParticleDrawSubmissionsR1480++;
   }return true;
  }
  void DrawPlainShockTailR1480(ref int used,PlainShockSequenceR1480 s,float elapsed){
   var child=plainShockViewsR1480[BelowFiniteTailFxR294.Smoke];
   for(int parent=0;parent<3;parent++){
    int seed=unchecked(s.seed+parent*179);float depth;var point=PlainShockParticlePositionR1480(s.view.data,s.origin,s.depth,Mathf.Min(elapsed,2.5f),seed,s.unit,s.land,out depth,true);
    if(elapsed<2.5f)DrawPlainShockParticleR1480(ref used,s.view,point,depth,elapsed,seed,s.unit);
    double rate=OutskirtsRange(child.data.rate,0,seed,1);int visible=0;
    for(int index=BelowFiniteTailFxR294.TraceBirthCount(2,rate)-1;index>=0&&visible<child.data.capacity;index--){
     if(!BelowFiniteTailFxR294.TraceVisible(2,elapsed,index,rate))continue;float born=(float)BelowFiniteTailFxR294.TraceBirth(2,index,rate),age=elapsed-born;int cseed=BelowFiniteTailFxR294.ChildSeed(seed,index);
     var anchor=PlainShockParticlePositionR1480(s.view.data,s.origin,s.depth,born,seed,s.unit,s.land,out depth,true);var pos=PlainShockParticlePositionR1480(child.data,anchor,depth,age,cseed,s.unit,s.land,out depth,true);
     if(DrawPlainShockParticleR1480(ref used,child,pos,depth,age,cseed,s.unit))visible++;
    }
   }
  }
  void RenderPlainShockParticlesR1480(){
   if(plainShockDocumentR1480==null)return;
   PlainShockNeedR1480(PlainProductSessionActiveR1320&&plainProductRouteR1320==PlainProductResourceIndexR1320.Normal&&ReferenceEquals(game,plainShockModelR1480),"Plain visual owner/model changed");
   bool first=plainShockGenerationR1480<0,newGeneration=plainShockGenerationR1480!=game.NpcPresentationGeneration;
   if(first||newGeneration||plainShockRoomR1480!=game.room||game.elapsed<plainShockTimeR1480||plainShockWorldR1480!=worldRoot||plainShockRootR1480!=effectsRoot){
    ClearPlainShockInstancesR1480();ClearPlainShockPoolR1480();if(first||newGeneration)plainShockCursorR1480=0;
    plainShockGenerationR1480=game.NpcPresentationGeneration;plainShockRoomR1480=game.room;plainShockWorldR1480=worldRoot;plainShockRootR1480=effectsRoot;
   }
   plainShockTimeR1480=game.elapsed;CollectPlainShockParticlesR1480();if(!worldRoot||!effectsRoot){ClearPlainShockPoolR1480();return;}int used=0;
   for(int index=plainShockSequencesR1480.Count-1;index>=0;index--){
    var s=plainShockSequencesR1480[index];var p=s.view.data;float elapsed=game.elapsed-s.start;if(elapsed<0)continue;float end=s.tail?3.1f:(s.count-1)/s.rate+p.lifetime[1];
    if(elapsed>=end){plainShockSequencesR1480.RemoveAt(index);continue;}if(s.tail){DrawPlainShockTailR1480(ref used,s,elapsed);continue;}
    if(s.trace&&s.source.Actor.npcState==PlainShockAttackStateR1480)s.lastPose=Mathf.Clamp(s.source.Actor.npcTime,s.sourceAt,28f/60);
    int top=Math.Min(s.count-1,Mathf.FloorToInt(elapsed*s.rate)),visible=0;
    for(int i=top;i>=0&&visible<p.capacity;i--){float birth=i/s.rate,age=elapsed-birth;int seed=unchecked(s.seed+i*179);if(age>=OutskirtsRange(p.lifetime,0,seed,2))continue;
     float depth=s.depth;Vector3 origin=s.origin;if(s.trace)origin=PlainShockBirthOriginR1480(s,s.lastPose,out depth);
     var point=PlainShockParticlePositionR1480(p,origin,depth,age,seed,s.unit,s.land,out depth);if(DrawPlainShockParticleR1480(ref used,s.view,point,depth,age,seed,s.unit))visible++;
    }
   }
   for(int i=used;i<plainShockSpritesR1480.Count;i++)if(plainShockSpritesR1480[i])plainShockSpritesR1480[i].gameObject.SetActive(false);PlainShockParticleVisibleSpritesR1480=used;
  }
 }
}
