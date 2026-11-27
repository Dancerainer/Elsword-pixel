using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcEnvironmentRule {
        public string initialState,warningState,activeState,restState,deathState;
        public float interval=2,rate=50,near=560,warningDuration=2,gravity=1120,terminalSpeed=560;
        public string populationNpc,spawnNpc;
        public float waitSeconds,emitAt,ownerHpCost;
    }
    public sealed partial class GameModel {
        sealed class CaveEnvironmentActor {
            internal int line;internal float age,decision,initialY;internal bool hitPlayer;
            internal readonly HashSet<Enemy> hitEnemies=new HashSet<Enemy>();
        }
        readonly Dictionary<Enemy,CaveEnvironmentActor> caveEnvironmentActors=new Dictionary<Enemy,CaveEnvironmentActor>();
        public bool EnemyShowsHealthBar(Enemy enemy){
            if(enemy==null||!enemy.active||enemy.dormant||enemy.hp<=0||enemy.passive)return false;
            if(FactoryPressIdV569(enemy.sourceNPC)||IsRegionMirror(enemy)||IsPeitaCrossbow(enemy)||IsHamelSurvivalSpawner(enemy))return false;
            // Summons may be active even when the same hazard's room placement is passive.
            // The gate switch uses damage to signal the gate, then restores its health.
            switch(enemy.kind){
                case "down-stone-trap":
                case "down-stone-trap-besma":
                    // Room entry can draw before the suspended stone's first AI tick.
                    return !string.IsNullOrEmpty(enemy.npcState) && !enemy.npcState.EndsWith("_WAIT",StringComparison.Ordinal);
                case Switch:
                case "velder_stage6_boss_wall":
                    return false;
                case "ice_rock":
                    return enemy.npcState=="ICE_ROCK_ATTACK";
                case "chilling_spine_trap":
                    return enemy.npcState=="CHILLING_SPINE_TRAP_ATTACK"||enemy.npcState=="CHILLING_SPINE_TRAP_ATTACK_LOOP";
                default:
                    return true;
            }
        }
        public bool EnemyHasNpcInvincibility(Enemy enemy){
            // Both factory presses run a permanent hazard cycle and have no damage or death state.
            if(enemy!=null&&FactoryPressIdV569(enemy.sourceNPC))return true;
            if(PeitaRageStoneProtected(enemy)||PeitaManaEaterProtected(enemy)||PeitaSpecialProtected(enemy)||ExitGateProtected(enemy)||SwampActorHiddenV39(enemy)||FactoryEventSourceInvincibleV21(enemy))return true;
            if(CoreStopIntroOpeningProtectedV21(enemy)||FactoryRavenIntroOpeningProtectedV21(enemy))return true;
            if(AlteraAirshipDirectorInvincibleV18(enemy))return true;
            if(enemy!=null&&enemy.sourceNPC=="NUI_SOLDIER_CROSSBOW"&&NpcProfile(enemy)!=null)EnsureCastleSiegeLinks();
            var profile=NpcProfile(enemy);if(enemy!=null&&enemy.hp>0&&profile!=null&&profile.environmentKind=="timed-steam")return true;
            var state=NpcState(enemy);return enemy!=null&&enemy.hp>0&&state!=null&&state.invincibleEnd>state.invincibleStart&&CoreHardProtectionClockV21(enemy,state)>=state.invincibleStart&&CoreHardProtectionClockV21(enemy,state)<state.invincibleEnd;
        }
        // Optional cave profiles alone enable this branch. Source ACTIVE=false
        // is preserved for environmental stones. All unbound/forest actors
        // retain their existing runner, physics and sound channels.
        bool TickNpcCaveSpecial(Enemy enemy,float dt){if(TickFactoryPressSourceV569(enemy,dt))return true;if(TickFactoryEventStartV21(enemy,dt))return true;
            var profile=NpcProfile(enemy);if(profile==null)return false;
            if(TickPlainShockStickSourceR1477(enemy,profile,dt))return true;
            if(TickPlainOriginalMineHoldR1258(enemy,profile,dt))return true;
            if(TickBelowFortPartLifecycleV21(enemy,profile,dt))return true;ObserveDragonMissingBranchV20(enemy);if(TickBelowTunnelHeldSteamV20(enemy,profile,dt))return true;if(TickMine2StoneV21(enemy,profile,dt))return true;
            if(TickDragonGeyserV20(enemy,profile,dt))return true;
            if(TickAlteraPopulationEnvironmentV18(enemy,profile,dt))return true;
            if(TickNpcSourceSupportEnvironmentV17(enemy,profile,dt))return true;
            if(TickNpcLakeSourceEnvironmentV18(enemy,profile,dt))return true;
            if(profile.environmentKind=="recovery-container")return false;
            if(TickNpcOutskirtsEnvironment(enemy,profile,dt))return true;
            if(TickNpcPassageEnvironment(enemy,profile,dt))return true;
            if(!string.IsNullOrEmpty(profile.environmentKind)){
                if(profile.environment==null)throw new InvalidOperationException("Missing cave environment rule");
                TickNpcEnvironment(enemy,profile,dt);return true;
            }
            return false;
        }
        bool TickNpcCaveAction(Enemy enemy,float dt){
            var profile=NpcProfile(enemy);if(profile==null)return false;
            var state=NpcState(enemy);
            if(state==null||!enemy.active||enemy.dormant||enemy.hp<=0||enemy.stun>0||enemy.state==CombatState.Knockdown||enemy.state==CombatState.GettingUp||enemy.knockdownOnLanding)return false;
            if(state.followHitTarget){
                if(player.hp<=0){TryEnterNpcState(enemy,npcStates[profile.id+"/"+profile.waitState]);return true;}
                if(enemy.hitPause>0){float frozen=Math.Min(dt,enemy.hitPause);enemy.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
                // FollowHitter's native attachment bone is not reconstructed.
                // The playable adapter follows the victim's feet; original
                // attack helper samples, .25 damage and 1s repeat remain used.
                enemy.x=player.x;enemy.y=player.y;enemy.groundLine=player.groundLine;enemy.vx=enemy.vy=0;enemy.clock+=dt;
                TickNpcRule(enemy,dt);return true;
            }
            if(state.allowAirborne&&!Grounded(enemy)){
                float clock=enemy.clock;TickEnemyCore(enemy,dt);
                // Core already runs the rule if this packet lands. Do not
                // advance a second time or dispatch after a reaction/state end.
                if(!Grounded(enemy)&&NpcState(enemy)==state&&enemy.clock>clock)TickNpcRule(enemy,enemy.clock-clock);
                return true;
            }
            return false;
        }
        void EnterNpcEnvironment(Enemy enemy,NpcDefinition profile,string id,CaveEnvironmentActor actor){
            actor.age=0;EnterNpcState(enemy,npcStates[profile.id+"/"+id]);
        }
        void AdvanceNpcEnvironmentPose(Enemy enemy,NpcStateDefinition state,float dt){
            enemy.clock+=dt;
            if(state.loop){AdvanceNpcLoopSounds(enemy,state,enemy.npcTime,dt,elapsed-dt);enemy.npcTime=(enemy.npcTime+dt)%state.duration;}
            else{float to=Math.Min(state.duration,enemy.npcTime+dt);AdvanceNpcStateSounds(enemy,state,enemy.npcTime,to,elapsed-dt);enemy.npcTime=to;}
        }
        void TickNpcEnvironment(Enemy enemy,NpcDefinition profile,float dt){
            if(!enemy.active||enemy.dormant)return;
            // The deferred cannon gate plays its DAMAGE motion once before its
            // source motion-end transition. Door protection and death below
            // still follow the original environment rules.
            if(enemy.hp>0&&enemy.npcState=="CANNON_BIG_GATE_DAMAGE"&&OriginalCannonWreckV39(enemy)){
                var damage=NpcState(enemy);
                AdvanceNpcEnvironmentPose(enemy,damage,dt);
                if(enemy.npcTime+1e-6f>=damage.duration)EnterAlteraCannonGateState(enemy,damage.nextState);
                return;
            }
            CaveEnvironmentActor actor;
            if(!caveEnvironmentActors.TryGetValue(enemy,out actor)){
                // Room reset creates fresh actors; stale dictionaries must not
                // retain every discarded dungeon indefinitely.
                var stale=new List<Enemy>();foreach(var e in caveEnvironmentActors.Keys)if(!enemies.Contains(e))stale.Add(e);foreach(var e in stale)caveEnvironmentActors.Remove(e);
                actor=new CaveEnvironmentActor{line=enemy.groundLine,initialY=enemy.y};caveEnvironmentActors.Add(enemy,actor);
                EnterNpcEnvironment(enemy,profile,profile.environment.initialState,actor);
            }
            var rule=profile.environment;
            if(profile.environmentKind!="falling-stone"){
                // The source post and door are separate attackable actors.
                // Demo linkage removes a leftover co-located blocker after
                // either paired actor breaks; only the source door is a clear
                // predicate, so a decorative post cannot soft-lock the exit.
                foreach(var partner in enemies){var p=NpcProfile(partner);if(partner==enemy||p==null||p.environmentKind=="falling-stone"||string.IsNullOrEmpty(p.environmentKind))continue;
                    if(partner.hp<=0&&Math.Abs(partner.x-enemy.x)<1&&Math.Abs(partner.y-enemy.y)<1){enemy.hp=0;enemy.state=CombatState.Dead;break;}}
                if(enemy.hp<=0){
                    if(enemy.npcState!=rule.deathState)EnterNpcEnvironment(enemy,profile,rule.deathState,actor);
                    enemy.deathTime+=dt;AdvanceNpcEnvironmentPose(enemy,NpcState(enemy),dt);return;
                }
                bool live=false;foreach(var other in enemies)if(other.active&&!other.dormant&&other.hp>0&&other.countsForClear&&!other.passive&&!other.isGate&&other.encounterIndex==enemy.encounterIndex){live=true;break;}
                string desired=live?rule.initialState:rule.restState;
                if(enemy.npcState!=desired)EnterNpcEnvironment(enemy,profile,desired,actor);
                AdvanceNpcEnvironmentPose(enemy,NpcState(enemy),dt);return;
            }
            if(enemy.hp<=0){
                if(enemy.npcState!=rule.deathState)EnterNpcEnvironment(enemy,profile,rule.deathState,actor);
                enemy.deathTime+=dt;AdvanceNpcEnvironmentPose(enemy,NpcState(enemy),dt);return;
            }
            var state=NpcState(enemy);
            if(state==null){
                // Ground DAMAGE returns to WAIT2, but the falling-state END
                // callback must not replay just because a landed stone hurt.
                enemy.npcState=rule.restState;enemy.npcTime=enemy.clock=0;state=NpcState(enemy);enemy.clip=state.clip;CancelNpcStateSounds(enemy);
            }
            actor.age+=dt;AdvanceNpcEnvironmentPose(enemy,state,dt);
            if(state.id==rule.initialState){
                enemy.y=actor.initialY;enemy.vx=enemy.vy=0;enemy.passive=true;
                actor.decision+=dt;if(actor.decision<rule.interval)return;actor.decision%=rule.interval;
                // SAME_LINE is adapted to the source spawn support, not the
                // stone's suspended height. No extra aggro damage is required.
                bool sameLine=!NativeTerrain||player.groundLine==actor.line;
                float dx=player.x-enemy.x,dy=player.y-enemy.y;
                if(dx*dx+dy*dy<=rule.near*rule.near&&sameLine&&NpcRoll()<rule.rate)EnterNpcEnvironment(enemy,profile,rule.warningState,actor);
            }else if(state.id==rule.warningState){
                enemy.y=actor.initialY;enemy.vx=enemy.vy=0;enemy.passive=true;
                if(actor.age>=rule.warningDuration){EnterNpcEnvironment(enemy,profile,rule.activeState,actor);enemy.state=CombatState.Airborne;enemy.groundLine=-1;actor.hitPlayer=false;actor.hitEnemies.Clear();}
            }else if(state.id==rule.activeState){
                float before=enemy.y;enemy.vy=Math.Max(-rule.terminalSpeed,enemy.vy-rule.gravity*dt);enemy.y+=enemy.vy*dt;
                var floor=NativeTerrain?SurfaceBelow(enemy.x,before+.05f):null;float ground=floor==null?GroundAt(enemy.x):floor.Y(enemy.x);
                bool landed=enemy.y<=ground;if(landed){enemy.y=ground;enemy.vy=0;enemy.groundLine=floor==null?-1:floor.id;}
                // Swept source helper projection prevents a fast falling stone
                // tunnelling through the hero between adjacent model steps.
                if(!actor.hitPlayer&&CaveStoneTouches(enemy,state,before,new AttackBox{left=player.x-9.8f,right=player.x+9.8f,bottom=player.y,top=player.y+42})){
                    var hit=state.hits[0];int dealt=ApplyHitToPlayer((int)Math.Round(profile.physicalAttack*hit.physical),enemy.x,hit.reaction,hit.launch);if(dealt>0){actor.hitPlayer=true;TraceNpc(enemy,"trap-hit-player",0,dealt);}
                }
                // ATTACK_ALL_TEAM is explicit in the stone source. Other
                // stones, gates and noncombat props are excluded as adapters.
                foreach(var other in enemies){if(other==enemy||other.passive||other.isGate||other.hp<=0||!other.active||other.dormant||EnemyHasNpcInvincibility(other)||actor.hitEnemies.Contains(other))continue;
                    if(CaveStoneTouches(enemy,state,before,EnemyBounds(other))){var hit=state.hits[0];int dealt=ApplyHit(other,(int)Math.Round(profile.physicalAttack*hit.physical),hit.reaction,enemy.x,hit.launch,false);if(dealt>0){actor.hitEnemies.Add(other);TraceNpc(enemy,"trap-hit-npc",0,dealt);}}
                }
                if(landed){enemy.state=CombatState.Normal;enemy.passive=false;enemy.aiEnabled=false;EnterNpcEnvironment(enemy,profile,rule.restState,actor);CancelNpcStateSounds(enemy);SpawnEffect("dust",enemy.x,enemy.y);}
            }else{enemy.vx=enemy.vy=0;enemy.passive=false;enemy.aiEnabled=false;}
        }
        bool CaveStoneTouches(Enemy stone,NpcStateDefinition state,float previousY,AttackBox target){
            var spheres=state.samples[Math.Min(state.samples.Length-1,(int)(stone.npcTime*60))].spheres;
            for(int i=0;i<spheres.Length;i+=4){
                float x,y,r;if(!NpcCircle(stone,spheres,i,out x,out y,out r))continue;
                float xGap=x-Clamp(x,target.left,target.right),lo=y,hi=y+previousY-stone.y;
                float yGap=lo>target.top?lo-target.top:hi<target.bottom?target.bottom-hi:0;
                if(xGap*xGap+yGap*yGap<=r*r)return true;
            }
            return false;
        }
    }
}
