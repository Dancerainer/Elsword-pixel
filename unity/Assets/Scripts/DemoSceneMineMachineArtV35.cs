using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class MachineDrawV35 {
            public int frame;public string kind,alias;public float clock;public Sprite original;public SpriteRenderer carrier;public int index;
        }
        sealed class MachineViewV35 {
            public SpriteRenderer carrier;public FairyAuthoredPartsV33 parts;
        }
        readonly Dictionary<SpriteRenderer,MachineDrawV35> machineDrawsV35=new Dictionary<SpriteRenderer,MachineDrawV35>();
        readonly Dictionary<Enemy,MachineViewV35> machineViewsV35=new Dictionary<Enemy,MachineViewV35>();
        readonly HashSet<Enemy> machineTouchedV35=new HashSet<Enemy>();
        readonly HashSet<Enemy> machineFailedV35=new HashSet<Enemy>();
        readonly List<Enemy> machineStaleV35=new List<Enemy>();
        FairyPoseLibraryV33 machineBankV35;
        string machineKindV35;
        int machineMissingRailDrawsV35;

        void SelectMineMachineOriginalPoseV35(Enemy actor,ref string alias,ref float clock) {
            if(game.TryMineMachineOriginalPoseV35(actor,out var sourceAlias,out var sourceClock)){
                alias=sourceAlias;clock=sourceClock;
            }
        }

        Library mine2RailLibraryV35;
        NpcDefinition mine2RailProfileV35;
        readonly Dictionary<Enemy,MachineDrawV35> mine2RailFramesV35=new Dictionary<Enemy,MachineDrawV35>();
        int mine2RailPublishedV35;
        string mine2RailLastPhaseV35;
        int mine2RailReboundSpritesV37;

        // Mine2 creates and owns a private source library for the life of its
        // loaded route. Select its exact original Sprite once in RenderActors,
        // before applying the native camera projection or material properties.
        bool TryRenderMine2RailOriginalV35(Enemy actor,SpriteRenderer carrier,int actorIndex) {
            if(!Mine2ProductSessionActiveR1233 || actor.kind!=MineMachineRigV35.Rail ||
                actor.sourceNPC!=MineMachineRigV35.RailId)return false;
            if(mine2RailProfileV35==null || mine2RailLibraryV35==null)
                throw new InvalidOperationException("Missing owned Mine2 Rail profile/library");
            string state=actor.npcState;
            float time=actor.npcTime;
            if(string.IsNullOrEmpty(state)) {
                if(actor.hp<=0 && actor.state==CombatState.Dead){state="NASOD_RAIL_DYING_LAND";time=actor.deathTime;}
                else if(actor.state==CombatState.Hurt){state="NASOD_RAIL_DAMAGE";time=actor.clock;}
                else return false;
            }
            var source=Array.Find(mine2RailProfileV35.states,q=>q.id==state);
            if(source==null)throw new InvalidOperationException("Missing current Mine2 Rail source state "+state);
            string alias=source.clip;
            var loader=mine2RailLibraryV35;
            var clip=loader.clips[alias];var frames=loader.frames[alias];
            float motionClock=time*source.sourceAnimSpeed;
            if(clip.loop)motionClock=Mathf.Repeat(motionClock,clip.duration);
            RecordMonsterPixelSourceV53(actor,carrier,"Mine2/Rail",alias,source.sourceAnimation,motionClock);
            int index=Mathf.Clamp(Mathf.FloorToInt(motionClock*clip.fps),0,frames.Length-1);
            var selected=frames[index];carrier.sprite=selected;
            float scale=loader.data.logicalScale*actor.visualScale;
            carrier.transform.localScale=Vector3.one*scale;
            carrier.flipX=OwnedSourceFramingR1111?actor.facing<0:Generic7NormalFlipXV28(actor,actorIndex,alias,actor.facing<0);
            var position=RenderEnemyPosition(actor)+SpriteOffset(carrier)*scale;
            carrier.transform.localPosition=new Vector3(position.x,position.y,0);
            if(!mine2RailFramesV35.TryGetValue(actor,out var frame)){
                frame=new MachineDrawV35();mine2RailFramesV35.Add(actor,frame);
            }
            frame.frame=Time.frameCount;frame.kind=actor.kind;frame.alias=alias;
            frame.clock=motionClock;frame.original=selected;frame.carrier=carrier;frame.index=index;mine2RailPublishedV35++;
            return true;
        }

        void RecordMineMachinePoseV35(SpriteRenderer carrier,string kind,string alias,float clock){
            if((kind!=MineMachineRigV35.Rail&&kind!=MineMachineRigV35.Driller)||!carrier||!carrier.sprite)return;
            if(!machineDrawsV35.TryGetValue(carrier,out var draw)){
                draw=new MachineDrawV35();machineDrawsV35.Add(carrier,draw);
            }
            draw.frame=Time.frameCount;draw.kind=kind;draw.original=carrier.sprite;
            draw.alias=alias;draw.clock=clock;
        }
        void PrepareDrillerPixelSelectionsV53(){
            if(!PlainProductSessionActiveR1320||game==null||!actorsRoot||!worldRoot)return;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var actor=game.enemies[i];var carrier=enemyViews[i];
                if(actor.kind!=MineMachineRigV35.Driller||actor.sourceNPC!=MineMachineRigV35.DrillerId||!MonsterPixelRegisteredV53(actor,out _))continue;
                if(!carrier||!machineDrawsV35.TryGetValue(carrier,out var draw)||draw.frame!=Time.frameCount||draw.kind!=MineMachineRigV35.Driller||!carrier.sprite||carrier.sprite!=draw.original)continue;
                try{
                    ReadDrillerPixelClockV53(draw.alias,draw.clock,out string motion,out float sourceClock);
                    RecordMonsterPixelSourceV53(actor,carrier,"Plain/Driller",draw.alias,motion,sourceClock,carrier.flipX?-1:1);
                }catch(Exception error){FailMonsterPixelV53(actor,"Plain/Driller",draw.alias,"",draw.clock,error);}
            }
        }
        bool MachineOwnsRendererV35(Renderer r){
            foreach(var view in machineViewsV35.Values)if(view.parts.OwnsRenderer(r))return true;
            return false;
        }
        void DiscardMachineV35(Enemy actor){
            if(!machineViewsV35.TryGetValue(actor,out var view))return;
            machineViewsV35.Remove(actor);view.parts.Dispose();
        }
        void DisposeMachineViewsV35(){
            foreach(var view in machineViewsV35.Values)view.parts.Dispose();
            machineViewsV35.Clear();mine2RailFramesV35.Clear();machineDrawsV35.Clear();machineFailedV35.Clear();machineTouchedV35.Clear();machineStaleV35.Clear();
            if(machineBankV35!=null)machineBankV35.ReleaseOwned(DestroyNativeWorldObject);
            machineBankV35=null;machineKindV35=null;
        }
        // Pose has already resolved model clip fallback, original looping clock and exact source sprite.
        void UpdateMachineViewsV35(){
            if(game==null||!actorsRoot||!worldRoot||(!Mine2ProductSessionActiveR1233&&!PlainProductSessionActiveR1320)){
                if(machineKindV35!=null||machineViewsV35.Count>0||machineDrawsV35.Count>0)DisposeMachineViewsV35();
                return;
            }
            string expected=Mine2ProductSessionActiveR1233?MineMachineRigV35.Rail:MineMachineRigV35.Driller;
            string npc=Mine2ProductSessionActiveR1233?MineMachineRigV35.RailId:MineMachineRigV35.DrillerId;
            machineTouchedV35.Clear();
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                Enemy actor=game.enemies[i];if(MonsterPixelOwnsActorV53(actor)){DiscardMachineV35(actor);continue;}if(actor.sourceNPC!=npc||actor.kind!=expected)continue;
                machineTouchedV35.Add(actor);
                if(machineFailedV35.Contains(actor))continue;
                var carrier=enemyViews[i];
                if(StreamMobileArt&&carrier&&(!actor.active||actor.dormant)&&!carrier.gameObject.activeSelf)continue;
                try{
                    if(!carrier){DiscardMachineV35(actor);continue;}
                    MachineDrawV35 draw;
                    if(expected==MineMachineRigV35.Rail){
                        if(!mine2RailFramesV35.TryGetValue(actor,out draw) || draw.frame!=Time.frameCount || draw.carrier!=carrier){
                            machineMissingRailDrawsV35++;mine2RailLastPhaseV35="no current producer "+actor.npcState;
                            DiscardMachineV35(actor);continue;
                        }
                        if(!MonsterTextureRedrawV22.SameSourceFrameV35(carrier.sprite,draw.original))
                            throw new InvalidOperationException("Mine2 carrier lost its private source frame "+draw.alias+"/"+draw.index);
                        if(!ReferenceEquals(carrier.sprite,draw.original))mine2RailReboundSpritesV37++;
                        mine2RailLastPhaseV35=draw.alias+"/"+draw.index;
                    }else if(!machineDrawsV35.TryGetValue(carrier,out draw)||draw.frame!=Time.frameCount||
                        draw.kind!=expected||!carrier.sprite||carrier.sprite!=draw.original){DiscardMachineV35(actor);continue;}
                    if(machineKindV35==null){machineKindV35=expected;machineBankV35=MineMachineRigV35.Load(expected);}
                    if(machineBankV35==null)continue;
                    var track=machineBankV35.Find(draw.alias);
                    if(track==null)throw new InvalidOperationException("Original displayed machine clip "+draw.alias);
                    if(machineViewsV35.TryGetValue(actor,out var view)&&view.carrier!=carrier){DiscardMachineV35(actor);view=null;}
                    if(view==null){view=new MachineViewV35{carrier=carrier,parts=new FairyAuthoredPartsV33(carrier,machineBankV35,DestroyNativeWorldObject)};machineViewsV35.Add(actor,view);}
                    var point=RenderEnemyPosition(actor);
                    Vector3 sourceFoot=actorsRoot.TransformPoint(new Vector3(point.x,point.y,0));
                    Vector3 projected=ProjectNativePointWithEye(sourceFoot,actor.nativeDepth,out float eye);
                    var frame=new FairyAuthoredPartsV33.Frame{alias=draw.alias,sourceClock=draw.clock,facing=carrier.flipX?-1:1,
                        visible=actor.active&&!actor.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy};
                    view.parts.Apply(frame,projected);
                    if(expected==MineMachineRigV35.Rail)MineMachineRigV35.SeatPusherKnee(track,view.parts.Members);
                    foreach(var member in view.parts.Members)BindProjectedSceneOcclusion(member,sourceFoot,actor.nativeDepth,eye);
                    if(actor.hp>0&&i<enemyBars.Count&&enemyBars[i]&&enemyBars[i].enabled&&view.parts.TryVisibleTop(out float top)){
                        var bar=enemyBars[i].transform.position;bar.y=top+8;enemyBars[i].transform.position=bar;
                        if(i<enemyDebuffViews.Count&&enemyDebuffViews[i]){
                            var label=enemyDebuffViews[i].transform.position;label.y=top+20;enemyDebuffViews[i].transform.position=label;
                        }
                    }
                }catch(Exception e){DiscardMachineV35(actor);machineFailedV35.Add(actor);Debug.LogWarning("MINE_MACHINE_PARTS_FALLBACK_V35 "+actor.sourceNPC+" "+e.Message);}
            }
            machineStaleV35.Clear();
            foreach(var actor in machineViewsV35.Keys)if(!machineTouchedV35.Contains(actor))machineStaleV35.Add(actor);
            foreach(var actor in machineStaleV35)DiscardMachineV35(actor);
            machineFailedV35.RemoveWhere(e=>!machineTouchedV35.Contains(e));machineDrawsV35.Clear();
            if(machineTouchedV35.Count==0&&machineKindV35!=null)DisposeMachineViewsV35();
        }
    }
}
