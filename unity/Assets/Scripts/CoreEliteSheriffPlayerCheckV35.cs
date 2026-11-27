#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class CoreReactionPlayerCheckV32 {
        const string EliteStateV35="NASOD_ELITE_SHERIFF_";
        const string EliteShotV35="DamageEffect_NASOD_SHERIFF_ATTACK_A";

        void ElitePlayerAtV35(float side,bool protect) {
            float x=target.x+side;
            var floor=game.Surface(target.groundLine);
            if(floor==null||x<Math.Min(floor.x1,floor.x2)||x>Math.Max(floor.x1,floor.x2))
                floor=game.SurfaceBelow(x,target.y+75,true);
            Need(floor!=null,"Sheriff fixture player on source platform");
            game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;
            game.player.vx=game.player.vy=0;
            if(protect)game.player.invincible=12;
        }

        IEnumerator CheckEliteSheriffV35() {
            Need(target.sourceNPC==GameModel.EliteSheriffV35&&game.room==0&&game.enemies.IndexOf(target)==14,"Original Elite birth");
            var timers=new[]{.55f,1.23f,1.84f};
            var serials=new HashSet<int>();
            foreach(int side in new[]{1,-1}){
                bool[] shot=new bool[3];bool captured=false,impact=false,ready=false;
                var until=Time.realtimeSinceStartupAsDouble+40;game.player.invincible=0;
                while(!shot[0]||!shot[1]||!shot[2]||!captured||!impact){
                    Check();Need(Time.realtimeSinceStartupAsDouble<until,"Sheriff original fire/impact facing "+side);
                    ElitePlayerAtV35(side*25,impact);yield return null;
                    if(target.npcState==EliteStateV35+"ATTACK_A_READY"&&target.facing==side)ready=true;
                    if(!ready||target.npcState!=EliteStateV35+"ATTACK_A"||target.facing!=side)continue;
                    foreach(var p in game.npcEffects){
                        if(p.owner!=target||p.id!=EliteShotV35||p.facing!=side||!p.hasSourceLandY)continue;
                        if(serials.Add(p.serial)){
                            float at=p.bornAt-(game.elapsed-target.npcTime);
                            int n=Array.FindIndex(timers,q=>Math.Abs(at-q)<.02f);Need(n>=0,"Source fire timer "+at);
                            shot[n]=true;report.actions.Add("Elite sheriff "+side+" shot "+p.serial+" at "+at+" land "+p.sourceLandY);
                        }
                        if(!captured&&scene.FactorySheriffVisibleLasersV32>0&&ReadyOnscreen()){
                            var row=Observe(side>0?"FireRight":"FireLeft",true);
                            var body=(SpriteRenderer)((IList)Field(scene,"enemyViews"))[game.enemies.IndexOf(target)];
                            Need(row.sourceStatePreserved&&row.pixelsProven&&body.flipX==(side<0)&&
                                Mathf.Abs(body.transform.localScale.y-DemoScene.EliteWholeScaleV35)<.0001f,
                                "Actual registered sheriff size and facing");
                            captured=true;
                        }
                    }
                    foreach(var ev in game.npcPresentationEvents)
                        if(ev.owner==target&&ev.type=="hit"&&ev.effect==EliteShotV35&&ev.facing==side&&serials.Contains(ev.serial))impact=true;
                }
            }
            var getups=new HashSet<string>();var observedDown=new HashSet<string>();
            for(int i=0;i<60&&getups.Count<3;i++){
                bool rear=i==0;var until=Time.realtimeSinceStartupAsDouble+20;
                while(!CanHit()||!ReadyOnscreen()){
                    Check();Need(Time.realtimeSinceStartupAsDouble<until,"Elite awaiting physical vulnerability");
                    ElitePlayerAtV35(-45,true);yield return null;
                }
                ElitePlayerAtV35((rear?-1:1)*target.facing*40,true);
                string down=EliteStateV35+(rear?"DAMAGE_DOWN_BACK":"DAMAGE_DOWN_FRONT");
                Need(game.ApplyHitToEnemy(target,1,HitReaction.Knockdown)>0&&target.state==CombatState.Knockdown&&target.npcState==down,"Real public directed knockdown "+down);
                while(target.state==CombatState.Knockdown){
                    yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Elite down source motion end");
                    if(target.npcState==down&&target.npcTime>.15f&&ReadyOnscreen()&&observedDown.Add(down))Observe(down,true);
                }
                Need(target.state==CombatState.GettingUp,"Original recovery state");
                string selected=target.npcState;
                Need(rear?selected==EliteStateV35+"STAND_UP_BACK":selected==EliteStateV35+"STAND_UP_FRONT"||
                    selected==EliteStateV35+"ROLLING_STAND_UP_FRONT","Original ordered 20% choice "+selected);
                bool seen=getups.Contains(selected);
                while(target.state==CombatState.GettingUp){
                    if(!seen&&target.npcTime>.3f&&ReadyOnscreen()){
                        Observe(selected,true);getups.Add(selected);seen=true;
                    }
                    ElitePlayerAtV35(-45,true);yield return null;Check();
                    Need(Time.realtimeSinceStartupAsDouble<until,"Original get-up returns through motion end");
                }
                report.actions.Add("Public "+down+" -> "+selected);
            }
            Need(getups.Count==3&&observedDown.Count==2,"Back, ordinary front and rolling front with GPU-positive source frames");
            var deadline=Time.realtimeSinceStartupAsDouble+20;
            while(!CanHit()||!ReadyOnscreen()){Check();Need(Time.realtimeSinceStartupAsDouble<deadline,"Return from elite recovery");yield return null;}
        }
    }
}
#endif
