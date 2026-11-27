#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool dragonRoadPreviewSession;
  int dragonRoadLandingDiagnosticFrames;
  readonly HashSet<int> dragonRoadDiagnosticLandingSerials=new HashSet<int>();
  public bool DragonRoadPreviewActive {get{return dragonRoadPreviewSession&&forestPreviewActive;}}
  public int DragonRoadLandingDiagnosticFrames {get{return dragonRoadLandingDiagnosticFrames;}}
  [Serializable] sealed class DragonRoadArtBinding {public string sourceRulesSha256,effectsSha256;public bool runtimeAdmitted,internalPlaytestArtAccepted;public PreviewFile review,candidateManifest;}
  [Serializable] sealed class DragonRoadArtApproval {public string schema,reviewer,producer;public bool runtimeAdmitted,internalPlaytestArtAccepted,acceptedForPlaytest;public PreviewFile manifest;public DragonRoadReviewedRow[] rows;}
  [Serializable] sealed class DragonRoadReviewedRow {public string id,sourceNPC;public bool internalPlaytestArtAccepted,acceptedForPlaytest;public PreviewFile animations,atlas;}
  [Serializable] sealed class DragonRoadSourceVisualReview {public string schema,reviewer,producer,bindingSha256;public bool acceptedForPlaytest;public string[] blockingIssues;}
  void LoadDragonRoadSourceVisual(PreviewBundle bundle){
   var binding=PreviewJson<PreviewOutskirtsVisual>(bundle.outskirtsVisual);
   PreviewRequire(binding.schema=="dragon-road-visual-binding-v17"&&!binding.runtimeAdmitted&&binding.textures.Length==6,"Dragon Road source visual binding changed");
   DragonRoadSourceVisualReview approval=null;
   foreach(var file in bundle.reviewEvidence){var candidate=PreviewJson<DragonRoadSourceVisualReview>(file);if(candidate!=null&&candidate.schema=="dragon-road-source-visual-review-v17")approval=candidate;}
   PreviewRequire(approval!=null&&approval.reviewer=="art_auditor"&&approval.producer!=approval.reviewer&&approval.acceptedForPlaytest&&approval.bindingSha256==bundle.outskirtsVisual.sha256&&approval.blockingIssues!=null&&approval.blockingIssues.Length==0,"Dragon Road original effect presentation lacks independent approval");
   var document=PreviewJson<OutskirtsNpcVisualDocument>(binding.document);
   PreviewRequire(document.sourceRulesSha256==bundle.rules.sha256&&document.sourceEffectsSha256==bundle.effects.sha256,"Dragon Road source visual clocks changed");
   var meshes=PreviewJson<OutskirtsNpcMeshManifest>(binding.meshManifest);var textures=new Dictionary<string,Texture2D>();
   foreach(var texture in binding.textures){PreviewRequire(!string.IsNullOrEmpty(texture.key)&&!textures.ContainsKey(texture.key),"duplicate Dragon Road source visual texture");textures.Add(texture.key,PreviewTextureFile(new PreviewFile{path=texture.path,sha256=texture.sha256}));}
   foreach(var evidence in binding.evidence)PreviewRead(evidence);
   ConfigureOutskirtsNpcVisual(document,meshes,textures);
  }
  static readonly string[] DragonRoadWorlds={"WI_NEW_BESMA_DRAGON_ROAD_STAGE1","WI_NEW_BESMA_DRAGON_ROAD_STAGE2A","WI_NEW_BESMA_DRAGON_ROAD_STAGE4"};
  public static void ValidateDragonRoadPreviewEntry(string path){
   var b=JsonUtility.FromJson<PreviewBundle>(File.ReadAllText(path));
   PreviewRequire(b!=null&&b.schema=="dragon-road-editor-preview-v17"&&b.sourceDungeon=="Dungeon_Besma_Dragon_Road_Normal"&&b.editorOnly&&!b.runtimeAdmitted&&b.playableReady&&(b.blockers==null||b.blockers.Length==0),"Dragon Road bundle not ready");
  }
  public void BeginDragonRoadPreview(string path){ValidateDragonRoadPreviewEntry(path);BeginSourceDungeonPreview(path,false,false,false,false,true);}
  IEnumerator LoadDragonRoadPreview(){
   ValidateDragonRoadPreviewEntry(forestPreviewBundlePath);
   var bytes=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(bytes);
   var b=JsonUtility.FromJson<PreviewBundle>(Encoding.UTF8.GetString(bytes));
   PreviewRequire(b.worlds.Length==3&&b.sprites.Length+b.reusedAdmittedVisuals.Length==10&&b.originalEntityCount==44,"Dragon Road source scope changed");
   PreviewRead(b.admittedCatalog);PreviewRequire(PreviewHash(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes)==b.admittedCatalog.sha256,"Dragon Road preview catalog baseline changed");
   foreach(var f in b.reviewEvidence)PreviewRead(f);PreviewRead(b.soundMap);PreviewRead(b.materialOverrides);
   var art=PreviewJson<DragonRoadArtBinding>(b.artManifest);
   PreviewRequire(art.sourceRulesSha256==b.rules.sha256&&art.effectsSha256==b.effects.sha256&&!art.runtimeAdmitted&&art.internalPlaytestArtAccepted,"Dragon Road art binding changed");
   PreviewRead(art.candidateManifest);var review=PreviewJson<DragonRoadArtApproval>(art.review);
   PreviewRequire(review.schema=="dragon-road-overall-visual-review-v17"&&review.reviewer=="art_auditor"&&review.producer!=review.reviewer&&!review.runtimeAdmitted&&(review.internalPlaytestArtAccepted||review.acceptedForPlaytest)&&review.manifest.sha256==art.candidateManifest.sha256&&review.rows.Length==10,"Dragon Road independent visual approval changed");
   var rules=PreviewJson<PreviewRules>(b.rules).definitions;var effects=PreviewJson<NpcEffectDocument>(b.effects);
   var sounds=PreviewJson<NpcSoundDocument>(b.sounds);var mechanisms=PreviewJson<NpcMechanismDocument>(b.mechanisms);
   PreviewRequire(rules.Length==10&&!effects.runtimeAdmitted&&!sounds.runtimeAdmitted&&!mechanisms.runtimeAdmitted,"Dragon Road source profile scope changed");
   foreach(var f in new[]{b.effects,b.sounds,b.mechanisms})PreviewRequire(PreviewJson<PreviewSourceBinding>(f).sourceRulesSha256==b.rules.sha256,"Dragon Road sidecar changed");
   var allowed=new HashSet<string>{"NUI_BOX_BESMA1","NUI_CACTUS","NUI_KOMODO","NUI_LIZARDMAN_HIGH","NUI_LIZARDMAN_SAURUS","NUI_LIZARDMAN_SPEAR","NUI_SCORPION","NUI_SMALLGATE_BESMA_DRAGON_ROAD","NUI_SMALLGATE_POST_BESMA_DRAGON_ROAD","NUI_SPIKEBOARD_HERB"};
   var profiles=new Dictionary<string,NpcDefinition>();foreach(var p in rules){PreviewRequire(allowed.Remove(p.id),"unexpected Dragon Road source identity");profiles.Add(p.id,p);}PreviewRequire(allowed.Count==0,"missing Dragon Road source identity");
   var added=new HashSet<string>();foreach(var entry in b.sprites){
    var raw=PreviewJson<PreviewLibrary>(entry.animations);
    PreviewRequire(profiles.ContainsKey(entry.sourceNPC)&&raw.sourceNPC==entry.sourceNPC&&profiles[entry.sourceNPC].kind==raw.id&&added.Add(raw.id),"wrong Dragon Road new identity");
    var approved=Array.Find(review.rows,r=>r.sourceNPC==entry.sourceNPC);
    PreviewRequire(approved!=null&&(approved.internalPlaytestArtAccepted||approved.acceptedForPlaytest)&&approved.animations.sha256==entry.animations.sha256&&approved.atlas.sha256==entry.atlas.sha256,"Dragon Road exact incoming art was not approved");
    PreviewRead(approved.animations);PreviewRead(approved.atlas);LoadForestPreviewSprite(entry,raw);
   }
   foreach(var p in rules){
    PreviewRequire(libraries.ContainsKey(p.kind),"Dragon Road source art missing "+p.kind);
    foreach(var s in p.states)PreviewRequire(libraries[p.kind].frames.ContainsKey(s.clip),"missing Dragon Road action "+p.id+"/"+s.clip);
   }
   var model=PreviewJson<PreviewModel>(b.model);var activation=PreviewJson<PreviewActivationModel>(b.model);
   PreviewRequire(!model.runtimeAdmitted&&model.rooms.Length==3,"Dragon Road requires its three original worlds");
   forestPreviewStages=new StageArt[3];forestPreviewScenes=new NativeScene[3];forestPreviewGeometry=new byte[3][];
   var rooms=new List<RoomDefinition>();int entities=0,active=0;
   for(int i=0;i<3;i++){
    var room=model.rooms[i];var w=b.worlds[i];var scene=PreviewJson<NativeScene>(w.scene);var projection=PreviewJson<PreviewProjection>(w.projection);var geometry=PreviewRead(w.binary);
    PreviewRequire(w.id==DragonRoadWorlds[i]&&room.sourceWorldId==w.id&&scene.id==w.id&&projection.id==w.id&&projection.unit==.28f,"Dragon Road source projection changed");
    PreviewRequire(room.enemies.Length==new[]{22,20,2}[i]&&room.encounters.Length==new[]{2,2,1}[i],"Dragon Road declaration/zone count changed");
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;
    foreach(var t in w.textures){PreviewRead(t.source);forestPreviewTextureBindings[t.sourcePath]=PreviewTextureFile(t.selected);}
    foreach(var p in scene.parts)PreviewRequire(p.offset>=0&&p.count>0&&(long)p.offset+(long)p.count*20<=geometry.Length&&(string.IsNullOrEmpty(p.texture)||forestPreviewTextureBindings.ContainsKey(p.texture)),"Dragon Road mesh incomplete");
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=geometry;
    string music=Path.GetFileNameWithoutExtension(room.bgm[0]).ToLowerInvariant();PreviewRequire(sceneMusicAssets.ContainsKey(music),"Dragon Road original BGM missing "+music);
    string title=new[]{"Dragon Road / Rocky Pass","Dragon Road / Cactus Cliffs","Dragon Road / Twin Lizard Warriors"}[i];
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=w.id,title=title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=w.id,bgm=music};
    var target=new RoomDefinition{id=room.id,title=title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};
    for(int j=0;j<room.enemies.Length;j++){
     var e=room.enemies[j];var source=activation.rooms[i].enemies[j];PreviewRequire(e.sourceNPC==source.sourceNPC&&e.visualScale==1,"Dragon Road identity/scale changed");
     if(e.sourceNPC=="NUI_BOX_BESMA1")PreviewRequire(e.kind=="box-besma1"&&!e.aiEnabled&&!e.countsForClear,"Dragon Road box reuse changed");
     else {PreviewRequire(profiles.ContainsKey(e.sourceNPC)&&profiles[e.sourceNPC].kind==e.kind,"unbound Dragon Road NPC");if(e.aiEnabled){PreviewRequire(source.sourceActive&&e.countsForClear,"Dragon Road inactive actor enabled");active++;}}
     target.enemies.Add(e);entities++;
    }
    rooms.Add(target);
   }
   PreviewRequire(entities==44&&active==b.originalCombatCount,"Dragon Road source activation changed");
   var loaded=new Dictionary<string,AudioClip>();foreach(var audio in b.audio){
    var data=PreviewRead(audio.file);PreviewRequire(Array.Exists(sounds.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"Dragon Road unbound sound");
    AudioClip existing;if(audioClips.TryGetValue(audio.id,out existing)){PreviewRequire(existing&&PreviewSoundHashMatches(audio.id,audio.file.sha256),"Dragon Road source audio collision");continue;}
    bool wave=data.Length>=12&&data[0]=='R'&&data[8]=='W';
    using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(audio.file.path)).AbsoluteUri,wave?AudioType.WAV:AudioType.OGGVORBIS)){
     yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"Dragon Road source sound decode failed "+audio.id);
     var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip,"Dragon Road empty source audio");clip.name=audio.id;forestPreviewOwned.Add(clip);clip.LoadAudioData();
     while(clip.loadState==AudioDataLoadState.Loading)yield return null;PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"Dragon Road source audio unavailable");loaded.Add(audio.id,clip);
    }
   }
   RegisterNpcSoundClips(loaded);forestPreviewSounds.AddRange(loaded.Keys);
   LoadDragonRoadSourceVisual(b);
   dragonRoadLandingDiagnosticFrames=0;dragonRoadDiagnosticLandingSerials.Clear();
   game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(effects);game.SetNpcSoundDefinitions(sounds);game.SetNpcMechanismDefinitions(mechanisms);game.SetRooms(rooms);
   forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;
   ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();
   Debug.Log("DRAGON_ROAD_PREVIEW_READY worlds=3 zones=5 entities=44 sourceActive="+active+" libraries="+b.sprites.Length+" reused="+b.reusedAdmittedVisuals.Length+" bundle="+forestPreviewBundleHash);
  }
  // Draw the actual compiled effect helpers. Landing has attack lines and no
  // spheres, so the inherited sphere-only preview must not make it invisible.
  // This is explicitly diagnostic geometry, never replacement final artwork.
  void RenderDragonRoadHelpers(ref int used){
   foreach(var effect in game.npcEffects){
    if(outskirtsNpcVisual!=null&&Array.Exists(outskirtsNpcVisual.effectMeshes,b=>b.effect==effect.id))continue;
    var art=game.NpcEffectVisual(effect);if(!string.IsNullOrEmpty(art.id)&&libraries.ContainsKey(art.id))continue;
    var lines=game.NpcEffectLines(effect);bool landing=effect.id=="WALLY_8TH_JUMP_ATTACK_LANDING";
    var color=game.NpcEffectCanDamage(effect)?new Color(1,.35f,.15f,.98f):new Color(.75f,.85f,1,.75f);
    for(int i=0;i+5<lines.Length;i+=6){
     var line=ForestPreviewLine(used++,color);line.positionCount=2;
     for(int j=0;j<2;j++){int p=i+j*3;float depth=(effect.owner==null?0:effect.owner.nativeDepth)+lines[p+2]/.28f;line.SetPosition(j,ProjectNativePoint(new Vector3(lines[p],lines[p+1],0),depth));}
    }
    if(landing&&lines.Length>0){dragonRoadLandingDiagnosticFrames++;
     if(dragonRoadDiagnosticLandingSerials.Add(effect.serial))Debug.Log("DRAGON_ROAD_DIAGNOSTIC_LANDING serial="+effect.serial+" segments="+(lines.Length/6)+" sourceEffect="+effect.id+" originalGeometry=true");
    }
   }
  }
 }
}
#endif
