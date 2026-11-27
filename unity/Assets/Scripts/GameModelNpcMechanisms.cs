using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcMechanismStateRule {
        public string npc,state,sourceAnimation;public int sourceIndex;public float duration,cooldown,initialElapsed;
        public bool loop,cooldownExplicit,initialElapsedExplicit;
    }
    [Serializable] public sealed class NpcRetaliationRule {
        public string npc,sourceState,sourceCallback,cState,specialState;
        public bool sourceDamageFromFrontOnly;
        public int timerIndex=1,cRollExclusive=10,specialRollExclusive=15;
        public float timerInitial;public bool timerEnabled=true,stateChangeArgument=true;
        public double minimumTimerExclusive=1,maximumSourceDistance=500,cHpMaximum=.7,specialHpMaximum=.4;
    }
    [Serializable] public sealed class NpcMechanismDocument {
        public AlteraSteamControllerRuleV18[] alteraSteamControllers=new AlteraSteamControllerRuleV18[0];
        public NpcLakeVisualDocumentV18 lakeVisual;public AlteraRavenMaximumRuleV18 alteraRavenMaximum;public AlteraRavenMissileRainRuleV18 alteraRavenMissileRain;
        public AlteraNasodPopulationRuleV18[] alteraNasodPopulations=new AlteraNasodPopulationRuleV18[0];
        public string sourceRulesSha256,nativeEvidenceSha256;public bool runtimeAdmitted,sourceMechanismsComplete;
        public NpcMechanismStateRule[] states=new NpcMechanismStateRule[0];public NpcRetaliationRule retaliation;
        public NpcRetaliationRule[] retaliations=new NpcRetaliationRule[0];
        public NpcSourceLandingEmission[] sourceLandingEmissions=new NpcSourceLandingEmission[0];
        public CargoDirectorHostDocumentV21 sourceCargoDirectorV21;public CargoSourceDocumentV21 sourceCargoV21;public NpcSourceSupportDocumentV17 sourceSupport;public NpcMine2SourceDocumentV20 sourceMine2;public Mine2MechanismDocumentV21 sourceMine2MechanismsV21;
        public NpcLakeSourceDocumentV18 sourceLake;public NpcLakeNightDocumentV19 sourceLakeNight;public NpcDragonNestDocumentV20 sourceDragonNest;public NpcBelowTunnelSourceDocumentV20 sourceBelowTunnel;public BelowFortSourceDocumentV20 sourceBelowFort;public BelowFxSourceDocumentV20 sourceBelowFx;public BelowBombSourceDocumentV20 sourceBelowBomb;public BelowLaserPhaseDocumentV20 sourceBelowLaserPhase;public BelowMiniUpWaitDocumentV20 sourceBelowMiniUpWait;public BelowTunnelCandidateDocumentV21 sourceBelowCandidateV21;public BelowVisibilityDocumentV21 sourceBelowVisibilityV21;
        public AlteraRepairmanRuleV18[] alteraRepairmen=new AlteraRepairmanRuleV18[0];
        public AlteraAirshipIntroRuleV18[] alteraAirshipIntros=new AlteraAirshipIntroRuleV18[0];
    }
    [Serializable] public sealed class NpcMechanismEvent {
        public string npc,kind,state;public float time,timer,stateElapsed;public int roll;public double hpRatio,sourceDistance;
    }
    public sealed partial class GameModel {
        sealed class NpcMechanismActor {
            internal readonly Dictionary<string,float> elapsed=new Dictionary<string,float>(StringComparer.Ordinal);
            internal float timer,previousTimer;internal bool timerEnabled;
            internal string pending;internal bool pendingDamage;
        }
        readonly Dictionary<string,NpcMechanismStateRule> npcMechanismRules=new Dictionary<string,NpcMechanismStateRule>(StringComparer.Ordinal);
        readonly Dictionary<Enemy,NpcMechanismActor> npcMechanismActors=new Dictionary<Enemy,NpcMechanismActor>();
        readonly Dictionary<string,NpcRetaliationRule> npcRetaliations=new Dictionary<string,NpcRetaliationRule>(StringComparer.Ordinal);
        uint npcRetaliationRandom=0x47a193u;
        public bool NpcMechanismIsHost=true;
        // Explicit test/replay adapter. The original math.random sequence is not
        // claimed reproduced; its integer domain and invocation order are.
        public Func<int> NpcRetaliationRollOverride;
        public readonly List<NpcMechanismEvent> npcMechanismEvents=new List<NpcMechanismEvent>();
        public int NpcRetaliationDraws {get;private set;}
        public bool HasNpcMechanisms {get{return npcMechanismRules.Count!=0;}}

        public void SetNpcMechanismDefinitions(NpcMechanismDocument document){
            var rules=new Dictionary<string,NpcMechanismStateRule>(StringComparer.Ordinal);
            var callbacks=new Dictionary<string,NpcRetaliationRule>(StringComparer.Ordinal);
            if(document!=null){
                if(document.states==null||document.states.Length==0)throw new ArgumentException("Missing source NPC mechanism states");
                foreach(var row in document.states){
                    NpcStateDefinition state;string key=row==null?null:row.npc+"/"+row.state;
                    if(row==null||string.IsNullOrEmpty(row.npc)||string.IsNullOrEmpty(row.state)||rules.ContainsKey(key)||!npcStates.TryGetValue(key,out state)||
                       row.sourceAnimation!=state.sourceAnimation||row.loop!=state.loop||!Finite(row.duration)||Math.Abs(row.duration-state.duration)>.00001f||
                       !Finite(row.cooldown)||row.cooldown<0||!Finite(row.initialElapsed)||row.initialElapsed<0||!row.cooldownExplicit&&row.cooldown!=0||!row.initialElapsedExplicit&&row.initialElapsed!=0)
                        throw new ArgumentException("Invalid NPC mechanism state binding/default");
                    rules.Add(key,row);
                }
                var declared=new List<NpcRetaliationRule>();if(!EmptyNpcRetaliation(document.retaliation))declared.Add(document.retaliation);if(document.retaliations!=null)declared.AddRange(document.retaliations);
                foreach(var r in declared){
                if(r==null||string.IsNullOrEmpty(r.npc)||callbacks.ContainsKey(r.npc)||string.IsNullOrEmpty(r.sourceState)||string.IsNullOrEmpty(r.sourceCallback)||
                    !rules.ContainsKey(r.npc+"/"+r.cState)||!rules.ContainsKey(r.npc+"/"+r.specialState)||r.timerIndex!=1||!Finite(r.timerInitial)||r.timerInitial<0||
                    double.IsNaN(r.minimumTimerExclusive)||double.IsInfinity(r.minimumTimerExclusive)||r.minimumTimerExclusive<0||
                    double.IsNaN(r.maximumSourceDistance)||double.IsInfinity(r.maximumSourceDistance)||r.maximumSourceDistance<=0||
                    !(r.cHpMaximum>=0&&r.cHpMaximum<=1)||!(r.specialHpMaximum>=0&&r.specialHpMaximum<=1)||
                    r.cRollExclusive<1||r.specialRollExclusive<=r.cRollExclusive||r.specialRollExclusive>101)throw new ArgumentException("Invalid NPC DAMAGE callback binding");
                ValidateAlteraRavenFrontDamageRuleV18(r,rules);callbacks.Add(r.npc,r);
                }
            }
            SetBelowTunnelCandidateV21(null);
            SetNpcSourceLandingEmissionsV17(document==null?null:document.sourceLandingEmissions);
            npcMechanismRules.Clear();foreach(var pair in rules)npcMechanismRules.Add(pair.Key,pair.Value);
            npcRetaliations.Clear();foreach(var pair in callbacks)npcRetaliations.Add(pair.Key,pair.Value);
            npcMechanismActors.Clear();npcMechanismEvents.Clear();NpcRetaliationDraws=0;npcRetaliationRandom=0x47a193u;
            SetNpcSourceSupportV17(document==null?null:document.sourceSupport);
            SetNpcLakeSourceV18(document==null?null:document.sourceLake);SetNpcLakeNightV19(document==null?null:document.sourceLakeNight);
            SetNpcDragonNestV20(document==null?null:document.sourceDragonNest);SetNpcBelowTunnelSourceV20(document==null?null:document.sourceBelowTunnel);SetNpcBelowFortV20(document==null?null:document.sourceBelowFort);SetNpcBelowFxV20(document==null?null:document.sourceBelowFx);SetNpcBelowBombV20(document==null?null:document.sourceBelowBomb);SetNpcBelowLaserDamagePhasesV20(document==null?null:document.sourceBelowLaserPhase);SetNpcBelowMiniUpWaitV20(document==null?null:document.sourceBelowMiniUpWait);SetNpcMine2SourceV20(document==null?null:document.sourceMine2);SetMine2MechanismsV21(document==null?null:document.sourceMine2MechanismsV21);
            SetAlteraRepairmanRulesV18(document==null?null:document.alteraRepairmen);
            SetAlteraAirshipIntroDefinitionsV18(document==null?null:document.alteraAirshipIntros);
            SetAlteraNasodPopulationRulesV18(document==null?null:document.alteraNasodPopulations);
            SetAlteraSteamControllerRulesV18(document==null?null:document.alteraSteamControllers);
            SetNpcLakeVisualV18(document==null?null:document.lakeVisual);SetAlteraRavenMaximumReleaseV18(document==null?null:document.alteraRavenMaximum);SetAlteraRavenMissileRainV18(document==null?null:document.alteraRavenMissileRain);SetBelowTunnelCandidateV21(document==null?null:document.sourceBelowCandidateV21);SetBelowTunnelVisibilityV21(document==null?null:document.sourceBelowVisibilityV21);SetCargoSourceV21(document==null?null:document.sourceCargoV21);SetCargoDirectorHostV21(document==null?null:document.sourceCargoDirectorV21);
        }
        void InvalidateNpcMechanismDefinitions(){SetNpcMechanismDefinitions(null);}
        NpcMechanismActor MechanismActor(Enemy enemy){
            if(enemy==null||!HasNpcMechanisms||NpcProfile(enemy)==null)return null;
            NpcMechanismActor actor;if(npcMechanismActors.TryGetValue(enemy,out actor))return actor;
            actor=new NpcMechanismActor();foreach(var rule in npcMechanismRules.Values)if(rule.npc==enemy.sourceNPC)actor.elapsed.Add(rule.state,rule.initialElapsed);
            if(actor.elapsed.Count==0)return null;
            NpcRetaliationRule retaliation;if(npcRetaliations.TryGetValue(enemy.sourceNPC,out retaliation)){actor.timer=retaliation.timerInitial;actor.timerEnabled=retaliation.timerEnabled;}
            npcMechanismActors.Add(enemy,actor);return actor;
        }
        void PruneNpcMechanismActors(){
            var stale=new List<Enemy>();foreach(var e in npcMechanismActors.Keys)if(!enemies.Contains(e))stale.Add(e);
            foreach(var e in stale)npcMechanismActors.Remove(e);
        }
        public float NpcSourceTimer1(Enemy enemy){var actor=MechanismActor(enemy);return actor==null?0:actor.timer;}
        public float NpcSourcePreviousTimer1(Enemy enemy){var actor=MechanismActor(enemy);return actor==null?0:actor.previousTimer;}
        public float NpcSourceStateElapsed(Enemy enemy,string state){var actor=MechanismActor(enemy);float value;return actor!=null&&actor.elapsed.TryGetValue(state,out value)?value:0;}
        public string NpcPendingStateRequest(Enemy enemy){NpcMechanismActor actor;return enemy!=null&&npcMechanismActors.TryGetValue(enemy,out actor)?actor.pending:null;}
        void MechanismEvent(Enemy e,string kind,string state=null,int roll=0,double hp=0,double distance=0){
            if(npcMechanismEvents.Count>=2048)npcMechanismEvents.RemoveAt(0);
            npcMechanismEvents.Add(new NpcMechanismEvent{npc=e.sourceNPC,kind=kind,state=state,time=elapsed,timer=NpcSourceTimer1(e),stateElapsed=state==null?0:NpcSourceStateElapsed(e,state),roll=roll,hpRatio=hp,sourceDistance=distance});
        }
        // Original3250f420 discards the second ABI argument (RET8) without
        // reading it. TRUE cannot bypass the target's elapsed>=cooldown gate.
        // Queue only: no animation/stun mutation occurs on the damage stack.
        public bool TryRequestNpcState(Enemy enemy,string state,bool sourceArgument=true){
            if(enemy==null||string.IsNullOrEmpty(state)||!NpcMechanismIsHost||enemy.hp<=0||!enemy.active||enemy.dormant||enemy.passive||enemy.isGate||
               enemy.state==CombatState.Dead||enemy.state==CombatState.Knockdown||enemy.state==CombatState.GettingUp)return false;
            NpcMechanismStateRule rule;var actor=MechanismActor(enemy);
            if(actor==null||!npcMechanismRules.TryGetValue(enemy.sourceNPC+"/"+state,out rule))return false;
            float value=actor.elapsed[state];
            if(!(value>=rule.cooldown)){MechanismEvent(enemy,"request-cooldown",state);return false;}
            actor.elapsed[state]=0;actor.pending=state;actor.pendingDamage=false;MechanismEvent(enemy,"request-accepted",state);return true;
        }
        void CancelNpcMechanismRequest(Enemy enemy){
            NpcMechanismActor actor;if(enemy!=null&&npcMechanismActors.TryGetValue(enemy,out actor)){actor.pending=null;actor.pendingDamage=false;}
        }
        bool CommitNpcStateRequest(Enemy enemy){
            NpcMechanismActor actor;if(!npcMechanismActors.TryGetValue(enemy,out actor)||actor.pending==null)return false;
            var current=NpcState(enemy);bool sourceAirTransition=!actor.pendingDamage&&current!=null&&(current.allowAirborne||current.followHitTarget||CargoSourceFlightStateV21(enemy,current)||FactoryRavenCloneAirborneRequestV21(enemy,current,actor.pending))&&enemy.stun<=0&&!enemy.knockdownOnLanding;
            if(enemy.hp<=0||!enemy.active||enemy.dormant||enemy.passive||enemy.isGate||!sourceAirTransition&&(!Grounded(enemy)||enemy.state==CombatState.Airborne)||
               enemy.state==CombatState.Knockdown||enemy.state==CombatState.GettingUp||enemy.state==CombatState.Dead){CancelNpcMechanismRequest(enemy);return false;}
            string state=actor.pending;bool damage=actor.pendingDamage;actor.pending=null;actor.pendingDamage=false;
            if(damage){
                // Ground-Hurt adapter: commit only after all damage setters have
                // completed, so ApplySourceHit cannot re-stun the new ready pose.
                enemy.state=CombatState.Normal;enemy.stun=0;enemy.vx=enemy.vy=0;enemy.knockdownOnLanding=false;
            }
            EnterNpcState(enemy,npcStates[enemy.sourceNPC+"/"+state]);MechanismEvent(enemy,"request-committed",state);return true;
        }
        bool TryEnterNpcState(Enemy enemy,NpcStateDefinition state){
            // Without an optional sidecar, this is exactly the existing runner.
            if(!npcMechanismRules.ContainsKey(enemy.sourceNPC+"/"+state.id)){EnterNpcState(enemy,state);return true;}
            // A fractional demo stop may let Core reach AI this packet while
            // the source old-stop gate still defers a queued request. Preserve
            // that request until the next eligible actor update.
            if(NpcPendingStateRequest(enemy)!=null)return false;
            return TryRequestNpcState(enemy,state.id)&&CommitNpcStateRequest(enemy);
        }
        int RetaliationRoll(){
            int roll;if(NpcRetaliationRollOverride!=null)roll=NpcRetaliationRollOverride();else{npcRetaliationRandom^=npcRetaliationRandom<<13;npcRetaliationRandom^=npcRetaliationRandom>>17;npcRetaliationRandom^=npcRetaliationRandom<<5;roll=(int)(npcRetaliationRandom%100)+1;}
            if(roll<1||roll>100)throw new InvalidOperationException("NPC retaliation RNG must produce integer1..100");NpcRetaliationDraws++;return roll;
        }
        void NotifyNpcDamageEntry(Enemy enemy,float sourceDamageFromX=float.NaN){
            NpcRetaliationRule rule;if(enemy==null||!npcRetaliations.TryGetValue(enemy.sourceNPC,out rule)||enemy.state!=CombatState.Hurt||!Grounded(enemy)||enemy.hp<=0)return;
            if(!AlteraRavenFrontDamageEntryAllowedV18(enemy,rule,sourceDamageFromX))return;
            var actor=MechanismActor(enemy);if(actor==null)return;
            if(!NpcMechanismIsHost){MechanismEvent(enemy,"damage-nonhost");return;}
            // Exact Lua ordering: HP and distance first, then random BEFORE
            // distance and timer rejection. No RNG draw is hidden by trace.
            double hp=(double)enemy.hp/enemy.maxHp;double dx=(double)player.x-enemy.x,dy=(double)player.y-enemy.y;
            double distance=Math.Sqrt(dx*dx+dy*dy)/NativeToLogical;int roll=RetaliationRoll();
            MechanismEvent(enemy,"damage-roll",null,roll,hp,distance);
            if(distance>rule.maximumSourceDistance||actor.timer<=rule.minimumTimerExclusive)return;
            string requested=null;
            if(roll<rule.cRollExclusive){if(hp<=rule.cHpMaximum)requested=rule.cState;}
            else if(roll<rule.specialRollExclusive&&hp<=rule.specialHpMaximum)requested=rule.specialState;
            if(requested!=null&&TryRequestNpcState(enemy,requested,rule.stateChangeArgument))actor.pendingDamage=true;
            // This is outside the Lua roll/HP branches and state-request result.
            actor.timer=actor.previousTimer=0;MechanismEvent(enemy,"timer-restart",requested,roll,hp,distance);
        }
        // Native outer3253c810 applies/frames before incrementing per-state
        // cooldowns. Timer1 belongs to v374 and is skipped on OLD-positive STOP.
        // Core retains the existing demo's fractional hitpause physics; these
        // independent source clocks deliberately do not conflate that adapter.
        private void TickEnemy(Enemy enemy,float dt){RecoverEscapedObjective(enemy);if(TickSwampIntroActorV39(enemy))return;TickFactoryEntryActivationV542(enemy);
            if(TickInitialNpcPoseV25(enemy,dt))return;float previousClock=enemy.clock;
            try{TickEnemyWithoutAlteraDeferredV17(enemy,dt);}
            finally{TickAlteraDeferredCannonDeathV17(enemy,Math.Max(0,enemy.clock-previousClock));}
        }
        void TickEnemyWithoutAlteraDeferredV17(Enemy enemy,float dt){
            if(TickPlainIceRecoveryDownV35(enemy,dt)||TickCoreSheriffDownV35(enemy,dt))return;
            if(TickNpcCastleCenterSpecial(enemy,dt))return;
            if(TickNpcCaveSpecial(enemy,dt))return;
            if(!HasNpcMechanisms){TickNpcLifecycleCore(enemy,dt);return;}
            PruneNpcMechanismActors();var actor=MechanismActor(enemy);
            if(actor==null){TickNpcLifecycleCore(enemy,dt);return;}
            bool eligible=enemy.active&&!enemy.dormant&&!enemy.passive&&!enemy.isGate&&enemy.hp>0;
            bool oldStopped=enemy.hitPause>0;
            try{
                if(eligible&&!oldStopped){
                    CheckOriginalGroundCounterV45(enemy,actor,dt);
                    CommitNpcStateRequest(enemy);
                    actor.previousTimer=actor.timer;if(actor.timerEnabled)actor.timer+=dt;
                }
                TickNpcLifecycleCore(enemy,dt);
            }finally{
                if(eligible)foreach(var rule in npcMechanismRules.Values)if(rule.npc==enemy.sourceNPC&&rule.cooldown>0)actor.elapsed[rule.state]+=dt;
            }
        }
    }
}
