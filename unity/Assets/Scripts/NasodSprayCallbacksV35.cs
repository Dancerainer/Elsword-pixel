using System;
using System.Security.Cryptography;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class GameModel {
        internal const string SprayShot="DamageEffect_NASOD_SPRAY_ATTACK_B",IceSprayShot="DamageEffect_NASOD_SPRAY_ICE_ATTACK_B";
        const string SprayExplosion="DamageEffect_NASOD_SPRAY_ATTACK_B_EXPLOSION";
        internal int spraySourceShotsV35;
        internal float lastSprayBornAtV35,lastSprayLandV35;
        internal string lastSprayShotV35;
        internal static NpcEffectDocument WithCoreSprayEffectsV35(NpcEffectDocument core,byte[] raw){
            if(core==null||raw==null)throw new ArgumentException("Original Core/Spray effects");
            using(var h=SHA256.Create())if(BitConverter.ToString(h.ComputeHash(raw)).Replace("-","").ToLowerInvariant()!=
                "cedd60326d64ee1e7e325024fa68c40c6cf494c70d57fb7e1933c78c2ab9f06e")
                throw new InvalidOperationException("Source ice-spray projectile and its original explosion");
            var exact=FactoryTypedJsonR581.ReadMine2ProductRootR1226<NpcEffectDocument>(raw);
            if(exact.definitions.Length!=2 || Array.Find(exact.definitions,x=>x.id==SprayExplosion)==null)
                throw new InvalidOperationException("Two original ice-spray effect definitions");
            var head=Array.Find(exact.definitions,x=>x.id==IceSprayShot);
            if(head==null || head.deathChildren.Length!=1||head.deathChildren[0].effect!=SprayExplosion)
                throw new InvalidOperationException("Original ice projectile DIE_DAMAGE_EFFECT0");
            foreach(var fx in exact.definitions)if(Array.Exists(core.definitions,x=>x.id==fx.id))
                throw new InvalidOperationException("Duplicate Core spray definition "+fx.id);
            var combined=new NpcEffectDefinition[core.definitions.Length+2];core.definitions.CopyTo(combined,0);
            exact.definitions.CopyTo(combined,core.definitions.Length);
            return new NpcEffectDocument{runtimeAdmitted=core.runtimeAdmitted,sourceMechanismsComplete=core.sourceMechanismsComplete,
                definitions=combined,bindings=core.bindings,reactionBindings=core.reactionBindings};
        }
        bool OriginalNasodSprayV35(Enemy e){
            if(e==null||e.hp<=0||!e.active||e.dormant||e.encounterIndex!=EncounterIndex||!enemies.Contains(e))return false;
            if(e.sourceNPC==NasodSprayRigV35.Plain)
                return e.kind=="nasod-spray"&&PlainOriginalSourceBoundR1258&&
                    (plainRouteR1258==PlainNormalR1258||plainRouteR1258==PlainRecycleR1258)&&CurrentRoom.id==plainRouteR1258+"-"+room;
            return e.sourceNPC==NasodSprayRigV35.Ice&&e.kind=="nasod-spray-ice"&&coreHardV21!=null&&room==2&&CurrentRoom.id=="Dungeon_Altera_Core_Hard-2";
        }
        bool TryNasodSprayFarAttackV35(Enemy e,NpcStateDefinition state,float distance){
            if(e.sourceNPC!=NasodSprayRigV35.Plain&&e.sourceNPC!=NasodSprayRigV35.Ice)return false;
            if(!OriginalNasodSprayV35(e)||distance<=500*.28f)return false;
            string p=e.sourceNPC.Substring(4);
            if(state.id!=p+"_WAIT"&&state.id!=p+"_WALK"||NpcRoll()>=50)return false;
            // Both source condition tables order the near A before far B, with the
            // same exact 500 source-unit boundary, original decision interval, and RATE=50.
            var ready=npcStates[e.sourceNPC+"/"+p+"_ATTACK_B_READY"];
            if(!TryEnterNpcState(e,ready))return false;e.npcDecisionElapsed=0;return true;
        }
        void AdvanceNasodSprayCallbackV35(Enemy e,NpcStateDefinition state,float from,float to,float packetStart){
            const float at=.01f;
            if((e.sourceNPC!=NasodSprayRigV35.Plain&&e.sourceNPC!=NasodSprayRigV35.Ice)||from>=at||to<at||
                state.id!=e.sourceNPC.Substring(4)+"_ATTACK_B"||!OriginalNasodSprayV35(e))return;
            string id=e.sourceNPC==NasodSprayRigV35.Ice?IceSprayShot:SprayShot;
            if(!npcEffectDefinitions.TryGetValue(id,out var definition))throw new InvalidOperationException("Unbound original Nasod Spray effect "+id);
            // Both original 27-instruction callbacks pass GetBonePos(Bone_Rhand)
            // to CreateInstance, followed by GetLandPosition().y as a separate argument.
            // Values are the actual two KSM samples at AttackB_Fire 0.01.
            var emission=new NpcEffectEmission{effect=id,offsetX=60.7206671f,offsetY=e.sourceNPC==NasodSprayRigV35.Ice?43.9507020f:43.9507023f,
                offsetZ=-9.1621388f,sourceLandPositionYArgument=true};
            var shot=SpawnNpcEffect(e,definition,emission,packetStart+at-from);
            if(!shot.hasSourceLandY||shot.owner!=e)throw new InvalidOperationException("Original Nasod Spray land and owner");
            spraySourceShotsV35++;lastSprayBornAtV35=shot.bornAt;lastSprayLandV35=shot.sourceLandY;lastSprayShotV35=shot.id;
        }
    }
    public sealed partial class DemoScene {
        NpcEffectDocument CoreSprayEffectDocumentV35(NpcEffectDocument original){
            var exact=Resources.Load<TextAsset>(NasodSprayRigV35.Root(NasodSprayRigV35.Ice)+"source-attack-b");
            if(!exact)throw new InvalidOperationException("Original Core ice spray projectile registration");
            return GameModel.WithCoreSprayEffectsV35(original,exact.bytes);
        }
    }
}
