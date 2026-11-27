using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] class BendersArtSample {
            public string clip,sprite,texture,catalogTexture,screenshot;
            public int frameCount;
            public float scale,anchorX,anchorY;
            public Rect spriteRect;
            public Vector3 position;
            public bool isReviewedIdle;
        }
        [Serializable] class BendersArtReport {
            public string checkedUtc,executable,catalogSha256;
            public bool idleUsesReviewedTexture,actionsStillUseOriginal;
            public string scope="Actual Windows Player renderer capture. Only idle is replaced; walk and attack remain original. No animation approval.";
            public BendersArtSample[] samples;
        }

        // Explicit diagnostic entry only. Normal dungeon selection and AI are unchanged.
        IEnumerator ReviewBendersArt(){
            Application.runInBackground=true;
            selectedDungeon=previewDungeon=0;ConfigureRooms();
            typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{2});
            Enemy boss=game.enemies.Find(e=>e.kind=="benders");
            if(boss==null)throw new InvalidOperationException("Benders review requires the actual Tree of El boss spawn.");
            adventureStarted=true;openMenu="";pausedBeforeMenu=false;game.paused=true;game.roomBanner=0;
            game.player.x=boss.x-115;game.player.y=game.GroundAt(game.player.x);game.player.facing=1;
            shownRoom=previousRoom=cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();RenderMenus();
            var samples=new List<BendersArtSample>();
            foreach(string clip in new[]{"idle","walk","attack"}){
                boss.clip=clip;boss.clock=clip=="idle"?0:.25f;boss.facing=-1;
                // Let the normal production Update render this exact model state.
                yield return null;yield return new WaitForEndOfFrame();
                string name="benders-"+clip;
                Capture(name);
                var view=enemyViews[game.enemies.IndexOf(boss)];
                var lib=libraries["benders"];var data=lib.clips[clip];var frameData=data.frames[Array.IndexOf(lib.frames[clip],view.sprite)];
                samples.Add(new BendersArtSample{clip=clip,sprite=view.sprite.name,texture=view.sprite.texture.name,
                    catalogTexture=string.IsNullOrEmpty(data.texture)?lib.data.texture:data.texture,
                    screenshot=name+".png",frameCount=data.frames.Length,scale=view.transform.localScale.x,
                    anchorX=frameData.anchorX,anchorY=frameData.anchorY,spriteRect=view.sprite.rect,
                    position=view.transform.position,isReviewedIdle=clip=="idle"&&view.sprite.texture.name=="idle-lk-v07"});
                yield return null;
            }
            var json=Resources.Load<TextAsset>("Pixel/runtime-catalog");
            string catalogHash;using(var hash=SHA256.Create())catalogHash=BitConverter.ToString(hash.ComputeHash(json.bytes)).Replace("-","").ToLowerInvariant();
            File.WriteAllText(Path.Combine(captureDir,"benders-art-runtime.json"),JsonUtility.ToJson(new BendersArtReport{
                checkedUtc=DateTime.UtcNow.ToString("o"),executable=System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName,
                catalogSha256=catalogHash,samples=samples.ToArray(),idleUsesReviewedTexture=samples[0].isReviewedIdle,
                actionsStillUseOriginal=samples[1].catalogTexture!=samples[0].catalogTexture&&samples[2].catalogTexture!=samples[0].catalogTexture},true));
            boss.clip="idle";boss.clock=0;
            Debug.Log("BENDERS_ART_REVIEW_COMPLETE: idle-only replacement; original walk/attack. Report: "+captureDir);
        }
    }
}
