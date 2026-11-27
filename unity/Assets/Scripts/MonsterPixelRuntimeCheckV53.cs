#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class MonsterPixelCheckReportV53 {
            public bool passed,coverageComplete,gameplayUnchanged;
            public int checks,clips,continuousFrames,gpuPairs,normalRed,armorRed,normalPixels,buriedPixels,aliasCases,stateCases,inventoryActors,inventoryRoutes,additionalHostileParts,additionalHostileDevices,directorBindings,legacyCarrierFallbackCases,privateCarrierReferenceCases,sourceOwnerSelectorCases,privateMetadataAliases,privateMetadataFrames;
            public string error,rosterSha256;public string[] unmigrated;
            public long peakReviewLibraryBytes;public int peakReviewLibraryPages,reviewPageSweeps,reviewIdentityReleases;
            public MonsterPixelIdentityCheckV53[] additionalRuntimeChecks;
            public FemalePixelCheckV53 femaleProjection;
        }
        [Serializable] sealed class MonsterPixelIdentityCheckV53 {public string npc,kind,clockReference,sourceClockReviewStatus,privateMetadataResource,privateMetadataSha256;public int aliasCases,sourceReferenceCases,stateCases,carrierStateCases,routeScopedAliasesPending,clips,continuousFrames,normalRed,armorRed,privateMetadataAliases,privateMetadataFrames,inactivePrivateMetadataAliases,routeScopedPrivateAliasesPending;public bool passed,inactiveInConfiguredRoute;public MonsterPixelPrivateAliasCheckV53[] privateAliases;}
        [Serializable] sealed class MonsterPixelPrivateAliasCheckV53 {public string alias,motion,route;public float fps,duration;public int originalFrames,sourceFrames;public bool loop,routeObservationPending,inactiveInConfiguredRoute;}
        [Serializable] sealed class MonsterPixelCheckInventoryV53 {public MonsterPixelCheckIdentityV53[] actors,additionalEntities;public MonsterPixelCheckRouteV53[] routes;}
        [Serializable] sealed class MonsterPixelCheckIdentityV53 {public string kind,sourceNPC;public bool active;}
        [Serializable] sealed class MonsterPixelCheckRouteV53 {public string id;}
        [Serializable] sealed class MonsterPixelCheckExtensionsV53 {public MonsterPixelCheckExtensionV53[] entries;}
        [Serializable] sealed class MonsterPixelCheckExtensionV53 {public string kind,sourceNPC;public bool includeInMonsterBodyMigration;}

        // Run by the root's single Unity batch owner after source imports finish.
        // Deliberately reports incremental coverage separately from test success.
        public static void ReviewMonsterPixelRuntimeV53(){
            var args=Environment.GetCommandLineArgs();int option=Array.IndexOf(args,"--monster-pixel-review");
            string output=option>=0&&option+1<args.Length?Path.GetFullPath(args[option+1]):Path.GetFullPath("Reports/monster-pixel-v53/runtime-check");
            Directory.CreateDirectory(output);var report=new MonsterPixelCheckReportV53();GameObject fixture=null;int exit=1;
            void Need(bool condition,string reason){report.checks++;if(!condition)throw new InvalidOperationException(reason);}
            void ReviewMemory(MonsterPixelFramesV53 library,int page){
                report.peakReviewLibraryBytes=Math.Max(report.peakReviewLibraryBytes,library.ResidentBytes);report.peakReviewLibraryPages=Math.Max(report.peakReviewLibraryPages,library.ResidentPages);
                if(library.ResidentPages<=2)return;
                library.Trim(new HashSet<int>{page},0);EditorUtility.UnloadUnusedAssetsImmediate();report.reviewPageSweeps++;
            }
            try{
                fixture=new GameObject("Monster pixel v53 runtime check");fixture.SetActive(false);var scene=fixture.AddComponent<DemoScene>();
                scene.ReadMonsterPixelRosterV53();
                report.rosterSha256=scene.MonsterPixelRosterShaV53;
                Need(scene.monsterPixelActorsV53.TryGetValue("NUI_BENDERS_NORMAL/benders",out var definition),"Accepted Benders must be registered first");
                Need(definition.root=="Pixel/tree-v52/benders/"&&definition.scale==1&&definition.rootOffsetX==0&&definition.rootOffsetY==0&&definition.facingPolicy=="actor"&&definition.authoredFacing==1,"Accepted Benders root/scale/facing changed");
                var raw=Resources.Load<TextAsset>("Pixel/runtime-catalog");Need(raw,"Missing actual catalog metadata");
                scene.installedCatalogDigest=new CatalogDigest(raw.bytes);scene.catalog=scene.ApplyCompletedRedrawCatalogV31(scene.installedCatalogDigest,raw.text,JsonUtility.FromJson<ArtCatalog>(raw.text));
                scene.selectedDungeon=0;scene.ConfigureRooms();
                var metadata=Array.Find(scene.catalog.monsters,a=>a.id=="benders");Need(metadata!=null,"Missing Benders carrier metadata");
                scene.MakeLibrary(metadata);var legacyCarrier=scene.libraries[metadata.id];
                Need(legacyCarrier.frames.open!=null&&scene.atlasTextures.Count==0,"Migrated carrier eagerly loaded its legacy atlas");
                Need(legacyCarrier.frames.TryFrameCount("idle",out int legacyCount)&&legacyCount>0&&scene.atlasTextures.Count==0,"Carrier metadata lookup loaded legacy pixels");
                report.privateCarrierReferenceCases=scene.ReviewPrivateCarrierReferenceV53(Need);
                using(var gpu=new MonsterPixelGpuCheckV53(output)){
                    var carrier=gpu.Carrier();var actor=new Enemy{sourceNPC="NUI_BENDERS_NORMAL",kind="benders",active=true,hp=100,maxHp=100,visualScale=1,facing=1,state=CombatState.Normal};
                    string snapshot=JsonUtility.ToJson(actor),initialState=actor.npcState;float modelTime=scene.game.elapsed;
                    var source=BendersFamilyRigV36.Load(actor.sourceNPC,Object.DestroyImmediate);
                    try{
                        foreach(var alias in definition.aliases){
                            var clip=Array.Find(metadata.clips,c=>c.id==alias.alias);Need(clip!=null,"Actual carrier alias missing "+alias.alias);
                            foreach(float factor in new[]{0f,.31f,1f,1.27f}){
                                float clock=clip.duration*factor;scene.BeginMonsterPixelFrameV53();
                                scene.RecordMonsterPixelAliasV53(actor,carrier,alias.alias,clock,clip,alias.alias,false);
                                Need(source.Frame(actor,alias.alias,clock,clip.duration,clip.loop,out var expected),"Accepted old source selector rejected "+alias.alias);
                                Need(scene.monsterPixelSelectionsV53.TryGetValue(actor,out var selected)&&selected.motion==expected.alias&&Math.Abs(selected.clock-expected.sourceClock)<.0002f,"Alias clock differs from accepted source selector "+alias.alias);
                                report.aliasCases++;
                            }
                        }
                        foreach(var state in definition.states){
                            actor.npcState=state.state;
                            foreach(float factor in new[]{0f,.43f,1.25f}){
                                actor.npcTime=state.sourceDuration/state.speed*factor;scene.BeginMonsterPixelFrameV53();
                                var clip=new AnimationData{id="registered-state-probe",duration=1};
                                scene.RecordMonsterPixelAliasV53(actor,carrier,clip.id,0,clip,clip.id,false);
                                Need(source.Frame(actor,clip.id,0,1,false,out var expected),"Accepted old state selector rejected "+state.state);
                                Need(scene.monsterPixelSelectionsV53.TryGetValue(actor,out var selected)&&selected.motion==expected.alias&&Math.Abs(selected.clock-expected.sourceClock)<.0002f,"State clock differs from accepted source selector "+state.state);report.stateCases++;
                            }
                        }
                    }finally{source.lib.ReleaseOwned(Object.DestroyImmediate);}
                    actor.npcState=initialState;actor.npcTime=0;Need(JsonUtility.ToJson(actor)==snapshot&&scene.game.elapsed==modelTime,"Presentation selector changed gameplay state");report.gameplayUnchanged=true;
                    var library=scene.MonsterPixelLibraryV53(definition);
                    using(var lazy=new MonsterPixelFramesV53(definition,Object.DestroyImmediate)){
                        Need(lazy.ResidentPages==0,"Library metadata eagerly loaded every atlas");lazy.At("Wait",0,out _,out int page);Need(lazy.ResidentPages==1,"One frame did not load exactly one page");
                        lazy.Trim(new HashSet<int>{page},0);Need(lazy.ResidentPages==1,"Active page was reclaimed");lazy.Trim(new HashSet<int>(),0);Need(lazy.ResidentPages==0,"Unreferenced page was not reclaimed");
                    }
                    Need(scene.TryMonsterPixelPortraitV53(actor,out var portrait),"Danger portrait missing");
                    Need(portrait==library.At("Wait",0,out _,out _),"Danger portrait did not use accepted whole Wait0");
                    report.clips=library.data.clips.Length;
                    foreach(var clip in library.data.clips){
                        for(int i=0;i<clip.frames.Length;i++){
                            float clock=Mathf.Min(clip.duration,i/60f);var sprite=library.At(clip.name,clock,out int index,out int page);ReviewMemory(library,page);
                            Need(sprite&&index==i,"Missing exact 60 Hz exposure "+clip.name+"/"+i+" selected="+index);report.continuousFrames++;
                        }
                    }
                    scene.actorsRoot=gpu.Anchor;scene.worldRoot=gpu.Anchor;scene.game.enemies.Clear();scene.game.enemies.Add(actor);scene.enemyViews.Add(carrier);
                    scene.BeginMonsterPixelFrameV53();scene.RecordMonsterPixelSourceV53(actor,carrier,"owner-fixture","Wait","Wait",0);scene.PrepareMonsterPixelOwnersV53();scene.UpdateMonsterPixelViewsV53();
                    Need(scene.monsterPixelViewsV53.Count==1&&carrier.forceRenderingOff&&scene.monsterPixelViewsV53[actor].renderer.enabled,"Unified owner was not installed");
                    // Exercise the presentation handoff with an already selected
                    // real library. This is a fixture, not a claim of a live Core
                    // STOP acknowledgement (that requires the route/player run).
                    scene.coreEntrySessionV21=true;int residentBeforeEnvelope=library.ResidentPages;
                    var oldEnvelope=new CoreStopWholeEnvelopeV22();scene.coreFourWholeFramesV21[actor]=new CoreStopSourceActorFrameV21{actor=actor,root=carrier.gameObject,renderers=new Renderer[]{carrier},state=actor.npcState,renderedFrame=Time.frameCount,wholeStartEnvelopeV22=oldEnvelope};
                    scene.PrepareMonsterPixelDirectorFramesV53();var directorFrame=scene.coreFourWholeFramesV21[actor];var wholeRenderer=scene.monsterPixelViewsV53[actor].renderer;
                    Need(directorFrame.renderers.Length==1&&directorFrame.renderers[0]==wholeRenderer&&directorFrame.root==wholeRenderer.gameObject,"Director actual draw witness remained on hidden legacy carrier");
                    Need(library.TryClip("Wait",out var directorClip)&&directorFrame.wholeStartEnvelopeV22!=oldEnvelope&&directorFrame.wholeStartEnvelopeV22.phaseCount==directorClip.frames.Length,"Director envelope did not include every source frame");
                    var bounds=wholeRenderer.sprite.bounds;var envelope=directorFrame.wholeStartEnvelopeV22;
                    Need(bounds.min.x>=envelope.minX-.001&&bounds.max.x<=envelope.maxX+.001&&bounds.min.y>=envelope.minY-.001&&bounds.max.y<=envelope.maxY+.001,"Director complete START envelope cropped a displayed whole");
                    Need(library.ResidentPages==residentBeforeEnvelope,"Director envelope eagerly loaded all source atlas pages");
                    scene.UpdateMonsterPixelViewsV53();Need(scene.coreFourWholeFramesV21[actor].renderers[0]==scene.monsterPixelViewsV53[actor].renderer,"Final projection replaced director renderer identity");report.directorBindings++;
                    scene.coreEntrySessionV21=false;scene.coreFourWholeFramesV21.Clear();
                    var child=new Enemy{sourceNPC=actor.sourceNPC,kind=actor.kind,active=true,hp=10,maxHp=10,visualScale=1,facing=-1,x=99};var childCarrier=gpu.Carrier();scene.game.enemies.Add(child);scene.enemyViews.Add(childCarrier);
                    scene.RecordMonsterPixelSourceV53(child,childCarrier,"summon-fixture","Wait","Wait",.1f);scene.UpdateMonsterPixelViewsV53();
                    Need(scene.monsterPixelViewsV53.Count==2&&childCarrier.forceRenderingOff,"Dynamic actor outside the stage birth list was omitted");
                    scene.game.enemies.Remove(child);scene.BeginMonsterPixelFrameV53();scene.RecordMonsterPixelSourceV53(actor,carrier,"owner-fixture","Wait","Wait",0);scene.PrepareMonsterPixelOwnersV53();
                    Need(scene.monsterPixelViewsV53.Count==1&&!childCarrier.forceRenderingOff,"Removed summon retained a whole owner");
                    scene.BeginMonsterPixelFrameV53();scene.PrepareMonsterPixelOwnersV53();Need(scene.monsterPixelViewsV53.Count==0&&!carrier.forceRenderingOff,"Missing current selection retained stale ownership");
                    scene.enemyViews.Clear();scene.actorsRoot=null;scene.worldRoot=null;Object.DestroyImmediate(childCarrier.gameObject);
                    gpu.VerifyAcceptedParity(library,carrier,report,Need);
                    scene.monsterPixelPortraitLibraryV53=null;scene.monsterPixelPortraitPageV53=-1;
                    library.Dispose();scene.monsterPixelLibrariesV53.Remove(MonsterPixelIdentityV53(definition.npc,definition.kind));scene.monsterPixelSelectionsV53.Clear();EditorUtility.UnloadUnusedAssetsImmediate();report.reviewIdentityReleases++;
                    var additional=new List<MonsterPixelIdentityCheckV53>();
                    foreach(var registration in scene.monsterPixelActorsV53.Values){
                        if(registration.npc=="NUI_BENDERS_NORMAL"&&registration.kind=="benders")continue;
                        var check=new MonsterPixelIdentityCheckV53{npc=registration.npc,kind=registration.kind,clockReference="manifest-vs-current-catalog"};additional.Add(check);
                        if(registration.assembly=="female-six-root"){
                            report.femaleProjection=CheckFemalePixelProjectionV53(Path.Combine(output,"female-six-root"));
                            Need(report.femaleProjection.passed,"Six-root source projection failed: "+report.femaleProjection.error);
                            check.clockReference="FemaleNativeV28 original clock and six native root tracks";check.sourceClockReviewStatus="six-root source clock and GPU projection fixture passed; actual tree-2 route still required";check.clips=report.femaleProjection.roots;check.continuousFrames=report.femaleProjection.continuousFrames;check.passed=true;EditorUtility.UnloadUnusedAssetsImmediate();report.reviewIdentityReleases++;continue;
                        }
                        var actorMetadata=Array.Find(scene.catalog.monsters,a=>a.id==registration.kind);
                        var registeredActor=new Enemy{sourceNPC=registration.npc,kind=registration.kind,active=true,hp=100,maxHp=100,visualScale=1,facing=1,state=CombatState.Normal};
                        var registeredCarrier=gpu.Carrier();var registeredLibrary=scene.MonsterPixelLibraryV53(registration);BendersFamilyRigV36 oldSource=null;GiantWilliamBankV39 privateSource=null;
                        try{
                            report.sourceOwnerSelectorCases+=scene.ReviewSourceOwnerSelectorsV53(registeredActor,registeredCarrier,registeredLibrary,Need);
                            if(BendersFamilyPinsV36.Known(registration.npc)){oldSource=BendersFamilyRigV36.Load(registration.npc,Object.DestroyImmediate);check.clockReference="accepted-BendersFamilyRigV36.Frame";}
                            // These two original whole owners register source-native
                            // clips privately; they are not catalog Pose carriers.
                            // Keep their exact metadata in the test denominator,
                            // without inventing a selected display clock or route.
                            bool privateIdentity=registration.kind=="white-giant"&&registration.npc=="NUI_PPORU_GIANT_WHITE"||registration.kind=="william"&&registration.npc=="NUI_PPORU_WILLIAM_NORMAL";
                            if(privateIdentity){
                                privateSource=new GiantWilliamBankV39(registration.kind,registration.npc,scene.spriteOffsets,Object.DestroyImmediate);
                                check.privateMetadataResource="Pixel/feedback-v39/"+registration.kind+"/library";
                                var privateDocument=Resources.Load<TextAsset>(check.privateMetadataResource);Need(privateDocument,"Missing registered private owner metadata "+registration.kind);
                                using(var hash=System.Security.Cryptography.SHA256.Create())check.privateMetadataSha256=BitConverter.ToString(hash.ComputeHash(privateDocument.bytes)).Replace("-","").ToLowerInvariant();
                                check.clockReference="GiantWilliamBankV39 private source metadata; catalog Pose aliases tested separately";
                                if(registration.kind=="white-giant"){
                                    var field=typeof(GameModel).GetField("rooms",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
                                    var configured=field==null?null:field.GetValue(scene.game) as List<RoomDefinition>;
                                    Need(configured!=null&&scene.Dungeon.id=="tree","Cannot establish the actual configured white giant route");
                                    bool born=false;foreach(var room in configured)foreach(var spawn in room.enemies)if(spawn.kind==registration.kind&&spawn.sourceNPC==registration.npc)born=true;
                                    Need(!born,"White giant production birth unexpectedly returned after Benders");check.inactiveInConfiguredRoute=true;
                                }
                            }
                            var privateAliases=new List<MonsterPixelPrivateAliasCheckV53>();
                            if(registration.aliases!=null)foreach(var alias in registration.aliases){
                                var clip=actorMetadata==null?null:Array.Find(actorMetadata.clips,c=>c.id==alias.alias);
                                if(clip==null&&privateSource!=null){
                                    var verified=ReviewGiantWilliamPrivateAliasV53(privateSource,registeredLibrary,alias,Need);
                                    bool otherRoute=!string.IsNullOrEmpty(alias.route)&&alias.route!="tree";
                                    verified.routeObservationPending=otherRoute;verified.inactiveInConfiguredRoute=check.inactiveInConfiguredRoute;
                                    privateAliases.Add(verified);check.privateAliases=privateAliases.ToArray();check.privateMetadataAliases++;check.privateMetadataFrames+=verified.originalFrames;
                                    report.privateMetadataAliases++;report.privateMetadataFrames+=verified.originalFrames;
                                    if(check.inactiveInConfiguredRoute)check.inactivePrivateMetadataAliases++;
                                    if(otherRoute){check.routeScopedAliasesPending++;check.routeScopedPrivateAliasesPending++;}
                                    continue;
                                }
                                if(!string.IsNullOrEmpty(alias.route)&&alias.route!="tree"){check.routeScopedAliasesPending++;continue;}
                                Need(clip!=null,"Installed catalog alias missing "+registration.kind+"/"+alias.alias);
                                foreach(float factor in new[]{0f,.31f,1f,1.27f}){
                                    float clock=clip.duration*factor;scene.BeginMonsterPixelFrameV53();scene.RecordMonsterPixelAliasV53(registeredActor,registeredCarrier,alias.alias,clock,clip,alias.alias,false);
                                    Need(scene.monsterPixelSelectionsV53.TryGetValue(registeredActor,out var selected),"Installed alias pending "+registration.kind+"/"+alias.alias);
                                    if(oldSource!=null){
                                        Need(Array.Exists(oldSource.aliases,a=>a.key==alias.alias),"Manifest alias would override original source-state priority "+registration.kind+"/"+alias.alias);
                                        Need(oldSource.Frame(registeredActor,alias.alias,clock,clip.duration,clip.loop,out var expected),"Previous source selector rejected "+registration.kind+"/"+alias.alias);
                                        Need(selected.motion==expected.alias&&Math.Abs(selected.clock-expected.sourceClock)<.0002f,"Installed source clock differs "+registration.kind+"/"+alias.alias);check.sourceReferenceCases++;
                                    }
                                    check.aliasCases++;
                                }
                            }
                            if(privateSource!=null)Need(check.privateMetadataAliases==privateSource.Data.sourceMotionCount,"Private source aliases were removed from the checker denominator "+registration.kind);
                            if(oldSource!=null){
                                // Exercise real state clocks even when the carrier's
                                // idle/walk aliases are absent from the old bank. In
                                // that case Frame uses npcTime*speed, never the
                                // catalog display duration or its looping clock.
                                foreach(var state in oldSource.states){
                                    registeredActor.npcState=state.key;
                                    foreach(float factor in new[]{0f,.43f,1.25f}){
                                        registeredActor.npcTime=oldSource.lib.Find(state.motion).duration/state.speed*factor;
                                        var clip=new AnimationData{id="registered-state-probe",duration=1};
                                        scene.BeginMonsterPixelFrameV53();scene.RecordMonsterPixelAliasV53(registeredActor,registeredCarrier,clip.id,.73f,clip,clip.id,false);
                                        Need(oldSource.Frame(registeredActor,clip.id,.73f,clip.duration,false,out var expected),"Previous source state missing "+registration.kind+"/"+state.key);
                                        Need(scene.monsterPixelSelectionsV53.TryGetValue(registeredActor,out var selected)&&selected.motion==expected.alias&&Math.Abs(selected.clock-expected.sourceClock)<.0002f,"Installed source state clock differs "+registration.kind+"/"+state.key);check.stateCases++;
                                    }
                                }
                                if(registration.npc=="NUI_THIEF_THIN_ZERO")foreach(var alias in new[]{"idle","walk"}){
                                    string stateKey=alias=="idle"?"THIEF_THIN_ZERO_WAIT":"THIEF_THIN_ZERO_WALK";
                                    var state=Array.Find(oldSource.states,s=>s.key==stateKey);var clip=Array.Find(actorMetadata.clips,c=>c.id==alias);
                                    Need(state!=null&&clip!=null&&!Array.Exists(oldSource.aliases,a=>a.key==alias),"Thief real carrier/state fixture changed "+alias);
                                    registeredActor.npcState=state.key;
                                    foreach(float factor in new[]{0f,.17f,.61f,1.27f}){
                                        registeredActor.npcTime=oldSource.lib.Find(state.motion).duration/state.speed*factor;
                                        // Deliberately unrelated: the original fallback
                                        // must ignore this display clock completely.
                                        float displayClock=clip.duration*(1.81f-factor*.19f);
                                        scene.BeginMonsterPixelFrameV53();scene.RecordMonsterPixelAliasV53(registeredActor,registeredCarrier,alias,displayClock,clip,alias,false);
                                        Need(oldSource.Frame(registeredActor,alias,displayClock,clip.duration,clip.loop,out var expected),"Thief actual carrier state rejected "+state.key);
                                        Need(scene.monsterPixelSelectionsV53.TryGetValue(registeredActor,out var selected)&&selected.provider=="Pose/registered-source-state"&&selected.motion==expected.alias&&Math.Abs(selected.clock-expected.sourceClock)<.0002f,"Thief carrier alias stole source-state clock "+alias+"/"+factor);check.carrierStateCases++;
                                    }
                                }
                                registeredActor.npcState="";registeredActor.npcTime=0;
                            }
                            check.clips=registeredLibrary.data.clips.Length;
                            foreach(var clip in registeredLibrary.data.clips)for(int i=0;i<clip.frames.Length;i++){
                                var sprite=registeredLibrary.At(clip.name,Mathf.Min(clip.duration,i/60f),out int index,out int page);ReviewMemory(registeredLibrary,page);Need(sprite&&index==i,"Installed exact source exposure missing "+registration.kind+"/"+clip.name+"/"+i+" selected="+index);check.continuousFrames++;
                            }
                            gpu.VerifyInstalledMaterial(registeredLibrary,registeredCarrier,registration.standingMotion,check,Need);
                            check.sourceClockReviewStatus=check.privateMetadataAliases>0?(check.inactiveInConfiguredRoute?"all private source aliases/frames validated; actual configured tree rooms prove this historical actor inactive; no private Pose clock claimed":"all private source aliases/frames validated; other-route private owner clocks still require actual route observation"):check.routeScopedAliasesPending>0?"other route-specific alias clocks require actual route observation":oldSource!=null?"original family source selector fixture passed; actual route still required":check.aliasCases>0?"tree manifest alias fixture passed; original product/route selector witnesses still required":"baked frame exposure/material only; actual source-owner route clocks not exercised";
                            check.passed=true;
                        }finally{
                            report.additionalRuntimeChecks=additional.ToArray();
                            if(privateSource!=null)privateSource.Dispose();
                            if(oldSource!=null)oldSource.lib.ReleaseOwned(Object.DestroyImmediate);Object.DestroyImmediate(registeredCarrier.gameObject);
                            scene.monsterPixelSelectionsV53.Remove(registeredActor);registeredLibrary.Dispose();scene.monsterPixelLibrariesV53.Remove(MonsterPixelIdentityV53(registration.npc,registration.kind));EditorUtility.UnloadUnusedAssetsImmediate();report.reviewIdentityReleases++;
                        }
                    }
                    report.additionalRuntimeChecks=additional.ToArray();
                    // Missing bindings are explicit failures with the old view left
                    // available. They must never silently become a successful idle.
                    scene.BeginMonsterPixelFrameV53();
                    Need(!scene.RecordMonsterPixelSourceV53(actor,carrier,"negative-control","missing","__unbaked_v53_test__",0),"Unknown motion was accepted");
                    Need(!scene.MonsterPixelOwnsActorV53(actor),"Unknown motion suppressed its legacy owner");
                    var legacySprite=legacyCarrier.frames.At("idle",0,out int legacyIndex);Need(legacySprite&&legacyIndex==0&&legacyCarrier.clips.ContainsKey("idle"),"Deferred legacy carrier cannot serve a missing-v53-motion fallback");report.legacyCarrierFallbackCases++;
                    scene.RecordMonsterPixelAliasV53(actor,carrier,"idle",0,new AnimationData{id="idle",duration=1},"__missing_alias__",true);
                    var coverage=scene.ReadMonsterPixelCoverageV53();Need(!coverage.complete&&Array.Exists(coverage.observations,r=>r.status=="fallback"&&r.reason=="legacy-idle-fallback"),"Fallback absent from runtime report");
                    File.WriteAllText(Path.Combine(output,"observed-selectors.json"),JsonUtility.ToJson(coverage,true));
                    Object.DestroyImmediate(carrier.gameObject);
                }
                string inventoryPath=Path.GetFullPath("Reports/monster-pixel-v53/inventory.json");
                Need(File.Exists(inventoryPath),"Whole-route inventory missing");var inventory=JsonUtility.FromJson<MonsterPixelCheckInventoryV53>(File.ReadAllText(inventoryPath));
                report.inventoryActors=inventory.actors.Length;report.inventoryRoutes=inventory.routes.Length;
                Need(report.inventoryActors>=105&&report.inventoryRoutes==20,"Inventory silently narrowed below 105 identities / 20 routes");
                var missing=new List<string>();foreach(var actor in inventory.actors)if(!scene.monsterPixelActorsV53.ContainsKey(MonsterPixelIdentityV53(actor.sourceNPC,actor.kind)))missing.Add(actor.sourceNPC+"/"+actor.kind);
                if(inventory.additionalEntities!=null)foreach(var actor in inventory.additionalEntities)if(actor.kind!=null&&actor.kind.StartsWith("cannon-fort-parasite-",StringComparison.Ordinal)){
                    report.additionalHostileParts++;if(!scene.monsterPixelActorsV53.ContainsKey(MonsterPixelIdentityV53(actor.sourceNPC,actor.kind)))missing.Add(actor.sourceNPC+"/"+actor.kind);
                }
                Need(report.additionalHostileParts==7,"Seven hostile fortress components disappeared from inventory");
                string extensionPath=Path.GetFullPath("../art/production/lk-native-v02/dungeon/monster-pixel-v53/extension-classification.json");
                Need(File.Exists(extensionPath),"Visible hostile device inventory missing");
                var extensions=JsonUtility.FromJson<MonsterPixelCheckExtensionsV53>(File.ReadAllText(extensionPath));
                foreach(var actor in extensions.entries)if(actor.includeInMonsterBodyMigration){
                    report.additionalHostileDevices++;string id=MonsterPixelIdentityV53(actor.sourceNPC,actor.kind);if(!scene.monsterPixelActorsV53.ContainsKey(id)&&!missing.Contains(id))missing.Add(id);
                }
                Need(report.additionalHostileDevices==22,"Twenty-two visible hostile device bodies disappeared from inventory");
                report.unmigrated=missing.ToArray();report.coverageComplete=false;report.passed=true;exit=0;
            }catch(Exception error){report.error=error.ToString();Debug.LogException(error);}
            finally{
                if(fixture)Object.DestroyImmediate(fixture);
                File.WriteAllText(Path.Combine(output,"runtime-check.json"),JsonUtility.ToJson(report,true));
                if(exit==0&&Array.IndexOf(args,"--monster-pixel-continue-routes")>=0){
                    if(Array.IndexOf(args,"--monster-pixel-route-review")<0)throw new ArgumentException("Route continuation requires its own explicit output");
                    // Static fixture is destroyed above. Open the real build
                    // scene and let its sole runtime observer attach in Play.
                    StartMonsterPixelRouteReviewV53();
                }else EditorApplication.Exit(exit);
            }
        }

        static MonsterPixelPrivateAliasCheckV53 ReviewGiantWilliamPrivateAliasV53(GiantWilliamBankV39 owner,MonsterPixelFramesV53 current,MonsterPixelAliasV53 alias,Action<bool,string> need){
            var motion=Array.Find(owner.Data.clips,c=>c.id==alias.alias);var binding=owner.Binding(alias.alias);
            need(motion!=null&&binding!=null,"Missing exact registered private alias "+owner.Data.id+"/"+alias.alias);
            need(alias.alias=="source-native-"+motion.sourceAnimation&&alias.motion==motion.sourceAnimation&&binding.motion==motion.sourceAnimation,
                 "Private alias source identity differs "+owner.Data.id+"/"+alias.alias);
            need(!alias.hold&&!binding.paint&&binding.count==0&&binding.fps==0&&binding.duration==0&&binding.indices!=null&&binding.indices.Length==0,
                 "Original private source-clock sentinel changed "+owner.Data.id+"/"+alias.alias);
            need(motion.fps==24&&motion.duration>0&&motion.frames!=null&&motion.frames.Length==Mathf.CeilToInt(motion.duration*24-.001f)+1,
                 "Original private source exposure metadata changed "+owner.Data.id+"/"+alias.alias);
            need(Math.Abs(alias.displayDuration-motion.duration)<.0002f&&Math.Abs(alias.sourceDuration-motion.duration)<.0002f,
                 "Private alias duration differs from its actual owner "+owner.Data.id+"/"+alias.alias);
            need(current.TryClip(alias.motion,out var source)&&source.fps==60&&Math.Abs(source.duration-motion.duration)<.0002f,
                 "Private source duration absent from complete 60 Hz library "+owner.Data.id+"/"+alias.alias);
            need(!string.IsNullOrEmpty(alias.route)&&Array.Exists(owner.Data.stages,s=>s==alias.route||s.StartsWith(alias.route+"-",StringComparison.Ordinal)),
                 "Private alias route lacks original owner registration "+owner.Data.id+"/"+alias.alias);
            foreach(var frame in motion.frames){
                var page=frame==null?null:Array.Find(owner.Data.pages,p=>p.resource==frame.texture);
                need(frame!=null&&frame.hasAnchor&&page!=null&&frame.x>=0&&frame.y>=0&&frame.w>0&&frame.h>0&&frame.x+frame.w<=page.w&&frame.y+frame.h<=page.h,
                     "Private original frame metadata is invalid "+owner.Data.id+"/"+alias.alias);
            }
            // This is original metadata validation, not a fabricated Pose call.
            // Actual reaction/William attack adapters own source seconds, and
            // every new source frame and GPU material is checked below as usual.
            return new MonsterPixelPrivateAliasCheckV53{alias=alias.alias,motion=motion.sourceAnimation,route=alias.route,
                fps=motion.fps,duration=motion.duration,loop=motion.loop,originalFrames=motion.frames.Length,sourceFrames=source.frames.Length};
        }

        int ReviewPrivateCarrierReferenceV53(Action<bool,string> need){
            // Compare the deferred private owner with the preexisting eager V45
            // producer on real installed catalog data, including reused images.
            foreach(var data in catalog.monsters){
                if(!DeferMonsterPixelCarrierV53(data.id))continue;
                foreach(var clip in data.clips){
                    if(SourceMotionInkV45.For(data,clip)==null)continue;
                    var sample=new SpriteLibraryData{id=data.id,artVersion=data.artVersion,texture=data.texture,clips=new[]{clip}};
                    var expected=new Library{data=sample};var actual=new Library{data=sample};
                    var owned=new List<UnityEngine.Object>();var deferred=new List<UnityEngine.Object>();
                    try{
                        need(DeferPrivateFramesV53(actual,frame=>throw new InvalidOperationException("Private V45 unexpectedly selected the raw atlas"),deferred,true),"Reviewed private bank was not deferred");
                        need(actual.frames.ContainsKey(clip.id)&&actual.frames.TryFrameCount(clip.id,out int count)&&count==clip.frames.Length&&deferred.Count==0,"Private metadata access decoded old pictures");
                        need(BuildPrivateMotionV45(expected,clip,owned),"Original private reference producer rejected its registered clip");
                        int cases=0;foreach(int requested in new[]{0,clip.frames.Length/3,clip.frames.Length*2/3,clip.frames.Length-1}){
                            var before=expected.frames[clip.id][requested];var after=actual.frames.At(clip.id,requested,out int index);
                            need(index==requested&&after&&before.texture==after.texture&&before.rect==after.rect&&before.pivot==after.pivot&&before.pixelsPerUnit==after.pixelsPerUnit&&spriteOffsets[before]==spriteOffsets[after],"Private deferred fallback pixels/anchor differ from original producer "+data.id+"/"+clip.id+"/"+requested);cases++;
                        }
                        need(ReferenceEquals(actual.clips[clip.id],clip),"Private metadata duration/loop owner was replaced");return cases;
                    }finally{
                        foreach(var value in owned){var sprite=value as Sprite;if(sprite){spriteOffsets.Remove(sprite);nativeUiVisibleLocalRectsV28.Remove(sprite);}if(value)Object.DestroyImmediate(value);}
                        foreach(var value in deferred){var sprite=value as Sprite;if(sprite){spriteOffsets.Remove(sprite);nativeUiVisibleLocalRectsV28.Remove(sprite);}if(value)Object.DestroyImmediate(value);}
                    }
                }
            }
            throw new InvalidOperationException("No real registered private V45 reference available for deferred fallback verification");
        }

        sealed class MonsterPixelGpuCheckV53 : IDisposable {
            readonly string output;readonly GameObject root;readonly Camera camera;readonly RenderTexture target;readonly Texture2D readback;readonly Material material;readonly Sprite placeholder;
            internal Transform Anchor {get{return root.transform;}}
            internal Camera Camera {get{return camera;}}
            internal Color32[] CapturePixels(string name=null){return Capture(name);}
            internal MonsterPixelGpuCheckV53(string output){
                this.output=output;root=new GameObject("v53 whole-frame parity GPU");camera=new GameObject("v53 camera").AddComponent<Camera>();camera.transform.SetParent(root.transform);
                camera.enabled=false;camera.orthographic=true;camera.orthographicSize=128;camera.aspect=2;camera.transform.position=new Vector3(0,123,-100);camera.nearClipPlane=.1f;camera.farClipPlane=250;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.cullingMask=1<<31;camera.allowHDR=camera.allowMSAA=false;
                target=new RenderTexture(512,256,24){antiAliasing=1,filterMode=FilterMode.Point};target.Create();camera.targetTexture=target;readback=new Texture2D(512,256,TextureFormat.RGBA32,false){hideFlags=HideFlags.DontUnloadUnusedAsset};
                material=new Material(Resources.Load<Shader>("Pixel/Shaders/ArmoredSprite")){hideFlags=HideFlags.DontUnloadUnusedAsset};material.SetColor("_Color",Color.white);placeholder=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,1,1),Vector2.zero,1);placeholder.hideFlags=HideFlags.DontUnloadUnusedAsset;
                // The sweep intentionally releases unused library pages. These
                // fixture resources are held by a local non-Mono helper, which
                // Unity's asset sweep does not treat as a reference root. Keep
                // them alive until the explicit Dispose below.
            }
            internal SpriteRenderer Carrier(){var node=new GameObject("Carrier"){layer=31};node.transform.SetParent(root.transform);var r=node.AddComponent<SpriteRenderer>();r.sprite=placeholder;r.sharedMaterial=material;return r;}
            Color32[] Capture(string name=null){camera.Render();var previous=RenderTexture.active;RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,512,256),0,0);readback.Apply();RenderTexture.active=previous;if(name!=null)File.WriteAllBytes(Path.Combine(output,name+".png"),readback.EncodeToPNG());return readback.GetPixels32();}
            static int Count(Color32[] pixels,Func<Color32,bool> condition){int count=0;foreach(var p in pixels)if(condition(p))count++;return count;}
            internal void VerifyAcceptedParity(MonsterPixelFramesV53 library,SpriteRenderer carrier,MonsterPixelCheckReportV53 report,Action<bool,string> need){
                using(var accepted=new TreeBendersFrames(carrier,Object.DestroyImmediate))
                using(var whole=new MonsterPixelViewV53(carrier,Object.DestroyImmediate)){
                    var properties=new MaterialPropertyBlock();properties.SetFloat("_MonsterInk",1);properties.SetColor("_OutlineColor",new Color(1,.14f,.26f,1));carrier.SetPropertyBlock(properties);
                    // The old carrier may carry an obsolete part-rig tilt. Neither
                    // complete-model path may inherit it implicitly.
                    carrier.transform.localRotation=Quaternion.Euler(0,0,27);
                    foreach(string motion in new[]{"Wait","Dash","AttackA_Ready","AttackA_Fire","AttackB_Fire","SpecialAttackA_Fire","DamageSmall","Dying"}){
                        need(library.TryClip(motion,out var clip),"Required accepted motion missing "+motion);
                        foreach(float phase in new[]{0f,.37f,.81f})foreach(int facing in new[]{1,-1}){
                            float clock=clip.duration*phase;
                            whole.renderer.enabled=false;accepted.Apply(new FairyAuthoredPartsV33.Frame{alias=motion,sourceClock=clock,facing=facing,visible=true},Vector3.zero);var expected=Capture();accepted.Renderer.enabled=false;
                            whole.Apply(library.At(motion,clock,out _,out _),Vector3.zero,1,facing,true);var actual=Capture();
                            need(whole.renderer.transform.localRotation==Quaternion.identity,"Legacy skeleton tilt leaked into whole frames");
                            need(expected.Length==actual.Length,"GPU dimensions changed");int differences=0;for(int i=0;i<actual.Length;i++)if(!actual[i].Equals(expected[i]))differences++;
                            need(differences==0,"Accepted v52 pixels changed: "+motion+"/"+phase+"/"+facing+" differences="+differences);report.gpuPairs++;
                        }
                    }
                    var sprite=library.At("Wait",0,out _,out _);accepted.Renderer.enabled=false;
                    void Apply(){carrier.SetPropertyBlock(properties);whole.Apply(sprite,Vector3.zero,1,1,true);}
                    properties.SetFloat("_Outline",0);Apply();var normal=Capture("benders-v53-normal");report.normalRed=Count(normal,p=>p.a>0&&p.r>210&&p.g<80&&p.b<100);report.normalPixels=Count(normal,p=>p.a>0);
                    properties.SetFloat("_Outline",1);Apply();var armor=Capture("benders-v53-armor");report.armorRed=Count(armor,p=>p.a>0&&p.r>210&&p.g<80&&p.b<100);need(report.armorRed>=report.normalRed+35,"Lost super-armor red contour");
                    properties.SetFloat("_Outline",0);properties.SetFloat("_HitFlash",.65f);Apply();var flash=Capture("benders-v53-hit-flash");long normalLight=0,flashLight=0;for(int i=0;i<normal.Length;i++){normalLight+=normal[i].r+normal[i].g+normal[i].b;flashLight+=flash[i].r+flash[i].g+flash[i].b;}need(flashLight>normalLight,"Lost inherited hit flash");
                    properties.SetFloat("_HitFlash",0);properties.SetFloat("_BurialClip",1);properties.SetFloat("_BurialGroundY",60);Apply();var buried=Capture("benders-v53-buried");report.buriedPixels=Count(buried,p=>p.a>0);need(report.buriedPixels>0&&report.buriedPixels<report.normalPixels,"Burial clipping was lost");
                    var copied=new MaterialPropertyBlock();whole.renderer.GetPropertyBlock(copied);need(copied.GetFloat("_MonsterInk")==0&&copied.GetFloat("_BurialClip")==1,"Double ink or missing burial MPB");
                    carrier.enabled=false;whole.Apply(sprite,Vector3.zero,1,1,true);need(!whole.renderer.enabled,"Hidden carrier became visible");carrier.enabled=true;
                }
                need(!carrier.forceRenderingOff,"Disposing whole owner did not restore carrier");
            }
            internal void VerifyInstalledMaterial(MonsterPixelFramesV53 library,SpriteRenderer carrier,string standing,MonsterPixelIdentityCheckV53 report,Action<bool,string> need){
                var cameraPosition=camera.transform.position;float cameraSize=camera.orthographicSize;
                using(var whole=new MonsterPixelViewV53(carrier,Object.DestroyImmediate)){
                    var properties=new MaterialPropertyBlock();properties.SetFloat("_MonsterInk",1);properties.SetColor("_OutlineColor",new Color(1,.14f,.26f,1));var sprite=library.At(standing,0,out _,out _);
                    // An eye, a giant tree and a fortress cannot share Benders'
                    // fixed framing. Fit this body once, then mirror the camera
                    // center together with the tested facing so neither side is
                    // cropped by the fixture itself.
                    camera.orthographicSize=Mathf.Max(2,sprite.bounds.extents.y*1.2f,sprite.bounds.extents.x*.6f);
                    carrier.transform.localRotation=Quaternion.Euler(0,0,27);
                    void Apply(int facing=1){camera.transform.position=new Vector3(sprite.bounds.center.x*facing,sprite.bounds.center.y,-100);carrier.SetPropertyBlock(properties);whole.Apply(sprite,Vector3.zero,1,facing,true);}
                    Apply();var normal=Capture(report.kind+"-normal");need(whole.renderer.transform.localRotation==Quaternion.identity,"Installed whole inherited a legacy rig tilt");
                    report.normalRed=Count(normal,p=>p.a>0&&p.r>210&&p.g<80&&p.b<100);int opaque=Count(normal,p=>p.a>0);need(opaque>40,"Installed whole is invisible");
                    Apply(-1);var reverse=Capture(report.kind+"-reverse");need(Math.Abs(Count(reverse,p=>p.a>0)-opaque)<=2,"Whole-image mirroring changed visible area");
                    properties.SetFloat("_Outline",1);Apply();var armor=Capture(report.kind+"-armor");report.armorRed=Count(armor,p=>p.a>0&&p.r>210&&p.g<80&&p.b<100);need(report.armorRed>=report.normalRed+20,"Installed whole lost armor contour");
                    properties.SetFloat("_Outline",0);properties.SetFloat("_HitFlash",.65f);Apply();var flash=Capture(report.kind+"-hit-flash");long baseline=0,bright=0;for(int i=0;i<normal.Length;i++){baseline+=normal[i].r+normal[i].g+normal[i].b;bright+=flash[i].r+flash[i].g+flash[i].b;}need(bright>baseline,"Installed whole lost hit flash");
                    properties.SetFloat("_HitFlash",0);properties.SetFloat("_BurialClip",1);properties.SetFloat("_BurialGroundY",sprite.bounds.center.y);Apply();var buried=Capture(report.kind+"-buried");int visible=Count(buried,p=>p.a>0);need(visible>0&&visible<opaque,"Installed whole lost burial clip");
                }
                camera.transform.position=cameraPosition;camera.orthographicSize=cameraSize;
                need(!carrier.forceRenderingOff,"Installed owner did not restore carrier");
            }
            public void Dispose(){if(camera)camera.targetTexture=null;if(target){target.Release();Object.DestroyImmediate(target);}Object.DestroyImmediate(readback);Object.DestroyImmediate(placeholder);Object.DestroyImmediate(material);Object.DestroyImmediate(root);}
        }
    }
}
#endif
