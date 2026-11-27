using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcSummonSpawn {
        public string npc;public float range,delay;public int direction;
    }
    [Serializable] public sealed class NpcSummonRule {
        public string state,sourceCallback;public float emitAt;
        public string[] decisionStates=new string[0],requireAbsentTypes=new string[0];
        public NpcSummonSpawn[] spawns=new NpcSummonSpawn[0];
    }
    [Serializable] public sealed class NpcSummonEvent {
        public string kind,npc,owner;public float time,x,y;public int line;
    }
    public sealed partial class GameModel {
        sealed class NpcSummonRequest {
            internal Enemy owner;internal EnemySpawn template;internal string npc;
            internal int room,encounter,line,facing;internal float x,y,due;
            internal bool alreadyEmitted;internal bool belowManagerOwnedV20;
        }
        readonly List<NpcSummonRequest> npcSummonRequests=new List<NpcSummonRequest>();
        public readonly List<NpcSummonEvent> npcSummonEvents=new List<NpcSummonEvent>();
        uint npcSummonRandom=0x71bc492u;
        public int PendingNpcSummons {get{return npcSummonRequests.Count;}}
        static void ValidateNpcSummonRule(NpcDefinition profile,Dictionary<string,NpcDefinition> profiles){
            var rule=profile.summon;if(rule==null)return;
            var state=Array.Find(profile.states,s=>s.id==rule.state);
            if(state==null||rule.decisionStates==null||rule.decisionStates.Length==0||rule.requireAbsentTypes==null||rule.requireAbsentTypes.Length==0||rule.spawns==null||rule.spawns.Length==0||string.IsNullOrEmpty(rule.sourceCallback)||!Finite(rule.emitAt)||rule.emitAt<=0||rule.emitAt>state.duration)throw new ArgumentException("Invalid source NPC summon binding");
            foreach(string id in rule.decisionStates)if(id!=profile.waitState&&id!=profile.walkState)throw new ArgumentException("Summon condition must bind a source decision state");
            foreach(string id in rule.requireAbsentTypes)if(!profiles.ContainsKey(id))throw new ArgumentException("Missing source summon count identity");
            foreach(var spawn in rule.spawns)if(spawn==null||!profiles.ContainsKey(spawn.npc)||!Finite(spawn.range)||spawn.range<=0||!Finite(spawn.delay)||spawn.delay<0||Math.Abs(spawn.direction)>1)throw new ArgumentException("Invalid source summon request");
        }
        void ClearNpcSummons(){ClearFactoryRavenClonePhaseRoomV21();ClearPlainPairRoomR1258();ClearPlainMineOriginalServicesR1269();ClearBelowTunnelVisibilityV21();ClearBelowTunnelRoomV20();ClearBelowFortRoomV20();ClearBelowFxRoomV20();ClearBelowBombRoomV20();ClearBelowMiniUpWaitRoomV20();ClearMine2SourceRoomV20();ClearMine2MechanismsRoomV21();ClearMine2PresentationV21();ClearDragonNestRoomV20();ClearAlteraPopulationActorsV18();npcSummonRequests.Clear();npcSummonEvents.Clear();npcSummonRandom=0x71bc492u;ClearNpcPassageActors();ClearNpcSourceSupportActorsV17();ClearNpcLakeSourceActorsV18();ClearAlteraRepairmanRoomV18();ClearAlteraAirshipIntroRoomV18();}
        void SummonEvent(string kind,Enemy owner,string npc,float x,float y,int line){
            if(npcSummonEvents.Count>=256)npcSummonEvents.RemoveAt(0);
            npcSummonEvents.Add(new NpcSummonEvent{kind=kind,owner=owner.sourceNPC,npc=npc,time=elapsed,x=x,y=y,line=line});
        }
        EnemySpawn SummonTemplate(string npc){return CurrentRoom.enemies.Find(s=>s.sourceNPC==npc&&s.hp>0);}
        bool TryBeginNpcSummon(Enemy enemy,NpcDefinition profile,NpcStateDefinition state){
            var rule=profile.summon;if(rule==null||Array.IndexOf(rule.decisionStates,state.id)<0||!HasNpcMechanisms||enemy.hp<=0)return false;
            // LiveNPCNumType counts the two initial non-active thieves too.
            // The callback does not name their instance IDs or activate them.
            foreach(var other in enemies)if(other.hp>0&&Array.IndexOf(rule.requireAbsentTypes,other.sourceNPC)>=0)return false;
            foreach(var spawn in rule.spawns)if(SummonTemplate(spawn.npc)==null)return false;
            if(!TryEnterNpcState(enemy,npcStates[profile.id+"/"+rule.state]))return false;
            // Source RIGHT=TRUE; summon animation faces right independently of
            // the current nearest-player direction.
            enemy.facing=1;SummonEvent("begin",enemy,"",enemy.x,enemy.y,enemy.groundLine);return true;
        }
        float SummonRandom(){npcSummonRandom^=npcSummonRandom<<13;npcSummonRandom^=npcSummonRandom>>17;npcSummonRandom^=npcSummonRandom<<5;return(npcSummonRandom%1000000)/1000000f;}
        bool SummonGroundPoint(Enemy owner,NpcSummonSpawn request,out float x,out float y,out int line){
            x=owner.x;y=owner.y;line=-1;
            // Source requests random positions around the current boss, not
            // predefined new START_POS entries. Sample only real ground spans
            // in that radius and the active encounter. Opposite direction
            // booleans are represented as opposite sides (demo polarity).
            if(!NativeTerrain)return false;
            var intervals=new List<float[]>();var lines=new List<GroundSurface>();float length=0;
            foreach(var surface in CurrentRoom.surfaces){
                float lo=Math.Max(surface.x1+.03f,owner.x-request.range),hi=Math.Min(surface.x2-.03f,owner.x+request.range);
                if(request.direction>0)lo=Math.Max(lo,owner.x);else if(request.direction<0)hi=Math.Min(hi,owner.x);
                if(HasEncounters){lo=Math.Max(lo,CurrentEncounter.leftX+.03f);hi=Math.Min(hi,CurrentEncounter.rightX-.03f);}
                if(hi<=lo||Math.Abs(surface.Y((lo+hi)*.5f)-owner.y)>82)continue;
                intervals.Add(new[]{lo,hi});lines.Add(surface);length+=hi-lo;
            }
            if(length<=0){
                // A wall can remove one directional interval completely. The
                // playable boundary adapter still uses real ground inside the
                // original radius; it never invents an off-map fixed point or
                // silently drops one of the three source create requests.
                if(request.direction!=0){SummonEvent("source-side-clipped",owner,request.npc,owner.x,owner.y,owner.groundLine);return SummonGroundPoint(owner,new NpcSummonSpawn{npc=request.npc,range=request.range,delay=request.delay,direction=0},out x,out y,out line);}
                return false;
            }
            float distance=SummonRandom()*length;
            for(int i=0;i<intervals.Count;i++){
                var span=intervals[i];float width=span[1]-span[0];
                if(distance<=width||i==intervals.Count-1){x=span[0]+Math.Min(distance,width);y=lines[i].Y(x);line=lines[i].id;return true;}
                distance-=width;
            }
            return false;
        }
        void AdvanceNpcSummon(Enemy owner,NpcDefinition profile,NpcStateDefinition state,float from,float to,float packetStart){
            var rule=profile.summon;if(rule==null||state.id!=rule.state||!(from<rule.emitAt&&rule.emitAt<=to))return;
            float at=packetStart+rule.emitAt-from;
            foreach(var spawn in rule.spawns){
                var template=SummonTemplate(spawn.npc);float x,y;int line;
                if(template==null||!SummonGroundPoint(owner,spawn,out x,out y,out line)){SummonEvent("no-source-ground",owner,spawn.npc,owner.x,owner.y,-1);continue;}
                npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=template,npc=spawn.npc,room=room,encounter=owner.encounterIndex,line=line,x=x,y=y,due=at+spawn.delay});
                SummonEvent("request",owner,spawn.npc,x,y,line);SpawnEffect("cast",x,y+20);
            }
        }
        void FlushNpcSummonRequests(){FlushPlainPairRequestsR1258();FlushBelowFortRequestsV20();
            // Called after the actor foreach, so adding source-created NPCs
            // cannot invalidate the simulation's current enumerator. Requests
            // survive the summon animation's end but not death/room reset.
            for(int i=0;i<npcSummonRequests.Count;){
                var r=npcSummonRequests[i];
                if(r.room!=room||!enemies.Contains(r.owner)&&!r.belowManagerOwnedV20||r.owner.hp<=0&&!r.alreadyEmitted||victory){npcSummonRequests.RemoveAt(i);continue;}
                if(r.due>elapsed){i++;continue;}
                var p=npcDefinitions[r.npc];var t=r.template;float hp=t.hp;
                var created=new Enemy{sourceNPC=r.npc,kind=p.kind,x=r.x,spawnX=r.x,y=r.y,groundLine=r.line,facing=r.facing!=0?r.facing:player.x>=r.x?1:-1,
                    hp=hp,maxHp=hp,aggroRange=p.senseRange,visualScale=t.visualScale,nativeDepth=t.nativeDepth,encounterIndex=r.encounter,
                    active=true,aiEnabled=true,countsForClear=true,passive=false,dormant=false,state=CombatState.Normal};enemies.Add(created);InitializeNpcBirthStateV26(created);OnDragonSummonedV20(created);
                SummonEvent("spawn",r.owner,r.npc,r.x,r.y,r.line);npcSummonRequests.RemoveAt(i);
            }
        }
    }
}
