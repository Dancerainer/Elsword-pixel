using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GameModel {
        internal RoomDefinition MonsterArtRoom(int index) {
            return index >= 0 && index < rooms.Count ? rooms[index] : null;
        }
    }

    public sealed partial class DemoScene {
        const int MonsterBookLimit = 10;
        const long MonsterPageBudget = 64L * 1024 * 1024;
        const long MonsterCarryBudget = 64L * 1024 * 1024;
        static readonly string[] WarmMotions = { null, null, "Walk", "DamageSmall", null, "AttackA_Ready", "AttackA" };
        sealed class MonsterTarget {
            internal string kind, regionPage, walkMotion;
            internal MonsterPixelActorV53 actor;
        }
        sealed class MonsterPage {
            internal string kind;
            internal Texture2D texture;
            internal long bytes;
        }
        readonly Dictionary<MonsterPixelActorV53,MonsterPixelFramesV53> preparedMonsterBooks = new Dictionary<MonsterPixelActorV53,MonsterPixelFramesV53>();
        readonly Dictionary<string,MonsterPage> preparedMonsterPages = new Dictionary<string,MonsterPage>(StringComparer.Ordinal);
        long preparedMonsterBytes;
        RoomDefinition forecastRoom;
        int forecastEncounter = -1, monsterStreamGeneration;
        Coroutine monsterStreamRoutine;
        List<MonsterTarget> forecastTargets;
        bool monsterPagesConsumed;
        int nextMonsterRefillFrame;

        void AddMonsterTarget(List<MonsterTarget> targets,string npc,string kind,string region=null) {
            if(string.IsNullOrEmpty(kind)||targets.Count>=MonsterBookLimit)return;
            foreach(var row in targets)if(row.kind==kind)return;
            monsterPixelActorsV53.TryGetValue(MonsterPixelIdentityV53(npc,kind),out var actor);
            string page=null;
            if(actor==null){
                if(region=="peita"||region=="velder"||region=="hamel")page="Pixel/"+region+"/actors/"+kind;
                else if(libraries.TryGetValue(kind,out var lib)){
                    var path=lib.data.texture;
                    if(path!=null&&(path.StartsWith("Pixel/peita/actors/",StringComparison.Ordinal)||
                        path.StartsWith("Pixel/velder/actors/",StringComparison.Ordinal)||
                        path.StartsWith("Pixel/hamel/actors/",StringComparison.Ordinal)))page=path;
                }
                if(page==null)return;
            }
            string walk=null;
            if(actor!=null&&actor.aliases!=null)foreach(var alias in actor.aliases){
                if(alias.alias!="walk"&&alias.alias!="source-walk")continue;
                if(!string.IsNullOrEmpty(alias.route)&&region==null&&adventureStarted&&alias.route!=Dungeon.id)continue;
                walk=alias.motion;break;
            }
            targets.Add(new MonsterTarget{kind=kind,actor=actor,regionPage=page,walkMotion=walk});
        }

        void AddSpawnTargets(List<MonsterTarget> targets,RoomDefinition room,int wave) {
            if(room==null)return;
            foreach(var spawn in room.enemies)if(spawn.encounterIndex==wave)AddMonsterTarget(targets,spawn.sourceNPC,spawn.kind);
        }

        IEnumerator WarmEntryMonsters(CandidateDungeonArt row) {
            if(!StreamMobileArt)yield break;
            StageArt stage=null;string region=null;
            if(row.runtimeIndex>=0&&row.runtimeIndex<catalog.dungeons.Length&&catalog.dungeons[row.runtimeIndex].id==row.id){
                var route=catalog.dungeons[row.runtimeIndex];
                if(route.start>=0&&route.start<catalog.stages.Length)stage=catalog.stages[route.start];
            }else if(regionPacks.TryGetValue(row.region??"",out var pack)){
                var route=Array.Find(pack.dungeons,d=>d.id==row.id);
                if(route!=null&&route.start>=0&&route.start<pack.stages.Length){stage=pack.stages[route.start];region=row.region;}
            }
            if(stage==null||stage.spawns==null)yield break;
            ResetMonsterStream();
            ReadMonsterPixelRosterV53();
            var targets=new List<MonsterTarget>();
            foreach(var spawn in stage.spawns)if(spawn.encounterIndex==stage.entryEncounter){
                AddMonsterTarget(targets,spawn.sourceNPC,spawn.kind,region);
                if(spawn.choices!=null)foreach(var choice in spawn.choices)AddMonsterTarget(targets,choice.sourceNPC,choice.kind,region);
            }
            var work=PrepareMonsterTargets(targets,monsterStreamGeneration);
            try{while(!mobileCancelEntry&&work.MoveNext())yield return work.Current;}
            finally{(work as IDisposable)?.Dispose();}
        }

        void UpdateMonsterStreamForecast() {
            if(!StreamMobileArt||!adventureStarted||!string.IsNullOrEmpty(openMenu)||game==null||game.RoomCount==0)return;
            var room=game.CurrentRoom;int wave=game.EncounterIndex;
            if(ReferenceEquals(room,forecastRoom)&&wave==forecastEncounter){
                if(monsterPagesConsumed&&monsterStreamRoutine==null&&forecastTargets!=null&&
                    preparedMonsterBytes<MonsterPageBudget&&Time.frameCount>=nextMonsterRefillFrame)
                    StartMonsterWorker(forecastTargets);
                return;
            }
            forecastRoom=room;forecastEncounter=wave;
            var targets=new List<MonsterTarget>();
            // Schedule the source transition destinations first. Same-room waves and
            // cross-room branches are both identified by their original encounter indices.
            var encounter=game.CurrentEncounter;
            if(encounter!=null&&encounter.clauses!=null)foreach(var clause in encounter.clauses){
                if(clause.transitions==null)continue;
                foreach(var edge in clause.transitions)if(!edge.completeDungeon)
                    AddSpawnTargets(targets,game.MonsterArtRoom(edge.targetRoom),edge.targetEncounter);
            }
            if(targets.Count==0){var next=game.MonsterArtRoom(game.room+1);if(next!=null)AddSpawnTargets(targets,next,next.entryEncounter);}
            AddSpawnTargets(targets,room,wave);
            StopMonsterWorker();
            PruneMonsterForecast(targets);
            forecastTargets=targets;
            if(targets.Count>0)StartMonsterWorker(targets);
        }

        void StartMonsterWorker(List<MonsterTarget> targets) {
            monsterPagesConsumed=false;
            nextMonsterRefillFrame=Time.frameCount+16;
            monsterStreamRoutine=StartCoroutine(RunMonsterWorker(targets,monsterStreamGeneration));
        }

        IEnumerator RunMonsterWorker(List<MonsterTarget> targets,int generation) {
            var work=PrepareMonsterTargets(targets,generation);
            try{while(work.MoveNext())yield return work.Current;}
            finally{
                (work as IDisposable)?.Dispose();
                if(generation==monsterStreamGeneration)monsterStreamRoutine=null;
            }
        }

        bool TryMonsterBook(MonsterPixelActorV53 actor,out MonsterPixelFramesV53 book) {
            return preparedMonsterBooks.TryGetValue(actor,out book)||
                monsterPixelLibrariesV53.TryGetValue(MonsterPixelIdentityV53(actor.npc,actor.kind),out book);
        }

        IEnumerator PrepareMonsterTargets(List<MonsterTarget> targets,int generation) {
            yield return null;
            // ResourceRequests can integrate on the main thread. Keep at most one
            // request in flight; only the plain JSON data and clip indexes go to a worker.
            foreach(var target in targets){
                if(generation!=monsterStreamGeneration)yield break;
                var actor=target.actor;if(actor==null||TryMonsterBook(actor,out _))continue;
                var request=Resources.LoadAsync<TextAsset>(actor.root+"/library");request.priority=-1;
                yield return request;
                if(generation!=monsterStreamGeneration)yield break;
                var asset=request.asset as TextAsset;
                if(!asset||TryMonsterBook(actor,out _))continue;
                string json=asset.text;
                string failure=null;
                var build=Task.Run(()=>{
                    try{
                        var document=JsonUtility.FromJson<TreeBendersRasterData>(json);
                        if(document==null)throw new InvalidOperationException("Empty monster manifest "+actor.root);
                        return new MonsterPixelFramesV53(actor,DestroyNativeWorldObject,document);
                    }catch(Exception error){failure=error.Message;return null;}
                });
                while(!build.IsCompleted)yield return null;
                if(generation!=monsterStreamGeneration)yield break;
                if(failure!=null){Debug.LogWarning("Monster art prefetch: "+failure);continue;}
                if(!TryMonsterBook(actor,out _))preparedMonsterBooks.Add(actor,build.Result);
            }
            var selected=new HashSet<string>(StringComparer.Ordinal);
            long selectedBytes=0;
            for(int tier=0;tier<WarmMotions.Length;tier++)foreach(var target in targets){
                if(generation!=monsterStreamGeneration||selectedBytes>=MonsterPageBudget)yield break;
                string path=target.regionPage;
                MonsterPixelFramesV53 source=null;
                int page=-1;
                if(target.actor!=null){
                    if(!TryMonsterBook(target.actor,out source))continue;
                    string motion=WarmMotions[tier]??target.actor.standingMotion;
                    // William and several machine bosses enter pursuit using their source
                    // Dash alias on the very first tick. The atlas also contains a distinct Walk.
                    if(tier==0&&target.walkMotion=="Dash")motion=target.walkMotion;
                    if(tier==2&&!string.IsNullOrEmpty(target.walkMotion))motion=target.walkMotion;
                    if(!source.TryClip(motion,out var clip))continue;
                    // Frame zero is often shared with another animation on another page.
                    // The first simulated tick is already on the sustained pose page.
                    int index=tier==4?0:Math.Min(1,clip.frames.Length-1);
                    page=clip.frames[index].page;
                    if(source.PageResident(page))continue;
                    path=source.PagePath(page);
                }else if(tier!=0||atlasTextures.ContainsKey(path))continue;
                if(!selected.Add(path))continue;
                if(preparedMonsterPages.TryGetValue(path,out var existing)){selectedBytes+=existing.bytes;continue;}
                var request=Resources.LoadAsync<Texture2D>(path);request.priority=-1;yield return request;
                if(generation!=monsterStreamGeneration)yield break;
                var texture=request.asset as Texture2D;
                if(!texture||(source!=null&&source.PageResident(page))||(target.actor==null&&atlasTextures.ContainsKey(path)))continue;
                long bytes=(long)texture.width*texture.height*(texture.isReadable?8:4);
                if(bytes>MonsterPageBudget-selectedBytes)continue;
                // A newly imminent arrival takes precedence over the previous
                // encounter's unconsumed optional motion.
                if(bytes>MonsterPageBudget-preparedMonsterBytes){
                    foreach(var stale in new List<string>(preparedMonsterPages.Keys))if(!selected.Contains(stale)){
                        mobileRetiredBytes+=preparedMonsterPages[stale].bytes;RemoveMonsterStreamPage(stale);
                        if(bytes<=MonsterPageBudget-preparedMonsterBytes)break;
                    }
                }
                if(bytes>MonsterPageBudget-preparedMonsterBytes)continue;
                preparedMonsterPages.Add(path,new MonsterPage{kind=target.kind,texture=texture,bytes=bytes});
                preparedMonsterBytes+=bytes;selectedBytes+=bytes;
                yield return null;
            }
        }

        Texture2D TakeMonsterStreamPage(string path) {
            var texture=RemoveMonsterStreamPage(path);
            if(texture)monsterPagesConsumed=true;
            return texture;
        }

        Texture2D RemoveMonsterStreamPage(string path) {
            if(!preparedMonsterPages.TryGetValue(path,out var page))return null;
            preparedMonsterPages.Remove(path);preparedMonsterBytes-=page.bytes;
            return page.texture;
        }

        void StopMonsterWorker() {
            monsterStreamGeneration++;
            if(monsterStreamRoutine!=null)StopCoroutine(monsterStreamRoutine);
            monsterStreamRoutine=null;
        }

        void PruneMonsterForecast(List<MonsterTarget> targets) {
            var kinds=new HashSet<string>(StringComparer.Ordinal);
            foreach(var row in targets)kinds.Add(row.kind);
            foreach(var actor in new List<MonsterPixelActorV53>(preparedMonsterBooks.Keys))if(!kinds.Contains(actor.kind))preparedMonsterBooks.Remove(actor);
            foreach(var path in new List<string>(preparedMonsterPages.Keys)){
                var page=preparedMonsterPages[path];if(kinds.Contains(page.kind))continue;
                mobileRetiredBytes+=page.bytes;RemoveMonsterStreamPage(path);
            }
        }

        void CarryMonsterStreamLibraries() {
            if(game==null||game.RoomCount==0){ResetMonsterStream();}
            var needed=new HashSet<string>(StringComparer.Ordinal);
            if(game!=null&&game.RoomCount>0)foreach(var spawn in game.CurrentRoom.enemies)needed.Add(spawn.kind);
            long carried=0;var empty=new HashSet<int>();
            foreach(var key in new List<string>(monsterPixelLibrariesV53.Keys)){
                var lib=monsterPixelLibrariesV53[key];long bytes=lib.ResidentBytes;
                if(needed.Contains(lib.actor.kind)){
                    if(carried+bytes<=MonsterCarryBudget)carried+=bytes;
                    else mobileRetiredBytes+=lib.Trim(empty,0);
                    continue;
                }
                mobileRetiredBytes+=bytes;lib.Dispose();monsterPixelLibrariesV53.Remove(key);
            }
        }

        void ResetMonsterStream() {
            StopMonsterWorker();forecastRoom=null;forecastEncounter=-1;forecastTargets=null;monsterPagesConsumed=false;
            preparedMonsterBooks.Clear();preparedMonsterPages.Clear();mobileRetiredBytes+=preparedMonsterBytes;preparedMonsterBytes=0;
            if(StreamMobileArt)RequestMobileArtSweep(false);
        }
    }
}