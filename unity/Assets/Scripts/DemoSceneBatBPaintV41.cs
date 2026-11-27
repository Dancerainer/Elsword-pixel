using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class BatBFrameV41 { public int x,y,w,h; public float ax,ay; }
        [Serializable] sealed class BatBClipV41 { public string id,motion; public float fps,duration; public bool loop; public BatBFrameV41[] frames; }
        [Serializable] sealed class BatBBookV41 { public string id,npc,modelSha,atlasSha; public BatBClipV41[] clips; }
        readonly Dictionary<Sprite,Sprite> batBPaintV41=new Dictionary<Sprite,Sprite>();
        readonly Dictionary<SpriteRenderer,Sprite> batBDisplayedV41=new Dictionary<SpriteRenderer,Sprite>();
        bool batBLoadedV41;

        void LoadBatBPaintV41(Library lib){
            if(batBLoadedV41)return;
            const string path="Pixel/feedback-v41/bat-b/";
            var text=Resources.Load<TextAsset>(path+"motions");
            var page=Resources.Load<Texture2D>(path+"bat-b-painted-v41");
            var book=text?JsonUtility.FromJson<BatBBookV41>(text.text):null;
            if(!page||book==null||book.id!="bat-b"||book.npc!="NUI_BAT_B"||book.clips==null||book.clips.Length!=15)
                throw new InvalidOperationException("Incomplete Bat B artwork.");
            page.filterMode=FilterMode.Point;
            foreach(var c in book.clips){
                if(!lib.clips.TryGetValue(c.id,out var src)||!lib.frames.TryGetValue(c.id,out var selected)||
                    c.frames==null||selected.Length!=c.frames.Length||Mathf.Abs(c.duration-src.duration)>1e-5f||
                    Mathf.Abs(c.fps-src.fps)>1e-5f||c.loop!=src.loop)throw new InvalidOperationException("Bat B source timing: "+c.id);
                for(int i=0;i<selected.Length;i++){
                    var f=c.frames[i];if(f.w<=0||f.h<=0||f.x<0||f.y<0||f.x+f.w>page.width||f.y+f.h>page.height)
                        throw new InvalidOperationException("Bat B art rect: "+c.id+"/"+i);
                    var pic=Sprite.Create(page,new Rect(f.x,page.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                    pic.name=selected[i].name; spriteOffsets[pic]=new Vector2(f.w*.5f-f.ax,f.ay-f.h*.5f);
                    batBPaintV41.Add(selected[i],pic);
                }
            }
            batBLoadedV41=true;
        }
        void RestoreBatBPaintV41(){
            foreach(var p in batBDisplayedV41)if(p.Key)p.Key.sprite=p.Value;
            batBDisplayedV41.Clear();
        }
        void DisposeBatBPaintV41(){
            RestoreBatBPaintV41();
            foreach(var p in batBPaintV41.Values)if(p){spriteOffsets.Remove(p);nativeUiVisibleLocalRectsV28.Remove(p);DestroyNativeWorldObject(p);}
            batBPaintV41.Clear();batBLoadedV41=false;
        }
        void ApplyBatBPaintV41(){
            if(game==null||(game.paused&&openMenu=="dungeons")||!First3CombatScopeV28())return;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var e=game.enemies[i];var v=enemyViews[i];
                if(MonsterPixelOwnsActorV53(e))continue;
                if(e==null||e.kind!="bat-b"||e.sourceNPC!="NUI_BAT_B"||!v||!v.enabled||!v.sprite)continue;
                var source=v.sprite;
                if(!source.name.StartsWith("bat-b/source-",StringComparison.Ordinal))continue;
                LoadBatBPaintV41(libraries[e.kind]);
                if(!batBPaintV41.TryGetValue(source,out var painted))throw new InvalidOperationException("Unpainted Bat B motion: "+source.name);
                // All source clock, ceiling/Bip and native UI registration has already finished.
                // Leave the actor, material, depth, fade, facing and hits on their existing carrier.
                var previous=SpriteOffset(v);batBDisplayedV41[v]=source;v.sprite=painted;
                var delta=SpriteOffset(v)-previous;
                var p=v.transform.localPosition;p.x+=delta.x*v.transform.localScale.x;p.y+=delta.y*v.transform.localScale.y;v.transform.localPosition=p;
                if(e.hp>0&&enemyBars[i]&&enemyBars[i].enabled){
                    var bar=enemyBars[i].transform.position;
                    bar.x=v.bounds.center.x;bar.y=v.bounds.max.y+8;enemyBars[i].transform.position=bar;
                    bar.y+=12;enemyDebuffViews[i].transform.position=bar;
                }
            }
        }
    }
}
