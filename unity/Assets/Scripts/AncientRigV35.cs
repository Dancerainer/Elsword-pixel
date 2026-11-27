using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public static class AncientRigV35 {
        public const string Kind="ancient-phoru",Npc="NUI_ANCIENT_PPORU";
        public const string ResourceRoot="Pixel/feedback-v35/ancient-phoru/";
        public const string PagesSha=AncientPartsPinV35.PagesSha;
        public const string OriginalMotionSha="383d79e843ce8c551ee707eacf32232282dd0d69cc5122be9d0e13e9eef14df0";
        public const float Height=208.00194008f;
        static readonly string[] Ai={"WAIT","WALK","ATTACK_A_READY","ATTACK_A","ATTACK_B_READY","ATTACK_B","SPECIAL_ATTACK_READY","SPECIAL_ATTACK"};
        public static FairyPoseLibraryV33 Load() { return RegisteredPartRigV35.Load(ResourceRoot,Npc,PagesSha,Validate); }
        static void Validate(FairyPoseLibraryV33 lib) {
            if (lib.parts.Length!=17 || lib.tracks.Length!=11 || lib.motionSha256!=OriginalMotionSha || lib.authoredFacing!=1 ||
                lib.nativeYaw!=180 || Math.Abs(lib.heightRatio*lib.heroHeight-Height)>.0001f)
                throw new InvalidOperationException("Ancient original registration or source motion changed");
            var tracks=new Dictionary<string,FairyPoseTrackV33>();
            foreach(var t in lib.tracks) {
                if(t.components!=5 || Math.Abs(t.fps-24)>.0001f || t.values.Length!=t.count*lib.parts.Length*t.components)
                    throw new InvalidOperationException("Ancient original bone track "+t.alias);
                tracks.Add(t.alias,t);
            }
            foreach(string suffix in Ai) {
                var spec=Remaining3AttackSourceRulesV28.Find(Kind,Npc,"ANCIENT_PPORU_"+suffix);
                if(spec==null)throw new InvalidOperationException("Ancient original state "+suffix);
                Check(tracks,spec.alias,spec.animation,spec.sourceSpan,spec.samples,spec.loop);
            }
            var hurt=Remaining3HurtNativeRulesV28.Find(Kind,Npc);
            var air=Remaining4AirNativeRulesV28.Find(Kind,Npc);
            var death=Remaining3GroundNativeRulesV28.Find(Kind,Npc,(int)CombatState.Dead);
            Check(tracks,Remaining3HurtNativeRulesV28.Alias,hurt.animation,hurt.duration,hurt.samples,false);
            Check(tracks,air.alias,air.animation,air.duration,air.samples,false);
            Check(tracks,death.alias,death.animation,death.duration,death.samples,false);
        }
        static void Check(Dictionary<string,FairyPoseTrackV33> tracks,string alias,string motion,float span,int count,bool loop) {
            if(!tracks.TryGetValue(alias,out var t) || t.animation!=motion || Math.Abs(t.duration-span)>.00001f || t.count!=count || t.loop!=loop)
                throw new InvalidOperationException("Ancient carrier clock binding "+alias);
        }
    }
}
