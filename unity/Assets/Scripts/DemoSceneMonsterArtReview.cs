using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        bool monsterArtReview;
        [Serializable] class MotionArtScopeDocument {public MotionArtScopeEntry[] iterationArtOverrides;}
        [Serializable] class MotionArtScopeEntry {public string schema;public MotionArtScopeBatch[] batches;}
        [Serializable] class MotionArtScopeBatch {public string runtimeId;public MotionArtScopeClip[] clips;}
        [Serializable] class MotionArtScopeClip {public string id;}
        [Serializable] class MonsterArtProbe {
            public string monster,clip,actualSprite,texture,artVersion,screenshot;
            public int frameIndex,frameCount,distinctPaintedRects;
            public bool matchesExpected,newLibrary;
            public float anchorX,anchorY,scale;
            public Rect rect;
        }
        [Serializable] class MonsterArtRuntimeReport {
            public bool passed;
            public int monsters,clips,poseSamples;
            public string checkedUtc,executable,catalogSha256;
            public string scope="Actual Windows Player, production Pose/RenderActors/NativeProjection path. Paused renderer fixture; natural AI and dungeon progression checked separately.";
            public MonsterArtProbe[] samples;
        }
        IEnumerator ReviewAllMonsterArt(bool motionOnly=false,bool outskirtsOnly=false){
            var motionScopes=new Dictionary<string,HashSet<string>>();int expectedMotionClips=0;
            if(outskirtsOnly){
                var content=Array.Find(catalog.npcContentSets,s=>s.sourceDungeon==OutskirtsSourceDungeon);
                if(content==null||!outskirtsIterationApproved)throw new InvalidOperationException("No reviewed outskirts content in this build");
                foreach(var npc in content.npcRules)if(string.IsNullOrEmpty(npc.environmentKind)){
                    var monster=Array.Find(catalog.monsters,m=>m.id==npc.kind);var ids=new HashSet<string>();foreach(var clip in monster.clips)ids.Add(clip.id);motionScopes.Add(monster.id,ids);expectedMotionClips+=ids.Count;
                }
                if(motionScopes.Count!=5||expectedMotionClips!=72)throw new InvalidOperationException("Outskirts art review has a different five-actor/72-clip scope");
            }
            else if(motionOnly){
                var doc=JsonUtility.FromJson<MotionArtScopeDocument>(Resources.Load<TextAsset>("Pixel/runtime-catalog").text);
                if(doc.iterationArtOverrides!=null)foreach(var entry in doc.iterationArtOverrides){
                    if((entry.schema!="monster-motion-runtime-v13"&&entry.schema!="monster-motion-runtime-v18")||entry.batches==null)continue;
                    foreach(var batch in entry.batches){
                        HashSet<string> ids;
                        if(!motionScopes.TryGetValue(batch.runtimeId,out ids)){ids=new HashSet<string>();motionScopes.Add(batch.runtimeId,ids);}
                        if(batch.clips!=null)foreach(var clip in batch.clips)ids.Add(clip.id);
                    }
                }
                foreach(var ids in motionScopes.Values)expectedMotionClips+=ids.Count;
                if(motionScopes.Count==0)throw new InvalidOperationException("No reviewed motion clips in this build");
            }
            monsterArtReview=true;Application.runInBackground=true;
            selectedDungeon=previewDungeon=0;ConfigureRooms();
            typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{2});
            Enemy subject=game.enemies.Find(e=>e.kind=="benders");
            if(subject==null)throw new InvalidOperationException("Missing original boss room review subject");
            foreach(var enemy in game.enemies)enemy.active=enemy==subject;
            adventureStarted=true;openMenu="";pausedBeforeMenu=false;game.paused=true;game.roomBanner=0;
            game.player.x=subject.x-115;game.player.y=game.GroundAt(game.player.x);game.player.facing=1;
            shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();RenderMenus();
            hudCanvas.enabled=false;
            var probes=new List<MonsterArtProbe>();int monsterCount=0,clipCount=0;bool passed=true;
            foreach(var monster in catalog.monsters){
                if(monster.id.StartsWith("prop-")||motionOnly&&!motionScopes.ContainsKey(monster.id))continue;
                monsterCount++;subject.kind=monster.id;subject.facing=1;subject.visualScale=1;
                var lib=libraries[monster.id];
                foreach(var clip in monster.clips){
                    if(motionOnly&&!motionScopes[monster.id].Contains(clip.id))continue;
                    clipCount++;subject.clip=clip.id;subject.state=CombatState.Normal;subject.telegraph=0;
                    var distinct=new List<int>();var keys=new HashSet<string>();
                    for(int i=0;i<clip.frames.Length;i++){var f=clip.frames[i];string texture=!string.IsNullOrEmpty(f.texture)?f.texture:!string.IsNullOrEmpty(clip.texture)?clip.texture:monster.texture;string key=texture+","+f.x+","+f.y+","+f.w+","+f.h+","+f.anchorX+","+f.anchorY;if(keys.Add(key))distinct.Add(i);}
                    foreach(int requestedIndex in distinct){
                        subject.clock=(requestedIndex+.25f)/clip.fps;
                        yield return null;yield return new WaitForEndOfFrame();
                        var view=enemyViews[game.enemies.IndexOf(subject)];int actualIndex=Array.IndexOf(lib.frames[clip.id],view.sprite);
                        if(actualIndex<0)throw new InvalidOperationException("Unexpected sprite fallback "+monster.id+"/"+clip.id);
                        var actual=clip.frames[actualIndex];
                        bool same=view.sprite==lib.frames[clip.id][requestedIndex],isNew=motionOnly||monster.artVersion=="pixel-whole-pose-v07";
                        passed&=same&&isNew;
                        string capture="";
                        // Capture the complete idle, active strike, hurt, down, and
                        // rising silhouette; every other unique pose still gets a trace.
                        bool picture=(clip.id=="idle"&&requestedIndex==0)||(clip.id=="attack"&&distinct.Count>2&&requestedIndex==distinct[2])||(clip.id=="hurt"&&requestedIndex==distinct[0])||(clip.id=="down")||(clip.id=="get-up"&&distinct.Count>1&&requestedIndex==distinct[1]);
                        if(motionOnly)picture=requestedIndex==distinct[0]||requestedIndex==distinct[distinct.Count/2]||requestedIndex==distinct[distinct.Count-1];
                        if(picture){capture=(outskirtsOnly?"outskirts-":motionOnly?"motion-":"")+monster.id+"-"+clip.id+(motionOnly?"-"+requestedIndex.ToString("D3"):"");Capture(capture);}
                        probes.Add(new MonsterArtProbe{monster=monster.id,clip=clip.id,actualSprite=view.sprite.name,texture=view.sprite.texture.name,artVersion=monster.artVersion,
                            screenshot=string.IsNullOrEmpty(capture)?"":capture+".png",frameIndex=actualIndex,frameCount=clip.frames.Length,distinctPaintedRects=distinct.Count,
                            matchesExpected=same,newLibrary=isNew,anchorX=actual.anchorX,anchorY=actual.anchorY,scale=view.transform.localScale.x,rect=view.sprite.rect});
                        yield return null;
                    }
                }
                if(!motionOnly)foreach(var alias in new[]{new[]{"launched","launch"},new[]{"fall","launch"},new[]{"knockdown","down"},new[]{"getup","get-up"}}){
                    var resolved=lib.clips[alias[1]];subject.clip=alias[0];subject.state=CombatState.Normal;subject.clock=resolved.duration*.5f;
                    int expectedIndex=Mathf.Min(resolved.frames.Length-1,Mathf.FloorToInt(subject.clock*resolved.fps));
                    yield return null;yield return new WaitForEndOfFrame();
                    var view=enemyViews[game.enemies.IndexOf(subject)];int actualIndex=Array.IndexOf(lib.frames[alias[1]],view.sprite);
                    bool same=actualIndex==expectedIndex,isNew=monster.artVersion=="pixel-whole-pose-v07";passed&=same&&isNew;
                    var data=resolved.frames[Mathf.Max(0,actualIndex)];
                    probes.Add(new MonsterArtProbe{monster=monster.id,clip=alias[0]+" -> "+alias[1],actualSprite=view.sprite.name,texture=view.sprite.texture.name,artVersion=monster.artVersion,
                        frameIndex=actualIndex,frameCount=resolved.frames.Length,matchesExpected=same,newLibrary=isNew,anchorX=data.anchorX,anchorY=data.anchorY,scale=view.transform.localScale.x,rect=view.sprite.rect});
                    yield return null;
                }
            }
            var catalogJson=Resources.Load<TextAsset>("Pixel/runtime-catalog");string catalogHash;
            using(var hash=SHA256.Create())catalogHash=BitConverter.ToString(hash.ComputeHash(catalogJson.bytes)).Replace("-","").ToLowerInvariant();
            Directory.CreateDirectory(captureDir);
            File.WriteAllText(Path.Combine(captureDir,outskirtsOnly?"outskirts-art-runtime.json":motionOnly?"motion-art-runtime.json":"monster-art-runtime.json"),JsonUtility.ToJson(new MonsterArtRuntimeReport{
                passed=passed&&(motionOnly?monsterCount==motionScopes.Count&&clipCount==expectedMotionClips:monsterCount==13&&clipCount==156),monsters=monsterCount,clips=clipCount,poseSamples=probes.Count,
                checkedUtc=DateTime.UtcNow.ToString("o"),executable=System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName,catalogSha256=catalogHash,samples=probes.ToArray()},true));
            hudCanvas.enabled=true;monsterArtReview=false;
            Debug.Log("MONSTER_ART_RUNTIME_REVIEW_COMPLETE: "+monsterCount+" monsters, "+clipCount+" clips, "+probes.Count+" actual sprite probes, passed="+passed);
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--review-quit")>=0){yield return new WaitForSecondsRealtime(.5f);Application.Quit();}
            else{subject.kind="benders";subject.clip="idle";subject.clock=0;}
        }
    }
}
