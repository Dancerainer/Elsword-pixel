using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        static bool IsPeitaInteriorWall(NativeWorldPart part) {
            // These shells place the route between two textured walls. Render the
            // inside face so the outside facing the camera cannot bury the lane.
            switch (part.name) {
                case "Peita_Tower_Heart_s1_Back.Y":
                case "Peita_Tower_Heart_s2_back.Y":
                case "Peita_Tower_Heart_s3_back.Y":
                    return part.texture.EndsWith("/peita_heart_syilinder_wall", StringComparison.Ordinal);
                case "Peita_Chapel_Stage2-1_topL.Y":
                case "Peita_Chapel_Stage2-3top.Y":
                    return part.texture.EndsWith("/peita_chapel_wall_down1", StringComparison.Ordinal)
                        || part.texture.EndsWith("/peita_chapel_wall_easy", StringComparison.Ordinal);
                default:
                    return false;
            }
        }

        [Serializable] sealed class DungeonRegionPack {
            public DungeonArt[] dungeons;
            public StageArt[] stages;
            [NonSerialized] public Dictionary<string, Library> actors;
            [NonSerialized] public PeitaBehaviorRow[] behaviors;
            [NonSerialized] public RegionActorMechanics[] mechanics;
        }

        [Serializable] sealed class RegionalMonsterArt {
            public SpriteLibraryData[] monsters;
        }

        static readonly string[] PackagedRegions = { "peita", "velder", "hamel" };
        readonly Dictionary<string, DungeonRegionPack> regionPacks = new Dictionary<string, DungeonRegionPack>();
        readonly Dictionary<string, Library> replacedActors = new Dictionary<string, Library>();
        DungeonArt[] mainDungeons, regionalDungeons;
        StageArt[] mainStages, regionalStages;
        string activeDungeonRegion;
        int dungeonBeforeRegion;
        readonly System.Random placementRandom = new System.Random();

        bool ChooseRegionalSpawn(SpawnArt spawn, out string kind, out string sourceNpc) {
            kind = spawn.kind;
            sourceNpc = spawn.sourceNPC;
            // An omitted rate in the original declaration means the placement is guaranteed.
            if (spawn.spawnChance > 0 && spawn.spawnChance < 100 && placementRandom.Next(100) >= spawn.spawnChance)
                return false;
            if (spawn.choices == null || spawn.choices.Length == 0) return true;
            int sum = 0;
            foreach (var choice in spawn.choices) {
                if (choice.weight < 0) throw new InvalidOperationException("Negative NPC choice weight");
                sum = checked(sum + choice.weight);
            }
            if (sum == 0) throw new InvalidOperationException("Empty NPC choice distribution");
            int draw = placementRandom.Next(sum);
            foreach (var choice in spawn.choices) {
                draw -= choice.weight;
                if (draw >= 0) continue;
                kind = choice.kind;
                sourceNpc = choice.sourceNPC;
                break;
            }
            return true;
        }
        bool regionalSession { get { return activeDungeonRegion != null; } }
        bool peitaSession { get { return activeDungeonRegion == "peita"; } }

        void AddRegionalMapRoutes() {
            if (IsRubenElderRelease) return;
            if (regionalStages == null) {
                mainDungeons = catalog.dungeons;
                mainStages = catalog.stages;
                var routes = new List<DungeonArt>(mainDungeons);
                var stages = new List<StageArt>(mainStages);
                foreach (string region in PackagedRegions) {
                    var asset = Resources.Load<TextAsset>("Pixel/" + region + "/catalog");
                    if (!asset) throw new InvalidOperationException("Missing dungeon catalog for " + region);
                    var pack = JsonUtility.FromJson<DungeonRegionPack>(asset.text);
                    int offset = stages.Count;
                    foreach (var route in pack.dungeons) {
                        if (route.region != region || route.start < 0 || route.count < 1 ||
                            route.start + route.count > pack.stages.Length)
                            throw new InvalidOperationException("Invalid room range for " + route.id);
                        route.start += offset;
                        routes.Add(route);
                    }
                    stages.AddRange(pack.stages);
                    regionPacks.Add(region, pack);
                }
                regionalStages = stages.ToArray();
                regionalDungeons = routes.ToArray();
                if (musicInitialized) RegisterRegionalMusic();
            }

            for (int index = mainDungeons.Length; index < regionalDungeons.Length; index++) {
                var route = regionalDungeons[index];
                var rooms = new CandidateRoomArt[route.count];
                int zones = 0;
                for (int room = 0; room < rooms.Length; room++) {
                    var stage = regionalStages[route.start + room];
                    int count = Math.Max(1, stage.encounters?.Length ?? 0);
                    zones += count;
                    rooms[room] = new CandidateRoomArt {
                        id = stage.sourceWorldId, width = stage.width, zones = count,
                        spawnCount = stage.spawns?.Length ?? 0, bgm = stage.bgm,
                        bgmAvailable = !string.IsNullOrEmpty(stage.bgm)
                    };
                }
                mapDungeons.Add(new CandidateDungeonArt {
                    id = route.id, region = route.region, title = route.title, sourceGraph = route.sourceGraph,
                    description = route.description, count = rooms.Length, combatZones = zones,
                    runtimeIndex = index, rooms = rooms
                });
            }
        }

        void RegisterRegionalMusic() {
            foreach (var entry in regionPacks)
                foreach (var stage in entry.Value.stages) {
                    string track = stage.bgm;
                    if (!string.IsNullOrEmpty(track) && !sceneMusicAssets.ContainsKey(track))
                        sceneMusicAssets.Add(track, new SceneMusicAsset { id = track, resource = "Pixel/" + entry.Key + "/music/" + track });
                }
        }

        void AddRegionalPostcards() {
            foreach (var entry in regionPacks)
                foreach (var route in entry.Value.dungeons) {
                    var sprite = Picture("Pixel/" + entry.Key + "/postcards/" + route.id);
                    if (!sprite) sprite = Picture(regionalStages[route.start].background);
                    dungeonPostcards[route.id] = new DungeonPostcard { id = route.id, boss = route.boss, sprite = sprite };
                }
        }

        void BindRegionalActors(string region, DungeonRegionPack pack) {
            // Region variants may use the same kind as a mainline or neighboring monster.
            // Keep atlas frames lazy and restore original owners when returning to selection.
            if (pack.actors == null) {
                var index = Resources.Load<TextAsset>("Pixel/" + region + "/monsters");
                var behavior = Resources.Load<TextAsset>("Pixel/" + region + "/behavior");
                if (!index || !behavior) throw new InvalidOperationException("Missing monster definitions for " + region);
                pack.behaviors = JsonUtility.FromJson<PeitaBehaviorDocument>(behavior.text).rows;
                if (region != "peita") {
                    var mechanisms = Resources.Load<TextAsset>("Pixel/" + region + "/mechanics");
                    if (!mechanisms) throw new InvalidOperationException("Missing monster mechanisms for " + region);
                    pack.mechanics = JsonUtility.FromJson<RegionMechanicsDocument>(mechanisms.text).rows;
                }
                var actors = new Dictionary<string, Library>();
                foreach (var actor in JsonUtility.FromJson<RegionalMonsterArt>(index.text).monsters) {
                    libraries.TryGetValue(actor.id, out var original);
                    libraries.Remove(actor.id);
                    try {
                        MakeLibrary(actor, true);
                        actors.Add(actor.id, libraries[actor.id]);
                    } finally {
                        if (original != null) libraries[actor.id] = original;
                        else libraries.Remove(actor.id);
                    }
                }
                pack.actors = actors;
            }
            foreach (var actor in pack.actors) {
                libraries.TryGetValue(actor.Key, out var original);
                replacedActors.Add(actor.Key, original);
                libraries[actor.Key] = actor.Value;
            }
            game.SetPeitaEnemyBehaviors(pack.behaviors);
            if (pack.mechanics != null) game.SetRegionNpcMechanics(pack.mechanics);
            LoadRegionalEffects(region);
        }

        bool TryEnterRegionalDungeon() {
            if (IsRubenElderRelease) return false;
            if (!regionPacks.TryGetValue(mapRegion, out var pack) || mapSelected < 0 || mapSelected >= mapDungeons.Count) return false;
            var row = mapDungeons[mapSelected];
            if (row.region != mapRegion || row.runtimeIndex < mainDungeons.Length ||
                row.runtimeIndex >= regionalDungeons.Length || regionalDungeons[row.runtimeIndex].id != row.id) return false;

            dungeonBeforeRegion = selectedDungeon;
            activeDungeonRegion = row.region;
            catalog.dungeons = regionalDungeons;
            catalog.stages = regionalStages;
            try {
#if UNITY_EDITOR
                // The postcard camera draws the native scenery alone and has no monster layer.
                if (!capturingRegionalScenery)
#endif
                    BindRegionalActors(row.region, pack);
                while (stagesRoot.childCount < catalog.stages.Length)
                    CreateStageLayers(stagesRoot.childCount, false);
                StartSelectedCatalogDungeon();
            } catch {
                EndRegionalDungeon();
                throw;
            }
            return true;
        }

        void EndRegionalDungeon() {
            CancelMobileRoomWarmup();
            activeDungeonRegion = null;
            game.RegionalVisualCue=null;
            game.RegionSpellContactArt=null;
            regionCasts.Clear();
            regionalFx.Clear();
            foreach (var actor in replacedActors) {
                if (actor.Value == null) libraries.Remove(actor.Key);
                else libraries[actor.Key] = actor.Value;
            }
            replacedActors.Clear();
            catalog.dungeons = mainDungeons;
            catalog.stages = mainStages;
            game.SetPeitaEnemyBehaviors(null);
            selectedDungeon = previewDungeon = dungeonBeforeRegion;
            adventureStarted = false;
            shownRoom = previousRoom = cameraFollowRoom = -1;
            accumulator = 0;
            ConfigureRooms();
            RebuildRoom();
            ResetAudioTracking();
        }
    }
}
