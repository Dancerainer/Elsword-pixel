using System;
using System.Collections.Generic;

namespace ElswordPixel {
 // Typed pre-read only: all original floats use the existing exact JSON reader.
 // The shared source DTO names do not activate any Mine2 runtime consumer.
 public sealed class PlainProductDataPlanR1320 {
  public PlainProductResourceIndexR1320 index;
  public Mine2ProductModelR1211 model;public NpcDefinition[] rules;
  public NpcEffectDocument effects;public NpcSoundDocument sounds;public NpcMechanismDocument mechanisms;
  public Mine2ProductWorldPayloadR1211[] worlds;public Mine2ProductLibraryPayloadR1211[] libraries;public PlainPagedLibraryPayloadR1330[] pagedLibraries;
  public PlainOriginalFxCallbackPointR1315[] callbackPoints;public PlainVariablePhaseDocumentR1330 variablePhases;
  public bool[][] ordinaryAI;public int ordinaryAICount,managedPopulationCount,managedMineCount;
  static bool OrdinaryCombat(string id){switch(id){
   case "NUI_AIRSHIP_SOLDIER_GUNNER":case "NUI_AIRSHIP_SOLDIER_SWORDMAN":
   case "NUI_NASOD_MINER_THIN_DEFECT":case "NUI_NASOD_GUARDIAN_SMALL":
   case "NUI_NASOD_WATCH":case "NUI_NASOD_SPRAY":case "NUI_NASOD_MOUSE":
   case "NUI_NASOD_ELEMENT_FIRE":case "NUI_NASOD_ELEMENT_FIRE_SOLO":
   case "NUI_NASOD_ELEMENT_ICE":case "NUI_NASOD_ELEMENT_ICE_SOLO":
   case "NUI_NASOD_DRILLER_CANNON":return true;default:return false;
  }}
  static int PngInt(byte[] b,int i){return checked((int)((uint)b[i]<<24|(uint)b[i+1]<<16|(uint)b[i+2]<<8|b[i+3]));}
  public static PlainProductDataPlanR1320 Build(PlainProductResourceIndexR1320 index){
   PlainProductResourceIndexR1320.Need(index!=null,"index absent");var map=index.Map;bool normal=map.sourceDungeon==PlainProductResourceIndexR1320.Normal;
   byte[] modelBytes=index.RootBytes("model");
   var p=new PlainProductDataPlanR1320{index=index,
    model=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductModelR1211>(modelBytes),
    effects=FactoryTypedJsonR581.ReadMine2ProductRootR1226<NpcEffectDocument>(index.RootBytes("effects")),
    sounds=FactoryTypedJsonR581.ReadMine2ProductRootR1226<NpcSoundDocument>(index.RootBytes("sounds")),
    mechanisms=FactoryTypedJsonR581.Read<NpcMechanismDocument>(index.RootBytes("runtimeMechanisms")),
    worlds=new Mine2ProductWorldPayloadR1211[3],libraries=new Mine2ProductLibraryPayloadR1211[map.libraries.Length],ordinaryAI=new bool[3][]};
   var audit=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductAuditR1211>(modelBytes);
   var rules=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductRulesR1211>(index.RootBytes("rules"));
   PlainProductResourceIndexR1320.Need(p.model!=null&&p.model.sourceGraph==map.sourceDungeon+".lua"&&!p.model.runtimeAdmitted&&p.model.rooms!=null&&p.model.rooms.Length==3&&audit!=null&&audit.rooms!=null&&audit.rooms.Length==3&&rules!=null&&!rules.runtimeAdmitted&&!rules.sourceMechanismsComplete,"original data flags or model shape changed");
   p.rules=GameModel.ProjectPlainOriginalDefinitionsR1258(map.sourceDungeon,PlainProductResourceIndexR1320.RulesSHA,rules.definitions);
   var points=FactoryTypedJsonR581.ReadPlainProductCallbackPointsR1320(index.Read(index.Document.callbackPoints));
   PlainProductResourceIndexR1320.Need(points!=null&&points.schema=="plain-three-actual-original-callback-points-r1315"&&points.points!=null&&points.points.Length==3,"exact original callback point document changed");p.callbackPoints=points.points;
   int births=0,zones=0,sourceActive=0,states=0;foreach(var d in p.rules)states+=d.states.Length;
   for(int i=0;i<3;i++){
    var room=p.model.rooms[i];var a=audit.rooms[i];var w=map.worlds[i];
    PlainProductResourceIndexR1320.Need(room!=null&&room.id==map.sourceDungeon+"-"+i&&room.enemies!=null&&room.encounters!=null&&room.surfaces!=null&&room.groundNodes!=null&&a!=null&&a.enemies!=null&&a.enemies.Length==room.enemies.Length&&w!=null&&w.room==i&&w.id==room.sourceWorldId,"original room/world/birth cohort changed");
    p.ordinaryAI[i]=new bool[room.enemies.Length];
    for(int j=0;j<room.enemies.Length;j++){
     var e=room.enemies[j];var raw=a.enemies[j];
     PlainProductResourceIndexR1320.Need(e!=null&&raw!=null&&e.sourceNPC==raw.sourceNPC&&!e.aiEnabled&&!raw.aiEnabled&&Array.Exists(p.rules,d=>d.id==e.sourceNPC),"original birth AI-off declaration or profile changed");
     // sourceActive is not a blanket ordinary-AI switch. House and Mine use
     // their exact managed environment/population path before the AI gate.
     p.ordinaryAI[i][j]=raw.sourceActive&&OrdinaryCombat(e.sourceNPC);
     if(p.ordinaryAI[i][j])p.ordinaryAICount++;
     if(e.sourceNPC=="NUI_NASOD_MOUSE_HOUSE"){PlainProductResourceIndexR1320.Need(normal&&raw.sourceActive,"original active House birth changed");p.managedPopulationCount++;}
     if(e.sourceNPC=="NUI_ALTERA_MINE"){PlainProductResourceIndexR1320.Need(!normal&&!raw.sourceActive&&!e.countsForClear,"original passive-clear Mine birth changed");p.managedMineCount++;}
     if(raw.sourceActive)sourceActive++;births++;
    }
    zones+=room.encounters.Length;
    var sceneBytes=index.Read(w.scene);var scene=index.Json<Mine2ProductSceneR1211>(w.scene);var projection=index.Json<Mine2ProductProjectionR1211>(w.projection);var geometry=index.Read(w.binary);
    PlainProductResourceIndexR1320.Need(scene!=null&&scene.id==w.id&&scene.parts!=null&&projection!=null&&projection.id==w.id&&projection.unit==.28f&&projection.width==room.width&&projection.path!=null&&projection.path.Length>0,"original source projection changed");
    foreach(var q in projection.path)PlainProductResourceIndexR1320.Need(q!=null&&q.start!=null&&q.start.Length==3&&q.end!=null&&q.end.Length==3,"original source XYZ path changed");
    foreach(var q in scene.parts)PlainProductResourceIndexR1320.Need(q!=null&&q.offset>=0&&q.count>0&&(long)q.offset+(long)q.count*20<=geometry.LongLength&&(string.IsNullOrEmpty(q.texture)||Array.Exists(w.textures,t=>t.sourcePath==q.texture)),"original mesh/texture range absent");
    p.worlds[i]=new Mine2ProductWorldPayloadR1211{id=w.id,originalSceneJson=sceneBytes,originalGeometry=geometry,projection=projection,textures=w.textures};
   }
   PlainProductResourceIndexR1320.Need(births==(normal?92:95)&&zones==6&&states==(normal?284:187)&&sourceActive==(normal?67:66)&&p.ordinaryAICount==(normal?62:66)&&p.managedPopulationCount==(normal?5:0)&&p.managedMineCount==(normal?0:20),"original per-role activation cohort changed");
   p.variablePhases=PlainVariablePhaseDataR1330.Read(index,p.rules);
   var seen=new HashSet<string>(StringComparer.Ordinal);
   for(int i=0;i<map.libraries.Length;i++){
    var binding=map.libraries[i];var d=index.Json<Mine2ProductLibraryDocumentR1211>(binding.document);var bytes=index.Read(binding.atlas);
    PlainProductResourceIndexR1320.Need(bytes.Length>=24&&bytes[0]==137&&bytes[1]==80&&bytes[2]==78&&bytes[3]==71&&bytes[12]==73&&bytes[13]==72&&bytes[14]==68&&bytes[15]==82,"original atlas PNG header missing");int width=PngInt(bytes,16),height=PngInt(bytes,20);
    var profile=Array.Find(p.rules,r=>r.id==binding.npc);
    PlainProductResourceIndexR1320.Need(width>0&&height>0&&d!=null&&d.id==binding.id&&d.sourceNPC==binding.npc&&profile!=null&&profile.kind==d.id&&seen.Add(binding.npc)&&d.atlas!=null&&d.atlas.groundAnchor!=null&&d.atlas.groundAnchor.Length==2&&d.logicalScale==1&&d.clips!=null,"exact NPC library/physical scale mismatch");
    var clips=new HashSet<string>(StringComparer.Ordinal);foreach(var c in d.clips){PlainProductResourceIndexR1320.Need(c!=null&&clips.Add(c.id)&&c.fps==60&&c.durationSeconds>0&&c.frames!=null&&c.frames.Length>0,"original display clip/timing absent");foreach(var f in c.frames)PlainProductResourceIndexR1320.Need(f!=null&&f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2&&f.rect[0]>=0&&f.rect[1]>=0&&f.rect[2]>0&&f.rect[3]>0&&(long)f.rect[0]+f.rect[2]<=width&&(long)f.rect[1]+f.rect[3]<=height,"original atlas crop/anchor absent");}
    foreach(var s in profile.states)PlainProductResourceIndexR1320.Need(clips.Contains(s.clip),"exact source state display clip absent; no idle fallback");
    p.libraries[i]=new Mine2ProductLibraryPayloadR1211{binding=binding,document=d,atlasWidth=width,atlasHeight=height};
   }
   p.pagedLibraries=PlainPagedLibraryDataR1330.Read(index,p.rules,seen);
   PlainProductResourceIndexR1320.Need(seen.Count==p.rules.Length-1-PlainVariablePhaseDataR1330.RouteCount(map.sourceDungeon)&&Array.TrueForAll(p.rules,r=>r.id=="NUI_NASOD_WATCH"||PlainVariablePhaseDataR1330.IsNpc(r.id)||seen.Contains(r.id)),"exact non-Watch atlas cohort incomplete");
   return p;
  }
 }
 // Plain-only two-page original uniform reference adapter. Shared Mine2 DTOs and
 // legacy single-atlas readers remain unchanged. No AI or model clock is written.
 [Serializable] public sealed class PlainPagedLibraryBindingR1330 {public string id,npc;public Mine2ProductFileR1211 document;}
 [Serializable] public sealed class PlainPagedLibraryPageR1330 {public string pageKey,rgbaSha256;public Mine2ProductFileR1211 file;public int width,height;}
 [Serializable] public sealed class PlainPagedLibraryFrameR1330 {public string poseID,animation,pageKey,rgbaSha256;public double sourceTime,sourceDuration;public int originalGeometryOffset,uniqueGroupIndex;public int[] rect;public double[] anchor;}
 [Serializable] public sealed class PlainPagedLibraryClipR1330 {public string id;public double fps,durationSeconds;public bool loop;public PlainPagedLibraryFrameR1330[] frames;}
 [Serializable] public sealed class PlainPagedLibraryClockR1330 {public string id,animation;public double speed,nativeDuration,durationSeconds;public int steps;public bool loop;public string[] stateIds;public bool[] originalStateLoops;}
 [Serializable] public sealed class PlainPagedLibraryStateR1330 {public string state,modelClip,originalSourceAnimation,originalNextState;public double originalSourceAnimSpeed,originalMotionDuration,originalStateDuration;public bool originalLoop;}
 [Serializable] public sealed class PlainPagedLibraryDocumentR1330 {
  public string schema,id,sourceNPC,artVersion,sourceClockDesignSha256,sourceClockDesignJson,textureRgbaHashDomain;
  public bool sourceOnly,dataOnly,pixelArtAccepted,nativeEquivalent,runtimeReleaseAccepted,naturalRouteAccepted;
  public float logicalScale,standingHeight;public int paintedPoseCount,originalCanvasWidth,originalCanvasHeight,uniqueRGBAStorageCells;
  public int[] unionCrop;public double[] referenceAnchor,croppedAnchor;public Mine2ProductFileR1211 originalGeometryIdentity,actualIndependentDiagnostic;
  public PlainPagedLibraryPageR1330[] pages;public PlainPagedLibraryClipR1330[] clips;public PlainPagedLibraryFrameR1330[] logicalPoses;
  public PlainPagedLibraryClockR1330[] sourceClocks;public PlainPagedLibraryStateR1330[] stateBindings;
 }
 public sealed class PlainPagedLibraryPayloadR1330 {public PlainPagedLibraryBindingR1330 binding;public PlainPagedLibraryDocumentR1330 document;}
 public static class PlainPagedLibraryDataR1330 {
  public const string Schema="plain-original-uniform-paged-library-r1330",ClockSchema="plain-driller-original-uniform-clock-design-r1330";
  public const string ClockSHA="f65e3c0d3de7a64105c069723fefdf49965aca9735680c591201bc379e4dfb65",Npc="NUI_NASOD_DRILLER_CANNON",Kind="nasod-driller-cannon";
  static void Need(bool v,string why){PlainProductResourceIndexR1320.Need(v,"paged original Driller: "+why);}
  static bool Finite(double v){return!double.IsNaN(v)&&!double.IsInfinity(v);}
  static bool Pin(Mine2ProductFileR1211 p){return p!=null&&!string.IsNullOrEmpty(p.path)&&BelowPlayerResourceIndexV21.IsHash(p.sha256);}
  static bool SourcePose(PlainPagedLibraryFrameR1330 a,PlainPagedLibraryFrameR1330 b){return a!=null&&b!=null&&a.poseID==b.poseID&&a.animation==b.animation&&a.sourceTime==b.sourceTime&&a.sourceDuration==b.sourceDuration&&a.originalGeometryOffset==b.originalGeometryOffset;}
  static bool Frame(PlainPagedLibraryFrameR1330 a,PlainPagedLibraryFrameR1330 b){return SourcePose(a,b)&&a.pageKey==b.pageKey&&a.rgbaSha256==b.rgbaSha256&&a.uniqueGroupIndex==b.uniqueGroupIndex&&a.rect!=null&&b.rect!=null&&a.rect.Length==4&&b.rect.Length==4&&a.anchor!=null&&b.anchor!=null&&a.anchor.Length==2&&b.anchor.Length==2&&Array.TrueForAll(new[]{0,1,2,3},i=>a.rect[i]==b.rect[i])&&a.anchor[0]==b.anchor[0]&&a.anchor[1]==b.anchor[1];}
  public static PlainPagedLibraryPayloadR1330[] Read(PlainProductResourceIndexR1320 index,NpcDefinition[] rules,HashSet<string> seen){
   var rows=index.Document.pagedLibraryRows;if(rows==null||rows.Length==0)return new PlainPagedLibraryPayloadR1330[0];
   Need(index.Map.sourceDungeon==PlainProductResourceIndexR1320.Recycle&&rows.Length==1,"only one exact Recycle Driller binding");var binding=rows[0];
   Need(binding!=null&&binding.npc==Npc&&binding.id==Kind&&Pin(binding.document)&&!seen.Contains(Npc),"explicit original profile/library identity");
   var raw=index.Read(binding.document);Need(raw.Length<=4*1024*1024,"bounded paged metadata");var d=FactoryTypedJsonR581.ReadPlainPagedLibraryR1330(raw);
   Need(d!=null&&d.schema==Schema&&!d.sourceOnly&&d.dataOnly&&!d.pixelArtAccepted&&!d.nativeEquivalent&&!d.runtimeReleaseAccepted&&!d.naturalRouteAccepted&&d.id==Kind&&d.sourceNPC==Npc&&d.logicalScale==1&&d.paintedPoseCount==0&&Finite(d.standingHeight)&&d.standingHeight>0,"data-only original identity/scale");
   Need(d.sourceClockDesignSha256==ClockSHA&&d.sourceClockDesignJson!=null&&BelowPlayerResourceIndexV21.Hash(new System.Text.UTF8Encoding(false,true).GetBytes(d.sourceClockDesignJson))==ClockSHA,"exact original complete source clock design bytes");
   var source=FactoryTypedJsonR581.ReadPlainPagedLibraryR1330(new System.Text.UTF8Encoding(false,true).GetBytes(d.sourceClockDesignJson));
   Need(source!=null&&source.schema==ClockSchema&&source.sourceOnly&&source.dataOnly&&source.id==Kind&&source.sourceNPC==Npc&&source.sourceClocks!=null&&source.sourceClocks.Length==25&&source.stateBindings!=null&&source.stateBindings.Length==33&&source.logicalPoses!=null&&source.logicalPoses.Length==2173,"complete original25/33/2173 source contract");
   Need(Pin(d.originalGeometryIdentity)&&d.originalGeometryIdentity.sha256=="d8c6c7d76fc34058933613c1cd4c74bbf825ec8c262a3144f64479063bda80c3"&&Pin(d.actualIndependentDiagnostic)&&d.actualIndependentDiagnostic.sha256=="4a42e73f130ecfd4f43c41b0a643ab533b5f413808bf56414550704a69cf9b3e","actual source identity and independent diagnostic provenance");
   Need(d.textureRgbaHashDomain=="sha256-top-down-row-major-RGBA8-no-dimensions-prefix"&&d.originalCanvasWidth==512&&d.originalCanvasHeight==512&&d.uniqueRGBAStorageCells==1684&&d.pages!=null&&d.pages.Length==2&&d.clips!=null&&d.clips.Length==25&&d.logicalPoses!=null&&d.logicalPoses.Length==2173,"explicit two-page storage and unchanged logical cohort");
   Need(d.unionCrop!=null&&d.unionCrop.Length==4&&d.unionCrop[0]==116&&d.unionCrop[1]==204&&d.unionCrop[2]==305&&d.unionCrop[3]==244&&d.referenceAnchor!=null&&d.referenceAnchor.Length==2&&d.referenceAnchor[0]==256&&d.referenceAnchor[1]==372.8436510429362&&d.croppedAnchor!=null&&d.croppedAnchor.Length==2&&d.croppedAnchor[0]==140&&d.croppedAnchor[1]==372.8436510429362-204,"single measured union crop and source foot anchor");
   var pages=new Dictionary<string,PlainPagedLibraryPageR1330>(StringComparer.Ordinal);for(int i=0;i<2;i++){
    var page=d.pages[i];int height=i==0?8118:7872;Need(page!=null&&page.pageKey=="drill-page-"+i.ToString("000",System.Globalization.CultureInfo.InvariantCulture)&&Pin(page.file)&&BelowPlayerResourceIndexV21.IsHash(page.rgbaSha256)&&page.width==7982&&page.height==height&&page.width<=8192&&page.height<=8192,"bounded exact original raster page");
    var bytes=index.Read(page.file);Need(bytes.Length>=26&&bytes[0]==137&&bytes[1]==80&&bytes[2]==78&&bytes[3]==71&&bytes[12]==73&&bytes[13]==72&&bytes[14]==68&&bytes[15]==82&&PngInt(bytes,16)==page.width&&PngInt(bytes,20)==page.height&&bytes[24]==8&&bytes[25]==6,"actual original RGBA8 page dimensions");pages.Add(page.pageKey,page);
   }
   var poseIds=new Dictionary<string,PlainPagedLibraryFrameR1330>(StringComparer.Ordinal);var groups=new Dictionary<int,PlainPagedLibraryFrameR1330>();var motions=new Dictionary<string,List<PlainPagedLibraryFrameR1330>>(StringComparer.Ordinal);
   for(int i=0;i<2173;i++){
    var f=d.logicalPoses[i];var s=source.logicalPoses[i];Need(SourcePose(f,s)&&f.poseID=="pose-"+i.ToString("0000",System.Globalization.CultureInfo.InvariantCulture)&&Finite(f.sourceTime)&&Finite(f.sourceDuration)&&f.sourceTime>=0&&f.sourceTime<=f.sourceDuration&&f.sourceDuration>0&&BelowPlayerResourceIndexV21.IsHash(f.rgbaSha256)&&f.uniqueGroupIndex>=0&&f.uniqueGroupIndex<1684,"all ordered original logical poses and source-native times");
    int pageIndex=f.uniqueGroupIndex/858,cell=f.uniqueGroupIndex%858;string key="drill-page-"+pageIndex.ToString("000",System.Globalization.CultureInfo.InvariantCulture);Need(f.pageKey==key&&pages.ContainsKey(key)&&f.rect!=null&&f.rect.Length==4&&f.rect[0]==cell%26*307+1&&f.rect[1]==cell/26*246+1&&f.rect[2]==305&&f.rect[3]==244&&f.anchor!=null&&f.anchor.Length==2&&f.anchor[0]==d.croppedAnchor[0]&&f.anchor[1]==d.croppedAnchor[1],"exact alias group page/cell/one-pixel gutter and single anchor");
    PlainPagedLibraryFrameR1330 prior;if(groups.TryGetValue(f.uniqueGroupIndex,out prior))Need(prior.rgbaSha256==f.rgbaSha256&&prior.pageKey==f.pageKey&&prior.rect[0]==f.rect[0]&&prior.rect[1]==f.rect[1],"only identical measured RGBA groups may share cells");else groups.Add(f.uniqueGroupIndex,f);
    poseIds.Add(f.poseID,f);List<PlainPagedLibraryFrameR1330> motion;if(!motions.TryGetValue(f.animation,out motion)){motion=new List<PlainPagedLibraryFrameR1330>();motions.Add(f.animation,motion);}motion.Add(f);
   }
   Need(groups.Count==1684&&motions.Count==25&&!motions.ContainsKey("AttackB"),"all unique storage cells and original registered motion roster");var clipIds=new HashSet<string>(StringComparer.Ordinal);
   foreach(var clock in source.sourceClocks){
    Need(clock!=null&&!string.IsNullOrEmpty(clock.id)&&clipIds.Add(clock.id)&&Finite(clock.speed)&&clock.speed==1&&Finite(clock.nativeDuration)&&clock.nativeDuration>0&&clock.durationSeconds==clock.nativeDuration/clock.speed&&clock.steps>0,"original nonzero60Hz clip grid");
    List<PlainPagedLibraryFrameR1330> motion;Need(motions.TryGetValue(clock.animation,out motion)&&motion.Count==clock.steps+1,"every original temporal endpoint retained");
    for(int i=0;i<motion.Count;i++)Need(motion[i].sourceDuration==clock.nativeDuration&&motion[i].sourceTime==(i==clock.steps?clock.nativeDuration:clock.nativeDuration*i/clock.steps),"original native-time grid unchanged");
    var c=Array.Find(d.clips,x=>x!=null&&x.id==clock.id);int count=clock.loop?motion.Count-1:motion.Count;Need(c!=null&&c.fps==60&&c.durationSeconds==clock.durationSeconds&&c.loop==clock.loop&&c.frames!=null&&c.frames.Length==count,"fps/duration/speed/loop/endpoints unchanged");for(int i=0;i<count;i++)Need(Frame(c.frames[i],motion[i]),"every clip frame resolves its exact original page/pose");
   }
   var profile=Array.Find(rules,x=>x.id==Npc);Need(profile!=null&&profile.kind==Kind&&profile.states!=null&&profile.states.Length==33,"exact original Driller profile");var stateIds=new HashSet<string>(StringComparer.Ordinal);
   var rawClocks=FactoryTypedJsonR581.ReadMine2ProductRootR1226<Mine2ProductSourceClocksR1211>(index.RootBytes("rules"));var rawProfile=rawClocks==null||rawClocks.definitions==null?null:Array.Find(rawClocks.definitions,x=>x!=null&&x.id==Npc);
   Need(rawProfile!=null&&rawProfile.states!=null&&rawProfile.states.Length==33,"exact original Driller source motion clocks");
   foreach(var b in source.stateBindings){var s=Array.Find(profile.states,x=>x.id==b.state);var rawState=Array.Find(rawProfile.states,x=>x!=null&&x.id==b.state);Need(b!=null&&stateIds.Add(b.state)&&s!=null&&s.clip==b.modelClip&&s.sourceAnimation==b.originalSourceAnimation&&s.sourceAnimSpeed==(float)b.originalSourceAnimSpeed&&rawState!=null&&rawState.sourceMotionDuration==(float)b.originalMotionDuration&&s.duration==(float)b.originalStateDuration&&s.loop==b.originalLoop&&s.nextState==b.originalNextState&&clipIds.Contains(s.clip),"all33 state display bindings and original clocks");}
   seen.Add(Npc);return new[]{new PlainPagedLibraryPayloadR1330{binding=binding,document=d}};
  }
  static int PngInt(byte[] b,int i){return checked((int)((uint)b[i]<<24|(uint)b[i+1]<<16|(uint)b[i+2]<<8|b[i+3]));}
 }

}
