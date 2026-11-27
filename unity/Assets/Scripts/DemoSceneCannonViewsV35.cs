using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class CannonFrameV35 {
            public Sprite sprite;
            public SpriteRenderer carrier;
            public string alias,viewStatus;
            public float clock;
            public int number,projectionFrame;
        }
        sealed class CannonViewV35 {
            public SpriteRenderer carrier;
            public FairyAuthoredPartsV33 parts;
        }
        readonly Dictionary<Enemy,CannonFrameV35> cannonFramesV35 = new Dictionary<Enemy,CannonFrameV35>();
        readonly Dictionary<Enemy,CannonViewV35> cannonViewsV35 = new Dictionary<Enemy,CannonViewV35>();
        readonly Dictionary<string,FairyPoseLibraryV33> cannonBanksV35 = new Dictionary<string,FairyPoseLibraryV33>();
        readonly HashSet<Enemy> cannonSeenV35 = new HashSet<Enemy>();
        readonly HashSet<Enemy> cannonFailedV35 = new HashSet<Enemy>();
        readonly List<Enemy> cannonStaleV35 = new List<Enemy>();

        bool CannonScopeV35(Enemy actor) {
            return actor!=null && (BattleProductSessionActiveR1325 ||
                (BelowProductRouteActiveR1130 && actor.sourceNPC=="NUI_CANNON_MINI")) &&
                CannonRigV35.Kind(actor.sourceNPC)==actor.kind && actor.kind!=null;
        }
        void SelectCannonDeathV35(Enemy actor, ref string alias, ref float clock) {
            if(!CannonScopeV35(actor) || actor.hp>0 || actor.state!=CombatState.Dead ||
                !string.IsNullOrEmpty(actor.npcState) || !game.Grounded(actor))return;
            // Both original land-death states are in the pinned route library. The
            // Big gate's own grounded clock pauses exactly when the original actor pauses.
            if(actor.sourceNPC=="NUI_CANNON_BIG") {
                float t=game.AlteraCannonDyingLandClock(actor);
                if(t>=0){alias="source-down";clock=t;}
            } else if(actor.sourceNPC=="NUI_CANNON_HEAVY_MACHINE_GUN") {
                alias="broken";clock=actor.clock;
            }
        }
        // Pose() just selected the original registered sprite. Preserve its exact clock,
        // including its existing fallback to idle for an unmapped generic reaction.
        void RecordCannonPoseV35(Enemy actor, SpriteRenderer carrier, string alias, float clock) {
            if (!CannonScopeV35(actor) || !carrier || !carrier.sprite)return;
            // Called only by Pose() after selection of this exact live library frame.
            // Sprite names are presentation labels and are not used as an asset ID.
            var sprite=carrier.sprite;float t=clock;
            if (!cannonFramesV35.TryGetValue(actor,out var f)) {
                f=new CannonFrameV35();cannonFramesV35.Add(actor,f);
            }
            f.carrier=carrier;f.sprite=sprite;f.alias=alias;f.clock=Mathf.Max(0,t);f.number=Time.frameCount;
        }
        bool CannonOwnsRendererV35(Renderer r) {
            foreach (var v in cannonViewsV35.Values) if(v.parts.OwnsRenderer(r)) return true;
            return false;
        }
        void ReleaseCannonV35(Enemy actor) {
            if (cannonViewsV35.TryGetValue(actor,out var v)) {
                cannonViewsV35.Remove(actor);v.parts.Dispose();
            }
        }
        void DisposeCannonViewsV35() {
            foreach(var v in cannonViewsV35.Values)v.parts.Dispose();
            cannonViewsV35.Clear();cannonFramesV35.Clear();cannonFailedV35.Clear();
            foreach(var bank in cannonBanksV35.Values)bank.ReleaseOwned(DestroyNativeWorldObject);
            cannonBanksV35.Clear();cannonSeenV35.Clear();cannonStaleV35.Clear();
        }
        void UpdateCannonViewsV35() {
            if(game==null || !actorsRoot || (!BattleProductSessionActiveR1325 && !BelowProductRouteActiveR1130)) {
                if(cannonViewsV35.Count+cannonBanksV35.Count+cannonFramesV35.Count>0)DisposeCannonViewsV35();
                return;
            }
            cannonSeenV35.Clear();
            for (int i=0;i<game.enemies.Count && i<enemyViews.Count;i++) {
                var actor=game.enemies[i];
                if(MonsterPixelOwnsActorV53(actor)){ReleaseCannonV35(actor);continue;}
                if (!CannonScopeV35(actor)) continue;
                cannonSeenV35.Add(actor);
                try {
                    var s=enemyViews[i];
                    if(!cannonFramesV35.TryGetValue(actor,out var f)){ReleaseCannonV35(actor);continue;}
                    f.projectionFrame=Time.frameCount;
                    if(cannonFailedV35.Contains(actor))continue;
                    // Pose owns the canonical Battle/Below library frame and source
                    // clock. Projection owns this same actor's original renderer.
                    if(!s || f.carrier!=s || f.number!=Time.frameCount || !MonsterTextureRedrawV22.SameSourceFrameV35(s.sprite,f.sprite)) {
                        f.viewStatus=!s?"missing-view":f.carrier!=s?"different-renderer":f.number!=Time.frameCount?"earlier-selection":"unregistered-visible-sprite";
                        ReleaseCannonV35(actor);continue;
                    }
                    f.viewStatus="original-carrier";
                    if(!cannonBanksV35.TryGetValue(actor.sourceNPC,out var lib)) {
                        lib=CannonRigV35.Load(actor.sourceNPC);cannonBanksV35.Add(actor.sourceNPC,lib);
                    }
                    var track=lib.Find(f.alias);
                    var original=libraries[actor.kind].clips[f.alias];
                    if(track==null || Mathf.Abs(track.duration-original.duration)>.00002f || track.loop!=original.loop)
                        throw new InvalidOperationException("Original cannon carrier clock "+actor.kind+"/"+f.alias);
                    if(cannonViewsV35.TryGetValue(actor,out var v) && v.carrier!=s){ReleaseCannonV35(actor);v=null;}
                    if(v==null) {
                        v=new CannonViewV35{carrier=s,parts=new FairyAuthoredPartsV33(s,lib,DestroyNativeWorldObject)};
                        cannonViewsV35.Add(actor,v);
                    }
                    var local=RenderEnemyPosition(actor);
                    var world=actorsRoot.TransformPoint(new Vector3(local.x,local.y,0));
                    var projected=NativeWorldActive?ProjectNativePoint(world,actor.nativeDepth):world;
                    v.parts.Apply(new FairyAuthoredPartsV33.Frame{alias=f.alias,sourceClock=f.clock,
                        facing=s.flipX?-1:1,visible=s.enabled && s.gameObject.activeInHierarchy},projected);
                    if(NativeWorldActive)v.parts.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,world,actor.nativeDepth));
                    if(actor.hp>0 && enemyBars[i].enabled && v.parts.TryVisibleTop(out var top)) {
                        var bar=enemyBars[i].transform.position;bar.y=top+8;enemyBars[i].transform.position=bar;
                        var label=enemyDebuffViews[i].transform.position;label.y=top+20;enemyDebuffViews[i].transform.position=label;
                    }
                } catch(Exception ex) {
                    ReleaseCannonV35(actor);cannonFailedV35.Add(actor);
                    if(cannonFramesV35.TryGetValue(actor,out var failure))failure.viewStatus="failure: "+ex.Message;
                    Debug.LogWarning("CANNON_SOURCE_FALLBACK_V35 "+actor.sourceNPC+" "+ex.Message);
                }
            }
            cannonStaleV35.Clear();
            foreach(var actor in cannonFramesV35.Keys)if(!cannonSeenV35.Contains(actor))cannonStaleV35.Add(actor);
            foreach(var actor in cannonStaleV35){ReleaseCannonV35(actor);cannonFramesV35.Remove(actor);cannonFailedV35.Remove(actor);}
            if(cannonSeenV35.Count==0 && cannonBanksV35.Count>0)DisposeCannonViewsV35();
        }
    }
}
