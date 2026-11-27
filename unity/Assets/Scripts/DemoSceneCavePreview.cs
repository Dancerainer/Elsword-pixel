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
  static readonly string[] CavePreviewNPCs={"NUI_BAT_A","NUI_BAT_B","NUI_THIEF_THIN","NUI_THIEF_FAT","NUI_THIEF_FAT_BOSS","NUI_PPORU_GIANT_RED","NUI_BENDERS_UPGRADE_NORMAL","NUI_DOWN_STONE_TRAP","NUI_SMALLGATE_POST_BENDERS_CAVE_A","NUI_SMALLGATE_BENDERS_CAVE_A","NUI_SMALLGATE_BOSS_POST_BENDERS_CAVE","NUI_SMALLGATE_BOSS_BENDERS_CAVE"};
  static readonly string[] CavePreviewWorlds={"WI_ELDER_BENDERS_CAVE_STAGE1","WI_ELDER_BENDERS_CAVE_STAGE1_1","WI_ELDER_BENDERS_STAGE_BOSS"};
  public static void ValidateCavePreviewEntry(string path){
   var bundle=JsonUtility.FromJson<PreviewBundle>(File.ReadAllText(path));
   PreviewRequire(bundle!=null&&bundle.schema=="cave-editor-preview-v1"&&bundle.sourceDungeon=="Dungeon_Elder_Benders_Cave_Normal"&&bundle.editorOnly&&!bundle.runtimeAdmitted,"wrong cave session boundary");
   PreviewRequire(bundle.playableReady&&(bundle.blockers==null||bundle.blockers.Length==0),"cave entry blocked:\n"+string.Join("\n",bundle.blockers??new[]{"Build the cave bundle after providing all actor libraries"}));
  }
  public void BeginCavePreview(string bundlePath){ValidateCavePreviewEntry(bundlePath);BeginSourceDungeonPreview(bundlePath,true);}
  IEnumerator LoadCavePreview(){
   ValidateCavePreviewEntry(forestPreviewBundlePath);
   byte[] manifestBytes=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(manifestBytes);
   var bundle=JsonUtility.FromJson<PreviewBundle>(Encoding.UTF8.GetString(manifestBytes));
   PreviewRequire(bundle.worlds!=null&&bundle.worlds.Length==3&&bundle.sprites!=null&&bundle.sprites.Length==12&&bundle.audio!=null&&bundle.audio.Length==17&&bundle.originalEntityCount==39&&bundle.originalCombatCount==31,"wrong fixed cave scope");
   PreviewRead(bundle.admittedCatalog);PreviewRequire(PreviewHash(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes)==bundle.admittedCatalog.sha256,"running catalog differs from pinned four-dungeon baseline");
   PreviewRead(bundle.materialOverrides);PreviewRead(bundle.soundMap);foreach(var f in bundle.reviewEvidence)PreviewRead(f);
   var art=PreviewJson<PreviewArtManifest>(bundle.artManifest);
   PreviewRequire(!art.runtimeAdmitted&&art.sourceRulesSha256==bundle.rules.sha256&&art.effectsSha256==bundle.effects.sha256&&bundle.loadedEffectsSha256==bundle.effects.sha256,"cave art/sidecar binding differs");
   var rules=PreviewJson<PreviewRules>(bundle.rules).definitions;var effects=PreviewJson<NpcEffectDocument>(bundle.effects);
   var sounds=PreviewJson<NpcSoundDocument>(bundle.sounds);var mechanisms=PreviewJson<NpcMechanismDocument>(bundle.mechanisms);
   PreviewRequire(rules!=null&&rules.Length==12&&!effects.runtimeAdmitted&&!sounds.runtimeAdmitted&&!mechanisms.runtimeAdmitted,"cave candidate flags changed");
   PreviewRequire(PreviewJson<PreviewSourceBinding>(bundle.effects).sourceRulesSha256==bundle.rules.sha256&&PreviewJson<PreviewSourceBinding>(bundle.sounds).sourceRulesSha256==bundle.rules.sha256&&mechanisms.sourceRulesSha256==bundle.rules.sha256,"cave-r2 sidecar mismatch");
   var expected=new HashSet<string>(CavePreviewNPCs);var profiles=new Dictionary<string,NpcDefinition>();
   foreach(var rule in rules){PreviewRequire(rule!=null&&expected.Remove(rule.id)&&!rule.runtimeAdmitted,"unexpected cave source rule");profiles.Add(rule.id,rule);}
   PreviewRequire(expected.Count==0,"missing cave source rule");expected=new HashSet<string>(CavePreviewNPCs);
   foreach(var entry in bundle.sprites){
    var raw=PreviewJson<PreviewLibrary>(entry.animations);
    PreviewRequire(expected.Remove(entry.sourceNPC)&&raw.id==entry.id&&raw.sourceNPC==entry.sourceNPC&&profiles[entry.sourceNPC].kind==entry.id,"unexpected cave pixel identity");
    LoadForestPreviewSprite(entry,raw);
   }
   PreviewRequire(expected.Count==0,"missing cave pixel identity");
   foreach(var rule in rules){
    var library=libraries[rule.kind];foreach(var state in rule.states)PreviewRequire(library.frames.ContainsKey(state.clip),"cave state would fall back to idle: "+rule.id+"/"+state.clip);
    if(string.IsNullOrEmpty(rule.environmentKind))foreach(var clip in new[]{"hurt","launch","down","death","jump","fall"})PreviewRequire(library.frames.ContainsKey(clip),"missing cave reaction/traversal art: "+rule.id+"/"+clip);
   }
   // Current r2 effects have no asserted pixel visual identity. Show their real
   // helper geometry with the persistent diagnostic notice, never a fake slash.
   foreach(var fx in effects.definitions)PreviewRequire(string.IsNullOrEmpty(fx.visualId),"cave effect requires explicit candidate pixel binding: "+fx.visualId);
   var model=PreviewJson<PreviewModel>(bundle.model);var activation=PreviewJson<PreviewActivationModel>(bundle.model);
   PreviewRequire(!model.runtimeAdmitted&&model.rooms!=null&&model.rooms.Length==3&&activation.rooms.Length==3,"wrong cave source route");
   forestPreviewStages=new StageArt[3];forestPreviewScenes=new NativeScene[3];forestPreviewGeometry=new byte[3][];
   var rooms=new List<RoomDefinition>();int entities=0,combat=0,environments=0,boxes=0,enabledAI=0,inactiveAI=0;
   int[] counts={15,21,3},zones={1,2,1};
   for(int i=0;i<3;i++){
    var room=model.rooms[i];var world=bundle.worlds[i];var scene=PreviewJson<NativeScene>(world.scene);var projection=PreviewJson<PreviewProjection>(world.projection);var bytes=PreviewRead(world.binary);
    PreviewRequire(room.sourceWorldId==CavePreviewWorlds[i]&&world.id==room.sourceWorldId&&scene.id==world.id&&projection.id==world.id&&projection.path.Length>0&&Mathf.Abs(projection.unit-.28f)<.000001f,"cave scene/projection mismatch");
    PreviewRequire(room.enemies.Length==counts[i]&&room.encounters.Length==zones[i]&&activation.rooms[i].enemies.Length==counts[i],"cave roster/zones changed");
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;
    foreach(var binding in world.textures){PreviewRead(binding.source);forestPreviewTextureBindings[binding.sourcePath]=PreviewTextureFile(binding.selected);}
    foreach(var part in scene.parts)PreviewRequire(part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=bytes.Length&&(string.IsNullOrEmpty(part.texture)||forestPreviewTextureBindings.ContainsKey(part.texture)),"incomplete cave mesh/texture");
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=bytes;
    string music=Path.GetFileNameWithoutExtension(room.bgm[0]).ToLowerInvariant();PreviewRequire(music==(i==2?"music011_boss":"music010_stage")&&sceneMusicAssets.ContainsKey(music),"cave original BGM unavailable");
    string title="Banthus Cave / "+(i==0?"Entrance":i==1?"Deep Cavern":"Boss Den");
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=world.id,title=title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=world.id,bgm=music};
    var target=new RoomDefinition{id=room.id,title=title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};
    for(int j=0;j<room.enemies.Length;j++){
     var enemy=room.enemies[j];var source=activation.rooms[i].enemies[j];PreviewRequire(source.sourceNPC==enemy.sourceNPC,"cave activation identity mismatch");
     if(enemy.sourceNPC=="NUI_BOX"){PreviewRequire(enemy.kind=="prop-box"&&libraries.ContainsKey(enemy.kind)&&!source.sourceActive,"box must reuse the exact admitted identity");enemy.aiEnabled=false;boxes++;}
     else{
      NpcDefinition profile;PreviewRequire(profiles.TryGetValue(enemy.sourceNPC,out profile)&&profile.kind==enemy.kind,"unbound cave spawn "+enemy.sourceNPC);
      bool environment=!string.IsNullOrEmpty(profile.environmentKind);
      enemy.aiEnabled=!environment&&source.sourceActive&&enemy.countsForClear;
      if(environment){PreviewRequire(!source.sourceActive&&!enemy.countsForClear,"cave environment became a combat clear target");environments++;}
      else{combat++;if(enemy.aiEnabled)enabledAI++;else inactiveAI++;}
     }
     target.enemies.Add(enemy);entities++;
    }
    rooms.Add(target);
   }
   PreviewRequire(entities==39&&combat==31&&environments==6&&boxes==2&&enabledAI==29&&inactiveAI==2,"cave source ACTIVE/roster must be 29 active + two original inactive + eight props");
   var loaded=new Dictionary<string,AudioClip>();foreach(var audio in bundle.audio){
    PreviewRead(audio.file);PreviewRequire(Array.Exists(sounds.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"unbound cave sound");
    AudioClip existing;if(audioClips.TryGetValue(audio.id,out existing)){
     PreviewRequire(existing&&catalog.npcSoundRules!=null&&Array.Exists(catalog.npcSoundRules.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"cave sound ID collides with a different loaded asset");continue;
    }
    using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(audio.file.path)).AbsoluteUri,AudioType.OGGVORBIS)){
     yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"cave Ogg decode failed "+audio.id+" "+request.error);
     var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip!=null,"empty cave Ogg");clip.name=audio.id;forestPreviewOwned.Add(clip);clip.LoadAudioData();while(clip.loadState==AudioDataLoadState.Loading)yield return null;
     PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"cave Ogg data unavailable");loaded.Add(audio.id,clip);
    }
   }
   RegisterNpcSoundClips(loaded);forestPreviewSounds.AddRange(loaded.Keys);
   game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(effects);game.SetNpcSoundDefinitions(sounds);game.SetNpcMechanismDefinitions(mechanisms);game.SetRooms(rooms);
   forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;
   ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();
   Debug.Log("CAVE_PREVIEW_READY worlds=3 zones=4 entities=39 sourceActiveAI=29 originalInactiveAI=2 sourceSounds=17 bundle="+forestPreviewBundleHash+" runtimeAdmitted=false");
  }
 }
}
#endif
