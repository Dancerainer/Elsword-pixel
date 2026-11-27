using System.Runtime.CompilerServices;
namespace ElswordPixel {
    public sealed partial class GameModel {
        const string GuardianPrefixV36="NASOD_GUARDIAN_SMALL_";
        sealed class GuardianEntryV36 { public int completedWaitCycles;public bool hit; }
        readonly ConditionalWeakTable<Enemy,GuardianEntryV36> guardianEntriesV36=new ConditionalWeakTable<Enemy,GuardianEntryV36>();
        bool GuardianScopeV36(Enemy actor) {
            return plainRouteR1258==PlainNormalR1258 && actor!=null && actor.sourceNPC=="NUI_NASOD_GUARDIAN_SMALL";
        }
        bool GuardianIdleScopeV36(Enemy actor) {
            return GuardianScopeV36(actor) && (string.IsNullOrEmpty(actor.npcState) ||
                actor.npcState==GuardianPrefixV36+"WAIT" || actor.npcState==GuardianPrefixV36+"WAIT3");
        }
        void ResetGuardianEntryV36(Enemy actor) {
            if(GuardianScopeV36(actor))guardianEntriesV36.Remove(actor);
        }
        bool TryGuardianWait3V36(Enemy actor,NpcStateDefinition state,float dt) {
            if(!GuardianScopeV36(actor) || state.id!=GuardianPrefixV36+"WAIT" || actor.npcTime+dt<state.duration)return false;
            var entry=guardianEntriesV36.GetOrCreateValue(actor);
            // One check on the fifteenth completed Wait animation of this entry.
            if(++entry.completedWaitCycles!=15 || NpcRoll()>=50)return false;
            return TryEnterNpcState(actor,npcStates[actor.sourceNPC+"/"+GuardianPrefixV36+"WAIT3"]);
        }
        bool RememberGuardianContactV36(Enemy actor,NpcStateDefinition state) {
            if(!GuardianScopeV36(actor) || (state.id!=GuardianPrefixV36+"ATTACK" && state.id!=GuardianPrefixV36+"TAIL_ATTACK"))return false;
            guardianEntriesV36.GetOrCreateValue(actor).hit=true;return true;
        }
        bool TryGuardianHitChainV36(Enemy actor,NpcStateDefinition state,float from,float to) {
            if(!GuardianScopeV36(actor))return false;
            bool tail=state.id==GuardianPrefixV36+"TAIL_ATTACK";
            if(!tail && state.id!=GuardianPrefixV36+"ATTACK")return false;
            // Original SCT_CONDITION_TABLE is an ANIM_EVENT_TIMER crossing in
            // source seconds. Positive damage belongs to this state entry only.
            // A hit arriving after the timer cannot replay an elapsed crossing.
            float at=(tail?.90f:.88f)/state.sourceAnimSpeed;
            if(from>=at || to<at || !guardianEntriesV36.TryGetValue(actor,out var entry) || !entry.hit ||
                NpcRoll()>=(tail?40:30))return false;
            string next=GuardianPrefixV36+(tail?"DASH_ATTACK":"TAIL_ATTACK");
            return TryEnterNpcState(actor,npcStates[actor.sourceNPC+"/"+next]);
        }
    }
}
