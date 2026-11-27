using System;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GameModel {
        internal const string EliteSheriffV35 = "NUI_NASOD_ELITE_SHERIFF";
        const string SheriffPrefixV35 = "NASOD_ELITE_SHERIFF_";
        const string SheriffBulletV35 = "DamageEffect_NASOD_SHERIFF_ATTACK_A";
        const string SheriffMuzzleV35 = "AirShip_Soldier_Gunner_Weapon";

        static NpcEffectDocument ReadSheriffSourceV35(byte[] bytes,string sha256) {
            using(var hash=SHA256.Create())
                if(bytes==null||BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","").ToLowerInvariant()!=sha256)
                    throw new InvalidOperationException("Original sheriff effect or weapon track changed.");
            return FactoryTypedJsonR581.ReadMine2ProductRootR1226<NpcEffectDocument>(bytes);
        }

        internal static NpcEffectDocument WithCoreSheriffEffectV35(NpcEffectDocument core,byte[] weapon,byte[] bullet) {
            var track=ReadSheriffSourceV35(weapon,"03b48d70f2252ae4c9dd0530c61c72cc618e98223b89661df0aa176eeaa461fb");
            var source=ReadSheriffSourceV35(bullet,"1ffe8e522f3751d1e78517c21331dbd6c40e8ee42b19c0c8cd0c0bb6fa9a5535");
            var bind=track.bindings[0];var laser=source.definitions[0];
            if(track.bindings.Length!=1||bind.npc!=EliteSheriffV35||bind.state!=SheriffPrefixV35+"ATTACK_A"||
                bind.unitScale!=1||bind.tracks.Length!=1||bind.tracks[0].bone!=SheriffMuzzleV35||bind.tracks[0].samples.Length!=191||
                bind.sets.Length!=3||laser.id!=SheriffBulletV35||laser.lifetime!=.4f||laser.magic!=.17f||laser.push!=140||
                laser.geometrySamples.Length!=25||laser.geometrySamples[0].attackLines.Length!=6)
                throw new InvalidOperationException("Original Core elite sheriff line projectile and weapon.");
            var timers=new[]{.55f,1.23f,1.84f};
            for(int i=0;i<timers.Length;i++){
                var set=bind.sets[i];var shot=set.emissions[0];
                if(set.attachAt!=timers[i]||set.emissions.Length!=1||shot.start!=0||shot.bone!=SheriffMuzzleV35||
                    shot.effect!=SheriffBulletV35||!shot.sourceLandPositionYArgument||shot.follow)
                    throw new InvalidOperationException("Original elite sheriff fire callback.");
            }
            if(core==null||Array.Exists(core.definitions,d=>d.id==laser.id))
                throw new InvalidOperationException("Core elite sheriff effect registration.");
            var bindings=(NpcEffectBinding[])core.bindings.Clone();
            int index=Array.FindIndex(bindings,b=>b.npc==bind.npc&&b.state==bind.state);
            if(index<0||Array.FindLastIndex(bindings,b=>b.npc==bind.npc&&b.state==bind.state)!=index||
                bindings[index].tracks.Length!=0||bindings[index].sets.Length!=0)
                throw new InvalidOperationException("Original Core sheriff AttackA binding.");
            bindings[index]=bind;
            var effects=new NpcEffectDefinition[core.definitions.Length+1];core.definitions.CopyTo(effects,0);
            effects[effects.Length-1]=laser;
            return new NpcEffectDocument{runtimeAdmitted=core.runtimeAdmitted,sourceMechanismsComplete=core.sourceMechanismsComplete,
                definitions=effects,bindings=bindings,reactionBindings=core.reactionBindings};
        }

        bool CoreEliteSheriffBirthV35(Enemy e) {
            return coreHardV21!=null&&(room==0||room==2)&&e!=null&&e.sourceNPC==EliteSheriffV35&&e.kind=="nasod-elite-sheriff"&&
                e.active&&!e.dormant&&e.hp>0&&e.sourceKeyCode==0&&e.encounterIndex==EncounterIndex&&
                CurrentRoom.id=="Dungeon_Altera_Core_Hard-"+room&&enemies.Contains(e);
        }

        string CoreSheriffDownEntryV35(Fighter f,bool playerActor,float? attackerX) {
            var e=f as Enemy;if(playerActor||!CoreEliteSheriffBirthV35(e))return null;
            if(e.state==CombatState.Airborne&&e.knockdownOnLanding)return SheriffPrefixV35+"DAMAGE_AIR_DOWN_LANDING";
            bool rear=attackerX.HasValue&&(attackerX.Value-e.x)*e.facing<0;
            return SheriffPrefixV35+(rear?"DAMAGE_DOWN_BACK":"DAMAGE_DOWN_FRONT");
        }

        void BeginCoreSheriffDownV35(Enemy e,string id) {
            var state=npcStates[EliteSheriffV35+"/"+id];
            if(state.loop||!state.superArmor||state.sourceAnimSpeed!=1||state.duration<=0)
                throw new InvalidOperationException("Original elite sheriff one-play damage state.");
            EnterNpcState(e,state);e.downTime=state.duration;
        }

        bool CoreSheriffPhysicalDownV35(Enemy e) {
            if(!CoreEliteSheriffBirthV35(e)||e.state!=CombatState.Knockdown)return false;
            return e.npcState==SheriffPrefixV35+"DAMAGE_DOWN_FRONT"||e.npcState==SheriffPrefixV35+"DAMAGE_DOWN_BACK"||
                e.npcState==SheriffPrefixV35+"DAMAGE_AIR_DOWN_LANDING";
        }

        bool CoreSheriffSelectedGetUpV35(Enemy e) {
            return CoreEliteSheriffBirthV35(e)&&(e.npcState==SheriffPrefixV35+"ROLLING_STAND_UP_FRONT"||
                e.npcState==SheriffPrefixV35+"STAND_UP_BACK");
        }

        bool TickCoreSheriffDownV35(Enemy e,float packet) {
            if(!CoreSheriffPhysicalDownV35(e))return false;
            if(!Grounded(e)){
                e.state=CombatState.Airborne;e.knockdownOnLanding=true;e.downTime=0;
                EnterNpcState(e,npcStates[EliteSheriffV35+"/"+SheriffPrefixV35+"DAMAGE_AIR_FALL"]);return true;
            }
            float frozen=Math.Min(packet,e.hitPause);e.hitPause-=frozen;packet-=frozen;
            if(packet<=.000001f)return true;
            var state=NpcState(e);float to=Math.Min(state.duration,e.npcTime+packet);
            AdvanceNpcStateSounds(e,state,e.npcTime,to,elapsed-packet);
            e.clock=e.npcTime=to;e.downTime=state.duration-to;
            if(to<state.duration)return true;
            // Only DAMAGE_DOWN_FRONT declares a 20% roll after one play. Back
            // and air-down-landing each take the state's own motion-end edge.
            string target=state.nextState;
            if(state.id==SheriffPrefixV35+"DAMAGE_DOWN_FRONT"&&NpcRoll()<20)
                target=SheriffPrefixV35+"ROLLING_STAND_UP_FRONT";
            var next=npcStates[EliteSheriffV35+"/"+target];
            e.state=CombatState.GettingUp;e.downTime=e.stun=0;e.recoveryTime=next.duration;
            EnterNpcState(e,next);return true;
        }
    }
    public sealed partial class DemoScene {
        NpcEffectDocument CoreCombatEffectDocumentV35(NpcEffectDocument source) {
            var weapon=Resources.Load<TextAsset>("Pixel/dungeons/altera-core-hard/sheriff-weapon-v35");
            var bullet=Resources.Load<TextAsset>("Pixel/dungeons/altera-nasod-factory/sheriff-source-v32");
            if(!weapon||!bullet)throw new InvalidOperationException("Registered Core elite sheriff muzzle and projectile.");
            return GameModel.WithCoreSheriffEffectV35(CoreSprayEffectDocumentV35(source),weapon.bytes,bullet.bytes);
        }
    }
}
