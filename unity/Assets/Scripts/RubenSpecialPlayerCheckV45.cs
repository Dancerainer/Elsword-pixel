#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed partial class GiantWilliamPlayerCheckV39 {
        [Serializable] sealed class RubenSpecialCaseV45 {
            public string npc,ready,fire,originalMotion,effect;
            public float hpFraction,emissionClock;public int damage;
            public PixelProof preparing,attacking,stoneWall;
        }
        [Serializable] sealed class RubenSpecialReceiptV45 {
            public List<RubenSpecialCaseV45> cases=new List<RubenSpecialCaseV45>();
            public PixelProof magicStone;
        }
        [Serializable] sealed class RubenPaintCaseV45 {public string kind,stage,sourceState,motion,alias;public PixelProof pixels;}
        [Serializable] sealed class RubenPaintReceiptV45 {public List<RubenPaintCaseV45> frames=new List<RubenPaintCaseV45>();}
        IEnumerable RubenPaintV45() {
            var accepted=new RubenPaintReceiptV45();
            foreach(var pair in new[]{new[]{"tree","thief"},new[]{"swamp","benders-thief"},new[]{"ruins","william-cute","william-thief","tree-knight"}}){
                foreach(var step in EnterRoute(scene,"ruben",pair[0]))yield return step;
                game=(GameModel)Field(scene,"game");game.player.maxHp=game.player.hp=5000;
                for(int i=1;i<pair.Length;i++){
                    actor=game.enemies.Find(x=>x.kind==pair[i]&&x.aiEnabled);Need(actor!=null,"Original birth "+pair[i]);
                    rendererView=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
                    foreach(var step in RubenPaintActorV45(accepted))yield return step;
                }
                Press(scene,"dungeons");for(int f=0;f<6;f++)yield return null;
            }
            File.WriteAllText(Path.Combine(output,"ruben-paint.json"),JsonUtility.ToJson(accepted,true));
        }
        IEnumerable RubenPaintActorV45(RubenPaintReceiptV45 accepted){
            var seen=new HashSet<string>();double limit=Time.realtimeSinceStartupAsDouble+38,nextPlace=0;
            while(seen.Count!=3){
                yield return null;report.last=actor.kind+" "+actor.npcState+" "+(rendererView.sprite?rendererView.sprite.name:"none")+" seen="+string.Join(",",seen);
                Need(Time.realtimeSinceStartupAsDouble<limit,report.last);
                if(Time.realtimeSinceStartupAsDouble>=nextPlace){
                    Place(seen.Contains("walk")?55:actor.kind=="william-thief"?207:155);
                    nextPlace=Time.realtimeSinceStartupAsDouble+.18;
                }
                if(actor.state!=CombatState.Normal||string.IsNullOrEmpty(actor.npcState)||!rendererView.sprite)continue;
                var state=actor.npcState;string action=state.EndsWith("_WALK")?"walk":state.EndsWith("_ATTACK_READY")?"ready":state.EndsWith("_ATTACK_A")?"fire":null;
                if(action==null||seen.Contains(action)||actor.npcTime<.042f)continue;
                string alias="source-"+(action=="walk"?"walk":actor.kind=="tree-knight"?"attack":"attack-"+action);
                var views=new List<SpriteRenderer>();string motion;
                if(actor.kind=="tree-knight"){
                    var owners=(IDictionary)Field(scene,"treeKnightA1ViewsV30");if(!owners.Contains(actor))continue;
                    var parts=(TreeKnightAuthoredPartsV30)Field(owners[actor],"parts");
                    motion=action=="walk"?"Walk":parts.VisibleAttackMotionV45;if(motion==null)continue;
                    parts.VisitRenderersR1461(views.Add);
                }else if(actor.kind.StartsWith("william-")){
                    var painted=(IDictionary)Field(scene,"rubenForestByPaintV40");if(!painted.Contains(rendererView.sprite))continue;
                    var current=game.RubenActionForDisplayV45(actor);if(current==null)continue;
                    views.Add(rendererView);motion=current.sourceAnimation;
                }else{
                    var owners=(IDictionary)Field(scene,"bendersOwnersV36");if(!owners.Contains(actor))continue;
                    var parts=(FairyAuthoredPartsV33)Field(owners[actor],"parts");motion=parts.LastFrame.alias;
                    views.AddRange((SpriteRenderer[])Field(owners[actor],"members"));
                }
                if(!rendererView.sprite.name.StartsWith(actor.kind+"/"+alias+"/",StringComparison.Ordinal))continue;
                var pixels=CaptureRenderersAtResolution((Camera)Field(scene,"worldCamera"),game,actor,views,output,actor.kind+"-"+action,1280,720,false);
                if(pixels.count<16)continue;
                accepted.frames.Add(new RubenPaintCaseV45{kind=actor.kind,stage=game.CurrentRoom.id,sourceState=state,motion=motion,alias=alias,pixels=pixels});
                seen.Add(action);
            }
        }
        IEnumerable RubenSpecialV45() {
            var accepted=new RubenSpecialReceiptV45();
            foreach(var step in NaturalSpecialV45(true,accepted))yield return step;
            foreach(var step in Leave())yield return step;
            foreach(var step in CaptureThiefStoneV45(accepted))yield return step;
            foreach(var step in EnterRoute(scene,"ruben","tree"))yield return step;
            game=(GameModel)Field(scene,"game");Call(game,"EnterRoom",2);
            for(int i=0;i<4;i++)yield return null;
            actor=game.enemies.Find(x=>x.sourceNPC=="NUI_BENDERS_NORMAL");
            Need(actor!=null&&actor.active&&!actor.dormant&&game.CurrentRoom.id=="tree-3","Original Benders encounter");
            rendererView=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
            game.player.hp=game.player.maxHp=5000;game.player.invincible=100;
            foreach(var step in NaturalSpecialV45(false,accepted))yield return step;
            File.WriteAllText(Path.Combine(output,"ruben-specials.json"),JsonUtility.ToJson(accepted,true));
            Press(scene,"dungeons");for(int i=0;i<6;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons","Return from Ruben");
        }
        PixelProof ProjectilePixelsV45(string id,string label){
            var actual=new List<SpriteRenderer>();
            foreach(var view in (List<SpriteRenderer>)Field(scene,"npcEffectViews"))
                if(view&&view.sprite&&view.enabled&&view.gameObject.activeInHierarchy&&
                   view.sprite.name.StartsWith(id+"/",StringComparison.Ordinal))actual.Add(view);
            if(actual.Count==0)return null;
            var pix=CaptureRenderersAtResolution((Camera)Field(scene,"worldCamera"),game,actor,actual,output,label,1280,720,false);
            return pix.count>=16?pix:null;
        }
        IEnumerable CaptureThiefStoneV45(RubenSpecialReceiptV45 accepted){
            foreach(var step in EnterRoute(scene,"ruben","ruins"))yield return step;
            game=(GameModel)Field(scene,"game");Need(game.room==0,"Original ruins entry");
            actor=game.enemies.Find(x=>x.kind=="william-thief"&&x.aiEnabled);
            Need(actor!=null,"Original sorcerer birth");
            game.player.hp=game.player.maxHp=5000;game.player.invincible=180;
            game.traceNpcEffects=true;double end=Time.realtimeSinceStartupAsDouble+40,nextPlace=0;
            while(accepted.magicStone==null){
                yield return null;report.last="Ruben sorcerer "+actor.npcState;
                Need(Time.realtimeSinceStartupAsDouble<end,report.last);
                if(Time.realtimeSinceStartupAsDouble>=nextPlace){Place(60);nextPlace=Time.realtimeSinceStartupAsDouble+.4;}
                bool emitted=game.npcEffectEvents.Exists(e=>e.npc==actor.sourceNPC&&e.effect=="DamageEffect_PPORU_WILLIAM_CUTE_THIEF_ATTACK_A"&&e.type=="spawn");
                var view=((List<SpriteRenderer>)Field(scene,"enemyViews"))[game.enemies.IndexOf(actor)];
                if(emitted&&view.sprite&&view.sprite.name.StartsWith("william-thief/source-attack-fire/",StringComparison.Ordinal)&&
                   ((IDictionary)Field(scene,"rubenForestByPaintV40")).Contains(view.sprite))
                    accepted.magicStone=ProjectilePixelsV45("rubenfx-william-stone","projectile-thief");
            }
            Press(scene,"dungeons");for(int i=0;i<6;i++)yield return null;
        }
        IEnumerable NaturalSpecialV45(bool william,RubenSpecialReceiptV45 accepted){
            string fire=william?"WILLIAM_PPORU_SPECIAL_ATTACK":"BENDERS_NORMAL_SPECIAL_ATTACK_A";
            var c=new RubenSpecialCaseV45 {npc=actor.sourceNPC,fire=fire,ready=fire+"_READY",
                originalMotion=william?"Elsword_MegaSlash_Fire":"SpecialAttackA_Fire",
                effect="DamageEffect_"+fire};
            game.traceNpcEffects=true;
            foreach(var step in Vulnerable())yield return step;
            float damage=Mathf.Ceil(actor.hp-actor.maxHp*.32f);
            Need(damage>0&&game.ApplyHitToEnemy(actor,(int)damage,HitReaction.None)>0,"Real hit opens special attack");
            c.hpFraction=actor.hp/actor.maxHp;Need(c.hpFraction>0&&c.hpFraction<.4f,"Source special attack HP condition");
            double deadline=Time.realtimeSinceStartupAsDouble+60,nextPlace=0;
            while(c.preparing==null||c.attacking==null||c.damage<=0||(!william&&c.stoneWall==null)){
                yield return null;report.last="Ruben "+c.npc+" "+actor.npcState;
                Need(Time.realtimeSinceStartupAsDouble<deadline,report.last+" special images="+(c.attacking!=null)+" damage="+c.damage);
                if(Time.realtimeSinceStartupAsDouble>=nextPlace){
                    if(actor.npcState==c.fire)Place(william?72:45,actor.facing);else Place(62);
                    game.player.invincible=0; nextPlace=Time.realtimeSinceStartupAsDouble+.22;
                }
                bool ready=actor.npcState==c.ready,attack=actor.npcState==c.fire;
                if((ready&&c.preparing==null)||(attack&&c.attacking==null)){
                    var renderers=new List<SpriteRenderer>();
                    if(william){
                        var frame=scene.GiantWilliamFrameV39For(rendererView);
                        if(frame==null||frame.selected.motion!=(ready?"Elsword_MegaSlash_Ready":c.originalMotion))continue;
                        renderers.Add(rendererView);
                    } else{
                        var owners=(IDictionary)Field(scene,"bendersOwnersV36");
                        if(!owners.Contains(actor))continue;
                        var owner=owners[actor];
                        var parts=(SpriteRenderer[])Field(owner,"members");if(parts==null)continue;
                        renderers.AddRange(parts);
                    }
                    var pixels=CaptureRenderersAtResolution((Camera)Field(scene,"worldCamera"),game,actor,renderers,output,
                        (william?"william":"benders")+"-"+(ready?"preparing":"attacking"),1280,720,false);
                    if(pixels.count>0){if(ready)c.preparing=pixels;else c.attacking=pixels;}
                }
                foreach(var ev in game.npcEffectEvents)if(ev.npc==actor.sourceNPC&&ev.effect.StartsWith(c.effect,StringComparison.Ordinal)){
                    if(ev.type=="spawn")c.emissionClock=ev.time;
                    if(ev.type=="hit-player"&&ev.damage>0)c.damage=Math.Max(c.damage,ev.damage);
                }
                if(!william&&c.stoneWall==null)c.stoneWall=ProjectilePixelsV45("rubenfx-benders-stones","projectile-wall");
            }
            accepted.cases.Add(c);
        }
    }
}
#endif
