#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        const string MechanicalMouseV35="NUI_NASOD_MOUSE",MechanicalHouseV35="NUI_NASOD_MOUSE_HOUSE";
        IEnumerable PlainMouseStateV35(string required,int facing){
            double end=Time.realtimeSinceStartupAsDouble+38;
            do{Protect(facing*48);yield return null;Budget();Need(enemy.hp>0&&Time.realtimeSinceStartupAsDouble<end,"Original Plain state "+required+" actual="+enemy.npcState);
                var owners=(IDictionary)Field(scene,"mouseViewsV35");
                if(enemy.npcState==required&&enemy.npcTime>.1f&&owners.Contains(enemy)&&(string)Field(owners[enemy],"actualAlias")==required)yield break;
            }while(true);
        }
        void PlainMouseCaptureV35(string name){
            var row=CaptureMouseV35(name);var owners=(IDictionary)Field(scene,"mouseViewsV35");
            var o=owners[enemy];var pose=(string)Field(o,"plainSourcePose");var state=(string)Field(o,"actualAlias");
            var banks=(Dictionary<string,PlainVariablePhaseBankR1330>)Field(scene,"plainVariableBanksR1330");var bank=banks[MechanicalMouseV35];
            var b=Array.Find(bank.stateDisplayBindingsOnly,x=>x.state==state);var m=b==null?null:Array.Find(bank.motions,x=>x.animation==b.originalSourceAnimation);
            Need(m!=null&&m.animation==row.sourceMotion&&!string.IsNullOrEmpty(pose),"Actual original variable-phase identity/state/motion");
            double t=row.sourceTime;var key=Array.FindLast(m.frameKeys,k=>k.originalTime<=t+.00001);Need(key!=null&&key.poseID==pose,"Original selected pose ID "+pose);
            if(!string.IsNullOrEmpty(enemy.npcState)){
                double expected=enemy.npcTime*(double)(float)b.originalSourceAnimSpeed;
                if(b.originalLoop)expected%=m.sourceMotionDuration;else expected=Math.Min(expected,m.sourceMotionDuration);
                Need(Math.Abs(expected-t)<.00008,"Plain original scope and source clock");
            }
            report.events.Add(name+" registered original pose="+pose+" current="+state+" sourceTime="+t.ToString("R"));
        }
        IEnumerable PlainMouseFlowV35(){
            foreach(var x in Enter("altera","altera-plain"))yield return x;
            Call(game,"EnterRoom",1);if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);Call(scene,"RebuildRoom");
            Need(game.CurrentRoom.id=="Dungeon_Altera_Plain_Normal-1"&&scene.PlainProductSessionActiveR1320,"Original normal Plain room1");report.room=game.CurrentRoom.id;report.npc=MechanicalMouseV35;
            var originals=game.enemies.FindAll(e=>e.sourceNPC==MechanicalMouseV35);var houses=game.enemies.FindAll(e=>e.sourceNPC==MechanicalHouseV35);
            Need(originals.Count>3&&houses.Count>0,"Original guarded Plain population");var originalSet=new HashSet<Enemy>(originals);
            enemy=originals.Find(e=>e.active&&!e.dormant&&e.encounterIndex==game.EncounterIndex);Need(enemy!=null,"First original real mouse");
            foreach(var x in PlainMouseStateV35("NASOD_MOUSE_ATTACK_A_READY",1))yield return x;PlainMouseCaptureV35("plain-ready");
            foreach(var x in PlainMouseStateV35("NASOD_MOUSE_ATTACK",1))yield return x;PlainMouseCaptureV35("plain-attack");
            // The original Plain policy counts all living room mice. Use the
            // product's normal encounter transitions and PUBLIC hits to create
            // eligibility while preserving three source actors and every house.
            var sorted=new List<Enemy>(originals);sorted.Sort((a,b)=>a.encounterIndex.CompareTo(b.encounterIndex));
            var survivors=new HashSet<Enemy>(sorted.GetRange(sorted.Count-3,3));int current=-1;
            foreach(var mouse in sorted){
                if(mouse.encounterIndex!=current){current=mouse.encounterIndex;Call(game,"BeginEncounter",current);}
                if(survivors.Contains(mouse)||mouse.hp<=0)continue;enemy=mouse;double end=Time.realtimeSinceStartupAsDouble+16;
                while(enemy.hp>0){Protect(40);yield return null;Budget();Need(Time.realtimeSinceStartupAsDouble<end,"Original mouse became publicly damageable "+enemy.npcState);game.ApplyHitToEnemy(enemy,100000,HitReaction.None);}
            }
            Call(game,"BeginEncounter",current);
            Enemy born=null;double until=Time.realtimeSinceStartupAsDouble+45;
            // BeginEncounter leaves earlier admitted houses active. They compete
            // for the same <=3 room population and can emit before the final
            // encounter's houses. The production request pins the child's
            // encounter to the emitting house, not to the observer's position.
            while(born==null){
                Budget();game.player.invincible=30;
                if(Time.realtimeSinceStartupAsDouble>=until){
                    foreach(var h in houses)report.events.Add("Plain house timeout enc="+h.encounterIndex+" active="+h.active+
                        " state="+h.npcState+" sourceTime="+h.npcTime+" hp="+h.hp+" x="+h.x);
                    foreach(var ev in game.alteraPopulationEventsV18)if(ev.npc==MechanicalHouseV35)
                        report.events.Add("Plain source "+ev.kind+" count="+ev.count+" at="+ev.x+" t="+ev.time);
                    Need(false,"Natural Plain source mouse-house emission");
                }
                yield return null;
                born=game.enemies.Find(e=>e.sourceNPC==MechanicalMouseV35&&!originalSet.Contains(e)&&e.hp>0&&e.active);
            }
            var parent=houses.Find(h=>h.encounterIndex==born.encounterIndex&&Math.Abs(h.x-born.spawnX)<.02f);
            Need(parent!=null,"Original Plain house position and encounter ownership");
            if(born.encounterIndex!=game.EncounterIndex)Call(game,"BeginEncounter",born.encounterIndex);
            Need(born.active&&game.EncounterIndex==born.encounterIndex,"Rendered original emitting encounter");
            report.events.Add("Original house and emitted actor enc="+born.encounterIndex+" x="+born.spawnX);
            var emitted=game.alteraPopulationEventsV18.FindLast(e=>e.kind=="source-create-request"&&e.npc==MechanicalHouseV35&&Math.Abs(e.x-born.spawnX)<.02f);
            var admitted=emitted==null?null:game.alteraPopulationEventsV18.FindLast(e=>e.kind=="source-population-condition"&&e.npc==MechanicalHouseV35&&Math.Abs(e.x-born.spawnX)<.02f&&e.time<=emitted.time+.00001f);
            var cost=emitted==null?null:game.alteraPopulationEventsV18.FindLast(e=>e.kind=="source-host-hp-cost"&&e.npc==MechanicalHouseV35&&Math.Abs(e.x-born.spawnX)<.02f&&Math.Abs(e.time-emitted.time)<.00001f);
            Need(admitted!=null&&admitted.count<=3&&emitted!=null&&cost!=null&&cost.before-cost.after==500,"Verified <=3 original admission and500 host HP cost");
            report.events.Add("Original house count="+admitted.count+" create="+emitted.time.ToString("R")+" ownerHP="+cost.before+"->"+cost.after);
            enemy=born;foreach(var x in PlainMouseStateV35("NASOD_MOUSE_ATTACK_A_READY",-1))yield return x;PlainMouseCaptureV35("plain-house-born-ready");
            foreach(var x in PlainMouseStateV35("NASOD_MOUSE_ATTACK",-1))yield return x;PlainMouseCaptureV35("plain-house-born-attack");
        }
    }
}
#endif

