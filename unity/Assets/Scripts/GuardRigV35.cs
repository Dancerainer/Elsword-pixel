using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public static class GuardRigV35 {
        const string Root="Pixel/feedback-v35/nasod-guard/";
        public const string OriginalMotionSha="ea7dc1e0fc86f886be64783b67b9ff68f41efc9159418d4dbbd04e997a3facb5";
        internal const int PartCount=15;
        const float Height=75.17f;
        static readonly string[] Parts={"far-thigh", "far-shin", "left-upper-arm", "left-forearm-grip", "torso", "waist", "near-thigh", "near-shin", "head", "right-upper-arm", "right-forearm-grip", "blade", "shield", "far-boot", "near-boot"};
        static readonly Dictionary<string,string> States=new Dictionary<string,string>(StringComparer.Ordinal){
            {"NASOD_GUARD_START","WaitStart"},
            {"NASOD_GUARD_WAIT_START","WaitStart"},
            {"NASOD_GUARD_WAIT","Wait"},
            {"NASOD_GUARD_WALK","Walk"},
            {"NASOD_GUARD_ATTACK_A_READY","AttackA_Ready"},
            {"NASOD_GUARD_ATTACK_A","AttackA_Fire"},
            {"NASOD_GUARD_ATTACK_B_READY","AttackB_Ready"},
            {"NASOD_GUARD_ATTACK_B","AttackB_Fire"},
            {"NASOD_GUARD_JUMP_UP","JumpUp"},
            {"NASOD_GUARD_JUMP_DOWN","JumpDown"},
            {"NASOD_GUARD_JUMP_UP_DIR","JumpUp"},
            {"NASOD_GUARD_JUMP_DOWN_DIR","JumpDown"},
            {"NASOD_GUARD_JUMP_LANDING","JumpLanding"},
            {"NASOD_GUARD_DAMAGE_BIG","DamageBack"},
            {"NASOD_GUARD_DAMAGE_GUARD","WaitHabit"},
            {"NASOD_GUARD_DAMAGE_DOWN_BACK","DamageDownBack"},
            {"NASOD_GUARD_DAMAGE_FLY_BACK","DamageAirFlyBack"},
            {"NASOD_GUARD_DAMAGE_AIR","DamageAirUp"},
            {"NASOD_GUARD_DAMAGE_AIR_DOWN","DamageAirDown"},
            {"NASOD_GUARD_DAMAGE_AIR_UP","DamageAirUp"},
            {"NASOD_GUARD_DAMAGE_AIR_FALL","DamageAirFall"},
            {"NASOD_GUARD_DAMAGE_AIR_DOWN_LANDING","DamageAirDownLanding"},
            {"NASOD_GUARD_STAND_UP_FRONT","DamageStandUpFront"},
            {"NASOD_GUARD_STAND_UP_BACK","DamageStandUpBack"},
            {"NASOD_GUARD_DYING_LAND_FRONT","DamageDownBack"},
            {"NASOD_GUARD_DYING_LAND_BACK","DamageDownBack"},
            {"NASOD_GUARD_DYING_SKY","DamageAirDownLanding"},
        };
        static readonly Dictionary<string,float> Durations=new Dictionary<string,float>(StringComparer.Ordinal){
            {"Wait",1.000000000f},
            {"AttackA_Fire",2.466666667f},
            {"AttackA_Ready",1.800000000f},
            {"AttackB_Fire",2.500000000f},
            {"AttackB_Ready",1.800000000f},
            {"DamageAirDown",0.400000000f},
            {"DamageAirDownLanding",1.466666667f},
            {"DamageAirFall",1.000000000f},
            {"DamageAirFlyBack",1.000000000f},
            {"DamageAirUp",1.000000000f},
            {"DamageBack",1.166666667f},
            {"DamageDownBack",1.600000000f},
            {"DamageStandUpBack",1.333333333f},
            {"DamageStandUpFront",1.900000000f},
            {"JumpDown",1.000000000f},
            {"JumpLanding",1.433333333f},
            {"JumpUp",1.000000000f},
            {"WaitHabit",3.100000000f},
            {"WaitStart",3.166666667f},
            {"Walk",1.200000000f},
        };
        public static FairyPoseLibraryV33 Load(){
            return RegisteredPartRigV35.Load(Root,"NUI_NASOD_GUARD",GuardPagesPinV35.Sha,Validate);
        }
        static void Validate(FairyPoseLibraryV33 lib) {
            if(lib.motionSha256!=OriginalMotionSha||lib.authoredFacing!=1||lib.nativeYaw!=0||lib.parts.Length!=PartCount||
                lib.tracks.Length!=20||Math.Abs(lib.UnitsPerPixel*lib.standingPixels-Height)>.001f)
                throw new InvalidOperationException("Guard original source/registered proportions");
            for(int i=0;i<Parts.Length;i++)if(lib.parts[i].name!=Parts[i])throw new InvalidOperationException("Guard source part "+i);
            var unique=new HashSet<string>();
            foreach(var track in lib.tracks){
                if(track.animation!=track.alias||!Durations.TryGetValue(track.animation,out var duration)||!unique.Add(track.alias)||
                    Math.Abs(track.duration-duration)>.00001f||track.fps!=(track.animation=="Walk"?120:60)||track.components!=5||
                    track.count!=Mathf.CeilToInt(duration*track.fps-.0001f)+1||track.loop||track.values.Length!=track.count*Parts.Length*5)
                    throw new InvalidOperationException("Guard original motion/samples "+track.animation);
            }
        }
        public static bool TryFrame(FairyPoseLibraryV33 lib,SourceWholeSelectionV21 selection,Enemy actor,SpriteRenderer carrier,out FairyAuthoredPartsV33.Frame frame){
            frame=default;
            if(selection==null||actor==null||actor.kind!="nasod-guard"||actor.sourceNPC!="NUI_NASOD_GUARD"||!carrier||!carrier.sprite||
                actor.visualScale!=1||(actor.facing!=1&&actor.facing!=-1))return false;
            var state=selection.State;
            if(!States.TryGetValue(state.State,out var motion)||motion!=state.Motion||lib.Find(motion)==null||
                Math.Abs(Durations[motion]-state.MotionDuration)>.00001f||float.IsNaN(selection.SourceMotionClock)||
                float.IsInfinity(selection.SourceMotionClock)||selection.SourceMotionClock<0)return false;
            // SourcePhaseWholeLookupV21 has already mapped any state loop and
            // held endpoint into the original motion clock. Do not modulo twice.
            frame=new FairyAuthoredPartsV33.Frame{alias=motion,sourceClock=selection.SourceMotionClock,facing=actor.facing,
                visible=selection.ApplySourceShow(actor.active&&!actor.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy)};
            return true;
        }
    }
}
