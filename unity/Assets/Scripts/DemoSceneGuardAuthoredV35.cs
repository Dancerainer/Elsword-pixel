using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class GuardViewV35 {
            internal SpriteRenderer carrier;
            internal FairyAuthoredPartsV33 parts;
            internal SpriteRenderer[] members;
            internal readonly float[] depth=new float[GuardRigV35.PartCount];
        }
        sealed class GuardSelectionV35 {
            internal SpriteRenderer carrier;
            internal Sprite sprite;
            internal SourceWholeSelectionV21 selected;
            internal int frame;
        }
        const string GuardNpcV35="NUI_NASOD_GUARD", GuardKindV35="nasod-guard";
        readonly Dictionary<Enemy,GuardViewV35> guardViewsV35=new Dictionary<Enemy,GuardViewV35>();
        readonly Dictionary<Enemy,GuardSelectionV35> guardSelectionsV35=new Dictionary<Enemy,GuardSelectionV35>();
        readonly HashSet<Enemy> guardSeenV35=new HashSet<Enemy>();
        readonly HashSet<Enemy> guardFallbackV35=new HashSet<Enemy>();
        readonly List<Enemy> guardStaleV35=new List<Enemy>();
        FairyPoseLibraryV33 guardLibraryV35;
        GuardDepthV35 guardDepthV35;
        bool guardUnavailableV35;

        void RecordGuardSelectionV35(Enemy e,SpriteRenderer carrier,SourceWholeSelectionV21 selected) {
            if(MonsterPixelOwnsActorV53(e))return;
            if(e==null||e.sourceNPC!=GuardNpcV35||e.kind!=GuardKindV35||!FactoryProductSessionActiveR1179||!carrier||!carrier.sprite||selected==null)return;
            if(!guardSelectionsV35.TryGetValue(e,out var current)){current=new GuardSelectionV35();guardSelectionsV35.Add(e,current);}
            current.selected=selected;current.carrier=carrier;current.sprite=carrier.sprite;current.frame=Time.frameCount;
        }
        bool GuardOwnsRendererV35(Renderer r){
            foreach(var entry in guardViewsV35.Values)if(entry.parts.OwnsRenderer(r))return true;
            return false;
        }
        void DropGuardV35(Enemy e) {
            if(!guardViewsV35.TryGetValue(e,out var v))return;
            v.parts.Dispose();guardViewsV35.Remove(e);
        }
        void DisposeGuardViewsV35() {
            foreach(var entry in guardViewsV35.Values)entry.parts.Dispose();
            guardViewsV35.Clear();guardSelectionsV35.Clear();guardSeenV35.Clear();guardFallbackV35.Clear();guardStaleV35.Clear();
            if(guardLibraryV35!=null)guardLibraryV35.ReleaseOwned(DestroyNativeWorldObject);
            guardLibraryV35=null;guardDepthV35=null;guardUnavailableV35=false;
        }
        void UpdateGuardViewsV35() {
            if(!FactoryProductSessionActiveR1179||game==null||!actorsRoot||!worldRoot){
                if(guardLibraryV35!=null||guardSelectionsV35.Count>0||guardUnavailableV35)DisposeGuardViewsV35();
                return;
            }
            if(guardSelectionsV35.Count>0&&guardLibraryV35==null&&!guardUnavailableV35) {
                try{guardLibraryV35=GuardRigV35.Load();guardDepthV35=GuardDepthV35.Load(guardLibraryV35);}
                catch(Exception ex){if(guardLibraryV35!=null)guardLibraryV35.ReleaseOwned(DestroyNativeWorldObject);guardLibraryV35=null;guardUnavailableV35=true;Debug.LogWarning("GUARD_RIG_FALLBACK_V35 "+ex.Message);}
            }
            guardSeenV35.Clear();
            if(guardLibraryV35!=null)for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var e=game.enemies[i];if(MonsterPixelOwnsActorV53(e)){DropGuardV35(e);continue;}if(e.sourceNPC!=GuardNpcV35||e.kind!=GuardKindV35)continue;
                var carrier=enemyViews[i];
                if(!guardSelectionsV35.TryGetValue(e,out var w)||w.frame!=Time.frameCount||!carrier||carrier!=w.carrier||carrier.sprite!=w.sprite||guardFallbackV35.Contains(e)){DropGuardV35(e);continue;}
                guardSeenV35.Add(e);
                try{
                    if(!GuardRigV35.TryFrame(guardLibraryV35,w.selected,e,carrier,out var f)){DropGuardV35(e);continue;}
                    if(guardViewsV35.TryGetValue(e,out var view)&&view.carrier!=carrier){DropGuardV35(e);view=null;}
                    if(view==null){view=new GuardViewV35{carrier=carrier,parts=new FairyAuthoredPartsV33(carrier,guardLibraryV35,DestroyNativeWorldObject)};guardViewsV35.Add(e,view);var members=new List<SpriteRenderer>(GuardRigV35.PartCount);view.parts.VisitRenderers(members.Add);view.members=members.ToArray();}
                    var at=RenderEnemyPosition(e);
                    var world=actorsRoot.TransformPoint(new Vector3(at.x,at.y,0));
                    var foot=NativeWorldActive?ProjectNativePoint(world,e.nativeDepth):world;
                    view.parts.Apply(f,foot);
                    guardDepthV35.Apply(f.alias,f.sourceClock,view.members,view.depth);
                    view.parts.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,world,e.nativeDepth));
                    if(e.hp>0&&i<enemyBars.Count&&enemyBars[i]&&enemyBars[i].enabled&&view.parts.TryVisibleTop(out var top)){
                        var p=enemyBars[i].transform.position;p.y=top+8;enemyBars[i].transform.position=p;
                        if(i<enemyDebuffViews.Count&&enemyDebuffViews[i]){p=enemyDebuffViews[i].transform.position;p.y=top+20;enemyDebuffViews[i].transform.position=p;}
                    }
                }catch(Exception ex){DropGuardV35(e);guardFallbackV35.Add(e);Debug.LogWarning("GUARD_OWNER_FALLBACK_V35 "+ex.Message);}
            }
            guardStaleV35.Clear();foreach(var e in guardViewsV35.Keys)if(!guardSeenV35.Contains(e))guardStaleV35.Add(e);
            foreach(var e in guardStaleV35)DropGuardV35(e);
            guardStaleV35.Clear();foreach(var e in guardFallbackV35)if(!game.enemies.Contains(e))guardStaleV35.Add(e);
            foreach(var e in guardStaleV35)guardFallbackV35.Remove(e);
            guardSelectionsV35.Clear();
        }
    }
}

