#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ElswordPixel {
    // Controlled model probes use the same menu-loaded content and room setup as play.
    public sealed class DungeonMechanicsReviewV58 : MonoBehaviour {
        [Serializable] public class Birth {
            public string route,room,npc,kind,state,clip;
            public int index,encounter;
            public bool active,dormant,passive,gate,countsForClear;
            public float x,y;
        }
        [Serializable] public class Probe {
            public string route,room,kind,npc,afterHit,atHit;
            public int index,encounter,damage;
            public bool armorAtHit;
            public float yAtSpawn,yAtHit;
            public string[] states,summons;
        }
        [Serializable] public class StateRule {
            public string route,npc,id,motion,next;
            public bool armored,neverMove,suppressArmorOutline;
            public int armorWindows;
            public NpcSourceArmorWindow[] armorWindowTimes;
        }
        [Serializable] public class Summon {
            public string route,room,probeNpc,npc,kind,firstState,atHit,afterHit;
            public int encounter,damage;
            public bool hitTested;
        }
        public string output;
        public void Write(string file,object row) {
            File.AppendAllText(Path.Combine(output,file+".jsonl"),JsonUtility.ToJson(row)+Environment.NewLine);
        }
        public static void Run() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            EditorApplication.EnterPlaymode();
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        static void Attach() {
            string[] args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--mechanics-v58");
            if(i<0)return;
            var review=new GameObject("Dungeon mechanics review").AddComponent<DungeonMechanicsReviewV58>();
            review.output=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(review.output);
            Application.runInBackground=true;
        }
        IEnumerator Start() {
            DemoScene scene;
            while(!(scene=FindFirstObjectByType<DemoScene>()))yield return null;
            var args=Environment.GetCommandLineArgs();
            var work=scene.ReviewDungeonMechanicsV58(this,args[Array.IndexOf(args,"--routes-v58")+1].Split(','));
            bool success=false;
            while(true) {
                object item;
                try {if(!work.MoveNext()){success=true;break;}item=work.Current;}
                catch(Exception e){File.WriteAllText(Path.Combine(output,"failure.txt"),e.ToString());Debug.LogException(e);break;}
                yield return item;
            }
            File.WriteAllText(Path.Combine(output,"finished.txt"),success?"completed":"failed");
            EditorApplication.Exit(success?0:1);
        }
    }
    public sealed partial class DemoScene {
        internal IEnumerator ReviewDungeonMechanicsV58(DungeonMechanicsReviewV58 review,string[] routes) {
            yield return null;
            foreach(var id in routes) {
                Action("dungeons");yield return null;
                int index=mapDungeons.FindIndex(d=>d.id==id);
                if(index<0)throw new InvalidOperationException("Missing route: "+id);
                MenuAction("region:"+mapDungeons[index].region);RefreshMapVisible();
                int slot=mapVisible.IndexOf(index);
                if(slot<0)throw new InvalidOperationException("Route hidden: "+id);
                mapPage=slot/MapPageSize;MenuAction("dungeon-card:"+slot%MapPageSize);MenuAction("dungeon-enter");
                double end=Time.realtimeSinceStartupAsDouble+120;
                while(!adventureStarted||openMenu!=""||Dungeon.id!=id||forestPreviewLoading) {
                    if(Time.realtimeSinceStartupAsDouble>end)throw new TimeoutException(id);yield return null;
                }
                bool usesSceneServices=id=="altera-plain-recycle"||id=="altera-plain"||id=="altera-nasod-factory"||id=="altera-core-hard";
                var rooms=game.ReviewAllRoomsV58(id,review,usesSceneServices?(Action)RebuildRoom:null);
                while(rooms.MoveNext())yield return rooms.Current;
                RebuildRoom();
                File.AppendAllText(Path.Combine(review.output,"completed-routes.txt"),id+Environment.NewLine);
                yield return null;
            }
        }
    }
    public sealed partial class GameModel {
        void PrepareReviewRoomV58(int ri,int encounter) {
            victory=false;paused=false;player.hp=player.maxHp;player.invincible=1000;
            EnterRoom(ri);
            if(HasEncounters && EncounterIndex!=encounter)BeginEncounter(encounter);
        }
        internal IEnumerator ReviewAllRoomsV58(string route,DungeonMechanicsReviewV58 output,Action bindScene) {
            var recorded=new HashSet<NpcDefinition>();
            for(int ri=0;ri<RoomCount;ri++) {
                PrepareReviewRoomV58(ri,0);
                int count=EncounterCount;
                for(int ei=0;ei<count;ei++) {
                    PrepareReviewRoomV58(ri,ei);
                    var targets=new List<int>();var identities=new HashSet<string>();
                    for(int i=0;i<enemies.Count;i++) {
                        var e=enemies[i];if(e.encounterIndex!=ei)continue;
                        output.Write("births",new DungeonMechanicsReviewV58.Birth{route=route,room=CurrentRoom.id,index=i,encounter=ei,npc=e.sourceNPC,kind=e.kind,state=e.npcState,clip=e.clip,x=e.x,y=e.y,active=e.active,dormant=e.dormant,passive=e.passive,gate=e.isGate,countsForClear=e.countsForClear});
                        bool observe=e.active&&!e.isGate&&(!e.passive||e.sourceNPC=="NUI_CHICKEN_FEMALE");
                        if(observe&&identities.Add(e.kind+"/"+e.sourceNPC+"/"+e.dormant+"/"+e.aiEnabled))targets.Add(i);
                        var profile=NpcProfile(e);
                        if(profile!=null&&recorded.Add(profile))WriteReviewProfileV58(route,profile,output);
                    }
                    foreach(int index in targets) {
                        PrepareReviewRoomV58(ri,ei);var e=enemies[index];
                        if(bindScene!=null){bindScene();yield return null;}
                        var row=new DungeonMechanicsReviewV58.Probe{route=route,room=CurrentRoom.id,encounter=ei,index=index,npc=e.sourceNPC,kind=e.kind,yAtSpawn=e.y};
                        var states=new HashSet<string>();var summoned=new HashSet<string>();var initial=new HashSet<Enemy>(enemies);
                        var children=new Dictionary<Enemy,DungeonMechanicsReviewV58.Summon>();
                        var input=new Input();
                        bool summonOwner=e.kind=="mouse"||e.kind=="lizardman-shaman-king"||e.kind=="parasite-house"||e.kind=="nasod-maker"||e.kind=="raven-clone";
                        int ticks=e.kind=="lizardman-shaman-king"?2100:summonOwner?1200:330;
                        for(int tick=0;tick<ticks && room==ri && EncounterIndex==ei && !victory;tick++) {
                            player.x=Math.Max(22,Math.Min(CurrentRoom.width-22,e.x+e.facing*64));
                            player.y=e.y;player.invincible=1000;
                            Step(1f/60,input);
                            states.Add(string.IsNullOrEmpty(e.npcState)?e.state+"/"+e.clip:e.npcState);
                            if(tick==210) {
                                row.atHit=e.npcState;row.armorAtHit=EnemyHasSuperArmor(e);row.yAtHit=e.y;
                                row.damage=ApplyHitToEnemy(e,1,HitReaction.Launch); row.afterHit=e.state+"/"+e.npcState;
                            }
                            if(tick==340&&e.hp>0&&(e.kind=="mouse"||e.kind=="lizardman-shaman-king"||e.kind=="raven-clone")) {
                                float hpFraction=e.kind=="mouse"?0:e.kind=="raven-clone"?.69f:.39f;
                                ApplyHitToEnemy(e,Math.Max(1,(int)Math.Ceiling(e.hp-e.maxHp*hpFraction)),HitReaction.Hurt);
                            }
                            foreach(var child in enemies) {
                                if(initial.Contains(child))continue;
                                summoned.Add(child.sourceNPC??child.kind);
                                if(!children.TryGetValue(child,out var observed)){
                                    observed=new DungeonMechanicsReviewV58.Summon{route=route,room=CurrentRoom.id,encounter=ei,probeNpc=e.sourceNPC,npc=child.sourceNPC,kind=child.kind,firstState=child.npcState};
                                    children.Add(child,observed);
                                }else if((!observed.hitTested||observed.damage==0&&tick%30==0)&&child.hp>0&&child.active&&!child.dormant&&!string.IsNullOrEmpty(child.npcState)){
                                    observed.hitTested=true;observed.atHit=child.npcState;
                                    observed.damage=ApplyHitToEnemy(child,1,HitReaction.Hurt);observed.afterHit=child.state+"/"+child.npcState;
                                }
                            }
                            if(float.IsNaN(e.x)||float.IsInfinity(e.x)||float.IsNaN(e.y)||float.IsInfinity(e.y))throw new InvalidOperationException("Nonfinite actor "+e.kind);
                        }
                        row.states=states.OrderBy(x=>x).ToArray();row.summons=summoned.OrderBy(x=>x).ToArray();output.Write("probes",row);
                        foreach(var child in children.Values)output.Write("summons",child);
                    }
                }
            }
            foreach(var profile in npcDefinitions.Values)if(recorded.Add(profile))WriteReviewProfileV58(route,profile,output);
            PrepareReviewRoomV58(0,0);
        }
        static void WriteReviewProfileV58(string route,NpcDefinition profile,DungeonMechanicsReviewV58 output) {
            foreach(var state in profile.states)output.Write("rules",new DungeonMechanicsReviewV58.StateRule{route=route,npc=profile.id,id=state.id,motion=state.sourceAnimation,next=state.nextState,armored=state.superArmor,armorWindows=state.superArmorWindows==null?0:state.superArmorWindows.Length,armorWindowTimes=state.superArmorWindows,neverMove=state.neverMove,suppressArmorOutline=state.suppressArmorOutline});
        }
    }
}
#endif
