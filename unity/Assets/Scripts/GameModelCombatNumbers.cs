using System;

namespace ElswordPixel {
    public enum CombatNumberKind { Damage, Critical, Health, Mana }

    public sealed partial class GameModel {
        public const float DefaultCriticalChance=.20f;

        int CriticalDamage(int baseDamage,float chance,out bool critical) {
            critical=baseDamage>0&&chance>0&&SkillRandom()<chance;
            return critical?(int)Math.Round(baseDamage*1.5f):baseDamage;
        }

        public int ApplyEnemyAttackToPlayer(int damage,float fromX,HitReaction reaction=HitReaction.Hurt,float launchVelocity=270) {
            if(player.hp<=0||player.invincible>0||AwakeningCasting)return 0;
            damage=CriticalDamage(damage,DefaultCriticalChance,out bool critical);
            return HitPlayer(damage,fromX,reaction,launchVelocity,critical);
        }

        void SpawnDamageNumber(Fighter target,int amount,bool critical) {
            var foe=target as Enemy;
            SpawnNumberAt(target.x,target.y+55,foe==null?-1:1,amount,
                critical?CombatNumberKind.Critical:CombatNumberKind.Damage,foe!=null?foe.nativeDepth:0);
        }

        Effect SpawnNumberAt(float x,float y,int facing,int amount,CombatNumberKind kind,float depth=0) {
            // Separate simultaneous hits on the same target without jittering their values every frame.
            int lane=0;
            for(int i=effects.Count-1;i>=0;i--){
                var other=effects[i];
                if(other.type=="number"&&other.age<.28f&&Math.Abs(other.x-x)<18&&Math.Abs(other.y-y)<24){
                    lane=(other.numberLane+1)%4;break;
                }
            }
            var fx=new Effect{type="number",x=x,y=y,facing=facing,amount=amount,numberKind=kind,
                numberDepth=depth,numberLane=lane,lifetime=.86f};
            effects.Add(fx);
            return fx;
        }

        void SpawnRecoveryNumber(bool health,float amount,bool percent) {
            SpawnRecoveryNumber(player,health,amount,percent);
        }

        void SpawnRecoveryNumber(Fighter target,bool health,float amount,bool percent) {
            if(!(amount>0))return;
            var foe=target as Enemy;
            var fx=SpawnNumberAt(target.x,target.y+(health?83:47),foe==null?-1:1,0,
                health?CombatNumberKind.Health:CombatNumberKind.Mana,foe!=null?foe.nativeDepth:0);
            fx.recoveryAmount=amount;
            fx.numberPercent=percent;
        }

        void RecoverEnemyHealth(Enemy target,float points,float percent=0) {
            float gain=Math.Min(target.maxHp-target.hp,points);
            if(gain<=0)return;
            target.hp+=gain;
            SpawnRecoveryNumber(target,true,percent>0?percent:gain,percent>0);
        }

        void SpawnResourceDamage(Fighter target,float amount,bool critical=false) {
            if(!(amount>0))return;
            if(amount==Math.Floor(amount)){SpawnDamageNumber(target,(int)amount,critical);return;}
            var foe=target as Enemy;
            var fx=SpawnNumberAt(target.x,target.y+55,foe==null?-1:1,0,critical?CombatNumberKind.Critical:CombatNumberKind.Damage,foe!=null?foe.nativeDepth:0);
            fx.hasFractionalAmountR901=true;fx.fractionalAmountR901=amount;
        }

        void RecoverBetweenRooms(int next) {
            float hp=player.hp;
            mana=Math.Min(MaxMana,mana+60);player.hp=Math.Min(player.maxHp,player.hp+60);
            EnterRoom(next);
            SpawnRecoveryNumber(true,player.hp-hp,false);
        }

        void TickManaRecovery(float dt) {
            mana=Math.Min(MaxMana,mana+dt*7);
        }
    }
}
