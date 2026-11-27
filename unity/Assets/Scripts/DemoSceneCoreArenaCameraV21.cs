using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {

  sealed class CoreArenaCameraSavedV21 {
   public Camera world,native;public Transform screen;public RenderTexture target;public SpriteRenderer dim;
   public Vector3 wp,np,sp,ss,ds;public Quaternion wr,nr;public CameraClearFlags nativeClear;public Color nativeBackground;public float wh,nh,na,fy,fv;public int fr;
  }
  CoreArenaCameraSavedV21 coreArenaSavedV21;
  List<CoreArenaActorObservationV21> coreArenaLogicalRowsV21;CoreHardMechanismDocumentV21 coreArenaSourceRulesV21;CoreHardClosureDocumentV21 coreArenaSourceClosureV40;
  bool coreArenaCameraDisabledV21;int coreArenaPreparedFrameV21=-1,coreArenaFitFramesV21,coreArenaRestoresV21;
  float coreArenaWidthV21=480,coreArenaHeightV21=270,coreArenaBottomV21=.34f,coreArenaTopV21=.13f;
  float coreFightHeightV59;
  Vector2 coreFightAimV59;
  const float CoreArenaSideV21=.04f;
  [Serializable] public sealed class CoreArenaActorObservationV21 {
   public string identity,kind,key,sourceState,sourcePose,fitPolicy;public bool originalSourceGeometry,visible,includedInFit,modelActive,modelDormant,aiEnabled,sourceOwnerShown,hasSourceShow,sourceShow,participatedInProjection,actualProjectedBoundsAvailable;public int sourceParts,sourceRosterIndex=-1,sourcePopulationEpoch,sourceOriginalOrder=-1,sourceReplacementRequestIndex=-1;public string sourcePopulationLineage;public float modelX,modelY,nativeDepth,visualScale,npcTime;
   public Vector3 predictedProjectedMin,predictedProjectedMax,predictedViewportMin,predictedViewportMax;public Vector3 rendererScale,logicalRoot,logicalMin,logicalMax,projectedMin,projectedMax,viewportMin,viewportMax,fullRectLogicalMin,fullRectLogicalMax,fullRectProjectedMin,fullRectProjectedMax;
   public float projectedPixelHeight;public bool contained;
  }
  [Serializable] public sealed class CoreArenaObservationV21 {
   public string schema="Core-arena-approved-whole-Event-Plant-to-King-camera-v21",name,room,sourceWorld,key;public bool directorCompleted;public float directorProgress;public string nativeClearFlags;public Color nativeBackground,worldBackground;public bool backgroundOverrideApplied;public bool active,actualPlayMode,allBodiesContained,originalPointTargetUnchanged,actorScaleChanged=false,sourceGeometryPixelApproved=false,naturalRoute=false,runtimeAdmitted=false;
   public int frame,fitFrames,restores,targetWidth,targetHeight,hudMeasuredRects,sourcePopulationCount,hiddenMovieBodiesNotApplicable;public float worldHalf,nativeHalf,nativeUnit,width,height,bottomFraction,topFraction,sideFraction,actualHeroLogicalHeight,referenceHeroLogicalStandingHeight=67;
   public Vector3 worldCameraPosition,nativeCameraPosition,screenScale;public Quaternion nativeCameraRotation;public Rect cameraPixelRect;public CoreArenaActorObservationV21[] actors;
  }
  bool CoreArenaScopeV21(){return coreEntrySessionV21&&forestPreviewActive&&!calibrationRun&&game!=null&&game.room==4&&game.CurrentRoom.id=="Dungeon_Altera_Core_Hard-4"&&NativeWorldActive&&worldCamera&&nativeCamera&&nativeScreen&&nativeTarget&&worldCamera.orthographic&&nativeCamera.orthographic;}
  static bool CoreArenaBodyIdV21(string id){return id=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"||id=="NUI_NASOD_KING"||id=="NUI_EL_POWER_PLANT"||id=="NUI_EL_EXTRACTOR_EARTH"||id=="NUI_EL_EXTRACTOR_FIRE"||id=="NUI_EL_EXTRACTOR_ICE"||id=="NUI_EL_EXTRACTOR_NOVA";}
  public sealed class CoreArenaPopulationV40 {public Enemy actor;public int rosterIndex,epoch,originalOrder=-1,replacementRequestIndex=-1;public bool collectWhole;public string lineage;}
  static void CoreArenaPopulationRequireV40(bool condition,string message){if(!condition)throw new InvalidOperationException("Core arena source population: "+message);}
  static bool CoreArenaElementV40(string npc){return npc=="NUI_EL_EXTRACTOR_EARTH"||npc=="NUI_EL_EXTRACTOR_FIRE"||npc=="NUI_EL_EXTRACTOR_ICE"||npc=="NUI_EL_EXTRACTOR_NOVA";}
  public static CoreArenaPopulationV40[] ValidateCoreArenaPopulationV40(GameModel model,CoreHardMechanismDocumentV21 rules,CoreHardClosureDocumentV21 closure){
   CoreArenaPopulationRequireV40(model!=null&&model.CoreHardConfiguredV21&&model.CoreHardClosureConfiguredV21&&model.CoreHardLocalBalanceConfiguredV21&&model.NativeTerrain&&model.room==4&&model.EncounterIndex==0&&model.CurrentRoom.id=="Dungeon_Altera_Core_Hard-4"&&model.CurrentEncounter.id=="4:0"&&model.CurrentEncounter.sourceWorldId=="WI_ALTERA_CORE_BOSS_STAGE","exact configured Hard final scope required");
   CoreArenaPopulationRequireV40(rules!=null&&rules.births!=null&&rules.births.Length==85&&rules.sourceRespawnDelay==.3f&&closure!=null&&closure.receipt=="5f36e41905e3e659b7a4343b0f07ea8c557fd5ebb82a3b09fa50677a57b12960"&&closure.spawns!=null&&closure.spawns.Length==6&&closure.starts!=null,"frozen source documents required");
   var result=new CoreArenaPopulationV40[model.enemies.Count];var unique=new HashSet<Enemy>();var original=new HashSet<Enemy>();var due=new Dictionary<string,Queue<int>>();
   var originalBirths=Array.FindAll(rules.births,b=>b.room==4);CoreArenaPopulationRequireV40(originalBirths.Length==7&&model.CurrentRoom.enemies.Count==7&&model.enemies.Count>=7,"original seven reference roster required");
   foreach(var b in originalBirths){CoreArenaPopulationRequireV40(b.order>=0&&b.order<7,"original order outside source roster");var e=model.enemies[b.order];var spawn=model.CurrentRoom.enemies[b.order];float hp,physical,magic;CoreArenaPopulationRequireV40(e!=null&&original.Add(e)&&CoreArenaBodyIdV21(e.sourceNPC)&&model.TryGetCoreHardAuthoredStatsV21(e,out hp,out physical,out magic)&&e.sourceNPC==b.npc&&e.sourceNPC==spawn.sourceNPC&&e.kind==spawn.kind&&e.encounterIndex==b.encounter&&e.sourceKeyCode==b.sourceKeyCode&&e.visualScale==spawn.visualScale&&e.countsForClear==spawn.countsForClear&&e.isGate==spawn.isGate,"changed original source identity/reference at "+b.order);}
   for(int i=0;i<model.coreHardCreateRequestsV21.Count;i++){var req=model.coreHardCreateRequestsV21[i];CoreArenaPopulationRequireV40(req!=null&&CoreArenaElementV40(req.npc)&&req.callback=="EL_POWER_PLANT_DOWN_STATE_START"&&req.delay==rules.sourceRespawnDelay&&!req.nativeArgsProven&&req.nativeArgs=="hardLevel,false,startPosition,true,0.3,true,0"&&req.hardLevelArgument=="owner.GetHardLevel() UNRESOLVED"&&!float.IsNaN(req.due)&&!float.IsInfinity(req.due)&&req.due>=0,"changed Plant source replacement request "+i);
    int start=req.sourceStartIndex;bool allowed=req.npc=="NUI_EL_EXTRACTOR_FIRE"?(start==9||start==10):req.npc=="NUI_EL_EXTRACTOR_EARTH"?(start==10||start==11):req.npc=="NUI_EL_EXTRACTOR_NOVA"?(start==20||start==21):(start==21||start==22);CoreArenaPopulationRequireV40(allowed&&Array.Exists(closure.starts,s=>s.start==start),"changed exact replacement source start "+i);
    // Match the existing FlushCoreHardLocalBirths eligibility expression.
    if(model.elapsed+1e-6f<req.due)continue;Queue<int> queue;if(!due.TryGetValue(req.npc,out queue)){queue=new Queue<int>();due.Add(req.npc,queue);}queue.Enqueue(i);
   }
   for(int i=0;i<model.enemies.Count;i++){var e=model.enemies[i];CoreArenaPopulationRequireV40(e!=null&&unique.Add(e),"null or duplicate actor reference at "+i);var row=new CoreArenaPopulationV40{actor=e,rosterIndex=i,epoch=model.CoreHardClosureEpochV21};result[i]=row;
    if(original.Contains(e)){row.originalOrder=i;row.collectWhole=true;row.lineage="original balanced source birth";continue;}
    CoreArenaPopulationRequireV40(model.IsCoreHardLocalBirthV21(e),"actor lacks current closure-epoch constructor membership at "+i+"/"+e.sourceNPC);var prototype=Array.Find(closure.spawns,p=>p.npc==e.sourceNPC);CoreArenaPopulationRequireV40(prototype!=null&&e.kind==prototype.kind&&e.encounterIndex==0&&e.visualScale==prototype.scale&&e.sourceKeyCode==prototype.key&&e.countsForClear==prototype.counts&&!e.isGate&&!e.dormant&&e.maxHp==prototype.hp,"changed local source identity/prototype at "+i+"/"+e.sourceNPC);
    if(CoreArenaElementV40(e.sourceNPC)){Queue<int> queue;CoreArenaPopulationRequireV40(due.TryGetValue(e.sourceNPC,out queue)&&queue.Count>0,"local element lacks one eligible original Plant request at "+i);row.replacementRequestIndex=queue.Dequeue();row.collectWhole=true;row.lineage="current closure epoch local constructor; exact Plant replacement request";}
    else {CoreArenaPopulationRequireV40(e.sourceNPC=="NUI_WALLY_9TH_TYPE2"||e.sourceNPC=="NUI_NASOD_HEALER_BLADER","unreviewed local source identity at "+i+"/"+e.sourceNPC);row.collectWhole=false;row.lineage="current closure epoch original King Attack2 summon; separate existing source renderer";}
   }
   foreach(var pair in due)CoreArenaPopulationRequireV40(pair.Value.Count==0,"eligible Plant request lacks its current-epoch source instance: "+pair.Key);
   return result;
  }
  string CoreArenaWholeKeyV21(Enemy e){return e.sourceNPC=="NUI_NASOD_KING"?CoreKingWholeLastKeyV21:e.sourceNPC=="NUI_EL_POWER_PLANT"?CorePlantWholeLastKeyV21:e.sourceNPC=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"?CoreEventBossWholeLastKeyV21:"";}
  string CoreArenaWholeSourcePoseV21(Enemy e){return e.sourceNPC=="NUI_EL_POWER_PLANT"?CorePlantWholeLastSourcePoseV21:e.sourceNPC=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"?CoreEventBossWholeLastSourcePoseV21:"";}
  Bounds CoreArenaDenseBoundsV21(SpriteRenderer renderer){
   var sprite=renderer.sprite;
   if(!coreInkAlpha64V45.TryGetValue(sprite,out Rect rect)&&!TryCoreArenaAlphaRectV21(sprite.name,out rect)){
    // The frozen alpha table covers only a subset of LK's live animations.
    // Keep measured frames exact; an unmeasured hero frame uses its full local
    // sprite rectangle, including transparent padding, for conservative fit.
    if(renderer!=heroView)rect=CoreArenaAlphaRectV21(sprite.name);
    else rect=new Rect(0,0,sprite.bounds.size.x,sprite.bounds.size.y);
   }
   Vector3 lo=sprite.bounds.min;float x0=lo.x+rect.xMin,x1=lo.x+rect.xMax,y0=lo.y+rect.yMin,y1=lo.y+rect.yMax;
   if(renderer.flipX){float old=x0;x0=-x1;x1=-old;}if(renderer.flipY){float old=y0;y0=-y1;y1=-old;}
   var t=renderer.transform;var b=new Bounds(t.TransformPoint(new Vector3(x0,y0,0)),Vector3.zero);b.Encapsulate(t.TransformPoint(new Vector3(x1,y0,0)));b.Encapsulate(t.TransformPoint(new Vector3(x0,y1,0)));b.Encapsulate(t.TransformPoint(new Vector3(x1,y1,0)));return b;
  }
  bool CoreArenaHiddenMovieV53(Enemy actor){return actor!=null&&actor.sourceNPC=="NUI_EVENT_NPC_ALTERA_CORE_BOSS"&&game.SimpleDangerIntroductions&&!game.CoreHardActorShownV21(actor);}
  bool CoreArenaPixelLogicalBoundsV53(Enemy actor,out Bounds bounds,out Vector3 root,out Vector3 scale){
   bounds=default;root=scale=Vector3.zero;if(!monsterPixelSelectionsV53.TryGetValue(actor,out var selected))return false;
   PreviewRequire(selected.sprite&&selected.carrier&&actorsRoot,"Current Core pixel selection requires its source carrier/root");
   var logical=MonsterPixelLogicalRootV53(selected);root=actorsRoot.TransformPoint(new Vector3(logical.x,logical.y,0));
   float size=selected.definition.scale*actor.visualScale;scale=new Vector3(size*MonsterPixelFacingV53(selected),size,1);
   // Camera fitting runs before the whole-frame owner is projected. Rebuild
   // the exact future local transform from the selected raster, never from a
   // prior-frame projected view or the superseded legacy carrier's crop.
   var parent=selected.carrier.transform.parent;var localRoot=parent?parent.InverseTransformPoint(root):root;
   var matrix=(parent?parent.localToWorldMatrix:Matrix4x4.identity)*Matrix4x4.TRS(localRoot,Quaternion.Euler(0,0,selected.rotationZ),scale);
   var local=selected.sprite.bounds;bool first=true;
   foreach(float x in new[]{local.min.x,local.max.x})foreach(float y in new[]{local.min.y,local.max.y}){var point=matrix.MultiplyPoint3x4(new Vector3(x,y,0));if(first){bounds=new Bounds(point,Vector3.zero);first=false;}else bounds.Encapsulate(point);}
   return true;
  }
  List<CoreArenaActorObservationV21> CoreArenaCollectV21(bool projected){
   if(coreArenaSourceRulesV21==null)coreArenaSourceRulesV21=CoreEntryJsonV21<CoreHardMechanismDocumentV21>(coreEntryScopeV21.mechanisms);if(coreArenaSourceClosureV40==null)coreArenaSourceClosureV40=CoreEntryJsonV21<CoreHardClosureDocumentV21>(coreEntryScopeV21.closure);var population=ValidateCoreArenaPopulationV40(game,coreArenaSourceRulesV21,coreArenaSourceClosureV40);int expectedWhole=0;foreach(var member in population)if(member.collectWhole&&!CoreArenaHiddenMovieV53(member.actor))expectedWhole++;
   var rows=new List<CoreArenaActorObservationV21>();
   CoreArenaAddBoundsV21(rows,"LK",catalog.hero.id,"",false,1,CoreArenaDenseBoundsV21(heroView),heroView.transform.position,heroView.transform.localScale,RenderPlayerPosition.x,RenderPlayerPosition.y,0,1,projected);
   rows[0].fullRectLogicalMin=heroView.bounds.min;rows[0].fullRectLogicalMax=heroView.bounds.max;
   rows[0].visible=true;rows[0].includedInFit=true;rows[0].fitPolicy="Always preserve actual LK body, original logical standing height67";
   for(int i=0;i<game.enemies.Count;i++){var e=game.enemies[i];var member=population[i];if(!member.collectWhole||CoreArenaHiddenMovieV53(e))continue;
    PreviewRequire(e.visualScale==1,"Core arena camera may not change original visualScale");var sprite=enemyViews[i];
    bool pixel=CoreArenaPixelLogicalBoundsV53(e,out var bounds,out var root,out var scale);
    if(!pixel){PreviewRequire(sprite&&sprite.sprite,"Core arena approved whole body absent");bounds=CoreArenaDenseBoundsV21(sprite);root=sprite.transform.position;scale=sprite.transform.localScale;}
    // Original bodies and verified current-epoch element replacements use whole sprites.
    // Event/Plant source assemblies are deliberately not registered by this host.
    CoreArenaAddBoundsV21(rows,e.sourceNPC,e.kind,pixel?monsterPixelSelectionsV53[e].sprite.name:CoreArenaWholeKeyV21(e),false,1,bounds,root,scale,e.x,e.y,e.nativeDepth,e.visualScale,projected);
    var row=rows[rows.Count-1];row.sourceRosterIndex=member.rosterIndex;row.sourcePopulationEpoch=member.epoch;row.sourceOriginalOrder=member.originalOrder;row.sourceReplacementRequestIndex=member.replacementRequestIndex;row.sourcePopulationLineage=member.lineage;row.fullRectLogicalMin=pixel?bounds.min:sprite.bounds.min;row.fullRectLogicalMax=pixel?bounds.max:sprite.bounds.max;row.sourceState=e.npcState;row.npcTime=e.npcTime;row.visible=sprite.gameObject.activeInHierarchy&&sprite.enabled&&(!pixel||e.active&&!e.dormant);
    bool plantConcealed=e.sourceNPC=="NUI_EL_POWER_PLANT"&&(e.npcState=="EL_POWER_PLANT_START"||e.npcState=="EL_POWER_PLANT_WAIT_DOWN"||e.npcState=="EL_POWER_PLANT_DOWN");row.includedInFit=row.visible&&!plantConcealed;row.modelActive=e.active;row.modelDormant=e.dormant;row.aiEnabled=e.aiEnabled;row.sourceOwnerShown=game.CoreHardActorShownV21(e);row.sourcePose=CoreArenaWholeSourcePoseV21(e);if(coreArenaSourceRulesV21==null)coreArenaSourceRulesV21=CoreEntryJsonV21<CoreHardMechanismDocumentV21>(coreEntryScopeV21.mechanisms);var raw=Array.Find(coreArenaSourceRulesV21.states,q=>q.npc==e.sourceNPC&&q.state==e.npcState);row.hasSourceShow=raw!=null&&raw.hasSourceShow;row.sourceShow=raw!=null&&raw.sourceShow;
    row.fitPolicy=plantConcealed?"Original Plant concealed/down source phase: retain exact floor and bounds, do not fit underground body":row.includedInFit?"Actual visible body and unchanged source visibility owner":"Original visibility owner hides body; record root without forcing reveal";
    if(CoreHardProductSessionActiveR1147&&game.SimpleDangerIntroductions&&(e.hp<=0||e.sourceNPC=="NUI_NASOD_KING")){
     // The stationary King's wings and lower chassis form the backdrop. The
     // player attacks the live extractors and then the raised central Plant.
     row.includedInFit=false;row.fitPolicy="Encounter backdrop or defeated device";
    }
    if(pixel){row.sourcePose=monsterPixelSelectionsV53[e].motion;row.fitPolicy+="; current v53 selected raster rectangle, including conservative transparent crop padding";}
   }
   PreviewRequire(rows.Count==1+expectedWhole,"Core arena exact verified source population and LK whole bounds required");return rows;
  }
  void CoreArenaAddBoundsV21(List<CoreArenaActorObservationV21> rows,string id,string kind,string key,bool original,int parts,Bounds b,Vector3 root,Vector3 scale,float mx,float my,float depth,float visual,bool projected){
   var row=new CoreArenaActorObservationV21{identity=id,kind=kind,key=key,originalSourceGeometry=original,sourceParts=parts,modelX=mx,modelY=my,nativeDepth=depth,visualScale=visual,rendererScale=scale,logicalRoot=root,logicalMin=b.min,logicalMax=b.max};
   // Production FinishNativeProjection projects each actual whole-renderer
   // origin while local bounds and scale stay unchanged. The selected sprite
   // anchor already includes source floor, crop origin and Plant whole offset.
   // Project that renderer origin once; never add any source root shift here.
   Vector3 delta=projected?ProjectNativePoint(root,depth)-root:Vector3.zero;row.projectedMin=b.min+delta;row.projectedMax=b.max+delta;rows.Add(row);
  }
  void CoreArenaBaseNativeV21(){var c=worldCamera.transform.position;var at=NativePathPoint(c.x);at.y=(c.y-nativeScene.offsetY)/nativeScene.unit;var normal=NativeNormal(c.x);nativeCamera.transform.SetPositionAndRotation(at+normal*2200,Quaternion.LookRotation(-normal,Vector3.up));nativeScreen.transform.localPosition=new Vector3(c.x,c.y,10);}
  void CoreArenaSizesV21(float height){coreArenaHeightV21=height;coreArenaWidthV21=height*displayWorldAspect;worldCamera.orthographicSize=height*.5f;nativeCamera.orthographicSize=height*.5f/nativeScene.unit;nativeCamera.aspect=displayWorldAspect;nativeScreen.transform.localScale=new Vector3(coreArenaWidthV21,coreArenaHeightV21,1);if(sceneDim){sceneDim.transform.localScale=Vector3.Scale(coreArenaSavedV21.ds,new Vector3(coreArenaWidthV21/480,coreArenaHeightV21/270,1));}}
  void FinalizeCoreArenaCameraV21(){
   if(!CoreArenaScopeV21()){RestoreCoreArenaCameraV21();return;}
   // Wait for an original birth; a registered hit already supplies a visible body for camera fitting.
   foreach(var e in game.enemies){string initialState;if(game.TryCoreHardSourceInitializationPendingV32(e,out initialState)){RestoreCoreArenaCameraV21();coreArenaLogicalRowsV21=null;return;}}
   if(coreArenaCameraDisabledV21){RestoreCoreArenaCameraV21();coreArenaLogicalRowsV21=CoreArenaCollectV21(false);return;}
   bool newRoom=coreArenaSavedV21==null;
   if(newRoom)coreArenaSavedV21=new CoreArenaCameraSavedV21{world=worldCamera,native=nativeCamera,screen=nativeScreen.transform,target=nativeTarget,dim=sceneDim,wp=worldCamera.transform.position,wr=worldCamera.transform.rotation,np=nativeCamera.transform.position,nr=nativeCamera.transform.rotation,sp=nativeScreen.transform.localPosition,ss=nativeScreen.transform.localScale,ds=sceneDim?sceneDim.transform.localScale:Vector3.one,wh=worldCamera.orthographicSize,nh=nativeCamera.orthographicSize,na=nativeCamera.aspect,fy=cameraFollowY,fv=cameraFollowVelocity,fr=cameraFollowRoom,nativeClear=nativeCamera.clearFlags,nativeBackground=nativeCamera.backgroundColor};
   // Exact indoor Core arena only. Reuse the existing game-camera dark clear
   // as a finite display adapter; do not invent scenery or claim native color parity.
   if(nativeScene.id=="WI_ALTERA_CORE_BOSS_STAGE"){nativeCamera.clearFlags=CameraClearFlags.SolidColor;nativeCamera.backgroundColor=worldCamera.backgroundColor;}
   var rows=CoreArenaCollectV21(false);coreArenaLogicalRowsV21=rows;Vector3 lo=rows[0].logicalMin,hi=rows[0].logicalMax;foreach(var r in rows)if(r.includedInFit){lo=Vector3.Min(lo,r.logicalMin);hi=Vector3.Max(hi,r.logicalMax);}
   // Read actual current HUD geometry; conservative minima also cover the
   // temporary offscreen Canvas mode. These are camera fractions, not pixels
   // added to actors, and do not alter the original native render target.
   MeasureBelowHudMarginV21();var pixel=worldCamera.pixelRect;coreArenaBottomV21=.34f;coreArenaTopV21=.13f;
   if(belowHudGeometryMeasuredV21&&pixel.height>0)coreArenaBottomV21=Math.Max(coreArenaBottomV21,(belowHudTopScreenV21-pixel.yMin)/pixel.height+8f/270);
   bool combat=CoreHardProductSessionActiveR1147&&game.SimpleDangerIntroductions;
   if(combat&&MeasureCoreProductHudR1149(out var hudBottom,out var hudTop)){
    coreArenaBottomV21=Mathf.Max(.04f,(hudBottom-pixel.yMin)/pixel.height+.012f);
    coreArenaTopV21=Mathf.Max(.04f,(pixel.yMax-hudTop)/pixel.height+.012f);
   }
   PreviewRequire(coreArenaBottomV21+coreArenaTopV21<.85f,"Core arena HUD leaves no finite view");
   worldCamera.transform.position=new Vector3((lo.x+hi.x)*.5f,(lo.y+hi.y)*.5f,worldCamera.transform.position.z);CoreArenaSizesV21(270);CoreArenaBaseNativeV21();
   rows=CoreArenaCollectV21(true);lo=rows[0].projectedMin;hi=rows[0].projectedMax;foreach(var r in rows)if(r.includedInFit){lo=Vector3.Min(lo,r.projectedMin);hi=Vector3.Max(hi,r.projectedMax);}
   float h=Math.Max(270,Math.Max((hi.x-lo.x+24)/(displayWorldAspect*(1-2*CoreArenaSideV21)),(hi.y-lo.y+24)/(1-coreArenaBottomV21-coreArenaTopV21)));
   float blend=1-Mathf.Exp(-5*Time.unscaledDeltaTime);
   if(combat){
    // Preserve combat context at the exposed core. Zoom toward remaining
    // devices progressively; newly respawned devices must be visible at once.
    float required=Mathf.Max(620,h);
    h=newRoom?required:Mathf.Max(required,Mathf.Lerp(coreFightHeightV59,required,blend));
    coreFightHeightV59=h;
   }
   CoreArenaSizesV21(h);CoreArenaBaseNativeV21();
   rows=CoreArenaCollectV21(true);lo=rows[0].projectedMin;hi=rows[0].projectedMax;foreach(var r in rows)if(r.includedInFit){lo=Vector3.Min(lo,r.projectedMin);hi=Vector3.Max(hi,r.projectedMax);}
   var c=worldCamera.transform.position;float dx=(lo.x+hi.x)*.5f-c.x,dy=(lo.y+hi.y)*.5f-c.y-h*(coreArenaBottomV21-coreArenaTopV21)*.5f;
   if(combat){
    float halfWidth=h*(displayWorldAspect)*(.5f-CoreArenaSideV21);
    float maxY=lo.y+h*(.5f-coreArenaBottomV21)-12;
    var aim=new Vector2((hi.x+lo.x)*.5f,maxY);
    if(!newRoom)aim=Vector2.Lerp(coreFightAimV59,aim,blend);
    aim.x=Mathf.Clamp(aim.x,hi.x-halfWidth,lo.x+halfWidth);
    aim.y=Mathf.Clamp(aim.y,hi.y-h*(.5f-coreArenaTopV21),maxY);
    coreFightAimV59=aim;dx=aim.x-c.x;dy=aim.y-c.y;
   }
   // Native-plane centering handles true route curvature/nativeDepth. Paired
   // W/H in ProjectNativePoint keeps the body scale exactly one world unit.
   nativeCamera.transform.position+=nativeCamera.transform.right*(dx/nativeScene.unit)+Vector3.up*(dy/nativeScene.unit);coreArenaPreparedFrameV21=Time.frameCount;coreArenaFitFramesV21++;
  }
  bool ApplyCoreArenaPreparedNativeV21(bool render){if(!CoreArenaScopeV21()||coreArenaCameraDisabledV21||coreArenaSavedV21==null||coreArenaPreparedFrameV21!=Time.frameCount)return false;if(render)nativeCamera.Render();return true;}
  void AdjustCoreArenaProjectionV21(ref float w,ref float h){if(CoreArenaScopeV21()&&!coreArenaCameraDisabledV21&&coreArenaSavedV21!=null){w=coreArenaWidthV21;h=coreArenaHeightV21;}}
  void RestoreCoreArenaCameraV21(){var s=coreArenaSavedV21;if(s==null)return;if(s.world){s.world.orthographicSize=s.wh;s.world.transform.SetPositionAndRotation(s.wp,s.wr);}if(s.native){s.native.clearFlags=s.nativeClear;s.native.backgroundColor=s.nativeBackground;s.native.orthographicSize=s.nh;s.native.aspect=s.na;s.native.transform.SetPositionAndRotation(s.np,s.nr);}if(s.screen){s.screen.localPosition=s.sp;s.screen.localScale=s.ss;}if(s.dim)s.dim.transform.localScale=s.ds;cameraFollowY=s.fy;cameraFollowVelocity=s.fv;cameraFollowRoom=s.fr;coreArenaSavedV21=null;coreArenaPreparedFrameV21=-1;coreArenaWidthV21=480;coreArenaHeightV21=270;coreArenaRestoresV21++;}
  public bool CoreArenaCameraReleasedV21 {get{return coreArenaSavedV21==null;}}
  public bool CoreBossCameraRestoredV59(){return coreArenaSavedV21==null&&coreArenaPreparedFrameV21<0;}
  public CoreArenaObservationV21 VerifyCoreBossCameraV59(string phase){
   var frame=CoreArenaObserveV21(phase);
   if(!frame.active||!frame.allBodiesContained||coreArenaPreparedFrameV21!=Time.frameCount)
    throw new InvalidOperationException("Core boss camera lost or cropped a visible actor at "+phase);
   if(Mathf.Abs(frame.worldHalf*2-frame.height)>.05f||Mathf.Abs(frame.nativeHalf*frame.nativeUnit-frame.worldHalf)>.05f)
    throw new InvalidOperationException("Core boss world and native camera scales diverged at "+phase);
   return frame;
  }
  public CoreArenaObservationV21 CoreArenaObserveV21(string name){
   PreviewRequire(CoreArenaScopeV21(),"Core arena observation outside exact scope");
   var population=ValidateCoreArenaPopulationV40(game,coreArenaSourceRulesV21,coreArenaSourceClosureV40);var projected=new HashSet<Transform>();foreach(var p in projectionRestore)if(p.transform)projected.Add(p.transform);
   var actors=new List<CoreArenaActorObservationV21>();var hero=CoreArenaDenseBoundsV21(heroView);
   actors.Add(new CoreArenaActorObservationV21{identity="LK",kind=catalog.hero.id,visible=heroView.gameObject.activeInHierarchy&&heroView.enabled,participatedInProjection=projected.Contains(heroView.transform),projectedMin=hero.min,projectedMax=hero.max,fullRectProjectedMin=heroView.bounds.min,fullRectProjectedMax=heroView.bounds.max,rendererScale=heroView.transform.localScale,visualScale=1,sourceParts=1});
   int hiddenMovies=0;
   for(int i=0;i<game.enemies.Count;i++){var e=game.enemies[i];var member=population[i];if(!member.collectWhole)continue;if(CoreArenaHiddenMovieV53(e)){hiddenMovies++;continue;}
    var sprite=enemyViews[i];bool pixel=monsterPixelSelectionsV53.TryGetValue(e,out var selected);
    if(pixel){PreviewRequire(monsterPixelViewsV53.TryGetValue(e,out var view)&&view.renderer&&view.renderer.sprite==selected.sprite,"Actual current Core pixel whole absent");sprite=view.renderer;}
    PreviewRequire(sprite&&sprite.sprite,"Actual approved arena whole absent");var b=pixel?sprite.bounds:CoreArenaDenseBoundsV21(sprite);bool visible=sprite.gameObject.activeInHierarchy&&sprite.enabled,participated=projected.Contains(sprite.transform);
    if(pixel){var logical=MonsterPixelLogicalRootV53(selected);var root=actorsRoot.TransformPoint(new Vector3(logical.x,logical.y,0));participated=(sprite.transform.position-ProjectNativePoint(root,e.nativeDepth)).sqrMagnitude<.0001f;}
    actors.Add(new CoreArenaActorObservationV21{identity=e.sourceNPC,sourceRosterIndex=member.rosterIndex,sourcePopulationEpoch=member.epoch,sourceOriginalOrder=member.originalOrder,sourceReplacementRequestIndex=member.replacementRequestIndex,sourcePopulationLineage=member.lineage,kind=e.kind,key=pixel?selected.sprite.name:CoreArenaWholeKeyV21(e),originalSourceGeometry=false,sourceParts=1,modelX=e.x,modelY=e.y,nativeDepth=e.nativeDepth,visualScale=e.visualScale,rendererScale=sprite.transform.localScale,visible=visible,participatedInProjection=participated,projectedMin=b.min,projectedMax=b.max,fullRectProjectedMin=sprite.bounds.min,fullRectProjectedMax=sprite.bounds.max});
   }
   bool all=true;foreach(var row in actors){var original=coreArenaLogicalRowsV21.Find(q=>q.identity==row.identity&&q.sourceRosterIndex==row.sourceRosterIndex&&q.sourcePopulationEpoch==row.sourcePopulationEpoch);PreviewRequire(original!=null,"Actual original logical body observation absent");row.logicalMin=original.logicalMin;row.logicalMax=original.logicalMax;row.fullRectLogicalMin=original.fullRectLogicalMin;row.fullRectLogicalMax=original.fullRectLogicalMax;row.logicalRoot=original.logicalRoot;row.sourceState=original.sourceState;row.sourcePose=original.sourcePose;row.npcTime=original.npcTime;row.includedInFit=original.includedInFit;row.fitPolicy=original.fitPolicy;row.modelActive=original.modelActive;row.modelDormant=original.modelDormant;row.aiEnabled=original.aiEnabled;row.sourceOwnerShown=original.sourceOwnerShown;row.hasSourceShow=original.hasSourceShow;row.sourceShow=original.sourceShow;
    var delta=ProjectNativePoint(original.logicalRoot,row.nativeDepth)-original.logicalRoot;row.predictedProjectedMin=row.logicalMin+delta;row.predictedProjectedMax=row.logicalMax+delta;row.predictedViewportMin=worldCamera.WorldToViewportPoint(row.predictedProjectedMin);row.predictedViewportMax=worldCamera.WorldToViewportPoint(row.predictedProjectedMax);
    row.actualProjectedBoundsAvailable=row.visible&&row.participatedInProjection;
    if(row.actualProjectedBoundsAvailable){row.viewportMin=worldCamera.WorldToViewportPoint(row.projectedMin);row.viewportMax=worldCamera.WorldToViewportPoint(row.projectedMax);row.projectedPixelHeight=(row.viewportMax.y-row.viewportMin.y)*worldCamera.pixelRect.height;row.contained=row.viewportMin.x>=CoreArenaSideV21-.001f&&row.viewportMax.x<=1-CoreArenaSideV21+.001f&&row.viewportMin.y>=coreArenaBottomV21-.001f&&row.viewportMax.y<=1-coreArenaTopV21+.001f;}else{row.projectedMin=row.projectedMax=row.fullRectProjectedMin=row.fullRectProjectedMax=Vector3.zero;row.viewportMin=row.viewportMax=Vector3.zero;row.projectedPixelHeight=0;row.contained=false;}
    if(row.includedInFit)all&=row.actualProjectedBoundsAvailable&&row.contained;
   }
   return new CoreArenaObservationV21{name=name,sourcePopulationCount=population.Length,hiddenMovieBodiesNotApplicable=hiddenMovies,room=game.CurrentRoom.id,sourceWorld=nativeScene.id,key=CoreKingWholeLastKeyV21,directorCompleted=game.CoreHardDirectorCompletedV21,directorProgress=game.CoreHardDirectorProgressV21,nativeClearFlags=nativeCamera.clearFlags.ToString(),nativeBackground=nativeCamera.backgroundColor,worldBackground=worldCamera.backgroundColor,backgroundOverrideApplied=coreArenaSavedV21!=null&&nativeScene.id=="WI_ALTERA_CORE_BOSS_STAGE"&&nativeCamera.clearFlags==CameraClearFlags.SolidColor&&nativeCamera.backgroundColor==worldCamera.backgroundColor,actualPlayMode=Application.isPlaying,active=coreArenaSavedV21!=null,allBodiesContained=all,frame=Time.frameCount,fitFrames=coreArenaFitFramesV21,restores=coreArenaRestoresV21,worldHalf=worldCamera.orthographicSize,nativeHalf=nativeCamera.orthographicSize,nativeUnit=nativeScene.unit,width=coreArenaWidthV21,height=coreArenaHeightV21,bottomFraction=coreArenaBottomV21,topFraction=coreArenaTopV21,sideFraction=CoreArenaSideV21,targetWidth=nativeTarget.width,targetHeight=nativeTarget.height,originalPointTargetUnchanged=nativeTarget.width==480&&nativeTarget.height==270&&nativeTarget.filterMode==FilterMode.Point&&(coreArenaSavedV21==null||nativeTarget==coreArenaSavedV21.target),screenScale=nativeScreen.transform.localScale,worldCameraPosition=worldCamera.transform.position,nativeCameraPosition=nativeCamera.transform.position,nativeCameraRotation=nativeCamera.transform.rotation,cameraPixelRect=worldCamera.pixelRect,hudMeasuredRects=belowHudMeasuredRectsV21,actualHeroLogicalHeight=hero.size.y,actors=actors.ToArray()};
  }
  // Explicit component fixture: render a copy with an existing approved
  // state/clock; model actor, original npcTime and root remain untouched.

#if UNITY_EDITOR
  public string CoreArenaPoseComponentFixtureV21(string key){
   RestoreNativeProjection();Enemy original=null;int ix=-1;for(int i=0;i<game.enemies.Count;i++)if(game.enemies[i].sourceNPC=="NUI_NASOD_KING"){original=game.enemies[i];ix=i;break;}PreviewRequire(original!=null,"Original King actor required");
   CoreKingWholeStateV21 selected=null;float time=0;foreach(var state in coreKingWholeLookupV21.document.states){var clip=coreKingWholeLookupV21.Clip(state.displayClip);for(int i=0;i<clip.frames.Length;i++)if(clip.frames[i].paintedPoseKey==key){selected=state;time=(float)((i+.25)/clip.fps);break;}if(selected!=null)break;}PreviewRequire(selected!=null,"Exact approved key has no source display frame");
   var copy=(Enemy)typeof(object).GetMethod("MemberwiseClone",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(original,null);copy.npcState=selected.state;copy.npcTime=time;
   RenderCoreKingWholeCandidateV21(enemyViews[ix],copy,RenderEnemyPosition(original));PreviewRequire(CoreKingWholeLastKeyV21==key,"Exact component requested key must equal actual production lookup key");enemyViews[ix].gameObject.SetActive(true);enemyViews[ix].enabled=true;FinalizeCoreArenaCameraV21();RenderCombatPresentation();FinishNativeProjection();return selected.state+"@"+time.ToString("R")+" explicit renderer-visible component copy";
  }
#endif


 }
}
