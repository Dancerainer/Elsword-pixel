using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 [Serializable]sealed class CoreAlarmIndexV21 {public string schema;public int sourceStates;public PreviewFile stateClips,sharedAtlas,staticArtApproval;public CoreAlarmSpriteBindingV21[] sprites;}
 [Serializable]sealed class CoreAlarmSpriteBindingV21 {public string id,sourceNPC;public PreviewFile animations,atlas;}
 [Serializable]sealed class CoreAlarmMapV21 {public string schema;public int sourceStates;public CoreAlarmStateV21[] rows;}
 [Serializable]sealed class CoreAlarmStateV21 {public string sourceNPC,kind,state,modelClip,displayClip,sourceAnimation;public float sourceStateDuration,sourceMotionDuration,sourceAnimSpeed;public bool sourceLoop,useExistingNpcStateAndNpcTime,offsetAlreadyInFrameAnchor,hostMustNotApplyOffsetAgain,sourceClockChanged;public float[] frameAnchor;}
 readonly Dictionary<string,CoreAlarmStateV21> coreAlarmStateRowsV21=new Dictionary<string,CoreAlarmStateV21>(StringComparer.Ordinal);
 readonly Dictionary<string,Sprite> coreAlarmWholeSpritesV21=new Dictionary<string,Sprite>(StringComparer.Ordinal);
 public int CoreAlarmWholeDrawsV21 {get;private set;}public int CoreAlarmMissingStateHoldsV21 {get;private set;}
 public string CoreAlarmWholeLastSourceNPCV21 {get;private set;}public string CoreAlarmWholeLastStateV21 {get;private set;}public string CoreAlarmWholeLastDisplayClipV21 {get;private set;}public float CoreAlarmWholeLastNpcClockV21 {get;private set;}public float[] CoreAlarmWholeLastFrameAnchorV21 {get;private set;}
 public bool CoreAlarmOriginalShowHideGlowOriginConsumedV21 {get{return false;}}
 static bool IsCoreAlarmWholeIdV21(string id){return id=="NUI_ALARM_SYSTEM_A"||id=="NUI_ALARM_SYSTEM_B"||id=="NUI_ALARM_SYSTEM_C"||id=="NUI_ALARM_SYSTEM_C_MOVE";}
 void ConfigureCoreAlarmWholeV21(PreviewFile indexPin,NpcDefinition[] rules){
  PreviewRequire(indexPin.sha256=="4ab8434ef526080a2848af619778bf115724cb37f1f32c3fc5089d7fa7dedfac","Core Alarm exact index required");var index=CoreEntryJsonV21<CoreAlarmIndexV21>(indexPin);
  PreviewRequire(index.schema=="core-Alarm-four-ID-standard-whole-library-index-v21-r1"&&index.sourceStates==31&&index.sprites.Length==4&&index.stateClips.sha256=="88ae3ab3efff6a7b28022956ac6c1b1de7b9a2a11c25e128d9946ffd8dd3d6f5"&&index.staticArtApproval.sha256=="98f9f8ace0ec9afcb5e5e3c109374a3fa7a6fff3d05163e007d51b23c61d710d","Core Alarm static/body selector identity");if(!CoreProductResourcesActiveR1147)CoreEntryReadV21(index.staticArtApproval);
  var map=CoreEntryJsonV21<CoreAlarmMapV21>(index.stateClips);PreviewRequire(map.schema=="core-Alarm-source-state-display-map-v21-r1"&&map.rows.Length==31,"Core Alarm31 exact state rows");
  var texture=PreviewTextureFile(index.sharedAtlas);texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
  foreach(var binding in index.sprites){PreviewRequire(IsCoreAlarmWholeIdV21(binding.sourceNPC)&&binding.atlas.sha256==index.sharedAtlas.sha256,"Core Alarm exact source identity/atlas");var lib=CoreEntryJsonV21<PreviewLibrary>(binding.animations);PreviewRequire(lib.id==binding.id&&lib.sourceNPC==binding.sourceNPC&&lib.logicalScale==1,"Core Alarm whole library scale/identity");var profile=Array.Find(rules,p=>p.id==binding.sourceNPC);PreviewRequire(profile!=null&&profile.kind==binding.id,"Core Alarm exact profile");
   foreach(var row in map.rows){if(row.sourceNPC!=binding.sourceNPC)continue;var state=Array.Find(profile.states,s=>s.id==row.state);var clip=Array.Find(lib.clips,c=>c.id==row.displayClip);
    PreviewRequire(state!=null&&row.kind==profile.kind&&row.modelClip==state.clip&&row.sourceAnimation==state.sourceAnimation&&row.sourceAnimSpeed==state.sourceAnimSpeed&&row.sourceStateDuration==state.duration&&row.sourceLoop==state.loop&&row.useExistingNpcStateAndNpcTime&&row.offsetAlreadyInFrameAnchor&&row.hostMustNotApplyOffsetAgain&&!row.sourceClockChanged&&clip!=null&&clip.loop==row.sourceLoop&&clip.durationSeconds==row.sourceMotionDuration/row.sourceAnimSpeed&&clip.frames.Length==1,"Core Alarm original state/clock/loop mapping");
    var frame=clip.frames[0];PreviewRequire(frame.rect.Length==4&&frame.anchor.Length==2&&row.frameAnchor.Length==2&&frame.anchor[0]==row.frameAnchor[0]&&frame.anchor[1]==row.frameAnchor[1],"Core Alarm B offset already in anchor");string key=binding.id+"/"+row.displayClip;
    if(!coreAlarmWholeSpritesV21.ContainsKey(key)){var r=frame.rect;var sprite=Sprite.Create(texture,new Rect(r[0],texture.height-r[1]-r[3],r[2],r[3]),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core approved Alarm whole / "+key;forestPreviewOwned.Add(sprite);spriteOffsets.Add(sprite,new Vector2(r[2]*.5f-frame.anchor[0],frame.anchor[1]-r[3]*.5f));coreAlarmWholeSpritesV21.Add(key,sprite);}
    coreAlarmStateRowsV21.Add(row.sourceNPC+"/"+row.state,row);
   }
  }PreviewRequire(coreAlarmStateRowsV21.Count==31,"Core Alarm state selector incomplete");CoreAlarmWholeDrawsV21=CoreAlarmMissingStateHoldsV21=0;
 }
 bool RenderCoreAlarmWholeV21(Enemy e,SpriteRenderer s){
  if(!IsCoreAlarmWholeIdV21(e.sourceNPC))return false;
  CoreAlarmStateV21 row;if(string.IsNullOrEmpty(e.npcState)){s.enabled=false;s.sprite=null;CoreAlarmMissingStateHoldsV21++;return true;}
  PreviewRequire(coreAlarmStateRowsV21.TryGetValue(e.sourceNPC+"/"+e.npcState,out row),"Core Alarm unknown exact state; no idle fallback");row=coreAlarmStateRowsV21[e.sourceNPC+"/"+e.npcState];
  PreviewRequire(e.kind==row.kind&&e.visualScale==1&&!float.IsNaN(e.npcTime)&&!float.IsInfinity(e.npcTime)&&e.npcTime>=0,"Core Alarm original actor scale/clock");
  float sourceClock=e.npcTime*row.sourceAnimSpeed;sourceClock=row.sourceLoop?Mathf.Repeat(sourceClock,row.sourceMotionDuration):Mathf.Min(sourceClock,row.sourceMotionDuration);
  RecordMonsterPixelSourceV53(e,s,"Core/Alarm",row.displayClip,row.sourceAnimation,sourceClock,1);
  // Each source clip intentionally holds one approved whole; no clock mutation,
  // extra whole-body B offset, automatic mirror, or inferred Show/Glow owner.
  s.enabled=true;s.sprite=coreAlarmWholeSpritesV21[row.kind+"/"+row.displayClip];s.flipX=s.flipY=false;s.transform.localScale=Vector3.one;s.transform.localRotation=Quaternion.identity;var p=RenderEnemyPosition(e)+SpriteOffset(s);s.transform.localPosition=new Vector3(p.x,p.y,0);CoreAlarmWholeLastSourceNPCV21=e.sourceNPC;CoreAlarmWholeLastStateV21=e.npcState;CoreAlarmWholeLastDisplayClipV21=row.displayClip;CoreAlarmWholeLastNpcClockV21=e.npcTime;CoreAlarmWholeLastFrameAnchorV21=row.frameAnchor;CoreAlarmWholeDrawsV21++;return true;
 }
 void DisposeCoreAlarmWholeV21(){DisposeCoreAlarmBeamVisualR1417();foreach(var s in coreAlarmWholeSpritesV21.Values)spriteOffsets.Remove(s);coreAlarmWholeSpritesV21.Clear();coreAlarmStateRowsV21.Clear();}
}}


namespace ElswordPixel { public sealed partial class DemoScene {
 Texture2D coreAlarmBeamTextureR1417;
 Sprite coreAlarmBeamSpriteR1417;
 readonly List<SpriteRenderer> coreAlarmBeamViewsR1417=new List<SpriteRenderer>();
 readonly Dictionary<Enemy,Vector2Int> alarmLightRangesV59=new Dictionary<Enemy,Vector2Int>();
 public int CoreAlarmBeamVisibleSpritesR1417 {get;private set;}
 public int CoreAlarmBeamRenderedFramesR1417 {get;private set;}
 void CoreAlarmBeamHideR1417(int used){
  for(int i=used;i<coreAlarmBeamViewsR1417.Count;i++)if(coreAlarmBeamViewsR1417[i])coreAlarmBeamViewsR1417[i].gameObject.SetActive(false);
  CoreAlarmBeamVisibleSpritesR1417=used;
 }
 void DisposeCoreAlarmBeamVisualR1417(){
  foreach(var r in coreAlarmBeamViewsR1417)if(r)RemoveSceneObject(r.gameObject);
  coreAlarmBeamViewsR1417.Clear();CoreAlarmBeamVisibleSpritesR1417=0;
  if(coreAlarmBeamSpriteR1417)Destroy(coreAlarmBeamSpriteR1417);coreAlarmBeamSpriteR1417=null;
  if(coreAlarmBeamTextureR1417)Destroy(coreAlarmBeamTextureR1417);coreAlarmBeamTextureR1417=null;
 }
 void CoreAlarmBeamRequirePixelR1417(){
  if(coreAlarmBeamSpriteR1417)return;
  coreAlarmBeamTextureR1417=new Texture2D(1,1,TextureFormat.RGBA32,false){name="Alarm pixels",filterMode=FilterMode.Point};
  coreAlarmBeamTextureR1417.SetPixel(0,0,Color.white);coreAlarmBeamTextureR1417.Apply(false,true);
  coreAlarmBeamSpriteR1417=Sprite.Create(coreAlarmBeamTextureR1417,new Rect(0,0,1,1),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
 }
 void AlarmStrokeV59(ref int used,Vector2 from,Vector2 to,float width,Color color,int order){
  var delta=to-from;
  if(delta.sqrMagnitude<.01f)return;
  CoreAlarmBeamRequirePixelR1417();
  if(used==coreAlarmBeamViewsR1417.Count)
   coreAlarmBeamViewsR1417.Add(WorldSprite("Core alarm light",transform,coreAlarmBeamSpriteR1417,order,Vector3.zero));
  var r=coreAlarmBeamViewsR1417[used++];r.gameObject.SetActive(true);r.sortingOrder=order;r.color=color;
  r.transform.position=(from+to)*.5f;r.transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);r.transform.localScale=new Vector3(delta.magnitude,width,1);
 }
 static Color AlarmColorV59(Enemy actor){return actor.sourceKeyCode==2?new Color(.3f,1,.3f):new Color(.2f,.3f,1);}
 Vector3 AlarmSocketWorldV59(Enemy actor){
  return actorsRoot.TransformPoint(new Vector3(actor.x,actor.y+GameModel.CoreAlarmSocketHeightV59(actor)));
 }
 // The orthographic side camera collapses a Z-directed laser to a point.
 // Present its depth in a cabinet projection, keeping the real lane fixed.
 static readonly Vector2 AlarmBackV59=new Vector2(33,23);
 Vector2 AlarmLaneV59(Enemy e){return ProjectNativePoint(AlarmSocketWorldV59(e),0);}
 void AlignAlarmSocketV59(Enemy actor,SpriteRenderer body){
  // The baked sprite stored each frame's trim and removed the neutral model
  // floor. Align its visible lens to the source Dummy01 once, after projection.
  Vector3 world=AlarmSocketWorldV59(actor);
  float depth=actor.nativeDepth+CoreHardAlarmBeamViewR1417.SocketDepth;
  ProjectNativePointWithEye(world,depth,out float eye);
  Vector3 socket=AlarmLaneV59(actor)+AlarmBackV59;
  body.transform.position+=socket-body.bounds.center;
  BindProjectedSceneOcclusion(body,world,depth,eye);
 }
 void RenderCoreAlarmLaserVisualR1417(){
  alarmLightRangesV59.Clear();
  if(!coreProductSessionR1147||game==null||!NativeWorldActive){CoreAlarmBeamHideR1417(0);return;}
  int used=0;
  foreach(var beam in game.CoreHardAlarmBeamViewsR1417()){
   int first=used;
   Vector2 lane=AlarmLaneV59(beam.ownerEnemy);
   Vector2 socket=lane+AlarmBackV59;
   // The source mesh continues 4100 native units along depth, past the
   // gameplay camera. Show the part reaching the playable lane, plus a short
   // foreground tail. The reticle marks the actual collision cross-section.
   Vector2 tip=lane-AlarmBackV59*(140f/372f);
   bool live=beam.phase=="active";var color=AlarmColorV59(beam.ownerEnemy);
   var ink=new Color(.055f,.07f,.14f,.9f);
   if(live){
    AlarmStrokeV59(ref used,socket,tip,5,ink,45);
    AlarmStrokeV59(ref used,socket,tip,3,color,46);
    AlarmStrokeV59(ref used,socket,tip,1,Color.white,47);
   }else{
    float length=Vector2.Distance(socket,tip);var direction=(tip-socket).normalized;
    for(float at=0;at<length;at+=7)AlarmStrokeV59(ref used,socket+direction*at,socket+direction*Mathf.Min(length,at+3),1.5f,color,46);
   }
   float radius=live?beam.halfWidth+1:beam.halfWidth+2+Mathf.PingPong(game.elapsed*5,3);
   for(int i=0;i<4;i++){
    Vector2 u=i==0?Vector2.right:i==1?Vector2.up:i==2?Vector2.left:Vector2.down;
    Vector2 v=new Vector2(-u.y,u.x);
    Vector2 a=lane+u*radius,b=lane+v*radius;
    if(live)AlarmStrokeV59(ref used,a,b,2,ink,48);
    AlarmStrokeV59(ref used,a,live?b:Vector2.Lerp(a,b,.48f),1.1f,live?Color.Lerp(color,Color.white,.62f):color,49);
   }
   alarmLightRangesV59[beam.ownerEnemy]=new Vector2Int(first,used);
  }
  CoreAlarmBeamHideR1417(used);if(used>0)CoreAlarmBeamRenderedFramesR1417++;
 }
}}
