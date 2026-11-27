using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly Dictionary<string,CoreStopWholeEnvelopeV22> monsterPixelStartEnvelopesV53=new Dictionary<string,CoreStopWholeEnvelopeV22>(StringComparer.Ordinal);

        int MonsterPixelFacingV53(MonsterSelectionV53 selected){
            if(selected.facing!=0)return selected.facing;
            var definition=selected.definition;
            return definition.facingPolicy=="fixed"?1:definition.facingPolicy=="carrier"?(selected.carrier.flipX?-1:1):(selected.actor.facing<0?-1:1)*definition.authoredFacing;
        }
        Vector2 MonsterPixelLogicalRootV53(MonsterSelectionV53 selected){
            var definition=selected.definition;
            float rootY=definition.rootOffsetY;
            if(definition.contactRootCurves!=null)foreach(var curve in definition.contactRootCurves){
                if(curve.motion!=selected.motion)continue;
                rootY+=curve.y[selected.frame];break;
            }
            if(selected.actor.sourceNPC=="NUI_MONKEY_BOMB" && game.Grounded(selected.actor) &&
                (selected.motion=="Wait" || selected.motion=="Walk" || selected.motion=="AttackA_Ready" || selected.motion=="AttackA_Fire")){
                // In all four source motions the lowest mesh is the foot. Each
                // frame has two pixels of packing margin. Keep the same two-pixel
                // boot outline below the support as the standing frame.
                rootY+=Mathf.Max(0,selected.sprite.pivot.y-4)*definition.scale/selected.sprite.pixelsPerUnit;
            }
            return RenderEnemyPosition(selected.actor)+new Vector2(definition.rootOffsetX*MonsterPixelFacingV53(selected),rootY)*selected.actor.visualScale + Vector2.up * MonsterBurialOffsetV57(selected);
        }
        MonsterPixelViewV53 RequireMonsterPixelViewV53(MonsterSelectionV53 selected){
            if(!monsterPixelViewsV53.TryGetValue(selected.actor,out var view)){
                view=new MonsterPixelViewV53(selected.carrier,DestroyNativeWorldObject);monsterPixelViewsV53.Add(selected.actor,view);monsterPixelRenderersV53.Add(view.renderer);
            }
            return view;
        }
        bool TryMonsterPixelRendererV53(Enemy actor,out SpriteRenderer renderer){
            renderer=null;if(actor==null||!monsterPixelSelectionsV53.ContainsKey(actor)||!monsterPixelViewsV53.TryGetValue(actor,out var view))return false;
            renderer=view.renderer;return renderer&&renderer.enabled&&renderer.gameObject.activeInHierarchy&&renderer.sprite&&renderer.color.a>0;
        }
        CoreStopWholeEnvelopeV22 MonsterPixelStartEnvelopeV53(MonsterSelectionV53 selected){
            float scale=selected.definition.scale*selected.actor.visualScale;int facing=MonsterPixelFacingV53(selected);
            string key=selected.definition.root+"/"+selected.actor.npcState+"/"+selected.motion+"/"+scale.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"/"+facing+"/"+selected.rotationZ.ToString("R",System.Globalization.CultureInfo.InvariantCulture);
            if(monsterPixelStartEnvelopesV53.TryGetValue(key,out var envelope))return envelope;
            if(!selected.library.TryClip(selected.motion,out var clip))throw new InvalidOperationException("Missing source START motion "+selected.motion);
            envelope=new CoreStopWholeEnvelopeV22{npc=selected.actor.sourceNPC,state=selected.actor.npcState,motion=selected.motion,phaseCount=clip.frames.Length,wholeKeys=new string[clip.frames.Length],minX=double.PositiveInfinity,minY=double.PositiveInfinity,maxX=double.NegativeInfinity,maxY=double.NegativeInfinity};
            float units=scale/selected.library.data.pixelsPerUnit;var rotation=Quaternion.Euler(0,0,selected.rotationZ);
            for(int i=0;i<clip.frames.Length;i++){
                var frame=clip.frames[i];envelope.wholeKeys[i]="monster-v53/"+selected.actor.kind+"/"+selected.motion+"/"+i;
                foreach(float x in new[]{-frame.px,frame.w-frame.px})foreach(float y in new[]{-frame.py,frame.h-frame.py}){
                    var point=rotation*new Vector3(x*units*facing,y*units,0);
                    envelope.minX=Math.Min(envelope.minX,point.x);envelope.maxX=Math.Max(envelope.maxX,point.x);envelope.minY=Math.Min(envelope.minY,point.y);envelope.maxY=Math.Max(envelope.maxY,point.y);
                }
            }
            monsterPixelStartEnvelopesV53.Add(key,envelope);return envelope;
        }

        // The existing STOP bridge attaches its actual Camera draw witness to
        // this renderer. Prepare it before camera planning, then project the same
        // instance in UpdateMonsterPixelViewsV53. Never acknowledge a model ticket
        // here and never replace the already selected original source clock.
        void PrepareMonsterPixelDirectorFramesV53(){
            if((!coreEntrySessionV21&&!factoryProductSessionR1179)||!actorsRoot)return;
            PrepareMonsterPixelOwnersV53();
            foreach(var pair in monsterPixelSelectionsV53){
                var selected=pair.Value;var actor=pair.Key;
                bool factory=factoryRavenTicketV21!=null&&ReferenceEquals(factoryRavenTicketV21.Actor,actor)&&actor.npcState==factoryRavenTicketV21.InitialState;
                coreFourWholeFramesV21.TryGetValue(actor,out var source);
                if(!factory&&(source==null||source.renderedFrame!=Time.frameCount||source.state!=actor.npcState))continue;
                var view=RequireMonsterPixelViewV53(selected);var logical=MonsterPixelLogicalRootV53(selected);
                view.Apply(selected.sprite,actorsRoot.TransformPoint(new Vector3(logical.x,logical.y,0)),selected.definition.scale*actor.visualScale,MonsterPixelFacingV53(selected),actor.active&&!actor.dormant,selected.rotationZ);
                if(factory){
                    factoryRavenEnvelopeV21=MonsterPixelStartEnvelopeV53(selected);
                    RecordFactoryRavenIntroSelectionV21(actor,view.renderer);
                    if(factoryRavenFramingFrameV35!=null)factoryRavenFramingFrameV35.logicalActorRootV22=new Vector3(logical.x,logical.y,0);
                    ObserveMonsterPixelV53(actor,"Factory/STOP-actual-draw",selected.alias,selected.motion,selected.clock,"director-bound","",selected.frame,selected.page);
                    continue;
                }
                var clip=selected.library.data;var rect=selected.sprite.rect;var pivot=selected.sprite.pivot;
                coreFourWholeFramesV21[actor]=new CoreStopSourceActorFrameV21{
                    actor=actor,root=view.renderer.gameObject,renderers=new Renderer[]{view.renderer},state=source.state,pose=selected.sprite.name,renderedFrame=Time.frameCount,
                    wholeStartEnvelopeV22=source.wholeStartEnvelopeV22==null?null:MonsterPixelStartEnvelopeV53(selected),logicalActorRootV22=new Vector3(logical.x,logical.y,0),
                    wholeKeyV22=selected.sprite.name,signedAnchorXV22=pivot.x/clip.pixelsPerUnit,signedAnchorYV22=(rect.height-pivot.y)/clip.pixelsPerUnit
                };
                ObserveMonsterPixelV53(actor,"Core/STOP-actual-draw",selected.alias,selected.motion,selected.clock,"director-bound","",selected.frame,selected.page);
            }
        }
    }
}
