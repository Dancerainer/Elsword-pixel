using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Complete source poses for the longer holds in the early Besma whole-pose banks.
    // BuildAnimationFrames consumes only this visual sample. The live combat clip still owns its clock.
    internal static class SourceMotionInkV45 {
        [Serializable] internal sealed class Bank {
            public string id, sourceNPC, sourceSha256, artVersion;
            public int pageCount;
            public Frame[] frames;
            public Track[] clips;
        }
        [Serializable] internal sealed class Frame {
            public int page,x,y,w,h,anchorX,anchorY;
        }
        [Serializable] internal sealed class Track {
            public string alias,source;
            public int[] indices;
        }
        [Serializable] sealed class Index {public string[] ids;}
        static readonly Dictionary<string,Bank> loaded=new Dictionary<string,Bank>(StringComparer.Ordinal);
        static HashSet<string> ids;

        internal static Bank Library(string kind) {
            if(ids==null) {
                var index=Resources.Load<TextAsset>("Pixel/motion-v45/roster");
                if(!index)throw new InvalidOperationException("Missing Besma motion index");
                ids=new HashSet<string>(JsonUtility.FromJson<Index>(index.text).ids,StringComparer.Ordinal);
            }
            if(!ids.Contains(kind))return null;
            if(loaded.TryGetValue(kind,out Bank cached))return cached;
            string root="Pixel/motion-v45/"+kind+"/";
            var text=Resources.Load<TextAsset>(root+"library");
            if(!text)throw new InvalidOperationException("Missing Besma registered motion " +kind);
            var bank=JsonUtility.FromJson<Bank>(text.text);
            if(bank.id!=kind||bank.sourceSha256==null||bank.sourceSha256.Length!=64||
                bank.pageCount<1||bank.frames==null||bank.frames.Length==0||bank.clips==null)
                throw new InvalidOperationException("Invalid Besma registered motion "+kind);
            loaded.Add(kind,bank);
            return bank;
        }

        internal static bool UsesScorpionPages(string id){return id=="scorpion"||id=="scorpion-fire";}

        internal static AnimationData For(SpriteLibraryData data,AnimationData clock) {
            var bank=Library(data.id);
            if(bank==null||bank.artVersion!=data.artVersion)return null;
            var track=Array.Find(bank.clips,t=>t.alias==clock.id);
            if(track==null)return null;
            if(track.indices.Length!=clock.frames.Length)throw new InvalidOperationException("Source exposure changed "+data.id+"/"+clock.id);
            var frames=new FrameData[clock.frames.Length];
            var unique=new Dictionary<int,FrameData>();
            for(int i=0;i<frames.Length;i++) {
                int n=track.indices[i];
                if(!unique.TryGetValue(n,out FrameData picture)) {
                    if(n<0||n>=bank.frames.Length)throw new InvalidOperationException("Invalid pose "+data.id+"/"+clock.id);
                    var f=bank.frames[n];
                    if(f.page<0||f.page>=bank.pageCount||f.x<0||f.y<0||f.w<=0||f.h<=0)throw new InvalidOperationException("Invalid motion frame rectangle");
                    picture=new FrameData {texture="Pixel/motion-v45/"+data.id+"/page-"+f.page,x=f.x,y=f.y,w=f.w,h=f.h,
                                           hasAnchor=true,anchorX=f.anchorX,anchorY=f.anchorY};
                    unique.Add(n,picture);
                }
                frames[i]=picture;
            }
            // Both scorpion banks cover all model clips. Avoid loading the unused legacy atlas as a fallback.
            string fallback=UsesScorpionPages(data.id)?frames[0].texture:data.texture;
            return new AnimationData {id=clock.id,texture=fallback,fps=clock.fps,duration=clock.duration,loop=clock.loop,frames=frames};
        }

        internal static string ResourceFor(string kind,string clip,int index){
            var bank=Library(kind);if(bank==null)return null;
            var track=Array.Find(bank.clips,t=>t.alias==clip);
            if(track==null||index<0||index>=track.indices.Length)return null;
            return "Pixel/motion-v45/"+kind+"/page-"+bank.frames[track.indices[index]].page;
        }
    }

    public sealed partial class DemoScene {
        // All source pages and projected PNGs are verified tight at alpha > 0 by both installers.
        void RegisterTightMotionBoundsV45(Sprite sprite){
            Rect rect=sprite.rect;Vector2 pivot=sprite.pivot;float ppu=sprite.pixelsPerUnit;
            nativeUiVisibleLocalRectsV28[sprite]=new Rect(-pivot.x/ppu,-pivot.y/ppu,rect.width/ppu,rect.height/ppu);
        }

        bool BuildPrivateMotionV45(Library lib,AnimationData clock,List<UnityEngine.Object> owner){
            var drawn=SourceMotionInkV45.For(lib.data,clock);
            if(drawn==null)return false;
            var sprites=new Sprite[clock.frames.Length];
            var unique=new Dictionary<FrameData,Sprite>();
            var pages=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
            for(int i=0;i<sprites.Length;i++){
                var f=drawn.frames[i];
                if(!unique.TryGetValue(f,out Sprite sprite)){
                    if(!pages.TryGetValue(f.texture,out Texture2D page)){
                        page=Resources.Load<Texture2D>(f.texture);if(!page)throw new InvalidOperationException("Missing private source-motion page "+f.texture);
                        pages.Add(f.texture,page);
                    }
                    if(f.x+f.w>page.width||f.y+f.h>page.height)throw new InvalidOperationException("Private source pose outside "+f.texture);
                    sprite=Sprite.Create(page,new Rect(f.x,page.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                    owner.Add(sprite);RegisterTightMotionBoundsV45(sprite);sprite.name=lib.data.id+"/"+clock.id+"/"+i;
                    spriteOffsets[sprite]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);unique.Add(f,sprite);
                }
                sprites[i]=sprite;
            }
            lib.frames.Add(clock.id,sprites);lib.clips.Add(clock.id,clock);
            return true;
        }

        bool UseElderAttackInkV45(Enemy actor,SpriteRenderer carrier,string selected,float clock) {
            string kind=actor.kind;
            if(kind!="soldier-fat"&&kind!="thief-fat"&&kind!="thief-fat-boss")return false;
            string alias=selected.StartsWith("source-",StringComparison.Ordinal)?selected.Substring(7):selected;
            if(alias!="attack-a-ready"&&alias!="attack-a")return false;
            if(!libraries.TryGetValue(kind,out var bank)||!bank.clips.TryGetValue(selected,out var original)||
                !bank.clips.TryGetValue(alias,out var drawn)||!bank.frames.TryGetValue(alias,out var frames))return false;
            if(original.duration<=0||drawn.duration<=0||frames.Length==0)return false;
            // The compiled source and the gameplay alias may have different speeds.
            // Resolve exactly the same normalized phase as the existing upper-body owner.
            float phase=original.loop?Mathf.Repeat(clock,original.duration):Mathf.Clamp(clock,0,original.duration);
            int i=Mathf.Clamp(Mathf.FloorToInt(phase/original.duration*drawn.duration*drawn.fps+.00001f),0,frames.Length-1);
            if(SourceMotionInkV45.ResourceFor(kind,alias,i)==null)return false;
            DropReusablePaintV37(actor);
            paintSelectionsV37.Remove(actor);
            carrier.sprite=frames[i];
            return true;
        }
    }
}
