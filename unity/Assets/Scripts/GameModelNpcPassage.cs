using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public sealed partial class GameModel {
        sealed class PassageEnvironmentActor {
            internal float age,decision,x,y;internal int initialPopulation,line;
            internal bool emitted;
        }
        readonly Dictionary<Enemy,PassageEnvironmentActor> passageEnvironmentActors=new Dictionary<Enemy,PassageEnvironmentActor>();
        float MouseHouseHealthScale(string npc){
            return npc=="NUI_MOUSEHOUSE" && CurrentRoom.id=="waterway-2" ? .5f : 1f;
        }
        void ClearNpcPassageActors(){passageEnvironmentActors.Clear();}
        static void ValidateNpcPassageEnvironment(NpcDefinition p,Dictionary<string,NpcDefinition> all){
            if(p.environmentKind!="timed-steam"&&p.environmentKind!="mouse-house"){
                var unused=p.environment;if(unused!=null&&(!string.IsNullOrEmpty(unused.populationNpc)||!string.IsNullOrEmpty(unused.spawnNpc)||unused.waitSeconds!=0||unused.emitAt!=0||unused.ownerHpCost!=0))throw new ArgumentException("Population/timed passage payload on an unrelated NPC environment");
                return;
            }
            var r=p.environment;if(r==null)throw new ArgumentException("Missing source passage environment rule");
            foreach(string id in new[]{r.initialState,r.restState,r.activeState})if(string.IsNullOrEmpty(id)||!Array.Exists(p.states,s=>s.id==id))throw new ArgumentException("Missing source passage state");
            if(!Finite(r.waitSeconds)||r.waitSeconds<=0||!Finite(r.interval)||r.interval<=0)throw new ArgumentException("Invalid source passage timing");
            if(p.environmentKind=="timed-steam"){
                var active=Array.Find(p.states,s=>s.id==r.activeState);
                if(active.hits.Length==0||active.samples[0].attackLines.Length==0||p.bodySpheres.Length!=0||p.bodyHeight!=0)throw new ArgumentException("Steam requires original lines, no fabricated body");
                if(!string.IsNullOrEmpty(r.populationNpc)||!string.IsNullOrEmpty(r.spawnNpc)||r.emitAt!=0||r.ownerHpCost!=0)throw new ArgumentException("Steam cannot contain a population rule");
            }else{
                foreach(string id in new[]{r.warningState,r.deathState})if(string.IsNullOrEmpty(id)||!Array.Exists(p.states,s=>s.id==id))throw new ArgumentException("Missing mouse-house state");
                if(string.IsNullOrEmpty(r.populationNpc)||string.IsNullOrEmpty(r.spawnNpc)||!all.ContainsKey(r.populationNpc)||!all.ContainsKey(r.spawnNpc)||r.populationNpc==r.spawnNpc||
                    !Finite(r.emitAt)||r.emitAt<=0||r.emitAt>Array.Find(p.states,s=>s.id==r.activeState).duration||!Finite(r.ownerHpCost)||r.ownerHpCost<=0)throw new ArgumentException("Invalid source mouse-house population rule");
            }
        }
        void EnterNpcPassageState(Enemy e,NpcDefinition p,PassageEnvironmentActor actor,string id){
            actor.age=actor.decision=0;actor.emitted=false;EnterNpcState(e,npcStates[p.id+"/"+id]);
        }
        int PassagePopulation(Enemy owner,NpcEnvironmentRule rule){
            int count=0;foreach(var other in enemies)if(other.hp>0&&(other.sourceNPC==rule.populationNpc||other.sourceNPC==rule.spawnNpc))count++;
            // A request already emitted in this actor pass counts as a new live
            // mouse, even though mutation waits until after the foreach.
            foreach(var request in npcSummonRequests)if(request.room==room&&request.alreadyEmitted&&request.npc==rule.spawnNpc)count++;
            return count;
        }
        void EmitHouseMouse(Enemy owner,NpcEnvironmentRule rule,float at){
            // Original GetPos/GetIsRight: no random/new fixed spawn coordinate.
            // Source difficulty balance is reused from the room's ordinary
            // mouse; NODROP remains its own behavior and visual identity.
            var template=SummonTemplate(rule.populationNpc);
            if(template==null)throw new InvalidOperationException("Mouse-house has no original mouse balance template");
            npcSummonRequests.Add(new NpcSummonRequest{owner=owner,template=template,npc=rule.spawnNpc,room=room,encounter=owner.encounterIndex,
                x=owner.x,y=owner.y,line=owner.groundLine,facing=owner.facing,due=at,alreadyEmitted=true});
            SummonEvent("house-create",owner,rule.spawnNpc,owner.x,owner.y,owner.groundLine);
            // Each birth still spends the same share of the house's starting health.
            owner.hp=Math.Max(0,owner.hp-rule.ownerHpCost*MouseHouseHealthScale(owner.sourceNPC));
            if(owner.hp<=0){owner.state=CombatState.Dead;owner.stun=0;InterruptNpcAction(owner);}
        }
        void AdvancePassagePose(Enemy e,NpcStateDefinition state,float dt,float at){
            e.clock+=dt;
            if(state.loop){AdvanceNpcLoopSounds(e,state,e.npcTime,dt,at);e.npcTime=(e.npcTime+dt)%state.duration;}
            else{float to=Math.Min(state.duration,e.npcTime+dt);AdvanceNpcStateSounds(e,state,e.npcTime,to,at);e.npcTime=to;}
        }
        void HitSteam(Enemy e,NpcDefinition p,NpcStateDefinition state,float from,float to){
            for(int i=0;i<state.hits.Length;i++){
                var hit=state.hits[i];float lo=Math.Max(from,Math.Max(hit.start,e.npcNextHits[i])),hi=Math.Min(to,hit.end);
                if(hi<lo||!NpcStateLinesTouchPlayer(e,state,hi))continue;
                int damage=(int)Math.Round(p.physicalAttack*hit.physical+p.magicAttack*hit.magic);
                int dealt=ApplyEnemyAttackToPlayer(damage,e.x-e.facing,hit.reaction,hit.launch);
                if(dealt<=0)continue;
                e.npcNextHits[i]=hit.repeat?hi+hit.repeatGap:float.PositiveInfinity;
                if(!PlayerHasSuperArmor&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp)player.vx=e.facing*hit.push;
                hitstop=Math.Max(hitstop,hit.stopDefender);TraceNpc(e,"steam-hit-player",i,dealt,hi);
            }
        }
        bool TickNpcPassageEnvironment(Enemy e,NpcDefinition p,float dt){
            bool steam=p.environmentKind=="timed-steam",house=p.environmentKind=="mouse-house";
            if(!steam&&!house)return false;if(!e.active||e.dormant)return true;
            var r=p.environment;PassageEnvironmentActor actor;
            if(!passageEnvironmentActors.TryGetValue(e,out actor)){
                // The source START callback runs at room creation. Read that
                // immutable initial roster so a kill before the first frame
                // cannot reduce the snapshot accidentally.
                int initial=0;if(house)foreach(var spawn in CurrentRoom.enemies)if(spawn.sourceNPC==r.populationNpc&&spawn.hp>0)initial++;
                actor=new PassageEnvironmentActor{x=e.x,y=e.y,line=e.groundLine,initialPopulation=initial};passageEnvironmentActors.Add(e,actor);
                if(house&&!string.IsNullOrEmpty(p.initialState)){if(NpcState(e)==null&&e.state==CombatState.Normal)EnterNpcPassageState(e,p,actor,p.initialState);}else EnterNpcPassageState(e,p,actor,r.initialState);
            }
            e.x=actor.x;e.y=actor.y;e.groundLine=actor.line;e.vx=e.vy=0;
            if(steam){e.passive=true;e.aiEnabled=false;}
            if(e.hp<=0){
                CancelNpcStateSounds(e);e.deathTime+=dt;
                if(house){if(e.npcState!=r.deathState)EnterNpcPassageState(e,p,actor,r.deathState);AdvancePassagePose(e,NpcState(e),dt,elapsed-dt);}
                return true;
            }
            if(house&&(NpcState(e)==null||e.state!=CombatState.Normal)){
                // Every original house damage type maps to MOUSE_HOUSE_DAMAGE;
                // NEVER_MOVE forbids generic launch/knockdown displacement.
                if(e.state==CombatState.Knockdown||e.state==CombatState.GettingUp)e.superArmor=0;
                e.state=CombatState.Normal;e.stun=e.downTime=e.recoveryTime=0;e.knockdownOnLanding=false;
                string hurt=Array.Find(p.states,s=>s.clip=="hurt").id;
                EnterNpcPassageState(e,p,actor,hurt);
            }
            float remaining=dt,at=elapsed-dt;
            while(remaining>.000001f){
                float step=Math.Min(remaining,1f/120);var state=NpcState(e);float from=actor.age;actor.age+=step;actor.decision+=step;
                AdvancePassagePose(e,state,step,at);
                if(steam){
                    if(state.id==r.initialState){
                        if(actor.decision+1e-6f>=r.interval){actor.decision-=r.interval;if(actor.age>r.waitSeconds)EnterNpcPassageState(e,p,actor,r.activeState);}
                    }else{
                        HitSteam(e,p,state,from,actor.age);
                        if(actor.age+1e-6f>=state.duration)EnterNpcPassageState(e,p,actor,r.restState);
                    }
                }else if(!string.IsNullOrEmpty(p.initialState)&&state.id==p.initialState){
                    if(actor.age+1e-6f>=state.duration)EnterNpcPassageState(e,p,actor,state.nextState);
                }else if(state.id==r.initialState||state.id==r.restState){
                    if(actor.age+1e-6f>=r.waitSeconds)EnterNpcPassageState(e,p,actor,r.warningState);
                }else if(state.id==r.warningState){
                    if(PassagePopulation(e,r)<actor.initialPopulation)EnterNpcPassageState(e,p,actor,r.activeState);
                    else if(actor.age+1e-6f>=state.duration)EnterNpcPassageState(e,p,actor,r.restState);
                }else if(state.id==r.activeState){
                    if(!actor.emitted&&from<r.emitAt&&r.emitAt<=actor.age+1e-6f){actor.emitted=true;EmitHouseMouse(e,r,at+Math.Max(0,r.emitAt-from));}
                    if(e.hp<=0){EnterNpcPassageState(e,p,actor,r.deathState);break;}
                    if(actor.age+1e-6f>=state.duration)EnterNpcPassageState(e,p,actor,r.restState);
                }else if(actor.age+1e-6f>=state.duration)EnterNpcPassageState(e,p,actor,r.restState);
                remaining-=step;at+=step;
            }
            return true;
        }
    }
}
