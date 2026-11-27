namespace ElswordPixel {
    public sealed partial class GameModel {
        readonly System.Runtime.CompilerServices.ConditionalWeakTable<Enemy,NpcStateDefinition> plainLethalPoseV35 =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Enemy,NpcStateDefinition>();

        // The ordinary hit already owns attacker X, facing and support at the lethal
        // packet. Keep only the original display state. The model keeps its existing
        // generic Dead, interrupt, stage-clear and deathTime behavior.
        void RememberPlainLethalPoseV35(Enemy actor,float attackerX) {
            if(actor==null)return;
            bool driller=plainRouteR1258==PlainRecycleR1258 && actor.sourceNPC=="NUI_NASOD_DRILLER_CANNON";
            bool guardian=plainRouteR1258==PlainNormalR1258 && actor.sourceNPC=="NUI_NASOD_GUARDIAN_SMALL";
            bool healer=actor.sourceNPC=="NUI_NASOD_HEALER_BLADER" &&
                CurrentRoom.id!=null && CurrentRoom.id.StartsWith("Dungeon_Altera_Nasod_Factory_Normal-",System.StringComparison.Ordinal);
            bool watch=(plainRouteR1258==PlainNormalR1258 || plainRouteR1258==PlainRecycleR1258) && actor.sourceNPC=="NUI_NASOD_WATCH";
            bool battleRaven=actor.sourceNPC=="NUI_RAVEN" && actor.kind=="raven" &&
                CurrentRoom.id!=null && CurrentRoom.id.StartsWith("Dungeon_Altera_Battle_Air_Ship_Normal-",System.StringComparison.Ordinal);
            if(!driller && !guardian && !healer && !watch && !battleRaven)return;
            string state=!Grounded(actor)?"DYING_SKY":
                (attackerX-actor.x)*actor.facing<0?"DYING_LAND_BACK":"DYING_LAND_FRONT";
            var original=npcStates[actor.sourceNPC+"/"+actor.sourceNPC.Substring(4)+"_"+state];
            plainLethalPoseV35.Add(actor,original);
        }

        public bool TryPlainGuardianLethalPoseV35(Enemy actor,out string name,out float clock) {
            name=null;clock=0;
            if(plainRouteR1258!=PlainNormalR1258 || actor==null || actor.sourceNPC!="NUI_NASOD_GUARDIAN_SMALL" ||
                actor.hp>0 || actor.state!=CombatState.Dead || !plainLethalPoseV35.TryGetValue(actor,out var state))return false;
            name=state.id;clock=actor.deathTime;
            return true;
        }

        public bool TryPlainWatchLethalPoseV36(Enemy actor,out string state,out float clock) {
            state=null;clock=0;
            if(actor==null || actor.sourceNPC!="NUI_NASOD_WATCH" || actor.hp>0 || actor.state!=CombatState.Dead ||
                !plainLethalPoseV35.TryGetValue(actor,out var source))return false;
            state=source.id;clock=actor.deathTime;return true;
        }

        public bool TryBattleRavenLethalPoseV36(Enemy actor,out string state,out string alias,out float clock) {
            state=alias=null;clock=0;
            if(actor==null || actor.sourceNPC!="NUI_RAVEN" || actor.kind!="raven" || actor.hp>0 || actor.state!=CombatState.Dead ||
                !string.IsNullOrEmpty(actor.npcState) || !plainLethalPoseV35.TryGetValue(actor,out var original))return false;
            state=original.id;alias=original.clip;clock=actor.deathTime*original.sourceAnimSpeed;return true;
        }

        public bool TryFactoryHealerLethalPoseV35(Enemy actor,out string state,out float clock) {
            state=null;clock=0;
            if(actor==null || actor.hp>0 || actor.state!=CombatState.Dead || actor.sourceNPC!="NUI_NASOD_HEALER_BLADER" ||
                !plainLethalPoseV35.TryGetValue(actor,out var source))return false;
            state=source.id;clock=actor.deathTime;return true;
        }

        public bool TryMineMachineOriginalPoseV35(Enemy actor,out string alias,out float clock) {
            alias=null;clock=0;
            if(actor==null)return false;
            NpcStateDefinition state;
            if(plainRouteR1258==PlainRecycleR1258 && actor.sourceNPC=="NUI_NASOD_DRILLER_CANNON") {
                if(actor.hp>0 || actor.state!=CombatState.Dead || !plainLethalPoseV35.TryGetValue(actor,out state))return false;
                clock=actor.deathTime;
            } else if(Mine2RailBoundV21(actor)) {
                string name=actor.npcState;
                if(!string.IsNullOrEmpty(name))clock=actor.npcTime;
                else if(actor.hp<=0 && actor.state==CombatState.Dead) {name="NASOD_RAIL_DYING_LAND";clock=actor.deathTime;}
                else if(actor.state==CombatState.Hurt) {name="NASOD_RAIL_DAMAGE";clock=actor.clock;}
                else return false;
                if(!npcStates.TryGetValue(actor.sourceNPC+"/"+name,out state))return false;
            } else return false;
            alias=state.clip;
            clock=System.Math.Max(0,clock*state.sourceAnimSpeed);
            return true;
        }
        // The pinned Recycle source WAIT and WALK each place this fourth
        // SCT_CONDITION_TABLE after Special A, A, B. Existing near choices
        // include equality. DISTANCE_OVER_TARGET_NEAR is the opposite side.
        bool TryPlainDrillerFarAttackV35(Enemy actor,NpcStateDefinition state,float distance) {
            if(plainRouteR1258!=PlainRecycleR1258||actor.sourceNPC!="NUI_NASOD_DRILLER_CANNON"||
                (state.id!="NASOD_DRILLER_CANNON_WAIT"&&state.id!="NASOD_DRILLER_CANNON_WALK")||
                distance<=500*.28f||NpcRoll()>=25)return false;
            var ready=npcStates[actor.sourceNPC+"/NASOD_DRILLER_CANNON_ATTACK_C_READY"];
            if(!TryEnterNpcState(actor,ready))return false;
            actor.npcDecisionElapsed=0;return true;
        }
    }
}
