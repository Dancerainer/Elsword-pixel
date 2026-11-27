using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class BatViewV34 {
            public SpriteRenderer carrier;
            public BatAuthoredPartsV34 parts;
        }
        readonly Dictionary<Enemy,BatViewV34> batViewsV34 = new Dictionary<Enemy,BatViewV34>();
        readonly HashSet<Enemy> batBlockedV34 = new HashSet<Enemy>();
        readonly HashSet<Enemy> batSeenV34 = new HashSet<Enemy>();
        readonly List<Enemy> batStaleV34 = new List<Enemy>();
        BatAuthoredBankV34 batBankV34;
        bool batBankAttemptedV34;

        bool BatOwnsRendererV34(Renderer renderer) {
            foreach (var view in batViewsV34.Values) if (view.parts.Owns(renderer)) return true;
            return false;
        }
        void ReleaseBatViewV34(Enemy e) {
            BatViewV34 view;
            if (!batViewsV34.TryGetValue(e,out view)) return;
            batViewsV34.Remove(e);
            view.parts.Dispose();
        }
        void DisposeBatViewsV34() {
            foreach (var view in batViewsV34.Values) view.parts.Dispose();
            batViewsV34.Clear(); batBlockedV34.Clear(); batSeenV34.Clear(); batStaleV34.Clear();
        }
        bool SelectBatFrameV34(Enemy e,SpriteRenderer carrier,out BatAuthoredBankV34.Motion motion,out float clock) {
            motion=null; clock=0;
            if (!carrier || !carrier.sprite || !libraries.TryGetValue(e.kind,out var lib)) return false;
            string name=carrier.sprite.name;
            const string prefix="bat-a/";
            int slash=name.LastIndexOf('/');
            if (!name.StartsWith(prefix,StringComparison.Ordinal) || slash<=prefix.Length ||
                !int.TryParse(name.Substring(slash+1),out int index)) return false;
            string alias=name.Substring(prefix.Length,slash-prefix.Length);
            motion=batBankV34.Find(alias);
            if (motion==null || !lib.clips.TryGetValue(alias,out var clip) ||
                !lib.frames.TryGetValue(alias,out var frames) || index<0 || index>=frames.Length ||
                !ReferenceEquals(frames[index],carrier.sprite)) return false;
            if (frames.Length!=motion.samples.Length || Mathf.Abs(motion.duration-clip.duration)>.00001f ||
                clip.loop!=motion.loop || Mathf.Abs(motion.fps-clip.fps)>.00001f)
                throw new InvalidOperationException("Bat source clip metadata changed: "+alias);
            clock=alias=="source-start" || alias=="source-wait-zero2" ?
                game.EnemyInitialPoseClipClockV25(e) : First3ActualRenderClockV25(e);
            return batBankV34.CanDraw(motion,clock);
        }
        Vector3 BatSourceAnchorV34(Enemy e,string alias) {
            Vector2 point=RenderEnemyPosition(e);
            if (alias=="source-start") {
                point.y=game.EnemyInitialSupportYV25(e);
            } else if (alias=="source-wait-zero2") {
                float fraction=Mathf.SmoothStep(0,1,game.EnemyInitialPoseElapsedV25(e)/GameModel.BatReleaseDurationV25);
                point.y-=InitialBatFlightFloorV25(e)*fraction;
            } else {
                // The source adapter has already stabilized this frame's Bip relative to Wait0.
                // Recover the unprojected Wait support from the observation exactly once.
                var observed=First3ActualBoneObservationV25(e);
                if (observed==null || observed.clip!=alias || observed.registeredBipErrorLogical>.05f)
                    throw new InvalidOperationException("Missing exact Bat carrier Bip observation: "+alias);
                float scale=libraries[e.kind].data.logicalScale*e.visualScale;
                point.x=observed.expectedBipAnchor.x;
                point.y=observed.expectedBipAnchor.y-28.279397583f*scale+InitialBatFlightFloorV25(e);
            }
            return actorsRoot.TransformPoint(new Vector3(point.x,point.y,0));
        }
        void BatLabelsV34(Enemy e,int index,BatAuthoredPartsV34 parts) {
            if (e.hp<=0 || !enemyBars[index].enabled || !parts.TryTop(out float top)) return;
            var bar=enemyBars[index].transform.position;
            bar.y=top+8; enemyBars[index].transform.position=bar;
            var label=enemyDebuffViews[index].transform.position;
            label.y=top+20; enemyDebuffViews[index].transform.position=label;
        }
        void UpdateBatViewsV34() {
            if (game==null || !actorsRoot || !worldRoot || !First3CombatScopeV28() || (game.paused && openMenu=="dungeons")) {
                DisposeBatViewsV34(); return;
            }
            if (!batBankAttemptedV34) {
                batBankAttemptedV34=true;
                try { batBankV34=BatAuthoredBankV34.Load(); }
                catch (Exception ex) { Debug.LogWarning("BAT_AUTHORED_BANK_V34 "+ex.Message); }
            }
            if (batBankV34==null) return;
            batSeenV34.Clear();
            for (int i=0;i<game.enemies.Count && i<enemyViews.Count;i++) {
                var e=game.enemies[i];
                if(MonsterPixelOwnsActorV53(e)){ReleaseBatViewV34(e);continue;}
                if (e==null || e.kind!="bat-a" || e.sourceNPC!="NUI_BAT_A") continue;
                batSeenV34.Add(e);
                if (batBlockedV34.Contains(e)) continue;
                try {
                    var carrier=enemyViews[i];
                    if (!SelectBatFrameV34(e,carrier,out var motion,out float clock)) {
                        ReleaseBatViewV34(e); continue;
                    }
                    var worldAnchor=BatSourceAnchorV34(e,motion.alias);
                    var projected=NativeWorldActive?ProjectNativePoint(worldAnchor,e.nativeDepth):worldAnchor;
                    batViewsV34.TryGetValue(e,out var entry);
                    if (entry!=null && entry.carrier!=carrier) { ReleaseBatViewV34(e); entry=null; }
                    if (entry==null) {
                        entry=new BatViewV34 {carrier=carrier,parts=new BatAuthoredPartsV34(batBankV34,carrier,DestroyNativeWorldObject)};
                        batViewsV34.Add(e,entry);
                    }
                    entry.parts.Apply(motion,clock,projected,carrier.flipX?-1:1,
                        e.active && !e.dormant && carrier.enabled && carrier.gameObject.activeInHierarchy);
                    if (NativeWorldActive) entry.parts.Visit(r=>BindNativeSceneOcclusionR1461(r,worldAnchor,e.nativeDepth));
                    BatLabelsV34(e,i,entry.parts);
                } catch (Exception ex) {
                    ReleaseBatViewV34(e); batBlockedV34.Add(e);
                    Debug.LogWarning("BAT_AUTHORED_FALLBACK_V34 "+ex.Message);
                }
            }
            batStaleV34.Clear();
            foreach (var e in batViewsV34.Keys) if (!batSeenV34.Contains(e)) batStaleV34.Add(e);
            foreach (var e in batStaleV34) ReleaseBatViewV34(e);
            batBlockedV34.RemoveWhere(e=>!batSeenV34.Contains(e));
        }
    }
}
