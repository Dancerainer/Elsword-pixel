using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    [Serializable] public sealed class AwakeningEvent
    { public string type;public float time,duration;public int orbs; }

    public sealed partial class GameModel
    {
        // 30 seconds is target2016 INIT_COMPONENT.MAX_HYPER_MODE_TIME.
        // Other constants below are the explicitly qualified shared fixed-V5
        // consumer in lk-profile, not measurements of the original 2016 client.
        public const int AwakeningMaxOrbs=3;
        public const float AwakeningChargePerOrb=100,AwakeningSecondsPerOrb=30,AwakeningPowerMultiplier=1.2f;
        public const float AwakeningApplyTime=.83f,AwakeningDefaultCastDuration=1.2f;
        public readonly List<AwakeningEvent> awakeningEvents=new List<AwakeningEvent>();
        public int AwakeningOrbs {get;private set;}
        public float AwakeningGauge {get;private set;}
        public float AwakeningRemaining {get;private set;}
        public float AwakeningDuration {get;private set;}
        public int AwakeningActivationSerial {get;private set;}
        public bool AwakeningCasting {get;private set;}
        public bool AwakeningActive {get{return AwakeningRemaining>0;}}
        public bool IsAwakened {get{return AwakeningActive;}}
        public float AwakeningCastTime {get{return AwakeningCasting?player.clock:0;}}
        public bool PlayerHasSuperArmor {get{return player.superArmor>0||armor>0||CommandSuperArmorActive;}}
        public bool PlayerIsInvulnerable {get{return player.invincible>0||AwakeningCasting;}}
        private int awakeningInitialOrbs,awakeningPendingOrbs;
        private float awakeningInitialGauge;
        private bool awakeningApplied,awakeningStopApplied;
        public bool CanAwaken
        {
            get
            {
                if(paused||victory||player.hp<=0||AwakeningActive||AwakeningCasting||AwakeningOrbs<=0||!Grounded(player)||player.stun>0||player.knockdownOnLanding||player.state==CombatState.Knockdown||player.state==CombatState.GettingUp)return false;
                if(actionSkill!=null)return skillStop<=0&&(SkillRecoveryWindows()&SkillExit.Awaken)!=0;
                if(actionTiming!=null)return !charging&&actionTiming.command!=null&&actionTiming.command.skillCancel>=0&&player.clock>actionTiming.command.skillCancel;
                if(landing>0&&landingTiming!=null&&landingTiming.command!=null)return landingTiming.command.skillCancel>=0&&player.clock>landingTiming.command.skillCancel;
                return true;
            }
        }
        public void ConfigureAwakening(int initialOrbs,float initialGauge=0)
        {
            if(initialOrbs<0||initialOrbs>AwakeningMaxOrbs||!Finite(initialGauge)||initialGauge<0||initialGauge>=AwakeningChargePerOrb)throw new ArgumentOutOfRangeException("initialOrbs","Awakening requires 0..3 orbs and a partial gauge in [0,100).");
            awakeningInitialOrbs=initialOrbs;awakeningInitialGauge=initialOrbs==AwakeningMaxOrbs?0:initialGauge;ResetAwakening();
        }
        private void ResetAwakening()
        {
            AwakeningOrbs=awakeningInitialOrbs;AwakeningGauge=awakeningInitialGauge;AwakeningRemaining=AwakeningDuration=0;
            AwakeningCasting=awakeningApplied=awakeningStopApplied=false;awakeningPendingOrbs=0;AwakeningActivationSerial=0;awakeningEvents.Clear();
        }
        private void AwakeningEvent(string type,int orbs,float duration=0)
        {awakeningEvents.Add(new AwakeningEvent{type=type,orbs=orbs,time=elapsed,duration=duration});if(awakeningEvents.Count>100)awakeningEvents.RemoveAt(0);}
        private void AddAwakeningCharge(float amount)
        {
            if(amount<=0||player.hp<=0||AwakeningOrbs>=AwakeningMaxOrbs)return;
            AwakeningGauge+=amount;
            while(AwakeningGauge>=AwakeningChargePerOrb&&AwakeningOrbs<AwakeningMaxOrbs)
            {AwakeningGauge-=AwakeningChargePerOrb;AwakeningOrbs++;AwakeningEvent("orb",AwakeningOrbs);}
            if(AwakeningOrbs==AwakeningMaxOrbs)AwakeningGauge=0;
        }
        public bool TryAwaken()
        {
            if(!CanAwaken)return false;
            awakeningPendingOrbs=AwakeningOrbs;AwakeningOrbs=0;CancelAction(true);landing=0;landingTiming=null;hitstop=0;
            AwakeningCasting=true;awakeningApplied=awakeningStopApplied=false;player.vx=player.vy=0;player.clip="awakening";player.clock=0;
            player.invincible=Math.Max(player.invincible,AwakeningCastDuration());lastCommand="AWAKENING";commandDisplay=AwakeningCastDuration();
            AwakeningEvent("casting",awakeningPendingOrbs);return true;
        }
        private float AwakeningCastDuration()
        {ClipTiming t;return clipTimings.TryGetValue("awakening",out t)?t.duration:AwakeningDefaultCastDuration;}
        private void TickAwakening(float dt)
        {
            if(player.hp<=0)
            {
                if(AwakeningActive||AwakeningCasting)AwakeningEvent("ended",0);
                AwakeningRemaining=0;AwakeningCasting=false;awakeningPendingOrbs=0;return;
            }
            if(AwakeningRemaining>0)
            {AwakeningRemaining=Math.Max(0,AwakeningRemaining-dt);if(AwakeningRemaining==0)AwakeningEvent("ended",0);}
        }
        private void AdvanceAwakeningCast()
        {
            if(!AwakeningCasting)return;
            player.vx=player.vy=0;
            if(!awakeningStopApplied&&player.clock>=.03f)
            {awakeningStopApplied=true;RequestWorldStop(1.3f);}
            if(!awakeningApplied&&player.clock>=AwakeningApplyTime)
            {
                awakeningApplied=true;AwakeningDuration=awakeningPendingOrbs*AwakeningSecondsPerOrb;AwakeningRemaining=AwakeningDuration;AwakeningActivationSerial++;
                AwakeningEvent("activated",awakeningPendingOrbs,AwakeningDuration);SpawnEffect("awakening",player.x,player.y+32,player.facing);awakeningPendingOrbs=0;
            }
            if(player.clock>=AwakeningCastDuration())
            {AwakeningCasting=false;player.clip="idle";player.clock=0;AwakeningEvent("cast-ended",0,AwakeningRemaining);}
        }
        private int ApplyAwakeningDamage(int damage)
        {return AwakeningActive?Math.Max(1,(int)Math.Round(damage*AwakeningPowerMultiplier)):damage;}
    }
}
