using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    [Serializable] public class PeitaBehaviorDocument { public PeitaBehaviorRow[] rows; }
    [Serializable] public class PeitaBehaviorRow
    {
        public string id;
        public bool boss, ranged;
        public float walkSpeed, reach;
        public PeitaAttack[] actions;
        public PeitaAttack[] phases, enragedActions;
        public PeitaBowSample[] bowShot;
        public PeitaAttack special;
    }
    [Serializable] public class PeitaBowSample { public float time, low, high, radius; }
    [Serializable] public class PeitaAttack
    {
        public string clip, readyClip, sourceState;
        public bool magic, heavy, contact, readyArmor, attackArmor;
        public string hitReaction;
        public PeitaTrace trace;
        public float launchSpeed;
        public float windup, duration, hitAt;
        public NpcTimedSound[] sounds;
    }
    internal sealed class PeitaAttackState
    {
        public int sequence, nextSound;
        public long soundEntry;
        public PeitaAttack attack;
        public float elapsed, flightTime, landingY, alarmUntil;
        public bool struck, airborneStarted, special, entered, needsArchers, emittedWave;
        public Enemy owner, hands;
        public int templePhase;
        public bool amethystSpecialWalk;
        public float nextTempleCheck;
        public int bossPhase, boltsNeeded=2, boltHits;
        public PeitaHazardState hazard;
        public bool gardenRage;
        public float fleeUntil, escapeClock;
    }

    public sealed partial class GameModel
    {
        readonly Dictionary<string, PeitaBehaviorRow> peitaBehaviors = new Dictionary<string, PeitaBehaviorRow>();
        Enemy peitaHowlingBoss;
        float peitaHowlAlarmUntil;

        // The normal Berthe immune-skill state declares an orange danger alarm
        // for five seconds. Keep its owner tied to the current room's actors.
        public bool PeitaHowlAlarm {
            get {
                return peitaHowlingBoss != null && peitaHowlingBoss.active && peitaHowlingBoss.hp > 0 &&
                       elapsed < peitaHowlAlarmUntil && enemies.Contains(peitaHowlingBoss);
            }
        }

        public void SetPeitaEnemyBehaviors(PeitaBehaviorRow[] rows)
        {
            SetRegionNpcMechanics(null);
            peitaHowlingBoss=null;
            peitaBehaviors.Clear();
            if (rows == null) return;
            foreach (var row in rows)
                if (row != null && row.actions != null && row.actions.Length > 0)
                    peitaBehaviors.Add(row.id, row);
        }

        static bool IsPeitaBoss(string kind)
        {
            switch (kind) {
                case "dullahan": case "golem_boss": case "amethyst":
                case "plant_overload": case "plant_overload_quest": case "cuttysark": case "summon_boss_a": return true;
                default: return false;
            }
        }

        bool IsPeitaCrossbow(Enemy enemy)
        {
            return enemy != null && enemy.kind == "trap_crossbow" && peitaBehaviors.Count > 0;
        }

        static void PeitaPose(Enemy e, string clip)
        {
            if (e.clip == clip) return;
            e.clip = clip;
            e.clock = 0;
        }

        void EmitPeitaAttackSounds(Enemy e, PeitaAttackState state)
        {
            var sounds=state.attack.sounds;
            while(sounds!=null && state.nextSound<sounds.Length &&
                  sounds[state.nextSound].candidateTime<=state.elapsed+.000001f) {
                var cue=sounds[state.nextSound++];
                QueueNpcSound(cue,e,state.soundEntry,0,elapsed-state.elapsed+cue.candidateTime,state.attack.clip);
            }
        }

        void BeginPeitaMove(Enemy e, PeitaAttack a, bool special)
        {
            var state=e.peitaAttack;
            state.attack=a;
            state.elapsed=0;
            state.struck=state.emittedWave=false;
            state.special=special;
            state.nextSound=0;
            state.soundEntry=++npcSoundEntrySerial;
            e.telegraph=a.windup+a.duration;
            e.clock=0;
            PeitaPose(e,a.windup>0?a.readyClip:a.clip);
            EmitPeitaAttackSounds(e,state);
        }

        void PeitaImmuneSkill(SkillDefinition skill)
        {
            // These four equipped-capable source IDs have m_eType 16 (grade 3).
            // INIT_STATE.IMMUNE_SKILL_STATE sends Berthe into the Win/howl motion.
            if (peitaBehaviors.Count==0 || (skill.sourceSkillId!=21007 && skill.sourceSkillId!=21008 &&
                    skill.sourceSkillId!=21028 && skill.sourceSkillId!=21030))return;
            PeitaBehaviorRow row;
            if(!peitaBehaviors.TryGetValue("summon_boss_a",out row)||row.special==null)return;
            foreach(var e in enemies) {
                if(e.kind!=row.id||e.hp<=0||!e.active||e.dormant||!e.aiEnabled)continue;
                if(e.peitaAttack==null)e.peitaAttack=new PeitaAttackState();
                if(e.peitaAttack.special)continue;
                e.state=CombatState.Normal;
                e.stun=e.downTime=e.recoveryTime=e.hitPause=0;
                e.knockdownOnLanding=false;
                e.vx=e.vy=0;
                BeginPeitaMove(e,row.special,true);
                peitaHowlingBoss=e;
                peitaHowlAlarmUntil=elapsed+5f;
            }
        }

        static bool PeitaSpecialProtected(Enemy e)
        {
            return e!=null && e.peitaAttack!=null && e.peitaAttack.special;
        }

        bool PeitaIgnoresSkillStop(Enemy e)
        {
            RegionActorMechanics region;
            if(e!=null&&regionActors.TryGetValue(e.kind,out region))return region.immuneStop;
            if (e == null || !peitaBehaviors.ContainsKey(e.kind)) return false;
            // INIT_COMPONENT.IMMUNITY_TIME_STOP is specific to the source NPC variant.
            switch (e.kind) {
                case "summon_boss_a": case "dullahan": case "golem_boss": case "amethyst": case "amethyst_attacker":
                case "cuttysark": case "plant_overload": case "plant_overload_quest":
                case "gliter_archer_boss": case "gliter_hammer_boss": case "gliter_necromancer_boss": return true;
                default: return false;
            }
        }

        Enemy AddPeitaSummon(Enemy owner,string kind,float x,float y,bool hand)
        {
            var e=new Enemy {kind=kind,sourceNPC="NUI_"+kind.ToUpperInvariant(),x=x,spawnX=x,y=y,
                facing=owner.facing,hp=hand?owner.hp:150,maxHp=hand?owner.maxHp:150,
                groundLine=hand?owner.groundLine:-1,aggroRange=300,aiEnabled=!hand,
                countsForClear=false,passive=hand,active=true,visualScale=owner.visualScale,
                encounterIndex=owner.encounterIndex,nativeDepth=owner.nativeDepth,
                peitaAttack=new PeitaAttackState{owner=owner}};
            enemies.Add(e);
            return e;
        }

        void UpdatePeitaSummons()
        {
            if(peitaBehaviors.Count==0)return;
            // Append between enemy updates, preserving the original spawn order and queued sound handles.
            for(int i=0, count=enemies.Count;i<count;i++) {
                var e=enemies[i];
                var state=e.peitaAttack;
                if(state!=null && state.owner!=null && (state.owner.hp<=0||!state.owner.active||state.owner.dormant)) {
                    e.active=false;
                    e.hp=0;
                    continue;
                }
                if(!e.active || e.dormant || e.hp<=0 || !e.aiEnabled)continue;
                if(e.kind=="amethyst" && (state==null||!state.entered)) {
                    PeitaBehaviorRow body,hand;
                    if(!peitaBehaviors.TryGetValue("amethyst",out body)||body.special==null||
                       !peitaBehaviors.TryGetValue("amethyst_attacker",out hand))continue;
                    if(state==null)e.peitaAttack=state=new PeitaAttackState();
                    state.entered=true;
                    state.hands=AddPeitaSummon(e,"amethyst_attacker",e.x,e.y,true);
                    BeginPeitaMove(e,body.special,true);
                }
                if(state==null||!state.needsArchers)continue;
                state.needsArchers=false;
                int live=enemies.FindAll(a=>a.kind=="gliter_archer_elite"&&a.active&&!a.dormant&&a.hp>0).Count;
                for(int n=live;n<4;n++) {
                    // Project each source random-position request onto this arena's support geometry.
                    float x=Clamp(e.spawnX+(SkillRandom()-.5f)*510,36,RoomWidth-36);
                    var floor=NativeTerrain?SurfaceBelow(x,e.y+95):null;
                    var add=AddPeitaSummon(e,"gliter_archer_elite",x,floor!=null?floor.Y(x):e.y,false);
                    if(floor!=null)add.groundLine=floor.id;
                }
            }
        }

        bool MirrorPeitaHands(Enemy e)
        {
            if(e.kind!="amethyst_attacker" || e.peitaAttack==null || e.peitaAttack.owner==null)return false;
            var owner=e.peitaAttack.owner;
            e.active=owner.active&&!owner.dormant&&owner.hp>0;
            e.hp=owner.hp;
            e.x=owner.x;e.y=owner.y;e.facing=owner.facing;e.nativeDepth=owner.nativeDepth;
            e.clip=owner.clip;e.clock=owner.clock;e.groundLine=owner.groundLine;
            e.state=owner.state;
            return true;
        }

        void TickPeitaSpecial(Enemy e,float dt)
        {
            var state=e.peitaAttack;
            state.elapsed+=dt;
            EmitPeitaAttackSounds(e,state);
            if(e.kind=="summon_boss_a") {
                if(!state.emittedWave && state.elapsed>=1.1666f) {
                    state.emittedWave=true;
                    SpawnEffect("slam",e.x,e.y+18,e.facing);
                    shake=Math.Max(shake,6);
                }
                // Main effect starts at 1.221 s with a 50-unit cube at scale 12.
                if(!state.struck && state.elapsed>=1.221f && state.elapsed<=4.221f &&
                    Math.Abs(player.x-e.x)<=84+10 && Math.Abs(player.y+25-e.y-360*.28f)<=84+25) {
                    if(ApplyEnemyAttackToPlayer(82,e.x,HitReaction.Launch,290)>0)state.struck=true;
                }
                if(state.elapsed-dt<3.1f && state.elapsed>=3.1f && e.hp<e.maxHp*.5f)
                    state.needsArchers=true;
            }
            if(state.elapsed>=state.attack.duration) {
                state.special=false;state.attack=null;e.telegraph=0;e.cooldown=.7f;
                PeitaPose(e,"idle");
            }
        }

        bool TickPeitaDragonFlight(Enemy e, float dt)
        {
            if(e.kind!="cuttysark" || !e.aiEnabled || !peitaBehaviors.ContainsKey(e.kind))return false;
            if(e.peitaAttack==null)e.peitaAttack=new PeitaAttackState();
            var state=e.peitaAttack;
            if(!state.airborneStarted) {
                state.landingY=e.y;
                state.airborneStarted=true;
            }
            if(TickPeitaDragonPhase(e,dt))return true;
            state.flightTime+=dt;
            e.vx=e.vy=0;
            e.state=CombatState.Normal;
            float flightPeriod=state.flightTime%9;
            // Circle above the four crossbows, then dive into reach before climbing again.
            float target=flightPeriod<5.0f ? state.landingY+78 : state.landingY+8;
            float horizontal=flightPeriod<5.0f ? e.spawnX+185*(float)Math.Sin(state.flightTime*.85f) : player.x;
            e.y+=Clamp(target-e.y,-90*dt,65*dt);
            if(state.attack==null)
                e.x=Clamp(e.x+Clamp(horizontal-e.x,-95*dt,95*dt),22,RoomWidth-26);
            if(flightPeriod>=5.0f || state.attack!=null)TickPeitaEnemy(e,dt);
            else {
                e.facing=player.x>=e.x?1:-1;
                PeitaPose(e,"walk");
            }
            return true;
        }

        bool TickPeitaEnemy(Enemy e, float dt)
        {
            PeitaBehaviorRow row;
            if (!peitaBehaviors.TryGetValue(e.kind, out row)) return false;
            if (e.peitaAttack == null) e.peitaAttack = new PeitaAttackState();
            var state = e.peitaAttack;
            if (TickPeitaGardenRage(e,row))return true;
            if (TickPeitaTempleBoss(e,row,dt))return true;
            if (e.kind == "mana_eater" && TickPeitaManaEater(e, row, dt)) return true;
            float dx = player.x - e.x;
            if (!e.alerted && Math.Abs(dx) > e.aggroRange) return true;
            e.alerted = true;
            if (state.attack != null && e.telegraph <= 0) state.attack = null;
            if (state.attack != null) {
                var a = state.attack;
                state.elapsed += dt;
                EmitPeitaAttackSounds(e,state);
                bool spell=EmitPeitaSpell(e,a,state.elapsed-dt,state.elapsed);
                float strike = state.elapsed - a.windup;
                if (strike >= 0) {
                    PeitaPose(e, a.clip);
                    TickTeachFollowups(e,row,a,strike,dt);
                    bool lantern = TickPeitaLanternStrike(e, state, strike, dt, a.heavy ? 26 : 21);
                    if (!spell && !lantern && !state.struck && strike >= Math.Min(a.hitAt, a.duration*.95f)) {
                        state.struck = true;
                        int damage = row.boss ? 48 : 21;
                        if (a.heavy) damage += row.boss ? 12 : 5;
                        bool poison = PeitaPoisonMove(e, a);
                        if (poison || ((row.ranged || a.magic) && !a.contact)) {
                            effects.Add(new Effect { type=poison ? "peita-poison" : "enemy-bolt", x=e.x+20*e.facing, y=e.y+28,
                                facing=e.facing, velocityX=e.facing*(a.magic ? 185 : 245), hostileDamage=damage, lifetime=2.2f });
                        } else if (Math.Abs(dx) <= row.reach+18 && e.facing*dx >= -16 &&
                                Math.Abs(player.y-e.y) < (row.boss ? 70 : 44) && EnemyAccuracyAllowsHit(e)) {
                            ApplyEnemyAttackToPlayer(damage, e.x, PeitaContactReaction(a), a.launchSpeed > 0 ? a.launchSpeed : 270);
                        }
                    }
                }
                if (strike >= a.duration) {
                    state.attack = null;
                    e.telegraph = 0;
                    e.cooldown = row.boss ? .8f : .65f;
                    PeitaPose(e,state.gardenRage ? "peita-rage-idle" : "idle");
                }
                return true;
            }
            e.cooldown -= dt;
            e.facing = dx >= 0 ? 1 : -1;
            if (Math.Abs(dx) <= row.reach && Math.Abs(player.y-e.y) < (row.boss ? 76 : 48) && e.cooldown <= 0) {
                var choices=state.gardenRage ? row.enragedActions : row.actions;
                var a = PickPeitaAttack(e,choices);
                BeginPeitaMove(e,a,false);
            } else if (Math.Abs(dx) > row.reach*.8f && row.walkSpeed > 0) {
                MoveAlongGround(e,Clamp(e.x+e.facing*row.walkSpeed*dt,22,RoomWidth-26));
                PeitaPose(e,"walk");
            } else PeitaPose(e,state.gardenRage ? "peita-rage-idle" : "idle");
            return true;
        }
    }
}
