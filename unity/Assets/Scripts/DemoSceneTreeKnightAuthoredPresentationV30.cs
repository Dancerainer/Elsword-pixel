using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Candidate display bridge only. Called after the original final projection
    // and burial pass; no Update, clock advance, model write or physical bounds.
    public sealed partial class DemoScene {
        const string TreeKnightA1AtlasResourceV30="Pixel/monsters/TreeKnightA1V30/sixparts";
        sealed class TreeKnightA1ViewV30 {
            internal SpriteRenderer carrier;
            internal TreeKnightAuthoredPartsV30 parts;
            internal string actualAlias;
            internal int actualSourceFrame;
            internal Vector3 logicalFoot,unprojectedWorldFoot,projectedWorldFoot;
        }
        readonly Dictionary<Enemy,TreeKnightA1ViewV30> treeKnightA1ViewsV30=new Dictionary<Enemy,TreeKnightA1ViewV30>();
        readonly Dictionary<Enemy,TreeKnightAuthoredPartsV30.Binding> treeKnightA1BlockedV30=new Dictionary<Enemy,TreeKnightAuthoredPartsV30.Binding>();
        readonly HashSet<Enemy> treeKnightA1SeenV30=new HashSet<Enemy>();
        readonly List<Enemy> treeKnightA1StaleV30=new List<Enemy>();
        TreeKnightAuthoredPartsV30.Binding treeKnightA1BindingV30;
        bool treeKnightA1RegistrationAttemptedV30;

        static bool TreeKnightA1IdentityV30(Enemy e) {
            return e!=null&&e.kind=="tree-knight"&&e.sourceNPC=="NUI_TREE_KNIGHT";
        }
        static bool TreeKnightA1FiniteV30(float v) { return !float.IsNaN(v)&&!float.IsInfinity(v); }
        void EnsureTreeKnightA1RegistrationV30() {
            if(treeKnightA1RegistrationAttemptedV30)return;
            treeKnightA1RegistrationAttemptedV30=true;
            try {
                var atlas=Resources.Load<Texture2D>(TreeKnightA1AtlasResourceV30);
                if(!atlas)throw new InvalidOperationException("TreeKnight A1 sixparts resource is missing");
                // The visual owner validates the complete texture profile before
                // creating/hiding anything. Never rehash a texture each frame.
                treeKnightA1BindingV30=new TreeKnightAuthoredPartsV30.Binding(atlas,
                    TreeKnightAuthoredPartsV30.InitialUnitsPerArtPixel,
                    TreeKnightAuthoredPoseV30.CurrentModelContactPhase);
            } catch(Exception error) {
                treeKnightA1BindingV30=null;
                Debug.LogWarning("TREE_KNIGHT_A1_REGISTRATION_FALLBACK_V30 "+error.Message);
            }
        }
        bool TreeKnightA1OwnsRendererV30(Renderer renderer) {
            foreach(var entry in treeKnightA1ViewsV30.Values)
                if(entry.parts!=null&&entry.parts.OwnsRenderer(renderer))return true;
            return false;
        }
        void ReleaseTreeKnightA1ViewV30(Enemy e) {
            TreeKnightA1ViewV30 entry;
            if(!treeKnightA1ViewsV30.TryGetValue(e,out entry))return;
            treeKnightA1ViewsV30.Remove(e);
            if(entry.parts!=null)entry.parts.Dispose();
        }
        void DisposeTreeKnightA1ViewsV30() {
            foreach(var entry in treeKnightA1ViewsV30.Values)if(entry.parts!=null)entry.parts.Dispose();
            treeKnightA1ViewsV30.Clear();treeKnightA1BlockedV30.Clear();
            treeKnightA1SeenV30.Clear();treeKnightA1StaleV30.Clear();
            // Resources atlas and the scene's armoredMaterial remain shared.
        }

        bool TryTreeKnightA1FrameV30(Enemy e,SpriteRenderer carrier,
            out TreeKnightAuthoredPartsV30.Frame frame,out string alias,out int sourceFrame) {
            frame=new TreeKnightAuthoredPartsV30.Frame();alias=null;sourceFrame=-1;
            if(!carrier||!carrier.sprite||!TreeKnightA1IdentityV30(e))return false;
            if(!libraries.TryGetValue(e.kind,out var lib))return false;
            string name=carrier.sprite.name,prefix=e.kind+"/";
            int slash=name.LastIndexOf('/');
            if(!name.StartsWith(prefix,StringComparison.Ordinal)||slash<=prefix.Length||
                !int.TryParse(name.Substring(slash+1),out sourceFrame))return false;
            alias=name.Substring(prefix.Length,slash-prefix.Length);
            if(!lib.frames.TryGetValue(alias,out var frames)||sourceFrame<0||sourceFrame>=frames.Length||
                !ReferenceEquals(frames[sourceFrame],carrier.sprite)||!lib.clips.TryGetValue(alias,out var clip))return false;
            TreeKnightAuthoredPoseV30.State state;
            switch(alias) {
            case "source-start":state=TreeKnightAuthoredPoseV30.State.Reveal;break;
            case "source-idle":state=TreeKnightAuthoredPoseV30.State.Idle;break;
            case "source-walk":state=TreeKnightAuthoredPoseV30.State.Walk;break;
            case "source-attack":state=TreeKnightAuthoredPoseV30.State.Attack;break;
            case "source-hurt":state=TreeKnightAuthoredPoseV30.State.Hurt;break;
            case "source-jump":state=TreeKnightAuthoredPoseV30.State.Jump;break;
            case "source-launch":state=TreeKnightAuthoredPoseV30.State.Launch;break;
            case "source-fall":state=TreeKnightAuthoredPoseV30.State.Fall;break;
            case "source-down":state=TreeKnightAuthoredPoseV30.State.Down;break;
            case "source-get-up":state=TreeKnightAuthoredPoseV30.State.GetUp;break;
            case "source-death":state=TreeKnightAuthoredPoseV30.State.Death;break;
            default:return false; // Unsupported legacy/diagnostic clip stays original.
            }
            float sourceClock=First3ActualRenderClockV25(e);
            if(!TreeKnightA1FiniteV30(sourceClock)||sourceClock<0||!TreeKnightA1FiniteV30(e.clock)||e.clock<0||
                !TreeKnightA1FiniteV30(clip.duration)||clip.duration<=0)throw new InvalidOperationException("Original Tree display clock/span is invalid");
            float phase;
            if(state==TreeKnightAuthoredPoseV30.State.Reveal) {
                phase=Mathf.Clamp01(game.EnemyInitialRevealProgressV25(e));
            } else if(state==TreeKnightAuthoredPoseV30.State.Attack) {
                // Physical generic Tree timing: .52 total; the actual hit uses
                // telegraph<=.12 && !struck. Source sprite time is NOT host time.
                phase=Mathf.Clamp01(e.clock/TreeKnightAuthoredPoseV30.ModelAttackDuration);
            } else {
                // Pose has already selected the original sprite. This only
                // samples the authored cosmetic pose using that existing clock.
                // Nonloop extra endpoint clocks may exceed intrinsic duration.
                phase=clip.loop?Mathf.Repeat(sourceClock,clip.duration)/clip.duration:Mathf.Clamp01(sourceClock/clip.duration);
            }
            frame=new TreeKnightAuthoredPartsV30.Frame {
                state=state,normalizedPhase=phase,hostClock=e.clock,sourceRenderClock=sourceClock,
                strikeObserved=state==TreeKnightAuthoredPoseV30.State.Attack&&e.struck,
                visible=e.active&&!e.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy
            };
            return true;
        }

        // Both FinishNativeProjection branches call this only AFTER
        // FinishEnemyInitialProjectionV25 has committed final clipping/armor MPB.
        void UpdateTreeKnightA1ViewsV30() {
            if(game==null||!actorsRoot||!worldRoot){DisposeTreeKnightA1ViewsV30();return;}
            bool scoped=false;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++)if(!MonsterPixelOwnsActorV53(game.enemies[i])&&TreeKnightA1IdentityV30(game.enemies[i])){scoped=true;break;}
            if(!scoped){DisposeTreeKnightA1ViewsV30();return;}
            EnsureTreeKnightA1RegistrationV30();var binding=treeKnightA1BindingV30;
            if(binding==null){DisposeTreeKnightA1ViewsV30();return;}
            treeKnightA1SeenV30.Clear();
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++) {
                var e=game.enemies[i];if(MonsterPixelOwnsActorV53(e)){ReleaseTreeKnightA1ViewV30(e);continue;}if(!TreeKnightA1IdentityV30(e))continue;
                treeKnightA1SeenV30.Add(e);var carrier=enemyViews[i];
                TreeKnightA1ViewV30 entry;
                if(treeKnightA1ViewsV30.TryGetValue(e,out entry)&&
                    (entry.carrier!=carrier||entry.parts==null||!ReferenceEquals(entry.parts.BindingIdentity,binding))) {
                    ReleaseTreeKnightA1ViewV30(e);entry=null;
                }
                TreeKnightAuthoredPartsV30.Binding blocked;
                if(treeKnightA1BlockedV30.TryGetValue(e,out blocked)&&ReferenceEquals(blocked,binding))continue;
                try {
                    TreeKnightAuthoredPartsV30.Frame frame;string alias;int sourceFrame;
                    if(!TryTreeKnightA1FrameV30(e,carrier,out frame,out alias,out sourceFrame)) {
                        ReleaseTreeKnightA1ViewV30(e);continue;
                    }
                    // Exact RenderActors coordinate owner is actorsRoot, not an
                    // assumed identity worldRoot and not the crop-shifted carrier.
                    Vector2 point=RenderEnemyPosition(e);
                    string initial=e.hp>0?game.EnemyInitialPosePhaseV25(e):"complete";
                    if(initial=="emerging"&&game.EnemyInitialPresentationVisibleV25(e)) {
                        float reveal=Mathf.SmoothStep(0,1,game.EnemyInitialRevealProgressV25(e));
                        point.y-=24*e.visualScale*(1-reveal);
                    }
                    Vector3 logicalFoot=new Vector3(point.x,point.y,0);
                    Vector3 worldFoot=actorsRoot.TransformPoint(logicalFoot);
                    Vector3 projectedFoot=NativeWorldActive?ProjectNativePoint(worldFoot,e.nativeDepth):worldFoot;
                    // Never add SpriteOffset, Bip registration, .28, sourcePlace,
                    // or an independent native projection to the six children.
                    if(!treeKnightA1ViewsV30.TryGetValue(e,out entry)) {
                        entry=new TreeKnightA1ViewV30 {carrier=carrier,
                            parts=new TreeKnightAuthoredPartsV30(carrier,binding,DestroyNativeWorldObject)};
                        treeKnightA1ViewsV30.Add(e,entry);
                    }
                    entry.parts.Apply(frame,projectedFoot,e);
                    entry.parts.VisitRenderersR1461(r=>BindNativeSceneOcclusionR1461(r,worldFoot,e.nativeDepth));
                    entry.actualAlias=alias;entry.actualSourceFrame=sourceFrame;
                    entry.logicalFoot=logicalFoot;entry.unprojectedWorldFoot=worldFoot;entry.projectedWorldFoot=projectedFoot;
                } catch(Exception error) {
                    ReleaseTreeKnightA1ViewV30(e);treeKnightA1BlockedV30[e]=binding;
                    Debug.LogWarning("TREE_KNIGHT_A1_DRAWN_FALLBACK_V30 "+error.Message);
                }
            }
            treeKnightA1StaleV30.Clear();
            foreach(var e in treeKnightA1ViewsV30.Keys)if(!treeKnightA1SeenV30.Contains(e))treeKnightA1StaleV30.Add(e);
            foreach(var e in treeKnightA1StaleV30)ReleaseTreeKnightA1ViewV30(e);
            treeKnightA1StaleV30.Clear();
            foreach(var e in treeKnightA1BlockedV30.Keys)if(!treeKnightA1SeenV30.Contains(e))treeKnightA1StaleV30.Add(e);
            foreach(var e in treeKnightA1StaleV30)treeKnightA1BlockedV30.Remove(e);
        }
    }
}
