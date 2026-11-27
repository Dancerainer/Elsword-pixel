#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed partial class GiantWilliamPlayerCheckV39 {
        [Serializable] sealed class EscapeSampleV45 {
            public string npc,motion;public float hpAtTrigger,clock;
            public bool hiddenAtEnd;public int damage;
        }
        [Serializable] sealed class AlteraChoicesReceiptV45 {
            public float cannonRange;
            public string emitted;
            public List<EscapeSampleV45> escapes=new List<EscapeSampleV45>();
        }
        IEnumerable AlteraChoicesV45(){
            var record=new AlteraChoicesReceiptV45();
            foreach(var step in EnterRoute(scene,"altera","altera-battle-airship"))yield return step;
            while(!scene.BattleProductSessionActiveR1325)yield return null;
            game=(GameModel)Field(scene,"game");
            Call(game,"EnterRoom",2);Call(scene,"RebuildRoom");
            if(game.EncounterIndex!=0)Call(game,"BeginEncounter",0);
            actor=game.enemies[10];Need(actor.sourceNPC=="NUI_CANNON_BIG","Actual Battle cannon");
            game.traceNpcEffects=true;game.player.hp=game.player.maxHp=5000;
            double limit=Time.realtimeSinceStartupAsDouble+45;
            while(record.emitted==null){
                Place(330);
                yield return null;
                report.last="far cannon "+actor.npcState;
                Need(Time.realtimeSinceStartupAsDouble<limit,report.last);
                if(actor.npcState=="CANNON_BIG_ATTACK_1")
                    record.cannonRange=Vector2.Distance(new Vector2(actor.x,actor.y),new Vector2(game.player.x,game.player.y));
                foreach(var ev in game.npcEffectEvents)
                    if(ev.npc==actor.sourceNPC&&ev.effect=="DamageEffect_CANNON_BIG_ATTACK1"&&ev.type=="spawn")record.emitted=ev.effect;
            }
            Need(record.cannonRange>280,"Original long-range gate");
            Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;
            foreach(var step in EnterRoute(scene,"altera","altera-plain"))yield return step;
            while(!scene.PlainProductSessionActiveR1320)yield return null;
            game=(GameModel)Field(scene,"game");
            for(int i=0;i<2;i++){
                if(game.room!=i){Call(game,"EnterRoom",i);Call(scene,"RebuildRoom");}
                if(game.EncounterIndex!=i)Call(game,"BeginEncounter",i);
                actor=game.enemies[i==0?13:26];
                string npc=i==0?"NUI_NASOD_ELEMENT_FIRE_SOLO":"NUI_NASOD_ELEMENT_ICE_SOLO";
                Need(actor.sourceNPC==npc,"Original Plain birth "+npc);
                game.player.hp=game.player.maxHp=5000;
                var rec=new EscapeSampleV45{npc=npc,motion=npc.Substring(4)+"_TELEPORT"};record.escapes.Add(rec);
                Place(95);foreach(var step in Vulnerable())yield return step;
                rec.damage=game.ApplyHitToEnemy(actor,Mathf.CeilToInt(actor.hp-actor.maxHp*.06f),HitReaction.None);
                rec.hpAtTrigger=actor.hp;Need(rec.damage>0&&actor.hp>0&&actor.hp<actor.maxHp*.1f,"Real hit opened teleport");
                limit=Time.realtimeSinceStartupAsDouble+35;
                while(actor.hp>0){yield return null;Need(Time.realtimeSinceStartupAsDouble<limit,"Actual teleport "+actor.npcState);
                    if(actor.npcState==rec.motion)rec.clock=Math.Max(rec.clock,actor.npcTime);}
                bool visible=true,gauge=true;game.ApplyPlainMineSourceVisibilityR1302(actor,ref visible,ref gauge);
                rec.hiddenAtEnd=!visible&&!gauge;Need(rec.clock>.75f&&rec.hiddenAtEnd,"Teleported only when original motion completed");
            }
            File.WriteAllText(Path.Combine(output,"altera-choices.json"),JsonUtility.ToJson(record,true));
            Press(scene,"dungeons");for(int i=0;i<5;i++)yield return null;Need((string)Field(scene,"openMenu")=="dungeons","Return from Plain");
        }
    }
}
#endif
