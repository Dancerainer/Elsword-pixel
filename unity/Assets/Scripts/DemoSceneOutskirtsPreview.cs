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
  static readonly string[] OutskirtsNPCs={"NUI_MONKEY_BOMB","NUI_PPORU_WILLIAM_THIEF","NUI_PPORU_WILLIAM_THIEF_FIRE","NUI_PPORU_WILLIAM_B","NUI_PPORU_WILLIAM_C","NUI_WOOD_POST_TRAP","NUI_SMALLGATE_WALLY_CASTLE_A","NUI_SMALLGATE_POST_WALLY_CASTLE_A"};
  static readonly string[] OutskirtsWorlds={"WI_ELDER_WALLY_CASTLE_ROOF_STAGE1","WI_ELDER_WALLY_CASTLE_ROOF_STAGE2","WI_ELDER_WALLY_CASTLE_ROOF_STAGE4"};
  const string OutskirtsMovie="NUI_EVENT_NPC_WILLIAM_PPORU";
#pragma warning disable 0649
  [Serializable] sealed class PreviewOutskirtsTexture {public string key,path,sha256;}
  [Serializable] sealed class PreviewOutskirtsVisual {public string schema;public bool runtimeAdmitted;public PreviewFile document,meshManifest;public PreviewOutskirtsTexture[] textures;public PreviewFile[] evidence;}
#pragma warning restore 0649

  void LoadOutskirtsPreviewVisual(PreviewFile file,string rulesHash,string effectsHash){
   var binding=PreviewJson<PreviewOutskirtsVisual>(file);
   PreviewRequire(binding.schema=="outskirts-visual-binding-v14"&&!binding.runtimeAdmitted&&binding.textures.Length==8,"wrong scoped outskirts visual binding");
   var document=PreviewJson<OutskirtsNpcVisualDocument>(binding.document);
   PreviewRequire(document.sourceRulesSha256==rulesHash&&document.sourceEffectsSha256==effectsHash,"outskirts visual source clocks differ");
   var meshes=PreviewJson<OutskirtsNpcMeshManifest>(binding.meshManifest);
   var textures=new Dictionary<string,Texture2D>();
   foreach(var texture in binding.textures){PreviewRequire(!string.IsNullOrEmpty(texture.key)&&!textures.ContainsKey(texture.key),"duplicate source visual texture");textures.Add(texture.key,PreviewTextureFile(new PreviewFile{path=texture.path,sha256=texture.sha256}));}
   foreach(var evidence in binding.evidence)PreviewRead(evidence);
   ConfigureOutskirtsNpcVisual(document,meshes,textures);
  }

  public static void ValidateOutskirtsPreviewEntry(string path){
   var b=JsonUtility.FromJson<PreviewBundle>(File.ReadAllText(path));
   PreviewRequire(b!=null&&b.schema=="outskirts-editor-preview-v14"&&b.sourceDungeon=="Dungeon_Elder_Wally_Castle_Roof_Normal"&&b.editorOnly&&!b.runtimeAdmitted,"wrong outskirts session boundary");
   PreviewRequire(b.playableReady&&(b.blockers==null||b.blockers.Length==0),"outskirts entry blocked:\n"+string.Join("\n",b.blockers??new[]{"Build the external outskirts bundle"}));
  }
  public void BeginOutskirtsPreview(string path){ValidateOutskirtsPreviewEntry(path);BeginSourceDungeonPreview(path,false,false,true);}

  IEnumerator LoadOutskirtsPreview(){
   ValidateOutskirtsPreviewEntry(forestPreviewBundlePath);
   byte[] bytes=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(bytes);
   var bundle=JsonUtility.FromJson<PreviewBundle>(Encoding.UTF8.GetString(bytes));
   PreviewRequire(bundle.worlds.Length==3&&bundle.sprites.Length==9&&bundle.audio.Length==14&&bundle.originalEntityCount==47&&bundle.originalCombatCount==32,"wrong fixed outskirts scope");
   PreviewRead(bundle.admittedCatalog);
   PreviewRequire(PreviewHash(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes)==bundle.admittedCatalog.sha256,"running six-dungeon catalog differs from pinned baseline");
   PreviewRead(bundle.materialOverrides);PreviewRead(bundle.soundMap);foreach(var file in bundle.reviewEvidence)PreviewRead(file);
   var art=PreviewJson<PreviewArtManifest>(bundle.artManifest);
   PreviewRequire(!art.runtimeAdmitted&&art.sourceRulesSha256==bundle.rules.sha256&&art.effectsSha256==bundle.effects.sha256,"outskirts art/rules binding differs");
   var rules=PreviewJson<PreviewRules>(bundle.rules).definitions;
   var effects=PreviewJson<NpcEffectDocument>(bundle.effects);
   var sounds=PreviewJson<NpcSoundDocument>(bundle.sounds);
   var mechanisms=PreviewJson<NpcMechanismDocument>(bundle.mechanisms);
   PreviewRequire(rules.Length==8&&effects.definitions.Length==9&&!effects.runtimeAdmitted&&!sounds.runtimeAdmitted&&!mechanisms.runtimeAdmitted,"outskirts candidate flags changed");
   foreach(var file in new[]{bundle.effects,bundle.sounds,bundle.mechanisms})PreviewRequire(PreviewJson<PreviewSourceBinding>(file).sourceRulesSha256==bundle.rules.sha256,"outskirts-r2 sidecar binding differs");
   var expected=new HashSet<string>(OutskirtsNPCs);var profiles=new Dictionary<string,NpcDefinition>();
   foreach(var rule in rules){PreviewRequire(rule!=null&&expected.Remove(rule.id)&&!rule.runtimeAdmitted,"unexpected outskirts rule");profiles.Add(rule.id,rule);}
   PreviewRequire(expected.Count==0,"missing outskirts rule");expected=new HashSet<string>(OutskirtsNPCs);expected.Add(OutskirtsMovie);
   foreach(var entry in bundle.sprites){
    var raw=PreviewJson<PreviewLibrary>(entry.animations);
    PreviewRequire(expected.Remove(entry.sourceNPC)&&raw.sourceNPC==entry.sourceNPC&&raw.id==entry.id&&raw.sourceRulesSha256==bundle.rules.sha256,"unexpected outskirts sprite");
    PreviewRequire(entry.sourceNPC==OutskirtsMovie?entry.id=="event-npc-william-pporu":profiles[entry.sourceNPC].kind==entry.id,"outskirts rule/sprite identity mismatch");
    LoadForestPreviewSprite(entry,raw);
   }
   PreviewRequire(expected.Count==0,"missing source library");
   foreach(var rule in rules){
    var lib=libraries[rule.kind];foreach(var state in rule.states)PreviewRequire(lib.frames.ContainsKey(state.clip),"missing source-state clip "+rule.id+"/"+state.clip);
    if(string.IsNullOrEmpty(rule.environmentKind))foreach(var clip in new[]{"hurt","launch","down","death","jump","fall"})PreviewRequire(lib.frames.ContainsKey(clip),"missing source reaction/traversal clip "+rule.id+"/"+clip);
   }
   var model=PreviewJson<PreviewModel>(bundle.model);var activation=PreviewJson<PreviewActivationModel>(bundle.model);
   PreviewRequire(!model.runtimeAdmitted&&model.rooms.Length==3&&activation.rooms.Length==3,"wrong outskirts rooms");
   forestPreviewStages=new StageArt[3];forestPreviewScenes=new NativeScene[3];forestPreviewGeometry=new byte[3][];
   var rooms=new List<RoomDefinition>();int entities=0,active=0,inactive=0,traps=0,gates=0,boxes=0,movie=0;
   for(int i=0;i<3;i++){
    var room=model.rooms[i];var world=bundle.worlds[i];var scene=PreviewJson<NativeScene>(world.scene);var projection=PreviewJson<PreviewProjection>(world.projection);var geometry=PreviewRead(world.binary);
    PreviewRequire(room.sourceWorldId==OutskirtsWorlds[i]&&world.id==room.sourceWorldId&&scene.id==world.id&&projection.id==world.id&&Mathf.Abs(projection.unit-.28f)<.000001f&&projection.path.Length>0,"outskirts world projection mismatch");
    PreviewRequire(room.enemies.Length==new[]{15,29,3}[i]&&room.encounters.Length==new[]{1,2,1}[i],"source outskirts entities/zones changed");
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;
    foreach(var binding in world.textures){PreviewRead(binding.source);forestPreviewTextureBindings[binding.sourcePath]=PreviewTextureFile(binding.selected);}
    foreach(var part in scene.parts)PreviewRequire(part.offset>=0&&part.count>0&&(long)part.offset+(long)part.count*20<=geometry.Length&&(string.IsNullOrEmpty(part.texture)||forestPreviewTextureBindings.ContainsKey(part.texture)),"incomplete source outskirts geometry");
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=geometry;
    string music=Path.GetFileNameWithoutExtension(room.bgm[0]).ToLowerInvariant();
    PreviewRequire(music==(i==2?"music009_boss":"music016_stage")&&sceneMusicAssets.ContainsKey(music),"original outskirts BGM missing");
    string title="Wally Castle Outskirts / "+(i==0?"Approach":i==1?"Castle Walls":"William's Post");
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=world.id,title=title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=world.id,bgm=music};
    var target=new RoomDefinition{id=room.id,title=title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};
    for(int j=0;j<room.enemies.Length;j++){
     var e=room.enemies[j];var source=activation.rooms[i].enemies[j];
     PreviewRequire(e.sourceNPC==source.sourceNPC&&Mathf.Approximately(e.visualScale,1),"outskirts source identity/scale changed");e.aiEnabled=false;
     if(e.sourceNPC=="NUI_BOX"){PreviewRequire(e.kind=="prop-box"&&libraries.ContainsKey(e.kind)&&!source.sourceActive&&!e.countsForClear,"outskirts box identity changed");boxes++;}
     else if(e.sourceNPC==OutskirtsMovie){PreviewRequire(!source.sourceActive&&!e.countsForClear&&e.passive&&e.dormant,"source-inactive director must remain excluded from combat");movie++;}
     else{
      NpcDefinition p;PreviewRequire(profiles.TryGetValue(e.sourceNPC,out p)&&p.kind==e.kind,"unbound original outskirts spawn");
      if(p.environmentKind=="proximity-trap"){PreviewRequire(!source.sourceActive&&e.passive&&!e.countsForClear,"wood trap source activation changed");traps++;}
      else if(p.environmentKind=="source-gate"){PreviewRequire(!source.sourceActive&&e.isGate&&!e.countsForClear,"outskirts source gate lock changed");gates++;}
      else{e.aiEnabled=source.sourceActive&&e.countsForClear;if(e.aiEnabled)active++;else inactive++;}
     }
     target.enemies.Add(e);entities++;
    }
    rooms.Add(target);
   }
   PreviewRequire(entities==47&&active==32&&inactive==2&&traps==5&&gates==4&&boxes==3&&movie==1,"source outskirts roster changed");
   var loaded=new Dictionary<string,AudioClip>();
   foreach(var audio in bundle.audio){
    var data=PreviewRead(audio.file);PreviewRequire(Array.Exists(sounds.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"unbound original outskirts sound");
    AudioClip old;if(audioClips.TryGetValue(audio.id,out old)){PreviewRequire(old&&PreviewSoundHashMatches(audio.id,audio.file.sha256),"existing audio ID hash collision");continue;}
    bool wave=data.Length>=12&&data[0]=='R'&&data[1]=='I'&&data[2]=='F'&&data[3]=='F'&&data[8]=='W'&&data[9]=='A'&&data[10]=='V'&&data[11]=='E';
    bool ogg=data.Length>=4&&data[0]=='O'&&data[1]=='g'&&data[2]=='g'&&data[3]=='S';
    PreviewRequire(wave||ogg,"unknown source audio container "+audio.id);
    using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(audio.file.path)).AbsoluteUri,wave?AudioType.WAV:AudioType.OGGVORBIS)){
     yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"source decode failed "+audio.id);
     var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip,"empty source audio");clip.name=audio.id;forestPreviewOwned.Add(clip);clip.LoadAudioData();
     while(clip.loadState==AudioDataLoadState.Loading)yield return null;
     PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"source audio did not load");loaded.Add(audio.id,clip);
    }
   }
   RegisterNpcSoundClips(loaded);forestPreviewSounds.AddRange(loaded.Keys);
   LoadOutskirtsPreviewVisual(bundle.outskirtsVisual,bundle.rules.sha256,bundle.effects.sha256);
   game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(effects);game.SetNpcSoundDefinitions(sounds);game.SetNpcMechanismDefinitions(mechanisms);game.SetRooms(rooms);
   forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;
   ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();
   Debug.Log("OUTSKIRTS_PREVIEW_READY worlds=3 zones=4 entities=47 activeMobileAI=32 originalInactiveAI=2 traps=5 sourceSounds=14 bundle="+forestPreviewBundleHash+" runtimeAdmitted=false");
  }
 }
}
#endif
