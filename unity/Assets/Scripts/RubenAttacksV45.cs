using System;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] internal sealed class RubenAttackDocumentV45 {
        public NpcDefinition[] profiles;
        public NpcEffectBinding[] bindings;
        public NpcEffectDefinition[] effects;
        public NpcMechanismStateRule[] mechanisms;
    }
    [Serializable] internal sealed class RubenEffectArtV45 {public SpriteLibraryData[] graphics;}

    public sealed partial class DemoScene {
        RubenAttackDocumentV45 rubenAttacksV45;
        RubenEffectArtV45 rubenEffectArtV45;

        void ConfigureRubenAttacksV45(ref NpcDefinition[] profiles, ref NpcEffectDocument effects,
                                     ref NpcMechanismDocument mechanisms) {
            if (Dungeon.id!="tree" && Dungeon.id!="ruins" && Dungeon.id!="swamp") return;
            if (rubenAttacksV45==null) {
                var data=Resources.Load<TextAsset>("Pixel/monster-fidelity-v45/ruben-attacks");
                if (!data) throw new InvalidOperationException("Missing Ruben NPC combat data");
                rubenAttacksV45=JsonUtility.FromJson<RubenAttackDocumentV45>(data.text);
                BindRubenEffectArtV45("DamageEffect_BENDERS_NORMAL_SPECIAL_ATTACK_A_EFFECTSET","rubenfx-benders-stones");
                BindRubenEffectArtV45("DamageEffect_PPORU_WILLIAM_CUTE_THIEF_ATTACK_A","rubenfx-william-stone");
            }
            var supplemental=rubenAttacksV45;
            profiles=JoinRubenRowsV45(profiles,supplemental.profiles);
            var localEffects=new NpcEffectDocument {
                definitions=JoinRubenRowsV45(effects.definitions,supplemental.effects),
                bindings=JoinRubenRowsV45(effects.bindings,supplemental.bindings),
                reactionBindings=effects.reactionBindings,
                runtimeAdmitted=effects.runtimeAdmitted, sourceMechanismsComplete=effects.sourceMechanismsComplete
            };
            // The menu's shared document also services other routes. Keep their
            // previous rooms and cooldown tables independent of this instance.
            var localMechanisms=JsonUtility.FromJson<NpcMechanismDocument>(JsonUtility.ToJson(mechanisms));
            localMechanisms.states=JoinRubenRowsV45(localMechanisms.states,supplemental.mechanisms);
            effects=localEffects; mechanisms=localMechanisms;
        }
        void BindRubenEffectArtV45(string effectId,string graphic){
            var effect=Array.Find(rubenAttacksV45.effects,e=>e.id==effectId);
            if(effect==null||!effect.visualFromMesh)throw new InvalidOperationException("Missing original Ruben effect mesh "+effectId);
            effect.visualId=graphic;
        }
        void LoadRubenEffectArtV45(string id){
            if((id!="rubenfx-benders-stones"&&id!="rubenfx-william-stone")||libraries.ContainsKey(id))return;
            if(rubenEffectArtV45==null){
                var data=Resources.Load<TextAsset>("Pixel/monster-fidelity-v45/ruben-effect-art");
                if(!data)throw new InvalidOperationException("Missing Ruben projectile art");
                rubenEffectArtV45=JsonUtility.FromJson<RubenEffectArtV45>(data.text);
            }
            var library=Array.Find(rubenEffectArtV45.graphics,a=>a.id==id);
            if(library==null)throw new InvalidOperationException("Missing Ruben projectile "+id);
            MakeLibrary(library);
        }
        string RubenSourceCarrierV45(Enemy actor,int index,bool visible,string current,ref float clock) {
            if (!visible || actor.kind=="chicken" || current!=actor.clip || actor.state!=CombatState.Normal ||
                !actor.active || actor.dormant || !actor.aiEnabled || actor.hp<=0 || !Generic7OriginalBirthV28(actor,index)) return current;
            var state=game.RubenActionForDisplayV45(actor);
            if (state==null || !libraries.TryGetValue(actor.kind,out var bank)) return current;
            string alias;
            if (current=="idle" || current=="walk") alias=Generic7NormalNativeRulesV28.SourceAlias(actor.kind,actor.sourceNPC,current);
            else {
                var spec=Attack28NativeDisplayRulesV28.Generic(actor.kind,actor.sourceNPC,!state.id.EndsWith("_READY",StringComparison.Ordinal));
                if (spec==null || spec.sourceState!=state.id || spec.animation!=state.sourceAnimation ||
                    !StartedAttack28MetadataReadyV28(actor,bank,spec)) return current;
                alias=spec.alias;
            }
            if (alias==null || !bank.clips.TryGetValue(alias,out var clip) || clip.loop!=state.loop || clip.fps!=24) return current;
            clock=actor.npcTime*state.sourceAnimSpeed;
            return alias;
        }
        static T[] JoinRubenRowsV45<T>(T[] original,T[] extra) {
            var all=new T[original.Length+extra.Length];
            Array.Copy(original,all,original.Length);
            Array.Copy(extra,0,all,original.Length,extra.Length);
            return all;
        }
    }

    public sealed partial class GameModel {
        internal bool RubenTreeMotionClockV45(Enemy actor,string clip,out float clock) {
            clock=0;
            if(actor.sourceNPC!="NUI_TREE_KNIGHT" || actor.state!=CombatState.Normal)return false;
            var state=NpcState(actor);
            if(state==null || state.clip!=clip || (clip!="idle" && clip!="walk" && clip!="attack"))return false;
            clock=actor.npcTime*state.sourceAnimSpeed;return true;
        }
        internal NpcStateDefinition RubenActionForDisplayV45(Enemy actor) {
            if(actor.state!=CombatState.Normal)return null;
            switch(actor.sourceNPC){
                case "NUI_BENDERS_NORMAL":case "NUI_BENDERS_THIEF":case "NUI_THIEF_THIN_ZERO":
                case "NUI_PPORU_WILLIAM_NORMAL":case "NUI_PPORU_WILLIAM_CUTE":case "NUI_PPORU_WILLIAM_CUTE_THIEF":
                    return NpcState(actor);
                default:return null;
            }
        }
        internal bool RubenWilliamAttackMotionV45(Enemy actor,out string motion,out float clock) {
            motion=null;clock=0;
            if(actor.sourceNPC!="NUI_PPORU_WILLIAM_NORMAL" || actor.state!=CombatState.Normal) return false;
            var state=NpcState(actor);
            if(state==null || !state.id.Contains("_ATTACK")) return false;
            motion=state.sourceAnimation;clock=actor.npcTime*state.sourceAnimSpeed;
            if(state.id=="WILLIAM_PPORU_ATTACK_B")clock%=.6f;
            return true;
        }
    }
}
