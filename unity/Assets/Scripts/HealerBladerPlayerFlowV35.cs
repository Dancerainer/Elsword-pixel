#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class HealerBladerPlayerCheckV35 {
        string Phase() {
            if(target.state==CombatState.Airborne)return target.vy>0?"Launch":"Fall";
            return target.state.ToString();
        }
        bool ReadyToSee() {
            if(!Resolve(out var owner,out _)||!owner.LastFrame.visible)return false;
            var c=(Camera)Field(scene,"worldCamera");
            var v=c.WorldToViewportPoint(parts[0].bounds.center);
            return v.z>0&&v.x>.02f&&v.x<.98f&&v.y>.02f&&v.y<.98f;
        }
        IEnumerable Ready(bool needHit){
            double end=Time.realtimeSinceStartupAsDouble+40;
            while(true) {
                yield return null;
                if(ReadyToSee()&&(!needHit||CanHit(target)))yield break;
                Place(-90);Need(Time.realtimeSinceStartupAsDouble<end,"Original birth/parts ready "+target.npcState+" "+target.state);
            }
        }
        void Hit(HitReaction reaction,bool lethal){
            Place(-target.facing*45);float hp=target.hp;
            float factor=Core&&game.CoreHardDefenceBoostActiveR1417(target)?GameModel.CoreHardDefenceDeviceDamageTakenR1417:1f;
            int damage=lethal?Mathf.CeilToInt((hp+1)/factor):1;
            int actual=game.ApplyHitToEnemy(target,damage,reaction);
            Need(actual>0&&target.hp<hp,"Public hit "+reaction);
            report.actions.Add("public hit "+reaction+" requested="+damage+" actual="+actual+" defence="+factor+
                " hp="+hp+"->"+target.hp+" state="+target.state+" lethal="+lethal);
            if(lethal)Need(target.hp<=0&&target.state==CombatState.Dead,"Public lethal must produce Dead and zero HP before measuring death");
        }
        Enemy WoundNearbyAlly() {
            Enemy best=null;float bestDistance=195;
            foreach(var a in game.enemies) {
                if(a==target||a.encounterIndex!=target.encounterIndex||a.isGate||a.passive||a.kind.StartsWith("factory-press")||
                    a.hp<4||!CanHit(a))continue;
                float dx=a.x-target.x,dy=a.y-target.y-17,dz=(a.nativeDepth-target.nativeDepth)*.28f;
                float d=Mathf.Sqrt(dx*dx+dy*dy+dz*dz);
                if(d<bestDistance){best=a;bestDistance=d;}
            }
            if(best==null)return null;
            int dealt=game.ApplyHitToEnemy(best,Math.Max(1,Math.Min(30,(int)best.hp/4)),HitReaction.Hurt);
            if(dealt<=0)return null;
            report.actions.Add("public wound ally "+best.sourceNPC+" index="+game.enemies.IndexOf(best)+" dealt="+dealt);
            return best;
        }
        void RecordHeal(Enemy ally,float woundedHp) {
            if(ally==null||target.npcState!="NASOD_HEALER_BLADER_HEAL")return;
            float age=target.npcTime;
            float packet=-1;
            if(Core) {
                float query=-1;
                foreach(var e in game.coreHardLocalAddEventsV21) {
                    if(e.npc!=HealerBladerRigV35.Id||e.at<game.elapsed-.18f)continue;
                    if(e.kind=="source-particle-request-HOLD"&&e.particle=="TimeStop1"&&Mathf.Abs(age-.43f)<.18f&&
                        Mathf.Abs(e.x-target.x)<15)report.corePreEffect=true;
                    if(e.kind=="source-heal-query"&&Mathf.Abs(e.x-target.x)<15&&Mathf.Abs(age-.63f)<.18f)query=e.at;
                    if(e.kind=="source-real-heal"&&e.authoritativeHpWrite&&e.target==ally.sourceNPC&&
                        e.amount>0&&Mathf.Abs(e.at-query)<1e-4f&&Mathf.Abs(e.x-ally.x)<10&&ally.hp>woundedHp)
                        packet=e.at;
                }
            } else {
                foreach(var other in game.enemies)
                    if(other!=target&&other.active&&other.sourceNPC==target.sourceNPC&&other.npcState==target.npcState&&
                        Mathf.Abs(other.npcTime-.63f)<.18f&&Mathf.Abs(other.x-ally.x)<197)return;
                foreach(var e in game.npcSourceSupportEventsV17)
                    if(e.npc==HealerBladerRigV35.Id&&e.kind=="heal"&&e.target==ally.sourceNPC&&e.amount>0&&
                        e.time>=game.elapsed-.18f&&Mathf.Abs(age-.63f)<.18f&&Mathf.Abs(e.x-ally.x)<10&&ally.hp>woundedHp)
                        packet=e.time;
            }
            if(packet>=0&&!report.healHpWrite){
                report.healHpWrite=true;report.healPacketModelTime=packet;report.healClock=age;
                report.healedAllyIndex=game.enemies.IndexOf(ally);report.woundedHp=woundedHp;report.healedHp=ally.hp;
                Observe("source-heal-hp-write");
            }
        }
        void RecordBlade(float beforeHp){
            if(report.bladeHpHit||game.player.hp>=beforeHp)return;
            foreach(var e in game.npcActionEvents) {
                if(e.npc!=HealerBladerRigV35.Id||e.state!="NASOD_HEALER_BLADER_DASH_ATTACK"||
                    e.type!="hit"||e.damage<=0||e.time<game.elapsed-.12f||e.sourceTime<0||e.sourceTime>.32f||
                    Mathf.Abs(e.x-target.x)>20)continue;
                report.bladeHpHit=true;report.bladeHitDamage=e.damage;report.bladeClock=e.sourceTime;
                Observe("source-blade-hit");break;
            }
        }
        IEnumerable SourceActions() {
            var motions=new HashSet<string>();var facing=new HashSet<int>();
            Enemy ally=null;float woundedHp=0, beforePlayerHp=game.player.hp;double end=Time.realtimeSinceStartupAsDouble+125;
            while(true) {
                yield return null;report.frames++;Need(Time.realtimeSinceStartupAsDouble<end,
                    "Original AI source actions, two actual facings and ally heal. Observed "+string.Join(",",motions)+" hpWrite="+report.healHpWrite);
                if(!ReadyToSee()) { Place(-90);continue; }
                Resolve(out var owner,out _);
                var f=owner.LastFrame;
                if(motions.Add(f.alias)) {
                    report.motions.Add(f.alias);
                    Observe("source-"+f.alias);
                }
                if(facing.Add(f.facing))Observe(f.facing<0?"facing-left":"facing-right");
                RecordHeal(ally,woundedHp);
                RecordBlade(beforePlayerHp);beforePlayerHp=game.player.hp;
                if(!report.healHpWrite&&(ally==null||ally.hp<=0||ally.hp>=ally.maxHp)) {
                    ally=WoundNearbyAlly();if(ally!=null)woundedHp=ally.hp;
                }
                if((report.corePreEffect||!Core)&&report.healHpWrite&&report.bladeHpHit&&
                    motions.Contains("Healing")&&motions.Contains("DashAttackReady")&&motions.Contains("DashAttack")&&facing.Count==2)
                    yield break;
                // These two ordinary source choices use 140/196 distance gates.
                // Position only the observer's player, leaving AI, cooldowns, attack and heal clocks intact.
                int direction=facing.Count==1?-f.facing:(target.facing==0?1:target.facing);
                if(facing.Count==1&&facing.Contains(direction))direction=-direction;
                // After both facings are proven, keep the real Factory target
                // near its actually wounded ally. Holding the player ahead of
                // every dash otherwise leads the Healer away from its patient.
                if(!Core&&!report.healHpWrite&&facing.Count==2&&ally!=null&&Mathf.Abs(ally.x-target.x)>8)
                    direction=ally.x>target.x?1:-1;
                if(!report.bladeHpHit&&(f.alias=="DashAttackReady"&&f.sourceClock>.65f||f.alias=="DashAttack")){
                    Place(f.facing*28);game.player.invincible=0;
                }else Place(direction*90);
            }
        }
        IEnumerable Reactions(){
            foreach(var x in Ready(true))yield return x;
            Hit(HitReaction.Hurt,false);yield return null;Need(target.state==CombatState.Hurt,"Original Hurt");
            Observe("hurt");
            foreach(var x in Ready(true))yield return x;
            Hit(HitReaction.Launch,false);var phases=new HashSet<string>();double end=Time.realtimeSinceStartupAsDouble+32;
            do {
                yield return null;report.frames++;Need(Time.realtimeSinceStartupAsDouble<end,"Original launch recovery");
                if(target.state!=CombatState.Normal&&phases.Add(Phase()))Observe("reaction-"+Phase());
            } while(target.state!=CombatState.Normal);
            foreach(var p in new[]{"Launch","Fall","Knockdown","GettingUp"})Need(phases.Contains(p),"Original reaction "+p);
            foreach(var x in Ready(true))yield return x;
            Hit(HitReaction.Hurt,true);yield return null;Observe("death-first");float first=parts[0].color.a;
            if(!Core){
                var selected=report.shots[report.shots.Count-1];
                Need(game.TryFactoryHealerLethalPoseV35(target,out var source,out float clock) &&
                    source=="NASOD_HEALER_BLADER_DYING_LAND_BACK" && selected.state==source &&
                    selected.motion=="DamageDown" && Math.Abs(selected.sourceClock-clock)<.0001f,"Real Factory rear lethal source state, motion and clock");
            }
            end=Time.realtimeSinceStartupAsDouble+10;
            while(target.deathTime<.55f){yield return null;Need(Time.realtimeSinceStartupAsDouble<end,"Death clock");}
            Observe("death-mid");Need(parts[0].color.a>0&&parts[0].color.a<first,"Original carrier death fade travels to all nine parts");
        }
        IEnumerable Leave(bool proveRelease){
            var oldParts=parts.ToArray();
            var bank=(FairyPoseLibraryV33)Field(scene,"healerBankV35");
            Sprite sprite=bank?.sharedSprites[0];Texture2D packed=bank?.renderAtlas;
            target=null;Press("dungeons");for(int i=0;i<5;i++)yield return null;
            Need(!RouteLive&&ReferenceEquals(Field(scene,"game"),original)&&ReferenceEquals(Field(scene,"worldCamera"),cameraOwner)&&
                (string)Field(scene,"openMenu")=="dungeons","Ordinary Main model and camera restored");
            Need(((IDictionary)Field(scene,"healerViewsV35")).Count==0&&Field(scene,"healerBankV35")==null,"Room-owned bank and views released");
            foreach(var p in oldParts)Need(!p,"Retired renderer still alive");
            if(proveRelease)Need(!sprite&&!packed,"Retired runtime sprite/texture still alive");
            report.returned=report.release=true;
        }
        IEnumerator Run(){
            while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>()))yield return null;
            for(int i=0;i<3;i++)yield return null;
            original=(GameModel)Field(scene,"game");cameraOwner=(Camera)Field(scene,"worldCamera");
            Need((string)Field(scene,"openMenu")=="dungeons"&&original.paused,"Ordinary Main menu start");
            foreach(var x in Enter())yield return x;
            Stage();foreach(var x in Ready(false))yield return x;Observe("initial-born");
            foreach(var x in SourceActions())yield return x;
            foreach(var x in Reactions())yield return x;
            var firstGame=game;var firstTarget=target;var firstBank=Field(scene,"healerBankV35");
            foreach(var x in Leave(true))yield return x;
            foreach(var x in Enter())yield return x;
            Stage();foreach(var x in Ready(false))yield return x;
            Need(Core?ReferenceEquals(firstGame,original)&&ReferenceEquals(game,original):!ReferenceEquals(firstGame,game),
                "Original Core shared Main or fresh Factory component lifecycle");
            Need(!ReferenceEquals(firstTarget,target)&&firstBank!=null&&!ReferenceEquals(firstBank,Field(scene,"healerBankV35")),
                "Fresh source birth and independently loaded room-owned art");
            Observe("reentry");report.reentered=true;
            foreach(var x in Leave(true))yield return x;
        }
    }
}
#endif
