// Explicit, session-local Player candidate. No automatic map admission or publication.
using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class BelowPlayerHostInspectionV21 {
  public string schema="below-player-owned-host-candidate-v21-r2",sourceDungeon=BelowPlayerResourceIndexV21.SourceDungeon,manifestSha256=BelowPlayerBootstrapV21.ManifestSha256;
  public bool active,loading,typedEffectsConfigured,runtimeAdmitted=false,releaseReady=false,nativeEquivalent=false,actualPlayerValidated=false;
  public int routes,stages,contentSets,room,ownedObjects,ownedLibraries,ownedAtlasKeys,ownedAudioSlots,typedDeclaredHolds;
  public string scope="Resources-backed session; original 78 births / 24 profiles / 279 states / 11 effects / 67 sounds. Approved finite 2D display adapters, not native engine equivalence.";
 }
 public sealed partial class DemoScene {
  bool belowPlayerSessionV21,belowPlayerLoadingV21;BelowPlayerDataBridgeV21 belowPlayerDataV21;BelowPlayerDataPlanV21 belowPlayerPlanV21;BelowPlayerVisualPlanV21 belowPlayerFxV21;
  ArtCatalog belowPlayerOriginalCatalogV21;int belowPlayerOriginalDungeonV21,belowPlayerOriginalPreviewV21;bool belowPlayerOriginalPausedV21;string belowPlayerOriginalMenuV21;
  readonly List<UnityEngine.Object> belowPlayerOwnedV21=new List<UnityEngine.Object>();
  readonly List<string> belowPlayerLibrariesV21=new List<string>(),belowPlayerAtlasKeysV21=new List<string>(),belowPlayerAddedMusicV21=new List<string>();
  readonly Dictionary<string,AudioClip> belowPlayerSavedAudioV21=new Dictionary<string,AudioClip>(),belowPlayerMusicV21=new Dictionary<string,AudioClip>();
  readonly HashSet<string> belowPlayerAbsentAudioV21=new HashSet<string>();
  readonly List<Dictionary<string,Texture2D>> belowPlayerWorldTexturesV21=new List<Dictionary<string,Texture2D>>();
  bool BelowPresentationSessionV21 {get{
   if(belowPlayerSessionV21)return true;
#if UNITY_EDITOR
   return belowTunnelEntrySessionV21&&forestPreviewActive;
#else
   return false;
#endif
  }}
  static void BelowPlayerNeedV21(bool ok,string why){BelowPlayerResourceIndexV21.Need(ok,"Below Player candidate: "+why);}
  static bool BelowPlayerNoBodyV21(string id){return id=="NUI_STEAM_TRAP"||id=="NUI_STEAM_TRAP_DOWN"||id=="NUI_STEAM_TRAP_SIDE";}
  public BelowPlayerHostInspectionV21 InspectBelowPlayerCandidateV21(){return new BelowPlayerHostInspectionV21{active=belowPlayerSessionV21,loading=belowPlayerLoadingV21,typedEffectsConfigured=belowPlayerSessionV21&&BelowVisualConfiguredV21,routes=catalog==null?0:catalog.dungeons.Length,stages=catalog==null?0:catalog.stages.Length,contentSets=catalog==null?0:catalog.npcContentSets.Length,room=game.room,ownedObjects=belowPlayerOwnedV21.Count,ownedLibraries=belowPlayerLibrariesV21.Count,ownedAtlasKeys=belowPlayerAtlasKeysV21.Count,ownedAudioSlots=belowPlayerSavedAudioV21.Count+belowPlayerAbsentAudioV21.Count,typedDeclaredHolds=belowPlayerFxV21==null?0:belowPlayerFxV21.inspection.heldFields};}
  // Begin is a deliberate candidate API from a fresh menu, never an automatic
  // catalog whitelist override. It cannot replace an in-progress dungeon.
  public void BeginBelowPlayerCandidateV21(){BeginBelowPlayerSessionR1130(null);}
  void ConfigureBelowPlayerRoomsV21(){BelowPlayerNeedV21(belowPlayerPlanV21!=null,"Session data absent");var p=belowPlayerPlanV21;game.SetNpcDefinitions(p.rules);game.SetNpcEffectDefinitions(p.effects);game.SetNpcSoundDefinitions(p.sounds);game.SetNpcMechanismDefinitions(p.mechanisms);game.SetRooms(p.rooms);game.ConfigureAlteraDeferredCannonGates(null);}
  void BuildBelowPlayerWorldV21(){
   ClearBelowThinGroundMeshR1483();
   var payload=belowPlayerPlanV21.worlds[game.room];var scene=belowPlayerDataV21.Parse<NativeScene>(payload.scene);var p=payload.projection;scene.unit=p.unit;scene.offsetX=p.offsetX;scene.offsetY=p.offsetY;scene.path=new NativePath[p.path.Length];for(int i=0;i<p.path.Length;i++){var q=p.path[i];scene.path[i]=new NativePath{s=q.s,length=q.length,start=q.start,end=q.end};}var textures=belowPlayerWorldTexturesV21[game.room];BuildNativeWorld(scene,payload.geometry,key=>textures[key],belowPlayerPlanV21.stages[game.room].id);
  }
  static void ValidateBelowPlayerLibraryV21(BelowPlayerLibraryV21 raw,BelowPlayerSpriteV21 row){
   BelowPlayerNeedV21(raw!=null&&raw.id==row.id&&raw.sourceNPC==row.sourceNPC&&!raw.runtimeAdmitted&&!raw.animationLibraryApproved&&(string.IsNullOrEmpty(raw.sourceRulesSha256)||raw.sourceRulesSha256=="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb")&&raw.logicalScale>0&&raw.logicalScale<=8&&raw.atlas!=null&&raw.atlas.groundAnchor!=null&&raw.atlas.groundAnchor.Length==2&&raw.clips!=null&&raw.clips.Length>0,"Sprite identity/atlas/source scope");
  }
  void LoadBelowPlayerLibraryV21(BelowPlayerSpriteV21 row,BelowPlayerLibraryV21 raw){
   BelowPlayerNeedV21(!libraries.ContainsKey(row.id),"Candidate library conflicts with baseline "+row.id);string key="below-player-owned-v21/"+row.id;BelowPlayerNeedV21(!atlasTextures.ContainsKey(key),"Candidate atlas slot conflict");var texture=belowPlayerDataV21.CreatePointTexture(row.atlas);belowPlayerOwnedV21.Add(texture);BelowPlayerNeedV21(texture.width<=4096&&texture.height<=4096,"Atlas exceeds approved max");atlasTextures.Add(key,texture);belowPlayerAtlasKeysV21.Add(key);var clips=new List<AnimationData>();var ids=new HashSet<string>();
   foreach(var source in raw.clips){BelowPlayerNeedV21(!string.IsNullOrEmpty(source.id)&&ids.Add(source.id)&&source.fps>0&&source.durationSeconds>0&&source.frames!=null&&source.frames.Length>0,"Invalid source clip");var frames=new List<FrameData>();foreach(var f in source.frames){BelowPlayerNeedV21(f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2&&f.rect[0]>=0&&f.rect[1]>=0&&f.rect[2]>0&&f.rect[3]>0&&f.rect[0]+f.rect[2]<=texture.width&&f.rect[1]+f.rect[3]<=texture.height,"Source frame outside atlas");frames.Add(new FrameData{x=f.rect[0],y=f.rect[1],w=f.rect[2],h=f.rect[3],hasAnchor=true,anchorX=f.anchor[0],anchorY=f.anchor[1]});}clips.Add(new AnimationData{id=source.id,fps=source.fps,duration=source.durationSeconds,loop=source.loop,frames=frames.ToArray()});}
   belowPlayerLibrariesV21.Add(raw.id);var data=new SpriteLibraryData{id=raw.id,texture=key,artVersion=raw.artVersion,paintedPoseCount=raw.paintedPoseCount,standingHeight=raw.standingHeight,logicalScale=raw.logicalScale,frameWidth=raw.atlas.frameWidth,frameHeight=raw.atlas.frameHeight,anchorX=raw.atlas.groundAnchor[0],anchorY=raw.atlas.groundAnchor[1],clips=clips.ToArray()};var library=new Library{data=data};if(DeferPrivateFramesV53(library,frame=>texture,belowPlayerOwnedV21,true))libraries.Add(data.id,library);else MakeLibrary(data);
  }
  void TrackBelowPlayerSpriteV21(Sprite sprite){if(belowPlayerLoadingV21)belowPlayerOwnedV21.Add(sprite);}
  void LoadBelowPlayerVisualV21(BelowPlayerDataBridgeV21 data,BelowPlayerVisualPlanV21 plan){
   var textures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);var owned=new List<Texture2D>();try{foreach(var r in plan.binding.textures){var texture=data.CreatePointTexture(BelowPlayerVisualPlanV21.File(r));owned.Add(texture);textures.Add(r.key,texture);}ConfigureBelowVisualV21(plan.document,plan.meshes,textures);belowVisualLoadedTexturesV21.AddRange(owned);owned.Clear();}finally{foreach(var t in owned)BelowVisualDestroyV21(t);}
  }
  string BelowPlayerRenderClipV21(Enemy enemy,ref float clock){
   if(BelowPlayerNoBodyV21(enemy.sourceNPC))return enemy.clip;Library art;BelowPlayerNeedV21(libraries.TryGetValue(enemy.kind,out art),"Exact identity library absent "+enemy.sourceNPC);var p=belowPlayerPlanV21;string selected=BelowPixelStateClipV21.OverrideClip(enemy.sourceNPC,enemy.npcState);bool loaded=selected==null||art.frames.ContainsKey(selected)&&art.clips.ContainsKey(selected)&&!art.clips[selected].loop;bool hold;string clip;if(!p.recoveryLookup.TryResolveLocal(true,game.CurrentRoom.id,enemy,p.sourceClips,p.physicalClips,loaded,key=>art.frames.ContainsKey(key)&&art.clips.ContainsKey(key),game.Grounded(enemy),game.NpcPendingStateRequest(enemy),out clip,out hold))clip=p.physicalClips.Resolve(enemy,p.sourceClips,loaded,key=>art.frames.ContainsKey(key)&&art.clips.ContainsKey(key),out hold);BelowPlayerNeedV21(art.frames.ContainsKey(clip)&&art.clips.ContainsKey(clip),"Exact source/reaction clip absent; idle fallback forbidden "+enemy.sourceNPC+"/"+enemy.npcState+"/"+clip);if(hold){var info=art.clips[clip];clock=BelowPhysicalReactionClipV21.OnceThenHoldClock(clock,info.duration,info.fps,art.frames[clip].Length);}return clip;
  }
  sealed class BelowThinGroundMeshCacheR1483 {
   internal Enemy owner;internal NativeScene scene;internal Transform root,part;internal RoomDefinition room;internal Matrix4x4 partMatrix;
   internal int groundLine,appliedFrame=-1;internal float sampledX,sourceSupportY,meshNativeY,deltaLogical,appliedLogicalDelta;
   internal bool matched;internal Vector3 nativeFoot,a,b,c;
  }
  readonly Dictionary<Enemy,BelowThinGroundMeshCacheR1483> belowThinGroundMeshR1483=new Dictionary<Enemy,BelowThinGroundMeshCacheR1483>();
  readonly List<Enemy> belowThinGroundMeshRetiredR1483=new List<Enemy>();
  NativeScene belowThinGroundMeshSceneR1483;Transform belowThinGroundMeshRootR1483;RoomDefinition belowThinGroundMeshRoomR1483;int belowThinGroundMeshTrimFrameR1483=-1;
  void ClearBelowThinGroundMeshR1483(){belowThinGroundMeshR1483.Clear();belowThinGroundMeshRetiredR1483.Clear();belowThinGroundMeshSceneR1483=null;belowThinGroundMeshRootR1483=null;belowThinGroundMeshRoomR1483=null;belowThinGroundMeshTrimFrameR1483=-1;}
  Vector2 RetireBelowThinGroundMeshR1483(Enemy enemy){if(enemy!=null)belowThinGroundMeshR1483.Remove(enemy);return Vector2.zero;}
  void RefreshBelowThinGroundMeshScopeR1483(){
   if(!belowPlayerSessionV21||game==null||!NativeWorldActive||!nativeRoot){ClearBelowThinGroundMeshR1483();return;}
   if(!ReferenceEquals(belowThinGroundMeshSceneR1483,nativeScene)||!ReferenceEquals(belowThinGroundMeshRootR1483,nativeRoot)||!ReferenceEquals(belowThinGroundMeshRoomR1483,game.CurrentRoom)){
    ClearBelowThinGroundMeshR1483();belowThinGroundMeshSceneR1483=nativeScene;belowThinGroundMeshRootR1483=nativeRoot;belowThinGroundMeshRoomR1483=game.CurrentRoom;
   }
   if(belowThinGroundMeshTrimFrameR1483==Time.frameCount)return;belowThinGroundMeshTrimFrameR1483=Time.frameCount;belowThinGroundMeshRetiredR1483.Clear();
   foreach(var pair in belowThinGroundMeshR1483){var e=pair.Key;if(!game.enemies.Contains(e)||e.hp<=0||!e.active||e.dormant||e.state!=CombatState.Normal||!game.Grounded(e)||(e.npcState!="NASOD_MINER_THIN_DEFECT_DIE_DIEWAIT"&&(e.npcState!="NASOD_MINER_THIN_DEFECT_DIE_REBIRTH"||e.npcTime>=23f/60f)))belowThinGroundMeshRetiredR1483.Add(e);}
   foreach(var e in belowThinGroundMeshRetiredR1483)belowThinGroundMeshR1483.Remove(e);belowThinGroundMeshRetiredR1483.Clear();
  }
  static bool TryBelowThinGroundTriangleR1483(Vector3 a,Vector3 b,Vector3 c,Vector3 foot,float unit,out float delta){
   delta=0;if(!(unit>0)||float.IsNaN(unit)||float.IsInfinity(unit))return false;
   var n=Vector3.Cross(b-a,c-a);float length=n.magnitude;if(!(length>1e-5f)||Mathf.Abs(n.y)/length<.98f)return false;
   double den=((double)b.z-c.z)*(a.x-c.x)+((double)c.x-b.x)*(a.z-c.z);if(Math.Abs(den)<1e-8)return false;
   double u=(((double)b.z-c.z)*(foot.x-c.x)+((double)c.x-b.x)*(foot.z-c.z))/den;
   double v=(((double)c.z-a.z)*(foot.x-c.x)+((double)a.x-c.x)*(foot.z-c.z))/den,w=1-u-v;
   if(u< -1e-6||v< -1e-6||w< -1e-6)return false;
   double value=(u*a.y+v*b.y+w*c.y-foot.y)*unit;
   // This correction may only lower a lying image onto a nearby native face.
   // A face above the support, a wall, or a larger mismatch is uncovered.
   if(double.IsNaN(value)||double.IsInfinity(value)||value>0||value< -4)return false;delta=(float)value;return true;
  }
  float BelowThinGroundMeshOffsetR1483(Enemy enemy,float taper){
   if(!NativeWorldActive||!nativeRoot||game.CurrentRoom.id!="Dungeon_Altera_Below_Tunnel_Normal-"+game.room||
      nativeScene.id!=(game.room==0?"WI_ALTERA_BELOW_TUNNEL_STAGE1":game.room==1?"WI_ALTERA_BELOW_TUNNEL_STAGE2A":""))return 0;
   var renderFoot=RenderEnemyPosition(enemy);bool supportFound=false;float support=0;
   foreach(var line in game.CurrentRoom.surfaces){if(line.id!=enemy.groundLine||renderFoot.x<Mathf.Min(line.x1,line.x2)-.001f||renderFoot.x>Mathf.Max(line.x1,line.x2)+.001f||Mathf.Abs(line.x2-line.x1)<.001f)continue;support=line.y1+(line.y2-line.y1)*Mathf.Clamp01((renderFoot.x-line.x1)/(line.x2-line.x1));supportFound=true;break;}
   if(!supportFound||Mathf.Abs(renderFoot.y-support)>.05f){belowThinGroundMeshR1483.Remove(enemy);return 0;}
   Vector3 foot=NativePathPoint(renderFoot.x)+NativeNormal(renderFoot.x)*enemy.nativeDepth;foot.y=(support-nativeScene.offsetY)/nativeScene.unit;
   BelowThinGroundMeshCacheR1483 entry;
   if(belowThinGroundMeshR1483.TryGetValue(enemy,out entry)&&entry.groundLine!=enemy.groundLine)belowThinGroundMeshR1483.Remove(enemy);
   if(!belowThinGroundMeshR1483.TryGetValue(enemy,out entry)){
    entry=new BelowThinGroundMeshCacheR1483{owner=enemy,scene=nativeScene,root=nativeRoot,room=game.CurrentRoom,groundLine=enemy.groundLine,sampledX=renderFoot.x,sourceSupportY=support,nativeFoot=foot};belowThinGroundMeshR1483.Add(enemy,entry);
    // One finite search per eligible lying/rebirth span. Reuse native vertices
    // already owned by this world; no physics raycast or per-frame mesh scan.
    foreach(var part in cameraGeometryPartsV25){if(!part.transform||!part.transform.name.EndsWith("_ground1.Y",StringComparison.OrdinalIgnoreCase))continue;var matrix=part.transform.localToWorldMatrix;var vertices=part.vertices;
     for(int i=0;i+2<vertices.Length;i+=3){var a=matrix.MultiplyPoint3x4(vertices[i]);var b=matrix.MultiplyPoint3x4(vertices[i+1]);var c=matrix.MultiplyPoint3x4(vertices[i+2]);float delta;if(!TryBelowThinGroundTriangleR1483(a,b,c,foot,nativeScene.unit,out delta)||entry.matched&&delta<=entry.deltaLogical)continue;entry.matched=true;entry.part=part.transform;entry.partMatrix=matrix;entry.a=a;entry.b=b;entry.c=c;entry.deltaLogical=delta;entry.meshNativeY=foot.y+delta/nativeScene.unit;}
    }
   }
   // A moving owner or changed native transform leaves this sampled footprint.
   // Retire the match for this span instead of rescanning every rendered frame.
   entry.nativeFoot=foot;entry.appliedFrame=Time.frameCount;entry.appliedLogicalDelta=0;
   if(entry.groundLine!=enemy.groundLine||Mathf.Abs(renderFoot.x-entry.sampledX)>.25f||Mathf.Abs(support-entry.sourceSupportY)>.01f||!entry.part||entry.part.localToWorldMatrix!=entry.partMatrix)entry.matched=false;
   float current;if(!entry.matched||!TryBelowThinGroundTriangleR1483(entry.a,entry.b,entry.c,foot,nativeScene.unit,out current)){entry.matched=false;entry.deltaLogical=0;return 0;}
   entry.deltaLogical=current;entry.meshNativeY=foot.y+current/nativeScene.unit;entry.appliedLogicalDelta=current*taper;return entry.appliedLogicalDelta;
  }
  Vector2 BelowThinGroundedRegistrationR1481(Enemy enemy,SpriteRenderer renderer,float scale){
   RefreshBelowThinGroundMeshScopeR1483();
   if(!belowPlayerSessionV21||belowPlayerPlanV21==null||enemy==null||renderer==null||!renderer.sprite||enemy.sourceNPC!="NUI_NASOD_MINER_THIN_DEFECT_DIE"||enemy.kind!="nasod-miner-thin-defect-die"||enemy.hp<=0||enemy.state!=CombatState.Normal||!game.Grounded(enemy))return RetireBelowThinGroundMeshR1483(enemy);
   string clip;int expectedFrames;
   if(enemy.npcState=="NASOD_MINER_THIN_DEFECT_DIE_DIEWAIT"){clip="down-back";expectedFrames=120;}
   else if(enemy.npcState=="NASOD_MINER_THIN_DEFECT_DIE_REBIRTH"){clip="get-up-back";expectedFrames=92;}
   else return RetireBelowThinGroundMeshR1483(enemy);
   // This is the exact existing 2D library's registration, not a physical
   // ground snap or a rule for other NPCs. Future artwork needs a new receipt.
   var row=Array.Find(belowPlayerPlanV21.sprites,r=>r.id==enemy.kind);
   if(row==null||row.sourceNPC!=enemy.sourceNPC||row.animations==null||row.atlas==null||row.animations.sha256!="b8118c6ef58dba7dd2c7ab78f7fab54c10cfa0dd8e503854cd921d732a090cc3"||row.atlas.sha256!="07b4bd067d9aee6b407122818cdd071feb51eedc1819bed7ccfd00c5d7b5b80f")return RetireBelowThinGroundMeshR1483(enemy);
   Library art;Sprite[] frames;AnimationData info;
   if(!libraries.TryGetValue(enemy.kind,out art)||!art.frames.TryGetValue(clip,out frames)||frames.Length!=expectedFrames||!art.clips.TryGetValue(clip,out info)||info.frames==null||info.frames.Length!=expectedFrames||info.fps!=60||info.loop)return RetireBelowThinGroundMeshR1483(enemy);
   int frame=Array.IndexOf(frames,renderer.sprite);if(frame<0)return RetireBelowThinGroundMeshR1483(enemy);var f=info.frames[frame];
   if(!f.hasAnchor||f.anchorX!=96||f.anchorY!=144||f.w!=192||f.h!=192||!(scale>0)||float.IsNaN(scale)||float.IsInfinity(scale))return RetireBelowThinGroundMeshR1483(enemy);
   // Actual alpha >=16..192 agrees on these bottom edges. Ignore the almost
   // transparent source tail pixels. DIEWAIT's final painted pose is 80..119.
   float gap,meshTaper=1;if(clip=="down-back"){if(frame<80)return RetireBelowThinGroundMeshR1483(enemy);gap=13;}
   else{
    // Only bridge the lying entry. The original pose from frame23 onward
    // contains bent feet and a lower drill tip: preserve that motion exactly,
    // rather than treating its lowest alpha pixel as a supporting foot.
    if(frame>=23||enemy.npcTime<0||float.IsNaN(enemy.npcTime)||float.IsInfinity(enemy.npcTime))return RetireBelowThinGroundMeshR1483(enemy);
    float t=Mathf.Clamp01(enemy.npcTime/(23f/60f));meshTaper=1-t*t*(3-2*t);gap=14*meshTaper;
   }
   // The offset only moves this registered lying image downward; it releases
   // smoothly on the unchanged original source clock and never raises a pose.
   return new Vector2(0,-gap*scale+BelowThinGroundMeshOffsetR1483(enemy,meshTaper));
  }
  void StopBelowPlayerMusicV21(){foreach(var voice in musicVoices)if(voice){voice.Stop();voice.clip=null;}musicId="";musicActive=-1;for(int i=0;i<2;i++)musicFrom[i]=musicLevels[i]=0;}
  public void EndBelowPlayerCandidateV21(){DisposeBelowPlayerCandidateV21(true);}
  void DisposeBelowPlayerCandidateV21(bool restoreMenu){
   ClearBelowThinGroundMeshR1483();
   if(belowPlayerOriginalCatalogV21==null)return;game.paused=true;ClearInput();StopAudioVoices();StopBelowPlayerMusicV21();DisposeNativeWorld();DisposeBelowVisualV21();if(belowPlayerPlanV21!=null)belowPlayerPlanV21.recoveryLookup.Clear();
   foreach(var id in belowPlayerLibrariesV21)libraries.Remove(id);belowPlayerLibrariesV21.Clear();foreach(var key in belowPlayerAtlasKeysV21)atlasTextures.Remove(key);belowPlayerAtlasKeysV21.Clear();foreach(var p in belowPlayerSavedAudioV21)audioClips[p.Key]=p.Value;foreach(var id in belowPlayerAbsentAudioV21)audioClips.Remove(id);belowPlayerSavedAudioV21.Clear();belowPlayerAbsentAudioV21.Clear();foreach(var id in belowPlayerAddedMusicV21)sceneMusicAssets.Remove(id);belowPlayerAddedMusicV21.Clear();belowPlayerMusicV21.Clear();
   for(int i=belowPlayerOwnedV21.Count-1;i>=0;i--){var o=belowPlayerOwnedV21[i];var sprite=o as Sprite;if(!ReferenceEquals(sprite,null))spriteOffsets.Remove(sprite);BelowVisualDestroyV21(o);}belowPlayerOwnedV21.Clear();belowPlayerWorldTexturesV21.Clear();catalog=belowPlayerOriginalCatalogV21;belowPlayerOriginalCatalogV21=null;selectedDungeon=belowPlayerOriginalDungeonV21;previewDungeon=belowPlayerOriginalPreviewV21;belowPlayerSessionV21=belowPlayerLoadingV21=false;belowProductRouteSessionR1130=false;belowPlayerPlanV21=null;belowPlayerDataV21=null;belowPlayerFxV21=null;adventureStarted=false;openMenu=belowPlayerOriginalMenuV21;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;
   if(restoreMenu){ConfigureRooms();ApplyTimings();RebuildRoom();ResetAudioTracking();RenderMenus();game.paused=belowPlayerOriginalPausedV21;}
  }
 
  void BeginBelowPlayerSessionR1130(BelowProductResourceMapR1130 productMap){
   RequireDevelopmentContent();
   bool product=productMap!=null;
   BelowPlayerNeedV21(Application.isPlaying&&!plainProductSessionR1320&&!mine2ProductSessionR1233&&!battleProductSessionR1325&&!cargoProductSessionR1288&&!factoryProductSessionR1179&&!coreProductSessionR1147&&!belowPlayerSessionV21&&!belowPlayerLoadingV21&&!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&(product?openMenu=="dungeons"&&game.paused:!adventureStarted&&game.elapsed==0),"Requires a normal dungeon menu; active review or duplicate session rejected");
#if UNITY_EDITOR
   BelowPlayerNeedV21(!forestPreviewActive&&!forestPreviewLoading,"Another external preview owns the scene");
#endif
   var data=product?BelowPlayerDataBridgeV21.OpenCurrentProductR1130(productMap):BelowPlayerDataBridgeV21.OpenFrozen();var plan=product?data.BuildCurrentProductPlanR1130(catalog,productMap):data.BuildReviewPlan();var fx=BelowPlayerVisualPlanV21.Read(data);var projection=product?BelowProductResourcesR1130.AppendCurrentCatalog(catalog,plan,productMap):BelowPlayerCatalogProjectionV21.AppendCandidate(catalog,plan);
   // Preflight all source JSON before scene/model mutation. Texture/audio native
   // decoding happens below under owned cleanup, not in this read-only plan.
   foreach(var sprite in plan.sprites)ValidateBelowPlayerLibraryV21(data.ReadJson<BelowPlayerLibraryV21>(sprite.animations),sprite);
   if(product)ValidateBelowProductMainResourcesR1130();
   belowPlayerOriginalCatalogV21=catalog;belowPlayerOriginalDungeonV21=selectedDungeon;belowPlayerOriginalPreviewV21=previewDungeon;belowPlayerOriginalPausedV21=game.paused;belowPlayerOriginalMenuV21=openMenu;
   belowPlayerDataV21=data;belowPlayerPlanV21=plan;belowPlayerFxV21=fx;belowPlayerLoadingV21=true;game.paused=true;
   try{
    ClearInput();StopMapMusicPreview();StopAudioVoices();StopBelowPlayerMusicV21();DisposeNativeWorld();DisposeNpcSteamVisual();DisposeOutskirtsNpcVisual();
    foreach(var row in plan.sprites)LoadBelowPlayerLibraryV21(row,data.ReadJson<BelowPlayerLibraryV21>(row.animations));
    foreach(var rule in plan.rules){if(BelowPlayerNoBodyV21(rule.id))continue;Library library;BelowPlayerNeedV21(libraries.TryGetValue(rule.kind,out library),"Missing exact library "+rule.id);foreach(var state in rule.states){string clip=BelowPixelStateClipV21.OverrideClip(rule.id,state.id)??state.clip;BelowPlayerNeedV21(library.frames.ContainsKey(clip),"Source state clip absent "+rule.id+"/"+state.id+"/"+clip);}}
    foreach(var world in plan.worlds){var textures=new Dictionary<string,Texture2D>(StringComparer.Ordinal);belowPlayerWorldTexturesV21.Add(textures);foreach(var row in world.binding.textures){var texture=data.CreatePointTexture(row.selected);belowPlayerOwnedV21.Add(texture);textures.Add(row.sourcePath,texture);}}
    foreach(var row in plan.audio){var clip=data.CreatePcmAudio(row.pcm,row.id,row.sampleRate,row.channels,row.sourcePcmFrames);belowPlayerOwnedV21.Add(clip);AudioClip borrowed;if(audioClips.TryGetValue(row.id,out borrowed))belowPlayerSavedAudioV21.Add(row.id,borrowed);else belowPlayerAbsentAudioV21.Add(row.id);audioClips[row.id]=clip;}
    foreach(var song in plan.music){if(!song.externalOriginal)continue;if(belowPlayerMusicV21.ContainsKey(song.id))continue;var clip=data.CreatePcmAudio(song.pcm,song.id,song.sampleRate,song.channels,song.sourcePcmFrames);belowPlayerOwnedV21.Add(clip);belowPlayerMusicV21.Add(song.id,clip);if(!sceneMusicAssets.ContainsKey(song.id)){sceneMusicAssets.Add(song.id,new SceneMusicAsset{id=song.id,resource="below-player-owned/"+song.id,gain=.22f});belowPlayerAddedMusicV21.Add(song.id);}}
    LoadBelowPlayerVisualV21(data,fx);BelowPlayerNeedV21(BelowVisualConfiguredV21,"Typed FX consumer failed to configure");
    catalog=projection;selectedDungeon=previewDungeon=12;belowPlayerSessionV21=true;belowProductRouteSessionR1130=product;ConfigureBelowPlayerRoomsV21();adventureStarted=true;openMenu=menuOrigin="";pausedBeforeMenu=false;shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();belowPlayerLoadingV21=false;game.paused=false;
   }catch(Exception first){try{DisposeBelowPlayerCandidateV21(true);}catch(Exception cleanup){throw new AggregateException("Below Begin failed; original failure first, cleanup failure second",first,cleanup);}throw;}
  }
}
}
