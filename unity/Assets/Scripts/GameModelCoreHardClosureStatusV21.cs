using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  readonly double[] coreHardDotUntilV21=new double[2],coreHardDotNextV21=new double[2];readonly float[] coreHardDotPowerV21=new float[2];float coreHardColdUntilV21,coreHardStunUntilV21;int coreHardStunSubsequentV21,coreHardStunInitialSerialV21;
  public float CoreHardColdRemainingV21{get{return CoreHardClosureScopeV21()?Math.Max(0,coreHardColdUntilV21-elapsed):0;}}
  public float CoreHardStunRemainingV21{get{return CoreHardClosureScopeV21()?Math.Max(0,coreHardStunUntilV21-elapsed):0;}}
  public int CoreHardStunSubsequentHitsV21{get{return coreHardStunSubsequentV21;}}
  void ClearCoreHardClosureStatusesV21(){ClearCoreHardDotSupportOwnersR1417();for(int i=0;i<2;i++)coreHardDotUntilV21[i]=coreHardDotNextV21[i]=coreHardDotPowerV21[i]=0;coreHardColdUntilV21=coreHardStunUntilV21=0;coreHardStunSubsequentV21=coreHardStunInitialSerialV21=0;}
  void OnCoreHardPlayerDamageV21(){if(!CoreHardClosureScopeV21()||CoreHardStunRemainingV21<=0||PlayerImpactSerial==coreHardStunInitialSerialV21)return;coreHardStunSubsequentV21++;CoreClosureEventV21("stun-subsequent-real-hit",null,null,0,null,0,true,"Applying hit excluded; subsequent PlayerImpactSerial="+PlayerImpactSerial);if(coreHardStunSubsequentV21>=2){coreHardStunUntilV21=0;CoreClosureEventV21("stun-finalize-two-subsequent-hits",null,null,0,null,0,true);}}
  void ApplyCoreHardCasterStatusV21(NpcEffectInstance fx,CoreHardClosureRuleV21 r,float at){
   if(!CoreHardClosureScopeV21()||player.hp<=0)return;
   if(r.burnCoefficient>0&&r.statusDuration>0){int slot=r.coldSpeed<1?1:0;bool refresh=coreHardDotUntilV21[slot]>at&&coreHardDotPowerV21[slot]>0;coreHardDotUntilV21[slot]=at+r.statusDuration;coreHardDotPowerV21[slot]=(fx.physicalAttack+fx.magicAttack)*.5f*r.burnCoefficient;coreHardDotSupportOwnersR1417[slot]=fx.owner;if(!refresh)coreHardDotNextV21[slot]=at+1;CoreClosureEventV21(slot==0?"source-fire-status":"source-cold-status",fx.owner,fx,0,"BUFF_FACTOR",0,true,"Author2D MIX_ATTACK=(P+M)/2; source6seconds/coefficient retained",at);}
   if(r.coldSpeed<1&&r.statusDuration>0){coreHardColdUntilV21=at+r.statusDuration;CoreClosureEventV21("source-cold-action-speed085",fx.owner,fx,0,"BBT_CHANGE_ANIM_SPEED",0,true,null,at);}
   if(r.stunDuration>0){coreHardStunUntilV21=at+r.stunDuration;coreHardStunSubsequentV21=0;coreHardStunInitialSerialV21=PlayerImpactSerial;CoreClosureEventV21("source-stun-two-seconds-or-two-later-hits",fx.owner,fx,0,"BBT_STUN",0,true,"Initial applying damage serial="+PlayerImpactSerial,at);}
  }
  void TickCoreHardClosureStatusesV21(){if(!CoreHardClosureScopeV21())return;if(player.hp<=0){ClearCoreHardClosureStatusesV21();return;}
   for(int slot=0;slot<2;slot++){
    while(coreHardDotPowerV21[slot]>0&&coreHardDotNextV21[slot]<=elapsed+.000001&&coreHardDotNextV21[slot]<=coreHardDotUntilV21[slot]+.000001){float damage=Math.Min(player.hp,CoreHardOutgoingAttackPowerR1417(coreHardDotSupportOwnersR1417[slot],coreHardDotPowerV21[slot]));ApplyCombatHealth(player,Math.Max(0,player.hp-damage));SpawnResourceDamage(player,damage);CoreClosureEventV21(slot==0?"source-fire-resource-tick":"source-cold-resource-tick",null,null,0,"BBT_CHANGE_NOW_HP_PER_SECOND",0,damage>0,"Resource delta="+damage, (float)coreHardDotNextV21[slot]);coreHardDotNextV21[slot]+=1;if(player.hp<=0){player.state=CombatState.Dead;player.clip="death";player.clock=0;player.stun=0;CancelAction(true);ClearCoreHardClosureStatusesV21();return;}}
    if(elapsed>=coreHardDotUntilV21[slot]){coreHardDotUntilV21[slot]=coreHardDotNextV21[slot]=coreHardDotPowerV21[slot]=0;coreHardDotSupportOwnersR1417[slot]=null;}
   }
   if(coreHardStunUntilV21>0&&elapsed>=coreHardStunUntilV21){coreHardStunUntilV21=0;CoreClosureEventV21("source-stun-time-finalizer",null,null,0,"BFT_TIME2",0,true);}
  }
 }
}
