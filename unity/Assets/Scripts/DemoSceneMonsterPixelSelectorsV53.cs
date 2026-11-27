using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        void PrepareAncientPixelPose(){
            if(game==null||game.CurrentRoom.id!="swamp-3")return;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var actor=game.enemies[i];
                if(actor.sourceNPC!=AncientRigV35.Npc||actor.kind!=AncientRigV35.Kind||!actor.active||actor.dormant)continue;
                if(monsterPixelSelectionsV53.ContainsKey(actor))continue;
                var carrier=enemyViews[i];
                string motion;float clock=actor.clock;
                switch(actor.state){
                    case CombatState.Dead: motion="DamageDownFront";clock=actor.deathTime;break;
                    case CombatState.Hurt: motion=actor.originalHitBackV45?"DamageBack":"DamageFront";break;
                    case CombatState.Airborne: motion=actor.vy>0?"JumpUp":"DamageAirFall";break;
                    case CombatState.Knockdown: case CombatState.GettingUp:
                        if(MonsterPixelRegisteredV53(actor,out var definition)&&TryAncientPhysicalPose(actor,carrier,actor.clip,MonsterPixelLibraryV53(definition)))continue;
                        motion="DamageDownFront";break;
                    default:
                        if(lastAncientPose!=null&&lastAncientPose.actor==actor&&lastAncientPose.carrier==carrier&&lastAncientPose.sprite){
                            monsterPixelSelectionsV53[actor]=lastAncientPose;continue;
                        }
                        motion="Wait";clock=0;break;
                }
                RecordMonsterPixelSourceV53(actor,carrier,"Pose/AncientTransition",actor.clip,motion,Mathf.Max(0,clock));
            }
        }

        bool TryAncientPhysicalPose(Enemy actor,SpriteRenderer carrier,string alias,MonsterPixelFramesV53 library){
            string motion;
            float seconds;
            if(actor.state==CombatState.Knockdown||actor.state==CombatState.GettingUp){
                // The boss normally resists knockdown, but a full knockdown gauge can floor it.
                // Its art has directional falls ending prone and no separate rise animation.
                motion=actor.originalHitBackV45?"DamageDownBack":"DamageDownFront";
                if(!library.TryClip(motion,out var fall))return false;
                float phase=actor.state==CombatState.Knockdown
                    ? 1-actor.downTime/GameModel.EnemyDownDuration
                    : actor.recoveryTime/game.EnemyRecoveryDuration(actor);
                seconds=Mathf.Clamp01(phase)*fall.duration;
            }else if(actor.state==CombatState.Airborne&&actor.clip=="fall"){
                motion="DamageAirFall";seconds=actor.clock;
            }else if(actor.state==CombatState.Airborne&&actor.clip=="jump"&&actor.hasOriginalHitV45){
                motion=actor.vy>0?"JumpUp":"JumpDown";seconds=actor.clock;
            }else if(OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out var hit,out seconds)&&
                hit.motion!=hit.rendered&&library.TryClip(hit.rendered,out _)){
                // The original launcher requests "DamegeBack"; the registered raster is "DamageBack".
                motion=hit.rendered;
            }else return false;
            return RecordMonsterPixelSourceV53(actor,carrier,"Pose/AncientPhysical",alias,motion,seconds);
        }

        // These adapters follow the final existing view owner, after typed V45
        // reactions. They do not reinterpret every NPC state as an NPC clock.
        static readonly Dictionary<string,string> treePixelMotionsV53=new Dictionary<string,string>(StringComparer.Ordinal){
            {"source-start","WaitHabit"},{"source-idle","Wait"},{"source-walk","Walk"},
            {"source-hurt","DamageSmallFront"},{"source-launch","DamageAirUp"},{"source-fall","DamageAirFall"},
            {"source-down","DamageAirDown"},{"source-get-up","DamageStandUpFront"},
            {"source-jump","JumpUp"},{"source-death","DamageDownFront"}
        };
        bool TryExistingPixelSelectorV53(Enemy actor,SpriteRenderer carrier,string alias,float clock,AnimationData selected,MonsterPixelFramesV53 library){
            // Untyped public launches have no OriginalReactionV45 slot. The
            // original Chicken carrier still selects its authored AirUp track;
            // keep that exact clock on the red whole-body library as well.
            if(actor.kind=="chicken"&&actor.sourceNPC=="NUI_CHICKEN"&&alias=="source-air-up"){
                RecordPixelDisplayClockV53(actor,carrier,"Pose/ChickenAirUpV55",alias,"DamageAirUp",clock,selected,library);return true;
            }
            if(actor.kind=="william"&&game.RubenWilliamAttackMotionV45(actor,out var williamMotion,out float williamClock)){
                if(!library.TryClip(williamMotion,out var source))throw new InvalidOperationException("Missing original William attack "+williamMotion);
                RecordMonsterPixelSourceV53(actor,carrier,"Pose/RubenWilliamAttackV45",alias,williamMotion,Mathf.Clamp(williamClock,0,source.duration));return true;
            }
            if(TryGiantWilliamPixelSelectorV53(actor,carrier,alias,clock,selected,library))return true;
            if(actor.kind=="tree-knight"&&actor.sourceNPC=="NUI_TREE_KNIGHT"){
                if(alias=="source-start" && SaplingOpeningV57(actor)){
                    RecordMonsterPixelSourceV53(actor,carrier,"Pose/SaplingBuriedV57",alias,"WaitHabit",BuriedMonsterPresentationV57.SaplingClock(clock));return true;
                }
                // The legacy final owner overrides the shared source-attack
                // carrier with two independent source tracks. Ready is 1.25x.
                if(alias=="source-attack"){
                    var attack=TreeKnightHurtV45.OriginalAttackV45(actor,out float seconds);
                    if(attack!=null){RecordMonsterPixelSourceV53(actor,carrier,"Pose/TreeKnightHurtV45",alias,attack.motion,Mathf.Min(seconds,attack.duration));return true;}
                }
                if(treePixelMotionsV53.TryGetValue(alias,out var treeMotion)){
                    RecordPixelDisplayClockV53(actor,carrier,"Pose/TreeNativeV25",alias,treeMotion,clock,selected,library);return true;
                }
            }
            if(Alpha4NormalNativeRulesV28.Supported(actor.kind,actor.sourceNPC)&&Alpha4NormalNativeRulesV28.Alias(alias)){
                // Alpha4NormalNativeRulesV28.Select already validates the exact
                // live profile and applies speed once before Pose receives it.
                if(selected==null||Math.Abs(selected.duration-Alpha4NormalNativeRulesV28.Duration(actor.sourceNPC,alias))>.000002f)
                    throw new InvalidOperationException("Alpha4 final display duration changed");
                RecordPixelDisplayClockV53(actor,carrier,"Pose/Alpha4NormalV28",alias,alias=="source-idle"?"Wait":"Walk",clock,selected,library);return true;
            }
            return false;
        }

        static readonly string[] PixelRiseMotions={"JumpUp","DamageAirUp","JumpReady","JumpDown"};
        static readonly string[] PixelDropMotions={"JumpDown","JunpDown","DamageAirFall","JumpUp"};
        static readonly string[] PixelLaunchMotions={"DamageAirUp","DamageAirFlyFront","DamageAirFly","DamageAirSmall","DamageAir","JumpUp"};
        static readonly string[] PixelInjuredDropMotions={"DamageAirFall","DamageAirDown","DamageAirSmall","DamageAir","JumpDown","JunpDown"};
        static readonly string[] PixelAirHurtMotions={"DamageAirSmall","DamageAir","DamageAirFall","DamageSmall","Damage"};
        static readonly string[] PixelFrontHurtMotions={"DamageSmallFront","DamageSmall","DamageFront","DamageBig","Damage"};
        static readonly string[] PixelBackHurtMotions={"DamageSmallBack","DamageSmall","DamageBack","DamageBig","Damage"};
        static readonly string[] PixelFrontDownMotions={"DamageDownFront","DamageDown","DamageAirDownLanding","DamageDownBack"};
        static readonly string[] PixelBackDownMotions={"DamageDownBack","DamageDown","DamageAirDownLanding","DamageDownFront"};
        static readonly string[] PixelFrontRiseMotions={"DamageStandUpFront","DamageStandUp","DamageStandUpBack","DamageDownFront","DamageDown"};
        static readonly string[] PixelBackRiseMotions={"DamageStandUpBack","DamageStandUp","DamageStandUpFront","DamageDownBack","DamageDown"};

        void PreserveMonsterReactionPixels()
        {
            if(game==null)return;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var actor=game.enemies[i];
                var carrier=enemyViews[i];
                if(actor.hp<=0||!actor.active||actor.dormant||actor.isGate||actor.passive||
                   !carrier||!carrier.enabled||!carrier.gameObject.activeInHierarchy||
                   monsterPixelSourceHiddenV53.Contains(actor)||monsterPixelSelectionsV53.ContainsKey(actor))continue;

                string[] motions;
                switch(actor.state){
                    case CombatState.Airborne:
                        bool launched=actor.clip=="launched"||actor.knockdownOnLanding;
                        motions=actor.vy>0?(launched?PixelLaunchMotions:PixelRiseMotions):
                            (launched?PixelInjuredDropMotions:PixelDropMotions);
                        break;
                    case CombatState.Hurt:
                        motions=!game.Grounded(actor)?PixelAirHurtMotions:
                            actor.originalHitBackV45?PixelBackHurtMotions:PixelFrontHurtMotions;
                        break;
                    case CombatState.Knockdown:
                        motions=actor.originalHitBackV45?PixelBackDownMotions:PixelFrontDownMotions;
                        break;
                    case CombatState.GettingUp:
                        motions=actor.originalHitBackV45?PixelBackRiseMotions:PixelFrontRiseMotions;
                        break;
                    default:
                        continue;
                }
                if(!MonsterPixelRegisteredV53(actor,out var definition)||!string.IsNullOrEmpty(definition.assembly))continue;
                try{
                    var library=MonsterPixelLibraryV53(definition);
                    if(OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out var hurt,out float clock)){
                        string source=library.TryClip(hurt.motion,out _)?hurt.motion:hurt.rendered;
                        if(library.TryClip(source,out _)&&
                           RecordMonsterPixelSourceV53(actor,carrier,"Pose/OriginalReactionV45",actor.clip,source,clock))continue;
                    }
                    bool selected=false;
                    foreach(string motion in motions){
                        if(!library.TryClip(motion,out var clip))continue;
                        float phase=actor.clock;
                        if(actor.state==CombatState.Knockdown)phase=clip.duration;
                        else if(actor.state==CombatState.GettingUp)
                            phase=clip.duration*Mathf.Clamp01(1-actor.recoveryTime/game.EnemyRecoveryDuration(actor));
                        selected=RecordMonsterPixelSourceV53(actor,carrier,"Pose/PhysicalReaction",actor.clip,motion,
                            Mathf.Clamp(phase,0,clip.duration));
                        break;
                    }
                    // Some stationary or flying species have no damage raster of
                    // their own. They keep their current skin through that pose.
                    if(!selected)RecordMonsterPixelSourceV53(actor,carrier,"Pose/PhysicalReactionHold",actor.clip,definition.standingMotion,0);
                }catch(Exception error){
                    FailMonsterPixelV53(actor,"Pose/PhysicalReaction",actor.clip,"",actor.clock,error);
                }
            }
        }
        readonly HashSet<string> giantPixelClockValidatedV53=new HashSet<string>(StringComparer.Ordinal);
        static float GiantPixelSourceSecondsV53(string kind,string alias,float clock,float duration){
            // These are the exact continuous windows proven against every
            // original V39 exposure. Do not quantize the new 60 Hz source.
            if((kind=="pporu-william-b"||kind=="pporu-william-c")&&alias=="source-attack-b"){
                float phase=Mathf.Repeat(clock,duration);
                // Float storage of .6 and 1.8 must not expose the preceding
                // cycle's endpoint at an exact original period boundary.
                return duration-phase<.000002f?0:phase;
            }
            return Mathf.Clamp(clock,0,duration);
        }
        bool TryGiantWilliamPixelSelectorV53(Enemy actor,SpriteRenderer carrier,string alias,float clock,AnimationData selected,MonsterPixelFramesV53 library){
            if(!GiantWilliamBankV39.Actor(actor.kind,actor.sourceNPC)||!alias.StartsWith("source-",StringComparison.Ordinal)||alias.StartsWith("source-native-",StringComparison.Ordinal))return false;
            var bank=GiantWilliamBankForV39(actor);if(!bank.Stage(game.CurrentRoom.id))return false;
            var binding=bank.Binding(alias);if(binding==null)return false;
            var original=Array.Find(bank.Data.clips,c=>c.sourceAnimation==binding.motion);
            if(selected==null||original==null||!library.TryClip(binding.motion,out var source)||Math.Abs(original.duration-source.duration)>.0002f)
                throw new InvalidOperationException("Giant/William exact source registration changed "+actor.kind+"/"+alias);
            string key=actor.kind+"/"+alias;
            if(!giantPixelClockValidatedV53.Contains(key)){
                if(binding.count==0){
                    if(binding.indices.Length!=0||Math.Abs(selected.duration-source.duration)>.0002f)
                        throw new InvalidOperationException("Giant/William direct source clock changed "+key);
                }else{
                    if(binding.count!=selected.frames.Length||binding.indices.Length!=binding.count||binding.fps!=selected.fps||binding.loop!=selected.loop||Math.Abs(binding.duration-selected.duration)>.00001f)
                        throw new InvalidOperationException("Giant/William source exposure carrier changed "+key);
                    if(alias=="source-attack-b"&&Math.Abs(source.duration-.6f)>.00001f)
                        throw new InvalidOperationException("Giant/William exact dash period changed "+key);
                    for(int i=0;i<binding.count;i++){
                        float seconds=GiantPixelSourceSecondsV53(actor.kind,alias,i/binding.fps,source.duration);
                        int expected=seconds>=source.duration-.000001f?original.frames.Length-1:Mathf.Clamp(Mathf.FloorToInt(seconds*original.fps+.00001f),0,original.frames.Length-1);
                        if(binding.indices[i]!=expected)throw new InvalidOperationException("Giant/William original exposure window changed "+key+"/"+i);
                    }
                }
                giantPixelClockValidatedV53.Add(key);
            }
            float phase=selected.loop?Mathf.Repeat(clock,selected.duration):clock;
            RecordMonsterPixelSourceV53(actor,carrier,"Pose/GiantWilliamV39-source-window",alias,binding.motion,GiantPixelSourceSecondsV53(actor.kind,alias,phase,source.duration));return true;
        }
        void RecordPixelDisplayClockV53(Enemy actor,SpriteRenderer carrier,string provider,string alias,string motion,float clock,AnimationData selected,MonsterPixelFramesV53 library){
            if(selected==null||!library.TryClip(motion,out var source)||Math.Abs(selected.duration-source.duration)>.0002f)
                throw new InvalidOperationException("Original source display span changed "+actor.kind+"/"+alias);
            float sourceClock=selected.loop?Mathf.Repeat(clock,selected.duration):Mathf.Clamp(clock,0,selected.duration);
            RecordMonsterPixelSourceV53(actor,carrier,provider,alias,motion,sourceClock);
        }

        [Serializable] sealed class FairyPixelClockTrackV53 {public string alias,animation;public float duration;public bool loop;}
        [Serializable] sealed class FairyPixelClockBankV53 {public FairyPixelClockTrackV53[] tracks;}
        FairyPixelClockBankV53 fairyPixelClockBankV53;
        [Serializable] sealed class EmptyIdleBindingV53 {public string npc,kind,route,alias,motion;public float displayDuration,sourceDuration;public bool loop,hold;}
        [Serializable] sealed class EmptyIdleBankV53 {public string schema;public EmptyIdleBindingV53[] bindings;}
        EmptyIdleBankV53 emptyIdleBankV53;
        bool TryEmptyIdlePixelSelectorV53(Enemy actor,SpriteRenderer carrier,string alias,float clock,AnimationData selected,MonsterPixelFramesV53 library){
            if(actor.state!=CombatState.Normal||!string.IsNullOrEmpty(actor.npcState)||actor.clip!="idle"||alias!="idle")return false;
            if(emptyIdleBankV53==null){var file=Resources.Load<TextAsset>("Pixel/monster-v53/empty-state-idle");if(!file)return false;emptyIdleBankV53=JsonUtility.FromJson<EmptyIdleBankV53>(file.text);if(emptyIdleBankV53.schema!="monster-pixel-v53-explicit-empty-state-idle")throw new InvalidOperationException("Invalid empty-state visual adaptation");}
            var binding=Array.Find(emptyIdleBankV53.bindings,b=>b.npc==actor.sourceNPC&&b.kind==actor.kind&&b.route==Dungeon.id&&b.alias==alias);
            if(binding==null)return false;
            if(selected==null||selected.loop!=binding.loop||binding.hold||binding.displayDuration<=0||Math.Abs(selected.duration-binding.displayDuration)>.0002f||!library.TryClip(binding.motion,out var source)||Math.Abs(source.duration-binding.sourceDuration)>.0002f)
                throw new InvalidOperationException("Explicit empty-state display registration changed "+actor.kind);
            // The old view exposes a drawn idle here; it is not an original
            // source-clock owner. This is an explicit new visual conversion,
            // restricted to four reviewed route/identity/empty-state tuples.
            float phase=selected.loop?Mathf.Repeat(clock,selected.duration):Mathf.Clamp(clock,0,selected.duration);
            RecordMonsterPixelSourceV53(actor,carrier,"Pose/explicit-empty-state-idle-v53",alias,binding.motion,phase*binding.sourceDuration/binding.displayDuration);return true;
        }
        void PrepareFairyPixelSelectionsV53(){
            if(game==null)return;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var actor=game.enemies[i];
                if(!MonsterPixelRegisteredV53(actor,out _)||!IsFairySourceBirthV33(actor,i))continue;
                if(monsterPixelSelectionsV53.TryGetValue(actor,out var existing)&&existing.provider=="Pose/OriginalReactionV45")continue;
                try{
                    if(!TryFairySourceFrameV33(actor,enemyViews[i],out var frame))continue;
                    if(fairyPixelClockBankV53==null){
                        var text=Resources.Load<TextAsset>(FairyPoseLibraryV33.ResourceRoot+"poses");
                        string digest="";if(text)using(var hash=System.Security.Cryptography.SHA256.Create())digest=BitConverter.ToString(hash.ComputeHash(text.bytes)).Replace("-","").ToLowerInvariant();
                        if(digest!=FairyPoseLibraryV33.PoseSha)throw new InvalidOperationException("Original Guardian source registration changed");
                        fairyPixelClockBankV53=JsonUtility.FromJson<FairyPixelClockBankV53>(text.text);
                    }
                    var track=Array.Find(fairyPixelClockBankV53.tracks,t=>t.alias==frame.alias);
                    if(track==null||track.duration<=0)throw new InvalidOperationException("Missing original Guardian source alias "+frame.alias);
                    float clock=track.loop?Mathf.Repeat(frame.sourceClock,track.duration):Mathf.Clamp(frame.sourceClock,0,track.duration);
                    RecordMonsterPixelSourceV53(actor,enemyViews[i],"Fairy/V33-final-source-frame",frame.alias,track.animation,clock,frame.facing);
                }catch(Exception error){FailMonsterPixelV53(actor,"Fairy/V33-final-source-frame",actor.clip,"",actor.clock,error);}
            }
        }

        [Serializable] sealed class PixelTraversalStateV53 {public string npc,kind,state,motion,clip;public float sourceDuration,speed;public int phase;public bool loop;}
        [Serializable] sealed class PixelTraversalBankV53 {public string schema;public PixelTraversalStateV53[] states;}
        Dictionary<string,PixelTraversalStateV53> pixelTraversalStatesV53;
        bool TryTraversalPixelSelectorV53(Enemy actor,SpriteRenderer carrier,string alias,MonsterPixelFramesV53 library){
            if(string.IsNullOrEmpty(actor.npcState)||actor.knockdownOnLanding||
               (actor.state!=CombatState.Normal&&actor.state!=CombatState.Airborne))return false;
            if(pixelTraversalStatesV53==null){
                pixelTraversalStatesV53=new Dictionary<string,PixelTraversalStateV53>(StringComparer.Ordinal);
                var file=Resources.Load<TextAsset>("Pixel/monster-v53/traversal");
                if(file){var bank=JsonUtility.FromJson<PixelTraversalBankV53>(file.text);if(bank.schema!="monster-pixel-v53-exact-traversal-clocks")throw new InvalidOperationException("Invalid traversal source bindings");foreach(var row in bank.states)pixelTraversalStatesV53.Add(row.npc+"/"+row.kind+"/"+row.state,row);}
            }
            if(!pixelTraversalStatesV53.TryGetValue(actor.sourceNPC+"/"+actor.kind+"/"+actor.npcState,out var state))return false;
            // SetNpcTraversalPhase assigns these exact physical tuples and
            // resets both clocks. TickNpcTraversalFlight advances both by dt.
            // A hit-launch, source attack or unrelated state cannot enter here.
            if(actor.clip!=state.clip||actor.state!=(state.phase==3?CombatState.Normal:CombatState.Airborne)||Math.Abs(actor.clock-actor.npcTime)>.0002f)return false;
            if(state.speed!=1||!MonsterPixelFramesV53.Finite(actor.npcTime)||actor.npcTime<0||
               !library.TryClip(state.motion,out var source)||Math.Abs(source.duration-state.sourceDuration)>.0002f)
                throw new InvalidOperationException("Original traversal phase clock changed "+actor.npcState);
            float clock=state.loop?Mathf.Repeat(actor.npcTime,state.sourceDuration):Mathf.Min(actor.npcTime,state.sourceDuration);
            RecordMonsterPixelSourceV53(actor,carrier,"Pose/NpcTraversal-phase-clock",alias,state.motion,clock);return true;
        }
#if UNITY_EDITOR
        int ReviewSourceOwnerSelectorsV53(Enemy actor,SpriteRenderer carrier,MonsterPixelFramesV53 library,Action<bool,string> need){
            int cases=0;string snapshot=JsonUtility.ToJson(actor);int savedDungeon=selectedDungeon;
            try{
                var emptyFile=Resources.Load<TextAsset>("Pixel/monster-v53/empty-state-idle");
                if(emptyFile){var emptyBank=JsonUtility.FromJson<EmptyIdleBankV53>(emptyFile.text);foreach(var binding in emptyBank.bindings){if(binding.npc!=actor.sourceNPC||binding.kind!=actor.kind)continue;
                    selectedDungeon=Array.FindIndex(catalog.dungeons,d=>d.id==binding.route);need(selectedDungeon>=0,"Unknown explicit empty-state route");
                    var metadata=Array.Find(catalog.monsters,m=>m.id==actor.kind);var selected=Array.Find(metadata.clips,c=>c.id=="idle");actor.npcState="";actor.clip="idle";actor.state=CombatState.Normal;
                    foreach(float factor in new[]{0f,.31f,1.27f}){BeginMonsterPixelFrameV53();float clock=selected.duration*factor;need(TryEmptyIdlePixelSelectorV53(actor,carrier,"idle",clock,selected,library),"Explicit empty-state idle tuple rejected");need(monsterPixelSelectionsV53.TryGetValue(actor,out var actual)&&actual.motion==binding.motion&&Math.Abs(actual.clock-Mathf.Repeat(clock,selected.duration)*binding.sourceDuration/binding.displayDuration)<.000002,"Explicit idle visual duration conversion differs");cases++;}
                    actor.npcState="nonempty-state-probe";need(!TryEmptyIdlePixelSelectorV53(actor,carrier,"idle",.1f,selected,library),"Empty-state adaptation stole a live source-state clock");actor.npcState="";
                }}
                if(actor.kind=="tree-knight"){
                    var metadata=Array.Find(catalog.monsters,m=>m.id==actor.kind);var selected=Array.Find(metadata.clips,c=>c.id=="source-attack");
                    foreach(string state in new[]{"TREE_KNIGHT_ATTACK_READY","TREE_KNIGHT_ATTACK_A"})foreach(float factor in new[]{0f,.21f,.69f,1.2f}){
                        actor.npcState=state;actor.npcTime=factor;actor.state=CombatState.Normal;
                        var original=TreeKnightHurtV45.OriginalAttackV45(actor,out float seconds);BeginMonsterPixelFrameV53();
                        RecordMonsterPixelAliasV53(actor,carrier,"source-attack",.13f,selected,"source-attack",false);
                        need(monsterPixelSelectionsV53.TryGetValue(actor,out var actual)&&actual.provider=="Pose/TreeKnightHurtV45"&&actual.motion==original.motion&&Math.Abs(actual.clock-Math.Min(seconds,original.duration))<.000002,"Tree ready/fire final owner clock differs");cases++;
                    }
                }
                if(actor.kind=="curse-wisp"||actor.kind=="thief-fat"){
                    var file=Resources.Load<TextAsset>("Pixel/monster-v53/traversal");need(file,"Missing exact traversal binding resource");
                    var bank=JsonUtility.FromJson<PixelTraversalBankV53>(file.text);
                    foreach(var state in bank.states){if(state.kind!=actor.kind||state.npc!=actor.sourceNPC)continue;
                        foreach(float factor in new[]{0f,.31f,1f,1.27f}){
                            actor.npcState=state.state;actor.clip=state.clip;actor.state=state.phase==3?CombatState.Normal:CombatState.Airborne;
                            actor.npcTime=actor.clock=state.sourceDuration*factor;actor.knockdownOnLanding=false;BeginMonsterPixelFrameV53();
                            need(TryTraversalPixelSelectorV53(actor,carrier,actor.clip,library),"Explicit traversal tuple was not selected");
                            float expected=state.loop?Mathf.Repeat(actor.npcTime,state.sourceDuration):Mathf.Min(actor.npcTime,state.sourceDuration);
                            need(monsterPixelSelectionsV53.TryGetValue(actor,out var actual)&&actual.motion==state.motion&&Math.Abs(actual.clock-expected)<.000002,"Traversal phase clock differs from original host age");cases++;
                            actor.knockdownOnLanding=true;need(!TryTraversalPixelSelectorV53(actor,carrier,actor.clip,library),"Hit-launch incorrectly admitted as traversal");actor.knockdownOnLanding=false;
                        }
                    }
                }
            }finally{selectedDungeon=savedDungeon;JsonUtility.FromJsonOverwrite(snapshot,actor);monsterPixelSelectionsV53.Remove(actor);}
            return cases;
        }
#endif
    }
}
