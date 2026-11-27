#if UNITY_EDITOR || ROOT_REDRAW_CHECK_V32
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        bool routeCheckOwnsInput;
        [Serializable] sealed class PeitaRouteResult {
            public string id, problem;
            public int expectedRooms, hits, kills, soundsPlayed, soundsMissing, linkerEscapes;
            public float seconds, health;
            public bool victory, returned, roomGallery;
            public List<string> visited = new List<string>();
            public List<string> terrainEvents = new List<string>();
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        
        static void StartPeitaCheck() {
            var args = Environment.GetCommandLineArgs();
            int flag = Array.IndexOf(args, "--check-region-routes");
            if (flag < 0) flag = Array.IndexOf(args, "--check-peita");
            bool heroCheck = flag < 0;
            if (heroCheck) flag = Array.IndexOf(args, "--check-hero-slashes");
            if (flag < 0 || flag + 1 >= args.Length) return;
            var scene = UnityEngine.Object.FindFirstObjectByType<DemoScene>();
            Application.runInBackground = true;
            scene.routeCheckOwnsInput=!heroCheck;
            string path = Path.GetFullPath(args[flag + 1]);
            scene.StartCoroutine(GuardPeitaCheck(heroCheck ? scene.CheckHeroSlashes(path) : scene.CheckPeitaRoutes(path)));
        }

        IEnumerator CheckHeroSlashes(string output) {
            Directory.CreateDirectory(output);
            yield return null;
            foreach (int facing in new[] { 1, -1 }) {
                MenuAction("dungeons");
                MenuAction("region:peita");
                MenuAction("dungeon-card:0");
                MenuAction("dungeon-enter");
                yield return null;
                if (!NativeWorldActive || !game.EquipSkill(0, "double-slash"))
                    throw new InvalidOperationException("Could not equip Double Slash in the Feita entrance.");
                var target = game.enemies.Find(e => e.kind == "gliter_lance" && e.active && !e.dormant);
                if (target == null) throw new InvalidOperationException("Missing entrance lancer");
                // Start on opposite sides of the same placed monster for a repeatable camera comparison.
                game.player.x = target.x - facing * 30;
                game.player.y = target.y;
                game.player.groundLine = target.groundLine;
                game.player.facing = facing;
                game.mana = 300;
                game.roomBanner = 0;
                cameraFollowRoom = -1;
                string side = facing > 0 ? "right" : "left";
                float[] clocks = { .40f, .52f, 2.14f, 2.25f, 2.85f };
                string[] labels = { "first-contact", "first-arc", "second-contact", "fire", "follow-through" };
                string sequence = Path.Combine(output, "sequence");
                if (facing > 0) Directory.CreateDirectory(sequence);
                int sample = 0, hits = game.hits;
                for (int tick = 0; tick < 360 && sample < clocks.Length; tick++) {
                    RestoreNativeProjection();
                    RememberSimulationState();
                    if (tick > 0) game.Step(1f / 60, new Input { skill = tick == 1 ? 0 : -1 });
                    accumulator = 1f / 60;
                    DrawCombatFrame();
                    if (facing > 0 && tick % 5 == 0)
                        CaptureOffscreenFrame(Path.Combine(sequence, (tick / 5).ToString("D3") + ".png"));
                    if (tick == 0) CaptureOffscreenFrame(Path.Combine(output, side + "-idle.png"));
                    if (game.ActiveSkillId == "double-slash" && RenderPlayerClock >= clocks[sample]) {
                        CaptureOffscreenFrame(Path.Combine(output, side + "-" + labels[sample] + ".png"));
                        sample++;
                    }
                }
                if (sample != clocks.Length || game.hits <= hits || !heroView.sprite)
                    throw new InvalidOperationException(side + " sample count " + sample + ", connected hits " + (game.hits - hits));
                Debug.Log("HERO_SLASH_ROOM " + side + " connected hits=" + (game.hits - hits));
                accumulator = 0;
                MenuAction("dungeons");
            }
            ExitPeitaCheck(0);
        }

        static IEnumerator GuardPeitaCheck(IEnumerator check) {
            while (true) {
                try { if (!check.MoveNext()) break; }
                catch (Exception e) { Debug.LogException(e); ExitPeitaCheck(3); break; }
                yield return check.Current;
            }
        }

        static void ExitPeitaCheck(int code) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(code);
#else
            Application.Quit(code);
#endif
        }

#if UNITY_EDITOR
        public static void RunPeitaEditorCheck() {
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenes[0].path);
            UnityEditor.EditorApplication.isPlaying = true;
        }
#endif

        IEnumerator CheckPeitaRoutes(string output) {
            Directory.CreateDirectory(output);
            yield return null;
            var args = Environment.GetCommandLineArgs();
            int flag = Array.IndexOf(args, "--region");
            string region = flag >= 0 && flag + 1 < args.Length ? args[flag + 1] : "peita";
            if (!regionPacks.TryGetValue(region, out var pack))
                throw new InvalidOperationException("Unregistered dungeon region " + region);
            if (flag >= 0 && (mapRegion != region || openMenu != "dungeons" || adventureStarted)) {
                Debug.LogError("Launcher failed to open the " + region + " map");
                ExitPeitaCheck(2);
                yield break;
            }
            MenuAction("dungeons");
            MenuAction("region:" + region);
            if (mapVisible.Count != pack.dungeons.Length || mapVisible.Count == 0) {
                Debug.LogError("Unexpected dungeon card count for " + region + ": " + mapVisible.Count);
                ExitPeitaCheck(2);
                yield break;
            }
            yield return null;
            CaptureOffscreenFrame(Path.Combine(output, "menu.png"));
            bool passed = true;
            bool gallery = Array.IndexOf(Environment.GetCommandLineArgs(), "--peita-gallery") >= 0;
            int routeFlag=Array.IndexOf(args,"--only-route");
            string onlyRoute=routeFlag>=0&&routeFlag+1<args.Length?args[routeFlag+1]:null;
            if(onlyRoute!=null&&!Array.Exists(pack.dungeons,d=>d.id==onlyRoute))
                throw new InvalidOperationException("Unknown route "+onlyRoute);
            var enterRoom = typeof(GameModel).GetMethod("EnterRoom",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            for (int slot = 0; slot < pack.dungeons.Length; slot++) {
                if(onlyRoute!=null&&pack.dungeons[slot].id!=onlyRoute)continue;
                MenuAction("region:" + region);
                for (int page = 0; page < slot / MapPageSize; page++) MenuAction("dungeon-page:1");
                MenuAction("dungeon-card:" + slot % MapPageSize);
                if (slot > 0 && slot % MapPageSize == 0) {
                    yield return null;
                    CaptureOffscreenFrame(Path.Combine(output, "menu-page-" + (1 + slot / MapPageSize) + ".png"));
                }
                if (mapDungeons[mapSelected].id != pack.dungeons[slot].id)
                    throw new InvalidOperationException("Wrong selected card at " + slot);
                var selected = mapDungeons[mapSelected];
                int soundsBefore = NpcAudioPlayed, missingBefore = NpcAudioMissing;
                var result = new PeitaRouteResult {id = selected.id, expectedRooms = selected.count, roomGallery = gallery};
                MenuAction("dungeon-enter");
                if (Dungeon.id != selected.id || !string.IsNullOrEmpty(openMenu))
                    result.problem = "Menu failed to enter selected route";
                int lastRoom = -1, steps = 0, idleSteps = 0, previousHits = -1, previousEncounter = -1;
                smokeNextAttack = 0;
                while (string.IsNullOrEmpty(result.problem) && !game.victory) {
                    if (game.room != lastRoom) {
                        lastRoom = game.room;
                        yield return null;
                        if (!NativeWorldActive || nativeScene.id != CurrentStageArt.sourceWorldId || !nativeRoot || nativeRoot.childCount == 0) {
                            result.problem = "Missing native scenery in " + CurrentStageArt.sourceWorldId;
                            break;
                        }
                        result.visited.Add(nativeScene.id);
                        CaptureOffscreenFrame(Path.Combine(output, nativeScene.id + ".png"));
                        idleSteps = 0;
                        if (gallery) {
                            if (lastRoom + 1 == result.expectedRooms) break;
                            enterRoom.Invoke(game, new object[] {lastRoom + 1});
                            RestoreNativeProjection();
                            RebuildRoom();
                            continue;
                        }
                    }
                    if (BossIntroductionActive) { yield return null; continue; }
                    // Accelerate ordinary 60 Hz combat inputs. The actual scene renders
                    // every transition. No player, enemy or encounter state is injected.
                    int encounter = game.EncounterIndex;
                    for (int tick = 0; tick < 90 && game.room == lastRoom && game.EncounterIndex == encounter && !game.victory; tick++) {
                        var input = DungeonCombatInput();
                        // The three potion keys are also available while navigating.
                        input.potion |= game.player.hp<300;
                        if(region=="peita") {
                            if(game.mana<90)input.potionPressedMask |= game.potionCooldowns[1]<=0 ? 2 : 1;
                        } else if(!input.potion || game.potionCooldowns[2]>0) {
                            if(game.mana<=200 && game.potionCooldowns[1]<=0)input.potionPressedMask|=2;
                            else if(game.mana<=250 && game.potionCooldowns[0]<=0)input.potionPressedMask|=1;
                        }
                        bool operatingBridge=region=="hamel" && OperateRegionalBridge(game,input);
                        if (!operatingBridge && idleSteps > 240 && region != "peita")
                            RecoverRegionalRouteInput(game,input,idleSteps);
                        else if (region=="peita" && idleSteps > 360 && !game.gateOpen) TryPeitaLedgeAttack(input, idleSteps);
                        bool shaking=game.RegionShakeRequired;
                        if(shaking)
                            input=new Input {move=steps/6%2==0?-1:1,skill=-1,potion=input.potion,potionPressedMask=input.potionPressedMask};
                        game.Step(1f / 60, input);
                        if(shaking&&!game.RegionShakeRequired)result.linkerEscapes++;
                        if(region!="peita" && steps%6==0) {
                            ObserveRegionalCombat();
                            ObserveRegionalTerrain(result);
                        }
                        steps++;
                        idleSteps++;
                        if (game.player.hp <= 0 || game.paused) break;
                    }
                    // DemoScene normally does this immediately after its final Step,
                    // before the next frame has a chance to begin a boss introduction.
                    if (shownRoom != game.room) {
                        RestoreNativeProjection();
                        RebuildRoom();
                    }
                    RememberSimulationState();
                    if(region!="peita" && !gallery && !game.victory) CaptureRegionalBattle(output);
                    if (game.hits != previousHits || game.EncounterIndex != previousEncounter) {
                        previousHits = game.hits;
                        previousEncounter = game.EncounterIndex;
                        idleSteps = 0;
                    }
                    if (game.player.hp <= 0 || game.paused || idleSteps > 60 * 75 || steps > 60 * 2700) {
                        var enemies = new List<string>();
                        var inputOwners=new List<string>();
                        foreach (var e in game.enemies)
                        {
                            if(e.active&&e.hp>0&&e.regionActor?.capturesInput==true)
                                inputOwners.Add(e.kind+" "+e.npcState);
                            if (e.active && !e.dormant && e.hp > 0 && e.encounterIndex == game.EncounterIndex && (e.countsForClear || e.isGate))
                                enemies.Add(e.sourceNPC + "@" + e.x.ToString("F0") + "," + e.y.ToString("F0") +
                                    " hp=" + e.hp.ToString("F0") + " state=" + e.npcState + " on=" + e.groundLine);
                        }
                        result.problem = "Stopped in room " + game.room + " zone " + game.EncounterIndex +
                            " player=" + game.player.x.ToString("F0") + "," + game.player.y.ToString("F0") +
                            " on="+game.player.groundLine+" inputOwners="+string.Join(",",inputOwners)+
                            " gateOpen=" + game.gateOpen + " remaining=" + string.Join(" ", enemies);
                    }
                    yield return null;
                }
                result.hits = game.hits;
                result.kills = game.kills;
                result.seconds = game.elapsed;
                result.health = game.player.hp;
                result.victory = game.victory;
                result.soundsPlayed = NpcAudioPlayed - soundsBefore;
                result.soundsMissing = NpcAudioMissing - missingBefore;
                MenuAction("dungeons");
                result.returned = openMenu == "dungeons" && !regionalSession &&
                    ReferenceEquals(catalog.dungeons, mainDungeons) && ReferenceEquals(catalog.stages, mainStages);
                passed &= (gallery || result.victory) && result.returned && result.visited.Count == result.expectedRooms &&
                    result.soundsMissing == 0;
                File.WriteAllText(Path.Combine(output, result.id + ".json"), JsonUtility.ToJson(result, true));
                Debug.Log("PEITA_ROUTE_CHECK " + JsonUtility.ToJson(result));
                yield return null;
            }
            MenuAction("region:ruben");
            MenuAction("dungeon-card:0");
            MenuAction("dungeon-enter");
            passed &= Dungeon.id == "tree" && string.IsNullOrEmpty(openMenu);
            MenuAction("dungeons");
            MenuAction("region:" + region);
            MenuAction("dungeon-card:0");
            MenuAction("dungeon-enter");
            passed &= activeDungeonRegion == region && Dungeon.id == pack.dungeons[0].id;
            MenuAction("dungeons");
            if (!gallery && region!="peita") passed &= SaveRegionalObservations(output);
            Debug.Log("PEITA_MENU_RETURN_CHECK passed=" + passed);
            ExitPeitaCheck(passed ? 0 : 2);
        }

        void ObserveRegionalTerrain(PeitaRouteResult result) {
            string room=game.CurrentRoom.id;
            if(game.VelderPalaceAlarm)RecordTerrain(result,room+" magenta alarm");
            if(game.VelderChaseCamera(out _,out _))RecordTerrain(result,room+" chase camera");
            if(!room.StartsWith("hamel-",StringComparison.Ordinal))return;
            var foot=game.SurfaceBelow(game.player.x,game.player.y+.1f);
            if(game.IsUnderWater&&foot?.water!=null)
                RecordTerrain(result,room+" water line "+foot.id+" buoyancy "+foot.water.buoyancy.ToString("F2",System.Globalization.CultureInfo.InvariantCulture));
            foreach(var surface in game.CurrentRoom.surfaces) {
                if(surface.hamelTrigger!=null && game.HamelTriggerAge(surface.hamelTrigger.id)>=0)
                    RecordTerrain(result,room+" floor trigger "+surface.hamelTrigger.id);
            }
        }
        static void RecordTerrain(PeitaRouteResult result,string tag) {
            if(!result.terrainEvents.Contains(tag))result.terrainEvents.Add(tag);
        }

        static bool OperateRegionalBridge(GameModel game,Input input) {
            foreach(var floor in game.CurrentRoom.surfaces) {
                var trigger=floor.hamelTrigger;
                if(trigger==null||trigger.onLeave||trigger.events==null||floor.hamelDisabled)continue;
                foreach(var evt in trigger.events) {
                    if(evt.line<0||!evt.enable||game.Surface(evt.line)?.hamelDisabled!=true)continue;
                    float age=game.HamelTriggerAge(trigger.id);
                    if(age>=evt.at)continue;
                    float x=(floor.x1+floor.x2)*.5f,dx=x-game.player.x;
                    input.skill=-1;input.attack=input.attackX=input.drop=false;
                    input.dash=input.jump=false;
                    if(age>=0){input.move=0;return true;}
                    var current=game.SurfaceBelow(game.player.x,game.player.y+.1f);
                    if(current==null||x<game.CurrentEncounter.leftX||x>game.CurrentEncounter.rightX)continue;
                    input.move=Math.Abs(dx)>5?Math.Sign(dx):0;
                    if(floor.Y(x)>game.player.y+8)game.NativeNavigate(input,x,floor.Y(x));
                    return true;
                }
            }
            return false;
        }

        static void RecoverRegionalRouteInput(GameModel game, Input input, int idleSteps) {
            var hero=game.player;
            var support=game.SurfaceBelow(hero.x,hero.y+.1f);
            if (support==null) return;
            var choices=new List<Enemy>();
            bool dolls=game.enemies.Exists(e=>e.active&&e.hp>0&&e.npcState=="CRAZY_PUPPET_SPECIAL_ATTACK_A");
            foreach(var e in game.enemies) {
                if(!e.active||e.dormant||e.passive||e.hp<=0||e.encounterIndex!=game.EncounterIndex)continue;
                if(dolls) {
                    if(e.kind!="crazy_puppet_small_false"&&e.kind!="crazy_puppet_small_true")continue;
                } else if(e.isGate ? !game.RoomCombatClear||e.invincible>0 : !e.countsForClear)continue;
                choices.Add(e);
            }
            choices.Sort((a,b)=>RouteDistance(a,hero).CompareTo(RouteDistance(b,hero)));
            int phase=(idleSteps-241)/240;
            var target=choices.Count>0?choices[phase%Math.Min(choices.Count,3)]:null;
            if(target==null && !game.gateOpen)return;
            float x=target==null?game.CurrentEncounter.exitX:target.x;
            float y=target==null?game.GroundAt(game.CurrentRoom,x):target.y;
            float dx=x-hero.x,dy=y-hero.y;
            if(game.gateOpen&&x>=support.x1-1&&x<=support.x2+1) {
                input.move=Math.Abs(dx)>.05f?Math.Sign(dx):game.CurrentEncounter.direction;
                input.dash=true;input.drop=input.attack=input.attackX=false;input.skill=-1;
                input.jump=Math.Abs(dx)>16&&game.Grounded(hero)&&idleSteps%90<4;
                return;
            }
            float gapToLip=Math.Abs((dx<0?support.x1:support.x2)-hero.x);
            var targetFloor=game.SurfaceBelow(x,y+.1f);
            if(target!=null && target.groundLine<0 && Math.Abs(dx)<115 &&
               y-support.Y(hero.x)>25 && y-support.Y(hero.x)<160 &&
               (targetFloor==null || targetFloor.Y(x)<support.Y(hero.x)+28)) {
                bool grounded=game.Grounded(hero);
                input.move=Math.Abs(dx)>22 || dx*hero.facing<0 ? Math.Sign(dx) : 0;
                input.dash=false;input.drop=false;input.skill=-1;
                input.jump=grounded;
                input.attack=!grounded && Math.Abs(dy)<50 && idleSteps%12==0;
                input.attackX=!grounded && Math.Abs(dy)<65 && idleSteps%12==6;
                return;
            }
            if(targetFloor!=null&&targetFloor.Y(x)-support.Y(hero.x)>165&&game.NativeNavigate(input,x,y))
                return;
            if(target!=null && targetFloor!=null && targetFloor!=support && Math.Abs(dx)<145 &&
               targetFloor.Y(x)>support.Y(hero.x)+25 && dy> -15 && (phase%2==0||!game.Grounded(hero))) {
                // Keep steering onto the upper lip even after the enemy enters
                // ordinary horizontal attack range during the jump.
                float inset=Math.Min(12,(targetFloor.x2-targetFloor.x1)*.4f);
                float landing=Math.Max(targetFloor.x1+inset,Math.Min(targetFloor.x2-inset,x));
                input.move=Math.Abs(landing-hero.x)>3?Math.Sign(landing-hero.x):0;
                input.dash=Math.Abs(landing-hero.x)>90||targetFloor.Y(x)-support.Y(hero.x)>78;
                input.jump=game.Grounded(hero);
                input.drop=false;input.skill=-1;
                input.attack=!input.jump && Math.Abs(dx)<85 && Math.Abs(dy)<35 && idleSteps%10==0;
                return;
            }
            bool lowerObstruction=dy< -42&&dy> -145&&Math.Abs(dx)>55&&Math.Abs(dx)<190&&gapToLip<120;
            if(target!=null && Math.Abs(dx)<220 && dy<110 && (dy> -42 || lowerObstruction)) {
                input.move=Math.Abs(dx)>30||dx*hero.facing<0?Math.Sign(dx):0;
                input.jump=(dy>42||lowerObstruction)&&game.Grounded(hero);input.drop=false;
                input.dash=Math.Abs(dx)>85;input.attack=Math.Abs(dx)<160&&idleSteps%12==0;
                input.attackX=Math.Abs(dx)<160&&idleSteps%24==6;
                if(game.CanAwaken){input.awaken=true;return;}
                // A wide boss can block walking before its origin enters
                // sword range. Use the equipped travelling and area attacks.
                if(!game.Busy()) {
                    foreach(var preferred in new[]{"sonic-blade","flame-geyser","windmill","rolling-smash","mega-slash","armor-break","kick"})
                    for(int slot=0;slot<GameModel.SkillSlotCount;slot++){
                        var skill=game.GetEquippedSkill(slot);
                        if(skill==null||skill.id!=preferred||game.cooldowns[slot]>0||game.mana<skill.mpCost)continue;
                        input.skill=slot;input.attack=input.attackX=false;return;
                    }
                }
                return;
            }
            input.attack=input.attackX=input.awaken=false;input.skill=-1;
            if(dy< -29 && Math.Abs(dx)<165 && game.Grounded(hero)) {
                var below=game.SurfaceBelow(hero.x,hero.y-1);
                if(support.platform && below!=null && below.Y(hero.x)>=y-36) {
                    input.move=0;input.jump=false;input.drop=true;return;
                }
                // Walk off the nearer lip that actually has a lower landing.
                int side=0;float best=float.PositiveInfinity;
                foreach(int direction in new[]{-1,1}) {
                    float edge=direction<0?support.x1:support.x2;
                    float landingX=edge+direction*9;
                    var lower=game.SurfaceBelow(landingX,hero.y-4);
                    if(lower==null||lower.npcOnly||lower.bungeeTargetPlusOne>0)continue;
                    float score=Math.Abs(edge-hero.x)+.35f*Math.Abs(landingX-x);
                    if(score<best){best=score;side=direction;}
                }
                if(side!=0){input.move=side;input.jump=input.dash=input.drop=false;return;}
            }
            int toward=Math.Sign(dx);
            input.move=toward;input.dash=true;
            bool navigates=game.NativeNavigate(input,x,y);
            if(toward==0)return;
            if((!navigates || idleSteps%240<150) && dy>15) {
                float lip=toward>0?support.x2:support.x1;
                if((lip-hero.x)*toward<105 || (Math.Abs(dx)<100 && dy<180)) {
                    input.move=toward;input.dash=true;input.drop=false;
                    input.jump=game.Grounded(hero);
                }
            }
        }
        static float RouteDistance(Enemy enemy,Fighter hero) {
            float x=enemy.x-hero.x,y=enemy.y-hero.y;
            return x*x+y*y;
        }

        // A short ledge can leave an archer in striking range above the
        // player's head while its feet still belong to a different support.
        // Try the normal dash-jump and aerial attack buttons in that case.
        void TryPeitaLedgeAttack(Input input, int idleSteps) {
            Enemy closest = null;
            float distance = float.MaxValue;
            foreach (var enemy in game.enemies) {
                if (!enemy.active || enemy.dormant || enemy.passive || enemy.hp <= 0 ||
                    enemy.encounterIndex != game.EncounterIndex || !enemy.countsForClear) continue;
                float dx = Math.Abs(enemy.x - game.player.x);
                float dy = enemy.y - game.player.y;
                var gate = game.UnbrokenExitGate;
                bool beyondGate = gate != null && gate.invincible > 0 && dx < 140 &&
                    Math.Abs(gate.x - game.player.x) < 45 && Math.Abs(dy) < 20 &&
                    (game.player.x - gate.x) * (enemy.x - gate.x) < 0;
                if ((beyondGate || dx < 125 && dy > 8 && dy < 110) && dx + dy < distance) {
                    closest = enemy;
                    distance = dx + dy;
                }
            }
            if (closest == null) return;
            input.move = Math.Sign(closest.x - game.player.x);
            input.dash = true;
            if (game.Grounded(game.player)) input.jump = true;
            else if (idleSteps % 10 == 0) input.attack = true;
        }

    }
}
#endif
