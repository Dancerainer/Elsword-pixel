using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // C-only presentation candidate for the ten original combat Chicken births.
    // No model tick, action selection, contact, physics, source root or new clock.
    public sealed partial class DemoScene {
        const string ChickenA1AtlasResourceV31="Pixel/feedback-v29/white-chicken-a1/atlas";
        sealed class ChickenA1ViewV31 {
            internal SpriteRenderer carrier;
            internal ChickenAuthoredPartsV31 parts;
            internal string actualAlias;
            internal int actualSourceFrame;
            internal ChickenAuthoredPoseV31.AirFallContinuity motionContinuity;
            internal Vector3 logicalFoot,unprojectedWorldFoot,projectedWorldFoot;
        }
        readonly Dictionary<Enemy,ChickenA1ViewV31> chickenA1ViewsV31=new Dictionary<Enemy,ChickenA1ViewV31>();
        readonly Dictionary<Enemy,ChickenAuthoredPartsV31.Binding> chickenA1BlockedV31=new Dictionary<Enemy,ChickenAuthoredPartsV31.Binding>();
        readonly HashSet<Enemy> chickenA1SeenV31=new HashSet<Enemy>();
        readonly List<Enemy> chickenA1StaleV31=new List<Enemy>();
        ChickenAuthoredPartsV31.Binding chickenA1BindingV31;
        bool chickenA1RegistrationAttemptedV31;

        static bool ChickenA1IdentityV31(Enemy e) {
            return e!=null&&e.kind=="chicken"&&e.sourceNPC=="NUI_CHICKEN";
        }
        bool ChickenA1ScopeV31(Enemy e,int index) {
            // OriginalBirth owns the existing ten births in Tree1/Tree2/Swamp2.
            // Do not gate death on hp>0/aiEnabled or add births in other dungeons.
            return ChickenA1IdentityV31(e)&&Generic7OriginalBirthV28(e,index);
        }
        static bool ChickenA1FiniteV31(float v) { return !float.IsNaN(v)&&!float.IsInfinity(v); }
        static bool ChickenA1AliasV31(string alias,out ChickenAuthoredPoseV31.State state,
            out float duration,out int samples,out float fps,out bool loop) {
            state=ChickenAuthoredPoseV31.State.Idle;duration=0;samples=0;fps=24;loop=false;
            switch(alias) {
            case "source-idle":state=ChickenAuthoredPoseV31.State.Idle;duration=1f;samples=25;loop=true;break;
            case "source-walk":state=ChickenAuthoredPoseV31.State.Walk;duration=8f/15f;samples=14;loop=true;break;
            case "source-attack-ready":state=ChickenAuthoredPoseV31.State.AttackReady;duration=2.9f;samples=71;break;
            case "source-attack-fire":state=ChickenAuthoredPoseV31.State.AttackFire;duration=1.8f;samples=45;break;
            case "source-hurt":state=ChickenAuthoredPoseV31.State.Hurt;duration=2f/3f;samples=17;break;
            case "source-air-up":state=ChickenAuthoredPoseV31.State.AirUp;duration=.2f;samples=6;break;
            case "source-air-fall":state=ChickenAuthoredPoseV31.State.AirFall;duration=.2f;samples=6;break;
            case "source-down":state=ChickenAuthoredPoseV31.State.Down;duration=23f/30f;samples=20;break;
            case "source-get-up":state=ChickenAuthoredPoseV31.State.GetUp;duration=4f/3f;samples=33;break;
            case "source-death":state=ChickenAuthoredPoseV31.State.Death;duration=23f/30f;samples=20;break;
            // Existing original-carrier death fallback can be selected when its
            // source-death eligibility loses AI. Keep the real death clock and
            // metadata; never replace the selector or relabel it source-death.
            case "death":state=ChickenAuthoredPoseV31.State.Death;duration=1.2f;samples=72;fps=60;break;
            default:return false;
            }
            return true;
        }
        void RecordChickenA1RenderClockV31(Enemy e,string actualAlias,float actualClock) {
            if(!ChickenA1IdentityV31(e)||!ChickenA1FiniteV31(actualClock)||actualClock<0)return;
            ChickenAuthoredPoseV31.State state;float duration,fps;int samples;bool loop;
            if(!ChickenA1AliasV31(actualAlias,out state,out duration,out samples,out fps,out loop)||
                (actualAlias=="death"&&(e.hp>0||e.clip!="death")))return;
            // Called after ALL original selectors and before original Pose.
            // BeginFirst3NativeRenderFrameV25 already clears this dictionary.
            // This records the final existing float, with no new time advance.
            first3NativeRenderClocksV25[e]=actualClock;
        }
        void SelectChickenSourceCarrierV45(Enemy e,int index,ref string alias,ref float clock) {
            if(!ChickenA1IdentityV31(e)||!ChickenA1ScopeV31(e,index)||!libraries.TryGetValue(e.kind,out var lib))return;
            string next=null;float at=0;
            // These four actual NPC states now own movement and contact. The A1
            // part owner follows their original animation time, including the
            // 1.25x wind-up, rather than switching to the base 60 Hz picture.
            switch(e.npcState) {
                case "CHICKEN_WAIT":next="source-idle";at=e.npcTime;break;
                case "CHICKEN_WALK":next="source-walk";at=e.npcTime;break;
                case "CHICKEN_ATTACK_READY":next="source-attack-ready";at=e.npcTime*1.25f;break;
                case "CHICKEN_ATTACK_A":next="source-attack-fire";at=e.npcTime;break;
            }
            if(next==null&&!ChickenA1AliasV31(alias,out _,out _,out _,out _,out _)) {
                at=e.clock;
                switch(e.state) {
                    case CombatState.Hurt:next="source-hurt";break;
                    case CombatState.Airborne:next=e.clip=="fall"?"source-air-fall":"source-air-up";break;
                    case CombatState.Knockdown:next="source-down";break;
                    case CombatState.GettingUp:next="source-get-up";at=(4f/3f)*e.clock/Math.Max(.0001f,e.clock+e.recoveryTime);break;
                    case CombatState.Dead:next="source-death";at=(23f/30f)*e.deathTime/1.2f;break;
                }
            }
            if(next!=null&&lib.frames.ContainsKey(next)){alias=next;clock=at;}
            RecordChickenA1RenderClockV31(e,alias,clock);
        }
        void EnsureChickenA1RegistrationV31() {
            if(chickenA1RegistrationAttemptedV31)return;
            chickenA1RegistrationAttemptedV31=true;
            try {
                var atlas=Resources.Load<Texture2D>(ChickenA1AtlasResourceV31);
                if(!atlas)throw new InvalidOperationException("Existing Chicken A1 atlas resource is missing");
                // Existing shared7398 atlas only. The part owner validates its
                // texture profile before hiding a carrier; no pixels are edited.
                // Fixed registration preserves the original chicken:LK body ratio.
                chickenA1BindingV31=new ChickenAuthoredPartsV31.Binding(atlas,
                    ChickenAuthoredPartsV31.InitialUnitsPerArtPixel);
            } catch(Exception error) {
                chickenA1BindingV31=null;
                Debug.LogWarning("CHICKEN_A1_REGISTRATION_FALLBACK_V31 "+error.Message);
            }
        }
        bool ChickenA1OwnsRendererV31(Renderer renderer) {
            foreach(var entry in chickenA1ViewsV31.Values)
                if(entry.parts!=null&&entry.parts.OwnsRenderer(renderer))return true;
            return false;
        }
        void ReleaseChickenA1ViewV31(Enemy e) {
            ChickenA1ViewV31 entry;
            if(!chickenA1ViewsV31.TryGetValue(e,out entry))return;
            chickenA1ViewsV31.Remove(e);
            if(entry.parts!=null)entry.parts.Dispose();
        }
        void DisposeChickenA1ViewsV31() {
            foreach(var entry in chickenA1ViewsV31.Values)if(entry.parts!=null)entry.parts.Dispose();
            chickenA1ViewsV31.Clear();chickenA1BlockedV31.Clear();
            chickenA1SeenV31.Clear();chickenA1StaleV31.Clear();
            // Shared Resources atlas and original carrier material stay owned.
        }

        bool TryChickenA1FrameV31(Enemy e,SpriteRenderer carrier,
            out ChickenAuthoredPartsV31.Frame frame,out string alias,out int sourceFrame) {
            frame=new ChickenAuthoredPartsV31.Frame();alias=null;sourceFrame=-1;
            if(!carrier||!carrier.sprite||!ChickenA1IdentityV31(e))return false;
            if(!libraries.TryGetValue(e.kind,out var lib))return false;
            string name=carrier.sprite.name,prefix=e.kind+"/";
            int slash=name.LastIndexOf('/');
            if(!name.StartsWith(prefix,StringComparison.Ordinal)||slash<=prefix.Length||
                !int.TryParse(name.Substring(slash+1),out sourceFrame))return false;
            alias=name.Substring(prefix.Length,slash-prefix.Length);
            if(!lib.frames.TryGetValue(alias,out var frames)||sourceFrame<0||sourceFrame>=frames.Length||
                !ReferenceEquals(frames[sourceFrame],carrier.sprite)||!lib.clips.TryGetValue(alias,out var clip)||clip==null)return false;
            ChickenAuthoredPoseV31.State state;float duration,fps;int samples;bool loop;
            if(!ChickenA1AliasV31(alias,out state,out duration,out samples,out fps,out loop)||
                (alias=="death"&&(e.hp>0||e.clip!="death")))return false;
            if(clip.id!=alias||clip.fps!=fps||clip.loop!=loop||clip.frames==null||
                clip.frames.Length!=samples||frames.Length!=samples||clip.hits==null||clip.hits.Length!=0||
                clip.rootDistance!=0||!ChickenA1FiniteV31(clip.duration)||
                Math.Abs(clip.duration-duration)>.000001f)return false;
            float sourceClock=First3ActualRenderClockV25(e);
            if(!ChickenA1FiniteV31(sourceClock)||sourceClock<0||!ChickenA1FiniteV31(e.clock)||e.clock<0)
                throw new InvalidOperationException("Original Chicken display/host clock is invalid");
            // Original selectors already map idle1.4 / walk.48 / ready.40 /
            // fire.12 host seconds to their source spans. In particular ready's
            // source1.25 speed provenance is NOT multiplied again here.
            // Hurt/Air/Down retain actor seconds; GetUp and Death retain their
            // existing selector remaps. Normalization samples cosmetic art only.
            float sampledClock=clip.loop?Mathf.Repeat(sourceClock,clip.duration):sourceClock;
            int selected=Math.Min(frames.Length-1,Mathf.Max(0,Mathf.FloorToInt(sampledClock*clip.fps)));
            if(selected!=sourceFrame)return false;
            float phase=clip.loop?sampledClock/clip.duration:Mathf.Clamp01(sourceClock/clip.duration);
            // A1 recovery must finish when the existing Hurt state finishes.
            // Keep the original source frame/clock above exactly as selected.
            if(state==ChickenAuthoredPoseV31.State.Hurt&&e.state==CombatState.Hurt)
                phase=ChickenAuthoredPoseV31.HurtRecoveryPhase(e.clock,e.stun);
            frame=new ChickenAuthoredPartsV31.Frame {
                state=state,normalizedPhase=phase,hostClock=e.clock,sourceRenderClock=sourceClock,
                sourceDuration=clip.duration,airFallLaunchPhase=1,
                // Death remains eligible until the original actor/view disappears.
                // Final carrier clipping, color, flip, scale and MPB are copied
                // by the part owner, after the original final burial/armor pass.
                visible=e.active&&!e.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy
            };
            ChickenSourceReactionV45.Select(e,ref frame);
            return true;
        }

        void UpdateChickenA1LabelTopV31(Enemy e,int index,ChickenAuthoredPartsV31 parts) {
            if(e.hp<=0||index<0||index>=enemyBars.Count||!enemyBars[index]||!enemyBars[index].enabled)return;
            float top;if(!parts.TryVisibleTop(out top))return;
            // Preserve the original gauge/status visibility and X position.
            // The source carrier is hidden, so its old 49px top cannot anchor
            // labels above the larger source-ratio A1 drawing.
            var bar=enemyBars[index].transform.position;bar.y=top+8;enemyBars[index].transform.position=bar;
            if(index<enemyDebuffViews.Count&&enemyDebuffViews[index]) {
                var status=enemyDebuffViews[index].transform.position;status.y=top+20;enemyDebuffViews[index].transform.position=status;
            }
        }

        // Both projection branches call this after FinishEnemyInitialProjection.
        void UpdateChickenA1ViewsV31() {
            if(game==null||!actorsRoot||!worldRoot){DisposeChickenA1ViewsV31();return;}
            bool scoped=false;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++)
                if(!MonsterPixelOwnsActorV53(game.enemies[i])&&ChickenA1ScopeV31(game.enemies[i],i)){scoped=true;break;}
            if(!scoped){DisposeChickenA1ViewsV31();return;}
            EnsureChickenA1RegistrationV31();var binding=chickenA1BindingV31;
            if(binding==null){DisposeChickenA1ViewsV31();return;}
            chickenA1SeenV31.Clear();
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++) {
                var e=game.enemies[i];if(MonsterPixelOwnsActorV53(e)){ReleaseChickenA1ViewV31(e);continue;}if(!ChickenA1ScopeV31(e,i))continue;
                chickenA1SeenV31.Add(e);var carrier=enemyViews[i];
                ChickenA1ViewV31 entry;
                if(chickenA1ViewsV31.TryGetValue(e,out entry)&&
                    (entry.carrier!=carrier||entry.parts==null||!ReferenceEquals(entry.parts.BindingIdentity,binding))) {
                    ReleaseChickenA1ViewV31(e);entry=null;
                }
                ChickenAuthoredPartsV31.Binding blocked;
                if(chickenA1BlockedV31.TryGetValue(e,out blocked)&&ReferenceEquals(blocked,binding))continue;
                try {
                    ChickenAuthoredPartsV31.Frame frame;string alias;int sourceFrame;
                    if(!TryChickenA1FrameV31(e,carrier,out frame,out alias,out sourceFrame)) {
                        ReleaseChickenA1ViewV31(e);continue;
                    }
                    // RenderActors owns actorsRoot coordinates. The carrier's
                    // cropped sprite offset is deliberately not a foot anchor.
                    Vector2 point=RenderEnemyPosition(e);
                    Vector3 logicalFoot=new Vector3(point.x,point.y,0);
                    Vector3 worldFoot=actorsRoot.TransformPoint(logicalFoot);
                    Vector3 projectedFoot=NativeWorldActive?ProjectNativePoint(worldFoot,e.nativeDepth):worldFoot;
                    // No Tree emergence offset, SpriteOffset, .28, sourcePlace,
                    // Female trajectory or second projection of the children.
                    if(!chickenA1ViewsV31.TryGetValue(e,out entry)) {
                        entry=new ChickenA1ViewV31 {carrier=carrier,
                            parts=new ChickenAuthoredPartsV31(carrier,binding,DestroyNativeWorldObject)};
                        chickenA1ViewsV31.Add(e,entry);
                    }
                    // Only this view's cosmetic falling phase changes. The
                    // model, source sprite/frame/clock and projected foot above
                    // retain their original owners and values.
                    float fallLaunchPhase;
                    frame.normalizedPhase=entry.motionContinuity.Resolve(frame.state,frame.hostClock,
                        frame.normalizedPhase,frame.sourceDuration,out fallLaunchPhase);
                    frame.airFallLaunchPhase=fallLaunchPhase;
                    entry.parts.Apply(frame,projectedFoot);
                    entry.parts.VisitRenderersR1461(r=>BindNativeSceneOcclusionR1461(r,worldFoot,e.nativeDepth));
                    UpdateChickenA1LabelTopV31(e,i,entry.parts);
                    entry.actualAlias=alias;entry.actualSourceFrame=sourceFrame;
                    entry.logicalFoot=logicalFoot;entry.unprojectedWorldFoot=worldFoot;entry.projectedWorldFoot=projectedFoot;
                } catch(Exception error) {
                    ReleaseChickenA1ViewV31(e);chickenA1BlockedV31[e]=binding;
                    Debug.LogWarning("CHICKEN_A1_DRAWN_FALLBACK_V31 "+error.Message);
                }
            }
            chickenA1StaleV31.Clear();
            foreach(var e in chickenA1ViewsV31.Keys)if(!chickenA1SeenV31.Contains(e))chickenA1StaleV31.Add(e);
            foreach(var e in chickenA1StaleV31)ReleaseChickenA1ViewV31(e);
            chickenA1StaleV31.Clear();
            foreach(var e in chickenA1BlockedV31.Keys)if(!chickenA1SeenV31.Contains(e))chickenA1StaleV31.Add(e);
            foreach(var e in chickenA1StaleV31)chickenA1BlockedV31.Remove(e);
        }
    }
}
