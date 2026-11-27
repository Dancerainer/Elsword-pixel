using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cargo=ElswordPixel.CargoPresentationV21;
using CargoClock=ElswordPixel.CargoWallyDirectorDraftV21;
using CargoView=ElswordPixel.CargoDirectorControlledPrivateV21.View;
namespace ElswordPixel {
 public sealed class CargoProductTextureInputR1261 {public string sourcePath,sha256;}
 public sealed class CargoProductWorldInputR1261 {public string id,scene,projection,geometry;public CargoProductTextureInputR1261[] textures;}
 public sealed class CargoProductLibraryInputR1261 {public string id,npc,document,atlas;}
 public sealed class CargoProductMaterialInputR1261 {public string npc,indexControl,paletteLUT,mapping;}
 public sealed class CargoProductWorldDataR1261 {public CargoProductWorldInputR1261 input;public CargoProductSceneR1261 scene;public CargoProductProjectionR1261 projection;public string musicId,musicResource,musicSourceSha256;public byte[] originalScene,geometry;public Dictionary<string,byte[]> textures;}
 public sealed class CargoProductLibraryDataR1261 {public CargoProductLibraryInputR1261 input;public CargoProductLibraryR1261 document;public byte[] atlas;public int width,height;}
 public sealed class CargoProductMaterialDataR1261 {public CargoProductMaterialInputR1261 input;public byte[] indexControl,paletteLUT,mapping;}
 // Pre-read data only. The caller supplies original bytes by content SHA;
 // there is no filesystem, Resources, Unity object, GameModel or actor access.
 // Before-AI, original audit and activation candidate remain separate values.
 public sealed class CargoProductDataPlanR1261 {
  public CargoProductModelR1261 modelBeforeAI,originalModelAudit,activationCandidate;
  public CargoProductAuditR1261 sourceAudit;public CargoProductRulesR1261 rules;
  public NpcEffectDocument effects;public NpcSoundDocument sounds;public NpcMechanismDocument mechanisms;
  public CargoProductStateClipsR1261 stateClips;public CargoClock.ClockInputV21 directorClock;public CargoView.DirectorViewSpecV21 directorView;
  public Cargo.CargoVisualDocumentV21 visual;public Cargo.OutskirtsNpcMeshManifest visualMeshes;public Cargo.CargoAudioDocumentV21 audioContract;
  public CargoProductWorldDataR1261[] worlds;public CargoProductLibraryDataR1261[] libraries;public CargoProductMaterialDataR1261[] AIRMaterials;
  public byte[] originalBeforeAIJson,originalAuditJson,originalActivationCandidateJson,originalRulesJson,originalSoundJson,originalMechanismsJson;
  public bool[][] sourceActive,candidateAI;public int sourceActiveBirths,activationCandidateEnabledBirths,maximumAtlasDimension;
  public Dictionary<string,Cargo.CargoAudioAssetV21> originalAudioBindings;public Dictionary<string,byte[]> originalFxTextures;public byte[][] originalDirectorAtlases;
  public readonly string[] unresolvedBoundaries={
   "Explicit demo ground and own-source air display adapters are available; original physicalUnboundPolicyReady=false and unclaimed native directional-physics parity remain separate.",
   "Five original BossPost named motions remain unbound; a separate exact-held-identity 2D static-rest presentation uses its own frame without an invented NPC profile, state or animation alias.",
   "Four declared original FX fields remain HOLD; no particle/bone/lifetime substitute is selected.",
   "Original Wally9Armor_ChargeBallShot.ogg is missing; its unresolved SOUND0 remains recorded and has no synthetic replacement.",
   "The existing author key1 mapping for the original disabled director is an explicit 2D adapter, not a claimed native disabled-state ABI."
  };
  public bool ActivationApplied {get{return false;}}
  public bool OrdinaryHostAccepted {get{return false;}}
  public bool PCMResourcesBound {get{return false;}}
  public bool FullNaturalRouteVerified {get{return false;}}
  static void Need(bool value,string reason){if(!value)throw new InvalidOperationException("Cargo original data: "+reason);}
  static string Hash(byte[] raw){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(raw)).Replace("-","").ToLowerInvariant();}
  static byte[] Read(Func<string,byte[]> load,string sha){Need(load!=null&&sha!=null&&sha.Length==64,"original content binding absent");var raw=load(sha);Need(raw!=null&&raw.Length>0&&raw.Length<=64*1024*1024&&Hash(raw)==sha,"original payload absent, oversized or changed "+sha);return raw;}
  static T Json<T>(Func<string,byte[]> load,string sha){return CargoProductTypedJsonR1261.Read<T>(Read(load,sha));}
  static int U32(byte[] b,int i){return checked((int)((uint)b[i]<<24|(uint)b[i+1]<<16|(uint)b[i+2]<<8|b[i+3]));}
  static void Png(byte[] b,out int w,out int h){Need(b.Length>=24&&b[0]==137&&b[1]==80&&b[2]==78&&b[3]==71&&b[12]==73&&b[13]==72&&b[14]==68&&b[15]==82,"original PNG header absent");w=U32(b,16);h=U32(b,20);Need(w>0&&h>0,"invalid original atlas dimensions");}
  static bool EarlyCombat(string npc){return npc=="NUI_BEE_NASOD"||npc=="NUI_NASOD_HEALER"||npc=="NUI_NASOD_MINER_THIN_AIR"||npc=="NUI_NASOD_MINER_FAT_B_AIR"||npc=="NUI_NASOD_MINER_FAT_C_AIR";}
  public static CargoProductDataPlanR1261 Build(Func<string,byte[]> load){
   var p=new CargoProductDataPlanR1261();
   p.originalBeforeAIJson=Read(load,CargoProductInputBindingsR1261.ModelBeforeAI);p.originalAuditJson=Read(load,CargoProductInputBindingsR1261.OriginalModelAudit);p.originalActivationCandidateJson=Read(load,CargoProductInputBindingsR1261.ActivationCandidate);
   p.modelBeforeAI=CargoProductTypedJsonR1261.Read<CargoProductModelR1261>(p.originalBeforeAIJson);p.originalModelAudit=CargoProductTypedJsonR1261.Read<CargoProductModelR1261>(p.originalAuditJson);p.activationCandidate=CargoProductTypedJsonR1261.Read<CargoProductModelR1261>(p.originalActivationCandidateJson);p.sourceAudit=CargoProductTypedJsonR1261.Read<CargoProductAuditR1261>(p.originalBeforeAIJson);
   p.originalRulesJson=Read(load,CargoProductInputBindingsR1261.Rules);p.rules=CargoProductTypedJsonR1261.Read<CargoProductRulesR1261>(p.originalRulesJson);p.effects=Json<NpcEffectDocument>(load,CargoProductInputBindingsR1261.Effects);p.originalSoundJson=Read(load,CargoProductInputBindingsR1261.Sounds);p.sounds=CargoProductTypedJsonR1261.Read<NpcSoundDocument>(p.originalSoundJson);p.originalMechanismsJson=Read(load,CargoProductInputBindingsR1261.Mechanisms);p.mechanisms=CargoProductTypedJsonR1261.Read<NpcMechanismDocument>(p.originalMechanismsJson);
   p.stateClips=Json<CargoProductStateClipsR1261>(load,CargoProductInputBindingsR1261.StateClips);p.directorClock=Json<CargoClock.ClockInputV21>(load,CargoProductInputBindingsR1261.DirectorClock);p.directorView=Json<CargoView.DirectorViewSpecV21>(load,CargoProductInputBindingsR1261.DirectorView);p.audioContract=Json<Cargo.CargoAudioDocumentV21>(load,CargoProductInputBindingsR1261.AudioContract);p.visual=Json<Cargo.CargoVisualDocumentV21>(load,CargoProductInputBindingsR1261.VisualDocument);p.visualMeshes=Json<Cargo.OutskirtsNpcMeshManifest>(load,CargoProductInputBindingsR1261.VisualMeshes);
   foreach(var m in new[]{p.modelBeforeAI,p.originalModelAudit,p.activationCandidate})Need(m!=null&&m.sourceGraph=="Dungeon_Besma_Air_Ship_Normal.lua"&&!m.runtimeAdmitted&&m.rooms!=null&&m.rooms.Length==3,"original three-room graph changed");
   Need(p.sourceAudit!=null&&p.sourceAudit.rooms!=null&&p.sourceAudit.rooms.Length==3&&p.rules!=null&&!p.rules.runtimeAdmitted&&!p.rules.sourceMechanismsComplete&&p.rules.definitions!=null&&p.rules.definitions.Length==10,"original source audit/rule profiles changed");
   Need(p.effects!=null&&p.effects.definitions.Length==10&&p.sounds!=null&&p.sounds.assets.Length==64&&p.mechanisms!=null&&p.mechanisms.sourceCargoV21!=null&&p.mechanisms.sourceCargoDirectorV21!=null,"original effect/audio/source mechanisms incomplete");
   Need(p.stateClips!=null&&p.stateClips.sourceStates==171&&p.stateClips.rows!=null&&p.stateClips.rows.Length==171&&!p.stateClips.physicalUnboundPolicyReady,"original state table or its unresolved physical boundary changed");
   string[] ids={"NUI_BEE_NASOD","NUI_BOX_BESMA2","NUI_NASOD_HEALER","NUI_NASOD_MINER_FAT_B_AIR","NUI_NASOD_MINER_FAT_C_AIR","NUI_NASOD_MINER_THIN_AIR","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_BOSS_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP","NUI_WALLY_9TH_ARMOR"};int[] states={23,5,23,26,28,26,4,4,5,27};
   for(int i=0;i<10;i++)Need(p.rules.definitions[i]!=null&&p.rules.definitions[i].id==ids[i]&&p.rules.definitions[i].states.Length==states[i],"original finite rule/state identity changed");
   int births=0,supports=0,zones=0,parts=0;int[] counts={33,32,2};p.sourceActive=new bool[3][];p.candidateAI=new bool[3][];p.worlds=new CargoProductWorldDataR1261[3];
   for(int i=0;i<3;i++){
    var r=p.modelBeforeAI.rooms[i];var o=p.originalModelAudit.rooms[i];var a=p.activationCandidate.rooms[i];var source=p.sourceAudit.rooms[i];var input=CargoProductInputBindingsR1261.Worlds[i];
    Need(r.id=="Dungeon_Besma_Air_Ship_Normal-"+i&&r.sourceWorldId==input.id&&r.enemies!=null&&r.enemies.Length==counts[i]&&o.enemies.Length==counts[i]&&a.enemies.Length==counts[i]&&source.enemies.Length==counts[i]&&r.surfaces!=null&&r.encounters!=null&&r.bgm!=null&&r.bgm.Length==1&&r.bgm[0]==CargoProductInputBindingsR1261.OriginalRoomMusicNames[i],"source room/population/music changed");
    p.sourceActive[i]=new bool[counts[i]];p.candidateAI[i]=new bool[counts[i]];
    for(int j=0;j<counts[i];j++){
     var e=r.enemies[j];var original=o.enemies[j];var candidate=a.enemies[j];var audit=source.enemies[j];Need(e!=null&&original!=null&&candidate!=null&&audit!=null&&e.sourceNPC==original.sourceNPC&&e.sourceNPC==candidate.sourceNPC&&e.sourceNPC==audit.sourceNPC&&!e.aiEnabled&&!original.aiEnabled&&!audit.aiEnabled,"before-AI/audit identity or stopped AI changed");
     bool expected=i<2&&audit.sourceActive&&EarlyCombat(e.sourceNPC);Need(candidate.aiEnabled==expected&&e.hp==candidate.hp&&e.x==candidate.x&&e.y==candidate.y&&e.facing==candidate.facing&&e.sourceKeyCode==candidate.sourceKeyCode,"activation candidate changed non-AI birth contract");p.sourceActive[i][j]=audit.sourceActive;p.candidateAI[i][j]=candidate.aiEnabled;if(audit.sourceActive)p.sourceActiveBirths++;if(candidate.aiEnabled)p.activationCandidateEnabledBirths++;births++;
    }
    var raw=Read(load,input.scene);var scene=CargoProductTypedJsonR1261.Read<CargoProductSceneR1261>(raw);var projection=Json<CargoProductProjectionR1261>(load,input.projection);var geometry=Read(load,input.geometry);var textures=new Dictionary<string,byte[]>(StringComparer.Ordinal);
    foreach(var t in input.textures){Need(t!=null&&!textures.ContainsKey(t.sourcePath),"duplicate original scene texture key");textures.Add(t.sourcePath,Read(load,t.sha256));}
    Need(scene!=null&&scene.id==input.id&&scene.parts!=null&&projection!=null&&projection.id==input.id&&projection.unit==.28f&&projection.width==r.width&&projection.path!=null&&projection.path.Length>0,"original scene/projection changed");
    foreach(var path in projection.path)Need(path!=null&&path.start!=null&&path.start.Length==3&&path.end!=null&&path.end.Length==3,"original source path endpoint shape changed");
    foreach(var part in scene.parts){Need(part!=null&&part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=geometry.LongLength&&(string.IsNullOrEmpty(part.texture)||textures.ContainsKey(part.texture)),"original geometry range/texture missing");parts++;}
    p.worlds[i]=new CargoProductWorldDataR1261{input=input,scene=scene,projection=projection,musicId=CargoProductInputBindingsR1261.RoomMusicIds[i],musicResource=CargoProductInputBindingsR1261.RoomMusicResources[i],musicSourceSha256=CargoProductInputBindingsR1261.RoomMusicSourceSha[i],originalScene=raw,geometry=geometry,textures=textures};supports+=r.surfaces.Length;zones+=r.encounters.Length;
   }
   Need(births==67&&supports==155&&zones==5&&parts==1053&&p.sourceActiveBirths==58&&p.activationCandidateEnabledBirths==57,"original finite scene/population/activation totals changed");
   Need(p.modelBeforeAI.rooms[2].enemies[0].sourceNPC=="NUI_WALLY_9TH_ARMOR"&&!p.activationCandidate.rooms[2].enemies[0].aiEnabled&&p.modelBeforeAI.rooms[2].enemies[1].sourceNPC=="NUI_EVENT_NPC_WALLY_9TH_ARMOR"&&p.modelBeforeAI.rooms[2].enemies[1].sourceKeyCode==1&&!p.activationCandidate.rooms[2].enemies[1].aiEnabled,"original separate director author-key/BossSTOP contract changed");
   p.libraries=new CargoProductLibraryDataR1261[11];var byNpc=new Dictionary<string,CargoProductLibraryR1261>(StringComparer.Ordinal);
   for(int i=0;i<11;i++){
    var input=CargoProductInputBindingsR1261.Libraries[i];var d=Json<CargoProductLibraryR1261>(load,input.document);var atlas=Read(load,input.atlas);int w,h;Png(atlas,out w,out h);Need(d!=null&&d.id==input.id&&d.sourceNPC==input.npc&&d.logicalScale==1&&d.atlas!=null&&d.clips!=null&&d.clips.Length>0&&!byNpc.ContainsKey(input.npc),"original library binding changed");
    var names=new HashSet<string>(StringComparer.Ordinal);foreach(var clip in d.clips){Need(clip!=null&&names.Add(clip.id)&&clip.frames!=null&&clip.frames.Length>0&&clip.durationSeconds>0,"original clip missing; no idle fallback");foreach(var frame in clip.frames)Need(frame!=null&&frame.rect!=null&&frame.rect.Length==4&&frame.anchor!=null&&frame.anchor.Length==2&&frame.rect[0]>=0&&frame.rect[1]>=0&&frame.rect[2]>0&&frame.rect[3]>0&&(long)frame.rect[0]+frame.rect[2]<=w&&(long)frame.rect[1]+frame.rect[3]<=h,"original signed anchor/atlas rectangle changed");}
    byNpc.Add(input.npc,d);p.maximumAtlasDimension=Math.Max(p.maximumAtlasDimension,Math.Max(w,h));p.libraries[i]=new CargoProductLibraryDataR1261{input=input,document=d,atlas=atlas,width=w,height=h};
   }
   var seen=new HashSet<string>(StringComparer.Ordinal);foreach(var clip in p.stateClips.rows){Need(clip!=null&&seen.Add(clip.npc+"/"+clip.state)&&clip.sourceStateClockUnchanged,"source display clip identity changed");var profile=Array.Find(p.rules.definitions,r=>r.id==clip.npc);var state=profile==null?null:Array.Find(profile.states,s=>s.id==clip.state);CargoProductLibraryR1261 library;Need(state!=null&&profile.kind==clip.kind&&state.clip==clip.compiledClip&&state.sourceAnimation==clip.sourceAnimation&&state.duration==clip.sourceStateDuration&&state.sourceAnimSpeed==clip.sourceAnimSpeed&&byNpc.TryGetValue(clip.npc,out library),"compiled source clock/clip changed");library=byNpc[clip.npc];var display=Array.Find(library.clips,c=>c.id==clip.displayClip);Need(display!=null&&display.durationSeconds==clip.displayDuration&&display.loop==clip.displayLoop,"original explicit display clip absent; no idle fallback");}
   p.AIRMaterials=new CargoProductMaterialDataR1261[3];for(int i=0;i<3;i++){var input=CargoProductInputBindingsR1261.AIRMaterials[i];Need(byNpc.ContainsKey(input.npc),"AIR material identity absent");p.AIRMaterials[i]=new CargoProductMaterialDataR1261{input=input,indexControl=Read(load,input.indexControl),paletteLUT=Read(load,input.paletteLUT),mapping=Read(load,input.mapping)};}
   Need(p.directorClock!=null&&p.directorClock.sourceNPC=="NUI_EVENT_NPC_WALLY_9TH_ARMOR"&&p.directorClock.cameraFrames.Length==791&&p.directorView!=null&&p.directorView.cameraFrames.Length==791&&p.directorView.poses.Length==9&&p.directorView.assets.Length==2,"original director data incomplete");
   Need(p.visual!=null&&p.visual.schema=="cargo-source-visual-v21"&&p.visual.particles.Length==42&&p.visual.effectParticles.Length==46&&p.visual.effectMeshes.Length==2&&p.visual.effectPassiveParticles.Length==2&&p.visualMeshes!=null&&p.visualMeshes.components.Length==6,"original Cargo FX data incomplete");
   foreach(var particle in p.visual.particles)Need(particle!=null&&Hash(new UTF8Encoding(false,true).GetBytes(particle.particleJson??""))==particle.particleJsonSha256,"normalized original particle blob changed");
   p.originalFxTextures=new Dictionary<string,byte[]>(StringComparer.Ordinal);foreach(var texture in CargoProductInputBindingsR1261.FxTextures){Need(!p.originalFxTextures.ContainsKey(texture.sourcePath),"duplicate original FX texture key");p.originalFxTextures.Add(texture.sourcePath,Read(load,texture.sha256));}
   p.originalDirectorAtlases=new byte[p.directorView.assets.Length][];for(int i=0;i<p.originalDirectorAtlases.Length;i++)p.originalDirectorAtlases[i]=Read(load,p.directorView.assets[i].sha256);
   Need(p.audioContract!=null&&p.audioContract.schema=="cargo-exact-original-audio-v21"&&p.audioContract.assets.Length==64&&p.audioContract.sourceRulesSha256==CargoProductInputBindingsR1261.Rules&&p.audioContract.sourceSoundsSha256==CargoProductInputBindingsR1261.Sounds,"original64 audio container contract changed");
   // Original OGG archive/cache paths remain inert provenance fields. The
   // ordinary Host must use the separately prepared PCM Resources contract;
   // source OGG bytes and the old file-decoding AudioRuntime are not loaded.
   p.originalAudioBindings=new Dictionary<string,Cargo.CargoAudioAssetV21>(StringComparer.Ordinal);foreach(var asset in p.audioContract.assets){Need(asset!=null&&asset.file!=null&&asset.sha256==asset.file.sha256&&asset.sha256.Length==64&&!p.originalAudioBindings.ContainsKey(asset.id),"original audio binding changed");p.originalAudioBindings.Add(asset.id,asset);}
   return p;
  }
 }
}
