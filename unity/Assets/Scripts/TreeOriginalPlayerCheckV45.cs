#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        IEnumerable OriginalTreeHitsV45(){
            foreach(bool down in new[]{false,true}){
                foreach(bool rear in new[]{false,true}){
                    string suffix=rear?"Back":"Front";
                    double until=Time.realtimeSinceStartupAsDouble+30;
                    while(enemy.state!=CombatState.Normal||enemy.superArmor>0||enemy.invincible>0||
                          game.EnemyHasNpcInvincibility(enemy)||game.EnemyHasSuperArmor(enemy)){
                        Budget();Need(Time.realtimeSinceStartupAsDouble<until,"Source tree did not become vulnerable");
                        Protect(-45);yield return null;
                    }
                    Protect(enemy.facing*(rear?-38:38));
                    int dealt=game.ApplyHitToEnemy(enemy,1,down?HitReaction.Knockdown:HitReaction.Hurt);
                    Need(dealt>0&&enemy.originalHitBackV45==rear,"Actual directed tree damage");
                    string hit=(down?"DamageDown":"DamageSmall")+suffix;
                    bool witnessed=false,rise=false;
                    do{
                        yield return null;Budget();Need(Time.realtimeSinceStartupAsDouble<until,"Source tree reaction budget");
                        var owners=(IDictionary)Field(scene,"treeKnightA1ViewsV30");
                        if(!owners.Contains(enemy))continue;
                        var parts=(TreeKnightAuthoredPartsV30)Field(owners[enemy],"parts");
                        string visible=parts.VisibleHurtMotionV45;
                        if(!witnessed&&visible==hit&&enemy.clock>.055f){
                            var sample=Row("original-"+hit);sample.sourceMotion=visible;Capture(sample,true);witnessed=true;
                        }
                        string expectRise="DamageStandUp"+suffix;
                        if(down&&!rise&&visible==expectRise&&enemy.state==CombatState.GettingUp&&enemy.clock>.055f){
                            var sample=Row("original-"+expectRise);sample.sourceMotion=visible;Capture(sample,true);rise=true;
                        }
                    }while(enemy.state!=CombatState.Normal);
                    Need(witnessed&&(!down||rise),"Actual six parts drew "+hit+" and its recovery");
                }
            }
        }
    }
}
#endif
