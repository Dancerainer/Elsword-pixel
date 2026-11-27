#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class StructurePlayerCheckV41 {
        static partial void AdditionalStructureModeV41(string mode,ref Spec selection) {
            switch(mode) {
                case "mine": selection=new Spec("altera","altera-plain-recycle","Dungeon_Altera_Plain_Recycle_Normal-1",1,0,14,6,"NUI_ALTERA_MINE","altera-mine","altera-mine"); break;
                case "cave-stone": selection=new Spec("elder","banthus-cave","cave-1",0,0,10,6,"NUI_DOWN_STONE_TRAP","down-stone-trap","down-stone-trap"); break;
                case "mine-stone": selection=new Spec("bethma","bethma-richie-mine","Dungeon_Besma_MINE2_Normal-0",0,0,11,2,"NUI_DOWN_STONE_TRAP_BESMA","down-stone-trap-besma","down-stone-trap-besma"); break;
                case "spike": selection=new Spec("bethma","dragon-road","dragon-road-1",0,0,8,16,"NUI_SPIKEBOARD_HERB","spikeboard-herb","spikeboard-herb"); break;
                case "wood-post": selection=new Spec("elder","wally-castle-outskirts","outskirts-1",0,0,10,12,"NUI_WOOD_POST_TRAP","wood-post-trap","wood-post-trap"); break;
                case "shock": selection=PlainMachineV40(0,5,26,"SHOCK_STICK","shock-stick"); break;
                case "barrier-fire": selection=PlainMachineV40(2,2,18,"BARRIERMACHINE_FIRE","barriermachine-fire"); break;
                case "barrier-ice": selection=PlainMachineV40(2,3,21,"BARRIERMACHINE_ICE","barriermachine-ice"); break;
                case "nasod-mouse-house": selection=new Spec("altera","altera-plain","Dungeon_Altera_Plain_Normal-1",1,2,40,109,"NUI_NASOD_MOUSE_HOUSE","nasod-mouse-house","nasod-mouse-house"); break;
                case "rail-house": selection=new Spec("bethma","bethma-richie-mine","Dungeon_Besma_MINE2_Normal-1",1,0,10,29,"NUI_NASOD_RAIL_HOUSE","nasod-rail-house","nasod-rail-house"); break;
                case "mousehouse": selection=new Spec("elder","underground-waterway","waterway-2",1,0,3,23,"NUI_MOUSEHOUSE","mousehouse","mousehouse"); break;
                case "totem": selection=TotemMachineV40(false); break;
                case "totem-night": selection=TotemMachineV40(true); break;
            }
        }
        static Spec PlainMachineV40(int room,int index,int spawn,string npc,string kind) {
            return new Spec("altera","altera-plain","Dungeon_Altera_Plain_Normal-"+room,room,0,index,spawn,"NUI_"+npc,kind,kind);
        }
        static Spec TotemMachineV40(bool night) {
            return new Spec("bethma",night?"besma-lake-night":"besma-lake",night?"Dungeon_Besma_Lake_Night_Normal-0":"besma-lake-2",
                night?0:1,0,night?6:12,night?13:53,"NUI_BESMA_TOTEM_BASE","besma-totem-base","besma-totem-base");
        }
        partial void AdditionalStructureStepsV41(ref IEnumerable steps) {
            if(report.mode=="shock")steps=ShockV40();
            else if(report.mode.StartsWith("barrier-",StringComparison.Ordinal))steps=BarrierV40();
            else if(report.mode.StartsWith("totem",StringComparison.Ordinal))steps=TotemV40();
            else if(report.mode=="mine")steps=MineV40();
            else if(report.mode=="cave-stone" || report.mode=="mine-stone")steps=StoneV40();
            else if(report.mode=="spike" || report.mode=="wood-post")steps=StakesV40();
            else steps=HouseV40();
        }

        bool InViewV40() {
            var view=Current(); if(!view || !view.enabled || !view.sprite)return false;
            var camera=(Camera)Field(scene,"worldCamera");
            var point=camera.WorldToViewportPoint(view.bounds.center);
            return point.z>0 && point.x>.03f && point.x<.97f && point.y>.02f && point.y<.98f;
        }
        bool StateV40(string suffix) {
            return target.npcState!=null && target.npcState.EndsWith(suffix,StringComparison.Ordinal);
        }
        IEnumerable ObserveV40(string label,Func<bool> ready) {
            foreach(var step in Until(()=>InViewV40() && ready(),label,55))yield return step;
            var actual=Current().sprite.texture;
            Need(ReferenceEquals(actual,Resources.Load<Texture2D>("Pixel/combat-machines-v40/"+spec.kind)),
                "Own shipped authored page "+spec.npc);
            Capture(label);
        }
        IEnumerable DamageV40(Enemy victim,int amount) {
            double until=Time.realtimeSinceStartupAsDouble+35;
            while(victim.hp>0) {
                int dealt=game.ApplyHitToEnemy(victim,amount,HitReaction.Hurt);
                if(dealt>0) {report.damage+=dealt;yield break;}
                Need(Time.realtimeSinceStartupAsDouble<until,"Original vulnerable interval "+victim.sourceNPC);
                Protect();yield return null;Check();
            }
            throw new InvalidOperationException("Structure target died before public hit: "+victim.sourceNPC);
        }
        IEnumerable ShockV40() {
            foreach(var s in ObserveV40("01-rising",()=>StateV40("_READY") && target.npcTime>.9f))yield return s;
            foreach(var s in ObserveV40("02-discharging",()=>StateV40("_ATTACK") && target.npcTime>.1f))yield return s;
            Need(game.TryReadPlainShockStickSourceR1477(target,out var frame) && frame.AttackCallbacks>0 &&
                frame.DamageEffects==2*frame.AttackCallbacks,"Two original damage effects for each Shock attack");
            foreach(var s in ObserveV40("03-extended",()=>StateV40("_WAIT")))yield return s;
            foreach(var s in DamageV40(target,17))yield return s;
            foreach(var s in ObserveV40("04-damaged",()=>StateV40("_DAMAGE")))yield return s;
            foreach(var s in DamageV40(target,1000000))yield return s;
            foreach(var s in Until(()=>target.hp<=0 && StateV40("_DYING") && !Current().enabled,"Original Shock death hides its body"))yield return s;
            report.changedArt=true;
        }
        IEnumerable BarrierV40() {
            foreach(var s in ObserveV40("01-defence",()=>StateV40("_DEFENCE_UP")))yield return s;
            var open=Current().sprite.rect;
            string own=spec.kind.EndsWith("fire",StringComparison.Ordinal)?"NUI_NASOD_ELEMENT_FIRE":"NUI_NASOD_ELEMENT_ICE";
            var original=game.enemies.Find(e=>e.sourceNPC==own && e.hp>0);
            Need(original!=null,"Original paired element");
            foreach(var s in DamageV40(original,1000000))yield return s;
            Need(original.hp<=0,"Original element defeated while the other remains");
            foreach(var s in ObserveV40("02-respawn",()=>StateV40("_RESPAWN") &&
                target.npcTime>=.56f && Current().sprite.rect!=open))yield return s;
            var birth=game.enemies.Find(e=>e.sourceNPC==own && e!=original && e.hp>0);
            Need(birth!=null,"Original paired element returned through machine callback");
            foreach(var s in Until(()=>StateV40("_WAIT") || StateV40("_DEFENCE_UP"),"Machine returned after Respawn"))yield return s;
            foreach(var s in DamageV40(target,1000000))yield return s;
            foreach(var s in ObserveV40("03-shotdown",()=>StateV40("_SHOTDOWN")))yield return s;
            Need(Mathf.Abs(target.hp-target.maxHp*.3f)<.01f,"Original nonlethal thirty-percent floor");
            report.changedArt=true;
        }
        bool SpawnedAtHouseV40(string npc,int firstEvent) {
            return game.npcSummonEvents.FindIndex(firstEvent,e=>e.kind=="spawn" && e.owner==spec.npc && e.npc==npc &&
                Mathf.Abs(e.x-target.x)<.1f && Mathf.Abs(e.y-target.y)<.1f)>=0;
        }
        IEnumerable HouseV40() {
            foreach(var s in ObserveV40("01-intact",()=>StateV40("_WAIT")))yield return s;
            string child=spec.kind=="nasod-rail-house"?"NUI_NASOD_RAIL":spec.kind=="mousehouse"?"NUI_MOUSE_NODROP":"NUI_NASOD_MOUSE";
            // Natural house population caps include already living room actors. Defeat existing
            // members through the public hit path to leave a slot for the selected source owner.
            int firstEvent=game.npcSummonEvents.Count;
            var resident=new List<Enemy>(game.enemies);
            foreach(var e in resident)if(e.active && !e.dormant && e.hp>0 &&
                (e.sourceNPC==child || spec.kind=="mousehouse" && e.sourceNPC=="NUI_MOUSE")) {
                foreach(var s in DamageV40(e,1000000))yield return s;
            }
            foreach(var s in Until(()=>SpawnedAtHouseV40(child,firstEvent),"Original same-position committed house summon",55))yield return s;
            Need(game.enemies.Exists(e=>e.sourceNPC==child && e.hp>0 && !resident.Contains(e)),"New live child appears in the ordinary room");
            // Rail and Nasod mouse houses finish their armored summon pose after the birth packet.
            foreach(var s in Until(()=>StateV40("_WAIT") && !game.EnemyHasSuperArmor(target),
                "Original vulnerable house WAIT after the child was emitted"))yield return s;
            foreach(var s in DamageV40(target,17))yield return s;
            foreach(var s in ObserveV40("02-damage",()=>StateV40("_DAMAGE")))yield return s;
            foreach(var s in DamageV40(target,1000000))yield return s;
            foreach(var s in ObserveV40("03-breaking",()=>StateV40("_DYING") && target.deathTime>.15f))yield return s;
            report.changedArt=true;
        }
        IEnumerable TrapIntervalV40(string action,Func<bool> condition,float standOffset,bool imageRequired) {
            double until=Time.realtimeSinceStartupAsDouble+65;
            while(!condition() || imageRequired && !InViewV40()) {
                float x=target.x+standOffset;
                var floor=target.groundLine>=0?game.Surface(target.groundLine):null;
                if(floor==null || x<Math.Min(floor.x1,floor.x2) || x>Math.Max(floor.x1,floor.x2))
                    floor=game.SurfaceBelow(x,target.y+.1f,true);
                if(floor==null)floor=game.SurfaceBelow(x,target.y+160,true);
                Need(floor!=null,"Source platform near trap "+spec.npc);
                game.player.x=x;game.player.y=floor.Y(x);game.player.groundLine=floor.id;
                game.player.vx=game.player.vy=0;game.player.invincible=60;
                report.last=action+" "+target.npcState+"/"+target.npcTime+"/line "+target.groundLine;
                Need(Time.realtimeSinceStartupAsDouble<until,"Natural trap "+report.last);
                yield return null;Check();
            }
            if(imageRequired){
                Need(ReferenceEquals(Current().sprite.texture,Resources.Load<Texture2D>("Pixel/combat-machines-v40/"+spec.kind)),
                    "Trap rendered with its illustrated native page");
                Capture(action);
            }
        }
        IEnumerable MineV40() {
            Need(game.PlainMineOriginalReadyImplementedR1269,"Original Recycle spatial conditions and effects");
            bool previousTrace=game.traceNpcEffects;game.traceNpcEffects=true;
            int firstEvent=game.npcEffectEvents.Count;
            foreach(var s in TrapIntervalV40("01-wait",()=>StateV40("_WAIT"),-50,true))yield return s;
            foreach(var s in TrapIntervalV40("02-ready",()=>StateV40("_READY_EXPLODE"),-4,true))yield return s;
            foreach(var s in TrapIntervalV40("03-pre-explosion",()=>StateV40("_EXPLODE") && target.npcTime<.7f,-4,true))yield return s;
            foreach(var s in TrapIntervalV40("04-source-hide",()=>{
                bool visible=true,gauge=true;game.ApplyPlainMineSourceVisibilityR1302(target,ref visible,ref gauge);
                return !visible && !gauge && !Current().gameObject.activeInHierarchy;
            },-4,false))yield return s;
            Need(game.npcEffectEvents.FindIndex(firstEvent,e=>e.type=="spawn" && e.npc==spec.npc && e.effect=="MINE_EXPLOSION" &&
                Math.Abs(e.x-target.x)<.15f)>=0,"Mine owner emitted its original damage effect at the source station");
            game.traceNpcEffects=previousTrace;
            report.changedArt=true;
        }
        IEnumerable StoneV40() {
            foreach(var s in TrapIntervalV40("01-warning",()=>StateV40("_ATTACK_BEFORE"),-12,false))yield return s;
            float suspended=target.y;
            foreach(var s in TrapIntervalV40("02-fall",()=>StateV40("_ATTACK") && target.y<suspended-1,-12,true))yield return s;
            foreach(var s in TrapIntervalV40("03-landed",()=>StateV40("_WAIT2") && target.groundLine>=0,-12,true))yield return s;
            Need(target.y<suspended-1,"Rock released original support and landed");
            foreach(var s in DamageV40(target,1000000))yield return s;
            foreach(var s in TrapIntervalV40("04-fragments",()=>StateV40("_BROKEN") && target.npcTime>.3f,-12,true))yield return s;
            report.changedArt=true;
        }
        IEnumerable StakesV40() {
            foreach(var s in TrapIntervalV40("01-floor",()=>StateV40("_WAIT"),-40,true))yield return s;
            var rest=Current().sprite.rect;
            foreach(var s in TrapIntervalV40("02-extension",()=>StateV40("_ATTACK") && Current().sprite && Current().sprite.rect!=rest &&
                target.npcTime>=1.3f && target.npcTime<1.6f,-8,true))yield return s;
            foreach(var s in TrapIntervalV40("03-retracted",()=>StateV40("_WAIT"),-8,true))yield return s;
            report.changedArt=true;
        }
        IEnumerable TotemV40() {
            foreach(var s in ObserveV40("01-intact",()=>StateV40("_WAIT")))yield return s;
            bool previousTrace=game.traceNpcEffects;game.traceNpcEffects=true;
            int before=game.npcEffectEvents.Count;
            foreach(var s in ObserveV40("02-fire",()=>StateV40("_YOGA_FIRE") && target.npcTime>=3.61f))yield return s;
            var rule=((NpcLakeSourceDocumentV18)Field(game,"npcLakeSourceV18")).totems[0];
            float x=target.x+target.facing*(rule.originX+250*.28f),y=target.y+rule.originY-10*.28f;
            Need(game.npcEffectEvents.FindIndex(before,e=>e.type=="spawn" && e.npc==spec.npc && e.effect==rule.sourceEffect &&
                Math.Abs(e.x-x)<.15f && Math.Abs(e.y-y)<.15f)>=0,"Original totem fire effect at its selected source dummy");
            game.traceNpcEffects=previousTrace;
            var definition=(NpcDefinition)((IDictionary)Field(game,"npcDefinitions"))[spec.npc];
            var rest=Array.Find(definition.states,s=>s.id=="BESMA_TOTEM_BASE_WAIT");
            var attack=Array.Find(definition.states,s=>s.id=="BESMA_TOTEM_BASE_YOGA_FIRE");
            var hurt=Array.Find(definition.states,s=>s.id=="BESMA_TOTEM_BASE_DAMAGE");
            Need(rest.superArmor && attack.superArmor && !hurt.superArmor && hurt.clip=="hurt" &&
                hurt.sourceAnimation=="Damage" && Math.Abs(hurt.duration-34f/60)<.0001f,"Original totem armor and authored Damage clock");
            var library=((IDictionary)Field(scene,"libraries"))[spec.kind];
            var hurtFrames=((Dictionary<string,Sprite[]>)Field(library,"frames"))["hurt"];
            Need(hurtFrames.Length==34 && hurtFrames[0].rect!=hurtFrames[11].rect &&
                hurtFrames[11].rect!=hurtFrames[22].rect && hurtFrames[22].rect!=hurtFrames[33].rect &&
                ReferenceEquals(hurtFrames[11].texture,Current().sprite.texture),"Registered authentic four-key Damage bank");
            float hp=target.hp,from=target.npcTime;
            Need(StateV40("_YOGA_FIRE") && game.EnemyHasSuperArmor(target),"Original active totem armor before public hit");
            foreach(var s in DamageV40(target,17))yield return s;
            yield return null;Check();
            Need(target.hp==hp-17 && StateV40("_YOGA_FIRE") && target.npcTime>=from && game.EnemyHasSuperArmor(target),
                "Nonlethal hit reduced HP while the original armored fire state continued");
            Capture("03-armored-hit");
            report.actions.Add("Verified the four authored Damage exposures separately; WAIT and YOGA_FIRE both have original SUPER_ARMOR.");
            foreach(var s in DamageV40(target,1000000))yield return s;
            foreach(var s in ObserveV40("04-broken",()=>StateV40("_DYING") && target.deathTime>.25f))yield return s;
            report.changedArt=true;
        }
    }
}
#endif
