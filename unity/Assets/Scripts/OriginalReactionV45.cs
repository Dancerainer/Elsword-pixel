using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Slot order is shared with tools/art/monster-reactions-v45/originals.py.
    internal enum OriginalHurtSlotV45 {
        SmallFront, SmallBack, BigFront, BigBack, SmallAir, BigAir, FlyFront,
        FlyBack, DownFront, DownBack, Up, DownAir, Fall, StandFront, StandBack,
        DeadFront, DeadBack, DeadAir
    }

    [Serializable] internal sealed class OriginalHurtStateV45 { public string state, motion, rendered; public float speed; public string[] aliases; }
    [Serializable] internal sealed class OriginalHurtProfileV45 {
        public string npc, kind;
        public OriginalHurtStateV45[] roles;
    }
    [Serializable] internal sealed class OriginalHurtDocumentV45 { public OriginalHurtProfileV45[] profiles; }

    internal static class OriginalReactionV45 {
        static Dictionary<string, OriginalHurtProfileV45> profiles;

        static void Load(){
            var data=Resources.Load<TextAsset>("Pixel/feedback-v45/reactions");
            if(!data)throw new InvalidOperationException("Missing original NPC damage registration");
            var doc=JsonUtility.FromJson<OriginalHurtDocumentV45>(data.text);
            var rows=new Dictionary<string,OriginalHurtProfileV45>(StringComparer.Ordinal);
            foreach(var row in doc.profiles){
                if(row.roles==null||row.roles.Length!=(int)OriginalHurtSlotV45.DeadAir+1)
                    throw new InvalidOperationException("Invalid source damage roles for "+row.npc);
                rows.Add(row.npc,row);
            }
            profiles=rows;
        }

        internal static bool TrySlot(Enemy e,out OriginalHurtSlotV45 slot) {
            slot=default;
            if(e==null||!e.hasOriginalHitV45||!e.active||e.dormant||!string.IsNullOrEmpty(e.npcState))return false;
            bool back=e.originalHitBackV45, big=e.originalHitCodeV45==2;
            switch(e.state){
                case CombatState.Hurt:
                    slot=e.originalHitAirV45?(big?OriginalHurtSlotV45.BigAir:OriginalHurtSlotV45.SmallAir):
                        big?(back?OriginalHurtSlotV45.BigBack:OriginalHurtSlotV45.BigFront):
                            (back?OriginalHurtSlotV45.SmallBack:OriginalHurtSlotV45.SmallFront);
                    break;
                case CombatState.Airborne:
                    if(e.clip!="launched"&&(!e.knockdownOnLanding||e.clip!="fall"))return false;
                    if(e.clip=="fall")slot=OriginalHurtSlotV45.Fall;
                    else if(e.originalHitCodeV45==4)slot=OriginalHurtSlotV45.Up;
                    else if(e.originalHitCodeV45==3||e.originalHitWasDownV45)slot=OriginalHurtSlotV45.DownAir;
                    else if(e.originalHitCodeV45==5||back)slot=back?OriginalHurtSlotV45.FlyBack:OriginalHurtSlotV45.FlyFront;
                    else return false; // An untyped public launch retains this actor's registered default.
                    break;
                case CombatState.Knockdown:slot=back?OriginalHurtSlotV45.DownBack:OriginalHurtSlotV45.DownFront;break;
                case CombatState.GettingUp:slot=back?OriginalHurtSlotV45.StandBack:OriginalHurtSlotV45.StandFront;break;
                case CombatState.Dead:
                    slot=e.originalHitAirV45?OriginalHurtSlotV45.DeadAir:(back?OriginalHurtSlotV45.DeadBack:OriginalHurtSlotV45.DeadFront);
                    break;
                default:return false;
            }
            return true;
        }

        internal static bool TryState(Enemy e,out OriginalHurtStateV45 pose) {
            pose=null;
            if(!TrySlot(e,out var slot))return false;
            if(profiles==null)Load();
            if(!profiles.TryGetValue(e.sourceNPC??"",out var profile)||profile.kind!=e.kind)return false;
            pose=profile.roles[(int)slot];
            return pose!=null&&!string.IsNullOrEmpty(pose.state)&&!string.IsNullOrEmpty(pose.motion);
        }
        internal static bool TryPhysicalOrCommittedV45(Enemy e,out OriginalHurtStateV45 pose,out float seconds){
            seconds=0;
            if(TryState(e,out pose)){seconds=e.clock*pose.speed;return true;}
            if(e==null||!e.hasOriginalHitV45||e.state==CombatState.Normal||string.IsNullOrEmpty(e.npcState))return false;
            if(profiles==null)Load();
            if(!profiles.TryGetValue(e.sourceNPC??"",out var p)||p.kind!=e.kind)return false;
            foreach(var row in p.roles)if(row.state==e.npcState&&!string.IsNullOrEmpty(row.rendered)){
                pose=row;seconds=e.npcTime*row.speed;return true;
            }
            return false;
        }
    }

    public sealed partial class GameModel {
        internal bool OriginalHurtDefinitionV45(Enemy actor,OriginalHurtStateV45 pose,out NpcStateDefinition state){
            return npcStates.TryGetValue(actor.sourceNPC+"/"+pose.state,out state)&&state.sourceAnimation==pose.motion;
        }
    }
    public sealed partial class DemoScene {
        bool TryOriginalWholeReactionV45(Enemy actor,out string state,out float clock){
            state=null;clock=0;
            if(!OriginalReactionV45.TryState(actor,out var pose)||!game.OriginalHurtDefinitionV45(actor,pose,out var definition))return false;
            state=definition.id;clock=ClampOriginalClockV45(actor.clock,definition.duration);return true;
        }
        static float ClampOriginalClockV45(float clock,float span){return Math.Max(0,Math.Min(clock,span));}
        void SelectOriginalHurtClipV45(Enemy actor,ref string alias,ref float clock){
            if(!OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out var pose,out float seconds)||!libraries.TryGetValue(actor.kind,out var bank))return;
            if(TryOriginalReactionInkV45(actor,pose,ref alias,ref clock)){clock=seconds;return;}
            if(Array.IndexOf(pose.aliases,alias)>=0)return;
            if(game.OriginalHurtDefinitionV45(actor,pose,out var state)&&!string.IsNullOrEmpty(state.clip)&&bank.frames.ContainsKey(state.clip)){
                alias=state.clip;clock=ClampOriginalClockV45(seconds/pose.speed,state.duration);
            }else{
                foreach(string registered in pose.aliases)if(bank.frames.ContainsKey(registered)){
                    alias=registered;clock=seconds/pose.speed;break;
                }
            }
        }
    }
}
