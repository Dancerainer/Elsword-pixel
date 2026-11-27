using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ElswordPixel { public sealed partial class DemoScene {
 sealed partial class BattleProductViewR1325 {

  sealed class AirshipStateViewV20 {internal OutskirtsVisualParticle particle;internal Sprite sprite;internal Sprite[] frames;internal AirshipStateMeshV20 mesh;internal Material material;internal bool supported;}
  sealed class AirshipStateSequenceV20 {internal AirshipStateViewV20 view;internal Enemy owner;internal int handle,seed,facing,per,group;internal long total,born;internal float started,unit,x,y,depth,land,rate,stop=float.PositiveInfinity;internal bool continuous;}
  AirshipStatePayloadV20 airshipStatePayloadV20;
  readonly Dictionary<string,AirshipStateViewV20> airshipStateViewsV20=new Dictionary<string,AirshipStateViewV20>();
  readonly List<AirshipStateSequenceV20> airshipStateSequencesV20=new List<AirshipStateSequenceV20>();
  readonly List<SpriteRenderer> airshipStateSpritesV20=new List<SpriteRenderer>();readonly List<Transform> airshipStateTransformsV20=new List<Transform>();readonly List<Material> airshipStateMaterialsV20=new List<Material>();
  readonly HashSet<AlteraRepairmanEventV18> airshipStateRepairSeenV20=new HashSet<AlteraRepairmanEventV18>();readonly HashSet<string> airshipStateKindsV20=new HashSet<string>();
  readonly HashSet<AlteraRavenMaximumReleaseV18> airshipStateMaximumSeenV20=new HashSet<AlteraRavenMaximumReleaseV18>();
  readonly List<AirshipStateCameraRequestV20> airshipStateCameraRequestsV20=new List<AirshipStateCameraRequestV20>();
  Transform airshipStateWorldV20,airshipStateEffectsRootV20;int airshipStateRoomV20=-1,airshipStateGenerationV20=-1,airshipStateFeedGenerationV20=-1,airshipStateCursorV20;float airshipStateTimeV20=-1;bool airshipStateEnabledV20;
  public int AlteraAirshipStateDrawFramesV20 {get;private set;}
  public int AlteraAirshipStateCreatedSequencesV20 {get;private set;}
  public int AlteraAirshipStateRepairRequestsV20 {get;private set;}
  public int AlteraAirshipStateAutoDieV20 {get;private set;}
  public int AlteraAirshipStateMaximumCameraRequestsV20 {get;private set;}
  public int AlteraAirshipStateCameraGenerationV20 {get;private set;}
  public long AlteraAirshipStateParticleBirthsV20 {get;private set;}
  public int AlteraAirshipStateLiveSequencesV20 {get{return airshipStateSequencesV20.Count;}}
  public int AlteraAirshipStatePoolCountV20 {get{return airshipStateSpritesV20.Count;}}
  public int AlteraAirshipStateKindsDrawnV20 {get{return airshipStateKindsV20.Count;}}
  public IList<SpriteRenderer> AlteraAirshipStateSpritesV20 {get{return airshipStateSpritesV20.AsReadOnly();}}
  public IList<AirshipStateCameraRequestV20> AlteraAirshipStateCameraRequestsV20 {get{return airshipStateCameraRequestsV20.AsReadOnly();}}
  static void AirshipStateRequireV20(bool valid,string reason){if(!valid)throw new InvalidOperationException("Airship state visual: "+reason);}
  static void AirshipStateDestroyV20(UnityEngine.Object item){if(!item)return;if(Application.isPlaying)Destroy(item);else DestroyImmediate(item);}
  static string AirshipStateRgbaV20(Texture2D t){var pixels=t.GetPixels32();var bytes=new byte[pixels.Length*4];int n=0;for(int y=t.height-1;y>=0;y--)for(int x=0;x<t.width;x++){var p=pixels[y*t.width+x];bytes[n++]=p.r;bytes[n++]=p.g;bytes[n++]=p.b;bytes[n++]=p.a;}return AirshipStateDataV20.Hash(bytes);}
  public void ConfigureAlteraAirshipStateVisualV20(AlteraAirshipStateDocumentV20 doc,AirshipStatePayloadV20 typedPayload,IDictionary<string,Texture2D> textures){
   if(doc==null){DisposeAlteraAirshipStateVisualV20();return;}
   AirshipStateRequireV20(doc.schema=="altera-airship-state-runtime-v20"&&!doc.runtimeAdmitted&&doc.payloadSha256==AirshipStatePinsV20.Payload&&AirshipStateDataV20.Hash(doc.payloadJson)==doc.payloadSha256&&doc.feedSha256==AirshipStatePinsV20.Feed&&doc.sourceJsonSha256==AirshipStatePinsV20.Source&&AirshipStateDataV20.Hash(doc.sourceJson)==doc.sourceJsonSha256&&doc.sourceRulesSha256==AirshipStatePinsV20.Rules&&doc.sourceEffectsSha256==AirshipStatePinsV20.Effects,"Changed strong schema/original IR/payload/source pins");
   AirshipStateRequireV20(doc.particleAdapter=="source-BT_ALL-XY-Zspin-hold-fixed-basis-trace-final-v20"&&doc.sequenceAdapter=="source-seq-bone-snapshot-boolean-slots-retained-unmeasured-v20"&&doc.triggerAdapter=="source-count-window-rate-per-birth-capacity-visible-only-v20"&&doc.drawCountAdapter=="one-sprite-per-source-particle-multidraw-declared-v20"&&doc.meshAdapter=="original-point-baked-Size-Rotate-Color-no-doublecurve-flight-separate-v20"&&doc.ownerSlotAdapter=="source-Minor-slot0-persistent-across-state-entry-until-stop-v20"&&doc.cameraAdapter=="source-requests-only-projection-owner-Root-v20"&&doc.lifecycleAdapter=="CLR-Transform-owned-pool-generation-world-room-rewind-no-replay-v20","Undeclared adapter");
   var p=typedPayload;
   AirshipStateRequireV20(p!=null&&p.schema=="altera-airship-state-payload-v20"&&p.templates.Length==AirshipStatePinsV20.Templates&&p.bindings.Length==AirshipStatePinsV20.Bindings&&p.bones.Length==AirshipStatePinsV20.Bones&&p.meshes.Length==3&&p.textures.Length==AirshipStatePinsV20.Textures&&textures!=null&&textures.Count==p.textures.Length&&AirshipStateDataV20.FeedHash(p.bindings,p.bones)==AirshipStatePinsV20.Feed,"Missing/changed typed data");
   var next=new Dictionary<string,AirshipStateViewV20>();var mats=new List<Material>();
   try{
    foreach(var row in p.textures){Texture2D t;AirshipStateRequireV20(textures.TryGetValue(row.key,out t)&&t&&t.isReadable&&t.width==row.width&&t.height==row.height&&AirshipStateRgbaV20(t)==row.rgbaSha256,"Changed source RGBA/size "+row.key);t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;}
    foreach(var row in p.templates){var data=BattleProductTypedJsonR1282.Read<OutskirtsVisualParticle>(new UTF8Encoding(false,true).GetBytes(row.particleJson));ValidateLakeParticleV18(data);bool mesh=!string.IsNullOrEmpty(row.meshComponent);bool unknown=Array.Exists(data.curves,c=>c.property!="Color"&&c.property!="Size"&&c.property!="Rotate"&&c.property!="Velocity"&&c.property!="Scale");bool supported=(mesh||data.billboardType=="BT_ALL")&&!unknown&&data.id!="Minor/DieLight";
     AirshipStateRequireV20(row.supported==supported&&(supported?string.IsNullOrEmpty(row.holdReason):!string.IsNullOrEmpty(row.holdReason)),"Partially unsupported template enabled");var view=new AirshipStateViewV20{particle=data,supported=supported};next.Add(row.key,view);if(!supported)continue;
     view.material=OutskirtsMaterial(data.sourceBlend,data.destinationBlend);outskirtsMaterials.Remove(view.material);mats.Add(view.material);
     if(mesh){view.mesh=Array.Find(p.meshes,m=>m.id==row.meshComponent);AirshipStateRequireV20(view.mesh!=null&&view.mesh.frames.Length>0&&view.mesh.fps==60&&view.mesh.pixelsPerNativeUnit>0,"Missing exact Point mesh");view.frames=new Sprite[view.mesh.frames.Length];for(int i=0;i<view.frames.Length;i++){var frame=view.mesh.frames[i];Texture2D texture;AirshipStateRequireV20(textures.TryGetValue("components/"+frame.atlas,out texture)&&texture&&frame.rect.Length==4&&frame.anchor.Length==2,"Missing Point page/origin");var r=frame.rect;AirshipStateRequireV20(r[0]>=0&&r[1]>=0&&r[2]>0&&r[3]>0&&r[0]+r[2]<=texture.width&&r[1]+r[3]<=texture.height,"Invalid Point rect");view.frames[i]=Sprite.Create(texture,new Rect(r[0],texture.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1);}}
     else{Texture2D texture;AirshipStateRequireV20(textures.TryGetValue(data.texture,out texture)&&texture,"Missing source plane texture");view.sprite=PixelExplosionVisuals.Plane(data,texture);}
    }
   }catch{foreach(var view in next.Values){AirshipStateDestroyV20(view.sprite);if(view.frames!=null)foreach(var s in view.frames)AirshipStateDestroyV20(s);}foreach(var m in mats)AirshipStateDestroyV20(m);throw;}
   DisposeAlteraAirshipStateVisualV20();airshipStatePayloadV20=p;foreach(var v in next)airshipStateViewsV20.Add(v.Key,v.Value);airshipStateMaterialsV20.AddRange(mats);game.SetAlteraAirshipStatePresentationV20(p.bindings,p.bones);airshipStateEnabledV20=true;
   airshipStateWorldV20=worldRoot;airshipStateEffectsRootV20=effectsRoot;airshipStateRoomV20=game.room;airshipStateGenerationV20=game.NpcPresentationGeneration;airshipStateFeedGenerationV20=game.AlteraAirshipStateGenerationV20;airshipStateTimeV20=game.elapsed;
   foreach(var e in game.alteraRepairmanEventsV18)airshipStateRepairSeenV20.Add(e);foreach(var e in game.alteraRavenMaximumReleasesV18)airshipStateMaximumSeenV20.Add(e);
   AlteraAirshipStateDrawFramesV20=AlteraAirshipStateCreatedSequencesV20=AlteraAirshipStateRepairRequestsV20=AlteraAirshipStateAutoDieV20=AlteraAirshipStateMaximumCameraRequestsV20=0;AlteraAirshipStateParticleBirthsV20=0;
  }
  void ClearAirshipStatePoolV20(){foreach(var t in airshipStateTransformsV20)if(!ReferenceEquals(t,null))npcEffectDepths.Remove(t);foreach(var s in airshipStateSpritesV20)if(s)RemoveSceneObject(s.gameObject);airshipStateSpritesV20.Clear();airshipStateTransformsV20.Clear();}
  void ClearAirshipStateInstancesV20(){airshipStateSequencesV20.Clear();airshipStateCameraRequestsV20.Clear();AlteraAirshipStateCameraGenerationV20++;airshipRepairSeedV20=0;foreach(var s in airshipStateSpritesV20)if(s)s.gameObject.SetActive(false);}
  public void DisposeAlteraAirshipStateVisualV20(){ClearAirshipStateInstancesV20();ClearAirshipStatePoolV20();airshipStateRepairSeenV20.Clear();airshipStateMaximumSeenV20.Clear();foreach(var view in airshipStateViewsV20.Values){AirshipStateDestroyV20(view.sprite);if(view.frames!=null)foreach(var s in view.frames)AirshipStateDestroyV20(s);}foreach(var m in airshipStateMaterialsV20)AirshipStateDestroyV20(m);airshipStateViewsV20.Clear();airshipStateMaterialsV20.Clear();airshipStateKindsV20.Clear();airshipStatePayloadV20=null;airshipStateEnabledV20=false;airshipStateWorldV20=airshipStateEffectsRootV20=null;airshipStateRoomV20=airshipStateGenerationV20=airshipStateFeedGenerationV20=-1;airshipStateCursorV20=0;airshipStateTimeV20=-1;if(game!=null)game.SetAlteraAirshipStatePresentationV20(null,null);}
  // A display toggle must not destroy an original still-emitting owner slot.
  // Keep its exact object/handle/started/seed; only transient requests are
  // discarded. While hidden we still consume source slot starts/stops, so an
  // AutoDie or owner removal cannot later resurrect a stale emitter.
  public void SetAlteraAirshipStateVisualEnabledV20(bool enabled){
   if(airshipStatePayloadV20==null||enabled==airshipStateEnabledV20)return;
   game.SynchronizeAlteraAirshipStatePresentationV20();CollectAirshipStateEventsV20(true);
   airshipStateSequencesV20.RemoveAll(s=>s.handle<=0);airshipStateCameraRequestsV20.Clear();AlteraAirshipStateCameraGenerationV20++;
   PruneHiddenAirshipHpTailsV20();foreach(var s in airshipStateSpritesV20)if(s)s.gameObject.SetActive(false);
   airshipStateEnabledV20=enabled;
  }
  void PruneHiddenAirshipHpTailsV20(){
   for(int i=airshipStateSequencesV20.Count-1;i>=0;i--){var s=airshipStateSequencesV20[i];if(s.handle>0&&!s.continuous&&game.elapsed-s.started>s.stop+s.view.particle.lifetime[1])airshipStateSequencesV20.RemoveAt(i);}
  }
  SpriteRenderer AirshipStateSpriteV20(int slot){AirshipStateRequireV20(slot<8192,"Source display capacity exceeded");while(airshipStateSpritesV20.Count<=slot){airshipStateSpritesV20.Add(null);airshipStateTransformsV20.Add(null);}var s=airshipStateSpritesV20[slot];var t=airshipStateTransformsV20[slot];if(!s||!ReferenceEquals(s.transform.parent,effectsRoot)){if(!ReferenceEquals(t,null))npcEffectDepths.Remove(t);if(s)RemoveSceneObject(s.gameObject);s=WorldSprite("Original Airship state source",effectsRoot,null,44,Vector3.zero);airshipStateSpritesV20[slot]=s;airshipStateTransformsV20[slot]=s.transform;}s.gameObject.SetActive(true);s.enabled=true;return s;}
  static int AirshipStateSeedV20(string text){unchecked{uint h=2166136261;foreach(char c in text??"")h=(h^c)*16777619;return (int)h;}}
  void AddAirshipStateSequenceV20(AirshipStatePresentationEventV20 e){AirshipStateViewV20 view;if(!airshipStateViewsV20.TryGetValue(e.particleKey??"",out view)||!view.supported)return;var p=view.particle;int seed=unchecked(e.sequence*7369+AirshipStateSeedV20(e.binding));float rate=OutskirtsRange(p.rate,0,seed,1);int per=p.triggerCount;bool continuous=e.kind=="owner-slot-start";var sequence=new AirshipStateSequenceV20{view=view,owner=e.owner,handle=e.handle,seed=seed,facing=e.facing,per=per,group=p.triggerTime>0||continuous?per:1,total=p.triggerTime>0?(long)Math.Ceiling(p.triggerTime*rate)*per:per,rate=rate,started=e.time,x=e.x,y=e.y,depth=e.depth,land=e.landY,unit=e.unit,continuous=continuous};airshipStateSequencesV20.Add(sequence);AlteraAirshipStateCreatedSequencesV20++;}
  void AddAirshipStateCameraV20(string source,float time,float magnitude,float duration){if(airshipStateCameraRequestsV20.Count==256)airshipStateCameraRequestsV20.RemoveAt(0);airshipStateCameraRequestsV20.Add(new AirshipStateCameraRequestV20{sequence=airshipStateCameraRequestsV20.Count==0?1:airshipStateCameraRequestsV20[airshipStateCameraRequestsV20.Count-1].sequence+1,source=source,time=time,magnitude=magnitude,duration=duration});}
  void CollectAirshipStateEventsV20(bool hpSlotsOnly=false){
   foreach(var e in game.AlteraAirshipStateEventsV20){if(e.sequence<=airshipStateCursorV20)continue;airshipStateCursorV20=e.sequence;if(e.kind=="auto-die"){foreach(var s in airshipStateSequencesV20)if(s.handle==e.handle){s.stop=Math.Min(s.stop,e.time-s.started);s.total=Math.Max(s.total,(long)Math.Ceiling(Math.Max(0,s.stop)*s.rate)*s.group);s.continuous=false;}AlteraAirshipStateAutoDieV20++;}else if(e.kind=="owner-slot-start")AddAirshipStateSequenceV20(e);else if(!hpSlotsOnly){if(e.kind=="camera")AddAirshipStateCameraV20(e.binding,e.time,e.magnitude,e.cameraDuration);else AddAirshipStateSequenceV20(e);}}
   var live=new HashSet<AlteraRepairmanEventV18>(game.alteraRepairmanEventsV18);airshipStateRepairSeenV20.IntersectWith(live);
   foreach(var e in game.alteraRepairmanEventsV18){if(e.type!="source-visual-request"||!airshipStateRepairSeenV20.Add(e)||hpSlotsOnly)continue;var kind=Array.Find(airshipStatePayloadV20.repairRequests,r=>r.template==e.source);AirshipStateRequireV20(kind!=null,"Unknown actual Repairman source request");var floor=game.SurfaceBelow(e.x,e.y+.05f);AddAirshipStateSequenceV20(new AirshipStatePresentationEventV20{sequence=++airshipRepairSeedV20,binding="actual-repair-request/"+e.source,kind="spawn",particleKey=kind.particleKey,time=e.time,x=e.x,y=e.y,depth=e.z,landY=floor==null?e.y:floor.Y(e.x),unit=.28f,facing=1});AlteraAirshipStateRepairRequestsV20++;}
   airshipStateMaximumSeenV20.IntersectWith(game.alteraRavenMaximumReleasesV18);foreach(var e in game.alteraRavenMaximumReleasesV18){if(e.kind!="source-camera-request"||!airshipStateMaximumSeenV20.Add(e)||hpSlotsOnly)continue;AddAirshipStateCameraV20("existing-Maximum/.01",e.time,e.sourceCameraMagnitude,e.sourceCameraDuration);AlteraAirshipStateMaximumCameraRequestsV20++;}
  }
  int airshipRepairSeedV20;
  public bool AlteraAirshipGaugeVisibleV20(Enemy owner,bool otherwiseVisible){return owner!=null&&owner.sourceNPC=="NUI_CANNON_BIG_DOWN"?otherwiseVisible&&game.AlteraCannonGateGaugeVisible(owner):otherwiseVisible;}
  public void RenderAlteraAirshipStateVisualV20(){
   if(airshipStatePayloadV20==null)return;game.SynchronizeAlteraAirshipStatePresentationV20();int generation=game.NpcPresentationGeneration,feed=game.AlteraAirshipStateGenerationV20;bool first=airshipStateGenerationV20<0,rewind=game.elapsed+.000001f<airshipStateTimeV20;
   if(first||generation!=airshipStateGenerationV20||feed!=airshipStateFeedGenerationV20||game.room!=airshipStateRoomV20||rewind){bool newFeed=first||feed!=airshipStateFeedGenerationV20;ClearAirshipStateInstancesV20();ClearAirshipStatePoolV20();airshipStateCursorV20=newFeed?0:game.AlteraAirshipLastPresentationSequenceV20;if(!newFeed){foreach(var e in game.alteraRepairmanEventsV18)airshipStateRepairSeenV20.Add(e);foreach(var e in game.alteraRavenMaximumReleasesV18)airshipStateMaximumSeenV20.Add(e);}airshipStateGenerationV20=generation;airshipStateFeedGenerationV20=feed;airshipStateRoomV20=game.room;}
   if(!ReferenceEquals(airshipStateWorldV20,worldRoot)||!ReferenceEquals(airshipStateEffectsRootV20,effectsRoot)){ClearAirshipStatePoolV20();airshipStateWorldV20=worldRoot;airshipStateEffectsRootV20=effectsRoot;}airshipStateTimeV20=game.elapsed;
   if(!airshipStateEnabledV20){CollectAirshipStateEventsV20(true);PruneHiddenAirshipHpTailsV20();return;}CollectAirshipStateEventsV20();if(!worldRoot||!effectsRoot){ClearAirshipStatePoolV20();return;}
   int used=0;
   for(int q=airshipStateSequencesV20.Count-1;q>=0;q--){var sequence=airshipStateSequencesV20[q];var p=sequence.view.particle;float elapsed=game.elapsed-sequence.started;if(elapsed<0)continue;
    long top=sequence.continuous?(long)Math.Floor(elapsed*sequence.rate)*sequence.group+sequence.group-1:sequence.total-1;if(!float.IsPositiveInfinity(sequence.stop))top=Math.Min(top,(long)Math.Ceiling(Math.Max(0,sequence.stop)*sequence.rate)*sequence.group-1);
    long born=Math.Max(0,Math.Min(top+1,(long)Math.Floor(elapsed*sequence.rate)*sequence.group+sequence.group));if(born>sequence.born){AlteraAirshipStateParticleBirthsV20+=born-sequence.born;sequence.born=born;}
    long low=Math.Max(0,(long)Math.Floor(Math.Max(0,elapsed-p.lifetime[1])*sequence.rate)*sequence.group-sequence.group);int visible=0;
    if(!sequence.continuous&&elapsed>(top/sequence.group)/sequence.rate+p.lifetime[1]){airshipStateSequencesV20.RemoveAt(q);continue;}
    for(long index=top;index>=low&&visible<p.capacity;index--){float birth=(index/sequence.group)/sequence.rate,age=elapsed-birth;int seed=unchecked(sequence.seed+(int)index*179);if(age<0||age>=OutskirtsRange(p.lifetime,0,seed,2))continue;var mesh=sequence.view.mesh;float alpha=mesh==null?Mathf.Clamp01(OutskirtsCurve(p,"Color",3,age,seed,1)):1;if(alpha<=0)continue;
     var offset=new Vector3(OutskirtsRange(p.radius,0,seed,3)+OutskirtsVelocityIntegral(p,0,age,seed)+.5f*(p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,0,seed,20):p.gravity[0])*age*age,OutskirtsRange(p.radius,1,seed,3)+OutskirtsVelocityIntegral(p,1,age,seed)+.5f*(p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,1,seed,20):p.gravity[1])*age*age,OutskirtsRange(p.radius,2,seed,3)+OutskirtsVelocityIntegral(p,2,age,seed)+.5f*(p.gravityRanges!=null&&p.gravityRanges.Length==6?OutskirtsRange(p.gravityRanges,2,seed,20):p.gravity[2])*age*age)*sequence.unit;offset.x*=sequence.facing;
     var point=new Vector3(sequence.x+offset.x,sequence.y+offset.y,0);if(p.useLand)point.y=Mathf.Max(point.y,sequence.land);var s=AirshipStateSpriteV20(used++);s.gameObject.name="Airship A2 "+p.id;s.sharedMaterial=sequence.view.material;s.sortingOrder=44+(p.layer-14);s.flipX=sequence.facing<0;
     if(mesh==null){s.sprite=sequence.view.sprite;float scale=OutskirtsCurve(p,"Scale",0,age,seed,1);s.color=new Color(OutskirtsCurve(p,"Color",0,age,seed,1),OutskirtsCurve(p,"Color",1,age,seed,1),OutskirtsCurve(p,"Color",2,age,seed,1),alpha);s.transform.localScale=new Vector3(OutskirtsCurve(p,"Size",0,age,seed,1)*scale*sequence.unit/s.sprite.rect.width,OutskirtsCurve(p,"Size",1,age,seed,1)*scale*sequence.unit/s.sprite.rect.height,1);s.transform.localRotation=Quaternion.Euler(0,0,sequence.facing*OutskirtsCurve(p,"Rotate",2,age,seed,0));}
     else{if(age>=mesh.durationSeconds){used--;s.gameObject.SetActive(false);continue;}int i=Mathf.Clamp(Mathf.FloorToInt(age*mesh.fps),0,sequence.view.frames.Length-1);var f=mesh.frames[i];s.sprite=sequence.view.frames[i];float scale=sequence.unit/mesh.pixelsPerNativeUnit;s.color=Color.white;s.transform.localRotation=Quaternion.identity;s.transform.localScale=Vector3.one*scale;point+=new Vector3((f.rect[2]*.5f-f.anchor[0])*sequence.facing,f.anchor[1]-f.rect[3]*.5f,0)*scale;}
     s.transform.localPosition=point;npcEffectDepths[s.transform]=sequence.depth+offset.z/.28f;visible++;AlteraAirshipStateDrawFramesV20++;airshipStateKindsV20.Add(p.id);
    }
   }
   for(int i=used;i<airshipStateSpritesV20.Count;i++)if(airshipStateSpritesV20[i])airshipStateSpritesV20[i].gameObject.SetActive(false);
  }
 }
 }
}
