using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly List<SpriteRenderer> npcEffectViews=new List<SpriteRenderer>();
        readonly Dictionary<Transform,float> npcEffectDepths=new Dictionary<Transform,float>();
        struct NpcEffectSnapshot {public Vector3 position;public float age,angle;public bool reflected;}
        readonly Dictionary<NpcEffectInstance,NpcEffectSnapshot> previousNpcEffects=new Dictionary<NpcEffectInstance,NpcEffectSnapshot>();
        void RememberNpcEffectState(){previousNpcEffects.Clear();foreach(var effect in game.npcEffects)previousNpcEffects[effect]=new NpcEffectSnapshot{position=new Vector3(effect.x,effect.y,effect.z),age=effect.age,angle=game.NpcEffectVisual(effect).angle,reflected=effect.reflected};}
        void RenderNpcEffects(){
            int visible=0;
            foreach(var effect in game.npcEffects){float age=effect.age;var position=new Vector3(effect.x,effect.y,effect.z);NpcEffectSnapshot previous;bool interpolate=!calibrationRun&&previousRoom==game.room&&previousNpcEffects.TryGetValue(effect,out previous)&&previous.reflected==effect.reflected&&effect.age>=previous.age;
                if(interpolate){previous=previousNpcEffects[effect];age=Mathf.Lerp(previous.age,effect.age,RenderAlpha);position=Vector3.Lerp(previous.position,position,RenderAlpha);}
                var art=game.NpcEffectVisual(effect,age);if(string.IsNullOrEmpty(art.id))continue;
                LoadRubenEffectArtV45(art.id);
                if(!libraries.ContainsKey(art.id)){
#if UNITY_EDITOR
                    if(forestPreviewActive)continue;
#endif
                    throw new InvalidOperationException("Missing admitted NPC effect library "+art.id);
                }
                if(visible>=npcEffectViews.Count)npcEffectViews.Add(WorldSprite("Source NPC effect",effectsRoot,null,44,Vector3.zero));
                var sprite=npcEffectViews[visible++];sprite.gameObject.SetActive(true);Pose(sprite,art.id,"play",age);sprite.flipX=effect.facing<0;if(interpolate)art.angle=Mathf.LerpAngle(previousNpcEffects[effect].angle,art.angle,RenderAlpha);
                var scale=new Vector2(art.scaleX,art.scaleY)*libraries[art.id].data.logicalScale;var offset=Vector2.Scale(SpriteOffset(sprite),scale);
                var rotation=Quaternion.Euler(0,0,art.angle);var rotatedOffset=rotation*new Vector3(offset.x,offset.y,0);
                sprite.transform.localPosition=new Vector3(position.x+rotatedOffset.x,position.y+rotatedOffset.y,0);sprite.transform.localRotation=rotation;sprite.transform.localScale=new Vector3(scale.x,scale.y,1);sprite.color=new Color(1,1,1,art.alpha);
                npcEffectDepths[sprite.transform]=(effect.owner==null?0:effect.owner.nativeDepth)+position.z/(NativeWorldActive?nativeScene.unit:.28f);
            }
            RenderNasodSprayBallsV35();
            for(int i=visible;i<npcEffectViews.Count;i++)npcEffectViews[i].gameObject.SetActive(false);
            if(CoreHardProductSessionActiveR1147){CoreHardClosureVisualRequireV21(coreHardClosureVisualDocumentV21!=null&&coreHardClosureVisualEnabledV21&&worldRoot&&effectsRoot,"Active Core product closure visual ownership/configuration missing");RenderCoreHardClosureVisualV21();}
            if(BattleProductSessionActiveR1325){RenderBattleProductPresentationR1325();}
            else{
            RenderNpcSteamVisual();
            RenderOutskirtsNpcVisual();
            RenderAlteraAirshipStateVisualV20();
            RenderLakeNpcVisual();
            RenderDragonNestMainMeshesV20();
            RenderDragonNestEffectParticlesV20();RenderMine2VisualV21();RenderBelowVisualV21();
            }
            RenderMine2ProductWorldLayerR1233();RenderCargoProductFxR1288();RenderFactorySheriffVisualV32();RenderPlainShockParticlesR1480();
#if UNITY_EDITOR
            RenderMine2EntryWorldV21();
#endif

            if(!BattleProductSessionActiveR1325)RenderDragonNestCountdownV20();
#if UNITY_EDITOR
            if(forestPreviewActive)RenderForestPreviewHelpers();
#endif
        }
    }
}
