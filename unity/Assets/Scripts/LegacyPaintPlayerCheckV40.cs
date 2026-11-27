#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        partial void TraceLegacyPaintSelectionV40(Enemy actor,SpriteRenderer carrier,string alias,float clock,int index){
            GuardAndTreePlayerCheckV35.TraceV40(actor,carrier,alias,clock,index);
        }
    }
    public sealed partial class GuardAndTreePlayerCheckV35 {
        [Serializable] sealed class PaintFrameV40{
            public string operation,image,sourceAlias,paintClip,texture,shaRGBA,stage;
            public int nativeIndex,paintIndex,nativeFrames,paintFrames,facing,pixels;
            public float sourceClock,sourceDuration,paintDuration,bodyScale,nativeRelativeBody;
        }
        [Serializable] sealed class PaintCombatV40{
            public string kind,npc;public int damage,serial;public float hpBefore,hpAfter,time;
            public float sourceAttackEntered=-1,spawnTime=-1,expectedSpawnAfterEntry=-1;
            public string damageEffect;
            public float firstHitAt=-1,repeatHitAt=-1,fireWaitAt=-1;
            public int repeatDamage,unreachedEndFrames,unreachedEndPaintedKeys,reentryDamage;
            public List<PaintFrameV40> frames=new List<PaintFrameV40>();
        }
        readonly PaintCombatV40 paintLogV40=new PaintCombatV40();
        static GuardAndTreePlayerCheckV35 tracingPaintV40;
        SpriteRenderer selectedCarrierV40;string selectedAliasV40;float selectedClockV40;int selectedIndexV40,selectedAtV40;
        internal static void TraceV40(Enemy actor,SpriteRenderer carrier,string alias,float clock,int index){
            var probe=tracingPaintV40;if(!probe||actor==null||actor!=probe.enemy)return;
            probe.selectedCarrierV40=carrier;probe.selectedAliasV40=alias;probe.selectedClockV40=clock;probe.selectedIndexV40=index;probe.selectedAtV40=Time.frameCount;
        }
        static bool PaintModeV40(string mode){return mode=="paint-v14-thief"||mode=="paint-v14-fire"||mode=="paint-v14-monkey"||
            mode=="paint-ruben-cute"||mode=="paint-ruben-thief"||mode=="paint-forest-wisp"||mode=="paint-forest-knight";}
        static string PaintKindV40(string mode){switch(mode){
            case "paint-v14-thief":return "pporu-william-thief";case "paint-v14-fire":return "pporu-william-thief-fire";case "paint-v14-monkey":return "monkey-bomb";
            case "paint-ruben-cute":return "william-cute";case "paint-ruben-thief":return "william-thief";case "paint-forest-wisp":return "curse-wisp";default:return "anger-forest-knight";}}
        string PaintRouteV40(){return report.mode.StartsWith("paint-v14-",StringComparison.Ordinal)?"wally-castle-outskirts":report.mode.StartsWith("paint-ruben-",StringComparison.Ordinal)?"ruins":"dark-forest";}
        string PaintFirstRoomV40(){string route=PaintRouteV40();return route=="ruins"?"ruins-1":route=="dark-forest"?"forest-1":"outskirts-1";}
        void PaintRoomV40(){
            bool forest=PaintRouteV40()=="dark-forest";int slot;
            switch(report.mode){case "paint-v14-thief":slot=1;break;case "paint-v14-fire":slot=2;break;case "paint-v14-monkey":slot=6;break;
                case "paint-ruben-cute":slot=0;break;case "paint-ruben-thief":slot=1;break;case "paint-forest-wisp":slot=0;break;default:slot=3;break;}
            Call(game,"EnterRoom",forest?1:0);enemy=game.enemies[slot];
            if(enemy.encounterIndex!=game.EncounterIndex)Call(game,"BeginEncounter",enemy.encounterIndex);Call(scene,"RebuildRoom");
            Need(enemy.kind==PaintKindV40(report.mode)&&enemy.active&&!enemy.dormant,"Real original NPC in its source room");selectedAtV40=-1;
            report.npc=enemy.sourceNPC;report.room=game.CurrentRoom.id;paintLogV40.npc=enemy.sourceNPC;paintLogV40.kind=enemy.kind;
        }
        IEnumerable ClearPaintCompetitionV40(){
            double end=Time.realtimeSinceStartupAsDouble+42;int hits=0;
            for(;;){
                Enemy protectedActor=null;bool remaining=false;
                foreach(var other in game.enemies){
                    if(other==enemy||!other.active||other.dormant||other.isGate||other.passive||other.hp<=0)continue;
                    remaining=true;
                    int dealt=game.ApplyHitToEnemy(other,100000,HitReaction.None);
                    if(dealt>0){Need(other.hp==0,"Public competing attacker HP "+other.kind);hits++;}
                    else if(protectedActor==null)protectedActor=other;
                }
                if(!remaining){report.events.Add("Cleared "+hits+" original competing attackers by public damage.");yield break;}
                if(protectedActor==null)continue;
                // Original Tree Knights start buried in the Ruins. A public
                // hit must be rejected until the real opening exposes them.
                game.player.x=protectedActor.x+28;game.player.y=protectedActor.y;game.player.groundLine=protectedActor.groundLine;
                game.player.vx=game.player.vy=0;game.player.invincible=30;yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Waiting for actual original competing hurtbox "+protectedActor.sourceNPC+
                    " state="+protectedActor.npcState+" opening="+game.EnemyInitialPosePhaseV25(protectedActor));
            }
        }
        void PaintRowV40(Observation row){
            Need(selectedAtV40==Time.frameCount&&selectedCarrierV40&&selectedCarrierV40.sprite,"Actual selector witness");
            row.sourceMotion=enemy.clip;row.sourceKey=selectedAliasV40;row.sourceTime=selectedClockV40;row.phase=enemy.npcState;
        }
        void WritePaintV40(){File.WriteAllText(Path.Combine(output,"painted-source-exposures.json"),JsonUtility.ToJson(paintLogV40,true));}
        bool PaintShotV40(string label){
            var obs=Row(label);Capture(obs,true);Need(obs.eligible==1&&obs.callbacks==1&&obs.pixels>0,"Visible genuine painted carrier "+label);
            var lib=((IDictionary)Field(scene,"libraries"))[enemy.kind];
            var data=(SpriteLibraryData)Field(lib,"data");var clips=(Dictionary<string,AnimationData>)Field(lib,"clips");var frames=(Dictionary<string,Sprite[]>)Field(lib,"frames");
            var src=clips[selectedAliasV40];bool outskirts=OutskirtsPaintExposureV40.Kinds(enemy.kind);
            string key=outskirts?selectedAliasV40.Substring(7):DemoScene.RubenForestPaintKeyV40(enemy.kind,selectedAliasV40);
            Need(!string.IsNullOrEmpty(key),"Registered source alias "+selectedAliasV40);
            int nativeIndex=Mathf.Clamp(Mathf.FloorToInt(selectedClockV40*src.fps),0,src.frames.Length-1);
            Need(selectedIndexV40==nativeIndex,"Original clock and source sprite index");
            string spellKey;int spellIndex;bool spell=DemoScene.RubenSpellPoseV40(enemy.kind,selectedAliasV40,src,nativeIndex,out spellKey,out spellIndex);
            if(spell)key=spellKey;var old=clips[key];
            int drawnIndex=spell?spellIndex:outskirts?OutskirtsPaintExposureV40.Map(src,old,nativeIndex):DemoScene.RubenForestPaintIndexV40(enemy.kind,selectedAliasV40,src,old,nativeIndex);
            var expected=frames[key][drawnIndex];var actual=selectedCarrierV40.sprite;string resource=old.frames[drawnIndex].texture;
            if(string.IsNullOrEmpty(resource))resource=string.IsNullOrEmpty(old.texture)?data.texture:old.texture;
            if(outskirts)resource=SourceMotionInkV45.ResourceFor(enemy.kind,key,drawnIndex)??resource;
            resource=BluePoruInkV45.Resource(enemy.kind,resource);
            Need(actual&&actual.name==enemy.kind+"/"+selectedAliasV40+"/"+nativeIndex&&actual.rect==expected.rect&&actual.pivot==expected.pivot&&
                actual.texture==expected.texture&&Resources.Load<Texture2D>(resource)==actual.texture,"Exact existing painted image at selected original index "+label);
            Need(selectedCarrierV40.flipX==(enemy.facing<0),"Actual existing drawing follows the original facing "+enemy.facing);
            if(!outskirts)Need(frames[selectedAliasV40][nativeIndex].texture!=actual.texture,"Original V28 selector data retained independently of final paint");
            var ratio=LegacyBodyRegistrationV39.Find(enemy,PaintRouteV40(),data.artVersion);
            Need(ratio!=null&&Math.Abs(selectedCarrierV40.transform.localScale.y-ratio.factor)<.0001f,"Original character-relative scale");
            string rgba=(string)Call(scene,"FeedbackNpcTextureRgbaV25",actual.texture);
            string pin=BluePoruInkV45.Pin(resource)??PaintAtlasPinsV40.Sha(resource);
            if(pin!=null)Need(rgba==pin,"Reviewed RGBA pixels "+resource);
            else Need(SourceMotionInkV45.ResourceFor(enemy.kind,key,drawnIndex)==resource&&rgba.Length==64,"Registered V45 source drawing "+resource);
            paintLogV40.frames.Add(new PaintFrameV40{operation=label,image=obs.image,sourceAlias=selectedAliasV40,paintClip=key,texture=resource,shaRGBA=rgba,stage=game.CurrentRoom.id,
                nativeIndex=nativeIndex,paintIndex=drawnIndex,nativeFrames=src.frames.Length,paintFrames=old.frames.Length,facing=enemy.facing,pixels=obs.pixels,
                sourceClock=selectedClockV40,sourceDuration=src.duration,paintDuration=old.duration,bodyScale=ratio.factor,nativeRelativeBody=ratio.target});
            WritePaintV40();return true;
        }
        IEnumerable PaintUntilV40(Predicate<string> match,float offset,bool hittable=false,double seconds=30){
            double end=Time.realtimeSinceStartupAsDouble+seconds;
            for(;;){
                Protect(offset);if(hittable)game.player.invincible=0;yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Requested painted phase actual="+selectedAliasV40+" model="+enemy.clip+" npc="+enemy.npcState+" x="+enemy.x+" y="+enemy.y);
                if(selectedAtV40==Time.frameCount&&match(selectedAliasV40))yield break;
            }
        }
        float PaintChaseV40(){
            var profile=(NpcDefinition)Call(game,"NpcProfile",enemy);
            if(profile==null)return 150;
            // An unalerted Curse Wisp senses at 168 and attacks within 112.
            // Initial acquisition must occur inside the real sense interval.
            float upper=Math.Min(profile.senseRange,profile.lostRange);
            Need(upper>profile.approachDistance,"Original sensing/chase interval "+enemy.sourceNPC);
            return Math.Min(BendersChaseDistanceV36(),(profile.approachDistance+upper)*.5f);
        }
        float OpenPaintChaseV40(){
            int face=BendersAvailableV36(1)>BendersAvailableV36(-1)?1:-1;
            return face*PaintChaseV40();
        }
        void VerifyOwnedPaintHitV40(float before,float at){
            Need(game.player.hp<before,"Actual player HP loss inside the owner-only encounter");
            bool generic=enemy.kind=="william-thief"||enemy.kind=="william-cute",melee=enemy.kind=="anger-forest-knight";
            if(generic)Need(enemy.struck&&game.PlayerImpactSerial>0,"Generic source owner's attack impact");
            else if(melee)Need(game.npcActionEvents.Exists(a=>a.npc==enemy.sourceNPC&&a.type=="hit"&&a.time>=at&&a.damage>0),"Original Knight attack window dealt damage");
            else{
                var hit=game.npcEffectEvents.FindLast(e=>e.type=="hit-player"&&e.npc==enemy.sourceNPC&&e.time>=at&&e.damage>0);
                Need(hit!=null,"Actual owned NPC effect hit-player");paintLogV40.serial=hit.serial;
                paintLogV40.firstHitAt=hit.time;
                var born=game.npcPresentationEvents.Find(e=>e.type=="spawn"&&e.owner==enemy&&e.serial==hit.serial&&e.effect==hit.effect);
                var entry=game.npcActionEvents.FindLast(e=>e.npc==enemy.sourceNPC&&e.type=="state"&&e.state!=null&&e.state.EndsWith("_ATTACK_A",StringComparison.Ordinal)&&e.time>=at);
                Need(born!=null&&entry!=null,"Hit came from this owner's original state and spawned effect");
                float delay=enemy.kind=="pporu-william-thief"?.219f:enemy.kind=="monkey-bomb"?.001f:enemy.kind=="curse-wisp"?.01f:0;
                paintLogV40.sourceAttackEntered=entry.time;paintLogV40.spawnTime=born.time;paintLogV40.damageEffect=hit.effect;paintLogV40.expectedSpawnAfterEntry=delay;
                Need(Math.Abs(born.time-entry.time-delay)<.0005f,"Original source effect release time "+(born.time-entry.time)+" expected "+delay);
            }
            paintLogV40.damage=Mathf.RoundToInt(before-game.player.hp);paintLogV40.hpBefore=before;paintLogV40.hpAfter=game.player.hp;paintLogV40.time=game.elapsed;WritePaintV40();
        }
        IEnumerable FireRepeatV40(){
            int serial=paintLogV40.serial;float before=game.player.hp;double until=Time.realtimeSinceStartupAsDouble+8;
            for(;;){
                var live=game.npcEffects.Find(x=>x.owner==enemy&&x.serial==serial);
                Need(live!=null&&Time.realtimeSinceStartupAsDouble<until,"Original repeating fire effect in flight");
                // Hold the controllable player in the actual moving effect.
                // After the first real hit, preserve the model's invulnerability,
                // hit pause and elapsed time throughout this second contact.
                game.player.x=live.x;game.player.y=enemy.y;game.player.groundLine=enemy.groundLine;game.player.vx=game.player.vy=0;
                yield return null;Budget();
                var hit=game.npcEffectEvents.FindLast(x=>x.type=="hit-player"&&x.npc==enemy.sourceNPC&&x.serial==serial&&x.time>paintLogV40.firstHitAt+.001f&&x.damage>0);
                if(hit==null)continue;
                Need(hit.time-paintLogV40.firstHitAt>=.2f-.0005f&&game.player.hp<before,"Original fire repeat gap and new HP loss");
                paintLogV40.repeatDamage=Mathf.RoundToInt(before-game.player.hp);paintLogV40.repeatHitAt=hit.time;
                PaintShotV40("confirmed-repeat-contact");yield break;
            }
        }
        void VerifyRegisteredFireEndV40(){
            const string end="PPORU_WILLIAM_THIEF_FIRE_ATTACK_A_END";
            var profile=(NpcDefinition)Call(game,"NpcProfile",enemy);
            var fire=Array.Find(profile.states,s=>s.id=="PPORU_WILLIAM_THIEF_FIRE_ATTACK_A");var closing=Array.Find(profile.states,s=>s.id==end);
            Need(fire!=null&&closing!=null&&fire.nextState==profile.waitState&&closing.nextState==profile.waitState&&closing.sourceAnimation=="DashEnd","Original fire and independent end-state definitions");
            foreach(var state in profile.states){
                Need(state.nextState!=end&&state.onHitState!=end&&!Array.Exists(state.choices,c=>c.nextState==end)&&
                    (state.sourceEventTransitions==null||!Array.Exists(state.sourceEventTransitions,x=>x.nextState==end)),"No original inbound transition into the named end state");
            }
            var lib=((IDictionary)Field(scene,"libraries"))[enemy.kind];var clips=(Dictionary<string,AnimationData>)Field(lib,"clips");var frames=(Dictionary<string,Sprite[]>)Field(lib,"frames");
            var src=clips["source-attack-a-end"];var art=clips["attack-a-end"];var keys=new HashSet<Rect>();
            Need(src.frames.Length==74&&src.fps==60&&Math.Abs(src.duration-closing.duration)<.00001f,"Registered 74 independent end samples");
            var all=frames[src.id];var old=frames[art.id];
            for(int i=0;i<all.Length;i++){
                var rendered=all[i];var expected=old[OutskirtsPaintExposureV40.Map(src,art,i)];
                Need(rendered&&rendered.texture==expected.texture&&rendered.rect==expected.rect&&rendered.pivot==expected.pivot&&rendered.name==enemy.kind+"/"+src.id+"/"+i,"Registered existing V14 end exposure "+i);
                keys.Add(rendered.rect);
            }
            Need(keys.Count==4,"Four original end paintings");
            var stamp=game.npcActionEvents.Find(x=>x.npc==enemy.sourceNPC&&x.type=="state"&&x.state==profile.waitState&&x.time>paintLogV40.sourceAttackEntered);
            Need(stamp!=null&&stamp.time>=paintLogV40.sourceAttackEntered+fire.duration-.035f,"Natural original AttackA to WAIT after the fire motion");
            paintLogV40.fireWaitAt=stamp.time;paintLogV40.unreachedEndFrames=all.Length;paintLogV40.unreachedEndPaintedKeys=keys.Count;
            report.events.Add("Observed original ATTACK_A -> WAIT. Separate end state has no incoming source edge; verified 74 registered samples and 4 painted keys without entering that state.");WritePaintV40();
        }
        IEnumerable FireReentryV40(){
            var departed=enemy;Press("dungeons");for(int i=0;i<4;i++)yield return null;
            Need((string)Field(scene,"openMenu")=="dungeons","Return through actual UI before fire replay");
            foreach(var x in Enter("elder","wally-castle-outskirts"))yield return x;PaintRoomV40();
            game.traceNpcEffects=game.traceNpcActions=true;
            Need(!ReferenceEquals(departed,enemy)&&!game.enemies.Contains(departed)&&!game.npcEffects.Exists(e=>e.owner==departed),"Fresh ordinary fire birth without a departed effect owner");
            foreach(var x in ClearPaintCompetitionV40())yield return x;
            foreach(var x in PaintUntilV40(a=>a=="source-attack-ready",28))yield return x;
            PaintShotV40("reentry-ready");float hp=game.player.hp,entered=game.elapsed;
            foreach(var x in PaintUntilV40(a=>a=="source-attack-a"&&game.player.hp<hp,28,true))yield return x;
            var hit=game.npcEffectEvents.FindLast(x=>x.type=="hit-player"&&x.npc==enemy.sourceNPC&&x.time>=entered&&x.damage>0);
            Need(hit!=null&&game.npcPresentationEvents.Exists(x=>x.owner==enemy&&x.type=="spawn"&&x.serial==hit.serial&&x.effect==hit.effect),"Fresh reentry owner deals actual damage");
            paintLogV40.reentryDamage=Mathf.RoundToInt(hp-game.player.hp);PaintShotV40("reentry-contact");
        }
        IEnumerable LegacyPaintFlowV40(){
            tracingPaintV40=this;string route=PaintRouteV40();foreach(var x in Enter(route=="ruins"?"ruben":"elder",route))yield return x;
            bool priorFx=game.traceNpcEffects,priorActions=game.traceNpcActions;
            game.traceNpcEffects=game.traceNpcActions=true;
            try{
                foreach(int face in new[]{1,-1}){
                    PaintRoomV40();float dist=PaintChaseV40();foreach(var x in MakeBendersWalkingSpaceV36(face,dist))yield return x;
                    foreach(var x in PaintUntilV40(a=>a=="source-walk"&&enemy.facing==face,face*dist))yield return x;PaintShotV40("walk-"+face);
                }
                PaintRoomV40();foreach(var x in ClearPaintCompetitionV40())yield return x;
                foreach(var x in PaintUntilV40(a=>a.StartsWith("source-attack",StringComparison.Ordinal)&&a.EndsWith("ready",StringComparison.Ordinal),28))yield return x;
                PaintShotV40("ready");float hp=game.player.hp,t0=game.elapsed;
                foreach(var x in PaintUntilV40(a=>a.StartsWith("source-attack",StringComparison.Ordinal)&&!a.EndsWith("ready",StringComparison.Ordinal)&&!a.EndsWith("end",StringComparison.Ordinal),28,true))yield return x;
                PaintShotV40("fire");
                if(game.player.hp==hp)foreach(var x in PaintUntilV40(a=>game.player.hp<hp,28,true))yield return x;
                VerifyOwnedPaintHitV40(hp,t0);
                PaintShotV40("confirmed-contact");
                if(enemy.kind=="pporu-william-thief-fire"){
                    foreach(var x in FireRepeatV40())yield return x;
                    foreach(var x in PaintUntilV40(a=>(a=="source-idle"||a=="source-walk")&&game.npcActionEvents.Exists(
                        s=>s.npc==enemy.sourceNPC&&s.type=="state"&&s.state=="PPORU_WILLIAM_THIEF_FIRE_WAIT"&&s.time>paintLogV40.sourceAttackEntered),OpenPaintChaseV40()))yield return x;
                    VerifyRegisteredFireEndV40();PaintShotV40("natural-fire-return");
                }
                PaintRoomV40();foreach(var x in PaintUntilV40(a=>a=="source-walk"&&game.Grounded(enemy)&&!game.EnemyHasSuperArmor(enemy),OpenPaintChaseV40()))yield return x;
                Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt)>0&&enemy.state==CombatState.Hurt,"Actual public grounded wound");
                foreach(var x in PaintUntilV40(a=>a=="source-hurt",28))yield return x;PaintShotV40("hurt");
                Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Launch,280)>0&&enemy.state==CombatState.Airborne,"Actual public launch");
                foreach(var x in PaintUntilV40(a=>(a=="source-launch"||a=="source-air-up")&&enemy.vy>0,28))yield return x;PaintShotV40("launch");
                foreach(var x in PaintUntilV40(a=>(a=="source-fall"||a=="source-air-fall")&&enemy.vy<=0,28))yield return x;PaintShotV40("descent");
                foreach(var x in PaintUntilV40(a=>a=="source-down",28))yield return x;PaintShotV40("land");
                foreach(var x in PaintUntilV40(a=>a=="source-get-up",28))yield return x;PaintShotV40("stand");
                foreach(var x in PaintUntilV40(a=>a=="source-walk",OpenPaintChaseV40()))yield return x;
                Need(game.ApplyHitToEnemy(enemy,100000,HitReaction.None)>0&&enemy.hp==0,"Actual public lethal packet");
                foreach(var x in PaintUntilV40(a=>a=="source-death",28))yield return x;PaintShotV40("death");
                if(enemy.kind=="pporu-william-thief-fire")foreach(var x in FireReentryV40())yield return x;
            }finally{game.traceNpcEffects=priorFx;game.traceNpcActions=priorActions;tracingPaintV40=null;}
        }
    }
}
#endif
