using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class PeitaSphereFrame { public float[] spheres; }
    [Serializable] public sealed class PeitaTrace {
        public string name, sound;
        public float life, sampleSpan, speed, damage, gap, curseSeconds, soundAt;
        public bool loop, repeat;
        public float[] emitTimes;
        public PeitaSphereFrame[] frames;
    }

    internal sealed class PeitaHazardState {
        internal int phase;
        internal float timer, floor;
        internal readonly Dictionary<Fighter,float> nextHit = new Dictionary<Fighter,float>();
    }

    public sealed partial class GameModel {
        sealed class PeitaSpell {
            internal Enemy owner;
            internal PeitaTrace trace;
            internal Effect display;
            internal int facing;
            internal float age, x, y, nextHit;
            internal bool sounded;
        }
        readonly List<PeitaSpell> peitaSpells = new List<PeitaSpell>();

        void ClearPeitaSpells() {
            foreach (var spell in peitaSpells) effects.Remove(spell.display);
            peitaSpells.Clear();
        }

        void PeitaSourceSound(Enemy e,string key) {
            var sound = new NpcTimedSound { id=key, sound="Pixel/peita/sounds/"+key };
            QueueNpcSound(sound,e,++npcSoundEntrySerial,0,elapsed,e.clip);
        }

        static float[] PeitaSpheres(PeitaTrace trace, float time) {
            float normalized=time/Math.Max(.0001f,trace.sampleSpan);
            if(trace.loop) normalized-=(float)Math.Floor(normalized);
            int i=(int)Math.Round(Clamp(normalized,0,1)*(trace.frames.Length-1));
            return trace.frames[i].spheres;
        }

        static bool PeitaSphereHit(float[] points,float x,float y,int facing,float depth,AttackBox box) {
            for(int i=0;i<points.Length;i+=4) {
                float cx=x+facing*points[i], cy=y+points[i+1], r=points[i+3];
                float dx=cx-Clamp(cx,box.left,box.right), dy=cy-Clamp(cy,box.bottom,box.top);
                float dz=Math.Max(0,Math.Abs(depth-points[i+2])-9.8f);
                if(dx*dx+dy*dy+dz*dz<=r*r) return true;
            }
            return false;
        }

        AttackBox PeitaPlayerBox() {
            return new AttackBox {left=player.x-9.8f,right=player.x+9.8f,bottom=player.y,top=player.y+42};
        }

        bool EmitPeitaSpell(Enemy e,PeitaAttack action,float from,float to) {
            var trace=action.trace;
            if(trace==null || trace.emitTimes==null)return false;
            foreach(float at in trace.emitTimes) {
                if(at<=from || at>to+.000001f)continue;
                bool ghost=trace.speed>0, wave=e.kind=="gargoyle_stone";
                float dx=wave?29.6f:ghost?28:trace.curseSeconds>0?28:
                         trace.name.EndsWith("_A2",StringComparison.Ordinal)?-28:-8.4f;
                float dy=wave?25.8f:ghost?35:trace.curseSeconds>0?0:-7;
                string type=wave?"peita-sonic-wave":ghost?"peita-ghost":trace.curseSeconds>0?"peita-dark-cloud":"peita-ghost-ring";
                var display=new Effect{type=type,x=e.x+dx*e.facing,y=e.y+dy,facing=e.facing,lifetime=trace.life};
                effects.Add(display);
                peitaSpells.Add(new PeitaSpell {owner=e,trace=trace,x=display.x,y=display.y,
                    facing=e.facing,display=display});
                if(trace.curseSeconds>0 && Math.Abs(player.x-e.x)<336)shake=Math.Max(shake,3);
            }
            return true;
        }

        void TickPeitaSpells(float dt) {
            for(int i=peitaSpells.Count-1;i>=0;i--) {
                var s=peitaSpells[i];
                if(!s.owner.active || s.owner.dormant || !enemies.Contains(s.owner) || s.age>=s.trace.life) {
                    effects.Remove(s.display);peitaSpells.RemoveAt(i);continue;
                }
                s.age+=dt;
                var trace=s.trace;
                float[] shape=PeitaSpheres(trace,s.age);
                float x=s.x+s.facing*trace.speed*s.age;
                if(!s.sounded && s.age>=trace.soundAt) { PeitaSourceSound(s.owner,trace.sound);s.sounded=true; }
                // The cloud is a volume of several adjacent helpers. Its visual sits
                // at their union, while damage uses the individual animated spheres.
                float left=float.PositiveInfinity,right=float.NegativeInfinity,bottom=float.PositiveInfinity,top=float.NegativeInfinity;
                for(int j=0;j<shape.Length;j+=4) {
                    float cx=x+s.facing*shape[j], cy=s.y+shape[j+1], radius=shape[j+3];
                    left=Math.Min(left,cx-radius);right=Math.Max(right,cx+radius);
                    top=Math.Max(top,cy+radius);bottom=Math.Min(bottom,cy-radius);
                }
                s.display.x=(left+right)/2;s.display.y=(top+bottom)/2;
                s.display.peitaWidth=right-left;s.display.peitaHeight=top-bottom;
                if(s.age>trace.life || s.age<s.nextHit || player.hp<=0 ||
                   !PeitaSphereHit(shape,x,s.y,s.facing,0,PeitaPlayerBox()))continue;
                int damage=Math.Max(1,(int)Math.Round((s.owner.kind.EndsWith("_boss",StringComparison.Ordinal)?64:46)*trace.damage));
                var reaction=trace.curseSeconds>0?HitReaction.None:HitReaction.Hurt;
                if(ApplyEnemyAttackToPlayer(damage,x,reaction)<=0)continue;
                s.nextHit=trace.repeat?s.age+Math.Max(.05f,trace.gap):float.PositiveInfinity;
                if(trace.curseSeconds>0)ApplyNpcManaDrainV17(trace.curseSeconds,15,elapsed);
            }
        }

        PeitaAttack PickPeitaAttack(Enemy e,PeitaAttack[] options) {
            var state=e.peitaAttack;
            if(e.kind=="plant_whip_a")return options[options.Length-1];
            if(e.kind!="gliter_necromancer_boss")return options[state.sequence++%options.Length];
            // Its source WAIT gates B below 70% HP and C below 40%.
            for(int i=0;i<options.Length;i++) {
                var attack=options[state.sequence++%options.Length];
                string source=attack.trace?.name;
                if(source=="GLITER_NECROMANCER_BOSS_ATTACK_B" && e.hp>=e.maxHp*.7f)continue;
                if(source=="GLITER_NECROMANCER_BOSS_ATTACK_C" && e.hp>=e.maxHp*.4f)continue;
                return attack;
            }
            return options[0];
        }

        void PeitaAllTeamHit(Enemy owner,PeitaHazardState h,Fighter target,int damage,HitReaction reaction,float gap) {
            if(h.nextHit.TryGetValue(target,out float next) && next>elapsed)return;
            int dealt=target==player?ApplyEnemyAttackToPlayer(damage,owner.x,reaction,280):
                ApplyHit(target,damage,reaction,owner.x,280,false);
            if(dealt>0)h.nextHit[target]=elapsed+gap;
        }

        void PeitaTrapContacts(Enemy e,PeitaHazardState h,float[] points,int damage,HitReaction reaction,float gap) {
            // nativeDepth is measured in source units relative to the projected route rail.
            float depth=e.nativeDepth*.28f;
            if(PeitaSphereHit(points,e.x,e.y,e.facing,depth,PeitaPlayerBox()))
                PeitaAllTeamHit(e,h,player,damage,reaction,gap);
            foreach(var other in enemies) {
                if(other==e || other.hp<=0 || !other.active || other.dormant || other.passive || other.isGate ||
                    EnemyHasNpcInvincibility(other))continue;
                if(PeitaSphereHit(points,e.x,e.y,e.facing,depth-other.nativeDepth*.28f,EnemyBounds(other)))
                    PeitaAllTeamHit(e,h,other,damage,reaction,gap);
            }
        }

        bool TickPeitaEnvironment(Enemy e,float dt) {
            bool column=e.kind=="trap_column",chain=e.kind=="trap_chain"||e.kind=="trap_chain_red";
            bool golem=e.kind=="golem_stone_rage"||e.kind=="golem_stone",gargoyle=e.kind=="gargoyle_stone";
            bool whip=e.kind=="plant_whip_a"&&e.siegeMode;
            if(!column&&!chain&&!golem&&!gargoyle&&!whip)return false;
            if(!peitaBehaviors.TryGetValue(e.kind,out var row))return false;
            var state=e.peitaAttack??(e.peitaAttack=new PeitaAttackState());
            var h=state.hazard;
            if(h==null) {
                state.hazard=h=new PeitaHazardState();
                var support=Surface(e.groundLine);
                h.floor=support!=null?support.Y(e.x):GroundAt(e.x);
                if(golem && !e.siegeMode) {
                    h.phase=row.phases==null?5:4;
                    if(h.phase==4)BeginPeitaMove(e,row.phases[0],false);
                }
            }
            if(whip && h.phase<2 && row.phases!=null) {
                if(h.phase==0) {
                    h.phase=1;
                    BeginPeitaMove(e,row.phases[0],false);
                }
                state.elapsed+=dt;EmitPeitaAttackSounds(e,state);
                if(Math.Abs(player.x-e.x)>140){e.telegraph=0;return true;}
                h.phase=2;e.alerted=true;
                BeginPeitaMove(e,row.actions[0],false);
                return true;
            }
            if(whip)return false;
            if(golem)return TickPeitaRageStone(e,row,h,dt);
            if(gargoyle)return TickPeitaGargoyle(e,row,h,dt);
            if(row.actions[0].trace==null)return false;
            e.vx=e.vy=0;e.state=CombatState.Normal;
            if(column && h.phase==2)return true;
            if(state.attack==null) {
                if(column && Math.Abs(player.x-e.x)>420)return true;
                BeginPeitaMove(e,row.actions[column?0:h.phase],false);
                h.nextHit.Clear();
            }
            var action=state.attack;
            state.elapsed+=dt;EmitPeitaAttackSounds(e,state);
            float t=state.elapsed-action.windup;
            if(t<0)return true;
            PeitaPose(e,action.clip);
            if(t>=(column?.01f:0) && t<=action.trace.life)
                PeitaTrapContacts(e,h,PeitaSpheres(action.trace,t),column?12:35,
                    column?HitReaction.Hurt:HitReaction.Launch,column?.4f:float.PositiveInfinity);
            if(t>=action.duration) {
                h.phase=column?2:1-h.phase;state.attack=null;e.telegraph=0;
                if(column)e.clock=action.duration;
            }
            return true;
        }

        bool PeitaRageStoneProtected(Enemy e) {
            if(e==null||!e.siegeMode||!peitaBehaviors.ContainsKey(e.kind))return false;
            if(e.kind=="plant_whip_a") {
                var hazard=e.peitaAttack?.hazard;
                if(hazard==null||hazard.phase<2)return true;
                var attack=e.peitaAttack.attack;
                return attack!=null && attack==peitaBehaviors[e.kind].actions[0] && e.peitaAttack.elapsed>=attack.windup;
            }
            if(e.kind!="golem_stone"&&e.kind!="golem_stone_rage")return false;
            var h=e.peitaAttack?.hazard;
            if(h!=null)return h.phase<2;
            return e.siegeMode;
        }

        bool TickPeitaRageStone(Enemy e,PeitaBehaviorRow row,PeitaHazardState h,float dt) {
            if(h.phase==5)return false;
            if(h.phase==4 && (e.stun>0 || e.knockdownOnLanding || e.state==CombatState.Knockdown ||
                              e.state==CombatState.GettingUp || e.telegraph<=0)) {
                h.phase=5;e.peitaAttack.attack=null;return false;
            }
            e.state=CombatState.Normal;
            if(h.phase==0) {
                PeitaPose(e,"peita-trap");
                if(Math.Abs(player.x-e.x)>840)return true;
                h.phase=1;h.timer=1.2f;PeitaSourceSound(e,"downstonedust");
            } else if(h.phase==1) {
                h.timer-=dt;
                if(h.timer<=0){h.phase=2;e.vy=-168;}
            } else if(h.phase==2) {
                float before=e.y;e.superArmor=Math.Max(e.superArmor,dt*2);
                e.vy=Math.Max(-560,e.vy-1120*dt);e.y=Math.Max(h.floor,e.y+e.vy*dt);
                if(player.y+42>=e.y && player.y<=before+65 && Math.Abs(player.x-e.x)<38)
                    PeitaAllTeamHit(e,h,player,35,HitReaction.Hurt,float.PositiveInfinity);
                foreach(var target in enemies) {
                    if(target==e || !target.active || target.dormant || target.hp<=0 || target.passive ||
                       target.isGate || EnemyHasNpcInvincibility(target))continue;
                    var box=EnemyBounds(target);
                    if(box.right>=e.x-28 && box.left<=e.x+28 && box.top>=e.y && box.bottom<=before+65)
                        PeitaAllTeamHit(e,h,target,35,HitReaction.Hurt,float.PositiveInfinity);
                }
                if(e.y<=h.floor) {
                    e.vy=0;h.phase=3;h.timer=1;shake=Math.Max(shake,5);
                    SpawnEffect("slam",e.x,e.y+8,e.facing);PeitaSourceSound(e,"downstone");
                }
            } else if(h.phase==3) {
                h.timer-=dt;e.superArmor=Math.Max(e.superArmor,dt*2);
                if(h.timer<=0){h.phase=4;h.timer=0;if(row.phases!=null)BeginPeitaMove(e,row.phases[0],false);}
            } else {
                h.timer+=dt;
                var wake=e.peitaAttack.attack;
                if(wake!=null){e.peitaAttack.elapsed=h.timer;EmitPeitaAttackSounds(e,e.peitaAttack);}
                if(wake==null || h.timer>=wake.duration){h.phase=5;e.peitaAttack.attack=null;e.telegraph=0;PeitaPose(e,"idle");}
            }
            return true;
        }

        bool TickPeitaGargoyle(Enemy e,PeitaBehaviorRow row,PeitaHazardState h,float dt) {
            if(!e.aiEnabled || e.stun>0 || e.knockdownOnLanding || e.state==CombatState.Knockdown ||
               e.state==CombatState.GettingUp)return false;
            var state=e.peitaAttack;
            e.vx=e.vy=0;e.state=CombatState.Normal;
            h.timer+=dt;
            float dx=player.x-e.x;
            if(Math.Abs(dx)<=e.aggroRange)e.alerted=true;
            if(state.attack!=null && e.telegraph<=0)state.attack=null;
            float step=0;
            if(e.alerted && state.attack==null){
                e.facing=dx>=0?1:-1;
                step=Clamp(Math.Abs(dx)-112,-140*dt,140*dt);
                e.x=ConstrainEncounterX(e,Clamp(e.x+e.facing*step,22,RoomWidth-26));
            }
            h.floor=FlightFloor(e.x,h.floor,step);
            float altitude=e.alerted?Math.Max(h.floor+42,player.y+48):h.floor+42;
            e.y+=Clamp(altitude-e.y,-90*dt,90*dt);
            e.groundLine=-1;
            if(!e.alerted){PeitaPose(e,"idle");return true;}
            if(state.attack!=null){TickPeitaEnemy(e,dt);return true;}
            e.cooldown-=dt;
            if(Math.Abs(dx)<=168 && Math.Abs(player.y-e.y)<=90 && e.cooldown<=0)
                BeginPeitaMove(e,row.actions[0],false);
            else PeitaPose(e,Math.Abs(step)>.05f?"walk":"idle");
            return true;
        }
    }
}
