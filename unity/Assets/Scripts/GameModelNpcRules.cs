using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcMotionSample {public float rootX;public float[] spheres=new float[0],bodySpheres=new float[0],attackLines=new float[0];}
    [Serializable] public sealed class NpcCondition {public string nextState;public float near,rate=100,hpLessPercent=101;}
    [Serializable] public sealed class NpcVelocityEvent {public float time,x=-1,y=-1;public int mode=2;public bool sourceSetX,sourceSetY;}
    [Serializable] public sealed class NpcDamageChange {public float time,sourceAnimationTime;public string damageProfile;public bool clearHitList;}
    [Serializable] public sealed class NpcReactionBody {
        public CombatState combatState;public string sourceState,sourceAnimation;
        public float duration,sourceAnimSpeed=1;public NpcMotionSample[] samples=new NpcMotionSample[0];
    }
    [Serializable] public sealed class NpcHitWindow {
        public float start,end,physical,magic,launch,push,stopAttacker,stopDefender,repeatGap=.1f;
        public string damageProfile="DAMAGE_DATA";
        public bool repeat,nextDamageCallbackPending;public HitReaction reaction=HitReaction.Hurt;
    }
    [Serializable] public sealed class NpcStateDefinition {
        public string id,clip,nextState,sourceAnimation,onHitState;
        public float duration,walkSpeed,interval=1,sourceAnimSpeed=1;
        public bool loop,followTarget,superArmor,flipFacingOnEnd,neverMove;
        [NonSerialized] public bool suppressArmorOutline;
        public bool followHitTarget,drain,allowAirborne;
        public bool holdForSourceEvent;public float entryOffsetY;
        public NpcSourceEventTransition[] sourceEventTransitions=new NpcSourceEventTransition[0];
        public NpcSourceArmorWindow[] superArmorWindows=new NpcSourceArmorWindow[0];
        public float invincibleStart,invincibleEnd;
        public NpcCondition[] choices=new NpcCondition[0];
        public NpcMotionSample[] samples=new NpcMotionSample[0];
        public NpcHitWindow[] hits=new NpcHitWindow[0];
        public NpcVelocityEvent[] velocities=new NpcVelocityEvent[0];
        public NpcDamageChange[] damageChanges=new NpcDamageChange[0];
    }
    [Serializable] public sealed class NpcDefinition {
        public bool runtimeAdmitted,sourceMechanismsComplete;
        public string id,kind,waitState,walkState,getUpState,environmentKind,initialState;
        public NpcEnvironmentRule environment;
        public NpcTraversalDefinition traversal;
        public NpcSummonRule summon;
        public float senseRange,lostRange,approachDistance,physicalAttack,magicAttack,bodyWidth,bodyHeight;
        public float[] bodySpheres=new float[0];
        public NpcStateDefinition[] states=new NpcStateDefinition[0];
        public NpcReactionBody[] reactions=new NpcReactionBody[0];
    }
    [Serializable] public sealed class NpcActionEvent {
        public string npc,state,type;public float time,x,y,sourceTime;public int window,damage;
    }

    public sealed partial class GameModel {
        readonly Dictionary<string,NpcDefinition> npcDefinitions=new Dictionary<string,NpcDefinition>();
        readonly Dictionary<string,NpcStateDefinition> npcStates=new Dictionary<string,NpcStateDefinition>();
        public readonly List<NpcActionEvent> npcActionEvents=new List<NpcActionEvent>();
        public bool traceNpcActions;
        uint npcRandom=0x7a2189u;
        public void SetNpcDefinitions(IEnumerable<NpcDefinition> definitions) {
            BeginPlainDefinitionReplacementR1258(definitions);
            var profiles=new Dictionary<string,NpcDefinition>();var states=new Dictionary<string,NpcStateDefinition>();
            if(definitions!=null)foreach(var profile in definitions){
                if(profile==null||string.IsNullOrEmpty(profile.id)||profiles.ContainsKey(profile.id)||profile.states==null||profile.reactions==null||profile.bodySpheres==null||profile.bodySpheres.Length%4!=0)throw new ArgumentException("Invalid original NPC profile.");
                NormalizeNpcOptionalRules(profile);
                if(!Finite(profile.senseRange)||profile.senseRange<0||profile.senseRange==0&&string.IsNullOrEmpty(profile.environmentKind)&&!IsBelowTunnelZeroSenseV20(profile)&&!IsCoreHardPassiveRangesV21(profile)||!Finite(profile.lostRange)||profile.lostRange<profile.senseRange&&!IsCoreHardPassiveRangesV21(profile)&&!Remaining3AttackSourceRulesV28.OriginalSenseLostPair(profile)&&!(profile.id=="NUI_PPORU_WILLIAM_NORMAL"&&Math.Abs(profile.senseRange-9999*.28f)<.001f&&Math.Abs(profile.lostRange-600*.28f)<.001f)||!Finite(profile.approachDistance)||profile.approachDistance<0||!Finite(profile.physicalAttack)||profile.physicalAttack<0||!Finite(profile.magicAttack)||profile.magicAttack<0)throw new ArgumentException("Invalid NPC detection or authored attack power.");
                for(int i=0;i<profile.bodySpheres.Length;i++)if(!Finite(profile.bodySpheres[i])||i%4==3&&profile.bodySpheres[i]<=0)throw new ArgumentException("Invalid original body helper.");
                if(!Finite(profile.bodyWidth)||profile.bodyWidth<=0&&!IsCoreHardZeroBodyV21(profile)||!Finite(profile.bodyHeight)||profile.bodyHeight<0||profile.bodyHeight==0&&profile.environmentKind!="timed-steam"&&profile.environmentKind!="proximity-trap"&&!IsAlteraSteamZeroBodyV18(profile)&&!IsDragonGeyserZeroBodyV20(profile)&&!IsBelowTunnelZeroBodySteamV20(profile)&&!IsCoreHardZeroBodyV21(profile)&&!IsPlainOriginalMineZeroBodyR1258(profile))throw new ArgumentException("Invalid source unit dimensions.");
                var reactions=new HashSet<CombatState>();
                foreach(var reaction in profile.reactions){
                    if(reaction==null||!reactions.Add(reaction.combatState)||(reaction.combatState!=CombatState.Hurt&&reaction.combatState!=CombatState.Knockdown&&reaction.combatState!=CombatState.GettingUp)||string.IsNullOrEmpty(reaction.sourceState)||string.IsNullOrEmpty(reaction.sourceAnimation)||!Finite(reaction.duration)||reaction.duration<=0||!Finite(reaction.sourceAnimSpeed)||reaction.sourceAnimSpeed<=0||reaction.samples==null||reaction.samples.Length<(int)Math.Ceiling(reaction.duration*60-.001f)+1)throw new ArgumentException("Invalid original NPC reaction body.");
                    foreach(var frame in reaction.samples){
                        if(frame==null||frame.bodySpheres==null||frame.bodySpheres.Length==0||frame.bodySpheres.Length%4!=0||frame.bodySpheres.Length!=reaction.samples[0].bodySpheres.Length)throw new ArgumentException("Invalid source reaction helper sample.");
                        for(int i=0;i<frame.bodySpheres.Length;i++)if(!Finite(frame.bodySpheres[i])||i%4==3&&frame.bodySpheres[i]<=0)throw new ArgumentException("Invalid source reaction helper radius/coordinate.");
                    }
                }
                var local=new HashSet<string>();
                foreach(var state in profile.states){
                    if(state==null||string.IsNullOrEmpty(state.id)||!local.Add(state.id)||string.IsNullOrEmpty(state.clip)||!Finite(state.duration)||state.duration<=0||!Finite(state.interval)||state.interval<=0||!Finite(state.walkSpeed)||state.walkSpeed<0||state.samples==null||state.samples.Length==0||state.hits==null||state.choices==null||state.velocities==null||state.damageChanges==null)throw new ArgumentException("Invalid original NPC state.");
                    foreach(var frame in state.samples){if(frame==null||!Finite(frame.rootX)||frame.spheres==null||frame.spheres.Length%4!=0||frame.bodySpheres==null||frame.bodySpheres.Length%4!=0||frame.spheres.Length!=state.samples[0].spheres.Length||frame.bodySpheres.Length!=state.samples[0].bodySpheres.Length)throw new ArgumentException("Invalid source motion/helper sample.");foreach(var values in new[]{frame.spheres,frame.bodySpheres})for(int i=0;i<values.Length;i++)if(!Finite(values[i])||i%4==3&&values[i]<=0)throw new ArgumentException("Invalid source helper radius/coordinate.");
                        if(frame.attackLines==null)frame.attackLines=new float[0];ValidateNpcLines(frame.attackLines);if(frame.attackLines.Length!=state.samples[0].attackLines.Length)throw new ArgumentException("Inconsistent source attack line samples.");}
                    foreach(var hit in state.hits)if(hit==null||!Finite(hit.start)||!Finite(hit.end)||hit.start<0||hit.end<hit.start||hit.end>state.duration+.001f||!Finite(hit.physical)||hit.physical<0||!Finite(hit.magic)||hit.magic<0||!Finite(hit.repeatGap)||hit.repeatGap<=0)throw new ArgumentException("Invalid source NPC hit window.");
                    float previousChange=0;foreach(var change in state.damageChanges){
                        if(change==null||!Finite(change.time)||change.time<=previousChange||change.time>state.duration||!Finite(change.sourceAnimationTime)||change.sourceAnimationTime<=0||string.IsNullOrEmpty(change.damageProfile))throw new ArgumentException("Invalid source NPC damage callback.");
                        foreach(var hit in state.hits)if(hit.start<change.time&&change.time<=hit.end)throw new ArgumentException("NPC damage change crosses an unsplit hit window.");
                        previousChange=change.time;
                    }
                    foreach(var hit in state.hits)if(hit.damageProfile!=NpcDamageProfile(state,hit.start))throw new ArgumentException("NPC hit window does not match its source damage phase.");
                    float previousVelocity=-1;foreach(var velocity in state.velocities){if(velocity==null||!Finite(velocity.time)||velocity.time<previousVelocity||velocity.time<0||!Finite(velocity.x)||!Finite(velocity.y)||velocity.mode<1||velocity.mode>4)throw new ArgumentException("Invalid source NPC velocity event.");previousVelocity=velocity.time;}
                    BindNpcWeaponAttackLine(profile,state);
                    string sourceKey=profile.id+"/"+state.id;
                    state.neverMove=state.neverMove||sourceNeverMoveStatesV25.Contains(sourceKey);
                    state.suppressArmorOutline=sourceNonRedArmorStates.Contains(sourceKey);
                    states.Add(sourceKey,state);
                }
                if(!local.Contains(profile.waitState)||!local.Contains(profile.walkState))throw new ArgumentException("NPC has no source wait/walk state.");
                if(!string.IsNullOrEmpty(profile.getUpState)&&!local.Contains(profile.getUpState))throw new ArgumentException("NPC source get-up state is missing.");
                if(!string.IsNullOrEmpty(profile.initialState)&&!local.Contains(profile.initialState))throw new ArgumentException("NPC declared source initial state is missing.");
                foreach(var state in profile.states){
                    ValidateNpcSourceEventsV17(state.id,state.duration,state.sourceEventTransitions,state.superArmorWindows,state.entryOffsetY,local);
                    if(!state.loop&&!local.Contains(state.nextState))throw new ArgumentException("NPC source motion-end target is missing.");
                    if(!string.IsNullOrEmpty(state.onHitState)&&!local.Contains(state.onHitState))throw new ArgumentException("NPC hit-success target is missing.");
                    if(!Finite(state.invincibleStart)||!Finite(state.invincibleEnd)||state.invincibleStart<0||state.invincibleEnd<state.invincibleStart)throw new ArgumentException("Invalid NPC source invincibility interval.");
                    foreach(var condition in state.choices)if(condition==null||!local.Contains(condition.nextState)||!Finite(condition.near)||condition.near<0||!Finite(condition.rate)||condition.rate<0||condition.rate>100||!Finite(condition.hpLessPercent)||condition.hpLessPercent<0||condition.hpLessPercent>101)throw new ArgumentException("NPC source condition/target is invalid.");
                }
                profiles.Add(profile.id,profile);
            }
            foreach(var profile in profiles.Values){ValidateNpcSummonRule(profile,profiles);ValidateNpcTraversal(profile);ValidateNpcEnvironment(profile);ValidateNpcPassageEnvironment(profile,profiles);}
            InvalidateNpcMechanismDefinitions();
            ResetCastleSiegeLinks();
            ClearCargoBeeReactionR1484();npcDefinitions.Clear();npcStates.Clear();foreach(var p in profiles)npcDefinitions.Add(p.Key,p.Value);foreach(var s in states)npcStates.Add(s.Key,s.Value);
            foreach(var enemy in enemies)InterruptNpcAction(enemy);
            ResetNpcEffects();
        }
        // Optional original birth selector. Only Mouse, MouseNoDrop and Mousehouse declare it.
        // Attach before the first render and before queued children become visible;
        // motion-end and state-local invincibility stay with the normal rule runner.
        void InitializeNpcBirthStateV26(Enemy enemy){
            var p=NpcProfile(enemy);
            if(enemy==null||enemy.hp<=0||p==null||string.IsNullOrEmpty(p.initialState)||!string.IsNullOrEmpty(enemy.npcState))return;
            EnterNpcState(enemy,npcStates[p.id+"/"+p.initialState]);
        }
        static bool NpcSourceBirthStateActiveV26(Enemy enemy,NpcDefinition profile){
            return profile!=null&&!string.IsNullOrEmpty(profile.initialState)&&enemy.npcState==profile.initialState;
        }
        NpcDefinition NpcProfile(Enemy enemy){NpcDefinition profile;var found=enemy!=null&&!string.IsNullOrEmpty(enemy.sourceNPC)&&npcDefinitions.TryGetValue(enemy.sourceNPC,out profile)?profile:null;if(found!=null&&(Remaining3AttackSourceRulesV28.Supported(found.kind,found.id)&&!Remaining3SourceBirthBoundV28(enemy,found)||GuardianCombatNativeRulesV28.Identity(found.kind,found.id)&&!GuardianSourceBirthBoundV28(enemy,found)))found=null;return CargoBeeProfileR1484(enemy,CoreHardLocalBirthProfileV21(enemy,CoreHardAuthoredProfileV21(enemy,found)));}
        NpcStateDefinition NpcState(Enemy enemy){
            if(enemy==null||string.IsNullOrEmpty(enemy.npcState))return null;
            if(enemy.stateLookupNpc!=enemy.sourceNPC||enemy.stateLookupState!=enemy.npcState){
                enemy.stateLookupNpc=enemy.sourceNPC;enemy.stateLookupState=enemy.npcState;
                enemy.stateLookupKey=enemy.sourceNPC+"/"+enemy.npcState;
            }
            return npcStates.TryGetValue(enemy.stateLookupKey,out var state)?state:null;
        }
        static float[] HurtSphereBuffer(Enemy enemy,int length){
            if(enemy.hurtSphereBuffer==null||enemy.hurtSphereBuffer.Length!=length)
                enemy.hurtSphereBuffer=new float[length];
            return enemy.hurtSphereBuffer;
        }
        // The source collision helper is animated separately from the physical
        // UNIT_WIDTH blocker. Ground reactions use their own original helpers.
        // Front/back dispatch and state lifetimes still follow the documented
        // demo adapter; unbound airborne subtypes keep their existing fallback.
        float[] NpcHurtSpheres(Enemy enemy){
            RegionActorMechanics regional;
            if(enemy!=null&&regionActors.TryGetValue(enemy.kind,out regional)&&regional.body!=null&&regional.body.Length>0)
                return regional.body;
            if(EnemyInitialHurtboxHiddenV25(enemy))return Array.Empty<float>();var profile=NpcProfile(enemy);if(profile==null)return null;if(EnemyHangingV25(enemy)){var below=HurtSphereBuffer(enemy,profile.bodySpheres.Length);Array.Copy(profile.bodySpheres,below,below.Length);for(int i=1;i<below.Length;i+=4)below[i]-=profile.bodyHeight;return below;}
            if(enemy.state!=CombatState.Normal){
                foreach(var reaction in profile.reactions)if(reaction.combatState==enemy.state){
                    float sample=Clamp(Math.Min(enemy.clock,reaction.duration)*60,0,reaction.samples.Length-1);int index=(int)sample;float blend=sample-index;
                    var a=reaction.samples[index].bodySpheres;var b=reaction.samples[Math.Min(index+1,reaction.samples.Length-1)].bodySpheres;
                    if(blend<=0)return a;var result=HurtSphereBuffer(enemy,a.Length);for(int i=0;i<result.Length;i++)result[i]=a[i]+(b[i]-a[i])*blend;return result;
                }
                return null;
            }
            var state=NpcState(enemy);if(state==null)return profile.bodySpheres;
            int frame=Math.Min(state.samples.Length-1,Math.Max(0,(int)Math.Round(NpcLakeNightMotionFrameV19(state,enemy.npcTime))));
            return state.samples[frame].bodySpheres;
        }
        // NPC rule compilation subtracts render_floor(Wait, 0) from its
        // animated helpers. Restore that native distance in model units.
        // The sprite library applies its separate pixel-art height scale.
        static float NativeHoverFloorV58(string npc){
            switch(npc){
                case "NUI_BAT_A": case "NUI_BAT_B_FIRE": return 18.48088f;
                case "NUI_BAT_B": return 18.64388f;
                case "NUI_BAT_SONIC": return 8.20046f;
                case "NUI_BEE_NASOD": return 15.38402f;
                case "NUI_FOREST_DARK_MAGICIAN": case "NUI_GHOST_MAGICIAN_RAGGED": return 6.91933f;
                case "NUI_NASOD_HEALER": case "NUI_NASOD_HEALER_BLADER": return 39.02549f;
                case "NUI_PARASITE": return 18.14648f;
                case "NUI_WALLY_9TH_ARMOR": return 11.81553f;
                default: return 0;
            }
        }
        static readonly Dictionary<string,Dictionary<string,float[]>> nativeHoverCurvesV58=new Dictionary<string,Dictionary<string,float[]>>();
        internal static void RegisterHoverCurveV58(string npc,string motion,float artFloor,float[] artY){
            if(!nativeHoverCurvesV58.TryGetValue(npc,out var curves)){
                curves=new Dictionary<string,float[]>();nativeHoverCurvesV58.Add(npc,curves);
            }
            float ratio=NativeHoverFloorV58(npc)/artFloor;
            curves[motion]=Array.ConvertAll(artY,y=>y*ratio);
        }
        static float ContactPoseYV58(float[] curve,float seconds){
            float at=Clamp(seconds*60,0,curve.Length-1);
            int i=(int)at;return curve[i]+(curve[Math.Min(i+1,curve.Length-1)]-curve[i])*(at-i);
        }
        float NativeHoverLiftV58(Enemy enemy){
            float floor=NativeHoverFloorV58(enemy.sourceNPC);
            if(floor==0||EnemyHangingV25(enemy))return 0;
            if(nativeHoverCurvesV58.TryGetValue(enemy.sourceNPC,out var curves)){
                string motion=null;float time=0;
                if(initialNpcPosesV25.TryGetValue(enemy,out var pose)&&!pose.tree&&!pose.done&&pose.phase==1&&curves.TryGetValue("WaitZero2",out var release)){
                    // These two cave bats borrow Wait's helpers during release.
                    // Continue from the suspended helper position on the first
                    // frame, then hand back to the original flying helper.
                    float progress=Clamp(pose.time/BatReleaseDurationV25,0,1);
                    float first=(floor+release[0])*enemy.visualScale;
                    return (floor+ContactPoseYV58(release,pose.time))*enemy.visualScale-
                        (HangingNpcBodyHeightV25(enemy)+first)*(1-progress);
                }
                if(enemy.state==CombatState.Normal){
                    var state=NpcState(enemy);
                    if(state!=null){motion=state.sourceAnimation;time=enemy.npcTime*state.sourceAnimSpeed;}
                }else if(enemy.sourceNPC=="NUI_BEE_NASOD"&&
                         (enemy.state==CombatState.Knockdown||enemy.state==CombatState.GettingUp)&&CargoBeePhysicalBoundR1484(enemy)){
                    // Cargo constructs these two reactions from state samples.
                    // All other physical reaction helpers already contain Y.
                    motion=enemy.state==CombatState.Knockdown?"DamageDown":"DamageStandUp";time=enemy.clock;
                }
                if(motion!=null&&curves.TryGetValue(motion,out var curve))floor+=ContactPoseYV58(curve,time);
            }
            return floor*enemy.visualScale;
        }
        bool NpcCircle(Enemy enemy,float[] spheres,int offset,out float x,out float y,out float radius){
            float scale=enemy.visualScale,z=spheres[offset+2]*scale,r=spheres[offset+3]*scale;
            float depth=Math.Max(0,Math.Abs(z)-9.8f);x=enemy.x+enemy.facing*spheres[offset]*scale;
            float rootAdjust=enemy.sourceNPC=="NUI_EL_POWER_PLANT"&&CoreHardClosureScopeV21()?CorePlantHelperOriginYV59:0;
            y=enemy.y+NativeHoverLiftV58(enemy)+(spheres[offset+1]+rootAdjust)*scale;
            radius=depth>r?-1:(float)Math.Sqrt(Math.Max(0,r*r-depth*depth));return radius>=0;
        }
        bool SourceEnemyBounds(Enemy enemy,out AttackBox box){
            box=null;var spheres=NpcHurtSpheres(enemy);if(spheres==null)return false;
            float left=float.PositiveInfinity,right=float.NegativeInfinity,bottom=float.PositiveInfinity,top=float.NegativeInfinity;
            for(int i=0;i<spheres.Length;i+=4){float x,y,r;if(!NpcCircle(enemy,spheres,i,out x,out y,out r))continue;left=Math.Min(left,x-r);right=Math.Max(right,x+r);bottom=Math.Min(bottom,y-r);top=Math.Max(top,y+r);}
            box=new AttackBox{left=left,right=right,bottom=bottom,top=top};return true;
        }
        bool EnemySegmentIntersects(Enemy enemy,float ax,float ay,float bx,float by,float radius){
            var spheres=NpcHurtSpheres(enemy);if(spheres==null)return LineHitsBox(ax,ay,bx,by,EnemyBounds(enemy),radius);
            float dx=bx-ax,dy=by-ay,length=dx*dx+dy*dy;
            for(int i=0;i<spheres.Length;i+=4){float x,y,r;if(!NpcCircle(enemy,spheres,i,out x,out y,out r))continue;float t=length<.000001f?0:Clamp(((x-ax)*dx+(y-ay)*dy)/length,0,1);float ex=x-(ax+t*dx),ey=y-(ay+t*dy);if(ex*ex+ey*ey<=(r+radius)*(r+radius))return true;}
            return false;
        }
        bool EnemyCircleIntersects(Enemy enemy,float x,float y,float radius){return EnemySegmentIntersects(enemy,x,y,x,y,radius);}
        AttackBox EnemyBlockingBounds(Enemy enemy){
            var profile=NpcProfile(enemy);if(profile==null)return EnemyBounds(enemy);if(EnemyHangingV25(enemy)){float hangingHalf=profile.bodyWidth*enemy.visualScale*.5f;return new AttackBox{left=enemy.x-hangingHalf,right=enemy.x+hangingHalf,bottom=enemy.y-HangingNpcBodyHeightV25(enemy),top=enemy.y};}
            float half=profile.bodyWidth*enemy.visualScale*.5f,height=profile.bodyHeight*enemy.visualScale,bottom=enemy.y;
            if(NativeHoverFloorV58(enemy.sourceNPC)>0&&SourceEnemyBounds(enemy,out var body)&&Finite(body.bottom)&&Finite(body.top))
                bottom=(body.bottom+body.top-height)*.5f;
            return new AttackBox{left=enemy.x-half,right=enemy.x+half,bottom=bottom,top=bottom+height};
        }
        public bool EnemyHasSuperArmor(Enemy enemy)
        {
            if(enemy==null||enemy.hp<=0)return false;
            var actor=enemy.regionActor;
            if(actor!=null&&actor.ActionArmored)return true;
            if(enemy.superArmor>0||PeitaRootedBoss(enemy)||PeitaMoveArmored(enemy)||FairySourceGroundArmorV34(enemy)||
               EnemyInitialSuperArmorV25(enemy)||BelowBombArmorV20(enemy))return true;
            var state=NpcState(enemy);
            return state!=null&&SourceNpcArmorAtV17(state.superArmor,state.superArmorWindows,CoreHardProtectionClockV21(enemy,state));
        }
        void TraceNpc(Enemy enemy,string type,int window=-1,int damage=0,float sourceTime=-1){if(!traceNpcActions)return;if(npcActionEvents.Count>=4096)npcActionEvents.RemoveAt(0);npcActionEvents.Add(new NpcActionEvent{npc=enemy.sourceNPC,state=enemy.npcState,type=type,time=elapsed,x=enemy.x,y=enemy.y,window=window,damage=damage,sourceTime=sourceTime<0?enemy.npcTime:sourceTime});}
        static string NpcDamageProfile(NpcStateDefinition state,float at){string profile="DAMAGE_DATA";foreach(var change in state.damageChanges){if(change.time>at)break;profile=change.damageProfile;}return profile;}
        void AdvanceNpcDamageCallbacks(Enemy enemy,NpcStateDefinition state,float from,float to){
            // Native AnimEventTimer is previous < threshold <= current.
            // State time already divides source animation time by ANIM_SPEED.
            for(int i=0;i<state.damageChanges.Length;i++){
                var change=state.damageChanges[i];if(!(from<change.time&&change.time<=to))continue;
                if(change.clearHitList){for(int j=0;j<enemy.npcNextHits.Length;j++)enemy.npcNextHits[j]=float.NegativeInfinity;TraceNpc(enemy,"clear-hit-list",i,0,change.time);}
                TraceNpc(enemy,"source-damage-change",i,0,change.time);
            }
        }
        void InterruptNpcAction(Enemy enemy){if(enemy==null)return;if(PlainPairInterruptR1258(enemy))return;OnCargoSourceStateExitedV21(enemy);OnAlteraAirshipStateExitedV20(enemy);OnDragonStateExitV20(enemy,NpcState(enemy));OnBelowMiniUpWaitExitV20(enemy,NpcState(enemy));OnBelowTunnelStateExitV20(enemy,NpcState(enemy));OnBelowFortExitV20(enemy,NpcState(enemy));OnBelowFxStateExitV20(enemy);OnBelowBombExitV20(enemy,NpcState(enemy));OnBelowVisibilityStateExitedV21(enemy,NpcState(enemy));OnMine2PresentationStateExitedV21(enemy);OnMine2SourceStateExitV20(enemy,NpcState(enemy));CancelNpcMechanismRequest(enemy);CancelNpcStateSounds(enemy);OnBelowVisibilityInterruptedV21(enemy);enemy.npcState="";enemy.npcTime=enemy.npcDecisionElapsed=enemy.npcVelocityX=0;enemy.npcVelocityIndex=enemy.npcVelocityMode=0;enemy.npcNextHits=new float[0];}
        void EnterNpcState(Enemy enemy,NpcStateDefinition state){
            state=SimpleDangerEntryState(enemy,state);
            if(CloneComboV45(state.id))cloneComboContactsV45.Remove(enemy);
            ResetGuardianEntryV36(enemy);PlainPairOnStateExitedR1258(enemy,NpcState(enemy));
            OnCargoSourceStateExitedV21(enemy);OnAlteraAirshipStateExitedV20(enemy);
            OnDragonStateExitV20(enemy,NpcState(enemy));OnBelowMiniUpWaitExitV20(enemy,NpcState(enemy));OnBelowTunnelStateExitV20(enemy,NpcState(enemy));OnBelowFortExitV20(enemy,NpcState(enemy));OnBelowFxStateExitV20(enemy);OnBelowBombExitV20(enemy,NpcState(enemy));OnBelowVisibilityStateExitedV21(enemy,NpcState(enemy));OnMine2PresentationStateExitedV21(enemy);OnMine2SourceStateExitV20(enemy,NpcState(enemy));
            OnNpcLakeVisualStateEnteringV18(enemy,state);
            if(state.entryOffsetY!=0){enemy.y+=state.entryOffsetY;enemy.groundLine=-1;}
            enemy.npcState=state.id;enemy.npcTime=0;enemy.npcVelocityX=0;enemy.npcVelocityIndex=enemy.npcVelocityMode=0;enemy.npcNextHits=new float[state.hits.Length];
            // Source jump entry velocity must precede gravity and velocity
            // predicates. Otherwise a raised foot with vy=0 immediately reads
            // as descending before its declared time-zero jump impulse runs.
            if(state.entryOffsetY!=0||state.sourceEventTransitions!=null&&state.sourceEventTransitions.Length>0){
                while(enemy.npcVelocityIndex<state.velocities.Length&&state.velocities[enemy.npcVelocityIndex].time==0)ApplyNpcVelocityEvent(enemy,state.velocities[enemy.npcVelocityIndex++]);
            }
            for(int i=0;i<enemy.npcNextHits.Length;i++)enemy.npcNextHits[i]=float.NegativeInfinity;
            enemy.clip=state.clip;enemy.clock=0;enemy.telegraph=0;TraceNpc(enemy,"state");
            enemy.npcAttachedEffectSets=new bool[0];AttachNpcEffectSets(enemy,state,0,0,elapsed);
            OnAlteraAirshipStateEnteredV20(enemy,state);
            BeginNpcStateSounds(enemy,state);
            OnAlteraRepairmanStateEnteredV18(enemy,state);
            OnDragonStateEnteredV20(enemy,state);OnBelowMiniUpWaitEnteredV20(enemy,state);OnBelowTunnelStateEnteringV20(enemy,state);OnBelowFortEnteredV20(enemy,state);OnBelowFxStateEnteredV20(enemy);OnBelowBombEnteredV20(enemy,state);OnBelowVisibilityStateEnteredV21(enemy,state);OnMine2PresentationStateEnteredV21(enemy,state);OnCoreHardStateEnteredV21(enemy,state);OnCargoSourceStateEnteredV21(enemy,state);PlainPairOnStateEnteredR1258(enemy,state);OnFactoryRavenCloneStateEnteredV21(enemy,state);
        }
        float NpcRoll(){npcRandom^=npcRandom<<13;npcRandom^=npcRandom>>17;npcRandom^=npcRandom<<5;return (npcRandom%1000000)/10000f;}
        static float NpcRoot(NpcStateDefinition state,float time){float frame=Clamp(NpcLakeNightMotionFrameV19(state,time),0,state.samples.Length-1);int a=(int)frame,b=Math.Min(a+1,state.samples.Length-1);return state.samples[a].rootX+(state.samples[b].rootX-state.samples[a].rootX)*(frame-a);}
        void MoveNpcVelocity(Enemy enemy,float dt){
            // Same documented demo velocity-mode adapter as player commands:
            // 1 add impulse, 2 replace impulse, 3 passive speed, 4 vertical only.
            // This is not a measurement of the native callback implementation.
            if(enemy.npcVelocityMode!=3)enemy.npcVelocityX=Math.Sign(enemy.npcVelocityX)*Math.Max(0,Math.Abs(enemy.npcVelocityX)-560*dt);
            MoveAlongGround(enemy,enemy.x+enemy.facing*enemy.npcVelocityX*dt);
        }
        static void ApplyNpcVelocityEvent(Enemy enemy,NpcVelocityEvent velocity){
            bool x=SourceNpcVelocityHasComponentV17(velocity.x,velocity.sourceSetX),y=SourceNpcVelocityHasComponentV17(velocity.y,velocity.sourceSetY);
            if(velocity.mode==1){if(x)enemy.npcVelocityX+=velocity.x;if(y)enemy.vy+=velocity.y;enemy.npcVelocityMode=1;}
            else if(velocity.mode==2||velocity.mode==3){if(x)enemy.npcVelocityX=velocity.x;if(y)enemy.vy=velocity.y;enemy.npcVelocityMode=velocity.mode;}
            else if(velocity.mode==4){if(y)enemy.vy=velocity.y;}
        }
        bool TickNpcRule(Enemy enemy,float dt){
            var profile=NpcProfile(enemy);if(profile==null)return false;if(TickBelowMiniUpWaitV20(enemy,dt))return true;var cargoState=NpcState(enemy);if(cargoState!=null&&TryCargoSourceImmediateV21(enemy,cargoState))return true;
            bool repairSupport=EnsureAlteraRepairmanInitialStateV18(enemy,profile);
            float dx=player.x-enemy.x,dy=player.y-enemy.y,distance=(float)Math.Sqrt(dx*dx+dy*dy);
            if(Mine2RailBoundV21(enemy))enemy.alerted=true;if(!enemy.alerted){if(distance<=Remaining3SourceAlertRangeV28(enemy,profile))enemy.alerted=true;else if(!repairSupport&&!AlteraAirshipScriptedStartActiveV18(enemy)&&!FeedbackMiniUpSourceStartActiveV25(enemy)&&!CargoSourceScriptedStartActiveV21(enemy)&&!NpcSourceBirthStateActiveV26(enemy,profile)&&!GuardianIdleScopeV36(enemy))return true;}
            var state=NpcState(enemy);
            if(state==null){if(!TryEnterNpcState(enemy,npcStates[profile.id+"/"+profile.waitState]))return true;state=NpcState(enemy);}
            if(TickFactoryRavenDisabledV21(enemy,state,dt))return true;
            string sourceEvent=SourceNpcEventTargetV17(state.sourceEventTransitions,enemy.vy,Grounded(enemy));
            if(sourceEvent!=null){
                var transition=Array.Find(state.sourceEventTransitions,t=>t.nextState==sourceEvent);
                if(TryEnterNpcState(enemy,npcStates[profile.id+"/"+sourceEvent]))EmitNpcSourceLandingV17(enemy,state.id,sourceEvent,transition.condition);
                return true;
            }
            if(TickGuardianTrapWaitV28(enemy,profile,state,distance,dt))return true;
            bool deciding=!Mine2RailBoundV21(enemy)&&(state.id==profile.waitState||state.id==profile.walkState);
            if(deciding){
                if(TrySoloEscapeV45(enemy))return true;
                if(!enemy.alerted && GuardianIdleScopeV36(enemy)){
                    if(TryGuardianWait3V36(enemy,state,dt))return true;
                    AdvanceNpcLoopSounds(enemy,state,enemy.npcTime,dt,elapsed-dt);
                    enemy.npcTime=(enemy.npcTime+dt)%state.duration;
                    return true;
                }
                if(distance>profile.lostRange){enemy.alerted=false;if(!repairSupport){TryEnterNpcState(enemy,npcStates[profile.id+"/"+profile.waitState]);return true;}}
                if(TryBeginNpcSummon(enemy,profile,state))return true;
                enemy.npcDecisionElapsed+=dt;if(!repairSupport||enemy.alerted)enemy.facing=dx>=0?1:-1;
                if(enemy.npcDecisionElapsed+1e-6f>=state.interval){
                    enemy.npcDecisionElapsed%=state.interval;
                    if(TryAlteraRepairmanDecisionV18(enemy,state,true))return true;if(TryCargoSourceDecisionV21(enemy,state,distance))return true;
                    foreach(var condition in CargoFallbackChoicesV21(enemy,state,state.choices)){
                        if(repairSupport&&!enemy.alerted)continue;
                        if(distance>condition.near||enemy.hp*100>=enemy.maxHp*condition.hpLessPercent)continue;
                        if(condition.rate<100&&NpcRoll()>=condition.rate)continue;
                        if(!NpcLakeConditionAllowedV18(enemy,condition))continue;
                        if(!DragonConditionAllowedV20(enemy,condition,distance)||!FactoryRavenCloneConditionAllowedV21(enemy,condition))continue;
                        if(!TryEnterNpcState(enemy,npcStates[profile.id+"/"+condition.nextState]))continue;enemy.npcDecisionElapsed=0;return true;
                    }
                    if(TryNasodSprayFarAttackV35(enemy,state,distance)||TryPlainDrillerFarAttackV35(enemy,state,distance)||TryFarCannonV45(enemy,state,distance))return true;
                }
                bool moving=(!repairSupport||enemy.alerted)&&Math.Abs(dx)>profile.approachDistance;string desired=moving?profile.walkState:profile.waitState;
                if(state.id!=desired){if(!TryEnterNpcState(enemy,npcStates[profile.id+"/"+desired]))return true;state=NpcState(enemy);}
                if(moving)MoveAlongGround(enemy,enemy.x+enemy.facing*state.walkSpeed*dt);
                if(TryGuardianWait3V36(enemy,state,dt))return true;
                AdvanceNpcLoopSounds(enemy,state,enemy.npcTime,dt,elapsed-dt);enemy.npcTime=(enemy.npcTime+dt)%state.duration;return true;
            }
            if(state.followTarget)enemy.facing=dx>=0?1:-1;
            bool timedRush=OriginalRushTimerV45.Active(state);
            float from=enemy.npcTime,to=Mine2RailLoopV21(state)||timedRush?from+dt:NpcLakeNightStepEndV19(state,from,dt);
            AttachNpcEffectSets(enemy,state,from,to,elapsed-dt);
            AdvanceNpcDamageCallbacks(enemy,state,from,to);AdvanceAlteraRavenMaximumReleaseV18(enemy,state,from,to,elapsed-dt);AdvanceAlteraRavenMissileRainV18(enemy,state,from,to,elapsed-dt);
            AdvanceNpcStateSounds(enemy,state,from,to,elapsed-dt);AdvanceCargoSourceCallbacksV21(enemy,state,from,to,elapsed-dt);
            AdvanceNpcSummon(enemy,profile,state,from,to,elapsed-dt);
            AdvanceNpcSourceSupportV17(enemy,state,from,to,elapsed-dt);AdvanceCoreHardLocalAddsV21(enemy,state,from,to,elapsed-dt);
            AdvanceNpcLakeVisualV18(enemy,state,from,to,elapsed-dt);
            AdvanceNpcLakeSourceV18(enemy,state,from,to,elapsed-dt);
            AdvanceDragonCallbacksV20(enemy,state,from,to,elapsed-dt);AdvanceBelowFxV20(enemy,state,from,to,elapsed-dt);AdvanceMine2SourceV20(enemy,state,from,to,elapsed-dt);
            // Integrate the declared passive speed over exact event boundaries.
            float cursor=from;
            while(enemy.npcVelocityIndex<state.velocities.Length&&state.velocities[enemy.npcVelocityIndex].time<=to+1e-6f){
                var velocity=state.velocities[enemy.npcVelocityIndex++];float at=Clamp(velocity.time,from,to);
                MoveNpcVelocity(enemy,at-cursor);cursor=at;
                ApplyNpcVelocityEvent(enemy,velocity);
                TraceNpc(enemy,"velocity");
            }
            MoveNpcVelocity(enemy,to-cursor);MoveAlongGround(enemy,enemy.x+enemy.facing*(NpcRoot(state,to)-NpcRoot(state,from)));
            for(int i=0;i<state.hits.Length;i++){
                var hit=state.hits[i];float hitEnd=Mine2RailLoopV21(state)?100:NpcLakeNightHitEndV19(state,hit);
                float lo,hi;
                if(timedRush){
                    if(!OriginalRushTimerV45.HitSpan(state,hit,from,to,enemy.npcNextHits[i],out lo,out hi))continue;
                }else{
                    if(to<hit.start||from>hitEnd||to<enemy.npcNextHits[i])continue;
                    lo=Math.Max(from,Math.Max(hit.start,enemy.npcNextHits[i]));hi=Math.Min(to,hitEnd);
                }
                if(hit.nextDamageCallbackPending){if(float.IsNegativeInfinity(enemy.npcNextHits[i]))TraceNpc(enemy,"pending-native-damage-callback",i);enemy.npcNextHits[i]=float.PositiveInfinity;continue;}
                bool contact=false;
                for(float at=lo;at<=hi+.00001f;at=Math.Min(hi,at+1f/120)){
                    // Fast vines can cross a small target between adjacent
                    // exported 60Hz samples. Interpolate the original helper
                    // candidates at each contact substep instead of rounding
                    // both substeps to opposite ends of a long movement.
                    float sample=Clamp(NpcLakeNightMotionFrameV19(state,at),0,state.samples.Length-1);int index=(int)sample;float blend=sample-index;var spheres=state.samples[index].spheres;var next=state.samples[Math.Min(index+1,state.samples.Length-1)].spheres;
                    float rootY=enemy.y+NativeHoverLiftV58(enemy);
                    for(int j=0;j<spheres.Length;j+=4){float scale=enemy.visualScale,cx=enemy.x+enemy.facing*(spheres[j]+(next[j]-spheres[j])*blend)*scale,cy=rootY+(spheres[j+1]+(next[j+1]-spheres[j+1])*blend)*scale,cz=(spheres[j+2]+(next[j+2]-spheres[j+2])*blend)*scale,radius=(spheres[j+3]+(next[j+3]-spheres[j+3])*blend)*scale;float px=Clamp(cx,player.x-9.8f,player.x+9.8f),py=Clamp(cy,player.y,player.y+42),pz=Clamp(cz,-9.8f,9.8f);if((px-cx)*(px-cx)+(py-cy)*(py-cy)+(pz-cz)*(pz-cz)<=radius*radius){contact=true;break;}}
                    if(!contact)contact=NpcStateLinesTouchPlayer(enemy,state,at);
                    if(contact||at>=hi)break;
                }
                if(!contact)continue;
                int damage=CoreHardOutgoingAttackDamageR1417(enemy,(int)Math.Round(profile.physicalAttack*hit.physical+profile.magicAttack*hit.magic));
                int dealt=ApplyEnemyAttackToPlayer(damage,enemy.x,hit.reaction,hit.launch);
                if(dealt>0){
                    if(state.drain)RecoverEnemyHealth(enemy,dealt);
                    // One target's source hit memory spans the attack's windows.
                    // A second authored window is not itself a ClearHitUnitList.
                    float nextContact=hit.repeat?to+hit.repeatGap:float.PositiveInfinity;
                    for(int j=0;j<enemy.npcNextHits.Length;j++)enemy.npcNextHits[j]=nextContact;
                    if(!PlayerHasSuperArmor&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp)player.vx=enemy.facing*hit.push;
                    enemy.hitPause=Math.Max(enemy.hitPause,hit.stopAttacker);hitstop=Math.Max(hitstop,hit.stopDefender);TraceNpc(enemy,"hit",i,dealt);
                    if(!RememberGuardianContactV36(enemy,state)&&!RememberCloneComboHitV45(enemy,state)&&!string.IsNullOrEmpty(state.onHitState)&&TryEnterNpcState(enemy,npcStates[profile.id+"/"+state.onHitState]))return true;
                }
            }
            AdvanceFactoryRavenClonePhaseV21(enemy,state,from,to,elapsed-dt);enemy.npcTime=to;
            string rushEnd=timedRush?OriginalRushTimerV45.EndState(state,to):null;
            if(rushEnd!=null&&TryEnterNpcState(enemy,npcStates[profile.id+"/"+rushEnd]))return true;
            if(CompleteSoloEscapeV45(enemy,state,to))return true;
            if(TryCloneSpecialFollowUpV45(enemy,state,from,to))return true;
            if(TryGuardianHitChainV36(enemy,state,from,to)||AdvanceMine2RailTransitionV21(enemy,state,to)||AdvanceNpcLakeNightTransitionV19(enemy,state,from,to)||AdvanceDragonTimerV20(enemy,state,from,to))return true;
            if(AdvanceAlteraRepairmanHealV18(enemy,state,to,elapsed))return true;
            if(!Mine2RailLoopV21(state)&&!state.holdForSourceEvent&&to+1e-6f>=state.duration){TraceNpc(enemy,"motion-end");if(TryEnterNpcState(enemy,npcStates[profile.id+"/"+state.nextState])&&state.flipFacingOnEnd)enemy.facing=-enemy.facing;enemy.npcDecisionElapsed=0;}
            return true;
        }
    }
}
