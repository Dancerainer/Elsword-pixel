#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed partial class GiantWilliamPlayerCheckV39 {
        [Serializable] sealed class WhiteRushReceiptV45 {
            public float hpFraction, startedX, endedX, maxRushClock, contactClock, landedDamage;
            public bool ready, rush, secondLoop, end;
            public int facing;
        }
        IEnumerable WhiteRushV45() {
            var rec=new WhiteRushReceiptV45();
            foreach(var step in Vulnerable())yield return step;
            int request=Mathf.CeilToInt(actor.hp-actor.maxHp*.58f);
            Need(request>0&&game.ApplyHitToEnemy(actor,request,HitReaction.None)>0,"Public damage to open the source HP condition");
            rec.hpFraction=actor.hp/actor.maxHp;
            Need(rec.hpFraction>0&&rec.hpFraction<.70f,"White Giant low HP source condition");
            game.traceNpcActions=true;
            double deadline=Time.realtimeSinceStartupAsDouble+100,nextPlace=0;
            while(!rec.end){
                yield return null;
                report.last="white natural rush: "+actor.npcState;
                Need(Time.realtimeSinceStartupAsDouble<deadline,report.last);
                var frame=scene.GiantWilliamFrameV39For(rendererView);
                if(frame==null)continue;
                var cam=(Camera)Field(scene,"worldCamera");
                string state=actor.npcState;
                if(!rec.ready&&state!=WhiteGiantRushV45.Ready&&Time.realtimeSinceStartupAsDouble>=nextPlace){
                    Place(76);nextPlace=Time.realtimeSinceStartupAsDouble+.55;
                }
                if(state==WhiteGiantRushV45.Ready&&frame.selected.motion=="DashAttackReady"&&!rec.ready){
                    rec.ready=Observe(frame,cam,"white-rush-02-ready",true);
                    if(rec.ready)Place(104,actor.facing);
                }
                if(!rec.ready)continue;
                if(state==WhiteGiantRushV45.Rush&&frame.selected.motion=="DashAttack"){
                    if(!rec.rush){
                        rec.startedX=actor.x;rec.facing=actor.facing;
                        rec.rush=Observe(frame,cam,"white-rush-03-charge",true);
                    }
                    rec.maxRushClock=Math.Max(rec.maxRushClock,actor.npcTime);rec.endedX=actor.x;
                    if(actor.npcTime>.62f&&!rec.secondLoop)
                        rec.secondLoop=Observe(frame,cam,"white-rush-04-second-loop",true);
                    if(actor.npcTime>.48f&&rec.landedDamage==0){
                        Place(106,actor.facing);game.player.invincible=0;
                    }
                    foreach(var e in game.npcActionEvents)
                        if(e.npc==actor.sourceNPC&&e.state==WhiteGiantRushV45.Rush&&e.type=="hit"&&e.damage>0){
                            rec.landedDamage=e.damage;rec.contactClock=e.sourceTime;
                        }
                }
                if(state==WhiteGiantRushV45.End&&frame.selected.motion=="DashAttackEnd"&&rec.secondLoop)
                    rec.end=Observe(frame,cam,"white-rush-05-recovery",true);
            }
            File.WriteAllText(Path.Combine(output,"white-rush.json"),JsonUtility.ToJson(rec,true));
            Need(rec.rush&&rec.maxRushClock>.85f&&(rec.endedX-rec.startedX)*rec.facing>80&&rec.landedDamage>0,
                 "Source looping movement and a real charge hit");
        }
    }
}
#endif
