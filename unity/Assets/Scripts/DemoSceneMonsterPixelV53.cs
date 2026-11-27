using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class MonsterSelectionV53 {
            public Enemy actor;public SpriteRenderer carrier;public MonsterPixelActorV53 definition;
            public MonsterPixelFramesV53 library;public Sprite sprite;
            public string provider,alias,motion;public float clock,rotationZ;public int frame,page,facing;
        }
        [Serializable] public sealed class MonsterPixelObservationV53 {
            public string route,room,npc,kind,state,combatState,provider,alias,motion,status,reason;
            public int samples,frame,page,lastRenderFrame,actorIndex;public float sourceClock;public bool active,dormant,carrierExists,carrierEnabled,carrierHasSprite,carrierActiveInHierarchy;
        }
        [Serializable] public sealed class MonsterPixelCoverageV53 {
            public string schema="monster-pixel-runtime-coverage-v53";
            public string rosterSha256;
            public int registeredIdentities,loadedLibraries,residentPages;public long residentBytes;
            public bool complete;public MonsterPixelObservationV53[] observations;
        }
        readonly Dictionary<string,MonsterPixelActorV53> monsterPixelActorsV53=new Dictionary<string,MonsterPixelActorV53>(StringComparer.Ordinal);
        readonly HashSet<string> monsterPixelDeferredCarrierKindsV53=new HashSet<string>(StringComparer.Ordinal);
        readonly Dictionary<string,MonsterPixelFramesV53> monsterPixelLibrariesV53=new Dictionary<string,MonsterPixelFramesV53>(StringComparer.Ordinal);
        readonly Dictionary<Enemy,MonsterSelectionV53> monsterPixelSelectionsV53=new Dictionary<Enemy,MonsterSelectionV53>();
        readonly Dictionary<Enemy,MonsterPixelViewV53> monsterPixelViewsV53=new Dictionary<Enemy,MonsterPixelViewV53>();
        readonly HashSet<Renderer> monsterPixelRenderersV53=new HashSet<Renderer>();
        readonly Dictionary<string,MonsterPixelObservationV53> monsterPixelObservationsV53=new Dictionary<string,MonsterPixelObservationV53>(StringComparer.Ordinal);
        readonly HashSet<string> monsterPixelFailuresV53=new HashSet<string>(StringComparer.Ordinal);
        readonly HashSet<Enemy> monsterPixelSourceHiddenV53=new HashSet<Enemy>();
        readonly List<Enemy> monsterPixelStaleV53=new List<Enemy>();
        bool monsterPixelRosterReadV53;string monsterPixelRosterShaV53;
        internal string MonsterPixelRosterShaV53 {get{ReadMonsterPixelRosterV53();return monsterPixelRosterShaV53;}}
        MonsterPixelFramesV53 monsterPixelPortraitLibraryV53;int monsterPixelPortraitPageV53=-1;
        MonsterSelectionV53 lastAncientPose;

        static string MonsterPixelIdentityV53(string npc,string kind){return (npc??"")+"/"+(kind??"");}
        void ReadMonsterPixelRosterV53(){
            if(monsterPixelRosterReadV53)return;monsterPixelRosterReadV53=true;
            var file=Resources.Load<TextAsset>("Pixel/monster-v53/roster");if(!file)return;
            using(var hash=System.Security.Cryptography.SHA256.Create())monsterPixelRosterShaV53=BitConverter.ToString(hash.ComputeHash(file.bytes)).Replace("-","").ToLowerInvariant();
            var roster=JsonUtility.FromJson<MonsterPixelRosterV53>(file.text);
            if(roster==null||roster.actors==null)throw new InvalidOperationException("Invalid monster-v53 roster");
            foreach(var actor in roster.actors){
                if(actor==null||string.IsNullOrEmpty(actor.npc)||string.IsNullOrEmpty(actor.kind)||string.IsNullOrEmpty(actor.root))throw new InvalidOperationException("Incomplete monster-v53 identity");
                if(actor.facingPolicy!="actor"&&actor.facingPolicy!="carrier"&&actor.facingPolicy!="fixed")throw new InvalidOperationException("Explicit v53 facing policy required: "+actor.kind);
                if(actor.npc=="NUI_MONKEY_BOMB" && actor.kind=="monkey-bomb")actor.scale*=GameModel.BombMonkeyDisplayScale;
                if(actor.npc=="NUI_FAIRY_GUARDIAN" && actor.kind=="fairy-guardian"){
                    // The atlas used the full standing height to normalize the
                    // shorter TrapModeWait mesh. Restore the original pose ratio.
                    actor.scale*=68.89960f/82.55920f;
                }
                monsterPixelActorsV53.Add(MonsterPixelIdentityV53(actor.npc,actor.kind),actor);
                if(actor.contactRootCurves!=null)foreach(var curve in actor.contactRootCurves)
                    GameModel.RegisterHoverCurveV58(actor.npc,curve.motion,actor.rootOffsetY,curve.y);
                // The dedicated six-root owner retains its original native
                // fallback admission. Ordinary migrated bodies can defer their
                // legacy carrier pixels while keeping all source metadata.
                if(string.IsNullOrEmpty(actor.assembly))monsterPixelDeferredCarrierKindsV53.Add(actor.kind);
            }
        }
        bool DeferMonsterPixelCarrierV53(string kind){ReadMonsterPixelRosterV53();return monsterPixelDeferredCarrierKindsV53.Contains(kind??"");}
        void BeginMonsterPixelFrameV53(){ReadMonsterPixelRosterV53();UpdateMonsterStreamForecast();monsterPixelSelectionsV53.Clear();monsterPixelSourceHiddenV53.Clear();}
        bool MonsterPixelRegisteredV53(Enemy actor,out MonsterPixelActorV53 definition){ReadMonsterPixelRosterV53();definition=null;return actor!=null&&monsterPixelActorsV53.TryGetValue(MonsterPixelIdentityV53(actor.sourceNPC,actor.kind),out definition);}
        bool MonsterPixelOwnsActorV53(Enemy actor){return actor!=null&&monsterPixelSelectionsV53.ContainsKey(actor);}
        bool MonsterPixelOwnsRendererV53(Renderer renderer){return renderer&&monsterPixelRenderersV53.Contains(renderer);}

        void ObserveMonsterPixelV53(Enemy actor,string provider,string alias,string motion,float clock,string status,string reason,int frame=-1,int page=-1){
            if(actor==null)return;
            string room=game?.CurrentRoom?.id??"";
            string route=catalog?.dungeons!=null?Dungeon.id:"";
            int actorIndex=game?.enemies.IndexOf(actor)??-1;
            string key=route+"/"+room+"/"+MonsterPixelIdentityV53(actor.sourceNPC,actor.kind)+"/"+actorIndex+"/"+actor.npcState+"/"+actor.state+"/"+actor.active+"/"+actor.dormant+"/"+provider+"/"+alias+"/"+motion+"/"+status+"/"+reason;
            if(!monsterPixelObservationsV53.TryGetValue(key,out var row)){
                row=new MonsterPixelObservationV53{route=route,room=room,npc=actor.sourceNPC,kind=actor.kind,provider=provider,alias=alias,motion=motion,status=status,reason=reason};monsterPixelObservationsV53.Add(key,row);
            }
            var carrier=actorIndex>=0&&actorIndex<enemyViews.Count?enemyViews[actorIndex]:null;
            row.carrierExists=carrier;row.carrierEnabled=carrier&&carrier.enabled;row.carrierHasSprite=carrier&&carrier.sprite;row.carrierActiveInHierarchy=carrier&&carrier.gameObject.activeInHierarchy;
            row.state=actor.npcState;row.combatState=actor.state.ToString();row.actorIndex=actorIndex;row.active=actor.active;row.dormant=actor.dormant;row.sourceClock=clock;row.frame=frame;row.page=page;row.lastRenderFrame=Time.frameCount;row.samples++;
        }
        void RecordMonsterPixelSourceHiddenV53(Enemy actor,SpriteRenderer carrier,string provider){
            // Called only at the original source owner's explicit no-display
            // branch, after that owner disables its renderer and clears sprite.
            // A disabled carrier alone is insufficient: other rigs may replace
            // that carrier with visible parts, so no global visibility guess.
            if(!carrier||carrier.enabled||carrier.sprite)throw new InvalidOperationException("Original source hidden gate did not disable its actual carrier");
            monsterPixelSourceHiddenV53.Add(actor);ObserveMonsterPixelV53(actor,provider,actor.clip,"",actor.clock,"inactive","original-source-initialization-hidden");
        }
        MonsterPixelFramesV53 MonsterPixelLibraryV53(MonsterPixelActorV53 definition){
            string key=MonsterPixelIdentityV53(definition.npc,definition.kind);
            if(!monsterPixelLibrariesV53.TryGetValue(key,out var library)){
                if(!preparedMonsterBooks.TryGetValue(definition,out library))
                    library=new MonsterPixelFramesV53(definition,DestroyNativeWorldObject);
                else preparedMonsterBooks.Remove(definition);
                if(StreamMobileArt)library.takePreparedPage=TakeMonsterStreamPage;
                monsterPixelLibrariesV53.Add(key,library);
            }
            return library;
        }

        // Called by Pose after its real alias fallback and loop selection. No
        // inferred NPC-state clock is substituted for this exact display clock.
        void RecordMonsterPixelAliasV53(Enemy actor,SpriteRenderer carrier,string alias,float clock,AnimationData selected,string requestedAlias,bool idleFallback){
            if(actor==null)return;
            if(!actor.active||actor.dormant){ObserveMonsterPixelV53(actor,"Pose",alias,"",clock,"inactive","original-birth-visibility-gate");return;}
            if(actor.sourceNPC=="NUI_CHICKEN_FEMALE")return; // six native roots have their own exact source-clock owner
            if(idleFallback&&actor.sourceNPC!=AncientRigV35.Npc){ObserveMonsterPixelV53(actor,"Pose",requestedAlias,"",clock,"fallback","legacy-idle-fallback");return;}
            if(!MonsterPixelRegisteredV53(actor,out var definition)){ObserveMonsterPixelV53(actor,"Pose",alias,"",clock,"unmigrated","identity-not-in-roster");return;}
            try{
                var library=MonsterPixelLibraryV53(definition);
                if(actor.sourceNPC==AncientRigV35.Npc&&TryAncientPhysicalPose(actor,carrier,alias,library))return;
                if(OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out var hurt,out float sourceSeconds)&&library.TryClip(hurt.motion,out _)){
                    RecordMonsterPixelSourceV53(actor,carrier,"Pose/OriginalReactionV45",alias,hurt.motion,sourceSeconds);return;
                }
                // An unknown display alias was replaced with "idle" by the legacy carrier.
                // Resolve the actual Phoru state at the final ownership handoff instead.
                if(idleFallback)return;
                if(TryTraversalPixelSelectorV53(actor,carrier,alias,library)||TryExistingPixelSelectorV53(actor,carrier,alias,clock,selected,library)||TryEmptyIdlePixelSelectorV53(actor,carrier,alias,clock,selected,library))return;
                MonsterPixelAliasV53 binding=null;int specificity=-1;
                if(definition.aliases!=null)foreach(var candidate in definition.aliases){
                    if(candidate.alias!=alias||(!string.IsNullOrEmpty(candidate.route)&&!(Dungeon.id==candidate.route||game.CurrentRoom.id==candidate.route||game.CurrentRoom.id.StartsWith(candidate.route+"-",StringComparison.Ordinal))))continue;
                    int scope=string.IsNullOrEmpty(candidate.route)?0:1;if(scope<specificity)continue;
                    if(scope==specificity&&binding!=null&&(binding.motion!=candidate.motion||Math.Abs(binding.displayDuration-candidate.displayDuration)>.0001f||binding.hold!=candidate.hold))throw new InvalidOperationException("Ambiguous exact alias "+alias);
                    binding=candidate;specificity=scope;
                }
                if(binding==null){
                    // Guardian's final V33 selector below includes rear-hit
                    // variants and verifies the actual carrier frame first.
                    if(GuardianCombatNativeRulesV28.Identity(actor.kind,actor.sourceNPC))return;
                    // BendersFamilyRigV36.Frame has this exact final selector:
                    // a pinned state -> source motion/speed mapping, after visual
                    // aliases and physical reactions. Other identities do not
                    // inherit it unless their manifest explicitly registers it.
                    var state=definition.stateClockPolicy=="clamp-source-state"&&definition.states!=null?Array.Find(definition.states,s=>s.state==actor.npcState):null;
                    if(state!=null){
                        if(state.speed<=0||!MonsterPixelFramesV53.Finite(actor.npcTime)||actor.npcTime<0||!library.TryClip(state.motion,out var source)||Math.Abs(source.duration-state.sourceDuration)>.0002f)throw new InvalidOperationException("Invalid registered source-state clock "+actor.npcState);
                        RecordMonsterPixelSourceV53(actor,carrier,"Pose/registered-source-state",alias,state.motion,Mathf.Min(actor.npcTime*state.speed,source.duration));return;
                    }
                    ObserveMonsterPixelV53(actor,"Pose",alias,"",clock,"pending","exact-alias-binding-missing");return;
                }
                if(selected==null||selected.duration<=0||binding.displayDuration<=0||binding.sourceDuration<=0||Math.Abs(selected.duration-binding.displayDuration)>.0002f)throw new InvalidOperationException("Exact alias duration differs: "+alias);
                if(!library.TryClip(binding.motion,out var motion)||Math.Abs(motion.duration-binding.sourceDuration)>.0002f)throw new InvalidOperationException("Exact source duration differs: "+binding.motion);
                float phase=selected.loop?Mathf.Repeat(clock,selected.duration):Mathf.Min(clock,selected.duration);
                float sourceClock=binding.hold?0:phase*binding.sourceDuration/binding.displayDuration;
                RecordMonsterPixelSourceV53(actor,carrier,"Pose/manifest-alias",alias,binding.motion,sourceClock);
            }catch(Exception error){FailMonsterPixelV53(actor,"Pose",alias,"",clock,error);}
        }
        // Product/rig selectors call this with the source motion and seconds they
        // actually selected. The caller has already applied speed, looping, hold,
        // physical-reaction fallback and any source animation callback clock.
        bool RecordMonsterPixelSourceV53(Enemy actor,SpriteRenderer carrier,string provider,string alias,string motion,float sourceClock,int facing=0,float sourceRotationZ=0){
            if(actor==null)return false;
            if(!MonsterPixelRegisteredV53(actor,out var definition)){ObserveMonsterPixelV53(actor,provider,alias,motion,sourceClock,"unmigrated","identity-not-in-roster");return false;}
            if(actor.sourceNPC=="NUI_CHICKEN_FEMALE"){
                ObserveMonsterPixelV53(actor,provider,alias,motion,sourceClock,"pending","six-root-native-projection-adapter-required");return false;
            }
            try{
                var library=MonsterPixelLibraryV53(definition);
                if(!carrier)throw new InvalidOperationException("Source selector has no carrier");
                var sprite=library.At(motion,sourceClock,out int frame,out int page);
                monsterPixelSelectionsV53[actor]=new MonsterSelectionV53{actor=actor,carrier=carrier,definition=definition,library=library,sprite=sprite,provider=provider,alias=alias,motion=motion,clock=sourceClock,frame=frame,page=page,facing=facing,rotationZ=sourceRotationZ};
                ObserveMonsterPixelV53(actor,provider,alias,motion,sourceClock,"selected","",frame,page);return true;
            }catch(Exception error){FailMonsterPixelV53(actor,provider,alias,motion,sourceClock,error);return false;}
        }
        void FailMonsterPixelV53(Enemy actor,string provider,string alias,string motion,float clock,Exception error){
            ObserveMonsterPixelV53(actor,provider,alias,motion,clock,"fallback",error.Message);
            string key=MonsterPixelIdentityV53(actor.sourceNPC,actor.kind)+"/"+provider+"/"+error.Message;
            if(monsterPixelFailuresV53.Add(key))Debug.LogWarning("MONSTER_PIXEL_V53_PENDING "+key);
        }
        void DropMonsterPixelV53(Enemy actor){if(!monsterPixelViewsV53.TryGetValue(actor,out var view))return;monsterPixelRenderersV53.Remove(view.renderer);monsterPixelViewsV53.Remove(actor);view.Dispose();}
        // Run before legacy owners, so releasing a stale whole view cannot undo
        // the legacy owner's new forceRenderingOff assignment later in the frame.
        void PrepareMonsterPixelOwnersV53(){
            PrepareDrillerPixelSelectionsV53();
            PrepareFairyPixelSelectionsV53();
            PrepareAncientPixelPose();
            PreserveMonsterReactionPixels();
            monsterPixelStaleV53.Clear();foreach(var pair in monsterPixelViewsV53)if(!monsterPixelSelectionsV53.TryGetValue(pair.Key,out var selection)||selection.carrier!=pair.Value.carrier)monsterPixelStaleV53.Add(pair.Key);
            foreach(var actor in monsterPixelStaleV53)DropMonsterPixelV53(actor);monsterPixelStaleV53.Clear();
        }
        void UpdateMonsterPixelViewsV53(){
            if(game==null||!actorsRoot||!worldRoot){DisposeMonsterPixelViewsV53();return;}
            BeginFortPixelSortingV54();
            foreach(var pair in monsterPixelSelectionsV53){
                var selection=pair.Value;var actor=pair.Key;var carrier=selection.carrier;var definition=selection.definition;
                if(!carrier||!game.enemies.Contains(actor))continue;
                var view=RequireMonsterPixelViewV53(selection);int facing=MonsterPixelFacingV53(selection);
                var position=MonsterPixelLogicalRootV53(selection);
                var world=actorsRoot.TransformPoint(new Vector3(position.x,position.y,0));float eye=0;
                var projected=NativeWorldActive?ProjectNativePointWithEye(world,actor.nativeDepth,out eye):world;
                view.Apply(selection.sprite,projected,definition.scale*actor.visualScale,facing,actor.active&&!actor.dormant,selection.rotationZ);
                // Whole-model atlases leave two transparent pixels below the ink.
                if(selection.rotationZ==0)SeatGroundedReaction(actor,view.renderer,2);
                if(actor.sourceNPC==AncientRigV35.Npc)lastAncientPose=selection;
                // The throne stands far behind the playable plane. Keep its large
                // silhouette behind the four devices and the central exposed Plant.
                if(actor.sourceNPC=="NUI_NASOD_KING"&&game.CurrentRoom.id=="Dungeon_Altera_Core_Hard-4")
                    view.renderer.sortingOrder=carrier.sortingOrder-1;
                ApplyMonsterBurialV57(selection,view.renderer);
                ApplyFortPixelSortingV54(selection,view.renderer);
                if(NativeWorldActive)BindProjectedSceneOcclusion(view.renderer,world,actor.nativeDepth,eye);
                if(coreProductSessionR1147&&IsCoreAlarmWholeIdV21(actor.sourceNPC))AlignAlarmSocketV59(actor,view.renderer);
                int index=game.enemies.IndexOf(actor);
                if(index>=0&&index<enemyBars.Count&&actor.hp>0&&enemyBars[index]&&enemyBars[index].enabled){
                    float top=view.renderer.bounds.max.y;var p=enemyBars[index].transform.position;p.y=top+8;enemyBars[index].transform.position=p;
                    if(index<enemyDebuffViews.Count&&enemyDebuffViews[index]){p=enemyDebuffViews[index].transform.position;p.y=top+20;enemyDebuffViews[index].transform.position=p;}
                }
                ObserveMonsterPixelV53(actor,selection.provider,selection.alias,selection.motion,selection.clock,"rendered","",selection.frame,selection.page);
            }
            EndFortPixelSortingV54();
            if(regionalSession){
                // Regional combat sprites are cropped to the last opaque row.
                for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++)
                    SeatGroundedReaction(game.enemies[i],enemyViews[i],0);
            }
            RenderCoreAlarmLaserVisualR1417();
            foreach(var actor in game.enemies)if(actor.active&&!actor.dormant&&!monsterPixelSourceHiddenV53.Contains(actor)&&!monsterPixelSelectionsV53.ContainsKey(actor)&&!FemalePixelOwnsV53(actor))ObserveMonsterPixelV53(actor,"final-owner",actor.clip,"",actor.clock,MonsterPixelRegisteredV53(actor,out _)?"pending":"unmigrated","no-exact-source-selection-this-frame");
            if(Time.frameCount%120==0)TrimMonsterPixelMemoryV53(false);
        }

        void SeatGroundedReaction(Enemy actor,SpriteRenderer body,int bottomPadding){
            if(actor.hp<=0||!actor.active||actor.dormant||actor.isGate||!body||!body.enabled||body.forceRenderingOff||!body.sprite)return;
            string state=actor.npcState;
            bool recovery=actor.state==CombatState.Knockdown||actor.state==CombatState.GettingUp;
            if(!recovery&&(string.IsNullOrEmpty(state)||
                (state.IndexOf("_DAMAGE_DOWN",StringComparison.Ordinal)<0&&state.IndexOf("_STAND_UP",StringComparison.Ordinal)<0&&
                 state.IndexOf("_STANDUP",StringComparison.Ordinal)<0&&state.IndexOf("_DAMAGE_AIR_DOWN_LANDING",StringComparison.Ordinal)<0&&
                 state.IndexOf("_DAMAGE_AIR_FALL_LANDING",StringComparison.Ordinal)<0)))return;
            if(!game.Grounded(actor)||actor.vy>0)return;
            // Some source actors take off directly from a rising pose.
            if(!string.IsNullOrEmpty(state)&&state.EndsWith("_STANDUP_FLY",StringComparison.Ordinal))return;
            var point=RenderEnemyPosition(actor);
            var floor=game.Surface(actor.groundLine);
            point.y=floor!=null?floor.Y(Mathf.Clamp(point.x,floor.x1,floor.x2)):actor.y;
            var ground=actorsRoot.TransformPoint(point);
            if(NativeWorldActive)ground=ProjectNativePoint(ground,actor.nativeDepth);
            float bottom=body.bounds.min.y+bottomPadding*body.transform.lossyScale.y/body.sprite.pixelsPerUnit;
            if(bottom>ground.y)body.transform.position+=Vector3.down*(bottom-ground.y);
        }
        void TrimMonsterPixelMemoryV53(bool lowMemory){
            foreach(var library in monsterPixelLibrariesV53.Values){var protect=new HashSet<int>();foreach(var selected in monsterPixelSelectionsV53.Values)if(selected.library==library)protect.Add(selected.page);if(femalePixelOwnerV53!=null)for(int i=0;i<6;i++)if(femalePixelOwnerV53.libraries[i]==library)protect.Add(femalePixelOwnerV53.pages[i]);if(BossIntroductionActive&&library==monsterPixelPortraitLibraryV53)protect.Add(monsterPixelPortraitPageV53);foreach(var row in bossHudRows)if(row.owner!=null&&row.portraitLibrary==library)protect.Add(row.portraitPage);mobileRetiredBytes+=library.Trim(protect,lowMemory?0:StreamMobileArt?120:600);}
            RequestMobileArtSweep(lowMemory);
        }
        void DisposeMonsterPixelViewsV53(){
            ResetMonsterStream();
            ReleaseMonsterPixelRoom(false);
        }
        void ReleaseMonsterPixelRoom(bool carry){
            DisposeFemalePixelV53();
            lastAncientPose=null;monsterPixelPortraitLibraryV53=null;monsterPixelPortraitPageV53=-1;
            monsterPixelStartEnvelopesV53.Clear();monsterPixelSourceHiddenV53.Clear();
            foreach(var pair in monsterPixelViewsV53){
                var view=pair.Value;view.Dispose();
                // Unity retires room objects at the end of the frame. A camera can
                // render synchronously during room setup before that destruction.
                if(pair.Key.sourceNPC==AncientRigV35.Npc&&view.carrier)view.carrier.forceRenderingOff=true;
            }
            monsterPixelViewsV53.Clear();monsterPixelRenderersV53.Clear();monsterPixelSelectionsV53.Clear();
            DisposeFortPixelSortingV54();
            if(carry)CarryMonsterStreamLibraries();
            else{foreach(var library in monsterPixelLibrariesV53.Values){mobileRetiredBytes+=library.ResidentBytes;library.Dispose();}monsterPixelLibrariesV53.Clear();}
        }
        bool TryMonsterPixelPortraitV53(Enemy actor,out Sprite sprite){
            return TryMonsterPixelPortraitV53(actor,out sprite,out monsterPixelPortraitLibraryV53,out monsterPixelPortraitPageV53);
        }
        bool TryMonsterPixelPortraitV53(Enemy actor,out Sprite sprite,out MonsterPixelFramesV53 library,out int page){
            sprite=null;library=null;page=-1;if(!MonsterPixelRegisteredV53(actor,out var definition)||string.IsNullOrEmpty(definition.standingMotion))return false;
            try{library=MonsterPixelLibraryV53(definition);sprite=library.At(definition.standingMotion,0,out _,out page);return true;}
            catch(Exception error){FailMonsterPixelV53(actor,"DangerPortrait","",definition.standingMotion,0,error);return false;}
        }
        public MonsterPixelCoverageV53 ReadMonsterPixelCoverageV53(){
            ReadMonsterPixelRosterV53();var rows=new List<MonsterPixelObservationV53>(monsterPixelObservationsV53.Values);
            rows.Sort((a,b)=>StringComparer.Ordinal.Compare(a.room+"/"+a.npc+"/"+a.state+"/"+a.provider+"/"+a.alias,b.room+"/"+b.npc+"/"+b.state+"/"+b.provider+"/"+b.alias));
            var report=new MonsterPixelCoverageV53{rosterSha256=monsterPixelRosterShaV53,registeredIdentities=monsterPixelActorsV53.Count,loadedLibraries=monsterPixelLibrariesV53.Count,observations=rows.ToArray(),complete=false};
            foreach(var library in monsterPixelLibrariesV53.Values){report.residentPages+=library.ResidentPages;report.residentBytes+=library.ResidentBytes;}
            // Observed success alone cannot establish all-20-route/state coverage.
            // The inventory-driven offline checker must make that determination.
            return report;
        }
        public void WriteMonsterPixelCoverageV53(string path){if(string.IsNullOrEmpty(path))throw new ArgumentNullException("path");string folder=Path.GetDirectoryName(Path.GetFullPath(path));Directory.CreateDirectory(folder);File.WriteAllText(path,JsonUtility.ToJson(ReadMonsterPixelCoverageV53(),true));}
    }
}
