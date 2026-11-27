using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        const int KnightGuardReady = 1, KnightGuardHold = 2, KnightGuardEnd = 3;
        const int KnightCounterReady = 4, KnightCounter = 5, KnightCombo = 6;
        const int KnightAttackCReady = 7, KnightAttackC = 8;

        void TemplePhase(Enemy enemy, PeitaBehaviorRow row, int phase) {
            var state = enemy.peitaAttack;
            state.templePhase = phase;
            if (phase != 0) BeginPeitaMove(enemy, row.phases[phase-1], false);
            else {
                state.attack = null;
                enemy.telegraph = 0;
                enemy.cooldown = .8f;
                PeitaPose(enemy,"idle");
            }
        }

        int GuardDurahan(Enemy e, int damage, float attackerX, ref HitReaction reaction, ref bool shieldContact) {
            if (e.kind != "dullahan" || e.peitaAttack == null || (attackerX-e.x)*e.facing < 0) return damage;
            int phase = e.peitaAttack.templePhase;
            if (phase != KnightGuardReady && phase != KnightGuardHold) return damage;
            reaction = HitReaction.None;
            shieldContact=true;
            SpawnEffect("guard",e.x,e.y+25,e.facing);
            return Math.Max(1,(int)Math.Ceiling(damage*.3));
        }

        void ReactDurahanToDamage(Enemy e, HitReaction impact) {
            if (e.kind != "dullahan" || e.state != CombatState.Hurt || impact != HitReaction.Hurt || e.hp<=0) return;
            PeitaBehaviorRow row;
            if (!peitaBehaviors.TryGetValue(e.kind,out row) || row.phases==null || row.phases.Length!=8) return;
            var s = e.peitaAttack ?? (e.peitaAttack = new PeitaAttackState());
            if (s.templePhase!=0 || elapsed<=Math.Max(1,s.nextTempleCheck) ||
                Math.Abs(player.x-e.x)>500*.28f) return;
            // DULLAHAN_DAMAGE_STATE_START_FUNC rolls integers 1..100 once
            // its one-second hit timer has expired, then restarts the timer.
            s.nextTempleCheck = elapsed+1;
            int roll = 1+(int)(SkillRandom()*100);
            int phase = roll<10 && e.hp<=.7f*e.maxHp ? KnightGuardReady :
                        roll>=10 && roll<15 && e.hp<=.4f*e.maxHp ? KnightAttackCReady : 0;
            if (phase==0) return;
            e.state=CombatState.Normal;
            e.stun=e.downTime=e.recoveryTime=e.vx=e.vy=0;
            e.knockdownOnLanding=false;
            TemplePhase(e,row,phase);
        }

        bool TickPeitaTempleBoss(Enemy e, PeitaBehaviorRow row, float dt) {
            var state=e.peitaAttack;
            if (row.phases==null) return false;
            if (e.kind=="amethyst" && row.phases.Length==2)
                return TickAmethystRelocation(e,row,dt);
            if (e.kind!="dullahan" || state.templePhase==0 || row.phases.Length!=8) return false;
            int phase=state.templePhase;
            var move=row.phases[phase-1];
            state.elapsed+=dt;
            EmitPeitaAttackSounds(e,state);
            if (phase==KnightGuardHold) e.clock=state.elapsed%move.duration;
            if (phase==KnightGuardHold && state.elapsed<move.duration*3 && elapsed>=state.nextTempleCheck) {
                state.nextTempleCheck=elapsed+.03f;
                if (Math.Abs(player.x-e.x)<=140 && SkillRandom()<.5f) {
                    TemplePhase(e,row,KnightCounterReady);
                    return true;
                }
            }
            if ((phase==KnightCounter || phase==KnightCombo || phase==KnightAttackC) && !state.struck &&
                state.elapsed>=move.hitAt && Math.Abs(player.y-e.y)<70 &&
                (player.x-e.x)*e.facing>=-16 && Math.Abs(player.x-e.x)<=row.reach+18) {
                state.struck=ApplyEnemyAttackToPlayer(52,e.x,PeitaContactReaction(move))>0;
            }
            if (phase==KnightCounter && state.struck && state.elapsed>=.7f) {
                TemplePhase(e,row,KnightCombo);
                return true;
            }
            if (state.elapsed<move.duration*(phase==KnightGuardHold?3:1)) return true;
            switch (phase) {
                case KnightGuardReady: TemplePhase(e,row,KnightGuardHold); state.nextTempleCheck=elapsed+.03f; break;
                case KnightGuardHold: TemplePhase(e,row,KnightGuardEnd); break;
                case KnightCounterReady: TemplePhase(e,row,KnightCounter); break;
                case KnightAttackCReady: TemplePhase(e,row,KnightAttackC); break;
                default: TemplePhase(e,row,0); state.nextTempleCheck=elapsed+1; break;
            }
            return true;
        }

        bool TickAmethystRelocation(Enemy e, PeitaBehaviorRow row, float dt) {
            var state=e.peitaAttack;
            if (state.templePhase==0) {
                if (state.attack!=null || !e.alerted) return false;
                e.cooldown-=dt;
                if (e.cooldown>0 || elapsed<state.nextTempleCheck) return true;
                state.nextTempleCheck=elapsed+.03f;
                float dx=Math.Abs(player.x-e.x);
                bool walk = dx>112 && SkillRandom()<.1f || dx<=168 && SkillRandom()<.1f;
                state.amethystSpecialWalk = !walk && dx<=99999*.28f && SkillRandom()<.3f;
                if (!walk && !state.amethystSpecialWalk) {
                    if (dx<=168 && SkillRandom()<.1f) {
                        e.facing=player.x>=e.x?1:-1;
                        BeginPeitaMove(e,row.actions[state.sequence++%2],false);
                    }
                    return true;
                }
                TemplePhase(e,row,1);
            }
            state.elapsed+=dt;
            EmitPeitaAttackSounds(e,state);
            if (state.elapsed<state.attack.duration) return true;
            if (state.templePhase==1) {
                // AMETHYST_ARM_WALK_END_STATE_START selects a side of the
                // hero and a line-map point within 800 native units on it.
                // SPECIAL_WALK_END instead requests GetStartPosition(1), the
                // chapel stage's authored spawn for NUI_AMETHYST itself.
                float x=e.spawnX;
                int facing=-1;
                if (!state.amethystSpecialWalk) {
                    int side=SkillRandom()<.5f?-1:1;
                    x=ConstrainEncounterX(e,NativeClampX(player.x+side*800*.28f*SkillRandom()));
                    facing=-side;
                }
                var floor=SurfaceBelow(x,10000);
                if (floor!=null) {e.x=x;e.y=floor.Y(x);e.groundLine=floor.id;e.facing=facing;}
                TemplePhase(e,row,2);
            } else {
                TemplePhase(e,row,0);
                state.nextTempleCheck=elapsed+.03f;
                if (state.amethystSpecialWalk) e.facing=player.x>=e.x?1:-1;
                BeginPeitaMove(e,row.actions[state.amethystSpecialWalk?2:0],false);
                state.amethystSpecialWalk=false;
            }
            return true;
        }

        void TickTeachFollowups(Enemy e, PeitaBehaviorRow row, PeitaAttack move, float strike, float dt) {
            if (e.kind!="golem_boss") return;
            bool second=move.clip=="peita-2" && strike>=1.428f && strike-dt<1.428f;
            bool last=move.clip=="peita-2" && strike>=3.41f && strike-dt<3.41f;
            bool quake=move.clip=="peita-3" && strike>=1.452f && strike-dt<1.452f;
            if (!second && !last && !quake) return;
            float x=e.x+e.facing*row.reach*.5f;
            SpawnEffect("slam",x,e.y,e.facing);
            shake=Math.Max(shake,quake?6:4);
            if (Math.Abs(player.x-x)<=row.reach*.65f && Math.Abs(player.y-e.y)<60)
                ApplyEnemyAttackToPlayer(quake?18:48,e.x,last?HitReaction.Launch:HitReaction.Hurt);
        }
    }
}
