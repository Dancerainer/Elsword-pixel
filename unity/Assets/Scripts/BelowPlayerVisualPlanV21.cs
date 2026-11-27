using System;
using System.Collections.Generic;
using System.Text;
namespace ElswordPixel {
 public sealed class BelowPlayerVisualPlanV21 {
  public BelowVisualBindingV21 binding;public BelowVisualDocumentV21 document;public OutskirtsNpcMeshManifest meshes;public BelowVisualInspectionV21 inspection;
  public static BelowPlayerVisualPlanV21 Read(BelowPlayerDataBridgeV21 data){
   var pin=data.VisualBinding;var b=data.ReadJson<BelowVisualBindingV21>(pin);
   Need(b!=null&&b.schema=="below-source-visual-binding-v21"&&!b.runtimeAdmitted&&!b.actualUnityVerified&&!b.nativeEquivalent&&b.textures!=null&&b.declaredHolds!=null,"Binding scope");
   var d=data.ReadJson<BelowVisualDocumentV21>(File(b.document));var effectBytes=data.Read(File(b.sourceEffects));var rulesBytes=data.Read(File(b.sourceRules));var effects=data.Parse<NpcEffectDocument>(effectBytes);
   Need(d!=null&&d.schema=="below-source-visual-v21"&&!d.runtimeAdmitted&&!d.actualUnityVerified&&d.unit==.28f&&d.sourceRulesSha256==BelowPlayerResourceIndexV21.Hash(rulesBytes)&&d.sourceEffectsSha256==BelowPlayerResourceIndexV21.Hash(effectBytes),"Source rules/effects");
   Need(d.sourceRulesSha256=="479a89b87a2e69732532ce978e3802d0047b434f2efd17785ebdc1c093233dcb"&&d.sourceEffectsSha256=="89e0d3e4a89d5850fd546948e2b2cf08ee2350d7503044684cbd7060c1f1fa42","Frozen source hashes");
   Need(d.definitions!=null&&d.definitions.Length==11&&d.particles!=null&&d.particles.Length==17&&d.effectParticles!=null&&d.effectParticles.Length==52&&d.effectPassiveParticles!=null&&d.effectPassiveParticles.Length==2&&d.effectMeshes!=null&&d.effectMeshes.Length==6&&effects.definitions.Length==11,"Typed closure");
   DemoScene.ValidateBelowFiniteTailDocumentR294(d);
   var exact=new HashSet<string>(StringComparer.Ordinal);foreach(var source in effects.definitions)Need(exact.Add(source.id),"Duplicate source effect");
   foreach(var r in d.definitions){Need(exact.Remove(r.id),"Unbound typed effect");var source=Array.Find(effects.definitions,q=>q.id==r.id);Need(source.lifetime==r.lifetime&&source.visualFromMesh==r.sourceVisibleMesh&&r.sourceVisibleMesh==r.mainMeshRequired&&r.sourceDrawCount==(r.mainMeshRequired?1:0)&&string.IsNullOrEmpty(source.visualId),"Identity/lifetime/draw count changed");int main=0,fields=0;foreach(var mesh in d.effectMeshes)if(mesh.effect==r.id)main++;foreach(var f in d.effectParticles)if(f.effect==r.id)fields++;Need(main==(r.mainMeshRequired?1:0)&&fields>0,"Missing typed mesh/field lifecycle");}Need(exact.Count==0,"Missing source effects");
   var meshes=data.ReadJson<OutskirtsNpcMeshManifest>(File(b.meshManifest));Need(meshes!=null&&meshes.components!=null&&meshes.components.Length==9,"Mesh components");
   var keys=new HashSet<string>(StringComparer.Ordinal);foreach(var t in b.textures){Need(t!=null&&!string.IsNullOrEmpty(t.key)&&keys.Add(t.key),"Duplicate texture binding");data.Read(File(t));}
   foreach(var mesh in meshes.components){Need(mesh!=null&&mesh.fps==60&&mesh.durationSeconds>0&&mesh.pixelsPerNativeUnit>0&&mesh.frames!=null&&mesh.frames.Length>0,"Original mesh clock");foreach(var f in mesh.frames)Need(f.rect!=null&&f.rect.Length==4&&f.anchor!=null&&f.anchor.Length==2&&keys.Contains("components/"+f.atlas),"Mesh texture/rect");}
   var templates=new HashSet<string>(StringComparer.Ordinal);int heldTemplates=0,supported=0;foreach(var p in d.particles){Need(p!=null&&templates.Add(p.key)&&BelowPlayerResourceIndexV21.Hash(Encoding.UTF8.GetBytes(p.particleJson??""))==p.particleJsonSha256,"Normalized original particle");if(!p.supported){Need(!string.IsNullOrEmpty(p.holdReason),"Template HOLD reason");heldTemplates++;}else if(string.IsNullOrEmpty(p.mesh))Need(keys.Contains(p.textureKey),"Particle texture");else Need(Array.Exists(meshes.components,m=>m.id==p.mesh),"Particle mesh");}
   var holds=new List<BelowVisualHoldV21>();foreach(var f in d.effectParticles){var p=Array.Find(d.particles,q=>q.key==f.particleKey);Need(p!=null&&f.count>0&&(f.eventType=="spawn"||f.eventType=="hit"),"Typed START/HIT template");if(f.supported&&p.supported)supported++;else holds.Add(new BelowVisualHoldV21{effect=f.effect,sourceField=f.sourceField,particleKey=f.particleKey,reason=!f.supported?f.holdReason:p.holdReason});}
   foreach(var f in d.effectPassiveParticles){var p=Array.Find(d.particles,q=>q.key==f.particleKey);Need(p!=null,"Passive template");if(p.supported)supported++;else holds.Add(new BelowVisualHoldV21{effect=f.effect,sourceField=f.sourceField,particleKey=f.particleKey,reason=p.holdReason});}
   var declared=new HashSet<string>(StringComparer.Ordinal);foreach(var h in b.declaredHolds)Need(h!=null&&!string.IsNullOrEmpty(h.reason)&&declared.Add(Key(h)),"Ambiguous HOLD");foreach(var h in holds)Need(declared.Remove(Key(h)),"Unreported HOLD");Need(declared.Count==0,"Obsolete HOLD");
   return new BelowPlayerVisualPlanV21{binding=b,document=d,meshes=meshes,inspection=new BelowVisualInspectionV21{bindingSha256=pin.sha256,completeDefinitionCoverage=true,allHoldsExplicit=true,sourceDefinitionsUnchanged=true,definitions=11,mainMeshes=6,particleTemplates=17,effectFields=52,passives=2,supportedFields=supported,heldFields=holds.Count,heldTemplates=heldTemplates,holds=holds.ToArray()}};
  }
  static string Key(BelowVisualHoldV21 h){return h.effect+"/"+h.sourceField+"/"+h.particleKey+"/"+h.reason;}
  public static BelowPlayerFileV21 File(BelowVisualFilePinV21 p){Need(p!=null,"Missing pin");return new BelowPlayerFileV21{path=p.path,sha256=p.sha256};}
  public static BelowPlayerFileV21 File(BelowVisualTexturePinV21 p){Need(p!=null,"Missing texture pin");return new BelowPlayerFileV21{path=p.path,sha256=p.sha256};}
  static void Need(bool ok,string reason){BelowPlayerResourceIndexV21.Need(ok,"Resources typed FX: "+reason);}
 }
 [Serializable] public sealed class BelowPlayerAtlasV21 {public int frameWidth,frameHeight;public float[] groundAnchor;}
 [Serializable] public sealed class BelowPlayerFrameV21 {public int[] rect;public float[] anchor;}
 [Serializable] public sealed class BelowPlayerClipV21 {public string id;public float fps,durationSeconds;public bool loop;public BelowPlayerFrameV21[] frames;}
 [Serializable] public sealed class BelowPlayerLibraryV21 {public string id,sourceNPC,artVersion,sourceRulesSha256;public bool runtimeAdmitted,animationLibraryApproved;public float logicalScale,standingHeight;public int paintedPoseCount;public BelowPlayerAtlasV21 atlas;public BelowPlayerClipV21[] clips;}
 public static class BelowPlayerCatalogProjectionV21 {
  // Session-local proposal only. Existing admission objects/arrays are never modified.
  public static ArtCatalog AppendCandidate(ArtCatalog original,BelowPlayerDataPlanV21 plan){
   BelowPlayerResourceIndexV21.Need(original!=null&&original.dungeons.Length==12&&original.stages.Length==35&&original.npcContentSets.Length==8&&plan!=null&&plan.rooms.Length==3&&plan.route.start==35,"Formal12 append boundary");
   foreach(var route in original.dungeons)BelowPlayerResourceIndexV21.Need(route.sourceGraph!=BelowPlayerResourceIndexV21.SourceDungeon,"Below already present");
   var routes=new List<DungeonArt>(original.dungeons);routes.Add(plan.route);var stages=new List<StageArt>(original.stages);stages.AddRange(plan.stages);var sets=new List<NpcContentSetData>(original.npcContentSets);sets.Add(new NpcContentSetData{sourceDungeon=BelowPlayerResourceIndexV21.SourceDungeon,npcRules=plan.rules,npcEffectRules=plan.effects,npcSoundRules=plan.sounds,npcMechanismRules=plan.mechanisms,admission=new IterationAdmissionData{schema="below-player-candidate-admission-v21-r2",sourceDungeon=BelowPlayerResourceIndexV21.SourceDungeon,bundleSha256=BelowPlayerResourceIndexV21.ApprovedBundle,reviewed=false,residualNotes=new[]{"Player native load/render/audio/End proof pending","corrected natural route and formal whitelist/build gates pending"}}});
   return new ArtCatalog{hero=original.hero,monsters=original.monsters,effects=original.effects,stages=stages.ToArray(),dungeons=routes.ToArray(),icons=original.icons,npcRules=original.npcRules,npcEffectRules=original.npcEffectRules,npcSoundRules=original.npcSoundRules,npcMechanismRules=original.npcMechanismRules,npcSounds=original.npcSounds,iterationAdmission=original.iterationAdmission,npcContentSets=sets.ToArray()};
  }
 }
}
