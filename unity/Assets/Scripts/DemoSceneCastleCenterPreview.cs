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
  static readonly string[] CastleCenterWorlds={"WI_ELDER_WALLY_CASTLE_CENTER_STAGE3","WI_ELDER_WALLY_CASTLE_CENTER_STAGE4"};
  public static void ValidateCastleCenterPreviewEntry(string path){
   var b=JsonUtility.FromJson<PreviewBundle>(File.ReadAllText(path));
   PreviewRequire(b!=null&&b.schema=="castle-center-editor-preview-v16"&&b.sourceDungeon=="Dungeon_Elder_Wally_Castle_Center_Normal"&&b.editorOnly&&!b.runtimeAdmitted&&b.playableReady&&(b.blockers==null||b.blockers.Length==0),"Castle Center bundle not ready");
  }
  public void BeginCastleCenterPreview(string path){ValidateCastleCenterPreviewEntry(path);BeginSourceDungeonPreview(path,false,false,false,true);}
  IEnumerator LoadCastleCenterPreview(){
   ValidateCastleCenterPreviewEntry(forestPreviewBundlePath);
   var bytes=File.ReadAllBytes(forestPreviewBundlePath);forestPreviewBundleHash=PreviewHash(bytes);
   var b=JsonUtility.FromJson<PreviewBundle>(Encoding.UTF8.GetString(bytes));
   PreviewRequire(b.worlds.Length==2&&b.sprites.Length==4&&b.originalEntityCount==40,"Castle Center source scope changed");
   PreviewRead(b.admittedCatalog);PreviewRequire(PreviewHash(Resources.Load<TextAsset>("Pixel/runtime-catalog").bytes)==b.admittedCatalog.sha256,"Castle Center preview catalog baseline changed");
   foreach(var f in b.reviewEvidence)PreviewRead(f);PreviewRead(b.soundMap);PreviewRead(b.materialOverrides);
   var art=PreviewJson<PreviewArtManifest>(b.artManifest);
   PreviewRequire(art.sourceRulesSha256==b.rules.sha256&&art.effectsSha256==b.effects.sha256&&!art.runtimeAdmitted,"Castle Center art binding changed");
   var rules=PreviewJson<PreviewRules>(b.rules).definitions;var effects=PreviewJson<NpcEffectDocument>(b.effects);
   var sounds=PreviewJson<NpcSoundDocument>(b.sounds);var mechanisms=PreviewJson<NpcMechanismDocument>(b.mechanisms);
   PreviewRequire(rules.Length==9&&!effects.runtimeAdmitted&&!sounds.runtimeAdmitted&&!mechanisms.runtimeAdmitted,"Castle Center source profile scope changed");
   foreach(var f in new[]{b.effects,b.sounds,b.mechanisms})PreviewRequire(PreviewJson<PreviewSourceBinding>(f).sourceRulesSha256==b.rules.sha256,"Castle Center sidecar changed");
   var profiles=new Dictionary<string,NpcDefinition>();foreach(var p in rules)profiles.Add(p.id,p);
   var added=new HashSet<string>();foreach(var entry in b.sprites){
    var raw=PreviewJson<PreviewLibrary>(entry.animations);
    PreviewRequire(profiles.ContainsKey(entry.sourceNPC)&&raw.sourceNPC==entry.sourceNPC&&profiles[entry.sourceNPC].kind==raw.id&&added.Add(raw.id),"wrong Castle Center new identity");
    LoadForestPreviewSprite(entry,raw);
   }
   foreach(var p in rules){
    PreviewRequire(libraries.ContainsKey(p.kind),"Castle Center source art missing "+p.kind);
    foreach(var s in p.states)PreviewRequire(libraries[p.kind].frames.ContainsKey(s.clip),"missing Castle Center action "+p.id+"/"+s.clip);
   }
   var model=PreviewJson<PreviewModel>(b.model);var activation=PreviewJson<PreviewActivationModel>(b.model);
   PreviewRequire(!model.runtimeAdmitted&&model.rooms.Length==2,"Castle Center requires its two original worlds");
   forestPreviewStages=new StageArt[2];forestPreviewScenes=new NativeScene[2];forestPreviewGeometry=new byte[2][];
   var rooms=new List<RoomDefinition>();int entities=0,active=0;
   for(int i=0;i<2;i++){
    var room=model.rooms[i];var w=b.worlds[i];var scene=PreviewJson<NativeScene>(w.scene);var projection=PreviewJson<PreviewProjection>(w.projection);var geometry=PreviewRead(w.binary);
    PreviewRequire(w.id==CastleCenterWorlds[i]&&room.sourceWorldId==w.id&&scene.id==w.id&&projection.id==w.id&&projection.unit==.28f,"Castle Center source projection changed");
    PreviewRequire(room.enemies.Length==new[]{37,3}[i]&&room.encounters.Length==new[]{3,1}[i],"Castle Center declaration/zone count changed");
    scene.unit=projection.unit;scene.offsetX=projection.offsetX;scene.offsetY=projection.offsetY;scene.path=projection.path;
    foreach(var t in w.textures){PreviewRead(t.source);forestPreviewTextureBindings[t.sourcePath]=PreviewTextureFile(t.selected);}
    foreach(var p in scene.parts)PreviewRequire(p.offset>=0&&p.count>0&&(long)p.offset+(long)p.count*20<=geometry.Length&&(string.IsNullOrEmpty(p.texture)||forestPreviewTextureBindings.ContainsKey(p.texture)),"Castle Center mesh incomplete");
    forestPreviewScenes[i]=scene;forestPreviewGeometry[i]=geometry;
    string music=Path.GetFileNameWithoutExtension(room.bgm[0]).ToLowerInvariant();PreviewRequire(sceneMusicAssets.ContainsKey(music),"Castle Center original BGM missing "+music);
    string title=i==0?"Wally Castle Center / Siege Hall":"Wally Castle Center / Wally No.8";
    forestPreviewStages[i]=new StageArt{id=room.id,sourceWorldId=w.id,title=title,width=projection.width,height=projection.height,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces,nativeScene=w.id,bgm=music};
    var target=new RoomDefinition{id=room.id,title=title,width=room.width,spawnX=room.spawnX,spawnY=room.spawnY,hasSpawnY=room.hasSpawnY,exitX=room.exitX,entryEncounter=room.entryEncounter,encounters=room.encounters,groundNodes=room.groundNodes,surfaces=room.surfaces};
    for(int j=0;j<room.enemies.Length;j++){
     var e=room.enemies[j];var source=activation.rooms[i].enemies[j];PreviewRequire(e.sourceNPC==source.sourceNPC&&e.visualScale==1,"Castle Center identity/scale changed");
     if(e.sourceNPC=="NUI_BOX")PreviewRequire(e.kind=="prop-box"&&!e.aiEnabled&&!e.countsForClear,"Castle Center box reuse changed");
     else {PreviewRequire(profiles.ContainsKey(e.sourceNPC)&&profiles[e.sourceNPC].kind==e.kind,"unbound Castle Center NPC");if(e.aiEnabled){PreviewRequire(source.sourceActive&&e.countsForClear,"Castle Center inactive actor enabled");active++;}}
     target.enemies.Add(e);entities++;
    }
    rooms.Add(target);
   }
   PreviewRequire(entities==40&&active==b.originalCombatCount,"Castle Center source activation changed");
   var loaded=new Dictionary<string,AudioClip>();foreach(var audio in b.audio){
    var data=PreviewRead(audio.file);PreviewRequire(Array.Exists(sounds.assets,a=>a.id==audio.id&&a.sha256==audio.file.sha256),"Castle Center unbound sound");
    AudioClip existing;if(audioClips.TryGetValue(audio.id,out existing)){PreviewRequire(existing&&PreviewSoundHashMatches(audio.id,audio.file.sha256),"Castle Center source audio collision");continue;}
    bool wave=data.Length>=12&&data[0]=='R'&&data[8]=='W';
    using(var request=UnityWebRequestMultimedia.GetAudioClip(new Uri(Path.GetFullPath(audio.file.path)).AbsoluteUri,wave?AudioType.WAV:AudioType.OGGVORBIS)){
     yield return request.SendWebRequest();PreviewRequire(request.result==UnityWebRequest.Result.Success,"Castle Center source sound decode failed "+audio.id);
     var clip=DownloadHandlerAudioClip.GetContent(request);PreviewRequire(clip,"Castle Center empty source audio");clip.name=audio.id;forestPreviewOwned.Add(clip);clip.LoadAudioData();
     while(clip.loadState==AudioDataLoadState.Loading)yield return null;PreviewRequire(clip.loadState==AudioDataLoadState.Loaded,"Castle Center source audio unavailable");loaded.Add(audio.id,clip);
    }
   }
   RegisterNpcSoundClips(loaded);forestPreviewSounds.AddRange(loaded.Keys);
   game.SetNpcDefinitions(rules);game.SetNpcEffectDefinitions(effects);game.SetNpcSoundDefinitions(sounds);game.SetNpcMechanismDefinitions(mechanisms);game.SetRooms(rooms);
   forestPreviewActive=true;forestPreviewLoading=false;adventureStarted=true;openMenu="";menuOrigin="";pausedBeforeMenu=false;game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;
   ApplyTimings();ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();RenderForestPreviewStatus();
   Debug.Log("CASTLE_CENTER_PREVIEW_READY worlds=2 zones=4 entities=40 sourceActive="+active+" libraries=4 reused=5 bundle="+forestPreviewBundleHash);
  }
 }
}
#endif
