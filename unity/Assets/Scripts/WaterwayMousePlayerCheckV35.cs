#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        IEnumerable WaitMouseAliasV35(string alias,float minimumSource,int facing) {
            double end=Time.realtimeSinceStartupAsDouble+42;
            for(;;){
                Protect(facing*(alias=="walk"?185:43));yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Original mouse "+alias+" actual="+enemy.npcState+"/"+enemy.clip+" hp="+enemy.hp);
                var d=(IDictionary)Field(scene,"mouseViewsV35");
                if(!d.Contains(enemy))continue;var o=d[enemy];
                if((string)Field(o,"actualAlias")==alias&&(float)Field(o,"sourceClock")>=minimumSource)yield break;
            }
        }
        Observation CaptureMouseV35(string name) {
            var r=Row(name);Capture(r,true);
            Need(r.callbacks==5&&r.actualParts==5&&r.pixels>0,"Five genuine mouse camera draws");
            var d=(IDictionary)Field(scene,"mouseViewsV35");var o=d[enemy];var members=(SpriteRenderer[])Field(o,"members");var keys=(int[])Field(o,"keys");
            Need(members.Length==5&&keys.Length==5,"Original five-rigid-mesh membership");
            int sourceFace=enemy.facing;
            var positions=new List<string>();
            for(int i=0;i<5;i++){
                Need(members[i]&&members[i].sprite&&members[i].gameObject.activeInHierarchy&&members[i].enabled,"Individual mouse "+i);
                for(int j=0;j<i;j++)Need(!ReferenceEquals(members[i],members[j]),"Unique source body renderer");
                positions.Add(members[i].transform.position.ToString("F3"));
            }
            Need(MathematicalFacingV35(members[0])==sourceFace,"Mouse original model facing");
            report.events.Add(name+" positions="+string.Join(" / ",positions)+" variants="+r.sourceKey);
            return r;
        }
        static int MathematicalFacingV35(SpriteRenderer child){return child.transform.parent.localScale.x<0?-1:1;}
        IEnumerable WaterwayMouseFlowV35() {
            foreach(var x in Enter("elder","underground-waterway"))yield return x;
            Call(game,"EnterRoom",1);Call(scene,"RebuildRoom");
            Need(game.CurrentRoom.id=="waterway-2","Original Waterway second room");
            report.room=game.CurrentRoom.id;report.npc="NUI_MOUSE / NUI_MOUSE_NODROP";
            var originals=game.enemies.FindAll(e=>e.sourceNPC=="NUI_MOUSE");var houses=game.enemies.FindAll(e=>e.sourceNPC=="NUI_MOUSEHOUSE");
            Need(originals.Count==4&&houses.Count==2,"Four actual mouse births and two original houses");
            enemy=originals[0];foreach(var x in WaitMouseAliasV35("attack-ready",.1f,1))yield return x;
            var ready=CaptureMouseV35("ordinary-ready");Need(ready.sourceMotion=="AttackA_Ready"&&Math.Abs(ready.sourceTime-enemy.npcTime*1.25f)<.00008f,"Ordinary 2.16s host -> original 2.7s");
            foreach(var x in WaitMouseAliasV35("attack",.12f,1))yield return x;
            var bite=CaptureMouseV35("ordinary-bite");Need(bite.sourceMotion=="AttackA_Fire","Original fire motion");
            foreach(var x in WaitMouseAliasV35("walk",.05f,-1))yield return x;
            CaptureMouseV35("ordinary-walk");int hit=game.ApplyHitToEnemy(enemy,7,HitReaction.Hurt);Need(hit==7&&enemy.state==CombatState.Hurt,"Public ordinary hurt");
            yield return null;var hurt=CaptureMouseV35("ordinary-hurt");Need(hurt.sourceMotion=="Damage","Original hurt motion");
            // Deliberately separate the two original eligibility checks with
            // public damage to the second house, without assigning any enemy.
            int actualDelay=game.ApplyHitToEnemy(houses[1],1,HitReaction.Hurt);Need(actualDelay==1,"Second house accepts public stagger");
            float ownerSum=houses[0].hp+houses[1].hp;
            Need(game.ApplyHitToEnemy(enemy,100000,HitReaction.None)>0&&enemy.state==CombatState.Dead,"Actual killed original mouse");
            yield return null;var dead=CaptureMouseV35("ordinary-death");Need(dead.sourceMotion=="DamageDown","Original death alias");
            double until=Time.realtimeSinceStartupAsDouble+32;Enemy child=null;List<Enemy> born=null;
            while(child==null){Budget();Need(Time.realtimeSinceStartupAsDouble<until,"A real house emission");yield return null;
                born=game.enemies.FindAll(e=>e.sourceNPC=="NUI_MOUSE_NODROP"&&e.hp>0);if(born.Count>0)child=born[0];}
            Need(born.Count<=2&&child.kind=="mouse-nodrop"&&child.maxHp==originals[0].maxHp,"Original dynamic body and balance");
            Need(Math.Abs(ownerSum-houses[0].hp-houses[1].hp-500*born.Count)<.01f,"Original 500 HP cost for each committed emission");
            Need(houses.Exists(h=>Math.Abs(h.x-child.spawnX)<.01f),"Child uses actual house spawn anchor");
            report.events.Add("Original source birth count="+born.Count+" after one vacancy. Eligibility is checked before the .4s emission.");
            enemy=child;
            foreach(var x in WaitMouseAliasV35("attack-ready",.08f,-1))yield return x;
            ready=CaptureMouseV35("house-born-ready");Need(ready.sourceMotion=="AttackA_Ready"&&Math.Abs(ready.sourceTime-enemy.npcTime)<.00008f,"NODROP 2.7s at original1x");
            foreach(var x in WaitMouseAliasV35("attack",.12f,-1))yield return x;CaptureMouseV35("house-born-bite");
        }
    }
}
#endif

