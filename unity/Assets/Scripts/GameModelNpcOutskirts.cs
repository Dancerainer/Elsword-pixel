using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public sealed partial class GameModel {
        sealed class OutskirtsTrapActor {internal float x,y,decision;internal int line,facing;}
        readonly Dictionary<Enemy,OutskirtsTrapActor> outskirtsTraps=new Dictionary<Enemy,OutskirtsTrapActor>();
        void ClearNpcOutskirtsActors(){outskirtsTraps.Clear();}
        static void ValidateNpcOutskirtsEnvironment(NpcDefinition p){
            if(p.environmentKind!="proximity-trap")return;
            var r=p.environment;
            if(r==null||!Finite(r.interval)||r.interval<=0||!Finite(r.rate)||r.rate<0||r.rate>100||!Finite(r.near)||r.near<=0)throw new ArgumentException("Invalid source proximity trap condition");
            foreach(string id in new[]{r.initialState,r.restState,r.activeState})if(string.IsNullOrEmpty(id)||!Array.Exists(p.states,s=>s.id==id))throw new ArgumentException("Missing original proximity trap state");
            var attack=Array.Find(p.states,s=>s.id==r.activeState);
            if(attack.hits.Length==0||attack.samples[0].spheres.Length==0||p.bodySpheres.Length!=0||p.bodyHeight!=0)throw new ArgumentException("Wood trap requires its source attack helpers and absent body");
        }
        bool TickNpcOutskirtsEnvironment(Enemy e,NpcDefinition p,float dt){
            if(p.environmentKind!="proximity-trap")return false;
            if(!e.active||e.dormant)return true;
            OutskirtsTrapActor actor;var r=p.environment;
            if(!outskirtsTraps.TryGetValue(e,out actor)){
                actor=new OutskirtsTrapActor{x=e.x,y=e.y,line=e.groundLine,facing=e.facing};outskirtsTraps.Add(e,actor);
                EnterNpcState(e,npcStates[p.id+"/"+r.initialState]);
            }
            e.x=actor.x;e.y=actor.y;e.groundLine=actor.line;e.facing=actor.facing;e.vx=e.vy=0;e.passive=true;e.aiEnabled=false;
            if(e.hp<=0){CancelNpcStateSounds(e);e.deathTime+=dt;return true;}
            float remaining=dt,at=elapsed-dt;
            while(remaining>.000001f){
                float step=Math.Min(remaining,1f/120);var state=NpcState(e);
                if(state==null){EnterNpcState(e,npcStates[p.id+"/"+r.restState]);state=NpcState(e);actor.decision=0;}
                if(state.id==r.initialState||state.id==r.restState){
                    AdvancePassagePose(e,state,step,at);actor.decision+=step;
                    if(actor.decision+1e-6f>=r.interval){
                        actor.decision-=r.interval;float dx=player.x-e.x,dy=player.y-e.y;
                        if(player.hp>0&&dx*dx+dy*dy<=r.near*r.near&&NpcRoll()<r.rate){
                            EnterNpcState(e,npcStates[p.id+"/"+r.activeState]);actor.decision=0;e.alerted=true;
                        }
                    }
                }else{
                    e.clock+=step;TickNpcRule(e,step);
                    // Existing rule consumer owns sampled hit windows, sound,
                    // hit memory and motion-end. Trap NEVER_MOVE fixes its
                    // source anchor/facing after that common consumer runs.
                    e.x=actor.x;e.y=actor.y;e.groundLine=actor.line;e.facing=actor.facing;e.vx=e.vy=0;
                    // Passive environments deliberately cannot use the combat
                    // request queue. Their source motion-end is owned here.
                    if(e.npcState==state.id&&e.npcTime+1e-6f>=state.duration){EnterNpcState(e,npcStates[p.id+"/"+r.restState]);actor.decision=0;}
                }
                remaining-=step;at+=step;
            }
            return true;
        }
    }
}
