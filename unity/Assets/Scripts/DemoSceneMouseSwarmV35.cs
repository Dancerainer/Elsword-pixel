using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class MouseSelectionV35 {internal SpriteRenderer carrier;internal string alias;internal float clock;internal int frame;internal string plainMotion,plainPose;internal Sprite selectedSprite;}
        sealed class MouseViewV35 {
            internal SpriteRenderer carrier;internal FairyAuthoredPartsV33 parts;internal SpriteRenderer[] members;
            internal readonly float[] depths=new float[5];internal readonly int[] keys=new int[5];
            internal string actualAlias,motion,plainSourcePose;internal float originalClock,sourceClock;
        }
        readonly Dictionary<Enemy,MouseSelectionV35> mouseSelectionsV35=new Dictionary<Enemy,MouseSelectionV35>();
        readonly Dictionary<Enemy,MouseViewV35> mouseViewsV35=new Dictionary<Enemy,MouseViewV35>();
        readonly HashSet<Enemy> mouseSeenV35=new HashSet<Enemy>(),mouseFallbackV35=new HashSet<Enemy>();
        readonly List<Enemy> mouseStaleV35=new List<Enemy>();
        MouseRigV35 mouseRigV35;bool mouseFailedV35;string mouseFamilyV35;

        bool MouseRoomV35(){var s=CurrentNpcContentSet;return s!=null&&s.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal";}
        void RecordMouseSelectionV35(Enemy enemy,SpriteRenderer carrier,string alias,float clock){
            if(MonsterPixelOwnsActorV53(enemy))return;
            if(!MouseRigV35.Identity(enemy)||!MouseRoomV35()||!carrier||!carrier.sprite)return;
            if(!mouseSelectionsV35.TryGetValue(enemy,out var s)){s=new MouseSelectionV35();mouseSelectionsV35.Add(enemy,s);}
            s.carrier=carrier;s.alias=alias;s.clock=clock;s.frame=Time.frameCount;s.plainMotion=null;s.plainPose=null;s.selectedSprite=carrier.sprite;
        }
        bool MouseOwnsRendererV35(Renderer renderer){return mouseFamilyV35!="plain"&&RegisteredMouseOwnsRendererV35(renderer);} 
        bool RegisteredMouseOwnsRendererV35(Renderer renderer){foreach(var e in mouseViewsV35.Values)if(e.parts.OwnsRenderer(renderer))return true;return false;}
        void DropMouseV35(Enemy e){if(mouseViewsV35.TryGetValue(e,out var v)){v.parts.Dispose();mouseViewsV35.Remove(e);}}
        void DisposeMouseViewsV35(){
            foreach(var v in mouseViewsV35.Values)v.parts.Dispose();mouseViewsV35.Clear();mouseSelectionsV35.Clear();mouseSeenV35.Clear();mouseFallbackV35.Clear();mouseStaleV35.Clear();
            if(mouseRigV35!=null)mouseRigV35.library.ReleaseOwned(DestroyNativeWorldObject);mouseRigV35=null;mouseFailedV35=false;mouseFamilyV35=null;
        }
        bool MouseFrameV35(Enemy e,SpriteRenderer carrier,MouseSelectionV35 witness,out string motion,out float clock){
            motion=null;clock=0;
            if(PlainMouseSourceV35(e)){
                if(witness.frame!=Time.frameCount||!carrier||carrier!=witness.carrier||!carrier.sprite||carrier.sprite!=witness.selectedSprite||string.IsNullOrEmpty(witness.plainPose))return false;
                motion=witness.plainMotion;clock=witness.clock;return motion!=null&&mouseRigV35.library.Find(motion)!=null&&clock>=0&&!float.IsInfinity(clock)&&!float.IsNaN(clock);
            }
            if(witness.frame!=Time.frameCount||!carrier||witness.carrier!=carrier||!carrier.sprite||!libraries.TryGetValue(e.kind,out var library)||
                !library.frames.TryGetValue(witness.alias,out var frames)||!library.clips.TryGetValue(witness.alias,out var clip))return false;
            int slash=carrier.sprite.name.LastIndexOf('/');string prefix=e.kind+"/"+witness.alias+"/";
            if(slash!=prefix.Length-1||!carrier.sprite.name.StartsWith(prefix,StringComparison.Ordinal)||
                !int.TryParse(carrier.sprite.name.Substring(slash+1),out var ix)||ix<0||ix>=frames.Length||!ReferenceEquals(frames[ix],carrier.sprite))return false;
            float original=clip.loop?Mathf.Repeat(witness.clock,clip.duration):witness.clock;
            if(ix!=Math.Min(frames.Length-1,Mathf.FloorToInt(original*clip.fps)))return false;
            return mouseRigV35.TryMotion(e,witness.alias,witness.clock,clip.duration,clip.loop,out motion,out clock);
        }
        void UpdateMouseViewsV35(){if(!PlainMouseRoomV35())UpdateRegisteredMouseViewsV35(false);}
        void UpdateRegisteredMouseViewsV35(bool plain){
            string family=plain?"plain":"waterway";
            if(mouseFamilyV35!=null&&mouseFamilyV35!=family)DisposeMouseViewsV35();
            if(game==null||!actorsRoot||!worldRoot||!plain&&!MouseRoomV35()){
                if(mouseRigV35!=null||mouseFailedV35||mouseSelectionsV35.Count>0)DisposeMouseViewsV35();return;
            }
            if(mouseRigV35==null&&!mouseFailedV35&&mouseSelectionsV35.Count>0){
                try{mouseFamilyV35=family;mouseRigV35=plain?LoadPlainMouseV35():MouseRigV35.Load();}catch(Exception ex){mouseFailedV35=true;Debug.LogWarning("MOUSE_RIG_FALLBACK_V35 "+ex.Message);}
            }
            mouseSeenV35.Clear();
            if(mouseRigV35!=null)for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var e=game.enemies[i];if(MonsterPixelOwnsActorV53(e)){DropMouseV35(e);continue;}if(!(plain?PlainMouseSourceV35(e):MouseRigV35.Identity(e)))continue;
                var carrier=enemyViews[i];
                if(!mouseSelectionsV35.TryGetValue(e,out var w)||mouseFallbackV35.Contains(e)||!MouseFrameV35(e,carrier,w,out var motion,out var clock)){DropMouseV35(e);continue;}
                mouseSeenV35.Add(e);
                try {
                    if(mouseViewsV35.TryGetValue(e,out var v)&&v.carrier!=carrier){DropMouseV35(e);v=null;}
                    if(v==null){v=new MouseViewV35{carrier=carrier,parts=new FairyAuthoredPartsV33(carrier,mouseRigV35.library,DestroyNativeWorldObject)};
                        mouseViewsV35.Add(e,v);var list=new List<SpriteRenderer>(5);v.parts.VisitRenderers(list.Add);v.members=list.ToArray();}
                    var local=RenderEnemyPosition(e);var world=actorsRoot.TransformPoint(new Vector3(local.x,local.y,0));
                    var foot=NativeWorldActive?ProjectNativePoint(world,e.nativeDepth):world;
                    v.actualAlias=w.alias;v.plainSourcePose=w.plainPose;v.originalClock=w.clock;v.motion=motion;v.sourceClock=clock;
                    v.parts.Apply(new FairyAuthoredPartsV33.Frame{alias=motion,sourceClock=clock,facing=e.facing,visible=e.active&&!e.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy},foot);
                    mouseRigV35.ApplyVariants(motion,clock,v.members,v.depths,v.keys);
                    foreach(var child in v.members)BindNativeSceneOcclusionR1461(child,world,e.nativeDepth);
                    if(e.hp>0&&i<enemyBars.Count&&enemyBars[i]&&enemyBars[i].enabled&&mouseRigV35.VisibleTop(v.members,v.keys,out float top)){
                        var p=enemyBars[i].transform.position;p.y=top+8;enemyBars[i].transform.position=p;
                        if(i<enemyDebuffViews.Count&&enemyDebuffViews[i]){p=enemyDebuffViews[i].transform.position;p.y=top+20;enemyDebuffViews[i].transform.position=p;}
                    }
                }catch(Exception ex){DropMouseV35(e);mouseFallbackV35.Add(e);Debug.LogWarning("MOUSE_OWNER_FALLBACK_V35 "+ex.Message);}
            }
            mouseStaleV35.Clear();foreach(var e in mouseViewsV35.Keys)if(!mouseSeenV35.Contains(e))mouseStaleV35.Add(e);
            foreach(var e in mouseStaleV35)DropMouseV35(e);
            mouseStaleV35.Clear();foreach(var e in mouseFallbackV35)if(!game.enemies.Contains(e))mouseStaleV35.Add(e);
            foreach(var e in mouseStaleV35)mouseFallbackV35.Remove(e);
            mouseSelectionsV35.Clear();
        }
    }
}

