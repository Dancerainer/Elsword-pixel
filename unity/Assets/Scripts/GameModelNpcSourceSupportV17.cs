using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcSourceHealerRuleV17 {
        public string npc,state,sourceAnimation,originBone,sourceCallback,sourceInstructionsSha256;
        public float sourceAnimSpeed=1,emitAt,sourceAnimationTime,originX,originY,originZ,range,healFraction;
        public bool excludeSelf,requireSameTeam,requireAlive,excludeFullHp,rangeIsExclusive;
    }
    [Serializable] public sealed class NpcSourceRailHouseRuleV17 {
        public string npc,spawnNpc,initialState,restState,readyState,spawnState,damageState,deathState,sourceCallback,sourceInstructionsSha256;
        public float waitSeconds,emitAt,sourceAnimationTime,ownerHpCostFraction;public int sourcePopulationSlot;
        public bool oneActorPerCrossing,sameOriginalPosition,inheritFacing;
    }
    [Serializable] public sealed class NpcSourceSupportDocumentV17 {
        public bool runtimeAdmitted,nativeExecuted;public string schema,sourceRulesSha256;
        public NpcSourceHealerRuleV17[] healers=new NpcSourceHealerRuleV17[0];
        public NpcSourceRailHouseRuleV17[] railHouses=new NpcSourceRailHouseRuleV17[0];
    }
    [Serializable] public sealed class NpcSourceSupportEventV17 {
        public string kind,npc,target;public float time,amount,x,y;
    }
    public sealed partial class GameModel {
        readonly Dictionary<string,NpcSourceHealerRuleV17> npcSourceHealersV17=new Dictionary<string,NpcSourceHealerRuleV17>(StringComparer.Ordinal);
        NpcSourceHealerRuleV17 npcStandardHealerBladerR1419;
        readonly Dictionary<string,NpcSourceRailHouseRuleV17> npcSourceRailHousesV17=new Dictionary<string,NpcSourceRailHouseRuleV17>(StringComparer.Ordinal);
        sealed class RailHouseActorV17 {internal float age,x,y;internal int line,facing,initialPopulation;internal bool emitted;}
        readonly Dictionary<Enemy,RailHouseActorV17> npcRailHouseActorsV17=new Dictionary<Enemy,RailHouseActorV17>();
        public readonly List<NpcSourceSupportEventV17> npcSourceSupportEventsV17=new List<NpcSourceSupportEventV17>();
        static bool SourceSupportHashV17(string hash){if(string.IsNullOrEmpty(hash)||hash.Length!=64)return false;foreach(char c in hash)if(!Uri.IsHexDigit(c))return false;return true;}
        public void SetNpcSourceSupportV17(NpcSourceSupportDocumentV17 document){
            // JsonUtility can materialize an omitted nested DTO as a complete
            // empty instance. Only that full default signature means absent.
            if(document!=null&&string.IsNullOrEmpty(document.schema)&&string.IsNullOrEmpty(document.sourceRulesSha256)&&
                !document.runtimeAdmitted&&!document.nativeExecuted&&NoNpcRows(document.healers)&&NoNpcRows(document.railHouses))document=null;
            var healers=new Dictionary<string,NpcSourceHealerRuleV17>(StringComparer.Ordinal);var houses=new Dictionary<string,NpcSourceRailHouseRuleV17>(StringComparer.Ordinal);
            if(document!=null){
                if(document.schema!="bethma-support-source-v17"||document.healers==null||document.railHouses==null)throw new ArgumentException("Invalid source support document.");
                foreach(var rule in document.healers){
                    NpcStateDefinition state;
                    if(rule==null||string.IsNullOrEmpty(rule.npc)||healers.ContainsKey(rule.npc)||!npcStates.TryGetValue(rule.npc+"/"+rule.state,out state)||
                        rule.sourceAnimation!=state.sourceAnimation||Math.Abs(rule.sourceAnimSpeed-state.sourceAnimSpeed)>.00001f||
                        !Finite(rule.emitAt)||rule.emitAt<=0||rule.emitAt>state.duration||!Finite(rule.sourceAnimationTime)||Math.Abs(rule.emitAt-rule.sourceAnimationTime/rule.sourceAnimSpeed)>.00001f||
                        !Finite(rule.range)||rule.range<=0||!Finite(rule.healFraction)||rule.healFraction<=0||rule.healFraction>1||
                        !Finite(rule.originX)||!Finite(rule.originY)||!Finite(rule.originZ)||string.IsNullOrEmpty(rule.originBone)||
                        !rule.excludeSelf||!rule.requireSameTeam||!rule.requireAlive||!rule.excludeFullHp||!rule.rangeIsExclusive||
                        string.IsNullOrEmpty(rule.sourceCallback)||!SourceSupportHashV17(rule.sourceInstructionsSha256))throw new ArgumentException("Invalid original NPC healing rule.");
                    healers.Add(rule.npc,rule);
                }
                foreach(var rule in document.railHouses){
                    NpcDefinition profile,spawn;NpcStateDefinition state;
                    if(rule==null||string.IsNullOrEmpty(rule.npc)||houses.ContainsKey(rule.npc)||!npcDefinitions.TryGetValue(rule.npc,out profile)||profile.environmentKind!="source-rail-house"||
                        !npcDefinitions.TryGetValue(rule.spawnNpc??"",out spawn)||!npcStates.TryGetValue(rule.npc+"/"+rule.spawnState,out state)||
                        !Finite(rule.waitSeconds)||rule.waitSeconds<=0||!Finite(rule.emitAt)||rule.emitAt<=0||rule.emitAt>state.duration||
                        !Finite(rule.ownerHpCostFraction)||rule.ownerHpCostFraction<=0||rule.ownerHpCostFraction>1||rule.sourcePopulationSlot!=0||
                        !rule.oneActorPerCrossing||!rule.sameOriginalPosition||!rule.inheritFacing||
                        string.IsNullOrEmpty(rule.sourceCallback)||!SourceSupportHashV17(rule.sourceInstructionsSha256))throw new ArgumentException("Invalid original rail-house support rule.");
                    foreach(string id in new[]{rule.initialState,rule.restState,rule.readyState,rule.spawnState,rule.damageState,rule.deathState})
                        if(!npcStates.ContainsKey(rule.npc+"/"+id))throw new ArgumentException("Unbound original rail-house state.");
                    houses.Add(rule.npc,rule);
                }
            }
            var standardBlader=StandardHealerBladerRuleR1419();
            if(standardBlader!=null&&!healers.ContainsKey(standardBlader.npc))healers.Add(standardBlader.npc,standardBlader);else standardBlader=null;
            npcSourceHealersV17.Clear();npcSourceRailHousesV17.Clear();foreach(var pair in healers)npcSourceHealersV17.Add(pair.Key,pair.Value);foreach(var pair in houses)npcSourceRailHousesV17.Add(pair.Key,pair.Value);npcStandardHealerBladerR1419=standardBlader;ClearNpcSourceSupportActorsV17();
        }
        // The same original Nasod_Healer_Blader.lua callback is shared by the
        // standard profile: .63s Bone_Jewelry query,700 native*.28,half maxHP.
        //35/30 is the full Healing motion duration; it is not the heal timer.
        NpcSourceHealerRuleV17 StandardHealerBladerRuleR1419(){
            const string npc="NUI_NASOD_HEALER_BLADER",heal="NASOD_HEALER_BLADER_HEAL";
            NpcDefinition profile;NpcStateDefinition state;NpcMechanismStateRule clock;
            if(!npcDefinitions.TryGetValue(npc,out profile)||profile.kind!="nasod-healer-blader"||profile.waitState!="NASOD_HEALER_BLADER_WAIT"||profile.walkState!="NASOD_HEALER_BLADER_WALK"||
                !npcStates.TryGetValue(npc+"/"+heal,out state)||state.sourceAnimation!="Healing"||state.sourceAnimSpeed!=1||Math.Abs(state.duration-35f/30f)>.00001f||state.loop||!state.superArmor||state.nextState!=profile.waitState||
                !npcMechanismRules.TryGetValue(npc+"/"+heal,out clock)||!clock.cooldownExplicit||clock.cooldown!=7||clock.initialElapsed!=0)return null;
            return new NpcSourceHealerRuleV17{npc=npc,state=heal,sourceAnimation="Healing",sourceAnimSpeed=1,emitAt=.63f,sourceAnimationTime=.63f,
                originBone="Bone_Jewelry",originX=.0415170355f,originY=16.9835088f,originZ=.0239690772f,range=196,healFraction=.5f,
                excludeSelf=true,requireSameTeam=true,requireAlive=true,excludeFullHp=true,rangeIsExclusive=true,
                sourceCallback="NASOD_HEALER_BLADER_HEAL_FRAME_MOVE",sourceInstructionsSha256="59f5c398d0bf642b98ed67e1f6a7a3c97e163e1c0c8861c6c8ab86cad5d40477"};
        }
        bool StandardHealerBladerActorR1419(Enemy actor){
            // Enemy roster is the model's hostile team. The player never enters
            // this list. This user-requested live/current-encounter product policy
            // includes bosses and leaves the original source exclusion JSON intact.
            if(actor==null||CurrentEncounter==null||!actor.active||actor.dormant||actor.passive||actor.isGate||actor.hp<=0||actor.state==CombatState.Dead||
                actor.encounterIndex!=EncounterIndex||!enemies.Contains(actor))return false;
            var profile=NpcProfile(actor);
            if(profile==null||profile.id!=actor.sourceNPC||profile.kind!=actor.kind)return false;
            // Original Factory Press/Press3 are active profiled environment
            // actors with passive=false and empty environmentKind, not monsters.
            // Keep the stationary Nasod Maker and every eligible boss target.
            return !(profile.id=="NUI_FACTORY_PRESS"&&profile.kind=="factory-press"||profile.id=="NUI_FACTORY_PRESS3"&&profile.kind=="factory-press3");
        }
        void ClearNpcSourceSupportActorsV17(){npcRailHouseActorsV17.Clear();npcSourceSupportEventsV17.Clear();}
        static bool ValidateNpcSourceSupportEnvironmentV17(NpcDefinition profile){
            if(profile.environmentKind!="source-rail-house")return false;
            var rule=profile.environment;if(rule==null)throw new ArgumentException("Missing original rail-house environment state binding.");
            foreach(string id in new[]{rule.initialState,rule.restState,rule.deathState})if(string.IsNullOrEmpty(id)||!Array.Exists(profile.states,s=>s.id==id))throw new ArgumentException("Missing rail-house source state.");
            return true;
        }
        void SourceSupportEventV17(string kind,Enemy owner,Enemy target,float at,float amount){if(kind=="heal")EmitMine2ActualHealPresentationV21(owner,target,at);
            if(!traceNpcActions)return;if(npcSourceSupportEventsV17.Count>=512)npcSourceSupportEventsV17.RemoveAt(0);
            npcSourceSupportEventsV17.Add(new NpcSourceSupportEventV17{kind=kind,npc=owner.sourceNPC,target=target==null?"":target.sourceNPC,time=at,amount=amount,x=target==null?owner.x:target.x,y=target==null?owner.y:target.y});
        }
        void AdvanceNpcSourceSupportV17(Enemy owner,NpcStateDefinition state,float from,float to,float packetStart){
            NpcSourceHealerRuleV17 rule;
            if(owner==null||state==null||owner.hp<=0||!owner.active||owner.dormant||!npcSourceHealersV17.TryGetValue(owner.sourceNPC,out rule)||state.id!=rule.state||!(from<rule.emitAt&&rule.emitAt<=to))return;
            bool standardBlader=object.ReferenceEquals(rule,npcStandardHealerBladerR1419);
            // Core's configured source callback is the sole owner for its .63s
            // packet. Never run this second consumer for the same Core actor.
            if(standardBlader&&(coreHardLocalAddsV21!=null||!StandardHealerBladerActorR1419(owner)||!NpcMechanismIsHost))return;
            float depthUnit=standardBlader ? .28f : 1f;
            float at=packetStart+rule.emitAt-from;float x=owner.x+owner.facing*rule.originX*owner.visualScale,y=owner.y+rule.originY*owner.visualScale,z=owner.nativeDepth*depthUnit+rule.originZ*owner.visualScale;
            // Enemy actors share the hostile team in this model. Dormant
            // directors and passive non-NPC props are outside that NPC list;
            // original inactive living combat NPCs are still eligible.
            foreach(var target in enemies){
                if(object.ReferenceEquals(target,owner)||!target.active||target.dormant||target.passive||target.isGate||target.hp<=0||target.hp>=target.maxHp||NpcProfile(target)==null)continue;
                if(standardBlader&&!StandardHealerBladerActorR1419(target))continue;
                float dx=target.x-x,dy=target.y-y,dz=target.nativeDepth*depthUnit-z;
                if(dx*dx+dy*dy+dz*dz>=rule.range*rule.range)continue;
                float amount=Math.Min(target.maxHp-target.hp,target.maxHp*rule.healFraction);RecoverEnemyHealth(target,amount,rule.healFraction*100);SourceSupportEventV17("heal",owner,target,at,amount);
            }
        }
        int RailHousePopulationV17(string npc){
            int count=0;foreach(var enemy in enemies)if(enemy.hp>0&&enemy.sourceNPC==npc)count++;
            foreach(var request in npcSummonRequests)if(request.room==room&&request.alreadyEmitted&&request.npc==npc)count++;
            return count;
        }
        void EnterRailHouseStateV17(Enemy owner,RailHouseActorV17 actor,string state){actor.age=0;actor.emitted=false;EnterNpcState(owner,npcStates[owner.sourceNPC+"/"+state]);}
        void EmitRailHouseV17(Enemy owner,NpcSourceRailHouseRuleV17 rule,float at){
            var original=SummonTemplate(rule.spawnNpc);if(original==null)throw new InvalidOperationException("Original rail-house population has no original rail template.");
            // Preserve difficulty/scale balance while using the house's exact
            // source position, depth and facing for the newly created rail.
            var template=new EnemySpawn{hp=original.hp,visualScale=original.visualScale,nativeDepth=owner.nativeDepth};
            npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=template,npc=rule.spawnNpc,room=room,encounter=owner.encounterIndex,x=owner.x,y=owner.y,line=owner.groundLine,facing=owner.facing,due=at,alreadyEmitted=true});
            SummonEvent("rail-house-create",owner,rule.spawnNpc,owner.x,owner.y,owner.groundLine);SourceSupportEventV17("rail-house-create",owner,null,at,1);
            // The original create call precedes host-owned self cost. Queue
            // commit intentionally retains an already-emitted final request.
            if(NpcMechanismIsHost){float cost=owner.maxHp*rule.ownerHpCostFraction;owner.hp=Math.Max(0,owner.hp-cost);SourceSupportEventV17("rail-house-self-cost",owner,null,at,cost);if(owner.hp<=0){owner.state=CombatState.Dead;owner.stun=0;InterruptNpcAction(owner);}}
        }
        bool TickNpcSourceSupportEnvironmentV17(Enemy owner,NpcDefinition profile,float dt){
            if(profile.environmentKind!="source-rail-house")return false;
            if(!owner.active||owner.dormant)return true;
            NpcSourceRailHouseRuleV17 rule;if(!npcSourceRailHousesV17.TryGetValue(owner.sourceNPC,out rule))throw new InvalidOperationException("Rail-house environment has no verified original support rule.");
            RailHouseActorV17 actor;
            if(!npcRailHouseActorsV17.TryGetValue(owner,out actor)){
                int initial=0;foreach(var spawn in CurrentRoom.enemies)if(spawn.sourceNPC==rule.spawnNpc&&spawn.hp>0)initial++;
                actor=new RailHouseActorV17{x=owner.x,y=owner.y,line=owner.groundLine,facing=owner.facing,initialPopulation=initial};npcRailHouseActorsV17.Add(owner,actor);EnterRailHouseStateV17(owner,actor,rule.initialState);
            }
            owner.x=actor.x;owner.y=actor.y;owner.groundLine=actor.line;owner.facing=actor.facing;owner.vx=owner.vy=0;owner.aiEnabled=false;owner.passive=false;
            if(owner.hp<=0){CancelNpcStateSounds(owner);owner.deathTime+=dt;if(owner.npcState!=rule.deathState)EnterRailHouseStateV17(owner,actor,rule.deathState);AdvancePassagePose(owner,NpcState(owner),dt,elapsed-dt);return true;}
            if(owner.hitPause>0){float frozen=Math.Min(dt,owner.hitPause);owner.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return true;}
            if(NpcState(owner)==null||owner.state!=CombatState.Normal){owner.state=CombatState.Normal;owner.stun=owner.downTime=owner.recoveryTime=0;owner.knockdownOnLanding=false;EnterRailHouseStateV17(owner,actor,rule.damageState);}
            float remaining=dt,at=elapsed-dt;
            while(remaining>.000001f){
                float step=Math.Min(remaining,1f/120);var state=NpcState(owner);float from=actor.age;actor.age+=step;AdvancePassagePose(owner,state,step,at);
                if(state.id==rule.initialState){if(actor.age+1e-6f>=state.duration)EnterRailHouseStateV17(owner,actor,rule.restState);}
                else if(state.id==rule.restState){if(actor.age+1e-6f>=rule.waitSeconds)EnterRailHouseStateV17(owner,actor,rule.readyState);}
                else if(state.id==rule.readyState){if(RailHousePopulationV17(rule.spawnNpc)<actor.initialPopulation)EnterRailHouseStateV17(owner,actor,rule.spawnState);else if(actor.age+1e-6f>=state.duration)EnterRailHouseStateV17(owner,actor,rule.restState);}
                else if(state.id==rule.spawnState){
                    if(!actor.emitted&&from<rule.emitAt&&rule.emitAt<=actor.age+1e-6f){actor.emitted=true;EmitRailHouseV17(owner,rule,at+Math.Max(0,rule.emitAt-from));}
                    if(owner.hp<=0){EnterRailHouseStateV17(owner,actor,rule.deathState);break;}
                    if(actor.age+1e-6f>=state.duration)EnterRailHouseStateV17(owner,actor,rule.restState);
                }else if(actor.age+1e-6f>=state.duration)EnterRailHouseStateV17(owner,actor,rule.restState);
                remaining-=step;at+=step;
            }
            return true;
        }
    }
}
