using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        static bool NoNpcRows<T>(T[] rows){return rows==null||rows.Length==0;}
        // Unity's inline serialization can materialize an omitted nested DTO
        // as an empty instance instead of null. Normalize only its complete
        // empty/default signature, never a partially authored rule.
        static void NormalizeNpcOptionalRules(NpcDefinition profile){
            var summon=profile.summon;
            if(summon!=null&&string.IsNullOrEmpty(summon.state)&&string.IsNullOrEmpty(summon.sourceCallback)&&summon.emitAt==0&&NoNpcRows(summon.decisionStates)&&NoNpcRows(summon.requireAbsentTypes)&&NoNpcRows(summon.spawns))profile.summon=null;
            var t=profile.traversal;
            if(t!=null&&string.IsNullOrEmpty(t.sourcePrefix)&&t.upVelocity==0&&t.directionalUpVelocity==0&&t.upHorizontalSpeed==0&&t.downHorizontalSpeed==0&&t.entryOffset==0&&t.interval==0&&t.landingDuration==0&&t.maximumAssistedUpVelocity==0)profile.traversal=null;
            var e=profile.environment;
            if(string.IsNullOrEmpty(profile.environmentKind)&&e!=null&&string.IsNullOrEmpty(e.initialState)&&string.IsNullOrEmpty(e.warningState)&&string.IsNullOrEmpty(e.activeState)&&string.IsNullOrEmpty(e.restState)&&string.IsNullOrEmpty(e.deathState)&&string.IsNullOrEmpty(e.populationNpc)&&string.IsNullOrEmpty(e.spawnNpc)&&e.waitSeconds==0&&e.emitAt==0&&e.ownerHpCost==0){
                bool zero=e.interval==0&&e.rate==0&&e.near==0&&e.warningDuration==0&&e.gravity==0&&e.terminalSpeed==0;
                bool defaults=e.interval==2&&e.rate==50&&e.near==560&&e.warningDuration==2&&e.gravity==1120&&e.terminalSpeed==560;
                if(zero||defaults)profile.environment=null;
            }
        }
        static void ValidateNpcEnvironment(NpcDefinition profile){
            if(ValidateBelowTunnelHeldSteamV20(profile))return;if(ValidateDragonEnvironmentV20(profile))return;
            if(ValidateAlteraPopulationEnvironmentV18(profile))return;
            if(ValidateNpcSourceSupportEnvironmentV17(profile))return;
            if(ValidateNpcLakeSourceEnvironmentV18(profile))return;
            var e=profile.environment;
            if(string.IsNullOrEmpty(profile.environmentKind)){if(e!=null)throw new ArgumentException("NPC environment payload has no kind");return;}
            if(profile.environmentKind=="recovery-container")return;
            if(profile.environmentKind=="proximity-trap"){ValidateNpcOutskirtsEnvironment(profile);return;}
            if(profile.environmentKind=="timed-steam"||profile.environmentKind=="mouse-house")return; // Strict state/parameter/dependency checks follow after every profile is known.
            if(e==null||(profile.environmentKind!="falling-stone"&&profile.environmentKind!="gate-post"&&profile.environmentKind!="source-gate"&&profile.environmentKind!="siege-bunker"))throw new ArgumentException("Invalid NPC environment kind/binding");
            foreach(string id in new[]{e.initialState,e.restState,e.deathState})if(string.IsNullOrEmpty(id)||!Array.Exists(profile.states,s=>s.id==id))throw new ArgumentException("Missing NPC environment state");
            if(profile.environmentKind!="falling-stone")return;
            foreach(string id in new[]{e.warningState,e.activeState})if(string.IsNullOrEmpty(id)||!Array.Exists(profile.states,s=>s.id==id))throw new ArgumentException("Missing falling-stone state");
            if(!Finite(e.interval)||e.interval<=0||!Finite(e.rate)||e.rate<0||e.rate>100||!Finite(e.near)||e.near<=0||!Finite(e.warningDuration)||e.warningDuration<=0||!Finite(e.gravity)||e.gravity<=0||!Finite(e.terminalSpeed)||e.terminalSpeed<=0)throw new ArgumentException("Invalid falling-stone timing/physics");
        }
        static bool EmptyNpcRetaliation(NpcRetaliationRule r){
            if(r==null)return true;
            if(r.sourceDamageFromFrontOnly)return false;
            if(!string.IsNullOrEmpty(r.npc)||!string.IsNullOrEmpty(r.sourceState)||!string.IsNullOrEmpty(r.sourceCallback)||!string.IsNullOrEmpty(r.cState)||!string.IsNullOrEmpty(r.specialState))return false;
            bool zero=r.timerIndex==0&&r.cRollExclusive==0&&r.specialRollExclusive==0&&r.timerInitial==0&&!r.timerEnabled&&!r.stateChangeArgument&&r.minimumTimerExclusive==0&&r.maximumSourceDistance==0&&r.cHpMaximum==0&&r.specialHpMaximum==0;
            bool defaults=r.timerIndex==1&&r.cRollExclusive==10&&r.specialRollExclusive==15&&r.timerInitial==0&&r.timerEnabled&&r.stateChangeArgument&&r.minimumTimerExclusive==1&&r.maximumSourceDistance==500&&r.cHpMaximum==.7&&r.specialHpMaximum==.4;
            return zero||defaults;
        }
    }
}
