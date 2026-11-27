using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcTraversalDefinition {
        public string sourcePrefix;public float upVelocity,directionalUpVelocity,upHorizontalSpeed,downHorizontalSpeed,entryOffset,interval,landingDuration,maximumAssistedUpVelocity;
    }
    public sealed partial class GameModel {
        static void ValidateNpcTraversal(NpcDefinition profile){
            var t=profile.traversal;if(t==null)return;
            if(string.IsNullOrEmpty(t.sourcePrefix)||!Finite(t.upVelocity)||t.upVelocity<=0||!Finite(t.directionalUpVelocity)||t.directionalUpVelocity<=0||!Finite(t.upHorizontalSpeed)||t.upHorizontalSpeed<0||!Finite(t.downHorizontalSpeed)||t.downHorizontalSpeed<0||!Finite(t.entryOffset)||t.entryOffset<0||!Finite(t.interval)||t.interval<=0||!Finite(t.landingDuration)||t.landingDuration<=0||!Finite(t.maximumAssistedUpVelocity)||t.maximumAssistedUpVelocity<t.upVelocity)throw new ArgumentException("Invalid source NPC traversal profile");
        }
        // Forest playable traversal adapter. The four source tables each have
        // UP / UP_DIR / DOWN / DOWN_DIR / LANDING; Ent has none. Their authored
        // .28-scaled entry offset, impulses, gravity and landing durations are
        // retained. Deterministic support routing, air steering/braking and
        // existing jump/fall/idle art are demo adapters, not native CHASE code.
        // Source: Reports/forest-jump-retaliation-audit-v09/source-audit.json.
        sealed class NpcTraversal {
            public string sourceState="";
            public int phase,origin=-1,destination=-1,goal=-1;
            public float age,nextJumpAt,landingX;
            public bool directional;
        }
        readonly Dictionary<Enemy,NpcTraversal> npcTraversal=new Dictionary<Enemy,NpcTraversal>();
        readonly List<Enemy> npcTraversalStale=new List<Enemy>();
        float npcTraversalPrunedAt=float.NaN;
        static bool ForestCanJump(string npc){return npc=="NUI_FOREST_DARK_MAGICIAN"||npc=="NUI_CURSE_WISP"||npc=="NUI_ANGER_FOREST_KNIGHT"||npc=="NUI_ANGER_FOREST_FAIRY";}
        static float ForestLandingDuration(string npc){return npc=="NUI_ANGER_FOREST_FAIRY"?.5f:npc=="NUI_ANGER_FOREST_KNIGHT"?50f/60:56f/60;}
        void PruneNpcTraversal(){
            if(npcTraversalPrunedAt==elapsed)return;npcTraversalPrunedAt=elapsed;npcTraversalStale.Clear();
            foreach(var item in npcTraversal)if(!enemies.Contains(item.Key))npcTraversalStale.Add(item.Key);
            foreach(var enemy in npcTraversalStale)npcTraversal.Remove(enemy);
        }
        void SetNpcTraversalPhase(Enemy enemy,NpcTraversal travel,int phase,bool directional=false){
            travel.phase=phase;travel.age=0;travel.directional=directional;
            string suffix=phase==1?(directional?"JUMP_UP_DIR":"JUMP_UP"):phase==2?(directional?"JUMP_DOWN_DIR":"JUMP_DOWN"):"JUMP_LANDING";
            var profile=NpcProfile(enemy);string prefix=profile.traversal==null?enemy.sourceNPC.Substring(4):profile.traversal.sourcePrefix;
            InterruptNpcAction(enemy);enemy.npcState=prefix+"_"+suffix;
            travel.sourceState=enemy.npcState;enemy.clip=phase==1?"jump":phase==2?"fall":"idle";enemy.clock=enemy.npcTime=0;
            if(phase==3){enemy.state=CombatState.Normal;enemy.vx=enemy.vy=0;}
            else enemy.state=CombatState.Airborne;
            TraceNpc(enemy,"traversal-source-state");
        }
        GroundSurface NpcTraversalDestination(Enemy enemy,NpcTraversal travel,GroundSurface start,GroundSurface goal){
            var cached=Surface(travel.destination);
            if(cached!=null&&travel.goal==goal.id&&enemy.groundLine!=cached.id)return cached;
            travel.destination=-1;travel.origin=start.id;travel.goal=goal.id;
            if(start.id==goal.id)return null;
            var queue=new Queue<int>();var previous=new Dictionary<int,int>();queue.Enqueue(start.id);previous[start.id]=-1;
            while(queue.Count>0&&!previous.ContainsKey(goal.id)){
                var a=Surface(queue.Dequeue());
                foreach(var b in CurrentRoom.surfaces){
                    if(previous.ContainsKey(b.id)||a.id==b.id||HasEncounters&&(b.x2<CurrentEncounter.leftX||b.x1>CurrentEncounter.rightX))continue;
                    float lo=Math.Max(a.x1,b.x1),hi=Math.Min(a.x2,b.x2),gap=lo-hi,x=(lo+hi)*.5f;
                    float dy=b.Y(Clamp(x,b.x1,b.x2))-a.Y(Clamp(x,a.x1,a.x2));
                    // A 420 straight jump clears the forest's 67.2px risers.
                    // Its late 112px/s steering cannot bridge a large high gap.
                    var traversal=NpcProfile(enemy).traversal;float maxRise=traversal==null?82:traversal.entryOffset+traversal.maximumAssistedUpVelocity*traversal.maximumAssistedUpVelocity/2240-8;
                    if(gap>90||dy>maxRise||dy< -300||dy>55&&gap>16)continue;
                    if(dy< -8&&gap<0&&!a.platform)continue;
                    previous[b.id]=a.id;queue.Enqueue(b.id);
                }
            }
            if(!previous.ContainsKey(goal.id))return null;
            int next=goal.id;while(previous[next]!=start.id)next=previous[next];
            travel.destination=next;return Surface(next);
        }
        bool ForestTraversalIntent(Enemy enemy,NpcDefinition profile,NpcTraversal travel,out float portal,out bool jump,out bool drop){
            portal=enemy.x;jump=drop=false;
            var start=SurfaceBelow(enemy.x,enemy.y+.1f);var goal=SurfaceBelow(player.x,player.y+.1f);
            if(start==null||goal==null)return false;
            // Let source attack decisions handle ordinary combat on the same
            // floor, including continuous ramps and already reachable bodies.
            if(start.id==goal.id||Math.Abs(player.y-enemy.y)<=32&&travel.destination<0)return false;
            var destination=NpcTraversalDestination(enemy,travel,start,goal);if(destination==null)return false;
            var origin=Surface(travel.origin)??start;
            float lo=Math.Max(origin.x1,destination.x1),hi=Math.Min(origin.x2,destination.x2);
            bool overlap=hi-lo>6;
            float margin=overlap?Math.Min(12,(hi-lo)*.25f):0;
            portal=overlap?Clamp(player.x,lo+margin,hi-margin):destination.x1>=origin.x2?origin.x2-.04f:origin.x1+.04f;
            float landMargin=Math.Min(10,(destination.x2-destination.x1)*.25f);
            travel.landingX=overlap?portal:destination.x1>=origin.x2?destination.x1+landMargin:destination.x2-landMargin;
            float dy=destination.Y(Clamp(portal,destination.x1,destination.x2))-origin.Y(Clamp(portal,origin.x1,origin.x2));
            if(dy< -8&&origin.platform&&overlap){drop=Math.Abs(enemy.x-portal)<=3;return true;}
            if(dy<=7){
                portal=travel.landingX;
                jump=!overlap&&lo-hi>7&&Math.Abs(enemy.x-(destination.x1>=origin.x2?origin.x2:origin.x1))<18;
                travel.directional=jump;return true;
            }
            jump=Math.Abs(enemy.x-portal)<=3;
            float directionalRise=profile.traversal==null?55:profile.traversal.entryOffset+profile.traversal.directionalUpVelocity*profile.traversal.directionalUpVelocity/2240-8;
            travel.directional=!overlap&&dy<=directionalRise;return true;
        }
        void TickNpcTraversalCore(Enemy enemy,float dt){
            PruneNpcTraversal();NpcTraversal travel;npcTraversal.TryGetValue(enemy,out travel);
            var profile=NpcProfile(enemy);
            bool eligible=NativeTerrain&&profile!=null&&(ForestCanJump(enemy.sourceNPC)||profile.traversal!=null)&&enemy.active&&!enemy.dormant&&!enemy.passive&&!enemy.isGate&&enemy.aiEnabled&&enemy.hp>0&&enemy.stun<=0&&!enemy.knockdownOnLanding&&(enemy.state==CombatState.Normal||enemy.state==CombatState.Airborne);
            bool inFlight=travel!=null&&travel.phase!=0&&travel.sourceState==enemy.npcState;
            if(!eligible||travel!=null&&travel.phase!=0&&!inFlight){npcTraversal.Remove(enemy);TickEnemyCore(enemy,dt);return;}
            if(inFlight){TickNpcTraversalFlight(enemy,travel,dt);return;}
            var sourceState=NpcState(enemy);
            if(sourceState!=null&&sourceState.id!=profile.waitState&&sourceState.id!=profile.walkState){TickEnemyCore(enemy,dt);return;}
            if(!Grounded(enemy)){
                // Authored WAIT/WALK foot-off-line transition. Hit-launched
                // actors are excluded above and retain their reaction physics.
                if(sourceState!=null&&enemy.vy<=0){
                    if(travel==null){travel=new NpcTraversal();npcTraversal[enemy]=travel;}
                    travel.landingX=enemy.x;SetNpcTraversalPhase(enemy,travel,2);TickNpcTraversalFlight(enemy,travel,dt);return;
                }
                TickEnemyCore(enemy,dt);return;
            }
            float dx=player.x-enemy.x,dy=player.y-enemy.y,distance=(float)Math.Sqrt(dx*dx+dy*dy);
            if(distance>profile.lostRange||!enemy.alerted&&distance>enemy.aggroRange){TickEnemyCore(enemy,dt);return;}
            if(travel==null){travel=new NpcTraversal();npcTraversal[enemy]=travel;}
            float portal;bool jump,drop;
            if(!ForestTraversalIntent(enemy,profile,travel,out portal,out jump,out drop)){travel.destination=-1;TickEnemyCore(enemy,dt);return;}
            if(enemy.hitPause>0){float frozen=Math.Min(enemy.hitPause,dt);enemy.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return;}
            enemy.alerted=true;enemy.clock+=dt;IntegrateBody(enemy,dt,false);
            if(!Grounded(enemy)){SetNpcTraversalPhase(enemy,travel,2);return;}
            if(drop){
                enemy.dropThrough=.25f;enemy.y-=.1f;enemy.groundLine=-1;enemy.vy=-20;enemy.vx=0;
                SetNpcTraversalPhase(enemy,travel,2);return;
            }
            if(jump&&elapsed>=travel.nextJumpAt){
                bool directional=travel.directional;enemy.facing=travel.landingX>=enemy.x?1:-1;
                var source=profile.traversal;float offset=source==null?12.6f:source.entryOffset,impulse=source==null?(directional?336:420):(directional?source.directionalUpVelocity:source.upVelocity);
                if(source!=null&&!directional){var destination=Surface(travel.destination);if(destination!=null){float rise=destination.Y(travel.landingX)-enemy.y;float needed=(float)Math.Sqrt(Math.Max(0,2*1120*(rise-offset+8)));impulse=Math.Min(source.maximumAssistedUpVelocity,Math.Max(impulse,needed));}}
                enemy.y+=offset;enemy.vy=impulse;enemy.vx=0;enemy.groundLine=-1;
                travel.nextJumpAt=elapsed+(source==null?5:source.interval);SetNpcTraversalPhase(enemy,travel,1,directional);return;
            }
            if(sourceState==null||sourceState.id!=profile.walkState)TryEnterNpcState(enemy,npcStates[profile.id+"/"+profile.walkState]);
            var walk=npcStates[profile.id+"/"+profile.walkState];float move=Clamp(portal-enemy.x,-walk.walkSpeed*dt,walk.walkSpeed*dt);
            if(Math.Abs(move)>.00001f){enemy.facing=move>0?1:-1;MoveAlongGround(enemy,enemy.x+move);}
            AdvanceNpcLoopSounds(enemy,walk,enemy.npcTime,dt,elapsed-dt);enemy.npcTime=(enemy.npcTime+dt)%walk.duration;
        }
        void TickNpcTraversalFlight(Enemy enemy,NpcTraversal travel,float dt){
            if(enemy.hitPause>0){float frozen=Math.Min(enemy.hitPause,dt);enemy.hitPause-=frozen;dt-=frozen;if(dt<=.000001f)return;}
            enemy.clock+=dt;travel.age+=dt;enemy.npcTime=travel.age;
            if(travel.phase!=3){
                var source=NpcProfile(enemy).traversal;
                float speed=source==null?(travel.phase==1&&travel.directional?224:112):(travel.phase==1&&travel.directional?source.upHorizontalSpeed:source.downHorizontalSpeed);
                var target=Surface(travel.destination);
                // Straight source ascent has zero X. Begin demo steering only
                // after the feet clear the target, then brake at its safe point.
                bool steer=travel.directional||travel.phase==2||target!=null&&enemy.y>target.Y(travel.landingX)+1;
                if(steer)MoveAlongGround(enemy,enemy.x+Clamp(travel.landingX-enemy.x,-speed*dt,speed*dt));
                enemy.vx=0;IntegrateBody(enemy,dt,false);
                if(Grounded(enemy)){SetNpcTraversalPhase(enemy,travel,3);return;}
                if(travel.phase==1&&enemy.vy<=0){SetNpcTraversalPhase(enemy,travel,2,travel.directional);return;}
                return;
            }
            IntegrateBody(enemy,dt,false);
            if(!Grounded(enemy)){SetNpcTraversalPhase(enemy,travel,2);return;}
            var landingProfile=NpcProfile(enemy).traversal;
            if(travel.age+1e-6f<(landingProfile==null?ForestLandingDuration(enemy.sourceNPC):landingProfile.landingDuration))return;
            travel.phase=0;travel.sourceState="";travel.destination=-1;
            var profile=NpcProfile(enemy);TryEnterNpcState(enemy,npcStates[profile.id+"/"+profile.waitState]);
            TraceNpc(enemy,"traversal-landed");
        }
    }
}
