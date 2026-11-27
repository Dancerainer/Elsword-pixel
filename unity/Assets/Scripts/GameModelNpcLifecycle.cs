using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        public float EnemyRecoveryDuration(Enemy enemy){
            var profile=NpcProfile(enemy);NpcStateDefinition state;
            if(enemy!=null&&enemy.state==CombatState.GettingUp&&!string.IsNullOrEmpty(enemy.npcState)&&npcStates.TryGetValue(enemy.sourceNPC+"/"+enemy.npcState,out state))return state.duration;
            if(profile!=null&&TryOriginalRecoveryV45(enemy,profile,out state))return state.duration;
            return profile!=null&&!string.IsNullOrEmpty(profile.getUpState)&&npcStates.TryGetValue(profile.id+"/"+profile.getUpState,out state)?state.duration:EnemyGetUpDuration;
        }
        bool TryOriginalRecoveryV45(Enemy enemy,NpcDefinition profile,out NpcStateDefinition state){
            state=null;
            string original=OriginalRecoveryV45.For(enemy);
            return !string.IsNullOrEmpty(original)&&!string.IsNullOrEmpty(profile.getUpState)&&npcStates.TryGetValue(profile.id+"/"+original,out state);
        }
        // Optional source recovery for profiles that supply a real state.
        // Forest profiles omit getUpState and retain the existing adapter.
        // BAT has different front recovery and attack-follow-through motions;
        // normal attack motion-end already enters STAND_UP_BACK in its graph.
        void TickNpcLifecycleCore(Enemy enemy,float dt){if(TickBelowBombV20(enemy,dt))return;
            if(TickNpcCaveAction(enemy,dt))return;
            var profile=NpcProfile(enemy);
            bool selected=profile!=null&&!string.IsNullOrEmpty(profile.getUpState)&&enemy.active&&!enemy.dormant&&enemy.hp>0;
            bool sourceRecovery=selected && (PlainIceBackGetUpV35(enemy)||CoreSheriffSelectedGetUpV35(enemy));
            string recoveryId=sourceRecovery?enemy.npcState:(selected?profile.getUpState:null);
            // Source-owned recoveries (including the sheriff's conditional roll) take priority.
            // Other eligible profiles use the motion-end edge of their actual front/back down.
            if(selected&&!sourceRecovery&&TryOriginalRecoveryV45(enemy,profile,out var originalRise)&&
               (enemy.state==CombatState.GettingUp||enemy.npcState==originalRise.id))recoveryId=originalRise.id;
            bool continuing=selected&&enemy.npcState==recoveryId;
            if(!selected||enemy.state!=CombatState.GettingUp&&!continuing){TickNpcTraversalCore(enemy,dt);return;}
            if((Mine2NativeDashGravityV21||NativeTerrain)&&(enemy.state==CombatState.GettingUp||enemy.state==CombatState.Knockdown||continuing)&&!Grounded(enemy)){
                // A physical recovery cannot stand on an absent source floor.
                // Preserve pending down on real landing; do not snap X/Y.
                InterruptNpcAction(enemy);enemy.state=CombatState.Airborne;enemy.knockdownOnLanding=true;enemy.recoveryTime=0;TickEnemyCore(enemy,dt);return;
            }
            var recovery=npcStates[profile.id+"/"+recoveryId];
            if(!continuing){
                EnterNpcState(enemy,recovery);enemy.recoveryTime=recovery.duration;
                enemy.superArmor=Math.Max(enemy.superArmor,recovery.duration+.55f);
            }
            // Generic TickState runs before this wrapper. Keep physical recovery
            // active until the source motion finishes, including frozen poses.
            enemy.state=CombatState.GettingUp;enemy.recoveryTime=Math.Max(.0001f,recovery.duration-enemy.npcTime);
            float previousClock=enemy.clock;TickEnemyCore(enemy,dt);float advanced=enemy.clock-previousClock;
            if(advanced>0){
                // A completed source get-up owns its normal motion-end exit.
                // Finish the physical protection before requesting that exit:
                // the mechanism queue correctly rejects ordinary DAMAGE
                // requests while GettingUp, including a queued WAIT otherwise.
                // Keep the usual cooldown/state request path; this is not a
                // force-enter or a relaxation of protected damage callbacks.
                bool finishing=enemy.npcTime+advanced+1e-6f>=recovery.duration;
                if(finishing){enemy.state=CombatState.Normal;enemy.recoveryTime=0;enemy.stun=0;}
                TickNpcRule(enemy,advanced);
                if(finishing&&enemy.npcState==recoveryId){enemy.state=CombatState.GettingUp;enemy.recoveryTime=.0001f;}
            }
            if(enemy.npcState!=recoveryId){enemy.state=CombatState.Normal;enemy.recoveryTime=0;enemy.superArmor=Math.Max(enemy.superArmor,.55f);}
        }
    }
}
