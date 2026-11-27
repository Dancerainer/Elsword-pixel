using System;

namespace ElswordPixel
{
    public sealed partial class GameModel
    {
        public string PeitaArenaAlarm {
            get {
                if (PeitaHowlAlarm) return "DANGER   BERTHE'S HOWL";
                foreach (var e in enemies) {
                    var state=e.peitaAttack;
                    if (state==null || e.hp<=0 || !e.active || e.dormant || elapsed>=state.alarmUntil) continue;
                    if (state.gardenRage) return "DANGER   PLANT OVERLORD ENRAGED";
                    if (e.kind=="cuttysark") return "CUTTYSARK IS DOWN";
                }
                return "";
            }
        }

        bool PeitaRootedBoss(Enemy e)
        {
            return e != null && (e.kind == "plant_overload" || e.kind == "plant_overload_quest")
                && peitaBehaviors.ContainsKey(e.kind);
        }

        bool PeitaTowerDragon(Enemy e)
        {
            return e != null && e.kind == "cuttysark" && peitaBehaviors.ContainsKey(e.kind);
        }

        bool TickPeitaGardenRage(Enemy e, PeitaBehaviorRow row)
        {
            if (!PeitaRootedBoss(e) || row.phases == null || row.enragedActions == null) return false;
            var state = e.peitaAttack;
            if (state.gardenRage || e.kind != "plant_overload" || state.attack != null) return false;
            // CF_PLANT_OVERLOAD_CENTER_CHANGE_OVERMODE counts these two distinct IDs.
            foreach (var ally in enemies) {
                if (ally == e || ally.hp <= 0 || ally.dormant || !ally.active) continue;
                if (ally.kind == "plant_overload" || ally.kind == "plant_overload_quest") return false;
            }
            state.gardenRage = true;
            state.alarmUntil = elapsed+5;
            state.sequence = 0;
            e.hp = Math.Min(e.maxHp, e.hp + 2800); // Normal difficulty is 1.
            BeginPeitaMove(e, row.phases[0], false);
            return true;
        }

        // The source uses ordinary damage to charge a launcher, and restores its HP on firing.
        int TriggerPeitaCrossbow(Enemy e, int damage)
        {
            var row = peitaBehaviors[e.kind];
            if (!e.active || e.dormant || damage <= 0 || row.phases == null) return 0;
            var state = e.peitaAttack;
            if (state == null || state.bossPhase == 1 || state.bossPhase == 3) return 0;
            int dealt = Math.Min(damage, (int)Math.Ceiling(e.hp));
            e.hp -= dealt;
            SpawnResourceDamage(e, dealt, false);
            if (e.hp < e.maxHp * .05f) {
                e.hp = e.maxHp;
                StartPeitaPhase(e, row, 3);
            } else StartPeitaPhase(e, row, 2);
            return dealt;
        }

        void StartPeitaPhase(Enemy e, PeitaBehaviorRow row, int phase)
        {
            e.peitaAttack.bossPhase = phase;
            if (e.kind=="cuttysark" && phase==4) e.peitaAttack.alarmUntil=elapsed+5;
            BeginPeitaMove(e, row.phases[phase - 1], false);
            e.telegraph = 0;
        }

        void EndPeitaPhase(Enemy e)
        {
            e.peitaAttack.bossPhase = 0;
            e.peitaAttack.attack = null;
            e.telegraph = 0;
            PeitaPose(e, "idle");
        }

        void TickPeitaCrossbow(Enemy e, float dt)
        {
            var row = peitaBehaviors[e.kind];
            if (row.phases == null || row.bowShot == null) return;
            if (e.peitaAttack == null) {
                e.peitaAttack = new PeitaAttackState();
                StartPeitaPhase(e, row, 1);
            }
            var state = e.peitaAttack;
            if (state.bossPhase == 0) return;
            float previous = state.elapsed;
            state.elapsed += dt;
            EmitPeitaAttackSounds(e,state);
            if (state.bossPhase != 3) {
                if (state.elapsed >= state.attack.duration) EndPeitaPhase(e);
                return;
            }
            if (!state.struck && row.bowShot.Length > 0 && previous <= row.bowShot[row.bowShot.Length-1].time) {
                foreach (var dragon in enemies) {
                    if (dragon.kind != "cuttysark" || dragon.hp <= 0 || !dragon.active || dragon.dormant) continue;
                    if (!PeitaBoltIntersects(e, dragon, row.bowShot, previous, state.elapsed)) continue;
                    state.struck = true;
                    HitPeitaDragonWithBolt(dragon, e);
                    break;
                }
            }
            if (state.elapsed >= 7) StartPeitaPhase(e, row, 1);
        }

        static bool PeitaBoltIntersects(Enemy bow, Enemy dragon, PeitaBowSample[] samples, float start, float end)
        {
            // In the pixel projection the three spheres travel along the bow's X.
            // Sweep between source samples so a fast bolt cannot tunnel through the dragon.
            float x = Math.Abs(dragon.x - bow.x);
            float y = dragon.y - bow.y;
            for (int i = 1; i < samples.Length; i++) {
                var a = samples[i-1];
                var b = samples[i];
                if (a.time > end) break;
                if (b.time < start) continue;
                float from = Clamp((start-a.time)/(b.time-a.time),0,1);
                float to = Clamp((end-a.time)/(b.time-a.time),0,1);
                float low = a.low + from * (b.low-a.low);
                float high = a.high + to * (b.high-a.high);
                // CUTTYSARK.INIT_SYSTEM declares width 150 and height 300 native units.
                if (x <= 21 + Math.Max(a.radius,b.radius) && y <= high && y+84 >= low) return true;
            }
            return false;
        }

        void HitPeitaDragonWithBolt(Enemy e, Enemy bow)
        {
            PeitaBehaviorRow row;
            if (!peitaBehaviors.TryGetValue(e.kind, out row) || row.phases == null) return;
            var state = e.peitaAttack ?? (e.peitaAttack = new PeitaAttackState());
            ApplyHitToEnemyFromXR1417(e,78,HitReaction.None,0,bow.x);
            if (e.hp <= 0 || (state.bossPhase >= 1 && state.bossPhase <= 6)) return;
            if (++state.boltHits < state.boltsNeeded) {
                StartPeitaPhase(e,row,7);
                return;
            }
            state.boltHits = 0;
            StartPeitaPhase(e,row,1);
        }

        bool TickPeitaDragonPhase(Enemy e, float dt)
        {
            var state = e.peitaAttack;
            int phase = state.bossPhase;
            if (phase == 0) return false;
            var row = peitaBehaviors[e.kind];
            state.elapsed += dt;
            EmitPeitaAttackSounds(e,state);
            if (phase == 2) {
                e.y = Math.Max(state.landingY, e.y - 220 * dt);
                if (e.y > state.landingY) return true;
                SpawnEffect("slam",e.x,e.y,e.facing);
                shake = Math.Max(shake, 6);
            } else {
                // CF_CUTTYSARK_FLY_STANDUP waits 5, 6 or 7 s for 2, 3 or 4 required bolts.
                if (phase == 6) e.y = Math.Min(state.landingY+78,e.y+220*dt);
                float duration = phase == 4 ? state.boltsNeeded+3 : state.attack.duration;
                if (state.elapsed < duration) return true;
            }
            if (phase == 4) state.boltsNeeded = 2 + Math.Min(2,(int)(SkillRandom()*3));
            if (phase == 6 || phase == 7) {
                if (phase == 6) state.flightTime=0;
                EndPeitaPhase(e);
            } else StartPeitaPhase(e,row,phase+1);
            return true;
        }
    }
}
