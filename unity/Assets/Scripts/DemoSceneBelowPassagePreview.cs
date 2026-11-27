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
  static readonly string[] BelowPassageNPCs={"NUI_GHOST_MAGICIAN_RAGGED","NUI_SOLDIER_THIN","NUI_SOLDIER_FAT","NUI_BENDERS_NASOD","NUI_MOUSE","NUI_MOUSE_NODROP","NUI_MOUSEHOUSE","NUI_STEAM_TRAP","NUI_LATTICE_GATE_BELOW_PASSAGE"};
  static readonly string[] BelowPassageWorlds={"WI_ELDER_BELOW_PASSAGE_STAGE2_1A","WI_ELDER_BELOW_PASSAGE_STAGE3_1A","WI_ELDER_BELOW_PASSAGE_STAGE4"};
  const string BelowPassageMovie="NUI_EVENT_NPC_BENDERS_ELDER";
  NpcDefinition belowPassageSteamProfile;
#pragma warning disable 0649
  [Serializable] sealed class PreviewSteamVisual {public string schema;public float unit,offsetY;public PreviewFile texture,sourceDDS,sourceContract;public NpcSteamEmitter[] emitters;}
#pragma warning restore 0649
  public static void ValidateBelowPassagePreviewEntry(string path){
   var bundle=JsonUtility.FromJson<PreviewBundle>(File.ReadAllText(path));
   PreviewRequire(bundle!=null&&bundle.schema=="below-passage-editor-preview-v1"&&bundle.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal"&&bundle.editorOnly&&!bundle.runtimeAdmitted,"wrong waterway session boundary");
   PreviewRequire(bundle.playableReady&&(bundle.blockers==null||bundle.blockers.Length==0),"waterway entry blocked:\n"+string.Join("\n",bundle.blockers??new[]{"Build the external waterway bundle"}));
  }
  public void BeginBelowPassagePreview(string path){ValidateBelowPassagePreviewEntry(path);BeginSourceDungeonPreview(path,false,true);}
  bool PreviewSoundHashMatches(string id,string hash){
   if(catalog.npcSoundRules!=null&&Array.Exists(catalog.npcSoundRules.assets,a=>a.id==id&&a.sha256==hash))return true;
   if(catalog.npcContentSets!=null)foreach(var scope in catalog.npcContentSets)if(scope!=null&&scope.npcSoundRules!=null&&Array.Exists(scope.npcSoundRules.assets,a=>a.id==id&&a.sha256==hash))return true;
   return false;
  }
  IEnumerator LoadBelowPassagePreview(){
   ValidateBelowPassagePreviewEntry(forestPreviewBundlePath);byte[] bytes=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(bytes);var bundle=JsonUtility.FromJson<PreviewBundle>(Encoding.UTF8.GetString(bytes));
   PreviewRequire(bundle.worlds.Length==3&&bundle.sprites.Length==10&&bundle.audio.Length==10&&bundle.originalEntityCount==44&&bundle.originalCombatCount==36,"wrong fixed waterway scope");
   PreviewRead(bundle.admittedCatalog);PreviewRequire(PreviewHash(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes)==bundle.admittedCatalog.sha256,"running five-dungeon catalog differs from pinned baseline");
   PreviewRead(bundle.materialOverrides);PreviewRead(bundle.soundMap);foreach(var f in bundle.reviewEvidence)PreviewRead(f);
   var art=PreviewJson<PreviewArtManifest>(bundle.artManifest);PreviewRequire(!art.runtimeAdmitted&&art.sourceRulesSha256==bundle.rules.sha256&&art.effectsSha256==bundle.effects.sha256,"waterway art/rules binding differs");
   var rules=PreviewJson<PreviewRules>(bundle.rules).definitions;var effects=PreviewJson<NpcEffectDocument>(bundle.effects);var sounds=PreviewJson<NpcSoundDocument>(bundle.sounds);var mechanisms=PreviewJson<NpcMechanismDocument>(bundle.mechanisms);
   PreviewRequire(rules.Length==9&&!effects.runtimeAdmitted&&!sounds.runtimeAdmitted&&!mechanisms.runtimeAdmitted,"candidate flags changed");
   foreach(var f in new[]{bundle.effects,bundle.sounds,bundle.mechanisms})PreviewRequire(PreviewJson<PreviewSourceBinding>(f).sourceRulesSha256==bundle.rules.sha256,"waterway-r2 sidecar binding differs");
   var expected=new HashSet<string>(BelowPassageNPCs);var profiles=new Dictionary<string,NpcDefinition>();
   foreach(var r in rules){PreviewRequire(r!=null&&expected.Remove(r.id)&&!r.runtimeAdmitted,"unexpected waterway rule");profiles.Add(r.id,r);}PreviewRequire(expected.Count==0,"missing waterway rule");belowPassageSteamProfile=profiles["NUI_STEAM_TRAP"];LoadBelowPassageSteam(bundle.steamVisual);
   expected=new HashSet<string>(BelowPassageNPCs);expected.Add(BelowPassageMovie);
   foreach(var entry in bundle.sprites){var raw=PreviewJson<PreviewLibrary>(entry.animations);PreviewRequire(expected.Remove(entry.sourceNPC)&&raw.sourceNPC==entry.sourceNPC&&raw.id==entry.id,"unexpected waterway sprite");
    if(entry.sourceNPC!=BelowPassageMovie)PreviewRequire(profiles[entry.sourceNPC].kind==entry.id,"rule/sprite kind mismatch");else PreviewRequire(entry.id=="event-npc-benders-elder","director ensemble identity changed");
    LoadForestPreviewSprite(entry,raw);
   }PreviewRequire(expected.Count==0,"missing source library");
   foreach(var rule in rules){var lib=libraries[rule.kind];foreach(var state in rule.states)PreviewRequire(lib.frames.ContainsKey(state.clip),"missing source-state clip "+rule.id+"/"+state.clip);if(string.IsNullOrEmpty(rule.environmentKind))foreach(var clip in new[]{"hurt","launch","down","death","jump","fall"})PreviewRequire(lib.frames.ContainsKey(clip),"missing source reaction/traversal clip "+rule.id+"/"+clip);}
   foreach(var id in bundle.reusedAdmittedVisuals??new string[0])PreviewRequire(libraries.ContainsKey(id)&&libraries[id].frames.ContainsKey("play"),"required exact admitted effect unavailable "+id);
   foreach(var fx in effects.definitions)PreviewRequire(string.IsNullOrEmpty(fx.visualId)||libraries.ContainsKey(fx.visualId),"effect visual unavailable "+fx.visualId);
   var model=PreviewJson<PreviewModel>(bundle.model);var activation=PreviewJson<PreviewActivationModel>(bundle.model);PreviewRequire(!model.runtimeAdmitted&&model.rooms.Length==3&&activation.rooms.Length==3,"wrong source rooms");
   forestPreviewStages=new StageArt[3];forestPreviewScenes=new NativeScene[3];forestPreviewGeometry=new byte[3][];var rooms=new List<RoomDefinition>();int entities=0,active=0,inactive=0,houses=0,steam=0,gates=0,boxes=0,movie=0;
   for(int i=0;i<3;i++){
    var room=model.rooms[i];var world=bundle.worlds[i];var scene=PreviewJson<NativeScene>(world.scene);var projection=PreviewJson<PreviewProjection>(world.projection);var geometry=PreviewRead(world.binary);
    PreviewRequire(room.sourceWorldId==BelowPassageWorlds[i]&&world.id==room.sourceWorldId&&scene.id==world.id&&projection.id==world.id&&Mathf.Abs(projection.unit-.28f)<.000001f&&projection.path.Length>0,"world projection mismatch");
    PreviewRequire(room.enemies.Length==new[]{14,25,5}[i]&&room.encounters.Length==new[]{1,2,1}[i],"source entities/zones changed");scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;
    foreach(var binding in world.textures){PreviewRead(binding.source);forestPreviewTextureBindings[binding.sourcePath]=PreviewTextureFile(binding.selected);}foreach(var p in scene.parts)PreviewRequire(p.offset>=0&&p.count>0&&(long)p.offset+(long)p.count*20<=geometry.Length&&(string.IsNullOrEmpty(p.texture)||forestPreviewTextureBindings.ContainsKey(p.texture)),"incomplete source geometry");
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=geometry;string music=Path.GetFileNameWithoutExtension(room.bgm[0]).ToLowerInvariant();PreviewRequire(music==(i==2?"music015_boss":"music014_stage")&&sceneMusicAssets.ContainsKey(music),"original waterway BGM missing");
    string title="Underground Waterway / "+(i==0?"Entrance":i==1?"Rat Tunnels":"Nasod Chamber");forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=world.id,title=title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=world.id,bgm=music};
    var target=new RoomDefinition{id=room.id,title=title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};
    for(int j=0;j<room.enemies.Length;j++){var e=room.enemies[j];var source=activation.rooms[i].enemies[j];PreviewRequire(e.sourceNPC==source.sourceNPC&&Mathf.Approximately(e.visualScale,1),"source identity/scale changed");e.aiEnabled=false;
     if(e.sourceNPC=="NUI_BOX"){PreviewRequire(e.kind=="prop-box"&&libraries.ContainsKey(e.kind)&&!source.sourceActive,"box identity changed");boxes++;}
     else if(e.sourceNPC==BelowPassageMovie){PreviewRequire(!source.sourceActive&&!e.countsForClear&&e.passive&&e.dormant,"source-inactive director must remain excluded from combat");movie++;}
     else{NpcDefinition p;PreviewRequire(profiles.TryGetValue(e.sourceNPC,out p)&&p.kind==e.kind,"unbound original source spawn");
      if(p.environmentKind=="mouse-house"){PreviewRequire(source.sourceActive&&e.countsForClear&&Mathf.Approximately(e.hp,4000),"mousehouse source active/HP changed");houses++;}
      else if(p.environmentKind=="timed-steam"){PreviewRequire(!source.sourceActive&&e.passive&&!e.countsForClear,"Steam must have no active combat body");steam++;}
      else if(p.environmentKind=="source-gate"){PreviewRequire(!source.sourceActive&&e.isGate&&!e.countsForClear,"gate source lock changed");gates++;}
      else{e.aiEnabled=source.sourceActive&&e.countsForClear;if(e.aiEnabled)active++;else inactive++;}
     }target.enemies.Add(e);entities++;
    }rooms.Add(target);
   }
   PreviewRequire(entities==44&&active==32&&inactive==2&&houses==2&&steam==1&&gates==2&&boxes==4&&movie==1,"source roster changed");
   var loaded=new Dictionary<string,AudioClip>();foreach(var audio in bundle.audio){PreviewRead(audio.file);PreviewRequire(Array.Exists(sounds.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"unbound original sound");
    AudioClip old;if(audioClips.TryGetValue(audio.id,out old)){PreviewRequire(old&&PreviewSoundHashMatches(audio.id,audio.file.sha256),"existing audio ID hash collision");continue;}
    using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(audio.file.path)).AbsoluteUri,AudioType.OGGVORBIS)){yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"Ogg decode failed "+audio.id);var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip,"empty source audio");clip.name=audio.id;forestPreviewOwned.Add(clip);clip.LoadAudioData();while(clip.loadState==AudioDataLoadState.Loading)yield return null;PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"Ogg did not load");loaded.Add(audio.id,clip);}
   }RegisterNpcSoundClips(loaded);forestPreviewSounds.AddRange(loaded.Keys);
   game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(effects);game.SetNpcSoundDefinitions(sounds);game.SetNpcMechanismDefinitions(mechanisms);game.SetRooms(rooms);
   forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;
   ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();Debug.Log("WATERWAY_PREVIEW_READY worlds=3 zones=4 entities=44 activeMobileAI=32 activeHouses=2 originalInactiveAI=2 sourceSounds=10 bundle="+forestPreviewBundleHash+" runtimeAdmitted=false");
  }
  void RenderBelowPassageHelpers(ref int used){
   if(belowPassageSteamProfile==null)return;
   foreach(var e in game.enemies){if(e.sourceNPC!="NUI_STEAM_TRAP"||!e.active||e.dormant)continue;var state=Array.Find(belowPassageSteamProfile.states,s=>s.id==e.npcState);if(state==null||state.samples==null||state.samples.Length==0||state.hits.Length==0)continue;
    bool hot=false;foreach(var hit in state.hits)if(e.npcTime>=hit.start&&e.npcTime<=hit.end)hot=true;
    float f=Mathf.Clamp01(e.npcTime/state.duration)*(state.samples.Length-1);int a=Mathf.FloorToInt(f),b=Mathf.Min(a+1,state.samples.Length-1);float t=f-a;var x=state.samples[a].attackLines;var y=state.samples[b].attackLines;
    for(int n=0;n+5<x.Length;n+=6){var line=ForestPreviewLine(used++,hot?new Color(1,.25f,.2f,.95f):new Color(1,.8f,.3f,.6f));line.positionCount=2;for(int j=0;j<2;j++){int k=n+j*3;float px=e.x+e.facing*Mathf.Lerp(x[k],y[k],t),py=e.y+Mathf.Lerp(x[k+1],y[k+1],t),z=e.nativeDepth+Mathf.Lerp(x[k+2],y[k+2],t)/.28f;line.SetPosition(j,ProjectNativePoint(new Vector3(px,py,0),z));}}
   }
  }
  void LoadBelowPassageSteam(PreviewFile binding){
   var data=PreviewJson<PreviewSteamVisual>(binding);PreviewRequire(data.schema=="below-passage-source-steam-visual-v1"&&Mathf.Approximately(data.unit,.28f)&&data.emitters.Length==2,"invalid source Steam visual");PreviewRead(data.sourceDDS);PreviewRead(data.sourceContract);
   foreach(var emitter in data.emitters)PreviewRequire(emitter.radius.Length==6&&emitter.rate>0&&emitter.lifetime>0&&emitter.capacity==100,"invalid Steam emitter");
   ConfigureNpcSteamVisual(new NpcSteamVisualData{unit=data.unit,offsetY=data.offsetY,emitters=data.emitters},PreviewTextureFile(data.texture),belowPassageSteamProfile);
  }
  void DisposeBelowPassageSteam(){DisposeNpcSteamVisual();belowPassageSteamProfile=null;}
 }
}
#endif
